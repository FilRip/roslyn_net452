Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ElseDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _elseKeyword As KeywordSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ElseKeyword As KeywordSyntax
			Get
				Return Me._elseKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(elseKeyword)
			Me._elseKeyword = elseKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(elseKeyword)
			Me._elseKeyword = elseKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(elseKeyword)
			Me._elseKeyword = elseKeyword
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._elseKeyword = keywordSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitElseDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._hashToken
			ElseIf (num = 1) Then
				greenNode = Me._elseKeyword
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._elseKeyword)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._elseKeyword)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._elseKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace