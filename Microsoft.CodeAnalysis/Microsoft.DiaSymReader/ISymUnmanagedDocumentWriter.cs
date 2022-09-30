using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.DiaSymReader
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("B01FAFEB-C450-3A4D-BEEC-B4CEEC01E006")]
    [SuppressUnmanagedCodeSecurity]
    internal interface ISymUnmanagedDocumentWriter
    {
        unsafe void SetSource(uint sourceSize, byte* source);

        unsafe void SetCheckSum(Guid algorithmId, uint checkSumSize, byte* checkSum);
    }
}
