using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class NamespaceOrTypeSymbol : Symbol, INamespaceOrTypeSymbol, INamespaceOrTypeSymbolInternal
	{
		public bool IsNamespace => Kind == SymbolKind.Namespace;

		public bool IsType => !IsNamespace;

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		public abstract ImmutableArray<Symbol> GetMembers();

		internal virtual ImmutableArray<Symbol> GetMembersUnordered()
		{
			return GetMembers().ConditionallyDeOrder();
		}

		public abstract ImmutableArray<Symbol> GetMembers(string name);

		internal virtual ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return GetTypeMembers().ConditionallyDeOrder();
		}

		public abstract ImmutableArray<NamedTypeSymbol> GetTypeMembers();

		public abstract ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name);

		public virtual ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol type, int arity_) => type.Arity == arity_, arity);
		}

		internal NamespaceOrTypeSymbol()
		{
		}

		internal bool AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, IEnumerable<KeyValuePair<string, ImmutableArray<Symbol>>> membersByName)
		{
			bool result = false;
			foreach (KeyValuePair<string, ImmutableArray<Symbol>> item in membersByName)
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = item.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current = enumerator2.Current;
					if (current.Kind != SymbolKind.Method)
					{
						continue;
					}
					MethodSymbol methodSymbol = (MethodSymbol)current;
					if (methodSymbol.MayBeReducibleExtensionMethod)
					{
						result = true;
						if (AddExtensionMethodLookupSymbolsInfoViabilityCheck(methodSymbol, options, nameSet, originalBinder))
						{
							nameSet.AddSymbol(current, current.Name, SymbolExtensions.GetArity(current));
							break;
						}
					}
				}
			}
			return result;
		}

		internal virtual bool AddExtensionMethodLookupSymbolsInfoViabilityCheck(MethodSymbol method, LookupOptions options, LookupSymbolsInfo nameSet, Binder originalBinder)
		{
			return originalBinder.CanAddLookupSymbolInfo(method, options, nameSet, method.ContainingType);
		}

		internal IEnumerable<NamespaceOrTypeSymbol> GetNamespaceOrTypeByQualifiedName(IEnumerable<string> qualifiedName)
		{
			NamespaceOrTypeSymbol namespaceOrTypeSymbol = this;
			IEnumerable<NamespaceOrTypeSymbol> enumerable = null;
			foreach (string item in qualifiedName)
			{
				if (enumerable != null)
				{
					namespaceOrTypeSymbol = SymbolExtensions.OfMinimalArity(enumerable);
					if ((object)namespaceOrTypeSymbol == null)
					{
						return SpecializedCollections.EmptyEnumerable<NamespaceOrTypeSymbol>();
					}
				}
				enumerable = namespaceOrTypeSymbol.GetMembers(item).OfType<NamespaceOrTypeSymbol>();
			}
			return enumerable;
		}

		private ImmutableArray<ISymbol> INamespaceOrTypeSymbol_GetMembers()
		{
			return StaticCast<ISymbol>.From(GetMembers());
		}

		ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceOrTypeSymbol_GetMembers
			return this.INamespaceOrTypeSymbol_GetMembers();
		}

		private ImmutableArray<ISymbol> INamespaceOrTypeSymbol_GetMembers(string name)
		{
			return StaticCast<ISymbol>.From(GetMembers(name));
		}

		ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceOrTypeSymbol_GetMembers
			return this.INamespaceOrTypeSymbol_GetMembers(name);
		}

		private ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol_GetTypeMembers()
		{
			return StaticCast<INamedTypeSymbol>.From(GetTypeMembers());
		}

		ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceOrTypeSymbol_GetTypeMembers
			return this.INamespaceOrTypeSymbol_GetTypeMembers();
		}

		private ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol_GetTypeMembers(string name)
		{
			return StaticCast<INamedTypeSymbol>.From(GetTypeMembers(name));
		}

		ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceOrTypeSymbol_GetTypeMembers
			return this.INamespaceOrTypeSymbol_GetTypeMembers(name);
		}

		public ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol_GetTypeMembers(string name, int arity)
		{
			return StaticCast<INamedTypeSymbol>.From(GetTypeMembers(name, arity));
		}

		ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name, int arity)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceOrTypeSymbol_GetTypeMembers
			return this.INamespaceOrTypeSymbol_GetTypeMembers(name, arity);
		}
	}
}
