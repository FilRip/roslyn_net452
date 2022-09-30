using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SimpleBaseTypeSyntax : BaseTypeSyntax
    {
        internal readonly TypeSyntax type;

        public override TypeSyntax Type => type;

        public SimpleBaseTypeSyntax(SyntaxKind kind, TypeSyntax type, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public SimpleBaseTypeSyntax(SyntaxKind kind, TypeSyntax type, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public SimpleBaseTypeSyntax(SyntaxKind kind, TypeSyntax type)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return type;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SimpleBaseTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSimpleBaseType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSimpleBaseType(this);
        }

        public SimpleBaseTypeSyntax Update(TypeSyntax type)
        {
            if (type != Type)
            {
                SimpleBaseTypeSyntax simpleBaseTypeSyntax = SyntaxFactory.SimpleBaseType(type);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    simpleBaseTypeSyntax = simpleBaseTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    simpleBaseTypeSyntax = simpleBaseTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return simpleBaseTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SimpleBaseTypeSyntax(base.Kind, type, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SimpleBaseTypeSyntax(base.Kind, type, GetDiagnostics(), annotations);
        }

        public SimpleBaseTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            type = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(type);
        }

        static SimpleBaseTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SimpleBaseTypeSyntax), (ObjectReader r) => new SimpleBaseTypeSyntax(r));
        }
    }
}
