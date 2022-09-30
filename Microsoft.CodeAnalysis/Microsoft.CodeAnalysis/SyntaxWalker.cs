namespace Microsoft.CodeAnalysis
{
    public abstract class SyntaxWalker
    {
        protected SyntaxWalkerDepth Depth { get; }

        protected SyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
        {
            Depth = depth;
        }

        public virtual void Visit(SyntaxNode node)
        {
            ChildSyntaxList.Enumerator enumerator = node.ChildNodesAndTokens().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxNodeOrToken current = enumerator.Current;
                if (current.IsNode)
                {
                    if (Depth >= SyntaxWalkerDepth.Node)
                    {
                        Visit(current.AsNode());
                    }
                }
                else if (current.IsToken && Depth >= SyntaxWalkerDepth.Token)
                {
                    VisitToken(current.AsToken());
                }
            }
        }

        protected virtual void VisitToken(SyntaxToken token)
        {
            if (Depth >= SyntaxWalkerDepth.Trivia)
            {
                VisitLeadingTrivia(in token);
                VisitTrailingTrivia(in token);
            }
        }

        private void VisitLeadingTrivia(in SyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                SyntaxTriviaList.Enumerator enumerator = token.LeadingTrivia.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    VisitTrivia(current);
                }
            }
        }

        private void VisitTrailingTrivia(in SyntaxToken token)
        {
            if (token.HasTrailingTrivia)
            {
                SyntaxTriviaList.Enumerator enumerator = token.TrailingTrivia.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    VisitTrivia(current);
                }
            }
        }

        protected virtual void VisitTrivia(SyntaxTrivia trivia)
        {
            if (Depth >= SyntaxWalkerDepth.StructuredTrivia && trivia.HasStructure)
            {
                Visit(trivia.GetStructure());
            }
        }
    }
}
