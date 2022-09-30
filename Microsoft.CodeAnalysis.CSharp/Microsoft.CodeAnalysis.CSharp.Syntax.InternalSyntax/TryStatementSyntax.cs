using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class TryStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken tryKeyword;

        internal readonly BlockSyntax block;

        internal readonly GreenNode? catches;

        internal readonly FinallyClauseSyntax? @finally;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken TryKeyword => tryKeyword;

        public BlockSyntax Block => block;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchClauseSyntax> Catches => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchClauseSyntax>(catches);

        public FinallyClauseSyntax? Finally => @finally;

        public TryStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken tryKeyword, BlockSyntax block, GreenNode? catches, FinallyClauseSyntax? @finally, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(tryKeyword);
            this.tryKeyword = tryKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
            if (catches != null)
            {
                AdjustFlagsAndWidth(catches);
                this.catches = catches;
            }
            if (@finally != null)
            {
                AdjustFlagsAndWidth(@finally);
                this.@finally = @finally;
            }
        }

        public TryStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken tryKeyword, BlockSyntax block, GreenNode? catches, FinallyClauseSyntax? @finally, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(tryKeyword);
            this.tryKeyword = tryKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
            if (catches != null)
            {
                AdjustFlagsAndWidth(catches);
                this.catches = catches;
            }
            if (@finally != null)
            {
                AdjustFlagsAndWidth(@finally);
                this.@finally = @finally;
            }
        }

        public TryStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken tryKeyword, BlockSyntax block, GreenNode? catches, FinallyClauseSyntax? @finally)
            : base(kind)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(tryKeyword);
            this.tryKeyword = tryKeyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
            if (catches != null)
            {
                AdjustFlagsAndWidth(catches);
                this.catches = catches;
            }
            if (@finally != null)
            {
                AdjustFlagsAndWidth(@finally);
                this.@finally = @finally;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => tryKeyword,
                2 => block,
                3 => catches,
                4 => @finally,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTryStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTryStatement(this);
        }

        public TryStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken tryKeyword, BlockSyntax block, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax @finally)
        {
            if (attributeLists != AttributeLists || tryKeyword != TryKeyword || block != Block || catches != Catches || @finally != Finally)
            {
                TryStatementSyntax tryStatementSyntax = SyntaxFactory.TryStatement(attributeLists, tryKeyword, block, catches, @finally);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    tryStatementSyntax = tryStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    tryStatementSyntax = tryStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return tryStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TryStatementSyntax(base.Kind, attributeLists, tryKeyword, block, catches, @finally, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TryStatementSyntax(base.Kind, attributeLists, tryKeyword, block, catches, @finally, GetDiagnostics(), annotations);
        }

        public TryStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            tryKeyword = node;
            BlockSyntax node2 = (BlockSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            block = node2;
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                catches = greenNode2;
            }
            FinallyClauseSyntax finallyClauseSyntax = (FinallyClauseSyntax)reader.ReadValue();
            if (finallyClauseSyntax != null)
            {
                AdjustFlagsAndWidth(finallyClauseSyntax);
                @finally = finallyClauseSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(tryKeyword);
            writer.WriteValue(block);
            writer.WriteValue(catches);
            writer.WriteValue(@finally);
        }

        static TryStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TryStatementSyntax), (ObjectReader r) => new TryStatementSyntax(r));
        }
    }
}
