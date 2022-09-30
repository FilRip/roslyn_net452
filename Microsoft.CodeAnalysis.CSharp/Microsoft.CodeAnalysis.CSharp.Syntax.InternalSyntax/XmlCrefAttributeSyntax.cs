using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlCrefAttributeSyntax : XmlAttributeSyntax
    {
        internal readonly XmlNameSyntax name;

        internal readonly SyntaxToken equalsToken;

        internal readonly SyntaxToken startQuoteToken;

        internal readonly CrefSyntax cref;

        internal readonly SyntaxToken endQuoteToken;

        public override XmlNameSyntax Name => name;

        public override SyntaxToken EqualsToken => equalsToken;

        public override SyntaxToken StartQuoteToken => startQuoteToken;

        public CrefSyntax Cref => cref;

        public override SyntaxToken EndQuoteToken => endQuoteToken;

        public XmlCrefAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(startQuoteToken);
            this.startQuoteToken = startQuoteToken;
            AdjustFlagsAndWidth(cref);
            this.cref = cref;
            AdjustFlagsAndWidth(endQuoteToken);
            this.endQuoteToken = endQuoteToken;
        }

        public XmlCrefAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(startQuoteToken);
            this.startQuoteToken = startQuoteToken;
            AdjustFlagsAndWidth(cref);
            this.cref = cref;
            AdjustFlagsAndWidth(endQuoteToken);
            this.endQuoteToken = endQuoteToken;
        }

        public XmlCrefAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(startQuoteToken);
            this.startQuoteToken = startQuoteToken;
            AdjustFlagsAndWidth(cref);
            this.cref = cref;
            AdjustFlagsAndWidth(endQuoteToken);
            this.endQuoteToken = endQuoteToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => equalsToken,
                2 => startQuoteToken,
                3 => cref,
                4 => endQuoteToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlCrefAttribute(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlCrefAttribute(this);
        }

        public XmlCrefAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken)
        {
            if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || cref != Cref || endQuoteToken != EndQuoteToken)
            {
                XmlCrefAttributeSyntax xmlCrefAttributeSyntax = SyntaxFactory.XmlCrefAttribute(name, equalsToken, startQuoteToken, cref, endQuoteToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlCrefAttributeSyntax = xmlCrefAttributeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlCrefAttributeSyntax = xmlCrefAttributeSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlCrefAttributeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlCrefAttributeSyntax(base.Kind, name, equalsToken, startQuoteToken, cref, endQuoteToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlCrefAttributeSyntax(base.Kind, name, equalsToken, startQuoteToken, cref, endQuoteToken, GetDiagnostics(), annotations);
        }

        public XmlCrefAttributeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            XmlNameSyntax node = (XmlNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            equalsToken = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            startQuoteToken = node3;
            CrefSyntax node4 = (CrefSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            cref = node4;
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            endQuoteToken = node5;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(name);
            writer.WriteValue(equalsToken);
            writer.WriteValue(startQuoteToken);
            writer.WriteValue(cref);
            writer.WriteValue(endQuoteToken);
        }

        static XmlCrefAttributeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlCrefAttributeSyntax), (ObjectReader r) => new XmlCrefAttributeSyntax(r));
        }
    }
}
