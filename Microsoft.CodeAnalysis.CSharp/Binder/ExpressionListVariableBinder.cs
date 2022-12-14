// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class ExpressionListVariableBinder : LocalScopeBinder
    {
        private readonly SeparatedSyntaxList<ExpressionSyntax> _expressions;

        internal ExpressionListVariableBinder(SeparatedSyntaxList<ExpressionSyntax> expressions, Binder next) : base(next)
        {
            _expressions = expressions;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            var builder = ArrayBuilder<LocalSymbol>.GetInstance();
            ExpressionVariableFinder.FindExpressionVariables(this, builder, _expressions);
            return builder.ToImmutableAndFree();
        }

        internal override SyntaxNode ScopeDesignator
        {
            get
            {
                return _expressions[0];
            }
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (ScopeDesignator == scopeDesignator)
            {
                return this.Locals;
            }

            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
