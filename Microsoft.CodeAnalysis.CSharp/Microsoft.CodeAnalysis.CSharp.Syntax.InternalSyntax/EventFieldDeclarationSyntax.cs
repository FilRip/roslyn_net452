using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EventFieldDeclarationSyntax : BaseFieldDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken eventKeyword;

        internal readonly VariableDeclarationSyntax declaration;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken EventKeyword => eventKeyword;

        public override VariableDeclarationSyntax Declaration => declaration;

        public override SyntaxToken SemicolonToken => semicolonToken;

        public EventFieldDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
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
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public EventFieldDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
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
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public EventFieldDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 5;
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
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => eventKeyword,
                3 => declaration,
                4 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEventFieldDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEventFieldDeclaration(this);
        }

        public EventFieldDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || eventKeyword != EventKeyword || declaration != Declaration || semicolonToken != SemicolonToken)
            {
                EventFieldDeclarationSyntax eventFieldDeclarationSyntax = SyntaxFactory.EventFieldDeclaration(attributeLists, modifiers, eventKeyword, declaration, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    eventFieldDeclarationSyntax = eventFieldDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    eventFieldDeclarationSyntax = eventFieldDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return eventFieldDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EventFieldDeclarationSyntax(base.Kind, attributeLists, modifiers, eventKeyword, declaration, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EventFieldDeclarationSyntax(base.Kind, attributeLists, modifiers, eventKeyword, declaration, semicolonToken, GetDiagnostics(), annotations);
        }

        public EventFieldDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
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
            VariableDeclarationSyntax node2 = (VariableDeclarationSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            declaration = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            semicolonToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(eventKeyword);
            writer.WriteValue(declaration);
            writer.WriteValue(semicolonToken);
        }

        static EventFieldDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EventFieldDeclarationSyntax), (ObjectReader r) => new EventFieldDeclarationSyntax(r));
        }
    }
}
