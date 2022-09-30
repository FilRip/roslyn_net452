using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DelegateDeclarationSyntax : MemberDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken delegateKeyword;

        internal readonly TypeSyntax returnType;

        internal readonly SyntaxToken identifier;

        internal readonly TypeParameterListSyntax? typeParameterList;

        internal readonly ParameterListSyntax parameterList;

        internal readonly GreenNode? constraintClauses;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken DelegateKeyword => delegateKeyword;

        public TypeSyntax ReturnType => returnType;

        public SyntaxToken Identifier => identifier;

        public TypeParameterListSyntax? TypeParameterList => typeParameterList;

        public ParameterListSyntax ParameterList => parameterList;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax>(constraintClauses);

        public SyntaxToken SemicolonToken => semicolonToken;

        public DelegateDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, GreenNode? constraintClauses, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
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
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public DelegateDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, GreenNode? constraintClauses, SyntaxToken semicolonToken, SyntaxFactoryContext context)
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
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public DelegateDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, GreenNode? constraintClauses, SyntaxToken semicolonToken)
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
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (typeParameterList != null)
            {
                AdjustFlagsAndWidth(typeParameterList);
                this.typeParameterList = typeParameterList;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (constraintClauses != null)
            {
                AdjustFlagsAndWidth(constraintClauses);
                this.constraintClauses = constraintClauses;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => delegateKeyword,
                3 => returnType,
                4 => identifier,
                5 => typeParameterList,
                6 => parameterList,
                7 => constraintClauses,
                8 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDelegateDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDelegateDeclaration(this);
        }

        public DelegateDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || delegateKeyword != DelegateKeyword || returnType != ReturnType || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || constraintClauses != ConstraintClauses || semicolonToken != SemicolonToken)
            {
                DelegateDeclarationSyntax delegateDeclarationSyntax = SyntaxFactory.DelegateDeclaration(attributeLists, modifiers, delegateKeyword, returnType, identifier, typeParameterList, parameterList, constraintClauses, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    delegateDeclarationSyntax = delegateDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    delegateDeclarationSyntax = delegateDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return delegateDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DelegateDeclarationSyntax(base.Kind, attributeLists, modifiers, delegateKeyword, returnType, identifier, typeParameterList, parameterList, constraintClauses, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DelegateDeclarationSyntax(base.Kind, attributeLists, modifiers, delegateKeyword, returnType, identifier, typeParameterList, parameterList, constraintClauses, semicolonToken, GetDiagnostics(), annotations);
        }

        public DelegateDeclarationSyntax(ObjectReader reader)
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
            delegateKeyword = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            returnType = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            identifier = node3;
            TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)reader.ReadValue();
            if (typeParameterListSyntax != null)
            {
                AdjustFlagsAndWidth(typeParameterListSyntax);
                typeParameterList = typeParameterListSyntax;
            }
            ParameterListSyntax node4 = (ParameterListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            parameterList = node4;
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                constraintClauses = greenNode3;
            }
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            semicolonToken = node5;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(delegateKeyword);
            writer.WriteValue(returnType);
            writer.WriteValue(identifier);
            writer.WriteValue(typeParameterList);
            writer.WriteValue(parameterList);
            writer.WriteValue(constraintClauses);
            writer.WriteValue(semicolonToken);
        }

        static DelegateDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DelegateDeclarationSyntax), (ObjectReader r) => new DelegateDeclarationSyntax(r));
        }
    }
}
