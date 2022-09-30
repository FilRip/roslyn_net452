using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class IdentifierNameSyntax : SimpleNameSyntax
    {
        internal readonly SyntaxToken identifier;

        public override SyntaxToken Identifier => identifier;

        public override string ToString()
        {
            return Identifier.Text;
        }

        public IdentifierNameSyntax(SyntaxKind kind, SyntaxToken identifier, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public IdentifierNameSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public IdentifierNameSyntax(SyntaxKind kind, SyntaxToken identifier)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return identifier;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIdentifierName(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitIdentifierName(this);
        }

        public IdentifierNameSyntax Update(SyntaxToken identifier)
        {
            if (identifier != Identifier)
            {
                IdentifierNameSyntax identifierNameSyntax = SyntaxFactory.IdentifierName(identifier);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    identifierNameSyntax = identifierNameSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    identifierNameSyntax = identifierNameSyntax.WithAnnotationsGreen(annotations);
                }
                return identifierNameSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new IdentifierNameSyntax(base.Kind, identifier, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new IdentifierNameSyntax(base.Kind, identifier, GetDiagnostics(), annotations);
        }

        public IdentifierNameSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(identifier);
        }

        static IdentifierNameSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(IdentifierNameSyntax), (ObjectReader r) => new IdentifierNameSyntax(r));
        }
    }
}
