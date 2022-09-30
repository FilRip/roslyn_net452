using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConstantPatternSyntax : PatternSyntax
    {
        internal readonly ExpressionSyntax expression;

        public ExpressionSyntax Expression => expression;

        public ConstantPatternSyntax(SyntaxKind kind, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ConstantPatternSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ConstantPatternSyntax(SyntaxKind kind, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return expression;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstantPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConstantPattern(this);
        }

        public ConstantPatternSyntax Update(ExpressionSyntax expression)
        {
            if (expression != Expression)
            {
                ConstantPatternSyntax constantPatternSyntax = SyntaxFactory.ConstantPattern(expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    constantPatternSyntax = constantPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    constantPatternSyntax = constantPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return constantPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConstantPatternSyntax(base.Kind, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConstantPatternSyntax(base.Kind, expression, GetDiagnostics(), annotations);
        }

        public ConstantPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
        }

        static ConstantPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConstantPatternSyntax), (ObjectReader r) => new ConstantPatternSyntax(r));
        }
    }
}
