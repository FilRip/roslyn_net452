Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ReferenceDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _referenceKeyword As KeywordSyntax

		Friend ReadOnly _file As StringLiteralTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property File As StringLiteralTokenSyntax
			Get
				Return Me._file
			End Get
		End Property

		Friend ReadOnly Property ReferenceKeyword As KeywordSyntax
			Get
				Return Me._referenceKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal referenceKeyword As KeywordSyntax, ByVal file As StringLiteralTokenSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(referenceKeyword)
			Me._referenceKeyword = referenceKeyword
			MyBase.AdjustFlagsAndWidth(file)
			Me._file = file
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal referenceKeyword As KeywordSyntax, ByVal file As StringLiteralTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(referenceKeyword)
			Me._referenceKeyword = referenceKeyword
			MyBase.AdjustFlagsAndWidth(file)
			Me._file = file
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal referenceKeyword As KeywordSyntax, ByVal file As StringLiteralTokenSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(referenceKeyword)
			Me._referenceKeyword = referenceKeyword
			MyBase.AdjustFlagsAndWidth(file)
			Me._file = file
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._referenceKeyword = keywordSyntax
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (stringLiteralTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stringLiteralTokenSyntax)
				Me._file = stringLiteralTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitReferenceDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._hashToken
					Exit Select
				Case 1
					greenNode = Me._referenceKeyword
					Exit Select
				Case 2
					greenNode = Me._file
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._referenceKeyword, Me._file)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._referenceKeyword, Me._file)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._referenceKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._file, IObjectWritable))
		End Sub
	End Class
End Namespace