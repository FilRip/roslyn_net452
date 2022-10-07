Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBadExpression
		Inherits BoundExpression
		Implements IBoundInvalidNode
		Private ReadOnly _ResultKind As LookupResultKind

		Private ReadOnly _Symbols As ImmutableArray(Of Symbol)

		Private ReadOnly _ChildBoundNodes As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property ChildBoundNodes As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ChildBoundNodes
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(Me.ChildBoundNodes)
			End Get
		End Property

		ReadOnly Property IBoundInvalidNode_InvalidNodeChildren As ImmutableArray(Of BoundNode) Implements IBoundInvalidNode.InvalidNodeChildren
			Get
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(Me.ChildBoundNodes)
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me._ResultKind
			End Get
		End Property

		Public ReadOnly Property Symbols As ImmutableArray(Of Symbol)
			Get
				Return Me._Symbols
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal resultKind As LookupResultKind, ByVal symbols As ImmutableArray(Of Symbol), ByVal childBoundNodes As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.BadExpression, syntax, type, If(hasErrors, True, childBoundNodes.NonNullAndHasErrors()))
			Me._ResultKind = resultKind
			Me._Symbols = symbols
			Me._ChildBoundNodes = childBoundNodes
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBadExpression(Me)
		End Function

		Public Function Update(ByVal resultKind As LookupResultKind, ByVal symbols As ImmutableArray(Of Symbol), ByVal childBoundNodes As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression
			If (resultKind <> Me.ResultKind OrElse symbols <> Me.Symbols OrElse childBoundNodes <> Me.ChildBoundNodes OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundBadExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(MyBase.Syntax, resultKind, symbols, childBoundNodes, type, MyBase.HasErrors)
				boundBadExpression1.CopyAttributes(Me)
				boundBadExpression = boundBadExpression1
			Else
				boundBadExpression = Me
			End If
			Return boundBadExpression
		End Function
	End Class
End Namespace