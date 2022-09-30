using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class ForEachEnumeratorInfo
    {
        internal struct Builder
        {
            public TypeSymbol CollectionType;

            public TypeWithAnnotations ElementTypeWithAnnotations;

            public MethodArgumentInfo? GetEnumeratorInfo;

            public MethodSymbol CurrentPropertyGetter;

            public MethodArgumentInfo? MoveNextInfo;

            public bool IsAsync;

            public bool NeedsDisposal;

            public BoundAwaitableInfo? DisposeAwaitableInfo;

            public MethodArgumentInfo? PatternDisposeInfo;

            public Conversion CollectionConversion;

            public Conversion CurrentConversion;

            public Conversion EnumeratorConversion;

            public TypeSymbol ElementType => ElementTypeWithAnnotations.Type;

            public bool IsIncomplete
            {
                get
                {
                    if ((object)GetEnumeratorInfo != null && (object)MoveNextInfo != null)
                    {
                        return (object)CurrentPropertyGetter == null;
                    }
                    return true;
                }
            }

            public ForEachEnumeratorInfo Build(BinderFlags location)
            {
                return new ForEachEnumeratorInfo(CollectionType, ElementTypeWithAnnotations, GetEnumeratorInfo, CurrentPropertyGetter, MoveNextInfo, IsAsync, NeedsDisposal, DisposeAwaitableInfo, PatternDisposeInfo, CollectionConversion, CurrentConversion, EnumeratorConversion, location);
            }
        }

        public readonly TypeSymbol CollectionType;

        public readonly TypeWithAnnotations ElementTypeWithAnnotations;

        public readonly MethodArgumentInfo GetEnumeratorInfo;

        public readonly MethodSymbol CurrentPropertyGetter;

        public readonly MethodArgumentInfo MoveNextInfo;

        public readonly bool NeedsDisposal;

        public readonly bool IsAsync;

        public readonly BoundAwaitableInfo? DisposeAwaitableInfo;

        public readonly MethodArgumentInfo? PatternDisposeInfo;

        public readonly Conversion CollectionConversion;

        public readonly Conversion CurrentConversion;

        public readonly Conversion EnumeratorConversion;

        public readonly BinderFlags Location;

        public TypeSymbol ElementType => ElementTypeWithAnnotations.Type;

        private ForEachEnumeratorInfo(TypeSymbol collectionType, TypeWithAnnotations elementType, MethodArgumentInfo getEnumeratorInfo, MethodSymbol currentPropertyGetter, MethodArgumentInfo moveNextInfo, bool isAsync, bool needsDisposal, BoundAwaitableInfo? disposeAwaitableInfo, MethodArgumentInfo? patternDisposeInfo, Conversion collectionConversion, Conversion currentConversion, Conversion enumeratorConversion, BinderFlags location)
        {
            CollectionType = collectionType;
            ElementTypeWithAnnotations = elementType;
            GetEnumeratorInfo = getEnumeratorInfo;
            CurrentPropertyGetter = currentPropertyGetter;
            MoveNextInfo = moveNextInfo;
            IsAsync = isAsync;
            NeedsDisposal = needsDisposal;
            DisposeAwaitableInfo = disposeAwaitableInfo;
            PatternDisposeInfo = patternDisposeInfo;
            CollectionConversion = collectionConversion;
            CurrentConversion = currentConversion;
            EnumeratorConversion = enumeratorConversion;
            Location = location;
        }
    }
}
