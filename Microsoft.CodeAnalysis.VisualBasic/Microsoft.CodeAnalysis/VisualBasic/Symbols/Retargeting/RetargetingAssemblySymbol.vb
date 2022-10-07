Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingAssemblySymbol
		Inherits NonMissingAssemblySymbol
		Private ReadOnly _underlyingAssembly As SourceAssemblySymbol

		Private ReadOnly _modules As ImmutableArray(Of ModuleSymbol)

		Private _noPiaResolutionAssemblies As ImmutableArray(Of AssemblySymbol)

		Private _linkedReferencedAssemblies As ImmutableArray(Of AssemblySymbol)

		Friend ReadOnly m_NoPiaUnificationMap As ConcurrentDictionary(Of NamedTypeSymbol, NamedTypeSymbol)

		Private ReadOnly _isLinked As Boolean

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Public Overrides ReadOnly Property AssemblyVersionPattern As Version
			Get
				Return Me._underlyingAssembly.AssemblyVersionPattern
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Identity As AssemblyIdentity
			Get
				Return Me._underlyingAssembly.Identity
			End Get
		End Property

		Friend Overrides ReadOnly Property IsLinked As Boolean
			Get
				Return Me._isLinked
			End Get
		End Property

		Friend Overrides ReadOnly Property KeepLookingForDeclaredSpecialTypes As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingAssembly.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me._underlyingAssembly.MightContainExtensionMethods
			End Get
		End Property

		Public Overrides ReadOnly Property Modules As ImmutableArray(Of ModuleSymbol)
			Get
				Return Me._modules
			End Get
		End Property

		Public Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return Me._underlyingAssembly.NamespaceNames
			End Get
		End Property

		Friend Overrides ReadOnly Property PublicKey As ImmutableArray(Of Byte)
			Get
				Return Me._underlyingAssembly.PublicKey
			End Get
		End Property

		Friend ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return DirectCast(Me.Modules(0), RetargetingModuleSymbol).RetargetingTranslator
			End Get
		End Property

		Public Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return Me._underlyingAssembly.TypeNames
			End Get
		End Property

		Public ReadOnly Property UnderlyingAssembly As SourceAssemblySymbol
			Get
				Return Me._underlyingAssembly
			End Get
		End Property

		Public Sub New(ByVal underlyingAssembly As SourceAssemblySymbol, ByVal isLinked As Boolean)
			MyBase.New()
			Me.m_NoPiaUnificationMap = New ConcurrentDictionary(Of NamedTypeSymbol, NamedTypeSymbol)()
			Me._underlyingAssembly = underlyingAssembly
			Dim modules As ImmutableArray(Of ModuleSymbol) = underlyingAssembly.Modules
			Dim retargetingModuleSymbol(modules.Length - 1 + 1 - 1) As ModuleSymbol
			modules = underlyingAssembly.Modules
			retargetingModuleSymbol(0) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingModuleSymbol(Me, DirectCast(modules(0), SourceModuleSymbol))
			modules = underlyingAssembly.Modules
			Dim length As Integer = modules.Length - 1
			Dim num As Integer = 1
			Do
				modules = underlyingAssembly.Modules
				Dim item As PEModuleSymbol = DirectCast(modules(num), PEModuleSymbol)
				retargetingModuleSymbol(num) = New PEModuleSymbol(Me, item.[Module], item.ImportOptions, num)
				num = num + 1
			Loop While num <= length
			Me._modules = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ModuleSymbol)(retargetingModuleSymbol)
			Me._isLinked = isLinked
		End Sub

		Friend Overrides Function AreInternalsVisibleToThisAssembly(ByVal potentialGiverOfAccess As AssemblySymbol) As Boolean
			Return Me._underlyingAssembly.AreInternalsVisibleToThisAssembly(potentialGiverOfAccess)
		End Function

		Friend Overrides Function GetAllTopLevelForwardedTypes() As IEnumerable(Of NamedTypeSymbol)
			Return New RetargetingAssemblySymbol.VB$StateMachine_45_GetAllTopLevelForwardedTypes(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingAssembly, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetDeclaredSpecialType(ByVal type As SpecialType) As NamedTypeSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingAssembly.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetGuidString(ByRef guidString As String) As Boolean
			Return Me._underlyingAssembly.GetGuidString(guidString)
		End Function

		Friend Overrides Function GetInternalsVisibleToPublicKeys(ByVal simpleName As String) As IEnumerable(Of ImmutableArray(Of Byte))
			Return Me._underlyingAssembly.GetInternalsVisibleToPublicKeys(simpleName)
		End Function

		Friend Overrides Function GetLinkedReferencedAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return Me._linkedReferencedAssemblies
		End Function

		Public Overrides Function GetMetadata() As AssemblyMetadata
			Return Me._underlyingAssembly.GetMetadata()
		End Function

		Friend Overrides Function GetNoPiaResolutionAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return Me._noPiaResolutionAssemblies
		End Function

		Friend Overrides Sub SetLinkedReferencedAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Me._linkedReferencedAssemblies = assemblies
		End Sub

		Friend Overrides Sub SetNoPiaResolutionAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Me._noPiaResolutionAssemblies = assemblies
		End Sub

		Friend Overrides Function TryLookupForwardedMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal ignoreCase As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.UnderlyingAssembly.TryLookupForwardedMetadataType(emittedName, ignoreCase)
			If (namedTypeSymbol1 IsNot Nothing) Then
				namedTypeSymbol = Me.RetargetingTranslator.Retarget(namedTypeSymbol1, RetargetOptions.RetargetPrimitiveTypesByName)
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function
	End Class
End Namespace