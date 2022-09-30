using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Roslyn.Utilities
{
    internal static class SerializationThreadPool
    {
        private static class ImmediateBackgroundThreadPool
        {
            private static readonly TimeSpan s_idleTimeout = TimeSpan.FromSeconds(1.0);

            private static readonly Queue<(Delegate function, object? state, TaskCompletionSource<object?> tcs)> s_queue = new Queue<(Delegate, object, TaskCompletionSource<object>)>();

            private static int s_availableThreads = 0;

            public static Task<object?> QueueAsync(Func<object?> threadStart)
            {
                return QueueAsync(threadStart, null);
            }

            public static Task<object?> QueueAsync(Func<object?, object?> threadStart, object? state)
            {
                return QueueAsync((Delegate)threadStart, state);
            }

            private static Task<object?> QueueAsync(Delegate threadStart, object? state)
            {
                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(/*TaskCreationOptions.RunContinuationsAsynchronously*/TaskCreationOptions.PreferFairness);
                enqueue((threadStart, state, taskCompletionSource));
                return taskCompletionSource.Task;
                static void createThread()
                {
                    Thread thread = new Thread((ThreadStart)delegate
                    {
                        while (tryDequeue(out (Delegate, object, TaskCompletionSource<object>) item2))
                        {
                            try
                            {
                                if (item2.Item1 is Func<object, object> func)
                                {
                                    item2.Item3.SetResult(func(item2.Item2));
                                }
                                else
                                {
                                    item2.Item3.SetResult(((Func<object>)item2.Item1)());
                                }
                            }
                            catch (OperationCanceledException/* ex*/)
                            {
                                item2.Item3.TrySetCanceled(/*ex.CancellationToken*/);
                            }
                            catch (Exception exception)
                            {
                                item2.Item3.TrySetException(exception);
                            }
                        }
                    });
                    thread.IsBackground = true;
                    thread.Start();
                }
                static void enqueue((Delegate function, object? state, TaskCompletionSource<object?> tcs) item)
                {
                    lock (s_queue)
                    {
                        s_queue.Enqueue(item);
                        if (s_queue.Count <= s_availableThreads)
                        {
                            Monitor.Pulse(s_queue);
                            return;
                        }
                    }
                    createThread();
                }
                static bool tryDequeue(out (Delegate function, object? state, TaskCompletionSource<object?> tcs) item)
                {
                    lock (s_queue)
                    {
                        s_availableThreads++;
                        try
                        {
                            while (s_queue.Count == 0)
                            {
                                if (!Monitor.Wait(s_queue, s_idleTimeout))
                                {
                                    if (s_queue.Count > 0)
                                    {
                                        break;
                                    }
                                    item = default((Delegate, object, TaskCompletionSource<object>));
                                    return false;
                                }
                            }
                        }
                        finally
                        {
                            s_availableThreads--;
                        }
                        item = s_queue.Dequeue();
                        return true;
                    }
                }
            }
        }

        public static Task<object?> RunOnBackgroundThreadAsync(Func<object?> start)
        {
            return ImmediateBackgroundThreadPool.QueueAsync(start);
        }

        public static Task<object?> RunOnBackgroundThreadAsync(Func<object?, object?> start, object? obj)
        {
            return ImmediateBackgroundThreadPool.QueueAsync(start, obj);
        }
    }
}
