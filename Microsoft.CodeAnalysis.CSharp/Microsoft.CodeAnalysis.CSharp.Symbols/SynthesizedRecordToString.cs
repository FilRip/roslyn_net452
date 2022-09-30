using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordToString : SynthesizedRecordObjectMethod
    {
        private readonly MethodSymbol _printMethod;

        protected override SpecialMember OverriddenSpecialMember => SpecialMember.System_Object__ToString;

        public SynthesizedRecordToString(SourceMemberContainerTypeSymbol containingType, MethodSymbol printMethod, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "ToString", memberOffset, diagnostics)
        {
            _printMethod = printMethod;
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            return (TypeWithAnnotations.Create(nullableAnnotation: ContainingType.IsRecordStruct ? NullableAnnotation.Oblivious : NullableAnnotation.NotAnnotated, typeSymbol: Binder.GetSpecialType(declaringCompilation, SpecialType.System_String, returnTypeLocation, diagnostics)), ImmutableArray<ParameterSymbol>.Empty, false, ImmutableArray<TypeParameterConstraintClause>.Empty);
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
                _ = ContainingType.DeclaringCompilation;
                NamedTypeSymbol type = syntheticBoundNodeFactory.WellKnownType(WellKnownType.System_Text_StringBuilder);
                MethodSymbol ctor = syntheticBoundNodeFactory.WellKnownMethod(WellKnownMember.System_Text_StringBuilder__ctor);
                LocalSymbol localSymbol = syntheticBoundNodeFactory.SynthesizedLocal(type);
                BoundLocal boundLocal = syntheticBoundNodeFactory.Local(localSymbol);
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                instance.Add(syntheticBoundNodeFactory.Assignment(boundLocal, syntheticBoundNodeFactory.New(ctor)));
                instance.Add(makeAppendString(syntheticBoundNodeFactory, boundLocal, ContainingType.Name));
                instance.Add(makeAppendString(syntheticBoundNodeFactory, boundLocal, " { "));
                instance.Add(syntheticBoundNodeFactory.If(syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.This(), _printMethod, boundLocal), makeAppendString(syntheticBoundNodeFactory, boundLocal, " ")));
                instance.Add(makeAppendString(syntheticBoundNodeFactory, boundLocal, "}"));
                instance.Add(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Call(boundLocal, syntheticBoundNodeFactory.SpecialMethod(SpecialMember.System_Object__ToString))));
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree()));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
            static BoundStatement makeAppendString(SyntheticBoundNodeFactory F, BoundLocal builder, string value)
            {
                return F.ExpressionStatement(F.Call(builder, F.WellKnownMethod(WellKnownMember.System_Text_StringBuilder__AppendString), F.StringLiteral(value)));
            }
        }
    }
}
