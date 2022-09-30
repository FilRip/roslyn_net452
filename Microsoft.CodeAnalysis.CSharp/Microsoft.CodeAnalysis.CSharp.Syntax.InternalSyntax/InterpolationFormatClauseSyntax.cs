using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InterpolationFormatClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken colonToken;

        internal readonly SyntaxToken formatStringToken;

        public SyntaxToken ColonToken => colonToken;

        public SyntaxToken FormatStringToken => formatStringToken;

        public InterpolationFormatClauseSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken formatStringToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(formatStringToken);
            this.formatStringToken = formatStringToken;
        }

        public InterpolationFormatClauseSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken formatStringToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(formatStringToken);
            this.formatStringToken = formatStringToken;
        }

        public InterpolationFormatClauseSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken formatStringToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(formatStringToken);
            this.formatStringToken = formatStringToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => colonToken,
                1 => formatStringToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolationFormatClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInterpolationFormatClause(this);
        }

        public InterpolationFormatClauseSyntax Update(SyntaxToken colonToken, SyntaxToken formatStringToken)
        {
            if (colonToken != ColonToken || formatStringToken != FormatStringToken)
            {
                InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    interpolationFormatClauseSyntax = interpolationFormatClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    interpolationFormatClauseSyntax = interpolationFormatClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return interpolationFormatClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InterpolationFormatClauseSyntax(base.Kind, colonToken, formatStringToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InterpolationFormatClauseSyntax(base.Kind, colonToken, formatStringToken, GetDiagnostics(), annotations);
        }

        public InterpolationFormatClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            colonToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            formatStringToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(colonToken);
            writer.WriteValue(formatStringToken);
        }

        static InterpolationFormatClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InterpolationFormatClauseSyntax), (ObjectReader r) => new InterpolationFormatClauseSyntax(r));
        }
    }
}
