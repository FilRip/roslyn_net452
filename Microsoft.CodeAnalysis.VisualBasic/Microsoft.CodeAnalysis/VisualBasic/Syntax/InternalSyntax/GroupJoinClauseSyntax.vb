Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class GroupJoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
		Friend ReadOnly _groupKeyword As KeywordSyntax

		Friend ReadOnly _intoKeyword As KeywordSyntax

		Friend ReadOnly _aggregationVariables As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(Me._aggregationVariables))
			End Get
		End Property

		Friend ReadOnly Property GroupKeyword As KeywordSyntax
			Get
				Return Me._groupKeyword
			End Get
		End Property

		Friend ReadOnly Property IntoKeyword As KeywordSyntax
			Get
				Return Me._intoKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal groupKeyword As KeywordSyntax, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 8
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal groupKeyword As KeywordSyntax, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 8
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal groupKeyword As KeywordSyntax, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind, errors, annotations, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
			MyBase._slotCount = 8
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 8
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._groupKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._intoKeyword = keywordSyntax1
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._aggregationVariables = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitGroupJoinClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._groupKeyword
					Exit Select
				Case 1
					greenNode = Me._joinKeyword
					Exit Select
				Case 2
					greenNode = Me._joinedVariables
					Exit Select
				Case 3
					greenNode = Me._additionalJoins
					Exit Select
				Case 4
					greenNode = Me._onKeyword
					Exit Select
				Case 5
					greenNode = Me._joinConditions
					Exit Select
				Case 6
					greenNode = Me._intoKeyword
					Exit Select
				Case 7
					greenNode = Me._aggregationVariables
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._groupKeyword, Me._joinKeyword, Me._joinedVariables, Me._additionalJoins, Me._onKeyword, Me._joinConditions, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._groupKeyword, Me._joinKeyword, Me._joinedVariables, Me._additionalJoins, Me._onKeyword, Me._joinConditions, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._groupKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._intoKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aggregationVariables, IObjectWritable))
		End Sub
	End Class
End Namespace