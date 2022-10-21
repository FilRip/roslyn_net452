using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal static class NamedPipeUtil
    {
        //private const int PipeBufferSize = 65536;

        //private const int s_currentUserOnlyValue = 536870912;

        private static readonly PipeOptions CurrentUserOption = (PlatformInformation.IsRunningOnMono ? ((PipeOptions)536870912) : PipeOptions.None);

        private static string GetPipeNameOrPath(string pipeName)
        {
            if (PlatformInformation.IsUnix)
            {
                return Path.Combine("/tmp", pipeName);
            }
            return pipeName;
        }

        internal static NamedPipeClientStream CreateClient(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
        {
            return new NamedPipeClientStream(serverName, GetPipeNameOrPath(pipeName), direction, options | CurrentUserOption);
        }

        internal static bool CheckClientElevationMatches(NamedPipeServerStream pipeStream)
        {
            if (PlatformInformation.IsWindows)
            {
                (string, bool) tuple = getIdentity(impersonating: false);
                (string name, bool admin) clientIdentity = default((string, bool));
                pipeStream.RunAsClient(delegate
                {
                    clientIdentity = getIdentity(impersonating: true);
                });
                if (StringComparer.OrdinalIgnoreCase.Equals(tuple.Item1, clientIdentity.name))
                {
                    return tuple.Item2 == clientIdentity.admin;
                }
                return false;
            }
            return true;
            static (string name, bool admin) getIdentity(bool impersonating)
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent(impersonating);
                return new ValueTuple<string, bool>(item2: new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator), item1: current.Name);
            }
        }

        internal static NamedPipeServerStream CreateServer(string pipeName, PipeDirection? pipeDirection = null)
        {
            PipeOptions options = PipeOptions.WriteThrough | PipeOptions.Asynchronous;
            return CreateServer(pipeName, pipeDirection ?? PipeDirection.InOut, -1, PipeTransmissionMode.Byte, options, 65536, 65536);
        }

        private static NamedPipeServerStream CreateServer(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
        {
            return new NamedPipeServerStream(GetPipeNameOrPath(pipeName), direction, maxNumberOfServerInstances, transmissionMode, options | CurrentUserOption, inBufferSize, outBufferSize, CreatePipeSecurity(), HandleInheritability.None);
        }

        internal static bool CheckPipeConnectionOwnership(NamedPipeClientStream pipeStream)
        {
            if (PlatformInformation.IsWindows)
            {
                SecurityIdentifier owner = WindowsIdentity.GetCurrent().Owner;
                IdentityReference owner2 = pipeStream.GetAccessControl().GetOwner(typeof(SecurityIdentifier));
                return owner.Equals(owner2);
            }
            return true;
        }

        internal static PipeSecurity? CreatePipeSecurity()
        {
            if (PlatformInformation.IsRunningOnMono)
            {
                return null;
            }
            PipeSecurity pipeSecurity = new();
            SecurityIdentifier owner = WindowsIdentity.GetCurrent().Owner;
            PipeAccessRule rule = new(owner, PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow);
            pipeSecurity.AddAccessRule(rule);
            pipeSecurity.SetOwner(owner);
            return pipeSecurity;
        }
    }
}
