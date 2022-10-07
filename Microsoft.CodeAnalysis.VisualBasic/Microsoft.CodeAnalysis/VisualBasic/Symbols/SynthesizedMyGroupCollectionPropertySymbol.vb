Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedMyGroupCollectionPropertySymbol
		Inherits SynthesizedPropertyBase
		Private ReadOnly _name As String

		Private ReadOnly _field As SynthesizedMyGroupCollectionPropertyBackingFieldSymbol

		Private ReadOnly _getMethod As SynthesizedMyGroupCollectionPropertyGetAccessorSymbol

		Private ReadOnly _setMethodOpt As SynthesizedMyGroupCollectionPropertySetAccessorSymbol

		Public ReadOnly AttributeSyntax As SyntaxReference

		Public ReadOnly DefaultInstanceAlias As String

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._field
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._field.ContainingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._field.ContainingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._setMethodOpt
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._field.Type
			End Get
		End Property

		Public Sub New(ByVal container As SourceNamedTypeSymbol, ByVal attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, ByVal propertyName As String, ByVal fieldName As String, ByVal type As NamedTypeSymbol, ByVal createMethod As String, ByVal disposeMethod As String, ByVal defaultInstanceAlias As String)
			MyBase.New()
			Me.AttributeSyntax = attributeSyntax.SyntaxTree.GetReference(attributeSyntax)
			Me.DefaultInstanceAlias = defaultInstanceAlias
			Me._name = propertyName
			Me._field = New SynthesizedMyGroupCollectionPropertyBackingFieldSymbol(container, Me, type, fieldName)
			Me._getMethod = New SynthesizedMyGroupCollectionPropertyGetAccessorSymbol(container, Me, createMethod)
			If (disposeMethod.Length > 0) Then
				Me._setMethodOpt = New SynthesizedMyGroupCollectionPropertySetAccessorSymbol(container, Me, disposeMethod)
			End If
		End Sub

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return LexicalSortKey.NotInSource
		End Function

		Public Sub RelocateDiagnostics(ByVal source As DiagnosticBag, ByVal destination As DiagnosticBag)
			Dim enumerator As IEnumerator(Of Diagnostic) = Nothing
			If (Not source.IsEmptyWithoutResolution) Then
				Using location As Microsoft.CodeAnalysis.Location = Me.AttributeSyntax.GetLocation()
					enumerator = source.AsEnumerable().GetEnumerator()
					While enumerator.MoveNext()
						destination.Add(DirectCast(enumerator.Current, VBDiagnostic).WithLocation(location))
					End While
				End Using
			End If
		End Sub
	End Class
End Namespace