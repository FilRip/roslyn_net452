using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DoStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken doKeyword;

        internal readonly StatementSyntax statement;

        internal readonly SyntaxToken whileKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly ExpressionSyntax condition;

        internal readonly SyntaxToken closeParenToken;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken DoKeyword => doKeyword;

        public StatementSyntax Statement => statement;

        public SyntaxToken WhileKeyword => whileKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public ExpressionSyntax Condition => condition;

        public SyntaxToken CloseParenToken => closeParenToken;

        public SyntaxToken SemicolonToken => semicolonToken;

        public DoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(doKeyword);
            this.doKeyword = doKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public DoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(doKeyword);
            this.doKeyword = doKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public DoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 8;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(doKeyword);
            this.doKeyword = doKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            AdjustFlagsAndWidth(whileKeyword);
            this.whileKeyword = whileKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => doKeyword,
                2 => statement,
                3 => whileKeyword,
                4 => openParenToken,
                5 => condition,
                6 => closeParenToken,
                7 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDoStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDoStatement(this);
        }

        public DoStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || doKeyword != DoKeyword || statement != Statement || whileKeyword != WhileKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || semicolonToken != SemicolonToken)
            {
                DoStatementSyntax doStatementSyntax = SyntaxFactory.DoStatement(attributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    doStatementSyntax = doStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    doStatementSyntax = doStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return doStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DoStatementSyntax(base.Kind, attributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DoStatementSyntax(base.Kind, attributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken, GetDiagnostics(), annotations);
        }

        public DoStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 8;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            doKeyword = node;
            StatementSyntax node2 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            statement = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            whileKeyword = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            openParenToken = node4;
            ExpressionSyntax node5 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            condition = node5;
            SyntaxToken node6 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node6);
            closeParenToken = node6;
            SyntaxToken node7 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node7);
            semicolonToken = node7;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(doKeyword);
            writer.WriteValue(statement);
            writer.WriteValue(whileKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(condition);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(semicolonToken);
        }

        static DoStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DoStatementSyntax), (ObjectReader r) => new DoStatementSyntax(r));
        }
    }
}
