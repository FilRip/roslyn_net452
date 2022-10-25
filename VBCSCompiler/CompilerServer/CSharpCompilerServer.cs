using System;
using System.IO;

using Microsoft.CodeAnalysis.CSharp;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class CSharpCompilerServer : CSharpCompiler
    {
        private readonly Func<string, MetadataReferenceProperties, PortableExecutableReference> _metadataProvider;

        internal CSharpCompilerServer(
          Func<string, MetadataReferenceProperties, PortableExecutableReference> metadataProvider,
          string[] args,
          BuildPaths buildPaths,
          string? libDirectory,
          IAnalyzerAssemblyLoader analyzerLoader)
          : this(metadataProvider, Path.Combine(buildPaths.ClientDirectory, "csc.rsp"), args, buildPaths, libDirectory, analyzerLoader)
        {
        }

        internal CSharpCompilerServer(
          Func<string, MetadataReferenceProperties, PortableExecutableReference> metadataProvider,
          string? responseFile,
          string[] args,
          BuildPaths buildPaths,
          string? libDirectory,
          IAnalyzerAssemblyLoader analyzerLoader)
          : base(CSharpCommandLineParser.Default, responseFile, args, buildPaths, libDirectory, analyzerLoader)
        {
            this._metadataProvider = metadataProvider;
        }

        public override Func<string, MetadataReferenceProperties, PortableExecutableReference> GetMetadataProvider() => this._metadataProvider;
    }
}
