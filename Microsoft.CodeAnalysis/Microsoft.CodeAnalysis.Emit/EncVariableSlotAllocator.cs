using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    internal sealed class EncVariableSlotAllocator : VariableSlotAllocator
    {
        private readonly SymbolMatcher _symbolMap;

        private readonly Func<SyntaxNode, SyntaxNode?>? _syntaxMap;

        private readonly IMethodSymbolInternal _previousTopLevelMethod;

        private readonly DebugId _methodId;

        private readonly IReadOnlyDictionary<EncLocalInfo, int> _previousLocalSlots;

        private readonly ImmutableArray<EncLocalInfo> _previousLocals;

        private readonly string? _stateMachineTypeName;

        private readonly int _hoistedLocalSlotCount;

        private readonly IReadOnlyDictionary<EncHoistedLocalInfo, int>? _hoistedLocalSlots;

        private readonly int _awaiterCount;

        private readonly IReadOnlyDictionary<ITypeReference, int>? _awaiterMap;

        private readonly IReadOnlyDictionary<int, KeyValuePair<DebugId, int>>? _lambdaMap;

        private readonly IReadOnlyDictionary<int, DebugId>? _closureMap;

        private readonly LambdaSyntaxFacts _lambdaSyntaxFacts;

        public override DebugId? MethodId => _methodId;

        public override string? PreviousStateMachineTypeName => _stateMachineTypeName;

        public override int PreviousHoistedLocalSlotCount => _hoistedLocalSlotCount;

        public override int PreviousAwaiterSlotCount => _awaiterCount;

        public EncVariableSlotAllocator(SymbolMatcher symbolMap, Func<SyntaxNode, SyntaxNode?>? syntaxMap, IMethodSymbolInternal previousTopLevelMethod, DebugId methodId, ImmutableArray<EncLocalInfo> previousLocals, IReadOnlyDictionary<int, KeyValuePair<DebugId, int>>? lambdaMap, IReadOnlyDictionary<int, DebugId>? closureMap, string? stateMachineTypeName, int hoistedLocalSlotCount, IReadOnlyDictionary<EncHoistedLocalInfo, int>? hoistedLocalSlots, int awaiterCount, IReadOnlyDictionary<ITypeReference, int>? awaiterMap, LambdaSyntaxFacts lambdaSyntaxFacts)
        {
            _symbolMap = symbolMap;
            _syntaxMap = syntaxMap;
            _previousLocals = previousLocals;
            _previousTopLevelMethod = previousTopLevelMethod;
            _methodId = methodId;
            _hoistedLocalSlots = hoistedLocalSlots;
            _hoistedLocalSlotCount = hoistedLocalSlotCount;
            _stateMachineTypeName = stateMachineTypeName;
            _awaiterCount = awaiterCount;
            _awaiterMap = awaiterMap;
            _lambdaMap = lambdaMap;
            _closureMap = closureMap;
            _lambdaSyntaxFacts = lambdaSyntaxFacts;
            Dictionary<EncLocalInfo, int> dictionary = new Dictionary<EncLocalInfo, int>();
            for (int i = 0; i < previousLocals.Length; i++)
            {
                EncLocalInfo key = previousLocals[i];
                if (!key.IsUnused)
                {
                    dictionary.Add(key, i);
                }
            }
            _previousLocalSlots = dictionary;
        }

        private int CalculateSyntaxOffsetInPreviousMethod(SyntaxNode node)
        {
            return _previousTopLevelMethod.CalculateLocalSyntaxOffset(_lambdaSyntaxFacts.GetDeclaratorPosition(node), node.SyntaxTree);
        }

        public override void AddPreviousLocals(ArrayBuilder<ILocalDefinition> builder)
        {
            builder.AddRange(_previousLocals.Select((EncLocalInfo info, int index) => new SignatureOnlyLocalDefinition(info.Signature, index)));
        }

        private bool TryGetPreviousLocalId(SyntaxNode currentDeclarator, LocalDebugId currentId, out LocalDebugId previousId)
        {
            if (_syntaxMap == null)
            {
                previousId = currentId;
                return true;
            }
            SyntaxNode syntaxNode = _syntaxMap!(currentDeclarator);
            if (syntaxNode == null)
            {
                previousId = default(LocalDebugId);
                return false;
            }
            int syntaxOffset = CalculateSyntaxOffsetInPreviousMethod(syntaxNode);
            previousId = new LocalDebugId(syntaxOffset, currentId.Ordinal);
            return true;
        }

        public override LocalDefinition? GetPreviousLocal(ITypeReference currentType, ILocalSymbolInternal currentLocalSymbol, string? name, SynthesizedLocalKind kind, LocalDebugId id, LocalVariableAttributes pdbAttributes, LocalSlotConstraints constraints, ImmutableArray<bool> dynamicTransformFlags, ImmutableArray<string> tupleElementNames)
        {
            if (id.IsNone)
            {
                return null;
            }
            if (!TryGetPreviousLocalId(currentLocalSymbol.GetDeclaratorSyntax(), id, out var previousId))
            {
                return null;
            }
            ITypeReference typeReference = _symbolMap.MapReference(currentType);
            if (typeReference == null)
            {
                return null;
            }
            EncLocalInfo key = new EncLocalInfo(new LocalSlotDebugInfo(kind, previousId), typeReference, constraints, null);
            if (!_previousLocalSlots.TryGetValue(key, out var value))
            {
                return null;
            }
            return new LocalDefinition(currentLocalSymbol, name, currentType, value, kind, id, pdbAttributes, constraints, dynamicTransformFlags, tupleElementNames);
        }

        public override bool TryGetPreviousHoistedLocalSlotIndex(SyntaxNode currentDeclarator, ITypeReference currentType, SynthesizedLocalKind synthesizedKind, LocalDebugId currentId, DiagnosticBag diagnostics, out int slotIndex)
        {
            if (_hoistedLocalSlots == null)
            {
                slotIndex = -1;
                return false;
            }
            if (!TryGetPreviousLocalId(currentDeclarator, currentId, out var previousId))
            {
                slotIndex = -1;
                return false;
            }
            ITypeReference typeReference = _symbolMap.MapReference(currentType);
            if (typeReference == null)
            {
                slotIndex = -1;
                return false;
            }
            EncHoistedLocalInfo key = new EncHoistedLocalInfo(new LocalSlotDebugInfo(synthesizedKind, previousId), typeReference);
            return _hoistedLocalSlots!.TryGetValue(key, out slotIndex);
        }

        public override bool TryGetPreviousAwaiterSlotIndex(ITypeReference currentType, DiagnosticBag diagnostics, out int slotIndex)
        {
            if (_awaiterMap == null)
            {
                slotIndex = -1;
                return false;
            }
            ITypeReference key = _symbolMap.MapReference(currentType);
            return _awaiterMap!.TryGetValue(key, out slotIndex);
        }

        private bool TryGetPreviousSyntaxOffset(SyntaxNode currentSyntax, out int previousSyntaxOffset)
        {
            SyntaxNode syntaxNode = _syntaxMap?.Invoke(currentSyntax);
            if (syntaxNode == null)
            {
                previousSyntaxOffset = 0;
                return false;
            }
            previousSyntaxOffset = CalculateSyntaxOffsetInPreviousMethod(syntaxNode);
            return true;
        }

        private bool TryGetPreviousLambdaSyntaxOffset(SyntaxNode lambdaOrLambdaBodySyntax, bool isLambdaBody, out int previousSyntaxOffset)
        {
            SyntaxNode arg = (isLambdaBody ? _lambdaSyntaxFacts.GetLambda(lambdaOrLambdaBodySyntax) : lambdaOrLambdaBodySyntax);
            SyntaxNode syntaxNode = _syntaxMap?.Invoke(arg);
            if (syntaxNode == null)
            {
                previousSyntaxOffset = 0;
                return false;
            }
            SyntaxNode syntaxNode2;
            if (isLambdaBody)
            {
                syntaxNode2 = _lambdaSyntaxFacts.TryGetCorrespondingLambdaBody(syntaxNode, lambdaOrLambdaBodySyntax);
                if (syntaxNode2 == null)
                {
                    previousSyntaxOffset = 0;
                    return false;
                }
            }
            else
            {
                syntaxNode2 = syntaxNode;
            }
            previousSyntaxOffset = CalculateSyntaxOffsetInPreviousMethod(syntaxNode2);
            return true;
        }

        public override bool TryGetPreviousClosure(SyntaxNode scopeSyntax, out DebugId closureId)
        {
            if (_closureMap != null && TryGetPreviousSyntaxOffset(scopeSyntax, out var previousSyntaxOffset) && _closureMap!.TryGetValue(previousSyntaxOffset, out closureId))
            {
                return true;
            }
            closureId = default(DebugId);
            return false;
        }

        public override bool TryGetPreviousLambda(SyntaxNode lambdaOrLambdaBodySyntax, bool isLambdaBody, out DebugId lambdaId)
        {
            if (_lambdaMap != null && TryGetPreviousLambdaSyntaxOffset(lambdaOrLambdaBodySyntax, isLambdaBody, out var previousSyntaxOffset) && _lambdaMap!.TryGetValue(previousSyntaxOffset, out var value))
            {
                lambdaId = value.Key;
                return true;
            }
            lambdaId = default(DebugId);
            return false;
        }
    }
}
