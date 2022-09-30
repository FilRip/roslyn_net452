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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class YieldStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken yieldKeyword;

        internal readonly SyntaxToken returnOrBreakKeyword;

        internal readonly ExpressionSyntax? expression;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken YieldKeyword => yieldKeyword;

        public SyntaxToken ReturnOrBreakKeyword => returnOrBreakKeyword;

        public ExpressionSyntax? Expression => expression;

        public SyntaxToken SemicolonToken => semicolonToken;

        public YieldStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(yieldKeyword);
            this.yieldKeyword = yieldKeyword;
            AdjustFlagsAndWidth(returnOrBreakKeyword);
            this.returnOrBreakKeyword = returnOrBreakKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public YieldStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(yieldKeyword);
            this.yieldKeyword = yieldKeyword;
            AdjustFlagsAndWidth(returnOrBreakKeyword);
            this.returnOrBreakKeyword = returnOrBreakKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public YieldStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 5;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(yieldKeyword);
            this.yieldKeyword = yieldKeyword;
            AdjustFlagsAndWidth(returnOrBreakKeyword);
            this.returnOrBreakKeyword = returnOrBreakKeyword;
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
                1 => yieldKeyword,
                2 => returnOrBreakKeyword,
                3 => expression,
                4 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitYieldStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitYieldStatement(this);
        }

        public YieldStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || yieldKeyword != YieldKeyword || returnOrBreakKeyword != ReturnOrBreakKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                YieldStatementSyntax yieldStatementSyntax = SyntaxFactory.YieldStatement(base.Kind, attributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    yieldStatementSyntax = yieldStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    yieldStatementSyntax = yieldStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return yieldStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new YieldStatementSyntax(base.Kind, attributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new YieldStatementSyntax(base.Kind, attributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken, GetDiagnostics(), annotations);
        }

        public YieldStatementSyntax(ObjectReader reader)
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
            yieldKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            returnOrBreakKeyword = node2;
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                expression = expressionSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            semicolonToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(yieldKeyword);
            writer.WriteValue(returnOrBreakKeyword);
            writer.WriteValue(expression);
            writer.WriteValue(semicolonToken);
        }

        static YieldStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(YieldStatementSyntax), (ObjectReader r) => new YieldStatementSyntax(r));
        }
    }
}
