#nullable enable

#nullable enable

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
    public sealed class IfStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? condition;

        private StatementSyntax? statement;

        private ElseClauseSyntax? @else;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IfStatementSyntax)base.Green).ifKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IfStatementSyntax)base.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Condition => GetRed(ref condition, 3);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IfStatementSyntax)base.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref statement, 5);

        public ElseClauseSyntax? Else => GetRed(ref @else, 6);

        public IfStatementSyntax Update(SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax @else)
        {
            return Update(AttributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
        }

        internal IfStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref condition, 3),
                5 => GetRed(ref statement, 5),
                6 => GetRed(ref @else, 6),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => condition,
                5 => statement,
                6 => @else,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIfStatement(this);

        public IfStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else)
        {
            if (attributeLists != AttributeLists || ifKeyword != IfKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || statement != Statement || @else != Else)
            {
                IfStatementSyntax ifStatementSyntax = SyntaxFactory.IfStatement(attributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return ifStatementSyntax;
                }
                return ifStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new IfStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, IfKeyword, OpenParenToken, Condition, CloseParenToken, Statement, Else);
        }

        public IfStatementSyntax WithIfKeyword(SyntaxToken ifKeyword)
        {
            return Update(AttributeLists, ifKeyword, OpenParenToken, Condition, CloseParenToken, Statement, Else);
        }

        public IfStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, IfKeyword, openParenToken, Condition, CloseParenToken, Statement, Else);
        }

        public IfStatementSyntax WithCondition(ExpressionSyntax condition)
        {
            return Update(AttributeLists, IfKeyword, OpenParenToken, condition, CloseParenToken, Statement, Else);
        }

        public IfStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, IfKeyword, OpenParenToken, Condition, closeParenToken, Statement, Else);
        }

        public IfStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, IfKeyword, OpenParenToken, Condition, CloseParenToken, statement, Else);
        }

        public IfStatementSyntax WithElse(ElseClauseSyntax? @else)
        {
            return Update(AttributeLists, IfKeyword, OpenParenToken, Condition, CloseParenToken, Statement, @else);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new IfStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
