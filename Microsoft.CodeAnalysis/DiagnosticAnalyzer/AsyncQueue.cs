// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    /// <summary>
    /// A queue whose enqueue and dequeue operations can be performed in parallel.
    /// </summary>
    /// <typeparam name="TElement">The type of values kept by the queue.</typeparam>
    public sealed class AsyncQueue<TElement>
    {
        // Continuations run asynchronously to ensure user code does not execute within protected regions and lead to
        // delays, deadlocks, and/or state corruption.
        private readonly TaskCompletionSource<bool> _whenCompleted = new(); // FilRip Remove parameter TaskCreationOptions.RunContinuationsAsynchronously

        // Note: All of the below fields are accessed in parallel and may only be accessed
        // when protected by lock (SyncObject)
        private readonly Queue<TElement> _data = new();
        private Queue<TaskCompletionSource<Optional<TElement>>> _waiters;
        private bool _completed;
        private bool _disallowEnqueue;

        private object SyncObject
        {
            get { return _data; }
        }

        /// <summary>
        /// The number of unconsumed elements in the queue.
        /// </summary>
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

        /// <summary>
        /// Adds an element to the tail of the queue.  This method will throw if the queue 
        /// is completed.
        /// </summary>
        /// <exception cref="InvalidOperationException">The queue is already completed.</exception>
        /// <param name="value">The value to add.</param>
        public void Enqueue(TElement value)
        {
            if (!EnqueueCore(value))
            {
                throw new InvalidOperationException($"Cannot call {nameof(Enqueue)} when the queue is already completed.");
            }
        }

        /// <summary>
        /// Tries to add an element to the tail of the queue.  This method will return false if the queue
        /// is completed.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public bool TryEnqueue(TElement value)
        {
            return EnqueueCore(value);
        }

        private bool EnqueueCore(TElement value)
        {
        retry:
            if (_disallowEnqueue)
            {
                throw new InvalidOperationException($"Cannot enqueue data after PromiseNotToEnqueue.");
            }

            TaskCompletionSource<Optional<TElement>> waiter;
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

                Debug.Assert(_data.Count == 0);
                waiter = _waiters.Dequeue();
            }

            if (!waiter.TrySetResult(value))
            {
                // A waiter was available in the queue, but was cancelled before we were able to assign this value
                goto retry;
            }

            return true;
        }

        /// <summary>
        /// Attempts to dequeue an existing item and return whether or not it was available.
        /// </summary>
        public bool TryDequeue(out TElement d)
        {
            lock (SyncObject)
            {
                if (_data.Count == 0)
                {
                    d = default;
                    return false;
                }

                d = _data.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the queue has completed.
        /// </summary>
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

        /// <summary>
        /// Signals that no further elements will be enqueued.  All outstanding and future
        /// Dequeue Task will be cancelled.
        /// </summary>
        /// <exception cref="InvalidOperationException">The queue is already completed.</exception>
        public void Complete()
        {
            if (!CompleteCore())
            {
                throw new InvalidOperationException($"Cannot call {nameof(Complete)} when the queue is already completed.");
            }
        }

        public void PromiseNotToEnqueue()
        {
            _disallowEnqueue = true;
        }

        /// <summary>
        /// Same operation as <see cref="AsyncQueue{TElement}.Complete"/> except it will not
        /// throw if the queue is already completed.
        /// </summary>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool TryComplete()
        {
            return CompleteCore();
        }

        private bool CompleteCore()
        {
            Queue<TaskCompletionSource<Optional<TElement>>> existingWaiters;
            lock (SyncObject)
            {
                if (_completed)
                {
                    return false;
                }

                _completed = true;

                existingWaiters = _waiters;
                _waiters = null;
            }

            if (existingWaiters?.Count > 0)
            {
                // cancel waiters.
                // NOTE: AsyncQueue has an invariant that 
                //       the queue can either have waiters or items, not both
                //       adding an item would "unwait" the waiters
                //       the fact that we _had_ waiters at the time we completed the queue
                //       guarantees that there is no items in the queue now or in the future, 
                //       so it is safe to cancel waiters with no loss of diagnostics
                foreach (var tcs in existingWaiters)
                {
                    tcs.TrySetResult(default);
                }
            }

            _whenCompleted.SetResult(true);

            return true;
        }

        /// <summary>
        /// Gets a task that transitions to a completed state when <see cref="Complete"/> or
        /// <see cref="TryComplete"/> is called.  This transition will not happen synchronously.
        /// 
        /// This Task will not complete until it has completed all existing values returned
        /// from <see cref="DequeueAsync"/>.
        /// </summary>
        public Task WhenCompletedTask
        {
            get
            {
                return _whenCompleted.Task;
            }
        }

        /// <summary>
        /// Gets a task whose result is the element at the head of the queue. If the queue
        /// is empty, the returned task waits for an element to be enqueued. If <see cref="Complete"/> 
        /// is called before an element becomes available, the returned task is cancelled.
        /// </summary>
        [PerformanceSensitive("https://github.com/dotnet/roslyn/issues/23582", OftenCompletesSynchronously = true)]
        public Task<TElement> DequeueAsync(CancellationToken cancellationToken = default)
        {
            var optionalResult = TryDequeueAsync(cancellationToken);
            if (optionalResult.IsCompletedSuccessfully)
            {
                var result = optionalResult.Result;
                return result.HasValue
                    ? Task.FromResult(result.Value)
                    : //Task.FromCanceled<TElement>(new CancellationToken(canceled: true));
                      // FilRip create canceled task
                    new Task<TElement>(new Func<TElement>(() => { return default; }), new CancellationToken(true));
            }

            return dequeueSlowAsync(optionalResult);

            static async Task<TElement> dequeueSlowAsync(ValueTask<Optional<TElement>> optionalResult)
            {
                var result = await optionalResult.ConfigureAwait(false);
                if (!result.HasValue)
                    new CancellationToken(canceled: true).ThrowIfCancellationRequested();

                return result.Value;
            }
        }

        /// <summary>
        /// Gets a task whose result is the element at the head of the queue. If the queue
        /// is empty, the returned task waits for an element to be enqueued. If <see cref="Complete"/> 
        /// is called before an element becomes available, the returned task is completed and
        /// <see cref="Optional{T}.HasValue"/> will be <see langword="false"/>.
        /// </summary>
        [PerformanceSensitive("https://github.com/dotnet/roslyn/issues/23582", OftenCompletesSynchronously = true)]
        public ValueTask<Optional<TElement>> TryDequeueAsync(CancellationToken cancellationToken)
        {
            lock (SyncObject)
            {
                // No matter what the state we allow DequeueAsync to drain the existing items 
                // in the queue.  This keeps the behavior in line with TryDequeue
                if (_data.Count > 0)
                {
                    return ValueTaskFactory.FromResult<Optional<TElement>>(_data.Dequeue());
                }

                if (_completed)
                {
                    return ValueTaskFactory.FromResult(default(Optional<TElement>));
                }

                _waiters ??= new Queue<TaskCompletionSource<Optional<TElement>>>();

                // Continuations run asynchronously to ensure user code does not execute within protected regions.
                var waiter = new TaskCompletionSource<Optional<TElement>>(); // FilRip Remove parameter TaskCreationOptions.RunContinuationsAsynchronously
                AttachCancellation(waiter, cancellationToken);
                _waiters.Enqueue(waiter);
                return new ValueTask<Optional<TElement>>(waiter.Task);
            }
        }

        /// <summary>
        /// Cancels a <see cref="TaskCompletionSource{TResult}.Task"/> if a given <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <typeparam name="T">The type of value returned by a successfully completed <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="taskCompletionSource">The <see cref="TaskCompletionSource{TResult}"/> to cancel.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <seealso href="https://github.com/microsoft/vs-threading/blob/558f24c576cc620a00b20ed1fa90a5e2d13b0440/src/Microsoft.VisualStudio.Threading/ThreadingTools.cs#L181-L255"/>
        private static void AttachCancellation<T>(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled || taskCompletionSource.Task.IsCompleted)
                return;

            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.TrySetCanceled();
                return;
            }

            var cancelableTaskCompletionSource = new CancelableTaskCompletionSource<T>(taskCompletionSource, cancellationToken);
            cancelableTaskCompletionSource.CancellationTokenRegistration = cancellationToken.Register(
                static s =>
                {
                    var t = (CancelableTaskCompletionSource<T>)s!;
                    t.TaskCompletionSource.TrySetCanceled();
                },
                cancelableTaskCompletionSource,
                useSynchronizationContext: false);

            taskCompletionSource.Task.ContinueWith(
                static (_, s) =>
                {
                    var t = (CancelableTaskCompletionSource<T>)s!;
                    t.CancellationTokenRegistration.Dispose();
                },
                cancelableTaskCompletionSource,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// A state object for tracking cancellation and a TaskCompletionSource.
        /// </summary>
        /// <typeparam name="T">The type of value returned from a task.</typeparam>
        /// <remarks>
        /// We use this class so that we only allocate one object to support all continuations
        /// required for cancellation handling, rather than a special closure and delegate for each one.
        /// </remarks>
        /// <seealso href="https://github.com/microsoft/vs-threading/blob/558f24c576cc620a00b20ed1fa90a5e2d13b0440/src/Microsoft.VisualStudio.Threading/ThreadingTools.cs#L318-L372"/>
        private sealed class CancelableTaskCompletionSource<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CancelableTaskCompletionSource{T}"/> class.
            /// </summary>
            /// <param name="taskCompletionSource">The task completion source.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            internal CancelableTaskCompletionSource(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
            {
                TaskCompletionSource = taskCompletionSource;
                CancellationToken = cancellationToken;
            }

            /// <summary>
            /// Gets the cancellation token.
            /// </summary>
            internal CancellationToken CancellationToken { get; }

            /// <summary>
            /// Gets the Task completion source.
            /// </summary>
            internal TaskCompletionSource<T> TaskCompletionSource { get; }

            /// <summary>
            /// Gets or sets the cancellation token registration.
            /// </summary>
            internal CancellationTokenRegistration CancellationTokenRegistration { get; set; }
        }
    }
}
