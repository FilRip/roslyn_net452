using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNamespaceExpression : BoundExpression
    {
        public override Symbol ExpressionSymbol => (Symbol)(AliasOpt ?? ((object)NamespaceSymbol));

        public new TypeSymbol? Type => base.Type;

        public NamespaceSymbol NamespaceSymbol { get; }

        public AliasSymbol? AliasOpt { get; }

        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol, bool hasErrors = false)
            : this(syntax, namespaceSymbol, null, hasErrors)
        {
        }

        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol)
            : this(syntax, namespaceSymbol, null)
        {
        }

        public BoundNamespaceExpression Update(NamespaceSymbol namespaceSymbol)
        {
            return Update(namespaceSymbol, AliasOpt);
        }

        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt, bool hasErrors)
            : base(BoundKind.NamespaceExpression, syntax, null, hasErrors)
        {
            NamespaceSymbol = namespaceSymbol;
            AliasOpt = aliasOpt;
        }

        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt)
            : base(BoundKind.NamespaceExpression, syntax, null)
        {
            NamespaceSymbol = namespaceSymbol;
            AliasOpt = aliasOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNamespaceExpression(this);
        }

        public BoundNamespaceExpression Update(NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(namespaceSymbol, NamespaceSymbol) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(aliasOpt, AliasOpt))
            {
                BoundNamespaceExpression boundNamespaceExpression = new BoundNamespaceExpression(Syntax, namespaceSymbol, aliasOpt, base.HasErrors);
                boundNamespaceExpression.CopyAttributes(this);
                return boundNamespaceExpression;
            }
            return this;
        }
    }
}
