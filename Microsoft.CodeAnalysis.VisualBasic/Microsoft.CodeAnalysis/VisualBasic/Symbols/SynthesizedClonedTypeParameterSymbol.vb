Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedClonedTypeParameterSymbol
		Inherits SubstitutableTypeParameterSymbol
		Private ReadOnly _typeMapFactory As Func(Of Symbol, TypeSubstitution)

		Private ReadOnly _container As Symbol

		Private ReadOnly _correspondingMethodTypeParameter As TypeParameterSymbol

		Private ReadOnly _name As String

		Private _lazyConstraints As ImmutableArray(Of TypeSymbol)

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				If (Me._lazyConstraints.IsDefault) Then
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(Me.TypeMap, Me._correspondingMethodTypeParameter.ConstraintTypesNoUseSiteDiagnostics)
					ImmutableInterlocked.InterlockedInitialize(Of TypeSymbol)(Me._lazyConstraints, typeSymbols)
				End If
				Return Me._lazyConstraints
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._correspondingMethodTypeParameter.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Return Me._correspondingMethodTypeParameter.HasConstructorConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Return Me._correspondingMethodTypeParameter.HasReferenceTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Return Me._correspondingMethodTypeParameter.HasValueTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._correspondingMethodTypeParameter.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._correspondingMethodTypeParameter.Ordinal
			End Get
		End Property

		Private ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Me._typeMapFactory(Me._container)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				If (Not TypeOf Me.ContainingSymbol Is MethodSymbol) Then
					Return Microsoft.CodeAnalysis.TypeParameterKind.Type
				End If
				Return Microsoft.CodeAnalysis.TypeParameterKind.Method
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return Me._correspondingMethodTypeParameter.Variance
			End Get
		End Property

		Friend Sub New(ByVal correspondingMethodTypeParameter As TypeParameterSymbol, ByVal container As Symbol, ByVal name As String, ByVal typeMapFactory As Func(Of Symbol, TypeSubstitution))
			MyBase.New()
			Me._container = container
			Me._correspondingMethodTypeParameter = correspondingMethodTypeParameter
			Me._name = name
			Me._typeMapFactory = typeMapFactory
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			Me._correspondingMethodTypeParameter.EnsureAllConstraintsAreResolved()
		End Sub

		Friend Shared Function MakeTypeParameters(ByVal origParameters As ImmutableArray(Of TypeParameterSymbol), ByVal container As Symbol, ByVal mapFunction As Func(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)) As ImmutableArray(Of TypeParameterSymbol)
			Return ImmutableArrayExtensions.SelectAsArray(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)(origParameters, mapFunction, container)
		End Function
	End Class
End Namespace