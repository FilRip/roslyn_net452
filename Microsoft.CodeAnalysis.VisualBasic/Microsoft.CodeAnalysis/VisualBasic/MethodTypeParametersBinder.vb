Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MethodTypeParametersBinder
		Inherits Binder
		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Public Sub New(ByVal containingBinder As Binder, ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol))
			MyBase.New(containingBinder)
			Me._typeParameters = typeParameters
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = Me._typeParameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterSymbol = enumerator.Current
				If (Not originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, Nothing)) Then
					Continue While
				End If
				nameSet.AddSymbol(current, current.Name, 0)
			End While
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim length As Integer = Me._typeParameters.Length - 1
			For i As Integer = 0 To length
				Dim item As TypeParameterSymbol = Me._typeParameters(i)
				If (CaseInsensitiveComparison.Equals(item.Name, name)) Then
					lookupResult.SetFrom(MyBase.CheckViability(item, arity, options, Nothing, useSiteInfo))
				End If
			Next

		End Sub
	End Class
End Namespace