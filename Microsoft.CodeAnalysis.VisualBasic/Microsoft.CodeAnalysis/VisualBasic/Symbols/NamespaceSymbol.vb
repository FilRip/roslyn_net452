Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class NamespaceSymbol
		Inherits NamespaceOrTypeSymbol
		Implements INamespace, INamespaceSymbol, INamespaceSymbolInternal
		Friend ReadOnly Property AdaptedNamespaceSymbol As NamespaceSymbol
			Get
				Return Me
			End Get
		End Property

		Public Overridable ReadOnly Property ConstituentNamespaces As ImmutableArray(Of NamespaceSymbol)
			Get
				Return ImmutableArray.Create(Of NamespaceSymbol)(Me)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol

		Public ReadOnly Property ContainingCompilation As VisualBasicCompilation
			Get
				If (Me.NamespaceKind <> Microsoft.CodeAnalysis.NamespaceKind.Compilation) Then
					Return Nothing
				End If
				Return Me.Extent.Compilation
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Dim [module] As ModuleSymbol
				Dim extent As NamespaceExtent = Me.Extent
				If (extent.Kind <> Microsoft.CodeAnalysis.NamespaceKind.[Module]) Then
					[module] = Nothing
				Else
					[module] = extent.[Module]
				End If
				Return [module]
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Friend MustOverride ReadOnly Property DeclaredAccessibilityOfMostAccessibleDescendantType As Accessibility

		Friend MustOverride ReadOnly Property Extent As NamespaceExtent

		ReadOnly Property INamedEntity_Name As String
			Get
				Return Me.AdaptedNamespaceSymbol.MetadataName
			End Get
		End Property

		ReadOnly Property INamespaceSymbol_ConstituentNamespaces As ImmutableArray(Of INamespaceSymbol) Implements INamespaceSymbol.ConstituentNamespaces
			Get
				Return StaticCast(Of INamespaceSymbol).From(Of NamespaceSymbol)(Me.ConstituentNamespaces)
			End Get
		End Property

		ReadOnly Property INamespaceSymbol_ContainingCompilation As Compilation Implements INamespaceSymbol.ContainingCompilation
			Get
				Return Me.ContainingCompilation
			End Get
		End Property

		ReadOnly Property INamespaceSymbol_ContainingNamespace As INamespace Implements INamespace.ContainingNamespace
			Get
				Dim containingNamespace As NamespaceSymbol = Me.AdaptedNamespaceSymbol.ContainingNamespace
				If (containingNamespace IsNot Nothing) Then
					Return containingNamespace.GetCciAdapter()
				End If
				Return Nothing
			End Get
		End Property

		ReadOnly Property INamespaceSymbol_NamespaceKind As Microsoft.CodeAnalysis.NamespaceKind Implements INamespaceSymbol.NamespaceKind
			Get
				Return Me.NamespaceKind
			End Get
		End Property

		Public Overridable ReadOnly Property IsGlobalNamespace As Boolean Implements INamespaceSymbol.IsGlobalNamespace, INamespaceSymbolInternal.IsGlobalNamespace
			Get
				Return MyBase.ContainingNamespace Is Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.IsGlobalNamespace
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.[Namespace]
			End Get
		End Property

		Public ReadOnly Property NamespaceKind As Microsoft.CodeAnalysis.NamespaceKind
			Get
				Return Me.Extent.Kind
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitNamespace(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitNamespace(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitNamespace(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitNamespace(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitNamespace(Me)
		End Function

		Friend Overridable Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me)
		End Sub

		Friend MustOverride Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamespaceSymbol)

		Friend Sub AddMemberIfExtension(ByVal bucket As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), ByVal member As Symbol)
			If (member.Kind = SymbolKind.Method) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				If (methodSymbol.MayBeReducibleExtensionMethod) Then
					Me.BuildExtensionMethodsMapBucket(bucket, methodSymbol)
				End If
			End If
		End Sub

		Friend MustOverride Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol))

		Friend Overridable Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)))
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = Me.TypesToCheckForExtensionMethods.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.BuildExtensionMethodsMap(map, Me)
			End While
		End Sub

		Friend Function BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)), ByVal membersByName As IEnumerable(Of KeyValuePair(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)))) As Boolean
			Dim enumerator As IEnumerator(Of KeyValuePair(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))) = Nothing
			Using flag As Boolean = False
				enumerator = membersByName.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)) = enumerator.Current
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Nothing
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = current.Value.GetEnumerator()
					While enumerator1.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (symbol.Kind <> SymbolKind.Method) Then
							Continue While
						End If
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (Not methodSymbol.MayBeReducibleExtensionMethod) Then
							Continue While
						End If
						If (instance Is Nothing AndAlso Not map.TryGetValue(methodSymbol.Name, instance)) Then
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
							map.Add(current.Key, instance)
						End If
						Me.BuildExtensionMethodsMapBucket(instance, methodSymbol)
						flag = True
					End While
				End While
			End Using
			Return flag
		End Function

		Friend Overridable Sub BuildExtensionMethodsMapBucket(ByVal bucket As ArrayBuilder(Of MethodSymbol), ByVal method As MethodSymbol)
			bucket.Add(method)
		End Sub

		Friend Overridable Function ContainsTypesAccessibleFrom(ByVal fromAssembly As AssemblySymbol) As Boolean
			Dim flag As Boolean
			Dim declaredAccessibilityOfMostAccessibleDescendantType As Accessibility = Me.DeclaredAccessibilityOfMostAccessibleDescendantType
			If (declaredAccessibilityOfMostAccessibleDescendantType = Accessibility.[Public]) Then
				flag = True
			ElseIf (declaredAccessibilityOfMostAccessibleDescendantType <> Accessibility.Internal) Then
				flag = False
			Else
				Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
				flag = If(containingAssembly Is Nothing, False, AccessCheck.HasFriendAccessTo(fromAssembly, containingAssembly))
			End If
			Return flag
		End Function

		Friend Shadows Function GetCciAdapter() As NamespaceSymbol
			Return Me
		End Function

		Protected Overridable Function GetDeclaredAccessibilityOfMostAccessibleDescendantType() As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility1 As Microsoft.CodeAnalysis.Accessibility = Microsoft.CodeAnalysis.Accessibility.NotApplicable
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = Me.GetTypeMembersUnordered().GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = Me.GetMembersUnordered().GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As Symbol = enumerator1.Current
						If (current.Kind <> SymbolKind.[Namespace]) Then
							Continue While
						End If
						Dim declaredAccessibilityOfMostAccessibleDescendantType As Microsoft.CodeAnalysis.Accessibility = DirectCast(current, NamespaceSymbol).DeclaredAccessibilityOfMostAccessibleDescendantType
						If (declaredAccessibilityOfMostAccessibleDescendantType <= accessibility1) Then
							Continue While
						End If
						If (declaredAccessibilityOfMostAccessibleDescendantType <> Microsoft.CodeAnalysis.Accessibility.[Public]) Then
							accessibility1 = declaredAccessibilityOfMostAccessibleDescendantType
						Else
							accessibility = Microsoft.CodeAnalysis.Accessibility.[Public]
							Return accessibility
						End If
					End While
					accessibility = accessibility1
					Exit While
				ElseIf (enumerator.Current.DeclaredAccessibility <> Microsoft.CodeAnalysis.Accessibility.[Public]) Then
					accessibility1 = Microsoft.CodeAnalysis.Accessibility.Internal
				Else
					accessibility = Microsoft.CodeAnalysis.Accessibility.[Public]
					Exit While
				End If
			End While
			Return accessibility
		End Function

		Friend Overridable Sub GetExtensionMethods(ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal name As String)
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = Me.TypesToCheckForExtensionMethods.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.GetExtensionMethods(methods, Me, name)
			End While
		End Sub

		Public MustOverride Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)

		Public Overridable Function GetModuleMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim typeKind As Func(Of NamedTypeSymbol, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (NamespaceSymbol._Closure$__.$I10-0 Is Nothing) Then
				typeKind = Function(t As NamedTypeSymbol) t.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]
				NamespaceSymbol._Closure$__.$I10-0 = typeKind
			Else
				typeKind = NamespaceSymbol._Closure$__.$I10-0
			End If
			Return typeMembers.WhereAsArray(typeKind)
		End Function

		Public Overridable Function GetNamespaceMembers() As IEnumerable(Of NamespaceSymbol)
			Return Me.GetMembers().OfType(Of NamespaceSymbol)()
		End Function

		Friend Function GetNestedNamespace(ByVal name As String) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers(name).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					If (current.Kind = SymbolKind.[Namespace]) Then
						namespaceSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
						Exit While
					End If
				Else
					namespaceSymbol = Nothing
					Exit While
				End If
			End While
			Return namespaceSymbol
		End Function

		Friend Function GetNestedNamespace(ByVal name As NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim nestedNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = name.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
				nestedNamespace = Me.GetNestedNamespace(DirectCast(name, IdentifierNameSyntax).Identifier.ValueText)
			Else
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
					Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
					Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GetNestedNamespace(qualifiedNameSyntax.Left)
					If (namespaceSymbol Is Nothing) Then
						GoTo Label1
					End If
					nestedNamespace = namespaceSymbol.GetNestedNamespace(qualifiedNameSyntax.Right)
					Return nestedNamespace
				End If
			Label1:
				nestedNamespace = Nothing
			End If
			Return nestedNamespace
		End Function

		Private Function INamespaceSymbol_GetInternalSymbol() As INamespaceSymbolInternal Implements INamespace.GetInternalSymbol
			Return Me.AdaptedNamespaceSymbol
		End Function

		Private Function ExplicitINamespaceSymbol_GetMembers() As IEnumerable(Of INamespaceOrTypeSymbol) Implements INamespaceSymbol.GetMembers
			Return Me.GetMembers().OfType(Of INamespaceOrTypeSymbol)()
		End Function

		Private Function ExplicitINamespaceSymbol_GetMembers(ByVal name As String) As IEnumerable(Of INamespaceOrTypeSymbol) Implements INamespaceSymbol.GetMembers
			Return Me.GetMembers(name).OfType(Of INamespaceOrTypeSymbol)()
		End Function

		Private Function INamespaceSymbol_GetNamespaceMembers() As IEnumerable(Of INamespaceSymbol) Implements INamespaceSymbol.GetNamespaceMembers
			Return Me.GetNamespaceMembers()
		End Function

		Friend Overridable Function IsDeclaredInSourceModule(ByVal [module] As ModuleSymbol) As Boolean
			Return CObj(Me.ContainingModule) = CObj([module])
		End Function

		Friend Overrides Function IsDefinedInSourceTree(ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Dim flag As Boolean
			flag = If(Not Me.IsGlobalNamespace, MyBase.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken), True)
			Return flag
		End Function

		Friend Overridable Function LookupMetadataType(ByRef fullEmittedName As MetadataTypeName) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim topLevel As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim typeMembers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim displayString As String = MyBase.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
			If (fullEmittedName.IsMangled AndAlso (fullEmittedName.ForcedArity = -1 OrElse fullEmittedName.ForcedArity = fullEmittedName.InferredArity)) Then
				typeMembers = Me.GetTypeMembers(fullEmittedName.UnmangledTypeName)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
					If (fullEmittedName.InferredArity <> current.Arity OrElse Not current.MangleName OrElse Not [String].Equals(current.Name, fullEmittedName.UnmangledTypeName, StringComparison.Ordinal)) Then
						Continue While
					End If
					If (Not [String].Equals(fullEmittedName.NamespaceName, If(current.GetEmittedNamespaceName(), displayString), StringComparison.Ordinal)) Then
						Continue While
					End If
					If (namedTypeSymbol Is Nothing) Then
						namedTypeSymbol = current
					Else
						namedTypeSymbol = Nothing
						Exit While
					End If
				End While
			End If
			Dim forcedArity As Integer = fullEmittedName.ForcedArity
			If (fullEmittedName.UseCLSCompliantNameArityEncoding) Then
				If (fullEmittedName.InferredArity > 0) Then
					If (namedTypeSymbol IsNot Nothing) Then
						topLevel = namedTypeSymbol
					Else
						If (fullEmittedName.FullName.StartsWith("Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxList", StringComparison.Ordinal)) Then
							Debugger.Break()
						End If
						topLevel = New MissingMetadataTypeSymbol.TopLevel(Me.ContainingModule, fullEmittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
					End If
					Return topLevel
				ElseIf (forcedArity = -1) Then
					forcedArity = 0
				ElseIf (forcedArity <> 0) Then
					If (namedTypeSymbol IsNot Nothing) Then
						topLevel = namedTypeSymbol
					Else
						If (fullEmittedName.FullName.StartsWith("Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxList", StringComparison.Ordinal)) Then
							Debugger.Break()
						End If
						topLevel = New MissingMetadataTypeSymbol.TopLevel(Me.ContainingModule, fullEmittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
					End If
					Return topLevel
				End If
			End If
			typeMembers = Me.GetTypeMembers(fullEmittedName.TypeName)
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
			While enumerator1.MoveNext()
				Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
				If (current1.MangleName OrElse forcedArity <> -1 AndAlso forcedArity <> current1.Arity OrElse Not [String].Equals(current1.Name, fullEmittedName.TypeName, StringComparison.Ordinal)) Then
					Continue While
				End If
				If (Not [String].Equals(fullEmittedName.NamespaceName, If(current1.GetEmittedNamespaceName(), displayString), StringComparison.Ordinal)) Then
					Continue While
				End If
				If (namedTypeSymbol Is Nothing) Then
					namedTypeSymbol = current1
				Else
					namedTypeSymbol = Nothing
					Exit While
				End If
			End While
			If (namedTypeSymbol IsNot Nothing) Then
				topLevel = namedTypeSymbol
			Else
				If (fullEmittedName.FullName.StartsWith("Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxList", StringComparison.Ordinal)) Then
					Debugger.Break()
				End If
				topLevel = New MissingMetadataTypeSymbol.TopLevel(Me.ContainingModule, fullEmittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
			End If
			Return topLevel
		End Function

		Friend Function LookupNestedNamespace(ByVal names As ImmutableArray(Of String)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me
			Dim enumerator As ImmutableArray(Of String).Enumerator = names.GetEnumerator()
			Do
				If (Not enumerator.MoveNext()) Then
					Exit Do
				End If
				Dim current As String = enumerator.Current
				Dim namespaceSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Nothing
				Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = namespaceSymbol.GetMembers(current).GetEnumerator()
				While enumerator1.MoveNext()
					Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(DirectCast(enumerator1.Current, NamespaceOrTypeSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
					If (current1 Is Nothing) Then
						Continue While
					End If
					If (namespaceSymbol1 Is Nothing) Then
						namespaceSymbol1 = current1
					Else
						namespaceSymbol1 = Nothing
						Exit While
					End If
				End While
				namespaceSymbol = namespaceSymbol1
			Loop While namespaceSymbol IsNot Nothing
			Return namespaceSymbol
		End Function

		Friend Function LookupNestedNamespace(ByVal names As String()) As NamespaceSymbol
			Return Me.LookupNestedNamespace(ImmutableArrayExtensions.AsImmutableOrNull(Of String)(names))
		End Function
	End Class
End Namespace