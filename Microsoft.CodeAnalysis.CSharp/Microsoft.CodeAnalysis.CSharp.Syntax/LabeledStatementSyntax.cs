#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LabeledStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LabeledStatementSyntax)base.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LabeledStatementSyntax)base.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        public StatementSyntax Statement => GetRed(ref statement, 3);

        public LabeledStatementSyntax Update(SyntaxToken identifier, SyntaxToken colonToken, StatementSyntax statement)
        {
            return Update(AttributeLists, identifier, colonToken, statement);
        }

        internal LabeledStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref statement, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLabeledStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLabeledStatement(this);

        public LabeledStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, SyntaxToken colonToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || identifier != Identifier || colonToken != ColonToken || statement != Statement)
            {
                LabeledStatementSyntax labeledStatementSyntax = SyntaxFactory.LabeledStatement(attributeLists, identifier, colonToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return labeledStatementSyntax;
                }
                return labeledStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new LabeledStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Identifier, ColonToken, Statement);
        }

        public LabeledStatementSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, identifier, ColonToken, Statement);
        }

        public LabeledStatementSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(AttributeLists, Identifier, colonToken, Statement);
        }

        public LabeledStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, Identifier, ColonToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new LabeledStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
