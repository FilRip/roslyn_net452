using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordObjEquals : SynthesizedRecordObjectMethod
    {
        private readonly MethodSymbol _typedRecordEquals;

        protected override SpecialMember OverriddenSpecialMember => SpecialMember.System_Object__Equals;

        public SynthesizedRecordObjEquals(SourceMemberContainerTypeSymbol containingType, MethodSymbol typedRecordEquals, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "Equals", memberOffset, diagnostics)
        {
            _typedRecordEquals = typedRecordEquals;
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            NullableAnnotation nullableAnnotation = (ContainingType.IsRecordStruct ? NullableAnnotation.Oblivious : NullableAnnotation.Annotated);
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Boolean, returnTypeLocation, diagnostics)), ImmutableArray.Create((ParameterSymbol)new SourceSimpleParameterSymbol(this, TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Object, returnTypeLocation, diagnostics), nullableAnnotation), 0, RefKind.None, "obj", Locations)), false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 1;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, SyntaxNode, compilationState, diagnostics);
            try
            {
                if (_typedRecordEquals.ReturnType.SpecialType != SpecialType.System_Boolean)
                {
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                    return;
                }
                BoundParameter boundParameter = syntheticBoundNodeFactory.Parameter(Parameters[0]);
                BoundExpression expression = ((!ContainingType.IsRecordStruct) ? syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.This(), _typedRecordEquals, syntheticBoundNodeFactory.As(boundParameter, ContainingType)) : ((BoundExpression)syntheticBoundNodeFactory.LogicalAnd(syntheticBoundNodeFactory.Is(boundParameter, ContainingType), syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.This(), _typedRecordEquals, syntheticBoundNodeFactory.Convert(ContainingType, boundParameter)))));
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(ImmutableArray.Create((BoundStatement)syntheticBoundNodeFactory.Return(expression))));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
        }
    }
}
