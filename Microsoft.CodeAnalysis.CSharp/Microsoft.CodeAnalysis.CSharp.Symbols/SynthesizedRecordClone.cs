using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordClone : SynthesizedRecordOrdinaryMethod
    {
        public SynthesizedRecordClone(SourceMemberContainerTypeSymbol containingType, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "<Clone>$", !containingType.IsAbstract, memberOffset, diagnostics)
        {
        }

        protected override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            DeclarationModifiers declarationModifiers = DeclarationModifiers.Public;
            declarationModifiers = (((object)VirtualCloneInBase() == null) ? (declarationModifiers | ((!ContainingType.IsSealed) ? DeclarationModifiers.Virtual : DeclarationModifiers.None)) : (declarationModifiers | DeclarationModifiers.Override));
            if (ContainingType.IsAbstract)
            {
                declarationModifiers &= ~DeclarationModifiers.Virtual;
                declarationModifiers |= DeclarationModifiers.Abstract;
            }
            return declarationModifiers;
        }

        private MethodSymbol? VirtualCloneInBase()
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = ContainingType.BaseTypeNoUseSiteDiagnostics;
            if (!baseTypeNoUseSiteDiagnostics.IsObjectType())
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                return FindValidCloneMethod(baseTypeNoUseSiteDiagnostics, ref useSiteInfo);
            }
            return null;
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            return (VirtualCloneInBase()?.ReturnTypeWithAnnotations ?? TypeWithAnnotations.Create(isNullableEnabled: true, ContainingType), ImmutableArray<ParameterSymbol>.Empty, false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 0;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, ContainingType.GetNonNullSyntaxNode(), compilationState, diagnostics);
            try
            {
                if (base.ReturnType.IsErrorType())
                {
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                    return;
                }
                ImmutableArray<MethodSymbol>.Enumerator enumerator = ContainingType.InstanceConstructors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MethodSymbol current = enumerator.Current;
                    if (current.ParameterCount == 1 && current.Parameters[0].RefKind == RefKind.None && current.Parameters[0].Type.Equals(ContainingType, TypeCompareKind.AllIgnoreOptions))
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.New(current, syntheticBoundNodeFactory.This())));
                        return;
                    }
                }
                throw ExceptionUtilities.Unreachable;
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
        }

        internal static MethodSymbol? FindValidCloneMethod(TypeSymbol containingType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (containingType.IsObjectType() || !(containingType is NamedTypeSymbol namedTypeSymbol))
            {
                return null;
            }
            if (!namedTypeSymbol.HasPossibleWellKnownCloneMethod())
            {
                return null;
            }
            MethodSymbol methodSymbol = null;
            ImmutableArray<Symbol>.Enumerator enumerator = containingType.GetMembers("<Clone>$").GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current is MethodSymbol methodSymbol2 && current.DeclaredAccessibility == Accessibility.Public && !current.IsStatic && methodSymbol2.ParameterCount == 0 && methodSymbol2.Arity == 0)
                {
                    if ((object)methodSymbol != null)
                    {
                        return null;
                    }
                    methodSymbol = methodSymbol2;
                }
            }
            if ((object)methodSymbol == null || (!containingType.IsSealed && !methodSymbol.IsOverride && !methodSymbol.IsVirtual && !methodSymbol.IsAbstract) || !containingType.IsEqualToOrDerivedFrom(methodSymbol.ReturnType, TypeCompareKind.AllIgnoreOptions, ref useSiteInfo))
            {
                return null;
            }
            return methodSymbol;
        }
    }
}
