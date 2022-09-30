using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Roslyn.Utilities
{
    internal readonly struct ConfiguredYieldAwaitable
    {
        public readonly struct ConfiguredYieldAwaiter : INotifyCompletion, ICriticalNotifyCompletion
        {
            private static readonly WaitCallback s_runContinuation = (continuation) =>
            {
                ((Action)continuation)();
            };

            private readonly YieldAwaitable.YieldAwaiter _awaiter;

            private readonly bool _continueOnCapturedContext;

            public bool IsCompleted
            {
                get
                {
                    YieldAwaitable.YieldAwaiter awaiter = _awaiter;
                    return awaiter.IsCompleted;
                }
            }

            public ConfiguredYieldAwaiter(YieldAwaitable.YieldAwaiter awaiter, bool continueOnCapturedContext)
            {
                _awaiter = awaiter;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            public void GetResult()
            {
                YieldAwaitable.YieldAwaiter awaiter = _awaiter;
                awaiter.GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                if (_continueOnCapturedContext)
                {
                    YieldAwaitable.YieldAwaiter awaiter = _awaiter;
                    awaiter.OnCompleted(continuation);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(s_runContinuation, continuation);
                }
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (_continueOnCapturedContext)
                {
                    YieldAwaitable.YieldAwaiter awaiter = _awaiter;
                    awaiter.UnsafeOnCompleted(continuation);
                }
                else
                {
                    ThreadPool.UnsafeQueueUserWorkItem(s_runContinuation, continuation);
                }
            }
        }

        private readonly YieldAwaitable _awaitable;

        private readonly bool _continueOnCapturedContext;

        public ConfiguredYieldAwaitable(YieldAwaitable awaitable, bool continueOnCapturedContext)
        {
            _awaitable = awaitable;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public ConfiguredYieldAwaiter GetAwaiter()
        {
            YieldAwaitable awaitable = _awaitable;
            return new ConfiguredYieldAwaiter(awaitable.GetAwaiter(), _continueOnCapturedContext);
        }
    }
}
