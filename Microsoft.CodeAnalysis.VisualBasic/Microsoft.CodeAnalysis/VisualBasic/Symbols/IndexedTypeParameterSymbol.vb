Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class IndexedTypeParameterSymbol
		Inherits TypeParameterSymbol
		Private Shared s_parameterPool As TypeParameterSymbol()

		Private ReadOnly _index As Integer

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Return ImmutableArray(Of TypeSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._index
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Microsoft.CodeAnalysis.TypeParameterKind.Method
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return VarianceKind.None
			End Get
		End Property

		Shared Sub New()
			IndexedTypeParameterSymbol.s_parameterPool = Array.Empty(Of TypeParameterSymbol)()
		End Sub

		Private Sub New(ByVal index As Integer)
			MyBase.New()
			Me._index = index
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
		End Sub

		Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return Me = other
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._index
		End Function

		Friend Shared Function GetTypeParameter(ByVal index As Integer) As TypeParameterSymbol
			If (index >= CInt(IndexedTypeParameterSymbol.s_parameterPool.Length)) Then
				IndexedTypeParameterSymbol.GrowPool(index + 1)
			End If
			Return IndexedTypeParameterSymbol.s_parameterPool(index)
		End Function

		Private Shared Sub GrowPool(ByVal count As Integer)
			Dim sParameterPool As TypeParameterSymbol() = Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol.s_parameterPool
			While count > CInt(sParameterPool.Length)
				Dim indexedTypeParameterSymbol((count + 15 And -16) - 1 + 1 - 1) As TypeParameterSymbol
				Array.Copy(sParameterPool, indexedTypeParameterSymbol, CInt(sParameterPool.Length))
				Dim length As Integer = CInt(indexedTypeParameterSymbol.Length) - 1
				Dim num As Integer = CInt(sParameterPool.Length)
				Do
					indexedTypeParameterSymbol(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol(num)
					num = num + 1
				Loop While num <= length
				Interlocked.CompareExchange(Of TypeParameterSymbol())(Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol.s_parameterPool, indexedTypeParameterSymbol, sParameterPool)
				sParameterPool = Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol.s_parameterPool
			End While
		End Sub

		Friend Shared Function Take(ByVal count As Integer) As ImmutableArray(Of TypeParameterSymbol)
			If (count > CInt(IndexedTypeParameterSymbol.s_parameterPool.Length)) Then
				IndexedTypeParameterSymbol.GrowPool(count)
			End If
			Dim instance As ArrayBuilder(Of TypeParameterSymbol) = ArrayBuilder(Of TypeParameterSymbol).GetInstance()
			Dim num As Integer = count - 1
			Dim num1 As Integer = 0
			Do
				instance.Add(IndexedTypeParameterSymbol.GetTypeParameter(num1))
				num1 = num1 + 1
			Loop While num1 <= num
			Return instance.ToImmutableAndFree()
		End Function
	End Class
End Namespace