using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class SourceMemberFlagsExtensions
	{
		internal static MethodKind ToMethodKind(this SourceMemberFlags flags)
		{
			return (MethodKind)(((int)flags >> 27) & 0x1F);
		}
	}
}
