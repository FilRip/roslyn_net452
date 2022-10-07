Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class RegionDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _regionKeyword As KeywordSyntax

		Friend ReadOnly _name As StringLiteralTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Name As StringLiteralTokenSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property RegionKeyword As KeywordSyntax
			Get
				Return Me._regionKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal regionKeyword As KeywordSyntax, ByVal name As StringLiteralTokenSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(regionKeyword)
			Me._regionKeyword = regionKeyword
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal regionKeyword As KeywordSyntax, ByVal name As StringLiteralTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(regionKeyword)
			Me._regionKeyword = regionKeyword
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal regionKeyword As KeywordSyntax, ByVal name As StringLiteralTokenSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(regionKeyword)
			Me._regionKeyword = regionKeyword
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._regionKeyword = keywordSyntax
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (stringLiteralTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stringLiteralTokenSyntax)
				Me._name = stringLiteralTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitRegionDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._hashToken
					Exit Select
				Case 1
					greenNode = Me._regionKeyword
					Exit Select
				Case 2
					greenNode = Me._name
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._regionKeyword, Me._name)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._regionKeyword, Me._name)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._regionKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
		End Sub
	End Class
End Namespace