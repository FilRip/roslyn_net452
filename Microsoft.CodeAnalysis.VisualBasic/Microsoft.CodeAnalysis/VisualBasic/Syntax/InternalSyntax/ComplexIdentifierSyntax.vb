Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ComplexIdentifierSyntax
		Inherits IdentifierTokenSyntax
		Private ReadOnly _possibleKeywordKind As SyntaxKind

		Private ReadOnly _isBracketed As Boolean

		Private ReadOnly _identifierText As String

		Private ReadOnly _typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter

		Friend Overrides ReadOnly Property IdentifierText As String
			Get
				Return Me._identifierText
			End Get
		End Property

		Friend Overrides ReadOnly Property IsBracketed As Boolean
			Get
				Return Me._isBracketed
			End Get
		End Property

		Friend Overrides ReadOnly Property PossibleKeywordKind As SyntaxKind
			Get
				Return Me._possibleKeywordKind
			End Get
		End Property

		Public Overrides ReadOnly Property RawContextualKind As Integer
			Get
				Return CInt(Me._possibleKeywordKind)
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Get
				Return Me._typeCharacter
			End Get
		End Property

		Shared Sub New()
			ObjectBinder.RegisterTypeReader(GetType(ComplexIdentifierSyntax), Function(r As ObjectReader) New ComplexIdentifierSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode, ByVal possibleKeywordKind As SyntaxKind, ByVal isBracketed As Boolean, ByVal identifierText As String, ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter)
			MyBase.New(kind, errors, annotations, text, precedingTrivia, followingTrivia)
			Me._possibleKeywordKind = possibleKeywordKind
			Me._isBracketed = isBracketed
			Me._identifierText = identifierText
			Me._typeCharacter = typeCharacter
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._possibleKeywordKind = reader.ReadUInt16()
			Me._isBracketed = reader.ReadBoolean()
			Me._identifierText = reader.ReadString()
			Me._typeCharacter = reader.ReadByte()
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New ComplexIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me.PossibleKeywordKind, Me.IsBracketed, Me.IdentifierText, Me.TypeCharacter)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New ComplexIdentifierSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me.PossibleKeywordKind, Me.IsBracketed, Me.IdentifierText, Me.TypeCharacter)
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New ComplexIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia(), Me.PossibleKeywordKind, Me.IsBracketed, Me.IdentifierText, Me.TypeCharacter)
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New ComplexIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia, Me.PossibleKeywordKind, Me.IsBracketed, Me.IdentifierText, Me.TypeCharacter)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteUInt16(CUShort(Me._possibleKeywordKind))
			writer.WriteBoolean(Me._isBracketed)
			writer.WriteString(Me._identifierText)
			writer.WriteByte(CByte(Me._typeCharacter))
		End Sub
	End Class
End Namespace