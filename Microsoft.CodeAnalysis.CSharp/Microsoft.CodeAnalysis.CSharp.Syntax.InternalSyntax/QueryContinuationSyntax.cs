using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class QueryContinuationSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken intoKeyword;

        internal readonly SyntaxToken identifier;

        internal readonly QueryBodySyntax body;

        public SyntaxToken IntoKeyword => intoKeyword;

        public SyntaxToken Identifier => identifier;

        public QueryBodySyntax Body => body;

        public QueryContinuationSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public QueryContinuationSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public QueryContinuationSyntax(SyntaxKind kind, SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(intoKeyword);
            this.intoKeyword = intoKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(body);
            this.body = body;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => intoKeyword,
                1 => identifier,
                2 => body,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQueryContinuation(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitQueryContinuation(this);
        }

        public QueryContinuationSyntax Update(SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body)
        {
            if (intoKeyword != IntoKeyword || identifier != Identifier || body != Body)
            {
                QueryContinuationSyntax queryContinuationSyntax = SyntaxFactory.QueryContinuation(intoKeyword, identifier, body);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    queryContinuationSyntax = queryContinuationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    queryContinuationSyntax = queryContinuationSyntax.WithAnnotationsGreen(annotations);
                }
                return queryContinuationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new QueryContinuationSyntax(base.Kind, intoKeyword, identifier, body, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new QueryContinuationSyntax(base.Kind, intoKeyword, identifier, body, GetDiagnostics(), annotations);
        }

        public QueryContinuationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            intoKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            QueryBodySyntax node3 = (QueryBodySyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            body = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(intoKeyword);
            writer.WriteValue(identifier);
            writer.WriteValue(body);
        }

        static QueryContinuationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(QueryContinuationSyntax), (ObjectReader r) => new QueryContinuationSyntax(r));
        }
    }
}
