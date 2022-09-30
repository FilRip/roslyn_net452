using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NameEqualsSyntax : CSharpSyntaxNode
    {
        internal readonly IdentifierNameSyntax name;

        internal readonly SyntaxToken equalsToken;

        public IdentifierNameSyntax Name => name;

        public SyntaxToken EqualsToken => equalsToken;

        public NameEqualsSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken equalsToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
        }

        public NameEqualsSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken equalsToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
        }

        public NameEqualsSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken equalsToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => equalsToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNameEquals(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNameEquals(this);
        }

        public NameEqualsSyntax Update(IdentifierNameSyntax name, SyntaxToken equalsToken)
        {
            if (name != Name || equalsToken != EqualsToken)
            {
                NameEqualsSyntax nameEqualsSyntax = SyntaxFactory.NameEquals(name, equalsToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    nameEqualsSyntax = nameEqualsSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    nameEqualsSyntax = nameEqualsSyntax.WithAnnotationsGreen(annotations);
                }
                return nameEqualsSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NameEqualsSyntax(base.Kind, name, equalsToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NameEqualsSyntax(base.Kind, name, equalsToken, GetDiagnostics(), annotations);
        }

        public NameEqualsSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            IdentifierNameSyntax node = (IdentifierNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            equalsToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(name);
            writer.WriteValue(equalsToken);
        }

        static NameEqualsSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NameEqualsSyntax), (ObjectReader r) => new NameEqualsSyntax(r));
        }
    }
}
