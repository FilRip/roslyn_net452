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
	Friend NotInheritable Class EmbeddedField
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedField
		Protected Overrides ReadOnly Property IsCompileTimeConstant As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.IsMetadataConstant
			End Get
		End Property

		Protected Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.IsMarshalledExplicitly
			End Get
		End Property

		Protected Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.IsNotSerialized
			End Get
		End Property

		Protected Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.IsReadOnly
			End Get
		End Property

		Protected Overrides ReadOnly Property IsRuntimeSpecial As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.HasRuntimeSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSpecialName As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.HasSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsStatic As Boolean
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.IsShared
			End Get
		End Property

		Protected Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.MarshallingDescriptor
			End Get
		End Property

		Protected Overrides ReadOnly Property MarshallingInformation As IMarshallingInformation
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.MarshallingInformation
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.MetadataName
			End Get
		End Property

		Protected Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return MyBase.UnderlyingField.AdaptedFieldSymbol.TypeLayoutOffset
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeManager As EmbeddedTypesManager
			Get
				Return Me.ContainingType.TypeManager
			End Get
		End Property

		Protected Overrides ReadOnly Property Visibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(MyBase.UnderlyingField.AdaptedFieldSymbol)
			End Get
		End Property

		Public Sub New(ByVal containingType As EmbeddedType, ByVal underlyingField As FieldSymbol)
			MyBase.New(containingType, underlyingField)
		End Sub

		Protected Overrides Function GetCompileTimeValue(ByVal context As EmitContext) As MetadataConstant
			Return MyBase.UnderlyingField.GetMetadataConstantValue(context)
		End Function

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return MyBase.UnderlyingField.AdaptedFieldSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function
	End Class
End Namespace