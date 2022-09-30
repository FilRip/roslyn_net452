using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConditionalExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax condition;

        internal readonly SyntaxToken questionToken;

        internal readonly ExpressionSyntax whenTrue;

        internal readonly SyntaxToken colonToken;

        internal readonly ExpressionSyntax whenFalse;

        public ExpressionSyntax Condition => condition;

        public SyntaxToken QuestionToken => questionToken;

        public ExpressionSyntax WhenTrue => whenTrue;

        public SyntaxToken ColonToken => colonToken;

        public ExpressionSyntax WhenFalse => whenFalse;

        public ConditionalExpressionSyntax(SyntaxKind kind, ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
            AdjustFlagsAndWidth(whenTrue);
            this.whenTrue = whenTrue;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(whenFalse);
            this.whenFalse = whenFalse;
        }

        public ConditionalExpressionSyntax(SyntaxKind kind, ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
            AdjustFlagsAndWidth(whenTrue);
            this.whenTrue = whenTrue;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(whenFalse);
            this.whenFalse = whenFalse;
        }

        public ConditionalExpressionSyntax(SyntaxKind kind, ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
            AdjustFlagsAndWidth(whenTrue);
            this.whenTrue = whenTrue;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(whenFalse);
            this.whenFalse = whenFalse;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => condition,
                1 => questionToken,
                2 => whenTrue,
                3 => colonToken,
                4 => whenFalse,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConditionalExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConditionalExpression(this);
        }

        public ConditionalExpressionSyntax Update(ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse)
        {
            if (condition != Condition || questionToken != QuestionToken || whenTrue != WhenTrue || colonToken != ColonToken || whenFalse != WhenFalse)
            {
                ConditionalExpressionSyntax conditionalExpressionSyntax = SyntaxFactory.ConditionalExpression(condition, questionToken, whenTrue, colonToken, whenFalse);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    conditionalExpressionSyntax = conditionalExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    conditionalExpressionSyntax = conditionalExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return conditionalExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConditionalExpressionSyntax(base.Kind, condition, questionToken, whenTrue, colonToken, whenFalse, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConditionalExpressionSyntax(base.Kind, condition, questionToken, whenTrue, colonToken, whenFalse, GetDiagnostics(), annotations);
        }

        public ConditionalExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            condition = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            questionToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            whenTrue = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            colonToken = node4;
            ExpressionSyntax node5 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            whenFalse = node5;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(condition);
            writer.WriteValue(questionToken);
            writer.WriteValue(whenTrue);
            writer.WriteValue(colonToken);
            writer.WriteValue(whenFalse);
        }

        static ConditionalExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConditionalExpressionSyntax), (ObjectReader r) => new ConditionalExpressionSyntax(r));
        }
    }
}
