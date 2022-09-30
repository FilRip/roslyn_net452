using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RangeExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax? leftOperand;

        internal readonly SyntaxToken operatorToken;

        internal readonly ExpressionSyntax? rightOperand;

        public ExpressionSyntax? LeftOperand => leftOperand;

        public SyntaxToken OperatorToken => operatorToken;

        public ExpressionSyntax? RightOperand => rightOperand;

        public RangeExpressionSyntax(SyntaxKind kind, ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (leftOperand != null)
            {
                AdjustFlagsAndWidth(leftOperand);
                this.leftOperand = leftOperand;
            }
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            if (rightOperand != null)
            {
                AdjustFlagsAndWidth(rightOperand);
                this.rightOperand = rightOperand;
            }
        }

        public RangeExpressionSyntax(SyntaxKind kind, ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (leftOperand != null)
            {
                AdjustFlagsAndWidth(leftOperand);
                this.leftOperand = leftOperand;
            }
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            if (rightOperand != null)
            {
                AdjustFlagsAndWidth(rightOperand);
                this.rightOperand = rightOperand;
            }
        }

        public RangeExpressionSyntax(SyntaxKind kind, ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand)
            : base(kind)
        {
            base.SlotCount = 3;
            if (leftOperand != null)
            {
                AdjustFlagsAndWidth(leftOperand);
                this.leftOperand = leftOperand;
            }
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            if (rightOperand != null)
            {
                AdjustFlagsAndWidth(rightOperand);
                this.rightOperand = rightOperand;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => leftOperand,
                1 => operatorToken,
                2 => rightOperand,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRangeExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRangeExpression(this);
        }

        public RangeExpressionSyntax Update(ExpressionSyntax leftOperand, SyntaxToken operatorToken, ExpressionSyntax rightOperand)
        {
            if (leftOperand != LeftOperand || operatorToken != OperatorToken || rightOperand != RightOperand)
            {
                RangeExpressionSyntax rangeExpressionSyntax = SyntaxFactory.RangeExpression(leftOperand, operatorToken, rightOperand);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    rangeExpressionSyntax = rangeExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    rangeExpressionSyntax = rangeExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return rangeExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RangeExpressionSyntax(base.Kind, leftOperand, operatorToken, rightOperand, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RangeExpressionSyntax(base.Kind, leftOperand, operatorToken, rightOperand, GetDiagnostics(), annotations);
        }

        public RangeExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                leftOperand = expressionSyntax;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorToken = node;
            ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax2 != null)
            {
                AdjustFlagsAndWidth(expressionSyntax2);
                rightOperand = expressionSyntax2;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(leftOperand);
            writer.WriteValue(operatorToken);
            writer.WriteValue(rightOperand);
        }

        static RangeExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RangeExpressionSyntax), (ObjectReader r) => new RangeExpressionSyntax(r));
        }
    }
}
