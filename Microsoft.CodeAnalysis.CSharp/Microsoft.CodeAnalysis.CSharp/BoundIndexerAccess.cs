using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIndexerAccess : BoundExpression, IBoundInvalidNode
    {
        public override Symbol? ExpressionSymbol => Indexer;

        public override LookupResultKind ResultKind
        {
            get
            {
                if (OriginalIndexersOpt.IsDefault)
                {
                    return base.ResultKind;
                }
                return LookupResultKind.OverloadResolutionFailure;
            }
        }

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ReceiverOpt, Arguments);

        public new TypeSymbol Type => base.Type;

        public BoundExpression? ReceiverOpt { get; }

        public PropertySymbol Indexer { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public ImmutableArray<PropertySymbol> OriginalIndexersOpt { get; }

        public static BoundIndexerAccess ErrorAccess(SyntaxNode node, BoundExpression receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> namedArguments, ImmutableArray<RefKind> refKinds, ImmutableArray<PropertySymbol> originalIndexers)
        {
            return new BoundIndexerAccess(node, receiverOpt, indexer, arguments, namedArguments, refKinds, expanded: false, default(ImmutableArray<int>), default(BitVector), originalIndexers, indexer.Type, hasErrors: true);
        }

        public BoundIndexerAccess(SyntaxNode syntax, BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, TypeSymbol type, bool hasErrors = false)
            : this(syntax, receiverOpt, indexer, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, default(ImmutableArray<PropertySymbol>), type, hasErrors)
        {
        }

        public BoundIndexerAccess Update(BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, TypeSymbol type)
        {
            return Update(receiverOpt, indexer, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, OriginalIndexersOpt, type);
        }

        public BoundIndexerAccess(SyntaxNode syntax, BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ImmutableArray<PropertySymbol> originalIndexersOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IndexerAccess, syntax, type, hasErrors || receiverOpt.HasErrors() || arguments.HasErrors())
        {
            ReceiverOpt = receiverOpt;
            Indexer = indexer;
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            Expanded = expanded;
            ArgsToParamsOpt = argsToParamsOpt;
            DefaultArguments = defaultArguments;
            OriginalIndexersOpt = originalIndexersOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIndexerAccess(this);
        }

        public BoundIndexerAccess Update(BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ImmutableArray<PropertySymbol> originalIndexersOpt, TypeSymbol type)
        {
            if (receiverOpt != ReceiverOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(indexer, Indexer) || arguments != Arguments || argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || expanded != Expanded || argsToParamsOpt != ArgsToParamsOpt || defaultArguments != DefaultArguments || originalIndexersOpt != OriginalIndexersOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundIndexerAccess boundIndexerAccess = new BoundIndexerAccess(Syntax, receiverOpt, indexer, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, originalIndexersOpt, type, base.HasErrors);
                boundIndexerAccess.CopyAttributes(this);
                return boundIndexerAccess;
            }
            return this;
        }
    }
}
