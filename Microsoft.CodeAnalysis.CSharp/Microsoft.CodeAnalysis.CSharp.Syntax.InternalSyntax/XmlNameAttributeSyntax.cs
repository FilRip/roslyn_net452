using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlNameAttributeSyntax : XmlAttributeSyntax
    {
        internal readonly XmlNameSyntax name;

        internal readonly SyntaxToken equalsToken;

        internal readonly SyntaxToken startQuoteToken;

        internal readonly IdentifierNameSyntax identifier;

        internal readonly SyntaxToken endQuoteToken;

        public override XmlNameSyntax Name => name;

        public override SyntaxToken EqualsToken => equalsToken;

        public override SyntaxToken StartQuoteToken => startQuoteToken;

        public IdentifierNameSyntax Identifier => identifier;

        public override SyntaxToken EndQuoteToken => endQuoteToken;

        public XmlNameAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(startQuoteToken);
            this.startQuoteToken = startQuoteToken;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(endQuoteToken);
            this.endQuoteToken = endQuoteToken;
        }

        public XmlNameAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken, SyntaxFactoryContext context)
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
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(endQuoteToken);
            this.endQuoteToken = endQuoteToken;
        }

        public XmlNameAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(startQuoteToken);
            this.startQuoteToken = startQuoteToken;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
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
                3 => identifier,
                4 => endQuoteToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlNameAttribute(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlNameAttribute(this);
        }

        public XmlNameAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || identifier != Identifier || endQuoteToken != EndQuoteToken)
            {
                XmlNameAttributeSyntax xmlNameAttributeSyntax = SyntaxFactory.XmlNameAttribute(name, equalsToken, startQuoteToken, identifier, endQuoteToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlNameAttributeSyntax = xmlNameAttributeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlNameAttributeSyntax = xmlNameAttributeSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlNameAttributeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlNameAttributeSyntax(base.Kind, name, equalsToken, startQuoteToken, identifier, endQuoteToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlNameAttributeSyntax(base.Kind, name, equalsToken, startQuoteToken, identifier, endQuoteToken, GetDiagnostics(), annotations);
        }

        public XmlNameAttributeSyntax(ObjectReader reader)
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
            IdentifierNameSyntax node4 = (IdentifierNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            identifier = node4;
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
            writer.WriteValue(identifier);
            writer.WriteValue(endQuoteToken);
        }

        static XmlNameAttributeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlNameAttributeSyntax), (ObjectReader r) => new XmlNameAttributeSyntax(r));
        }
    }
}
