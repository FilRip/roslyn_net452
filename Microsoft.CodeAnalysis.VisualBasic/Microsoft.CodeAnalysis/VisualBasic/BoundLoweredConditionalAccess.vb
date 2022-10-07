Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLoweredConditionalAccess
		Inherits BoundExpression
		Private ReadOnly _ReceiverOrCondition As BoundExpression

		Private ReadOnly _CaptureReceiver As Boolean

		Private ReadOnly _PlaceholderId As Integer

		Private ReadOnly _WhenNotNull As BoundExpression

		Private ReadOnly _WhenNullOpt As BoundExpression

		Public ReadOnly Property CaptureReceiver As Boolean
			Get
				Return Me._CaptureReceiver
			End Get
		End Property

		Public ReadOnly Property PlaceholderId As Integer
			Get
				Return Me._PlaceholderId
			End Get
		End Property

		Public ReadOnly Property ReceiverOrCondition As BoundExpression
			Get
				Return Me._ReceiverOrCondition
			End Get
		End Property

		Public ReadOnly Property WhenNotNull As BoundExpression
			Get
				Return Me._WhenNotNull
			End Get
		End Property

		Public ReadOnly Property WhenNullOpt As BoundExpression
			Get
				Return Me._WhenNullOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiverOrCondition As BoundExpression, ByVal captureReceiver As Boolean, ByVal placeholderId As Integer, ByVal whenNotNull As BoundExpression, ByVal whenNullOpt As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LoweredConditionalAccess, syntax, type, If(hasErrors OrElse receiverOrCondition.NonNullAndHasErrors() OrElse whenNotNull.NonNullAndHasErrors(), True, whenNullOpt.NonNullAndHasErrors()))
			Me._ReceiverOrCondition = receiverOrCondition
			Me._CaptureReceiver = captureReceiver
			Me._PlaceholderId = placeholderId
			Me._WhenNotNull = whenNotNull
			Me._WhenNullOpt = whenNullOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLoweredConditionalAccess(Me)
		End Function

		Public Function Update(ByVal receiverOrCondition As BoundExpression, ByVal captureReceiver As Boolean, ByVal placeholderId As Integer, ByVal whenNotNull As BoundExpression, ByVal whenNullOpt As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess
			Dim boundLoweredConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess
			If (receiverOrCondition <> Me.ReceiverOrCondition OrElse captureReceiver <> Me.CaptureReceiver OrElse placeholderId <> Me.PlaceholderId OrElse whenNotNull <> Me.WhenNotNull OrElse whenNullOpt <> Me.WhenNullOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLoweredConditionalAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess(MyBase.Syntax, receiverOrCondition, captureReceiver, placeholderId, whenNotNull, whenNullOpt, type, MyBase.HasErrors)
				boundLoweredConditionalAccess1.CopyAttributes(Me)
				boundLoweredConditionalAccess = boundLoweredConditionalAccess1
			Else
				boundLoweredConditionalAccess = Me
			End If
			Return boundLoweredConditionalAccess
		End Function
	End Class
End Namespace