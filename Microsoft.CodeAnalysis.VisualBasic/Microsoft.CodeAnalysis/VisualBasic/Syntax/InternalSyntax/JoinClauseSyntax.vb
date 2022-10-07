Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class JoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _joinKeyword As KeywordSyntax

		Friend ReadOnly _joinedVariables As GreenNode

		Friend ReadOnly _additionalJoins As GreenNode

		Friend ReadOnly _onKeyword As KeywordSyntax

		Friend ReadOnly _joinConditions As GreenNode

		Friend ReadOnly Property AdditionalJoins As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)(Me._additionalJoins)
			End Get
		End Property

		Friend ReadOnly Property JoinConditions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(Me._joinConditions))
			End Get
		End Property

		Friend ReadOnly Property JoinedVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(Me._joinedVariables))
			End Get
		End Property

		Friend ReadOnly Property JoinKeyword As KeywordSyntax
			Get
				Return Me._joinKeyword
			End Get
		End Property

		Friend ReadOnly Property OnKeyword As KeywordSyntax
			Get
				Return Me._onKeyword
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(joinKeyword)
			Me._joinKeyword = joinKeyword
			If (joinedVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinedVariables)
				Me._joinedVariables = joinedVariables
			End If
			If (additionalJoins IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalJoins)
				Me._additionalJoins = additionalJoins
			End If
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			If (joinConditions IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinConditions)
				Me._joinConditions = joinConditions
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(joinKeyword)
			Me._joinKeyword = joinKeyword
			If (joinedVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinedVariables)
				Me._joinedVariables = joinedVariables
			End If
			If (additionalJoins IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalJoins)
				Me._additionalJoins = additionalJoins
			End If
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			If (joinConditions IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinConditions)
				Me._joinConditions = joinConditions
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As GreenNode, ByVal additionalJoins As GreenNode, ByVal onKeyword As KeywordSyntax, ByVal joinConditions As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(joinKeyword)
			Me._joinKeyword = joinKeyword
			If (joinedVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinedVariables)
				Me._joinedVariables = joinedVariables
			End If
			If (additionalJoins IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalJoins)
				Me._additionalJoins = additionalJoins
			End If
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			If (joinConditions IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(joinConditions)
				Me._joinConditions = joinConditions
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._joinKeyword = keywordSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._joinedVariables = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._additionalJoins = greenNode1
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._onKeyword = keywordSyntax1
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
				Me._joinConditions = greenNode2
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._joinKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._joinedVariables, IObjectWritable))
			writer.WriteValue(DirectCast(Me._additionalJoins, IObjectWritable))
			writer.WriteValue(DirectCast(Me._onKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._joinConditions, IObjectWritable))
		End Sub
	End Class
End Namespace