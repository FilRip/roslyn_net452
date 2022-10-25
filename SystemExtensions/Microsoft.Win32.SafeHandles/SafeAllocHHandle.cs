using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeAllocHHandle : SafeBuffer
    {
        internal static SafeAllocHHandle InvalidHandle => new(IntPtr.Zero);

        private SafeAllocHHandle()
            : base(ownsHandle: true)
        {
        }

        internal SafeAllocHHandle(IntPtr handle)
            : base(ownsHandle: true)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(handle);
            }
            return true;
        }
    }
}
