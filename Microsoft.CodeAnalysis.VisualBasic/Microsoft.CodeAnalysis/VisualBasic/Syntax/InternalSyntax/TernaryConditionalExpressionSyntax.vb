Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TernaryConditionalExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _ifKeyword As KeywordSyntax

		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _firstCommaToken As PunctuationSyntax

		Friend ReadOnly _whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _secondCommaToken As PunctuationSyntax

		Friend ReadOnly _whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._condition
			End Get
		End Property

		Friend ReadOnly Property FirstCommaToken As PunctuationSyntax
			Get
				Return Me._firstCommaToken
			End Get
		End Property

		Friend ReadOnly Property IfKeyword As KeywordSyntax
			Get
				Return Me._ifKeyword
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Friend ReadOnly Property SecondCommaToken As PunctuationSyntax
			Get
				Return Me._secondCommaToken
			End Get
		End Property

		Friend ReadOnly Property WhenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._whenFalse
			End Get
		End Property

		Friend ReadOnly Property WhenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._whenTrue
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 8
			MyBase.AdjustFlagsAndWidth(ifKeyword)
			Me._ifKeyword = ifKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(whenTrue)
			Me._whenTrue = whenTrue
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(whenFalse)
			Me._whenFalse = whenFalse
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 8
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(ifKeyword)
			Me._ifKeyword = ifKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(whenTrue)
			Me._whenTrue = whenTrue
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(whenFalse)
			Me._whenFalse = whenFalse
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 8
			MyBase.AdjustFlagsAndWidth(ifKeyword)
			Me._ifKeyword = ifKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(condition)
			Me._condition = condition
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(whenTrue)
			Me._whenTrue = whenTrue
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(whenFalse)
			Me._whenFalse = whenFalse
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 8
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._ifKeyword = keywordSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._condition = expressionSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._firstCommaToken = punctuationSyntax1
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax1)
				Me._whenTrue = expressionSyntax1
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._secondCommaToken = punctuationSyntax2
			End If
			Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax2)
				Me._whenFalse = expressionSyntax2
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax3)
				Me._closeParenToken = punctuationSyntax3
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTernaryConditionalExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._ifKeyword
					Exit Select
				Case 1
					greenNode = Me._openParenToken
					Exit Select
				Case 2
					greenNode = Me._condition
					Exit Select
				Case 3
					greenNode = Me._firstCommaToken
					Exit Select
				Case 4
					greenNode = Me._whenTrue
					Exit Select
				Case 5
					greenNode = Me._secondCommaToken
					Exit Select
				Case 6
					greenNode = Me._whenFalse
					Exit Select
				Case 7
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._ifKeyword, Me._openParenToken, Me._condition, Me._firstCommaToken, Me._whenTrue, Me._secondCommaToken, Me._whenFalse, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._ifKeyword, Me._openParenToken, Me._condition, Me._firstCommaToken, Me._whenTrue, Me._secondCommaToken, Me._whenFalse, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._ifKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._condition, IObjectWritable))
			writer.WriteValue(DirectCast(Me._firstCommaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._whenTrue, IObjectWritable))
			writer.WriteValue(DirectCast(Me._secondCommaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._whenFalse, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace