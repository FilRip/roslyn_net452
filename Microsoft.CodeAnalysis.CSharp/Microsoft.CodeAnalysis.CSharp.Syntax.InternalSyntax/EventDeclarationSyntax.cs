using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EventDeclarationSyntax : BasePropertyDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken eventKeyword;

        internal readonly TypeSyntax type;

        internal readonly ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;

        internal readonly SyntaxToken identifier;

        internal readonly AccessorListSyntax? accessorList;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken EventKeyword => eventKeyword;

        public override TypeSyntax Type => type;

        public override ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => explicitInterfaceSpecifier;

        public SyntaxToken Identifier => identifier;

        public override AccessorListSyntax? AccessorList => accessorList;

        public SyntaxToken? SemicolonToken => semicolonToken;

        public EventDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 8;
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
            AdjustFlagsAndWidth(eventKeyword);
            this.eventKeyword = eventKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (explicitInterfaceSpecifier != null)
            {
                AdjustFlagsAndWidth(explicitInterfaceSpecifier);
                this.explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (accessorList != null)
            {
                AdjustFlagsAndWidth(accessorList);
                this.accessorList = accessorList;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public EventDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 8;
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
            AdjustFlagsAndWidth(eventKeyword);
            this.eventKeyword = eventKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (explicitInterfaceSpecifier != null)
            {
                AdjustFlagsAndWidth(explicitInterfaceSpecifier);
                this.explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (accessorList != null)
            {
                AdjustFlagsAndWidth(accessorList);
                this.accessorList = accessorList;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public EventDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 8;
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
            AdjustFlagsAndWidth(eventKeyword);
            this.eventKeyword = eventKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (explicitInterfaceSpecifier != null)
            {
                AdjustFlagsAndWidth(explicitInterfaceSpecifier);
                this.explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (accessorList != null)
            {
                AdjustFlagsAndWidth(accessorList);
                this.accessorList = accessorList;
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
                2 => eventKeyword,
                3 => type,
                4 => explicitInterfaceSpecifier,
                5 => identifier,
                6 => accessorList,
                7 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEventDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEventDeclaration(this);
        }

        public EventDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax accessorList, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || eventKeyword != EventKeyword || type != Type || explicitInterfaceSpecifier != ExplicitInterfaceSpecifier || identifier != Identifier || accessorList != AccessorList || semicolonToken != SemicolonToken)
            {
                EventDeclarationSyntax eventDeclarationSyntax = SyntaxFactory.EventDeclaration(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    eventDeclarationSyntax = eventDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    eventDeclarationSyntax = eventDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return eventDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EventDeclarationSyntax(base.Kind, attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EventDeclarationSyntax(base.Kind, attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, semicolonToken, GetDiagnostics(), annotations);
        }

        public EventDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 8;
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
            eventKeyword = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax = (ExplicitInterfaceSpecifierSyntax)reader.ReadValue();
            if (explicitInterfaceSpecifierSyntax != null)
            {
                AdjustFlagsAndWidth(explicitInterfaceSpecifierSyntax);
                explicitInterfaceSpecifier = explicitInterfaceSpecifierSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            identifier = node3;
            AccessorListSyntax accessorListSyntax = (AccessorListSyntax)reader.ReadValue();
            if (accessorListSyntax != null)
            {
                AdjustFlagsAndWidth(accessorListSyntax);
                accessorList = accessorListSyntax;
            }
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
            writer.WriteValue(eventKeyword);
            writer.WriteValue(type);
            writer.WriteValue(explicitInterfaceSpecifier);
            writer.WriteValue(identifier);
            writer.WriteValue(accessorList);
            writer.WriteValue(semicolonToken);
        }

        static EventDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EventDeclarationSyntax), (ObjectReader r) => new EventDeclarationSyntax(r));
        }
    }
}
