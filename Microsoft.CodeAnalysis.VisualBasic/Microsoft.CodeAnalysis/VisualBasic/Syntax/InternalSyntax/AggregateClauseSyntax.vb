Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class AggregateClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _aggregateKeyword As KeywordSyntax

		Friend ReadOnly _variables As GreenNode

		Friend ReadOnly _additionalQueryOperators As GreenNode

		Friend ReadOnly _intoKeyword As KeywordSyntax

		Friend ReadOnly _aggregationVariables As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AdditionalQueryOperators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)(Me._additionalQueryOperators)
			End Get
		End Property

		Friend ReadOnly Property AggregateKeyword As KeywordSyntax
			Get
				Return Me._aggregateKeyword
			End Get
		End Property

		Friend ReadOnly Property AggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(Me._aggregationVariables))
			End Get
		End Property

		Friend ReadOnly Property IntoKeyword As KeywordSyntax
			Get
				Return Me._intoKeyword
			End Get
		End Property

		Friend ReadOnly Property Variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(Me._variables))
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal aggregateKeyword As KeywordSyntax, ByVal variables As GreenNode, ByVal additionalQueryOperators As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(aggregateKeyword)
			Me._aggregateKeyword = aggregateKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
			If (additionalQueryOperators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalQueryOperators)
				Me._additionalQueryOperators = additionalQueryOperators
			End If
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal aggregateKeyword As KeywordSyntax, ByVal variables As GreenNode, ByVal additionalQueryOperators As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(aggregateKeyword)
			Me._aggregateKeyword = aggregateKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
			If (additionalQueryOperators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalQueryOperators)
				Me._additionalQueryOperators = additionalQueryOperators
			End If
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal aggregateKeyword As KeywordSyntax, ByVal variables As GreenNode, ByVal additionalQueryOperators As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(aggregateKeyword)
			Me._aggregateKeyword = aggregateKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
			If (additionalQueryOperators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(additionalQueryOperators)
				Me._additionalQueryOperators = additionalQueryOperators
			End If
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._aggregateKeyword = keywordSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._variables = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._additionalQueryOperators = greenNode1
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._intoKeyword = keywordSyntax1
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
				Me._aggregationVariables = greenNode2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitAggregateClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._aggregateKeyword
					Exit Select
				Case 1
					greenNode = Me._variables
					Exit Select
				Case 2
					greenNode = Me._additionalQueryOperators
					Exit Select
				Case 3
					greenNode = Me._intoKeyword
					Exit Select
				Case 4
					greenNode = Me._aggregationVariables
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._aggregateKeyword, Me._variables, Me._additionalQueryOperators, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._aggregateKeyword, Me._variables, Me._additionalQueryOperators, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._aggregateKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._variables, IObjectWritable))
			writer.WriteValue(DirectCast(Me._additionalQueryOperators, IObjectWritable))
			writer.WriteValue(DirectCast(Me._intoKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aggregationVariables, IObjectWritable))
		End Sub
	End Class
End Namespace