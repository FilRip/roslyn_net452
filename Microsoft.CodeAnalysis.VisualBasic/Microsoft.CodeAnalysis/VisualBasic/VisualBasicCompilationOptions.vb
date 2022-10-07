Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public NotInheritable Class VisualBasicCompilationOptions
		Inherits CompilationOptions
		Implements IEquatable(Of VisualBasicCompilationOptions)
		Private _globalImports As ImmutableArray(Of GlobalImport)

		Private _rootNamespace As String

		Private _optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict

		Private _optionInfer As Boolean

		Private _optionExplicit As Boolean

		Private _optionCompareText As Boolean

		Private _embedVbCoreRuntime As Boolean

		Private _parseOptions As VisualBasicParseOptions

		Private _suppressEmbeddedDeclarations As Boolean

		Private _ignoreCorLibraryDuplicatedTypes As Boolean

		Public ReadOnly Property EmbedVbCoreRuntime As Boolean
			Get
				Return Me._embedVbCoreRuntime
			End Get
		End Property

		Public ReadOnly Property GlobalImports As ImmutableArray(Of GlobalImport)
			Get
				Return Me._globalImports
			End Get
		End Property

		Friend ReadOnly Property IgnoreCorLibraryDuplicatedTypes As Boolean
			Get
				Return Me._ignoreCorLibraryDuplicatedTypes
			End Get
		End Property

		Public Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Public Overrides Property NullableContextOptions As Microsoft.CodeAnalysis.NullableContextOptions
			Get
				Return Microsoft.CodeAnalysis.NullableContextOptions.Disable
			End Get
			Protected Set(ByVal value As Microsoft.CodeAnalysis.NullableContextOptions)
				Throw New NotImplementedException()
			End Set
		End Property

		Public ReadOnly Property OptionCompareText As Boolean
			Get
				Return Me._optionCompareText
			End Get
		End Property

		Public ReadOnly Property OptionExplicit As Boolean
			Get
				Return Me._optionExplicit
			End Get
		End Property

		Public ReadOnly Property OptionInfer As Boolean
			Get
				Return Me._optionInfer
			End Get
		End Property

		Public ReadOnly Property OptionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
			Get
				Return Me._optionStrict
			End Get
		End Property

		Public ReadOnly Property ParseOptions As VisualBasicParseOptions
			Get
				Return Me._parseOptions
			End Get
		End Property

		Public ReadOnly Property RootNamespace As String
			Get
				Return Me._rootNamespace
			End Get
		End Property

		Friend ReadOnly Property SuppressEmbeddedDeclarations As Boolean
			Get
				Return Me._suppressEmbeddedDeclarations
			End Get
		End Property

		Public Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, Optional ByVal moduleName As String = Nothing, Optional ByVal mainTypeName As String = Nothing, Optional ByVal scriptClassName As String = "Script", Optional ByVal globalImports As IEnumerable(Of GlobalImport) = Nothing, Optional ByVal rootNamespace As String = Nothing, Optional ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict = 0, Optional ByVal optionInfer As Boolean = True, Optional ByVal optionExplicit As Boolean = True, Optional ByVal optionCompareText As Boolean = False, Optional ByVal parseOptions As VisualBasicParseOptions = Nothing, Optional ByVal embedVbCoreRuntime As Boolean = False, Optional ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel = 0, Optional ByVal checkOverflow As Boolean = True, Optional ByVal cryptoKeyContainer As String = Nothing, Optional ByVal cryptoKeyFile As String = Nothing, Optional ByVal cryptoPublicKey As ImmutableArray(Of Byte) = Nothing, Optional ByVal delaySign As Nullable(Of Boolean) = Nothing, Optional ByVal platform As Microsoft.CodeAnalysis.Platform = 0, Optional ByVal generalDiagnosticOption As ReportDiagnostic = 0, Optional ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)) = Nothing, Optional ByVal concurrentBuild As Boolean = True, Optional ByVal deterministic As Boolean = False, Optional ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver = Nothing, Optional ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver = Nothing, Optional ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver = Nothing, Optional ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer = Nothing, Optional ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider = Nothing, Optional ByVal publicSign As Boolean = False, Optional ByVal reportSuppressedDiagnostics As Boolean = False, Optional ByVal metadataImportOptions As Microsoft.CodeAnalysis.MetadataImportOptions = 0)
			MyClass.New(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, DateTime.MinValue, False, False, xmlReferenceResolver, sourceReferenceResolver, Nothing, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, False, False)
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal moduleName As String, ByVal mainTypeName As String, ByVal scriptClassName As String, ByVal globalImports As IEnumerable(Of GlobalImport), ByVal rootNamespace As String, ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict, ByVal optionInfer As Boolean, ByVal optionExplicit As Boolean, ByVal optionCompareText As Boolean, ByVal parseOptions As VisualBasicParseOptions, ByVal embedVbCoreRuntime As Boolean, ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel, ByVal checkOverflow As Boolean, ByVal cryptoKeyContainer As String, ByVal cryptoKeyFile As String, ByVal cryptoPublicKey As ImmutableArray(Of Byte), ByVal delaySign As Nullable(Of Boolean), ByVal platform As Microsoft.CodeAnalysis.Platform, ByVal generalDiagnosticOption As ReportDiagnostic, ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)), ByVal concurrentBuild As Boolean, ByVal deterministic As Boolean, ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver, ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver, ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver, ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer, ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider, ByVal publicSign As Boolean, ByVal reportSuppressedDiagnostics As Boolean)
			MyClass.New(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign, reportSuppressedDiagnostics, Microsoft.CodeAnalysis.MetadataImportOptions.[Public])
		End Sub

		Private Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal reportSuppressedDiagnostics As Boolean, ByVal moduleName As String, ByVal mainTypeName As String, ByVal scriptClassName As String, ByVal globalImports As IEnumerable(Of GlobalImport), ByVal rootNamespace As String, ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict, ByVal optionInfer As Boolean, ByVal optionExplicit As Boolean, ByVal optionCompareText As Boolean, ByVal parseOptions As VisualBasicParseOptions, ByVal embedVbCoreRuntime As Boolean, ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel, ByVal checkOverflow As Boolean, ByVal cryptoKeyContainer As String, ByVal cryptoKeyFile As String, ByVal cryptoPublicKey As ImmutableArray(Of Byte), ByVal delaySign As Nullable(Of Boolean), ByVal publicSign As Boolean, ByVal platform As Microsoft.CodeAnalysis.Platform, ByVal generalDiagnosticOption As ReportDiagnostic, ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)), ByVal concurrentBuild As Boolean, ByVal deterministic As Boolean, ByVal currentLocalTime As DateTime, ByVal suppressEmbeddedDeclarations As Boolean, ByVal debugPlusMode As Boolean, ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver, ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver, ByVal syntaxTreeOptionsProvider As Microsoft.CodeAnalysis.SyntaxTreeOptionsProvider, ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver, ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer, ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider, ByVal metadataImportOptions As Microsoft.CodeAnalysis.MetadataImportOptions, ByVal referencesSupersedeLowerVersions As Boolean, ByVal ignoreCorLibraryDuplicatedTypes As Boolean)
			MyBase.New(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign, optimizationLevel, checkOverflow, platform, generalDiagnosticOption, 1, specificDiagnosticOptions.ToImmutableDictionaryOrEmpty(CaseInsensitiveComparison.Comparer), concurrentBuild, deterministic, currentLocalTime, debugPlusMode, xmlReferenceResolver, sourceReferenceResolver, syntaxTreeOptionsProvider, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions)
			Me._globalImports = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrEmpty(Of GlobalImport)(globalImports)
			Me._rootNamespace = If(rootNamespace, [String].Empty)
			Me._optionStrict = optionStrict
			Me._optionInfer = optionInfer
			Me._optionExplicit = optionExplicit
			Me._optionCompareText = optionCompareText
			Me._embedVbCoreRuntime = embedVbCoreRuntime
			Me._suppressEmbeddedDeclarations = suppressEmbeddedDeclarations
			Me._parseOptions = parseOptions
			Me._ignoreCorLibraryDuplicatedTypes = ignoreCorLibraryDuplicatedTypes
		End Sub

		Friend Sub New(ByVal other As VisualBasicCompilationOptions)
			MyClass.New(other.OutputKind, other.ReportSuppressedDiagnostics, other.ModuleName, other.MainTypeName, other.ScriptClassName, DirectCast(other.GlobalImports, IEnumerable(Of GlobalImport)), other.RootNamespace, other.OptionStrict, other.OptionInfer, other.OptionExplicit, other.OptionCompareText, other.ParseOptions, other.EmbedVbCoreRuntime, other.OptimizationLevel, other.CheckOverflow, other.CryptoKeyContainer, other.CryptoKeyFile, other.CryptoPublicKey, other.DelaySign, other.PublicSign, other.Platform, other.GeneralDiagnosticOption, other.SpecificDiagnosticOptions, other.ConcurrentBuild, other.Deterministic, other.CurrentLocalTime, other.SuppressEmbeddedDeclarations, other.DebugPlusMode, other.XmlReferenceResolver, other.SourceReferenceResolver, other.SyntaxTreeOptionsProvider, other.MetadataReferenceResolver, other.AssemblyIdentityComparer, other.StrongNameProvider, other.MetadataImportOptions, other.ReferencesSupersedeLowerVersions, other.IgnoreCorLibraryDuplicatedTypes)
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal moduleName As String, ByVal mainTypeName As String, ByVal scriptClassName As String, ByVal globalImports As IEnumerable(Of GlobalImport), ByVal rootNamespace As String, ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict, ByVal optionInfer As Boolean, ByVal optionExplicit As Boolean, ByVal optionCompareText As Boolean, ByVal parseOptions As VisualBasicParseOptions, ByVal embedVbCoreRuntime As Boolean, ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel, ByVal checkOverflow As Boolean, ByVal cryptoKeyContainer As String, ByVal cryptoKeyFile As String, ByVal cryptoPublicKey As ImmutableArray(Of Byte), ByVal delaySign As Nullable(Of Boolean), ByVal platform As Microsoft.CodeAnalysis.Platform, ByVal generalDiagnosticOption As ReportDiagnostic, ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)), ByVal concurrentBuild As Boolean, ByVal deterministic As Boolean, ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver, ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver, ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver, ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer, ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider)
			MyClass.New(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, False, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, False, False, Microsoft.CodeAnalysis.MetadataImportOptions.[Public])
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal moduleName As String, ByVal mainTypeName As String, ByVal scriptClassName As String, ByVal globalImports As IEnumerable(Of GlobalImport), ByVal rootNamespace As String, ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict, ByVal optionInfer As Boolean, ByVal optionExplicit As Boolean, ByVal optionCompareText As Boolean, ByVal parseOptions As VisualBasicParseOptions, ByVal embedVbCoreRuntime As Boolean, ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel, ByVal checkOverflow As Boolean, ByVal cryptoKeyContainer As String, ByVal cryptoKeyFile As String, ByVal cryptoPublicKey As ImmutableArray(Of Byte), ByVal delaySign As Nullable(Of Boolean), ByVal platform As Microsoft.CodeAnalysis.Platform, ByVal generalDiagnosticOption As ReportDiagnostic, ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)), ByVal concurrentBuild As Boolean, ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver, ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver, ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver, ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer, ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider)
			MyClass.New(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, False, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider)
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Sub New(ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal reportSuppressedDiagnostics As Boolean, Optional ByVal moduleName As String = Nothing, Optional ByVal mainTypeName As String = Nothing, Optional ByVal scriptClassName As String = "Script", Optional ByVal globalImports As IEnumerable(Of GlobalImport) = Nothing, Optional ByVal rootNamespace As String = Nothing, Optional ByVal optionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict = 0, Optional ByVal optionInfer As Boolean = True, Optional ByVal optionExplicit As Boolean = True, Optional ByVal optionCompareText As Boolean = False, Optional ByVal parseOptions As VisualBasicParseOptions = Nothing, Optional ByVal embedVbCoreRuntime As Boolean = False, Optional ByVal optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel = 0, Optional ByVal checkOverflow As Boolean = True, Optional ByVal cryptoKeyContainer As String = Nothing, Optional ByVal cryptoKeyFile As String = Nothing, Optional ByVal cryptoPublicKey As ImmutableArray(Of Byte) = Nothing, Optional ByVal delaySign As Nullable(Of Boolean) = Nothing, Optional ByVal platform As Microsoft.CodeAnalysis.Platform = 0, Optional ByVal generalDiagnosticOption As ReportDiagnostic = 0, Optional ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)) = Nothing, Optional ByVal concurrentBuild As Boolean = True, Optional ByVal deterministic As Boolean = False, Optional ByVal xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver = Nothing, Optional ByVal sourceReferenceResolver As Microsoft.CodeAnalysis.SourceReferenceResolver = Nothing, Optional ByVal metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver = Nothing, Optional ByVal assemblyIdentityComparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer = Nothing, Optional ByVal strongNameProvider As Microsoft.CodeAnalysis.StrongNameProvider = Nothing)
			MyClass.New(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, False, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, DateTime.MinValue, False, False, xmlReferenceResolver, sourceReferenceResolver, Nothing, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, Microsoft.CodeAnalysis.MetadataImportOptions.[Public], False, False)
		End Sub

		Protected Overrides Function CommonWithAssemblyIdentityComparer(ByVal comparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer) As CompilationOptions
			Return Me.WithAssemblyIdentityComparer(comparer)
		End Function

		Protected Overrides Function CommonWithCheckOverflow(ByVal checkOverflow As Boolean) As CompilationOptions
			Return Me.WithOverflowChecks(checkOverflow)
		End Function

		Protected Overrides Function CommonWithConcurrentBuild(ByVal concurrent As Boolean) As CompilationOptions
			Return Me.WithConcurrentBuild(concurrent)
		End Function

		Protected Overrides Function CommonWithCryptoKeyContainer(ByVal cryptoKeyContainer As String) As CompilationOptions
			Return Me.WithCryptoKeyContainer(cryptoKeyContainer)
		End Function

		Protected Overrides Function CommonWithCryptoKeyFile(ByVal cryptoKeyFile As String) As CompilationOptions
			Return Me.WithCryptoKeyFile(cryptoKeyFile)
		End Function

		Protected Overrides Function CommonWithCryptoPublicKey(ByVal cryptoPublicKey As ImmutableArray(Of Byte)) As CompilationOptions
			Return Me.WithCryptoPublicKey(cryptoPublicKey)
		End Function

		Protected Overrides Function CommonWithDelaySign(ByVal delaySign As Nullable(Of Boolean)) As CompilationOptions
			Return Me.WithDelaySign(delaySign)
		End Function

		Protected Overrides Function CommonWithDeterministic(ByVal deterministic As Boolean) As CompilationOptions
			Return Me.WithDeterministic(deterministic)
		End Function

		<Obsolete>
		Protected Overrides Function CommonWithFeatures(ByVal features As ImmutableArray(Of String)) As CompilationOptions
			Throw New NotImplementedException()
		End Function

		Protected Overrides Function CommonWithGeneralDiagnosticOption(ByVal value As ReportDiagnostic) As CompilationOptions
			Return Me.WithGeneralDiagnosticOption(value)
		End Function

		Protected Overrides Function CommonWithMainTypeName(ByVal mainTypeName As String) As CompilationOptions
			Return Me.WithMainTypeName(mainTypeName)
		End Function

		Protected Overrides Function CommonWithMetadataImportOptions(ByVal value As Microsoft.CodeAnalysis.MetadataImportOptions) As CompilationOptions
			Return Me.WithMetadataImportOptions(value)
		End Function

		Protected Overrides Function CommonWithMetadataReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.MetadataReferenceResolver) As CompilationOptions
			Return Me.WithMetadataReferenceResolver(resolver)
		End Function

		Protected Overrides Function CommonWithModuleName(ByVal moduleName As String) As CompilationOptions
			Return Me.WithModuleName(moduleName)
		End Function

		Protected Overrides Function CommonWithOptimizationLevel(ByVal value As Microsoft.CodeAnalysis.OptimizationLevel) As CompilationOptions
			Return Me.WithOptimizationLevel(value)
		End Function

		Protected Overrides Function CommonWithOutputKind(ByVal kind As Microsoft.CodeAnalysis.OutputKind) As CompilationOptions
			Return Me.WithOutputKind(kind)
		End Function

		Protected Overrides Function CommonWithPlatform(ByVal platform As Microsoft.CodeAnalysis.Platform) As CompilationOptions
			Return Me.WithPlatform(platform)
		End Function

		Protected Overrides Function CommonWithPublicSign(ByVal publicSign As Boolean) As CompilationOptions
			Return Me.WithPublicSign(publicSign)
		End Function

		Protected Overrides Function CommonWithReportSuppressedDiagnostics(ByVal reportSuppressedDiagnostics As Boolean) As CompilationOptions
			Return Me.WithReportSuppressedDiagnostics(reportSuppressedDiagnostics)
		End Function

		Protected Overrides Function CommonWithScriptClassName(ByVal scriptClassName As String) As CompilationOptions
			Return Me.WithScriptClassName(scriptClassName)
		End Function

		Protected Overrides Function CommonWithSourceReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.SourceReferenceResolver) As CompilationOptions
			Return Me.WithSourceReferenceResolver(resolver)
		End Function

		Protected Overrides Function CommonWithSpecificDiagnosticOptions(ByVal specificDiagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)) As CompilationOptions
			Return Me.WithSpecificDiagnosticOptions(specificDiagnosticOptions)
		End Function

		Protected Overrides Function CommonWithSpecificDiagnosticOptions(ByVal specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic))) As CompilationOptions
			Return Me.WithSpecificDiagnosticOptions(specificDiagnosticOptions)
		End Function

		Protected Overrides Function CommonWithStrongNameProvider(ByVal provider As Microsoft.CodeAnalysis.StrongNameProvider) As CompilationOptions
			Return Me.WithStrongNameProvider(provider)
		End Function

		Protected Overrides Function CommonWithSyntaxTreeOptionsProvider(ByVal provider As Microsoft.CodeAnalysis.SyntaxTreeOptionsProvider) As CompilationOptions
			Return Me.WithSyntaxTreeOptionsProvider(provider)
		End Function

		Protected Overrides Function CommonWithXmlReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.XmlReferenceResolver) As CompilationOptions
			Return Me.WithXmlReferenceResolver(resolver)
		End Function

		Public Function ExplicitEquals(ByVal other As VisualBasicCompilationOptions) As Boolean Implements IEquatable(Of VisualBasicCompilationOptions).Equals
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (CObj(Me) = CObj(other)) Then
				flag = True
			ElseIf (MyBase.EqualsHelper(other)) Then
				If (If(Me.GlobalImports.IsDefault, Not other.GlobalImports.IsDefault, Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of GlobalImport, GlobalImport)(Me.GlobalImports, other.GlobalImports, DirectCast(Nothing, IEqualityComparer(Of GlobalImport)))) OrElse Not [String].Equals(Me.RootNamespace, other.RootNamespace, StringComparison.Ordinal) OrElse Me.OptionStrict <> other.OptionStrict OrElse Me.OptionInfer <> other.OptionInfer OrElse Me.OptionExplicit <> other.OptionExplicit OrElse Me.OptionCompareText <> other.OptionCompareText OrElse Me.EmbedVbCoreRuntime <> other.EmbedVbCoreRuntime OrElse Me.SuppressEmbeddedDeclarations <> other.SuppressEmbeddedDeclarations OrElse Me.IgnoreCorLibraryDuplicatedTypes <> other.IgnoreCorLibraryDuplicatedTypes) Then
					flag1 = False
				Else
					flag1 = If(Me.ParseOptions Is Nothing, other.ParseOptions Is Nothing, Me.ParseOptions.ExplicitEquals(other.ParseOptions))
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of VisualBasicCompilationOptions).Equals
			Return Me.ExplicitEquals(TryCast(obj, VisualBasicCompilationOptions))
		End Function

		Friend Overrides Function FilterDiagnostic(ByVal diagnostic As Microsoft.CodeAnalysis.Diagnostic, ByVal cancellationToken As System.Threading.CancellationToken) As Microsoft.CodeAnalysis.Diagnostic
			Return VisualBasicDiagnosticFilter.Filter(diagnostic, MyBase.GeneralDiagnosticOption, MyBase.SpecificDiagnosticOptions, MyBase.SyntaxTreeOptionsProvider, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(MyBase.GetHashCodeHelper(), Hash.Combine(Hash.CombineValues(Of GlobalImport)(Me.GlobalImports, 2147483647), Hash.Combine(If(Me.RootNamespace IsNot Nothing, StringComparer.Ordinal.GetHashCode(Me.RootNamespace), 0), Hash.Combine(CInt(Me.OptionStrict), Hash.Combine(Me.OptionInfer, Hash.Combine(Me.OptionExplicit, Hash.Combine(Me.OptionCompareText, Hash.Combine(Me.EmbedVbCoreRuntime, Hash.Combine(Me.SuppressEmbeddedDeclarations, Hash.Combine(Me.IgnoreCorLibraryDuplicatedTypes, Hash.Combine(Of VisualBasicParseOptions)(Me.ParseOptions, 0)))))))))))
		End Function

		Friend Overrides Function GetImports() As ImmutableArray(Of String)
			Dim instance As ArrayBuilder(Of String) = ArrayBuilder(Of String).GetInstance(Me.GlobalImports.Length)
			Dim enumerator As ImmutableArray(Of GlobalImport).Enumerator = Me.GlobalImports.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As GlobalImport = enumerator.Current
				If (current.IsXmlClause) Then
					Continue While
				End If
				instance.Add(current.Name)
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Friend Function GetRootNamespaceParts() As ImmutableArray(Of String)
			Dim empty As ImmutableArray(Of String)
			If ([String].IsNullOrEmpty(Me._rootNamespace) OrElse Not OptionsValidator.IsValidNamespaceName(Me._rootNamespace)) Then
				empty = ImmutableArray(Of String).Empty
			Else
				empty = MetadataHelpers.SplitQualifiedName(Me._rootNamespace)
			End If
			Return empty
		End Function

		Friend Overrides Sub ValidateOptions(ByVal builder As ArrayBuilder(Of Diagnostic))
			Dim platform As Microsoft.CodeAnalysis.Platform
			MyBase.ValidateOptions(builder, Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance)
			If (Me.ParseOptions IsNot Nothing) Then
				builder.AddRange(Me.ParseOptions.Errors)
			End If
			If (Me.EmbedVbCoreRuntime AndAlso MyBase.OutputKind.IsNetModule()) Then
				builder.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 2042))
			End If
			If (Not MyBase.Platform.IsValid()) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim str() As [Object] = { "Platform", Nothing }
				platform = MyBase.Platform
				str(1) = platform.ToString()
				builder.Add(Diagnostic.Create(instance, 2014, str))
			End If
			If (MyBase.ModuleName IsNot Nothing) Then
				MetadataHelpers.CheckAssemblyOrModuleName(MyBase.ModuleName, Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 37206, builder)
			End If
			If (Not MyBase.OutputKind.IsValid()) Then
				Dim messageProvider As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim objArray() As [Object] = { "OutputKind", Nothing }
				objArray(1) = MyBase.OutputKind.ToString()
				builder.Add(Diagnostic.Create(messageProvider, 2014, objArray))
			End If
			If (Not MyBase.OptimizationLevel.IsValid()) Then
				Dim instance1 As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim str1() As [Object] = { "OptimizationLevel", Nothing }
				str1(1) = MyBase.OptimizationLevel.ToString()
				builder.Add(Diagnostic.Create(instance1, 2014, str1))
			End If
			If (MyBase.ScriptClassName Is Nothing OrElse Not StringExtensions.IsValidClrTypeName(MyBase.ScriptClassName)) Then
				Dim diagnostics As ArrayBuilder(Of Diagnostic) = builder
				Dim messageProvider1 As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim scriptClassName() As [Object] = { "ScriptClassName", Nothing }
				scriptClassName(1) = If(MyBase.ScriptClassName, "Nothing")
				diagnostics.Add(Diagnostic.Create(messageProvider1, 2014, scriptClassName))
			End If
			If (MyBase.MainTypeName IsNot Nothing AndAlso Not StringExtensions.IsValidClrTypeName(MyBase.MainTypeName)) Then
				builder.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 2014, New [Object]() { "MainTypeName", MyBase.MainTypeName }))
			End If
			If (Not [String].IsNullOrEmpty(Me.RootNamespace) AndAlso Not OptionsValidator.IsValidNamespaceName(Me.RootNamespace)) Then
				builder.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 2014, New [Object]() { "RootNamespace", Me.RootNamespace }))
			End If
			If (Not Me.OptionStrict.IsValid()) Then
				Dim instance2 As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim objArray1() As [Object] = { "OptionStrict", Nothing }
				objArray1(1) = Me.OptionStrict.ToString()
				builder.Add(Diagnostic.Create(instance2, 2014, objArray1))
			End If
			If (MyBase.Platform = Microsoft.CodeAnalysis.Platform.AnyCpu32BitPreferred AndAlso MyBase.OutputKind.IsValid() AndAlso MyBase.OutputKind <> Microsoft.CodeAnalysis.OutputKind.ConsoleApplication AndAlso MyBase.OutputKind <> Microsoft.CodeAnalysis.OutputKind.WindowsApplication AndAlso MyBase.OutputKind <> Microsoft.CodeAnalysis.OutputKind.WindowsRuntimeApplication) Then
				Dim messageProvider2 As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim str2() As [Object] = { "Platform", Nothing }
				platform = MyBase.Platform
				str2(1) = platform.ToString()
				builder.Add(Diagnostic.Create(messageProvider2, 31392, str2))
			End If
			If (Not MyBase.MetadataImportOptions.IsValid()) Then
				Dim instance3 As Microsoft.CodeAnalysis.VisualBasic.MessageProvider = Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
				Dim objArray2() As [Object] = { "MetadataImportOptions", Nothing }
				objArray2(1) = MyBase.MetadataImportOptions.ToString()
				builder.Add(Diagnostic.Create(instance3, 2014, objArray2))
			End If
		End Sub

		Public Shadows Function WithAssemblyIdentityComparer(ByVal comparer As Microsoft.CodeAnalysis.AssemblyIdentityComparer) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			comparer = If(comparer, Microsoft.CodeAnalysis.AssemblyIdentityComparer.[Default])
			visualBasicCompilationOption = If(comparer <> MyBase.AssemblyIdentityComparer, New VisualBasicCompilationOptions(Me) With
			{
				.AssemblyIdentityComparer = comparer
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithConcurrentBuild(ByVal concurrentBuild As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(concurrentBuild <> MyBase.ConcurrentBuild, New VisualBasicCompilationOptions(Me) With
			{
				.ConcurrentBuild = concurrentBuild
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithCryptoKeyContainer(ByVal name As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not [String].Equals(name, MyBase.CryptoKeyContainer, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				.CryptoKeyContainer = name
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithCryptoKeyFile(ByVal path As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			If ([String].IsNullOrEmpty(path)) Then
				path = Nothing
			End If
			visualBasicCompilationOption = If(Not [String].Equals(path, MyBase.CryptoKeyFile, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				.CryptoKeyFile = path
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithCryptoPublicKey(ByVal value As ImmutableArray(Of Byte)) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			If (value.IsDefault) Then
				value = ImmutableArray(Of Byte).Empty
			End If
			visualBasicCompilationOption = If(value <> MyBase.CryptoPublicKey, New VisualBasicCompilationOptions(Me) With
			{
				.CryptoPublicKey = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Friend Function WithCurrentLocalTime(ByVal value As DateTime) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not value.Equals(MyBase.CurrentLocalTime), New VisualBasicCompilationOptions(Me) With
			{
				.CurrentLocalTime = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Friend Function WithDebugPlusMode(ByVal debugPlusMode As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(debugPlusMode <> MyBase.DebugPlusMode, New VisualBasicCompilationOptions(Me) With
			{
				.DebugPlusMode = debugPlusMode
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithDelaySign(ByVal value As Nullable(Of Boolean)) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			Dim nullable As Nullable(Of Boolean)
			Dim nullable1 As Nullable(Of Boolean) = value
			Dim delaySign As Nullable(Of Boolean) = MyBase.DelaySign
			If (nullable1.HasValue And delaySign.HasValue) Then
				nullable = New Nullable(Of Boolean)(nullable1.GetValueOrDefault() = delaySign.GetValueOrDefault())
			Else
				nullable = Nothing
			End If
			delaySign = nullable
			visualBasicCompilationOption = If(Not delaySign.GetValueOrDefault(), New VisualBasicCompilationOptions(Me) With
			{
				.DelaySign = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithDeterministic(ByVal deterministic As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(deterministic <> MyBase.Deterministic, New VisualBasicCompilationOptions(Me) With
			{
				.Deterministic = deterministic
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithEmbedVbCoreRuntime(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> Me.EmbedVbCoreRuntime, New VisualBasicCompilationOptions(Me) With
			{
				._embedVbCoreRuntime = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithGeneralDiagnosticOption(ByVal value As ReportDiagnostic) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.GeneralDiagnosticOption, New VisualBasicCompilationOptions(Me) With
			{
				.GeneralDiagnosticOption = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithGlobalImports(ByVal globalImports As ImmutableArray(Of GlobalImport)) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not Me.GlobalImports.Equals(globalImports), New VisualBasicCompilationOptions(Me) With
			{
				._globalImports = globalImports
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithGlobalImports(ByVal globalImports As IEnumerable(Of GlobalImport)) As VisualBasicCompilationOptions
			Return New VisualBasicCompilationOptions(Me) With
			{
				._globalImports = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrEmpty(Of GlobalImport)(globalImports)
			}
		End Function

		Public Function WithGlobalImports(ByVal ParamArray globalImports As GlobalImport()) As VisualBasicCompilationOptions
			Return Me.WithGlobalImports(DirectCast(globalImports, IEnumerable(Of GlobalImport)))
		End Function

		Friend Function WithIgnoreCorLibraryDuplicatedTypes(ByVal ignoreCorLibraryDuplicatedTypes As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(ignoreCorLibraryDuplicatedTypes <> Me._ignoreCorLibraryDuplicatedTypes, New VisualBasicCompilationOptions(Me) With
			{
				._ignoreCorLibraryDuplicatedTypes = ignoreCorLibraryDuplicatedTypes
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithMainTypeName(ByVal name As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not [String].Equals(name, MyBase.MainTypeName, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				.MainTypeName = name
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithMetadataImportOptions(ByVal value As Microsoft.CodeAnalysis.MetadataImportOptions) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.MetadataImportOptions, New VisualBasicCompilationOptions(Me) With
			{
				.MetadataImportOptions = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithMetadataReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.MetadataReferenceResolver) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(resolver <> MyBase.MetadataReferenceResolver, New VisualBasicCompilationOptions(Me) With
			{
				.MetadataReferenceResolver = resolver
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithModuleName(ByVal moduleName As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not [String].Equals(moduleName, MyBase.ModuleName, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				.ModuleName = moduleName
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithOptimizationLevel(ByVal value As Microsoft.CodeAnalysis.OptimizationLevel) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.OptimizationLevel, New VisualBasicCompilationOptions(Me) With
			{
				.OptimizationLevel = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithOptionCompareText(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> Me.OptionCompareText, New VisualBasicCompilationOptions(Me) With
			{
				._optionCompareText = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithOptionExplicit(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> Me.OptionExplicit, New VisualBasicCompilationOptions(Me) With
			{
				._optionExplicit = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithOptionInfer(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> Me.OptionInfer, New VisualBasicCompilationOptions(Me) With
			{
				._optionInfer = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithOptionStrict(ByVal value As Microsoft.CodeAnalysis.VisualBasic.OptionStrict) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> Me.OptionStrict, New VisualBasicCompilationOptions(Me) With
			{
				._optionStrict = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithOutputKind(ByVal kind As Microsoft.CodeAnalysis.OutputKind) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(kind <> MyBase.OutputKind, New VisualBasicCompilationOptions(Me) With
			{
				.OutputKind = kind
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithOverflowChecks(ByVal enabled As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(enabled <> MyBase.CheckOverflow, New VisualBasicCompilationOptions(Me) With
			{
				.CheckOverflow = enabled
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithParseOptions(ByVal options As VisualBasicParseOptions) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(CObj(options) <> CObj(Me.ParseOptions), New VisualBasicCompilationOptions(Me) With
			{
				._parseOptions = options
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithPlatform(ByVal value As Microsoft.CodeAnalysis.Platform) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.Platform, New VisualBasicCompilationOptions(Me) With
			{
				.Platform = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithPublicSign(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.PublicSign, New VisualBasicCompilationOptions(Me) With
			{
				.PublicSign = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Friend Function WithReferencesSupersedeLowerVersions(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.ReferencesSupersedeLowerVersions, New VisualBasicCompilationOptions(Me) With
			{
				.ReferencesSupersedeLowerVersions = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithReportSuppressedDiagnostics(ByVal value As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(value <> MyBase.ReportSuppressedDiagnostics, New VisualBasicCompilationOptions(Me) With
			{
				.ReportSuppressedDiagnostics = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Function WithRootNamespace(ByVal rootNamespace As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not [String].Equals(rootNamespace, Me.RootNamespace, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				._rootNamespace = rootNamespace
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithScriptClassName(ByVal name As String) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(Not [String].Equals(name, MyBase.ScriptClassName, StringComparison.Ordinal), New VisualBasicCompilationOptions(Me) With
			{
				.ScriptClassName = name
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithSourceReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.SourceReferenceResolver) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(resolver <> MyBase.SourceReferenceResolver, New VisualBasicCompilationOptions(Me) With
			{
				.SourceReferenceResolver = resolver
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithSpecificDiagnosticOptions(ByVal value As ImmutableDictionary(Of String, ReportDiagnostic)) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			If (value Is Nothing) Then
				value = ImmutableDictionary(Of String, ReportDiagnostic).Empty
			End If
			visualBasicCompilationOption = If(value <> MyBase.SpecificDiagnosticOptions, New VisualBasicCompilationOptions(Me) With
			{
				.SpecificDiagnosticOptions = value
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithSpecificDiagnosticOptions(ByVal value As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic))) As VisualBasicCompilationOptions
			Return New VisualBasicCompilationOptions(Me) With
			{
				.SpecificDiagnosticOptions = value.ToImmutableDictionaryOrEmpty()
			}
		End Function

		Public Shadows Function WithStrongNameProvider(ByVal provider As Microsoft.CodeAnalysis.StrongNameProvider) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(provider <> MyBase.StrongNameProvider, New VisualBasicCompilationOptions(Me) With
			{
				.StrongNameProvider = provider
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Friend Function WithSuppressEmbeddedDeclarations(ByVal suppressEmbeddedDeclarations As Boolean) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(suppressEmbeddedDeclarations <> Me._suppressEmbeddedDeclarations, New VisualBasicCompilationOptions(Me) With
			{
				._suppressEmbeddedDeclarations = suppressEmbeddedDeclarations
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithSyntaxTreeOptionsProvider(ByVal provider As Microsoft.CodeAnalysis.SyntaxTreeOptionsProvider) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(provider <> MyBase.SyntaxTreeOptionsProvider, New VisualBasicCompilationOptions(Me) With
			{
				.SyntaxTreeOptionsProvider = provider
			}, Me)
			Return visualBasicCompilationOption
		End Function

		Public Shadows Function WithXmlReferenceResolver(ByVal resolver As Microsoft.CodeAnalysis.XmlReferenceResolver) As VisualBasicCompilationOptions
			Dim visualBasicCompilationOption As VisualBasicCompilationOptions
			visualBasicCompilationOption = If(resolver <> MyBase.XmlReferenceResolver, New VisualBasicCompilationOptions(Me) With
			{
				.XmlReferenceResolver = resolver
			}, Me)
			Return visualBasicCompilationOption
		End Function
	End Class
End Namespace