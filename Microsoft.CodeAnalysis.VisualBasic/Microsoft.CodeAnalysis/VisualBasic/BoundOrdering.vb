Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundOrdering
		Inherits BoundQueryPart
		Private ReadOnly _UnderlyingExpression As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.UnderlyingExpression)
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.UnderlyingExpression.ExpressionSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me.UnderlyingExpression.ResultKind
			End Get
		End Property

		Public ReadOnly Property UnderlyingExpression As BoundExpression
			Get
				Return Me._UnderlyingExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal underlyingExpression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Ordering, syntax, type, If(hasErrors, True, underlyingExpression.NonNullAndHasErrors()))
			Me._UnderlyingExpression = underlyingExpression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitOrdering(Me)
		End Function

		Public Function Update(ByVal underlyingExpression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundOrdering
			Dim boundOrdering As Microsoft.CodeAnalysis.VisualBasic.BoundOrdering
			If (underlyingExpression <> Me.UnderlyingExpression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundOrdering1 As Microsoft.CodeAnalysis.VisualBasic.BoundOrdering = New Microsoft.CodeAnalysis.VisualBasic.BoundOrdering(MyBase.Syntax, underlyingExpression, type, MyBase.HasErrors)
				boundOrdering1.CopyAttributes(Me)
				boundOrdering = boundOrdering1
			Else
				boundOrdering = Me
			End If
			Return boundOrdering
		End Function
	End Class
End Namespace