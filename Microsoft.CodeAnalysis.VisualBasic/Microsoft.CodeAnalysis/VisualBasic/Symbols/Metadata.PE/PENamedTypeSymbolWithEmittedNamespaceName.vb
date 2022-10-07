Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PENamedTypeSymbolWithEmittedNamespaceName
		Inherits PENamedTypeSymbol
		Private ReadOnly _emittedNamespaceName As String

		Private ReadOnly _corTypeId As Microsoft.CodeAnalysis.SpecialType

		Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
			Get
				Return Me._corTypeId
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingNamespace As PENamespaceSymbol, ByVal typeDef As TypeDefinitionHandle, ByVal emittedNamespaceName As String)
			MyBase.New(moduleSymbol, containingNamespace, typeDef)
			Me._emittedNamespaceName = emittedNamespaceName
			If (Me.Arity <> 0 AndAlso Not Me.MangleName OrElse Not moduleSymbol.ContainingAssembly.KeepLookingForDeclaredSpecialTypes OrElse Me.DeclaredAccessibility <> Accessibility.[Public]) Then
				Me._corTypeId = Microsoft.CodeAnalysis.SpecialType.None
				Return
			End If
			Me._corTypeId = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(emittedNamespaceName, Me.MetadataName))
		End Sub

		Friend Overrides Function GetEmittedNamespaceName() As String
			Return Me._emittedNamespaceName
		End Function
	End Class
End Namespace