Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TuplePropertySymbol
		Inherits WrappedPropertySymbol
		Private ReadOnly _containingType As TupleTypeSymbol

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._containingType.GetTupleMemberSymbolForUnderlyingMember(Of FieldSymbol)(Me._underlyingProperty.AssociatedField)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Return Me._underlyingProperty.ExplicitInterfaceImplementations
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._containingType.GetTupleMemberSymbolForUnderlyingMember(Of MethodSymbol)(Me._underlyingProperty.GetMethod)
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return Me._underlyingProperty.IsMyGroupCollectionProperty
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me._underlyingProperty.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsTupleProperty As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me._lazyParameters.IsDefault) Then
					InterlockedOperations.Initialize(Of ParameterSymbol)(Me._lazyParameters, Me.CreateParameters())
				End If
				Return Me._lazyParameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._underlyingProperty.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._containingType.GetTupleMemberSymbolForUnderlyingMember(Of MethodSymbol)(Me._underlyingProperty.SetMethod)
			End Get
		End Property

		Public Overrides ReadOnly Property TupleUnderlyingProperty As PropertySymbol
			Get
				Return Me._underlyingProperty
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._underlyingProperty.Type
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._underlyingProperty.TypeCustomModifiers
			End Get
		End Property

		Public Sub New(ByVal container As TupleTypeSymbol, ByVal underlyingProperty As PropertySymbol)
			MyBase.New(underlyingProperty)
			Me._containingType = container
		End Sub

		Private Function CreateParameters() As ImmutableArray(Of ParameterSymbol)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of ParameterSymbol, ParameterSymbol)(Me._underlyingProperty.Parameters, Function(p As ParameterSymbol) New TupleParameterSymbol(Me, p))
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TuplePropertySymbol))
		End Function

		Public Function Equals(ByVal other As TuplePropertySymbol) As Boolean
			If (CObj(other) = CObj(Me)) Then
				Return True
			End If
			If (other Is Nothing OrElse Not TypeSymbol.Equals(Me._containingType, other._containingType, TypeCompareKind.ConsiderEverything)) Then
				Return False
			End If
			Return Me._underlyingProperty = other._underlyingProperty
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingProperty.GetAttributes()
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._underlyingProperty.GetHashCode()
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.GetUseSiteInfo()
			MyBase.MergeUseSiteInfo(useSiteInfo, Me._underlyingProperty.GetUseSiteInfo())
			Return useSiteInfo
		End Function
	End Class
End Namespace