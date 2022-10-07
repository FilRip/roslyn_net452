Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSourceDocumentIndex
		Inherits BoundExpression
		Private ReadOnly _Document As DebugSourceDocument

		Public ReadOnly Property Document As DebugSourceDocument
			Get
				Return Me._Document
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal document As DebugSourceDocument, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.SourceDocumentIndex, syntax, type, hasErrors)
			Me._Document = document
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal document As DebugSourceDocument, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.SourceDocumentIndex, syntax, type)
			Me._Document = document
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSourceDocumentIndex(Me)
		End Function

		Public Function Update(ByVal document As DebugSourceDocument, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex
			Dim boundSourceDocumentIndex As Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex
			If (document <> Me.Document OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundSourceDocumentIndex1 As Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex(MyBase.Syntax, document, type, MyBase.HasErrors)
				boundSourceDocumentIndex1.CopyAttributes(Me)
				boundSourceDocumentIndex = boundSourceDocumentIndex1
			Else
				boundSourceDocumentIndex = Me
			End If
			Return boundSourceDocumentIndex
		End Function
	End Class
End Namespace