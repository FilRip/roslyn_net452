Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedParameter
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedParameter
		Protected Overrides ReadOnly Property HasDefaultValue As Boolean
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.HasMetadataConstantValue
			End Get
		End Property

		Protected Overrides ReadOnly Property Index As UShort
			Get
				Return CUShort(Me.UnderlyingParameter.AdaptedParameterSymbol.Ordinal)
			End Get
		End Property

		Protected Overrides ReadOnly Property IsIn As Boolean
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.IsMetadataIn
			End Get
		End Property

		Protected Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.IsMarshalledExplicitly
			End Get
		End Property

		Protected Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.IsMetadataOptional
			End Get
		End Property

		Protected Overrides ReadOnly Property IsOut As Boolean
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.IsMetadataOut
			End Get
		End Property

		Protected Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.MarshallingDescriptor
			End Get
		End Property

		Protected Overrides ReadOnly Property MarshallingInformation As IMarshallingInformation
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.MarshallingInformation
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return Me.UnderlyingParameter.AdaptedParameterSymbol.MetadataName
			End Get
		End Property

		Protected Overrides ReadOnly Property UnderlyingParameterTypeInformation As IParameterTypeInformation
			Get
				Return Me.UnderlyingParameter
			End Get
		End Property

		Public Sub New(ByVal containingPropertyOrMethod As EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMember, ByVal underlyingParameter As ParameterSymbol)
			MyBase.New(containingPropertyOrMethod, underlyingParameter)
		End Sub

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.UnderlyingParameter.AdaptedParameterSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function

		Protected Overrides Function GetDefaultValue(ByVal context As EmitContext) As MetadataConstant
			Return Me.UnderlyingParameter.GetMetadataConstantValue(context)
		End Function
	End Class
End Namespace