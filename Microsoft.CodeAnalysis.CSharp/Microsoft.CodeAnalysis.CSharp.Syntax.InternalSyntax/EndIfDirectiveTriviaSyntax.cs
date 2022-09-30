using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class EndIfDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken endIfKeyword;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken EndIfKeyword => endIfKeyword;

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public EndIfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(endIfKeyword);
            this.endIfKeyword = endIfKeyword;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public EndIfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(endIfKeyword);
            this.endIfKeyword = endIfKeyword;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public EndIfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(endIfKeyword);
            this.endIfKeyword = endIfKeyword;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => hashToken,
                1 => endIfKeyword,
                2 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.EndIfDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEndIfDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitEndIfDirectiveTrivia(this);
        }

        public EndIfDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || endIfKeyword != EndIfKeyword || endOfDirectiveToken != EndOfDirectiveToken)
            {
                EndIfDirectiveTriviaSyntax endIfDirectiveTriviaSyntax = SyntaxFactory.EndIfDirectiveTrivia(hashToken, endIfKeyword, endOfDirectiveToken, isActive);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    endIfDirectiveTriviaSyntax = endIfDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    endIfDirectiveTriviaSyntax = endIfDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return endIfDirectiveTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new EndIfDirectiveTriviaSyntax(base.Kind, hashToken, endIfKeyword, endOfDirectiveToken, isActive, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new EndIfDirectiveTriviaSyntax(base.Kind, hashToken, endIfKeyword, endOfDirectiveToken, isActive, GetDiagnostics(), annotations);
        }

        public EndIfDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            endIfKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            endOfDirectiveToken = node3;
            isActive = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(endIfKeyword);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
        }

        static EndIfDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(EndIfDirectiveTriviaSyntax), (ObjectReader r) => new EndIfDirectiveTriviaSyntax(r));
        }
    }
}
