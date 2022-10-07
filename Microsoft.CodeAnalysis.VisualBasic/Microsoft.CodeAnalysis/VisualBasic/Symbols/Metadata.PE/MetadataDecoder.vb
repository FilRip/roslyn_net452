Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend Class MetadataDecoder
		Inherits MetadataDecoder(Of PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol)
		Private ReadOnly _typeContextOpt As PENamedTypeSymbol

		Private ReadOnly _methodContextOpt As PEMethodSymbol

		Friend ReadOnly Property ModuleSymbol As PEModuleSymbol
			Get
				Return Me.moduleSymbol
			End Get
		End Property

		Public Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal context As PENamedTypeSymbol)
			MyClass.New(moduleSymbol, context, Nothing)
		End Sub

		Public Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal context As PEMethodSymbol)
			MyClass.New(moduleSymbol, DirectCast(context.ContainingType, PENamedTypeSymbol), context)
		End Sub

		Public Sub New(ByVal moduleSymbol As PEModuleSymbol)
			MyClass.New(moduleSymbol, Nothing, Nothing)
		End Sub

		Private Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal typeContextOpt As PENamedTypeSymbol, ByVal methodContextOpt As PEMethodSymbol)
			MyBase.New(moduleSymbol.[Module], If(TypeOf moduleSymbol.ContainingAssembly Is PEAssemblySymbol, moduleSymbol.ContainingAssembly.Identity, Nothing), SymbolFactory.Instance, moduleSymbol)
			Me._typeContextOpt = typeContextOpt
			Me._methodContextOpt = methodContextOpt
		End Sub

		Protected Overrides Sub EnqueueTypeSymbol(ByVal typeDefsToSearch As Queue(Of TypeDefinitionHandle), ByVal typeSymbolsToSearch As Queue(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			If (typeSymbol IsNot Nothing) Then
				Dim pENamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol = TryCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol)
				If (pENamedTypeSymbol IsNot Nothing AndAlso CObj(pENamedTypeSymbol.ContainingPEModule) = CObj(Me.ModuleSymbol)) Then
					typeDefsToSearch.Enqueue(pENamedTypeSymbol.Handle)
					Return
				End If
				typeSymbolsToSearch.Enqueue(typeSymbol)
			End If
		End Sub

		Protected Overrides Sub EnqueueTypeSymbolInterfacesAndBaseTypes(ByVal typeDefsToSearch As Queue(Of TypeDefinitionHandle), ByVal typeSymbolsToSearch As Queue(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = typeSymbol.InterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Me.EnqueueTypeSymbol(typeDefsToSearch, typeSymbolsToSearch, enumerator.Current)
			End While
			Me.EnqueueTypeSymbol(typeDefsToSearch, typeSymbolsToSearch, typeSymbol.BaseTypeNoUseSiteDiagnostics)
		End Sub

		Protected Overrides Function FindFieldSymbolInType(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal fieldDef As FieldDefinitionHandle) As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = typeSymbol.GetMembersUnordered().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As PEFieldSymbol = TryCast(enumerator.Current, PEFieldSymbol)
					If (current IsNot Nothing AndAlso current.Handle = fieldDef) Then
						fieldSymbol = current
						Exit While
					End If
				Else
					fieldSymbol = Nothing
					Exit While
				End If
			End While
			Return fieldSymbol
		End Function

		Protected Overrides Function FindMethodSymbolInType(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal targetMethodDef As MethodDefinitionHandle) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = typeSymbol.GetMembersUnordered().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As PEMethodSymbol = TryCast(enumerator.Current, PEMethodSymbol)
					If (current IsNot Nothing AndAlso current.Handle = targetMethodDef) Then
						methodSymbol = current
						Exit While
					End If
				Else
					methodSymbol = Nothing
					Exit While
				End If
			End While
			Return methodSymbol
		End Function

		Protected Overrides Function GetGenericMethodTypeParamSymbol(ByVal position As Integer) As TypeSymbol
			Dim item As TypeSymbol
			If (Me._methodContextOpt IsNot Nothing) Then
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = Me._methodContextOpt.TypeParameters
				If (typeParameters.Length > position) Then
					item = typeParameters(position)
				Else
					item = New UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
				End If
			Else
				item = New UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
			End If
			Return item
		End Function

		Protected Overrides Function GetGenericTypeParamSymbol(ByVal position As Integer) As TypeSymbol
			Dim unsupportedMetadataTypeSymbol As TypeSymbol
			Dim containingSymbol As PENamedTypeSymbol = Me._typeContextOpt
			While containingSymbol IsNot Nothing AndAlso containingSymbol.MetadataArity - containingSymbol.Arity > position
				containingSymbol = TryCast(containingSymbol.ContainingSymbol, PENamedTypeSymbol)
			End While
			If (containingSymbol Is Nothing OrElse containingSymbol.MetadataArity <= position) Then
				unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
			Else
				position = position - (containingSymbol.MetadataArity - containingSymbol.Arity)
				unsupportedMetadataTypeSymbol = containingSymbol.TypeParameters(position)
			End If
			Return unsupportedMetadataTypeSymbol
		End Function

		Protected Overrides Function GetIndexOfReferencedAssembly(ByVal identity As AssemblyIdentity) As Integer
			Dim num As Integer
			Dim referencedAssemblies As ImmutableArray(Of AssemblyIdentity) = Me.ModuleSymbol.GetReferencedAssemblies()
			Dim length As Integer = referencedAssemblies.Length - 1
			Dim num1 As Integer = 0
			While True
				If (num1 > length) Then
					num = -1
					Exit While
				ElseIf (Not identity.Equals(referencedAssemblies(num1))) Then
					num1 = num1 + 1
				Else
					num = num1
					Exit While
				End If
			End While
			Return num
		End Function

		Protected Overrides Function GetMethodHandle(ByVal method As MethodSymbol) As System.Reflection.Metadata.MethodDefinitionHandle
			Dim methodDefinitionHandle As System.Reflection.Metadata.MethodDefinitionHandle
			Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol)
			methodDefinitionHandle = If(pEMethodSymbol Is Nothing OrElse pEMethodSymbol.ContainingModule <> Me.ModuleSymbol, New System.Reflection.Metadata.MethodDefinitionHandle(), pEMethodSymbol.Handle)
			Return methodDefinitionHandle
		End Function

		Friend Overrides Function GetSymbolForMemberRef(ByVal memberRef As MemberReferenceHandle, Optional ByVal scope As TypeSymbol = Nothing, Optional ByVal methodsOnly As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim memberRefTypeSymbol As TypeSymbol = MyBase.GetMemberRefTypeSymbol(memberRef)
			If (memberRefTypeSymbol IsNot Nothing) Then
				If (scope IsNot Nothing AndAlso Not TypeSymbol.Equals(memberRefTypeSymbol, scope, TypeCompareKind.ConsiderEverything)) Then
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					If (memberRefTypeSymbol.IsBaseTypeOrInterfaceOf(scope, discarded)) Then
						GoTo Label1
					End If
					symbol = Nothing
					Return symbol
				End If
			Label1:
				If (Not memberRefTypeSymbol.IsTupleCompatible()) Then
					memberRefTypeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(memberRefTypeSymbol, New ImmutableArray(Of String)())
				End If
				symbol = (New MemberRefMetadataDecoder(Me.ModuleSymbol, memberRefTypeSymbol)).FindMember(memberRefTypeSymbol, memberRef, methodsOnly)
			Else
				symbol = Nothing
			End If
			Return symbol
		End Function

		Protected Overrides Function GetTypeHandleToTypeMap() As ConcurrentDictionary(Of TypeDefinitionHandle, TypeSymbol)
			Return Me.ModuleSymbol.TypeHandleToTypeMap
		End Function

		Protected Overrides Function GetTypeRefHandleToTypeMap() As ConcurrentDictionary(Of TypeReferenceHandle, TypeSymbol)
			Return Me.ModuleSymbol.TypeRefHandleToTypeMap
		End Function

		Public Shared Function IsOrClosedOverATypeFromAssemblies(ByVal this As TypeSymbol, ByVal assemblies As ImmutableArray(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = this.Kind
			If (kind > SymbolKind.ErrorType) Then
				If (kind = SymbolKind.NamedType) Then
					GoTo Label0
				End If
				If (kind <> SymbolKind.TypeParameter) Then
					Throw ExceptionUtilities.UnexpectedValue(this.Kind)
				End If
				flag = False
				Return flag
			Else
				If (kind <> SymbolKind.ArrayType) Then
					GoTo Label3
				End If
				flag = MetadataDecoder.IsOrClosedOverATypeFromAssemblies(DirectCast(this, ArrayTypeSymbol).ElementType, assemblies)
				Return flag
			End If
			Throw ExceptionUtilities.UnexpectedValue(this.Kind)
		Label0:
			Dim containingType As NamedTypeSymbol = DirectCast(this, NamedTypeSymbol)
			Dim containingAssembly As AssemblySymbol = containingType.OriginalDefinition.ContainingAssembly
			If (containingAssembly IsNot Nothing) Then
				Dim length As Integer = assemblies.Length - 1
				Dim num As Integer = 0
				While num <= length
					If (CObj(containingAssembly) <> CObj(assemblies(num))) Then
						num = num + 1
					Else
						flag = True
						Return flag
					End If
				End While
			End If
			Do
				If (Not containingType.IsTupleType) Then
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = containingType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						If (Not MetadataDecoder.IsOrClosedOverATypeFromAssemblies(enumerator.Current, assemblies)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
					containingType = containingType.ContainingType
				Else
					flag = MetadataDecoder.IsOrClosedOverATypeFromAssemblies(containingType.TupleUnderlyingType, assemblies)
					Return flag
				End If
			Loop While containingType IsNot Nothing
			flag = False
			Return flag
		Label3:
			If (kind = SymbolKind.ErrorType) Then
				GoTo Label0
			End If
			Throw ExceptionUtilities.UnexpectedValue(this.Kind)
		End Function

		Protected Overrides Function LookupNestedTypeDefSymbol(ByVal container As TypeSymbol, ByRef emittedName As MetadataTypeName) As TypeSymbol
			Return container.LookupMetadataType(emittedName)
		End Function

		Protected Overrides Function LookupTopLevelTypeDefSymbol(ByVal referencedAssemblyIndex As Integer, ByRef emittedName As MetadataTypeName) As TypeSymbol
			Dim unsupportedMetadataTypeSymbol As TypeSymbol
			Dim referencedAssemblySymbol As AssemblySymbol = Me.ModuleSymbol.GetReferencedAssemblySymbol(referencedAssemblyIndex)
			If (referencedAssemblySymbol IsNot Nothing) Then
				Try
					unsupportedMetadataTypeSymbol = referencedAssemblySymbol.LookupTopLevelMetadataType(emittedName, True)
				Catch exception As System.Exception When FatalError.ReportAndPropagate(exception)
					Throw ExceptionUtilities.Unreachable
				End Try
			Else
				unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
			End If
			Return unsupportedMetadataTypeSymbol
		End Function

		Protected Overrides Function LookupTopLevelTypeDefSymbol(ByVal moduleName As String, ByRef emittedName As MetadataTypeName, <Out> ByRef isNoPiaLocalType As Boolean) As TypeSymbol
			Dim topLevel As TypeSymbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol).Enumerator = Me.ModuleSymbol.ContainingAssembly.Modules.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol = enumerator.Current
					If ([String].Equals(current.Name, moduleName, StringComparison.OrdinalIgnoreCase)) Then
						If (current <> Me.ModuleSymbol) Then
							isNoPiaLocalType = False
							topLevel = current.LookupTopLevelMetadataType(emittedName)
							Exit While
						Else
							topLevel = Me.ModuleSymbol.LookupTopLevelMetadataType(emittedName, isNoPiaLocalType)
							Exit While
						End If
					End If
				Else
					isNoPiaLocalType = False
					topLevel = New MissingMetadataTypeSymbol.TopLevel(New MissingModuleSymbolWithName(Me.ModuleSymbol.ContainingAssembly, moduleName), emittedName, SpecialType.None)
					Exit While
				End If
			End While
			Return topLevel
		End Function

		Protected Overrides Function LookupTopLevelTypeDefSymbol(ByRef emittedName As MetadataTypeName, <Out> ByRef isNoPiaLocalType As Boolean) As TypeSymbol
			Return Me.ModuleSymbol.LookupTopLevelMetadataType(emittedName, isNoPiaLocalType)
		End Function

		Protected Overrides Function SubstituteNoPiaLocalType(ByVal typeDef As TypeDefinitionHandle, ByRef name As MetadataTypeName, ByVal interfaceGuid As String, ByVal scope As String, ByVal identifier As String) As TypeSymbol
			Dim unsupportedMetadataTypeSymbol As TypeSymbol
			Try
				Dim flag As Boolean = Me.[Module].IsInterfaceOrThrow(typeDef)
				Dim typeOfToken As TypeSymbol = Nothing
				If (Not flag) Then
					Dim baseTypeOfTypeOrThrow As EntityHandle = Me.[Module].GetBaseTypeOfTypeOrThrow(typeDef)
					If (Not baseTypeOfTypeOrThrow.IsNil) Then
						typeOfToken = MyBase.GetTypeOfToken(baseTypeOfTypeOrThrow)
					End If
				End If
				unsupportedMetadataTypeSymbol = MetadataDecoder.SubstituteNoPiaLocalType(name, flag, typeOfToken, interfaceGuid, scope, identifier, Me.ModuleSymbol.ContainingAssembly)
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				unsupportedMetadataTypeSymbol = MyBase.GetUnsupportedMetadataTypeSymbol(badImageFormatException)
				ProjectData.ClearProjectError()
			End Try
			Return Me.GetTypeHandleToTypeMap().GetOrAdd(typeDef, unsupportedMetadataTypeSymbol)
		End Function

		Friend Shared Function SubstituteNoPiaLocalType(ByRef fullEmittedName As MetadataTypeName, ByVal isInterface As Boolean, ByVal baseType As TypeSymbol, ByVal interfaceGuid As String, ByVal scope As String, ByVal identifier As String, ByVal referringAssembly As AssemblySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim specialType As Microsoft.CodeAnalysis.SpecialType
			Dim noPiaAmbiguousCanonicalTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim guid As System.Guid = New System.Guid()
			Dim flag As Boolean = False
			Dim guid1 As System.Guid = New System.Guid()
			Dim flag1 As Boolean = False
			If (isInterface AndAlso interfaceGuid IsNot Nothing) Then
				flag = System.Guid.TryParse(interfaceGuid, guid)
				If (flag) Then
					scope = Nothing
					identifier = Nothing
				End If
			End If
			If (scope IsNot Nothing) Then
				flag1 = System.Guid.TryParse(scope, guid1)
			End If
			Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = referringAssembly.GetNoPiaResolutionAssemblies().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AssemblySymbol = enumerator.Current
				If (CObj(current) = CObj(referringAssembly)) Then
					Continue While
				End If
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = current.LookupTopLevelMetadataType(fullEmittedName, False)
				If (namedTypeSymbol.Kind = SymbolKind.ErrorType OrElse CObj(namedTypeSymbol.ContainingAssembly) <> CObj(current) OrElse namedTypeSymbol.DeclaredAccessibility <> Accessibility.[Public]) Then
					Continue While
				End If
				Dim str As String = Nothing
				Dim flag2 As Boolean = False
				Dim guid2 As System.Guid = New System.Guid()
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = namedTypeSymbol.TypeKind
				Select Case typeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Label0:
						If (isInterface) Then
							Continue While
						End If
						Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics
						If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
							specialType = baseTypeNoUseSiteDiagnostics.SpecialType
						Else
							specialType = Microsoft.CodeAnalysis.SpecialType.None
						End If
						Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = specialType
						If (specialType1 = Microsoft.CodeAnalysis.SpecialType.None) Then
							Continue While
						End If
						If (specialType1 <> If(baseType IsNot Nothing, baseType.SpecialType, Microsoft.CodeAnalysis.SpecialType.None)) Then
							Continue While
						Else
							Exit Select
						End If
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
						Continue While
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						If (Not isInterface) Then
							Continue While
						End If
						If (Not namedTypeSymbol.GetGuidString(str) OrElse str Is Nothing) Then
							Exit Select
						End If
						flag2 = System.Guid.TryParse(str, guid2)
						Exit Select
					Case Else
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
							GoTo Label0
						End If
						Continue While
				End Select
				If (flag OrElse flag2) Then
					If (Not flag OrElse Not flag2) Then
						Continue While
					End If
					If (guid2 <> guid) Then
						Continue While
					End If
				Else
					If (Not flag1 OrElse identifier Is Nothing OrElse Not [String].Equals(identifier, fullEmittedName.FullName, StringComparison.Ordinal)) Then
						Continue While
					End If
					flag2 = False
					If (current.GetGuidString(str) AndAlso str IsNot Nothing) Then
						flag2 = System.Guid.TryParse(str, guid2)
					End If
					If (Not flag2 OrElse guid1 <> guid2) Then
						Continue While
					End If
				End If
				If (noPiaAmbiguousCanonicalTypeSymbol Is Nothing) Then
					noPiaAmbiguousCanonicalTypeSymbol = namedTypeSymbol
				Else
					noPiaAmbiguousCanonicalTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaAmbiguousCanonicalTypeSymbol(referringAssembly, noPiaAmbiguousCanonicalTypeSymbol, namedTypeSymbol)
					Exit While
				End If
			End While
			If (noPiaAmbiguousCanonicalTypeSymbol Is Nothing) Then
				noPiaAmbiguousCanonicalTypeSymbol = New NoPiaMissingCanonicalTypeSymbol(referringAssembly, fullEmittedName.FullName, interfaceGuid, scope, identifier)
			End If
			Return noPiaAmbiguousCanonicalTypeSymbol
		End Function
	End Class
End Namespace