using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class AttributeArgumentListSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly GreenNode? arguments;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeArgumentSyntax> Arguments => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeArgumentSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(arguments));

        public SyntaxToken CloseParenToken => closeParenToken;

        public AttributeArgumentListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? arguments, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public AttributeArgumentListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? arguments, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public AttributeArgumentListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? arguments, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => arguments,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAttributeArgumentList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAttributeArgumentList(this);
        }

        public AttributeArgumentListSyntax Update(SyntaxToken openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken == OpenParenToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeArgumentSyntax> right = Arguments;
                if (!(arguments != right) && closeParenToken == CloseParenToken)
                {
                    return this;
                }
            }
            AttributeArgumentListSyntax attributeArgumentListSyntax = SyntaxFactory.AttributeArgumentList(openParenToken, arguments, closeParenToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                attributeArgumentListSyntax = attributeArgumentListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                attributeArgumentListSyntax = attributeArgumentListSyntax.WithAnnotationsGreen(annotations);
            }
            return attributeArgumentListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AttributeArgumentListSyntax(base.Kind, openParenToken, arguments, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AttributeArgumentListSyntax(base.Kind, openParenToken, arguments, closeParenToken, GetDiagnostics(), annotations);
        }

        public AttributeArgumentListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openParenToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                arguments = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeParenToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(arguments);
            writer.WriteValue(closeParenToken);
        }

        static AttributeArgumentListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AttributeArgumentListSyntax), (ObjectReader r) => new AttributeArgumentListSyntax(r));
        }
    }
}
