using System;
using System.Runtime.InteropServices;

namespace Internal.NativeCrypto
{
	internal abstract class SafeBCryptHandle : SafeHandle
	{
		public sealed override bool IsInvalid => handle == IntPtr.Zero;

		public SafeBCryptHandle()
			: base(IntPtr.Zero, ownsHandle: true)
		{
		}
	}
}
