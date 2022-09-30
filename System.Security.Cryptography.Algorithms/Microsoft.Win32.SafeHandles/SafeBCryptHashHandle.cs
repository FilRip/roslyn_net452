namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeBCryptHashHandle : SafeBCryptHandle
	{
		private SafeBCryptHashHandle()
		{
		}

		protected sealed override bool ReleaseHandle()
		{
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptDestroyHash(handle);
			return nTSTATUS == Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
		}
	}
}
