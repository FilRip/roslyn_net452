Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundFieldAccess
		Inherits BoundExpression
		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _FieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol

		Private ReadOnly _IsLValue As Boolean

		Private ReadOnly _SuppressVirtualCalls As Boolean

		Private ReadOnly _ConstantsInProgressOpt As ConstantFieldsInProgress

		Public ReadOnly Property ConstantsInProgressOpt As ConstantFieldsInProgress
			Get
				Return Me._ConstantsInProgressOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
				Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue
				If (Not Me._IsLValue) Then
					Dim constantsInProgressOpt As ConstantFieldsInProgress = Me.ConstantsInProgressOpt
					constantValue1 = If(constantsInProgressOpt Is Nothing, Me.FieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty), Me.FieldSymbol.GetConstantValue(constantsInProgressOpt))
					constantValue = constantValue1
				Else
					constantValue = Nothing
				End If
				Return constantValue
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.FieldSymbol
			End Get
		End Property

		Public ReadOnly Property FieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Get
				Return Me._FieldSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public Overrides ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return Me._SuppressVirtualCalls
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiverOpt As BoundExpression, ByVal fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, receiverOpt, fieldSymbol, isLValue, False, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiverOpt As BoundExpression, ByVal fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, ByVal isLValue As Boolean, ByVal suppressVirtualCalls As Boolean, ByVal constantsInProgressOpt As ConstantFieldsInProgress, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.FieldAccess, syntax, type, If(hasErrors, True, receiverOpt.NonNullAndHasErrors()))
			Me._ReceiverOpt = receiverOpt
			Me._FieldSymbol = fieldSymbol
			Me._IsLValue = isLValue
			Me._SuppressVirtualCalls = suppressVirtualCalls
			Me._ConstantsInProgressOpt = constantsInProgressOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitFieldAccess(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			boundFieldAccess = If(Not Me._IsLValue, Me, Me.Update(Me._ReceiverOpt, Me._FieldSymbol, False, Me.SuppressVirtualCalls, Me.ConstantsInProgressOpt, MyBase.Type))
			Return boundFieldAccess
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal receiverOpt As BoundExpression, ByVal fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, ByVal isLValue As Boolean, ByVal suppressVirtualCalls As Boolean, ByVal constantsInProgressOpt As ConstantFieldsInProgress, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			If (receiverOpt <> Me.ReceiverOpt OrElse CObj(fieldSymbol) <> CObj(Me.FieldSymbol) OrElse isLValue <> Me.IsLValue OrElse suppressVirtualCalls <> Me.SuppressVirtualCalls OrElse constantsInProgressOpt <> Me.ConstantsInProgressOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(MyBase.Syntax, receiverOpt, fieldSymbol, isLValue, suppressVirtualCalls, constantsInProgressOpt, type, MyBase.HasErrors)
				boundFieldAccess1.CopyAttributes(Me)
				boundFieldAccess = boundFieldAccess1
			Else
				boundFieldAccess = Me
			End If
			Return boundFieldAccess
		End Function
	End Class
End Namespace