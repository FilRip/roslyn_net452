using System.Collections.Immutable;
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
    public sealed class BoundAttribute : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol? ExpressionSymbol => Constructor;

        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(ConstructorArguments.AddRange(StaticCast<BoundExpression>.From(NamedArguments)));

        public new TypeSymbol Type => base.Type;

        public MethodSymbol? Constructor { get; }

        public ImmutableArray<BoundExpression> ConstructorArguments { get; }

        public ImmutableArray<string> ConstructorArgumentNamesOpt { get; }

        public ImmutableArray<int> ConstructorArgumentsToParamsOpt { get; }

        public bool ConstructorExpanded { get; }

        public ImmutableArray<BoundAssignmentOperator> NamedArguments { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public BoundAttribute(SyntaxNode syntax, MethodSymbol? constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<string> constructorArgumentNamesOpt, ImmutableArray<int> constructorArgumentsToParamsOpt, bool constructorExpanded, ImmutableArray<BoundAssignmentOperator> namedArguments, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Attribute, syntax, type, hasErrors || constructorArguments.HasErrors() || namedArguments.HasErrors())
        {
            Constructor = constructor;
            ConstructorArguments = constructorArguments;
            ConstructorArgumentNamesOpt = constructorArgumentNamesOpt;
            ConstructorArgumentsToParamsOpt = constructorArgumentsToParamsOpt;
            ConstructorExpanded = constructorExpanded;
            NamedArguments = namedArguments;
            _ResultKind = resultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAttribute(this);
        }

        public BoundAttribute Update(MethodSymbol? constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<string> constructorArgumentNamesOpt, ImmutableArray<int> constructorArgumentsToParamsOpt, bool constructorExpanded, ImmutableArray<BoundAssignmentOperator> namedArguments, LookupResultKind resultKind, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, Constructor) || constructorArguments != ConstructorArguments || constructorArgumentNamesOpt != ConstructorArgumentNamesOpt || constructorArgumentsToParamsOpt != ConstructorArgumentsToParamsOpt || constructorExpanded != ConstructorExpanded || namedArguments != NamedArguments || resultKind != ResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAttribute boundAttribute = new BoundAttribute(Syntax, constructor, constructorArguments, constructorArgumentNamesOpt, constructorArgumentsToParamsOpt, constructorExpanded, namedArguments, resultKind, type, base.HasErrors);
                boundAttribute.CopyAttributes(this);
                return boundAttribute;
            }
            return this;
        }
    }
}
