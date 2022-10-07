Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class NonMissingModuleSymbol
		Inherits ModuleSymbol
		Private _moduleReferences As ModuleReferences(Of AssemblySymbol)

		Friend Overrides ReadOnly Property HasUnifiedReferences As Boolean
			Get
				Return Me.GetUnifiedAssemblies().Length > 0
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMissing As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
				If (containingAssembly.Modules(0) <> Me) Then
					Return False
				End If
				Return containingAssembly.MightContainExtensionMethods
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		<Conditional("DEBUG")>
		Friend Sub AssertReferencesInitialized()
		End Sub

		<Conditional("DEBUG")>
		Friend Sub AssertReferencesUninitialized()
		End Sub

		Friend NotOverridable Overrides Function GetReferencedAssemblies() As ImmutableArray(Of AssemblyIdentity)
			Return Me._moduleReferences.Identities
		End Function

		Friend NotOverridable Overrides Function GetReferencedAssemblySymbols() As ImmutableArray(Of AssemblySymbol)
			Return Me._moduleReferences.Symbols
		End Function

		Friend Overrides Function GetUnificationUseSiteErrorInfo(ByVal dependentType As TypeSymbol) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			If (Me.HasUnifiedReferences) Then
				Dim containingAssembly As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = Me.ContainingAssembly
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = dependentType.ContainingAssembly
				If (CObj(containingAssembly) <> CObj(assemblySymbol)) Then
					Dim enumerator As ImmutableArray(Of UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)).Enumerator = Me.GetUnifiedAssemblies().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As UnifiedAssembly(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol) = enumerator.Current
						If (current.TargetAssembly <> assemblySymbol) Then
							Continue While
						End If
						Dim originalReference As AssemblyIdentity = current.OriginalReference
						Dim identity As AssemblyIdentity = assemblySymbol.Identity
						If (identity.Version >= originalReference.Version) Then
							Continue While
						End If
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_SxSIndirectRefHigherThanDirectRef3, New [Object]() { identity.Name, originalReference.Version.ToString(), identity.Version.ToString() })
						Return diagnosticInfo
					End While
					diagnosticInfo = Nothing
				Else
					diagnosticInfo = Nothing
				End If
			Else
				diagnosticInfo = Nothing
			End If
			Return diagnosticInfo
		End Function

		Friend Function GetUnifiedAssemblies() As ImmutableArray(Of UnifiedAssembly(Of AssemblySymbol))
			Return Me._moduleReferences.UnifiedAssemblies
		End Function

		Friend NotOverridable Overrides Function LookupTopLevelMetadataType(ByRef emittedName As MetadataTypeName) As NamedTypeSymbol
			Dim topLevel As NamedTypeSymbol = Nothing
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GlobalNamespace.LookupNestedNamespace(emittedName.NamespaceSegments)
			If (namespaceSymbol IsNot Nothing) Then
				topLevel = namespaceSymbol.LookupMetadataType(emittedName)
			Else
				topLevel = New MissingMetadataTypeSymbol.TopLevel(Me, emittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
			End If
			Return topLevel
		End Function

		Friend Overrides Sub SetReferences(ByVal moduleReferences As ModuleReferences(Of AssemblySymbol), Optional ByVal originatingSourceAssemblyDebugOnly As SourceAssemblySymbol = Nothing)
			Me._moduleReferences = moduleReferences
		End Sub
	End Class
End Namespace