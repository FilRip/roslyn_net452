using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConvertedTupleLiteral : BoundTupleExpression
    {
        public BoundTupleLiteral? SourceTuple { get; }

        public bool WasTargetTyped { get; }

        public BoundConvertedTupleLiteral(SyntaxNode syntax, BoundTupleLiteral? sourceTuple, bool wasTargetTyped, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ConvertedTupleLiteral, syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, hasErrors || sourceTuple.HasErrors() || arguments.HasErrors())
        {
            SourceTuple = sourceTuple;
            WasTargetTyped = wasTargetTyped;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConvertedTupleLiteral(this);
        }

        public BoundConvertedTupleLiteral Update(BoundTupleLiteral? sourceTuple, bool wasTargetTyped, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type)
        {
            if (sourceTuple != SourceTuple || wasTargetTyped != WasTargetTyped || arguments != base.Arguments || argumentNamesOpt != base.ArgumentNamesOpt || inferredNamesOpt != base.InferredNamesOpt || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConvertedTupleLiteral boundConvertedTupleLiteral = new BoundConvertedTupleLiteral(Syntax, sourceTuple, wasTargetTyped, arguments, argumentNamesOpt, inferredNamesOpt, type, base.HasErrors);
                boundConvertedTupleLiteral.CopyAttributes(this);
                return boundConvertedTupleLiteral;
            }
            return this;
        }
    }
}
