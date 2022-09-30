using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IAssemblySymbol : ISymbol, IEquatable<ISymbol?>
    {
        bool IsInteractive { get; }

        AssemblyIdentity Identity { get; }

        INamespaceSymbol GlobalNamespace { get; }

        IEnumerable<IModuleSymbol> Modules { get; }

        ICollection<string> TypeNames { get; }

        ICollection<string> NamespaceNames { get; }

        bool MightContainExtensionMethods { get; }

        bool GivesAccessTo(IAssemblySymbol toAssembly);

        INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName);

        INamedTypeSymbol? ResolveForwardedType(string fullyQualifiedMetadataName);

        ImmutableArray<INamedTypeSymbol> GetForwardedTypes();

        AssemblyMetadata? GetMetadata();
    }
}
