Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class GroupTypeInferenceLambda
		Inherits BoundExpression
		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Private ReadOnly _Parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _Compilation As VisualBasicCompilation

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me._Compilation
			End Get
		End Property

		Public ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._Parameters
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal compilation As VisualBasicCompilation, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.GroupTypeInferenceLambda, syntax, Nothing, hasErrors)
			Me._Binder = binder
			Me._Parameters = parameters
			Me._Compilation = compilation
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal compilation As VisualBasicCompilation)
			MyBase.New(BoundKind.GroupTypeInferenceLambda, syntax, Nothing)
			Me._Binder = binder
			Me._Parameters = parameters
			Me._Compilation = compilation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitGroupTypeInferenceLambda(Me)
		End Function

		Public Function InferLambdaReturnType(ByVal delegateParams As ImmutableArray(Of ParameterSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (delegateParams.Length = 2) Then
				Dim anonymousTypeManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager = Me.Compilation.AnonymousTypeManager
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = delegateParams(1).Type
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = MyBase.Syntax.QueryClauseKeywordOrRangeVariableIdentifier()
				Dim anonymousTypeFields As ImmutableArray(Of AnonymousTypeField) = ImmutableArray.Create(Of AnonymousTypeField)(New AnonymousTypeField("$VB$ItAnonymous", type, syntaxToken.GetLocation(), True))
				syntaxToken = MyBase.Syntax.QueryClauseKeywordOrRangeVariableIdentifier()
				typeSymbol = anonymousTypeManager.ConstructAnonymousTypeSymbol(New AnonymousTypeDescriptor(anonymousTypeFields, syntaxToken.GetLocation(), True))
			Else
				typeSymbol = Nothing
			End If
			Return typeSymbol
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.GroupTypeInferenceLambda
			Dim groupTypeInferenceLambda As Microsoft.CodeAnalysis.VisualBasic.GroupTypeInferenceLambda
			If (binder <> Me.Binder OrElse parameters <> Me.Parameters OrElse compilation <> Me.Compilation) Then
				Dim groupTypeInferenceLambda1 As Microsoft.CodeAnalysis.VisualBasic.GroupTypeInferenceLambda = New Microsoft.CodeAnalysis.VisualBasic.GroupTypeInferenceLambda(MyBase.Syntax, binder, parameters, compilation, MyBase.HasErrors)
				groupTypeInferenceLambda1.CopyAttributes(Me)
				groupTypeInferenceLambda = groupTypeInferenceLambda1
			Else
				groupTypeInferenceLambda = Me
			End If
			Return groupTypeInferenceLambda
		End Function
	End Class
End Namespace