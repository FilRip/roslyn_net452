Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module StateMachineStates
		Public FinishedStateMachine As Integer

		Public NotStartedStateMachine As Integer

		Public FirstUnusedState As Integer

		Sub New()
			StateMachineStates.FinishedStateMachine = -2
			StateMachineStates.NotStartedStateMachine = -1
			StateMachineStates.FirstUnusedState = 0
		End Sub
	End Module
End Namespace