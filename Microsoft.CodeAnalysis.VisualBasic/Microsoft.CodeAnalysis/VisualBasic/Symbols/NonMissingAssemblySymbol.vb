Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class NonMissingAssemblySymbol
		Inherits AssemblySymbol
		Private ReadOnly _emittedNameToTypeMap As ConcurrentDictionary(Of MetadataTypeName.Key, NamedTypeSymbol)

		Private _lazyGlobalNamespace As NamespaceSymbol

		Friend ReadOnly Property EmittedNameToTypeMapCount As Integer
			Get
				Return Me._emittedNameToTypeMap.Count
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				If (Me._lazyGlobalNamespace Is Nothing) Then
					Interlocked.CompareExchange(Of NamespaceSymbol)(Me._lazyGlobalNamespace, MergedNamespaceSymbol.CreateGlobalNamespace(Me), Nothing)
				End If
				Return Me._lazyGlobalNamespace
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMissing As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
			Me._emittedNameToTypeMap = New ConcurrentDictionary(Of MetadataTypeName.Key, NamedTypeSymbol)()
		End Sub

		Friend Function CachedTypeByEmittedName(ByVal emittedname As String) As NamedTypeSymbol
			Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(emittedname, False, -1)
			Return Me._emittedNameToTypeMap(metadataTypeName.ToKey())
		End Function

		Private Sub CacheTopLevelMetadataType(ByRef emittedName As MetadataTypeName, ByVal result As NamedTypeSymbol)
			Me._emittedNameToTypeMap.GetOrAdd(emittedName.ToKey(), result)
		End Sub

		Private Function LookupTopLevelMetadataTypeInCache(ByRef emittedName As MetadataTypeName) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			If (Not Me._emittedNameToTypeMap.TryGetValue(emittedName.ToKey(), namedTypeSymbol1)) Then
				namedTypeSymbol = Nothing
			Else
				namedTypeSymbol = namedTypeSymbol1
			End If
			Return namedTypeSymbol
		End Function

		Friend NotOverridable Overrides Function LookupTopLevelMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal digThroughForwardedTypes As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim topLevel As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			namedTypeSymbol = Me.LookupTopLevelMetadataTypeInCache(emittedName)
			If (namedTypeSymbol Is Nothing) Then
				Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
				Dim length As Integer = modules.Length
				Dim num As Integer = 0
				namedTypeSymbol = modules(num).LookupTopLevelMetadataType(emittedName)
				If (TypeOf namedTypeSymbol Is MissingMetadataTypeSymbol) Then
					Dim num1 As Integer = length - 1
					num = 1
					While num <= num1
						Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = modules(num).LookupTopLevelMetadataType(emittedName)
						If (TypeOf namedTypeSymbol1 Is MissingMetadataTypeSymbol) Then
							num = num + 1
						Else
							namedTypeSymbol = namedTypeSymbol1
							Exit While
						End If
					End While
				End If
				Dim flag As Boolean = num < length
				If (Not flag AndAlso digThroughForwardedTypes) Then
					Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.TryLookupForwardedMetadataTypeWithCycleDetection(emittedName, visitedAssemblies, False)
					If (namedTypeSymbol2 IsNot Nothing) Then
						namedTypeSymbol = namedTypeSymbol2
					End If
				End If
				If (digThroughForwardedTypes OrElse flag) Then
					Me.CacheTopLevelMetadataType(emittedName, namedTypeSymbol)
				End If
				topLevel = namedTypeSymbol
			ElseIf (digThroughForwardedTypes OrElse Not namedTypeSymbol.IsErrorType() AndAlso namedTypeSymbol.ContainingAssembly = Me) Then
				topLevel = namedTypeSymbol
			Else
				Dim moduleSymbols As ImmutableArray(Of ModuleSymbol) = Me.Modules
				topLevel = New MissingMetadataTypeSymbol.TopLevel(moduleSymbols(0), emittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
			End If
			Return topLevel
		End Function

		Friend Overrides MustOverride Function TryLookupForwardedMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal ignoreCase As Boolean) As NamedTypeSymbol
	End Class
End Namespace