Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class IfDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _elseKeyword As KeywordSyntax

		Friend ReadOnly _ifOrElseIfKeyword As KeywordSyntax

		Friend ReadOnly _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _thenKeyword As KeywordSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._condition
			End Get
		End Property

		Friend ReadOnly Property ElseKeyword As KeywordSyntax
			Get
				Return Me._elseKeyword
			End Get
		End Property

		Friend ReadOnly Property IfOrElseIfKeyword As KeywordSyntax
			Get
				Return Me._ifOrElseIfKeyword
			End Get
		End Property

		Friend ReadOnly Property ThenKeyword As KeywordSyntax
			Get
				Return Me._thenKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 5
			If (elseKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseKeyword)
				Me._elseKeyword = elseKeyword
			End If
			MyBase.AdjustFlagsAndWidth(ifOrElseIfKeyword)
			Me._ifOrElseIfKeyword = ifOrElseIfKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			If (thenKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(thenKeyword)
				Me._thenKeyword = thenKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			If (elseKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseKeyword)
				Me._elseKeyword = elseKeyword
			End If
			MyBase.AdjustFlagsAndWidth(ifOrElseIfKeyword)
			Me._ifOrElseIfKeyword = ifOrElseIfKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			If (thenKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(thenKeyword)
				Me._thenKeyword = thenKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 5
			If (elseKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseKeyword)
				Me._elseKeyword = elseKeyword
			End If
			MyBase.AdjustFlagsAndWidth(ifOrElseIfKeyword)
			Me._ifOrElseIfKeyword = ifOrElseIfKeyword
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			If (thenKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(thenKeyword)
				Me._thenKeyword = thenKeyword
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._elseKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._ifOrElseIfKeyword = keywordSyntax1
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._condition = expressionSyntax
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._thenKeyword = keywordSyntax2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitIfDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._hashToken
					Exit Select
				Case 1
					greenNode = Me._elseKeyword
					Exit Select
				Case 2
					greenNode = Me._ifOrElseIfKeyword
					Exit Select
				Case 3
					greenNode = Me._condition
					Exit Select
				Case 4
					greenNode = Me._thenKeyword
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._elseKeyword, Me._ifOrElseIfKeyword, Me._condition, Me._thenKeyword)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._elseKeyword, Me._ifOrElseIfKeyword, Me._condition, Me._thenKeyword)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._elseKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._ifOrElseIfKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._condition, IObjectWritable))
			writer.WriteValue(DirectCast(Me._thenKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace