namespace System.Security.Cryptography.Algorithms // FilRip Added sub namespace "Algorithms"
{
	public abstract class AsymmetricKeyExchangeDeformatter
	{
		public abstract string Parameters { get; set; }

		public abstract void SetKey(AsymmetricAlgorithm key);

		public abstract byte[] DecryptKeyExchange(byte[] rgb);
	}
}
