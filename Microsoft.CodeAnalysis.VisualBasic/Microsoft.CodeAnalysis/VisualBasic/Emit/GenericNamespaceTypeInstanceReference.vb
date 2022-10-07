Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class GenericNamespaceTypeInstanceReference
		Inherits GenericTypeInstanceReference
		Public Overrides ReadOnly Property AsGenericTypeInstanceReference As IGenericTypeInstanceReference
			Get
				Return Me
			End Get
		End Property

		Public Overrides ReadOnly Property AsNamespaceTypeReference As INamespaceTypeReference
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property AsNestedTypeReference As INestedTypeReference
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property AsSpecializedNestedTypeReference As ISpecializedNestedTypeReference
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal underlyingNamedType As NamedTypeSymbol)
			MyBase.New(underlyingNamedType)
		End Sub
	End Class
End Namespace