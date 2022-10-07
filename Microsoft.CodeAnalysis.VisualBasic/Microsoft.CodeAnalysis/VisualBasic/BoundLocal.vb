Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLocal
		Inherits BoundExpression
		Private ReadOnly _LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol

		Private ReadOnly _IsLValue As Boolean

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
				If (MyBase.HasErrors OrElse MyBase.Type.IsErrorType()) Then
					constantValue = Nothing
				Else
					constantValue = Me.LocalSymbol.GetConstantValue(Nothing)
				End If
				Return constantValue
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.LocalSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Get
				Return Me._LocalSymbol
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyClass.New(syntax, localSymbol, Not localSymbol.IsReadOnly, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal type As TypeSymbol)
			MyClass.New(syntax, localSymbol, Not localSymbol.IsReadOnly, type)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Local, syntax, type, hasErrors)
			Me._LocalSymbol = localSymbol
			Me._IsLValue = isLValue
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.Local, syntax, type)
			Me._LocalSymbol = localSymbol
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLocal(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			boundLocal = If(Not Me._IsLValue, Me, Me.Update(Me._LocalSymbol, False, MyBase.Type))
			Return boundLocal
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			If (CObj(localSymbol) <> CObj(Me.LocalSymbol) OrElse isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(MyBase.Syntax, localSymbol, isLValue, type, MyBase.HasErrors)
				boundLocal1.CopyAttributes(Me)
				boundLocal = boundLocal1
			Else
				boundLocal = Me
			End If
			Return boundLocal
		End Function
	End Class
End Namespace