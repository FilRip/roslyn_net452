Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMethodDefIndex
		Inherits BoundExpression
		Private ReadOnly _Method As MethodSymbol

		Public ReadOnly Property Method As MethodSymbol
			Get
				Return Me._Method
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MethodDefIndex, syntax, type, hasErrors)
			Me._Method = method
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MethodDefIndex, syntax, type)
			Me._Method = method
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMethodDefIndex(Me)
		End Function

		Public Function Update(ByVal method As MethodSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex
			Dim boundMethodDefIndex As Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex
			If (CObj(method) <> CObj(Me.Method) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundMethodDefIndex1 As Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex(MyBase.Syntax, method, type, MyBase.HasErrors)
				boundMethodDefIndex1.CopyAttributes(Me)
				boundMethodDefIndex = boundMethodDefIndex1
			Else
				boundMethodDefIndex = Me
			End If
			Return boundMethodDefIndex
		End Function
	End Class
End Namespace