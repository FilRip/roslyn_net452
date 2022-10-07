Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Collections
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
	Friend MustInherit Class MergedNamespaceSymbol
		Inherits PEOrSourceOrMergedNamespaceSymbol
		Protected ReadOnly _namespacesToMerge As ImmutableArray(Of NamespaceSymbol)

		Protected ReadOnly _containingNamespace As MergedNamespaceSymbol

		Private ReadOnly _cachedLookup As CachingDictionary(Of String, Symbol)

		Private _lazyModuleMembers As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyMembers As ImmutableArray(Of Symbol)

		Private _lazyEmbeddedKind As Integer

		Public Overrides ReadOnly Property ConstituentNamespaces As ImmutableArray(Of NamespaceSymbol)
			Get
				Return Me._namespacesToMerge
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Dim assembly As AssemblySymbol
				If (Me.Extent.Kind = Microsoft.CodeAnalysis.NamespaceKind.[Module]) Then
					assembly = Me.Extent.[Module].ContainingAssembly
				ElseIf (Me.Extent.Kind <> Microsoft.CodeAnalysis.NamespaceKind.Assembly) Then
					assembly = Nothing
				Else
					assembly = Me.Extent.Assembly
				End If
				Return assembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingNamespace
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim func As Func(Of NamespaceSymbol, IEnumerable(Of SyntaxReference))
				Dim func1 As Func(Of NamespaceSymbol, SyntaxReference, SyntaxReference)
				Dim namespaceSymbols As ImmutableArray(Of NamespaceSymbol) = Me._namespacesToMerge
				If (MergedNamespaceSymbol._Closure$__.$I40-0 Is Nothing) Then
					func = Function(ns As NamespaceSymbol) DirectCast(ns.DeclaringSyntaxReferences, IEnumerable(Of SyntaxReference))
					MergedNamespaceSymbol._Closure$__.$I40-0 = func
				Else
					func = MergedNamespaceSymbol._Closure$__.$I40-0
				End If
				If (MergedNamespaceSymbol._Closure$__.$I40-1 Is Nothing) Then
					func1 = Function(ns As NamespaceSymbol, reference As SyntaxReference) reference
					MergedNamespaceSymbol._Closure$__.$I40-1 = func1
				Else
					func1 = MergedNamespaceSymbol._Closure$__.$I40-1
				End If
				Return ImmutableArray.CreateRange(Of SyntaxReference)(namespaceSymbols.SelectMany(Of SyntaxReference, SyntaxReference)(func, func1))
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				If (Me._lazyEmbeddedKind = 1) Then
					Dim num As Integer = 0
					Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
					While enumerator.MoveNext()
						num = num Or CInt(enumerator.Current.EmbeddedSymbolKind)
					End While
					Interlocked.CompareExchange(Me._lazyEmbeddedKind, num, 1)
				End If
				Return DirectCast(CByte(Me._lazyEmbeddedKind), Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind)
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim func As Func(Of NamespaceSymbol, IEnumerable(Of Location))
				Dim func1 As Func(Of NamespaceSymbol, Location, Location)
				Dim namespaceSymbols As ImmutableArray(Of NamespaceSymbol) = Me._namespacesToMerge
				If (MergedNamespaceSymbol._Closure$__.$I38-0 Is Nothing) Then
					func = Function(ns As NamespaceSymbol) DirectCast(ns.Locations, IEnumerable(Of Location))
					MergedNamespaceSymbol._Closure$__.$I38-0 = func
				Else
					func = MergedNamespaceSymbol._Closure$__.$I38-0
				End If
				If (MergedNamespaceSymbol._Closure$__.$I38-1 Is Nothing) Then
					func1 = Function(ns As NamespaceSymbol, loc As Location) loc
					MergedNamespaceSymbol._Closure$__.$I38-1 = func1
				Else
					func1 = MergedNamespaceSymbol._Closure$__.$I38-1
				End If
				Return ImmutableArray.CreateRange(Of Location)(namespaceSymbols.SelectMany(Of Location, Location)(func, func1))
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._namespacesToMerge(0).Name
			End Get
		End Property

		Friend MustOverride ReadOnly Property RawContainsAccessibleTypes As ThreeState

		Private Sub New(ByVal containingNamespace As MergedNamespaceSymbol, ByVal namespacesToMerge As ImmutableArray(Of NamespaceSymbol))
			MyBase.New()
			Me._lazyEmbeddedKind = 1
			Me._namespacesToMerge = namespacesToMerge
			Me._containingNamespace = containingNamespace
			Me._cachedLookup = New CachingDictionary(Of String, Symbol)(New Func(Of String, ImmutableArray(Of Symbol))(AddressOf Me.SlowGetChildrenOfName), New Func(Of IEqualityComparer(Of String), HashSet(Of String))(AddressOf Me.SlowGetChildNames), CaseInsensitiveComparison.Comparer)
		End Sub

		Private Shared Function ConstituentGlobalNamespaces(ByVal extent As AssemblySymbol) As IEnumerable(Of NamespaceSymbol)
			Return New MergedNamespaceSymbol.VB$StateMachine_7_ConstituentGlobalNamespaces(-2) With
			{
				.$P_extent = extent
			}
		End Function

		Private Shared Function ConstituentGlobalNamespaces(ByVal extent As VisualBasicCompilation) As IEnumerable(Of NamespaceSymbol)
			Return New MergedNamespaceSymbol.VB$StateMachine_11_ConstituentGlobalNamespaces(-2) With
			{
				.$P_extent = extent
			}
		End Function

		Private Shared Function Create(ByVal extent As AssemblySymbol, ByVal containingNamespace As MergedNamespaceSymbol.AssemblyMergedNamespaceSymbol, ByVal namespacesToMerge As ImmutableArray(Of NamespaceSymbol)) As NamespaceSymbol
			Dim assemblyMergedNamespaceSymbol As NamespaceSymbol
			If (namespacesToMerge.Length <> 1) Then
				assemblyMergedNamespaceSymbol = New MergedNamespaceSymbol.AssemblyMergedNamespaceSymbol(extent, containingNamespace, namespacesToMerge)
			Else
				assemblyMergedNamespaceSymbol = namespacesToMerge(0)
			End If
			Return assemblyMergedNamespaceSymbol
		End Function

		Private Shared Function Create(ByVal extent As VisualBasicCompilation, ByVal containingNamespace As MergedNamespaceSymbol.CompilationMergedNamespaceSymbol, ByVal namespacesToMerge As IEnumerable(Of NamespaceSymbol)) As NamespaceSymbol
			Dim compilationMergedNamespaceSymbol As NamespaceSymbol
			Dim instance As ArrayBuilder(Of NamespaceSymbol) = ArrayBuilder(Of NamespaceSymbol).GetInstance()
			instance.AddRange(namespacesToMerge)
			If (instance.Count <> 1) Then
				compilationMergedNamespaceSymbol = New MergedNamespaceSymbol.CompilationMergedNamespaceSymbol(extent, containingNamespace, instance.ToImmutableAndFree())
			Else
				Dim item As NamespaceSymbol = instance(0)
				instance.Free()
				compilationMergedNamespaceSymbol = item
			End If
			Return compilationMergedNamespaceSymbol
		End Function

		Private Shared Function Create(ByVal containingNamespace As MergedNamespaceSymbol.NamespaceGroupSymbol, ByVal namespacesToMerge As IEnumerable(Of NamespaceSymbol)) As NamespaceSymbol
			Dim namespaceGroupSymbol As NamespaceSymbol
			Dim instance As ArrayBuilder(Of NamespaceSymbol) = ArrayBuilder(Of NamespaceSymbol).GetInstance()
			instance.AddRange(namespacesToMerge)
			If (instance.Count <> 1) Then
				namespaceGroupSymbol = New MergedNamespaceSymbol.NamespaceGroupSymbol(containingNamespace, instance.ToImmutableAndFree())
			Else
				Dim item As NamespaceSymbol = instance(0)
				instance.Free()
				namespaceGroupSymbol = item
			End If
			Return namespaceGroupSymbol
		End Function

		Protected MustOverride Function CreateChildMergedNamespaceSymbol(ByVal nsSymbols As ImmutableArray(Of NamespaceSymbol)) As NamespaceSymbol

		Friend Shared Function CreateForTestPurposes(ByVal extent As AssemblySymbol, ByVal namespacesToMerge As IEnumerable(Of NamespaceSymbol)) As NamespaceSymbol
			Return MergedNamespaceSymbol.Create(extent, Nothing, namespacesToMerge.AsImmutable())
		End Function

		Public Shared Function CreateGlobalNamespace(ByVal extent As AssemblySymbol) As NamespaceSymbol
			Return MergedNamespaceSymbol.Create(extent, Nothing, MergedNamespaceSymbol.ConstituentGlobalNamespaces(extent).AsImmutable())
		End Function

		Public Shared Function CreateGlobalNamespace(ByVal extent As VisualBasicCompilation) As NamespaceSymbol
			Return MergedNamespaceSymbol.Create(extent, Nothing, MergedNamespaceSymbol.ConstituentGlobalNamespaces(extent))
		End Function

		Public Shared Function CreateNamespaceGroup(ByVal namespacesToMerge As IEnumerable(Of NamespaceSymbol)) As NamespaceSymbol
			Return MergedNamespaceSymbol.Create(MergedNamespaceSymbol.NamespaceGroupSymbol.GlobalNamespace, namespacesToMerge)
		End Function

		Friend Function GetConstituentForCompilation(ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = enumerator.Current
					If (current.IsFromCompilation(compilation)) Then
						namespaceSymbol = current
						Exit While
					End If
				Else
					namespaceSymbol = Nothing
					Exit While
				End If
			End While
			Return namespaceSymbol
		End Function

		Protected NotOverridable Overrides Function GetDeclaredAccessibilityOfMostAccessibleDescendantType() As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility1 As Microsoft.CodeAnalysis.Accessibility = Microsoft.CodeAnalysis.Accessibility.NotApplicable
			Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim declaredAccessibilityOfMostAccessibleDescendantType As Microsoft.CodeAnalysis.Accessibility = enumerator.Current.DeclaredAccessibilityOfMostAccessibleDescendantType
					If (declaredAccessibilityOfMostAccessibleDescendantType > accessibility1) Then
						If (declaredAccessibilityOfMostAccessibleDescendantType <> Microsoft.CodeAnalysis.Accessibility.[Public]) Then
							accessibility1 = declaredAccessibilityOfMostAccessibleDescendantType
						Else
							accessibility = Microsoft.CodeAnalysis.Accessibility.[Public]
							Exit While
						End If
					End If
				Else
					accessibility = accessibility1
					Exit While
				End If
			End While
			Return accessibility
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			If (Me._lazyMembers.IsDefault) Then
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Me._cachedLookup.AddValues(instance)
				Me._lazyMembers = instance.ToImmutableAndFree()
			End If
			Return Me._lazyMembers
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return Me._cachedLookup(name)
		End Function

		Public Overrides Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)
			If (Me._lazyModuleMembers.IsDefault) Then
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
				While enumerator.MoveNext()
					instance.AddRange(enumerator.Current.GetModuleMembers())
				End While
				Dim immutableAndFree As ImmutableArray(Of NamedTypeSymbol) = instance.ToImmutableAndFree()
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
				ImmutableInterlocked.InterlockedCompareExchange(Of NamedTypeSymbol)(Me._lazyModuleMembers, immutableAndFree, namedTypeSymbols)
			End If
			Return Me._lazyModuleMembers
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray.CreateRange(Of NamedTypeSymbol)(Me.GetMembers().OfType(Of NamedTypeSymbol)())
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim members As ImmutableArray(Of Symbol) = Me.GetMembers(name)
			Return ImmutableArray.CreateRange(Of NamedTypeSymbol)(members.OfType(Of NamedTypeSymbol)())
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray.CreateRange(Of NamedTypeSymbol)(Me.GetMembersUnordered().OfType(Of NamedTypeSymbol)())
		End Function

		Friend Overrides Function IsDeclaredInSourceModule(ByVal [module] As ModuleSymbol) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (enumerator.Current.IsDeclaredInSourceModule([module])) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Public Overridable Function Shrink(ByVal namespacesToMerge As IEnumerable(Of NamespaceSymbol)) As NamespaceSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function SlowGetChildNames(ByVal comparer As IEqualityComparer(Of String)) As HashSet(Of String)
			Dim strs As HashSet(Of String) = New HashSet(Of String)(comparer)
			Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.GetMembersUnordered().GetEnumerator()
				While enumerator1.MoveNext()
					strs.Add(DirectCast(enumerator1.Current, NamespaceOrTypeSymbol).Name)
				End While
			End While
			Return strs
		End Function

		Private Function SlowGetChildrenOfName(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim instance As ArrayBuilder(Of NamespaceSymbol) = Nothing
			Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.GetMembers(name).GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As Symbol = enumerator1.Current
					If (current.Kind <> SymbolKind.[Namespace]) Then
						symbols.Add(current)
					Else
						instance = If(instance, ArrayBuilder(Of NamespaceSymbol).GetInstance())
						instance.Add(DirectCast(current, NamespaceSymbol))
					End If
				End While
			End While
			If (instance IsNot Nothing) Then
				symbols.Add(Me.CreateChildMergedNamespaceSymbol(instance.ToImmutableAndFree()))
			End If
			Return symbols.ToImmutableAndFree()
		End Function

		Private NotInheritable Class AssemblyMergedNamespaceSymbol
			Inherits MergedNamespaceSymbol
			Private ReadOnly _assembly As AssemblySymbol

			Friend Overrides ReadOnly Property Extent As NamespaceExtent
				Get
					Return New NamespaceExtent(Me._assembly)
				End Get
			End Property

			Friend Overrides ReadOnly Property RawContainsAccessibleTypes As ThreeState
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Public Sub New(ByVal assembly As AssemblySymbol, ByVal containingNamespace As MergedNamespaceSymbol.AssemblyMergedNamespaceSymbol, ByVal namespacesToMerge As ImmutableArray(Of NamespaceSymbol))
				MyBase.New(containingNamespace, namespacesToMerge)
				Me._assembly = assembly
			End Sub

			Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)))
				Throw ExceptionUtilities.Unreachable
			End Sub

			Protected Overrides Function CreateChildMergedNamespaceSymbol(ByVal nsSymbols As ImmutableArray(Of NamespaceSymbol)) As NamespaceSymbol
				Return MergedNamespaceSymbol.Create(Me._assembly, Me, nsSymbols)
			End Function
		End Class

		Private NotInheritable Class CompilationMergedNamespaceSymbol
			Inherits MergedNamespaceSymbol
			Private ReadOnly _compilation As VisualBasicCompilation

			Private _containsAccessibleTypes As ThreeState

			Private _isDeclaredInSourceModule As ThreeState

			Friend Overrides ReadOnly Property Extent As NamespaceExtent
				Get
					Return New NamespaceExtent(Me._compilation)
				End Get
			End Property

			Friend Overrides ReadOnly Property RawContainsAccessibleTypes As ThreeState
				Get
					Return Me._containsAccessibleTypes
				End Get
			End Property

			Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal containingNamespace As MergedNamespaceSymbol.CompilationMergedNamespaceSymbol, ByVal namespacesToMerge As ImmutableArray(Of NamespaceSymbol))
				MyBase.New(containingNamespace, namespacesToMerge)
				Me._containsAccessibleTypes = ThreeState.Unknown
				Me._isDeclaredInSourceModule = ThreeState.Unknown
				Me._compilation = compilation
			End Sub

			Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)))
				Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.BuildExtensionMethodsMap(map)
				End While
			End Sub

			Friend Overrides Function ContainsTypesAccessibleFrom(ByVal fromAssembly As AssemblySymbol) As Boolean
				Dim flag As Boolean
				If (Me._containsAccessibleTypes.HasValue()) Then
					flag = Me._containsAccessibleTypes = ThreeState.[True]
				ElseIf (MyBase.RawLazyDeclaredAccessibilityOfMostAccessibleDescendantType <> Accessibility.[Public]) Then
					Dim flag1 As Boolean = False
					Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
					While enumerator.MoveNext()
						If (Not enumerator.Current.ContainsTypesAccessibleFrom(fromAssembly)) Then
							Continue While
						End If
						flag1 = True
						Exit While
					End While
					If (Not flag1) Then
						Me._containsAccessibleTypes = ThreeState.[False]
					Else
						Me._containsAccessibleTypes = ThreeState.[True]
						Dim containingSymbol As MergedNamespaceSymbol.CompilationMergedNamespaceSymbol = TryCast(Me.ContainingSymbol, MergedNamespaceSymbol.CompilationMergedNamespaceSymbol)
						While containingSymbol IsNot Nothing
							If (containingSymbol._containsAccessibleTypes = ThreeState.Unknown) Then
								containingSymbol._containsAccessibleTypes = ThreeState.[True]
								containingSymbol = TryCast(containingSymbol.ContainingSymbol, MergedNamespaceSymbol.CompilationMergedNamespaceSymbol)
							Else
								Exit While
							End If
						End While
					End If
					flag = flag1
				Else
					flag = True
				End If
				Return flag
			End Function

			Protected Overrides Function CreateChildMergedNamespaceSymbol(ByVal nsSymbols As ImmutableArray(Of NamespaceSymbol)) As NamespaceSymbol
				Return MergedNamespaceSymbol.Create(Me._compilation, Me, DirectCast(nsSymbols, IEnumerable(Of NamespaceSymbol)))
			End Function

			Friend Overrides Sub GetExtensionMethods(ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal name As String)
				Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.GetExtensionMethods(methods, name)
				End While
			End Sub

			Friend Overrides Function IsDeclaredInSourceModule(ByVal [module] As ModuleSymbol) As Boolean
				Dim flag As Boolean
				If (Not Me._isDeclaredInSourceModule.HasValue()) Then
					Dim flag1 As Boolean = MyBase.IsDeclaredInSourceModule([module])
					If (Not flag1) Then
						Me._isDeclaredInSourceModule = ThreeState.[False]
					Else
						Me._isDeclaredInSourceModule = ThreeState.[True]
						Dim containingSymbol As MergedNamespaceSymbol.CompilationMergedNamespaceSymbol = TryCast(Me.ContainingSymbol, MergedNamespaceSymbol.CompilationMergedNamespaceSymbol)
						While containingSymbol IsNot Nothing
							If (containingSymbol._isDeclaredInSourceModule = ThreeState.Unknown) Then
								containingSymbol._isDeclaredInSourceModule = ThreeState.[True]
								containingSymbol = TryCast(containingSymbol.ContainingSymbol, MergedNamespaceSymbol.CompilationMergedNamespaceSymbol)
							Else
								Exit While
							End If
						End While
					End If
					flag = flag1
				Else
					flag = Me._isDeclaredInSourceModule = ThreeState.[True]
				End If
				Return flag
			End Function
		End Class

		Private NotInheritable Class NamespaceGroupSymbol
			Inherits MergedNamespaceSymbol
			Public ReadOnly Shared GlobalNamespace As MergedNamespaceSymbol.NamespaceGroupSymbol

			Friend Overrides ReadOnly Property Extent As NamespaceExtent
				Get
					Return New NamespaceExtent()
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					If (Me._namespacesToMerge.Length <= 0) Then
						Return ""
					End If
					Return Me._namespacesToMerge(0).Name
				End Get
			End Property

			Friend Overrides ReadOnly Property RawContainsAccessibleTypes As ThreeState
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Shared Sub New()
				MergedNamespaceSymbol.NamespaceGroupSymbol.GlobalNamespace = New MergedNamespaceSymbol.NamespaceGroupSymbol(Nothing, ImmutableArray(Of NamespaceSymbol).Empty)
			End Sub

			Public Sub New(ByVal containingNamespace As MergedNamespaceSymbol.NamespaceGroupSymbol, ByVal namespacesToMerge As ImmutableArray(Of NamespaceSymbol))
				MyBase.New(containingNamespace, namespacesToMerge)
			End Sub

			Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)))
				Throw ExceptionUtilities.Unreachable
			End Sub

			Protected Overrides Function CreateChildMergedNamespaceSymbol(ByVal nsSymbols As ImmutableArray(Of NamespaceSymbol)) As NamespaceSymbol
				Return MergedNamespaceSymbol.Create(Me, DirectCast(nsSymbols, IEnumerable(Of NamespaceSymbol)))
			End Function

			Public Overrides Function Shrink(ByVal namespacesToMerge As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).GetInstance()
				instance.AddRange(namespacesToMerge)
				If (instance.Count = 0) Then
					instance.Free()
					namespaceSymbol = Me
				ElseIf (instance.Count <> 1) Then
					Dim namespaceSymbols As SmallDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, Boolean) = New SmallDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, Boolean)()
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).Enumerator = Me._namespacesToMerge.GetEnumerator()
					While enumerator.MoveNext()
						namespaceSymbols(enumerator.Current) = False
					End While
					Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).Enumerator = instance.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = enumerator1.Current
						Dim flag As Boolean = False
						If (namespaceSymbols.TryGetValue(current, flag)) Then
							Continue While
						End If
						instance.Free()
						namespaceSymbol = Me
						Return namespaceSymbol
					End While
					namespaceSymbol = Me.Shrink(instance.ToImmutableAndFree())
				Else
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = instance(0)
					instance.Free()
					If (Not Me._namespacesToMerge.Contains(item)) Then
						Throw ExceptionUtilities.Unreachable
					End If
					namespaceSymbol = item
				End If
				Return namespaceSymbol
			End Function

			Private Function Shrink(ByVal namespaceArray As ImmutableArray(Of NamespaceSymbol)) As MergedNamespaceSymbol.NamespaceGroupSymbol
				Dim namespaceGroupSymbol As MergedNamespaceSymbol.NamespaceGroupSymbol
				If (namespaceArray.Length >= Me._namespacesToMerge.Length) Then
					namespaceGroupSymbol = Me
				ElseIf (Me._containingNamespace <> MergedNamespaceSymbol.NamespaceGroupSymbol.GlobalNamespace) Then
					Dim instance As ArrayBuilder(Of NamespaceSymbol) = ArrayBuilder(Of NamespaceSymbol).GetInstance(namespaceArray.Length)
					Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = namespaceArray.GetEnumerator()
					While enumerator.MoveNext()
						instance.Add(enumerator.Current.ContainingNamespace)
					End While
					namespaceGroupSymbol = New MergedNamespaceSymbol.NamespaceGroupSymbol(DirectCast(Me._containingNamespace, MergedNamespaceSymbol.NamespaceGroupSymbol).Shrink(instance.ToImmutableAndFree()), namespaceArray)
				Else
					namespaceGroupSymbol = New MergedNamespaceSymbol.NamespaceGroupSymbol(MergedNamespaceSymbol.NamespaceGroupSymbol.GlobalNamespace, namespaceArray)
				End If
				Return namespaceGroupSymbol
			End Function
		End Class
	End Class
End Namespace