Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundContinueStatement
		Inherits BoundStatement
		Private ReadOnly _Label As LabelSymbol

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ContinueStatement, syntax, hasErrors)
			Me._Label = label
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol)
			MyBase.New(BoundKind.ContinueStatement, syntax)
			Me._Label = label
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitContinueStatement(Me)
		End Function

		Public Function Update(ByVal label As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundContinueStatement
			Dim boundContinueStatement As Microsoft.CodeAnalysis.VisualBasic.BoundContinueStatement
			If (CObj(label) = CObj(Me.Label)) Then
				boundContinueStatement = Me
			Else
				Dim boundContinueStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundContinueStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundContinueStatement(MyBase.Syntax, label, MyBase.HasErrors)
				boundContinueStatement1.CopyAttributes(Me)
				boundContinueStatement = boundContinueStatement1
			End If
			Return boundContinueStatement
		End Function
	End Class
End Namespace