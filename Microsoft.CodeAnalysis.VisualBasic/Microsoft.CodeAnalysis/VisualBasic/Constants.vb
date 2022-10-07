Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module Constants
		Friend Const ATTACH_LISTENER_PREFIX As String = "add_"

		Friend Const REMOVE_LISTENER_PREFIX As String = "remove_"

		Friend Const FIRE_LISTENER_PREFIX As String = "raise_"

		Friend Const EVENT_DELEGATE_SUFFIX As String = "EventHandler"

		Friend Const EVENT_VARIABLE_SUFFIX As String = "Event"
	End Module
End Namespace