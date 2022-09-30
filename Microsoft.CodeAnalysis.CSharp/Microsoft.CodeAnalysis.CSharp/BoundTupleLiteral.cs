using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTupleLiteral : BoundTupleExpression
    {
        public new TypeSymbol? Type => base.Type;

        public BoundTupleLiteral(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.TupleLiteral, syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, hasErrors || arguments.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTupleLiteral(this);
        }

        public BoundTupleLiteral Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type)
        {
            if (arguments != base.Arguments || argumentNamesOpt != base.ArgumentNamesOpt || inferredNamesOpt != base.InferredNamesOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTupleLiteral boundTupleLiteral = new BoundTupleLiteral(Syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, base.HasErrors);
                boundTupleLiteral.CopyAttributes(this);
                return boundTupleLiteral;
            }
            return this;
        }
    }
}
