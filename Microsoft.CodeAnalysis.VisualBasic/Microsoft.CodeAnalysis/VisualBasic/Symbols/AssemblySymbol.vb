Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class AssemblySymbol
		Inherits Symbol
		Implements IAssemblySymbol, IAssemblySymbolInternal
		Private _corLibrary As AssemblySymbol

		Private ReadOnly Shared s_nestedTypeNameSeparators As Char()

		Public MustOverride ReadOnly Property AssemblyVersionPattern As Version Implements IAssemblySymbolInternal.AssemblyVersionPattern

		Friend ReadOnly Property Bit32Required As Boolean
			Get
				Return Me.Modules(0).Bit32Required
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend ReadOnly Property CorLibrary As AssemblySymbol
			Get
				Return Me._corLibrary
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public MustOverride ReadOnly Property GlobalNamespace As NamespaceSymbol

		ReadOnly Property IAssemblySymbol_GlobalNamespace As INamespaceSymbol Implements IAssemblySymbol.GlobalNamespace
			Get
				Return Me.GlobalNamespace
			End Get
		End Property

		ReadOnly Property IAssemblySymbol_Modules As IEnumerable(Of IModuleSymbol) Implements IAssemblySymbol.Modules
			Get
				Return DirectCast(Me.Modules, IEnumerable(Of IModuleSymbol))
			End Get
		End Property

		ReadOnly Property IAssemblySymbolInternal_CorLibrary As IAssemblySymbolInternal Implements IAssemblySymbolInternal.CorLibrary
			Get
				Return Me.CorLibrary
			End Get
		End Property

		Public MustOverride ReadOnly Property Identity As AssemblyIdentity Implements IAssemblySymbol.Identity, IAssemblySymbolInternal.Identity

		Public Overridable ReadOnly Property IsInteractive As Boolean Implements IAssemblySymbol.IsInteractive
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsLinked As Boolean

		Friend MustOverride ReadOnly Property IsMissing As Boolean

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property KeepLookingForDeclaredSpecialTypes As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.Assembly
			End Get
		End Property

		Friend ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine
			Get
				Return Me.Modules(0).Machine
			End Get
		End Property

		Public MustOverride ReadOnly Property MightContainExtensionMethods As Boolean Implements IAssemblySymbol.MightContainExtensionMethods

		Public MustOverride ReadOnly Property Modules As ImmutableArray(Of ModuleSymbol)

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.Identity.Name
			End Get
		End Property

		Public MustOverride ReadOnly Property NamespaceNames As ICollection(Of String) Implements IAssemblySymbol.NamespaceNames

		Friend ReadOnly Property ObjectType As NamedTypeSymbol
			Get
				Return Me.GetSpecialType(SpecialType.System_Object)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property PublicKey As ImmutableArray(Of Byte)

		Friend ReadOnly Property RuntimeSupportsDefaultInterfaceImplementation As Boolean
			Get
				Return Me.RuntimeSupportsFeature(SpecialMember.System_Runtime_CompilerServices_RuntimeFeature__DefaultImplementationsOfInterfaces)
			End Get
		End Property

		Public MustOverride ReadOnly Property TypeNames As ICollection(Of String) Implements IAssemblySymbol.TypeNames

		Shared Sub New()
			AssemblySymbol.s_nestedTypeNameSeparators = New [Char]() { "+"C }
		End Sub

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitAssembly(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitAssembly(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitAssembly(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitAssembly(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitAssembly(Me)
		End Function

		Friend MustOverride Function AreInternalsVisibleToThisAssembly(ByVal other As AssemblySymbol) As Boolean

		Friend Function CreateCycleInTypeForwarderErrorTypeSymbol(ByRef emittedName As MetadataTypeName) As ErrorTypeSymbol
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = New Microsoft.CodeAnalysis.DiagnosticInfo(MessageProvider.Instance, 31425, New [Object]() { emittedName.FullName, Me })
			Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
			Return New MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo(modules(0), emittedName, diagnosticInfo, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
		End Function

		Friend Function CreateMultipleForwardingErrorTypeSymbol(ByRef emittedName As MetadataTypeName, ByVal forwardingModule As ModuleSymbol, ByVal destination1 As AssemblySymbol, ByVal destination2 As AssemblySymbol) As ErrorTypeSymbol
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = New Microsoft.CodeAnalysis.DiagnosticInfo(MessageProvider.Instance, 37208, New [Object]() { forwardingModule, Me, emittedName.FullName, destination1, destination2 })
			Return New MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo(forwardingModule, emittedName, diagnosticInfo, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
		End Function

		Friend MustOverride Function GetAllTopLevelForwardedTypes() As IEnumerable(Of NamedTypeSymbol)

		Friend Function GetAssemblyNamespace(ByVal namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim globalNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			If (Not namespaceSymbol.IsGlobalNamespace) Then
				Dim containingNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = namespaceSymbol.ContainingNamespace
				If (containingNamespace Is Nothing) Then
					globalNamespace = Me.GlobalNamespace
				ElseIf (namespaceSymbol.Extent.Kind <> NamespaceKind.Assembly OrElse Not (namespaceSymbol.ContainingAssembly = Me)) Then
					Dim assemblyNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GetAssemblyNamespace(containingNamespace)
					If (CObj(assemblyNamespace) = CObj(containingNamespace)) Then
						globalNamespace = namespaceSymbol
					ElseIf (assemblyNamespace IsNot Nothing) Then
						globalNamespace = assemblyNamespace.GetNestedNamespace(namespaceSymbol.Name)
					Else
						globalNamespace = Nothing
					End If
				Else
					globalNamespace = namespaceSymbol
				End If
			Else
				globalNamespace = Me.GlobalNamespace
			End If
			Return globalNamespace
		End Function

		Friend MustOverride Function GetDeclaredSpecialType(ByVal type As SpecialType) As NamedTypeSymbol

		Friend Overridable Function GetDeclaredSpecialTypeMember(ByVal member As SpecialMember) As Symbol
			Return Nothing
		End Function

		Friend Overridable Function GetGuidString(ByRef guidString As String) As Boolean
			Return MyBase.GetGuidStringDefaultImplementation(guidString)
		End Function

		Friend MustOverride Function GetInternalsVisibleToPublicKeys(ByVal simpleName As String) As IEnumerable(Of ImmutableArray(Of Byte))

		Friend MustOverride Function GetLinkedReferencedAssemblies() As ImmutableArray(Of AssemblySymbol)

		Public MustOverride Function GetMetadata() As AssemblyMetadata Implements IAssemblySymbol.GetMetadata

		Friend MustOverride Function GetNoPiaResolutionAssemblies() As ImmutableArray(Of AssemblySymbol)

		Friend Function GetPrimitiveType(ByVal type As PrimitiveTypeCode) As NamedTypeSymbol
			Return Me.GetSpecialType(SpecialTypes.GetTypeFromMetadataName(type))
		End Function

		Friend Function GetSpecialType(ByVal type As SpecialType) As NamedTypeSymbol
			If (type <= SpecialType.None OrElse type > SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute) Then
				Throw New ArgumentOutOfRangeException("type", [String].Format("Unexpected SpecialType: '{0}'.", CInt(type)))
			End If
			Return Me.CorLibrary.GetDeclaredSpecialType(type)
		End Function

		Friend Overridable Function GetSpecialTypeMember(ByVal member As SpecialMember) As Symbol
			Return Me.CorLibrary.GetDeclaredSpecialTypeMember(member)
		End Function

		Friend Function GetTopLevelTypeByMetadataName(ByRef metadataName As MetadataTypeName, ByVal includeReferences As Boolean, ByVal isWellKnownType As Boolean, <Out> ByRef conflicts As ValueTuple(Of AssemblySymbol, AssemblySymbol), Optional ByVal ignoreCorLibraryDuplicatedTypes As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			conflicts = New ValueTuple(Of AssemblySymbol, AssemblySymbol)()
			Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.LookupTopLevelMetadataType(metadataName, False)
			If (isWellKnownType AndAlso Not Me.IsValidWellKnownType(namedTypeSymbol2)) Then
				namedTypeSymbol2 = Nothing
			End If
			If (Not AssemblySymbol.IsAcceptableMatchForGetTypeByNameAndArity(namedTypeSymbol2)) Then
				namedTypeSymbol2 = Nothing
				If (includeReferences) Then
					Dim flag As Boolean = False
					If (CObj(Me.CorLibrary) <> CObj(Me) AndAlso Not Me.CorLibrary.IsMissing AndAlso Not ignoreCorLibraryDuplicatedTypes) Then
						Dim namedTypeSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.CorLibrary.LookupTopLevelMetadataType(metadataName, False)
						flag = True
						If (Not Me.IsValidCandidate(namedTypeSymbol3, isWellKnownType)) Then
							GoTo Label1
						End If
						namedTypeSymbol = namedTypeSymbol3
						Return namedTypeSymbol
					End If
				Label1:
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
					Dim referencedAssemblySymbols As ImmutableArray(Of AssemblySymbol) = modules(0).GetReferencedAssemblySymbols()
					Dim length As Integer = referencedAssemblySymbols.Length - 1
					Dim num As Integer = 0
					While True
						If (num <= length) Then
							Dim item As AssemblySymbol = referencedAssemblySymbols(num)
							If (Not flag OrElse CObj(item) <> CObj(Me.CorLibrary)) Then
								namedTypeSymbol1 = item.LookupTopLevelMetadataType(metadataName, False)
								If (Me.IsValidCandidate(namedTypeSymbol1, isWellKnownType) AndAlso Not TypeSymbol.Equals(namedTypeSymbol1, namedTypeSymbol2, TypeCompareKind.ConsiderEverything)) Then
									If (namedTypeSymbol2 Is Nothing) Then
										namedTypeSymbol2 = namedTypeSymbol1
									Else
										If (Not ignoreCorLibraryDuplicatedTypes) Then
											Exit While
										End If
										If (Not Me.IsInCorLib(namedTypeSymbol1)) Then
											If (Not Me.IsInCorLib(namedTypeSymbol2)) Then
												Exit While
											End If
											namedTypeSymbol2 = namedTypeSymbol1
										End If
									End If
								End If
							End If
							num = num + 1
						Else
							namedTypeSymbol = namedTypeSymbol2
							Return namedTypeSymbol
						End If
					End While
					conflicts = New ValueTuple(Of AssemblySymbol, AssemblySymbol)(namedTypeSymbol2.ContainingAssembly, namedTypeSymbol1.ContainingAssembly)
					namedTypeSymbol = Nothing
				Else
					namedTypeSymbol = namedTypeSymbol2
				End If
			Else
				namedTypeSymbol = namedTypeSymbol2
			End If
			Return namedTypeSymbol
		End Function

		Public Function GetTypeByMetadataName(ByVal fullyQualifiedMetadataName As String) As NamedTypeSymbol
			Dim valueTuple As ValueTuple(Of AssemblySymbol, AssemblySymbol) = New ValueTuple(Of AssemblySymbol, AssemblySymbol)()
			Return Me.GetTypeByMetadataName(fullyQualifiedMetadataName, False, False, valueTuple, False, False)
		End Function

		Friend Function GetTypeByMetadataName(ByVal metadataName As String, ByVal includeReferences As Boolean, ByVal isWellKnownType As Boolean, <Out> ByRef conflicts As ValueTuple(Of AssemblySymbol, AssemblySymbol), Optional ByVal useCLSCompliantNameArityEncoding As Boolean = False, Optional ByVal ignoreCorLibraryDuplicatedTypes As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim topLevelTypeByMetadataName As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (metadataName Is Nothing) Then
				Throw New ArgumentNullException("metadataName")
			End If
			If (Not metadataName.Contains("+")) Then
				metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(metadataName, useCLSCompliantNameArityEncoding, -1)
				topLevelTypeByMetadataName = Me.GetTopLevelTypeByMetadataName(metadataTypeName, includeReferences, isWellKnownType, conflicts, ignoreCorLibraryDuplicatedTypes)
			Else
				Dim strArray As String() = metadataName.Split(AssemblySymbol.s_nestedTypeNameSeparators)
				metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(strArray(0), useCLSCompliantNameArityEncoding, -1)
				topLevelTypeByMetadataName = Me.GetTopLevelTypeByMetadataName(metadataTypeName, includeReferences, isWellKnownType, conflicts, False)
				Dim num As Integer = 1
				While topLevelTypeByMetadataName IsNot Nothing AndAlso Not topLevelTypeByMetadataName.IsErrorType()
					If (num < CInt(strArray.Length)) Then
						metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromTypeName(strArray(num), False, -1)
						Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = topLevelTypeByMetadataName.LookupMetadataType(metadataTypeName)
						If (Not isWellKnownType OrElse Me.IsValidWellKnownType(namedTypeSymbol1)) Then
							namedTypeSymbol = namedTypeSymbol1
						Else
							namedTypeSymbol = Nothing
						End If
						topLevelTypeByMetadataName = namedTypeSymbol
						num = num + 1
					Else
						Exit While
					End If
				End While
			End If
			If (topLevelTypeByMetadataName IsNot Nothing AndAlso Not topLevelTypeByMetadataName.IsErrorType()) Then
				Return topLevelTypeByMetadataName
			End If
			Return Nothing
		End Function

		Private Function IAssemblySymbol_GetForwardedTypes() As ImmutableArray(Of INamedTypeSymbol) Implements IAssemblySymbol.GetForwardedTypes
			Dim displayString As Func(Of NamedTypeSymbol, String)
			Dim allTopLevelForwardedTypes As IEnumerable(Of NamedTypeSymbol) = Me.GetAllTopLevelForwardedTypes()
			If (AssemblySymbol._Closure$__.$I101-0 Is Nothing) Then
				displayString = Function(t As NamedTypeSymbol) t.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat)
				AssemblySymbol._Closure$__.$I101-0 = displayString
			Else
				displayString = AssemblySymbol._Closure$__.$I101-0
			End If
			Return allTopLevelForwardedTypes.OrderBy(Of String)(displayString).AsImmutable()
		End Function

		Private Function IAssemblySymbol_GetTypeByMetadataName(ByVal metadataName As String) As INamedTypeSymbol Implements IAssemblySymbol.GetTypeByMetadataName
			Return Me.GetTypeByMetadataName(metadataName)
		End Function

		Private Function IAssemblySymbol_GivesAccessTo(ByVal assemblyWantingAccess As IAssemblySymbol) As Boolean Implements IAssemblySymbol.GivesAccessTo
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of ImmutableArray(Of Byte)) = Nothing
			If (Not [Object].Equals(Me, assemblyWantingAccess)) Then
				Dim internalsVisibleToPublicKeys As IEnumerable(Of ImmutableArray(Of Byte)) = Me.GetInternalsVisibleToPublicKeys(assemblyWantingAccess.Name)
				If (Not internalsVisibleToPublicKeys.Any() OrElse Not assemblyWantingAccess.IsNetModule()) Then
					Using enumerator
						enumerator = internalsVisibleToPublicKeys.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As ImmutableArray(Of Byte) = enumerator.Current
							If (ISymbolExtensions.PerformIVTCheck(Me.Identity, assemblyWantingAccess.Identity.PublicKey, current) <> IVTConclusion.Match) Then
								Continue While
							End If
							flag = True
							Return flag
						End While
					End Using
					flag = False
				Else
					flag = True
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function IAssemblySymbol_ResolveForwardedType(ByVal metadataName As String) As INamedTypeSymbol Implements IAssemblySymbol.ResolveForwardedType
			Return Me.ResolveForwardedType(metadataName)
		End Function

		Friend Shared Function IsAcceptableMatchForGetTypeByNameAndArity(ByVal candidate As NamedTypeSymbol) As Boolean
			If (candidate Is Nothing) Then
				Return False
			End If
			If (candidate.Kind <> SymbolKind.ErrorType) Then
				Return True
			End If
			Return Not TypeOf candidate Is MissingMetadataTypeSymbol
		End Function

		Private Function IsInCorLib(ByVal type As NamedTypeSymbol) As Boolean
			Return CObj(type.ContainingAssembly) = CObj(Me.CorLibrary)
		End Function

		Private Function IsValidCandidate(ByVal candidate As NamedTypeSymbol, ByVal isWellKnownType As Boolean) As Boolean
			If (isWellKnownType AndAlso Not Me.IsValidWellKnownType(candidate) OrElse Not AssemblySymbol.IsAcceptableMatchForGetTypeByNameAndArity(candidate) OrElse candidate.IsHiddenByVisualBasicEmbeddedAttribute()) Then
				Return False
			End If
			Return Not candidate.IsHiddenByCodeAnalysisEmbeddedAttribute()
		End Function

		Friend Function IsValidWellKnownType(ByVal result As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			If (result Is Nothing OrElse result.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
				flag = False
			Else
				flag = If(result.DeclaredAccessibility = Accessibility.[Public], True, Symbol.IsSymbolAccessible(result, Me))
			End If
			Return flag
		End Function

		Friend Function LookupTopLevelMetadataType(ByRef emittedName As MetadataTypeName, ByVal digThroughForwardedTypes As Boolean) As NamedTypeSymbol
			Return Me.LookupTopLevelMetadataTypeWithCycleDetection(emittedName, Nothing, digThroughForwardedTypes)
		End Function

		Friend MustOverride Function LookupTopLevelMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal digThroughForwardedTypes As Boolean) As NamedTypeSymbol

		Friend Overridable Sub RegisterDeclaredSpecialType(ByVal corType As NamedTypeSymbol)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Public Function ResolveForwardedType(ByVal fullyQualifiedMetadataName As String) As NamedTypeSymbol
			If (fullyQualifiedMetadataName Is Nothing) Then
				Throw New ArgumentNullException("fullyQualifiedMetadataName")
			End If
			Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(fullyQualifiedMetadataName, False, -1)
			Return Me.TryLookupForwardedMetadataType(metadataTypeName, False)
		End Function

		Private Function RuntimeSupportsFeature(ByVal feature As SpecialMember) As Boolean
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Runtime_CompilerServices_RuntimeFeature)
			If (Not specialType.IsClassType() OrElse Not specialType.IsMetadataAbstract OrElse Not specialType.IsMetadataSealed) Then
				Return False
			End If
			Return CObj(Me.GetSpecialTypeMember(feature)) <> CObj(Nothing)
		End Function

		Friend Sub SetCorLibrary(ByVal corLibrary As AssemblySymbol)
			Me._corLibrary = corLibrary
		End Sub

		Friend MustOverride Sub SetLinkedReferencedAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))

		Friend MustOverride Sub SetNoPiaResolutionAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))

		Friend Function TryLookupForwardedMetadataType(ByRef emittedName As MetadataTypeName, ByVal ignoreCase As Boolean) As NamedTypeSymbol
			Return Me.TryLookupForwardedMetadataTypeWithCycleDetection(emittedName, Nothing, ignoreCase)
		End Function

		Friend Overridable Function TryLookupForwardedMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal ignoreCase As Boolean) As NamedTypeSymbol
			Return Nothing
		End Function
	End Class
End Namespace