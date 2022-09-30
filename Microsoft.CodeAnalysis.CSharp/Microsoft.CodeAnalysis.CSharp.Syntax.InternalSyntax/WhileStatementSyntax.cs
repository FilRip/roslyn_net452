using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken whileKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly ExpressionSyntax condition;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken WhileKeyword => whileKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public ExpressionSyntax Condition => condition;

        public SyntaxToken CloseParenToken => closeParenToken;

        public StatementSyntax Statement => statement;

        public WhileStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public WhileStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public WhileStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
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
                1 => whileKeyword,
                2 => openParenToken,
                3 => condition,
                4 => closeParenToken,
                5 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }

        public WhileStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || whileKeyword != WhileKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || statement != Statement)
            {
                WhileStatementSyntax whileStatementSyntax = SyntaxFactory.WhileStatement(attributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    whileStatementSyntax = whileStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    whileStatementSyntax = whileStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return whileStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new WhileStatementSyntax(base.Kind, attributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new WhileStatementSyntax(base.Kind, attributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement, GetDiagnostics(), annotations);
        }

        public WhileStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            whileKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            condition = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeParenToken = node4;
            StatementSyntax node5 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            statement = node5;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(whileKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(condition);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
        }

        static WhileStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(WhileStatementSyntax), (ObjectReader r) => new WhileStatementSyntax(r));
        }
    }
}
