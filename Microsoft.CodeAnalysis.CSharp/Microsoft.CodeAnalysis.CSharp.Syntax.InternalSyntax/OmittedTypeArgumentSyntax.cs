using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class OmittedTypeArgumentSyntax : TypeSyntax
    {
        internal readonly SyntaxToken omittedTypeArgumentToken;

        public SyntaxToken OmittedTypeArgumentToken => omittedTypeArgumentToken;

        public OmittedTypeArgumentSyntax(SyntaxKind kind, SyntaxToken omittedTypeArgumentToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(omittedTypeArgumentToken);
            this.omittedTypeArgumentToken = omittedTypeArgumentToken;
        }

        public OmittedTypeArgumentSyntax(SyntaxKind kind, SyntaxToken omittedTypeArgumentToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(omittedTypeArgumentToken);
            this.omittedTypeArgumentToken = omittedTypeArgumentToken;
        }

        public OmittedTypeArgumentSyntax(SyntaxKind kind, SyntaxToken omittedTypeArgumentToken)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(omittedTypeArgumentToken);
            this.omittedTypeArgumentToken = omittedTypeArgumentToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return omittedTypeArgumentToken;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.OmittedTypeArgumentSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitOmittedTypeArgument(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitOmittedTypeArgument(this);
        }

        public OmittedTypeArgumentSyntax Update(SyntaxToken omittedTypeArgumentToken)
        {
            if (omittedTypeArgumentToken != OmittedTypeArgumentToken)
            {
                OmittedTypeArgumentSyntax omittedTypeArgumentSyntax = SyntaxFactory.OmittedTypeArgument(omittedTypeArgumentToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    omittedTypeArgumentSyntax = omittedTypeArgumentSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    omittedTypeArgumentSyntax = omittedTypeArgumentSyntax.WithAnnotationsGreen(annotations);
                }
                return omittedTypeArgumentSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new OmittedTypeArgumentSyntax(base.Kind, omittedTypeArgumentToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new OmittedTypeArgumentSyntax(base.Kind, omittedTypeArgumentToken, GetDiagnostics(), annotations);
        }

        public OmittedTypeArgumentSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            omittedTypeArgumentToken = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(omittedTypeArgumentToken);
        }

        static OmittedTypeArgumentSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(OmittedTypeArgumentSyntax), (ObjectReader r) => new OmittedTypeArgumentSyntax(r));
        }
    }
}
