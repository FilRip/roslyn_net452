using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct LoweredDynamicOperation
    {
        private readonly SyntheticBoundNodeFactory? _factory;

        private readonly TypeSymbol _resultType;

        private readonly ImmutableArray<LocalSymbol> _temps;

        public readonly BoundExpression? SiteInitialization;

        public readonly BoundExpression SiteInvocation;

        public LoweredDynamicOperation(SyntheticBoundNodeFactory? factory, BoundExpression? siteInitialization, BoundExpression siteInvocation, TypeSymbol resultType, ImmutableArray<LocalSymbol> temps)
        {
            _factory = factory;
            _resultType = resultType;
            _temps = temps;
            SiteInitialization = siteInitialization;
            SiteInvocation = siteInvocation;
        }

        public static LoweredDynamicOperation Bad(BoundExpression? loweredReceiver, ImmutableArray<BoundExpression> loweredArguments, BoundExpression? loweredRight, TypeSymbol resultType)
        {
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            instance.AddOptional(loweredReceiver);
            instance.AddRange(loweredArguments);
            instance.AddOptional(loweredRight);
            return Bad(resultType, instance.ToImmutableAndFree());
        }

        public static LoweredDynamicOperation Bad(TypeSymbol resultType, ImmutableArray<BoundExpression> children)
        {
            BoundBadExpression siteInvocation = new BoundBadExpression(children[0].Syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, children, resultType);
            return new LoweredDynamicOperation(null, null, siteInvocation, resultType, default(ImmutableArray<LocalSymbol>));
        }

        public BoundExpression ToExpression()
        {
            if (_factory == null)
            {
                return SiteInvocation;
            }
            if (_temps.IsDefaultOrEmpty)
            {
                return _factory!.Sequence(new BoundExpression[1] { SiteInitialization }, SiteInvocation, _resultType);
            }
            return new BoundSequence(_factory!.Syntax, _temps, ImmutableArray.Create(SiteInitialization), SiteInvocation, _resultType)
            {
                WasCompilerGenerated = true
            };
        }
    }
}
