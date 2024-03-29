// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class SourceAssemblySymbol : AssemblySymbol, ISourceAssemblySymbol, IAssemblySymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Symbols.SourceAssemblySymbol _underlying;

        public SourceAssemblySymbol(Symbols.SourceAssemblySymbol underlying)
        {
            _underlying = underlying;
        }

        internal override Symbols.AssemblySymbol UnderlyingAssemblySymbol => _underlying;
        internal override CSharp.Symbol UnderlyingSymbol => _underlying;

        Compilation ISourceAssemblySymbol.Compilation => _underlying.DeclaringCompilation;
    }
}
