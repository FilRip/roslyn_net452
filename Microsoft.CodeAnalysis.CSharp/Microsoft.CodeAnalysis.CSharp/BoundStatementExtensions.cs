using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class BoundStatementExtensions
    {
        [Conditional("DEBUG")]
        internal static void AssertIsLabeledStatement(this BoundStatement node)
        {
            BoundKind kind = node.Kind;
            if (kind != BoundKind.LabelStatement && kind != BoundKind.LabeledStatement && kind != BoundKind.SwitchSection)
            {
                throw ExceptionUtilities.UnexpectedValue(node.Kind);
            }
        }

        [Conditional("DEBUG")]
        internal static void AssertIsLabeledStatementWithLabel(this BoundStatement node, LabelSymbol label)
        {
            switch (node.Kind)
            {
                case BoundKind.SwitchSection:
                    {
                        ImmutableArray<BoundSwitchLabel>.Enumerator enumerator = ((BoundSwitchSection)node).SwitchLabels.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.Label == label)
                            {
                                return;
                            }
                        }
                        throw ExceptionUtilities.Unreachable;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind);
                case BoundKind.LabelStatement:
                case BoundKind.LabeledStatement:
                    break;
            }
        }
    }
}
