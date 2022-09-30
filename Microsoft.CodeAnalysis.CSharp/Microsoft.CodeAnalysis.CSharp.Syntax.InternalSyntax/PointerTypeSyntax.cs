using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PointerTypeSyntax : TypeSyntax
    {
        internal readonly TypeSyntax elementType;

        internal readonly SyntaxToken asteriskToken;

        public TypeSyntax ElementType => elementType;

        public SyntaxToken AsteriskToken => asteriskToken;

        public PointerTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken asteriskToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
        }

        public PointerTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken asteriskToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
        }

        public PointerTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken asteriskToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => elementType,
                1 => asteriskToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PointerTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPointerType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPointerType(this);
        }

        public PointerTypeSyntax Update(TypeSyntax elementType, SyntaxToken asteriskToken)
        {
            if (elementType != ElementType || asteriskToken != AsteriskToken)
            {
                PointerTypeSyntax pointerTypeSyntax = SyntaxFactory.PointerType(elementType, asteriskToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    pointerTypeSyntax = pointerTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    pointerTypeSyntax = pointerTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return pointerTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PointerTypeSyntax(base.Kind, elementType, asteriskToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PointerTypeSyntax(base.Kind, elementType, asteriskToken, GetDiagnostics(), annotations);
        }

        public PointerTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            elementType = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            asteriskToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(elementType);
            writer.WriteValue(asteriskToken);
        }

        static PointerTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PointerTypeSyntax), (ObjectReader r) => new PointerTypeSyntax(r));
        }
    }
}
