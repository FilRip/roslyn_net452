using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class StateMachineStates
	{
		public static int FinishedStateMachine = -2;

		public static int NotStartedStateMachine = -1;

		public static int FirstUnusedState = 0;
	}
}
