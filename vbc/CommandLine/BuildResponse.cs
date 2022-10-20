// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.BuildResponse
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal abstract class BuildResponse
    {
        public abstract BuildResponse.ResponseType Type { get; }

        public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.Unicode))
                {
                    writer.Write((int)this.Type);
                    this.AddResponseBody(writer);
                    writer.Flush();
                    cancellationToken.ThrowIfCancellationRequested();
                    int length = checked((int)memoryStream.Length);
                    await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4, cancellationToken).ConfigureAwait(false);
                    memoryStream.Position = 0L;
                    await memoryStream.CopyToAsync(outStream, length, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        protected abstract void AddResponseBody(BinaryWriter writer);

        public static async Task<BuildResponse> ReadAsync(
          Stream stream,
          CancellationToken cancellationToken = default)
        {
            byte[] lengthBuffer = new byte[4];
            await BuildProtocolConstants.ReadAllAsync(stream, lengthBuffer, 4, cancellationToken).ConfigureAwait(false);
            byte[] responseBuffer = new byte[(int)BitConverter.ToUInt32(lengthBuffer, 0)];
            await BuildProtocolConstants.ReadAllAsync(stream, responseBuffer, responseBuffer.Length, cancellationToken).ConfigureAwait(false);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(responseBuffer), Encoding.Unicode))
            {
                switch (reader.ReadInt32())
                {
                    case 0:
                        return new MismatchedVersionBuildResponse();
                    case 1:
                        return CompletedBuildResponse.Create(reader);
                    case 2:
                        return AnalyzerInconsistencyBuildResponse.Create(reader);
                    case 3:
                        return ShutdownBuildResponse.Create(reader);
                    case 4:
                        return RejectedBuildResponse.Create(reader);
                    case 5:
                        return new IncorrectHashBuildResponse();
                    default:
                        throw new InvalidOperationException("Received invalid response type from server.");
                }
            }
        }

        public enum ResponseType
        {
            MismatchedVersion,
            Completed,
            AnalyzerInconsistency,
            Shutdown,
            Rejected,
            IncorrectHash,
        }
    }
}
