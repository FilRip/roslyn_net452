Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ImportAliasesBinder
		Inherits Binder
		Private ReadOnly _importedAliases As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition)

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return MyBase.Compilation.SourceModule
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal importedAliases As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition))
			MyBase.New(containingBinder)
			Me._importedAliases = importedAliases
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Dim enumerator As IEnumerator(Of AliasAndImportsClausePosition) = Nothing
			Try
				enumerator = Me._importedAliases.Values.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AliasAndImportsClausePosition = enumerator.Current
					Dim target As NamespaceOrTypeSymbol = current.[Alias].Target
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					If (Not originalBinder.CheckViability(target, -1, options, Nothing, discarded).IsGoodOrAmbiguous) Then
						Continue While
					End If
					nameSet.AddSymbol(current.[Alias], current.[Alias].Name, 0)
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim aliasAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition()
			If (Me._importedAliases.TryGetValue(name, aliasAndImportsClausePosition)) Then
				Dim singleLookupResult As Microsoft.CodeAnalysis.VisualBasic.SingleLookupResult = MyBase.CheckViability(aliasAndImportsClausePosition.[Alias], arity, options, Nothing, useSiteInfo)
				If (singleLookupResult.IsGoodOrAmbiguous AndAlso Not originalBinder.IsSemanticModelBinder) Then
					MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, aliasAndImportsClausePosition.ImportsClausePosition)
				End If
				lookupResult.SetFrom(singleLookupResult)
			End If
		End Sub
	End Class
End Namespace