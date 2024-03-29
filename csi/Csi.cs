﻿using System;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Microsoft.CodeAnalysis.CSharp.Scripting.Hosting
{
    internal static class Csi
    {
        private const string InteractiveResponseFileName = "csi.rsp";

        internal static int Main(string[] args)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(typeof(Csi).GetTypeInfo().Assembly.ManifestModule.FullyQualifiedName);
                BuildPaths buildPaths = new(directoryName, Directory.GetCurrentDirectory(), RuntimeMetadataReferenceResolver.GetDesktopFrameworkDirectory(), Path.GetTempPath());
                CSharpInteractiveCompiler compiler = new(Path.Combine(directoryName, InteractiveResponseFileName), buildPaths, args, new NotImplementedAnalyzerLoader());
                return new CommandLineRunner(ConsoleIO.Default, compiler, CSharpScriptCompiler.Instance, CSharpObjectFormatter.Instance).RunInteractive();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
