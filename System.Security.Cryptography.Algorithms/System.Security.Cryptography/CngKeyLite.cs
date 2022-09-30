using System.Runtime.InteropServices;
using System.Text;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal static class CngKeyLite
	{
		private static readonly SafeNCryptProviderHandle s_microsoftSoftwareProviderHandle = OpenNCryptProvider("Microsoft Software Key Storage Provider");

		internal static SafeNCryptKeyHandle ImportKeyBlob(string blobType, byte[] keyBlob)
		{
			SafeNCryptKeyHandle phKey;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptImportKey(s_microsoftSoftwareProviderHandle, IntPtr.Zero, blobType, IntPtr.Zero, out phKey, keyBlob, keyBlob.Length, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			SetExportable(phKey);
			return phKey;
		}

		internal static SafeNCryptKeyHandle ImportKeyBlob(string blobType, byte[] keyBlob, string curveName)
		{
			SafeNCryptKeyHandle safeNCryptKeyHandle = ECCng.ImportKeyBlob(blobType, keyBlob, curveName, s_microsoftSoftwareProviderHandle);
			SetExportable(safeNCryptKeyHandle);
			return safeNCryptKeyHandle;
		}

		internal static byte[] ExportKeyBlob(SafeNCryptKeyHandle keyHandle, string blobType)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptExportKey(keyHandle, IntPtr.Zero, blobType, IntPtr.Zero, null, 0, out var pcbResult, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			byte[] array = new byte[pcbResult];
			errorCode = Interop.NCrypt.NCryptExportKey(keyHandle, IntPtr.Zero, blobType, IntPtr.Zero, array, array.Length, out pcbResult, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			Array.Resize(ref array, pcbResult);
			return array;
		}

		internal static SafeNCryptKeyHandle GenerateNewExportableKey(string algorithm, int keySize)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptCreatePersistedKey(s_microsoftSoftwareProviderHandle, out var phKey, algorithm, null, 0, CngKeyCreationOptions.None);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			SetExportable(phKey);
			SetKeyLength(phKey, keySize);
			errorCode = Interop.NCrypt.NCryptFinalizeKey(phKey, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return phKey;
		}

		internal static SafeNCryptKeyHandle GenerateNewExportableKey(string algorithm, string curveName)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptCreatePersistedKey(s_microsoftSoftwareProviderHandle, out var phKey, algorithm, null, 0, CngKeyCreationOptions.None);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			SetExportable(phKey);
			SetCurveName(phKey, curveName);
			errorCode = Interop.NCrypt.NCryptFinalizeKey(phKey, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return phKey;
		}

		internal static SafeNCryptKeyHandle GenerateNewExportableKey(string algorithm, ref ECCurve explicitCurve)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptCreatePersistedKey(s_microsoftSoftwareProviderHandle, out var phKey, algorithm, null, 0, CngKeyCreationOptions.None);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			SetExportable(phKey);
			byte[] primeCurveParameterBlob = ECCng.GetPrimeCurveParameterBlob(ref explicitCurve);
			SetProperty(phKey, "ECCParameters", primeCurveParameterBlob);
			errorCode = Interop.NCrypt.NCryptFinalizeKey(phKey, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return phKey;
		}

		private unsafe static void SetExportable(SafeNCryptKeyHandle keyHandle)
		{
			CngExportPolicies cngExportPolicies = CngExportPolicies.AllowPlaintextExport;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptSetProperty(keyHandle, "Export Policy", &cngExportPolicies, 4, CngPropertyOptions.Persist);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
		}

		private unsafe static void SetKeyLength(SafeNCryptKeyHandle keyHandle, int keySize)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptSetProperty(keyHandle, "Length", &keySize, 4, CngPropertyOptions.Persist);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
		}

		internal static int GetKeyLength(SafeNCryptKeyHandle keyHandle)
		{
			int result = 0;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptGetIntProperty(keyHandle, "PublicKeyLength", ref result);
			if (errorCode != 0)
			{
				errorCode = Interop.NCrypt.NCryptGetIntProperty(keyHandle, "Length", ref result);
			}
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return result;
		}

		private static SafeNCryptProviderHandle OpenNCryptProvider(string providerName)
		{
			SafeNCryptProviderHandle phProvider;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptOpenStorageProvider(out phProvider, providerName, 0);
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			return phProvider;
		}

		private unsafe static byte[] GetProperty(SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, null, 0, out var pcbResult, options);
			switch (errorCode)
			{
			case Interop.NCrypt.ErrorCode.NTE_NOT_FOUND:
				return null;
			default:
				throw errorCode.ToCryptographicException();
			case Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
			{
				byte[] array = new byte[pcbResult];
				fixed (byte* pbOutput = array)
				{
					errorCode = Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, pbOutput, array.Length, out pcbResult, options);
				}
				switch (errorCode)
				{
				case Interop.NCrypt.ErrorCode.NTE_NOT_FOUND:
					return null;
				default:
					throw errorCode.ToCryptographicException();
				case Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
					Array.Resize(ref array, pcbResult);
					return array;
				}
			}
			}
		}

		internal unsafe static string GetPropertyAsString(SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
		{
			byte[] property = GetProperty(ncryptHandle, propertyName, options);
			if (property == null)
			{
				return null;
			}
			if (property.Length == 0)
			{
				return string.Empty;
			}
			fixed (byte* ptr = &property[0])
			{
				return Marshal.PtrToStringUni((IntPtr)ptr);
			}
		}

		internal static string GetCurveName(SafeNCryptHandle ncryptHandle)
		{
			return GetPropertyAsString(ncryptHandle, "ECCCurveName", CngPropertyOptions.None);
		}

		internal static void SetCurveName(SafeNCryptHandle keyHandle, string curveName)
		{
			byte[] array = new byte[(curveName.Length + 1) * 2];
			Encoding.Unicode.GetBytes(curveName, 0, curveName.Length, array, 0);
			SetProperty(keyHandle, "ECCCurveName", array);
		}

		private unsafe static void SetProperty(SafeNCryptHandle ncryptHandle, string propertyName, byte[] value)
		{
			fixed (byte* pbInput = value)
			{
				Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptSetProperty(ncryptHandle, propertyName, pbInput, value.Length, CngPropertyOptions.None);
				if (errorCode != 0)
				{
					throw errorCode.ToCryptographicException();
				}
			}
		}
	}
}
