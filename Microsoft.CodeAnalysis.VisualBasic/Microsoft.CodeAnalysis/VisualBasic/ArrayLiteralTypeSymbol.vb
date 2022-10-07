Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ArrayLiteralTypeSymbol
		Inherits ArrayTypeSymbol
		Private ReadOnly _arrayLiteral As BoundArrayLiteral

		Friend ReadOnly Property ArrayLiteral As BoundArrayLiteral
			Get
				Return Me._arrayLiteral
			End Get
		End Property

		Friend Overrides ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol
			Get
				Return Me._arrayLiteral.InferredType.BaseTypeNoUseSiteDiagnostics
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._arrayLiteral.InferredType.CustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ElementType As TypeSymbol
			Get
				Return Me._arrayLiteral.InferredType.ElementType
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDefaultSizesAndLowerBounds As Boolean
			Get
				Return Me._arrayLiteral.InferredType.HasDefaultSizesAndLowerBounds
			End Get
		End Property

		Friend Overrides ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
			Get
				Return Me._arrayLiteral.InferredType.InterfacesNoUseSiteDiagnostics
			End Get
		End Property

		Friend Overrides ReadOnly Property IsSZArray As Boolean
			Get
				Return Me._arrayLiteral.InferredType.IsSZArray
			End Get
		End Property

		Public Overrides ReadOnly Property Rank As Integer
			Get
				Return Me._arrayLiteral.InferredType.Rank
			End Get
		End Property

		Friend Sub New(ByVal arrayLiteral As BoundArrayLiteral)
			MyBase.New()
			Me._arrayLiteral = arrayLiteral
		End Sub

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function WithElementType(ByVal elementType As TypeSymbol) As ArrayTypeSymbol
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace