using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal class SafeNCryptHandle : SafeHandle
	{
		public override bool IsInvalid => handle == IntPtr.Zero;

		public SafeNCryptHandle()
			: base(IntPtr.Zero, ownsHandle: true)
		{
		}

		protected override bool ReleaseHandle()
		{
			Interop.NCrypt.ErrorCode errorCode = Interop.NCrypt.NCryptFreeObject(handle);
			bool result = errorCode == Interop.NCrypt.ErrorCode.ERROR_SUCCESS;
			handle = IntPtr.Zero;
			return result;
		}
	}
}
