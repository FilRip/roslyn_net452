Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUnstructuredExceptionResumeSwitch
		Inherits BoundStatement
		Private ReadOnly _ResumeTargetTemporary As BoundLocal

		Private ReadOnly _ResumeLabel As BoundLabelStatement

		Private ReadOnly _ResumeNextLabel As BoundLabelStatement

		Private ReadOnly _Jumps As ImmutableArray(Of BoundGotoStatement)

		Public ReadOnly Property Jumps As ImmutableArray(Of BoundGotoStatement)
			Get
				Return Me._Jumps
			End Get
		End Property

		Public ReadOnly Property ResumeLabel As BoundLabelStatement
			Get
				Return Me._ResumeLabel
			End Get
		End Property

		Public ReadOnly Property ResumeNextLabel As BoundLabelStatement
			Get
				Return Me._ResumeNextLabel
			End Get
		End Property

		Public ReadOnly Property ResumeTargetTemporary As BoundLocal
			Get
				Return Me._ResumeTargetTemporary
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal resumeTargetTemporary As BoundLocal, ByVal resumeLabel As BoundLabelStatement, ByVal resumeNextLabel As BoundLabelStatement, ByVal jumps As ImmutableArray(Of BoundGotoStatement), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UnstructuredExceptionResumeSwitch, syntax, If(hasErrors OrElse resumeTargetTemporary.NonNullAndHasErrors() OrElse resumeLabel.NonNullAndHasErrors() OrElse resumeNextLabel.NonNullAndHasErrors(), True, jumps.NonNullAndHasErrors()))
			Me._ResumeTargetTemporary = resumeTargetTemporary
			Me._ResumeLabel = resumeLabel
			Me._ResumeNextLabel = resumeNextLabel
			Me._Jumps = jumps
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnstructuredExceptionResumeSwitch(Me)
		End Function

		Public Function Update(ByVal resumeTargetTemporary As BoundLocal, ByVal resumeLabel As BoundLabelStatement, ByVal resumeNextLabel As BoundLabelStatement, ByVal jumps As ImmutableArray(Of BoundGotoStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionResumeSwitch
			Dim boundUnstructuredExceptionResumeSwitch As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionResumeSwitch
			If (resumeTargetTemporary <> Me.ResumeTargetTemporary OrElse resumeLabel <> Me.ResumeLabel OrElse resumeNextLabel <> Me.ResumeNextLabel OrElse jumps <> Me.Jumps) Then
				Dim boundUnstructuredExceptionResumeSwitch1 As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionResumeSwitch = New Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionResumeSwitch(MyBase.Syntax, resumeTargetTemporary, resumeLabel, resumeNextLabel, jumps, MyBase.HasErrors)
				boundUnstructuredExceptionResumeSwitch1.CopyAttributes(Me)
				boundUnstructuredExceptionResumeSwitch = boundUnstructuredExceptionResumeSwitch1
			Else
				boundUnstructuredExceptionResumeSwitch = Me
			End If
			Return boundUnstructuredExceptionResumeSwitch
		End Function
	End Class
End Namespace