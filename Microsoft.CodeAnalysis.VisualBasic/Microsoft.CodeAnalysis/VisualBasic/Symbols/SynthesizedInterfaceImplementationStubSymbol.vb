Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedInterfaceImplementationStubSymbol
		Inherits SynthesizedMethodBase
		Private ReadOnly _name As String

		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private ReadOnly _typeParametersSubstitution As TypeSubstitution

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnType As TypeSymbol

		Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

		Private _explicitInterfaceImplementationsBuilder As ArrayBuilder(Of MethodSymbol)

		Private _explicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)

		Private ReadOnly Shared s_typeParametersSubstitutionFactory As Func(Of Symbol, TypeSubstitution)

		Private ReadOnly Shared s_createTypeParameter As Func(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._typeParameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return Me._explicitInterfaceImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
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

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._returnType.IsVoidType()
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.Ordinary
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._customModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Shared Sub New()
			SynthesizedInterfaceImplementationStubSymbol.s_typeParametersSubstitutionFactory = Function(container As Symbol) DirectCast(container, SynthesizedInterfaceImplementationStubSymbol)._typeParametersSubstitution
			SynthesizedInterfaceImplementationStubSymbol.s_createTypeParameter = Function(typeParameter As TypeParameterSymbol, container As Symbol) New SynthesizedClonedTypeParameterSymbol(typeParameter, container, typeParameter.Name, SynthesizedInterfaceImplementationStubSymbol.s_typeParametersSubstitutionFactory)
		End Sub

		Friend Sub New(ByVal implementingMethod As MethodSymbol, ByVal implementedMethod As MethodSymbol)
			MyBase.New(implementingMethod.ContainingType)
			Me._explicitInterfaceImplementationsBuilder = ArrayBuilder(Of MethodSymbol).GetInstance()
			Me._name = [String].Concat("$VB$Stub_", implementingMethod.MetadataName)
			Me._typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(implementingMethod.TypeParameters, Me, SynthesizedInterfaceImplementationStubSymbol.s_createTypeParameter)
			Me._typeParametersSubstitution = TypeSubstitution.Create(implementingMethod, implementingMethod.TypeParameters, StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me._typeParameters), False)
			If (implementedMethod.IsGenericMethod) Then
				implementedMethod = implementedMethod.Construct(StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me._typeParameters))
			End If
			Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance()
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = implementingMethod.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				Dim item As ParameterSymbol = implementedMethod.Parameters(current.Ordinal)
				instance.Add(SynthesizedParameterSymbol.Create(Me, item.Type, current.Ordinal, current.IsByRef, current.Name, item.CustomModifiers, item.RefCustomModifiers))
			End While
			Me._parameters = instance.ToImmutableAndFree()
			Me._returnType = implementedMethod.ReturnType
			Me._customModifiers = implementedMethod.ReturnTypeCustomModifiers
		End Sub

		Public Sub AddImplementedMethod(ByVal implemented As MethodSymbol)
			Me._explicitInterfaceImplementationsBuilder.Add(implemented)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeDebuggerHiddenAttribute())
		End Sub

		Friend Overrides Sub AddSynthesizedReturnTypeAttributes(ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedReturnTypeAttributes(attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (Me.ReturnType.ContainsTupleNames() AndAlso declaringCompilation.HasTupleNamesAttributes) Then
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Me.ReturnType))
			End If
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Sub Seal()
			Me._explicitInterfaceImplementations = Me._explicitInterfaceImplementationsBuilder.ToImmutableAndFree()
			Me._explicitInterfaceImplementationsBuilder = Nothing
		End Sub
	End Class
End Namespace