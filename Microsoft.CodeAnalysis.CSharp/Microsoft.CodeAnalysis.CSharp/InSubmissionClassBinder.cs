using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class InSubmissionClassBinder : InContainerBinder
    {
        private readonly CompilationUnitSyntax _declarationSyntax;

        private readonly bool _inUsings;

        private QuickAttributeChecker? _lazyQuickAttributeChecker;

        internal override ImmutableArray<AliasAndExternAliasDirective> ExternAliases => ((SourceNamespaceSymbol)base.Compilation.SourceModule.GlobalNamespace).GetExternAliases(_declarationSyntax);

        internal override ImmutableArray<AliasAndUsingDirective> UsingAliases => ((SourceNamespaceSymbol)base.Compilation.SourceModule.GlobalNamespace).GetUsingAliases(_declarationSyntax, null);

        internal override QuickAttributeChecker QuickAttributeChecker
        {
            get
            {
                if (_lazyQuickAttributeChecker == null)
                {
                    QuickAttributeChecker quickAttributeChecker = base.Next!.QuickAttributeChecker;
                    quickAttributeChecker = (_lazyQuickAttributeChecker = quickAttributeChecker.AddAliasesIfAny(_declarationSyntax.Usings));
                }
                return _lazyQuickAttributeChecker;
            }
        }

        internal InSubmissionClassBinder(NamedTypeSymbol submissionClass, Binder next, CompilationUnitSyntax declarationSyntax, bool inUsings)
            : base(submissionClass, next)
        {
            _declarationSyntax = declarationSyntax;
            _inUsings = inUsings;
        }

        internal override void GetCandidateExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name, int arity, LookupOptions options, Binder originalBinder)
        {
            for (CSharpCompilation cSharpCompilation = base.Compilation; cSharpCompilation != null; cSharpCompilation = cSharpCompilation.PreviousSubmission)
            {
                cSharpCompilation.ScriptClass?.GetExtensionMethods(methods, name, arity, options);
            }
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            LookupMembersInSubmissions(result, (NamedTypeSymbol)base.Container, _declarationSyntax, _inUsings, name, arity, basesBeingResolved, options, originalBinder, diagnose, ref useSiteInfo);
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            AddMemberLookupSymbolsInfoInSubmissions(result, (NamedTypeSymbol)base.Container, _inUsings, options, originalBinder);
        }
    }
}
