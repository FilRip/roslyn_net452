Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEModuleSymbol
		Inherits NonMissingModuleSymbol
		Private ReadOnly _assemblySymbol As AssemblySymbol

		Private ReadOnly _ordinal As Integer

		Private ReadOnly _module As PEModule

		Private ReadOnly _globalNamespace As PENamespaceSymbol

		Private _lazySystemTypeSymbol As NamedTypeSymbol

		Private Const s_defaultTypeMapCapacity As Integer = 31

		Friend ReadOnly TypeHandleToTypeMap As ConcurrentDictionary(Of TypeDefinitionHandle, TypeSymbol)

		Friend ReadOnly TypeRefHandleToTypeMap As ConcurrentDictionary(Of TypeReferenceHandle, TypeSymbol)

		Friend ReadOnly MetadataLocation As ImmutableArray(Of Microsoft.CodeAnalysis.MetadataLocation)

		Friend ReadOnly ImportOptions As MetadataImportOptions

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyAssemblyAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyTypeNames As ICollection(Of String)

		Private _lazyNamespaceNames As ICollection(Of String)

		Friend Overrides ReadOnly Property Bit32Required As Boolean
			Get
				Return Me._module.Bit32Required
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._assemblySymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._assemblySymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultMarshallingCharSet As Nullable(Of CharSet)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend ReadOnly Property DocumentationProvider As Microsoft.CodeAnalysis.DocumentationProvider
			Get
				Dim containingAssembly As PEAssemblySymbol = TryCast(Me.ContainingAssembly, PEAssemblySymbol)
				Return If(containingAssembly Is Nothing, Microsoft.CodeAnalysis.DocumentationProvider.[Default], containingAssembly.DocumentationProvider)
			End Get
		End Property

		Public Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				Return Me._globalNamespace
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyCompilationRelaxationsAttribute As Boolean
			Get
				Return Me.GetAssemblyAttributes().IndexOfAttribute(Me, AttributeDescription.CompilationRelaxationsAttribute) >= 0
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyRuntimeCompatibilityAttribute As Boolean
			Get
				Return Me.GetAssemblyAttributes().IndexOfAttribute(Me, AttributeDescription.RuntimeCompatibilityAttribute) >= 0
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of Microsoft.CodeAnalysis.MetadataLocation)(Me.MetadataLocation)
			End Get
		End Property

		Friend Overrides ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine
			Get
				Return Me._module.Machine
			End Get
		End Property

		Friend ReadOnly Property [Module] As PEModule
			Get
				Return Me._module
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._module.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				If (Me._lazyNamespaceNames Is Nothing) Then
					Interlocked.CompareExchange(Of ICollection(Of String))(Me._lazyNamespaceNames, Me._module.NamespaceNames.AsCaseInsensitiveCollection(), Nothing)
				End If
				Return Me._lazyNamespaceNames
			End Get
		End Property

		Friend Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Friend ReadOnly Property SystemTypeSymbol As NamedTypeSymbol
			Get
				If (Me._lazySystemTypeSymbol Is Nothing) Then
					Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._lazySystemTypeSymbol, Me.GetWellKnownType(WellKnownType.System_Type), Nothing)
				End If
				Return Me._lazySystemTypeSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				If (Me._lazyTypeNames Is Nothing) Then
					Interlocked.CompareExchange(Of ICollection(Of String))(Me._lazyTypeNames, Me._module.TypeNames.AsCaseInsensitiveCollection(), Nothing)
				End If
				Return Me._lazyTypeNames
			End Get
		End Property

		Friend Sub New(ByVal assemblySymbol As PEAssemblySymbol, ByVal [module] As PEModule, ByVal importOptions As MetadataImportOptions, ByVal ordinal As Integer)
			MyClass.New(DirectCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), [module], importOptions, ordinal)
		End Sub

		Friend Sub New(ByVal assemblySymbol As SourceAssemblySymbol, ByVal [module] As PEModule, ByVal importOptions As MetadataImportOptions, ByVal ordinal As Integer)
			MyClass.New(DirectCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), [module], importOptions, ordinal)
		End Sub

		Friend Sub New(ByVal assemblySymbol As RetargetingAssemblySymbol, ByVal [module] As PEModule, ByVal importOptions As MetadataImportOptions, ByVal ordinal As Integer)
			MyClass.New(DirectCast(assemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), [module], importOptions, ordinal)
		End Sub

		Private Sub New(ByVal assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, ByVal [module] As PEModule, ByVal importOptions As MetadataImportOptions, ByVal ordinal As Integer)
			MyBase.New()
			Me.TypeHandleToTypeMap = New ConcurrentDictionary(Of TypeDefinitionHandle, TypeSymbol)(2, 31)
			Me.TypeRefHandleToTypeMap = New ConcurrentDictionary(Of TypeReferenceHandle, TypeSymbol)(2, 31)
			Me.MetadataLocation = ImmutableArray.Create(Of Microsoft.CodeAnalysis.MetadataLocation)(New Microsoft.CodeAnalysis.MetadataLocation(Me))
			Me._assemblySymbol = assemblySymbol
			Me._ordinal = ordinal
			Me._module = [module]
			Me._globalNamespace = New PEGlobalNamespaceSymbol(Me)
			Me.ImportOptions = importOptions
		End Sub

		Friend Function GetAssembliesForForwardedType(ByRef fullName As MetadataTypeName, ByVal ignoreCase As Boolean, <Out> ByRef matchedName As String) As <TupleElementNames(New String() { "FirstSymbol", "SecondSymbol" })> ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			Dim valueTuple As ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			Dim assemblyRefsForForwardedType As ValueTuple(Of Integer, Integer) = Me.[Module].GetAssemblyRefsForForwardedType(fullName.FullName, ignoreCase, matchedName)
			If (assemblyRefsForForwardedType.Item1 >= 0) Then
				Dim referencedAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = MyBase.GetReferencedAssemblySymbol(assemblyRefsForForwardedType.Item1)
				If (assemblyRefsForForwardedType.Item2 >= 0) Then
					Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = MyBase.GetReferencedAssemblySymbol(assemblyRefsForForwardedType.Item2)
					valueTuple = New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(referencedAssemblySymbol, assemblySymbol)
				Else
					valueTuple = New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(referencedAssemblySymbol, Nothing)
				End If
			Else
				valueTuple = New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)(Nothing, Nothing)
			End If
			Return valueTuple
		End Function

		Friend Function GetAssemblyAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim enumerator As CustomAttributeHandleCollection.Enumerator = New CustomAttributeHandleCollection.Enumerator()
			Dim immutableAndFree As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyAssemblyAttributes.IsDefault) Then
				Dim instance As ArrayBuilder(Of VisualBasicAttributeData) = Nothing
				Dim name As String = Me.ContainingAssembly.CorLibrary.Name
				Dim assemblyRef As EntityHandle = Me.[Module].GetAssemblyRef(name)
				If (Not assemblyRef.IsNil) Then
					Dim enumerator1 As IEnumerator = MetadataWriter.dummyAssemblyAttributeParentQualifier.GetEnumerator()
					While enumerator1.MoveNext()
						Dim str As String = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(enumerator1.Current)
						Dim typeRef As EntityHandle = Me.[Module].GetTypeRef(assemblyRef, "System.Runtime.CompilerServices", [String].Concat("AssemblyAttributesGoHere", str))
						If (typeRef.IsNil) Then
							Continue While
						End If
						Try
							Try
								enumerator = Me.[Module].GetCustomAttributesOrThrow(typeRef).GetEnumerator()
								While enumerator.MoveNext()
									Dim current As CustomAttributeHandle = enumerator.Current
									If (instance Is Nothing) Then
										instance = ArrayBuilder(Of VisualBasicAttributeData).GetInstance()
									End If
									instance.Add(New PEAttributeData(Me, current))
								End While
							Finally
								DirectCast(enumerator, IDisposable).Dispose()
							End Try
						Catch badImageFormatException As System.BadImageFormatException
							ProjectData.SetProjectError(badImageFormatException)
							ProjectData.ClearProjectError()
						End Try
					End While
				End If
				If (instance IsNot Nothing) Then
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = ImmutableArray(Of VisualBasicAttributeData).Empty
				End If
				Dim visualBasicAttributeDatas As ImmutableArray(Of VisualBasicAttributeData) = New ImmutableArray(Of VisualBasicAttributeData)()
				ImmutableInterlocked.InterlockedCompareExchange(Of VisualBasicAttributeData)(Me._lazyAssemblyAttributes, immutableAndFree, visualBasicAttributeDatas)
			End If
			Return Me._lazyAssemblyAttributes
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributes.IsDefault) Then
				Me.LoadCustomAttributes(EntityHandle.ModuleDefinition, Me._lazyCustomAttributes)
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Function GetCustomAttributesForToken(ByVal token As EntityHandle) As ImmutableArray(Of VisualBasicAttributeData)
			Dim customAttributeHandle As System.Reflection.Metadata.CustomAttributeHandle = New System.Reflection.Metadata.CustomAttributeHandle()
			Dim attributeDescription As Microsoft.CodeAnalysis.AttributeDescription = New Microsoft.CodeAnalysis.AttributeDescription()
			Dim attributeDescription1 As Microsoft.CodeAnalysis.AttributeDescription = attributeDescription
			Dim customAttributeHandle1 As System.Reflection.Metadata.CustomAttributeHandle = New System.Reflection.Metadata.CustomAttributeHandle()
			attributeDescription = New Microsoft.CodeAnalysis.AttributeDescription()
			Return Me.GetCustomAttributesForToken(token, customAttributeHandle, attributeDescription1, customAttributeHandle1, attributeDescription)
		End Function

		Friend Function GetCustomAttributesForToken(ByVal token As EntityHandle, <Out> ByRef filteredOutAttribute1 As CustomAttributeHandle, ByVal filterOut1 As AttributeDescription, <Out> Optional ByRef filteredOutAttribute2 As CustomAttributeHandle = Nothing, Optional ByVal filterOut2 As AttributeDescription = Nothing) As ImmutableArray(Of VisualBasicAttributeData)
			Dim empty As ImmutableArray(Of VisualBasicAttributeData)
			Dim enumerator As CustomAttributeHandleCollection.Enumerator = New CustomAttributeHandleCollection.Enumerator()
			Dim instance As ArrayBuilder(Of VisualBasicAttributeData) = Nothing
			filteredOutAttribute1 = New CustomAttributeHandle()
			filteredOutAttribute2 = New CustomAttributeHandle()
			Try
				Try
					enumerator = Me.[Module].GetCustomAttributesOrThrow(token).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As CustomAttributeHandle = enumerator.Current
						If (instance Is Nothing) Then
							instance = ArrayBuilder(Of VisualBasicAttributeData).GetInstance()
						End If
						If (filterOut1.Signatures IsNot Nothing AndAlso Me.[Module].GetTargetAttributeSignatureIndex(current, filterOut1) <> -1) Then
							filteredOutAttribute1 = current
						ElseIf (filterOut2.Signatures Is Nothing OrElse Me.[Module].GetTargetAttributeSignatureIndex(current, filterOut2) = -1) Then
							instance.Add(New PEAttributeData(Me, current))
						Else
							filteredOutAttribute2 = current
						End If
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				ProjectData.ClearProjectError()
			End Try
			If (instance Is Nothing) Then
				empty = ImmutableArray(Of VisualBasicAttributeData).Empty
			Else
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function

		Public Function GetEventRegistrationTokenType() As NamedTypeSymbol
			Return Me.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
		End Function

		Friend Function GetForwardedTypes() As IEnumerable(Of NamedTypeSymbol)
			Return New PEModuleSymbol.VB$StateMachine_64_GetForwardedTypes(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Function GetHash(ByVal algorithmId As AssemblyHashAlgorithm) As ImmutableArray(Of Byte)
			Return Me._module.GetHash(algorithmId)
		End Function

		Public Overrides Function GetMetadata() As ModuleMetadata
			Return Me._module.GetNonDisposableMetadata()
		End Function

		Private Function GetReferencedAssemblySymbol(ByVal assemblyRef As AssemblyReferenceHandle) As AssemblySymbol
			Dim referencedAssemblySymbol As AssemblySymbol
			Dim assemblyReferenceIndexOrThrow As Integer
			Try
				assemblyReferenceIndexOrThrow = Me.[Module].GetAssemblyReferenceIndexOrThrow(assemblyRef)
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				referencedAssemblySymbol = Nothing
				ProjectData.ClearProjectError()
				Return referencedAssemblySymbol
			End Try
			referencedAssemblySymbol = MyBase.GetReferencedAssemblySymbol(assemblyReferenceIndexOrThrow)
			Return referencedAssemblySymbol
		End Function

		Private Function GetWellKnownType(ByVal type As WellKnownType) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(type.GetMetadataName(), True, -1)
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = MyBase.LookupTopLevelMetadataType(metadataTypeName)
			If (Not PEModuleSymbol.IsAcceptableSystemTypeSymbol(namedTypeSymbol1)) Then
				Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = MyBase.GetReferencedAssemblySymbols().GetEnumerator()
				While enumerator.MoveNext()
					Dim namedTypeSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current.LookupTopLevelMetadataType(metadataTypeName, True)
					If (Not PEModuleSymbol.IsAcceptableSystemTypeSymbol(namedTypeSymbol3)) Then
						Continue While
					End If
					If (namedTypeSymbol2 IsNot Nothing) Then
						If (CObj(namedTypeSymbol2) = CObj(namedTypeSymbol3)) Then
							Continue While
						End If
						namedTypeSymbol2 = Nothing
						Exit While
					Else
						namedTypeSymbol2 = namedTypeSymbol3
					End If
				End While
				namedTypeSymbol = If(namedTypeSymbol2 Is Nothing, namedTypeSymbol1, namedTypeSymbol2)
			Else
				namedTypeSymbol = namedTypeSymbol1
			End If
			Return namedTypeSymbol
		End Function

		Private Shared Function IsAcceptableSystemTypeSymbol(ByVal candidate As NamedTypeSymbol) As Boolean
			If (candidate.Kind = SymbolKind.ErrorType) Then
				Return False
			End If
			Return Not TypeOf candidate Is MissingMetadataTypeSymbol
		End Function

		Friend Sub LoadCustomAttributes(ByVal token As EntityHandle, ByRef lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData))
			Dim customAttributesForToken As ImmutableArray(Of VisualBasicAttributeData) = Me.GetCustomAttributesForToken(token)
			ImmutableInterlocked.InterlockedCompareExchange(Of VisualBasicAttributeData)(lazyCustomAttributes, customAttributesForToken, New ImmutableArray(Of VisualBasicAttributeData)())
		End Sub

		Friend Function LookupTopLevelMetadataType(ByRef emittedName As MetadataTypeName, <Out> ByRef isNoPiaLocalType As Boolean) As NamedTypeSymbol
			Dim topLevel As NamedTypeSymbol
			Dim pENamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamespaceSymbol = DirectCast(Me.GlobalNamespace.LookupNestedNamespace(emittedName.NamespaceSegments), Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamespaceSymbol)
			If (pENamespaceSymbol IsNot Nothing) Then
				topLevel = pENamespaceSymbol.LookupMetadataType(emittedName, isNoPiaLocalType)
			Else
				isNoPiaLocalType = False
				topLevel = New MissingMetadataTypeSymbol.TopLevel(Me, emittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
			End If
			Return topLevel
		End Function

		Friend Sub OnNewTypeDeclarationsLoaded(ByVal typesDict As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)))
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of PENamedTypeSymbol)).ValueCollection.Enumerator()
			Dim keepLookingForDeclaredSpecialTypes As Boolean = If(Me._ordinal <> 0, False, Me._assemblySymbol.KeepLookingForDeclaredSpecialTypes)
			Try
				enumerator = typesDict.Values.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of PENamedTypeSymbol).Enumerator = enumerator.Current.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As PENamedTypeSymbol = enumerator1.Current
						Me.TypeHandleToTypeMap.TryAdd(current.Handle, current)
						If (Not keepLookingForDeclaredSpecialTypes OrElse current.SpecialType = SpecialType.None) Then
							Continue While
						End If
						Me._assemblySymbol.RegisterDeclaredSpecialType(current)
						keepLookingForDeclaredSpecialTypes = Me._assemblySymbol.KeepLookingForDeclaredSpecialTypes
					End While
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
		End Sub
	End Class
End Namespace