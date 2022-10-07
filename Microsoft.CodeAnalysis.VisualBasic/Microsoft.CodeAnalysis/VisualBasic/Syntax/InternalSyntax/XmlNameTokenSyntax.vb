Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlNameTokenSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
		Friend ReadOnly _possibleKeywordKind As SyntaxKind

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property PossibleKeywordKind As SyntaxKind
			Get
				Return Me._possibleKeywordKind
			End Get
		End Property

		Shared Sub New()
			XmlNameTokenSyntax.CreateInstance = Function(o As ObjectReader) New XmlNameTokenSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(XmlNameTokenSyntax), Function(r As ObjectReader) New XmlNameTokenSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal possibleKeywordKind As SyntaxKind)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			Me._possibleKeywordKind = possibleKeywordKind
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal possibleKeywordKind As SyntaxKind, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			MyBase.SetFactoryContext(context)
			Me._possibleKeywordKind = possibleKeywordKind
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal possibleKeywordKind As SyntaxKind)
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
			Me._possibleKeywordKind = possibleKeywordKind
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._possibleKeywordKind = CUShort(reader.ReadInt32())
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New XmlNameTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._possibleKeywordKind)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New XmlNameTokenSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._possibleKeywordKind)
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New XmlNameTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia(), Me._possibleKeywordKind)
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New XmlNameTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia, Me._possibleKeywordKind)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteInt32(CInt(Me._possibleKeywordKind))
		End Sub
	End Class
End Namespace