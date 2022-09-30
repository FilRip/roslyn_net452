using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeUnicodeStringHandle : SafeHandle
	{
		public sealed override bool IsInvalid => handle == IntPtr.Zero;

		public SafeUnicodeStringHandle(string s)
			: base(IntPtr.Zero, ownsHandle: true)
		{
			handle = Marshal.StringToHGlobalUni(s);
		}

		protected sealed override bool ReleaseHandle()
		{
			Marshal.FreeHGlobal(handle);
			return true;
		}
	}
}
