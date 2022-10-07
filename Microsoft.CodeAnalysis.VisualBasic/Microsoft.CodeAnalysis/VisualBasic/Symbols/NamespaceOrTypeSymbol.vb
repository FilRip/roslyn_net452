Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class NamespaceOrTypeSymbol
		Inherits Symbol
		Implements INamespaceOrTypeSymbol, INamespaceOrTypeSymbolInternal
		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property IsNamespace As Boolean Implements INamespaceOrTypeSymbol.IsNamespace
			Get
				Return Me.Kind = SymbolKind.[Namespace]
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property IsType As Boolean Implements INamespaceOrTypeSymbol.IsType
			Get
				Return Not Me.IsNamespace
			End Get
		End Property

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Function AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal membersByName As IEnumerable(Of KeyValuePair(Of String, ImmutableArray(Of Symbol)))) As Boolean
			Dim enumerator As IEnumerator(Of KeyValuePair(Of String, ImmutableArray(Of Symbol))) = Nothing
			Using flag As Boolean = False
				enumerator = membersByName.GetEnumerator()
			Label0:
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.Value.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As Symbol = enumerator1.Current
						If (current.Kind <> SymbolKind.Method) Then
							Continue While
						End If
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (Not methodSymbol.MayBeReducibleExtensionMethod) Then
							Continue While
						End If
						flag = True
						If (Not Me.AddExtensionMethodLookupSymbolsInfoViabilityCheck(methodSymbol, options, nameSet, originalBinder)) Then
							Continue While
						End If
						nameSet.AddSymbol(current, current.Name, current.GetArity())
						GoTo Label0
					End While
				End While
			End Using
			Return flag
		End Function

		Friend Overridable Function AddExtensionMethodLookupSymbolsInfoViabilityCheck(ByVal method As MethodSymbol, ByVal options As LookupOptions, ByVal nameSet As LookupSymbolsInfo, ByVal originalBinder As Binder) As Boolean
			Return originalBinder.CanAddLookupSymbolInfo(method, options, nameSet, method.ContainingType)
		End Function

		Public MustOverride Function GetMembers() As ImmutableArray(Of Symbol)

		Public MustOverride Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)

		Friend Overridable Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ConditionallyDeOrder(Of Symbol)(Me.GetMembers())
		End Function

		Friend Function GetNamespaceOrTypeByQualifiedName(ByVal qualifiedName As IEnumerable(Of String)) As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)
			Dim namespaceOrTypeSymbols As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)
			Dim enumerator As IEnumerator(Of String) = Nothing
			Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me
			Using namespaceOrTypeSymbols1 As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol) = Nothing
				enumerator = qualifiedName.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As String = enumerator.Current
					If (namespaceOrTypeSymbols1 IsNot Nothing) Then
						namespaceOrTypeSymbol = namespaceOrTypeSymbols1.OfMinimalArity()
						If (namespaceOrTypeSymbol Is Nothing) Then
							namespaceOrTypeSymbols = SpecializedCollections.EmptyEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)()
							Return namespaceOrTypeSymbols
						End If
					End If
					namespaceOrTypeSymbols1 = namespaceOrTypeSymbol.GetMembers(current).OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)()
				End While
			End Using
			namespaceOrTypeSymbols = namespaceOrTypeSymbols1
			Return namespaceOrTypeSymbols
		End Function

		Public MustOverride Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)

		Public MustOverride Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)

		Public Overridable Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, Integer, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (NamespaceOrTypeSymbol._Closure$__.$I10-0 Is Nothing) Then
				func = Function(type As NamedTypeSymbol, arity_ As Integer) type.Arity = arity_
				NamespaceOrTypeSymbol._Closure$__.$I10-0 = func
			Else
				func = NamespaceOrTypeSymbol._Closure$__.$I10-0
			End If
			Return typeMembers.WhereAsArray(Of Integer)(func, arity)
		End Function

		Friend Overridable Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ConditionallyDeOrder(Of NamedTypeSymbol)(Me.GetTypeMembers())
		End Function

		Private Function ExplicitINamespaceOrTypeSymbol_GetMembers() As ImmutableArray(Of ISymbol) Implements INamespaceOrTypeSymbol.GetMembers
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.GetMembers())
		End Function

		Private Function ExplicitINamespaceOrTypeSymbol_GetMembers(ByVal name As String) As ImmutableArray(Of ISymbol) Implements INamespaceOrTypeSymbol.GetMembers
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.GetMembers(name))
		End Function

		Private Function ExplicitINamespaceOrTypeSymbol_GetTypeMembers() As ImmutableArray(Of INamedTypeSymbol) Implements INamespaceOrTypeSymbol.GetTypeMembers
			Return StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(Me.GetTypeMembers())
		End Function

		Private Function ExplicitINamespaceOrTypeSymbol_GetTypeMembers(ByVal name As String) As ImmutableArray(Of INamedTypeSymbol) Implements INamespaceOrTypeSymbol.GetTypeMembers
			Return StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(Me.GetTypeMembers(name))
		End Function

		Public Function ExplicitINamespaceOrTypeSymbol_GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of INamedTypeSymbol) Implements INamespaceOrTypeSymbol.GetTypeMembers
			Return StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(Me.GetTypeMembers(name, arity))
		End Function
	End Class
End Namespace