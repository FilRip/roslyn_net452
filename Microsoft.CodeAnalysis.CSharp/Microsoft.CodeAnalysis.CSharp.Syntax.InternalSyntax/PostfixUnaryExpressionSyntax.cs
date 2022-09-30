using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PostfixUnaryExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax operand;

        internal readonly SyntaxToken operatorToken;

        public ExpressionSyntax Operand => operand;

        public SyntaxToken OperatorToken => operatorToken;

        public PostfixUnaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax operand, SyntaxToken operatorToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
        }

        public PostfixUnaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax operand, SyntaxToken operatorToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
        }

        public PostfixUnaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax operand, SyntaxToken operatorToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => operand,
                1 => operatorToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PostfixUnaryExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPostfixUnaryExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPostfixUnaryExpression(this);
        }

        public PostfixUnaryExpressionSyntax Update(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            if (operand != Operand || operatorToken != OperatorToken)
            {
                PostfixUnaryExpressionSyntax postfixUnaryExpressionSyntax = SyntaxFactory.PostfixUnaryExpression(base.Kind, operand, operatorToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    postfixUnaryExpressionSyntax = postfixUnaryExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    postfixUnaryExpressionSyntax = postfixUnaryExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return postfixUnaryExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PostfixUnaryExpressionSyntax(base.Kind, operand, operatorToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PostfixUnaryExpressionSyntax(base.Kind, operand, operatorToken, GetDiagnostics(), annotations);
        }

        public PostfixUnaryExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operand = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(operand);
            writer.WriteValue(operatorToken);
        }

        static PostfixUnaryExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PostfixUnaryExpressionSyntax), (ObjectReader r) => new PostfixUnaryExpressionSyntax(r));
        }
    }
}
