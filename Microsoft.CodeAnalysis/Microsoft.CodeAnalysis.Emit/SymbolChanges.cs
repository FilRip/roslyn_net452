using System;
using System.Collections.Generic;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class SymbolChanges
    {
        private readonly DefinitionMap _definitionMap;

        private readonly IReadOnlyDictionary<ISymbol, SymbolChange> _changes;

        private readonly Func<ISymbol, bool> _isAddedSymbol;

        protected SymbolChanges(DefinitionMap definitionMap, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol)
        {
            _definitionMap = definitionMap;
            _isAddedSymbol = isAddedSymbol;
            _changes = CalculateChanges(edits);
        }

        public bool IsAdded(ISymbol symbol)
        {
            return _isAddedSymbol(symbol);
        }

        public bool RequiresCompilation(ISymbol symbol)
        {
            return GetChange(symbol) != SymbolChange.None;
        }

        public SymbolChange GetChange(IDefinition def)
        {
            ISymbolInternal internalSymbol = def.GetInternalSymbol();
            if (internalSymbol is ISynthesizedMethodBodyImplementationSymbol synthesizedMethodBodyImplementationSymbol)
            {
                IMethodSymbolInternal method = synthesizedMethodBodyImplementationSymbol.Method;
                ISymbolInternal symbolInternal = synthesizedMethodBodyImplementationSymbol;
                SymbolChange change = GetChange((IDefinition)method.GetCciAdapter());
                switch (change)
                {
                    case SymbolChange.Updated:
                        if (!_definitionMap.DefinitionExists((IDefinition)symbolInternal.ContainingType.GetCciAdapter()))
                        {
                            return SymbolChange.Added;
                        }
                        if (!_definitionMap.DefinitionExists(def))
                        {
                            return SymbolChange.Added;
                        }
                        if (!synthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency)
                        {
                            return SymbolChange.None;
                        }
                        if (symbolInternal.Kind == SymbolKind.NamedType)
                        {
                            return SymbolChange.ContainsChanges;
                        }
                        if (symbolInternal.Kind == SymbolKind.Method)
                        {
                            return SymbolChange.Updated;
                        }
                        return SymbolChange.None;
                    case SymbolChange.Added:
                        if (!_definitionMap.DefinitionExists(def))
                        {
                            return SymbolChange.Added;
                        }
                        if (symbolInternal.Kind == SymbolKind.NamedType)
                        {
                            return SymbolChange.ContainsChanges;
                        }
                        if (symbolInternal.Kind == SymbolKind.Method)
                        {
                            return SymbolChange.Updated;
                        }
                        return SymbolChange.None;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(change);
                }
            }
            if (internalSymbol != null)
            {
                return GetChange(internalSymbol.GetISymbol());
            }
            if (_definitionMap.DefinitionExists(def))
            {
                if (!(def is ITypeDefinition))
                {
                    return SymbolChange.None;
                }
                return SymbolChange.ContainsChanges;
            }
            return SymbolChange.Added;
        }

        private SymbolChange GetChange(ISymbol symbol)
        {
            if (_changes.TryGetValue(symbol, out var value))
            {
                return value;
            }
            ISymbol containingSymbol = GetContainingSymbol(symbol);
            if (containingSymbol == null)
            {
                return SymbolChange.None;
            }
            SymbolChange change = GetChange(containingSymbol);
            switch (change)
            {
                case SymbolChange.Added:
                    return SymbolChange.Added;
                case SymbolChange.None:
                    return SymbolChange.None;
                case SymbolChange.ContainsChanges:
                case SymbolChange.Updated:
                    {
                        IReference reference = GetISymbolInternalOrNull(symbol)?.GetCciAdapter();
                        if (reference is IDefinition definition)
                        {
                            if (!_definitionMap.DefinitionExists(definition))
                            {
                                return SymbolChange.Added;
                            }
                            return SymbolChange.None;
                        }
                        if (reference is INamespace @namespace)
                        {
                            if (!_definitionMap.NamespaceExists(@namespace))
                            {
                                return SymbolChange.Added;
                            }
                            return SymbolChange.ContainsChanges;
                        }
                        return SymbolChange.None;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(change);
            }
        }

        protected abstract ISymbolInternal? GetISymbolInternalOrNull(ISymbol symbol);

        public IEnumerable<INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context)
        {
            foreach (ISymbol key in _changes.Keys)
            {
                INamespaceTypeDefinition namespaceTypeDefinition = (GetISymbolInternalOrNull(key)?.GetCciAdapter() as ITypeDefinition)?.AsNamespaceTypeDefinition(context);
                if (namespaceTypeDefinition != null)
                {
                    yield return namespaceTypeDefinition;
                }
            }
        }

        private static IReadOnlyDictionary<ISymbol, SymbolChange> CalculateChanges(IEnumerable<SemanticEdit> edits)
        {
            Dictionary<ISymbol, SymbolChange> dictionary = new Dictionary<ISymbol, SymbolChange>();
            foreach (SemanticEdit edit in edits)
            {
                SymbolChange value;
                switch (edit.Kind)
                {
                    case SemanticEditKind.Update:
                        value = SymbolChange.Updated;
                        break;
                    case SemanticEditKind.Insert:
                        value = SymbolChange.Added;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(edit.Kind);
                    case SemanticEditKind.Delete:
                        continue;
                }
                ISymbol symbol = edit.NewSymbol;
                if (symbol.Kind == SymbolKind.Method)
                {
                    IMethodSymbol partialDefinitionPart = ((IMethodSymbol)symbol).PartialDefinitionPart;
                    if (partialDefinitionPart != null)
                    {
                        symbol = partialDefinitionPart;
                    }
                }
                AddContainingTypesAndNamespaces(dictionary, symbol);
                dictionary.Add(symbol, value);
            }
            return dictionary;
        }

        private static void AddContainingTypesAndNamespaces(Dictionary<ISymbol, SymbolChange> changes, ISymbol symbol)
        {
            while (true)
            {
                ISymbol containingSymbol = GetContainingSymbol(symbol);
                if (containingSymbol == null || changes.ContainsKey(containingSymbol))
                {
                    break;
                }
                SymbolKind kind = containingSymbol.Kind;
                SymbolChange value = ((kind != SymbolKind.Property && kind != SymbolKind.Event) ? SymbolChange.ContainsChanges : SymbolChange.Updated);
                changes.Add(containingSymbol, value);
                symbol = containingSymbol;
            }
        }

        private static ISymbol? GetContainingSymbol(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    {
                        ISymbol associatedSymbol2 = ((IFieldSymbol)symbol).AssociatedSymbol;
                        if (associatedSymbol2 != null)
                        {
                            return associatedSymbol2;
                        }
                        break;
                    }
                case SymbolKind.Method:
                    {
                        ISymbol associatedSymbol = ((IMethodSymbol)symbol).AssociatedSymbol;
                        if (associatedSymbol != null)
                        {
                            return associatedSymbol;
                        }
                        break;
                    }
            }
            symbol = symbol.ContainingSymbol;
            if (symbol != null)
            {
                SymbolKind kind = symbol.Kind;
                if (kind == SymbolKind.Assembly || kind == SymbolKind.NetModule)
                {
                    return null;
                }
            }
            return symbol;
        }
    }
}
