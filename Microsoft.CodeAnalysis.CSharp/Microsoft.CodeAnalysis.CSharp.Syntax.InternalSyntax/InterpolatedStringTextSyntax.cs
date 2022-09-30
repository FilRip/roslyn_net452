using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InterpolatedStringTextSyntax : InterpolatedStringContentSyntax
    {
        internal readonly SyntaxToken textToken;

        public SyntaxToken TextToken => textToken;

        public InterpolatedStringTextSyntax(SyntaxKind kind, SyntaxToken textToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(textToken);
            this.textToken = textToken;
        }

        public InterpolatedStringTextSyntax(SyntaxKind kind, SyntaxToken textToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(textToken);
            this.textToken = textToken;
        }

        public InterpolatedStringTextSyntax(SyntaxKind kind, SyntaxToken textToken)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(textToken);
            this.textToken = textToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return textToken;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringTextSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolatedStringText(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInterpolatedStringText(this);
        }

        public InterpolatedStringTextSyntax Update(SyntaxToken textToken)
        {
            if (textToken != TextToken)
            {
                InterpolatedStringTextSyntax interpolatedStringTextSyntax = SyntaxFactory.InterpolatedStringText(textToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    interpolatedStringTextSyntax = interpolatedStringTextSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    interpolatedStringTextSyntax = interpolatedStringTextSyntax.WithAnnotationsGreen(annotations);
                }
                return interpolatedStringTextSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InterpolatedStringTextSyntax(base.Kind, textToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InterpolatedStringTextSyntax(base.Kind, textToken, GetDiagnostics(), annotations);
        }

        public InterpolatedStringTextSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            textToken = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(textToken);
        }

        static InterpolatedStringTextSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InterpolatedStringTextSyntax), (ObjectReader r) => new InterpolatedStringTextSyntax(r));
        }
    }
}
