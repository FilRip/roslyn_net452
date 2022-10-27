using System;
using System.IO;

using Microsoft.CodeAnalysis.VisualBasic;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class VisualBasicCompilerServer : VisualBasicCompiler
    {
        private readonly Func<string, MetadataReferenceProperties, PortableExecutableReference> _metadataProvider;

        internal VisualBasicCompilerServer(
            Func<string, MetadataReferenceProperties, PortableExecutableReference> metadataProvider,
            string[] args,
            BuildPaths buildPaths,
            string? libDirectory,
            IAnalyzerAssemblyLoader analyzerLoader)
            : this(metadataProvider, Path.Combine(buildPaths.ClientDirectory, "vbc.rsp"), args, buildPaths, libDirectory, analyzerLoader)
        {
        }

        internal VisualBasicCompilerServer(
            Func<string, MetadataReferenceProperties, PortableExecutableReference> metadataProvider,
            string? responseFile,
            string[] args,
            BuildPaths buildPaths,
            string? libDirectory,
            IAnalyzerAssemblyLoader analyzerLoader)
            : base(VisualBasicCommandLineParser.Default, responseFile, args, buildPaths, libDirectory, analyzerLoader)
        {
            this._metadataProvider = metadataProvider;
        }

        public override Func<string, MetadataReferenceProperties, PortableExecutableReference> GetMetadataProvider() => this._metadataProvider;
    }
}
