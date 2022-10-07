Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundObjectInitializerExpression
		Inherits BoundObjectInitializerExpressionBase
		Private ReadOnly _CreateTemporaryLocalForInitialization As Boolean

		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property CreateTemporaryLocalForInitialization As Boolean
			Get
				Return Me._CreateTemporaryLocalForInitialization
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal createTemporaryLocalForInitialization As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal placeholderOpt As BoundWithLValueExpressionPlaceholder, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ObjectInitializerExpression, syntax, placeholderOpt, initializers, type, If(hasErrors OrElse placeholderOpt.NonNullAndHasErrors(), True, initializers.NonNullAndHasErrors()))
			Me._CreateTemporaryLocalForInitialization = createTemporaryLocalForInitialization
			Me._Binder = binder
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitObjectInitializerExpression(Me)
		End Function

		Public Function Update(ByVal createTemporaryLocalForInitialization As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal placeholderOpt As BoundWithLValueExpressionPlaceholder, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression
			Dim boundObjectInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression
			If (createTemporaryLocalForInitialization <> Me.CreateTemporaryLocalForInitialization OrElse binder <> Me.Binder OrElse placeholderOpt <> MyBase.PlaceholderOpt OrElse initializers <> MyBase.Initializers OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundObjectInitializerExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression(MyBase.Syntax, createTemporaryLocalForInitialization, binder, placeholderOpt, initializers, type, MyBase.HasErrors)
				boundObjectInitializerExpression1.CopyAttributes(Me)
				boundObjectInitializerExpression = boundObjectInitializerExpression1
			Else
				boundObjectInitializerExpression = Me
			End If
			Return boundObjectInitializerExpression
		End Function
	End Class
End Namespace