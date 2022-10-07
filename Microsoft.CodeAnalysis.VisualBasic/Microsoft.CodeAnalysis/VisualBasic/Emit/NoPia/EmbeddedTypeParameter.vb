Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedTypeParameter
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedTypeParameter
		Protected Overrides ReadOnly Property Index As UShort
			Get
				Return CUShort(Me.UnderlyingTypeParameter.AdaptedTypeParameterSymbol.Ordinal)
			End Get
		End Property

		Protected Overrides ReadOnly Property MustBeReferenceType As Boolean
			Get
				Return Me.UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasReferenceTypeConstraint
			End Get
		End Property

		Protected Overrides ReadOnly Property MustBeValueType As Boolean
			Get
				Return Me.UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasValueTypeConstraint
			End Get
		End Property

		Protected Overrides ReadOnly Property MustHaveDefaultConstructor As Boolean
			Get
				Return Me.UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasConstructorConstraint
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return Me.UnderlyingTypeParameter.AdaptedTypeParameterSymbol.MetadataName
			End Get
		End Property

		Public Sub New(ByVal containingMethod As EmbeddedMethod, ByVal underlyingTypeParameter As TypeParameterSymbol)
			MyBase.New(containingMethod, underlyingTypeParameter)
		End Sub

		Protected Overrides Function GetConstraints(ByVal context As EmitContext) As IEnumerable(Of TypeReferenceWithAttributes)
			Return Me.UnderlyingTypeParameter.GetConstraints(context)
		End Function
	End Class
End Namespace