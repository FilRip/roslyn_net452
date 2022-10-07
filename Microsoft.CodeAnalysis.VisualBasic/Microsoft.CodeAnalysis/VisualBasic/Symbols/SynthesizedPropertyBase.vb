Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedPropertyBase
		Inherits PropertySymbol
		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Microsoft.Cci.CallingConvention.HasThis
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Return ImmutableArray(Of PropertySymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace