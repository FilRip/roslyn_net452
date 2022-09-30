using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class IteratorStateMachine : StateMachineTypeSymbol
	{
		private readonly SynthesizedSimpleConstructorSymbol _constructor;

		private readonly MethodSymbol _iteratorMethod;

		public override TypeKind TypeKind => TypeKind.Class;

		protected internal override MethodSymbol Constructor => _constructor;

		protected internal IteratorStateMachine(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol iteratorMethod, int iteratorMethodOrdinal, TypeSymbol valueTypeSymbol, bool isEnumerable)
			: base(slotAllocatorOpt, compilationState, iteratorMethod, iteratorMethodOrdinal, iteratorMethod.ContainingAssembly.GetSpecialType(SpecialType.System_Object), GetIteratorInterfaces(valueTypeSymbol, isEnumerable, iteratorMethod.ContainingAssembly))
		{
			NamedTypeSymbol specialType = DeclaringCompilation.GetSpecialType(SpecialType.System_Int32);
			_constructor = new SynthesizedSimpleConstructorSymbol(this);
			ImmutableArray<ParameterSymbol> parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(_constructor, specialType, 0, isByRef: false, GeneratedNames.MakeStateMachineStateFieldName()));
			_constructor.SetParameters(parameters);
			_iteratorMethod = iteratorMethod;
		}

		private static ImmutableArray<NamedTypeSymbol> GetIteratorInterfaces(TypeSymbol elementType, bool isEnumerable, AssemblySymbol containingAssembly)
		{
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			if (isEnumerable)
			{
				instance.Add(containingAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(elementType));
				instance.Add(containingAssembly.GetSpecialType(SpecialType.System_Collections_IEnumerable));
			}
			instance.Add(containingAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T).Construct(elementType));
			instance.Add(containingAssembly.GetSpecialType(SpecialType.System_IDisposable));
			instance.Add(containingAssembly.GetSpecialType(SpecialType.System_Collections_IEnumerator));
			return instance.ToImmutableAndFree();
		}
	}
}
