using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NameColonSyntax : CSharpSyntaxNode
    {
        internal readonly IdentifierNameSyntax name;

        internal readonly SyntaxToken colonToken;

        public IdentifierNameSyntax Name => name;

        public SyntaxToken ColonToken => colonToken;

        public NameColonSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken colonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public NameColonSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken colonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public NameColonSyntax(SyntaxKind kind, IdentifierNameSyntax name, SyntaxToken colonToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => colonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNameColon(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNameColon(this);
        }

        public NameColonSyntax Update(IdentifierNameSyntax name, SyntaxToken colonToken)
        {
            if (name != Name || colonToken != ColonToken)
            {
                NameColonSyntax nameColonSyntax = SyntaxFactory.NameColon(name, colonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    nameColonSyntax = nameColonSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    nameColonSyntax = nameColonSyntax.WithAnnotationsGreen(annotations);
                }
                return nameColonSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NameColonSyntax(base.Kind, name, colonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NameColonSyntax(base.Kind, name, colonToken, GetDiagnostics(), annotations);
        }

        public NameColonSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            IdentifierNameSyntax node = (IdentifierNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            colonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(name);
            writer.WriteValue(colonToken);
        }

        static NameColonSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NameColonSyntax), (ObjectReader r) => new NameColonSyntax(r));
        }
    }
}
