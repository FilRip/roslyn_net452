using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ParameterSyntax : BaseParameterSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly TypeSyntax? type;

        internal readonly SyntaxToken identifier;

        internal readonly EqualsValueClauseSyntax? @default;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public override TypeSyntax? Type => type;

        public SyntaxToken Identifier => identifier;

        public EqualsValueClauseSyntax? Default => @default;

        public ParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
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
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (@default != null)
            {
                AdjustFlagsAndWidth(@default);
                this.@default = @default;
            }
        }

        public ParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default, SyntaxFactoryContext context)
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
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (@default != null)
            {
                AdjustFlagsAndWidth(@default);
                this.@default = @default;
            }
        }

        public ParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default)
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
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            if (@default != null)
            {
                AdjustFlagsAndWidth(@default);
                this.@default = @default;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => type,
                3 => identifier,
                4 => @default,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitParameter(this);
        }

        public ParameterSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax @default)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || type != Type || identifier != Identifier || @default != Default)
            {
                ParameterSyntax parameterSyntax = SyntaxFactory.Parameter(attributeLists, modifiers, type, identifier, @default);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    parameterSyntax = parameterSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    parameterSyntax = parameterSyntax.WithAnnotationsGreen(annotations);
                }
                return parameterSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ParameterSyntax(base.Kind, attributeLists, modifiers, type, identifier, @default, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ParameterSyntax(base.Kind, attributeLists, modifiers, type, identifier, @default, GetDiagnostics(), annotations);
        }

        public ParameterSyntax(ObjectReader reader)
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
            TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
            if (typeSyntax != null)
            {
                AdjustFlagsAndWidth(typeSyntax);
                type = typeSyntax;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
            EqualsValueClauseSyntax equalsValueClauseSyntax = (EqualsValueClauseSyntax)reader.ReadValue();
            if (equalsValueClauseSyntax != null)
            {
                AdjustFlagsAndWidth(equalsValueClauseSyntax);
                @default = equalsValueClauseSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(type);
            writer.WriteValue(identifier);
            writer.WriteValue(@default);
        }

        static ParameterSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ParameterSyntax), (ObjectReader r) => new ParameterSyntax(r));
        }
    }
}
