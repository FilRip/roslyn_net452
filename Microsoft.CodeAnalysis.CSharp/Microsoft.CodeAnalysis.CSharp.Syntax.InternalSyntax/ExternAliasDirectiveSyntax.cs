using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ExternAliasDirectiveSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken externKeyword;

        internal readonly SyntaxToken aliasKeyword;

        internal readonly SyntaxToken identifier;

        internal readonly SyntaxToken semicolonToken;

        public SyntaxToken ExternKeyword => externKeyword;

        public SyntaxToken AliasKeyword => aliasKeyword;

        public SyntaxToken Identifier => identifier;

        public SyntaxToken SemicolonToken => semicolonToken;

        public ExternAliasDirectiveSyntax(SyntaxKind kind, SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(externKeyword);
            this.externKeyword = externKeyword;
            AdjustFlagsAndWidth(aliasKeyword);
            this.aliasKeyword = aliasKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ExternAliasDirectiveSyntax(SyntaxKind kind, SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(externKeyword);
            this.externKeyword = externKeyword;
            AdjustFlagsAndWidth(aliasKeyword);
            this.aliasKeyword = aliasKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ExternAliasDirectiveSyntax(SyntaxKind kind, SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(externKeyword);
            this.externKeyword = externKeyword;
            AdjustFlagsAndWidth(aliasKeyword);
            this.aliasKeyword = aliasKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => externKeyword,
                1 => aliasKeyword,
                2 => identifier,
                3 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitExternAliasDirective(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitExternAliasDirective(this);
        }

        public ExternAliasDirectiveSyntax Update(SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            if (externKeyword != ExternKeyword || aliasKeyword != AliasKeyword || identifier != Identifier || semicolonToken != SemicolonToken)
            {
                ExternAliasDirectiveSyntax externAliasDirectiveSyntax = SyntaxFactory.ExternAliasDirective(externKeyword, aliasKeyword, identifier, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    externAliasDirectiveSyntax = externAliasDirectiveSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    externAliasDirectiveSyntax = externAliasDirectiveSyntax.WithAnnotationsGreen(annotations);
                }
                return externAliasDirectiveSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ExternAliasDirectiveSyntax(base.Kind, externKeyword, aliasKeyword, identifier, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ExternAliasDirectiveSyntax(base.Kind, externKeyword, aliasKeyword, identifier, semicolonToken, GetDiagnostics(), annotations);
        }

        public ExternAliasDirectiveSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            externKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            aliasKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            identifier = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            semicolonToken = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(externKeyword);
            writer.WriteValue(aliasKeyword);
            writer.WriteValue(identifier);
            writer.WriteValue(semicolonToken);
        }

        static ExternAliasDirectiveSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ExternAliasDirectiveSyntax), (ObjectReader r) => new ExternAliasDirectiveSyntax(r));
        }
    }
}
