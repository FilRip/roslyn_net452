Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class NamedTypeBinder
		Inherits Binder
		Private ReadOnly _typeSymbol As NamedTypeSymbol

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return Me._typeSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingNamespaceOrType As NamespaceOrTypeSymbol
			Get
				Return Me._typeSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._typeSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsInQuery As Boolean
			Get
				Return False
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal typeSymbol As NamedTypeSymbol)
			MyBase.New(containingBinder)
			Me._typeSymbol = typeSymbol
		End Sub

		Protected Overrides Sub AddExtensionMethodLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me._typeSymbol.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder)
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			If (Me._typeSymbol.Arity > 0) Then
				Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = Me._typeSymbol.TypeParameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeParameterSymbol = enumerator.Current
					If (Not originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, Nothing)) Then
						Continue While
					End If
					nameSet.AddSymbol(current, current.Name, 0)
				End While
			End If
			originalBinder.AddMemberLookupSymbolsInfo(nameSet, Me._typeSymbol, options)
		End Sub

		Public Overrides Function CheckAccessibility(ByVal sym As Symbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal accessThroughType As TypeSymbol = Nothing, Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			If (MyBase.IgnoresAccessibility) Then
				Return AccessCheckResult.Accessible
			End If
			Return AccessCheck.CheckSymbolAccessibility(sym, Me._typeSymbol, accessThroughType, useSiteInfo, basesBeingResolved)
		End Function

		Protected Overrides Sub CollectProbableExtensionMethodsInSingleBinder(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal originalBinder As Binder)
			Me._typeSymbol.AppendProbableExtensionMethods(name, methods)
		End Sub

		Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Binder
			If (Me._typeSymbol.IsScriptClass) Then
				Return Me
			End If
			Return Me.m_containingBinder.GetBinder(node)
		End Function

		Public Overrides Function GetBinder(ByVal stmtList As SyntaxList(Of StatementSyntax)) As Binder
			If (Me._typeSymbol.IsScriptClass) Then
				Return Me
			End If
			Return Me.m_containingBinder.GetBinder(stmtList)
		End Function

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			originalBinder.LookupMember(lookupResult, Me._typeSymbol, name, arity, options, useSiteInfo)
			If (Not lookupResult.StopFurtherLookup) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.LookupResult = Microsoft.CodeAnalysis.VisualBasic.LookupResult.GetInstance()
				Me.LookupTypeParameter(instance, name, arity, options, originalBinder, useSiteInfo)
				lookupResult.MergePrioritized(instance)
				instance.Free()
			End If
		End Sub

		Private Sub LookupTypeParameter(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (Me._typeSymbol.Arity > 0) Then
				Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = Me._typeSymbol.TypeParameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeParameterSymbol = enumerator.Current
					If (Not CaseInsensitiveComparison.Equals(current.Name, name)) Then
						Continue While
					End If
					lookupResult.SetFrom(originalBinder.CheckViability(current, arity, options, Nothing, useSiteInfo))
				End While
			End If
		End Sub
	End Class
End Namespace