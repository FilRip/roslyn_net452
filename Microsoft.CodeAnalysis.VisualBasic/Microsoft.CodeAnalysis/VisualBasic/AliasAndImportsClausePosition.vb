Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure AliasAndImportsClausePosition
		Public ReadOnly [Alias] As AliasSymbol

		Public ReadOnly ImportsClausePosition As Integer

		Public ReadOnly Dependencies As ImmutableArray(Of AssemblySymbol)

		Public Sub New(ByVal [alias] As AliasSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As ImmutableArray(Of AssemblySymbol))
			Me = New AliasAndImportsClausePosition() With
			{
				.[Alias] = [alias],
				.ImportsClausePosition = importsClausePosition,
				.Dependencies = dependencies
			}
		End Sub
	End Structure
End Namespace