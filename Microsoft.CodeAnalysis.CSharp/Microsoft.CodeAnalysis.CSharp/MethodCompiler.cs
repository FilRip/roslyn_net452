using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class MethodCompiler : CSharpSymbolVisitor<TypeCompilationState, object>
    {
        private readonly CSharpCompilation _compilation;

        private readonly bool _emittingPdb;

        private readonly bool _emitTestCoverageData;

        private readonly CancellationToken _cancellationToken;

        private readonly BindingDiagnosticBag _diagnostics;

        private readonly bool _hasDeclarationErrors;

        private readonly bool _emitMethodBodies;

        private readonly PEModuleBuilder _moduleBeingBuiltOpt;

        private readonly Predicate<Symbol> _filterOpt;

        private readonly DebugDocumentProvider _debugDocumentProvider;

        private readonly SynthesizedEntryPointSymbol.AsyncForwardEntryPoint _entryPointOpt;

        private ConcurrentStack<Task> _compilerTasks;

        private bool _globalHasErrors;

        private void SetGlobalErrorIfTrue(bool arg)
        {
            if (arg)
            {
                _globalHasErrors = true;
            }
        }

        internal MethodCompiler(CSharpCompilation compilation, PEModuleBuilder moduleBeingBuiltOpt, bool emittingPdb, bool emitTestCoverageData, bool hasDeclarationErrors, bool emitMethodBodies, BindingDiagnosticBag diagnostics, Predicate<Symbol> filterOpt, SynthesizedEntryPointSymbol.AsyncForwardEntryPoint entryPointOpt, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _moduleBeingBuiltOpt = moduleBeingBuiltOpt;
            _emittingPdb = emittingPdb;
            _cancellationToken = cancellationToken;
            _diagnostics = diagnostics;
            _filterOpt = filterOpt;
            _entryPointOpt = entryPointOpt;
            _hasDeclarationErrors = hasDeclarationErrors;
            SetGlobalErrorIfTrue(hasDeclarationErrors);
            if (emittingPdb || emitTestCoverageData)
            {
                _debugDocumentProvider = (string path, string basePath) => moduleBeingBuiltOpt.DebugDocumentsBuilder.GetOrAddDebugDocument(path, basePath, CreateDebugDocumentForFile);
            }
            _emitTestCoverageData = emitTestCoverageData;
            _emitMethodBodies = emitMethodBodies;
        }

        public static void CompileMethodBodies(CSharpCompilation compilation, PEModuleBuilder moduleBeingBuiltOpt, bool emittingPdb, bool emitTestCoverageData, bool hasDeclarationErrors, bool emitMethodBodies, BindingDiagnosticBag diagnostics, Predicate<Symbol> filterOpt, CancellationToken cancellationToken)
        {
            if (compilation.PreviousSubmission != null)
            {
                compilation.PreviousSubmission!.EnsureAnonymousTypeTemplates(cancellationToken);
            }
            MethodSymbol methodSymbol = null;
            if (filterOpt == null)
            {
                methodSymbol = GetEntryPoint(compilation, moduleBeingBuiltOpt, hasDeclarationErrors, emitMethodBodies, diagnostics, cancellationToken);
            }
            MethodCompiler methodCompiler = new MethodCompiler(compilation, moduleBeingBuiltOpt, emittingPdb, emitTestCoverageData, hasDeclarationErrors, emitMethodBodies, diagnostics, filterOpt, methodSymbol as SynthesizedEntryPointSymbol.AsyncForwardEntryPoint, cancellationToken);
            if (compilation.Options.ConcurrentBuild)
            {
                methodCompiler._compilerTasks = new ConcurrentStack<Task>();
            }
            methodCompiler.CompileNamespace(compilation.SourceModule.GlobalNamespace);
            methodCompiler.WaitForWorkers();
            if (moduleBeingBuiltOpt != null)
            {
                ImmutableArray<NamedTypeSymbol> additionalTopLevelTypes = moduleBeingBuiltOpt.GetAdditionalTopLevelTypes();
                methodCompiler.CompileSynthesizedMethods(additionalTopLevelTypes, diagnostics);
                ImmutableArray<NamedTypeSymbol> embeddedTypes = moduleBeingBuiltOpt.GetEmbeddedTypes(diagnostics);
                methodCompiler.CompileSynthesizedMethods(embeddedTypes, diagnostics);
                if (emitMethodBodies)
                {
                    compilation.AnonymousTypeManager.AssignTemplatesNamesAndCompile(methodCompiler, moduleBeingBuiltOpt, diagnostics);
                }
                methodCompiler.WaitForWorkers();
                PrivateImplementationDetails privateImplClass = moduleBeingBuiltOpt.PrivateImplClass;
                if (privateImplClass != null)
                {
                    privateImplClass.Freeze();
                    methodCompiler.CompileSynthesizedMethods(privateImplClass, diagnostics);
                }
            }
            if (moduleBeingBuiltOpt != null && (methodCompiler._globalHasErrors || moduleBeingBuiltOpt.SourceModule.HasBadAttributes) && !diagnostics.HasAnyErrors() && !hasDeclarationErrors)
            {
                string nameOfLocalizableResource = (methodCompiler._globalHasErrors ? "UnableToDetermineSpecificCauseOfFailure" : "ModuleHasInvalidAttributes");
                diagnostics.Add(ErrorCode.ERR_ModuleEmitFailure, NoLocation.Singleton, ((INamedEntity)moduleBeingBuiltOpt).Name, new LocalizableResourceString(nameOfLocalizableResource, CodeAnalysisResources.ResourceManager, typeof(CodeAnalysisResources)));
            }
            diagnostics.AddRange(compilation.AdditionalCodegenWarnings);
            if (filterOpt == null)
            {
                WarnUnusedFields(compilation, diagnostics, cancellationToken);
                if (moduleBeingBuiltOpt != null && methodSymbol != null && compilation.Options.OutputKind.IsApplication())
                {
                    moduleBeingBuiltOpt.SetPEEntryPoint(methodSymbol, diagnostics.DiagnosticBag);
                }
            }
        }

        private static MethodSymbol GetEntryPoint(CSharpCompilation compilation, PEModuleBuilder moduleBeingBuilt, bool hasDeclarationErrors, bool emitMethodBodies, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            CSharpCompilation.EntryPoint entryPointAndDiagnostics = compilation.GetEntryPointAndDiagnostics(cancellationToken);
            diagnostics.AddRange(entryPointAndDiagnostics.Diagnostics, allowMismatchInDependencyAccumulation: true);
            MethodSymbol methodSymbol = entryPointAndDiagnostics.MethodSymbol;
            if ((object)methodSymbol == null)
            {
                return null;
            }
            SynthesizedEntryPointSymbol synthesizedEntryPointSymbol = methodSymbol as SynthesizedEntryPointSymbol;
            if ((object)synthesizedEntryPointSymbol == null)
            {
                TypeSymbol returnType = methodSymbol.ReturnType;
                if (returnType.IsGenericTaskType(compilation) || returnType.IsNonGenericTaskType(compilation))
                {
                    synthesizedEntryPointSymbol = new SynthesizedEntryPointSymbol.AsyncForwardEntryPoint(compilation, methodSymbol.ContainingType, methodSymbol);
                    methodSymbol = synthesizedEntryPointSymbol;
                    moduleBeingBuilt?.AddSynthesizedDefinition(methodSymbol.ContainingType, synthesizedEntryPointSymbol.GetCciAdapter());
                }
            }
            if ((object)synthesizedEntryPointSymbol != null && moduleBeingBuilt != null && !hasDeclarationErrors && !diagnostics.HasAnyErrors())
            {
                BoundStatement boundStatement = synthesizedEntryPointSymbol.CreateBody(diagnostics);
                if (boundStatement.HasErrors || diagnostics.HasAnyErrors())
                {
                    return methodSymbol;
                }
                ImmutableArray<SourceSpan> dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
                VariableSlotAllocator lazyVariableSlotAllocator = null;
                ArrayBuilder<LambdaDebugInfo> instance = ArrayBuilder<LambdaDebugInfo>.GetInstance();
                ArrayBuilder<ClosureDebugInfo> instance2 = ArrayBuilder<ClosureDebugInfo>.GetInstance();
                BoundStatement block = LowerBodyOrInitializer(synthesizedEntryPointSymbol, -1, boundStatement, null, new TypeCompilationState(synthesizedEntryPointSymbol.ContainingType, compilation, moduleBeingBuilt), instrumentForDynamicAnalysis: false, null, ref dynamicAnalysisSpans, diagnostics, ref lazyVariableSlotAllocator, instance, instance2, out StateMachineTypeSymbol stateMachineTypeOpt);
                instance.Free();
                instance2.Free();
                if (emitMethodBodies)
                {
                    MethodBody body = GenerateMethodBody(moduleBeingBuilt, synthesizedEntryPointSymbol, -1, block, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, null, null, diagnostics, null, null, emittingPdb: false, emitTestCoverageData: false, ImmutableArray<SourceSpan>.Empty, null);
                    moduleBeingBuilt.SetMethodBody(synthesizedEntryPointSymbol, body);
                }
            }
            return methodSymbol;
        }

        private void WaitForWorkers()
        {
            ConcurrentStack<Task> compilerTasks = _compilerTasks;
            if (compilerTasks != null)
            {
                while (compilerTasks.TryPop(out Task result))
                {
                    result.GetAwaiter().GetResult();
                }
            }
        }

        private static void WarnUnusedFields(CSharpCompilation compilation, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            SourceAssemblySymbol sourceAssemblySymbol = (SourceAssemblySymbol)compilation.Assembly;
            diagnostics.AddRange(sourceAssemblySymbol.GetUnusedFieldWarnings(cancellationToken));
        }

        public override object VisitNamespace(NamespaceSymbol symbol, TypeCompilationState arg)
        {
            if (!PassesFilter(_filterOpt, symbol))
            {
                return null;
            }
            arg = null;
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (_compilation.Options.ConcurrentBuild)
            {
                Task item = CompileNamespaceAsAsync(symbol);
                _compilerTasks.Push(item);
            }
            else
            {
                CompileNamespace(symbol);
            }
            return null;
        }

        private Task CompileNamespaceAsAsync(NamespaceSymbol symbol)
        {
            return Task.Run(UICultureUtilities.WithCurrentUICulture(delegate
            {
                try
                {
                    CompileNamespace(symbol);
                }
                catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception))
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }), _cancellationToken);
        }

        private void CompileNamespace(NamespaceSymbol symbol)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Accept(this, null);
            }
        }

        public override object VisitNamedType(NamedTypeSymbol symbol, TypeCompilationState arg)
        {
            if (!PassesFilter(_filterOpt, symbol))
            {
                return null;
            }
            arg = null;
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (_compilation.Options.ConcurrentBuild)
            {
                Task item = CompileNamedTypeAsync(symbol);
                _compilerTasks.Push(item);
            }
            else
            {
                CompileNamedType(symbol);
            }
            return null;
        }

        private Task CompileNamedTypeAsync(NamedTypeSymbol symbol)
        {
            return Task.Run(UICultureUtilities.WithCurrentUICulture(delegate
            {
                try
                {
                    CompileNamedType(symbol);
                }
                catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception))
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }), _cancellationToken);
        }

        private void CompileNamedType(NamedTypeSymbol containingType)
        {
            TypeCompilationState typeCompilationState = new TypeCompilationState(containingType, _compilation, _moduleBeingBuiltOpt);
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            SynthesizedInstanceConstructor synthesizedInstanceConstructor = null;
            SynthesizedInteractiveInitializerMethod scriptInitializerOpt = null;
            SynthesizedEntryPointSymbol synthesizedEntryPointSymbol = null;
            int methodOrdinal = -1;
            if (containingType.IsScriptClass)
            {
                synthesizedInstanceConstructor = containingType.GetScriptConstructor();
                scriptInitializerOpt = containingType.GetScriptInitializer();
                synthesizedEntryPointSymbol = containingType.GetScriptEntryPoint();
            }
            SynthesizedSubmissionFields synthesizedSubmissionFields = (containingType.IsSubmissionClass ? new SynthesizedSubmissionFields(_compilation, containingType) : null);
            Binder.ProcessedFieldInitializers processedInitializers = default(Binder.ProcessedFieldInitializers);
            Binder.ProcessedFieldInitializers processedInitializers2 = default(Binder.ProcessedFieldInitializers);
            SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol = containingType as SourceMemberContainerTypeSymbol;
            if ((object)sourceMemberContainerTypeSymbol != null)
            {
                cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                Binder.BindFieldInitializers(_compilation, scriptInitializerOpt, sourceMemberContainerTypeSymbol.StaticInitializers, _diagnostics, ref processedInitializers);
                cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                Binder.BindFieldInitializers(_compilation, scriptInitializerOpt, sourceMemberContainerTypeSymbol.InstanceInitializers, _diagnostics, ref processedInitializers2);
                if (typeCompilationState.Emitting)
                {
                    CompileSynthesizedExplicitImplementations(sourceMemberContainerTypeSymbol, typeCompilationState);
                }
            }
            bool flag = false;
            ImmutableArray<Symbol> members = containingType.GetMembers();
            for (int i = 0; i < members.Length; i++)
            {
                Symbol symbol = members[i];
                if (!PassesFilter(_filterOpt, symbol))
                {
                    continue;
                }
                switch (symbol.Kind)
                {
                    case SymbolKind.NamedType:
                        symbol.Accept(this, typeCompilationState);
                        break;
                    case SymbolKind.Method:
                        {
                            MethodSymbol methodSymbol = (MethodSymbol)symbol;
                            if (methodSymbol.IsScriptConstructor)
                            {
                                methodOrdinal = i;
                            }
                            else
                            {
                                if ((object)methodSymbol == synthesizedEntryPointSymbol || IsFieldLikeEventAccessor(methodSymbol))
                                {
                                    break;
                                }
                                if (methodSymbol.IsPartialDefinition())
                                {
                                    methodSymbol = methodSymbol.PartialImplementationPart;
                                    if ((object)methodSymbol == null)
                                    {
                                        break;
                                    }
                                }
                                Binder.ProcessedFieldInitializers processedInitializers3 = ((methodSymbol.MethodKind == MethodKind.Constructor || methodSymbol.IsScriptInitializer) ? processedInitializers2 : ((methodSymbol.MethodKind == MethodKind.StaticConstructor) ? processedInitializers : default(Binder.ProcessedFieldInitializers)));
                                CompileMethod(methodSymbol, i, ref processedInitializers3, synthesizedSubmissionFields, typeCompilationState);
                                if (methodSymbol.MethodKind == MethodKind.StaticConstructor)
                                {
                                    flag = true;
                                }
                            }
                            break;
                        }
                    case SymbolKind.Property:
                        if (symbol is SourcePropertySymbolBase sourcePropertySymbolBase && sourcePropertySymbolBase.IsSealed && typeCompilationState.Emitting)
                        {
                            CompileSynthesizedSealedAccessors(sourcePropertySymbolBase, typeCompilationState);
                        }
                        break;
                    case SymbolKind.Event:
                        if (symbol is SourceEventSymbol sourceEventSymbol && sourceEventSymbol.HasAssociatedField && !sourceEventSymbol.IsAbstract && typeCompilationState.Emitting)
                        {
                            CompileFieldLikeEventAccessor(sourceEventSymbol, isAddMethod: true);
                            CompileFieldLikeEventAccessor(sourceEventSymbol, isAddMethod: false);
                        }
                        break;
                    case SymbolKind.Field:
                        {
                            FieldSymbol fieldSymbol = (FieldSymbol)symbol;
                            if (!(symbol is TupleErrorFieldSymbol))
                            {
                                if (fieldSymbol.IsConst)
                                {
                                    ConstantValue constantValue = fieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
                                    SetGlobalErrorIfTrue(constantValue == null || constantValue.IsBad);
                                }
                                if (fieldSymbol.IsFixedSizeBuffer && typeCompilationState.Emitting)
                                {
                                    fieldSymbol.FixedImplementationType(typeCompilationState.ModuleBuilderOpt);
                                }
                            }
                            break;
                        }
                }
            }
            if (AnonymousTypeManager.IsAnonymousTypeTemplate(containingType))
            {
                Binder.ProcessedFieldInitializers processedInitializers4 = default(Binder.ProcessedFieldInitializers);
                ImmutableArray<MethodSymbol>.Enumerator enumerator = AnonymousTypeManager.GetAnonymousTypeHiddenMethods(containingType).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MethodSymbol current = enumerator.Current;
                    CompileMethod(current, -1, ref processedInitializers4, synthesizedSubmissionFields, typeCompilationState);
                }
            }
            if (_moduleBeingBuiltOpt != null && !flag && !processedInitializers.BoundInitializers.IsDefaultOrEmpty)
            {
                MethodSymbol methodSymbol2 = new SynthesizedStaticConstructor(sourceMemberContainerTypeSymbol);
                if (PassesFilter(_filterOpt, methodSymbol2))
                {
                    CompileMethod(methodSymbol2, -1, ref processedInitializers, synthesizedSubmissionFields, typeCompilationState);
                    if (_moduleBeingBuiltOpt.GetMethodBody(methodSymbol2) != null)
                    {
                        _moduleBeingBuiltOpt.AddSynthesizedDefinition(sourceMemberContainerTypeSymbol, methodSymbol2.GetCciAdapter());
                    }
                }
            }
            bool flag2 = !flag && processedInitializers.BoundInitializers.IsDefaultOrEmpty && _compilation.LanguageVersion >= MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion();
            bool flag3;
            if (flag2)
            {
                if ((object)containingType != null && !containingType.IsImplicitlyDeclared)
                {
                    TypeKind typeKind = containingType.TypeKind;
                    if (typeKind == TypeKind.Class || typeKind == TypeKind.Interface || typeKind == TypeKind.Struct)
                    {
                        flag3 = true;
                        goto IL_03f7;
                    }
                }
                flag3 = false;
                goto IL_03f7;
            }
            goto IL_03fb;
        IL_03f7:
            flag2 = flag3;
            goto IL_03fb;
        IL_03fb:
            if (flag2)
            {
                NullableWalker.AnalyzeIfNeeded(_compilation, new SynthesizedStaticConstructor(containingType), GetSynthesizedEmptyBody(containingType), _diagnostics.DiagnosticBag, useConstructorExitWarnings: true, null, getFinalNullableState: false, out var _);
            }
            if (synthesizedInstanceConstructor != null && typeCompilationState.Emitting)
            {
                Binder.ProcessedFieldInitializers processedFieldInitializers = default(Binder.ProcessedFieldInitializers);
                processedFieldInitializers.BoundInitializers = ImmutableArray<BoundInitializer>.Empty;
                Binder.ProcessedFieldInitializers processedInitializers5 = processedFieldInitializers;
                CompileMethod(synthesizedInstanceConstructor, methodOrdinal, ref processedInitializers5, synthesizedSubmissionFields, typeCompilationState);
                synthesizedSubmissionFields?.AddToType(containingType, typeCompilationState.ModuleBuilderOpt);
            }
            if (_moduleBeingBuiltOpt != null)
            {
                CompileSynthesizedMethods(typeCompilationState);
            }
            typeCompilationState.Free();
        }

        private void CompileSynthesizedMethods(PrivateImplementationDetails privateImplClass, BindingDiagnosticBag diagnostics)
        {
            TypeCompilationState typeCompilationState = new TypeCompilationState(null, _compilation, _moduleBeingBuiltOpt);
            foreach (IMethodDefinition method in privateImplClass.GetMethods(new EmitContext(_moduleBeingBuiltOpt, null, diagnostics.DiagnosticBag, metadataOnly: false, includePrivateMembers: true)))
            {
                ((MethodSymbol)method.GetInternalSymbol()).GenerateMethodBody(typeCompilationState, diagnostics);
            }
            CompileSynthesizedMethods(typeCompilationState);
            typeCompilationState.Free();
        }

        private void CompileSynthesizedMethods(ImmutableArray<NamedTypeSymbol> additionalTypes, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = additionalTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                TypeCompilationState typeCompilationState = new TypeCompilationState(current, _compilation, _moduleBeingBuiltOpt);
                foreach (MethodSymbol item in current.GetMethodsToEmit())
                {
                    item.GenerateMethodBody(typeCompilationState, diagnostics);
                }
                if (!diagnostics.HasAnyErrors())
                {
                    CompileSynthesizedMethods(typeCompilationState);
                }
                typeCompilationState.Free();
            }
        }

        private void CompileSynthesizedMethods(TypeCompilationState compilationState)
        {
            ArrayBuilder<TypeCompilationState.MethodWithBody> synthesizedMethods = compilationState.SynthesizedMethods;
            if (synthesizedMethods == null)
            {
                return;
            }
            ImportChain currentImportChain = compilationState.CurrentImportChain;
            try
            {
                ArrayBuilder<TypeCompilationState.MethodWithBody>.Enumerator enumerator = synthesizedMethods.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeCompilationState.MethodWithBody current = enumerator.Current;
                    ImportChain importChain = (compilationState.CurrentImportChain = current.ImportChain);
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
                    MethodSymbol method = current.Method;
                    VariableSlotAllocator variableSlotAllocator = ((method is SynthesizedClosureMethod synthesizedClosureMethod) ? _moduleBeingBuiltOpt.TryCreateVariableSlotAllocator(synthesizedClosureMethod, synthesizedClosureMethod.TopLevelMethod, instance.DiagnosticBag) : _moduleBeingBuiltOpt.TryCreateVariableSlotAllocator(method, method, instance.DiagnosticBag));
                    MethodBody methodBody = null;
                    try
                    {
                        BoundStatement boundStatement = IteratorRewriter.Rewrite(current.Body, method, -1, variableSlotAllocator, compilationState, instance, out IteratorStateMachine stateMachineType);
                        StateMachineTypeSymbol stateMachineTypeSymbol = stateMachineType;
                        if (!boundStatement.HasErrors)
                        {
                            boundStatement = AsyncRewriter.Rewrite(boundStatement, method, -1, variableSlotAllocator, compilationState, instance, out var stateMachineType2);
                            stateMachineTypeSymbol = stateMachineTypeSymbol ?? stateMachineType2;
                        }
                        if (_emitMethodBodies && !instance.HasAnyErrors() && !_globalHasErrors)
                        {
                            methodBody = GenerateMethodBody(_moduleBeingBuiltOpt, method, -1, boundStatement, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, stateMachineTypeSymbol, variableSlotAllocator, instance, _debugDocumentProvider, method.GenerateDebugInfo ? importChain : null, _emittingPdb, _emitTestCoverageData, ImmutableArray<SourceSpan>.Empty, _entryPointOpt);
                        }
                    }
                    catch (BoundTreeVisitor.CancelledByStackGuardException ex)
                    {
                        ex.AddAnError(_diagnostics);
                    }
                    _diagnostics.AddRange(instance);
                    instance.Free();
                    if (_emitMethodBodies)
                    {
                        if (methodBody == null)
                        {
                            break;
                        }
                        _moduleBeingBuiltOpt.SetMethodBody(method, methodBody);
                    }
                }
            }
            finally
            {
                compilationState.CurrentImportChain = currentImportChain;
            }
        }

        private static bool IsFieldLikeEventAccessor(MethodSymbol method)
        {
            Symbol associatedSymbol = method.AssociatedSymbol;
            if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Event)
            {
                return ((EventSymbol)associatedSymbol).HasAssociatedField;
            }
            return false;
        }

        private void CompileSynthesizedExplicitImplementations(SourceMemberContainerTypeSymbol sourceTypeSymbol, TypeCompilationState compilationState)
        {
            if (!_globalHasErrors)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
                ImmutableArray<SynthesizedExplicitImplementationForwardingMethod>.Enumerator enumerator = sourceTypeSymbol.GetSynthesizedExplicitImplementations(_cancellationToken).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SynthesizedExplicitImplementationForwardingMethod current = enumerator.Current;
                    current.GenerateMethodBody(compilationState, instance);
                    instance.DiagnosticBag!.Clear();
                    _moduleBeingBuiltOpt.AddSynthesizedDefinition(sourceTypeSymbol, current.GetCciAdapter());
                }
                _diagnostics.AddRangeAndFree(instance);
            }
        }

        private void CompileSynthesizedSealedAccessors(SourcePropertySymbolBase sourceProperty, TypeCompilationState compilationState)
        {
            SynthesizedSealedPropertyAccessor synthesizedSealedAccessorOpt = sourceProperty.SynthesizedSealedAccessorOpt;
            if ((object)synthesizedSealedAccessorOpt != null && !_globalHasErrors)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
                synthesizedSealedAccessorOpt.GenerateMethodBody(compilationState, instance);
                _diagnostics.AddDependencies(instance);
                instance.Free();
                _moduleBeingBuiltOpt.AddSynthesizedDefinition(sourceProperty.ContainingType, synthesizedSealedAccessorOpt.GetCciAdapter());
            }
        }

        private void CompileFieldLikeEventAccessor(SourceEventSymbol eventSymbol, bool isAddMethod)
        {
            MethodSymbol methodSymbol = (isAddMethod ? eventSymbol.AddMethod : eventSymbol.RemoveMethod);
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
            try
            {
                BoundBlock block = MethodBodySynthesizer.ConstructFieldLikeEventAccessorBody(eventSymbol, isAddMethod, _compilation, instance);
                bool flag = instance.HasAnyErrors();
                SetGlobalErrorIfTrue(flag);
                if (!flag && !_hasDeclarationErrors && _emitMethodBodies)
                {
                    MethodBody body = GenerateMethodBody(_moduleBeingBuiltOpt, methodSymbol, -1, block, ImmutableArray<LambdaDebugInfo>.Empty, ImmutableArray<ClosureDebugInfo>.Empty, null, null, instance, _debugDocumentProvider, null, emittingPdb: false, _emitTestCoverageData, ImmutableArray<SourceSpan>.Empty, null);
                    _moduleBeingBuiltOpt.SetMethodBody(methodSymbol, body);
                }
            }
            finally
            {
                _diagnostics.AddRange(instance);
                instance.Free();
            }
        }

        public override object VisitMethod(MethodSymbol symbol, TypeCompilationState arg)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override object VisitProperty(PropertySymbol symbol, TypeCompilationState argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override object VisitEvent(EventSymbol symbol, TypeCompilationState argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override object VisitField(FieldSymbol symbol, TypeCompilationState argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private void CompileMethod(MethodSymbol methodSymbol, int methodOrdinal, ref Binder.ProcessedFieldInitializers processedInitializers, SynthesizedSubmissionFields previousSubmissionFields, TypeCompilationState compilationState)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            SourceMemberMethodSymbol sourceMemberMethodSymbol = methodSymbol as SourceMemberMethodSymbol;
            if (!methodSymbol.IsAbstract)
            {
                NamedTypeSymbol containingType = methodSymbol.ContainingType;
                if ((object)containingType == null || !containingType.IsDelegateType())
                {
                    if (_moduleBeingBuiltOpt == null && (object)sourceMemberMethodSymbol != null)
                    {
                        ImmutableArray<Diagnostic> diagnostics = sourceMemberMethodSymbol.Diagnostics;
                        if (!diagnostics.IsDefault)
                        {
                            _diagnostics.AddRange(diagnostics);
                            return;
                        }
                    }
                    ImportChain currentImportChain = compilationState.CurrentImportChain;
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
                    try
                    {
                        if (methodSymbol.SynthesizesLoweredBoundBody)
                        {
                            if (_moduleBeingBuiltOpt != null)
                            {
                                methodSymbol.GenerateMethodBody(compilationState, instance);
                                _diagnostics.AddRange(instance);
                            }
                        }
                        else
                        {
                            if (methodSymbol.IsDefaultValueTypeConstructor())
                            {
                                return;
                            }
                            bool flag = false;
                            bool originalBodyNested = false;
                            BoundStatementList boundStatementList = null;
                            MethodBodySemanticModel.InitialState forSemanticModel = default(MethodBodySemanticModel.InitialState);
                            ImportChain importChain = null;
                            bool hasTrailingExpression = false;
                            BoundBlock boundBlock;
                            if (methodSymbol.IsScriptConstructor)
                            {
                                boundBlock = new BoundBlock(methodSymbol.GetNonNullSyntaxNode(), ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty)
                                {
                                    WasCompilerGenerated = true
                                };
                            }
                            else if (methodSymbol.IsScriptInitializer)
                            {
                                BoundTypeOrInstanceInitializers boundTypeOrInstanceInitializers = InitializerRewriter.RewriteScriptInitializer(processedInitializers.BoundInitializers, (SynthesizedInteractiveInitializerMethod)methodSymbol, out hasTrailingExpression);
                                boundBlock = BoundBlock.SynthesizedNoLocals(boundTypeOrInstanceInitializers.Syntax, boundTypeOrInstanceInitializers.Statements);
                                NullableWalker.AnalyzeIfNeeded(_compilation, methodSymbol, boundTypeOrInstanceInitializers, instance.DiagnosticBag, useConstructorExitWarnings: false, null, getFinalNullableState: true, out processedInitializers.AfterInitializersState);
                                DiagnosticBag instance2 = DiagnosticBag.GetInstance();
                                DefiniteAssignmentPass.Analyze(_compilation, methodSymbol, boundTypeOrInstanceInitializers, instance2, requireOutParamsAssigned: false);
                                DiagnosticsPass.IssueDiagnostics(_compilation, boundTypeOrInstanceInitializers, BindingDiagnosticBag.Discarded, methodSymbol);
                                instance2.Free();
                            }
                            else
                            {
                                bool num = methodSymbol.IncludeFieldInitializersInBody();
                                flag = num && !processedInitializers.BoundInitializers.IsDefaultOrEmpty;
                                if (flag && processedInitializers.LoweredInitializers == null)
                                {
                                    boundStatementList = InitializerRewriter.RewriteConstructor(processedInitializers.BoundInitializers, methodSymbol);
                                    processedInitializers.HasErrors = processedInitializers.HasErrors || boundStatementList.HasAnyErrors;
                                }
                                if (num && processedInitializers.AfterInitializersState == null)
                                {
                                    NullableWalker.AnalyzeIfNeeded(_compilation, methodSymbol, boundStatementList ?? GetSynthesizedEmptyBody(methodSymbol), instance.DiagnosticBag, useConstructorExitWarnings: false, null, getFinalNullableState: true, out processedInitializers.AfterInitializersState);
                                }
                                boundBlock = BindMethodBody(methodSymbol, compilationState, instance, processedInitializers.AfterInitializersState, out importChain, out originalBodyNested, out forSemanticModel);
                                if (instance.HasAnyErrors() && boundBlock != null)
                                {
                                    boundBlock = (BoundBlock)boundBlock.WithHasErrors();
                                }
                                if (flag && processedInitializers.LoweredInitializers == null)
                                {
                                    if (boundBlock != null && ((methodSymbol.ContainingType.IsStructType() && !methodSymbol.IsImplicitConstructor) || methodSymbol is SynthesizedRecordConstructor || _emitTestCoverageData))
                                    {
                                        if (_emitTestCoverageData && methodSymbol.IsImplicitConstructor)
                                        {
                                            DefiniteAssignmentPass.Analyze(_compilation, methodSymbol, boundStatementList, instance.DiagnosticBag, requireOutParamsAssigned: false);
                                        }
                                        boundBlock = boundBlock.Update(boundBlock.Locals, boundBlock.LocalFunctions, boundBlock.Statements.Insert(0, boundStatementList));
                                        flag = false;
                                        boundStatementList = null;
                                    }
                                    else
                                    {
                                        DefiniteAssignmentPass.Analyze(_compilation, methodSymbol, boundStatementList, instance.DiagnosticBag, requireOutParamsAssigned: false);
                                        DiagnosticsPass.IssueDiagnostics(_compilation, boundStatementList, instance, methodSymbol);
                                    }
                                }
                            }
                            importChain = (compilationState.CurrentImportChain = importChain ?? processedInitializers.FirstImportChain);
                            if (boundBlock != null)
                            {
                                DiagnosticsPass.IssueDiagnostics(_compilation, boundBlock, instance, methodSymbol);
                            }
                            BoundBlock boundBlock2 = null;
                            if (boundBlock != null)
                            {
                                boundBlock2 = FlowAnalysisPass.Rewrite(methodSymbol, boundBlock, instance.DiagnosticBag, hasTrailingExpression, originalBodyNested);
                            }
                            bool flag2 = _hasDeclarationErrors || instance.HasAnyErrors() || processedInitializers.HasErrors;
                            SetGlobalErrorIfTrue(flag2);
                            bool diagsWritten = false;
                            ImmutableBindingDiagnostic<AssemblySymbol> other = instance.ToReadOnly();
                            if (sourceMemberMethodSymbol != null)
                            {
                                other = new ImmutableBindingDiagnostic<AssemblySymbol>(sourceMemberMethodSymbol.SetDiagnostics(other.Diagnostics, out diagsWritten), other.Dependencies);
                            }
                            if (diagsWritten && !methodSymbol.IsImplicitlyDeclared && _compilation.EventQueue != null)
                            {
                                SyntaxTreeSemanticModel semanticModelWithCachedBoundNodes = null;
                                if (boundBlock != null)
                                {
                                    CSharpSyntaxNode syntax = forSemanticModel.Syntax;
                                    if (syntax != null && _compilation.SemanticModelProvider is CachingSemanticModelProvider cachingSemanticModelProvider)
                                    {
                                        SyntaxNode syntax2 = boundBlock.Syntax;
                                        semanticModelWithCachedBoundNodes = (SyntaxTreeSemanticModel)cachingSemanticModelProvider.GetSemanticModel(syntax2.SyntaxTree, _compilation);
                                        semanticModelWithCachedBoundNodes.GetOrAddModel(syntax, (CSharpSyntaxNode rootSyntax) => MethodBodySemanticModel.Create(semanticModelWithCachedBoundNodes, methodSymbol, forSemanticModel));
                                    }
                                }
                                _compilation.EventQueue!.TryEnqueue(new SymbolDeclaredCompilationEvent(_compilation, methodSymbol.GetPublicSymbol(), semanticModelWithCachedBoundNodes));
                            }
                            if (!(_moduleBeingBuiltOpt == null || flag2))
                            {
                                ImmutableArray<SourceSpan> dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
                                bool flag3 = boundBlock2 != null;
                                VariableSlotAllocator lazyVariableSlotAllocator = null;
                                StateMachineTypeSymbol stateMachineTypeOpt = null;
                                ArrayBuilder<LambdaDebugInfo> instance3 = ArrayBuilder<LambdaDebugInfo>.GetInstance();
                                ArrayBuilder<ClosureDebugInfo> instance4 = ArrayBuilder<ClosureDebugInfo>.GetInstance();
                                BoundStatement boundStatement = null;
                                try
                                {
                                    boundStatement = ((!flag3) ? null : LowerBodyOrInitializer(methodSymbol, methodOrdinal, boundBlock2, previousSubmissionFields, compilationState, _emitTestCoverageData, _debugDocumentProvider, ref dynamicAnalysisSpans, instance, ref lazyVariableSlotAllocator, instance3, instance4, out stateMachineTypeOpt));
                                    flag2 = flag2 || (flag3 && boundStatement.HasErrors) || instance.HasAnyErrors();
                                    SetGlobalErrorIfTrue(flag2);
                                    if (!flag2 && (flag3 || flag))
                                    {
                                        ImmutableArray<BoundStatement> immutableArray;
                                        if (methodSymbol.IsScriptConstructor)
                                        {
                                            immutableArray = MethodBodySynthesizer.ConstructScriptConstructorBody(boundStatement, methodSymbol, previousSubmissionFields, _compilation);
                                        }
                                        else
                                        {
                                            immutableArray = ImmutableArray<BoundStatement>.Empty;
                                            if (boundStatementList != null)
                                            {
                                                BoundStatement boundStatement3 = (processedInitializers.LoweredInitializers = LowerBodyOrInitializer(methodSymbol, methodOrdinal, boundStatementList, previousSubmissionFields, compilationState, _emitTestCoverageData, _debugDocumentProvider, ref dynamicAnalysisSpans, instance, ref lazyVariableSlotAllocator, instance3, instance4, out StateMachineTypeSymbol stateMachineTypeOpt2));
                                                flag2 = boundStatement3.HasAnyErrors || instance.HasAnyErrors();
                                                SetGlobalErrorIfTrue(flag2);
                                                if (flag2)
                                                {
                                                    _diagnostics.AddRange(instance);
                                                    return;
                                                }
                                                processedInitializers.LoweredInitializers = (BoundStatementList)boundStatement3;
                                            }
                                            if (flag)
                                            {
                                                if (processedInitializers.LoweredInitializers!.Kind == BoundKind.StatementList)
                                                {
                                                    BoundStatementList boundStatementList2 = (BoundStatementList)processedInitializers.LoweredInitializers;
                                                    immutableArray = immutableArray.Concat(boundStatementList2.Statements);
                                                }
                                                else
                                                {
                                                    immutableArray = immutableArray.Add(processedInitializers.LoweredInitializers);
                                                }
                                            }
                                            if (flag3)
                                            {
                                                immutableArray = immutableArray.Concat(ImmutableArray.Create(boundStatement));
                                            }
                                        }
                                        if (_emitMethodBodies && (!(methodSymbol is SynthesizedStaticConstructor synthesizedStaticConstructor) || synthesizedStaticConstructor.ShouldEmit(processedInitializers.BoundInitializers)))
                                        {
                                            BoundStatementList block = BoundStatementList.Synthesized(methodSymbol.GetNonNullSyntaxNode(), immutableArray);
                                            MethodBody body = GenerateMethodBody(_moduleBeingBuiltOpt, methodSymbol, methodOrdinal, block, instance3.ToImmutable(), instance4.ToImmutable(), stateMachineTypeOpt, lazyVariableSlotAllocator, instance, _debugDocumentProvider, importChain, _emittingPdb, _emitTestCoverageData, dynamicAnalysisSpans, null);
                                            _moduleBeingBuiltOpt.SetMethodBody(methodSymbol.PartialDefinitionPart ?? methodSymbol, body);
                                        }
                                    }
                                    _diagnostics.AddRange(instance);
                                    return;
                                }
                                finally
                                {
                                    instance3.Free();
                                    instance4.Free();
                                }
                            }
                            _diagnostics.AddRange(other);
                        }
                        return;
                    }
                    finally
                    {
                        instance.Free();
                        compilationState.CurrentImportChain = currentImportChain;
                    }
                }
            }
            if ((object)sourceMemberMethodSymbol != null)
            {
                sourceMemberMethodSymbol.SetDiagnostics(ImmutableArray<Diagnostic>.Empty, out var diagsWritten2);
                if (diagsWritten2 && !methodSymbol.IsImplicitlyDeclared && _compilation.EventQueue != null)
                {
                    _compilation.SymbolDeclaredEvent(methodSymbol);
                }
            }
        }

        internal static BoundStatement LowerBodyOrInitializer(MethodSymbol method, int methodOrdinal, BoundStatement body, SynthesizedSubmissionFields previousSubmissionFields, TypeCompilationState compilationState, bool instrumentForDynamicAnalysis, DebugDocumentProvider debugDocumentProvider, ref ImmutableArray<SourceSpan> dynamicAnalysisSpans, BindingDiagnosticBag diagnostics, ref VariableSlotAllocator lazyVariableSlotAllocator, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, ArrayBuilder<ClosureDebugInfo> closureDebugInfoBuilder, out StateMachineTypeSymbol stateMachineTypeOpt)
        {
            stateMachineTypeOpt = null;
            if (body.HasErrors)
            {
                return body;
            }
            try
            {
                BoundStatement boundStatement = LocalRewriter.Rewrite(method.DeclaringCompilation, method, methodOrdinal, method.ContainingType, body, compilationState, previousSubmissionFields, allowOmissionOfConditionalCalls: true, instrumentForDynamicAnalysis, ref dynamicAnalysisSpans, debugDocumentProvider, diagnostics, out bool sawLambdas, out bool sawLocalFunctions, out bool sawAwaitInExceptionHandler);
                if (boundStatement.HasErrors)
                {
                    return boundStatement;
                }
                if (sawAwaitInExceptionHandler)
                {
                    boundStatement = AsyncExceptionHandlerRewriter.Rewrite(method, method.ContainingType, boundStatement, compilationState, diagnostics);
                }
                if (boundStatement.HasErrors)
                {
                    return boundStatement;
                }
                if (lazyVariableSlotAllocator == null)
                {
                    lazyVariableSlotAllocator = compilationState.ModuleBuilderOpt!.TryCreateVariableSlotAllocator(method, method, diagnostics.DiagnosticBag);
                }
                BoundStatement boundStatement2 = boundStatement;
                if (sawLambdas || sawLocalFunctions)
                {
                    boundStatement2 = ClosureConversion.Rewrite(boundStatement, method.ContainingType, method.ThisParameter, method, methodOrdinal, null, lambdaDebugInfoBuilder, closureDebugInfoBuilder, lazyVariableSlotAllocator, compilationState, diagnostics, null);
                }
                if (boundStatement2.HasErrors)
                {
                    return boundStatement2;
                }
                BoundStatement boundStatement3 = IteratorRewriter.Rewrite(boundStatement2, method, methodOrdinal, lazyVariableSlotAllocator, compilationState, diagnostics, out IteratorStateMachine stateMachineType);
                if (boundStatement3.HasErrors)
                {
                    return boundStatement3;
                }
                BoundStatement result = AsyncRewriter.Rewrite(boundStatement3, method, methodOrdinal, lazyVariableSlotAllocator, compilationState, diagnostics, out AsyncStateMachine stateMachineType2);
                stateMachineTypeOpt = (StateMachineTypeSymbol)(stateMachineType ?? ((object)stateMachineType2));
                return result;
            }
            catch (BoundTreeVisitor.CancelledByStackGuardException ex)
            {
                ex.AddAnError(diagnostics);
                return new BoundBadStatement(body.Syntax, ImmutableArray.Create((BoundNode)body), hasErrors: true);
            }
        }

        private static MethodBody GenerateMethodBody(PEModuleBuilder moduleBuilder, MethodSymbol method, int methodOrdinal, BoundStatement block, ImmutableArray<LambdaDebugInfo> lambdaDebugInfo, ImmutableArray<ClosureDebugInfo> closureDebugInfo, StateMachineTypeSymbol stateMachineTypeOpt, VariableSlotAllocator variableSlotAllocatorOpt, BindingDiagnosticBag diagnostics, DebugDocumentProvider debugDocumentProvider, ImportChain importChainOpt, bool emittingPdb, bool emitTestCoverageData, ImmutableArray<SourceSpan> dynamicAnalysisSpans, SynthesizedEntryPointSymbol.AsyncForwardEntryPoint entryPointOpt)
        {
            CSharpCompilation compilation = moduleBuilder.Compilation;
            LocalSlotManager localSlotManager = new LocalSlotManager(variableSlotAllocatorOpt);
            OptimizationLevel optimizationLevel = compilation.Options.OptimizationLevel;
            ILBuilder iLBuilder = new ILBuilder(moduleBuilder, localSlotManager, optimizationLevel, method.AreLocalsZeroed);
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, diagnostics.AccumulatesDependencies);
            try
            {
                StateMachineMoveNextBodyDebugInfo stateMachineMoveNextDebugInfoOpt = null;
                CodeGenerator codeGenerator = new CodeGenerator(method, block, iLBuilder, moduleBuilder, instance.DiagnosticBag, optimizationLevel, emittingPdb);
                if (instance.HasAnyErrors())
                {
                    return null;
                }
                bool flag;
                MethodSymbol kickoffMethod;
                if (method is SynthesizedStateMachineMethod synthesizedStateMachineMethod && method.Name == "MoveNext")
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
                bool hasStackAlloc;
                if (flag)
                {
                    codeGenerator.Generate(out var asyncCatchHandlerOffset, out var asyncYieldPoints, out var asyncResumePoints, out hasStackAlloc);
                    bool flag2 = entryPointOpt?.UserMain.Equals(kickoffMethod) ?? false;
                    stateMachineMoveNextDebugInfoOpt = new AsyncMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter(), (kickoffMethod.ReturnsVoid || flag2) ? asyncCatchHandlerOffset : (-1), asyncYieldPoints, asyncResumePoints);
                }
                else
                {
                    codeGenerator.Generate(out hasStackAlloc);
                    if ((object)kickoffMethod != null)
                    {
                        stateMachineMoveNextDebugInfoOpt = new IteratorMoveNextBodyDebugInfo(kickoffMethod.GetCciAdapter());
                    }
                }
                ImmutableArray<StateMachineHoistedLocalScope> stateMachineHoistedLocalScopes = (((object)kickoffMethod != null) ? iLBuilder.GetHoistedLocalScopes() : default(ImmutableArray<StateMachineHoistedLocalScope>));
                IImportScope importScopeOpt = importChainOpt?.Translate(moduleBuilder, instance.DiagnosticBag);
                ImmutableArray<ILocalDefinition> locals = iLBuilder.LocalSlotManager.LocalsInOrder();
                if (locals.Length > 65534)
                {
                    instance.Add(ErrorCode.ERR_TooManyLocals, method.Locations.First());
                }
                if (instance.HasAnyErrors())
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
                    GetStateMachineSlotDebugInfo(moduleBuilder, moduleBuilder.GetSynthesizedFields(stateMachineTypeOpt), variableSlotAllocatorOpt, instance, out hoistedVariableSlots, out awaiterSlots);
                }
                DynamicAnalysisMethodBodyData dynamicAnalysisDataOpt = null;
                if (emitTestCoverageData)
                {
                    dynamicAnalysisDataOpt = new DynamicAnalysisMethodBodyData(dynamicAnalysisSpans);
                }
                return new MethodBody(iLBuilder.RealizedIL, iLBuilder.MaxStack, (method.PartialDefinitionPart ?? method).GetCciAdapter(), variableSlotAllocatorOpt?.MethodId ?? new DebugId(methodOrdinal, moduleBuilder.CurrentGenerationOrdinal), locals, iLBuilder.RealizedSequencePoints, debugDocumentProvider, iLBuilder.RealizedExceptionHandlers, iLBuilder.AreLocalsZeroed, hasStackAlloc, iLBuilder.GetAllScopes(), iLBuilder.HasDynamicLocal, importScopeOpt, lambdaDebugInfo, closureDebugInfo, stateMachineTypeOpt?.Name, stateMachineHoistedLocalScopes, hoistedVariableSlots, awaiterSlots, stateMachineMoveNextDebugInfoOpt, dynamicAnalysisDataOpt);
            }
            finally
            {
                iLBuilder.FreeBasicBlocks();
                diagnostics.AddRange(instance);
                instance.Free();
            }
        }

        private static void GetStateMachineSlotDebugInfo(PEModuleBuilder moduleBuilder, IEnumerable<IFieldDefinition> fieldDefs, VariableSlotAllocator variableSlotAllocatorOpt, BindingDiagnosticBag diagnostics, out ImmutableArray<EncHoistedLocalInfo> hoistedVariableSlots, out ImmutableArray<ITypeReference> awaiterSlots)
        {
            ArrayBuilder<EncHoistedLocalInfo> instance = ArrayBuilder<EncHoistedLocalInfo>.GetInstance();
            ArrayBuilder<ITypeReference> instance2 = ArrayBuilder<ITypeReference>.GetInstance();
            foreach (StateMachineFieldSymbol fieldDef in fieldDefs)
            {
                int slotIndex = fieldDef.SlotIndex;
                if (fieldDef.SlotDebugInfo.SynthesizedKind == SynthesizedLocalKind.AwaiterField)
                {
                    while (slotIndex >= instance2.Count)
                    {
                        instance2.Add(null);
                    }
                    instance2[slotIndex] = moduleBuilder.EncTranslateLocalVariableType(fieldDef.Type, diagnostics.DiagnosticBag);
                }
                else if (!fieldDef.SlotDebugInfo.Id.IsNone)
                {
                    while (slotIndex >= instance.Count)
                    {
                        instance.Add(new EncHoistedLocalInfo(_: true));
                    }
                    instance[slotIndex] = new EncHoistedLocalInfo(fieldDef.SlotDebugInfo, moduleBuilder.EncTranslateLocalVariableType(fieldDef.Type, diagnostics.DiagnosticBag));
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

        internal static BoundBlock BindMethodBody(MethodSymbol method, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            return BindMethodBody(method, compilationState, diagnostics, null, out ImportChain importChain, out bool originalBodyNested, out MethodBodySemanticModel.InitialState forSemanticModel);
        }

        private static BoundBlock BindMethodBody(MethodSymbol method, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, NullableWalker.VariableState nullableInitialState, out ImportChain importChain, out bool originalBodyNested, out MethodBodySemanticModel.InitialState forSemanticModel)
        {
            originalBodyNested = false;
            importChain = null;
            forSemanticModel = default(MethodBodySemanticModel.InitialState);
            NullableWalker.VariableState finalNullableState;
            BoundBlock boundBlock;
            if (method is SynthesizedRecordConstructor synthesizedRecordConstructor && method.ContainingType.IsRecordStruct)
            {
                boundBlock = BoundBlock.SynthesizedNoLocals(synthesizedRecordConstructor.GetSyntax());
            }
            else if (method is SourceMemberMethodSymbol sourceMemberMethodSymbol)
            {
                CSharpSyntaxNode syntaxNode = sourceMemberMethodSymbol.SyntaxNode;
                if (method.MethodKind == MethodKind.StaticConstructor && syntaxNode is ConstructorDeclarationSyntax constructorDeclarationSyntax && constructorDeclarationSyntax.Initializer != null)
                {
                    diagnostics.Add(ErrorCode.ERR_StaticConstructorWithExplicitConstructorCall, constructorDeclarationSyntax.Initializer!.ThisOrBaseKeyword.GetLocation(), constructorDeclarationSyntax.Identifier.ValueText);
                }
                ExecutableCodeBinder executableCodeBinder = sourceMemberMethodSymbol.TryGetBodyBinder();
                if (sourceMemberMethodSymbol.IsExtern || sourceMemberMethodSymbol.IsDefaultValueTypeConstructor())
                {
                    return null;
                }
                if (executableCodeBinder == null)
                {
                    if (sourceMemberMethodSymbol.AssociatedSymbol is SourcePropertySymbolBase sourcePropertySymbolBase && sourcePropertySymbolBase.IsAutoPropertyWithGetAccessor)
                    {
                        return MethodBodySynthesizer.ConstructAutoPropertyAccessorBody(sourceMemberMethodSymbol);
                    }
                    return null;
                }
                importChain = executableCodeBinder.ImportChain;
                BoundNode boundNode = executableCodeBinder.BindMethodBody(syntaxNode, diagnostics);
                BoundNode bodyOpt = boundNode;
                NullableWalker.SnapshotManager snapshotManager = null;
                ImmutableDictionary<Symbol, Symbol> remappedSymbols = null;
                CSharpCompilation compilation = executableCodeBinder.Compilation;
                bool flag = compilation.LanguageVersion >= MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion();
                if (compilation.IsNullableAnalysisEnabledIn(method))
                {
                    bodyOpt = NullableWalker.AnalyzeAndRewrite(compilation, method, boundNode, executableCodeBinder, nullableInitialState, flag ? diagnostics.DiagnosticBag : new DiagnosticBag(), createSnapshots: true, out snapshotManager, ref remappedSymbols);
                }
                else
                {
                    NullableWalker.AnalyzeIfNeeded(compilation, method, boundNode, diagnostics.DiagnosticBag, useConstructorExitWarnings: true, nullableInitialState, getFinalNullableState: false, out finalNullableState);
                }
                forSemanticModel = new MethodBodySemanticModel.InitialState(syntaxNode, bodyOpt, executableCodeBinder, snapshotManager, remappedSymbols);
                switch (boundNode.Kind)
                {
                    case BoundKind.ConstructorMethodBody:
                        {
                            BoundConstructorMethodBody boundConstructorMethodBody = (BoundConstructorMethodBody)boundNode;
                            boundBlock = boundConstructorMethodBody.BlockBody ?? boundConstructorMethodBody.ExpressionBody;
                            if (boundConstructorMethodBody.Initializer != null)
                            {
                                ReportCtorInitializerCycles(method, boundConstructorMethodBody.Initializer!.Expression, compilationState, diagnostics);
                                if (boundBlock == null)
                                {
                                    boundBlock = new BoundBlock(boundConstructorMethodBody.Syntax, boundConstructorMethodBody.Locals, ImmutableArray.Create((BoundStatement)boundConstructorMethodBody.Initializer));
                                }
                                else
                                {
                                    boundBlock = new BoundBlock(boundConstructorMethodBody.Syntax, boundConstructorMethodBody.Locals, ImmutableArray.Create(boundConstructorMethodBody.Initializer, (BoundStatement)boundBlock));
                                    originalBodyNested = true;
                                }
                                return boundBlock;
                            }
                            break;
                        }
                    case BoundKind.NonConstructorMethodBody:
                        {
                            BoundNonConstructorMethodBody boundNonConstructorMethodBody = (BoundNonConstructorMethodBody)boundNode;
                            boundBlock = boundNonConstructorMethodBody.BlockBody ?? boundNonConstructorMethodBody.ExpressionBody;
                            break;
                        }
                    case BoundKind.Block:
                        boundBlock = (BoundBlock)boundNode;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(boundNode.Kind);
                }
            }
            else if (method is SynthesizedInstanceConstructor synthesizedInstanceConstructor)
            {
                CSharpSyntaxNode nonNullSyntaxNode = synthesizedInstanceConstructor.GetNonNullSyntaxNode();
                SyntheticBoundNodeFactory factory = new SyntheticBoundNodeFactory(synthesizedInstanceConstructor, nonNullSyntaxNode, compilationState, diagnostics);
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                synthesizedInstanceConstructor.GenerateMethodBodyStatements(factory, instance, diagnostics);
                boundBlock = BoundBlock.SynthesizedNoLocals(nonNullSyntaxNode, instance.ToImmutableAndFree());
            }
            else
            {
                boundBlock = null;
            }
            if (method.IsConstructor() && method.IsImplicitlyDeclared && nullableInitialState != null)
            {
                NullableWalker.AnalyzeIfNeeded(compilationState.Compilation, method, boundBlock ?? GetSynthesizedEmptyBody(method), diagnostics.DiagnosticBag, useConstructorExitWarnings: true, nullableInitialState, getFinalNullableState: false, out finalNullableState);
            }
            if (method.MethodKind == MethodKind.Destructor && boundBlock != null)
            {
                return MethodBodySynthesizer.ConstructDestructorBody(method, boundBlock);
            }
            BoundStatement boundStatement = BindImplicitConstructorInitializerIfAny(method, compilationState, diagnostics);
            ImmutableArray<BoundStatement> statements;
            if (boundStatement == null)
            {
                if (boundBlock != null)
                {
                    return boundBlock;
                }
                statements = ImmutableArray<BoundStatement>.Empty;
            }
            else if (boundBlock == null)
            {
                statements = ImmutableArray.Create(boundStatement);
            }
            else
            {
                statements = ImmutableArray.Create(boundStatement, boundBlock);
                originalBodyNested = true;
            }
            return BoundBlock.SynthesizedNoLocals(method.GetNonNullSyntaxNode(), statements);
        }

        private static BoundBlock GetSynthesizedEmptyBody(Symbol symbol)
        {
            return BoundBlock.SynthesizedNoLocals(symbol.GetNonNullSyntaxNode());
        }

        private static BoundStatement BindImplicitConstructorInitializerIfAny(MethodSymbol method, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            if (method.MethodKind == MethodKind.Constructor && !method.IsExtern)
            {
                CSharpCompilation declaringCompilation = method.DeclaringCompilation;
                BoundExpression boundExpression = BindImplicitConstructorInitializer(method, diagnostics, declaringCompilation);
                if (boundExpression != null)
                {
                    ReportCtorInitializerCycles(method, boundExpression, compilationState, diagnostics);
                    return new BoundExpressionStatement(boundExpression.Syntax, boundExpression)
                    {
                        WasCompilerGenerated = method.IsImplicitlyDeclared
                    };
                }
            }
            return null;
        }

        private static void ReportCtorInitializerCycles(MethodSymbol method, BoundExpression initializerInvocation, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            if (initializerInvocation is BoundCall boundCall && !boundCall.HasAnyErrors && boundCall.Method != method && TypeSymbol.Equals(boundCall.Method.ContainingType, method.ContainingType, TypeCompareKind.ConsiderEverything))
            {
                compilationState.ReportCtorInitializerCycles(method, boundCall.Method, boundCall.Syntax, diagnostics);
            }
        }

        internal static BoundExpression BindImplicitConstructorInitializer(MethodSymbol constructor, BindingDiagnosticBag diagnostics, CSharpCompilation compilation)
        {
            NamedTypeSymbol containingType = constructor.ContainingType;
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
            SourceMemberMethodSymbol sourceMemberMethodSymbol = constructor as SourceMemberMethodSymbol;
            if ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                if (baseTypeNoUseSiteDiagnostics.SpecialType == SpecialType.System_Object)
                {
                    return GenerateBaseParameterlessConstructorInitializer(constructor, diagnostics);
                }
                if (baseTypeNoUseSiteDiagnostics.IsErrorType() || baseTypeNoUseSiteDiagnostics.IsStatic)
                {
                    return null;
                }
            }
            if (containingType.IsStructType() || containingType.IsEnumType())
            {
                return null;
            }
            if (constructor is SynthesizedRecordCopyCtor constructor2)
            {
                return GenerateBaseCopyConstructorInitializer(constructor2, diagnostics);
            }
            Binder binder;
            if ((object)sourceMemberMethodSymbol == null)
            {
                CSharpSyntaxNode nonNullSyntaxNode = constructor.GetNonNullSyntaxNode();
                BinderFactory binderFactory = compilation.GetBinderFactory(nonNullSyntaxNode.SyntaxTree);
                binder = ((!(nonNullSyntaxNode is RecordDeclarationSyntax typeDecl)) ? binderFactory.GetBinder(nonNullSyntaxNode, GetImplicitConstructorBodyToken(nonNullSyntaxNode).Position) : binderFactory.GetInRecordBodyBinder(typeDecl));
            }
            else
            {
                BinderFactory binderFactory2 = compilation.GetBinderFactory(sourceMemberMethodSymbol.SyntaxTree);
                CSharpSyntaxNode syntaxNode = sourceMemberMethodSymbol.SyntaxNode;
                if (!(syntaxNode is ConstructorDeclarationSyntax constructorDeclarationSyntax))
                {
                    if (!(syntaxNode is RecordDeclarationSyntax typeDecl2))
                    {
                        throw ExceptionUtilities.Unreachable;
                    }
                    binder = binderFactory2.GetInRecordBodyBinder(typeDecl2);
                }
                else
                {
                    binder = binderFactory2.GetBinder(constructorDeclarationSyntax.ParameterList);
                }
            }
            return binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.ConstructorInitializer, constructor).BindConstructorInitializer(null, constructor, diagnostics);
        }

        private static SyntaxToken GetImplicitConstructorBodyToken(CSharpSyntaxNode containerNode)
        {
            return ((BaseTypeDeclarationSyntax)containerNode).OpenBraceToken;
        }

        internal static BoundCall GenerateBaseParameterlessConstructorInitializer(MethodSymbol constructor, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = constructor.ContainingType.BaseTypeNoUseSiteDiagnostics;
            MethodSymbol methodSymbol = null;
            LookupResultKind resultKind = LookupResultKind.Viable;
            Location location = (constructor.Locations.IsEmpty ? NoLocation.Singleton : constructor.Locations[0]);
            ImmutableArray<MethodSymbol>.Enumerator enumerator = baseTypeNoUseSiteDiagnostics.InstanceConstructors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (current.ParameterCount == 0)
                {
                    methodSymbol = current;
                    break;
                }
            }
            if ((object)methodSymbol == null)
            {
                diagnostics.Add(ErrorCode.ERR_BadCtorArgCount, location, baseTypeNoUseSiteDiagnostics, 0);
                return null;
            }
            if (Binder.ReportUseSite(methodSymbol, diagnostics, location))
            {
                return null;
            }
            bool hasErrors = false;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, constructor.ContainingAssembly);
            if (!AccessCheck.IsSymbolAccessible(methodSymbol, constructor.ContainingType, ref useSiteInfo))
            {
                diagnostics.Add(ErrorCode.ERR_BadAccess, location, methodSymbol);
                resultKind = LookupResultKind.Inaccessible;
                hasErrors = true;
            }
            diagnostics.Add(location, useSiteInfo);
            CSharpSyntaxNode nonNullSyntaxNode = constructor.GetNonNullSyntaxNode();
            BoundExpression receiverOpt = new BoundThisReference(nonNullSyntaxNode, constructor.ContainingType)
            {
                WasCompilerGenerated = true
            };
            return new BoundCall(nonNullSyntaxNode, receiverOpt, methodSymbol, ImmutableArray<BoundExpression>.Empty, ImmutableArray<string>.Empty, ImmutableArray<RefKind>.Empty, isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, ImmutableArray<int>.Empty, BitVector.Empty, resultKind, methodSymbol.ReturnType, hasErrors)
            {
                WasCompilerGenerated = true
            };
        }

        private static BoundCall GenerateBaseCopyConstructorInitializer(SynthesizedRecordCopyCtor constructor, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol containingType = constructor.ContainingType;
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
            Location location = constructor.Locations.FirstOrNone();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, containingType.ContainingAssembly);
            MethodSymbol methodSymbol = SynthesizedRecordCopyCtor.FindCopyConstructor(baseTypeNoUseSiteDiagnostics, containingType, ref useSiteInfo);
            if ((object)methodSymbol == null)
            {
                diagnostics.Add(ErrorCode.ERR_NoCopyConstructorInBaseType, location, baseTypeNoUseSiteDiagnostics);
                return null;
            }
            if (Binder.ReportUseSite(methodSymbol, diagnostics, location))
            {
                return null;
            }
            diagnostics.Add(location, useSiteInfo);
            CSharpSyntaxNode nonNullSyntaxNode = constructor.GetNonNullSyntaxNode();
            BoundExpression receiverOpt = new BoundThisReference(nonNullSyntaxNode, constructor.ContainingType)
            {
                WasCompilerGenerated = true
            };
            BoundExpression item = new BoundParameter(nonNullSyntaxNode, constructor.Parameters[0]);
            return new BoundCall(nonNullSyntaxNode, receiverOpt, methodSymbol, ImmutableArray.Create(item), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, methodSymbol.ReturnType)
            {
                WasCompilerGenerated = true
            };
        }

        private static DebugSourceDocument CreateDebugDocumentForFile(string normalizedPath)
        {
            return new DebugSourceDocument(normalizedPath, DebugSourceDocument.CorSymLanguageTypeCSharp);
        }

        private static bool PassesFilter(Predicate<Symbol> filterOpt, Symbol symbol)
        {
            return filterOpt?.Invoke(symbol) ?? true;
        }
    }
}
