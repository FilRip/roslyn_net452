using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class ParameterSignature
    {
        internal readonly ImmutableArray<TypeWithAnnotations> parameterTypesWithAnnotations;

        internal readonly ImmutableArray<RefKind> parameterRefKinds;

        internal static readonly ParameterSignature NoParams = new ParameterSignature(ImmutableArray<TypeWithAnnotations>.Empty, default(ImmutableArray<RefKind>));

        private ParameterSignature(ImmutableArray<TypeWithAnnotations> parameterTypesWithAnnotations, ImmutableArray<RefKind> parameterRefKinds)
        {
            this.parameterTypesWithAnnotations = parameterTypesWithAnnotations;
            this.parameterRefKinds = parameterRefKinds;
        }

        private static ParameterSignature MakeParamTypesAndRefKinds(ImmutableArray<ParameterSymbol> parameters)
        {
            if (parameters.Length == 0)
            {
                return NoParams;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<RefKind> arrayBuilder = null;
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterSymbol parameterSymbol = parameters[i];
                instance.Add(parameterSymbol.TypeWithAnnotations);
                RefKind refKind = parameterSymbol.RefKind;
                if (arrayBuilder == null)
                {
                    if (refKind != 0)
                    {
                        arrayBuilder = ArrayBuilder<RefKind>.GetInstance(i, RefKind.None);
                        arrayBuilder.Add(refKind);
                    }
                }
                else
                {
                    arrayBuilder.Add(refKind);
                }
            }
            ImmutableArray<RefKind> immutableArray = arrayBuilder?.ToImmutableAndFree() ?? default(ImmutableArray<RefKind>);
            return new ParameterSignature(instance.ToImmutableAndFree(), immutableArray);
        }

        internal static void PopulateParameterSignature(ImmutableArray<ParameterSymbol> parameters, ref ParameterSignature lazySignature)
        {
            if (lazySignature == null)
            {
                Interlocked.CompareExchange(ref lazySignature, MakeParamTypesAndRefKinds(parameters), null);
            }
        }
    }
}
