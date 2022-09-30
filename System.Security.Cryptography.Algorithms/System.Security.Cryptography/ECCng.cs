using System.Runtime.InteropServices;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal static class ECCng
	{
		internal static Interop.BCrypt.ECC_CURVE_ALG_ID_ENUM GetHashAlgorithmId(HashAlgorithmName? name)
		{
			if (!name.HasValue || string.IsNullOrEmpty(name.Value.Name))
			{
				return Interop.BCrypt.ECC_CURVE_ALG_ID_ENUM.BCRYPT_NO_CURVE_GENERATION_ALG_ID;
			}
			Interop.Crypt32.CRYPT_OID_INFO cRYPT_OID_INFO = Interop.Crypt32.FindOidInfo(Interop.Crypt32.CryptOidInfoKeyType.CRYPT_OID_INFO_NAME_KEY, name.Value.Name, OidGroup.HashAlgorithm, fallBackToAllGroups: false);
			if (cRYPT_OID_INFO.AlgId == -1)
			{
				throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, name.Value.Name);
			}
			return (Interop.BCrypt.ECC_CURVE_ALG_ID_ENUM)cRYPT_OID_INFO.AlgId;
		}

		internal static HashAlgorithmName? GetHashAlgorithmName(Interop.BCrypt.ECC_CURVE_ALG_ID_ENUM hashId)
		{
			Interop.Crypt32.CRYPT_OID_INFO cRYPT_OID_INFO = Interop.Crypt32.FindAlgIdOidInfo(hashId);
			if (cRYPT_OID_INFO.AlgId == -1)
			{
				return null;
			}
			return new HashAlgorithmName(cRYPT_OID_INFO.Name);
		}

		internal static bool IsECNamedCurve(string algorithm)
		{
			if (!(algorithm == "ECDH"))
			{
				return algorithm == "ECDSA";
			}
			return true;
		}

		internal static string SpecialNistAlgorithmToCurveName(string algorithm)
		{
			switch (algorithm)
			{
			case "ECDH_P256":
			case "ECDSA_P256":
				return "nistP256";
			case "ECDH_P384":
			case "ECDSA_P384":
				return "nistP384";
			case "ECDH_P521":
			case "ECDSA_P521":
				return "nistP521";
			default:
				throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, algorithm));
			}
		}

		internal unsafe static byte[] GetNamedCurveBlob(ref ECParameters parameters, bool ecdh)
		{
			bool flag = parameters.D != null;
			int num = sizeof(Interop.BCrypt.BCRYPT_ECCKEY_BLOB) + parameters.Q.X.Length + parameters.Q.Y.Length;
			if (flag)
			{
				num += parameters.D.Length;
			}
			byte[] array = new byte[num];
			fixed (byte* ptr = &array[0])
			{
				Interop.BCrypt.BCRYPT_ECCKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_ECCKEY_BLOB*)ptr;
				ptr2->Magic = (ecdh ? EcdhCurveNameToMagicNumber(parameters.Curve.Oid.FriendlyName, flag) : EcdsaCurveNameToMagicNumber(parameters.Curve.Oid.FriendlyName, flag));
				ptr2->cbKey = parameters.Q.X.Length;
				int offset = sizeof(Interop.BCrypt.BCRYPT_ECCKEY_BLOB);
				Interop.BCrypt.Emit(array, ref offset, parameters.Q.X);
				Interop.BCrypt.Emit(array, ref offset, parameters.Q.Y);
				if (flag)
				{
					Interop.BCrypt.Emit(array, ref offset, parameters.D);
				}
			}
			return array;
		}

		internal unsafe static byte[] GetPrimeCurveBlob(ref ECParameters parameters, bool ecdh)
		{
			bool flag = parameters.D != null;
			ECCurve curve = parameters.Curve;
			int num = sizeof(Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB) + curve.Prime.Length + curve.A.Length + curve.B.Length + curve.G.X.Length + curve.G.Y.Length + curve.Order.Length + curve.Cofactor.Length + ((curve.Seed != null) ? curve.Seed.Length : 0) + parameters.Q.X.Length + parameters.Q.Y.Length;
			if (flag)
			{
				num += parameters.D.Length;
			}
			byte[] array = new byte[num];
			fixed (byte* ptr = &array[0])
			{
				Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB*)ptr;
				ptr2->Version = 1;
				ptr2->Magic = ((!flag) ? (ecdh ? Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_GENERIC_MAGIC : Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_GENERIC_MAGIC) : (ecdh ? Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_GENERIC_MAGIC : Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_GENERIC_MAGIC));
				ptr2->cbCofactor = curve.Cofactor.Length;
				ptr2->cbFieldLength = parameters.Q.X.Length;
				ptr2->cbSeed = ((curve.Seed != null) ? curve.Seed.Length : 0);
				ptr2->cbSubgroupOrder = curve.Order.Length;
				ptr2->CurveGenerationAlgId = GetHashAlgorithmId(curve.Hash);
				ptr2->CurveType = ConvertToCurveTypeEnum(curve.CurveType);
				int offset = sizeof(Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB);
				Interop.BCrypt.Emit(array, ref offset, curve.Prime);
				Interop.BCrypt.Emit(array, ref offset, curve.A);
				Interop.BCrypt.Emit(array, ref offset, curve.B);
				Interop.BCrypt.Emit(array, ref offset, curve.G.X);
				Interop.BCrypt.Emit(array, ref offset, curve.G.Y);
				Interop.BCrypt.Emit(array, ref offset, curve.Order);
				Interop.BCrypt.Emit(array, ref offset, curve.Cofactor);
				if (curve.Seed != null)
				{
					Interop.BCrypt.Emit(array, ref offset, curve.Seed);
				}
				Interop.BCrypt.Emit(array, ref offset, parameters.Q.X);
				Interop.BCrypt.Emit(array, ref offset, parameters.Q.Y);
				if (flag)
				{
					Interop.BCrypt.Emit(array, ref offset, parameters.D);
				}
			}
			return array;
		}

		internal unsafe static void ExportNamedCurveParameters(ref ECParameters ecParams, byte[] ecBlob, bool includePrivateParameters)
		{
			Interop.BCrypt.KeyBlobMagicNumber magic = (Interop.BCrypt.KeyBlobMagicNumber)BitConverter.ToInt32(ecBlob, 0);
			CheckMagicValueOfKey(magic, includePrivateParameters);
			if (ecBlob.Length < sizeof(Interop.BCrypt.BCRYPT_ECCKEY_BLOB))
			{
				throw Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
			}
			fixed (byte* ptr = &ecBlob[0])
			{
				Interop.BCrypt.BCRYPT_ECCKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_ECCKEY_BLOB*)ptr;
				int offset = sizeof(Interop.BCrypt.BCRYPT_ECCKEY_BLOB);
				ecParams.Q = new ECPoint
				{
					X = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey),
					Y = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey)
				};
				if (includePrivateParameters)
				{
					ecParams.D = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey);
				}
			}
		}

		internal unsafe static void ExportPrimeCurveParameters(ref ECParameters ecParams, byte[] ecBlob, bool includePrivateParameters)
		{
			Interop.BCrypt.KeyBlobMagicNumber magic = (Interop.BCrypt.KeyBlobMagicNumber)BitConverter.ToInt32(ecBlob, 0);
			CheckMagicValueOfKey(magic, includePrivateParameters);
			if (ecBlob.Length < sizeof(Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB))
			{
				throw Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
			}
			fixed (byte* ptr = &ecBlob[0])
			{
				Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB* ptr2 = (Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB*)ptr;
				ECCurve curve = default(ECCurve);
				curve.CurveType = ConvertToCurveTypeEnum(ptr2->CurveType);
				curve.Hash = GetHashAlgorithmName(ptr2->CurveGenerationAlgId);
				int offset = sizeof(Interop.BCrypt.BCRYPT_ECCFULLKEY_BLOB);
				curve.Prime = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength);
				curve.A = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength);
				curve.B = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength);
				curve.G = new ECPoint
				{
					X = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength),
					Y = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength)
				};
				curve.Order = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbSubgroupOrder);
				curve.Cofactor = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbCofactor);
				curve.Seed = ((ptr2->cbSeed == 0) ? null : Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbSeed));
				ecParams.Q = new ECPoint
				{
					X = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength),
					Y = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbFieldLength)
				};
				if (includePrivateParameters)
				{
					ecParams.D = Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbSubgroupOrder);
				}
				ecParams.Curve = curve;
			}
		}

		internal unsafe static byte[] GetPrimeCurveParameterBlob(ref ECCurve curve)
		{
			int num = sizeof(Interop.BCrypt.BCRYPT_ECC_PARAMETER_HEADER) + curve.Prime.Length + curve.A.Length + curve.B.Length + curve.G.X.Length + curve.G.Y.Length + curve.Order.Length + curve.Cofactor.Length + ((curve.Seed != null) ? curve.Seed.Length : 0);
			byte[] array = new byte[num];
			fixed (byte* ptr = &array[0])
			{
				Interop.BCrypt.BCRYPT_ECC_PARAMETER_HEADER* ptr2 = (Interop.BCrypt.BCRYPT_ECC_PARAMETER_HEADER*)ptr;
				ptr2->Version = 1;
				ptr2->cbCofactor = curve.Cofactor.Length;
				ptr2->cbFieldLength = curve.A.Length;
				ptr2->cbSeed = ((curve.Seed != null) ? curve.Seed.Length : 0);
				ptr2->cbSubgroupOrder = curve.Order.Length;
				ptr2->CurveGenerationAlgId = GetHashAlgorithmId(curve.Hash);
				ptr2->CurveType = ConvertToCurveTypeEnum(curve.CurveType);
				int offset = sizeof(Interop.BCrypt.BCRYPT_ECC_PARAMETER_HEADER);
				Interop.BCrypt.Emit(array, ref offset, curve.Prime);
				Interop.BCrypt.Emit(array, ref offset, curve.A);
				Interop.BCrypt.Emit(array, ref offset, curve.B);
				Interop.BCrypt.Emit(array, ref offset, curve.G.X);
				Interop.BCrypt.Emit(array, ref offset, curve.G.Y);
				Interop.BCrypt.Emit(array, ref offset, curve.Order);
				Interop.BCrypt.Emit(array, ref offset, curve.Cofactor);
				if (curve.Seed != null)
				{
					Interop.BCrypt.Emit(array, ref offset, curve.Seed);
				}
			}
			return array;
		}

		private static void CheckMagicValueOfKey(Interop.BCrypt.KeyBlobMagicNumber magic, bool includePrivateParameters)
		{
			if (includePrivateParameters)
			{
				if (!IsMagicValueOfKeyPrivate(magic))
				{
					throw new CryptographicException(SR.Cryptography_NotValidPrivateKey);
				}
			}
			else if (!IsMagicValueOfKeyPublic(magic))
			{
				throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
			}
		}

		private static bool IsMagicValueOfKeyPrivate(Interop.BCrypt.KeyBlobMagicNumber magic)
		{
			switch (magic)
			{
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P256_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P256_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P384_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P384_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P521_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P521_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_GENERIC_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_GENERIC_MAGIC:
				return true;
			default:
				return false;
			}
		}

		private static bool IsMagicValueOfKeyPublic(Interop.BCrypt.KeyBlobMagicNumber magic)
		{
			switch (magic)
			{
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P256_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P256_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P384_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P384_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P521_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P521_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_GENERIC_MAGIC:
			case Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_GENERIC_MAGIC:
				return true;
			default:
				return IsMagicValueOfKeyPrivate(magic);
			}
		}

		private static Interop.BCrypt.KeyBlobMagicNumber EcdsaCurveNameToMagicNumber(string name, bool includePrivateParameters)
		{
			switch (EcdsaCurveNameToAlgorithm(name))
			{
			case "ECDSA_P256":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P256_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P256_MAGIC;
			case "ECDSA_P384":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P384_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P384_MAGIC;
			case "ECDSA_P521":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_P521_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_P521_MAGIC;
			default:
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PUBLIC_GENERIC_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDSA_PRIVATE_GENERIC_MAGIC;
			}
		}

		private static Interop.BCrypt.KeyBlobMagicNumber EcdhCurveNameToMagicNumber(string name, bool includePrivateParameters)
		{
			switch (EcdhCurveNameToAlgorithm(name))
			{
			case "ECDH_P256":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P256_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P256_MAGIC;
			case "ECDH_P384":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P384_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P384_MAGIC;
			case "ECDH_P521":
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_P521_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_P521_MAGIC;
			default:
				if (!includePrivateParameters)
				{
					return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PUBLIC_GENERIC_MAGIC;
				}
				return Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_ECDH_PRIVATE_GENERIC_MAGIC;
			}
		}

		private static Interop.BCrypt.ECC_CURVE_TYPE_ENUM ConvertToCurveTypeEnum(ECCurve.ECCurveType value)
		{
			return (Interop.BCrypt.ECC_CURVE_TYPE_ENUM)value;
		}

		private static ECCurve.ECCurveType ConvertToCurveTypeEnum(Interop.BCrypt.ECC_CURVE_TYPE_ENUM value)
		{
			return (ECCurve.ECCurveType)value;
		}

		internal static SafeNCryptKeyHandle ImportKeyBlob(string blobType, byte[] keyBlob, string curveName, SafeNCryptProviderHandle provider)
		{
			Interop.NCrypt.ErrorCode errorCode;
			SafeNCryptKeyHandle phKey;
			using (SafeUnicodeStringHandle safeUnicodeStringHandle = new SafeUnicodeStringHandle(curveName))
			{
				Interop.BCrypt.BCryptBufferDesc structure = default(Interop.BCrypt.BCryptBufferDesc);
				Interop.BCrypt.BCryptBuffer structure2 = default(Interop.BCrypt.BCryptBuffer);
				IntPtr intPtr = IntPtr.Zero;
				IntPtr intPtr2 = IntPtr.Zero;
				try
				{
					intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
					intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(structure2));
					structure2.cbBuffer = (curveName.Length + 1) * 2;
					structure2.BufferType = Interop.BCrypt.NCryptBufferDescriptors.NCRYPTBUFFER_ECC_CURVE_NAME;
					structure2.pvBuffer = safeUnicodeStringHandle.DangerousGetHandle();
					Marshal.StructureToPtr(structure2, intPtr2, fDeleteOld: false);
					structure.cBuffers = 1;
					structure.pBuffers = intPtr2;
					structure.ulVersion = 0;
					Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
					errorCode = Interop.NCrypt.NCryptImportKey(provider, IntPtr.Zero, blobType, intPtr, out phKey, keyBlob, keyBlob.Length, 0);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
					Marshal.FreeHGlobal(intPtr2);
				}
			}
			if (errorCode != 0)
			{
				Exception ex = errorCode.ToCryptographicException();
				if (errorCode == Interop.NCrypt.ErrorCode.NTE_INVALID_PARAMETER)
				{
					throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, curveName), ex);
				}
				throw ex;
			}
			return phKey;
		}

		internal static string EcdsaCurveNameToAlgorithm(string algorithm)
		{
			switch (algorithm)
			{
			case "nistP256":
			case "ECDSA_P256":
				return "ECDSA_P256";
			case "nistP384":
			case "ECDSA_P384":
				return "ECDSA_P384";
			case "nistP521":
			case "ECDSA_P521":
				return "ECDSA_P521";
			default:
				return "ECDSA";
			}
		}

		internal static string EcdhCurveNameToAlgorithm(string algorithm)
		{
			switch (algorithm)
			{
			case "nistP256":
			case "ECDH_P256":
				return "ECDH_P256";
			case "nistP384":
			case "ECDH_P384":
				return "ECDH_P384";
			case "nistP521":
			case "ECDH_P521":
				return "ECDH_P521";
			default:
				return "ECDH";
			}
		}
	}
}
