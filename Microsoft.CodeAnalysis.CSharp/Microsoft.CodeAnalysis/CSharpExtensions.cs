using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class CSharpExtensions
    {
        public static bool IsKind(this SyntaxToken token, SyntaxKind kind)
        {
            return token.RawKind == (int)kind;
        }

        public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind)
        {
            return trivia.RawKind == (int)kind;
        }

        public static bool IsKind([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] this SyntaxNode? node, SyntaxKind kind)
        {
            return node?.RawKind == (int?)kind;
        }

        public static bool IsKind(this SyntaxNodeOrToken nodeOrToken, SyntaxKind kind)
        {
            return nodeOrToken.RawKind == (int)kind;
        }

        internal static SyntaxKind ContextualKind(this SyntaxToken token)
        {
            if ((object)token.Language != "C#")
            {
                return SyntaxKind.None;
            }
            return (SyntaxKind)token.RawContextualKind;
        }

        internal static bool IsUnderscoreToken(this SyntaxToken identifier)
        {
            return identifier.ContextualKind() == SyntaxKind.UnderscoreToken;
        }

        public static int IndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf((int)kind);
        }

        public static bool Any<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) >= 0;
        }

        public static int IndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf((int)kind);
        }

        public static bool Any<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) >= 0;
        }

        public static int IndexOf(this SyntaxTriviaList list, SyntaxKind kind)
        {
            return list.IndexOf((int)kind);
        }

        public static bool Any(this SyntaxTriviaList list, SyntaxKind kind)
        {
            return list.IndexOf(kind) >= 0;
        }

        public static int IndexOf(this SyntaxTokenList list, SyntaxKind kind)
        {
            return list.IndexOf((int)kind);
        }

        public static bool Any(this SyntaxTokenList list, SyntaxKind kind)
        {
            return list.IndexOf(kind) >= 0;
        }

        internal static SyntaxToken FirstOrDefault(this SyntaxTokenList list, SyntaxKind kind)
        {
            int num = list.IndexOf(kind);
            if (num < 0)
            {
                return default(SyntaxToken);
            }
            return list[num];
        }
    }
}
