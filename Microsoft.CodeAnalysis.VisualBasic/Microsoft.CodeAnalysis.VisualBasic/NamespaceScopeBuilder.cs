using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class NamespaceScopeBuilder
	{
		private NamespaceScopeBuilder()
		{
		}

		public static ImmutableArray<UsedNamespaceOrType> BuildNamespaceScope(PEModuleBuilder moduleBuilder, IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespacesOpt, IEnumerable<AliasAndImportsClausePosition> aliasImportsOpt, ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports, DiagnosticBag diagnostics)
		{
			ArrayBuilder<UsedNamespaceOrType> instance = ArrayBuilder<UsedNamespaceOrType>.GetInstance();
			if (xmlNamespacesOpt != null)
			{
				foreach (KeyValuePair<string, XmlNamespaceAndImportsClausePosition> item in xmlNamespacesOpt)
				{
					instance.Add(UsedNamespaceOrType.CreateXmlNamespace(item.Key, item.Value.XmlNamespace));
				}
			}
			if (aliasImportsOpt != null)
			{
				foreach (AliasAndImportsClausePosition item2 in aliasImportsOpt)
				{
					NamespaceOrTypeSymbol target = item2.Alias.Target;
					if (target.IsNamespace)
					{
						instance.Add(UsedNamespaceOrType.CreateNamespace(((NamespaceSymbol)target).GetCciAdapter(), null, item2.Alias.Name));
					}
					else if (target.Kind != SymbolKind.ErrorType && !target.ContainingAssembly.IsLinked)
					{
						ITypeReference typeReference = GetTypeReference((NamedTypeSymbol)target, moduleBuilder, diagnostics);
						instance.Add(UsedNamespaceOrType.CreateType(typeReference, item2.Alias.Name));
					}
				}
			}
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator3 = memberImports.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				NamespaceOrTypeSymbol namespaceOrType = enumerator3.Current.NamespaceOrType;
				if (namespaceOrType.IsNamespace)
				{
					instance.Add(UsedNamespaceOrType.CreateNamespace(((NamespaceSymbol)namespaceOrType).GetCciAdapter()));
				}
				else if (!namespaceOrType.ContainingAssembly.IsLinked)
				{
					ITypeReference typeReference2 = GetTypeReference((NamedTypeSymbol)namespaceOrType, moduleBuilder, diagnostics);
					instance.Add(UsedNamespaceOrType.CreateType(typeReference2));
				}
			}
			return instance.ToImmutableAndFree();
		}

		private static ITypeReference GetTypeReference(TypeSymbol type, CommonPEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
		{
			return moduleBuilder.Translate(type, null, diagnostics);
		}
	}
}
