using System.Buffers;
using System.IO;

namespace System.Security.Cryptography
{
	public abstract class ECDsa : AsymmetricAlgorithm
	{
		public override string KeyExchangeAlgorithm => null;

		public override string SignatureAlgorithm => "ECDsa";

		public new static ECDsa Create(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			return CryptoConfig.CreateFromName(algorithm) as ECDsa;
		}

		public virtual ECParameters ExportParameters(bool includePrivateParameters)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		public virtual ECParameters ExportExplicitParameters(bool includePrivateParameters)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		public virtual void ImportParameters(ECParameters parameters)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		public virtual void GenerateKey(ECCurve curve)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		public virtual byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return SignData(data, 0, data.Length, hashAlgorithm);
		}

		public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > data.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			byte[] hash = HashData(data, offset, count, hashAlgorithm);
			return SignHash(hash);
		}

		public virtual bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			if (TryHashData(data, destination, hashAlgorithm, out var bytesWritten2) && TrySignHash(destination.Slice(0, bytesWritten2), destination, out bytesWritten))
			{
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			byte[] hash = HashData(data, hashAlgorithm);
			return SignHash(hash);
		}

		public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return VerifyData(data, 0, data.Length, signature, hashAlgorithm);
		}

		public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > data.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			byte[] hash = HashData(data, offset, count, hashAlgorithm);
			return VerifyHash(hash, signature);
		}

		public virtual bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			int num = 256;
			while (true)
			{
				int bytesWritten = 0;
				byte[] array = ArrayPool<byte>.Shared.Rent(num);
				try
				{
					if (TryHashData(data, array, hashAlgorithm, out bytesWritten))
					{
						return VerifyHash(new ReadOnlySpan<byte>(array, 0, bytesWritten), signature);
					}
				}
				finally
				{
					Array.Clear(array, 0, bytesWritten);
					ArrayPool<byte>.Shared.Return(array);
				}
				num = checked(num * 2);
			}
		}

		public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			byte[] hash = HashData(data, hashAlgorithm);
			return VerifyHash(hash, signature);
		}

		public abstract byte[] SignHash(byte[] hash);

		public abstract bool VerifyHash(byte[] hash, byte[] signature);

		protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		protected virtual bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				data.CopyTo(array);
				byte[] array2 = HashData(array, 0, data.Length, hashAlgorithm);
				if (array2.Length <= destination.Length)
				{
					new ReadOnlySpan<byte>(array2).CopyTo(destination);
					bytesWritten = array2.Length;
					return true;
				}
				bytesWritten = 0;
				return false;
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array);
			}
		}

		public virtual bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
		{
			byte[] array = SignHash(hash.ToArray());
			if (array.Length <= destination.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
		{
			return VerifyHash(hash.ToArray(), signature.ToArray());
		}

		public override void FromXmlString(string xmlString)
		{
			throw new NotImplementedException(SR.Cryptography_ECXmlSerializationFormatRequired);
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			throw new NotImplementedException(SR.Cryptography_ECXmlSerializationFormatRequired);
		}

		public new static ECDsa Create()
		{
			return new ECDsaImplementation.ECDsaCng();
		}

		public static ECDsa Create(ECCurve curve)
		{
			return new ECDsaImplementation.ECDsaCng(curve);
		}

		public static ECDsa Create(ECParameters parameters)
		{
			ECDsa eCDsa = new ECDsaImplementation.ECDsaCng();
			eCDsa.ImportParameters(parameters);
			return eCDsa;
		}
	}
}
