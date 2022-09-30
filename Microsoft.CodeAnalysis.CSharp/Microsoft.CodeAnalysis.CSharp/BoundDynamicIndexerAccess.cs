using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDynamicIndexerAccess : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(Arguments.Insert(0, Receiver));

        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public ImmutableArray<PropertySymbol> ApplicableIndexers { get; }

        internal string? TryGetIndexedPropertyName()
        {
            ImmutableArray<PropertySymbol>.Enumerator enumerator = ApplicableIndexers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropertySymbol current = enumerator.Current;
                if (!current.IsIndexer && current.IsIndexedProperty)
                {
                    return current.Name;
                }
            }
            return null;
        }

        public BoundDynamicIndexerAccess(SyntaxNode syntax, BoundExpression receiver, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<PropertySymbol> applicableIndexers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicIndexerAccess, syntax, type, hasErrors || receiver.HasErrors() || arguments.HasErrors())
        {
            Receiver = receiver;
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            ApplicableIndexers = applicableIndexers;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDynamicIndexerAccess(this);
        }

        public BoundDynamicIndexerAccess Update(BoundExpression receiver, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<PropertySymbol> applicableIndexers, TypeSymbol type)
        {
            if (receiver != Receiver || arguments != Arguments || argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || applicableIndexers != ApplicableIndexers || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDynamicIndexerAccess boundDynamicIndexerAccess = new BoundDynamicIndexerAccess(Syntax, receiver, arguments, argumentNamesOpt, argumentRefKindsOpt, applicableIndexers, type, base.HasErrors);
                boundDynamicIndexerAccess.CopyAttributes(this);
                return boundDynamicIndexerAccess;
            }
            return this;
        }
    }
}
