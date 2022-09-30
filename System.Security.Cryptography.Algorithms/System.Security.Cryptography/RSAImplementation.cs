using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.Algorithms // FilRip : Added sub namespace "Algorithms"
{
	internal static class RSAImplementation
	{
		public sealed class RSACng : RSA
		{
			private SafeNCryptKeyHandle _keyHandle;

			private int _lastKeySize;

			private static readonly ConcurrentDictionary<HashAlgorithmName, int> s_hashSizes = new ConcurrentDictionary<HashAlgorithmName, int>(new KeyValuePair<HashAlgorithmName, int>[3]
			{
				KeyValuePair.Create(HashAlgorithmName.SHA256, 32),
				KeyValuePair.Create(HashAlgorithmName.SHA384, 48),
				KeyValuePair.Create(HashAlgorithmName.SHA512, 64)
			});

			public override KeySizes[] LegalKeySizes => new KeySizes[1]
			{
				new KeySizes(512, 16384, 64)
			};

			private SafeNCryptKeyHandle GetDuplicatedKeyHandle()
			{
				int keySize = KeySize;
				if (_lastKeySize != keySize)
				{
					if (_keyHandle != null)
					{
						_keyHandle.Dispose();
					}
					_keyHandle = CngKeyLite.GenerateNewExportableKey("RSA", keySize);
					_lastKeySize = keySize;
				}
				return new DuplicateSafeNCryptKeyHandle(_keyHandle);
			}

			private byte[] ExportKeyBlob(bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "RSAFULLPRIVATEBLOB" : "RSAPUBLICBLOB");
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return CngKeyLite.ExportKeyBlob(keyHandle, blobType);
			}

			private void ImportKeyBlob(byte[] rsaBlob, bool includePrivate)
			{
				string blobType = (includePrivate ? "RSAPRIVATEBLOB" : "RSAPUBLICBLOB");
				int keyLength = CngKeyLite.GetKeyLength(_keyHandle = CngKeyLite.ImportKeyBlob(blobType, rsaBlob));
				ForceSetKeySize(keyLength);
				_lastKeySize = keyLength;
			}

			public RSACng()
				: this(2048)
			{
			}

			public RSACng(int keySize)
			{
				KeySize = keySize;
			}

			protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
			{
				return CngCommon.HashData(data, offset, count, hashAlgorithm);
			}

			protected override bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
			{
				return CngCommon.TryHashData(data, destination, hashAlgorithm, out bytesWritten);
			}

			protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
			{
				return CngCommon.HashData(data, hashAlgorithm);
			}

			private void ForceSetKeySize(int newKeySize)
			{
				KeySizeValue = newKeySize;
			}

			public override byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
			{
				return EncryptOrDecrypt(data, padding, encrypt: true);
			}

			public override byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
			{
				return EncryptOrDecrypt(data, padding, encrypt: false);
			}

			public override bool TryEncrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
			{
				return TryEncryptOrDecrypt(data, destination, padding, encrypt: true, out bytesWritten);
			}

			public override bool TryDecrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
			{
				return TryEncryptOrDecrypt(data, destination, padding, encrypt: false, out bytesWritten);
			}

			private unsafe byte[] EncryptOrDecrypt(byte[] data, RSAEncryptionPadding padding, bool encrypt)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (padding == null)
				{
					throw new ArgumentNullException("padding");
				}
				int num = RsaPaddingProcessor.BytesRequiredForBitCount(KeySize);
				if (!encrypt && data.Length != num)
				{
					throw new CryptographicException(SR.Cryptography_RSA_DecryptWrongSize);
				}
				if (encrypt && padding.Mode == RSAEncryptionPaddingMode.Pkcs1 && data.Length > num - 11)
				{
					throw new CryptographicException(SR.Format(SR.Cryptography_Encryption_MessageTooLong, num - 11));
				}
				using SafeNCryptKeyHandle key = GetDuplicatedKeyHandle();
				if (encrypt && data.Length == 0)
				{
					byte[] array = ArrayPool<byte>.Shared.Rent(num);
					Span<byte> span = new Span<byte>(array, 0, num);
					try
					{
						if (padding == RSAEncryptionPadding.Pkcs1)
						{
							RsaPaddingProcessor.PadPkcs1Encryption(data, span);
						}
						else
						{
							if (padding.Mode != RSAEncryptionPaddingMode.Oaep)
							{
								throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
							}
							RsaPaddingProcessor rsaPaddingProcessor = RsaPaddingProcessor.OpenProcessor(padding.OaepHashAlgorithm);
							rsaPaddingProcessor.PadOaep(data, span);
						}
						return EncryptOrDecrypt(key, span, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_NO_PADDING_FLAG, null, encrypt);
					}
					finally
					{
						CryptographicOperations.ZeroMemory(span);
						ArrayPool<byte>.Shared.Return(array);
					}
				}
				switch (padding.Mode)
				{
				case RSAEncryptionPaddingMode.Pkcs1:
					return EncryptOrDecrypt(key, data, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PKCS1_FLAG, null, encrypt);
				case RSAEncryptionPaddingMode.Oaep:
				{
					IntPtr intPtr = Marshal.StringToHGlobalUni(padding.OaepHashAlgorithm.Name);
					try
					{
						Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO bCRYPT_OAEP_PADDING_INFO = default(Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO);
						bCRYPT_OAEP_PADDING_INFO.pszAlgId = intPtr;
						bCRYPT_OAEP_PADDING_INFO.pbLabel = IntPtr.Zero;
						bCRYPT_OAEP_PADDING_INFO.cbLabel = 0;
						Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO bCRYPT_OAEP_PADDING_INFO2 = bCRYPT_OAEP_PADDING_INFO;
						return EncryptOrDecrypt(key, data, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_OAEP_FLAG, &bCRYPT_OAEP_PADDING_INFO2, encrypt);
					}
					finally
					{
						Marshal.FreeHGlobal(intPtr);
					}
				}
				default:
					throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
				}
			}

			private unsafe bool TryEncryptOrDecrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, bool encrypt, out int bytesWritten)
			{
				if (padding == null)
				{
					throw new ArgumentNullException("padding");
				}
				int num = RsaPaddingProcessor.BytesRequiredForBitCount(KeySize);
				if (!encrypt && data.Length != num)
				{
					throw new CryptographicException(SR.Cryptography_RSA_DecryptWrongSize);
				}
				if (encrypt && padding.Mode == RSAEncryptionPaddingMode.Pkcs1 && data.Length > num - 11)
				{
					throw new CryptographicException(SR.Format(SR.Cryptography_Encryption_MessageTooLong, num - 11));
				}
				using SafeNCryptKeyHandle key = GetDuplicatedKeyHandle();
				if (encrypt && data.Length == 0)
				{
					byte[] array = ArrayPool<byte>.Shared.Rent(num);
					Span<byte> span = new Span<byte>(array, 0, num);
					try
					{
						if (padding == RSAEncryptionPadding.Pkcs1)
						{
							RsaPaddingProcessor.PadPkcs1Encryption(data, span);
						}
						else
						{
							if (padding.Mode != RSAEncryptionPaddingMode.Oaep)
							{
								throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
							}
							RsaPaddingProcessor rsaPaddingProcessor = RsaPaddingProcessor.OpenProcessor(padding.OaepHashAlgorithm);
							rsaPaddingProcessor.PadOaep(data, span);
						}
						return TryEncryptOrDecrypt(key, span, destination, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_NO_PADDING_FLAG, null, encrypt, out bytesWritten);
					}
					finally
					{
						CryptographicOperations.ZeroMemory(span);
						ArrayPool<byte>.Shared.Return(array);
					}
				}
				switch (padding.Mode)
				{
				case RSAEncryptionPaddingMode.Pkcs1:
					return TryEncryptOrDecrypt(key, data, destination, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PKCS1_FLAG, null, encrypt, out bytesWritten);
				case RSAEncryptionPaddingMode.Oaep:
				{
					IntPtr intPtr = Marshal.StringToHGlobalUni(padding.OaepHashAlgorithm.Name);
					try
					{
						Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO bCRYPT_OAEP_PADDING_INFO = default(Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO);
						bCRYPT_OAEP_PADDING_INFO.pszAlgId = intPtr;
						bCRYPT_OAEP_PADDING_INFO.pbLabel = IntPtr.Zero;
						bCRYPT_OAEP_PADDING_INFO.cbLabel = 0;
						Interop.BCrypt.BCRYPT_OAEP_PADDING_INFO bCRYPT_OAEP_PADDING_INFO2 = bCRYPT_OAEP_PADDING_INFO;
						return TryEncryptOrDecrypt(key, data, destination, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_OAEP_FLAG, &bCRYPT_OAEP_PADDING_INFO2, encrypt, out bytesWritten);
					}
					finally
					{
						Marshal.FreeHGlobal(intPtr);
					}
				}
				default:
					throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
				}
			}

			private unsafe byte[] EncryptOrDecrypt(SafeNCryptKeyHandle key, ReadOnlySpan<byte> input, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* paddingInfo, bool encrypt)
			{
				int num = KeySize / 8;
				byte[] array = new byte[num];
				int bytesNeeded;
				Interop.NCrypt.ErrorCode errorCode = EncryptOrDecrypt(key, input, array, paddingMode, paddingInfo, encrypt, out bytesNeeded);
				if (errorCode == Interop.NCrypt.ErrorCode.NTE_BUFFER_TOO_SMALL)
				{
					CryptographicOperations.ZeroMemory(array);
					array = new byte[bytesNeeded];
					errorCode = EncryptOrDecrypt(key, input, array, paddingMode, paddingInfo, encrypt, out bytesNeeded);
				}
				if (errorCode != 0)
				{
					throw errorCode.ToCryptographicException();
				}
				if (bytesNeeded != array.Length)
				{
					byte[] array2 = array.AsSpan(0, bytesNeeded).ToArray();
					CryptographicOperations.ZeroMemory(array);
					array = array2;
				}
				return array;
			}

			private unsafe bool TryEncryptOrDecrypt(SafeNCryptKeyHandle key, ReadOnlySpan<byte> input, Span<byte> output, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* paddingInfo, bool encrypt, out int bytesWritten)
			{
				int bytesNeeded;
				Interop.NCrypt.ErrorCode errorCode = EncryptOrDecrypt(key, input, output, paddingMode, paddingInfo, encrypt, out bytesNeeded);
				switch (errorCode)
				{
				case Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
					bytesWritten = bytesNeeded;
					return true;
				case Interop.NCrypt.ErrorCode.NTE_BUFFER_TOO_SMALL:
					bytesWritten = 0;
					return false;
				default:
					throw errorCode.ToCryptographicException();
				}
			}

			private unsafe static Interop.NCrypt.ErrorCode EncryptOrDecrypt(SafeNCryptKeyHandle key, ReadOnlySpan<byte> input, Span<byte> output, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* paddingInfo, bool encrypt, out int bytesNeeded)
			{
				Interop.NCrypt.ErrorCode errorCode = (encrypt ? Interop.NCrypt.NCryptEncrypt(key, input, input.Length, paddingInfo, output, output.Length, out bytesNeeded, paddingMode) : Interop.NCrypt.NCryptDecrypt(key, input, input.Length, paddingInfo, output, output.Length, out bytesNeeded, paddingMode));
				if (errorCode == Interop.NCrypt.ErrorCode.ERROR_SUCCESS && bytesNeeded > output.Length)
				{
					errorCode = Interop.NCrypt.ErrorCode.NTE_BUFFER_TOO_SMALL;
				}
				return errorCode;
			}

			public unsafe override void ImportParameters(RSAParameters parameters)
			{
				if (parameters.Exponent == null || parameters.Modulus == null)
				{
					throw new CryptographicException(SR.Cryptography_InvalidRsaParameters);
				}
				bool flag;
				if (parameters.D == null)
				{
					flag = false;
					if (parameters.P != null || parameters.DP != null || parameters.Q != null || parameters.DQ != null || parameters.InverseQ != null)
					{
						throw new CryptographicException(SR.Cryptography_InvalidRsaParameters);
					}
				}
				else
				{
					flag = true;
					if (parameters.P == null || parameters.DP == null || parameters.Q == null || parameters.DQ == null || parameters.InverseQ == null)
					{
						throw new CryptographicException(SR.Cryptography_InvalidRsaParameters);
					}
				}
				int num = sizeof(Interop.BCrypt.BCRYPT_RSAKEY_BLOB) + parameters.Exponent.Length + parameters.Modulus.Length;
				if (flag)
				{
					num += parameters.P.Length + parameters.Q.Length;
				}
				byte[] array = new byte[num];
				fixed (byte* ptr = &array[0])
				{
					Interop.BCrypt.BCRYPT_RSAKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_RSAKEY_BLOB*)ptr;
					ptr2->Magic = (flag ? Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC : Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPUBLIC_MAGIC);
					ptr2->BitLength = parameters.Modulus.Length * 8;
					ptr2->cbPublicExp = parameters.Exponent.Length;
					ptr2->cbModulus = parameters.Modulus.Length;
					if (flag)
					{
						ptr2->cbPrime1 = parameters.P.Length;
						ptr2->cbPrime2 = parameters.Q.Length;
					}
					int offset = sizeof(Interop.BCrypt.BCRYPT_RSAKEY_BLOB);
					Interop.BCrypt.Emit(array, ref offset, parameters.Exponent);
					Interop.BCrypt.Emit(array, ref offset, parameters.Modulus);
					if (flag)
					{
						Interop.BCrypt.Emit(array, ref offset, parameters.P);
						Interop.BCrypt.Emit(array, ref offset, parameters.Q);
					}
				}
				ImportKeyBlob(array, flag);
			}

			public override RSAParameters ExportParameters(bool includePrivateParameters)
			{
				byte[] rsaBlob = ExportKeyBlob(includePrivateParameters);
				RSAParameters rsaParams = default(RSAParameters);
				ExportParameters(ref rsaParams, rsaBlob, includePrivateParameters);
				return rsaParams;
			}

			private unsafe static void ExportParameters(ref RSAParameters rsaParams, byte[] rsaBlob, bool includePrivateParameters)
			{
				Interop.BCrypt.KeyBlobMagicNumber magic = (Interop.BCrypt.KeyBlobMagicNumber)BitConverter.ToInt32(rsaBlob, 0);
				CheckMagicValueOfKey(magic, includePrivateParameters);
				if (rsaBlob.Length < sizeof(Interop.BCrypt.BCRYPT_RSAKEY_BLOB))
				{
					throw Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
				}
				fixed (byte* ptr = &rsaBlob[0])
				{
					Interop.BCrypt.BCRYPT_RSAKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_RSAKEY_BLOB*)ptr;
					int offset = sizeof(Interop.BCrypt.BCRYPT_RSAKEY_BLOB);
					rsaParams.Exponent = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPublicExp);
					rsaParams.Modulus = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbModulus);
					if (includePrivateParameters)
					{
						rsaParams.P = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
						rsaParams.Q = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime2);
						rsaParams.DP = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
						rsaParams.DQ = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime2);
						rsaParams.InverseQ = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
						rsaParams.D = Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbModulus);
					}
				}
			}

			private static void CheckMagicValueOfKey(Interop.BCrypt.KeyBlobMagicNumber magic, bool includePrivateParameters)
			{
				if (includePrivateParameters)
				{
					if (magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC && magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAFULLPRIVATE_MAGIC)
					{
						throw new CryptographicException(SR.Cryptography_NotValidPrivateKey);
					}
				}
				else if (magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPUBLIC_MAGIC && magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC && magic != Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAFULLPRIVATE_MAGIC)
				{
					throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
				}
			}

			private static int GetHashSizeInBytes(HashAlgorithmName hashAlgorithm)
			{
				return s_hashSizes.GetOrAdd(hashAlgorithm, delegate(HashAlgorithmName alg)
				{
					using HashProviderCng hashProviderCng = new HashProviderCng(alg.Name, null);
					return hashProviderCng.HashSizeInBytes;
				});
			}

			public unsafe override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
			{
				if (hash == null)
				{
					throw new ArgumentNullException("hash");
				}
				string name = hashAlgorithm.Name;
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
				}
				if (padding == null)
				{
					throw new ArgumentNullException("padding");
				}
				if (hash.Length != GetHashSizeInBytes(hashAlgorithm))
				{
					throw new CryptographicException(SR.Cryptography_SignHash_WrongSize);
				}
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				IntPtr intPtr = Marshal.StringToHGlobalUni(name);
				try
				{
					int estimatedSize = KeySize / 8;
					switch (padding.Mode)
					{
					case RSASignaturePaddingMode.Pkcs1:
					{
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO);
						bCRYPT_PKCS1_PADDING_INFO.pszAlgId = intPtr;
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO2 = bCRYPT_PKCS1_PADDING_INFO;
						return keyHandle.SignHash(hash, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PKCS1_FLAG, &bCRYPT_PKCS1_PADDING_INFO2, estimatedSize);
					}
					case RSASignaturePaddingMode.Pss:
					{
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PSS_PADDING_INFO);
						bCRYPT_PSS_PADDING_INFO.pszAlgId = intPtr;
						bCRYPT_PSS_PADDING_INFO.cbSalt = hash.Length;
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO2 = bCRYPT_PSS_PADDING_INFO;
						return keyHandle.SignHash(hash, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PSS_FLAG, &bCRYPT_PSS_PADDING_INFO2, estimatedSize);
					}
					default:
						throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}

			public unsafe override bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
			{
				string name = hashAlgorithm.Name;
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
				}
				if (padding == null)
				{
					throw new ArgumentNullException("padding");
				}
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				if (hash.Length != GetHashSizeInBytes(hashAlgorithm))
				{
					throw new CryptographicException(SR.Cryptography_SignHash_WrongSize);
				}
				IntPtr intPtr = Marshal.StringToHGlobalUni(name);
				try
				{
					switch (padding.Mode)
					{
					case RSASignaturePaddingMode.Pkcs1:
					{
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO);
						bCRYPT_PKCS1_PADDING_INFO.pszAlgId = intPtr;
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO2 = bCRYPT_PKCS1_PADDING_INFO;
						return keyHandle.TrySignHash(hash, destination, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PKCS1_FLAG, &bCRYPT_PKCS1_PADDING_INFO2, out bytesWritten);
					}
					case RSASignaturePaddingMode.Pss:
					{
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PSS_PADDING_INFO);
						bCRYPT_PSS_PADDING_INFO.pszAlgId = intPtr;
						bCRYPT_PSS_PADDING_INFO.cbSalt = hash.Length;
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO2 = bCRYPT_PSS_PADDING_INFO;
						return keyHandle.TrySignHash(hash, destination, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PSS_FLAG, &bCRYPT_PSS_PADDING_INFO2, out bytesWritten);
					}
					default:
						throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}

			public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
			{
				if (hash == null)
				{
					throw new ArgumentNullException("hash");
				}
				if (signature == null)
				{
					throw new ArgumentNullException("signature");
				}
				return VerifyHash((ReadOnlySpan<byte>)hash, (ReadOnlySpan<byte>)signature, hashAlgorithm, padding);
			}

			public unsafe override bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
			{
				string name = hashAlgorithm.Name;
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
				}
				if (padding == null)
				{
					throw new ArgumentNullException("padding");
				}
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				if (hash.Length != GetHashSizeInBytes(hashAlgorithm))
				{
					return false;
				}
				IntPtr intPtr = Marshal.StringToHGlobalUni(name);
				try
				{
					switch (padding.Mode)
					{
					case RSASignaturePaddingMode.Pkcs1:
					{
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO);
						bCRYPT_PKCS1_PADDING_INFO.pszAlgId = intPtr;
						Interop.BCrypt.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO2 = bCRYPT_PKCS1_PADDING_INFO;
						return keyHandle.VerifyHash(hash, signature, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PKCS1_FLAG, &bCRYPT_PKCS1_PADDING_INFO2);
					}
					case RSASignaturePaddingMode.Pss:
					{
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO = default(Interop.BCrypt.BCRYPT_PSS_PADDING_INFO);
						bCRYPT_PSS_PADDING_INFO.pszAlgId = intPtr;
						bCRYPT_PSS_PADDING_INFO.cbSalt = hash.Length;
						Interop.BCrypt.BCRYPT_PSS_PADDING_INFO bCRYPT_PSS_PADDING_INFO2 = bCRYPT_PSS_PADDING_INFO;
						return keyHandle.VerifyHash(hash, signature, Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PSS_FLAG, &bCRYPT_PSS_PADDING_INFO2);
					}
					default:
						throw new CryptographicException(SR.Cryptography_UnsupportedPaddingMode);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}
	}
}
