Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundPreviousSubmissionReference
		Inherits BoundExpression
		Private ReadOnly _SourceType As NamedTypeSymbol

		Public ReadOnly Property SourceType As NamedTypeSymbol
			Get
				Return Me._SourceType
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal sourceType As NamedTypeSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.PreviousSubmissionReference, syntax, type, hasErrors)
			Me._SourceType = sourceType
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal sourceType As NamedTypeSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.PreviousSubmissionReference, syntax, type)
			Me._SourceType = sourceType
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitPreviousSubmissionReference(Me)
		End Function

		Public Function Update(ByVal sourceType As NamedTypeSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundPreviousSubmissionReference
			Dim boundPreviousSubmissionReference As Microsoft.CodeAnalysis.VisualBasic.BoundPreviousSubmissionReference
			If (CObj(sourceType) <> CObj(Me.SourceType) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundPreviousSubmissionReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundPreviousSubmissionReference = New Microsoft.CodeAnalysis.VisualBasic.BoundPreviousSubmissionReference(MyBase.Syntax, sourceType, type, MyBase.HasErrors)
				boundPreviousSubmissionReference1.CopyAttributes(Me)
				boundPreviousSubmissionReference = boundPreviousSubmissionReference1
			Else
				boundPreviousSubmissionReference = Me
			End If
			Return boundPreviousSubmissionReference
		End Function
	End Class
End Namespace