namespace System.Security.Cryptography
{
	public class SignatureDescription
	{
		public string KeyAlgorithm { get; set; }

		public string DigestAlgorithm { get; set; }

		public string FormatterAlgorithm { get; set; }

		public string DeformatterAlgorithm { get; set; }

		public SignatureDescription()
		{
		}

		public SignatureDescription(SecurityElement el)
		{
			if (el == null)
			{
				throw new ArgumentNullException("el");
			}
			KeyAlgorithm = el.SearchForTextOfTag("Key");
			DigestAlgorithm = el.SearchForTextOfTag("Digest");
			FormatterAlgorithm = el.SearchForTextOfTag("Formatter");
			DeformatterAlgorithm = el.SearchForTextOfTag("Deformatter");
		}

		public virtual AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(DeformatterAlgorithm);
			asymmetricSignatureDeformatter.SetKey(key);
			return asymmetricSignatureDeformatter;
		}

		public virtual AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(FormatterAlgorithm);
			asymmetricSignatureFormatter.SetKey(key);
			return asymmetricSignatureFormatter;
		}

		public virtual HashAlgorithm CreateDigest()
		{
			return (HashAlgorithm)CryptoConfig.CreateFromName(DigestAlgorithm);
		}
	}
}
