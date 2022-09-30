using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InterpolationSyntax : InterpolatedStringContentSyntax
    {
        internal readonly SyntaxToken openBraceToken;

        internal readonly ExpressionSyntax expression;

        internal readonly InterpolationAlignmentClauseSyntax? alignmentClause;

        internal readonly InterpolationFormatClauseSyntax? formatClause;

        internal readonly SyntaxToken closeBraceToken;

        public SyntaxToken OpenBraceToken => openBraceToken;

        public ExpressionSyntax Expression => expression;

        public InterpolationAlignmentClauseSyntax? AlignmentClause => alignmentClause;

        public InterpolationFormatClauseSyntax? FormatClause => formatClause;

        public SyntaxToken CloseBraceToken => closeBraceToken;

        public InterpolationSyntax(SyntaxKind kind, SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            if (alignmentClause != null)
            {
                AdjustFlagsAndWidth(alignmentClause);
                this.alignmentClause = alignmentClause;
            }
            if (formatClause != null)
            {
                AdjustFlagsAndWidth(formatClause);
                this.formatClause = formatClause;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public InterpolationSyntax(SyntaxKind kind, SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            if (alignmentClause != null)
            {
                AdjustFlagsAndWidth(alignmentClause);
                this.alignmentClause = alignmentClause;
            }
            if (formatClause != null)
            {
                AdjustFlagsAndWidth(formatClause);
                this.formatClause = formatClause;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public InterpolationSyntax(SyntaxKind kind, SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            if (alignmentClause != null)
            {
                AdjustFlagsAndWidth(alignmentClause);
                this.alignmentClause = alignmentClause;
            }
            if (formatClause != null)
            {
                AdjustFlagsAndWidth(formatClause);
                this.formatClause = formatClause;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBraceToken,
                1 => expression,
                2 => alignmentClause,
                3 => formatClause,
                4 => closeBraceToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolation(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInterpolation(this);
        }

        public InterpolationSyntax Update(SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != OpenBraceToken || expression != Expression || alignmentClause != AlignmentClause || formatClause != FormatClause || closeBraceToken != CloseBraceToken)
            {
                InterpolationSyntax interpolationSyntax = SyntaxFactory.Interpolation(openBraceToken, expression, alignmentClause, formatClause, closeBraceToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    interpolationSyntax = interpolationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    interpolationSyntax = interpolationSyntax.WithAnnotationsGreen(annotations);
                }
                return interpolationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InterpolationSyntax(base.Kind, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InterpolationSyntax(base.Kind, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken, GetDiagnostics(), annotations);
        }

        public InterpolationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBraceToken = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
            InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = (InterpolationAlignmentClauseSyntax)reader.ReadValue();
            if (interpolationAlignmentClauseSyntax != null)
            {
                AdjustFlagsAndWidth(interpolationAlignmentClauseSyntax);
                alignmentClause = interpolationAlignmentClauseSyntax;
            }
            InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = (InterpolationFormatClauseSyntax)reader.ReadValue();
            if (interpolationFormatClauseSyntax != null)
            {
                AdjustFlagsAndWidth(interpolationFormatClauseSyntax);
                formatClause = interpolationFormatClauseSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeBraceToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(expression);
            writer.WriteValue(alignmentClause);
            writer.WriteValue(formatClause);
            writer.WriteValue(closeBraceToken);
        }

        static InterpolationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InterpolationSyntax), (ObjectReader r) => new InterpolationSyntax(r));
        }
    }
}
