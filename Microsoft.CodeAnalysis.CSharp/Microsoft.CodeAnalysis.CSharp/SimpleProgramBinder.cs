using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SimpleProgramBinder : LocalScopeBinder
    {
        private readonly SynthesizedSimpleProgramEntryPointSymbol _entryPoint;

        internal override bool IsLocalFunctionsScopeBinder => true;

        internal override bool IsLabelsScopeBinder => true;

        internal override SyntaxNode ScopeDesignator => _entryPoint.SyntaxNode;

        public SimpleProgramBinder(Binder enclosing, SynthesizedSimpleProgramEntryPointSymbol entryPoint)
            : base(enclosing, enclosing.Flags)
        {
            _entryPoint = entryPoint;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = _entryPoint.CompilationUnit.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is GlobalStatementSyntax globalStatementSyntax)
                {
                    BuildLocals(this, globalStatementSyntax.Statement, instance);
                }
            }
            return instance.ToImmutableAndFree();
        }

        protected override ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions()
        {
            ArrayBuilder<LocalFunctionSymbol> locals = null;
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = _entryPoint.CompilationUnit.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is GlobalStatementSyntax globalStatementSyntax)
                {
                    BuildLocalFunctions(globalStatementSyntax.Statement, ref locals);
                }
            }
            return locals?.ToImmutableAndFree() ?? ImmutableArray<LocalFunctionSymbol>.Empty;
        }

        protected override ImmutableArray<LabelSymbol> BuildLabels()
        {
            ArrayBuilder<LabelSymbol> labels = null;
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = _entryPoint.CompilationUnit.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is GlobalStatementSyntax globalStatementSyntax)
                {
                    LocalScopeBinder.BuildLabels(_entryPoint, globalStatementSyntax.Statement, ref labels);
                }
            }
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
