using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class CompilerServerHost : ICompilerServerHost
    {
        public IAnalyzerAssemblyLoader AnalyzerAssemblyLoader { get; } = new ShadowCopyAnalyzerAssemblyLoader(Path.Combine(Path.GetTempPath(), "VBCSCompiler", nameof(AnalyzerAssemblyLoader)));

        public static Func<string, MetadataReferenceProperties, PortableExecutableReference> SharedAssemblyReferenceProvider { get; } = (path, properties) => new CachingMetadataReference(path, properties);

        private Func<string, MetadataReferenceProperties, PortableExecutableReference> AssemblyReferenceProvider { get; } = CompilerServerHost.SharedAssemblyReferenceProvider;

        private string ClientDirectory { get; }

        private string SdkDirectory { get; }

        public ICompilerServerLogger Logger { get; }

        internal CompilerServerHost(
            string clientDirectory,
            string sdkDirectory,
            ICompilerServerLogger logger)
        {
            this.ClientDirectory = clientDirectory;
            this.SdkDirectory = sdkDirectory;
            this.Logger = logger;
        }

        private bool CheckAnalyzers(
            string baseDirectory,
            ImmutableArray<CommandLineAnalyzerReference> analyzers,
            [NotNullWhen(false)] out List<string>? errorMessages)
        {
            return AnalyzerConsistencyChecker.Check(baseDirectory, analyzers, this.AnalyzerAssemblyLoader, this.Logger, out errorMessages);
        }

        public bool TryCreateCompiler(
            in RunRequest request,
            BuildPaths buildPaths,
            [NotNullWhen(true)] out CommonCompiler? compiler)
        {
            string language = request.Language;
            if (language != "C#")
            {
                if (language == "Visual Basic")
                {
                    compiler = new VisualBasicCompilerServer(this.AssemblyReferenceProvider, request.Arguments, buildPaths, request.LibDirectory, this.AnalyzerAssemblyLoader);
                    return true;
                }
                compiler = null;
                return false;
            }
            compiler = new CSharpCompilerServer(this.AssemblyReferenceProvider, request.Arguments, buildPaths, request.LibDirectory, this.AnalyzerAssemblyLoader);
            return true;
        }

        public BuildResponse RunCompilation(
            in RunRequest request,
            CancellationToken cancellationToken)
        {
            this.Logger.Log(string.Format("\r\nRun Compilation for {0}\r\n  Language = {1}\r\n  CurrentDirectory = '{2}\r\n  LIB = '{3}'", request.RequestId, request.Language, request.WorkingDirectory, request.LibDirectory));
            if (string.IsNullOrEmpty(request.WorkingDirectory))
            {
                string reason = "Missing working directory";
                this.Logger.Log(string.Format("Rejected: {0}: {1}", request.RequestId, reason));
                return new RejectedBuildResponse(reason);
            }
            if (string.IsNullOrEmpty(request.TempDirectory))
            {
                string reason = "Missing temp directory";
                this.Logger.Log(string.Format("Rejected: {0}: {1}", request.RequestId, reason));
                return new RejectedBuildResponse(reason);
            }
#nullable restore
            BuildPaths buildPaths = new(this.ClientDirectory, request.WorkingDirectory, this.SdkDirectory, request.TempDirectory);
            if (!this.TryCreateCompiler(in request, buildPaths, out CommonCompiler compiler))
            {
                string reason = "Cannot create compiler for language id " + request.Language;
                this.Logger.Log(string.Format("Rejected: {0}: {1}", request.RequestId, reason));
                return new RejectedBuildResponse(reason);
            }
            bool utf8Output = compiler.Arguments.Utf8Output;
            if (!this.CheckAnalyzers(request.WorkingDirectory, compiler.Arguments.AnalyzerReferences, out List<string> errorMessages))
            {
                this.Logger.Log(string.Format("Rejected: {0}: for analyer load issues {1}", request.RequestId, string.Join(";", errorMessages)));
                return new AnalyzerInconsistencyBuildResponse(new ReadOnlyCollection<string>(errorMessages));
            }
            this.Logger.Log(string.Format("Begin {0} {1} compiler run", request.RequestId, request.Language));
            try
            {
                TextWriter consoleOutput = new StringWriter(CultureInfo.InvariantCulture);
                int returnCode = compiler.Run(consoleOutput, cancellationToken);
                string output = consoleOutput.ToString();
                this.Logger.Log(string.Format("End {0} {1} compiler run\r\nReturn code: {2}\r\nOutput:\r\n{3}", request.RequestId, request.Language, returnCode, output));
                return new CompletedBuildResponse(returnCode, utf8Output, output);
            }
            catch (Exception ex)
            {
                this.Logger.LogException(ex, string.Format("Running compilation for {0}", request.RequestId));
                throw;
            }
        }

        BuildResponse ICompilerServerHost.RunCompilation(
            in RunRequest request,
            CancellationToken cancellationToken)
        {
            return this.RunCompilation(in request, cancellationToken);
        }
    }
}
