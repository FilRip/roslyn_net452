using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public class LocalScopeBinder : Binder
    {
        private ImmutableArray<LocalSymbol> _locals;

        private ImmutableArray<LocalFunctionSymbol> _localFunctions;

        private ImmutableArray<LabelSymbol> _labels;

        private readonly uint _localScopeDepth;

        private SmallDictionary<string, LocalSymbol> _lazyLocalsMap;

        private SmallDictionary<string, LocalFunctionSymbol> _lazyLocalFunctionsMap;

        private SmallDictionary<string, LabelSymbol> _lazyLabelsMap;

        internal sealed override ImmutableArray<LocalSymbol> Locals
        {
            get
            {
                if (_locals.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _locals, BuildLocals(), default(ImmutableArray<LocalSymbol>));
                }
                return _locals;
            }
        }

        internal sealed override ImmutableArray<LocalFunctionSymbol> LocalFunctions
        {
            get
            {
                if (_localFunctions.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _localFunctions, BuildLocalFunctions(), default(ImmutableArray<LocalFunctionSymbol>));
                }
                return _localFunctions;
            }
        }

        internal sealed override ImmutableArray<LabelSymbol> Labels
        {
            get
            {
                if (_labels.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _labels, BuildLabels(), default(ImmutableArray<LabelSymbol>));
                }
                return _labels;
            }
        }

        private SmallDictionary<string, LocalSymbol> LocalsMap
        {
            get
            {
                if (_lazyLocalsMap == null && Locals.Length > 0)
                {
                    _lazyLocalsMap = BuildMap(Locals);
                }
                return _lazyLocalsMap;
            }
        }

        private SmallDictionary<string, LocalFunctionSymbol> LocalFunctionsMap
        {
            get
            {
                if (_lazyLocalFunctionsMap == null && LocalFunctions.Length > 0)
                {
                    _lazyLocalFunctionsMap = BuildMap(LocalFunctions);
                }
                return _lazyLocalFunctionsMap;
            }
        }

        private SmallDictionary<string, LabelSymbol> LabelsMap
        {
            get
            {
                if (_lazyLabelsMap == null && Labels.Length > 0)
                {
                    _lazyLabelsMap = BuildMap(Labels);
                }
                return _lazyLabelsMap;
            }
        }

        internal override uint LocalScopeDepth => _localScopeDepth;

        internal LocalScopeBinder(Binder next)
            : this(next, next.Flags)
        {
        }

        internal LocalScopeBinder(Binder next, BinderFlags flags)
            : base(next, flags)
        {
            uint localScopeDepth = next.LocalScopeDepth;
            if (localScopeDepth != 1)
            {
                _localScopeDepth = localScopeDepth + 1;
                return;
            }
            for (Binder binder = next; binder != null; binder = binder.Next)
            {
                if (binder is InMethodBinder || binder is WithLambdaParametersBinder)
                {
                    _localScopeDepth = 1u;
                    break;
                }
                if (binder is LocalScopeBinder)
                {
                    _localScopeDepth = 2u;
                    break;
                }
            }
        }

        protected virtual ImmutableArray<LocalSymbol> BuildLocals()
        {
            return ImmutableArray<LocalSymbol>.Empty;
        }

        protected virtual ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions()
        {
            return ImmutableArray<LocalFunctionSymbol>.Empty;
        }

        protected virtual ImmutableArray<LabelSymbol> BuildLabels()
        {
            return ImmutableArray<LabelSymbol>.Empty;
        }

        private static SmallDictionary<string, TSymbol> BuildMap<TSymbol>(ImmutableArray<TSymbol> array) where TSymbol : Symbol
        {
            SmallDictionary<string, TSymbol> smallDictionary = new SmallDictionary<string, TSymbol>();
            for (int num = array.Length - 1; num >= 0; num--)
            {
                TSymbol val = array[num];
                smallDictionary[val.Name] = val;
            }
            return smallDictionary;
        }

        protected ImmutableArray<LocalSymbol> BuildLocals(SyntaxList<StatementSyntax> statements, Binder enclosingBinder)
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            SyntaxList<StatementSyntax>.Enumerator enumerator = statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StatementSyntax current = enumerator.Current;
                BuildLocals(enclosingBinder, current, instance);
            }
            return instance.ToImmutableAndFree();
        }

        internal void BuildLocals(Binder enclosingBinder, StatementSyntax statement, ArrayBuilder<LocalSymbol> locals)
        {
            StatementSyntax statementSyntax = statement;
            while (statementSyntax.Kind() == SyntaxKind.LabeledStatement)
            {
                statementSyntax = ((LabeledStatementSyntax)statementSyntax).Statement;
            }
            switch (statementSyntax.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        Binder binder2 = enclosingBinder.GetBinder(statementSyntax) ?? enclosingBinder;
                        LocalDeclarationStatementSyntax localDeclarationStatementSyntax = (LocalDeclarationStatementSyntax)statementSyntax;
                        TypeSyntax type = localDeclarationStatementSyntax.Declaration.Type;
                        Action<ArrayRankSpecifierSyntax, (LocalScopeBinder, ArrayBuilder<LocalSymbol>, Binder)> action = delegate (ArrayRankSpecifierSyntax rankSpecifier, (LocalScopeBinder localScopeBinder, ArrayBuilder<LocalSymbol> locals, Binder localDeclarationBinder) args)
                        {
                            SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator2 = rankSpecifier.Sizes.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                ExpressionSyntax current2 = enumerator2.Current;
                                if (current2.Kind() != SyntaxKind.OmittedArraySizeExpression)
                                {
                                    ExpressionVariableFinder.FindExpressionVariables(args.localScopeBinder, args.locals, current2, args.localDeclarationBinder);
                                }
                            }
                        };
                        (LocalScopeBinder, ArrayBuilder<LocalSymbol>, Binder) argument = (this, locals, binder2);
                        type.VisitRankSpecifiers(action, in argument);
                        LocalDeclarationKind kind = (localDeclarationStatementSyntax.IsConst ? LocalDeclarationKind.Constant : ((!(localDeclarationStatementSyntax.UsingKeyword != default(SyntaxToken))) ? LocalDeclarationKind.RegularVariable : LocalDeclarationKind.UsingVariable));
                        SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = localDeclarationStatementSyntax.Declaration.Variables.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            VariableDeclaratorSyntax current = enumerator.Current;
                            SourceLocalSymbol item = MakeLocal(localDeclarationStatementSyntax.Declaration, current, kind, binder2);
                            locals.Add(item);
                            ExpressionVariableFinder.FindExpressionVariables(this, locals, current, binder2);
                        }
                        break;
                    }
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ThrowStatement:
                case SyntaxKind.IfStatement:
                    ExpressionVariableFinder.FindExpressionVariables(this, locals, statementSyntax, enclosingBinder.GetBinder(statementSyntax) ?? enclosingBinder);
                    break;
                case SyntaxKind.SwitchStatement:
                    {
                        SwitchStatementSyntax switchStatementSyntax = (SwitchStatementSyntax)statementSyntax;
                        ExpressionVariableFinder.FindExpressionVariables(this, locals, statementSyntax, enclosingBinder.GetBinder(switchStatementSyntax.Expression) ?? enclosingBinder);
                        break;
                    }
                case SyntaxKind.LockStatement:
                    {
                        Binder binder = enclosingBinder.GetBinder(statementSyntax);
                        ExpressionVariableFinder.FindExpressionVariables(this, locals, statementSyntax, binder);
                        break;
                    }
            }
        }

        protected ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions(SyntaxList<StatementSyntax> statements)
        {
            ArrayBuilder<LocalFunctionSymbol> locals = null;
            SyntaxList<StatementSyntax>.Enumerator enumerator = statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StatementSyntax current = enumerator.Current;
                BuildLocalFunctions(current, ref locals);
            }
            return locals?.ToImmutableAndFree() ?? ImmutableArray<LocalFunctionSymbol>.Empty;
        }

        internal void BuildLocalFunctions(StatementSyntax statement, ref ArrayBuilder<LocalFunctionSymbol> locals)
        {
            StatementSyntax statementSyntax = statement;
            while (statementSyntax.Kind() == SyntaxKind.LabeledStatement)
            {
                statementSyntax = ((LabeledStatementSyntax)statementSyntax).Statement;
            }
            if (statementSyntax.Kind() == SyntaxKind.LocalFunctionStatement)
            {
                LocalFunctionStatementSyntax declaration = (LocalFunctionStatementSyntax)statementSyntax;
                if (locals == null)
                {
                    locals = ArrayBuilder<LocalFunctionSymbol>.GetInstance();
                }
                LocalFunctionSymbol item = MakeLocalFunction(declaration);
                locals.Add(item);
            }
        }

        protected SourceLocalSymbol MakeLocal(VariableDeclarationSyntax declaration, VariableDeclaratorSyntax declarator, LocalDeclarationKind kind, Binder initializerBinderOpt = null)
        {
            return SourceLocalSymbol.MakeLocal(ContainingMemberOrLambda, this, allowRefKind: true, declaration.Type, declarator.Identifier, kind, declarator.Initializer, initializerBinderOpt);
        }

        protected LocalFunctionSymbol MakeLocalFunction(LocalFunctionStatementSyntax declaration)
        {
            return new LocalFunctionSymbol(this, ContainingMemberOrLambda, declaration);
        }

        protected void BuildLabels(SyntaxList<StatementSyntax> statements, ref ArrayBuilder<LabelSymbol> labels)
        {
            MethodSymbol containingMethod = (MethodSymbol)ContainingMemberOrLambda;
            SyntaxList<StatementSyntax>.Enumerator enumerator = statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StatementSyntax current = enumerator.Current;
                BuildLabels(containingMethod, current, ref labels);
            }
        }

        internal static void BuildLabels(MethodSymbol containingMethod, StatementSyntax statement, ref ArrayBuilder<LabelSymbol> labels)
        {
            while (statement.Kind() == SyntaxKind.LabeledStatement)
            {
                LabeledStatementSyntax labeledStatementSyntax = (LabeledStatementSyntax)statement;
                if (labels == null)
                {
                    labels = ArrayBuilder<LabelSymbol>.GetInstance();
                }
                SourceLabelSymbol item = new SourceLabelSymbol(containingMethod, labeledStatementSyntax.Identifier);
                labels.Add(item);
                statement = labeledStatementSyntax.Statement;
            }
        }

        protected override SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            if (LocalsMap != null && LocalsMap.TryGetValue(nameToken.ValueText, out LocalSymbol value))
            {
                if (value.IdentifierToken == nameToken)
                {
                    return (SourceLocalSymbol)value;
                }
                ImmutableArray<LocalSymbol>.Enumerator enumerator = Locals.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalSymbol current = enumerator.Current;
                    if (current.IdentifierToken == nameToken)
                    {
                        return (SourceLocalSymbol)current;
                    }
                }
            }
            return base.LookupLocal(nameToken);
        }

        protected override LocalFunctionSymbol LookupLocalFunction(SyntaxToken nameToken)
        {
            if (LocalFunctionsMap != null && LocalFunctionsMap.TryGetValue(nameToken.ValueText, out LocalFunctionSymbol value))
            {
                if (value.NameToken == nameToken)
                {
                    return value;
                }
                ImmutableArray<LocalFunctionSymbol>.Enumerator enumerator = LocalFunctions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalFunctionSymbol current = enumerator.Current;
                    if (current.NameToken == nameToken)
                    {
                        return current;
                    }
                }
            }
            return base.LookupLocalFunction(nameToken);
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((options & LookupOptions.LabelsOnly) != 0)
            {
                SmallDictionary<string, LabelSymbol> labelsMap = LabelsMap;
                if (labelsMap != null && labelsMap.TryGetValue(name, out var value))
                {
                    result.MergeEqual(LookupResult.Good(value));
                }
                return;
            }
            SmallDictionary<string, LocalSymbol> localsMap = LocalsMap;
            if (localsMap != null && (options & LookupOptions.NamespaceAliasesOnly) == 0 && localsMap.TryGetValue(name, out var value2))
            {
                result.MergeEqual(originalBinder.CheckViability(value2, arity, options, null, diagnose, ref useSiteInfo, basesBeingResolved));
            }
            SmallDictionary<string, LocalFunctionSymbol> localFunctionsMap = LocalFunctionsMap;
            if (localFunctionsMap != null && options.CanConsiderLocals() && localFunctionsMap.TryGetValue(name, out var value3))
            {
                result.MergeEqual(originalBinder.CheckViability(value3, arity, options, null, diagnose, ref useSiteInfo, basesBeingResolved));
            }
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if ((options & LookupOptions.LabelsOnly) != 0 && LabelsMap != null)
            {
                SmallDictionary<string, LabelSymbol>.Enumerator enumerator = LabelsMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, LabelSymbol> current = enumerator.Current;
                    result.AddSymbol(current.Value, current.Key, 0);
                }
            }
            if (!options.CanConsiderLocals())
            {
                return;
            }
            if (LocalsMap != null)
            {
                SmallDictionary<string, LocalSymbol>.Enumerator enumerator2 = LocalsMap.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<string, LocalSymbol> current2 = enumerator2.Current;
                    if (originalBinder.CanAddLookupSymbolInfo(current2.Value, options, result, null))
                    {
                        result.AddSymbol(current2.Value, current2.Key, 0);
                    }
                }
            }
            if (LocalFunctionsMap == null)
            {
                return;
            }
            SmallDictionary<string, LocalFunctionSymbol>.Enumerator enumerator3 = LocalFunctionsMap.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                KeyValuePair<string, LocalFunctionSymbol> current3 = enumerator3.Current;
                if (originalBinder.CanAddLookupSymbolInfo(current3.Value, options, result, null))
                {
                    result.AddSymbol(current3.Value, current3.Key, 0);
                }
            }
        }

        private bool ReportConflictWithLocal(Symbol local, Symbol newSymbol, string name, Location newLocation, BindingDiagnosticBag diagnostics)
        {
            SymbolKind symbolKind = newSymbol?.Kind ?? SymbolKind.Parameter;
            if (symbolKind == SymbolKind.ErrorType)
            {
                return true;
            }
            if ((false | (symbolKind == SymbolKind.Local && Locals.Contains((LocalSymbol)newSymbol)) | (symbolKind == SymbolKind.Method && LocalFunctions.Contains((LocalFunctionSymbol)newSymbol))) && newLocation.SourceSpan.Start >= local.Locations[0].SourceSpan.Start)
            {
                diagnostics.Add(ErrorCode.ERR_LocalDuplicate, newLocation, name);
                return true;
            }
            switch (symbolKind)
            {
                case SymbolKind.Local:
                case SymbolKind.Method:
                case SymbolKind.Parameter:
                case SymbolKind.TypeParameter:
                    diagnostics.Add(ErrorCode.ERR_LocalIllegallyOverrides, newLocation, name);
                    return true;
                case SymbolKind.RangeVariable:
                    diagnostics.Add(ErrorCode.ERR_QueryRangeVariableOverrides, newLocation, name);
                    return true;
                default:
                    diagnostics.Add(ErrorCode.ERR_InternalError, newLocation);
                    return false;
            }
        }

        internal virtual bool EnsureSingleDefinition(Symbol symbol, string name, Location location, BindingDiagnosticBag diagnostics)
        {
            LocalSymbol value = null;
            LocalFunctionSymbol value2 = null;
            SmallDictionary<string, LocalSymbol> localsMap = LocalsMap;
            SmallDictionary<string, LocalFunctionSymbol> localFunctionsMap = LocalFunctionsMap;
            if ((localsMap != null && localsMap.TryGetValue(name, out value)) || (localFunctionsMap != null && localFunctionsMap.TryGetValue(name, out value2)))
            {
                Symbol symbol2 = (Symbol)(value ?? ((object)value2));
                if (symbol == symbol2)
                {
                    return false;
                }
                return ReportConflictWithLocal(symbol2, symbol, name, location, diagnostics);
            }
            return false;
        }
    }
}
