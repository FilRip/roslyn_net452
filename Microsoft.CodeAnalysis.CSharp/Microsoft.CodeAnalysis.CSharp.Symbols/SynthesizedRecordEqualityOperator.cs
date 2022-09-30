using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordEqualityOperator : SynthesizedRecordEqualityOperatorBase
    {
        public SynthesizedRecordEqualityOperator(SourceMemberContainerTypeSymbol containingType, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "op_Equality", memberOffset, diagnostics)
        {
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, ContainingType.GetNonNullSyntaxNode(), compilationState, diagnostics);
            try
            {
                MethodSymbol methodSymbol = null;
                ImmutableArray<Symbol>.Enumerator enumerator = ContainingType.GetMembers("Equals").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is MethodSymbol methodSymbol2 && methodSymbol2.ParameterCount == 1 && methodSymbol2.Parameters[0].RefKind == RefKind.None && methodSymbol2.ReturnType.SpecialType == SpecialType.System_Boolean && !methodSymbol2.IsStatic && methodSymbol2.Parameters[0].Type.Equals(ContainingType, TypeCompareKind.AllIgnoreOptions))
                    {
                        methodSymbol = methodSymbol2;
                        break;
                    }
                }
                if ((object)methodSymbol == null)
                {
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                    return;
                }
                BoundParameter boundParameter = syntheticBoundNodeFactory.Parameter(Parameters[0]);
                BoundParameter boundParameter2 = syntheticBoundNodeFactory.Parameter(Parameters[1]);
                BoundExpression expression;
                if (ContainingType.IsRecordStruct)
                {
                    expression = syntheticBoundNodeFactory.Call(boundParameter, methodSymbol, boundParameter2);
                }
                else
                {
                    BoundExpression left = syntheticBoundNodeFactory.ObjectEqual(boundParameter, boundParameter2);
                    BoundExpression right = syntheticBoundNodeFactory.LogicalAnd(syntheticBoundNodeFactory.ObjectNotEqual(boundParameter, syntheticBoundNodeFactory.Null(syntheticBoundNodeFactory.SpecialType(SpecialType.System_Object))), syntheticBoundNodeFactory.Call(boundParameter, methodSymbol, boundParameter2));
                    expression = syntheticBoundNodeFactory.LogicalOr(left, right);
                }
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(expression)));
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
            }
        }
    }
}
