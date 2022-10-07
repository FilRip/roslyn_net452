Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundValueTypeMeReference
		Inherits BoundExpression
		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ValueTypeMeReference, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.ValueTypeMeReference, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitValueTypeMeReference(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundValueTypeMeReference
			Dim boundValueTypeMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundValueTypeMeReference
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundValueTypeMeReference = Me
			Else
				Dim boundValueTypeMeReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundValueTypeMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundValueTypeMeReference(MyBase.Syntax, type, MyBase.HasErrors)
				boundValueTypeMeReference1.CopyAttributes(Me)
				boundValueTypeMeReference = boundValueTypeMeReference1
			End If
			Return boundValueTypeMeReference
		End Function
	End Class
End Namespace