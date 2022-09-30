using System.Buffers;
using System.IO;

namespace System.Security.Cryptography.Algorithms // FilRip : Added sub namespace "Algorithms"
{
	public abstract class RSA : AsymmetricAlgorithm
	{
		public override string KeyExchangeAlgorithm => "RSA";

		public override string SignatureAlgorithm => "RSA";

		public new static RSA Create(string algName)
		{
			return (RSA)CryptoConfig.CreateFromName(algName);
		}

		public static RSA Create(int keySizeInBits)
		{
			RSA rSA = Create();
			try
			{
				rSA.KeySize = keySizeInBits;
				return rSA;
			}
			catch
			{
				rSA.Dispose();
				throw;
			}
		}

		public static RSA Create(RSAParameters parameters)
		{
			RSA rSA = Create();
			try
			{
				rSA.ImportParameters(parameters);
				return rSA;
			}
			catch
			{
				rSA.Dispose();
				throw;
			}
		}

		public abstract RSAParameters ExportParameters(bool includePrivateParameters);

		public abstract void ImportParameters(RSAParameters parameters);

		public virtual byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
		{
			throw DerivedClassMustOverride();
		}

		public virtual byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
		{
			throw DerivedClassMustOverride();
		}

		public virtual byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			throw DerivedClassMustOverride();
		}

		public virtual bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			throw DerivedClassMustOverride();
		}

		protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			throw DerivedClassMustOverride();
		}

		protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			throw DerivedClassMustOverride();
		}

		public virtual bool TryDecrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
		{
			byte[] array = Decrypt(data.ToArray(), padding);
			if (destination.Length >= array.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool TryEncrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
		{
			byte[] array = Encrypt(data.ToArray(), padding);
			if (destination.Length >= array.Length)
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
			byte[] array2;
			try
			{
				data.CopyTo(array);
				array2 = HashData(array, 0, data.Length, hashAlgorithm);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array);
			}
			if (destination.Length >= array2.Length)
			{
				new ReadOnlySpan<byte>(array2).CopyTo(destination);
				bytesWritten = array2.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
		{
			byte[] array = SignHash(hash.ToArray(), hashAlgorithm, padding);
			if (destination.Length >= array.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			return VerifyHash(hash.ToArray(), signature.ToArray(), hashAlgorithm, padding);
		}

		private static Exception DerivedClassMustOverride()
		{
			return new NotImplementedException(SR.NotSupported_SubclassOverride);
		}

		public virtual byte[] DecryptValue(byte[] rgb)
		{
			throw new NotSupportedException(SR.NotSupported_Method);
		}

		public virtual byte[] EncryptValue(byte[] rgb)
		{
			throw new NotSupportedException(SR.NotSupported_Method);
		}

		public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return SignData(data, 0, data.Length, hashAlgorithm, padding);
		}

		public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
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
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = HashData(data, offset, count, hashAlgorithm);
			return SignHash(hash, hashAlgorithm, padding);
		}

		public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = HashData(data, hashAlgorithm);
			return SignHash(hash, hashAlgorithm, padding);
		}

		public virtual bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			if (TryHashData(data, destination, hashAlgorithm, out var bytesWritten2) && TrySignHash(destination.Slice(0, bytesWritten2), destination, hashAlgorithm, padding, out bytesWritten))
			{
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return VerifyData(data, 0, data.Length, signature, hashAlgorithm, padding);
		}

		public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
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
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = HashData(data, offset, count, hashAlgorithm);
			return VerifyHash(hash, signature, hashAlgorithm, padding);
		}

		public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
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
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = HashData(data, hashAlgorithm);
			return VerifyHash(hash, signature, hashAlgorithm, padding);
		}

		public virtual bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
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
						return VerifyHash(new ReadOnlySpan<byte>(array, 0, bytesWritten), signature, hashAlgorithm, padding);
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

		private static Exception HashAlgorithmNameNullOrEmpty()
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

		public new static RSA Create()
		{
			return new RSAImplementation.RSACng();
		}
	}
}
