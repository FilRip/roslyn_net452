#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LocalFunctionStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? returnType;

        private TypeParameterListSyntax? typeParameterList;

        private ParameterListSyntax? parameterList;

        private SyntaxNode? constraintClauses;

        private BlockSyntax? body;

        private ArrowExpressionClauseSyntax? expressionBody;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(1);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public TypeSyntax ReturnType => GetRed(ref returnType, 2);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LocalFunctionStatementSyntax)base.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public TypeParameterListSyntax? TypeParameterList => GetRed(ref typeParameterList, 4);

        public ParameterListSyntax ParameterList => GetRed(ref parameterList, 5);

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref constraintClauses, 6));

        public BlockSyntax? Body => GetRed(ref body, 7);

        public ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref expressionBody, 8);

        public SyntaxToken SemicolonToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken semicolonToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LocalFunctionStatementSyntax)base.Green).semicolonToken;
                if (semicolonToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, semicolonToken, GetChildPosition(9), GetChildIndex(9));
            }
        }

        public LocalFunctionStatementSyntax Update(SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax body, ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, semicolonToken);
        }

        internal LocalFunctionStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref returnType, 2),
                4 => GetRed(ref typeParameterList, 4),
                5 => GetRed(ref parameterList, 5),
                6 => GetRed(ref constraintClauses, 6),
                7 => GetRed(ref body, 7),
                8 => GetRed(ref expressionBody, 8),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => returnType,
                4 => typeParameterList,
                5 => parameterList,
                6 => constraintClauses,
                7 => body,
                8 => expressionBody,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLocalFunctionStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLocalFunctionStatement(this);

        public LocalFunctionStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || returnType != ReturnType || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || constraintClauses != ConstraintClauses || body != Body || expressionBody != ExpressionBody || semicolonToken != SemicolonToken)
            {
                LocalFunctionStatementSyntax localFunctionStatementSyntax = SyntaxFactory.LocalFunctionStatement(attributeLists, modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return localFunctionStatementSyntax;
                }
                return localFunctionStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new LocalFunctionStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithReturnType(TypeSyntax returnType)
        {
            return Update(AttributeLists, Modifiers, returnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, ReturnType, identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, typeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithParameterList(ParameterListSyntax parameterList)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, parameterList, ConstraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, constraintClauses, Body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithBody(BlockSyntax? body)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, body, ExpressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, expressionBody, SemicolonToken);
        }

        public LocalFunctionStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, Body, ExpressionBody, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new LocalFunctionStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public LocalFunctionStatementSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public LocalFunctionStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            TypeParameterListSyntax typeParameterListSyntax = TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterListSyntax.WithParameters(typeParameterListSyntax.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            return WithParameterList(ParameterList.WithParameters(ParameterList.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return WithConstraintClauses(ConstraintClauses.AddRange(items));
        }

        public LocalFunctionStatementSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            BlockSyntax blockSyntax = Body ?? SyntaxFactory.Block();
            return WithBody(blockSyntax.WithAttributeLists(blockSyntax.AttributeLists.AddRange(items)));
        }

        public LocalFunctionStatementSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            BlockSyntax blockSyntax = Body ?? SyntaxFactory.Block();
            return WithBody(blockSyntax.WithStatements(blockSyntax.Statements.AddRange(items)));
        }
    }
}
