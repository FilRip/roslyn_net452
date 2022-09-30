using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DefaultConstraintSyntax : TypeParameterConstraintSyntax
    {
        internal readonly SyntaxToken defaultKeyword;

        public SyntaxToken DefaultKeyword => defaultKeyword;

        public DefaultConstraintSyntax(SyntaxKind kind, SyntaxToken defaultKeyword, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(defaultKeyword);
            this.defaultKeyword = defaultKeyword;
        }

        public DefaultConstraintSyntax(SyntaxKind kind, SyntaxToken defaultKeyword, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(defaultKeyword);
            this.defaultKeyword = defaultKeyword;
        }

        public DefaultConstraintSyntax(SyntaxKind kind, SyntaxToken defaultKeyword)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(defaultKeyword);
            this.defaultKeyword = defaultKeyword;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return defaultKeyword;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DefaultConstraintSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDefaultConstraint(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDefaultConstraint(this);
        }

        public DefaultConstraintSyntax Update(SyntaxToken defaultKeyword)
        {
            if (defaultKeyword != DefaultKeyword)
            {
                DefaultConstraintSyntax defaultConstraintSyntax = SyntaxFactory.DefaultConstraint(defaultKeyword);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    defaultConstraintSyntax = defaultConstraintSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    defaultConstraintSyntax = defaultConstraintSyntax.WithAnnotationsGreen(annotations);
                }
                return defaultConstraintSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DefaultConstraintSyntax(base.Kind, defaultKeyword, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DefaultConstraintSyntax(base.Kind, defaultKeyword, GetDiagnostics(), annotations);
        }

        public DefaultConstraintSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            defaultKeyword = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(defaultKeyword);
        }

        static DefaultConstraintSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DefaultConstraintSyntax), (ObjectReader r) => new DefaultConstraintSyntax(r));
        }
    }
}
