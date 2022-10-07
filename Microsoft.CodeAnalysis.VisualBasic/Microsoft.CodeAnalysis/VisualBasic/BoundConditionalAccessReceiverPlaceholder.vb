Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConditionalAccessReceiverPlaceholder
		Inherits BoundRValuePlaceholderBase
		Private ReadOnly _PlaceholderId As Integer

		Public ReadOnly Property PlaceholderId As Integer
			Get
				Return Me._PlaceholderId
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal placeholderId As Integer, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ConditionalAccessReceiverPlaceholder, syntax, type, hasErrors)
			Me._PlaceholderId = placeholderId
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal placeholderId As Integer, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.ConditionalAccessReceiverPlaceholder, syntax, type)
			Me._PlaceholderId = placeholderId
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConditionalAccessReceiverPlaceholder(Me)
		End Function

		Public Function Update(ByVal placeholderId As Integer, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder
			Dim boundConditionalAccessReceiverPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder
			If (placeholderId <> Me.PlaceholderId OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundConditionalAccessReceiverPlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder(MyBase.Syntax, placeholderId, type, MyBase.HasErrors)
				boundConditionalAccessReceiverPlaceholder1.CopyAttributes(Me)
				boundConditionalAccessReceiverPlaceholder = boundConditionalAccessReceiverPlaceholder1
			Else
				boundConditionalAccessReceiverPlaceholder = Me
			End If
			Return boundConditionalAccessReceiverPlaceholder
		End Function
	End Class
End Namespace