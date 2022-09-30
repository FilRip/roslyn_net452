using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlElementEndTagSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken lessThanSlashToken;

        internal readonly XmlNameSyntax name;

        internal readonly SyntaxToken greaterThanToken;

        public SyntaxToken LessThanSlashToken => lessThanSlashToken;

        public XmlNameSyntax Name => name;

        public SyntaxToken GreaterThanToken => greaterThanToken;

        public XmlElementEndTagSyntax(SyntaxKind kind, SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanSlashToken);
            this.lessThanSlashToken = lessThanSlashToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public XmlElementEndTagSyntax(SyntaxKind kind, SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanSlashToken);
            this.lessThanSlashToken = lessThanSlashToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public XmlElementEndTagSyntax(SyntaxKind kind, SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanSlashToken);
            this.lessThanSlashToken = lessThanSlashToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => lessThanSlashToken,
                1 => name,
                2 => greaterThanToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlElementEndTag(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlElementEndTag(this);
        }

        public XmlElementEndTagSyntax Update(SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
        {
            if (lessThanSlashToken != LessThanSlashToken || name != Name || greaterThanToken != GreaterThanToken)
            {
                XmlElementEndTagSyntax xmlElementEndTagSyntax = SyntaxFactory.XmlElementEndTag(lessThanSlashToken, name, greaterThanToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlElementEndTagSyntax = xmlElementEndTagSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlElementEndTagSyntax = xmlElementEndTagSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlElementEndTagSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlElementEndTagSyntax(base.Kind, lessThanSlashToken, name, greaterThanToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlElementEndTagSyntax(base.Kind, lessThanSlashToken, name, greaterThanToken, GetDiagnostics(), annotations);
        }

        public XmlElementEndTagSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            lessThanSlashToken = node;
            XmlNameSyntax node2 = (XmlNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            name = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            greaterThanToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(lessThanSlashToken);
            writer.WriteValue(name);
            writer.WriteValue(greaterThanToken);
        }

        static XmlElementEndTagSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlElementEndTagSyntax), (ObjectReader r) => new XmlElementEndTagSyntax(r));
        }
    }
}
