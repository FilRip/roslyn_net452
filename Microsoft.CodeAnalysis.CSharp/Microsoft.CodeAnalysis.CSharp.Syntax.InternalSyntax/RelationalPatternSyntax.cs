using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RelationalPatternSyntax : PatternSyntax
    {
        internal readonly SyntaxToken operatorToken;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken OperatorToken => operatorToken;

        public ExpressionSyntax Expression => expression;

        public RelationalPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public RelationalPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public RelationalPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => operatorToken,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RelationalPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRelationalPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRelationalPattern(this);
        }

        public RelationalPatternSyntax Update(SyntaxToken operatorToken, ExpressionSyntax expression)
        {
            if (operatorToken != OperatorToken || expression != Expression)
            {
                RelationalPatternSyntax relationalPatternSyntax = SyntaxFactory.RelationalPattern(operatorToken, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    relationalPatternSyntax = relationalPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    relationalPatternSyntax = relationalPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return relationalPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RelationalPatternSyntax(base.Kind, operatorToken, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RelationalPatternSyntax(base.Kind, operatorToken, expression, GetDiagnostics(), annotations);
        }

        public RelationalPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorToken = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(operatorToken);
            writer.WriteValue(expression);
        }

        static RelationalPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RelationalPatternSyntax), (ObjectReader r) => new RelationalPatternSyntax(r));
        }
    }
}
