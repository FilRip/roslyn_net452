using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class BuildServerController
    {
        internal const string KeepAliveSettingName = "keepalive";
        private readonly NameValueCollection _appSettings;
        private readonly ICompilerServerLogger _logger;

        internal BuildServerController(NameValueCollection appSettings, ICompilerServerLogger logger)
        {
            this._appSettings = appSettings;
            this._logger = logger;
        }

        internal int Run(string[] args)
        {
#nullable restore
            if (!BuildServerController.ParseCommandLine(args, out string pipeName1, out bool shutdown))
                return 1;
            string str = pipeName1 ?? BuildServerController.GetDefaultPipeName();
            if (str == null)
                throw new Exception("Cannot calculate pipe name");
            CancellationTokenSource cancellationTokenSource = new();
            Console.CancelKeyPress += (sender, e) => cancellationTokenSource.Cancel();
            if (!shutdown)
            {
                string pipeName2 = str;
                CancellationToken token = cancellationTokenSource.Token;
                TimeSpan? keepAlive = new();
                CancellationToken cancellationToken = token;
                return this.RunServer(pipeName2, keepAlive: keepAlive, cancellationToken: cancellationToken);
            }
            string pipeName3 = str;
            CancellationToken token1 = cancellationTokenSource.Token;
            TimeSpan? timeout = new();
            CancellationToken cancellationToken1 = token1;
            return this.RunShutdown(pipeName3, timeout: timeout, cancellationToken: cancellationToken1);
        }

        internal TimeSpan? GetKeepAliveTimeout()
        {
            try
            {
                if (!int.TryParse(this._appSettings["keepalive"], NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) || result < 0)
                    return new TimeSpan?(ServerDispatcher.DefaultServerKeepAlive);
                return result == 0 ? new TimeSpan?() : new TimeSpan?(TimeSpan.FromSeconds(result));
            }
            catch (Exception ex)
            {
                this._logger.LogException(ex, "Could not read AppSettings");
                return new TimeSpan?(ServerDispatcher.DefaultServerKeepAlive);
            }
        }

        // False warning
#pragma warning disable IDE0090
        private static bool? WasServerRunning(string pipeName) => new bool?(BuildServerConnection.WasServerMutexOpen(BuildServerConnection.GetServerMutexName(pipeName)));
#pragma warning restore IDE0090

        internal static IClientConnectionHost CreateClientConnectionHost(
            string pipeName,
            ICompilerServerLogger logger)
        {
            return new NamedPipeClientConnectionHost(pipeName, logger);
        }

        internal static ICompilerServerHost CreateCompilerServerHost(
            ICompilerServerLogger logger)
        {
            return new CompilerServerHost(AppDomain.CurrentDomain.BaseDirectory, BuildClient.GetSystemSdkDirectory(), logger);
        }

#nullable enable

        private async Task<Stream?> ConnectForShutdownAsync(string pipeName, int timeout) => await BuildServerConnection.TryConnectToServerAsync(pipeName, timeout, this._logger, new CancellationToken()).ConfigureAwait(false);

        private static string? GetDefaultPipeName() => BuildServerConnection.GetPipeNameForPath(AppDomain.CurrentDomain.BaseDirectory);

        internal int RunServer(
            string pipeName,
            ICompilerServerHost? compilerServerHost = null,
            IClientConnectionHost? clientConnectionHost = null,
            IDiagnosticListener? listener = null,
            TimeSpan? keepAlive = null,
            CancellationToken cancellationToken = default)
        {
            if (!keepAlive.HasValue)
                keepAlive = this.GetKeepAliveTimeout();
            if (listener == null)
                listener = new EmptyDiagnosticListener();
            if (compilerServerHost == null)
                compilerServerHost = CreateCompilerServerHost(this._logger);
            if (clientConnectionHost == null)
                clientConnectionHost = CreateClientConnectionHost(pipeName, this._logger);
            using (BuildServerConnection.OpenOrCreateMutex(BuildServerConnection.GetServerMutexName(pipeName), out bool createdNew))
            {
                if (!createdNew)
                    return 1;
                compilerServerHost.Logger.Log("Keep alive timeout is: {0} milliseconds.", (object)(keepAlive.HasValue ? keepAlive.GetValueOrDefault().TotalMilliseconds : 0.0));
                FatalError.Handler = new Action<Exception>(FailFast.OnFatalException);
                new ServerDispatcher(compilerServerHost, clientConnectionHost, listener).ListenAndDispatchConnections(keepAlive, cancellationToken);
                return 0;
            }
        }

        internal static int CreateAndRunServer(
            string pipeName,
            ICompilerServerHost? compilerServerHost = null,
            IClientConnectionHost? clientConnectionHost = null,
            IDiagnosticListener? listener = null,
            TimeSpan? keepAlive = null,
            NameValueCollection? appSettings = null,
            ICompilerServerLogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            if (appSettings == null)
                appSettings = new NameValueCollection();
            if (logger == null)
                logger = EmptyCompilerServerLogger.Instance;
            return new BuildServerController(appSettings, logger).RunServer(pipeName, compilerServerHost, clientConnectionHost, listener, keepAlive, cancellationToken);
        }

        internal int RunShutdown(
            string pipeName,
            bool waitForProcess = true,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            return this.RunShutdownAsync(pipeName, waitForProcess, timeout, cancellationToken).GetAwaiter().GetResult();
        }

        internal async Task<int> RunShutdownAsync(
            string pipeName,
            bool waitForProcess = true,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            bool? nullable1 = WasServerRunning(pipeName);
            bool flag1 = false;
            if (nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue)
                return 0;
            try
            {
                int timeout1 = timeout.HasValue ? (int)timeout.Value.TotalMilliseconds : -1;
#nullable restore
                using Stream client = await this.ConnectForShutdownAsync(pipeName, timeout1).ConfigureAwait(false);
                if (client != null)
                {
                    await BuildRequest.CreateShutdown().WriteAsync(client, cancellationToken).ConfigureAwait(false);
                    ShutdownBuildResponse shutdownBuildResponse = (ShutdownBuildResponse)await BuildResponse.ReadAsync(client, cancellationToken).ConfigureAwait(false);
                    if (waitForProcess)
                    {
                        try
                        {
                            Process.GetProcessById(shutdownBuildResponse.ServerProcessId).WaitForExit();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                bool? nullable2 = BuildServerController.WasServerRunning(pipeName);
                bool flag2 = false;
                return !(nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue) ? 1 : 0;
            }
        }

#nullable enable

        internal static bool ParseCommandLine(string[] args, out string? pipeName, out bool shutdown)
        {
            pipeName = null;
            shutdown = false;
            foreach (string str in args)
            {
                if (str.StartsWith("-pipename:", StringComparison.Ordinal))
                {
                    pipeName = str.Substring("-pipename:".Length);
                }
                else
                {
                    if (!(str == "-shutdown"))
                        return false;
                    shutdown = true;
                }
            }
            return true;
        }
    }
}
