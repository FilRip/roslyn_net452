Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SimpleJoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode)
			MyBase.New(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 5
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode)
			MyBase.New(kind, errors, annotations, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 5
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSimpleJoinClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._joinKeyword
					Exit Select
				Case 1
					greenNode = Me._joinedVariables
					Exit Select
				Case 2
					greenNode = Me._additionalJoins
					Exit Select
				Case 3
					greenNode = Me._onKeyword
					Exit Select
				Case 4
					greenNode = Me._joinConditions
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._joinKeyword, Me._joinedVariables, Me._additionalJoins, Me._onKeyword, Me._joinConditions)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._joinKeyword, Me._joinedVariables, Me._additionalJoins, Me._onKeyword, Me._joinConditions)
		End Function
	End Class
End Namespace