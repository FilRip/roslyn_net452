Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAnonymousTypeFieldInitializer
		Inherits BoundExpression
		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeFieldInitializerBinder

		Private ReadOnly _Value As BoundExpression

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeFieldInitializerBinder
			Get
				Return Me._Binder
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Value)
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeFieldInitializerBinder, ByVal value As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AnonymousTypeFieldInitializer, syntax, type, If(hasErrors, True, value.NonNullAndHasErrors()))
			Me._Binder = binder
			Me._Value = value
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAnonymousTypeFieldInitializer(Me)
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeFieldInitializerBinder, ByVal value As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeFieldInitializer
			Dim boundAnonymousTypeFieldInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeFieldInitializer
			If (binder <> Me.Binder OrElse value <> Me.Value OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAnonymousTypeFieldInitializer1 As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeFieldInitializer = New Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeFieldInitializer(MyBase.Syntax, binder, value, type, MyBase.HasErrors)
				boundAnonymousTypeFieldInitializer1.CopyAttributes(Me)
				boundAnonymousTypeFieldInitializer = boundAnonymousTypeFieldInitializer1
			Else
				boundAnonymousTypeFieldInitializer = Me
			End If
			Return boundAnonymousTypeFieldInitializer
		End Function
	End Class
End Namespace