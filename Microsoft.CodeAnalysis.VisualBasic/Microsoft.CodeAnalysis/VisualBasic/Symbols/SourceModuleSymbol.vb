Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceModuleSymbol
		Inherits NonMissingModuleSymbol
		Implements IAttributeTargetSymbol
		Private ReadOnly _assemblySymbol As SourceAssemblySymbol

		Private ReadOnly _declarationTable As DeclarationTable

		Private ReadOnly _options As VisualBasicCompilationOptions

		Private _lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _lazyContainsExtensionMethods As Byte

		Private _lazyAssembliesToEmbedTypesFrom As ImmutableArray(Of AssemblySymbol)

		Private _lazyContainsExplicitDefinitionOfNoPiaLocalTypes As ThreeState

		Private _locations As ImmutableArray(Of Location)

		Private ReadOnly _diagnosticBagDeclare As DiagnosticBag

		Private _hasBadAttributes As Boolean

		Private ReadOnly _sourceFileMap As ConcurrentDictionary(Of SyntaxTree, SourceFile)

		Private _lazyGlobalNamespace As SourceNamespaceSymbol

		Private _lazyBoundImports As SourceModuleSymbol.BoundImports

		Private _lazyQuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker

		Private _lazyLinkedAssemblyDiagnostics As ImmutableArray(Of Diagnostic)

		Private _lazyTypesWithDefaultInstanceAlias As Dictionary(Of NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol)

		Private ReadOnly Shared s_noTypesWithDefaultInstanceAlias As Dictionary(Of NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol)

		Private ReadOnly _nameAndExtension As String

		Private ReadOnly _diagnosticLock As Object

		Friend ReadOnly Property AliasImports As ImmutableArray(Of AliasAndImportsClausePosition)
			Get
				Me.EnsureImportsAreBound(CancellationToken.None)
				Return Me._lazyBoundImports.AliasImports
			End Get
		End Property

		Friend ReadOnly Property AliasImportsMap As Dictionary(Of String, AliasAndImportsClausePosition)
			Get
				Me.EnsureImportsAreBound(CancellationToken.None)
				Return Me._lazyBoundImports.AliasImportsMap
			End Get
		End Property

		Friend ReadOnly Property AnyReferencedAssembliesAreLinked As Boolean
			Get
				Return Me.GetAssembliesToEmbedTypesFrom().Length > 0
			End Get
		End Property

		Friend Overrides ReadOnly Property Bit32Required As Boolean
			Get
				Return Me.DeclaringCompilation.Options.Platform = Platform.X86
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._assemblySymbol
			End Get
		End Property

		Public ReadOnly Property ContainingSourceAssembly As SourceAssemblySymbol
			Get
				Return Me._assemblySymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._assemblySymbol
			End Get
		End Property

		Friend ReadOnly Property ContainsExplicitDefinitionOfNoPiaLocalTypes As Boolean
			Get
				If (Me._lazyContainsExplicitDefinitionOfNoPiaLocalTypes = ThreeState.Unknown) Then
					Me._lazyContainsExplicitDefinitionOfNoPiaLocalTypes = SourceModuleSymbol.NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(Me.GlobalNamespace).ToThreeState()
				End If
				Return Me._lazyContainsExplicitDefinitionOfNoPiaLocalTypes = ThreeState.[True]
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Me._assemblySymbol.DeclaringCompilation
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.[Module]
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultMarshallingCharSet As Nullable(Of CharSet)
			Get
				Dim decodedWellKnownAttributeData As CommonModuleWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing OrElse Not decodedWellKnownAttributeData.HasDefaultCharSetAttribute) Then
					Return DirectCast(Nothing, Nullable(Of CharSet))
				End If
				Return New Nullable(Of CharSet)(decodedWellKnownAttributeData.DefaultCharacterSet)
			End Get
		End Property

		Public Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				If (Me._lazyGlobalNamespace Is Nothing) Then
					Dim sourceNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol(Me.DeclaringCompilation.MergedRootDeclaration, Nothing, Me)
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol)(Me._lazyGlobalNamespace, sourceNamespaceSymbol, Nothing)
				End If
				Return Me._lazyGlobalNamespace
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyCompilationRelaxationsAttribute As Boolean
			Get
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = DirectCast(Me.ContainingAssembly, SourceAssemblySymbol).GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return sourceDecodedWellKnownAttributeData.HasCompilationRelaxationsAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyRuntimeCompatibilityAttribute As Boolean
			Get
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = DirectCast(Me.ContainingAssembly, SourceAssemblySymbol).GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return sourceDecodedWellKnownAttributeData.HasRuntimeCompatibilityAttribute
			End Get
		End Property

		Friend ReadOnly Property HasBadAttributes As Boolean
			Get
				Return Me._hasBadAttributes
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim location As Func(Of SingleNamespaceDeclaration, Microsoft.CodeAnalysis.Location)
				If (Me._locations.IsDefault) Then
					Dim declarations As ImmutableArray(Of SingleNamespaceDeclaration) = Me.DeclaringCompilation.MergedRootDeclaration.Declarations
					If (SourceModuleSymbol._Closure$__.$I46-0 Is Nothing) Then
						location = Function(d As SingleNamespaceDeclaration) d.Location
						SourceModuleSymbol._Closure$__.$I46-0 = location
					Else
						location = SourceModuleSymbol._Closure$__.$I46-0
					End If
					ImmutableInterlocked.InterlockedInitialize(Of Microsoft.CodeAnalysis.Location)(Me._locations, Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of SingleNamespaceDeclaration, Microsoft.CodeAnalysis.Location)(declarations, location))
				End If
				Return Me._locations
			End Get
		End Property

		Friend Overrides ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine
			Get
				Dim machine1 As System.Reflection.PortableExecutable.Machine
				Select Case Me.DeclaringCompilation.Options.Platform
					Case Platform.X64
						machine1 = System.Reflection.PortableExecutable.Machine.Amd64
						Exit Select
					Case Platform.Itanium
						machine1 = System.Reflection.PortableExecutable.Machine.IA64
						Exit Select
					Case Platform.AnyCpu32BitPreferred
					Label0:
						machine1 = System.Reflection.PortableExecutable.Machine.I386
						Exit Select
					Case Platform.Arm
						machine1 = System.Reflection.PortableExecutable.Machine.ArmThumb2
						Exit Select
					Case Platform.Arm64
						machine1 = System.Reflection.PortableExecutable.Machine.Arm64
						Exit Select
					Case Else
						GoTo Label0
				End Select
				Return machine1
			End Get
		End Property

		Friend ReadOnly Property MemberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)
			Get
				Me.EnsureImportsAreBound(CancellationToken.None)
				Return Me._lazyBoundImports.MemberImports
			End Get
		End Property

		Friend Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				If (Me._lazyContainsExtensionMethods = 0 AndAlso Me._assemblySymbol.Modules(0) <> Me) Then
					Me._lazyContainsExtensionMethods = 1
				End If
				Return Me._lazyContainsExtensionMethods <> 1
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._nameAndExtension
			End Get
		End Property

		Friend Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return Me._declarationTable.NamespaceNames
			End Get
		End Property

		Friend ReadOnly Property Options As VisualBasicCompilationOptions
			Get
				Return Me._options
			End Get
		End Property

		Friend Overrides ReadOnly Property Ordinal As Integer
			Get
				Return 0
			End Get
		End Property

		Public ReadOnly Property QuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Get
				If (Me._lazyQuickAttributeChecker Is Nothing) Then
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker)(Me._lazyQuickAttributeChecker, Me.CreateQuickAttributeChecker(), Nothing)
				End If
				Return Me._lazyQuickAttributeChecker
			End Get
		End Property

		Friend ReadOnly Property RootNamespace As NamespaceSymbol
			Get
				Return Me.GlobalNamespace.LookupNestedNamespace(Me.Options.GetRootNamespaceParts())
			End Get
		End Property

		Friend ReadOnly Property SyntaxTrees As IEnumerable(Of SyntaxTree)
			Get
				Return DirectCast(Me._assemblySymbol.DeclaringCompilation.AllSyntaxTrees, IEnumerable(Of SyntaxTree))
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return Me._declarationTable.TypeNames
			End Get
		End Property

		Friend ReadOnly Property XmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition)
			Get
				Me.EnsureImportsAreBound(CancellationToken.None)
				Return Me._lazyBoundImports.XmlNamespaces
			End Get
		End Property

		Shared Sub New()
			SourceModuleSymbol.s_noTypesWithDefaultInstanceAlias = New Dictionary(Of NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol)()
		End Sub

		Friend Sub New(ByVal assemblySymbol As SourceAssemblySymbol, ByVal declarationTable As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTable, ByVal options As VisualBasicCompilationOptions, ByVal nameAndExtension As String)
			MyBase.New()
			Me._lazyContainsExtensionMethods = 0
			Me._lazyContainsExplicitDefinitionOfNoPiaLocalTypes = ThreeState.Unknown
			Me._diagnosticBagDeclare = New DiagnosticBag()
			Me._sourceFileMap = New ConcurrentDictionary(Of SyntaxTree, SourceFile)()
			Me._diagnosticLock = RuntimeHelpers.GetObjectValue(New [Object]())
			Me._assemblySymbol = assemblySymbol
			Me._declarationTable = declarationTable
			Me._options = options
			Me._nameAndExtension = nameAndExtension
		End Sub

		Friend Sub AddDeclarationDiagnostics(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not diagBag.IsEmpty) Then
				SyncLock Me._diagnosticLock
					Me.StoreDeclarationDiagnostics(diagBag)
				End SyncLock
			End If
		End Sub

		Friend Function AtomicSetFlagAndRaiseSymbolDeclaredEvent(ByRef variable As Integer, ByVal mask As Integer, ByVal comparand As Integer, ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean
			SyncLock Me._diagnosticLock
				Dim flag1 As Boolean = (variable And mask) = comparand
				If (flag1) Then
					Me.DeclaringCompilation.SymbolDeclaredEvent(symbol)
					If (Not ThreadSafeFlagOperations.[Set](variable, mask)) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
				flag = flag1
			End SyncLock
			Return flag
		End Function

		Friend Function AtomicSetFlagAndStoreDiagnostics(ByRef variable As Integer, ByVal mask As Integer, ByVal comparand As Integer, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				flag = ThreadSafeFlagOperations.[Set](variable, mask)
			Else
				SyncLock Me._diagnosticLock
					Dim flag1 As Boolean = (variable And mask) = comparand
					If (flag1) Then
						Me.StoreDeclarationDiagnostics(diagBag)
						If (Not ThreadSafeFlagOperations.[Set](variable, mask) AndAlso Not SourceModuleSymbol.IsEmptyIgnoringLazyDiagnostics(diagBag)) Then
							Throw ExceptionUtilities.Unreachable
						End If
					End If
					flag = flag1
				End SyncLock
			End If
			Return flag
		End Function

		Friend Function AtomicStoreArrayAndDiagnostics(Of T)(ByRef variable As ImmutableArray(Of T), ByVal value As ImmutableArray(Of T), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				flag = ImmutableInterlocked.InterlockedInitialize(Of T)(variable, value)
			Else
				SyncLock Me._diagnosticLock
					If (Not variable.IsDefault) Then
						flag = False
					Else
						Me.StoreDeclarationDiagnostics(diagBag)
						Dim flag1 As Boolean = ImmutableInterlocked.InterlockedInitialize(Of T)(variable, value)
						If (Not flag1 AndAlso Not SourceModuleSymbol.IsEmptyIgnoringLazyDiagnostics(diagBag)) Then
							Throw ExceptionUtilities.Unreachable
						End If
						flag = flag1
					End If
				End SyncLock
			End If
			Return flag
		End Function

		Friend Sub AtomicStoreAttributesAndDiagnostics(ByVal attributesBag As CustomAttributesBag(Of VisualBasicAttributeData), ByVal attributesToStore As ImmutableArray(Of VisualBasicAttributeData), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Me.RecordPresenceOfBadAttributes(attributesToStore)
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				attributesBag.SetAttributes(attributesToStore)
				Return
			End If
			SyncLock Me._diagnosticLock
				If (Not attributesBag.IsSealed) Then
					Me.StoreDeclarationDiagnostics(diagBag)
					If (Not attributesBag.SetAttributes(attributesToStore) AndAlso Not SourceModuleSymbol.IsEmptyIgnoringLazyDiagnostics(diagBag)) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
			End SyncLock
		End Sub

		Friend Sub AtomicStoreIntegerAndDiagnostics(ByRef variable As Integer, ByVal value As Integer, ByVal comparand As Integer, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				Interlocked.CompareExchange(variable, value, comparand)
				Return
			End If
			SyncLock Me._diagnosticLock
				If (variable = comparand) Then
					Me.StoreDeclarationDiagnostics(diagBag)
					If (Interlocked.CompareExchange(variable, value, comparand) <> comparand AndAlso Not SourceModuleSymbol.IsEmptyIgnoringLazyDiagnostics(diagBag)) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
			End SyncLock
		End Sub

		Friend Function AtomicStoreReferenceAndDiagnostics(Of T As Class)(ByRef variable As T, ByVal value As T, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal comparand As T = Nothing) As Boolean
			Dim flag As Boolean
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				flag = If(Interlocked.CompareExchange(Of T)(variable, value, comparand) <> comparand, False, comparand Is Nothing)
			Else
				Dim flag1 As Boolean = False
				SyncLock Me._diagnosticLock
					If (variable = comparand) Then
						Me.StoreDeclarationDiagnostics(diagBag)
						flag1 = Interlocked.CompareExchange(Of T)(variable, value, comparand) = comparand
						If (Not flag1 AndAlso Not SourceModuleSymbol.IsEmptyIgnoringLazyDiagnostics(diagBag)) Then
							Throw ExceptionUtilities.Unreachable
						End If
					End If
				End SyncLock
				flag = If(Not flag1, False, comparand Is Nothing)
			End If
			Return flag
		End Function

		Private Function BindImports(ByVal cancellationToken As System.Threading.CancellationToken) As SourceModuleSymbol.BoundImports
			Dim boundImport As SourceModuleSymbol.BoundImports
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.Diagnostic) = Nothing
			Dim strs As Dictionary(Of String, XmlNamespaceAndImportsClausePosition)
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag()
			Dim namespaceOrTypeSymbols As HashSet(Of NamespaceOrTypeSymbol) = New HashSet(Of NamespaceOrTypeSymbol)()
			Dim strs1 As Dictionary(Of String, AliasAndImportsClausePosition) = New Dictionary(Of String, AliasAndImportsClausePosition)(CaseInsensitiveComparison.Comparer)
			Dim instance As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition) = ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition).GetInstance()
			Dim globalImportInfos As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo) = ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo).GetInstance()
			Dim aliasAndImportsClausePositions As ArrayBuilder(Of AliasAndImportsClausePosition) = ArrayBuilder(Of AliasAndImportsClausePosition).GetInstance()
			Dim instance1 As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo) = ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo).GetInstance()
			Dim strs2 As Dictionary(Of String, XmlNamespaceAndImportsClausePosition) = New Dictionary(Of String, XmlNamespaceAndImportsClausePosition)()
			Dim bindingDiagnosticBag1 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
			Try
				Dim enumerator1 As ImmutableArray(Of GlobalImport).Enumerator = Me.Options.GlobalImports.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As GlobalImport = enumerator1.Current
					cancellationToken.ThrowIfCancellationRequested()
					bindingDiagnosticBag1.Clear()
					Dim moduleImportDatum As SourceModuleSymbol.ModuleImportData = New SourceModuleSymbol.ModuleImportData(current, namespaceOrTypeSymbols, strs1, instance, globalImportInfos, aliasAndImportsClausePositions, instance1, strs2, bindingDiagnosticBag1.DependenciesBag)
					BinderBuilder.CreateBinderForProjectImports(Me, VisualBasicSyntaxTree.Dummy).BindImportClause(current.Clause, moduleImportDatum, bindingDiagnosticBag1.DiagnosticBag)
					Using enumerator
						enumerator = bindingDiagnosticBag1.DiagnosticBag.AsEnumerable().GetEnumerator()
						While enumerator.MoveNext()
							Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic = enumerator.Current
							If (diagnostic.Code = 31051) Then
								Continue While
							End If
							bindingDiagnosticBag.Add(current.MapDiagnostic(diagnostic))
						End While
					End Using
					bindingDiagnosticBag.AddDependencies(bindingDiagnosticBag1, False)
				End While
				Dim immutable As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition) = instance.ToImmutable()
				Dim immutable1 As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo) = globalImportInfos.ToImmutable()
				Dim strs3 As Dictionary(Of String, AliasAndImportsClausePosition) = strs1
				Dim aliasAndImportsClausePositions1 As ImmutableArray(Of AliasAndImportsClausePosition) = aliasAndImportsClausePositions.ToImmutable()
				Dim globalImportInfos1 As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo) = instance1.ToImmutable()
				If (strs2.Count > 0) Then
					strs = strs2
				Else
					strs = Nothing
				End If
				boundImport = New SourceModuleSymbol.BoundImports(immutable, immutable1, strs3, aliasAndImportsClausePositions1, globalImportInfos1, strs, bindingDiagnosticBag)
			Finally
				instance.Free()
				globalImportInfos.Free()
				aliasAndImportsClausePositions.Free()
				instance1.Free()
				bindingDiagnosticBag1.Free()
			End Try
			Return boundImport
		End Function

		Private Function CreateQuickAttributeChecker() As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Dim quickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker = New Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker()
			quickAttributeChecker.AddName(AttributeDescription.CaseInsensitiveExtensionAttribute.Name, QuickAttributes.Extension)
			quickAttributeChecker.AddName(AttributeDescription.ObsoleteAttribute.Name, QuickAttributes.Obsolete)
			quickAttributeChecker.AddName(AttributeDescription.DeprecatedAttribute.Name, QuickAttributes.Obsolete)
			quickAttributeChecker.AddName(AttributeDescription.ExperimentalAttribute.Name, QuickAttributes.Obsolete)
			quickAttributeChecker.AddName(AttributeDescription.MyGroupCollectionAttribute.Name, QuickAttributes.MyGroupCollection)
			quickAttributeChecker.AddName(AttributeDescription.TypeIdentifierAttribute.Name, QuickAttributes.TypeIdentifier)
			Dim enumerator As ImmutableArray(Of GlobalImport).Enumerator = Me.Options.GlobalImports.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As GlobalImport = enumerator.Current
				If (current.Clause.Kind() <> SyntaxKind.SimpleImportsClause) Then
					Continue While
				End If
				Dim clause As SimpleImportsClauseSyntax = DirectCast(current.Clause, SimpleImportsClauseSyntax)
				If (clause.[Alias] Is Nothing) Then
					Continue While
				End If
				quickAttributeChecker.AddAlias(clause)
			End While
			quickAttributeChecker.Seal()
			Return quickAttributeChecker
		End Function

		Friend Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			If (attribute.IsTargetAttribute(Me, AttributeDescription.DefaultCharSetAttribute)) Then
				Dim constructorArgument As CharSet = attribute.GetConstructorArgument(Of CharSet)(0, SpecialType.System_Enum)
				If (CommonModuleWellKnownAttributeData.IsValidCharSet(constructorArgument)) Then
					arguments.GetOrCreateData(Of CommonModuleWellKnownAttributeData)().DefaultCharacterSet = constructorArgument
				Else
					Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
					Dim argumentSyntaxes As SeparatedSyntaxList(Of ArgumentSyntax) = arguments.AttributeSyntaxOpt.ArgumentList.Arguments
					diagnostics.Add(ERRID.ERR_BadAttribute1, argumentSyntaxes(0).GetLocation(), New [Object]() { attribute.AttributeClass })
				End If
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.DebuggableAttribute)) Then
				arguments.GetOrCreateData(Of CommonModuleWellKnownAttributeData)().HasDebuggableAttribute = True
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Private Sub EnsureImportsAreBound(ByVal cancellationToken As System.Threading.CancellationToken)
			If (Me._lazyBoundImports Is Nothing AndAlso Interlocked.CompareExchange(Of SourceModuleSymbol.BoundImports)(Me._lazyBoundImports, Me.BindImports(cancellationToken), Nothing) Is Nothing) Then
				Me.ValidateImports(Me._lazyBoundImports.MemberImports, Me._lazyBoundImports.MemberImportsInfo, Me._lazyBoundImports.AliasImports, Me._lazyBoundImports.AliasImportsInfo, Me._lazyBoundImports.Diagnostics)
			End If
		End Sub

		Private Sub EnsureLinkedAssembliesAreValidated(ByVal cancellationToken As System.Threading.CancellationToken)
			If (Me._lazyLinkedAssemblyDiagnostics.IsDefault) Then
				Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
				Me.ValidateLinkedAssemblies(instance, cancellationToken)
				ImmutableInterlocked.InterlockedInitialize(Of Diagnostic)(Me._lazyLinkedAssemblyDiagnostics, instance.ToReadOnlyAndFree())
			End If
		End Sub

		Friend Sub GetAllDeclarationErrors(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken, ByRef hasExtensionMethods As Boolean)
			Dim variable As SourceModuleSymbol._Closure$__78-1 = Nothing
			Dim variable1 As SourceModuleSymbol._Closure$__78-0 = Nothing
			Dim enumerator As IEnumerator(Of SyntaxTree) = Nothing
			Dim enumerator1 As IEnumerator(Of SyntaxTree) = Nothing
			Dim tasks As ConcurrentStack(Of System.Threading.Tasks.Task)
			variable = New SourceModuleSymbol._Closure$__78-1(variable) With
			{
				.$VB$Me = Me,
				.$VB$Local_cancellationToken = cancellationToken
			}
			Me.EnsureImportsAreBound(variable.$VB$Local_cancellationToken)
			If (Not Me.ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild) Then
				Try
					enumerator = Me.SyntaxTrees.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As SyntaxTree = enumerator.Current
						variable.$VB$Local_cancellationToken.ThrowIfCancellationRequested()
						Me.TryGetSourceFile(current).GenerateAllDeclarationErrors()
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			Else
				variable1 = New SourceModuleSymbol._Closure$__78-0(variable1) With
				{
					.$VB$NonLocal_$VB$Closure_2 = variable,
					.$VB$Local_trees = ArrayBuilder(Of SyntaxTree).GetInstance()
				}
				variable1.$VB$Local_trees.AddRange(Me.SyntaxTrees)
				RoslynParallel.[For](0, variable1.$VB$Local_trees.Count, UICultureUtilities.WithCurrentUICulture(Of Integer)(Sub(i As Integer) Me.$VB$NonLocal_$VB$Closure_2.$VB$Me.TryGetSourceFile(Me.$VB$Local_trees(i)).GenerateAllDeclarationErrors()), variable1.$VB$NonLocal_$VB$Closure_2.$VB$Local_cancellationToken)
				variable1.$VB$Local_trees.Free()
			End If
			Me.GetAttributes()
			Me.ContainingAssembly.GetAttributes()
			Me.EnsureLinkedAssembliesAreValidated(variable.$VB$Local_cancellationToken)
			If (Me.ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild) Then
				tasks = New ConcurrentStack(Of System.Threading.Tasks.Task)()
			Else
				tasks = Nothing
			End If
			Dim tasks1 As ConcurrentStack(Of System.Threading.Tasks.Task) = tasks
			Me.VisitAllSourceTypesAndNamespaces(Sub(typeOrNamespace As NamespaceOrTypeSymbol)
				If (typeOrNamespace.IsNamespace) Then
					DirectCast(typeOrNamespace, SourceNamespaceSymbol).GenerateDeclarationErrors(Me.$VB$Local_cancellationToken)
					Return
				End If
				DirectCast(typeOrNamespace, NamedTypeSymbol).GenerateDeclarationErrors(Me.$VB$Local_cancellationToken)
			End Sub, tasks1, variable.$VB$Local_cancellationToken)
			If (tasks1 IsNot Nothing) Then
				Dim task As System.Threading.Tasks.Task = Nothing
				While tasks1.TryPop(task)
					task.GetAwaiter().GetResult()
				End While
			End If
			If (Me._lazyContainsExtensionMethods = 0) Then
				Me._lazyContainsExtensionMethods = 1
			End If
			hasExtensionMethods = Me._lazyContainsExtensionMethods = 2
			diagnostics.AddRange(Me._diagnosticBagDeclare)
			diagnostics.AddRange(Me._lazyBoundImports.Diagnostics, True)
			diagnostics.AddRange(Of Diagnostic)(Me._lazyLinkedAssemblyDiagnostics)
			Using enumerator1
				enumerator1 = Me.SyntaxTrees.GetEnumerator()
				While enumerator1.MoveNext()
					diagnostics.AddRange(Me.TryGetSourceFile(enumerator1.Current).DeclarationDiagnostics)
				End While
			End Using
		End Sub

		Friend Function GetAssembliesToEmbedTypesFrom() As ImmutableArray(Of AssemblySymbol)
			If (Me._lazyAssembliesToEmbedTypesFrom.IsDefault) Then
				Dim instance As ArrayBuilder(Of AssemblySymbol) = ArrayBuilder(Of AssemblySymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = MyBase.GetReferencedAssemblySymbols().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AssemblySymbol = enumerator.Current
					If (Not current.IsLinked) Then
						Continue While
					End If
					instance.Add(current)
				End While
				ImmutableInterlocked.InterlockedInitialize(Of AssemblySymbol)(Me._lazyAssembliesToEmbedTypesFrom, instance.ToImmutableAndFree())
			End If
			Return Me._lazyAssembliesToEmbedTypesFrom
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Private Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributesBag Is Nothing OrElse Not Me._lazyCustomAttributesBag.IsSealed) Then
				Dim attributeDeclarations As ImmutableArray(Of SyntaxList(Of AttributeListSyntax)) = DirectCast(Me.ContainingAssembly, SourceAssemblySymbol).GetAttributeDeclarations()
				MyBase.LoadAndValidateAttributes(OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(attributeDeclarations), Me._lazyCustomAttributesBag, AttributeLocation.None)
			End If
			Return Me._lazyCustomAttributesBag
		End Function

		Friend Function GetDeclarationErrorsInTree(ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal locationFilter As Func(Of IEnumerable(Of Diagnostic), SyntaxTree, Nullable(Of TextSpan), IEnumerable(Of Diagnostic)), ByVal cancellationToken As System.Threading.CancellationToken) As ImmutableArray(Of Diagnostic)
			Dim variable As SourceModuleSymbol._Closure$__77-0 = Nothing
			Dim enumerator As IEnumerator(Of Diagnostic) = Nothing
			Dim tasks As ConcurrentStack(Of System.Threading.Tasks.Task)
			variable = New SourceModuleSymbol._Closure$__77-0(variable) With
			{
				.$VB$Local_tree = tree,
				.$VB$Local_filterSpanWithinTree = filterSpanWithinTree,
				.$VB$Local_cancellationToken = cancellationToken
			}
			Dim instance As ArrayBuilder(Of Diagnostic) = ArrayBuilder(Of Diagnostic).GetInstance()
			Dim sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile = Me.TryGetSourceFile(variable.$VB$Local_tree)
			If (Not variable.$VB$Local_filterSpanWithinTree.HasValue) Then
				sourceFile.GenerateAllDeclarationErrors()
			Else
				Dim declarationErrorsInSpan As IEnumerable(Of Diagnostic) = sourceFile.GetDeclarationErrorsInSpan(variable.$VB$Local_filterSpanWithinTree.Value, variable.$VB$Local_cancellationToken)
				declarationErrorsInSpan = locationFilter(declarationErrorsInSpan, variable.$VB$Local_tree, variable.$VB$Local_filterSpanWithinTree)
				instance.AddRange(declarationErrorsInSpan)
			End If
			Me.GetAttributes()
			Me.ContainingAssembly.GetAttributes()
			If (Me.ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild) Then
				tasks = New ConcurrentStack(Of System.Threading.Tasks.Task)()
			Else
				tasks = Nothing
			End If
			Dim tasks1 As ConcurrentStack(Of System.Threading.Tasks.Task) = tasks
			Me.VisitAllSourceTypesAndNamespaces(Sub(typeOrNamespace As NamespaceOrTypeSymbol)
			If (typeOrNamespace.IsDefinedInSourceTree(Me.$VB$Local_tree, Me.$VB$Local_filterSpanWithinTree, New System.Threading.CancellationToken())) Then
				If (typeOrNamespace.IsNamespace) Then
					DirectCast(typeOrNamespace, SourceNamespaceSymbol).GenerateDeclarationErrorsInTree(Me.$VB$Local_tree, Me.$VB$Local_filterSpanWithinTree, Me.$VB$Local_cancellationToken)
					Return
				End If
				DirectCast(typeOrNamespace, NamedTypeSymbol).GenerateDeclarationErrors(Me.$VB$Local_cancellationToken)
			End If
			End Sub, tasks1, variable.$VB$Local_cancellationToken)
			If (tasks1 IsNot Nothing) Then
				Dim task As System.Threading.Tasks.Task = Nothing
				While tasks1.TryPop(task)
					task.GetAwaiter().GetResult()
				End While
			End If
			Dim diagnostics As IEnumerable(Of Diagnostic) = sourceFile.DeclarationDiagnostics.AsEnumerable()
			If (locationFilter IsNot Nothing) Then
				diagnostics = locationFilter(diagnostics, variable.$VB$Local_tree, variable.$VB$Local_filterSpanWithinTree)
			End If
			Using enumerator
				enumerator = diagnostics.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current)
				End While
			End Using
			Return instance.ToImmutableAndFree()
		End Function

		Friend Function GetDecodedWellKnownAttributeData() As CommonModuleWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.DecodedWellKnownAttributeData, CommonModuleWellKnownAttributeData)
		End Function

		Public Overrides Function GetMetadata() As ModuleMetadata
			Return Nothing
		End Function

		Public Function GetMyGroupCollectionPropertyWithDefaultInstanceAlias(ByVal classType As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol
			Dim synthesizedMyGroupCollectionPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol
			If (Me._lazyTypesWithDefaultInstanceAlias Is Nothing) Then
				Me._lazyTypesWithDefaultInstanceAlias = Me.GetTypesWithDefaultInstanceAlias()
			End If
			Dim synthesizedMyGroupCollectionPropertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol = Nothing
			If (Me._lazyTypesWithDefaultInstanceAlias = SourceModuleSymbol.s_noTypesWithDefaultInstanceAlias OrElse Not Me._lazyTypesWithDefaultInstanceAlias.TryGetValue(classType, synthesizedMyGroupCollectionPropertySymbol1)) Then
				synthesizedMyGroupCollectionPropertySymbol = Nothing
			Else
				synthesizedMyGroupCollectionPropertySymbol = synthesizedMyGroupCollectionPropertySymbol1
			End If
			Return synthesizedMyGroupCollectionPropertySymbol
		End Function

		Private Function GetTypesWithDefaultInstanceAlias() As Dictionary(Of NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol)
			Dim sNoTypesWithDefaultInstanceAlias As Dictionary(Of NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol) = Nothing
			If (Me._assemblySymbol.DeclaringCompilation.MyTemplate IsNot Nothing) Then
				SourceModuleSymbol.GetTypesWithDefaultInstanceAlias(Me.GlobalNamespace, sNoTypesWithDefaultInstanceAlias)
			End If
			If (sNoTypesWithDefaultInstanceAlias Is Nothing) Then
				sNoTypesWithDefaultInstanceAlias = SourceModuleSymbol.s_noTypesWithDefaultInstanceAlias
			End If
			Return sNoTypesWithDefaultInstanceAlias
		End Function

		Private Shared Sub GetTypesWithDefaultInstanceAlias(ByVal namespaceOrType As NamespaceOrTypeSymbol, <InAttribute> <Out> ByRef result As Dictionary(Of NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol))
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = namespaceOrType.GetMembersUnordered().GetEnumerator()
		Label0:
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				Select Case current.Kind
					Case SymbolKind.NamedType
						Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
						If (sourceNamedTypeSymbol Is Nothing) Then
							Continue While
						End If
						Dim enumerator1 As ImmutableArray(Of SyntaxReference).Enumerator = sourceNamedTypeSymbol.SyntaxReferences.GetEnumerator()
						While enumerator1.MoveNext()
							If (Not enumerator1.Current.SyntaxTree.IsMyTemplate()) Then
								Continue While
							End If
							SourceModuleSymbol.GetTypesWithDefaultInstanceAlias(sourceNamedTypeSymbol, result)
							GoTo Label0
						End While
						Continue While
					Case SymbolKind.[Namespace]
						SourceModuleSymbol.GetTypesWithDefaultInstanceAlias(DirectCast(current, NamespaceSymbol), result)
						Continue While
					Case SymbolKind.[Property]
						If (Not current.IsMyGroupCollectionProperty) Then
							Continue While
						End If
						Dim synthesizedMyGroupCollectionPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol)
						If (synthesizedMyGroupCollectionPropertySymbol.DefaultInstanceAlias.Length <= 0) Then
							Continue While
						End If
						Dim type As NamedTypeSymbol = DirectCast(synthesizedMyGroupCollectionPropertySymbol.Type, NamedTypeSymbol)
						If (result Is Nothing) Then
							result = New Dictionary(Of NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol)(ReferenceEqualityComparer.Instance)
						ElseIf (result.ContainsKey(type)) Then
							result(type) = Nothing
							Continue While
						End If
						result.Add(type, synthesizedMyGroupCollectionPropertySymbol)
						Continue While
					Case Else
						Continue While
				End Select
			End While
		End Sub

		Private Shared Function HasAllLazyDiagnostics(ByVal diagBag As DiagnosticBag) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of Diagnostic) = Nothing
			Using enumerator
				enumerator = diagBag.AsEnumerable().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As DiagnosticWithInfo = TryCast(enumerator.Current, DiagnosticWithInfo)
					If (current IsNot Nothing AndAlso current.HasLazyInfo) Then
						Continue While
					End If
					flag = False
					Return flag
				End While
			End Using
			flag = True
			Return flag
		End Function

		Private Shared Function IsEmptyIgnoringLazyDiagnostics(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			If (Not diagBag.DependenciesBag.IsNullOrEmpty()) Then
				Return False
			End If
			If (Not diagBag.AccumulatesDiagnostics) Then
				Return True
			End If
			Return SourceModuleSymbol.HasAllLazyDiagnostics(diagBag.DiagnosticBag)
		End Function

		Friend Function MightContainNoPiaLocalTypes() As Boolean
			If (Me.AnyReferencedAssembliesAreLinked) Then
				Return True
			End If
			Return Me.ContainsExplicitDefinitionOfNoPiaLocalTypes
		End Function

		Private Shared Function NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(ByVal ns As NamespaceSymbol) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = ns.GetMembersUnordered().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					Dim kind As SymbolKind = current.Kind
					If (kind <> SymbolKind.NamedType) Then
						If (kind = SymbolKind.[Namespace] AndAlso SourceModuleSymbol.NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(DirectCast(current, NamespaceSymbol))) Then
							flag = True
							Exit While
						End If
					ElseIf (DirectCast(current, NamedTypeSymbol).IsExplicitDefinitionOfNoPiaLocalType) Then
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

		Private Sub RecordPresenceOfBadAttributes(ByVal attributes As ImmutableArray(Of VisualBasicAttributeData))
			If (Not Me._hasBadAttributes) Then
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = attributes.GetEnumerator()
				While enumerator.MoveNext()
					If (Not enumerator.Current.HasErrors) Then
						Continue While
					End If
					Me._hasBadAttributes = True
					Return
				End While
			End If
		End Sub

		Friend Sub RecordPresenceOfExtensionMethods()
			Me._lazyContainsExtensionMethods = 2
		End Sub

		Private Sub StoreDeclarationDiagnostics(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As IEnumerator(Of Diagnostic) = Nothing
			Dim nullable As Nullable(Of Boolean)
			If (Not diagBag.IsEmpty) Then
				Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = diagBag.DiagnosticBag
				If (diagnosticBag IsNot Nothing) Then
					nullable = New Nullable(Of Boolean)(diagnosticBag.IsEmptyWithoutResolution)
				Else
					nullable = Nothing
				End If
				Dim nullable1 As Nullable(Of Boolean) = nullable
				nullable1 = If(nullable1.HasValue, New Nullable(Of Boolean)(Not nullable1.GetValueOrDefault()), nullable1)
				If (nullable1.GetValueOrDefault()) Then
					Try
						enumerator = diagBag.DiagnosticBag.AsEnumerableWithoutResolution().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Diagnostic = enumerator.Current
							Dim location As Microsoft.CodeAnalysis.Location = current.Location
							If (Not location.IsInSource) Then
								Me._diagnosticBagDeclare.Add(current)
							Else
								Dim sourceTree As VisualBasicSyntaxTree = DirectCast(location.SourceTree, VisualBasicSyntaxTree)
								Me.TryGetSourceFile(sourceTree).DeclarationDiagnostics.Add(current)
							End If
						End While
					Finally
						If (enumerator IsNot Nothing) Then
							enumerator.Dispose()
						End If
					End Try
				End If
				If (Not diagBag.DependenciesBag.IsNullOrEmpty()) Then
					Me.DeclaringCompilation.AddUsedAssemblies(diagBag.DependenciesBag)
				End If
			End If
		End Sub

		Friend Function TryGetSourceFile(ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile
			Dim sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile = Nothing
			If (Me._sourceFileMap.TryGetValue(tree, sourceFile)) Then
				orAdd = sourceFile
			ElseIf (Not Me._assemblySymbol.DeclaringCompilation.AllSyntaxTrees.Contains(tree)) Then
				orAdd = Nothing
			Else
				sourceFile = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile(Me, tree)
				orAdd = Me._sourceFileMap.GetOrAdd(tree, sourceFile)
			End If
			Return orAdd
		End Function

		Private Sub ValidateImport(ByVal type As TypeSymbol, ByVal info As SourceModuleSymbol.GlobalImportInfo, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
			Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
			type.CheckAllConstraints(instance, typeParameterDiagnosticInfos, New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, Me.ContainingAssembly))
			If (typeParameterDiagnosticInfos IsNot Nothing) Then
				instance.AddRange(typeParameterDiagnosticInfos)
			End If
			Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterDiagnosticInfo = enumerator.Current
				If (current.UseSiteInfo.DiagnosticInfo IsNot Nothing) Then
					diagnostics.Add(info.Import.MapDiagnostic(New VBDiagnostic(current.UseSiteInfo.DiagnosticInfo, info.SyntaxReference.GetLocation(), False)))
				End If
				diagnostics.AddDependencies(current.UseSiteInfo)
			End While
			instance.Free()
		End Sub

		Private Sub ValidateImports(ByVal memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), ByVal memberImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo), ByVal aliasImports As ImmutableArray(Of AliasAndImportsClausePosition), ByVal aliasImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not memberImports.IsDefault) Then
				Dim length As Integer = memberImports.Length - 1
				For i As Integer = 0 To length
					Dim namespaceOrType As NamespaceOrTypeSymbol = memberImports(i).NamespaceOrType
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(namespaceOrType, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
					If (typeSymbol Is Nothing) Then
						diagnostics.AddAssembliesUsedByNamespaceReference(DirectCast(namespaceOrType, NamespaceSymbol))
					Else
						Me.ValidateImport(typeSymbol, memberImportsInfo(i), diagnostics)
					End If
				Next

			End If
			If (Not aliasImports.IsDefault) Then
				Dim num As Integer = aliasImports.Length - 1
				For j As Integer = 0 To num
					Dim target As NamespaceOrTypeSymbol = aliasImports(j).[Alias].Target
					Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(target, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
					If (typeSymbol1 Is Nothing) Then
						diagnostics.AddAssembliesUsedByNamespaceReference(DirectCast(target, NamespaceSymbol))
					Else
						Me.ValidateImport(typeSymbol1, aliasImportsInfo(j), diagnostics)
					End If
				Next

			End If
		End Sub

		Private Sub ValidateLinkedAssemblies(ByVal diagnostics As DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = MyBase.GetReferencedAssemblySymbols().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AssemblySymbol = enumerator.Current
				cancellationToken.ThrowIfCancellationRequested()
				If (current.IsMissing OrElse Not current.IsLinked) Then
					Continue While
				End If
				Dim flag As Boolean = False
				Dim flag1 As Boolean = False
				Dim enumerator1 As ImmutableArray(Of VisualBasicAttributeData).Enumerator = current.GetAttributes().GetEnumerator()
				Do
					If (Not enumerator1.MoveNext()) Then
						Exit Do
					End If
					Dim visualBasicAttributeDatum As VisualBasicAttributeData = enumerator1.Current
					If (visualBasicAttributeDatum.IsTargetAttribute(current, AttributeDescription.GuidAttribute)) Then
						If (visualBasicAttributeDatum.CommonConstructorArguments.Length <> 1) Then
							Continue Do
						End If
						Dim item As TypedConstant = visualBasicAttributeDatum.CommonConstructorArguments(0)
						Dim objectValue As Object = RuntimeHelpers.GetObjectValue(item.ValueInternal)
						If (objectValue IsNot Nothing AndAlso Not TypeOf objectValue Is [String]) Then
							Continue Do
						End If
						flag = True
					ElseIf (Not visualBasicAttributeDatum.IsTargetAttribute(current, AttributeDescription.ImportedFromTypeLibAttribute)) Then
						If (Not visualBasicAttributeDatum.IsTargetAttribute(current, AttributeDescription.PrimaryInteropAssemblyAttribute) OrElse visualBasicAttributeDatum.CommonConstructorArguments.Length <> 2) Then
							Continue Do
						End If
						flag1 = True
					Else
						If (visualBasicAttributeDatum.CommonConstructorArguments.Length <> 1) Then
							Continue Do
						End If
						flag1 = True
					End If
				Loop While Not flag OrElse Not flag1
				If (Not flag) Then
					diagnostics.Add(ERRID.ERR_PIAHasNoAssemblyGuid1, NoLocation.Singleton, New [Object]() { current, AttributeDescription.GuidAttribute.FullName })
				End If
				If (flag1) Then
					Continue While
				End If
				diagnostics.Add(ERRID.ERR_PIAHasNoTypeLibAttribute1, NoLocation.Singleton, New [Object]() { current, AttributeDescription.ImportedFromTypeLibAttribute.FullName, AttributeDescription.PrimaryInteropAssemblyAttribute.FullName })
			End While
		End Sub

		Private Sub VisitAllSourceTypesAndNamespaces(ByVal visitor As Action(Of NamespaceOrTypeSymbol), ByVal tasks As ConcurrentStack(Of Task), ByVal cancellationToken As System.Threading.CancellationToken)
			Me.VisitTypesAndNamespacesWithin(Me.GlobalNamespace, visitor, tasks, cancellationToken)
		End Sub

		Private Sub VisitTypesAndNamespacesWithin(ByVal ns As NamespaceOrTypeSymbol, ByVal visitor As Action(Of NamespaceOrTypeSymbol), ByVal tasks As ConcurrentStack(Of System.Threading.Tasks.Task), ByVal cancellationToken As System.Threading.CancellationToken)
			Dim variable As SourceModuleSymbol._Closure$__80-0 = Nothing
			Dim variable1 As SourceModuleSymbol._Closure$__80-1 = Nothing
			Dim enumerator As IEnumerator(Of Symbol) = Nothing
			variable = New SourceModuleSymbol._Closure$__80-0(variable) With
			{
				.$VB$Local_visitor = visitor
			}
			Dim instance As ArrayBuilder(Of NamespaceOrTypeSymbol) = ArrayBuilder(Of NamespaceOrTypeSymbol).GetInstance()
			Try
				instance.Push(ns)
				While instance.Count > 0
					variable1 = New SourceModuleSymbol._Closure$__80-1(variable1) With
					{
						.$VB$NonLocal_$VB$Closure_2 = variable
					}
					cancellationToken.ThrowIfCancellationRequested()
					variable1.$VB$Local_symbol = instance.Pop()
					If (tasks Is Nothing) Then
						variable1.$VB$NonLocal_$VB$Closure_2.$VB$Local_visitor(variable1.$VB$Local_symbol)
					Else
						Dim task As System.Threading.Tasks.Task = System.Threading.Tasks.Task.Run(UICultureUtilities.WithCurrentUICulture(Sub()
						Try
							Me.$VB$NonLocal_$VB$Closure_2.$VB$Local_visitor(Me.$VB$Local_symbol)
						Catch exception As System.Exception When FatalError.ReportAndPropagateUnlessCanceled(exception)
							Throw ExceptionUtilities.Unreachable
						End Try
						End Sub), cancellationToken)
						tasks.Push(task)
					End If
					Using enumerator
						enumerator = If(variable1.$VB$Local_symbol.IsNamespace, DirectCast(variable1.$VB$Local_symbol.GetMembers(), IEnumerable(Of Symbol)), DirectCast(variable1.$VB$Local_symbol.GetTypeMembers(), IEnumerable).Cast(Of Symbol)()).GetEnumerator()
						While enumerator.MoveNext()
							instance.Push(DirectCast(enumerator.Current, NamespaceOrTypeSymbol))
						End While
					End Using
				End While
			Finally
				instance.Free()
			End Try
		End Sub

		Private NotInheritable Class BoundImports
			Public ReadOnly MemberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)

			Public ReadOnly MemberImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo)

			Public ReadOnly AliasImportsMap As Dictionary(Of String, AliasAndImportsClausePosition)

			Public ReadOnly AliasImports As ImmutableArray(Of AliasAndImportsClausePosition)

			Public ReadOnly AliasImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo)

			Public ReadOnly XmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition)

			Public ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Public Sub New(ByVal memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), ByVal memberImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo), ByVal aliasImportsMap As Dictionary(Of String, AliasAndImportsClausePosition), ByVal aliasImports As ImmutableArray(Of AliasAndImportsClausePosition), ByVal aliasImportsInfo As ImmutableArray(Of SourceModuleSymbol.GlobalImportInfo), ByVal xmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition), ByVal diags As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New()
				Me.MemberImports = memberImports
				Me.MemberImportsInfo = memberImportsInfo
				Me.AliasImportsMap = aliasImportsMap
				Me.AliasImports = aliasImports
				Me.AliasImportsInfo = aliasImportsInfo
				Me.XmlNamespaces = xmlNamespaces
				Me.Diagnostics = diags
			End Sub
		End Class

		Private Structure GlobalImportInfo
			Public ReadOnly Import As GlobalImport

			Public ReadOnly SyntaxReference As SyntaxReference

			Public Sub New(ByVal import As GlobalImport, ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference)
				Me = New SourceModuleSymbol.GlobalImportInfo() With
				{
					.Import = import,
					.SyntaxReference = syntaxReference
				}
			End Sub
		End Structure

		Private NotInheritable Class ModuleImportData
			Inherits ImportData
			Private ReadOnly _globalImport As GlobalImport

			Private ReadOnly _membersBuilder As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition)

			Private ReadOnly _membersInfoBuilder As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo)

			Private ReadOnly _aliasesBuilder As ArrayBuilder(Of AliasAndImportsClausePosition)

			Private ReadOnly _aliasesInfoBuilder As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo)

			Private ReadOnly _dependencies As ICollection(Of AssemblySymbol)

			Public Sub New(ByVal globalImport As Microsoft.CodeAnalysis.VisualBasic.GlobalImport, ByVal membersMap As HashSet(Of NamespaceOrTypeSymbol), ByVal aliasesMap As Dictionary(Of String, AliasAndImportsClausePosition), ByVal membersBuilder As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition), ByVal membersInfoBuilder As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo), ByVal aliasesBuilder As ArrayBuilder(Of AliasAndImportsClausePosition), ByVal aliasesInfoBuilder As ArrayBuilder(Of SourceModuleSymbol.GlobalImportInfo), ByVal xmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition), ByVal dependencies As ICollection(Of AssemblySymbol))
				MyBase.New(membersMap, aliasesMap, xmlNamespaces)
				Me._globalImport = globalImport
				Me._membersBuilder = membersBuilder
				Me._membersInfoBuilder = membersInfoBuilder
				Me._aliasesBuilder = aliasesBuilder
				Me._aliasesInfoBuilder = aliasesInfoBuilder
				Me._dependencies = dependencies
			End Sub

			Public Overrides Sub AddAlias(ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal [alias] As AliasSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
				Dim aliasAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition([alias], importsClausePosition, ImmutableArray(Of AssemblySymbol).Empty)
				Me.Aliases.Add(name, aliasAndImportsClausePosition)
				Me._aliasesBuilder.Add(aliasAndImportsClausePosition)
				Me._aliasesInfoBuilder.Add(New SourceModuleSymbol.GlobalImportInfo(Me._globalImport, syntaxRef))
				Me.AddDependencies(dependencies)
			End Sub

			Private Sub AddDependencies(ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
				Dim enumerator As IEnumerator(Of AssemblySymbol) = Nothing
				Try
					enumerator = dependencies.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As AssemblySymbol = enumerator.Current
						Me._dependencies.Add(current)
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End Sub

			Public Overrides Sub AddMember(ByVal syntaxRef As SyntaxReference, ByVal member As NamespaceOrTypeSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
				Dim namespaceOrTypeAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.NamespaceOrTypeAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.NamespaceOrTypeAndImportsClausePosition(member, importsClausePosition, ImmutableArray(Of AssemblySymbol).Empty)
				Me.Members.Add(member)
				Me._membersBuilder.Add(namespaceOrTypeAndImportsClausePosition)
				Me._membersInfoBuilder.Add(New SourceModuleSymbol.GlobalImportInfo(Me._globalImport, syntaxRef))
				Me.AddDependencies(dependencies)
			End Sub
		End Class
	End Class
End Namespace