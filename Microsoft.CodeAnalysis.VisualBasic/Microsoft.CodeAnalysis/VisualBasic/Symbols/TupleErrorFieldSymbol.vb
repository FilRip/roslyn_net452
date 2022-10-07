Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TupleErrorFieldSymbol
		Inherits SynthesizedFieldSymbol
		Private ReadOnly _tupleElementIndex As Integer

		Private ReadOnly _locations As ImmutableArray(Of Location)

		Private ReadOnly _useSiteDiagnosticInfo As DiagnosticInfo

		Private ReadOnly _correspondingDefaultField As TupleErrorFieldSymbol

		Private ReadOnly _isImplicitlyDeclared As Boolean

		Public Overrides ReadOnly Property CorrespondingTupleField As FieldSymbol
			Get
				Return Me._correspondingDefaultField
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				If (Me._isImplicitlyDeclared) Then
					Return ImmutableArray(Of SyntaxReference).Empty
				End If
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of VisualBasicSyntaxNode)(Me._locations)
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefaultTupleElement As Boolean
			Get
				Return (Me._tupleElementIndex And -2147483647) = 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._isImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsTupleField As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
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
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Public Sub New(ByVal container As NamedTypeSymbol, ByVal name As String, ByVal tupleElementIndex As Integer, ByVal location As Microsoft.CodeAnalysis.Location, ByVal type As TypeSymbol, ByVal useSiteDiagnosticInfo As DiagnosticInfo, ByVal isImplicitlyDeclared As Boolean, ByVal correspondingDefaultFieldOpt As TupleErrorFieldSymbol)
			MyBase.New(container, container, type, name, Accessibility.[Public], False, False, False)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
			If (location Is Nothing) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.Location).Empty
			Else
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location)
			End If
			Me._locations = empty
			Me._useSiteDiagnosticInfo = useSiteDiagnosticInfo
			Me._tupleElementIndex = If(correspondingDefaultFieldOpt Is Nothing, tupleElementIndex << 1, (tupleElementIndex << 1) + 1)
			Me._isImplicitlyDeclared = isImplicitlyDeclared
			Me._correspondingDefaultField = If(correspondingDefaultFieldOpt, Me)
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TupleErrorFieldSymbol))
		End Function

		Public Function Equals(ByVal other As TupleErrorFieldSymbol) As Boolean
			If (CObj(other) = CObj(Me)) Then
				Return True
			End If
			If (other Is Nothing OrElse Me._tupleElementIndex <> other._tupleElementIndex) Then
				Return False
			End If
			Return TypeSymbol.Equals(Me.ContainingType, other.ContainingType, TypeCompareKind.ConsiderEverything)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me.ContainingType.GetHashCode(), Me._tupleElementIndex.GetHashCode())
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return New UseSiteInfo(Of AssemblySymbol)(Me._useSiteDiagnosticInfo)
		End Function
	End Class
End Namespace