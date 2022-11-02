// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Represents structured trivia that contains skipped tokens. This is implemented by
    /// <see cref="T:Microsoft.CodeAnalysis.CSharp.Syntax.SkippedTokensTriviaSyntax"/> and
    /// <see cref="T:Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax"/>.
    /// </summary>
    public interface ISkippedTokensTriviaSyntax
    {
        SyntaxTokenList Tokens { get; }
    }
}
