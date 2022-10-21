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
        public static string? ReadLengthPrefixedString(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            return count < 0 ? null : new string(reader.ReadChars(count));
        }

        public static void WriteLengthPrefixedString(BinaryWriter writer, string? value)
        {
            if (value != null)
            {
                writer.Write(value.Length);
                writer.Write(value.ToCharArray());
            }
            else
                writer.Write(-1);
        }

        public static string? GetCommitHash()
        {
            IEnumerable<CommitHashAttribute> customAttributes = typeof(BuildRequest).Assembly.GetCustomAttributes<CommitHashAttribute>();
            return customAttributes.Count<CommitHashAttribute>() != 1 ? null : customAttributes.Single<CommitHashAttribute>().Hash;
        }

        internal static async Task ReadAllAsync(
          Stream stream,
          byte[] buffer,
          int count,
          CancellationToken cancellationToken)
        {
            int totalBytesRead = 0;
            do
            {
                int num = await stream.ReadAsync(buffer, totalBytesRead, count - totalBytesRead, cancellationToken).ConfigureAwait(false);
                if (num == 0)
                    throw new EndOfStreamException("Reached end of stream before end of read.");
                totalBytesRead += num;
            }
            while (totalBytesRead < count);
        }

        public enum ArgumentId
        {
            CurrentDirectory = 1360294433, // 0x51147221
            CommandLineArgument = 1360294434, // 0x51147222
            LibEnvVariable = 1360294435, // 0x51147223
            KeepAlive = 1360294436, // 0x51147224
            Shutdown = 1360294437, // 0x51147225
            TempDirectory = 1360294438, // 0x51147226
        }
    }
}
