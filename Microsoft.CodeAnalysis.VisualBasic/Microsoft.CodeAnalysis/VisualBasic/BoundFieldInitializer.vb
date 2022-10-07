Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundFieldInitializer
		Inherits BoundFieldOrPropertyInitializer
		Private ReadOnly _InitializedFields As ImmutableArray(Of FieldSymbol)

		Public ReadOnly Property InitializedFields As ImmutableArray(Of FieldSymbol)
			Get
				Return Me._InitializedFields
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal initializedFields As ImmutableArray(Of FieldSymbol), ByVal memberAccessExpressionOpt As BoundExpression, ByVal initialValue As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.FieldInitializer, syntax, memberAccessExpressionOpt, initialValue, If(hasErrors OrElse memberAccessExpressionOpt.NonNullAndHasErrors(), True, initialValue.NonNullAndHasErrors()))
			Me._InitializedFields = initializedFields
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitFieldInitializer(Me)
		End Function

		Public Function Update(ByVal initializedFields As ImmutableArray(Of FieldSymbol), ByVal memberAccessExpressionOpt As BoundExpression, ByVal initialValue As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer
			Dim boundFieldInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer
			If (initializedFields <> Me.InitializedFields OrElse memberAccessExpressionOpt <> MyBase.MemberAccessExpressionOpt OrElse initialValue <> MyBase.InitialValue) Then
				Dim boundFieldInitializer1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer(MyBase.Syntax, initializedFields, memberAccessExpressionOpt, initialValue, MyBase.HasErrors)
				boundFieldInitializer1.CopyAttributes(Me)
				boundFieldInitializer = boundFieldInitializer1
			Else
				boundFieldInitializer = Me
			End If
			Return boundFieldInitializer
		End Function
	End Class
End Namespace