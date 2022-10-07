Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUnstructuredExceptionHandlingCatchFilter
		Inherits BoundExpression
		Private ReadOnly _ActiveHandlerLocal As BoundLocal

		Private ReadOnly _ResumeTargetLocal As BoundLocal

		Public ReadOnly Property ActiveHandlerLocal As BoundLocal
			Get
				Return Me._ActiveHandlerLocal
			End Get
		End Property

		Public ReadOnly Property ResumeTargetLocal As BoundLocal
			Get
				Return Me._ResumeTargetLocal
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal activeHandlerLocal As BoundLocal, ByVal resumeTargetLocal As BoundLocal, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UnstructuredExceptionHandlingCatchFilter, syntax, type, If(hasErrors OrElse activeHandlerLocal.NonNullAndHasErrors(), True, resumeTargetLocal.NonNullAndHasErrors()))
			Me._ActiveHandlerLocal = activeHandlerLocal
			Me._ResumeTargetLocal = resumeTargetLocal
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnstructuredExceptionHandlingCatchFilter(Me)
		End Function

		Public Function Update(ByVal activeHandlerLocal As BoundLocal, ByVal resumeTargetLocal As BoundLocal, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingCatchFilter
			Dim boundUnstructuredExceptionHandlingCatchFilter As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingCatchFilter
			If (activeHandlerLocal <> Me.ActiveHandlerLocal OrElse resumeTargetLocal <> Me.ResumeTargetLocal OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundUnstructuredExceptionHandlingCatchFilter1 As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingCatchFilter = New Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingCatchFilter(MyBase.Syntax, activeHandlerLocal, resumeTargetLocal, type, MyBase.HasErrors)
				boundUnstructuredExceptionHandlingCatchFilter1.CopyAttributes(Me)
				boundUnstructuredExceptionHandlingCatchFilter = boundUnstructuredExceptionHandlingCatchFilter1
			Else
				boundUnstructuredExceptionHandlingCatchFilter = Me
			End If
			Return boundUnstructuredExceptionHandlingCatchFilter
		End Function
	End Class
End Namespace