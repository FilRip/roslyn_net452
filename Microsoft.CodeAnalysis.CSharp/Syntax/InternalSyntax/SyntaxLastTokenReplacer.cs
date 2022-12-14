// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable


namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    internal class SyntaxLastTokenReplacer : CSharpSyntaxRewriter
    {
        private readonly SyntaxToken _oldToken;
        private readonly SyntaxToken _newToken;
        private int _count = 1;
        private bool _found;

        private SyntaxLastTokenReplacer(SyntaxToken oldToken, SyntaxToken newToken)
        {
            _oldToken = oldToken;
            _newToken = newToken;
        }

        public SyntaxToken OldToken
        {
            get { return _oldToken; }
        }

        internal static TRoot Replace<TRoot>(TRoot root, SyntaxToken newToken)
            where TRoot : CSharpSyntaxNode
        {
            var oldToken = root.GetLastToken();
            var replacer = new SyntaxLastTokenReplacer(oldToken, newToken);
            var newRoot = (TRoot)replacer.Visit(root);
            return newRoot;
        }

        private static int CountNonNullSlots(CSharpSyntaxNode node)
        {
            return node.ChildNodesAndTokens().Count;
        }

        public override CSharpSyntaxNode Visit(CSharpSyntaxNode node)
        {
            if (node != null && !_found)
            {
                _count--;
                if (_count == 0)
                {
                    if (node is SyntaxToken)
                    {
                        _found = true;
                        return _newToken;
                    }

                    _count += CountNonNullSlots(node);
                    return base.Visit(node);
                }
            }

            return node;
        }
    }
}
