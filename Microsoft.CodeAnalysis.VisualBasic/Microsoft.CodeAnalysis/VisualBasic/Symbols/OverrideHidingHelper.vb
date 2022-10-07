Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class OverrideHidingHelper
		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function AssociatedSymbolName(ByVal associatedSymbol As Symbol) As String
			If (Not associatedSymbol.IsUserDefinedOperator()) Then
				Return associatedSymbol.Name
			End If
			Return SyntaxFacts.GetText(OverloadResolution.GetOperatorTokenKind(associatedSymbol.Name))
		End Function

		Friend Shared Function CanOverrideOrHide(ByVal sym As Symbol) As Boolean
			Dim flag As Boolean
			If (sym.Kind = SymbolKind.Method) Then
				Select Case DirectCast(sym, MethodSymbol).MethodKind
					Case MethodKind.AnonymousFunction
					Case MethodKind.Constructor
					Case MethodKind.StaticConstructor
						flag = False
						Exit Select
					Case MethodKind.Conversion
					Case MethodKind.DelegateInvoke
					Case MethodKind.EventAdd
					Case MethodKind.EventRaise
					Case MethodKind.EventRemove
					Case MethodKind.UserDefinedOperator
					Case MethodKind.Ordinary
					Case MethodKind.PropertyGet
					Case MethodKind.PropertySet
					Case MethodKind.DeclareMethod
						flag = True
						Exit Select
					Case MethodKind.Destructor
					Case MethodKind.ExplicitInterfaceImplementation
					Case MethodKind.ReducedExtension
					Case MethodKind.BuiltinOperator
					Label0:
						flag = False
						Exit Select
					Case Else
						GoTo Label0
				End Select
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Sub CheckAllAbstractsAreOverriddenAndNotHidden(ByVal container As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim locations As ImmutableArray(Of Location)
			If (Not container.IsMustInherit AndAlso Not container.IsNotInheritable) Then
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = container.GetMembers().GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						GoTo Label0
					End If
				Loop While Not enumerator.Current.IsMustOverride
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MustOverridesInClass1, New [Object]() { container.Name })
				locations = container.Locations
				diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
			End If
		Label0:
			Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = container.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics IsNot Nothing AndAlso baseTypeNoUseSiteDiagnostics.IsMustInherit) Then
				Dim symbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = container
				While namedTypeSymbol IsNot Nothing
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = namedTypeSymbol.GetMembers().GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (Not OverrideHidingHelper.CanOverrideOrHide(current) OrElse current.IsAccessor()) Then
							Continue While
						End If
						If (current.IsOverrides AndAlso OverrideHidingHelper.GetOverriddenMember(current) IsNot Nothing) Then
							symbols.Add(OverrideHidingHelper.GetOverriddenMember(current))
						End If
						If (Not current.IsMustOverride OrElse CObj(namedTypeSymbol) = CObj(container) OrElse symbols.Contains(current)) Then
							Continue While
						End If
						instance.Add(current)
					End While
					namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics
				End While
				If (instance.Any()) Then
					If (Not container.IsMustInherit) Then
						Dim diagnosticInfos As ArrayBuilder(Of Microsoft.CodeAnalysis.DiagnosticInfo) = ArrayBuilder(Of Microsoft.CodeAnalysis.DiagnosticInfo).GetInstance(instance.Count)
						Dim enumerator2 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = instance.GetEnumerator()
						While enumerator2.MoveNext()
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
							If (symbol.IsAccessor()) Then
								Continue While
							End If
							If (symbol.Kind <> SymbolKind.[Event]) Then
								diagnosticInfos.Add(ErrorFactory.ErrorInfo(ERRID.ERR_UnimplementedMustOverride, New [Object]() { symbol.ContainingType, symbol }))
							Else
								Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MustInheritEventNotOverridden, New [Object]() { symbol, CustomSymbolDisplayFormatter.QualifiedName(symbol.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(container) })
								locations = container.Locations
								diagnostics.Add(New VBDiagnostic(diagnosticInfo1, locations(0), False))
							End If
						End While
						If (diagnosticInfos.Count <= 0) Then
							diagnosticInfos.Free()
						Else
							Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_BaseOnlyClassesMustBeExplicit2, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(container), New CompoundDiagnosticInfo(diagnosticInfos.ToArrayAndFree()) })
							locations = container.Locations
							diagnostics.Add(New VBDiagnostic(diagnosticInfo2, locations(0), False))
						End If
					Else
						Dim symbols1 As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
						Dim enumerator3 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = instance.GetEnumerator()
						While enumerator3.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator3.Current
							Dim enumerator4 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = container.GetMembers(current1.Name).GetEnumerator()
							While enumerator4.MoveNext()
								Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator4.Current
								If (Not OverrideHidingHelper.DoesHide(symbol1, current1) OrElse symbols1.Contains(symbol1)) Then
									Continue While
								End If
								OverrideHidingHelper.ReportShadowingMustOverrideError(symbol1, current1, diagnostics)
								symbols1.Add(symbol1)
							End While
						End While
					End If
				End If
				instance.Free()
			End If
		End Sub

		Public Shared Sub CheckHidingAndOverridingForType(ByVal container As SourceMemberContainerTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = container.TypeKind
			If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Interface] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
				OverrideHidingHelper.CheckMembersAgainstBaseType(container, diagnostics)
				OverrideHidingHelper.CheckAllAbstractsAreOverriddenAndNotHidden(container, diagnostics)
			End If
		End Sub

		Private Shared Sub CheckMembersAgainstBaseType(ByVal container As SourceMemberContainerTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim locations As ImmutableArray(Of Location)
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = container.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (Not OverrideHidingHelper.CanOverrideOrHide(current)) Then
					Continue While
				End If
				Dim kind As SymbolKind = current.Kind
				If (kind = SymbolKind.Method) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (Not methodSymbol.IsAccessor()) Then
						If (methodSymbol.IsOverrides) Then
							OverrideHidingHelper(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).CheckOverrideMember(methodSymbol, methodSymbol.OverriddenMembers, diagnostics)
						ElseIf (methodSymbol.IsNotOverridable) Then
							Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_NotOverridableRequiresOverrides)
							locations = methodSymbol.Locations
							diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
						End If
					End If
				ElseIf (kind = SymbolKind.[Property]) Then
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					If (propertySymbol.IsOverrides) Then
						OverrideHidingHelper(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol).CheckOverrideMember(propertySymbol, propertySymbol.OverriddenMembers, diagnostics)
					ElseIf (propertySymbol.IsNotOverridable) Then
						Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_NotOverridableRequiresOverrides)
						locations = propertySymbol.Locations
						diagnostics.Add(New VBDiagnostic(diagnosticInfo1, locations(0), False))
					End If
				End If
				OverrideHidingHelper.CheckShadowing(container, current, diagnostics)
			End While
		End Sub

		Protected Shared Sub CheckShadowing(ByVal container As SourceMemberContainerTypeSymbol, ByVal member As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim flag As Boolean = member.IsOverloads()
			Dim shadowsExplicitly As Boolean = Not member.ShadowsExplicitly
			If (shadowsExplicitly) Then
				If (container.IsInterfaceType()) Then
					Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = container.AllInterfacesNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						OverrideHidingHelper.CheckShadowingInBaseType(container, member, flag, current, diagnostics, shadowsExplicitly)
					End While
					Return
				End If
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = container.BaseTypeNoUseSiteDiagnostics
				While baseTypeNoUseSiteDiagnostics IsNot Nothing
					OverrideHidingHelper.CheckShadowingInBaseType(container, member, flag, baseTypeNoUseSiteDiagnostics, diagnostics, shadowsExplicitly)
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
				End While
			End If
		End Sub

		Private Shared Sub CheckShadowingInBaseType(ByVal container As SourceMemberContainerTypeSymbol, ByVal member As Symbol, ByVal memberIsOverloads As Boolean, ByVal baseType As NamedTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef warnForHiddenMember As Boolean)
			If (warnForHiddenMember) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = baseType.GetMembers(member.Name).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
					If (Not AccessCheck.IsSymbolAccessible(current, container, Nothing, discarded, basesBeingResolved) OrElse memberIsOverloads AndAlso current.Kind = member.Kind AndAlso Not current.IsWithEventsProperty() AndAlso (member.Kind <> SymbolKind.Method OrElse DirectCast(member, MethodSymbol).IsUserDefinedOperator() = DirectCast(current, MethodSymbol).IsUserDefinedOperator()) AndAlso member.IsAccessor() = current.IsAccessor() OrElse member.IsAccessor() AndAlso current.IsAccessor() OrElse member.Kind = SymbolKind.NamedType AndAlso current.Kind = SymbolKind.NamedType AndAlso member.GetArity() <> current.GetArity()) Then
						Continue While
					End If
					OverrideHidingHelper.ReportShadowingDiagnostic(member, current, diagnostics)
					warnForHiddenMember = False
					Return
				End While
			End If
		End Sub

		Friend Shared Function DetailedSignatureCompare(ByVal sym1 As Symbol, ByVal sym2 As Symbol, ByVal comparisons As SymbolComparisonResults, Optional ByVal stopIfAny As SymbolComparisonResults = 0) As SymbolComparisonResults
			Dim symbolComparisonResult As SymbolComparisonResults
			symbolComparisonResult = If(sym1.Kind <> SymbolKind.[Property], MethodSignatureComparer.DetailedCompare(DirectCast(sym1, MethodSymbol), DirectCast(sym2, MethodSymbol), comparisons, stopIfAny), PropertySignatureComparer.DetailedCompare(DirectCast(sym1, PropertySymbol), DirectCast(sym2, PropertySymbol), comparisons, stopIfAny))
			Return symbolComparisonResult
		End Function

		Private Shared Function DoesHide(ByVal hidingMember As Symbol, ByVal hiddenMember As Symbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim kind As SymbolKind = hidingMember.Kind
			If (kind = SymbolKind.Method) Then
				If (Not hidingMember.IsOverloads() OrElse hiddenMember.Kind <> SymbolKind.Method) Then
					flag = True
				Else
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(hidingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (Not methodSymbol.IsOverrides) Then
						Dim flag2 As Boolean = False
						flag1 = False
						flag = If(Not OverrideHidingHelper.SignaturesMatch(methodSymbol, DirectCast(hiddenMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), flag1, flag2), False, flag2)
					Else
						flag = False
					End If
				End If
			ElseIf (kind <> SymbolKind.[Property]) Then
				flag = True
			ElseIf (Not hidingMember.IsOverloads() OrElse hiddenMember.Kind <> SymbolKind.[Property]) Then
				flag = True
			Else
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(hidingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
				If (Not propertySymbol.IsOverrides) Then
					Dim flag3 As Boolean = False
					flag1 = False
					flag = If(Not OverrideHidingHelper.SignaturesMatch(propertySymbol, DirectCast(hiddenMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol), flag1, flag3), False, flag3)
				Else
					flag = False
				End If
			End If
			Return flag
		End Function

		Protected Shared Function GetOverriddenMember(ByVal sym As Symbol) As Symbol
			Dim overriddenEvent As Symbol
			Dim kind As SymbolKind = sym.Kind
			If (kind = SymbolKind.[Event]) Then
				overriddenEvent = DirectCast(sym, EventSymbol).OverriddenEvent
			ElseIf (kind = SymbolKind.Method) Then
				overriddenEvent = DirectCast(sym, MethodSymbol).OverriddenMethod
			ElseIf (kind = SymbolKind.[Property]) Then
				overriddenEvent = DirectCast(sym, PropertySymbol).OverriddenProperty
			Else
				overriddenEvent = Nothing
			End If
			Return overriddenEvent
		End Function

		Private Shared Sub ReportShadowingDiagnostic(ByVal hidingMember As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal hiddenMember As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim locations As ImmutableArray(Of Location)
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim implicitlyDefinedBy As Microsoft.CodeAnalysis.VisualBasic.Symbol = hiddenMember(Nothing)
			If (implicitlyDefinedBy Is Nothing AndAlso hiddenMember.IsUserDefinedOperator() AndAlso Not hidingMember.IsUserDefinedOperator()) Then
				implicitlyDefinedBy = hiddenMember
			End If
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = hidingMember(Nothing)
			If (symbol Is Nothing AndAlso hidingMember.IsUserDefinedOperator() AndAlso Not hiddenMember.IsUserDefinedOperator()) Then
				symbol = hidingMember
			End If
			If (implicitlyDefinedBy Is Nothing) Then
				If (symbol IsNot Nothing) Then
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5, New [Object]() { symbol.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(symbol), hidingMember.Name, hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
					locations = symbol.Locations
					diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
					Return
				End If
				If (hidingMember.Kind = hiddenMember.Kind AndAlso (hidingMember.Kind = SymbolKind.[Property] OrElse hidingMember.Kind = SymbolKind.Method) AndAlso Not hiddenMember.IsWithEventsProperty() AndAlso Not hidingMember.IsWithEventsProperty()) Then
					eRRID = If(hiddenMember.IsOverridable OrElse hiddenMember.IsOverrides OrElse hiddenMember.IsMustOverride AndAlso Not hiddenMember.ContainingType.IsInterface, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverloadBase4)
					Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(eRRID, New [Object]() { hidingMember.GetKindText(), hidingMember.Name, hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
					locations = hidingMember.Locations
					diagnostics.Add(New VBDiagnostic(diagnosticInfo1, locations(0), False))
					Return
				End If
				Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5, New [Object]() { hidingMember.GetKindText(), hidingMember.Name, hiddenMember.GetKindText(), hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
				locations = hidingMember.Locations
				diagnostics.Add(New VBDiagnostic(diagnosticInfo2, locations(0), False))
			Else
				If (symbol Is Nothing) Then
					Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MemberShadowsSynthMember6, New [Object]() { hidingMember.GetKindText(), hidingMember.Name, implicitlyDefinedBy.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(implicitlyDefinedBy), hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
					locations = hidingMember.Locations
					diagnostics.Add(New VBDiagnostic(diagnosticInfo3, locations(0), False))
					Return
				End If
				If (Not CaseInsensitiveComparison.Equals(implicitlyDefinedBy.Name, symbol.Name)) Then
					Dim diagnosticInfo4 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsSynthMember7, New [Object]() { symbol.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(symbol), hidingMember.Name, implicitlyDefinedBy.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(implicitlyDefinedBy), hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
					locations = hidingMember.Locations
					diagnostics.Add(New VBDiagnostic(diagnosticInfo4, locations(0), False))
					Return
				End If
			End If
		End Sub

		Private Shared Sub ReportShadowingMustOverrideError(ByVal hidingMember As Symbol, ByVal hiddenMember As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim locations As ImmutableArray(Of Location)
			If (Not hidingMember.IsAccessor()) Then
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_CantShadowAMustOverride1, New [Object]() { hidingMember })
				locations = hidingMember.Locations
				diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
				Return
			End If
			Dim associatedSymbol As Symbol = DirectCast(hidingMember, MethodSymbol).AssociatedSymbol
			Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_SynthMemberShadowsMustOverride5, New [Object]() { hidingMember, associatedSymbol.GetKindText(), associatedSymbol.Name, hiddenMember.ContainingType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType) })
			locations = hidingMember.Locations
			diagnostics.Add(New VBDiagnostic(diagnosticInfo1, locations(0), False))
		End Sub

		Public Shared Function RequiresExplicitOverride(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			If (Not method.IsAccessor()) Then
				If (method.OverriddenMethod IsNot Nothing) Then
					Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = method.OverriddenMembers.InaccessibleMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As MethodSymbol = enumerator.Current
						If (Not current.IsOverridable AndAlso Not current.IsMustOverride AndAlso Not current.IsOverrides) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
				End If
				flag = False
			Else
				flag = If(Not TypeOf method.AssociatedSymbol Is EventSymbol, OverrideHidingHelper.RequiresExplicitOverride(DirectCast(method.AssociatedSymbol, PropertySymbol)), False)
			End If
			Return flag
		End Function

		Private Shared Function RequiresExplicitOverride(ByVal prop As PropertySymbol) As Boolean
			Dim flag As Boolean
			If (prop.OverriddenProperty IsNot Nothing) Then
				Dim enumerator As ImmutableArray(Of PropertySymbol).Enumerator = prop.OverriddenMembers.InaccessibleMembers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As PropertySymbol = enumerator.Current
					If (Not current.IsOverridable AndAlso Not current.IsMustOverride AndAlso Not current.IsOverrides) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function RequiresExplicitOverride(ByVal [event] As EventSymbol) As Boolean
			Dim flag As Boolean
			If ([event].OverriddenEvent IsNot Nothing) Then
				Dim enumerator As ImmutableArray(Of EventSymbol).Enumerator = [event].OverriddenOrHiddenMembers.InaccessibleMembers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As EventSymbol = enumerator.Current
					If (Not current.IsOverridable AndAlso Not current.IsMustOverride AndAlso Not current.IsOverrides) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
			End If
			flag = False
			Return flag
		End Function

		Public Shared Function SignaturesMatch(ByVal sym1 As Symbol, ByVal sym2 As Symbol, <Out> ByRef exactMatch As Boolean, <Out> ByRef exactMatchIgnoringCustomModifiers As Boolean) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.OverrideHidingHelper::SignaturesMatch(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,System.Boolean&,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean SignaturesMatch(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,System.Boolean&,System.Boolean&)
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

		End Function
	End Class
End Namespace