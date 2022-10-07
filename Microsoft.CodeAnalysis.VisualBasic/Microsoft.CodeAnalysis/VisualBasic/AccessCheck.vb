Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AccessCheck
		Private ReadOnly Shared s_mapAccessToAccessOutsideAssembly As Accessibility()

		Shared Sub New()
			AccessCheck.s_mapAccessToAccessOutsideAssembly = New Accessibility() { Accessibility.NotApplicable, Accessibility.[Private], Accessibility.[Private], Accessibility.[Protected], Accessibility.[Private], Accessibility.[Protected], Accessibility.[Public] }
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub AddBaseInterfaces(ByVal derived As TypeSymbol, ByVal baseInterfaces As ArrayBuilder(Of NamedTypeSymbol), ByVal interfacesLookedAt As PooledHashSet(Of NamedTypeSymbol), ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved)
			Dim typeSymbols As ImmutableArray(Of TypeSymbol)
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim inheritsBeingResolvedOpt As ConsList(Of TypeSymbol) = basesBeingResolved.InheritsBeingResolvedOpt
			If (inheritsBeingResolvedOpt IsNot Nothing) Then
				flag = inheritsBeingResolvedOpt.Contains(derived)
			Else
				flag = False
			End If
			If (Not flag) Then
				Dim implementsBeingResolvedOpt As ConsList(Of TypeSymbol) = basesBeingResolved.ImplementsBeingResolvedOpt
				If (implementsBeingResolvedOpt IsNot Nothing) Then
					flag1 = implementsBeingResolvedOpt.Contains(derived)
				Else
					flag1 = False
				End If
				If (Not flag1) Then
					Dim kind As SymbolKind = derived.Kind
					If (kind = SymbolKind.ErrorType OrElse kind = SymbolKind.NamedType) Then
						typeSymbols = ImmutableArray(Of TypeSymbol).CastUp(Of NamedTypeSymbol)(DirectCast(derived, NamedTypeSymbol).GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved))
					Else
						typeSymbols = If(kind <> SymbolKind.TypeParameter, ImmutableArray(Of TypeSymbol).CastUp(Of NamedTypeSymbol)(derived.InterfacesNoUseSiteDiagnostics), DirectCast(derived, TypeParameterSymbol).ConstraintTypesNoUseSiteDiagnostics)
					End If
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeSymbol = enumerator.Current
						Dim typeKind As Microsoft.CodeAnalysis.TypeKind = current.TypeKind
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Interface]) Then
							Dim originalDefinition As NamedTypeSymbol = DirectCast(current.OriginalDefinition, NamedTypeSymbol)
							If (Not interfacesLookedAt.Add(originalDefinition)) Then
								Continue While
							End If
							baseInterfaces.Add(originalDefinition)
						ElseIf (typeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
							AccessCheck.AddBaseInterfaces(current, baseInterfaces, interfacesLookedAt, basesBeingResolved)
						End If
					End While
				End If
			End If
		End Sub

		Private Shared Function CanBeAccessedThroughInheritance(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal container As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal isOutsideAssembly As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (AccessCheck.GetAccessInAssemblyContext(type, isOutsideAssembly) <> Accessibility.[Private]) Then
				Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ContainingType
				If (containingType IsNot Nothing) Then
					Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingType.OriginalDefinition
					If (Not container.OriginalDefinition.Equals(originalDefinition)) Then
						If (Not containingType.IsInterfaceType()) Then
							Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = container.BaseTypeOriginalDefinition(useSiteInfo)
							While namedTypeSymbol IsNot Nothing
								If (Not namedTypeSymbol.Equals(originalDefinition)) Then
									namedTypeSymbol = namedTypeSymbol.BaseTypeOriginalDefinition(useSiteInfo)
								Else
									flag = True
									Return flag
								End If
							End While
						Else
							Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = container.AllInterfacesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
							While enumerator.MoveNext()
								If (Not enumerator.Current.OriginalDefinition.Equals(originalDefinition)) Then
									Continue While
								End If
								flag = True
								Return flag
							End While
						End If
						flag = If(AccessCheck.GetAccessInAssemblyContext(type, isOutsideAssembly) = Accessibility.[Protected], False, AccessCheck.CanBeAccessedThroughInheritance(containingType, container, isOutsideAssembly, useSiteInfo))
					Else
						flag = True
					End If
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function CheckMemberAccessibility(ByVal containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal declaredAccessibility As Accessibility, ByVal within As Symbol, ByVal throughTypeOpt As TypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingType.OriginalDefinition
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(within, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = If(TryCast(within, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), namedTypeSymbol.ContainingAssembly)
			Dim accessCheckResult1 As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult = AccessCheck.CheckNamedTypeAccessibility(containingType, within, basesBeingResolved, useSiteInfo)
			If (accessCheckResult1 = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible) Then
				Select Case declaredAccessibility
					Case Accessibility.NotApplicable
						accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
						Return accessCheckResult
					Case Accessibility.[Private]
						If (containingType.TypeKind <> TypeKind.Submission) Then
							accessCheckResult = If(namedTypeSymbol Is Nothing, Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible, AccessCheck.CheckPrivateSymbolAccessibility(namedTypeSymbol, originalDefinition))
							Return accessCheckResult
						Else
							accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
							Return accessCheckResult
						End If
					Case Accessibility.ProtectedAndInternal
						If (AccessCheck.HasFriendAccessTo(assemblySymbol, containingType.ContainingAssembly)) Then
							accessCheckResult = AccessCheck.CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, useSiteInfo)
							Return accessCheckResult
						Else
							accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible
							Return accessCheckResult
						End If
					Case Accessibility.[Protected]
						accessCheckResult = AccessCheck.CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, useSiteInfo)
						Return accessCheckResult
					Case Accessibility.Internal
						accessCheckResult = If(AccessCheck.HasFriendAccessTo(assemblySymbol, containingType.ContainingAssembly), Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible, Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible)
						Return accessCheckResult
					Case Accessibility.ProtectedOrInternal
						If (Not AccessCheck.HasFriendAccessTo(assemblySymbol, containingType.ContainingAssembly)) Then
							accessCheckResult = AccessCheck.CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, useSiteInfo)
							Return accessCheckResult
						Else
							accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
							Return accessCheckResult
						End If
					Case Accessibility.[Public]
						accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
						Return accessCheckResult
				End Select
				Throw ExceptionUtilities.UnexpectedValue(declaredAccessibility)
			Else
				accessCheckResult = accessCheckResult1
			End If
			Return accessCheckResult
		End Function

		Private Shared Function CheckNamedTypeAccessibility(ByVal typeSym As NamedTypeSymbol, ByVal within As Symbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			If (Not typeSym.IsDefinition) Then
				Dim typeSymbols As ImmutableArray(Of TypeSymbol) = typeSym.TypeArgumentsWithDefinitionUseSiteDiagnostics(useSiteInfo)
				Dim length As Integer = typeSymbols.Length - 1
				For i As Integer = 0 To length
					If (typeSymbols(i).Kind <> SymbolKind.TypeParameter) Then
						Dim accessCheckResult1 As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult = AccessCheck.CheckSymbolAccessibilityCore(typeSymbols(i), within, Nothing, basesBeingResolved, useSiteInfo)
						If (accessCheckResult1 <> Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible) Then
							accessCheckResult = accessCheckResult1
							Return accessCheckResult
						End If
					End If
				Next

			End If
			accessCheckResult = If(typeSym.ContainingType IsNot Nothing, AccessCheck.CheckMemberAccessibility(typeSym.ContainingType, typeSym.DeclaredAccessibility, within, Nothing, basesBeingResolved, useSiteInfo), AccessCheck.CheckNonNestedTypeAccessibility(typeSym.ContainingAssembly, typeSym.DeclaredAccessibility, within))
			Return accessCheckResult
		End Function

		Private Shared Function CheckNonNestedTypeAccessibility(ByVal assembly As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol, ByVal declaredAccessibility As Accessibility, ByVal within As Symbol) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = If(TryCast(within, Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol), DirectCast(within, NamedTypeSymbol).ContainingAssembly)
			Select Case declaredAccessibility
				Case Accessibility.NotApplicable
				Case Accessibility.[Public]
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
					Exit Select
				Case Accessibility.[Private]
				Case Accessibility.ProtectedAndInternal
				Case Accessibility.[Protected]
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
					Exit Select
				Case Accessibility.Internal
				Case Accessibility.ProtectedOrInternal
					accessCheckResult = If(AccessCheck.HasFriendAccessTo(assemblySymbol, assembly), Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible, Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(declaredAccessibility)
			End Select
			Return accessCheckResult
		End Function

		Private Shared Function CheckPrivateSymbolAccessibility(ByVal within As Symbol, ByVal originalContainingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(within, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol IsNot Nothing) Then
				accessCheckResult = If(AccessCheck.IsNestedWithinOriginalContainingType(namedTypeSymbol, originalContainingType), Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible, Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible)
			Else
				accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible
			End If
			Return accessCheckResult
		End Function

		Private Shared Function CheckProtectedSymbolAccessibility(ByVal within As Symbol, ByVal throughTypeOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal originalContainingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (originalContainingType.TypeKind <> TypeKind.Submission) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(within, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol Is Nothing) Then
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible
				ElseIf (Not AccessCheck.IsNestedWithinOriginalContainingType(namedTypeSymbol, originalContainingType)) Then
					namedTypeSymbol = namedTypeSymbol.OriginalDefinition
					Dim accessCheckResult1 As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Inaccessible
					If (throughTypeOpt IsNot Nothing) Then
						originalDefinition = throughTypeOpt.OriginalDefinition
					Else
						originalDefinition = Nothing
					End If
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = originalDefinition
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol
					While containingType IsNot Nothing
						If (AccessCheck.InheritsFromOrImplementsIgnoringConstruction(containingType, originalContainingType, basesBeingResolved, useSiteInfo)) Then
							If (typeSymbol Is Nothing OrElse AccessCheck.InheritsFromOrImplementsIgnoringConstruction(typeSymbol, containingType, basesBeingResolved, useSiteInfo)) Then
								accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
								Return accessCheckResult
							Else
								accessCheckResult1 = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.InaccessibleViaThroughType
							End If
						End If
						containingType = containingType.ContainingType
					End While
					accessCheckResult = accessCheckResult1
				Else
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
				End If
			Else
				accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
			End If
			Return accessCheckResult
		End Function

		Public Shared Function CheckSymbolAccessibility(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As AssemblySymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			Return AccessCheck.CheckSymbolAccessibilityCore(symbol, within, Nothing, basesBeingResolved, useSiteInfo)
		End Function

		Public Shared Function CheckSymbolAccessibility(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As NamedTypeSymbol, ByVal throughTypeOpt As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			Return AccessCheck.CheckSymbolAccessibilityCore(symbol, within, throughTypeOpt, basesBeingResolved, useSiteInfo)
		End Function

		Private Shared Function CheckSymbolAccessibilityCore(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal throughTypeOpt As TypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			Dim accessCheckResult As Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult
			If (Not TypeOf within Is AssemblySymbol) Then
				Dim containingAssembly As AssemblySymbol = DirectCast(within, NamedTypeSymbol).ContainingAssembly
			End If
			Select Case symbol.Kind
				Case SymbolKind.[Alias]
					accessCheckResult = AccessCheck.CheckSymbolAccessibilityCore(DirectCast(symbol, AliasSymbol).Target, within, Nothing, basesBeingResolved, useSiteInfo)
					Exit Select
				Case SymbolKind.ArrayType
					accessCheckResult = AccessCheck.CheckSymbolAccessibilityCore(DirectCast(symbol, ArrayTypeSymbol).ElementType, within, Nothing, basesBeingResolved, useSiteInfo)
					Exit Select
				Case SymbolKind.Assembly
				Case SymbolKind.Label
				Case SymbolKind.Local
				Case SymbolKind.NetModule
				Case SymbolKind.[Namespace]
				Case SymbolKind.Parameter
				Case SymbolKind.RangeVariable
				Case SymbolKind.TypeParameter
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
					Exit Select
				Case SymbolKind.DynamicType
				Case SymbolKind.PointerType
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				Case SymbolKind.ErrorType
					accessCheckResult = Microsoft.CodeAnalysis.VisualBasic.AccessCheckResult.Accessible
					Exit Select
				Case SymbolKind.[Event]
				Case SymbolKind.Field
				Case SymbolKind.Method
				Case SymbolKind.[Property]
					If (symbol.IsShared) Then
						throughTypeOpt = Nothing
					End If
					accessCheckResult = AccessCheck.CheckMemberAccessibility(symbol.ContainingType, symbol.DeclaredAccessibility, within, throughTypeOpt, basesBeingResolved, useSiteInfo)
					Exit Select
				Case SymbolKind.NamedType
					accessCheckResult = AccessCheck.CheckNamedTypeAccessibility(DirectCast(symbol, NamedTypeSymbol), within, basesBeingResolved, useSiteInfo)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
			End Select
			Return accessCheckResult
		End Function

		Private Shared Function FindEnclosingTypeWithGivenAccess(ByVal member As Symbol, ByVal stopAtAccess As Accessibility, ByVal isOutsideAssembly As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = member.ContainingType
			If (member.Kind = SymbolKind.NamedType AndAlso containingType Is Nothing) Then
				containingType = DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			While True
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingType.ContainingType
				If (namedTypeSymbol Is Nothing OrElse AccessCheck.GetAccessInAssemblyContext(containingType, isOutsideAssembly) <= stopAtAccess) Then
					Exit While
				End If
				containingType = namedTypeSymbol
			End While
			Return containingType
		End Function

		Public Shared Function GetAccessibilityForErrorMessage(ByVal sym As Symbol, ByVal fromAssembly As AssemblySymbol) As String
			Return sym.DeclaredAccessibility.ToDisplay()
		End Function

		Private Shared Function GetAccessInAssemblyContext(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal isOutsideAssembly As Boolean) As Accessibility
			Dim declaredAccessibility As Accessibility = symbol.DeclaredAccessibility
			If (isOutsideAssembly) Then
				declaredAccessibility = AccessCheck.s_mapAccessToAccessOutsideAssembly(CInt(declaredAccessibility))
			End If
			Return declaredAccessibility
		End Function

		Private Shared Function GetEffectiveAccessOutsideAssembly(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility As Microsoft.CodeAnalysis.Accessibility
			Dim sMapAccessToAccessOutsideAssembly As Microsoft.CodeAnalysis.Accessibility = AccessCheck.s_mapAccessToAccessOutsideAssembly(CInt(symbol.DeclaredAccessibility))
			If (sMapAccessToAccessOutsideAssembly <> Microsoft.CodeAnalysis.Accessibility.[Private]) Then
				Dim containingType As NamedTypeSymbol = symbol.ContainingType
				While containingType IsNot Nothing
					Dim sMapAccessToAccessOutsideAssembly1 As Microsoft.CodeAnalysis.Accessibility = AccessCheck.s_mapAccessToAccessOutsideAssembly(CInt(containingType.DeclaredAccessibility))
					If (sMapAccessToAccessOutsideAssembly1 < sMapAccessToAccessOutsideAssembly) Then
						sMapAccessToAccessOutsideAssembly = sMapAccessToAccessOutsideAssembly1
					End If
					If (sMapAccessToAccessOutsideAssembly <> Microsoft.CodeAnalysis.Accessibility.[Private]) Then
						containingType = containingType.ContainingType
					Else
						accessibility = sMapAccessToAccessOutsideAssembly
						Return accessibility
					End If
				End While
				accessibility = sMapAccessToAccessOutsideAssembly
			Else
				accessibility = sMapAccessToAccessOutsideAssembly
			End If
			Return accessibility
		End Function

		Public Shared Function HasFriendAccessTo(ByVal fromAssembly As AssemblySymbol, ByVal toAssembly As AssemblySymbol) As Boolean
			If (AccessCheck.IsSameAssembly(fromAssembly, toAssembly)) Then
				Return True
			End If
			Return AccessCheck.InternalsAccessibleTo(toAssembly, fromAssembly)
		End Function

		Private Shared Function InheritsFromOrImplementsIgnoringConstruction(ByVal derivedType As TypeSymbol, ByVal baseType As TypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim enumerator As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator()
			Dim flag As Boolean
			Dim instance As PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = Nothing
			Dim namedTypeSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = Nothing
			Dim flag1 As Boolean = baseType.IsInterfaceType()
			If (flag1) Then
				instance = PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).GetInstance()
				namedTypeSymbols = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).GetInstance()
			End If
			Dim originalDefinition As TypeSymbol = derivedType
			Dim flag2 As Boolean = False
			While originalDefinition IsNot Nothing
				If (flag1 <> originalDefinition.IsInterfaceType() OrElse Not originalDefinition.Equals(baseType)) Then
					If (flag1) Then
						AccessCheck.AddBaseInterfaces(originalDefinition, namedTypeSymbols, instance, basesBeingResolved)
					End If
					Dim inheritsBeingResolvedOpt As ConsList(Of TypeSymbol) = basesBeingResolved.InheritsBeingResolvedOpt
					If (inheritsBeingResolvedOpt IsNot Nothing) Then
						flag = inheritsBeingResolvedOpt.Contains(originalDefinition)
					Else
						flag = False
					End If
					If (Not flag) Then
						Dim typeKind As Microsoft.CodeAnalysis.TypeKind = originalDefinition.TypeKind
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Interface]) Then
							originalDefinition = Nothing
						Else
							originalDefinition = If(typeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter, DirectCast(originalDefinition, TypeParameterSymbol).GetClassConstraint(useSiteInfo), originalDefinition.GetDirectBaseTypeWithDefinitionUseSiteDiagnostics(basesBeingResolved, useSiteInfo))
						End If
						If (originalDefinition Is Nothing) Then
							Continue While
						End If
						originalDefinition = originalDefinition.OriginalDefinition
					Else
						originalDefinition = Nothing
					End If
				Else
					flag2 = True
					Exit While
				End If
			End While
			If (Not flag2 AndAlso flag1) Then
				While namedTypeSymbols.Count <> 0
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbols.Pop()
					If (Not namedTypeSymbol.Equals(baseType)) Then
						AccessCheck.AddBaseInterfaces(namedTypeSymbol, namedTypeSymbols, instance, basesBeingResolved)
					Else
						flag2 = True
						Exit While
					End If
				End While
				If (Not flag2) Then
					Try
						enumerator = instance.GetEnumerator()
						While enumerator.MoveNext()
							enumerator.Current.AddUseSiteInfo(useSiteInfo)
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
			End If
			If (instance IsNot Nothing) Then
				instance.Free()
			End If
			If (namedTypeSymbols IsNot Nothing) Then
				namedTypeSymbols.Free()
			End If
			Return flag2
		End Function

		Private Shared Function InternalsAccessibleTo(ByVal toAssembly As AssemblySymbol, ByVal assemblyWantingAccess As AssemblySymbol) As Boolean
			Dim flag As Boolean
			If (Not assemblyWantingAccess.AreInternalsVisibleToThisAssembly(toAssembly)) Then
				flag = If(Not assemblyWantingAccess.IsInteractive OrElse Not toAssembly.IsInteractive, False, True)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function IsNestedWithinOriginalContainingType(ByVal withinType As NamedTypeSymbol, ByVal originalContainingType As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim originalDefinition As NamedTypeSymbol = withinType.OriginalDefinition
			While True
				If (originalDefinition Is Nothing) Then
					flag = False
					Exit While
				ElseIf (CObj(originalDefinition) <> CObj(originalContainingType)) Then
					originalDefinition = originalDefinition.ContainingType
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function IsSameAssembly(ByVal fromAssembly As AssemblySymbol, ByVal toAssembly As AssemblySymbol) As Boolean
			Return [Object].Equals(fromAssembly, toAssembly)
		End Function

		Public Shared Function IsSymbolAccessible(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As AssemblySymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As Boolean
			Return AccessCheck.CheckSymbolAccessibilityCore(symbol, within, Nothing, basesBeingResolved, useSiteInfo) = AccessCheckResult.Accessible
		End Function

		Public Shared Function IsSymbolAccessible(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As NamedTypeSymbol, ByVal throughTypeOpt As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As Boolean
			Return AccessCheck.CheckSymbolAccessibilityCore(symbol, within, throughTypeOpt, basesBeingResolved, useSiteInfo) = AccessCheckResult.Accessible
		End Function

		Private Shared Function IsTypeNestedIn(ByVal probablyNestedType As NamedTypeSymbol, ByVal probablyEnclosingType As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			probablyNestedType = probablyNestedType.OriginalDefinition
			Dim containingType As NamedTypeSymbol = probablyNestedType.ContainingType
			While True
				If (containingType Is Nothing) Then
					flag = False
					Exit While
				ElseIf (Not containingType.Equals(probablyEnclosingType)) Then
					containingType = containingType.ContainingType
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function MemberIsOrNestedInType(ByVal member As Symbol, ByVal type As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			type = type.OriginalDefinition
			If (Not member.Equals(type)) Then
				Dim containingType As NamedTypeSymbol = member.ContainingType
				While containingType IsNot Nothing
					If (Not containingType.Equals(type)) Then
						containingType = containingType.ContainingType
					Else
						flag = True
						Return flag
					End If
				End While
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function VerifyAccessExposure(ByVal exposedThrough As Symbol, ByVal exposedType As TypeSymbol, ByRef illegalExposure As ArrayBuilder(Of AccessCheck.AccessExposure), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind
			Dim flag1 As Boolean = True
			Do
			Label1:
				kind = exposedType.Kind
				If (kind > SymbolKind.ErrorType) Then
					If (kind <> SymbolKind.NamedType) Then
						Continue Do
					End If
					Dim tupleUnderlyingType As NamedTypeSymbol = DirectCast(exposedType, NamedTypeSymbol)
					If (tupleUnderlyingType.IsTupleType) Then
						tupleUnderlyingType = tupleUnderlyingType.TupleUnderlyingType
					End If
					Dim containingType As NamedTypeSymbol = tupleUnderlyingType
					Do
						If (containingType.Arity > 0) Then
							Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = containingType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
							While enumerator.MoveNext()
								If (AccessCheck.VerifyAccessExposure(exposedThrough, enumerator.Current, illegalExposure, useSiteInfo)) Then
									Continue While
								End If
								flag1 = False
							End While
						End If
						containingType = containingType.ContainingType
					Loop While containingType IsNot Nothing
					Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Nothing
					If (Not AccessCheck.VerifyAccessExposure(exposedThrough, tupleUnderlyingType.OriginalDefinition, namespaceOrTypeSymbol, useSiteInfo)) Then
						If (illegalExposure Is Nothing) Then
							illegalExposure = ArrayBuilder(Of AccessCheck.AccessExposure).GetInstance()
						End If
						Dim accessExposures As ArrayBuilder(Of AccessCheck.AccessExposure) = illegalExposure
						Dim accessExposure As AccessCheck.AccessExposure = New AccessCheck.AccessExposure() With
						{
							.ExposedType = tupleUnderlyingType,
							.ExposedTo = namespaceOrTypeSymbol
						}
						accessExposures.Add(accessExposure)
						flag = False
						Return flag
					Else
						flag = flag1
						Return flag
					End If
				ElseIf (kind = SymbolKind.ArrayType) Then
					exposedType = DirectCast(exposedType, ArrayTypeSymbol).ElementType
					GoTo Label1
				Else
					If (kind = SymbolKind.ErrorType) Then
						Exit Do
					End If
					GoTo Label1
				End If
			Loop While kind <> SymbolKind.TypeParameter
			flag = True
			Return flag
		End Function

		Private Shared Function VerifyAccessExposure(ByVal exposedThrough As Symbol, ByVal exposedType As NamedTypeSymbol, ByRef containerWithAccessError As NamespaceOrTypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			containerWithAccessError = Nothing
			If (exposedType.DeclaredAccessibility = Accessibility.[Public]) Then
				Dim containingSymbol As Symbol = exposedType.ContainingSymbol
				If (containingSymbol Is Nothing OrElse containingSymbol.Kind <> SymbolKind.[Namespace]) Then
					If (Not AccessCheck.MemberIsOrNestedInType(exposedThrough, exposedType)) Then
						flag = If(AccessCheck.VerifyAccessExposureWithinAssembly(exposedThrough, exposedType, containerWithAccessError, useSiteInfo), AccessCheck.VerifyAccessExposureOutsideAssembly(exposedThrough, exposedType, useSiteInfo), False)
					Else
						flag = True
					End If
					Return flag
				End If
				flag = True
				Return flag
			End If
			If (Not AccessCheck.MemberIsOrNestedInType(exposedThrough, exposedType)) Then
				flag = If(AccessCheck.VerifyAccessExposureWithinAssembly(exposedThrough, exposedType, containerWithAccessError, useSiteInfo), AccessCheck.VerifyAccessExposureOutsideAssembly(exposedThrough, exposedType, useSiteInfo), False)
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Shared Sub VerifyAccessExposureForMemberType(ByVal member As Symbol, ByVal errorLocation As SyntaxNodeOrToken, ByVal typeBehindMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal isDelegateFromImplements As Boolean = False)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim accessExposures As ArrayBuilder(Of AccessCheck.AccessExposure) = Nothing
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagBag, member.ContainingAssembly)
			AccessCheck.VerifyAccessExposure(member, typeBehindMember, accessExposures, compoundUseSiteInfo)
			diagBag.Add(errorLocation, compoundUseSiteInfo)
			If (accessExposures IsNot Nothing) Then
				namedTypeSymbol = If(member.Kind <> SymbolKind.NamedType, member.ContainingType, DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
				Dim str As String = If(namedTypeSymbol.IsDelegateType(), namedTypeSymbol.Name, member.Name)
				Dim enumerator As ArrayBuilder(Of AccessCheck.AccessExposure).Enumerator = accessExposures.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AccessCheck.AccessExposure = enumerator.Current
					Dim exposedTo As NamespaceOrTypeSymbol = current.ExposedTo
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = current.ExposedType.DigThroughArrayType()
					If (exposedTo IsNot Nothing) Then
						If (Not isDelegateFromImplements) Then
							Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatch6, New [Object]() { str, typeSymbol, exposedTo.GetKindText(), exposedTo.ToErrorMessageArgument(ERRID.ERR_None), namedTypeSymbol.GetKindText(), namedTypeSymbol.Name })
						Else
							Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchImplementedEvent6, New [Object]() { str, typeSymbol, exposedTo.GetKindText(), exposedTo.ToErrorMessageArgument(ERRID.ERR_None), namedTypeSymbol.GetKindText(), namedTypeSymbol.Name })
						End If
					ElseIf (Not isDelegateFromImplements) Then
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchOutsideAssembly4, New [Object]() { str, typeSymbol, namedTypeSymbol.GetKindText(), namedTypeSymbol.Name })
					Else
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchImplementedEvent4, New [Object]() { str, typeSymbol, namedTypeSymbol.GetKindText(), namedTypeSymbol.Name })
					End If
				End While
				accessExposures.Free()
			End If
		End Sub

		Public Shared Sub VerifyAccessExposureForParameterType(ByVal member As Symbol, ByVal paramName As String, ByVal errorLocation As VisualBasicSyntaxNode, ByVal TypeBehindParam As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim accessExposures As ArrayBuilder(Of AccessCheck.AccessExposure) = Nothing
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagBag, member.ContainingAssembly)
			AccessCheck.VerifyAccessExposure(member, TypeBehindParam, accessExposures, compoundUseSiteInfo)
			diagBag.Add(errorLocation, compoundUseSiteInfo)
			If (accessExposures IsNot Nothing) Then
				Dim enumerator As ArrayBuilder(Of AccessCheck.AccessExposure).Enumerator = accessExposures.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AccessCheck.AccessExposure = enumerator.Current
					Dim exposedTo As NamespaceOrTypeSymbol = current.ExposedTo
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = current.ExposedType.DigThroughArrayType()
					Dim containingType As NamedTypeSymbol = member.ContainingType
					If (exposedTo Is Nothing) Then
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchOutsideAssembly4, New [Object]() { paramName, typeSymbol, containingType.GetKindText(), containingType.Name })
					Else
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatch6, New [Object]() { paramName, typeSymbol, exposedTo.GetKindText(), exposedTo.ToErrorMessageArgument(ERRID.ERR_None), containingType.GetKindText(), containingType.Name })
					End If
				End While
				accessExposures.Free()
			End If
		End Sub

		Private Shared Function VerifyAccessExposureHelper(ByVal exposingMember As Symbol, ByVal exposedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByRef containerWithAccessError As NamespaceOrTypeSymbol, ByRef seenThroughInheritance As Boolean, ByVal isOutsideAssembly As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim containingAssembly As Symbol
			seenThroughInheritance = False
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim accessInAssemblyContext As Microsoft.CodeAnalysis.Accessibility = AccessCheck.GetAccessInAssemblyContext(exposingMember, isOutsideAssembly)
			If (accessInAssemblyContext <> Microsoft.CodeAnalysis.Accessibility.[Private]) Then
				namedTypeSymbol = AccessCheck.FindEnclosingTypeWithGivenAccess(exposingMember, Microsoft.CodeAnalysis.Accessibility.[Protected], isOutsideAssembly)
			Else
				If (exposingMember.Kind = SymbolKind.NamedType AndAlso AccessCheck.IsTypeNestedIn(exposedType, DirectCast(exposingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))) Then
					GoTo Label1
				End If
				flag = True
				Return flag
			End If
		Label2:
			Dim accessibility As Microsoft.CodeAnalysis.Accessibility = AccessCheck.GetAccessInAssemblyContext(namedTypeSymbol, isOutsideAssembly)
			If (accessInAssemblyContext > Microsoft.CodeAnalysis.Accessibility.[Protected] OrElse Not AccessCheck.CanBeAccessedThroughInheritance(exposedType, exposingMember.ContainingType, isOutsideAssembly, useSiteInfo)) Then
				Dim containingNamespaceOrType As NamespaceOrTypeSymbol = namedTypeSymbol.ContainingNamespaceOrType
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = exposedType
				If (containingNamespaceOrType.IsNamespace) Then
					containingAssembly = containingNamespaceOrType.ContainingAssembly
				Else
					containingAssembly = containingNamespaceOrType
				End If
				Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				If (AccessCheck.CheckNamedTypeAccessibility(namedTypeSymbol1, containingAssembly, basesBeingResolved, useSiteInfo) = AccessCheckResult.Accessible) Then
					flag = If(accessibility = Microsoft.CodeAnalysis.Accessibility.[Protected], AccessCheck.VerifyAccessExposureHelper(namedTypeSymbol, exposedType, containerWithAccessError, seenThroughInheritance, isOutsideAssembly, useSiteInfo), True)
				Else
					containerWithAccessError = containingNamespaceOrType
					flag = False
				End If
			Else
				seenThroughInheritance = True
				flag = True
			End If
			Return flag
		Label1:
			namedTypeSymbol = DirectCast(exposingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			GoTo Label2
		End Function

		Public Shared Function VerifyAccessExposureOfBaseClassOrInterface(ByVal classOrInterface As NamedTypeSymbol, ByVal baseClassSyntax As TypeSyntax, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			Dim accessExposures As ArrayBuilder(Of AccessCheck.AccessExposure) = Nothing
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagBag, classOrInterface.ContainingAssembly)
			AccessCheck.VerifyAccessExposure(classOrInterface, base, accessExposures, compoundUseSiteInfo)
			diagBag.Add(baseClassSyntax, compoundUseSiteInfo)
			If (accessExposures Is Nothing) Then
				flag = True
			Else
				Dim enumerator As ArrayBuilder(Of AccessCheck.AccessExposure).Enumerator = accessExposures.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AccessCheck.AccessExposure = enumerator.Current
					Dim exposedTo As NamespaceOrTypeSymbol = current.ExposedTo
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = current.ExposedType.DigThroughArrayType()
					If (exposedTo IsNot Nothing) Then
						If (Not typeSymbol.Equals(base)) Then
							Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritsTypeArgAccessMismatch7, New [Object]() { classOrInterface.Name, base.GetKindText(), base.ToErrorMessageArgument(ERRID.ERR_None), typeSymbol, exposedTo.GetKindText(), exposedTo.ToErrorMessageArgument(ERRID.ERR_None) })
						Else
							Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritanceAccessMismatch5, New [Object]() { classOrInterface.Name, base.GetKindText(), base.ToErrorMessageArgument(ERRID.ERR_None), exposedTo.GetKindText(), exposedTo.ToErrorMessageArgument(ERRID.ERR_None) })
						End If
					ElseIf (Not typeSymbol.Equals(base)) Then
						Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritsTypeArgAccessMismatchOutside5, New [Object]() { classOrInterface.Name, base.GetKindText(), base.ToErrorMessageArgument(ERRID.ERR_None), typeSymbol })
					Else
						Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritanceAccessMismatchOutside3, New [Object]() { classOrInterface.Name, base.GetKindText(), base.ToErrorMessageArgument(ERRID.ERR_None) })
					End If
				End While
				accessExposures.Free()
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function VerifyAccessExposureOutsideAssembly(ByVal exposedThrough As Symbol, ByVal exposedType As NamedTypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim effectiveAccessOutsideAssembly As Microsoft.CodeAnalysis.Accessibility = AccessCheck.GetEffectiveAccessOutsideAssembly(exposedThrough)
			If (effectiveAccessOutsideAssembly <> Microsoft.CodeAnalysis.Accessibility.[Private]) Then
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility = AccessCheck.GetEffectiveAccessOutsideAssembly(exposedType)
				If (accessibility = Microsoft.CodeAnalysis.Accessibility.[Private]) Then
					flag = False
				ElseIf (accessibility = Microsoft.CodeAnalysis.Accessibility.[Public]) Then
					flag = True
				ElseIf (effectiveAccessOutsideAssembly <> Microsoft.CodeAnalysis.Accessibility.[Public]) Then
					Dim flag1 As Boolean = False
					Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Nothing
					AccessCheck.VerifyAccessExposureHelper(exposedThrough, exposedType, namespaceOrTypeSymbol, flag1, True, useSiteInfo)
					flag = flag1
				Else
					flag = False
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function VerifyAccessExposureWithinAssembly(ByVal exposedThrough As Symbol, ByVal exposedType As NamedTypeSymbol, ByRef containerWithAccessError As NamespaceOrTypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean = False
			Return AccessCheck.VerifyAccessExposureHelper(exposedThrough, exposedType, containerWithAccessError, flag, False, useSiteInfo)
		End Function

		Private Structure AccessExposure
			Public ExposedType As TypeSymbol

			Public ExposedTo As NamespaceOrTypeSymbol
		End Structure
	End Class
End Namespace