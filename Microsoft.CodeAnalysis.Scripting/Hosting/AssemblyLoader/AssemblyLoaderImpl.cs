// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;

namespace Microsoft.CodeAnalysis.Scripting.Hosting
{
    internal abstract class AssemblyLoaderImpl : IDisposable
    {
        internal readonly InteractiveAssemblyLoader Loader;

        protected AssemblyLoaderImpl(InteractiveAssemblyLoader loader)
        {
            Loader = loader;
        }

        public static AssemblyLoaderImpl Create(InteractiveAssemblyLoader loader)
        {
#if NETSTANDARD2_0
            if (CoreClrShim.AssemblyLoadContext.Type != null)
            {
                return CreateCoreImpl(loader);
            }
            else
            {
                return new DesktopAssemblyLoaderImpl(loader);
            }
#else
            return new DesktopAssemblyLoaderImpl(loader);
#endif
        }

#if NETSTANDARD2_0
        // NoInlining to avoid loading AssemblyLoadContext if not available.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static AssemblyLoaderImpl CreateCoreImpl(InteractiveAssemblyLoader loader)
        {
            return new CoreAssemblyLoaderImpl(loader);
        }
#endif

        public abstract Assembly LoadFromStream(Stream peStream, Stream pdbStream);
        public abstract AssemblyAndLocation LoadFromPath(string path);
        protected virtual void Dispose(bool disposing)
        {
            // Nothing to do in this abstract class
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
