using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PredefinedTypeSyntax : TypeSyntax
    {
        internal readonly SyntaxToken keyword;

        public SyntaxToken Keyword => keyword;

        public PredefinedTypeSyntax(SyntaxKind kind, SyntaxToken keyword, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
        }

        public PredefinedTypeSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
        }

        public PredefinedTypeSyntax(SyntaxKind kind, SyntaxToken keyword)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return keyword;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPredefinedType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPredefinedType(this);
        }

        public PredefinedTypeSyntax Update(SyntaxToken keyword)
        {
            if (keyword != Keyword)
            {
                PredefinedTypeSyntax predefinedTypeSyntax = SyntaxFactory.PredefinedType(keyword);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    predefinedTypeSyntax = predefinedTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    predefinedTypeSyntax = predefinedTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return predefinedTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PredefinedTypeSyntax(base.Kind, keyword, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PredefinedTypeSyntax(base.Kind, keyword, GetDiagnostics(), annotations);
        }

        public PredefinedTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            keyword = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(keyword);
        }

        static PredefinedTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PredefinedTypeSyntax), (ObjectReader r) => new PredefinedTypeSyntax(r));
        }
    }
}
