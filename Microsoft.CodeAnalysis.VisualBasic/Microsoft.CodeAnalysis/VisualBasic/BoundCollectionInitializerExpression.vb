Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCollectionInitializerExpression
		Inherits BoundObjectInitializerExpressionBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal placeholderOpt As BoundWithLValueExpressionPlaceholder, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.CollectionInitializerExpression, syntax, placeholderOpt, initializers, type, If(hasErrors OrElse placeholderOpt.NonNullAndHasErrors(), True, initializers.NonNullAndHasErrors()))
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCollectionInitializerExpression(Me)
		End Function

		Public Function Update(ByVal placeholderOpt As BoundWithLValueExpressionPlaceholder, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundCollectionInitializerExpression
			Dim boundCollectionInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundCollectionInitializerExpression
			If (placeholderOpt <> MyBase.PlaceholderOpt OrElse initializers <> MyBase.Initializers OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundCollectionInitializerExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundCollectionInitializerExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCollectionInitializerExpression(MyBase.Syntax, placeholderOpt, initializers, type, MyBase.HasErrors)
				boundCollectionInitializerExpression1.CopyAttributes(Me)
				boundCollectionInitializerExpression = boundCollectionInitializerExpression1
			Else
				boundCollectionInitializerExpression = Me
			End If
			Return boundCollectionInitializerExpression
		End Function
	End Class
End Namespace