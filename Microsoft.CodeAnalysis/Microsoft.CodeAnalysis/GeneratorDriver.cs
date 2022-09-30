using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class GeneratorDriver
    {
        public readonly GeneratorDriverState _state;

        public abstract CommonMessageProvider MessageProvider { get; }

        public GeneratorDriver(GeneratorDriverState state)
        {
            _state = state;
        }

        public GeneratorDriver(ParseOptions parseOptions, ImmutableArray<ISourceGenerator> generators, AnalyzerConfigOptionsProvider optionsProvider, ImmutableArray<AdditionalText> additionalTexts)
        {
            _state = new GeneratorDriverState(parseOptions, optionsProvider, generators, additionalTexts, ImmutableArray.Create(new GeneratorState[generators.Length]), ImmutableArray<PendingEdit>.Empty, editsFailed: true);
        }

        public GeneratorDriver RunGenerators(Compilation compilation, CancellationToken cancellationToken = default(CancellationToken))
        {
            GeneratorDriverState state = RunGeneratorsCore(compilation, null, cancellationToken);
            return FromState(state);
        }

        public GeneratorDriver RunGeneratorsAndUpdateCompilation(Compilation compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken = default(CancellationToken))
        {
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            GeneratorDriverState state = RunGeneratorsCore(compilation, instance, cancellationToken);
            diagnostics = instance.ToReadOnlyAndFree();
            return BuildFinalCompilation(compilation, out outputCompilation, state, cancellationToken);
        }

        public GeneratorDriver TryApplyEdits(Compilation compilation, out Compilation outputCompilation, out bool success, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_state.EditsFailed || _state.Edits.Length == 0)
            {
                outputCompilation = compilation;
                success = !_state.EditsFailed;
                return this;
            }
            GeneratorDriverState state = _state;
            ImmutableArray<PendingEdit>.Enumerator enumerator = _state.Edits.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingEdit current = enumerator.Current;
                state = ApplyPartialEdit(state, current, cancellationToken);
                if (state.EditsFailed)
                {
                    outputCompilation = compilation;
                    success = false;
                    return this;
                }
            }
            compilation = RemoveGeneratedSyntaxTrees(_state, compilation);
            success = true;
            return BuildFinalCompilation(compilation, out outputCompilation, state, cancellationToken);
        }

        public GeneratorDriver AddGenerators(ImmutableArray<ISourceGenerator> generators)
        {
            ref readonly GeneratorDriverState state = ref _state;
            ImmutableArray<ISourceGenerator>? generators2 = _state.Generators.AddRange(generators);
            ImmutableArray<GeneratorState>? generatorStates = _state.GeneratorStates.AddRange(new GeneratorState[generators.Length]);
            bool? editsFailed = true;
            GeneratorDriverState state2 = state.With(generators2, generatorStates, null, null, editsFailed);
            return FromState(state2);
        }

        public GeneratorDriver RemoveGenerators(ImmutableArray<ISourceGenerator> generators)
        {
            ImmutableArray<ISourceGenerator> value = _state.Generators;
            ImmutableArray<GeneratorState> value2 = _state.GeneratorStates;
            for (int i = 0; i < value.Length; i++)
            {
                if (generators.Contains(value[i]))
                {
                    value = value.RemoveAt(i);
                    value2 = value2.RemoveAt(i);
                    i--;
                }
            }
            return FromState(_state.With(value, value2));
        }

        public GeneratorDriver AddAdditionalTexts(ImmutableArray<AdditionalText> additionalTexts)
        {
            ref readonly GeneratorDriverState state = ref _state;
            ImmutableArray<AdditionalText>? additionalTexts2 = _state.AdditionalTexts.AddRange(additionalTexts);
            GeneratorDriverState state2 = state.With(null, null, additionalTexts2);
            return FromState(state2);
        }

        public GeneratorDriver RemoveAdditionalTexts(ImmutableArray<AdditionalText> additionalTexts)
        {
            ref readonly GeneratorDriverState state = ref _state;
            ImmutableArray<AdditionalText>? additionalTexts2 = _state.AdditionalTexts.RemoveRange(additionalTexts);
            GeneratorDriverState state2 = state.With(null, null, additionalTexts2);
            return FromState(state2);
        }

        public GeneratorDriverRunResult GetRunResult()
        {
            return new GeneratorDriverRunResult(_state.Generators.ZipAsArray(_state.GeneratorStates, (ISourceGenerator generator, GeneratorState generatorState) => new GeneratorRunResult(generator, diagnostics: generatorState.Diagnostics, exception: generatorState.Exception, generatedSources: getGeneratorSources(generatorState))));
            static ImmutableArray<GeneratedSourceResult> getGeneratorSources(GeneratorState generatorState)
            {
                ArrayBuilder<GeneratedSourceResult> instance = ArrayBuilder<GeneratedSourceResult>.GetInstance(generatorState.PostInitTrees.Length + generatorState.GeneratedTrees.Length);
                ImmutableArray<GeneratedSyntaxTree>.Enumerator enumerator = generatorState.PostInitTrees.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GeneratedSyntaxTree current = enumerator.Current;
                    instance.Add(new GeneratedSourceResult(current.Tree, current.Text, current.HintName));
                }
                enumerator = generatorState.GeneratedTrees.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GeneratedSyntaxTree current2 = enumerator.Current;
                    instance.Add(new GeneratedSourceResult(current2.Tree, current2.Text, current2.HintName));
                }
                return instance.ToImmutableAndFree();
            }
        }

        public GeneratorDriverState RunGeneratorsCore(Compilation compilation, DiagnosticBag? diagnosticsBag, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_state.Generators.IsEmpty)
            {
                return _state;
            }
            GeneratorDriverState generatorDriverState = StateWithPendingEditsApplied(_state);
            ArrayBuilder<GeneratorState> instance = ArrayBuilder<GeneratorState>.GetInstance(generatorDriverState.Generators.Length);
            ArrayBuilder<SyntaxTree> instance2 = ArrayBuilder<SyntaxTree>.GetInstance();
            ArrayBuilder<GeneratorSyntaxWalker> instance3 = ArrayBuilder<GeneratorSyntaxWalker>.GetInstance(generatorDriverState.Generators.Length, null);
            int num = 0;
            for (int i = 0; i < generatorDriverState.Generators.Length; i++)
            {
                ISourceGenerator sourceGenerator = generatorDriverState.Generators[i];
                GeneratorState generatorState = generatorDriverState.GeneratorStates[i];
                if (!generatorState.Info.Initialized)
                {
                    GeneratorInitializationContext context = new GeneratorInitializationContext(cancellationToken);
                    Exception ex = null;
                    try
                    {
                        sourceGenerator.Initialize(context);
                    }
                    catch (Exception ex2) when (FatalError.ReportAndCatchUnlessCanceled(ex2, cancellationToken))
                    {
                        ex = ex2;
                    }
                    generatorState = ((ex == null) ? new GeneratorState(context.InfoBuilder.ToImmutable()) : SetGeneratorException(MessageProvider, GeneratorState.Uninitialized, sourceGenerator, ex, diagnosticsBag, isInit: true));
                    if (generatorState.Info.PostInitCallback != null)
                    {
                        AdditionalSourcesCollection additionalSourcesCollection = CreateSourcesCollection();
                        GeneratorPostInitializationContext obj = new GeneratorPostInitializationContext(additionalSourcesCollection, cancellationToken);
                        try
                        {
                            generatorState.Info.PostInitCallback!(obj);
                        }
                        catch (Exception ex3)
                        {
                            ex = ex3;
                        }
                        generatorState = ((ex == null) ? new GeneratorState(generatorState.Info, ParseAdditionalSources(sourceGenerator, additionalSourcesCollection.ToImmutableAndFree(), cancellationToken)) : SetGeneratorException(MessageProvider, generatorState, sourceGenerator, ex, diagnosticsBag, isInit: true));
                    }
                }
                if (generatorState.Info.SyntaxContextReceiverCreator != null && generatorState.Exception == null)
                {
                    ISyntaxContextReceiver syntaxContextReceiver = null;
                    try
                    {
                        syntaxContextReceiver = generatorState.Info.SyntaxContextReceiverCreator!();
                    }
                    catch (Exception e)
                    {
                        generatorState = SetGeneratorException(MessageProvider, generatorState, sourceGenerator, e, diagnosticsBag);
                    }
                    if (syntaxContextReceiver != null)
                    {
                        instance3.SetItem(i, new GeneratorSyntaxWalker(syntaxContextReceiver));
                        generatorState = generatorState.WithReceiver(syntaxContextReceiver);
                        num++;
                    }
                }
                if (generatorState.Exception == null && generatorState.PostInitTrees.Length > 0)
                {
                    instance2.AddRange(generatorState.PostInitTrees.Select((GeneratedSyntaxTree t) => t.Tree));
                }
                instance.Add(generatorState);
            }
            if (instance2.Count > 0)
            {
                compilation = compilation.AddSyntaxTrees(instance2);
            }
            instance2.Free();
            if (num > 0)
            {
                foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
                {
                    SyntaxNode root = syntaxTree.GetRoot(cancellationToken);
                    SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                    for (int j = 0; j < instance3.Count; j++)
                    {
                        GeneratorSyntaxWalker generatorSyntaxWalker = instance3[j];
                        if (generatorSyntaxWalker != null)
                        {
                            try
                            {
                                generatorSyntaxWalker.VisitWithModel(semanticModel, root);
                            }
                            catch (Exception e2)
                            {
                                instance[j] = SetGeneratorException(MessageProvider, instance[j], generatorDriverState.Generators[j], e2, diagnosticsBag);
                                instance3.SetItem(j, null);
                            }
                        }
                    }
                }
            }
            instance3.Free();
            for (int k = 0; k < generatorDriverState.Generators.Length; k++)
            {
                ISourceGenerator sourceGenerator2 = generatorDriverState.Generators[k];
                GeneratorState generatorState2 = instance[k];
                if (generatorState2.Exception == null)
                {
                    GeneratorExecutionContext context2 = new GeneratorExecutionContext(compilation, generatorDriverState.ParseOptions, generatorDriverState.AdditionalTexts.NullToEmpty(), generatorDriverState.OptionsProvider, generatorState2.SyntaxReceiver, CreateSourcesCollection(), cancellationToken);
                    try
                    {
                        sourceGenerator2.Execute(context2);
                    }
                    catch (Exception ex4) when (FatalError.ReportAndCatchUnlessCanceled(ex4, cancellationToken))
                    {
                        instance[k] = SetGeneratorException(MessageProvider, generatorState2, sourceGenerator2, ex4, diagnosticsBag);
                        continue;
                    }
                    (ImmutableArray<GeneratedSourceText> sources, ImmutableArray<Diagnostic> diagnostics) tuple = context2.ToImmutableAndFree();
                    ImmutableArray<GeneratedSourceText> item = tuple.sources;
                    ImmutableArray<Diagnostic> item2 = tuple.diagnostics;
                    item2 = FilterDiagnostics(compilation, item2, diagnosticsBag, cancellationToken);
                    instance[k] = new GeneratorState(generatorState2.Info, generatorState2.PostInitTrees, ParseAdditionalSources(sourceGenerator2, item, cancellationToken), item2);
                }
            }
            ImmutableArray<GeneratorState>? generatorStates = instance.ToImmutableAndFree();
            return generatorDriverState.With(null, generatorStates);
        }

        public GeneratorDriver WithPendingEdits(ImmutableArray<PendingEdit> edits)
        {
            ref readonly GeneratorDriverState state = ref _state;
            ImmutableArray<PendingEdit>? edits2 = _state.Edits.AddRange(edits);
            GeneratorDriverState state2 = state.With(null, null, null, edits2);
            return FromState(state2);
        }

        public GeneratorDriverState ApplyPartialEdit(GeneratorDriverState state, PendingEdit edit, CancellationToken cancellationToken = default(CancellationToken))
        {
            GeneratorDriverState generatorDriverState = state;
            PooledDictionary<ISourceGenerator, GeneratorState>.GetInstance();
            for (int i = 0; i < generatorDriverState.Generators.Length; i++)
            {
                ISourceGenerator generator = generatorDriverState.Generators[i];
                GeneratorState generatorState = generatorDriverState.GeneratorStates[i];
                if (edit.AcceptedBy(generatorState.Info))
                {
                    AdditionalSourcesCollection additionalSourcesCollection = CreateSourcesCollection();
                    additionalSourcesCollection.AddRange(generatorState.GeneratedTrees);
                    if (!edit.TryApply(context: new GeneratorEditContext(additionalSourcesCollection, cancellationToken), info: generatorState.Info))
                    {
                        bool? editsFailed = true;
                        return generatorDriverState.With(null, null, null, null, editsFailed);
                    }
                    ImmutableArray<GeneratedSourceText> generatedSources = additionalSourcesCollection.ToImmutableAndFree();
                    ImmutableArray<GeneratorState>? generatorStates = state.GeneratorStates.SetItem(i, new GeneratorState(generatorState.Info, generatorState.PostInitTrees, ParseAdditionalSources(generator, generatedSources, cancellationToken), ImmutableArray<Diagnostic>.Empty));
                    state = state.With(null, generatorStates);
                }
            }
            state = edit.Commit(state);
            return state;
        }

        private static GeneratorDriverState StateWithPendingEditsApplied(GeneratorDriverState state)
        {
            if (state.Edits.Length == 0)
            {
                return state;
            }
            ImmutableArray<PendingEdit>.Enumerator enumerator = state.Edits.GetEnumerator();
            while (enumerator.MoveNext())
            {
                state = enumerator.Current.Commit(state);
            }
            ImmutableArray<PendingEdit>? edits = ImmutableArray<PendingEdit>.Empty;
            bool? editsFailed = false;
            return state.With(null, null, null, edits, editsFailed);
        }

        private static Compilation RemoveGeneratedSyntaxTrees(GeneratorDriverState state, Compilation compilation)
        {
            ArrayBuilder<SyntaxTree> instance = ArrayBuilder<SyntaxTree>.GetInstance();
            ImmutableArray<GeneratorState>.Enumerator enumerator = state.GeneratorStates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<GeneratedSyntaxTree>.Enumerator enumerator2 = enumerator.Current.GeneratedTrees.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    GeneratedSyntaxTree current = enumerator2.Current;
                    if (current.Tree != null && compilation.ContainsSyntaxTree(current.Tree))
                    {
                        instance.Add(current.Tree);
                    }
                }
            }
            Compilation result = compilation.RemoveSyntaxTrees(instance);
            instance.Free();
            return result;
        }

        private ImmutableArray<GeneratedSyntaxTree> ParseAdditionalSources(ISourceGenerator generator, ImmutableArray<GeneratedSourceText> generatedSources, CancellationToken cancellationToken)
        {
            ArrayBuilder<GeneratedSyntaxTree> instance = ArrayBuilder<GeneratedSyntaxTree>.GetInstance(generatedSources.Length);
            generator.GetType();
            string filePathPrefixForGenerator = GetFilePathPrefixForGenerator(generator);
            ImmutableArray<GeneratedSourceText>.Enumerator enumerator = generatedSources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GeneratedSourceText current = enumerator.Current;
                SyntaxTree tree = ParseGeneratedSourceText(current, Path.Combine(filePathPrefixForGenerator, current.HintName), cancellationToken);
                instance.Add(new GeneratedSyntaxTree(current.HintName, current.Text, tree));
            }
            return instance.ToImmutableAndFree();
        }

        private GeneratorDriver BuildFinalCompilation(Compilation compilation, out Compilation outputCompilation, GeneratorDriverState state, CancellationToken cancellationToken)
        {
            ArrayBuilder<SyntaxTree> instance = ArrayBuilder<SyntaxTree>.GetInstance();
            ImmutableArray<GeneratorState>.Enumerator enumerator = state.GeneratorStates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GeneratorState current = enumerator.Current;
                instance.AddRange(current.PostInitTrees.Select((GeneratedSyntaxTree t) => t.Tree));
                instance.AddRange(current.GeneratedTrees.Select((GeneratedSyntaxTree t) => t.Tree));
            }
            outputCompilation = compilation.AddSyntaxTrees(instance);
            instance.Free();
            ImmutableArray<PendingEdit>? edits = ImmutableArray<PendingEdit>.Empty;
            bool? editsFailed = false;
            state = state.With(null, null, null, edits, editsFailed);
            return FromState(state);
        }

        private static GeneratorState SetGeneratorException(CommonMessageProvider provider, GeneratorState generatorState, ISourceGenerator generator, Exception e, DiagnosticBag? diagnosticBag, bool isInit = false)
        {
            int num = (isInit ? provider.WRN_GeneratorFailedDuringInitialization : provider.WRN_GeneratorFailedDuringGeneration);
            string text = string.Format(provider.GetDescription(num).ToString(CultureInfo.CurrentUICulture), e);
            Diagnostic diagnostic = Diagnostic.Create(new DiagnosticDescriptor(provider.GetIdForErrorCode(num), provider.GetTitle(num), provider.GetMessageFormat(num), "Compiler", DiagnosticSeverity.Warning, true, text, null, "AnalyzerException"), Location.None, generator.GetType().Name, e.GetType().Name, e.Message);
            diagnosticBag?.Add(diagnostic);
            return new GeneratorState(generatorState.Info, e, diagnostic);
        }

        private static ImmutableArray<Diagnostic> FilterDiagnostics(Compilation compilation, ImmutableArray<Diagnostic> generatorDiagnostics, DiagnosticBag? driverDiagnostics, CancellationToken cancellationToken)
        {
            ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance();
            ImmutableArray<Diagnostic>.Enumerator enumerator = generatorDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Diagnostic current = enumerator.Current;
                Diagnostic diagnostic = compilation.Options.FilterDiagnostic(current, cancellationToken);
                if (diagnostic != null)
                {
                    instance.Add(diagnostic);
                    driverDiagnostics?.Add(diagnostic);
                }
            }
            return instance.ToImmutableAndFree();
        }

        public static string GetFilePathPrefixForGenerator(ISourceGenerator generator)
        {
            Type type = generator.GetType();
            return Path.Combine(type.Assembly.GetName().Name ?? string.Empty, type.FullName);
        }

        public abstract GeneratorDriver FromState(GeneratorDriverState state);

        public abstract SyntaxTree ParseGeneratedSourceText(GeneratedSourceText input, string fileName, CancellationToken cancellationToken);

        public abstract AdditionalSourcesCollection CreateSourcesCollection();
    }
}
