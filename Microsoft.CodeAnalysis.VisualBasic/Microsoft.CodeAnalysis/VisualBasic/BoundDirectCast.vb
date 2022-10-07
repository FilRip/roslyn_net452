Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundDirectCast
		Inherits BoundConversionOrCast
		Private ReadOnly _Operand As BoundExpression

		Private ReadOnly _ConversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind

		Private ReadOnly _SuppressVirtualCalls As Boolean

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Private ReadOnly _RelaxationLambdaOpt As BoundLambda

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ConversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Get
				Return Me._ConversionKind
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitCastInCode As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public ReadOnly Property RelaxationLambdaOpt As BoundLambda
			Get
				Return Me._RelaxationLambdaOpt
			End Get
		End Property

		Public Overrides ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return Me._SuppressVirtualCalls
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operand, conversionKind, False, Nothing, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal relaxationLambdaOpt As BoundLambda, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operand, conversionKind, False, Nothing, relaxationLambdaOpt, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operand, conversionKind, False, constantValueOpt, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal suppressVirtualCalls As Boolean, ByVal constantValueOpt As ConstantValue, ByVal relaxationLambdaOpt As BoundLambda, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.[DirectCast], syntax, type, If(hasErrors OrElse operand.NonNullAndHasErrors(), True, relaxationLambdaOpt.NonNullAndHasErrors()))
			Me._Operand = operand
			Me._ConversionKind = conversionKind
			Me._SuppressVirtualCalls = suppressVirtualCalls
			Me._ConstantValueOpt = constantValueOpt
			Me._RelaxationLambdaOpt = relaxationLambdaOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitDirectCast(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal suppressVirtualCalls As Boolean, ByVal constantValueOpt As ConstantValue, ByVal relaxationLambdaOpt As BoundLambda, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast
			Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast
			If (operand <> Me.Operand OrElse conversionKind <> Me.ConversionKind OrElse suppressVirtualCalls <> Me.SuppressVirtualCalls OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse relaxationLambdaOpt <> Me.RelaxationLambdaOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundDirectCast1 As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(MyBase.Syntax, operand, conversionKind, suppressVirtualCalls, constantValueOpt, relaxationLambdaOpt, type, MyBase.HasErrors)
				boundDirectCast1.CopyAttributes(Me)
				boundDirectCast = boundDirectCast1
			Else
				boundDirectCast = Me
			End If
			Return boundDirectCast
		End Function
	End Class
End Namespace