using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IMethodBody
    {
        ImmutableArray<ExceptionHandlerRegion> ExceptionRegions { get; }

        bool AreLocalsZeroed { get; }

        bool HasStackalloc { get; }

        ImmutableArray<ILocalDefinition> LocalVariables { get; }

        IMethodDefinition MethodDefinition { get; }

        StateMachineMoveNextBodyDebugInfo MoveNextBodyInfo { get; }

        ushort MaxStack { get; }

        ImmutableArray<byte> IL { get; }

        ImmutableArray<SequencePoint> SequencePoints { get; }

        bool HasDynamicLocalVariables { get; }

        ImmutableArray<LocalScope> LocalScopes { get; }

        IImportScope ImportScope { get; }

        DebugId MethodId { get; }

        ImmutableArray<StateMachineHoistedLocalScope> StateMachineHoistedLocalScopes { get; }

        string StateMachineTypeName { get; }

        ImmutableArray<EncHoistedLocalInfo> StateMachineHoistedLocalSlots { get; }

        ImmutableArray<ITypeReference?> StateMachineAwaiterSlots { get; }

        ImmutableArray<ClosureDebugInfo> ClosureDebugInfo { get; }

        ImmutableArray<LambdaDebugInfo> LambdaDebugInfo { get; }

        DynamicAnalysisMethodBodyData DynamicAnalysisData { get; }
    }
}
