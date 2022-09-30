using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundCollectionElementInitializer : BoundExpression, IBoundInvalidNode
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol ExpressionSymbol => AddMethod;

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ImplicitReceiverOpt, Arguments);

        public new TypeSymbol Type => base.Type;

        public MethodSymbol AddMethod { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public BoundExpression? ImplicitReceiverOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public bool InvokedAsExtensionMethod { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public BoundCollectionElementInitializer(SyntaxNode syntax, MethodSymbol addMethod, ImmutableArray<BoundExpression> arguments, BoundExpression? implicitReceiverOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool invokedAsExtensionMethod, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CollectionElementInitializer, syntax, type, hasErrors || arguments.HasErrors() || implicitReceiverOpt.HasErrors())
        {
            AddMethod = addMethod;
            Arguments = arguments;
            ImplicitReceiverOpt = implicitReceiverOpt;
            Expanded = expanded;
            ArgsToParamsOpt = argsToParamsOpt;
            DefaultArguments = defaultArguments;
            InvokedAsExtensionMethod = invokedAsExtensionMethod;
            _ResultKind = resultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitCollectionElementInitializer(this);
        }

        public BoundCollectionElementInitializer Update(MethodSymbol addMethod, ImmutableArray<BoundExpression> arguments, BoundExpression? implicitReceiverOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool invokedAsExtensionMethod, LookupResultKind resultKind, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(addMethod, AddMethod) || arguments != Arguments || implicitReceiverOpt != ImplicitReceiverOpt || expanded != Expanded || argsToParamsOpt != ArgsToParamsOpt || defaultArguments != DefaultArguments || invokedAsExtensionMethod != InvokedAsExtensionMethod || resultKind != ResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundCollectionElementInitializer boundCollectionElementInitializer = new BoundCollectionElementInitializer(Syntax, addMethod, arguments, implicitReceiverOpt, expanded, argsToParamsOpt, defaultArguments, invokedAsExtensionMethod, resultKind, type, base.HasErrors);
                boundCollectionElementInitializer.CopyAttributes(this);
                return boundCollectionElementInitializer;
            }
            return this;
        }
    }
}
