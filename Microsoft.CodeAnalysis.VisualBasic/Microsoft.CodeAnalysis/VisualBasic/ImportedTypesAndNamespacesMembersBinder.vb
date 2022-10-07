Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ImportedTypesAndNamespacesMembersBinder
		Inherits Binder
		Private ReadOnly _importedSymbols As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)

		Friend Shared GenerateAmbiguityError As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic)

		Shared Sub New()
			ImportedTypesAndNamespacesMembersBinder.GenerateAmbiguityError = Function(ambiguousSymbols As ImmutableArray(Of Symbol))
				Dim containingSymbol As Func(Of Symbol, Symbol)
				Dim symbols As ImmutableArray(Of Symbol) = ambiguousSymbols
				Dim name() As [Object] = { ambiguousSymbols(0).Name, Nothing }
				Dim symbols1 As ImmutableArray(Of Symbol) = ambiguousSymbols
				If (ImportedTypesAndNamespacesMembersBinder._Closure$__.$I0-1 Is Nothing) Then
					containingSymbol = Function(sym As Symbol) sym.ContainingSymbol
					ImportedTypesAndNamespacesMembersBinder._Closure$__.$I0-1 = containingSymbol
				Else
					containingSymbol = ImportedTypesAndNamespacesMembersBinder._Closure$__.$I0-1
				End If
				name(1) = New FormattedSymbolList(symbols1.[Select](Of Symbol)(containingSymbol), Nothing)
				Return New AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInImports2, symbols, name)
			End Function
		End Sub

		Public Sub New(ByVal containingBinder As Binder, ByVal importedSymbols As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition))
			MyBase.New(containingBinder)
			Me._importedSymbols = importedSymbols
		End Sub

		Protected Overrides Sub AddExtensionMethodLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (current.NamespaceOrType.Kind <> SymbolKind.NamedType) Then
					Continue While
				End If
				DirectCast(current.NamespaceOrType, NamedTypeSymbol).AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder)
			End While
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				originalBinder.AddMemberLookupSymbolsInfo(nameSet, current.NamespaceOrType, options Or LookupOptions.IgnoreExtensionMethods)
			End While
		End Sub

		Protected Overrides Sub CollectProbableExtensionMethodsInSingleBinder(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				If (current.NamespaceOrType.Kind <> SymbolKind.NamedType) Then
					Continue While
				End If
				DirectCast(current.NamespaceOrType, NamedTypeSymbol).AppendProbableExtensionMethods(name, methods)
				If (methods.Count = 0 OrElse originalBinder.IsSemanticModelBinder) Then
					Continue While
				End If
				MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, current.ImportsClausePosition)
			End While
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim isAmbiguous As Boolean
			options = options Or LookupOptions.IgnoreExtensionMethods
			Dim enumerator As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = Me._importedSymbols.GetEnumerator()
			Do
				If (Not enumerator.MoveNext()) Then
					Exit Do
				End If
				Dim current As NamespaceOrTypeAndImportsClausePosition = enumerator.Current
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.LookupResult = Microsoft.CodeAnalysis.VisualBasic.LookupResult.GetInstance()
				If (Not current.NamespaceOrType.IsNamespace) Then
					originalBinder.LookupMember(instance, current.NamespaceOrType, name, arity, options, useSiteInfo)
				Else
					originalBinder.LookupMemberImmediate(instance, DirectCast(current.NamespaceOrType, NamespaceSymbol), name, arity, options, useSiteInfo)
				End If
				If (instance.IsGoodOrAmbiguous AndAlso Not originalBinder.IsSemanticModelBinder) Then
					MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, current.ImportsClausePosition)
				End If
				isAmbiguous = instance.IsAmbiguous
				If (isAmbiguous) Then
					lookupResult.SetFrom(instance)
				ElseIf (Not instance.IsGood OrElse Not instance.HasSingleSymbol OrElse instance.SingleSymbol.Kind <> SymbolKind.[Namespace] OrElse DirectCast(instance.SingleSymbol, NamespaceSymbol).ContainsTypesAccessibleFrom(MyBase.Compilation.Assembly)) Then
					If (Not lookupResult.StopFurtherLookup OrElse Not instance.StopFurtherLookup) Then
						lookupResult.MergeAmbiguous(instance, ImportedTypesAndNamespacesMembersBinder.GenerateAmbiguityError)
					Else
						Dim kind As Boolean = lookupResult.Symbols(0).Kind = SymbolKind.[Namespace]
						Dim flag As Boolean = instance.Symbols(0).Kind = SymbolKind.[Namespace]
						If (kind AndAlso Not flag) Then
							lookupResult.SetFrom(instance)
						ElseIf ((Not flag OrElse kind) AndAlso (lookupResult.Symbols.Count <> instance.Symbols.Count OrElse Not lookupResult.Symbols(0).Equals(instance.Symbols(0)))) Then
							If (Not kind OrElse Not instance.IsGood OrElse Not lookupResult.IsGood) Then
								lookupResult.MergeAmbiguous(instance, ImportedTypesAndNamespacesMembersBinder.GenerateAmbiguityError)
							Else
								lookupResult.Symbols.AddRange(instance.Symbols)
							End If
						End If
					End If
				End If
				instance.Free()
			Loop While Not isAmbiguous
			If (lookupResult.IsGood AndAlso lookupResult.Symbols.Count > 1 AndAlso lookupResult.Symbols(0).Kind = SymbolKind.[Namespace]) Then
				lookupResult.SetFrom(MergedNamespaceSymbol.CreateNamespaceGroup(lookupResult.Symbols.Cast(Of NamespaceSymbol)()))
			End If
		End Sub
	End Class
End Namespace