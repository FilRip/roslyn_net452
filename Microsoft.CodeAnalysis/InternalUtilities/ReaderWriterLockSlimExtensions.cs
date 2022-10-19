// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace Roslyn.Utilities
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static ReadLockExiter DisposableRead(this ReaderWriterLockSlim @lock)
        {
            return new ReadLockExiter(@lock);
        }

        [NonCopyable]
        public readonly struct ReadLockExiter : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            internal ReadLockExiter(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                @lock.EnterReadLock();
            }

            public void Dispose()
            {
                _lock.ExitReadLock();
            }
        }

        public static UpgradeableReadLockExiter DisposableUpgradeableRead(this ReaderWriterLockSlim @lock)
        {
            return new UpgradeableReadLockExiter(@lock);
        }

        [NonCopyable]
        public readonly struct UpgradeableReadLockExiter : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            internal UpgradeableReadLockExiter(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                @lock.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                if (_lock.IsWriteLockHeld)
                {
                    _lock.ExitWriteLock();
                }

                _lock.ExitUpgradeableReadLock();
            }

            public void EnterWrite()
            {
                _lock.EnterWriteLock();
            }
        }

        public static WriteLockExiter DisposableWrite(this ReaderWriterLockSlim @lock)
        {
            return new WriteLockExiter(@lock);
        }

        [NonCopyable]
        public readonly struct WriteLockExiter : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            internal WriteLockExiter(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                @lock.EnterWriteLock();
            }

            public void Dispose()
            {
                _lock.ExitWriteLock();
            }
        }

        public static void AssertCanRead(this ReaderWriterLockSlim @lock)
        {
            if (!@lock.IsReadLockHeld && !@lock.IsUpgradeableReadLockHeld && !@lock.IsWriteLockHeld)
            {
                throw new InvalidOperationException();
            }
        }

        internal static void AssertCanWrite(this ReaderWriterLockSlim @lock)
        {
            if (!@lock.IsWriteLockHeld)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
