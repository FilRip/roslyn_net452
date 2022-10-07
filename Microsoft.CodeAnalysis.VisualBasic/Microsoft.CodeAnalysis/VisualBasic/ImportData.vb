Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class ImportData
		Public ReadOnly Members As HashSet(Of NamespaceOrTypeSymbol)

		Public ReadOnly Aliases As Dictionary(Of String, AliasAndImportsClausePosition)

		Public ReadOnly XmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition)

		Protected Sub New(ByVal members As HashSet(Of NamespaceOrTypeSymbol), ByVal aliases As Dictionary(Of String, AliasAndImportsClausePosition), ByVal xmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition))
			MyBase.New()
			Me.Members = members
			Me.Aliases = aliases
			Me.XmlNamespaces = xmlNamespaces
		End Sub

		Public MustOverride Sub AddAlias(ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal [alias] As AliasSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))

		Public MustOverride Sub AddMember(ByVal syntaxRef As SyntaxReference, ByVal member As NamespaceOrTypeSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
	End Class
End Namespace