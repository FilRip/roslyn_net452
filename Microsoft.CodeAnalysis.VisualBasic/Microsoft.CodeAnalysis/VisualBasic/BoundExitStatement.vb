Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundExitStatement
		Inherits BoundStatement
		Private ReadOnly _Label As LabelSymbol

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ExitStatement, syntax, hasErrors)
			Me._Label = label
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol)
			MyBase.New(BoundKind.ExitStatement, syntax)
			Me._Label = label
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitExitStatement(Me)
		End Function

		Public Function Update(ByVal label As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExitStatement
			Dim boundExitStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExitStatement
			If (CObj(label) = CObj(Me.Label)) Then
				boundExitStatement = Me
			Else
				Dim boundExitStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExitStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExitStatement(MyBase.Syntax, label, MyBase.HasErrors)
				boundExitStatement1.CopyAttributes(Me)
				boundExitStatement = boundExitStatement1
			End If
			Return boundExitStatement
		End Function
	End Class
End Namespace