using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SynthesizedClosureEnvironmentConstructor : SynthesizedInstanceConstructor, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => ((ISynthesizedMethodBodyImplementationSymbol)ContainingSymbol).Method;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => false;

        internal SynthesizedClosureEnvironmentConstructor(SynthesizedClosureEnvironment frame)
            : base(frame)
        {
        }
    }
}
