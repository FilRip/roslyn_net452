using System;

namespace Microsoft.CodeAnalysis.PooledObjects
{
    internal static class PooledDelegates
    {
        private static class DefaultDelegatePool<T> where T : class, new()
        {
            public static readonly ObjectPool<T> Instance = new ObjectPool<T>(() => new T(), 20);
        }

        [NonCopyable]
        public struct Releaser : IDisposable
        {
            private readonly Poolable _pooledObject;

            internal Releaser(Poolable pooledObject)
            {
                _pooledObject = pooledObject;
            }

            public void Dispose()
            {
                _pooledObject.ClearAndFree();
            }
        }

        internal abstract class Poolable
        {
            public abstract void ClearAndFree();
        }

        private abstract class AbstractDelegateWithBoundArgument<TSelf, TArg, TUnboundDelegate, TBoundDelegate> : Poolable where TSelf : AbstractDelegateWithBoundArgument<TSelf, TArg, TUnboundDelegate, TBoundDelegate>, new() where TUnboundDelegate : Delegate where TBoundDelegate : Delegate
        {
            public TBoundDelegate BoundDelegate { get; }

            public TUnboundDelegate UnboundDelegate { get; private set; }

            public TArg Argument { get; private set; }

            protected AbstractDelegateWithBoundArgument()
            {
                BoundDelegate = Bind();
                UnboundDelegate = null;
                Argument = default(TArg);
            }

            public void Initialize(TUnboundDelegate unboundDelegate, TArg argument)
            {
                UnboundDelegate = unboundDelegate;
                Argument = argument;
            }

            public sealed override void ClearAndFree()
            {
                Argument = default(TArg);
                UnboundDelegate = null;
                DefaultDelegatePool<TSelf>.Instance.Free((TSelf)this);
            }

            protected abstract TBoundDelegate Bind();
        }

        private sealed class ActionWithBoundArgument<TArg> : AbstractDelegateWithBoundArgument<ActionWithBoundArgument<TArg>, TArg, Action<TArg>, Action>
        {
            protected override Action Bind()
            {
                return delegate
                {
                    base.UnboundDelegate(base.Argument);
                };
            }
        }

        private sealed class ActionWithBoundArgument<T1, TArg> : AbstractDelegateWithBoundArgument<ActionWithBoundArgument<T1, TArg>, TArg, Action<T1, TArg>, Action<T1>>
        {
            protected override Action<T1> Bind()
            {
                return delegate (T1 arg1)
                {
                    base.UnboundDelegate(arg1, base.Argument);
                };
            }
        }

        private sealed class ActionWithBoundArgument<T1, T2, TArg> : AbstractDelegateWithBoundArgument<ActionWithBoundArgument<T1, T2, TArg>, TArg, Action<T1, T2, TArg>, Action<T1, T2>>
        {
            protected override Action<T1, T2> Bind()
            {
                return delegate (T1 arg1, T2 arg2)
                {
                    base.UnboundDelegate(arg1, arg2, base.Argument);
                };
            }
        }

        private sealed class ActionWithBoundArgument<T1, T2, T3, TArg> : AbstractDelegateWithBoundArgument<ActionWithBoundArgument<T1, T2, T3, TArg>, TArg, Action<T1, T2, T3, TArg>, Action<T1, T2, T3>>
        {
            protected override Action<T1, T2, T3> Bind()
            {
                return delegate (T1 arg1, T2 arg2, T3 arg3)
                {
                    base.UnboundDelegate(arg1, arg2, arg3, base.Argument);
                };
            }
        }

        private sealed class FuncWithBoundArgument<TArg, TResult> : AbstractDelegateWithBoundArgument<FuncWithBoundArgument<TArg, TResult>, TArg, Func<TArg, TResult>, Func<TResult>>
        {
            protected override Func<TResult> Bind()
            {
                return () => base.UnboundDelegate(base.Argument);
            }
        }

        private sealed class FuncWithBoundArgument<T1, TArg, TResult> : AbstractDelegateWithBoundArgument<FuncWithBoundArgument<T1, TArg, TResult>, TArg, Func<T1, TArg, TResult>, Func<T1, TResult>>
        {
            protected override Func<T1, TResult> Bind()
            {
                return (T1 arg1) => base.UnboundDelegate(arg1, base.Argument);
            }
        }

        private sealed class FuncWithBoundArgument<T1, T2, TArg, TResult> : AbstractDelegateWithBoundArgument<FuncWithBoundArgument<T1, T2, TArg, TResult>, TArg, Func<T1, T2, TArg, TResult>, Func<T1, T2, TResult>>
        {
            protected override Func<T1, T2, TResult> Bind()
            {
                return (T1 arg1, T2 arg2) => base.UnboundDelegate(arg1, arg2, base.Argument);
            }
        }

        private sealed class FuncWithBoundArgument<T1, T2, T3, TArg, TResult> : AbstractDelegateWithBoundArgument<FuncWithBoundArgument<T1, T2, T3, TArg, TResult>, TArg, Func<T1, T2, T3, TArg, TResult>, Func<T1, T2, T3, TResult>>
        {
            protected override Func<T1, T2, T3, TResult> Bind()
            {
                return (T1 arg1, T2 arg2, T3 arg3) => base.UnboundDelegate(arg1, arg2, arg3, base.Argument);
            }
        }

        [AttributeUsage(AttributeTargets.Struct)]
        private sealed class NonCopyableAttribute : Attribute
        {
        }

        private static Releaser GetPooledDelegate<TPooled, TArg, TUnboundDelegate, TBoundDelegate>(TUnboundDelegate unboundDelegate, TArg argument, out TBoundDelegate boundDelegate) where TPooled : AbstractDelegateWithBoundArgument<TPooled, TArg, TUnboundDelegate, TBoundDelegate>, new() where TUnboundDelegate : Delegate where TBoundDelegate : Delegate
        {
            TPooled val = DefaultDelegatePool<TPooled>.Instance.Allocate();
            val.Initialize(unboundDelegate, argument);
            boundDelegate = val.BoundDelegate;
            return new Releaser(val);
        }

        public static Releaser GetPooledAction<TArg>(Action<TArg> unboundAction, TArg argument, out Action boundAction)
        {
            return GetPooledDelegate<ActionWithBoundArgument<TArg>, TArg, Action<TArg>, Action>(unboundAction, argument, out boundAction);
        }

        public static Releaser GetPooledAction<T1, TArg>(Action<T1, TArg> unboundAction, TArg argument, out Action<T1> boundAction)
        {
            return GetPooledDelegate<ActionWithBoundArgument<T1, TArg>, TArg, Action<T1, TArg>, Action<T1>>(unboundAction, argument, out boundAction);
        }

        public static Releaser GetPooledAction<T1, T2, TArg>(Action<T1, T2, TArg> unboundAction, TArg argument, out Action<T1, T2> boundAction)
        {
            return GetPooledDelegate<ActionWithBoundArgument<T1, T2, TArg>, TArg, Action<T1, T2, TArg>, Action<T1, T2>>(unboundAction, argument, out boundAction);
        }

        public static Releaser GetPooledAction<T1, T2, T3, TArg>(Action<T1, T2, T3, TArg> unboundAction, TArg argument, out Action<T1, T2, T3> boundAction)
        {
            return GetPooledDelegate<ActionWithBoundArgument<T1, T2, T3, TArg>, TArg, Action<T1, T2, T3, TArg>, Action<T1, T2, T3>>(unboundAction, argument, out boundAction);
        }

        public static Releaser GetPooledFunction<TArg, TResult>(Func<TArg, TResult> unboundFunction, TArg argument, out Func<TResult> boundFunction)
        {
            return GetPooledDelegate<FuncWithBoundArgument<TArg, TResult>, TArg, Func<TArg, TResult>, Func<TResult>>(unboundFunction, argument, out boundFunction);
        }

        public static Releaser GetPooledFunction<T1, TArg, TResult>(Func<T1, TArg, TResult> unboundFunction, TArg argument, out Func<T1, TResult> boundFunction)
        {
            return GetPooledDelegate<FuncWithBoundArgument<T1, TArg, TResult>, TArg, Func<T1, TArg, TResult>, Func<T1, TResult>>(unboundFunction, argument, out boundFunction);
        }

        public static Releaser GetPooledFunction<T1, T2, TArg, TResult>(Func<T1, T2, TArg, TResult> unboundFunction, TArg argument, out Func<T1, T2, TResult> boundFunction)
        {
            return GetPooledDelegate<FuncWithBoundArgument<T1, T2, TArg, TResult>, TArg, Func<T1, T2, TArg, TResult>, Func<T1, T2, TResult>>(unboundFunction, argument, out boundFunction);
        }

        public static Releaser GetPooledFunction<T1, T2, T3, TArg, TResult>(Func<T1, T2, T3, TArg, TResult> unboundFunction, TArg argument, out Func<T1, T2, T3, TResult> boundFunction)
        {
            return GetPooledDelegate<FuncWithBoundArgument<T1, T2, T3, TArg, TResult>, TArg, Func<T1, T2, T3, TArg, TResult>, Func<T1, T2, T3, TResult>>(unboundFunction, argument, out boundFunction);
        }
    }
}
