Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class InterpolatedStringExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _dollarSignDoubleQuoteToken As PunctuationSyntax

		Friend ReadOnly _contents As GreenNode

		Friend ReadOnly _doubleQuoteToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Contents As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)(Me._contents)
			End Get
		End Property

		Friend ReadOnly Property DollarSignDoubleQuoteToken As PunctuationSyntax
			Get
				Return Me._dollarSignDoubleQuoteToken
			End Get
		End Property

		Friend ReadOnly Property DoubleQuoteToken As PunctuationSyntax
			Get
				Return Me._doubleQuoteToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal dollarSignDoubleQuoteToken As PunctuationSyntax, ByVal contents As GreenNode, ByVal doubleQuoteToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(dollarSignDoubleQuoteToken)
			Me._dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken
			If (contents IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(contents)
				Me._contents = contents
			End If
			MyBase.AdjustFlagsAndWidth(doubleQuoteToken)
			Me._doubleQuoteToken = doubleQuoteToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal dollarSignDoubleQuoteToken As PunctuationSyntax, ByVal contents As GreenNode, ByVal doubleQuoteToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(dollarSignDoubleQuoteToken)
			Me._dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken
			If (contents IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(contents)
				Me._contents = contents
			End If
			MyBase.AdjustFlagsAndWidth(doubleQuoteToken)
			Me._doubleQuoteToken = doubleQuoteToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal dollarSignDoubleQuoteToken As PunctuationSyntax, ByVal contents As GreenNode, ByVal doubleQuoteToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(dollarSignDoubleQuoteToken)
			Me._dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken
			If (contents IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(contents)
				Me._contents = contents
			End If
			MyBase.AdjustFlagsAndWidth(doubleQuoteToken)
			Me._doubleQuoteToken = doubleQuoteToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._dollarSignDoubleQuoteToken = punctuationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._contents = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._doubleQuoteToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitInterpolatedStringExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._dollarSignDoubleQuoteToken
					Exit Select
				Case 1
					greenNode = Me._contents
					Exit Select
				Case 2
					greenNode = Me._doubleQuoteToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._dollarSignDoubleQuoteToken, Me._contents, Me._doubleQuoteToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._dollarSignDoubleQuoteToken, Me._contents, Me._doubleQuoteToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._dollarSignDoubleQuoteToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._contents, IObjectWritable))
			writer.WriteValue(DirectCast(Me._doubleQuoteToken, IObjectWritable))
		End Sub
	End Class
End Namespace