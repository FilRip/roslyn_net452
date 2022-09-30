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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class AttributeArgumentSyntax : CSharpSyntaxNode
    {
        internal readonly NameEqualsSyntax? nameEquals;

        internal readonly NameColonSyntax? nameColon;

        internal readonly ExpressionSyntax expression;

        public NameEqualsSyntax? NameEquals => nameEquals;

        public NameColonSyntax? NameColon => nameColon;

        public ExpressionSyntax Expression => expression;

        public AttributeArgumentSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, NameColonSyntax? nameColon, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AttributeArgumentSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, NameColonSyntax? nameColon, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AttributeArgumentSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, NameColonSyntax? nameColon, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 3;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => nameEquals,
                1 => nameColon,
                2 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAttributeArgument(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAttributeArgument(this);
        }

        public AttributeArgumentSyntax Update(NameEqualsSyntax nameEquals, NameColonSyntax nameColon, ExpressionSyntax expression)
        {
            if (nameEquals != NameEquals || nameColon != NameColon || expression != Expression)
            {
                AttributeArgumentSyntax attributeArgumentSyntax = SyntaxFactory.AttributeArgument(nameEquals, nameColon, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    attributeArgumentSyntax = attributeArgumentSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    attributeArgumentSyntax = attributeArgumentSyntax.WithAnnotationsGreen(annotations);
                }
                return attributeArgumentSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AttributeArgumentSyntax(base.Kind, nameEquals, nameColon, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AttributeArgumentSyntax(base.Kind, nameEquals, nameColon, expression, GetDiagnostics(), annotations);
        }

        public AttributeArgumentSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            NameEqualsSyntax nameEqualsSyntax = (NameEqualsSyntax)reader.ReadValue();
            if (nameEqualsSyntax != null)
            {
                AdjustFlagsAndWidth(nameEqualsSyntax);
                nameEquals = nameEqualsSyntax;
            }
            NameColonSyntax nameColonSyntax = (NameColonSyntax)reader.ReadValue();
            if (nameColonSyntax != null)
            {
                AdjustFlagsAndWidth(nameColonSyntax);
                nameColon = nameColonSyntax;
            }
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(nameEquals);
            writer.WriteValue(nameColon);
            writer.WriteValue(expression);
        }

        static AttributeArgumentSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AttributeArgumentSyntax), (ObjectReader r) => new AttributeArgumentSyntax(r));
        }
    }
}
