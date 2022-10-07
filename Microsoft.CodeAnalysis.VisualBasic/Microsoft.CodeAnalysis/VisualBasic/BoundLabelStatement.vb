Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLabelStatement
		Inherits BoundStatement
		Private ReadOnly _Label As LabelSymbol

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.LabelStatement, syntax, hasErrors)
			Me._Label = label
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol)
			MyBase.New(BoundKind.LabelStatement, syntax)
			Me._Label = label
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLabelStatement(Me)
		End Function

		Public Function Update(ByVal label As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement
			If (CObj(label) = CObj(Me.Label)) Then
				boundLabelStatement = Me
			Else
				Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(MyBase.Syntax, label, MyBase.HasErrors)
				boundLabelStatement1.CopyAttributes(Me)
				boundLabelStatement = boundLabelStatement1
			End If
			Return boundLabelStatement
		End Function
	End Class
End Namespace