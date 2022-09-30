using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class UsingDirectiveSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken? globalKeyword;

        internal readonly SyntaxToken usingKeyword;

        internal readonly SyntaxToken? staticKeyword;

        internal readonly NameEqualsSyntax? alias;

        internal readonly NameSyntax name;

        internal readonly SyntaxToken semicolonToken;

        public SyntaxToken? GlobalKeyword => globalKeyword;

        public SyntaxToken UsingKeyword => usingKeyword;

        public SyntaxToken? StaticKeyword => staticKeyword;

        public NameEqualsSyntax? Alias => alias;

        public NameSyntax Name => name;

        public SyntaxToken SemicolonToken => semicolonToken;

        public UsingDirectiveSyntax(SyntaxKind kind, SyntaxToken? globalKeyword, SyntaxToken usingKeyword, SyntaxToken? staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            if (globalKeyword != null)
            {
                AdjustFlagsAndWidth(globalKeyword);
                this.globalKeyword = globalKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            if (staticKeyword != null)
            {
                AdjustFlagsAndWidth(staticKeyword);
                this.staticKeyword = staticKeyword;
            }
            if (alias != null)
            {
                AdjustFlagsAndWidth(alias);
                this.alias = alias;
            }
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public UsingDirectiveSyntax(SyntaxKind kind, SyntaxToken? globalKeyword, SyntaxToken usingKeyword, SyntaxToken? staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            if (globalKeyword != null)
            {
                AdjustFlagsAndWidth(globalKeyword);
                this.globalKeyword = globalKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            if (staticKeyword != null)
            {
                AdjustFlagsAndWidth(staticKeyword);
                this.staticKeyword = staticKeyword;
            }
            if (alias != null)
            {
                AdjustFlagsAndWidth(alias);
                this.alias = alias;
            }
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public UsingDirectiveSyntax(SyntaxKind kind, SyntaxToken? globalKeyword, SyntaxToken usingKeyword, SyntaxToken? staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 6;
            if (globalKeyword != null)
            {
                AdjustFlagsAndWidth(globalKeyword);
                this.globalKeyword = globalKeyword;
            }
            AdjustFlagsAndWidth(usingKeyword);
            this.usingKeyword = usingKeyword;
            if (staticKeyword != null)
            {
                AdjustFlagsAndWidth(staticKeyword);
                this.staticKeyword = staticKeyword;
            }
            if (alias != null)
            {
                AdjustFlagsAndWidth(alias);
                this.alias = alias;
            }
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => globalKeyword,
                1 => usingKeyword,
                2 => staticKeyword,
                3 => alias,
                4 => name,
                5 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitUsingDirective(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitUsingDirective(this);
        }

        public UsingDirectiveSyntax Update(SyntaxToken globalKeyword, SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameEqualsSyntax alias, NameSyntax name, SyntaxToken semicolonToken)
        {
            if (globalKeyword != GlobalKeyword || usingKeyword != UsingKeyword || staticKeyword != StaticKeyword || alias != Alias || name != Name || semicolonToken != SemicolonToken)
            {
                UsingDirectiveSyntax usingDirectiveSyntax = SyntaxFactory.UsingDirective(globalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    usingDirectiveSyntax = usingDirectiveSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    usingDirectiveSyntax = usingDirectiveSyntax.WithAnnotationsGreen(annotations);
                }
                return usingDirectiveSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new UsingDirectiveSyntax(base.Kind, globalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new UsingDirectiveSyntax(base.Kind, globalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken, GetDiagnostics(), annotations);
        }

        public UsingDirectiveSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                globalKeyword = syntaxToken;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            usingKeyword = node;
            SyntaxToken syntaxToken2 = (SyntaxToken)reader.ReadValue();
            if (syntaxToken2 != null)
            {
                AdjustFlagsAndWidth(syntaxToken2);
                staticKeyword = syntaxToken2;
            }
            NameEqualsSyntax nameEqualsSyntax = (NameEqualsSyntax)reader.ReadValue();
            if (nameEqualsSyntax != null)
            {
                AdjustFlagsAndWidth(nameEqualsSyntax);
                alias = nameEqualsSyntax;
            }
            NameSyntax node2 = (NameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            name = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            semicolonToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(globalKeyword);
            writer.WriteValue(usingKeyword);
            writer.WriteValue(staticKeyword);
            writer.WriteValue(alias);
            writer.WriteValue(name);
            writer.WriteValue(semicolonToken);
        }

        static UsingDirectiveSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(UsingDirectiveSyntax), (ObjectReader r) => new UsingDirectiveSyntax(r));
        }
    }
}
