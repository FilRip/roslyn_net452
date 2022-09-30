using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public class HMACSHA384 : HMAC
	{
		private HMACCommon _hMacCommon;

		public bool ProduceLegacyHmacValues
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new PlatformNotSupportedException();
				}
			}
		}

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

		public HMACSHA384()
			: this(Internal.Cryptography.Helpers.GenerateRandom(128))
		{
		}

		public HMACSHA384(byte[] key)
		{
			base.HashName = "SHA384";
			_hMacCommon = new HMACCommon("SHA384", key, 128);
			base.Key = _hMacCommon.ActualKey;
			base.BlockSizeValue = 128;
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
