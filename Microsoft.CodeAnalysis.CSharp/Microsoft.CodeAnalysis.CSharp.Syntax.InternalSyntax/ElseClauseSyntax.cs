using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ElseClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken elseKeyword;

        internal readonly StatementSyntax statement;

        public SyntaxToken ElseKeyword => elseKeyword;

        public StatementSyntax Statement => statement;

        public ElseClauseSyntax(SyntaxKind kind, SyntaxToken elseKeyword, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elseKeyword);
            this.elseKeyword = elseKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ElseClauseSyntax(SyntaxKind kind, SyntaxToken elseKeyword, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elseKeyword);
            this.elseKeyword = elseKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ElseClauseSyntax(SyntaxKind kind, SyntaxToken elseKeyword, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elseKeyword);
            this.elseKeyword = elseKeyword;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => elseKeyword,
                1 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitElseClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitElseClause(this);
        }

        public ElseClauseSyntax Update(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            if (elseKeyword != ElseKeyword || statement != Statement)
            {
                ElseClauseSyntax elseClauseSyntax = SyntaxFactory.ElseClause(elseKeyword, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    elseClauseSyntax = elseClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    elseClauseSyntax = elseClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return elseClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ElseClauseSyntax(base.Kind, elseKeyword, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ElseClauseSyntax(base.Kind, elseKeyword, statement, GetDiagnostics(), annotations);
        }

        public ElseClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            elseKeyword = node;
            StatementSyntax node2 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            statement = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(elseKeyword);
            writer.WriteValue(statement);
        }

        static ElseClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ElseClauseSyntax), (ObjectReader r) => new ElseClauseSyntax(r));
        }
    }
}
