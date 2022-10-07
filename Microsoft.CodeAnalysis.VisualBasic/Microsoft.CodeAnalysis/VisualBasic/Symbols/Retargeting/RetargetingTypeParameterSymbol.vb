Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingTypeParameterSymbol
		Inherits SubstitutableTypeParameterSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingTypeParameter As TypeParameterSymbol

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingTypeParameter.ConstraintTypesNoUseSiteDiagnostics)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._retargetingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingTypeParameter.ContainingSymbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

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

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingTypeParameter.MetadataName
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

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
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

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingTypeParameter As TypeParameterSymbol)
			MyBase.New()
			If (TypeOf underlyingTypeParameter Is RetargetingTypeParameterSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingTypeParameter = underlyingTypeParameter
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			Me._underlyingTypeParameter.EnsureAllConstraintsAreResolved()
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingTypeParameter.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function
	End Class
End Namespace