using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlPrefixSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken prefix;

        internal readonly SyntaxToken colonToken;

        public SyntaxToken Prefix => prefix;

        public SyntaxToken ColonToken => colonToken;

        public XmlPrefixSyntax(SyntaxKind kind, SyntaxToken prefix, SyntaxToken colonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(prefix);
            this.prefix = prefix;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public XmlPrefixSyntax(SyntaxKind kind, SyntaxToken prefix, SyntaxToken colonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(prefix);
            this.prefix = prefix;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public XmlPrefixSyntax(SyntaxKind kind, SyntaxToken prefix, SyntaxToken colonToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(prefix);
            this.prefix = prefix;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => prefix,
                1 => colonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlPrefix(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlPrefix(this);
        }

        public XmlPrefixSyntax Update(SyntaxToken prefix, SyntaxToken colonToken)
        {
            if (prefix != Prefix || colonToken != ColonToken)
            {
                XmlPrefixSyntax xmlPrefixSyntax = SyntaxFactory.XmlPrefix(prefix, colonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlPrefixSyntax = xmlPrefixSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlPrefixSyntax = xmlPrefixSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlPrefixSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlPrefixSyntax(base.Kind, prefix, colonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlPrefixSyntax(base.Kind, prefix, colonToken, GetDiagnostics(), annotations);
        }

        public XmlPrefixSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            prefix = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            colonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(prefix);
            writer.WriteValue(colonToken);
        }

        static XmlPrefixSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlPrefixSyntax), (ObjectReader r) => new XmlPrefixSyntax(r));
        }
    }
}
