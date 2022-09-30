using System.Reflection.Metadata;
using System.Security.Cryptography;

using Microsoft.Cci;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class StrongNameProvider
    {
        internal abstract StrongNameFileSystem FileSystem { get; }

        public abstract override int GetHashCode();

        public abstract override bool Equals(object? other);

        internal abstract void SignFile(StrongNameKeys keys, string filePath);

        internal abstract void SignBuilder(ExtendedPEBuilder peBuilder, BlobBuilder peBlob, RSAParameters privateKey);

        internal abstract StrongNameKeys CreateKeys(string? keyFilePath, string? keyContainerName, bool hasCounterSignature, CommonMessageProvider messageProvider);
    }
}
