Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SubstitutedTypeParameterSymbol
		Inherits TypeParameterSymbol
		Private _containingSymbol As Symbol

		Private ReadOnly _originalDefinition As TypeParameterSymbol

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Return TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(Me.TypeSubstitution, Me._originalDefinition.ConstraintTypesNoUseSiteDiagnostics)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._originalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Return Me._originalDefinition.HasConstructorConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Return Me._originalDefinition.HasReferenceTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Return Me._originalDefinition.HasValueTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._originalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._originalDefinition.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._originalDefinition.Name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._originalDefinition.Ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As TypeParameterSymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property ReducedFrom As TypeParameterSymbol
			Get
				Return Me._originalDefinition.ReducedFrom
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Me._originalDefinition.TypeParameterKind
			End Get
		End Property

		Private ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				If (Me._containingSymbol.Kind <> SymbolKind.Method) Then
					Return DirectCast(Me._containingSymbol, NamedTypeSymbol).TypeSubstitution
				End If
				Return DirectCast(Me._containingSymbol, SubstitutedMethodSymbol).TypeSubstitution
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return Me._originalDefinition.Variance
			End Get
		End Property

		Public Sub New(ByVal originalDefinition As TypeParameterSymbol)
			MyBase.New()
			Me._originalDefinition = originalDefinition
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			Me._originalDefinition.EnsureAllConstraintsAreResolved()
		End Sub

		Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return Me.Equals(TryCast(other, TypeParameterSymbol), comparison)
		End Function

		Private Function Equals(ByVal other As TypeParameterSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Dim flag As Boolean
			If (Me <> other) Then
				flag = If(other Is Nothing OrElse Not Me.OriginalDefinition.Equals(other.OriginalDefinition), False, Me.ContainingSymbol.Equals(other.ContainingSymbol, comparison))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalDefinition.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer
			Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = TryCast(Me._containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
			If (substitutedNamedType Is Nothing OrElse Not substitutedNamedType.TypeSubstitution.WasConstructedForModifiers()) Then
				Dim ordinal As Integer = Me.Ordinal
				hashCode = Hash.Combine(ordinal.GetHashCode(), Me._containingSymbol.GetHashCode())
			Else
				hashCode = Me._originalDefinition.GetHashCode()
			End If
			Return hashCode
		End Function

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
			Dim typeWithModifier As TypeWithModifiers
			If (substitution Is Nothing) Then
				typeWithModifier = New TypeWithModifiers(Me)
			Else
				If (CObj(substitution.TargetGenericDefinition) <> CObj(Me._containingSymbol)) Then
					Throw ExceptionUtilities.Unreachable
				End If
				typeWithModifier = substitution.GetSubstitutionFor(Me)
			End If
			Return typeWithModifier
		End Function

		Public Sub SetContainingSymbol(ByVal container As Symbol)
			Me._containingSymbol = container
		End Sub
	End Class
End Namespace