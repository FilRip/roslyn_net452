using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DiscardPatternSyntax : PatternSyntax
    {
        internal readonly SyntaxToken underscoreToken;

        public SyntaxToken UnderscoreToken => underscoreToken;

        public DiscardPatternSyntax(SyntaxKind kind, SyntaxToken underscoreToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(underscoreToken);
            this.underscoreToken = underscoreToken;
        }

        public DiscardPatternSyntax(SyntaxKind kind, SyntaxToken underscoreToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(underscoreToken);
            this.underscoreToken = underscoreToken;
        }

        public DiscardPatternSyntax(SyntaxKind kind, SyntaxToken underscoreToken)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(underscoreToken);
            this.underscoreToken = underscoreToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return underscoreToken;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDiscardPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDiscardPattern(this);
        }

        public DiscardPatternSyntax Update(SyntaxToken underscoreToken)
        {
            if (underscoreToken != UnderscoreToken)
            {
                DiscardPatternSyntax discardPatternSyntax = SyntaxFactory.DiscardPattern(underscoreToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    discardPatternSyntax = discardPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    discardPatternSyntax = discardPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return discardPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DiscardPatternSyntax(base.Kind, underscoreToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DiscardPatternSyntax(base.Kind, underscoreToken, GetDiagnostics(), annotations);
        }

        public DiscardPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            underscoreToken = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(underscoreToken);
        }

        static DiscardPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DiscardPatternSyntax), (ObjectReader r) => new DiscardPatternSyntax(r));
        }
    }
}
