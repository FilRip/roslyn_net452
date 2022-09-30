using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SkippedTokensTriviaSyntax : StructuredTriviaSyntax
    {
        internal readonly GreenNode? tokens;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Tokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(tokens);

        public SkippedTokensTriviaSyntax(SyntaxKind kind, GreenNode? tokens, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            if (tokens != null)
            {
                AdjustFlagsAndWidth(tokens);
                this.tokens = tokens;
            }
        }

        public SkippedTokensTriviaSyntax(SyntaxKind kind, GreenNode? tokens, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            if (tokens != null)
            {
                AdjustFlagsAndWidth(tokens);
                this.tokens = tokens;
            }
        }

        public SkippedTokensTriviaSyntax(SyntaxKind kind, GreenNode? tokens)
            : base(kind)
        {
            base.SlotCount = 1;
            if (tokens != null)
            {
                AdjustFlagsAndWidth(tokens);
                this.tokens = tokens;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return tokens;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SkippedTokensTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSkippedTokensTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSkippedTokensTrivia(this);
        }

        public SkippedTokensTriviaSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> tokens)
        {
            if (tokens != Tokens)
            {
                SkippedTokensTriviaSyntax skippedTokensTriviaSyntax = SyntaxFactory.SkippedTokensTrivia(tokens);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    skippedTokensTriviaSyntax = skippedTokensTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    skippedTokensTriviaSyntax = skippedTokensTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return skippedTokensTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SkippedTokensTriviaSyntax(base.Kind, tokens, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SkippedTokensTriviaSyntax(base.Kind, tokens, GetDiagnostics(), annotations);
        }

        public SkippedTokensTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                tokens = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(tokens);
        }

        static SkippedTokensTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SkippedTokensTriviaSyntax), (ObjectReader r) => new SkippedTokensTriviaSyntax(r));
        }
    }
}
