Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class InterpolationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax
		Friend ReadOnly _openBraceToken As PunctuationSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax

		Friend ReadOnly _formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax

		Friend ReadOnly _closeBraceToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AlignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Get
				Return Me._alignmentClause
			End Get
		End Property

		Friend ReadOnly Property CloseBraceToken As PunctuationSyntax
			Get
				Return Me._closeBraceToken
			End Get
		End Property

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property FormatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Get
				Return Me._formatClause
			End Get
		End Property

		Friend ReadOnly Property OpenBraceToken As PunctuationSyntax
			Get
				Return Me._openBraceToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openBraceToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			If (alignmentClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(alignmentClause)
				Me._alignmentClause = alignmentClause
			End If
			If (formatClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(formatClause)
				Me._formatClause = formatClause
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openBraceToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			If (alignmentClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(alignmentClause)
				Me._alignmentClause = alignmentClause
			End If
			If (formatClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(formatClause)
				Me._formatClause = formatClause
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openBraceToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			If (alignmentClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(alignmentClause)
				Me._alignmentClause = alignmentClause
			End If
			If (formatClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(formatClause)
				Me._formatClause = formatClause
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openBraceToken = punctuationSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)
			If (interpolationAlignmentClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(interpolationAlignmentClauseSyntax)
				Me._alignmentClause = interpolationAlignmentClauseSyntax
			End If
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)
			If (interpolationFormatClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(interpolationFormatClauseSyntax)
				Me._formatClause = interpolationFormatClauseSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._closeBraceToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitInterpolation(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._openBraceToken
					Exit Select
				Case 1
					greenNode = Me._expression
					Exit Select
				Case 2
					greenNode = Me._alignmentClause
					Exit Select
				Case 3
					greenNode = Me._formatClause
					Exit Select
				Case 4
					greenNode = Me._closeBraceToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._openBraceToken, Me._expression, Me._alignmentClause, Me._formatClause, Me._closeBraceToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._openBraceToken, Me._expression, Me._alignmentClause, Me._formatClause, Me._closeBraceToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._openBraceToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._alignmentClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._formatClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeBraceToken, IObjectWritable))
		End Sub
	End Class
End Namespace