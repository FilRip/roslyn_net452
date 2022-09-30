using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken ifKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly ExpressionSyntax condition;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        internal readonly ElseClauseSyntax? @else;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken IfKeyword => ifKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public ExpressionSyntax Condition => condition;

        public SyntaxToken CloseParenToken => closeParenToken;

        public StatementSyntax Statement => statement;

        public ElseClauseSyntax? Else => @else;

        public IfStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 7;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            if (@else != null)
            {
                AdjustFlagsAndWidth(@else);
                this.@else = @else;
            }
        }

        public IfStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 7;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            if (@else != null)
            {
                AdjustFlagsAndWidth(@else);
                this.@else = @else;
            }
        }

        public IfStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else)
            : base(kind)
        {
            base.SlotCount = 7;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
            if (@else != null)
            {
                AdjustFlagsAndWidth(@else);
                this.@else = @else;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => ifKeyword,
                2 => openParenToken,
                3 => condition,
                4 => closeParenToken,
                5 => statement,
                6 => @else,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitIfStatement(this);
        }

        public IfStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax @else)
        {
            if (attributeLists != AttributeLists || ifKeyword != IfKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || statement != Statement || @else != Else)
            {
                IfStatementSyntax ifStatementSyntax = SyntaxFactory.IfStatement(attributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    ifStatementSyntax = ifStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    ifStatementSyntax = ifStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return ifStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new IfStatementSyntax(base.Kind, attributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new IfStatementSyntax(base.Kind, attributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else, GetDiagnostics(), annotations);
        }

        public IfStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 7;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            ifKeyword = node;
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
            ElseClauseSyntax elseClauseSyntax = (ElseClauseSyntax)reader.ReadValue();
            if (elseClauseSyntax != null)
            {
                AdjustFlagsAndWidth(elseClauseSyntax);
                @else = elseClauseSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(ifKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(condition);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
            writer.WriteValue(@else);
        }

        static IfStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(IfStatementSyntax), (ObjectReader r) => new IfStatementSyntax(r));
        }
    }
}
