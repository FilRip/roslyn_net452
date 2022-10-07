Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SubstitutedFieldSymbol
		Inherits FieldSymbol
		Private ReadOnly _containingType As SubstitutedNamedType

		Private ReadOnly _originalDefinition As FieldSymbol

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.OriginalDefinition.AssociatedSymbol
				If (symbol Is Nothing) Then
					Return Nothing
				End If
				Return symbol.AsMember(Me.ContainingType)
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValue As Object
			Get
				Return Me._originalDefinition.ConstantValue
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._containingType.TypeSubstitution.SubstituteCustomModifiers(Me._originalDefinition.Type, Me._originalDefinition.CustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._originalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._originalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._originalDefinition.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._originalDefinition.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return Me._originalDefinition.IsConst
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._originalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return Me._originalDefinition.IsNotSerialized
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return Me._originalDefinition.IsReadOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._originalDefinition.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._originalDefinition.MarshallingInformation
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

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As FieldSymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._originalDefinition.Type.InternalSubstituteTypeParameters(Me._containingType.TypeSubstitution).Type
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Me._originalDefinition.TypeLayoutOffset
			End Get
		End Property

		Public Sub New(ByVal container As SubstitutedNamedType, ByVal originalDefinition As FieldSymbol)
			MyBase.New()
			Me._containingType = container
			Me._originalDefinition = originalDefinition
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim substitutedFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol)
				If (substitutedFieldSymbol Is Nothing) Then
					flag = False
				ElseIf (Me._originalDefinition.Equals(substitutedFieldSymbol._originalDefinition)) Then
					flag = If(Me._containingType.Equals(substitutedFieldSymbol._containingType), True, False)
				Else
					flag = False
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalDefinition.GetAttributes()
		End Function

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Me._originalDefinition.GetConstantValue(inProgress)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = Me._originalDefinition.GetHashCode()
			Return Hash.Combine(Of SubstitutedNamedType)(Me._containingType, hashCode)
		End Function
	End Class
End Namespace