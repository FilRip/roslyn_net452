Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBadStatement
		Inherits BoundStatement
		Implements IBoundInvalidNode
		Private ReadOnly _ChildBoundNodes As ImmutableArray(Of BoundNode)

		Public ReadOnly Property ChildBoundNodes As ImmutableArray(Of BoundNode)
			Get
				Return Me._ChildBoundNodes
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return Me.ChildBoundNodes
			End Get
		End Property

		ReadOnly Property IBoundInvalidNode_InvalidNodeChildren As ImmutableArray(Of BoundNode) Implements IBoundInvalidNode.InvalidNodeChildren
			Get
				Return Me.ChildBoundNodes
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal childBoundNodes As ImmutableArray(Of BoundNode), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.BadStatement, syntax, If(hasErrors, True, childBoundNodes.NonNullAndHasErrors()))
			Me._ChildBoundNodes = childBoundNodes
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBadStatement(Me)
		End Function

		Public Function Update(ByVal childBoundNodes As ImmutableArray(Of BoundNode)) As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement
			Dim boundBadStatement As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement
			If (childBoundNodes = Me.ChildBoundNodes) Then
				boundBadStatement = Me
			Else
				Dim boundBadStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement(MyBase.Syntax, childBoundNodes, MyBase.HasErrors)
				boundBadStatement1.CopyAttributes(Me)
				boundBadStatement = boundBadStatement1
			End If
			Return boundBadStatement
		End Function
	End Class
End Namespace