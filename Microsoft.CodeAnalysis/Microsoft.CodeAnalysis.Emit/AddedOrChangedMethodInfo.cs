using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    internal readonly struct AddedOrChangedMethodInfo
    {
        public readonly DebugId MethodId;

        public readonly ImmutableArray<EncLocalInfo> Locals;

        public readonly ImmutableArray<LambdaDebugInfo> LambdaDebugInfo;

        public readonly ImmutableArray<ClosureDebugInfo> ClosureDebugInfo;

        public readonly string? StateMachineTypeName;

        public readonly ImmutableArray<EncHoistedLocalInfo> StateMachineHoistedLocalSlotsOpt;

        public readonly ImmutableArray<ITypeReference?> StateMachineAwaiterSlotsOpt;

        public AddedOrChangedMethodInfo(DebugId methodId, ImmutableArray<EncLocalInfo> locals, ImmutableArray<LambdaDebugInfo> lambdaDebugInfo, ImmutableArray<ClosureDebugInfo> closureDebugInfo, string? stateMachineTypeName, ImmutableArray<EncHoistedLocalInfo> stateMachineHoistedLocalSlotsOpt, ImmutableArray<ITypeReference?> stateMachineAwaiterSlotsOpt)
        {
            MethodId = methodId;
            Locals = locals;
            LambdaDebugInfo = lambdaDebugInfo;
            ClosureDebugInfo = closureDebugInfo;
            StateMachineTypeName = stateMachineTypeName;
            StateMachineHoistedLocalSlotsOpt = stateMachineHoistedLocalSlotsOpt;
            StateMachineAwaiterSlotsOpt = stateMachineAwaiterSlotsOpt;
        }

        public AddedOrChangedMethodInfo MapTypes(SymbolMatcher map)
        {
            ImmutableArray<EncLocalInfo> locals = ImmutableArray.CreateRange(Locals, MapLocalInfo, map);
            ImmutableArray<EncHoistedLocalInfo> stateMachineHoistedLocalSlotsOpt = (StateMachineHoistedLocalSlotsOpt.IsDefault ? default(ImmutableArray<EncHoistedLocalInfo>) : ImmutableArray.CreateRange(StateMachineHoistedLocalSlotsOpt, MapHoistedLocalSlot, map));
            ImmutableArray<ITypeReference> stateMachineAwaiterSlotsOpt = (StateMachineAwaiterSlotsOpt.IsDefault ? default(ImmutableArray<ITypeReference>) : ImmutableArray.CreateRange<ITypeReference, SymbolMatcher, ITypeReference>(StateMachineAwaiterSlotsOpt, (ITypeReference typeRef, SymbolMatcher map) => (typeRef != null) ? map.MapReference(typeRef) : null, map));
            return new AddedOrChangedMethodInfo(MethodId, locals, LambdaDebugInfo, ClosureDebugInfo, StateMachineTypeName, stateMachineHoistedLocalSlotsOpt, stateMachineAwaiterSlotsOpt);
        }

        private static EncLocalInfo MapLocalInfo(EncLocalInfo info, SymbolMatcher map)
        {
            if (info.Type == null)
            {
                return info;
            }
            ITypeReference type = map.MapReference(info.Type);
            return new EncLocalInfo(info.SlotInfo, type, info.Constraints, info.Signature);
        }

        private static EncHoistedLocalInfo MapHoistedLocalSlot(EncHoistedLocalInfo info, SymbolMatcher map)
        {
            if (info.Type == null)
            {
                return info;
            }
            ITypeReference type = map.MapReference(info.Type);
            return new EncHoistedLocalInfo(info.SlotInfo, type);
        }
    }
}
