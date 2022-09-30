using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ForEachStatementSyntax : CommonForEachStatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken? awaitKeyword;

        internal readonly SyntaxToken forEachKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly TypeSyntax type;

        internal readonly SyntaxToken identifier;

        internal readonly SyntaxToken inKeyword;

        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override SyntaxToken? AwaitKeyword => awaitKeyword;

        public override SyntaxToken ForEachKeyword => forEachKeyword;

        public override SyntaxToken OpenParenToken => openParenToken;

        public TypeSyntax Type => type;

        public SyntaxToken Identifier => identifier;

        public override SyntaxToken InKeyword => inKeyword;

        public override ExpressionSyntax Expression => expression;

        public override SyntaxToken CloseParenToken => closeParenToken;

        public override StatementSyntax Statement => statement;

        public ForEachStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 10;
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
            AdjustFlagsAndWidth(forEachKeyword);
            this.forEachKeyword = forEachKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ForEachStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 10;
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
            AdjustFlagsAndWidth(forEachKeyword);
            this.forEachKeyword = forEachKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ForEachStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 10;
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
            AdjustFlagsAndWidth(forEachKeyword);
            this.forEachKeyword = forEachKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
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
                2 => forEachKeyword,
                3 => openParenToken,
                4 => type,
                5 => identifier,
                6 => inKeyword,
                7 => expression,
                8 => closeParenToken,
                9 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitForEachStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitForEachStatement(this);
        }

        public ForEachStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || forEachKeyword != ForEachKeyword || openParenToken != OpenParenToken || type != Type || identifier != Identifier || inKeyword != InKeyword || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                ForEachStatementSyntax forEachStatementSyntax = SyntaxFactory.ForEachStatement(attributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    forEachStatementSyntax = forEachStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    forEachStatementSyntax = forEachStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return forEachStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ForEachStatementSyntax(base.Kind, attributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ForEachStatementSyntax(base.Kind, attributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement, GetDiagnostics(), annotations);
        }

        public ForEachStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 10;
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
            forEachKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            TypeSyntax node3 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            type = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            identifier = node4;
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            inKeyword = node5;
            ExpressionSyntax node6 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node6);
            expression = node6;
            SyntaxToken node7 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node7);
            closeParenToken = node7;
            StatementSyntax node8 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node8);
            statement = node8;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(awaitKeyword);
            writer.WriteValue(forEachKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(type);
            writer.WriteValue(identifier);
            writer.WriteValue(inKeyword);
            writer.WriteValue(expression);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
        }

        static ForEachStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ForEachStatementSyntax), (ObjectReader r) => new ForEachStatementSyntax(r));
        }
    }
}
