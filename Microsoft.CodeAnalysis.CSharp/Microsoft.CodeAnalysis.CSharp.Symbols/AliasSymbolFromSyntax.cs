using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class AliasSymbolFromSyntax : AliasSymbol
    {
        private readonly SyntaxReference _directive;

        private SymbolCompletionState _state;

        private NamespaceOrTypeSymbol? _aliasTarget;

        private BindingDiagnosticBag? _aliasTargetDiagnostics;

        public override NamespaceOrTypeSymbol Target => GetAliasTarget(null);

        internal BindingDiagnosticBag AliasTargetDiagnostics
        {
            get
            {
                GetAliasTarget(null);
                return _aliasTargetDiagnostics;
            }
        }

        internal override bool RequiresCompletion => true;

        internal AliasSymbolFromSyntax(SourceNamespaceSymbol containingSymbol, UsingDirectiveSyntax syntax)
            : base(syntax.Alias!.Name.Identifier.ValueText, containingSymbol, ImmutableArray.Create(syntax.Alias!.Name.Identifier.GetLocation()), isExtern: false)
        {
            _directive = syntax.GetReference();
        }

        internal AliasSymbolFromSyntax(SourceNamespaceSymbol containingSymbol, ExternAliasDirectiveSyntax syntax)
            : base(syntax.Identifier.ValueText, containingSymbol, ImmutableArray.Create(syntax.Identifier.GetLocation()), isExtern: true)
        {
            _directive = syntax.GetReference();
        }

        internal override NamespaceOrTypeSymbol GetAliasTarget(ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (!_state.HasComplete(CompletionPart.StartBaseType))
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                NamespaceOrTypeSymbol value = (IsExtern ? ResolveExternAliasTarget(instance) : ResolveAliasTarget(((UsingDirectiveSyntax)_directive.GetSyntax()).Name, instance, basesBeingResolved));
                if ((object)Interlocked.CompareExchange(ref _aliasTarget, value, null) == null)
                {
                    Interlocked.Exchange(ref _aliasTargetDiagnostics, instance);
                    _state.NotePartComplete(CompletionPart.StartBaseType);
                }
                else
                {
                    instance.Free();
                    _state.SpinWaitComplete(CompletionPart.StartBaseType, default(CancellationToken));
                }
            }
            return _aliasTarget;
        }

        private NamespaceSymbol ResolveExternAliasTarget(BindingDiagnosticBag diagnostics)
        {
            if (!ContainingSymbol!.DeclaringCompilation.GetExternAliasTarget(Name, out var @namespace))
            {
                diagnostics.Add(ErrorCode.ERR_BadExternAlias, Locations[0], Name);
            }
            return @namespace;
        }

        private NamespaceOrTypeSymbol ResolveAliasTarget(NameSyntax syntax, BindingDiagnosticBag diagnostics, ConsList<TypeSymbol>? basesBeingResolved)
        {
            return ContainingSymbol!.DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax).WithAdditionalFlags(BinderFlags.SuppressConstraintChecks | BinderFlags.SuppressObsoleteChecks)
                .BindNamespaceOrTypeSymbol(syntax, diagnostics, basesBeingResolved)
                .NamespaceOrTypeSymbol;
        }
    }
}
