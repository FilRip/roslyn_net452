using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;
using Microsoft.CodeAnalysis.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class NamedPipeClientConnectionHost : IClientConnectionHost
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Task[]? _listenTasks;
        private AsyncQueue<NamedPipeClientConnectionHost.ListenResult>? _queue;

        public string PipeName { get; }

        public ICompilerServerLogger Logger { get; }

        public bool IsListening { get; private set; }

        internal NamedPipeClientConnectionHost(string pipeName, ICompilerServerLogger logger)
        {
            this.PipeName = pipeName;
            this.Logger = logger;
        }

        public void BeginListening()
        {
            this.IsListening = !this.IsListening ? true : throw new InvalidOperationException();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._queue = new AsyncQueue<ListenResult>();
            int length = Math.Min(4, Environment.ProcessorCount);
            this._listenTasks = new Task[length];
            for (int index = 0; index < length; ++index)
            {
                Task task = Task.Run(() => ListenCoreAsync(this.PipeName, this.Logger, this._queue, this._cancellationTokenSource.Token));
                this._listenTasks[index] = task;
            }
        }

#nullable restore

        public void EndListening()
        {
            if (!this.IsListening)
                throw new InvalidOperationException();
            try
            {
                this._cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                this.Logger.LogException(ex, "Cancelling server listens threw an exception");
            }
            try
            {
                Task.WaitAll(this._listenTasks);
            }
            catch (Exception ex)
            {
                this.Logger.LogException(ex, "Listen tasks threw exception during EndListening");
            }
            this._queue.Complete();
            this._queue.WhenCompletedTask.Wait();
            while (this._queue.TryDequeue(out ListenResult d))
                d.NamedPipeClientConnection?.Dispose();
            this._queue = null;
            this._cancellationTokenSource.Dispose();
            this._cancellationTokenSource = null;
            this._listenTasks = null;
            this.IsListening = false;
        }

        public async Task<IClientConnection> GetNextClientConnectionAsync()
        {
            if (!this.IsListening)
                throw new InvalidOperationException();
            ListenResult listenResult = await this._queue.DequeueAsync(this._cancellationTokenSource.Token).ConfigureAwait(false);
            if (listenResult.Exception != null)
                throw new VbCsCompilerException("Error occurred listening for connections", listenResult.Exception);
            return listenResult.NamedPipeClientConnection ?? throw new OperationCanceledException();
        }

        private static async Task ListenCoreAsync(
            string pipeName,
            ICompilerServerLogger logger,
            AsyncQueue<ListenResult> queue,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                NamedPipeServerStream pipeStream = null;
                try
                {
                    logger.Log("Constructing pipe and waiting for connections '" + pipeName + "'");
                    pipeStream = NamedPipeUtil.CreateServer(pipeName);
                    pipeStream.WaitForConnection();
                    if (!PlatformInformation.IsWindows)
                    {
                        Task cancelTask = Task.Delay(TimeSpan.FromMilliseconds(-1.0), cancellationToken);
                        cancelTask = await Task.WhenAny(cancelTask).ConfigureAwait(false) != cancelTask ? null : throw new OperationCanceledException();
                    }
                    logger.Log("Pipe connection established.");
                    queue.Enqueue(new ListenResult(new NamedPipeClientConnection(pipeStream, logger)));
                }
                catch (OperationCanceledException)
                {
                    logger.Log("Pipe connection cancelled");
                    pipeStream?.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Pipe connection error");
                    queue.Enqueue(new ListenResult(exception: ex));
                    pipeStream?.Dispose();
                }
            }
        }

#nullable enable

        private readonly struct ListenResult
        {
            internal NamedPipeClientConnection? NamedPipeClientConnection { get; }

            internal Exception? Exception { get; }

            internal ListenResult(NamedPipeClientConnection? connection = null, Exception? exception = null)
            {
                this.NamedPipeClientConnection = connection;
                this.Exception = exception;
            }
        }
    }
}
