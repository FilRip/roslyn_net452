using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordPrintMembers : SynthesizedRecordOrdinaryMethod
    {
        public SynthesizedRecordPrintMembers(SourceMemberContainerTypeSymbol containingType, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "PrintMembers", hasBody: true, memberOffset, diagnostics)
        {
        }

        protected override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            DeclarationModifiers declarationModifiers = ((ContainingType.IsRecordStruct || (ContainingType.BaseTypeNoUseSiteDiagnostics.IsObjectType() && ContainingType.IsSealed)) ? DeclarationModifiers.Private : DeclarationModifiers.Protected);
            if (ContainingType.IsRecord && !ContainingType.BaseTypeNoUseSiteDiagnostics.IsObjectType())
            {
                return declarationModifiers | DeclarationModifiers.Override;
            }
            return declarationModifiers | ((!ContainingType.IsSealed) ? DeclarationModifiers.Virtual : DeclarationModifiers.None);
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            NullableAnnotation nullableAnnotation = (ContainingType.IsRecordStruct ? NullableAnnotation.Oblivious : NullableAnnotation.NotAnnotated);
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Boolean, returnTypeLocation, diagnostics)), ImmutableArray.Create((ParameterSymbol)new SourceSimpleParameterSymbol(this, TypeWithAnnotations.Create(Binder.GetWellKnownType(declaringCompilation, WellKnownType.System_Text_StringBuilder, diagnostics, returnTypeLocation), nullableAnnotation), 0, RefKind.None, "builder", Locations)), false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 1;
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            base.MethodChecks(diagnostics);
            MethodSymbol overriddenMethod = base.OverriddenMethod;
            if ((object)overriddenMethod != null && !overriddenMethod.ContainingType.Equals(ContainingType.BaseTypeNoUseSiteDiagnostics, TypeCompareKind.AllIgnoreOptions))
            {
                diagnostics.Add(ErrorCode.ERR_DoesNotOverrideBaseMethod, Locations[0], this, ContainingType.BaseTypeNoUseSiteDiagnostics);
            }
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, ContainingType.GetNonNullSyntaxNode(), compilationState, diagnostics);
            try
            {
                ImmutableArray<Symbol> immutableArray = ContainingType.GetMembers().WhereAsArray((Symbol m) => isPrintable(m));
                if (base.ReturnType.IsErrorType() || immutableArray.Any((Symbol m) => m.GetTypeOrReturnType().Type.IsErrorType()))
                {
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                    return;
                }
                BoundParameter boundParameter = syntheticBoundNodeFactory.Parameter(Parameters[0]);
                ArrayBuilder<BoundStatement> instance;
                if (ContainingType.BaseTypeNoUseSiteDiagnostics.IsObjectType() || ContainingType.IsRecordStruct)
                {
                    if (immutableArray.IsEmpty)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Literal(value: false)));
                        return;
                    }
                    instance = ArrayBuilder<BoundStatement>.GetInstance();
                }
                else
                {
                    MethodSymbol overriddenMethod = base.OverriddenMethod;
                    if ((object)overriddenMethod == null || overriddenMethod.ReturnType.SpecialType != SpecialType.System_Boolean)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    BoundCall boundCall = syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Base(ContainingType.BaseTypeNoUseSiteDiagnostics), overriddenMethod, boundParameter);
                    if (immutableArray.IsEmpty)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Return(boundCall));
                        return;
                    }
                    instance = ArrayBuilder<BoundStatement>.GetInstance();
                    instance.Add(syntheticBoundNodeFactory.If(boundCall, makeAppendString(syntheticBoundNodeFactory, boundParameter, ", ")));
                }
                for (int i = 0; i < immutableArray.Length; i++)
                {
                    Symbol symbol = immutableArray[i];
                    instance.Add(makeAppendString(syntheticBoundNodeFactory, boundParameter, symbol.Name));
                    instance.Add(makeAppendString(syntheticBoundNodeFactory, boundParameter, " = "));
                    BoundExpression boundExpression = symbol.Kind switch
                    {
                        SymbolKind.Field => syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), (FieldSymbol)symbol),
                        SymbolKind.Property => syntheticBoundNodeFactory.Property(syntheticBoundNodeFactory.This(), (PropertySymbol)symbol),
                        _ => throw ExceptionUtilities.UnexpectedValue(symbol.Kind),
                    };
                    if (boundExpression.Type!.IsValueType)
                    {
                        instance.Add(syntheticBoundNodeFactory.ExpressionStatement(syntheticBoundNodeFactory.Call(boundParameter, syntheticBoundNodeFactory.WellKnownMethod(WellKnownMember.System_Text_StringBuilder__AppendString), syntheticBoundNodeFactory.Call(boundExpression, syntheticBoundNodeFactory.SpecialMethod(SpecialMember.System_Object__ToString)))));
                    }
                    else
                    {
                        instance.Add(syntheticBoundNodeFactory.ExpressionStatement(syntheticBoundNodeFactory.Call(boundParameter, syntheticBoundNodeFactory.WellKnownMethod(WellKnownMember.System_Text_StringBuilder__AppendObject), syntheticBoundNodeFactory.Convert(syntheticBoundNodeFactory.SpecialType(SpecialType.System_Object), boundExpression))));
                    }
                    if (i < immutableArray.Length - 1)
                    {
                        instance.Add(makeAppendString(syntheticBoundNodeFactory, boundParameter, ", "));
                    }
                }
                instance.Add(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Literal(value: true)));
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree()));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
            static bool isPrintable(Symbol m)
            {
                if (m.DeclaredAccessibility != Accessibility.Public || m.IsStatic)
                {
                    return false;
                }
                if (m.Kind == SymbolKind.Field)
                {
                    return true;
                }
                if (m.Kind == SymbolKind.Property)
                {
                    PropertySymbol propertySymbol = (PropertySymbol)m;
                    if (!propertySymbol.IsIndexer && !propertySymbol.IsOverride)
                    {
                        return (object)propertySymbol.GetMethod != null;
                    }
                    return false;
                }
                return false;
            }
            static BoundStatement makeAppendString(SyntheticBoundNodeFactory F, BoundParameter builder, string value)
            {
                return F.ExpressionStatement(F.Call(builder, F.WellKnownMethod(WellKnownMember.System_Text_StringBuilder__AppendString), F.StringLiteral(value)));
            }
        }

        internal static void VerifyOverridesPrintMembersFromBase(MethodSymbol overriding, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = overriding.ContainingType.BaseTypeNoUseSiteDiagnostics;
            if (baseTypeNoUseSiteDiagnostics.IsObjectType())
            {
                return;
            }
            bool flag = false;
            if (!overriding.IsOverride)
            {
                flag = true;
            }
            else
            {
                MethodSymbol overriddenMethod = overriding.OverriddenMethod;
                if ((object)overriddenMethod != null && !overriddenMethod.ContainingType.Equals(baseTypeNoUseSiteDiagnostics, TypeCompareKind.AllIgnoreOptions))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                diagnostics.Add(ErrorCode.ERR_DoesNotOverrideBaseMethod, overriding.Locations[0], overriding, baseTypeNoUseSiteDiagnostics);
            }
        }
    }
}
