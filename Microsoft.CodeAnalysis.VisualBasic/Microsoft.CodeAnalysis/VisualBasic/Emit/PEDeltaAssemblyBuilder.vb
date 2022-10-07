Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class PEDeltaAssemblyBuilder
		Inherits PEAssemblyBuilderBase
		Implements IPEDeltaAssemblyBuilder
		Private ReadOnly _previousGeneration As EmitBaseline

		Private ReadOnly _previousDefinitions As VisualBasicDefinitionMap

		Private ReadOnly _changes As SymbolChanges

		Private ReadOnly _deepTranslator As VisualBasicSymbolMatcher.DeepTranslator

		Friend Overrides ReadOnly Property AllowOmissionOfConditionalCalls As Boolean
			Get
				Return True
			End Get
		End Property

		Friend ReadOnly Property Changes As SymbolChanges
			Get
				Return Me._changes
			End Get
		End Property

		Public Overrides ReadOnly Property CurrentGenerationOrdinal As Integer
			Get
				Return Me._previousGeneration.Ordinal + 1
			End Get
		End Property

		Public Overrides ReadOnly Property LinkedAssembliesDebugInfo As IEnumerable(Of String)
			Get
				Return SpecializedCollections.EmptyEnumerable(Of String)()
			End Get
		End Property

		Friend ReadOnly Property PreviousDefinitions As VisualBasicDefinitionMap
			Get
				Return Me._previousDefinitions
			End Get
		End Property

		Friend ReadOnly Property PreviousGeneration As EmitBaseline
			Get
				Return Me._previousGeneration
			End Get
		End Property

		Friend Overrides ReadOnly Property SupportsPrivateImplClass As Boolean
			Get
				Return False
			End Get
		End Property

		Public Sub New(ByVal sourceAssembly As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol, ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal serializationProperties As ModulePropertiesForSerialization, ByVal manifestResources As IEnumerable(Of ResourceDescription), ByVal previousGeneration As EmitBaseline, ByVal edits As IEnumerable(Of SemanticEdit), ByVal isAddedSymbol As Func(Of ISymbol, Boolean))
			MyBase.New(sourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources, ImmutableArray(Of NamedTypeSymbol).Empty)
			Dim initialBaseline As EmitBaseline = previousGeneration.InitialBaseline
			Dim emitContext As Microsoft.CodeAnalysis.Emit.EmitContext = New Microsoft.CodeAnalysis.Emit.EmitContext(Me, Nothing, New DiagnosticBag(), False, True)
			Dim metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder = DirectCast(PEDeltaAssemblyBuilder.GetOrCreateMetadataSymbols(initialBaseline, sourceAssembly.DeclaringCompilation).MetadataDecoder, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder)
			Dim containingAssembly As PEAssemblySymbol = DirectCast(metadataDecoder.ModuleSymbol.ContainingAssembly, PEAssemblySymbol)
			Dim visualBasicSymbolMatcher As Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicSymbolMatcher = New Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicSymbolMatcher(initialBaseline.LazyMetadataSymbols.AnonymousTypes, sourceAssembly, emitContext, containingAssembly)
			Dim visualBasicSymbolMatcher1 As Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicSymbolMatcher = Nothing
			If (previousGeneration.Ordinal > 0) Then
				Dim sourceAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol = DirectCast(previousGeneration.Compilation, VisualBasicCompilation).SourceAssembly
				Dim emitContext1 As Microsoft.CodeAnalysis.Emit.EmitContext = New Microsoft.CodeAnalysis.Emit.EmitContext(DirectCast(previousGeneration.PEModuleBuilder, PEModuleBuilder), Nothing, New DiagnosticBag(), False, True)
				visualBasicSymbolMatcher1 = New Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicSymbolMatcher(previousGeneration.AnonymousTypeMap, sourceAssembly, emitContext, sourceAssemblySymbol, emitContext1, previousGeneration.SynthesizedMembers)
			End If
			Me._previousDefinitions = New VisualBasicDefinitionMap(edits, metadataDecoder, visualBasicSymbolMatcher, visualBasicSymbolMatcher1)
			Me._previousGeneration = previousGeneration
			Me._changes = New VisualBasicSymbolChanges(Me._previousDefinitions, edits, isAddedSymbol)
			Me._deepTranslator = New Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicSymbolMatcher.DeepTranslator(sourceAssembly.GetSpecialType(SpecialType.System_Object))
		End Sub

		Friend Overrides Function EncTranslateLocalVariableType(ByVal type As TypeSymbol, ByVal diagnostics As DiagnosticBag) As ITypeReference
			Return MyBase.Translate(If(DirectCast(Me._deepTranslator.Visit(type), TypeSymbol), type), Nothing, diagnostics)
		End Function

		Private Shared Function GetAnonymousDelegateKey(ByVal type As NamedTypeSymbol) As AnonymousTypeKey
			Dim anonymousTypeKeyField As Func(Of ParameterSymbol, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)
			Dim item As MethodSymbol = DirectCast(type.GetMembers("Invoke")(0), MethodSymbol)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField) = ArrayBuilder(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField).GetInstance()
			Dim anonymousTypeKeyFields As ArrayBuilder(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField) = instance
			Dim parameters As ImmutableArray(Of ParameterSymbol) = item.Parameters
			If (PEDeltaAssemblyBuilder._Closure$__.$I11-0 Is Nothing) Then
				anonymousTypeKeyField = Function(p As ParameterSymbol) New Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField(p.Name, p.IsByRef, True)
				PEDeltaAssemblyBuilder._Closure$__.$I11-0 = anonymousTypeKeyField
			Else
				anonymousTypeKeyField = PEDeltaAssemblyBuilder._Closure$__.$I11-0
			End If
			anonymousTypeKeyFields.AddRange(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of ParameterSymbol, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)(parameters, anonymousTypeKeyField))
			instance.Add(New Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField(AnonymousTypeDescriptor.GetReturnParameterName(Not item.IsSub), False, True))
			Return New AnonymousTypeKey(instance.ToImmutableAndFree(), True)
		End Function

		Private Shared Function GetAnonymousTypeKey(ByVal type As NamedTypeSymbol) As Microsoft.CodeAnalysis.Emit.AnonymousTypeKey
			Dim anonymousTypeKey As Microsoft.CodeAnalysis.Emit.AnonymousTypeKey
			Dim length As Integer = type.TypeParameters.Length
			If (length <> 0) Then
				Dim anonymousTypeKeyField(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = type.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.[Property]) Then
						Continue While
					End If
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = propertySymbol.Type
					If (typeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
						Continue While
					End If
					Dim ordinal As Integer = DirectCast(typeSymbol, TypeParameterSymbol).Ordinal
					anonymousTypeKeyField(ordinal) = New Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField(propertySymbol.Name, propertySymbol.IsReadOnly, True)
				End While
				anonymousTypeKey = New Microsoft.CodeAnalysis.Emit.AnonymousTypeKey(ImmutableArray.Create(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)(anonymousTypeKeyField), False)
			Else
				anonymousTypeKey = New Microsoft.CodeAnalysis.Emit.AnonymousTypeKey(ImmutableArray(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField).Empty, False)
			End If
			Return anonymousTypeKey
		End Function

		Friend Function GetAnonymousTypeMap() As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue) Implements IPEDeltaAssemblyBuilder.GetAnonymousTypeMap
			Return Me.Compilation.AnonymousTypeManager.GetAnonymousTypeMap()
		End Function

		Friend Shared Function GetAnonymousTypeMapFromMetadata(ByVal reader As MetadataReader, ByVal metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder) As IReadOnlyDictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue)
			Dim enumerator As TypeDefinitionHandleCollection.Enumerator = New TypeDefinitionHandleCollection.Enumerator()
			Dim anonymousTypeKeys As Dictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue) = New Dictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue)()
			Try
				enumerator = reader.TypeDefinitions.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeDefinitionHandle = enumerator.Current
					Dim typeDefinition As System.Reflection.Metadata.TypeDefinition = reader.GetTypeDefinition(current)
					If (Not typeDefinition.[Namespace].IsNil OrElse Not reader.StringComparer.StartsWith(typeDefinition.Name, "VB$Anonymous")) Then
						Continue While
					End If
					Dim num As Short = 0
					Dim str As String = MetadataHelpers.InferTypeArityAndUnmangleMetadataName(reader.GetString(typeDefinition.Name), num)
					Dim num1 As Integer = 0
					If (Not GeneratedNames.TryParseAnonymousTypeTemplateName("VB$AnonymousType_", str, num1)) Then
						If (Not GeneratedNames.TryParseAnonymousTypeTemplateName("VB$AnonymousDelegate_", str, num1)) Then
							Continue While
						End If
						Dim typeOfToken As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(metadataDecoder.GetTypeOfToken(current), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						Dim anonymousDelegateKey As Microsoft.CodeAnalysis.Emit.AnonymousTypeKey = PEDeltaAssemblyBuilder.GetAnonymousDelegateKey(typeOfToken)
						Dim anonymousTypeValue As Microsoft.CodeAnalysis.Emit.AnonymousTypeValue = New Microsoft.CodeAnalysis.Emit.AnonymousTypeValue(str, num1, typeOfToken.GetCciAdapter())
						anonymousTypeKeys.Add(anonymousDelegateKey, anonymousTypeValue)
					Else
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(metadataDecoder.GetTypeOfToken(current), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						Dim anonymousTypeKey As Microsoft.CodeAnalysis.Emit.AnonymousTypeKey = PEDeltaAssemblyBuilder.GetAnonymousTypeKey(namedTypeSymbol)
						Dim anonymousTypeValue1 As Microsoft.CodeAnalysis.Emit.AnonymousTypeValue = New Microsoft.CodeAnalysis.Emit.AnonymousTypeValue(str, num1, namedTypeSymbol.GetCciAdapter())
						anonymousTypeKeys.Add(anonymousTypeKey, anonymousTypeValue1)
					End If
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return anonymousTypeKeys
		End Function

		Friend Overrides Function GetNextAnonymousTypeIndex(ByVal fromDelegates As Boolean) As Integer
			Return Me._previousGeneration.GetNextAnonymousTypeIndex(fromDelegates)
		End Function

		Private Shared Function GetOrCreateMetadataSymbols(ByVal initialBaseline As EmitBaseline, ByVal compilation As VisualBasicCompilation) As EmitBaseline.MetadataSymbols
			Dim lazyMetadataSymbols As EmitBaseline.MetadataSymbols
			If (initialBaseline.LazyMetadataSymbols Is Nothing) Then
				Dim originalMetadata As ModuleMetadata = initialBaseline.OriginalMetadata
				Dim assemblyIdentities As ImmutableDictionary(Of AssemblyIdentity, AssemblyIdentity) = Nothing
				Dim pEAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEAssemblySymbol = compilation.RemoveAllSyntaxTrees().GetBoundReferenceManager().CreatePEAssemblyForAssemblyMetadata(AssemblyMetadata.Create(originalMetadata), MetadataImportOptions.All, assemblyIdentities)
				Dim metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder(pEAssemblySymbol.PrimaryModule)
				Dim anonymousTypeMapFromMetadata As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue) = PEDeltaAssemblyBuilder.GetAnonymousTypeMapFromMetadata(originalMetadata.MetadataReader, metadataDecoder)
				Dim metadataSymbol As EmitBaseline.MetadataSymbols = New EmitBaseline.MetadataSymbols(anonymousTypeMapFromMetadata, metadataDecoder, assemblyIdentities)
				lazyMetadataSymbols = InterlockedOperations.Initialize(Of EmitBaseline.MetadataSymbols)(initialBaseline.LazyMetadataSymbols, metadataSymbol)
			Else
				lazyMetadataSymbols = initialBaseline.LazyMetadataSymbols
			End If
			Return lazyMetadataSymbols
		End Function

		Friend Overrides Function GetPreviousAnonymousTypes() As ImmutableArray(Of AnonymousTypeKey)
			Return ImmutableArray.CreateRange(Of AnonymousTypeKey)(Me._previousGeneration.AnonymousTypeMap.Keys)
		End Function

		Public Overrides Function GetTopLevelSourceTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Return Me._changes.GetTopLevelSourceTypeDefinitions(context)
		End Function

		Public Overrides Function GetTopLevelTypeDefinitions(ByVal context As EmitContext) As IEnumerable(Of INamespaceTypeDefinition)
			Return New PEDeltaAssemblyBuilder.VB$StateMachine_25_GetTopLevelTypeDefinitions(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Friend Sub OnCreatedIndices(ByVal diagnostics As DiagnosticBag) Implements IPEDeltaAssemblyBuilder.OnCreatedIndices
			Dim enumerator As IEnumerator(Of NamedTypeSymbol) = Nothing
			Dim embeddedTypesManagerOpt As EmbeddedTypesManager = Me.EmbeddedTypesManagerOpt
			If (embeddedTypesManagerOpt IsNot Nothing) Then
				Try
					enumerator = embeddedTypesManagerOpt.EmbeddedTypesMap.Keys.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_EncNoPIAReference, New [Object]() { current.AdaptedNamedTypeSymbol }), Location.None)
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
		End Sub

		Friend Overrides Function TryCreateVariableSlotAllocator(ByVal method As MethodSymbol, ByVal topLevelMethod As MethodSymbol, ByVal diagnostics As DiagnosticBag) As VariableSlotAllocator
			Return Me._previousDefinitions.TryCreateVariableSlotAllocator(Me._previousGeneration, Me.Compilation, method, topLevelMethod, diagnostics)
		End Function

		Friend Overrides Function TryGetAnonymousTypeName(ByVal template As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef name As String, <Out> ByRef index As Integer) As Boolean
			Return Me._previousDefinitions.TryGetAnonymousTypeName(template, name, index)
		End Function
	End Class
End Namespace