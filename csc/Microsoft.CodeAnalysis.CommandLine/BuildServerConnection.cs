using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class BuildServerConnection
	{
		internal const int TimeOutMsExistingProcess = 1000;

		internal const int TimeOutMsNewProcess = 20000;

		internal static bool IsCompilerServerSupported => GetPipeNameForPath("") != null;

		public static Task<BuildResponse> RunServerCompilationAsync(Guid requestId, RequestLanguage language, string? sharedCompilationId, List<string> arguments, BuildPathsAlt buildPaths, string? keepAlive, string? libEnvVariable, ICompilerServerLogger logger, CancellationToken cancellationToken)
		{
			string? pipeName = sharedCompilationId ?? GetPipeNameForPath(buildPaths.ClientDirectory);
			return RunServerCompilationCoreAsync(requestId, language, arguments, buildPaths, pipeName, keepAlive, libEnvVariable, null, TryCreateServerCore, logger, cancellationToken);
		}

		internal static async Task<BuildResponse> RunServerCompilationCoreAsync(Guid requestId, RequestLanguage language, List<string> arguments, BuildPathsAlt buildPaths, string? pipeName, string? keepAlive, string? libDirectory, int? timeoutOverride, CreateServerFunc createServerFunc, ICompilerServerLogger logger, CancellationToken cancellationToken)
		{
			if (pipeName == null)
			{
				throw new ArgumentException("pipeName");
			}
			if (buildPaths.TempDirectory == null)
			{
				throw new ArgumentException("buildPaths");
			}
			if (string.IsNullOrWhiteSpace(BuildProtocolConstants.GetCommitHash()))
			{
				return new IncorrectHashBuildResponse();
			}
			Task<NamedPipeClientStream?>? task = tryConnectToServer(pipeName, buildPaths, timeoutOverride, createServerFunc, logger, cancellationToken);
			if (task == null)
			{
				return new RejectedBuildResponse("Failed to connect to server");
			}
			using (NamedPipeClientStream? pipe = await task.ConfigureAwait(continueOnCapturedContext: false))
			{
				if (pipe == null)
				{
					return new RejectedBuildResponse("Failed to connect to server");
				}
				BuildRequest request = BuildRequest.Create(language, arguments, buildPaths.WorkingDirectory, buildPaths.TempDirectory, BuildProtocolConstants.GetCommitHash() ?? "", requestId, keepAlive, libDirectory);
				return await TryCompileAsync(pipe, request, logger, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			static Task<NamedPipeClientStream?>? tryConnectToServer(string pipeName, BuildPathsAlt buildPaths, int? timeoutOverride, CreateServerFunc createServerFunc, ICompilerServerLogger logger, CancellationToken cancellationToken)
			{
				int currentManagedThreadId = Environment.CurrentManagedThreadId;
				string clientDirectory = buildPaths.ClientDirectory;
				int num = timeoutOverride ?? 20000;
				int num2 = timeoutOverride ?? 1000;
				Task<NamedPipeClientStream?>? result = null;
				IServerMutex? serverMutex = null;
				try
				{
					bool createdNew = false;
					try
					{
						serverMutex = OpenOrCreateMutex(GetClientMutexName(pipeName), out createdNew);
					}
					catch
					{
						return null;
					}
					if (!createdNew)
					{
						try
						{
							if (!serverMutex.TryLock(num))
							{
								return null;
							}
						}
						catch (AbandonedMutexException)
						{
							createdNew = true;
						}
					}
					bool num3 = WasServerMutexOpen(GetServerMutexName(pipeName));
					int timeoutMs = (num3 ? num2 : num);
					if (num3 || createServerFunc(clientDirectory, pipeName, logger))
					{
						result = TryConnectToServerAsync(pipeName, timeoutMs, logger, cancellationToken);
					}
					return result;
				}
				finally
				{
					try
					{
						serverMutex?.Dispose();
					}
					catch (ApplicationException innerException)
					{
						int currentManagedThreadId2 = Environment.CurrentManagedThreadId;
						throw new Exception($"ReleaseMutex failed. WaitOne Id: {currentManagedThreadId} Release Id: {currentManagedThreadId2}", innerException);
					}
				}
			}
		}

		private static async Task<BuildResponse> TryCompileAsync(NamedPipeClientStream pipeStream, BuildRequest request, ICompilerServerLogger logger, CancellationToken cancellationToken)
		{
			using (pipeStream)
			{
				try
				{
					logger.Log($"Begin writing request for {request.RequestId}");
					await request.WriteAsync(pipeStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					logger.Log($"End writing request for {request.RequestId}");
				}
				catch (Exception ex)
				{
					logger.LogException(ex, $"Error writing build request for {request.RequestId}");
					return new RejectedBuildResponse("Error writing build request: " + ex.Message);
				}
				CancellationTokenSource serverCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				logger.Log($"Begin reading response for {request.RequestId}");
				Task<BuildResponse> responseTask = BuildResponse.ReadAsync(pipeStream, serverCts.Token);
				Task task = MonitorDisconnectAsync(pipeStream, request.RequestId, logger, serverCts.Token);
				await Task.WhenAny(responseTask, task).ConfigureAwait(continueOnCapturedContext: false);
				logger.Log($"End reading response for {request.RequestId}");
				BuildResponse result;
				if (responseTask.IsCompleted)
				{
					try
					{
						result = await responseTask.ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex2)
					{
						logger.LogException(ex2, $"Reading response for {request.RequestId}");
						result = new RejectedBuildResponse("Error reading response: " + ex2.Message);
					}
				}
				else
				{
					logger.LogError($"Client disconnect for {request.RequestId}");
					result = new RejectedBuildResponse("Client disconnected");
				}
				serverCts.Cancel();
				return result;
			}
		}

		internal static async Task MonitorDisconnectAsync(PipeStream pipeStream, Guid requestId, ICompilerServerLogger logger, CancellationToken cancellationToken = default)
		{
			byte[] buffer = new byte[0];
			while (!cancellationToken.IsCancellationRequested && pipeStream.IsConnected)
			{
				try
				{
					await Task.Delay(100, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					await pipeStream.ReadAsync(buffer, 0, 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception exception)
				{
					logger.LogException(exception, $"Error poking pipe {requestId}.");
				}
			}
		}

		internal static async Task<NamedPipeClientStream?> TryConnectToServerAsync(string pipeName, int timeoutMs, ICompilerServerLogger logger, CancellationToken cancellationToken)
		{
			NamedPipeClientStream? pipeStream = null;
			try
			{
				logger.Log("Attempt to open named pipe '{0}'", (object)pipeName);
				pipeStream = NamedPipeUtil.CreateClient(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
				cancellationToken.ThrowIfCancellationRequested();
				logger.Log("Attempt to connect named pipe '{0}'", (object)pipeName);
				try
				{
					await Task.Run(() => pipeStream.Connect(timeoutMs)).ConfigureAwait(false);
				}
				catch (Exception ex) when (ex is IOException || ex is TimeoutException)
				{
					logger.LogException(ex, string.Format("Connecting to server timed out after {0} ms", timeoutMs));
					pipeStream.Dispose();
					return null;
				}
				logger.Log("Named pipe '{0}' connected", (object)pipeName);
				cancellationToken.ThrowIfCancellationRequested();
				if (NamedPipeUtil.CheckPipeConnectionOwnership(pipeStream))
					return pipeStream;
				pipeStream.Dispose();
				logger.LogError("Owner of named pipe is incorrect");
				return null;
			}
			catch (Exception ex) when (!(ex is TaskCanceledException) && !(ex is OperationCanceledException))
			{
				logger.LogException(ex, "Exception while connecting to process");
				pipeStream?.Dispose();
				return null;
			}
		}

		internal static (string processFilePath, string commandLineArguments, string toolFilePath) GetServerProcessInfo(string clientDir, string pipeName)
		{
			string toolFilePathWithoutExtension = Path.Combine(clientDir, "VBCSCompiler");
			string commandLineArguments = "\"-pipename:" + pipeName + "\"";
			return RuntimeHostInfo.GetProcessInfo(toolFilePathWithoutExtension, commandLineArguments);
		}

		internal static bool TryCreateServerCore(string clientDir, string pipeName, ICompilerServerLogger logger)
		{
			(string, string, string) serverProcessInfo = GetServerProcessInfo(clientDir, pipeName);
			if (!File.Exists(serverProcessInfo.Item3))
			{
				return false;
			}
			if (PlatformInformation.IsWindows)
			{
				STARTUPINFO lpStartupInfo = default;
				lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
				lpStartupInfo.hStdError = NativeMethods.InvalidIntPtr;
				lpStartupInfo.hStdInput = NativeMethods.InvalidIntPtr;
				lpStartupInfo.hStdOutput = NativeMethods.InvalidIntPtr;
				lpStartupInfo.dwFlags = 256;
				uint dwCreationFlags = 134217760u;
				logger.Log("Attempting to create process '{0}'", serverProcessInfo.Item1);
				StringBuilder lpCommandLine = new("\"" + serverProcessInfo.Item1 + "\" " + serverProcessInfo.Item2);
                bool flag = NativeMethods.CreateProcess(null, lpCommandLine, NativeMethods.NullPtr, NativeMethods.NullPtr, bInheritHandles: false, dwCreationFlags, NativeMethods.NullPtr, clientDir, ref lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
                if (flag)
				{
					logger.Log("Successfully created process with process id {0}", lpProcessInformation.dwProcessId);
					NativeMethods.CloseHandle(lpProcessInformation.hProcess);
					NativeMethods.CloseHandle(lpProcessInformation.hThread);
				}
				else
				{
					logger.LogError("Failed to create process. GetLastError={0}", Marshal.GetLastWin32Error());
				}
				return flag;
			}
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = serverProcessInfo.Item1,
					Arguments = serverProcessInfo.Item2,
					UseShellExecute = false,
					WorkingDirectory = clientDir,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				});
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal static string? GetPipeNameForPath(string compilerExeDirectory)
		{
			bool isAdmin = false;
			if (PlatformInformation.IsWindows)
			{
				isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
			}
			string userName = Environment.UserName;
			if (userName == null)
			{
				return null;
			}
			return GetPipeName(userName, isAdmin, compilerExeDirectory);
		}

		internal static string GetPipeName(string userName, bool isAdmin, string compilerExeDirectory)
		{
			compilerExeDirectory = compilerExeDirectory.TrimEnd(Path.DirectorySeparatorChar);
			string s = $"{userName}.{isAdmin}.{compilerExeDirectory}";
			using SHA256 sHA = SHA256.Create();
			return Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("/", "_").Replace("=", string.Empty);
		}

		internal static bool WasServerMutexOpen(string mutexName)
		{
			try
			{
				if (PlatformInformation.IsRunningOnMono)
				{
					IServerMutex? serverMutex = null;
					bool createdNew = false;
					try
					{
						serverMutex = new ServerFileMutexPair(mutexName, initiallyOwned: false, out createdNew);
						return !createdNew;
					}
					finally
					{
						serverMutex?.Dispose();
					}
				}
				return ServerNamedMutex.WasOpen(mutexName);
			}
			catch
			{
				return false;
			}
		}

		internal static IServerMutex OpenOrCreateMutex(string name, out bool createdNew)
		{
			if (PlatformInformation.IsRunningOnMono)
			{
				return new ServerFileMutexPair(name, initiallyOwned: true, out createdNew);
			}
			return new ServerNamedMutex(name, out createdNew);
		}

		internal static string GetServerMutexName(string pipeName)
		{
			return pipeName + ".server";
		}

		internal static string GetClientMutexName(string pipeName)
		{
			return pipeName + ".client";
		}

		public static string? GetTempPath(string? workingDir)
		{
			if (PlatformInformation.IsUnix)
			{
				return Path.GetTempPath();
			}
			string environmentVariable = Environment.GetEnvironmentVariable("TMP");
			if (Path.IsPathRooted(environmentVariable))
			{
				return environmentVariable;
			}
			string environmentVariable2 = Environment.GetEnvironmentVariable("TEMP");
			if (Path.IsPathRooted(environmentVariable2))
			{
				return environmentVariable2;
			}
			if (!string.IsNullOrEmpty(workingDir))
			{
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					return Path.Combine(workingDir, environmentVariable);
				}
				if (!string.IsNullOrEmpty(environmentVariable2))
				{
					return Path.Combine(workingDir, environmentVariable2);
				}
			}
			string environmentVariable3 = Environment.GetEnvironmentVariable("USERPROFILE");
			if (Path.IsPathRooted(environmentVariable3))
			{
				return environmentVariable3;
			}
			return Environment.GetEnvironmentVariable("SYSTEMROOT");
		}
	}
}
