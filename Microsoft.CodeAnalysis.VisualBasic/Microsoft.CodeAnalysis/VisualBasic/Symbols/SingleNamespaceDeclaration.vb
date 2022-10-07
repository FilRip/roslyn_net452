Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SingleNamespaceDeclaration
		Inherits SingleNamespaceOrTypeDeclaration
		Private ReadOnly _children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)

		Public ReadOnly IsPartOfRootNamespace As Boolean

		Public ReadOnly Shared EqualityComparer As IEqualityComparer(Of SingleNamespaceDeclaration)

		Public Property HasImports As Boolean

		Public Overridable ReadOnly Property IsGlobalNamespace As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As DeclarationKind
			Get
				Return DeclarationKind.[Namespace]
			End Get
		End Property

		Shared Sub New()
			SingleNamespaceDeclaration.EqualityComparer = New SingleNamespaceDeclaration.Comparer()
		End Sub

		Public Sub New(ByVal name As String, ByVal hasImports As Boolean, ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal nameLocation As Microsoft.CodeAnalysis.Location, ByVal children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration), Optional ByVal isPartOfRootNamespace As Boolean = False)
			MyBase.New(name, syntaxReference, nameLocation)
			Me._children = children
			Me.HasImports = hasImports
			Me.IsPartOfRootNamespace = isPartOfRootNamespace
		End Sub

		Public Shared Shadows Function BestName(Of T As SingleNamespaceDeclaration)(ByVal singleDeclarations As ImmutableArray(Of T), ByRef multipleSpellings As Boolean) As String
			Dim str As String
			multipleSpellings = False
			Dim name As String = singleDeclarations(0).Name
			Dim length As Integer = singleDeclarations.Length - 1
			Dim num As Integer = 1
			While True
				If (num <= length) Then
					Dim name1 As String = singleDeclarations(num).Name
					Dim num1 As Integer = [String].Compare(name, name1, StringComparison.Ordinal)
					If (num1 <> 0) Then
						multipleSpellings = True
						If (singleDeclarations(0).IsPartOfRootNamespace) Then
							str = name
							Exit While
						ElseIf (singleDeclarations(num).IsPartOfRootNamespace) Then
							str = name1
							Exit While
						ElseIf (num1 > 0) Then
							name = name1
						End If
					End If
					num = num + 1
				Else
					str = name
					Exit While
				End If
			End While
			Return str
		End Function

		Public Function GetNamespaceBlockSyntax() As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			If (Me.SyntaxReference IsNot Nothing) Then
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me.SyntaxReference
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				namespaceBlockSyntax = syntaxReference.GetSyntax(cancellationToken).AncestorsAndSelf(True).OfType(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax)().FirstOrDefault()
			Else
				namespaceBlockSyntax = Nothing
			End If
			Return namespaceBlockSyntax
		End Function

		Protected Overrides Function GetNamespaceOrTypeDeclarationChildren() As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)
			Return Me._children
		End Function

		Private Class Comparer
			Implements IEqualityComparer(Of SingleNamespaceDeclaration)
			Public Sub New()
				MyBase.New()
			End Sub

			Private Function Equals(ByVal decl1 As SingleNamespaceDeclaration, ByVal decl2 As SingleNamespaceDeclaration) As Boolean Implements IEqualityComparer(Of SingleNamespaceDeclaration).Equals
				Return CaseInsensitiveComparison.Equals(decl1.Name, decl2.Name)
			End Function

			Private Function GetHashCode(ByVal decl1 As SingleNamespaceDeclaration) As Integer Implements IEqualityComparer(Of SingleNamespaceDeclaration).GetHashCode
				Return CaseInsensitiveComparison.GetHashCode(decl1.Name)
			End Function
		End Class
	End Class
End Namespace