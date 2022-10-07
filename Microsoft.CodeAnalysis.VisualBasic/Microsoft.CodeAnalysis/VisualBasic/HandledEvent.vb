Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public NotInheritable Class HandledEvent
		Private ReadOnly _kind As HandledEventKind

		Private ReadOnly _eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol

		Private ReadOnly _WithEventsContainerOpt As PropertySymbol

		Private ReadOnly _WithEventsSourcePropertyOpt As PropertySymbol

		Friend ReadOnly delegateCreation As BoundExpression

		Friend ReadOnly hookupMethod As MethodSymbol

		Public ReadOnly Property EventContainer As IPropertySymbol
			Get
				Return Me._WithEventsContainerOpt
			End Get
		End Property

		Public ReadOnly Property EventSymbol As IEventSymbol
			Get
				Return Me._eventSymbol
			End Get
		End Property

		Public ReadOnly Property HandlesKind As HandledEventKind
			Get
				Return Me._kind
			End Get
		End Property

		Public ReadOnly Property WithEventsSourceProperty As IPropertySymbol
			Get
				Return Me._WithEventsSourcePropertyOpt
			End Get
		End Property

		Friend Sub New(ByVal kind As HandledEventKind, ByVal eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal withEventsContainerOpt As PropertySymbol, ByVal withEventsSourcePropertyOpt As PropertySymbol, ByVal delegateCreation As BoundExpression, ByVal hookupMethod As MethodSymbol)
			MyBase.New()
			Me._kind = kind
			Me._eventSymbol = eventSymbol
			Me._WithEventsContainerOpt = withEventsContainerOpt
			Me._WithEventsSourcePropertyOpt = withEventsSourcePropertyOpt
			Me.delegateCreation = delegateCreation
			Me.hookupMethod = hookupMethod
		End Sub
	End Class
End Namespace