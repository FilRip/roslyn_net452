#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class CSharpSyntaxWalker : CSharpSyntaxVisitor
    {
        private int _recursionDepth;

        protected SyntaxWalkerDepth Depth { get; }

        protected CSharpSyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
        {
            Depth = depth;
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node != null)
            {
                _recursionDepth++;
                StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
                ((CSharpSyntaxNode)node).Accept(this);
                _recursionDepth--;
            }
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            int count = node.ChildNodesAndTokens().Count;
            int num = 0;
            do
            {
                SyntaxNodeOrToken syntaxNodeOrToken = ChildSyntaxList.ItemInternal((CSharpSyntaxNode)node, num);
                num++;
                SyntaxNode syntaxNode = syntaxNodeOrToken.AsNode();
                if (syntaxNode != null)
                {
                    if (Depth >= SyntaxWalkerDepth.Node)
                    {
                        Visit(syntaxNode);
                    }
                }
                else if (Depth >= SyntaxWalkerDepth.Token)
                {
                    VisitToken(syntaxNodeOrToken.AsToken());
                }
            }
            while (num < count);
        }

        public virtual void VisitToken(SyntaxToken token)
        {
            if (Depth >= SyntaxWalkerDepth.Trivia)
            {
                VisitLeadingTrivia(token);
                VisitTrailingTrivia(token);
            }
        }

        public virtual void VisitLeadingTrivia(SyntaxToken token)
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

        public virtual void VisitTrailingTrivia(SyntaxToken token)
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

        public virtual void VisitTrivia(SyntaxTrivia trivia)
        {
            if (Depth >= SyntaxWalkerDepth.StructuredTrivia && trivia.HasStructure)
            {
                Visit((CSharpSyntaxNode)trivia.GetStructure());
            }
        }
    }
}
