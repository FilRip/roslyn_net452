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
    public sealed class ArrayRankSpecifierSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openBracketToken;

        internal readonly GreenNode? sizes;

        internal readonly SyntaxToken closeBracketToken;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Sizes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(sizes));

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public ArrayRankSpecifierSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? sizes, SyntaxToken closeBracketToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (sizes != null)
            {
                AdjustFlagsAndWidth(sizes);
                this.sizes = sizes;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public ArrayRankSpecifierSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? sizes, SyntaxToken closeBracketToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (sizes != null)
            {
                AdjustFlagsAndWidth(sizes);
                this.sizes = sizes;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public ArrayRankSpecifierSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? sizes, SyntaxToken closeBracketToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (sizes != null)
            {
                AdjustFlagsAndWidth(sizes);
                this.sizes = sizes;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBracketToken,
                1 => sizes,
                2 => closeBracketToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArrayRankSpecifier(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitArrayRankSpecifier(this);
        }

        public ArrayRankSpecifierSyntax Update(SyntaxToken openBracketToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> sizes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken == OpenBracketToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> right = Sizes;
                if (!(sizes != right) && closeBracketToken == CloseBracketToken)
                {
                    return this;
                }
            }
            ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = SyntaxFactory.ArrayRankSpecifier(openBracketToken, sizes, closeBracketToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                arrayRankSpecifierSyntax = arrayRankSpecifierSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                arrayRankSpecifierSyntax = arrayRankSpecifierSyntax.WithAnnotationsGreen(annotations);
            }
            return arrayRankSpecifierSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ArrayRankSpecifierSyntax(base.Kind, openBracketToken, sizes, closeBracketToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ArrayRankSpecifierSyntax(base.Kind, openBracketToken, sizes, closeBracketToken, GetDiagnostics(), annotations);
        }

        public ArrayRankSpecifierSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBracketToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                sizes = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBracketToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(sizes);
            writer.WriteValue(closeBracketToken);
        }

        static ArrayRankSpecifierSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ArrayRankSpecifierSyntax), (ObjectReader r) => new ArrayRankSpecifierSyntax(r));
        }
    }
}
