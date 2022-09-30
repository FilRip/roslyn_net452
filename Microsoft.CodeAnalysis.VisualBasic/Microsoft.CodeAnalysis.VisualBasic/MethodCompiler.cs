using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.CodeGen;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MethodCompiler : VisualBasicSymbolVisitor
	{
		private sealed class InitializeComponentCallTreeBuilder : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private HashSet<MethodSymbol> _calledMethods;

			private readonly NamedTypeSymbol _containingType;

			private InitializeComponentCallTreeBuilder(NamedTypeSymbol containingType)
			{
				_containingType = containingType;
			}

			public static void CollectCallees(TypeCompilationState compilationState, MethodSymbol method, BoundBlock block)
			{
				InitializeComponentCallTreeBuilder initializeComponentCallTreeBuilder = new InitializeComponentCallTreeBuilder(method.ContainingType);
				initializeComponentCallTreeBuilder.VisitBlock(block);
				if (initializeComponentCallTreeBuilder._calledMethods != null)
				{
					compilationState.AddToInitializeComponentCallTree(method, initializeComponentCallTreeBuilder._calledMethods.ToArray().AsImmutableOrNull());
				}
			}

			public override BoundNode VisitCall(BoundCall node)
			{
				if (node.ReceiverOpt != null && (node.ReceiverOpt.Kind == BoundKind.MeReference || node.ReceiverOpt.Kind == BoundKind.MyClassReference) && !node.Method.IsShared && (object)node.Method.OriginalDefinition.ContainingType == _containingType)
				{
					if (_calledMethods == null)
					{
						_calledMethods = new HashSet<MethodSymbol>(ReferenceEqualityComparer.Instance);
					}
					_calledMethods.Add(node.Method.OriginalDefinition);
				}
				return base.VisitCall(node);
			}
		}

		private readonly VisualBasicCompilation _compilation;

		private readonly CancellationToken _cancellationToken;

		private readonly bool _emittingPdb;

		private readonly bool _emitTestCoverageData;

		private readonly BindingDiagnosticBag _diagnostics;

		private readonly bool _hasDeclarationErrors;

		private readonly PEModuleBuilder _moduleBeingBuiltOpt;

		private readonly Predicate<Symbol> _filterOpt;

		private readonly DebugDocumentProvider _debugDocumentProvider;

		private readonly bool _doEmitPhase;

		private readonly bool _doLoweringPhase;

		private readonly ConcurrentStack<Task> _compilerTasks;

		private bool _globalHasErrors;

		private bool GlobalHasErrors => _globalHasErrors;

		private bool DoEmitPhase => _doEmitPhase;

		private bool DoLoweringPhase => _doLoweringPhase;

		private void SetGlobalErrorIfTrue(bool arg)
		{
			if (arg)
			{
				_globalHasErrors = true;
			}
		}

		private MethodCompiler(VisualBasicCompilation compilation, PEModuleBuilder moduleBeingBuiltOpt, bool emittingPdb, bool emitTestCoverageData, bool doLoweringPhase, bool doEmitPhase, bool hasDeclarationErrors, BindingDiagnosticBag diagnostics, Predicate<Symbol> filter, CancellationToken cancellationToken)
		{
			_compilation = compilation;
			_moduleBeingBuiltOpt = moduleBeingBuiltOpt;
			_diagnostics = diagnostics;
			_hasDeclarationErrors = hasDeclarationErrors;
			_cancellationToken = cancellationToken;
			_doLoweringPhase = doEmitPhase || doLoweringPhase;
			_doEmitPhase = doEmitPhase;
			_emittingPdb = emittingPdb;
			_emitTestCoverageData = emitTestCoverageData;
			_filterOpt = filter;
			if (emittingPdb || emitTestCoverageData)
			{
				_debugDocumentProvider = (string path, string basePath) => moduleBeingBuiltOpt.DebugDocumentsBuilder.GetOrAddDebugDocument(path, basePath, CreateDebugDocumentForFile);
			}
			if (compilation.Options.ConcurrentBuild)
			{
				_compilerTasks = new ConcurrentStack<Task>();
			}
		}

		private static bool IsDefinedOrImplementedInSourceTree(Symbol symbol, SyntaxTree tree, TextSpan? span)
		{
			if (symbol.IsDefinedInSourceTree(tree, span))
			{
				return true;
			}
			if (symbol is SourceMemberMethodSymbol sourceMemberMethodSymbol && sourceMemberMethodSymbol.IsPartialDefinition)
			{
				MethodSymbol partialImplementationPart = sourceMemberMethodSymbol.PartialImplementationPart;
				if ((object)partialImplementationPart != null)
				{
					return partialImplementationPart.IsDefinedInSourceTree(tree, span);
				}
			}
			if (symbol.Kind == SymbolKind.Method && symbol.IsImplicitlyDeclared && ((MethodSymbol)symbol).MethodKind == MethodKind.Constructor)
			{
				return IsDefinedOrImplementedInSourceTree(symbol.ContainingType, tree, span);
			}
			return false;
		}

		public static void GetCompileDiagnostics(VisualBasicCompilation compilation, NamespaceSymbol root, SyntaxTree tree, TextSpan? filterSpanWithinTree, bool hasDeclarationErrors, BindingDiagnosticBag diagnostics, bool doLoweringPhase, CancellationToken cancellationToken)
		{
			Predicate<Symbol> filter = null;
			if (tree != null)
			{
				filter = (Symbol sym) => IsDefinedOrImplementedInSourceTree(sym, tree, filterSpanWithinTree);
			}
			MethodCompiler methodCompiler = new MethodCompiler(compilation, doLoweringPhase ? ((PEModuleBuilder)compilation.CreateModuleBuilder(EmitOptions.Default, null, null, null, null, null, diagnostics.DiagnosticBag, cancellationToken)) : null, emittingPdb: false, emitTestCoverageData: false, doLoweringPhase, doEmitPhase: false, hasDeclarationErrors, diagnostics, filter, cancellationToken);
			root.Accept(methodCompiler);
			if (tree == null)
			{
				VisualBasicCompilation.EntryPoint entryPointAndDiagnostics = compilation.GetEntryPointAndDiagnostics(cancellationToken);
				if (entryPointAndDiagnostics != null)
				{
					diagnostics.AddRange(entryPointAndDiagnostics.Diagnostics);
				}
			}
			methodCompiler.WaitForWorkers();
		}

		internal static void CompileMethodBodies(VisualBasicCompilation compilation, PEModuleBuilder moduleBeingBuiltOpt, bool emittingPdb, bool emitTestCoverageData, bool hasDeclarationErrors, Predicate<Symbol> filter, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (compilation.PreviousSubmission != null)
			{
				compilation.PreviousSubmission.EnsureAnonymousTypeTemplates(cancellationToken);
			}
			MethodCompiler methodCompiler = new MethodCompiler(compilation, moduleBeingBuiltOpt, emittingPdb, emitTestCoverageData, doLoweringPhase: true, doEmitPhase: true, hasDeclarationErrors, diagnostics, filter, cancellationToken);
			compilation.SourceModule.GlobalNamespace.Accept(methodCompiler);
			methodCompiler.WaitForWorkers();
			if (moduleBeingBuiltOpt != null)
			{
				ImmutableArray<NamedTypeSymbol> additionalTopLevelTypes = moduleBeingBuiltOpt.GetAdditionalTopLevelTypes();
				if (!additionalTopLevelTypes.IsEmpty)
				{
					methodCompiler.CompileSynthesizedMethods(additionalTopLevelTypes);
				}
				compilation.AnonymousTypeManager.AssignTemplatesNamesAndCompile(methodCompiler, moduleBeingBuiltOpt, diagnostics);
				methodCompiler.WaitForWorkers();
				if (compilation.EmbeddedSymbolManager.Embedded != 0)
				{
					methodCompiler.ProcessEmbeddedMethods();
				}
				PrivateImplementationDetails privateImplClass = moduleBeingBuiltOpt.PrivateImplClass;
				if (privateImplClass != null)
				{
					privateImplClass.Freeze();
					methodCompiler.CompileSynthesizedMethods(privateImplClass);
				}
			}
			MethodSymbol entryPoint = GetEntryPoint(compilation, moduleBeingBuiltOpt, diagnostics, cancellationToken);
			if (moduleBeingBuiltOpt != null)
			{
				if ((object)entryPoint != null && compilation.Options.OutputKind.IsApplication())
				{
					moduleBeingBuiltOpt.SetPEEntryPoint(entryPoint, diagnostics.DiagnosticBag);
				}
				if ((methodCompiler.GlobalHasErrors || moduleBeingBuiltOpt.SourceModule.HasBadAttributes) && !hasDeclarationErrors && !diagnostics.HasAnyErrors())
				{
					string nameOfLocalizableResource = (methodCompiler.GlobalHasErrors ? "UnableToDetermineSpecificCauseOfFailure" : "ModuleHasInvalidAttributes");
					diagnostics.Add(ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, moduleBeingBuiltOpt.SourceModule.Name, new LocalizableResourceString(nameOfLocalizableResource, CodeAnalysisResources.ResourceManager, typeof(CodeAnalysisResources)));
				}
			}
		}

		private static MethodSymbol GetEntryPoint(VisualBasicCompilation compilation, PEModuleBuilder moduleBeingBuilt, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			VisualBasicCompilation.EntryPoint entryPointAndDiagnostics = compilation.GetEntryPointAndDiagnostics(cancellationToken);
			if (entryPointAndDiagnostics == null)
			{
				return null;
			}
			diagnostics.AddRange(entryPointAndDiagnostics.Diagnostics);
			MethodSymbol methodSymbol = entryPointAndDiagnostics.MethodSymbol;
			if (methodSymbol is SynthesizedEntryPointSymbol synthesizedEntryPointSymbol && moduleBeingBuilt != null && !diagnostics.HasAnyErrors())
			{
				new TypeCompilationState(compilation, moduleBeingBuilt, null);
				BoundBlock block = synthesizedEntryPointSymbol.CreateBody();
				MethodBody body = GenerateMethodBody(moduleBeingBuilt, synthesizedEntryPointSymbol, -1, block, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, null, null, null, diagnostics, emittingPdb: false, emitTestCoverageData: false, ImmutableArray<SourceSpan>.Empty);
				moduleBeingBuilt.SetMethodBody(synthesizedEntryPointSymbol, body);
			}
			return methodSymbol;
		}

		private void WaitForWorkers()
		{
			ConcurrentStack<Task> compilerTasks = _compilerTasks;
			if (compilerTasks != null)
			{
				Task result = null;
				while (compilerTasks.TryPop(out result))
				{
					result.GetAwaiter().GetResult();
				}
			}
		}

		private void ProcessEmbeddedMethods()
		{
			EmbeddedSymbolManager embeddedSymbolManager = _compilation.EmbeddedSymbolManager;
			ConcurrentSet<Symbol> concurrentSet = new ConcurrentSet<Symbol>(ReferenceEqualityComparer.Instance);
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			while (true)
			{
				embeddedSymbolManager.GetCurrentReferencedSymbolsSnapshot(instance, concurrentSet);
				if (instance.Count == 0)
				{
					break;
				}
				int num = instance.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					Symbol symbol = instance[i];
					concurrentSet.Add(symbol);
					if (symbol.Kind == SymbolKind.Method)
					{
						MethodSymbol method = (MethodSymbol)symbol;
						VisitEmbeddedMethod(method);
					}
				}
				instance.Clear();
			}
			instance.Free();
			embeddedSymbolManager.SealCollection();
		}

		private void VisitEmbeddedMethod(MethodSymbol method)
		{
			TypeCompilationState compilationState = new TypeCompilationState(_compilation, _moduleBeingBuiltOpt, null);
			Binder containingTypeBinder = ((method.MethodKind == MethodKind.Ordinary) ? null : BinderBuilder.CreateBinderForType((SourceModuleSymbol)method.ContainingModule, LocationExtensions.PossiblyEmbeddedOrMySourceTree(method.ContainingType.Locations[0]), method.ContainingType));
			int withEventPropertyIdDispenser = 0;
			int delegateRelaxationIdDispenser = 0;
			MethodSymbol referencedConstructor = null;
			CompileMethod(method, -1, ref withEventPropertyIdDispenser, ref delegateRelaxationIdDispenser, null, compilationState, Binder.ProcessedFieldOrPropertyInitializers.Empty, containingTypeBinder, null, ref referencedConstructor);
		}

		[Conditional("DEBUG")]
		private void AssertAllInitializersAreConstants(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers)
		{
			if (initializers.IsDefaultOrEmpty)
			{
				return;
			}
			ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<FieldOrPropertyInitializer> current = enumerator.Current;
				if (current.IsEmpty)
				{
					continue;
				}
				ImmutableArray<FieldOrPropertyInitializer>.Enumerator enumerator2 = current.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ImmutableArray<Symbol>.Enumerator enumerator3 = enumerator2.Current.FieldsOrProperties.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						_ = enumerator3.Current;
					}
				}
			}
		}

		public override void VisitNamespace(NamespaceSymbol symbol)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			if (_compilation.Options.ConcurrentBuild)
			{
				Task item = CompileNamespaceAsync(symbol);
				_compilerTasks.Push(item);
			}
			else
			{
				CompileNamespace(symbol);
			}
		}

		private Task CompileNamespaceAsync(NamespaceSymbol symbol)
		{
			return Task.Run(UICultureUtilities.WithCurrentUICulture(delegate
			{
				try
				{
					CompileNamespace(symbol);
				}
				catch (Exception ex) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					ProjectData.SetProjectError(ex);
					return FatalError.ReportAndPropagateUnlessCanceled(ex);
				}).Invoke())
				{
					throw ExceptionUtilities.Unreachable;
				}
			}), _cancellationToken);
		}

		private void CompileNamespace(NamespaceSymbol symbol)
		{
			if (PassesFilter(_filterOpt, symbol))
			{
				ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembersUnordered().GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Accept(this);
				}
			}
		}

		public override void VisitNamedType(NamedTypeSymbol symbol)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			if (PassesFilter(_filterOpt, symbol))
			{
				if (_compilation.Options.ConcurrentBuild)
				{
					Task item = CompileNamedTypeAsync(symbol, _filterOpt);
					_compilerTasks.Push(item);
				}
				else
				{
					CompileNamedType(symbol, _filterOpt);
				}
			}
		}

		private Task CompileNamedTypeAsync(NamedTypeSymbol symbol, Predicate<Symbol> filter)
		{
			return Task.Run(UICultureUtilities.WithCurrentUICulture(delegate
			{
				try
				{
					CompileNamedType(symbol, filter);
				}
				catch (Exception ex) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					ProjectData.SetProjectError(ex);
					return FatalError.ReportAndPropagateUnlessCanceled(ex);
				}).Invoke())
				{
					throw ExceptionUtilities.Unreachable;
				}
			}), _cancellationToken);
		}

		private void CompileNamedType(NamedTypeSymbol containingType, Predicate<Symbol> filter)
		{
			if (containingType.IsEmbedded)
			{
				return;
			}
			SynthesizedConstructorBase synthesizedConstructorBase = null;
			SynthesizedInteractiveInitializerMethod scriptInitializerOpt = null;
			SynthesizedEntryPointSymbol synthesizedEntryPointSymbol = null;
			int num = -1;
			if (containingType.IsScriptClass)
			{
				synthesizedConstructorBase = containingType.GetScriptConstructor();
				scriptInitializerOpt = containingType.GetScriptInitializer();
				synthesizedEntryPointSymbol = containingType.GetScriptEntryPoint();
			}
			Binder.ProcessedFieldOrPropertyInitializers processedFieldOrPropertyInitializers = Binder.ProcessedFieldOrPropertyInitializers.Empty;
			Binder.ProcessedFieldOrPropertyInitializers processedFieldOrPropertyInitializers2 = Binder.ProcessedFieldOrPropertyInitializers.Empty;
			SynthesizedSubmissionFields synthesizedSubmissionFields = (containingType.IsSubmissionClass ? new SynthesizedSubmissionFields(_compilation, containingType) : null);
			SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol = containingType as SourceMemberContainerTypeSymbol;
			MethodSymbol methodSymbol = null;
			if ((object)sourceMemberContainerTypeSymbol != null && DoLoweringPhase)
			{
				methodSymbol = GetDesignerInitializeComponentMethod(sourceMemberContainerTypeSymbol);
			}
			TypeCompilationState typeCompilationState = new TypeCompilationState(_compilation, _moduleBeingBuiltOpt, methodSymbol);
			Binder binder = null;
			if ((object)sourceMemberContainerTypeSymbol != null)
			{
				binder = BinderBuilder.CreateBinderForType((SourceModuleSymbol)sourceMemberContainerTypeSymbol.ContainingModule, LocationExtensions.PossiblyEmbeddedOrMySourceTree(sourceMemberContainerTypeSymbol.Locations[0]), sourceMemberContainerTypeSymbol);
				processedFieldOrPropertyInitializers = new Binder.ProcessedFieldOrPropertyInitializers(Binder.BindFieldAndPropertyInitializers(sourceMemberContainerTypeSymbol, sourceMemberContainerTypeSymbol.StaticInitializers, scriptInitializerOpt, _diagnostics));
				processedFieldOrPropertyInitializers2 = new Binder.ProcessedFieldOrPropertyInitializers(Binder.BindFieldAndPropertyInitializers(sourceMemberContainerTypeSymbol, sourceMemberContainerTypeSymbol.InstanceInitializers, scriptInitializerOpt, _diagnostics));
				MethodSymbol methodSymbol2 = sourceMemberContainerTypeSymbol.CreateSharedConstructorsForConstFieldsIfRequired(binder, _diagnostics);
				if ((object)methodSymbol2 != null && PassesFilter(filter, methodSymbol2))
				{
					int withEventPropertyIdDispenser = 0;
					int delegateRelaxationIdDispenser = 0;
					Binder.ProcessedFieldOrPropertyInitializers processedInitializers = processedFieldOrPropertyInitializers;
					Binder containingTypeBinder = binder;
					MethodSymbol referencedConstructor = null;
					CompileMethod(methodSymbol2, -1, ref withEventPropertyIdDispenser, ref delegateRelaxationIdDispenser, filter, typeCompilationState, processedInitializers, containingTypeBinder, synthesizedSubmissionFields, ref referencedConstructor);
					if (_moduleBeingBuiltOpt != null)
					{
						_moduleBeingBuiltOpt.AddSynthesizedDefinition(sourceMemberContainerTypeSymbol, methodSymbol2.GetCciAdapter());
					}
				}
			}
			Dictionary<MethodSymbol, MethodSymbol> dictionary = null;
			ImmutableArray<Symbol> members = containingType.GetMembers();
			int withEventPropertyIdDispenser2 = 0;
			int delegateRelaxationIdDispenser2 = 0;
			int num2 = members.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				Symbol symbol = members[i];
				if (!PassesFilter(filter, symbol))
				{
					continue;
				}
				switch (symbol.Kind)
				{
				case SymbolKind.NamedType:
					symbol.Accept(this);
					break;
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol3 = (MethodSymbol)symbol;
					if (methodSymbol3.IsScriptConstructor)
					{
						num = i;
					}
					else
					{
						if ((object)methodSymbol3 == synthesizedEntryPointSymbol)
						{
							break;
						}
						if (MethodSymbolExtensions.IsPartial(methodSymbol3))
						{
							MethodSymbol partialImplementationPart = methodSymbol3.PartialImplementationPart;
							if ((object)partialImplementationPart != methodSymbol3)
							{
								if (((SourceMethodSymbol)methodSymbol3).SetDiagnostics(ImmutableArray<Diagnostic>.Empty))
								{
									methodSymbol3.DeclaringCompilation.SymbolDeclaredEvent(methodSymbol3);
								}
								if ((object)partialImplementationPart == null)
								{
									break;
								}
								methodSymbol3 = partialImplementationPart;
							}
						}
						Binder.ProcessedFieldOrPropertyInitializers processedInitializers2 = Binder.ProcessedFieldOrPropertyInitializers.Empty;
						if (methodSymbol3.MethodKind == MethodKind.StaticConstructor)
						{
							processedInitializers2 = processedFieldOrPropertyInitializers;
						}
						else if (methodSymbol3.MethodKind == MethodKind.Constructor || methodSymbol3.IsScriptInitializer)
						{
							processedInitializers2 = processedFieldOrPropertyInitializers2;
						}
						MethodSymbol referencedConstructor2 = null;
						CompileMethod(methodSymbol3, i, ref withEventPropertyIdDispenser2, ref delegateRelaxationIdDispenser2, filter, typeCompilationState, processedInitializers2, binder, synthesizedSubmissionFields, ref referencedConstructor2);
						if ((object)referencedConstructor2 != null && referencedConstructor2.ContainingType.Equals(containingType))
						{
							if (dictionary == null)
							{
								dictionary = new Dictionary<MethodSymbol, MethodSymbol>();
							}
							dictionary.Add(methodSymbol3, referencedConstructor2);
						}
						if (DoLoweringPhase && _moduleBeingBuiltOpt != null)
						{
							CreateExplicitInterfaceImplementationStubs(typeCompilationState, methodSymbol3);
						}
					}
					break;
				}
				}
			}
			if (dictionary != null)
			{
				DetectAndReportCyclesInConstructorCalls(dictionary, _diagnostics);
			}
			if ((object)synthesizedConstructorBase != null)
			{
				SynthesizedConstructorBase method = synthesizedConstructorBase;
				int methodOrdinal = num;
				Binder.ProcessedFieldOrPropertyInitializers empty = Binder.ProcessedFieldOrPropertyInitializers.Empty;
				Binder containingTypeBinder2 = binder;
				MethodSymbol referencedConstructor = null;
				CompileMethod(method, methodOrdinal, ref withEventPropertyIdDispenser2, ref delegateRelaxationIdDispenser2, filter, typeCompilationState, empty, containingTypeBinder2, synthesizedSubmissionFields, ref referencedConstructor);
				if (synthesizedSubmissionFields != null && _moduleBeingBuiltOpt != null)
				{
					synthesizedSubmissionFields.AddToType(containingType, _moduleBeingBuiltOpt);
				}
			}
			if ((object)methodSymbol != null)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = containingType.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (!current.IsShared && current.IsFromCompilation(_compilation) && current.Kind == SymbolKind.Method && current is SourceMemberMethodSymbol sourceMemberMethodSymbol && sourceMemberMethodSymbol.MethodKind == MethodKind.Constructor && !typeCompilationState.CallsInitializeComponent(sourceMemberMethodSymbol))
					{
						Location nonMergedLocation = sourceMemberMethodSymbol.NonMergedLocation;
						if ((object)nonMergedLocation != null)
						{
							Binder.ReportDiagnostic(_diagnostics, nonMergedLocation, ERRID.WRN_ExpectedInitComponentCall2, sourceMemberMethodSymbol, sourceMemberContainerTypeSymbol);
						}
					}
				}
			}
			if (_moduleBeingBuiltOpt != null)
			{
				CompileSynthesizedMethods(typeCompilationState);
			}
			typeCompilationState.Free();
		}

		private void CreateExplicitInterfaceImplementationStubs(TypeCompilationState compilationState, MethodSymbol method)
		{
			ArrayBuilder<SynthesizedInterfaceImplementationStubSymbol> arrayBuilder = null;
			ImmutableArray<MethodSymbol>.Enumerator enumerator = method.ExplicitInterfaceImplementations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(method, current) || !MethodSignatureComparer.ParametersAndReturnTypeSignatureComparer.Equals(method, current))
				{
					continue;
				}
				if (arrayBuilder == null)
				{
					arrayBuilder = ArrayBuilder<SynthesizedInterfaceImplementationStubSymbol>.GetInstance();
				}
				SynthesizedInterfaceImplementationStubSymbol synthesizedInterfaceImplementationStubSymbol = null;
				ArrayBuilder<SynthesizedInterfaceImplementationStubSymbol>.Enumerator enumerator2 = arrayBuilder.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SynthesizedInterfaceImplementationStubSymbol current2 = enumerator2.Current;
					if (MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(current2, current))
					{
						synthesizedInterfaceImplementationStubSymbol = current2;
						break;
					}
				}
				if ((object)synthesizedInterfaceImplementationStubSymbol == null)
				{
					synthesizedInterfaceImplementationStubSymbol = new SynthesizedInterfaceImplementationStubSymbol(method, current);
					arrayBuilder.Add(synthesizedInterfaceImplementationStubSymbol);
					SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(synthesizedInterfaceImplementationStubSymbol, synthesizedInterfaceImplementationStubSymbol, method.Syntax ?? VisualBasicSyntaxTree.Dummy.GetRoot(), compilationState, BindingDiagnosticBag.Discarded);
					MethodSymbol method2 = ((!method.IsGenericMethod) ? method : method.Construct(synthesizedInterfaceImplementationStubSymbol.TypeArguments));
					ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(synthesizedInterfaceImplementationStubSymbol.ParameterCount);
					ImmutableArray<ParameterSymbol>.Enumerator enumerator3 = synthesizedInterfaceImplementationStubSymbol.Parameters.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						ParameterSymbol current3 = enumerator3.Current;
						BoundParameter boundParameter = syntheticBoundNodeFactory.Parameter(current3);
						if (!current3.IsByRef)
						{
							boundParameter = boundParameter.MakeRValue();
						}
						instance.Add(boundParameter);
					}
					BoundCall boundCall = syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Me(), method2, instance.ToImmutableAndFree());
					BoundBlock body = ((!method.IsSub) ? syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(boundCall)) : syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.ExpressionStatement(boundCall), syntheticBoundNodeFactory.Return()));
					syntheticBoundNodeFactory.CloseMethod(body);
					_moduleBeingBuiltOpt.AddSynthesizedDefinition(method.ContainingType, synthesizedInterfaceImplementationStubSymbol.GetCciAdapter());
				}
				synthesizedInterfaceImplementationStubSymbol.AddImplementedMethod(current);
			}
			if (arrayBuilder != null)
			{
				ArrayBuilder<SynthesizedInterfaceImplementationStubSymbol>.Enumerator enumerator4 = arrayBuilder.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					enumerator4.Current.Seal();
				}
				arrayBuilder.Free();
			}
		}

		private static MethodSymbol GetDesignerInitializeComponentMethod(SourceMemberContainerTypeSymbol sourceTypeSymbol)
		{
			if (sourceTypeSymbol.TypeKind == TypeKind.Class && AttributeDataExtensions.IndexOfAttribute(sourceTypeSymbol.GetAttributes(), sourceTypeSymbol, AttributeDescription.DesignerGeneratedAttribute) > -1)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = sourceTypeSymbol.GetMembers("InitializeComponent").GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Method)
					{
						MethodSymbol methodSymbol = (MethodSymbol)current;
						if (methodSymbol.IsSub && !methodSymbol.IsShared && !methodSymbol.IsGenericMethod && methodSymbol.ParameterCount == 0)
						{
							return methodSymbol;
						}
					}
				}
			}
			return null;
		}

		private void CompileSynthesizedMethods(PrivateImplementationDetails privateImplClass)
		{
			TypeCompilationState typeCompilationState = new TypeCompilationState(_compilation, _moduleBeingBuiltOpt, null);
			foreach (IMethodDefinition method in privateImplClass.GetMethods(default(EmitContext)))
			{
				MethodSymbol methodSymbol = (MethodSymbol)method.GetInternalSymbol();
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
				Binder methodBodyBinder = null;
				BoundBlock boundMethodBody = methodSymbol.GetBoundMethodBody(typeCompilationState, instance, out methodBodyBinder);
				if (DoEmitPhase && !instance.HasAnyErrors())
				{
					MethodBody methodBody = GenerateMethodBody(_moduleBeingBuiltOpt, methodSymbol, -1, boundMethodBody, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, null, null, _emitTestCoverageData ? _debugDocumentProvider : null, instance, emittingPdb: false, _emitTestCoverageData, ImmutableArray<SourceSpan>.Empty);
					if (methodBody == null)
					{
						_diagnostics.AddRange(instance);
						instance.Free();
						break;
					}
					_moduleBeingBuiltOpt.SetMethodBody(methodSymbol, methodBody);
				}
				_diagnostics.AddRange(instance);
				instance.Free();
			}
			typeCompilationState.Free();
		}

		private void CompileSynthesizedMethods(ImmutableArray<NamedTypeSymbol> additionalTypes)
		{
			TypeCompilationState typeCompilationState = new TypeCompilationState(_compilation, _moduleBeingBuiltOpt, null);
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = additionalTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				int num = 0;
				foreach (MethodSymbol item in current.GetMethodsToEmit())
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
					Binder methodBodyBinder = null;
					BoundBlock boundMethodBody = item.GetBoundMethodBody(typeCompilationState, instance, out methodBodyBinder);
					MethodBody methodBody = null;
					if (!instance.HasAnyErrors())
					{
						VariableSlotAllocator lazyVariableSlotAllocator = null;
						StateMachineTypeSymbol stateMachineTypeOpt = null;
						ArrayBuilder<LambdaDebugInfo> instance2 = ArrayBuilder<LambdaDebugInfo>.GetInstance();
						ArrayBuilder<ClosureDebugInfo> instance3 = ArrayBuilder<ClosureDebugInfo>.GetInstance();
						int delegateRelaxationIdDispenser = 0;
						ImmutableArray<SourceSpan> dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
						BoundBlock block = Rewriter.LowerBodyOrInitializer(item, num, boundMethodBody, null, typeCompilationState, instrumentForDynamicAnalysis: false, out dynamicAnalysisSpans, _debugDocumentProvider, instance, ref lazyVariableSlotAllocator, instance2, instance3, ref delegateRelaxationIdDispenser, out stateMachineTypeOpt, _moduleBeingBuiltOpt.AllowOmissionOfConditionalCalls, isBodySynthesized: true);
						if (DoEmitPhase && !instance.HasAnyErrors())
						{
							methodBody = GenerateMethodBody(_moduleBeingBuiltOpt, item, -1, block, instance2.ToImmutable(), instance3.ToImmutable(), stateMachineTypeOpt, lazyVariableSlotAllocator, null, instance, emittingPdb: false, emitTestCoverageData: false, dynamicAnalysisSpans);
						}
						instance2.Free();
						instance3.Free();
					}
					_diagnostics.AddRange(instance);
					instance.Free();
					if (methodBody == null)
					{
						if (DoEmitPhase)
						{
							break;
						}
					}
					else
					{
						_moduleBeingBuiltOpt.SetMethodBody(item, methodBody);
					}
					num++;
				}
			}
			if (!_diagnostics.HasAnyErrors())
			{
				CompileSynthesizedMethods(typeCompilationState);
			}
			typeCompilationState.Free();
		}

		private void CompileSynthesizedMethods(TypeCompilationState compilationState)
		{
			if (!DoEmitPhase || !compilationState.HasSynthesizedMethods)
			{
				return;
			}
			ArrayBuilder<TypeCompilationState.MethodWithBody>.Enumerator enumerator = compilationState.SynthesizedMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeCompilationState.MethodWithBody current = enumerator.Current;
				if (!current.Body.HasErrors)
				{
					MethodSymbol method = current.Method;
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
					MethodBody methodBody = GenerateMethodBody(_moduleBeingBuiltOpt, method, -1, current.Body, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, null, null, _debugDocumentProvider, instance, _emittingPdb, _emitTestCoverageData, ImmutableArray<SourceSpan>.Empty);
					_diagnostics.AddRange(instance);
					instance.Free();
					if (methodBody == null)
					{
						break;
					}
					_moduleBeingBuiltOpt.SetMethodBody(method, methodBody);
				}
			}
		}

		private void DetectAndReportCyclesInConstructorCalls(Dictionary<MethodSymbol, MethodSymbol> constructorCallMap, BindingDiagnosticBag diagnostics)
		{
			Dictionary<MethodSymbol, int> dictionary = new Dictionary<MethodSymbol, int>();
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			MethodSymbol methodSymbol = constructorCallMap.Keys.First();
			while (true)
			{
				MethodSymbol value = null;
				if (constructorCallMap.TryGetValue(methodSymbol, out value))
				{
					constructorCallMap.Remove(methodSymbol);
					dictionary.Add(methodSymbol, instance.Count);
					instance.Add(methodSymbol);
					if (!dictionary.TryGetValue(value, out var value2))
					{
						methodSymbol = value;
						continue;
					}
					instance.Add(value);
					ReportConstructorCycles(value2, instance.Count - 1, instance, diagnostics);
				}
				dictionary.Clear();
				instance.Clear();
				if (constructorCallMap.Count == 0)
				{
					break;
				}
				methodSymbol = constructorCallMap.Keys.First();
			}
			instance.Free();
		}

		private static void ReportConstructorCycles(int startsAt, int endsAt, ArrayBuilder<MethodSymbol> path, BindingDiagnosticBag diagnostics)
		{
			ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
			MethodSymbol methodSymbol = path[startsAt];
			for (int i = startsAt + 1; i <= endsAt; i++)
			{
				MethodSymbol methodSymbol2 = path[i];
				instance.Add(ErrorFactory.ErrorInfo(ERRID.ERR_SubNewCycle2, methodSymbol, methodSymbol2));
				methodSymbol = methodSymbol2;
			}
			int num = endsAt - 1;
			for (int j = startsAt; j <= num; j++)
			{
				methodSymbol = path[j];
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_SubNewCycle1, methodSymbol, new CompoundDiagnosticInfo(instance.ToArray())), methodSymbol.Locations[0]));
				if (instance.Count > 1)
				{
					DiagnosticInfo item = instance[0];
					instance.RemoveAt(0);
					instance.Add(item);
				}
			}
			instance.Free();
		}

		internal static bool CanBindMethod(MethodSymbol method)
		{
			if (method.IsExternalMethod || method.IsMustOverride)
			{
				return false;
			}
			if (SymbolExtensions.IsDefaultValueTypeConstructor(method))
			{
				return false;
			}
			if (MethodSymbolExtensions.IsPartialWithoutImplementation(method))
			{
				return false;
			}
			if (!method.IsImplicitlyDeclared && (!(method is SourceMethodSymbol sourceMethodSymbol) || sourceMethodSymbol.BlockSyntax == null))
			{
				return false;
			}
			return true;
		}

		private void CompileMethod(MethodSymbol method, int methodOrdinal, ref int withEventPropertyIdDispenser, ref int delegateRelaxationIdDispenser, Predicate<Symbol> filter, TypeCompilationState compilationState, Binder.ProcessedFieldOrPropertyInitializers processedInitializers, Binder containingTypeBinder, SynthesizedSubmissionFields previousSubmissionFields, ref MethodSymbol referencedConstructor = null)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			SourceMethodSymbol sourceMethodSymbol = method as SourceMethodSymbol;
			if (!DoLoweringPhase && (object)sourceMethodSymbol != null)
			{
				ImmutableArray<Diagnostic> diagnostics = sourceMethodSymbol.Diagnostics;
				if (!diagnostics.IsDefault)
				{
					_diagnostics.AddRange(diagnostics);
					return;
				}
			}
			if (!CanBindMethod(method))
			{
				if ((object)sourceMethodSymbol != null && sourceMethodSymbol.SetDiagnostics(ImmutableArray<Diagnostic>.Empty))
				{
					sourceMethodSymbol.DeclaringCompilation.SymbolDeclaredEvent(method);
				}
				return;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
			Binder methodBodyBinder = null;
			bool injectDefaultConstructorCall = default(bool);
			BoundBlock boundBlock = BindAndAnalyzeMethodBody(method, compilationState, instance, containingTypeBinder, ref referencedConstructor, ref injectDefaultConstructorCall, ref methodBodyBinder);
			processedInitializers.EnsureInitializersAnalyzed(method, instance.DiagnosticBag);
			bool flag = _hasDeclarationErrors || instance.HasAnyErrors() || processedInitializers.HasAnyErrors || boundBlock.HasErrors;
			SetGlobalErrorIfTrue(flag);
			if ((object)sourceMethodSymbol != null && sourceMethodSymbol.SetDiagnostics(instance.DiagnosticBag!.ToReadOnly()))
			{
				VisualBasicCompilation compilation = compilationState.Compilation;
				if (compilation.ShouldAddEvent(method))
				{
					if (boundBlock == null)
					{
						compilation.SymbolDeclaredEvent(sourceMethodSymbol);
					}
					else
					{
						SyntaxTreeSemanticModel syntaxTreeSemanticModel = null;
						if (compilation.SemanticModelProvider is CachingSemanticModelProvider cachingSemanticModelProvider)
						{
							SyntaxNode syntax = boundBlock.Syntax;
							syntaxTreeSemanticModel = (SyntaxTreeSemanticModel)cachingSemanticModelProvider.GetSemanticModel(syntax.SyntaxTree, compilation);
							((MethodBodySemanticModel)syntaxTreeSemanticModel.GetMemberSemanticModel(syntax))?.CacheBoundNodes(boundBlock, syntax);
						}
						compilation.EventQueue!.TryEnqueue(new SymbolDeclaredCompilationEvent(compilation, method, syntaxTreeSemanticModel));
					}
				}
			}
			if (!DoLoweringPhase && (object)sourceMethodSymbol != null)
			{
				_diagnostics.AddRange(sourceMethodSymbol.Diagnostics);
				return;
			}
			if (DoLoweringPhase && !flag)
			{
				LowerAndEmitMethod(method, methodOrdinal, boundBlock, methodBodyBinder ?? containingTypeBinder, compilationState, instance, processedInitializers, previousSubmissionFields, injectDefaultConstructorCall ? referencedConstructor : null, ref delegateRelaxationIdDispenser);
				ImmutableArray<HandledEvent> handledEvents = method.HandledEvents;
				if (!handledEvents.IsEmpty)
				{
					CreateSyntheticWithEventOverridesIfNeeded(handledEvents, ref delegateRelaxationIdDispenser, ref withEventPropertyIdDispenser, compilationState, containingTypeBinder, instance, previousSubmissionFields);
				}
			}
			_diagnostics.AddRange(instance);
			instance.Free();
		}

		private void CreateSyntheticWithEventOverridesIfNeeded(ImmutableArray<HandledEvent> handledEvents, ref int delegateRelaxationIdDispenser, ref int withEventPropertyIdDispenser, TypeCompilationState compilationState, Binder containingTypeBinder, BindingDiagnosticBag diagnostics, SynthesizedSubmissionFields previousSubmissionFields)
		{
			ImmutableArray<HandledEvent>.Enumerator enumerator = handledEvents.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HandledEvent current = enumerator.Current;
				if (current.HandlesKind == HandledEventKind.WithEvents && current.hookupMethod.AssociatedSymbol is SynthesizedOverridingWithEventsProperty synthesizedOverridingWithEventsProperty)
				{
					MethodSymbol getMethod = synthesizedOverridingWithEventsProperty.GetMethod;
					if (!compilationState.HasMethodWrapper(getMethod))
					{
						MethodSymbol setMethod = synthesizedOverridingWithEventsProperty.SetMethod;
						NamedTypeSymbol containingType = synthesizedOverridingWithEventsProperty.ContainingType;
						BoundBlock boundMethodBody = getMethod.GetBoundMethodBody(compilationState, diagnostics, out containingTypeBinder);
						compilationState.AddMethodWrapper(getMethod, getMethod, boundMethodBody);
						_moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, getMethod.GetCciAdapter());
						BoundBlock boundMethodBody2 = setMethod.GetBoundMethodBody(compilationState, diagnostics, out containingTypeBinder);
						ArrayBuilder<LambdaDebugInfo> instance = ArrayBuilder<LambdaDebugInfo>.GetInstance();
						ArrayBuilder<ClosureDebugInfo> instance2 = ArrayBuilder<ClosureDebugInfo>.GetInstance();
						int methodOrdinal = withEventPropertyIdDispenser;
						BoundBlock body = boundMethodBody2;
						ImmutableArray<SourceSpan> dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
						DebugDocumentProvider debugDocumentProvider = _debugDocumentProvider;
						VariableSlotAllocator lazyVariableSlotAllocator = null;
						StateMachineTypeSymbol stateMachineTypeOpt = null;
						boundMethodBody2 = Rewriter.LowerBodyOrInitializer(setMethod, methodOrdinal, body, previousSubmissionFields, compilationState, instrumentForDynamicAnalysis: false, out dynamicAnalysisSpans, debugDocumentProvider, diagnostics, ref lazyVariableSlotAllocator, instance, instance2, ref delegateRelaxationIdDispenser, out stateMachineTypeOpt, allowOmissionOfConditionalCalls: true, isBodySynthesized: true);
						instance.Free();
						instance2.Free();
						compilationState.AddMethodWrapper(setMethod, setMethod, boundMethodBody2);
						_moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, setMethod.GetCciAdapter());
						_moduleBeingBuiltOpt.AddSynthesizedDefinition(containingType, synthesizedOverridingWithEventsProperty.GetCciAdapter());
						withEventPropertyIdDispenser++;
					}
				}
			}
		}

		private static MethodSymbol TryGetMethodCalledInBoundExpressionStatement(BoundExpressionStatement stmt)
		{
			if (stmt == null || stmt.HasErrors)
			{
				return null;
			}
			BoundExpression expression = stmt.Expression;
			if (expression.Kind != BoundKind.Call)
			{
				return null;
			}
			return ((BoundCall)expression).Method;
		}

		private void LowerAndEmitMethod(MethodSymbol method, int methodOrdinal, BoundBlock block, Binder binderOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagsForCurrentMethod, Binder.ProcessedFieldOrPropertyInitializers processedInitializers, SynthesizedSubmissionFields previousSubmissionFields, MethodSymbol constructorToInject, ref int delegateRelaxationIdDispenser)
		{
			BoundExpressionStatement boundExpressionStatement = (((object)constructorToInject == null) ? null : BindDefaultConstructorInitializer(method, constructorToInject, diagsForCurrentMethod, binderOpt));
			if (diagsForCurrentMethod.HasAnyErrors() || (boundExpressionStatement != null && boundExpressionStatement.HasErrors))
			{
				return;
			}
			BoundBlock body = ((method.MethodKind == MethodKind.Constructor || method.MethodKind == MethodKind.StaticConstructor) ? ((!method.IsScriptConstructor) ? InitializerRewriter.BuildConstructorBody(compilationState, method, boundExpressionStatement, processedInitializers, block) : block) : ((!method.IsScriptInitializer) ? block : InitializerRewriter.BuildScriptInitializerBody((SynthesizedInteractiveInitializerMethod)method, processedInitializers, block)));
			BindingDiagnosticBag bindingDiagnosticBag = diagsForCurrentMethod;
			if (method.IsImplicitlyDeclared && (object)method.AssociatedSymbol != null && method.AssociatedSymbol.IsMyGroupCollectionProperty)
			{
				bindingDiagnosticBag = BindingDiagnosticBag.GetInstance(diagsForCurrentMethod);
			}
			VariableSlotAllocator lazyVariableSlotAllocator = null;
			StateMachineTypeSymbol stateMachineTypeOpt = null;
			bool allowOmissionOfConditionalCalls = _moduleBeingBuiltOpt == null || _moduleBeingBuiltOpt.AllowOmissionOfConditionalCalls;
			ArrayBuilder<LambdaDebugInfo> instance = ArrayBuilder<LambdaDebugInfo>.GetInstance();
			ArrayBuilder<ClosureDebugInfo> instance2 = ArrayBuilder<ClosureDebugInfo>.GetInstance();
			ImmutableArray<SourceSpan> dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
			body = Rewriter.LowerBodyOrInitializer(method, methodOrdinal, body, previousSubmissionFields, compilationState, _emitTestCoverageData, out dynamicAnalysisSpans, _debugDocumentProvider, bindingDiagnosticBag, ref lazyVariableSlotAllocator, instance, instance2, ref delegateRelaxationIdDispenser, out stateMachineTypeOpt, allowOmissionOfConditionalCalls, isBodySynthesized: false);
			ImmutableArray<BoundStatement> items = (method.IsSubmissionConstructor ? SynthesizedSubmissionConstructorSymbol.MakeSubmissionInitialization(block.Syntax, method, previousSubmissionFields, _compilation) : ImmutableArray<BoundStatement>.Empty);
			bool flag = body.HasErrors || diagsForCurrentMethod.HasAnyErrors() || (bindingDiagnosticBag != diagsForCurrentMethod && bindingDiagnosticBag.HasAnyErrors());
			SetGlobalErrorIfTrue(flag);
			if (_moduleBeingBuiltOpt == null || flag)
			{
				if (bindingDiagnosticBag != diagsForCurrentMethod)
				{
					((SynthesizedMyGroupCollectionPropertySymbol)method.AssociatedSymbol).RelocateDiagnostics(bindingDiagnosticBag.DiagnosticBag, diagsForCurrentMethod.DiagnosticBag);
					diagsForCurrentMethod.AddDependencies(bindingDiagnosticBag);
					bindingDiagnosticBag.Free();
				}
				return;
			}
			if (method.IsScriptConstructor)
			{
				ArrayBuilder<BoundStatement> instance3 = ArrayBuilder<BoundStatement>.GetInstance();
				instance3.Add(boundExpressionStatement);
				instance3.AddRange(items);
				instance3.Add(body);
				body = BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(body.Syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, instance3.ToImmutableAndFree(), body.HasErrors));
			}
			if (DoEmitPhase)
			{
				MethodBody body2 = GenerateMethodBody(_moduleBeingBuiltOpt, method, methodOrdinal, body, instance.ToImmutable(), instance2.ToImmutable(), stateMachineTypeOpt, lazyVariableSlotAllocator, _debugDocumentProvider, bindingDiagnosticBag, _emittingPdb, _emitTestCoverageData, dynamicAnalysisSpans);
				_moduleBeingBuiltOpt.SetMethodBody(method.PartialDefinitionPart ?? method, body2);
			}
			if (bindingDiagnosticBag != diagsForCurrentMethod)
			{
				((SynthesizedMyGroupCollectionPropertySymbol)method.AssociatedSymbol).RelocateDiagnostics(bindingDiagnosticBag.DiagnosticBag, diagsForCurrentMethod.DiagnosticBag);
				diagsForCurrentMethod.AddDependencies(bindingDiagnosticBag);
				bindingDiagnosticBag.Free();
			}
			instance.Free();
			instance2.Free();
		}

		internal static MethodBody GenerateMethodBody(PEModuleBuilder moduleBuilder, MethodSymbol method, int methodOrdinal, BoundStatement block, ImmutableArray<LambdaDebugInfo> lambdaDebugInfo, ImmutableArray<ClosureDebugInfo> closureDebugInfo, StateMachineTypeSymbol stateMachineTypeOpt, VariableSlotAllocator variableSlotAllocatorOpt, DebugDocumentProvider debugDocumentProvider, BindingDiagnosticBag diagnostics, bool emittingPdb, bool emitTestCoverageData, ImmutableArray<SourceSpan> dynamicAnalysisSpans)
		{
			VisualBasicCompilation compilation = moduleBuilder.Compilation;
			LocalSlotManager localSlotManager = new LocalSlotManager(variableSlotAllocatorOpt);
			OptimizationLevel optimizationLevel = compilation.Options.OptimizationLevel;
			if (method.IsEmbedded)
			{
				optimizationLevel = OptimizationLevel.Release;
			}
			ILBuilder iLBuilder = new ILBuilder(moduleBuilder, localSlotManager, optimizationLevel, areLocalsZeroed: true);
			try
			{
				StateMachineMoveNextBodyDebugInfo stateMachineMoveNextDebugInfoOpt = null;
				CodeGenerator codeGenerator = new CodeGenerator(method, block, iLBuilder, moduleBuilder, diagnostics.DiagnosticBag, optimizationLevel, emittingPdb);
				if (diagnostics.HasAnyErrors())
				{
					return null;
				}
				bool flag;
				MethodSymbol kickoffMethod;
				if (method is SynthesizedStateMachineMethod synthesizedStateMachineMethod && EmbeddedOperators.CompareString(method.Name, "MoveNext", TextCompare: false) == 0)
				{
					kickoffMethod = synthesizedStateMachineMethod.StateMachineType.KickoffMethod;
					flag = kickoffMethod.IsAsync;
					kickoffMethod = kickoffMethod.PartialDefinitionPart ?? kickoffMethod;
				}
				else
				{
					kickoffMethod = null;
					flag = false;
				}
				if (flag)
				{
					int asyncCatchHandlerOffset = -1;
					ImmutableArray<int> asyncYieldPoints = default(ImmutableArray<int>);
					ImmutableArray<int> asyncResumePoints = default(ImmutableArray<int>);
					codeGenerator.Generate(out asyncCatchHandlerOffset, out asyncYieldPoints, out asyncResumePoints);
					stateMachineMoveNextDebugInfoOpt = new AsyncMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter(), kickoffMethod.IsSub ? asyncCatchHandlerOffset : (-1), asyncYieldPoints, asyncResumePoints);
				}
				else
				{
					codeGenerator.Generate();
					if ((object)kickoffMethod != null)
					{
						stateMachineMoveNextDebugInfoOpt = new IteratorMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter());
					}
				}
				ImmutableArray<StateMachineHoistedLocalScope> stateMachineHoistedLocalScopes = (((object)kickoffMethod == null || moduleBuilder.DebugInformationFormat == DebugInformationFormat.Pdb) ? default(ImmutableArray<StateMachineHoistedLocalScope>) : iLBuilder.GetHoistedLocalScopes());
				IImportScope importScopeOpt = ((method.Syntax == null) ? null : moduleBuilder.SourceModule.TryGetSourceFile(method.Syntax.SyntaxTree)?.Translate(moduleBuilder, diagnostics.DiagnosticBag));
				if (diagnostics.HasAnyErrors())
				{
					return null;
				}
				if (moduleBuilder.SaveTestData)
				{
					moduleBuilder.SetMethodTestData(method, iLBuilder.GetSnapshot());
				}
				ImmutableArray<EncHoistedLocalInfo> hoistedVariableSlots = default(ImmutableArray<EncHoistedLocalInfo>);
				ImmutableArray<ITypeReference> awaiterSlots = default(ImmutableArray<ITypeReference>);
				if (optimizationLevel == OptimizationLevel.Debug && (object)stateMachineTypeOpt != null)
				{
					GetStateMachineSlotDebugInfo(moduleBuilder, moduleBuilder.GetSynthesizedFields(stateMachineTypeOpt), variableSlotAllocatorOpt, diagnostics.DiagnosticBag, ref hoistedVariableSlots, ref awaiterSlots);
				}
				ImmutableArray<LocalScope> allScopes = iLBuilder.GetAllScopes();
				DynamicAnalysisMethodBodyData dynamicAnalysisDataOpt = null;
				if (emitTestCoverageData)
				{
					dynamicAnalysisDataOpt = new DynamicAnalysisMethodBodyData(dynamicAnalysisSpans);
				}
				ImmutableArray<byte> realizedIL = iLBuilder.RealizedIL;
				ushort maxStack = iLBuilder.MaxStack;
				MethodSymbol cciAdapter = (method.PartialDefinitionPart ?? method).GetCciAdapter();
				DebugId? debugId;
				DebugId? debugId2 = (debugId = variableSlotAllocatorOpt?.MethodId);
				return new MethodBody(realizedIL, maxStack, cciAdapter, debugId2.HasValue ? debugId.GetValueOrDefault() : new DebugId(methodOrdinal, moduleBuilder.CurrentGenerationOrdinal), iLBuilder.LocalSlotManager.LocalsInOrder(), iLBuilder.RealizedSequencePoints, debugDocumentProvider, iLBuilder.RealizedExceptionHandlers, areLocalsZeroed: true, hasStackalloc: false, allScopes, hasDynamicLocalVariables: false, importScopeOpt, lambdaDebugInfo, closureDebugInfo, stateMachineTypeOpt?.Name, stateMachineHoistedLocalScopes, hoistedVariableSlots, awaiterSlots, stateMachineMoveNextDebugInfoOpt, dynamicAnalysisDataOpt);
			}
			finally
			{
				iLBuilder.FreeBasicBlocks();
			}
		}

		private static void GetStateMachineSlotDebugInfo(PEModuleBuilder moduleBuilder, IEnumerable<IFieldDefinition> fieldDefs, VariableSlotAllocator variableSlotAllocatorOpt, DiagnosticBag diagnostics, ref ImmutableArray<EncHoistedLocalInfo> hoistedVariableSlots, ref ImmutableArray<ITypeReference> awaiterSlots)
		{
			ArrayBuilder<EncHoistedLocalInfo> instance = ArrayBuilder<EncHoistedLocalInfo>.GetInstance();
			ArrayBuilder<ITypeReference> instance2 = ArrayBuilder<ITypeReference>.GetInstance();
			foreach (IFieldDefinition fieldDef in fieldDefs)
			{
				StateMachineFieldSymbol stateMachineFieldSymbol = (StateMachineFieldSymbol)fieldDef.GetInternalSymbol();
				int slotIndex = stateMachineFieldSymbol.SlotIndex;
				if (stateMachineFieldSymbol.SlotDebugInfo.SynthesizedKind == SynthesizedLocalKind.AwaiterField)
				{
					while (slotIndex >= instance2.Count)
					{
						instance2.Add(null);
					}
					instance2[slotIndex] = moduleBuilder.EncTranslateLocalVariableType(stateMachineFieldSymbol.Type, diagnostics);
				}
				else if (!stateMachineFieldSymbol.SlotDebugInfo.Id.IsNone)
				{
					while (slotIndex >= instance.Count)
					{
						instance.Add(default(EncHoistedLocalInfo));
					}
					instance[slotIndex] = new EncHoistedLocalInfo(stateMachineFieldSymbol.SlotDebugInfo, moduleBuilder.EncTranslateLocalVariableType(stateMachineFieldSymbol.Type, diagnostics));
				}
			}
			if (variableSlotAllocatorOpt != null)
			{
				int previousAwaiterSlotCount = variableSlotAllocatorOpt.PreviousAwaiterSlotCount;
				while (instance2.Count < previousAwaiterSlotCount)
				{
					instance2.Add(null);
				}
				int previousHoistedLocalSlotCount = variableSlotAllocatorOpt.PreviousHoistedLocalSlotCount;
				while (instance.Count < previousHoistedLocalSlotCount)
				{
					instance.Add(new EncHoistedLocalInfo(_: true));
				}
			}
			hoistedVariableSlots = instance.ToImmutableAndFree();
			awaiterSlots = instance2.ToImmutableAndFree();
		}

		private static BoundBlock BindAndAnalyzeMethodBody(MethodSymbol method, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, Binder containingTypeBinder, ref MethodSymbol referencedConstructor, ref bool injectDefaultConstructorCall, ref Binder methodBodyBinder)
		{
			referencedConstructor = null;
			injectDefaultConstructorCall = false;
			methodBodyBinder = null;
			BoundBlock boundMethodBody = method.GetBoundMethodBody(compilationState, diagnostics, out methodBodyBinder);
			Analyzer.AnalyzeMethodBody(method, boundMethodBody, diagnostics.DiagnosticBag);
			DiagnosticsPass.IssueDiagnostics(boundMethodBody, diagnostics.DiagnosticBag, method);
			if (!method.IsShared && (object)compilationState.InitializeComponentOpt != null && !method.IsImplicitlyDeclared)
			{
				try
				{
					InitializeComponentCallTreeBuilder.CollectCallees(compilationState, method, boundMethodBody);
				}
				catch (BoundTreeVisitor.CancelledByStackGuardException ex)
				{
					ProjectData.SetProjectError(ex);
					BoundTreeVisitor.CancelledByStackGuardException ex2 = ex;
					ex2.AddAnError(diagnostics);
					ProjectData.ClearProjectError();
				}
			}
			if (method.MethodKind == MethodKind.Constructor)
			{
				injectDefaultConstructorCall = !method.ContainingType.IsValueType;
				if (boundMethodBody != null && boundMethodBody.Statements.Length > 0)
				{
					BoundStatement boundStatement = boundMethodBody.Statements[0];
					if (boundStatement.HasErrors)
					{
						injectDefaultConstructorCall = false;
					}
					else if (boundStatement.Kind == BoundKind.ExpressionStatement)
					{
						MethodSymbol methodSymbol = TryGetMethodCalledInBoundExpressionStatement((BoundExpressionStatement)boundStatement);
						if ((object)methodSymbol != null && methodSymbol.MethodKind == MethodKind.Constructor)
						{
							referencedConstructor = methodSymbol;
							injectDefaultConstructorCall = false;
						}
					}
				}
				if (injectDefaultConstructorCall)
				{
					referencedConstructor = FindConstructorToCallByDefault(method, diagnostics, methodBodyBinder ?? containingTypeBinder);
				}
			}
			return boundMethodBody;
		}

		private static MethodSymbol FindConstructorToCallByDefault(MethodSymbol constructor, BindingDiagnosticBag diagnostics, Binder binderForAccessibilityCheckOpt = null)
		{
			NamedTypeSymbol containingType = constructor.ContainingType;
			if (containingType.IsSubmissionClass)
			{
				return constructor.ContainingAssembly.GetSpecialType(SpecialType.System_Object).InstanceConstructors.Single();
			}
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics == null || TypeSymbolExtensions.IsErrorType(baseTypeNoUseSiteDiagnostics))
			{
				return null;
			}
			MethodSymbol methodSymbol = null;
			bool flag = false;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, containingType.ContainingAssembly);
			ImmutableArray<MethodSymbol>.Enumerator enumerator = baseTypeNoUseSiteDiagnostics.InstanceConstructors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (current.IsGenericMethod)
				{
					continue;
				}
				if (binderForAccessibilityCheckOpt != null)
				{
					if (!binderForAccessibilityCheckOpt.IsAccessible(current, ref useSiteInfo, containingType))
					{
						continue;
					}
				}
				else
				{
					if (current.DeclaredAccessibility != Accessibility.Public)
					{
						continue;
					}
					if (current.ParameterCount != 0)
					{
						flag = true;
						continue;
					}
				}
				flag = true;
				if (!(containingType.IsReferenceType ? MethodSymbolExtensions.CanBeCalledWithNoParameters(current) : (current.ParameterCount == 0)))
				{
					continue;
				}
				if ((object)methodSymbol == null)
				{
					methodSymbol = current;
					continue;
				}
				if (constructor.IsImplicitlyDeclared)
				{
					diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NoUniqueConstructorOnBase2, containingType, containingType.BaseTypeNoUseSiteDiagnostics), containingType.Locations[0]));
				}
				else
				{
					diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_RequiredNewCallTooMany2, baseTypeNoUseSiteDiagnostics, containingType), constructor.Locations[0]));
				}
				return methodSymbol;
			}
			ImmutableArray<Location> immutableArray = (constructor.IsImplicitlyDeclared ? containingType.Locations : constructor.Locations);
			diagnostics.Add(immutableArray.IsDefaultOrEmpty ? Location.None : immutableArray[0], useSiteInfo);
			if ((object)methodSymbol == null)
			{
				if (flag)
				{
					if (constructor.IsImplicitlyDeclared)
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NoConstructorOnBase2, containingType, containingType.BaseTypeNoUseSiteDiagnostics), containingType.Locations[0]));
					}
					else
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_RequiredNewCall2, baseTypeNoUseSiteDiagnostics, containingType), constructor.Locations[0]));
					}
				}
				else
				{
					diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NoAccessibleConstructorOnBase, containingType.BaseTypeNoUseSiteDiagnostics), containingType.Locations[0]));
				}
			}
			if ((object)methodSymbol != null)
			{
				methodSymbol.ForceCompleteObsoleteAttribute();
				if (methodSymbol.ObsoleteState == ThreeState.True)
				{
					ObsoleteAttributeData obsoleteAttributeData = methodSymbol.ObsoleteAttributeData;
					if (constructor.IsImplicitlyDeclared)
					{
						if (string.IsNullOrEmpty(obsoleteAttributeData.Message))
						{
							diagnostics.Add(obsoleteAttributeData.IsError ? ERRID.ERR_NoNonObsoleteConstructorOnBase3 : ERRID.WRN_NoNonObsoleteConstructorOnBase3, containingType.Locations[0], containingType, methodSymbol, containingType.BaseTypeNoUseSiteDiagnostics);
						}
						else
						{
							diagnostics.Add(obsoleteAttributeData.IsError ? ERRID.ERR_NoNonObsoleteConstructorOnBase4 : ERRID.WRN_NoNonObsoleteConstructorOnBase4, containingType.Locations[0], containingType, methodSymbol, containingType.BaseTypeNoUseSiteDiagnostics, obsoleteAttributeData.Message);
						}
					}
					else if (string.IsNullOrEmpty(obsoleteAttributeData.Message))
					{
						diagnostics.Add(obsoleteAttributeData.IsError ? ERRID.ERR_RequiredNonObsoleteNewCall3 : ERRID.WRN_RequiredNonObsoleteNewCall3, constructor.Locations[0], methodSymbol, containingType.BaseTypeNoUseSiteDiagnostics, containingType);
					}
					else
					{
						diagnostics.Add(obsoleteAttributeData.IsError ? ERRID.ERR_RequiredNonObsoleteNewCall4 : ERRID.WRN_RequiredNonObsoleteNewCall4, constructor.Locations[0], methodSymbol, containingType.BaseTypeNoUseSiteDiagnostics, containingType, obsoleteAttributeData.Message);
					}
				}
			}
			return methodSymbol;
		}

		private static BoundExpressionStatement BindDefaultConstructorInitializer(MethodSymbol constructor, MethodSymbol constructorToCall, BindingDiagnosticBag diagnostics, Binder binderOpt = null)
		{
			NamedTypeSymbol specialType = constructor.ContainingAssembly.GetSpecialType(SpecialType.System_Void);
			SyntaxNode syntax = constructor.Syntax;
			BoundMeReference boundMeReference = new BoundMeReference(syntax, constructor.ContainingType);
			boundMeReference.SetWasCompilerGenerated();
			BoundExpression boundExpression = null;
			if (constructorToCall.ParameterCount == 0)
			{
				boundExpression = new BoundCall(syntax, constructorToCall, null, boundMeReference, ImmutableArray<BoundExpression>.Empty, null, specialType);
			}
			else
			{
				BoundMethodGroup group = new BoundMethodGroup(constructor.Syntax, null, ImmutableArray.Create(constructorToCall), LookupResultKind.Good, boundMeReference, QualificationKind.QualifiedViaValue);
				boundExpression = binderOpt.BindInvocationExpression(constructor.Syntax, null, TypeCharacter.None, group, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<string>), diagnostics, null, allowConstructorCall: true);
			}
			boundExpression.SetWasCompilerGenerated();
			BoundExpressionStatement boundExpressionStatement = new BoundExpressionStatement(syntax, boundExpression);
			boundExpressionStatement.SetWasCompilerGenerated();
			return boundExpressionStatement;
		}

		internal static BoundExpressionStatement BindDefaultConstructorInitializer(MethodSymbol constructor, BindingDiagnosticBag diagnostics)
		{
			MethodSymbol methodSymbol = FindConstructorToCallByDefault(constructor, diagnostics);
			if ((object)methodSymbol == null)
			{
				return null;
			}
			return BindDefaultConstructorInitializer(constructor, methodSymbol, diagnostics);
		}

		private static DebugSourceDocument CreateDebugDocumentForFile(string normalizedPath)
		{
			return new DebugSourceDocument(normalizedPath, DebugSourceDocument.CorSymLanguageTypeBasic);
		}

		private static bool PassesFilter(Predicate<Symbol> filterOpt, Symbol symbol)
		{
			return filterOpt?.Invoke(symbol) ?? true;
		}
	}
}
