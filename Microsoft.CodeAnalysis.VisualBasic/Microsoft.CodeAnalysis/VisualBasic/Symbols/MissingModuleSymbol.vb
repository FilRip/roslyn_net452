Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.PortableExecutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MissingModuleSymbol
		Inherits ModuleSymbol
		Protected ReadOnly m_Assembly As AssemblySymbol

		Protected ReadOnly m_Ordinal As Integer

		Protected ReadOnly m_GlobalNamespace As MissingNamespaceSymbol

		Friend Overrides ReadOnly Property Bit32Required As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me.m_Assembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.m_Assembly
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultMarshallingCharSet As Nullable(Of CharSet)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				Return Me.m_GlobalNamespace
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyCompilationRelaxationsAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyRuntimeCompatibilityAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasUnifiedReferences As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMissing As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine
			Get
				Return System.Reflection.PortableExecutable.Machine.I386
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return "<Missing Module>"
			End Get
		End Property

		Friend Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return SpecializedCollections.EmptyCollection(Of String)()
			End Get
		End Property

		Friend Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me.m_Ordinal
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return SpecializedCollections.EmptyCollection(Of String)()
			End Get
		End Property

		Public Sub New(ByVal assembly As AssemblySymbol, ByVal ordinal As Integer)
			MyBase.New()
			Me.m_Assembly = assembly
			Me.m_Ordinal = ordinal
			Me.m_GlobalNamespace = New MissingNamespaceSymbol(Me)
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim missingModuleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingModuleSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingModuleSymbol)
				flag = If(missingModuleSymbol Is Nothing, False, Me.m_Assembly.Equals(missingModuleSymbol.m_Assembly))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.m_Assembly.GetHashCode()
		End Function

		Public Overrides Function GetMetadata() As ModuleMetadata
			Return Nothing
		End Function

		Friend Overrides Function GetReferencedAssemblies() As ImmutableArray(Of AssemblyIdentity)
			Return ImmutableArray(Of AssemblyIdentity).Empty
		End Function

		Friend Overrides Function GetReferencedAssemblySymbols() As ImmutableArray(Of AssemblySymbol)
			Return ImmutableArray(Of AssemblySymbol).Empty
		End Function

		Friend Overrides Function GetUnificationUseSiteErrorInfo(ByVal dependentType As TypeSymbol) As DiagnosticInfo
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function LookupTopLevelMetadataType(ByRef emittedName As MetadataTypeName) As NamedTypeSymbol
			Return New MissingMetadataTypeSymbol.TopLevel(Me, emittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
		End Function

		Friend Overrides Sub SetReferences(ByVal moduleReferences As ModuleReferences(Of AssemblySymbol), Optional ByVal originatingSourceAssemblyDebugOnly As SourceAssemblySymbol = Nothing)
			Throw ExceptionUtilities.Unreachable
		End Sub
	End Class
End Namespace