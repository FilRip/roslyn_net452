using System;
using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal sealed class AnalyzerCompilationStartAnalysisContext : CompilationStartAnalysisContext
    {
        private readonly DiagnosticAnalyzer _analyzer;

        private readonly HostCompilationStartAnalysisScope _scope;

        private readonly CompilationAnalysisValueProviderFactory _compilationAnalysisValueProviderFactory;

        public AnalyzerCompilationStartAnalysisContext(DiagnosticAnalyzer analyzer, HostCompilationStartAnalysisScope scope, Compilation compilation, AnalyzerOptions options, CompilationAnalysisValueProviderFactory compilationAnalysisValueProviderFactory, CancellationToken cancellationToken)
            : base(compilation, options, cancellationToken)
        {
            _analyzer = analyzer;
            _scope = scope;
            _compilationAnalysisValueProviderFactory = compilationAnalysisValueProviderFactory;
        }

        public override void RegisterCompilationEndAction(Action<CompilationAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterCompilationEndAction(_analyzer, action);
        }

        public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterSyntaxTreeAction(_analyzer, action);
        }

        public override void RegisterAdditionalFileAction(Action<AdditionalFileAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterAdditionalFileAction(_analyzer, action);
        }

        public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterSemanticModelAction(_analyzer, action);
        }

        public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action, symbolKinds);
            _scope.RegisterSymbolAction(_analyzer, action, symbolKinds);
        }

        public override void RegisterSymbolStartAction(Action<SymbolStartAnalysisContext> action, SymbolKind symbolKind)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterSymbolStartAction(_analyzer, action, symbolKind);
        }

        public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterCodeBlockStartAction(_analyzer, action);
        }

        public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterCodeBlockAction(_analyzer, action);
        }

        public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action, syntaxKinds);
            _scope.RegisterSyntaxNodeAction(_analyzer, action, syntaxKinds);
        }

        public override void RegisterOperationBlockStartAction(Action<OperationBlockStartAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterOperationBlockStartAction(_analyzer, action);
        }

        public override void RegisterOperationBlockAction(Action<OperationBlockAnalysisContext> action)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action);
            _scope.RegisterOperationBlockAction(_analyzer, action);
        }

        public override void RegisterOperationAction(Action<OperationAnalysisContext> action, ImmutableArray<OperationKind> operationKinds)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(action, operationKinds);
            _scope.RegisterOperationAction(_analyzer, action, operationKinds);
        }

        internal override bool TryGetValueCore<TKey, TValue>(TKey key, AnalysisValueProvider<TKey, TValue> valueProvider, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            return _compilationAnalysisValueProviderFactory.GetValueProvider(valueProvider).TryGetValue(key, out value);
        }
    }
}
