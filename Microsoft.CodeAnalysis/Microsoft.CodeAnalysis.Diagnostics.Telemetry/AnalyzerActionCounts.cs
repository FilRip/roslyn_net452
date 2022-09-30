namespace Microsoft.CodeAnalysis.Diagnostics.Telemetry
{
    internal class AnalyzerActionCounts
    {
        internal static readonly AnalyzerActionCounts Empty = new AnalyzerActionCounts(in AnalyzerActions.Empty);

        public int CompilationStartActionsCount { get; }

        public int CompilationEndActionsCount { get; }

        public int CompilationActionsCount { get; }

        public int SyntaxTreeActionsCount { get; }

        public int AdditionalFileActionsCount { get; }

        public int SemanticModelActionsCount { get; }

        public int SymbolActionsCount { get; }

        public int SymbolStartActionsCount { get; }

        public int SymbolEndActionsCount { get; }

        public int SyntaxNodeActionsCount { get; }

        public int CodeBlockStartActionsCount { get; }

        public int CodeBlockEndActionsCount { get; }

        public int CodeBlockActionsCount { get; }

        public int OperationActionsCount { get; }

        public int OperationBlockStartActionsCount { get; }

        public int OperationBlockEndActionsCount { get; }

        public int OperationBlockActionsCount { get; }

        public bool HasAnyExecutableCodeActions { get; }

        public bool Concurrent { get; }

        internal AnalyzerActionCounts(in AnalyzerActions analyzerActions)
            : this(analyzerActions.CompilationStartActionsCount, analyzerActions.CompilationEndActionsCount, analyzerActions.CompilationActionsCount, analyzerActions.SyntaxTreeActionsCount, analyzerActions.AdditionalFileActionsCount, analyzerActions.SemanticModelActionsCount, analyzerActions.SymbolActionsCount, analyzerActions.SymbolStartActionsCount, analyzerActions.SymbolEndActionsCount, analyzerActions.SyntaxNodeActionsCount, analyzerActions.CodeBlockStartActionsCount, analyzerActions.CodeBlockEndActionsCount, analyzerActions.CodeBlockActionsCount, analyzerActions.OperationActionsCount, analyzerActions.OperationBlockStartActionsCount, analyzerActions.OperationBlockEndActionsCount, analyzerActions.OperationBlockActionsCount, analyzerActions.Concurrent)
        {
        }

        internal AnalyzerActionCounts(int compilationStartActionsCount, int compilationEndActionsCount, int compilationActionsCount, int syntaxTreeActionsCount, int additionalFileActionsCount, int semanticModelActionsCount, int symbolActionsCount, int symbolStartActionsCount, int symbolEndActionsCount, int syntaxNodeActionsCount, int codeBlockStartActionsCount, int codeBlockEndActionsCount, int codeBlockActionsCount, int operationActionsCount, int operationBlockStartActionsCount, int operationBlockEndActionsCount, int operationBlockActionsCount, bool concurrent)
        {
            CompilationStartActionsCount = compilationStartActionsCount;
            CompilationEndActionsCount = compilationEndActionsCount;
            CompilationActionsCount = compilationActionsCount;
            SyntaxTreeActionsCount = syntaxTreeActionsCount;
            AdditionalFileActionsCount = additionalFileActionsCount;
            SemanticModelActionsCount = semanticModelActionsCount;
            SymbolActionsCount = symbolActionsCount;
            SymbolStartActionsCount = symbolStartActionsCount;
            SymbolEndActionsCount = symbolEndActionsCount;
            SyntaxNodeActionsCount = syntaxNodeActionsCount;
            CodeBlockStartActionsCount = codeBlockStartActionsCount;
            CodeBlockEndActionsCount = codeBlockEndActionsCount;
            CodeBlockActionsCount = codeBlockActionsCount;
            OperationActionsCount = operationActionsCount;
            OperationBlockStartActionsCount = operationBlockStartActionsCount;
            OperationBlockEndActionsCount = operationBlockEndActionsCount;
            OperationBlockActionsCount = operationBlockActionsCount;
            Concurrent = concurrent;
            HasAnyExecutableCodeActions = CodeBlockActionsCount > 0 || CodeBlockStartActionsCount > 0 || SyntaxNodeActionsCount > 0 || OperationActionsCount > 0 || OperationBlockActionsCount > 0 || OperationBlockStartActionsCount > 0 || SymbolStartActionsCount > 0;
        }
    }
}
