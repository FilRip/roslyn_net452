using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class QueryExpressionSyntax : ExpressionSyntax
    {
        internal readonly FromClauseSyntax fromClause;

        internal readonly QueryBodySyntax body;

        public FromClauseSyntax FromClause => fromClause;

        public QueryBodySyntax Body => body;

        public QueryExpressionSyntax(SyntaxKind kind, FromClauseSyntax fromClause, QueryBodySyntax body, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(fromClause);
            this.fromClause = fromClause;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public QueryExpressionSyntax(SyntaxKind kind, FromClauseSyntax fromClause, QueryBodySyntax body, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(fromClause);
            this.fromClause = fromClause;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public QueryExpressionSyntax(SyntaxKind kind, FromClauseSyntax fromClause, QueryBodySyntax body)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(fromClause);
            this.fromClause = fromClause;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => fromClause,
                1 => body,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.QueryExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQueryExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitQueryExpression(this);
        }

        public QueryExpressionSyntax Update(FromClauseSyntax fromClause, QueryBodySyntax body)
        {
            if (fromClause != FromClause || body != Body)
            {
                QueryExpressionSyntax queryExpressionSyntax = SyntaxFactory.QueryExpression(fromClause, body);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    queryExpressionSyntax = queryExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    queryExpressionSyntax = queryExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return queryExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new QueryExpressionSyntax(base.Kind, fromClause, body, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new QueryExpressionSyntax(base.Kind, fromClause, body, GetDiagnostics(), annotations);
        }

        public QueryExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            FromClauseSyntax node = (FromClauseSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            fromClause = node;
            QueryBodySyntax node2 = (QueryBodySyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            body = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(fromClause);
            writer.WriteValue(body);
        }

        static QueryExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(QueryExpressionSyntax), (ObjectReader r) => new QueryExpressionSyntax(r));
        }
    }
}
