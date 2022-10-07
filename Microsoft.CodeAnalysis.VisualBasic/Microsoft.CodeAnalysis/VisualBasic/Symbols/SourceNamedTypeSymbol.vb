Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceNamedTypeSymbol
		Inherits SourceMemberContainerTypeSymbol
		Implements IAttributeTargetSymbol
		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Protected m_lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private ReadOnly _corTypeId As Microsoft.CodeAnalysis.SpecialType

		Private _lazyDocComment As String

		Private _lazyExpandedDocComment As String

		Private _lazyEnumUnderlyingType As NamedTypeSymbol

		Private _lazyWithEventsOverrides As ConcurrentDictionary(Of PropertySymbol, SynthesizedOverridingWithEventsProperty)

		Private _withEventsOverridesAreFrozen As Boolean

		Friend Const DelegateConstructorMethodFlags As SourceMemberFlags = SourceMemberFlags.[Static]

		Friend Const DelegateCommonMethodFlags As SourceMemberFlags = SourceMemberFlags.[Overridable]

		Private _lazyLexicalSortKey As LexicalSortKey

		Private _lazyIsExtensibleInterface As ThreeState

		Private _lazyIsExplicitDefinitionOfNoPiaLocalType As ThreeState

		Private _comClassData As SourceNamedTypeSymbol.ComClassData

		Private _lazyCoClassType As TypeSymbol

		Protected m_baseCycleDiagnosticInfo As DiagnosticInfo

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				If (Me._lazyCoClassType = ErrorTypeSymbol.UnknownResultType) Then
					If (Me.IsInterface) Then
						Me.GetDecodedWellKnownAttributeData()
						If (Me._lazyCoClassType = ErrorTypeSymbol.UnknownResultType) Then
							Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyCoClassType, Nothing, ErrorTypeSymbol.UnknownResultType)
						End If
					Else
						Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyCoClassType, Nothing, ErrorTypeSymbol.UnknownResultType)
					End If
				End If
				Return Me._lazyCoClassType
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.Type
			End Get
		End Property

		Public Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				If (Me.IsEnumType()) Then
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._lazyEnumUnderlyingType
					If (namedTypeSymbol1 Is Nothing) Then
						Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
						Dim item As SyntaxReference = MyBase.SyntaxReferences(0)
						Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = item.SyntaxTree
						Dim syntax As EnumBlockSyntax = DirectCast(item.GetSyntax(New CancellationToken()), EnumBlockSyntax)
						Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(MyBase.ContainingSourceModule, syntaxTree, Me)
						namedTypeSymbol1 = Me.BindEnumUnderlyingType(syntax, binder, instance)
						If (Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(Me._lazyEnumUnderlyingType, namedTypeSymbol1, Nothing) IsNot Nothing) Then
							namedTypeSymbol1 = Me._lazyEnumUnderlyingType
						Else
							MyBase.ContainingSourceModule.AddDeclarationDiagnostics(instance)
						End If
						instance.Free()
					End If
					namedTypeSymbol = namedTypeSymbol1
				Else
					namedTypeSymbol = Nothing
				End If
				Return namedTypeSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As TypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCodeAnalysisEmbeddedAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasDeclarativeSecurity
			End Get
		End Property

		Friend ReadOnly Property HasSecurityCriticalAttributes As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasSecurityCriticalAttributes
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasSpecialNameAttribute
			End Get
		End Property

		Friend ReadOnly Property HasStructLayoutAttribute As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasStructLayoutAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As TypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasVisualBasicEmbeddedAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As TypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasComImportAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasExcludeFromCodeCoverageAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitDefinitionOfNoPiaLocalType As Boolean
			Get
				If (Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown) Then
					Me.CheckPresenceOfTypeIdentifierAttribute()
					If (Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown) Then
						Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.[False]
					End If
				End If
				Return Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.[True]
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

		Public NotOverridable Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasSerializableAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasWindowsRuntimeImportAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Layout As TypeLayout
			Get
				Dim typeLayout As Microsoft.CodeAnalysis.TypeLayout
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing OrElse Not decodedWellKnownAttributeData.HasStructLayoutAttribute) Then
					typeLayout = If(Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Struct, New Microsoft.CodeAnalysis.TypeLayout(), New Microsoft.CodeAnalysis.TypeLayout(LayoutKind.Sequential, If(Me.HasInstanceFields(), 0, 1), 0))
				Else
					typeLayout = decodedWellKnownAttributeData.Layout
				End If
				Return typeLayout
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData IsNot Nothing AndAlso decodedWellKnownAttributeData.HasStructLayoutAttribute) Then
					Return decodedWellKnownAttributeData.MarshallingCharSet
				End If
				Return MyBase.DefaultMarshallingCharSet
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim uninitialized As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
				If (mLazyCustomAttributesBag Is Nothing OrElse Not mLazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
					Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = MyBase.TypeDeclaration.Declarations.GetEnumerator()
					While enumerator.MoveNext()
						If (Not enumerator.Current.HasAnyAttributes) Then
							Continue While
						End If
						uninitialized = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
						Return uninitialized
					End While
					uninitialized = Nothing
				Else
					Dim earlyDecodedWellKnownAttributeData As CommonTypeEarlyWellKnownAttributeData = DirectCast(mLazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, CommonTypeEarlyWellKnownAttributeData)
					If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
						obsoleteAttributeDatum = earlyDecodedWellKnownAttributeData.ObsoleteAttributeData
					Else
						obsoleteAttributeDatum = Nothing
					End If
					uninitialized = obsoleteAttributeDatum
				End If
				Return uninitialized
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
			Get
				Return Me._corTypeId
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				If (Me._lazyTypeParameters.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of TypeParameterSymbol)(Me._lazyTypeParameters, Me.MakeTypeParameters())
				End If
				Return Me._lazyTypeParameters
			End Get
		End Property

		Friend Sub New(ByVal declaration As MergedTypeDeclaration, ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal containingModule As SourceModuleSymbol)
			MyBase.New(declaration, containingSymbol, containingModule)
			Me._lazyLexicalSortKey = LexicalSortKey.NotInitialized
			Me._lazyIsExtensibleInterface = ThreeState.Unknown
			Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown
			Me._lazyCoClassType = ErrorTypeSymbol.UnknownResultType
			Me.m_baseCycleDiagnosticInfo = Nothing
			If (containingSymbol.Kind <> SymbolKind.[Namespace] OrElse Not containingSymbol.ContainingAssembly.KeepLookingForDeclaredSpecialTypes OrElse Me.DeclaredAccessibility <> Accessibility.[Public]) Then
				Me._corTypeId = Microsoft.CodeAnalysis.SpecialType.None
			Else
				Dim str As String = MetadataHelpers.BuildQualifiedName(If(Me.GetEmittedNamespaceName(), MyBase.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)), Me.MetadataName)
				Me._corTypeId = SpecialTypes.GetTypeFromMetadataName(str)
			End If
			If (containingSymbol.Kind = SymbolKind.NamedType) Then
				Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.[False]
			End If
		End Sub

		Protected Overrides Sub AddDeclaredNonTypeMembers(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::AddDeclaredNonTypeMembers(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/MembersAndInitializersBuilder,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void AddDeclaredNonTypeMembers(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/MembersAndInitializersBuilder,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Protected Overrides Sub AddEntryPointIfNeeded(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
			Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso Not MyBase.IsGenericType) Then
				Dim mainTypeName As String = Me.DeclaringCompilation.Options.MainTypeName
				If (mainTypeName IsNot Nothing AndAlso CaseInsensitiveComparison.EndsWith(mainTypeName, MyBase.Name) AndAlso CaseInsensitiveComparison.Equals(mainTypeName, MyBase.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat))) Then
					Dim wellKnownType As NamedTypeSymbol = Me.DeclaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Windows_Forms_Form)
					If (Not wellKnownType.IsErrorType()) Then
						Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						If (Me.IsOrDerivedFrom(wellKnownType, discarded)) Then
							Dim str As String = "Main"
							If (Not membersBuilder.Members.ContainsKey(str) AndAlso Not MyBase.GetTypeMembersDictionary().ContainsKey(str)) Then
								Dim symbols As ArrayBuilder(Of Symbol) = Nothing
								Dim flag As Boolean = False
								If (membersBuilder.Members.TryGetValue(".ctor", symbols)) Then
									Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = symbols.GetEnumerator()
									While enumerator.MoveNext()
										Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
										If (methodSymbol.MethodKind <> MethodKind.Constructor OrElse methodSymbol.ParameterCount <> 0) Then
											Continue While
										End If
										flag = True
										Exit While
									End While
									If (Not flag) Then
										Dim enumerator1 As ArrayBuilder(Of Symbol).Enumerator = symbols.GetEnumerator()
										Do
											If (Not enumerator1.MoveNext()) Then
												GoTo Label0
											End If
											current = DirectCast(enumerator1.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
										Loop While current.MethodKind <> MethodKind.Constructor OrElse Not current.CanBeCalledWithNoParameters()
										flag = True
									End If
								End If
							Label0:
								If (flag) Then
									Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = MyBase.SyntaxReferences.First()
									Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(MyBase.ContainingSourceModule, syntaxReference.SyntaxTree, Me)
									Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
									Dim synthesizedMainTypeEntryPoint As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMainTypeEntryPoint = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMainTypeEntryPoint(syntaxReference.GetVisualBasicSyntax(cancellationToken), Me)
									MyBase.AddMember(synthesizedMainTypeEntryPoint, binder, membersBuilder, True)
								End If
							End If
						End If
					End If
				End If
			End If
		End Sub

		Private Sub AddEnumMembers(ByVal syntax As EnumBlockSyntax, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
			Dim sourceEnumConstantSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol
			Dim synthesizedFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedFieldSymbol(Me, Me, Me.EnumUnderlyingType, "value__", Accessibility.[Public], False, False, True)
			MyBase.AddMember(synthesizedFieldSymbol, bodyBinder, members, False)
			Dim sourceEnumConstantSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol = Nothing
			Dim num As Integer = 0
			If (syntax.Members.Count = 0) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.EnumStatement.Identifier, ERRID.ERR_BadEmptyEnum1, New [Object]() { syntax.EnumStatement.Identifier.ValueText })
				Return
			End If
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = syntax.Members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As StatementSyntax = enumerator.Current
				If (current.Kind() <> SyntaxKind.EnumMemberDeclaration) Then
					Continue While
				End If
				Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax)
				Dim initializer As EqualsValueSyntax = enumMemberDeclarationSyntax.Initializer
				sourceEnumConstantSymbol = If(initializer Is Nothing, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol.CreateImplicitValuedConstant(Me, bodyBinder, enumMemberDeclarationSyntax, sourceEnumConstantSymbol1, num, diagnostics), Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol.CreateExplicitValuedConstant(Me, bodyBinder, enumMemberDeclarationSyntax, diagnostics))
				If (initializer IsNot Nothing OrElse sourceEnumConstantSymbol1 Is Nothing) Then
					sourceEnumConstantSymbol1 = sourceEnumConstantSymbol
					num = 1
				Else
					num = num + 1
				End If
				MyBase.AddMember(sourceEnumConstantSymbol, bodyBinder, members, False)
			End While
		End Sub

		Protected Overrides Sub AddGroupClassMembersIfNeeded(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim strArray As String()
			Dim flag As Boolean
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso Not MyBase.IsGenericType) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = Nothing
				Dim myGroupCollectionAttributeData As VisualBasicAttributeData = Me.GetMyGroupCollectionAttributeData(diagnostics, binder, attributeSyntax)
				If (myGroupCollectionAttributeData IsNot Nothing) Then
					Dim chrArray() As [Char] = { ","C }
					Dim chrArray1() As [Char] = { "."C }
					Dim strArray1 As String() = (If(myGroupCollectionAttributeData.GetConstructorArgument(Of String)(0, Microsoft.CodeAnalysis.SpecialType.System_String), "")).Split(chrArray, StringSplitOptions.None)
					Dim strArray2 As String() = (If(myGroupCollectionAttributeData.GetConstructorArgument(Of String)(1, Microsoft.CodeAnalysis.SpecialType.System_String), "")).Split(chrArray, StringSplitOptions.None)
					Dim strArray3 As String() = (If(myGroupCollectionAttributeData.GetConstructorArgument(Of String)(2, Microsoft.CodeAnalysis.SpecialType.System_String), "")).Split(chrArray, StringSplitOptions.None)
					strArray = If(Not attributeSyntax.SyntaxTree.IsMyTemplate(), Array.Empty(Of String)(), (If(myGroupCollectionAttributeData.GetConstructorArgument(Of String)(3, Microsoft.CodeAnalysis.SpecialType.System_String), "")).Split(chrArray, StringSplitOptions.None))
					Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
					Dim flag1 As Boolean = False
					Dim num As Integer = Math.Min(CInt(strArray1.Length), CInt(strArray2.Length)) - 1
					Dim num1 As Integer = 0
					Do
						strArray1(num1) = strArray1(num1).Trim()
						strArray2(num1) = strArray2(num1).Trim()
						If (strArray1(num1).Length = 0 OrElse strArray2(num1).Length = 0) Then
							Exit Do
						End If
						If (num1 < CInt(strArray3.Length)) Then
							strArray3(num1) = strArray3(num1).Trim()
						End If
						If (num1 < CInt(strArray.Length)) Then
							strArray(num1) = strArray(num1).Trim()
						End If
						SourceNamedTypeSymbol.FindGroupClassBaseTypes(strArray1(num1).Split(chrArray1, StringSplitOptions.None), Me.DeclaringCompilation.GlobalNamespace, 0, instance)
						If (Not flag1 AndAlso instance.Count > 0 AndAlso instance.Last() IsNot Nothing) Then
							flag1 = True
						End If
						instance.Add(Nothing)
						num1 = num1 + 1
					Loop While num1 <= num
					If (flag1) Then
						Dim keyValuePairs As ArrayBuilder(Of KeyValuePair(Of NamedTypeSymbol, Integer)) = ArrayBuilder(Of KeyValuePair(Of NamedTypeSymbol, Integer)).GetInstance()
						SourceNamedTypeSymbol.GetMyGroupCollectionTypes(Me.ContainingModule.GlobalNamespace, instance, binder, keyValuePairs)
						If (keyValuePairs.Count > 0) Then
							keyValuePairs.Sort(SourceNamedTypeSymbol.GroupCollectionComparer.Singleton)
							Dim count As Integer = keyValuePairs.Count - 1
							For i As Integer = 0 To count
								Dim item As KeyValuePair(Of NamedTypeSymbol, Integer) = keyValuePairs(i)
								If (i > 0 AndAlso CaseInsensitiveComparison.Equals(item.Key.Name, keyValuePairs(i - 1).Key.Name)) Then
									flag = True
								ElseIf (i >= keyValuePairs.Count - 1) Then
									flag = False
								Else
									Dim name As String = item.Key.Name
									Dim keyValuePair As KeyValuePair(Of NamedTypeSymbol, Integer) = keyValuePairs(i + 1)
									flag = CaseInsensitiveComparison.Equals(name, keyValuePair.Key.Name)
								End If
								Dim flag2 As Boolean = flag
								Me.AddSyntheticMyGroupCollectionProperty(item.Key, flag2, strArray2(item.Value), If(item.Value < CInt(strArray3.Length), strArray3(item.Value), ""), If(item.Value < CInt(strArray.Length), strArray(item.Value), ""), membersBuilder, binder, attributeSyntax, diagnostics)
							Next

						End If
						keyValuePairs.Free()
					End If
					instance.Free()
				End If
			End If
		End Sub

		Private Function AddMembersInPart(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As VisualBasicSyntaxNode, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessModifiers As DeclarationModifiers, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByRef staticInitializers As ArrayBuilder(Of FieldOrPropertyInitializer), ByRef instanceInitializers As ArrayBuilder(Of FieldOrPropertyInitializer), ByRef nodeNameIsAlreadyDefined As Boolean) As DeclarationModifiers
			Me.CheckDeclarationNameAndTypeParameters(node, binder, diagBag, nodeNameIsAlreadyDefined)
			Dim declarationModifier As DeclarationModifiers = Me.CheckDeclarationModifiers(node, binder, diagBag.DiagnosticBag, accessModifiers)
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
				If (members.Members.Count = 0) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					Dim methodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					Dim parameterList As ParameterListSyntax = DirectCast(node, DelegateStatementSyntax).ParameterList
					SourceDelegateMethodSymbol.MakeDelegateMembers(Me, node, parameterList, binder, methodSymbol, methodSymbol1, methodSymbol2, methodSymbol3, diagBag)
					MyBase.AddSymbolToMembers(methodSymbol, members.Members)
					If (methodSymbol1 IsNot Nothing) Then
						MyBase.AddSymbolToMembers(methodSymbol1, members.Members)
					End If
					If (methodSymbol2 IsNot Nothing) Then
						MyBase.AddSymbolToMembers(methodSymbol2, members.Members)
					End If
					MyBase.AddSymbolToMembers(methodSymbol3, members.Members)
				End If
			ElseIf (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = DirectCast(node, TypeBlockSyntax).Members.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As StatementSyntax = enumerator.Current
					MyBase.AddMember(current, binder, diagBag, members, staticInitializers, instanceInitializers, False)
				End While
			Else
				Me.AddEnumMembers(DirectCast(node, EnumBlockSyntax), binder, diagBag, members)
			End If
			Return declarationModifier
		End Function

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (Not [String].IsNullOrEmpty(Me.DefaultPropertyName) AndAlso Not Me.HasDefaultMemberAttribute()) Then
				Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String)
				Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(specialType, TypedConstantKind.Primitive, Me.DefaultPropertyName))
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
				Dim typedConstants1 As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute__ctor, typedConstants1, keyValuePairs, False))
			End If
			If (Me._comClassData IsNot Nothing) Then
				If (Me._comClassData.ClassId IsNot Nothing) Then
					Dim typedConstants2 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String), TypedConstantKind.Primitive, Me._comClassData.ClassId))
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, typedConstants2, keyValuePairs, False))
				End If
				Dim typedConstants3 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32), TypedConstantKind.[Enum], 0))
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType, typedConstants3, keyValuePairs, False))
				Dim synthesizedEventInterface As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._comClassData.GetSynthesizedEventInterface()
				If (synthesizedEventInterface IsNot Nothing) Then
					Dim name As String = synthesizedEventInterface.Name
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.ContainingType
					While containingType IsNot Nothing
						name = [String].Concat(namedTypeSymbol.Name, "+", name)
						namedTypeSymbol = containingType
						containingType = namedTypeSymbol.ContainingType
					End While
					name = [String].Concat(namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), "+", name)
					Dim typedConstants4 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String), TypedConstantKind.Primitive, name))
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString, typedConstants4, keyValuePairs, False))
				End If
			End If
			Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics IsNot Nothing AndAlso baseTypeNoUseSiteDiagnostics.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeTupleNamesAttribute(baseTypeNoUseSiteDiagnostics))
			End If
		End Sub

		Private Sub AddSyntheticMyGroupCollectionProperty(ByVal targetType As NamedTypeSymbol, ByVal mangleNames As Boolean, ByVal createMethod As String, ByVal disposeMethod As String, ByVal defaultInstanceAlias As String, ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim name As String
			If (Not mangleNames) Then
				name = targetType.Name
			Else
				name = targetType.ToDisplayString(Nothing)
				name = name.Replace("."C, Strings.ChrW(95))
			End If
			Dim str As String = [String].Concat("m_", name)
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
			Dim typeMembersDictionary As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)) = MyBase.GetTypeMembersDictionary()
			Dim flag As Boolean = Me.IsCompilationOutputWinMdObj()
			If (SourceNamedTypeSymbol.ConflictsWithExistingMemberOrType(name, membersBuilder, typeMembersDictionary, symbol) OrElse SourceNamedTypeSymbol.ConflictsWithExistingMemberOrType(Microsoft.CodeAnalysis.VisualBasic.Binder.GetAccessorName(name, MethodKind.PropertyGet, False), membersBuilder, typeMembersDictionary, symbol) OrElse disposeMethod.Length > 0 AndAlso SourceNamedTypeSymbol.ConflictsWithExistingMemberOrType(Microsoft.CodeAnalysis.VisualBasic.Binder.GetAccessorName(name, MethodKind.PropertySet, flag), membersBuilder, typeMembersDictionary, symbol) OrElse SourceNamedTypeSymbol.ConflictsWithExistingMemberOrType(str, membersBuilder, typeMembersDictionary, symbol)) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, attributeSyntax, ERRID.ERR_PropertyNameConflictInMyCollection, New [Object]() { symbol, targetType })
				Return
			End If
			Dim synthesizedMyGroupCollectionPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMyGroupCollectionPropertySymbol(Me, attributeSyntax, name, str, targetType, createMethod, disposeMethod, defaultInstanceAlias)
			MyBase.AddMember(synthesizedMyGroupCollectionPropertySymbol.AssociatedField, binder, membersBuilder, True)
			MyBase.AddMember(synthesizedMyGroupCollectionPropertySymbol, binder, membersBuilder, True)
			MyBase.AddMember(synthesizedMyGroupCollectionPropertySymbol.GetMethod, binder, membersBuilder, True)
			If (synthesizedMyGroupCollectionPropertySymbol.SetMethod IsNot Nothing) Then
				MyBase.AddMember(synthesizedMyGroupCollectionPropertySymbol.SetMethod, binder, membersBuilder, True)
			End If
		End Sub

		Private Function AsPeOrRetargetingType(ByVal potentialBaseType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(potentialBaseType, PENamedTypeSymbol)
			If (namedTypeSymbol Is Nothing) Then
				namedTypeSymbol = TryCast(potentialBaseType, RetargetingNamedTypeSymbol)
			End If
			Return namedTypeSymbol
		End Function

		Private Function BindEnumUnderlyingType(ByVal syntax As EnumBlockSyntax, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim specialType As NamedTypeSymbol
			Dim underlyingType As AsClauseSyntax = syntax.EnumStatement.UnderlyingType
			If (underlyingType IsNot Nothing AndAlso Not underlyingType.Type().IsMissing) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = bodyBinder.BindTypeSyntax(underlyingType.Type(), diagnostics, False, False, False)
				If (typeSymbol.IsValidEnumUnderlyingType()) Then
					specialType = DirectCast(typeSymbol, NamedTypeSymbol)
					Return specialType
				End If
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, underlyingType.Type(), ERRID.ERR_InvalidEnumBase)
			End If
			specialType = bodyBinder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32, syntax.EnumStatement.Identifier, diagnostics)
			Return specialType
		End Function

		Friend Sub BindTypeParameterConstraints(ByVal typeParameter As SourceTypeParameterOnTypeSymbol, <Out> ByRef variance As VarianceKind, <Out> ByRef constraints As ImmutableArray(Of TypeParameterConstraint), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.GetTypeMembersDictionary()
			Dim typeParameterInfo As SourceNamedTypeSymbol.TypeParameterInfo = New SourceNamedTypeSymbol.TypeParameterInfo()
			Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = MyBase.SyntaxReferences.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxReference = enumerator.Current
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = current.SyntaxTree
				Dim visualBasicSyntax As VisualBasicSyntaxNode = current.GetVisualBasicSyntax(New CancellationToken())
				Dim flag As Boolean = False
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = visualBasicSyntax.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = True
				End If
				Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = SourceNamedTypeSymbol.GetTypeParameterListSyntax(visualBasicSyntax)
				Me.CreateTypeParameterInfoInPart(syntaxTree, typeParameter, typeParameterListSyntax, flag, typeParameterInfo, diagnostics)
			End While
			variance = typeParameterInfo.Variance
			constraints = typeParameterInfo.Constraints
		End Sub

		Private Sub CheckBaseConstraints()
			If ((Me.m_lazyState And 4) = 0) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Nothing
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
				If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
					Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = Me.FirstDeclarationWithExplicitBases()
					If (singleTypeDeclaration IsNot Nothing) Then
						Dim nameLocation As Location = singleTypeDeclaration.NameLocation
						instance = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
						baseTypeNoUseSiteDiagnostics.CheckAllConstraints(nameLocation, instance, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, Me.m_containingModule.ContainingAssembly))
						If (MyBase.IsGenericType) Then
							Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(instance, Me.m_containingModule.ContainingAssembly)
							Dim flag As Boolean = Me.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Attribute).IsBaseTypeOf(baseTypeNoUseSiteDiagnostics, compoundUseSiteInfo)
							instance.Add(nameLocation, compoundUseSiteInfo)
							If (flag) Then
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, nameLocation, ERRID.ERR_GenericClassCannotInheritAttr)
							End If
						End If
					End If
				End If
				Me.m_containingModule.AtomicSetFlagAndStoreDiagnostics(Me.m_lazyState, 4, 0, instance)
				If (instance IsNot Nothing) Then
					instance.Free()
				End If
			End If
		End Sub

		Private Function CheckDeclarationModifiers(ByVal node As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag, ByVal accessModifiers As DeclarationModifiers) As DeclarationModifiers
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationModifiers Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::CheckDeclarationModifiers(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag,Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationModifiers)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationModifiers CheckDeclarationModifiers(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag,Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationModifiers)
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

		Private Sub CheckDeclarationNameAndTypeParameters(ByVal node As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef nodeNameIsAlreadyDeclared As Boolean)
			Dim symbols As ImmutableArray(Of Symbol)
			Dim kindText As Object
			Dim typeIdentifierToken As SyntaxToken = Me.GetTypeIdentifierToken(node)
			Microsoft.CodeAnalysis.VisualBasic.Binder.DisallowTypeCharacter(typeIdentifierToken, diagBag, ERRID.ERR_TypecharNotallowed)
			Dim isEmbedded As Boolean = MyBase.IsEmbedded
			Dim containingSymbol As NamespaceOrTypeSymbol = TryCast(Me.ContainingSymbol, NamespaceOrTypeSymbol)
			If (containingSymbol IsNot Nothing) Then
				symbols = If(Not containingSymbol.IsNamespace, StaticCast(Of Symbol).From(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(containingSymbol.GetTypeMembers(MyBase.Name)), containingSymbol.GetMembers(MyBase.Name))
				Dim arity As Integer = MyBase.Arity
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = symbols.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						GoTo Label0
					End If
					Dim current As Symbol = enumerator.Current
					If (current <> Me) Then
						Dim kind As SymbolKind = current.Kind
						If (kind = SymbolKind.NamedType) Then
							Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							If (namedTypeSymbol.Arity <> arity) Then
								Continue While
							End If
							kindText = namedTypeSymbol.GetKindText()
						Else
							If (kind <> SymbolKind.[Namespace] OrElse arity > 0) Then
								Continue While
							End If
							kindText = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).GetKindText()
						End If
						If (current.IsEmbedded) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_TypeClashesWithVbCoreType4, New [Object]() { Me.GetKindText(), typeIdentifierToken.ToString(), kindText, current.Name })
							Exit While
						ElseIf (Not isEmbedded) Then
							If (Me.ContainingType IsNot Nothing AndAlso containingSymbol.Locations.Length <> 1 AndAlso TypeOf containingSymbol Is SourceMemberContainerTypeSymbol AndAlso Not DirectCast(containingSymbol, SourceMemberContainerTypeSymbol).IsPartial) Then
								Exit While
							End If
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_TypeConflict6, New [Object]() { Me.GetKindText(), typeIdentifierToken.ToString(), kindText, current.Name, containingSymbol.GetKindText(), Me.ContainingSymbol.ToErrorMessageArgument(ERRID.ERR_TypeConflict6) })
							Exit While
						ElseIf (current.Kind = SymbolKind.[Namespace]) Then
							Dim flag As Boolean = False
							Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
							While enumerator1.MoveNext()
								Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
								If (Not location.IsInSource OrElse DirectCast(location.SourceTree, VisualBasicSyntaxTree).IsEmbeddedSyntaxTree()) Then
									Continue While
								End If
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, location, ERRID.ERR_TypeClashesWithVbCoreType4, New [Object]() { kindText, current.Name, Me.GetKindText(), typeIdentifierToken.ToString() })
								flag = True
								Exit While
							End While
							If (flag) Then
								GoTo Label0
							End If
						End If
					End If
				End While
				nodeNameIsAlreadyDeclared = True
			Label0:
				If (Not nodeNameIsAlreadyDeclared AndAlso containingSymbol.IsNamespace AndAlso Me.ContainingAssembly.Modules.Length > 1) Then
					Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
					Dim assemblyNamespace As MergedNamespaceSymbol = TryCast(Me.ContainingAssembly.GetAssemblyNamespace(namespaceSymbol), MergedNamespaceSymbol)
					If (assemblyNamespace IsNot Nothing) Then
						Dim emittedNamespaceName As String = If(Me.GetEmittedNamespaceName(), namespaceSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat))
						Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
						Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol).Enumerator = assemblyNamespace.ConstituentNamespaces.GetEnumerator()
					Label1:
						While enumerator2.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = enumerator2.Current
							If (current1 = containingSymbol OrElse namedTypeSymbol1 IsNot Nothing AndAlso namedTypeSymbol1.ContainingModule.Ordinal < current1.ContainingModule.Ordinal) Then
								Continue While
							End If
							Dim typeMembers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = current1.GetTypeMembers(MyBase.Name, arity)
							If (typeMembers.Length = 0) Then
								Continue While
							End If
							Dim displayString As String = current1.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
							Dim enumerator3 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
							While enumerator3.MoveNext()
								Dim current2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator3.Current
								If (current2.DeclaredAccessibility <> Accessibility.[Public] OrElse current2.MangleName <> MyBase.MangleName OrElse Not [String].Equals(MyBase.Name, current2.Name, StringComparison.Ordinal)) Then
									Continue While
								End If
								If (Not [String].Equals(emittedNamespaceName, If(current2.GetEmittedNamespaceName(), displayString), StringComparison.Ordinal)) Then
									Continue While
								End If
								namedTypeSymbol1 = current2
								GoTo Label1
							End While
						End While
						If (namedTypeSymbol1 IsNot Nothing) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_CollisionWithPublicTypeInModule, New [Object]() { Me, namedTypeSymbol1.ContainingModule })
						End If
					End If
				End If
			End If
			Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
			If (sourceNamedTypeSymbol IsNot Nothing AndAlso sourceNamedTypeSymbol.TypeParameters.MatchesAnyName(MyBase.Name)) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_ShadowingGenericParamWithMember1, New [Object]() { MyBase.Name })
			End If
			Me.CheckForDuplicateTypeParameters(Me.TypeParameters, diagBag)
		End Sub

		Private Sub CheckDeclarationPart(ByVal tree As SyntaxTree, ByVal node As VisualBasicSyntaxNode, ByVal firstNode As VisualBasicSyntaxNode, ByVal foundPartial As Boolean, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::CheckDeclarationPart(Microsoft.CodeAnalysis.SyntaxTree,Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void CheckDeclarationPart(Microsoft.CodeAnalysis.SyntaxTree,Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Friend Sub CheckForDuplicateTypeParameters(ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not typeParameters.IsDefault) Then
				Dim strs As HashSet(Of String) = New HashSet(Of String)(CaseInsensitiveComparison.Comparer)
				Dim length As Integer = typeParameters.Length - 1
				For i As Integer = 0 To length
					Dim item As TypeParameterSymbol = typeParameters(i)
					If (strs.Contains(item.Name)) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item.Locations(0), ERRID.ERR_DuplicateTypeParamName1, New [Object]() { item.Name })
					Else
						strs.Add(item.Name)
						If (Me.ShadowsTypeParameter(item)) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item.Locations(0), ERRID.WRN_ShadowingGenericParamWithParam1, New [Object]() { item.Name })
						End If
					End If
				Next

			End If
		End Sub

		Private Sub CheckInterfacesConstraints()
			If ((Me.m_lazyState And 8) = 0) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Nothing
				Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = MyBase.InterfacesNoUseSiteDiagnostics
				If (Not interfacesNoUseSiteDiagnostics.IsEmpty) Then
					Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = Me.FirstDeclarationWithExplicitInterfaces()
					If (singleTypeDeclaration IsNot Nothing) Then
						Dim nameLocation As Location = singleTypeDeclaration.NameLocation
						instance = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
						Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = interfacesNoUseSiteDiagnostics.GetEnumerator()
						While enumerator.MoveNext()
							enumerator.Current.CheckAllConstraints(nameLocation, instance, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, Me.m_containingModule.ContainingAssembly))
						End While
					End If
				End If
				If (Me.m_containingModule.AtomicSetFlagAndStoreDiagnostics(Me.m_lazyState, 8, 0, instance)) Then
					Me.DeclaringCompilation.SymbolDeclaredEvent(Me)
				End If
				If (instance IsNot Nothing) Then
					instance.Free()
				End If
			End If
		End Sub

		Private Sub CheckNoBase(Of T As InheritsOrImplementsStatementSyntax)(ByVal baseDeclList As SyntaxList(Of T), ByVal errId As Microsoft.CodeAnalysis.VisualBasic.ERRID, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (baseDeclList.Count > 0) Then
				Dim enumerator As SyntaxList(Of T).Enumerator = baseDeclList.GetEnumerator()
				While enumerator.MoveNext()
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, DirectCast(enumerator.Current, SyntaxNode), errId)
				End While
			End If
		End Sub

		Private Sub CheckPresenceOfTypeIdentifierAttribute()
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::CheckPresenceOfTypeIdentifierAttribute()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void CheckPresenceOfTypeIdentifierAttribute()
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

		Private Shared Function ConflictsWithExistingMemberOrType(ByVal name As String, ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal nestedTypes As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)), <Out> ByRef conflictsWith As Symbol) As Boolean
			Dim symbols As ArrayBuilder(Of Symbol) = Nothing
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
			If (membersBuilder.Members.TryGetValue(name, symbols)) Then
				conflictsWith = symbols(0)
			ElseIf (Not nestedTypes.TryGetValue(name, namedTypeSymbols)) Then
				conflictsWith = Nothing
			Else
				conflictsWith = namedTypeSymbols(0)
			End If
			Return CObj(conflictsWith) <> CObj(Nothing)
		End Function

		Private Function CreateLocationSpecificBinderForType(ByVal tree As SyntaxTree, ByVal location As BindingLocation) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(MyBase.ContainingSourceModule, tree, Me)
			Return New LocationSpecificBinder(location, binder)
		End Function

		Private Sub CreateTypeParameterInfoInPart(ByVal tree As SyntaxTree, ByVal typeParameter As SourceTypeParameterOnTypeSymbol, ByVal typeParamListSyntax As TypeParameterListSyntax, ByVal allowVarianceSpecifier As Boolean, ByRef info As SourceNamedTypeSymbol.TypeParameterInfo, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateLocationSpecificBinderForType(tree, BindingLocation.GenericConstraintsClause)
			Dim item As TypeParameterSyntax = typeParamListSyntax.Parameters(typeParameter.Ordinal)
			Dim identifier As SyntaxToken = item.Identifier
			Microsoft.CodeAnalysis.VisualBasic.Binder.DisallowTypeCharacter(identifier, diagBag, ERRID.ERR_TypeCharOnGenericParam)
			Dim valueText As String = identifier.ValueText
			Dim varianceKeyword As SyntaxToken = item.VarianceKeyword
			Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind = Microsoft.CodeAnalysis.VarianceKind.None
			If (varianceKeyword.Kind() <> SyntaxKind.None) Then
				If (Not allowVarianceSpecifier) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, varianceKeyword, ERRID.ERR_VarianceDisallowedHere)
				Else
					varianceKind = Microsoft.CodeAnalysis.VisualBasic.Binder.DecodeVariance(varianceKeyword)
				End If
			End If
			Dim typeParameterConstraints As ImmutableArray(Of TypeParameterConstraint) = binder.BindTypeParameterConstraintClause(Me, item.TypeParameterConstraintClause, diagBag)
			If (Not info.Initialized) Then
				info = New SourceNamedTypeSymbol.TypeParameterInfo(varianceKind, typeParameterConstraints)
			Else
				If (Not CaseInsensitiveComparison.Equals(typeParameter.Name, valueText)) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, identifier, ERRID.ERR_PartialTypeTypeParamNameMismatch3, New [Object]() { valueText, typeParameter.Name, MyBase.Name })
				End If
				If (Not SourceNamedTypeSymbol.HaveSameConstraints(info.Constraints, typeParameterConstraints)) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, identifier, ERRID.ERR_PartialTypeConstraintMismatch1, New [Object]() { MyBase.Name })
					Return
				End If
			End If
		End Sub

		Private Function DecodeDeclarationModifiers(ByVal node As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag, ByRef modifiers As SyntaxTokenList, ByRef id As SyntaxToken) As DeclarationModifiers
			Dim foundFlags As DeclarationModifiers
			Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Dim sourceMemberFlag As SourceMemberFlags = SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.InvalidIfDefault
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadModuleFlags1
					sourceMemberFlag = SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial]
					typeBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
					modifiers = typeBlockSyntax.BlockStatement.Modifiers
					id = typeBlockSyntax.BlockStatement.Identifier
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadRecordFlags1
					sourceMemberFlag = SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial]
					typeBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
					modifiers = typeBlockSyntax.BlockStatement.Modifiers
					id = typeBlockSyntax.BlockStatement.Identifier
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadInterfaceFlags1
					sourceMemberFlag = SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial]
					typeBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
					modifiers = typeBlockSyntax.BlockStatement.Modifiers
					id = typeBlockSyntax.BlockStatement.Identifier
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadClassFlags1
					sourceMemberFlag = SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial] Or SourceMemberFlags.[MustInherit] Or SourceMemberFlags.[NotInheritable] Or SourceMemberFlags.TypeInheritModifiers
					typeBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
					modifiers = typeBlockSyntax.BlockStatement.Modifiers
					id = typeBlockSyntax.BlockStatement.Identifier
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadEnumFlags1
					Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax)
					modifiers = enumBlockSyntax.EnumStatement.Modifiers
					id = enumBlockSyntax.EnumStatement.Identifier
					Exit Select
				Case Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Throw ExceptionUtilities.UnexpectedValue(node.Kind())
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadDelegateFlags1
					modifiers = DirectCast(node, DelegateStatementSyntax).Modifiers
					id = DirectCast(node, DelegateStatementSyntax).Identifier
					Exit Select
			End Select
			If (modifiers.Count = 0) Then
				foundFlags = DeclarationModifiers.None
			Else
				Dim memberModifier As MemberModifiers = binder.DecodeModifiers(modifiers, sourceMemberFlag, eRRID, Accessibility.NotApplicable, diagBag)
				foundFlags = CInt((memberModifier.FoundFlags And SourceMemberFlags.DeclarationModifierFlagMask)) >> CInt(SourceMemberFlags.AccessibilityProtected)
			End If
			Return foundFlags
		End Function

		Private Function DecodeIsExtensibleInterface() As Boolean
			Dim flag As Boolean
			If (Me.IsInterfaceType()) Then
				Dim earlyDecodedWellKnownAttributeData As TypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing OrElse Not earlyDecodedWellKnownAttributeData.HasAttributeForExtensibleInterface) Then
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

		Friend NotOverridable Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim locations As ImmutableArray(Of Location)
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				diagnostics.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			Dim flag As Boolean = False
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = Me.TypeKind
			If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Interface]) Then
					If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
						If (Me.ContainingSymbol.Kind = SymbolKind.[Namespace] AndAlso attribute.IsTargetAttribute(Me, AttributeDescription.CaseInsensitiveExtensionAttribute)) Then
							MyBase.SuppressExtensionAttributeSynthesis()
							flag = True
						ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.VisualBasicComClassAttribute)) Then
							Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_InvalidAttributeUsage2, New [Object]() { AttributeDescription.VisualBasicComClassAttribute.Name, MyBase.Name })
							locations = MyBase.Locations
							diagnostics.Add(diagnosticInfo, locations(0))
							flag = True
						End If
					End If
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.CoClassAttribute)) Then
					Dim item As TypedConstant = attribute.CommonConstructorArguments(0)
					Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyCoClassType, DirectCast(item.ValueInternal, TypeSymbol), ErrorTypeSymbol.UnknownResultType)
					flag = True
				End If
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.CaseInsensitiveExtensionAttribute)) Then
				Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionOnlyAllowedOnModuleSubOrFunction)
				locations = MyBase.Locations
				diagnostics.Add(diagnosticInfo1, locations(0))
				flag = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.VisualBasicComClassAttribute)) Then
				If (Not MyBase.IsGenericType) Then
					Interlocked.CompareExchange(Of SourceNamedTypeSymbol.ComClassData)(Me._comClassData, New SourceNamedTypeSymbol.ComClassData(attribute), Nothing)
				Else
					locations = MyBase.Locations
					diagnostics.Add(ERRID.ERR_ComClassOnGeneric, locations(0))
				End If
				flag = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.DefaultEventAttribute)) Then
				If (attribute.CommonConstructorArguments.Length = 1 AndAlso attribute.CommonConstructorArguments(0).Kind = TypedConstantKind.Primitive) Then
					Dim valueInternal As String = TryCast(attribute.CommonConstructorArguments(0).ValueInternal, [String])
					If (valueInternal IsNot Nothing AndAlso valueInternal.Length > 0 AndAlso Not Me.FindDefaultEvent(valueInternal)) Then
						diagnostics.Add(ERRID.ERR_DefaultEventNotFound1, arguments.AttributeSyntaxOpt.GetLocation(), New [Object]() { valueInternal })
					End If
				End If
				flag = True
			End If
			If (Not flag) Then
				If (attribute.IsTargetAttribute(Me, AttributeDescription.DefaultMemberAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasDefaultMemberAttribute = True
					Dim str As String = attribute.DecodeDefaultMemberAttribute()
					Dim defaultPropertyName As String = Me.DefaultPropertyName
					If (Not [String].IsNullOrEmpty(defaultPropertyName) AndAlso Not CaseInsensitiveComparison.Equals(defaultPropertyName, str)) Then
						diagnostics.Add(ERRID.ERR_ConflictDefaultPropertyAttribute, MyBase.Locations(0), New [Object]() { Me })
					End If
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SerializableAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasSerializableAttribute = True
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ExcludeFromCodeCoverageAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasExcludeFromCodeCoverageAttribute = True
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SpecialNameAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasSpecialNameAttribute = True
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.StructLayoutAttribute)) Then
					AttributeData.DecodeStructLayoutAttribute(Of CommonTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation)(arguments, MyBase.DefaultMarshallingCharSet, If(Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct, 1, 0), MessageProvider.Instance)
					If (MyBase.IsGenericType) Then
						diagnostics.Add(ERRID.ERR_StructLayoutAttributeNotAllowed, arguments.AttributeSyntaxOpt.GetLocation(), New [Object]() { Me })
					End If
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasSuppressUnmanagedCodeSecurityAttribute = True
				ElseIf (attribute.IsSecurityAttribute(Me.DeclaringCompilation)) Then
					attribute.DecodeSecurityAttribute(Of CommonTypeWellKnownAttributeData)(Me, Me.DeclaringCompilation, arguments)
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ClassInterfaceAttribute)) Then
					attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, diagnostics)
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.InterfaceTypeAttribute)) Then
					attribute.DecodeInterfaceTypeAttribute(arguments.AttributeSyntaxOpt, diagnostics)
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.GuidAttribute)) Then
					attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, diagnostics)
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.WindowsRuntimeImportAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasWindowsRuntimeImportAttribute = True
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SecurityCriticalAttribute) OrElse attribute.IsTargetAttribute(Me, AttributeDescription.SecuritySafeCriticalAttribute)) Then
					arguments.GetOrCreateData(Of CommonTypeWellKnownAttributeData)().HasSecurityCriticalAttributes = True
				ElseIf (Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown AndAlso attribute.IsTargetAttribute(Me, AttributeDescription.TypeIdentifierAttribute)) Then
					Me._lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.[True]
				ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.RequiredAttributeAttribute)) Then
					diagnostics.Add(ERRID.ERR_CantUseRequiredAttribute, arguments.AttributeSyntaxOpt.GetLocation(), New [Object]() { Me })
				End If
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Friend Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::EarlyDecodeWellKnownAttribute(Microsoft.CodeAnalysis.EarlyDecodeWellKnownAttributeArguments`4<Microsoft.CodeAnalysis.VisualBasic.EarlyWellKnownAttributeBinder,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData EarlyDecodeWellKnownAttribute(Microsoft.CodeAnalysis.EarlyDecodeWellKnownAttributeArguments<Microsoft.CodeAnalysis.VisualBasic.EarlyWellKnownAttributeBinder,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation>&)
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

		Private Sub EnsureAllHandlesAreBound()
			If (Not Me._withEventsOverridesAreFrozen) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembersUnordered().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Dim handledEvents As ImmutableArray(Of HandledEvent) = DirectCast(current, MethodSymbol).HandledEvents
				End While
				Me._withEventsOverridesAreFrozen = True
			End If
		End Sub

		Private Shared Function FindBaseInMyGroupCollection(ByVal classType As NamedTypeSymbol, ByVal bases As ArrayBuilder(Of NamedTypeSymbol)) As Integer
			Dim num As Integer
			classType = classType.BaseTypeNoUseSiteDiagnostics
			While True
				If (classType Is Nothing OrElse classType.IsObjectType()) Then
					num = -1
					Exit While
				Else
					Dim num1 As Integer = 0
					Dim enumerator As ArrayBuilder(Of NamedTypeSymbol).Enumerator = bases.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						If (current IsNot Nothing) Then
							If (CObj(classType.OriginalDefinition) <> CObj(current)) Then
								Continue While
							End If
							num = num1
							Return num
						Else
							num1 = num1 + 1
						End If
					End While
					classType = classType.BaseTypeNoUseSiteDiagnostics
				End If
			End While
			Return num
		End Function

		Private Function FindDefaultEvent(ByVal eventName As String) As Boolean
			Dim flag As Boolean
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me
		Label0:
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = baseTypeNoUseSiteDiagnostics.GetMembers(eventName).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					If (current.Kind = SymbolKind.[Event] AndAlso (current.DeclaredAccessibility = Accessibility.[Public] OrElse current.DeclaredAccessibility = Accessibility.Internal)) Then
						flag = True
						Exit While
					End If
				Else
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
					If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
						GoTo Label0
					End If
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Sub FindGroupClassBaseTypes(ByVal nameParts As String(), ByVal current As NamespaceOrTypeSymbol, ByVal nextPart As Integer, ByVal candidates As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
			If (nextPart <> CInt(nameParts.Length)) Then
				Dim str As String = nameParts(nextPart)
				nextPart = nextPart + 1
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = current.GetMembers(str).GetEnumerator()
				While enumerator.MoveNext()
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (CInt(symbol.Kind) - CInt(SymbolKind.NamedType) > CInt(SymbolKind.ArrayType)) Then
						Continue While
					End If
					SourceNamedTypeSymbol.FindGroupClassBaseTypes(nameParts, DirectCast(symbol, NamespaceOrTypeSymbol), nextPart, candidates)
				End While
			ElseIf (current.Kind = SymbolKind.NamedType) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso Not namedTypeSymbol.IsNotInheritable) Then
					candidates.Add(namedTypeSymbol)
					Return
				End If
			End If
		End Sub

		Private Function FirstDeclarationWithExplicitBases() As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration
			Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).Enumerator = MyBase.TypeDeclaration.Declarations.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = enumerator.Current
					Dim visualBasicSyntax As VisualBasicSyntaxNode = current.SyntaxReference.GetVisualBasicSyntax(New CancellationToken())
					If (visualBasicSyntax.Kind() = SyntaxKind.ClassBlock AndAlso DirectCast(visualBasicSyntax, TypeBlockSyntax).[Inherits].Count > 0) Then
						singleTypeDeclaration = current
						Exit While
					End If
				Else
					singleTypeDeclaration = Nothing
					Exit While
				End If
			End While
			Return singleTypeDeclaration
		End Function

		Private Function FirstDeclarationWithExplicitInterfaces() As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration
			Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).Enumerator = MyBase.TypeDeclaration.Declarations.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = enumerator.Current
					Dim visualBasicSyntax As VisualBasicSyntaxNode = current.SyntaxReference.GetVisualBasicSyntax(New CancellationToken())
					Select Case visualBasicSyntax.Kind()
						Case SyntaxKind.StructureBlock
						Case SyntaxKind.ClassBlock
							If (DirectCast(visualBasicSyntax, TypeBlockSyntax).[Implements].Count <= 0) Then
								Continue While
							End If
							singleTypeDeclaration = current

						Case SyntaxKind.InterfaceBlock
							If (DirectCast(visualBasicSyntax, TypeBlockSyntax).[Inherits].Count <= 0) Then
								Continue While
							End If
							singleTypeDeclaration = current

						Case Else
							Continue While
					End Select
				Else
					singleTypeDeclaration = Nothing
					Exit While
				End If
			End While
			Return singleTypeDeclaration
		End Function

		Protected Overrides Sub GenerateAllDeclarationErrorsImpl(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateAllDeclarationErrorsImpl(cancellationToken)
			Me._withEventsOverridesAreFrozen = True
			cancellationToken.ThrowIfCancellationRequested()
			Me.PerformComClassAnalysis()
			cancellationToken.ThrowIfCancellationRequested()
			Me.CheckBaseConstraints()
			cancellationToken.ThrowIfCancellationRequested()
			Me.CheckInterfacesConstraints()
		End Sub

		Friend NotOverridable Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Dim earlyDecodedWellKnownAttributeData As CommonTypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
			If (earlyDecodedWellKnownAttributeData Is Nothing) Then
				Return ImmutableArray(Of String).Empty
			End If
			Return earlyDecodedWellKnownAttributeData.ConditionalSymbols
		End Function

		Private Function GetAttributeDeclarations() As ImmutableArray(Of SyntaxList(Of AttributeListSyntax))
			Return MyBase.TypeDeclaration.GetAttributeDeclarations()
		End Function

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Private Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me.m_lazyCustomAttributesBag Is Nothing OrElse Not Me.m_lazyCustomAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(Me.GetAttributeDeclarations()), Me.m_lazyCustomAttributesBag, AttributeLocation.None)
			End If
			Return Me.m_lazyCustomAttributesBag
		End Function

		Friend NotOverridable Overrides Function GetAttributeUsageInfo() As Microsoft.CodeAnalysis.AttributeUsageInfo
			Dim attributeUsageInfo As Microsoft.CodeAnalysis.AttributeUsageInfo
			Dim earlyDecodedWellKnownAttributeData As TypeEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
			If (earlyDecodedWellKnownAttributeData Is Nothing OrElse earlyDecodedWellKnownAttributeData.AttributeUsageInfo.IsNull) Then
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
				attributeUsageInfo = If(baseTypeNoUseSiteDiagnostics IsNot Nothing, baseTypeNoUseSiteDiagnostics.GetAttributeUsageInfo(), Microsoft.CodeAnalysis.AttributeUsageInfo.[Default])
			Else
				attributeUsageInfo = earlyDecodedWellKnownAttributeData.AttributeUsageInfo
			End If
			Return attributeUsageInfo
		End Function

		Private Shared Function GetConstraintKind(ByVal constraints As ImmutableArray(Of TypeParameterConstraint)) As TypeParameterConstraintKind
			Dim kind As TypeParameterConstraintKind = TypeParameterConstraintKind.None
			Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = constraints.GetEnumerator()
			While enumerator.MoveNext()
				kind = kind Or enumerator.Current.Kind
			End While
			Return kind
		End Function

		Friend Function GetCorrespondingComClassInterfaceMethod(ByVal method As MethodSymbol) As MethodSymbol
			Dim correspondingComClassInterfaceMethod As MethodSymbol
			Me.GetAttributes()
			If (Me._comClassData IsNot Nothing) Then
				Me._comClassData.PerformComClassAnalysis(Me)
				correspondingComClassInterfaceMethod = Me._comClassData.GetCorrespondingComClassInterfaceMethod(method)
			Else
				correspondingComClassInterfaceMethod = Nothing
			End If
			Return correspondingComClassInterfaceMethod
		End Function

		Friend Overrides Function GetDeclaredBaseInterfacesSafe(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			If (Me.m_baseCycleDiagnosticInfo IsNot Nothing) Then
				namedTypeSymbols = New ImmutableArray(Of NamedTypeSymbol)()
			ElseIf (Me <> basesBeingResolved.InheritsBeingResolvedOpt.Head) Then
				Dim dependenceDiagnosticForBase As DiagnosticInfo = BaseTypeAnalysis.GetDependenceDiagnosticForBase(Me, basesBeingResolved)
				If (dependenceDiagnosticForBase IsNot Nothing) Then
					Interlocked.CompareExchange(Of DiagnosticInfo)(Me.m_baseCycleDiagnosticInfo, dependenceDiagnosticForBase, Nothing)
					namedTypeSymbols = New ImmutableArray(Of NamedTypeSymbol)()
				Else
					Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved)
					If (Me.m_baseCycleDiagnosticInfo Is Nothing) Then
						empty = declaredInterfacesNoUseSiteDiagnostics
					Else
						empty = ImmutableArray(Of NamedTypeSymbol).Empty
					End If
					namedTypeSymbols = empty
				End If
			Else
				namedTypeSymbols = New ImmutableArray(Of NamedTypeSymbol)()
			End If
			Return namedTypeSymbols
		End Function

		Private Function GetDeclaredBaseSafe(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (Me.m_baseCycleDiagnosticInfo IsNot Nothing) Then
				namedTypeSymbol = Nothing
			ElseIf (Me <> basesBeingResolved.InheritsBeingResolvedOpt.Head) Then
				Dim dependenceDiagnosticForBase As DiagnosticInfo = BaseTypeAnalysis.GetDependenceDiagnosticForBase(Me, basesBeingResolved)
				If (dependenceDiagnosticForBase IsNot Nothing) Then
					Interlocked.CompareExchange(Of DiagnosticInfo)(Me.m_baseCycleDiagnosticInfo, dependenceDiagnosticForBase, Nothing)
					namedTypeSymbol = Nothing
				Else
					Dim declaredBase As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetDeclaredBase(basesBeingResolved)
					If (Me.m_baseCycleDiagnosticInfo Is Nothing) Then
						namedTypeSymbol1 = declaredBase
					Else
						namedTypeSymbol1 = Nothing
					End If
					namedTypeSymbol = namedTypeSymbol1
				End If
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function

		Private Function GetDecodedWellKnownAttributeData() As CommonTypeWellKnownAttributeData
			Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
			If (mLazyCustomAttributesBag Is Nothing OrElse Not mLazyCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				mLazyCustomAttributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(mLazyCustomAttributesBag.DecodedWellKnownAttributeData, CommonTypeWellKnownAttributeData)
		End Function

		Friend Overrides Function GetDirectBaseTypeNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As NamedTypeSymbol
			Dim specialType As NamedTypeSymbol
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Enum)
			ElseIf (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
				specialType = If(basesBeingResolved.InheritsBeingResolvedOpt IsNot Nothing, Me.GetDeclaredBaseSafe(basesBeingResolved), MyBase.BaseTypeNoUseSiteDiagnostics)
			Else
				specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate)
			End If
			Return specialType
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Dim str As String
			str = If(Not expandIncludes, SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyDocComment, cancellationToken), SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyExpandedDocComment, cancellationToken))
			Return str
		End Function

		Private Function GetEarlyDecodedWellKnownAttributeData() As TypeEarlyWellKnownAttributeData
			Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
			If (mLazyCustomAttributesBag Is Nothing OrElse Not mLazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
				mLazyCustomAttributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(mLazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, TypeEarlyWellKnownAttributeData)
		End Function

		Private Function GetInheritsLocation(ByVal base As NamedTypeSymbol) As Location
			Return Me.GetInheritsOrImplementsLocation(base, True)
		End Function

		Protected Overrides Function GetInheritsOrImplementsLocation(ByVal base As NamedTypeSymbol, ByVal getInherits As Boolean) As Microsoft.CodeAnalysis.Location
			Dim location As Microsoft.CodeAnalysis.Location
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax) = Nothing
			Dim location1 As Microsoft.CodeAnalysis.Location = Nothing
			Dim enumerator1 As ImmutableArray(Of SyntaxReference).Enumerator = MyBase.SyntaxReferences.GetEnumerator()
			While True
				If (enumerator1.MoveNext()) Then
					Dim current As SyntaxReference = enumerator1.Current
					Dim syntax As TypeBlockSyntax = DirectCast(current.GetSyntax(New CancellationToken()), TypeBlockSyntax)
					Dim inheritsOrImplementsStatementSyntaxes As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax) = If(getInherits, DirectCast(syntax.[Inherits], IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax)), DirectCast(syntax.[Implements], IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax)))
					Dim basesBeingResolvedBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateLocationSpecificBinderForType(current.SyntaxTree, BindingLocation.BaseTypes)
					Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
					basesBeingResolved = If(Not getInherits, basesBeingResolved.PrependImplementsBeingResolved(Me), basesBeingResolved.PrependInheritsBeingResolved(Me))
					Using basesBeingResolvedBinder = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolvedBinder(basesBeingResolvedBinder, basesBeingResolved)
						enumerator = inheritsOrImplementsStatementSyntaxes.GetEnumerator()
						While enumerator.MoveNext()
							Dim inheritsOrImplementsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax = enumerator.Current
							If (location1 Is Nothing) Then
								location1 = inheritsOrImplementsStatementSyntax.GetLocation()
							End If
							Dim enumerator2 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax).Enumerator = If(getInherits, DirectCast(inheritsOrImplementsStatementSyntax, InheritsStatementSyntax).Types, DirectCast(inheritsOrImplementsStatementSyntax, ImplementsStatementSyntax).Types).GetEnumerator()
							While enumerator2.MoveNext()
								Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = enumerator2.Current
								If (Not TypeSymbol.Equals(basesBeingResolvedBinder.BindTypeSyntax(typeSyntax, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, True, False, False), base, TypeCompareKind.ConsiderEverything)) Then
									Continue While
								End If
								location = typeSyntax.GetLocation()
								Return location
							End While
						End While
					End Using
				Else
					location = location1
					Exit While
				End If
			End While
			Return location
		End Function

		Private Function GetMyGroupCollectionAttributeData(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax) As VisualBasicAttributeData
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::GetMyGroupCollectionAttributeData(Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag,Microsoft.CodeAnalysis.VisualBasic.Binder&,Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData GetMyGroupCollectionAttributeData(Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag,Microsoft.CodeAnalysis.VisualBasic.Binder&,Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax&)
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

		Private Shared Sub GetMyGroupCollectionTypes(ByVal ns As NamespaceSymbol, ByVal baseTypes As ArrayBuilder(Of NamedTypeSymbol), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal collectionTypes As ArrayBuilder(Of KeyValuePair(Of NamedTypeSymbol, Integer)))
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = ns.GetMembersUnordered().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				Dim kind As SymbolKind = current.Kind
				If (kind = SymbolKind.NamedType) Then
					Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
					If (sourceNamedTypeSymbol Is Nothing OrElse sourceNamedTypeSymbol.IsImplicitlyDeclared OrElse sourceNamedTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Class] OrElse sourceNamedTypeSymbol.IsGenericType OrElse sourceNamedTypeSymbol.IsMustInherit) Then
						Continue While
					End If
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
					If (Not binder.IsAccessible(sourceNamedTypeSymbol, discarded, Nothing, basesBeingResolved)) Then
						Continue While
					End If
					Dim num As Integer = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol.FindBaseInMyGroupCollection(sourceNamedTypeSymbol, baseTypes)
					If (num < 0 OrElse Not Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol.MyGroupCollectionCandidateHasPublicParameterlessConstructor(sourceNamedTypeSymbol)) Then
						Continue While
					End If
					collectionTypes.Add(New KeyValuePair(Of NamedTypeSymbol, Integer)(sourceNamedTypeSymbol, num))
				ElseIf (kind = SymbolKind.[Namespace]) Then
					Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol.GetMyGroupCollectionTypes(DirectCast(current, NamespaceSymbol), baseTypes, binder, collectionTypes)
				End If
			End While
		End Sub

		Friend Function GetOrAddWithEventsOverride(baseProperty As PropertySymbol) As SynthesizedOverridingWithEventsProperty
			Dim overridesDict = Me._lazyWithEventsOverrides
			If overridesDict Is Nothing Then
				Interlocked.CompareExchange(Me._lazyWithEventsOverrides,
											New ConcurrentDictionary(Of PropertySymbol, SynthesizedOverridingWithEventsProperty),
											Nothing)

				overridesDict = Me._lazyWithEventsOverrides
			End If

			Dim result As SynthesizedOverridingWithEventsProperty = Nothing
			If overridesDict.TryGetValue(baseProperty, result) Then
				Return result
			Else
				' we need to create a lambda here since we need to close over baseProperty
				' we will however create a lambda only on a cache miss, hopefully not very often.
				Return overridesDict.GetOrAdd(baseProperty, Function()
																Return New SynthesizedOverridingWithEventsProperty(baseProperty, Me)
															End Function)
			End If
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Dim securityAttributes As IEnumerable(Of SecurityAttribute)
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.GetAttributesBag()
			Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = DirectCast(attributesBag.DecodedWellKnownAttributeData, CommonTypeWellKnownAttributeData)
			If (decodedWellKnownAttributeData IsNot Nothing) Then
				Dim securityInformation As SecurityWellKnownAttributeData = decodedWellKnownAttributeData.SecurityInformation
				If (securityInformation Is Nothing) Then
					securityAttributes = SpecializedCollections.EmptyEnumerable(Of SecurityAttribute)()
					Return securityAttributes
				End If
				securityAttributes = securityInformation.GetSecurityAttributes(Of VisualBasicAttributeData)(attributesBag.Attributes)
				Return securityAttributes
			End If
			securityAttributes = SpecializedCollections.EmptyEnumerable(Of SecurityAttribute)()
			Return securityAttributes
		End Function

		Private Function GetSpecialType(ByVal type As Microsoft.CodeAnalysis.SpecialType) As NamedTypeSymbol
			Return Me.ContainingModule.ContainingAssembly.GetSpecialType(type)
		End Function

		Friend Overrides Function GetSynthesizedImplements() As IEnumerable(Of NamedTypeSymbol)
			Dim synthesizedImplements As IEnumerable(Of NamedTypeSymbol)
			If (Me._comClassData IsNot Nothing) Then
				synthesizedImplements = Me._comClassData.GetSynthesizedImplements()
			Else
				synthesizedImplements = Nothing
			End If
			Return synthesizedImplements
		End Function

		Friend Overrides Function GetSynthesizedNestedTypes() As IEnumerable(Of INestedTypeDefinition)
			Dim nestedTypeDefinitions As IEnumerable(Of INestedTypeDefinition)
			If (Me._comClassData IsNot Nothing) Then
				Dim synthesizedInterfaces As ImmutableArray(Of NamedTypeSymbol) = Me._comClassData.GetSynthesizedInterfaces()
				If (Not synthesizedInterfaces.IsEmpty) Then
					nestedTypeDefinitions = DirectCast(synthesizedInterfaces, IEnumerable(Of NamedTypeSymbol)).AsEnumerable()
				Else
					nestedTypeDefinitions = Nothing
				End If
			Else
				nestedTypeDefinitions = Nothing
			End If
			Return nestedTypeDefinitions
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Dim values As IEnumerable(Of PropertySymbol)
			Me.EnsureAllHandlesAreBound()
			Dim propertySymbols As ConcurrentDictionary(Of PropertySymbol, SynthesizedOverridingWithEventsProperty) = Me._lazyWithEventsOverrides
			If (propertySymbols Is Nothing) Then
				values = SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
			Else
				values = propertySymbols.Values
			End If
			Return values
		End Function

		Friend Function GetTypeIdentifierToken(ByVal node As VisualBasicSyntaxNode) As SyntaxToken
			Dim identifier As SyntaxToken
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock) <= CUShort((Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement))) Then
				identifier = DirectCast(node, TypeBlockSyntax).BlockStatement.Identifier
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock) Then
				identifier = DirectCast(node, EnumBlockSyntax).EnumStatement.Identifier
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
				End If
				identifier = DirectCast(node, DelegateStatementSyntax).Identifier
			End If
			Return identifier
		End Function

		Private Shared Function GetTypeParameterListSyntax(ByVal syntax As VisualBasicSyntaxNode) As TypeParameterListSyntax
			Dim typeParameterList As TypeParameterListSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntax.Kind()
			If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				typeParameterList = DirectCast(syntax, TypeBlockSyntax).BlockStatement.TypeParameterList
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				typeParameterList = DirectCast(syntax, DelegateStatementSyntax).TypeParameterList
			Else
				typeParameterList = Nothing
			End If
			Return typeParameterList
		End Function

		Friend Function HasClassInterfaceAttribute() As Boolean
			Return Me.GetAttributes().IndexOfAttribute(Me, AttributeDescription.ClassInterfaceAttribute) > -1
		End Function

		Friend Function HasComSourceInterfacesAttribute() As Boolean
			Return Me.GetAttributes().IndexOfAttribute(Me, AttributeDescription.ComSourceInterfacesAttribute) > -1
		End Function

		Private Function HasDefaultMemberAttribute() As Boolean
			Dim decodedWellKnownAttributeData As CommonTypeWellKnownAttributeData = DirectCast(Me.GetAttributesBag().DecodedWellKnownAttributeData, CommonTypeWellKnownAttributeData)
			If (decodedWellKnownAttributeData Is Nothing) Then
				Return False
			End If
			Return decodedWellKnownAttributeData.HasDefaultMemberAttribute
		End Function

		Friend Function HasGuidAttribute() As Boolean
			Return Me.GetAttributes().IndexOfAttribute(Me, AttributeDescription.GuidAttribute) > -1
		End Function

		Private Function HasInstanceFields() As Boolean
			Dim flag As Boolean
			Dim membersUnordered As ImmutableArray(Of Symbol) = Me.GetMembersUnordered()
			Dim length As Integer = membersUnordered.Length - 1
			Dim num As Integer = 0
			While True
				If (num <= length) Then
					Dim item As Symbol = membersUnordered(num)
					If (Not (Not item.IsShared And item.Kind = SymbolKind.Field)) Then
						num = num + 1
					Else
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

		Private Shared Function HaveSameConstraints(ByVal constraints1 As ImmutableArray(Of TypeParameterConstraint), ByVal constraints2 As ImmutableArray(Of TypeParameterConstraint)) As Boolean
			Dim flag As Boolean
			Dim length As Integer = constraints1.Length
			Dim num As Integer = constraints2.Length
			If (length <> num) Then
				flag = False
			ElseIf (length = 0 AndAlso num = 0) Then
				flag = True
			ElseIf (SourceNamedTypeSymbol.GetConstraintKind(constraints1) = SourceNamedTypeSymbol.GetConstraintKind(constraints2)) Then
				Dim typeSymbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)()
				Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = constraints1.GetEnumerator()
				While enumerator.MoveNext()
					Dim typeConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator.Current.TypeConstraint
					If (typeConstraint Is Nothing) Then
						Continue While
					End If
					typeSymbols.Add(typeConstraint)
				End While
				Dim enumerator1 As ImmutableArray(Of TypeParameterConstraint).Enumerator = constraints2.GetEnumerator()
				While enumerator1.MoveNext()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator1.Current.TypeConstraint
					If (typeSymbol Is Nothing OrElse typeSymbols.Contains(typeSymbol)) Then
						Continue While
					End If
					flag = False
					Return flag
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function InferFromSyntaxIfClassWillHavePublicParameterlessConstructor() As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol::InferFromSyntaxIfClassWillHavePublicParameterlessConstructor()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean InferFromSyntaxIfClassWillHavePublicParameterlessConstructor()
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

		Private Function IsRestrictedBaseClass(ByVal type As Microsoft.CodeAnalysis.SpecialType) As Boolean
			Dim flag As Boolean
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = type
			flag = If(CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_Enum) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Array, True, False)
			Return flag
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim extendedErrorTypeSymbol As NamedTypeSymbol
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim declaredBase As NamedTypeSymbol = Me.GetDeclaredBase(New BasesBeingResolved())
			If (declaredBase IsNot Nothing) Then
				Dim mBaseCycleDiagnosticInfo As DiagnosticInfo = If(Me.m_baseCycleDiagnosticInfo, BaseTypeAnalysis.GetDependenceDiagnosticForBase(Me, declaredBase))
				If (mBaseCycleDiagnosticInfo Is Nothing) Then
					GoTo Label1
				End If
				Dim inheritsLocation As Location = Me.GetInheritsLocation(declaredBase)
				diagnostics.Add(New VBDiagnostic(mBaseCycleDiagnosticInfo, inheritsLocation, False))
				extendedErrorTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.ExtendedErrorTypeSymbol(mBaseCycleDiagnosticInfo, False, Nothing)
				Return extendedErrorTypeSymbol
			End If
		Label1:
			Dim specialType As NamedTypeSymbol = declaredBase
			If (specialType Is Nothing AndAlso Me.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
				Select Case Me.TypeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Class]
						specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
						specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate)
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
						Throw ExceptionUtilities.UnexpectedValue(Me.TypeKind)
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
						specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Enum)
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						specialType = Nothing
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.[Module]
						specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.Struct
						specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_ValueType)
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.Submission
						Me.ReportUseSiteInfoForBaseType(Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object), declaredBase, diagnostics)
						specialType = Nothing
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me.TypeKind)
				End Select
			End If
			If (specialType IsNot Nothing) Then
				Me.ReportUseSiteInfoForBaseType(specialType, declaredBase, diagnostics)
			End If
			extendedErrorTypeSymbol = specialType
			Return extendedErrorTypeSymbol
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim dependenceDiagnosticForBase As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = Me.GetDeclaredInterfacesNoUseSiteDiagnostics(New BasesBeingResolved())
			Dim flag As Boolean = Me.IsInterfaceType()
			If (flag) Then
				instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).GetInstance()
			Else
				instance = Nothing
			End If
			Dim namedTypeSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = instance
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
				If (Not flag OrElse current.IsErrorType()) Then
					dependenceDiagnosticForBase = Nothing
				Else
					dependenceDiagnosticForBase = BaseTypeAnalysis.GetDependenceDiagnosticForBase(Me, current)
				End If
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = dependenceDiagnosticForBase
				If (diagnosticInfo Is Nothing) Then
					If (Not current.IsErrorType()) Then
						Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, Me.ContainingAssembly)
						If (current.DeclaringCompilation <> Me.DeclaringCompilation) Then
							current.AddUseSiteInfo(compoundUseSiteInfo)
							Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator()
							While enumerator1.MoveNext()
								Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
								If (namedTypeSymbol.DeclaringCompilation = Me.DeclaringCompilation) Then
									Continue While
								End If
								namedTypeSymbol.AddUseSiteInfo(compoundUseSiteInfo)
							End While
						End If
						If (compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
							diagnostics.AddDependencies(compoundUseSiteInfo)
						Else
							diagnostics.Add(If(flag, Me.GetInheritsLocation(current), Me.GetInheritsOrImplementsLocation(current, False)), compoundUseSiteInfo)
						End If
					End If
					If (Not flag) Then
						Continue While
					End If
					namedTypeSymbols.Add(current)
				Else
					Dim inheritsLocation As Location = Me.GetInheritsLocation(current)
					diagnostics.Add(New VBDiagnostic(diagnosticInfo, inheritsLocation, False))
					namedTypeSymbols.Add(New ExtendedErrorTypeSymbol(diagnosticInfo, False, Nothing))
				End If
			End While
			If (Not flag) Then
				Return declaredInterfacesNoUseSiteDiagnostics
			End If
			Return namedTypeSymbols.ToImmutableAndFree()
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim containingSymbol As SourceNamedTypeSymbol = TryCast(Me.ContainingSymbol, SourceNamedTypeSymbol)
			If (containingSymbol IsNot Nothing) Then
				containingSymbol.GetDeclaredBaseSafe(basesBeingResolved.PrependInheritsBeingResolved(Me))
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = MyBase.TypeDeclaration.Declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SingleTypeDeclaration = enumerator.Current
				If (Not current.HasBaseDeclarations) Then
					Continue While
				End If
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = current.SyntaxReference
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = syntaxReference.SyntaxTree
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Me.MakeDeclaredBaseInPart(syntaxTree, syntaxReference.GetVisualBasicSyntax(cancellationToken), namedTypeSymbol, basesBeingResolved, diagnostics)
			End While
			Return namedTypeSymbol
		End Function

		Private Sub MakeDeclaredBaseInPart(ByVal tree As SyntaxTree, ByVal syntaxNode As VisualBasicSyntaxNode, ByRef baseType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateLocationSpecificBinderForType(tree, BindingLocation.BaseTypes)
			Select Case syntaxNode.Kind()
				Case SyntaxKind.ModuleBlock
					Me.CheckNoBase(Of InheritsStatementSyntax)(DirectCast(syntaxNode, TypeBlockSyntax).[Inherits], ERRID.ERR_ModuleCantInherit, diagBag)
					Return
				Case SyntaxKind.StructureBlock
					Me.CheckNoBase(Of InheritsStatementSyntax)(DirectCast(syntaxNode, TypeBlockSyntax).[Inherits], ERRID.ERR_StructCantInherit, diagBag)
					Return
				Case SyntaxKind.InterfaceBlock
					Return
				Case SyntaxKind.ClassBlock
					Dim [inherits] As SyntaxList(Of InheritsStatementSyntax) = DirectCast(syntaxNode, TypeBlockSyntax).[Inherits]
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.ValidateClassBase([inherits], baseType, basesBeingResolved, binder, diagBag)
					If (baseType IsNot Nothing) Then
						Return
					End If
					baseType = namedTypeSymbol
					Return
				Case Else
					Return
			End Select
		End Sub

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim containingSymbol As SourceNamedTypeSymbol = TryCast(Me.ContainingSymbol, SourceNamedTypeSymbol)
			If (Me.IsInterface AndAlso containingSymbol IsNot Nothing AndAlso containingSymbol.IsInterface) Then
				containingSymbol.GetDeclaredBaseInterfacesSafe(basesBeingResolved.PrependInheritsBeingResolved(Me))
			End If
			Dim namedTypeSymbols As SetWithInsertionOrder(Of NamedTypeSymbol) = New SetWithInsertionOrder(Of NamedTypeSymbol)()
			Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = MyBase.SyntaxReferences.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxReference = enumerator.Current
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = current.SyntaxTree
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Me.MakeDeclaredInterfacesInPart(syntaxTree, current.GetVisualBasicSyntax(cancellationToken), namedTypeSymbols, basesBeingResolved, diagnostics)
			End While
			Return namedTypeSymbols.AsImmutable()
		End Function

		Private Sub MakeDeclaredInterfacesInPart(ByVal tree As SyntaxTree, ByVal syntaxNode As VisualBasicSyntaxNode, ByVal interfaces As SetWithInsertionOrder(Of NamedTypeSymbol), ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateLocationSpecificBinderForType(tree, BindingLocation.BaseTypes)
			Select Case syntaxNode.Kind()
				Case SyntaxKind.ModuleBlock
					Me.CheckNoBase(Of ImplementsStatementSyntax)(DirectCast(syntaxNode, TypeBlockSyntax).[Implements], ERRID.ERR_ModuleCantImplement, diagBag)
					Return
				Case SyntaxKind.StructureBlock
					Dim [implements] As SyntaxList(Of ImplementsStatementSyntax) = DirectCast(syntaxNode, TypeBlockSyntax).[Implements]
					Me.ValidateImplementedInterfaces([implements], interfaces, basesBeingResolved, binder, diagBag)
					Return
				Case SyntaxKind.InterfaceBlock
					Dim [inherits] As SyntaxList(Of InheritsStatementSyntax) = DirectCast(syntaxNode, TypeBlockSyntax).[Inherits]
					Me.ValidateInheritedInterfaces([inherits], interfaces, basesBeingResolved, binder, diagBag)
					Return
				Case SyntaxKind.ClassBlock
					Dim implementsStatementSyntaxes As SyntaxList(Of ImplementsStatementSyntax) = DirectCast(syntaxNode, TypeBlockSyntax).[Implements]
					Me.ValidateImplementedInterfaces(implementsStatementSyntaxes, interfaces, basesBeingResolved, binder, diagBag)
					Return
				Case Else
					Return
			End Select
		End Sub

		Private Function MakeTypeParameters() As ImmutableArray(Of TypeParameterSymbol)
			Dim empty As ImmutableArray(Of TypeParameterSymbol)
			Dim arity As Integer = MyBase.TypeDeclaration.Arity
			If (arity <> 0) Then
				Dim sourceTypeParameterOnTypeSymbol(arity - 1 + 1 - 1) As TypeParameterSymbol
				Dim num As Integer = arity - 1
				Dim num1 As Integer = 0
				Do
					Dim instance As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
					Dim valueText As String = Nothing
					Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = MyBase.SyntaxReferences.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As SyntaxReference = enumerator.Current
						Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = current.SyntaxTree
						Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
						Dim parameters As SeparatedSyntaxList(Of TypeParameterSyntax) = SourceNamedTypeSymbol.GetTypeParameterListSyntax(current.GetVisualBasicSyntax(cancellationToken)).Parameters
						Dim item As TypeParameterSyntax = parameters(num1)
						If (valueText Is Nothing) Then
							valueText = item.Identifier.ValueText
						End If
						instance.Add(syntaxTree.GetReference(item))
					End While
					sourceTypeParameterOnTypeSymbol(num1) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceTypeParameterOnTypeSymbol(Me, num1, valueText, instance.ToImmutableAndFree())
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(sourceTypeParameterOnTypeSymbol)
			Else
				empty = ImmutableArray(Of TypeParameterSymbol).Empty
			End If
			Return empty
		End Function

		Private Shared Function MyGroupCollectionCandidateHasPublicParameterlessConstructor(ByVal candidate As SourceNamedTypeSymbol) As Boolean
			Dim flag As Boolean
			flag = If(Not candidate.MembersHaveBeenCreated, candidate.InferFromSyntaxIfClassWillHavePublicParameterlessConstructor(), ConstraintsHelper.HasPublicParameterlessConstructor(candidate))
			Return flag
		End Function

		Private Sub PerformComClassAnalysis()
			If (Me._comClassData IsNot Nothing) Then
				Me._comClassData.PerformComClassAnalysis(Me)
			End If
		End Sub

		Friend Overrides Sub PostDecodeWellKnownAttributes(ByVal boundAttributes As ImmutableArray(Of VisualBasicAttributeData), ByVal allAttributeSyntaxNodes As ImmutableArray(Of AttributeSyntax), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal symbolPart As AttributeLocation, ByVal decodedData As WellKnownAttributeData)
			Me.ValidateStandardModuleAttribute(diagnostics)
			MyBase.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData)
		End Sub

		Private Sub ReportUseSiteInfoForBaseType(ByVal baseType As NamedTypeSymbol, ByVal declaredBase As NamedTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim inheritsLocation As Location
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, Me.ContainingAssembly)
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = baseType
			Do
				If (baseTypeNoUseSiteDiagnostics.DeclaringCompilation = Me.DeclaringCompilation) Then
					Exit Do
				End If
				baseTypeNoUseSiteDiagnostics.AddUseSiteInfo(compoundUseSiteInfo)
				baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
			Loop While baseTypeNoUseSiteDiagnostics IsNot Nothing
			If (compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
				diagnostics.AddDependencies(compoundUseSiteInfo)
				Return
			End If
			If (CObj(declaredBase) <> CObj(baseType)) Then
				Dim visualBasicSyntax As VisualBasicSyntaxNode = MyBase.SyntaxReferences.First().GetVisualBasicSyntax(New CancellationToken())
				inheritsLocation = If(visualBasicSyntax.Kind() = SyntaxKind.CompilationUnit OrElse visualBasicSyntax.Kind() = SyntaxKind.NamespaceBlock, MyBase.Locations(0), Me.GetTypeIdentifierToken(visualBasicSyntax).GetLocation())
			Else
				inheritsLocation = Me.GetInheritsLocation(baseType)
			End If
			diagnostics.Add(inheritsLocation, compoundUseSiteInfo)
		End Sub

		Private Function ShadowsTypeParameter(ByVal typeParameter As TypeParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim containingType As SourceNamedTypeSymbol
			Dim name As String = typeParameter.Name
			containingType = If(typeParameter.TypeParameterKind <> TypeParameterKind.Method, TryCast(Me.ContainingType, SourceNamedTypeSymbol), Me)
			While True
				If (containingType Is Nothing) Then
					flag = False
					Exit While
				ElseIf (Not containingType.TypeParameters.MatchesAnyName(name)) Then
					containingType = TryCast(containingType.ContainingType, SourceNamedTypeSymbol)
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function ValidateClassBase(ByVal inheritsSyntax As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax), ByVal baseInOtherPartial As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (inheritsSyntax.Count <> 0) Then
				basesBeingResolved = basesBeingResolved.PrependInheritsBeingResolved(Me)
				binder = New BasesBeingResolvedBinder(binder, basesBeingResolved)
				Dim item As TypeSyntax = Nothing
				Dim enumerator As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax).Enumerator = inheritsSyntax.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax = enumerator.Current
					If (current.Kind() <> SyntaxKind.InheritsStatement) Then
						Continue While
					End If
					Dim inheritsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax = current
					If (item IsNot Nothing OrElse inheritsStatementSyntax.Types.Count > 1) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, inheritsStatementSyntax, ERRID.ERR_MultipleExtends)
					End If
					If (item IsNot Nothing OrElse inheritsStatementSyntax.Types.Count <= 0) Then
						Continue While
					End If
					item = inheritsStatementSyntax.Types(0)
				End While
				If (item IsNot Nothing) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.BindTypeSyntax(item, diagBag, True, False, True)
					If (typeSymbol IsNot Nothing) Then
						Select Case typeSymbol.TypeKind
							Case Microsoft.CodeAnalysis.TypeKind.Unknown
							Case Microsoft.CodeAnalysis.TypeKind.[Error]
								namedTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
								Exit Select
							Case Microsoft.CodeAnalysis.TypeKind.Array
							Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
							Case Microsoft.CodeAnalysis.TypeKind.[Enum]
							Case Microsoft.CodeAnalysis.TypeKind.[Interface]
							Case Microsoft.CodeAnalysis.TypeKind.[Module]
							Case Microsoft.CodeAnalysis.TypeKind.Struct
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_InheritsFromNonClass)
								namedTypeSymbol = Nothing
								Exit Select
							Case Microsoft.CodeAnalysis.TypeKind.[Class]
								If (Not Me.IsRestrictedBaseClass(typeSymbol.SpecialType)) Then
									If (Not DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).IsNotInheritable) Then
										GoTo Label0
									End If
									Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_InheritsFromCantInherit3, New [Object]() { MyBase.Name, typeSymbol.Name, typeSymbol.GetKindText() })
									namedTypeSymbol = Nothing
									Exit Select
								Else
									Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_InheritsFromRestrictedType1, New [Object]() { typeSymbol })
									namedTypeSymbol = Nothing
									Exit Select
								End If
							Case Microsoft.CodeAnalysis.TypeKind.Dynamic
							Case Microsoft.CodeAnalysis.TypeKind.Pointer
								If (baseInOtherPartial IsNot Nothing) Then
									If (Not typeSymbol.Equals(baseInOtherPartial)) Then
										Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_BaseMismatchForPartialClass3, New [Object]() { typeSymbol, MyBase.Name, baseInOtherPartial })
										namedTypeSymbol = Nothing
										Exit Select
									End If
								ElseIf (Not typeSymbol.IsErrorType()) Then
									AccessCheck.VerifyAccessExposureOfBaseClassOrInterface(Me, item, typeSymbol, diagBag)
								End If
								namedTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
								Exit Select
							Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_GenericParamBase2, New [Object]() { "Class", MyBase.Name })
								namedTypeSymbol = Nothing
								Exit Select
							Case Else
								GoTo Label0
						End Select
					Else
						namedTypeSymbol = Nothing
					End If
				Else
					namedTypeSymbol = Nothing
				End If
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function

		Private Sub ValidateImplementedInterfaces(ByVal baseSyntax As SyntaxList(Of ImplementsStatementSyntax), ByVal basesInOtherPartials As SetWithInsertionOrder(Of NamedTypeSymbol), ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (baseSyntax.Count <> 0) Then
				basesBeingResolved = basesBeingResolved.PrependImplementsBeingResolved(Me)
				binder = New BasesBeingResolvedBinder(binder, basesBeingResolved)
				Dim typeSymbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)()
				Dim enumerator As SyntaxList(Of ImplementsStatementSyntax).Enumerator = baseSyntax.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As SeparatedSyntaxList(Of TypeSyntax).Enumerator = enumerator.Current.Types.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As TypeSyntax = enumerator1.Current
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.BindTypeSyntax(current, diagBag, True, False, False)
						If (typeSymbols.Add(typeSymbol)) Then
							Dim typeKind As Microsoft.CodeAnalysis.TypeKind = typeSymbol.TypeKind
							If (typeKind = Microsoft.CodeAnalysis.TypeKind.Unknown) Then
								Continue While
							End If
							If (CByte(typeKind) - CByte(Microsoft.CodeAnalysis.TypeKind.[Error]) <= CByte(Microsoft.CodeAnalysis.TypeKind.Array)) Then
								basesInOtherPartials.Add(DirectCast(typeSymbol, NamedTypeSymbol))
							ElseIf (typeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_BadImplementsType)
							Else
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_ImplementsGenericParam, New [Object]() { "Interface", MyBase.Name })
							End If
						Else
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_InterfaceImplementedTwice1, New [Object]() { typeSymbol })
						End If
					End While
				End While
			End If
		End Sub

		Private Sub ValidateInheritedInterfaces(ByVal baseSyntax As SyntaxList(Of InheritsStatementSyntax), ByVal basesInOtherPartials As SetWithInsertionOrder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (baseSyntax.Count <> 0) Then
				basesBeingResolved = basesBeingResolved.PrependInheritsBeingResolved(Me)
				binder = New BasesBeingResolvedBinder(binder, basesBeingResolved)
				Dim namedTypeSymbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)()
				Dim enumerator As SyntaxList(Of InheritsStatementSyntax).Enumerator = baseSyntax.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As SeparatedSyntaxList(Of TypeSyntax).Enumerator = enumerator.Current.Types.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As TypeSyntax = enumerator1.Current
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.BindTypeSyntax(current, diagBag, True, False, False)
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						If (namedTypeSymbol Is Nothing OrElse Not namedTypeSymbols.Contains(namedTypeSymbol)) Then
							If (namedTypeSymbol IsNot Nothing) Then
								namedTypeSymbols.Add(namedTypeSymbol)
							End If
							Dim typeKind As Microsoft.CodeAnalysis.TypeKind = typeSymbol.TypeKind
							If (typeKind = Microsoft.CodeAnalysis.TypeKind.Unknown) Then
								Continue While
							End If
							If (CByte(typeKind) - CByte(Microsoft.CodeAnalysis.TypeKind.[Error]) <= CByte(Microsoft.CodeAnalysis.TypeKind.Array)) Then
								basesInOtherPartials.Add(namedTypeSymbol)
								If (typeSymbol.IsErrorType()) Then
									Continue While
								End If
								AccessCheck.VerifyAccessExposureOfBaseClassOrInterface(Me, current, typeSymbol, diagBag)
							ElseIf (typeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_InheritsFromNonInterface)
							Else
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_GenericParamBase2, New [Object]() { "Interface", MyBase.Name })
							End If
						Else
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateInInherits1, New [Object]() { typeSymbol })
						End If
					End While
				End While
			End If
		End Sub

		Private Sub ValidateStandardModuleAttribute(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
				Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
				Dim locations As ImmutableArray(Of Location) = MyBase.Locations
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute__ctor, declaringCompilation, locations(0), diagnostics)
			End If
		End Sub

		Private Class ComClassData
			Public ReadOnly ClassId As String

			Public ReadOnly InterfaceId As String

			Public ReadOnly EventId As String

			Public ReadOnly InterfaceShadows As Boolean

			Private _syntheticInterfaces As ImmutableArray(Of NamedTypeSymbol)

			Public Sub New(ByVal attrData As VisualBasicAttributeData)
				MyBase.New()
				Dim item As TypedConstant
				Dim valueInternal As String
				Dim str As String
				Dim valueInternal1 As String
				Dim commonConstructorArguments As ImmutableArray(Of TypedConstant) = attrData.CommonConstructorArguments
				If (commonConstructorArguments.Length > 0) Then
					If (commonConstructorArguments(0).Kind <> TypedConstantKind.Array) Then
						item = commonConstructorArguments(0)
						valueInternal = TryCast(item.ValueInternal, [String])
					Else
						valueInternal = Nothing
					End If
					Dim str1 As String = valueInternal
					If (Not [String].IsNullOrEmpty(str1)) Then
						Me.ClassId = str1
					End If
					If (commonConstructorArguments.Length > 1) Then
						If (commonConstructorArguments(1).Kind <> TypedConstantKind.Array) Then
							item = commonConstructorArguments(1)
							str = TryCast(item.ValueInternal, [String])
						Else
							str = Nothing
						End If
						str1 = str
						If (Not [String].IsNullOrEmpty(str1)) Then
							Me.InterfaceId = str1
						End If
						If (commonConstructorArguments.Length > 2) Then
							If (commonConstructorArguments(2).Kind <> TypedConstantKind.Array) Then
								item = commonConstructorArguments(2)
								valueInternal1 = TryCast(item.ValueInternal, [String])
							Else
								valueInternal1 = Nothing
							End If
							str1 = valueInternal1
							If (Not [String].IsNullOrEmpty(str1)) Then
								Me.EventId = str1
							End If
						End If
					End If
				End If
				Me.InterfaceShadows = attrData.DecodeNamedArgument(Of Boolean)("InterfaceShadows", Microsoft.CodeAnalysis.SpecialType.System_Boolean, False)
			End Sub

			Private Sub CheckForNameCollisions(ByVal comClass As SourceNamedTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				Dim num As Integer = 0
				Do
					Dim str As String = [String].Concat(If(num = 0, "_", "__"), comClass.Name)
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = comClass.GetMembers(str).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Symbol = enumerator.Current
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, current.Locations(0), ERRID.ERR_MemberConflictWithSynth4, New [Object]() { [String].Concat(SyntaxFacts.GetText(SyntaxKind.InterfaceKeyword), " ", str), AttributeDescription.VisualBasicComClassAttribute.Name, SyntaxFacts.GetText(SyntaxKind.ClassKeyword), comClass.Name })
					End While
					If (Not Me.InterfaceShadows) Then
						Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = comClass.BaseTypeNoUseSiteDiagnostics
						While baseTypeNoUseSiteDiagnostics IsNot Nothing
							Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = baseTypeNoUseSiteDiagnostics.GetMembers(str).GetEnumerator()
							While enumerator1.MoveNext()
								If (enumerator1.Current.DeclaredAccessibility = Accessibility.[Private]) Then
									Continue While
								End If
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, comClass.Locations(0), ERRID.WRN_ComClassInterfaceShadows5, New [Object]() { comClass.Name, SyntaxFacts.GetText(SyntaxKind.InterfaceKeyword), str, SyntaxFacts.GetText(SyntaxKind.ClassKeyword), baseTypeNoUseSiteDiagnostics })
							End While
							baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
						End While
					End If
					num = num + 1
				Loop While num <= 1
			End Sub

			Private Sub GetComClassMembers(ByVal comClass As SourceNamedTypeSymbol, ByVal interfaceMembers As ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)), ByVal eventMembers As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)), <Out> ByRef haveDefaultProperty As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				haveDefaultProperty = False
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = comClass.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.IsShared OrElse current.DeclaredAccessibility <> Accessibility.[Public] OrElse current.IsImplicitlyDeclared) Then
						Continue While
					End If
					Dim kind As SymbolKind = current.Kind
					If (kind = SymbolKind.Field) Then
						Continue While
					End If
					If (kind <> SymbolKind.Method) Then
						If (kind = SymbolKind.NamedType) Then
							Continue While
						End If
					ElseIf (DirectCast(current, MethodSymbol).MethodKind <> MethodKind.Ordinary) Then
						Continue While
					End If
					If (Not SourceNamedTypeSymbol.ComClassData.GetComVisibleState(current)) Then
						Continue While
					End If
					If (kind = SymbolKind.[Event]) Then
						eventMembers.Add(New KeyValuePair(Of EventSymbol, Integer)(DirectCast(current, EventSymbol), SourceNamedTypeSymbol.ComClassData.GetUserSpecifiedDispId(current, diagnostics)))
					ElseIf (kind = SymbolKind.Method) Then
						If (DirectCast(current, MethodSymbol).IsGenericMethod) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, current.Locations(0), ERRID.ERR_ComClassGenericMethod)
						End If
						interfaceMembers.Add(New KeyValuePair(Of Symbol, Integer)(current, SourceNamedTypeSymbol.ComClassData.GetUserSpecifiedDispId(current, diagnostics)))
					Else
						If (kind <> SymbolKind.[Property]) Then
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
						Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
						If (propertySymbol.IsWithEvents) Then
							Continue While
						End If
						Dim getMethod As MethodSymbol = propertySymbol.GetMethod
						Dim setMethod As MethodSymbol = propertySymbol.SetMethod
						If (getMethod IsNot Nothing) Then
							If (getMethod.IsImplicitlyDeclared) Then
								Continue While
							End If
							If (getMethod.DeclaredAccessibility <> Accessibility.[Public] OrElse Not SourceNamedTypeSymbol.ComClassData.GetComVisibleState(getMethod)) Then
								getMethod = Nothing
							End If
						End If
						If (setMethod IsNot Nothing) Then
							If (setMethod.IsImplicitlyDeclared) Then
								Continue While
							End If
							If (setMethod.DeclaredAccessibility <> Accessibility.[Public] OrElse Not SourceNamedTypeSymbol.ComClassData.GetComVisibleState(setMethod)) Then
								setMethod = Nothing
							End If
						End If
						If (getMethod Is Nothing AndAlso setMethod Is Nothing) Then
							Continue While
						End If
						If (propertySymbol.Type.IsObjectType() AndAlso propertySymbol.SetMethod IsNot Nothing) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, propertySymbol.Locations(0), ERRID.WRN_ComClassPropertySetObject1, New [Object]() { propertySymbol })
						End If
						interfaceMembers.Add(New KeyValuePair(Of Symbol, Integer)(propertySymbol, SourceNamedTypeSymbol.ComClassData.GetUserSpecifiedDispId(propertySymbol, diagnostics)))
						If (propertySymbol.IsDefault) Then
							haveDefaultProperty = True
						End If
						interfaceMembers.Add(New KeyValuePair(Of Symbol, Integer)(getMethod, If(getMethod Is Nothing, -1, SourceNamedTypeSymbol.ComClassData.GetUserSpecifiedDispId(getMethod, diagnostics))))
						interfaceMembers.Add(New KeyValuePair(Of Symbol, Integer)(setMethod, If(setMethod Is Nothing, -1, SourceNamedTypeSymbol.ComClassData.GetUserSpecifiedDispId(setMethod, diagnostics))))
					End If
				End While
			End Sub

			Private Shared Function GetComVisibleState(ByVal target As Symbol) As Boolean
				Dim flag As Boolean
				Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = target.GetAttributes()
				Dim num As Integer = attributes.IndexOfAttribute(target, AttributeDescription.ComVisibleAttribute)
				If (num <= -1) Then
					flag = True
				Else
					Dim item As TypedConstant = attributes(num).CommonConstructorArguments(0)
					Dim objectValue As Object = RuntimeHelpers.GetObjectValue(If(item.Kind <> TypedConstantKind.Array, item.ValueInternal, Nothing))
					flag = If(objectValue Is Nothing OrElse TypeOf objectValue Is [Boolean] AndAlso Not CBool(objectValue), False, True)
				End If
				Return flag
			End Function

			Public Function GetCorrespondingComClassInterfaceMethod(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				If (Not Me._syntheticInterfaces.IsEmpty) Then
					Dim members As ImmutableArray(Of Symbol) = Me._syntheticInterfaces(0).GetMembers()
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Symbol = enumerator.Current
						If (current.Kind <> SymbolKind.Method) Then
							Continue While
						End If
						Dim synthesizedComMethod As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod = DirectCast(current, SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod)
						If (CObj(synthesizedComMethod.ClonedFrom) <> CObj(method)) Then
							Continue While
						End If
						methodSymbol = synthesizedComMethod
						Return methodSymbol
					End While
					methodSymbol = Nothing
				Else
					methodSymbol = Nothing
				End If
				Return methodSymbol
			End Function

			Public Function GetSynthesizedEventInterface() As NamedTypeSymbol
				Dim item As NamedTypeSymbol
				If (Me._syntheticInterfaces.Length <= 1) Then
					item = Nothing
				Else
					item = Me._syntheticInterfaces(1)
				End If
				Return item
			End Function

			Public Function GetSynthesizedImplements() As IEnumerable(Of NamedTypeSymbol)
				Dim namedTypeSymbols As IEnumerable(Of NamedTypeSymbol)
				If (Not Me._syntheticInterfaces.IsEmpty) Then
					namedTypeSymbols = SpecializedCollections.SingletonEnumerable(Of NamedTypeSymbol)(Me._syntheticInterfaces(0))
				Else
					namedTypeSymbols = Nothing
				End If
				Return namedTypeSymbols
			End Function

			Public Function GetSynthesizedInterfaces() As ImmutableArray(Of NamedTypeSymbol)
				Return Me._syntheticInterfaces
			End Function

			Private Shared Function GetUserSpecifiedDispId(ByVal target As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Integer
				Dim num As Integer
				Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = target.GetAttributes()
				Dim num1 As Integer = attributes.IndexOfAttribute(target, AttributeDescription.DispIdAttribute)
				If (num1 > -1) Then
					Dim item As TypedConstant = attributes(num1).CommonConstructorArguments(0)
					Dim objectValue As Object = RuntimeHelpers.GetObjectValue(If(item.Kind <> TypedConstantKind.Array, item.ValueInternal, Nothing))
					If (objectValue Is Nothing OrElse Not TypeOf objectValue Is Int32) Then
						num = -1
						Return num
					End If
					Dim num2 As Integer = CInt(objectValue)
					If (num2 = 0) Then
						If (target.Kind <> SymbolKind.[Property] OrElse Not DirectCast(target, PropertySymbol).IsDefault) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, target.Locations(0), ERRID.ERR_ComClassReservedDispIdZero1, New [Object]() { target.Name })
						End If
					ElseIf (num2 < 0) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, target.Locations(0), ERRID.ERR_ComClassReservedDispId1, New [Object]() { target.Name })
					End If
					num = num2
					Return num
				End If
				num = -1
				Return num
			End Function

			Public Sub PerformComClassAnalysis(ByVal comClass As SourceNamedTypeSymbol)
				Dim guid As System.Guid
				Dim guid1 As System.Guid
				Dim flag As Boolean
				Dim locations As ImmutableArray(Of Location)
				Dim current As KeyValuePair(Of EventSymbol, Integer)
				If (Me._syntheticInterfaces.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim empty As ImmutableArray(Of NamedTypeSymbol) = ImmutableArray(Of NamedTypeSymbol).Empty
					Dim keyValuePairs As ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)) = ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)).GetInstance()
					Dim instance1 As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)) = ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)).GetInstance()
					Dim classId As String = Me.ClassId
					Dim guid2 As System.Guid = New System.Guid()
					SourceNamedTypeSymbol.ComClassData.ValidateComClassGuid(comClass, classId, instance, guid2)
					If (SourceNamedTypeSymbol.ComClassData.ValidateComClassGuid(comClass, Me.InterfaceId, instance, guid) And SourceNamedTypeSymbol.ComClassData.ValidateComClassGuid(comClass, Me.EventId, instance, guid1) AndAlso Me.InterfaceId IsNot Nothing AndAlso Me.EventId IsNot Nothing AndAlso guid = guid1) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassDuplicateGuids1, New [Object]() { comClass.Name })
					End If
					If (comClass.HasGuidAttribute()) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassAndReservedAttribute1, New [Object]() { AttributeDescription.GuidAttribute.Name })
					End If
					If (comClass.HasClassInterfaceAttribute()) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassAndReservedAttribute1, New [Object]() { AttributeDescription.ClassInterfaceAttribute.Name })
					End If
					If (comClass.HasComSourceInterfacesAttribute()) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassAndReservedAttribute1, New [Object]() { AttributeDescription.ComSourceInterfacesAttribute.Name })
					End If
					If (Not SourceNamedTypeSymbol.ComClassData.GetComVisibleState(comClass)) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassAndReservedAttribute1, New [Object]() { [String].Concat(AttributeDescription.ComVisibleAttribute.Name, "(False)") })
					End If
					If (comClass.DeclaredAccessibility = Accessibility.[Public]) Then
						Dim containingType As NamedTypeSymbol = comClass.ContainingType
						While containingType IsNot Nothing
							If (containingType.DeclaredAccessibility = Accessibility.[Public]) Then
								containingType = containingType.ContainingType
							Else
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassRequiresPublicClass2, New [Object]() { comClass.Name, containingType.Name })
								Exit While
							End If
						End While
					Else
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.ERR_ComClassRequiresPublicClass1, New [Object]() { comClass.Name })
					End If
					If (comClass.IsMustInherit) Then
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, locations(0), ERRID.ERR_ComClassCantBeAbstract0)
					End If
					Me.CheckForNameCollisions(comClass, instance)
					Me.GetComClassMembers(comClass, keyValuePairs, instance1, flag, instance)
					If (keyValuePairs.Count = 0 AndAlso instance1.Count = 0) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(instance, comClass.Locations(0), ERRID.WRN_ComClassNoMembers1, New [Object]() { comClass.Name })
					ElseIf (Not instance.HasAnyErrors()) Then
						Dim synthesizedComInterface As NamedTypeSymbol = New SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface(comClass, keyValuePairs)
						empty = If(instance1.Count <> 0, ImmutableArray.Create(Of NamedTypeSymbol)(synthesizedComInterface, New SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface(comClass, instance1)), ImmutableArray.Create(Of NamedTypeSymbol)(synthesizedComInterface))
					End If
					If (Me.ClassId IsNot Nothing OrElse Me.InterfaceId IsNot Nothing AndAlso empty.Length > 0 OrElse Me.EventId IsNot Nothing AndAlso empty.Length > 1) Then
						Dim declaringCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, declaringCompilation, locations(0), instance)
					End If
					Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
					locations = comClass.Locations
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType, visualBasicCompilation, locations(0), instance)
					If (empty.Length > 1) Then
						Dim declaringCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString, declaringCompilation1, locations(0), instance)
						Dim visualBasicCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16, visualBasicCompilation1, locations(0), instance)
					End If
					If (empty.Length > 0) Then
						Dim declaringCompilation2 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ComVisibleAttribute__ctor, declaringCompilation2, locations(0), instance)
					End If
					Dim flag1 As Boolean = False
					Dim enumerator As ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)).Enumerator = keyValuePairs.GetEnumerator()
					While enumerator.MoveNext()
						Dim keyValuePair As KeyValuePair(Of Symbol, Integer) = enumerator.Current
						If (keyValuePair.Key Is Nothing OrElse keyValuePair.Value <> -1) Then
							Continue While
						End If
						flag1 = True
						Exit While
					End While
					If (Not flag1) Then
						Dim enumerator1 As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)).Enumerator = instance1.GetEnumerator()
						Do
							If (Not enumerator1.MoveNext()) Then
								GoTo Label0
							End If
							current = enumerator1.Current
						Loop While current.Value <> -1
						flag1 = True
					End If
				Label0:
					If (flag1) Then
						Dim visualBasicCompilation2 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, visualBasicCompilation2, locations(0), instance)
					End If
					If (flag) Then
						Dim declaringCompilation3 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = comClass.DeclaringCompilation
						locations = comClass.Locations
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, declaringCompilation3, locations(0), instance)
					End If
					keyValuePairs.Free()
					instance1.Free()
					comClass.ContainingSourceModule.AtomicStoreArrayAndDiagnostics(Of NamedTypeSymbol)(Me._syntheticInterfaces, empty, instance)
					instance.Free()
				End If
			End Sub

			Private Shared Function ValidateComClassGuid(ByVal comClass As SourceNamedTypeSymbol, ByVal id As String, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> Optional ByRef guidVal As Guid = Nothing) As Boolean
				Dim flag As Boolean
				If (id Is Nothing) Then
					guidVal = New Guid()
				Else
					If (Guid.TryParseExact(id, "D", guidVal)) Then
						flag = True
						Return flag
					End If
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, comClass.Locations(0), ERRID.ERR_BadAttributeUuid2, New [Object]() { AttributeDescription.VisualBasicComClassAttribute.Name, id })
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End Function

			Private Enum ReservedDispId
				DISPID_NEWENUM = -4
				None = -1
				DISPID_VALUE = 0
			End Enum

			Private Class SynthesizedComEventMethod
				Inherits SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod
				Private ReadOnly _event As EventSymbol

				Friend Overrides ReadOnly Property HasSpecialName As Boolean
					Get
						Return Me._event.HasSpecialName
					End Get
				End Property

				Protected Overrides ReadOnly Property NameAndAttributesSource As Symbol
					Get
						Return Me._event
					End Get
				End Property

				Public Sub New(ByVal container As SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface, ByVal [event] As EventSymbol, ByVal clone As MethodSymbol, ByVal synthesizedDispId As Integer)
					MyBase.New(container, clone, synthesizedDispId)
					Me._event = [event]
				End Sub
			End Class

			Private NotInheritable Class SynthesizedComInterface
				Inherits NamedTypeSymbol
				Private ReadOnly _comClass As SourceNamedTypeSymbol

				Private ReadOnly _isEventInterface As Boolean

				Private ReadOnly _members As ImmutableArray(Of Symbol)

				Private ReadOnly _defaultMemberName As String

				Public Overrides ReadOnly Property Arity As Integer
					Get
						Return 0
					End Get
				End Property

				Friend Overrides ReadOnly Property CanConstruct As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property CoClassType As TypeSymbol
					Get
						Return Nothing
					End Get
				End Property

				Public ReadOnly Property ComClass As SourceNamedTypeSymbol
					Get
						Return Me._comClass
					End Get
				End Property

				Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
					Get
						Return Me
					End Get
				End Property

				Public Overrides ReadOnly Property ContainingSymbol As Symbol
					Get
						Return Me._comClass
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
					Get
						Return Accessibility.[Public]
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property DefaultPropertyName As String
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property HasSpecialName As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property IsComImport As Boolean
					Get
						Return False
					End Get
				End Property

				Public ReadOnly Property IsEventInterface As Boolean
					Get
						Return Me._isEventInterface
					End Get
				End Property

				Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property IsInterface As Boolean
					Get
						Return True
					End Get
				End Property

				Public Overrides ReadOnly Property IsMustInherit As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsNotInheritable As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsSerializable As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property Layout As TypeLayout
					Get
						Return New TypeLayout()
					End Get
				End Property

				Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property MangleName As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
					Get
						Return MyBase.DefaultMarshallingCharSet
					End Get
				End Property

				Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property Name As String
					Get
						Return [String].Concat(If(Me._isEventInterface, "__", "_"), Me._comClass.Name)
					End Get
				End Property

				Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
					Get
						Return Nothing
					End Get
				End Property

				Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
					Get
						Return ImmutableArray(Of TypeSymbol).Empty
					End Get
				End Property

				Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
					Get
						Return Microsoft.CodeAnalysis.TypeKind.[Interface]
					End Get
				End Property

				Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
					Get
						Return ImmutableArray(Of TypeParameterSymbol).Empty
					End Get
				End Property

				Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
					Get
						Return Nothing
					End Get
				End Property

				Public Sub New(ByVal comClass As SourceNamedTypeSymbol, ByVal interfaceMembers As ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)))
					MyBase.New()
					Dim nextAvailableDispId As Integer
					Me._comClass = comClass
					Me._isEventInterface = False
					Dim nums As HashSet(Of Integer) = New HashSet(Of Integer)()
					Dim enumerator As ArrayBuilder(Of KeyValuePair(Of Symbol, Integer)).Enumerator = interfaceMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of Symbol, Integer) = enumerator.Current
						If (current.Value = -1) Then
							Continue While
						End If
						nums.Add(current.Value)
					End While
					Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
					Dim num As Integer = 1
					Dim count As Integer = interfaceMembers.Count - 1
					Dim num1 As Integer = 0
					Do
						Dim item As KeyValuePair(Of Symbol, Integer) = interfaceMembers(num1)
						Dim key As Symbol = item.Key
						Dim value As Integer = item.Value
						Dim kind As SymbolKind = key.Kind
						If (kind = SymbolKind.Method) Then
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(key, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (value <> -1) Then
								nextAvailableDispId = -1
							Else
								nextAvailableDispId = SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface.GetNextAvailableDispId(nums, num)
								If (CaseInsensitiveComparison.Equals(methodSymbol.Name, "GetEnumerator") AndAlso methodSymbol.ParameterCount = 0 AndAlso methodSymbol.ReturnType.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator) Then
									nextAvailableDispId = -4
								End If
							End If
							instance.Add(New SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod(Me, methodSymbol, nextAvailableDispId))
						Else
							If (kind <> SymbolKind.[Property]) Then
								Throw ExceptionUtilities.UnexpectedValue(key.Kind)
							End If
							Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(key, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
							Dim synthesizedComMethod As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod = Nothing
							Dim synthesizedComMethod1 As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod = Nothing
							If (Me._defaultMemberName Is Nothing AndAlso propertySymbol.IsDefault) Then
								Me._defaultMemberName = propertySymbol.Name
							End If
							num1 = num1 + 1
							Dim keyValuePair As KeyValuePair(Of Symbol, Integer) = interfaceMembers(num1)
							num1 = num1 + 1
							Dim item1 As KeyValuePair(Of Symbol, Integer) = interfaceMembers(num1)
							If (value = -1 OrElse keyValuePair.Key IsNot Nothing AndAlso keyValuePair.Value = -1 OrElse item1.Key IsNot Nothing AndAlso item1.Value = -1) Then
								nextAvailableDispId = SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface.GetNextAvailableDispId(nums, num)
								If (CaseInsensitiveComparison.Equals(propertySymbol.Name, "GetEnumerator") AndAlso propertySymbol.ParameterCount = 0 AndAlso propertySymbol.Type.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator) Then
									nextAvailableDispId = -4
								ElseIf (propertySymbol.IsDefault) Then
									nextAvailableDispId = 0
								ElseIf (value <> -1) Then
									nextAvailableDispId = value
								End If
							Else
								nextAvailableDispId = -1
							End If
							If (keyValuePair.Key IsNot Nothing) Then
								synthesizedComMethod = New SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod(Me, DirectCast(keyValuePair.Key, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), If(keyValuePair.Value = -1, nextAvailableDispId, -1))
							End If
							If (item1.Key IsNot Nothing) Then
								synthesizedComMethod1 = New SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod(Me, DirectCast(item1.Key, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), If(item1.Value = -1, nextAvailableDispId, -1))
							End If
							If (synthesizedComMethod Is Nothing) Then
								instance.Add(synthesizedComMethod1)
							ElseIf (synthesizedComMethod1 Is Nothing) Then
								instance.Add(synthesizedComMethod)
							ElseIf (LexicalOrderSymbolComparer.Instance.Compare(propertySymbol.GetMethod, propertySymbol.SetMethod) > 0) Then
								instance.Add(synthesizedComMethod1)
								instance.Add(synthesizedComMethod)
							Else
								instance.Add(synthesizedComMethod)
								instance.Add(synthesizedComMethod1)
							End If
							instance.Add(New SourceNamedTypeSymbol.ComClassData.SynthesizedComProperty(Me, propertySymbol, synthesizedComMethod, synthesizedComMethod1, If(value = -1, nextAvailableDispId, -1)))
						End If
						num1 = num1 + 1
					Loop While num1 <= count
					Me._members = instance.ToImmutableAndFree()
				End Sub

				Public Sub New(ByVal comClass As SourceNamedTypeSymbol, ByVal interfaceMembers As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)))
					MyBase.New()
					Dim num As Integer
					Me._comClass = comClass
					Me._isEventInterface = True
					Dim nums As HashSet(Of Integer) = New HashSet(Of Integer)()
					Dim enumerator As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)).Enumerator = interfaceMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of EventSymbol, Integer) = enumerator.Current
						If (current.Value = -1) Then
							Continue While
						End If
						nums.Add(current.Value)
					End While
					Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
					Dim num1 As Integer = 1
					Dim enumerator1 As ArrayBuilder(Of KeyValuePair(Of EventSymbol, Integer)).Enumerator = interfaceMembers.GetEnumerator()
					While enumerator1.MoveNext()
						Dim keyValuePair As KeyValuePair(Of EventSymbol, Integer) = enumerator1.Current
						Dim key As EventSymbol = keyValuePair.Key
						If (Not key.Type.IsDelegateType()) Then
							Continue While
						End If
						Dim delegateInvokeMethod As MethodSymbol = DirectCast(key.Type, NamedTypeSymbol).DelegateInvokeMethod
						If (delegateInvokeMethod Is Nothing) Then
							Continue While
						End If
						num = If(keyValuePair.Value <> -1, -1, SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface.GetNextAvailableDispId(nums, num1))
						instance.Add(New SourceNamedTypeSymbol.ComClassData.SynthesizedComEventMethod(Me, key, delegateInvokeMethod, num))
					End While
					Me._members = instance.ToImmutableAndFree()
				End Sub

				Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
					Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))
					MyBase.AddSynthesizedAttributes(compilationState, attributes)
					Dim declaringCompilation As VisualBasicCompilation = Me._comClass.DeclaringCompilation
					Dim str As String = If(Me._isEventInterface, Me._comClass._comClassData.EventId, Me._comClass._comClassData.InterfaceId)
					If (str IsNot Nothing) Then
						Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._comClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String), TypedConstantKind.Primitive, str))
						keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, typedConstants, keyValuePairs, False))
					End If
					If (Me._isEventInterface) Then
						Dim typedConstants1 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._comClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int16), TypedConstantKind.Primitive, CShort(2)))
						keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16, typedConstants1, keyValuePairs, False))
					End If
					Dim typedConstants2 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._comClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), TypedConstantKind.Primitive, True))
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ComVisibleAttribute__ctor, typedConstants2, keyValuePairs, False))
					If (Me._defaultMemberName IsNot Nothing) Then
						Dim typedConstants3 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._comClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String), TypedConstantKind.Primitive, Me._defaultMemberName))
						keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, typedConstants3, keyValuePairs, False))
					End If
				End Sub

				Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
					Throw ExceptionUtilities.Unreachable
				End Function

				Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
					Return Me = other
				End Function

				Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
					Throw ExceptionUtilities.Unreachable
				End Sub

				Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
					Return ImmutableArray(Of String).Empty
				End Function

				Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
					Return ImmutableArray(Of VisualBasicAttributeData).Empty
				End Function

				Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
					Return SpecializedCollections.EmptyEnumerable(Of FieldSymbol)()
				End Function

				Public Overrides Function GetHashCode() As Integer
					Return RuntimeHelpers.GetHashCode(Me)
				End Function

				Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
					Return Me._members
				End Function

				Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
					Throw ExceptionUtilities.Unreachable
				End Function

				Private Shared Function GetNextAvailableDispId(ByVal usedDispIds As HashSet(Of Integer), <InAttribute> <Out> ByRef nextDispId As Integer) As Integer
					Dim num As Integer = nextDispId
					While usedDispIds.Contains(num)
						num = num + 1
					End While
					nextDispId = num + 1
					Return num
				End Function

				Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
					Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
				End Function

				Public Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
					Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
				End Function

				Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
					Return ImmutableArray(Of NamedTypeSymbol).Empty
				End Function

				Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
					Throw ExceptionUtilities.Unreachable
				End Function

				Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
					Return Nothing
				End Function

				Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
					Return Nothing
				End Function

				Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
					Return ImmutableArray(Of NamedTypeSymbol).Empty
				End Function

				Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
					Return Nothing
				End Function

				Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
					Return ImmutableArray(Of NamedTypeSymbol).Empty
				End Function
			End Class

			Private Class SynthesizedComMethod
				Inherits MethodSymbol
				Public ReadOnly ClonedFrom As MethodSymbol

				Private ReadOnly _synthesizedDispId As Integer

				Private ReadOnly _interface As SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface

				Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

				Public Overrides ReadOnly Property Arity As Integer
					Get
						Return 0
					End Get
				End Property

				Public Overrides ReadOnly Property AssociatedSymbol As Symbol
					Get
						Return Nothing
					End Get
				End Property

				Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
					Get
						Return Me.ClonedFrom.CallingConvention
					End Get
				End Property

				Public Overrides ReadOnly Property ContainingSymbol As Symbol
					Get
						Return Me._interface
					End Get
				End Property

				Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
					Get
						Return Me._interface
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
					Get
						Return Accessibility.[Public]
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
					Get
						Return False
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
					Get
						Return False
					End Get
				End Property

				Friend Overrides ReadOnly Property HasSpecialName As Boolean
					Get
						Return Me.ClonedFrom.HasSpecialName
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
					Get
						Return MethodImplAttributes.IL
					End Get
				End Property

				Public Overrides ReadOnly Property IsAsync As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsExtensionMethod As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property IsExternalMethod As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsIterator As Boolean
					Get
						Return False
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
					Get
						Return Me.ClonedFrom.IsMethodKindBasedOnSyntax
					End Get
				End Property

				Public Overrides ReadOnly Property IsMustOverride As Boolean
					Get
						Return True
					End Get
				End Property

				Public Overrides ReadOnly Property IsNotOverridable As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverloads As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverridable As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverrides As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsShared As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsSub As Boolean
					Get
						Return Me.ClonedFrom.IsSub
					End Get
				End Property

				Public Overrides ReadOnly Property IsVararg As Boolean
					Get
						Return Me.ClonedFrom.IsVararg
					End Get
				End Property

				Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property MethodKind As MethodKind
					Get
						Dim methodKind1 As MethodKind
						Dim methodKind2 As MethodKind = Me.ClonedFrom.MethodKind
						If (methodKind2 = MethodKind.PropertyGet) Then
							methodKind1 = MethodKind.PropertyGet
						Else
							methodKind1 = If(methodKind2 = MethodKind.PropertySet, MethodKind.PropertySet, MethodKind.Ordinary)
						End If
						Return methodKind1
					End Get
				End Property

				Public Overrides ReadOnly Property Name As String
					Get
						Return Me.NameAndAttributesSource.Name
					End Get
				End Property

				Protected Overridable ReadOnly Property NameAndAttributesSource As Symbol
					Get
						Return Me.ClonedFrom
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
					Get
						Return Nothing
					End Get
				End Property

				Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
					Get
						Return Me._parameters
					End Get
				End Property

				Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me.ClonedFrom.RefCustomModifiers
					End Get
				End Property

				Public Overrides ReadOnly Property ReturnsByRef As Boolean
					Get
						Return Me.ClonedFrom.ReturnsByRef
					End Get
				End Property

				Public Overrides ReadOnly Property ReturnType As TypeSymbol
					Get
						Return Me.ClonedFrom.ReturnType
					End Get
				End Property

				Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me.ClonedFrom.ReturnTypeCustomModifiers
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
					Get
						Return Me.ClonedFrom.ReturnTypeMarshallingInformation
					End Get
				End Property

				Friend Overrides ReadOnly Property Syntax As SyntaxNode
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
					Get
						Return ImmutableArray(Of TypeSymbol).Empty
					End Get
				End Property

				Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
					Get
						Return ImmutableArray(Of TypeParameterSymbol).Empty
					End Get
				End Property

				Public Sub New(ByVal container As SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface, ByVal clone As MethodSymbol, ByVal synthesizedDispId As Integer)
					MyBase.New()
					Me._interface = container
					Me._synthesizedDispId = synthesizedDispId
					Me.ClonedFrom = clone
					If (clone.ParameterCount = 0) Then
						Me._parameters = ImmutableArray(Of ParameterSymbol).Empty
						Return
					End If
					Dim synthesizedComParameter(clone.ParameterCount - 1 + 1 - 1) As ParameterSymbol
					Dim length As Integer = CInt(synthesizedComParameter.Length) - 1
					Dim num As Integer = 0
					Do
						Dim parameters As ImmutableArray(Of ParameterSymbol) = clone.Parameters
						synthesizedComParameter(num) = New SourceNamedTypeSymbol.ComClassData.SynthesizedComParameter(Me, parameters(num))
						num = num + 1
					Loop While num <= length
					Me._parameters = synthesizedComParameter.AsImmutable(Of ParameterSymbol)()
				End Sub

				Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
					MyBase.AddSynthesizedAttributes(compilationState, attributes)
					If (Me._synthesizedDispId <> -1) Then
						Dim declaringCompilation As VisualBasicCompilation = Me._interface.ComClass.DeclaringCompilation
						Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._interface.ComClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32), TypedConstantKind.Primitive, Me._synthesizedDispId))
						Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, typedConstants, keyValuePairs, False))
					End If
				End Sub

				Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
					Return ImmutableArray(Of String).Empty
				End Function

				Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
					' 
					' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData> Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol/ComClassData/SynthesizedComMethod::GetAttributes()
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

				Public NotOverridable Overrides Function GetDllImportData() As DllImportData
					Return Nothing
				End Function

				Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
					Dim empty As ImmutableArray(Of VisualBasicAttributeData)
					Dim nameAndAttributesSource As Symbol = Me.NameAndAttributesSource
					If (nameAndAttributesSource.Kind <> SymbolKind.Method) Then
						empty = ImmutableArray(Of VisualBasicAttributeData).Empty
					Else
						empty = DirectCast(nameAndAttributesSource, MethodSymbol).GetReturnTypeAttributes()
					End If
					Return empty
				End Function

				Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
					Throw ExceptionUtilities.Unreachable
				End Function

				Friend NotOverridable Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
					Return True
				End Function
			End Class

			Private NotInheritable Class SynthesizedComParameter
				Inherits ParameterSymbol
				Private ReadOnly _container As Symbol

				Private ReadOnly _clonedFrom As ParameterSymbol

				Public Overrides ReadOnly Property ContainingSymbol As Symbol
					Get
						Return Me._container
					End Get
				End Property

				Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me._clonedFrom.CustomModifiers
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
					Get
						Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
						If (Not Me.IsComEventParameter) Then
							constantValue = Me._clonedFrom(inProgress)
						Else
							constantValue = Nothing
						End If
						Return constantValue
					End Get
				End Property

				Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.HasExplicitDefaultValue, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property HasOptionCompare As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.HasOptionCompare, False)
					End Get
				End Property

				Public Overrides ReadOnly Property IsByRef As Boolean
					Get
						Return Me._clonedFrom.IsByRef
					End Get
				End Property

				Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsCallerFilePath, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsCallerLineNumber, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsCallerMemberName, False)
					End Get
				End Property

				Public ReadOnly Property IsComEventParameter As Boolean
					Get
						Return DirectCast(Me._container.ContainingSymbol, SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface).IsEventInterface
					End Get
				End Property

				Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
					Get
						Return Me._clonedFrom.IsExplicitByRef
					End Get
				End Property

				Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsIDispatchConstant, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsIUnknownConstant, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property IsMetadataIn As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsMetadataIn, False)
					End Get
				End Property

				Friend Overrides ReadOnly Property IsMetadataOut As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsMetadataOut, False)
					End Get
				End Property

				Public Overrides ReadOnly Property IsOptional As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsOptional, False)
					End Get
				End Property

				Public Overrides ReadOnly Property IsParamArray As Boolean
					Get
						Return If(Not Me.IsComEventParameter, Me._clonedFrom.IsParamArray, False)
					End Get
				End Property

				Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
					Get
						Dim marshalPseudoCustomAttributeDatum As MarshalPseudoCustomAttributeData
						If (Not Me.IsComEventParameter) Then
							marshalPseudoCustomAttributeDatum = Me._clonedFrom.MarshallingInformation
						Else
							marshalPseudoCustomAttributeDatum = Nothing
						End If
						Return marshalPseudoCustomAttributeDatum
					End Get
				End Property

				Public Overrides ReadOnly Property Name As String
					Get
						Return Me._clonedFrom.Name
					End Get
				End Property

				Public Overrides ReadOnly Property Ordinal As Integer
					Get
						Return Me._clonedFrom.Ordinal
					End Get
				End Property

				Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me._clonedFrom.RefCustomModifiers
					End Get
				End Property

				Public Overrides ReadOnly Property Type As TypeSymbol
					Get
						Return Me._clonedFrom.Type
					End Get
				End Property

				Public Sub New(ByVal container As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod, ByVal clone As ParameterSymbol)
					MyBase.New()
					Me._container = container
					Me._clonedFrom = clone
				End Sub

				Public Sub New(ByVal container As SourceNamedTypeSymbol.ComClassData.SynthesizedComProperty, ByVal clone As ParameterSymbol)
					MyBase.New()
					Me._container = container
					Me._clonedFrom = clone
				End Sub

				Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
					MyBase.AddSynthesizedAttributes(compilationState, attributes)
					If (Not Me.IsComEventParameter) Then
						Dim synthesizedAttributeDatas As ArrayBuilder(Of SynthesizedAttributeData) = Nothing
						Me._clonedFrom.AddSynthesizedAttributes(compilationState, synthesizedAttributeDatas)
						Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
						Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = declaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_ParamArrayAttribute)
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = declaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_CompilerServices_DateTimeConstantAttribute)
						Dim wellKnownType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = declaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_CompilerServices_DecimalConstantAttribute)
						If (synthesizedAttributeDatas IsNot Nothing) Then
							Dim enumerator As ArrayBuilder(Of SynthesizedAttributeData).Enumerator = synthesizedAttributeDatas.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As SynthesizedAttributeData = enumerator.Current
								If (CObj(current.AttributeClass) <> CObj(wellKnownType) AndAlso CObj(current.AttributeClass) <> CObj(namedTypeSymbol) AndAlso CObj(current.AttributeClass) <> CObj(wellKnownType1)) Then
									Continue While
								End If
								Symbol.AddSynthesizedAttribute(attributes, current)
							End While
							synthesizedAttributeDatas.Free()
						End If
					End If
				End Sub

				Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
					Dim attributes As ImmutableArray(Of VisualBasicAttributeData)
					If (Not Me.IsComEventParameter) Then
						attributes = Me._clonedFrom.GetAttributes()
					Else
						attributes = ImmutableArray(Of VisualBasicAttributeData).Empty
					End If
					Return attributes
				End Function
			End Class

			Private Class SynthesizedComProperty
				Inherits PropertySymbol
				Private ReadOnly _interface As SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface

				Private ReadOnly _clonedFrom As PropertySymbol

				Private ReadOnly _synthesizedDispId As Integer

				Private ReadOnly _getter As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod

				Private ReadOnly _setter As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod

				Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

				Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
					Get
						Return Nothing
					End Get
				End Property

				Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
					Get
						Return Me._clonedFrom.CallingConvention
					End Get
				End Property

				Public Overrides ReadOnly Property ContainingSymbol As Symbol
					Get
						Return Me._interface
					End Get
				End Property

				Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
					Get
						Return Me._interface
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
					Get
						Return Accessibility.[Public]
					End Get
				End Property

				Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
					Get
						Return ImmutableArray(Of PropertySymbol).Empty
					End Get
				End Property

				Public Overrides ReadOnly Property GetMethod As MethodSymbol
					Get
						Return Me._getter
					End Get
				End Property

				Friend Overrides ReadOnly Property HasSpecialName As Boolean
					Get
						Return Me._clonedFrom.HasSpecialName
					End Get
				End Property

				Public Overrides ReadOnly Property IsDefault As Boolean
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property IsMustOverride As Boolean
					Get
						Return True
					End Get
				End Property

				Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsNotOverridable As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverloads As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverridable As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsOverrides As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property IsShared As Boolean
					Get
						Return False
					End Get
				End Property

				Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
					Get
						Throw ExceptionUtilities.Unreachable
					End Get
				End Property

				Public Overrides ReadOnly Property Name As String
					Get
						Return Me._clonedFrom.Name
					End Get
				End Property

				Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
					Get
						Return Nothing
					End Get
				End Property

				Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
					Get
						Return Me._parameters
					End Get
				End Property

				Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me._clonedFrom.RefCustomModifiers
					End Get
				End Property

				Public Overrides ReadOnly Property ReturnsByRef As Boolean
					Get
						Return Me._clonedFrom.ReturnsByRef
					End Get
				End Property

				Public Overrides ReadOnly Property SetMethod As MethodSymbol
					Get
						Return Me._setter
					End Get
				End Property

				Public Overrides ReadOnly Property Type As TypeSymbol
					Get
						Return Me._clonedFrom.Type
					End Get
				End Property

				Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
					Get
						Return Me._clonedFrom.TypeCustomModifiers
					End Get
				End Property

				Public Sub New(ByVal container As SourceNamedTypeSymbol.ComClassData.SynthesizedComInterface, ByVal clone As PropertySymbol, ByVal getter As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod, ByVal setter As SourceNamedTypeSymbol.ComClassData.SynthesizedComMethod, ByVal synthesizedDispId As Integer)
					MyBase.New()
					Me._interface = container
					Me._clonedFrom = clone
					Me._synthesizedDispId = synthesizedDispId
					Me._getter = getter
					Me._setter = setter
					If (clone.ParameterCount = 0) Then
						Me._parameters = ImmutableArray(Of ParameterSymbol).Empty
						Return
					End If
					Dim synthesizedComParameter(clone.ParameterCount - 1 + 1 - 1) As ParameterSymbol
					Dim length As Integer = CInt(synthesizedComParameter.Length) - 1
					Dim num As Integer = 0
					Do
						Dim parameters As ImmutableArray(Of ParameterSymbol) = clone.Parameters
						synthesizedComParameter(num) = New SourceNamedTypeSymbol.ComClassData.SynthesizedComParameter(Me, parameters(num))
						num = num + 1
					Loop While num <= length
					Me._parameters = synthesizedComParameter.AsImmutable(Of ParameterSymbol)()
				End Sub

				Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
					MyBase.AddSynthesizedAttributes(compilationState, attributes)
					If (Me._synthesizedDispId <> -1) Then
						Dim declaringCompilation As VisualBasicCompilation = Me._interface.ComClass.DeclaringCompilation
						Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me._interface.ComClass.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32), TypedConstantKind.Primitive, Me._synthesizedDispId))
						Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, typedConstants, keyValuePairs, False))
					End If
				End Sub

				Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
					Return Me._clonedFrom.GetAttributes()
				End Function
			End Class
		End Class

		Private Class GroupCollectionComparer
			Implements IComparer(Of KeyValuePair(Of NamedTypeSymbol, Integer))
			Public ReadOnly Shared Singleton As SourceNamedTypeSymbol.GroupCollectionComparer

			Shared Sub New()
				SourceNamedTypeSymbol.GroupCollectionComparer.Singleton = New SourceNamedTypeSymbol.GroupCollectionComparer()
			End Sub

			Private Sub New()
				MyBase.New()
			End Sub

			Public Function Compare(ByVal x As KeyValuePair(Of NamedTypeSymbol, Integer), ByVal y As KeyValuePair(Of NamedTypeSymbol, Integer)) As Integer Implements IComparer(Of KeyValuePair(Of NamedTypeSymbol, Integer)).Compare
				Return CaseInsensitiveComparison.Compare(x.Key.Name, y.Key.Name)
			End Function
		End Class

		Private Structure TypeParameterInfo
			Public ReadOnly Variance As VarianceKind

			Public ReadOnly Constraints As ImmutableArray(Of TypeParameterConstraint)

			Public ReadOnly Property Initialized As Boolean
				Get
					Return Not Me.Constraints.IsDefault
				End Get
			End Property

			Public Sub New(ByVal variance As VarianceKind, ByVal constraints As ImmutableArray(Of TypeParameterConstraint))
				Me = New SourceNamedTypeSymbol.TypeParameterInfo() With
				{
					.Variance = variance,
					.Constraints = constraints
				}
			End Sub
		End Structure
	End Class
End Namespace