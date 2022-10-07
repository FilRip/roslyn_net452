Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class WrappedTypeParameterSymbol
		Inherits TypeParameterSymbol
		Protected _underlyingTypeParameter As TypeParameterSymbol

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingTypeParameter.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Return Me._underlyingTypeParameter.HasConstructorConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Return Me._underlyingTypeParameter.HasReferenceTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Return Me._underlyingTypeParameter.HasValueTypeConstraint
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingTypeParameter.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingTypeParameter.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingTypeParameter.Name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._underlyingTypeParameter.Ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Me._underlyingTypeParameter.TypeParameterKind
			End Get
		End Property

		Public ReadOnly Property UnderlyingTypeParameter As TypeParameterSymbol
			Get
				Return Me._underlyingTypeParameter
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return Me._underlyingTypeParameter.Variance
			End Get
		End Property

		Public Sub New(ByVal underlyingTypeParameter As TypeParameterSymbol)
			MyBase.New()
			Me._underlyingTypeParameter = underlyingTypeParameter
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			Me._underlyingTypeParameter.EnsureAllConstraintsAreResolved()
		End Sub

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function
	End Class
End Namespace