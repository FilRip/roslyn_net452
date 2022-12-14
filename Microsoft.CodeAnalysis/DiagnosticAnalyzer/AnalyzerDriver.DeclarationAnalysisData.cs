// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract partial class AnalyzerDriver
    {
        internal sealed class DeclarationAnalysisData
        {
            public DeclarationAnalysisData(
                SyntaxNode declaringReferenceSyntax,
                SyntaxNode topmostNodeForAnalysis,
                ImmutableArray<DeclarationInfo> declarationsInNodeBuilder,
                ImmutableArray<SyntaxNode> descendantNodesToAnalyze,
                bool isPartialAnalysis)
            {
                DeclaringReferenceSyntax = declaringReferenceSyntax;
                TopmostNodeForAnalysis = topmostNodeForAnalysis;
                DeclarationsInNode = declarationsInNodeBuilder;
                DescendantNodesToAnalyze = descendantNodesToAnalyze;
                IsPartialAnalysis = isPartialAnalysis;
            }

            /// <summary>
            /// GetSyntax() for the given SyntaxReference.
            /// </summary>
            public SyntaxNode DeclaringReferenceSyntax { get; }

            /// <summary>
            /// Topmost declaration node for analysis.
            /// </summary>
            public SyntaxNode TopmostNodeForAnalysis { get; }

            /// <summary>
            /// All member declarations within the declaration.
            /// </summary>
            public ImmutableArray<DeclarationInfo> DeclarationsInNode { get; }

            /// <summary>
            /// All descendant nodes for syntax node actions.
            /// </summary>
            public ImmutableArray<SyntaxNode> DescendantNodesToAnalyze { get; }

            /// <summary>
            /// Flag indicating if this is a partial analysis.
            /// </summary>
            public bool IsPartialAnalysis { get; }
        }
    }
}
