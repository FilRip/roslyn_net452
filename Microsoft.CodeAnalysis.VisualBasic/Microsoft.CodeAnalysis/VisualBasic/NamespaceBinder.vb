Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class NamespaceBinder
		Inherits Binder
		Private ReadOnly _nsSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return Me._nsSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingNamespaceOrType As NamespaceOrTypeSymbol
			Get
				Return Me._nsSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsInQuery As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property NamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Get
				Return Me._nsSymbol
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal nsSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			MyBase.New(containingBinder)
			Me._nsSymbol = nsSymbol
		End Sub

		Protected Overrides Sub AddExtensionMethodLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me._nsSymbol.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder)
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			originalBinder.AddMemberLookupSymbolsInfo(nameSet, Me._nsSymbol, options Or LookupOptions.IgnoreExtensionMethods)
		End Sub

		Protected Overrides Sub CollectProbableExtensionMethodsInSingleBinder(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal originalBinder As Binder)
			Me._nsSymbol.AppendProbableExtensionMethods(name, methods)
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			originalBinder.LookupMember(lookupResult, Me._nsSymbol, name, arity, options Or LookupOptions.IgnoreExtensionMethods, useSiteInfo)
		End Sub
	End Class
End Namespace