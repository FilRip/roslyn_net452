Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module ImplementsHelper
		Public Function ComputeImplementationForInterfaceMember(Of TSymbol As Symbol)(ByVal interfaceMember As TSymbol, ByVal implementingType As TypeSymbol, ByVal comparer As IEqualityComparer(Of TSymbol)) As TSymbol
			Dim tSymbol1 As TSymbol
			Dim containingType As NamedTypeSymbol = interfaceMember.ContainingType
			Dim flag As Boolean = False
			Dim baseTypeNoUseSiteDiagnostics As TypeSymbol = implementingType
			While True
				If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
					Dim item As MultiDictionary(Of Symbol, Symbol).ValueSet = baseTypeNoUseSiteDiagnostics.ExplicitInterfaceImplementationMap(DirectCast(interfaceMember, Symbol))
					If (item.Count = 1) Then
						tSymbol1 = DirectCast(item.[Single](), TSymbol)
						Exit While
					ElseIf (item.Count <= 1) Then
						If (Not baseTypeNoUseSiteDiagnostics.Dangerous_IsFromSomeCompilationIncludingRetargeting AndAlso IReadOnlyListExtensions.Contains(Of NamedTypeSymbol)(DirectCast(baseTypeNoUseSiteDiagnostics.InterfacesNoUseSiteDiagnostics, IReadOnlyList(Of NamedTypeSymbol)), containingType, EqualsIgnoringComparer.InstanceCLRSignatureCompare)) Then
							flag = True
						End If
						If (flag) Then
							Dim tSymbol2 As TSymbol = ImplementsHelper.FindImplicitImplementationDeclaredInType(Of TSymbol)(interfaceMember, baseTypeNoUseSiteDiagnostics, comparer)
							If (tSymbol2 IsNot Nothing) Then
								tSymbol1 = tSymbol2
								Exit While
							End If
						End If
						baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
					Else
						tSymbol1 = Nothing
						Exit While
					End If
				Else
					tSymbol1 = Nothing
					Exit While
				End If
			End While
			Return tSymbol1
		End Function

		Public Function FindExplicitlyImplementedMember(Of TSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)(ByVal implementingSym As TSymbol, ByVal containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal implementedMemberSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal candidateSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef resultKind As LookupResultKind) As TSymbol
			Dim tSymbol1 As TSymbol
			Dim tSymbol2 As TSymbol
			resultKind = LookupResultKind.Good
			Dim left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax = implementedMemberSyntax.Left
			Dim valueText As String = implementedMemberSyntax.Right.Identifier.ValueText
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.BindTypeSyntax(left, diagBag, False, False, False)
			If (typeSymbol.IsInterfaceType()) Then
				Dim flag As Boolean = False
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (Not containingType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics(namedTypeSymbol).Contains(namedTypeSymbol)) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, left, ERRID.ERR_InterfaceNotImplemented1, New [Object]() { typeSymbol })
					resultKind = LookupResultKind.NotReferencable
					flag = True
				End If
				Dim instance As LookupResult = LookupResult.GetInstance()
				Dim tSymbol3 As TSymbol = Nothing
				Dim lookupOption As LookupOptions = LookupOptions.IgnoreAccessibility Or LookupOptions.AllMethodsOfAnyArity Or LookupOptions.IgnoreExtensionMethods
				If (implementingSym.Kind = SymbolKind.[Event]) Then
					lookupOption = lookupOption Or LookupOptions.EventsOnly
				End If
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = binder.GetNewCompoundUseSiteInfo(diagBag)
				binder.LookupMember(instance, typeSymbol, valueText, -1, lookupOption, newCompoundUseSiteInfo)
				If (instance.IsAmbiguous) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplementsMember3, New [Object]() { valueText, valueText })
					If (candidateSymbols IsNot Nothing) Then
						candidateSymbols.AddRange(DirectCast(instance.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
					End If
					resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.Ambiguous)
					flag = True
				ElseIf (instance.IsGood) Then
					Dim tSymbols As ArrayBuilder(Of TSymbol) = Nothing
					Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = instance.Symbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TSymbol = DirectCast(TryCast(enumerator.Current, TSymbol), TSymbol)
						If (current Is Nothing OrElse Not current.ContainingType.IsInterface OrElse Not ImplementsHelper.MembersAreMatchingForPurposesOfInterfaceImplementation(DirectCast(implementingSym, Microsoft.CodeAnalysis.VisualBasic.Symbol), DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbol))) Then
							Continue While
						End If
						If (tSymbols Is Nothing) Then
							tSymbols = ArrayBuilder(Of TSymbol).GetInstance()
						End If
						tSymbols.Add(current)
					End While
					Dim num As Integer = If(tSymbols IsNot Nothing, tSymbols.Count, 0)
					If (num > 1) Then
						Dim count As Integer = tSymbols.Count - 2
						For i As Integer = 0 To count
							Dim item As TSymbol = tSymbols(i)
							If (item IsNot Nothing) Then
								Dim count1 As Integer = tSymbols.Count - 1
								For j As Integer = i + 1 To count1
									Dim item1 As TSymbol = tSymbols(j)
									If (item1 IsNot Nothing) Then
										Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item1.ContainingType
										Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item.ContainingType
										Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
										If (Not namedTypeSymbol1.ImplementsInterface(namedTypeSymbol2, Nothing, discarded)) Then
											Dim namedTypeSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item.ContainingType
											Dim namedTypeSymbol4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item1.ContainingType
											discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
											If (namedTypeSymbol3.ImplementsInterface(namedTypeSymbol4, Nothing, discarded)) Then
												tSymbol2 = Nothing
												tSymbols(j) = tSymbol2
												num = num - 1
											End If
										Else
											tSymbol2 = Nothing
											tSymbols(i) = tSymbol2
											num = num - 1
											Exit For
										End If
									End If
								Next

							End If
						Next

					End If
					If (num > 1) Then
						Dim num1 As Integer = tSymbols.Count - 2
						Dim num2 As Integer = 0
						While True
							If (num2 <= num1) Then
								Dim item2 As TSymbol = tSymbols(num2)
								If (item2 IsNot Nothing) Then
									If (tSymbol3 Is Nothing) Then
										tSymbol3 = item2
									End If
									Dim count2 As Integer = tSymbols.Count - 1
									Dim num3 As Integer = num2 + 1
									While num3 <= count2
										Dim item3 As TSymbol = tSymbols(num3)
										If (item3 Is Nothing OrElse Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(item2.ContainingType, item3.ContainingType, TypeCompareKind.ConsiderEverything)) Then
											num3 = num3 + 1
										Else
											Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplements3, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(item2.ContainingType), valueText, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(item2.ContainingType), item2, item3 })
											flag = True
											resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.OverloadResolutionFailure)
											GoTo Label1
										End If
									End While
								End If
								num2 = num2 + 1
							Else
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplementsMember3, New [Object]() { valueText, valueText })
								resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.Ambiguous)
								flag = True
								Exit While
							End If
						End While
					Label1:
						If (candidateSymbols IsNot Nothing) Then
							candidateSymbols.AddRange(instance.Symbols)
						End If
					ElseIf (num <> 1) Then
						If (candidateSymbols IsNot Nothing) Then
							candidateSymbols.AddRange(instance.Symbols)
						End If
						resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.OverloadResolutionFailure)
					Else
						Dim count3 As Integer = tSymbols.Count - 1
						Dim num4 As Integer = 0
						While num4 <= count3
							Dim item4 As TSymbol = tSymbols(num4)
							If (item4 Is Nothing) Then
								num4 = num4 + 1
							Else
								tSymbol3 = item4
								GoTo Label0
							End If
						End While
					End If
				Label0:
					If (tSymbols IsNot Nothing) Then
						tSymbols.Free()
					End If
					If (tSymbol3 IsNot Nothing) Then
						If (CObj(namedTypeSymbol.CoClassType) <> CObj(Nothing) AndAlso implementingSym.Kind = SymbolKind.[Event] <> (tSymbol3.Kind = SymbolKind.[Event])) Then
							tSymbol3 = Nothing
						End If
						If (Not flag) Then
							tSymbol3 = ImplementsHelper.ValidateImplementedMember(Of TSymbol)(implementingSym, tSymbol3, implementedMemberSyntax, binder, diagBag, typeSymbol, valueText, flag)
						End If
						If (tSymbol3 IsNot Nothing) Then
							If (candidateSymbols IsNot Nothing) Then
								candidateSymbols.Add(DirectCast(tSymbol3, Microsoft.CodeAnalysis.VisualBasic.Symbol))
							End If
							resultKind = LookupResult.WorseResultKind(DirectCast(CInt(resultKind), LookupResultKind), instance.Kind)
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = DirectCast(tSymbol3, Microsoft.CodeAnalysis.VisualBasic.Symbol)
							Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
							If (Not binder.IsAccessible(symbol, newCompoundUseSiteInfo, Nothing, basesBeingResolved)) Then
								resultKind = LookupResult.WorseResultKind(DirectCast(CInt(resultKind), LookupResultKind), LookupResultKind.Inaccessible)
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, binder.GetInaccessibleErrorInfo(DirectCast(tSymbol3, Microsoft.CodeAnalysis.VisualBasic.Symbol)))
							ElseIf (tSymbol3.Kind = SymbolKind.[Property]) Then
								Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(DirectCast(tSymbol3, Microsoft.CodeAnalysis.VisualBasic.Symbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
								Dim getMethod As MethodSymbol = propertySymbol.GetMethod
								If (getMethod Is Nothing OrElse getMethod.DeclaredAccessibility = propertySymbol.DeclaredAccessibility OrElse Not getMethod.RequiresImplementation()) Then
									getMethod = propertySymbol.SetMethod
								End If
								If (getMethod IsNot Nothing AndAlso getMethod.DeclaredAccessibility <> propertySymbol.DeclaredAccessibility AndAlso getMethod.RequiresImplementation()) Then
									basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
									If (Not binder.IsAccessible(getMethod, newCompoundUseSiteInfo, Nothing, basesBeingResolved)) Then
										Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, binder.GetInaccessibleErrorInfo(getMethod))
									End If
								End If
							End If
						End If
					End If
				End If
				diagBag.Add(left, newCompoundUseSiteInfo)
				instance.Free()
				If (tSymbol3 Is Nothing And Not flag) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_IdentNotMemberOfInterface4, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(DirectCast(implementingSym, Microsoft.CodeAnalysis.VisualBasic.Symbol)), valueText, DirectCast(implementingSym, Microsoft.CodeAnalysis.VisualBasic.Symbol).GetKindText(), CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(typeSymbol) })
				End If
				tSymbol1 = tSymbol3
			ElseIf (typeSymbol.TypeKind <> TypeKind.[Error]) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, left, ERRID.ERR_BadImplementsType)
				tSymbol1 = Nothing
			Else
				tSymbol1 = Nothing
			End If
			Return tSymbol1
		End Function

		Public Function FindImplementingSyntax(Of TSymbol As Symbol)(ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax, ByVal implementingSym As TSymbol, ByVal implementedSym As TSymbol, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = 0
			Dim enumerator As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax).Enumerator = implementsClause.InterfaceMembers.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = enumerator.Current
					If (DirectCast(ImplementsHelper.FindExplicitlyImplementedMember(Of TSymbol)(implementingSym, container, current, binder, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, Nothing, lookupResultKind), Symbol) = DirectCast(implementedSym, Symbol)) Then
						qualifiedNameSyntax = current
						Exit While
					End If
				Else
					qualifiedNameSyntax = Nothing
					Exit While
				End If
			End While
			Return qualifiedNameSyntax
		End Function

		Private Function FindImplicitImplementationDeclaredInType(Of TSymbol As Symbol)(ByVal interfaceMember As TSymbol, ByVal currType As TypeSymbol, ByVal comparer As IEqualityComparer(Of TSymbol)) As TSymbol
			Dim tSymbol1 As TSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = currType.GetMembers(interfaceMember.Name).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					If (current.DeclaredAccessibility = Accessibility.[Public] AndAlso Not current.IsShared AndAlso TypeOf current Is TSymbol AndAlso comparer.Equals(interfaceMember, DirectCast(current, TSymbol))) Then
						tSymbol1 = DirectCast(current, TSymbol)
						Exit While
					End If
				Else
					tSymbol1 = Nothing
					Exit While
				End If
			End While
			Return tSymbol1
		End Function

		Public Function GetExplicitInterfaceImplementations(ByVal member As Symbol) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim kind As SymbolKind = member.Kind
			If (kind = SymbolKind.[Event]) Then
				empty = StaticCast(Of Symbol).From(Of EventSymbol)(DirectCast(member, EventSymbol).ExplicitInterfaceImplementations)
			ElseIf (kind = SymbolKind.Method) Then
				empty = StaticCast(Of Symbol).From(Of MethodSymbol)(DirectCast(member, MethodSymbol).ExplicitInterfaceImplementations)
			ElseIf (kind = SymbolKind.[Property]) Then
				empty = StaticCast(Of Symbol).From(Of PropertySymbol)(DirectCast(member, PropertySymbol).ExplicitInterfaceImplementations)
			Else
				empty = ImmutableArray(Of Symbol).Empty
			End If
			Return empty
		End Function

		Public Function GetImplementingLocation(ByVal sourceSym As Symbol, ByVal implementedSym As Symbol) As Location
			Dim implementingLocation As Location
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(sourceSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			If (sourceMethodSymbol Is Nothing) Then
				Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = TryCast(sourceSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
				If (sourcePropertySymbol Is Nothing) Then
					Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = TryCast(sourceSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol)
					If (sourceEventSymbol Is Nothing) Then
						Throw ExceptionUtilities.Unreachable
					End If
					implementingLocation = sourceEventSymbol.GetImplementingLocation(DirectCast(implementedSym, EventSymbol))
				Else
					implementingLocation = sourcePropertySymbol.GetImplementingLocation(DirectCast(implementedSym, PropertySymbol))
				End If
			Else
				implementingLocation = sourceMethodSymbol.GetImplementingLocation(DirectCast(implementedSym, MethodSymbol))
			End If
			Return implementingLocation
		End Function

		Private Function MembersAreMatching(ByVal implementingSym As Symbol, ByVal implementedSym As Symbol, ByVal comparisons As SymbolComparisonResults, ByVal eventComparer As EventSignatureComparer) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = implementingSym.Kind
			If (kind = SymbolKind.[Event]) Then
				flag = eventComparer.Equals(DirectCast(implementedSym, EventSymbol), DirectCast(implementingSym, EventSymbol))
			ElseIf (kind = SymbolKind.Method) Then
				flag = CInt(MethodSignatureComparer.DetailedCompare(DirectCast(implementedSym, MethodSymbol), DirectCast(implementingSym, MethodSymbol), comparisons, comparisons)) = 0
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(implementingSym.Kind)
				End If
				flag = CInt(PropertySignatureComparer.DetailedCompare(DirectCast(implementedSym, PropertySymbol), DirectCast(implementingSym, PropertySymbol), comparisons, comparisons)) = 0
			End If
			Return flag
		End Function

		Private Function MembersAreMatchingForPurposesOfInterfaceImplementation(ByVal implementingSym As Symbol, ByVal implementedSym As Symbol) As Boolean
			Return ImplementsHelper.MembersAreMatching(implementingSym, implementedSym, SymbolComparisonResults.ReturnTypeMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.CallingConventionMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.OptionalParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterValueMismatch Or SymbolComparisonResults.ParameterByrefMismatch Or SymbolComparisonResults.ParamArrayMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch, EventSignatureComparer.ExplicitEventImplementationComparer)
		End Function

		Private Function MembersHaveMatchingTupleNames(ByVal implementingSym As Symbol, ByVal implementedSym As Symbol) As Boolean
			Return ImplementsHelper.MembersAreMatching(implementingSym, implementedSym, SymbolComparisonResults.TupleNamesMismatch, EventSignatureComparer.ExplicitEventImplementationWithTupleNamesComparer)
		End Function

		Public Function ProcessImplementsClause(Of TSymbol As Symbol)(ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax, ByVal implementingSym As TSymbol, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TSymbol)
			Dim empty As ImmutableArray(Of TSymbol)
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = 0
			If (container.IsInterface) Then
				If (implementingSym.Kind <> SymbolKind.Method) Then
					eRRID = If(implementingSym.Kind <> SymbolKind.[Property], Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InterfaceCantUseEventSpecifier1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadInterfacePropertyFlags1)
				Else
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadInterfaceMethodFlags1
				End If
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementsClause, eRRID, New [Object]() { implementsClause.ImplementsKeyword.ToString() })
				empty = ImmutableArray(Of TSymbol).Empty
			ElseIf (Not container.IsModuleType()) Then
				Dim instance As ArrayBuilder(Of TSymbol) = ArrayBuilder(Of TSymbol).GetInstance()
				Dim threeState As Microsoft.CodeAnalysis.ThreeState = Microsoft.CodeAnalysis.ThreeState.Unknown
				Dim kind As Boolean = implementingSym.Kind = SymbolKind.[Event]
				Dim enumerator As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax).Enumerator = implementsClause.InterfaceMembers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = enumerator.Current
					Dim tSymbol1 As TSymbol = ImplementsHelper.FindExplicitlyImplementedMember(Of TSymbol)(implementingSym, container, current, binder, diagBag, Nothing, lookupResultKind)
					If (tSymbol1 Is Nothing) Then
						Continue While
					End If
					instance.Add(tSymbol1)
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnosticsIfObsolete(diagBag, DirectCast(implementingSym, Symbol), DirectCast(tSymbol1, Symbol), implementsClause)
					If (Not kind) Then
						Continue While
					End If
					If (threeState.HasValue()) Then
						Dim isWindowsRuntimeEvent As Boolean = TryCast(tSymbol1, EventSymbol).IsWindowsRuntimeEvent
						Dim flag As Boolean = threeState.Value()
						If (isWindowsRuntimeEvent = flag) Then
							Continue While
						End If
						Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagBag
						Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken = current
						Dim objArray() As [Object] = { CustomSymbolDisplayFormatter.ShortErrorName(DirectCast(implementingSym, Symbol)), Nothing, Nothing }
						objArray(1) = CustomSymbolDisplayFormatter.QualifiedName(DirectCast(If(flag, instance(0), tSymbol1), Symbol))
						objArray(2) = CustomSymbolDisplayFormatter.QualifiedName(DirectCast(If(flag, tSymbol1, instance(0)), Symbol))
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(bindingDiagnosticBag, syntaxNodeOrToken, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MixingWinRTAndNETEvents, objArray)
					Else
						threeState = TryCast(tSymbol1, EventSymbol).IsWindowsRuntimeEvent.ToThreeState()
					End If
				End While
				empty = instance.ToImmutableAndFree()
			Else
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementsClause.ImplementsKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ModuleMemberCantImplement)
				empty = ImmutableArray(Of TSymbol).Empty
			End If
			Return empty
		End Function

		Public Function SubstituteExplicitInterfaceImplementations(Of TSymbol As Symbol)(ByVal unsubstitutedImplementations As ImmutableArray(Of TSymbol), ByVal substitution As TypeSubstitution) As ImmutableArray(Of TSymbol)
			Dim empty As ImmutableArray(Of TSymbol)
			If (unsubstitutedImplementations.Length <> 0) Then
				Dim item(unsubstitutedImplementations.Length - 1 + 1 - 1) As TSymbol
				Dim length As Integer = unsubstitutedImplementations.Length - 1
				Dim num As Integer = 0
				Do
					Dim item1 As TSymbol = unsubstitutedImplementations(num)
					Dim containingType As NamedTypeSymbol = item1.ContainingType
					item(num) = unsubstitutedImplementations(num)
					If (containingType.IsGenericType) Then
						Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = TryCast(containingType.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
						If (substitutedNamedType IsNot Nothing) Then
							item(num) = DirectCast(substitutedNamedType.GetMemberForDefinition(item1.OriginalDefinition), TSymbol)
						End If
					End If
					num = num + 1
				Loop While num <= length
				empty = ImmutableArray.Create(Of TSymbol)(item)
			Else
				empty = ImmutableArray(Of TSymbol).Empty
			End If
			Return empty
		End Function

		Private Function ValidateImplementedMember(Of TSymbol As Symbol)(ByVal implementingSym As TSymbol, ByVal implementedSym As TSymbol, ByVal implementedMemberSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal interfaceType As TypeSymbol, ByVal implementedMethodName As String, ByRef errorReported As Boolean) As TSymbol
			Dim tSymbol1 As TSymbol
			Dim nullable As Nullable(Of Boolean)
			Dim nullable1 As Nullable(Of Boolean)
			Dim nullable2 As Nullable(Of Boolean)
			Dim nullable3 As Nullable(Of Boolean)
			Dim nullable4 As Nullable(Of Boolean)
			Dim nullable5 As Nullable(Of Boolean)
			Dim nullable6 As Nullable(Of Boolean)
			If (DirectCast(implementedSym, Symbol).RequiresImplementation()) Then
				If (implementedSym.Kind = SymbolKind.[Property]) Then
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(implementedSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					Dim getMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = propertySymbol.GetMethod
					If (getMethod IsNot Nothing) Then
						nullable2 = New Nullable(Of Boolean)(getMethod.RequiresImplementation())
					Else
						nullable = Nothing
						nullable2 = nullable
					End If
					Dim nullable7 As Nullable(Of Boolean) = nullable2
					nullable7 = If(nullable7.HasValue, New Nullable(Of Boolean)(Not nullable7.GetValueOrDefault()), nullable7)
					If (nullable7.GetValueOrDefault()) Then
						getMethod = Nothing
					End If
					Dim setMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = propertySymbol.SetMethod
					If (setMethod IsNot Nothing) Then
						nullable3 = New Nullable(Of Boolean)(setMethod.RequiresImplementation())
					Else
						nullable = Nothing
						nullable3 = nullable
					End If
					nullable7 = nullable3
					nullable7 = If(nullable7.HasValue, New Nullable(Of Boolean)(Not nullable7.GetValueOrDefault()), nullable7)
					If (nullable7.GetValueOrDefault()) Then
						setMethod = Nothing
					End If
					Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(implementingSym, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					If (getMethod IsNot Nothing AndAlso propertySymbol1.GetMethod Is Nothing OrElse setMethod IsNot Nothing AndAlso propertySymbol1.SetMethod Is Nothing) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_PropertyDoesntImplementAllAccessors, New [Object]() { propertySymbol, propertySymbol1.GetPropertyKindText() })
						errorReported = True
					ElseIf (getMethod Is Nothing Xor setMethod Is Nothing AndAlso propertySymbol1.GetMethod IsNot Nothing AndAlso propertySymbol1.SetMethod IsNot Nothing) Then
						errorReported = errorReported Or Not Parser.CheckFeatureAvailability(diagBag, implementedMemberSyntax.GetLocation(), DirectCast(implementedMemberSyntax.SyntaxTree, VisualBasicSyntaxTree).Options.LanguageVersion, Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite)
					End If
					If (setMethod IsNot Nothing) Then
						nullable4 = New Nullable(Of Boolean)(setMethod.IsInitOnly)
					Else
						nullable1 = Nothing
						nullable4 = nullable1
					End If
					nullable7 = nullable4
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = propertySymbol1.SetMethod
					If (methodSymbol IsNot Nothing) Then
						nullable5 = New Nullable(Of Boolean)(methodSymbol.IsInitOnly)
					Else
						nullable1 = Nothing
						nullable5 = nullable1
					End If
					nullable = nullable5
					If (nullable7.HasValue And nullable.HasValue) Then
						nullable6 = New Nullable(Of Boolean)(nullable7.GetValueOrDefault() <> nullable.GetValueOrDefault())
					Else
						nullable1 = Nothing
						nullable6 = nullable1
					End If
					nullable = nullable6
					If (nullable.GetValueOrDefault()) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_PropertyDoesntImplementInitOnly, New [Object]() { propertySymbol })
						errorReported = True
					End If
				End If
				If (implementedSym IsNot Nothing AndAlso DirectCast(implementingSym, Symbol).ContainsTupleNames() AndAlso Not ImplementsHelper.MembersHaveMatchingTupleNames(DirectCast(implementingSym, Symbol), DirectCast(implementedSym, Symbol))) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_ImplementingInterfaceWithDifferentTupleNames5, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(DirectCast(implementingSym, Symbol)), DirectCast(implementingSym, Symbol).GetKindText(), implementedMethodName, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(interfaceType), implementingSym, implementedSym })
					errorReported = True
				End If
				tSymbol1 = implementedSym
			Else
				tSymbol1 = Nothing
			End If
			Return tSymbol1
		End Function

		Public Sub ValidateImplementedMethodConstraints(ByVal implementingMethod As SourceMethodSymbol, ByVal implementedMethod As MethodSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not MethodSignatureComparer.HaveSameConstraints(implementedMethod, implementingMethod)) Then
				Dim implementingLocation As Location = implementingMethod.GetImplementingLocation(implementedMethod)
				diagBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ImplementsWithConstraintMismatch3, New [Object]() { implementingMethod, implementedMethod.ContainingType, implementedMethod }), implementingLocation)
			End If
		End Sub
	End Module
End Namespace