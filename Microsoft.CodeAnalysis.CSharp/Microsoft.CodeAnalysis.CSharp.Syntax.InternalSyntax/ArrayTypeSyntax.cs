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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ArrayTypeSyntax : TypeSyntax
    {
        internal readonly TypeSyntax elementType;

        internal readonly GreenNode? rankSpecifiers;

        public TypeSyntax ElementType => elementType;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>(rankSpecifiers);

        public ArrayTypeSyntax(SyntaxKind kind, TypeSyntax elementType, GreenNode? rankSpecifiers, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            if (rankSpecifiers != null)
            {
                AdjustFlagsAndWidth(rankSpecifiers);
                this.rankSpecifiers = rankSpecifiers;
            }
        }

        public ArrayTypeSyntax(SyntaxKind kind, TypeSyntax elementType, GreenNode? rankSpecifiers, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            if (rankSpecifiers != null)
            {
                AdjustFlagsAndWidth(rankSpecifiers);
                this.rankSpecifiers = rankSpecifiers;
            }
        }

        public ArrayTypeSyntax(SyntaxKind kind, TypeSyntax elementType, GreenNode? rankSpecifiers)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            if (rankSpecifiers != null)
            {
                AdjustFlagsAndWidth(rankSpecifiers);
                this.rankSpecifiers = rankSpecifiers;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => elementType,
                1 => rankSpecifiers,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        public ArrayTypeSyntax Update(TypeSyntax elementType, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            if (elementType != ElementType || rankSpecifiers != RankSpecifiers)
            {
                ArrayTypeSyntax arrayTypeSyntax = SyntaxFactory.ArrayType(elementType, rankSpecifiers);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    arrayTypeSyntax = arrayTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    arrayTypeSyntax = arrayTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return arrayTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ArrayTypeSyntax(base.Kind, elementType, rankSpecifiers, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ArrayTypeSyntax(base.Kind, elementType, rankSpecifiers, GetDiagnostics(), annotations);
        }

        public ArrayTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            elementType = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                rankSpecifiers = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(elementType);
            writer.WriteValue(rankSpecifiers);
        }

        static ArrayTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ArrayTypeSyntax), (ObjectReader r) => new ArrayTypeSyntax(r));
        }
    }
}
