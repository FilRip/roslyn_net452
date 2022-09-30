using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NamespaceDeclarationSyntax : MemberDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken namespaceKeyword;

        internal readonly NameSyntax name;

        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? externs;

        internal readonly GreenNode? usings;

        internal readonly GreenNode? members;

        internal readonly SyntaxToken closeBraceToken;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken NamespaceKeyword => namespaceKeyword;

        public NameSyntax Name => name;

        public SyntaxToken OpenBraceToken => openBraceToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax> Externs => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax>(externs);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax> Usings => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax>(usings);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax>(members);

        public SyntaxToken CloseBraceToken => closeBraceToken;

        public SyntaxToken? SemicolonToken => semicolonToken;

        public NamespaceDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, GreenNode? externs, GreenNode? usings, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 10;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(namespaceKeyword);
            this.namespaceKeyword = namespaceKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public NamespaceDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, GreenNode? externs, GreenNode? usings, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 10;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(namespaceKeyword);
            this.namespaceKeyword = namespaceKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public NamespaceDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, GreenNode? externs, GreenNode? usings, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 10;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(namespaceKeyword);
            this.namespaceKeyword = namespaceKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => namespaceKeyword,
                3 => name,
                4 => openBraceToken,
                5 => externs,
                6 => usings,
                7 => members,
                8 => closeBraceToken,
                9 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNamespaceDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNamespaceDeclaration(this);
        }

        public NamespaceDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax> externs, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || namespaceKeyword != NamespaceKeyword || name != Name || openBraceToken != OpenBraceToken || externs != Externs || usings != Usings || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(attributeLists, modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    namespaceDeclarationSyntax = namespaceDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    namespaceDeclarationSyntax = namespaceDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return namespaceDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NamespaceDeclarationSyntax(base.Kind, attributeLists, modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NamespaceDeclarationSyntax(base.Kind, attributeLists, modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken, GetDiagnostics(), annotations);
        }

        public NamespaceDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 10;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                modifiers = greenNode2;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            namespaceKeyword = node;
            NameSyntax node2 = (NameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            name = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            openBraceToken = node3;
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                externs = greenNode3;
            }
            GreenNode greenNode4 = (GreenNode)reader.ReadValue();
            if (greenNode4 != null)
            {
                AdjustFlagsAndWidth(greenNode4);
                usings = greenNode4;
            }
            GreenNode greenNode5 = (GreenNode)reader.ReadValue();
            if (greenNode5 != null)
            {
                AdjustFlagsAndWidth(greenNode5);
                members = greenNode5;
            }
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeBraceToken = node4;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                semicolonToken = syntaxToken;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(namespaceKeyword);
            writer.WriteValue(name);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(externs);
            writer.WriteValue(usings);
            writer.WriteValue(members);
            writer.WriteValue(closeBraceToken);
            writer.WriteValue(semicolonToken);
        }

        static NamespaceDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NamespaceDeclarationSyntax), (ObjectReader r) => new NamespaceDeclarationSyntax(r));
        }
    }
}
