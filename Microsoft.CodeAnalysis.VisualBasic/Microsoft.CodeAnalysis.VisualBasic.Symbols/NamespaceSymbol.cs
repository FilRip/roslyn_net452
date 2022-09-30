using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class NamespaceSymbol : NamespaceOrTypeSymbol, INamespace, INamespaceSymbol, INamespaceSymbolInternal
	{
		private string INamedEntity_Name => AdaptedNamespaceSymbol.MetadataName;

		private INamespace INamespaceSymbol_ContainingNamespace => AdaptedNamespaceSymbol.ContainingNamespace?.GetCciAdapter();

		internal NamespaceSymbol AdaptedNamespaceSymbol => this;

		public virtual bool IsGlobalNamespace => (object)base.ContainingNamespace == null;

		internal abstract NamespaceExtent Extent { get; }

		public NamespaceKind NamespaceKind => Extent.Kind;

		public VisualBasicCompilation ContainingCompilation
		{
			get
			{
				if (NamespaceKind != NamespaceKind.Compilation)
				{
					return null;
				}
				return Extent.Compilation;
			}
		}

		public virtual ImmutableArray<NamespaceSymbol> ConstituentNamespaces => ImmutableArray.Create(this);

		public abstract override AssemblySymbol ContainingAssembly { get; }

		public sealed override NamedTypeSymbol ContainingType => null;

		public override ModuleSymbol ContainingModule
		{
			get
			{
				NamespaceExtent extent = Extent;
				if (extent.Kind == NamespaceKind.Module)
				{
					return extent.Module;
				}
				return null;
			}
		}

		public sealed override SymbolKind Kind => SymbolKind.Namespace;

		public sealed override bool IsImplicitlyDeclared => IsGlobalNamespace;

		public sealed override Accessibility DeclaredAccessibility => Accessibility.Public;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal abstract Accessibility DeclaredAccessibilityOfMostAccessibleDescendantType { get; }

		public sealed override bool IsShared => true;

		internal abstract ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods { get; }

		private ImmutableArray<INamespaceSymbol> INamespaceSymbol_ConstituentNamespaces => StaticCast<INamespaceSymbol>.From(ConstituentNamespaces);

		private NamespaceKind INamespaceSymbol_NamespaceKind => NamespaceKind;

		private Compilation INamespaceSymbol_ContainingCompilation => ContainingCompilation;

		private INamespaceSymbolInternal INamespaceSymbol_GetInternalSymbol()
		{
			return AdaptedNamespaceSymbol;
		}

		INamespaceSymbolInternal INamespace.GetInternalSymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceSymbol_GetInternalSymbol
			return this.INamespaceSymbol_GetInternalSymbol();
		}

		internal new NamespaceSymbol GetCciAdapter()
		{
			return this;
		}

		public virtual IEnumerable<NamespaceSymbol> GetNamespaceMembers()
		{
			return GetMembers().OfType<NamespaceSymbol>();
		}

		public abstract ImmutableArray<NamedTypeSymbol> GetModuleMembers();

		public virtual ImmutableArray<NamedTypeSymbol> GetModuleMembers(string name)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol t) => t.TypeKind == TypeKind.Module);
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitNamespace(this, arg);
		}

		internal NamespaceSymbol()
		{
		}

		protected virtual Accessibility GetDeclaredAccessibilityOfMostAccessibleDescendantType()
		{
			Accessibility accessibility = Accessibility.NotApplicable;
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = GetTypeMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Accessibility declaredAccessibility = enumerator.Current.DeclaredAccessibility;
				if (declaredAccessibility == Accessibility.Public)
				{
					return Accessibility.Public;
				}
				accessibility = Accessibility.Internal;
			}
			ImmutableArray<Symbol>.Enumerator enumerator2 = GetMembersUnordered().GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Symbol current = enumerator2.Current;
				if (current.Kind != SymbolKind.Namespace)
				{
					continue;
				}
				Accessibility declaredAccessibilityOfMostAccessibleDescendantType = ((NamespaceSymbol)current).DeclaredAccessibilityOfMostAccessibleDescendantType;
				if (declaredAccessibilityOfMostAccessibleDescendantType > accessibility)
				{
					if (declaredAccessibilityOfMostAccessibleDescendantType == Accessibility.Public)
					{
						return Accessibility.Public;
					}
					accessibility = declaredAccessibilityOfMostAccessibleDescendantType;
				}
			}
			return accessibility;
		}

		internal virtual bool ContainsTypesAccessibleFrom(AssemblySymbol fromAssembly)
		{
			switch (DeclaredAccessibilityOfMostAccessibleDescendantType)
			{
			case Accessibility.Public:
				return true;
			case Accessibility.Internal:
			{
				AssemblySymbol containingAssembly = ContainingAssembly;
				return (object)containingAssembly != null && AccessCheck.HasFriendAccessTo(fromAssembly, containingAssembly);
			}
			default:
				return false;
			}
		}

		internal NamespaceSymbol LookupNestedNamespace(ImmutableArray<string> names)
		{
			NamespaceSymbol namespaceSymbol = this;
			ImmutableArray<string>.Enumerator enumerator = names.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				NamespaceSymbol namespaceSymbol2 = null;
				ImmutableArray<Symbol>.Enumerator enumerator2 = namespaceSymbol.GetMembers(current).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if ((NamespaceOrTypeSymbol)enumerator2.Current is NamespaceSymbol namespaceSymbol3)
					{
						if ((object)namespaceSymbol2 != null)
						{
							namespaceSymbol2 = null;
							break;
						}
						namespaceSymbol2 = namespaceSymbol3;
					}
				}
				namespaceSymbol = namespaceSymbol2;
				if ((object)namespaceSymbol == null)
				{
					break;
				}
			}
			return namespaceSymbol;
		}

		internal NamespaceSymbol LookupNestedNamespace(string[] names)
		{
			return LookupNestedNamespace(names.AsImmutableOrNull());
		}

		internal virtual NamedTypeSymbol LookupMetadataType(ref MetadataTypeName fullEmittedName)
		{
			NamedTypeSymbol namedTypeSymbol = null;
			string text = ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
			if (fullEmittedName.IsMangled && (fullEmittedName.ForcedArity == -1 || fullEmittedName.ForcedArity == fullEmittedName.InferredArity))
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = GetTypeMembers(fullEmittedName.UnmangledTypeName).GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					if (fullEmittedName.InferredArity == current.Arity && current.MangleName && string.Equals(current.Name, fullEmittedName.UnmangledTypeName, StringComparison.Ordinal) && string.Equals(fullEmittedName.NamespaceName, current.GetEmittedNamespaceName() ?? text, StringComparison.Ordinal))
					{
						if ((object)namedTypeSymbol != null)
						{
							namedTypeSymbol = null;
							break;
						}
						namedTypeSymbol = current;
					}
				}
			}
			int num = fullEmittedName.ForcedArity;
			if (fullEmittedName.UseCLSCompliantNameArityEncoding)
			{
				if (fullEmittedName.InferredArity > 0)
				{
					goto IL_0151;
				}
				if (num == -1)
				{
					num = 0;
				}
				else if (num != 0)
				{
					goto IL_0151;
				}
			}
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = GetTypeMembers(fullEmittedName.TypeName).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				NamedTypeSymbol current2 = enumerator2.Current;
				if (!current2.MangleName && (num == -1 || num == current2.Arity) && string.Equals(current2.Name, fullEmittedName.TypeName, StringComparison.Ordinal) && string.Equals(fullEmittedName.NamespaceName, current2.GetEmittedNamespaceName() ?? text, StringComparison.Ordinal))
				{
					if ((object)namedTypeSymbol != null)
					{
						namedTypeSymbol = null;
						break;
					}
					namedTypeSymbol = current2;
				}
			}
			goto IL_0151;
			IL_0151:
			if ((object)namedTypeSymbol == null)
			{
				if (fullEmittedName.FullName.StartsWith("Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxList", StringComparison.Ordinal))
				{
					Debugger.Break();
				}
				return new MissingMetadataTypeSymbol.TopLevel(ContainingModule, ref fullEmittedName);
			}
			return namedTypeSymbol;
		}

		internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (IsGlobalNamespace)
			{
				return true;
			}
			return base.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken);
		}

		internal NamespaceSymbol GetNestedNamespace(string name)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Namespace)
				{
					return (NamespaceSymbol)current;
				}
			}
			return null;
		}

		internal NamespaceSymbol GetNestedNamespace(NameSyntax name)
		{
			switch (name.Kind())
			{
			case SyntaxKind.IdentifierName:
				return GetNestedNamespace(((IdentifierNameSyntax)name).Identifier.ValueText);
			case SyntaxKind.QualifiedName:
			{
				QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)name;
				NamespaceSymbol nestedNamespace = GetNestedNamespace(qualifiedNameSyntax.Left);
				if ((object)nestedNamespace != null)
				{
					return nestedNamespace.GetNestedNamespace(qualifiedNameSyntax.Right);
				}
				break;
			}
			}
			return null;
		}

		internal virtual bool IsDeclaredInSourceModule(ModuleSymbol module)
		{
			return (object)ContainingModule == module;
		}

		internal abstract void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods);

		internal virtual void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, this);
		}

		internal abstract void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamespaceSymbol appendThrough);

		internal virtual void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = TypesToCheckForExtensionMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.BuildExtensionMethodsMap(map, this);
			}
		}

		internal virtual void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = TypesToCheckForExtensionMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.GetExtensionMethods(methods, this, name);
			}
		}

		internal bool BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map, IEnumerable<KeyValuePair<string, ImmutableArray<Symbol>>> membersByName)
		{
			bool result = false;
			foreach (KeyValuePair<string, ImmutableArray<Symbol>> item in membersByName)
			{
				ArrayBuilder<MethodSymbol> value = null;
				ImmutableArray<Symbol>.Enumerator enumerator2 = item.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					if (current2.Kind != SymbolKind.Method)
					{
						continue;
					}
					MethodSymbol methodSymbol = (MethodSymbol)current2;
					if (methodSymbol.MayBeReducibleExtensionMethod)
					{
						if (value == null && !map.TryGetValue(methodSymbol.Name, out value))
						{
							value = ArrayBuilder<MethodSymbol>.GetInstance();
							map.Add(item.Key, value);
						}
						BuildExtensionMethodsMapBucket(value, methodSymbol);
						result = true;
					}
				}
			}
			return result;
		}

		internal void AddMemberIfExtension(ArrayBuilder<MethodSymbol> bucket, Symbol member)
		{
			if (member.Kind == SymbolKind.Method)
			{
				MethodSymbol methodSymbol = (MethodSymbol)member;
				if (methodSymbol.MayBeReducibleExtensionMethod)
				{
					BuildExtensionMethodsMapBucket(bucket, methodSymbol);
				}
			}
		}

		internal virtual void BuildExtensionMethodsMapBucket(ArrayBuilder<MethodSymbol> bucket, MethodSymbol method)
		{
			bucket.Add(method);
		}

		private IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol_GetMembers()
		{
			return GetMembers().OfType<INamespaceOrTypeSymbol>();
		}

		IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol.GetMembers()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceSymbol_GetMembers
			return this.INamespaceSymbol_GetMembers();
		}

		private IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol_GetMembers(string name)
		{
			return GetMembers(name).OfType<INamespaceOrTypeSymbol>();
		}

		IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol.GetMembers(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceSymbol_GetMembers
			return this.INamespaceSymbol_GetMembers(name);
		}

		private IEnumerable<INamespaceSymbol> INamespaceSymbol_GetNamespaceMembers()
		{
			return GetNamespaceMembers();
		}

		IEnumerable<INamespaceSymbol> INamespaceSymbol.GetNamespaceMembers()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceSymbol_GetNamespaceMembers
			return this.INamespaceSymbol_GetNamespaceMembers();
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitNamespace(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitNamespace(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitNamespace(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitNamespace(this);
		}
	}
}
