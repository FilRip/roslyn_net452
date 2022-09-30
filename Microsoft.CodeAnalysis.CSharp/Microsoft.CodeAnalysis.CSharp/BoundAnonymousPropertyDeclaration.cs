using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAnonymousPropertyDeclaration : BoundExpression
    {
        public override Symbol ExpressionSymbol => Property;

        public new TypeSymbol Type => base.Type;

        public PropertySymbol Property { get; }

        public BoundAnonymousPropertyDeclaration(SyntaxNode syntax, PropertySymbol property, TypeSymbol type, bool hasErrors)
            : base(BoundKind.AnonymousPropertyDeclaration, syntax, type, hasErrors)
        {
            Property = property;
        }

        public BoundAnonymousPropertyDeclaration(SyntaxNode syntax, PropertySymbol property, TypeSymbol type)
            : base(BoundKind.AnonymousPropertyDeclaration, syntax, type)
        {
            Property = property;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAnonymousPropertyDeclaration(this);
        }

        public BoundAnonymousPropertyDeclaration Update(PropertySymbol property, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, Property) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAnonymousPropertyDeclaration boundAnonymousPropertyDeclaration = new BoundAnonymousPropertyDeclaration(Syntax, property, type, base.HasErrors);
                boundAnonymousPropertyDeclaration.CopyAttributes(this);
                return boundAnonymousPropertyDeclaration;
            }
            return this;
        }
    }
}
