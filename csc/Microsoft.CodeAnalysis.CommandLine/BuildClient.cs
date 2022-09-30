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

using SystemExtensions;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class BuildClient
	{
		private readonly RequestLanguage _language;

		private readonly CompileFunc _compileFunc;

		private readonly ICompilerServerLogger _logger;

		private readonly CreateServerFunc _createServerFunc;

		private readonly int? _timeoutOverride;

		internal static bool IsRunningOnWindows => Path.DirectorySeparatorChar == '\\';

		internal BuildClient(RequestLanguage language, CompileFunc compileFunc, ICompilerServerLogger logger, CreateServerFunc createServerFunc = null, int? timeoutOverride = null)
		{
			_language = language;
			_compileFunc = compileFunc;
			_logger = logger;
			_createServerFunc = createServerFunc ?? new CreateServerFunc(BuildServerConnection.TryCreateServerCore);
			_timeoutOverride = timeoutOverride;
		}

		public static string GetSystemSdkDirectory()
		{
			if (!RuntimeHostInfo.IsCoreClrRuntime)
			{
				return RuntimeEnvironment.GetRuntimeDirectory();
			}
			return null;
		}

		internal static int Run(IEnumerable<string> arguments, RequestLanguage language, CompileFunc compileFunc, ICompilerServerLogger logger, Guid? requestId = null)
		{
			string systemSdkDirectory = GetSystemSdkDirectory();
			if (RuntimeHostInfo.IsCoreClrRuntime)
			{
				ExtensionsEncoding.Encoding_RegisterProvider(CodePagesEncodingProvider.Instance);
			}
			BuildClient buildClient = new BuildClient(language, compileFunc, logger);
			string baseDirectory = AppContext.BaseDirectory;
			string currentDirectory = Directory.GetCurrentDirectory();
			string tempPath = BuildServerConnection.GetTempPath(currentDirectory);
			BuildPaths buildPaths = new BuildPaths(baseDirectory, currentDirectory, systemSdkDirectory, tempPath);
			IEnumerable<string> commandLineArgs = GetCommandLineArgs(arguments);
			return buildClient.RunCompilation(commandLineArgs, buildPaths, null, null, requestId).ExitCode;
		}

		internal RunCompilationResult RunCompilation(IEnumerable<string> originalArguments, BuildPaths buildPaths, TextWriter textWriter = null, string pipeName = null, Guid? requestId = null)
		{
			textWriter = textWriter ?? Console.Out;
			if (CommandLineParser.TryParseClientArgs(originalArguments.Select((string arg) => arg.Trim()).ToArray(), out var parsedArgs, out var containsShared, out var keepAliveValue, out var pipeName2, out var errorMessage))
			{
				if (pipeName == null)
				{
					pipeName = pipeName2;
				}
				if (containsShared)
				{
					pipeName = pipeName ?? GetPipeName(buildPaths);
					string environmentVariable = Environment.GetEnvironmentVariable("LIB");
					RunCompilationResult? runCompilationResult = RunServerCompilation(textWriter, parsedArgs, buildPaths, environmentVariable, pipeName, keepAliveValue, requestId);
					if (runCompilationResult.HasValue)
					{
						return runCompilationResult.Value;
					}
				}
				return new RunCompilationResult(RunLocalCompilation(parsedArgs.ToArray(), buildPaths, textWriter));
			}
			textWriter.WriteLine(errorMessage);
			return RunCompilationResult.Failed;
		}

		private static bool TryEnableMulticoreJitting(out string errorMessage)
		{
			errorMessage = null;
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoslynCompiler", "ProfileOptimization");
				AssemblyName name = Assembly.GetExecutingAssembly().GetName();
				string profile = name.Name + name.Version?.ToString() + ".profile";
				Directory.CreateDirectory(text);
				ProfileOptimization.SetProfileRoot(text);
				ProfileOptimization.StartProfile(profile);
			}
			catch (Exception ex)
			{
				errorMessage = string.Format(CodeAnalysisResources.ExceptionEnablingMulticoreJit, ex.Message);
				return false;
			}
			return true;
		}

		public Task<RunCompilationResult> RunCompilationAsync(IEnumerable<string> originalArguments, BuildPaths buildPaths, TextWriter textWriter = null)
		{
			TaskCompletionSource<RunCompilationResult> tcs = new TaskCompletionSource<RunCompilationResult>();
			new Thread((ThreadStart)delegate
			{
				try
				{
					RunCompilationResult result = RunCompilation(originalArguments, buildPaths, textWriter);
					tcs.SetResult(result);
				}
				catch (Exception exception)
				{
					tcs.SetException(exception);
				}
			}).Start();
			return tcs.Task;
		}

		private int RunLocalCompilation(string[] arguments, BuildPaths buildPaths, TextWriter textWriter)
		{
			DefaultAnalyzerAssemblyLoader analyzerAssemblyLoader = new DefaultAnalyzerAssemblyLoader();
			return _compileFunc(arguments, buildPaths, textWriter, analyzerAssemblyLoader);
		}

		private RunCompilationResult? RunServerCompilation(TextWriter textWriter, List<string> arguments, BuildPaths buildPaths, string libDirectory, string pipeName, string keepAlive, Guid? requestId)
		{
			if (!AreNamedPipesSupported())
			{
				return null;
			}
			BuildResponse result;
			try
			{
				BuildPathsAlt buildPaths2 = new BuildPathsAlt(buildPaths.ClientDirectory, buildPaths.WorkingDirectory, buildPaths.SdkDirectory, buildPaths.TempDirectory);
				result = BuildServerConnection.RunServerCompilationCoreAsync(requestId ?? Guid.NewGuid(), _language, arguments, buildPaths2, pipeName, keepAlive, libDirectory, _timeoutOverride, _createServerFunc, _logger, CancellationToken.None).Result;
				if (result == null)
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
			switch (result.Type)
			{
			case BuildResponse.ResponseType.Completed:
			{
				CompletedBuildResponse completedResponse = (CompletedBuildResponse)result;
				return ConsoleUtil.RunWithUtf8Output(completedResponse.Utf8Output, textWriter, delegate(TextWriter tw)
				{
					tw.Write(completedResponse.Output);
					return new RunCompilationResult(completedResponse.ReturnCode, ranOnServer: true);
				});
			}
			case BuildResponse.ResponseType.MismatchedVersion:
			case BuildResponse.ResponseType.AnalyzerInconsistency:
			case BuildResponse.ResponseType.Rejected:
			case BuildResponse.ResponseType.IncorrectHash:
				return null;
			default:
				return null;
			}
		}

		private static string GetPipeName(BuildPaths buildPaths)
		{
			return BuildServerConnection.GetPipeNameForPath(buildPaths.ClientDirectory);
		}

		private static IEnumerable<string> GetCommandLineArgs(IEnumerable<string> args)
		{
			if (UseNativeArguments())
			{
				return GetCommandLineWindows(args);
			}
			return args;
		}

		private static bool UseNativeArguments()
		{
			if (!IsRunningOnWindows)
			{
				return false;
			}
			if (PlatformInformation.IsRunningOnMono)
			{
				return false;
			}
			if (RuntimeHostInfo.IsCoreClrRuntime)
			{
				return false;
			}
			return true;
		}

		private static bool AreNamedPipesSupported()
		{
			if (!PlatformInformation.IsRunningOnMono)
			{
				return true;
			}
			IDisposable disposable = null;
			try
			{
				string pipeName = $"mono-{Guid.NewGuid()}";
				disposable = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
				disposable.Dispose();
				return true;
			}
			catch (PlatformNotSupportedException)
			{
				if (disposable != null)
				{
					GC.SuppressFinalize(disposable);
				}
				return false;
			}
		}

		private static IEnumerable<string> GetCommandLineWindows(IEnumerable<string> args)
		{
			IntPtr commandLine = NativeMethods.GetCommandLine();
			if (commandLine == IntPtr.Zero)
			{
				return args;
			}
			return CommandLineParser.SplitCommandLineIntoArguments(Marshal.PtrToStringUni(commandLine), removeHashComments: false).Skip(1);
		}
	}
}
