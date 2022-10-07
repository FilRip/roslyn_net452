Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundDelegateCreationExpression
		Inherits BoundExpression
		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _Method As MethodSymbol

		Private ReadOnly _RelaxationLambdaOpt As BoundLambda

		Private ReadOnly _RelaxationReceiverPlaceholderOpt As BoundRValuePlaceholder

		Private ReadOnly _MethodGroupOpt As BoundMethodGroup

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.ReceiverOpt)
			End Get
		End Property

		Public ReadOnly Property Method As MethodSymbol
			Get
				Return Me._Method
			End Get
		End Property

		Public ReadOnly Property MethodGroupOpt As BoundMethodGroup
			Get
				Return Me._MethodGroupOpt
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public ReadOnly Property RelaxationLambdaOpt As BoundLambda
			Get
				Return Me._RelaxationLambdaOpt
			End Get
		End Property

		Public ReadOnly Property RelaxationReceiverPlaceholderOpt As BoundRValuePlaceholder
			Get
				Return Me._RelaxationReceiverPlaceholderOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiverOpt As BoundExpression, ByVal method As MethodSymbol, ByVal relaxationLambdaOpt As BoundLambda, ByVal relaxationReceiverPlaceholderOpt As BoundRValuePlaceholder, ByVal methodGroupOpt As BoundMethodGroup, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.DelegateCreationExpression, syntax, type, If(hasErrors OrElse receiverOpt.NonNullAndHasErrors() OrElse relaxationLambdaOpt.NonNullAndHasErrors() OrElse relaxationReceiverPlaceholderOpt.NonNullAndHasErrors(), True, methodGroupOpt.NonNullAndHasErrors()))
			Me._ReceiverOpt = receiverOpt
			Me._Method = method
			Me._RelaxationLambdaOpt = relaxationLambdaOpt
			Me._RelaxationReceiverPlaceholderOpt = relaxationReceiverPlaceholderOpt
			Me._MethodGroupOpt = methodGroupOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitDelegateCreationExpression(Me)
		End Function

		Public Function Update(ByVal receiverOpt As BoundExpression, ByVal method As MethodSymbol, ByVal relaxationLambdaOpt As BoundLambda, ByVal relaxationReceiverPlaceholderOpt As BoundRValuePlaceholder, ByVal methodGroupOpt As BoundMethodGroup, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression
			Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression
			If (receiverOpt <> Me.ReceiverOpt OrElse CObj(method) <> CObj(Me.Method) OrElse relaxationLambdaOpt <> Me.RelaxationLambdaOpt OrElse relaxationReceiverPlaceholderOpt <> Me.RelaxationReceiverPlaceholderOpt OrElse methodGroupOpt <> Me.MethodGroupOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundDelegateCreationExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(MyBase.Syntax, receiverOpt, method, relaxationLambdaOpt, relaxationReceiverPlaceholderOpt, methodGroupOpt, type, MyBase.HasErrors)
				boundDelegateCreationExpression1.CopyAttributes(Me)
				boundDelegateCreationExpression = boundDelegateCreationExpression1
			Else
				boundDelegateCreationExpression = Me
			End If
			Return boundDelegateCreationExpression
		End Function
	End Class
End Namespace