using System.IO;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal static class ECDsaImplementation
	{
		public sealed class ECDsaCng : ECDsa
		{
			private readonly ECCngKey _key = new ECCngKey("ECDSA");

			public override int KeySize
			{
				get
				{
					return base.KeySize;
				}
				set
				{
					if (KeySize != value)
					{
						base.KeySize = value;
						DisposeKey();
					}
				}
			}

			public override KeySizes[] LegalKeySizes => new KeySizes[2]
			{
				new KeySizes(256, 384, 128),
				new KeySizes(521, 521, 0)
			};

			private void ImportFullKeyBlob(byte[] ecfullKeyBlob, bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "ECCFULLPRIVATEBLOB" : "ECCFULLPUBLICBLOB");
				SafeNCryptKeyHandle keyHandle = CngKeyLite.ImportKeyBlob(blobType, ecfullKeyBlob);
				_key.SetHandle(keyHandle, "ECDSA");
				ForceSetKeySize(_key.KeySize);
			}

			private void ImportKeyBlob(byte[] ecKeyBlob, string curveName, bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "ECCPRIVATEBLOB" : "ECCPUBLICBLOB");
				SafeNCryptKeyHandle keyHandle = CngKeyLite.ImportKeyBlob(blobType, ecKeyBlob, curveName);
				_key.SetHandle(keyHandle, ECCng.EcdsaCurveNameToAlgorithm(curveName));
				ForceSetKeySize(_key.KeySize);
			}

			private byte[] ExportKeyBlob(bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "ECCPRIVATEBLOB" : "ECCPUBLICBLOB");
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return CngKeyLite.ExportKeyBlob(keyHandle, blobType);
			}

			private byte[] ExportFullKeyBlob(bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "ECCFULLPRIVATEBLOB" : "ECCFULLPUBLICBLOB");
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return CngKeyLite.ExportKeyBlob(keyHandle, blobType);
			}

			private string GetCurveName()
			{
				return _key.GetCurveName(KeySize);
			}

			public override void GenerateKey(ECCurve curve)
			{
				_key.GenerateKey(curve);
				ForceSetKeySize(_key.KeySize);
			}

			private SafeNCryptKeyHandle GetDuplicatedKeyHandle()
			{
				return _key.GetDuplicatedKeyHandle(KeySize);
			}

			private void DisposeKey()
			{
				_key.DisposeKey();
			}

			public ECDsaCng(ECCurve curve)
			{
				GenerateKey(curve);
			}

			public ECDsaCng()
				: this(521)
			{
			}

			public ECDsaCng(int keySize)
			{
				KeySize = keySize;
			}

			private void ForceSetKeySize(int newKeySize)
			{
				KeySizeValue = newKeySize;
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

			public override void ImportParameters(ECParameters parameters)
			{
				parameters.Validate();
				ECCurve curve = parameters.Curve;
				bool includePrivateParameters = parameters.D != null;
				if (curve.IsPrime)
				{
					byte[] primeCurveBlob = ECCng.GetPrimeCurveBlob(ref parameters, ecdh: false);
					ImportFullKeyBlob(primeCurveBlob, includePrivateParameters);
					return;
				}
				if (curve.IsNamed)
				{
					if (string.IsNullOrEmpty(curve.Oid.FriendlyName))
					{
						throw new PlatformNotSupportedException(string.Format(SR.Cryptography_InvalidCurveOid, curve.Oid.Value.ToString()));
					}
					byte[] namedCurveBlob = ECCng.GetNamedCurveBlob(ref parameters, ecdh: false);
					ImportKeyBlob(namedCurveBlob, curve.Oid.FriendlyName, includePrivateParameters);
					return;
				}
				throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, curve.CurveType.ToString()));
			}

			public override ECParameters ExportExplicitParameters(bool includePrivateParameters)
			{
				byte[] ecBlob = ExportFullKeyBlob(includePrivateParameters);
				ECParameters ecParams = default(ECParameters);
				ECCng.ExportPrimeCurveParameters(ref ecParams, ecBlob, includePrivateParameters);
				return ecParams;
			}

			public override ECParameters ExportParameters(bool includePrivateParameters)
			{
				ECParameters ecParams = default(ECParameters);
				string curveName = GetCurveName();
				if (string.IsNullOrEmpty(curveName))
				{
					byte[] ecBlob = ExportFullKeyBlob(includePrivateParameters);
					ECCng.ExportPrimeCurveParameters(ref ecParams, ecBlob, includePrivateParameters);
				}
				else
				{
					byte[] ecBlob2 = ExportKeyBlob(includePrivateParameters);
					ECCng.ExportNamedCurveParameters(ref ecParams, ecBlob2, includePrivateParameters);
					ecParams.Curve = ECCurve.CreateFromFriendlyName(curveName);
				}
				return ecParams;
			}

			public unsafe override byte[] SignHash(byte[] hash)
			{
				if (hash == null)
				{
					throw new ArgumentNullException("hash");
				}
				int estimatedSize = KeySize switch
				{
					256 => 64, 
					384 => 96, 
					521 => 132, 
					_ => KeySize / 4, 
				};
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return keyHandle.SignHash(hash, Interop.NCrypt.AsymmetricPaddingMode.None, null, estimatedSize);
			}

			public unsafe override bool TrySignHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
			{
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return keyHandle.TrySignHash(source, destination, Interop.NCrypt.AsymmetricPaddingMode.None, null, out bytesWritten);
			}

			public override bool VerifyHash(byte[] hash, byte[] signature)
			{
				if (hash == null)
				{
					throw new ArgumentNullException("hash");
				}
				if (signature == null)
				{
					throw new ArgumentNullException("signature");
				}
				return VerifyHash((ReadOnlySpan<byte>)hash, (ReadOnlySpan<byte>)signature);
			}

			public unsafe override bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
			{
				using SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle();
				return keyHandle.VerifyHash(hash, signature, Interop.NCrypt.AsymmetricPaddingMode.None, null);
			}
		}
	}
}
