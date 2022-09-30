using System;
using System.Reflection;
using System.Threading;

namespace Microsoft.CodeAnalysis
{
    public class DefaultAnalyzerAssemblyLoader : AnalyzerAssemblyLoader
    {
        private int _hookedAssemblyResolve;

        protected override Assembly LoadFromPathImpl(string fullPath)
        {
            if (Interlocked.CompareExchange(ref _hookedAssemblyResolve, 0, 1) == 0)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
            return LoadImpl(fullPath);
        }

        protected virtual Assembly LoadImpl(string fullPath)
        {
            return Assembly.LoadFrom(fullPath);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return Load(AppDomain.CurrentDomain.ApplyPolicy(args.Name));
            }
            catch
            {
                return null;
            }
        }
    }
}
