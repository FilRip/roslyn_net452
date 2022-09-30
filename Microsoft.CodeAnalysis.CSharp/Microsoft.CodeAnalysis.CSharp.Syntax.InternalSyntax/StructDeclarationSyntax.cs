using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class StructDeclarationSyntax : TypeDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken keyword;

        internal readonly SyntaxToken identifier;

        internal readonly TypeParameterListSyntax? typeParameterList;

        internal readonly BaseListSyntax? baseList;

        internal readonly GreenNode? constraintClauses;

        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? members;

        internal readonly SyntaxToken closeBraceToken;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public override SyntaxToken Keyword => keyword;

        public override SyntaxToken Identifier => identifier;

        public override TypeParameterListSyntax? TypeParameterList => typeParameterList;

        public override BaseListSyntax? BaseList => baseList;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax>(constraintClauses);

        public override SyntaxToken OpenBraceToken => openBraceToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax>(members);

        public override SyntaxToken CloseBraceToken => closeBraceToken;

        public override SyntaxToken? SemicolonToken => semicolonToken;

        public StructDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 11;
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
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
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

        public StructDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 11;
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
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
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

        public StructDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken openBraceToken, GreenNode? members, SyntaxToken closeBraceToken, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 11;
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
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (baseList != null)
            {
                AdjustFlagsAndWidth(baseList);
                this.baseList = baseList;
            }
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
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
                2 => keyword,
                3 => identifier,
                4 => typeParameterList,
                5 => baseList,
                6 => constraintClauses,
                7 => openBraceToken,
                8 => members,
                9 => closeBraceToken,
                10 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitStructDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitStructDeclaration(this);
        }

        public StructDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, BaseListSyntax baseList, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || keyword != Keyword || identifier != Identifier || typeParameterList != TypeParameterList || baseList != BaseList || constraintClauses != ConstraintClauses || openBraceToken != OpenBraceToken || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                StructDeclarationSyntax structDeclarationSyntax = SyntaxFactory.StructDeclaration(attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    structDeclarationSyntax = structDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    structDeclarationSyntax = structDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return structDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new StructDeclarationSyntax(base.Kind, attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new StructDeclarationSyntax(base.Kind, attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken, GetDiagnostics(), annotations);
        }

        public StructDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 11;
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
            keyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)reader.ReadValue();
            if (typeParameterListSyntax != null)
            {
                AdjustFlagsAndWidth(typeParameterListSyntax);
                typeParameterList = typeParameterListSyntax;
            }
            BaseListSyntax baseListSyntax = (BaseListSyntax)reader.ReadValue();
            if (baseListSyntax != null)
            {
                AdjustFlagsAndWidth(baseListSyntax);
                baseList = baseListSyntax;
            }
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                constraintClauses = greenNode3;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            openBraceToken = node3;
            GreenNode greenNode4 = (GreenNode)reader.ReadValue();
            if (greenNode4 != null)
            {
                AdjustFlagsAndWidth(greenNode4);
                members = greenNode4;
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
            writer.WriteValue(keyword);
            writer.WriteValue(identifier);
            writer.WriteValue(typeParameterList);
            writer.WriteValue(baseList);
            writer.WriteValue(constraintClauses);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(members);
            writer.WriteValue(closeBraceToken);
            writer.WriteValue(semicolonToken);
        }

        static StructDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(StructDeclarationSyntax), (ObjectReader r) => new StructDeclarationSyntax(r));
        }
    }
}
