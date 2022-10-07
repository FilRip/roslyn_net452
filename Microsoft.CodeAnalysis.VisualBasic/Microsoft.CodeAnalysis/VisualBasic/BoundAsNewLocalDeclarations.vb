Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAsNewLocalDeclarations
		Inherits BoundLocalDeclarationBase
		Private ReadOnly _LocalDeclarations As ImmutableArray(Of BoundLocalDeclaration)

		Private ReadOnly _Initializer As BoundExpression

		Public ReadOnly Property Initializer As BoundExpression
			Get
				Return Me._Initializer
			End Get
		End Property

		Public ReadOnly Property LocalDeclarations As ImmutableArray(Of BoundLocalDeclaration)
			Get
				Return Me._LocalDeclarations
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localDeclarations As ImmutableArray(Of BoundLocalDeclaration), ByVal initializer As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AsNewLocalDeclarations, syntax, If(hasErrors OrElse localDeclarations.NonNullAndHasErrors(), True, initializer.NonNullAndHasErrors()))
			Me._LocalDeclarations = localDeclarations
			Me._Initializer = initializer
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAsNewLocalDeclarations(Me)
		End Function

		Public Function Update(ByVal localDeclarations As ImmutableArray(Of BoundLocalDeclaration), ByVal initializer As BoundExpression) As BoundAsNewLocalDeclarations
			Dim boundAsNewLocalDeclaration As BoundAsNewLocalDeclarations
			If (localDeclarations <> Me.LocalDeclarations OrElse initializer <> Me.Initializer) Then
				Dim boundAsNewLocalDeclaration1 As BoundAsNewLocalDeclarations = New BoundAsNewLocalDeclarations(MyBase.Syntax, localDeclarations, initializer, MyBase.HasErrors)
				boundAsNewLocalDeclaration1.CopyAttributes(Me)
				boundAsNewLocalDeclaration = boundAsNewLocalDeclaration1
			Else
				boundAsNewLocalDeclaration = Me
			End If
			Return boundAsNewLocalDeclaration
		End Function
	End Class
End Namespace