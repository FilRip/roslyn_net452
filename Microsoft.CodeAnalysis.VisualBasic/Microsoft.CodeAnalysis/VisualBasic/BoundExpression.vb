Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundExpression
		Inherits BoundNode
		Private ReadOnly _Type As TypeSymbol

		Public Overridable ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property IsConstant As Boolean
			Get
				Return CObj(Me.ConstantValueOpt) <> CObj(Nothing)
			End Get
		End Property

		Public Overridable ReadOnly Property IsLValue As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property ResultKind As LookupResultKind
			Get
				Return LookupResultKind.Good
			End Get
		End Property

		Public Overridable ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property Type As TypeSymbol
			Get
				Return Me._Type
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, hasErrors)
			Me._Type = type
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(kind, syntax)
			Me._Type = type
		End Sub

		Public Function MakeRValue() As BoundExpression
			Return Me.MakeRValueImpl()
		End Function

		Protected Overridable Function MakeRValueImpl() As BoundExpression
			Return Me
		End Function
	End Class
End Namespace