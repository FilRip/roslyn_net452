Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAnonymousTypeCreationExpression
		Inherits BoundExpression
		Private ReadOnly _BinderOpt As Binder.AnonymousTypeCreationBinder

		Private ReadOnly _Declarations As ImmutableArray(Of BoundAnonymousTypePropertyAccess)

		Private ReadOnly _Arguments As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property Arguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Arguments
			End Get
		End Property

		Public ReadOnly Property BinderOpt As Binder.AnonymousTypeCreationBinder
			Get
				Return Me._BinderOpt
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(Me.Arguments)
			End Get
		End Property

		Public ReadOnly Property Declarations As ImmutableArray(Of BoundAnonymousTypePropertyAccess)
			Get
				Return Me._Declarations
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Dim item As Symbol
				Dim type As TypeSymbol = MyBase.Type
				If (Not type.IsErrorType()) Then
					item = DirectCast(type, NamedTypeSymbol).InstanceConstructors(0)
				Else
					item = Nothing
				End If
				Return item
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binderOpt As Binder.AnonymousTypeCreationBinder, ByVal declarations As ImmutableArray(Of BoundAnonymousTypePropertyAccess), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AnonymousTypeCreationExpression, syntax, type, If(hasErrors OrElse declarations.NonNullAndHasErrors(), True, arguments.NonNullAndHasErrors()))
			Me._BinderOpt = binderOpt
			Me._Declarations = declarations
			Me._Arguments = arguments
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAnonymousTypeCreationExpression(Me)
		End Function

		Public Function Update(ByVal binderOpt As Binder.AnonymousTypeCreationBinder, ByVal declarations As ImmutableArray(Of BoundAnonymousTypePropertyAccess), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeCreationExpression
			Dim boundAnonymousTypeCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeCreationExpression
			If (binderOpt <> Me.BinderOpt OrElse declarations <> Me.Declarations OrElse arguments <> Me.Arguments OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAnonymousTypeCreationExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeCreationExpression(MyBase.Syntax, binderOpt, declarations, arguments, type, MyBase.HasErrors)
				boundAnonymousTypeCreationExpression1.CopyAttributes(Me)
				boundAnonymousTypeCreationExpression = boundAnonymousTypeCreationExpression1
			Else
				boundAnonymousTypeCreationExpression = Me
			End If
			Return boundAnonymousTypeCreationExpression
		End Function
	End Class
End Namespace