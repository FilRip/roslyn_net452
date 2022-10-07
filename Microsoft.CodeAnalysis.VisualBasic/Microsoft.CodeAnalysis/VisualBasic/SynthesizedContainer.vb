Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class SynthesizedContainer
		Inherits InstanceTypeSymbol
		Private ReadOnly _containingType As NamedTypeSymbol

		Private ReadOnly _baseType As NamedTypeSymbol

		Private ReadOnly _name As String

		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private ReadOnly _interfaces As ImmutableArray(Of NamedTypeSymbol)

		Private ReadOnly _typeMap As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Private ReadOnly Shared s_typeSubstitutionFactory As Func(Of Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)

		Private ReadOnly Shared s_createTypeParameter As Func(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._typeParameters.Length
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Protected Friend MustOverride ReadOnly Property Constructor As MethodSymbol

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return New TypeLayout()
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me._typeParameters.Length > 0
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return MyBase.DefaultMarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return SpecializedCollections.SingletonEnumerable(Of String)(".ctor")
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Me._typeMap
			End Get
		End Property

		Shared Sub New()
			SynthesizedContainer.s_typeSubstitutionFactory = Function(container As Symbol) DirectCast(container, SynthesizedContainer).TypeSubstitution
			SynthesizedContainer.s_createTypeParameter = Function(typeParameter As TypeParameterSymbol, container As Symbol) New SynthesizedClonedTypeParameterSymbol(typeParameter, container, [String].Concat("SM$", typeParameter.Name), SynthesizedContainer.s_typeSubstitutionFactory)
		End Sub

		Protected Friend Sub New(ByVal topLevelMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal typeName As String, ByVal baseType As NamedTypeSymbol, ByVal originalInterfaces As ImmutableArray(Of NamedTypeSymbol))
			MyBase.New()
			Me._containingType = topLevelMethod.ContainingType
			Me._name = typeName
			Me._baseType = baseType
			If (Not topLevelMethod.IsGenericMethod) Then
				Me._typeMap = Nothing
				Me._typeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
				Me._interfaces = originalInterfaces
				Return
			End If
			Me._typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.OriginalDefinition.TypeParameters, Me, SynthesizedContainer.s_createTypeParameter)
			Dim item(Me._typeParameters.Length - 1 + 1 - 1) As TypeSymbol
			Dim length As Integer = Me._typeParameters.Length - 1
			Dim num As Integer = 0
			Do
				item(num) = Me._typeParameters(num)
				num = num + 1
			Loop While num <= length
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = topLevelMethod.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(item))
			Me._typeMap = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(methodSymbol.OriginalDefinition, methodSymbol.OriginalDefinition.TypeParameters, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(item), False)
			Me._interfaces = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, NamedTypeSymbol)(originalInterfaces, Function(i As NamedTypeSymbol) DirectCast(i.InternalSubstituteTypeParameters(Me._typeMap).AsTypeSymbolOnly(), NamedTypeSymbol))
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
		End Sub

		Friend NotOverridable Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend NotOverridable Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Friend NotOverridable Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return SpecializedCollections.EmptyEnumerable(Of FieldSymbol)()
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return ImmutableArray.Create(Of Symbol)(Me.Constructor)
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			If (Not CaseInsensitiveComparison.Equals(name, ".ctor")) Then
				Return ImmutableArray(Of Symbol).Empty
			End If
			Return ImmutableArray.Create(Of Symbol)(Me.Constructor)
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Public NotOverridable Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public NotOverridable Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public NotOverridable Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me._baseType
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return Me._interfaces
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me.MakeAcyclicBaseType(diagnostics)
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.MakeAcyclicInterfaces(diagnostics)
		End Function
	End Class
End Namespace