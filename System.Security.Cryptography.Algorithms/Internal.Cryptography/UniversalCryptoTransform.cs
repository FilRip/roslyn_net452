using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
	internal abstract class UniversalCryptoTransform : ICryptoTransform, IDisposable
	{
		public bool CanReuseTransform => true;

		public bool CanTransformMultipleBlocks => true;

		public int InputBlockSize => BasicSymmetricCipher.BlockSizeInBytes;

		public int OutputBlockSize => BasicSymmetricCipher.BlockSizeInBytes;

		protected PaddingMode PaddingMode { get; private set; }

		protected BasicSymmetricCipher BasicSymmetricCipher { get; private set; }

		public static ICryptoTransform Create(PaddingMode paddingMode, BasicSymmetricCipher cipher, bool encrypting)
		{
			if (encrypting)
			{
				return new UniversalCryptoEncryptor(paddingMode, cipher);
			}
			return new UniversalCryptoDecryptor(paddingMode, cipher);
		}

		protected UniversalCryptoTransform(PaddingMode paddingMode, BasicSymmetricCipher basicSymmetricCipher)
		{
			PaddingMode = paddingMode;
			BasicSymmetricCipher = basicSymmetricCipher;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (inputOffset > inputBuffer.Length)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (inputCount <= 0)
			{
				throw new ArgumentOutOfRangeException("inputCount");
			}
			if (inputCount % InputBlockSize != 0)
			{
				throw new ArgumentOutOfRangeException("inputCount", System.SR.Cryptography_MustTransformWholeBlock);
			}
			if (inputCount > inputBuffer.Length - inputOffset)
			{
				throw new ArgumentOutOfRangeException("inputCount", System.SR.Cryptography_TransformBeyondEndOfBuffer);
			}
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (outputOffset > outputBuffer.Length)
			{
				throw new ArgumentOutOfRangeException("outputOffset");
			}
			if (inputCount > outputBuffer.Length - outputOffset)
			{
				throw new ArgumentOutOfRangeException("outputOffset", System.SR.Cryptography_TransformBeyondEndOfBuffer);
			}
			return UncheckedTransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (inputCount < 0)
			{
				throw new ArgumentOutOfRangeException("inputCount");
			}
			if (inputOffset > inputBuffer.Length)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (inputCount > inputBuffer.Length - inputOffset)
			{
				throw new ArgumentOutOfRangeException("inputCount", System.SR.Cryptography_TransformBeyondEndOfBuffer);
			}
			return UncheckedTransformFinalBlock(inputBuffer, inputOffset, inputCount);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				BasicSymmetricCipher.Dispose();
			}
		}

		protected abstract int UncheckedTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

		protected abstract byte[] UncheckedTransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
	}
}
