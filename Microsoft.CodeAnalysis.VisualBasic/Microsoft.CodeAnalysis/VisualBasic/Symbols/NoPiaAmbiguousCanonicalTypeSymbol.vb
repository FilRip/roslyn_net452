Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class NoPiaAmbiguousCanonicalTypeSymbol
		Inherits ErrorTypeSymbol
		Private ReadOnly _embeddingAssembly As AssemblySymbol

		Private ReadOnly _firstCandidate As NamedTypeSymbol

		Private ReadOnly _secondCandidate As NamedTypeSymbol

		Public ReadOnly Property EmbeddingAssembly As AssemblySymbol
			Get
				Return Me._embeddingAssembly
			End Get
		End Property

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Return ErrorFactory.ErrorInfo(ERRID.ERR_AbsentReferenceToPIA1, New [Object]() { CustomSymbolDisplayFormatter.QualifiedName(Me._firstCandidate) })
			End Get
		End Property

		Public ReadOnly Property FirstCandidate As NamedTypeSymbol
			Get
				Return Me._firstCandidate
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property SecondCandidate As NamedTypeSymbol
			Get
				Return Me._secondCandidate
			End Get
		End Property

		Public Sub New(ByVal embeddingAssembly As AssemblySymbol, ByVal firstCandidate As NamedTypeSymbol, ByVal secondCandidate As NamedTypeSymbol)
			MyBase.New()
			Me._embeddingAssembly = embeddingAssembly
			Me._firstCandidate = firstCandidate
			Me._secondCandidate = secondCandidate
		End Sub

		Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return obj = Me
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return RuntimeHelpers.GetHashCode(Me)
		End Function
	End Class
End Namespace