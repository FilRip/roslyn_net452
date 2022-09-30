using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public ExpressionSyntax Expression => expression;

        public SyntaxToken SemicolonToken => semicolonToken;

        public ExpressionStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ExpressionStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ExpressionStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => expression,
                2 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitExpressionStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }

        public ExpressionStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || expression != Expression || semicolonToken != SemicolonToken)
            {
                ExpressionStatementSyntax expressionStatementSyntax = SyntaxFactory.ExpressionStatement(attributeLists, expression, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    expressionStatementSyntax = expressionStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    expressionStatementSyntax = expressionStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return expressionStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ExpressionStatementSyntax(base.Kind, attributeLists, expression, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ExpressionStatementSyntax(base.Kind, attributeLists, expression, semicolonToken, GetDiagnostics(), annotations);
        }

        public ExpressionStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            semicolonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(expression);
            writer.WriteValue(semicolonToken);
        }

        static ExpressionStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ExpressionStatementSyntax), (ObjectReader r) => new ExpressionStatementSyntax(r));
        }
    }
}
