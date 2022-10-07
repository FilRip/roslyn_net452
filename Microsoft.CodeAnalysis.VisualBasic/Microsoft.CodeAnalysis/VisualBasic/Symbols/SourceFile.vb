Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceFile
		Implements IImportScope
		Private ReadOnly _sourceModule As SourceModuleSymbol

		Private ReadOnly _syntaxTree As SyntaxTree

		Private ReadOnly _diagnosticBagDeclare As DiagnosticBag

		Private _lazyBoundInformation As SourceFile.BoundFileInformation

		Private _importsValidated As Integer

		Private _lazyQuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker

		Private _lazyTranslatedImports As ImmutableArray(Of UsedNamespaceOrType)

		Public ReadOnly Property AliasImportsOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition)
			Get
				Return Me.BoundInformation.AliasImportsOpt
			End Get
		End Property

		Private ReadOnly Property BoundInformation As SourceFile.BoundFileInformation
			Get
				Return Me.GetBoundInformation(CancellationToken.None)
			End Get
		End Property

		Public ReadOnly Property DeclarationDiagnostics As DiagnosticBag
			Get
				Return Me._diagnosticBagDeclare
			End Get
		End Property

		Public ReadOnly Property MemberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)
			Get
				Return Me.BoundInformation.MemberImports
			End Get
		End Property

		Public ReadOnly Property OptionCompareText As Nullable(Of Boolean)
			Get
				Return Me.BoundInformation.OptionCompareText
			End Get
		End Property

		Public ReadOnly Property OptionExplicit As Nullable(Of Boolean)
			Get
				Return Me.BoundInformation.OptionExplicit
			End Get
		End Property

		Public ReadOnly Property OptionInfer As Nullable(Of Boolean)
			Get
				Return Me.BoundInformation.OptionInfer
			End Get
		End Property

		Public ReadOnly Property OptionStrict As Nullable(Of Boolean)
			Get
				Return Me.BoundInformation.OptionStrict
			End Get
		End Property

		Public ReadOnly Property Parent As IImportScope Implements IImportScope.Parent
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property QuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Get
				If (Me._lazyQuickAttributeChecker Is Nothing) Then
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker)(Me._lazyQuickAttributeChecker, Me.CreateQuickAttributeChecker(), Nothing)
				End If
				Return Me._lazyQuickAttributeChecker
			End Get
		End Property

		Public ReadOnly Property XmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition)
			Get
				Return Me.BoundInformation.XmlNamespacesOpt
			End Get
		End Property

		Public Sub New(ByVal sourceModule As SourceModuleSymbol, ByVal tree As SyntaxTree)
			MyBase.New()
			Me._diagnosticBagDeclare = New DiagnosticBag()
			Me._sourceModule = sourceModule
			Me._syntaxTree = tree
		End Sub

		Private Function BindFileInformation(ByVal diagBag As DiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal filterSpan As Nullable(Of TextSpan) = Nothing) As SourceFile.BoundFileInformation
			Dim nullable As Nullable(Of Boolean) = New Nullable(Of Boolean)()
			Dim nullable1 As Nullable(Of Boolean) = New Nullable(Of Boolean)()
			Dim nullable2 As Nullable(Of Boolean) = New Nullable(Of Boolean)()
			Dim nullable3 As Nullable(Of Boolean) = New Nullable(Of Boolean)()
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForSourceFileImports(Me._sourceModule, Me._syntaxTree)
			Dim compilationUnitRoot As CompilationUnitSyntax = Me._syntaxTree.GetCompilationUnitRoot()
			SourceFile.BindOptions(compilationUnitRoot.Options, diagBag, nullable, nullable1, nullable2, nullable3, filterSpan)
			Dim namespaceOrTypeAndImportsClausePositions As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition) = New ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)()
			Dim syntaxReferences As ImmutableArray(Of SyntaxReference) = New ImmutableArray(Of SyntaxReference)()
			Dim strs As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition) = Nothing
			Dim strs1 As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition) = Nothing
			SourceFile.BindImports(compilationUnitRoot.[Imports], binder, diagBag, namespaceOrTypeAndImportsClausePositions, syntaxReferences, strs, strs1, cancellationToken, filterSpan)
			Return New SourceFile.BoundFileInformation(namespaceOrTypeAndImportsClausePositions, syntaxReferences, strs, strs1, nullable, nullable1, nullable2, nullable3)
		End Function

		Private Shared Sub BindImports(ByVal importsListSyntax As SyntaxList(Of ImportsStatementSyntax), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag, <Out> ByRef importMembersOf As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), <Out> ByRef importMembersOfSyntax As ImmutableArray(Of SyntaxReference), <Out> ByRef importAliasesOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition), <Out> ByRef xmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition), ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal filterSpan As Nullable(Of TextSpan) = Nothing)
			Dim aliases As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition)
			Dim xmlNamespaces As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition)
			Dim instance As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition) = ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition).GetInstance()
			Dim syntaxReferences As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
			Dim fileImportDatum As SourceFile.FileImportData = New SourceFile.FileImportData(instance, syntaxReferences)
			Try
				Dim enumerator As SyntaxList(Of ImportsStatementSyntax).Enumerator = importsListSyntax.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ImportsStatementSyntax = enumerator.Current
					If (filterSpan.HasValue AndAlso Not filterSpan.Value.IntersectsWith(current.FullSpan)) Then
						Continue While
					End If
					cancellationToken.ThrowIfCancellationRequested()
					binder.Compilation.RecordImports(current)
					Dim enumerator1 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax).Enumerator = current.ImportsClauses.GetEnumerator()
					While enumerator1.MoveNext()
						Dim importsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax = enumerator1.Current
						If (filterSpan.HasValue AndAlso Not filterSpan.Value.IntersectsWith(current.FullSpan)) Then
							Continue While
						End If
						cancellationToken.ThrowIfCancellationRequested()
						binder.BindImportClause(importsClauseSyntax, fileImportDatum, diagBag)
					End While
				End While
				importMembersOf = instance.ToImmutable()
				importMembersOfSyntax = syntaxReferences.ToImmutable()
				If (fileImportDatum.Aliases.Count = 0) Then
					aliases = Nothing
				Else
					aliases = fileImportDatum.Aliases
				End If
				importAliasesOpt = aliases
				If (fileImportDatum.XmlNamespaces.Count > 0) Then
					xmlNamespaces = fileImportDatum.XmlNamespaces
				Else
					xmlNamespaces = Nothing
				End If
				xmlNamespacesOpt = xmlNamespaces
			Finally
				instance.Free()
				syntaxReferences.Free()
			End Try
		End Sub

		Private Shared Sub BindOptions(ByVal optionsSyntax As SyntaxList(Of OptionStatementSyntax), ByVal diagBag As DiagnosticBag, ByRef optionStrict As Nullable(Of Boolean), ByRef optionInfer As Nullable(Of Boolean), ByRef optionExplicit As Nullable(Of Boolean), ByRef optionCompareText As Nullable(Of Boolean), Optional ByVal filterSpan As Nullable(Of TextSpan) = Nothing)
			optionStrict = Nothing
			optionInfer = Nothing
			optionExplicit = Nothing
			optionCompareText = Nothing
			Dim enumerator As SyntaxList(Of OptionStatementSyntax).Enumerator = optionsSyntax.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As OptionStatementSyntax = enumerator.Current
				If (filterSpan.HasValue AndAlso Not filterSpan.Value.IntersectsWith(current.FullSpan)) Then
					Continue While
				End If
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.NameKeyword.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword) Then
							If (Not optionExplicit.HasValue) Then
								optionExplicit = New Nullable(Of Boolean)(Binder.DecodeOnOff(current.ValueKeyword))
							Else
								Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, New [Object]() { "Explicit" })
							End If
						End If
					ElseIf (Not optionCompareText.HasValue) Then
						optionCompareText = Binder.DecodeTextBinary(current.ValueKeyword)
					Else
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, New [Object]() { "Compare" })
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword) Then
						Continue While
					End If
					If (Not optionStrict.HasValue) Then
						optionStrict = New Nullable(Of Boolean)(Binder.DecodeOnOff(current.ValueKeyword))
					Else
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, New [Object]() { "Strict" })
					End If
				ElseIf (Not optionInfer.HasValue) Then
					optionInfer = New Nullable(Of Boolean)(Binder.DecodeOnOff(current.ValueKeyword))
				Else
					Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, New [Object]() { "Infer" })
				End If
			End While
		End Sub

		Private Function CreateQuickAttributeChecker() As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Dim quickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker = New Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker(Me._sourceModule.QuickAttributeChecker)
			Dim [imports] As SyntaxList(Of ImportsStatementSyntax) = Me._syntaxTree.GetCompilationUnitRoot().[Imports]
			Dim enumerator As SyntaxList(Of ImportsStatementSyntax).Enumerator = [imports].GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As SeparatedSyntaxList(Of ImportsClauseSyntax).Enumerator = enumerator.Current.ImportsClauses.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As ImportsClauseSyntax = enumerator1.Current
					If (current.Kind() <> SyntaxKind.SimpleImportsClause) Then
						Continue While
					End If
					Dim simpleImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax)
					If (simpleImportsClauseSyntax.[Alias] Is Nothing) Then
						Continue While
					End If
					quickAttributeChecker.AddAlias(simpleImportsClauseSyntax)
				End While
			End While
			quickAttributeChecker.Seal()
			Return quickAttributeChecker
		End Function

		Private Sub EnsureImportsValidated()
			If (Me._importsValidated = 0) Then
				Dim boundInformation As SourceFile.BoundFileInformation = Me.BoundInformation
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag()
				SourceFile.ValidateImports(Me._sourceModule.DeclaringCompilation, boundInformation.MemberImports, boundInformation.MemberImportsSyntax, boundInformation.AliasImportsOpt, bindingDiagnosticBag)
				Me._sourceModule.AtomicStoreIntegerAndDiagnostics(Me._importsValidated, 1, 0, bindingDiagnosticBag)
			End If
		End Sub

		Friend Sub GenerateAllDeclarationErrors()
			Dim boundInformation As SourceFile.BoundFileInformation = Me.BoundInformation
			Me.EnsureImportsValidated()
		End Sub

		Private Function GetBoundInformation(ByVal cancellationToken As System.Threading.CancellationToken) As SourceFile.BoundFileInformation
			If (Me._lazyBoundInformation Is Nothing) Then
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(New DiagnosticBag())
				Dim boundFileInformation As SourceFile.BoundFileInformation = Me.BindFileInformation(bindingDiagnosticBag.DiagnosticBag, cancellationToken, Nothing)
				Me._sourceModule.AtomicStoreReferenceAndDiagnostics(Of SourceFile.BoundFileInformation)(Me._lazyBoundInformation, boundFileInformation, bindingDiagnosticBag, Nothing)
			End If
			Return Me._lazyBoundInformation
		End Function

		Friend Function GetDeclarationErrorsInSpan(ByVal filterSpan As TextSpan, ByVal cancellationToken As System.Threading.CancellationToken) As IEnumerable(Of Diagnostic)
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Me.BindFileInformation(instance, cancellationToken, New Nullable(Of TextSpan)(filterSpan))
			Return DirectCast(instance.ToReadOnlyAndFree(), IEnumerable(Of Diagnostic))
		End Function

		Public Function GetUsedNamespaces() As ImmutableArray(Of UsedNamespaceOrType) Implements IImportScope.GetUsedNamespaces
			Return Me._lazyTranslatedImports
		End Function

		Public Function Translate(ByVal moduleBuilder As PEModuleBuilder, ByVal diagnostics As DiagnosticBag) As IImportScope
			If (Me._lazyTranslatedImports.IsDefault) Then
				ImmutableInterlocked.InterlockedInitialize(Of UsedNamespaceOrType)(Me._lazyTranslatedImports, Me.TranslateImports(moduleBuilder, diagnostics))
			End If
			Return Me
		End Function

		Private Function TranslateImports(ByVal moduleBuilder As Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder, ByVal diagnostics As DiagnosticBag) As ImmutableArray(Of UsedNamespaceOrType)
			Dim values As IEnumerable(Of AliasAndImportsClausePosition)
			Dim pEModuleBuilder As Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder = moduleBuilder
			Dim xmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition) = Me.XmlNamespacesOpt
			If (Me.AliasImportsOpt IsNot Nothing) Then
				values = Me.AliasImportsOpt.Values
			Else
				values = Nothing
			End If
			Return NamespaceScopeBuilder.BuildNamespaceScope(pEModuleBuilder, xmlNamespacesOpt, values, Me.MemberImports, diagnostics)
		End Function

		Private Shared Sub ValidateImports(ByVal compilation As VisualBasicCompilation, ByVal memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), ByVal memberImportsSyntax As ImmutableArray(Of SyntaxReference), ByVal aliasImportsOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As IEnumerator(Of AliasAndImportsClausePosition) = Nothing
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
			Dim length As Integer = memberImports.Length - 1
			Dim num As Integer = 0
			Do
				SourceFile.ValidateImportsClause(compilation, instance, memberImports(num).NamespaceOrType, memberImports(num).Dependencies, memberImportsSyntax(num).GetLocation(), memberImports(num).ImportsClausePosition, diagnostics)
				num = num + 1
			Loop While num <= length
			If (aliasImportsOpt IsNot Nothing) Then
				Try
					enumerator = aliasImportsOpt.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As AliasAndImportsClausePosition = enumerator.Current
						Dim target As NamespaceOrTypeSymbol = current.[Alias].Target
						Dim dependencies As ImmutableArray(Of AssemblySymbol) = current.Dependencies
						Dim locations As ImmutableArray(Of Location) = current.[Alias].Locations
						SourceFile.ValidateImportsClause(compilation, instance, target, dependencies, locations(0), current.ImportsClausePosition, diagnostics)
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
			instance.Free()
		End Sub

		Private Shared Sub ValidateImportsClause(ByVal compilation As VisualBasicCompilation, ByVal clauseDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal namespaceOrType As NamespaceOrTypeSymbol, ByVal dependencies As ImmutableArray(Of AssemblySymbol), ByVal location As Microsoft.CodeAnalysis.Location, ByVal importsClausePosition As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(namespaceOrType, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			If (typeSymbol Is Nothing) Then
				If (Not Microsoft.CodeAnalysis.Compilation.ReportUnusedImportsInTree(location.PossiblyEmbeddedOrMySourceTree())) Then
					diagnostics.AddAssembliesUsedByNamespaceReference(DirectCast(namespaceOrType, NamespaceSymbol))
				End If
				Return
			End If
			clauseDiagnostics.Clear()
			typeSymbol.CheckAllConstraints(location, clauseDiagnostics, New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, compilation.Assembly))
			diagnostics.AddRange(clauseDiagnostics.DiagnosticBag)
			If (Not Microsoft.CodeAnalysis.Compilation.ReportUnusedImportsInTree(location.PossiblyEmbeddedOrMySourceTree())) Then
				diagnostics.AddDependencies(dependencies)
				diagnostics.AddDependencies(clauseDiagnostics.DependenciesBag)
				Return
			End If
			If (clauseDiagnostics.DependenciesBag.Count <> 0) Then
				If (Not dependencies.IsEmpty) Then
					clauseDiagnostics.AddDependencies(dependencies)
				End If
				dependencies = clauseDiagnostics.DependenciesBag.ToImmutableArray()
			End If
			compilation.RecordImportsClauseDependencies(location.PossiblyEmbeddedOrMySourceTree(), importsClausePosition, dependencies)
		End Sub

		Private NotInheritable Class BoundFileInformation
			Public ReadOnly MemberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition)

			Public ReadOnly MemberImportsSyntax As ImmutableArray(Of SyntaxReference)

			Public ReadOnly AliasImportsOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition)

			Public ReadOnly XmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition)

			Public ReadOnly OptionStrict As Nullable(Of Boolean)

			Public ReadOnly OptionInfer As Nullable(Of Boolean)

			Public ReadOnly OptionExplicit As Nullable(Of Boolean)

			Public ReadOnly OptionCompareText As Nullable(Of Boolean)

			Public Sub New(ByVal memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition), ByVal memberImportsSyntax As ImmutableArray(Of SyntaxReference), ByVal importAliasesOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition), ByVal xmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition), ByVal optionStrict As Nullable(Of Boolean), ByVal optionInfer As Nullable(Of Boolean), ByVal optionExplicit As Nullable(Of Boolean), ByVal optionCompareText As Nullable(Of Boolean))
				MyBase.New()
				Me.MemberImports = memberImports
				Me.MemberImportsSyntax = memberImportsSyntax
				Me.AliasImportsOpt = importAliasesOpt
				Me.XmlNamespacesOpt = xmlNamespacesOpt
				Me.OptionStrict = optionStrict
				Me.OptionInfer = optionInfer
				Me.OptionExplicit = optionExplicit
				Me.OptionCompareText = optionCompareText
			End Sub
		End Class

		Private NotInheritable Class FileImportData
			Inherits ImportData
			Private ReadOnly _membersBuilder As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition)

			Private ReadOnly _membersSyntaxBuilder As ArrayBuilder(Of SyntaxReference)

			Public Sub New(ByVal membersBuilder As ArrayBuilder(Of NamespaceOrTypeAndImportsClausePosition), ByVal membersSyntaxBuilder As ArrayBuilder(Of SyntaxReference))
				MyBase.New(New HashSet(Of NamespaceOrTypeSymbol)(), New Dictionary(Of String, AliasAndImportsClausePosition)(CaseInsensitiveComparison.Comparer), New Dictionary(Of String, XmlNamespaceAndImportsClausePosition)())
				Me._membersBuilder = membersBuilder
				Me._membersSyntaxBuilder = membersSyntaxBuilder
			End Sub

			Public Overrides Sub AddAlias(ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal [alias] As AliasSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
				Me.Aliases.Add(name, New AliasAndImportsClausePosition([alias], importsClausePosition, dependencies.ToImmutableArray()))
			End Sub

			Public Overrides Sub AddMember(ByVal syntaxRef As SyntaxReference, ByVal member As NamespaceOrTypeSymbol, ByVal importsClausePosition As Integer, ByVal dependencies As IReadOnlyCollection(Of AssemblySymbol))
				Dim namespaceOrTypeAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.NamespaceOrTypeAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.NamespaceOrTypeAndImportsClausePosition(member, importsClausePosition, dependencies.ToImmutableArray())
				Me.Members.Add(member)
				Me._membersBuilder.Add(namespaceOrTypeAndImportsClausePosition)
				Me._membersSyntaxBuilder.Add(syntaxRef)
			End Sub
		End Class
	End Class
End Namespace