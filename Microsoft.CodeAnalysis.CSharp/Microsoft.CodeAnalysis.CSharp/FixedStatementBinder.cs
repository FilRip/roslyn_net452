using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class FixedStatementBinder : LocalScopeBinder
    {
        private readonly FixedStatementSyntax _syntax;

        internal override SyntaxNode ScopeDesignator => _syntax;

        public FixedStatementBinder(Binder enclosing, FixedStatementSyntax syntax)
            : base(enclosing)
        {
            _syntax = syntax;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            if (_syntax.Declaration != null)
            {
                ArrayBuilder<LocalSymbol> arrayBuilder = new ArrayBuilder<LocalSymbol>(_syntax.Declaration.Variables.Count);
                TypeSyntax type = _syntax.Declaration.Type;
                static void action(ArrayRankSpecifierSyntax rankSpecifier, (FixedStatementBinder binder, ArrayBuilder<LocalSymbol> locals) args)
                {
                    SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator2 = rankSpecifier.Sizes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        ExpressionSyntax current2 = enumerator2.Current;
                        if (current2.Kind() != SyntaxKind.OmittedArraySizeExpression)
                        {
                            ExpressionVariableFinder.FindExpressionVariables(args.binder, args.locals, current2);
                        }
                    }
                }
                (FixedStatementBinder, ArrayBuilder<LocalSymbol>) argument = (this, arrayBuilder);
                type.VisitRankSpecifiers(action, in argument);
                SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = _syntax.Declaration.Variables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VariableDeclaratorSyntax current = enumerator.Current;
                    arrayBuilder.Add(MakeLocal(_syntax.Declaration, current, LocalDeclarationKind.FixedVariable));
                    ExpressionVariableFinder.FindExpressionVariables(this, arrayBuilder, current);
                }
                return arrayBuilder.ToImmutable();
            }
            return ImmutableArray<LocalSymbol>.Empty;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (_syntax == scopeDesignator)
            {
                return Locals;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
