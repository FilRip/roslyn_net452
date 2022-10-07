Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEAssemblySymbol
		Inherits MetadataOrSourceAssemblySymbol
		Private ReadOnly _assembly As PEAssembly

		Private ReadOnly _documentationProvider As Microsoft.CodeAnalysis.DocumentationProvider

		Private ReadOnly _modules As ImmutableArray(Of ModuleSymbol)

		Private _noPiaResolutionAssemblies As ImmutableArray(Of AssemblySymbol)

		Private _linkedReferencedAssemblies As ImmutableArray(Of AssemblySymbol)

		Private ReadOnly _isLinked As Boolean

		Private _lazyMightContainExtensionMethods As Byte

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Friend ReadOnly Property Assembly As PEAssembly
			Get
				Return Me._assembly
			End Get
		End Property

		Public Overrides ReadOnly Property AssemblyVersionPattern As Version
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Friend ReadOnly Property DocumentationProvider As Microsoft.CodeAnalysis.DocumentationProvider
			Get
				Return Me._documentationProvider
			End Get
		End Property

		Public Overrides ReadOnly Property Identity As AssemblyIdentity
			Get
				Return Me._assembly.Identity
			End Get
		End Property

		Friend Overrides ReadOnly Property IsLinked As Boolean
			Get
				Return Me._isLinked
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of MetadataLocation)(Me.PrimaryModule.MetadataLocation)
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				If (Me._lazyMightContainExtensionMethods = 0) Then
					If (Not Me.PrimaryModule.[Module].HasExtensionAttribute(Me._assembly.Handle, True)) Then
						Me._lazyMightContainExtensionMethods = 1
					Else
						Me._lazyMightContainExtensionMethods = 2
					End If
				End If
				Return Me._lazyMightContainExtensionMethods = 2
			End Get
		End Property

		Public Overrides ReadOnly Property Modules As ImmutableArray(Of ModuleSymbol)
			Get
				Return Me._modules
			End Get
		End Property

		Friend ReadOnly Property PrimaryModule As PEModuleSymbol
			Get
				Return DirectCast(Me.Modules(0), PEModuleSymbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property PublicKey As ImmutableArray(Of Byte)
			Get
				Return Me._assembly.Identity.PublicKey
			End Get
		End Property

		Friend Sub New(ByVal assembly As PEAssembly, ByVal documentationProvider As Microsoft.CodeAnalysis.DocumentationProvider, ByVal isLinked As Boolean, ByVal importOptions As MetadataImportOptions)
			MyBase.New()
			Me._lazyMightContainExtensionMethods = 0
			Me._assembly = assembly
			Me._documentationProvider = documentationProvider
			Dim modules As ImmutableArray(Of PEModule) = assembly.Modules
			Dim pEModuleSymbol(modules.Length - 1 + 1 - 1) As ModuleSymbol
			modules = assembly.Modules
			Dim length As Integer = modules.Length - 1
			Dim num As Integer = 0
			Do
				modules = assembly.Modules
				pEModuleSymbol(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol(Me, modules(num), importOptions, num)
				num = num + 1
			Loop While num <= length
			Me._modules = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ModuleSymbol)(pEModuleSymbol)
			Me._isLinked = isLinked
		End Sub

		Friend Overrides Function AreInternalsVisibleToThisAssembly(ByVal potentialGiverOfAccess As AssemblySymbol) As Boolean
			Return MyBase.MakeFinalIVTDetermination(potentialGiverOfAccess) = IVTConclusion.Match
		End Function

		Friend Overrides Function GetAllTopLevelForwardedTypes() As IEnumerable(Of NamedTypeSymbol)
			Return Me.PrimaryModule.GetForwardedTypes()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributes.IsDefault) Then
				Me.PrimaryModule.LoadCustomAttributes(Me.Assembly.Handle, Me._lazyCustomAttributes)
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Overrides Function GetGuidString(ByRef guidString As String) As Boolean
			Dim modules As ImmutableArray(Of PEModule) = Me.Assembly.Modules
			Return modules(0).HasGuidAttribute(Me.Assembly.Handle, guidString)
		End Function

		Friend Overrides Function GetInternalsVisibleToPublicKeys(ByVal simpleName As String) As IEnumerable(Of ImmutableArray(Of Byte))
			Return Me.Assembly.GetInternalsVisibleToPublicKeys(simpleName)
		End Function

		Friend Overrides Function GetLinkedReferencedAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return Me._linkedReferencedAssemblies
		End Function

		Public Overrides Function GetMetadata() As AssemblyMetadata
			Return Me._assembly.GetNonDisposableMetadata()
		End Function

		Friend Overrides Function GetNoPiaResolutionAssemblies() As ImmutableArray(Of AssemblySymbol)
			Return Me._noPiaResolutionAssemblies
		End Function

		Friend Function LookupAssembliesForForwardedMetadataType(ByRef emittedName As MetadataTypeName, ByVal ignoreCase As Boolean, <Out> ByRef matchedName As String) As <TupleElementNames(New String() { "FirstSymbol", "SecondSymbol" })> ValueTuple(Of AssemblySymbol, AssemblySymbol)
			Return Me.PrimaryModule.GetAssembliesForForwardedType(emittedName, ignoreCase, matchedName)
		End Function

		Friend Overrides Sub SetLinkedReferencedAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Me._linkedReferencedAssemblies = assemblies
		End Sub

		Friend Overrides Sub SetNoPiaResolutionAssemblies(ByVal assemblies As ImmutableArray(Of AssemblySymbol))
			Me._noPiaResolutionAssemblies = assemblies
		End Sub

		Friend Overrides Function TryLookupForwardedMetadataTypeWithCycleDetection(ByRef emittedName As MetadataTypeName, ByVal visitedAssemblies As ConsList(Of AssemblySymbol), ByVal ignoreCase As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim str As String = Nothing
			Dim valueTuple As ValueTuple(Of AssemblySymbol, AssemblySymbol) = Me.LookupAssembliesForForwardedMetadataType(emittedName, ignoreCase, str)
			If (valueTuple.Item1 Is Nothing) Then
				namedTypeSymbol = Nothing
			ElseIf (valueTuple.Item2 IsNot Nothing) Then
				namedTypeSymbol = MyBase.CreateMultipleForwardingErrorTypeSymbol(emittedName, Me.PrimaryModule, valueTuple.Item1, valueTuple.Item2)
			ElseIf (visitedAssemblies Is Nothing OrElse Not visitedAssemblies.Contains(valueTuple.Item1)) Then
				Dim empty As [Object] = visitedAssemblies
				If (empty Is Nothing) Then
					empty = ConsList(Of AssemblySymbol).Empty
				End If
				visitedAssemblies = New ConsList(Of AssemblySymbol)(Me, empty)
				If (ignoreCase AndAlso Not [String].Equals(emittedName.FullName, str, StringComparison.Ordinal)) Then
					emittedName = MetadataTypeName.FromFullName(str, emittedName.UseCLSCompliantNameArityEncoding, emittedName.ForcedArity)
				End If
				namedTypeSymbol = valueTuple.Item1.LookupTopLevelMetadataTypeWithCycleDetection(emittedName, visitedAssemblies, True)
			Else
				namedTypeSymbol = MyBase.CreateCycleInTypeForwarderErrorTypeSymbol(emittedName)
			End If
			Return namedTypeSymbol
		End Function
	End Class
End Namespace