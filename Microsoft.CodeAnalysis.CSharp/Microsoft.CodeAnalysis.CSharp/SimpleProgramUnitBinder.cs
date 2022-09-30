using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SimpleProgramUnitBinder : LocalScopeBinder
    {
        private readonly SimpleProgramBinder _scope;

        internal override bool IsLocalFunctionsScopeBinder => _scope.IsLocalFunctionsScopeBinder;

        internal override bool IsLabelsScopeBinder => false;

        internal override SyntaxNode? ScopeDesignator => _scope.ScopeDesignator;

        public SimpleProgramUnitBinder(Binder enclosing, SimpleProgramBinder scope)
            : base(enclosing, enclosing.Flags)
        {
            _scope = scope;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            return _scope.Locals;
        }

        protected override ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions()
        {
            return _scope.LocalFunctions;
        }

        protected override ImmutableArray<LabelSymbol> BuildLabels()
        {
            return ImmutableArray<LabelSymbol>.Empty;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            return _scope.GetDeclaredLocalsForScope(scopeDesignator);
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            return _scope.GetDeclaredLocalFunctionsForScope(scopeDesignator);
        }
    }
}
