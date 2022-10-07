Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SingleNamespaceOrTypeDeclaration
		Inherits Declaration
		Public ReadOnly SyntaxReference As Microsoft.CodeAnalysis.SyntaxReference

		Public ReadOnly NameLocation As Microsoft.CodeAnalysis.Location

		Public Shadows ReadOnly Property Children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)
			Get
				Return Me.GetNamespaceOrTypeDeclarationChildren()
			End Get
		End Property

		Public ReadOnly Property Location As Microsoft.CodeAnalysis.Location
			Get
				Return Me.SyntaxReference.GetLocation()
			End Get
		End Property

		Protected Sub New(ByVal name As String, ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal nameLocation As Microsoft.CodeAnalysis.Location)
			MyBase.New(name)
			Me.SyntaxReference = syntaxReference
			Me.NameLocation = nameLocation
		End Sub

		Public Shared Function BestName(Of T As SingleNamespaceOrTypeDeclaration)(ByVal singleDeclarations As ImmutableArray(Of T), ByRef multipleSpellings As Boolean) As String
			multipleSpellings = False
			Dim name As String = singleDeclarations(0).Name
			Dim length As Integer = singleDeclarations.Length - 1
			Dim num As Integer = 1
			Do
				Dim str As String = singleDeclarations(num).Name
				Dim num1 As Integer = [String].Compare(name, str, StringComparison.Ordinal)
				If (num1 <> 0) Then
					multipleSpellings = True
					If (num1 > 0) Then
						name = str
					End If
				End If
				num = num + 1
			Loop While num <= length
			Return name
		End Function

		Public Shared Function BestName(Of T As SingleNamespaceOrTypeDeclaration)(ByVal singleDeclarations As ImmutableArray(Of T)) As String
			Dim flag As Boolean = False
			Return SingleNamespaceOrTypeDeclaration.BestName(Of T)(singleDeclarations, flag)
		End Function

		Protected Overrides Function GetDeclarationChildren() As ImmutableArray(Of Declaration)
			Return StaticCast(Of Declaration).From(Of SingleNamespaceOrTypeDeclaration)(Me.GetNamespaceOrTypeDeclarationChildren())
		End Function

		Protected MustOverride Function GetNamespaceOrTypeDeclarationChildren() As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)
	End Class
End Namespace