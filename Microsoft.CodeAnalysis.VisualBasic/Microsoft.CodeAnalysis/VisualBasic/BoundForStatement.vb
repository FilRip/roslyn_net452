Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundForStatement
		Inherits BoundLoopStatement
		Private ReadOnly _DeclaredOrInferredLocalOpt As LocalSymbol

		Private ReadOnly _ControlVariable As BoundExpression

		Private ReadOnly _Body As BoundStatement

		Private ReadOnly _NextVariablesOpt As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property Body As BoundStatement
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property ControlVariable As BoundExpression
			Get
				Return Me._ControlVariable
			End Get
		End Property

		Public ReadOnly Property DeclaredOrInferredLocalOpt As LocalSymbol
			Get
				Return Me._DeclaredOrInferredLocalOpt
			End Get
		End Property

		Public ReadOnly Property NextVariablesOpt As ImmutableArray(Of BoundExpression)
			Get
				Return Me._NextVariablesOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal declaredOrInferredLocalOpt As LocalSymbol, ByVal controlVariable As BoundExpression, ByVal body As BoundStatement, ByVal nextVariablesOpt As ImmutableArray(Of BoundExpression), ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, continueLabel, exitLabel, hasErrors)
			Me._DeclaredOrInferredLocalOpt = declaredOrInferredLocalOpt
			Me._ControlVariable = controlVariable
			Me._Body = body
			Me._NextVariablesOpt = nextVariablesOpt
		End Sub
	End Class
End Namespace