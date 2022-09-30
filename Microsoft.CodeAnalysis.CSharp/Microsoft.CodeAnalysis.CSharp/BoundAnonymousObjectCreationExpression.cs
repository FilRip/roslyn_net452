using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAnonymousObjectCreationExpression : BoundExpression
    {
        public override Symbol ExpressionSymbol => Constructor;

        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(Arguments);

        public new TypeSymbol Type => base.Type;

        public MethodSymbol Constructor { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<BoundAnonymousPropertyDeclaration> Declarations { get; }

        public BoundAnonymousObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AnonymousObjectCreationExpression, syntax, type, hasErrors || arguments.HasErrors() || declarations.HasErrors())
        {
            Constructor = constructor;
            Arguments = arguments;
            Declarations = declarations;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAnonymousObjectCreationExpression(this);
        }

        public BoundAnonymousObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, Constructor) || arguments != Arguments || declarations != Declarations || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAnonymousObjectCreationExpression boundAnonymousObjectCreationExpression = new BoundAnonymousObjectCreationExpression(Syntax, constructor, arguments, declarations, type, base.HasErrors);
                boundAnonymousObjectCreationExpression.CopyAttributes(this);
                return boundAnonymousObjectCreationExpression;
            }
            return this;
        }
    }
}
