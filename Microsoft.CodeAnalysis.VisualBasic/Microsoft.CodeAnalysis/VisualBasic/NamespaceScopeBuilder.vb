Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class NamespaceScopeBuilder
		Private Sub New()
			MyBase.New()
		End Sub

		Public Shared Function BuildNamespaceScope(ByVal moduleBuilder As PEModuleBuilder, ByVal xmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition), ByVal aliasImportsOpt As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition), ByVal memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), ByVal diagnostics As DiagnosticBag) As ImmutableArray(Of UsedNamespaceOrType)
			Dim enumerator As IEnumerator(Of KeyValuePair(Of String, XmlNamespaceAndImportsClausePosition)) = Nothing
			Dim enumerator1 As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition) = Nothing
			Dim instance As ArrayBuilder(Of UsedNamespaceOrType) = ArrayBuilder(Of UsedNamespaceOrType).GetInstance()
			If (xmlNamespacesOpt IsNot Nothing) Then
				Try
					enumerator = xmlNamespacesOpt.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, XmlNamespaceAndImportsClausePosition) = enumerator.Current
						instance.Add(UsedNamespaceOrType.CreateXmlNamespace(current.Key, current.Value.XmlNamespace))
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
			If (aliasImportsOpt IsNot Nothing) Then
				Try
					enumerator1 = aliasImportsOpt.GetEnumerator()
					While enumerator1.MoveNext()
						Dim aliasAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition = enumerator1.Current
						Dim target As NamespaceOrTypeSymbol = aliasAndImportsClausePosition.[Alias].Target
						If (Not target.IsNamespace) Then
							If (target.Kind = SymbolKind.ErrorType OrElse target.ContainingAssembly.IsLinked) Then
								Continue While
							End If
							Dim typeReference As ITypeReference = NamespaceScopeBuilder.GetTypeReference(DirectCast(target, NamedTypeSymbol), moduleBuilder, diagnostics)
							instance.Add(UsedNamespaceOrType.CreateType(typeReference, aliasAndImportsClausePosition.[Alias].Name))
						Else
							instance.Add(UsedNamespaceOrType.CreateNamespace(DirectCast(target, NamespaceSymbol).GetCciAdapter(), Nothing, aliasAndImportsClausePosition.[Alias].Name))
						End If
					End While
				Finally
					If (enumerator1 IsNot Nothing) Then
						enumerator1.Dispose()
					End If
				End Try
			End If
			Dim enumerator2 As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition).Enumerator = memberImports.GetEnumerator()
			While enumerator2.MoveNext()
				Dim namespaceOrType As NamespaceOrTypeSymbol = enumerator2.Current.NamespaceOrType
				If (Not namespaceOrType.IsNamespace) Then
					If (namespaceOrType.ContainingAssembly.IsLinked) Then
						Continue While
					End If
					Dim typeReference1 As ITypeReference = NamespaceScopeBuilder.GetTypeReference(DirectCast(namespaceOrType, NamedTypeSymbol), moduleBuilder, diagnostics)
					instance.Add(UsedNamespaceOrType.CreateType(typeReference1, Nothing))
				Else
					instance.Add(UsedNamespaceOrType.CreateNamespace(DirectCast(namespaceOrType, NamespaceSymbol).GetCciAdapter(), Nothing, Nothing))
				End If
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Shared Function GetTypeReference(ByVal type As TypeSymbol, ByVal moduleBuilder As CommonPEModuleBuilder, ByVal diagnostics As DiagnosticBag) As ITypeReference
			Return moduleBuilder.Translate(type, Nothing, diagnostics)
		End Function
	End Class
End Namespace