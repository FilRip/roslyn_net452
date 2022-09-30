using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class GotoStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken gotoKeyword;

        internal readonly SyntaxToken? caseOrDefaultKeyword;

        internal readonly ExpressionSyntax? expression;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken GotoKeyword => gotoKeyword;

        public SyntaxToken? CaseOrDefaultKeyword => caseOrDefaultKeyword;

        public ExpressionSyntax? Expression => expression;

        public SyntaxToken SemicolonToken => semicolonToken;

        public GotoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken gotoKeyword, SyntaxToken? caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(gotoKeyword);
            this.gotoKeyword = gotoKeyword;
            if (caseOrDefaultKeyword != null)
            {
                AdjustFlagsAndWidth(caseOrDefaultKeyword);
                this.caseOrDefaultKeyword = caseOrDefaultKeyword;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public GotoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken gotoKeyword, SyntaxToken? caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(gotoKeyword);
            this.gotoKeyword = gotoKeyword;
            if (caseOrDefaultKeyword != null)
            {
                AdjustFlagsAndWidth(caseOrDefaultKeyword);
                this.caseOrDefaultKeyword = caseOrDefaultKeyword;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public GotoStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken gotoKeyword, SyntaxToken? caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(gotoKeyword);
            this.gotoKeyword = gotoKeyword;
            if (caseOrDefaultKeyword != null)
            {
                AdjustFlagsAndWidth(caseOrDefaultKeyword);
                this.caseOrDefaultKeyword = caseOrDefaultKeyword;
            }
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => gotoKeyword,
                2 => caseOrDefaultKeyword,
                3 => expression,
                4 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGotoStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitGotoStatement(this);
        }

        public GotoStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || gotoKeyword != GotoKeyword || caseOrDefaultKeyword != CaseOrDefaultKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                GotoStatementSyntax gotoStatementSyntax = SyntaxFactory.GotoStatement(base.Kind, attributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    gotoStatementSyntax = gotoStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    gotoStatementSyntax = gotoStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return gotoStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new GotoStatementSyntax(base.Kind, attributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new GotoStatementSyntax(base.Kind, attributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken, GetDiagnostics(), annotations);
        }

        public GotoStatementSyntax(ObjectReader reader)
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
            gotoKeyword = node;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                caseOrDefaultKeyword = syntaxToken;
            }
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                expression = expressionSyntax;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            semicolonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(gotoKeyword);
            writer.WriteValue(caseOrDefaultKeyword);
            writer.WriteValue(expression);
            writer.WriteValue(semicolonToken);
        }

        static GotoStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(GotoStatementSyntax), (ObjectReader r) => new GotoStatementSyntax(r));
        }
    }
}
