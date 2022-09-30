using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography
{
	internal static class CngCommon
	{
		public static byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			using HashProviderCng hashProviderCng = new HashProviderCng(hashAlgorithm.Name, null);
			hashProviderCng.AppendHashData(data, offset, count);
			return hashProviderCng.FinalizeHashAndReset();
		}

		public static bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			using HashProviderCng hashProviderCng = new HashProviderCng(hashAlgorithm.Name, null);
			if (destination.Length < hashProviderCng.HashSizeInBytes)
			{
				bytesWritten = 0;
				return false;
			}
			hashProviderCng.AppendHashData(source);
			return hashProviderCng.TryFinalizeHashAndReset(destination, out bytesWritten);
		}

		public static byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			using HashProviderCng hashProviderCng = new HashProviderCng(hashAlgorithm.Name, null);
			byte[] array = new byte[4096];
			int count;
			while ((count = data.Read(array, 0, array.Length)) > 0)
			{
				hashProviderCng.AppendHashData(array, 0, count);
			}
			return hashProviderCng.FinalizeHashAndReset();
		}

		public unsafe static byte[] SignHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo, int estimatedSize)
		{
			byte[] array = new byte[estimatedSize];
			int pcbResult;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, hash.Length, array, array.Length, out pcbResult, paddingMode);
			if (errorCode == Interop.NCrypt.ErrorCode.NTE_BUFFER_TOO_SMALL)
			{
				array = new byte[pcbResult];
				errorCode = Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, hash.Length, array, array.Length, out pcbResult, paddingMode);
			}
			if (errorCode != 0)
			{
				throw errorCode.ToCryptographicException();
			}
			Array.Resize(ref array, pcbResult);
			return array;
		}

		public unsafe static bool TrySignHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, Span<byte> signature, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo, out int bytesWritten)
		{
			int pcbResult;
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, hash.Length, signature, signature.Length, out pcbResult, paddingMode);
			switch (errorCode)
			{
			case Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
				bytesWritten = pcbResult;
				return true;
			case Interop.NCrypt.ErrorCode.NTE_BUFFER_TOO_SMALL:
				bytesWritten = 0;
				return false;
			default:
				throw errorCode.ToCryptographicException();
			}
		}

		public unsafe static bool VerifyHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo)
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptVerifySignature(keyHandle, pPaddingInfo, hash, hash.Length, signature, signature.Length, paddingMode);
			return errorCode == Interop.NCrypt.ErrorCode.ERROR_SUCCESS;
		}
	}
}
