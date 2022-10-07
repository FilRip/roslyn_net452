Imports Microsoft.CodeAnalysis
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SingleTypeDeclaration
		Inherits SingleNamespaceOrTypeDeclaration
		Private ReadOnly _children As ImmutableArray(Of SingleTypeDeclaration)

		Private ReadOnly _kind As DeclarationKind

		Private ReadOnly _flags As SingleTypeDeclaration.TypeDeclarationFlags

		Private ReadOnly _arity As UShort

		Private ReadOnly _modifiers As DeclarationModifiers

		Public ReadOnly Shared EqualityComparer As IEqualityComparer(Of SingleTypeDeclaration)

		Public ReadOnly Property AnyMemberHasAttributes As Boolean
			Get
				Return (Me._flags And SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasAttributes) <> SingleTypeDeclaration.TypeDeclarationFlags.None
			End Get
		End Property

		Public ReadOnly Property Arity As Integer
			Get
				Return Me._arity
			End Get
		End Property

		Public Shadows ReadOnly Property Children As ImmutableArray(Of SingleTypeDeclaration)
			Get
				Return Me._children
			End Get
		End Property

		Public ReadOnly Property HasAnyAttributes As Boolean
			Get
				Return (Me._flags And SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes) <> SingleTypeDeclaration.TypeDeclarationFlags.None
			End Get
		End Property

		Public ReadOnly Property HasAnyNontypeMembers As Boolean
			Get
				Return (Me._flags And SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers) <> SingleTypeDeclaration.TypeDeclarationFlags.None
			End Get
		End Property

		Public ReadOnly Property HasBaseDeclarations As Boolean
			Get
				Return (Me._flags And SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations) <> SingleTypeDeclaration.TypeDeclarationFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As DeclarationKind
			Get
				Return Me._kind
			End Get
		End Property

		Public ReadOnly Property MemberNames As ImmutableHashSet(Of String)

		Public ReadOnly Property Modifiers As DeclarationModifiers
			Get
				Return Me._modifiers
			End Get
		End Property

		Shared Sub New()
			SingleTypeDeclaration.EqualityComparer = New SingleTypeDeclaration.Comparer()
		End Sub

		Public Sub New(ByVal kind As DeclarationKind, ByVal name As String, ByVal arity As Integer, ByVal modifiers As DeclarationModifiers, ByVal declFlags As SingleTypeDeclaration.TypeDeclarationFlags, ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal nameLocation As Microsoft.CodeAnalysis.Location, ByVal memberNames As ImmutableHashSet(Of String), ByVal children As ImmutableArray(Of SingleTypeDeclaration))
			MyBase.New(name, syntaxReference, nameLocation)
			Me._kind = kind
			Me._arity = CUShort(arity)
			Me._flags = declFlags
			Me._modifiers = modifiers
			Me.MemberNames = memberNames
			Me._children = children
		End Sub

		Private Function GetEmbeddedSymbolKind() As EmbeddedSymbolKind
			Return Me.SyntaxReference.SyntaxTree.GetEmbeddedKind()
		End Function

		Protected Overrides Function GetNamespaceOrTypeDeclarationChildren() As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)
			Return StaticCast(Of SingleNamespaceOrTypeDeclaration).From(Of SingleTypeDeclaration)(Me._children)
		End Function

		Private NotInheritable Class Comparer
			Implements IEqualityComparer(Of SingleTypeDeclaration)
			Public Sub New()
				MyBase.New()
			End Sub

			Private Function Equals(ByVal decl1 As SingleTypeDeclaration, ByVal decl2 As SingleTypeDeclaration) As Boolean Implements IEqualityComparer(Of SingleTypeDeclaration).Equals
				If (Not CaseInsensitiveComparison.Equals(decl1.Name, decl2.Name) OrElse decl1.Kind <> decl2.Kind OrElse decl1.Kind = DeclarationKind.[Enum] OrElse decl1.Arity <> decl2.Arity) Then
					Return False
				End If
				Return decl1.GetEmbeddedSymbolKind() = decl2.GetEmbeddedSymbolKind()
			End Function

			Private Function GetHashCode(ByVal decl1 As SingleTypeDeclaration) As Integer Implements IEqualityComparer(Of SingleTypeDeclaration).GetHashCode
				Dim hashCode As Integer = CaseInsensitiveComparison.GetHashCode(decl1.Name)
				Dim arity As Integer = decl1.Arity
				Return Hash.Combine(hashCode, Hash.Combine(arity.GetHashCode(), CInt(decl1.Kind)))
			End Function
		End Class

		Friend Enum TypeDeclarationFlags As Byte
			None = 0
			HasAnyAttributes = 2
			HasBaseDeclarations = 4
			AnyMemberHasAttributes = 8
			HasAnyNontypeMembers = 16
		End Enum
	End Class
End Namespace