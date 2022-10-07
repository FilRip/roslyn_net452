Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundObjectCreationExpression
		Inherits BoundObjectCreationExpressionBase
		Private ReadOnly _ConstructorOpt As MethodSymbol

		Private ReadOnly _MethodGroupOpt As BoundMethodGroup

		Private ReadOnly _Arguments As ImmutableArray(Of BoundExpression)

		Private ReadOnly _DefaultArguments As BitVector

		Public ReadOnly Property Arguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Arguments
			End Get
		End Property

		Public ReadOnly Property ConstructorOpt As MethodSymbol
			Get
				Return Me._ConstructorOpt
			End Get
		End Property

		Public ReadOnly Property DefaultArguments As BitVector
			Get
				Return Me._DefaultArguments
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.ConstructorOpt
			End Get
		End Property

		Public ReadOnly Property MethodGroupOpt As BoundMethodGroup
			Get
				Return Me._MethodGroupOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal constructorOpt As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False, Optional ByVal defaultArguments As BitVector = Nothing)
			MyClass.New(syntax, constructorOpt, Nothing, arguments, defaultArguments, initializerOpt, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal constructorOpt As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ObjectCreationExpression, syntax, initializerOpt, type, If(hasErrors OrElse methodGroupOpt.NonNullAndHasErrors() OrElse arguments.NonNullAndHasErrors(), True, initializerOpt.NonNullAndHasErrors()))
			Me._ConstructorOpt = constructorOpt
			Me._MethodGroupOpt = methodGroupOpt
			Me._Arguments = arguments
			Me._DefaultArguments = defaultArguments
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitObjectCreationExpression(Me)
		End Function

		Public Function Update(ByVal constructorOpt As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol) As BoundObjectCreationExpression
			Return Me.Update(constructorOpt, Nothing, arguments, defaultArguments, initializerOpt, type)
		End Function

		Public Function Update(ByVal constructorOpt As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression
			Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression
			If (CObj(constructorOpt) <> CObj(Me.ConstructorOpt) OrElse methodGroupOpt <> Me.MethodGroupOpt OrElse arguments <> Me.Arguments OrElse defaultArguments <> Me.DefaultArguments OrElse initializerOpt <> MyBase.InitializerOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundObjectCreationExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(MyBase.Syntax, constructorOpt, methodGroupOpt, arguments, defaultArguments, initializerOpt, type, MyBase.HasErrors)
				boundObjectCreationExpression1.CopyAttributes(Me)
				boundObjectCreationExpression = boundObjectCreationExpression1
			Else
				boundObjectCreationExpression = Me
			End If
			Return boundObjectCreationExpression
		End Function
	End Class
End Namespace