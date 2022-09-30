using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class JoinIntoClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken intoKeyword;

        internal readonly SyntaxToken identifier;

        public SyntaxToken IntoKeyword => intoKeyword;

        public SyntaxToken Identifier => identifier;

        public JoinIntoClauseSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public JoinIntoClauseSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public JoinIntoClauseSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => intoKeyword,
                1 => identifier,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitJoinIntoClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitJoinIntoClause(this);
        }

        public JoinIntoClauseSyntax Update(SyntaxToken intoKeyword, SyntaxToken identifier)
        {
            if (intoKeyword != IntoKeyword || identifier != Identifier)
            {
                JoinIntoClauseSyntax joinIntoClauseSyntax = SyntaxFactory.JoinIntoClause(intoKeyword, identifier);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    joinIntoClauseSyntax = joinIntoClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    joinIntoClauseSyntax = joinIntoClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return joinIntoClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new JoinIntoClauseSyntax(base.Kind, intoKeyword, identifier, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new JoinIntoClauseSyntax(base.Kind, intoKeyword, identifier, GetDiagnostics(), annotations);
        }

        public JoinIntoClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            intoKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(intoKeyword);
            writer.WriteValue(identifier);
        }

        static JoinIntoClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(JoinIntoClauseSyntax), (ObjectReader r) => new JoinIntoClauseSyntax(r));
        }
    }
}
