Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend Class PENamedTypeSymbol
		Inherits InstanceTypeSymbol
		Private ReadOnly _container As NamespaceOrTypeSymbol

		Private ReadOnly _handle As TypeDefinitionHandle

		Private ReadOnly _genericParameterHandles As GenericParameterHandleCollection

		Private ReadOnly _name As String

		Private ReadOnly _flags As TypeAttributes

		Private ReadOnly _arity As UShort

		Private ReadOnly _mangleName As Boolean

		Private _lazyNestedTypes As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol))

		Private _lazyMemberNames As ICollection(Of String)

		Private _lazyMembers As Dictionary(Of String, ImmutableArray(Of Symbol))

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyEnumUnderlyingType As NamedTypeSymbol

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyConditionalAttributeSymbols As ImmutableArray(Of String)

		Private _lazyAttributeUsageInfo As AttributeUsageInfo

		Private _lazyCoClassType As TypeSymbol

		Private _lazyTypeKind As Integer

		Private _lazyDocComment As Tuple(Of CultureInfo, String)

		Private _lazyDefaultPropertyName As String

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Private _lazyMightContainExtensionMethods As Byte

		Private _lazyHasCodeAnalysisEmbeddedAttribute As Integer

		Private _lazyHasVisualBasicEmbeddedAttribute As Integer

		Private _lazyObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

		Private _lazyIsExtensibleInterface As ThreeState

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				If (Me._lazyCoClassType = ErrorTypeSymbol.UnknownResultType) Then
					Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyCoClassType, Me.MakeComImportCoClassType(), ErrorTypeSymbol.UnknownResultType)
				End If
				Return Me._lazyCoClassType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me.ContainingPEModule
			End Get
		End Property

		Friend ReadOnly Property ContainingPEModule As PEModuleSymbol
			Get
				Dim containingSymbol As Symbol = Me._container
				While containingSymbol.Kind <> SymbolKind.[Namespace]
					containingSymbol = containingSymbol.ContainingSymbol
				End While
				Return DirectCast(containingSymbol, PENamespaceSymbol).ContainingPEModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return TryCast(Me._container, NamedTypeSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.Accessibility Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::get_DeclaredAccessibility()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.Accessibility get_DeclaredAccessibility()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
				'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
				'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
				'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
				'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				If (Me._lazyDefaultPropertyName Is Nothing) Then
					Dim str As String = If(Me.GetDefaultPropertyName(), [String].Empty)
					Interlocked.CompareExchange(Of String)(Me._lazyDefaultPropertyName, str, Nothing)
				End If
				If ([String].IsNullOrEmpty(Me._lazyDefaultPropertyName)) Then
					Return Nothing
				End If
				Return Me._lazyDefaultPropertyName
			End Get
		End Property

		Public Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				If (Me._lazyEnumUnderlyingType Is Nothing AndAlso Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
					Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Symbol = enumerator.Current
						If (current.IsShared OrElse current.Kind <> SymbolKind.Field) Then
							Continue While
						End If
						Dim type As TypeSymbol = DirectCast(current, FieldSymbol).Type
						If (Not type.SpecialType.IsClrInteger()) Then
							Continue While
						End If
						If (unsupportedMetadataTypeSymbol IsNot Nothing) Then
							unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
							Exit While
						Else
							unsupportedMetadataTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						End If
					End While
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = unsupportedMetadataTypeSymbol
					If (namedTypeSymbol Is Nothing) Then
						namedTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
					End If
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(Me._lazyEnumUnderlyingType, namedTypeSymbol, Nothing)
				End If
				Return Me._lazyEnumUnderlyingType
			End Get
		End Property

		Friend ReadOnly Property Handle As TypeDefinitionHandle
			Get
				Return Me._handle
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				If (Me._lazyHasCodeAnalysisEmbeddedAttribute = 0) Then
					Interlocked.CompareExchange(Me._lazyHasCodeAnalysisEmbeddedAttribute, CInt(Me.ContainingPEModule.[Module].HasCodeAnalysisEmbeddedAttribute(Me._handle).ToThreeState()), 0)
				End If
				Return Me._lazyHasCodeAnalysisEmbeddedAttribute = 2
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return (Me._flags And TypeAttributes.SpecialName) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				If (Me._lazyHasVisualBasicEmbeddedAttribute = 0) Then
					Interlocked.CompareExchange(Me._lazyHasVisualBasicEmbeddedAttribute, CInt(Me.ContainingPEModule.[Module].HasVisualBasicEmbeddedAttribute(Me._handle).ToThreeState()), 0)
				End If
				Return Me._lazyHasVisualBasicEmbeddedAttribute = 2
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return (Me._flags And TypeAttributes.Import) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				If (Me._lazyIsExtensibleInterface = ThreeState.Unknown) Then
					Me._lazyIsExtensibleInterface = Me.DecodeIsExtensibleInterface().ToThreeState()
				End If
				Return Me._lazyIsExtensibleInterface.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return (Me._flags And TypeAttributes.ClassSemanticsMask) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataAbstract As Boolean
			Get
				Return (Me._flags And TypeAttributes.Abstract) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataSealed As Boolean
			Get
				Return (Me._flags And TypeAttributes.Sealed) <> TypeAttributes.NotPublic
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::get_IsMustInherit()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsMustInherit()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return (Me._flags And TypeAttributes.Sealed) <> TypeAttributes.NotPublic
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return (Me._flags And TypeAttributes.Serializable) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return (Me._flags And TypeAttributes.WindowsRuntime) <> TypeAttributes.NotPublic
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As Microsoft.CodeAnalysis.TypeLayout
			Get
				Return Me.ContainingPEModule.[Module].GetTypeLayout(Me._handle)
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of MetadataLocation)(Me.ContainingPEModule.MetadataLocation)
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me._mangleName
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Dim charSet As System.Runtime.InteropServices.CharSet = Me._flags.ToCharSet()
				Return If(CInt(charSet) <> 0, charSet, System.Runtime.InteropServices.CharSet.Ansi)
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Me.EnsureNonTypeMemberNamesAreLoaded()
				Return Me._lazyMemberNames
			End Get
		End Property

		Friend ReadOnly Property MetadataArity As Integer
			Get
				Return Me._genericParameterHandles.Count
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				If (Me._lazyMightContainExtensionMethods = 0) Then
					Dim flag As Boolean = False
					If (Me._container.Kind = SymbolKind.[Namespace] AndAlso Me._arity = 0) Then
						Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
						If (containingPEModule.MightContainExtensionMethods AndAlso containingPEModule.[Module].HasExtensionAttribute(Me._handle, True)) Then
							flag = True
						End If
					End If
					If (Not flag) Then
						Me._lazyMightContainExtensionMethods = 1
					Else
						Me._lazyMightContainExtensionMethods = 2
					End If
				End If
				Return Me._lazyMightContainExtensionMethods = 2
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(Me._lazyObsoleteAttributeData, Me._handle, Me.ContainingPEModule)
				Return Me._lazyObsoleteAttributeData
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return Me.IsWindowsRuntimeImport
			End Get
		End Property

		Friend ReadOnly Property TypeDefFlags As TypeAttributes
			Get
				Return Me._flags
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.TypeKind Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::get_TypeKind()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.TypeKind get_TypeKind()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Me.EnsureTypeParametersAreLoaded()
				Return Me._lazyTypeParameters
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingNamespace As PENamespaceSymbol, ByVal handle As TypeDefinitionHandle)
			MyClass.New(moduleSymbol, containingNamespace, 0, handle)
		End Sub

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As TypeDefinitionHandle)
			MyClass.New(moduleSymbol, containingType, CUShort(containingType.MetadataArity), handle)
		End Sub

		Private Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal container As NamespaceOrTypeSymbol, ByVal containerMetadataArity As UShort, ByVal handle As TypeDefinitionHandle)
			MyBase.New()
			Dim typeDefNameOrThrow As String
			Dim count As Integer
			Me._lazyAttributeUsageInfo = AttributeUsageInfo.Null
			Me._lazyCoClassType = ErrorTypeSymbol.UnknownResultType
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			Me._lazyMightContainExtensionMethods = 0
			Me._lazyHasCodeAnalysisEmbeddedAttribute = 0
			Me._lazyHasVisualBasicEmbeddedAttribute = 0
			Me._lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
			Me._lazyIsExtensibleInterface = ThreeState.Unknown
			Me._handle = handle
			Me._container = container
			Dim flag As Boolean = False
			Try
				typeDefNameOrThrow = moduleSymbol.[Module].GetTypeDefNameOrThrow(handle)
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				typeDefNameOrThrow = [String].Empty
				flag = True
				ProjectData.ClearProjectError()
			End Try
			Try
				Me._flags = moduleSymbol.[Module].GetTypeDefFlagsOrThrow(handle)
			Catch badImageFormatException1 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException1)
				flag = True
				ProjectData.ClearProjectError()
			End Try
			Try
				Me._genericParameterHandles = moduleSymbol.[Module].GetTypeDefGenericParamsOrThrow(handle)
				count = Me._genericParameterHandles.Count
			Catch badImageFormatException2 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException2)
				Me._genericParameterHandles = New GenericParameterHandleCollection()
				count = 0
				flag = True
				ProjectData.ClearProjectError()
			End Try
			If (count > containerMetadataArity) Then
				Me._arity = CUShort((count - containerMetadataArity))
			End If
			If (Me._arity <> 0) Then
				Me._name = MetadataHelpers.UnmangleMetadataNameForArity(typeDefNameOrThrow, CInt(Me._arity))
				Me._mangleName = CObj(Me._name) <> CObj(typeDefNameOrThrow)
			Else
				Me._lazyTypeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
				Me._name = typeDefNameOrThrow
				Me._mangleName = False
			End If
			If (flag OrElse count < containerMetadataArity) Then
				Dim objArray() As [Object] = { Me }
				Me._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, objArray))
			End If
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal appendThrough As NamedTypeSymbol)
			If (Me.MightContainExtensionMethods) Then
				Me.EnsureNestedTypesAreLoaded()
				Me.EnsureNonTypeMembersAreLoaded()
				If (Not appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me._lazyMembers)) Then
					Me._lazyMightContainExtensionMethods = 1
				End If
			End If
		End Sub

		Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)), ByVal appendThrough As NamespaceSymbol)
			If (Me.MightContainExtensionMethods) Then
				Me.EnsureNestedTypesAreLoaded()
				Me.EnsureNonTypeMembersAreLoaded()
				If (Not appendThrough.BuildExtensionMethodsMap(map, Me._lazyMembers)) Then
					Me._lazyMightContainExtensionMethods = 1
				End If
			End If
		End Sub

		Private Function CalculateUseSiteInfoImpl() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim objArray As [Object]()
			Dim isMissing As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.CalculateUseSiteInfo()
			If (useSiteInfo1.DiagnosticInfo Is Nothing) Then
				If (Not Me.ContainingPEModule.[Module].HasRequiredAttributeAttribute(Me.Handle)) Then
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = Me.TypeKind
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = Me.SpecialType
					If ((typeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_Enum AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate) Then
						Dim declaredBase As TypeSymbol = Me.GetDeclaredBase(New BasesBeingResolved())
						If (declaredBase IsNot Nothing AndAlso declaredBase.SpecialType = Microsoft.CodeAnalysis.SpecialType.None) Then
							Dim containingAssembly As AssemblySymbol = declaredBase.ContainingAssembly
							If (containingAssembly IsNot Nothing) Then
								isMissing = containingAssembly.IsMissing
							Else
								isMissing = False
							End If
							If (isMissing) Then
								Dim topLevel As MissingMetadataTypeSymbol.TopLevel = TryCast(declaredBase, MissingMetadataTypeSymbol.TopLevel)
								If (topLevel Is Nothing OrElse topLevel.Arity <> 0 OrElse CSByte(SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(topLevel.NamespaceName, topLevel.MetadataName))) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_Enum) > CSByte(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate)) Then
									If (Me.MatchesContainingTypeParameters()) Then
										useSiteInfo = useSiteInfo1
										Return useSiteInfo
									End If
									objArray = New [Object]() { Me }
									useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_NestingViolatesCLS1, objArray))
									Return useSiteInfo
								End If
								useSiteInfo = topLevel.GetUseSiteInfo()
								Return useSiteInfo
							End If
						End If
					End If
					If (Me.MatchesContainingTypeParameters()) Then
						useSiteInfo = useSiteInfo1
						Return useSiteInfo
					End If
					objArray = New [Object]() { Me }
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_NestingViolatesCLS1, objArray))
					Return useSiteInfo
				Else
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, New [Object]() { Me }))
					Return useSiteInfo
				End If
			End If
			useSiteInfo = useSiteInfo1
			Return useSiteInfo
		End Function

		Private Sub CreateEvents(ByVal methodHandleToSymbol As Dictionary(Of MethodDefinitionHandle, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol), ByVal members As ArrayBuilder(Of Symbol))
			Dim enumerator As EventDefinitionHandleCollection.Enumerator = New EventDefinitionHandleCollection.Enumerator()
			Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
			Dim [module] As PEModule = containingPEModule.[Module]
			Try
				Try
					enumerator = [module].GetEventsOfTypeOrThrow(Me._handle).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As EventDefinitionHandle = enumerator.Current
						Try
							Dim eventMethodsOrThrow As EventAccessors = [module].GetEventMethodsOrThrow(current)
							Dim accessorMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = PENamedTypeSymbol.GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Adder)
							Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = PENamedTypeSymbol.GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Remover)
							Dim accessorMethod1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = PENamedTypeSymbol.GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Raiser)
							If (accessorMethod IsNot Nothing AndAlso pEMethodSymbol IsNot Nothing) Then
								members.Add(New PEEventSymbol(containingPEModule, Me, current, accessorMethod, pEMethodSymbol, accessorMethod1))
							End If
						Catch badImageFormatException As System.BadImageFormatException
							ProjectData.SetProjectError(badImageFormatException)
							ProjectData.ClearProjectError()
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			Catch badImageFormatException1 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException1)
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Private Sub CreateFields(ByVal members As ArrayBuilder(Of Symbol), <Out> ByRef witheventPropertyNames As HashSet(Of String))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::CreateFields(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>,System.Collections.Generic.HashSet`1<System.String>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void CreateFields(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbol>,System.Collections.Generic.HashSet<System.String>&)
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

		Private Function CreateMethods() As Dictionary(Of MethodDefinitionHandle, PEMethodSymbol)
			Dim enumerator As MethodDefinitionHandleCollection.Enumerator = New MethodDefinitionHandleCollection.Enumerator()
			Dim methodDefinitionHandles As Dictionary(Of MethodDefinitionHandle, PEMethodSymbol) = New Dictionary(Of MethodDefinitionHandle, PEMethodSymbol)()
			Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
			Dim [module] As PEModule = containingPEModule.[Module]
			Try
				Try
					enumerator = [module].GetMethodsOfTypeOrThrow(Me._handle).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As MethodDefinitionHandle = enumerator.Current
						If (Not [module].ShouldImportMethod(current, containingPEModule.ImportOptions)) Then
							Continue While
						End If
						methodDefinitionHandles.Add(current, New PEMethodSymbol(containingPEModule, Me, current))
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				ProjectData.ClearProjectError()
			End Try
			Return methodDefinitionHandles
		End Function

		Private Function CreateNestedTypes() As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol))
			Dim enumerator As IEnumerator(Of IGrouping(Of String, PENamedTypeSymbol)) = Nothing
			Dim name As Func(Of PENamedTypeSymbol, String)
			Dim instance As ArrayBuilder(Of PENamedTypeSymbol) = ArrayBuilder(Of PENamedTypeSymbol).GetInstance()
			Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
			Dim [module] As PEModule = containingPEModule.[Module]
			Try
				Dim enumerator1 As ImmutableArray(Of TypeDefinitionHandle).Enumerator = [module].GetNestedTypeDefsOrThrow(Me._handle).GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As TypeDefinitionHandle = enumerator1.Current
					If (Not [module].ShouldImportNestedType(current)) Then
						Continue While
					End If
					instance.Add(New PENamedTypeSymbol(containingPEModule, Me, current))
				End While
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				ProjectData.ClearProjectError()
			End Try
			Dim pENamedTypeSymbols As ArrayBuilder(Of PENamedTypeSymbol) = instance
			If (PENamedTypeSymbol._Closure$__.$I127-0 Is Nothing) Then
				name = Function(t As PENamedTypeSymbol) t.Name
				PENamedTypeSymbol._Closure$__.$I127-0 = name
			Else
				name = PENamedTypeSymbol._Closure$__.$I127-0
			End If
			Dim groupings As IEnumerable(Of IGrouping(Of String, PENamedTypeSymbol)) = pENamedTypeSymbols.GroupBy(Of String)(name, CaseInsensitiveComparison.Comparer)
			Using strs As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)) = New Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol))(CaseInsensitiveComparison.Comparer)
				enumerator = groupings.GetEnumerator()
				While enumerator.MoveNext()
					Dim current1 As IGrouping(Of String, PENamedTypeSymbol) = enumerator.Current
					strs.Add(current1.Key, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of PENamedTypeSymbol)(current1.ToArray()))
				End While
			End Using
			instance.Free()
			Return strs
		End Function

		Private Sub CreateProperties(ByVal methodHandleToSymbol As Dictionary(Of MethodDefinitionHandle, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol), ByVal members As ArrayBuilder(Of Symbol))
			Dim enumerator As PropertyDefinitionHandleCollection.Enumerator = New PropertyDefinitionHandleCollection.Enumerator()
			Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
			Dim [module] As PEModule = containingPEModule.[Module]
			Try
				Try
					enumerator = [module].GetPropertiesOfTypeOrThrow(Me._handle).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As PropertyDefinitionHandle = enumerator.Current
						Try
							Dim propertyMethodsOrThrow As PropertyAccessors = [module].GetPropertyMethodsOrThrow(current)
							Dim accessorMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = PENamedTypeSymbol.GetAccessorMethod(containingPEModule, methodHandleToSymbol, propertyMethodsOrThrow.Getter)
							Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = PENamedTypeSymbol.GetAccessorMethod(containingPEModule, methodHandleToSymbol, propertyMethodsOrThrow.Setter)
							If (accessorMethod IsNot Nothing OrElse pEMethodSymbol IsNot Nothing) Then
								members.Add(PEPropertySymbol.Create(containingPEModule, Me, current, accessorMethod, pEMethodSymbol))
							End If
						Catch badImageFormatException As System.BadImageFormatException
							ProjectData.SetProjectError(badImageFormatException)
							ProjectData.ClearProjectError()
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			Catch badImageFormatException1 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException1)
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Private Shared Function CyclicInheritanceError(ByVal diag As DiagnosticInfo) As ErrorTypeSymbol
			Return New ExtendedErrorTypeSymbol(diag, True, Nothing)
		End Function

		Private Function DecodeAttributeUsageInfo() As Microsoft.CodeAnalysis.AttributeUsageInfo
			Dim attributeUsageInfo As Microsoft.CodeAnalysis.AttributeUsageInfo
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol
			Dim attributeUsageInfo1 As Microsoft.CodeAnalysis.AttributeUsageInfo
			Dim attributeUsageAttributeHandle As CustomAttributeHandle = Me.ContainingPEModule.[Module].GetAttributeUsageAttributeHandle(Me._handle)
			If (Not attributeUsageAttributeHandle.IsNil) Then
				Dim typedConstantArray As TypedConstant() = Nothing
				Dim keyValuePairArray As KeyValuePair(Of String, TypedConstant)() = Nothing
				If (Not (New MetadataDecoder(Me.ContainingPEModule)).GetCustomAttribute(attributeUsageAttributeHandle, typedConstantArray, keyValuePairArray)) Then
					baseTypeNoUseSiteDiagnostics = MyBase.BaseTypeNoUseSiteDiagnostics
					attributeUsageInfo1 = If(baseTypeNoUseSiteDiagnostics IsNot Nothing, baseTypeNoUseSiteDiagnostics.GetAttributeUsageInfo(), Microsoft.CodeAnalysis.AttributeUsageInfo.[Default])
					attributeUsageInfo = attributeUsageInfo1
					Return attributeUsageInfo
				End If
				attributeUsageInfo = AttributeData.DecodeAttributeUsageAttribute(typedConstantArray(0), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, TypedConstant))(keyValuePairArray))
				Return attributeUsageInfo
			End If
			baseTypeNoUseSiteDiagnostics = MyBase.BaseTypeNoUseSiteDiagnostics
			attributeUsageInfo1 = If(baseTypeNoUseSiteDiagnostics IsNot Nothing, baseTypeNoUseSiteDiagnostics.GetAttributeUsageInfo(), Microsoft.CodeAnalysis.AttributeUsageInfo.[Default])
			attributeUsageInfo = attributeUsageInfo1
			Return attributeUsageInfo
		End Function

		Private Function DecodeIsExtensibleInterface() As Boolean
			Dim flag As Boolean
			If (Me.IsInterfaceType()) Then
				If (Not Me.HasAttributeForExtensibleInterface()) Then
					Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = MyBase.AllInterfacesNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						If (Not enumerator.Current.IsExtensibleInterfaceNoUseSiteDiagnostics) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
				Else
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Sub EnsureNestedTypesAreLoaded()
			If (Me._lazyNestedTypes Is Nothing) Then
				Dim strs As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)) = Me.CreateNestedTypes()
				Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)))(Me._lazyNestedTypes, strs, Nothing)
				If (Me._lazyNestedTypes = strs) Then
					Me.ContainingPEModule.OnNewTypeDeclarationsLoaded(strs)
				End If
			End If
		End Sub

		Private Sub EnsureNonTypeMemberNamesAreLoaded()
			Dim enumerator As MethodDefinitionHandleCollection.Enumerator = New MethodDefinitionHandleCollection.Enumerator()
			Dim enumerator1 As PropertyDefinitionHandleCollection.Enumerator = New PropertyDefinitionHandleCollection.Enumerator()
			Dim enumerator2 As EventDefinitionHandleCollection.Enumerator = New EventDefinitionHandleCollection.Enumerator()
			Dim enumerator3 As FieldDefinitionHandleCollection.Enumerator = New FieldDefinitionHandleCollection.Enumerator()
			If (Me._lazyMemberNames Is Nothing) Then
				Dim [module] As PEModule = Me.ContainingPEModule.[Module]
				Dim strs As HashSet(Of String) = New HashSet(Of String)()
				Try
					Try
						enumerator = [module].GetMethodsOfTypeOrThrow(Me._handle).GetEnumerator()
						While enumerator.MoveNext()
							Dim current As MethodDefinitionHandle = enumerator.Current
							Try
								strs.Add([module].GetMethodDefNameOrThrow(current))
							Catch badImageFormatException As System.BadImageFormatException
								ProjectData.SetProjectError(badImageFormatException)
								ProjectData.ClearProjectError()
							End Try
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				Catch badImageFormatException1 As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException1)
					ProjectData.ClearProjectError()
				End Try
				Try
					Try
						enumerator1 = [module].GetPropertiesOfTypeOrThrow(Me._handle).GetEnumerator()
						While enumerator1.MoveNext()
							Dim propertyDefinitionHandle As System.Reflection.Metadata.PropertyDefinitionHandle = enumerator1.Current
							Try
								strs.Add([module].GetPropertyDefNameOrThrow(propertyDefinitionHandle))
							Catch badImageFormatException2 As System.BadImageFormatException
								ProjectData.SetProjectError(badImageFormatException2)
								ProjectData.ClearProjectError()
							End Try
						End While
					Finally
						DirectCast(enumerator1, IDisposable).Dispose()
					End Try
				Catch badImageFormatException3 As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException3)
					ProjectData.ClearProjectError()
				End Try
				Try
					Try
						enumerator2 = [module].GetEventsOfTypeOrThrow(Me._handle).GetEnumerator()
						While enumerator2.MoveNext()
							Dim eventDefinitionHandle As System.Reflection.Metadata.EventDefinitionHandle = enumerator2.Current
							Try
								strs.Add([module].GetEventDefNameOrThrow(eventDefinitionHandle))
							Catch badImageFormatException4 As System.BadImageFormatException
								ProjectData.SetProjectError(badImageFormatException4)
								ProjectData.ClearProjectError()
							End Try
						End While
					Finally
						DirectCast(enumerator2, IDisposable).Dispose()
					End Try
				Catch badImageFormatException5 As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException5)
					ProjectData.ClearProjectError()
				End Try
				Try
					Try
						enumerator3 = [module].GetFieldsOfTypeOrThrow(Me._handle).GetEnumerator()
						While enumerator3.MoveNext()
							Dim fieldDefinitionHandle As System.Reflection.Metadata.FieldDefinitionHandle = enumerator3.Current
							Try
								strs.Add([module].GetFieldDefNameOrThrow(fieldDefinitionHandle))
							Catch badImageFormatException6 As System.BadImageFormatException
								ProjectData.SetProjectError(badImageFormatException6)
								ProjectData.ClearProjectError()
							End Try
						End While
					Finally
						DirectCast(enumerator3, IDisposable).Dispose()
					End Try
				Catch badImageFormatException7 As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException7)
					ProjectData.ClearProjectError()
				End Try
				Interlocked.CompareExchange(Of ICollection(Of String))(Me._lazyMemberNames, SpecializedCollections.ReadOnlySet(Of String)(strs), Nothing)
			End If
		End Sub

		Private Sub EnsureNonTypeMembersAreLoaded()
			Dim enumerator As Dictionary(Of MethodDefinitionHandle, PEMethodSymbol).ValueCollection.Enumerator = New Dictionary(Of MethodDefinitionHandle, PEMethodSymbol).ValueCollection.Enumerator()
			Dim enumerator1 As IEnumerator(Of IGrouping(Of String, Symbol)) = Nothing
			Dim enumerator2 As HashSet(Of String).Enumerator = New HashSet(Of String).Enumerator()
			Dim enumerator3 As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator()
			Dim name As Func(Of Symbol, String)
			If (Me._lazyMembers Is Nothing) Then
				Dim methodDefinitionHandles As Dictionary(Of MethodDefinitionHandle, PEMethodSymbol) = Me.CreateMethods()
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim flag As Boolean = If(Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum], Not MyBase.IsShared, False)
				Try
					enumerator = methodDefinitionHandles.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As PEMethodSymbol = enumerator.Current
						instance.Add(current)
						If (Not flag) Then
							Continue While
						End If
						flag = Not current.IsParameterlessConstructor()
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				If (flag) Then
					instance.Add(New SynthesizedConstructorSymbol(Nothing, Me, MyBase.IsShared, False, Nothing, Nothing))
				End If
				Dim strs As HashSet(Of String) = Nothing
				Me.CreateProperties(methodDefinitionHandles, instance)
				Me.CreateFields(instance, strs)
				Me.CreateEvents(methodDefinitionHandles, instance)
				Dim strs1 As Dictionary(Of String, ImmutableArray(Of Symbol)) = New Dictionary(Of String, ImmutableArray(Of Symbol))(CaseInsensitiveComparison.Comparer)
				Dim symbols As ArrayBuilder(Of Symbol) = instance
				If (PENamedTypeSymbol._Closure$__.$I74-0 Is Nothing) Then
					name = Function(m As Symbol) m.Name
					PENamedTypeSymbol._Closure$__.$I74-0 = name
				Else
					name = PENamedTypeSymbol._Closure$__.$I74-0
				End If
				Using groupings As IEnumerable(Of IGrouping(Of String, Symbol)) = symbols.GroupBy(Of String)(name, CaseInsensitiveComparison.Comparer)
					enumerator1 = groupings.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current1 As IGrouping(Of String, Symbol) = enumerator1.Current
						strs1.Add(current1.Key, ImmutableArray.CreateRange(Of Symbol)(current1))
					End While
				End Using
				instance.Free()
				If (strs IsNot Nothing) Then
					Try
						enumerator2 = strs.GetEnumerator()
						While enumerator2.MoveNext()
							Dim str As String = enumerator2.Current
							Dim symbols1 As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
							If (Not strs1.TryGetValue(str, symbols1) OrElse symbols1.Length <> 1) Then
								Continue While
							End If
							Dim item As PEPropertySymbol = TryCast(symbols1(0), PEPropertySymbol)
							If (item Is Nothing OrElse Not Me.IsValidWithEventsProperty(item)) Then
								Continue While
							End If
							item.SetIsWithEvents(True)
						End While
					Finally
						DirectCast(enumerator2, IDisposable).Dispose()
					End Try
				End If
				Try
					enumerator3 = Me._lazyNestedTypes.Values.GetEnumerator()
					While enumerator3.MoveNext()
						Dim pENamedTypeSymbols As ImmutableArray(Of PENamedTypeSymbol) = enumerator3.Current
						Dim name1 As String = pENamedTypeSymbols(0).Name
						Dim symbols2 As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
						If (strs1.TryGetValue(name1, symbols2)) Then
							strs1(name1) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Symbol)(symbols2, StaticCast(Of Symbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols))
						Else
							strs1.Add(name1, StaticCast(Of Symbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols))
						End If
					End While
				Finally
					DirectCast(enumerator3, IDisposable).Dispose()
				End Try
				If (Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of Symbol)))(Me._lazyMembers, strs1, Nothing) Is Nothing) Then
					Dim strs2 As ICollection(Of String) = SpecializedCollections.ReadOnlyCollection(Of String)(strs1.Keys)
					Interlocked.Exchange(Of ICollection(Of String))(Me._lazyMemberNames, strs2)
				End If
			End If
		End Sub

		Private Sub EnsureTypeParametersAreLoaded()
			If (Me._lazyTypeParameters.IsDefault) Then
				Dim pETypeParameterSymbol(' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::EnsureTypeParametersAreLoaded()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void EnsureTypeParametersAreLoaded()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( , TypeReference , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3653
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( , TypeReference , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1891
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1867
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 309
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 970
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


		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Private Shared Function GetAccessorMethod(ByVal moduleSymbol As PEModuleSymbol, ByVal methodHandleToSymbol As Dictionary(Of MethodDefinitionHandle, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol), ByVal methodDef As MethodDefinitionHandle) As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol
			Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol
			If (Not methodDef.IsNil) Then
				Dim pEMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = Nothing
				methodHandleToSymbol.TryGetValue(methodDef, pEMethodSymbol1)
				pEMethodSymbol = pEMethodSymbol1
			Else
				pEMethodSymbol = Nothing
			End If
			Return pEMethodSymbol
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			If (Me._lazyConditionalAttributeSymbols.IsDefault) Then
				Dim conditionalAttributeValues As ImmutableArray(Of String) = Me.ContainingPEModule.[Module].GetConditionalAttributeValues(Me._handle)
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				ImmutableInterlocked.InterlockedCompareExchange(Of String)(Me._lazyConditionalAttributeSymbols, conditionalAttributeValues, strs)
			End If
			Return Me._lazyConditionalAttributeSymbols
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			' 
			' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData> Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::GetAttributes()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData> GetAttributes()
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

		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			If (Me._lazyAttributeUsageInfo.IsNull) Then
				Me._lazyAttributeUsageInfo = Me.DecodeAttributeUsageInfo()
			End If
			Return Me._lazyAttributeUsageInfo
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return New PENamedTypeSymbol.VB$StateMachine_63_GetCustomAttributesToEmit(-2) With
			{
				.$VB$Me = Me,
				.$P_compilationState = compilationState
			}
		End Function

		Private Function GetDefaultPropertyName() As String
			Dim str As String
			Dim str1 As String = Nothing
			Me.ContainingPEModule.[Module].HasDefaultMemberAttribute(Me._handle, str1)
			If (str1 IsNot Nothing) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers(str1).GetEnumerator()
				While enumerator.MoveNext()
					If (enumerator.Current.Kind <> SymbolKind.[Property]) Then
						Continue While
					End If
					str = str1
					Return str
				End While
			End If
			str = Nothing
			Return str
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return PEDocumentationCommentUtils.GetDocumentationComment(Me, Me.ContainingPEModule, preferredCulture, cancellationToken, Me._lazyDocComment)
		End Function

		Friend Overrides Function GetEventsToEmit() As IEnumerable(Of EventSymbol)
			Return PENamedTypeSymbol.GetMembers(Of EventSymbol)(Me.GetMembers(), SymbolKind.[Event], -1)
		End Function

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return PENamedTypeSymbol.GetMembers(Of FieldSymbol)(Me.GetMembers(), SymbolKind.Field, 0)
		End Function

		Friend Overrides Function GetGuidString(ByRef guidString As String) As Boolean
			Return Me.ContainingPEModule.[Module].HasGuidAttribute(Me._handle, guidString)
		End Function

		Private Shared Function GetIndexOfFirstMember(ByVal members As ImmutableArray(Of Symbol), ByVal kind As SymbolKind) As Integer
			Dim num As Integer
			Dim length As Integer = members.Length
			Dim num1 As Integer = length - 1
			Dim num2 As Integer = 0
			While True
				If (num2 > num1) Then
					num = length
					Exit While
				ElseIf (members(num2).Kind <> kind) Then
					num2 = num2 + 1
				Else
					num = num2
					Exit While
				End If
			End While
			Return num
		End Function

		Friend Overrides Function GetInterfacesToEmit() As IEnumerable(Of NamedTypeSymbol)
			Return DirectCast(MyBase.InterfacesNoUseSiteDiagnostics, IEnumerable(Of NamedTypeSymbol))
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Me.EnsureNestedTypesAreLoaded()
			Me.EnsureNonTypeMembersAreLoaded()
			Return Me._lazyMembers.Flatten(PENamedTypeSymbol.DeclarationOrderSymbolComparer.Instance)
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Me.EnsureNestedTypesAreLoaded()
			Me.EnsureNonTypeMembersAreLoaded()
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			If (Not Me._lazyMembers.TryGetValue(name, symbols)) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = symbols
			End If
			Return empty
		End Function

		Private Shared Function GetMembers(Of TSymbol As Symbol)(ByVal members As ImmutableArray(Of Symbol), ByVal kind As SymbolKind, Optional ByVal offset As Integer = -1) As IEnumerable(Of TSymbol)
			Return New PENamedTypeSymbol.VB$StateMachine_152_GetMembers(Of TSymbol)(-2) With
			{
				.$P_members = members,
				.$P_kind = kind,
				.$P_offset = offset
			}
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			Me.EnsureNestedTypesAreLoaded()
			Me.EnsureNonTypeMembersAreLoaded()
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ConditionallyDeOrder(Of Symbol)(Me._lazyMembers.Flatten(Nothing))
		End Function

		Friend Overrides Function GetMethodsToEmit() As IEnumerable(Of MethodSymbol)
			Return New PENamedTypeSymbol.VB$StateMachine_70_GetMethodsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Function GetPropertiesToEmit() As IEnumerable(Of PropertySymbol)
			Return PENamedTypeSymbol.GetMembers(Of PropertySymbol)(Me.GetMembers(), SymbolKind.[Property], -1)
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Me.EnsureNestedTypesAreLoaded()
			Return StaticCast(Of NamedTypeSymbol).From(Of PENamedTypeSymbol)(Me._lazyNestedTypes.Flatten(PENamedTypeSymbol.DeclarationOrderSymbolComparer.Instance))
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Me.EnsureNestedTypesAreLoaded()
			Dim pENamedTypeSymbols As ImmutableArray(Of PENamedTypeSymbol) = New ImmutableArray(Of PENamedTypeSymbol)()
			If (Not Me._lazyNestedTypes.TryGetValue(name, pENamedTypeSymbols)) Then
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			Else
				empty = StaticCast(Of NamedTypeSymbol).From(Of PENamedTypeSymbol)(pENamedTypeSymbols)
			End If
			Return empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, Integer, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (PENamedTypeSymbol._Closure$__.$I81-0 Is Nothing) Then
				func = Function(type As NamedTypeSymbol, arity_ As Integer) type.Arity = arity_
				PENamedTypeSymbol._Closure$__.$I81-0 = func
			Else
				func = PENamedTypeSymbol._Closure$__.$I81-0
			End If
			Return typeMembers.WhereAsArray(Of Integer)(func, arity)
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Me.EnsureNestedTypesAreLoaded()
			Return StaticCast(Of NamedTypeSymbol).From(Of PENamedTypeSymbol)(Me._lazyNestedTypes.Flatten(Nothing))
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, Me.CalculateUseSiteInfoImpl())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Private Function HasAttributeForExtensibleInterface() As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::HasAttributeForExtensibleInterface()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean HasAttributeForExtensibleInterface()
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

		End Function

		Private Function IsValidWithEventsProperty(ByVal prop As PEPropertySymbol) As Boolean
			Dim flag As Boolean
			If (Not (prop.IsReadOnly Or prop.IsWriteOnly)) Then
				flag = If(prop.IsOverridable, True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim declaredBase As NamedTypeSymbol
			Dim dependencyDiagnosticsForImportedClass As DiagnosticInfo = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedClass(Me)
			If (dependencyDiagnosticsForImportedClass Is Nothing) Then
				declaredBase = Me.GetDeclaredBase(New BasesBeingResolved())
			Else
				declaredBase = PENamedTypeSymbol.CyclicInheritanceError(dependencyDiagnosticsForImportedClass)
			End If
			Return declaredBase
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			Dim func
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.GetDeclaredInterfacesNoUseSiteDiagnostics(New BasesBeingResolved())
			If (Me.IsInterface) Then
				Dim collection = declaredInterfacesNoUseSiteDiagnostics.[Select](Function(t As NamedTypeSymbol) New With { Key .t = t, Key .diag = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedBaseInterface(Me, t) })
				If (PENamedTypeSymbol._Closure$__.$I53-1 Is Nothing) Then
					func = Function(argument0)
						If (argument0.diag Is Nothing) Then
							Return argument0.t
						End If
						Return PENamedTypeSymbol.CyclicInheritanceError(argument0.diag)
					End Function
					PENamedTypeSymbol._Closure$__.$I53-1 = func
				Else
					func = PENamedTypeSymbol._Closure$__.$I53-1
				End If
				namedTypeSymbols = collection.[Select](func).AsImmutable()
			Else
				namedTypeSymbols = declaredInterfacesNoUseSiteDiagnostics
			End If
			Return namedTypeSymbols
		End Function

		Private Function MakeComImportCoClassType() As TypeSymbol
			Dim typeSymbolForSerializedType As TypeSymbol
			If (Me.IsInterface) Then
				Dim str As String = Nothing
				If (Me.ContainingPEModule.[Module].HasStringValuedAttribute(Me._handle, AttributeDescription.CoClassAttribute, str)) Then
					typeSymbolForSerializedType = (New MetadataDecoder(Me.ContainingPEModule)).GetTypeSymbolForSerializedType(str)
				Else
					typeSymbolForSerializedType = Nothing
				End If
			Else
				typeSymbolForSerializedType = Nothing
			End If
			Return typeSymbolForSerializedType
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol::MakeDeclaredBase(Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol MakeDeclaredBase(Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim enumerator As InterfaceImplementationHandleCollection.Enumerator = New InterfaceImplementationHandleCollection.Enumerator()
			Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Try
				Dim containingPEModule As PEModuleSymbol = Me.ContainingPEModule
				Dim interfaceImplementationsOrThrow As InterfaceImplementationHandleCollection = containingPEModule.[Module].GetInterfaceImplementationsOrThrow(Me._handle)
				If (interfaceImplementationsOrThrow.Count <> 0) Then
					Dim namedTypeSymbolArray(interfaceImplementationsOrThrow.Count - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
					Dim metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder(containingPEModule, Me)
					Dim num As Integer = 0
					Try
						enumerator = interfaceImplementationsOrThrow.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As InterfaceImplementationHandle = enumerator.Current
							Dim [interface] As EntityHandle = containingPEModule.[Module].MetadataReader.GetInterfaceImplementation(current).[Interface]
							Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(DirectCast(TupleTypeDecoder.DecodeTupleTypesIfApplicable(metadataDecoder.GetTypeOfToken([interface]), current, containingPEModule), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							Dim namedTypeSymbolArray1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol() = namedTypeSymbolArray
							Dim num1 As Integer = num
							If (namedTypeSymbol IsNot Nothing) Then
								unsupportedMetadataTypeSymbol = namedTypeSymbol
							Else
								unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, System.BadImageFormatException))
							End If
							namedTypeSymbolArray1(num1) = unsupportedMetadataTypeSymbol
							num = num + 1
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
					empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(namedTypeSymbolArray)
				Else
					empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Empty
				End If
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(badImageFormatException))
				ProjectData.ClearProjectError()
			End Try
			Return empty
		End Function

		Private Function MatchesContainingTypeParameters() As Boolean
			Dim flag As Boolean
			If (Me._genericParameterHandles.Count <> 0) Then
				Dim containingType As NamedTypeSymbol = Me.ContainingType
				If (containingType IsNot Nothing) Then
					Dim allTypeParameters As ImmutableArray(Of TypeParameterSymbol) = containingType.GetAllTypeParameters()
					Dim length As Integer = allTypeParameters.Length
					If (length <> 0) Then
						Dim pENamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol(Me.ContainingPEModule, DirectCast(MyBase.ContainingNamespace, PENamespaceSymbol), Me._handle)
						Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = pENamedTypeSymbol.TypeParameters
						Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = IndexedTypeParameterSymbol.Take(length)
						Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(containingType, allTypeParameters, typeParameterSymbols.[As](Of TypeSymbol)(), False)
						Dim typeParameterSymbols1 As ImmutableArray(Of TypeParameterSymbol) = typeParameters
						typeParameterSymbols = IndexedTypeParameterSymbol.Take(typeParameters.Length)
						Dim typeSubstitution1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(pENamedTypeSymbol, typeParameterSymbols1, typeParameterSymbols.[As](Of TypeSymbol)(), False)
						Dim num As Integer = length - 1
						Dim num1 As Integer = 0
						While num1 <= num
							If (MethodSignatureComparer.HaveSameConstraints(allTypeParameters(num1), typeSubstitution, typeParameters(num1), typeSubstitution1)) Then
								num1 = num1 + 1
							Else
								flag = False
								Return flag
							End If
						End While
						flag = True
					Else
						flag = True
					End If
				Else
					flag = True
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Class DeclarationOrderSymbolComparer
			Implements IComparer(Of ISymbol)
			Public ReadOnly Shared Instance As PENamedTypeSymbol.DeclarationOrderSymbolComparer

			Shared Sub New()
				PENamedTypeSymbol.DeclarationOrderSymbolComparer.Instance = New PENamedTypeSymbol.DeclarationOrderSymbolComparer()
			End Sub

			Private Sub New()
				MyBase.New()
			End Sub

			Public Function Compare(ByVal x As ISymbol, ByVal y As ISymbol) As Integer Implements IComparer(Of ISymbol).Compare
				Dim num As Integer
				If (x <> y) Then
					Dim sortOrder As Integer = x.Kind.ToSortOrder() - y.Kind.ToSortOrder()
					If (sortOrder = 0) Then
						Dim kind As SymbolKind = x.Kind
						Select Case kind
							Case SymbolKind.[Event]
								num = HandleComparer.[Default].Compare(DirectCast(x, PEEventSymbol).Handle, DirectCast(y, PEEventSymbol).Handle)
								Exit Select
							Case SymbolKind.Field
								num = HandleComparer.[Default].Compare(DirectCast(x, PEFieldSymbol).Handle, DirectCast(y, PEFieldSymbol).Handle)
								Exit Select
							Case SymbolKind.Label
							Case SymbolKind.Local
							Case SymbolKind.NetModule
								Throw ExceptionUtilities.UnexpectedValue(x.Kind)
							Case SymbolKind.Method
								If (DirectCast(x, MethodSymbol).IsDefaultValueTypeConstructor()) Then
									num = -1
									Exit Select
								ElseIf (Not DirectCast(y, MethodSymbol).IsDefaultValueTypeConstructor()) Then
									num = HandleComparer.[Default].Compare(DirectCast(x, PEMethodSymbol).Handle, DirectCast(y, PEMethodSymbol).Handle)
									Exit Select
								Else
									num = 1
									Exit Select
								End If
							Case SymbolKind.NamedType
								num = HandleComparer.[Default].Compare(DirectCast(x, PENamedTypeSymbol).Handle, DirectCast(y, PENamedTypeSymbol).Handle)
								Exit Select
							Case Else
								If (kind = SymbolKind.[Property]) Then
									num = HandleComparer.[Default].Compare(DirectCast(x, PEPropertySymbol).Handle, DirectCast(y, PEPropertySymbol).Handle)
									Exit Select
								Else
									Throw ExceptionUtilities.UnexpectedValue(x.Kind)
								End If
						End Select
					Else
						num = sortOrder
					End If
				Else
					num = 0
				End If
				Return num
			End Function
		End Class
	End Class
End Namespace