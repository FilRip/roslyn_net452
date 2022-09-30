using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InterpolationAlignmentClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken commaToken;

        internal readonly ExpressionSyntax value;

        public SyntaxToken CommaToken => commaToken;

        public ExpressionSyntax Value => value;

        public InterpolationAlignmentClauseSyntax(SyntaxKind kind, SyntaxToken commaToken, ExpressionSyntax value, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(commaToken);
            this.commaToken = commaToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public InterpolationAlignmentClauseSyntax(SyntaxKind kind, SyntaxToken commaToken, ExpressionSyntax value, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(commaToken);
            this.commaToken = commaToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public InterpolationAlignmentClauseSyntax(SyntaxKind kind, SyntaxToken commaToken, ExpressionSyntax value)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(commaToken);
            this.commaToken = commaToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => commaToken,
                1 => value,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationAlignmentClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolationAlignmentClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInterpolationAlignmentClause(this);
        }

        public InterpolationAlignmentClauseSyntax Update(SyntaxToken commaToken, ExpressionSyntax value)
        {
            if (commaToken != CommaToken || value != Value)
            {
                InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = SyntaxFactory.InterpolationAlignmentClause(commaToken, value);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    interpolationAlignmentClauseSyntax = interpolationAlignmentClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    interpolationAlignmentClauseSyntax = interpolationAlignmentClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return interpolationAlignmentClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InterpolationAlignmentClauseSyntax(base.Kind, commaToken, value, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InterpolationAlignmentClauseSyntax(base.Kind, commaToken, value, GetDiagnostics(), annotations);
        }

        public InterpolationAlignmentClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            commaToken = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            value = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(commaToken);
            writer.WriteValue(value);
        }

        static InterpolationAlignmentClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InterpolationAlignmentClauseSyntax), (ObjectReader r) => new InterpolationAlignmentClauseSyntax(r));
        }
    }
}
