Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure NamespaceOrTypeAndImportsClausePosition
		Public ReadOnly NamespaceOrType As NamespaceOrTypeSymbol

		Public ReadOnly ImportsClausePosition As Integer

		Public ReadOnly Dependencies As ImmutableArray(Of AssemblySymbol)

		Public Sub New(ByVal namespaceOrType As NamespaceOrTypeSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As ImmutableArray(Of AssemblySymbol))
			Me = New NamespaceOrTypeAndImportsClausePosition() With
			{
				.NamespaceOrType = namespaceOrType,
				.ImportsClausePosition = importsClausePosition,
				.Dependencies = dependencies
			}
		End Sub
	End Structure
End Namespace