using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ExplicitInterfaceSpecifierSyntax : CSharpSyntaxNode
    {
        internal readonly NameSyntax name;

        internal readonly SyntaxToken dotToken;

        public NameSyntax Name => name;

        public SyntaxToken DotToken => dotToken;

        public ExplicitInterfaceSpecifierSyntax(SyntaxKind kind, NameSyntax name, SyntaxToken dotToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
        }

        public ExplicitInterfaceSpecifierSyntax(SyntaxKind kind, NameSyntax name, SyntaxToken dotToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
        }

        public ExplicitInterfaceSpecifierSyntax(SyntaxKind kind, NameSyntax name, SyntaxToken dotToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => dotToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitExplicitInterfaceSpecifier(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitExplicitInterfaceSpecifier(this);
        }

        public ExplicitInterfaceSpecifierSyntax Update(NameSyntax name, SyntaxToken dotToken)
        {
            if (name != Name || dotToken != DotToken)
            {
                ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax = SyntaxFactory.ExplicitInterfaceSpecifier(name, dotToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    explicitInterfaceSpecifierSyntax = explicitInterfaceSpecifierSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    explicitInterfaceSpecifierSyntax = explicitInterfaceSpecifierSyntax.WithAnnotationsGreen(annotations);
                }
                return explicitInterfaceSpecifierSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ExplicitInterfaceSpecifierSyntax(base.Kind, name, dotToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ExplicitInterfaceSpecifierSyntax(base.Kind, name, dotToken, GetDiagnostics(), annotations);
        }

        public ExplicitInterfaceSpecifierSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            NameSyntax node = (NameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            dotToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(name);
            writer.WriteValue(dotToken);
        }

        static ExplicitInterfaceSpecifierSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ExplicitInterfaceSpecifierSyntax), (ObjectReader r) => new ExplicitInterfaceSpecifierSyntax(r));
        }
    }
}
