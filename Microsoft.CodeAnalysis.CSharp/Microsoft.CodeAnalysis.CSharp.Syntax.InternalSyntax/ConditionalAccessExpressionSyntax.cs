using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConditionalAccessExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken operatorToken;

        internal readonly ExpressionSyntax whenNotNull;

        public ExpressionSyntax Expression => expression;

        public SyntaxToken OperatorToken => operatorToken;

        public ExpressionSyntax WhenNotNull => whenNotNull;

        public ConditionalAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(whenNotNull);
            this.whenNotNull = whenNotNull;
        }

        public ConditionalAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(whenNotNull);
            this.whenNotNull = whenNotNull;
        }

        public ConditionalAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(whenNotNull);
            this.whenNotNull = whenNotNull;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => expression,
                1 => operatorToken,
                2 => whenNotNull,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConditionalAccessExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConditionalAccessExpression(this);
        }

        public ConditionalAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull)
        {
            if (expression != Expression || operatorToken != OperatorToken || whenNotNull != WhenNotNull)
            {
                ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = SyntaxFactory.ConditionalAccessExpression(expression, operatorToken, whenNotNull);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    conditionalAccessExpressionSyntax = conditionalAccessExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    conditionalAccessExpressionSyntax = conditionalAccessExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return conditionalAccessExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConditionalAccessExpressionSyntax(base.Kind, expression, operatorToken, whenNotNull, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConditionalAccessExpressionSyntax(base.Kind, expression, operatorToken, whenNotNull, GetDiagnostics(), annotations);
        }

        public ConditionalAccessExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            whenNotNull = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(operatorToken);
            writer.WriteValue(whenNotNull);
        }

        static ConditionalAccessExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConditionalAccessExpressionSyntax), (ObjectReader r) => new ConditionalAccessExpressionSyntax(r));
        }
    }
}
