Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundPropertyInitializer
		Inherits BoundFieldOrPropertyInitializer
		Private ReadOnly _InitializedProperties As ImmutableArray(Of PropertySymbol)

		Public ReadOnly Property InitializedProperties As ImmutableArray(Of PropertySymbol)
			Get
				Return Me._InitializedProperties
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal initializedProperties As ImmutableArray(Of PropertySymbol), ByVal memberAccessExpressionOpt As BoundExpression, ByVal initialValue As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.PropertyInitializer, syntax, memberAccessExpressionOpt, initialValue, If(hasErrors OrElse memberAccessExpressionOpt.NonNullAndHasErrors(), True, initialValue.NonNullAndHasErrors()))
			Me._InitializedProperties = initializedProperties
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitPropertyInitializer(Me)
		End Function

		Public Function Update(ByVal initializedProperties As ImmutableArray(Of PropertySymbol), ByVal memberAccessExpressionOpt As BoundExpression, ByVal initialValue As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyInitializer
			Dim boundPropertyInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyInitializer
			If (initializedProperties <> Me.InitializedProperties OrElse memberAccessExpressionOpt <> MyBase.MemberAccessExpressionOpt OrElse initialValue <> MyBase.InitialValue) Then
				Dim boundPropertyInitializer1 As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyInitializer = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyInitializer(MyBase.Syntax, initializedProperties, memberAccessExpressionOpt, initialValue, MyBase.HasErrors)
				boundPropertyInitializer1.CopyAttributes(Me)
				boundPropertyInitializer = boundPropertyInitializer1
			Else
				boundPropertyInitializer = Me
			End If
			Return boundPropertyInitializer
		End Function
	End Class
End Namespace