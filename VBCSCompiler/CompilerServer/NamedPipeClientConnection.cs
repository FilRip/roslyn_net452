using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class NamedPipeClientConnection : IClientConnection, IDisposable
    {
        private CancellationTokenSource DisconnectCancellationTokenSource { get; } = new CancellationTokenSource();

        private TaskCompletionSource<object> DisconnectTaskCompletionSource { get; } = new TaskCompletionSource<object>();

        public NamedPipeServerStream Stream { get; }

        public ICompilerServerLogger Logger { get; }

        public bool IsDisposed { get; private set; }

        public Task DisconnectTask => DisconnectTaskCompletionSource.Task;

        internal NamedPipeClientConnection(NamedPipeServerStream stream, ICompilerServerLogger logger)
        {
            this.Stream = stream;
            this.Logger = logger;
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            try
            {
                this.DisconnectTaskCompletionSource.TrySetResult(new object());
                this.DisconnectCancellationTokenSource.Cancel();
                this.Stream.Close();
            }
            catch (Exception ex)
            {
                this.Logger.LogException(ex, "Error closing client connection");
            }
            this.IsDisposed = true;
        }

        public async Task<BuildRequest> ReadBuildRequestAsync(
          CancellationToken cancellationToken)
        {
            BuildRequest request = await BuildRequest.ReadAsync(Stream, cancellationToken).ConfigureAwait(false);
            if (!NamedPipeUtil.CheckClientElevationMatches(this.Stream))
                throw new Exception("Client identity does not match server identity.");
            await MonitorDisconnect();
            return request;

            async Task MonitorDisconnect()
            {
                try
                {
                    await BuildServerConnection.MonitorDisconnectAsync(Stream, request.RequestId, this.Logger, this.DisconnectCancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogException(ex, string.Format("Error monitoring disconnect {0}", request.RequestId));
                }
                finally
                {
                    this.DisconnectTaskCompletionSource.TrySetResult(this);
                }
            }
        }

        public Task WriteBuildResponseAsync(
          BuildResponse response,
          CancellationToken cancellationToken)
        {
            return response.WriteAsync(Stream, cancellationToken);
        }
    }
}
