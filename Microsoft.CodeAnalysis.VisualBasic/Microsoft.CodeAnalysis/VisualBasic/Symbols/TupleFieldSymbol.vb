Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class TupleFieldSymbol
		Inherits WrappedFieldSymbol
		Protected ReadOnly _containingTuple As TupleTypeSymbol

		Private ReadOnly _tupleElementIndex As Integer

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._containingTuple.GetTupleMemberSymbolForUnderlyingMember(Of Symbol)(Me._underlyingField.AssociatedSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingTuple
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._underlyingField.CustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefaultTupleElement As Boolean
			Get
				Return (Me._tupleElementIndex And -2147483647) = 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsTupleField As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property TupleElementIndex As Integer
			Get
				Dim num As Integer
				num = If(Me._tupleElementIndex >= 0, Me._tupleElementIndex >> 1, -1)
				Return num
			End Get
		End Property

		Public Overrides ReadOnly Property TupleUnderlyingField As FieldSymbol
			Get
				Return Me._underlyingField
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._underlyingField.Type
			End Get
		End Property

		Public Sub New(ByVal container As TupleTypeSymbol, ByVal underlyingField As FieldSymbol, ByVal tupleElementIndex As Integer)
			MyBase.New(underlyingField)
			Me._containingTuple = container
			Me._tupleElementIndex = tupleElementIndex
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TupleFieldSymbol))
		End Function

		Public Function Equals(ByVal other As TupleFieldSymbol) As Boolean
			If (other Is Nothing OrElse Me._tupleElementIndex <> other._tupleElementIndex) Then
				Return False
			End If
			Return TypeSymbol.Equals(Me._containingTuple, other._containingTuple, TypeCompareKind.ConsiderEverything)
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingField.GetAttributes()
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me._containingTuple.GetHashCode(), Me._tupleElementIndex.GetHashCode())
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.GetUseSiteInfo()
			MyBase.MergeUseSiteInfo(useSiteInfo, Me._underlyingField.GetUseSiteInfo())
			Return useSiteInfo
		End Function
	End Class
End Namespace