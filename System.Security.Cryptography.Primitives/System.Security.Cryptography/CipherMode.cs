using System.ComponentModel;

namespace System.Security.Cryptography.Primitives // FilRip Added sub namespace "Primitives"
{
	public enum CipherMode
	{
		CBC = 1,
		ECB,
		[EditorBrowsable(EditorBrowsableState.Never)]
		OFB,
		[EditorBrowsable(EditorBrowsableState.Never)]
		CFB,
		CTS
	}
}
