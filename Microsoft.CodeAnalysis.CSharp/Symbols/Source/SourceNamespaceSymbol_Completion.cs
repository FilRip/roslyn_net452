// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal partial class SourceNamespaceSymbol
    {
        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            while (true)
            {
                bool exit = false;
                cancellationToken.ThrowIfCancellationRequested();
                var incompletePart = _state.NextIncompletePart;
                switch (incompletePart)
                {
                    case CompletionPart.NameToMembersMap:
                        {
                            var tmp = GetNameToMembersMap();
                        }
                        break;

                    case CompletionPart.MembersCompleted:
                        {
                            SingleNamespaceDeclaration targetDeclarationWithImports = null;

                            // ensure relevant imports are complete.
                            foreach (var declaration in _mergedDeclaration.Declarations)
                            {
                                if (locationOpt == null || locationOpt.SourceTree == declaration.SyntaxReference.SyntaxTree)
                                {
                                    if (declaration.HasGlobalUsings || declaration.HasUsings || declaration.HasExternAliases)
                                    {
                                        targetDeclarationWithImports = declaration;
                                        _aliasesAndUsings[declaration].Complete(this, declaration.SyntaxReference, cancellationToken);
                                    }
                                }
                            }

                            if (IsGlobalNamespace && (locationOpt is null || targetDeclarationWithImports is not null))
                            {
                                GetMergedGlobalAliasesAndUsings(basesBeingResolved: null, cancellationToken).Complete(this, cancellationToken);
                            }

                            var members = this.GetMembers();

                            bool allCompleted = true;

                            if (this.DeclaringCompilation.Options.ConcurrentBuild)
                            {
                                RoslynParallel.For(
                                    0,
                                    members.Length,
                                    UICultureUtilities.WithCurrentUICulture<int>(i => ForceCompleteMemberByLocation(locationOpt, members[i], cancellationToken)),
                                    cancellationToken);

                                foreach (var member in members)
                                {
                                    if (!member.HasComplete(CompletionPart.All))
                                    {
                                        allCompleted = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var member in members)
                                {
                                    ForceCompleteMemberByLocation(locationOpt, member, cancellationToken);
                                    allCompleted = allCompleted && member.HasComplete(CompletionPart.All);
                                }
                            }

                            if (allCompleted)
                            {
                                _state.NotePartComplete(CompletionPart.MembersCompleted);
                                break;
                            }
                            else
                            {
                                // NOTE: we're going to kick out of the completion part loop after this,
                                // so not making progress isn't a problem.
                                exit = true;
                                break;
                            }
                        }

                    case CompletionPart.None:
                        return;

                    default:
                        // any other values are completion parts intended for other kinds of symbols
                        _state.NotePartComplete(CompletionPart.All & ~CompletionPart.NamespaceSymbolAll);
                        break;
                }

                if (exit)
                    break;
                _state.SpinWaitComplete(incompletePart, cancellationToken);
            }

            // Don't return until we've seen all of the CompletionParts. This ensures all
            // diagnostics have been reported (not necessarily on this thread).
            CompletionPart allParts = (locationOpt == null) ? CompletionPart.NamespaceSymbolAll : CompletionPart.NamespaceSymbolAll & ~CompletionPart.MembersCompleted;
            _state.SpinWaitComplete(allParts, cancellationToken);
        }

        internal override bool HasComplete(CompletionPart part)
        {
            return _state.HasComplete(part);
        }
    }
}
