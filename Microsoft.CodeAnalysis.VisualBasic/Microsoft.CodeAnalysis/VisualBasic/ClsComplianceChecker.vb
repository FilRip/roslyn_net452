Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ClsComplianceChecker
		Inherits VisualBasicSymbolVisitor
		Private ReadOnly _compilation As VisualBasicCompilation

		Private ReadOnly _filterTree As SyntaxTree

		Private ReadOnly _filterSpanWithinTree As Nullable(Of TextSpan)

		Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Private ReadOnly _cancellationToken As CancellationToken

		Private ReadOnly _declaredOrInheritedCompliance As ConcurrentDictionary(Of Symbol, ClsComplianceChecker.Compliance)

		Private ReadOnly _compilerTasks As ConcurrentStack(Of Task)

		Private ReadOnly Property ConcurrentAnalysis As Boolean
			Get
				If (Me._filterTree IsNot Nothing) Then
					Return False
				End If
				Return Me._compilation.Options.ConcurrentBuild
			End Get
		End Property

		Private Sub New(ByVal compilation As VisualBasicCompilation, ByVal filterTree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.New()
			Me._compilation = compilation
			Me._filterTree = filterTree
			Me._filterSpanWithinTree = filterSpanWithinTree
			Me._diagnostics = diagnostics
			Me._cancellationToken = cancellationToken
			Me._declaredOrInheritedCompliance = New ConcurrentDictionary(Of Symbol, ClsComplianceChecker.Compliance)()
			If (Me.ConcurrentAnalysis) Then
				Me._compilerTasks = New ConcurrentStack(Of Task)()
			End If
		End Sub

		Private Sub AddDiagnostic(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal code As ERRID, ByVal ParamArray args As Object())
			Me.AddDiagnostic(symbol, code, If(symbol.Locations.IsEmpty, NoLocation.Singleton, symbol.Locations(0)), args)
		End Sub

		Private Sub AddDiagnostic(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal code As ERRID, ByVal location As Microsoft.CodeAnalysis.Location, ByVal ParamArray args As Object())
			Dim badSymbolDiagnostic As Microsoft.CodeAnalysis.VisualBasic.BadSymbolDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.BadSymbolDiagnostic(symbol, code, args)
			Dim vBDiagnostic As Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(badSymbolDiagnostic, location, False)
			Me._diagnostics.Add(vBDiagnostic)
		End Sub

		Private Sub CheckBaseTypeCompliance(ByVal symbol As NamedTypeSymbol)
			If (symbol.IsInterface) Then
				Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = symbol.InterfacesNoUseSiteDiagnostics.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As NamedTypeSymbol = enumerator.Current
					If (Not Me.ShouldReportNonCompliantType(current, symbol, Nothing)) Then
						Continue While
					End If
					Me.AddDiagnostic(symbol, ERRID.WRN_InheritedInterfaceNotCLSCompliant2, New [Object]() { symbol, current })
				End While
				Return
			End If
			If (symbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = symbol.BaseTypeNoUseSiteDiagnostics
				If (baseTypeNoUseSiteDiagnostics IsNot Nothing AndAlso Me.ShouldReportNonCompliantType(baseTypeNoUseSiteDiagnostics, symbol, Nothing)) Then
					Me.AddDiagnostic(symbol, ERRID.WRN_BaseClassNotCLSCompliant2, New [Object]() { symbol, baseTypeNoUseSiteDiagnostics })
				End If
			Else
				Dim enumUnderlyingType As NamedTypeSymbol = symbol.EnumUnderlyingType
				If (Me.ShouldReportNonCompliantType(enumUnderlyingType, symbol, Nothing)) Then
					Me.AddDiagnostic(symbol, ERRID.WRN_EnumUnderlyingTypeNotCLS1, New [Object]() { enumUnderlyingType })
					Return
				End If
			End If
		End Sub

		Public Shared Sub CheckCompliance(ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal filterTree As SyntaxTree = Nothing, Optional ByVal filterSpanWithinTree As Nullable(Of TextSpan) = Nothing)
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(diagnostics.DiagnosticBag, New ConcurrentSet(Of AssemblySymbol)())
			Dim clsComplianceChecker As Microsoft.CodeAnalysis.VisualBasic.ClsComplianceChecker = New Microsoft.CodeAnalysis.VisualBasic.ClsComplianceChecker(compilation, filterTree, filterSpanWithinTree, bindingDiagnosticBag, cancellationToken)
			clsComplianceChecker.Visit(compilation.Assembly)
			clsComplianceChecker.WaitForWorkers()
			diagnostics.AddDependencies(bindingDiagnosticBag.DependenciesBag)
		End Sub

		Private Sub CheckEventTypeCompliance(ByVal symbol As EventSymbol)
			Dim associatedSymbol As EventSymbol
			Dim type As TypeSymbol = symbol.Type
			If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate] AndAlso type.IsImplicitlyDeclared) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol IsNot Nothing) Then
					associatedSymbol = namedTypeSymbol.AssociatedSymbol
				Else
					associatedSymbol = Nothing
				End If
				If (CObj(associatedSymbol) = CObj(symbol)) Then
					Me.CheckParameterCompliance(symbol.DelegateParameters, symbol.ContainingType)
					Return
				End If
			End If
			If (Me.ShouldReportNonCompliantType(type, symbol.ContainingType, symbol)) Then
				Me.AddDiagnostic(symbol, ERRID.WRN_EventDelegateTypeNotCLSCompliant2, New [Object]() { type, symbol.Name })
			End If
		End Sub

		Private Sub CheckForCompliantWithinNonCompliant(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim containingType As NamedTypeSymbol = symbol.ContainingType
			If (containingType IsNot Nothing AndAlso Not ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(containingType))) Then
				Me.AddDiagnostic(symbol, ERRID.WRN_CLSMemberInNonCLSType3, New [Object]() { symbol.GetKindText(), symbol, containingType })
			End If
		End Sub

		Private Sub CheckForNonCompliantAbstractMember(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim containingType As NamedTypeSymbol = symbol.ContainingType
			If (containingType IsNot Nothing AndAlso containingType.IsInterface) Then
				Me.AddDiagnostic(symbol, ERRID.WRN_NonCLSMemberInCLSInterface1, New [Object]() { symbol })
				Return
			End If
			If (symbol.IsMustOverride AndAlso symbol.Kind <> SymbolKind.NamedType) Then
				Me.AddDiagnostic(symbol, ERRID.WRN_NonCLSMustOverrideInCLSType1, New [Object]() { containingType })
			End If
		End Sub

		Private Sub CheckMemberDistinctness(ByVal symbol As NamespaceOrTypeSymbol)
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).KeyCollection.Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).KeyCollection.Enumerator()
			Dim strs As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol) = New MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol)(CaseInsensitiveComparison.Comparer)
			If (symbol.Kind <> SymbolKind.[Namespace]) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Try
					enumerator = namedTypeSymbol.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator.Current
						If (Not ClsComplianceChecker.IsAccessibleOutsideAssembly(current)) Then
							Continue While
						End If
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = current.GetMembersUnordered().GetEnumerator()
						While enumerator1.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
							If (Not ClsComplianceChecker.IsAccessibleIfContainerIsAccessible(current1) OrElse current1.IsOverrides AndAlso (current1.Kind = SymbolKind.Method OrElse current1.Kind = SymbolKind.[Property])) Then
								Continue While
							End If
							strs.Add(current1.Name, current1)
						End While
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics
				While baseTypeNoUseSiteDiagnostics IsNot Nothing
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = baseTypeNoUseSiteDiagnostics.GetMembersUnordered().GetEnumerator()
					While enumerator2.MoveNext()
						Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
						If (Not ClsComplianceChecker.IsAccessibleOutsideAssembly(symbol1) OrElse Not ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(symbol1)) OrElse symbol1.IsOverrides AndAlso (symbol1.Kind = SymbolKind.Method OrElse symbol1.Kind = SymbolKind.[Property])) Then
							Continue While
						End If
						strs.Add(symbol1.Name, symbol1)
					End While
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
				End While
			End If
			Dim enumerator3 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
			While enumerator3.MoveNext()
				Dim current2 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator3.Current
				If (Me.DoNotVisit(current2) OrElse Not ClsComplianceChecker.IsAccessibleIfContainerIsAccessible(current2) OrElse Not ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(current2)) OrElse current2.IsOverrides) Then
					Continue While
				End If
				Dim name As String = current2.Name
				Dim item As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet = strs(name)
				If (item.Count > 0) Then
					Me.CheckSymbolDistinctness(current2, item)
				End If
				strs.Add(name, current2)
			End While
		End Sub

		Private Sub CheckName(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (symbol.CanBeReferencedByName) Then
				Dim name As String = symbol.Name
				If (name.Length > 0 AndAlso name(0) = Strings.ChrW(95)) Then
					If (symbol.Kind = SymbolKind.[Namespace]) Then
						Dim rootNamespace As NamespaceSymbol = Me._compilation.RootNamespace
						If (symbol = rootNamespace AndAlso symbol.ContainingNamespace.IsGlobalNamespace) Then
							Me.AddDiagnostic(symbol, ERRID.WRN_RootNamespaceNotCLSCompliant1, New [Object]() { rootNamespace })
							Return
						End If
						Dim containingNamespace As NamespaceSymbol = rootNamespace
						While containingNamespace IsNot Nothing
							If (symbol = containingNamespace) Then
								Me.AddDiagnostic(symbol, ERRID.WRN_RootNamespaceNotCLSCompliant2, New [Object]() { symbol.Name, rootNamespace })
								Return
							End If
							containingNamespace = containingNamespace.ContainingNamespace
						End While
					End If
					Me.AddDiagnostic(symbol, ERRID.WRN_NameNotCLSCompliant1, New [Object]() { name })
				End If
			End If
		End Sub

		Private Sub CheckParameterCompliance(ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal context As NamedTypeSymbol)
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				If (Not Me.ShouldReportNonCompliantType(current.Type, context, current)) Then
					If (Not current.HasExplicitDefaultValue) Then
						Continue While
					End If
					Dim discriminator As ConstantValueTypeDiscriminator = current.ExplicitDefaultConstantValue.Discriminator
					If (discriminator <> ConstantValueTypeDiscriminator.[SByte]) Then
						Select Case discriminator
							Case ConstantValueTypeDiscriminator.UInt16
							Case ConstantValueTypeDiscriminator.UInt32
							Case ConstantValueTypeDiscriminator.UInt64
								Exit Select
							Case Else
								Continue While
						End Select
					End If
					Me.AddDiagnostic(current, ERRID.WRN_OptionalValueNotCLSCompliant1, New [Object]() { current.Name })
				Else
					Me.AddDiagnostic(current, ERRID.WRN_ParamNotCLSCompliant1, New [Object]() { current.Name })
				End If
			End While
		End Sub

		Private Sub CheckReturnTypeCompliance(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim type As TypeSymbol
			Dim kind As SymbolKind = symbol.Kind
			If (kind = SymbolKind.Field) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_FieldNotCLSCompliant1
				type = DirectCast(symbol, FieldSymbol).Type
			ElseIf (kind = SymbolKind.Method) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ProcTypeNotCLSCompliant1
				type = DirectCast(symbol, MethodSymbol).ReturnType
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				End If
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ProcTypeNotCLSCompliant1
				type = DirectCast(symbol, PropertySymbol).Type
			End If
			If (Me.ShouldReportNonCompliantType(type, symbol.ContainingType, symbol)) Then
				Me.AddDiagnostic(symbol, eRRID, New [Object]() { symbol.Name })
			End If
		End Sub

		Private Sub CheckSymbolDistinctness(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal sameNameSymbols As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet)
			Dim enumerator As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet.Enumerator = New MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet.Enumerator()
			If (If(symbol.Kind = SymbolKind.Method, True, symbol.Kind = SymbolKind.[Property])) Then
				Try
					enumerator = sameNameSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
						If (symbol.Kind <> current.Kind OrElse symbol.IsAccessor() OrElse current.IsAccessor() OrElse Not ClsComplianceChecker.SignaturesCollide(symbol, current)) Then
							Continue While
						End If
						Me.AddDiagnostic(symbol, ERRID.WRN_ArrayOverloadsNonCLS2, New [Object]() { symbol, current })
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
		End Sub

		Private Sub CheckTypeParameterCompliance(ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol), ByVal context As NamedTypeSymbol)
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = typeParameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterSymbol = enumerator.Current
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator()
				While enumerator1.MoveNext()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator1.Current
					If (Not Me.ShouldReportNonCompliantType(typeSymbol, context, current)) Then
						Continue While
					End If
					Me.AddDiagnostic(current, ERRID.WRN_GenericConstraintNotCLSCompliant1, New [Object]() { typeSymbol })
				End While
			End While
		End Sub

		Private Function DoNotVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean
			If (symbol.Kind <> SymbolKind.[Namespace]) Then
				flag = If(symbol.DeclaringCompilation <> Me._compilation OrElse symbol.IsImplicitlyDeclared, True, Me.IsSyntacticallyFilteredOut(symbol))
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function GetContainingModuleOrAssembly(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim containingModule As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim containingAssembly As AssemblySymbol = symbol.ContainingAssembly
			If (CObj(containingAssembly) = CObj(Me._compilation.Assembly)) Then
				If (Me._compilation.Options.OutputKind = OutputKind.NetModule) Then
					containingModule = symbol.ContainingModule
				Else
					containingModule = containingAssembly
				End If
				symbol1 = containingModule
			Else
				symbol1 = containingAssembly
			End If
			Return symbol1
		End Function

		Private Function GetDeclaredCompliance(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, <Out> ByRef attributeLocation As Location) As Nullable(Of Boolean)
			Dim declaredComplianceHelper As Nullable(Of Boolean)
			Dim nullable As Nullable(Of Boolean)
			Dim nullable1 As Nullable(Of Boolean)
			If (symbol.IsFromCompilation(Me._compilation) OrElse symbol.Kind <> SymbolKind.NamedType) Then
				Dim flag As Boolean = False
				declaredComplianceHelper = Me.GetDeclaredComplianceHelper(symbol, attributeLocation, flag)
			Else
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = DirectCast(symbol, NamedTypeSymbol)
				While baseTypeNoUseSiteDiagnostics IsNot Nothing
					Dim flag1 As Boolean = False
					Dim declaredComplianceHelper1 As Nullable(Of Boolean) = Me.GetDeclaredComplianceHelper(baseTypeNoUseSiteDiagnostics, attributeLocation, flag1)
					If (Not declaredComplianceHelper1.HasValue) Then
						baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
					Else
						If (baseTypeNoUseSiteDiagnostics <> symbol) Then
							If (flag1) Then
								nullable = If(declaredComplianceHelper1.HasValue, New Nullable(Of Boolean)(Not declaredComplianceHelper1.GetValueOrDefault()), declaredComplianceHelper1)
								If (nullable.GetValueOrDefault()) Then
									nullable1 = declaredComplianceHelper1
									declaredComplianceHelper = nullable1
									Return declaredComplianceHelper
								End If
							End If
							nullable = Nothing
							nullable1 = nullable
							declaredComplianceHelper = nullable1
							Return declaredComplianceHelper
						End If
						nullable1 = declaredComplianceHelper1
						declaredComplianceHelper = nullable1
						Return declaredComplianceHelper
					End If
				End While
				declaredComplianceHelper = Nothing
			End If
			Return declaredComplianceHelper
		End Function

		Private Function GetDeclaredComplianceHelper(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, <Out> ByRef attributeLocation As Location, <Out> ByRef isAttributeInherited As Boolean) As Nullable(Of Boolean)
			Dim nullable As Nullable(Of Boolean)
			attributeLocation = Nothing
			isAttributeInherited = False
			Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = symbol.GetAttributes().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute)) Then
						Dim attributeClass As NamedTypeSymbol = current.AttributeClass
						If (attributeClass IsNot Nothing) Then
							Me._diagnostics.ReportUseSite(attributeClass, If(symbol.Locations.IsEmpty, NoLocation.Singleton, symbol.Locations(0)))
						End If
						If (Not current.HasErrors) Then
							If (Not Me.TryGetAttributeWarningLocation(current, attributeLocation)) Then
								attributeLocation = Nothing
							End If
							isAttributeInherited = current.AttributeClass.GetAttributeUsageInfo().Inherited
							Dim item As TypedConstant = current.CommonConstructorArguments(0)
							nullable = New Nullable(Of Boolean)(CBool(item.ValueInternal))
							Exit While
						End If
					End If
				Else
					nullable = Nothing
					Exit While
				End If
			End While
			Return nullable
		End Function

		Private Function GetDeclaredOrInheritedCompliance(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As ClsComplianceChecker.Compliance
			Dim declaredOrInheritedCompliance As ClsComplianceChecker.Compliance
			Dim compliance As ClsComplianceChecker.Compliance
			If (symbol.Kind <> SymbolKind.[Namespace]) Then
				If (symbol.Kind = SymbolKind.Method) Then
					Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = DirectCast(symbol, MethodSymbol).AssociatedSymbol
					If (associatedSymbol Is Nothing) Then
						GoTo Label1
					End If
					declaredOrInheritedCompliance = Me.GetDeclaredOrInheritedCompliance(associatedSymbol)
					Return declaredOrInheritedCompliance
				End If
			Label1:
				If (Not Me._declaredOrInheritedCompliance.TryGetValue(symbol, compliance)) Then
					Dim location As Microsoft.CodeAnalysis.Location = Nothing
					Dim declaredCompliance As Nullable(Of Boolean) = Me.GetDeclaredCompliance(symbol, location)
					If (declaredCompliance.HasValue) Then
						compliance = If(declaredCompliance.GetValueOrDefault(), ClsComplianceChecker.Compliance.DeclaredTrue, ClsComplianceChecker.Compliance.DeclaredFalse)
					ElseIf (symbol.Kind = SymbolKind.Assembly OrElse symbol.Kind = SymbolKind.NetModule) Then
						compliance = ClsComplianceChecker.Compliance.ImpliedFalse
					Else
						compliance = If(ClsComplianceChecker.IsTrue(Me.GetInheritedCompliance(symbol)), ClsComplianceChecker.Compliance.InheritedTrue, ClsComplianceChecker.Compliance.InheritedFalse)
					End If
					Dim kind As SymbolKind = symbol.Kind
					declaredOrInheritedCompliance = If(kind = SymbolKind.Assembly OrElse CInt(kind) - CInt(SymbolKind.NetModule) <= CInt(SymbolKind.ArrayType), Me._declaredOrInheritedCompliance.GetOrAdd(symbol, compliance), compliance)
				Else
					declaredOrInheritedCompliance = compliance
				End If
			Else
				declaredOrInheritedCompliance = Me.GetDeclaredOrInheritedCompliance(Me.GetContainingModuleOrAssembly(symbol))
			End If
			Return declaredOrInheritedCompliance
		End Function

		Private Function GetInheritedCompliance(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As ClsComplianceChecker.Compliance
			Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbol.ContainingType
			If (containingType Is Nothing) Then
				containingType = Me.GetContainingModuleOrAssembly(symbol)
			End If
			Return Me.GetDeclaredOrInheritedCompliance(containingType)
		End Function

		Private Shared Function GetParameterRefKinds(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As ImmutableArray(Of RefKind)
			Dim immutableAndFree As ImmutableArray(Of RefKind)
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			Dim kind As SymbolKind = symbol.Kind
			If (kind = SymbolKind.Method) Then
				parameters = DirectCast(symbol, MethodSymbol).Parameters
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				End If
				parameters = DirectCast(symbol, PropertySymbol).Parameters
			End If
			If (Not parameters.IsEmpty) Then
				Dim instance As ArrayBuilder(Of RefKind) = ArrayBuilder(Of RefKind).GetInstance(parameters.Length)
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					instance.Add(If(current.IsByRef, RefKind.Ref, RefKind.None))
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of RefKind).Empty
			End If
			Return immutableAndFree
		End Function

		Private Shared Function GetParameterTypes(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As ImmutableArray(Of TypeSymbol)
			Dim immutableAndFree As ImmutableArray(Of TypeSymbol)
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			Dim kind As SymbolKind = symbol.Kind
			If (kind = SymbolKind.Method) Then
				parameters = DirectCast(symbol, MethodSymbol).Parameters
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				End If
				parameters = DirectCast(symbol, PropertySymbol).Parameters
			End If
			If (Not parameters.IsEmpty) Then
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(parameters.Length)
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current.Type)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of TypeSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Private Function HasAcceptableAttributeConstructor(ByVal attributeType As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = attributeType.InstanceConstructors.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As MethodSymbol = enumerator.Current
					If (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(current)) AndAlso ClsComplianceChecker.IsAccessibleIfContainerIsAccessible(current)) Then
						Dim flag1 As Boolean = False
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = ClsComplianceChecker.GetParameterTypes(current).GetEnumerator()
						While enumerator1.MoveNext()
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator1.Current
							If (typeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Array AndAlso TypedConstant.GetTypedConstantKind(typeSymbol, Me._compilation) <> TypedConstantKind.[Error]) Then
								Continue While
							End If
							flag1 = True
							Exit While
						End While
						If (Not flag1) Then
							flag = True
							Exit While
						End If
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function IsAccessibleIfContainerIsAccessible(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean
			Select Case symbol.DeclaredAccessibility
				Case Accessibility.NotApplicable
					flag = False
					Exit Select
				Case Accessibility.[Private]
				Case Accessibility.ProtectedAndInternal
				Case Accessibility.Internal
					flag = False
					Exit Select
				Case Accessibility.[Protected]
				Case Accessibility.ProtectedOrInternal
					Dim containingType As NamedTypeSymbol = symbol.ContainingType
					flag = If(containingType Is Nothing, True, Not containingType.IsNotInheritable)
					Exit Select
				Case Accessibility.[Public]
					flag = True
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility)
			End Select
			Return flag
		End Function

		Private Shared Function IsAccessibleOutsideAssembly(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean
			While True
				If (symbol Is Nothing OrElse ClsComplianceChecker.IsImplicitClass(symbol)) Then
					flag = True
					Exit While
				ElseIf (ClsComplianceChecker.IsAccessibleIfContainerIsAccessible(symbol)) Then
					symbol = symbol.ContainingType
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function IsArrayOfArrays(ByVal arrayType As ArrayTypeSymbol) As Boolean
			Return arrayType.ElementType.Kind = SymbolKind.ArrayType
		End Function

		Private Function IsCompliantType(ByVal type As TypeSymbol, ByVal context As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Select Case type.TypeKind
				Case Microsoft.CodeAnalysis.TypeKind.Array
					flag = Me.IsCompliantType(DirectCast(type, ArrayTypeSymbol).ElementType, context)
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
				Case Microsoft.CodeAnalysis.TypeKind.Struct
				Case Microsoft.CodeAnalysis.TypeKind.Submission
					flag = Me.IsCompliantType(DirectCast(type, NamedTypeSymbol))
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Throw ExceptionUtilities.UnexpectedValue(type.TypeKind)
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
				Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
					flag = True
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(type.TypeKind)
			End Select
			Return flag
		End Function

		Private Function IsCompliantType(ByVal type As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = type.SpecialType
			If (specialType > Microsoft.CodeAnalysis.SpecialType.System_UInt64) Then
				If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_UIntPtr AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_TypedReference) Then
					If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
						flag = True
					ElseIf (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(type.OriginalDefinition))) Then
						flag = If(Not type.IsTupleType, True, Me.IsCompliantType(type.TupleUnderlyingType))
					Else
						flag = False
					End If
					Return flag
				End If
				flag = False
				Return flag
			Else
				If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_SByte) Then
					Select Case specialType
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							Exit Select
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
							If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
								flag = True
							ElseIf (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(type.OriginalDefinition))) Then
								flag = If(Not type.IsTupleType, True, Me.IsCompliantType(type.TupleUnderlyingType))
							Else
								flag = False
							End If
							Return flag
						Case Else
							If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
								flag = True
							ElseIf (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(type.OriginalDefinition))) Then
								flag = If(Not type.IsTupleType, True, Me.IsCompliantType(type.TupleUnderlyingType))
							Else
								flag = False
							End If
							Return flag
					End Select
				End If
				flag = False
				Return flag
			End If
			If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
				flag = True
			ElseIf (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(type.OriginalDefinition))) Then
				flag = If(Not type.IsTupleType, True, Me.IsCompliantType(type.TupleUnderlyingType))
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function IsDeclared(ByVal compliance As ClsComplianceChecker.Compliance) As Boolean
			Dim flag As Boolean
			Dim compliance1 As ClsComplianceChecker.Compliance = compliance
			If (compliance1 <= ClsComplianceChecker.Compliance.DeclaredFalse) Then
				flag = True
			Else
				If (CInt(compliance1) - CInt(ClsComplianceChecker.Compliance.InheritedTrue) > CInt(ClsComplianceChecker.Compliance.InheritedTrue)) Then
					Throw ExceptionUtilities.UnexpectedValue(compliance)
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function IsImplicitClass(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (symbol.Kind <> SymbolKind.NamedType) Then
				Return False
			End If
			Return DirectCast(symbol, NamedTypeSymbol).IsImplicitClass
		End Function

		Private Function IsSyntacticallyFilteredOut(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (Me._filterTree Is Nothing) Then
				Return False
			End If
			Return Not symbol.IsDefinedInSourceTree(Me._filterTree, Me._filterSpanWithinTree, Me._cancellationToken)
		End Function

		Private Shared Function IsTrue(ByVal compliance As ClsComplianceChecker.Compliance) As Boolean
			Dim flag As Boolean
			Select Case compliance
				Case ClsComplianceChecker.Compliance.DeclaredTrue
				Case ClsComplianceChecker.Compliance.InheritedTrue
					flag = True
					Exit Select
				Case ClsComplianceChecker.Compliance.DeclaredFalse
				Case ClsComplianceChecker.Compliance.InheritedFalse
				Case ClsComplianceChecker.Compliance.ImpliedFalse
					flag = False
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(compliance)
			End Select
			Return flag
		End Function

		Private Sub ReportNonCompliantTypeArguments(ByVal type As TypeSymbol, ByVal context As NamedTypeSymbol, ByVal diagnosticSymbol As Symbol)
			Select Case type.TypeKind
				Case Microsoft.CodeAnalysis.TypeKind.Array
					Me.ReportNonCompliantTypeArguments(DirectCast(type, ArrayTypeSymbol).ElementType, context, diagnosticSymbol)
					Return
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
				Case Microsoft.CodeAnalysis.TypeKind.Struct
				Case Microsoft.CodeAnalysis.TypeKind.Submission
					Me.ReportNonCompliantTypeArguments(DirectCast(type, NamedTypeSymbol), context, diagnosticSymbol)
					Return
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Throw ExceptionUtilities.UnexpectedValue(type.TypeKind)
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
				Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
					Return
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(type.TypeKind)
			End Select
		End Sub

		Private Sub ReportNonCompliantTypeArguments(ByVal type As NamedTypeSymbol, ByVal context As NamedTypeSymbol, ByVal diagnosticSymbol As Symbol)
			If (type.IsTupleType) Then
				type = type.TupleUnderlyingType
			End If
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = type.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeSymbol = enumerator.Current
				If (Not Me.IsCompliantType(current, context)) Then
					Me.AddDiagnostic(diagnosticSymbol, ERRID.WRN_TypeNotCLSCompliant1, New [Object]() { current })
				End If
				Me.ReportNonCompliantTypeArguments(current, context, diagnosticSymbol)
			End While
		End Sub

		Private Function ShouldReportNonCompliantType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal context As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Optional ByVal diagnosticSymbol As Symbol = Nothing) As Boolean
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = context
			Dim obj As [Object] = diagnosticSymbol
			If (obj Is Nothing) Then
				obj = context
			End If
			Me.ReportNonCompliantTypeArguments(typeSymbol, namedTypeSymbol, DirectCast(obj, Symbol))
			Return Not Me.IsCompliantType(type, context)
		End Function

		Private Shared Function SignaturesCollide(ByVal x As Symbol, ByVal y As Symbol) As Boolean
			Dim flag As Boolean
			Dim parameterTypes As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = ClsComplianceChecker.GetParameterTypes(x)
			Dim typeSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = ClsComplianceChecker.GetParameterTypes(y)
			ClsComplianceChecker.GetParameterRefKinds(x)
			ClsComplianceChecker.GetParameterRefKinds(y)
			Dim length As Integer = parameterTypes.Length
			If (typeSymbols.Length = length) Then
				Dim flag1 As Boolean = False
				Dim flag2 As Boolean = False
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				While num1 <= num
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = parameterTypes(num1)
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbols(num1)
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = item.TypeKind
					If (typeSymbol.TypeKind = typeKind) Then
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.Array) Then
							Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							flag1 = If(flag1, True, arrayTypeSymbol.Rank <> arrayTypeSymbol1.Rank)
							Dim flag3 As Boolean = Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(arrayTypeSymbol.ElementType, arrayTypeSymbol1.ElementType, TypeCompareKind.ConsiderEverything)
							If (ClsComplianceChecker.IsArrayOfArrays(arrayTypeSymbol) AndAlso ClsComplianceChecker.IsArrayOfArrays(arrayTypeSymbol1)) Then
								flag2 = If(flag2, True, flag3)
							ElseIf (flag3) Then
								flag = False
								Return flag
							End If
						ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(item, typeSymbol, TypeCompareKind.ConsiderEverything)) Then
							flag = False
							Return flag
						End If
						num1 = num1 + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = If(flag2, True, flag1)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function TryGetAttributeWarningLocation(ByVal attribute As VisualBasicAttributeData, ByRef location As Microsoft.CodeAnalysis.Location) As Boolean
			Dim flag As Boolean
			Dim applicationSyntaxReference As SyntaxReference = attribute.ApplicationSyntaxReference
			If (applicationSyntaxReference Is Nothing AndAlso Me._filterTree Is Nothing) Then
				location = NoLocation.Singleton
				flag = True
			ElseIf (Me._filterTree Is Nothing OrElse applicationSyntaxReference IsNot Nothing AndAlso applicationSyntaxReference.SyntaxTree = Me._filterTree) Then
				location = New SourceLocation(applicationSyntaxReference)
				flag = True
			Else
				location = Nothing
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Sub VisitAssembly(ByVal symbol As AssemblySymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (symbol.Modules.Length > 1 AndAlso Me.ConcurrentAnalysis) Then
				Me.VisitAssemblyMembersAsTasks(symbol)
				Return
			End If
			Me.VisitAssemblyMembers(symbol)
		End Sub

		Private Sub VisitAssemblyMembers(ByVal symbol As AssemblySymbol)
			Dim enumerator As ImmutableArray(Of ModuleSymbol).Enumerator = symbol.Modules.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitModule(enumerator.Current)
			End While
		End Sub

		Private Sub VisitAssemblyMembersAsTasks(ByVal symbol As AssemblySymbol)
			Dim variable As ClsComplianceChecker._Closure$__13-0 = Nothing
			Dim enumerator As ImmutableArray(Of ModuleSymbol).Enumerator = symbol.Modules.GetEnumerator()
			While enumerator.MoveNext()
				variable = New ClsComplianceChecker._Closure$__13-0(variable) With
				{
					.$VB$Me = Me,
					.$VB$Local_m = enumerator.Current
				}
				Me._compilerTasks.Push(Task.Run(UICultureUtilities.WithCurrentUICulture(Sub()
				Try
					Me.$VB$Me.VisitModule(Me.$VB$Local_m)
				Catch exception As System.Exception When FatalError.ReportAndPropagateUnlessCanceled(exception)
					Throw ExceptionUtilities.Unreachable
				End Try
				End Sub), Me._cancellationToken))
			End While
		End Sub

		Public Overrides Sub VisitEvent(ByVal symbol As EventSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				Me.VisitTypeOrMember(symbol, Me.GetDeclaredOrInheritedCompliance(symbol))
			End If
		End Sub

		Public Overrides Sub VisitField(ByVal symbol As FieldSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				Me.VisitTypeOrMember(symbol, Me.GetDeclaredOrInheritedCompliance(symbol))
			End If
		End Sub

		Public Overrides Sub VisitMethod(ByVal symbol As MethodSymbol)
			Dim nullable As Nullable(Of Boolean)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				Dim declaredOrInheritedCompliance As ClsComplianceChecker.Compliance = Me.GetDeclaredOrInheritedCompliance(symbol)
				Dim flag As Boolean = Me.VisitTypeOrMember(symbol, declaredOrInheritedCompliance)
				Dim flag1 As Boolean = symbol.IsAccessor()
				If (flag OrElse flag1) Then
					If (flag1) Then
						Dim methodKind As Microsoft.CodeAnalysis.MethodKind = symbol.MethodKind
						If (methodKind <> Microsoft.CodeAnalysis.MethodKind.EventAdd AndAlso methodKind <> Microsoft.CodeAnalysis.MethodKind.EventRemove) Then
							If (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.PropertyGet) <= CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
								Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = symbol.GetAttributes().GetEnumerator()
								While enumerator.MoveNext()
									Dim current As VisualBasicAttributeData = enumerator.Current
									If (Not current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute)) Then
										Continue While
									End If
									Dim location As Microsoft.CodeAnalysis.Location = Nothing
									If (Not Me.TryGetAttributeWarningLocation(current, location)) Then
										Continue While
									End If
									Dim attributeUsageInfo As Microsoft.CodeAnalysis.AttributeUsageInfo = current.AttributeClass.GetAttributeUsageInfo()
									Me.AddDiagnostic(symbol, ERRID.WRN_CLSAttrInvalidOnGetSet, location, New [Object]() { current.AttributeClass.Name, attributeUsageInfo.GetValidTargetsErrorArgument() })
									Return
								End While
								Return
							End If
						ElseIf (flag) Then
							Dim containingType As NamedTypeSymbol = symbol.ContainingType
							If (Not ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(containingType))) Then
								Dim location1 As Microsoft.CodeAnalysis.Location = Nothing
								Dim declaredCompliance As Nullable(Of Boolean) = Me.GetDeclaredCompliance(symbol, location1)
								If (declaredCompliance.HasValue) Then
									nullable = New Nullable(Of Boolean)(declaredCompliance.GetValueOrDefault())
								Else
									nullable = Nothing
								End If
								declaredCompliance = nullable
								If (declaredCompliance.GetValueOrDefault()) Then
									Me.AddDiagnostic(symbol, ERRID.WRN_CLSEventMethodInNonCLSType3, location1, New [Object]() { methodKind.TryGetAccessorDisplayName(), symbol.AssociatedSymbol.Name, containingType })
								End If
							End If
						End If
					ElseIf (ClsComplianceChecker.IsTrue(declaredOrInheritedCompliance)) Then
						Me.CheckParameterCompliance(symbol.Parameters, symbol.ContainingType)
						Me.CheckTypeParameterCompliance(symbol.TypeParameters, symbol.ContainingType)
						Return
					End If
				End If
			End If
		End Sub

		Public Overrides Sub VisitModule(ByVal symbol As ModuleSymbol)
			Me.Visit(symbol.GlobalNamespace)
		End Sub

		Public Overrides Sub VisitNamedType(ByVal symbol As NamedTypeSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				Dim declaredOrInheritedCompliance As ClsComplianceChecker.Compliance = Me.GetDeclaredOrInheritedCompliance(symbol)
				If (Me.VisitTypeOrMember(symbol, declaredOrInheritedCompliance) AndAlso ClsComplianceChecker.IsTrue(declaredOrInheritedCompliance)) Then
					Me.CheckBaseTypeCompliance(symbol)
					Me.CheckTypeParameterCompliance(symbol.TypeParameters, symbol)
					Me.CheckMemberDistinctness(symbol)
					If (symbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						Me.CheckParameterCompliance(symbol.DelegateInvokeMethod.Parameters, symbol)
					End If
				End If
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembersUnordered().GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(enumerator.Current)
				End While
			End If
		End Sub

		Public Overrides Sub VisitNamespace(ByVal symbol As NamespaceSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				If (ClsComplianceChecker.IsTrue(Me.GetDeclaredOrInheritedCompliance(symbol))) Then
					Me.CheckName(symbol)
					Me.CheckMemberDistinctness(symbol)
				End If
				If (Me.ConcurrentAnalysis) Then
					Me.VisitNamespaceMembersAsTasks(symbol)
					Return
				End If
				Me.VisitNamespaceMembers(symbol)
			End If
		End Sub

		Private Sub VisitNamespaceMembers(ByVal symbol As NamespaceSymbol)
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembersUnordered().GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
		End Sub

		Private Sub VisitNamespaceMembersAsTasks(ByVal symbol As NamespaceSymbol)
			Dim variable As ClsComplianceChecker._Closure$__17-0 = Nothing
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembersUnordered().GetEnumerator()
			While enumerator.MoveNext()
				variable = New ClsComplianceChecker._Closure$__17-0(variable) With
				{
					.$VB$Me = Me,
					.$VB$Local_m = enumerator.Current
				}
				Me._compilerTasks.Push(Task.Run(UICultureUtilities.WithCurrentUICulture(Sub()
				Try
					Me.$VB$Me.Visit(Me.$VB$Local_m)
				Catch exception As System.Exception When FatalError.ReportAndPropagateUnlessCanceled(exception)
					Throw ExceptionUtilities.Unreachable
				End Try
				End Sub), Me._cancellationToken))
			End While
		End Sub

		Public Overrides Sub VisitProperty(ByVal symbol As PropertySymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me.DoNotVisit(symbol)) Then
				Dim declaredOrInheritedCompliance As ClsComplianceChecker.Compliance = Me.GetDeclaredOrInheritedCompliance(symbol)
				If (Me.VisitTypeOrMember(symbol, declaredOrInheritedCompliance) AndAlso ClsComplianceChecker.IsTrue(declaredOrInheritedCompliance)) Then
					Me.CheckParameterCompliance(symbol.Parameters, symbol.ContainingType)
				End If
			End If
		End Sub

		Private Function VisitTypeOrMember(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal compliance As ClsComplianceChecker.Compliance) As Boolean
			Dim flag As Boolean
			If (ClsComplianceChecker.IsAccessibleOutsideAssembly(symbol)) Then
				Dim flag1 As Boolean = symbol.IsAccessor()
				If (ClsComplianceChecker.IsTrue(compliance)) Then
					Me.CheckName(symbol)
					If (Not flag1) Then
						Me.CheckForCompliantWithinNonCompliant(symbol)
					End If
					If (symbol.Kind = SymbolKind.NamedType) Then
						Dim delegateInvokeMethod As MethodSymbol = DirectCast(symbol, NamedTypeSymbol).DelegateInvokeMethod
						If (delegateInvokeMethod IsNot Nothing) Then
							Me.CheckReturnTypeCompliance(delegateInvokeMethod)
						End If
					ElseIf (symbol.Kind = SymbolKind.[Event]) Then
						Me.CheckEventTypeCompliance(DirectCast(symbol, EventSymbol))
					ElseIf (Not flag1) Then
						Me.CheckReturnTypeCompliance(symbol)
					End If
				ElseIf (Not flag1 AndAlso ClsComplianceChecker.IsTrue(Me.GetInheritedCompliance(symbol))) Then
					Me.CheckForNonCompliantAbstractMember(symbol)
				End If
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Sub WaitForWorkers()
			Dim tasks As ConcurrentStack(Of System.Threading.Tasks.Task) = Me._compilerTasks
			If (tasks IsNot Nothing) Then
				Dim task As System.Threading.Tasks.Task = Nothing
				While tasks.TryPop(task)
					task.GetAwaiter().GetResult()
				End While
			End If
		End Sub

		Private Enum Compliance
			DeclaredTrue
			DeclaredFalse
			InheritedTrue
			InheritedFalse
			ImpliedFalse
		End Enum
	End Class
End Namespace