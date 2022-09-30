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
    public sealed class CheckedStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken keyword;

        internal readonly BlockSyntax block;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken Keyword => keyword;

        public BlockSyntax Block => block;

        public CheckedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken keyword, BlockSyntax block, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public CheckedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken keyword, BlockSyntax block, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public CheckedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken keyword, BlockSyntax block)
            : base(kind)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(block);
            this.block = block;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => keyword,
                2 => block,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCheckedStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCheckedStatement(this);
        }

        public CheckedStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken keyword, BlockSyntax block)
        {
            if (attributeLists != AttributeLists || keyword != Keyword || block != Block)
            {
                CheckedStatementSyntax checkedStatementSyntax = SyntaxFactory.CheckedStatement(base.Kind, attributeLists, keyword, block);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    checkedStatementSyntax = checkedStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    checkedStatementSyntax = checkedStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return checkedStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CheckedStatementSyntax(base.Kind, attributeLists, keyword, block, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CheckedStatementSyntax(base.Kind, attributeLists, keyword, block, GetDiagnostics(), annotations);
        }

        public CheckedStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            keyword = node;
            BlockSyntax node2 = (BlockSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            block = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(keyword);
            writer.WriteValue(block);
        }

        static CheckedStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CheckedStatementSyntax), (ObjectReader r) => new CheckedStatementSyntax(r));
        }
    }
}
