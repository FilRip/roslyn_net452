Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMethodInfo
		Inherits BoundExpression
		Private ReadOnly _Method As MethodSymbol

		Public ReadOnly Property Method As MethodSymbol
			Get
				Return Me._Method
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MethodInfo, syntax, type, hasErrors)
			Me._Method = method
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MethodInfo, syntax, type)
			Me._Method = method
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMethodInfo(Me)
		End Function

		Public Function Update(ByVal method As MethodSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo
			Dim boundMethodInfo As Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo
			If (CObj(method) <> CObj(Me.Method) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundMethodInfo1 As Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo(MyBase.Syntax, method, type, MyBase.HasErrors)
				boundMethodInfo1.CopyAttributes(Me)
				boundMethodInfo = boundMethodInfo1
			Else
				boundMethodInfo = Me
			End If
			Return boundMethodInfo
		End Function
	End Class
End Namespace