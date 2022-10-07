Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundSingleValueCaseClause
		Inherits BoundCaseClause
		Private ReadOnly _ValueOpt As BoundExpression

		Private ReadOnly _ConditionOpt As BoundExpression

		Public ReadOnly Property ConditionOpt As BoundExpression
			Get
				Return Me._ConditionOpt
			End Get
		End Property

		Public ReadOnly Property ValueOpt As BoundExpression
			Get
				Return Me._ValueOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, hasErrors)
			Me._ValueOpt = valueOpt
			Me._ConditionOpt = conditionOpt
		End Sub
	End Class
End Namespace