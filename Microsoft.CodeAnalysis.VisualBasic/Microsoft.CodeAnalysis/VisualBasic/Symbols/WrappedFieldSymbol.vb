Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class WrappedFieldSymbol
		Inherits FieldSymbol
		Protected _underlyingField As FieldSymbol

		Public Overrides ReadOnly Property ConstantValue As Object
			Get
				Return Me._underlyingField.ConstantValue
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingField.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingField.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._underlyingField.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingField.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return Me._underlyingField.IsConst
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingField.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me._underlyingField.IsMarshalledExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return Me._underlyingField.IsNotSerialized
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return Me._underlyingField.IsReadOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._underlyingField.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingField.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me._underlyingField.MarshallingDescriptor
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._underlyingField.MarshallingInformation
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingField.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingField.ObsoleteAttributeData
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Me._underlyingField.TypeLayoutOffset
			End Get
		End Property

		Public ReadOnly Property UnderlyingField As FieldSymbol
			Get
				Return Me._underlyingField
			End Get
		End Property

		Public Sub New(ByVal underlyingField As FieldSymbol)
			MyBase.New()
			Me._underlyingField = underlyingField
		End Sub

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Me._underlyingField.GetConstantValue(inProgress)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingField.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function
	End Class
End Namespace