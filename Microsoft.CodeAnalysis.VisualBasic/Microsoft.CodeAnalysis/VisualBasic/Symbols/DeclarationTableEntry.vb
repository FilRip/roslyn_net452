Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class DeclarationTableEntry
		Public ReadOnly Root As Lazy(Of RootSingleNamespaceDeclaration)

		Public ReadOnly IsEmbedded As Boolean

		Public Sub New(ByVal root As Lazy(Of RootSingleNamespaceDeclaration), ByVal isEmbedded As Boolean)
			MyBase.New()
			Me.Root = root
			Me.IsEmbedded = isEmbedded
		End Sub
	End Class
End Namespace