Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure AwaitExpressionInfo
		Private ReadOnly _getAwaiter As IMethodSymbol

		Private ReadOnly _isCompleted As IPropertySymbol

		Private ReadOnly _getResult As IMethodSymbol

		Public ReadOnly Property GetAwaiterMethod As IMethodSymbol
			Get
				Return Me._getAwaiter
			End Get
		End Property

		Public ReadOnly Property GetResultMethod As IMethodSymbol
			Get
				Return Me._getResult
			End Get
		End Property

		Public ReadOnly Property IsCompletedProperty As IPropertySymbol
			Get
				Return Me._isCompleted
			End Get
		End Property

		Friend Sub New(ByVal getAwaiter As IMethodSymbol, ByVal isCompleted As IPropertySymbol, ByVal getResult As IMethodSymbol)
			Me = New AwaitExpressionInfo() With
			{
				._getAwaiter = getAwaiter,
				._isCompleted = isCompleted,
				._getResult = getResult
			}
		End Sub
	End Structure
End Namespace