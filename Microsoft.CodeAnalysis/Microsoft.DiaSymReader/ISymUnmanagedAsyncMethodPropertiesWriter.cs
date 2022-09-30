using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.DiaSymReader
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("FC073774-1739-4232-BD56-A027294BEC15")]
    [SuppressUnmanagedCodeSecurity]
    internal interface ISymUnmanagedAsyncMethodPropertiesWriter
    {
        void DefineKickoffMethod(int kickoffMethod);

        void DefineCatchHandlerILOffset(int catchHandlerOffset);

        unsafe void DefineAsyncStepInfo(int count, int* yieldOffsets, int* breakpointOffset, int* breakpointMethod);
    }
}
