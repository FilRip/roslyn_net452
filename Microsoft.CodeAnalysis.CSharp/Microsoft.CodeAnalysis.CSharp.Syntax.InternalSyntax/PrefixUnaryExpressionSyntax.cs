using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PrefixUnaryExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken operatorToken;

        internal readonly ExpressionSyntax operand;

        public SyntaxToken OperatorToken => operatorToken;

        public ExpressionSyntax Operand => operand;

        public PrefixUnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
        }

        public PrefixUnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
        }

        public PrefixUnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(operand);
            this.operand = operand;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => operatorToken,
                1 => operand,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PrefixUnaryExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPrefixUnaryExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPrefixUnaryExpression(this);
        }

        public PrefixUnaryExpressionSyntax Update(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            if (operatorToken != OperatorToken || operand != Operand)
            {
                PrefixUnaryExpressionSyntax prefixUnaryExpressionSyntax = SyntaxFactory.PrefixUnaryExpression(base.Kind, operatorToken, operand);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    prefixUnaryExpressionSyntax = prefixUnaryExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    prefixUnaryExpressionSyntax = prefixUnaryExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return prefixUnaryExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PrefixUnaryExpressionSyntax(base.Kind, operatorToken, operand, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PrefixUnaryExpressionSyntax(base.Kind, operatorToken, operand, GetDiagnostics(), annotations);
        }

        public PrefixUnaryExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorToken = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operand = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(operatorToken);
            writer.WriteValue(operand);
        }

        static PrefixUnaryExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PrefixUnaryExpressionSyntax), (ObjectReader r) => new PrefixUnaryExpressionSyntax(r));
        }
    }
}
