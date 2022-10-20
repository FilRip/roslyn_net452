// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.VisualBasic.CommandLine.Program
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System;
using System.IO;

using Microsoft.CodeAnalysis.CommandLine;

namespace Microsoft.CodeAnalysis.VisualBasic.CommandLine
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                return Program.MainCore(args);
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
            using (CompilerServerLogger logger = new CompilerServerLogger(string.Format("vbc {0}", guid)))
                return BuildClient.Run(args, RequestLanguage.VisualBasicCompile, new CompileFunc(Vbc.Run), logger, new Guid?(guid));
        }

        public static int Run(
          string[] args,
          string clientDir,
          string workingDir,
          string sdkDir,
          string tempDir,
          TextWriter textWriter,
          IAnalyzerAssemblyLoader analyzerLoader)
        {
            return Vbc.Run(args, new BuildPaths(clientDir, workingDir, sdkDir, tempDir), textWriter, analyzerLoader);
        }
    }
}
