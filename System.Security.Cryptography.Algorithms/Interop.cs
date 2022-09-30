using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal class BCrypt
	{
		internal enum NTSTATUS : uint
		{
			STATUS_SUCCESS = 0u,
			STATUS_NOT_FOUND = 3221226021u,
			STATUS_INVALID_PARAMETER = 3221225485u,
			STATUS_NO_MEMORY = 3221225495u
		}

		internal struct BCRYPT_OAEP_PADDING_INFO
		{
			internal IntPtr pszAlgId;

			internal IntPtr pbLabel;

			internal int cbLabel;
		}

		internal struct BCRYPT_PKCS1_PADDING_INFO
		{
			internal IntPtr pszAlgId;
		}

		internal struct BCRYPT_PSS_PADDING_INFO
		{
			internal IntPtr pszAlgId;

			internal int cbSalt;
		}

		internal enum KeyBlobMagicNumber
		{
			BCRYPT_DSA_PUBLIC_MAGIC = 1112560452,
			BCRYPT_DSA_PRIVATE_MAGIC = 1448104772,
			BCRYPT_DSA_PUBLIC_MAGIC_V2 = 843206724,
			BCRYPT_DSA_PRIVATE_MAGIC_V2 = 844517444,
			BCRYPT_ECDH_PUBLIC_P256_MAGIC = 827016005,
			BCRYPT_ECDH_PRIVATE_P256_MAGIC = 843793221,
			BCRYPT_ECDH_PUBLIC_P384_MAGIC = 860570437,
			BCRYPT_ECDH_PRIVATE_P384_MAGIC = 877347653,
			BCRYPT_ECDH_PUBLIC_P521_MAGIC = 894124869,
			BCRYPT_ECDH_PRIVATE_P521_MAGIC = 910902085,
			BCRYPT_ECDH_PUBLIC_GENERIC_MAGIC = 1347109701,
			BCRYPT_ECDH_PRIVATE_GENERIC_MAGIC = 1447772997,
			BCRYPT_ECDSA_PUBLIC_P256_MAGIC = 827540293,
			BCRYPT_ECDSA_PRIVATE_P256_MAGIC = 844317509,
			BCRYPT_ECDSA_PUBLIC_P384_MAGIC = 861094725,
			BCRYPT_ECDSA_PRIVATE_P384_MAGIC = 877871941,
			BCRYPT_ECDSA_PUBLIC_P521_MAGIC = 894649157,
			BCRYPT_ECDSA_PRIVATE_P521_MAGIC = 911426373,
			BCRYPT_ECDSA_PUBLIC_GENERIC_MAGIC = 1346650949,
			BCRYPT_ECDSA_PRIVATE_GENERIC_MAGIC = 1447314245,
			BCRYPT_RSAPUBLIC_MAGIC = 826364754,
			BCRYPT_RSAPRIVATE_MAGIC = 843141970,
			BCRYPT_RSAFULLPRIVATE_MAGIC = 859919186,
			BCRYPT_KEY_DATA_BLOB_MAGIC = 1296188491
		}

		internal struct BCRYPT_RSAKEY_BLOB
		{
			internal KeyBlobMagicNumber Magic;

			internal int BitLength;

			internal int cbPublicExp;

			internal int cbModulus;

			internal int cbPrime1;

			internal int cbPrime2;
		}

		internal struct BCRYPT_DSA_KEY_BLOB
		{
			internal KeyBlobMagicNumber Magic;

			internal int cbKey;

			internal unsafe fixed byte Count[4];

			internal unsafe fixed byte Seed[20];

			internal unsafe fixed byte q[20];
		}

		internal struct BCRYPT_DSA_KEY_BLOB_V2
		{
			internal KeyBlobMagicNumber Magic;

			internal int cbKey;

			internal HASHALGORITHM_ENUM hashAlgorithm;

			internal DSAFIPSVERSION_ENUM standardVersion;

			internal int cbSeedLength;

			internal int cbGroupSize;

			internal unsafe fixed byte Count[4];
		}

		public enum HASHALGORITHM_ENUM
		{
			DSA_HASH_ALGORITHM_SHA1,
			DSA_HASH_ALGORITHM_SHA256,
			DSA_HASH_ALGORITHM_SHA512
		}

		public enum DSAFIPSVERSION_ENUM
		{
			DSA_FIPS186_2,
			DSA_FIPS186_3
		}

		internal struct BCRYPT_ECCKEY_BLOB
		{
			internal KeyBlobMagicNumber Magic;

			internal int cbKey;
		}

		internal enum ECC_CURVE_TYPE_ENUM
		{
			BCRYPT_ECC_PRIME_SHORT_WEIERSTRASS_CURVE = 1,
			BCRYPT_ECC_PRIME_TWISTED_EDWARDS_CURVE,
			BCRYPT_ECC_PRIME_MONTGOMERY_CURVE
		}

		internal enum ECC_CURVE_ALG_ID_ENUM
		{
			BCRYPT_NO_CURVE_GENERATION_ALG_ID
		}

		internal struct BCRYPT_ECCFULLKEY_BLOB
		{
			internal KeyBlobMagicNumber Magic;

			internal int Version;

			internal ECC_CURVE_TYPE_ENUM CurveType;

			internal ECC_CURVE_ALG_ID_ENUM CurveGenerationAlgId;

			internal int cbFieldLength;

			internal int cbSubgroupOrder;

			internal int cbCofactor;

			internal int cbSeed;
		}

		internal enum NCryptBufferDescriptors
		{
			NCRYPTBUFFER_ECC_CURVE_NAME = 60
		}

		internal struct BCryptBuffer
		{
			internal int cbBuffer;

			internal NCryptBufferDescriptors BufferType;

			internal IntPtr pvBuffer;
		}

		internal struct BCryptBufferDesc
		{
			internal int ulVersion;

			internal int cBuffers;

			internal IntPtr pBuffers;
		}

		internal struct BCRYPT_ECC_PARAMETER_HEADER
		{
			internal int Version;

			internal ECC_CURVE_TYPE_ENUM CurveType;

			internal ECC_CURVE_ALG_ID_ENUM CurveGenerationAlgId;

			internal int cbFieldLength;

			internal int cbSubgroupOrder;

			internal int cbCofactor;

			internal int cbSeed;
		}

		[Flags]
		internal enum BCryptOpenAlgorithmProviderFlags
		{
			None = 0,
			BCRYPT_ALG_HANDLE_HMAC_FLAG = 8
		}

		[Flags]
		internal enum BCryptCreateHashFlags
		{
			None = 0,
			BCRYPT_HASH_REUSABLE_FLAG = 0x20
		}

		internal static class BCryptAlgorithmCache
		{
			private struct Entry
			{
				public string HashAlgorithmId { get; private set; }

				public BCryptOpenAlgorithmProviderFlags Flags { get; private set; }

				public SafeBCryptAlgorithmHandle Handle { get; private set; }

				public Entry(string hashAlgorithmId, BCryptOpenAlgorithmProviderFlags flags, SafeBCryptAlgorithmHandle handle)
				{
					this = default(Entry);
					HashAlgorithmId = hashAlgorithmId;
					Flags = flags;
					Handle = handle;
				}
			}

			private static volatile Entry[] _cache = new Entry[0];

			public static SafeBCryptAlgorithmHandle GetCachedBCryptAlgorithmHandle(string hashAlgorithmId, BCryptOpenAlgorithmProviderFlags flags)
			{
				Entry[] cache = _cache;
				Entry[] array = cache;
				for (int i = 0; i < array.Length; i++)
				{
					Entry entry = array[i];
					if (entry.HashAlgorithmId == hashAlgorithmId && entry.Flags == flags)
					{
						return entry.Handle;
					}
				}
				SafeBCryptAlgorithmHandle phAlgorithm;
				NTSTATUS nTSTATUS = BCryptOpenAlgorithmProvider(out phAlgorithm, hashAlgorithmId, null, flags);
				if (nTSTATUS != 0)
				{
					throw CreateCryptographicException(nTSTATUS);
				}
				Entry[] array2 = new Entry[cache.Length + 1];
				Entry entry2 = new Entry(hashAlgorithmId, flags, phAlgorithm);
				Array.Copy(cache, 0, array2, 0, cache.Length);
				array2[array2.Length - 1] = entry2;
				_cache = array2;
				return entry2.Handle;
			}
		}

		internal static NTSTATUS BCryptGenRandom(ref byte pbBuffer, int count)
		{
			return BCryptGenRandom(IntPtr.Zero, ref pbBuffer, count, 2);
		}

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		private static extern NTSTATUS BCryptGenRandom(IntPtr hAlgorithm, ref byte pbBuffer, int cbBuffer, int dwFlags);

		internal static void Emit(byte[] blob, ref int offset, byte[] value)
		{
			Buffer.BlockCopy(value, 0, blob, offset, value.Length);
			offset += value.Length;
		}

		internal static void EmitByte(byte[] blob, ref int offset, byte value, int count = 1)
		{
			int num = offset + count;
			for (int i = offset; i < num; i++)
			{
				blob[i] = value;
			}
			offset = num;
		}

		internal static void EmitBigEndian(byte[] blob, ref int offset, int value)
		{
			blob[offset++] = (byte)(value >> 24);
			blob[offset++] = (byte)(value >> 16);
			blob[offset++] = (byte)(value >> 8);
			blob[offset++] = (byte)value;
		}

		internal static byte[] Consume(byte[] blob, ref int offset, int count)
		{
			byte[] array = new byte[count];
			Buffer.BlockCopy(blob, offset, array, 0, count);
			offset += count;
			return array;
		}

		internal static Exception CreateCryptographicException(NTSTATUS ntStatus)
		{
			int hr = (int)(ntStatus | (NTSTATUS)16777216u);
			return hr.ToCryptographicException();
		}

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern NTSTATUS BCryptOpenAlgorithmProvider(out SafeBCryptAlgorithmHandle phAlgorithm, string pszAlgId, string pszImplementation, BCryptOpenAlgorithmProviderFlags dwFlags);

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern NTSTATUS BCryptCloseAlgorithmProvider(IntPtr hAlgorithm, int dwFlags);

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern NTSTATUS BCryptDestroyHash(IntPtr hHash);

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern NTSTATUS BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm, out SafeBCryptHashHandle phHash, IntPtr pbHashObject, int cbHashObject, [In][Out] byte[] pbSecret, int cbSecret, BCryptCreateHashFlags dwFlags);

		internal static NTSTATUS BCryptHashData(SafeBCryptHashHandle hHash, ReadOnlySpan<byte> pbInput, int cbInput, int dwFlags)
		{
			return BCryptHashData(hHash, ref MemoryMarshal.GetReference(pbInput), cbInput, dwFlags);
		}

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		private static extern NTSTATUS BCryptHashData(SafeBCryptHashHandle hHash, ref byte pbInput, int cbInput, int dwFlags);

		internal static NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, Span<byte> pbOutput, int cbOutput, int dwFlags)
		{
			return BCryptFinishHash(hHash, ref MemoryMarshal.GetReference(pbOutput), cbOutput, dwFlags);
		}

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		private static extern NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, ref byte pbOutput, int cbOutput, int dwFlags);

		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern NTSTATUS BCryptGetProperty(SafeBCryptHandle hObject, string pszProperty, void* pbOutput, int cbOutput, out int pcbResult, int dwFlags);
	}

	internal static class NCrypt
	{
		[Flags]
		internal enum SecretAgreementFlags
		{
			None = 0,
			UseSecretAsHmacKey = 1
		}

		internal enum BufferType
		{
			KdfHashAlgorithm,
			KdfSecretPrepend,
			KdfSecretAppend,
			KdfHmacKey,
			KdfTlsLabel,
			KdfTlsSeed
		}

		internal struct NCryptBuffer
		{
			public int cbBuffer;

			public BufferType BufferType;

			public IntPtr pvBuffer;
		}

		internal struct NCryptBufferDesc
		{
			public int ulVersion;

			public int cBuffers;

			public IntPtr pBuffers;
		}

		internal enum AsymmetricPaddingMode
		{
			None = 0,
			NCRYPT_NO_PADDING_FLAG = 1,
			NCRYPT_PAD_PKCS1_FLAG = 2,
			NCRYPT_PAD_OAEP_FLAG = 4,
			NCRYPT_PAD_PSS_FLAG = 8
		}

		internal enum ErrorCode
		{
			ERROR_SUCCESS = 0,
			NTE_BAD_SIGNATURE = -2146893818,
			NTE_NOT_FOUND = -2146893807,
			NTE_BAD_KEYSET = -2146893802,
			NTE_INVALID_PARAMETER = -2146893785,
			NTE_BUFFER_TOO_SMALL = -2146893784,
			NTE_NOT_SUPPORTED = -2146893783,
			NTE_NO_MORE_ITEMS = -2146893782,
			E_FAIL = -2147467259
		}

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		private static extern ErrorCode NCryptDeriveKey(SafeNCryptSecretHandle hSharedSecret, string pwszKDF, [In] ref NCryptBufferDesc pParameterList, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] pbDerivedKey, int cbDerivedKey, out int pcbResult, SecretAgreementFlags dwFlags);

		private unsafe static byte[] DeriveKeyMaterial(SafeNCryptSecretHandle secretAgreement, string kdf, string hashAlgorithm, byte[] hmacKey, byte[] secretPrepend, byte[] secretAppend, SecretAgreementFlags flags)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.StringToCoTaskMemUni(hashAlgorithm);
				Span<NCryptBuffer> span = stackalloc NCryptBuffer[4];
				int num = 0;
				NCryptBuffer nCryptBuffer = default(NCryptBuffer);
				nCryptBuffer.cbBuffer = (hashAlgorithm.Length + 1) * 2;
				nCryptBuffer.BufferType = BufferType.KdfHashAlgorithm;
				nCryptBuffer.pvBuffer = intPtr;
				span[num] = nCryptBuffer;
				num++;
				fixed (byte* ptr = hmacKey)
				{
					fixed (byte* ptr2 = secretPrepend)
					{
						fixed (byte* ptr3 = secretAppend)
						{
							if (ptr != null)
							{
								NCryptBuffer nCryptBuffer2 = default(NCryptBuffer);
								nCryptBuffer2.cbBuffer = hmacKey.Length;
								nCryptBuffer2.BufferType = BufferType.KdfHmacKey;
								nCryptBuffer2.pvBuffer = new IntPtr(ptr);
								span[num] = nCryptBuffer2;
								num++;
							}
							if (ptr2 != null)
							{
								NCryptBuffer nCryptBuffer3 = default(NCryptBuffer);
								nCryptBuffer3.cbBuffer = secretPrepend.Length;
								nCryptBuffer3.BufferType = BufferType.KdfSecretPrepend;
								nCryptBuffer3.pvBuffer = new IntPtr(ptr2);
								span[num] = nCryptBuffer3;
								num++;
							}
							if (ptr3 != null)
							{
								NCryptBuffer nCryptBuffer4 = default(NCryptBuffer);
								nCryptBuffer4.cbBuffer = secretAppend.Length;
								nCryptBuffer4.BufferType = BufferType.KdfSecretAppend;
								nCryptBuffer4.pvBuffer = new IntPtr(ptr3);
								span[num] = nCryptBuffer4;
								num++;
							}
							return DeriveKeyMaterial(secretAgreement, kdf, span.Slice(0, num), flags);
						}
					}
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
		}

		private unsafe static byte[] DeriveKeyMaterial(SafeNCryptSecretHandle secretAgreement, string kdf, ReadOnlySpan<NCryptBuffer> parameters, SecretAgreementFlags flags)
		{
			fixed (NCryptBuffer* value = &MemoryMarshal.GetReference(parameters))
			{
				NCryptBufferDesc pParameterList = default(NCryptBufferDesc);
				pParameterList.ulVersion = 0;
				pParameterList.cBuffers = parameters.Length;
				pParameterList.pBuffers = new IntPtr(value);
				ErrorCode errorCode = NCryptDeriveKey(secretAgreement, kdf, ref pParameterList, null, 0, out var pcbResult, flags);
				if (errorCode != 0 && errorCode != ErrorCode.NTE_BUFFER_TOO_SMALL)
				{
					throw errorCode.ToCryptographicException();
				}
				byte[] array = new byte[pcbResult];
				errorCode = NCryptDeriveKey(secretAgreement, kdf, ref pParameterList, array, array.Length, out pcbResult, flags);
				if (errorCode != 0)
				{
					throw errorCode.ToCryptographicException();
				}
				Array.Resize(ref array, Math.Min(pcbResult, array.Length));
				return array;
			}
		}

		internal static byte[] DeriveKeyMaterialHash(SafeNCryptSecretHandle secretAgreement, string hashAlgorithm, byte[] secretPrepend, byte[] secretAppend, SecretAgreementFlags flags)
		{
			return DeriveKeyMaterial(secretAgreement, "HASH", hashAlgorithm, null, secretPrepend, secretAppend, flags);
		}

		internal static byte[] DeriveKeyMaterialHmac(SafeNCryptSecretHandle secretAgreement, string hashAlgorithm, byte[] hmacKey, byte[] secretPrepend, byte[] secretAppend, SecretAgreementFlags flags)
		{
			return DeriveKeyMaterial(secretAgreement, "HMAC", hashAlgorithm, hmacKey, secretPrepend, secretAppend, flags);
		}

		internal unsafe static byte[] DeriveKeyMaterialTls(SafeNCryptSecretHandle secretAgreement, byte[] label, byte[] seed, SecretAgreementFlags flags)
		{
			Span<NCryptBuffer> span = stackalloc NCryptBuffer[2];
			fixed (byte* value = label)
			{
				fixed (byte* value2 = seed)
				{
					NCryptBuffer nCryptBuffer = default(NCryptBuffer);
					nCryptBuffer.cbBuffer = label.Length;
					nCryptBuffer.BufferType = BufferType.KdfTlsLabel;
					nCryptBuffer.pvBuffer = new IntPtr(value);
					span[0] = nCryptBuffer;
					NCryptBuffer nCryptBuffer2 = default(NCryptBuffer);
					nCryptBuffer2.cbBuffer = seed.Length;
					nCryptBuffer2.BufferType = BufferType.KdfTlsSeed;
					nCryptBuffer2.pvBuffer = new IntPtr(value2);
					span[1] = nCryptBuffer2;
					return DeriveKeyMaterial(secretAgreement, "TLS_PRF", span, flags);
				}
			}
		}

		[DllImport("ncrypt.dll")]
		private static extern ErrorCode NCryptSecretAgreement(SafeNCryptKeyHandle hPrivKey, SafeNCryptKeyHandle hPubKey, out SafeNCryptSecretHandle phSecret, int dwFlags);

		internal static SafeNCryptSecretHandle DeriveSecretAgreement(SafeNCryptKeyHandle privateKey, SafeNCryptKeyHandle otherPartyPublicKey)
		{
			SafeNCryptSecretHandle phSecret;
			ErrorCode errorCode = NCryptSecretAgreement(privateKey, otherPartyPublicKey, out phSecret, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return phSecret;
		}

		internal unsafe static ErrorCode NCryptEncrypt(SafeNCryptKeyHandle hKey, ReadOnlySpan<byte> pbInput, int cbInput, void* pPaddingInfo, Span<byte> pbOutput, int cbOutput, out int pcbResult, AsymmetricPaddingMode dwFlags)
		{
			return NCryptEncrypt(hKey, ref MemoryMarshal.GetReference(pbInput), cbInput, pPaddingInfo, ref MemoryMarshal.GetReference(pbOutput), cbOutput, out pcbResult, dwFlags);
		}

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		private unsafe static extern ErrorCode NCryptEncrypt(SafeNCryptKeyHandle hKey, ref byte pbInput, int cbInput, void* pPaddingInfo, ref byte pbOutput, int cbOutput, out int pcbResult, AsymmetricPaddingMode dwFlags);

		internal unsafe static ErrorCode NCryptDecrypt(SafeNCryptKeyHandle hKey, ReadOnlySpan<byte> pbInput, int cbInput, void* pPaddingInfo, Span<byte> pbOutput, int cbOutput, out int pcbResult, AsymmetricPaddingMode dwFlags)
		{
			return NCryptDecrypt(hKey, ref MemoryMarshal.GetReference(pbInput), cbInput, pPaddingInfo, ref MemoryMarshal.GetReference(pbOutput), cbOutput, out pcbResult, dwFlags);
		}

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		private unsafe static extern ErrorCode NCryptDecrypt(SafeNCryptKeyHandle hKey, ref byte pbInput, int cbInput, void* pPaddingInfo, ref byte pbOutput, int cbOutput, out int pcbResult, AsymmetricPaddingMode dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptImportKey(SafeNCryptProviderHandle hProvider, IntPtr hImportKey, string pszBlobType, IntPtr pParameterList, out SafeNCryptKeyHandle phKey, [In] byte[] pbData, int cbData, int dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptExportKey(SafeNCryptKeyHandle hKey, IntPtr hExportKey, string pszBlobType, IntPtr pParameterList, [Out] byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptCreatePersistedKey(SafeNCryptProviderHandle hProvider, out SafeNCryptKeyHandle phKey, string pszAlgId, string pszKeyName, int dwLegacyKeySpec, CngKeyCreationOptions dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptFinalizeKey(SafeNCryptKeyHandle hKey, int dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptFreeObject(IntPtr hObject);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern ErrorCode NCryptOpenStorageProvider(out SafeNCryptProviderHandle phProvider, string pszProviderName, int dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern ErrorCode NCryptGetProperty(SafeNCryptHandle hObject, string pszProperty, [Out] void* pbOutput, int cbOutput, out int pcbResult, CngPropertyOptions dwFlags);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern ErrorCode NCryptSetProperty(SafeNCryptHandle hObject, string pszProperty, [In] void* pbInput, int cbInput, CngPropertyOptions dwFlags);

		internal unsafe static ErrorCode NCryptGetIntProperty(SafeNCryptHandle hObject, string pszProperty, ref int result)
		{
			ErrorCode result2;
			fixed (int* pbOutput = &result)
			{
				result2 = NCryptGetProperty(hObject, pszProperty, pbOutput, 4, out var _, CngPropertyOptions.None);
			}
			return result2;
		}

		internal unsafe static ErrorCode NCryptSignHash(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ReadOnlySpan<byte> pbHashValue, int cbHashValue, Span<byte> pbSignature, int cbSignature, out int pcbResult, AsymmetricPaddingMode dwFlags)
		{
			return NCryptSignHash(hKey, pPaddingInfo, ref MemoryMarshal.GetReference(pbHashValue), cbHashValue, ref MemoryMarshal.GetReference(pbSignature), cbSignature, out pcbResult, dwFlags);
		}

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		private unsafe static extern ErrorCode NCryptSignHash(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ref byte pbHashValue, int cbHashValue, ref byte pbSignature, int cbSignature, out int pcbResult, AsymmetricPaddingMode dwFlags);

		internal unsafe static ErrorCode NCryptVerifySignature(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ReadOnlySpan<byte> pbHashValue, int cbHashValue, ReadOnlySpan<byte> pbSignature, int cbSignature, AsymmetricPaddingMode dwFlags)
		{
			return NCryptVerifySignature(hKey, pPaddingInfo, ref MemoryMarshal.GetReference(pbHashValue), cbHashValue, ref MemoryMarshal.GetReference(pbSignature), cbSignature, dwFlags);
		}

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		private unsafe static extern ErrorCode NCryptVerifySignature(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ref byte pbHashValue, int cbHashValue, ref byte pbSignature, int cbSignature, AsymmetricPaddingMode dwFlags);
	}

	internal static class Crypt32
	{
		internal struct CRYPT_OID_INFO
		{
			public int cbSize;

			public IntPtr pszOID;

			public IntPtr pwszName;

			public OidGroup dwGroupId;

			public int AlgId;

			public int cbData;

			public IntPtr pbData;

			public string Name => Marshal.PtrToStringUni(pwszName);
		}

		internal enum CryptOidInfoKeyType
		{
			CRYPT_OID_INFO_OID_KEY = 1,
			CRYPT_OID_INFO_NAME_KEY,
			CRYPT_OID_INFO_ALGID_KEY,
			CRYPT_OID_INFO_SIGN_KEY,
			CRYPT_OID_INFO_CNG_ALGID_KEY,
			CRYPT_OID_INFO_CNG_SIGN_KEY
		}

		internal static CRYPT_OID_INFO FindOidInfo(CryptOidInfoKeyType keyType, string key, OidGroup group, bool fallBackToAllGroups)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = keyType switch
				{
					CryptOidInfoKeyType.CRYPT_OID_INFO_OID_KEY => Marshal.StringToCoTaskMemAnsi(key), 
					CryptOidInfoKeyType.CRYPT_OID_INFO_NAME_KEY => Marshal.StringToCoTaskMemUni(key), 
					_ => throw new NotSupportedException(), 
				};
				if (!OidGroupWillNotUseActiveDirectory(group))
				{
					OidGroup group2 = group | (OidGroup)(-2147483648);
					IntPtr intPtr2 = CryptFindOIDInfo(keyType, intPtr, group2);
					if (intPtr2 != IntPtr.Zero)
					{
						return Marshal.PtrToStructure<CRYPT_OID_INFO>(intPtr2);
					}
				}
				IntPtr intPtr3 = CryptFindOIDInfo(keyType, intPtr, group);
				if (intPtr3 != IntPtr.Zero)
				{
					return Marshal.PtrToStructure<CRYPT_OID_INFO>(intPtr3);
				}
				if (fallBackToAllGroups && group != 0)
				{
					IntPtr intPtr4 = CryptFindOIDInfo(keyType, intPtr, OidGroup.All);
					if (intPtr4 != IntPtr.Zero)
					{
						return Marshal.PtrToStructure<CRYPT_OID_INFO>(intPtr4);
					}
				}
				CRYPT_OID_INFO result = default(CRYPT_OID_INFO);
				result.AlgId = -1;
				return result;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
		}

		private static bool OidGroupWillNotUseActiveDirectory(OidGroup group)
		{
			if (group != OidGroup.HashAlgorithm && group != OidGroup.EncryptionAlgorithm && group != OidGroup.PublicKeyAlgorithm && group != OidGroup.SignatureAlgorithm && group != OidGroup.Attribute && group != OidGroup.ExtensionOrAttribute)
			{
				return group == OidGroup.KeyDerivationFunction;
			}
			return true;
		}

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr CryptFindOIDInfo(CryptOidInfoKeyType dwKeyType, IntPtr pvKey, OidGroup group);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr CryptFindOIDInfo(CryptOidInfoKeyType dwKeyType, ref int pvKey, OidGroup group);

		public static CRYPT_OID_INFO FindAlgIdOidInfo(BCrypt.ECC_CURVE_ALG_ID_ENUM algId)
		{
			int pvKey = (int)algId;
			IntPtr intPtr = CryptFindOIDInfo(CryptOidInfoKeyType.CRYPT_OID_INFO_ALGID_KEY, ref pvKey, OidGroup.HashAlgorithm);
			if (intPtr != IntPtr.Zero)
			{
				return Marshal.PtrToStructure<CRYPT_OID_INFO>(intPtr);
			}
			CRYPT_OID_INFO result = default(CRYPT_OID_INFO);
			result.AlgId = -1;
			return result;
		}
	}

	internal class Kernel32
	{
		[DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true)]
		private unsafe static extern int FormatMessage(int dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId, char* lpBuffer, int nSize, IntPtr[] arguments);

		internal static string GetMessage(int errorCode)
		{
			return GetMessage(IntPtr.Zero, errorCode);
		}

		internal static string GetMessage(IntPtr moduleHandle, int errorCode)
		{
			Span<char> buffer = stackalloc char[256];
			do
			{
				if (TryGetErrorMessage(moduleHandle, errorCode, buffer, out var errorMsg))
				{
					return errorMsg;
				}
				buffer = new char[buffer.Length * 4];
			}
			while (buffer.Length < 66560);
			return $"Unknown error (0x{errorCode:x})";
		}

		private unsafe static bool TryGetErrorMessage(IntPtr moduleHandle, int errorCode, Span<char> buffer, out string errorMsg)
		{
			int num = 12800;
			if (moduleHandle != IntPtr.Zero)
			{
				num |= 0x800;
			}
			int num2;
			fixed (char* lpBuffer = &MemoryMarshal.GetReference(buffer))
			{
				num2 = FormatMessage(num, moduleHandle, (uint)errorCode, 0, lpBuffer, buffer.Length, null);
			}
			if (num2 != 0)
			{
				int num3;
				for (num3 = num2; num3 > 0; num3--)
				{
					char c = buffer[num3 - 1];
					if (c > ' ' && c != '.')
					{
						break;
					}
				}
				errorMsg = buffer.Slice(0, num3).ToString();
			}
			else
			{
				if (Marshal.GetLastWin32Error() == 122)
				{
					errorMsg = "";
					return false;
				}
				errorMsg = $"Unknown error (0x{errorCode:x})";
			}
			return true;
		}
	}
}
