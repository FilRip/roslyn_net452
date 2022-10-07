Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VBLocation
		Inherits Location
		Friend Overridable ReadOnly Property EmbeddedKind As EmbeddedSymbolKind
			Get
				Return EmbeddedSymbolKind.None
			End Get
		End Property

		Friend Overridable ReadOnly Property PossiblyEmbeddedOrMySourceSpan As TextSpan
			Get
				Return Me.SourceSpan
			End Get
		End Property

		Friend Overridable ReadOnly Property PossiblyEmbeddedOrMySourceTree As SyntaxTree
			Get
				Return DirectCast(Me.SourceTree, VisualBasicSyntaxTree)
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace