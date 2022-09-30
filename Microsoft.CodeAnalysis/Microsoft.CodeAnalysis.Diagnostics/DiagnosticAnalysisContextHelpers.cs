using System;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal static class DiagnosticAnalysisContextHelpers
    {
        internal static void VerifyArguments<TContext>(Action<TContext> action)
        {
            VerifyAction(action);
        }

        internal static void VerifyArguments<TContext>(Action<TContext> action, ImmutableArray<SymbolKind> symbolKinds)
        {
            VerifyAction(action);
            VerifySymbolKinds(symbolKinds);
        }

        internal static void VerifyArguments<TContext, TLanguageKindEnum>(Action<TContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds) where TLanguageKindEnum : struct
        {
            VerifyAction(action);
            VerifySyntaxKinds(syntaxKinds);
        }

        internal static void VerifyArguments<TContext>(Action<TContext> action, ImmutableArray<OperationKind> operationKinds)
        {
            VerifyAction(action);
            VerifyOperationKinds(operationKinds);
        }

        internal static void VerifyArguments(Diagnostic diagnostic, Compilation? compilation, Func<Diagnostic, bool> isSupportedDiagnostic)
        {
            if (!(diagnostic is DiagnosticWithInfo))
            {
                if (diagnostic == null)
                {
                    throw new ArgumentNullException("diagnostic");
                }
                if (compilation != null)
                {
                    VerifyDiagnosticLocationsInCompilation(diagnostic, compilation);
                }
                if (!isSupportedDiagnostic(diagnostic))
                {
                    throw new ArgumentException(string.Format(CodeAnalysisResources.UnsupportedDiagnosticReported, diagnostic.Id), "diagnostic");
                }
                if (!UnicodeCharacterUtilities.IsValidIdentifier(diagnostic.Id))
                {
                    throw new ArgumentException(string.Format(CodeAnalysisResources.InvalidDiagnosticIdReported, diagnostic.Id), "diagnostic");
                }
            }
        }

        internal static void VerifyDiagnosticLocationsInCompilation(Diagnostic diagnostic, Compilation compilation)
        {
            VerifyDiagnosticLocationInCompilation(diagnostic.Id, diagnostic.Location, compilation);
            if (diagnostic.AdditionalLocations == null)
            {
                return;
            }
            foreach (Location additionalLocation in diagnostic.AdditionalLocations)
            {
                _ = additionalLocation;
                VerifyDiagnosticLocationInCompilation(diagnostic.Id, diagnostic.Location, compilation);
            }
        }

        private static void VerifyDiagnosticLocationInCompilation(string id, Location location, Compilation compilation)
        {
            if (location.IsInSource)
            {
                if (!compilation.ContainsSyntaxTree(location.SourceTree))
                {
                    throw new ArgumentException(string.Format(CodeAnalysisResources.InvalidDiagnosticLocationReported, id, location.SourceTree!.FilePath), "diagnostic");
                }
                if (location.SourceSpan.End > location.SourceTree!.Length)
                {
                    throw new ArgumentException(string.Format(CodeAnalysisResources.InvalidDiagnosticSpanReported, id, location.SourceSpan, location.SourceTree!.FilePath), "diagnostic");
                }
            }
        }

        private static void VerifyAction<TContext>(Action<TContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
        }

        private static void VerifySymbolKinds(ImmutableArray<SymbolKind> symbolKinds)
        {
            if (symbolKinds.IsDefault)
            {
                throw new ArgumentNullException("symbolKinds");
            }
            if (symbolKinds.IsEmpty)
            {
                throw new ArgumentException(CodeAnalysisResources.ArgumentCannotBeEmpty, "symbolKinds");
            }
        }

        private static void VerifySyntaxKinds<TLanguageKindEnum>(ImmutableArray<TLanguageKindEnum> syntaxKinds) where TLanguageKindEnum : struct
        {
            if (syntaxKinds.IsDefault)
            {
                throw new ArgumentNullException("syntaxKinds");
            }
            if (syntaxKinds.IsEmpty)
            {
                throw new ArgumentException(CodeAnalysisResources.ArgumentCannotBeEmpty, "syntaxKinds");
            }
        }

        private static void VerifyOperationKinds(ImmutableArray<OperationKind> operationKinds)
        {
            if (operationKinds.IsDefault)
            {
                throw new ArgumentNullException("operationKinds");
            }
            if (operationKinds.IsEmpty)
            {
                throw new ArgumentException(CodeAnalysisResources.ArgumentCannotBeEmpty, "operationKinds");
            }
        }

        internal static void VerifyArguments<TKey, TValue>(TKey key, AnalysisValueProvider<TKey, TValue> valueProvider) where TKey : class
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (valueProvider == null)
            {
                throw new ArgumentNullException("valueProvider");
            }
        }

        internal static ControlFlowGraph GetControlFlowGraph(IOperation operation, Func<IOperation, ControlFlowGraph>? getControlFlowGraph, CancellationToken cancellationToken)
        {
            IOperation rootOperation = operation.GetRootOperation();
            if (getControlFlowGraph == null)
            {
                return ControlFlowGraph.CreateCore(rootOperation, "rootOperation", cancellationToken);
            }
            return getControlFlowGraph!(rootOperation);
        }
    }
}
