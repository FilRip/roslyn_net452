Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class PEModuleBuilder
		Inherits PEModuleBuilder(Of VisualBasicCompilation, SourceModuleSymbol, AssemblySymbol, TypeSymbol, NamedTypeSymbol, MethodSymbol, SyntaxNode, EmbeddedTypesManager, ModuleCompilationState)
		Private ReadOnly _disableJITOptimization As ConcurrentDictionary(Of MethodSymbol, Boolean)

		Private ReadOnly _metadataName As String

		Private _lazyExportedTypes As ImmutableArray(Of ExportedType)

		Private ReadOnly _lazyNumberOfTypesFromOtherModules As Integer

		Private _lazyTranslatedImports As ImmutableArray(Of UsedNamespaceOrType)

		Private _lazyDefaultNamespace As String

		Protected ReadOnly m_AssemblyOrModuleSymbolToModuleRefMap As ConcurrentDictionary(Of Symbol, IModuleReference)

		Private ReadOnly _genericInstanceMap As ConcurrentDictionary(Of Symbol, Object)

		Private ReadOnly _reportedErrorTypesMap As ConcurrentSet(Of TypeSymbol)

		Private ReadOnly _embeddedTypesManagerOpt As EmbeddedTypesManager

		Private _addedEmbeddedSymbols As ConcurrentSet(Of Symbol)

		Friend MustOverride ReadOnly Property AllowOmissionOfConditionalCalls As Boolean

		Friend NotOverridable Overrides ReadOnly Property CorLibrary As AssemblySymbol
			Get
				Return Me.SourceModule.ContainingSourceAssembly.CorLibrary
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DefaultNamespace As String
			Get
				Dim empty As String
				If (Me._lazyDefaultNamespace Is Nothing) Then
					Dim rootNamespace As NamespaceSymbol = Me.SourceModule.RootNamespace
					If (Not rootNamespace.IsGlobalNamespace) Then
						Me._lazyDefaultNamespace = rootNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
						empty = Me._lazyDefaultNamespace
					Else
						empty = [String].Empty
					End If
				Else
					empty = Me._lazyDefaultNamespace
				End If
				Return empty
			End Get
		End Property

		Public Overrides ReadOnly Property EmbeddedTypesManagerOpt As EmbeddedTypesManager
			Get
				Return Me._embeddedTypesManagerOpt
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property GenerateVisualBasicStylePdb As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overridable ReadOnly Property IgnoreAccessibility As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property LinkedAssembliesDebugInfo As IEnumerable(Of String)
			Get
				Dim isLinked As Func(Of AssemblySymbol, Boolean)
				Dim name As Func(Of AssemblySymbol, String)
				Dim referencedAssemblySymbols As ImmutableArray(Of AssemblySymbol) = Me.SourceModule.ReferencedAssemblySymbols
				If (PEModuleBuilder._Closure$__.$I18-0 Is Nothing) Then
					isLinked = Function(a As AssemblySymbol) a.IsLinked
					PEModuleBuilder._Closure$__.$I18-0 = isLinked
				Else
					isLinked = PEModuleBuilder._Closure$__.$I18-0
				End If
				Dim assemblySymbols As IEnumerable(Of AssemblySymbol) = referencedAssemblySymbols.Where(isLinked)
				If (PEModuleBuilder._Closure$__.$I18-1 Is Nothing) Then
					name = Function(a As AssemblySymbol) a.Name
					PEModuleBuilder._Closure$__.$I18-1 = name
				Else
					name = PEModuleBuilder._Closure$__.$I18-1
				End If
				Return assemblySymbols.[Select](Of String)(name)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ModuleName As String
			Get
				Return Me._metadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._metadataName
			End Get
		End Property

		Friend Sub New(ByVal sourceModule As SourceModuleSymbol, ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal serializationProperties As ModulePropertiesForSerialization, ByVal manifestResources As IEnumerable(Of ResourceDescription))
			MyBase.New(sourceModule.ContainingSourceAssembly.DeclaringCompilation, sourceModule, serializationProperties, manifestResources, outputKind, emitOptions, New ModuleCompilationState())
			Me._disableJITOptimization = New ConcurrentDictionary(Of MethodSymbol, Boolean)(ReferenceEqualityComparer.Instance)
			Me.m_AssemblyOrModuleSymbolToModuleRefMap = New ConcurrentDictionary(Of Symbol, IModuleReference)()
			Me._genericInstanceMap = New ConcurrentDictionary(Of Symbol, Object)()
			Me._reportedErrorTypesMap = New ConcurrentSet(Of TypeSymbol)()
			Me._addedEmbeddedSymbols = Nothing
			Dim metadataName As String = sourceModule.MetadataName
			Me._metadataName = If(EmbeddedOperators.CompareString(metadataName, "?", False) <> 0, metadataName, If(emitOptions.OutputNameOverride, metadataName))
			Me.m_AssemblyOrModuleSymbolToModuleRefMap.Add(sourceModule, Me)
			If (sourceModule.AnyReferencedAssembliesAreLinked) Then
				Me._embeddedTypesManagerOpt = New EmbeddedTypesManager(Me)
			End If
		End Sub

		Private Sub AddSymbolLocation(ByVal result As MultiDictionary(Of DebugSourceDocument, DefinitionWithLocation), ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim smallestSourceLocationOrNull As Location = Me.GetSmallestSourceLocationOrNull(symbol)
			If (smallestSourceLocationOrNull IsNot Nothing) Then
				Me.AddSymbolLocation(result, smallestSourceLocationOrNull, DirectCast(symbol.GetCciAdapter(), IDefinition))
			End If
		End Sub

		Private Sub AddSymbolLocation(ByVal result As MultiDictionary(Of Microsoft.Cci.DebugSourceDocument, DefinitionWithLocation), ByVal location As Microsoft.CodeAnalysis.Location, ByVal definition As IDefinition)
			Dim lineSpan As FileLinePositionSpan = location.GetLineSpan()
			Dim debugSourceDocument As Microsoft.Cci.DebugSourceDocument = Me.DebugDocumentsBuilder.TryGetDebugDocument(lineSpan.Path, location.SourceTree.FilePath)
			If (debugSourceDocument IsNot Nothing) Then
				Dim startLinePosition As LinePosition = lineSpan.StartLinePosition
				Dim line As Integer = startLinePosition.Line
				startLinePosition = lineSpan.StartLinePosition
				Dim character As Integer = startLinePosition.Character
				startLinePosition = lineSpan.EndLinePosition
				Dim num As Integer = startLinePosition.Line
				startLinePosition = lineSpan.EndLinePosition
				result.Add(debugSourceDocument, New DefinitionWithLocation(definition, line, character, num, startLinePosition.Character))
			End If
		End Sub

		Private Function CalculateExportedTypes() As ImmutableArray(Of ExportedType)
			Dim instance As ArrayBuilder(Of ExportedType) = ArrayBuilder(Of ExportedType).GetInstance()
			Dim containingSourceAssembly As SourceAssemblySymbol = Me.SourceModule.ContainingSourceAssembly
			If (Not Me.OutputKind.IsNetModule()) Then
				Dim modules As ImmutableArray(Of ModuleSymbol) = containingSourceAssembly.Modules
				Dim length As Integer = modules.Length - 1
				For i As Integer = 1 To length
					Me.GetExportedTypes(modules(i).GlobalNamespace, -1, instance)
				Next

			End If
			PEModuleBuilder.GetForwardedTypes(containingSourceAssembly, instance)
			Return instance.ToImmutableAndFree()
		End Function

		Private Sub CheckTupleUnderlyingType(ByVal namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag)
			Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics
			If ((baseTypeNoUseSiteDiagnostics Is Nothing OrElse baseTypeNoUseSiteDiagnostics.SpecialType <> SpecialType.System_ValueType) AndAlso Me._reportedErrorTypesMap.Add(namedTypeSymbol)) Then
				Dim location As Microsoft.CodeAnalysis.Location = If(syntaxNodeOpt Is Nothing, NoLocation.Singleton, syntaxNodeOpt.GetLocation())
				If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo
					If (diagnosticInfo IsNot Nothing) Then
						diagnostics.Add(diagnosticInfo, location)
						Return
					End If
				End If
				diagnostics.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_PredefinedValueTupleTypeMustBeStruct, New [Object]() { namedTypeSymbol.MetadataName }), location, False))
			End If
		End Sub

		Private Shared Function ContainerIsGeneric(ByVal container As Symbol) As Boolean
			If (container.Kind = SymbolKind.Method AndAlso DirectCast(container, MethodSymbol).IsGenericMethod) Then
				Return True
			End If
			Return container.ContainingType.IsGenericType
		End Function

		Private Function CreateParameterTypeInformationWrapper(ByVal param As ParameterSymbol) As IParameterTypeInformation
			Dim orAdd As IParameterTypeInformation
			Dim obj As Object = Nothing
			If (Not Me._genericInstanceMap.TryGetValue(param, obj)) Then
				Dim parameterTypeInformation As IParameterTypeInformation = New Microsoft.CodeAnalysis.VisualBasic.Emit.ParameterTypeInformation(param)
				orAdd = DirectCast(Me._genericInstanceMap.GetOrAdd(param, parameterTypeInformation), IParameterTypeInformation)
			Else
				orAdd = DirectCast(obj, IParameterTypeInformation)
			End If
			Return orAdd
		End Function

		Protected NotOverridable Overrides Function CreatePrivateImplementationDetailsStaticConstructor(ByVal details As PrivateImplementationDetails, ByVal syntaxOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As IMethodDefinition
			Return (New SynthesizedPrivateImplementationDetailsSharedConstructor(Me.SourceModule, details, Me.GetUntranslatedSpecialType(SpecialType.System_Void, syntaxOpt, diagnostics))).GetCciAdapter()
		End Function

		Public Overrides Function GetAdditionalTopLevelTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Return DirectCast(Me.GetAdditionalTopLevelTypes(), IEnumerable(Of INamespaceTypeDefinition))
		End Function

		Public NotOverridable Overrides Function GetAnonymousTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Dim namespaceTypeDefinitions As IEnumerable(Of INamespaceTypeDefinition)
			namespaceTypeDefinitions = If(Not context.MetadataOnly, DirectCast(Me.SourceModule.ContainingSourceAssembly.DeclaringCompilation.AnonymousTypeManager.AllCreatedTemplates, IEnumerable(Of INamespaceTypeDefinition)), SpecializedCollections.EmptyEnumerable(Of INamespaceTypeDefinition)())
			Return namespaceTypeDefinitions
		End Function

		Protected NotOverridable Overrides Function GetAssemblyReferencesFromAddedModules(ByVal diagnostics As DiagnosticBag) As IEnumerable(Of IAssemblyReference)
			Return New PEModuleBuilder.VB$StateMachine_23_GetAssemblyReferencesFromAddedModules(-2) With
			{
				.$VB$Me = Me,
				.$P_diagnostics = diagnostics
			}
		End Function

		Private Function GetCciAdapter(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Object
			Dim cciAdapter As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Object)
			Dim symbols As ConcurrentDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Object) = Me._genericInstanceMap
			Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbol
			If (PEModuleBuilder._Closure$__.$I71-0 Is Nothing) Then
				cciAdapter = Function(s As Microsoft.CodeAnalysis.VisualBasic.Symbol) s.GetCciAdapter()
				PEModuleBuilder._Closure$__.$I71-0 = cciAdapter
			Else
				cciAdapter = PEModuleBuilder._Closure$__.$I71-0
			End If
			Return symbols.GetOrAdd(symbol1, cciAdapter)
		End Function

		Protected NotOverridable Overrides Function GetCorLibraryReferenceToEmit(ByVal context As EmitContext) As IAssemblyReference
			Dim assemblyReference As IAssemblyReference
			Dim corLibrary As AssemblySymbol = Me.CorLibrary
			If (corLibrary.IsMissing OrElse corLibrary.IsLinked OrElse CObj(corLibrary) = CObj(Me.SourceModule.ContainingAssembly)) Then
				assemblyReference = Nothing
			Else
				assemblyReference = Me.Translate(corLibrary, context.Diagnostics)
			End If
			Return assemblyReference
		End Function

		Public Overrides Function GetEmbeddedTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Return DirectCast(Me.GetEmbeddedTypes(context.Diagnostics), IEnumerable(Of INamespaceTypeDefinition))
		End Function

		Public NotOverridable Overrides Function GetExportedTypes(ByVal diagnostics As DiagnosticBag) As ImmutableArray(Of ExportedType)
			If (Me._lazyExportedTypes.IsDefault) Then
				Me._lazyExportedTypes = Me.CalculateExportedTypes()
				If (Me._lazyExportedTypes.Length > 0) Then
					Me.ReportExportedTypeNameCollisions(Me._lazyExportedTypes, diagnostics)
				End If
			End If
			Return Me._lazyExportedTypes
		End Function

		Private Sub GetExportedTypes(ByVal symbol As NamespaceOrTypeSymbol, ByVal parentIndex As Integer, ByVal builder As ArrayBuilder(Of ExportedType))
			Dim count As Integer
			If (symbol.Kind <> SymbolKind.NamedType) Then
				count = -1
			Else
				If (symbol.DeclaredAccessibility <> Accessibility.[Public]) Then
					Return
				End If
				count = builder.Count
				builder.Add(New ExportedType(DirectCast(symbol, NamedTypeSymbol).GetCciAdapter(), parentIndex, False))
			End If
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceOrTypeSymbol = TryCast(enumerator.Current, NamespaceOrTypeSymbol)
				If (current Is Nothing) Then
					Continue While
				End If
				Me.GetExportedTypes(current, count, builder)
			End While
		End Sub

		Friend Shared Function GetForwardedTypes(ByVal sourceAssembly As SourceAssemblySymbol, ByVal builderOpt As ArrayBuilder(Of ExportedType)) As HashSet(Of NamedTypeSymbol)
			Dim namedTypeSymbols As HashSet(Of NamedTypeSymbol) = New HashSet(Of NamedTypeSymbol)()
			PEModuleBuilder.GetForwardedTypes(namedTypeSymbols, sourceAssembly.GetSourceDecodedWellKnownAttributeData(), builderOpt)
			If (Not sourceAssembly.DeclaringCompilation.Options.OutputKind.IsNetModule()) Then
				PEModuleBuilder.GetForwardedTypes(namedTypeSymbols, sourceAssembly.GetNetModuleDecodedWellKnownAttributeData(), builderOpt)
			End If
			Return namedTypeSymbols
		End Function

		Private Shared Sub GetForwardedTypes(ByVal seenTopLevelTypes As HashSet(Of NamedTypeSymbol), ByVal wellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), ByVal builderOpt As ArrayBuilder(Of ExportedType))
			Dim nullable As Nullable(Of Integer)
			Dim nullable1 As Nullable(Of Boolean)
			Dim enumerator As IEnumerator(Of NamedTypeSymbol) = Nothing
			Dim nullable2 As Nullable(Of Integer)
			Dim nullable3 As Nullable(Of Boolean)
			Dim displayString As Func(Of NamedTypeSymbol, String)
			If (wellKnownAttributeData IsNot Nothing) Then
				Dim forwardedTypes As HashSet(Of NamedTypeSymbol) = wellKnownAttributeData.ForwardedTypes
				If (forwardedTypes IsNot Nothing) Then
					nullable2 = New Nullable(Of Integer)(forwardedTypes.Count)
				Else
					nullable = Nothing
					nullable2 = nullable
				End If
			Else
				nullable = Nothing
				nullable2 = nullable
			End If
			Dim nullable4 As Nullable(Of Integer) = nullable2
			If (nullable4.HasValue) Then
				nullable3 = New Nullable(Of Boolean)(nullable4.GetValueOrDefault() > 0)
			Else
				nullable1 = Nothing
				nullable3 = nullable1
			End If
			nullable1 = nullable3
			If (nullable1.GetValueOrDefault()) Then
				Dim instance As ArrayBuilder(Of ValueTuple(Of NamedTypeSymbol, Integer)) = ArrayBuilder(Of ValueTuple(Of NamedTypeSymbol, Integer)).GetInstance()
				Dim namedTypeSymbols As IEnumerable(Of NamedTypeSymbol) = wellKnownAttributeData.ForwardedTypes
				If (builderOpt IsNot Nothing) Then
					Dim namedTypeSymbols1 As IEnumerable(Of NamedTypeSymbol) = namedTypeSymbols
					If (PEModuleBuilder._Closure$__.$I46-0 Is Nothing) Then
						displayString = Function(t As NamedTypeSymbol) t.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat)
						PEModuleBuilder._Closure$__.$I46-0 = displayString
					Else
						displayString = PEModuleBuilder._Closure$__.$I46-0
					End If
					namedTypeSymbols = namedTypeSymbols1.OrderBy(Of String)(displayString)
				End If
				Using enumerator
					enumerator = namedTypeSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim originalDefinition As NamedTypeSymbol = enumerator.Current.OriginalDefinition
						If (Not seenTopLevelTypes.Add(originalDefinition) OrElse builderOpt Is Nothing) Then
							Continue While
						End If
						instance.Push(New ValueTuple(Of NamedTypeSymbol, Integer)(originalDefinition, -1))
						While instance.Count > 0
							Dim valueTuple As ValueTuple(Of NamedTypeSymbol, Integer) = instance.Pop()
							If (valueTuple.Item1.DeclaredAccessibility = Accessibility.[Private]) Then
								Continue While
							End If
							Dim count As Integer = builderOpt.Count
							builderOpt.Add(New ExportedType(valueTuple.Item1.GetCciAdapter(), valueTuple.Item2, True))
							Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = valueTuple.Item1.GetTypeMembers()
							For i As Integer = typeMembers.Length - 1 To 0 Step -1
								instance.Push(New ValueTuple(Of NamedTypeSymbol, Integer)(typeMembers(i), count))
							Next

						End While
					End While
				End Using
				instance.Free()
			End If
		End Sub

		Public NotOverridable Overrides Function GetImports() As ImmutableArray(Of UsedNamespaceOrType)
			Return Me._lazyTranslatedImports
		End Function

		Public NotOverridable Overrides Function GetInitArrayHelper() As IMethodReference
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle), MethodSymbol)
			If (wellKnownTypeMember IsNot Nothing) Then
				Return wellKnownTypeMember.GetCciAdapter()
			End If
			Return Nothing
		End Function

		Friend Overridable Function GetNextAnonymousTypeIndex(ByVal fromDelegates As Boolean) As Integer
			Return 0
		End Function

		Friend Overridable Function GetPreviousAnonymousTypes() As ImmutableArray(Of AnonymousTypeKey)
			Return ImmutableArray(Of AnonymousTypeKey).Empty
		End Function

		Friend Function GetReferencedAssembliesUsedSoFar() As IEnumerable(Of AssemblySymbol)
			Return New PEModuleBuilder.VB$StateMachine_47_GetReferencedAssembliesUsedSoFar(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Private Function GetSmallestSourceLocationOrNull(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.Location
			Dim declaringCompilation As VisualBasicCompilation = symbol.DeclaringCompilation
			Dim location As Microsoft.CodeAnalysis.Location = Nothing
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = symbol.Locations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.Location = enumerator.Current
				If (Not current.IsInSource OrElse location IsNot Nothing AndAlso declaringCompilation.CompareSourceLocations(location, current) <= 0) Then
					Continue While
				End If
				location = current
			End While
			Return location
		End Function

		Public NotOverridable Overrides Function GetSourceAssemblyAttributes(ByVal isRefAssembly As Boolean) As IEnumerable(Of ICustomAttribute)
			Return Me.SourceModule.ContainingSourceAssembly.GetAssemblyCustomAttributesToEmit(Me.CompilationState, isRefAssembly, Me.OutputKind.IsNetModule())
		End Function

		Public NotOverridable Overrides Function GetSourceAssemblySecurityAttributes() As IEnumerable(Of SecurityAttribute)
			Return Me.SourceModule.ContainingSourceAssembly.GetSecurityAttributes()
		End Function

		Public NotOverridable Overrides Function GetSourceModuleAttributes() As IEnumerable(Of ICustomAttribute)
			Return Me.SourceModule.GetCustomAttributesToEmit(Me.CompilationState)
		End Function

		Friend NotOverridable Overrides Function GetSpecialType(ByVal specialType As Microsoft.CodeAnalysis.SpecialType, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As INamedTypeReference
			Return Me.Translate(Me.GetUntranslatedSpecialType(specialType, syntaxNodeOpt, diagnostics), syntaxNodeOpt, diagnostics, False, True)
		End Function

		Public NotOverridable Overrides Function GetSymbolToLocationMap() As MultiDictionary(Of DebugSourceDocument, DefinitionWithLocation)
			Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim debugSourceDocuments As MultiDictionary(Of DebugSourceDocument, DefinitionWithLocation) = New MultiDictionary(Of DebugSourceDocument, DefinitionWithLocation)()
			Dim namespaceOrTypeSymbols As Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol) = New Stack(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)()
			namespaceOrTypeSymbols.Push(Me.SourceModule.GlobalNamespace)
			Dim smallestSourceLocationOrNull As Location = Nothing
		Label0:
			While namespaceOrTypeSymbols.Count > 0
				Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = namespaceOrTypeSymbols.Pop()
				Dim kind As Microsoft.CodeAnalysis.SymbolKind = namespaceOrTypeSymbol.Kind
				If (kind = Microsoft.CodeAnalysis.SymbolKind.NamedType) Then
					smallestSourceLocationOrNull = Me.GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol)
					If (smallestSourceLocationOrNull Is Nothing) Then
						Continue While
					End If
					Me.AddSymbolLocation(debugSourceDocuments, smallestSourceLocationOrNull, DirectCast(namespaceOrTypeSymbol.GetCciAdapter(), IDefinition))
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = namespaceOrTypeSymbol.GetMembers().GetEnumerator()
					While True
						If (enumerator.MoveNext()) Then
							current = enumerator.Current
							Dim symbolKind As Microsoft.CodeAnalysis.SymbolKind = current.Kind
							Select Case symbolKind
								Case Microsoft.CodeAnalysis.SymbolKind.[Event]
									Me.AddSymbolLocation(debugSourceDocuments, current)
									Dim associatedField As FieldSymbol = DirectCast(current, EventSymbol).AssociatedField
									If (associatedField Is Nothing) Then
										Continue While
									End If
									Me.AddSymbolLocation(debugSourceDocuments, associatedField)
									Continue While
								Case Microsoft.CodeAnalysis.SymbolKind.Field
								Label1:
									Me.AddSymbolLocation(debugSourceDocuments, current)
									Continue While
								Case Microsoft.CodeAnalysis.SymbolKind.Label
								Case Microsoft.CodeAnalysis.SymbolKind.Local
								Case Microsoft.CodeAnalysis.SymbolKind.NetModule
									Exit Select
								Case Microsoft.CodeAnalysis.SymbolKind.Method
									Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
									If (methodSymbol.IsDefaultValueTypeConstructor() OrElse methodSymbol.IsPartialWithoutImplementation()) Then
										Continue While
									End If
									Me.AddSymbolLocation(debugSourceDocuments, current)
									Continue While
								Case Microsoft.CodeAnalysis.SymbolKind.NamedType
									namespaceOrTypeSymbols.Push(DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol))
									Continue While
								Case Else
									If (symbolKind = Microsoft.CodeAnalysis.SymbolKind.[Property]) Then
										GoTo Label1
									End If

							End Select
						Else
							GoTo Label0
						End If
					End While
					Throw ExceptionUtilities.UnexpectedValue(current.Kind)
				Else
					If (kind <> Microsoft.CodeAnalysis.SymbolKind.[Namespace]) Then
						Throw ExceptionUtilities.UnexpectedValue(namespaceOrTypeSymbol.Kind)
					End If
					smallestSourceLocationOrNull = Me.GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol)
					If (smallestSourceLocationOrNull Is Nothing) Then
						Continue While
					End If
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = namespaceOrTypeSymbol.GetMembers().GetEnumerator()
					While enumerator1.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (CInt(symbol.Kind) - CInt(Microsoft.CodeAnalysis.SymbolKind.NamedType) > CInt(Microsoft.CodeAnalysis.SymbolKind.ArrayType)) Then
							Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
						End If
						namespaceOrTypeSymbols.Push(DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol))
					End While
				End If
			End While
			Return debugSourceDocuments
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedNestedTypes(ByVal container As NamedTypeSymbol) As IEnumerable(Of INestedTypeDefinition)
			Return container.GetSynthesizedNestedTypes()
		End Function

		Public Overrides Function GetTopLevelSourceTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Return New PEModuleBuilder.VB$StateMachine_40_GetTopLevelSourceTypeDefinitions(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function GetUntranslatedSpecialType(ByVal specialType As Microsoft.CodeAnalysis.SpecialType, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.SourceModule.ContainingAssembly.GetSpecialType(specialType)
			Dim useSiteInfoForSpecialType As UseSiteInfo(Of AssemblySymbol) = Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForSpecialType(namedTypeSymbol, False)
			If (useSiteInfoForSpecialType.DiagnosticInfo IsNot Nothing) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, If(syntaxNodeOpt IsNot Nothing, syntaxNodeOpt.GetLocation(), NoLocation.Singleton), useSiteInfoForSpecialType.DiagnosticInfo)
			End If
			Return namedTypeSymbol
		End Function

		Public NotOverridable Overrides Function IsPlatformType(ByVal typeRef As ITypeReference, ByVal platformType As Microsoft.Cci.PlatformType) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As NamedTypeSymbol = TryCast(typeRef.GetInternalSymbol(), NamedTypeSymbol)
			If (internalSymbol Is Nothing) Then
				flag = False
			Else
				flag = If(platformType <> Microsoft.Cci.PlatformType.SystemType, CSByte(internalSymbol.SpecialType) = CSByte(platformType), CObj(internalSymbol) = CObj(Me.Compilation.GetWellKnownType(WellKnownType.System_Type)))
			End If
			Return flag
		End Function

		Public Function JITOptimizationIsDisabled(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Boolean
			Return Me._disableJITOptimization.ContainsKey(methodSymbol)
		End Function

		Public Shared Function MemberVisibility(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.Cci.TypeMemberVisibility
			Dim typeMemberVisibility As Microsoft.Cci.TypeMemberVisibility
			Select Case symbol.DeclaredAccessibility
				Case Accessibility.[Private]
					If (symbol.ContainingType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Submission) Then
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Private]
						Exit Select
					Else
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Public]
						Exit Select
					End If
				Case Accessibility.ProtectedAndInternal
					typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.FamilyAndAssembly
					Exit Select
				Case Accessibility.[Protected]
					If (symbol.ContainingType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Submission) Then
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.Family
						Exit Select
					Else
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Public]
						Exit Select
					End If
				Case Accessibility.Internal
					If (Not symbol.ContainingAssembly.IsInteractive) Then
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.Assembly
						Exit Select
					Else
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Public]
						Exit Select
					End If
				Case Accessibility.ProtectedOrInternal
					If (Not symbol.ContainingAssembly.IsInteractive) Then
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.FamilyOrAssembly
						Exit Select
					Else
						typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Public]
						Exit Select
					End If
				Case Accessibility.[Public]
					typeMemberVisibility = Microsoft.Cci.TypeMemberVisibility.[Public]
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility)
			End Select
			Return typeMemberVisibility
		End Function

		Private Shared Function MustBeWrapped(ByVal param As ParameterSymbol) As Boolean
			Dim flag As Boolean
			flag = If(Not param.IsDefinition OrElse Not PEModuleBuilder.ContainerIsGeneric(param.ContainingSymbol), False, True)
			Return flag
		End Function

		Private Sub ProcessReferencedSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (symbol.EmbeddedSymbolKind <> EmbeddedSymbolKind.None) Then
				If (Me._addedEmbeddedSymbols Is Nothing) Then
					Interlocked.CompareExchange(Of ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))(Me._addedEmbeddedSymbols, New ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(ReferenceEqualityComparer.Instance), Nothing)
				End If
				Me.SourceModule.ContainingSourceAssembly.DeclaringCompilation.EmbeddedSymbolManager.MarkSymbolAsReferenced(symbol.OriginalDefinition, Me._addedEmbeddedSymbols)
			End If
		End Sub

		Private Sub ReportExportedTypeNameCollisions(ByVal exportedTypes As ImmutableArray(Of ExportedType), ByVal diagnostics As DiagnosticBag)
			Dim containingSourceAssembly As SourceAssemblySymbol = Me.SourceModule.ContainingSourceAssembly
			Dim strs As Dictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = New Dictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)()
			Dim enumerator As ImmutableArray(Of ExportedType).Enumerator = Me._lazyExportedTypes.GetEnumerator()
			While enumerator.MoveNext()
				Dim type As ITypeReference = enumerator.Current.Type
				Dim internalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type.GetInternalSymbol(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (internalSymbol.ContainingType IsNot Nothing) Then
					Continue While
				End If
				Dim str As String = MetadataHelpers.BuildQualifiedName(DirectCast(type, INamespaceTypeReference).NamespaceName, MetadataWriter.GetMangledName(DirectCast(type, INamedTypeReference)))
				If (Not MyBase.ContainsTopLevelType(str)) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
					If (Not strs.TryGetValue(str, namedTypeSymbol)) Then
						strs.Add(str, internalSymbol)
					ElseIf (internalSymbol.ContainingAssembly = containingSourceAssembly) Then
						diagnostics.Add(ERRID.ERR_ExportedTypesConflict, NoLocation.Singleton, New [Object]() { CustomSymbolDisplayFormatter.DefaultErrorFormat(internalSymbol), CustomSymbolDisplayFormatter.DefaultErrorFormat(internalSymbol.ContainingModule), CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol.ContainingModule) })
					ElseIf (namedTypeSymbol.ContainingAssembly <> containingSourceAssembly) Then
						diagnostics.Add(ERRID.ERR_ForwardedTypesConflict, NoLocation.Singleton, New [Object]() { CustomSymbolDisplayFormatter.DefaultErrorFormat(internalSymbol), internalSymbol.ContainingAssembly, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), namedTypeSymbol.ContainingAssembly })
					Else
						diagnostics.Add(ERRID.ERR_ForwardedTypeConflictsWithExportedType, NoLocation.Singleton, New [Object]() { CustomSymbolDisplayFormatter.DefaultErrorFormat(internalSymbol), internalSymbol.ContainingAssembly, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol.ContainingModule) })
					End If
				ElseIf (internalSymbol.ContainingAssembly <> containingSourceAssembly) Then
					diagnostics.Add(ERRID.ERR_ForwardedTypeConflictsWithDeclaration, NoLocation.Singleton, New [Object]() { CustomSymbolDisplayFormatter.DefaultErrorFormat(internalSymbol) })
				Else
					diagnostics.Add(ERRID.ERR_ExportedTypeConflictsWithDeclaration, NoLocation.Singleton, New [Object]() { internalSymbol, internalSymbol.ContainingModule })
				End If
			End While
		End Sub

		Public Sub SetDisableJITOptimization(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Me._disableJITOptimization.TryAdd(methodSymbol, True)
		End Sub

		Friend NotOverridable Overrides Function SynthesizeAttribute(ByVal attributeConstructor As WellKnownMember) As ICustomAttribute
			Dim compilation As !0 = Me.Compilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Return compilation.TrySynthesizeAttribute(attributeConstructor, typedConstants, New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))(), False)
		End Function

		Friend NotOverridable Overrides Function Translate(ByVal assembly As AssemblySymbol, ByVal diagnostics As DiagnosticBag) As IAssemblyReference
			Dim assemblyReference As IAssemblyReference
			If (CObj(Me.SourceModule.ContainingAssembly) <> CObj(assembly)) Then
				Dim moduleReference As IModuleReference = Nothing
				If (Not Me.m_AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(assembly, moduleReference)) Then
					Dim assemblyReference1 As Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference(assembly)
					Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference = DirectCast(Me.m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(assembly, assemblyReference1), Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference)
					If (orAdd = assemblyReference1) Then
						Me.ValidateReferencedAssembly(assembly, orAdd, diagnostics)
					End If
					Dim mAssemblyOrModuleSymbolToModuleRefMap As ConcurrentDictionary(Of Symbol, IModuleReference) = Me.m_AssemblyOrModuleSymbolToModuleRefMap
					Dim modules As ImmutableArray(Of ModuleSymbol) = assembly.Modules
					mAssemblyOrModuleSymbolToModuleRefMap.TryAdd(modules(0), orAdd)
					assemblyReference = orAdd
				Else
					assemblyReference = DirectCast(moduleReference, IAssemblyReference)
				End If
			Else
				assemblyReference = DirectCast(Me, IAssemblyReference)
			End If
			Return assemblyReference
		End Function

		Friend Function Translate(ByVal [module] As ModuleSymbol, ByVal diagnostics As DiagnosticBag) As IModuleReference
			Dim moduleReference As IModuleReference
			If (Me.SourceModule <> [module]) Then
				Dim orAdd As IModuleReference = Nothing
				If (Not Me.m_AssemblyOrModuleSymbolToModuleRefMap.TryGetValue([module], orAdd)) Then
					orAdd = Me.TranslateModule([module], diagnostics)
					orAdd = Me.m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd([module], orAdd)
					moduleReference = orAdd
				Else
					moduleReference = orAdd
				End If
			Else
				moduleReference = Me
			End If
			Return moduleReference
		End Function

		Friend Function Translate(ByVal namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, Optional ByVal fromImplements As Boolean = False, Optional ByVal needDeclaration As Boolean = False) As INamedTypeReference
			Dim cciAdapter As INamedTypeReference
			Dim specializedNestedTypeReference As INamedTypeReference
			Dim obj As Object
			If (namedTypeSymbol.IsAnonymousType) Then
				namedTypeSymbol = AnonymousTypeManager.TranslateAnonymousTypeSymbol(namedTypeSymbol)
			ElseIf (namedTypeSymbol.IsTupleType) Then
				namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType
				Me.CheckTupleUnderlyingType(namedTypeSymbol, syntaxNodeOpt, diagnostics)
			End If
			If (namedTypeSymbol.OriginalDefinition.Kind <> SymbolKind.ErrorType) Then
				Me.ProcessReferencedSymbol(namedTypeSymbol)
				If (CObj(namedTypeSymbol) <> CObj(namedTypeSymbol.OriginalDefinition)) Then
					If (namedTypeSymbol.IsUnboundGenericType) Then
						GoTo Label2
					End If
					cciAdapter = DirectCast(Me.GetCciAdapter(namedTypeSymbol), INamedTypeReference)
					Return cciAdapter
				ElseIf (Not needDeclaration) Then
					Dim obj1 As Object = Nothing
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.ContainingType
					If (namedTypeSymbol.Arity <= 0) Then
						If (Not containingType.IsOrInGenericType()) Then
							GoTo Label1
						End If
						If (Not Me._genericInstanceMap.TryGetValue(namedTypeSymbol, obj1)) Then
							specializedNestedTypeReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedNestedTypeReference(namedTypeSymbol)
							specializedNestedTypeReference = DirectCast(Me._genericInstanceMap.GetOrAdd(namedTypeSymbol, specializedNestedTypeReference), INamedTypeReference)
							cciAdapter = specializedNestedTypeReference
							Return cciAdapter
						Else
							cciAdapter = DirectCast(obj1, INamedTypeReference)
							Return cciAdapter
						End If
					ElseIf (Not Me._genericInstanceMap.TryGetValue(namedTypeSymbol, obj1)) Then
						If (containingType Is Nothing) Then
							specializedNestedTypeReference = New GenericNamespaceTypeInstanceReference(namedTypeSymbol)
						ElseIf (Not containingType.IsOrInGenericType()) Then
							specializedNestedTypeReference = New GenericNestedTypeInstanceReference(namedTypeSymbol)
						Else
							specializedNestedTypeReference = New SpecializedGenericNestedTypeInstanceReference(namedTypeSymbol)
						End If
						specializedNestedTypeReference = DirectCast(Me._genericInstanceMap.GetOrAdd(namedTypeSymbol, specializedNestedTypeReference), INamedTypeReference)
						cciAdapter = specializedNestedTypeReference
						Return cciAdapter
					Else
						cciAdapter = DirectCast(obj1, INamedTypeReference)
						Return cciAdapter
					End If
				End If
			Label1:
				Dim embeddedTypesManager As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedTypesManager = Me._embeddedTypesManagerOpt
				If (embeddedTypesManager IsNot Nothing) Then
					obj = embeddedTypesManager.EmbedTypeIfNeedTo(namedTypeSymbol, fromImplements, syntaxNodeOpt, diagnostics)
				Else
					obj = Nothing
				End If
				If (obj Is Nothing) Then
					obj = namedTypeSymbol.GetCciAdapter()
				End If
				cciAdapter = obj
			Else
				Dim originalDefinition As ErrorTypeSymbol = DirectCast(namedTypeSymbol.OriginalDefinition, ErrorTypeSymbol)
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = If(originalDefinition.GetUseSiteInfo().DiagnosticInfo, originalDefinition.ErrorInfo)
				If (diagnosticInfo Is Nothing AndAlso namedTypeSymbol.Kind = SymbolKind.ErrorType) Then
					originalDefinition = DirectCast(namedTypeSymbol, ErrorTypeSymbol)
					diagnosticInfo = If(originalDefinition.GetUseSiteInfo().DiagnosticInfo, originalDefinition.ErrorInfo)
				End If
				If (Me._reportedErrorTypesMap.Add(originalDefinition)) Then
					diagnostics.Add(New VBDiagnostic(If(diagnosticInfo, ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, New [Object]() { [String].Empty })), If(syntaxNodeOpt Is Nothing, NoLocation.Singleton, syntaxNodeOpt.GetLocation()), False))
				End If
				cciAdapter = ErrorType.Singleton
			End If
			Return cciAdapter
		Label2:
			namedTypeSymbol = namedTypeSymbol.OriginalDefinition
			GoTo Label1
		End Function

		Friend Function Translate(ByVal param As TypeParameterSymbol) As IGenericParameterReference
			Return param.GetCciAdapter()
		End Function

		Friend NotOverridable Overrides Function Translate(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As ITypeReference
			Dim typeReference As ITypeReference
			Dim kind As SymbolKind = typeSymbol.Kind
			If (kind > SymbolKind.ErrorType) Then
				If (kind = SymbolKind.NamedType) Then
					typeReference = Me.Translate(DirectCast(typeSymbol, NamedTypeSymbol), syntaxNodeOpt, diagnostics, False, False)
					Return typeReference
				End If
				If (kind <> SymbolKind.TypeParameter) Then
					Throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind)
				End If
				typeReference = Me.Translate(DirectCast(typeSymbol, TypeParameterSymbol))
				Return typeReference
			Else
				If (kind <> SymbolKind.ArrayType) Then
					If (kind = SymbolKind.ErrorType) Then
						typeReference = Me.Translate(DirectCast(typeSymbol, NamedTypeSymbol), syntaxNodeOpt, diagnostics, False, False)
						Return typeReference
					End If
					Throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind)
				End If
				typeReference = Me.Translate(DirectCast(typeSymbol, ArrayTypeSymbol))
				Return typeReference
			End If
			Throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind)
		End Function

		Friend Function Translate(ByVal fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, Optional ByVal needDeclaration As Boolean = False) As IFieldReference
			Dim cciAdapter As IFieldReference
			Me.ProcessReferencedSymbol(fieldSymbol)
			If (CObj(fieldSymbol) <> CObj(fieldSymbol.OriginalDefinition)) Then
				cciAdapter = DirectCast(Me.GetCciAdapter(fieldSymbol), IFieldReference)
			ElseIf (Not needDeclaration AndAlso fieldSymbol.ContainingType.IsOrInGenericType()) Then
				Dim obj As Object = Nothing
				If (Not Me._genericInstanceMap.TryGetValue(fieldSymbol, obj)) Then
					Dim specializedFieldReference As IFieldReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedFieldReference(fieldSymbol)
					cciAdapter = DirectCast(Me._genericInstanceMap.GetOrAdd(fieldSymbol, specializedFieldReference), IFieldReference)
				Else
					cciAdapter = DirectCast(obj, IFieldReference)
				End If
			ElseIf (Me._embeddedTypesManagerOpt Is Nothing) Then
				cciAdapter = fieldSymbol.GetCciAdapter()
			Else
				cciAdapter = Me._embeddedTypesManagerOpt.EmbedFieldIfNeedTo(fieldSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			End If
			Return cciAdapter
		End Function

		Friend Overrides Function Translate(ByVal symbol As MethodSymbol, ByVal diagnostics As DiagnosticBag, ByVal needDeclaration As Boolean) As IMethodReference
			Return Me.Translate(symbol, Nothing, diagnostics, needDeclaration)
		End Function

		Friend Function Translate(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, Optional ByVal needDeclaration As Boolean = False) As IMethodReference
			Dim cciAdapter As IMethodReference
			Dim specializedMethodReference As IMethodReference
			Dim containingType As NamedTypeSymbol = methodSymbol.ContainingType
			If (containingType.IsAnonymousType) Then
				methodSymbol = AnonymousTypeManager.TranslateAnonymousTypeMethodSymbol(methodSymbol)
			ElseIf (methodSymbol.IsTupleMethod) Then
				containingType = containingType.TupleUnderlyingType
				methodSymbol = methodSymbol.TupleUnderlyingMethod
			End If
			Me.ProcessReferencedSymbol(methodSymbol)
			If (CObj(methodSymbol.OriginalDefinition) = CObj(methodSymbol)) Then
				If (Not needDeclaration) Then
					Dim isGenericMethod As Boolean = methodSymbol.IsGenericMethod
					Dim flag As Boolean = containingType.IsOrInGenericType()
					If (Not isGenericMethod AndAlso Not flag) Then
						GoTo Label1
					End If
					Dim obj As Object = Nothing
					If (Not Me._genericInstanceMap.TryGetValue(methodSymbol, obj)) Then
						If (Not isGenericMethod) Then
							specializedMethodReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedMethodReference(methodSymbol)
						ElseIf (Not flag) Then
							specializedMethodReference = New GenericMethodInstanceReference(methodSymbol)
						Else
							specializedMethodReference = New SpecializedGenericMethodInstanceReference(methodSymbol)
						End If
						specializedMethodReference = DirectCast(Me._genericInstanceMap.GetOrAdd(methodSymbol, specializedMethodReference), IMethodReference)
						cciAdapter = specializedMethodReference
						Return cciAdapter
					Else
						cciAdapter = DirectCast(obj, IMethodReference)
						Return cciAdapter
					End If
				End If
			Label1:
				If (Me._embeddedTypesManagerOpt Is Nothing) Then
					cciAdapter = methodSymbol.GetCciAdapter()
				Else
					cciAdapter = Me._embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics)
				End If
			Else
				cciAdapter = DirectCast(Me.GetCciAdapter(methodSymbol), IMethodReference)
			End If
			Return cciAdapter
		End Function

		Friend Function Translate(ByVal params As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of IParameterTypeInformation)
			Dim parameterTypeInformations As ImmutableArray(Of IParameterTypeInformation)
			parameterTypeInformations = If(If(Not System.Linq.ImmutableArrayExtensions.Any(Of ParameterSymbol)(params), False, PEModuleBuilder.MustBeWrapped(params.First())), Me.TranslateAll(params), StaticCast(Of IParameterTypeInformation).From(Of ParameterSymbol)(params))
			Return parameterTypeInformations
		End Function

		Friend Function Translate(ByVal symbol As ArrayTypeSymbol) As IArrayTypeReference
			Return DirectCast(Me.GetCciAdapter(symbol), IArrayTypeReference)
		End Function

		Private Function TranslateAll(ByVal params As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of IParameterTypeInformation)
			Dim instance As ArrayBuilder(Of IParameterTypeInformation) = ArrayBuilder(Of IParameterTypeInformation).GetInstance()
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = params.GetEnumerator()
			While enumerator.MoveNext()
				instance.Add(Me.CreateParameterTypeInformationWrapper(enumerator.Current))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Public Sub TranslateImports(ByVal diagnostics As DiagnosticBag)
			If (Me._lazyTranslatedImports.IsDefault) Then
				ImmutableInterlocked.InterlockedInitialize(Of UsedNamespaceOrType)(Me._lazyTranslatedImports, NamespaceScopeBuilder.BuildNamespaceScope(Me, Me.SourceModule.XmlNamespaces, DirectCast(Me.SourceModule.AliasImports, IEnumerable(Of AliasAndImportsClausePosition)), Me.SourceModule.MemberImports, diagnostics))
			End If
		End Sub

		Protected Overridable Function TranslateModule(ByVal [module] As ModuleSymbol, ByVal diagnostics As DiagnosticBag) As IModuleReference
			Dim moduleReference As IModuleReference
			Dim containingAssembly As AssemblySymbol = [module].ContainingAssembly
			If (containingAssembly Is Nothing OrElse CObj(containingAssembly.Modules(0)) <> CObj([module])) Then
				moduleReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.ModuleReference(Me, [module])
			Else
				Dim assemblyReference As IModuleReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference(containingAssembly)
				Dim orAdd As IModuleReference = Me.m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(containingAssembly, assemblyReference)
				If (orAdd <> assemblyReference) Then
					assemblyReference = orAdd
				Else
					Me.ValidateReferencedAssembly(containingAssembly, DirectCast(assemblyReference, Microsoft.CodeAnalysis.VisualBasic.Emit.AssemblyReference), diagnostics)
				End If
				moduleReference = assemblyReference
			End If
			Return moduleReference
		End Function

		Friend Function TranslateOverriddenMethodReference(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal syntaxNodeOpt As VisualBasicSyntaxNode, ByVal diagnostics As DiagnosticBag) As IMethodReference
			Dim specializedMethodReference As IMethodReference
			If (methodSymbol.ContainingType.IsOrInGenericType()) Then
				If (Not methodSymbol.IsDefinition) Then
					specializedMethodReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedMethodReference(methodSymbol)
				Else
					Dim obj As Object = Nothing
					If (Not Me._genericInstanceMap.TryGetValue(methodSymbol, obj)) Then
						specializedMethodReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedMethodReference(methodSymbol)
						specializedMethodReference = DirectCast(Me._genericInstanceMap.GetOrAdd(methodSymbol, specializedMethodReference), IMethodReference)
					Else
						specializedMethodReference = DirectCast(obj, IMethodReference)
					End If
				End If
			ElseIf (Me._embeddedTypesManagerOpt Is Nothing) Then
				specializedMethodReference = methodSymbol.GetCciAdapter()
			Else
				specializedMethodReference = Me._embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			End If
			Return specializedMethodReference
		End Function

		Friend Overridable Function TryCreateVariableSlotAllocator(ByVal method As MethodSymbol, ByVal topLevelMethod As MethodSymbol, ByVal diagnostics As DiagnosticBag) As VariableSlotAllocator
			Return Nothing
		End Function

		Friend Overridable Function TryGetAnonymousTypeName(ByVal template As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef name As String, <Out> ByRef index As Integer) As Boolean
			name = Nothing
			index = -1
			Return False
		End Function

		Private Sub ValidateReferencedAssembly(ByVal assembly As AssemblySymbol, ByVal asmRef As AssemblyReference, ByVal diagnostics As DiagnosticBag)
			Dim identity As Microsoft.CodeAnalysis.AssemblyIdentity = Me.SourceModule.ContainingAssembly.Identity
			Dim assemblyIdentity As Microsoft.CodeAnalysis.AssemblyIdentity = asmRef.Identity
			If (identity.IsStrongName AndAlso Not assemblyIdentity.IsStrongName AndAlso asmRef.Identity.ContentType <> AssemblyContentType.WindowsRuntime) Then
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName, New [Object]() { assembly }), NoLocation.Singleton)
			End If
			If (Me.OutputKind <> OutputKind.NetModule AndAlso Not [String].IsNullOrEmpty(assemblyIdentity.CultureName) AndAlso Not [String].Equals(assemblyIdentity.CultureName, identity.CultureName, StringComparison.OrdinalIgnoreCase)) Then
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.WRN_RefCultureMismatch, New [Object]() { assembly, assemblyIdentity.CultureName }), NoLocation.Singleton)
			End If
			Dim machine As System.Reflection.PortableExecutable.Machine = assembly.Machine
			If (CObj(assembly) <> CObj(assembly.CorLibrary) AndAlso (machine <> System.Reflection.PortableExecutable.Machine.I386 OrElse assembly.Bit32Required)) Then
				Dim machine1 As System.Reflection.PortableExecutable.Machine = Me.SourceModule.Machine
				If ((machine1 <> System.Reflection.PortableExecutable.Machine.I386 OrElse Me.SourceModule.Bit32Required) AndAlso machine1 <> machine) Then
					diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.WRN_ConflictingMachineAssembly, New [Object]() { assembly }), NoLocation.Singleton)
				End If
			End If
			If (Me._embeddedTypesManagerOpt IsNot Nothing AndAlso Me._embeddedTypesManagerOpt.IsFrozen) Then
				Me._embeddedTypesManagerOpt.ReportIndirectReferencesToLinkedAssemblies(assembly, diagnostics)
			End If
		End Sub
	End Class
End Namespace