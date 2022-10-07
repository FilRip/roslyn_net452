Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class ExtendedErrorTypeSymbol
		Inherits InstanceErrorTypeSymbol
		Private ReadOnly _diagnosticInfo As DiagnosticInfo

		Private ReadOnly _reportErrorWhenReferenced As Boolean

		Private ReadOnly _name As String

		Private ReadOnly _candidateSymbols As ImmutableArray(Of Symbol)

		Private ReadOnly _resultKind As LookupResultKind

		Private ReadOnly _containingSymbol As NamespaceOrTypeSymbol

		Public Overrides ReadOnly Property CandidateSymbols As ImmutableArray(Of Symbol)
			Get
				Return Me._candidateSymbols
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Return Me._diagnosticInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return MyBase.Arity > 0
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me._resultKind
			End Get
		End Property

		Friend Sub New(ByVal errorInfo As DiagnosticInfo, Optional ByVal reportErrorWhenReferenced As Boolean = False, Optional ByVal nonErrorGuessType As NamedTypeSymbol = Nothing)
			MyClass.New(errorInfo, [String].Empty, 0, reportErrorWhenReferenced, nonErrorGuessType)
		End Sub

		Friend Sub New(ByVal errorInfo As DiagnosticInfo, ByVal name As String, Optional ByVal reportErrorWhenReferenced As Boolean = False, Optional ByVal nonErrorGuessType As NamedTypeSymbol = Nothing)
			MyClass.New(errorInfo, name, 0, reportErrorWhenReferenced, nonErrorGuessType)
		End Sub

		Friend Sub New(ByVal errorInfo As DiagnosticInfo, ByVal name As String, ByVal arity As Integer, ByVal candidateSymbols As ImmutableArray(Of Symbol), ByVal resultKind As LookupResultKind, Optional ByVal reportErrorWhenReferenced As Boolean = False)
			MyBase.New(arity)
			Me._name = name
			Me._diagnosticInfo = errorInfo
			Me._reportErrorWhenReferenced = reportErrorWhenReferenced
			If (candidateSymbols.Length <> 1 OrElse candidateSymbols(0).Kind <> SymbolKind.[Namespace] OrElse CInt(DirectCast(candidateSymbols(0), NamespaceSymbol).NamespaceKind) <> 0) Then
				Me._candidateSymbols = candidateSymbols
			Else
				Me._candidateSymbols = StaticCast(Of Symbol).From(Of NamespaceSymbol)(DirectCast(candidateSymbols(0), NamespaceSymbol).ConstituentNamespaces)
			End If
			Me._resultKind = resultKind
		End Sub

		Friend Sub New(ByVal errorInfo As DiagnosticInfo, ByVal name As String, ByVal arity As Integer, Optional ByVal reportErrorWhenReferenced As Boolean = False, Optional ByVal nonErrorGuessType As NamedTypeSymbol = Nothing)
			MyBase.New(arity)
			Me._name = name
			Me._diagnosticInfo = errorInfo
			Me._reportErrorWhenReferenced = reportErrorWhenReferenced
			If (nonErrorGuessType Is Nothing) Then
				Me._candidateSymbols = ImmutableArray(Of Symbol).Empty
				Me._resultKind = LookupResultKind.Empty
				Return
			End If
			Me._candidateSymbols = ImmutableArray.Create(Of Symbol)(nonErrorGuessType)
			Me._resultKind = LookupResultKind.NotATypeOrNamespace
		End Sub

		Friend Sub New(ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal name As String, ByVal arity As Integer)
			MyClass.New(Nothing, name, arity, False, Nothing)
			Me._containingSymbol = containingSymbol
		End Sub

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(MyBase.Arity, Hash.Combine(If(Me.ContainingSymbol Is Nothing, 0, Me.ContainingSymbol.GetHashCode()), If(Me.Name Is Nothing, 0, Me.Name.GetHashCode())))
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not Me._reportErrorWhenReferenced, New UseSiteInfo(Of AssemblySymbol)(), New UseSiteInfo(Of AssemblySymbol)(Me.ErrorInfo))
			Return useSiteInfo
		End Function

		Protected Overrides Function SpecializedEquals(ByVal obj As InstanceErrorTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim extendedErrorTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ExtendedErrorTypeSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.ExtendedErrorTypeSymbol)
			If (extendedErrorTypeSymbol IsNot Nothing) Then
				flag = If(Not [Object].Equals(Me.ContainingSymbol, extendedErrorTypeSymbol.ContainingSymbol) OrElse Not [String].Equals(Me.Name, extendedErrorTypeSymbol.Name, StringComparison.Ordinal), False, MyBase.Arity = extendedErrorTypeSymbol.Arity)
			Else
				flag = False
			End If
			Return flag
		End Function
	End Class
End Namespace