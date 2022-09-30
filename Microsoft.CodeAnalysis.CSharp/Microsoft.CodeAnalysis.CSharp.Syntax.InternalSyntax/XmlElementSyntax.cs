using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlElementSyntax : XmlNodeSyntax
    {
        internal readonly XmlElementStartTagSyntax startTag;

        internal readonly GreenNode? content;

        internal readonly XmlElementEndTagSyntax endTag;

        public XmlElementStartTagSyntax StartTag => startTag;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> Content => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(content);

        public XmlElementEndTagSyntax EndTag => endTag;

        public XmlElementSyntax(SyntaxKind kind, XmlElementStartTagSyntax startTag, GreenNode? content, XmlElementEndTagSyntax endTag, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startTag);
            this.startTag = startTag;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endTag);
            this.endTag = endTag;
        }

        public XmlElementSyntax(SyntaxKind kind, XmlElementStartTagSyntax startTag, GreenNode? content, XmlElementEndTagSyntax endTag, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startTag);
            this.startTag = startTag;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endTag);
            this.endTag = endTag;
        }

        public XmlElementSyntax(SyntaxKind kind, XmlElementStartTagSyntax startTag, GreenNode? content, XmlElementEndTagSyntax endTag)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startTag);
            this.startTag = startTag;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endTag);
            this.endTag = endTag;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => startTag,
                1 => content,
                2 => endTag,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlElement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlElement(this);
        }

        public XmlElementSyntax Update(XmlElementStartTagSyntax startTag, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> content, XmlElementEndTagSyntax endTag)
        {
            if (startTag != StartTag || content != Content || endTag != EndTag)
            {
                XmlElementSyntax xmlElementSyntax = SyntaxFactory.XmlElement(startTag, content, endTag);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlElementSyntax = xmlElementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlElementSyntax = xmlElementSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlElementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlElementSyntax(base.Kind, startTag, content, endTag, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlElementSyntax(base.Kind, startTag, content, endTag, GetDiagnostics(), annotations);
        }

        public XmlElementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            XmlElementStartTagSyntax node = (XmlElementStartTagSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            startTag = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                content = greenNode;
            }
            XmlElementEndTagSyntax node2 = (XmlElementEndTagSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            endTag = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(startTag);
            writer.WriteValue(content);
            writer.WriteValue(endTag);
        }

        static XmlElementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlElementSyntax), (ObjectReader r) => new XmlElementSyntax(r));
        }
    }
}
