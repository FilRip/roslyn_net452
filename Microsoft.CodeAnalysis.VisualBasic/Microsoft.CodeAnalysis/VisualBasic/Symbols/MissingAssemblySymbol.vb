Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MissingAssemblySymbol
		Inherits AssemblySymbol
		Protected ReadOnly m_Identity As AssemblyIdentity

		Protected ReadOnly m_ModuleSymbol As MissingModuleSymbol

		Private _lazyModules As ImmutableArray(Of ModuleSymbol)

		Public Overrides ReadOnly Property AssemblyVersionPattern As Version
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				Return Me.m_ModuleSymbol.GlobalNamespace
			End Get
		End Property

		Public Overrides ReadOnly Property Identity As AssemblyIdentity
			Get
				Return Me.m_Identity
			End Get
		End Property

		Friend Overrides ReadOnly Property IsLinked As Boolean
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

		Public NotOverridable Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Modules As ImmutableArray(Of ModuleSymbol)
			Get
				If (Me._lazyModules.IsDefault) Then
					Me._lazyModules = ImmutableArray.Create(Of ModuleSymbol)(Me.m_ModuleSymbol)
				End If
				Return Me._lazyModules
			End Get
		End Property

		Public Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return SpecializedCollections.EmptyCollection(Of String)()
			End Get
		End Property

		Friend Overrides ReadOnly Property PublicKey As ImmutableArray(Of Byte)
			Get
				Return Me.Identity.PublicKey
			End Get
		End Property

		Public Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return SpecializedCollections.EmptyCollection(Of String)()
			End Get
		End Property

		Public Sub New(ByVal identity As AssemblyIdentity)
			MyBase.New()
			Me.m_Identity = identity
			Me.m_ModuleSymbol = New MissingModuleSymbol(Me, 0)
		End Sub

		Friend Overrides Function AreInternalsVisibleToThisAssembly(ByVal other As AssemblySymbol) As Boolean
			Return False
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, MissingAssemblySymbol))
		End Function

		Public Function Equals(ByVal other As MissingAssemblySymbol) As Boolean
			If (other Is Nothing) Then
				Return False
			End If
			If (CObj(Me) = CObj(other)) Then
				Return True
			End If
			Return Me.m_Identity.Equals(other.m_Identity)
		End Function

		Friend NotOverridable Overrides Function GetAllTopLevelForwardedTypes() As IEnumerable(Of NamedTypeSymbol)
			Return SpecializedCollections.EmptyEnumerable(Of NamedTypeSymbol)()
		End Function

		Friend Overrides Function GetDeclaredSpecialType(ByVal type As SpecialType) As NamedTypeSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetDeclaredSpecialTypeMember(ByVal member As SpecialMember) As Symbol
			Return Nothing
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.m_Identity.GetHashCode()
		End Function

		Friend Overrides Function GetInternalsVisibleToPublicKeys(ByVal simpleName As String) As IEnumerable(Of ImmutableArray(Of Byte))
			Return SpecializedCollections.EmptyEnumerable(Of ImmutableArray(Of Byte))()
		End Function

		Friend Overrides Function GetLinkedReferencedAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return ImmutableArray(Of AssemblySymbol).Empty
		End Function

		Public Overrides Function GetMetadata() As AssemblyMetadata
			Return Nothing
		End Function

		Friend Overrides Function GetNoPiaResolutionAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return ImmutableArray(Of AssemblySymbol).Empty
		End Function

		Friend Overrides Function LookupTopLevelMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal digThroughForwardedTypes As Boolean) As NamedTypeSymbol
			Return Me.m_ModuleSymbol.LookupTopLevelMetadataType(emittedName)
		End Function

		Friend Overrides Sub SetLinkedReferencedAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Sub SetNoPiaResolutionAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Throw ExceptionUtilities.Unreachable
		End Sub
	End Class
End Namespace