Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAnonymousTypePropertyAccess
		Inherits BoundExpression
		Private ReadOnly _lazyPropertySymbol As Lazy(Of PropertySymbol)

		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeCreationBinder

		Private ReadOnly _PropertyIndex As Integer

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeCreationBinder
			Get
				Return Me._Binder
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me._lazyPropertySymbol.Value
			End Get
		End Property

		Public ReadOnly Property PropertyIndex As Integer
			Get
				Return Me._PropertyIndex
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeCreationBinder, ByVal propertyIndex As Integer, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.AnonymousTypePropertyAccess, syntax, type, hasErrors)
			Me._lazyPropertySymbol = New Lazy(Of PropertySymbol)(New Func(Of PropertySymbol)(AddressOf Me.LazyGetProperty))
			Me._Binder = binder
			Me._PropertyIndex = propertyIndex
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeCreationBinder, ByVal propertyIndex As Integer, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.AnonymousTypePropertyAccess, syntax, type)
			Me._lazyPropertySymbol = New Lazy(Of PropertySymbol)(New Func(Of PropertySymbol)(AddressOf Me.LazyGetProperty))
			Me._Binder = binder
			Me._PropertyIndex = propertyIndex
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAnonymousTypePropertyAccess(Me)
		End Function

		Private Function LazyGetProperty() As PropertySymbol
			Return Me.Binder.GetAnonymousTypePropertySymbol(Me.PropertyIndex)
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeCreationBinder, ByVal propertyIndex As Integer, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypePropertyAccess
			Dim boundAnonymousTypePropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypePropertyAccess
			If (binder <> Me.Binder OrElse propertyIndex <> Me.PropertyIndex OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAnonymousTypePropertyAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypePropertyAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypePropertyAccess(MyBase.Syntax, binder, propertyIndex, type, MyBase.HasErrors)
				boundAnonymousTypePropertyAccess1.CopyAttributes(Me)
				boundAnonymousTypePropertyAccess = boundAnonymousTypePropertyAccess1
			Else
				boundAnonymousTypePropertyAccess = Me
			End If
			Return boundAnonymousTypePropertyAccess
		End Function
	End Class
End Namespace