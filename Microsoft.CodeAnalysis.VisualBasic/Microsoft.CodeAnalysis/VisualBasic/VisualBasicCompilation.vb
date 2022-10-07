Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.InternalUtilities
Imports Microsoft.CodeAnalysis.Operations
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Resources
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports System.Xml.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public NotInheritable Class VisualBasicCompilation
		Inherits Compilation
		Private ReadOnly Shared s_myTemplateCache As ConcurrentLruCache(Of VisualBasicParseOptions, SyntaxTree)

		Private _lazyAssemblySymbol As SourceAssemblySymbol

		Private _referenceManager As VisualBasicCompilation.ReferenceManager

		Private ReadOnly _options As VisualBasicCompilationOptions

		Private _lazyGlobalNamespace As NamespaceSymbol

		Private ReadOnly _syntaxTrees As ImmutableArray(Of SyntaxTree)

		Private ReadOnly _syntaxTreeOrdinalMap As ImmutableDictionary(Of SyntaxTree, Integer)

		Private _lazyAllSyntaxTrees As ImmutableArray(Of SyntaxTree)

		Private ReadOnly _rootNamespaces As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry)

		Private _lazyImportInfos As ConcurrentQueue(Of VisualBasicCompilation.ImportInfo)

		<TupleElementNames(New String() { "SyntaxTree", "ImportsClausePosition" })>
		Private _lazyImportClauseDependencies As ConcurrentDictionary(Of ValueTuple(Of SyntaxTree, Integer), ImmutableArray(Of AssemblySymbol))

		Private _lazyClsComplianceDiagnostics As ImmutableArray(Of Diagnostic)

		Private _lazyClsComplianceDependencies As ImmutableArray(Of AssemblySymbol)

		Private ReadOnly _embeddedTrees As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration)

		Private ReadOnly _declarationTable As DeclarationTable

		Private ReadOnly _anonymousTypeManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager

		Private _lazyEmbeddedSymbolManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager

		Private _lazyMyTemplate As SyntaxTree

		Private ReadOnly _scriptClass As Lazy(Of ImplicitNamedTypeSymbol)

		Private _lazyEntryPoint As VisualBasicCompilation.EntryPoint

		Private _lazyCompilationUnitCompletedTrees As HashSet(Of SyntaxTree)

		Private ReadOnly _languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion

		Private _lazyUsedAssemblyReferences As ConcurrentSet(Of AssemblySymbol)

		Private _usedAssemblyReferencesFrozen As Boolean

		Private ReadOnly _wellKnownMemberSignatureComparer As VisualBasicCompilation.WellKnownMembersSignatureComparer

		Private _lazyWellKnownTypes As NamedTypeSymbol()

		Private _lazyWellKnownTypeMembers As Symbol()

		Private _lazyExtensionAttributeConstructor As Symbol

		Private _lazyExtensionAttributeConstructorErrorInfo As Object

		Friend ReadOnly Property AliasImports As ImmutableArray(Of AliasSymbol)
			Get
				Dim [alias] As Func(Of AliasAndImportsClausePosition, AliasSymbol)
				Dim aliasAndImportsClausePositions As ImmutableArray(Of AliasAndImportsClausePosition) = DirectCast(Me.SourceModule, SourceModuleSymbol).AliasImports
				If (VisualBasicCompilation._Closure$__.$I137-0 Is Nothing) Then
					[alias] = Function(a As AliasAndImportsClausePosition) a.[Alias]
					VisualBasicCompilation._Closure$__.$I137-0 = [alias]
				Else
					[alias] = VisualBasicCompilation._Closure$__.$I137-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AliasAndImportsClausePosition, AliasSymbol)(aliasAndImportsClausePositions, [alias])
			End Get
		End Property

		Friend ReadOnly Property AllSyntaxTrees As ImmutableArray(Of SyntaxTree)
			Get
				If (Me._lazyAllSyntaxTrees.IsDefault) Then
					Dim instance As ArrayBuilder(Of SyntaxTree) = ArrayBuilder(Of SyntaxTree).GetInstance()
					instance.AddRange(Me._syntaxTrees)
					Dim enumerator As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration).Enumerator = Me._embeddedTrees.GetEnumerator()
					While enumerator.MoveNext()
						Dim value As SyntaxTree = enumerator.Current.Tree.Value
						If (value Is Nothing) Then
							Continue While
						End If
						instance.Add(value)
					End While
					ImmutableInterlocked.InterlockedInitialize(Of SyntaxTree)(Me._lazyAllSyntaxTrees, instance.ToImmutableAndFree())
				End If
				Return Me._lazyAllSyntaxTrees
			End Get
		End Property

		Friend ReadOnly Property AnonymousTypeManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager
			Get
				Return Me._anonymousTypeManager
			End Get
		End Property

		Friend Shadows ReadOnly Property Assembly As AssemblySymbol
			Get
				Return Me.SourceAssembly
			End Get
		End Property

		Friend Overrides ReadOnly Property CommonAnonymousTypeManager As Microsoft.CodeAnalysis.Symbols.CommonAnonymousTypeManager
			Get
				Return Me._anonymousTypeManager
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonAssembly As IAssemblySymbol
			Get
				Return Me.Assembly
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonDynamicType As ITypeSymbol
			Get
				Throw New NotSupportedException(VBResources.ThereIsNoDynamicTypeInVB)
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonGlobalNamespace As INamespaceSymbol
			Get
				Return Me.GlobalNamespace
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonObjectType As INamedTypeSymbol
			Get
				Return Me.ObjectType
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonOptions As CompilationOptions
			Get
				Return Me.Options
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonScriptClass As INamedTypeSymbol
			Get
				Return Me.ScriptClass
			End Get
		End Property

		Friend Overrides ReadOnly Property CommonScriptCompilationInfo As Microsoft.CodeAnalysis.ScriptCompilationInfo
			Get
				Return Me.ScriptCompilationInfo
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonScriptGlobalsType As ITypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonSourceModule As IModuleSymbol
			Get
				Return Me.SourceModule
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonSyntaxTrees As ImmutableArray(Of SyntaxTree)
			Get
				Return Me.SyntaxTrees
			End Get
		End Property

		Friend Overrides ReadOnly Property DebugSourceDocumentLanguageId As Guid
			Get
				Return DebugSourceDocument.CorSymLanguageTypeBasic
			End Get
		End Property

		Friend ReadOnly Property Declarations As DeclarationTable
			Get
				Return Me._declarationTable
			End Get
		End Property

		Friend ReadOnly Property DeclaresTheObjectClass As Boolean
			Get
				Return Me.SourceAssembly.DeclaresTheObjectClass
			End Get
		End Property

		Public Overrides ReadOnly Property DirectiveReferences As ImmutableArray(Of MetadataReference)
			Get
				Return Me.GetBoundReferenceManager().DirectiveReferences
			End Get
		End Property

		Friend ReadOnly Property EmbeddedSymbolManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager
			Get
				If (Me._lazyEmbeddedSymbolManager Is Nothing) Then
					Dim embeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind = If(Me.Options.EmbedVbCoreRuntime, 4, 0) Or If(Me.IncludeInternalXmlHelper(), 8, 0)
					If (embeddedSymbolKind <> Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None) Then
						embeddedSymbolKind = embeddedSymbolKind Or Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.EmbeddedAttribute
					End If
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager)(Me._lazyEmbeddedSymbolManager, New Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager(embeddedSymbolKind), Nothing)
				End If
				Return Me._lazyEmbeddedSymbolManager
			End Get
		End Property

		Friend ReadOnly Property EnableEnumArrayBlockInitialization As Boolean
			Get
				Dim wellKnownTypeMember As Symbol = Me.GetWellKnownTypeMember(WellKnownMember.System_Runtime_GCLatencyMode__SustainedLowLatency)
				If (wellKnownTypeMember Is Nothing) Then
					Return False
				End If
				Return wellKnownTypeMember.ContainingAssembly = Me.Assembly.CorLibrary
			End Get
		End Property

		Friend ReadOnly Property FeatureStrictEnabled As Boolean
			Get
				Return CObj(MyBase.Feature("strict")) <> CObj(Nothing)
			End Get
		End Property

		Friend Shadows ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				If (Me._lazyGlobalNamespace Is Nothing) Then
					Interlocked.CompareExchange(Of NamespaceSymbol)(Me._lazyGlobalNamespace, MergedNamespaceSymbol.CreateGlobalNamespace(Me), Nothing)
				End If
				Return Me._lazyGlobalNamespace
			End Get
		End Property

		Friend ReadOnly Property HasTupleNamesAttributes As Boolean
			Get
				Dim wellKnownTypeMember As MethodSymbol = TryCast(Me.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames), MethodSymbol)
				If (wellKnownTypeMember Is Nothing) Then
					Return False
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForWellKnownTypeMember(wellKnownTypeMember, WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, False).DiagnosticInfo Is Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsCaseSensitive As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsDelaySigned As Boolean
			Get
				Return Me.SourceAssembly.IsDelaySigned
			End Get
		End Property

		Public Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Public ReadOnly Property LanguageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Get
				Return Me._languageVersion
			End Get
		End Property

		Friend Overrides ReadOnly Property LinkerMajorVersion As Byte
			Get
				Return CByte(80)
			End Get
		End Property

		Friend ReadOnly Property MemberImports As ImmutableArray(Of NamespaceOrTypeSymbol)
			Get
				Dim namespaceOrType As Func(Of NamespaceOrTypeAndImportsClausePosition, NamespaceOrTypeSymbol)
				Dim namespaceOrTypeAndImportsClausePositions As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition) = DirectCast(Me.SourceModule, SourceModuleSymbol).MemberImports
				If (VisualBasicCompilation._Closure$__.$I135-0 Is Nothing) Then
					namespaceOrType = Function(m As NamespaceOrTypeAndImportsClausePosition) m.NamespaceOrType
					VisualBasicCompilation._Closure$__.$I135-0 = namespaceOrType
				Else
					namespaceOrType = VisualBasicCompilation._Closure$__.$I135-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamespaceOrTypeAndImportsClausePosition, NamespaceOrTypeSymbol)(namespaceOrTypeAndImportsClausePositions, namespaceOrType)
			End Get
		End Property

		Friend ReadOnly Property MergedRootDeclaration As MergedNamespaceDeclaration
			Get
				Return Me.Declarations.GetMergedRoot(Me)
			End Get
		End Property

		Friend Overrides ReadOnly Property MessageProvider As CommonMessageProvider
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
			End Get
		End Property

		Friend Property MyTemplate As SyntaxTree
			Get
				If (Me._lazyMyTemplate = VisualBasicSyntaxTree.Dummy) Then
					Dim options As VisualBasicCompilationOptions = Me.Options
					If (options.EmbedVbCoreRuntime OrElse options.SuppressEmbeddedDeclarations) Then
						Me._lazyMyTemplate = Nothing
					Else
						Dim parseOptions As VisualBasicParseOptions = If(options.ParseOptions, VisualBasicParseOptions.[Default])
						Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Nothing
						If (Not VisualBasicCompilation.s_myTemplateCache.TryGetValue(parseOptions, syntaxTree)) Then
							Dim vbMyTemplateText As String = EmbeddedResources.VbMyTemplateText
							Dim visualBasicParseOption As VisualBasicParseOptions = parseOptions.WithLanguageVersion(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default])
							Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
							syntaxTree = VisualBasicSyntaxTree.ParseText(vbMyTemplateText, True, visualBasicParseOption, "", Nothing, Nothing, cancellationToken)
							cancellationToken = New System.Threading.CancellationToken()
							If (syntaxTree.GetDiagnostics(cancellationToken).Any()) Then
								Throw ExceptionUtilities.Unreachable
							End If
							If (Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.SyntaxTree)(Me._lazyMyTemplate, syntaxTree, VisualBasicSyntaxTree.Dummy) = VisualBasicSyntaxTree.Dummy) Then
								VisualBasicCompilation.s_myTemplateCache(parseOptions) = syntaxTree
							End If
						Else
							Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.SyntaxTree)(Me._lazyMyTemplate, syntaxTree, VisualBasicSyntaxTree.Dummy)
						End If
					End If
				End If
				Return Me._lazyMyTemplate
			End Get
			Set(ByVal value As SyntaxTree)
				Dim flag As Boolean
				If (value IsNot Nothing) Then
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					flag = value.GetDiagnostics(cancellationToken).Any()
				Else
					flag = False
				End If
				If (flag) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Me._lazyMyTemplate = value
			End Set
		End Property

		Friend Shadows ReadOnly Property ObjectType As NamedTypeSymbol
			Get
				Return Me.Assembly.ObjectType
			End Get
		End Property

		Public Shadows ReadOnly Property Options As VisualBasicCompilationOptions
			Get
				Return Me._options
			End Get
		End Property

		Friend ReadOnly Property PreviousSubmission As VisualBasicCompilation
			Get
				Dim scriptCompilationInfo As VisualBasicScriptCompilationInfo = Me.ScriptCompilationInfo
				If (scriptCompilationInfo IsNot Nothing) Then
					Return scriptCompilationInfo.PreviousScriptCompilation
				End If
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property ReferencedAssemblyNames As IEnumerable(Of AssemblyIdentity)
			Get
				Dim referencedAssemblies As Func(Of ModuleSymbol, IEnumerable(Of AssemblyIdentity))
				Dim modules As IEnumerable(Of ModuleSymbol) = DirectCast(Me.Assembly.Modules, IEnumerable(Of ModuleSymbol))
				If (VisualBasicCompilation._Closure$__.$I107-0 Is Nothing) Then
					referencedAssemblies = Function(m As ModuleSymbol) DirectCast(m.GetReferencedAssemblies(), IEnumerable(Of AssemblyIdentity))
					VisualBasicCompilation._Closure$__.$I107-0 = referencedAssemblies
				Else
					referencedAssemblies = VisualBasicCompilation._Closure$__.$I107-0
				End If
				Return modules.SelectMany(Of AssemblyIdentity)(referencedAssemblies)
			End Get
		End Property

		<TupleElementNames(New String() { "path", "content" })>
		Friend Overrides ReadOnly Property ReferenceDirectiveMap As <TupleElementNames(New String() { "path", "content" })> IDictionary(Of ValueTuple(Of String, String), MetadataReference)
			Get
				Return Me.GetBoundReferenceManager().ReferenceDirectiveMap
			End Get
		End Property

		Friend Overrides ReadOnly Property ReferenceDirectives As IEnumerable(Of ReferenceDirective)
			Get
				Return Me._declarationTable.ReferenceDirectives
			End Get
		End Property

		Friend ReadOnly Property RootNamespace As NamespaceSymbol
			Get
				Return DirectCast(Me.SourceModule, SourceModuleSymbol).RootNamespace
			End Get
		End Property

		Friend Shadows ReadOnly Property ScriptClass As NamedTypeSymbol
			Get
				Return Me.SourceScriptClass
			End Get
		End Property

		Friend Shadows ReadOnly Property ScriptCompilationInfo As VisualBasicScriptCompilationInfo

		Friend ReadOnly Property SourceAssembly As SourceAssemblySymbol
			Get
				Me.GetBoundReferenceManager()
				Return Me._lazyAssemblySymbol
			End Get
		End Property

		Friend Shadows ReadOnly Property SourceModule As ModuleSymbol
			Get
				Return Me.Assembly.Modules(0)
			End Get
		End Property

		Friend ReadOnly Property SourceScriptClass As ImplicitNamedTypeSymbol
			Get
				Return Me._scriptClass.Value
			End Get
		End Property

		Friend Overrides ReadOnly Property StrongNameKeys As Microsoft.CodeAnalysis.StrongNameKeys
			Get
				Return Me.SourceAssembly.StrongNameKeys
			End Get
		End Property

		Public Shadows ReadOnly Property SyntaxTrees As ImmutableArray(Of SyntaxTree)
			Get
				Return Me._syntaxTrees
			End Get
		End Property

		Shared Sub New()
			VisualBasicCompilation.s_myTemplateCache = New ConcurrentLruCache(Of VisualBasicParseOptions, SyntaxTree)(5)
		End Sub

		Private Sub New(ByVal assemblyName As String, ByVal options As VisualBasicCompilationOptions, ByVal references As ImmutableArray(Of MetadataReference), ByVal syntaxTrees As ImmutableArray(Of SyntaxTree), ByVal syntaxTreeOrdinalMap As ImmutableDictionary(Of SyntaxTree, Integer), ByVal rootNamespaces As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry), ByVal embeddedTrees As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration), ByVal declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable, ByVal previousSubmission As VisualBasicCompilation, ByVal submissionReturnType As Type, ByVal hostObjectType As Type, ByVal isSubmission As Boolean, ByVal referenceManager As VisualBasicCompilation.ReferenceManager, ByVal reuseReferenceManager As Boolean, ByVal semanticModelProvider As Microsoft.CodeAnalysis.SemanticModelProvider, Optional ByVal eventQueue As AsyncQueue(Of CompilationEvent) = Nothing)
			MyBase.New(assemblyName, references, Compilation.SyntaxTreeCommonFeatures(DirectCast(syntaxTrees, IEnumerable(Of SyntaxTree))), isSubmission, semanticModelProvider, eventQueue)
			Dim observedMetadata As Dictionary(Of MetadataReference, Object)
			Me._lazyMyTemplate = VisualBasicSyntaxTree.Dummy
			Me._wellKnownMemberSignatureComparer = New VisualBasicCompilation.WellKnownMembersSignatureComparer(Me)
			Me._lazyExtensionAttributeConstructor = ErrorTypeSymbol.UnknownResultType
			Me._options = options
			Me._syntaxTrees = syntaxTrees
			Me._syntaxTreeOrdinalMap = syntaxTreeOrdinalMap
			Me._rootNamespaces = rootNamespaces
			Me._embeddedTrees = embeddedTrees
			Me._declarationTable = declarationTable
			Me._anonymousTypeManager = New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager(Me)
			Me._languageVersion = Me.CommonLanguageVersion(syntaxTrees)
			Me._scriptClass = New Lazy(Of ImplicitNamedTypeSymbol)(New Func(Of ImplicitNamedTypeSymbol)(AddressOf Me.BindScriptClass))
			If (isSubmission) Then
				Me.ScriptCompilationInfo = New VisualBasicScriptCompilationInfo(previousSubmission, submissionReturnType, hostObjectType)
			End If
			If (Not reuseReferenceManager) Then
				Dim str As String = MyBase.MakeSourceAssemblySimpleName()
				Dim assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer = options.AssemblyIdentityComparer
				If (referenceManager IsNot Nothing) Then
					observedMetadata = referenceManager.ObservedMetadata
				Else
					observedMetadata = Nothing
				End If
				Me._referenceManager = New VisualBasicCompilation.ReferenceManager(str, assemblyIdentityComparer, observedMetadata)
			Else
				Me._referenceManager = referenceManager
			End If
			If (MyBase.EventQueue IsNot Nothing) Then
				MyBase.EventQueue.TryEnqueue(New CompilationStartedEvent(Me))
			End If
		End Sub

		Private Sub AddClsComplianceDiagnostics(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal filterTree As SyntaxTree = Nothing, Optional ByVal filterSpanWithinTree As Nullable(Of TextSpan) = Nothing)
			If (filterTree IsNot Nothing) Then
				ClsComplianceChecker.CheckCompliance(Me, diagnostics, cancellationToken, filterTree, filterSpanWithinTree)
				Return
			End If
			If (Me._lazyClsComplianceDiagnostics.IsDefault OrElse Me._lazyClsComplianceDependencies.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				ClsComplianceChecker.CheckCompliance(Me, instance, cancellationToken, Nothing, Nothing)
				Dim readOnlyAndFree As ImmutableBindingDiagnostic(Of AssemblySymbol) = instance.ToReadOnlyAndFree()
				ImmutableInterlocked.InterlockedInitialize(Of AssemblySymbol)(Me._lazyClsComplianceDependencies, readOnlyAndFree.Dependencies)
				ImmutableInterlocked.InterlockedInitialize(Of Diagnostic)(Me._lazyClsComplianceDiagnostics, readOnlyAndFree.Diagnostics)
			End If
			diagnostics.AddRange(New ImmutableBindingDiagnostic(Of AssemblySymbol)(Me._lazyClsComplianceDiagnostics, Me._lazyClsComplianceDependencies), True)
		End Sub

		Friend Overrides Sub AddDebugSourceDocumentsForChecksumDirectives(ByVal documentsBuilder As DebugDocumentsBuilder, ByVal tree As SyntaxTree, ByVal diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag)
			Dim enumerator As IEnumerator(Of DirectiveTriviaSyntax) = Nothing
			Dim func As Func(Of DirectiveTriviaSyntax, Boolean)
			Dim root As SyntaxNode = tree.GetRoot(New CancellationToken())
			If (VisualBasicCompilation._Closure$__.$I204-0 Is Nothing) Then
				func = Function(d As DirectiveTriviaSyntax)
					If (d.Kind() <> SyntaxKind.ExternalChecksumDirectiveTrivia) Then
						Return False
					End If
					Return Not d.ContainsDiagnostics
				End Function
				VisualBasicCompilation._Closure$__.$I204-0 = func
			Else
				func = VisualBasicCompilation._Closure$__.$I204-0
			End If
			Using directives As IList(Of DirectiveTriviaSyntax) = root.GetDirectives(func)
				enumerator = directives.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ExternalChecksumDirectiveTriviaSyntax = DirectCast(enumerator.Current, ExternalChecksumDirectiveTriviaSyntax)
					Dim valueText As String = current.ExternalSource.ValueText
					Dim str As String = current.Checksum.ValueText
					Dim str1 As String = documentsBuilder.NormalizeDebugDocumentPath(valueText, tree.FilePath)
					Dim debugSourceDocument As Microsoft.Cci.DebugSourceDocument = documentsBuilder.TryGetDebugDocumentForNormalizedPath(str1)
					If (debugSourceDocument Is Nothing) Then
						Dim corSymLanguageTypeBasic As Guid = Microsoft.Cci.DebugSourceDocument.CorSymLanguageTypeBasic
						Dim checksum As SyntaxToken = current.Checksum
						Dim nums As ImmutableArray(Of Byte) = VisualBasicCompilation.MakeCheckSumBytes(checksum.ValueText)
						checksum = current.Guid
						Dim debugSourceDocument1 As Microsoft.Cci.DebugSourceDocument = New Microsoft.Cci.DebugSourceDocument(str1, corSymLanguageTypeBasic, nums, Guid.Parse(checksum.ValueText))
						documentsBuilder.AddDebugDocument(debugSourceDocument1)
					Else
						If (debugSourceDocument.IsComputedChecksum) Then
							Continue While
						End If
						Dim sourceInfo As DebugSourceInfo = debugSourceDocument.GetSourceInfo()
						If (VisualBasicCompilation.CheckSumMatches(str, sourceInfo.Checksum) AndAlso Guid.Parse(current.Guid.ValueText) = sourceInfo.ChecksumAlgorithmId) Then
							Continue While
						End If
						diagnosticBag.Add(ERRID.WRN_MultipleDeclFileExtChecksum, New SourceLocation(current), New [Object]() { valueText })
					End If
				End While
			End Using
		End Sub

		Private Function AddedModulesResourceNames(ByVal diagnostics As DiagnosticBag) As IEnumerable(Of String)
			Return New VisualBasicCompilation.VB$StateMachine_201_AddedModulesResourceNames(-2) With
			{
				.$VB$Me = Me,
				.$P_diagnostics = diagnostics
			}
		End Function

		Private Shared Function AddEmbeddedTrees(ByVal declTable As DeclarationTable, ByVal embeddedTrees As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration)) As DeclarationTable
			Dim enumerator As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration).Enumerator = embeddedTrees.GetEnumerator()
			While enumerator.MoveNext()
				declTable = declTable.AddRootDeclaration(enumerator.Current.DeclarationEntry)
			End While
			Return declTable
		End Function

		Private Sub AddImportsDependencies(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal infoTree As SyntaxTree, ByVal clauseSpan As TextSpan)
			Dim assemblySymbols As ImmutableArray(Of AssemblySymbol) = New ImmutableArray(Of AssemblySymbol)()
			If (diagnostics.AccumulatesDependencies AndAlso Me._lazyImportClauseDependencies IsNot Nothing AndAlso Me._lazyImportClauseDependencies.TryGetValue(New ValueTuple(Of SyntaxTree, Integer)(infoTree, clauseSpan.Start), assemblySymbols)) Then
				diagnostics.AddDependencies(assemblySymbols)
			End If
		End Sub

		Private Sub AddReferencedAssemblies(ByVal assembly As AssemblySymbol, ByVal includeMainModule As Boolean, ByVal stack As ArrayBuilder(Of AssemblySymbol))
			Dim num As Integer
			num = If(includeMainModule, 0, 1)
			Dim modules As ImmutableArray(Of ModuleSymbol) = assembly.Modules
			Dim length As Integer = modules.Length - 1
			For i As Integer = num To length
				modules = assembly.Modules
				Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = modules(i).ReferencedAssemblySymbols.GetEnumerator()
				While enumerator.MoveNext()
					Me.AddUsedAssembly(enumerator.Current, stack)
				End While
			Next

		End Sub

		Public Shadows Function AddReferences(ByVal ParamArray references As MetadataReference()) As VisualBasicCompilation
			Return DirectCast(MyBase.AddReferences(references), VisualBasicCompilation)
		End Function

		Public Shadows Function AddReferences(ByVal references As IEnumerable(Of MetadataReference)) As VisualBasicCompilation
			Return DirectCast(MyBase.AddReferences(references), VisualBasicCompilation)
		End Function

		Public Shadows Function AddSyntaxTrees(ByVal ParamArray trees As SyntaxTree()) As VisualBasicCompilation
			Return Me.AddSyntaxTrees(DirectCast(trees, IEnumerable(Of SyntaxTree)))
		End Function

		Public Shadows Function AddSyntaxTrees(ByVal trees As IEnumerable(Of SyntaxTree)) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim enumerator As IEnumerator(Of SyntaxTree) = Nothing
			If (trees Is Nothing) Then
				Throw New ArgumentNullException("trees")
			End If
			If (trees.Any()) Then
				Dim instance As ArrayBuilder(Of SyntaxTree) = ArrayBuilder(Of SyntaxTree).GetInstance()
				Try
					instance.AddRange(Me._syntaxTrees)
					Dim flag As Boolean = False
					Dim length As Integer = Me._syntaxTrees.Length
					Dim syntaxTrees As ImmutableDictionary(Of SyntaxTree, Integer) = Me._syntaxTreeOrdinalMap
					Dim syntaxTrees1 As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry) = Me._rootNamespaces
					Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Me._declarationTable
					Using num As Integer = 0
						enumerator = trees.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As SyntaxTree = enumerator.Current
							If (current Is Nothing) Then
								Throw New ArgumentNullException([String].Format(VBResources.Trees0, num))
							End If
							If (Not current.HasCompilationUnitRoot) Then
								Throw New ArgumentException([String].Format(VBResources.TreesMustHaveRootNode, num))
							End If
							If (current.IsEmbeddedOrMyTemplateTree()) Then
								Throw New ArgumentException(VBResources.CannotAddCompilerSpecialTree)
							End If
							If (syntaxTrees1.ContainsKey(current)) Then
								Throw New ArgumentException(VBResources.SyntaxTreeAlreadyPresent, [String].Format(VBResources.Trees0, num))
							End If
							Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.AddSyntaxTreeToDeclarationMapAndTable(current, Me._options, MyBase.IsSubmission, syntaxTrees1, declarationTable, flag)
							instance.Add(current)
							syntaxTrees = syntaxTrees.Add(current, length + num)
							num = num + 1
						End While
					End Using
					If (MyBase.IsSubmission AndAlso syntaxTrees1.Count > 1) Then
						Throw New ArgumentException(VBResources.SubmissionCanHaveAtMostOneSyntaxTree, "trees")
					End If
					visualBasicCompilation = Me.UpdateSyntaxTrees(instance.ToImmutable(), syntaxTrees, syntaxTrees1, declarationTable, flag)
				Finally
					instance.Free()
				End Try
			Else
				visualBasicCompilation = Me
			End If
			Return visualBasicCompilation
		End Function

		Private Shared Sub AddSyntaxTreeToDeclarationMapAndTable(ByVal tree As SyntaxTree, ByVal compilationOptions As VisualBasicCompilationOptions, ByVal isSubmission As Boolean, ByRef declMap As ImmutableDictionary(Of SyntaxTree, Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTableEntry), ByRef declTable As DeclarationTable, ByRef referenceDirectivesChanged As Boolean)
			Dim declarationTableEntry As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTableEntry = New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTableEntry(New Lazy(Of RootSingleNamespaceDeclaration)(Function() VisualBasicCompilation.ForTree(tree, compilationOptions, isSubmission)), False)
			declMap = declMap.Add(tree, declarationTableEntry)
			declTable = declTable.AddRootDeclaration(declarationTableEntry)
			referenceDirectivesChanged = If(referenceDirectivesChanged, True, tree.HasReferenceDirectives())
		End Sub

		Friend Sub AddUsedAssemblies(ByVal assemblies As ICollection(Of AssemblySymbol))
			Dim enumerator As IEnumerator(Of AssemblySymbol) = Nothing
			If (Not assemblies.IsNullOrEmpty()) Then
				Try
					enumerator = assemblies.GetEnumerator()
					While enumerator.MoveNext()
						Me.AddUsedAssembly(enumerator.Current)
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
		End Sub

		Private Sub AddUsedAssembly(ByVal dependency As AssemblySymbol, ByVal stack As ArrayBuilder(Of AssemblySymbol))
			If (Me.AddUsedAssembly(dependency)) Then
				stack.Push(dependency)
			End If
		End Sub

		Friend Function AddUsedAssembly(ByVal assembly As AssemblySymbol) As Boolean
			Dim flag As Boolean
			If (assembly Is Nothing OrElse assembly = Me.SourceAssembly OrElse assembly.IsMissing) Then
				flag = False
			Else
				If (Me._lazyUsedAssemblyReferences Is Nothing) Then
					Interlocked.CompareExchange(Of ConcurrentSet(Of AssemblySymbol))(Me._lazyUsedAssemblyReferences, New ConcurrentSet(Of AssemblySymbol)(), Nothing)
				End If
				flag = Me._lazyUsedAssemblyReferences.Add(assembly)
			End If
			Return flag
		End Function

		Protected Overrides Sub AppendDefaultVersionResource(ByVal resourceStream As Stream)
			Dim fileVersion As String = If(Me.SourceAssembly.FileVersion, Me.SourceAssembly.Identity.Version.ToString())
			Win32ResourceConversions.AppendVersionToResourceStream(resourceStream, Not Me.Options.OutputKind.IsApplication(), fileVersion, Me.SourceModule.Name, Me.SourceModule.Name, If(Me.SourceAssembly.InformationalVersion, fileVersion), Me.SourceAssembly.Identity.Version, If(Me.SourceAssembly.Title, " "), If(Me.SourceAssembly.Copyright, " "), Me.SourceAssembly.Trademark, Me.SourceAssembly.Product, Me.SourceAssembly.Description, Me.SourceAssembly.Company)
		End Sub

		Private Function BindScriptClass() As ImplicitNamedTypeSymbol
			Return DirectCast(MyBase.CommonBindScriptClass(), ImplicitNamedTypeSymbol)
		End Function

		Private Shared Function CheckSumMatches(ByVal bytesText As String, ByVal bytes As ImmutableArray(Of Byte)) As Boolean
			Dim flag As Boolean
			If (bytesText.Length = bytes.Length * 2) Then
				Dim length As Integer = bytesText.Length / 2 - 1
				Dim num As Integer = 0
				While num <= length
					If (' 
					' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation::CheckSumMatches(System.String,System.Collections.Immutable.ImmutableArray`1<System.Byte>)
					' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
					' 
					' Product version: 2019.1.118.0
					' Exception in: System.Boolean CheckSumMatches(System.String,System.Collections.Immutable.ImmutableArray<System.Byte>)
					' 
					' La référence d'objet n'est pas définie à une instance d'un objet.
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3490
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 102
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3529
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1111
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 90
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
					'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
					' 
					' mailto: JustDecompilePublicFeedback@telerik.com


		Public Overrides Function ClassifyCommonConversion(ByVal source As ITypeSymbol, ByVal destination As ITypeSymbol) As CommonConversion
			Return Me.ClassifyConversion(source, destination).ToCommonConversion()
		End Function

		Public Function ClassifyConversion(ByVal source As ITypeSymbol, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			If (source Is Nothing) Then
				Throw New ArgumentNullException("source")
			End If
			If (destination Is Nothing) Then
				Throw New ArgumentNullException("destination")
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = source.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("source")
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("destination")
			If (typeSymbol.IsErrorType() OrElse typeSymbol1.IsErrorType()) Then
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(New KeyValuePair(Of ConversionKind, MethodSymbol)())
			Else
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(typeSymbol, typeSymbol1, discarded))
			End If
			Return conversion
		End Function

		Friend Overrides Function ClassifyConvertibleConversion(ByVal source As IOperation, ByVal destination As ITypeSymbol, ByRef constantValue As Microsoft.CodeAnalysis.ConstantValue) As IConvertibleConversion
			Dim conversion As IConvertibleConversion
			Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol)
			constantValue = Nothing
			If (destination IsNot Nothing) Then
				Dim type As ITypeSymbol = source.Type
				Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = source.GetConstantValue()
				If (type IsNot Nothing) Then
					Dim conversion1 As Microsoft.CodeAnalysis.VisualBasic.Conversion = Me.ClassifyConversion(type, destination)
					If (conversion1.IsReference AndAlso constantValue1 IsNot Nothing AndAlso constantValue1.IsNothing) Then
						constantValue = constantValue1
					End If
					conversion = DirectCast(conversion1, IConvertibleConversion)
				ElseIf (constantValue1 Is Nothing OrElse Not constantValue1.IsNothing OrElse Not destination.IsReferenceType) Then
					keyValuePair = New KeyValuePair(Of ConversionKind, MethodSymbol)()
					conversion = DirectCast(New Microsoft.CodeAnalysis.VisualBasic.Conversion(keyValuePair), IConvertibleConversion)
				Else
					constantValue = constantValue1
					conversion = DirectCast(New Microsoft.CodeAnalysis.VisualBasic.Conversion(New KeyValuePair(Of ConversionKind, MethodSymbol)(ConversionKind.WideningNothingLiteral, Nothing)), IConvertibleConversion)
				End If
			Else
				keyValuePair = New KeyValuePair(Of ConversionKind, MethodSymbol)()
				conversion = DirectCast(New Microsoft.CodeAnalysis.VisualBasic.Conversion(keyValuePair), IConvertibleConversion)
			End If
			Return conversion
		End Function

		Public Shadows Function Clone() As VisualBasicCompilation
			Return New VisualBasicCompilation(MyBase.AssemblyName, Me._options, MyBase.ExternalReferences, Me._syntaxTrees, Me._syntaxTreeOrdinalMap, Me._rootNamespaces, Me._embeddedTrees, Me._declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, True, MyBase.SemanticModelProvider, Nothing)
		End Function

		Protected Overrides Function CommonAddSyntaxTrees(ByVal trees As IEnumerable(Of SyntaxTree)) As Microsoft.CodeAnalysis.Compilation
			Dim compilation As Microsoft.CodeAnalysis.Compilation
			Dim syntaxTreeArray As SyntaxTree() = TryCast(trees, SyntaxTree())
			If (syntaxTreeArray Is Nothing) Then
				If (trees Is Nothing) Then
					Throw New ArgumentNullException("trees")
				End If
				compilation = Me.AddSyntaxTrees(trees.Cast(Of SyntaxTree)())
			Else
				compilation = Me.AddSyntaxTrees(syntaxTreeArray)
			End If
			Return compilation
		End Function

		Protected Overrides Function CommonClone() As Compilation
			Return Me.Clone()
		End Function

		Protected Overrides Function CommonContainsSyntaxTree(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			Return Me.ContainsSyntaxTree(syntaxTree)
		End Function

		Protected Overrides Function CommonCreateAnonymousTypeSymbol(ByVal memberTypes As ImmutableArray(Of ITypeSymbol), ByVal memberNames As ImmutableArray(Of String), ByVal memberLocations As ImmutableArray(Of Microsoft.CodeAnalysis.Location), ByVal memberIsReadOnly As ImmutableArray(Of Boolean), ByVal memberNullableAnnotations As ImmutableArray(Of NullableAnnotation)) As INamedTypeSymbol
			Dim flag As Boolean
			Dim num As Integer = 0
			Dim enumerator As ImmutableArray(Of ITypeSymbol).Enumerator = memberTypes.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.EnsureVbSymbolOrNothing(Of TypeSymbol)([String].Format("{0}({1})", CObj("memberTypes"), num))
				num = num + 1
			End While
			Dim instance As ArrayBuilder(Of AnonymousTypeField) = ArrayBuilder(Of AnonymousTypeField).GetInstance()
			Dim length As Integer = memberTypes.Length - 1
			num = 0
			Do
				Dim item As ITypeSymbol = memberTypes(num)
				Dim str As String = memberNames(num)
				Dim location As Microsoft.CodeAnalysis.Location = If(memberLocations.IsDefault, Microsoft.CodeAnalysis.Location.None, memberLocations(num))
				flag = If(memberIsReadOnly.IsDefault, True, memberIsReadOnly(num))
				instance.Add(New AnonymousTypeField(str, DirectCast(item, TypeSymbol), location, flag))
				num = num + 1
			Loop While num <= length
			Dim anonymousTypeDescriptor As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor = New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor(instance.ToImmutableAndFree(), Microsoft.CodeAnalysis.Location.None, False)
			Return Me.AnonymousTypeManager.ConstructAnonymousTypeSymbol(anonymousTypeDescriptor)
		End Function

		Protected Overrides Function CommonCreateArrayTypeSymbol(ByVal elementType As ITypeSymbol, ByVal rank As Integer, ByVal elementNullableAnnotation As NullableAnnotation) As IArrayTypeSymbol
			Return Me.CreateArrayTypeSymbol(elementType.EnsureVbSymbolOrNothing(Of TypeSymbol)("elementType"), rank)
		End Function

		Protected Overrides Function CommonCreateErrorNamespaceSymbol(ByVal container As INamespaceSymbol, ByVal name As String) As INamespaceSymbol
			Return New MissingNamespaceSymbol(container.EnsureVbSymbolOrNothing(Of NamespaceSymbol)("container"), name)
		End Function

		Protected Overrides Function CommonCreateErrorTypeSymbol(ByVal container As INamespaceOrTypeSymbol, ByVal name As String, ByVal arity As Integer) As INamedTypeSymbol
			Return New ExtendedErrorTypeSymbol(container.EnsureVbSymbolOrNothing(Of NamespaceOrTypeSymbol)("container"), name, arity)
		End Function

		Protected Overrides Function CommonCreateFunctionPointerTypeSymbol(ByVal returnType As ITypeSymbol, ByVal refKind As Microsoft.CodeAnalysis.RefKind, ByVal parameterTypes As ImmutableArray(Of ITypeSymbol), ByVal parameterRefKinds As ImmutableArray(Of Microsoft.CodeAnalysis.RefKind), ByVal callingConvention As SignatureCallingConvention, ByVal callingConventionTypes As ImmutableArray(Of INamedTypeSymbol)) As IFunctionPointerTypeSymbol
			Throw New NotSupportedException(VBResources.ThereAreNoFunctionPointerTypesInVB)
		End Function

		Protected Overrides Function CommonCreateNativeIntegerTypeSymbol(ByVal signed As Boolean) As INamedTypeSymbol
			Throw New NotSupportedException(VBResources.ThereAreNoNativeIntegerTypesInVB)
		End Function

		Protected Overrides Function CommonCreatePointerTypeSymbol(ByVal elementType As ITypeSymbol) As IPointerTypeSymbol
			Throw New NotSupportedException(VBResources.ThereAreNoPointerTypesInVB)
		End Function

		Protected Overrides Function CommonCreateTupleTypeSymbol(ByVal elementTypes As ImmutableArray(Of ITypeSymbol), ByVal elementNames As ImmutableArray(Of String), ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNullableAnnotations As ImmutableArray(Of NullableAnnotation)) As INamedTypeSymbol
			Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(elementTypes.Length)
			Dim length As Integer = elementTypes.Length - 1
			Dim num As Integer = 0
			Do
				instance.Add(elementTypes(num).EnsureVbSymbolOrNothing(Of TypeSymbol)([String].Format("{0}[{1}]", CObj("elementTypes"), num)))
				num = num + 1
			Loop While num <= length
			Dim immutableAndFree As ImmutableArray(Of TypeSymbol) = instance.ToImmutableAndFree()
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Return TupleTypeSymbol.Create(Nothing, immutableAndFree, elementLocations, elementNames, Me, False, flags, Nothing, Nothing)
		End Function

		Protected Overrides Function CommonCreateTupleTypeSymbol(ByVal underlyingType As INamedTypeSymbol, ByVal elementNames As ImmutableArray(Of String), ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNullableAnnotations As ImmutableArray(Of NullableAnnotation)) As INamedTypeSymbol
			Dim num As Integer
			If (Not underlyingType.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)("underlyingType").IsTupleCompatible(num)) Then
				Throw New ArgumentException(CodeAnalysisResources.TupleUnderlyingTypeMustBeTupleCompatible, "underlyingType")
			End If
			elementNames = Compilation.CheckTupleElementNames(num, elementNames)
			Compilation.CheckTupleElementLocations(num, elementLocations)
			Compilation.CheckTupleElementNullableAnnotations(num, elementNullableAnnotations)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = underlyingType.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)("underlyingType")
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Return TupleTypeSymbol.Create(Nothing, namedTypeSymbol, elementLocations, elementNames, flags)
		End Function

		Protected Overrides Function CommonGetAssemblyOrModuleSymbol(ByVal reference As MetadataReference) As ISymbol
			Return Me.GetAssemblyOrModuleSymbol(reference)
		End Function

		Friend Overrides Function CommonGetBoundReferenceManager() As CommonReferenceManager
			Return Me.GetBoundReferenceManager()
		End Function

		Protected Overrides Function CommonGetCompilationNamespace(ByVal namespaceSymbol As INamespaceSymbol) As INamespaceSymbol
			Return Me.GetCompilationNamespace(namespaceSymbol)
		End Function

		Protected Overrides Function CommonGetEntryPoint(ByVal cancellationToken As System.Threading.CancellationToken) As IMethodSymbol
			Return Me.GetEntryPoint(cancellationToken)
		End Function

		Protected Friend Overrides Function CommonGetMetadataReference(ByVal assemblySymbol As IAssemblySymbol) As Microsoft.CodeAnalysis.MetadataReference
			Dim metadataReference As Microsoft.CodeAnalysis.MetadataReference
			Dim assemblySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = TryCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			If (assemblySymbol1 Is Nothing) Then
				metadataReference = Nothing
			Else
				metadataReference = Me.GetMetadataReference(assemblySymbol1)
			End If
			Return metadataReference
		End Function

		Protected Overrides Function CommonGetSemanticModel(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal ignoreAccessibility As Boolean) As SemanticModel
			Return Me.GetSemanticModel(syntaxTree, ignoreAccessibility)
		End Function

		Protected Friend Overrides Function CommonGetSpecialType(ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As INamedTypeSymbolInternal
			Return Me.GetSpecialType(specialType)
		End Function

		Friend Overrides Function CommonGetSpecialTypeMember(ByVal specialMember As Microsoft.CodeAnalysis.SpecialMember) As ISymbolInternal
			Return Me.GetSpecialTypeMember(specialMember)
		End Function

		Protected Overrides Function CommonGetTypeByMetadataName(ByVal metadataName As String) As INamedTypeSymbol
			Return Me.GetTypeByMetadataName(metadataName)
		End Function

		Friend Overrides Function CommonGetWellKnownType(ByVal wellknownType As Microsoft.CodeAnalysis.WellKnownType) As ITypeSymbolInternal
			Return Me.GetWellKnownType(wellknownType)
		End Function

		Friend Overrides Function CommonGetWellKnownTypeMember(ByVal member As WellKnownMember) As ISymbolInternal
			Return Me.GetWellKnownTypeMember(member)
		End Function

		Private Function CommonLanguageVersion(ByVal syntaxTrees As ImmutableArray(Of SyntaxTree)) As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Dim nullable As Nullable(Of Boolean)
			Dim nullable1 As Nullable(Of Integer)
			Dim nullable2 As Nullable(Of Boolean)
			Dim nullable3 As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) = Nothing
			Dim enumerator As ImmutableArray(Of SyntaxTree).Enumerator = syntaxTrees.GetEnumerator()
			While enumerator.MoveNext()
				Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = DirectCast(enumerator.Current.Options, VisualBasicParseOptions).LanguageVersion
				If (nullable3.HasValue) Then
					If (nullable3.HasValue) Then
						nullable1 = New Nullable(Of Integer)(nullable3.GetValueOrDefault())
					Else
						nullable1 = Nothing
					End If
					Dim nullable4 As Nullable(Of Integer) = nullable1
					Dim num As Integer = CInt(languageVersion)
					If (nullable4.HasValue) Then
						nullable2 = New Nullable(Of Boolean)(nullable4.GetValueOrDefault() <> num)
					Else
						nullable = Nothing
						nullable2 = nullable
					End If
					nullable = nullable2
					If (Not nullable.GetValueOrDefault()) Then
						Continue While
					End If
					Throw New ArgumentException(CodeAnalysisResources.InconsistentLanguageVersions, "syntaxTrees")
				Else
					nullable3 = New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.LanguageVersion)(languageVersion)
				End If
			End While
			If (Not nullable3.HasValue) Then
				Return Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default].MapSpecifiedToEffectiveVersion()
			End If
			Return nullable3.GetValueOrDefault()
		End Function

		Protected Overrides Function CommonRemoveAllSyntaxTrees() As Compilation
			Return Me.RemoveAllSyntaxTrees()
		End Function

		Protected Overrides Function CommonRemoveSyntaxTrees(ByVal trees As IEnumerable(Of SyntaxTree)) As Microsoft.CodeAnalysis.Compilation
			Dim compilation As Microsoft.CodeAnalysis.Compilation
			Dim syntaxTreeArray As SyntaxTree() = TryCast(trees, SyntaxTree())
			If (syntaxTreeArray Is Nothing) Then
				If (trees Is Nothing) Then
					Throw New ArgumentNullException("trees")
				End If
				compilation = Me.RemoveSyntaxTrees(trees.Cast(Of SyntaxTree)())
			Else
				compilation = Me.RemoveSyntaxTrees(syntaxTreeArray)
			End If
			Return compilation
		End Function

		Protected Overrides Function CommonReplaceSyntaxTree(ByVal oldTree As SyntaxTree, ByVal newTree As SyntaxTree) As Compilation
			Return Me.ReplaceSyntaxTree(oldTree, newTree)
		End Function

		Protected Overrides Function CommonWithAssemblyName(ByVal assemblyName As String) As Compilation
			Return Me.WithAssemblyName(assemblyName)
		End Function

		Protected Overrides Function CommonWithOptions(ByVal options As CompilationOptions) As Compilation
			Return Me.WithOptions(DirectCast(options, VisualBasicCompilationOptions))
		End Function

		Protected Overrides Function CommonWithReferences(ByVal newReferences As IEnumerable(Of MetadataReference)) As Compilation
			Return Me.WithReferences(newReferences)
		End Function

		Protected Overrides Function CommonWithScriptCompilationInfo(ByVal info As Microsoft.CodeAnalysis.ScriptCompilationInfo) As Compilation
			Return Me.WithScriptCompilationInfo(DirectCast(info, VisualBasicScriptCompilationInfo))
		End Function

		Friend Overrides Function CompareSourceLocations(ByVal first As Location, ByVal second As Location) As Integer
			Return LexicalSortKey.Compare(first, second, Me)
		End Function

		Friend Overrides Function CompareSourceLocations(ByVal first As SyntaxReference, ByVal second As SyntaxReference) As Integer
			Return LexicalSortKey.Compare(first, second, Me)
		End Function

		Friend Overrides Function CompileMethods(ByVal moduleBuilder As CommonPEModuleBuilder, ByVal emittingPdb As Boolean, ByVal emitMetadataOnly As Boolean, ByVal emitTestCoverageData As Boolean, ByVal diagnostics As DiagnosticBag, ByVal filterOpt As Predicate(Of ISymbolInternal), ByVal cancellationToken As System.Threading.CancellationToken) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean = Not MyBase.FilterAndAppendDiagnostics(diagnostics, DirectCast(Me.GetDiagnostics(CompilationStage.[Declare], True, cancellationToken), IEnumerable(Of Diagnostic)), Nothing, cancellationToken)
			Dim pEModuleBuilder As Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder = DirectCast(moduleBuilder, Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder)
			Me.EmbeddedSymbolManager.MarkAllDeferredSymbolsAsReferenced(Me)
			If (Not flag1) Then
				pEModuleBuilder.TranslateImports(diagnostics)
			End If
			If (emitMetadataOnly) Then
				If (Not flag1) Then
					If (pEModuleBuilder.SourceModule.HasBadAttributes) Then
						diagnostics.Add(ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, New [Object]() { pEModuleBuilder.SourceModule.Name, New LocalizableResourceString("ModuleHasInvalidAttributes", CodeAnalysisResources.ResourceManager, GetType(CodeAnalysisResources)) })
						flag = False
						Return flag
					End If
					SynthesizedMetadataCompiler.ProcessSynthesizedMembers(Me, pEModuleBuilder, cancellationToken)
					cancellationToken.ThrowIfCancellationRequested()
					flag = True
					Return flag
				Else
					flag = False
				End If
			ElseIf ((emittingPdb OrElse emitTestCoverageData) AndAlso Not MyBase.CreateDebugDocuments(pEModuleBuilder.DebugDocumentsBuilder, pEModuleBuilder.EmbeddedTexts, diagnostics)) Then
				flag = False
			Else
				Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
				MethodCompiler.CompileMethodBodies(Me, pEModuleBuilder, emittingPdb, emitTestCoverageData, flag1, filterOpt, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(instance), cancellationToken)
				Dim flag2 As Boolean = Not MyBase.FilterAndAppendAndFreeDiagnostics(diagnostics, instance, cancellationToken)
				If (Not flag1 AndAlso Not flag2) Then
					cancellationToken.ThrowIfCancellationRequested()
					flag = True
					Return flag
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Sub CompleteTheSetOfUsedAssemblies(ByVal seenErrors As Boolean, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim referencedAssemblySymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			Dim completeSetOfUsedAssemblies As ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			If (Not Me._usedAssemblyReferencesFrozen AndAlso Not Volatile.Read(Me._usedAssemblyReferencesFrozen)) Then
				If (Not seenErrors) Then
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me.SourceAssembly.Modules
					Dim length As Integer = modules.Length - 1
					Dim num As Integer = 1
					Do
						modules = Me.SourceAssembly.Modules
						referencedAssemblySymbols = modules(num).ReferencedAssemblySymbols
						Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Enumerator = referencedAssemblySymbols.GetEnumerator()
						While enumerator.MoveNext()
							Me.AddUsedAssembly(enumerator.Current)
						End While
						num = num + 1
					Loop While num <= length
					If (Not Me._usedAssemblyReferencesFrozen AndAlso Not Volatile.Read(Me._usedAssemblyReferencesFrozen)) Then
						If (Me._lazyUsedAssemblyReferences IsNot Nothing) Then
							SyncLock Me._lazyUsedAssemblyReferences
								If (Me._usedAssemblyReferencesFrozen OrElse Volatile.Read(Me._usedAssemblyReferencesFrozen)) Then
									Return
								Else
									Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).GetInstance(Me._lazyUsedAssemblyReferences.Count)
									instance.AddRange(Me._lazyUsedAssemblyReferences)
									While instance.Count <> 0
										Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = instance.Pop()
										Dim sourceAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol = TryCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol)
										If (sourceAssemblySymbol Is Nothing) Then
											Dim retargetingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol = TryCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol)
											If (retargetingAssemblySymbol Is Nothing) Then
												Me.AddReferencedAssemblies(assemblySymbol, True, instance)
											Else
												completeSetOfUsedAssemblies = retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken)
												If (completeSetOfUsedAssemblies IsNot Nothing) Then
													referencedAssemblySymbols = retargetingAssemblySymbol.UnderlyingAssembly.SourceModule.ReferencedAssemblySymbols
													Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Enumerator = referencedAssemblySymbols.GetEnumerator()
													While enumerator1.MoveNext()
														Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = enumerator1.Current
														If (current.IsLinked OrElse Not completeSetOfUsedAssemblies.Contains(current)) Then
															Continue While
														End If
														Dim assemblySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = Nothing
														If (Not DirectCast(retargetingAssemblySymbol.Modules(0), RetargetingModuleSymbol).RetargetingDefinitions(current, assemblySymbol1)) Then
															assemblySymbol1 = current
														End If
														Me.AddUsedAssembly(assemblySymbol1, instance)
													End While
												End If
												Me.AddReferencedAssemblies(retargetingAssemblySymbol, False, instance)
											End If
										Else
											completeSetOfUsedAssemblies = sourceAssemblySymbol.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken)
											If (completeSetOfUsedAssemblies Is Nothing) Then
												Continue While
											End If
											Dim keyEnumerator As ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).KeyEnumerator = completeSetOfUsedAssemblies.GetEnumerator()
											While keyEnumerator.MoveNext()
												Me.AddUsedAssembly(keyEnumerator.Current, instance)
											End While
										End If
									End While
									instance.Free()
								End If
							End SyncLock
						End If
						If (Me.SourceAssembly.CorLibrary IsNot Nothing) Then
							Me.AddUsedAssembly(Me.SourceAssembly.CorLibrary)
						End If
					Else
						Return
					End If
				Else
					referencedAssemblySymbols = Me.SourceModule.ReferencedAssemblySymbols
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Enumerator = referencedAssemblySymbols.GetEnumerator()
					While enumerator2.MoveNext()
						Me.AddUsedAssembly(enumerator2.Current)
					End While
				End If
				Me._usedAssemblyReferencesFrozen = True
			End If
		End Sub

		Private Sub CompleteTree(ByVal tree As SyntaxTree)
			If (Not tree.IsEmbeddedOrMyTemplateTree()) Then
				If (Me._lazyCompilationUnitCompletedTrees Is Nothing) Then
					Interlocked.CompareExchange(Of HashSet(Of SyntaxTree))(Me._lazyCompilationUnitCompletedTrees, New HashSet(Of SyntaxTree)(), Nothing)
				End If
				SyncLock Me._lazyCompilationUnitCompletedTrees
					If (Me._lazyCompilationUnitCompletedTrees.Add(tree)) Then
						MyBase.EventQueue.TryEnqueue(New CompilationUnitCompletedEvent(Me, tree))
						If (Me._lazyCompilationUnitCompletedTrees.Count = Me.SyntaxTrees.Length) Then
							MyBase.CompleteCompilationEventQueue_NoLock()
						End If
					End If
				End SyncLock
			End If
		End Sub

		Friend Overrides Sub CompleteTrees(ByVal filterTree As SyntaxTree)
			If (MyBase.EventQueue IsNot Nothing) Then
				If (filterTree IsNot Nothing) Then
					Me.CompleteTree(filterTree)
					Return
				End If
				Dim enumerator As ImmutableArray(Of SyntaxTree).Enumerator = Me.SyntaxTrees.GetEnumerator()
				While enumerator.MoveNext()
					Me.CompleteTree(enumerator.Current)
				End While
			End If
		End Sub

		Public Overrides Function ContainsSymbolsWithName(ByVal predicate As Func(Of String, Boolean), Optional ByVal filter As SymbolFilter = 6, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			If (predicate Is Nothing) Then
				Throw New ArgumentNullException("predicate")
			End If
			If (filter = SymbolFilter.None) Then
				Throw New ArgumentException(VBResources.NoNoneSearchCriteria, "filter")
			End If
			Return DeclarationTable.ContainsName(Me.MergedRootDeclaration, predicate, filter, cancellationToken)
		End Function

		Public Overrides Function ContainsSymbolsWithName(ByVal name As String, Optional ByVal filter As SymbolFilter = 6, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			If (name Is Nothing) Then
				Throw New ArgumentNullException("name")
			End If
			If (filter = SymbolFilter.None) Then
				Throw New ArgumentException(VBResources.NoNoneSearchCriteria, "filter")
			End If
			Return DeclarationTable.ContainsName(Me.MergedRootDeclaration, name, filter, cancellationToken)
		End Function

		Public Shadows Function ContainsSyntaxTree(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			If (syntaxTree Is Nothing) Then
				Return False
			End If
			Return Me._rootNamespaces.ContainsKey(syntaxTree)
		End Function

		Public Shared Function Create(ByVal assemblyName As String, Optional ByVal syntaxTrees As IEnumerable(Of SyntaxTree) = Nothing, Optional ByVal references As IEnumerable(Of MetadataReference) = Nothing, Optional ByVal options As VisualBasicCompilationOptions = Nothing) As VisualBasicCompilation
			Dim syntaxTrees1 As IEnumerable(Of SyntaxTree)
			Dim str As String = assemblyName
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions = options
			If (syntaxTrees IsNot Nothing) Then
				syntaxTrees1 = syntaxTrees.Cast(Of SyntaxTree)()
			Else
				syntaxTrees1 = Nothing
			End If
			Return VisualBasicCompilation.Create(str, visualBasicCompilationOption, syntaxTrees1, references, Nothing, Nothing, Nothing, False)
		End Function

		Private Shared Function Create(ByVal assemblyName As String, ByVal options As VisualBasicCompilationOptions, ByVal syntaxTrees As IEnumerable(Of SyntaxTree), ByVal references As IEnumerable(Of MetadataReference), ByVal previousSubmission As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation, ByVal returnType As Type, ByVal hostObjectType As Type, ByVal isSubmission As Boolean) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			If (options Is Nothing) Then
				Dim nums As ImmutableArray(Of Byte) = New ImmutableArray(Of Byte)()
				Dim nullable As Nullable(Of Boolean) = Nothing
				options = New VisualBasicCompilationOptions(OutputKind.ConsoleApplication, Nothing, Nothing, "Script", Nothing, Nothing, OptionStrict.Off, True, True, False, Nothing, False, OptimizationLevel.Debug, True, Nothing, Nothing, nums, nullable, Platform.AnyCpu, ReportDiagnostic.[Default], Nothing, True, False, Nothing, Nothing, Nothing, Nothing, Nothing, False, False, MetadataImportOptions.[Public])
			End If
			Dim metadataReferences As ImmutableArray(Of MetadataReference) = Compilation.ValidateReferences(Of VisualBasicCompilationReference)(references)
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Nothing
			Dim embeddedTreeAndDeclarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration) = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.CreateEmbeddedTrees(New Lazy(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)(Function() visualBasicCompilation))
			Dim syntaxTrees1 As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry) = ImmutableDictionary.Create(Of SyntaxTree, DeclarationTableEntry)()
			Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.AddEmbeddedTrees(Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable.Empty, embeddedTreeAndDeclarations)
			visualBasicCompilation = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation(assemblyName, options, metadataReferences, ImmutableArray(Of SyntaxTree).Empty, ImmutableDictionary.Create(Of SyntaxTree, Integer)(), syntaxTrees1, embeddedTreeAndDeclarations, declarationTable, previousSubmission, returnType, hostObjectType, isSubmission, Nothing, False, Nothing, Nothing)
			If (syntaxTrees IsNot Nothing) Then
				visualBasicCompilation = visualBasicCompilation.AddSyntaxTrees(syntaxTrees)
			End If
			Return visualBasicCompilation
		End Function

		Friend Overrides Function CreateAnalyzerDriver(ByVal analyzers As ImmutableArray(Of DiagnosticAnalyzer), ByVal analyzerManager As Microsoft.CodeAnalysis.Diagnostics.AnalyzerManager, ByVal severityFilter As Microsoft.CodeAnalysis.Diagnostics.SeverityFilter) As AnalyzerDriver
			Dim func As Func(Of SyntaxNode, SyntaxKind)
			Dim func1 As Func(Of SyntaxTrivia, Boolean)
			If (VisualBasicCompilation._Closure$__.$I189-0 Is Nothing) Then
				func = Function(node As SyntaxNode) node.Kind()
				VisualBasicCompilation._Closure$__.$I189-0 = func
			Else
				func = VisualBasicCompilation._Closure$__.$I189-0
			End If
			Dim func2 As Func(Of SyntaxNode, SyntaxKind) = func
			If (VisualBasicCompilation._Closure$__.$I189-1 Is Nothing) Then
				func1 = Function(trivia As SyntaxTrivia) trivia.Kind() = SyntaxKind.CommentTrivia
				VisualBasicCompilation._Closure$__.$I189-1 = func1
			Else
				func1 = VisualBasicCompilation._Closure$__.$I189-1
			End If
			Return New AnalyzerDriver(Of SyntaxKind)(analyzers, func2, analyzerManager, severityFilter, func1)
		End Function

		Friend Function CreateArrayTypeSymbol(ByVal elementType As TypeSymbol, Optional ByVal rank As Integer = 1) As ArrayTypeSymbol
			If (elementType Is Nothing) Then
				Throw New ArgumentNullException("elementType")
			End If
			If (rank < 1) Then
				Throw New ArgumentException("rank")
			End If
			Return ArrayTypeSymbol.CreateVBArray(elementType, New ImmutableArray(Of CustomModifier)(), rank, Me)
		End Function

		Private Shared Function CreateEmbeddedTrees(ByVal compReference As Lazy(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration)
			Return ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration)(New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration(Function()
				Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compReference.Value
				If (Not visualBasicCompilation.Options.EmbedVbCoreRuntime And Not visualBasicCompilation.IncludeInternalXmlHelper()) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.EmbeddedSyntax
			End Function, Function()
				Dim visualBasicCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compReference.Value
				If (Not visualBasicCompilation1.Options.EmbedVbCoreRuntime And Not visualBasicCompilation1.IncludeInternalXmlHelper()) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.ForTree(Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.EmbeddedSyntax, visualBasicCompilation1.Options, False)
			End Function), New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration(Function()
				If (Not compReference.Value.Options.EmbedVbCoreRuntime) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.VbCoreSyntaxTree
			End Function, Function()
				Dim visualBasicCompilation2 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compReference.Value
				If (Not visualBasicCompilation2.Options.EmbedVbCoreRuntime) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.ForTree(Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.VbCoreSyntaxTree, visualBasicCompilation2.Options, False)
			End Function), New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration(Function()
				If (Not compReference.Value.IncludeInternalXmlHelper()) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.InternalXmlHelperSyntax
			End Function, Function()
				Dim visualBasicCompilation3 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compReference.Value
				If (Not visualBasicCompilation3.IncludeInternalXmlHelper()) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.ForTree(Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager.InternalXmlHelperSyntax, visualBasicCompilation3.Options, False)
			End Function), New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration(Function() compReference.Value.MyTemplate, Function()
				Dim value As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compReference.Value
				If (value.MyTemplate Is Nothing) Then
					Return Nothing
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.ForTree(value.MyTemplate, value.Options, False)
			End Function))
		End Function

		Friend Overrides Function CreateModuleBuilder(ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal debugEntryPoint As IMethodSymbol, ByVal sourceLinkStream As Stream, ByVal embeddedTexts As IEnumerable(Of EmbeddedText), ByVal manifestResources As IEnumerable(Of ResourceDescription), ByVal testData As CompilationTestData, ByVal diagnostics As DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken) As CommonPEModuleBuilder
			Return Me.CreateModuleBuilder(emitOptions, debugEntryPoint, sourceLinkStream, embeddedTexts, manifestResources, testData, diagnostics, ImmutableArray(Of NamedTypeSymbol).Empty, cancellationToken)
		End Function

		Friend Function CreateModuleBuilder(ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal debugEntryPoint As IMethodSymbol, ByVal sourceLinkStream As Stream, ByVal embeddedTexts As IEnumerable(Of EmbeddedText), ByVal manifestResources As IEnumerable(Of ResourceDescription), ByVal testData As CompilationTestData, ByVal diagnostics As DiagnosticBag, ByVal additionalTypes As ImmutableArray(Of NamedTypeSymbol), ByVal cancellationToken As System.Threading.CancellationToken) As CommonPEModuleBuilder
			Dim pEAssemblyBuilder As PEModuleBuilder
			Dim runtimeMetadataVersion As String = Me.GetRuntimeMetadataVersion()
			Dim modulePropertiesForSerialization As Microsoft.Cci.ModulePropertiesForSerialization = MyBase.ConstructModuleSerializationProperties(emitOptions, runtimeMetadataVersion, New Guid())
			If (manifestResources Is Nothing) Then
				manifestResources = SpecializedCollections.EmptyEnumerable(Of ResourceDescription)()
			End If
			If (Not Me.Options.OutputKind.IsNetModule()) Then
				Dim outputKind As Microsoft.CodeAnalysis.OutputKind = If(Me.Options.OutputKind.IsValid(), Me.Options.OutputKind, Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary)
				pEAssemblyBuilder = New Microsoft.CodeAnalysis.VisualBasic.Emit.PEAssemblyBuilder(Me.SourceAssembly, emitOptions, outputKind, modulePropertiesForSerialization, manifestResources, additionalTypes)
			Else
				pEAssemblyBuilder = New PENetModuleBuilder(DirectCast(Me.SourceModule, SourceModuleSymbol), emitOptions, modulePropertiesForSerialization, manifestResources)
			End If
			If (debugEntryPoint IsNot Nothing) Then
				pEAssemblyBuilder.SetDebugEntryPoint(DirectCast(debugEntryPoint, MethodSymbol), diagnostics)
			End If
			pEAssemblyBuilder.SourceLinkStreamOpt = sourceLinkStream
			If (embeddedTexts IsNot Nothing) Then
				pEAssemblyBuilder.EmbeddedTexts = embeddedTexts
			End If
			If (testData IsNot Nothing) Then
				pEAssemblyBuilder.SetMethodTestData(testData.Methods)
				testData.[Module] = pEAssemblyBuilder
			End If
			Return pEAssemblyBuilder
		End Function

		Friend Shared Function CreateScriptCompilation(ByVal assemblyName As String, Optional ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Nothing, Optional ByVal references As IEnumerable(Of MetadataReference) = Nothing, Optional ByVal options As VisualBasicCompilationOptions = Nothing, Optional ByVal previousScriptCompilation As VisualBasicCompilation = Nothing, Optional ByVal returnType As Type = Nothing, Optional ByVal globalsType As Type = Nothing) As VisualBasicCompilation
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation::CreateScriptCompilation(System.String,Microsoft.CodeAnalysis.SyntaxTree,System.Collections.Generic.IEnumerable`1<Microsoft.CodeAnalysis.MetadataReference>,Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationOptions,Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation,System.Type,System.Type)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation CreateScriptCompilation(System.String,Microsoft.CodeAnalysis.SyntaxTree,System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.MetadataReference>,Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationOptions,Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation,System.Type,System.Type)
			' 
			' L'argument spécifié n'était pas dans les limites de la plage des valeurs valides.
			' Nom du paramètre : Target of array indexer expression is not an array.
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:ligne 129
			'    à Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:ligne 214
			'    à Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.set_Left(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:ligne 241
			'    à ..(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 74
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 97
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 44
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Overrides Function CreateSemanticModel(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal ignoreAccessibility As Boolean) As SemanticModel
			Return New SyntaxTreeSemanticModel(Me, DirectCast(Me.SourceModule, SourceModuleSymbol), syntaxTree, ignoreAccessibility)
		End Function

		Friend Overrides Function EmitDifference(ByVal baseline As EmitBaseline, ByVal edits As IEnumerable(Of SemanticEdit), ByVal isAddedSymbol As Func(Of ISymbol, Boolean), ByVal metadataStream As Stream, ByVal ilStream As Stream, ByVal pdbStream As Stream, ByVal updatedMethods As ICollection(Of MethodDefinitionHandle), ByVal testData As CompilationTestData, ByVal cancellationToken As System.Threading.CancellationToken) As EmitDifferenceResult
			Return EmitHelpers.EmitDifference(Me, baseline, edits, isAddedSymbol, metadataStream, ilStream, pdbStream, updatedMethods, testData, cancellationToken)
		End Function

		Private Shared Function FilterDiagnosticsByLocation(ByVal diagnostics As IEnumerable(Of Diagnostic), ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan)) As IEnumerable(Of Diagnostic)
			Return New VisualBasicCompilation.VB$StateMachine_185_FilterDiagnosticsByLocation(-2) With
			{
				.$P_diagnostics = diagnostics,
				.$P_tree = tree,
				.$P_filterSpanWithinTree = filterSpanWithinTree
			}
		End Function

		Private Function FindEntryPoint(ByVal cancellationToken As System.Threading.CancellationToken, ByRef sealedDiagnostics As ImmutableArray(Of Diagnostic)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim scriptEntryPoint As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim sourceMemberContainerTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol
			Dim enumerator As IEnumerator(Of ISymbol) = Nothing
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Dim methodSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
			Try
				Dim mainTypeName As String = Me.Options.MainTypeName
				Dim globalNamespace As NamespaceSymbol = Me.SourceModule.GlobalNamespace
				If (mainTypeName Is Nothing) Then
					sourceMemberContainerTypeSymbol = Nothing
					Using MyBase.AssemblyName
						enumerator = Me.GetSymbolsWithName("Main", SymbolFilter.Member, cancellationToken).GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (If(current IsNot Nothing, Not current.IsEntryPointCandidate, True)) Then
								Continue While
							End If
							methodSymbols.Add(current)
						End While
					End Using
					If (Me.ScriptClass IsNot Nothing) Then
						Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = methodSymbols.GetEnumerator()
						While enumerator1.MoveNext()
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator1.Current
							instance.Add(ERRID.WRN_MainIgnored, methodSymbol.Locations.First(), New [Object]() { methodSymbol })
						End While
						scriptEntryPoint = Me.ScriptClass.GetScriptEntryPoint()
						Return scriptEntryPoint
					End If
				ElseIf (Me.ScriptClass Is Nothing) Then
					Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = globalNamespace.GetNamespaceOrTypeByQualifiedName(mainTypeName.Split(New [Char]() { "."C })).OfType(Of NamedTypeSymbol)().OfMinimalArity()
					If (namespaceOrTypeSymbol IsNot Nothing) Then
						sourceMemberContainerTypeSymbol = TryCast(namespaceOrTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol)
						If (sourceMemberContainerTypeSymbol Is Nothing OrElse sourceMemberContainerTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso sourceMemberContainerTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Struct AndAlso sourceMemberContainerTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Module]) Then
							instance.Add(ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, New [Object]() { sourceMemberContainerTypeSymbol })
							scriptEntryPoint = Nothing
							Return scriptEntryPoint
						ElseIf (Not sourceMemberContainerTypeSymbol.IsGenericType) Then
							Dim obj As Object = sourceMemberContainerTypeSymbol
							Dim containingSourceModule As SourceModuleSymbol = sourceMemberContainerTypeSymbol.ContainingSourceModule
							Dim syntaxReferences As ImmutableArray(Of SyntaxReference) = sourceMemberContainerTypeSymbol.SyntaxReferences
							Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(containingSourceModule, syntaxReferences(0).SyntaxTree, sourceMemberContainerTypeSymbol)
							Dim lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult = Microsoft.CodeAnalysis.VisualBasic.LookupResult.GetInstance()
							Dim lookupOption As LookupOptions = LookupOptions.AllMethodsOfAnyArity Or LookupOptions.IgnoreExtensionMethods
							Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
							binder.LookupMember(lookupResult, sourceMemberContainerTypeSymbol, "Main", 0, lookupOption, discarded)
							If (Not lookupResult.IsGoodOrAmbiguous OrElse lookupResult.Symbols(0).Kind <> SymbolKind.Method) Then
								instance.Add(ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, New [Object]() { sourceMemberContainerTypeSymbol })
								lookupResult.Free()
								scriptEntryPoint = Nothing
								Return scriptEntryPoint
							Else
								Dim enumerator2 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = lookupResult.Symbols.GetEnumerator()
								While enumerator2.MoveNext()
									Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
									If (symbol.ContainingAssembly <> Me.Assembly) Then
										Continue While
									End If
									methodSymbols.Add(DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))
								End While
								lookupResult.Free()
							End If
						Else
							instance.Add(ERRID.ERR_GenericSubMainsFound1, NoLocation.Singleton, New [Object]() { sourceMemberContainerTypeSymbol })
							scriptEntryPoint = Nothing
							Return scriptEntryPoint
						End If
					Else
						instance.Add(ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, New [Object]() { mainTypeName })
						scriptEntryPoint = Nothing
						Return scriptEntryPoint
					End If
				Else
					instance.Add(ERRID.WRN_MainIgnored, NoLocation.Singleton, New [Object]() { mainTypeName })
					scriptEntryPoint = Me.ScriptClass.GetScriptEntryPoint()
					Return scriptEntryPoint
				End If
				If (methodSymbols.Count <> 0) Then
					Dim flag As Boolean = False
					Dim instance1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
					Dim enumerator3 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = methodSymbols.GetEnumerator()
					While enumerator3.MoveNext()
						Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator3.Current
						If (Not current1.IsViableMainMethod) Then
							Continue While
						End If
						If (current1.IsGenericMethod OrElse current1.ContainingType.IsGenericType) Then
							flag = True
						Else
							instance1.Add(current1)
						End If
					End While
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					If (instance1.Count = 0) Then
						If (Not flag) Then
							instance.Add(ERRID.ERR_InValidSubMainsFound1, NoLocation.Singleton, New [Object]() { obj })
						Else
							instance.Add(ERRID.ERR_GenericSubMainsFound1, NoLocation.Singleton, New [Object]() { obj })
						End If
					ElseIf (instance1.Count <= 1) Then
						item = instance1(0)
						If (item.IsAsync) Then
							Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
							If (sourceMemberMethodSymbol IsNot Nothing) Then
								Dim nonMergedLocation As Location = sourceMemberMethodSymbol.NonMergedLocation
								If (nonMergedLocation IsNot Nothing) Then
									Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, nonMergedLocation, ERRID.ERR_AsyncSubMain)
								End If
							End If
						End If
					Else
						instance1.Sort(LexicalOrderSymbolComparer.Instance)
						instance.Add(ERRID.ERR_MoreThanOneValidMainWasFound2, NoLocation.Singleton, New [Object]() { MyBase.AssemblyName, New FormattedSymbolList(instance1.ToArray(), CustomSymbolDisplayFormatter.ErrorMessageFormatNoModifiersNoReturnType) })
					End If
					instance1.Free()
					scriptEntryPoint = item
				Else
					instance.Add(ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, New [Object]() { obj })
					scriptEntryPoint = Nothing
				End If
			Finally
				methodSymbols.Free()
				sealedDiagnostics = instance.ToReadOnlyAndFree()
			End Try
			Return scriptEntryPoint
		End Function

		Private Shared Function ForTree(ByVal tree As SyntaxTree, ByVal options As VisualBasicCompilationOptions, ByVal isSubmission As Boolean) As RootSingleNamespaceDeclaration
			Return DeclarationTreeBuilder.ForTree(tree, options.GetRootNamespaceParts(), If(options.ScriptClassName, ""), isSubmission)
		End Function

		Friend Overrides Function GenerateResourcesAndDocumentationComments(ByVal moduleBuilder As CommonPEModuleBuilder, ByVal xmlDocStream As Stream, ByVal win32Resources As Stream, ByVal useRawWin32Resources As Boolean, ByVal outputNameOverride As String, ByVal diagnostics As Microsoft.CodeAnalysis.DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken) As Boolean
			Dim flag As Boolean
			Dim name As Func(Of ModuleSymbol, String)
			Dim instance As Microsoft.CodeAnalysis.DiagnosticBag = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
			MyBase.SetupWin32Resources(moduleBuilder, win32Resources, useRawWin32Resources, instance)
			Dim manifestResources As IEnumerable(Of ResourceDescription) = moduleBuilder.ManifestResources
			Dim moduleSymbols As IEnumerable(Of ModuleSymbol) = DirectCast(Me.SourceAssembly.Modules, IEnumerable(Of ModuleSymbol)).Skip(1)
			If (VisualBasicCompilation._Closure$__.$I200-0 Is Nothing) Then
				name = Function(x As ModuleSymbol) x.Name
				VisualBasicCompilation._Closure$__.$I200-0 = name
			Else
				name = VisualBasicCompilation._Closure$__.$I200-0
			End If
			MyBase.ReportManifestResourceDuplicates(manifestResources, moduleSymbols.[Select](Of String)(name), Me.AddedModulesResourceNames(instance), instance)
			If (MyBase.FilterAndAppendAndFreeDiagnostics(diagnostics, instance, cancellationToken)) Then
				cancellationToken.ThrowIfCancellationRequested()
				Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
				Dim str As String = FileNameUtilities.ChangeExtension(outputNameOverride, Nothing)
				Dim nullable As Nullable(Of TextSpan) = Nothing
				VisualBasicCompilation.DocumentationCommentCompiler.WriteDocumentationCommentXml(Me, str, xmlDocStream, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(diagnosticBag), cancellationToken, Nothing, nullable)
				flag = MyBase.FilterAndAppendAndFreeDiagnostics(diagnostics, diagnosticBag, cancellationToken)
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shadows Function GetAssemblyOrModuleSymbol(ByVal reference As MetadataReference) As Symbol
			Dim referencedAssemblySymbol As Symbol
			Dim item As Symbol
			If (reference Is Nothing) Then
				Throw New ArgumentNullException("reference")
			End If
			If (reference.Properties.Kind <> MetadataImageKind.Assembly) Then
				Dim referencedModuleIndex As Integer = Me.GetBoundReferenceManager().GetReferencedModuleIndex(reference)
				If (referencedModuleIndex < 0) Then
					item = Nothing
				Else
					item = Me.Assembly.Modules(referencedModuleIndex)
				End If
				referencedAssemblySymbol = item
			Else
				referencedAssemblySymbol = Me.GetBoundReferenceManager().GetReferencedAssemblySymbol(reference)
			End If
			Return referencedAssemblySymbol
		End Function

		Friend Shadows Function GetBoundReferenceManager() As VisualBasicCompilation.ReferenceManager
			If (Me._lazyAssemblySymbol Is Nothing) Then
				Me._referenceManager.CreateSourceAssemblyForCompilation(Me)
			End If
			Return Me._referenceManager
		End Function

		Friend Shadows Function GetCompilationNamespace(ByVal namespaceSymbol As INamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim globalNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			If (namespaceSymbol Is Nothing) Then
				Throw New ArgumentNullException("namespaceSymbol")
			End If
			Dim namespaceSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(namespaceSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			If (namespaceSymbol1 IsNot Nothing AndAlso namespaceSymbol1.Extent.Kind = NamespaceKind.Compilation AndAlso namespaceSymbol1.Extent.Compilation = Me) Then
				globalNamespace = namespaceSymbol1
			ElseIf (namespaceSymbol.ContainingNamespace IsNot Nothing) Then
				Dim compilationNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GetCompilationNamespace(namespaceSymbol.ContainingNamespace)
				If (compilationNamespace IsNot Nothing) Then
					Dim members As ImmutableArray(Of Symbol) = compilationNamespace.GetMembers(namespaceSymbol.Name)
					globalNamespace = members.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)().FirstOrDefault()
				Else
					globalNamespace = Nothing
				End If
			Else
				globalNamespace = Me.GlobalNamespace
			End If
			Return globalNamespace
		End Function

		Private Function GetCompleteSetOfUsedAssemblies(ByVal cancellationToken As System.Threading.CancellationToken) As ConcurrentSet(Of AssemblySymbol)
			If (Not Me._usedAssemblyReferencesFrozen AndAlso Not Volatile.Read(Me._usedAssemblyReferencesFrozen)) Then
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(DiagnosticBag.GetInstance(), New ConcurrentSet(Of AssemblySymbol)())
				Me.GetDiagnosticsWithoutFiltering(CompilationStage.[Declare], True, bindingDiagnosticBag, cancellationToken)
				Dim flag As Boolean = bindingDiagnosticBag.HasAnyErrors()
				If (Not flag) Then
					bindingDiagnosticBag.DiagnosticBag.Clear()
					Me.GetDiagnosticsForAllMethodBodies(False, bindingDiagnosticBag, True, cancellationToken)
					flag = bindingDiagnosticBag.HasAnyErrors()
					If (Not flag) Then
						Me.AddUsedAssemblies(bindingDiagnosticBag.DependenciesBag)
					End If
				End If
				Me.CompleteTheSetOfUsedAssemblies(flag, cancellationToken)
				bindingDiagnosticBag.DiagnosticBag.Free()
			End If
			Return Me._lazyUsedAssemblyReferences
		End Function

		Public Overrides Function GetDeclarationDiagnostics(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me.GetDiagnostics(CompilationStage.[Declare], False, cancellationToken)
		End Function

		Public Overrides Function GetDiagnostics(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me.GetDiagnostics(CompilationStage.Compile, True, cancellationToken)
		End Function

		Friend Function GetDiagnostics(ByVal stage As CompilationStage, Optional ByVal includeEarlierStages As Boolean = True, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Me.GetDiagnostics(stage, includeEarlierStages, instance, cancellationToken)
			Return instance.ToReadOnlyAndFree()
		End Function

		Friend Overrides Sub GetDiagnostics(ByVal stage As CompilationStage, ByVal includeEarlierStages As Boolean, ByVal diagnostics As DiagnosticBag, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Me.GetDiagnosticsWithoutFiltering(stage, includeEarlierStages, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(instance), cancellationToken)
			MyBase.FilterAndAppendAndFreeDiagnostics(diagnostics, instance, cancellationToken)
		End Sub

		Private Sub GetDiagnosticsForAllMethodBodies(ByVal hasDeclarationErrors As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal doLowering As Boolean, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim nullable As Nullable(Of TextSpan) = Nothing
			MethodCompiler.GetCompileDiagnostics(Me, Me.SourceModule.GlobalNamespace, Nothing, nullable, hasDeclarationErrors, diagnostics, doLowering, cancellationToken)
			nullable = Nothing
			VisualBasicCompilation.DocumentationCommentCompiler.WriteDocumentationCommentXml(Me, Nothing, Nothing, diagnostics, cancellationToken, Nothing, nullable)
			Me.ReportUnusedImports(Nothing, diagnostics, cancellationToken)
		End Sub

		Private Sub GetDiagnosticsForMethodBodiesInTree(ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal hasDeclarationErrors As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim sourceModule As SourceModuleSymbol = DirectCast(Me.SourceModule, SourceModuleSymbol)
			MethodCompiler.GetCompileDiagnostics(Me, Me.SourceModule.GlobalNamespace, tree, filterSpanWithinTree, hasDeclarationErrors, diagnostics, False, cancellationToken)
			VisualBasicCompilation.DocumentationCommentCompiler.WriteDocumentationCommentXml(Me, Nothing, Nothing, diagnostics, cancellationToken, tree, filterSpanWithinTree)
			If (Not filterSpanWithinTree.HasValue OrElse filterSpanWithinTree.Value = tree.GetRoot(cancellationToken).FullSpan) Then
				Me.ReportUnusedImports(tree, diagnostics, cancellationToken)
			End If
		End Sub

		Friend Function GetDiagnosticsForSyntaxTree(ByVal stage As CompilationStage, ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal includeEarlierStages As Boolean, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Dim enumerator As IEnumerator(Of Diagnostic) = Nothing
			If (Not Me.SyntaxTrees.Contains(tree)) Then
				Throw New ArgumentException("Cannot GetDiagnosticsForSyntax for a tree that is not part of the compilation", "tree")
			End If
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Microsoft.CodeAnalysis.DiagnosticBag.GetInstance())
			If (stage = CompilationStage.Parse OrElse stage > CompilationStage.Parse AndAlso includeEarlierStages) Then
				cancellationToken.ThrowIfCancellationRequested()
				Dim diagnostics As IEnumerable(Of Diagnostic) = tree.GetDiagnostics(cancellationToken)
				bindingDiagnosticBag.AddRange(VisualBasicCompilation.FilterDiagnosticsByLocation(diagnostics, tree, filterSpanWithinTree))
			End If
			If (stage = CompilationStage.[Declare] OrElse stage > CompilationStage.[Declare] AndAlso includeEarlierStages) Then
				Dim diagnostics1 As IEnumerable(Of Diagnostic) = VisualBasicCompilation.FilterDiagnosticsByLocation(DirectCast(DirectCast(Me.SourceModule, SourceModuleSymbol).GetDeclarationErrorsInTree(tree, filterSpanWithinTree, New Func(Of IEnumerable(Of Diagnostic), SyntaxTree, Nullable(Of TextSpan), IEnumerable(Of Diagnostic))(AddressOf VisualBasicCompilation.FilterDiagnosticsByLocation), cancellationToken), IEnumerable(Of Diagnostic)), tree, filterSpanWithinTree)
				bindingDiagnosticBag.AddRange(diagnostics1)
				Me.AddClsComplianceDiagnostics(bindingDiagnosticBag, cancellationToken, tree, filterSpanWithinTree)
			End If
			If (stage = CompilationStage.Compile OrElse stage > CompilationStage.Compile AndAlso includeEarlierStages) Then
				Dim bindingDiagnosticBag1 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Microsoft.CodeAnalysis.DiagnosticBag.GetInstance())
				Me.GetDiagnosticsForMethodBodiesInTree(tree, filterSpanWithinTree, bindingDiagnosticBag.HasAnyErrors(), bindingDiagnosticBag1, cancellationToken)
				If (Not bindingDiagnosticBag1.DiagnosticBag.IsEmptyWithoutResolution) Then
					Using diagnostics2 As IEnumerable(Of Diagnostic) = VisualBasicCompilation.FilterDiagnosticsByLocation(bindingDiagnosticBag1.DiagnosticBag.AsEnumerableWithoutResolution(), tree, filterSpanWithinTree)
						enumerator = diagnostics2.GetEnumerator()
						While enumerator.MoveNext()
							bindingDiagnosticBag.Add(enumerator.Current)
						End While
					End Using
				End If
			End If
			Dim instance As Microsoft.CodeAnalysis.DiagnosticBag = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
			Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = bindingDiagnosticBag.DiagnosticBag
			MyBase.FilterAndAppendAndFreeDiagnostics(instance, diagnosticBag, cancellationToken)
			Return instance.ToReadOnlyAndFree(Of Diagnostic)()
		End Function

		Private Sub GetDiagnosticsWithoutFiltering(ByVal stage As CompilationStage, ByVal includeEarlierStages As Boolean, ByVal builder As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			Dim variable As VisualBasicCompilation._Closure$__183-0 = Nothing
			Dim assemblySymbols As ICollection(Of AssemblySymbol)
			variable = New VisualBasicCompilation._Closure$__183-0(variable) With
			{
				.$VB$Me = Me,
				.$VB$Local_builder = builder,
				.$VB$Local_cancellationToken = cancellationToken
			}
			If (stage = CompilationStage.Parse OrElse stage > CompilationStage.Parse AndAlso includeEarlierStages) Then
				If (Not Me.Options.ConcurrentBuild) Then
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxTree).Enumerator = Me.SyntaxTrees.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.SyntaxTree = enumerator.Current
						variable.$VB$Local_cancellationToken.ThrowIfCancellationRequested()
						variable.$VB$Local_builder.AddRange(current.GetDiagnostics(variable.$VB$Local_cancellationToken))
					End While
				Else
					Dim syntaxTrees As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxTree) = Me.SyntaxTrees
					RoslynParallel.[For](0, syntaxTrees.Length, UICultureUtilities.WithCurrentUICulture(Of Integer)(Sub(i As Integer) Me.$VB$Local_builder.AddRange(Me.$VB$Me.SyntaxTrees(i).GetDiagnostics(Me.$VB$Local_cancellationToken))), variable.$VB$Local_cancellationToken)
				End If
				Dim parseOptions As HashSet(Of Microsoft.CodeAnalysis.ParseOptions) = New HashSet(Of Microsoft.CodeAnalysis.ParseOptions)()
				If (Me.Options.ParseOptions IsNot Nothing) Then
					parseOptions.Add(Me.Options.ParseOptions)
				End If
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxTree).Enumerator = Me.SyntaxTrees.GetEnumerator()
				While enumerator1.MoveNext()
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = enumerator1.Current
					variable.$VB$Local_cancellationToken.ThrowIfCancellationRequested()
					If (syntaxTree.Options.Errors.IsDefaultOrEmpty OrElse Not parseOptions.Add(syntaxTree.Options)) Then
						Continue While
					End If
					Dim location As Microsoft.CodeAnalysis.Location = syntaxTree.GetLocation(TextSpan.FromBounds(0, 0))
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.Diagnostic).Enumerator = syntaxTree.Options.Errors.GetEnumerator()
					While enumerator2.MoveNext()
						Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic = enumerator2.Current
						variable.$VB$Local_builder.Add(diagnostic.WithLocation(location))
					End While
				End While
			End If
			If (stage = CompilationStage.[Declare] OrElse stage > CompilationStage.[Declare] AndAlso includeEarlierStages) Then
				MyBase.CheckAssemblyName(variable.$VB$Local_builder.DiagnosticBag)
				variable.$VB$Local_builder.AddRange(Of Microsoft.CodeAnalysis.Diagnostic)(Me.Options.Errors)
				variable.$VB$Local_builder.AddRange(Of Microsoft.CodeAnalysis.Diagnostic)(Me.GetBoundReferenceManager().Diagnostics)
				Me.SourceAssembly.GetAllDeclarationErrors(variable.$VB$Local_builder, variable.$VB$Local_cancellationToken)
				Me.AddClsComplianceDiagnostics(variable.$VB$Local_builder, variable.$VB$Local_cancellationToken, Nothing, Nothing)
				If (MyBase.EventQueue IsNot Nothing AndAlso Me.SyntaxTrees.Length = 0) Then
					MyBase.EnsureCompilationEventQueueCompleted()
				End If
			End If
			If (stage = CompilationStage.Compile OrElse stage > CompilationStage.Compile AndAlso includeEarlierStages) Then
				Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
				If (variable.$VB$Local_builder.AccumulatesDependencies) Then
					assemblySymbols = New ConcurrentSet(Of AssemblySymbol)()
				Else
					assemblySymbols = Nothing
				End If
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(instance, assemblySymbols)
				Me.GetDiagnosticsForAllMethodBodies(variable.$VB$Local_builder.HasAnyErrors(), bindingDiagnosticBag, False, variable.$VB$Local_cancellationToken)
				variable.$VB$Local_builder.AddRange(bindingDiagnosticBag, False)
				bindingDiagnosticBag.DiagnosticBag.Free()
			End If
		End Sub

		Friend Shadows Function GetEntryPoint(ByVal cancellationToken As System.Threading.CancellationToken) As MethodSymbol
			Dim entryPointAndDiagnostics As VisualBasicCompilation.EntryPoint = Me.GetEntryPointAndDiagnostics(cancellationToken)
			If (entryPointAndDiagnostics Is Nothing) Then
				Return Nothing
			End If
			Return entryPointAndDiagnostics.MethodSymbol
		End Function

		Friend Function GetEntryPointAndDiagnostics(ByVal cancellationToken As System.Threading.CancellationToken) As VisualBasicCompilation.EntryPoint
			Dim entryPoint As VisualBasicCompilation.EntryPoint
			If (Not Me.Options.OutputKind.IsApplication() AndAlso Me.ScriptClass Is Nothing) Then
				entryPoint = Nothing
			ElseIf (Me.Options.MainTypeName Is Nothing OrElse StringExtensions.IsValidClrTypeName(Me.Options.MainTypeName)) Then
				If (Me._lazyEntryPoint Is Nothing) Then
					Dim diagnostics As ImmutableArray(Of Diagnostic) = New ImmutableArray(Of Diagnostic)()
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.FindEntryPoint(cancellationToken, diagnostics)
					Interlocked.CompareExchange(Of VisualBasicCompilation.EntryPoint)(Me._lazyEntryPoint, New VisualBasicCompilation.EntryPoint(methodSymbol, diagnostics), Nothing)
				End If
				entryPoint = Me._lazyEntryPoint
			Else
				entryPoint = New VisualBasicCompilation.EntryPoint(Nothing, ImmutableArray(Of Diagnostic).Empty)
			End If
			Return entryPoint
		End Function

		Friend Function GetExtensionAttributeConstructor(<Out> ByRef useSiteInfo As UseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol)
			Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved
			Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Nothing
			If (Me._lazyExtensionAttributeConstructor = ErrorTypeSymbol.UnknownResultType) Then
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GlobalNamespace.LookupNestedNamespace(ImmutableArray.Create(Of String)("System", "Runtime", "CompilerServices"))
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				Dim sourceModule As SourceModuleSymbol = DirectCast(Me.SourceModule, SourceModuleSymbol)
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateSourceModuleBinder(sourceModule)
				If (namespaceSymbol IsNot Nothing) Then
					Dim typeMembers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = namespaceSymbol.GetTypeMembers(AttributeDescription.CaseInsensitiveExtensionAttribute.Name, 0)
					Dim flag As Boolean = False
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
						discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
						If (Not binder.IsAccessible(current1, discarded, Nothing, basesBeingResolved)) Then
							Continue While
						End If
						If (current1.ContainingModule = sourceModule) Then
							namedTypeSymbol = current1
							flag = False
							Exit While
						ElseIf (namedTypeSymbol Is Nothing) Then
							namedTypeSymbol = current1
						ElseIf (CObj(current1.ContainingAssembly) <> CObj(Me.Assembly)) Then
							If (CObj(namedTypeSymbol.ContainingAssembly) = CObj(Me.Assembly)) Then
								Continue While
							End If
							flag = True
						ElseIf (CObj(namedTypeSymbol.ContainingAssembly) <> CObj(Me.Assembly)) Then
							namedTypeSymbol = current1
							flag = False
						Else
							flag = True
						End If
					End While
					If (flag) Then
						namedTypeSymbol = Nothing
					End If
				End If
				If (namedTypeSymbol IsNot Nothing AndAlso Not namedTypeSymbol.IsStructureType() AndAlso Not namedTypeSymbol.IsMustInherit) Then
					Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Attribute)
					discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					If (wellKnownType.IsBaseTypeOf(namedTypeSymbol, discarded)) Then
						discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
						If (binder.IsAccessible(namedTypeSymbol, discarded, Nothing, basesBeingResolved)) Then
							Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = namedTypeSymbol.InstanceConstructors.GetEnumerator()
							Do
								If (Not enumerator1.MoveNext()) Then
									GoTo Label0
								End If
								current = enumerator1.Current
							Loop While current.ParameterCount <> 0
							discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
							basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
							If (binder.IsAccessible(current, discarded, Nothing, basesBeingResolved)) Then
								methodSymbol = current
							End If
						End If
					End If
				End If
				If (methodSymbol Is Nothing) Then
					Dim objArray(0) As [Object]
					Dim caseInsensitiveExtensionAttribute As AttributeDescription = AttributeDescription.CaseInsensitiveExtensionAttribute
					objArray(0) = [String].Concat(caseInsensitiveExtensionAttribute.FullName, "..ctor")
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, objArray)
				ElseIf ((methodSymbol.ContainingType.GetAttributeUsageInfo().ValidTargets And (AttributeTargets.Assembly Or AttributeTargets.[Class] Or AttributeTargets.Method)) <> (AttributeTargets.Assembly Or AttributeTargets.[Class] Or AttributeTargets.Method)) Then
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionAttributeInvalid)
				End If
				Me._lazyExtensionAttributeConstructorErrorInfo = diagnosticInfo
				Interlocked.CompareExchange(Of Symbol)(Me._lazyExtensionAttributeConstructor, methodSymbol, ErrorTypeSymbol.UnknownResultType)
			End If
			methodSymbol = DirectCast(Me._lazyExtensionAttributeConstructor, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			diagnosticInfo = DirectCast(Volatile.Read(Of Object)(Me._lazyExtensionAttributeConstructorErrorInfo), Microsoft.CodeAnalysis.DiagnosticInfo)
			If (diagnosticInfo Is Nothing) Then
				useSiteInfo = Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForMemberAndContainingType(methodSymbol)
			Else
				useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(diagnosticInfo)
			End If
			Return methodSymbol
		End Function

		Friend Function GetMetadataReference(ByVal assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) As MetadataReference
			Return Me.GetBoundReferenceManager().GetMetadataReference(assemblySymbol)
		End Function

		Public Overrides Function GetMethodBodyDiagnostics(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me.GetDiagnostics(CompilationStage.Compile, False, cancellationToken)
		End Function

		Public Overrides Function GetParseDiagnostics(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me.GetDiagnostics(CompilationStage.Parse, False, cancellationToken)
		End Function

		Friend Shared Function GetRuntimeMember(ByVal declaringType As NamedTypeSymbol, ByRef descriptor As Microsoft.CodeAnalysis.RuntimeMembers.MemberDescriptor, ByVal comparer As SignatureComparer(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, FieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, TypeSymbol, ParameterSymbol), ByVal accessWithinOpt As AssemblySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbolKind As Microsoft.CodeAnalysis.SymbolKind
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = Microsoft.CodeAnalysis.MethodKind.Ordinary
			Dim flags As Boolean = CInt((descriptor.Flags And MemberFlags.[Static])) <> 0
			Dim memberFlag As MemberFlags = descriptor.Flags And MemberFlags.KindMask
			Select Case memberFlag
				Case MemberFlags.Method
					symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method
					Exit Select
				Case MemberFlags.Field
					symbolKind = Microsoft.CodeAnalysis.SymbolKind.Field
					Exit Select
				Case MemberFlags.Method Or MemberFlags.Field
					Throw ExceptionUtilities.UnexpectedValue(descriptor.Flags)
				Case MemberFlags.Constructor
					symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method
					methodKind = Microsoft.CodeAnalysis.MethodKind.Constructor
					Exit Select
				Case Else
					If (memberFlag = MemberFlags.PropertyGet) Then
						symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method
						methodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet
						Exit Select
					ElseIf (memberFlag = MemberFlags.[Property]) Then
						symbolKind = Microsoft.CodeAnalysis.SymbolKind.[Property]
						Exit Select
					Else
						Throw ExceptionUtilities.UnexpectedValue(descriptor.Flags)
					End If
			End Select
			Dim members As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = declaringType.GetMembers(descriptor.Name)
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
				If (current.Kind <> symbolKind OrElse current.IsShared <> flags OrElse current.DeclaredAccessibility <> Accessibility.[Public] AndAlso (accessWithinOpt Is Nothing OrElse Not Microsoft.CodeAnalysis.VisualBasic.Symbol.IsSymbolAccessible(current, accessWithinOpt)) OrElse Not [String].Equals(current.Name, descriptor.Name, StringComparison.Ordinal)) Then
					Continue While
				End If
				If (symbolKind <> Microsoft.CodeAnalysis.SymbolKind.Field) Then
					If (symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method) Then
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						Dim methodKind1 As Microsoft.CodeAnalysis.MethodKind = methodSymbol.MethodKind
						If (methodKind1 = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind1 = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) Then
							methodKind1 = Microsoft.CodeAnalysis.MethodKind.Ordinary
						End If
						If (methodSymbol.Arity <> descriptor.Arity OrElse methodKind1 <> methodKind) Then
							Continue While
						End If
						If (CInt((descriptor.Flags And MemberFlags.Virtual)) <> 0 <> If(methodSymbol.IsOverridable OrElse methodSymbol.IsOverrides, True, methodSymbol.IsMustOverride)) Then
							Continue While
						End If
						If (Not comparer.MatchMethodSignature(methodSymbol, descriptor.Signature)) Then
							Continue While
						End If
					Else
						If (symbolKind <> Microsoft.CodeAnalysis.SymbolKind.[Property]) Then
							Throw ExceptionUtilities.UnexpectedValue(symbolKind)
						End If
						Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
						If (CInt((descriptor.Flags And MemberFlags.Virtual)) <> 0 <> If(propertySymbol.IsOverridable OrElse propertySymbol.IsOverrides, True, propertySymbol.IsMustOverride)) Then
							Continue While
						End If
						If (Not comparer.MatchPropertySignature(propertySymbol, descriptor.Signature)) Then
							Continue While
						End If
					End If
				ElseIf (Not comparer.MatchFieldSignature(DirectCast(current, FieldSymbol), descriptor.Signature)) Then
					Continue While
				End If
				If (symbol Is Nothing) Then
					symbol = current
				Else
					symbol = Nothing
					Exit While
				End If
			End While
			Return symbol
		End Function

		Friend Function GetRuntimeMetadataVersion() As String
			Dim corLibrary As PEAssemblySymbol = TryCast(Me.Assembly.CorLibrary, PEAssemblySymbol)
			If (corLibrary Is Nothing) Then
				Return [String].Empty
			End If
			Return corLibrary.Assembly.ManifestModule.MetadataVersion
		End Function

		Public Shadows Function GetSemanticModel(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, Optional ByVal ignoreAccessibility As Boolean = False) As Microsoft.CodeAnalysis.SemanticModel
			Dim semanticModel As Microsoft.CodeAnalysis.SemanticModel = Nothing
			If (MyBase.SemanticModelProvider IsNot Nothing) Then
				semanticModel = MyBase.SemanticModelProvider.GetSemanticModel(syntaxTree, Me, ignoreAccessibility)
			End If
			Return If(semanticModel, Me.CreateSemanticModel(syntaxTree, ignoreAccessibility))
		End Function

		Friend Shadows Function GetSpecialType(ByVal typeId As SpecialType) As NamedTypeSymbol
			Return Me.Assembly.GetSpecialType(typeId)
		End Function

		Friend Function GetSpecialTypeMember(ByVal memberId As SpecialMember) As Symbol
			Return Me.Assembly.GetSpecialTypeMember(memberId)
		End Function

		Friend Function GetSubmissionInitializer() As SynthesizedInteractiveInitializerMethod
			If (Not MyBase.IsSubmission OrElse Me.ScriptClass Is Nothing) Then
				Return Nothing
			End If
			Return Me.ScriptClass.GetScriptInitializer()
		End Function

		Public Overrides Function GetSymbolsWithName(ByVal predicate As Func(Of String, Boolean), Optional ByVal filter As SymbolFilter = 6, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEnumerable(Of ISymbol)
			If (predicate Is Nothing) Then
				Throw New ArgumentNullException("predicate")
			End If
			If (filter = SymbolFilter.None) Then
				Throw New ArgumentException(VBResources.NoNoneSearchCriteria, "filter")
			End If
			Return (New VisualBasicCompilation.PredicateSymbolSearcher(Me, filter, predicate, cancellationToken)).GetSymbolsWithName()
		End Function

		Public Overrides Function GetSymbolsWithName(ByVal name As String, Optional ByVal filter As SymbolFilter = 6, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEnumerable(Of ISymbol)
			If (name Is Nothing) Then
				Throw New ArgumentNullException("name")
			End If
			If (filter = SymbolFilter.None) Then
				Throw New ArgumentException(VBResources.NoNoneSearchCriteria, "filter")
			End If
			Return (New VisualBasicCompilation.NameSymbolSearcher(Me, filter, name, cancellationToken)).GetSymbolsWithName()
		End Function

		Friend Overrides Function GetSyntaxTreeOrdinal(ByVal tree As SyntaxTree) As Integer
			Return Me._syntaxTreeOrdinalMap(tree)
		End Function

		Friend Shadows Function GetTypeByMetadataName(ByVal fullyQualifiedMetadataName As String) As NamedTypeSymbol
			Dim assembly As AssemblySymbol = Me.Assembly
			Dim valueTuple As ValueTuple(Of AssemblySymbol, AssemblySymbol) = New ValueTuple(Of AssemblySymbol, AssemblySymbol)()
			Return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName, True, False, valueTuple, False, False)
		End Function

		Friend Function GetTypeByReflectionType(ByVal type As System.Type) As TypeSymbol
			Return Me.GetSpecialType(SpecialType.System_Object)
		End Function

		Public Overrides Function GetUsedAssemblyReferences(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of MetadataReference)
			Dim immutableAndFree As ImmutableArray(Of MetadataReference)
			Dim enumerator As IEnumerator(Of MetadataReference) = Nothing
			Dim completeSetOfUsedAssemblies As ConcurrentSet(Of AssemblySymbol) = Me.GetCompleteSetOfUsedAssemblies(cancellationToken)
			If (completeSetOfUsedAssemblies IsNot Nothing) Then
				Using instance As ArrayBuilder(Of MetadataReference) = ArrayBuilder(Of MetadataReference).GetInstance(completeSetOfUsedAssemblies.Count)
					enumerator = MyBase.References.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As MetadataReference = enumerator.Current
						If (current.Properties.Kind <> MetadataImageKind.Assembly) Then
							Continue While
						End If
						Dim referencedAssemblySymbol As Symbol = Me.GetBoundReferenceManager().GetReferencedAssemblySymbol(current)
						If (referencedAssemblySymbol Is Nothing OrElse Not completeSetOfUsedAssemblies.Contains(DirectCast(referencedAssemblySymbol, AssemblySymbol))) Then
							Continue While
						End If
						instance.Add(current)
					End While
				End Using
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of MetadataReference).Empty
			End If
			Return immutableAndFree
		End Function

		Friend Function GetWellKnownType(ByVal type As WellKnownType) As NamedTypeSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation::GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Function GetWellKnownTypeMember(ByVal member As WellKnownMember) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			If (Not MyBase.IsMemberMissing(member)) Then
				If (Me._lazyWellKnownTypeMembers Is Nothing OrElse Me._lazyWellKnownTypeMembers(CInt(member)) = ErrorTypeSymbol.UnknownResultType) Then
					If (Me._lazyWellKnownTypeMembers Is Nothing) Then
						Dim unknownResultType(417) As Microsoft.CodeAnalysis.VisualBasic.Symbol
						Dim length As Integer = CInt(unknownResultType.Length) - 1
						Dim num As Integer = 0
						Do
							unknownResultType(num) = ErrorTypeSymbol.UnknownResultType
							num = num + 1
						Loop While num <= length
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbol())(Me._lazyWellKnownTypeMembers, unknownResultType, Nothing)
					End If
					Dim descriptor As Microsoft.CodeAnalysis.RuntimeMembers.MemberDescriptor = WellKnownMembers.GetDescriptor(member)
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = If(descriptor.DeclaringTypeId <= 45, Me.GetSpecialType(DirectCast(CSByte(descriptor.DeclaringTypeId), SpecialType)), Me.GetWellKnownType(DirectCast(descriptor.DeclaringTypeId, WellKnownType)))
					Dim runtimeMember As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
					If (Not namedTypeSymbol.IsErrorType()) Then
						runtimeMember = VisualBasicCompilation.GetRuntimeMember(namedTypeSymbol, descriptor, Me._wellKnownMemberSignatureComparer, Me.Assembly)
					End If
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(Me._lazyWellKnownTypeMembers(CInt(member)), runtimeMember, ErrorTypeSymbol.UnknownResultType)
				End If
				symbol = Me._lazyWellKnownTypeMembers(CInt(member))
			Else
				symbol = Nothing
			End If
			Return symbol
		End Function

		Friend Overrides Function HasCodeToEmit() As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of SyntaxTree).Enumerator = Me.SyntaxTrees.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (enumerator.Current.GetCompilationUnitRoot().Members.Count > 0) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Friend Overrides Function HasSubmissionResult() As Boolean
			Dim specialType As Boolean
			Dim cancellationToken As System.Threading.CancellationToken
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = System.Linq.ImmutableArrayExtensions.SingleOrDefault(Of Microsoft.CodeAnalysis.SyntaxTree)(Me.SyntaxTrees)
			If (syntaxTree IsNot Nothing) Then
				Dim compilationUnitRoot As CompilationUnitSyntax = syntaxTree.GetCompilationUnitRoot()
				If (Not compilationUnitRoot.HasErrors) Then
					Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = compilationUnitRoot.Members.LastOrDefault()
					If (statementSyntax IsNot Nothing) Then
						Dim semanticModel As Microsoft.CodeAnalysis.SemanticModel = Me.GetSemanticModel(syntaxTree, False)
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = statementSyntax.Kind()
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement) Then
							Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(statementSyntax, ExpressionStatementSyntax).Expression
							cancellationToken = New System.Threading.CancellationToken()
							Dim typeInfo As Microsoft.CodeAnalysis.TypeInfo = semanticModel.GetTypeInfo(expression, cancellationToken)
							specialType = typeInfo.Type.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Void
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement) Then
							Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(statementSyntax, PrintStatementSyntax).Expression
							cancellationToken = New System.Threading.CancellationToken()
							semanticModel.GetTypeInfo(expressionSyntax, cancellationToken)
							specialType = True
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement) Then
							Dim invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(statementSyntax, CallStatementSyntax).Invocation
							cancellationToken = New System.Threading.CancellationToken()
							Dim typeInfo1 As Microsoft.CodeAnalysis.TypeInfo = semanticModel.GetTypeInfo(invocation, cancellationToken)
							specialType = typeInfo1.Type.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Void
						Else
							specialType = False
						End If
					Else
						specialType = False
					End If
				Else
					specialType = False
				End If
			Else
				specialType = False
			End If
			Return specialType
		End Function

		Private Function IncludeInternalXmlHelper() As Boolean
			If (Me.Options.SuppressEmbeddedDeclarations OrElse Not Me.InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Linq_Enumerable) OrElse Not Me.InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XElement) OrElse Not Me.InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XName) OrElse Not Me.InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XAttribute)) Then
				Return False
			End If
			Return Me.InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XNamespace)
		End Function

		Private Function InternalXmlHelperDependencyIsSatisfied(ByVal type As WellKnownType) As Boolean
			Dim flag As Boolean
			Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(type.GetMetadataName(), True, -1)
			Dim sourceAssembly As SourceAssemblySymbol = Me.SourceAssembly
			Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = sourceAssembly.SourceModule.GetReferencedAssemblySymbols().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current.LookupTopLevelMetadataType(metadataTypeName, False)
					If (sourceAssembly.IsValidWellKnownType(namedTypeSymbol) AndAlso AssemblySymbol.IsAcceptableMatchForGetTypeByNameAndArity(namedTypeSymbol)) Then
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Friend Overrides Function IsAttributeType(ByVal type As ITypeSymbol) As Boolean
			Dim flag As Boolean
			If (type.Kind = SymbolKind.NamedType) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				flag = namedTypeSymbol.IsOrDerivedFromWellKnownClass(WellKnownType.System_Attribute, Me, discarded)
			Else
				flag = False
			End If
			Return flag
		End Function

		<Obsolete("Compilation.IsSymbolAccessibleWithin is not designed for use within the compilers", True)>
		Friend Shadows Function IsSymbolAccessibleWithin(ByVal symbol As ISymbol, ByVal within As ISymbol, Optional ByVal throughType As ITypeSymbol = Nothing) As Boolean
			Throw New NotImplementedException()
		End Function

		Protected Friend Overrides Function IsSymbolAccessibleWithinCore(ByVal symbol As ISymbol, ByVal within As ISymbol, ByVal throughType As ITypeSymbol) As Boolean
			Dim discarded As CompoundUseSiteInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved
			Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbol.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)("symbol")
			Dim symbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbol = within.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)("within")
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = throughType.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("throughType")
			If (symbol2.Kind = SymbolKind.Assembly) Then
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = DirectCast(symbol2, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
				discarded = CompoundUseSiteInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Discarded
				basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				Return AccessCheck.IsSymbolAccessible(symbol1, assemblySymbol, discarded, basesBeingResolved)
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(symbol2, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			discarded = CompoundUseSiteInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Discarded
			basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
			Return AccessCheck.IsSymbolAccessible(symbol1, namedTypeSymbol, typeSymbol, discarded, basesBeingResolved)
		End Function

		Friend Overrides Function IsSystemTypeReference(ByVal type As ITypeSymbolInternal) As Boolean
			Return TypeSymbol.Equals(DirectCast(type, TypeSymbol), Me.GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.ConsiderEverything)
		End Function

		Friend Overrides Function IsUnreferencedAssemblyIdentityDiagnosticCode(ByVal code As Integer) As Boolean
			Dim flag As Boolean
			Dim num As Integer = code
			flag = If(num = 30005 OrElse num = 30652, True, False)
			Return flag
		End Function

		Private Shared Function MakeCheckSumBytes(ByVal bytesText As String) As ImmutableArray(Of Byte)
			Dim instance As ArrayBuilder(Of Byte) = ArrayBuilder(Of Byte).GetInstance()
			Dim length As Integer = bytesText.Length / 2 - 1
			Dim num As Integer = 0
			Do
				Dim num1 As Byte = CByte((' 
				' Current member / type: System.Collections.Immutable.ImmutableArray`1<System.Byte> Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation::MakeCheckSumBytes(System.String)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Collections.Immutable.ImmutableArray<System.Byte> MakeCheckSumBytes(System.String)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1210
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1196
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 162
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 827
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 822
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1121
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 93
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Friend Function MightContainNoPiaLocalTypes() As Boolean
			Return Me.SourceAssembly.MightContainNoPiaLocalTypes()
		End Function

		Friend Sub RecordImports(ByVal syntax As ImportsStatementSyntax)
			LazyInitializer.EnsureInitialized(Of ConcurrentQueue(Of VisualBasicCompilation.ImportInfo))(Me._lazyImportInfos).Enqueue(New VisualBasicCompilation.ImportInfo(syntax))
		End Sub

		Friend Sub RecordImportsClauseDependencies(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal importsClausePosition As Integer, ByVal dependencies As ImmutableArray(Of AssemblySymbol))
			If (Not dependencies.IsDefaultOrEmpty) Then
				LazyInitializer.EnsureInitialized(Of ConcurrentDictionary(Of ValueTuple(Of Microsoft.CodeAnalysis.SyntaxTree, Integer), ImmutableArray(Of AssemblySymbol)))(Me._lazyImportClauseDependencies).TryAdd(New ValueTuple(Of Microsoft.CodeAnalysis.SyntaxTree, Integer)(syntaxTree, importsClausePosition), dependencies)
			End If
		End Sub

		Friend Function ReferenceManagerEquals(ByVal other As VisualBasicCompilation) As Boolean
			Return Me._referenceManager = other._referenceManager
		End Function

		Public Shadows Function RemoveAllReferences() As VisualBasicCompilation
			Return DirectCast(MyBase.RemoveAllReferences(), VisualBasicCompilation)
		End Function

		Public Shadows Function RemoveAllSyntaxTrees() As VisualBasicCompilation
			Return Me.UpdateSyntaxTrees(ImmutableArray(Of SyntaxTree).Empty, ImmutableDictionary.Create(Of SyntaxTree, Integer)(), ImmutableDictionary.Create(Of SyntaxTree, DeclarationTableEntry)(), VisualBasicCompilation.AddEmbeddedTrees(DeclarationTable.Empty, Me._embeddedTrees), Me._declarationTable.ReferenceDirectives.Any())
		End Function

		Private Shared Function RemoveEmbeddedTrees(ByVal declTable As DeclarationTable, ByVal embeddedTrees As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration)) As DeclarationTable
			Dim enumerator As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration).Enumerator = embeddedTrees.GetEnumerator()
			While enumerator.MoveNext()
				declTable = declTable.RemoveRootDeclaration(enumerator.Current.DeclarationEntry)
			End While
			Return declTable
		End Function

		Public Shadows Function RemoveReferences(ByVal ParamArray references As MetadataReference()) As VisualBasicCompilation
			Return DirectCast(MyBase.RemoveReferences(references), VisualBasicCompilation)
		End Function

		Public Shadows Function RemoveReferences(ByVal references As IEnumerable(Of MetadataReference)) As VisualBasicCompilation
			Return DirectCast(MyBase.RemoveReferences(references), VisualBasicCompilation)
		End Function

		Private Shared Sub RemoveSyntaxTreeFromDeclarationMapAndTable(ByVal tree As SyntaxTree, ByRef declMap As ImmutableDictionary(Of SyntaxTree, Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTableEntry), ByRef declTable As DeclarationTable, ByRef referenceDirectivesChanged As Boolean)
			Dim declarationTableEntry As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTableEntry = Nothing
			If (Not declMap.TryGetValue(tree, declarationTableEntry)) Then
				Throw New ArgumentException([String].Format(VBResources.SyntaxTreeNotFoundToRemove, tree))
			End If
			declTable = declTable.RemoveRootDeclaration(declarationTableEntry)
			declMap = declMap.Remove(tree)
			referenceDirectivesChanged = If(referenceDirectivesChanged, True, tree.HasReferenceDirectives())
		End Sub

		Public Shadows Function RemoveSyntaxTrees(ByVal ParamArray trees As SyntaxTree()) As VisualBasicCompilation
			Return Me.RemoveSyntaxTrees(DirectCast(trees, IEnumerable(Of SyntaxTree)))
		End Function

		Public Shadows Function RemoveSyntaxTrees(ByVal trees As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTree)) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.SyntaxTree) = Nothing
			If (trees Is Nothing) Then
				Throw New ArgumentNullException("trees")
			End If
			If (trees.Any()) Then
				Dim flag As Boolean = False
				Dim syntaxTrees As HashSet(Of Microsoft.CodeAnalysis.SyntaxTree) = New HashSet(Of Microsoft.CodeAnalysis.SyntaxTree)()
				Dim syntaxTrees1 As ImmutableDictionary(Of Microsoft.CodeAnalysis.SyntaxTree, DeclarationTableEntry) = Me._rootNamespaces
				Using declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Me._declarationTable
					enumerator = trees.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.SyntaxTree = enumerator.Current
						If (current.IsEmbeddedOrMyTemplateTree()) Then
							Throw New ArgumentException(VBResources.CannotRemoveCompilerSpecialTree)
						End If
						Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.RemoveSyntaxTreeFromDeclarationMapAndTable(current, syntaxTrees1, declarationTable, flag)
						syntaxTrees.Add(current)
					End While
				End Using
				Dim syntaxTrees2 As ImmutableDictionary(Of Microsoft.CodeAnalysis.SyntaxTree, Integer) = ImmutableDictionary.Create(Of Microsoft.CodeAnalysis.SyntaxTree, Integer)()
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxTree) = ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxTree).GetInstance()
				Dim num As Integer = 0
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxTree).Enumerator = Me._syntaxTrees.GetEnumerator()
				While enumerator1.MoveNext()
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = enumerator1.Current
					If (syntaxTrees.Contains(syntaxTree)) Then
						Continue While
					End If
					instance.Add(syntaxTree)
					syntaxTrees2 = syntaxTrees2.Add(syntaxTree, num)
					num = num + 1
				End While
				visualBasicCompilation = Me.UpdateSyntaxTrees(instance.ToImmutableAndFree(), syntaxTrees2, syntaxTrees1, declarationTable, flag)
			Else
				visualBasicCompilation = Me
			End If
			Return visualBasicCompilation
		End Function

		Public Shadows Function ReplaceReference(ByVal oldReference As MetadataReference, ByVal newReference As MetadataReference) As VisualBasicCompilation
			Return DirectCast(MyBase.ReplaceReference(oldReference, newReference), VisualBasicCompilation)
		End Function

		Public Shadows Function ReplaceSyntaxTree(ByVal oldTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal newTree As Microsoft.CodeAnalysis.SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			If (oldTree Is Nothing) Then
				Throw New ArgumentNullException("oldTree")
			End If
			If (newTree Is Nothing) Then
				visualBasicCompilation = Me.RemoveSyntaxTrees(New Microsoft.CodeAnalysis.SyntaxTree() { oldTree })
			ElseIf (newTree <> oldTree) Then
				If (Not newTree.HasCompilationUnitRoot) Then
					Throw New ArgumentException(VBResources.TreeMustHaveARootNodeWithCompilationUnit, "newTree")
				End If
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = oldTree
				Dim syntaxTree1 As Microsoft.CodeAnalysis.SyntaxTree = newTree
				If (syntaxTree.IsEmbeddedOrMyTemplateTree()) Then
					Throw New ArgumentException(VBResources.CannotRemoveCompilerSpecialTree)
				End If
				If (syntaxTree1.IsEmbeddedOrMyTemplateTree()) Then
					Throw New ArgumentException(VBResources.CannotAddCompilerSpecialTree)
				End If
				Dim syntaxTrees As ImmutableDictionary(Of Microsoft.CodeAnalysis.SyntaxTree, DeclarationTableEntry) = Me._rootNamespaces
				If (syntaxTrees.ContainsKey(syntaxTree1)) Then
					Throw New ArgumentException(VBResources.SyntaxTreeAlreadyPresent, "newTree")
				End If
				Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Me._declarationTable
				Dim flag As Boolean = False
				Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.RemoveSyntaxTreeFromDeclarationMapAndTable(syntaxTree, syntaxTrees, declarationTable, flag)
				Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.AddSyntaxTreeToDeclarationMapAndTable(syntaxTree1, Me._options, MyBase.IsSubmission, syntaxTrees, declarationTable, flag)
				Dim syntaxTrees1 As ImmutableDictionary(Of Microsoft.CodeAnalysis.SyntaxTree, Integer) = Me._syntaxTreeOrdinalMap
				Dim item As Integer = syntaxTrees1(oldTree)
				Dim array As Microsoft.CodeAnalysis.SyntaxTree() = Me._syntaxTrees.ToArray()
				array(item) = syntaxTree1
				syntaxTrees1 = syntaxTrees1.Remove(oldTree)
				syntaxTrees1 = syntaxTrees1.Add(newTree, item)
				visualBasicCompilation = Me.UpdateSyntaxTrees(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.SyntaxTree)(array), syntaxTrees1, syntaxTrees, declarationTable, flag)
			Else
				visualBasicCompilation = Me
			End If
			Return visualBasicCompilation
		End Function

		Friend Overrides Sub ReportUnusedImports(ByVal diagnostics As DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
			Me.ReportUnusedImports(Nothing, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(diagnostics), cancellationToken)
		End Sub

		Private Sub ReportUnusedImports(ByVal filterTree As SyntaxTree, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim enumerator As IEnumerator(Of VisualBasicCompilation.ImportInfo) = Nothing
			If (Me._lazyImportInfos IsNot Nothing AndAlso (filterTree Is Nothing OrElse Compilation.ReportUnusedImportsInTree(filterTree))) Then
				Using instance As ArrayBuilder(Of Microsoft.CodeAnalysis.Text.TextSpan) = Nothing
					enumerator = Me._lazyImportInfos.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As VisualBasicCompilation.ImportInfo = enumerator.Current
						cancellationToken.ThrowIfCancellationRequested()
						Dim tree As SyntaxTree = current.Tree
						If (filterTree IsNot Nothing AndAlso filterTree <> tree OrElse Not Compilation.ReportUnusedImportsInTree(tree)) Then
							Continue While
						End If
						Dim clauseSpans As ImmutableArray(Of Microsoft.CodeAnalysis.Text.TextSpan) = current.ClauseSpans
						Dim length As Integer = clauseSpans.Length
						If (length <> 1) Then
							If (instance IsNot Nothing) Then
								instance.Clear()
							End If
							Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Text.TextSpan).Enumerator = current.ClauseSpans.GetEnumerator()
							While enumerator1.MoveNext()
								Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = enumerator1.Current
								If (MyBase.IsImportDirectiveUsed(tree, textSpan.Start)) Then
									Me.AddImportsDependencies(diagnostics, tree, textSpan)
								Else
									If (instance Is Nothing) Then
										instance = ArrayBuilder(Of Microsoft.CodeAnalysis.Text.TextSpan).GetInstance()
									End If
									instance.Add(textSpan)
								End If
							End While
							If (instance Is Nothing OrElse instance.Count <= 0) Then
								Continue While
							End If
							If (instance.Count <> length) Then
								Dim enumerator2 As ArrayBuilder(Of Microsoft.CodeAnalysis.Text.TextSpan).Enumerator = instance.GetEnumerator()
								While enumerator2.MoveNext()
									Dim current1 As Microsoft.CodeAnalysis.Text.TextSpan = enumerator2.Current
									diagnostics.Add(ERRID.HDN_UnusedImportClause, tree.GetLocation(current1))
								End While
							Else
								diagnostics.Add(ERRID.HDN_UnusedImportStatement, tree.GetLocation(current.StatementSpan))
							End If
						ElseIf (MyBase.IsImportDirectiveUsed(tree, clauseSpans(0).Start)) Then
							Me.AddImportsDependencies(diagnostics, tree, clauseSpans(0))
						Else
							diagnostics.Add(ERRID.HDN_UnusedImportStatement, tree.GetLocation(current.StatementSpan))
						End If
					End While
				End Using
				If (instance IsNot Nothing) Then
					instance.Free()
				End If
			End If
			Me.CompleteTrees(filterTree)
		End Sub

		Private Shared Function ReturnNothingOrThrowIfAttributeNonOptional(ByVal constructor As WellKnownMember, Optional ByVal isOptionalUse As Boolean = False) As SynthesizedAttributeData
			If (Not isOptionalUse AndAlso Not WellKnownMembers.IsSynthesizedAttributeOptional(constructor)) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Return Nothing
		End Function

		Friend Overrides Sub SerializePdbEmbeddedCompilationOptions(ByVal builder As System.Reflection.Metadata.BlobBuilder)
			Dim func As Func(Of KeyValuePair(Of String, Object), String)
			Dim name As Func(Of GlobalImport, String)
			Me.WriteValue(builder, "language-version", Me.LanguageVersion.ToDisplayString())
			Dim checkOverflow As Boolean = Me.Options.CheckOverflow
			Me.WriteValue(builder, "checked", checkOverflow.ToString())
			Dim optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict = Me.Options.OptionStrict
			Me.WriteValue(builder, "option-strict", optionStrict.ToString())
			checkOverflow = Me.Options.OptionInfer
			Me.WriteValue(builder, "option-infer", checkOverflow.ToString())
			checkOverflow = Me.Options.OptionCompareText
			Me.WriteValue(builder, "option-compare-text", checkOverflow.ToString())
			checkOverflow = Me.Options.OptionExplicit
			Me.WriteValue(builder, "option-explicit", checkOverflow.ToString())
			checkOverflow = Me.Options.EmbedVbCoreRuntime
			Me.WriteValue(builder, "embed-runtime", checkOverflow.ToString())
			If (Me.Options.GlobalImports.Length > 0) Then
				Dim blobBuilder As System.Reflection.Metadata.BlobBuilder = builder
				Dim globalImports As ImmutableArray(Of GlobalImport) = Me.Options.GlobalImports
				If (VisualBasicCompilation._Closure$__.$I61-0 Is Nothing) Then
					name = Function(x As GlobalImport) x.Name
					VisualBasicCompilation._Closure$__.$I61-0 = name
				Else
					name = VisualBasicCompilation._Closure$__.$I61-0
				End If
				Me.WriteValue(blobBuilder, "global-namespaces", [String].Join(";", globalImports.[Select](Of String)(name)))
			End If
			If (Not [String].IsNullOrEmpty(Me.Options.RootNamespace)) Then
				Me.WriteValue(builder, "root-namespace", Me.Options.RootNamespace)
			End If
			If (Me.Options.ParseOptions IsNot Nothing) Then
				Dim preprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object)) = Me.Options.ParseOptions.PreprocessorSymbols
				If (VisualBasicCompilation._Closure$__.$I61-1 Is Nothing) Then
					func = Function(p As KeyValuePair(Of String, Object)) If(Not TypeOf p.Value Is [String], If(p.Value IsNot Nothing, [String].Concat(p.Key, "=", p.Value.ToString()), p.Key), [String].Concat(p.Key, "=""", p.Value.ToString(), """"))
					VisualBasicCompilation._Closure$__.$I61-1 = func
				Else
					func = VisualBasicCompilation._Closure$__.$I61-1
				End If
				Dim strs As IEnumerable(Of String) = preprocessorSymbols.[Select](Of String)(func)
				Me.WriteValue(builder, "define", [String].Join(",", strs))
			End If
		End Sub

		Friend Function ShouldAddEvent(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (MyBase.EventQueue Is Nothing) Then
				Return False
			End If
			Return symbol.IsInSource()
		End Function

		Friend Sub SymbolDeclaredEvent(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (Me.ShouldAddEvent(symbol)) Then
				MyBase.EventQueue.TryEnqueue(New SymbolDeclaredCompilationEvent(Me, symbol, Nothing))
			End If
		End Sub

		Friend Function SynthesizeDebuggerBrowsableNeverAttribute() As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			If (Me.Options.OptimizationLevel = OptimizationLevel.Debug) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.GetWellKnownType(WellKnownType.System_Diagnostics_DebuggerBrowsableState), TypedConstantKind.[Enum], DebuggerBrowsableState.Never))
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				synthesizedAttributeDatum = Me.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor, typedConstants, keyValuePairs, False)
			Else
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Function SynthesizeDebuggerHiddenAttribute() As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			If (Me.Options.OptimizationLevel = OptimizationLevel.Debug) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				synthesizedAttributeDatum = Me.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor, typedConstants, keyValuePairs, False)
			Else
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Function SynthesizeDebuggerNonUserCodeAttribute() As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			If (Me.Options.OptimizationLevel = OptimizationLevel.Debug) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				synthesizedAttributeDatum = Me.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerNonUserCodeAttribute__ctor, typedConstants, keyValuePairs, False)
			Else
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Function SynthesizeDecimalConstantAttribute(ByVal value As [Decimal]) As SynthesizedAttributeData
			Dim flag As Boolean
			Dim num As Byte
			Dim num1 As UInteger
			Dim num2 As UInteger
			Dim num3 As UInteger
			value.GetBits(flag, num, num1, num2, num3)
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Byte)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_UInt32)
			Dim typedConstant() As Microsoft.CodeAnalysis.TypedConstant = { New Microsoft.CodeAnalysis.TypedConstant(specialType, TypedConstantKind.Primitive, num), Nothing, Nothing, Nothing, Nothing }
			typedConstant(1) = New Microsoft.CodeAnalysis.TypedConstant(specialType, TypedConstantKind.Primitive, CByte(If(flag, 128, 0)))
			typedConstant(2) = New Microsoft.CodeAnalysis.TypedConstant(namedTypeSymbol, TypedConstantKind.Primitive, num3)
			typedConstant(3) = New Microsoft.CodeAnalysis.TypedConstant(namedTypeSymbol, TypedConstantKind.Primitive, num2)
			typedConstant(4) = New Microsoft.CodeAnalysis.TypedConstant(namedTypeSymbol, TypedConstantKind.Primitive, num1)
			Return Me.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctor, ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant), New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))(), False)
		End Function

		Friend Function SynthesizeEditorBrowsableNeverAttribute() As SynthesizedAttributeData
			Return Me.TrySynthesizeAttribute(WellKnownMember.System_ComponentModel_EditorBrowsableAttribute__ctor, ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.GetWellKnownType(WellKnownType.System_ComponentModel_EditorBrowsableState), TypedConstantKind.[Enum], EditorBrowsableState.Never)), New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))(), False)
		End Function

		Friend Function SynthesizeExtensionAttribute() As SynthesizedAttributeData
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim extensionAttributeConstructor As MethodSymbol = Me.GetExtensionAttributeConstructor(useSiteInfo)
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Return SynthesizedAttributeData.Create(extensionAttributeConstructor, WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor, typedConstants, New ImmutableArray(Of KeyValuePair(Of String, TypedConstant))())
		End Function

		Friend Function SynthesizeOptionalDebuggerStepThroughAttribute() As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			If (Me.Options.OptimizationLevel = OptimizationLevel.Debug) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				synthesizedAttributeDatum = Me.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerStepThroughAttribute__ctor, typedConstants, keyValuePairs, False)
			Else
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Function SynthesizeStateMachineAttribute(ByVal method As MethodSymbol, ByVal compilationState As ModuleCompilationState) As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			If (Not compilationState.TryGetStateMachineType(method, namedTypeSymbol)) Then
				synthesizedAttributeDatum = Nothing
			Else
				Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = If(method.IsAsync, Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor, Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor)
				Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(Me.GetWellKnownType(WellKnownType.System_Type), TypedConstantKind.Type, If(namedTypeSymbol.IsGenericType, namedTypeSymbol.ConstructUnboundGenericType(), namedTypeSymbol))
				Dim typedConstants As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant)
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.WellKnownMember, Microsoft.CodeAnalysis.TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
				synthesizedAttributeDatum = Me.TrySynthesizeAttribute(wellKnownMember, typedConstants, keyValuePairs, False)
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Function SynthesizeTupleNamesAttribute(ByVal type As TypeSymbol) As SynthesizedAttributeData
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String)
			Dim typedConstants As ImmutableArray(Of TypedConstant) = VisualBasicCompilation.TupleNamesEncoder.Encode(type, specialType)
			Dim typedConstants1 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(ArrayTypeSymbol.CreateSZArray(specialType, ImmutableArray(Of CustomModifier).Empty, specialType.ContainingAssembly), typedConstants))
			Return Me.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, typedConstants1, New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))(), False)
		End Function

		Public Overrides Function ToMetadataReference(Optional ByVal aliases As ImmutableArray(Of String) = Nothing, Optional ByVal embedInteropTypes As Boolean = False) As CompilationReference
			Return New VisualBasicCompilationReference(Me, aliases, embedInteropTypes)
		End Function

		Friend Function TrySynthesizeAttribute(ByVal constructor As WellKnownMember, Optional ByVal arguments As ImmutableArray(Of TypedConstant) = Nothing, Optional ByVal namedArguments As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = Nothing, Optional ByVal isOptionalUse As Boolean = False) As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			Dim immutableAndFree As ImmutableArray(Of KeyValuePair(Of String, TypedConstant))
			Dim wellKnownTypeMember As MethodSymbol = TryCast(Me.GetWellKnownTypeMember(constructor), MethodSymbol)
			If (wellKnownTypeMember Is Nothing OrElse Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForWellKnownTypeMember(wellKnownTypeMember, constructor, False).DiagnosticInfo IsNot Nothing) Then
				synthesizedAttributeDatum = VisualBasicCompilation.ReturnNothingOrThrowIfAttributeNonOptional(constructor, isOptionalUse)
			Else
				If (arguments.IsDefault) Then
					arguments = ImmutableArray(Of TypedConstant).Empty
				End If
				If (Not namedArguments.IsDefault) Then
					Dim keyValuePairs As ArrayBuilder(Of KeyValuePair(Of String, TypedConstant)) = New ArrayBuilder(Of KeyValuePair(Of String, TypedConstant))(namedArguments.Length)
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)).Enumerator = namedArguments.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of WellKnownMember, TypedConstant) = enumerator.Current
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.GetWellKnownTypeMember(current.Key)
						If (symbol Is Nothing OrElse TypeOf symbol Is ErrorTypeSymbol OrElse Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForWellKnownTypeMember(symbol, current.Key, False).DiagnosticInfo IsNot Nothing) Then
							synthesizedAttributeDatum = VisualBasicCompilation.ReturnNothingOrThrowIfAttributeNonOptional(constructor, False)
							Return synthesizedAttributeDatum
						Else
							keyValuePairs.Add(New KeyValuePair(Of String, TypedConstant)(symbol.Name, current.Value))
						End If
					End While
					immutableAndFree = keyValuePairs.ToImmutableAndFree()
				Else
					immutableAndFree = ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty
				End If
				synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownTypeMember, arguments, immutableAndFree)
			End If
			Return synthesizedAttributeDatum
		End Function

		Private Function UpdateSyntaxTrees(ByVal syntaxTrees As ImmutableArray(Of SyntaxTree), ByVal syntaxTreeOrdinalMap As ImmutableDictionary(Of SyntaxTree, Integer), ByVal rootNamespaces As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry), ByVal declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable, ByVal referenceDirectivesChanged As Boolean) As VisualBasicCompilation
			Return New VisualBasicCompilation(MyBase.AssemblyName, Me._options, MyBase.ExternalReferences, syntaxTrees, syntaxTreeOrdinalMap, rootNamespaces, Me._embeddedTrees, declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, Not referenceDirectivesChanged, MyBase.SemanticModelProvider, Nothing)
		End Function

		Friend Overrides Sub ValidateDebugEntryPoint(ByVal debugEntryPoint As IMethodSymbol, ByVal diagnostics As DiagnosticBag)
			Dim declaringCompilation As VisualBasicCompilation
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(debugEntryPoint, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol IsNot Nothing) Then
				declaringCompilation = methodSymbol.DeclaringCompilation
			Else
				declaringCompilation = Nothing
			End If
			If (declaringCompilation <> Me OrElse Not methodSymbol.IsDefinition) Then
				diagnostics.Add(ERRID.ERR_DebugEntryPointNotSourceMethodDefinition, Location.None)
			End If
		End Sub

		Public Shadows Function WithAssemblyName(ByVal assemblyName As String) As VisualBasicCompilation
			Return New VisualBasicCompilation(assemblyName, Me.Options, MyBase.ExternalReferences, Me._syntaxTrees, Me._syntaxTreeOrdinalMap, Me._rootNamespaces, Me._embeddedTrees, Me._declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, [String].Equals(assemblyName, MyBase.AssemblyName, StringComparison.Ordinal), MyBase.SemanticModelProvider, Nothing)
		End Function

		Friend Overrides Function WithEventQueue(ByVal eventQueue As AsyncQueue(Of CompilationEvent)) As Compilation
			Return New VisualBasicCompilation(MyBase.AssemblyName, Me.Options, MyBase.ExternalReferences, Me._syntaxTrees, Me._syntaxTreeOrdinalMap, Me._rootNamespaces, Me._embeddedTrees, Me._declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, True, MyBase.SemanticModelProvider, eventQueue)
		End Function

		Public Function WithOptions(ByVal newOptions As VisualBasicCompilationOptions) As VisualBasicCompilation
			Dim variable As VisualBasicCompilation._Closure$__57-0 = Nothing
			variable = New VisualBasicCompilation._Closure$__57-0(variable)
			If (newOptions Is Nothing) Then
				Throw New ArgumentNullException("newOptions")
			End If
			variable.$VB$Local_c = Nothing
			Dim embeddedTreeAndDeclarations As ImmutableArray(Of VisualBasicCompilation.EmbeddedTreeAndDeclaration) = Me._embeddedTrees
			Dim empty As DeclarationTable = Me._declarationTable
			Dim syntaxTrees As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry) = Me._rootNamespaces
			If (Not [String].Equals(Me.Options.RootNamespace, newOptions.RootNamespace, StringComparison.Ordinal)) Then
				syntaxTrees = ImmutableDictionary.Create(Of SyntaxTree, DeclarationTableEntry)()
				empty = DeclarationTable.Empty
				embeddedTreeAndDeclarations = VisualBasicCompilation.CreateEmbeddedTrees(New Lazy(Of VisualBasicCompilation)(Function() Me.$VB$Local_c))
				empty = VisualBasicCompilation.AddEmbeddedTrees(empty, embeddedTreeAndDeclarations)
				Dim flag As Boolean = False
				Dim enumerator As ImmutableArray(Of SyntaxTree).Enumerator = Me._syntaxTrees.GetEnumerator()
				While enumerator.MoveNext()
					VisualBasicCompilation.AddSyntaxTreeToDeclarationMapAndTable(enumerator.Current, newOptions, MyBase.IsSubmission, syntaxTrees, empty, flag)
				End While
			ElseIf (Me.Options.EmbedVbCoreRuntime <> newOptions.EmbedVbCoreRuntime OrElse Me.Options.ParseOptions <> newOptions.ParseOptions) Then
				empty = VisualBasicCompilation.RemoveEmbeddedTrees(empty, Me._embeddedTrees)
				embeddedTreeAndDeclarations = VisualBasicCompilation.CreateEmbeddedTrees(New Lazy(Of VisualBasicCompilation)(Function() Me.$VB$Local_c))
				empty = VisualBasicCompilation.AddEmbeddedTrees(empty, embeddedTreeAndDeclarations)
			End If
			variable.$VB$Local_c = New VisualBasicCompilation(MyBase.AssemblyName, newOptions, MyBase.ExternalReferences, Me._syntaxTrees, Me._syntaxTreeOrdinalMap, syntaxTrees, embeddedTreeAndDeclarations, empty, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, Me._options.CanReuseCompilationReferenceManager(newOptions), MyBase.SemanticModelProvider, Nothing)
			Return variable.$VB$Local_c
		End Function

		Public Shadows Function WithReferences(ByVal ParamArray newReferences As MetadataReference()) As VisualBasicCompilation
			Return Me.WithReferences(DirectCast(newReferences, IEnumerable(Of MetadataReference)))
		End Function

		Public Shadows Function WithReferences(ByVal newReferences As IEnumerable(Of MetadataReference)) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.RemoveEmbeddedTrees(Me._declarationTable, Me._embeddedTrees)
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Nothing
			Dim embeddedTreeAndDeclarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration) = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.CreateEmbeddedTrees(New Lazy(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)(Function() visualBasicCompilation))
			declarationTable = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.AddEmbeddedTrees(declarationTable, embeddedTreeAndDeclarations)
			visualBasicCompilation = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation(MyBase.AssemblyName, Me.Options, Compilation.ValidateReferences(Of VisualBasicCompilationReference)(newReferences), Me._syntaxTrees, Me._syntaxTreeOrdinalMap, Me._rootNamespaces, embeddedTreeAndDeclarations, declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Nothing, False, MyBase.SemanticModelProvider, Nothing)
			Return visualBasicCompilation
		End Function

		Friend Function WithScriptCompilationInfo(ByVal info As VisualBasicScriptCompilationInfo) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim previousScriptCompilation As Object
			Dim obj As Object
			Dim previousScriptCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
			Dim returnTypeOpt As Type
			Dim globalsType As Type
			If (info <> Me.ScriptCompilationInfo) Then
				Dim scriptCompilationInfo As VisualBasicScriptCompilationInfo = Me.ScriptCompilationInfo
				If (scriptCompilationInfo IsNot Nothing) Then
					previousScriptCompilation = scriptCompilationInfo.PreviousScriptCompilation
				Else
					previousScriptCompilation = Nothing
				End If
				If (info IsNot Nothing) Then
					obj = info.PreviousScriptCompilation
				Else
					obj = Nothing
				End If
				Dim flag As Boolean = CObj(previousScriptCompilation) = CObj(obj)
				Dim assemblyName As String = MyBase.AssemblyName
				Dim options As VisualBasicCompilationOptions = Me.Options
				Dim externalReferences As ImmutableArray(Of MetadataReference) = MyBase.ExternalReferences
				Dim syntaxTrees As ImmutableArray(Of SyntaxTree) = Me._syntaxTrees
				Dim syntaxTrees1 As ImmutableDictionary(Of SyntaxTree, Integer) = Me._syntaxTreeOrdinalMap
				Dim syntaxTrees2 As ImmutableDictionary(Of SyntaxTree, DeclarationTableEntry) = Me._rootNamespaces
				Dim embeddedTreeAndDeclarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EmbeddedTreeAndDeclaration) = Me._embeddedTrees
				Dim declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable = Me._declarationTable
				If (info IsNot Nothing) Then
					previousScriptCompilation1 = info.PreviousScriptCompilation
				Else
					previousScriptCompilation1 = Nothing
				End If
				If (info IsNot Nothing) Then
					returnTypeOpt = info.ReturnTypeOpt
				Else
					returnTypeOpt = Nothing
				End If
				If (info IsNot Nothing) Then
					globalsType = info.GlobalsType
				Else
					globalsType = Nothing
				End If
				visualBasicCompilation = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation(assemblyName, options, externalReferences, syntaxTrees, syntaxTrees1, syntaxTrees2, embeddedTreeAndDeclarations, declarationTable, previousScriptCompilation1, returnTypeOpt, globalsType, info IsNot Nothing, Me._referenceManager, flag, MyBase.SemanticModelProvider, Nothing)
			Else
				visualBasicCompilation = Me
			End If
			Return visualBasicCompilation
		End Function

		Friend Overrides Function WithSemanticModelProvider(ByVal semanticModelProvider As Microsoft.CodeAnalysis.SemanticModelProvider) As Microsoft.CodeAnalysis.Compilation
			Dim compilation As Microsoft.CodeAnalysis.Compilation
			compilation = If(MyBase.SemanticModelProvider <> semanticModelProvider, New VisualBasicCompilation(MyBase.AssemblyName, Me.Options, MyBase.ExternalReferences, Me._syntaxTrees, Me._syntaxTreeOrdinalMap, Me._rootNamespaces, Me._embeddedTrees, Me._declarationTable, Me.PreviousSubmission, MyBase.SubmissionReturnType, MyBase.HostObjectType, MyBase.IsSubmission, Me._referenceManager, True, semanticModelProvider, Nothing), Me)
			Return compilation
		End Function

		Private Sub WriteValue(ByVal builder As BlobBuilder, ByVal key As String, ByVal value As String)
			builder.WriteUTF8(key, True)
			builder.WriteByte(0)
			builder.WriteUTF8(value, True)
			builder.WriteByte(0)
		End Sub

		Private MustInherit Class AbstractSymbolSearcher
			Private ReadOnly _cache As PooledDictionary(Of Declaration, NamespaceOrTypeSymbol)

			Private ReadOnly _compilation As VisualBasicCompilation

			Private ReadOnly _includeNamespace As Boolean

			Private ReadOnly _includeType As Boolean

			Private ReadOnly _includeMember As Boolean

			Private ReadOnly _cancellationToken As CancellationToken

			Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal filter As SymbolFilter, ByVal cancellationToken As System.Threading.CancellationToken)
				MyBase.New()
				Me._cache = PooledDictionary(Of Declaration, NamespaceOrTypeSymbol).GetInstance()
				Me._compilation = compilation
				Me._includeNamespace = (filter And SymbolFilter.[Namespace]) = SymbolFilter.[Namespace]
				Me._includeType = (filter And SymbolFilter.Type) = SymbolFilter.Type
				Me._includeMember = (filter And SymbolFilter.Member) = SymbolFilter.Member
				Me._cancellationToken = cancellationToken
			End Sub

			Private Sub AddCache(ByVal symbols As IEnumerable(Of NamespaceOrTypeSymbol))
				Dim enumerator As IEnumerator(Of NamespaceOrTypeSymbol) = Nothing
				Try
					enumerator = symbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamespaceOrTypeSymbol = enumerator.Current
						Dim mergedNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceSymbol)
						If (mergedNamespaceSymbol Is Nothing) Then
							Dim sourceNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol)
							If (sourceNamespaceSymbol Is Nothing) Then
								Dim sourceMemberContainerTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol)
								If (sourceMemberContainerTypeSymbol Is Nothing) Then
									Continue While
								End If
								Me._cache(sourceMemberContainerTypeSymbol.TypeDeclaration) = sourceMemberContainerTypeSymbol
							Else
								Me._cache(sourceNamespaceSymbol.MergedDeclaration) = sourceNamespaceSymbol
							End If
						Else
							Dim pooledDictionary As PooledDictionary(Of Declaration, NamespaceOrTypeSymbol) = Me._cache
							Dim constituentNamespaces As ImmutableArray(Of NamespaceSymbol) = mergedNamespaceSymbol.ConstituentNamespaces
							pooledDictionary(constituentNamespaces.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol)().First().MergedDeclaration) = current
						End If
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End Sub

			Private Sub AppendMemberSymbolsWithName(ByVal spine As ArrayBuilder(Of MergedNamespaceOrTypeDeclaration), ByVal mergedType As MergedTypeDeclaration, ByVal [set] As HashSet(Of ISymbol))
				Dim enumerator As IEnumerator(Of String) = Nothing
				Me._cancellationToken.ThrowIfCancellationRequested()
				spine.Add(mergedType)
				Using spineSymbol As NamespaceOrTypeSymbol = Nothing
					enumerator = mergedType.MemberNames.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As String = enumerator.Current
						If (Not Me.Matches(current)) Then
							Continue While
						End If
						spineSymbol = If(spineSymbol, Me.GetSpineSymbol(spine))
						If (spineSymbol Is Nothing) Then
							Continue While
						End If
						[set].UnionWith(DirectCast(spineSymbol.GetMembers(current), IEnumerable(Of ISymbol)))
					End While
				End Using
				spine.RemoveAt(spine.Count - 1)
			End Sub

			Private Sub AppendSymbolsWithName(ByVal spine As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration), ByVal current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration, ByVal [set] As HashSet(Of ISymbol))
				If (current.Kind <> DeclarationKind.[Namespace]) Then
					If (Me._includeType AndAlso Me.Matches(current.Name)) Then
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me.GetSymbol(Me.GetSpineSymbol(spine), current)
						If (symbol IsNot Nothing) Then
							[set].Add(symbol)
						End If
					End If
					If (Me._includeMember) Then
						Dim mergedTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedTypeDeclaration = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedTypeDeclaration)
						If (Me.ShouldCheckTypeForMembers(mergedTypeDeclaration)) Then
							Me.AppendMemberSymbolsWithName(spine, mergedTypeDeclaration, [set])
						End If
					End If
				ElseIf (Me._includeNamespace AndAlso Me.Matches(current.Name)) Then
					Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me.GetSymbol(Me.GetSpineSymbol(spine), current)
					If (namespaceOrTypeSymbol IsNot Nothing) Then
						[set].Add(namespaceOrTypeSymbol)
					End If
				End If
				spine.Add(current)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration).Enumerator = current.Children.GetEnumerator()
				While enumerator.MoveNext()
					Dim declaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.Declaration = enumerator.Current
					Dim mergedNamespaceOrTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration = TryCast(declaration, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceOrTypeDeclaration)
					If (mergedNamespaceOrTypeDeclaration Is Nothing OrElse Not Me._includeMember AndAlso Not Me._includeType AndAlso declaration.Kind <> DeclarationKind.[Namespace]) Then
						Continue While
					End If
					Me.AppendSymbolsWithName(spine, mergedNamespaceOrTypeDeclaration, [set])
				End While
				spine.RemoveAt(spine.Count - 1)
			End Sub

			Private Function GetCachedSymbol(ByVal declaration As MergedNamespaceOrTypeDeclaration) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				Dim namespaceOrTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Nothing
				If (Not Me._cache.TryGetValue(declaration, namespaceOrTypeSymbol1)) Then
					namespaceOrTypeSymbol = Nothing
				Else
					namespaceOrTypeSymbol = namespaceOrTypeSymbol1
				End If
				Return namespaceOrTypeSymbol
			End Function

			Private Function GetSpineSymbol(ByVal spine As ArrayBuilder(Of MergedNamespaceOrTypeDeclaration)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				If (spine.Count <> 0) Then
					Dim cachedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me.GetCachedSymbol(spine(spine.Count - 1))
					If (cachedSymbol Is Nothing) Then
						Dim globalNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me._compilation.GlobalNamespace
						Dim count As Integer = spine.Count - 1
						Dim num As Integer = 1
						Do
							globalNamespace = Me.GetSymbol(globalNamespace, spine(num))
							num = num + 1
						Loop While num <= count
						namespaceOrTypeSymbol = globalNamespace
					Else
						namespaceOrTypeSymbol = cachedSymbol
					End If
				Else
					namespaceOrTypeSymbol = Nothing
				End If
				Return namespaceOrTypeSymbol
			End Function

			Private Function GetSymbol(ByVal container As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol, ByVal declaration As MergedNamespaceOrTypeDeclaration) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				Dim cachedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
				If (container IsNot Nothing) Then
					Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Me.GetCachedSymbol(declaration)
					If (namespaceOrTypeSymbol Is Nothing) Then
						If (declaration.Kind <> DeclarationKind.[Namespace]) Then
							Me.AddCache(DirectCast(container.GetTypeMembers(declaration.Name), IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)))
						Else
							Dim members As ImmutableArray(Of Symbol) = container.GetMembers(declaration.Name)
							Me.AddCache(members.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)())
						End If
						cachedSymbol = Me.GetCachedSymbol(declaration)
					Else
						cachedSymbol = namespaceOrTypeSymbol
					End If
				Else
					cachedSymbol = Me._compilation.GlobalNamespace
				End If
				Return cachedSymbol
			End Function

			Public Function GetSymbolsWithName() As IEnumerable(Of ISymbol)
				Dim symbols As HashSet(Of ISymbol) = New HashSet(Of ISymbol)()
				Dim instance As ArrayBuilder(Of MergedNamespaceOrTypeDeclaration) = ArrayBuilder(Of MergedNamespaceOrTypeDeclaration).GetInstance()
				Me.AppendSymbolsWithName(instance, Me._compilation.MergedRootDeclaration, symbols)
				instance.Free()
				Me._cache.Free()
				Return symbols
			End Function

			Protected MustOverride Function Matches(ByVal name As String) As Boolean

			Protected MustOverride Function ShouldCheckTypeForMembers(ByVal typeDeclaration As MergedTypeDeclaration) As Boolean
		End Class

		Friend Class DocumentationCommentCompiler
			Inherits VisualBasicSymbolVisitor
			Private ReadOnly _assemblyName As String

			Private ReadOnly _compilation As VisualBasicCompilation

			Private ReadOnly _processIncludes As Boolean

			Private ReadOnly _isForSingleSymbol As Boolean

			Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Private ReadOnly _cancellationToken As CancellationToken

			Private ReadOnly _filterSyntaxTree As SyntaxTree

			Private ReadOnly _filterSpanWithinTree As Nullable(Of TextSpan)

			Private _writer As VisualBasicCompilation.DocumentationCommentCompiler.DocWriter

			Private _includedFileCache As DocumentationCommentIncludeCache

			Private ReadOnly Property IsInSemanticModelMode As Boolean
				Get
					Return Me._isForSingleSymbol
				End Get
			End Property

			Private ReadOnly Property [Module] As SourceModuleSymbol
				Get
					Return DirectCast(Me._compilation.SourceModule, SourceModuleSymbol)
				End Get
			End Property

			Private Sub New(ByVal assemblyName As String, ByVal compilation As VisualBasicCompilation, ByVal writer As TextWriter, ByVal processIncludes As Boolean, ByVal isForSingleSymbol As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal filterTree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal preferredCulture As CultureInfo, ByVal cancellationToken As System.Threading.CancellationToken)
				MyBase.New()
				Me._assemblyName = assemblyName
				Me._compilation = compilation
				Me._writer = New VisualBasicCompilation.DocumentationCommentCompiler.DocWriter(writer)
				Me._processIncludes = processIncludes
				Me._isForSingleSymbol = isForSingleSymbol
				Me._diagnostics = diagnostics
				Me._filterSyntaxTree = filterTree
				Me._filterSpanWithinTree = filterSpanWithinTree
				Me._cancellationToken = cancellationToken
			End Sub

			Private Shared Function CreateDocumentationCommentBinderForSymbol(ByVal [module] As SourceModuleSymbol, ByVal sym As Symbol, ByVal tree As SyntaxTree, ByVal binderType As DocumentationCommentBinder.BinderType) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder2 As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim kind As SymbolKind = sym.Kind
				Select Case kind
					Case SymbolKind.[Event]
					Case SymbolKind.Field
					Case SymbolKind.Method
					Label1:
						binder1 = BinderBuilder.CreateBinderForType([module], tree, sym.ContainingType)
						Exit Select
					Case SymbolKind.Label
					Case SymbolKind.Local
					Case SymbolKind.NetModule
						binder = Nothing
						Return binder
					Case SymbolKind.NamedType
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(sym, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						If (namedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
							Dim containingNamespaceOrType As NamespaceOrTypeSymbol = namedTypeSymbol.ContainingNamespaceOrType
							If (containingNamespaceOrType.IsNamespace) Then
								binder2 = BinderBuilder.CreateBinderForNamespace([module], tree, DirectCast(containingNamespaceOrType, NamespaceSymbol))
							Else
								binder2 = BinderBuilder.CreateBinderForType([module], tree, DirectCast(containingNamespaceOrType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
							End If
							binder1 = binder2
							Exit Select
						Else
							binder1 = BinderBuilder.CreateBinderForType([module], tree, namedTypeSymbol)
							Exit Select
						End If
					Case Else
						If (kind = SymbolKind.[Property]) Then
							GoTo Label1
						End If
						binder = Nothing
						Return binder
				End Select
				binder = BinderBuilder.CreateBinderForDocumentationComment(binder1, sym, binderType)
				Return binder
			End Function

			Private Function FormatComment(ByVal substitutedText As String) As String
				Me._writer.BeginTemporaryString()
				Me.WriteFormattedComment(substitutedText)
				Return Me._writer.GetAndEndTemporaryString()
			End Function

			Private Shared Sub FreeWellKnownElementNodes(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)))
				Dim enumerator As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)).ValueCollection.Enumerator = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)).ValueCollection.Enumerator()
				Try
					enumerator = wellKnownElementNodes.Values.GetEnumerator()
					While enumerator.MoveNext()
						enumerator.Current.Free()
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End Sub

			Private Shared Function GetDescription(ByVal e As XmlException) As String
				Dim str As String
				Dim message As String = e.Message
				Try
					Dim str1 As String = [String].Format((New ResourceManager("System.Xml", GetType(XmlException).GetTypeInfo().Assembly)).GetString("Xml_MessageWithErrorPosition"), CObj(""), e.LineNumber, e.LinePosition)
					Dim num As Integer = message.IndexOf(str1, StringComparison.Ordinal)
					str = If(num < 0, message, message.Remove(num, str1.Length))
				Catch exception As System.Exception
					ProjectData.SetProjectError(exception)
					str = message
					ProjectData.ClearProjectError()
				End Try
				Return str
			End Function

			Private Function GetDocumentationCommentForSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal trivia As DocumentationCommentTriviaSyntax, ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))) As String
				Dim str As String
				Dim enumerator As IEnumerator(Of SyntaxNode) = Nothing
				If (Not Me.IsInSemanticModelMode) Then
					If (Not trivia.ContainsDiagnostics) Then
						Try
							enumerator = trivia.ChildNodes().GetEnumerator()
							While enumerator.MoveNext()
								If (Not enumerator.Current.ContainsDiagnostics) Then
									Continue While
								End If
								str = Nothing
								Return str
							End While
						Finally
							If (enumerator IsNot Nothing) Then
								enumerator.Dispose()
							End If
						End Try
					Else
						str = Nothing
						Return str
					End If
				End If
				Dim substitutedText As String = VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker.GetSubstitutedText(symbol, trivia, wellKnownElementNodes, Me._diagnostics)
				If (substitutedText IsNot Nothing) Then
					Dim str1 As String = Me.FormatComment(substitutedText)
					Dim str2 As String = Nothing
					If (Not Me._processIncludes) Then
						str2 = str1
					Else
						Dim xmlNodeSyntaxes As ArrayBuilder(Of XmlNodeSyntax) = Nothing
						wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Include, xmlNodeSyntaxes)
						str2 = VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ProcessIncludes(str1, symbol, xmlNodeSyntaxes, Me._compilation, Me._filterSyntaxTree, Me._filterSpanWithinTree, Me._includedFileCache, Me._diagnostics, Me._cancellationToken)
					End If
					If (Not Me.IsInSemanticModelMode) Then
						Dim xmlException As System.Xml.XmlException = XmlDocumentationCommentTextReader.ParseAndGetException(str2)
						If (xmlException Is Nothing) Then
							str = str2
						Else
							If (trivia.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
								Me._diagnostics.Add(ERRID.WRN_XMLDocParseError1, trivia.GetLocation(), New [Object]() { VisualBasicCompilation.DocumentationCommentCompiler.GetDescription(xmlException) })
							End If
							str = Nothing
						End If
					Else
						str = str2
					End If
				Else
					str = Nothing
				End If
				Return str
			End Function

			Friend Shared Function GetDocumentationCommentXml(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal processIncludes As Boolean, ByVal preferredCulture As CultureInfo, ByVal cancellationToken As System.Threading.CancellationToken) As String
				Dim declaringCompilation As VisualBasicCompilation = symbol.DeclaringCompilation
				Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
				Dim stringWriter As System.IO.StringWriter = New System.IO.StringWriter(instance.Builder, CultureInfo.InvariantCulture)
				Dim nullable As Nullable(Of TextSpan) = Nothing
				Dim documentationCommentCompiler As VisualBasicCompilation.DocumentationCommentCompiler = New VisualBasicCompilation.DocumentationCommentCompiler(Nothing, declaringCompilation, stringWriter, processIncludes, True, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, Nothing, nullable, preferredCulture, cancellationToken)
				documentationCommentCompiler.Visit(symbol)
				stringWriter.Dispose()
				Return instance.ToStringAndFree()
			End Function

			Private Shared Function GetElementAttributes(ByVal element As XmlNodeSyntax) As SortedDictionary(Of String, String)
				Dim localName As SyntaxToken
				Dim strs As SortedDictionary(Of String, String) = New SortedDictionary(Of String, String)()
				Dim enumerator As SyntaxList(Of XmlNodeSyntax).Enumerator = VisualBasicCompilation.DocumentationCommentCompiler.GetXmlElementAttributes(element).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As XmlNodeSyntax = enumerator.Current
					Dim valueText As String = Nothing
					Dim xmlString As String = Nothing
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
						Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)
						If (xmlAttributeSyntax.Name.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName OrElse xmlAttributeSyntax.Value.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString) Then
							Continue While
						End If
						localName = DirectCast(xmlAttributeSyntax.Name, XmlNameSyntax).LocalName
						valueText = localName.ValueText
						xmlString = Microsoft.CodeAnalysis.VisualBasic.Binder.GetXmlString(DirectCast(xmlAttributeSyntax.Value, XmlStringSyntax).TextTokens)
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) Then
						valueText = "cref"
						xmlString = DirectCast(current, XmlCrefAttributeSyntax).Reference.ToFullString().Trim()
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameAttribute) Then
						valueText = "name"
						localName = DirectCast(current, XmlNameAttributeSyntax).Reference.Identifier
						xmlString = localName.ToString()
					Else
						Continue While
					End If
					If (valueText Is Nothing OrElse xmlString Is Nothing OrElse strs.ContainsKey(valueText)) Then
						Continue While
					End If
					strs.Add(valueText, xmlString.Trim())
				End While
				Return strs
			End Function

			Private Shared Function GetElementNameOfWellKnownTag(ByVal tag As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag) As String
				Dim str As String
				Dim wellKnownTag As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag = tag
				If (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Permission) Then
					If (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.List) Then
						If (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Exception) Then
							Select Case wellKnownTag
								Case VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.C
									str = "c"
									Exit Select
								Case VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Code
									str = "code"
									Exit Select
								Case VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.C Or VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Code
									Throw ExceptionUtilities.UnexpectedValue(tag)
								Case VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Example
									str = "example"
									Exit Select
								Case Else
									If (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Exception) Then
										str = "exception"
										Exit Select
									Else
										Throw ExceptionUtilities.UnexpectedValue(tag)
									End If
							End Select
						ElseIf (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Include) Then
							str = "include"
						Else
							If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.List) Then
								Throw ExceptionUtilities.UnexpectedValue(tag)
							End If
							str = "list"
						End If
					ElseIf (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param) Then
						If (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Para) Then
							str = "para"
						Else
							If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param) Then
								Throw ExceptionUtilities.UnexpectedValue(tag)
							End If
							str = "param"
						End If
					ElseIf (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.ParamRef) Then
						str = "paramref"
					Else
						If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Permission) Then
							Throw ExceptionUtilities.UnexpectedValue(tag)
						End If
						str = "permission"
					End If
				ElseIf (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.SeeAlso) Then
					If (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns) Then
						If (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Remarks) Then
							str = "remarks"
						Else
							If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns) Then
								Throw ExceptionUtilities.UnexpectedValue(tag)
							End If
							str = "returns"
						End If
					ElseIf (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.See) Then
						str = "see"
					Else
						If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.SeeAlso) Then
							Throw ExceptionUtilities.UnexpectedValue(tag)
						End If
						str = "seealso"
					End If
				ElseIf (wellKnownTag <= VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam) Then
					If (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Summary) Then
						str = "summary"
					Else
						If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam) Then
							Throw ExceptionUtilities.UnexpectedValue(tag)
						End If
						str = "typeparam"
					End If
				ElseIf (wellKnownTag = VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParamRef) Then
					str = "typeparamref"
				Else
					If (wellKnownTag <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value) Then
						Throw ExceptionUtilities.UnexpectedValue(tag)
					End If
					str = "value"
				End If
				Return str
			End Function

			Private Function GetFirstNameAttributeValue(ByVal element As XmlNodeSyntax, ByVal symbolName As String, ByVal badNameValueError As ERRID, ByVal missingNameValueError As ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
				Dim xmlNameAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
				Dim enumerator As SyntaxList(Of XmlNodeSyntax).Enumerator = VisualBasicCompilation.DocumentationCommentCompiler.GetXmlElementAttributes(element).GetEnumerator()
				While True
					If (enumerator.MoveNext()) Then
						Dim current As XmlNodeSyntax = enumerator.Current
						If (current.Kind() = SyntaxKind.XmlNameAttribute) Then
							xmlNameAttributeSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax)
							Exit While
						ElseIf (current.Kind() = SyntaxKind.XmlAttribute) Then
							Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)
							Dim name As XmlNodeSyntax = xmlAttributeSyntax.Name
							If (name.Kind() = SyntaxKind.XmlName AndAlso DocumentationCommentXmlNames.AttributeEquals(DirectCast(name, XmlNameSyntax).LocalName.ValueText, "name")) Then
								If (element.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
									Dim value As XmlNodeSyntax = xmlAttributeSyntax.Value
									Dim str As String = If(value.Kind() = SyntaxKind.XmlString, Microsoft.CodeAnalysis.VisualBasic.Binder.GetXmlString(DirectCast(value, XmlStringSyntax).TextTokens), value.ToString())
									Me._diagnostics.Add(badNameValueError, current.GetLocation(), New [Object]() { str, symbolName })
								End If
								xmlNameAttributeSyntax = Nothing
								Exit While
							End If
						End If
					Else
						If (missingNameValueError <> ERRID.ERR_None AndAlso element.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
							Me._diagnostics.Add(missingNameValueError, element.GetLocation())
						End If
						xmlNameAttributeSyntax = Nothing
						Exit While
					End If
				End While
				Return xmlNameAttributeSyntax
			End Function

			Private Shared Function GetIndexOfFirstNonWhitespaceChar(ByVal str As String) As Integer
				Return VisualBasicCompilation.DocumentationCommentCompiler.GetIndexOfFirstNonWhitespaceChar(str, 0, str.Length)
			End Function

			Private Shared Function GetIndexOfFirstNonWhitespaceChar(ByVal str As String, ByVal start As Integer, ByVal [end] As Integer) As Integer
				While start < [end] And [Char].IsWhiteSpace(str(start))
					start = start + 1
				End While
				Return start
			End Function

			Private Shared Function GetSymbolName(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As String
				Dim str As String
				Dim str1 As String
				Dim kind As SymbolKind = symbol.Kind
				Select Case kind
					Case SymbolKind.[Event]
						str = "event"
						Exit Select
					Case SymbolKind.Field
						Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = DirectCast(symbol, FieldSymbol).AssociatedSymbol
						str = If(associatedSymbol Is Nothing OrElse Not associatedSymbol.IsWithEventsProperty(), "variable", "WithEvents variable")
						Exit Select
					Case SymbolKind.Label
					Case SymbolKind.Local
					Case SymbolKind.NetModule
						Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
					Case SymbolKind.Method
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (methodSymbol.MethodKind = MethodKind.DeclareMethod) Then
							str1 = "declare"
						ElseIf (methodSymbol.MethodKind = MethodKind.UserDefinedOperator OrElse methodSymbol.MethodKind = MethodKind.Conversion) Then
							str1 = "operator"
						Else
							str1 = If(DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).IsSub, "sub", "function")
						End If
						str = str1
						Exit Select
					Case SymbolKind.NamedType
						Select Case DirectCast(symbol, NamedTypeSymbol).TypeKind
							Case Microsoft.CodeAnalysis.TypeKind.[Class]
								str = "class"

							Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
								str = "delegate"

							Case Microsoft.CodeAnalysis.TypeKind.Dynamic
							Case Microsoft.CodeAnalysis.TypeKind.[Error]
							Case Microsoft.CodeAnalysis.TypeKind.Pointer
								Throw ExceptionUtilities.UnexpectedValue(DirectCast(symbol, NamedTypeSymbol).TypeKind)
							Case Microsoft.CodeAnalysis.TypeKind.[Enum]
								str = "enum"

							Case Microsoft.CodeAnalysis.TypeKind.[Interface]
								str = "interface"

							Case Microsoft.CodeAnalysis.TypeKind.[Module]
								str = "module"

							Case Microsoft.CodeAnalysis.TypeKind.Struct
								str = "structure"

							Case Else
								Throw ExceptionUtilities.UnexpectedValue(DirectCast(symbol, NamedTypeSymbol).TypeKind)
						End Select

					Case Else
						If (kind = SymbolKind.[Property]) Then
							str = "property"
							Exit Select
						Else
							Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
						End If
				End Select
				Return str
			End Function

			Private Shared Function GetWellKnownTag(ByVal elementName As String) As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag
				' 
				' Current member / type: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation/DocumentationCommentCompiler/WellKnownTag Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation/DocumentationCommentCompiler::GetWellKnownTag(System.String)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation/DocumentationCommentCompiler/WellKnownTag GetWellKnownTag(System.String)
				' 
				' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
				' Nom du paramètre : index
				'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
				'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
				'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Private Shared Function GetXmlElementAttributes(ByVal element As XmlNodeSyntax) As SyntaxList(Of XmlNodeSyntax)
				Dim xmlElementAttributes As SyntaxList(Of XmlNodeSyntax)
				Select Case element.Kind()
					Case SyntaxKind.XmlElement
						xmlElementAttributes = VisualBasicCompilation.DocumentationCommentCompiler.GetXmlElementAttributes(DirectCast(element, XmlElementSyntax).StartTag)
						Exit Select
					Case SyntaxKind.XmlText
					Case SyntaxKind.XmlElementEndTag
					Label0:
						xmlElementAttributes = New SyntaxList(Of XmlNodeSyntax)()
						Exit Select
					Case SyntaxKind.XmlElementStartTag
						xmlElementAttributes = DirectCast(element, XmlElementStartTagSyntax).Attributes
						Exit Select
					Case SyntaxKind.XmlEmptyElement
						xmlElementAttributes = DirectCast(element, XmlEmptyElementSyntax).Attributes
						Exit Select
					Case Else
						GoTo Label0
				End Select
				Return xmlElementAttributes
			End Function

			Private Sub Indent()
				Me._writer.Indent()
			End Sub

			Private Shared Function IndexOfNewLine(ByVal str As String, ByVal start As Integer, <Out> ByRef newLineLength As Integer) As Integer
				Dim num As Integer
				Dim length As Integer = str.Length
				While True
					If (start < length) Then
						Dim chr As Char = str(start)
						If (chr = Strings.ChrW(10)) Then
							newLineLength = 1
							num = start
							Exit While
						ElseIf (chr <> Strings.ChrW(13)) Then
							start = start + 1
						Else
							If (start + 1 >= str.Length OrElse str(start + 1) <> Strings.ChrW(10)) Then
								newLineLength = 1
							Else
								newLineLength = 2
							End If
							num = start
							Exit While
						End If
					Else
						newLineLength = 0
						num = start
						Exit While
					End If
				End While
				Return num
			End Function

			Private Sub ReportIllegalWellKnownTagIfAny(ByVal tag As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal symbolName As String)
				Me.ReportIllegalWellKnownTagIfAny(tag, ERRID.WRN_XMLDocIllegalTagOnElement2, wellKnownElementNodes, New [Object]() { VisualBasicCompilation.DocumentationCommentCompiler.GetElementNameOfWellKnownTag(tag), symbolName })
			End Sub

			Private Sub ReportIllegalWellKnownTagIfAny(ByVal tag As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ByVal errorId As ERRID, ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal ParamArray args As Object())
				Dim xmlNodeSyntaxes As ArrayBuilder(Of XmlNodeSyntax) = Nothing
				If (wellKnownElementNodes.TryGetValue(tag, xmlNodeSyntaxes)) Then
					Dim enumerator As ArrayBuilder(Of XmlNodeSyntax).Enumerator = xmlNodeSyntaxes.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As XmlNodeSyntax = enumerator.Current
						If (Not current.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
							Continue While
						End If
						Me._diagnostics.Add(errorId, current.GetLocation(), args)
					End While
				End If
			End Sub

			Private Sub ReportWarningsForDuplicatedTags(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), Optional ByVal isEvent As Boolean = False)
				Dim xmlNodeSyntaxes As ArrayBuilder(Of XmlNodeSyntax) = Nothing
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Include, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "include")
				End If
				If (Not isEvent AndAlso wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "param")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Permission, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "permission")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Remarks, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "remarks")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "returns")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Summary, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "summary")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "typeparam")
				End If
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value, xmlNodeSyntaxes)) Then
					Me.ReportWarningsForDuplicatedTags(xmlNodeSyntaxes, "value")
				End If
			End Sub

			Private Sub ReportWarningsForDuplicatedTags(ByVal nodes As ArrayBuilder(Of XmlNodeSyntax), ByVal tagName As String)
				If (nodes IsNot Nothing AndAlso nodes.Count >= 2) Then
					Dim flag As Boolean = nodes(0).SyntaxTree.ReportDocumentationCommentDiagnostics()
					Dim instance As ArrayBuilder(Of VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes) = ArrayBuilder(Of VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes).GetInstance()
					Dim count As Integer = nodes.Count - 1
					Dim num As Integer = 0
					Do
						instance.Add(New VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes(nodes(num)))
						num = num + 1
					Loop While num <= count
					instance.Sort()
					Dim count1 As Integer = instance.Count - 2
					Dim num1 As Integer = 0
					Do
						Dim item As VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes = instance(num1)
						Dim xmlNodeWithAttribute As VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes = instance(num1 + 1)
						If (VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes.CompareAttributes(item.Attributes, xmlNodeWithAttribute.Attributes) = 0 AndAlso flag) Then
							Me._diagnostics.Add(ERRID.WRN_XMLDocDuplicateXMLNode1, xmlNodeWithAttribute.Node.GetLocation(), New [Object]() { tagName })
						End If
						num1 = num1 + 1
					Loop While num1 <= count1
					instance.Free()
				End If
			End Sub

			Private Sub ReportWarningsForExceptionTags(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)))
				Dim xmlNodeSyntaxes As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Nothing
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Exception, xmlNodeSyntaxes)) Then
					Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax).Enumerator = xmlNodeSyntaxes.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = enumerator.Current
						Dim flag As Boolean = False
						Dim enumerator1 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax).Enumerator = VisualBasicCompilation.DocumentationCommentCompiler.GetXmlElementAttributes(current).GetEnumerator()
						While enumerator1.MoveNext()
							Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = enumerator1.Current
							Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = xmlNodeSyntax.Kind()
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
								Dim name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(xmlNodeSyntax, XmlAttributeSyntax).Name
								If (name.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName OrElse Not DocumentationCommentXmlNames.AttributeEquals(DirectCast(name, XmlNameSyntax).LocalName.ValueText, "cref")) Then
									Continue While
								End If
								flag = True
								Exit While
							Else
								If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) Then
									Continue While
								End If
								flag = True
								Exit While
							End If
						End While
						If (flag OrElse Not current.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
							Continue While
						End If
						Me._diagnostics.Add(ERRID.WRN_XMLDocExceptionTagWithoutCRef, current.GetLocation())
					End While
				End If
			End Sub

			Private Sub ReportWarningsForParamAndParamRefTags(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal symbolName As String, ByVal parameters As ImmutableArray(Of ParameterSymbol))
				Me.ReportWarningsForParamOrTypeParamTags(Of ParameterSymbol)(wellKnownElementNodes, VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param, VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.ParamRef, symbolName, ERRID.WRN_XMLDocBadParamTag2, ERRID.WRN_XMLDocParamTagWithoutName, parameters)
			End Sub

			Private Sub ReportWarningsForParamOrTypeParamTags(Of TSymbol As Symbol)(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal tag As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ByVal tagRef As VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ByVal symbolName As String, ByVal badNameValueError As ERRID, ByVal missingNameValueError As ERRID, ByVal allowedSymbols As ImmutableArray(Of TSymbol))
				Dim xmlNodeSyntaxes As ArrayBuilder(Of XmlNodeSyntax) = Nothing
				wellKnownElementNodes.TryGetValue(tag, xmlNodeSyntaxes)
				Dim xmlNodeSyntaxes1 As ArrayBuilder(Of XmlNodeSyntax) = Nothing
				If (tagRef <> VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.None) Then
					wellKnownElementNodes.TryGetValue(tagRef, xmlNodeSyntaxes1)
				End If
				If (xmlNodeSyntaxes IsNot Nothing OrElse xmlNodeSyntaxes1 IsNot Nothing) Then
					Dim strs As HashSet(Of String) = Nothing
					If (allowedSymbols.Length > 10) Then
						strs = New HashSet(Of String)(CaseInsensitiveComparison.Comparer)
						Dim enumerator As ImmutableArray(Of TSymbol).Enumerator = allowedSymbols.GetEnumerator()
						While enumerator.MoveNext()
							strs.Add(enumerator.Current.Name)
						End While
					End If
					If (xmlNodeSyntaxes IsNot Nothing) Then
						Me.ReportWarningsForParamOrTypeParamTags(Of TSymbol)(xmlNodeSyntaxes, symbolName, badNameValueError, missingNameValueError, allowedSymbols, strs)
					End If
					If (xmlNodeSyntaxes1 IsNot Nothing) Then
						Me.ReportWarningsForParamOrTypeParamTags(Of TSymbol)(xmlNodeSyntaxes1, symbolName, badNameValueError, ERRID.ERR_None, allowedSymbols, strs)
					End If
				End If
			End Sub

			Private Sub ReportWarningsForParamOrTypeParamTags(Of TSymbol As Symbol)(ByVal builder As ArrayBuilder(Of XmlNodeSyntax), ByVal symbolName As String, ByVal badNameValueError As ERRID, ByVal missingNameValueError As ERRID, ByVal allowedSymbols As ImmutableArray(Of TSymbol), ByVal [set] As HashSet(Of String))
				Dim enumerator As ArrayBuilder(Of XmlNodeSyntax).Enumerator = builder.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As XmlNodeSyntax = enumerator.Current
					Dim firstNameAttributeValue As XmlNameAttributeSyntax = Me.GetFirstNameAttributeValue(current, symbolName, badNameValueError, missingNameValueError)
					If (firstNameAttributeValue Is Nothing) Then
						Continue While
					End If
					Dim valueText As String = firstNameAttributeValue.Reference.Identifier.ValueText
					Dim flag As Boolean = True
					If ([set] IsNot Nothing) Then
						flag = Not [set].Contains(valueText)
					Else
						Dim enumerator1 As ImmutableArray(Of TSymbol).Enumerator = allowedSymbols.GetEnumerator()
						While enumerator1.MoveNext()
							If (Not CaseInsensitiveComparison.Equals(valueText, enumerator1.Current.Name)) Then
								Continue While
							End If
							flag = False
							GoTo Label0
						End While
					End If
				Label0:
					If (Not flag OrElse Not current.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
						Continue While
					End If
					Me._diagnostics.Add(badNameValueError, current.GetLocation(), New [Object]() { valueText.Trim(), symbolName })
				End While
			End Sub

			Private Sub ReportWarningsForTypeParamRefTags(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal symbolName As String, ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal tree As SyntaxTree)
				Dim xmlNodeSyntaxes As ArrayBuilder(Of XmlNodeSyntax) = Nothing
				If (wellKnownElementNodes.TryGetValue(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParamRef, xmlNodeSyntaxes)) Then
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = VisualBasicCompilation.DocumentationCommentCompiler.CreateDocumentationCommentBinderForSymbol(Me.[Module], symbol, tree, DocumentationCommentBinder.BinderType.NameInTypeParamRef)
					Dim enumerator As ArrayBuilder(Of XmlNodeSyntax).Enumerator = xmlNodeSyntaxes.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As XmlNodeSyntax = enumerator.Current
						Dim firstNameAttributeValue As XmlNameAttributeSyntax = Me.GetFirstNameAttributeValue(current, symbolName, ERRID.WRN_XMLDocBadGenericParamTag2, ERRID.ERR_None)
						If (firstNameAttributeValue Is Nothing) Then
							Continue While
						End If
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = binder.GetNewCompoundUseSiteInfo(Me._diagnostics)
						Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = binder.BindXmlNameAttributeValue(firstNameAttributeValue.Reference, newCompoundUseSiteInfo)
						If (Not current.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
							Me._diagnostics.AddDependencies(newCompoundUseSiteInfo)
						Else
							Me._diagnostics.Add(current, newCompoundUseSiteInfo)
						End If
						Dim kind As Boolean = True
						If (Not symbols.IsDefault AndAlso symbols.Length = 1) Then
							kind = symbols(0).Kind <> SymbolKind.TypeParameter
						End If
						If (Not kind OrElse Not current.SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
							Continue While
						End If
						Me._diagnostics.Add(ERRID.WRN_XMLDocBadGenericParamTag2, current.GetLocation(), New [Object]() { firstNameAttributeValue.Reference.Identifier.ValueText, symbolName })
					End While
				End If
			End Sub

			Private Sub ReportWarningsForTypeParamTags(ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal symbolName As String, ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol))
				Me.ReportWarningsForParamOrTypeParamTags(Of TypeParameterSymbol)(wellKnownElementNodes, VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.None, symbolName, ERRID.WRN_XMLDocBadGenericParamTag2, ERRID.WRN_XMLDocGenericParamTagWithoutName, typeParameters)
			End Sub

			Private Function ShouldSkipSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
				If (Me._filterSyntaxTree Is Nothing) Then
					Return False
				End If
				Return Not symbol.IsDefinedInSourceTree(Me._filterSyntaxTree, Me._filterSpanWithinTree, Me._cancellationToken)
			End Function

			Private Shared Function TrimmedStringStartsWith(ByVal str As String, ByVal prefix As String) As Boolean
				Dim flag As Boolean
				Dim indexOfFirstNonWhitespaceChar As Integer = VisualBasicCompilation.DocumentationCommentCompiler.GetIndexOfFirstNonWhitespaceChar(str)
				If (str.Length - indexOfFirstNonWhitespaceChar >= prefix.Length) Then
					Dim num As Integer = 0
					Dim num1 As Integer = indexOfFirstNonWhitespaceChar
					While num < prefix.Length
						If (prefix(num) = str(num1)) Then
							num = num + 1
							num1 = num1 + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				Else
					flag = False
				End If
				Return flag
			End Function

			Private Function TryGetDocCommentTriviaAndGenerateDiagnostics(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
				Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
				Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Nothing
				Dim flag As Boolean = False
				Dim enumerator As SyntaxTriviaList.Enumerator = syntaxNode.GetLeadingTrivia().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxTrivia = enumerator.Current
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia) Then
						If ([structure] IsNot Nothing AndAlso DirectCast(current.SyntaxTree, VisualBasicSyntaxTree).ReportDocumentationCommentDiagnostics()) Then
							Me._diagnostics.Add(ERRID.WRN_XMLDocMoreThanOneCommentBlock, [structure].GetLocation())
						End If
						[structure] = DirectCast(current.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax)
						flag = False
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia) Then
						flag = True
					End If
				End While
				If ([structure] Is Nothing) Then
					documentationCommentTriviaSyntax = Nothing
				ElseIf (Not flag) Then
					documentationCommentTriviaSyntax = [structure]
				Else
					If ([structure].SyntaxTree.ReportDocumentationCommentDiagnostics()) Then
						Me._diagnostics.Add(ERRID.WRN_XMLDocBadXMLLine, [structure].GetLocation())
					End If
					documentationCommentTriviaSyntax = Nothing
				End If
				Return documentationCommentTriviaSyntax
			End Function

			Private Sub Unindent()
				Me._writer.Unindent()
			End Sub

			Public Overrides Sub VisitEvent(ByVal symbol As EventSymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol)
					If (sourceEventSymbol IsNot Nothing) Then
						Me.WriteDocumentationCommentForEvent(sourceEventSymbol)
					End If
				End If
			End Sub

			Public Overrides Sub VisitField(ByVal symbol As FieldSymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)
					If (sourceFieldSymbol IsNot Nothing) Then
						Me.WriteDocumentationCommentForField(sourceFieldSymbol)
					End If
				End If
			End Sub

			Public Overrides Sub VisitMethod(ByVal symbol As MethodSymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					Dim sourceNonPropertyAccessorMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNonPropertyAccessorMethodSymbol = TryCast(symbol, SourceMemberMethodSymbol)
					If (sourceNonPropertyAccessorMethodSymbol Is Nothing) Then
						sourceNonPropertyAccessorMethodSymbol = TryCast(symbol, SourceDeclareMethodSymbol)
					End If
					Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = sourceNonPropertyAccessorMethodSymbol
					If (sourceMethodSymbol IsNot Nothing) Then
						Me.WriteDocumentationCommentForMethod(sourceMethodSymbol)
					End If
				End If
			End Sub

			Public Overrides Sub VisitNamedType(ByVal symbol As NamedTypeSymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
					If (sourceNamedTypeSymbol IsNot Nothing) Then
						Me.WriteDocumentationCommentForNamedType(sourceNamedTypeSymbol)
					End If
					If (Not Me._isForSingleSymbol) Then
						Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
						While enumerator.MoveNext()
							Me.Visit(enumerator.Current)
						End While
					End If
				End If
			End Sub

			Public Overrides Sub VisitNamespace(ByVal symbol As NamespaceSymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					If (symbol.IsGlobalNamespace) Then
						Me.WriteLine("<?xml version=""1.0""?>")
						Me.WriteLine("<doc>")
						Me.Indent()
						If (Not Me._compilation.Options.OutputKind.IsNetModule()) Then
							Me.WriteLine("<assembly>")
							Me.Indent()
							Me.WriteLine("<name>")
							Me.WriteLine(Me._assemblyName)
							Me.WriteLine("</name>")
							Me.Unindent()
							Me.WriteLine("</assembly>")
						End If
						Me.WriteLine("<members>")
						Me.Indent()
					End If
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
					While enumerator.MoveNext()
						Me.Visit(enumerator.Current)
					End While
					If (symbol.IsGlobalNamespace) Then
						Me.Unindent()
						Me.WriteLine("</members>")
						Me.Unindent()
						Me.WriteLine("</doc>")
					End If
				End If
			End Sub

			Public Overrides Sub VisitProperty(ByVal symbol As PropertySymbol)
				Me._cancellationToken.ThrowIfCancellationRequested()
				If (Not Me.ShouldSkipSymbol(symbol)) Then
					Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
					If (sourcePropertySymbol IsNot Nothing) Then
						Me.WriteDocumentationCommentForProperty(sourcePropertySymbol)
					End If
				End If
			End Sub

			Private Sub Write(ByVal message As String)
				Me._writer.Write(message)
			End Sub

			Private Sub WriteDocumentationCommentForEvent(ByVal [event] As SourceEventSymbol)
				Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Me.TryGetDocCommentTriviaAndGenerateDiagnostics([event].SyntaxReference.GetVisualBasicSyntax(Me._cancellationToken))
				If (documentationCommentTriviaSyntax IsNot Nothing) Then
					Dim wellKnownTags As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)) = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))()
					Dim documentationCommentForSymbol As String = Me.GetDocumentationCommentForSymbol([event], documentationCommentTriviaSyntax, wellKnownTags)
					If (documentationCommentForSymbol Is Nothing) Then
						VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
						Return
					End If
					If (documentationCommentTriviaSyntax.SyntaxTree.ReportDocumentationCommentDiagnostics() OrElse Me._writer.IsSpecified) Then
						Dim symbolName As String = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName([event])
						Me.ReportWarningsForDuplicatedTags(wellKnownTags, True)
						Me.ReportWarningsForExceptionTags(wellKnownTags)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, wellKnownTags, symbolName)
						Dim empty As ImmutableArray(Of ParameterSymbol) = ImmutableArray(Of ParameterSymbol).Empty
						Dim type As NamedTypeSymbol = TryCast([event].Type, NamedTypeSymbol)
						If (type IsNot Nothing) Then
							Dim delegateInvokeMethod As MethodSymbol = type.DelegateInvokeMethod
							If (delegateInvokeMethod IsNot Nothing) Then
								empty = delegateInvokeMethod.Parameters
							End If
						End If
						Me.ReportWarningsForParamAndParamRefTags(wellKnownTags, symbolName, empty)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
						Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, [event], documentationCommentTriviaSyntax.SyntaxTree)
					End If
					VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
					Me.WriteDocumentationCommentForSymbol(documentationCommentForSymbol)
				End If
			End Sub

			Private Sub WriteDocumentationCommentForField(ByVal field As SourceFieldSymbol)
				Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Me.TryGetDocCommentTriviaAndGenerateDiagnostics(field.DeclarationSyntax)
				If (documentationCommentTriviaSyntax IsNot Nothing) Then
					Dim wellKnownTags As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)) = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))()
					Dim documentationCommentForSymbol As String = Me.GetDocumentationCommentForSymbol(field, documentationCommentTriviaSyntax, wellKnownTags)
					If (documentationCommentForSymbol Is Nothing) Then
						VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
						Return
					End If
					If (documentationCommentTriviaSyntax.SyntaxTree.ReportDocumentationCommentDiagnostics() OrElse Me._writer.IsSpecified) Then
						Dim symbolName As String = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName(field)
						Me.ReportWarningsForDuplicatedTags(wellKnownTags, False)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Exception, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.ParamRef, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
						Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, field, documentationCommentTriviaSyntax.SyntaxTree)
					End If
					VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
					Me.WriteDocumentationCommentForSymbol(documentationCommentForSymbol)
				End If
			End Sub

			Private Function WriteDocumentationCommentForMethod(ByVal method As SourceMethodSymbol) As Boolean
				Dim flag As Boolean
				Dim partialImplementationPart As SourceMethodSymbol = TryCast(method.PartialImplementationPart, SourceMethodSymbol)
				If (partialImplementationPart Is Nothing OrElse Not Me.WriteDocumentationCommentForMethod(partialImplementationPart)) Then
					Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Me.TryGetDocCommentTriviaAndGenerateDiagnostics(method.Syntax)
					If (documentationCommentTriviaSyntax IsNot Nothing) Then
						Dim wellKnownTags As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)) = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))()
						Dim documentationCommentForSymbol As String = Me.GetDocumentationCommentForSymbol(method, documentationCommentTriviaSyntax, wellKnownTags)
						If (documentationCommentForSymbol IsNot Nothing) Then
							If (documentationCommentTriviaSyntax.SyntaxTree.ReportDocumentationCommentDiagnostics() OrElse Me._writer.IsSpecified) Then
								Dim symbolName As String = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName(method)
								Me.ReportWarningsForDuplicatedTags(wellKnownTags, False)
								Me.ReportWarningsForExceptionTags(wellKnownTags)
								If (method.IsSub) Then
									If (method.MethodKind <> MethodKind.DeclareMethod) Then
										Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, wellKnownTags, symbolName)
									Else
										Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, ERRID.WRN_XMLDocReturnsOnADeclareSub, wellKnownTags, New [Object](-1) {})
									End If
								End If
								Me.ReportWarningsForParamAndParamRefTags(wellKnownTags, symbolName, method.Parameters)
								Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value, wellKnownTags, symbolName)
								If (method.MethodKind = MethodKind.UserDefinedOperator OrElse method.MethodKind = MethodKind.DeclareMethod) Then
									Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
								Else
									Me.ReportWarningsForTypeParamTags(wellKnownTags, symbolName, method.TypeParameters)
								End If
								Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, method, documentationCommentTriviaSyntax.SyntaxTree)
							End If
							VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
							Me.WriteDocumentationCommentForSymbol(documentationCommentForSymbol)
							flag = True
						Else
							VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
							flag = False
						End If
					Else
						flag = False
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Private Sub WriteDocumentationCommentForNamedType(ByVal namedType As SourceNamedTypeSymbol)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax).GetInstance()
				Dim documentationMode As Microsoft.CodeAnalysis.DocumentationMode = Microsoft.CodeAnalysis.DocumentationMode.None
				Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = namedType.SyntaxReferences.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxReference = enumerator.Current
					Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Me.TryGetDocCommentTriviaAndGenerateDiagnostics(current.GetVisualBasicSyntax(Me._cancellationToken))
					If (documentationCommentTriviaSyntax Is Nothing) Then
						Continue While
					End If
					instance.Add(documentationCommentTriviaSyntax)
					Dim documentationMode1 As Microsoft.CodeAnalysis.DocumentationMode = documentationCommentTriviaSyntax.SyntaxTree.Options.DocumentationMode
					If (documentationMode >= documentationMode1) Then
						Continue While
					End If
					documentationMode = documentationMode1
				End While
				Dim symbolName As String = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName(namedType)
				If (instance.Count > 1) Then
					If (documentationMode = Microsoft.CodeAnalysis.DocumentationMode.Diagnose) Then
						Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax).Enumerator = instance.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = enumerator1.Current
							Me._diagnostics.Add(ERRID.WRN_XMLDocOnAPartialType, current1.GetLocation(), New [Object]() { symbolName })
						End While
					End If
					instance.Free()
					Return
				End If
				If (instance.Count = 0) Then
					instance.Free()
					Return
				End If
				Dim wellKnownTags As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)) = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))()
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = instance(0)
				instance.Free()
				Dim documentationCommentForSymbol As String = Me.GetDocumentationCommentForSymbol(namedType, item, wellKnownTags)
				If (documentationCommentForSymbol Is Nothing) Then
					VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
					Return
				End If
				If (item.SyntaxTree.ReportDocumentationCommentDiagnostics() OrElse Me._writer.IsSpecified) Then
					Dim delegateInvokeMethod As MethodSymbol = namedType.DelegateInvokeMethod
					Me.ReportWarningsForDuplicatedTags(wellKnownTags, False)
					Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Exception, wellKnownTags, symbolName)
					If (namedType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, wellKnownTags, symbolName)
					ElseIf (delegateInvokeMethod IsNot Nothing AndAlso delegateInvokeMethod.IsSub) Then
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, wellKnownTags, "delegate sub")
					End If
					If (namedType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Param, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.ParamRef, wellKnownTags, symbolName)
					Else
						Me.ReportWarningsForParamAndParamRefTags(wellKnownTags, VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName(delegateInvokeMethod), delegateInvokeMethod.Parameters)
					End If
					Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Value, wellKnownTags, symbolName)
					If (namedType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
						Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, namedType, item.SyntaxTree)
					ElseIf (namedType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse namedType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
						Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParamRef, wellKnownTags, symbolName)
					Else
						Me.ReportWarningsForTypeParamTags(wellKnownTags, symbolName, namedType.TypeParameters)
						Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, namedType, item.SyntaxTree)
					End If
				End If
				VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
				Me.WriteDocumentationCommentForSymbol(documentationCommentForSymbol)
			End Sub

			Private Sub WriteDocumentationCommentForProperty(ByVal [property] As SourcePropertySymbol)
				If (Not [property].IsWithEventsProperty()) Then
					Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Me.TryGetDocCommentTriviaAndGenerateDiagnostics((If([property].BlockSyntaxReference, [property].SyntaxReference)).GetVisualBasicSyntax(Me._cancellationToken))
					If (documentationCommentTriviaSyntax IsNot Nothing) Then
						Dim wellKnownTags As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)) = New Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))()
						Dim documentationCommentForSymbol As String = Me.GetDocumentationCommentForSymbol([property], documentationCommentTriviaSyntax, wellKnownTags)
						If (documentationCommentForSymbol Is Nothing) Then
							VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
							Return
						End If
						If (documentationCommentTriviaSyntax.SyntaxTree.ReportDocumentationCommentDiagnostics() OrElse Me._writer.IsSpecified) Then
							Dim symbolName As String = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName([property])
							Me.ReportWarningsForDuplicatedTags(wellKnownTags, False)
							Me.ReportWarningsForExceptionTags(wellKnownTags)
							If ([property].IsWriteOnly) Then
								Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.Returns, ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty, wellKnownTags, New [Object](-1) {})
							End If
							Me.ReportWarningsForParamAndParamRefTags(wellKnownTags, symbolName, [property].Parameters)
							Me.ReportIllegalWellKnownTagIfAny(VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag.TypeParam, wellKnownTags, symbolName)
							Me.ReportWarningsForTypeParamRefTags(wellKnownTags, symbolName, [property], documentationCommentTriviaSyntax.SyntaxTree)
						End If
						VisualBasicCompilation.DocumentationCommentCompiler.FreeWellKnownElementNodes(wellKnownTags)
						Me.WriteDocumentationCommentForSymbol(documentationCommentForSymbol)
					End If
				End If
			End Sub

			Private Sub WriteDocumentationCommentForSymbol(ByVal xmlDocComment As String)
				If (Not Me._isForSingleSymbol) Then
					Me.Write(xmlDocComment)
					Return
				End If
				Dim length As Integer = xmlDocComment.Length
				If (length - 1 > 0 AndAlso EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(xmlDocComment(length - 1)), "" & VbCrLf & "", False) = 0) Then
					length = length - 1
					If (length - 1 > 0 AndAlso EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(xmlDocComment(length - 1)), "", False) = 0) Then
						length = length - 1
					End If
				End If
				Me._writer.WriteSubString(xmlDocComment, 0, length, False)
			End Sub

			Friend Shared Sub WriteDocumentationCommentXml(ByVal compilation As VisualBasicCompilation, ByVal assemblyName As String, ByVal xmlDocStream As Stream, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal filterTree As SyntaxTree = Nothing, Optional ByVal filterSpanWithinTree As Nullable(Of TextSpan) = Nothing)
				Dim streamWriter As System.IO.StreamWriter = Nothing
				If (xmlDocStream IsNot Nothing AndAlso xmlDocStream.CanWrite) Then
					streamWriter = New System.IO.StreamWriter(xmlDocStream, New UTF8Encoding(True, False), 1024, True)
				End If
				Try
					Using streamWriter
						Dim documentationCommentCompiler As VisualBasicCompilation.DocumentationCommentCompiler = New VisualBasicCompilation.DocumentationCommentCompiler(If(assemblyName, compilation.SourceAssembly.Name), compilation, streamWriter, True, False, diagnostics, filterTree, filterSpanWithinTree, Nothing, cancellationToken)
						documentationCommentCompiler.Visit(compilation.SourceAssembly.GlobalNamespace)
						If (streamWriter IsNot Nothing) Then
							streamWriter.Flush()
						End If
					End Using
				Catch exception1 As System.Exception
					ProjectData.SetProjectError(exception1)
					Dim exception As System.Exception = exception1
					diagnostics.Add(ERRID.ERR_DocFileGen, Location.None, New [Object]() { exception.Message })
					ProjectData.ClearProjectError()
				End Try
				If (diagnostics.AccumulatesDiagnostics) Then
					If (filterTree IsNot Nothing) Then
						VisualBasicCompilation.DocumentationCommentCompiler.MislocatedDocumentationCommentFinder.ReportUnprocessed(filterTree, filterSpanWithinTree, diagnostics.DiagnosticBag, cancellationToken)
						Return
					End If
					Dim enumerator As ImmutableArray(Of SyntaxTree).Enumerator = compilation.SyntaxTrees.GetEnumerator()
					While enumerator.MoveNext()
						VisualBasicCompilation.DocumentationCommentCompiler.MislocatedDocumentationCommentFinder.ReportUnprocessed(enumerator.Current, Nothing, diagnostics.DiagnosticBag, cancellationToken)
					End While
				End If
			End Sub

			Private Sub WriteFormattedComment(ByVal text As String)
				Dim num As Integer = 3
				Dim num1 As Integer = 0
				Dim length As Integer = text.Length
				While num1 < length
					Dim num2 As Integer = 0
					Dim num3 As Integer = VisualBasicCompilation.DocumentationCommentCompiler.IndexOfNewLine(text, num1, num2)
					Dim indexOfFirstNonWhitespaceChar As Integer = VisualBasicCompilation.DocumentationCommentCompiler.GetIndexOfFirstNonWhitespaceChar(text, num1, num3)
					If (indexOfFirstNonWhitespaceChar < num3 AndAlso text(indexOfFirstNonWhitespaceChar) = "'"C) Then
						indexOfFirstNonWhitespaceChar += num
					End If
					Me._writer.WriteSubString(text, indexOfFirstNonWhitespaceChar, num3 - indexOfFirstNonWhitespaceChar, True)
					num1 = num3 + num2
				End While
			End Sub

			Private Sub WriteLine(ByVal message As String)
				Me._writer.WriteLine(message)
			End Sub

			Private Class DocumentationCommentWalker
				Inherits VisualBasicSyntaxWalker
				Private ReadOnly _symbol As Symbol

				Private ReadOnly _syntaxTree As SyntaxTree

				Private ReadOnly _wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))

				Private ReadOnly _reportDiagnostics As Boolean

				Private ReadOnly _writer As TextWriter

				Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

				Private ReadOnly Property Compilation As VisualBasicCompilation
					Get
						Return Me._symbol.DeclaringCompilation
					End Get
				End Property

				Private ReadOnly Property [Module] As SourceModuleSymbol
					Get
						Return DirectCast(Me.Compilation.SourceModule, SourceModuleSymbol)
					End Get
				End Property

				Private Sub New(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal writer As TextWriter, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
					MyBase.New(SyntaxWalkerDepth.Token)
					Me._symbol = symbol
					Me._syntaxTree = syntaxTree
					Me._wellKnownElementNodes = wellKnownElementNodes
					Me._writer = writer
					Me._diagnostics = diagnostics
					Me._reportDiagnostics = syntaxTree.ReportDocumentationCommentDiagnostics()
				End Sub

				Private Sub CaptureWellKnownTagNode(ByVal node As XmlNodeSyntax, ByVal name As XmlNodeSyntax)
					' 
					' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation/DocumentationCommentCompiler/DocumentationCommentWalker::CaptureWellKnownTagNode(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax,Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
					' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
					' 
					' Product version: 2019.1.118.0
					' Exception in: System.Void CaptureWellKnownTagNode(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax,Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
					' 
					' La référence d'objet n'est pas définie à une instance d'un objet.
					'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
					'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
					'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
					'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
					'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
					'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
					'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
					'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
					' 
					' mailto: JustDecompilePublicFeedback@telerik.com

				End Sub

				Public Overrides Sub DefaultVisit(ByVal node As SyntaxNode)
					Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)
					Dim location As Func(Of Diagnostic, Microsoft.CodeAnalysis.Location)
					Dim func As Func(Of Microsoft.CodeAnalysis.Location, Boolean)
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
							Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)
							If (Not xmlAttributeSyntax.ContainsDiagnostics AndAlso DocumentationCommentXmlNames.AttributeEquals(DirectCast(xmlAttributeSyntax.Name, XmlNameSyntax).LocalName.ValueText, "cref")) Then
								Dim value As XmlStringSyntax = DirectCast(xmlAttributeSyntax.Value, XmlStringSyntax)
								Dim xmlString As String = Microsoft.CodeAnalysis.VisualBasic.Binder.GetXmlString(value.TextTokens)
								Dim flag As Boolean = If(xmlString.Length < 2 OrElse xmlString(0) = ":"C, True, xmlString(1) <> ":"C)
								Me.Visit(xmlAttributeSyntax.Name)
								Me.VisitToken(xmlAttributeSyntax.EqualsToken)
								Me.VisitToken(value.StartQuoteToken)
								If (flag AndAlso Me._reportDiagnostics) Then
									Me._diagnostics.Add(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1, node.GetLocation(), New [Object]() { xmlString.Trim() })
								End If
								If (flag AndAlso Me._writer IsNot Nothing) Then
									Me._writer.Write("!:")
								End If
								Dim enumerator As SyntaxTokenList.Enumerator = value.TextTokens.GetEnumerator()
								While enumerator.MoveNext()
									Me.VisitToken(enumerator.Current)
								End While
								Me.VisitToken(value.EndQuoteToken)
								Return
							End If
						End If
						MyBase.DefaultVisit(node)
						Return
					End If
					Dim xmlCrefAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax)
					Me.Visit(xmlCrefAttributeSyntax.Name)
					Me.VisitToken(xmlCrefAttributeSyntax.EqualsToken)
					Me.VisitToken(xmlCrefAttributeSyntax.StartQuoteToken)
					Dim reference As CrefReferenceSyntax = xmlCrefAttributeSyntax.Reference
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = VisualBasicCompilation.DocumentationCommentCompiler.CreateDocumentationCommentBinderForSymbol(Me.[Module], Me._symbol, Me._syntaxTree, DocumentationCommentBinder.BinderType.Cref)
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = binder.GetNewCompoundUseSiteInfo(Me._diagnostics)
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(True, Me._diagnostics.AccumulatesDependencies)
					Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = binder.BindInsideCrefAttributeValue(reference, False, instance, newCompoundUseSiteInfo)
					Me._diagnostics.AddDependencies(instance, False)
					Me._diagnostics.AddDependencies(newCompoundUseSiteInfo)
					Dim [readOnly] As ImmutableArray(Of Diagnostic) = instance.DiagnosticBag.ToReadOnly()
					If (VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-0 Is Nothing) Then
						location = Function(x As Diagnostic) x.Location
						VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-0 = location
					Else
						location = VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-0
					End If
					Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Diagnostic, Microsoft.CodeAnalysis.Location)([readOnly], location)
					If (VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-1 Is Nothing) Then
						func = Function(x As Microsoft.CodeAnalysis.Location) CObj(x) <> CObj(Nothing)
						VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-1 = func
					Else
						func = VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker._Closure$__.$I16-1
					End If
					Dim locations1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = locations.WhereAsArray(func)
					instance.Free()
					If (Not newCompoundUseSiteInfo.Diagnostics.IsNullOrEmpty() AndAlso Me._reportDiagnostics) Then
						Me.ProcessErrorLocations(node, locations1, newCompoundUseSiteInfo, Nothing)
					End If
					If (symbols.IsEmpty) Then
						compoundUseSiteInfo = New CompoundUseSiteInfo(Of AssemblySymbol)()
						Me.ProcessErrorLocations(xmlCrefAttributeSyntax, locations1, compoundUseSiteInfo, New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID)(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1))
					ElseIf (symbols.Length <= 1 OrElse reference.Signature Is Nothing) Then
						Dim compilation As VisualBasicCompilation = Me.Compilation
						Dim str As String = Nothing
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
						Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbols.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
							If (current.Kind <> SymbolKind.TypeParameter) Then
								Dim documentationCommentId As String = current.OriginalDefinition.GetDocumentationCommentId()
								If (documentationCommentId Is Nothing OrElse str IsNot Nothing AndAlso [String].CompareOrdinal(str, documentationCommentId) <= 0) Then
									Continue While
								End If
								str = documentationCommentId
								symbol = current
							Else
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefToTypeParameter
							End If
						End While
						If (str IsNot Nothing) Then
							If (Me._writer IsNot Nothing) Then
								Me._writer.Write(str)
							End If
							Me._diagnostics.AddAssembliesUsedByCrefTarget(symbol.OriginalDefinition)
						Else
							compoundUseSiteInfo = New CompoundUseSiteInfo(Of AssemblySymbol)()
							Me.ProcessErrorLocations(xmlCrefAttributeSyntax, locations1, compoundUseSiteInfo, New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID)(eRRID))
						End If
					Else
						compoundUseSiteInfo = New CompoundUseSiteInfo(Of AssemblySymbol)()
						Me.ProcessErrorLocations(xmlCrefAttributeSyntax, locations1, compoundUseSiteInfo, New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID)(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1))
					End If
					Me.VisitToken(xmlCrefAttributeSyntax.EndQuoteToken)
				End Sub

				Friend Shared Function GetSubstitutedText(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal trivia As DocumentationCommentTriviaSyntax, ByVal wellKnownElementNodes As Dictionary(Of VisualBasicCompilation.DocumentationCommentCompiler.WellKnownTag, ArrayBuilder(Of XmlNodeSyntax)), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As String
					Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
					Using stringWriter As System.IO.StringWriter = New System.IO.StringWriter(instance.Builder)
						Dim documentationCommentWalker As VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker = New VisualBasicCompilation.DocumentationCommentCompiler.DocumentationCommentWalker(symbol, trivia.SyntaxTree, wellKnownElementNodes, stringWriter, diagnostics)
						documentationCommentWalker.WriteHeaderAndVisit(symbol, trivia)
					End Using
					Return instance.ToStringAndFree()
				End Function

				Private Sub ProcessBadNameInCrefAttribute(ByVal crefAttribute As XmlCrefAttributeSyntax, ByVal errorLocation As Microsoft.CodeAnalysis.Location, ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID)
					If (Me._writer IsNot Nothing) Then
						Me._writer.Write("!:")
					End If
					Dim reference As VisualBasicSyntaxNode = crefAttribute.Reference
					Me.Visit(reference)
					If (Me._reportDiagnostics) Then
						Dim location As Microsoft.CodeAnalysis.Location = If(errorLocation, reference.GetLocation())
						Me._diagnostics.Add(errid, location, New [Object]() { reference.ToFullString().TrimEnd(New [Char](-1) {}) })
					End If
				End Sub

				Private Sub ProcessErrorLocations(ByVal node As SyntaxNode, ByVal errorLocations As ImmutableArray(Of Location), ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal errid As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID))
					Dim xmlCrefAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax)
					If (xmlCrefAttributeSyntax IsNot Nothing AndAlso errid.HasValue) Then
						If (errorLocations.Length = 0) Then
							Me.ProcessBadNameInCrefAttribute(xmlCrefAttributeSyntax, xmlCrefAttributeSyntax.GetLocation(), errid.Value)
							Return
						End If
						Dim enumerator As ImmutableArray(Of Location).Enumerator = errorLocations.GetEnumerator()
						While enumerator.MoveNext()
							Me.ProcessBadNameInCrefAttribute(xmlCrefAttributeSyntax, enumerator.Current, errid.Value)
						End While
						Return
					End If
					If (errorLocations.Length = 0 AndAlso useSiteInfo.Diagnostics IsNot Nothing) Then
						Me._diagnostics.AddDiagnostics(node, useSiteInfo)
						Return
					End If
					If (useSiteInfo.Diagnostics IsNot Nothing) Then
						Dim enumerator1 As ImmutableArray(Of Location).Enumerator = errorLocations.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As Location = enumerator1.Current
							Me._diagnostics.AddDiagnostics(current, useSiteInfo)
						End While
					End If
				End Sub

				Public Overrides Sub VisitToken(ByVal token As SyntaxToken)
					If (Me._writer IsNot Nothing) Then
						token.WriteTo(Me._writer)
					End If
					MyBase.VisitToken(token)
				End Sub

				Public Overrides Sub VisitXmlElement(ByVal node As XmlElementSyntax)
					Me.CaptureWellKnownTagNode(node, node.StartTag.Name)
					MyBase.VisitXmlElement(node)
				End Sub

				Public Overrides Sub VisitXmlEmptyElement(ByVal node As XmlEmptyElementSyntax)
					Me.CaptureWellKnownTagNode(node, node.Name)
					MyBase.VisitXmlEmptyElement(node)
				End Sub

				Private Sub WriteHeaderAndVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal trivia As DocumentationCommentTriviaSyntax)
					Me._writer.Write("<member name=""")
					Me._writer.Write(symbol.GetDocumentationCommentId())
					Me._writer.WriteLine(""">")
					Me.Visit(trivia)
					Me._writer.WriteLine("</member>")
				End Sub
			End Class

			Private Structure DocWriter
				Private ReadOnly _writer As TextWriter

				Private _indentDepth As Integer

				Private _temporaryStringBuilders As Stack(Of VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.TemporaryStringBuilder)

				Public ReadOnly Property IndentDepth As Integer
					Get
						Return Me._indentDepth
					End Get
				End Property

				Public ReadOnly Property IsSpecified As Boolean
					Get
						Return Me._writer IsNot Nothing
					End Get
				End Property

				Public Sub New(ByVal writer As TextWriter)
					Me = New VisualBasicCompilation.DocumentationCommentCompiler.DocWriter() With
					{
						._writer = writer,
						._indentDepth = 0,
						._temporaryStringBuilders = Nothing
					}
				End Sub

				Public Sub BeginTemporaryString()
					If (Me._temporaryStringBuilders Is Nothing) Then
						Me._temporaryStringBuilders = New Stack(Of VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.TemporaryStringBuilder)()
					End If
					Me._temporaryStringBuilders.Push(New VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.TemporaryStringBuilder(Me._indentDepth))
				End Sub

				Public Function GetAndEndTemporaryString() As String
					Dim temporaryStringBuilder As VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.TemporaryStringBuilder = Me._temporaryStringBuilders.Pop()
					Me._indentDepth = temporaryStringBuilder.InitialIndentDepth
					Return temporaryStringBuilder.Pooled.ToStringAndFree()
				End Function

				Public Sub Indent()
				End Sub

				Private Shared Function MakeIndent(ByVal depth As Integer) As String
					Dim str As String
					Select Case depth
						Case 0
							str = ""
							Exit Select
						Case 1
							str = "    "
							Exit Select
						Case 2
							str = "        "
							Exit Select
						Case 3
							str = "            "
							Exit Select
						Case Else
							str = New [String](Strings.ChrW(32), depth * 4)
							Exit Select
					End Select
					Return str
				End Function

				Public Sub Unindent()
				End Sub

				Public Sub Write(ByVal message As String)
					If (Me.IsSpecified) Then
						If (Me._temporaryStringBuilders IsNot Nothing AndAlso Me._temporaryStringBuilders.Count > 0) Then
							Dim builder As StringBuilder = Me._temporaryStringBuilders.Peek().Pooled.Builder
							builder.Append(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me._indentDepth))
							builder.Append(message)
							Return
						End If
						If (Me._writer IsNot Nothing) Then
							Me._writer.Write(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me._indentDepth))
							Me._writer.Write(message)
						End If
					End If
				End Sub

				Public Sub WriteLine(ByVal message As String)
					If (Me.IsSpecified) Then
						If (Me._temporaryStringBuilders IsNot Nothing AndAlso Me._temporaryStringBuilders.Count > 0) Then
							Dim builder As StringBuilder = Me._temporaryStringBuilders.Peek().Pooled.Builder
							builder.Append(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me._indentDepth))
							builder.AppendLine(message)
							Return
						End If
						If (Me._writer IsNot Nothing) Then
							Me._writer.Write(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me._indentDepth))
							Me._writer.WriteLine(message)
						End If
					End If
				End Sub

				Public Sub WriteSubString(ByVal message As String, ByVal start As Integer, ByVal length As Integer, Optional ByVal appendNewLine As Boolean = True)
					If (Me._temporaryStringBuilders IsNot Nothing AndAlso Me._temporaryStringBuilders.Count > 0) Then
						Dim builder As StringBuilder = Me._temporaryStringBuilders.Peek().Pooled.Builder
						builder.Append(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me.IndentDepth))
						builder.Append(message, start, length)
						If (appendNewLine) Then
							builder.AppendLine()
							Return
						End If
					ElseIf (Me._writer IsNot Nothing) Then
						Me._writer.Write(VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.MakeIndent(Me.IndentDepth))
						Dim num As Integer = length - 1
						Dim num1 As Integer = 0
						Do
							Me._writer.Write(message(start + num1))
							num1 = num1 + 1
						Loop While num1 <= num
						If (appendNewLine) Then
							Me._writer.WriteLine()
						End If
					End If
				End Sub

				Private Structure TemporaryStringBuilder
					Public ReadOnly Pooled As PooledStringBuilder

					Public ReadOnly InitialIndentDepth As Integer

					Public Sub New(ByVal indentDepth As Integer)
						Me = New VisualBasicCompilation.DocumentationCommentCompiler.DocWriter.TemporaryStringBuilder() With
						{
							.InitialIndentDepth = indentDepth,
							.Pooled = PooledStringBuilder.GetInstance()
						}
					End Sub
				End Structure
			End Structure

			Private Class IncludeElementExpander
				Private ReadOnly _symbol As Symbol

				Private ReadOnly _tagsSupport As VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.WellKnownTagsSupport

				Private ReadOnly _sourceIncludeElementNodes As ArrayBuilder(Of XmlNodeSyntax)

				Private ReadOnly _compilation As VisualBasicCompilation

				Private ReadOnly _tree As SyntaxTree

				Private ReadOnly _onlyDiagnosticsFromTree As SyntaxTree

				Private ReadOnly _filterSpanWithinTree As Nullable(Of TextSpan)

				Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

				Private ReadOnly _cancellationToken As CancellationToken

				Private _binders As Dictionary(Of DocumentationCommentBinder.BinderType, Microsoft.CodeAnalysis.VisualBasic.Binder)

				Private _nextSourceIncludeElementIndex As Integer

				Private _inProgressIncludeElementNodes As HashSet(Of Location)

				Private _includedFileCache As DocumentationCommentIncludeCache

				Private ReadOnly Property [Module] As SourceModuleSymbol
					Get
						Return DirectCast(Me._compilation.SourceModule, SourceModuleSymbol)
					End Get
				End Property

				Private ReadOnly Property ProduceDiagnostics As Boolean
					Get
						Return Me._tree.ReportDocumentationCommentDiagnostics()
					End Get
				End Property

				Private ReadOnly Property ProduceXmlDiagnostics As Boolean
					Get
						If (Not Me._tree.ReportDocumentationCommentDiagnostics()) Then
							Return False
						End If
						Return Me._onlyDiagnosticsFromTree Is Nothing
					End Get
				End Property

				Private Sub New(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal sourceIncludeElementNodes As ArrayBuilder(Of XmlNodeSyntax), ByVal compilation As VisualBasicCompilation, ByVal includedFileCache As DocumentationCommentIncludeCache, ByVal onlyDiagnosticsFromTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
					MyBase.New()
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
					Me._binders = Nothing
					Me._symbol = symbol
					Me._tagsSupport = New VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.WellKnownTagsSupport(symbol)
					Me._sourceIncludeElementNodes = sourceIncludeElementNodes
					Me._compilation = compilation
					Me._onlyDiagnosticsFromTree = onlyDiagnosticsFromTree
					Me._filterSpanWithinTree = filterSpanWithinTree
					Me._diagnostics = diagnostics
					Me._cancellationToken = cancellationToken
					If (sourceIncludeElementNodes Is Nothing OrElse sourceIncludeElementNodes.Count = 0) Then
						syntaxTree = Nothing
					Else
						syntaxTree = sourceIncludeElementNodes(0).SyntaxTree
					End If
					Me._tree = syntaxTree
					Me._includedFileCache = includedFileCache
					Me._nextSourceIncludeElementIndex = 0
				End Sub

				Private Function AddIncludeElementLocation(ByVal location As Microsoft.CodeAnalysis.Location) As Boolean
					If (Me._inProgressIncludeElementNodes Is Nothing) Then
						Me._inProgressIncludeElementNodes = New HashSet(Of Microsoft.CodeAnalysis.Location)()
					End If
					Return Me._inProgressIncludeElementNodes.Add(location)
				End Function

				Private Shared Function AttributeNameIs(ByVal attribute As XAttribute, ByVal name As String) As Boolean
					If (Not [String].IsNullOrEmpty(attribute.Name.NamespaceName)) Then
						Return False
					End If
					Return DocumentationCommentXmlNames.AttributeEquals(attribute.Name.LocalName, name)
				End Function

				Private Sub BindAndReplaceCref(ByVal attribute As XAttribute, ByVal currentXmlFilePath As String)
					Dim location As Func(Of Diagnostic, Microsoft.CodeAnalysis.Location)
					Dim func As Func(Of Microsoft.CodeAnalysis.Location, Boolean)
					Dim baseXmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax = SyntaxFactory.ParseDocCommentAttributeAsStandAloneEntity(attribute.ToString(), "")
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = baseXmlAttributeSyntax.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
						Dim str As String = attribute.Value.Trim()
						If (str.Length >= 2 AndAlso str(0) <> ":"C AndAlso str(1) = ":"C) Then
							Return
						End If
						If (Me.ProduceXmlDiagnostics) Then
							Me._diagnostics.Add(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1, Microsoft.CodeAnalysis.XmlLocation.Create(attribute, currentXmlFilePath), New [Object]() { str })
						End If
						attribute.Value = [String].Concat("?:", str)
						Return
					End If
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) Then
						Throw ExceptionUtilities.UnexpectedValue(baseXmlAttributeSyntax.Kind())
					End If
					Dim orCreateBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetOrCreateBinder(DocumentationCommentBinder.BinderType.Cref)
					Dim reference As CrefReferenceSyntax = DirectCast(baseXmlAttributeSyntax, XmlCrefAttributeSyntax).Reference
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = orCreateBinder.GetNewCompoundUseSiteInfo(Me._diagnostics)
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(Me._diagnostics)
					Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = orCreateBinder.BindInsideCrefAttributeValue(reference, False, instance, newCompoundUseSiteInfo)
					Me._diagnostics.AddDependencies(instance, False)
					Me._diagnostics.AddDependencies(newCompoundUseSiteInfo)
					Dim [readOnly] As ImmutableArray(Of Diagnostic) = instance.DiagnosticBag.ToReadOnly()
					If (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-0 Is Nothing) Then
						location = Function(x As Diagnostic) x.Location
						VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-0 = location
					Else
						location = VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-0
					End If
					Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Diagnostic, Microsoft.CodeAnalysis.Location)([readOnly], location)
					If (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-1 Is Nothing) Then
						func = Function(x As Microsoft.CodeAnalysis.Location) CObj(x) <> CObj(Nothing)
						VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-1 = func
					Else
						func = VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander._Closure$__.$I34-1
					End If
					Dim locations1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = locations.WhereAsArray(func)
					instance.Free()
					If (Me.ProduceXmlDiagnostics AndAlso Not newCompoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
						Dim nullable As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID) = Nothing
						Me.ProcessErrorLocations(Microsoft.CodeAnalysis.XmlLocation.Create(attribute, currentXmlFilePath), Nothing, newCompoundUseSiteInfo, locations1, nullable)
					End If
					If (symbols.IsDefaultOrEmpty) Then
						If (Me.ProduceXmlDiagnostics) Then
							Me.ProcessErrorLocations(Microsoft.CodeAnalysis.XmlLocation.Create(attribute, currentXmlFilePath), reference.ToFullString().TrimEnd(New [Char](-1) {}), newCompoundUseSiteInfo, locations1, New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID)(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1))
						End If
						attribute.Value = [String].Concat("?:", attribute.Value)
						Return
					End If
					Dim str1 As String = Nothing
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
					Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
						If (current.Kind <> SymbolKind.TypeParameter) Then
							Dim documentationCommentId As String = current.OriginalDefinition.GetDocumentationCommentId()
							If (documentationCommentId Is Nothing OrElse str1 IsNot Nothing AndAlso Me._compilation.CompareSourceLocations(symbol.Locations(0), current.Locations(0)) <= 0) Then
								Continue While
							End If
							str1 = documentationCommentId
							symbol = current
						Else
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefToTypeParameter
						End If
					End While
					If (str1 IsNot Nothing) Then
						attribute.Value = str1
						Me._diagnostics.AddAssembliesUsedByCrefTarget(symbol.OriginalDefinition)
						Return
					End If
					If (Me.ProduceXmlDiagnostics) Then
						Dim xmlLocation As Microsoft.CodeAnalysis.XmlLocation = Microsoft.CodeAnalysis.XmlLocation.Create(attribute, currentXmlFilePath)
						Dim str2 As String = reference.ToString()
						Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)()
						Me.ProcessErrorLocations(xmlLocation, str2, compoundUseSiteInfo, locations1, New Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID)(eRRID))
					End If
					attribute.Value = [String].Concat("?:", attribute.Value)
				End Sub

				Private Function BindName(ByVal attribute As XAttribute, ByVal elementName As String, ByVal type As DocumentationCommentBinder.BinderType, ByVal badNameValueError As ERRID, ByVal currentXmlFilePath As String) As String
					Dim str As String = Nothing
					Dim str1 As String = attribute.ToString()
					Dim str2 As String = attribute.Value.Trim()
					Dim baseXmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax = SyntaxFactory.ParseDocCommentAttributeAsStandAloneEntity(str1, elementName)
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = baseXmlAttributeSyntax.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
						str = Me.GenerateDiagnostic(XmlLocation.Create(attribute, currentXmlFilePath), badNameValueError, New [Object]() { str2, Me._tagsSupport.SymbolName })
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameAttribute) Then
							Throw ExceptionUtilities.UnexpectedValue(baseXmlAttributeSyntax.Kind())
						End If
						Dim orCreateBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetOrCreateBinder(type)
						Dim reference As IdentifierNameSyntax = DirectCast(baseXmlAttributeSyntax, XmlNameAttributeSyntax).Reference
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = orCreateBinder.GetNewCompoundUseSiteInfo(Me._diagnostics)
						Dim symbols As ImmutableArray(Of Symbol) = orCreateBinder.BindXmlNameAttributeValue(reference, newCompoundUseSiteInfo)
						Me._diagnostics.AddDependencies(newCompoundUseSiteInfo)
						If (Me.ProduceDiagnostics AndAlso Not newCompoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
							Dim location As Microsoft.CodeAnalysis.Location = XmlLocation.Create(attribute, currentXmlFilePath)
							If (Me.ShouldProcessLocation(location)) Then
								Me._diagnostics.AddDiagnostics(location, newCompoundUseSiteInfo)
							End If
						End If
						If (symbols.IsDefaultOrEmpty) Then
							str = Me.GenerateDiagnostic(XmlLocation.Create(attribute, currentXmlFilePath), badNameValueError, New [Object]() { str2, Me._tagsSupport.SymbolName })
						End If
					End If
					Return str
				End Function

				Private Shared Function ElementNameIs(ByVal element As XElement, ByVal name As String) As Boolean
					If (Not [String].IsNullOrEmpty(element.Name.NamespaceName)) Then
						Return False
					End If
					Return DocumentationCommentXmlNames.ElementEquals(element.Name.LocalName, name, True)
				End Function

				Private Function GenerateDiagnostic(ByVal suppressDiagnostic As Boolean, ByVal loc As Location, ByVal id As ERRID, ByVal ParamArray arguments As Object()) As String
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(id, arguments)
					If (Not suppressDiagnostic AndAlso Me.ProduceDiagnostics AndAlso Me.ShouldProcessLocation(loc)) Then
						Me._diagnostics.Add(New VBDiagnostic(diagnosticInfo, loc, False))
					End If
					Return diagnosticInfo.ToString()
				End Function

				Private Function GenerateDiagnostic(ByVal loc As Location, ByVal id As ERRID, ByVal ParamArray arguments As Object()) As String
					Return Me.GenerateDiagnostic(False, loc, id, arguments)
				End Function

				Private Function GetIncludeElementLocation(ByVal includeElement As XElement, ByRef currentXmlFilePath As String, ByRef originatingSyntax As XmlNodeSyntax) As Microsoft.CodeAnalysis.Location
					Dim location As Microsoft.CodeAnalysis.Location
					Dim location1 As Microsoft.CodeAnalysis.Location = includeElement.Annotation(Of Microsoft.CodeAnalysis.Location)()
					If (location1 Is Nothing) Then
						If (currentXmlFilePath IsNot Nothing) Then
							location1 = XmlLocation.Create(includeElement, currentXmlFilePath)
						Else
							originatingSyntax = Me._sourceIncludeElementNodes(Me._nextSourceIncludeElementIndex)
							location1 = originatingSyntax.GetLocation()
							Me._nextSourceIncludeElementIndex = Me._nextSourceIncludeElementIndex + 1
							includeElement.AddAnnotation(location1)
							currentXmlFilePath = location1.GetLineSpan().Path
						End If
						location = location1
					Else
						location = location1
					End If
					Return location
				End Function

				Private Function GetOrCreateBinder(ByVal type As DocumentationCommentBinder.BinderType) As Microsoft.CodeAnalysis.VisualBasic.Binder
					If (Me._binders Is Nothing) Then
						Me._binders = New Dictionary(Of DocumentationCommentBinder.BinderType, Microsoft.CodeAnalysis.VisualBasic.Binder)()
					End If
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
					If (Not Me._binders.TryGetValue(type, binder)) Then
						binder = VisualBasicCompilation.DocumentationCommentCompiler.CreateDocumentationCommentBinderForSymbol(Me.[Module], Me._symbol, Me._tree, type)
						Me._binders.Add(type, binder)
					End If
					Return binder
				End Function

				Private Sub ProcessErrorLocations(ByVal currentXmlLocation As XmlLocation, ByVal referenceName As String, ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal errorLocations As ImmutableArray(Of Microsoft.CodeAnalysis.Location), ByVal errid As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ERRID))
					If (errorLocations.Length <> 0) Then
						If (errid.HasValue) Then
							Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = errorLocations.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As Microsoft.CodeAnalysis.Location = enumerator.Current
								Me._diagnostics.Add(errid.Value, current, New [Object]() { referenceName })
							End While
							Return
						End If
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = errorLocations.GetEnumerator()
						While enumerator1.MoveNext()
							Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
							Me._diagnostics.AddDiagnostics(location, useSiteInfo)
						End While
					Else
						If (useSiteInfo.Diagnostics IsNot Nothing) Then
							Me._diagnostics.AddDiagnostics(currentXmlLocation, useSiteInfo)
							Return
						End If
						If (errid.HasValue) Then
							Me._diagnostics.Add(errid.Value, currentXmlLocation, New [Object]() { referenceName })
							Return
						End If
					End If
				End Sub

				Friend Shared Function ProcessIncludes(ByVal unprocessed As String, ByVal memberSymbol As Symbol, ByVal sourceIncludeElementNodes As ArrayBuilder(Of XmlNodeSyntax), ByVal compilation As VisualBasicCompilation, ByVal onlyDiagnosticsFromTree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByRef includedFileCache As DocumentationCommentIncludeCache, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken) As String
					Dim stringAndFree As String
					Dim xDocument As System.Xml.Linq.XDocument
					If (sourceIncludeElementNodes IsNot Nothing) Then
						Try
							xDocument = System.Xml.Linq.XDocument.Parse(unprocessed, LoadOptions.PreserveWhitespace)
						Catch xmlException As System.Xml.XmlException
							ProjectData.SetProjectError(xmlException)
							stringAndFree = unprocessed
							ProjectData.ClearProjectError()
							Return stringAndFree
						End Try
						Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
						Using stringWriter As System.IO.StringWriter = New System.IO.StringWriter(instance.Builder, CultureInfo.InvariantCulture)
							cancellationToken.ThrowIfCancellationRequested()
							Dim includeElementExpander As VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander = New VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander(memberSymbol, sourceIncludeElementNodes, compilation, includedFileCache, onlyDiagnosticsFromTree, filterSpanWithinTree, diagnostics, cancellationToken)
							Dim xNodeArray As System.Xml.Linq.XNode() = includeElementExpander.Rewrite(xDocument, Nothing, Nothing)
							Dim num As Integer = 0
							Do
								Dim xNode As System.Xml.Linq.XNode = xNodeArray(num)
								cancellationToken.ThrowIfCancellationRequested()
								stringWriter.Write(xNode)
								num = num + 1
							Loop While num < CInt(xNodeArray.Length)
							includedFileCache = includeElementExpander._includedFileCache
						End Using
						stringAndFree = instance.ToStringAndFree()
					Else
						stringAndFree = unprocessed
					End If
					Return stringAndFree
				End Function

				Private Function RemoveIncludeElementLocation(ByVal location As Microsoft.CodeAnalysis.Location) As Boolean
					Return Me._inProgressIncludeElementNodes.Remove(location)
				End Function

				Private Function Rewrite(ByVal node As XNode, ByVal currentXmlFilePath As String, ByVal originatingSyntax As XmlNodeSyntax) As XNode()
					Dim xNodeArray As XNode()
					Dim enumerator As IEnumerator(Of System.Xml.Linq.XAttribute) = Nothing
					Me._cancellationToken.ThrowIfCancellationRequested()
					Dim str As String = Nothing
					If (node.NodeType = XmlNodeType.Element) Then
						Dim xElement As System.Xml.Linq.XElement = DirectCast(node, System.Xml.Linq.XElement)
						If (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement, "include")) Then
							Dim xNodeArray1 As XNode() = Me.RewriteIncludeElement(xElement, currentXmlFilePath, originatingSyntax, str)
							If (xNodeArray1 Is Nothing) Then
								GoTo Label1
							End If
							xNodeArray = xNodeArray1
							Return xNodeArray
						End If
					End If
				Label1:
					Dim xContainer As System.Xml.Linq.XContainer = TryCast(node, System.Xml.Linq.XContainer)
					If (xContainer IsNot Nothing) Then
						Dim xNodes As IEnumerable(Of XNode) = xContainer.Nodes()
						xContainer = xContainer.Copy(True)
						If (xNodes IsNot Nothing) Then
							Dim xNodeArray2 As XNode() = Me.RewriteMany(xNodes.ToArray(), currentXmlFilePath, originatingSyntax)
							xContainer.ReplaceNodes(xNodeArray2)
						End If
						If (xContainer.NodeType = XmlNodeType.Element AndAlso originatingSyntax IsNot Nothing) Then
							Dim xElement1 As System.Xml.Linq.XElement = DirectCast(xContainer, System.Xml.Linq.XElement)
							Dim name As XName = xElement1.Name
							Dim binderType As DocumentationCommentBinder.BinderType = DocumentationCommentBinder.BinderType.None
							Dim flag As Boolean = False
							If (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "exception")) Then
								If (Me._tagsSupport.ExceptionSupported) Then
									flag = True
								Else
									str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName })
								End If
							ElseIf (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "returns")) Then
								If (Not Me._tagsSupport.ReturnsSupported) Then
									If (Not Me._tagsSupport.IsDeclareMethod) Then
										str = If(Not Me._tagsSupport.IsWriteOnlyProperty, Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName }), Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty, New [Object](-1) {}))
									Else
										str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocReturnsOnADeclareSub, New [Object](-1) {})
									End If
								End If
							ElseIf (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "param") OrElse VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "paramref")) Then
								binderType = DocumentationCommentBinder.BinderType.NameInParamOrParamRef
								If (Not Me._tagsSupport.ParamAndParamRefSupported) Then
									str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName })
								End If
							ElseIf (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "value")) Then
								If (Not Me._tagsSupport.ValueSupported) Then
									str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName })
								End If
							ElseIf (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "typeparam")) Then
								binderType = DocumentationCommentBinder.BinderType.NameInTypeParam
								If (Not Me._tagsSupport.TypeParamSupported) Then
									str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName })
								End If
							ElseIf (VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.ElementNameIs(xElement1, "typeparamref")) Then
								binderType = DocumentationCommentBinder.BinderType.NameInTypeParamRef
								If (Not Me._tagsSupport.TypeParamRefSupported) Then
									str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, New [Object]() { name.LocalName, Me._tagsSupport.SymbolName })
								End If
							End If
							If (str Is Nothing) Then
								Dim xAttribute As System.Xml.Linq.XAttribute = Nothing
								Using flag1 As Boolean = False
									enumerator = xElement1.Attributes().GetEnumerator()
									While enumerator.MoveNext()
										Dim current As System.Xml.Linq.XAttribute = enumerator.Current
										If (Not VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.AttributeNameIs(current, "cref")) Then
											If (Not VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.AttributeNameIs(current, "name")) Then
												Continue While
											End If
											xAttribute = current
										Else
											Me.BindAndReplaceCref(current, currentXmlFilePath)
											flag1 = True
										End If
									End While
								End Using
								If (flag) Then
									If (Not flag1) Then
										str = Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), ERRID.WRN_XMLDocExceptionTagWithoutCRef, New [Object](-1) {})
									End If
								ElseIf (binderType <> DocumentationCommentBinder.BinderType.None) Then
									str = If(xAttribute IsNot Nothing, Me.BindName(xAttribute, name.LocalName, binderType, If(binderType = DocumentationCommentBinder.BinderType.NameInParamOrParamRef, ERRID.WRN_XMLDocBadParamTag2, ERRID.WRN_XMLDocBadGenericParamTag2), currentXmlFilePath), Me.GenerateDiagnostic(XmlLocation.Create(xElement1, currentXmlFilePath), If(binderType = DocumentationCommentBinder.BinderType.NameInParamOrParamRef, ERRID.WRN_XMLDocParamTagWithoutName, ERRID.WRN_XMLDocGenericParamTagWithoutName), New [Object](-1) {}))
								End If
							End If
						End If
						xNodeArray = If(str IsNot Nothing, New XNode() { New XComment(str), xContainer }, New XNode() { xContainer })
					Else
						xNodeArray = New XNode() { node.Copy(True) }
					End If
					Return xNodeArray
				End Function

				Private Function RewriteIncludeElement(ByVal includeElement As XElement, ByVal currentXmlFilePath As String, ByVal originatingSyntax As XmlNodeSyntax, <Out> ByRef commentMessage As String) As XNode()
					Dim xComment As XNode()
					Dim orMakeDocument As XDocument
					Dim includeElementLocation As Location = Me.GetIncludeElementLocation(includeElement, currentXmlFilePath, originatingSyntax)
					If (Me.AddIncludeElementLocation(includeElementLocation)) Then
						Try
							Dim xAttribute As System.Xml.Linq.XAttribute = includeElement.Attribute(XName.[Get]("file"))
							Dim xAttribute1 As System.Xml.Linq.XAttribute = includeElement.Attribute(XName.[Get]("path"))
							Dim flag As Boolean = xAttribute IsNot Nothing
							Dim flag1 As Boolean = xAttribute1 IsNot Nothing
							If (Not flag OrElse Not flag1) Then
								If (Not flag) Then
									commentMessage = Me.GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLMissingFileOrPathAttribute1, New [Object]() { "file" })
								End If
								If (Not flag1) Then
									commentMessage = [String].Concat(If(commentMessage Is Nothing, "", [String].Concat(commentMessage, " ")), Me.GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLMissingFileOrPathAttribute1, New [Object]() { "path" }))
								End If
								xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
							Else
								Dim value As String = xAttribute1.Value
								Dim str As String = xAttribute.Value
								Dim xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver = Me._compilation.Options.XmlReferenceResolver
								If (xmlReferenceResolver IsNot Nothing) Then
									Dim str1 As String = xmlReferenceResolver.ResolveReference(str, currentXmlFilePath)
									If (str1 IsNot Nothing) Then
										If (Me._includedFileCache Is Nothing) Then
											Me._includedFileCache = New DocumentationCommentIncludeCache(Me._compilation.Options.XmlReferenceResolver)
										End If
										Try
											Try
												orMakeDocument = Me._includedFileCache.GetOrMakeDocument(str1)
											Catch oException1 As IOException
												ProjectData.SetProjectError(oException1)
												Dim oException As IOException = oException1
												commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, New [Object]() { str, value, oException.Message })
												xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
												ProjectData.ClearProjectError()
												Return xComment
											End Try
											Dim str2 As String = Nothing
											Dim flag2 As Boolean = False
											Dim xElementArray As XElement() = XmlUtilities.TrySelectElements(orMakeDocument, value, str2, flag2)
											If (xElementArray IsNot Nothing) Then
												If (xElementArray IsNot Nothing AndAlso CInt(xElementArray.Length) > 0) Then
													Dim xNodeArray As XNode() = Me.RewriteMany(xElementArray, str1, originatingSyntax)
													If (CInt(xNodeArray.Length) > 0) Then
														commentMessage = Nothing
														xComment = xNodeArray
														Return xComment
													End If
												End If
												commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, New [Object]() { value, str })
												xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
											Else
												commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, New [Object]() { value, str })
												xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
											End If
										Catch xmlException As System.Xml.XmlException
											ProjectData.SetProjectError(xmlException)
											commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, New [Object]() { value, str })
											xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
											ProjectData.ClearProjectError()
										End Try
									Else
										commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, New [Object]() { str, value, New CodeAnalysisResourcesLocalizableErrorArgument("FileNotFound") })
										xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
									End If
								Else
									commentMessage = Me.GenerateDiagnostic(True, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, New [Object]() { str, value, New CodeAnalysisResourcesLocalizableErrorArgument("XmlReferencesNotSupported") })
									xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
								End If
							End If
						Finally
							Me.RemoveIncludeElementLocation(includeElementLocation)
						End Try
					Else
						Dim xAttribute2 As System.Xml.Linq.XAttribute = includeElement.Attribute(XName.[Get]("file"))
						Dim xAttribute3 As System.Xml.Linq.XAttribute = includeElement.Attribute(XName.[Get]("path"))
						commentMessage = Me.GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, New [Object]() { xAttribute2.Value, xAttribute3.Value })
						xComment = New XNode() { New System.Xml.Linq.XComment(commentMessage) }
					End If
					Return xComment
				End Function

				Private Function RewriteMany(ByVal nodes As System.Xml.Linq.XNode(), ByVal currentXmlFilePath As String, ByVal originatingSyntax As XmlNodeSyntax) As System.Xml.Linq.XNode()
					Dim instance As ArrayBuilder(Of System.Xml.Linq.XNode) = Nothing
					Dim xNodeArray As System.Xml.Linq.XNode() = nodes
					Dim num As Integer = 0
					Do
						Dim xNode As System.Xml.Linq.XNode = xNodeArray(num)
						If (instance Is Nothing) Then
							instance = ArrayBuilder(Of System.Xml.Linq.XNode).GetInstance()
						End If
						instance.AddRange(Me.Rewrite(xNode, currentXmlFilePath, originatingSyntax))
						num = num + 1
					Loop While num < CInt(xNodeArray.Length)
					If (instance Is Nothing) Then
						Return Array.Empty(Of System.Xml.Linq.XNode)()
					End If
					Return instance.ToArrayAndFree()
				End Function

				Private Function ShouldProcessLocation(ByVal loc As Location) As Boolean
					If (Me._onlyDiagnosticsFromTree Is Nothing) Then
						Return True
					End If
					If (loc.Kind <> LocationKind.SourceFile OrElse DirectCast(loc, SourceLocation).SourceTree <> Me._onlyDiagnosticsFromTree) Then
						Return False
					End If
					If (Not Me._filterSpanWithinTree.HasValue) Then
						Return True
					End If
					Return Me._filterSpanWithinTree.Value.Contains(loc.SourceSpan)
				End Function

				Private Structure WellKnownTagsSupport
					Public ReadOnly ExceptionSupported As Boolean

					Public ReadOnly ReturnsSupported As Boolean

					Public ReadOnly ParamAndParamRefSupported As Boolean

					Public ReadOnly ValueSupported As Boolean

					Public ReadOnly TypeParamSupported As Boolean

					Public ReadOnly TypeParamRefSupported As Boolean

					Public ReadOnly IsDeclareMethod As Boolean

					Public ReadOnly IsWriteOnlyProperty As Boolean

					Public ReadOnly SymbolName As String

					Public Sub New(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
						Me = New VisualBasicCompilation.DocumentationCommentCompiler.IncludeElementExpander.WellKnownTagsSupport() With
						{
							.ExceptionSupported = False,
							.ReturnsSupported = False,
							.ParamAndParamRefSupported = False,
							.ValueSupported = False,
							.TypeParamSupported = False,
							.TypeParamRefSupported = False,
							.IsDeclareMethod = False,
							.IsWriteOnlyProperty = False,
							.SymbolName = VisualBasicCompilation.DocumentationCommentCompiler.GetSymbolName(symbol)
						}
						Dim kind As SymbolKind = symbol.Kind
						Select Case kind
							Case SymbolKind.[Event]
								Me.ExceptionSupported = True
								Me.ParamAndParamRefSupported = True
								Me.TypeParamRefSupported = True
								Return
							Case SymbolKind.Field
								Me.TypeParamRefSupported = True
								Return
							Case SymbolKind.Label
							Case SymbolKind.Local
							Case SymbolKind.NetModule
								Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
							Case SymbolKind.Method
								Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								Me.IsDeclareMethod = methodSymbol.MethodKind = MethodKind.DeclareMethod
								Me.ExceptionSupported = True
								Me.ParamAndParamRefSupported = True
								Me.TypeParamSupported = If(Me.IsDeclareMethod, False, methodSymbol.MethodKind <> MethodKind.UserDefinedOperator)
								Me.TypeParamRefSupported = True
								If (methodSymbol.IsSub) Then
									Return
								End If
								Me.ReturnsSupported = True
								Return
							Case SymbolKind.NamedType
								Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
								Dim delegateInvokeMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
								If (namedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
									If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.IsSub) Then
										Me.SymbolName = "delegate sub"
									Else
										Me.ReturnsSupported = True
									End If
								End If
								Me.ParamAndParamRefSupported = namedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]
								Me.TypeParamSupported = If(namedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum], False, namedTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Module])
								Me.TypeParamRefSupported = namedTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Module]
								Return
							Case Else
								If (kind = SymbolKind.[Property]) Then
									Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
									Me.ExceptionSupported = True
									Me.ParamAndParamRefSupported = True
									Me.TypeParamRefSupported = True
									Me.ValueSupported = True
									Me.IsWriteOnlyProperty = propertySymbol.IsWriteOnly
									Me.ReturnsSupported = Not Me.IsWriteOnlyProperty
									Return
								End If
								Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
						End Select
					End Sub
				End Structure
			End Class

			Private Class MislocatedDocumentationCommentFinder
				Inherits VisualBasicSyntaxWalker
				Private ReadOnly _diagnostics As DiagnosticBag

				Private ReadOnly _filterSpanWithinTree As Nullable(Of TextSpan)

				Private ReadOnly _cancellationToken As CancellationToken

				Private _isInsideMethodOrLambda As Boolean

				Private Sub New(ByVal diagnostics As DiagnosticBag, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal cancellationToken As System.Threading.CancellationToken)
					MyBase.New(SyntaxWalkerDepth.Trivia)
					Me._diagnostics = diagnostics
					Me._filterSpanWithinTree = filterSpanWithinTree
					Me._cancellationToken = cancellationToken
					Me._isInsideMethodOrLambda = False
				End Sub

				Public Overrides Sub DefaultVisit(ByVal node As SyntaxNode)
					If (node.HasStructuredTrivia AndAlso Not Me.IsSyntacticallyFilteredOut(node.FullSpan)) Then
						MyBase.DefaultVisit(node)
					End If
				End Sub

				Private Sub DefaultVisitChildrenStartingWith(ByVal node As Microsoft.CodeAnalysis.SyntaxNode, ByVal start As Integer)
					Dim childSyntaxLists As ChildSyntaxList = node.ChildNodesAndTokens()
					Dim count As Integer = childSyntaxLists.Count
					Dim num As Integer = start
					While num < count
						Dim item As SyntaxNodeOrToken = childSyntaxLists(num)
						num = num + 1
						Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = item.AsNode()
						If (syntaxNode Is Nothing) Then
							Me.VisitToken(item.AsToken())
						Else
							Me.Visit(syntaxNode)
						End If
					End While
				End Sub

				Private Function IsSyntacticallyFilteredOut(ByVal fullSpan As TextSpan) As Boolean
					If (Not Me._filterSpanWithinTree.HasValue) Then
						Return False
					End If
					Return Not Me._filterSpanWithinTree.Value.Contains(fullSpan)
				End Function

				Public Shared Sub ReportUnprocessed(ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal diagnostics As DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
					If (tree.ReportDocumentationCommentDiagnostics()) Then
						Dim mislocatedDocumentationCommentFinder As VisualBasicCompilation.DocumentationCommentCompiler.MislocatedDocumentationCommentFinder = New VisualBasicCompilation.DocumentationCommentCompiler.MislocatedDocumentationCommentFinder(diagnostics, filterSpanWithinTree, cancellationToken)
						mislocatedDocumentationCommentFinder.Visit(tree.GetRoot(cancellationToken))
					End If
				End Sub

				Public Overrides Sub VisitAccessorBlock(ByVal node As AccessorBlockSyntax)
					Me.VisitMethodBlockBase(node)
				End Sub

				Public Overrides Sub VisitConstructorBlock(ByVal node As ConstructorBlockSyntax)
					Me.VisitMethodBlockBase(node)
				End Sub

				Public Overrides Sub VisitMethodBlock(ByVal node As MethodBlockSyntax)
					Me.VisitMethodBlockBase(node)
				End Sub

				Private Sub VisitMethodBlockBase(ByVal node As MethodBlockBaseSyntax)
					Me._cancellationToken.ThrowIfCancellationRequested()
					If (Not Me.IsSyntacticallyFilteredOut(node.FullSpan)) Then
						Dim flag As Boolean = Me._isInsideMethodOrLambda
						Me._isInsideMethodOrLambda = False
						Me.Visit(node.BlockStatement)
						Me._isInsideMethodOrLambda = True
						Me.DefaultVisitChildrenStartingWith(node, 1)
						Me._isInsideMethodOrLambda = flag
					End If
				End Sub

				Public Overrides Sub VisitMultiLineLambdaExpression(ByVal node As MultiLineLambdaExpressionSyntax)
					Me._cancellationToken.ThrowIfCancellationRequested()
					If (Not Me.IsSyntacticallyFilteredOut(node.FullSpan)) Then
						Dim flag As Boolean = Me._isInsideMethodOrLambda
						Me._isInsideMethodOrLambda = True
						MyBase.VisitMultiLineLambdaExpression(node)
						Me._isInsideMethodOrLambda = flag
					End If
				End Sub

				Public Overrides Sub VisitOperatorBlock(ByVal node As OperatorBlockSyntax)
					Me.VisitMethodBlockBase(node)
				End Sub

				Public Overrides Sub VisitTrivia(ByVal trivia As SyntaxTrivia)
					If (Not Me.IsSyntacticallyFilteredOut(trivia.FullSpan)) Then
						If (trivia.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia) Then
							If (Not Me._isInsideMethodOrLambda) Then
								Dim parent As VisualBasicSyntaxNode = DirectCast(trivia.Token.Parent, VisualBasicSyntaxNode)
								While True
									Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
									If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement) Then
										If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
											GoTo Label0
										End If
										If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList) Then
											Exit While
										End If
										parent = parent.Parent
									Else
										If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement) <= 4 OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement)) Then
											GoTo Label0
										End If
										Exit While
									End If
								End While
								Me._diagnostics.Add(ERRID.WRN_XMLDocWithoutLanguageElement, trivia.GetLocation())
							Else
								Me._diagnostics.Add(ERRID.WRN_XMLDocInsideMethod, trivia.GetLocation())
							End If
						End If
					Label0:
						MyBase.VisitTrivia(trivia)
					End If
				End Sub
			End Class

			<Flags>
			Friend Enum WellKnownTag
				None = 0
				C = 1
				Code = 2
				Example = 4
				Exception = 8
				Include = 16
				List = 32
				Para = 64
				Param = 128
				ParamRef = 256
				Permission = 512
				Remarks = 1024
				Returns = 2048
				See = 4096
				SeeAlso = 8192
				Summary = 16384
				TypeParam = 32768
				TypeParamRef = 65536
				Value = 131072
				AllCollectable = 249752
			End Enum

			Private Structure XmlNodeWithAttributes
				Implements IComparable(Of VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes)
				Public ReadOnly Node As XmlNodeSyntax

				Public ReadOnly Attributes As SortedDictionary(Of String, String)

				Public Sub New(ByVal node As XmlNodeSyntax)
					Me = New VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes() With
					{
						.Node = node,
						.Attributes = VisualBasicCompilation.DocumentationCommentCompiler.GetElementAttributes(node)
					}
				End Sub

				Public Shared Function CompareAttributes(ByVal a As SortedDictionary(Of String, String), ByVal b As SortedDictionary(Of String, String)) As Integer
					Dim num As Integer
					Dim count As Integer = a.Count
					Dim num1 As Integer = count.CompareTo(b.Count)
					If (num1 = 0) Then
						If (count > 0) Then
							Dim enumerator As SortedDictionary(Of String, String).Enumerator = a.GetEnumerator()
							Dim enumerator1 As SortedDictionary(Of String, String).Enumerator = b.GetEnumerator()
							While enumerator.MoveNext() AndAlso enumerator1.MoveNext()
								Dim current As KeyValuePair(Of String, String) = enumerator.Current
								Dim key As String = current.Key
								current = enumerator1.Current
								num1 = key.CompareTo(current.Key)
								If (num1 = 0) Then
									current = enumerator.Current
									Dim value As String = current.Value
									current = enumerator1.Current
									num1 = value.CompareTo(current.Value)
									If (num1 = 0) Then
										Continue While
									End If
									num = num1
									Return num
								Else
									num = num1
									Return num
								End If
							End While
						End If
						num = 0
					Else
						num = num1
					End If
					Return num
				End Function

				Public Function CompareTo(ByVal other As VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes) As Integer Implements IComparable(Of VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes).CompareTo
					Dim num As Integer
					Dim num1 As Integer = VisualBasicCompilation.DocumentationCommentCompiler.XmlNodeWithAttributes.CompareAttributes(Me.Attributes, other.Attributes)
					If (num1 = 0) Then
						num = If(Me.Node.SpanStart > other.Node.SpanStart, 1, -1)
					Else
						num = num1
					End If
					Return num
				End Function
			End Structure
		End Class

		Private Structure EmbeddedTreeAndDeclaration
			Public ReadOnly Tree As Lazy(Of SyntaxTree)

			Public ReadOnly DeclarationEntry As DeclarationTableEntry

			Public Sub New(ByVal treeOpt As Func(Of SyntaxTree), ByVal rootNamespaceOpt As Func(Of RootSingleNamespaceDeclaration))
				Me = New VisualBasicCompilation.EmbeddedTreeAndDeclaration() With
				{
					.Tree = New Lazy(Of SyntaxTree)(treeOpt),
					.DeclarationEntry = New DeclarationTableEntry(New Lazy(Of RootSingleNamespaceDeclaration)(rootNamespaceOpt), True)
				}
			End Sub
		End Structure

		Friend Class EntryPoint
			Public ReadOnly MethodSymbol As MethodSymbol

			Public ReadOnly Diagnostics As ImmutableArray(Of Diagnostic)

			Public Sub New(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal diagnostics As ImmutableArray(Of Diagnostic))
				MyBase.New()
				Me.MethodSymbol = methodSymbol
				Me.Diagnostics = diagnostics
			End Sub
		End Class

		Private Structure ImportInfo
			Public ReadOnly Tree As SyntaxTree

			Public ReadOnly StatementSpan As TextSpan

			Public ReadOnly ClauseSpans As ImmutableArray(Of TextSpan)

			Public Sub New(ByVal syntax As ImportsStatementSyntax)
				Me = New VisualBasicCompilation.ImportInfo() With
				{
					.Tree = syntax.SyntaxTree,
					.StatementSpan = syntax.Span
				}
				Dim instance As ArrayBuilder(Of TextSpan) = ArrayBuilder(Of TextSpan).GetInstance()
				Dim enumerator As SeparatedSyntaxList(Of ImportsClauseSyntax).Enumerator = syntax.ImportsClauses.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current.Span)
				End While
				Me.ClauseSpans = instance.ToImmutableAndFree()
			End Sub
		End Structure

		Private Class NameSymbolSearcher
			Inherits VisualBasicCompilation.AbstractSymbolSearcher
			Private ReadOnly _name As String

			Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal filter As SymbolFilter, ByVal name As String, ByVal cancellationToken As System.Threading.CancellationToken)
				MyBase.New(compilation, filter, cancellationToken)
				Me._name = name
			End Sub

			Protected Overrides Function Matches(ByVal name As String) As Boolean
				Return CaseInsensitiveComparison.Equals(Me._name, name)
			End Function

			Protected Overrides Function ShouldCheckTypeForMembers(ByVal current As MergedTypeDeclaration) As Boolean
				Dim flag As Boolean
				Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = current.Declarations.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.MemberNames.Contains(Me._name)) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Function
		End Class

		Private Class PredicateSymbolSearcher
			Inherits VisualBasicCompilation.AbstractSymbolSearcher
			Private ReadOnly _predicate As Func(Of String, Boolean)

			Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal filter As SymbolFilter, ByVal predicate As Func(Of String, Boolean), ByVal cancellationToken As System.Threading.CancellationToken)
				MyBase.New(compilation, filter, cancellationToken)
				Me._predicate = predicate
			End Sub

			Protected Overrides Function Matches(ByVal name As String) As Boolean
				Return Me._predicate(name)
			End Function

			Protected Overrides Function ShouldCheckTypeForMembers(ByVal current As MergedTypeDeclaration) As Boolean
				Return True
			End Function
		End Class

		Friend NotInheritable Class ReferenceManager
			Inherits CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol)
			Protected Overrides ReadOnly Property MessageProvider As CommonMessageProvider
				Get
					Return Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				End Get
			End Property

			Public Sub New(ByVal simpleAssemblyName As String, ByVal identityComparer As AssemblyIdentityComparer, ByVal observedMetadata As Dictionary(Of MetadataReference, Object))
				MyBase.New(simpleAssemblyName, identityComparer, observedMetadata)
			End Sub

			Protected Overrides Function CheckPropertiesConsistency(ByVal primaryReference As MetadataReference, ByVal duplicateReference As MetadataReference, ByVal diagnostics As DiagnosticBag) As Boolean
				Return True
			End Function

			Friend Function CreateAndSetSourceAssemblyFullBind(ByVal compilation As VisualBasicCompilation) As Boolean
				Dim flag As Boolean
				Dim flag1 As Boolean
				Dim num As Integer
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim implicitReferenceResolutions As Object
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
				Dim referencesSupersedeLowerVersions As Boolean = compilation.Options.ReferencesSupersedeLowerVersions
				Dim pooledDictionary As PooledDictionary(Of String, List(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).ReferencedAssemblyIdentity)) = PooledDictionary(Of String, List(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).ReferencedAssemblyIdentity)).GetInstance()
				Try
					Dim valueTuples As IDictionary(Of ValueTuple(Of String, String), MetadataReference) = Nothing
					Dim metadataReferences As ImmutableArray(Of MetadataReference) = New ImmutableArray(Of MetadataReference)()
					Dim assemblyDatas As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData) = New ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData)()
					Dim pEModules As ImmutableArray(Of PEModule) = New ImmutableArray(Of PEModule)()
					Dim metadataReferences1 As ImmutableArray(Of MetadataReference) = New ImmutableArray(Of MetadataReference)()
					Dim resolvedReferences As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).ResolvedReference) = MyBase.ResolveMetadataReferences(compilation, pooledDictionary, metadataReferences1, valueTuples, metadataReferences, assemblyDatas, pEModules, diagnosticBag)
					Dim simpleAssemblyName As String = Me.SimpleAssemblyName
					Dim nums As ImmutableArray(Of Byte) = New ImmutableArray(Of Byte)()
					Dim assemblyDataForAssemblyBeingBuilt As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyDataForAssemblyBeingBuilt = New CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyDataForAssemblyBeingBuilt(New AssemblyIdentity(True, simpleAssemblyName, Nothing, Nothing, nums, False, False, AssemblyContentType.[Default]), assemblyDatas, pEModules)
					Dim assemblyDatas1 As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData) = assemblyDatas.Insert(0, assemblyDataForAssemblyBeingBuilt)
					Dim metadataReferences2 As ImmutableArray(Of MetadataReference) = New ImmutableArray(Of MetadataReference)()
					Dim resolvedReferences1 As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).ResolvedReference) = New ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).ResolvedReference)()
					Dim assemblyDatas2 As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData) = New ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData)()
					Dim scriptCompilationInfo As VisualBasicScriptCompilationInfo = compilation.ScriptCompilationInfo
					If (scriptCompilationInfo IsNot Nothing) Then
						Dim previousScriptCompilation As VisualBasicCompilation = scriptCompilationInfo.PreviousScriptCompilation
						If (previousScriptCompilation IsNot Nothing) Then
							implicitReferenceResolutions = previousScriptCompilation.GetBoundReferenceManager().ImplicitReferenceResolutions
						Else
							implicitReferenceResolutions = Nothing
						End If
					Else
						implicitReferenceResolutions = Nothing
					End If
					If (implicitReferenceResolutions Is Nothing) Then
						implicitReferenceResolutions = ImmutableDictionary(Of AssemblyIdentity, PortableExecutableReference).Empty
					End If
					Dim assemblyIdentities As ImmutableDictionary(Of AssemblyIdentity, PortableExecutableReference) = implicitReferenceResolutions
					Dim boundInputAssemblyArray As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).BoundInputAssembly() = MyBase.Bind(compilation, assemblyDatas1, pEModules, metadataReferences1, resolvedReferences, compilation.Options.MetadataReferenceResolver, compilation.Options.MetadataImportOptions, referencesSupersedeLowerVersions, pooledDictionary, assemblyDatas2, metadataReferences2, resolvedReferences1, assemblyIdentities, diagnosticBag, flag1, num)
					Dim metadataReferences3 As ImmutableArray(Of MetadataReference) = metadataReferences1.AddRange(metadataReferences2)
					resolvedReferences = resolvedReferences.AddRange(resolvedReferences1)
					Dim metadataReferences4 As Dictionary(Of MetadataReference, Integer) = Nothing
					Dim metadataReferences5 As Dictionary(Of MetadataReference, Integer) = Nothing
					Dim immutableArrays As ImmutableArray(Of ImmutableArray(Of String)) = New ImmutableArray(Of ImmutableArray(Of String))()
					Dim metadataReferences6 As Dictionary(Of MetadataReference, ImmutableArray(Of MetadataReference)) = Nothing
					CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).BuildReferencedAssembliesAndModulesMaps(boundInputAssemblyArray, metadataReferences3, resolvedReferences, pEModules.Length, assemblyDatas.Length, pooledDictionary, referencesSupersedeLowerVersions, metadataReferences4, metadataReferences5, immutableArrays, metadataReferences6)
					Dim nums1 As List(Of Integer) = New List(Of Integer)()
					Dim length As Integer = CInt(boundInputAssemblyArray.Length) - 1
					Dim num1 As Integer = 1
					Do
						If (boundInputAssemblyArray(num1).AssemblySymbol Is Nothing) Then
							boundInputAssemblyArray(num1).AssemblySymbol = DirectCast(assemblyDatas2(num1), VisualBasicCompilation.ReferenceManager.AssemblyDataForMetadataOrCompilation).CreateAssemblySymbol()
							nums1.Add(num1)
						End If
						num1 = num1 + 1
					Loop While num1 <= length
					Dim sourceAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol(compilation, Me.SimpleAssemblyName, compilation.MakeSourceModuleName(), pEModules)
					If (num = 0) Then
						instance = sourceAssemblySymbol
					ElseIf (num <= 0) Then
						instance = MissingCorLibrarySymbol.Instance
					Else
						instance = boundInputAssemblyArray(num).AssemblySymbol
					End If
					sourceAssemblySymbol.SetCorLibrary(instance)
					Dim assemblyIdentities1 As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol) = Nothing
					Dim length1 As Integer = assemblyDatas2.Length - 1
					Dim moduleReferences As ImmutableArray(Of ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)) = New ImmutableArray(Of ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol))()
					VisualBasicCompilation.ReferenceManager.SetupReferencesForSourceAssembly(sourceAssemblySymbol, pEModules, length1, boundInputAssemblyArray, assemblyIdentities1, moduleReferences)
					If (nums1.Count > 0) Then
						If (flag1) Then
							boundInputAssemblyArray(0).AssemblySymbol = sourceAssemblySymbol
						End If
						VisualBasicCompilation.ReferenceManager.InitializeNewSymbols(nums1, sourceAssemblySymbol, assemblyDatas2, boundInputAssemblyArray, assemblyIdentities1)
					End If
					If (compilation._lazyAssemblySymbol Is Nothing) Then
						SyncLock CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard
							If (compilation._lazyAssemblySymbol Is Nothing) Then
								If (Not MyBase.IsBound) Then
									Me.UpdateSymbolCacheNoLock(nums1, assemblyDatas2, boundInputAssemblyArray)
									Dim metadataReferences7 As Dictionary(Of MetadataReference, Integer) = metadataReferences4
									Dim metadataReferences8 As Dictionary(Of MetadataReference, Integer) = metadataReferences5
									Dim valueTuples1 As IDictionary(Of ValueTuple(Of String, String), MetadataReference) = valueTuples
									Dim metadataReferences9 As ImmutableArray(Of MetadataReference) = metadataReferences
									Dim metadataReferences10 As ImmutableArray(Of MetadataReference) = metadataReferences1
									Dim assemblyIdentities2 As ImmutableDictionary(Of AssemblyIdentity, PortableExecutableReference) = assemblyIdentities
									Dim flag2 As Boolean = flag1
									Dim [readOnly] As ImmutableArray(Of Diagnostic) = diagnosticBag.ToReadOnly()
									If (instance = sourceAssemblySymbol) Then
										assemblySymbol = Nothing
									Else
										assemblySymbol = instance
									End If
									MyBase.InitializeNoLock(metadataReferences7, metadataReferences8, valueTuples1, metadataReferences9, metadataReferences10, assemblyIdentities2, flag2, [readOnly], assemblySymbol, pEModules, moduleReferences, sourceAssemblySymbol.SourceModule.GetReferencedAssemblySymbols(), immutableArrays, sourceAssemblySymbol.SourceModule.GetUnifiedAssemblies(), metadataReferences6)
									compilation._referenceManager = Me
									compilation._lazyAssemblySymbol = sourceAssemblySymbol
								Else
									flag = False
									Return flag
								End If
							End If
						End SyncLock
					End If
					flag = True
				Finally
					diagnosticBag.Free()
					pooledDictionary.Free()
				End Try
				Return flag
			End Function

			Private Sub CreateAndSetSourceAssemblyReuseData(ByVal compilation As VisualBasicCompilation)
				Dim str As String = compilation.MakeSourceModuleName()
				Dim sourceAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol(compilation, Me.SimpleAssemblyName, str, MyBase.ReferencedModules)
				Me.InitializeAssemblyReuseData(sourceAssemblySymbol, MyBase.ReferencedAssemblies, MyBase.UnifiedAssemblies)
				If (compilation._lazyAssemblySymbol Is Nothing) Then
					SyncLock CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard
						If (compilation._lazyAssemblySymbol Is Nothing) Then
							compilation._lazyAssemblySymbol = sourceAssemblySymbol
						End If
					End SyncLock
				End If
			End Sub

			Protected Overrides Function CreateAssemblyDataForCompilation(ByVal compilationReference As Microsoft.CodeAnalysis.CompilationReference) As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyData
				Dim visualBasicCompilationReference As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationReference = TryCast(compilationReference, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationReference)
				If (visualBasicCompilationReference Is Nothing) Then
					Throw New NotSupportedException([String].Format(VBResources.CantReferenceCompilationFromTypes, compilationReference.[GetType](), "Visual Basic"))
				End If
				Return New VisualBasicCompilation.ReferenceManager.AssemblyDataForCompilation(visualBasicCompilationReference.Compilation, visualBasicCompilationReference.Properties.EmbedInteropTypes)
			End Function

			Protected Overrides Function CreateAssemblyDataForFile(ByVal assembly As PEAssembly, ByVal cachedSymbols As WeakList(Of IAssemblySymbolInternal), ByVal documentationProvider As Microsoft.CodeAnalysis.DocumentationProvider, ByVal sourceAssemblySimpleName As String, ByVal importOptions As MetadataImportOptions, ByVal embedInteropTypes As Boolean) As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyData
				Return New VisualBasicCompilation.ReferenceManager.AssemblyDataForFile(assembly, cachedSymbols, embedInteropTypes, documentationProvider, sourceAssemblySimpleName, importOptions)
			End Function

			Friend Function CreatePEAssemblyForAssemblyMetadata(ByVal metadata As AssemblyMetadata, ByVal importOptions As MetadataImportOptions, <Out> ByRef assemblyReferenceIdentityMap As ImmutableDictionary(Of AssemblyIdentity, AssemblyIdentity)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEAssemblySymbol
				Dim func As Func(Of UnifiedAssembly(Of AssemblySymbol), AssemblyIdentityMap(Of AssemblySymbol), Boolean)
				Dim assemblyIdentityMap As AssemblyIdentityMap(Of AssemblySymbol) = New AssemblyIdentityMap(Of AssemblySymbol)()
				Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = MyBase.ReferencedAssemblies.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AssemblySymbol = enumerator.Current
					assemblyIdentityMap.Add(current.Identity, current)
				End While
				Dim assembly As PEAssembly = metadata.GetAssembly()
				Dim assemblySymbols As ImmutableArray(Of AssemblySymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AssemblyIdentity, AssemblyIdentityMap(Of AssemblySymbol), AssemblySymbol)(assembly.AssemblyReferences, New Func(Of AssemblyIdentity, AssemblyIdentityMap(Of AssemblySymbol), AssemblySymbol)(AddressOf VisualBasicCompilation.ReferenceManager.MapAssemblyIdentityToResolvedSymbol), assemblyIdentityMap)
				assemblyReferenceIdentityMap = CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).GetAssemblyReferenceIdentityBaselineMap(assemblySymbols, assembly.AssemblyReferences)
				Dim pEAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEAssemblySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEAssemblySymbol(assembly, DocumentationProvider.[Default], False, importOptions)
				Dim unifiedAssemblies As ImmutableArray(Of UnifiedAssembly(Of !1)) = MyBase.UnifiedAssemblies
				If (VisualBasicCompilation.ReferenceManager._Closure$__.$I12-0 Is Nothing) Then
					func = Function(unified As UnifiedAssembly(Of AssemblySymbol), refAsmByIdentity As AssemblyIdentityMap(Of AssemblySymbol)) refAsmByIdentity.Contains(unified.OriginalReference, False)
					VisualBasicCompilation.ReferenceManager._Closure$__.$I12-0 = func
				Else
					func = VisualBasicCompilation.ReferenceManager._Closure$__.$I12-0
				End If
				Dim unifiedAssemblies1 As ImmutableArray(Of UnifiedAssembly(Of AssemblySymbol)) = unifiedAssemblies.WhereAsArray(Of AssemblyIdentityMap(Of AssemblySymbol))(func, assemblyIdentityMap)
				Me.InitializeAssemblyReuseData(pEAssemblySymbol, assemblySymbols, unifiedAssemblies1)
				If (assembly.ContainsNoPiaLocalTypes()) Then
					pEAssemblySymbol.SetNoPiaResolutionAssemblies(MyBase.ReferencedAssemblies)
				End If
				Return pEAssemblySymbol
			End Function

			Public Sub CreateSourceAssemblyForCompilation(ByVal compilation As VisualBasicCompilation)
				If (MyBase.IsBound OrElse Not Me.CreateAndSetSourceAssemblyFullBind(compilation)) Then
					If (Not MyBase.HasCircularReference) Then
						Me.CreateAndSetSourceAssemblyReuseData(compilation)
						Return
					End If
					Dim flag As Boolean = (New VisualBasicCompilation.ReferenceManager(Me.SimpleAssemblyName, Me.IdentityComparer, Me.ObservedMetadata)).CreateAndSetSourceAssemblyFullBind(compilation)
				End If
			End Sub

			Protected Overrides Sub GetActualBoundReferencesUsedBy(ByVal assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, ByVal referencedAssemblySymbols As List(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol))
				Dim enumerator As ImmutableArray(Of ModuleSymbol).Enumerator = assemblySymbol.Modules.GetEnumerator()
				While enumerator.MoveNext()
					referencedAssemblySymbols.AddRange(DirectCast(enumerator.Current.GetReferencedAssemblySymbols(), IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)))
				End While
				Dim count As Integer = referencedAssemblySymbols.Count - 1
				For i As Integer = 0 To count
					If (referencedAssemblySymbols(i).IsMissing) Then
						referencedAssemblySymbols(i) = Nothing
					End If
				Next

			End Sub

			Private Shared Function GetAssemblyDefinitionSymbol(ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).BoundInputAssembly(), ByVal referenceBinding As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyReferenceBinding, ByRef unifiedAssemblies As ArrayBuilder(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol))) As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = bindingResult(referenceBinding.DefinitionIndex).AssemblySymbol
				If (referenceBinding.VersionDifference <> 0) Then
					If (unifiedAssemblies Is Nothing) Then
						unifiedAssemblies = New ArrayBuilder(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol))()
					End If
					unifiedAssemblies.Add(New UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(assemblySymbol, referenceBinding.ReferenceIdentity))
				End If
				Return assemblySymbol
			End Function

			Protected Overrides Function GetCorLibrary(ByVal candidateAssembly As AssemblySymbol) As AssemblySymbol
				Dim corLibrary As AssemblySymbol = candidateAssembly.CorLibrary
				If (Not corLibrary.IsMissing) Then
					Return corLibrary
				End If
				Return Nothing
			End Function

			Protected Overrides Function GetNoPiaResolutionAssemblies(ByVal candidateAssembly As AssemblySymbol) As ImmutableArray(Of AssemblySymbol)
				Dim noPiaResolutionAssemblies As ImmutableArray(Of AssemblySymbol)
				If (Not TypeOf candidateAssembly Is SourceAssemblySymbol) Then
					noPiaResolutionAssemblies = candidateAssembly.GetNoPiaResolutionAssemblies()
				Else
					noPiaResolutionAssemblies = ImmutableArray(Of AssemblySymbol).Empty
				End If
				Return noPiaResolutionAssemblies
			End Function

			Private Shared Function GetOrAddMissingAssemblySymbol(ByVal identity As AssemblyIdentity, ByRef missingAssemblies As Dictionary(Of AssemblyIdentity, Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol
				Dim missingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol
				Dim missingAssemblySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol = Nothing
				If (missingAssemblies IsNot Nothing) Then
					If (Not missingAssemblies.TryGetValue(identity, missingAssemblySymbol1)) Then
						missingAssemblySymbol1 = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol(identity)
						missingAssemblies.Add(identity, missingAssemblySymbol1)
						missingAssemblySymbol = missingAssemblySymbol1
						Return missingAssemblySymbol
					End If
					missingAssemblySymbol = missingAssemblySymbol1
					Return missingAssemblySymbol
				Else
					missingAssemblies = New Dictionary(Of AssemblyIdentity, Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol)()
				End If
				missingAssemblySymbol1 = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol(identity)
				missingAssemblies.Add(identity, missingAssemblySymbol1)
				missingAssemblySymbol = missingAssemblySymbol1
				Return missingAssemblySymbol
			End Function

			Private Sub InitializeAssemblyReuseData(ByVal assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, ByVal referencedAssemblies As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), ByVal unifiedAssemblies As ImmutableArray(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)))
				Dim identity As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, AssemblyIdentity)
				assemblySymbol.SetCorLibrary(If(MyBase.CorLibraryOpt, assemblySymbol))
				Dim assemblySymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = referencedAssemblies
				If (VisualBasicCompilation.ReferenceManager._Closure$__.$I15-0 Is Nothing) Then
					identity = Function(a As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) a.Identity
					VisualBasicCompilation.ReferenceManager._Closure$__.$I15-0 = identity
				Else
					identity = VisualBasicCompilation.ReferenceManager._Closure$__.$I15-0
				End If
				Dim moduleReference As ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = New ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, AssemblyIdentity)(assemblySymbols, identity), referencedAssemblies, unifiedAssemblies)
				assemblySymbol.Modules(0).SetReferences(moduleReference, Nothing)
				Dim modules As ImmutableArray(Of ModuleSymbol) = assemblySymbol.Modules
				Dim referencedModulesReferences As ImmutableArray(Of ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)) = MyBase.ReferencedModulesReferences
				Dim length As Integer = modules.Length - 1
				For i As Integer = 1 To length
					modules(i).SetReferences(referencedModulesReferences(i - 1), Nothing)
				Next

			End Sub

			Private Shared Sub InitializeNewSymbols(ByVal newSymbols As List(Of Integer), ByVal assemblySymbol As SourceAssemblySymbol, ByVal assemblies As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyData), ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).BoundInputAssembly(), ByVal missingAssemblies As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol))
				Dim enumerator As List(Of Integer).Enumerator = New List(Of Integer).Enumerator()
				Dim enumerator1 As List(Of Integer).Enumerator = New List(Of Integer).Enumerator()
				Dim enumerator2 As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol).ValueCollection.Enumerator = New Dictionary(Of AssemblyIdentity, MissingAssemblySymbol).ValueCollection.Enumerator()
				Dim corLibrary As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = assemblySymbol.CorLibrary
				Try
					enumerator = newSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Integer = enumerator.Current
						If (Not TypeOf assemblies(current) Is VisualBasicCompilation.ReferenceManager.AssemblyDataForCompilation) Then
							VisualBasicCompilation.ReferenceManager.SetupReferencesForFileAssembly(DirectCast(assemblies(current), VisualBasicCompilation.ReferenceManager.AssemblyDataForFile), bindingResult, current, missingAssemblies, assemblySymbol)
						Else
							VisualBasicCompilation.ReferenceManager.SetupReferencesForRetargetingAssembly(bindingResult, current, missingAssemblies, assemblySymbol)
						End If
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).GetInstance()
				Try
					enumerator1 = newSymbols.GetEnumerator()
					While enumerator1.MoveNext()
						Dim num As Integer = enumerator1.Current
						If (assemblies(num).ContainsNoPiaLocalTypes) Then
							bindingResult(num).AssemblySymbol.SetNoPiaResolutionAssemblies(assemblySymbol.Modules(0).GetReferencedAssemblySymbols())
						End If
						instance.Clear()
						If (assemblies(num).IsLinked) Then
							instance.Add(bindingResult(num).AssemblySymbol)
						End If
						Dim referenceBinding As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyReferenceBinding() = bindingResult(num).ReferenceBinding
						Dim num1 As Integer = 0
						Do
							Dim assemblyReferenceBinding As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyReferenceBinding = referenceBinding(num1)
							If (assemblyReferenceBinding.IsBound AndAlso assemblies(assemblyReferenceBinding.DefinitionIndex).IsLinked) Then
								instance.Add(bindingResult(assemblyReferenceBinding.DefinitionIndex).AssemblySymbol)
							End If
							num1 = num1 + 1
						Loop While num1 < CInt(referenceBinding.Length)
						If (instance.Count > 0) Then
							instance.RemoveDuplicates()
							bindingResult(num).AssemblySymbol.SetLinkedReferencedAssemblies(instance.ToImmutable())
						End If
						bindingResult(num).AssemblySymbol.SetCorLibrary(corLibrary)
					End While
				Finally
					DirectCast(enumerator1, IDisposable).Dispose()
				End Try
				instance.Free()
				If (missingAssemblies IsNot Nothing) Then
					Try
						enumerator2 = missingAssemblies.Values.GetEnumerator()
						While enumerator2.MoveNext()
							enumerator2.Current.SetCorLibrary(corLibrary)
						End While
					Finally
						DirectCast(enumerator2, IDisposable).Dispose()
					End Try
				End If
			End Sub

			Protected Overrides Function IsLinked(ByVal candidateAssembly As AssemblySymbol) As Boolean
				Return candidateAssembly.IsLinked
			End Function

			Friend Shared Function IsReferenceManagerInitialized(ByVal compilation As VisualBasicCompilation) As Boolean
				Return compilation._referenceManager.IsBound
			End Function

			Friend Shared Function IsSourceAssemblySymbolCreated(ByVal compilation As VisualBasicCompilation) As Boolean
				Return CObj(compilation._lazyAssemblySymbol) <> CObj(Nothing)
			End Function

			Private Shared Function MapAssemblyIdentityToResolvedSymbol(ByVal identity As Microsoft.CodeAnalysis.AssemblyIdentity, ByVal map As AssemblyIdentityMap(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim missingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim func As Func(Of Version, Version, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Boolean)
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = Nothing
				If (Not map.TryGetValue(identity, assemblySymbol, New Func(Of Version, Version, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Boolean)(AddressOf CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).CompareVersionPartsSpecifiedInSource))) Then
					Dim assemblyIdentityMap As AssemblyIdentityMap(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = map
					Dim assemblyIdentity As Microsoft.CodeAnalysis.AssemblyIdentity = identity
					If (VisualBasicCompilation.ReferenceManager._Closure$__.$I13-0 Is Nothing) Then
						func = Function(v1 As Version, v2 As Version, s As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) True
						VisualBasicCompilation.ReferenceManager._Closure$__.$I13-0 = func
					Else
						func = VisualBasicCompilation.ReferenceManager._Closure$__.$I13-0
					End If
					If (assemblyIdentityMap.TryGetValue(assemblyIdentity, assemblySymbol, func)) Then
						Throw New NotSupportedException([String].Format(CodeAnalysisResources.ChangingVersionOfAssemblyReferenceIsNotAllowedDuringDebugging, identity, assemblySymbol.Identity.Version))
					End If
					missingAssemblySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingAssemblySymbol(identity)
				Else
					missingAssemblySymbol = assemblySymbol
				End If
				Return missingAssemblySymbol
			End Function

			Private Shared Sub SetupReferencesForFileAssembly(ByVal fileData As VisualBasicCompilation.ReferenceManager.AssemblyDataForFile, ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).BoundInputAssembly(), ByVal bindingIndex As Integer, ByRef missingAssemblies As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol), ByVal sourceAssemblyDebugOnly As SourceAssemblySymbol)
				Dim modules As ImmutableArray(Of ModuleSymbol) = DirectCast(bindingResult(bindingIndex).AssemblySymbol, PEAssemblySymbol).Modules
				Dim num As Integer = 0
				Dim length As Integer = modules.Length - 1
				For i As Integer = 0 To length
					Dim item As Integer = fileData.Assembly.ModuleReferenceCounts(i)
					Dim assemblyIdentityArray(item - 1 + 1 - 1) As AssemblyIdentity
					Dim orAddMissingAssemblySymbol(item - 1 + 1 - 1) As AssemblySymbol
					fileData.AssemblyReferences.CopyTo(num, assemblyIdentityArray, 0, item)
					Dim unifiedAssemblies As ArrayBuilder(Of UnifiedAssembly(Of AssemblySymbol)) = Nothing
					Dim num1 As Integer = item - 1
					Dim num2 As Integer = 0
					Do
						Dim referenceBinding As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyReferenceBinding = bindingResult(bindingIndex).ReferenceBinding(num + num2)
						If (Not referenceBinding.IsBound) Then
							orAddMissingAssemblySymbol(num2) = VisualBasicCompilation.ReferenceManager.GetOrAddMissingAssemblySymbol(assemblyIdentityArray(num2), missingAssemblies)
						Else
							orAddMissingAssemblySymbol(num2) = VisualBasicCompilation.ReferenceManager.GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, unifiedAssemblies)
						End If
						num2 = num2 + 1
					Loop While num2 <= num1
					Dim moduleReference As ModuleReferences(Of AssemblySymbol) = New ModuleReferences(Of AssemblySymbol)(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AssemblyIdentity)(assemblyIdentityArray), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AssemblySymbol)(orAddMissingAssemblySymbol), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrEmpty(Of UnifiedAssembly(Of AssemblySymbol))(unifiedAssemblies))
					modules(i).SetReferences(moduleReference, sourceAssemblyDebugOnly)
					num += item
				Next

			End Sub

			Private Shared Sub SetupReferencesForRetargetingAssembly(ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).BoundInputAssembly(), ByVal bindingIndex As Integer, ByRef missingAssemblies As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol), ByVal sourceAssemblyDebugOnly As SourceAssemblySymbol)
				Dim assemblySymbol As RetargetingAssemblySymbol = DirectCast(bindingResult(bindingIndex).AssemblySymbol, RetargetingAssemblySymbol)
				Dim modules As ImmutableArray(Of ModuleSymbol) = assemblySymbol.Modules
				Dim num As Integer = 0
				Dim length As Integer = modules.Length - 1
				For i As Integer = 0 To length
					Dim moduleSymbols As ImmutableArray(Of ModuleSymbol) = assemblySymbol.UnderlyingAssembly.Modules
					Dim referencedAssemblies As ImmutableArray(Of AssemblyIdentity) = moduleSymbols(i).GetReferencedAssemblies()
					If (i = 0) Then
						moduleSymbols = assemblySymbol.UnderlyingAssembly.Modules
						Dim referencedAssemblySymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = moduleSymbols(0).GetReferencedAssemblySymbols()
						Dim num1 As Integer = 0
						Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).Enumerator = referencedAssemblySymbols.GetEnumerator()
						While enumerator.MoveNext()
							If (Not enumerator.Current.IsLinked) Then
								Continue While
							End If
							num1 = num1 + 1
						End While
						If (num1 > 0) Then
							Dim item(referencedAssemblies.Length - num1 - 1 + 1 - 1) As AssemblyIdentity
							Dim num2 As Integer = 0
							Dim length1 As Integer = referencedAssemblySymbols.Length - 1
							Dim num3 As Integer = 0
							Do
								If (Not referencedAssemblySymbols(num3).IsLinked) Then
									item(num2) = referencedAssemblies(num3)
									num2 = num2 + 1
								End If
								num3 = num3 + 1
							Loop While num3 <= length1
							referencedAssemblies = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AssemblyIdentity)(item)
						End If
					End If
					Dim length2 As Integer = referencedAssemblies.Length
					Dim orAddMissingAssemblySymbol(length2 - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
					Dim unifiedAssemblies As ArrayBuilder(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)) = Nothing
					Dim num4 As Integer = length2 - 1
					Dim num5 As Integer = 0
					Do
						Dim referenceBinding As CommonReferenceManager(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol).AssemblyReferenceBinding = bindingResult(bindingIndex).ReferenceBinding(num + num5)
						If (Not referenceBinding.IsBound) Then
							orAddMissingAssemblySymbol(num5) = VisualBasicCompilation.ReferenceManager.GetOrAddMissingAssemblySymbol(referencedAssemblies(num5), missingAssemblies)
						Else
							orAddMissingAssemblySymbol(num5) = VisualBasicCompilation.ReferenceManager.GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, unifiedAssemblies)
						End If
						num5 = num5 + 1
					Loop While num5 <= num4
					Dim moduleReference As ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = New ModuleReferences(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(referencedAssemblies, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(orAddMissingAssemblySymbol), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrEmpty(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol))(unifiedAssemblies))
					modules(i).SetReferences(moduleReference, sourceAssemblyDebugOnly)
					num += length2
				Next

			End Sub

			Private Shared Sub SetupReferencesForSourceAssembly(ByVal sourceAssembly As SourceAssemblySymbol, ByVal modules As ImmutableArray(Of PEModule), ByVal totalReferencedAssemblyCount As Integer, ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).BoundInputAssembly(), ByRef missingAssemblies As Dictionary(Of AssemblyIdentity, MissingAssemblySymbol), ByRef moduleReferences As ImmutableArray(Of ModuleReferences(Of AssemblySymbol)))
				Dim instance As ArrayBuilder(Of ModuleReferences(Of AssemblySymbol))
				Dim moduleSymbols As ImmutableArray(Of ModuleSymbol) = sourceAssembly.Modules
				If (moduleSymbols.Length > 1) Then
					instance = ArrayBuilder(Of ModuleReferences(Of AssemblySymbol)).GetInstance()
				Else
					instance = Nothing
				End If
				Dim moduleReferences1 As ArrayBuilder(Of ModuleReferences(Of AssemblySymbol)) = instance
				Dim num As Integer = 0
				Dim length As Integer = moduleSymbols.Length - 1
				Dim num1 As Integer = 0
				Do
					Dim num2 As Integer = If(num1 = 0, totalReferencedAssemblyCount, modules(num1 - 1).ReferencedAssemblies.Length)
					Dim referenceIdentity(num2 - 1 + 1 - 1) As AssemblyIdentity
					Dim orAddMissingAssemblySymbol(num2 - 1 + 1 - 1) As AssemblySymbol
					Dim unifiedAssemblies As ArrayBuilder(Of UnifiedAssembly(Of AssemblySymbol)) = Nothing
					Dim num3 As Integer = num2 - 1
					Dim num4 As Integer = 0
					Do
						Dim referenceBinding As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyReferenceBinding = bindingResult(0).ReferenceBinding(num + num4)
						If (Not referenceBinding.IsBound) Then
							orAddMissingAssemblySymbol(num4) = VisualBasicCompilation.ReferenceManager.GetOrAddMissingAssemblySymbol(referenceBinding.ReferenceIdentity, missingAssemblies)
						Else
							orAddMissingAssemblySymbol(num4) = VisualBasicCompilation.ReferenceManager.GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, unifiedAssemblies)
						End If
						referenceIdentity(num4) = referenceBinding.ReferenceIdentity
						num4 = num4 + 1
					Loop While num4 <= num3
					Dim moduleReference As ModuleReferences(Of AssemblySymbol) = New ModuleReferences(Of AssemblySymbol)(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AssemblyIdentity)(referenceIdentity), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AssemblySymbol)(orAddMissingAssemblySymbol), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrEmpty(Of UnifiedAssembly(Of AssemblySymbol))(unifiedAssemblies))
					If (num1 > 0) Then
						moduleReferences1.Add(moduleReference)
					End If
					moduleSymbols(num1).SetReferences(moduleReference, sourceAssembly)
					num += num2
					num1 = num1 + 1
				Loop While num1 <= length
				moduleReferences = ArrayBuilderExtensions.ToImmutableOrEmptyAndFree(Of ModuleReferences(Of AssemblySymbol))(moduleReferences1)
			End Sub

			Private Sub UpdateSymbolCacheNoLock(ByVal newSymbols As List(Of Integer), ByVal assemblies As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyData), ByVal bindingResult As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).BoundInputAssembly())
				Dim enumerator As List(Of Integer).Enumerator = New List(Of Integer).Enumerator()
				Try
					enumerator = newSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Integer = enumerator.Current
						Dim item As VisualBasicCompilation.ReferenceManager.AssemblyDataForCompilation = TryCast(assemblies(current), VisualBasicCompilation.ReferenceManager.AssemblyDataForCompilation)
						If (item Is Nothing) Then
							DirectCast(assemblies(current), VisualBasicCompilation.ReferenceManager.AssemblyDataForFile).CachedSymbols.Add(bindingResult(current).AssemblySymbol)
						Else
							item.Compilation.CacheRetargetingAssemblySymbolNoLock(bindingResult(current).AssemblySymbol)
						End If
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End Sub

			Protected Overrides Function WeakIdentityPropertiesEquivalent(ByVal identity1 As AssemblyIdentity, ByVal identity2 As AssemblyIdentity) As Boolean
				Return identity1.Version = identity2.Version
			End Function

			Private NotInheritable Class AssemblyDataForCompilation
				Inherits VisualBasicCompilation.ReferenceManager.AssemblyDataForMetadataOrCompilation
				Public ReadOnly Compilation As VisualBasicCompilation

				Public Overrides ReadOnly Property ContainsNoPiaLocalTypes As Boolean
					Get
						Return Me.Compilation.MightContainNoPiaLocalTypes()
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaresTheObjectClass As Boolean
					Get
						Return Me.Compilation.DeclaresTheObjectClass
					End Get
				End Property

				Public Overrides ReadOnly Property SourceCompilation As Compilation
					Get
						Return Me.Compilation
					End Get
				End Property

				Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal embedInteropTypes As Boolean)
					MyBase.New(compilation.Assembly.Identity, VisualBasicCompilation.ReferenceManager.AssemblyDataForCompilation.GetReferencedAssemblies(compilation), embedInteropTypes)
					Me.Compilation = compilation
				End Sub

				Protected Overrides Sub AddAvailableSymbols(ByVal assemblies As List(Of AssemblySymbol))
					assemblies.Add(Me.Compilation.Assembly)
					SyncLock CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard
						Me.Compilation.AddRetargetingAssemblySymbolsNoLock(Of AssemblySymbol)(assemblies)
					End SyncLock
				End Sub

				Friend Overrides Function CreateAssemblySymbol() As AssemblySymbol
					Return New RetargetingAssemblySymbol(Me.Compilation.SourceAssembly, MyBase.IsLinked)
				End Function

				Private Shared Function GetReferencedAssemblies(ByVal compilation As VisualBasicCompilation) As ImmutableArray(Of AssemblyIdentity)
					Dim instance As ArrayBuilder(Of AssemblyIdentity) = ArrayBuilder(Of AssemblyIdentity).GetInstance()
					Dim modules As ImmutableArray(Of ModuleSymbol) = compilation.Assembly.Modules
					Dim referencedAssemblies As ImmutableArray(Of AssemblyIdentity) = modules(0).GetReferencedAssemblies()
					Dim referencedAssemblySymbols As ImmutableArray(Of AssemblySymbol) = modules(0).GetReferencedAssemblySymbols()
					Dim length As Integer = referencedAssemblies.Length - 1
					Dim num As Integer = 0
					Do
						If (Not referencedAssemblySymbols(num).IsLinked) Then
							instance.Add(referencedAssemblies(num))
						End If
						num = num + 1
					Loop While num <= length
					Dim length1 As Integer = modules.Length - 1
					Dim num1 As Integer = 1
					Do
						instance.AddRange(modules(num1).GetReferencedAssemblies())
						num1 = num1 + 1
					Loop While num1 <= length1
					Return instance.ToImmutableAndFree()
				End Function

				Public Overrides Function IsMatchingAssembly(ByVal candidateAssembly As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) As Boolean
					Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
					Dim retargetingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol = TryCast(candidateAssembly, Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol)
					assemblySymbol = If(retargetingAssemblySymbol Is Nothing, TryCast(candidateAssembly, SourceAssemblySymbol), retargetingAssemblySymbol.UnderlyingAssembly)
					Return CObj(assemblySymbol) = CObj(Me.Compilation.Assembly)
				End Function
			End Class

			Private NotInheritable Class AssemblyDataForFile
				Inherits VisualBasicCompilation.ReferenceManager.AssemblyDataForMetadataOrCompilation
				Public ReadOnly Assembly As PEAssembly

				Public ReadOnly CachedSymbols As WeakList(Of IAssemblySymbolInternal)

				Public ReadOnly DocumentationProvider As DocumentationProvider

				Private ReadOnly _compilationImportOptions As MetadataImportOptions

				Private ReadOnly _sourceAssemblySimpleName As String

				Private _internalsVisibleComputed As Boolean

				Private _internalsPotentiallyVisibleToCompilation As Boolean

				Public Overrides ReadOnly Property ContainsNoPiaLocalTypes As Boolean
					Get
						Return Me.Assembly.ContainsNoPiaLocalTypes()
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaresTheObjectClass As Boolean
					Get
						Return Me.Assembly.DeclaresTheObjectClass
					End Get
				End Property

				Friend ReadOnly Property EffectiveImportOptions As MetadataImportOptions
					Get
						Dim metadataImportOption As MetadataImportOptions
						metadataImportOption = If(Not Me.InternalsMayBeVisibleToCompilation OrElse Me._compilationImportOptions <> MetadataImportOptions.[Public], Me._compilationImportOptions, MetadataImportOptions.Internal)
						Return metadataImportOption
					End Get
				End Property

				Friend ReadOnly Property InternalsMayBeVisibleToCompilation As Boolean
					Get
						If (Not Me._internalsVisibleComputed) Then
							Me._internalsPotentiallyVisibleToCompilation = CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).InternalsMayBeVisibleToAssemblyBeingCompiled(Me._sourceAssemblySimpleName, Me.Assembly)
							Me._internalsVisibleComputed = True
						End If
						Return Me._internalsPotentiallyVisibleToCompilation
					End Get
				End Property

				Public Overrides ReadOnly Property SourceCompilation As Compilation
					Get
						Return Nothing
					End Get
				End Property

				Public Sub New(ByVal assembly As PEAssembly, ByVal cachedSymbols As WeakList(Of IAssemblySymbolInternal), ByVal embedInteropTypes As Boolean, ByVal documentationProvider As Microsoft.CodeAnalysis.DocumentationProvider, ByVal sourceAssemblySimpleName As String, ByVal compilationImportOptions As MetadataImportOptions)
					MyBase.New(assembly.Identity, assembly.AssemblyReferences, embedInteropTypes)
					Me._internalsVisibleComputed = False
					Me._internalsPotentiallyVisibleToCompilation = False
					Me.CachedSymbols = cachedSymbols
					Me.Assembly = assembly
					Me.DocumentationProvider = documentationProvider
					Me._compilationImportOptions = compilationImportOptions
					Me._sourceAssemblySimpleName = sourceAssemblySimpleName
				End Sub

				Protected Overrides Sub AddAvailableSymbols(ByVal assemblies As List(Of AssemblySymbol))
					Dim enumerator As IEnumerator(Of IAssemblySymbolInternal) = Nothing
					Dim internalsMayBeVisibleToCompilation As Boolean = Me.InternalsMayBeVisibleToCompilation
					SyncLock CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard
						Try
							enumerator = Me.CachedSymbols.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As PEAssemblySymbol = TryCast(enumerator.Current, PEAssemblySymbol)
								If (Not Me.IsMatchingAssembly(current)) Then
									Continue While
								End If
								assemblies.Add(current)
							End While
						Finally
							If (enumerator IsNot Nothing) Then
								enumerator.Dispose()
							End If
						End Try
					End SyncLock
				End Sub

				Friend Overrides Function CreateAssemblySymbol() As AssemblySymbol
					Return New PEAssemblySymbol(Me.Assembly, Me.DocumentationProvider, MyBase.IsLinked, Me.EffectiveImportOptions)
				End Function

				Public Overrides Function IsMatchingAssembly(ByVal candidateAssembly As AssemblySymbol) As Boolean
					Return Me.IsMatchingAssembly(TryCast(candidateAssembly, PEAssemblySymbol))
				End Function

				Private Function IsMatchingAssembly(ByVal peAssembly As PEAssemblySymbol) As Boolean
					Dim flag As Boolean
					If (peAssembly Is Nothing) Then
						flag = False
					ElseIf (peAssembly.Assembly <> Me.Assembly) Then
						flag = False
					ElseIf (Me.EffectiveImportOptions = peAssembly.PrimaryModule.ImportOptions) Then
						flag = If(peAssembly.DocumentationProvider.Equals(Me.DocumentationProvider), True, False)
					Else
						flag = False
					End If
					Return flag
				End Function
			End Class

			Private MustInherit Class AssemblyDataForMetadataOrCompilation
				Inherits CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyData
				Private _assemblies As List(Of AssemblySymbol)

				Private ReadOnly _identity As AssemblyIdentity

				Private ReadOnly _referencedAssemblies As ImmutableArray(Of AssemblyIdentity)

				Private ReadOnly _embedInteropTypes As Boolean

				Public Overrides ReadOnly Property AssemblyReferences As ImmutableArray(Of AssemblyIdentity)
					Get
						Return Me._referencedAssemblies
					End Get
				End Property

				Public Overrides ReadOnly Property AvailableSymbols As IEnumerable(Of AssemblySymbol)
					Get
						If (Me._assemblies Is Nothing) Then
							Me._assemblies = New List(Of AssemblySymbol)()
							Me.AddAvailableSymbols(Me._assemblies)
						End If
						Return Me._assemblies
					End Get
				End Property

				Public Overrides ReadOnly Property Identity As AssemblyIdentity
					Get
						Return Me._identity
					End Get
				End Property

				Public NotOverridable Overrides ReadOnly Property IsLinked As Boolean
					Get
						Return Me._embedInteropTypes
					End Get
				End Property

				Protected Sub New(ByVal identity As AssemblyIdentity, ByVal referencedAssemblies As ImmutableArray(Of AssemblyIdentity), ByVal embedInteropTypes As Boolean)
					MyBase.New()
					Me._embedInteropTypes = embedInteropTypes
					Me._identity = identity
					Me._referencedAssemblies = referencedAssemblies
				End Sub

				Protected MustOverride Sub AddAvailableSymbols(ByVal assemblies As List(Of AssemblySymbol))

				Public Overrides Function BindAssemblyReferences(ByVal assemblies As ImmutableArray(Of CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyData), ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer) As CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).AssemblyReferenceBinding()
					Return CommonReferenceManager(Of VisualBasicCompilation, AssemblySymbol).ResolveReferencedAssemblies(Me._referencedAssemblies, assemblies, 0, assemblyIdentityComparer)
				End Function

				Friend MustOverride Function CreateAssemblySymbol() As AssemblySymbol
			End Class
		End Class

		Friend Class SpecialMembersSignatureComparer
			Inherits SignatureComparer(Of MethodSymbol, FieldSymbol, PropertySymbol, TypeSymbol, ParameterSymbol)
			Public ReadOnly Shared Instance As VisualBasicCompilation.SpecialMembersSignatureComparer

			Shared Sub New()
				VisualBasicCompilation.SpecialMembersSignatureComparer.Instance = New VisualBasicCompilation.SpecialMembersSignatureComparer()
			End Sub

			Protected Sub New()
				MyBase.New()
			End Sub

			Protected Overrides Function GetFieldType(ByVal field As FieldSymbol) As TypeSymbol
				Return field.Type
			End Function

			Protected Overrides Function GetGenericTypeArgument(ByVal type As TypeSymbol, ByVal argumentIndex As Integer) As TypeSymbol
				Dim item As TypeSymbol
				If (type.Kind = SymbolKind.NamedType) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol.Arity <= argumentIndex) Then
						item = Nothing
					ElseIf (namedTypeSymbol.ContainingType Is Nothing) Then
						item = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics(argumentIndex)
					Else
						item = Nothing
					End If
				Else
					item = Nothing
				End If
				Return item
			End Function

			Protected Overrides Function GetGenericTypeDefinition(ByVal type As TypeSymbol) As TypeSymbol
				Dim originalDefinition As TypeSymbol
				If (type.Kind = SymbolKind.NamedType) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol.ContainingType IsNot Nothing) Then
						originalDefinition = Nothing
					ElseIf (namedTypeSymbol.Arity <> 0) Then
						originalDefinition = namedTypeSymbol.OriginalDefinition
					Else
						originalDefinition = Nothing
					End If
				Else
					originalDefinition = Nothing
				End If
				Return originalDefinition
			End Function

			Protected Overrides Function GetMDArrayElementType(ByVal type As TypeSymbol) As TypeSymbol
				Dim elementType As TypeSymbol
				If (type.Kind = SymbolKind.ArrayType) Then
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					If (Not arrayTypeSymbol.IsSZArray) Then
						elementType = arrayTypeSymbol.ElementType
					Else
						elementType = Nothing
					End If
				Else
					elementType = Nothing
				End If
				Return elementType
			End Function

			Protected Overrides Function GetParameters(ByVal method As MethodSymbol) As ImmutableArray(Of ParameterSymbol)
				Return method.Parameters
			End Function

			Protected Overrides Function GetParameters(ByVal [property] As PropertySymbol) As ImmutableArray(Of ParameterSymbol)
				Return [property].Parameters
			End Function

			Protected Overrides Function GetParamType(ByVal parameter As ParameterSymbol) As TypeSymbol
				Return parameter.Type
			End Function

			Protected Overrides Function GetPointedToType(ByVal type As TypeSymbol) As TypeSymbol
				Return Nothing
			End Function

			Protected Overrides Function GetPropertyType(ByVal prop As PropertySymbol) As TypeSymbol
				Return prop.Type
			End Function

			Protected Overrides Function GetReturnType(ByVal method As MethodSymbol) As TypeSymbol
				Return method.ReturnType
			End Function

			Protected Overrides Function GetSZArrayElementType(ByVal type As TypeSymbol) As TypeSymbol
				Dim elementType As TypeSymbol
				If (type.Kind = SymbolKind.ArrayType) Then
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					If (arrayTypeSymbol.IsSZArray) Then
						elementType = arrayTypeSymbol.ElementType
					Else
						elementType = Nothing
					End If
				Else
					elementType = Nothing
				End If
				Return elementType
			End Function

			Protected Overrides Function IsByRefMethod(ByVal method As MethodSymbol) As Boolean
				Return method.ReturnsByRef
			End Function

			Protected Overrides Function IsByRefParam(ByVal parameter As ParameterSymbol) As Boolean
				Return parameter.IsByRef
			End Function

			Protected Overrides Function IsByRefProperty(ByVal [property] As PropertySymbol) As Boolean
				Return [property].ReturnsByRef
			End Function

			Protected Overrides Function IsGenericMethodTypeParam(ByVal type As TypeSymbol, ByVal paramPosition As Integer) As Boolean
				Dim flag As Boolean
				If (type.Kind = SymbolKind.TypeParameter) Then
					Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
					flag = If(typeParameterSymbol.ContainingSymbol.Kind = SymbolKind.Method, typeParameterSymbol.Ordinal = paramPosition, False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Protected Overrides Function IsGenericTypeParam(ByVal type As TypeSymbol, ByVal paramPosition As Integer) As Boolean
				Dim flag As Boolean
				If (type.Kind = SymbolKind.TypeParameter) Then
					Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
					flag = If(typeParameterSymbol.ContainingSymbol.Kind = SymbolKind.NamedType, typeParameterSymbol.Ordinal = paramPosition, False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Protected Overrides Function MatchArrayRank(ByVal type As TypeSymbol, ByVal countOfDimensions As Integer) As Boolean
				Dim flag As Boolean
				If (countOfDimensions <> 1) Then
					flag = If(type.Kind = SymbolKind.ArrayType, DirectCast(type, ArrayTypeSymbol).Rank = countOfDimensions, False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Protected Overrides Function MatchTypeToTypeId(ByVal type As TypeSymbol, ByVal typeId As Integer) As Boolean
				Return CInt(type.SpecialType) = typeId
			End Function
		End Class

		Friend Class TupleNamesEncoder
			Public Sub New()
				MyBase.New()
			End Sub

			Private Shared Function AddNames(ByVal type As TypeSymbol, ByVal namesBuilder As ArrayBuilder(Of String)) As Boolean
				If (type.IsTupleType) Then
					If (Not type.TupleElementNames.IsDefaultOrEmpty) Then
						namesBuilder.AddRange(type.TupleElementNames)
					Else
						namesBuilder.AddMany(Nothing, type.TupleElementTypes.Length)
					End If
				End If
				Return False
			End Function

			Public Shared Function Encode(ByVal type As TypeSymbol) As ImmutableArray(Of String)
				Dim immutableAndFree As ImmutableArray(Of String)
				Dim instance As ArrayBuilder(Of String) = ArrayBuilder(Of String).GetInstance()
				If (VisualBasicCompilation.TupleNamesEncoder.TryGetNames(type, instance)) Then
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					instance.Free()
					immutableAndFree = New ImmutableArray(Of String)()
				End If
				Return immutableAndFree
			End Function

			Public Shared Function Encode(ByVal type As TypeSymbol, ByVal stringType As TypeSymbol) As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant)
				Dim typedConstants As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant)
				Dim typedConstant As Func(Of String, TypeSymbol, Microsoft.CodeAnalysis.TypedConstant)
				Dim instance As ArrayBuilder(Of String) = ArrayBuilder(Of String).GetInstance()
				If (VisualBasicCompilation.TupleNamesEncoder.TryGetNames(type, instance)) Then
					Dim strs As ArrayBuilder(Of String) = instance
					If (VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I2-0 Is Nothing) Then
						typedConstant = Function(name As String, constantType As TypeSymbol) New Microsoft.CodeAnalysis.TypedConstant(constantType, TypedConstantKind.Primitive, name)
						VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I2-0 = typedConstant
					Else
						typedConstant = VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I2-0
					End If
					Dim typedConstants1 As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = strs.SelectAsArray(Of TypeSymbol, Microsoft.CodeAnalysis.TypedConstant)(typedConstant, stringType)
					instance.Free()
					typedConstants = typedConstants1
				Else
					instance.Free()
					typedConstants = New ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant)()
				End If
				Return typedConstants
			End Function

			Friend Shared Function TryGetNames(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal namesBuilder As ArrayBuilder(Of String)) As Boolean
				Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ArrayBuilder(Of String), Boolean)
				Dim func1 As Func(Of String, Boolean)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type
				If (VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-0 Is Nothing) Then
					func = Function(t As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, builder As ArrayBuilder(Of String)) VisualBasicCompilation.TupleNamesEncoder.AddNames(t, builder)
					VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-0 = func
				Else
					func = VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-0
				End If
				typeSymbol.VisitType(Of ArrayBuilder(Of String))(func, namesBuilder)
				Dim strs As ArrayBuilder(Of String) = namesBuilder
				If (VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-1 Is Nothing) Then
					func1 = Function(name As String) CObj(name) <> CObj(Nothing)
					VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-1 = func1
				Else
					func1 = VisualBasicCompilation.TupleNamesEncoder._Closure$__.$I3-1
				End If
				Return strs.Any(func1)
			End Function
		End Class

		Private Class WellKnownMembersSignatureComparer
			Inherits VisualBasicCompilation.SpecialMembersSignatureComparer
			Private ReadOnly _compilation As VisualBasicCompilation

			Public Sub New(ByVal compilation As VisualBasicCompilation)
				MyBase.New()
				Me._compilation = compilation
			End Sub

			Protected Overrides Function MatchTypeToTypeId(ByVal type As TypeSymbol, ByVal typeId As Integer) As Boolean
				Dim flag As Boolean
				Dim wellKnownType As Microsoft.CodeAnalysis.WellKnownType = typeId
				flag = If(Not wellKnownType.IsWellKnownType(), MyBase.MatchTypeToTypeId(type, typeId), type = Me._compilation.GetWellKnownType(wellKnownType))
				Return flag
			End Function
		End Class
	End Class
End Namespace