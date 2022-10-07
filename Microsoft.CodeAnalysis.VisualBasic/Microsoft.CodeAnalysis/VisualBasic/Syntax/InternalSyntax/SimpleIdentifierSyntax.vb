Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SimpleIdentifierSyntax
		Inherits IdentifierTokenSyntax
		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend Overrides ReadOnly Property IdentifierText As String
			Get
				Return MyBase.Text
			End Get
		End Property

		Friend Overrides ReadOnly Property IsBracketed As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property PossibleKeywordKind As SyntaxKind
			Get
				Return SyntaxKind.IdentifierToken
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None
			End Get
		End Property

		Shared Sub New()
			SimpleIdentifierSyntax.CreateInstance = Function(o As ObjectReader) New SimpleIdentifierSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(SimpleIdentifierSyntax), Function(r As ObjectReader) New SimpleIdentifierSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode)
			MyBase.New(kind, errors, annotations, text, precedingTrivia, followingTrivia)
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New SimpleIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New SimpleIdentifierSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New SimpleIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New SimpleIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia)
		End Function
	End Class
End Namespace