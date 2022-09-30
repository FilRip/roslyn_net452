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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private VariableDeclarationSyntax? declaration;

        private SyntaxNode? initializers;

        private ExpressionSyntax? condition;

        private SyntaxNode? incrementors;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken ForKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForStatementSyntax)base.Green).forKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForStatementSyntax)base.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public VariableDeclarationSyntax? Declaration => GetRed(ref declaration, 3);

        public SeparatedSyntaxList<ExpressionSyntax> Initializers
        {
            get
            {
                SyntaxNode red = GetRed(ref initializers, 4);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ExpressionSyntax>);
                }
                return new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(4));
            }
        }

        public SyntaxToken FirstSemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForStatementSyntax)base.Green).firstSemicolonToken, GetChildPosition(5), GetChildIndex(5));

        public ExpressionSyntax? Condition => GetRed(ref condition, 6);

        public SyntaxToken SecondSemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForStatementSyntax)base.Green).secondSemicolonToken, GetChildPosition(7), GetChildIndex(7));

        public SeparatedSyntaxList<ExpressionSyntax> Incrementors
        {
            get
            {
                SyntaxNode red = GetRed(ref incrementors, 8);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ExpressionSyntax>);
                }
                return new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(8));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForStatementSyntax)base.Green).closeParenToken, GetChildPosition(9), GetChildIndex(9));

        public StatementSyntax Statement => GetRed(ref statement, 10);

        public ForStatementSyntax Update(SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<ExpressionSyntax> incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement);
        }

        internal ForStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref declaration, 3),
                4 => GetRed(ref initializers, 4),
                6 => GetRed(ref condition, 6),
                8 => GetRed(ref incrementors, 8),
                10 => GetRed(ref statement, 10),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => declaration,
                4 => initializers,
                6 => condition,
                8 => incrementors,
                10 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitForStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForStatement(this);

        public ForStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<ExpressionSyntax> incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || forKeyword != ForKeyword || openParenToken != OpenParenToken || declaration != Declaration || initializers != Initializers || firstSemicolonToken != FirstSemicolonToken || condition != Condition || secondSemicolonToken != SecondSemicolonToken || incrementors != Incrementors || closeParenToken != CloseParenToken || statement != Statement)
            {
                ForStatementSyntax forStatementSyntax = SyntaxFactory.ForStatement(attributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return forStatementSyntax;
                }
                return forStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ForStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithForKeyword(SyntaxToken forKeyword)
        {
            return Update(AttributeLists, forKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, ForKeyword, openParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithDeclaration(VariableDeclarationSyntax? declaration)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithInitializers(SeparatedSyntaxList<ExpressionSyntax> initializers)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithFirstSemicolonToken(SyntaxToken firstSemicolonToken)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, firstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithCondition(ExpressionSyntax? condition)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, condition, SecondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithSecondSemicolonToken(SyntaxToken secondSemicolonToken)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, secondSemicolonToken, Incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithIncrementors(SeparatedSyntaxList<ExpressionSyntax> incrementors)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, incrementors, CloseParenToken, Statement);
        }

        public ForStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, closeParenToken, Statement);
        }

        public ForStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, ForKeyword, OpenParenToken, Declaration, Initializers, FirstSemicolonToken, Condition, SecondSemicolonToken, Incrementors, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ForStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public ForStatementSyntax AddInitializers(params ExpressionSyntax[] items)
        {
            return WithInitializers(Initializers.AddRange(items));
        }

        public ForStatementSyntax AddIncrementors(params ExpressionSyntax[] items)
        {
            return WithIncrementors(Incrementors.AddRange(items));
        }
    }
}
