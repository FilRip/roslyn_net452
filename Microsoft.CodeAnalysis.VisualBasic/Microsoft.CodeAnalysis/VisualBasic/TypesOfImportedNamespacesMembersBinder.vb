Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class TypesOfImportedNamespacesMembersBinder
		Inherits Binder
		Private ReadOnly _importedSymbols As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)

		Public Sub New(ByVal containingBinder As Binder, ByVal importedSymbols As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition))
			MyBase.New(containingBinder)
			Me._importedSymbols = importedSymbols
		End Sub

		Protected Overrides Sub AddExtensionMethodLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (Not current.NamespaceOrType.IsNamespace) Then
					Continue While
				End If
				DirectCast(current.NamespaceOrType, NamespaceSymbol).AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder)
			End While
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			options = options Or LookupOptions.IgnoreExtensionMethods
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (Not current.NamespaceOrType.IsNamespace) Then
					Continue While
				End If
				Dim enumerator1 As ImmutableArray(Of NamedTypeSymbol).Enumerator = DirectCast(current.NamespaceOrType, NamespaceSymbol).GetModuleMembers().GetEnumerator()
				While enumerator1.MoveNext()
					originalBinder.AddMemberLookupSymbolsInfo(nameSet, enumerator1.Current, options)
				End While
			End While
		End Sub

		Protected Overrides Sub CollectProbableExtensionMethodsInSingleBinder(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (Not current.NamespaceOrType.IsNamespace) Then
					Continue While
				End If
				Dim count As Integer = methods.Count
				DirectCast(current.NamespaceOrType, NamespaceSymbol).AppendProbableExtensionMethods(name, methods)
				If (methods.Count = count OrElse originalBinder.IsSemanticModelBinder) Then
					Continue While
				End If
				MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, current.ImportsClausePosition)
			End While
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (Not current.NamespaceOrType.IsNamespace) Then
					Continue While
				End If
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.LookupResult = Microsoft.CodeAnalysis.VisualBasic.LookupResult.GetInstance()
				originalBinder.LookupMemberInModules(instance, DirectCast(current.NamespaceOrType, NamespaceSymbol), name, arity, options, useSiteInfo)
				If (instance.IsGood AndAlso Not originalBinder.IsSemanticModelBinder) Then
					MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, current.ImportsClausePosition)
				End If
				lookupResult.MergeAmbiguous(instance, ImportedTypesAndNamespacesMembersBinder.GenerateAmbiguityError)
				instance.Free()
			End While
		End Sub
	End Class
End Namespace