Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class PartitionWhileClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _skipOrTakeKeyword As KeywordSyntax

		Friend ReadOnly _whileKeyword As KeywordSyntax

		Friend ReadOnly _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._condition
			End Get
		End Property

		Friend ReadOnly Property SkipOrTakeKeyword As KeywordSyntax
			Get
				Return Me._skipOrTakeKeyword
			End Get
		End Property

		Friend ReadOnly Property WhileKeyword As KeywordSyntax
			Get
				Return Me._whileKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(skipOrTakeKeyword)
			Me._skipOrTakeKeyword = skipOrTakeKeyword
			MyBase.AdjustFlagsAndWidth(whileKeyword)
			Me._whileKeyword = whileKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(skipOrTakeKeyword)
			Me._skipOrTakeKeyword = skipOrTakeKeyword
			MyBase.AdjustFlagsAndWidth(whileKeyword)
			Me._whileKeyword = whileKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(skipOrTakeKeyword)
			Me._skipOrTakeKeyword = skipOrTakeKeyword
			MyBase.AdjustFlagsAndWidth(whileKeyword)
			Me._whileKeyword = whileKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._skipOrTakeKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._whileKeyword = keywordSyntax1
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._condition = expressionSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitPartitionWhileClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._skipOrTakeKeyword
					Exit Select
				Case 1
					greenNode = Me._whileKeyword
					Exit Select
				Case 2
					greenNode = Me._condition
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._skipOrTakeKeyword, Me._whileKeyword, Me._condition)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._skipOrTakeKeyword, Me._whileKeyword, Me._condition)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._skipOrTakeKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._whileKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._condition, IObjectWritable))
		End Sub
	End Class
End Namespace