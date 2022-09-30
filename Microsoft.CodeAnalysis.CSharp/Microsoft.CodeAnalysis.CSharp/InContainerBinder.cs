using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class InContainerBinder : Binder
    {
        private readonly NamespaceOrTypeSymbol _container;

        internal NamespaceOrTypeSymbol Container => _container;

        internal override Symbol ContainingMemberOrLambda
        {
            get
            {
                if (!(_container is MergedNamespaceSymbol mergedNamespaceSymbol))
                {
                    return _container;
                }
                return mergedNamespaceSymbol.GetConstituentForCompilation(base.Compilation);
            }
        }

        private bool IsScriptClass
        {
            get
            {
                if (_container.Kind == SymbolKind.NamedType)
                {
                    return ((NamedTypeSymbol)_container).IsScriptClass;
                }
                return false;
            }
        }

        internal override bool SupportsExtensionMethods => true;

        internal override uint LocalScopeDepth => 0u;

        internal InContainerBinder(NamespaceOrTypeSymbol container, Binder next)
            : base(next)
        {
            _container = container;
        }

        internal override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved)
        {
            if (_container is NamedTypeSymbol within)
            {
                return IsSymbolAccessibleConditional(symbol, within, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo);
            }
            return base.Next!.IsAccessibleHelper(symbol, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo, basesBeingResolved);
        }

        internal override void GetCandidateExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name, int arity, LookupOptions options, Binder originalBinder)
        {
            if (_container.Kind == SymbolKind.Namespace)
            {
                ((NamespaceSymbol)_container).GetExtensionMethods(methods, name, arity, options);
            }
        }

        internal override TypeWithAnnotations GetIteratorElementType()
        {
            if (IsScriptClass)
            {
                return TypeWithAnnotations.Create(base.Compilation.GetSpecialType(SpecialType.System_Object));
            }
            return base.Next!.GetIteratorElementType();
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((options & LookupOptions.NamespaceAliasesOnly) == 0)
            {
                LookupMembersInternal(result, _container, name, arity, basesBeingResolved, options, originalBinder, diagnose, ref useSiteInfo);
                if (result.IsMultiViable && arity == 0 && base.Next is WithExternAndUsingAliasesBinder withExternAndUsingAliasesBinder && withExternAndUsingAliasesBinder.IsUsingAlias(name, originalBinder.IsSemanticModelBinder, basesBeingResolved))
                {
                    CSDiagnosticInfo errorInfo = new CSDiagnosticInfo(ErrorCode.ERR_ConflictAliasAndMember, name, _container);
                    ExtendedErrorTypeSymbol symbol = new ExtendedErrorTypeSymbol((NamespaceOrTypeSymbol?)null, name, arity, errorInfo, unreported: true, variableUsedBeforeDeclaration: false);
                    result.SetFrom(LookupResult.Good(symbol));
                }
            }
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            AddMemberLookupSymbolsInfo(result, _container, options, originalBinder);
        }

        protected override SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected override LocalFunctionSymbol LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }
    }
}
