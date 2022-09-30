using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class AsyncQueue<TElement>
    {
        private sealed class CancelableTaskCompletionSource<T>
        {
            internal CancellationToken CancellationToken { get; }

            internal TaskCompletionSource<T> TaskCompletionSource { get; }

            internal CancellationTokenRegistration CancellationTokenRegistration { get; set; }

            internal CancelableTaskCompletionSource(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
            {
                TaskCompletionSource = taskCompletionSource;
                CancellationToken = cancellationToken;
            }
        }

        private readonly TaskCompletionSource<bool> _whenCompleted = new TaskCompletionSource<bool>(); // FilRip Remove parameter TaskCreationOptions.RunContinuationsAsynchronously

        private readonly Queue<TElement> _data = new Queue<TElement>();

        private Queue<TaskCompletionSource<Optional<TElement>>> _waiters;

        private bool _completed;

        private bool _disallowEnqueue;

        private object SyncObject => _data;

        public int Count
        {
            get
            {
                lock (SyncObject)
                {
                    return _data.Count;
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                lock (SyncObject)
                {
                    return _completed;
                }
            }
        }

        public Task WhenCompletedTask => _whenCompleted.Task;

        public void Enqueue(TElement value)
        {
            if (!EnqueueCore(value))
            {
                throw new InvalidOperationException("Cannot call Enqueue when the queue is already completed.");
            }
        }

        public bool TryEnqueue(TElement value)
        {
            return EnqueueCore(value);
        }

        private bool EnqueueCore(TElement value)
        {
            TaskCompletionSource<Optional<TElement>> taskCompletionSource;
            do
            {
                if (_disallowEnqueue)
                {
                    throw new InvalidOperationException("Cannot enqueue data after PromiseNotToEnqueue.");
                }
                lock (SyncObject)
                {
                    if (_completed)
                    {
                        return false;
                    }
                    if (_waiters == null || _waiters.Count == 0)
                    {
                        _data.Enqueue(value);
                        return true;
                    }
                    taskCompletionSource = _waiters.Dequeue();
                }
            }
            while (!taskCompletionSource.TrySetResult(value));
            return true;
        }

        public bool TryDequeue(out TElement d)
        {
            lock (SyncObject)
            {
                if (_data.Count == 0)
                {
                    d = default(TElement);
                    return false;
                }
                d = _data.Dequeue();
                return true;
            }
        }

        public void Complete()
        {
            if (!CompleteCore())
            {
                throw new InvalidOperationException("Cannot call Complete when the queue is already completed.");
            }
        }

        public void PromiseNotToEnqueue()
        {
            _disallowEnqueue = true;
        }

        public bool TryComplete()
        {
            return CompleteCore();
        }

        private bool CompleteCore()
        {
            Queue<TaskCompletionSource<Optional<TElement>>> waiters;
            lock (SyncObject)
            {
                if (_completed)
                {
                    return false;
                }
                _completed = true;
                waiters = _waiters;
                _waiters = null;
            }
            if (waiters != null && waiters.Count > 0)
            {
                foreach (TaskCompletionSource<Optional<TElement>> item in waiters)
                {
                    item.TrySetResult(default(Optional<TElement>));
                }
            }
            _whenCompleted.SetResult(result: true);
            return true;
        }

        public Task<TElement> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ValueTask<Optional<TElement>> optionalResult2 = TryDequeueAsync(cancellationToken);
            if (optionalResult2.IsCompletedSuccessfully)
            {
                Optional<TElement> result = optionalResult2.Result;
                if (!result.HasValue)
                {
                    //return Task.FromCanceled<TElement>(new CancellationToken(canceled: true));
                    // FilRip create canceled task
                    return new Task<TElement>(new Func<TElement>(() => { return default; }), new CancellationToken(true));
                }
                return Task.FromResult(result.Value);
            }
            return dequeueSlowAsync(optionalResult2);
            static async Task<TElement> dequeueSlowAsync(ValueTask<Optional<TElement>> optionalResult)
            {
                Optional<TElement> optional = await optionalResult.ConfigureAwait(continueOnCapturedContext: false);
                if (!optional.HasValue)
                {
                    new CancellationToken(canceled: true).ThrowIfCancellationRequested();
                }
                return optional.Value;
            }
        }

        public ValueTask<Optional<TElement>> TryDequeueAsync(CancellationToken cancellationToken)
        {
            lock (SyncObject)
            {
                if (_data.Count > 0)
                {
                    return ValueTaskFactory.FromResult((Optional<TElement>)_data.Dequeue());
                }
                if (_completed)
                {
                    return ValueTaskFactory.FromResult(default(Optional<TElement>));
                }
                if (_waiters == null)
                {
                    _waiters = new Queue<TaskCompletionSource<Optional<TElement>>>();
                }
                TaskCompletionSource<Optional<TElement>> taskCompletionSource = new TaskCompletionSource<Optional<TElement>>(); // FilRip Remove parameter TaskCreationOptions.RunContinuationsAsynchronously
                AttachCancellation(taskCompletionSource, cancellationToken);
                _waiters.Enqueue(taskCompletionSource);
                return new ValueTask<Optional<TElement>>(taskCompletionSource.Task);
            }
        }

        private static void AttachCancellation<T>(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled || taskCompletionSource.Task.IsCompleted)
            {
                return;
            }
            if (cancellationToken.IsCancellationRequested)
            {
                //taskCompletionSource.TrySetCanceled(cancellationToken);
                // FilRip : TrySetCanceled without parameters
                taskCompletionSource.TrySetCanceled();
                return;
            }
            CancelableTaskCompletionSource<T> cancelableTaskCompletionSource = new CancelableTaskCompletionSource<T>(taskCompletionSource, cancellationToken);
            cancelableTaskCompletionSource.CancellationTokenRegistration = cancellationToken.Register(delegate (object s)
            {
                CancelableTaskCompletionSource<T> cancelableTaskCompletionSource2 = (CancelableTaskCompletionSource<T>)s;
                //cancelableTaskCompletionSource2.TaskCompletionSource.TrySetCanceled(cancelableTaskCompletionSource2.CancellationToken);
                // FilRip : TrySetCanceled without parameters
                cancelableTaskCompletionSource2.TaskCompletionSource.TrySetCanceled();
            }, cancelableTaskCompletionSource, useSynchronizationContext: false);
            taskCompletionSource.Task.ContinueWith(delegate (Task<T> _, object s)
            {
                ((CancelableTaskCompletionSource<T>)s).CancellationTokenRegistration.Dispose();
            }, cancelableTaskCompletionSource, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }
}
