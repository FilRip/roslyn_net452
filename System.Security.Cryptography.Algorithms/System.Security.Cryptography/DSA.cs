using System.Buffers;
using System.IO;

namespace System.Security.Cryptography
{
	public abstract class DSA : AsymmetricAlgorithm
	{
		public abstract DSAParameters ExportParameters(bool includePrivateParameters);

		public abstract void ImportParameters(DSAParameters parameters);

		public new static DSA Create(string algName)
		{
			return (DSA)CryptoConfig.CreateFromName(algName);
		}

		public static DSA Create(int keySizeInBits)
		{
			DSA dSA = Create();
			try
			{
				dSA.KeySize = keySizeInBits;
				return dSA;
			}
			catch
			{
				dSA.Dispose();
				throw;
			}
		}

		public static DSA Create(DSAParameters parameters)
		{
			DSA dSA = Create();
			try
			{
				dSA.ImportParameters(parameters);
				return dSA;
			}
			catch
			{
				dSA.Dispose();
				throw;
			}
		}

		public abstract byte[] CreateSignature(byte[] rgbHash);

		public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);

		protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			throw DerivedClassMustOverride();
		}

		protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			throw DerivedClassMustOverride();
		}

		public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
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
				throw HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = HashData(data, offset, count, hashAlgorithm);
			return CreateSignature(rgbHash);
		}

		public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = HashData(data, hashAlgorithm);
			return CreateSignature(rgbHash);
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
				throw HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = HashData(data, offset, count, hashAlgorithm);
			return VerifySignature(rgbHash, signature);
		}

		public virtual bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm)
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
				throw HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = HashData(data, hashAlgorithm);
			return VerifySignature(rgbHash, signature);
		}

		public virtual bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
		{
			byte[] array = CreateSignature(hash.ToArray());
			if (array.Length <= destination.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		protected virtual bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				data.CopyTo(array);
				byte[] array2 = HashData(array, 0, data.Length, hashAlgorithm);
				if (destination.Length >= array2.Length)
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

		public virtual bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
			}
			if (TryHashData(data, destination, hashAlgorithm, out var bytesWritten2) && TryCreateSignature(destination.Slice(0, bytesWritten2), destination, out bytesWritten))
			{
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
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
						return VerifySignature(new ReadOnlySpan<byte>(array, 0, bytesWritten), signature);
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

		public virtual bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
		{
			return VerifySignature(hash.ToArray(), signature.ToArray());
		}

		private static Exception DerivedClassMustOverride()
		{
			return new NotImplementedException(SR.NotSupported_SubclassOverride);
		}

		internal static Exception HashAlgorithmNameNullOrEmpty()
		{
			return new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
		}

		public override void FromXmlString(string xmlString)
		{
			throw new PlatformNotSupportedException();
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public new static DSA Create()
		{
			return new DSAImplementation.DSACng();
		}
	}
}
