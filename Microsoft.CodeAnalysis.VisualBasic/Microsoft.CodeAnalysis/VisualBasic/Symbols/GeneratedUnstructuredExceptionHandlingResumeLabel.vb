Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class GeneratedUnstructuredExceptionHandlingResumeLabel
		Inherits GeneratedLabelSymbol
		Public ReadOnly ResumeStatement As StatementSyntax

		Public Sub New(ByVal resumeStmt As StatementSyntax)
			MyBase.New("$VB$UnstructuredExceptionHandling_TargetResumeLabel")
			Me.ResumeStatement = resumeStmt
		End Sub
	End Class
End Namespace