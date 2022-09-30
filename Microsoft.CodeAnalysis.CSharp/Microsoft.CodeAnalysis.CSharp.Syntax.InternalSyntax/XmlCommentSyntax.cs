using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class XmlCommentSyntax : XmlNodeSyntax
    {
        internal readonly SyntaxToken lessThanExclamationMinusMinusToken;

        internal readonly GreenNode? textTokens;

        internal readonly SyntaxToken minusMinusGreaterThanToken;

        public SyntaxToken LessThanExclamationMinusMinusToken => lessThanExclamationMinusMinusToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(textTokens);

        public SyntaxToken MinusMinusGreaterThanToken => minusMinusGreaterThanToken;

        public XmlCommentSyntax(SyntaxKind kind, SyntaxToken lessThanExclamationMinusMinusToken, GreenNode? textTokens, SyntaxToken minusMinusGreaterThanToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
            this.lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(minusMinusGreaterThanToken);
            this.minusMinusGreaterThanToken = minusMinusGreaterThanToken;
        }

        public XmlCommentSyntax(SyntaxKind kind, SyntaxToken lessThanExclamationMinusMinusToken, GreenNode? textTokens, SyntaxToken minusMinusGreaterThanToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
            this.lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(minusMinusGreaterThanToken);
            this.minusMinusGreaterThanToken = minusMinusGreaterThanToken;
        }

        public XmlCommentSyntax(SyntaxKind kind, SyntaxToken lessThanExclamationMinusMinusToken, GreenNode? textTokens, SyntaxToken minusMinusGreaterThanToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
            this.lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
            if (textTokens != null)
            {
                AdjustFlagsAndWidth(textTokens);
                this.textTokens = textTokens;
            }
            AdjustFlagsAndWidth(minusMinusGreaterThanToken);
            this.minusMinusGreaterThanToken = minusMinusGreaterThanToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => lessThanExclamationMinusMinusToken,
                1 => textTokens,
                2 => minusMinusGreaterThanToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.XmlCommentSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlComment(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitXmlComment(this);
        }

        public XmlCommentSyntax Update(SyntaxToken lessThanExclamationMinusMinusToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> textTokens, SyntaxToken minusMinusGreaterThanToken)
        {
            if (lessThanExclamationMinusMinusToken != LessThanExclamationMinusMinusToken || textTokens != TextTokens || minusMinusGreaterThanToken != MinusMinusGreaterThanToken)
            {
                XmlCommentSyntax xmlCommentSyntax = SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    xmlCommentSyntax = xmlCommentSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    xmlCommentSyntax = xmlCommentSyntax.WithAnnotationsGreen(annotations);
                }
                return xmlCommentSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new XmlCommentSyntax(base.Kind, lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new XmlCommentSyntax(base.Kind, lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken, GetDiagnostics(), annotations);
        }

        public XmlCommentSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            lessThanExclamationMinusMinusToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                textTokens = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            minusMinusGreaterThanToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(lessThanExclamationMinusMinusToken);
            writer.WriteValue(textTokens);
            writer.WriteValue(minusMinusGreaterThanToken);
        }

        static XmlCommentSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlCommentSyntax), (ObjectReader r) => new XmlCommentSyntax(r));
        }
    }
}
