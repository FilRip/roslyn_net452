using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class BaseTypeAnalysis
	{
		private struct DependencyDesc
		{
			public readonly DependencyKind kind;

			public readonly TypeSymbol dependent;

			internal DependencyDesc(DependencyKind kind, TypeSymbol dependent)
			{
				this = default(DependencyDesc);
				this.kind = kind;
				this.dependent = dependent;
			}
		}

		private enum DependencyKind
		{
			Inheritance,
			Containment
		}

		internal static DiagnosticInfo GetDependenceDiagnosticForBase(SourceNamedTypeSymbol @this, BasesBeingResolved basesBeingResolved)
		{
			bool flag = false;
			ConsList<TypeSymbol> consList = basesBeingResolved.InheritsBeingResolvedOpt;
			NamedTypeSymbol namedTypeSymbol = @this;
			ConsList<DependencyDesc> consList2 = ConsList<DependencyDesc>.Empty.Prepend(new DependencyDesc(DependencyKind.Inheritance, @this));
			int num = 1;
			while (consList.Any())
			{
				NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)consList.Head;
				if ((object)namedTypeSymbol2 == @this)
				{
					return ErrorFactory.ErrorInfo(ERRID.ERR_IllegalBaseTypeReferences3, SymbolExtensions.GetKindText(@this), @this, GetBaseTypeReferenceDetails(consList2));
				}
				if (!@this.DetectTypeCircularity_ShouldStepIntoType(namedTypeSymbol2))
				{
					return null;
				}
				if ((object)namedTypeSymbol != null && (object)namedTypeSymbol2.ContainingSymbol == namedTypeSymbol)
				{
					flag = true;
				}
				consList2 = consList2.Prepend(new DependencyDesc(flag ? DependencyKind.Containment : DependencyKind.Inheritance, namedTypeSymbol2));
				num++;
				namedTypeSymbol = namedTypeSymbol2;
				consList = consList.Tail;
			}
			return null;
		}

		internal static DiagnosticInfo GetDependenceDiagnosticForBase(SourceNamedTypeSymbol @this, TypeSymbol @base)
		{
			ConsList<DependencyDesc> dependenceChain = GetDependenceChain(new HashSet<Symbol>(), (SourceNamedTypeSymbol)@this.OriginalDefinition, @base);
			if (dependenceChain == null)
			{
				return null;
			}
			dependenceChain = dependenceChain.Prepend(new DependencyDesc(DependencyKind.Inheritance, @this));
			int num = 0;
			bool flag = false;
			foreach (DependencyDesc item in dependenceChain)
			{
				if (item.kind == DependencyKind.Containment)
				{
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				if (@this.TypeKind == TypeKind.Class)
				{
					return ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycle1, @this, GetInheritanceDetails(dependenceChain));
				}
				return ErrorFactory.ErrorInfo(ERRID.ERR_InterfaceCycle1, @this, GetInheritanceDetails(dependenceChain));
			}
			if (num > 2)
			{
				return ErrorFactory.ErrorInfo(ERRID.ERR_CircularBaseDependencies4, SymbolExtensions.GetKindText(@this), @this, GetInheritanceDetails(dependenceChain));
			}
			return ErrorFactory.ErrorInfo(ERRID.ERR_NestedBase2, SymbolExtensions.GetKindText(@this), @this);
		}

		private static DiagnosticInfo GetInheritanceDetails(ConsList<DependencyDesc> chain)
		{
			return GetInheritanceOrDependenceDetails(chain, ERRID.ERR_InheritsFrom2);
		}

		private static DiagnosticInfo GetBaseTypeReferenceDetails(ConsList<DependencyDesc> chain)
		{
			return GetInheritanceOrDependenceDetails(chain, ERRID.ERR_BaseTypeReferences2);
		}

		private static DiagnosticInfo GetInheritanceOrDependenceDetails(ConsList<DependencyDesc> chain, ERRID inheritsOrDepends)
		{
			ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
			DependencyDesc dependencyDesc = chain.Head;
			ERRID id;
			foreach (DependencyDesc item in chain.Tail)
			{
				DependencyKind kind = dependencyDesc.kind;
				id = ((kind != DependencyKind.Containment) ? inheritsOrDepends : ERRID.ERR_IsNestedIn2);
				instance.Add(ErrorFactory.ErrorInfo(id, dependencyDesc.dependent, item.dependent));
				dependencyDesc = item;
			}
			DependencyKind kind2 = dependencyDesc.kind;
			id = ((kind2 != DependencyKind.Containment) ? inheritsOrDepends : ERRID.ERR_IsNestedIn2);
			instance.Add(ErrorFactory.ErrorInfo(id, dependencyDesc.dependent, chain.Head.dependent));
			return new CompoundDiagnosticInfo(instance.ToArrayAndFree());
		}

		private static ConsList<DependencyDesc> GetDependenceChain(HashSet<Symbol> visited, SourceNamedTypeSymbol root, TypeSymbol current)
		{
			if ((object)current == null || current.Kind == SymbolKind.ErrorType)
			{
				return null;
			}
			TypeSymbol originalDefinition = current.OriginalDefinition;
			if ((object)root == originalDefinition)
			{
				return ConsList<DependencyDesc>.Empty;
			}
			if (!visited.Add(current))
			{
				return null;
			}
			if (!(originalDefinition is NamedTypeSymbol namedTypeSymbol))
			{
				return null;
			}
			ConsList<DependencyDesc> consList = null;
			consList = GetDependenceChain(visited, root, namedTypeSymbol.ContainingType);
			if (consList != null)
			{
				return consList.Prepend(new DependencyDesc(DependencyKind.Containment, current));
			}
			if (!root.DetectTypeCircularity_ShouldStepIntoType(namedTypeSymbol))
			{
				return null;
			}
			if (namedTypeSymbol.TypeKind == TypeKind.Class)
			{
				NamedTypeSymbol bestKnownBaseType = namedTypeSymbol.GetBestKnownBaseType();
				consList = GetDependenceChain(visited, root, bestKnownBaseType);
				if (consList != null)
				{
					return consList.Prepend(new DependencyDesc(DependencyKind.Inheritance, current));
				}
			}
			if (namedTypeSymbol.IsInterface)
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedTypeSymbol.GetBestKnownInterfacesNoUseSiteDiagnostics().GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current2 = enumerator.Current;
					consList = GetDependenceChain(visited, root, current2);
					if (consList != null)
					{
						return consList.Prepend(new DependencyDesc(DependencyKind.Inheritance, current));
					}
				}
			}
			return null;
		}

		internal static DiagnosticInfo GetDependencyDiagnosticsForImportedClass(NamedTypeSymbol @this)
		{
			NamedTypeSymbol originalDefinition = @this.OriginalDefinition;
			if ((object)originalDefinition == null)
			{
				return null;
			}
			NamedTypeSymbol declaredBase = @this.GetDeclaredBase(default(BasesBeingResolved));
			while ((object)declaredBase != null)
			{
				declaredBase = declaredBase.OriginalDefinition;
				if ((object)originalDefinition == declaredBase)
				{
					return ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, @this);
				}
				declaredBase = declaredBase.GetDeclaredBase(default(BasesBeingResolved));
				if ((object)declaredBase == null)
				{
					break;
				}
				declaredBase = declaredBase.OriginalDefinition;
				if ((object)originalDefinition == declaredBase)
				{
					return ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, @this);
				}
				declaredBase = declaredBase.GetDeclaredBase(default(BasesBeingResolved));
				originalDefinition = originalDefinition.GetDeclaredBase(default(BasesBeingResolved)).OriginalDefinition;
			}
			return null;
		}

		internal static DiagnosticInfo GetDependencyDiagnosticsForImportedBaseInterface(NamedTypeSymbol @this, NamedTypeSymbol @base)
		{
			@base = @base.OriginalDefinition;
			if ((object)@base == null)
			{
				return null;
			}
			HashSet<TypeSymbol> derived = new HashSet<TypeSymbol> { @base };
			HashSet<TypeSymbol> verified = new HashSet<TypeSymbol>();
			if (HasCycles(derived, verified, @base))
			{
				return ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, @this);
			}
			return null;
		}

		private static bool HasCycles(HashSet<TypeSymbol> derived, HashSet<TypeSymbol> verified, NamedTypeSymbol @interface)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = @interface.GetDeclaredInterfacesNoUseSiteDiagnostics(default(BasesBeingResolved));
			if (!declaredInterfacesNoUseSiteDiagnostics.IsEmpty)
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					current = current.OriginalDefinition;
					if ((object)current != null && !verified.Contains(current))
					{
						if (!derived.Add(current))
						{
							return true;
						}
						if (HasCycles(derived, verified, current))
						{
							return true;
						}
					}
				}
			}
			verified.Add(@interface);
			derived.Remove(@interface);
			return false;
		}
	}
}
