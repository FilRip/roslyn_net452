using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class StateMachineFieldSymbol : SynthesizedFieldSymbolBase, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private readonly TypeWithAnnotations _type;

        private readonly bool _isThis;

        internal readonly int SlotIndex;

        internal readonly LocalSlotDebugInfo SlotDebugInfo;

        internal override bool SuppressDynamicAttribute => true;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => true;

        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => ((ISynthesizedMethodBodyImplementationSymbol)ContainingSymbol).Method;

        internal override bool IsCapturedFrame => _isThis;

        public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, TypeWithAnnotations type, string name, bool isPublic, bool isThis)
            : this(stateMachineType, type, name, new LocalSlotDebugInfo(SynthesizedLocalKind.LoweringTemp, LocalDebugId.None), -1, isPublic)
        {
            _isThis = isThis;
        }

        public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, TypeSymbol type, string name, SynthesizedLocalKind synthesizedKind, int slotIndex, bool isPublic)
            : this(stateMachineType, type, name, new LocalSlotDebugInfo(synthesizedKind, LocalDebugId.None), slotIndex, isPublic)
        {
        }

        public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, TypeSymbol type, string name, LocalSlotDebugInfo slotDebugInfo, int slotIndex, bool isPublic)
            : this(stateMachineType, TypeWithAnnotations.Create(type), name, slotDebugInfo, slotIndex, isPublic)
        {
        }

        public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, TypeWithAnnotations type, string name, LocalSlotDebugInfo slotDebugInfo, int slotIndex, bool isPublic)
            : base(stateMachineType, name, isPublic, isReadOnly: false, isStatic: false)
        {
            _type = type;
            SlotIndex = slotIndex;
            SlotDebugInfo = slotDebugInfo;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _type;
        }
    }
}
