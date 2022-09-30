using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EqualsValueClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken equalsToken;

        internal readonly ExpressionSyntax value;

        public SyntaxToken EqualsToken => equalsToken;

        public ExpressionSyntax Value => value;

        public EqualsValueClauseSyntax(SyntaxKind kind, SyntaxToken equalsToken, ExpressionSyntax value, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public EqualsValueClauseSyntax(SyntaxKind kind, SyntaxToken equalsToken, ExpressionSyntax value, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public EqualsValueClauseSyntax(SyntaxKind kind, SyntaxToken equalsToken, ExpressionSyntax value)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(value);
            this.value = value;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => equalsToken,
                1 => value,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEqualsValueClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEqualsValueClause(this);
        }

        public EqualsValueClauseSyntax Update(SyntaxToken equalsToken, ExpressionSyntax value)
        {
            if (equalsToken != EqualsToken || value != Value)
            {
                EqualsValueClauseSyntax equalsValueClauseSyntax = SyntaxFactory.EqualsValueClause(equalsToken, value);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    equalsValueClauseSyntax = equalsValueClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    equalsValueClauseSyntax = equalsValueClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return equalsValueClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EqualsValueClauseSyntax(base.Kind, equalsToken, value, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EqualsValueClauseSyntax(base.Kind, equalsToken, value, GetDiagnostics(), annotations);
        }

        public EqualsValueClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            equalsToken = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            value = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(equalsToken);
            writer.WriteValue(value);
        }

        static EqualsValueClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EqualsValueClauseSyntax), (ObjectReader r) => new EqualsValueClauseSyntax(r));
        }
    }
}
