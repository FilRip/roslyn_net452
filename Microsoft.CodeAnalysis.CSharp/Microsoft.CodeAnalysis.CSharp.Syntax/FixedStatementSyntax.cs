#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FixedStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private VariableDeclarationSyntax? declaration;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken FixedKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FixedStatementSyntax)base.Green).fixedKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FixedStatementSyntax)base.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public VariableDeclarationSyntax Declaration => GetRed(ref declaration, 3);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FixedStatementSyntax)base.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref statement, 5);

        public FixedStatementSyntax Update(SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement);
        }

        internal FixedStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref declaration, 3),
                5 => GetRed(ref statement, 5),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => declaration,
                5 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFixedStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFixedStatement(this);

        public FixedStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || fixedKeyword != FixedKeyword || openParenToken != OpenParenToken || declaration != Declaration || closeParenToken != CloseParenToken || statement != Statement)
            {
                FixedStatementSyntax fixedStatementSyntax = SyntaxFactory.FixedStatement(attributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return fixedStatementSyntax;
                }
                return fixedStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new FixedStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, FixedKeyword, OpenParenToken, Declaration, CloseParenToken, Statement);
        }

        public FixedStatementSyntax WithFixedKeyword(SyntaxToken fixedKeyword)
        {
            return Update(AttributeLists, fixedKeyword, OpenParenToken, Declaration, CloseParenToken, Statement);
        }

        public FixedStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, FixedKeyword, openParenToken, Declaration, CloseParenToken, Statement);
        }

        public FixedStatementSyntax WithDeclaration(VariableDeclarationSyntax declaration)
        {
            return Update(AttributeLists, FixedKeyword, OpenParenToken, declaration, CloseParenToken, Statement);
        }

        public FixedStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, FixedKeyword, OpenParenToken, Declaration, closeParenToken, Statement);
        }

        public FixedStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, FixedKeyword, OpenParenToken, Declaration, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new FixedStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public FixedStatementSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items)
        {
            return WithDeclaration(Declaration.WithVariables(Declaration.Variables.AddRange(items)));
        }
    }
}
