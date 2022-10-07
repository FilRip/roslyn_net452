Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend MustInherit Class PENamespaceSymbol
		Inherits PEOrSourceOrMergedNamespaceSymbol
		Protected m_lazyMembers As Dictionary(Of String, ImmutableArray(Of Symbol))

		Protected m_lazyTypes As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol))

		Private _lazyNoPiaLocalTypes As Dictionary(Of String, TypeDefinitionHandle)

		Private _lazyModules As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyFlattenedTypes As ImmutableArray(Of NamedTypeSymbol)

		Friend ReadOnly Property AreTypesLoaded As Boolean
			Get
				Return Me.m_lazyTypes IsNot Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property ContainingPEModule As PEModuleSymbol

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Extent As NamespaceExtent
			Get
				Return New NamespaceExtent(Me.ContainingPEModule)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of MetadataLocation)(Me.ContainingPEModule.MetadataLocation)
			End Get
		End Property

		Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
			Get
				Dim empty As ImmutableArray(Of NamedTypeSymbol)
				If (Not Me.ContainingPEModule.MightContainExtensionMethods) Then
					empty = ImmutableArray(Of NamedTypeSymbol).Empty
				Else
					empty = Me.GetTypeMembers()
				End If
				Return empty
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Protected MustOverride Sub EnsureAllMembersLoaded()

		Public NotOverridable Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Me.EnsureAllMembersLoaded()
			Return Me.m_lazyMembers.Flatten(Nothing)
		End Function

		Public NotOverridable Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Me.EnsureAllMembersLoaded()
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			If (Not Me.m_lazyMembers.TryGetValue(name, symbols)) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = symbols
			End If
			Return empty
		End Function

		Public Overrides Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)
			Dim typeKind As Func(Of NamedTypeSymbol, Boolean)
			If (Me._lazyModules.IsDefault) Then
				Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers()
				If (PENamespaceSymbol._Closure$__.$I8-0 Is Nothing) Then
					typeKind = Function(t As NamedTypeSymbol) t.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]
					PENamespaceSymbol._Closure$__.$I8-0 = typeKind
				Else
					typeKind = PENamespaceSymbol._Closure$__.$I8-0
				End If
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = typeMembers.WhereAsArray(typeKind)
				Dim namedTypeSymbols1 As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
				ImmutableInterlocked.InterlockedCompareExchange(Of NamedTypeSymbol)(Me._lazyModules, namedTypeSymbols, namedTypeSymbols1)
			End If
			Return Me._lazyModules
		End Function

		Public Overrides Function GetModuleMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim typeKind As Func(Of NamedTypeSymbol, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (PENamespaceSymbol._Closure$__.$I9-0 Is Nothing) Then
				typeKind = Function(t As NamedTypeSymbol) t.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]
				PENamespaceSymbol._Closure$__.$I9-0 = typeKind
			Else
				typeKind = PENamespaceSymbol._Closure$__.$I9-0
			End If
			Return typeMembers.WhereAsArray(typeKind)
		End Function

		Public NotOverridable Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols1 As ImmutableArray(Of NamedTypeSymbol) = Me._lazyFlattenedTypes
			If (namedTypeSymbols1.IsDefault) Then
				Me.EnsureAllMembersLoaded()
				namedTypeSymbols1 = StaticCast(Of NamedTypeSymbol).From(Of PENamedTypeSymbol)(Me.m_lazyTypes.Flatten(Nothing))
				Me._lazyFlattenedTypes = namedTypeSymbols1
				namedTypeSymbols = namedTypeSymbols1
			Else
				namedTypeSymbols = namedTypeSymbols1
			End If
			Return namedTypeSymbols
		End Function

		Public NotOverridable Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Me.EnsureAllMembersLoaded()
			Dim pENamedTypeSymbols As ImmutableArray(Of PENamedTypeSymbol) = New ImmutableArray(Of PENamedTypeSymbol)()
			If (Not Me.m_lazyTypes.TryGetValue(name, pENamedTypeSymbols)) Then
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			Else
				empty = StaticCast(Of NamedTypeSymbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols)
			End If
			Return empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, Integer, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (PENamespaceSymbol._Closure$__.$I16-0 Is Nothing) Then
				func = Function(type As NamedTypeSymbol, arity_ As Integer) type.Arity = arity_
				PENamespaceSymbol._Closure$__.$I16-0 = func
			Else
				func = PENamespaceSymbol._Closure$__.$I16-0
			End If
			Return typeMembers.WhereAsArray(Of Integer)(func, arity)
		End Function

		Private Sub LazyInitializeNamespaces(ByVal childNamespaces As IEnumerable(Of KeyValuePair(Of String, IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))))
			Dim enumerator As IEnumerator(Of KeyValuePair(Of String, IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))) = Nothing
			Dim enumerator1 As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator()
			If (Me.m_lazyMembers Is Nothing) Then
				Using strs As Dictionary(Of String, ImmutableArray(Of Symbol)) = New Dictionary(Of String, ImmutableArray(Of Symbol))(CaseInsensitiveComparison.Comparer)
					enumerator = childNamespaces.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle))) = enumerator.Current
						Dim pENestedNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENestedNamespaceSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENestedNamespaceSymbol(current.Key, Me, current.Value)
						strs.Add(pENestedNamespaceSymbol.Name, ImmutableArray.Create(Of Symbol)(pENestedNamespaceSymbol))
					End While
				End Using
				Try
					enumerator1 = Me.m_lazyTypes.Values.GetEnumerator()
					While enumerator1.MoveNext()
						Dim pENamedTypeSymbols As ImmutableArray(Of PENamedTypeSymbol) = enumerator1.Current
						Dim name As String = pENamedTypeSymbols(0).Name
						Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
						If (strs.TryGetValue(name, symbols)) Then
							strs(name) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Symbol)(symbols, StaticCast(Of Symbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols))
						Else
							strs.Add(name, StaticCast(Of Symbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols))
						End If
					End While
				Finally
					DirectCast(enumerator1, IDisposable).Dispose()
				End Try
				Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of Symbol)))(Me.m_lazyMembers, strs, Nothing)
			End If
		End Sub

		Private Sub LazyInitializeTypes(ByVal typeGroups As IEnumerable(Of IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle)))
			Dim enumerator As IEnumerator(Of IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle)) = Nothing
			Dim enumerator1 As IEnumerator(Of System.Reflection.Metadata.TypeDefinitionHandle) = Nothing
			Dim pENamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol
			Dim name As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol, String)
			If (Me.m_lazyTypes Is Nothing) Then
				Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol).GetInstance()
				Dim flag As Boolean = Not containingPEModule.[Module].ContainsNoPiaLocalTypes()
				Dim strs As Dictionary(Of String, System.Reflection.Metadata.TypeDefinitionHandle) = Nothing
				Using isGlobalNamespace As Boolean = Me.IsGlobalNamespace
					enumerator = typeGroups.GetEnumerator()
					While enumerator.MoveNext()
						Using current As IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle) = enumerator.Current
							enumerator1 = current.GetEnumerator()
							While enumerator1.MoveNext()
								Dim typeDefinitionHandle As System.Reflection.Metadata.TypeDefinitionHandle = enumerator1.Current
								If (flag OrElse Not containingPEModule.[Module].IsNoPiaLocalType(typeDefinitionHandle)) Then
									If (isGlobalNamespace) Then
										pENamedTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol(containingPEModule, Me, typeDefinitionHandle)
									Else
										pENamedTypeSymbol = New PENamedTypeSymbolWithEmittedNamespaceName(containingPEModule, Me, typeDefinitionHandle, current.Key)
									End If
									instance.Add(pENamedTypeSymbol)
								Else
									Try
										Dim typeDefNameOrThrow As String = containingPEModule.[Module].GetTypeDefNameOrThrow(typeDefinitionHandle)
										If (strs Is Nothing) Then
											strs = New Dictionary(Of String, System.Reflection.Metadata.TypeDefinitionHandle)()
										End If
										Dim str As String = MetadataHelpers.BuildQualifiedName(current.Key, typeDefNameOrThrow)
										strs(str) = typeDefinitionHandle
									Catch badImageFormatException As System.BadImageFormatException
										ProjectData.SetProjectError(badImageFormatException)
										ProjectData.ClearProjectError()
									End Try
								End If
							End While
						End Using
					End While
				End Using
				Dim pENamedTypeSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol) = instance
				If (PENamespaceSymbol._Closure$__.$I26-0 Is Nothing) Then
					name = Function(c As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol) c.Name
					PENamespaceSymbol._Closure$__.$I26-0 = name
				Else
					name = PENamespaceSymbol._Closure$__.$I26-0
				End If
				Dim dictionary As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol)) = pENamedTypeSymbols.ToDictionary(Of String)(name, CaseInsensitiveComparison.Comparer)
				instance.Free()
				If (Me._lazyNoPiaLocalTypes Is Nothing) Then
					Interlocked.CompareExchange(Of Dictionary(Of String, System.Reflection.Metadata.TypeDefinitionHandle))(Me._lazyNoPiaLocalTypes, strs, Nothing)
				End If
				If (Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol)))(Me.m_lazyTypes, dictionary, Nothing) Is Nothing) Then
					containingPEModule.OnNewTypeDeclarationsLoaded(dictionary)
				End If
			End If
		End Sub

		Protected Sub LoadAllMembers(ByVal typesByNS As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))
			Dim groupings As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)) = Nothing
			Dim keyValuePairs As IEnumerable(Of KeyValuePair(Of String, IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))) = Nothing
			Dim isGlobalNamespace As Boolean = Me.IsGlobalNamespace
			MetadataHelpers.GetInfoForImmediateNamespaceMembers(isGlobalNamespace, If(isGlobalNamespace, 0, MyBase.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat).Length), typesByNS, CaseInsensitiveComparison.Comparer, groupings, keyValuePairs)
			Me.LazyInitializeTypes(groupings)
			Me.LazyInitializeNamespaces(keyValuePairs)
		End Sub

		Friend Function LookupMetadataType(ByRef emittedTypeName As MetadataTypeName, <Out> ByRef isNoPiaLocalType As Boolean) As NamedTypeSymbol
			Dim typeOfToken As NamedTypeSymbol = Me.LookupMetadataType(emittedTypeName)
			isNoPiaLocalType = False
			If (TypeOf typeOfToken Is MissingMetadataTypeSymbol) Then
				Me.EnsureAllMembersLoaded()
				Dim typeDefinitionHandle As System.Reflection.Metadata.TypeDefinitionHandle = New System.Reflection.Metadata.TypeDefinitionHandle()
				If (Me._lazyNoPiaLocalTypes IsNot Nothing AndAlso Me._lazyNoPiaLocalTypes.TryGetValue(emittedTypeName.FullName, typeDefinitionHandle)) Then
					typeOfToken = DirectCast((New MetadataDecoder(Me.ContainingPEModule)).GetTypeOfToken(typeDefinitionHandle, isNoPiaLocalType), NamedTypeSymbol)
				End If
			End If
			Return typeOfToken
		End Function
	End Class
End Namespace