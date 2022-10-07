Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAttribute
		Inherits BoundExpression
		Private ReadOnly _Constructor As MethodSymbol

		Private ReadOnly _ConstructorArguments As ImmutableArray(Of BoundExpression)

		Private ReadOnly _NamedArguments As ImmutableArray(Of BoundExpression)

		Private ReadOnly _ResultKind As LookupResultKind

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Dim constructorArguments As ImmutableArray(Of BoundExpression) = Me.ConstructorArguments
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(constructorArguments.AddRange(Me.NamedArguments))
			End Get
		End Property

		Public ReadOnly Property Constructor As MethodSymbol
			Get
				Return Me._Constructor
			End Get
		End Property

		Public ReadOnly Property ConstructorArguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ConstructorArguments
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.Constructor
			End Get
		End Property

		Public ReadOnly Property NamedArguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._NamedArguments
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me._ResultKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal constructor As MethodSymbol, ByVal constructorArguments As ImmutableArray(Of BoundExpression), ByVal namedArguments As ImmutableArray(Of BoundExpression), ByVal resultKind As LookupResultKind, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Attribute, syntax, type, If(hasErrors OrElse constructorArguments.NonNullAndHasErrors(), True, namedArguments.NonNullAndHasErrors()))
			Me._Constructor = constructor
			Me._ConstructorArguments = constructorArguments
			Me._NamedArguments = namedArguments
			Me._ResultKind = resultKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAttribute(Me)
		End Function

		Public Function Update(ByVal constructor As MethodSymbol, ByVal constructorArguments As ImmutableArray(Of BoundExpression), ByVal namedArguments As ImmutableArray(Of BoundExpression), ByVal resultKind As LookupResultKind, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute
			Dim boundAttribute As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute
			If (CObj(constructor) <> CObj(Me.Constructor) OrElse constructorArguments <> Me.ConstructorArguments OrElse namedArguments <> Me.NamedArguments OrElse resultKind <> Me.ResultKind OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAttribute1 As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute = New Microsoft.CodeAnalysis.VisualBasic.BoundAttribute(MyBase.Syntax, constructor, constructorArguments, namedArguments, resultKind, type, MyBase.HasErrors)
				boundAttribute1.CopyAttributes(Me)
				boundAttribute = boundAttribute1
			Else
				boundAttribute = Me
			End If
			Return boundAttribute
		End Function
	End Class
End Namespace