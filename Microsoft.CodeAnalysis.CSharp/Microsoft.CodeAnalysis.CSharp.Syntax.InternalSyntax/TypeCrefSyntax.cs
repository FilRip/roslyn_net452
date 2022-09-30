using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class TypeCrefSyntax : CrefSyntax
    {
        internal readonly TypeSyntax type;

        public TypeSyntax Type => type;

        public TypeCrefSyntax(SyntaxKind kind, TypeSyntax type, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public TypeCrefSyntax(SyntaxKind kind, TypeSyntax type, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public TypeCrefSyntax(SyntaxKind kind, TypeSyntax type)
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
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TypeCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTypeCref(this);
        }

        public TypeCrefSyntax Update(TypeSyntax type)
        {
            if (type != Type)
            {
                TypeCrefSyntax typeCrefSyntax = SyntaxFactory.TypeCref(type);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    typeCrefSyntax = typeCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    typeCrefSyntax = typeCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return typeCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TypeCrefSyntax(base.Kind, type, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TypeCrefSyntax(base.Kind, type, GetDiagnostics(), annotations);
        }

        public TypeCrefSyntax(ObjectReader reader)
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

        static TypeCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TypeCrefSyntax), (ObjectReader r) => new TypeCrefSyntax(r));
        }
    }
}
