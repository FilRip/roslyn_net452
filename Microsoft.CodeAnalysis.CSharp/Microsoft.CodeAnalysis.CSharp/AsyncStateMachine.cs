using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class AsyncStateMachine : StateMachineTypeSymbol
    {
        private readonly TypeKind _typeKind;

        private readonly MethodSymbol _constructor;

        private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

        internal readonly TypeSymbol IteratorElementType;

        public override TypeKind TypeKind => _typeKind;

        internal override MethodSymbol Constructor => _constructor;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        public AsyncStateMachine(VariableSlotAllocator variableAllocatorOpt, TypeCompilationState compilationState, MethodSymbol asyncMethod, int asyncMethodOrdinal, TypeKind typeKind)
            : base(variableAllocatorOpt, compilationState, asyncMethod, asyncMethodOrdinal)
        {
            _typeKind = typeKind;
            CSharpCompilation declaringCompilation = asyncMethod.DeclaringCompilation;
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            bool isIterator = asyncMethod.IsIterator;
            if (isIterator)
            {
                TypeSymbol typeSymbol = (IteratorElementType = base.TypeMap.SubstituteType(asyncMethod.IteratorElementTypeWithAnnotations).Type);
                if (asyncMethod.IsAsyncReturningIAsyncEnumerable(declaringCompilation))
                {
                    instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T).Construct(typeSymbol));
                }
                instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T).Construct(typeSymbol));
                instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T).Construct(declaringCompilation.GetSpecialType(SpecialType.System_Boolean)));
                instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource));
                instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable));
            }
            instance.Add(declaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine));
            _interfaces = instance.ToImmutableAndFree();
            _constructor = (isIterator ? new IteratorConstructor(this) : ((MethodSymbol)new AsyncConstructor(this)));
        }

        internal override bool HasPossibleWellKnownCloneMethod()
        {
            return false;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _interfaces;
        }
    }
}
