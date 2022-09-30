using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class StateMachineTypeSymbol : SynthesizedContainer, ISynthesizedMethodBodyImplementationSymbol
	{
		private ImmutableArray<VisualBasicAttributeData> _attributes;

		public readonly MethodSymbol KickoffMethod;

		public bool HasMethodBodyDependency => true;

		public IMethodSymbolInternal Method => KickoffMethod;

		internal sealed override bool IsInterface => false;

		public StateMachineTypeSymbol(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol kickoffMethod, int kickoffMethodOrdinal, NamedTypeSymbol baseType, ImmutableArray<NamedTypeSymbol> originalInterfaces)
			: base(kickoffMethod, MakeName(slotAllocatorOpt, compilationState, kickoffMethod, kickoffMethodOrdinal), baseType, originalInterfaces)
		{
			KickoffMethod = kickoffMethod;
		}

		private static string MakeName(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol kickoffMethod, int kickoffMethodOrdinal)
		{
			return slotAllocatorOpt?.PreviousStateMachineTypeName ?? GeneratedNames.MakeStateMachineTypeName(kickoffMethod.Name, kickoffMethodOrdinal, compilationState.ModuleBuilderOpt?.CurrentGenerationOrdinal ?? 0);
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_attributes.IsDefault)
			{
				ArrayBuilder<VisualBasicAttributeData> arrayBuilder = null;
				NamedTypeSymbol containingType = KickoffMethod.ContainingType;
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = containingType.GetAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (current.IsTargetAttribute(containingType, AttributeDescription.DebuggerNonUserCodeAttribute) || current.IsTargetAttribute(containingType, AttributeDescription.DebuggerStepThroughAttribute))
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<VisualBasicAttributeData>.GetInstance(2);
						}
						arrayBuilder.Add(current);
					}
				}
				ImmutableInterlocked.InterlockedCompareExchange(ref _attributes, arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<VisualBasicAttributeData>.Empty, default(ImmutableArray<VisualBasicAttributeData>));
			}
			return _attributes;
		}
	}
}
