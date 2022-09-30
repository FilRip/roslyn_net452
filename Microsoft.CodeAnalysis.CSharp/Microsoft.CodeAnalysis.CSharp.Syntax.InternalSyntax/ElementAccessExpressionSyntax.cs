using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ElementAccessExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly BracketedArgumentListSyntax argumentList;

        public ExpressionSyntax Expression => expression;

        public BracketedArgumentListSyntax ArgumentList => argumentList;

        public ElementAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, BracketedArgumentListSyntax argumentList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public ElementAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, BracketedArgumentListSyntax argumentList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public ElementAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, BracketedArgumentListSyntax argumentList)
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
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitElementAccessExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitElementAccessExpression(this);
        }

        public ElementAccessExpressionSyntax Update(ExpressionSyntax expression, BracketedArgumentListSyntax argumentList)
        {
            if (expression != Expression || argumentList != ArgumentList)
            {
                ElementAccessExpressionSyntax elementAccessExpressionSyntax = SyntaxFactory.ElementAccessExpression(expression, argumentList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    elementAccessExpressionSyntax = elementAccessExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    elementAccessExpressionSyntax = elementAccessExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return elementAccessExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ElementAccessExpressionSyntax(base.Kind, expression, argumentList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ElementAccessExpressionSyntax(base.Kind, expression, argumentList, GetDiagnostics(), annotations);
        }

        public ElementAccessExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            BracketedArgumentListSyntax node2 = (BracketedArgumentListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            argumentList = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(argumentList);
        }

        static ElementAccessExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ElementAccessExpressionSyntax), (ObjectReader r) => new ElementAccessExpressionSyntax(r));
        }
    }
}
