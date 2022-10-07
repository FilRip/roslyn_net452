Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class OverrideHidingHelper(Of TSymbol As Symbol)
		Inherits OverrideHidingHelper
		Private Shared s_runtimeSignatureComparer As IEqualityComparer(Of TSymbol)

		Shared Sub New()
			OverrideHidingHelper(Of MethodSymbol).s_runtimeSignatureComparer = MethodSignatureComparer.RuntimeMethodSignatureComparer
			OverrideHidingHelper(Of PropertySymbol).s_runtimeSignatureComparer = PropertySignatureComparer.RuntimePropertySignatureComparer
			OverrideHidingHelper(Of EventSymbol).s_runtimeSignatureComparer = EventSignatureComparer.RuntimeEventSignatureComparer
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub AddMemberToABuilder(ByVal member As TSymbol, ByVal builder As ArrayBuilder(Of TSymbol))
			Dim containingType As NamedTypeSymbol = member.ContainingType
			Dim count As Integer = builder.Count - 1
			Dim num As Integer = 0
			While True
				If (num <= count) Then
					Dim flag As Boolean = False
					If (Not TypeSymbol.Equals(builder(num).ContainingType, containingType, TypeCompareKind.ConsiderEverything)) Then
						Dim flag1 As Boolean = False
						If (OverrideHidingHelper.SignaturesMatch(DirectCast(builder(num), Symbol), DirectCast(member, Symbol), flag1, flag) AndAlso flag) Then
							Exit While
						End If
					End If
					num = num + 1
				Else
					builder.Add(member)
					Exit While
				End If
			End While
		End Sub

		Friend Shared Sub CheckOverrideMember(ByVal member As TSymbol, ByVal overriddenMembersResult As OverriddenMembersResult(Of TSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.OverrideHidingHelper`1::CheckOverrideMember(TSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.OverriddenMembersResult`1<TSymbol>,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void CheckOverrideMember(TSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.OverriddenMembersResult<TSymbol>,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Friend Shared Sub CheckOverridePropertyAccessor(ByVal overridingAccessor As MethodSymbol, ByVal overriddenAccessor As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = 0
			If (overridingAccessor IsNot Nothing AndAlso overriddenAccessor IsNot Nothing) Then
				Dim originalDefinition As MethodSymbol = overriddenAccessor.OriginalDefinition
				Dim containingType As NamedTypeSymbol = overridingAccessor.ContainingType
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				If (Not AccessCheck.IsSymbolAccessible(originalDefinition, containingType, Nothing, discarded, basesBeingResolved)) Then
					OverrideHidingHelper(Of TSymbol).ReportBadOverriding(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CannotOverrideInAccessibleMember, overridingAccessor, overriddenAccessor, diagnostics)
				ElseIf (Not OverrideHidingHelper(Of TSymbol).ConsistentAccessibility(overridingAccessor, overriddenAccessor, eRRID)) Then
					OverrideHidingHelper(Of TSymbol).ReportBadOverriding(eRRID, overridingAccessor, overriddenAccessor, diagnostics)
				End If
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = overriddenAccessor.GetUseSiteInfo()
				Dim locations As ImmutableArray(Of Location) = overridingAccessor.Locations
				diagnostics.Add(useSiteInfo, locations(0))
			End If
		End Sub

		Private Shared Function ConsistentAccessibility(ByVal overriding As Symbol, ByVal overridden As Symbol, ByRef errorId As ERRID) As Boolean
			Dim declaredAccessibility As Boolean
			If (Not (overridden.DeclaredAccessibility = Accessibility.ProtectedOrInternal And Not (overriding.ContainingAssembly = overridden.ContainingAssembly))) Then
				errorId = ERRID.ERR_BadOverrideAccess2
				declaredAccessibility = overridden.DeclaredAccessibility = overriding.DeclaredAccessibility
			Else
				errorId = ERRID.ERR_FriendAssemblyBadAccessOverride2
				declaredAccessibility = overriding.DeclaredAccessibility = Accessibility.[Protected]
			End If
			Return declaredAccessibility
		End Function

		Private Shared Function FindOverriddenMembersInType(ByVal overridingSym As TSymbol, ByVal overridingIsFromSomeCompilation As Boolean, ByVal overridingContainingType As NamedTypeSymbol, ByVal currType As NamedTypeSymbol, ByVal overriddenBuilder As ArrayBuilder(Of TSymbol), ByVal inexactOverriddenMembers As ArrayBuilder(Of TSymbol), ByVal inaccessibleBuilder As ArrayBuilder(Of TSymbol)) As Boolean
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = Nothing
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim instance As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
			Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = currType.GetMembers(overridingSym.Name).GetEnumerator()
			While enumerator1.MoveNext()
				OverrideHidingHelper(Of TSymbol).ProcessMemberWithMatchingName(enumerator1.Current, overridingSym, overridingIsFromSomeCompilation, overridingContainingType, inexactOverriddenMembers, inaccessibleBuilder, instance, flag, flag1)
			End While
			If (overridingSym.Kind = SymbolKind.[Property]) Then
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(overridingSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
				If (propertySymbol.IsImplicitlyDeclared AndAlso propertySymbol.IsWithEvents) Then
					Try
						enumerator = currType.GetSynthesizedWithEventsOverrides().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = enumerator.Current
							If (Not current.Name.Equals(propertySymbol.Name)) Then
								Continue While
							End If
							OverrideHidingHelper(Of TSymbol).ProcessMemberWithMatchingName(current, overridingSym, overridingIsFromSomeCompilation, overridingContainingType, inexactOverriddenMembers, inaccessibleBuilder, instance, flag, flag1)
						End While
					Finally
						If (enumerator IsNot Nothing) Then
							enumerator.Dispose()
						End If
					End Try
				End If
			End If
			If (instance.Count > 1) Then
				OverrideHidingHelper(Of TSymbol).RemoveMembersWithConflictingAccessibility(instance)
			End If
			If (instance.Count > 0) Then
				If (flag1) Then
					overriddenBuilder.Clear()
				End If
				If (overriddenBuilder.Count = 0) Then
					overriddenBuilder.AddRange(instance)
				End If
			End If
			instance.Free()
			Return flag
		End Function

		Friend Shared Function MakeOverriddenMembers(ByVal overridingSym As TSymbol) As OverriddenMembersResult(Of TSymbol)
			Dim empty As OverriddenMembersResult(Of TSymbol)
			If (Not overridingSym.IsOverrides OrElse Not OverrideHidingHelper.CanOverrideOrHide(DirectCast(overridingSym, Symbol))) Then
				empty = OverriddenMembersResult(Of TSymbol).Empty
			Else
				Dim dangerousIsFromSomeCompilationIncludingRetargeting As Boolean = overridingSym.Dangerous_IsFromSomeCompilationIncludingRetargeting
				Dim containingType As NamedTypeSymbol = overridingSym.ContainingType
				Dim instance As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
				Dim tSymbols As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
				Dim instance1 As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = containingType.BaseTypeNoUseSiteDiagnostics
				While baseTypeNoUseSiteDiagnostics IsNot Nothing AndAlso Not OverrideHidingHelper(Of TSymbol).FindOverriddenMembersInType(overridingSym, dangerousIsFromSomeCompilationIncludingRetargeting, containingType, baseTypeNoUseSiteDiagnostics, instance, tSymbols, instance1)
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
				End While
				empty = OverriddenMembersResult(Of TSymbol).Create(instance.ToImmutableAndFree(), tSymbols.ToImmutableAndFree(), instance1.ToImmutableAndFree())
			End If
			Return empty
		End Function

		Private Shared Sub ProcessMemberWithMatchingName(ByVal sym As Symbol, ByVal overridingSym As TSymbol, ByVal overridingIsFromSomeCompilation As Boolean, ByVal overridingContainingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal inexactOverriddenMembers As ArrayBuilder(Of TSymbol), ByVal inaccessibleBuilder As ArrayBuilder(Of TSymbol), ByVal overriddenInThisType As ArrayBuilder(Of TSymbol), ByRef stopLookup As Boolean, ByRef haveExactMatch As Boolean)
			Dim flag As Boolean
			Dim originalDefinition As Symbol = sym.OriginalDefinition
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = overridingContainingType.OriginalDefinition
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
			Dim flag1 As Boolean = AccessCheck.IsSymbolAccessible(originalDefinition, namedTypeSymbol, Nothing, discarded, basesBeingResolved)
			If (sym.Kind = overridingSym.Kind AndAlso OverrideHidingHelper.CanOverrideOrHide(sym)) Then
				Dim tSymbol As TSymbol = DirectCast(sym, TSymbol)
				Dim flag2 As Boolean = True
				Dim flag3 As Boolean = True
				If (overridingIsFromSomeCompilation) Then
					flag = If(sym.IsWithEventsProperty() <> DirectCast(overridingSym, Symbol).IsWithEventsProperty(), False, OverrideHidingHelper.SignaturesMatch(DirectCast(overridingSym, Symbol), DirectCast(tSymbol, Symbol), flag2, flag3))
				Else
					flag = OverrideHidingHelper(Of TSymbol).s_runtimeSignatureComparer.Equals(overridingSym, tSymbol)
				End If
				If (Not flag) Then
					If (Not DirectCast(tSymbol, Symbol).IsOverloads() AndAlso flag1) Then
						stopLookup = True
						Return
					End If
				ElseIf (flag1) Then
					If (Not flag3) Then
						OverrideHidingHelper(Of TSymbol).AddMemberToABuilder(tSymbol, inexactOverriddenMembers)
						Return
					End If
					If (flag2) Then
						If (Not haveExactMatch) Then
							haveExactMatch = True
							stopLookup = True
							overriddenInThisType.Clear()
						End If
						overriddenInThisType.Add(tSymbol)
						Return
					End If
					If (Not haveExactMatch) Then
						overriddenInThisType.Add(tSymbol)
						Return
					End If
				ElseIf (flag3) Then
					inaccessibleBuilder.Add(tSymbol)
					Return
				End If
			ElseIf (flag1) Then
				stopLookup = True
			End If
		End Sub

		Private Shared Sub RemoveMembersWithConflictingAccessibility(ByVal members As ArrayBuilder(Of TSymbol))
			If (members.Count >= 2) Then
				Dim instance As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
				Dim enumerator As ArrayBuilder(Of TSymbol).Enumerator = members.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TSymbol = enumerator.Current
					Dim flag As Boolean = False
					Dim enumerator1 As ArrayBuilder(Of TSymbol).Enumerator = members.GetEnumerator()
					While enumerator1.MoveNext()
						Dim tSymbol As TSymbol = enumerator1.Current
						If (current = tSymbol) Then
							Continue While
						End If
						Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbol = current.OriginalDefinition
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = tSymbol.OriginalDefinition
						If (Not TypeSymbol.Equals(originalDefinition.ContainingType, symbol.ContainingType, TypeCompareKind.ConsiderEverything) OrElse CInt(OverrideHidingHelper.DetailedSignatureCompare(originalDefinition, symbol, SymbolComparisonResults.NameMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch, 0)) <> 0 OrElse LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(originalDefinition, symbol) >= 0) Then
							Continue While
						End If
						flag = True
						Exit While
					End While
					If (flag) Then
						Continue While
					End If
					instance.Add(current)
				End While
				If (instance.Count <> members.Count) Then
					members.Clear()
					members.AddRange(instance)
				End If
				instance.Free()
			End If
		End Sub

		Private Shared Sub ReportBadOverriding(ByVal id As ERRID, ByVal overridingMember As Symbol, ByVal overriddenMember As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(id, New [Object]() { overridingMember, overriddenMember })
			Dim locations As ImmutableArray(Of Location) = overridingMember.Locations
			diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
		End Sub
	End Class
End Namespace