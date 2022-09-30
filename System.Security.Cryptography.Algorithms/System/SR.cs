using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.Security.Cryptography.Algorithms;

namespace System
{
	internal static class SR
	{
		private static ResourceManager s_resourceManager;

		private static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(ResourceType));

		internal static Type ResourceType { get; } = typeof(FxResources.System.Security.Cryptography.Algorithms.SR);


		internal static string ArgumentOutOfRange_NeedNonNegNum => GetResourceString("ArgumentOutOfRange_NeedNonNegNum", null);

		internal static string ArgumentOutOfRange_NeedPosNum => GetResourceString("ArgumentOutOfRange_NeedPosNum", null);

		internal static string Argument_InvalidOffLen => GetResourceString("Argument_InvalidOffLen", null);

		internal static string ArgumentNull_Buffer => GetResourceString("ArgumentNull_Buffer", null);

		internal static string Cryptography_ArgECDHKeySizeMismatch => GetResourceString("Cryptography_ArgECDHKeySizeMismatch", null);

		internal static string Cryptography_ArgECDHRequiresECDHKey => GetResourceString("Cryptography_ArgECDHRequiresECDHKey", null);

		internal static string Cryptography_Config_EncodedOIDError => GetResourceString("Cryptography_Config_EncodedOIDError", null);

		internal static string Cryptography_Encryption_MessageTooLong => GetResourceString("Cryptography_Encryption_MessageTooLong", null);

		internal static string Cryptography_ECXmlSerializationFormatRequired => GetResourceString("Cryptography_ECXmlSerializationFormatRequired", null);

		internal static string Cryptography_HashAlgorithmNameNullOrEmpty => GetResourceString("Cryptography_HashAlgorithmNameNullOrEmpty", null);

		internal static string Cryptography_InvalidOID => GetResourceString("Cryptography_InvalidOID", null);

		internal static string Cryptography_CurveNotSupported => GetResourceString("Cryptography_CurveNotSupported", null);

		internal static string Cryptography_InvalidCurveOid => GetResourceString("Cryptography_InvalidCurveOid", null);

		internal static string Cryptography_InvalidCurveKeyParameters => GetResourceString("Cryptography_InvalidCurveKeyParameters", null);

		internal static string Cryptography_InvalidDsaParameters_MissingFields => GetResourceString("Cryptography_InvalidDsaParameters_MissingFields", null);

		internal static string Cryptography_InvalidDsaParameters_MismatchedPGY => GetResourceString("Cryptography_InvalidDsaParameters_MismatchedPGY", null);

		internal static string Cryptography_InvalidDsaParameters_MismatchedQX => GetResourceString("Cryptography_InvalidDsaParameters_MismatchedQX", null);

		internal static string Cryptography_InvalidDsaParameters_MismatchedPJ => GetResourceString("Cryptography_InvalidDsaParameters_MismatchedPJ", null);

		internal static string Cryptography_InvalidDsaParameters_SeedRestriction_ShortKey => GetResourceString("Cryptography_InvalidDsaParameters_SeedRestriction_ShortKey", null);

		internal static string Cryptography_InvalidDsaParameters_QRestriction_ShortKey => GetResourceString("Cryptography_InvalidDsaParameters_QRestriction_ShortKey", null);

		internal static string Cryptography_InvalidDsaParameters_QRestriction_LargeKey => GetResourceString("Cryptography_InvalidDsaParameters_QRestriction_LargeKey", null);

		internal static string Cryptography_InvalidECCharacteristic2Curve => GetResourceString("Cryptography_InvalidECCharacteristic2Curve", null);

		internal static string Cryptography_InvalidECPrimeCurve => GetResourceString("Cryptography_InvalidECPrimeCurve", null);

		internal static string Cryptography_InvalidECNamedCurve => GetResourceString("Cryptography_InvalidECNamedCurve", null);

		internal static string Cryptography_InvalidKeySize => GetResourceString("Cryptography_InvalidKeySize", null);

		internal static string Cryptography_InvalidKey_SemiWeak => GetResourceString("Cryptography_InvalidKey_SemiWeak", null);

		internal static string Cryptography_InvalidKey_Weak => GetResourceString("Cryptography_InvalidKey_Weak", null);

		internal static string Cryptography_InvalidIVSize => GetResourceString("Cryptography_InvalidIVSize", null);

		internal static string Cryptography_InvalidOperation => GetResourceString("Cryptography_InvalidOperation", null);

		internal static string Cryptography_InvalidPadding => GetResourceString("Cryptography_InvalidPadding", null);

		internal static string Cryptography_InvalidRsaParameters => GetResourceString("Cryptography_InvalidRsaParameters", null);

		internal static string Cryptography_KeyTooSmall => GetResourceString("Cryptography_KeyTooSmall", null);

		internal static string Cryptography_MissingIV => GetResourceString("Cryptography_MissingIV", null);

		internal static string Cryptography_MissingKey => GetResourceString("Cryptography_MissingKey", null);

		internal static string Cryptography_MissingOID => GetResourceString("Cryptography_MissingOID", null);

		internal static string Cryptography_MustTransformWholeBlock => GetResourceString("Cryptography_MustTransformWholeBlock", null);

		internal static string Cryptography_NotValidPrivateKey => GetResourceString("Cryptography_NotValidPrivateKey", null);

		internal static string Cryptography_NotValidPublicOrPrivateKey => GetResourceString("Cryptography_NotValidPublicOrPrivateKey", null);

		internal static string Cryptography_PartialBlock => GetResourceString("Cryptography_PartialBlock", null);

		internal static string Cryptography_PasswordDerivedBytes_FewBytesSalt => GetResourceString("Cryptography_PasswordDerivedBytes_FewBytesSalt", null);

		internal static string Cryptography_RC2_EKS40 => GetResourceString("Cryptography_RC2_EKS40", null);

		internal static string Cryptography_RC2_EKSKS => GetResourceString("Cryptography_RC2_EKSKS", null);

		internal static string Cryptography_RC2_EKSKS2 => GetResourceString("Cryptography_RC2_EKSKS2", null);

		internal static string Cryptography_Rijndael_BlockSize => GetResourceString("Cryptography_Rijndael_BlockSize", null);

		internal static string Cryptography_RSA_DecryptWrongSize => GetResourceString("Cryptography_RSA_DecryptWrongSize", null);

		internal static string Cryptography_SignHash_WrongSize => GetResourceString("Cryptography_SignHash_WrongSize", null);

		internal static string Cryptography_TransformBeyondEndOfBuffer => GetResourceString("Cryptography_TransformBeyondEndOfBuffer", null);

		internal static string Cryptography_UnknownHashAlgorithm => GetResourceString("Cryptography_UnknownHashAlgorithm", null);

		internal static string Cryptography_UnknownPaddingMode => GetResourceString("Cryptography_UnknownPaddingMode", null);

		internal static string Cryptography_UnexpectedTransformTruncation => GetResourceString("Cryptography_UnexpectedTransformTruncation", null);

		internal static string Cryptography_UnsupportedPaddingMode => GetResourceString("Cryptography_UnsupportedPaddingMode", null);

		internal static string NotSupported_Method => GetResourceString("NotSupported_Method", null);

		internal static string NotSupported_SubclassOverride => GetResourceString("NotSupported_SubclassOverride", null);

		internal static string Cryptography_AlgorithmTypesMustBeVisible => GetResourceString("Cryptography_AlgorithmTypesMustBeVisible", null);

		internal static string Cryptography_AddNullOrEmptyName => GetResourceString("Cryptography_AddNullOrEmptyName", null);

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = ResourceManager.GetString(resourceKey);
			}
			catch (MissingManifestResourceException)
			{
			}
			if (defaultString != null && resourceKey.Equals(text, StringComparison.Ordinal))
			{
				return defaultString;
			}
			return text;
		}

		internal static string Format(string resourceFormat, object p1)
		{
			if (UsingResourceKeys())
			{
				return string.Join(", ", resourceFormat, p1);
			}
			return string.Format(resourceFormat, p1);
		}
	}
}
