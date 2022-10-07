Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TupleMethodSymbol
		Inherits WrappedMethodSymbol
		Private ReadOnly _containingType As TupleTypeSymbol

		Private ReadOnly _underlyingMethod As MethodSymbol

		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._containingType.GetTupleMemberSymbolForUnderlyingMember(Of Symbol)(Me._underlyingMethod.ConstructedFrom.AssociatedSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return Me._underlyingMethod.ConstructedFrom.ExplicitInterfaceImplementations
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._underlyingMethod.IsSub
			End Get
		End Property

		Public Overrides ReadOnly Property IsTupleMethod As Boolean
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
				Return Me._underlyingMethod.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._underlyingMethod.ReturnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._underlyingMethod.ReturnTypeCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property TupleUnderlyingMethod As MethodSymbol
			Get
				Return Me._underlyingMethod.ConstructedFrom
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me._typeParameters)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Public Overrides ReadOnly Property UnderlyingMethod As MethodSymbol
			Get
				Return Me._underlyingMethod
			End Get
		End Property

		Public Sub New(ByVal container As TupleTypeSymbol, ByVal underlyingMethod As MethodSymbol)
			MyBase.New()
			Me._containingType = container
			Me._underlyingMethod = underlyingMethod
			Me._typeParameters = Me._underlyingMethod.TypeParameters
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function CreateParameters() As ImmutableArray(Of ParameterSymbol)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of ParameterSymbol, ParameterSymbol)(Me._underlyingMethod.Parameters, Function(p As ParameterSymbol) New TupleParameterSymbol(Me, p))
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TupleMethodSymbol))
		End Function

		Public Function Equals(ByVal other As TupleMethodSymbol) As Boolean
			If (CObj(other) = CObj(Me)) Then
				Return True
			End If
			If (other Is Nothing OrElse Not TypeSymbol.Equals(Me._containingType, other._containingType, TypeCompareKind.ConsiderEverything)) Then
				Return False
			End If
			Return Me._underlyingMethod.ConstructedFrom = other._underlyingMethod.ConstructedFrom
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingMethod.GetAttributes()
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._underlyingMethod.ConstructedFrom.GetHashCode()
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingMethod.GetReturnTypeAttributes()
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.GetUseSiteInfo()
			MyBase.MergeUseSiteInfo(useSiteInfo, Me._underlyingMethod.GetUseSiteInfo())
			Return useSiteInfo
		End Function
	End Class
End Namespace