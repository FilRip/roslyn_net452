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
        private static readonly PipeOptions CurrentUserOption = PlatformInformation.IsRunningOnMono ? (PipeOptions)536870912 : PipeOptions.None;

        private static string GetPipeNameOrPath(string pipeName) => PlatformInformation.IsUnix ? Path.Combine("/tmp", pipeName) : pipeName;

        internal static NamedPipeClientStream CreateClient(
            string serverName,
            string pipeName,
            PipeDirection direction,
            PipeOptions options)
        {
            return new NamedPipeClientStream(serverName, GetPipeNameOrPath(pipeName), direction, options | NamedPipeUtil.CurrentUserOption);
        }

        internal static bool CheckClientElevationMatches(NamedPipeServerStream pipeStream)
        {
            if (!PlatformInformation.IsWindows)
                return true;
            (string name, bool admin) = getIdentity(false);
            (string, bool) clientIdentity = ("", false);
            pipeStream.RunAsClient(() => clientIdentity = getIdentity(true));
            return StringComparer.OrdinalIgnoreCase.Equals(name, clientIdentity.Item1) && admin == clientIdentity.Item2;

            static (string name, bool admin) getIdentity(bool impersonating)
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent(impersonating);
                return (current.Name, new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator));
            }
        }

        internal static NamedPipeServerStream CreateServer(
            string pipeName,
            PipeDirection? pipeDirection = null)
        {
            PipeOptions options = PipeOptions.WriteThrough | PipeOptions.Asynchronous;
            return CreateServer(pipeName, pipeDirection ?? PipeDirection.InOut, -1, PipeTransmissionMode.Byte, options, 65536, 65536);
        }

        private static NamedPipeServerStream CreateServer(
            string pipeName,
            PipeDirection direction,
            int maxNumberOfServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions options,
            int inBufferSize,
            int outBufferSize)
        {
            return new NamedPipeServerStream(GetPipeNameOrPath(pipeName), direction, maxNumberOfServerInstances, transmissionMode, options | NamedPipeUtil.CurrentUserOption, inBufferSize, outBufferSize, NamedPipeUtil.CreatePipeSecurity(), HandleInheritability.None);
        }

        internal static bool CheckPipeConnectionOwnership(NamedPipeClientStream pipeStream) => !PlatformInformation.IsWindows || WindowsIdentity.GetCurrent().Owner.Equals(pipeStream.GetAccessControl().GetOwner(typeof(SecurityIdentifier)));

        internal static PipeSecurity? CreatePipeSecurity()
        {
            if (PlatformInformation.IsRunningOnMono)
                return null;
            PipeSecurity pipeSecurity = new();
            SecurityIdentifier owner = WindowsIdentity.GetCurrent().Owner;
            pipeSecurity.AddAccessRule(new PipeAccessRule(owner, PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
            pipeSecurity.SetOwner(owner);
            return pipeSecurity;
        }
    }
}
