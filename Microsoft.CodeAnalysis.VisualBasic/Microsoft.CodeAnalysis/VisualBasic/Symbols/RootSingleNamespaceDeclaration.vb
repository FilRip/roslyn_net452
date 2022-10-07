Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class RootSingleNamespaceDeclaration
		Inherits GlobalNamespaceDeclaration
		Private _referenceDirectives As ImmutableArray(Of ReferenceDirective)

		Private ReadOnly _hasAssemblyAttributes As Boolean

		Public ReadOnly Property HasAssemblyAttributes As Boolean
			Get
				Return Me._hasAssemblyAttributes
			End Get
		End Property

		Public ReadOnly Property ReferenceDirectives As ImmutableArray(Of ReferenceDirective)
			Get
				Return Me._referenceDirectives
			End Get
		End Property

		Public Sub New(ByVal hasImports As Boolean, ByVal treeNode As Microsoft.CodeAnalysis.SyntaxReference, ByVal children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration), ByVal referenceDirectives As ImmutableArray(Of ReferenceDirective), ByVal hasAssemblyAttributes As Boolean)
			MyBase.New(hasImports, treeNode, treeNode.GetLocation(), children)
			Me._referenceDirectives = referenceDirectives
			Me._hasAssemblyAttributes = hasAssemblyAttributes
		End Sub
	End Class
End Namespace