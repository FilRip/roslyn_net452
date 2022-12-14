// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.Cci
{
    public sealed class PooledBlobBuilder : BlobBuilder, IDisposable
    {
        private const int PoolSize = 128;
        private const int ChunkSize = 1024;

        private static readonly ObjectPool<PooledBlobBuilder> s_chunkPool = new(() => new PooledBlobBuilder(ChunkSize), PoolSize);

        private PooledBlobBuilder(int size)
            : base(size)
        {
        }

#pragma warning disable IDE0060
        public static PooledBlobBuilder GetInstance(int size = ChunkSize)
#pragma warning restore IDE0060
        {
            // TODO: use size
            return s_chunkPool.Allocate();
        }

        protected override BlobBuilder AllocateChunk(int minimalSize)
        {
            if (minimalSize <= ChunkSize)
            {
                return s_chunkPool.Allocate();
            }

            return new BlobBuilder(minimalSize);
        }

        protected override void FreeChunk()
        {
            s_chunkPool.Free(this);
        }

        public new void Free()
        {
            base.Free();
        }

        void IDisposable.Dispose()
        {
            Free();
        }
    }
}
