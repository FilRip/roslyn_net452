// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// AssemblySymbol to represent missing, for whatever reason, CorLibrary.
    /// The symbol is created by ReferenceManager on as needed basis and is shared by all compilations
    /// with missing CorLibraries.
    /// </summary>
    internal sealed class MissingCorLibrarySymbol : MissingAssemblySymbol
    {
        internal static readonly MissingCorLibrarySymbol Instance = new();

        /// <summary>
        /// An array of cached Cor types defined in this assembly.
        /// Lazily filled by GetDeclaredSpecialType method.
        /// </summary>
        /// <remarks></remarks>
        private NamedTypeSymbol[] _lazySpecialTypes;

        private MissingCorLibrarySymbol()
            : base(new AssemblyIdentity("<Missing Core Assembly>"))
        {
            this.SetCorLibrary(this);
        }

        /// <summary>
        /// Lookup declaration for predefined CorLib type in this Assembly. Only should be
        /// called if it is know that this is the Cor Library (mscorlib).
        /// </summary>
        /// <param name="type"></param>
        internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
        {
#if DEBUG
            foreach (var module in this.Modules)
            {
            }
#endif

            if (_lazySpecialTypes == null)
            {
                Interlocked.CompareExchange(ref _lazySpecialTypes,
                    new NamedTypeSymbol[(int)SpecialType.Count + 1], null);
            }

            if (_lazySpecialTypes[(int)type] is null)
            {
                MetadataTypeName emittedFullName = MetadataTypeName.FromFullName(SpecialTypes.GetMetadataName(type), useCLSCompliantNameArityEncoding: true);
                NamedTypeSymbol corType = new MissingMetadataTypeSymbol.TopLevel(this.moduleSymbol, ref emittedFullName, type);
                Interlocked.CompareExchange(ref _lazySpecialTypes[(int)type], corType, null);
            }

            return _lazySpecialTypes[(int)type];
        }
    }
}
