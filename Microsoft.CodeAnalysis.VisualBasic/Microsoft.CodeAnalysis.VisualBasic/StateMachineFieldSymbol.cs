using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class StateMachineFieldSymbol : SynthesizedFieldSymbol, ISynthesizedMethodBodyImplementationSymbol
	{
		internal readonly int SlotIndex;

		internal readonly LocalSlotDebugInfo SlotDebugInfo;

		public bool HasMethodBodyDependency => true;

		public IMethodSymbolInternal Method => ((ISynthesizedMethodBodyImplementationSymbol)ContainingSymbol).Method;

		public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, Symbol implicitlyDefinedBy, TypeSymbol type, string name, Accessibility accessibility = Accessibility.Private, bool isReadOnly = false, bool isShared = false, bool isSpecialNameAndRuntimeSpecial = false)
			: this(stateMachineType, implicitlyDefinedBy, type, name, new LocalSlotDebugInfo(SynthesizedLocalKind.LoweringTemp, LocalDebugId.None), -1, accessibility, isReadOnly, isShared)
		{
		}

		public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, Symbol implicitlyDefinedBy, TypeSymbol type, string name, SynthesizedLocalKind synthesizedKind, int slotindex, Accessibility accessibility = Accessibility.Private, bool isReadOnly = false, bool isShared = false, bool isSpecialNameAndRuntimeSpecial = false)
			: this(stateMachineType, implicitlyDefinedBy, type, name, new LocalSlotDebugInfo(synthesizedKind, LocalDebugId.None), slotindex, accessibility, isReadOnly, isShared)
		{
		}

		public StateMachineFieldSymbol(NamedTypeSymbol stateMachineType, Symbol implicitlyDefinedBy, TypeSymbol type, string name, LocalSlotDebugInfo slotDebugInfo, int slotIndex, Accessibility accessibility = Accessibility.Private, bool isReadOnly = false, bool isShared = false, bool isSpecialNameAndRuntimeSpecial = false)
			: base(stateMachineType, implicitlyDefinedBy, type, name, accessibility, isReadOnly, isShared, isSpecialNameAndRuntimeSpecial)
		{
			SlotIndex = slotIndex;
			SlotDebugInfo = slotDebugInfo;
		}
	}
}
