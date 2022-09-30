using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class EmbeddedStatementBinder : LocalScopeBinder
    {
        private readonly StatementSyntax _statement;

        internal override bool IsLocalFunctionsScopeBinder => true;

        internal override bool IsLabelsScopeBinder => true;

        internal override SyntaxNode ScopeDesignator => _statement;

        public EmbeddedStatementBinder(Binder enclosing, StatementSyntax statement)
            : base(enclosing, enclosing.Flags)
        {
            _statement = statement;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            BuildLocals(this, _statement, instance);
            return instance.ToImmutableAndFree();
        }

        protected override ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions()
        {
            ArrayBuilder<LocalFunctionSymbol> locals = null;
            BuildLocalFunctions(_statement, ref locals);
            return locals?.ToImmutableAndFree() ?? ImmutableArray<LocalFunctionSymbol>.Empty;
        }

        protected override ImmutableArray<LabelSymbol> BuildLabels()
        {
            ArrayBuilder<LabelSymbol> labels = null;
            LocalScopeBinder.BuildLabels((MethodSymbol)ContainingMemberOrLambda, _statement, ref labels);
            return labels?.ToImmutableAndFree() ?? ImmutableArray<LabelSymbol>.Empty;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (ScopeDesignator == scopeDesignator)
            {
                return Locals;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            if (ScopeDesignator == scopeDesignator)
            {
                return LocalFunctions;
            }
            throw ExceptionUtilities.Unreachable;
        }
    }
}
