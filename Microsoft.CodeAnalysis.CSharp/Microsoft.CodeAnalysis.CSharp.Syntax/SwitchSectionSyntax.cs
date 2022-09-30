#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SwitchSectionSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? labels;

        private SyntaxNode? statements;

        public SyntaxList<SwitchLabelSyntax> Labels => new SyntaxList<SwitchLabelSyntax>(GetRed(ref labels, 0));

        public SyntaxList<StatementSyntax> Statements => new SyntaxList<StatementSyntax>(GetRed(ref statements, 1));

        internal SwitchSectionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref labels),
                1 => GetRed(ref statements, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => labels,
                1 => statements,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchSection(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchSection(this);

        public SwitchSectionSyntax Update(SyntaxList<SwitchLabelSyntax> labels, SyntaxList<StatementSyntax> statements)
        {
            if (labels != Labels || statements != Statements)
            {
                SwitchSectionSyntax switchSectionSyntax = SyntaxFactory.SwitchSection(labels, statements);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return switchSectionSyntax;
                }
                return switchSectionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SwitchSectionSyntax WithLabels(SyntaxList<SwitchLabelSyntax> labels)
        {
            return Update(labels, Statements);
        }

        public SwitchSectionSyntax WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return Update(Labels, statements);
        }

        public SwitchSectionSyntax AddLabels(params SwitchLabelSyntax[] items)
        {
            return WithLabels(Labels.AddRange(items));
        }

        public SwitchSectionSyntax AddStatements(params StatementSyntax[] items)
        {
            return WithStatements(Statements.AddRange(items));
        }
    }
}
