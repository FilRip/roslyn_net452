using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal static class ECDiffieHellmanImplementation
	{
		public sealed class ECDiffieHellmanCng : ECDiffieHellman
		{
			private readonly ECCngKey _key = new ECCngKey("ECDH");

			public override ECDiffieHellmanPublicKey PublicKey
			{
				get
				{
					string curveName = GetCurveName();
					return new ECDiffieHellmanCngPublicKey((curveName == null) ? ExportFullKeyBlob(includePrivateParameters: false) : ExportKeyBlob(includePrivateParameters: false), curveName);
				}
			}

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
				_key.SetHandle(keyHandle, "ECDH");
				ForceSetKeySize(_key.KeySize);
			}

			private void ImportKeyBlob(byte[] ecKeyBlob, string curveName, bool includePrivateParameters)
			{
				string blobType = (includePrivateParameters ? "ECCPRIVATEBLOB" : "ECCPUBLICBLOB");
				SafeNCryptKeyHandle keyHandle = CngKeyLite.ImportKeyBlob(blobType, ecKeyBlob, curveName);
				_key.SetHandle(keyHandle, ECCng.EcdhCurveNameToAlgorithm(curveName));
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

			public override byte[] DeriveKeyMaterial(ECDiffieHellmanPublicKey otherPartyPublicKey)
			{
				if (otherPartyPublicKey == null)
				{
					throw new ArgumentNullException("otherPartyPublicKey");
				}
				return DeriveKeyFromHash(otherPartyPublicKey, HashAlgorithmName.SHA256);
			}

			private SafeNCryptSecretHandle DeriveSecretAgreementHandle(ECDiffieHellmanPublicKey otherPartyPublicKey)
			{
				if (otherPartyPublicKey == null)
				{
					throw new ArgumentNullException("otherPartyPublicKey");
				}
				ECParameters parameters = otherPartyPublicKey.ExportParameters();
				using ECDiffieHellmanCng eCDiffieHellmanCng = (ECDiffieHellmanCng)ECDiffieHellman.Create(parameters);
				using SafeNCryptKeyHandle safeNCryptKeyHandle = eCDiffieHellmanCng.GetDuplicatedKeyHandle();
				string propertyAsString = CngKeyLite.GetPropertyAsString(safeNCryptKeyHandle, "Algorithm Group", CngPropertyOptions.None);
				if (propertyAsString != "ECDH")
				{
					throw new ArgumentException(SR.Cryptography_ArgECDHRequiresECDHKey, "otherPartyPublicKey");
				}
				if (CngKeyLite.GetKeyLength(safeNCryptKeyHandle) != KeySize)
				{
					throw new ArgumentException(SR.Cryptography_ArgECDHKeySizeMismatch, "otherPartyPublicKey");
				}
				using SafeNCryptKeyHandle privateKey = GetDuplicatedKeyHandle();
				return Interop.NCrypt.DeriveSecretAgreement(privateKey, safeNCryptKeyHandle);
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

			public ECDiffieHellmanCng()
				: this(521)
			{
			}

			public ECDiffieHellmanCng(int keySize)
			{
				KeySize = keySize;
			}

			public ECDiffieHellmanCng(ECCurve curve)
			{
				GenerateKey(curve);
			}

			private void ForceSetKeySize(int newKeySize)
			{
				KeySizeValue = newKeySize;
			}

			public override byte[] DeriveKeyFromHash(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm, byte[] secretPrepend, byte[] secretAppend)
			{
				if (otherPartyPublicKey == null)
				{
					throw new ArgumentNullException("otherPartyPublicKey");
				}
				if (string.IsNullOrEmpty(hashAlgorithm.Name))
				{
					throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
				}
				using SafeNCryptSecretHandle secretAgreement = DeriveSecretAgreementHandle(otherPartyPublicKey);
				return Interop.NCrypt.DeriveKeyMaterialHash(secretAgreement, hashAlgorithm.Name, secretPrepend, secretAppend, Interop.NCrypt.SecretAgreementFlags.None);
			}

			public override byte[] DeriveKeyFromHmac(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm, byte[] hmacKey, byte[] secretPrepend, byte[] secretAppend)
			{
				if (otherPartyPublicKey == null)
				{
					throw new ArgumentNullException("otherPartyPublicKey");
				}
				if (string.IsNullOrEmpty(hashAlgorithm.Name))
				{
					throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
				}
				using SafeNCryptSecretHandle secretAgreement = DeriveSecretAgreementHandle(otherPartyPublicKey);
				Interop.NCrypt.SecretAgreementFlags flags = ((hmacKey == null) ? Interop.NCrypt.SecretAgreementFlags.UseSecretAsHmacKey : Interop.NCrypt.SecretAgreementFlags.None);
				return Interop.NCrypt.DeriveKeyMaterialHmac(secretAgreement, hashAlgorithm.Name, hmacKey, secretPrepend, secretAppend, flags);
			}

			public override byte[] DeriveKeyTls(ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] prfLabel, byte[] prfSeed)
			{
				if (otherPartyPublicKey == null)
				{
					throw new ArgumentNullException("otherPartyPublicKey");
				}
				if (prfLabel == null)
				{
					throw new ArgumentNullException("prfLabel");
				}
				if (prfSeed == null)
				{
					throw new ArgumentNullException("prfSeed");
				}
				using SafeNCryptSecretHandle secretAgreement = DeriveSecretAgreementHandle(otherPartyPublicKey);
				return Interop.NCrypt.DeriveKeyMaterialTls(secretAgreement, prfLabel, prfSeed, Interop.NCrypt.SecretAgreementFlags.None);
			}

			public override void ImportParameters(ECParameters parameters)
			{
				parameters.Validate();
				ECCurve curve = parameters.Curve;
				bool includePrivateParameters = parameters.D != null;
				if (curve.IsPrime)
				{
					byte[] primeCurveBlob = ECCng.GetPrimeCurveBlob(ref parameters, ecdh: true);
					ImportFullKeyBlob(primeCurveBlob, includePrivateParameters);
					return;
				}
				if (curve.IsNamed)
				{
					if (string.IsNullOrEmpty(curve.Oid.FriendlyName))
					{
						throw new PlatformNotSupportedException(string.Format(SR.Cryptography_InvalidCurveOid, curve.Oid.Value));
					}
					byte[] namedCurveBlob = ECCng.GetNamedCurveBlob(ref parameters, ecdh: true);
					ImportKeyBlob(namedCurveBlob, curve.Oid.FriendlyName, includePrivateParameters);
					return;
				}
				throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, curve.CurveType.ToString()));
			}

			public override ECParameters ExportExplicitParameters(bool includePrivateParameters)
			{
				byte[] array = ExportFullKeyBlob(includePrivateParameters);
				try
				{
					ECParameters ecParams = default(ECParameters);
					ECCng.ExportPrimeCurveParameters(ref ecParams, array, includePrivateParameters);
					return ecParams;
				}
				finally
				{
					Array.Clear(array, 0, array.Length);
				}
			}

			public override ECParameters ExportParameters(bool includePrivateParameters)
			{
				ECParameters ecParams = default(ECParameters);
				string curveName = GetCurveName();
				byte[] array = null;
				try
				{
					if (string.IsNullOrEmpty(curveName))
					{
						array = ExportFullKeyBlob(includePrivateParameters);
						ECCng.ExportPrimeCurveParameters(ref ecParams, array, includePrivateParameters);
					}
					else
					{
						array = ExportKeyBlob(includePrivateParameters);
						ECCng.ExportNamedCurveParameters(ref ecParams, array, includePrivateParameters);
						ecParams.Curve = ECCurve.CreateFromFriendlyName(curveName);
					}
					return ecParams;
				}
				finally
				{
					if (array != null)
					{
						Array.Clear(array, 0, array.Length);
					}
				}
			}
		}

		public sealed class ECDiffieHellmanCngPublicKey : ECDiffieHellmanPublicKey
		{
			private byte[] _keyBlob;

			internal string _curveName;

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			public override string ToXmlString()
			{
				throw new PlatformNotSupportedException();
			}

			internal ECDiffieHellmanCngPublicKey(byte[] keyBlob, string curveName)
				: base(keyBlob)
			{
				_keyBlob = keyBlob;
				_curveName = curveName;
			}

			public override ECParameters ExportExplicitParameters()
			{
				ECParameters ecParams = default(ECParameters);
				ECCng.ExportPrimeCurveParameters(ref ecParams, _keyBlob, includePrivateParameters: false);
				return ecParams;
			}

			public override ECParameters ExportParameters()
			{
				if (string.IsNullOrEmpty(_curveName))
				{
					return ExportExplicitParameters();
				}
				ECParameters ecParams = default(ECParameters);
				ECCng.ExportNamedCurveParameters(ref ecParams, _keyBlob, includePrivateParameters: false);
				ecParams.Curve = ECCurve.CreateFromFriendlyName(_curveName);
				return ecParams;
			}
		}
	}
}
