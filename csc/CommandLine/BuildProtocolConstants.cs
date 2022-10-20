using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal static class BuildProtocolConstants
    {
        public enum ArgumentId
        {
            CurrentDirectory = 1360294433,
            CommandLineArgument,
            LibEnvVariable,
            KeepAlive,
            Shutdown,
            TempDirectory
        }

        public static string? ReadLengthPrefixedString(BinaryReader reader)
        {
            int num = reader.ReadInt32();
            if (num < 0)
            {
                return null;
            }
            return new string(reader.ReadChars(num));
        }

        public static void WriteLengthPrefixedString(BinaryWriter writer, string? value)
        {
            if (value != null)
            {
                writer.Write(value!.Length);
                writer.Write(value!.ToCharArray());
            }
            else
            {
                writer.Write(-1);
            }
        }

        public static string? GetCommitHash()
        {
            IEnumerable<CommitHashAttribute> customAttributes = typeof(BuildRequest).Assembly.GetCustomAttributes<CommitHashAttribute>();
            if (customAttributes.Count() != 1)
            {
                return null;
            }
            return customAttributes.Single().Hash;
        }

        internal static async Task ReadAllAsync(Stream stream, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            int totalBytesRead = 0;
            do
            {
                int num = await stream.ReadAsync(buffer, totalBytesRead, count - totalBytesRead, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                if (num == 0)
                {
                    throw new EndOfStreamException("Reached end of stream before end of read.");
                }
                totalBytesRead += num;
            }
            while (totalBytesRead < count);
        }
    }
}
