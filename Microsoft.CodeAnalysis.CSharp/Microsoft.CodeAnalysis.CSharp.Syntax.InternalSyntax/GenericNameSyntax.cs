using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class GenericNameSyntax : SimpleNameSyntax
    {
        internal readonly SyntaxToken identifier;

        internal readonly TypeArgumentListSyntax typeArgumentList;

        public override SyntaxToken Identifier => identifier;

        public TypeArgumentListSyntax TypeArgumentList => typeArgumentList;

        public GenericNameSyntax(SyntaxKind kind, SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(typeArgumentList);
            this.typeArgumentList = typeArgumentList;
        }

        public GenericNameSyntax(SyntaxKind kind, SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(typeArgumentList);
            this.typeArgumentList = typeArgumentList;
        }

        public GenericNameSyntax(SyntaxKind kind, SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(typeArgumentList);
            this.typeArgumentList = typeArgumentList;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => identifier,
                1 => typeArgumentList,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGenericName(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitGenericName(this);
        }

        public GenericNameSyntax Update(SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList)
        {
            if (identifier != Identifier || typeArgumentList != TypeArgumentList)
            {
                GenericNameSyntax genericNameSyntax = SyntaxFactory.GenericName(identifier, typeArgumentList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    genericNameSyntax = genericNameSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    genericNameSyntax = genericNameSyntax.WithAnnotationsGreen(annotations);
                }
                return genericNameSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new GenericNameSyntax(base.Kind, identifier, typeArgumentList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new GenericNameSyntax(base.Kind, identifier, typeArgumentList, GetDiagnostics(), annotations);
        }

        public GenericNameSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
            TypeArgumentListSyntax node2 = (TypeArgumentListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            typeArgumentList = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(identifier);
            writer.WriteValue(typeArgumentList);
        }

        static GenericNameSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(GenericNameSyntax), (ObjectReader r) => new GenericNameSyntax(r));
        }
    }
}
