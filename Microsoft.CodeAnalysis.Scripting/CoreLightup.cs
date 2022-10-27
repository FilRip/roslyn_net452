// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Reflection;

namespace Roslyn.Utilities
{
    /// <summary>
    /// This type contains the light up scenarios for various platform and runtimes.  Any function
    /// in this type can, and is expected to, fail on various platforms.  These are light up scenarios
    /// only.
    /// </summary>
    internal static class CoreLightup
    {
        internal static class Desktop
        {
            private static class UAssembly
            {
                internal static readonly Type Type = typeof(Assembly);

                internal static readonly Func<Assembly, bool> get_GlobalAssemblyCache = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("get_GlobalAssemblyCache")
                    .CreateDelegate<Func<Assembly, bool>>();
            }

            private static class UResolveEventArgs
            {
                internal static readonly Type Type = ReflectionUtilities.TryGetType("System.ResolveEventArgs");

                internal static readonly MethodInfo get_Name = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("get_Name");

                internal static readonly MethodInfo get_RequestingAssembly = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("get_RequestingAssembly");
            }

            private static class UAppDomain
            {
                internal static readonly Type Type = ReflectionUtilities.TryGetType("System.AppDomain");
                internal static readonly Type ResolveEventHandlerType = ReflectionUtilities.TryGetType("System.ResolveEventHandler");

                internal static readonly MethodInfo get_CurrentDomain = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("get_CurrentDomain");

                internal static readonly MethodInfo add_AssemblyResolve = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("add_AssemblyResolve", ResolveEventHandlerType);

                internal static readonly MethodInfo remove_AssemblyResolve = Type
                    .GetTypeInfo()
                    .GetDeclaredMethod("remove_AssemblyResolve", ResolveEventHandlerType);
            }

            internal static bool IsAssemblyFromGlobalAssemblyCache(Assembly assembly)
            {
                if (UAssembly.get_GlobalAssemblyCache == null)
                {
                    throw new PlatformNotSupportedException();
                }

                return UAssembly.get_GlobalAssemblyCache(assembly);
            }

            private sealed class AssemblyResolveWrapper
            {
                private readonly Func<string, Assembly, Assembly> _handler;
                private static readonly MethodInfo s_stubInfo = typeof(AssemblyResolveWrapper).GetTypeInfo().GetDeclaredMethod(nameof(Stub));

                public AssemblyResolveWrapper(Func<string, Assembly, Assembly> handler)
                {
                    _handler = handler;
                }

                // Necessary parameter to keep compatible with event delegate
#pragma warning disable IDE0060
                private Assembly Stub(object sender, object resolveEventArgs)
#pragma warning restore IDE0060
                {
                    var name = (string)UResolveEventArgs.get_Name.Invoke(resolveEventArgs, new object[0] { });
                    var requestingAssembly = (Assembly)UResolveEventArgs.get_RequestingAssembly.Invoke(resolveEventArgs, new object[0] { });

                    return _handler(name, requestingAssembly);
                }

                public object GetHandler()
                {
                    return s_stubInfo.CreateDelegate(UAppDomain.ResolveEventHandlerType, this);
                }
            }

            internal static void GetOrRemoveAssemblyResolveHandler(Func<string, Assembly, Assembly> handler, MethodInfo handlerOperation)
            {
                if (UAppDomain.add_AssemblyResolve == null)
                {
                    throw new PlatformNotSupportedException();
                }

                var currentAppDomain = AppDomain.CurrentDomain;
                object resolveEventHandler = new AssemblyResolveWrapper(handler).GetHandler();

                handlerOperation.Invoke(currentAppDomain, new[] { resolveEventHandler });
            }

            internal static void AddAssemblyResolveHandler(Func<string, Assembly, Assembly> handler)
            {
                GetOrRemoveAssemblyResolveHandler(handler, UAppDomain.add_AssemblyResolve);
            }

            internal static void RemoveAssemblyResolveHandler(Func<string, Assembly, Assembly> handler)
            {
                GetOrRemoveAssemblyResolveHandler(handler, UAppDomain.remove_AssemblyResolve);
            }
        }
    }
}
