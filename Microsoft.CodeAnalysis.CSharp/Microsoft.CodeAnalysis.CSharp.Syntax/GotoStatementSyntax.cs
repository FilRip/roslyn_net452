#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class GotoStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken GotoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.GotoStatementSyntax)base.Green).gotoKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken CaseOrDefaultKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken caseOrDefaultKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.GotoStatementSyntax)base.Green).caseOrDefaultKeyword;
                if (caseOrDefaultKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, caseOrDefaultKeyword, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public ExpressionSyntax? Expression => GetRed(ref expression, 3);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.GotoStatementSyntax)base.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        public GotoStatementSyntax Update(SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken);
        }

        internal GotoStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref expression, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGotoStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGotoStatement(this);

        public GotoStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || gotoKeyword != GotoKeyword || caseOrDefaultKeyword != CaseOrDefaultKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                GotoStatementSyntax gotoStatementSyntax = SyntaxFactory.GotoStatement(Kind(), attributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return gotoStatementSyntax;
                }
                return gotoStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new GotoStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, GotoKeyword, CaseOrDefaultKeyword, Expression, SemicolonToken);
        }

        public GotoStatementSyntax WithGotoKeyword(SyntaxToken gotoKeyword)
        {
            return Update(AttributeLists, gotoKeyword, CaseOrDefaultKeyword, Expression, SemicolonToken);
        }

        public GotoStatementSyntax WithCaseOrDefaultKeyword(SyntaxToken caseOrDefaultKeyword)
        {
            return Update(AttributeLists, GotoKeyword, caseOrDefaultKeyword, Expression, SemicolonToken);
        }

        public GotoStatementSyntax WithExpression(ExpressionSyntax? expression)
        {
            return Update(AttributeLists, GotoKeyword, CaseOrDefaultKeyword, expression, SemicolonToken);
        }

        public GotoStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, GotoKeyword, CaseOrDefaultKeyword, Expression, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new GotoStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
