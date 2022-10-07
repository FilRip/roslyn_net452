Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundQueryableSource
		Inherits BoundQueryClauseBase
		Private ReadOnly _Source As BoundQueryPart

		Private ReadOnly _RangeVariableOpt As RangeVariableSymbol

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Source)
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.Source.ExpressionSymbol
			End Get
		End Property

		Public ReadOnly Property RangeVariableOpt As RangeVariableSymbol
			Get
				Return Me._RangeVariableOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me.Source.ResultKind
			End Get
		End Property

		Public ReadOnly Property Source As BoundQueryPart
			Get
				Return Me._Source
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal source As BoundQueryPart, ByVal rangeVariableOpt As RangeVariableSymbol, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.QueryableSource, syntax, rangeVariables, compoundVariableType, binders, type, If(hasErrors, True, source.NonNullAndHasErrors()))
			Me._Source = source
			Me._RangeVariableOpt = rangeVariableOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitQueryableSource(Me)
		End Function

		Public Function Update(ByVal source As BoundQueryPart, ByVal rangeVariableOpt As RangeVariableSymbol, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource
			Dim boundQueryableSource As Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource
			If (source <> Me.Source OrElse CObj(rangeVariableOpt) <> CObj(Me.RangeVariableOpt) OrElse rangeVariables <> MyBase.RangeVariables OrElse CObj(compoundVariableType) <> CObj(MyBase.CompoundVariableType) OrElse binders <> MyBase.Binders OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundQueryableSource1 As Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource = New Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource(MyBase.Syntax, source, rangeVariableOpt, rangeVariables, compoundVariableType, binders, type, MyBase.HasErrors)
				boundQueryableSource1.CopyAttributes(Me)
				boundQueryableSource = boundQueryableSource1
			Else
				boundQueryableSource = Me
			End If
			Return boundQueryableSource
		End Function
	End Class
End Namespace