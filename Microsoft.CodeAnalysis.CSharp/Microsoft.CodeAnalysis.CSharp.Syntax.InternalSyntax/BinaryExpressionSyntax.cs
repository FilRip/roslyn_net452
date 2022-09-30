using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax left;

        internal readonly SyntaxToken operatorToken;

        internal readonly ExpressionSyntax right;

        public ExpressionSyntax Left => left;

        public SyntaxToken OperatorToken => operatorToken;

        public ExpressionSyntax Right => right;

        public BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => left,
                1 => operatorToken,
                2 => right,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBinaryExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }

        public BinaryExpressionSyntax Update(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            if (left != Left || operatorToken != OperatorToken || right != Right)
            {
                BinaryExpressionSyntax binaryExpressionSyntax = SyntaxFactory.BinaryExpression(base.Kind, left, operatorToken, right);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    binaryExpressionSyntax = binaryExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    binaryExpressionSyntax = binaryExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return binaryExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new BinaryExpressionSyntax(base.Kind, left, operatorToken, right, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new BinaryExpressionSyntax(base.Kind, left, operatorToken, right, GetDiagnostics(), annotations);
        }

        public BinaryExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            left = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            right = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(left);
            writer.WriteValue(operatorToken);
            writer.WriteValue(right);
        }

        static BinaryExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(BinaryExpressionSyntax), (ObjectReader r) => new BinaryExpressionSyntax(r));
        }
    }
}
