using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EnumMemberDeclarationSyntax : MemberDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken identifier;

        internal readonly EqualsValueClauseSyntax? equalsValue;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken Identifier => identifier;

        public EqualsValueClauseSyntax? EqualsValue => equalsValue;

        public EnumMemberDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
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
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (equalsValue != null)
            {
                AdjustFlagsAndWidth(equalsValue);
                this.equalsValue = equalsValue;
            }
        }

        public EnumMemberDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
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
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (equalsValue != null)
            {
                AdjustFlagsAndWidth(equalsValue);
                this.equalsValue = equalsValue;
            }
        }

        public EnumMemberDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue)
            : base(kind)
        {
            base.SlotCount = 4;
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
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (equalsValue != null)
            {
                AdjustFlagsAndWidth(equalsValue);
                this.equalsValue = equalsValue;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => identifier,
                3 => equalsValue,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEnumMemberDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEnumMemberDeclaration(this);
        }

        public EnumMemberDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken identifier, EqualsValueClauseSyntax equalsValue)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || identifier != Identifier || equalsValue != EqualsValue)
            {
                EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = SyntaxFactory.EnumMemberDeclaration(attributeLists, modifiers, identifier, equalsValue);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    enumMemberDeclarationSyntax = enumMemberDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    enumMemberDeclarationSyntax = enumMemberDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return enumMemberDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EnumMemberDeclarationSyntax(base.Kind, attributeLists, modifiers, identifier, equalsValue, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EnumMemberDeclarationSyntax(base.Kind, attributeLists, modifiers, identifier, equalsValue, GetDiagnostics(), annotations);
        }

        public EnumMemberDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
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
            identifier = node;
            EqualsValueClauseSyntax equalsValueClauseSyntax = (EqualsValueClauseSyntax)reader.ReadValue();
            if (equalsValueClauseSyntax != null)
            {
                AdjustFlagsAndWidth(equalsValueClauseSyntax);
                equalsValue = equalsValueClauseSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(identifier);
            writer.WriteValue(equalsValue);
        }

        static EnumMemberDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EnumMemberDeclarationSyntax), (ObjectReader r) => new EnumMemberDeclarationSyntax(r));
        }
    }
}
