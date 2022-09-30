using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public struct AnalyzerActions
    {
        public static readonly AnalyzerActions Empty = new AnalyzerActions(concurrent: false);

        private ImmutableArray<CompilationStartAnalyzerAction> _compilationStartActions;

        private ImmutableArray<CompilationAnalyzerAction> _compilationEndActions;

        private ImmutableArray<CompilationAnalyzerAction> _compilationActions;

        private ImmutableArray<SyntaxTreeAnalyzerAction> _syntaxTreeActions;

        private ImmutableArray<AdditionalFileAnalyzerAction> _additionalFileActions;

        private ImmutableArray<SemanticModelAnalyzerAction> _semanticModelActions;

        private ImmutableArray<SymbolAnalyzerAction> _symbolActions;

        private ImmutableArray<SymbolStartAnalyzerAction> _symbolStartActions;

        private ImmutableArray<SymbolEndAnalyzerAction> _symbolEndActions;

        private ImmutableArray<AnalyzerAction> _codeBlockStartActions;

        private ImmutableArray<CodeBlockAnalyzerAction> _codeBlockEndActions;

        private ImmutableArray<CodeBlockAnalyzerAction> _codeBlockActions;

        private ImmutableArray<OperationBlockStartAnalyzerAction> _operationBlockStartActions;

        private ImmutableArray<OperationBlockAnalyzerAction> _operationBlockEndActions;

        private ImmutableArray<OperationBlockAnalyzerAction> _operationBlockActions;

        private ImmutableArray<AnalyzerAction> _syntaxNodeActions;

        private ImmutableArray<OperationAnalyzerAction> _operationActions;

        private bool _concurrent;

        public readonly int CompilationStartActionsCount => _compilationStartActions.Length;

        public readonly int CompilationEndActionsCount => _compilationEndActions.Length;

        public readonly int CompilationActionsCount => _compilationActions.Length;

        public readonly int SyntaxTreeActionsCount => _syntaxTreeActions.Length;

        public readonly int AdditionalFileActionsCount => _additionalFileActions.Length;

        public readonly int SemanticModelActionsCount => _semanticModelActions.Length;

        public readonly int SymbolActionsCount => _symbolActions.Length;

        public readonly int SymbolStartActionsCount => _symbolStartActions.Length;

        public readonly int SymbolEndActionsCount => _symbolEndActions.Length;

        public readonly int SyntaxNodeActionsCount => _syntaxNodeActions.Length;

        public readonly int OperationActionsCount => _operationActions.Length;

        public readonly int OperationBlockStartActionsCount => _operationBlockStartActions.Length;

        public readonly int OperationBlockEndActionsCount => _operationBlockEndActions.Length;

        public readonly int OperationBlockActionsCount => _operationBlockActions.Length;

        public readonly int CodeBlockStartActionsCount => _codeBlockStartActions.Length;

        public readonly int CodeBlockEndActionsCount => _codeBlockEndActions.Length;

        public readonly int CodeBlockActionsCount => _codeBlockActions.Length;

        public readonly bool Concurrent => _concurrent;

        public bool IsEmpty { get; private set; }

        public readonly bool IsDefault => _compilationStartActions.IsDefault;

        internal readonly ImmutableArray<CompilationStartAnalyzerAction> CompilationStartActions => _compilationStartActions;

        internal readonly ImmutableArray<CompilationAnalyzerAction> CompilationEndActions => _compilationEndActions;

        internal readonly ImmutableArray<CompilationAnalyzerAction> CompilationActions => _compilationActions;

        internal readonly ImmutableArray<SyntaxTreeAnalyzerAction> SyntaxTreeActions => _syntaxTreeActions;

        internal readonly ImmutableArray<AdditionalFileAnalyzerAction> AdditionalFileActions => _additionalFileActions;

        internal readonly ImmutableArray<SemanticModelAnalyzerAction> SemanticModelActions => _semanticModelActions;

        internal readonly ImmutableArray<SymbolAnalyzerAction> SymbolActions => _symbolActions;

        internal readonly ImmutableArray<SymbolStartAnalyzerAction> SymbolStartActions => _symbolStartActions;

        internal readonly ImmutableArray<SymbolEndAnalyzerAction> SymbolEndActions => _symbolEndActions;

        internal readonly ImmutableArray<CodeBlockAnalyzerAction> CodeBlockEndActions => _codeBlockEndActions;

        internal readonly ImmutableArray<CodeBlockAnalyzerAction> CodeBlockActions => _codeBlockActions;

        internal readonly ImmutableArray<OperationBlockAnalyzerAction> OperationBlockActions => _operationBlockActions;

        internal readonly ImmutableArray<OperationBlockAnalyzerAction> OperationBlockEndActions => _operationBlockEndActions;

        internal readonly ImmutableArray<OperationBlockStartAnalyzerAction> OperationBlockStartActions => _operationBlockStartActions;

        internal readonly ImmutableArray<OperationAnalyzerAction> OperationActions => _operationActions;

        internal AnalyzerActions(bool concurrent)
        {
            _compilationStartActions = ImmutableArray<CompilationStartAnalyzerAction>.Empty;
            _compilationEndActions = ImmutableArray<CompilationAnalyzerAction>.Empty;
            _compilationActions = ImmutableArray<CompilationAnalyzerAction>.Empty;
            _syntaxTreeActions = ImmutableArray<SyntaxTreeAnalyzerAction>.Empty;
            _additionalFileActions = ImmutableArray<AdditionalFileAnalyzerAction>.Empty;
            _semanticModelActions = ImmutableArray<SemanticModelAnalyzerAction>.Empty;
            _symbolActions = ImmutableArray<SymbolAnalyzerAction>.Empty;
            _symbolStartActions = ImmutableArray<SymbolStartAnalyzerAction>.Empty;
            _symbolEndActions = ImmutableArray<SymbolEndAnalyzerAction>.Empty;
            _codeBlockStartActions = ImmutableArray<AnalyzerAction>.Empty;
            _codeBlockEndActions = ImmutableArray<CodeBlockAnalyzerAction>.Empty;
            _codeBlockActions = ImmutableArray<CodeBlockAnalyzerAction>.Empty;
            _operationBlockStartActions = ImmutableArray<OperationBlockStartAnalyzerAction>.Empty;
            _operationBlockEndActions = ImmutableArray<OperationBlockAnalyzerAction>.Empty;
            _operationBlockActions = ImmutableArray<OperationBlockAnalyzerAction>.Empty;
            _syntaxNodeActions = ImmutableArray<AnalyzerAction>.Empty;
            _operationActions = ImmutableArray<OperationAnalyzerAction>.Empty;
            _concurrent = concurrent;
            IsEmpty = true;
        }

        public AnalyzerActions(ImmutableArray<CompilationStartAnalyzerAction> compilationStartActions, ImmutableArray<CompilationAnalyzerAction> compilationEndActions, ImmutableArray<CompilationAnalyzerAction> compilationActions, ImmutableArray<SyntaxTreeAnalyzerAction> syntaxTreeActions, ImmutableArray<AdditionalFileAnalyzerAction> additionalFileActions, ImmutableArray<SemanticModelAnalyzerAction> semanticModelActions, ImmutableArray<SymbolAnalyzerAction> symbolActions, ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions, ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, ImmutableArray<AnalyzerAction> codeBlockStartActions, ImmutableArray<CodeBlockAnalyzerAction> codeBlockEndActions, ImmutableArray<CodeBlockAnalyzerAction> codeBlockActions, ImmutableArray<OperationBlockStartAnalyzerAction> operationBlockStartActions, ImmutableArray<OperationBlockAnalyzerAction> operationBlockEndActions, ImmutableArray<OperationBlockAnalyzerAction> operationBlockActions, ImmutableArray<AnalyzerAction> syntaxNodeActions, ImmutableArray<OperationAnalyzerAction> operationActions, bool concurrent, bool isEmpty)
        {
            _compilationStartActions = compilationStartActions;
            _compilationEndActions = compilationEndActions;
            _compilationActions = compilationActions;
            _syntaxTreeActions = syntaxTreeActions;
            _additionalFileActions = additionalFileActions;
            _semanticModelActions = semanticModelActions;
            _symbolActions = symbolActions;
            _symbolStartActions = symbolStartActions;
            _symbolEndActions = symbolEndActions;
            _codeBlockStartActions = codeBlockStartActions;
            _codeBlockEndActions = codeBlockEndActions;
            _codeBlockActions = codeBlockActions;
            _operationBlockStartActions = operationBlockStartActions;
            _operationBlockEndActions = operationBlockEndActions;
            _operationBlockActions = operationBlockActions;
            _syntaxNodeActions = syntaxNodeActions;
            _operationActions = operationActions;
            _concurrent = concurrent;
            IsEmpty = isEmpty;
        }

        internal readonly ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> GetCodeBlockStartActions<TLanguageKindEnum>() where TLanguageKindEnum : struct
        {
            return _codeBlockStartActions.OfType<CodeBlockStartAnalyzerAction<TLanguageKindEnum>>().ToImmutableArray();
        }

        internal readonly ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> GetSyntaxNodeActions<TLanguageKindEnum>() where TLanguageKindEnum : struct
        {
            return _syntaxNodeActions.OfType<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>().ToImmutableArray();
        }

        internal readonly ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> GetSyntaxNodeActions<TLanguageKindEnum>(DiagnosticAnalyzer analyzer) where TLanguageKindEnum : struct
        {
            ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> instance = ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>.GetInstance();
            ImmutableArray<AnalyzerAction>.Enumerator enumerator = _syntaxNodeActions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AnalyzerAction current = enumerator.Current;
                if (current.Analyzer == analyzer && current is SyntaxNodeAnalyzerAction<TLanguageKindEnum> item)
                {
                    instance.Add(item);
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal void AddCompilationStartAction(CompilationStartAnalyzerAction action)
        {
            _compilationStartActions = _compilationStartActions.Add(action);
            IsEmpty = false;
        }

        internal void AddCompilationEndAction(CompilationAnalyzerAction action)
        {
            _compilationEndActions = _compilationEndActions.Add(action);
            IsEmpty = false;
        }

        internal void AddCompilationAction(CompilationAnalyzerAction action)
        {
            _compilationActions = _compilationActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSyntaxTreeAction(SyntaxTreeAnalyzerAction action)
        {
            _syntaxTreeActions = _syntaxTreeActions.Add(action);
            IsEmpty = false;
        }

        internal void AddAdditionalFileAction(AdditionalFileAnalyzerAction action)
        {
            _additionalFileActions = _additionalFileActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSemanticModelAction(SemanticModelAnalyzerAction action)
        {
            _semanticModelActions = _semanticModelActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSymbolAction(SymbolAnalyzerAction action)
        {
            _symbolActions = _symbolActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSymbolStartAction(SymbolStartAnalyzerAction action)
        {
            _symbolStartActions = _symbolStartActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSymbolEndAction(SymbolEndAnalyzerAction action)
        {
            _symbolEndActions = _symbolEndActions.Add(action);
            IsEmpty = false;
        }

        internal void AddCodeBlockStartAction<TLanguageKindEnum>(CodeBlockStartAnalyzerAction<TLanguageKindEnum> action) where TLanguageKindEnum : struct
        {
            _codeBlockStartActions = _codeBlockStartActions.Add(action);
            IsEmpty = false;
        }

        internal void AddCodeBlockEndAction(CodeBlockAnalyzerAction action)
        {
            _codeBlockEndActions = _codeBlockEndActions.Add(action);
            IsEmpty = false;
        }

        internal void AddCodeBlockAction(CodeBlockAnalyzerAction action)
        {
            _codeBlockActions = _codeBlockActions.Add(action);
            IsEmpty = false;
        }

        internal void AddSyntaxNodeAction<TLanguageKindEnum>(SyntaxNodeAnalyzerAction<TLanguageKindEnum> action) where TLanguageKindEnum : struct
        {
            _syntaxNodeActions = _syntaxNodeActions.Add(action);
            IsEmpty = false;
        }

        internal void AddOperationBlockStartAction(OperationBlockStartAnalyzerAction action)
        {
            _operationBlockStartActions = _operationBlockStartActions.Add(action);
            IsEmpty = false;
        }

        internal void AddOperationBlockAction(OperationBlockAnalyzerAction action)
        {
            _operationBlockActions = _operationBlockActions.Add(action);
            IsEmpty = false;
        }

        internal void AddOperationBlockEndAction(OperationBlockAnalyzerAction action)
        {
            _operationBlockEndActions = _operationBlockEndActions.Add(action);
            IsEmpty = false;
        }

        internal void AddOperationAction(OperationAnalyzerAction action)
        {
            _operationActions = _operationActions.Add(action);
            IsEmpty = false;
        }

        internal void EnableConcurrentExecution()
        {
            _concurrent = true;
        }

        public readonly AnalyzerActions Append(in AnalyzerActions otherActions, bool appendSymbolStartAndSymbolEndActions = true)
        {
            if (otherActions.IsDefault)
            {
                throw new ArgumentNullException("otherActions");
            }
            AnalyzerActions result = new AnalyzerActions(_concurrent || otherActions.Concurrent);
            result._compilationStartActions = _compilationStartActions.AddRange(otherActions._compilationStartActions);
            result._compilationEndActions = _compilationEndActions.AddRange(otherActions._compilationEndActions);
            result._compilationActions = _compilationActions.AddRange(otherActions._compilationActions);
            result._syntaxTreeActions = _syntaxTreeActions.AddRange(otherActions._syntaxTreeActions);
            result._additionalFileActions = _additionalFileActions.AddRange(otherActions._additionalFileActions);
            result._semanticModelActions = _semanticModelActions.AddRange(otherActions._semanticModelActions);
            result._symbolActions = _symbolActions.AddRange(otherActions._symbolActions);
            result._symbolStartActions = (appendSymbolStartAndSymbolEndActions ? _symbolStartActions.AddRange(otherActions._symbolStartActions) : _symbolStartActions);
            result._symbolEndActions = (appendSymbolStartAndSymbolEndActions ? _symbolEndActions.AddRange(otherActions._symbolEndActions) : _symbolEndActions);
            result._codeBlockStartActions = _codeBlockStartActions.AddRange(otherActions._codeBlockStartActions);
            result._codeBlockEndActions = _codeBlockEndActions.AddRange(otherActions._codeBlockEndActions);
            result._codeBlockActions = _codeBlockActions.AddRange(otherActions._codeBlockActions);
            result._syntaxNodeActions = _syntaxNodeActions.AddRange(otherActions._syntaxNodeActions);
            result._operationActions = _operationActions.AddRange(otherActions._operationActions);
            result._operationBlockStartActions = _operationBlockStartActions.AddRange(otherActions._operationBlockStartActions);
            result._operationBlockEndActions = _operationBlockEndActions.AddRange(otherActions._operationBlockEndActions);
            result._operationBlockActions = _operationBlockActions.AddRange(otherActions._operationBlockActions);
            result.IsEmpty = IsEmpty && otherActions.IsEmpty;
            return result;
        }
    }
}
