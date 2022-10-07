Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TupleExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _arguments As GreenNode

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(Me._arguments))
			End Get
		End Property

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openParenToken As PunctuationSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openParenToken As PunctuationSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As PunctuationSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._arguments = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._closeParenToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTupleExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._openParenToken
					Exit Select
				Case 1
					greenNode = Me._arguments
					Exit Select
				Case 2
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._openParenToken, Me._arguments, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._openParenToken, Me._arguments, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._arguments, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace