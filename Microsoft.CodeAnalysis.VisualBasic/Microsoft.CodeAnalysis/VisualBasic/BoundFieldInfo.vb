Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundFieldInfo
		Inherits BoundExpression
		Private ReadOnly _Field As FieldSymbol

		Public ReadOnly Property Field As FieldSymbol
			Get
				Return Me._Field
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal field As FieldSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.FieldInfo, syntax, type, hasErrors)
			Me._Field = field
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal field As FieldSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.FieldInfo, syntax, type)
			Me._Field = field
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitFieldInfo(Me)
		End Function

		Public Function Update(ByVal field As FieldSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo
			Dim boundFieldInfo As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo
			If (CObj(field) <> CObj(Me.Field) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundFieldInfo1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo(MyBase.Syntax, field, type, MyBase.HasErrors)
				boundFieldInfo1.CopyAttributes(Me)
				boundFieldInfo = boundFieldInfo1
			Else
				boundFieldInfo = Me
			End If
			Return boundFieldInfo
		End Function
	End Class
End Namespace