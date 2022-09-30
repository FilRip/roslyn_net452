using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal static class AssemblyIdentityUtils
    {
        public static AssemblyIdentity? TryGetAssemblyIdentity(string filePath)
        {
            try
            {
                using FileStream peStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using PEReader peReader = new PEReader(peStream);
                MetadataReader metadataReader = peReader.GetMetadataReader();
                AssemblyDefinition assemblyDefinition = metadataReader.GetAssemblyDefinition();
                string @string = metadataReader.GetString(assemblyDefinition.Name);
                Version version = assemblyDefinition.Version;
                StringHandle culture = assemblyDefinition.Culture;
                string cultureName = ((!culture.IsNil) ? metadataReader.GetString(culture) : null);
                bool hasPublicKey = (assemblyDefinition.Flags & AssemblyFlags.PublicKey) != 0;
                BlobHandle publicKey = assemblyDefinition.PublicKey;
                ImmutableArray<byte> publicKeyOrToken = ((!publicKey.IsNil) ? metadataReader.GetBlobBytes(publicKey).AsImmutableOrNull() : default(ImmutableArray<byte>));
                return new AssemblyIdentity(@string, version, cultureName, publicKeyOrToken, hasPublicKey);
            }
            catch
            {
            }
            return null;
        }
    }
}
