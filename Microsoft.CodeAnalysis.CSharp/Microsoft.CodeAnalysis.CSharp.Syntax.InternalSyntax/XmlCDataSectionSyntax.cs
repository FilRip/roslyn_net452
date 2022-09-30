using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlCDataSectionSyntax : XmlNodeSyntax
    {
        internal readonly SyntaxToken startCDataToken;

        internal readonly GreenNode? textTokens;

        internal readonly SyntaxToken endCDataToken;

        public SyntaxToken StartCDataToken => startCDataToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(textTokens);

        public SyntaxToken EndCDataToken => endCDataToken;

        public XmlCDataSectionSyntax(SyntaxKind kind, SyntaxToken startCDataToken, GreenNode? textTokens, SyntaxToken endCDataToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startCDataToken);
            this.startCDataToken = startCDataToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(endCDataToken);
            this.endCDataToken = endCDataToken;
        }

        public XmlCDataSectionSyntax(SyntaxKind kind, SyntaxToken startCDataToken, GreenNode? textTokens, SyntaxToken endCDataToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startCDataToken);
            this.startCDataToken = startCDataToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(endCDataToken);
            this.endCDataToken = endCDataToken;
        }

        public XmlCDataSectionSyntax(SyntaxKind kind, SyntaxToken startCDataToken, GreenNode? textTokens, SyntaxToken endCDataToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(startCDataToken);
            this.startCDataToken = startCDataToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(endCDataToken);
            this.endCDataToken = endCDataToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => startCDataToken,
                1 => textTokens,
                2 => endCDataToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlCDataSectionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlCDataSection(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlCDataSection(this);
        }

        public XmlCDataSectionSyntax Update(SyntaxToken startCDataToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> textTokens, SyntaxToken endCDataToken)
        {
            if (startCDataToken != StartCDataToken || textTokens != TextTokens || endCDataToken != EndCDataToken)
            {
                XmlCDataSectionSyntax xmlCDataSectionSyntax = SyntaxFactory.XmlCDataSection(startCDataToken, textTokens, endCDataToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlCDataSectionSyntax = xmlCDataSectionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlCDataSectionSyntax = xmlCDataSectionSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlCDataSectionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlCDataSectionSyntax(base.Kind, startCDataToken, textTokens, endCDataToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlCDataSectionSyntax(base.Kind, startCDataToken, textTokens, endCDataToken, GetDiagnostics(), annotations);
        }

        public XmlCDataSectionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            startCDataToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                textTokens = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            endCDataToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(startCDataToken);
            writer.WriteValue(textTokens);
            writer.WriteValue(endCDataToken);
        }

        static XmlCDataSectionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlCDataSectionSyntax), (ObjectReader r) => new XmlCDataSectionSyntax(r));
        }
    }
}
