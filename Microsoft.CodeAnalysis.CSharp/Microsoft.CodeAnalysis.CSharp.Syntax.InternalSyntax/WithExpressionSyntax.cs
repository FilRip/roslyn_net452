using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class WithExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken withKeyword;

        internal readonly InitializerExpressionSyntax initializer;

        public ExpressionSyntax Expression => expression;

        public SyntaxToken WithKeyword => withKeyword;

        public InitializerExpressionSyntax Initializer => initializer;

        public WithExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(withKeyword);
            this.withKeyword = withKeyword;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public WithExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(withKeyword);
            this.withKeyword = withKeyword;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public WithExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(withKeyword);
            this.withKeyword = withKeyword;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => expression,
                1 => withKeyword,
                2 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.WithExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWithExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitWithExpression(this);
        }

        public WithExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer)
        {
            if (expression != Expression || withKeyword != WithKeyword || initializer != Initializer)
            {
                WithExpressionSyntax withExpressionSyntax = SyntaxFactory.WithExpression(expression, withKeyword, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    withExpressionSyntax = withExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    withExpressionSyntax = withExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return withExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new WithExpressionSyntax(base.Kind, expression, withKeyword, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new WithExpressionSyntax(base.Kind, expression, withKeyword, initializer, GetDiagnostics(), annotations);
        }

        public WithExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            withKeyword = node2;
            InitializerExpressionSyntax node3 = (InitializerExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            initializer = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(withKeyword);
            writer.WriteValue(initializer);
        }

        static WithExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(WithExpressionSyntax), (ObjectReader r) => new WithExpressionSyntax(r));
        }
    }
}
