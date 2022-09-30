using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public class HMACMD5 : HMAC
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

		public HMACMD5()
			: this(Internal.Cryptography.Helpers.GenerateRandom(64))
		{
		}

		public HMACMD5(byte[] key)
		{
			base.HashName = "MD5";
			_hMacCommon = new HMACCommon("MD5", key, 64);
			base.Key = _hMacCommon.ActualKey;
			base.BlockSizeValue = 64;
			HashSizeValue = _hMacCommon.HashSizeInBits;
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
