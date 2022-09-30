using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EnumDeclarationSyntax : BaseTypeDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken enumKeyword;

        internal readonly SyntaxToken identifier;

        internal readonly BaseListSyntax? baseList;

        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? members;

        internal readonly SyntaxToken closeBraceToken;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken EnumKeyword => enumKeyword;

        public override SyntaxToken Identifier => identifier;

        public override BaseListSyntax? BaseList => baseList;

        public override SyntaxToken OpenBraceToken => openBraceToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(members));

        public override SyntaxToken CloseBraceToken => closeBraceToken;

        public override SyntaxToken? SemicolonToken => semicolonToken;

        public EnumDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax? baseList, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(enumKeyword);
            this.enumKeyword = enumKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
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

        public EnumDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax? baseList, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(enumKeyword);
            this.enumKeyword = enumKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
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

        public EnumDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax? baseList, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(enumKeyword);
            this.enumKeyword = enumKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
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
                2 => enumKeyword,
                3 => identifier,
                4 => baseList,
                5 => openBraceToken,
                6 => members,
                7 => closeBraceToken,
                8 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEnumDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEnumDeclaration(this);
        }

        public EnumDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax baseList, SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (!(attributeLists != AttributeLists) && !(modifiers != Modifiers) && enumKeyword == EnumKeyword && identifier == Identifier && baseList == BaseList && openBraceToken == OpenBraceToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax> right = Members;
                if (!(members != right) && closeBraceToken == CloseBraceToken && semicolonToken == SemicolonToken)
                {
                    return this;
                }
            }
            EnumDeclarationSyntax enumDeclarationSyntax = SyntaxFactory.EnumDeclaration(attributeLists, modifiers, enumKeyword, identifier, baseList, openBraceToken, members, closeBraceToken, semicolonToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                enumDeclarationSyntax = enumDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                enumDeclarationSyntax = enumDeclarationSyntax.WithAnnotationsGreen(annotations);
            }
            return enumDeclarationSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EnumDeclarationSyntax(base.Kind, attributeLists, modifiers, enumKeyword, identifier, baseList, openBraceToken, members, closeBraceToken, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EnumDeclarationSyntax(base.Kind, attributeLists, modifiers, enumKeyword, identifier, baseList, openBraceToken, members, closeBraceToken, semicolonToken, GetDiagnostics(), annotations);
        }

        public EnumDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 9;
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
            enumKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            BaseListSyntax baseListSyntax = (BaseListSyntax)reader.ReadValue();
            if (baseListSyntax != null)
            {
                AdjustFlagsAndWidth(baseListSyntax);
                baseList = baseListSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            openBraceToken = node3;
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                members = greenNode3;
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
            writer.WriteValue(enumKeyword);
            writer.WriteValue(identifier);
            writer.WriteValue(baseList);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(members);
            writer.WriteValue(closeBraceToken);
            writer.WriteValue(semicolonToken);
        }

        static EnumDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EnumDeclarationSyntax), (ObjectReader r) => new EnumDeclarationSyntax(r));
        }
    }
}
