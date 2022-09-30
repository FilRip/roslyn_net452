using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordEquals : SynthesizedRecordOrdinaryMethod
    {
        private readonly PropertySymbol? _equalityContract;

        public SynthesizedRecordEquals(SourceMemberContainerTypeSymbol containingType, PropertySymbol? equalityContract, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "Equals", hasBody: true, memberOffset, diagnostics)
        {
            _equalityContract = equalityContract;
        }

        protected override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            return DeclarationModifiers.Public | ((!ContainingType.IsSealed) ? DeclarationModifiers.Virtual : DeclarationModifiers.None);
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            NullableAnnotation nullableAnnotation = (ContainingType.IsRecordStruct ? NullableAnnotation.Oblivious : NullableAnnotation.Annotated);
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Boolean, returnTypeLocation, diagnostics)), ImmutableArray.Create((ParameterSymbol)new SourceSimpleParameterSymbol(this, TypeWithAnnotations.Create(ContainingType, nullableAnnotation), 0, RefKind.None, "other", Locations)), false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 1;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, ContainingType.GetNonNullSyntaxNode(), compilationState, diagnostics);
            try
            {
                BoundParameter boundParameter = syntheticBoundNodeFactory.Parameter(Parameters[0]);
                bool isRecordStruct = ContainingType.IsRecordStruct;
                BoundExpression boundExpression;
                if (isRecordStruct)
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
                    if (_equalityContract!.IsStatic || !_equalityContract!.Type.Equals(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.AllIgnoreOptions))
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    boundExpression = syntheticBoundNodeFactory.ObjectNotEqual(boundParameter, syntheticBoundNodeFactory.Null(syntheticBoundNodeFactory.SpecialType(SpecialType.System_Object)));
                    BoundCall right = syntheticBoundNodeFactory.Call(null, syntheticBoundNodeFactory.WellKnownMethod(WellKnownMember.System_Type__op_Equality), syntheticBoundNodeFactory.Property(syntheticBoundNodeFactory.This(), _equalityContract), syntheticBoundNodeFactory.Property(boundParameter, _equalityContract));
                    boundExpression = syntheticBoundNodeFactory.LogicalAnd(boundExpression, right);
                }
                else
                {
                    MethodSymbol overriddenMethod = ContainingType.GetMembersUnordered().OfType<SynthesizedRecordBaseEquals>().Single()
                        .OverriddenMethod;
                    if ((object)overriddenMethod == null || !overriddenMethod.ContainingType.Equals(ContainingType.BaseTypeNoUseSiteDiagnostics, TypeCompareKind.AllIgnoreOptions) || overriddenMethod.ReturnType.SpecialType != SpecialType.System_Boolean)
                    {
                        syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                        return;
                    }
                    boundExpression = syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Base(overriddenMethod.ContainingType), overriddenMethod, syntheticBoundNodeFactory.Convert(overriddenMethod.Parameters[0].Type, boundParameter));
                }
                ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance();
                bool flag = false;
                foreach (FieldSymbol item in ContainingType.GetFieldsToEmit())
                {
                    if (!item.IsStatic)
                    {
                        instance.Add(item);
                        TypeSymbol type = item.Type;
                        if (type.IsUnsafe())
                        {
                            diagnostics.Add(ErrorCode.ERR_BadFieldTypeInRecord, item.Locations.FirstOrNone(), type);
                            flag = true;
                        }
                        else if (type.IsRestrictedType())
                        {
                            flag = true;
                        }
                    }
                }
                if (instance.Count > 0 && !flag)
                {
                    boundExpression = MethodBodySynthesizer.GenerateFieldEquals(boundExpression, boundParameter, instance, syntheticBoundNodeFactory);
                }
                else if (boundExpression == null)
                {
                    boundExpression = syntheticBoundNodeFactory.Literal(value: true);
                }
                instance.Free();
                if (!isRecordStruct)
                {
                    boundExpression = syntheticBoundNodeFactory.LogicalOr(syntheticBoundNodeFactory.ObjectEqual(syntheticBoundNodeFactory.This(), boundParameter), boundExpression);
                }
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(boundExpression)));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
        }
    }
}
