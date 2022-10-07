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
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedMethod
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMethod
		Protected Overrides ReadOnly Property AcceptsExtraArguments As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsVararg
			End Get
		End Property

		Protected Overrides ReadOnly Property ContainingNamespace As INamespace
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.ContainingNamespace.GetCciAdapter()
			End Get
		End Property

		Protected Overrides ReadOnly Property IsAbstract As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsMustOverride
			End Get
		End Property

		Protected Overrides ReadOnly Property IsAccessCheckedOnOverride As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsAccessCheckedOnOverride
			End Get
		End Property

		Protected Overrides ReadOnly Property IsConstructor As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.MethodKind = MethodKind.Constructor
			End Get
		End Property

		Protected Overrides ReadOnly Property IsExternal As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsExternal
			End Get
		End Property

		Protected Overrides ReadOnly Property IsHiddenBySignature As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsHiddenBySignature
			End Get
		End Property

		Protected Overrides ReadOnly Property IsNewSlot As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataNewSlot(False)
			End Get
		End Property

		Protected Overrides ReadOnly Property IsRuntimeSpecial As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.HasRuntimeSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSealed As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataFinal
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSpecialName As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.HasSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsStatic As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsShared
			End Get
		End Property

		Protected Overrides ReadOnly Property IsVirtual As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataVirtual()
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.MetadataName
			End Get
		End Property

		Protected Overrides ReadOnly Property PlatformInvokeData As IPlatformInvokeInformation
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.GetDllImportData()
			End Get
		End Property

		Protected Overrides ReadOnly Property ReturnValueIsMarshalledExplicitly As Boolean
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueIsMarshalledExplicitly
			End Get
		End Property

		Protected Overrides ReadOnly Property ReturnValueMarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueMarshallingDescriptor
			End Get
		End Property

		Protected Overrides ReadOnly Property ReturnValueMarshallingInformation As IMarshallingInformation
			Get
				Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.ReturnTypeMarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeManager As EmbeddedTypesManager
			Get
				Return Me.ContainingType.TypeManager
			End Get
		End Property

		Protected Overrides ReadOnly Property UnderlyingMethodSignature As ISignature
			Get
				Return MyBase.UnderlyingMethod
			End Get
		End Property

		Protected Overrides ReadOnly Property Visibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(MyBase.UnderlyingMethod.AdaptedMethodSymbol)
			End Get
		End Property

		Public Sub New(ByVal containingType As EmbeddedType, ByVal underlyingMethod As MethodSymbol)
			MyBase.New(containingType, underlyingMethod)
		End Sub

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function

		Protected Overrides Function GetImplementationAttributes(ByVal context As EmitContext) As MethodImplAttributes
			Return MyBase.UnderlyingMethod.AdaptedMethodSymbol.ImplementationAttributes
		End Function

		Protected Overrides Function GetParameters() As ImmutableArray(Of EmbeddedParameter)
			Return EmbeddedTypesManager.EmbedParameters(Me, MyBase.UnderlyingMethod.AdaptedMethodSymbol.Parameters)
		End Function

		Protected Overrides Function GetTypeParameters() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedTypeParameter)
			Dim embeddedTypeParameter As Func(Of TypeParameterSymbol, EmbeddedMethod, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedTypeParameter)
			Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = MyBase.UnderlyingMethod.AdaptedMethodSymbol.TypeParameters
			If (EmbeddedMethod._Closure$__.$I5-0 Is Nothing) Then
				embeddedTypeParameter = Function(typeParameter As TypeParameterSymbol, container As EmbeddedMethod) New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedTypeParameter(container, typeParameter.GetCciAdapter())
				EmbeddedMethod._Closure$__.$I5-0 = embeddedTypeParameter
			Else
				embeddedTypeParameter = EmbeddedMethod._Closure$__.$I5-0
			End If
			Return ImmutableArrayExtensions.SelectAsArray(Of TypeParameterSymbol, EmbeddedMethod, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedTypeParameter)(typeParameters, embeddedTypeParameter, Me)
		End Function
	End Class
End Namespace