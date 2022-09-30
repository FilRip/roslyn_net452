using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class SyntaxNavigator
    {
        [Flags()]
        private enum SyntaxKinds
        {
            DocComments = 1,
            Directives = 2,
            SkippedTokens = 4
        }

        private const int None = 0;

        public static readonly SyntaxNavigator Instance = new SyntaxNavigator();

        private static readonly Func<SyntaxTrivia, bool>?[] s_stepIntoFunctions = new Func<SyntaxTrivia, bool>[8]
        {
            null,
            (SyntaxTrivia t) => t.IsDocumentationCommentTrivia,
            (SyntaxTrivia t) => t.IsDirective,
            (SyntaxTrivia t) => t.IsDirective || t.IsDocumentationCommentTrivia,
            (SyntaxTrivia t) => t.IsSkippedTokensTrivia,
            (SyntaxTrivia t) => t.IsSkippedTokensTrivia || t.IsDocumentationCommentTrivia,
            (SyntaxTrivia t) => t.IsSkippedTokensTrivia || t.IsDirective,
            (SyntaxTrivia t) => t.IsSkippedTokensTrivia || t.IsDirective || t.IsDocumentationCommentTrivia
        };

        private static readonly ObjectPool<Stack<ChildSyntaxList.Enumerator>> s_childEnumeratorStackPool = new ObjectPool<Stack<ChildSyntaxList.Enumerator>>(() => new Stack<ChildSyntaxList.Enumerator>(), 10);

        private static readonly ObjectPool<Stack<ChildSyntaxList.Reversed.Enumerator>> s_childReversedEnumeratorStackPool = new ObjectPool<Stack<ChildSyntaxList.Reversed.Enumerator>>(() => new Stack<ChildSyntaxList.Reversed.Enumerator>(), 10);

        private SyntaxNavigator()
        {
        }

        private static Func<SyntaxTrivia, bool>? GetStepIntoFunction(bool skipped, bool directives, bool docComments)
        {
            SyntaxKinds syntaxKinds = (skipped ? SyntaxKinds.SkippedTokens : 0) | (directives ? SyntaxKinds.Directives : 0) | (docComments ? SyntaxKinds.DocComments : 0);
            return s_stepIntoFunctions[(int)syntaxKinds];
        }

        private static Func<SyntaxToken, bool> GetPredicateFunction(bool includeZeroWidth)
        {
            if (!includeZeroWidth)
            {
                return SyntaxToken.NonZeroWidth;
            }
            return SyntaxToken.Any;
        }

        private static bool Matches(Func<SyntaxToken, bool>? predicate, SyntaxToken token)
        {
            if (predicate != null && (object)predicate != SyntaxToken.Any)
            {
                return predicate!(token);
            }
            return true;
        }

        public SyntaxToken GetFirstToken(in SyntaxNode current, bool includeZeroWidth, bool includeSkipped, bool includeDirectives, bool includeDocumentationComments)
        {
            return GetFirstToken(current, GetPredicateFunction(includeZeroWidth), GetStepIntoFunction(includeSkipped, includeDirectives, includeDocumentationComments));
        }

        public SyntaxToken GetLastToken(in SyntaxNode current, bool includeZeroWidth, bool includeSkipped, bool includeDirectives, bool includeDocumentationComments)
        {
            return GetLastToken(current, GetPredicateFunction(includeZeroWidth), GetStepIntoFunction(includeSkipped, includeDirectives, includeDocumentationComments));
        }

        public SyntaxToken GetPreviousToken(in SyntaxToken current, bool includeZeroWidth, bool includeSkipped, bool includeDirectives, bool includeDocumentationComments)
        {
            return GetPreviousToken(in current, GetPredicateFunction(includeZeroWidth), GetStepIntoFunction(includeSkipped, includeDirectives, includeDocumentationComments));
        }

        public SyntaxToken GetNextToken(in SyntaxToken current, bool includeZeroWidth, bool includeSkipped, bool includeDirectives, bool includeDocumentationComments)
        {
            return GetNextToken(in current, GetPredicateFunction(includeZeroWidth), GetStepIntoFunction(includeSkipped, includeDirectives, includeDocumentationComments));
        }

        public SyntaxToken GetPreviousToken(in SyntaxToken current, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            return GetPreviousToken(in current, predicate, stepInto != null, stepInto);
        }

        public SyntaxToken GetNextToken(in SyntaxToken current, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            return GetNextToken(in current, predicate, stepInto != null, stepInto);
        }

        public SyntaxToken GetFirstToken(SyntaxNode current, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            Stack<ChildSyntaxList.Enumerator> stack = s_childEnumeratorStackPool.Allocate();
            try
            {
                stack.Push(current.ChildNodesAndTokens().GetEnumerator());
                while (stack.Count > 0)
                {
                    ChildSyntaxList.Enumerator item = stack.Pop();
                    if (!item.MoveNext())
                    {
                        continue;
                    }
                    SyntaxNodeOrToken current2 = item.Current;
                    if (current2.IsToken)
                    {
                        SyntaxToken firstToken = GetFirstToken(current2.AsToken(), predicate, stepInto);
                        if (firstToken.RawKind != 0)
                        {
                            return firstToken;
                        }
                    }
                    stack.Push(item);
                    if (current2.IsNode)
                    {
                        stack.Push(current2.AsNode()!.ChildNodesAndTokens().GetEnumerator());
                    }
                }
                return default(SyntaxToken);
            }
            finally
            {
                stack.Clear();
                s_childEnumeratorStackPool.Free(stack);
            }
        }

        public SyntaxToken GetLastToken(SyntaxNode current, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            Stack<ChildSyntaxList.Reversed.Enumerator> stack = s_childReversedEnumeratorStackPool.Allocate();
            try
            {
                stack.Push(current.ChildNodesAndTokens().Reverse().GetEnumerator());
                while (stack.Count > 0)
                {
                    ChildSyntaxList.Reversed.Enumerator item = stack.Pop();
                    if (!item.MoveNext())
                    {
                        continue;
                    }
                    SyntaxNodeOrToken current2 = item.Current;
                    if (current2.IsToken)
                    {
                        SyntaxToken lastToken = GetLastToken(current2.AsToken(), predicate, stepInto);
                        if (lastToken.RawKind != 0)
                        {
                            return lastToken;
                        }
                    }
                    stack.Push(item);
                    if (current2.IsNode)
                    {
                        stack.Push(current2.AsNode()!.ChildNodesAndTokens().Reverse().GetEnumerator());
                    }
                }
                return default(SyntaxToken);
            }
            finally
            {
                stack.Clear();
                s_childReversedEnumeratorStackPool.Free(stack);
            }
        }

        public SyntaxToken GetFirstToken(SyntaxTriviaList triviaList, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool> stepInto)
        {
            SyntaxTriviaList.Enumerator enumerator = triviaList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxTrivia current = enumerator.Current;
                if (current.TryGetStructure(out var structure) && stepInto(current))
                {
                    SyntaxToken firstToken = GetFirstToken(structure, predicate, stepInto);
                    if (firstToken.RawKind != 0)
                    {
                        return firstToken;
                    }
                }
            }
            return default(SyntaxToken);
        }

        private SyntaxToken GetLastToken(SyntaxTriviaList list, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool> stepInto)
        {
            SyntaxTriviaList.Reversed.Enumerator enumerator = list.Reverse().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxTrivia current = enumerator.Current;
                if (TryGetLastTokenForStructuredTrivia(current, predicate, stepInto, out var token))
                {
                    return token;
                }
            }
            return default(SyntaxToken);
        }

        private bool TryGetLastTokenForStructuredTrivia(SyntaxTrivia trivia, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto, out SyntaxToken token)
        {
            token = default(SyntaxToken);
            if (!trivia.TryGetStructure(out var structure) || stepInto == null || !stepInto!(trivia))
            {
                return false;
            }
            token = GetLastToken(structure, predicate, stepInto);
            return token.RawKind != 0;
        }

        private SyntaxToken GetFirstToken(SyntaxToken token, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            if (stepInto != null)
            {
                SyntaxToken firstToken = GetFirstToken(token.LeadingTrivia, predicate, stepInto);
                if (firstToken.RawKind != 0)
                {
                    return firstToken;
                }
            }
            if (Matches(predicate, token))
            {
                return token;
            }
            if (stepInto != null)
            {
                SyntaxToken firstToken2 = GetFirstToken(token.TrailingTrivia, predicate, stepInto);
                if (firstToken2.RawKind != 0)
                {
                    return firstToken2;
                }
            }
            return default(SyntaxToken);
        }

        private SyntaxToken GetLastToken(SyntaxToken token, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            if (stepInto != null)
            {
                SyntaxToken lastToken = GetLastToken(token.TrailingTrivia, predicate, stepInto);
                if (lastToken.RawKind != 0)
                {
                    return lastToken;
                }
            }
            if (Matches(predicate, token))
            {
                return token;
            }
            if (stepInto != null)
            {
                SyntaxToken lastToken2 = GetLastToken(token.LeadingTrivia, predicate, stepInto);
                if (lastToken2.RawKind != 0)
                {
                    return lastToken2;
                }
            }
            return default(SyntaxToken);
        }

        public SyntaxToken GetNextToken(SyntaxTrivia current, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            bool returnNext = false;
            SyntaxToken nextToken = GetNextToken(current, current.Token.LeadingTrivia, predicate, stepInto, ref returnNext);
            if (nextToken.RawKind != 0)
            {
                return nextToken;
            }
            if (returnNext && (predicate == null || (Delegate)predicate == (Delegate)SyntaxToken.Any || predicate!(current.Token)))
            {
                return current.Token;
            }
            nextToken = GetNextToken(current, current.Token.TrailingTrivia, predicate, stepInto, ref returnNext);
            if (nextToken.RawKind != 0)
            {
                return nextToken;
            }
            SyntaxToken current2 = current.Token;
            return GetNextToken(in current2, predicate, searchInsideCurrentTokenTrailingTrivia: false, stepInto);
        }

        public SyntaxToken GetPreviousToken(SyntaxTrivia current, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            bool returnPrevious = false;
            SyntaxToken previousToken = GetPreviousToken(current, current.Token.TrailingTrivia, predicate, stepInto, ref returnPrevious);
            if (previousToken.RawKind != 0)
            {
                return previousToken;
            }
            if (returnPrevious && Matches(predicate, current.Token))
            {
                return current.Token;
            }
            previousToken = GetPreviousToken(current, current.Token.LeadingTrivia, predicate, stepInto, ref returnPrevious);
            if (previousToken.RawKind != 0)
            {
                return previousToken;
            }
            SyntaxToken current2 = current.Token;
            return GetPreviousToken(in current2, predicate, searchInsideCurrentTokenLeadingTrivia: false, stepInto);
        }

        private SyntaxToken GetNextToken(SyntaxTrivia current, SyntaxTriviaList list, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool>? stepInto, ref bool returnNext)
        {
            SyntaxTriviaList.Enumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxTrivia current2 = enumerator.Current;
                if (returnNext)
                {
                    if (current2.TryGetStructure(out var structure) && stepInto != null && stepInto!(current2))
                    {
                        SyntaxToken firstToken = GetFirstToken(structure, predicate, stepInto);
                        if (firstToken.RawKind != 0)
                        {
                            return firstToken;
                        }
                    }
                }
                else if (current2 == current)
                {
                    returnNext = true;
                }
            }
            return default(SyntaxToken);
        }

        private SyntaxToken GetPreviousToken(SyntaxTrivia current, SyntaxTriviaList list, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto, ref bool returnPrevious)
        {
            SyntaxTriviaList.Reversed.Enumerator enumerator = list.Reverse().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxTrivia current2 = enumerator.Current;
                if (returnPrevious)
                {
                    if (TryGetLastTokenForStructuredTrivia(current2, predicate, stepInto, out var token))
                    {
                        return token;
                    }
                }
                else if (current2 == current)
                {
                    returnPrevious = true;
                }
            }
            return default(SyntaxToken);
        }

        public SyntaxToken GetNextToken(SyntaxNode node, Func<SyntaxToken, bool>? predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            while (node.Parent != null)
            {
                bool flag = false;
                ChildSyntaxList.Enumerator enumerator = node.Parent!.ChildNodesAndTokens().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNodeOrToken current = enumerator.Current;
                    if (flag)
                    {
                        if (current.IsToken)
                        {
                            SyntaxToken firstToken = GetFirstToken(current.AsToken(), predicate, stepInto);
                            if (firstToken.RawKind != 0)
                            {
                                return firstToken;
                            }
                        }
                        else
                        {
                            SyntaxToken firstToken2 = GetFirstToken(current.AsNode(), predicate, stepInto);
                            if (firstToken2.RawKind != 0)
                            {
                                return firstToken2;
                            }
                        }
                    }
                    else if (current.IsNode && current.AsNode() == node)
                    {
                        flag = true;
                    }
                }
                node = node.Parent;
            }
            if (node.IsStructuredTrivia)
            {
                return GetNextToken(((IStructuredTriviaSyntax)node).ParentTrivia, predicate, stepInto);
            }
            return default(SyntaxToken);
        }

        public SyntaxToken GetPreviousToken(SyntaxNode node, Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto)
        {
            while (node.Parent != null)
            {
                bool flag = false;
                ChildSyntaxList.Reversed.Enumerator enumerator = node.Parent!.ChildNodesAndTokens().Reverse().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNodeOrToken current = enumerator.Current;
                    if (flag)
                    {
                        if (current.IsToken)
                        {
                            SyntaxToken lastToken = GetLastToken(current.AsToken(), predicate, stepInto);
                            if (lastToken.RawKind != 0)
                            {
                                return lastToken;
                            }
                        }
                        else
                        {
                            SyntaxToken lastToken2 = GetLastToken(current.AsNode(), predicate, stepInto);
                            if (lastToken2.RawKind != 0)
                            {
                                return lastToken2;
                            }
                        }
                    }
                    else if (current.IsNode && current.AsNode() == node)
                    {
                        flag = true;
                    }
                }
                node = node.Parent;
            }
            if (node.IsStructuredTrivia)
            {
                return GetPreviousToken(((IStructuredTriviaSyntax)node).ParentTrivia, predicate, stepInto);
            }
            return default(SyntaxToken);
        }

        public SyntaxToken GetNextToken(in SyntaxToken current, Func<SyntaxToken, bool>? predicate, bool searchInsideCurrentTokenTrailingTrivia, Func<SyntaxTrivia, bool>? stepInto)
        {
            if (current.Parent != null)
            {
                if (searchInsideCurrentTokenTrailingTrivia)
                {
                    SyntaxToken firstToken = GetFirstToken(current.TrailingTrivia, predicate, stepInto);
                    if (firstToken.RawKind != 0)
                    {
                        return firstToken;
                    }
                }
                bool flag = false;
                ChildSyntaxList.Enumerator enumerator = current.Parent!.ChildNodesAndTokens().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNodeOrToken current2 = enumerator.Current;
                    if (flag)
                    {
                        if (current2.IsToken)
                        {
                            SyntaxToken firstToken2 = GetFirstToken(current2.AsToken(), predicate, stepInto);
                            if (firstToken2.RawKind != 0)
                            {
                                return firstToken2;
                            }
                        }
                        else
                        {
                            SyntaxToken firstToken3 = GetFirstToken(current2.AsNode(), predicate, stepInto);
                            if (firstToken3.RawKind != 0)
                            {
                                return firstToken3;
                            }
                        }
                    }
                    else if (current2.IsToken && current2.AsToken() == current)
                    {
                        flag = true;
                    }
                }
                return GetNextToken(current.Parent, predicate, stepInto);
            }
            return default(SyntaxToken);
        }

        public SyntaxToken GetPreviousToken(in SyntaxToken current, Func<SyntaxToken, bool> predicate, bool searchInsideCurrentTokenLeadingTrivia, Func<SyntaxTrivia, bool>? stepInto)
        {
            if (current.Parent != null)
            {
                if (searchInsideCurrentTokenLeadingTrivia)
                {
                    SyntaxToken lastToken = GetLastToken(current.LeadingTrivia, predicate, stepInto);
                    if (lastToken.RawKind != 0)
                    {
                        return lastToken;
                    }
                }
                bool flag = false;
                ChildSyntaxList.Reversed.Enumerator enumerator = current.Parent!.ChildNodesAndTokens().Reverse().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNodeOrToken current2 = enumerator.Current;
                    if (flag)
                    {
                        if (current2.IsToken)
                        {
                            SyntaxToken lastToken2 = GetLastToken(current2.AsToken(), predicate, stepInto);
                            if (lastToken2.RawKind != 0)
                            {
                                return lastToken2;
                            }
                        }
                        else
                        {
                            SyntaxToken lastToken3 = GetLastToken(current2.AsNode(), predicate, stepInto);
                            if (lastToken3.RawKind != 0)
                            {
                                return lastToken3;
                            }
                        }
                    }
                    else if (current2.IsToken && current2.AsToken() == current)
                    {
                        flag = true;
                    }
                }
                return GetPreviousToken(current.Parent, predicate, stepInto);
            }
            return default(SyntaxToken);
        }
    }
}
