Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TypeArgumentListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _ofKeyword As KeywordSyntax

		Friend ReadOnly _arguments As GreenNode

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(Me._arguments))
			End Get
		End Property

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property OfKeyword As KeywordSyntax
			Get
				Return Me._ofKeyword
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(ofKeyword)
			Me._ofKeyword = ofKeyword
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(ofKeyword)
			Me._ofKeyword = ofKeyword
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal arguments As GreenNode, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(ofKeyword)
			Me._ofKeyword = ofKeyword
			If (arguments IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arguments)
				Me._arguments = arguments
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._ofKeyword = keywordSyntax
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
			Return visitor.VisitTypeArgumentList(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._openParenToken
					Exit Select
				Case 1
					greenNode = Me._ofKeyword
					Exit Select
				Case 2
					greenNode = Me._arguments
					Exit Select
				Case 3
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._openParenToken, Me._ofKeyword, Me._arguments, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._openParenToken, Me._ofKeyword, Me._arguments, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._ofKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._arguments, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace