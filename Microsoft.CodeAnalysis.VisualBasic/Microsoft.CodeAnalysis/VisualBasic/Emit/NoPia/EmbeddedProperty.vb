Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedProperty
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedProperty
		Protected Overrides ReadOnly Property ContainingType As EmbeddedType
			Get
				Return MyBase.AnAccessor.ContainingType
			End Get
		End Property

		Protected Overrides ReadOnly Property IsRuntimeSpecial As Boolean
			Get
				Return MyBase.UnderlyingProperty.AdaptedPropertySymbol.HasRuntimeSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSpecialName As Boolean
			Get
				Return MyBase.UnderlyingProperty.AdaptedPropertySymbol.HasSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return MyBase.UnderlyingProperty.AdaptedPropertySymbol.MetadataName
			End Get
		End Property

		Protected Overrides ReadOnly Property UnderlyingPropertySignature As ISignature
			Get
				Return MyBase.UnderlyingProperty
			End Get
		End Property

		Protected Overrides ReadOnly Property Visibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(MyBase.UnderlyingProperty.AdaptedPropertySymbol)
			End Get
		End Property

		Public Sub New(ByVal underlyingProperty As PropertySymbol, ByVal getter As EmbeddedMethod, ByVal setter As EmbeddedMethod)
			MyBase.New(underlyingProperty, getter, setter)
		End Sub

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return MyBase.UnderlyingProperty.AdaptedPropertySymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function

		Protected Overrides Function GetParameters() As ImmutableArray(Of EmbeddedParameter)
			Return EmbeddedTypesManager.EmbedParameters(Me, MyBase.UnderlyingProperty.AdaptedPropertySymbol.Parameters)
		End Function
	End Class
End Namespace