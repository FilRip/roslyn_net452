// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// Information to be deduced while binding a foreach loop so that the loop can be lowered
    /// to a while over an enumerator.  Not applicable to the array or string forms.
    /// </summary>
    public sealed class ForEachEnumeratorInfo
    {
        // Types identified by the algorithm in the spec (8.8.4).
        public readonly TypeSymbol CollectionType;
        // public readonly TypeSymbol EnumeratorType; // redundant - return type of GetEnumeratorMethod
        public readonly TypeWithAnnotations ElementTypeWithAnnotations;
        public TypeSymbol ElementType => ElementTypeWithAnnotations.Type;

        // Members required by the "pattern" based approach.  Also populated for other approaches.
        public readonly MethodArgumentInfo GetEnumeratorInfo;
        public readonly MethodSymbol CurrentPropertyGetter;
        public readonly MethodArgumentInfo MoveNextInfo;

        // True if the enumerator needs disposal once used. 
        // Will be either IDisposable/IAsyncDisposable, or use DisposeMethod below if set
        // Computed during initial binding so that we can expose it in the semantic model.
        public readonly bool NeedsDisposal;

        public readonly bool IsAsync;

        // When async and needs disposal, this stores the information to await the DisposeAsync() invocation
        public readonly BoundAwaitableInfo? DisposeAwaitableInfo;

        // When using pattern-based Dispose, this stores the method to invoke to Dispose
        public readonly MethodArgumentInfo? PatternDisposeInfo;

        // Conversions that will be required when the foreach is lowered.
        public readonly Conversion CollectionConversion; //collection expression to collection type
        public readonly Conversion CurrentConversion; // current to element type
        // public readonly Conversion ElementConversion; // element type to iteration var type - also required for arrays, so stored elsewhere
        public readonly Conversion EnumeratorConversion; // enumerator to object

        public readonly EBinder Location;

        private ForEachEnumeratorInfo(
            TypeSymbol collectionType,
            TypeWithAnnotations elementType,
            MethodArgumentInfo getEnumeratorInfo,
            MethodSymbol currentPropertyGetter,
            MethodArgumentInfo moveNextInfo,
            bool isAsync,
            bool needsDisposal,
            BoundAwaitableInfo? disposeAwaitableInfo,
            MethodArgumentInfo? patternDisposeInfo,
            Conversion collectionConversion,
            Conversion currentConversion,
            Conversion enumeratorConversion,
            EBinder location)
        {

            this.CollectionType = collectionType;
            this.ElementTypeWithAnnotations = elementType;
            this.GetEnumeratorInfo = getEnumeratorInfo;
            this.CurrentPropertyGetter = currentPropertyGetter;
            this.MoveNextInfo = moveNextInfo;
            this.IsAsync = isAsync;
            this.NeedsDisposal = needsDisposal;
            this.DisposeAwaitableInfo = disposeAwaitableInfo;
            this.PatternDisposeInfo = patternDisposeInfo;
            this.CollectionConversion = collectionConversion;
            this.CurrentConversion = currentConversion;
            this.EnumeratorConversion = enumeratorConversion;
            this.Location = location;
        }

        // Mutable version of ForEachEnumeratorInfo.  Convert to immutable using Build.
        internal struct Builder
        {
            public TypeSymbol CollectionType;
            public TypeWithAnnotations ElementTypeWithAnnotations;
            public TypeSymbol ElementType => ElementTypeWithAnnotations.Type;

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

            public ForEachEnumeratorInfo Build(EBinder location)
            {
#nullable restore
                return new ForEachEnumeratorInfo(
                    CollectionType,
                    ElementTypeWithAnnotations,
                    GetEnumeratorInfo,
                    CurrentPropertyGetter,
                    MoveNextInfo,
                    IsAsync,
                    NeedsDisposal,
                    DisposeAwaitableInfo,
                    PatternDisposeInfo,
                    CollectionConversion,
                    CurrentConversion,
                    EnumeratorConversion,
                    location);
            }

            public bool IsIncomplete
                => GetEnumeratorInfo is null || MoveNextInfo is null || CurrentPropertyGetter is null;
        }
    }
}
