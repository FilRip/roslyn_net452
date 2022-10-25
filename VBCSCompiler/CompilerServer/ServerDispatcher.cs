using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class ServerDispatcher
    {
        internal static readonly TimeSpan DefaultServerKeepAlive = TimeSpan.FromMinutes(10.0);
        internal static readonly TimeSpan GCTimeout = TimeSpan.FromSeconds(30.0);
        private readonly ICompilerServerHost _compilerServerHost;
        private readonly ICompilerServerLogger _logger;
        private readonly IClientConnectionHost _clientConnectionHost;
        private readonly IDiagnosticListener _diagnosticListener;
        private ServerDispatcher.State _state;
        private Task? _timeoutTask;
        private Task? _gcTask;
        private Task<IClientConnection>? _listenTask;
        private readonly List<Task<CompletionData>> _connectionList = new();
        private TimeSpan? _keepAlive;
        private bool _keepAliveIsDefault;

        internal ServerDispatcher(
          ICompilerServerHost compilerServerHost,
          IClientConnectionHost clientConnectionHost,
          IDiagnosticListener? diagnosticListener = null)
        {
            this._compilerServerHost = compilerServerHost;
            this._logger = compilerServerHost.Logger;
            this._clientConnectionHost = clientConnectionHost;
            this._diagnosticListener = diagnosticListener ?? new EmptyDiagnosticListener();
        }

        public void ListenAndDispatchConnections(
          TimeSpan? keepAlive,
          CancellationToken cancellationToken = default)
        {
            this._state = State.Running;
            this._keepAlive = keepAlive;
            this._keepAliveIsDefault = true;
            try
            {
                this._clientConnectionHost.BeginListening();
                this.ListenAndDispatchConnectionsCore(cancellationToken);
            }
            finally
            {
                this._state = ServerDispatcher.State.Completed;
                this._gcTask = null;
                this._timeoutTask = null;
                if (this._clientConnectionHost.IsListening)
                    this._clientConnectionHost.EndListening();
                if (this._listenTask != null)
                {
                    if (!this._listenTask.IsCompleted)
                        this._listenTask.ContinueWith(_ => { }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Wait(CancellationToken.None);
                    if (this._listenTask.Status == TaskStatus.RanToCompletion)
                    {
                        try
                        {
                            this._listenTask.Result.Dispose();
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogException(ex, "Error disposing of _listenTask");
                        }
                    }
                }
            }
            this._logger.Log("End ListenAndDispatchConnections");
        }

        public void ListenAndDispatchConnectionsCore(CancellationToken cancellationToken)
        {
            do
            {
                this.MaybeCreateListenTask();
                this.MaybeCreateTimeoutTask();
                this.MaybeCreateGCTask();
                this.WaitForAnyCompletion(cancellationToken);
                this.CheckCompletedTasks(cancellationToken);
            }
            while (this._connectionList.Count > 0 || this._state == ServerDispatcher.State.Running);
        }

        private void CheckCompletedTasks(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                this.ChangeToShuttingDown("Server cancellation");
#nullable restore
            Task<IClientConnection> listenTask = this._listenTask;
            // ISSUE: explicit non-virtual call
            if ((listenTask != null ? ((listenTask.IsCompleted) ? 1 : 0) : 0) != 0)
            {
                this._diagnosticListener.ConnectionReceived();
                this._connectionList.Add(ServerDispatcher.ProcessClientConnectionAsync(this._compilerServerHost, this._listenTask, this._state == ServerDispatcher.State.Running, cancellationToken));
                this._timeoutTask = null;
                this._gcTask = null;
                this._listenTask = null;
            }
            Task timeoutTask = this._timeoutTask;
            // ISSUE: explicit non-virtual call
            if ((timeoutTask != null ? ((timeoutTask.IsCompleted) ? 1 : 0) : 0) != 0)
            {
                this._diagnosticListener.KeepAliveReached();
                this.ChangeToShuttingDown("Keep alive hit");
            }
            Task gcTask = this._gcTask;
            // ISSUE: explicit non-virtual call
            if ((gcTask != null ? ((gcTask.IsCompleted) ? 1 : 0) : 0) != 0)
                this.RunGC();
            this.HandleCompletedConnections();
        }

        private void WaitForAnyCompletion(CancellationToken cancellationToken)
        {
            List<Task> all = new();
            all.AddRange(_connectionList);
            AddNonNull(this._timeoutTask);
            AddNonNull(_listenTask);
            AddNonNull(this._gcTask);
            try
            {
                Task.WaitAny(all.ToArray(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }

#nullable enable

            void AddNonNull(Task? task)
            {
                if (task == null)
                    return;
                all.Add(task);
            }
        }

        private void ChangeToShuttingDown(string reason)
        {
            if (this._state == ServerDispatcher.State.ShuttingDown)
                return;
            this._logger.Log("Shutting down server: " + reason);
            this._state = ServerDispatcher.State.ShuttingDown;
            this._timeoutTask = null;
            this._gcTask = null;
        }

        private void RunGC()
        {
            this._gcTask = null;
            for (int index = 0; index < 10; ++index)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }

        private void MaybeCreateListenTask()
        {
            if (this._listenTask != null)
                return;
            this._listenTask = this._clientConnectionHost.GetNextClientConnectionAsync();
        }

        private void MaybeCreateTimeoutTask()
        {
            if (this._state != ServerDispatcher.State.Running || this._connectionList.Count != 0 || this._timeoutTask != null || !this._keepAlive.HasValue)
                return;
            this._timeoutTask = Task.Delay(this._keepAlive.Value);
        }

        private void MaybeCreateGCTask()
        {
            if (this._state != ServerDispatcher.State.Running || this._connectionList.Count != 0 || this._gcTask != null)
                return;
            this._gcTask = Task.Delay(ServerDispatcher.GCTimeout);
        }

        private void HandleCompletedConnections()
        {
            bool flag = false;
            int index = 0;
            while (index < this._connectionList.Count)
            {
                Task<CompletionData> connection = this._connectionList[index];
                if (!connection.IsCompleted)
                {
                    ++index;
                }
                else
                {
                    this._connectionList.RemoveAt(index);
                    CompletionData result = connection.Result;
                    switch (result.Reason)
                    {
                        case CompletionReason.RequestCompleted:
                            this._logger.Log("Client request completed");
                            TimeSpan? newKeepAlive = result.NewKeepAlive;
                            if (newKeepAlive.HasValue)
                            {
                                TimeSpan valueOrDefault = newKeepAlive.GetValueOrDefault();
                                this._logger.Log(string.Format("Client changed keep alive to {0}", valueOrDefault));
                                this.ChangeKeepAlive(valueOrDefault);
                            }
                            if (result.ShutdownRequest)
                            {
                                this._logger.Log("Client requested shutdown");
                                flag = true;
                                break;
                            }
                            break;
                        case CompletionReason.RequestError:
                            this._logger.LogError("Client request failed");
                            flag = true;
                            break;
                        default:
                            this._logger.LogError("Unexpected enum value");
                            flag = true;
                            break;
                    }
                    this._diagnosticListener.ConnectionCompleted(result);
                }
            }
            if (!flag)
                return;
            this.ChangeToShuttingDown("Error handling client connection");
        }

        private void ChangeKeepAlive(TimeSpan keepAlive)
        {
            if (!this._keepAliveIsDefault && this._keepAlive.HasValue && !(keepAlive > this._keepAlive.Value))
                return;
            this._keepAlive = new TimeSpan?(keepAlive);
            this._keepAliveIsDefault = false;
            this._diagnosticListener.UpdateKeepAlive(keepAlive);
        }

        internal static async Task<CompletionData> ProcessClientConnectionAsync(
          ICompilerServerHost compilerServerHost,
          Task<IClientConnection> clientStreamTask,
          bool allowCompilationRequests,
          CancellationToken cancellationToken)
        {
            return await new ClientConnectionHandler(compilerServerHost).ProcessAsync(clientStreamTask, allowCompilationRequests, cancellationToken).ConfigureAwait(false);
        }

        private enum State
        {
            Running,
            ShuttingDown,
            Completed,
        }
    }
}
