using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FinallyClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken finallyKeyword;

        internal readonly BlockSyntax block;

        public SyntaxToken FinallyKeyword => finallyKeyword;

        public BlockSyntax Block => block;

        public FinallyClauseSyntax(SyntaxKind kind, SyntaxToken finallyKeyword, BlockSyntax block, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(finallyKeyword);
            this.finallyKeyword = finallyKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public FinallyClauseSyntax(SyntaxKind kind, SyntaxToken finallyKeyword, BlockSyntax block, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(finallyKeyword);
            this.finallyKeyword = finallyKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public FinallyClauseSyntax(SyntaxKind kind, SyntaxToken finallyKeyword, BlockSyntax block)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(finallyKeyword);
            this.finallyKeyword = finallyKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => finallyKeyword,
                1 => block,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFinallyClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFinallyClause(this);
        }

        public FinallyClauseSyntax Update(SyntaxToken finallyKeyword, BlockSyntax block)
        {
            if (finallyKeyword != FinallyKeyword || block != Block)
            {
                FinallyClauseSyntax finallyClauseSyntax = SyntaxFactory.FinallyClause(finallyKeyword, block);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    finallyClauseSyntax = finallyClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    finallyClauseSyntax = finallyClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return finallyClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FinallyClauseSyntax(base.Kind, finallyKeyword, block, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FinallyClauseSyntax(base.Kind, finallyKeyword, block, GetDiagnostics(), annotations);
        }

        public FinallyClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            finallyKeyword = node;
            BlockSyntax node2 = (BlockSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            block = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(finallyKeyword);
            writer.WriteValue(block);
        }

        static FinallyClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FinallyClauseSyntax), (ObjectReader r) => new FinallyClauseSyntax(r));
        }
    }
}
