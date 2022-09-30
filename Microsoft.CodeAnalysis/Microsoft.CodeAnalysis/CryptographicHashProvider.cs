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

        public const int Sha1HashSize = 20;

        public abstract ImmutableArray<byte> ComputeHash(HashAlgorithm algorithm);

        public ImmutableArray<byte> GetHash(AssemblyHashAlgorithm algorithmId)
        {
            using HashAlgorithm hashAlgorithm = TryGetAlgorithm(algorithmId);
            if (hashAlgorithm == null)
            {
                return ImmutableArray.Create<byte>();
            }
            return algorithmId switch
            {
                AssemblyHashAlgorithm.None or AssemblyHashAlgorithm.Sha1 => GetHash(ref _lazySHA1Hash, hashAlgorithm),
                AssemblyHashAlgorithm.Sha256 => GetHash(ref _lazySHA256Hash, hashAlgorithm),
                AssemblyHashAlgorithm.Sha384 => GetHash(ref _lazySHA384Hash, hashAlgorithm),
                AssemblyHashAlgorithm.Sha512 => GetHash(ref _lazySHA512Hash, hashAlgorithm),
                AssemblyHashAlgorithm.MD5 => GetHash(ref _lazyMD5Hash, hashAlgorithm),
                _ => throw ExceptionUtilities.UnexpectedValue(algorithmId),
            };
        }

        public static int GetHashSize(SourceHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                SourceHashAlgorithm.Sha1 => 20,
                SourceHashAlgorithm.Sha256 => 32,
                _ => throw ExceptionUtilities.UnexpectedValue(algorithmId),
            };
        }

        public static HashAlgorithm? TryGetAlgorithm(SourceHashAlgorithm algorithmId)
        {
            return algorithmId switch
            {
                SourceHashAlgorithm.Sha1 => SHA1.Create(),
                SourceHashAlgorithm.Sha256 => SHA256.Create(),
                _ => null,
            };
        }

        // FilRip : Commented, System.Security.Cryptography.Primitives & Algorithms not exists in 4.5.2
        /*public static HashAlgorithmName GetAlgorithmName(SourceHashAlgorithm algorithmId)
		{
			return algorithmId switch
			{
				SourceHashAlgorithm.Sha1 => HashAlgorithmName.SHA1, 
				SourceHashAlgorithm.Sha256 => HashAlgorithmName.SHA256, 
				_ => throw ExceptionUtilities.UnexpectedValue(algorithmId), 
			};
		}*/

        public static HashAlgorithm? TryGetAlgorithm(AssemblyHashAlgorithm algorithmId)
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
            if (algorithmId == AssemblyHashAlgorithm.None || (uint)(algorithmId - 32771) <= 1u || (uint)(algorithmId - 32780) <= 2u)
            {
                return true;
            }
            return false;
        }

        private ImmutableArray<byte> GetHash(ref ImmutableArray<byte> lazyHash, HashAlgorithm algorithm)
        {
            if (lazyHash.IsDefault)
            {
                ImmutableInterlocked.InterlockedCompareExchange(ref lazyHash, ComputeHash(algorithm), default(ImmutableArray<byte>));
            }
            return lazyHash;
        }

        public static ImmutableArray<byte> ComputeSha1(Stream stream)
        {
            if (stream != null)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                using SHA1 sHA = SHA1.Create();
                return ImmutableArray.Create(sHA.ComputeHash(stream));
            }
            return ImmutableArray<byte>.Empty;
        }

        public static ImmutableArray<byte> ComputeSha1(ImmutableArray<byte> bytes)
        {
            return ComputeSha1(bytes.ToArray());
        }

        public static ImmutableArray<byte> ComputeSha1(byte[] bytes)
        {
            using SHA1 sHA = SHA1.Create();
            return ImmutableArray.Create(sHA.ComputeHash(bytes));
        }

        /*public static ImmutableArray<byte> ComputeHash(HashAlgorithmName algorithmName, IEnumerable<Blob> bytes)
		{
			using IncrementalHash incrementalHash = IncrementalHash.CreateHash(algorithmName);
			incrementalHash.AppendData(bytes);
			return ImmutableArray.Create(incrementalHash.GetHashAndReset());
		}

		public static ImmutableArray<byte> ComputeHash(HashAlgorithmName algorithmName, IEnumerable<ArraySegment<byte>> bytes)
		{
			using IncrementalHash incrementalHash = IncrementalHash.CreateHash(algorithmName);
			incrementalHash.AppendData(bytes);
			return ImmutableArray.Create(incrementalHash.GetHashAndReset());
		}

		internal static ImmutableArray<byte> ComputeSourceHash(ImmutableArray<byte> bytes, SourceHashAlgorithm hashAlgorithm = SourceHashAlgorithm.Sha256)
		{
			using IncrementalHash incrementalHash = IncrementalHash.CreateHash(GetAlgorithmName(hashAlgorithm));
			incrementalHash.AppendData(bytes.ToArray());
			return ImmutableArray.Create(incrementalHash.GetHashAndReset());
		}

		internal static ImmutableArray<byte> ComputeSourceHash(IEnumerable<Blob> bytes, SourceHashAlgorithm hashAlgorithm = SourceHashAlgorithm.Sha256)
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
