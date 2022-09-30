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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlNameSyntax : CSharpSyntaxNode
    {
        internal readonly XmlPrefixSyntax? prefix;

        internal readonly SyntaxToken localName;

        public XmlPrefixSyntax? Prefix => prefix;

        public SyntaxToken LocalName => localName;

        public XmlNameSyntax(SyntaxKind kind, XmlPrefixSyntax? prefix, SyntaxToken localName, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            if (prefix != null)
            {
                AdjustFlagsAndWidth(prefix);
                this.prefix = prefix;
            }
            AdjustFlagsAndWidth(localName);
            this.localName = localName;
        }

        public XmlNameSyntax(SyntaxKind kind, XmlPrefixSyntax? prefix, SyntaxToken localName, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            if (prefix != null)
            {
                AdjustFlagsAndWidth(prefix);
                this.prefix = prefix;
            }
            AdjustFlagsAndWidth(localName);
            this.localName = localName;
        }

        public XmlNameSyntax(SyntaxKind kind, XmlPrefixSyntax? prefix, SyntaxToken localName)
            : base(kind)
        {
            base.SlotCount = 2;
            if (prefix != null)
            {
                AdjustFlagsAndWidth(prefix);
                this.prefix = prefix;
            }
            AdjustFlagsAndWidth(localName);
            this.localName = localName;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => prefix,
                1 => localName,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlName(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlName(this);
        }

        public XmlNameSyntax Update(XmlPrefixSyntax prefix, SyntaxToken localName)
        {
            if (prefix != Prefix || localName != LocalName)
            {
                XmlNameSyntax xmlNameSyntax = SyntaxFactory.XmlName(prefix, localName);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlNameSyntax = xmlNameSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlNameSyntax = xmlNameSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlNameSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlNameSyntax(base.Kind, prefix, localName, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlNameSyntax(base.Kind, prefix, localName, GetDiagnostics(), annotations);
        }

        public XmlNameSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            XmlPrefixSyntax xmlPrefixSyntax = (XmlPrefixSyntax)reader.ReadValue();
            if (xmlPrefixSyntax != null)
            {
                AdjustFlagsAndWidth(xmlPrefixSyntax);
                prefix = xmlPrefixSyntax;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            localName = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(prefix);
            writer.WriteValue(localName);
        }

        static XmlNameSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlNameSyntax), (ObjectReader r) => new XmlNameSyntax(r));
        }
    }
}
