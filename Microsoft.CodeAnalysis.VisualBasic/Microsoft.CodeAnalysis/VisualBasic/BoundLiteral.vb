Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLiteral
		Inherits BoundExpression
		Private ReadOnly _Value As ConstantValue

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me.Value
			End Get
		End Property

		Public ReadOnly Property Value As ConstantValue
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal value As ConstantValue, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Literal, syntax, type, hasErrors)
			Me._Value = value
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal value As ConstantValue, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.Literal, syntax, type)
			Me._Value = value
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLiteral(Me)
		End Function

		Public Function Update(ByVal value As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			If (CObj(value) <> CObj(Me.Value) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(MyBase.Syntax, value, type, MyBase.HasErrors)
				boundLiteral1.CopyAttributes(Me)
				boundLiteral = boundLiteral1
			Else
				boundLiteral = Me
			End If
			Return boundLiteral
		End Function
	End Class
End Namespace