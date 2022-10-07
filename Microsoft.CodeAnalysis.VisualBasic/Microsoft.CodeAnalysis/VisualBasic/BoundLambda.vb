Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLambda
		Inherits BoundExpression
		Private ReadOnly _LambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol

		Private ReadOnly _Body As BoundBlock

		Private ReadOnly _Diagnostics As ImmutableBindingDiagnostic(Of AssemblySymbol)

		Private ReadOnly _LambdaBinderOpt As LambdaBodyBinder

		Private ReadOnly _DelegateRelaxation As ConversionKind

		Private ReadOnly _MethodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property DelegateRelaxation As ConversionKind
			Get
				Return Me._DelegateRelaxation
			End Get
		End Property

		Public ReadOnly Property Diagnostics As ImmutableBindingDiagnostic(Of AssemblySymbol)
			Get
				Return Me._Diagnostics
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.LambdaSymbol
			End Get
		End Property

		Public ReadOnly Property IsSingleLine As Boolean
			Get
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = MyBase.Syntax.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
					Return True
				End If
				Return syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression
			End Get
		End Property

		Public ReadOnly Property LambdaBinderOpt As LambdaBodyBinder
			Get
				Return Me._LambdaBinderOpt
			End Get
		End Property

		Public ReadOnly Property LambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol
			Get
				Return Me._LambdaSymbol
			End Get
		End Property

		Public ReadOnly Property MethodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			Get
				Return Me._MethodConversionKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol, ByVal body As BoundBlock, ByVal diagnostics As ImmutableBindingDiagnostic(Of AssemblySymbol), ByVal lambdaBinderOpt As LambdaBodyBinder, ByVal delegateRelaxation As ConversionKind, ByVal methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Lambda, syntax, Nothing, If(hasErrors, True, body.NonNullAndHasErrors()))
			Me._LambdaSymbol = lambdaSymbol
			Me._Body = body
			Me._Diagnostics = diagnostics
			Me._LambdaBinderOpt = lambdaBinderOpt
			Me._DelegateRelaxation = delegateRelaxation
			Me._MethodConversionKind = methodConversionKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLambda(Me)
		End Function

		Public Function Update(ByVal lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol, ByVal body As BoundBlock, ByVal diagnostics As ImmutableBindingDiagnostic(Of AssemblySymbol), ByVal lambdaBinderOpt As LambdaBodyBinder, ByVal delegateRelaxation As ConversionKind, ByVal methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind) As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			If (CObj(lambdaSymbol) <> CObj(Me.LambdaSymbol) OrElse body <> Me.Body OrElse diagnostics <> Me.Diagnostics OrElse lambdaBinderOpt <> Me.LambdaBinderOpt OrElse delegateRelaxation <> Me.DelegateRelaxation OrElse methodConversionKind <> Me.MethodConversionKind) Then
				Dim boundLambda1 As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = New Microsoft.CodeAnalysis.VisualBasic.BoundLambda(MyBase.Syntax, lambdaSymbol, body, diagnostics, lambdaBinderOpt, delegateRelaxation, methodConversionKind, MyBase.HasErrors)
				boundLambda1.CopyAttributes(Me)
				boundLambda = boundLambda1
			Else
				boundLambda = Me
			End If
			Return boundLambda
		End Function
	End Class
End Namespace