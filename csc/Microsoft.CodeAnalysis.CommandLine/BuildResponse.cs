using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal abstract class BuildResponse
	{
		public enum ResponseType
		{
			MismatchedVersion,
			Completed,
			AnalyzerInconsistency,
			Shutdown,
			Rejected,
			IncorrectHash
		}

		public abstract ResponseType Type { get; }

		public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.Unicode);
			writer.Write((int)Type);
			AddResponseBody(writer);
			writer.Flush();
			cancellationToken.ThrowIfCancellationRequested();
			int length = checked((int)memoryStream.Length);
			await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			memoryStream.Position = 0L;
			await memoryStream.CopyToAsync(outStream, length, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		protected abstract void AddResponseBody(BinaryWriter writer);

		public static async Task<BuildResponse> ReadAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
		{
			byte[] lengthBuffer = new byte[4];
			await BuildProtocolConstants.ReadAllAsync(stream, lengthBuffer, 4, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			uint num = BitConverter.ToUInt32(lengthBuffer, 0);
			byte[] responseBuffer = new byte[num];
			await BuildProtocolConstants.ReadAllAsync(stream, responseBuffer, responseBuffer.Length, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			using BinaryReader binaryReader = new BinaryReader(new MemoryStream(responseBuffer), Encoding.Unicode);
			return binaryReader.ReadInt32() switch
			{
				1 => CompletedBuildResponse.Create(binaryReader), 
				0 => new MismatchedVersionBuildResponse(), 
				5 => new IncorrectHashBuildResponse(), 
				2 => AnalyzerInconsistencyBuildResponse.Create(binaryReader), 
				3 => ShutdownBuildResponse.Create(binaryReader), 
				4 => RejectedBuildResponse.Create(binaryReader), 
				_ => throw new InvalidOperationException("Received invalid response type from server."), 
			};
		}
	}
}
