Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundQueryClauseBase
		Inherits BoundQueryPart
		Private ReadOnly _RangeVariables As ImmutableArray(Of RangeVariableSymbol)

		Private ReadOnly _CompoundVariableType As TypeSymbol

		Private ReadOnly _Binders As ImmutableArray(Of Binder)

		Public ReadOnly Property Binders As ImmutableArray(Of Binder)
			Get
				Return Me._Binders
			End Get
		End Property

		Public ReadOnly Property CompoundVariableType As TypeSymbol
			Get
				Return Me._CompoundVariableType
			End Get
		End Property

		Public ReadOnly Property RangeVariables As ImmutableArray(Of RangeVariableSymbol)
			Get
				Return Me._RangeVariables
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, type, hasErrors)
			Me._RangeVariables = rangeVariables
			Me._CompoundVariableType = compoundVariableType
			Me._Binders = binders
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol)
			MyBase.New(kind, syntax, type)
			Me._RangeVariables = rangeVariables
			Me._CompoundVariableType = compoundVariableType
			Me._Binders = binders
		End Sub
	End Class
End Namespace