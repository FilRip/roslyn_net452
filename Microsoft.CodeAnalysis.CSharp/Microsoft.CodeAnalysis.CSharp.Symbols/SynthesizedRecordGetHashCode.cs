using System.Collections.Immutable;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordGetHashCode : SynthesizedRecordObjectMethod
    {
        private readonly PropertySymbol? _equalityContract;

        protected override SpecialMember OverriddenSpecialMember => SpecialMember.System_Object__GetHashCode;

        public SynthesizedRecordGetHashCode(SourceMemberContainerTypeSymbol containingType, PropertySymbol? equalityContract, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "GetHashCode", memberOffset, diagnostics)
        {
            _equalityContract = equalityContract;
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Int32, returnTypeLocation, diagnostics)), ImmutableArray<ParameterSymbol>.Empty, false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 0;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, SyntaxNode, compilationState, diagnostics);
            try
            {
                MethodSymbol equalityComparer_GetHashCode2 = null;
                MethodSymbol equalityComparer_get_Default2 = null;
                BoundExpression boundExpression;
                if (ContainingType.IsRecordStruct)
                {
                    boundExpression = null;
                }
                else if (ContainingType.BaseTypeNoUseSiteDiagnostics.IsObjectType())
                {
                    if ((object)_equalityContract!.GetMethod == null)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    if (_equalityContract!.IsStatic)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    ensureEqualityComparerHelpers(syntheticBoundNodeFactory, ref equalityComparer_GetHashCode2, ref equalityComparer_get_Default2);
                    boundExpression = MethodBodySynthesizer.GenerateGetHashCode(equalityComparer_GetHashCode2, equalityComparer_get_Default2, syntheticBoundNodeFactory.Property(syntheticBoundNodeFactory.This(), _equalityContract), syntheticBoundNodeFactory);
                }
                else
                {
                    MethodSymbol overriddenMethod = base.OverriddenMethod;
                    if ((object)overriddenMethod == null || overriddenMethod.ReturnType.SpecialType != SpecialType.System_Int32)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    boundExpression = syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Base(overriddenMethod.ContainingType), overriddenMethod);
                }
                BoundLiteral boundHashFactor = null;
                foreach (FieldSymbol item in ContainingType.GetFieldsToEmit())
                {
                    if (!item.IsStatic)
                    {
                        ensureEqualityComparerHelpers(syntheticBoundNodeFactory, ref equalityComparer_GetHashCode2, ref equalityComparer_get_Default2);
                        boundExpression = ((boundExpression != null) ? MethodBodySynthesizer.GenerateHashCombine(boundExpression, equalityComparer_GetHashCode2, equalityComparer_get_Default2, ref boundHashFactor, syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), item), syntheticBoundNodeFactory) : MethodBodySynthesizer.GenerateGetHashCode(equalityComparer_GetHashCode2, equalityComparer_get_Default2, syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), item), syntheticBoundNodeFactory));
                    }
                }
                if (boundExpression == null)
                {
                    boundExpression = syntheticBoundNodeFactory.Literal(0);
                }
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(boundExpression)));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
            static void ensureEqualityComparerHelpers(SyntheticBoundNodeFactory F, [System.Diagnostics.CodeAnalysis.NotNull] ref MethodSymbol? equalityComparer_GetHashCode, [System.Diagnostics.CodeAnalysis.NotNull] ref MethodSymbol? equalityComparer_get_Default)
            {
                if ((object)equalityComparer_GetHashCode == null)
                {
                    equalityComparer_GetHashCode = F.WellKnownMethod(WellKnownMember.System_Collections_Generic_EqualityComparer_T__GetHashCode);
                }
                if ((object)equalityComparer_get_Default == null)
                {
                    equalityComparer_get_Default = F.WellKnownMethod(WellKnownMember.System_Collections_Generic_EqualityComparer_T__get_Default);
                }
            }
        }
    }
}
