Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class PEOrSourceOrMergedNamespaceSymbol
		Inherits NamespaceSymbol
		Private _lazyExtensionMethodsMap As Dictionary(Of String, ImmutableArray(Of Symbol))

		Private _extQueryCnt As Integer

		Private ReadOnly Shared s_emptyDictionary As Dictionary(Of String, ImmutableArray(Of Symbol))

		Private _lazyDeclaredAccessibilityOfMostAccessibleDescendantType As Byte

		Friend NotOverridable Overrides ReadOnly Property DeclaredAccessibilityOfMostAccessibleDescendantType As Accessibility
			Get
				If (Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 1) Then
					Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = CByte(Me.GetDeclaredAccessibilityOfMostAccessibleDescendantType())
					If (Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 6) Then
						Dim containingSymbol As PEOrSourceOrMergedNamespaceSymbol = TryCast(Me.ContainingSymbol, PEOrSourceOrMergedNamespaceSymbol)
						While containingSymbol IsNot Nothing AndAlso containingSymbol._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 1
							containingSymbol._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 6
							containingSymbol = TryCast(containingSymbol.ContainingSymbol, PEOrSourceOrMergedNamespaceSymbol)
						End While
					End If
				End If
				Return DirectCast(Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType, Accessibility)
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind

		Friend ReadOnly Property RawLazyDeclaredAccessibilityOfMostAccessibleDescendantType As Accessibility
			Get
				Return DirectCast(Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType, Accessibility)
			End Get
		End Property

		Shared Sub New()
			PEOrSourceOrMergedNamespaceSymbol.s_emptyDictionary = New Dictionary(Of String, ImmutableArray(Of Symbol))()
		End Sub

		Protected Sub New()
			MyBase.New()
			Me._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 1
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamespaceSymbol)
			Me.EnsureExtensionMethodsAreCollected()
			appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me._lazyExtensionMethodsMap)
		End Sub

		Friend Overrides Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol))
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			If (Me._lazyExtensionMethodsMap Is Nothing) Then
				If (Interlocked.Increment(Me._extQueryCnt) <> 40) Then
					Me.GetExtensionMethods(methods, name)
					Return
				End If
				Me.EnsureExtensionMethodsAreCollected()
			End If
			If (Me._lazyExtensionMethodsMap.TryGetValue(name, symbols)) Then
				methods.AddRange(symbols.[As](Of MethodSymbol)())
			End If
		End Sub

		Private Sub EnsureExtensionMethodsAreCollected()
			Dim enumerator As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)).Enumerator = New Dictionary(Of String, ArrayBuilder(Of MethodSymbol)).Enumerator()
			If (Me._lazyExtensionMethodsMap Is Nothing) Then
				Dim strs As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)) = New Dictionary(Of String, ArrayBuilder(Of MethodSymbol))(CaseInsensitiveComparison.Comparer)
				Me.BuildExtensionMethodsMap(strs)
				If (strs.Count = 0) Then
					Me._lazyExtensionMethodsMap = PEOrSourceOrMergedNamespaceSymbol.s_emptyDictionary
					Return
				End If
				Dim strs1 As Dictionary(Of String, ImmutableArray(Of Symbol)) = New Dictionary(Of String, ImmutableArray(Of Symbol))(strs.Count, CaseInsensitiveComparison.Comparer)
				Try
					enumerator = strs.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, ArrayBuilder(Of MethodSymbol)) = enumerator.Current
						strs1.Add(current.Key, StaticCast(Of Symbol).From(Of MethodSymbol)(current.Value.ToImmutableAndFree()))
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Me._lazyExtensionMethodsMap = strs1
			End If
		End Sub
	End Class
End Namespace