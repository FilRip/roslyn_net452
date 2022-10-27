using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class ClientConnectionHandler
    {
        internal ICompilerServerHost CompilerServerHost { get; }

        internal ICompilerServerLogger Logger => this.CompilerServerHost.Logger;

        internal ClientConnectionHandler(ICompilerServerHost compilerServerHost) => this.CompilerServerHost = compilerServerHost;

        internal async Task<CompletionData> ProcessAsync(
            Task<IClientConnection> clientConnectionTask,
            bool allowCompilationRequests = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await ProcessCore().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogException(ex, "Error processing request for client");
                return CompletionData.RequestError;
            }

            async Task<CompletionData> ProcessCore()
            {
                using IClientConnection clientConnection = await clientConnectionTask.ConfigureAwait(false);
                BuildRequest request = await clientConnection.ReadBuildRequestAsync(cancellationToken).ConfigureAwait(false);
                this.Logger.Log(string.Format("Received request {0} of type {1}", request.RequestId, request.GetType()));
                if (!string.Equals(request.CompilerHash, BuildProtocolConstants.GetCommitHash(), StringComparison.OrdinalIgnoreCase))
                    return await this.WriteBuildResponseAsync(clientConnection, request.RequestId, new IncorrectHashBuildResponse(), CompletionData.RequestError, cancellationToken).ConfigureAwait(false);
                if (request.Arguments.Count == 1 && request.Arguments[0].ArgumentId == BuildProtocolConstants.ArgumentId.Shutdown)
                    return await this.WriteBuildResponseAsync(clientConnection, request.RequestId, new ShutdownBuildResponse(Process.GetCurrentProcess().Id), new CompletionData(CompletionReason.RequestCompleted, shutdownRequested: true), cancellationToken).ConfigureAwait(false);
                if (!allowCompilationRequests)
                    return await this.WriteBuildResponseAsync(clientConnection, request.RequestId, new RejectedBuildResponse("Compilation not allowed at this time"), CompletionData.RequestCompleted, cancellationToken).ConfigureAwait(false);
                return !Environment.Is64BitProcess && !MemoryHelper.IsMemoryAvailable(this.Logger) ? await this.WriteBuildResponseAsync(clientConnection, request.RequestId, new RejectedBuildResponse("Not enough resources to accept connection"), CompletionData.RequestError, cancellationToken).ConfigureAwait(false) : await this.ProcessCompilationRequestAsync(clientConnection, request, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<CompletionData> WriteBuildResponseAsync(
            IClientConnection clientConnection,
            Guid requestId,
            BuildResponse response,
            CompletionData completionData,
            CancellationToken cancellationToken)
        {
            this.Logger.Log(response is not RejectedBuildResponse rejectedBuildResponse ? string.Format("Writing {0} response for {1}", response.Type, requestId) : string.Format("Writing {0} response '{1}' for {2}", rejectedBuildResponse.Type, rejectedBuildResponse.Reason, requestId));
            await clientConnection.WriteBuildResponseAsync(response, cancellationToken).ConfigureAwait(false);
            return completionData;
        }

        private async Task<CompletionData> ProcessCompilationRequestAsync(
            IClientConnection clientConnection,
            BuildRequest request,
            CancellationToken cancellationToken)
        {
            CancellationTokenSource buildCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task<BuildResponse> compilationTask = ProcessCompilationRequestCore(this.CompilerServerHost, request, buildCancellationTokenSource.Token);
            Task task = await Task.WhenAny(compilationTask, clientConnection.DisconnectTask).ConfigureAwait(false);
            try
            {
                if (!compilationTask.IsCompleted)
                    return CompletionData.RequestError;
                BuildResponse response;
                CompletionData completionData;
                try
                {
                    response = await compilationTask.ConfigureAwait(false);
                    completionData = !(response is AnalyzerInconsistencyBuildResponse) ? new CompletionData(CompletionReason.RequestCompleted, ClientConnectionHandler.CheckForNewKeepAlive(request)) : CompletionData.RequestError;
                }
                catch (Exception ex)
                {
                    this.Logger.LogException(ex, string.Format("Exception running compilation for {0}", request.RequestId));
                    response = new RejectedBuildResponse("Exception during compilation: " + ex.Message);
                    completionData = CompletionData.RequestError;
                }
                return await this.WriteBuildResponseAsync(clientConnection, request.RequestId, response, completionData, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                buildCancellationTokenSource.Cancel();
            }

            static Task<BuildResponse> ProcessCompilationRequestCore(
                ICompilerServerHost compilerServerHost,
                BuildRequest buildRequest,
                CancellationToken cancellationToken)
            {
                Task<BuildResponse> task = new(() =>
               {
                   RunRequest request1 = BuildProtocolUtil.GetRunRequest(buildRequest);
                   return compilerServerHost.RunCompilation(in request1, cancellationToken);
               }, cancellationToken, TaskCreationOptions.LongRunning);
                task.Start();
                return task;
            }
        }

        private static TimeSpan? CheckForNewKeepAlive(BuildRequest request)
        {
            TimeSpan? nullable = new();
            foreach (BuildRequest.Argument obj in request.Arguments)
            {
                if (obj.ArgumentId == BuildProtocolConstants.ArgumentId.KeepAlive && int.TryParse(obj.Value, out int result))
                    nullable = new TimeSpan?(TimeSpan.FromSeconds(result));
            }
            return nullable;
        }
    }
}
