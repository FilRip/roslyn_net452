// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Struct containing information about a source declaration.
    /// </summary>
    public readonly struct DeclarationInfo
    {
#nullable enable
        public DeclarationInfo(SyntaxNode declaredNode, ImmutableArray<SyntaxNode> executableCodeBlocks, ISymbol? declaredSymbol)
#nullable restore
        {
            Debug.Assert(declaredNode != null);
            Debug.Assert(!executableCodeBlocks.IsDefault);

            // TODO: Below assert has been commented out as is not true for VB field decls where multiple variables can share same initializer.
            // Declared node is the identifier, which doesn't contain the initializer. Can we tweak the assert somehow to handle this case?
#pragma warning disable S125 // Sections of code should not be commented out
            // Debug.Assert(executableCodeBlocks.All(n => n.Ancestors().Contains(declaredNode)));
#pragma warning restore S125 // Sections of code should not be commented out

            DeclaredNode = declaredNode;
            ExecutableCodeBlocks = executableCodeBlocks;
            DeclaredSymbol = declaredSymbol;
        }

        /// <summary>
        /// Topmost syntax node for this declaration.
        /// </summary>
        public SyntaxNode DeclaredNode { get; }

        /// <summary>
        /// Syntax nodes for executable code blocks (method body, initializers, etc.) associated with this declaration.
        /// </summary>
        public ImmutableArray<SyntaxNode> ExecutableCodeBlocks { get; }

        /// <summary>
        /// Symbol declared by this declaration.
        /// </summary>
#nullable enable
        public ISymbol? DeclaredSymbol { get; }
    }
}
