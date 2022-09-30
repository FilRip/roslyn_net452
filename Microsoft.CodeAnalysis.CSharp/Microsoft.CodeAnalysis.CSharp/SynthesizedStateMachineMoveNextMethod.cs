using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SynthesizedStateMachineMoveNextMethod : SynthesizedStateMachineMethod
    {
        private ImmutableArray<CSharpAttributeData> _attributes;

        public SynthesizedStateMachineMoveNextMethod(MethodSymbol interfaceMethod, StateMachineTypeSymbol stateMachineType)
            : base("MoveNext", interfaceMethod, stateMachineType, null, generateDebugInfo: true, hasMethodBodyDependency: true)
        {
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_attributes.IsDefault)
            {
                ArrayBuilder<CSharpAttributeData> arrayBuilder = null;
                MethodSymbol kickoffMethod = base.StateMachineType.KickoffMethod;
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator = kickoffMethod.GetAttributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpAttributeData current = enumerator.Current;
                    if (current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerHiddenAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerNonUserCodeAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepperBoundaryAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepThroughAttribute))
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<CSharpAttributeData>.GetInstance(4);
                        }
                        arrayBuilder.Add(current);
                    }
                }
                ImmutableInterlocked.InterlockedCompareExchange(ref _attributes, arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<CSharpAttributeData>.Empty, default(ImmutableArray<CSharpAttributeData>));
            }
            return _attributes;
        }
    }
}
