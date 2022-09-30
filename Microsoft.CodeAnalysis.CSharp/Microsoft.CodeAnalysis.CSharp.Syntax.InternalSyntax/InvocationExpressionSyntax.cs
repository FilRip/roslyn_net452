using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InvocationExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly ArgumentListSyntax argumentList;

        public ExpressionSyntax Expression => expression;

        public ArgumentListSyntax ArgumentList => argumentList;

        public InvocationExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax argumentList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public InvocationExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax argumentList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public InvocationExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax argumentList)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => expression,
                1 => argumentList,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInvocationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInvocationExpression(this);
        }

        public InvocationExpressionSyntax Update(ExpressionSyntax expression, ArgumentListSyntax argumentList)
        {
            if (expression != Expression || argumentList != ArgumentList)
            {
                InvocationExpressionSyntax invocationExpressionSyntax = SyntaxFactory.InvocationExpression(expression, argumentList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    invocationExpressionSyntax = invocationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    invocationExpressionSyntax = invocationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return invocationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InvocationExpressionSyntax(base.Kind, expression, argumentList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InvocationExpressionSyntax(base.Kind, expression, argumentList, GetDiagnostics(), annotations);
        }

        public InvocationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            ArgumentListSyntax node2 = (ArgumentListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            argumentList = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(argumentList);
        }

        static InvocationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InvocationExpressionSyntax), (ObjectReader r) => new InvocationExpressionSyntax(r));
        }
    }
}
