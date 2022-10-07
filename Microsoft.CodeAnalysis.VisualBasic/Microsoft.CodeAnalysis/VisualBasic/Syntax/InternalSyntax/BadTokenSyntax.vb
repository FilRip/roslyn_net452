Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class BadTokenSyntax
		Inherits PunctuationSyntax
		Private ReadOnly _subKind As SyntaxSubKind

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property SubKind As SyntaxSubKind
			Get
				Return Me._subKind
			End Get
		End Property

		Shared Sub New()
			BadTokenSyntax.CreateInstance = Function(o As ObjectReader) New BadTokenSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(BadTokenSyntax), Function(r As ObjectReader) New BadTokenSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal subKind As SyntaxSubKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode)
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
			Me._subKind = subKind
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._subKind = reader.ReadUInt16()
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New BadTokenSyntax(MyBase.Kind, Me.SubKind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New BadTokenSyntax(MyBase.Kind, Me.SubKind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New BadTokenSyntax(MyBase.Kind, Me.SubKind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New BadTokenSyntax(MyBase.Kind, Me.SubKind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteUInt16(CUShort(Me._subKind))
		End Sub
	End Class
End Namespace