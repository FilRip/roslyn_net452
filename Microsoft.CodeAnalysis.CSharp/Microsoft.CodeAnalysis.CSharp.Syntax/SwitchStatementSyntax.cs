#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SwitchStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        private SyntaxNode? sections;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken SwitchKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchStatementSyntax)base.Green).switchKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken openParenToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchStatementSyntax)base.Green).openParenToken;
                if (openParenToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, openParenToken, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public ExpressionSyntax Expression => GetRed(ref expression, 3);

        public SyntaxToken CloseParenToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken closeParenToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchStatementSyntax)base.Green).closeParenToken;
                if (closeParenToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, closeParenToken, GetChildPosition(4), GetChildIndex(4));
            }
        }

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchStatementSyntax)base.Green).openBraceToken, GetChildPosition(5), GetChildIndex(5));

        public SyntaxList<SwitchSectionSyntax> Sections => new SyntaxList<SwitchSectionSyntax>(GetRed(ref sections, 6));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchStatementSyntax)base.Green).closeBraceToken, GetChildPosition(7), GetChildIndex(7));

        public SwitchStatementSyntax Update(SyntaxToken switchKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, switchKeyword, openParenToken, expression, closeParenToken, openBraceToken, sections, closeBraceToken);
        }

        internal SwitchStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref expression, 3),
                6 => GetRed(ref sections, 6),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => expression,
                6 => sections,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchStatement(this);

        public SwitchStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken switchKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            if (attributeLists != AttributeLists || switchKeyword != SwitchKeyword || openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken || openBraceToken != OpenBraceToken || sections != Sections || closeBraceToken != CloseBraceToken)
            {
                SwitchStatementSyntax switchStatementSyntax = SyntaxFactory.SwitchStatement(attributeLists, switchKeyword, openParenToken, expression, closeParenToken, openBraceToken, sections, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return switchStatementSyntax;
                }
                return switchStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new SwitchStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, SwitchKeyword, OpenParenToken, Expression, CloseParenToken, OpenBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithSwitchKeyword(SyntaxToken switchKeyword)
        {
            return Update(AttributeLists, switchKeyword, OpenParenToken, Expression, CloseParenToken, OpenBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, SwitchKeyword, openParenToken, Expression, CloseParenToken, OpenBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AttributeLists, SwitchKeyword, OpenParenToken, expression, CloseParenToken, OpenBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, SwitchKeyword, OpenParenToken, Expression, closeParenToken, OpenBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(AttributeLists, SwitchKeyword, OpenParenToken, Expression, CloseParenToken, openBraceToken, Sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithSections(SyntaxList<SwitchSectionSyntax> sections)
        {
            return Update(AttributeLists, SwitchKeyword, OpenParenToken, Expression, CloseParenToken, OpenBraceToken, sections, CloseBraceToken);
        }

        public SwitchStatementSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, SwitchKeyword, OpenParenToken, Expression, CloseParenToken, OpenBraceToken, Sections, closeBraceToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new SwitchStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public SwitchStatementSyntax AddSections(params SwitchSectionSyntax[] items)
        {
            return WithSections(Sections.AddRange(items));
        }
    }
}
