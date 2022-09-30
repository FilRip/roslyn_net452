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
    public sealed class PositionalPatternClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly GreenNode? subpatterns;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> Subpatterns => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(subpatterns));

        public SyntaxToken CloseParenToken => closeParenToken;

        public PositionalPatternClauseSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? subpatterns, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public PositionalPatternClauseSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? subpatterns, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public PositionalPatternClauseSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? subpatterns, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (subpatterns != null)
            {
                AdjustFlagsAndWidth(subpatterns);
                this.subpatterns = subpatterns;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => subpatterns,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPositionalPatternClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPositionalPatternClause(this);
        }

        public PositionalPatternClauseSyntax Update(SyntaxToken openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeParenToken)
        {
            if (openParenToken == OpenParenToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> right = Subpatterns;
                if (!(subpatterns != right) && closeParenToken == CloseParenToken)
                {
                    return this;
                }
            }
            PositionalPatternClauseSyntax positionalPatternClauseSyntax = SyntaxFactory.PositionalPatternClause(openParenToken, subpatterns, closeParenToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                positionalPatternClauseSyntax = positionalPatternClauseSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                positionalPatternClauseSyntax = positionalPatternClauseSyntax.WithAnnotationsGreen(annotations);
            }
            return positionalPatternClauseSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PositionalPatternClauseSyntax(base.Kind, openParenToken, subpatterns, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PositionalPatternClauseSyntax(base.Kind, openParenToken, subpatterns, closeParenToken, GetDiagnostics(), annotations);
        }

        public PositionalPatternClauseSyntax(ObjectReader reader)
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
                subpatterns = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeParenToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(subpatterns);
            writer.WriteValue(closeParenToken);
        }

        static PositionalPatternClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PositionalPatternClauseSyntax), (ObjectReader r) => new PositionalPatternClauseSyntax(r));
        }
    }
}
