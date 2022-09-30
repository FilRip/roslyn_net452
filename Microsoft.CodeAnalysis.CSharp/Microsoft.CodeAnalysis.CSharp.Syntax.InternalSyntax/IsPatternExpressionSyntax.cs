using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class IsPatternExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken isKeyword;

        internal readonly PatternSyntax pattern;

        public ExpressionSyntax Expression => expression;

        public SyntaxToken IsKeyword => isKeyword;

        public PatternSyntax Pattern => pattern;

        public IsPatternExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(isKeyword);
            this.isKeyword = isKeyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public IsPatternExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(isKeyword);
            this.isKeyword = isKeyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public IsPatternExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(isKeyword);
            this.isKeyword = isKeyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => expression,
                1 => isKeyword,
                2 => pattern,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIsPatternExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitIsPatternExpression(this);
        }

        public IsPatternExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern)
        {
            if (expression != Expression || isKeyword != IsKeyword || pattern != Pattern)
            {
                IsPatternExpressionSyntax isPatternExpressionSyntax = SyntaxFactory.IsPatternExpression(expression, isKeyword, pattern);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    isPatternExpressionSyntax = isPatternExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    isPatternExpressionSyntax = isPatternExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return isPatternExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new IsPatternExpressionSyntax(base.Kind, expression, isKeyword, pattern, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new IsPatternExpressionSyntax(base.Kind, expression, isKeyword, pattern, GetDiagnostics(), annotations);
        }

        public IsPatternExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            isKeyword = node2;
            PatternSyntax node3 = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            pattern = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(isKeyword);
            writer.WriteValue(pattern);
        }

        static IsPatternExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(IsPatternExpressionSyntax), (ObjectReader r) => new IsPatternExpressionSyntax(r));
        }
    }
}
