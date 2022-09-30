using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class AliasQualifiedNameSyntax : NameSyntax
    {
        internal readonly IdentifierNameSyntax alias;

        internal readonly SyntaxToken colonColonToken;

        internal readonly SimpleNameSyntax name;

        public IdentifierNameSyntax Alias => alias;

        public SyntaxToken ColonColonToken => colonColonToken;

        public SimpleNameSyntax Name => name;

        public AliasQualifiedNameSyntax(SyntaxKind kind, IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(alias);
            this.alias = alias;
            AdjustFlagsAndWidth(colonColonToken);
            this.colonColonToken = colonColonToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public AliasQualifiedNameSyntax(SyntaxKind kind, IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(alias);
            this.alias = alias;
            AdjustFlagsAndWidth(colonColonToken);
            this.colonColonToken = colonColonToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public AliasQualifiedNameSyntax(SyntaxKind kind, IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(alias);
            this.alias = alias;
            AdjustFlagsAndWidth(colonColonToken);
            this.colonColonToken = colonColonToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => alias,
                1 => colonColonToken,
                2 => name,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAliasQualifiedName(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAliasQualifiedName(this);
        }

        public AliasQualifiedNameSyntax Update(IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name)
        {
            if (alias != Alias || colonColonToken != ColonColonToken || name != Name)
            {
                AliasQualifiedNameSyntax aliasQualifiedNameSyntax = SyntaxFactory.AliasQualifiedName(alias, colonColonToken, name);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    aliasQualifiedNameSyntax = aliasQualifiedNameSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    aliasQualifiedNameSyntax = aliasQualifiedNameSyntax.WithAnnotationsGreen(annotations);
                }
                return aliasQualifiedNameSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AliasQualifiedNameSyntax(base.Kind, alias, colonColonToken, name, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AliasQualifiedNameSyntax(base.Kind, alias, colonColonToken, name, GetDiagnostics(), annotations);
        }

        public AliasQualifiedNameSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            IdentifierNameSyntax node = (IdentifierNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            alias = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            colonColonToken = node2;
            SimpleNameSyntax node3 = (SimpleNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            name = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(alias);
            writer.WriteValue(colonColonToken);
            writer.WriteValue(name);
        }

        static AliasQualifiedNameSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AliasQualifiedNameSyntax), (ObjectReader r) => new AliasQualifiedNameSyntax(r));
        }
    }
}
