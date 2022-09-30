using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class StateMachineTypeSymbol : SynthesizedContainer, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private ImmutableArray<CSharpAttributeData> _attributes;

        public readonly MethodSymbol KickoffMethod;

        public override Symbol ContainingSymbol => KickoffMethod.ContainingType;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => true;

        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => KickoffMethod;

        public sealed override bool AreLocalsZeroed => KickoffMethod.AreLocalsZeroed;

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        public StateMachineTypeSymbol(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol kickoffMethod, int kickoffMethodOrdinal)
            : base(MakeName(slotAllocatorOpt, compilationState, kickoffMethod, kickoffMethodOrdinal), kickoffMethod)
        {
            KickoffMethod = kickoffMethod;
        }

        private static string MakeName(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol kickoffMethod, int kickoffMethodOrdinal)
        {
            return slotAllocatorOpt?.PreviousStateMachineTypeName ?? GeneratedNames.MakeStateMachineTypeName(kickoffMethod.Name, kickoffMethodOrdinal, compilationState.ModuleBuilderOpt!.CurrentGenerationOrdinal);
        }

        private static int SequenceNumber(MethodSymbol kickoffMethod)
        {
            int num = 0;
            ImmutableArray<Symbol>.Enumerator enumerator = kickoffMethod.ContainingType.GetMembers(kickoffMethod.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                num++;
                if ((object)kickoffMethod == current)
                {
                    return num;
                }
            }
            return num;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_attributes.IsDefault)
            {
                ArrayBuilder<CSharpAttributeData> arrayBuilder = null;
                NamedTypeSymbol containingType = KickoffMethod.ContainingType;
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator = containingType.GetAttributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpAttributeData current = enumerator.Current;
                    if (current.IsTargetAttribute(containingType, AttributeDescription.DebuggerNonUserCodeAttribute) || current.IsTargetAttribute(containingType, AttributeDescription.DebuggerStepThroughAttribute))
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<CSharpAttributeData>.GetInstance(2);
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
