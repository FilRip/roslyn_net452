Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module OverloadingHelper
		Private Sub FindOverloads(ByVal name As String, ByVal kind As SymbolKind, ByVal container As NamedTypeSymbol, ByVal overloadsMembers As ArrayBuilder(Of Symbol), ByRef hasOverloadSpecifier As Boolean, ByRef hasOverrideSpecifier As Boolean)
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = container.GetMembers(name).GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (Not OverloadingHelper.IsCandidateMember(current, kind)) Then
					Continue While
				End If
				overloadsMembers.Add(current)
				If (Not current.IsOverrides) Then
					If (Not current.IsOverloads()) Then
						Continue While
					End If
					hasOverloadSpecifier = True
				Else
					hasOverrideSpecifier = True
				End If
			End While
		End Sub

		Private Function GetBaseMemberMetadataName(ByVal name As String, ByVal kind As SymbolKind, ByVal container As NamedTypeSymbol) As String
			Dim current As Symbol
			Dim metadataName As String = Nothing
			Dim containingModule As SourceModuleSymbol = DirectCast(container.ContainingModule, SourceModuleSymbol)
			Dim locations As ImmutableArray(Of Location) = container.Locations
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(containingModule, locations(0).PossiblyEmbeddedOrMySourceTree(), container)
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			binder.LookupMember(instance, container, name, 0, LookupOptions.AllMethodsOfAnyArity Or LookupOptions.IgnoreExtensionMethods, discarded)
			If (instance.IsGoodOrAmbiguous) Then
				Dim symbols As ArrayBuilder(Of Symbol) = instance.Symbols
				If (instance.Kind = LookupResultKind.Ambiguous AndAlso instance.HasDiagnostic AndAlso TypeOf instance.Diagnostic Is AmbiguousSymbolDiagnostic) Then
					symbols.AddRange(DirectCast(instance.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
				End If
				Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = symbols.GetEnumerator()
				Do
				Label1:
					If (Not enumerator.MoveNext()) Then
						instance.Free()
						Return metadataName
					End If
					current = enumerator.Current
					If (OverloadingHelper.IsCandidateMember(current, kind) AndAlso CObj(current.ContainingType) <> CObj(container)) Then
						If (metadataName IsNot Nothing) Then
							Continue Do
						End If
						metadataName = current.MetadataName
						GoTo Label1
					Else
						GoTo Label1
					End If
				Loop While [String].Equals(metadataName, current.MetadataName, StringComparison.Ordinal)
				metadataName = Nothing
			End If
			instance.Free()
			Return metadataName
		End Function

		Private Function IsCandidateMember(ByVal member As Symbol, ByVal kind As SymbolKind) As Boolean
			If (member.Kind <> kind) Then
				Return False
			End If
			Return Not member.IsAccessor()
		End Function

		Private Function NameOfFirstMember(ByVal overloadedMembers As ArrayBuilder(Of Symbol), ByVal compilation As VisualBasicCompilation) As String
			Dim name As String = Nothing
			Dim location As Microsoft.CodeAnalysis.Location = Nothing
			Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = overloadedMembers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				Dim item As Microsoft.CodeAnalysis.Location = current.Locations(0)
				If (name IsNot Nothing AndAlso compilation.CompareSourceLocations(item, location) >= 0) Then
					Continue While
				End If
				name = current.Name
				location = item
			End While
			Return name
		End Function

		Public Sub SetMetadataNameForAllOverloads(ByVal name As String, ByVal kind As SymbolKind, ByVal container As NamedTypeSymbol)
			Dim declaringCompilation As VisualBasicCompilation = container.DeclaringCompilation
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim baseMemberMetadataName As String = Nothing
			Try
				OverloadingHelper.FindOverloads(name, kind, container, instance, flag, flag1)
				If (instance.Count <> 1 OrElse flag OrElse flag1) Then
					If (flag1) Then
						baseMemberMetadataName = OverloadingHelper.SetMetadataNamesOfOverrides(instance, declaringCompilation)
					ElseIf (flag) Then
						baseMemberMetadataName = OverloadingHelper.GetBaseMemberMetadataName(name, kind, container)
					End If
					If (baseMemberMetadataName Is Nothing) Then
						baseMemberMetadataName = OverloadingHelper.NameOfFirstMember(instance, declaringCompilation)
					End If
					Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = instance.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Symbol = enumerator.Current
						If (current.IsOverrides AndAlso current.OverriddenMember() IsNot Nothing) Then
							Continue While
						End If
						current.SetMetadataName(baseMemberMetadataName)
					End While
				Else
					instance(0).SetMetadataName(instance(0).Name)
				End If
			Finally
				instance.Free()
			End Try
		End Sub

		Private Function SetMetadataNamesOfOverrides(ByVal overloadedMembers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal compilation As VisualBasicCompilation) As String
			Dim item As Location = Nothing
			Dim str As String = Nothing
			Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = overloadedMembers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
				If (Not current.IsOverrides) Then
					Continue While
				End If
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = current.OverriddenMember()
				If (symbol Is Nothing) Then
					Continue While
				End If
				Dim metadataName As String = symbol.MetadataName
				current.SetMetadataName(metadataName)
				If (str IsNot Nothing AndAlso compilation.CompareSourceLocations(current.Locations(0), item) >= 0) Then
					Continue While
				End If
				str = metadataName
				item = current.Locations(0)
			End While
			Return str
		End Function
	End Module
End Namespace