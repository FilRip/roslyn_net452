using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class BaseExpressionSyntax : InstanceExpressionSyntax
    {
        internal readonly SyntaxToken token;

        public SyntaxToken Token => token;

        public BaseExpressionSyntax(SyntaxKind kind, SyntaxToken token, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public BaseExpressionSyntax(SyntaxKind kind, SyntaxToken token, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public BaseExpressionSyntax(SyntaxKind kind, SyntaxToken token)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return token;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.BaseExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBaseExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitBaseExpression(this);
        }

        public BaseExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                BaseExpressionSyntax baseExpressionSyntax = SyntaxFactory.BaseExpression(token);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    baseExpressionSyntax = baseExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    baseExpressionSyntax = baseExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return baseExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new BaseExpressionSyntax(base.Kind, token, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new BaseExpressionSyntax(base.Kind, token, GetDiagnostics(), annotations);
        }

        public BaseExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            token = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(token);
        }

        static BaseExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(BaseExpressionSyntax), (ObjectReader r) => new BaseExpressionSyntax(r));
        }
    }
}
