// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.ConsoleUtil
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

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
