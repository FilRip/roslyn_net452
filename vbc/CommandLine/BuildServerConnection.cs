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

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class BuildServerConnection
    {
        internal const int TimeOutMsExistingProcess = 1000;
        internal const int TimeOutMsNewProcess = 20000;

        internal static bool IsCompilerServerSupported => BuildServerConnection.GetPipeNameForPath("") != null;

        public static Task<BuildResponse> RunServerCompilationAsync(
          Guid requestId,
          RequestLanguage language,
          string? sharedCompilationId,
          List<string> arguments,
          BuildPathsAlt buildPaths,
          string? keepAlive,
          string? libEnvVariable,
          ICompilerServerLogger logger,
          CancellationToken cancellationToken)
        {
            string? pipeName = sharedCompilationId ?? GetPipeNameForPath(buildPaths.ClientDirectory);
            return RunServerCompilationCoreAsync(requestId, language, arguments, buildPaths, pipeName, keepAlive, libEnvVariable, new int?(), new CreateServerFunc(TryCreateServerCore), logger, cancellationToken);
        }

        internal static async Task<BuildResponse> RunServerCompilationCoreAsync(
          Guid requestId,
          RequestLanguage language,
          List<string> arguments,
          BuildPathsAlt buildPaths,
          string? pipeName,
          string? keepAlive,
          string? libDirectory,
          int? timeoutOverride,
          CreateServerFunc createServerFunc,
          ICompilerServerLogger logger,
          CancellationToken cancellationToken)
        {
            if (pipeName == null)
                throw new ArgumentException(nameof(pipeName));
            if (buildPaths.TempDirectory == null)
                throw new ArgumentException(nameof(buildPaths));
            if (string.IsNullOrWhiteSpace(BuildProtocolConstants.GetCommitHash()))
                return new IncorrectHashBuildResponse();
            Task<NamedPipeClientStream?>? server = tryConnectToServer(pipeName, buildPaths, timeoutOverride, createServerFunc, logger, cancellationToken);
            if (server == null)
                return new RejectedBuildResponse("Failed to connect to server");
            using (NamedPipeClientStream? pipe = await server.ConfigureAwait(false))
                return pipe == null ? new RejectedBuildResponse("Failed to connect to server") : await TryCompileAsync(pipe, BuildRequest.Create(language, arguments, buildPaths.WorkingDirectory, buildPaths.TempDirectory, BuildProtocolConstants.GetCommitHash() ?? "", new Guid?(requestId), keepAlive, libDirectory), logger, cancellationToken).ConfigureAwait(false);

            static Task<NamedPipeClientStream?>? tryConnectToServer(
              string pipeName,
              BuildPathsAlt buildPaths,
              int? timeoutOverride,
              CreateServerFunc createServerFunc,
              ICompilerServerLogger logger,
              CancellationToken cancellationToken)
            {
                int currentManagedThreadId1 = Environment.CurrentManagedThreadId;
                string clientDirectory = buildPaths.ClientDirectory;
                int timeoutMs1 = timeoutOverride ?? 20000;
                int num1 = timeoutOverride ?? 1000;
                Task<NamedPipeClientStream?>? server = null;
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
                            if (!serverMutex.TryLock(timeoutMs1))
                                return null;
                        }
                        catch (AbandonedMutexException)
                        {
                        }
                    }
                    int num2 = WasServerMutexOpen(GetServerMutexName(pipeName)) ? 1 : 0;
                    int timeoutMs2 = num2 != 0 ? num1 : timeoutMs1;
                    if (num2 != 0 || createServerFunc(clientDirectory, pipeName, logger))
                        server = TryConnectToServerAsync(pipeName, timeoutMs2, logger, cancellationToken);
                    return server;
                }
                finally
                {
                    try
                    {
                        serverMutex?.Dispose();
                    }
                    catch (ApplicationException ex)
                    {
                        int currentManagedThreadId2 = Environment.CurrentManagedThreadId;
                        throw new Exception(string.Format("ReleaseMutex failed. WaitOne Id: {0} Release Id: {1}", currentManagedThreadId1, currentManagedThreadId2), ex);
                    }
                }
            }
        }

        private static async Task<BuildResponse> TryCompileAsync(
          NamedPipeClientStream pipeStream,
          BuildRequest request,
          ICompilerServerLogger logger,
          CancellationToken cancellationToken)
        {
            using (pipeStream)
            {
                try
                {
                    logger.Log(string.Format("Begin writing request for {0}", request.RequestId));
                    await request.WriteAsync(pipeStream, cancellationToken).ConfigureAwait(false);
                    logger.Log(string.Format("End writing request for {0}", request.RequestId));
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, string.Format("Error writing build request for {0}", request.RequestId));
                    return new RejectedBuildResponse("Error writing build request: " + ex.Message);
                }
                CancellationTokenSource serverCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                logger.Log(string.Format("Begin reading response for {0}", request.RequestId));
                Task<BuildResponse> responseTask = BuildResponse.ReadAsync(pipeStream, serverCts.Token);
                Task task1 = BuildServerConnection.MonitorDisconnectAsync(pipeStream, request.RequestId, logger, serverCts.Token);
                Task task2 = await Task.WhenAny(responseTask, task1).ConfigureAwait(false);
                logger.Log(string.Format("End reading response for {0}", request.RequestId));
                BuildResponse buildResponse;
                if (responseTask.IsCompleted)
                {
                    try
                    {
                        buildResponse = await responseTask.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex, string.Format("Reading response for {0}", request.RequestId));
                        buildResponse = new RejectedBuildResponse("Error reading response: " + ex.Message);
                    }
                }
                else
                {
                    logger.LogError(string.Format("Client disconnect for {0}", request.RequestId));
                    buildResponse = new RejectedBuildResponse("Client disconnected");
                }
                serverCts.Cancel();
                return buildResponse;
            }
        }

        internal static async Task MonitorDisconnectAsync(
          PipeStream pipeStream,
          Guid requestId,
          ICompilerServerLogger logger,
          CancellationToken cancellationToken = default)
        {
            byte[]? buffer = new byte[0];
            while (!cancellationToken.IsCancellationRequested)
            {
                if (pipeStream.IsConnected)
                {
                    try
                    {
                        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                        int num = await pipeStream.ReadAsync(buffer, 0, 0, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex, string.Format("Error poking pipe {0}.", requestId));
                    }
                }
                else
                {
                    buffer = null;
                    return;
                }
            }
            buffer = null;
        }

        internal static async Task<NamedPipeClientStream?> TryConnectToServerAsync(
          string pipeName,
          int timeoutMs,
          ICompilerServerLogger logger,
          CancellationToken cancellationToken)
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
                    await Task.Run(() => pipeStream.Connect(timeoutMs), cancellationToken).ConfigureAwait(false);
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

        internal static (string processFilePath, string commandLineArguments, string toolFilePath) GetServerProcessInfo(
          string clientDir,
          string pipeName)
        {
            return RuntimeHostInfo.GetProcessInfo(Path.Combine(clientDir, "VBCSCompiler"), "\"-pipename:" + pipeName + "\"");
        }

        internal static bool TryCreateServerCore(
          string clientDir,
          string pipeName,
          ICompilerServerLogger logger)
        {
            (string processFilePath, string commandLineArguments, string toolFilePath) = GetServerProcessInfo(clientDir, pipeName);
            if (!File.Exists(toolFilePath))
                return false;
            if (PlatformInformation.IsWindows)
            {
                STARTUPINFO lpStartupInfo = new();
                lpStartupInfo.cb = Marshal.SizeOf<STARTUPINFO>(lpStartupInfo);
                lpStartupInfo.hStdError = NativeMethods.InvalidIntPtr;
                lpStartupInfo.hStdInput = NativeMethods.InvalidIntPtr;
                lpStartupInfo.hStdOutput = NativeMethods.InvalidIntPtr;
                lpStartupInfo.dwFlags = 256;
                uint dwCreationFlags = 134217760;
                logger.Log("Attempting to create process '{0}'", (object)processFilePath);
                bool process = NativeMethods.CreateProcess(null, new StringBuilder("\"" + processFilePath + "\" " + commandLineArguments), NativeMethods.NullPtr, NativeMethods.NullPtr, false, dwCreationFlags, NativeMethods.NullPtr, clientDir, ref lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
                if (process)
                {
                    logger.Log("Successfully created process with process id {0}", (object)lpProcessInformation.dwProcessId);
                    NativeMethods.CloseHandle(lpProcessInformation.hProcess);
                    NativeMethods.CloseHandle(lpProcessInformation.hThread);
                }
                else
                    logger.LogError("Failed to create process. GetLastError={0}", (object)Marshal.GetLastWin32Error());
                return process;
            }
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = processFilePath,
                    Arguments = commandLineArguments,
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
                isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            string userName = Environment.UserName;
            return userName == null ? null : BuildServerConnection.GetPipeName(userName, isAdmin, compilerExeDirectory);
        }

        internal static string GetPipeName(string userName, bool isAdmin, string compilerExeDirectory)
        {
            compilerExeDirectory = compilerExeDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string s = string.Format("{0}.{1}.{2}", userName, isAdmin, compilerExeDirectory);
            using SHA256 shA256 = SHA256.Create();
            return Convert.ToBase64String(shA256.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("/", "_").Replace("=", string.Empty);
        }

        internal static bool WasServerMutexOpen(string mutexName)
        {
            try
            {
                if (!PlatformInformation.IsRunningOnMono)
                    return ServerNamedMutex.WasOpen(mutexName);
                IServerMutex? serverMutex = null;
                bool createdNew = false;
                try
                {
                    serverMutex = new ServerFileMutexPair(mutexName, false, out createdNew);
                    return !createdNew;
                }
                finally
                {
                    serverMutex?.Dispose();
                }
            }
            catch
            {
                return false;
            }
        }

        internal static IServerMutex OpenOrCreateMutex(string name, out bool createdNew) => PlatformInformation.IsRunningOnMono ? new ServerFileMutexPair(name, true, out createdNew) : new ServerNamedMutex(name, out createdNew);

        internal static string GetServerMutexName(string pipeName) => pipeName + ".server";

        internal static string GetClientMutexName(string pipeName) => pipeName + ".client";

        public static string? GetTempPath(string? workingDir)
        {
            if (PlatformInformation.IsUnix)
                return Path.GetTempPath();
            string environmentVariable1 = Environment.GetEnvironmentVariable("TMP");
            if (Path.IsPathRooted(environmentVariable1))
                return environmentVariable1;
            string environmentVariable2 = Environment.GetEnvironmentVariable("TEMP");
            if (Path.IsPathRooted(environmentVariable2))
                return environmentVariable2;
            if (!string.IsNullOrEmpty(workingDir))
            {
                if (!string.IsNullOrEmpty(environmentVariable1))
                    return Path.Combine(workingDir, environmentVariable1);
                if (!string.IsNullOrEmpty(environmentVariable2))
                    return Path.Combine(workingDir, environmentVariable2);
            }
            string environmentVariable3 = Environment.GetEnvironmentVariable("USERPROFILE");
            return Path.IsPathRooted(environmentVariable3) ? environmentVariable3 : Environment.GetEnvironmentVariable("SYSTEMROOT");
        }
    }
}
