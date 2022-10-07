Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceNamespaceSymbol
		Inherits PEOrSourceOrMergedNamespaceSymbol
		Private ReadOnly _declaration As MergedNamespaceDeclaration

		Private ReadOnly _containingNamespace As SourceNamespaceSymbol

		Private ReadOnly _containingModule As SourceModuleSymbol

		Private _nameToMembersMap As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol))

		Private _nameToTypeMembersMap As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))

		Private _lazyEmbeddedKind As Integer

		Private _lazyState As Integer

		Private _lazyModuleMembers As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyAllMembers As ImmutableArray(Of Symbol)

		Private _lazyLexicalSortKey As LexicalSortKey

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._containingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._containingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Dim obj As Object = Me._containingNamespace
				If (obj Is Nothing) Then
					obj = Me._containingModule
				End If
				Return obj
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me.ComputeDeclaringReferencesCore()
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				If (Me._lazyEmbeddedKind = 1) Then
					Dim embeddedKind As Integer = 0
					Dim enumerator As ImmutableArray(Of Location).Enumerator = Me._declaration.NameLocations.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Location = enumerator.Current
						If (current.Kind <> LocationKind.None) Then
							Continue While
						End If
						Dim embeddedTreeLocation As Microsoft.CodeAnalysis.VisualBasic.EmbeddedTreeLocation = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.EmbeddedTreeLocation)
						If (embeddedTreeLocation Is Nothing) Then
							Continue While
						End If
						embeddedKind = embeddedKind Or CInt(embeddedTreeLocation.EmbeddedKind)
					End While
					Interlocked.CompareExchange(Me._lazyEmbeddedKind, embeddedKind, 1)
				End If
				Return DirectCast(CByte(Me._lazyEmbeddedKind), Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind)
			End Get
		End Property

		Friend Overrides ReadOnly Property Extent As NamespaceExtent
			Get
				Return New NamespaceExtent(Me._containingModule)
			End Get
		End Property

		Friend ReadOnly Property HasMultipleSpellings As Boolean
			Get
				Return (Me._lazyState And 1) <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of Location)(Me._declaration.NameLocations)
			End Get
		End Property

		Public ReadOnly Property MergedDeclaration As MergedNamespaceDeclaration
			Get
				Return Me._declaration
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._declaration.Name
			End Get
		End Property

		Private ReadOnly Property NameToMembersMap As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol))
			Get
				Return Me.GetNameToMembersMap()
			End Get
		End Property

		Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
			Get
				Dim empty As ImmutableArray(Of NamedTypeSymbol)
				If (Not Me._containingModule.MightContainExtensionMethods) Then
					empty = ImmutableArray(Of NamedTypeSymbol).Empty
				Else
					empty = Me.GetModuleMembers()
				End If
				Return empty
			End Get
		End Property

		Friend Sub New(ByVal decl As MergedNamespaceDeclaration, ByVal containingNamespace As SourceNamespaceSymbol, ByVal containingModule As SourceModuleSymbol)
			MyBase.New()
			Me._lazyEmbeddedKind = 1
			Me._lazyLexicalSortKey = LexicalSortKey.NotInitialized
			Me._declaration = decl
			Me._containingNamespace = containingNamespace
			Me._containingModule = containingModule
			If (containingNamespace IsNot Nothing AndAlso containingNamespace.HasMultipleSpellings OrElse decl.HasMultipleSpellings) Then
				Me._lazyState = 1
			End If
		End Sub

		Private Function BuildSymbol(ByVal decl As MergedNamespaceOrTypeDeclaration) As NamespaceOrTypeSymbol
			Dim sourceNamespaceSymbol As NamespaceOrTypeSymbol
			Dim mergedNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration = TryCast(decl, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceDeclaration)
			If (mergedNamespaceDeclaration Is Nothing) Then
				sourceNamespaceSymbol = SourceMemberContainerTypeSymbol.Create(DirectCast(decl, MergedTypeDeclaration), Me, Me._containingModule)
			Else
				sourceNamespaceSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol(mergedNamespaceDeclaration, Me, Me._containingModule)
			End If
			Return sourceNamespaceSymbol
		End Function

		Private Function ComputeDeclaringReferencesCore() As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxReference)
			Dim declarations As ImmutableArray(Of SingleNamespaceDeclaration) = Me._declaration.Declarations
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxReference) = ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxReference).GetInstance(declarations.Length)
			Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = enumerator.Current.SyntaxReference
				If (syntaxReference Is Nothing OrElse syntaxReference.SyntaxTree.IsEmbeddedOrMyTemplateTree()) Then
					Continue While
				End If
				instance.Add(New NamespaceDeclarationSyntaxReference(syntaxReference))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			Me.ValidateDeclaration(Nothing, cancellationToken)
			Me.GetMembers()
		End Sub

		Friend Sub GenerateDeclarationErrorsInTree(ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal cancellationToken As System.Threading.CancellationToken)
			Me.ValidateDeclaration(tree, cancellationToken)
			Me.GetMembers()
		End Sub

		Friend Function GetDeclarationSpelling(ByVal tree As SyntaxTree, ByVal location As Integer) As String
			Dim displayString As String
			Dim str As String
			Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration, Boolean)
			If (Me.HasMultipleSpellings) Then
				Dim singleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)(Me._declaration.Declarations, Function(decl As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)
					Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax = decl.GetNamespaceBlockSyntax()
					If (namespaceBlockSyntax Is Nothing OrElse namespaceBlockSyntax.SyntaxTree <> tree) Then
						Return False
					End If
					Return namespaceBlockSyntax.Span.Contains(location)
				End Function)
				If (singleNamespaceDeclaration Is Nothing) Then
					Dim declarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration) = Me._declaration.Declarations
					If (SourceNamespaceSymbol._Closure$__.$I56-1 Is Nothing) Then
						func = Function(decl As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration) decl.GetNamespaceBlockSyntax() Is Nothing
						SourceNamespaceSymbol._Closure$__.$I56-1 = func
					Else
						func = SourceNamespaceSymbol._Closure$__.$I56-1
					End If
					singleNamespaceDeclaration = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)(declarations, func)
				End If
				Dim str1 As String = If(singleNamespaceDeclaration IsNot Nothing, singleNamespaceDeclaration.Name, Me.Name)
				Dim containingNamespace As SourceNamespaceSymbol = TryCast(MyBase.ContainingNamespace, SourceNamespaceSymbol)
				str = If(containingNamespace Is Nothing OrElse EmbeddedOperators.CompareString(containingNamespace.Name, "", False) = 0, str1, [String].Concat(containingNamespace.GetDeclarationSpelling(tree, location), ".", str1))
				displayString = str
			Else
				displayString = MyBase.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
			End If
			Return displayString
		End Function

		Friend Overrides Function GetLexicalSortKey() As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey
			If (Not Me._lazyLexicalSortKey.IsInitialized) Then
				Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = Me._declaration.GetLexicalSortKey(Me.DeclaringCompilation)
				Me._lazyLexicalSortKey.SetFrom(lexicalSortKey)
			End If
			Return Me._lazyLexicalSortKey
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol)
			If ((Me._lazyState And 2) = 0) Then
				Dim membersUnordered As ImmutableArray(Of Symbol) = Me.GetMembersUnordered()
				If (membersUnordered.Length >= 2) Then
					membersUnordered = membersUnordered.Sort(LexicalOrderSymbolComparer.Instance)
					ImmutableInterlocked.InterlockedExchange(Of Symbol)(Me._lazyAllMembers, membersUnordered)
				End If
				ThreadSafeFlagOperations.[Set](Me._lazyState, 2)
				symbols = membersUnordered
			Else
				symbols = Me._lazyAllMembers
			End If
			Return symbols
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim namespaceOrTypeSymbols As ImmutableArray(Of NamespaceOrTypeSymbol) = New ImmutableArray(Of NamespaceOrTypeSymbol)()
			If (Not Me.GetNameToMembersMap().TryGetValue(name, namespaceOrTypeSymbols)) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = ImmutableArray(Of Symbol).CastUp(Of NamespaceOrTypeSymbol)(namespaceOrTypeSymbols)
			End If
			Return empty
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			If (Me._lazyAllMembers.IsDefault) Then
				Dim symbols As ImmutableArray(Of Symbol) = StaticCast(Of Symbol).From(Of NamespaceOrTypeSymbol)(Me.GetNameToMembersMap().Flatten(Nothing))
				Dim symbols1 As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
				ImmutableInterlocked.InterlockedCompareExchange(Of Symbol)(Me._lazyAllMembers, symbols, symbols1)
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ConditionallyDeOrder(Of Symbol)(Me._lazyAllMembers)
		End Function

		Public Overrides Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)
			If (Me._lazyModuleMembers.IsDefault) Then
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of MergedNamespaceOrTypeDeclaration).Enumerator = Me._declaration.Children.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MergedNamespaceOrTypeDeclaration = enumerator.Current
					If (current.Kind <> DeclarationKind.[Module]) Then
						Continue While
					End If
					instance.AddRange(Me.GetModuleMembers(current.Name))
				End While
				Dim immutableAndFree As ImmutableArray(Of NamedTypeSymbol) = instance.ToImmutableAndFree()
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
				ImmutableInterlocked.InterlockedCompareExchange(Of NamedTypeSymbol)(Me._lazyModuleMembers, immutableAndFree, namedTypeSymbols)
			End If
			Return Me._lazyModuleMembers
		End Function

		Private Function GetNameToMembersMap() As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol))
			If (Me._nameToMembersMap Is Nothing) Then
				Dim membersMap As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)) = Me.MakeNameToMembersMap()
				If (Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)))(Me._nameToMembersMap, membersMap, Nothing) Is Nothing) Then
					Me.RegisterDeclaredCorTypes()
				End If
			End If
			Return Me._nameToMembersMap
		End Function

		Private Function GetNameToTypeMembersMap() As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)).Enumerator = New Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)).Enumerator()
			If (Me._nameToTypeMembersMap Is Nothing) Then
				Dim strs As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)) = New Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))(CaseInsensitiveComparison.Comparer)
				Dim nameToMembersMap As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)) = Me.GetNameToMembersMap()
				Try
					enumerator = nameToMembersMap.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)) = enumerator.Current
						Dim value As ImmutableArray(Of NamespaceOrTypeSymbol) = current.Value
						Dim flag As Boolean = False
						Dim flag1 As Boolean = False
						Dim enumerator1 As ImmutableArray(Of NamespaceOrTypeSymbol).Enumerator = value.GetEnumerator()
						Do
						Label0:
							If (Not enumerator1.MoveNext()) Then
								Exit Do
							End If
							If (enumerator1.Current.Kind <> SymbolKind.NamedType) Then
								flag1 = True
							Else
								flag = True
								If (flag1) Then
									Exit Do
								Else
									GoTo Label0
								End If
							End If
						Loop While Not flag
						If (Not flag) Then
							Continue While
						End If
						If (Not flag1) Then
							strs.Add(current.Key, value.[As](Of NamedTypeSymbol)())
						Else
							strs.Add(current.Key, value.OfType(Of NamedTypeSymbol)().AsImmutable())
						End If
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)))(Me._nameToTypeMembersMap, strs, Nothing)
			End If
			Return Me._nameToTypeMembersMap
		End Function

		Private Function GetSourcePathForDeclaration() As Object
			Dim localizableErrorArgument As Object = Nothing
			Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = Me._declaration.Declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SingleNamespaceDeclaration = enumerator.Current
				If ([String].Compare(Me.Name, current.Name, StringComparison.Ordinal) <> 0) Then
					Continue While
				End If
				If (Not current.IsPartOfRootNamespace) Then
					If (current.SyntaxReference Is Nothing OrElse current.SyntaxReference.SyntaxTree.FilePath Is Nothing) Then
						Continue While
					End If
					Dim filePath As String = current.SyntaxReference.SyntaxTree.FilePath
					If (localizableErrorArgument IsNot Nothing) Then
						If ([String].Compare(localizableErrorArgument.ToString(), filePath.ToString(), StringComparison.Ordinal) <= 0) Then
							Continue While
						End If
						localizableErrorArgument = filePath
					Else
						localizableErrorArgument = filePath
					End If
				Else
					localizableErrorArgument = New Microsoft.CodeAnalysis.VisualBasic.LocalizableErrorArgument(ERRID.IDS_ProjectSettingsLocationName)
				End If
			End While
			Return localizableErrorArgument
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.GetNameToTypeMembersMap().Flatten(LexicalOrderSymbolComparer.Instance)
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
			If (Not Me.GetNameToTypeMembersMap().TryGetValue(name, namedTypeSymbols)) Then
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			Else
				empty = namedTypeSymbols
			End If
			Return empty
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.GetNameToTypeMembersMap().Flatten(Nothing)
		End Function

		Friend Overrides Function IsDefinedInSourceTree(ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Dim flag As Boolean
			If (Not Me.IsGlobalNamespace) Then
				Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = Me._declaration.Declarations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SingleNamespaceDeclaration = enumerator.Current
					cancellationToken.ThrowIfCancellationRequested()
					Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = current.SyntaxReference
					If (syntaxReference Is Nothing OrElse syntaxReference.SyntaxTree <> tree) Then
						If (Not current.IsPartOfRootNamespace) Then
							Continue While
						End If
						flag = True
						Return flag
					Else
						If (syntaxReference.SyntaxTree.IsEmbeddedOrMyTemplateTree()) Then
							Continue While
						End If
						Dim syntax As SyntaxNode = (New NamespaceDeclarationSyntaxReference(syntaxReference)).GetSyntax(cancellationToken)
						If (TypeOf syntax Is NamespaceStatementSyntax) Then
							syntax = syntax.Parent
						End If
						If (Not Symbol.IsDefinedInSourceTree(syntax, tree, definedWithinSpan, cancellationToken)) Then
							Continue While
						End If
						flag = True
						Return flag
					End If
				End While
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function MakeNameToMembersMap() As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol))
			Dim nameToSymbolMapBuilder As SourceNamespaceSymbol.NameToSymbolMapBuilder = New SourceNamespaceSymbol.NameToSymbolMapBuilder(Me._declaration.Children.Length)
			Dim enumerator As ImmutableArray(Of MergedNamespaceOrTypeDeclaration).Enumerator = Me._declaration.Children.GetEnumerator()
			While enumerator.MoveNext()
				nameToSymbolMapBuilder.Add(Me.BuildSymbol(enumerator.Current))
			End While
			Return nameToSymbolMapBuilder.CreateMap()
		End Function

		Private Sub RegisterDeclaredCorTypes()
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of NamespaceOrTypeSymbol)).ValueCollection.Enumerator()
			Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
			If (containingAssembly.KeepLookingForDeclaredSpecialTypes) Then
				Try
					enumerator = Me._nameToMembersMap.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim enumerator1 As ImmutableArray(Of NamespaceOrTypeSymbol).Enumerator = enumerator.Current.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As NamedTypeSymbol = TryCast(enumerator1.Current, NamedTypeSymbol)
							If (current Is Nothing OrElse current.SpecialType = SpecialType.None) Then
								Continue While
							End If
							containingAssembly.RegisterDeclaredSpecialType(current)
							If (containingAssembly.KeepLookingForDeclaredSpecialTypes) Then
								Continue While
							End If
							Return
						End While
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
		End Sub

		Private Sub ValidateDeclaration(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal cancellationToken As System.Threading.CancellationToken)
			If ((Me._lazyState And 4) = 0) Then
				Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
				Dim flag As Boolean = False
				Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = Me._declaration.SyntaxReferences.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxReference = enumerator.Current
					If (tree IsNot Nothing AndAlso current.SyntaxTree <> tree) Then
						Continue While
					End If
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = current.SyntaxTree
					Dim visualBasicSyntax As VisualBasicSyntaxNode = current.GetVisualBasicSyntax(cancellationToken)
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = visualBasicSyntax.Kind()
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName
								Me.ValidateNamespaceNameSyntax(DirectCast(visualBasicSyntax, IdentifierNameSyntax), instance, flag)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GenericName
								Throw ExceptionUtilities.UnexpectedValue(visualBasicSyntax.Kind())
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName
								Me.ValidateNamespaceNameSyntax(DirectCast(visualBasicSyntax, QualifiedNameSyntax).Right, instance, flag)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalName
								Me.ValidateNamespaceGlobalSyntax(DirectCast(visualBasicSyntax, GlobalNameSyntax), instance)
								Exit Select
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(visualBasicSyntax.Kind())
						End Select
					End If
					cancellationToken.ThrowIfCancellationRequested()
				End While
				If (Me._containingModule.AtomicSetFlagAndStoreDiagnostics(Me._lazyState, 4, 0, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(instance))) Then
					Me.DeclaringCompilation.SymbolDeclaredEvent(Me)
				End If
				instance.Free()
			End If
		End Sub

		Private Sub ValidateNamespaceGlobalSyntax(ByVal node As GlobalNameSyntax, ByVal diagnostics As DiagnosticBag)
			Dim parent As VisualBasicSyntaxNode = node.Parent
			Dim flag As Boolean = False
			While parent IsNot Nothing
				If (parent.Kind() = SyntaxKind.NamespaceBlock) Then
					If (Not flag) Then
						flag = True
					Else
						Dim vBDiagnostic As Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NestedGlobalNamespace), node.GetLocation(), False)
						diagnostics.Add(vBDiagnostic)
					End If
				End If
				parent = parent.Parent
			End While
		End Sub

		Private Sub ValidateNamespaceNameSyntax(ByVal node As SimpleNameSyntax, ByVal diagnostics As DiagnosticBag, ByRef reportedNamespaceMismatch As Boolean)
			If (node.Identifier.GetTypeCharacter() <> TypeCharacter.None) Then
				Dim vBDiagnostic As Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_TypecharNotallowed), node.GetLocation(), False)
				diagnostics.Add(vBDiagnostic)
			End If
			If (Not reportedNamespaceMismatch AndAlso [String].Compare(node.Identifier.ValueText, Me.Name, StringComparison.Ordinal) <> 0) Then
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetSourcePathForDeclaration())
				Dim vBDiagnostic1 As Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_NamespaceCaseMismatch3, New [Object]() { node.Identifier.ValueText, Me.Name, objectValue }), node.GetLocation(), False)
				diagnostics.Add(vBDiagnostic1)
				reportedNamespaceMismatch = True
			End If
		End Sub

		Private Structure NameToSymbolMapBuilder
			Private ReadOnly _dictionary As Dictionary(Of String, Object)

			Public Sub New(ByVal capacity As Integer)
				Me = New SourceNamespaceSymbol.NameToSymbolMapBuilder() With
				{
					._dictionary = New Dictionary(Of String, Object)(capacity, CaseInsensitiveComparison.Comparer)
				}
			End Sub

			Public Sub Add(ByVal symbol As NamespaceOrTypeSymbol)
				Dim name As String = symbol.Name
				Dim obj As Object = Nothing
				If (Not Me._dictionary.TryGetValue(name, obj)) Then
					Me._dictionary(name) = symbol
					Return
				End If
				Dim instance As ArrayBuilder(Of NamespaceOrTypeSymbol) = TryCast(obj, ArrayBuilder(Of NamespaceOrTypeSymbol))
				If (instance Is Nothing) Then
					instance = ArrayBuilder(Of NamespaceOrTypeSymbol).GetInstance()
					instance.Add(DirectCast(obj, NamespaceOrTypeSymbol))
					Me._dictionary(name) = instance
				End If
				instance.Add(symbol)
			End Sub

			Public Function CreateMap() As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol))
				Dim enumerator As Dictionary(Of String, Object).Enumerator = New Dictionary(Of String, Object).Enumerator()
				Dim namespaceOrTypeSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)
				Dim strs As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)) = New Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol))(Me._dictionary.Count, CaseInsensitiveComparison.Comparer)
				Try
					enumerator = Me._dictionary.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, Object) = enumerator.Current
						Dim objectValue As Object = RuntimeHelpers.GetObjectValue(current.Value)
						Dim namespaceOrTypeSymbols1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol) = TryCast(objectValue, ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol))
						If (namespaceOrTypeSymbols1 Is Nothing) Then
							Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = DirectCast(objectValue, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)
							namespaceOrTypeSymbols = If(namespaceOrTypeSymbol.Kind <> SymbolKind.[Namespace], StaticCast(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol).From(Of NamedTypeSymbol)(ImmutableArray.Create(Of NamedTypeSymbol)(DirectCast(namespaceOrTypeSymbol, NamedTypeSymbol))), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)(namespaceOrTypeSymbol))
						Else
							Dim flag As Boolean = False
							Dim count As Integer = namespaceOrTypeSymbols1.Count - 1
							Dim num As Integer = 0
							While num <= count
								If (namespaceOrTypeSymbols1(num).Kind <> SymbolKind.[Namespace]) Then
									num = num + 1
								Else
									flag = True
									Exit While
								End If
							End While
							namespaceOrTypeSymbols = If(Not flag, StaticCast(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol).From(Of NamedTypeSymbol)(namespaceOrTypeSymbols1.ToDowncastedImmutable(Of NamedTypeSymbol)()), namespaceOrTypeSymbols1.ToImmutable())
							namespaceOrTypeSymbols1.Free()
						End If
						strs.Add(current.Key, namespaceOrTypeSymbols)
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Return strs
			End Function
		End Structure

		<Flags>
		Private Enum StateFlags
			HasMultipleSpellings = 1
			AllMembersIsSorted = 2
			DeclarationValidated = 4
		End Enum
	End Class
End Namespace