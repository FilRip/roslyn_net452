using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlTextSyntax : XmlNodeSyntax
    {
        internal readonly GreenNode? textTokens;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(textTokens);

        public XmlTextSyntax(SyntaxKind kind, GreenNode? textTokens, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
        }

        public XmlTextSyntax(SyntaxKind kind, GreenNode? textTokens, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
        }

        public XmlTextSyntax(SyntaxKind kind, GreenNode? textTokens)
            : base(kind)
        {
            base.SlotCount = 1;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return textTokens;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlText(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlText(this);
        }

        public XmlTextSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> textTokens)
        {
            if (textTokens != TextTokens)
            {
                XmlTextSyntax xmlTextSyntax = SyntaxFactory.XmlText(textTokens);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlTextSyntax = xmlTextSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlTextSyntax = xmlTextSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlTextSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlTextSyntax(base.Kind, textTokens, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlTextSyntax(base.Kind, textTokens, GetDiagnostics(), annotations);
        }

        public XmlTextSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                textTokens = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(textTokens);
        }

        static XmlTextSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlTextSyntax), (ObjectReader r) => new XmlTextSyntax(r));
        }
    }
}
