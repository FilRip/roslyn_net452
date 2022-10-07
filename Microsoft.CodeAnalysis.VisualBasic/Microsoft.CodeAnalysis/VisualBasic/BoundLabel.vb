Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLabel
		Inherits BoundExpression
		Private ReadOnly _Label As LabelSymbol

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.Label
			End Get
		End Property

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Label, syntax, type, hasErrors)
			Me._Label = label
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.Label, syntax, type)
			Me._Label = label
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLabel(Me)
		End Function

		Public Function Update(ByVal label As LabelSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLabel
			Dim boundLabel As Microsoft.CodeAnalysis.VisualBasic.BoundLabel
			If (CObj(label) <> CObj(Me.Label) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLabel1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabel = New Microsoft.CodeAnalysis.VisualBasic.BoundLabel(MyBase.Syntax, label, type, MyBase.HasErrors)
				boundLabel1.CopyAttributes(Me)
				boundLabel = boundLabel1
			Else
				boundLabel = Me
			End If
			Return boundLabel
		End Function
	End Class
End Namespace