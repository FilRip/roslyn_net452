using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Roslyn.Utilities;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class BuildClient
    {
        private readonly RequestLanguage _language;
        private readonly
#nullable disable
    CompileFunc _compileFunc;
        private readonly ICompilerServerLogger _logger;
        private readonly CreateServerFunc _createServerFunc;
        private readonly int? _timeoutOverride;

        internal static bool IsRunningOnWindows => Path.DirectorySeparatorChar == '\\';

        internal BuildClient(
          RequestLanguage language,
          CompileFunc compileFunc,
          ICompilerServerLogger logger,
          CreateServerFunc createServerFunc = null,
          int? timeoutOverride = null)
        {
            this._language = language;
            this._compileFunc = compileFunc;
            this._logger = logger;
            this._createServerFunc = createServerFunc ?? new CreateServerFunc(BuildServerConnection.TryCreateServerCore);
            this._timeoutOverride = timeoutOverride;
        }

        public static string GetSystemSdkDirectory() => !RuntimeHostInfo.IsCoreClrRuntime ? RuntimeEnvironment.GetRuntimeDirectory() : null;

        internal static int Run(
          IEnumerable<string> arguments,
          RequestLanguage language,
          CompileFunc compileFunc,
          ICompilerServerLogger logger,
          Guid? requestId = null)
        {
            string systemSdkDirectory = BuildClient.GetSystemSdkDirectory();
            if (RuntimeHostInfo.IsCoreClrRuntime)
                SystemExtensions.ExtensionsEncoding.Encoding_RegisterProvider(CodePagesEncodingProvider.Instance);
            BuildClient buildClient = new(language, compileFunc, logger);
            string baseDirectory = AppContext.BaseDirectory;
            string currentDirectory = Directory.GetCurrentDirectory();
            string tempPath = BuildServerConnection.GetTempPath(currentDirectory);
            BuildPaths buildPaths1 = new(baseDirectory, currentDirectory, systemSdkDirectory, tempPath);
            IEnumerable<string> commandLineArgs = GetCommandLineArgs(arguments);
            BuildPaths buildPaths2 = buildPaths1;
            Guid? requestId1 = requestId;
            return buildClient.RunCompilation(commandLineArgs, buildPaths2, requestId: requestId1).ExitCode;
        }

        internal RunCompilationResult RunCompilation(
          IEnumerable<string> originalArguments,
          BuildPaths buildPaths,
          TextWriter textWriter = null,
          string pipeName = null,
          Guid? requestId = null)
        {
            textWriter ??= Console.Out;
            if (CommandLineParser.TryParseClientArgs(originalArguments.Select<string, string>(arg => arg.Trim()).ToArray<string>(), out List<string> parsedArgs, out bool containsShared, out string keepAliveValue, out string pipeName1, out string errorMessage))
            {
                if (pipeName == null)
                    pipeName = pipeName1;
                if (containsShared)
                {
                    pipeName ??= GetPipeName(buildPaths);
                    string environmentVariable = Environment.GetEnvironmentVariable("LIB");
                    RunCompilationResult? nullable = this.RunServerCompilation(textWriter, parsedArgs, buildPaths, environmentVariable, pipeName, keepAliveValue, requestId);
                    if (nullable.HasValue)
                        return nullable.Value;
                }
                return new RunCompilationResult(this.RunLocalCompilation(parsedArgs.ToArray(), buildPaths, textWriter));
            }
            textWriter.WriteLine(errorMessage);
            return RunCompilationResult.Failed;
        }

        /*private static bool TryEnableMulticoreJitting(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoslynCompiler", "ProfileOptimization");
                AssemblyName name = Assembly.GetExecutingAssembly().GetName();
                string profile = name.Name + name.Version?.ToString() + ".profile";
                Directory.CreateDirectory(str);
                ProfileOptimization.SetProfileRoot(str);
                ProfileOptimization.StartProfile(profile);
            }
            catch (Exception ex)
            {
                errorMessage = string.Format(CodeAnalysisResources.ExceptionEnablingMulticoreJit, ex.Message);
                return false;
            }
            return true;
        }*/

        public Task<RunCompilationResult> RunCompilationAsync(
          IEnumerable<string> originalArguments,
          BuildPaths buildPaths,
          TextWriter textWriter = null)
        {
            TaskCompletionSource<RunCompilationResult> tcs = new();
            new Thread(() =>
           {
               try
               {
                   tcs.SetResult(this.RunCompilation(originalArguments, buildPaths, textWriter));
               }
               catch (Exception ex)
               {
                   tcs.SetException(ex);
               }
           }).Start();
            return tcs.Task;
        }

        private int RunLocalCompilation(
          string[] arguments,
          BuildPaths buildPaths,
          TextWriter textWriter)
        {
            DefaultAnalyzerAssemblyLoader analyzerAssemblyLoader = new();
            return this._compileFunc(arguments, buildPaths, textWriter, analyzerAssemblyLoader);
        }

        private RunCompilationResult? RunServerCompilation(
          TextWriter textWriter,
          List<string> arguments,
          BuildPaths buildPaths,
          string libDirectory,
          string pipeName,
          string keepAlive,
          Guid? requestId)
        {
            if (!BuildClient.AreNamedPipesSupported())
                return new RunCompilationResult?();
            BuildResponse result;
            try
            {
                BuildPathsAlt buildPaths1 = new(buildPaths.ClientDirectory, buildPaths.WorkingDirectory, buildPaths.SdkDirectory, buildPaths.TempDirectory);
                result = BuildServerConnection.RunServerCompilationCoreAsync(requestId ?? Guid.NewGuid(), this._language, arguments, buildPaths1, pipeName, keepAlive, libDirectory, this._timeoutOverride, this._createServerFunc, this._logger, CancellationToken.None).Result;
                if (result == null)
                    return new RunCompilationResult?();
            }
            catch (Exception)
            {
                return new RunCompilationResult?();
            }
            switch (result.Type)
            {
                case BuildResponse.ResponseType.MismatchedVersion:
                case BuildResponse.ResponseType.AnalyzerInconsistency:
                case BuildResponse.ResponseType.Rejected:
                case BuildResponse.ResponseType.IncorrectHash:
                    return new RunCompilationResult?();
                case BuildResponse.ResponseType.Completed:
                    CompletedBuildResponse completedResponse = (CompletedBuildResponse)result;
                    return new RunCompilationResult?(ConsoleUtil.RunWithUtf8Output<RunCompilationResult>(completedResponse.Utf8Output, textWriter, tw =>
                   {
                       tw.Write(completedResponse.Output);
                       return new RunCompilationResult(completedResponse.ReturnCode, true);
                   }));
                default:
                    return new RunCompilationResult?();
            }
        }

        private static string GetPipeName(BuildPaths buildPaths) => BuildServerConnection.GetPipeNameForPath(buildPaths.ClientDirectory);

        private static IEnumerable<string> GetCommandLineArgs(IEnumerable<string> args) => BuildClient.UseNativeArguments() ? BuildClient.GetCommandLineWindows(args) : args;

        private static bool UseNativeArguments() => BuildClient.IsRunningOnWindows && !PlatformInformation.IsRunningOnMono && !RuntimeHostInfo.IsCoreClrRuntime;

        private static bool AreNamedPipesSupported()
        {
            if (!PlatformInformation.IsRunningOnMono)
                return true;
            IDisposable disposable = null;
            try
            {
                disposable = new NamedPipeClientStream(".", string.Format("mono-{0}", Guid.NewGuid()), PipeDirection.InOut);
                disposable.Dispose();
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                if (disposable != null)
                    GC.SuppressFinalize(disposable);
                return false;
            }
        }

        private static IEnumerable<string> GetCommandLineWindows(IEnumerable<string> args)
        {
            IntPtr commandLine = NativeMethods.GetCommandLine();
            return commandLine == IntPtr.Zero ? args : CommandLineParser.SplitCommandLineIntoArguments(Marshal.PtrToStringUni(commandLine), false).Skip<string>(1);
        }
    }
}
