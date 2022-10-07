Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class DeclarationTable
		Public ReadOnly Shared Empty As DeclarationTable

		Private ReadOnly _allOlderRootDeclarations As ImmutableSetWithInsertionOrder(Of DeclarationTableEntry)

		Private ReadOnly _latestLazyRootDeclaration As DeclarationTableEntry

		Private ReadOnly _cache As DeclarationTable.Cache

		Private _mergedRoot As MergedNamespaceDeclaration

		Private ReadOnly _typeNames As Lazy(Of ICollection(Of String))

		Private ReadOnly _namespaceNames As Lazy(Of ICollection(Of String))

		Private ReadOnly _referenceDirectives As Lazy(Of ICollection(Of ReferenceDirective))

		Private _lazyAllRootDeclarations As ImmutableArray(Of RootSingleNamespaceDeclaration)

		Private ReadOnly Shared s_isNamespacePredicate As Predicate(Of Declaration)

		Private ReadOnly Shared s_isTypePredicate As Predicate(Of Declaration)

		Public ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return Me._namespaceNames.Value
			End Get
		End Property

		Public ReadOnly Property ReferenceDirectives As ICollection(Of ReferenceDirective)
			Get
				Return Me._referenceDirectives.Value
			End Get
		End Property

		Public ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return Me._typeNames.Value
			End Get
		End Property

		Shared Sub New()
			DeclarationTable.Empty = New DeclarationTable(ImmutableSetWithInsertionOrder(Of DeclarationTableEntry).Empty, Nothing, Nothing)
			DeclarationTable.s_isNamespacePredicate = Function(d As Declaration) d.Kind = DeclarationKind.[Namespace]
			DeclarationTable.s_isTypePredicate = Function(d As Declaration) d.Kind <> DeclarationKind.[Namespace]
		End Sub

		Private Sub New(ByVal allOlderRootDeclarations As ImmutableSetWithInsertionOrder(Of DeclarationTableEntry), ByVal latestLazyRootDeclaration As DeclarationTableEntry, ByVal cache As DeclarationTable.Cache)
			MyBase.New()
			Me._allOlderRootDeclarations = allOlderRootDeclarations
			Me._latestLazyRootDeclaration = latestLazyRootDeclaration
			Me._cache = If(cache, New DeclarationTable.Cache(Me))
			Me._typeNames = New Lazy(Of ICollection(Of String))(New Func(Of ICollection(Of String))(AddressOf Me.GetMergedTypeNames))
			Me._namespaceNames = New Lazy(Of ICollection(Of String))(New Func(Of ICollection(Of String))(AddressOf Me.GetMergedNamespaceNames))
			Me._referenceDirectives = New Lazy(Of ICollection(Of ReferenceDirective))(New Func(Of ICollection(Of ReferenceDirective))(AddressOf Me.GetMergedReferenceDirectives))
		End Sub

		Public Function AddRootDeclaration(ByVal lazyRootDeclaration As DeclarationTableEntry) As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable
			Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable
			declarationTable = If(Me._latestLazyRootDeclaration IsNot Nothing, New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable(Me._allOlderRootDeclarations.Add(Me._latestLazyRootDeclaration), lazyRootDeclaration, Nothing), New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable(Me._allOlderRootDeclarations, lazyRootDeclaration, Me._cache))
			Return declarationTable
		End Function

		Public Function AllRootNamespaces() As ImmutableArray(Of RootSingleNamespaceDeclaration)
			If (Me._lazyAllRootDeclarations.IsDefault) Then
				Dim instance As ArrayBuilder(Of RootSingleNamespaceDeclaration) = ArrayBuilder(Of RootSingleNamespaceDeclaration).GetInstance()
				Me.GetOlderNamespaces(instance)
				Dim latestRootDeclarationIfAny As RootSingleNamespaceDeclaration = Me.GetLatestRootDeclarationIfAny(True)
				If (latestRootDeclarationIfAny IsNot Nothing) Then
					instance.Add(latestRootDeclarationIfAny)
				End If
				ImmutableInterlocked.InterlockedInitialize(Of RootSingleNamespaceDeclaration)(Me._lazyAllRootDeclarations, instance.ToImmutableAndFree())
			End If
			Return Me._lazyAllRootDeclarations
		End Function

		Friend Function CalculateMergedRoot(ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration
			Dim mergedNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration
			Dim value As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration = Me._cache.MergedRoot.Value
			Dim latestRootDeclarationIfAny As RootSingleNamespaceDeclaration = Me.GetLatestRootDeclarationIfAny(True)
			If (latestRootDeclarationIfAny Is Nothing) Then
				mergedNamespaceDeclaration = value
			ElseIf (value IsNot Nothing) Then
				Dim declarations As ImmutableArray(Of SingleNamespaceDeclaration) = value.Declarations
				Dim instance As ArrayBuilder(Of SingleNamespaceDeclaration) = ArrayBuilder(Of SingleNamespaceDeclaration).GetInstance(declarations.Length + 1)
				instance.AddRange(declarations)
				instance.Add(Me._latestLazyRootDeclaration.Root.Value)
				If (compilation IsNot Nothing) Then
					instance.Sort(New DeclarationTable.RootNamespaceLocationComparer(compilation))
				End If
				mergedNamespaceDeclaration = Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration.Create(DirectCast(instance.ToImmutableAndFree(), IEnumerable(Of SingleNamespaceDeclaration)))
			Else
				mergedNamespaceDeclaration = Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration.Create(New SingleNamespaceDeclaration() { latestRootDeclarationIfAny })
			End If
			Return mergedNamespaceDeclaration
		End Function

		Public Function Contains(ByVal rootDeclaration As DeclarationTableEntry) As Boolean
			If (rootDeclaration Is Nothing) Then
				Return False
			End If
			If (Me._allOlderRootDeclarations.Contains(rootDeclaration)) Then
				Return True
			End If
			Return Me._latestLazyRootDeclaration = rootDeclaration
		End Function

		Public Shared Function ContainsName(ByVal mergedRoot As MergedNamespaceDeclaration, ByVal name As String, ByVal filter As SymbolFilter, ByVal cancellationToken As System.Threading.CancellationToken) As Boolean
			Return DeclarationTable.ContainsNameHelper(mergedRoot, Function(n As String) CaseInsensitiveComparison.Equals(n, name), filter, Function(t As SingleTypeDeclaration) t.MemberNames.Contains(name), cancellationToken)
		End Function

		Public Shared Function ContainsName(ByVal mergedRoot As MergedNamespaceDeclaration, ByVal predicate As Func(Of String, Boolean), ByVal filter As SymbolFilter, ByVal cancellationToken As System.Threading.CancellationToken) As Boolean
			Dim variable As DeclarationTable._Closure$__38-0 = Nothing
			variable = New DeclarationTable._Closure$__38-0(variable) With
			{
				.$VB$Local_predicate = predicate
			}
			Return DeclarationTable.ContainsNameHelper(mergedRoot, variable.$VB$Local_predicate, filter, Function(t As SingleTypeDeclaration)
				Dim flag As Boolean
				Dim enumerator As ImmutableHashSet(Of String).Enumerator = New ImmutableHashSet(Of String).Enumerator()
				Try
					enumerator = t.MemberNames.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As String = enumerator.Current
						If (Not Me.$VB$Local_predicate(current)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				flag = False
				Return flag
			End Function, cancellationToken)
		End Function

		Public Shared Function ContainsNameHelper(ByVal mergedRoot As MergedNamespaceDeclaration, ByVal predicate As Func(Of String, Boolean), ByVal filter As SymbolFilter, ByVal typePredicate As Func(Of SingleTypeDeclaration, Boolean), ByVal cancellationToken As System.Threading.CancellationToken) As Boolean
			Dim flag As Boolean
			Dim current As SingleTypeDeclaration
			Dim flag1 As Boolean = (filter And SymbolFilter.[Namespace]) = SymbolFilter.[Namespace]
			Dim flag2 As Boolean = (filter And SymbolFilter.Type) = SymbolFilter.Type
			Dim flag3 As Boolean = (filter And SymbolFilter.Member) = SymbolFilter.Member
			Dim mergedNamespaceOrTypeDeclarations As Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration) = New Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration)()
			mergedNamespaceOrTypeDeclarations.Push(mergedRoot)
			While True
				If (mergedNamespaceOrTypeDeclarations.Count > 0) Then
					cancellationToken.ThrowIfCancellationRequested()
					Dim mergedNamespaceOrTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration = mergedNamespaceOrTypeDeclarations.Pop()
					If (mergedNamespaceOrTypeDeclaration IsNot Nothing) Then
						If (mergedNamespaceOrTypeDeclaration.Kind = DeclarationKind.[Namespace]) Then
							If (flag1 AndAlso predicate(mergedNamespaceOrTypeDeclaration.Name)) Then
								flag = True
								Exit While
							End If
						ElseIf (flag2 AndAlso predicate(mergedNamespaceOrTypeDeclaration.Name)) Then
							flag = True
							Exit While
						ElseIf (flag3) Then
							Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = DirectCast(mergedNamespaceOrTypeDeclaration, MergedTypeDeclaration).Declarations.GetEnumerator()
							Do
								If (Not enumerator.MoveNext()) Then
									GoTo Label0
								End If
								current = enumerator.Current
							Loop While Not typePredicate(current)
							flag = True
							Exit While
						End If
					Label0:
						Dim enumerator1 As ImmutableArray(Of Declaration).Enumerator = mergedNamespaceOrTypeDeclaration.Children.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration = DirectCast(enumerator1.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration)
							If (Not flag3 AndAlso Not flag2 AndAlso current1.Kind <> DeclarationKind.[Namespace]) Then
								Continue While
							End If
							mergedNamespaceOrTypeDeclarations.Push(current1)
						End While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function GetLatestRootDeclarationIfAny(ByVal includeEmbedded As Boolean) As RootSingleNamespaceDeclaration
			If (Me._latestLazyRootDeclaration Is Nothing OrElse Not includeEmbedded AndAlso Me._latestLazyRootDeclaration.IsEmbedded) Then
				Return Nothing
			End If
			Return Me._latestLazyRootDeclaration.Root.Value
		End Function

		Private Function GetMergedNamespaceNames() As ICollection(Of String)
			Dim strs As ICollection(Of String)
			Dim value As ICollection(Of String) = Me._cache.NamespaceNames.Value
			Dim latestRootDeclarationIfAny As RootSingleNamespaceDeclaration = Me.GetLatestRootDeclarationIfAny(True)
			strs = If(latestRootDeclarationIfAny IsNot Nothing, UnionCollection(Of String).Create(value, DeclarationTable.GetNamespaceNames(latestRootDeclarationIfAny)), value)
			Return strs
		End Function

		Private Function GetMergedReferenceDirectives() As ICollection(Of ReferenceDirective)
			Dim referenceDirectives As ICollection(Of ReferenceDirective)
			Dim value As ImmutableArray(Of ReferenceDirective) = Me._cache.ReferenceDirectives.Value
			Dim latestRootDeclarationIfAny As RootSingleNamespaceDeclaration = Me.GetLatestRootDeclarationIfAny(False)
			referenceDirectives = If(latestRootDeclarationIfAny IsNot Nothing, UnionCollection(Of ReferenceDirective).Create(DirectCast(value, ICollection(Of ReferenceDirective)), DirectCast(latestRootDeclarationIfAny.ReferenceDirectives, ICollection(Of ReferenceDirective))), DirectCast(value, ICollection(Of ReferenceDirective)))
			Return referenceDirectives
		End Function

		Public Function GetMergedRoot(ByVal compilation As VisualBasicCompilation) As MergedNamespaceDeclaration
			If (Me._mergedRoot Is Nothing) Then
				Interlocked.CompareExchange(Of MergedNamespaceDeclaration)(Me._mergedRoot, Me.CalculateMergedRoot(compilation), Nothing)
			End If
			Return Me._mergedRoot
		End Function

		Private Function GetMergedTypeNames() As ICollection(Of String)
			Dim strs As ICollection(Of String)
			Dim value As ICollection(Of String) = Me._cache.TypeNames.Value
			Dim latestRootDeclarationIfAny As RootSingleNamespaceDeclaration = Me.GetLatestRootDeclarationIfAny(True)
			strs = If(latestRootDeclarationIfAny IsNot Nothing, UnionCollection(Of String).Create(value, DeclarationTable.GetTypeNames(latestRootDeclarationIfAny)), value)
			Return strs
		End Function

		Private Shared Function GetNames(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration, ByVal predicate As Predicate(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration)) As ICollection(Of String)
			Dim identifierCollection As Microsoft.CodeAnalysis.IdentifierCollection = New Microsoft.CodeAnalysis.IdentifierCollection()
			Dim declarations As Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration) = New Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration)()
			declarations.Push(declaration)
			While declarations.Count > 0
				Dim declaration1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration = declarations.Pop()
				If (declaration1 Is Nothing) Then
					Continue While
				End If
				If (predicate(declaration1)) Then
					identifierCollection.AddIdentifier(declaration1.Name)
				End If
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration).Enumerator = declaration1.Children.GetEnumerator()
				While enumerator.MoveNext()
					declarations.Push(enumerator.Current)
				End While
			End While
			Return identifierCollection.AsCaseInsensitiveCollection()
		End Function

		Private Shared Function GetNamespaceNames(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration) As ICollection(Of String)
			Return DeclarationTable.GetNames(declaration, DeclarationTable.s_isNamespacePredicate)
		End Function

		Private Sub GetOlderNamespaces(ByVal builder As ArrayBuilder(Of RootSingleNamespaceDeclaration))
			Dim enumerator As IEnumerator(Of DeclarationTableEntry) = Nothing
			Try
				enumerator = Me._allOlderRootDeclarations.InInsertionOrder.GetEnumerator()
				While enumerator.MoveNext()
					Dim value As RootSingleNamespaceDeclaration = enumerator.Current.Root.Value
					If (value Is Nothing) Then
						Continue While
					End If
					builder.Add(value)
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Private Shared Function GetTypeNames(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration) As ICollection(Of String)
			Return DeclarationTable.GetNames(declaration, DeclarationTable.s_isTypePredicate)
		End Function

		Private Function MergeOlderNamespaces() As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration
			Dim instance As ArrayBuilder(Of RootSingleNamespaceDeclaration) = ArrayBuilder(Of RootSingleNamespaceDeclaration).GetInstance()
			Me.GetOlderNamespaces(instance)
			Dim mergedNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration = Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration.Create(instance)
			instance.Free()
			Return mergedNamespaceDeclaration
		End Function

		Public Function RemoveRootDeclaration(ByVal lazyRootDeclaration As DeclarationTableEntry) As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable
			Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable
			declarationTable = If(Me._latestLazyRootDeclaration <> lazyRootDeclaration, New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable(Me._allOlderRootDeclarations.Remove(lazyRootDeclaration), Me._latestLazyRootDeclaration, Nothing), New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable(Me._allOlderRootDeclarations, Nothing, Me._cache))
			Return declarationTable
		End Function

		Private Function SelectManyFromOlderDeclarationsNoEmbedded(Of T)(ByVal selector As Func(Of RootSingleNamespaceDeclaration, ImmutableArray(Of T))) As ImmutableArray(Of T)
			Dim isEmbedded As Func(Of DeclarationTableEntry, Boolean)
			Dim inInsertionOrder As IEnumerable(Of DeclarationTableEntry) = Me._allOlderRootDeclarations.InInsertionOrder
			If (DeclarationTable._Closure$__18(Of T).$I18-0 Is Nothing) Then
				isEmbedded = Function(d As DeclarationTableEntry)
					If (d.IsEmbedded) Then
						Return False
					End If
					Return d.Root.Value IsNot Nothing
				End Function
				DeclarationTable._Closure$__18(Of T).$I18-0 = isEmbedded
			Else
				isEmbedded = DeclarationTable._Closure$__18(Of T).$I18-0
			End If
			Return inInsertionOrder.Where(isEmbedded).SelectMany(Of T)(Function(d As DeclarationTableEntry) DirectCast(selector(d.Root.Value), IEnumerable(Of $CLS0))).AsImmutable()
		End Function

		Private Class Cache
			Friend ReadOnly MergedRoot As Lazy(Of MergedNamespaceDeclaration)

			Friend ReadOnly TypeNames As Lazy(Of ICollection(Of String))

			Friend ReadOnly NamespaceNames As Lazy(Of ICollection(Of String))

			Friend ReadOnly ReferenceDirectives As Lazy(Of ImmutableArray(Of ReferenceDirective))

			Public Sub New(ByVal table As DeclarationTable)
				MyBase.New()
				Me.MergedRoot = New Lazy(Of MergedNamespaceDeclaration)(New Func(Of MergedNamespaceDeclaration)(AddressOf table.MergeOlderNamespaces))
				Me.TypeNames = New Lazy(Of ICollection(Of String))(Function() DeclarationTable.GetTypeNames(Me.MergedRoot.Value))
				Me.NamespaceNames = New Lazy(Of ICollection(Of String))(Function() DeclarationTable.GetNamespaceNames(Me.MergedRoot.Value))
				Me.ReferenceDirectives = New Lazy(Of ImmutableArray(Of ReferenceDirective))(Function()
					Dim referenceDirectives As Func(Of RootSingleNamespaceDeclaration, ImmutableArray(Of ReferenceDirective))
					Dim u0024VBu0024LocalTable As DeclarationTable = table
					If (DeclarationTable.Cache._Closure$__.$I4-3 Is Nothing) Then
						referenceDirectives = Function(r As RootSingleNamespaceDeclaration) r.ReferenceDirectives
						DeclarationTable.Cache._Closure$__.$I4-3 = referenceDirectives
					Else
						referenceDirectives = DeclarationTable.Cache._Closure$__.$I4-3
					End If
					Return u0024VBu0024LocalTable.SelectManyFromOlderDeclarationsNoEmbedded(Of ReferenceDirective)(referenceDirectives)
				End Function)
			End Sub
		End Class

		Private NotInheritable Class RootNamespaceLocationComparer
			Implements IComparer(Of SingleNamespaceDeclaration)
			Private ReadOnly _compilation As VisualBasicCompilation

			Friend Sub New(ByVal compilation As VisualBasicCompilation)
				MyBase.New()
				Me._compilation = compilation
			End Sub

			Public Function Compare(ByVal x As SingleNamespaceDeclaration, ByVal y As SingleNamespaceDeclaration) As Integer Implements IComparer(Of SingleNamespaceDeclaration).Compare
				Return Me._compilation.CompareSourceLocations(x.SyntaxReference, y.SyntaxReference)
			End Function
		End Class
	End Class
End Namespace