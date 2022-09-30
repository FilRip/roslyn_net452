using System.Collections.Immutable;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public class ForEachLoopOperationInfo
    {
        public readonly ITypeSymbol ElementType;

        public readonly IMethodSymbol GetEnumeratorMethod;

        public readonly IPropertySymbol CurrentProperty;

        public readonly IMethodSymbol MoveNextMethod;

        public readonly bool IsAsynchronous;

        public readonly bool NeedsDispose;

        public readonly bool KnownToImplementIDisposable;

        public readonly IMethodSymbol? PatternDisposeMethod;

        public readonly IConvertibleConversion CurrentConversion;

        public readonly IConvertibleConversion ElementConversion;

        public readonly ImmutableArray<IArgumentOperation> GetEnumeratorArguments;

        public readonly ImmutableArray<IArgumentOperation> MoveNextArguments;

        public readonly ImmutableArray<IArgumentOperation> CurrentArguments;

        public readonly ImmutableArray<IArgumentOperation> DisposeArguments;

        public ForEachLoopOperationInfo(ITypeSymbol elementType, IMethodSymbol getEnumeratorMethod, IPropertySymbol currentProperty, IMethodSymbol moveNextMethod, bool isAsynchronous, bool needsDispose, bool knownToImplementIDisposable, IMethodSymbol? patternDisposeMethod, IConvertibleConversion currentConversion, IConvertibleConversion elementConversion, ImmutableArray<IArgumentOperation> getEnumeratorArguments = default(ImmutableArray<IArgumentOperation>), ImmutableArray<IArgumentOperation> moveNextArguments = default(ImmutableArray<IArgumentOperation>), ImmutableArray<IArgumentOperation> currentArguments = default(ImmutableArray<IArgumentOperation>), ImmutableArray<IArgumentOperation> disposeArguments = default(ImmutableArray<IArgumentOperation>))
        {
            ElementType = elementType;
            GetEnumeratorMethod = getEnumeratorMethod;
            CurrentProperty = currentProperty;
            MoveNextMethod = moveNextMethod;
            IsAsynchronous = isAsynchronous;
            KnownToImplementIDisposable = knownToImplementIDisposable;
            NeedsDispose = needsDispose;
            PatternDisposeMethod = patternDisposeMethod;
            CurrentConversion = currentConversion;
            ElementConversion = elementConversion;
            GetEnumeratorArguments = getEnumeratorArguments;
            MoveNextArguments = moveNextArguments;
            CurrentArguments = currentArguments;
            DisposeArguments = disposeArguments;
        }
    }
}
