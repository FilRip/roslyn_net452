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
        public struct Argument
        {
            public readonly BuildProtocolConstants.ArgumentId ArgumentId;

            public readonly int ArgumentIndex;

            public readonly string? Value;

            public Argument(BuildProtocolConstants.ArgumentId argumentId, int argumentIndex, string? value)
            {
                ArgumentId = argumentId;
                ArgumentIndex = argumentIndex;
                Value = value;
            }

            public static Argument ReadFromBinaryReader(BinaryReader reader)
            {
                int argumentId = reader.ReadInt32();
                int argumentIndex = reader.ReadInt32();
                string value = BuildProtocolConstants.ReadLengthPrefixedString(reader);
                return new Argument((BuildProtocolConstants.ArgumentId)argumentId, argumentIndex, value);
            }

            public void WriteToBinaryWriter(BinaryWriter writer)
            {
                writer.Write((int)ArgumentId);
                writer.Write(ArgumentIndex);
                BuildProtocolConstants.WriteLengthPrefixedString(writer, Value);
            }
        }

        //private const int MaximumRequestSize = 5242880;

        public readonly Guid RequestId;

        public readonly RequestLanguage Language;

        public readonly ReadOnlyCollection<Argument> Arguments;

        public readonly string CompilerHash;

        public BuildRequest(RequestLanguage language, string compilerHash, IEnumerable<Argument> arguments, Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.Empty;
            Language = language;
            Arguments = new ReadOnlyCollection<Argument>(arguments.ToList());
            CompilerHash = compilerHash;
            if (Arguments.Count > 65535)
            {
                throw new ArgumentOutOfRangeException("arguments", "Too many arguments: maximum of " + ushort.MaxValue + " arguments allowed.");
            }
        }

        public static BuildRequest Create(RequestLanguage language, IList<string> args, string workingDirectory, string tempDirectory, string compilerHash, Guid? requestId = null, string? keepAlive = null, string? libDirectory = null)
        {
            List<Argument> list = new(args.Count + 1 + ((libDirectory != null) ? 1 : 0));
            list.Add(new Argument(BuildProtocolConstants.ArgumentId.CurrentDirectory, 0, workingDirectory));
            list.Add(new Argument(BuildProtocolConstants.ArgumentId.TempDirectory, 0, tempDirectory));
            if (keepAlive != null)
            {
                list.Add(new Argument(BuildProtocolConstants.ArgumentId.KeepAlive, 0, keepAlive));
            }
            if (libDirectory != null)
            {
                list.Add(new Argument(BuildProtocolConstants.ArgumentId.LibEnvVariable, 0, libDirectory));
            }
            for (int i = 0; i < args.Count; i++)
            {
                string value = args[i];
                list.Add(new Argument(BuildProtocolConstants.ArgumentId.CommandLineArgument, i, value));
            }
            return new BuildRequest(language, compilerHash, list, requestId);
        }

        public static BuildRequest CreateShutdown()
        {
            Argument[] arguments = new Argument[1]
            {
                new Argument(BuildProtocolConstants.ArgumentId.Shutdown, 0, "")
            };
            return new BuildRequest(RequestLanguage.CSharpCompile, BuildProtocolConstants.GetCommitHash() ?? "", arguments);
        }

        public static async Task<BuildRequest> ReadAsync(Stream inStream, CancellationToken cancellationToken)
        {
            byte[] lengthBuffer = new byte[4];
            await BuildProtocolConstants.ReadAllAsync(inStream, lengthBuffer, 4, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            int num = BitConverter.ToInt32(lengthBuffer, 0);
            if (num > 5242880)
            {
                throw new ArgumentException($"Request is over {5}MB in length");
            }
            cancellationToken.ThrowIfCancellationRequested();
            byte[] requestBuffer = new byte[num];
            await BuildProtocolConstants.ReadAllAsync(inStream, requestBuffer, num, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            cancellationToken.ThrowIfCancellationRequested();
            using (BinaryReader binaryReader = new(new MemoryStream(requestBuffer), Encoding.Unicode))
            {
                Guid value = readGuid(binaryReader);
                RequestLanguage language = (RequestLanguage)binaryReader.ReadUInt32();
                string compilerHash = binaryReader.ReadString();
                uint num2 = binaryReader.ReadUInt32();
                List<Argument> list = new((int)num2);
                for (int i = 0; i < num2; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    list.Add(Argument.ReadFromBinaryReader(binaryReader));
                }
                return new BuildRequest(language, compilerHash, list, value);
            }
            static Guid readGuid(BinaryReader reader)
            {
                byte[] array = new byte[16];
                if (16 != reader.Read(array, 0, 16))
                {
                    throw new InvalidOperationException();
                }
                return new Guid(array);
            }
        }

        public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken = default)
        {
            using MemoryStream memoryStream = new();
            using BinaryWriter writer = new(memoryStream, Encoding.Unicode);
            writer.Write(RequestId.ToByteArray()); writer.Write((uint)Language);
            writer.Write(CompilerHash);
            writer.Write(Arguments.Count);
            foreach (Argument argument in Arguments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                argument.WriteToBinaryWriter(writer);
            }
            writer.Flush();
            cancellationToken.ThrowIfCancellationRequested();
            int length = checked((int)memoryStream.Length);
            if (memoryStream.Length > 5242880)
            {
                throw new ArgumentOutOfRangeException($"Request is over {5}MB in length");
            }
            await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            memoryStream.Position = 0L;
            await memoryStream.CopyToAsync(outStream, length, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
