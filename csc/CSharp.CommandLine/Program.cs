using System;
using System.IO;

using Microsoft.CodeAnalysis.CommandLine;

namespace Microsoft.CodeAnalysis.CSharp.CommandLine
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                return MainCore(args);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private static int MainCore(string[] args)
        {
            Guid guid = Guid.NewGuid();
            using CompilerServerLogger logger = new($"csc {guid}");
            return BuildClient.Run(args, RequestLanguage.CSharpCompile, Csc.Run, logger, guid);
        }

        public static int Run(string[] args, string clientDir, string workingDir, string sdkDir, string tempDir, TextWriter textWriter, IAnalyzerAssemblyLoader analyzerLoader)
        {
            return Csc.Run(args, new BuildPaths(clientDir, workingDir, sdkDir, tempDir), textWriter, analyzerLoader);
        }
    }
}
