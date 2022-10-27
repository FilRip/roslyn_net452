// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class CryptographicHashProvider
    {
        private ImmutableArray<byte> _lazySHA1Hash;
        private ImmutableArray<byte> _lazySHA256Hash;
        private ImmutableArray<byte> _lazySHA384Hash;
        private ImmutableArray<byte> _lazySHA512Hash;
        private ImmutableArray<byte> _lazyMD5Hash;

        internal abstract ImmutableArray<byte> ComputeHash(HashAlgorithm algorithm);

        internal ImmutableArray<byte> GetHash(AssemblyHashAlgorithm algorithmId)
        {
            using HashAlgorithm? algorithm = TryGetAlgorithm(algorithmId);
            // ERR_CryptoHashFailed has already been reported:
            if (algorithm == null)
            {
                return ImmutableArray.Create<byte>();
            }

            return algorithmId switch
            {
                AssemblyHashAlgorithm.None or AssemblyHashAlgorithm.Sha1 => GetHash(ref _lazySHA1Hash, algorithm),
                AssemblyHashAlgorithm.Sha256 => GetHash(ref _lazySHA256Hash, algorithm),
                AssemblyHashAlgorithm.Sha384 => GetHash(ref _lazySHA384Hash, algorithm),
                AssemblyHashAlgorithm.Sha512 => GetHash(ref _lazySHA512Hash, algorithm),
                AssemblyHashAlgorithm.MD5 => GetHash(ref _lazyMD5Hash, algorithm),
                _ => throw ExceptionUtilities.UnexpectedValue(algorithmId),
            };
        }

        internal static int GetHashSize(SourceHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                SourceHashAlgorithm.Sha1 => 160 / 8,
                SourceHashAlgorithm.Sha256 => 256 / 8,
                _ => throw ExceptionUtilities.UnexpectedValue(algorithmId),
            };
        }

        internal static HashAlgorithm? TryGetAlgorithm(SourceHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                SourceHashAlgorithm.Sha1 => SHA1.Create(),
                SourceHashAlgorithm.Sha256 => SHA256.Create(),
                _ => null,
            };
        }

        // TODO : FilRip : Commented, System.Security.Cryptography.Primitives & Algorithms not exists in 4.5.2
        /*internal static HashAlgorithmName GetAlgorithmName(SourceHashAlgorithm algorithmId)
        {
            switch (algorithmId)
            {
                case SourceHashAlgorithm.Sha1:
                    return HashAlgorithmName.SHA1;

                case SourceHashAlgorithm.Sha256:
                    return HashAlgorithmName.SHA256;

                default:
                    throw ExceptionUtilities.UnexpectedValue(algorithmId);
            }
        }*/

        internal static HashAlgorithm? TryGetAlgorithm(AssemblyHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                AssemblyHashAlgorithm.None or AssemblyHashAlgorithm.Sha1 => SHA1.Create(),
                AssemblyHashAlgorithm.Sha256 => SHA256.Create(),
                AssemblyHashAlgorithm.Sha384 => SHA384.Create(),
                AssemblyHashAlgorithm.Sha512 => SHA512.Create(),
                AssemblyHashAlgorithm.MD5 => MD5.Create(),
                _ => null,
            };
        }

        public static bool IsSupportedAlgorithm(AssemblyHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                AssemblyHashAlgorithm.None or AssemblyHashAlgorithm.Sha1 or AssemblyHashAlgorithm.Sha256 or AssemblyHashAlgorithm.Sha384 or AssemblyHashAlgorithm.Sha512 or AssemblyHashAlgorithm.MD5 => true,
                _ => false,
            };
        }

        private ImmutableArray<byte> GetHash(ref ImmutableArray<byte> lazyHash, HashAlgorithm algorithm)
        {
            if (lazyHash.IsDefault)
            {
                ImmutableInterlocked.InterlockedCompareExchange(ref lazyHash, ComputeHash(algorithm), default);
            }

            return lazyHash;
        }

        internal const int Sha1HashSize = 20;

        internal static ImmutableArray<byte> ComputeSha1(Stream stream)
        {
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using var hashProvider = SHA1.Create();
                return ImmutableArray.Create(hashProvider.ComputeHash(stream));
            }

            return ImmutableArray<byte>.Empty;
        }

        internal static ImmutableArray<byte> ComputeSha1(ImmutableArray<byte> bytes)
        {
            return ComputeSha1(bytes.ToArray());
        }

        internal static ImmutableArray<byte> ComputeSha1(byte[] bytes)
        {
            using var hashProvider = SHA1.Create();
            return ImmutableArray.Create(hashProvider.ComputeHash(bytes));
        }

        /*internal static ImmutableArray<byte> ComputeHash(HashAlgorithmName algorithmName, IEnumerable<Blob> bytes)
        {
            using (var incrementalHash = IncrementalHash.CreateHash(algorithmName))
            {
                incrementalHash.AppendData(bytes);
                return ImmutableArray.Create(incrementalHash.GetHashAndReset());
            }
        }

        internal static ImmutableArray<byte> ComputeHash(HashAlgorithmName algorithmName, IEnumerable<ArraySegment<byte>> bytes)
        {
            using (var incrementalHash = IncrementalHash.CreateHash(algorithmName))
            {
                incrementalHash.AppendData(bytes);
                return ImmutableArray.Create(incrementalHash.GetHashAndReset());
            }
        }

        internal static ImmutableArray<byte> ComputeSourceHash(ImmutableArray<byte> bytes, SourceHashAlgorithm hashAlgorithm = SourceHashAlgorithmUtils.DefaultContentHashAlgorithm)
        {
            var algorithmName = GetAlgorithmName(hashAlgorithm);
            using (var incrementalHash = IncrementalHash.CreateHash(algorithmName))
            {
                incrementalHash.AppendData(bytes.ToArray());
                return ImmutableArray.Create(incrementalHash.GetHashAndReset());
            }
        }

        internal static ImmutableArray<byte> ComputeSourceHash(IEnumerable<Blob> bytes, SourceHashAlgorithm hashAlgorithm = SourceHashAlgorithmUtils.DefaultContentHashAlgorithm)
        {
            return ComputeHash(GetAlgorithmName(hashAlgorithm), bytes);
        }*/


        // FilRip : Replace ComputeHash & ComputeSourceHash above by something compatible with net4.5.2 below :
        public static ImmutableArray<byte> ComputeHash(SourceHashAlgorithm algorithmId, IEnumerable<Blob> bytes)
        {
            byte[] octets = new byte[0];
            foreach (Blob b in bytes)
                if (b.GetBytes() != null && b.GetBytes().Count > 0)
                    octets = octets.Concat(b.GetBytes()).ToArray();

            HashAlgorithm sha;

            switch (algorithmId)
            {
                case SourceHashAlgorithm.Sha1:
                    sha = SHA1.Create();
                    break;
                case SourceHashAlgorithm.Sha256:
                    sha = SHA256.Create();
                    break;
                default:
                    return ImmutableArray.Create(octets);
            }
            return ImmutableArray.Create(sha.ComputeHash(octets));
        }

        public static ImmutableArray<byte> ComputeHash(SourceHashAlgorithm algorithmId, IEnumerable<ArraySegment<byte>> bytes)
        {
            byte[] octets = new byte[0];
            foreach (ArraySegment<byte> b in bytes)
                if (b.Array != null && b.Array.Length > 0)
                    octets = octets.Concat(b.Array).ToArray();

            HashAlgorithm sha;

            switch (algorithmId)
            {
                case SourceHashAlgorithm.Sha1:
                    sha = SHA1.Create();
                    break;
                case SourceHashAlgorithm.Sha256:
                    sha = SHA256.Create();
                    break;
                default:
                    return ImmutableArray.Create(octets);
            }
            return ImmutableArray.Create(sha.ComputeHash(octets));
        }

        public static ImmutableArray<byte> ComputeHash(SourceHashAlgorithm algorithmId, ImmutableArray<byte> bytes)
        {
            byte[] octets = new byte[0];
            octets = octets.Concat(bytes).ToArray();

            HashAlgorithm sha;

            switch (algorithmId)
            {
                case SourceHashAlgorithm.Sha1:
                    sha = SHA1.Create();
                    break;
                case SourceHashAlgorithm.Sha256:
                    sha = SHA256.Create();
                    break;
                default:
                    return ImmutableArray.Create(octets);
            }
            return ImmutableArray.Create(sha.ComputeHash(octets));
        }

        public static ImmutableArray<byte> ComputeSha256(Stream stream)
        {
            if (stream != null)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                using SHA256 sHA = SHA256.Create();
                return ImmutableArray.Create(sHA.ComputeHash(stream));
            }
            return ImmutableArray<byte>.Empty;
        }

        public static ImmutableArray<byte> ComputeSha256(ImmutableArray<byte> bytes)
        {
            return ComputeSha256(bytes.ToArray());
        }

        public static ImmutableArray<byte> ComputeSha256(byte[] bytes)
        {
            using SHA256 sHA = SHA256.Create();
            return ImmutableArray.Create(sHA.ComputeHash(bytes));
        }
    }
}
