using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class HasDynamicArgumentsExpression : Operation
    {
        public ImmutableArray<string> ArgumentNames { get; }

        public ImmutableArray<RefKind> ArgumentRefKinds { get; }

        public ImmutableArray<IOperation> Arguments { get; }

        public override ITypeSymbol? Type { get; }

        protected HasDynamicArgumentsExpression(ImmutableArray<IOperation> arguments, ImmutableArray<string> argumentNames, ImmutableArray<RefKind> argumentRefKinds, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Arguments = Operation.SetParentOperation(arguments, this);
            ArgumentNames = argumentNames;
            ArgumentRefKinds = argumentRefKinds;
            Type = type;
        }
    }
}
