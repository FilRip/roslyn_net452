Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ForStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax
		Friend ReadOnly _equalsToken As PunctuationSyntax

		Friend ReadOnly _fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _toKeyword As KeywordSyntax

		Friend ReadOnly _toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property EqualsToken As PunctuationSyntax
			Get
				Return Me._equalsToken
			End Get
		End Property

		Friend ReadOnly Property FromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._fromValue
			End Get
		End Property

		Friend ReadOnly Property StepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax
			Get
				Return Me._stepClause
			End Get
		End Property

		Friend ReadOnly Property ToKeyword As KeywordSyntax
			Get
				Return Me._toKeyword
			End Get
		End Property

		Friend ReadOnly Property ToValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._toValue
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal equalsToken As PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			MyBase.New(kind, forKeyword, controlVariable)
			MyBase._slotCount = 7
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(fromValue)
			Me._fromValue = fromValue
			MyBase.AdjustFlagsAndWidth(toKeyword)
			Me._toKeyword = toKeyword
			MyBase.AdjustFlagsAndWidth(toValue)
			Me._toValue = toValue
			If (stepClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stepClause)
				Me._stepClause = stepClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal equalsToken As PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, forKeyword, controlVariable)
			MyBase._slotCount = 7
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(fromValue)
			Me._fromValue = fromValue
			MyBase.AdjustFlagsAndWidth(toKeyword)
			Me._toKeyword = toKeyword
			MyBase.AdjustFlagsAndWidth(toValue)
			Me._toValue = toValue
			If (stepClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stepClause)
				Me._stepClause = stepClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal equalsToken As PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			MyBase.New(kind, errors, annotations, forKeyword, controlVariable)
			MyBase._slotCount = 7
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(fromValue)
			Me._fromValue = fromValue
			MyBase.AdjustFlagsAndWidth(toKeyword)
			Me._toKeyword = toKeyword
			MyBase.AdjustFlagsAndWidth(toValue)
			Me._toValue = toValue
			If (stepClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stepClause)
				Me._stepClause = stepClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 7
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._equalsToken = punctuationSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._fromValue = expressionSyntax
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._toKeyword = keywordSyntax
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax1)
				Me._toValue = expressionSyntax1
			End If
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			If (forStepClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(forStepClauseSyntax)
				Me._stepClause = forStepClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitForStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._forKeyword
					Exit Select
				Case 1
					greenNode = Me._controlVariable
					Exit Select
				Case 2
					greenNode = Me._equalsToken
					Exit Select
				Case 3
					greenNode = Me._fromValue
					Exit Select
				Case 4
					greenNode = Me._toKeyword
					Exit Select
				Case 5
					greenNode = Me._toValue
					Exit Select
				Case 6
					greenNode = Me._stepClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._forKeyword, Me._controlVariable, Me._equalsToken, Me._fromValue, Me._toKeyword, Me._toValue, Me._stepClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._forKeyword, Me._controlVariable, Me._equalsToken, Me._fromValue, Me._toKeyword, Me._toValue, Me._stepClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._equalsToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._fromValue, IObjectWritable))
			writer.WriteValue(DirectCast(Me._toKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._toValue, IObjectWritable))
			writer.WriteValue(DirectCast(Me._stepClause, IObjectWritable))
		End Sub
	End Class
End Namespace