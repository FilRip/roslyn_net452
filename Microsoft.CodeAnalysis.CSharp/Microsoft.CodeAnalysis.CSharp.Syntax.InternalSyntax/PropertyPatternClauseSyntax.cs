using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PropertyPatternClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? subpatterns;

        internal readonly SyntaxToken closeBraceToken;

        public SyntaxToken OpenBraceToken => openBraceToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> Subpatterns => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(subpatterns));

        public SyntaxToken CloseBraceToken => closeBraceToken;

        public PropertyPatternClauseSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? subpatterns, SyntaxToken closeBraceToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public PropertyPatternClauseSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? subpatterns, SyntaxToken closeBraceToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public PropertyPatternClauseSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? subpatterns, SyntaxToken closeBraceToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBraceToken,
                1 => subpatterns,
                2 => closeBraceToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPropertyPatternClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPropertyPatternClause(this);
        }

        public PropertyPatternClauseSyntax Update(SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeBraceToken)
        {
            if (openBraceToken == OpenBraceToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> right = Subpatterns;
                if (!(subpatterns != right) && closeBraceToken == CloseBraceToken)
                {
                    return this;
                }
            }
            PropertyPatternClauseSyntax propertyPatternClauseSyntax = SyntaxFactory.PropertyPatternClause(openBraceToken, subpatterns, closeBraceToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                propertyPatternClauseSyntax = propertyPatternClauseSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                propertyPatternClauseSyntax = propertyPatternClauseSyntax.WithAnnotationsGreen(annotations);
            }
            return propertyPatternClauseSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PropertyPatternClauseSyntax(base.Kind, openBraceToken, subpatterns, closeBraceToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PropertyPatternClauseSyntax(base.Kind, openBraceToken, subpatterns, closeBraceToken, GetDiagnostics(), annotations);
        }

        public PropertyPatternClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBraceToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                subpatterns = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBraceToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(subpatterns);
            writer.WriteValue(closeBraceToken);
        }

        static PropertyPatternClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PropertyPatternClauseSyntax), (ObjectReader r) => new PropertyPatternClauseSyntax(r));
        }
    }
}
