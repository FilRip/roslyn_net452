Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRelaxationLambda
		Inherits BoundExtendedConversionInfo
		Private ReadOnly _Lambda As BoundLambda

		Private ReadOnly _ReceiverPlaceholderOpt As BoundRValuePlaceholder

		Public ReadOnly Property Lambda As BoundLambda
			Get
				Return Me._Lambda
			End Get
		End Property

		Public ReadOnly Property ReceiverPlaceholderOpt As BoundRValuePlaceholder
			Get
				Return Me._ReceiverPlaceholderOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lambda As BoundLambda, ByVal receiverPlaceholderOpt As BoundRValuePlaceholder, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RelaxationLambda, syntax, If(hasErrors OrElse lambda.NonNullAndHasErrors(), True, receiverPlaceholderOpt.NonNullAndHasErrors()))
			Me._Lambda = lambda
			Me._ReceiverPlaceholderOpt = receiverPlaceholderOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRelaxationLambda(Me)
		End Function

		Public Function Update(ByVal lambda As BoundLambda, ByVal receiverPlaceholderOpt As BoundRValuePlaceholder) As Microsoft.CodeAnalysis.VisualBasic.BoundRelaxationLambda
			Dim boundRelaxationLambda As Microsoft.CodeAnalysis.VisualBasic.BoundRelaxationLambda
			If (lambda <> Me.Lambda OrElse receiverPlaceholderOpt <> Me.ReceiverPlaceholderOpt) Then
				Dim boundRelaxationLambda1 As Microsoft.CodeAnalysis.VisualBasic.BoundRelaxationLambda = New Microsoft.CodeAnalysis.VisualBasic.BoundRelaxationLambda(MyBase.Syntax, lambda, receiverPlaceholderOpt, MyBase.HasErrors)
				boundRelaxationLambda1.CopyAttributes(Me)
				boundRelaxationLambda = boundRelaxationLambda1
			Else
				boundRelaxationLambda = Me
			End If
			Return boundRelaxationLambda
		End Function
	End Class
End Namespace