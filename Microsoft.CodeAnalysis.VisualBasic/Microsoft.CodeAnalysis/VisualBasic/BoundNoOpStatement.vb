Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNoOpStatement
		Inherits BoundStatement
		Private ReadOnly _Flavor As NoOpStatementFlavor

		Public ReadOnly Property Flavor As NoOpStatementFlavor
			Get
				Return Me._Flavor
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode)
			MyClass.New(syntax, NoOpStatementFlavor.[Default])
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyClass.New(syntax, NoOpStatementFlavor.[Default], hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal flavor As NoOpStatementFlavor, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.NoOpStatement, syntax, hasErrors)
			Me._Flavor = flavor
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal flavor As NoOpStatementFlavor)
			MyBase.New(BoundKind.NoOpStatement, syntax)
			Me._Flavor = flavor
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNoOpStatement(Me)
		End Function

		Public Function Update(ByVal flavor As NoOpStatementFlavor) As Microsoft.CodeAnalysis.VisualBasic.BoundNoOpStatement
			Dim boundNoOpStatement As Microsoft.CodeAnalysis.VisualBasic.BoundNoOpStatement
			If (flavor = Me.Flavor) Then
				boundNoOpStatement = Me
			Else
				Dim boundNoOpStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundNoOpStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundNoOpStatement(MyBase.Syntax, flavor, MyBase.HasErrors)
				boundNoOpStatement1.CopyAttributes(Me)
				boundNoOpStatement = boundNoOpStatement1
			End If
			Return boundNoOpStatement
		End Function
	End Class
End Namespace