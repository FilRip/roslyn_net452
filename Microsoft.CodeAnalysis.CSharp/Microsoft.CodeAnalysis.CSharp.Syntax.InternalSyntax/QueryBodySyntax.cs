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
    public sealed class QueryBodySyntax : CSharpSyntaxNode
    {
        internal readonly GreenNode? clauses;

        internal readonly SelectOrGroupClauseSyntax selectOrGroup;

        internal readonly QueryContinuationSyntax? continuation;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> Clauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax>(clauses);

        public SelectOrGroupClauseSyntax SelectOrGroup => selectOrGroup;

        public QueryContinuationSyntax? Continuation => continuation;

        public QueryBodySyntax(SyntaxKind kind, GreenNode? clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax? continuation, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (clauses != null)
            {
                AdjustFlagsAndWidth(clauses);
                this.clauses = clauses;
            }
            AdjustFlagsAndWidth(selectOrGroup);
            this.selectOrGroup = selectOrGroup;
            if (continuation != null)
            {
                AdjustFlagsAndWidth(continuation);
                this.continuation = continuation;
            }
        }

        public QueryBodySyntax(SyntaxKind kind, GreenNode? clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax? continuation, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (clauses != null)
            {
                AdjustFlagsAndWidth(clauses);
                this.clauses = clauses;
            }
            AdjustFlagsAndWidth(selectOrGroup);
            this.selectOrGroup = selectOrGroup;
            if (continuation != null)
            {
                AdjustFlagsAndWidth(continuation);
                this.continuation = continuation;
            }
        }

        public QueryBodySyntax(SyntaxKind kind, GreenNode? clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax? continuation)
            : base(kind)
        {
            base.SlotCount = 3;
            if (clauses != null)
            {
                AdjustFlagsAndWidth(clauses);
                this.clauses = clauses;
            }
            AdjustFlagsAndWidth(selectOrGroup);
            this.selectOrGroup = selectOrGroup;
            if (continuation != null)
            {
                AdjustFlagsAndWidth(continuation);
                this.continuation = continuation;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => clauses,
                1 => selectOrGroup,
                2 => continuation,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQueryBody(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitQueryBody(this);
        }

        public QueryBodySyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax continuation)
        {
            if (clauses != Clauses || selectOrGroup != SelectOrGroup || continuation != Continuation)
            {
                QueryBodySyntax queryBodySyntax = SyntaxFactory.QueryBody(clauses, selectOrGroup, continuation);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    queryBodySyntax = queryBodySyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    queryBodySyntax = queryBodySyntax.WithAnnotationsGreen(annotations);
                }
                return queryBodySyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new QueryBodySyntax(base.Kind, clauses, selectOrGroup, continuation, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new QueryBodySyntax(base.Kind, clauses, selectOrGroup, continuation, GetDiagnostics(), annotations);
        }

        public QueryBodySyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                clauses = greenNode;
            }
            SelectOrGroupClauseSyntax node = (SelectOrGroupClauseSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            selectOrGroup = node;
            QueryContinuationSyntax queryContinuationSyntax = (QueryContinuationSyntax)reader.ReadValue();
            if (queryContinuationSyntax != null)
            {
                AdjustFlagsAndWidth(queryContinuationSyntax);
                continuation = queryContinuationSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(clauses);
            writer.WriteValue(selectOrGroup);
            writer.WriteValue(continuation);
        }

        static QueryBodySyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(QueryBodySyntax), (ObjectReader r) => new QueryBodySyntax(r));
        }
    }
}
