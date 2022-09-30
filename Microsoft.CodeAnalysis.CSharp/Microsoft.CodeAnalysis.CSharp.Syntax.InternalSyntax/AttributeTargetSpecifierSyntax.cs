using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class AttributeTargetSpecifierSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken identifier;

        internal readonly SyntaxToken colonToken;

        public SyntaxToken Identifier => identifier;

        public SyntaxToken ColonToken => colonToken;

        public AttributeTargetSpecifierSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public AttributeTargetSpecifierSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public AttributeTargetSpecifierSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => identifier,
                1 => colonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAttributeTargetSpecifier(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAttributeTargetSpecifier(this);
        }

        public AttributeTargetSpecifierSyntax Update(SyntaxToken identifier, SyntaxToken colonToken)
        {
            if (identifier != Identifier || colonToken != ColonToken)
            {
                AttributeTargetSpecifierSyntax attributeTargetSpecifierSyntax = SyntaxFactory.AttributeTargetSpecifier(identifier, colonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    attributeTargetSpecifierSyntax = attributeTargetSpecifierSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    attributeTargetSpecifierSyntax = attributeTargetSpecifierSyntax.WithAnnotationsGreen(annotations);
                }
                return attributeTargetSpecifierSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AttributeTargetSpecifierSyntax(base.Kind, identifier, colonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AttributeTargetSpecifierSyntax(base.Kind, identifier, colonToken, GetDiagnostics(), annotations);
        }

        public AttributeTargetSpecifierSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            colonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(identifier);
            writer.WriteValue(colonToken);
        }

        static AttributeTargetSpecifierSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AttributeTargetSpecifierSyntax), (ObjectReader r) => new AttributeTargetSpecifierSyntax(r));
        }
    }
}
