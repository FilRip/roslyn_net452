Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class GroupByClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _groupKeyword As KeywordSyntax

		Friend ReadOnly _items As GreenNode

		Friend ReadOnly _byKeyword As KeywordSyntax

		Friend ReadOnly _keys As GreenNode

		Friend ReadOnly _intoKeyword As KeywordSyntax

		Friend ReadOnly _aggregationVariables As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(Me._aggregationVariables))
			End Get
		End Property

		Friend ReadOnly Property ByKeyword As KeywordSyntax
			Get
				Return Me._byKeyword
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

		Friend ReadOnly Property Items As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(Me._items))
			End Get
		End Property

		Friend ReadOnly Property Keys As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(Me._keys))
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal groupKeyword As KeywordSyntax, ByVal items As GreenNode, ByVal byKeyword As KeywordSyntax, ByVal keys As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			If (items IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(items)
				Me._items = items
			End If
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (keys IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keys)
				Me._keys = keys
			End If
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal groupKeyword As KeywordSyntax, ByVal items As GreenNode, ByVal byKeyword As KeywordSyntax, ByVal keys As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 6
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			If (items IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(items)
				Me._items = items
			End If
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (keys IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keys)
				Me._keys = keys
			End If
			MyBase.AdjustFlagsAndWidth(intoKeyword)
			Me._intoKeyword = intoKeyword
			If (aggregationVariables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationVariables)
				Me._aggregationVariables = aggregationVariables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal groupKeyword As KeywordSyntax, ByVal items As GreenNode, ByVal byKeyword As KeywordSyntax, ByVal keys As GreenNode, ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(groupKeyword)
			Me._groupKeyword = groupKeyword
			If (items IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(items)
				Me._items = items
			End If
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (keys IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keys)
				Me._keys = keys
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
			MyBase._slotCount = 6
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._groupKeyword = keywordSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._items = greenNode
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._byKeyword = keywordSyntax1
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._keys = greenNode1
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._intoKeyword = keywordSyntax2
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
				Me._aggregationVariables = greenNode2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitGroupByClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._groupKeyword
					Exit Select
				Case 1
					greenNode = Me._items
					Exit Select
				Case 2
					greenNode = Me._byKeyword
					Exit Select
				Case 3
					greenNode = Me._keys
					Exit Select
				Case 4
					greenNode = Me._intoKeyword
					Exit Select
				Case 5
					greenNode = Me._aggregationVariables
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._groupKeyword, Me._items, Me._byKeyword, Me._keys, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._groupKeyword, Me._items, Me._byKeyword, Me._keys, Me._intoKeyword, Me._aggregationVariables)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._groupKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._items, IObjectWritable))
			writer.WriteValue(DirectCast(Me._byKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._keys, IObjectWritable))
			writer.WriteValue(DirectCast(Me._intoKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aggregationVariables, IObjectWritable))
		End Sub
	End Class
End Namespace