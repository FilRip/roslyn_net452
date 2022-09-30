using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class HostAnalysisScope
    {
        private readonly ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<AnalyzerActions>> _analyzerActions = new ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<AnalyzerActions>>();

        public virtual AnalyzerActions GetAnalyzerActions(DiagnosticAnalyzer analyzer)
        {
            return GetOrCreateAnalyzerActions(analyzer).Value;
        }

        public void RegisterCompilationAction(DiagnosticAnalyzer analyzer, Action<CompilationAnalysisContext> action)
        {
            CompilationAnalyzerAction action2 = new CompilationAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCompilationAction(action2);
        }

        public void RegisterCompilationEndAction(DiagnosticAnalyzer analyzer, Action<CompilationAnalysisContext> action)
        {
            CompilationAnalyzerAction action2 = new CompilationAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCompilationEndAction(action2);
        }

        public void RegisterSemanticModelAction(DiagnosticAnalyzer analyzer, Action<SemanticModelAnalysisContext> action)
        {
            SemanticModelAnalyzerAction action2 = new SemanticModelAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSemanticModelAction(action2);
        }

        public void RegisterSyntaxTreeAction(DiagnosticAnalyzer analyzer, Action<SyntaxTreeAnalysisContext> action)
        {
            SyntaxTreeAnalyzerAction action2 = new SyntaxTreeAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSyntaxTreeAction(action2);
        }

        public void RegisterAdditionalFileAction(DiagnosticAnalyzer analyzer, Action<AdditionalFileAnalysisContext> action)
        {
            AdditionalFileAnalyzerAction action2 = new AdditionalFileAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddAdditionalFileAction(action2);
        }

        public void RegisterSymbolAction(DiagnosticAnalyzer analyzer, Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
        {
            Action<SymbolAnalysisContext> action2 = action;
            SymbolAnalyzerAction action3 = new SymbolAnalyzerAction(action2, symbolKinds, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSymbolAction(action3);
            if (!symbolKinds.Contains(SymbolKind.Parameter))
            {
                return;
            }
            RegisterSymbolAction(analyzer, delegate (SymbolAnalysisContext context)
            {
                ImmutableArray<IParameterSymbol>.Enumerator enumerator = (context.Symbol.Kind switch
                {
                    SymbolKind.Method => ((IMethodSymbol)context.Symbol).Parameters,
                    SymbolKind.Property => ((IPropertySymbol)context.Symbol).Parameters,
                    SymbolKind.NamedType => ((INamedTypeSymbol)context.Symbol).DelegateInvokeMethod?.Parameters ?? ImmutableArray.Create<IParameterSymbol>(),
                    _ => throw new ArgumentException($"{context.Symbol.Kind} is not supported.", "context"),
                }).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IParameterSymbol current = enumerator.Current;
                    if (!current.IsImplicitlyDeclared)
                    {
                        action2(new SymbolAnalysisContext(current, context.Compilation, context.Options, context.ReportDiagnostic, context.IsSupportedDiagnostic, context.CancellationToken));
                    }
                }
            }, ImmutableArray.Create(SymbolKind.Method, SymbolKind.Property, SymbolKind.NamedType));
        }

        public void RegisterSymbolStartAction(DiagnosticAnalyzer analyzer, Action<SymbolStartAnalysisContext> action, SymbolKind symbolKind)
        {
            SymbolStartAnalyzerAction action2 = new SymbolStartAnalyzerAction(action, symbolKind, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSymbolStartAction(action2);
        }

        public void RegisterSymbolEndAction(DiagnosticAnalyzer analyzer, Action<SymbolAnalysisContext> action)
        {
            SymbolEndAnalyzerAction action2 = new SymbolEndAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSymbolEndAction(action2);
        }

        public void RegisterCodeBlockStartAction<TLanguageKindEnum>(DiagnosticAnalyzer analyzer, Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action) where TLanguageKindEnum : struct
        {
            CodeBlockStartAnalyzerAction<TLanguageKindEnum> action2 = new CodeBlockStartAnalyzerAction<TLanguageKindEnum>(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCodeBlockStartAction(action2);
        }

        public void RegisterCodeBlockEndAction(DiagnosticAnalyzer analyzer, Action<CodeBlockAnalysisContext> action)
        {
            CodeBlockAnalyzerAction action2 = new CodeBlockAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCodeBlockEndAction(action2);
        }

        public void RegisterCodeBlockAction(DiagnosticAnalyzer analyzer, Action<CodeBlockAnalysisContext> action)
        {
            CodeBlockAnalyzerAction action2 = new CodeBlockAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCodeBlockAction(action2);
        }

        public void RegisterSyntaxNodeAction<TLanguageKindEnum>(DiagnosticAnalyzer analyzer, Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds) where TLanguageKindEnum : struct
        {
            SyntaxNodeAnalyzerAction<TLanguageKindEnum> action2 = new SyntaxNodeAnalyzerAction<TLanguageKindEnum>(action, syntaxKinds, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddSyntaxNodeAction(action2);
        }

        public void RegisterOperationBlockStartAction(DiagnosticAnalyzer analyzer, Action<OperationBlockStartAnalysisContext> action)
        {
            OperationBlockStartAnalyzerAction action2 = new OperationBlockStartAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddOperationBlockStartAction(action2);
        }

        public void RegisterOperationBlockEndAction(DiagnosticAnalyzer analyzer, Action<OperationBlockAnalysisContext> action)
        {
            OperationBlockAnalyzerAction action2 = new OperationBlockAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddOperationBlockEndAction(action2);
        }

        public void RegisterOperationBlockAction(DiagnosticAnalyzer analyzer, Action<OperationBlockAnalysisContext> action)
        {
            OperationBlockAnalyzerAction action2 = new OperationBlockAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddOperationBlockAction(action2);
        }

        public void RegisterOperationAction(DiagnosticAnalyzer analyzer, Action<OperationAnalysisContext> action, ImmutableArray<OperationKind> operationKinds)
        {
            OperationAnalyzerAction action2 = new OperationAnalyzerAction(action, operationKinds, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddOperationAction(action2);
        }

        protected StrongBox<AnalyzerActions> GetOrCreateAnalyzerActions(DiagnosticAnalyzer analyzer)
        {
            return _analyzerActions.GetOrAdd(analyzer, (DiagnosticAnalyzer _) => new StrongBox<AnalyzerActions>(AnalyzerActions.Empty));
        }
    }
}
