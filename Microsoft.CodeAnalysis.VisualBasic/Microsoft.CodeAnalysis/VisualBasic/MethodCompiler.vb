Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Debugging
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.CodeGen
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MethodCompiler
		Inherits VisualBasicSymbolVisitor
		Private ReadOnly _compilation As VisualBasicCompilation

		Private ReadOnly _cancellationToken As CancellationToken

		Private ReadOnly _emittingPdb As Boolean

		Private ReadOnly _emitTestCoverageData As Boolean

		Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Private ReadOnly _hasDeclarationErrors As Boolean

		Private ReadOnly _moduleBeingBuiltOpt As PEModuleBuilder

		Private ReadOnly _filterOpt As Predicate(Of Symbol)

		Private ReadOnly _debugDocumentProvider As DebugDocumentProvider

		Private ReadOnly _doEmitPhase As Boolean

		Private ReadOnly _doLoweringPhase As Boolean

		Private ReadOnly _compilerTasks As ConcurrentStack(Of Task)

		Private _globalHasErrors As Boolean

		Private ReadOnly Property DoEmitPhase As Boolean
			Get
				Return Me._doEmitPhase
			End Get
		End Property

		Private ReadOnly Property DoLoweringPhase As Boolean
			Get
				Return Me._doLoweringPhase
			End Get
		End Property

		Private ReadOnly Property GlobalHasErrors As Boolean
			Get
				Return Me._globalHasErrors
			End Get
		End Property

		Private Sub New(ByVal compilation As VisualBasicCompilation, ByVal moduleBeingBuiltOpt As PEModuleBuilder, ByVal emittingPdb As Boolean, ByVal emitTestCoverageData As Boolean, ByVal doLoweringPhase As Boolean, ByVal doEmitPhase As Boolean, ByVal hasDeclarationErrors As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal filter As Predicate(Of Symbol), ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.New()
			Me._compilation = compilation
			Me._moduleBeingBuiltOpt = moduleBeingBuiltOpt
			Me._diagnostics = diagnostics
			Me._hasDeclarationErrors = hasDeclarationErrors
			Me._cancellationToken = cancellationToken
			Me._doLoweringPhase = If(doEmitPhase, True, doLoweringPhase)
			Me._doEmitPhase = doEmitPhase
			Me._emittingPdb = emittingPdb
			Me._emitTestCoverageData = emitTestCoverageData
			Me._filterOpt = filter
			If (emittingPdb OrElse emitTestCoverageData) Then
				Me._debugDocumentProvider = Function(path As String, basePath As String) moduleBeingBuiltOpt.DebugDocumentsBuilder.GetOrAddDebugDocument(path, basePath, New Func(Of String, DebugSourceDocument)(AddressOf MethodCompiler.CreateDebugDocumentForFile))
			End If
			If (compilation.Options.ConcurrentBuild) Then
				Me._compilerTasks = New ConcurrentStack(Of Task)()
			End If
		End Sub

		<Conditional("DEBUG")>
		Private Sub AssertAllInitializersAreConstants(ByVal initializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)))
			If (Not initializers.IsDefaultOrEmpty) Then
				Dim enumerator As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)).Enumerator = initializers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ImmutableArray(Of FieldOrPropertyInitializer) = enumerator.Current
					If (current.IsEmpty) Then
						Continue While
					End If
					Dim enumerator1 As ImmutableArray(Of FieldOrPropertyInitializer).Enumerator = current.GetEnumerator()
					While enumerator1.MoveNext()
						Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = enumerator1.Current.FieldsOrProperties.GetEnumerator()
						While enumerator2.MoveNext()
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
						End While
					End While
				End While
			End If
		End Sub

		Private Shared Function BindAndAnalyzeMethodBody(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal containingTypeBinder As Binder, ByRef referencedConstructor As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByRef injectDefaultConstructorCall As Boolean, ByRef methodBodyBinder As Binder) As BoundBlock
			referencedConstructor = Nothing
			injectDefaultConstructorCall = False
			methodBodyBinder = Nothing
			Dim boundMethodBody As BoundBlock = method.GetBoundMethodBody(compilationState, diagnostics, methodBodyBinder)
			Analyzer.AnalyzeMethodBody(method, boundMethodBody, diagnostics.DiagnosticBag)
			DiagnosticsPass.IssueDiagnostics(boundMethodBody, diagnostics.DiagnosticBag, method)
			If (Not method.IsShared AndAlso compilationState.InitializeComponentOpt IsNot Nothing AndAlso Not method.IsImplicitlyDeclared) Then
				Try
					MethodCompiler.InitializeComponentCallTreeBuilder.CollectCallees(compilationState, method, boundMethodBody)
				Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException
					ProjectData.SetProjectError(cancelledByStackGuardException)
					cancelledByStackGuardException.AddAnError(diagnostics)
					ProjectData.ClearProjectError()
				End Try
			End If
			If (method.MethodKind = MethodKind.Constructor) Then
				injectDefaultConstructorCall = Not method.ContainingType.IsValueType
				If (boundMethodBody IsNot Nothing AndAlso boundMethodBody.Statements.Length > 0) Then
					Dim item As BoundStatement = boundMethodBody.Statements(0)
					If (item.HasErrors) Then
						injectDefaultConstructorCall = False
					ElseIf (item.Kind = BoundKind.ExpressionStatement) Then
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = MethodCompiler.TryGetMethodCalledInBoundExpressionStatement(DirectCast(item, BoundExpressionStatement))
						If (methodSymbol IsNot Nothing AndAlso methodSymbol.MethodKind = MethodKind.Constructor) Then
							referencedConstructor = methodSymbol
							injectDefaultConstructorCall = False
						End If
					End If
				End If
				If (injectDefaultConstructorCall) Then
					referencedConstructor = MethodCompiler.FindConstructorToCallByDefault(method, diagnostics, If(methodBodyBinder, containingTypeBinder))
				End If
			End If
			Return boundMethodBody
		End Function

		Private Shared Function BindDefaultConstructorInitializer(ByVal constructor As MethodSymbol, ByVal constructorToCall As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal binderOpt As Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim specialType As NamedTypeSymbol = constructor.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void)
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = constructor.Syntax
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, constructor.ContainingType)
			boundMeReference.SetWasCompilerGenerated()
			Dim boundCall As BoundExpression = Nothing
			If (constructorToCall.ParameterCount <> 0) Then
				Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(constructor.Syntax, Nothing, ImmutableArray.Create(Of MethodSymbol)(constructorToCall), LookupResultKind.Good, boundMeReference, QualificationKind.QualifiedViaValue, False)
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = constructor.Syntax
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundCall = binderOpt.BindInvocationExpression(syntaxNode, Nothing, TypeCharacter.None, boundMethodGroup, empty, strs, diagnostics, Nothing, True, False, False, Nothing, False)
			Else
				Dim u00210s As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, constructorToCall, Nothing, boundMeReference, u00210s, Nothing, specialType, False, False, bitVector)
			End If
			boundCall.SetWasCompilerGenerated()
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, boundCall, False)
			boundExpressionStatement.SetWasCompilerGenerated()
			Return boundExpressionStatement
		End Function

		Friend Shared Function BindDefaultConstructorInitializer(ByVal constructor As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim callByDefault As MethodSymbol = MethodCompiler.FindConstructorToCallByDefault(constructor, diagnostics, Nothing)
			If (callByDefault IsNot Nothing) Then
				boundExpressionStatement = MethodCompiler.BindDefaultConstructorInitializer(constructor, callByDefault, diagnostics, Nothing)
			Else
				boundExpressionStatement = Nothing
			End If
			Return boundExpressionStatement
		End Function

		Friend Shared Function CanBindMethod(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			If (method.IsExternalMethod OrElse method.IsMustOverride) Then
				flag = False
			ElseIf (method.IsDefaultValueTypeConstructor()) Then
				flag = False
			ElseIf (Not method.IsPartialWithoutImplementation()) Then
				If (Not method.IsImplicitlyDeclared) Then
					Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
					If (sourceMethodSymbol IsNot Nothing AndAlso sourceMethodSymbol.BlockSyntax IsNot Nothing) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Sub CompileMethod(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal methodOrdinal As Integer, ByRef withEventPropertyIdDispenser As Integer, ByRef delegateRelaxationIdDispenser As Integer, ByVal filter As Predicate(Of Symbol), ByVal compilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState, ByVal processedInitializers As Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers, ByVal containingTypeBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal previousSubmissionFields As SynthesizedSubmissionFields, Optional ByRef referencedConstructor As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing)
			Dim flag As Boolean = False
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Me._cancellationToken.ThrowIfCancellationRequested()
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			If (Not Me.DoLoweringPhase AndAlso sourceMethodSymbol IsNot Nothing) Then
				Dim diagnostics As ImmutableArray(Of Diagnostic) = sourceMethodSymbol.Diagnostics
				If (Not diagnostics.IsDefault) Then
					Me._diagnostics.AddRange(Of Diagnostic)(diagnostics)
					Return
				End If
			End If
			If (MethodCompiler.CanBindMethod(method)) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(Me._diagnostics)
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = MethodCompiler.BindAndAnalyzeMethodBody(method, compilationState, instance, containingTypeBinder, referencedConstructor, flag, binder)
				processedInitializers.EnsureInitializersAnalyzed(method, instance.DiagnosticBag)
				Dim flag1 As Boolean = If(Me._hasDeclarationErrors OrElse instance.HasAnyErrors() OrElse processedInitializers.HasAnyErrors, True, boundBlock.HasErrors)
				Me.SetGlobalErrorIfTrue(flag1)
				If (sourceMethodSymbol IsNot Nothing AndAlso sourceMethodSymbol.SetDiagnostics(instance.DiagnosticBag.ToReadOnly())) Then
					Dim compilation As VisualBasicCompilation = compilationState.Compilation
					If (compilation.ShouldAddEvent(method)) Then
						If (boundBlock IsNot Nothing) Then
							Dim semanticModel As SyntaxTreeSemanticModel = Nothing
							Dim semanticModelProvider As CachingSemanticModelProvider = TryCast(compilation.SemanticModelProvider, CachingSemanticModelProvider)
							If (semanticModelProvider IsNot Nothing) Then
								Dim syntax As SyntaxNode = boundBlock.Syntax
								semanticModel = DirectCast(semanticModelProvider.GetSemanticModel(syntax.SyntaxTree, compilation, False), SyntaxTreeSemanticModel)
								Dim memberSemanticModel As MethodBodySemanticModel = DirectCast(semanticModel.GetMemberSemanticModel(syntax), MethodBodySemanticModel)
								If (memberSemanticModel IsNot Nothing) Then
									memberSemanticModel.CacheBoundNodes(boundBlock, syntax)
								End If
							End If
							compilation.EventQueue.TryEnqueue(New SymbolDeclaredCompilationEvent(compilation, method, semanticModel))
						Else
							compilation.SymbolDeclaredEvent(sourceMethodSymbol)
						End If
					End If
				End If
				If (Not Me.DoLoweringPhase AndAlso sourceMethodSymbol IsNot Nothing) Then
					Me._diagnostics.AddRange(Of Diagnostic)(sourceMethodSymbol.Diagnostics)
					Return
				End If
				If (Me.DoLoweringPhase AndAlso Not flag1) Then
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method
					Dim num As Integer = methodOrdinal
					Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = boundBlock
					Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder = If(binder, containingTypeBinder)
					Dim typeCompilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState = compilationState
					Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = instance
					Dim processedFieldOrPropertyInitializer As Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers = processedInitializers
					Dim synthesizedSubmissionField As SynthesizedSubmissionFields = previousSubmissionFields
					If (flag) Then
						methodSymbol = referencedConstructor
					Else
						methodSymbol = Nothing
					End If
					Me.LowerAndEmitMethod(methodSymbol1, num, boundBlock1, binder1, typeCompilationState, bindingDiagnosticBag, processedFieldOrPropertyInitializer, synthesizedSubmissionField, methodSymbol, delegateRelaxationIdDispenser)
					Dim handledEvents As ImmutableArray(Of HandledEvent) = method.HandledEvents
					If (Not handledEvents.IsEmpty) Then
						Me.CreateSyntheticWithEventOverridesIfNeeded(handledEvents, delegateRelaxationIdDispenser, withEventPropertyIdDispenser, compilationState, containingTypeBinder, instance, previousSubmissionFields)
					End If
				End If
				Me._diagnostics.AddRange(instance, False)
				instance.Free()
			ElseIf (sourceMethodSymbol IsNot Nothing AndAlso sourceMethodSymbol.SetDiagnostics(ImmutableArray(Of Diagnostic).Empty)) Then
				sourceMethodSymbol.DeclaringCompilation.SymbolDeclaredEvent(method)
				Return
			End If
		End Sub

		Friend Shared Sub CompileMethodBodies(ByVal compilation As VisualBasicCompilation, ByVal moduleBeingBuiltOpt As PEModuleBuilder, ByVal emittingPdb As Boolean, ByVal emitTestCoverageData As Boolean, ByVal hasDeclarationErrors As Boolean, ByVal filter As Predicate(Of Symbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			If (compilation.PreviousSubmission IsNot Nothing) Then
				compilation.PreviousSubmission.EnsureAnonymousTypeTemplates(cancellationToken)
			End If
			Dim methodCompiler As Microsoft.CodeAnalysis.VisualBasic.MethodCompiler = New Microsoft.CodeAnalysis.VisualBasic.MethodCompiler(compilation, moduleBeingBuiltOpt, emittingPdb, emitTestCoverageData, True, True, hasDeclarationErrors, diagnostics, filter, cancellationToken)
			compilation.SourceModule.GlobalNamespace.Accept(methodCompiler)
			methodCompiler.WaitForWorkers()
			If (moduleBeingBuiltOpt IsNot Nothing) Then
				Dim additionalTopLevelTypes As ImmutableArray(Of NamedTypeSymbol) = moduleBeingBuiltOpt.GetAdditionalTopLevelTypes()
				If (Not additionalTopLevelTypes.IsEmpty) Then
					methodCompiler.CompileSynthesizedMethods(additionalTopLevelTypes)
				End If
				compilation.AnonymousTypeManager.AssignTemplatesNamesAndCompile(methodCompiler, moduleBeingBuiltOpt, diagnostics)
				methodCompiler.WaitForWorkers()
				If (compilation.EmbeddedSymbolManager.Embedded <> EmbeddedSymbolKind.None) Then
					methodCompiler.ProcessEmbeddedMethods()
				End If
				Dim privateImplClass As PrivateImplementationDetails = moduleBeingBuiltOpt.PrivateImplClass
				If (privateImplClass IsNot Nothing) Then
					privateImplClass.Freeze()
					methodCompiler.CompileSynthesizedMethods(privateImplClass)
				End If
			End If
			Dim entryPoint As MethodSymbol = Microsoft.CodeAnalysis.VisualBasic.MethodCompiler.GetEntryPoint(compilation, moduleBeingBuiltOpt, diagnostics, cancellationToken)
			If (moduleBeingBuiltOpt IsNot Nothing) Then
				If (entryPoint IsNot Nothing AndAlso compilation.Options.OutputKind.IsApplication()) Then
					moduleBeingBuiltOpt.SetPEEntryPoint(entryPoint, diagnostics.DiagnosticBag)
				End If
				If ((methodCompiler.GlobalHasErrors OrElse moduleBeingBuiltOpt.SourceModule.HasBadAttributes) AndAlso Not hasDeclarationErrors AndAlso Not diagnostics.HasAnyErrors()) Then
					Dim str As String = If(methodCompiler.GlobalHasErrors, "UnableToDetermineSpecificCauseOfFailure", "ModuleHasInvalidAttributes")
					diagnostics.Add(ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, New [Object]() { moduleBeingBuiltOpt.SourceModule.Name, New LocalizableResourceString(str, CodeAnalysisResources.ResourceManager, GetType(CodeAnalysisResources)) })
				End If
			End If
		End Sub

		Private Sub CompileNamedType(ByVal containingType As NamedTypeSymbol, ByVal filter As Predicate(Of Symbol))
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim synthesizedSubmissionField As SynthesizedSubmissionFields
			If (Not containingType.IsEmbedded) Then
				Dim scriptConstructor As SynthesizedConstructorBase = Nothing
				Dim scriptInitializer As SynthesizedInteractiveInitializerMethod = Nothing
				Dim scriptEntryPoint As SynthesizedEntryPointSymbol = Nothing
				Dim num As Integer = -1
				If (containingType.IsScriptClass) Then
					scriptConstructor = containingType.GetScriptConstructor()
					scriptInitializer = containingType.GetScriptInitializer()
					scriptEntryPoint = containingType.GetScriptEntryPoint()
				End If
				Dim empty As Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers = Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers.Empty
				Dim processedFieldOrPropertyInitializer As Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers = Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers.Empty
				If (containingType.IsSubmissionClass) Then
					synthesizedSubmissionField = New SynthesizedSubmissionFields(Me._compilation, containingType)
				Else
					synthesizedSubmissionField = Nothing
				End If
				Dim synthesizedSubmissionField1 As SynthesizedSubmissionFields = synthesizedSubmissionField
				Dim sourceMemberContainerTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol = TryCast(containingType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol)
				Dim designerInitializeComponentMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (sourceMemberContainerTypeSymbol IsNot Nothing AndAlso Me.DoLoweringPhase) Then
					designerInitializeComponentMethod = MethodCompiler.GetDesignerInitializeComponentMethod(sourceMemberContainerTypeSymbol)
				End If
				Dim typeCompilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState = New Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState(Me._compilation, Me._moduleBeingBuiltOpt, designerInitializeComponentMethod)
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
				If (sourceMemberContainerTypeSymbol IsNot Nothing) Then
					Dim containingModule As SourceModuleSymbol = DirectCast(sourceMemberContainerTypeSymbol.ContainingModule, SourceModuleSymbol)
					Dim locations As ImmutableArray(Of Location) = sourceMemberContainerTypeSymbol.Locations
					binder = BinderBuilder.CreateBinderForType(containingModule, locations(0).PossiblyEmbeddedOrMySourceTree(), sourceMemberContainerTypeSymbol)
					empty = New Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers(Microsoft.CodeAnalysis.VisualBasic.Binder.BindFieldAndPropertyInitializers(sourceMemberContainerTypeSymbol, sourceMemberContainerTypeSymbol.StaticInitializers, scriptInitializer, Me._diagnostics))
					processedFieldOrPropertyInitializer = New Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers(Microsoft.CodeAnalysis.VisualBasic.Binder.BindFieldAndPropertyInitializers(sourceMemberContainerTypeSymbol, sourceMemberContainerTypeSymbol.InstanceInitializers, scriptInitializer, Me._diagnostics))
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = sourceMemberContainerTypeSymbol.CreateSharedConstructorsForConstFieldsIfRequired(binder, Me._diagnostics)
					If (methodSymbol1 IsNot Nothing AndAlso MethodCompiler.PassesFilter(filter, methodSymbol1)) Then
						Dim num1 As Integer = 0
						Dim num2 As Integer = 0
						methodSymbol = Nothing
						Me.CompileMethod(methodSymbol1, -1, num1, num2, filter, typeCompilationState, empty, binder, synthesizedSubmissionField1, methodSymbol)
						If (Me._moduleBeingBuiltOpt IsNot Nothing) Then
							Me._moduleBeingBuiltOpt.AddSynthesizedDefinition(sourceMemberContainerTypeSymbol, methodSymbol1.GetCciAdapter())
						End If
					End If
				End If
				Dim methodSymbols As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Nothing
				Dim members As ImmutableArray(Of Symbol) = containingType.GetMembers()
				Dim num3 As Integer = 0
				Dim num4 As Integer = 0
				Dim length As Integer = members.Length - 1
				Dim num5 As Integer = 0
				Do
					Dim item As Symbol = members(num5)
					If (MethodCompiler.PassesFilter(filter, item)) Then
						Dim kind As SymbolKind = item.Kind
						If (kind = SymbolKind.Method) Then
							Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (methodSymbol2.IsScriptConstructor) Then
								num = num5
							ElseIf (methodSymbol2 <> scriptEntryPoint) Then
								If (methodSymbol2.IsPartial()) Then
									Dim partialImplementationPart As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol2.PartialImplementationPart
									If (CObj(partialImplementationPart) <> CObj(methodSymbol2)) Then
										If (DirectCast(methodSymbol2, SourceMethodSymbol).SetDiagnostics(ImmutableArray(Of Diagnostic).Empty)) Then
											methodSymbol2.DeclaringCompilation.SymbolDeclaredEvent(methodSymbol2)
										End If
										If (partialImplementationPart Is Nothing) Then
											GoTo Label0
										End If
										methodSymbol2 = partialImplementationPart
									End If
								End If
								Dim empty1 As Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers = Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers.Empty
								If (methodSymbol2.MethodKind = MethodKind.StaticConstructor) Then
									empty1 = empty
								ElseIf (methodSymbol2.MethodKind = MethodKind.Constructor OrElse methodSymbol2.IsScriptInitializer) Then
									empty1 = processedFieldOrPropertyInitializer
								End If
								Dim methodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
								Me.CompileMethod(methodSymbol2, num5, num3, num4, filter, typeCompilationState, empty1, binder, synthesizedSubmissionField1, methodSymbol3)
								If (methodSymbol3 IsNot Nothing AndAlso methodSymbol3.ContainingType.Equals(containingType)) Then
									If (methodSymbols Is Nothing) Then
										methodSymbols = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
									End If
									methodSymbols.Add(methodSymbol2, methodSymbol3)
								End If
								If (Me.DoLoweringPhase AndAlso Me._moduleBeingBuiltOpt IsNot Nothing) Then
									Me.CreateExplicitInterfaceImplementationStubs(typeCompilationState, methodSymbol2)
								End If
							End If
						ElseIf (kind = SymbolKind.NamedType) Then
							item.Accept(Me)
						End If
					End If
				Label0:
					num5 = num5 + 1
				Loop While num5 <= length
				If (methodSymbols IsNot Nothing) Then
					Me.DetectAndReportCyclesInConstructorCalls(methodSymbols, Me._diagnostics)
				End If
				If (scriptConstructor IsNot Nothing) Then
					methodSymbol = Nothing
					Me.CompileMethod(scriptConstructor, num, num3, num4, filter, typeCompilationState, Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers.Empty, binder, synthesizedSubmissionField1, methodSymbol)
					If (synthesizedSubmissionField1 IsNot Nothing AndAlso Me._moduleBeingBuiltOpt IsNot Nothing) Then
						synthesizedSubmissionField1.AddToType(containingType, Me._moduleBeingBuiltOpt)
					End If
				End If
				If (designerInitializeComponentMethod IsNot Nothing) Then
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = containingType.GetMembers().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Symbol = enumerator.Current
						If (current.IsShared OrElse Not current.IsFromCompilation(Me._compilation) OrElse current.Kind <> SymbolKind.Method) Then
							Continue While
						End If
						Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
						If (sourceMemberMethodSymbol Is Nothing OrElse sourceMemberMethodSymbol.MethodKind <> MethodKind.Constructor OrElse typeCompilationState.CallsInitializeComponent(sourceMemberMethodSymbol)) Then
							Continue While
						End If
						Dim nonMergedLocation As Location = sourceMemberMethodSymbol.NonMergedLocation
						If (nonMergedLocation Is Nothing) Then
							Continue While
						End If
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(Me._diagnostics, nonMergedLocation, ERRID.WRN_ExpectedInitComponentCall2, New [Object]() { sourceMemberMethodSymbol, sourceMemberContainerTypeSymbol })
					End While
				End If
				If (Me._moduleBeingBuiltOpt IsNot Nothing) Then
					Me.CompileSynthesizedMethods(typeCompilationState)
				End If
				typeCompilationState.Free()
			End If
		End Sub

		Private Function CompileNamedTypeAsync(ByVal symbol As NamedTypeSymbol, ByVal filter As Predicate(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)) As Task
			Return Task.Run(UICultureUtilities.WithCurrentUICulture(Sub()
			Try
				Me.CompileNamedType(symbol, filter)
			Catch exception As System.Exception When FatalError.ReportAndPropagateUnlessCanceled(exception)
				Throw ExceptionUtilities.Unreachable
			End Try
			End Sub), Me._cancellationToken)
		End Function

		Private Sub CompileNamespace(ByVal symbol As NamespaceSymbol)
			If (MethodCompiler.PassesFilter(Me._filterOpt, symbol)) Then
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembersUnordered().GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.Accept(Me)
				End While
			End If
		End Sub

		Private Function CompileNamespaceAsync(ByVal symbol As NamespaceSymbol) As Task
			Return Task.Run(UICultureUtilities.WithCurrentUICulture(Sub()
			Try
				Me.CompileNamespace(symbol)
			Catch exception As System.Exception When FatalError.ReportAndPropagateUnlessCanceled(exception)
				Throw ExceptionUtilities.Unreachable
			End Try
			End Sub), Me._cancellationToken)
		End Function

		Private Sub CompileSynthesizedMethods(privateImplClass As PrivateImplementationDetails)
			Debug.Assert(_moduleBeingBuiltOpt IsNot Nothing)

			Dim compilationState As New TypeCompilationState(_compilation, _moduleBeingBuiltOpt, initializeComponentOpt:=Nothing)
			For Each methodDef In privateImplClass.GetMethods(Nothing)
				Dim method = DirectCast(methodDef.GetInternalSymbol(), MethodSymbol)
				Dim diagnosticsThisMethod = BindingDiagnosticBag.GetInstance(_diagnostics)

				Dim boundBody = method.GetBoundMethodBody(compilationState, diagnosticsThisMethod)

				If DoEmitPhase AndAlso Not diagnosticsThisMethod.HasAnyErrors Then
					Dim emittedBody = GenerateMethodBody(_moduleBeingBuiltOpt,
														 method,
														 methodOrdinal:=DebugId.UndefinedOrdinal,
														 block:=boundBody,
														 lambdaDebugInfo:=ImmutableArray(Of LambdaDebugInfo).Empty,
														 closureDebugInfo:=ImmutableArray(Of ClosureDebugInfo).Empty,
														 stateMachineTypeOpt:=Nothing,
														 variableSlotAllocatorOpt:=Nothing,
														 debugDocumentProvider:=If(_emitTestCoverageData, _debugDocumentProvider, Nothing),
														 diagnostics:=diagnosticsThisMethod,
														 emittingPdb:=False,
														 emitTestCoverageData:=_emitTestCoverageData,
														 dynamicAnalysisSpans:=ImmutableArray(Of SourceSpan).Empty)


					' error while generating IL
					If emittedBody Is Nothing Then
						_diagnostics.AddRange(diagnosticsThisMethod)
						diagnosticsThisMethod.Free()
						Exit For
					End If

					_moduleBeingBuiltOpt.SetMethodBody(method, emittedBody)
				End If

				_diagnostics.AddRange(diagnosticsThisMethod)
				diagnosticsThisMethod.Free()
			Next

			compilationState.Free()
		End Sub

		Private Sub CompileSynthesizedMethods(ByVal additionalTypes As ImmutableArray(Of NamedTypeSymbol))
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Nothing
			Dim typeCompilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState = New Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState(Me._compilation, Me._moduleBeingBuiltOpt, Nothing)
			Dim enumerator1 As ImmutableArray(Of NamedTypeSymbol).Enumerator = additionalTypes.GetEnumerator()
		Label0:
			While enumerator1.MoveNext()
				Dim current As NamedTypeSymbol = enumerator1.Current
				Using num As Integer = 0
					enumerator = current.GetMethodsToEmit().GetEnumerator()
					While enumerator.MoveNext()
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator.Current
						Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(Me._diagnostics)
						Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
						Dim boundMethodBody As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = methodSymbol.GetBoundMethodBody(typeCompilationState, instance, binder)
						Dim methodBody As Microsoft.CodeAnalysis.CodeGen.MethodBody = Nothing
						If (Not instance.HasAnyErrors()) Then
							Dim variableSlotAllocator As Microsoft.CodeAnalysis.CodeGen.VariableSlotAllocator = Nothing
							Dim stateMachineTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol = Nothing
							Dim lambdaDebugInfos As ArrayBuilder(Of LambdaDebugInfo) = ArrayBuilder(Of LambdaDebugInfo).GetInstance()
							Dim closureDebugInfos As ArrayBuilder(Of ClosureDebugInfo) = ArrayBuilder(Of ClosureDebugInfo).GetInstance()
							Dim num1 As Integer = 0
							Dim empty As ImmutableArray(Of SourceSpan) = ImmutableArray(Of SourceSpan).Empty
							Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Rewriter.LowerBodyOrInitializer(methodSymbol, num, boundMethodBody, Nothing, typeCompilationState, False, empty, Me._debugDocumentProvider, instance, variableSlotAllocator, lambdaDebugInfos, closureDebugInfos, num1, stateMachineTypeSymbol, Me._moduleBeingBuiltOpt.AllowOmissionOfConditionalCalls, True)
							If (Me.DoEmitPhase AndAlso Not instance.HasAnyErrors()) Then
								methodBody = MethodCompiler.GenerateMethodBody(Me._moduleBeingBuiltOpt, methodSymbol, -1, boundBlock, lambdaDebugInfos.ToImmutable(), closureDebugInfos.ToImmutable(), stateMachineTypeSymbol, variableSlotAllocator, Nothing, instance, False, False, empty)
							End If
							lambdaDebugInfos.Free()
							closureDebugInfos.Free()
						End If
						Me._diagnostics.AddRange(instance, False)
						instance.Free()
						If (methodBody IsNot Nothing) Then
							Me._moduleBeingBuiltOpt.SetMethodBody(methodSymbol, methodBody)
						ElseIf (Me.DoEmitPhase) Then
							GoTo Label0
						End If
						num = num + 1
					End While
				End Using
			End While
			If (Not Me._diagnostics.HasAnyErrors()) Then
				Me.CompileSynthesizedMethods(typeCompilationState)
			End If
			typeCompilationState.Free()
		End Sub

		Private Sub CompileSynthesizedMethods(ByVal compilationState As TypeCompilationState)
			If (Me.DoEmitPhase AndAlso compilationState.HasSynthesizedMethods) Then
				Dim enumerator As ArrayBuilder(Of TypeCompilationState.MethodWithBody).Enumerator = compilationState.SynthesizedMethods.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeCompilationState.MethodWithBody = enumerator.Current
					If (current.Body.HasErrors) Then
						Continue While
					End If
					Dim method As MethodSymbol = current.Method
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(Me._diagnostics)
					Dim methodBody As Microsoft.CodeAnalysis.CodeGen.MethodBody = MethodCompiler.GenerateMethodBody(Me._moduleBeingBuiltOpt, method, -1, current.Body, ImmutableArray(Of LambdaDebugInfo).Empty, ImmutableArray(Of ClosureDebugInfo).Empty, Nothing, Nothing, Me._debugDocumentProvider, instance, Me._emittingPdb, Me._emitTestCoverageData, ImmutableArray(Of SourceSpan).Empty)
					Me._diagnostics.AddRange(instance, False)
					instance.Free()
					If (methodBody Is Nothing) Then
						Exit While
					End If
					Me._moduleBeingBuiltOpt.SetMethodBody(method, methodBody)
				End While
			End If
		End Sub

		Private Shared Function CreateDebugDocumentForFile(ByVal normalizedPath As String) As DebugSourceDocument
			Return New DebugSourceDocument(normalizedPath, DebugSourceDocument.CorSymLanguageTypeBasic)
		End Function

		Private Sub CreateExplicitInterfaceImplementationStubs(ByVal compilationState As TypeCompilationState, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol) = Nothing
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = method.ExplicitInterfaceImplementations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator.Current
				If (MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(method, current) OrElse Not MethodSignatureComparer.ParametersAndReturnTypeSignatureComparer.Equals(method, current)) Then
					Continue While
				End If
				If (instance Is Nothing) Then
					instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol).GetInstance()
				End If
				Dim synthesizedInterfaceImplementationStubSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol = Nothing
				Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol).Enumerator = instance.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol = enumerator1.Current
					If (Not MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(current1, current)) Then
						Continue While
					End If
					synthesizedInterfaceImplementationStubSymbol = current1
					Exit While
				End While
				If (synthesizedInterfaceImplementationStubSymbol Is Nothing) Then
					synthesizedInterfaceImplementationStubSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol(method, current)
					instance.Add(synthesizedInterfaceImplementationStubSymbol)
					Dim synthesizedInterfaceImplementationStubSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol = synthesizedInterfaceImplementationStubSymbol
					Dim synthesizedInterfaceImplementationStubSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol = synthesizedInterfaceImplementationStubSymbol
					Dim syntax As SyntaxNode = method.Syntax
					If (syntax Is Nothing) Then
						syntax = VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken())
					End If
					Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(synthesizedInterfaceImplementationStubSymbol1, synthesizedInterfaceImplementationStubSymbol2, syntax, compilationState, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
					methodSymbol = If(Not method.IsGenericMethod, method, method.Construct(synthesizedInterfaceImplementationStubSymbol.TypeArguments))
					Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance(synthesizedInterfaceImplementationStubSymbol.ParameterCount)
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = synthesizedInterfaceImplementationStubSymbol.Parameters.GetEnumerator()
					While enumerator2.MoveNext()
						Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator2.Current
						Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = syntheticBoundNodeFactory.Parameter(parameterSymbol)
						If (Not parameterSymbol.IsByRef) Then
							boundParameter = boundParameter.MakeRValue()
						End If
						boundExpressions.Add(boundParameter)
					End While
					Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = syntheticBoundNodeFactory.[Call](syntheticBoundNodeFactory.[Me](), methodSymbol, boundExpressions.ToImmutableAndFree())
					boundBlock = If(Not method.IsSub, syntheticBoundNodeFactory.Block(New BoundStatement() { syntheticBoundNodeFactory.[Return](boundCall) }), syntheticBoundNodeFactory.Block(New BoundStatement() { syntheticBoundNodeFactory.ExpressionStatement(boundCall), syntheticBoundNodeFactory.[Return](Nothing) }))
					syntheticBoundNodeFactory.CloseMethod(boundBlock)
					Me._moduleBeingBuiltOpt.AddSynthesizedDefinition(method.ContainingType, synthesizedInterfaceImplementationStubSymbol.GetCciAdapter())
				End If
				synthesizedInterfaceImplementationStubSymbol.AddImplementedMethod(current)
			End While
			If (instance IsNot Nothing) Then
				Dim enumerator3 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInterfaceImplementationStubSymbol).Enumerator = instance.GetEnumerator()
				While enumerator3.MoveNext()
					enumerator3.Current.Seal()
				End While
				instance.Free()
			End If
		End Sub

		Private Sub CreateSyntheticWithEventOverridesIfNeeded(ByVal handledEvents As ImmutableArray(Of HandledEvent), ByRef delegateRelaxationIdDispenser As Integer, ByRef withEventPropertyIdDispenser As Integer, ByVal compilationState As TypeCompilationState, ByVal containingTypeBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal previousSubmissionFields As SynthesizedSubmissionFields)
			Dim enumerator As ImmutableArray(Of HandledEvent).Enumerator = handledEvents.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As HandledEvent = enumerator.Current
				If (current.HandlesKind <> HandledEventKind.[WithEvents]) Then
					Continue While
				End If
				Dim associatedSymbol As SynthesizedOverridingWithEventsProperty = TryCast(current.hookupMethod.AssociatedSymbol, SynthesizedOverridingWithEventsProperty)
				If (associatedSymbol Is Nothing) Then
					Continue While
				End If
				Dim getMethod As MethodSymbol = associatedSymbol.GetMethod
				If (compilationState.HasMethodWrapper(getMethod)) Then
					Continue While
				End If
				Dim setMethod As MethodSymbol = associatedSymbol.SetMethod
				Dim containingType As NamedTypeSymbol = associatedSymbol.ContainingType
				Dim boundMethodBody As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = getMethod.GetBoundMethodBody(compilationState, diagnostics, containingTypeBinder)
				compilationState.AddMethodWrapper(getMethod, getMethod, boundMethodBody)
				Me._moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, getMethod.GetCciAdapter())
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = setMethod.GetBoundMethodBody(compilationState, diagnostics, containingTypeBinder)
				Dim instance As ArrayBuilder(Of LambdaDebugInfo) = ArrayBuilder(Of LambdaDebugInfo).GetInstance()
				Dim closureDebugInfos As ArrayBuilder(Of ClosureDebugInfo) = ArrayBuilder(Of ClosureDebugInfo).GetInstance()
				Dim empty As ImmutableArray(Of SourceSpan) = ImmutableArray(Of SourceSpan).Empty
				Dim variableSlotAllocator As Microsoft.CodeAnalysis.CodeGen.VariableSlotAllocator = Nothing
				Dim stateMachineTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol = Nothing
				boundBlock = Rewriter.LowerBodyOrInitializer(setMethod, withEventPropertyIdDispenser, boundBlock, previousSubmissionFields, compilationState, False, empty, Me._debugDocumentProvider, diagnostics, variableSlotAllocator, instance, closureDebugInfos, delegateRelaxationIdDispenser, stateMachineTypeSymbol, True, True)
				instance.Free()
				closureDebugInfos.Free()
				compilationState.AddMethodWrapper(setMethod, setMethod, boundBlock)
				Me._moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, setMethod.GetCciAdapter())
				Me._moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, associatedSymbol.GetCciAdapter())
				withEventPropertyIdDispenser = withEventPropertyIdDispenser + 1
			End While
		End Sub

		Private Sub DetectAndReportCyclesInConstructorCalls(ByVal constructorCallMap As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim num As Integer
			Dim methodSymbols As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, Integer) = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, Integer)()
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = constructorCallMap.Keys.First()
			While True
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (constructorCallMap.TryGetValue(methodSymbol, methodSymbol1)) Then
					constructorCallMap.Remove(methodSymbol)
					methodSymbols.Add(methodSymbol, instance.Count)
					instance.Add(methodSymbol)
					If (Not methodSymbols.TryGetValue(methodSymbol1, num)) Then
						methodSymbol = methodSymbol1
						Continue While
					Else
						instance.Add(methodSymbol1)
						MethodCompiler.ReportConstructorCycles(num, instance.Count - 1, instance, diagnostics)
					End If
				End If
				methodSymbols.Clear()
				instance.Clear()
				If (constructorCallMap.Count = 0) Then
					Exit While
				End If
				methodSymbol = constructorCallMap.Keys.First()
			End While
			instance.Free()
		End Sub

		Private Shared Function FindConstructorToCallByDefault(ByVal constructor As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal binderForAccessibilityCheckOpt As Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim locations As ImmutableArray(Of Location)
			Dim containingType As NamedTypeSymbol = constructor.ContainingType
			If (Not containingType.IsSubmissionClass) Then
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = containingType.BaseTypeNoUseSiteDiagnostics
				If (baseTypeNoUseSiteDiagnostics Is Nothing OrElse baseTypeNoUseSiteDiagnostics.IsErrorType()) Then
					methodSymbol = Nothing
				Else
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
					Dim flag As Boolean = False
					Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, containingType.ContainingAssembly)
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = baseTypeNoUseSiteDiagnostics.InstanceConstructors.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator.Current
						If (current.IsGenericMethod) Then
							Continue While
						End If
						If (binderForAccessibilityCheckOpt Is Nothing) Then
							If (current.DeclaredAccessibility <> Accessibility.[Public]) Then
								Continue While
							End If
							If (current.ParameterCount <> 0) Then
								flag = True
								Continue While
							End If
						Else
							Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
							If (Not binderForAccessibilityCheckOpt.IsAccessible(current, compoundUseSiteInfo, containingType, basesBeingResolved)) Then
								Continue While
							End If
						End If
						flag = True
						If (If(containingType.IsReferenceType, Not current.CanBeCalledWithNoParameters(), current.ParameterCount <> 0)) Then
							Continue While
						End If
						If (methodSymbol1 IsNot Nothing) Then
							If (Not constructor.IsImplicitlyDeclared) Then
								Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_RequiredNewCallTooMany2, New [Object]() { baseTypeNoUseSiteDiagnostics, containingType })
								locations = constructor.Locations
								diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
							Else
								Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_NoUniqueConstructorOnBase2, New [Object]() { containingType, containingType.BaseTypeNoUseSiteDiagnostics })
								locations = containingType.Locations
								diagnostics.Add(New VBDiagnostic(diagnosticInfo1, locations(0), False))
							End If
							methodSymbol = methodSymbol1
							Return methodSymbol
						Else
							methodSymbol1 = current
						End If
					End While
					Dim locations1 As ImmutableArray(Of Location) = If(constructor.IsImplicitlyDeclared, containingType.Locations, constructor.Locations)
					diagnostics.Add(If(locations1.IsDefaultOrEmpty, Location.None, locations1(0)), compoundUseSiteInfo)
					If (methodSymbol1 Is Nothing) Then
						If (Not flag) Then
							Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_NoAccessibleConstructorOnBase, New [Object]() { containingType.BaseTypeNoUseSiteDiagnostics })
							locations = containingType.Locations
							diagnostics.Add(New VBDiagnostic(diagnosticInfo2, locations(0), False))
						ElseIf (Not constructor.IsImplicitlyDeclared) Then
							Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_RequiredNewCall2, New [Object]() { baseTypeNoUseSiteDiagnostics, containingType })
							locations = constructor.Locations
							diagnostics.Add(New VBDiagnostic(diagnosticInfo3, locations(0), False))
						Else
							Dim diagnosticInfo4 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_NoConstructorOnBase2, New [Object]() { containingType, containingType.BaseTypeNoUseSiteDiagnostics })
							locations = containingType.Locations
							diagnostics.Add(New VBDiagnostic(diagnosticInfo4, locations(0), False))
						End If
					End If
					If (methodSymbol1 IsNot Nothing) Then
						methodSymbol1.ForceCompleteObsoleteAttribute()
						If (methodSymbol1.ObsoleteState = ThreeState.[True]) Then
							Dim obsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData = methodSymbol1.ObsoleteAttributeData
							If (constructor.IsImplicitlyDeclared) Then
								If (Not [String].IsNullOrEmpty(obsoleteAttributeData.Message)) Then
									diagnostics.Add(If(obsoleteAttributeData.IsError, ERRID.ERR_NoNonObsoleteConstructorOnBase4, ERRID.WRN_NoNonObsoleteConstructorOnBase4), containingType.Locations(0), New [Object]() { containingType, methodSymbol1, containingType.BaseTypeNoUseSiteDiagnostics, obsoleteAttributeData.Message })
								Else
									diagnostics.Add(If(obsoleteAttributeData.IsError, ERRID.ERR_NoNonObsoleteConstructorOnBase3, ERRID.WRN_NoNonObsoleteConstructorOnBase3), containingType.Locations(0), New [Object]() { containingType, methodSymbol1, containingType.BaseTypeNoUseSiteDiagnostics })
								End If
							ElseIf (Not [String].IsNullOrEmpty(obsoleteAttributeData.Message)) Then
								diagnostics.Add(If(obsoleteAttributeData.IsError, ERRID.ERR_RequiredNonObsoleteNewCall4, ERRID.WRN_RequiredNonObsoleteNewCall4), constructor.Locations(0), New [Object]() { methodSymbol1, containingType.BaseTypeNoUseSiteDiagnostics, containingType, obsoleteAttributeData.Message })
							Else
								diagnostics.Add(If(obsoleteAttributeData.IsError, ERRID.ERR_RequiredNonObsoleteNewCall3, ERRID.WRN_RequiredNonObsoleteNewCall3), constructor.Locations(0), New [Object]() { methodSymbol1, containingType.BaseTypeNoUseSiteDiagnostics, containingType })
							End If
						End If
					End If
					methodSymbol = methodSymbol1
				End If
			Else
				methodSymbol = constructor.ContainingAssembly.GetSpecialType(SpecialType.System_Object).InstanceConstructors.[Single]()
			End If
			Return methodSymbol
		End Function

		Friend Shared Function GenerateMethodBody(ByVal moduleBuilder As PEModuleBuilder, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal block As BoundStatement, ByVal lambdaDebugInfo As ImmutableArray(Of Microsoft.CodeAnalysis.CodeGen.LambdaDebugInfo), ByVal closureDebugInfo As ImmutableArray(Of Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo), ByVal stateMachineTypeOpt As StateMachineTypeSymbol, ByVal variableSlotAllocatorOpt As VariableSlotAllocator, ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal emittingPdb As Boolean, ByVal emitTestCoverageData As Boolean, ByVal dynamicAnalysisSpans As ImmutableArray(Of SourceSpan)) As Microsoft.CodeAnalysis.CodeGen.MethodBody
			Dim methodBody As Microsoft.CodeAnalysis.CodeGen.MethodBody
			Dim isAsync As Boolean
			Dim kickoffMethod As MethodSymbol
			Dim nullable As Nullable(Of Microsoft.CodeAnalysis.CodeGen.DebugId)
			Dim importScope As IImportScope
			Dim methodId As Nullable(Of Microsoft.CodeAnalysis.CodeGen.DebugId)
			Dim debugId As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim name As String
			Dim compilation As !0 = moduleBuilder.Compilation
			Dim localSlotManager As Microsoft.CodeAnalysis.CodeGen.LocalSlotManager = New Microsoft.CodeAnalysis.CodeGen.LocalSlotManager(variableSlotAllocatorOpt)
			Dim optimizationLevel As Microsoft.CodeAnalysis.OptimizationLevel = compilation.Options.OptimizationLevel
			If (method.IsEmbedded) Then
				optimizationLevel = Microsoft.CodeAnalysis.OptimizationLevel.Release
			End If
			Dim lBuilder As ILBuilder = New ILBuilder(moduleBuilder, localSlotManager, optimizationLevel, True)
			Try
				Dim iteratorMoveNextBodyDebugInfo As StateMachineMoveNextBodyDebugInfo = Nothing
				Dim codeGenerator As Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator = New Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator(method, block, lBuilder, moduleBuilder, diagnostics.DiagnosticBag, optimizationLevel, emittingPdb)
				If (Not diagnostics.HasAnyErrors()) Then
					Dim synthesizedStateMachineMethod As Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineMethod = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineMethod)
					If (synthesizedStateMachineMethod Is Nothing OrElse EmbeddedOperators.CompareString(method.Name, "MoveNext", False) <> 0) Then
						kickoffMethod = Nothing
						isAsync = False
					Else
						kickoffMethod = synthesizedStateMachineMethod.StateMachineType.KickoffMethod
						isAsync = kickoffMethod.IsAsync
						kickoffMethod = If(kickoffMethod.PartialDefinitionPart, kickoffMethod)
					End If
					If (Not isAsync) Then
						codeGenerator.Generate()
						If (kickoffMethod IsNot Nothing) Then
							iteratorMoveNextBodyDebugInfo = New Microsoft.CodeAnalysis.Emit.IteratorMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter())
						End If
					Else
						Dim num As Integer = -1
						Dim nums As ImmutableArray(Of Integer) = New ImmutableArray(Of Integer)()
						Dim nums1 As ImmutableArray(Of Integer) = New ImmutableArray(Of Integer)()
						codeGenerator.Generate(num, nums, nums1)
						iteratorMoveNextBodyDebugInfo = New AsyncMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter(), If(kickoffMethod.IsSub, num, -1), nums, nums1)
					End If
					Dim stateMachineHoistedLocalScopes As ImmutableArray(Of StateMachineHoistedLocalScope) = If(kickoffMethod Is Nothing OrElse moduleBuilder.DebugInformationFormat = DebugInformationFormat.Pdb, New ImmutableArray(Of StateMachineHoistedLocalScope)(), lBuilder.GetHoistedLocalScopes())
					If (method.Syntax IsNot Nothing) Then
						Dim sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile = moduleBuilder.SourceModule.TryGetSourceFile(method.Syntax.SyntaxTree)
						If (sourceFile IsNot Nothing) Then
							importScope = sourceFile.Translate(moduleBuilder, diagnostics.DiagnosticBag)
						Else
							importScope = Nothing
						End If
					Else
						importScope = Nothing
					End If
					Dim importScope1 As IImportScope = importScope
					If (Not diagnostics.HasAnyErrors()) Then
						If (moduleBuilder.SaveTestData) Then
							moduleBuilder.SetMethodTestData(method, lBuilder.GetSnapshot())
						End If
						Dim encHoistedLocalInfos As ImmutableArray(Of EncHoistedLocalInfo) = New ImmutableArray(Of EncHoistedLocalInfo)()
						Dim typeReferences As ImmutableArray(Of ITypeReference) = New ImmutableArray(Of ITypeReference)()
						If (optimizationLevel = Microsoft.CodeAnalysis.OptimizationLevel.Debug AndAlso stateMachineTypeOpt IsNot Nothing) Then
							MethodCompiler.GetStateMachineSlotDebugInfo(moduleBuilder, moduleBuilder.GetSynthesizedFields(stateMachineTypeOpt), variableSlotAllocatorOpt, diagnostics.DiagnosticBag, encHoistedLocalInfos, typeReferences)
						End If
						Dim allScopes As ImmutableArray(Of LocalScope) = lBuilder.GetAllScopes()
						Dim dynamicAnalysisMethodBodyDatum As DynamicAnalysisMethodBodyData = Nothing
						If (emitTestCoverageData) Then
							dynamicAnalysisMethodBodyDatum = New DynamicAnalysisMethodBodyData(dynamicAnalysisSpans)
						End If
						Dim realizedIL As ImmutableArray(Of Byte) = lBuilder.RealizedIL
						Dim maxStack As UShort = lBuilder.MaxStack
						Dim cciAdapter As MethodSymbol = (If(method.PartialDefinitionPart, method)).GetCciAdapter()
						If (variableSlotAllocatorOpt IsNot Nothing) Then
							methodId = variableSlotAllocatorOpt.MethodId
						Else
							nullable = Nothing
							methodId = nullable
						End If
						Dim nullable1 As Nullable(Of Microsoft.CodeAnalysis.CodeGen.DebugId) = methodId
						nullable = methodId
						debugId = If(nullable.HasValue, nullable1.GetValueOrDefault(), New Microsoft.CodeAnalysis.CodeGen.DebugId(methodOrdinal, moduleBuilder.CurrentGenerationOrdinal))
						Dim localDefinitions As ImmutableArray(Of ILocalDefinition) = lBuilder.LocalSlotManager.LocalsInOrder()
						Dim realizedSequencePoints As SequencePointList = lBuilder.RealizedSequencePoints
						Dim debugDocumentProvider1 As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider = debugDocumentProvider
						Dim realizedExceptionHandlers As ImmutableArray(Of ExceptionHandlerRegion) = lBuilder.RealizedExceptionHandlers
						Dim localScopes As ImmutableArray(Of LocalScope) = allScopes
						Dim importScope2 As IImportScope = importScope1
						Dim lambdaDebugInfos As ImmutableArray(Of Microsoft.CodeAnalysis.CodeGen.LambdaDebugInfo) = lambdaDebugInfo
						Dim closureDebugInfos As ImmutableArray(Of Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo) = closureDebugInfo
						If (stateMachineTypeOpt IsNot Nothing) Then
							name = stateMachineTypeOpt.Name
						Else
							name = Nothing
						End If
						methodBody = New Microsoft.CodeAnalysis.CodeGen.MethodBody(realizedIL, maxStack, cciAdapter, debugId, localDefinitions, realizedSequencePoints, debugDocumentProvider1, realizedExceptionHandlers, True, False, localScopes, False, importScope2, lambdaDebugInfos, closureDebugInfos, name, stateMachineHoistedLocalScopes, encHoistedLocalInfos, typeReferences, iteratorMoveNextBodyDebugInfo, dynamicAnalysisMethodBodyDatum)
					Else
						methodBody = Nothing
					End If
				Else
					methodBody = Nothing
				End If
			Finally
				lBuilder.FreeBasicBlocks()
			End Try
			Return methodBody
		End Function

		Public Shared Sub GetCompileDiagnostics(ByVal compilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation, ByVal root As NamespaceSymbol, ByVal tree As SyntaxTree, ByVal filterSpanWithinTree As Nullable(Of TextSpan), ByVal hasDeclarationErrors As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal doLoweringPhase As Boolean, ByVal cancellationToken As System.Threading.CancellationToken)
			Dim pEModuleBuilder As Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder
			Dim predicate As Predicate(Of Symbol) = Nothing
			If (tree IsNot Nothing) Then
				predicate = Function(sym As Symbol) Microsoft.CodeAnalysis.VisualBasic.MethodCompiler.IsDefinedOrImplementedInSourceTree(sym, tree, filterSpanWithinTree)
			End If
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compilation
			If (doLoweringPhase) Then
				pEModuleBuilder = DirectCast(compilation.CreateModuleBuilder(EmitOptions.[Default], Nothing, Nothing, Nothing, Nothing, Nothing, diagnostics.DiagnosticBag, cancellationToken), Microsoft.CodeAnalysis.VisualBasic.Emit.PEModuleBuilder)
			Else
				pEModuleBuilder = Nothing
			End If
			Dim methodCompiler As Microsoft.CodeAnalysis.VisualBasic.MethodCompiler = New Microsoft.CodeAnalysis.VisualBasic.MethodCompiler(visualBasicCompilation, pEModuleBuilder, False, False, doLoweringPhase, False, hasDeclarationErrors, diagnostics, predicate, cancellationToken)
			root.Accept(methodCompiler)
			If (tree Is Nothing) Then
				Dim entryPointAndDiagnostics As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation.EntryPoint = compilation.GetEntryPointAndDiagnostics(cancellationToken)
				If (entryPointAndDiagnostics IsNot Nothing) Then
					diagnostics.AddRange(Of Diagnostic)(entryPointAndDiagnostics.Diagnostics)
				End If
			End If
			methodCompiler.WaitForWorkers()
		End Sub

		Private Shared Function GetDesignerInitializeComponentMethod(ByVal sourceTypeSymbol As SourceMemberContainerTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			If (sourceTypeSymbol.TypeKind = TypeKind.[Class] AndAlso sourceTypeSymbol.GetAttributes().IndexOfAttribute(sourceTypeSymbol, AttributeDescription.DesignerGeneratedAttribute) > -1) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = sourceTypeSymbol.GetMembers("InitializeComponent").GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (Not methodSymbol1.IsSub OrElse methodSymbol1.IsShared OrElse methodSymbol1.IsGenericMethod OrElse methodSymbol1.ParameterCount <> 0) Then
						Continue While
					End If
					methodSymbol = methodSymbol1
					Return methodSymbol
				End While
			End If
			methodSymbol = Nothing
			Return methodSymbol
		End Function

		Private Shared Function GetEntryPoint(ByVal compilation As VisualBasicCompilation, ByVal moduleBeingBuilt As PEModuleBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal cancellationToken As System.Threading.CancellationToken) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim entryPointAndDiagnostics As VisualBasicCompilation.EntryPoint = compilation.GetEntryPointAndDiagnostics(cancellationToken)
			If (entryPointAndDiagnostics IsNot Nothing) Then
				diagnostics.AddRange(Of Diagnostic)(entryPointAndDiagnostics.Diagnostics)
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = entryPointAndDiagnostics.MethodSymbol
				Dim synthesizedEntryPointSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedEntryPointSymbol = TryCast(methodSymbol1, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedEntryPointSymbol)
				If (synthesizedEntryPointSymbol IsNot Nothing AndAlso moduleBeingBuilt IsNot Nothing AndAlso Not diagnostics.HasAnyErrors()) Then
					Dim typeCompilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState = New Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState(compilation, moduleBeingBuilt, Nothing)
					Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = synthesizedEntryPointSymbol.CreateBody()
					Dim methodBody As Microsoft.CodeAnalysis.CodeGen.MethodBody = MethodCompiler.GenerateMethodBody(moduleBeingBuilt, synthesizedEntryPointSymbol, -1, boundBlock, ImmutableArray(Of LambdaDebugInfo).Empty, ImmutableArray(Of ClosureDebugInfo).Empty, Nothing, Nothing, Nothing, diagnostics, False, False, ImmutableArray(Of SourceSpan).Empty)
					moduleBeingBuilt.SetMethodBody(synthesizedEntryPointSymbol, methodBody)
				End If
				methodSymbol = methodSymbol1
			Else
				methodSymbol = Nothing
			End If
			Return methodSymbol
		End Function

		Private Shared Sub GetStateMachineSlotDebugInfo(ByVal moduleBuilder As PEModuleBuilder, ByVal fieldDefs As IEnumerable(Of IFieldDefinition), ByVal variableSlotAllocatorOpt As VariableSlotAllocator, ByVal diagnostics As DiagnosticBag, ByRef hoistedVariableSlots As ImmutableArray(Of EncHoistedLocalInfo), ByRef awaiterSlots As ImmutableArray(Of ITypeReference))
			Dim enumerator As IEnumerator(Of IFieldDefinition) = Nothing
			Dim instance As ArrayBuilder(Of EncHoistedLocalInfo) = ArrayBuilder(Of EncHoistedLocalInfo).GetInstance()
			Using typeReferences As ArrayBuilder(Of ITypeReference) = ArrayBuilder(Of ITypeReference).GetInstance()
				enumerator = fieldDefs.GetEnumerator()
				While enumerator.MoveNext()
					Dim internalSymbol As StateMachineFieldSymbol = DirectCast(enumerator.Current.GetInternalSymbol(), StateMachineFieldSymbol)
					Dim slotIndex As Integer = internalSymbol.SlotIndex
					If (internalSymbol.SlotDebugInfo.SynthesizedKind <> SynthesizedLocalKind.AwaiterField) Then
						If (internalSymbol.SlotDebugInfo.Id.IsNone) Then
							Continue While
						End If
						While slotIndex >= instance.Count
							instance.Add(New EncHoistedLocalInfo())
						End While
						instance(slotIndex) = New EncHoistedLocalInfo(internalSymbol.SlotDebugInfo, moduleBuilder.EncTranslateLocalVariableType(internalSymbol.Type, diagnostics))
					Else
						While slotIndex >= typeReferences.Count
							typeReferences.Add(Nothing)
						End While
						typeReferences(slotIndex) = moduleBuilder.EncTranslateLocalVariableType(internalSymbol.Type, diagnostics)
					End If
				End While
			End Using
			If (variableSlotAllocatorOpt IsNot Nothing) Then
				Dim previousAwaiterSlotCount As Integer = variableSlotAllocatorOpt.PreviousAwaiterSlotCount
				While typeReferences.Count < previousAwaiterSlotCount
					typeReferences.Add(Nothing)
				End While
				Dim previousHoistedLocalSlotCount As Integer = variableSlotAllocatorOpt.PreviousHoistedLocalSlotCount
				While instance.Count < previousHoistedLocalSlotCount
					instance.Add(New EncHoistedLocalInfo(True))
				End While
			End If
			hoistedVariableSlots = instance.ToImmutableAndFree()
			awaiterSlots = typeReferences.ToImmutableAndFree()
		End Sub

		Private Shared Function IsDefinedOrImplementedInSourceTree(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal tree As SyntaxTree, ByVal span As Nullable(Of TextSpan)) As Boolean
			Dim flag As Boolean
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			If (Not symbol.IsDefinedInSourceTree(tree, span, cancellationToken)) Then
				Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
				If (sourceMemberMethodSymbol IsNot Nothing AndAlso sourceMemberMethodSymbol.IsPartialDefinition) Then
					Dim partialImplementationPart As MethodSymbol = sourceMemberMethodSymbol.PartialImplementationPart
					If (partialImplementationPart Is Nothing) Then
						GoTo Label1
					End If
					cancellationToken = New System.Threading.CancellationToken()
					flag = partialImplementationPart.IsDefinedInSourceTree(tree, span, cancellationToken)
					Return flag
				End If
			Label1:
				flag = If(symbol.Kind <> SymbolKind.Method OrElse Not symbol.IsImplicitlyDeclared OrElse DirectCast(symbol, MethodSymbol).MethodKind <> MethodKind.Constructor, False, MethodCompiler.IsDefinedOrImplementedInSourceTree(symbol.ContainingType, tree, span))
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Sub LowerAndEmitMethod(ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal binderOpt As Binder, ByVal compilationState As TypeCompilationState, ByVal diagsForCurrentMethod As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal processedInitializers As Binder.ProcessedFieldOrPropertyInitializers, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal constructorToInject As MethodSymbol, ByRef delegateRelaxationIdDispenser As Integer)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim empty As ImmutableArray(Of BoundStatement)
			Dim flag As Boolean
			If (constructorToInject Is Nothing) Then
				boundExpressionStatement = Nothing
			Else
				boundExpressionStatement = MethodCompiler.BindDefaultConstructorInitializer(method, constructorToInject, diagsForCurrentMethod, binderOpt)
			End If
			Dim boundExpressionStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = boundExpressionStatement
			If (Not diagsForCurrentMethod.HasAnyErrors() AndAlso (boundExpressionStatement1 Is Nothing OrElse Not boundExpressionStatement1.HasErrors)) Then
				If (method.MethodKind = MethodKind.Constructor OrElse method.MethodKind = MethodKind.StaticConstructor) Then
					boundBlock = If(Not method.IsScriptConstructor, InitializerRewriter.BuildConstructorBody(compilationState, method, boundExpressionStatement1, processedInitializers, block), block)
				Else
					boundBlock = If(Not method.IsScriptInitializer, block, InitializerRewriter.BuildScriptInitializerBody(DirectCast(method, SynthesizedInteractiveInitializerMethod), processedInitializers, block))
				End If
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagsForCurrentMethod
				If (method.IsImplicitlyDeclared AndAlso method.AssociatedSymbol IsNot Nothing AndAlso method.AssociatedSymbol.IsMyGroupCollectionProperty) Then
					instance = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(diagsForCurrentMethod)
				End If
				Dim variableSlotAllocator As Microsoft.CodeAnalysis.CodeGen.VariableSlotAllocator = Nothing
				Dim stateMachineTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol = Nothing
				Dim flag1 As Boolean = If(Me._moduleBeingBuiltOpt Is Nothing, True, Me._moduleBeingBuiltOpt.AllowOmissionOfConditionalCalls)
				Dim lambdaDebugInfos As ArrayBuilder(Of LambdaDebugInfo) = ArrayBuilder(Of LambdaDebugInfo).GetInstance()
				Dim closureDebugInfos As ArrayBuilder(Of ClosureDebugInfo) = ArrayBuilder(Of ClosureDebugInfo).GetInstance()
				Dim sourceSpans As ImmutableArray(Of SourceSpan) = ImmutableArray(Of SourceSpan).Empty
				boundBlock = Rewriter.LowerBodyOrInitializer(method, methodOrdinal, boundBlock, previousSubmissionFields, compilationState, Me._emitTestCoverageData, sourceSpans, Me._debugDocumentProvider, instance, variableSlotAllocator, lambdaDebugInfos, closureDebugInfos, delegateRelaxationIdDispenser, stateMachineTypeSymbol, flag1, False)
				If (method.IsSubmissionConstructor) Then
					empty = SynthesizedSubmissionConstructorSymbol.MakeSubmissionInitialization(block.Syntax, method, previousSubmissionFields, Me._compilation)
				Else
					empty = ImmutableArray(Of BoundStatement).Empty
				End If
				Dim boundStatements As ImmutableArray(Of BoundStatement) = empty
				If (boundBlock.HasErrors OrElse diagsForCurrentMethod.HasAnyErrors()) Then
					flag = True
				Else
					flag = If(instance = diagsForCurrentMethod, False, instance.HasAnyErrors())
				End If
				Dim flag2 As Boolean = flag
				Me.SetGlobalErrorIfTrue(flag2)
				If (Me._moduleBeingBuiltOpt IsNot Nothing AndAlso Not flag2) Then
					If (method.IsScriptConstructor) Then
						Dim instance1 As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
						instance1.Add(boundExpressionStatement1)
						instance1.AddRange(boundStatements)
						instance1.Add(boundBlock)
						Dim syntax As SyntaxNode = boundBlock.Syntax
						Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
						boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, instance1.ToImmutableAndFree(), boundBlock.HasErrors)).MakeCompilerGenerated()
					End If
					If (Me.DoEmitPhase) Then
						Dim methodBody As Microsoft.CodeAnalysis.CodeGen.MethodBody = MethodCompiler.GenerateMethodBody(Me._moduleBeingBuiltOpt, method, methodOrdinal, boundBlock, lambdaDebugInfos.ToImmutable(), closureDebugInfos.ToImmutable(), stateMachineTypeSymbol, variableSlotAllocator, Me._debugDocumentProvider, instance, Me._emittingPdb, Me._emitTestCoverageData, sourceSpans)
						Me._moduleBeingBuiltOpt.SetMethodBody(If(method.PartialDefinitionPart, method), methodBody)
					End If
					If (instance <> diagsForCurrentMethod) Then
						DirectCast(method.AssociatedSymbol, SynthesizedMyGroupCollectionPropertySymbol).RelocateDiagnostics(instance.DiagnosticBag, diagsForCurrentMethod.DiagnosticBag)
						diagsForCurrentMethod.AddDependencies(instance, False)
						instance.Free()
					End If
					lambdaDebugInfos.Free()
					closureDebugInfos.Free()
				ElseIf (instance <> diagsForCurrentMethod) Then
					DirectCast(method.AssociatedSymbol, SynthesizedMyGroupCollectionPropertySymbol).RelocateDiagnostics(instance.DiagnosticBag, diagsForCurrentMethod.DiagnosticBag)
					diagsForCurrentMethod.AddDependencies(instance, False)
					instance.Free()
					Return
				End If
			End If
		End Sub

		Private Shared Function PassesFilter(ByVal filterOpt As Predicate(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (filterOpt Is Nothing) Then
				Return True
			End If
			Return filterOpt(symbol)
		End Function

		Private Sub ProcessEmbeddedMethods()
			Dim embeddedSymbolManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager = Me._compilation.EmbeddedSymbolManager
			Dim symbols As ConcurrentSet(Of Symbol) = New ConcurrentSet(Of Symbol)(ReferenceEqualityComparer.Instance)
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			While True
				embeddedSymbolManager.GetCurrentReferencedSymbolsSnapshot(instance, symbols)
				If (instance.Count = 0) Then
					Exit While
				End If
				Dim count As Integer = instance.Count - 1
				Dim num As Integer = 0
				Do
					Dim item As Symbol = instance(num)
					symbols.Add(item)
					If (item.Kind = SymbolKind.Method) Then
						Me.VisitEmbeddedMethod(DirectCast(item, MethodSymbol))
					End If
					num = num + 1
				Loop While num <= count
				instance.Clear()
			End While
			instance.Free()
			embeddedSymbolManager.SealCollection()
		End Sub

		Private Shared Sub ReportConstructorCycles(ByVal startsAt As Integer, ByVal endsAt As Integer, ByVal path As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.DiagnosticInfo) = ArrayBuilder(Of Microsoft.CodeAnalysis.DiagnosticInfo).GetInstance()
			Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = path(startsAt)
			Dim num As Integer = endsAt
			Dim num1 As Integer = startsAt + 1
			Do
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = path(num1)
				instance.Add(ErrorFactory.ErrorInfo(ERRID.ERR_SubNewCycle2, New [Object]() { item, methodSymbol }))
				item = methodSymbol
				num1 = num1 + 1
			Loop While num1 <= num
			Dim num2 As Integer = endsAt - 1
			Dim num3 As Integer = startsAt
			Do
				item = path(num3)
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_SubNewCycle1, New [Object]() { item, New CompoundDiagnosticInfo(instance.ToArray()) })
				Dim locations As ImmutableArray(Of Location) = item.Locations
				diagnostics.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
				If (instance.Count > 1) Then
					Dim item1 As Microsoft.CodeAnalysis.DiagnosticInfo = instance(0)
					instance.RemoveAt(0)
					instance.Add(item1)
				End If
				num3 = num3 + 1
			Loop While num3 <= num2
			instance.Free()
		End Sub

		Private Sub SetGlobalErrorIfTrue(ByVal arg As Boolean)
			If (arg) Then
				Me._globalHasErrors = True
			End If
		End Sub

		Private Shared Function TryGetMethodCalledInBoundExpressionStatement(ByVal stmt As BoundExpressionStatement) As MethodSymbol
			Dim method As MethodSymbol
			If (stmt Is Nothing OrElse stmt.HasErrors) Then
				method = Nothing
			Else
				Dim expression As BoundExpression = stmt.Expression
				If (expression.Kind = BoundKind.[Call]) Then
					method = DirectCast(expression, BoundCall).Method
				Else
					method = Nothing
				End If
			End If
			Return method
		End Function

		Private Sub VisitEmbeddedMethod(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim typeCompilationState As Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState = New Microsoft.CodeAnalysis.VisualBasic.TypeCompilationState(Me._compilation, Me._moduleBeingBuiltOpt, Nothing)
			If (method.MethodKind = MethodKind.Ordinary) Then
				binder = Nothing
			Else
				Dim containingModule As SourceModuleSymbol = DirectCast(method.ContainingModule, SourceModuleSymbol)
				Dim locations As ImmutableArray(Of Location) = method.ContainingType.Locations
				binder = BinderBuilder.CreateBinderForType(containingModule, locations(0).PossiblyEmbeddedOrMySourceTree(), method.ContainingType)
			End If
			Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder = binder
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Me.CompileMethod(method, -1, num, num1, Nothing, typeCompilationState, Microsoft.CodeAnalysis.VisualBasic.Binder.ProcessedFieldOrPropertyInitializers.Empty, binder1, Nothing, methodSymbol)
		End Sub

		Public Overrides Sub VisitNamedType(ByVal symbol As NamedTypeSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (MethodCompiler.PassesFilter(Me._filterOpt, symbol)) Then
				If (Me._compilation.Options.ConcurrentBuild) Then
					Dim task As System.Threading.Tasks.Task = Me.CompileNamedTypeAsync(symbol, Me._filterOpt)
					Me._compilerTasks.Push(task)
					Return
				End If
				Me.CompileNamedType(symbol, Me._filterOpt)
			End If
		End Sub

		Public Overrides Sub VisitNamespace(ByVal symbol As NamespaceSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			If (Not Me._compilation.Options.ConcurrentBuild) Then
				Me.CompileNamespace(symbol)
				Return
			End If
			Dim task As System.Threading.Tasks.Task = Me.CompileNamespaceAsync(symbol)
			Me._compilerTasks.Push(task)
		End Sub

		Private Sub WaitForWorkers()
			Dim tasks As ConcurrentStack(Of System.Threading.Tasks.Task) = Me._compilerTasks
			If (tasks IsNot Nothing) Then
				Dim task As System.Threading.Tasks.Task = Nothing
				While tasks.TryPop(task)
					task.GetAwaiter().GetResult()
				End While
			End If
		End Sub

		Private NotInheritable Class InitializeComponentCallTreeBuilder
			Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private _calledMethods As HashSet(Of MethodSymbol)

			Private ReadOnly _containingType As NamedTypeSymbol

			Private Sub New(ByVal containingType As NamedTypeSymbol)
				MyBase.New()
				Me._containingType = containingType
			End Sub

			Public Shared Sub CollectCallees(ByVal compilationState As TypeCompilationState, ByVal method As MethodSymbol, ByVal block As BoundBlock)
				Dim initializeComponentCallTreeBuilder As MethodCompiler.InitializeComponentCallTreeBuilder = New MethodCompiler.InitializeComponentCallTreeBuilder(method.ContainingType)
				initializeComponentCallTreeBuilder.VisitBlock(block)
				If (initializeComponentCallTreeBuilder._calledMethods IsNot Nothing) Then
					compilationState.AddToInitializeComponentCallTree(method, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of MethodSymbol)(initializeComponentCallTreeBuilder._calledMethods.ToArray()))
				End If
			End Sub

			Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
				If (node.ReceiverOpt IsNot Nothing AndAlso (node.ReceiverOpt.Kind = BoundKind.MeReference OrElse node.ReceiverOpt.Kind = BoundKind.MyClassReference) AndAlso Not node.Method.IsShared AndAlso CObj(node.Method.OriginalDefinition.ContainingType) = CObj(Me._containingType)) Then
					If (Me._calledMethods Is Nothing) Then
						Me._calledMethods = New HashSet(Of MethodSymbol)(ReferenceEqualityComparer.Instance)
					End If
					Me._calledMethods.Add(node.Method.OriginalDefinition)
				End If
				Return MyBase.VisitCall(node)
			End Function
		End Class
	End Class
End Namespace