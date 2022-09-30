namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeBCryptAlgorithmHandle : SafeBCryptHandle
	{
		private SafeBCryptAlgorithmHandle()
		{
		}

		protected sealed override bool ReleaseHandle()
		{
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptCloseAlgorithmProvider(handle, 0);
			return nTSTATUS == Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
		}
	}
}
