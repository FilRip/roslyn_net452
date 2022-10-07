Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class GlobalNamespaceDeclaration
		Inherits SingleNamespaceDeclaration
		Public Overrides ReadOnly Property IsGlobalNamespace As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal hasImports As Boolean, ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal nameLocation As Microsoft.CodeAnalysis.Location, ByVal children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration))
			MyBase.New([String].Empty, hasImports, syntaxReference, nameLocation, children, False)
		End Sub
	End Class
End Namespace