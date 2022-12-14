// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable


namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    internal class SyntaxFirstTokenReplacer : CSharpSyntaxRewriter
    {
        private readonly SyntaxToken _oldToken;
        private readonly SyntaxToken _newToken;
        private readonly int _diagnosticOffsetDelta;
        private bool _foundOldToken;

        private SyntaxFirstTokenReplacer(SyntaxToken oldToken, SyntaxToken newToken, int diagnosticOffsetDelta)
        {
            _oldToken = oldToken;
            _newToken = newToken;
            _diagnosticOffsetDelta = diagnosticOffsetDelta;
            _foundOldToken = false;
        }

        public SyntaxToken OldToken
        {
            get { return _oldToken; }
        }

        internal static TRoot Replace<TRoot>(TRoot root, SyntaxToken oldToken, SyntaxToken newToken, int diagnosticOffsetDelta)
            where TRoot : CSharpSyntaxNode
        {
            var replacer = new SyntaxFirstTokenReplacer(oldToken, newToken, diagnosticOffsetDelta);
            var newRoot = (TRoot)replacer.Visit(root);
            return newRoot;
        }

        public override CSharpSyntaxNode Visit(CSharpSyntaxNode node)
        {
            if (node != null)
            {
                if (!_foundOldToken)
                {
                    if (node is SyntaxToken)
                    {
                        _foundOldToken = true;
                        return _newToken; // NB: diagnostic offsets have already been updated (by SyntaxParser.AddSkippedSyntax)
                    }

                    return UpdateDiagnosticOffset(base.Visit(node), _diagnosticOffsetDelta);
                }
            }

            return node;
        }

        private static TSyntax UpdateDiagnosticOffset<TSyntax>(TSyntax node, int diagnosticOffsetDelta) where TSyntax : CSharpSyntaxNode
        {
            DiagnosticInfo[] oldDiagnostics = node.GetDiagnostics();
            if (oldDiagnostics == null || oldDiagnostics.Length == 0)
            {
                return node;
            }

            var numDiagnostics = oldDiagnostics.Length;
            DiagnosticInfo[] newDiagnostics = new DiagnosticInfo[numDiagnostics];
            for (int i = 0; i < numDiagnostics; i++)
            {
                DiagnosticInfo oldDiagnostic = oldDiagnostics[i];
                newDiagnostics[i] = oldDiagnostic is not SyntaxDiagnosticInfo oldSyntaxDiagnostic ?
                    oldDiagnostic :
                    new SyntaxDiagnosticInfo(
                        oldSyntaxDiagnostic.Offset + diagnosticOffsetDelta,
                        oldSyntaxDiagnostic.Width,
                        (ErrorCode)oldSyntaxDiagnostic.Code,
                        oldSyntaxDiagnostic.Arguments);
            }
            return node.WithDiagnosticsGreen(newDiagnostics);
        }
    }
}
