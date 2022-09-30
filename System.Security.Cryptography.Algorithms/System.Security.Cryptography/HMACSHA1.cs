using System.ComponentModel;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public class HMACSHA1 : HMAC
	{
		private HMACCommon _hMacCommon;

		public override byte[] Key
		{
			get
			{
				return base.Key;
			}
			set
			{
				_hMacCommon.ChangeKey(value);
				base.Key = _hMacCommon.ActualKey;
			}
		}

		public HMACSHA1()
			: this(Internal.Cryptography.Helpers.GenerateRandom(64))
		{
		}

		public HMACSHA1(byte[] key)
		{
			base.HashName = "SHA1";
			_hMacCommon = new HMACCommon("SHA1", key, 64);
			base.Key = _hMacCommon.ActualKey;
			base.BlockSizeValue = 64;
			HashSizeValue = _hMacCommon.HashSizeInBits;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public HMACSHA1(byte[] key, bool useManagedSha1)
			: this(key)
		{
		}

		protected override void HashCore(byte[] rgb, int ib, int cb)
		{
			_hMacCommon.AppendHashData(rgb, ib, cb);
		}

		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			_hMacCommon.AppendHashData(source);
		}

		protected override byte[] HashFinal()
		{
			return _hMacCommon.FinalizeHashAndReset();
		}

		protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
		{
			return _hMacCommon.TryFinalizeHashAndReset(destination, out bytesWritten);
		}

		public override void Initialize()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				HMACCommon hMacCommon = _hMacCommon;
				if (hMacCommon != null)
				{
					_hMacCommon = null;
					hMacCommon.Dispose(disposing);
				}
			}
			base.Dispose(disposing);
		}
	}
}
