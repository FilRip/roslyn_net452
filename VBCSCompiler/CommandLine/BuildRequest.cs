using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal class BuildRequest
    {
        private const int MaximumRequestSize = 5242880;
        public readonly Guid RequestId;
        public readonly RequestLanguage Language;
        public readonly ReadOnlyCollection<Argument> Arguments;
        public readonly string CompilerHash;

        public BuildRequest(
            RequestLanguage language,
            string compilerHash,
            IEnumerable<Argument> arguments,
            Guid? requestId = null)
        {
            this.RequestId = requestId ?? Guid.Empty;
            this.Language = language;
            this.Arguments = new ReadOnlyCollection<Argument>(arguments.ToList());
            this.CompilerHash = compilerHash;
            if (this.Arguments.Count > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(arguments), "Too many arguments: maximum of " + ushort.MaxValue.ToString() + " arguments allowed.");
        }

        public static BuildRequest Create(
            RequestLanguage language,
            IList<string> args,
            string workingDirectory,
            string tempDirectory,
            string compilerHash,
            Guid? requestId = null,
            string? keepAlive = null,
            string? libDirectory = null)
        {
            List<Argument> arguments = new(args.Count + 1 + (libDirectory == null ? 0 : 1))
            {
                new Argument(BuildProtocolConstants.ArgumentId.CurrentDirectory, 0, workingDirectory),
                new Argument(BuildProtocolConstants.ArgumentId.TempDirectory, 0, tempDirectory),
            };
            if (keepAlive != null)
                arguments.Add(new Argument(BuildProtocolConstants.ArgumentId.KeepAlive, 0, keepAlive));
            if (libDirectory != null)
                arguments.Add(new Argument(BuildProtocolConstants.ArgumentId.LibEnvVariable, 0, libDirectory));
            for (int index = 0; index < args.Count; ++index)
            {
                string str = args[index];
                arguments.Add(new Argument(BuildProtocolConstants.ArgumentId.CommandLineArgument, index, str));
            }
            return new BuildRequest(language, compilerHash, arguments, requestId);
        }

        public static BuildRequest CreateShutdown()
        {
            Argument[] arguments = new Argument[1]
            {
                new Argument(BuildProtocolConstants.ArgumentId.Shutdown, 0, "")
            };
            return new BuildRequest(RequestLanguage.CSharpCompile, BuildProtocolConstants.GetCommitHash() ?? "", arguments);
        }

        public static async Task<BuildRequest> ReadAsync(
            Stream inStream,
            CancellationToken cancellationToken)
        {
            byte[] lengthBuffer = new byte[4];
            await BuildProtocolConstants.ReadAllAsync(inStream, lengthBuffer, 4, cancellationToken).ConfigureAwait(false);
            int int32 = BitConverter.ToInt32(lengthBuffer, 0);
            if (int32 > MaximumRequestSize)
                throw new ArgumentException(string.Format("Request is over {0}MB in length", 5));
            cancellationToken.ThrowIfCancellationRequested();
            byte[] requestBuffer = new byte[int32];
            await BuildProtocolConstants.ReadAllAsync(inStream, requestBuffer, int32, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            BuildRequest buildRequest;
            using (BinaryReader reader = new(new MemoryStream(requestBuffer), Encoding.Unicode))
            {
                Guid guid = readGuid(reader);
                RequestLanguage language = (RequestLanguage)reader.ReadUInt32();
                string compilerHash = reader.ReadString();
                uint capacity = reader.ReadUInt32();
                List<Argument> arguments = new((int)capacity);
                for (int index = 0; index < capacity; ++index)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    arguments.Add(Argument.ReadFromBinaryReader(reader));
                }
                buildRequest = new BuildRequest(language, compilerHash, arguments, new Guid?(guid));
            }
#nullable restore
            return buildRequest;

            static Guid readGuid(BinaryReader reader)
            {
                byte[] numArray = new byte[16];
                return 16 == reader.Read(numArray, 0, 16) ? new Guid(numArray) : throw new InvalidOperationException();
            }
        }

        public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken = default)
        {
            BinaryWriter writer;
            using MemoryStream memoryStream = new();
            writer = new BinaryWriter(memoryStream, Encoding.Unicode);
            try
            {
                writer.Write(this.RequestId.ToByteArray());
                writer.Write((uint)this.Language);
                writer.Write(this.CompilerHash);
                writer.Write(this.Arguments.Count);
                foreach (BuildRequest.Argument obj in this.Arguments)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    obj.WriteToBinaryWriter(writer);
                }
                writer.Flush();
                cancellationToken.ThrowIfCancellationRequested();
                int length = checked((int)memoryStream.Length);
                if (memoryStream.Length > MaximumRequestSize)
                    throw new ArgumentOutOfRangeException(string.Format("Request is over {0}MB in length", 5));
                await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0L;
                await memoryStream.CopyToAsync(outStream, length, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                writer?.Dispose();
            }
        }

#nullable enable

        public struct Argument
        {
            public readonly BuildProtocolConstants.ArgumentId ArgumentId;
            public readonly int ArgumentIndex;
            public readonly string? Value;

            public Argument(
                BuildProtocolConstants.ArgumentId argumentId,
                int argumentIndex,
                string? value)
            {
                this.ArgumentId = argumentId;
                this.ArgumentIndex = argumentIndex;
                this.Value = value;
            }

            public static Argument ReadFromBinaryReader(BinaryReader reader)
            {
                int num1 = reader.ReadInt32();
                int num2 = reader.ReadInt32();
#nullable restore
                string str1 = BuildProtocolConstants.ReadLengthPrefixedString(reader);
                int argumentIndex = num2;
                string str2 = str1;
                return new Argument((BuildProtocolConstants.ArgumentId)num1, argumentIndex, str2);
            }

            public void WriteToBinaryWriter(BinaryWriter writer)
            {
                writer.Write((int)this.ArgumentId);
                writer.Write(this.ArgumentIndex);
                BuildProtocolConstants.WriteLengthPrefixedString(writer, this.Value);
            }
        }
    }
}
