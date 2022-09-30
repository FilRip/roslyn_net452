using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ContinueStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken continueKeyword;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken ContinueKeyword => continueKeyword;

        public SyntaxToken SemicolonToken => semicolonToken;

        public ContinueStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(continueKeyword);
            this.continueKeyword = continueKeyword;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ContinueStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(continueKeyword);
            this.continueKeyword = continueKeyword;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ContinueStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(continueKeyword);
            this.continueKeyword = continueKeyword;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => continueKeyword,
                2 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitContinueStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitContinueStatement(this);
        }

        public ContinueStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || continueKeyword != ContinueKeyword || semicolonToken != SemicolonToken)
            {
                ContinueStatementSyntax continueStatementSyntax = SyntaxFactory.ContinueStatement(attributeLists, continueKeyword, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    continueStatementSyntax = continueStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    continueStatementSyntax = continueStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return continueStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ContinueStatementSyntax(base.Kind, attributeLists, continueKeyword, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ContinueStatementSyntax(base.Kind, attributeLists, continueKeyword, semicolonToken, GetDiagnostics(), annotations);
        }

        public ContinueStatementSyntax(ObjectReader reader)
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
            continueKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            semicolonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(continueKeyword);
            writer.WriteValue(semicolonToken);
        }

        static ContinueStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ContinueStatementSyntax), (ObjectReader r) => new ContinueStatementSyntax(r));
        }
    }
}
