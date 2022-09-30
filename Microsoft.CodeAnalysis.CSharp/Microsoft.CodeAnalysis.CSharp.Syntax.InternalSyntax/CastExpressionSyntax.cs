using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class CastExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly TypeSyntax type;

        internal readonly SyntaxToken closeParenToken;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken OpenParenToken => openParenToken;

        public TypeSyntax Type => type;

        public SyntaxToken CloseParenToken => closeParenToken;

        public ExpressionSyntax Expression => expression;

        public CastExpressionSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public CastExpressionSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public CastExpressionSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => type,
                2 => closeParenToken,
                3 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCastExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCastExpression(this);
        }

        public CastExpressionSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression)
        {
            if (openParenToken != OpenParenToken || type != Type || closeParenToken != CloseParenToken || expression != Expression)
            {
                CastExpressionSyntax castExpressionSyntax = SyntaxFactory.CastExpression(openParenToken, type, closeParenToken, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    castExpressionSyntax = castExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    castExpressionSyntax = castExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return castExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CastExpressionSyntax(base.Kind, openParenToken, type, closeParenToken, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CastExpressionSyntax(base.Kind, openParenToken, type, closeParenToken, expression, GetDiagnostics(), annotations);
        }

        public CastExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openParenToken = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeParenToken = node3;
            ExpressionSyntax node4 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            expression = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(type);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(expression);
        }

        static CastExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CastExpressionSyntax), (ObjectReader r) => new CastExpressionSyntax(r));
        }
    }
}
