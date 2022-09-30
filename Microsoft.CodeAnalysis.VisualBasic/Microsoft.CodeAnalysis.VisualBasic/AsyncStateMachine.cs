using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AsyncStateMachine : StateMachineTypeSymbol
	{
		private readonly TypeKind _typeKind;

		private readonly SynthesizedSimpleConstructorSymbol _constructor;

		public override TypeKind TypeKind => _typeKind;

		protected internal override MethodSymbol Constructor => _constructor;

		protected internal AsyncStateMachine(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol asyncMethod, int asyncMethodOrdinal, TypeKind typeKind)
			: base(slotAllocatorOpt, compilationState, asyncMethod, asyncMethodOrdinal, asyncMethod.ContainingAssembly.GetSpecialType((typeKind != TypeKind.Struct) ? SpecialType.System_Object : SpecialType.System_ValueType), ImmutableArray.Create(asyncMethod.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine)))
		{
			_constructor = new SynthesizedSimpleConstructorSymbol(this);
			_constructor.SetParameters(ImmutableArray<ParameterSymbol>.Empty);
			_typeKind = typeKind;
		}
	}
}
