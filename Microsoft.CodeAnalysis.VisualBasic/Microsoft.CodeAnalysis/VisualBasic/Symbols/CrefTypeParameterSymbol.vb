Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class CrefTypeParameterSymbol
		Inherits TypeParameterSymbol
		Private ReadOnly _ordinal As Integer

		Private ReadOnly _name As String

		Private ReadOnly _syntaxReference As SyntaxReference

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
				Return ImmutableArray.Create(Of SyntaxReference)(Me._syntaxReference)
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
				Return ImmutableArray.Create(Of Location)(Me._syntaxReference.GetLocation())
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Microsoft.CodeAnalysis.TypeParameterKind.Cref
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return VarianceKind.None
			End Get
		End Property

		Public Sub New(ByVal ordinal As Integer, ByVal name As String, ByVal syntax As TypeSyntax)
			MyBase.New()
			Me._ordinal = ordinal
			Me._name = name
			Me._syntaxReference = syntax.GetReference()
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
		End Sub

		Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return Me.Equals(TryCast(other, CrefTypeParameterSymbol))
		End Function

		Public Function Equals(ByVal other As CrefTypeParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (CObj(Me) = CObj(other)) Then
				flag = True
			ElseIf (other IsNot Nothing) Then
				If (EmbeddedOperators.CompareString(Me._name, other._name, False) <> 0 OrElse Me._ordinal <> other._ordinal) Then
					flag1 = False
				Else
					Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxReference
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Dim syntax As SyntaxNode = syntaxReference.GetSyntax(cancellationToken)
					Dim syntaxReference1 As Microsoft.CodeAnalysis.SyntaxReference = other._syntaxReference
					cancellationToken = New System.Threading.CancellationToken()
					flag1 = syntax.Equals(syntaxReference1.GetSyntax(cancellationToken))
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Friend Overrides Function GetConstraints() As ImmutableArray(Of TypeParameterConstraint)
			Return ImmutableArray(Of TypeParameterConstraint).Empty
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Of String)(Me._name, Me._ordinal)
		End Function

		Friend Overrides Sub ResolveConstraints(ByVal inProgress As ConsList(Of TypeParameterSymbol))
		End Sub
	End Class
End Namespace