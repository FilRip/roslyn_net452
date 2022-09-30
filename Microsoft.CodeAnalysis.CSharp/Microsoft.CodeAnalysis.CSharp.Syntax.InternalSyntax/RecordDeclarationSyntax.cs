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

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RecordDeclarationSyntax : TypeDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken keyword;

        internal readonly SyntaxToken? classOrStructKeyword;

        internal readonly SyntaxToken identifier;

        internal readonly TypeParameterListSyntax? typeParameterList;

        internal readonly ParameterListSyntax? parameterList;

        internal readonly BaseListSyntax? baseList;

        internal readonly GreenNode? constraintClauses;

        internal readonly SyntaxToken? openBraceToken;

        internal readonly GreenNode? members;

        internal readonly SyntaxToken? closeBraceToken;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public override SyntaxToken Keyword => keyword;

        public SyntaxToken? ClassOrStructKeyword => classOrStructKeyword;

        public override SyntaxToken Identifier => identifier;

        public override TypeParameterListSyntax? TypeParameterList => typeParameterList;

        public ParameterListSyntax? ParameterList => parameterList;

        public override BaseListSyntax? BaseList => baseList;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax>(constraintClauses);

        public override SyntaxToken? OpenBraceToken => openBraceToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax>(members);

        public override SyntaxToken? CloseBraceToken => closeBraceToken;

        public override SyntaxToken? SemicolonToken => semicolonToken;

        public RecordDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken? classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken? openBraceToken, GreenNode? members, SyntaxToken? closeBraceToken, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 13;
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
            if (classOrStructKeyword != null)
            {
                AdjustFlagsAndWidth(classOrStructKeyword);
                this.classOrStructKeyword = classOrStructKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (parameterList != null)
            {
                AdjustFlagsAndWidth(parameterList);
                this.parameterList = parameterList;
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
            if (openBraceToken != null)
            {
                AdjustFlagsAndWidth(openBraceToken);
                this.openBraceToken = openBraceToken;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            if (closeBraceToken != null)
            {
                AdjustFlagsAndWidth(closeBraceToken);
                this.closeBraceToken = closeBraceToken;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public RecordDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken? classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken? openBraceToken, GreenNode? members, SyntaxToken? closeBraceToken, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 13;
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
            if (classOrStructKeyword != null)
            {
                AdjustFlagsAndWidth(classOrStructKeyword);
                this.classOrStructKeyword = classOrStructKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (parameterList != null)
            {
                AdjustFlagsAndWidth(parameterList);
                this.parameterList = parameterList;
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
            if (openBraceToken != null)
            {
                AdjustFlagsAndWidth(openBraceToken);
                this.openBraceToken = openBraceToken;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            if (closeBraceToken != null)
            {
                AdjustFlagsAndWidth(closeBraceToken);
                this.closeBraceToken = closeBraceToken;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public RecordDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken keyword, SyntaxToken? classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, GreenNode? constraintClauses, SyntaxToken? openBraceToken, GreenNode? members, SyntaxToken? closeBraceToken, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 13;
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
            if (classOrStructKeyword != null)
            {
                AdjustFlagsAndWidth(classOrStructKeyword);
                this.classOrStructKeyword = classOrStructKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            if (parameterList != null)
            {
                AdjustFlagsAndWidth(parameterList);
                this.parameterList = parameterList;
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
            if (openBraceToken != null)
            {
                AdjustFlagsAndWidth(openBraceToken);
                this.openBraceToken = openBraceToken;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            if (closeBraceToken != null)
            {
                AdjustFlagsAndWidth(closeBraceToken);
                this.closeBraceToken = closeBraceToken;
            }
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
                3 => classOrStructKeyword,
                4 => identifier,
                5 => typeParameterList,
                6 => parameterList,
                7 => baseList,
                8 => constraintClauses,
                9 => openBraceToken,
                10 => members,
                11 => closeBraceToken,
                12 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRecordDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRecordDeclaration(this);
        }

        public RecordDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken keyword, SyntaxToken classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, BaseListSyntax baseList, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || keyword != Keyword || classOrStructKeyword != ClassOrStructKeyword || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || baseList != BaseList || constraintClauses != ConstraintClauses || openBraceToken != OpenBraceToken || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                RecordDeclarationSyntax recordDeclarationSyntax = SyntaxFactory.RecordDeclaration(base.Kind, attributeLists, modifiers, keyword, classOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    recordDeclarationSyntax = recordDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    recordDeclarationSyntax = recordDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return recordDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RecordDeclarationSyntax(base.Kind, attributeLists, modifiers, keyword, classOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RecordDeclarationSyntax(base.Kind, attributeLists, modifiers, keyword, classOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken, GetDiagnostics(), annotations);
        }

        public RecordDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 13;
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
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                classOrStructKeyword = syntaxToken;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)reader.ReadValue();
            if (typeParameterListSyntax != null)
            {
                AdjustFlagsAndWidth(typeParameterListSyntax);
                typeParameterList = typeParameterListSyntax;
            }
            ParameterListSyntax parameterListSyntax = (ParameterListSyntax)reader.ReadValue();
            if (parameterListSyntax != null)
            {
                AdjustFlagsAndWidth(parameterListSyntax);
                parameterList = parameterListSyntax;
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
            SyntaxToken syntaxToken2 = (SyntaxToken)reader.ReadValue();
            if (syntaxToken2 != null)
            {
                AdjustFlagsAndWidth(syntaxToken2);
                openBraceToken = syntaxToken2;
            }
            GreenNode greenNode4 = (GreenNode)reader.ReadValue();
            if (greenNode4 != null)
            {
                AdjustFlagsAndWidth(greenNode4);
                members = greenNode4;
            }
            SyntaxToken syntaxToken3 = (SyntaxToken)reader.ReadValue();
            if (syntaxToken3 != null)
            {
                AdjustFlagsAndWidth(syntaxToken3);
                closeBraceToken = syntaxToken3;
            }
            SyntaxToken syntaxToken4 = (SyntaxToken)reader.ReadValue();
            if (syntaxToken4 != null)
            {
                AdjustFlagsAndWidth(syntaxToken4);
                semicolonToken = syntaxToken4;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(keyword);
            writer.WriteValue(classOrStructKeyword);
            writer.WriteValue(identifier);
            writer.WriteValue(typeParameterList);
            writer.WriteValue(parameterList);
            writer.WriteValue(baseList);
            writer.WriteValue(constraintClauses);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(members);
            writer.WriteValue(closeBraceToken);
            writer.WriteValue(semicolonToken);
        }

        static RecordDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RecordDeclarationSyntax), (ObjectReader r) => new RecordDeclarationSyntax(r));
        }
    }
}
