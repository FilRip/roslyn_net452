using System;
using System.IO;
using System.Text;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal static class ConsoleUtil
	{
		private static readonly Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

		internal static T RunWithUtf8Output<T>(Func<TextWriter, T> func)
		{
			Encoding outputEncoding = Console.OutputEncoding;
			try
			{
				Console.OutputEncoding = s_utf8Encoding;
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

		internal static T RunWithUtf8Output<T>(bool utf8Output, TextWriter textWriter, Func<TextWriter, T> func)
		{
			if (utf8Output && textWriter.Encoding.CodePage != s_utf8Encoding.CodePage)
			{
				if (textWriter != Console.Out)
				{
					throw new InvalidOperationException("Utf8Output is only supported when writing to Console.Out");
				}
				return RunWithUtf8Output(func);
			}
			return func(textWriter);
		}
	}
}
