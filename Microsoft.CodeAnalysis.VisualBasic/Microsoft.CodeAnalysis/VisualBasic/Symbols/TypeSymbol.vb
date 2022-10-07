Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class TypeSymbol
		Inherits NamespaceOrTypeSymbol
		Implements ITypeSymbol, ITypeSymbolInternal
		Friend Const ImplicitTypeName As String = "<invalid-global-code>"

		Private ReadOnly Shared s_EmptyTypeSymbols As TypeSymbol()

		Private _lazyAllInterfaces As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyInterfacesAndTheirBaseInterfaces As MultiDictionary(Of NamedTypeSymbol, NamedTypeSymbol)

		Private ReadOnly Shared EmptyInterfacesAndTheirBaseInterfaces As MultiDictionary(Of NamedTypeSymbol, NamedTypeSymbol)

		Private _lazyImplementationForInterfaceMemberMap As ConcurrentDictionary(Of Symbol, Symbol)

		Protected m_lazyExplicitInterfaceImplementationMap As MultiDictionary(Of Symbol, Symbol)

		Protected ReadOnly Shared EmptyExplicitImplementationMap As MultiDictionary(Of Symbol, Symbol)

		Friend ReadOnly Property AllInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
			Get
				If (Me._lazyAllInterfaces.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of NamedTypeSymbol)(Me._lazyAllInterfaces, Me.MakeAllInterfaces())
				End If
				Return Me._lazyAllInterfaces
			End Get
		End Property

		Friend MustOverride ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol

		Public ReadOnly Shared Property EmptyTypeSymbolsList As IList(Of TypeSymbol)
			Get
				Return TypeSymbol.s_EmptyTypeSymbols
			End Get
		End Property

		Friend Overridable ReadOnly Property ExplicitInterfaceImplementationMap As MultiDictionary(Of Symbol, Symbol)
			Get
				If (Me.m_lazyExplicitInterfaceImplementationMap Is Nothing) Then
					Interlocked.CompareExchange(Of MultiDictionary(Of Symbol, Symbol))(Me.m_lazyExplicitInterfaceImplementationMap, Me.MakeExplicitInterfaceImplementationMap(), Nothing)
				End If
				Return Me.m_lazyExplicitInterfaceImplementationMap
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Me.GetUseSiteInfo().DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				Return diagnosticInfo.Code = 30649
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30649
			End Get
		End Property

		Private ReadOnly Property ImplementationForInterfaceMemberMap As ConcurrentDictionary(Of Symbol, Symbol)
			Get
				Dim symbols As ConcurrentDictionary(Of Symbol, Symbol)
				Dim symbols1 As ConcurrentDictionary(Of Symbol, Symbol) = Me._lazyImplementationForInterfaceMemberMap
				If (symbols1 Is Nothing) Then
					symbols1 = New ConcurrentDictionary(Of Symbol, Symbol)(1, 1)
					symbols = If(Interlocked.CompareExchange(Of ConcurrentDictionary(Of Symbol, Symbol))(Me._lazyImplementationForInterfaceMemberMap, symbols1, Nothing), symbols1)
				Else
					symbols = symbols1
				End If
				Return symbols
			End Get
		End Property

		Friend ReadOnly Property InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics As MultiDictionary(Of NamedTypeSymbol, NamedTypeSymbol)
			Get
				If (Me._lazyInterfacesAndTheirBaseInterfaces Is Nothing) Then
					Interlocked.CompareExchange(Of MultiDictionary(Of NamedTypeSymbol, NamedTypeSymbol))(Me._lazyInterfacesAndTheirBaseInterfaces, TypeSymbol.MakeInterfacesAndTheirBaseInterfaces(Me.InterfacesNoUseSiteDiagnostics), Nothing)
				End If
				Return Me._lazyInterfacesAndTheirBaseInterfaces
			End Get
		End Property

		Friend MustOverride ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)

		Public Overridable ReadOnly Property IsAnonymousType As Boolean Implements ITypeSymbol.IsAnonymousType
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsReferenceType As Boolean Implements ITypeSymbol.IsReferenceType, ITypeSymbolInternal.IsReferenceType

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property IsTupleType As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsValueType As Boolean Implements ITypeSymbol.IsValueType, ITypeSymbolInternal.IsValueType

		ReadOnly Property ITypeSymbol_AllInterfaces As ImmutableArray(Of INamedTypeSymbol) Implements ITypeSymbol.AllInterfaces
			Get
				Return StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(Me.AllInterfacesNoUseSiteDiagnostics)
			End Get
		End Property

		ReadOnly Property ITypeSymbol_BaseType As INamedTypeSymbol Implements ITypeSymbol.BaseType
			Get
				Return Me.BaseTypeNoUseSiteDiagnostics
			End Get
		End Property

		ReadOnly Property ITypeSymbol_Interfaces As ImmutableArray(Of INamedTypeSymbol) Implements ITypeSymbol.Interfaces
			Get
				Return StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(Me.InterfacesNoUseSiteDiagnostics)
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsNativeIntegerType As Boolean Implements ITypeSymbol.IsNativeIntegerType
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsReadOnly As Boolean Implements ITypeSymbol.IsReadOnly
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsRecord As Boolean Implements ITypeSymbol.IsRecord
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsRefLikeType As Boolean Implements ITypeSymbol.IsRefLikeType
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsTupleSymbol As Boolean Implements ITypeSymbol.IsTupleType
			Get
				Return Me.IsTupleType
			End Get
		End Property

		ReadOnly Property ITypeSymbol_IsUnmanagedType As Boolean Implements ITypeSymbol.IsUnmanagedType
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeSymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements ITypeSymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property ITypeSymbol_OriginalDefinition As ITypeSymbol Implements ITypeSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property ITypeSymbol_TypeKind As Microsoft.CodeAnalysis.TypeKind Implements ITypeSymbol.TypeKind, ITypeSymbolInternal.TypeKind
			Get
				Return Me.TypeKind
			End Get
		End Property

		Public Shadows ReadOnly Property OriginalDefinition As TypeSymbol
			Get
				Return Me.OriginalTypeSymbolDefinition
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalTypeSymbolDefinition
			End Get
		End Property

		Protected Overridable ReadOnly Property OriginalTypeSymbolDefinition As TypeSymbol
			Get
				Return Me
			End Get
		End Property

		Friend ReadOnly Property PrimitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode
			Get
				Return SpecialTypes.GetTypeCode(Me.SpecialType)
			End Get
		End Property

		Public Overridable ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType Implements ITypeSymbol.SpecialType, ITypeSymbolInternal.SpecialType
			Get
				Return Microsoft.CodeAnalysis.SpecialType.None
			End Get
		End Property

		Public Overridable ReadOnly Property TupleElementNames As ImmutableArray(Of String)
			Get
				Return New ImmutableArray(Of String)()
			End Get
		End Property

		Public Overridable ReadOnly Property TupleElements As ImmutableArray(Of FieldSymbol)
			Get
				Return New ImmutableArray(Of FieldSymbol)()
			End Get
		End Property

		Public Overridable ReadOnly Property TupleElementTypes As ImmutableArray(Of TypeSymbol)
			Get
				Return New ImmutableArray(Of TypeSymbol)()
			End Get
		End Property

		Public Overridable ReadOnly Property TupleUnderlyingType As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind

		Shared Sub New()
			TypeSymbol.s_EmptyTypeSymbols = Array.Empty(Of TypeSymbol)()
			TypeSymbol.EmptyInterfacesAndTheirBaseInterfaces = New MultiDictionary(Of NamedTypeSymbol, NamedTypeSymbol)(0, EqualsIgnoringComparer.InstanceCLRSignatureCompare, Nothing)
			TypeSymbol.EmptyExplicitImplementationMap = New MultiDictionary(Of Symbol, Symbol)()
		End Sub

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Function AllInterfacesWithDefinitionUseSiteDiagnostics(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of NamedTypeSymbol)
			Dim allInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.AllInterfacesNoUseSiteDiagnostics
			Me.AddUseSiteDiagnosticsForBaseDefinitions(useSiteInfo)
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = allInterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End While
			Return allInterfacesNoUseSiteDiagnostics
		End Function

		Friend Function BaseTypeOriginalDefinition(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As NamedTypeSymbol
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
				baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.OriginalDefinition
				baseTypeNoUseSiteDiagnostics.AddUseSiteInfo(useSiteInfo)
			End If
			Return baseTypeNoUseSiteDiagnostics
		End Function

		Friend Function BaseTypeWithDefinitionUseSiteDiagnostics(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As NamedTypeSymbol
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
				baseTypeNoUseSiteDiagnostics.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End If
			Return baseTypeNoUseSiteDiagnostics
		End Function

		Private Function ComputeImplementationForInterfaceMember(ByVal interfaceMember As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim kind As SymbolKind = interfaceMember.Kind
			If (kind = SymbolKind.[Event]) Then
				symbol = ImplementsHelper.ComputeImplementationForInterfaceMember(Of EventSymbol)(DirectCast(interfaceMember, EventSymbol), Me, EventSignatureComparer.RuntimeEventSignatureComparer)
			ElseIf (kind = SymbolKind.Method) Then
				symbol = ImplementsHelper.ComputeImplementationForInterfaceMember(Of MethodSymbol)(DirectCast(interfaceMember, MethodSymbol), Me, MethodSignatureComparer.RuntimeMethodSignatureComparer)
			ElseIf (kind = SymbolKind.[Property]) Then
				symbol = ImplementsHelper.ComputeImplementationForInterfaceMember(Of PropertySymbol)(DirectCast(interfaceMember, PropertySymbol), Me, PropertySignatureComparer.RuntimePropertySignatureComparer)
			Else
				symbol = Nothing
			End If
			Return symbol
		End Function

		<Obsolete("Use TypeWithModifiers.Is method.", True)>
		Friend Function Equals(ByVal other As TypeWithModifiers) As Boolean
			Return other.[Is](Me)
		End Function

		Public Shared Function Equals(ByVal left As TypeSymbol, ByVal right As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			If (left Is Nothing) Then
				Return right Is Nothing
			End If
			Return left.Equals(right, comparison)
		End Function

		Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TypeSymbol), TypeCompareKind.ConsiderEverything)
		End Function

		Public NotOverridable Overrides Function Equals(ByVal other As Symbol, ByVal compareKind As TypeCompareKind) As Boolean
			Return Me.Equals(TryCast(other, TypeSymbol), compareKind)
		End Function

		Public MustOverride Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean

		Public Function FindImplementationForInterfaceMember(ByVal interfaceMember As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			If (interfaceMember Is Nothing) Then
				Throw New ArgumentNullException("interfaceMember")
			End If
			If (interfaceMember.RequiresImplementation() AndAlso Not Me.IsInterfaceType()) Then
				Dim containingType As NamedTypeSymbol = interfaceMember.ContainingType
				Dim instanceCLRSignatureCompare As EqualsIgnoringComparer = EqualsIgnoringComparer.InstanceCLRSignatureCompare
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				If (Not Me.ImplementsInterface(containingType, instanceCLRSignatureCompare, discarded)) Then
					symbol = Nothing
					Return symbol
				End If
				Dim implementationForInterfaceMemberMap As ConcurrentDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol) = Me.ImplementationForInterfaceMemberMap
				Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
				If (Not implementationForInterfaceMemberMap.TryGetValue(interfaceMember, symbol1)) Then
					symbol1 = Me.ComputeImplementationForInterfaceMember(interfaceMember)
					implementationForInterfaceMemberMap.TryAdd(interfaceMember, symbol1)
					symbol = symbol1
					Return symbol
				Else
					symbol = symbol1
					Return symbol
				End If
			End If
			symbol = Nothing
			Return symbol
		End Function

		Friend Overridable Function GetDirectBaseTypeNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As NamedTypeSymbol
			Return Me.BaseTypeNoUseSiteDiagnostics
		End Function

		Friend Overridable Function GetDirectBaseTypeWithDefinitionUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As NamedTypeSymbol
			Dim directBaseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.GetDirectBaseTypeNoUseSiteDiagnostics(basesBeingResolved)
			If (directBaseTypeNoUseSiteDiagnostics IsNot Nothing) Then
				directBaseTypeNoUseSiteDiagnostics.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End If
			Return directBaseTypeNoUseSiteDiagnostics
		End Function

		Public Overrides MustOverride Function GetHashCode() As Integer

		Friend MustOverride Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo

		Friend MustOverride Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers

		Public Overridable Function IsTupleCompatible(<Out> ByRef tupleCardinality As Integer) As Boolean
			tupleCardinality = 0
			Return False
		End Function

		Public Function IsTupleCompatible() As Boolean
			Dim num As Integer
			Return Me.IsTupleCompatible(num)
		End Function

		Public Function IsTupleOrCompatibleWithTupleOfCardinality(ByVal targetCardinality As Integer) As Boolean
			Dim length As Boolean
			Dim num As Integer
			If (Not Me.IsTupleType) Then
				length = If(Not Me.IsTupleCompatible(num), False, num = targetCardinality)
			Else
				length = Me.TupleElementTypes.Length = targetCardinality
			End If
			Return length
		End Function

		Private Function ITypeSymbol_FindImplementationForInterfaceMember(ByVal interfaceMember As ISymbol) As ISymbol Implements ITypeSymbol.FindImplementationForInterfaceMember
			If (Not TypeOf interfaceMember Is Symbol) Then
				Return Nothing
			End If
			Return Me.FindImplementationForInterfaceMember(DirectCast(interfaceMember, Symbol))
		End Function

		Private Function ITypeSymbol_ToDisplayParts(ByVal topLevelNullability As NullableFlowState, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart) Implements ITypeSymbol.ToDisplayParts
			Return MyBase.ToDisplayParts(format)
		End Function

		Private Function ITypeSymbol_ToDisplayString(ByVal topLevelNullability As NullableFlowState, Optional ByVal format As SymbolDisplayFormat = Nothing) As String Implements ITypeSymbol.ToDisplayString
			Return MyBase.ToDisplayString(format)
		End Function

		Private Function ITypeSymbol_ToMinimalDisplayParts(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal topLevelNullability As NullableFlowState, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart) Implements ITypeSymbol.ToMinimalDisplayParts
			Return MyBase.ToMinimalDisplayParts(semanticModel, position, format)
		End Function

		Private Function ITypeSymbol_ToMinimalDisplayString(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal topLevelNullability As NullableFlowState, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As String Implements ITypeSymbol.ToMinimalDisplayString
			Return MyBase.ToMinimalDisplayString(semanticModel, position, format)
		End Function

		Private Function ITypeSymbol_WithNullability(ByVal nullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation) As ITypeSymbol Implements ITypeSymbol.WithNullableAnnotation
			Return Me
		End Function

		Private Function ITypeSymbolInternal_GetITypeSymbol() As ITypeSymbol Implements ITypeSymbolInternal.GetITypeSymbol
			Return Me
		End Function

		Friend Overridable Function LookupMetadataType(ByRef emittedTypeName As MetadataTypeName) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim nested As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim typeMembers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			If (Me.Kind <> SymbolKind.ErrorType) Then
				If (emittedTypeName.IsMangled AndAlso (emittedTypeName.ForcedArity = -1 OrElse emittedTypeName.ForcedArity = emittedTypeName.InferredArity)) Then
					typeMembers = Me.GetTypeMembers(emittedTypeName.UnmangledTypeName)
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
						If (emittedTypeName.InferredArity <> current.Arity OrElse Not current.MangleName OrElse Not [String].Equals(current.Name, emittedTypeName.UnmangledTypeName, StringComparison.Ordinal)) Then
							Continue While
						End If
						If (namedTypeSymbol Is Nothing) Then
							namedTypeSymbol = current
						Else
							namedTypeSymbol = Nothing
							Exit While
						End If
					End While
				End If
				Dim forcedArity As Integer = emittedTypeName.ForcedArity
				If (emittedTypeName.UseCLSCompliantNameArityEncoding) Then
					If (emittedTypeName.InferredArity > 0) Then
						If (namedTypeSymbol IsNot Nothing) Then
							nested = namedTypeSymbol
						Else
							nested = New MissingMetadataTypeSymbol.Nested(DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), emittedTypeName)
						End If
						Return nested
					ElseIf (forcedArity = -1) Then
						forcedArity = 0
					ElseIf (forcedArity <> 0) Then
						If (namedTypeSymbol IsNot Nothing) Then
							nested = namedTypeSymbol
						Else
							nested = New MissingMetadataTypeSymbol.Nested(DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), emittedTypeName)
						End If
						Return nested
					End If
				End If
				typeMembers = Me.GetTypeMembers(emittedTypeName.TypeName)
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
					If (current1.MangleName OrElse forcedArity <> -1 AndAlso forcedArity <> current1.Arity OrElse Not [String].Equals(current1.Name, emittedTypeName.TypeName, StringComparison.Ordinal)) Then
						Continue While
					End If
					If (namedTypeSymbol Is Nothing) Then
						namedTypeSymbol = current1
					Else
						namedTypeSymbol = Nothing
						Exit While
					End If
				End While
			End If
			If (namedTypeSymbol IsNot Nothing) Then
				nested = namedTypeSymbol
			Else
				nested = New MissingMetadataTypeSymbol.Nested(DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), emittedTypeName)
			End If
			Return nested
		End Function

		Protected Overridable Function MakeAllInterfaces() As ImmutableArray(Of NamedTypeSymbol)
			Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
			Dim namedTypeSymbols As HashSet(Of NamedTypeSymbol) = New HashSet(Of NamedTypeSymbol)()
			Dim baseTypeNoUseSiteDiagnostics As TypeSymbol = Me
			While baseTypeNoUseSiteDiagnostics IsNot Nothing
				Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = baseTypeNoUseSiteDiagnostics.InterfacesNoUseSiteDiagnostics
				For i As Integer = interfacesNoUseSiteDiagnostics.Length - 1 To 0 Step -1
					TypeSymbol.MakeAllInterfacesInternal(interfacesNoUseSiteDiagnostics(i), namedTypeSymbols, instance)
				Next

				baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
			End While
			instance.ReverseContents()
			Return instance.ToImmutableAndFree()
		End Function

		Private Shared Sub MakeAllInterfacesInternal(ByVal i As NamedTypeSymbol, ByVal visited As HashSet(Of NamedTypeSymbol), ByVal result As ArrayBuilder(Of NamedTypeSymbol))
			If (visited.Add(i)) Then
				Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = i.InterfacesNoUseSiteDiagnostics
				For num As Integer = interfacesNoUseSiteDiagnostics.Length - 1 To 0 Step -1
					TypeSymbol.MakeAllInterfacesInternal(interfacesNoUseSiteDiagnostics(num), visited, result)
				Next

				result.Add(i)
			End If
		End Sub

		Private Function MakeExplicitInterfaceImplementationMap() As MultiDictionary(Of Symbol, Symbol)
			Dim emptyExplicitImplementationMap As MultiDictionary(Of Symbol, Symbol)
			If (Me.IsClassType() OrElse Me.IsStructureType()) Then
				Dim symbols As MultiDictionary(Of Symbol, Symbol) = New MultiDictionary(Of Symbol, Symbol)(TypeSymbol.ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance)
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembersUnordered().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = ImplementsHelper.GetExplicitInterfaceImplementations(current).GetEnumerator()
					While enumerator1.MoveNext()
						symbols.Add(enumerator1.Current, current)
					End While
				End While
				emptyExplicitImplementationMap = If(symbols.Count <= 0, TypeSymbol.EmptyExplicitImplementationMap, symbols)
			Else
				emptyExplicitImplementationMap = TypeSymbol.EmptyExplicitImplementationMap
			End If
			Return emptyExplicitImplementationMap
		End Function

		Private Shared Function MakeInterfacesAndTheirBaseInterfaces(ByVal declaredInterfaces As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)) As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim emptyInterfacesAndTheirBaseInterfaces As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (Not declaredInterfaces.IsEmpty) Then
				Dim namedTypeSymbols As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(declaredInterfaces.Length, EqualsIgnoringComparer.InstanceCLRSignatureCompare, Nothing)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = declaredInterfaces.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
					If (Not namedTypeSymbols.Add(current, current)) Then
						Continue While
					End If
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator()
					While enumerator1.MoveNext()
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
						namedTypeSymbols.Add(namedTypeSymbol, namedTypeSymbol)
					End While
				End While
				emptyInterfacesAndTheirBaseInterfaces = namedTypeSymbols
			Else
				emptyInterfacesAndTheirBaseInterfaces = TypeSymbol.EmptyInterfacesAndTheirBaseInterfaces
			End If
			Return emptyInterfacesAndTheirBaseInterfaces
		End Function

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator =(ByVal left As TypeSymbol, ByVal right As TypeSymbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator =(ByVal left As Symbol, ByVal right As TypeSymbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator =(ByVal left As TypeSymbol, ByVal right As Symbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator <>(ByVal left As TypeSymbol, ByVal right As TypeSymbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator <>(ByVal left As Symbol, ByVal right As TypeSymbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		<Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", True)>
		Public Shared Operator <>(ByVal left As TypeSymbol, ByVal right As Symbol) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Operator

		Protected Class ExplicitInterfaceImplementationTargetMemberEqualityComparer
			Implements IEqualityComparer(Of Symbol)
			Public ReadOnly Shared Instance As TypeSymbol.ExplicitInterfaceImplementationTargetMemberEqualityComparer

			Shared Sub New()
				TypeSymbol.ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance = New TypeSymbol.ExplicitInterfaceImplementationTargetMemberEqualityComparer()
			End Sub

			Private Sub New()
				MyBase.New()
			End Sub

			Public Function Equals(ByVal x As Symbol, ByVal y As Symbol) As Boolean Implements IEqualityComparer(Of Symbol).Equals
				If (x.OriginalDefinition <> y.OriginalDefinition) Then
					Return False
				End If
				Return EqualsIgnoringComparer.InstanceCLRSignatureCompare.Equals(x.ContainingType, y.ContainingType)
			End Function

			Public Function GetHashCode(ByVal obj As Symbol) As Integer Implements IEqualityComparer(Of Symbol).GetHashCode
				Return obj.OriginalDefinition.GetHashCode()
			End Function
		End Class
	End Class
End Namespace