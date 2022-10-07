Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUnstructuredExceptionHandlingStatement
		Inherits BoundStatement
		Private ReadOnly _ContainsOnError As Boolean

		Private ReadOnly _ContainsResume As Boolean

		Private ReadOnly _ResumeWithoutLabelOpt As StatementSyntax

		Private ReadOnly _TrackLineNumber As Boolean

		Private ReadOnly _Body As BoundBlock

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Body)
			End Get
		End Property

		Public ReadOnly Property ContainsOnError As Boolean
			Get
				Return Me._ContainsOnError
			End Get
		End Property

		Public ReadOnly Property ContainsResume As Boolean
			Get
				Return Me._ContainsResume
			End Get
		End Property

		Public ReadOnly Property ResumeWithoutLabelOpt As StatementSyntax
			Get
				Return Me._ResumeWithoutLabelOpt
			End Get
		End Property

		Public ReadOnly Property TrackLineNumber As Boolean
			Get
				Return Me._TrackLineNumber
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal containsOnError As Boolean, ByVal containsResume As Boolean, ByVal resumeWithoutLabelOpt As StatementSyntax, ByVal trackLineNumber As Boolean, ByVal body As BoundBlock, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UnstructuredExceptionHandlingStatement, syntax, If(hasErrors, True, body.NonNullAndHasErrors()))
			Me._ContainsOnError = containsOnError
			Me._ContainsResume = containsResume
			Me._ResumeWithoutLabelOpt = resumeWithoutLabelOpt
			Me._TrackLineNumber = trackLineNumber
			Me._Body = body
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnstructuredExceptionHandlingStatement(Me)
		End Function

		Public Function Update(ByVal containsOnError As Boolean, ByVal containsResume As Boolean, ByVal resumeWithoutLabelOpt As StatementSyntax, ByVal trackLineNumber As Boolean, ByVal body As BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingStatement
			Dim boundUnstructuredExceptionHandlingStatement As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingStatement
			If (containsOnError <> Me.ContainsOnError OrElse containsResume <> Me.ContainsResume OrElse resumeWithoutLabelOpt <> Me.ResumeWithoutLabelOpt OrElse trackLineNumber <> Me.TrackLineNumber OrElse body <> Me.Body) Then
				Dim boundUnstructuredExceptionHandlingStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionHandlingStatement(MyBase.Syntax, containsOnError, containsResume, resumeWithoutLabelOpt, trackLineNumber, body, MyBase.HasErrors)
				boundUnstructuredExceptionHandlingStatement1.CopyAttributes(Me)
				boundUnstructuredExceptionHandlingStatement = boundUnstructuredExceptionHandlingStatement1
			Else
				boundUnstructuredExceptionHandlingStatement = Me
			End If
			Return boundUnstructuredExceptionHandlingStatement
		End Function
	End Class
End Namespace