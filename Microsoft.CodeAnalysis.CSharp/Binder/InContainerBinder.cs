// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// A binder that places the members of a symbol in scope.
    /// </summary>
    internal class InContainerBinder : Binder
    {
        private readonly NamespaceOrTypeSymbol _container;

        /// <summary>
        /// Creates a binder for a container.
        /// </summary>
        internal InContainerBinder(NamespaceOrTypeSymbol container, Binder next)
            : base(next)
        {
            _container = container;
        }

        internal NamespaceOrTypeSymbol Container
        {
            get
            {
                return _container;
            }
        }

        internal override Symbol ContainingMemberOrLambda
        {
            get
            {
                return (_container is MergedNamespaceSymbol merged) ? merged.GetConstituentForCompilation(this.Compilation) : _container;
            }
        }

        private bool IsScriptClass
        {
            get { return (_container.Kind == SymbolKind.NamedType) && ((NamedTypeSymbol)_container).IsScriptClass; }
        }

        internal override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved)
        {
            if (_container is NamedTypeSymbol type)
            {
                return this.IsSymbolAccessibleConditional(symbol, type, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo);
            }
            else
            {
                return Next.IsAccessibleHelper(symbol, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo, basesBeingResolved);  // delegate to containing Binder, eventually checking assembly.
            }
        }

        internal override bool SupportsExtensionMethods
        {
            get { return true; }
        }

        internal override void GetCandidateExtensionMethods(
            ArrayBuilder<MethodSymbol> methods,
            string name,
            int arity,
            LookupOptions options,
            Binder originalBinder)
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
                // This is the scenario where a `yield return` exists in the script file as a global statement.
                // This method is to guard against hitting `BuckStopsHereBinder` and crash. 
                return TypeWithAnnotations.Create(this.Compilation.GetSpecialType(SpecialType.System_Object));
            }
            else
            {
                // This path would eventually throw, if we didn't have the case above.
                return Next.GetIteratorElementType();
            }
        }

        internal override void LookupSymbolsInSingleBinder(
            LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // first lookup members of the namespace
            if ((options & LookupOptions.NamespaceAliasesOnly) == 0)
            {
                this.LookupMembersInternal(result, _container, name, arity, basesBeingResolved, options, originalBinder, diagnose, ref useSiteInfo);

                if (result.IsMultiViable &&
                    arity == 0 &&
                    Next is WithExternAndUsingAliasesBinder withUsingAliases && withUsingAliases.IsUsingAlias(name, originalBinder.IsSemanticModelBinder, basesBeingResolved))
                {
                    // symbols cannot conflict with using alias names
                    CSDiagnosticInfo diagInfo = new(ErrorCode.ERR_ConflictAliasAndMember, name, _container);
                    var error = new ExtendedErrorTypeSymbol((NamespaceOrTypeSymbol)null, name, arity, diagInfo, unreported: true);
                    result.SetFrom(LookupResult.Good(error)); // force lookup to be done w/ error symbol as result

                    return;
                }
            }
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo info, LookupOptions options, Binder originalBinder)
        {
            this.AddMemberLookupSymbolsInfo(info, _container, options, originalBinder);
        }

        protected override SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected override LocalFunctionSymbol LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }

        internal override uint LocalScopeDepth => Binder.ExternalScope;
    }
}
