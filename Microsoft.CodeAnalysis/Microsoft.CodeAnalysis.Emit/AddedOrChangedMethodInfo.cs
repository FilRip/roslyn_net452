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
            var mappedLocals = ImmutableArray.CreateRange(Locals, MapLocalInfo, map);

            var mappedHoistedLocalSlots = StateMachineHoistedLocalSlotsOpt.IsDefault ? default :
                ImmutableArray.CreateRange(StateMachineHoistedLocalSlotsOpt, MapHoistedLocalSlot, map);

            var mappedAwaiterSlots = StateMachineAwaiterSlotsOpt.IsDefault ? default :
                ImmutableArray.CreateRange(StateMachineAwaiterSlotsOpt, static (typeRef, map) => (typeRef is null) ? null : map.MapReference(typeRef), map);

            return new AddedOrChangedMethodInfo(MethodId, mappedLocals, LambdaDebugInfo, ClosureDebugInfo, StateMachineTypeName, mappedHoistedLocalSlots, mappedAwaiterSlots);
        }

        private static EncLocalInfo MapLocalInfo(EncLocalInfo info, SymbolMatcher map)
        {
            if (info.Type is null)
            {
                return info;
            }

            var typeRef = map.MapReference(info.Type);

            return new EncLocalInfo(info.SlotInfo, typeRef, info.Constraints, info.Signature);
        }

        private static EncHoistedLocalInfo MapHoistedLocalSlot(EncHoistedLocalInfo info, SymbolMatcher map)
        {
            if (info.Type is null)
            {
                return info;
            }

            var typeRef = map.MapReference(info.Type);

            return new EncHoistedLocalInfo(info.SlotInfo, typeRef);
        }
    }
}
