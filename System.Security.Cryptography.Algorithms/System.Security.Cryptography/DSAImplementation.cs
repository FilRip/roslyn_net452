using System.Buffers;
using System.IO;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal static class DSAImplementation
	{
		public sealed class DSACng : DSA
		{
			private SafeNCryptKeyHandle _keyHandle;

			private int _lastKeySize;

			private static KeySizes[] s_legalKeySizes = new KeySizes[1]
			{
				new KeySizes(512, 3072, 64)
			};

			private static readonly int s_defaultKeySize = (Supports2048KeySize() ? 2048 : 1024);

			public override KeySizes[] LegalKeySizes => base.LegalKeySizes;

			public override string SignatureAlgorithm => "DSA";

			public override string KeyExchangeAlgorithm => null;

			private SafeNCryptKeyHandle GetDuplicatedKeyHandle()
			{
				int keySize = KeySize;
				if (_lastKeySize != keySize)
				{
					if (_keyHandle != null)
					{
						_keyHandle.Dispose();
					}
					_keyHandle = CngKeyLite.GenerateNewExportableKey("DSA", keySize);
					_lastKeySize = keySize;
				}
				return new DuplicateSafeNCryptKeyHandle(_keyHandle);
			}

			private byte[] ExportKeyBlob(bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "PRIVATEBLOB" : "PUBLICBLOB");
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return CngKeyLite.ExportKeyBlob(keyHandle, blobType);
			}

			private void ImportKeyBlob(byte[] rsaBlob, bool includePrivate)
			{
				string blobType = (includePrivate ? "PRIVATEBLOB" : "PUBLICBLOB");
				int keyLength = CngKeyLite.GetKeyLength(_keyHandle = CngKeyLite.ImportKeyBlob(blobType, rsaBlob));
				ForceSetKeySize(keyLength);
				_lastKeySize = keyLength;
			}

			public DSACng()
				: this(s_defaultKeySize)
			{
			}

			public DSACng(int keySize)
			{
				LegalKeySizesValue = s_legalKeySizes;
				KeySize = keySize;
			}

			protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
			{
				return CngCommon.HashData(data, offset, count, hashAlgorithm);
			}

			protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
			{
				return CngCommon.HashData(data, hashAlgorithm);
			}

			protected override bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
			{
				return CngCommon.TryHashData(source, destination, hashAlgorithm, out bytesWritten);
			}

			private void ForceSetKeySize(int newKeySize)
			{
				KeySizeValue = newKeySize;
			}

			private static bool Supports2048KeySize()
			{
				Version version = Environment.OSVersion.Version;
				return version.Major > 6 || (version.Major == 6 && version.Minor >= 2);
			}

			public override void ImportParameters(DSAParameters parameters)
			{
				if (parameters.P == null || parameters.Q == null || parameters.G == null || parameters.Y == null)
				{
					throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MissingFields);
				}
				if (parameters.J != null && parameters.J.Length >= parameters.P.Length)
				{
					throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedPJ);
				}
				bool flag = parameters.X != null;
				int num = parameters.P.Length;
				int num2 = num * 8;
				if (parameters.G.Length != num || parameters.Y.Length != num)
				{
					throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedPGY);
				}
				if (flag && parameters.X.Length != parameters.Q.Length)
				{
					throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedQX);
				}
				byte[] blob;
				if (num2 <= 1024)
				{
					GenerateV1DsaBlob(out blob, parameters, num, flag);
				}
				else
				{
					GenerateV2DsaBlob(out blob, parameters, num, flag);
				}
				ImportKeyBlob(blob, flag);
			}

			private unsafe static void GenerateV1DsaBlob(out byte[] blob, DSAParameters parameters, int cbKey, bool includePrivate)
			{
				int num = sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB) + cbKey + cbKey + cbKey;
				if (includePrivate)
				{
					num += 20;
				}
				blob = new byte[num];
				fixed (byte* ptr = &blob[0])
				{
					Interop.BCrypt.BCRYPT_DSA_KEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_DSA_KEY_BLOB*)ptr;
					ptr2->Magic = (includePrivate ? Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC : Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC);
					ptr2->cbKey = cbKey;
					int offset = 8;
					if (parameters.Seed != null)
					{
						if (parameters.Seed.Length != 20)
						{
							throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_SeedRestriction_ShortKey);
						}
						Interop.BCrypt.EmitBigEndian(blob, ref offset, parameters.Counter);
						Interop.BCrypt.Emit(blob, ref offset, parameters.Seed);
					}
					else
					{
						Interop.BCrypt.EmitByte(blob, ref offset, byte.MaxValue, 24);
					}
					if (parameters.Q.Length != 20)
					{
						throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_QRestriction_ShortKey);
					}
					Interop.BCrypt.Emit(blob, ref offset, parameters.Q);
					Interop.BCrypt.Emit(blob, ref offset, parameters.P);
					Interop.BCrypt.Emit(blob, ref offset, parameters.G);
					Interop.BCrypt.Emit(blob, ref offset, parameters.Y);
					if (includePrivate)
					{
						Interop.BCrypt.Emit(blob, ref offset, parameters.X);
					}
				}
			}

			private unsafe static void GenerateV2DsaBlob(out byte[] blob, DSAParameters parameters, int cbKey, bool includePrivateParameters)
			{
				int num = sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2) + ((parameters.Seed == null) ? parameters.Q.Length : parameters.Seed.Length) + parameters.Q.Length + parameters.P.Length + parameters.G.Length + parameters.Y.Length + (includePrivateParameters ? parameters.X.Length : 0);
				blob = new byte[num];
				fixed (byte* ptr = &blob[0])
				{
					Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2* ptr2 = (Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2*)ptr;
					ptr2->Magic = (includePrivateParameters ? Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC_V2 : Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC_V2);
					ptr2->cbKey = cbKey;
					ptr2->hashAlgorithm = parameters.Q.Length switch
					{
						20 => Interop.BCrypt.HASHALGORITHM_ENUM.DSA_HASH_ALGORITHM_SHA1, 
						32 => Interop.BCrypt.HASHALGORITHM_ENUM.DSA_HASH_ALGORITHM_SHA256, 
						64 => Interop.BCrypt.HASHALGORITHM_ENUM.DSA_HASH_ALGORITHM_SHA512, 
						_ => throw new PlatformNotSupportedException(SR.Cryptography_InvalidDsaParameters_QRestriction_LargeKey), 
					};
					ptr2->standardVersion = Interop.BCrypt.DSAFIPSVERSION_ENUM.DSA_FIPS186_3;
					int offset = sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2) - 4;
					if (parameters.Seed != null)
					{
						Interop.BCrypt.EmitBigEndian(blob, ref offset, parameters.Counter);
						ptr2->cbSeedLength = parameters.Seed.Length;
						ptr2->cbGroupSize = parameters.Q.Length;
						Interop.BCrypt.Emit(blob, ref offset, parameters.Seed);
					}
					else
					{
						Interop.BCrypt.EmitByte(blob, ref offset, byte.MaxValue, 4);
						int count = (ptr2->cbSeedLength = parameters.Q.Length);
						ptr2->cbGroupSize = parameters.Q.Length;
						Interop.BCrypt.EmitByte(blob, ref offset, byte.MaxValue, count);
					}
					Interop.BCrypt.Emit(blob, ref offset, parameters.Q);
					Interop.BCrypt.Emit(blob, ref offset, parameters.P);
					Interop.BCrypt.Emit(blob, ref offset, parameters.G);
					Interop.BCrypt.Emit(blob, ref offset, parameters.Y);
					if (includePrivateParameters)
					{
						Interop.BCrypt.Emit(blob, ref offset, parameters.X);
					}
				}
			}

			public unsafe override DSAParameters ExportParameters(bool includePrivateParameters)
			{
				byte[] array = ExportKeyBlob(includePrivateParameters);
				Interop.BCrypt.KeyBlobMagicNumber keyBlobMagicNumber = (Interop.BCrypt.KeyBlobMagicNumber)BitConverter.ToInt32(array, 0);
				CheckMagicValueOfKey(keyBlobMagicNumber, includePrivateParameters);
				DSAParameters result = default(DSAParameters);
				fixed (byte* ptr = array)
				{
					if (keyBlobMagicNumber == Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC || keyBlobMagicNumber == Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC)
					{
						if (array.Length < sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB))
						{
							throw Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
						}
						Interop.BCrypt.BCRYPT_DSA_KEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_DSA_KEY_BLOB*)ptr;
						int offset = 8;
						result.Counter = FromBigEndian(Interop.BCrypt.Consume(array, ref offset, 4));
						result.Seed = Interop.BCrypt.Consume(array, ref offset, 20);
						result.Q = Interop.BCrypt.Consume(array, ref offset, 20);
						result.P = Interop.BCrypt.Consume(array, ref offset, ptr2->cbKey);
						result.G = Interop.BCrypt.Consume(array, ref offset, ptr2->cbKey);
						result.Y = Interop.BCrypt.Consume(array, ref offset, ptr2->cbKey);
						if (includePrivateParameters)
						{
							result.X = Interop.BCrypt.Consume(array, ref offset, 20);
						}
					}
					else
					{
						if (array.Length < sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2))
						{
							throw Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
						}
						Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2* ptr3 = (Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2*)ptr;
						int offset = sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2) - 4;
						result.Counter = FromBigEndian(Interop.BCrypt.Consume(array, ref offset, 4));
						result.Seed = Interop.BCrypt.Consume(array, ref offset, ptr3->cbSeedLength);
						result.Q = Interop.BCrypt.Consume(array, ref offset, ptr3->cbGroupSize);
						result.P = Interop.BCrypt.Consume(array, ref offset, ptr3->cbKey);
						result.G = Interop.BCrypt.Consume(array, ref offset, ptr3->cbKey);
						result.Y = Interop.BCrypt.Consume(array, ref offset, ptr3->cbKey);
						if (includePrivateParameters)
						{
							result.X = Interop.BCrypt.Consume(array, ref offset, ptr3->cbGroupSize);
						}
					}
					if (result.Counter == -1)
					{
						result.Counter = 0;
						result.Seed = null;
					}
					return result;
				}
			}

			private static int FromBigEndian(byte[] b)
			{
				return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
			}

			private static void CheckMagicValueOfKey(Interop.BCrypt.KeyBlobMagicNumber magic, bool includePrivateParameters)
			{
				if (includePrivateParameters)
				{
					if (magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC && magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC_V2)
					{
						throw new CryptographicException(SR.Cryptography_NotValidPrivateKey);
					}
				}
				else if (magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC && magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC_V2)
				{
					throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
				}
			}

			public unsafe override byte[] CreateSignature(byte[] rgbHash)
			{
				if (rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				ReadOnlySpan<byte> hash = rgbHash;
				byte[] array = AdjustHashSizeIfNecessaryWithArrayPool(ref hash);
				try
				{
					using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
					return keyHandle.SignHash(hash, Interop.NCrypt.AsymmetricPaddingMode.None, null, hash.Length * 2);
				}
				finally
				{
					if (array != null)
					{
						Array.Clear(array, 0, hash.Length);
						ArrayPool<byte>.Shared.Return(array);
					}
				}
			}

			public unsafe override bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
			{
				byte[] array = AdjustHashSizeIfNecessaryWithArrayPool(ref hash);
				try
				{
					using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
					return keyHandle.TrySignHash(hash, destination, Interop.NCrypt.AsymmetricPaddingMode.None, null, out bytesWritten);
				}
				finally
				{
					if (array != null)
					{
						Array.Clear(array, 0, hash.Length);
						ArrayPool<byte>.Shared.Return(array);
					}
				}
			}

			public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
			{
				if (rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if (rgbSignature == null)
				{
					throw new ArgumentNullException("rgbSignature");
				}
				return VerifySignature((ReadOnlySpan<byte>)rgbHash, (ReadOnlySpan<byte>)rgbSignature);
			}

			public unsafe override bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
			{
				byte[] array = AdjustHashSizeIfNecessaryWithArrayPool(ref hash);
				try
				{
					using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
					return keyHandle.VerifyHash(hash, signature, Interop.NCrypt.AsymmetricPaddingMode.None, null);
				}
				finally
				{
					if (array != null)
					{
						Array.Clear(array, 0, hash.Length);
						ArrayPool<byte>.Shared.Return(array);
					}
				}
			}

			private byte[] AdjustHashSizeIfNecessaryWithArrayPool(ref ReadOnlySpan<byte> hash)
			{
				int num = ComputeQLength();
				if (num == hash.Length)
				{
					return null;
				}
				if (num < hash.Length)
				{
					hash = hash.Slice(0, num);
					return null;
				}
				byte[] array = ArrayPool<byte>.Shared.Rent(num);
				hash.CopyTo(new Span<byte>(array, num - hash.Length, hash.Length));
				hash = new ReadOnlySpan<byte>(array, 0, num);
				return array;
			}

			private unsafe int ComputeQLength()
			{
				byte[] array;
				using (GetDuplicatedKeyHandle())
				{
					array = ExportKeyBlob(includePrivateParameters: false);
				}
				if (array.Length < sizeof(Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2))
				{
					return 20;
				}
				fixed (byte* ptr = array)
				{
					Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2* ptr2 = (Interop.BCrypt.BCRYPT_DSA_KEY_BLOB_V2*)ptr;
					if (ptr2->Magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC_V2 && ptr2->Magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC_V2)
					{
						return 20;
					}
					return ptr2->cbGroupSize;
				}
			}
		}
	}
}
