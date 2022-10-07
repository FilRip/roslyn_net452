Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceAssemblySymbol
		Inherits MetadataOrSourceAssemblySymbol
		Implements ISourceAssemblySymbol, ISourceAssemblySymbolInternal, IAttributeTargetSymbol
		Private ReadOnly _compilation As VisualBasicCompilation

		Private _lazyStrongNameKeys As Microsoft.CodeAnalysis.StrongNameKeys

		Friend m_lazyIdentity As AssemblyIdentity

		Private ReadOnly _modules As ImmutableArray(Of ModuleSymbol)

		Private _lazySourceAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _lazyNetModuleAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _lazyDuplicateAttributeIndices As HashSet(Of Integer)

		Private _lazyEmitExtensionAttribute As Byte

		Private _lazyIsVbRuntime As ThreeState

		Private _lazyAssemblyLevelDeclarationErrors As ImmutableArray(Of Diagnostic)

		Private _lazyAssemblyLevelDeclarationDependencies As ImmutableArray(Of AssemblySymbol)

		Private ReadOnly _assemblySimpleName As String

		Private _lazyInternalsVisibleToMap As ConcurrentDictionary(Of String, ConcurrentDictionary(Of ImmutableArray(Of Byte), Tuple(Of Location, String)))

		<ThreadStatic>
		Private Shared s_AssemblyForWhichCurrentThreadIsComputingKeys As AssemblySymbol

		Private _optimisticallyGrantedInternalsAccess As ConcurrentDictionary(Of AssemblySymbol, Boolean)

		Friend ReadOnly Property AssemblyAlgorithmIdAttributeSetting As Nullable(Of AssemblyHashAlgorithm)
			Get
				Dim nullable As Nullable(Of AssemblyHashAlgorithm) = Nothing
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					nullable = sourceDecodedWellKnownAttributeData.AssemblyAlgorithmIdAttributeSetting
				End If
				If (Not nullable.HasValue) Then
					sourceDecodedWellKnownAttributeData = Me.GetNetModuleDecodedWellKnownAttributeData()
					If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
						nullable = sourceDecodedWellKnownAttributeData.AssemblyAlgorithmIdAttributeSetting
					End If
				End If
				Return nullable
			End Get
		End Property

		Private ReadOnly Property AssemblyCultureAttributeSetting As String
			Get
				Dim func As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I73-0 Is Nothing) Then
					func = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyCultureAttributeSetting
					SourceAssemblySymbol._Closure$__.$I73-0 = func
				Else
					func = SourceAssemblySymbol._Closure$__.$I73-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(func, Nothing)
			End Get
		End Property

		Public ReadOnly Property AssemblyFlags As System.Reflection.AssemblyFlags Implements ISourceAssemblySymbolInternal.AssemblyFlags
			Get
				Dim assemblyFlagsAttributeSetting As System.Reflection.AssemblyFlags = 0
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					assemblyFlagsAttributeSetting = sourceDecodedWellKnownAttributeData.AssemblyFlagsAttributeSetting
				End If
				sourceDecodedWellKnownAttributeData = Me.GetNetModuleDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					assemblyFlagsAttributeSetting = assemblyFlagsAttributeSetting Or sourceDecodedWellKnownAttributeData.AssemblyFlagsAttributeSetting
				End If
				Return assemblyFlagsAttributeSetting
			End Get
		End Property

		Private ReadOnly Property AssemblyKeyContainerAttributeSetting As String
			Get
				Dim func As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I69-0 Is Nothing) Then
					func = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyKeyContainerAttributeSetting
					SourceAssemblySymbol._Closure$__.$I69-0 = func
				Else
					func = SourceAssemblySymbol._Closure$__.$I69-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(func, WellKnownAttributeData.StringMissingValue)
			End Get
		End Property

		Private ReadOnly Property AssemblyKeyFileAttributeSetting As String
			Get
				Dim func As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I71-0 Is Nothing) Then
					func = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyKeyFileAttributeSetting
					SourceAssemblySymbol._Closure$__.$I71-0 = func
				Else
					func = SourceAssemblySymbol._Closure$__.$I71-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(func, WellKnownAttributeData.StringMissingValue)
			End Get
		End Property

		Private ReadOnly Property AssemblyVersionAttributeSetting As Version
			Get
				Dim version As System.Version = Nothing
				Dim version1 As System.Version = version
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					version1 = sourceDecodedWellKnownAttributeData.AssemblyVersionAttributeSetting
				End If
				If (CObj(version1) = CObj(version)) Then
					sourceDecodedWellKnownAttributeData = Me.GetNetModuleDecodedWellKnownAttributeData()
					If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
						version1 = sourceDecodedWellKnownAttributeData.AssemblyVersionAttributeSetting
					End If
				End If
				Return version1
			End Get
		End Property

		Public Overrides ReadOnly Property AssemblyVersionPattern As Version
			Get
				Dim assemblyVersionAttributeSetting As Version = Me.AssemblyVersionAttributeSetting
				If (assemblyVersionAttributeSetting IsNot Nothing AndAlso (assemblyVersionAttributeSetting.Build = 65535 OrElse assemblyVersionAttributeSetting.Revision = 65535)) Then
					Return assemblyVersionAttributeSetting
				End If
				Return Nothing
			End Get
		End Property

		Friend ReadOnly Property Company As String
			Get
				Dim assemblyCompanyAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I53-0 Is Nothing) Then
					assemblyCompanyAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyCompanyAttributeSetting
					SourceAssemblySymbol._Closure$__.$I53-0 = assemblyCompanyAttributeSetting
				Else
					assemblyCompanyAttributeSetting = SourceAssemblySymbol._Closure$__.$I53-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyCompanyAttributeSetting, Nothing)
			End Get
		End Property

		Friend ReadOnly Property Copyright As String
			Get
				Dim assemblyCopyrightAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I59-0 Is Nothing) Then
					assemblyCopyrightAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyCopyrightAttributeSetting
					SourceAssemblySymbol._Closure$__.$I59-0 = assemblyCopyrightAttributeSetting
				Else
					assemblyCopyrightAttributeSetting = SourceAssemblySymbol._Closure$__.$I59-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyCopyrightAttributeSetting, Nothing)
			End Get
		End Property

		Friend ReadOnly Property DeclaresTheObjectClass As Boolean
			Get
				Dim flag As Boolean
				If (MyBase.CorLibrary = Me) Then
					Dim specialType As NamedTypeSymbol = MyBase.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
					flag = If(specialType.IsErrorType(), False, specialType.DeclaredAccessibility = Accessibility.[Public])
				Else
					flag = False
				End If
				Return flag
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Me._compilation
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.Assembly
			End Get
		End Property

		Private ReadOnly Property DelaySignAttributeSetting As Boolean
			Get
				Dim flag As Boolean = False
				Dim assemblyDelaySignAttributeSetting As Boolean = flag
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					assemblyDelaySignAttributeSetting = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting = ThreeState.[True]
				End If
				If (assemblyDelaySignAttributeSetting = flag) Then
					sourceDecodedWellKnownAttributeData = Me.GetNetModuleDecodedWellKnownAttributeData()
					If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
						assemblyDelaySignAttributeSetting = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting = ThreeState.[True]
					End If
				End If
				Return assemblyDelaySignAttributeSetting
			End Get
		End Property

		Friend ReadOnly Property Description As String
			Get
				Dim assemblyDescriptionAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I51-0 Is Nothing) Then
					assemblyDescriptionAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyDescriptionAttributeSetting
					SourceAssemblySymbol._Closure$__.$I51-0 = assemblyDescriptionAttributeSetting
				Else
					assemblyDescriptionAttributeSetting = SourceAssemblySymbol._Closure$__.$I51-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyDescriptionAttributeSetting, Nothing)
			End Get
		End Property

		Friend ReadOnly Property FileVersion As String
			Get
				Dim assemblyFileVersionAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I47-0 Is Nothing) Then
					assemblyFileVersionAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyFileVersionAttributeSetting
					SourceAssemblySymbol._Closure$__.$I47-0 = assemblyFileVersionAttributeSetting
				Else
					assemblyFileVersionAttributeSetting = SourceAssemblySymbol._Closure$__.$I47-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyFileVersionAttributeSetting, Nothing)
			End Get
		End Property

		Private ReadOnly Property HasAssemblyOrModuleDebuggableAttribute As Boolean
			Get
				Dim flag As Boolean
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData Is Nothing OrElse Not sourceDecodedWellKnownAttributeData.HasDebuggableAttribute) Then
					Dim decodedWellKnownAttributeData As CommonModuleWellKnownAttributeData = Me.SourceModule.GetDecodedWellKnownAttributeData()
					flag = If(decodedWellKnownAttributeData Is Nothing OrElse Not decodedWellKnownAttributeData.HasDebuggableAttribute, False, True)
				Else
					flag = True
				End If
				Return flag
			End Get
		End Property

		Public ReadOnly Property HashAlgorithm As AssemblyHashAlgorithm Implements ISourceAssemblySymbolInternal.HashAlgorithm
			Get
				Dim assemblyAlgorithmIdAttributeSetting As Nullable(Of AssemblyHashAlgorithm) = Me.AssemblyAlgorithmIdAttributeSetting
				Dim nullable As Nullable(Of AssemblyHashAlgorithm) = assemblyAlgorithmIdAttributeSetting
				If (Not assemblyAlgorithmIdAttributeSetting.HasValue) Then
					Return AssemblyHashAlgorithm.Sha1
				End If
				Return nullable.GetValueOrDefault()
			End Get
		End Property

		Private ReadOnly Property HasReferenceAssemblyAttribute As Boolean
			Get
				Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return sourceDecodedWellKnownAttributeData.HasReferenceAssemblyAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property Identity As AssemblyIdentity
			Get
				If (Me.m_lazyIdentity Is Nothing) Then
					Interlocked.CompareExchange(Of AssemblyIdentity)(Me.m_lazyIdentity, Me.ComputeIdentity(), Nothing)
				End If
				Return Me.m_lazyIdentity
			End Get
		End Property

		Friend ReadOnly Property InformationalVersion As String
			Get
				Dim assemblyInformationalVersionAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I57-0 Is Nothing) Then
					assemblyInformationalVersionAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyInformationalVersionAttributeSetting
					SourceAssemblySymbol._Closure$__.$I57-0 = assemblyInformationalVersionAttributeSetting
				Else
					assemblyInformationalVersionAttributeSetting = SourceAssemblySymbol._Closure$__.$I57-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyInformationalVersionAttributeSetting, Nothing)
			End Get
		End Property

		Public ReadOnly Property InternalsAreVisible As Boolean Implements ISourceAssemblySymbolInternal.InternalsAreVisible
			Get
				Me.EnsureAttributesAreBound()
				Return Me._lazyInternalsVisibleToMap IsNot Nothing
			End Get
		End Property

		Friend ReadOnly Property IsDelaySigned As Boolean
			Get
				Dim flag As Boolean
				Me.EnsureAttributesAreBound()
				flag = If(Not Me.DeclaringCompilation.Options.DelaySign.HasValue, Me.DelaySignAttributeSetting, Me.DeclaringCompilation.Options.DelaySign.Value)
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property IsInteractive As Boolean
			Get
				Return Me._compilation.IsSubmission
			End Get
		End Property

		Friend Overrides ReadOnly Property IsLinked As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ISourceAssemblySymbol_Compilation As Microsoft.CodeAnalysis.Compilation Implements ISourceAssemblySymbol.Compilation
			Get
				Return Me._compilation
			End Get
		End Property

		Friend ReadOnly Property IsVbRuntime As Boolean
			Get
				If (Me._lazyIsVbRuntime = ThreeState.Unknown) Then
					Me._lazyIsVbRuntime = Me.CheckForRuntime().ToThreeState()
				End If
				Return Me._lazyIsVbRuntime = ThreeState.[True]
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim func As Func(Of ModuleSymbol, IEnumerable(Of Location))
				Dim modules As IEnumerable(Of ModuleSymbol) = DirectCast(Me.Modules, IEnumerable(Of ModuleSymbol))
				If (SourceAssemblySymbol._Closure$__.$I84-0 Is Nothing) Then
					func = Function(m As ModuleSymbol) DirectCast(m.Locations, IEnumerable(Of Location))
					SourceAssemblySymbol._Closure$__.$I84-0 = func
				Else
					func = SourceAssemblySymbol._Closure$__.$I84-0
				End If
				Return modules.SelectMany(Of Location)(func).AsImmutable()
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me.SourceModule.MightContainExtensionMethods
			End Get
		End Property

		Public Overrides ReadOnly Property Modules As ImmutableArray(Of ModuleSymbol)
			Get
				Return Me._modules
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._assemblySimpleName
			End Get
		End Property

		Friend ReadOnly Property Product As String
			Get
				Dim assemblyProductAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I55-0 Is Nothing) Then
					assemblyProductAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyProductAttributeSetting
					SourceAssemblySymbol._Closure$__.$I55-0 = assemblyProductAttributeSetting
				Else
					assemblyProductAttributeSetting = SourceAssemblySymbol._Closure$__.$I55-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyProductAttributeSetting, Nothing)
			End Get
		End Property

		Friend Overrides ReadOnly Property PublicKey As ImmutableArray(Of Byte)
			Get
				Return Me.StrongNameKeys.PublicKey
			End Get
		End Property

		Public ReadOnly Property SignatureKey As String Implements ISourceAssemblySymbolInternal.SignatureKey
			Get
				Dim assemblySignatureKeyAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I67-0 Is Nothing) Then
					assemblySignatureKeyAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblySignatureKeyAttributeSetting
					SourceAssemblySymbol._Closure$__.$I67-0 = assemblySignatureKeyAttributeSetting
				Else
					assemblySignatureKeyAttributeSetting = SourceAssemblySymbol._Closure$__.$I67-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblySignatureKeyAttributeSetting, Nothing)
			End Get
		End Property

		Friend ReadOnly Property SourceModule As SourceModuleSymbol
			Get
				Return DirectCast(Me._modules(0), SourceModuleSymbol)
			End Get
		End Property

		Friend ReadOnly Property StrongNameKeys As Microsoft.CodeAnalysis.StrongNameKeys
			Get
				If (Me._lazyStrongNameKeys Is Nothing) Then
					Try
						SourceAssemblySymbol.s_AssemblyForWhichCurrentThreadIsComputingKeys = Me
						Me.ComputeAndSetStrongNameKeys()
					Finally
						SourceAssemblySymbol.s_AssemblyForWhichCurrentThreadIsComputingKeys = Nothing
					End Try
				End If
				Return Me._lazyStrongNameKeys
			End Get
		End Property

		Friend ReadOnly Property Title As String
			Get
				Dim assemblyTitleAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I49-0 Is Nothing) Then
					assemblyTitleAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyTitleAttributeSetting
					SourceAssemblySymbol._Closure$__.$I49-0 = assemblyTitleAttributeSetting
				Else
					assemblyTitleAttributeSetting = SourceAssemblySymbol._Closure$__.$I49-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyTitleAttributeSetting, Nothing)
			End Get
		End Property

		Friend ReadOnly Property Trademark As String
			Get
				Dim assemblyTrademarkAttributeSetting As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String)
				If (SourceAssemblySymbol._Closure$__.$I61-0 Is Nothing) Then
					assemblyTrademarkAttributeSetting = Function(data As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)) data.AssemblyTrademarkAttributeSetting
					SourceAssemblySymbol._Closure$__.$I61-0 = assemblyTrademarkAttributeSetting
				Else
					assemblyTrademarkAttributeSetting = SourceAssemblySymbol._Closure$__.$I61-0
				End If
				Return Me.GetWellKnownAttributeDataStringField(assemblyTrademarkAttributeSetting, Nothing)
			End Get
		End Property

		Friend Sub New(ByVal compilation As VisualBasicCompilation, ByVal assemblySimpleName As String, ByVal moduleName As String, ByVal netModules As ImmutableArray(Of PEModule))
			MyBase.New()
			Me._lazyEmitExtensionAttribute = 0
			Me._lazyIsVbRuntime = ThreeState.Unknown
			Me._compilation = compilation
			Me._assemblySimpleName = assemblySimpleName
			Dim moduleSymbols As ArrayBuilder(Of ModuleSymbol) = New ArrayBuilder(Of ModuleSymbol)(1 + netModules.Length) From
			{
				New SourceModuleSymbol(Me, compilation.Declarations, compilation.Options, moduleName)
			}
			Dim metadataImportOption As MetadataImportOptions = If(compilation.Options.MetadataImportOptions = MetadataImportOptions.All, MetadataImportOptions.All, MetadataImportOptions.Internal)
			Dim enumerator As ImmutableArray(Of PEModule).Enumerator = netModules.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As PEModule = enumerator.Current
				moduleSymbols.Add(New PEModuleSymbol(Me, current, metadataImportOption, moduleSymbols.Count))
			End While
			Me._modules = moduleSymbols.ToImmutableAndFree()
			If (Not compilation.Options.CryptoPublicKey.IsEmpty) Then
				Dim nullable As Nullable(Of RSAParameters) = Nothing
				Me._lazyStrongNameKeys = Microsoft.CodeAnalysis.StrongNameKeys.Create(compilation.Options.CryptoPublicKey, nullable, False, MessageProvider.Instance)
			End If
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim hasAssemblyRuntimeCompatibilityAttribute As Func(Of ModuleSymbol, Boolean)
			Dim hasAssemblyCompilationRelaxationsAttribute As Func(Of ModuleSymbol, Boolean)
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim options As VisualBasicCompilationOptions = Me.DeclaringCompilation.Options
			Dim flag2 As Boolean = options.OutputKind.IsNetModule()
			If (Me._lazyEmitExtensionAttribute = 2) Then
				Symbol.AddSynthesizedAttribute(attributes, Me._compilation.SynthesizeExtensionAttribute())
			End If
			If (Me.DeclaringCompilation.EmbeddedSymbolManager.IsAnySymbolReferenced) Then
				Dim declaringCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = New ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant)()
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.Microsoft_VisualBasic_Embedded__ctor, typedConstants, keyValuePairs, False))
			End If
			If (flag2) Then
				flag = False
			Else
				Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
				If (SourceAssemblySymbol._Closure$__.$I122-0 Is Nothing) Then
					hasAssemblyCompilationRelaxationsAttribute = Function(m As ModuleSymbol) m.HasAssemblyCompilationRelaxationsAttribute
					SourceAssemblySymbol._Closure$__.$I122-0 = hasAssemblyCompilationRelaxationsAttribute
				Else
					hasAssemblyCompilationRelaxationsAttribute = SourceAssemblySymbol._Closure$__.$I122-0
				End If
				flag = Not modules.Any(hasAssemblyCompilationRelaxationsAttribute)
			End If
			If (flag AndAlso Not TypeOf Me.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) Is MissingMetadataTypeSymbol) Then
				Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(specialType, TypedConstantKind.Primitive, 8)
				Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants1 As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant)
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, visualBasicCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, typedConstants1, keyValuePairs, False))
			End If
			If (flag2) Then
				flag1 = False
			Else
				Dim moduleSymbols As ImmutableArray(Of ModuleSymbol) = Me.Modules
				If (SourceAssemblySymbol._Closure$__.$I122-1 Is Nothing) Then
					hasAssemblyRuntimeCompatibilityAttribute = Function(m As ModuleSymbol) m.HasAssemblyRuntimeCompatibilityAttribute
					SourceAssemblySymbol._Closure$__.$I122-1 = hasAssemblyRuntimeCompatibilityAttribute
				Else
					hasAssemblyRuntimeCompatibilityAttribute = SourceAssemblySymbol._Closure$__.$I122-1
				End If
				flag1 = Not moduleSymbols.Any(hasAssemblyRuntimeCompatibilityAttribute)
			End If
			If (flag1 AndAlso Not TypeOf Me.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) Is MissingMetadataTypeSymbol) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				Dim typedConstant1 As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(namedTypeSymbol, TypedConstantKind.Primitive, True)
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant).Empty, ImmutableArray.Create(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))(New KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant)(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, typedConstant1)), False))
			End If
			If (Not flag2 AndAlso Not Me.HasAssemblyOrModuleDebuggableAttribute) Then
				Dim specialType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				If (specialType1.GetUseSiteInfo().DiagnosticInfo Is Nothing) Then
					Dim debuggingMode As DebuggableAttribute.DebuggingModes = DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints
					If (options.OptimizationLevel = OptimizationLevel.Debug) Then
						debuggingMode = debuggingMode Or DebuggableAttribute.DebuggingModes.[Default] Or DebuggableAttribute.DebuggingModes.DisableOptimizations
					End If
					If (options.EnableEditAndContinue) Then
						debuggingMode = debuggingMode Or DebuggableAttribute.DebuggingModes.EnableEditAndContinue
					End If
					Dim typedConstant2 As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(specialType1, TypedConstantKind.[Enum], CInt(debuggingMode))
					Dim declaringCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me.DeclaringCompilation
					Dim typedConstants2 As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant2)
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation1.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggableAttribute__ctorDebuggingModes, typedConstants2, keyValuePairs, False))
				End If
			End If
			If (Me._compilation.Options.OutputKind = OutputKind.NetModule) Then
				If (Not [String].IsNullOrEmpty(Me._compilation.Options.CryptoKeyContainer) AndAlso CObj(Me.AssemblyKeyContainerAttributeSetting) = CObj(WellKnownAttributeData.StringMissingValue)) Then
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String)
					Dim typedConstant3 As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(namedTypeSymbol1, TypedConstantKind.Primitive, Me._compilation.Options.CryptoKeyContainer)
					Dim visualBasicCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me._compilation
					Dim typedConstants3 As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant3)
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, visualBasicCompilation1.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, typedConstants3, keyValuePairs, False))
				End If
				If (Not [String].IsNullOrEmpty(Me._compilation.Options.CryptoKeyFile) AndAlso CObj(Me.AssemblyKeyFileAttributeSetting) = CObj(WellKnownAttributeData.StringMissingValue)) Then
					Dim specialType2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String)
					Dim typedConstant4 As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(specialType2, TypedConstantKind.Primitive, Me._compilation.Options.CryptoKeyFile)
					Dim visualBasicCompilation2 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me._compilation
					Dim typedConstants4 As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant4)
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, visualBasicCompilation2.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, typedConstants4, keyValuePairs, False))
				End If
			End If
		End Sub

		Private Shared Function AddUniqueAssemblyAttribute(ByVal attribute As VisualBasicAttributeData, ByRef uniqueAttributes As HashSet(Of VisualBasicAttributeData)) As Boolean
			If (uniqueAttributes Is Nothing) Then
				uniqueAttributes = New HashSet(Of VisualBasicAttributeData)(CommonAttributeDataComparer.Instance)
			End If
			Return uniqueAttributes.Add(attribute)
		End Function

		Friend Sub AnErrorHasBeenReportedAboutExtensionAttribute()
			Me._lazyEmitExtensionAttribute = 1
		End Sub

		Friend Overrides Function AreInternalsVisibleToThisAssembly(ByVal potentialGiverOfAccess As AssemblySymbol) As Boolean
			Dim flag As Boolean
			If (Me._lazyStrongNameKeys IsNot Nothing OrElse SourceAssemblySymbol.s_AssemblyForWhichCurrentThreadIsComputingKeys Is Nothing) Then
				flag = MyBase.MakeFinalIVTDetermination(potentialGiverOfAccess) = IVTConclusion.Match
			ElseIf (potentialGiverOfAccess.GetInternalsVisibleToPublicKeys(Me.Name).IsEmpty()) Then
				flag = False
			Else
				If (Me._optimisticallyGrantedInternalsAccess Is Nothing) Then
					Interlocked.CompareExchange(Of ConcurrentDictionary(Of AssemblySymbol, Boolean))(Me._optimisticallyGrantedInternalsAccess, New ConcurrentDictionary(Of AssemblySymbol, Boolean)(), Nothing)
				End If
				Me._optimisticallyGrantedInternalsAccess.TryAdd(potentialGiverOfAccess, True)
				flag = True
			End If
			Return flag
		End Function

		Private Function CheckForRuntime() As Boolean
			Dim wellKnownType As NamedTypeSymbol = Me.DeclaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute)
			If (wellKnownType.IsErrorType() OrElse wellKnownType.IsEmbedded) Then
				Return False
			End If
			Return wellKnownType.ContainingAssembly = Me
		End Function

		Private Sub CheckOptimisticIVTAccessGrants(ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As IEnumerator(Of AssemblySymbol) = Nothing
			Dim assemblySymbols As ConcurrentDictionary(Of AssemblySymbol, Boolean) = Me._optimisticallyGrantedInternalsAccess
			If (assemblySymbols IsNot Nothing) Then
				Try
					enumerator = assemblySymbols.Keys.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As AssemblySymbol = enumerator.Current
						Dim vTConclusion As IVTConclusion = MyBase.MakeFinalIVTDetermination(current)
						If (vTConclusion <> IVTConclusion.PublicKeyDoesntMatch) Then
							If (vTConclusion <> IVTConclusion.OneSignedOneNot) Then
								Continue While
							End If
							bag.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_FriendRefSigningMismatch, New [Object]() { current.Identity, Me.Identity }), NoLocation.Singleton, False))
						Else
							bag.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_FriendRefNotEqualToThis, New [Object]() { current.Identity, Me.Identity }), NoLocation.Singleton, False))
						End If
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
		End Sub

		Private Sub ComputeAndSetStrongNameKeys()
			Dim strongNameKey As Microsoft.CodeAnalysis.StrongNameKeys
			Me.EnsureAttributesAreBound()
			If (Me._lazyStrongNameKeys Is Nothing) Then
				Dim cryptoKeyFile As String = Me._compilation.Options.CryptoKeyFile
				If (Me.DeclaringCompilation.Options.PublicSign) Then
					strongNameKey = If([String].IsNullOrEmpty(cryptoKeyFile) OrElse PathUtilities.IsAbsolute(cryptoKeyFile), Microsoft.CodeAnalysis.StrongNameKeys.Create(cryptoKeyFile, MessageProvider.Instance), Microsoft.CodeAnalysis.StrongNameKeys.None)
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.StrongNameKeys)(Me._lazyStrongNameKeys, strongNameKey, Nothing)
					Return
				End If
				If ([String].IsNullOrEmpty(cryptoKeyFile)) Then
					cryptoKeyFile = Me.AssemblyKeyFileAttributeSetting
					If (CObj(cryptoKeyFile) = CObj(WellKnownAttributeData.StringMissingValue)) Then
						cryptoKeyFile = Nothing
					End If
				End If
				Dim cryptoKeyContainer As String = Me._compilation.Options.CryptoKeyContainer
				If ([String].IsNullOrEmpty(cryptoKeyContainer)) Then
					cryptoKeyContainer = Me.AssemblyKeyContainerAttributeSetting
					If (CObj(cryptoKeyContainer) = CObj(WellKnownAttributeData.StringMissingValue)) Then
						cryptoKeyContainer = Nothing
					End If
				End If
				Dim flag As Boolean = Not [String].IsNullOrEmpty(Me.SignatureKey)
				strongNameKey = Microsoft.CodeAnalysis.StrongNameKeys.Create(Me.DeclaringCompilation.Options.StrongNameProvider, cryptoKeyFile, cryptoKeyContainer, flag, MessageProvider.Instance)
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.StrongNameKeys)(Me._lazyStrongNameKeys, strongNameKey, Nothing)
			End If
		End Sub

		Private Function ComputeIdentity() As AssemblyIdentity
			Me.EnsureAttributesAreBound()
			Dim str As String = Me._assemblySimpleName
			Dim version As System.Version = VersionHelper.GenerateVersionFromPatternAndCurrentTime(Me._compilation.Options.CurrentLocalTime, Me.AssemblyVersionAttributeSetting)
			Dim assemblyCultureAttributeSetting As String = Me.AssemblyCultureAttributeSetting
			Dim publicKey As ImmutableArray(Of Byte) = Me.StrongNameKeys.PublicKey
			Dim nums As ImmutableArray(Of Byte) = Me.StrongNameKeys.PublicKey
			Return New AssemblyIdentity(str, version, assemblyCultureAttributeSetting, publicKey, Not nums.IsDefault)
		End Function

		Friend Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim item As TypedConstant
			Dim [uInteger] As AssemblyHashAlgorithm
			Dim [integer] As System.Reflection.AssemblyFlags
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (attribute.IsTargetAttribute(Me, AttributeDescription.CaseInsensitiveExtensionAttribute)) Then
				Me._lazyEmitExtensionAttribute = 1
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.InternalsVisibleToAttribute)) Then
				Me.ProcessOneInternalsVisibleToAttribute(arguments.AttributeSyntaxOpt, attribute, diagnostics)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblySignatureKeyAttribute)) Then
				item = attribute.CommonConstructorArguments(0)
				Dim valueInternal As String = CStr(item.ValueInternal)
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblySignatureKeyAttributeSetting = valueInternal
				If (Not Microsoft.CodeAnalysis.StrongNameKeys.IsValidPublicKeyString(valueInternal)) Then
					diagnostics.Add(ERRID.ERR_InvalidSignaturePublicKey, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
				End If
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyKeyFileAttribute)) Then
				Dim orCreateData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				orCreateData.AssemblyKeyFileAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyKeyNameAttribute)) Then
				Dim commonAssemblyWellKnownAttributeDatum As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				commonAssemblyWellKnownAttributeDatum.AssemblyKeyContainerAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyDelaySignAttribute)) Then
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyDelaySignAttributeSetting = If(CBool(attribute.CommonConstructorArguments(0).ValueInternal), ThreeState.[True], ThreeState.[False])
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyVersionAttribute)) Then
				item = attribute.CommonConstructorArguments(0)
				Dim version As System.Version = Nothing
				If (Not VersionHelper.TryParseAssemblyVersion(CStr(item.ValueInternal), Not Me._compilation.IsEmitDeterministic, version)) Then
					diagnostics.Add(ERRID.ERR_InvalidVersionFormat, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
				End If
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyVersionAttributeSetting = version
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyFileVersionAttribute)) Then
				Dim version1 As System.Version = Nothing
				item = attribute.CommonConstructorArguments(0)
				Dim str As String = CStr(item.ValueInternal)
				If (Not VersionHelper.TryParse(str, version1)) Then
					diagnostics.Add(ERRID.WRN_InvalidVersionFormat, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
				End If
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyFileVersionAttributeSetting = str
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyInformationalVersionAttribute)) Then
				Dim orCreateData1 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				orCreateData1.AssemblyInformationalVersionAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyTitleAttribute)) Then
				Dim valueInternal1 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				valueInternal1.AssemblyTitleAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyDescriptionAttribute)) Then
				Dim commonAssemblyWellKnownAttributeDatum1 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				commonAssemblyWellKnownAttributeDatum1.AssemblyDescriptionAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCultureAttribute)) Then
				item = attribute.CommonConstructorArguments(0)
				Dim str1 As String = CStr(item.ValueInternal)
				If (Not [String].IsNullOrEmpty(str1)) Then
					If (Me.DeclaringCompilation.Options.OutputKind.IsApplication()) Then
						diagnostics.Add(ERRID.ERR_InvalidAssemblyCultureForExe, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
					ElseIf (Not AssemblyIdentity.IsValidCultureName(str1)) Then
						diagnostics.Add(ERRID.ERR_InvalidAssemblyCulture, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
						str1 = Nothing
					End If
				End If
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyCultureAttributeSetting = str1
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCompanyAttribute)) Then
				Dim orCreateData2 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				orCreateData2.AssemblyCompanyAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyProductAttribute)) Then
				Dim valueInternal2 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				valueInternal2.AssemblyProductAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyInformationalVersionAttribute)) Then
				Dim commonAssemblyWellKnownAttributeDatum2 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				commonAssemblyWellKnownAttributeDatum2.AssemblyInformationalVersionAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SatelliteContractVersionAttribute)) Then
				Dim version2 As System.Version = Nothing
				If (Not VersionHelper.TryParseAssemblyVersion(CStr(attribute.CommonConstructorArguments(0).ValueInternal), False, version2)) Then
					diagnostics.Add(ERRID.ERR_InvalidVersionFormat2, SourceAssemblySymbol.GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt))
				End If
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCopyrightAttribute)) Then
				Dim orCreateData3 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				orCreateData3.AssemblyCopyrightAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyTrademarkAttribute)) Then
				Dim valueInternal3 As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))()
				item = attribute.CommonConstructorArguments(0)
				valueInternal3.AssemblyTrademarkAttributeSetting = CStr(item.ValueInternal)
			ElseIf (attribute.IsSecurityAttribute(Me.DeclaringCompilation)) Then
				attribute.DecodeSecurityAttribute(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))(Me, Me.DeclaringCompilation, arguments)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ClassInterfaceAttribute)) Then
				attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, diagnostics)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.TypeLibVersionAttribute)) Then
				Me.ValidateIntegralAttributeNonNegativeArguments(attribute, arguments.AttributeSyntaxOpt, diagnostics)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ComCompatibleVersionAttribute)) Then
				Me.ValidateIntegralAttributeNonNegativeArguments(attribute, arguments.AttributeSyntaxOpt, diagnostics)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.GuidAttribute)) Then
				attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, diagnostics)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.CompilationRelaxationsAttribute)) Then
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().HasCompilationRelaxationsAttribute = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ReferenceAssemblyAttribute)) Then
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().HasReferenceAssemblyAttribute = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.RuntimeCompatibilityAttribute)) Then
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().RuntimeCompatibilityWrapNonExceptionThrows = True
			ElseIf (Not attribute.IsTargetAttribute(Me, AttributeDescription.DebuggableAttribute)) Then
				Dim targetAttributeSignatureIndex As Integer = attribute.GetTargetAttributeSignatureIndex(Me, AttributeDescription.AssemblyAlgorithmIdAttribute)
				If (targetAttributeSignatureIndex = -1) Then
					targetAttributeSignatureIndex = attribute.GetTargetAttributeSignatureIndex(Me, AttributeDescription.AssemblyFlagsAttribute)
					If (targetAttributeSignatureIndex <> -1) Then
						item = attribute.CommonConstructorArguments(0)
						Dim objectValue As Object = RuntimeHelpers.GetObjectValue(item.ValueInternal)
						If (targetAttributeSignatureIndex = 0 OrElse targetAttributeSignatureIndex = 1) Then
							[integer] = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(objectValue)
						Else
							[integer] = Microsoft.VisualBasic.CompilerServices.Conversions.ToUInteger(objectValue)
						End If
						arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyFlagsAttributeSetting = [integer]
					End If
				Else
					item = attribute.CommonConstructorArguments(0)
					Dim obj As Object = RuntimeHelpers.GetObjectValue(item.ValueInternal)
					If (targetAttributeSignatureIndex <> 0) Then
						[uInteger] = Microsoft.VisualBasic.CompilerServices.Conversions.ToUInteger(obj)
					Else
						[uInteger] = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(obj)
					End If
					arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().AssemblyAlgorithmIdAttributeSetting = New Nullable(Of AssemblyHashAlgorithm)([uInteger])
				End If
			Else
				arguments.GetOrCreateData(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))().HasDebuggableAttribute = True
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Private Sub DetectAttributeAndOptionConflicts(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Me.EnsureAttributesAreBound()
			If (Me._compilation.Options.PublicSign AndAlso Me.DelaySignAttributeSetting) Then
				diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, New [Object]() { AttributeDescription.AssemblyDelaySignAttribute.FullName, "PublicSign" })
			End If
			If (Me._compilation.Options.OutputKind = OutputKind.NetModule) Then
				If (Not [String].IsNullOrEmpty(Me._compilation.Options.CryptoKeyContainer)) Then
					Dim assemblyKeyContainerAttributeSetting As String = Me.AssemblyKeyContainerAttributeSetting
					If (CObj(assemblyKeyContainerAttributeSetting) = CObj(WellKnownAttributeData.StringMissingValue)) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, Me._compilation, NoLocation.Singleton, diagnostics)
					ElseIf ([String].Compare(Me._compilation.Options.CryptoKeyContainer, assemblyKeyContainerAttributeSetting, StringComparison.OrdinalIgnoreCase) <> 0) Then
						diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, New [Object]() { AttributeDescription.AssemblyKeyNameAttribute.FullName, "CryptoKeyContainer" })
					End If
				End If
				If (Not [String].IsNullOrEmpty(Me._compilation.Options.CryptoKeyFile)) Then
					Dim assemblyKeyFileAttributeSetting As String = Me.AssemblyKeyFileAttributeSetting
					If (CObj(assemblyKeyFileAttributeSetting) = CObj(WellKnownAttributeData.StringMissingValue)) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, Me._compilation, NoLocation.Singleton, diagnostics)
						Return
					End If
					If ([String].Compare(Me._compilation.Options.CryptoKeyFile, assemblyKeyFileAttributeSetting, StringComparison.OrdinalIgnoreCase) <> 0) Then
						diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, New [Object]() { AttributeDescription.AssemblyKeyFileAttribute.FullName, "CryptoKeyFile" })
						Return
					End If
				End If
			ElseIf (Me._compilation.Options.PublicSign) Then
				If (CObj(Me.AssemblyKeyContainerAttributeSetting) <> CObj(WellKnownAttributeData.StringMissingValue)) Then
					diagnostics.Add(ERRID.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, New [Object]() { AttributeDescription.AssemblyKeyNameAttribute.FullName })
				End If
				If (CObj(Me.AssemblyKeyFileAttributeSetting) <> CObj(WellKnownAttributeData.StringMissingValue)) Then
					diagnostics.Add(ERRID.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, New [Object]() { AttributeDescription.AssemblyKeyFileAttribute.FullName })
				End If
			End If
		End Sub

		Private Sub EnsureAttributesAreBound()
			If (Me._lazySourceAttributesBag Is Nothing OrElse Not Me._lazySourceAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(Me.GetAttributeDeclarations()), Me._lazySourceAttributesBag, AttributeLocation.None)
			End If
		End Sub

		Private Sub EnsureNetModuleAttributesAreBound()
			If (Me._lazyNetModuleAttributesBag Is Nothing) Then
				Me.LoadAndValidateNetModuleAttributes(Me._lazyNetModuleAttributesBag)
			End If
		End Sub

		Friend Sub GetAllDeclarationErrors(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			Dim flag As Boolean = False
			Me.SourceModule.GetAllDeclarationErrors(diagnostics, cancellationToken, flag)
			diagnostics.AddRange(Me.GetAssemblyLevelDeclarationErrors(flag), True)
		End Sub

		Friend Overrides Function GetAllTopLevelForwardedTypes() As IEnumerable(Of NamedTypeSymbol)
			Return PEModuleBuilder.GetForwardedTypes(Me, Nothing)
		End Function

		Private Shared Function GetAssemblyAttributeFirstArgumentLocation(ByVal attributeSyntaxOpt As AttributeSyntax) As Microsoft.CodeAnalysis.Location
			Dim location As Microsoft.CodeAnalysis.Location
			location = If(attributeSyntaxOpt IsNot Nothing, attributeSyntaxOpt.ArgumentList.Arguments.First().GetLocation(), NoLocation.Singleton)
			Return location
		End Function

		Friend Function GetAssemblyCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState, ByVal emittingRefAssembly As Boolean, ByVal emittingAssemblyAttributesInNetModule As Boolean) As IEnumerable(Of VisualBasicAttributeData)
			Dim synthesizedAttributeDatas As ArrayBuilder(Of SynthesizedAttributeData) = Nothing
			Me.AddSynthesizedAttributes(compilationState, synthesizedAttributeDatas)
			If (emittingRefAssembly AndAlso Not Me.HasReferenceAssemblyAttribute) Then
				Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Dim synthesizedAttributeDatum As SynthesizedAttributeData = declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor, typedConstants, keyValuePairs, True)
				Symbol.AddSynthesizedAttribute(synthesizedAttributeDatas, synthesizedAttributeDatum)
			End If
			Return MyBase.GetCustomAttributesToEmit(Me.GetAttributes(), synthesizedAttributeDatas, False, emittingAssemblyAttributesInNetModule)
		End Function

		Private Function GetAssemblyLevelDeclarationErrors(ByVal haveExtensionMethodsInSource As Boolean) As ImmutableBindingDiagnostic(Of AssemblySymbol)
			If (Me._lazyAssemblyLevelDeclarationErrors.IsDefault OrElse Me._lazyAssemblyLevelDeclarationDependencies.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim threeState As Microsoft.CodeAnalysis.ThreeState = Me._lazyEmitExtensionAttribute
				If (threeState = Microsoft.CodeAnalysis.ThreeState.Unknown) Then
					If (haveExtensionMethodsInSource) Then
						Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
						Me._compilation.GetExtensionAttributeConstructor(useSiteInfo)
						threeState = If(useSiteInfo.DiagnosticInfo Is Nothing, Microsoft.CodeAnalysis.ThreeState.[True], Microsoft.CodeAnalysis.ThreeState.[False])
					Else
						threeState = Microsoft.CodeAnalysis.ThreeState.[False]
					End If
				End If
				Me._lazyEmitExtensionAttribute = CByte(threeState)
				If (Me.StrongNameKeys.DiagnosticOpt IsNot Nothing AndAlso Not Me.IsNetModule()) Then
					instance.Add(Me.StrongNameKeys.DiagnosticOpt)
				End If
				Me.ValidateIVTPublicKeys(instance)
				Me.CheckOptimisticIVTAccessGrants(instance)
				Me.DetectAttributeAndOptionConflicts(instance)
				If (Me.IsDelaySigned AndAlso Not Me.Identity.HasPublicKey) Then
					instance.Add(ERRID.WRN_DelaySignButNoKey, NoLocation.Singleton)
				End If
				If (Me.DeclaringCompilation.Options.PublicSign) Then
					If (Me.IsNetModule()) Then
						instance.Add(ERRID.ERR_PublicSignNetModule, NoLocation.Singleton)
					ElseIf (Not Me.Identity.HasPublicKey) Then
						instance.Add(ERRID.ERR_PublicSignNoKey, NoLocation.Singleton)
					End If
				End If
				If (Me.DeclaringCompilation.Options.OutputKind <> OutputKind.NetModule AndAlso Me.DeclaringCompilation.Options.CryptoPublicKey.IsEmpty AndAlso Me.Identity.HasPublicKey AndAlso Not Me.IsDelaySigned AndAlso Not Me.DeclaringCompilation.Options.PublicSign AndAlso Not Me.StrongNameKeys.CanSign) Then
					instance.Add(ERRID.ERR_SignButNoPrivateKey, NoLocation.Singleton, New [Object]() { Me.StrongNameKeys.KeyFilePath })
				End If
				SourceAssemblySymbol.ReportDiagnosticsForSynthesizedAttributes(Me.DeclaringCompilation, instance)
				Me.ReportDiagnosticsForAddedModules(instance)
				Dim readOnlyAndFree As ImmutableBindingDiagnostic(Of AssemblySymbol) = instance.ToReadOnlyAndFree()
				ImmutableInterlocked.InterlockedInitialize(Of AssemblySymbol)(Me._lazyAssemblyLevelDeclarationDependencies, readOnlyAndFree.Dependencies)
				ImmutableInterlocked.InterlockedInitialize(Of Diagnostic)(Me._lazyAssemblyLevelDeclarationErrors, readOnlyAndFree.Diagnostics)
			End If
			Return New ImmutableBindingDiagnostic(Of AssemblySymbol)(Me._lazyAssemblyLevelDeclarationErrors, Me._lazyAssemblyLevelDeclarationDependencies)
		End Function

		Friend Function GetAttributeDeclarations() As ImmutableArray(Of SyntaxList(Of AttributeListSyntax))
			Dim instance As ArrayBuilder(Of SyntaxList(Of AttributeListSyntax)) = ArrayBuilder(Of SyntaxList(Of AttributeListSyntax)).GetInstance()
			Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = Me.DeclaringCompilation.MergedRootDeclaration.Declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As RootSingleNamespaceDeclaration = DirectCast(enumerator.Current, RootSingleNamespaceDeclaration)
				If (Not current.HasAssemblyAttributes) Then
					Continue While
				End If
				Dim attributes As SyntaxList(Of AttributesStatementSyntax) = DirectCast(current.Location.SourceTree.GetRoot(New CancellationToken()), CompilationUnitSyntax).Attributes
				If (Not attributes.Any()) Then
					Continue While
				End If
				Dim enumerator1 As SyntaxList(Of AttributesStatementSyntax).Enumerator = attributes.GetEnumerator()
				While enumerator1.MoveNext()
					instance.Add(enumerator1.Current.AttributeLists)
				End While
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = Me.GetSourceAttributesBag().Attributes
			Dim visualBasicAttributeDatas As ImmutableArray(Of VisualBasicAttributeData) = Me.GetNetModuleAttributesBag().Attributes
			If (attributes.Length <= 0) Then
				attributes = visualBasicAttributeDatas
			ElseIf (visualBasicAttributeDatas.Length > 0) Then
				attributes = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of VisualBasicAttributeData)(attributes, visualBasicAttributeDatas)
			End If
			Return attributes
		End Function

		Friend Overrides Function GetInternalsVisibleToPublicKeys(ByVal simpleName As String) As IEnumerable(Of ImmutableArray(Of Byte))
			Dim immutableArrays As IEnumerable(Of ImmutableArray(Of Byte))
			Dim keys As IEnumerable(Of ImmutableArray(Of Byte))
			Me.EnsureAttributesAreBound()
			If (Me._lazyInternalsVisibleToMap IsNot Nothing) Then
				Dim immutableArrays1 As ConcurrentDictionary(Of ImmutableArray(Of Byte), Tuple(Of Location, String)) = Nothing
				Me._lazyInternalsVisibleToMap.TryGetValue(simpleName, immutableArrays1)
				If (immutableArrays1 IsNot Nothing) Then
					keys = immutableArrays1.Keys
				Else
					keys = SpecializedCollections.EmptyEnumerable(Of ImmutableArray(Of Byte))()
				End If
				immutableArrays = keys
			Else
				immutableArrays = SpecializedCollections.EmptyEnumerable(Of ImmutableArray(Of Byte))()
			End If
			Return immutableArrays
		End Function

		Friend Overrides Function GetLinkedReferencedAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return New ImmutableArray(Of AssemblySymbol)()
		End Function

		Public Overrides Function GetMetadata() As AssemblyMetadata
			Return Nothing
		End Function

		Private Function GetNetModuleAttributes(<Out> ByRef netModuleNames As ImmutableArray(Of String)) As ImmutableArray(Of VisualBasicAttributeData)
			Dim immutableAndFree As ImmutableArray(Of VisualBasicAttributeData)
			Dim instance As ArrayBuilder(Of String) = Nothing
			Dim visualBasicAttributeDatas As ArrayBuilder(Of VisualBasicAttributeData) = Nothing
			Dim length As Integer = Me._modules.Length - 1
			Dim num As Integer = 1
			Do
				Dim item As PEModuleSymbol = DirectCast(Me._modules(num), PEModuleSymbol)
				Dim name As String = item.Name
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = item.GetAssemblyAttributes().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (instance Is Nothing) Then
						instance = ArrayBuilder(Of String).GetInstance()
						visualBasicAttributeDatas = ArrayBuilder(Of VisualBasicAttributeData).GetInstance()
					End If
					instance.Add(name)
					visualBasicAttributeDatas.Add(current)
				End While
				num = num + 1
			Loop While num <= length
			If (instance IsNot Nothing) Then
				netModuleNames = instance.ToImmutableAndFree()
				immutableAndFree = visualBasicAttributeDatas.ToImmutableAndFree()
			Else
				netModuleNames = ImmutableArray(Of String).Empty
				immutableAndFree = ImmutableArray(Of VisualBasicAttributeData).Empty
			End If
			Return immutableAndFree
		End Function

		Private Function GetNetModuleAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetNetModuleAttributesBag().Attributes
		End Function

		Friend Function GetNetModuleAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			Me.EnsureNetModuleAttributesAreBound()
			Return Me._lazyNetModuleAttributesBag
		End Function

		Friend Function GetNetModuleDecodedWellKnownAttributeData() As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)
			Return DirectCast(Me.GetNetModuleAttributesBag().DecodedWellKnownAttributeData, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))
		End Function

		Friend Overrides Function GetNoPiaResolutionAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return Me.SourceModule.GetReferencedAssemblySymbols()
		End Function

		Friend Function GetSecurityAttributes() As IEnumerable(Of SecurityAttribute)
			Dim securityAttributes As IEnumerable(Of SecurityAttribute) = Nothing
			Dim sourceAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.GetSourceAttributesBag()
			Dim decodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = DirectCast(sourceAttributesBag.DecodedWellKnownAttributeData, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))
			If (decodedWellKnownAttributeData IsNot Nothing) Then
				Dim securityInformation As SecurityWellKnownAttributeData = decodedWellKnownAttributeData.SecurityInformation
				If (securityInformation IsNot Nothing) Then
					securityAttributes = securityInformation.GetSecurityAttributes(Of VisualBasicAttributeData)(sourceAttributesBag.Attributes)
				End If
			End If
			Dim securityAttributes1 As IEnumerable(Of SecurityAttribute) = Nothing
			sourceAttributesBag = Me.GetNetModuleAttributesBag()
			decodedWellKnownAttributeData = DirectCast(sourceAttributesBag.DecodedWellKnownAttributeData, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))
			If (decodedWellKnownAttributeData IsNot Nothing) Then
				Dim securityWellKnownAttributeDatum As SecurityWellKnownAttributeData = decodedWellKnownAttributeData.SecurityInformation
				If (securityWellKnownAttributeDatum IsNot Nothing) Then
					securityAttributes1 = securityWellKnownAttributeDatum.GetSecurityAttributes(Of VisualBasicAttributeData)(sourceAttributesBag.Attributes)
				End If
			End If
			Dim securityAttributes2 As IEnumerable(Of SecurityAttribute) = Nothing
			If (securityAttributes Is Nothing) Then
				securityAttributes2 = If(securityAttributes1 Is Nothing, SpecializedCollections.EmptyEnumerable(Of SecurityAttribute)(), securityAttributes1)
			Else
				securityAttributes2 = If(securityAttributes1 Is Nothing, securityAttributes, securityAttributes.Concat(securityAttributes1))
			End If
			Return securityAttributes2
		End Function

		Friend Function GetSourceAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			Me.EnsureAttributesAreBound()
			Return Me._lazySourceAttributesBag
		End Function

		Friend Function GetSourceDecodedWellKnownAttributeData() As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)
			Dim sourceAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazySourceAttributesBag
			If (sourceAttributesBag Is Nothing OrElse Not sourceAttributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				sourceAttributesBag = Me.GetSourceAttributesBag()
			End If
			Return DirectCast(sourceAttributesBag.DecodedWellKnownAttributeData, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol))
		End Function

		Friend Overrides Function GetSpecialTypeMember(ByVal member As SpecialMember) As Symbol
			Dim specialTypeMember As Symbol
			If (Not Me._compilation.IsMemberMissing(member)) Then
				specialTypeMember = MyBase.GetSpecialTypeMember(member)
			Else
				specialTypeMember = Nothing
			End If
			Return specialTypeMember
		End Function

		Private Function GetUniqueSourceAssemblyAttributes(<Out> ByRef attributeIndicesToSkip As HashSet(Of Integer)) As HashSet(Of VisualBasicAttributeData)
			Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = Me.GetSourceAttributesBag().Attributes
			Dim visualBasicAttributeDatas As HashSet(Of VisualBasicAttributeData) = Nothing
			attributeIndicesToSkip = Nothing
			Dim length As Integer = attributes.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As VisualBasicAttributeData = attributes(num)
				If (Not item.HasErrors AndAlso Not SourceAssemblySymbol.AddUniqueAssemblyAttribute(item, visualBasicAttributeDatas)) Then
					If (attributeIndicesToSkip Is Nothing) Then
						attributeIndicesToSkip = New HashSet(Of Integer)()
					End If
					attributeIndicesToSkip.Add(num)
				End If
				num = num + 1
			Loop While num <= length
			Return visualBasicAttributeDatas
		End Function

		Private Function GetWellKnownAttributeDataStringField(ByVal fieldGetter As Func(Of CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol), String), Optional ByVal missingValue As String = Nothing) As String
			Dim str As String = missingValue
			Dim sourceDecodedWellKnownAttributeData As CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol) = Me.GetSourceDecodedWellKnownAttributeData()
			If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
				str = fieldGetter(sourceDecodedWellKnownAttributeData)
			End If
			If (CObj(str) = CObj(missingValue)) Then
				sourceDecodedWellKnownAttributeData = Me.GetNetModuleDecodedWellKnownAttributeData()
				If (sourceDecodedWellKnownAttributeData IsNot Nothing) Then
					str = fieldGetter(sourceDecodedWellKnownAttributeData)
				End If
			End If
			Return str
		End Function

		Friend Function IsIndexOfDuplicateAssemblyAttribute(ByVal index As Integer) As Boolean
			If (Me._lazyDuplicateAttributeIndices Is Nothing) Then
				Return False
			End If
			Return Me._lazyDuplicateAttributeIndices.Contains(index)
		End Function

		Private Function IsKnownAssemblyAttribute(ByVal attribute As VisualBasicAttributeData) As Boolean
			Dim flag As Boolean
			flag = If(attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyTitleAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyDescriptionAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyConfigurationAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCultureAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyVersionAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCompanyAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyProductAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyInformationalVersionAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyCopyrightAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyTrademarkAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyKeyFileAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyKeyNameAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyAlgorithmIdAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyFlagsAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyDelaySignAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblyFileVersionAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.SatelliteContractVersionAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.AssemblySignatureKeyAttribute), True, False)
			Return flag
		End Function

		Private Sub LoadAndValidateNetModuleAttributes(ByRef lazyNetModuleAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData))
			Dim customAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)
			Dim enumerator As IEnumerator(Of NamedTypeSymbol) = Nothing
			Dim nums As HashSet(Of Integer) = Nothing
			If (Me._compilation.Options.OutputKind.IsNetModule()) Then
				Me.GetUniqueSourceAssemblyAttributes(nums)
				Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyNetModuleAttributesBag, CustomAttributesBag(Of VisualBasicAttributeData).Empty, Nothing)
				Return
			End If
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			Dim netModuleAttributes As ImmutableArray(Of VisualBasicAttributeData) = Me.GetNetModuleAttributes(strs)
			Dim commonAssemblyWellKnownAttributeDatum As WellKnownAttributeData = Nothing
			If (Not System.Linq.ImmutableArrayExtensions.Any(Of VisualBasicAttributeData)(netModuleAttributes)) Then
				Me.GetUniqueSourceAssemblyAttributes(nums)
			Else
				commonAssemblyWellKnownAttributeDatum = Me.ValidateAttributeUsageAndDecodeWellKnownNetModuleAttributes(netModuleAttributes, strs, instance, nums)
			End If
			Dim forwardedTypes As HashSet(Of NamedTypeSymbol) = Nothing
			For i As Integer = Me._modules.Length - 1 To 1 Step -1
				Using item As PEModuleSymbol = DirectCast(Me._modules(i), PEModuleSymbol)
					enumerator = item.GetForwardedTypes().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						If (forwardedTypes Is Nothing) Then
							If (commonAssemblyWellKnownAttributeDatum Is Nothing) Then
								commonAssemblyWellKnownAttributeDatum = New CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)()
							End If
							forwardedTypes = DirectCast(commonAssemblyWellKnownAttributeDatum, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)).ForwardedTypes
							If (forwardedTypes Is Nothing) Then
								forwardedTypes = New HashSet(Of NamedTypeSymbol)()
								DirectCast(commonAssemblyWellKnownAttributeDatum, CommonAssemblyWellKnownAttributeData(Of NamedTypeSymbol)).ForwardedTypes = forwardedTypes
							End If
						End If
						If (Not forwardedTypes.Add(current) OrElse Not current.IsErrorType()) Then
							Continue While
						End If
						Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = If(current.GetUseSiteInfo().DiagnosticInfo, DirectCast(current, ErrorTypeSymbol).ErrorInfo)
						If (diagnosticInfo Is Nothing) Then
							Continue While
						End If
						instance.Add(diagnosticInfo, NoLocation.Singleton)
					End While
				End Using
			Next

			If (commonAssemblyWellKnownAttributeDatum IsNot Nothing OrElse System.Linq.ImmutableArrayExtensions.Any(Of VisualBasicAttributeData)(netModuleAttributes)) Then
				customAttributesBag = New CustomAttributesBag(Of VisualBasicAttributeData)()
				customAttributesBag.SetEarlyDecodedWellKnownAttributeData(Nothing)
				customAttributesBag.SetDecodedWellKnownAttributeData(commonAssemblyWellKnownAttributeDatum)
				customAttributesBag.SetAttributes(netModuleAttributes)
			Else
				customAttributesBag = CustomAttributesBag(Of VisualBasicAttributeData).Empty
			End If
			If (nums IsNot Nothing) Then
				Interlocked.CompareExchange(Of HashSet(Of Integer))(Me._lazyDuplicateAttributeIndices, nums, Nothing)
			End If
			Me.SourceModule.AtomicStoreReferenceAndDiagnostics(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyNetModuleAttributesBag, customAttributesBag, instance, Nothing)
			instance.Free()
		End Sub

		Friend Function MightContainNoPiaLocalTypes() As Boolean
			Dim flag As Boolean
			Dim length As Integer = Me._modules.Length - 1
			Dim num As Integer = 1
			While True
				If (num > length) Then
					flag = Me.SourceModule.MightContainNoPiaLocalTypes()
					Exit While
				ElseIf (Not DirectCast(Me._modules(num), PEModuleSymbol).[Module].ContainsNoPiaLocalTypes()) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function ProcessOneInternalsVisibleToAttribute(ByVal nodeOpt As AttributeSyntax, ByVal attrData As VisualBasicAttributeData, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol::ProcessOneInternalsVisibleToAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean ProcessOneInternalsVisibleToAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Private Sub ReportDiagnosticsForAddedModules(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As Dictionary(Of MetadataReference, Integer).Enumerator = New Dictionary(Of MetadataReference, Integer).Enumerator()
			Dim modules As ImmutableArray(Of ModuleSymbol)
			Dim enumerator1 As IEnumerator(Of String) = Nothing
			Try
				enumerator = Me._compilation.GetBoundReferenceManager().ReferencedModuleIndexMap.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of MetadataReference, Integer) = enumerator.Current
					Dim key As PortableExecutableReference = TryCast(current.Key, PortableExecutableReference)
					If (key Is Nothing OrElse key.FilePath Is Nothing) Then
						Continue While
					End If
					Dim fileName As String = FileNameUtilities.GetFileName(key.FilePath, True)
					modules = Me._modules
					Dim name As String = modules(current.Value).Name
					If ([String].Equals(fileName, name, StringComparison.OrdinalIgnoreCase)) Then
						Continue While
					End If
					diagnostics.Add(ERRID.ERR_NetModuleNameMismatch, NoLocation.Singleton, New [Object]() { name, fileName })
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			If (Me._modules.Length > 1 AndAlso Not Me._compilation.Options.OutputKind.IsNetModule()) Then
				Dim machine As System.Reflection.PortableExecutable.Machine = MyBase.Machine
				Dim flag As Boolean = If(machine <> System.Reflection.PortableExecutable.Machine.I386, False, Not MyBase.Bit32Required)
				Dim strs As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
				modules = Me.Modules
				Dim length As Integer = modules.Length - 1
				Dim num As Integer = 1
				Do
					Dim item As ModuleSymbol = Me.Modules(num)
					If (Not strs.Add(item.Name)) Then
						diagnostics.Add(ERRID.ERR_NetModuleNameMustBeUnique, NoLocation.Singleton, New [Object]() { item.Name })
					End If
					If (Not DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol).[Module].IsCOFFOnly) Then
						Dim machine1 As System.Reflection.PortableExecutable.Machine = item.Machine
						If (machine1 <> System.Reflection.PortableExecutable.Machine.I386 OrElse item.Bit32Required) Then
							If (flag) Then
								diagnostics.Add(ERRID.ERR_AgnosticToMachineModule, NoLocation.Singleton, New [Object]() { item })
							ElseIf (machine <> machine1) Then
								diagnostics.Add(ERRID.ERR_ConflictingMachineModule, NoLocation.Singleton, New [Object]() { item })
							End If
						End If
					End If
					num = num + 1
				Loop While num <= length
				modules = Me.Modules
				Dim length1 As Integer = modules.Length - 1
				For i As Integer = 1 To length1
					modules = Me.Modules
					Dim pEModuleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol = DirectCast(modules(i), Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol)
					Try
						Try
							enumerator1 = pEModuleSymbol.[Module].GetReferencedManagedModulesOrThrow().GetEnumerator()
							While enumerator1.MoveNext()
								Dim str As String = enumerator1.Current
								If (Not strs.Add(str)) Then
									Continue While
								End If
								diagnostics.Add(ERRID.ERR_MissingNetModuleReference, NoLocation.Singleton, New [Object]() { str })
							End While
						Finally
							If (enumerator1 IsNot Nothing) Then
								enumerator1.Dispose()
							End If
						End Try
					Catch badImageFormatException As System.BadImageFormatException
						ProjectData.SetProjectError(badImageFormatException)
						diagnostics.Add(ERRID.ERR_UnsupportedModule1, NoLocation.Singleton, New [Object]() { pEModuleSymbol })
						ProjectData.ClearProjectError()
					End Try
				Next

			End If
		End Sub

		Private Shared Sub ReportDiagnosticsForSynthesizedAttributes(ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not compilation.Options.OutputKind.IsNetModule()) Then
				If (Not TypeOf compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) Is MissingMetadataTypeSymbol) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, compilation, NoLocation.Singleton, diagnostics)
				End If
				If (Not TypeOf compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) Is MissingMetadataTypeSymbol) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, compilation, NoLocation.Singleton, diagnostics)
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, compilation, NoLocation.Singleton, diagnostics)
				End If
			End If
		End Sub

		Friend Overrides Sub SetLinkedReferencedAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Sub SetNoPiaResolutionAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function TryLookupForwardedMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal ignoreCase As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (Not Me._compilation.Options.OutputKind.IsNetModule()) Then
				Dim str As String = Nothing
				Dim length As Integer = Me._modules.Length - 1
				While length >= 1
					Dim item As PEModuleSymbol = DirectCast(Me._modules(length), PEModuleSymbol)
					Dim assembliesForForwardedType As ValueTuple(Of AssemblySymbol, AssemblySymbol) = item.GetAssembliesForForwardedType(emittedName, ignoreCase, str)
					If (assembliesForForwardedType.Item1 Is Nothing) Then
						length += -1
					ElseIf (assembliesForForwardedType.Item2 IsNot Nothing) Then
						namedTypeSymbol = MyBase.CreateMultipleForwardingErrorTypeSymbol(emittedName, item, assembliesForForwardedType.Item1, assembliesForForwardedType.Item2)
						Return namedTypeSymbol
					ElseIf (visitedAssemblies Is Nothing OrElse Not visitedAssemblies.Contains(assembliesForForwardedType.Item1)) Then
						Dim empty As [Object] = visitedAssemblies
						If (empty Is Nothing) Then
							empty = ConsList(Of AssemblySymbol).Empty
						End If
						visitedAssemblies = New ConsList(Of AssemblySymbol)(Me, empty)
						If (ignoreCase AndAlso Not [String].Equals(emittedName.FullName, str, StringComparison.Ordinal)) Then
							emittedName = MetadataTypeName.FromFullName(str, emittedName.UseCLSCompliantNameArityEncoding, emittedName.ForcedArity)
						End If
						namedTypeSymbol = assembliesForForwardedType.Item1.LookupTopLevelMetadataTypeWithCycleDetection(emittedName, visitedAssemblies, True)
						Return namedTypeSymbol
					Else
						namedTypeSymbol = MyBase.CreateCycleInTypeForwarderErrorTypeSymbol(emittedName)
						Return namedTypeSymbol
					End If
				End While
			End If
			namedTypeSymbol = Nothing
			Return namedTypeSymbol
		End Function

		Private Function ValidateAttributeUsageAndDecodeWellKnownNetModuleAttributes(ByVal attributesFromNetModules As ImmutableArray(Of VisualBasicAttributeData), ByVal netModuleNames As ImmutableArray(Of String), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef attributeIndicesToSkip As HashSet(Of Integer)) As WellKnownAttributeData
			VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken())
			BinderBuilder.CreateSourceModuleBinder(Me.SourceModule)
			Dim length As Integer = attributesFromNetModules.Length
			Dim num As Integer = Me.GetSourceAttributesBag().Attributes.Length
			Dim uniqueSourceAssemblyAttributes As HashSet(Of VisualBasicAttributeData) = Me.GetUniqueSourceAssemblyAttributes(attributeIndicesToSkip)
			Dim decodeWellKnownAttributeArgument As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation) = New DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation)() With
			{
				.AttributesCount = length,
				.Diagnostics = diagnostics,
				.SymbolPart = AttributeLocation.None
			}
			For i As Integer = length - 1 To 0 Step -1
				Dim item As VisualBasicAttributeData = attributesFromNetModules(i)
				If (item.HasErrors OrElse Not Me.ValidateAttributeUsageForNetModuleAttribute(item, netModuleNames(i), diagnostics, uniqueSourceAssemblyAttributes)) Then
					If (attributeIndicesToSkip Is Nothing) Then
						attributeIndicesToSkip = New HashSet(Of Integer)()
					End If
					attributeIndicesToSkip.Add(i + num)
				Else
					decodeWellKnownAttributeArgument.Attribute = item
					decodeWellKnownAttributeArgument.Index = i
					decodeWellKnownAttributeArgument.AttributeSyntaxOpt = Nothing
					Me.DecodeWellKnownAttribute(decodeWellKnownAttributeArgument)
				End If
			Next

			If (Not decodeWellKnownAttributeArgument.HasDecodedData) Then
				Return Nothing
			End If
			Return decodeWellKnownAttributeArgument.DecodedData
		End Function

		Private Function ValidateAttributeUsageForNetModuleAttribute(ByVal attribute As VisualBasicAttributeData, ByVal netModuleName As String, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef uniqueAttributes As HashSet(Of VisualBasicAttributeData)) As Boolean
			Dim flag As Boolean
			Dim attributeClass As NamedTypeSymbol = attribute.AttributeClass
			If (attributeClass.GetAttributeUsageInfo().AllowMultiple) Then
				flag = SourceAssemblySymbol.AddUniqueAssemblyAttribute(attribute, uniqueAttributes)
			ElseIf (uniqueAttributes Is Nothing OrElse Not uniqueAttributes.Contains(Function(a As VisualBasicAttributeData) TypeSymbol.Equals(a.AttributeClass, attributeClass, TypeCompareKind.ConsiderEverything))) Then
				SourceAssemblySymbol.AddUniqueAssemblyAttribute(attribute, uniqueAttributes)
				flag = True
			Else
				If (Me.IsKnownAssemblyAttribute(attribute)) Then
					If (Not uniqueAttributes.Contains(attribute)) Then
						diagnostics.Add(ERRID.WRN_AssemblyAttributeFromModuleIsOverridden, NoLocation.Singleton, New [Object]() { attribute.AttributeClass, netModuleName })
					End If
				ElseIf (SourceAssemblySymbol.AddUniqueAssemblyAttribute(attribute, uniqueAttributes)) Then
					diagnostics.Add(ERRID.ERR_InvalidMultipleAttributeUsageInNetModule2, NoLocation.Singleton, New [Object]() { attribute.AttributeClass.Name, netModuleName })
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Sub ValidateIntegralAttributeNonNegativeArguments(ByVal attrData As VisualBasicAttributeData, ByVal nodeOpt As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim length As Integer = attrData.CommonConstructorArguments.Length - 1
			For i As Integer = 0 To length
				If (attrData.GetConstructorArgument(Of Integer)(i, SpecialType.System_Int32) < 0) Then
					diagnostics.Add(ERRID.ERR_BadAttribute1, If(nodeOpt IsNot Nothing, nodeOpt.ArgumentList.Arguments(i).GetLocation(), NoLocation.Singleton), New [Object]() { attrData.AttributeClass })
				End If
			Next

		End Sub

		Protected Sub ValidateIVTPublicKeys(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As IEnumerator(Of ConcurrentDictionary(Of ImmutableArray(Of Byte), Tuple(Of Location, String))) = Nothing
			Dim enumerator1 As IEnumerator(Of KeyValuePair(Of ImmutableArray(Of Byte), Tuple(Of Location, String))) = Nothing
			Me.EnsureAttributesAreBound()
			If (Me.Identity.IsStrongName AndAlso Me._lazyInternalsVisibleToMap IsNot Nothing) Then
				Try
					enumerator = Me._lazyInternalsVisibleToMap.Values.GetEnumerator()
					While enumerator.MoveNext()
						Using current As ConcurrentDictionary(Of ImmutableArray(Of Byte), Tuple(Of Location, String)) = enumerator.Current
							enumerator1 = current.GetEnumerator()
							While enumerator1.MoveNext()
								Dim keyValuePair As KeyValuePair(Of ImmutableArray(Of Byte), Tuple(Of Location, String)) = enumerator1.Current
								If (Not keyValuePair.Key.IsDefaultOrEmpty) Then
									Continue While
								End If
								diagnostics.Add(ERRID.ERR_FriendAssemblyStrongNameRequired, keyValuePair.Value.Item1, New [Object]() { keyValuePair.Value.Item2 })
							End While
						End Using
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
		End Sub
	End Class
End Namespace