using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class Constants
	{
		internal const string ATTACH_LISTENER_PREFIX = "add_";

		internal const string REMOVE_LISTENER_PREFIX = "remove_";

		internal const string FIRE_LISTENER_PREFIX = "raise_";

		internal const string EVENT_DELEGATE_SUFFIX = "EventHandler";

		internal const string EVENT_VARIABLE_SUFFIX = "Event";
	}
}
