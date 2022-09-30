using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class DeclarationComputer
    {
        public static DeclarationInfo GetDeclarationInfo(SemanticModel model, SyntaxNode node, bool getSymbol, IEnumerable<SyntaxNode>? executableCodeBlocks, CancellationToken cancellationToken)
        {
            ISymbol declaredSymbol = GetDeclaredSymbol(model, node, getSymbol, cancellationToken);
            return GetDeclarationInfo(node, declaredSymbol, executableCodeBlocks);
        }

        public static DeclarationInfo GetDeclarationInfo(SyntaxNode node, ISymbol? declaredSymbol, IEnumerable<SyntaxNode>? executableCodeBlocks)
        {
            ImmutableArray<SyntaxNode> executableCodeBlocks2 = executableCodeBlocks?.Where((SyntaxNode c) => c != null).AsImmutableOrEmpty() ?? ImmutableArray<SyntaxNode>.Empty;
            return new DeclarationInfo(node, executableCodeBlocks2, declaredSymbol);
        }

        public static DeclarationInfo GetDeclarationInfo(SemanticModel model, SyntaxNode node, bool getSymbol, CancellationToken cancellationToken)
        {
            return GetDeclarationInfo(model, node, getSymbol, (IEnumerable<SyntaxNode>?)null, cancellationToken);
        }

        public static DeclarationInfo GetDeclarationInfo(SemanticModel model, SyntaxNode node, bool getSymbol, SyntaxNode executableCodeBlock, CancellationToken cancellationToken)
        {
            return GetDeclarationInfo(model, node, getSymbol, SpecializedCollections.SingletonEnumerable(executableCodeBlock), cancellationToken);
        }

        public static DeclarationInfo GetDeclarationInfo(SemanticModel model, SyntaxNode node, bool getSymbol, CancellationToken cancellationToken, params SyntaxNode[] executableCodeBlocks)
        {
            return GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks.AsEnumerable(), cancellationToken);
        }

        private static ISymbol? GetDeclaredSymbol(SemanticModel model, SyntaxNode node, bool getSymbol, CancellationToken cancellationToken)
        {
            if (!getSymbol)
            {
                return null;
            }
            ISymbol symbol = model.GetDeclaredSymbol(node, cancellationToken);
            if (symbol is INamespaceSymbol namespaceSymbol && namespaceSymbol.ConstituentNamespaces.Length > 1)
            {
                IAssemblySymbol assemblyToScope = model.Compilation.Assembly;
                INamespaceSymbol namespaceSymbol2 = namespaceSymbol.ConstituentNamespaces.FirstOrDefault((INamespaceSymbol ns) => ns.ContainingAssembly == assemblyToScope);
                if (namespaceSymbol2 != null)
                {
                    symbol = namespaceSymbol2;
                }
            }
            return symbol;
        }
    }
}
