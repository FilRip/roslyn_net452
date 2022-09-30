using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public class RSAPKCS1SignatureDeformatter : AsymmetricSignatureDeformatter
	{
		private RSA _rsaKey;

		private string _algName;

		public RSAPKCS1SignatureDeformatter()
		{
		}

		public RSAPKCS1SignatureDeformatter(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			_rsaKey = (RSA)key;
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			_rsaKey = (RSA)key;
		}

		public override void SetHashAlgorithm(string strName)
		{
			try
			{
				Oid.FromFriendlyName(strName, OidGroup.HashAlgorithm);
				_algName = HashAlgorithmNames.ToUpper(strName);
			}
			catch (CryptographicException)
			{
				_algName = null;
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
			if (_algName == null)
			{
				throw new CryptographicUnexpectedOperationException(SR.Cryptography_MissingOID);
			}
			if (_rsaKey == null)
			{
				throw new CryptographicUnexpectedOperationException(SR.Cryptography_MissingKey);
			}
			return _rsaKey.VerifyHash(rgbHash, rgbSignature, new HashAlgorithmName(_algName), RSASignaturePadding.Pkcs1);
		}
	}
}
