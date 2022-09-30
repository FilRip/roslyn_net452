using System.Collections.Generic;

using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class OperationMapBuilder
    {
        private sealed class Walker : OperationWalker<Dictionary<SyntaxNode, IOperation>>
        {
            internal static readonly Walker Instance = new Walker();

            public override object? DefaultVisit(IOperation operation, Dictionary<SyntaxNode, IOperation> argument)
            {
                RecordOperation(operation, argument);
                return base.DefaultVisit(operation, argument);
            }

            public override object? VisitBinaryOperator([System.Diagnostics.CodeAnalysis.DisallowNull] IBinaryOperation? operation, Dictionary<SyntaxNode, IOperation> argument)
            {
                while (true)
                {
                    RecordOperation(operation, argument);
                    Visit(operation!.RightOperand, argument);
                    if (!(operation!.LeftOperand is IBinaryOperation binaryOperation))
                    {
                        break;
                    }
                    operation = binaryOperation;
                }
                Visit(operation!.LeftOperand, argument);
                return null;
            }

            internal override object? VisitNoneOperation(IOperation operation, Dictionary<SyntaxNode, IOperation> argument)
            {
                return DefaultVisit(operation, argument);
            }

            private static void RecordOperation(IOperation operation, Dictionary<SyntaxNode, IOperation> argument)
            {
                if (!operation.IsImplicit)
                {
                    argument.Add(operation.Syntax, operation);
                }
            }
        }

        public static void AddToMap(IOperation root, Dictionary<SyntaxNode, IOperation> dictionary)
        {
            Walker.Instance.Visit(root, dictionary);
        }
    }
}
