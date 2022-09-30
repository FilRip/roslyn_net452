using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDeconstructionAssignmentOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create(Left, (BoundNode)Right);

        public new TypeSymbol Type => base.Type;

        public BoundTupleExpression Left { get; }

        public BoundConversion Right { get; }

        public bool IsUsed { get; }

        public BoundDeconstructionAssignmentOperator(SyntaxNode syntax, BoundTupleExpression left, BoundConversion right, bool isUsed, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DeconstructionAssignmentOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            Left = left;
            Right = right;
            IsUsed = isUsed;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDeconstructionAssignmentOperator(this);
        }

        public BoundDeconstructionAssignmentOperator Update(BoundTupleExpression left, BoundConversion right, bool isUsed, TypeSymbol type)
        {
            if (left != Left || right != Right || isUsed != IsUsed || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDeconstructionAssignmentOperator boundDeconstructionAssignmentOperator = new BoundDeconstructionAssignmentOperator(Syntax, left, right, isUsed, type, base.HasErrors);
                boundDeconstructionAssignmentOperator.CopyAttributes(this);
                return boundDeconstructionAssignmentOperator;
            }
            return this;
        }
    }
}
