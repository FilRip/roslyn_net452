Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class WrappedNamedTypeSymbol
		Inherits NamedTypeSymbol
		Protected _underlyingType As NamedTypeSymbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._underlyingType.Arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Me._underlyingType.CoClassType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingType.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingType.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Me._underlyingType.DefaultPropertyName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return Me._underlyingType.HasCodeAnalysisEmbeddedAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me._underlyingType.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingType.HasSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return Me._underlyingType.HasVisualBasicEmbeddedAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingType.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me._underlyingType.IsInterface
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataAbstract As Boolean
			Get
				Return Me._underlyingType.IsMetadataAbstract
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataSealed As Boolean
			Get
				Return Me._underlyingType.IsMetadataSealed
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return Me._underlyingType.IsMustInherit
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return Me._underlyingType.IsNotInheritable
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me._underlyingType.IsSerializable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return Me._underlyingType.IsWindowsRuntimeImport
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return Me._underlyingType.Layout
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingType.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me._underlyingType.MangleName
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return Me._underlyingType.MarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingType.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me._underlyingType.MightContainExtensionMethods
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingType.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingType.ObsoleteAttributeData
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return Me._underlyingType.ShouldAddWinRTMembers
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Me._underlyingType.TypeKind
			End Get
		End Property

		Public ReadOnly Property UnderlyingNamedType As NamedTypeSymbol
			Get
				Return Me._underlyingType
			End Get
		End Property

		Public Sub New(ByVal underlyingType As NamedTypeSymbol)
			MyBase.New()
			Me._underlyingType = underlyingType
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me._underlyingType.GetAppliedConditionalSymbols()
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return Me._underlyingType.GetAttributeUsageInfo()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingType.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetGuidString(<Out> ByRef guidString As String) As Boolean
			Return Me._underlyingType.GetGuidString(guidString)
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me._underlyingType.GetSecurityInformation()
		End Function
	End Class
End Namespace