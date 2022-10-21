using System;
using System.IO;
using System.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal static class ConsoleUtil
    {
        private static readonly Encoding s_utf8Encoding = new UTF8Encoding(false);

        internal static T RunWithUtf8Output<T>(Func<TextWriter, T> func)
        {
            Encoding outputEncoding = Console.OutputEncoding;
            try
            {
                Console.OutputEncoding = ConsoleUtil.s_utf8Encoding;
                return func(Console.Out);
            }
            finally
            {
                try
                {
                    Console.OutputEncoding = outputEncoding;
                }
                catch
                {
                }
            }
        }

        internal static T RunWithUtf8Output<T>(
          bool utf8Output,
          TextWriter textWriter,
          Func<TextWriter, T> func)
        {
            if (!utf8Output || textWriter.Encoding.CodePage == ConsoleUtil.s_utf8Encoding.CodePage)
                return func(textWriter);
            if (textWriter != Console.Out)
                throw new InvalidOperationException("Utf8Output is only supported when writing to Console.Out");
            return ConsoleUtil.RunWithUtf8Output<T>(func);
        }
    }
}
