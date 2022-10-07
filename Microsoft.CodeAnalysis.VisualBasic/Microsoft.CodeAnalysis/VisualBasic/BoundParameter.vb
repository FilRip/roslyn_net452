Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundParameter
		Inherits BoundExpression
		Private ReadOnly _ParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol

		Private ReadOnly _IsLValue As Boolean

		Private ReadOnly _SuppressVirtualCalls As Boolean

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.ParameterSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property ParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Get
				Return Me._ParameterSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return Me._SuppressVirtualCalls
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyClass.New(syntax, parameterSymbol, isLValue, False, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol)
			MyClass.New(syntax, parameterSymbol, isLValue, False, type)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyClass.New(syntax, parameterSymbol, True, False, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal type As TypeSymbol)
			MyClass.New(syntax, parameterSymbol, True, False, type)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal isLValue As Boolean, ByVal suppressVirtualCalls As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Parameter, syntax, type, hasErrors)
			Me._ParameterSymbol = parameterSymbol
			Me._IsLValue = isLValue
			Me._SuppressVirtualCalls = suppressVirtualCalls
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal isLValue As Boolean, ByVal suppressVirtualCalls As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.Parameter, syntax, type)
			Me._ParameterSymbol = parameterSymbol
			Me._IsLValue = isLValue
			Me._SuppressVirtualCalls = suppressVirtualCalls
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitParameter(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundParameter
			Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter
			boundParameter = If(Not Me._IsLValue, Me, Me.Update(Me._ParameterSymbol, False, Me.SuppressVirtualCalls, MyBase.Type))
			Return boundParameter
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal isLValue As Boolean, ByVal suppressVirtualCalls As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundParameter
			Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter
			If (CObj(parameterSymbol) <> CObj(Me.ParameterSymbol) OrElse isLValue <> Me.IsLValue OrElse suppressVirtualCalls <> Me.SuppressVirtualCalls OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundParameter1 As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(MyBase.Syntax, parameterSymbol, isLValue, suppressVirtualCalls, type, MyBase.HasErrors)
				boundParameter1.CopyAttributes(Me)
				boundParameter = boundParameter1
			Else
				boundParameter = Me
			End If
			Return boundParameter
		End Function
	End Class
End Namespace