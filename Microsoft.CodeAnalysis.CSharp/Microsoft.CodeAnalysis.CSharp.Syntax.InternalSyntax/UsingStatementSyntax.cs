using Roslyn.Utilities;

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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class UsingStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken? awaitKeyword;

        internal readonly SyntaxToken usingKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly VariableDeclarationSyntax? declaration;

        internal readonly ExpressionSyntax? expression;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken? AwaitKeyword => awaitKeyword;

        public SyntaxToken UsingKeyword => usingKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public VariableDeclarationSyntax? Declaration => declaration;

        public ExpressionSyntax? Expression => expression;

        public SyntaxToken CloseParenToken => closeParenToken;

        public StatementSyntax Statement => statement;

        public UsingStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public UsingStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public UsingStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => awaitKeyword,
                2 => usingKeyword,
                3 => openParenToken,
                4 => declaration,
                5 => expression,
                6 => closeParenToken,
                7 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitUsingStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitUsingStatement(this);
        }

        public UsingStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || usingKeyword != UsingKeyword || openParenToken != OpenParenToken || declaration != Declaration || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                UsingStatementSyntax usingStatementSyntax = SyntaxFactory.UsingStatement(attributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    usingStatementSyntax = usingStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    usingStatementSyntax = usingStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return usingStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new UsingStatementSyntax(base.Kind, attributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new UsingStatementSyntax(base.Kind, attributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement, GetDiagnostics(), annotations);
        }

        public UsingStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 8;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                awaitKeyword = syntaxToken;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            usingKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            VariableDeclarationSyntax variableDeclarationSyntax = (VariableDeclarationSyntax)reader.ReadValue();
            if (variableDeclarationSyntax != null)
            {
                AdjustFlagsAndWidth(variableDeclarationSyntax);
                declaration = variableDeclarationSyntax;
            }
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                expression = expressionSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeParenToken = node3;
            StatementSyntax node4 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            statement = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(awaitKeyword);
            writer.WriteValue(usingKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(declaration);
            writer.WriteValue(expression);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
        }

        static UsingStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(UsingStatementSyntax), (ObjectReader r) => new UsingStatementSyntax(r));
        }
    }
}
