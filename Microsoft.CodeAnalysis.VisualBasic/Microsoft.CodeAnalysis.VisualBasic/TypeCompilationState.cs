using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class TypeCompilationState
	{
		public struct MethodWithBody
		{
			public readonly MethodSymbol Method;

			public readonly BoundStatement Body;

			internal MethodWithBody(MethodSymbol _method, BoundStatement _body)
			{
				this = default(MethodWithBody);
				Method = _method;
				Body = _body;
			}
		}

		public readonly VisualBasicCompilation Compilation;

		public LambdaFrame staticLambdaFrame;

		public readonly PEModuleBuilder ModuleBuilderOpt;

		private ArrayBuilder<MethodWithBody> _synthesizedMethods;

		public readonly MethodSymbol InitializeComponentOpt;

		public readonly Dictionary<MethodSymbol, NamedTypeSymbol> StateMachineImplementationClass;

		private Dictionary<MethodSymbol, MethodSymbol> _methodWrappers;

		private Dictionary<MethodSymbol, ImmutableArray<MethodSymbol>> _initializeComponentCallTree;

		public bool HasSynthesizedMethods => _synthesizedMethods != null;

		public ArrayBuilder<MethodWithBody> SynthesizedMethods => _synthesizedMethods;

		public TypeCompilationState(VisualBasicCompilation compilation, PEModuleBuilder moduleBuilderOpt, MethodSymbol initializeComponentOpt)
		{
			_synthesizedMethods = null;
			StateMachineImplementationClass = new Dictionary<MethodSymbol, NamedTypeSymbol>(ReferenceEqualityComparer.Instance);
			_methodWrappers = null;
			_initializeComponentCallTree = null;
			Compilation = compilation;
			ModuleBuilderOpt = moduleBuilderOpt;
			InitializeComponentOpt = initializeComponentOpt;
		}

		public void AddSynthesizedMethod(MethodSymbol method, BoundStatement body)
		{
			if (_synthesizedMethods == null)
			{
				_synthesizedMethods = ArrayBuilder<MethodWithBody>.GetInstance();
			}
			_synthesizedMethods.Add(new MethodWithBody(method, body));
		}

		public bool HasMethodWrapper(MethodSymbol method)
		{
			if (_methodWrappers != null)
			{
				return _methodWrappers.ContainsKey(method);
			}
			return false;
		}

		public void AddMethodWrapper(MethodSymbol method, MethodSymbol wrapper, BoundStatement body)
		{
			if (_methodWrappers == null)
			{
				_methodWrappers = new Dictionary<MethodSymbol, MethodSymbol>();
			}
			_methodWrappers[method] = wrapper;
			AddSynthesizedMethod(wrapper, body);
		}

		public MethodSymbol GetMethodWrapper(MethodSymbol method)
		{
			MethodSymbol value = null;
			if (_methodWrappers == null || !_methodWrappers.TryGetValue(method, out value))
			{
				return null;
			}
			return value;
		}

		public void Free()
		{
			if (_synthesizedMethods != null)
			{
				_synthesizedMethods.Free();
				_synthesizedMethods = null;
			}
			if (_methodWrappers != null)
			{
				_methodWrappers = null;
			}
		}

		public void AddToInitializeComponentCallTree(MethodSymbol method, ImmutableArray<MethodSymbol> callees)
		{
			if (_initializeComponentCallTree == null)
			{
				_initializeComponentCallTree = new Dictionary<MethodSymbol, ImmutableArray<MethodSymbol>>(ReferenceEqualityComparer.Instance);
			}
			_initializeComponentCallTree.Add(method, callees);
		}

		public bool CallsInitializeComponent(MethodSymbol method)
		{
			if (_initializeComponentCallTree == null)
			{
				return false;
			}
			return CallsInitializeComponent(method, new HashSet<MethodSymbol>(ReferenceEqualityComparer.Instance));
		}

		private bool CallsInitializeComponent(MethodSymbol method, HashSet<MethodSymbol> visited)
		{
			visited.Add(method);
			ImmutableArray<MethodSymbol> value = default(ImmutableArray<MethodSymbol>);
			if (_initializeComponentCallTree.TryGetValue(method, out value))
			{
				ImmutableArray<MethodSymbol>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol current = enumerator.Current;
					if ((object)current == InitializeComponentOpt)
					{
						return true;
					}
					if (!visited.Contains(current) && CallsInitializeComponent(current, visited))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
