using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLeafDecisionDagNode : BoundDecisionDagNode
    {
        public LabelSymbol Label { get; }

        public BoundLeafDecisionDagNode(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
            : base(BoundKind.LeafDecisionDagNode, syntax, hasErrors)
        {
            Label = label;
        }

        public BoundLeafDecisionDagNode(SyntaxNode syntax, LabelSymbol label)
            : base(BoundKind.LeafDecisionDagNode, syntax)
        {
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLeafDecisionDagNode(this);
        }

        public BoundLeafDecisionDagNode Update(LabelSymbol label)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundLeafDecisionDagNode boundLeafDecisionDagNode = new BoundLeafDecisionDagNode(Syntax, label, base.HasErrors);
                boundLeafDecisionDagNode.CopyAttributes(this);
                return boundLeafDecisionDagNode;
            }
            return this;
        }
    }
}
