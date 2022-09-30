using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DocumentationCommentCrefBinder : DocumentationCommentBinder
	{
		private struct SignatureElement
		{
			public readonly TypeSymbol Type;

			public readonly bool IsByRef;

			public SignatureElement(TypeSymbol type, bool isByRef)
			{
				this = default(SignatureElement);
				Type = type;
				IsByRef = isByRef;
			}
		}

		private sealed class TypeParametersBinder : Binder
		{
			internal readonly Dictionary<string, CrefTypeParameterSymbol> _typeParameters;

			public TypeParametersBinder(Binder containingBinder, Dictionary<string, CrefTypeParameterSymbol> typeParameters)
				: base(containingBinder)
			{
				_typeParameters = typeParameters;
			}

			internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
			{
				CrefTypeParameterSymbol value = null;
				if (_typeParameters.TryGetValue(name, out value))
				{
					lookupResult.SetFrom(CheckViability(value, arity, options | LookupOptions.IgnoreAccessibility, null, ref useSiteInfo));
				}
			}

			internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
			{
				foreach (CrefTypeParameterSymbol value in _typeParameters.Values)
				{
					if (originalBinder.CanAddLookupSymbolInfo(value, options, nameSet, null))
					{
						nameSet.AddSymbol(value, value.Name, 0);
					}
				}
			}
		}

		private TypeParametersBinder _typeParameterBinder;

		public DocumentationCommentCrefBinder(Binder containingBinder, Symbol commentedSymbol)
			: base(containingBinder, commentedSymbol)
		{
		}

		private Binder GetOrCreateTypeParametersAwareBinder(Dictionary<string, CrefTypeParameterSymbol> typeParameters)
		{
			if (_typeParameterBinder == null)
			{
				Interlocked.CompareExchange(ref _typeParameterBinder, new TypeParametersBinder(this, typeParameters), null);
			}
			return _typeParameterBinder;
		}

		private static bool HasTrailingSkippedTokensAndShouldReportError(CrefReferenceSyntax reference)
		{
			SyntaxTriviaList.Enumerator enumerator = reference.GetTrailingTrivia().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (VisualBasicExtensions.Kind(enumerator.Current) != SyntaxKind.SkippedTokensTrivia)
				{
					continue;
				}
				TypeSyntax name = reference.Name;
				if (name.Kind() == SyntaxKind.IdentifierName)
				{
					SyntaxToken identifier = ((IdentifierNameSyntax)name).Identifier;
					if (!VisualBasicExtensions.IsBracketed(identifier) && DocumentationCommentBinder.IsIntrinsicTypeForDocumentationComment(SyntaxFacts.GetKeywordKind(identifier.ValueText)))
					{
						continue;
					}
				}
				else if (name.Kind() == SyntaxKind.PredefinedType)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(CrefReferenceSyntax reference, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (HasTrailingSkippedTokensAndShouldReportError(reference))
			{
				return ImmutableArray<Symbol>.Empty;
			}
			if (reference.Signature == null)
			{
				return BindNameInsideCrefReferenceInLegacyMode(reference.Name, preserveAliases, ref useSiteInfo);
			}
			if (NameSyntaxHasComplexGenericArguments(reference.Name))
			{
				return ImmutableArray<Symbol>.Empty;
			}
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			Dictionary<string, CrefTypeParameterSymbol> typeParameters = new Dictionary<string, CrefTypeParameterSymbol>(CaseInsensitiveComparison.Comparer);
			CollectCrefNameSymbolsStrict(reference.Name, reference.Signature.ArgumentTypes.Count, typeParameters, instance, preserveAliases, ref useSiteInfo);
			if (instance.Count == 0)
			{
				instance.Free();
				return ImmutableArray<Symbol>.Empty;
			}
			DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance);
			ArrayBuilder<SignatureElement> signatureTypes = null;
			TypeSymbol returnType = null;
			BindSignatureAndReturnValue(reference, typeParameters, out signatureTypes, out returnType, diagnosticBag);
			int num = signatureTypes?.Count ?? 0;
			int num2 = 0;
			int num3 = 0;
			while (num2 < instance.Count)
			{
				Symbol symbol = instance[num2];
				switch (symbol.Kind)
				{
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol = (MethodSymbol)symbol;
					if (methodSymbol.ParameterCount != num)
					{
						break;
					}
					ImmutableArray<ParameterSymbol> parameters2 = methodSymbol.Parameters;
					int num5 = num - 1;
					for (int j = 0; j <= num5; j++)
					{
						ParameterSymbol parameterSymbol2 = parameters2[j];
						if (parameterSymbol2.IsByRef != signatureTypes[j].IsByRef || !TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol2.Type, signatureTypes[j].Type))
						{
							goto end_IL_00df;
						}
					}
					if ((object)returnType == null || (!methodSymbol.IsSub && TypeSymbolExtensions.IsSameTypeIgnoringAll(methodSymbol.ReturnType, returnType)))
					{
						instance[num3] = symbol;
						num3++;
						num2++;
						continue;
					}
					break;
				}
				case SymbolKind.Property:
					{
						ImmutableArray<ParameterSymbol> parameters = ((PropertySymbol)symbol).Parameters;
						if (parameters.Length != num)
						{
							break;
						}
						int num4 = num - 1;
						for (int i = 0; i <= num4; i++)
						{
							ParameterSymbol parameterSymbol = parameters[i];
							if (parameterSymbol.IsByRef != signatureTypes[i].IsByRef || !TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol.Type, signatureTypes[i].Type))
							{
								goto end_IL_00df;
							}
						}
						instance[num3] = symbol;
						num3++;
						num2++;
						continue;
					}
					end_IL_00df:
					break;
				}
				num2++;
			}
			signatureTypes?.Free();
			if (num3 < num2)
			{
				instance.Clip(num3);
			}
			return instance.ToImmutableAndFree();
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(TypeSyntax name, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool partOfSignatureOrReturnType = false;
			CrefReferenceSyntax enclosingCrefReference = GetEnclosingCrefReference(name, out partOfSignatureOrReturnType);
			if (enclosingCrefReference == null)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			if (enclosingCrefReference.Signature == null)
			{
				return BindNameInsideCrefReferenceInLegacyMode(name, preserveAliases, ref useSiteInfo);
			}
			if (partOfSignatureOrReturnType)
			{
				return BindInsideCrefSignatureOrReturnType(enclosingCrefReference, name, preserveAliases, diagnosticBag);
			}
			return BindInsideCrefReferenceName(name, enclosingCrefReference.Signature.ArgumentTypes.Count, preserveAliases, ref useSiteInfo);
		}

		private ImmutableArray<Symbol> BindInsideCrefSignatureOrReturnType(CrefReferenceSyntax crefReference, TypeSyntax name, bool preserveAliases, BindingDiagnosticBag diagnosticBag)
		{
			Binder orCreateTypeParametersAwareBinder = GetOrCreateTypeParametersAwareBinder(crefReference);
			Symbol symbol = orCreateTypeParametersAwareBinder.BindNamespaceOrTypeOrAliasSyntax(name, diagnosticBag ?? BindingDiagnosticBag.Discarded);
			symbol = orCreateTypeParametersAwareBinder.BindNamespaceOrTypeOrAliasSyntax(name, diagnosticBag ?? BindingDiagnosticBag.Discarded);
			if ((object)symbol != null && symbol.Kind == SymbolKind.Alias && !preserveAliases)
			{
				symbol = ((AliasSymbol)symbol).Target;
			}
			if ((object)symbol != null)
			{
				return ImmutableArray.Create(symbol);
			}
			return ImmutableArray<Symbol>.Empty;
		}

		private Binder GetOrCreateTypeParametersAwareBinder(CrefReferenceSyntax crefReference)
		{
			if (_typeParameterBinder != null)
			{
				return _typeParameterBinder;
			}
			Dictionary<string, CrefTypeParameterSymbol> dictionary = new Dictionary<string, CrefTypeParameterSymbol>(CaseInsensitiveComparison.Comparer);
			TypeSyntax typeSyntax = crefReference.Name;
			GenericNameSyntax genericNameSyntax = null;
			while (typeSyntax != null)
			{
				SeparatedSyntaxList<TypeSyntax> arguments;
				int num;
				switch (typeSyntax.Kind())
				{
				case SyntaxKind.GenericName:
					genericNameSyntax = (GenericNameSyntax)typeSyntax;
					typeSyntax = null;
					goto IL_00d4;
				case SyntaxKind.QualifiedName:
				{
					QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)typeSyntax;
					typeSyntax = qualifiedNameSyntax.Left;
					if (qualifiedNameSyntax.Right.Kind() == SyntaxKind.GenericName)
					{
						genericNameSyntax = (GenericNameSyntax)qualifiedNameSyntax.Right;
					}
					goto IL_00d4;
				}
				case SyntaxKind.QualifiedCrefOperatorReference:
					typeSyntax = ((QualifiedCrefOperatorReferenceSyntax)typeSyntax).Left;
					goto IL_00d4;
				default:
					throw ExceptionUtilities.UnexpectedValue(typeSyntax.Kind());
				case SyntaxKind.PredefinedType:
				case SyntaxKind.IdentifierName:
				case SyntaxKind.GlobalName:
				case SyntaxKind.CrefOperatorReference:
					break;
					IL_00d4:
					if (genericNameSyntax == null)
					{
						continue;
					}
					arguments = genericNameSyntax.TypeArgumentList.Arguments;
					num = arguments.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						TypeSyntax typeSyntax2 = arguments[i];
						SyntaxKind syntaxKind = typeSyntax2.Kind();
						if (syntaxKind == SyntaxKind.IdentifierName)
						{
							IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)typeSyntax2;
							string valueText = identifierNameSyntax.Identifier.ValueText;
							if (!dictionary.ContainsKey(valueText))
							{
								dictionary[valueText] = new CrefTypeParameterSymbol(i, valueText, identifierNameSyntax);
							}
						}
					}
					continue;
				}
				break;
			}
			return GetOrCreateTypeParametersAwareBinder(dictionary);
		}

		private ImmutableArray<Symbol> BindInsideCrefReferenceName(TypeSyntax name, int argCount, bool preserveAliases, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			VisualBasicSyntaxNode parent = name.Parent;
			if (parent != null && parent.Kind() == SyntaxKind.TypeArgumentList)
			{
				int ordinal = ((TypeArgumentListSyntax)parent).Arguments.IndexOf(name);
				if (name.Kind() == SyntaxKind.IdentifierName)
				{
					IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)name;
					return ImmutableArray.Create((Symbol)new CrefTypeParameterSymbol(ordinal, identifierNameSyntax.Identifier.ValueText, identifierNameSyntax));
				}
				return ImmutableArray.Create((Symbol)new CrefTypeParameterSymbol(ordinal, "?", name));
			}
			bool flag = false;
			string right = null;
			int num = -1;
			while (true)
			{
				switch (name.Kind())
				{
				case SyntaxKind.IdentifierName:
				case SyntaxKind.GenericName:
					if (parent != null && parent.Kind() == SyntaxKind.QualifiedName)
					{
						QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)parent;
						if (qualifiedNameSyntax.Right == name)
						{
							name = qualifiedNameSyntax;
							parent = name.Parent;
							continue;
						}
					}
					flag = true;
					if (name.Kind() == SyntaxKind.IdentifierName)
					{
						right = ((IdentifierNameSyntax)name).Identifier.ValueText;
						num = 0;
					}
					else
					{
						GenericNameSyntax obj = (GenericNameSyntax)name;
						right = obj.Identifier.ValueText;
						num = obj.TypeArgumentList.Arguments.Count;
					}
					break;
				case SyntaxKind.CrefOperatorReference:
					if (parent != null && parent.Kind() == SyntaxKind.QualifiedCrefOperatorReference)
					{
						name = (QualifiedCrefOperatorReferenceSyntax)parent;
						parent = name.Parent;
						continue;
					}
					break;
				case SyntaxKind.GlobalName:
					return ImmutableArray.Create((Symbol)base.Compilation.GlobalNamespace);
				default:
					throw ExceptionUtilities.UnexpectedValue(name.Kind());
				case SyntaxKind.QualifiedName:
				case SyntaxKind.QualifiedCrefOperatorReference:
					break;
				}
				break;
			}
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			CollectCrefNameSymbolsStrict(name, argCount, new Dictionary<string, CrefTypeParameterSymbol>(CaseInsensitiveComparison.Comparer), instance, preserveAliases, ref useSiteInfo);
			DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance);
			if (instance.Count == 1 && flag)
			{
				Symbol symbol = instance[0];
				TypeSymbol typeSymbol = null;
				switch (symbol.Kind)
				{
				case SymbolKind.Field:
					typeSymbol = ((FieldSymbol)symbol).Type;
					break;
				case SymbolKind.Method:
					typeSymbol = ((MethodSymbol)symbol).ReturnType;
					break;
				case SymbolKind.Property:
					typeSymbol = ((PropertySymbol)symbol).Type;
					break;
				}
				bool flag2 = false;
				if ((object)typeSymbol != null && CaseInsensitiveComparison.Equals(typeSymbol.Name, right))
				{
					flag2 = ((!(typeSymbol is NamedTypeSymbol namedTypeSymbol)) ? (num == 0) : (namedTypeSymbol.Arity == num));
				}
				if (flag2)
				{
					instance[0] = typeSymbol;
				}
			}
			return instance.ToImmutableAndFree();
		}

		private static CrefReferenceSyntax GetEnclosingCrefReference(TypeSyntax nameFromCref, out bool partOfSignatureOrReturnType)
		{
			partOfSignatureOrReturnType = false;
			VisualBasicSyntaxNode visualBasicSyntaxNode;
			for (visualBasicSyntaxNode = nameFromCref; visualBasicSyntaxNode != null; visualBasicSyntaxNode = visualBasicSyntaxNode.Parent)
			{
				switch (visualBasicSyntaxNode.Kind())
				{
				case SyntaxKind.SimpleAsClause:
					partOfSignatureOrReturnType = true;
					continue;
				case SyntaxKind.CrefSignature:
					partOfSignatureOrReturnType = true;
					continue;
				default:
					continue;
				case SyntaxKind.CrefReference:
					break;
				}
				break;
			}
			return (CrefReferenceSyntax)visualBasicSyntaxNode;
		}

		private void BindSignatureAndReturnValue(CrefReferenceSyntax reference, Dictionary<string, CrefTypeParameterSymbol> typeParameters, out ArrayBuilder<SignatureElement> signatureTypes, out TypeSymbol returnType, BindingDiagnosticBag diagnosticBag)
		{
			signatureTypes = null;
			returnType = null;
			Binder orCreateTypeParametersAwareBinder = GetOrCreateTypeParametersAwareBinder(typeParameters);
			BindingDiagnosticBag diagBag = diagnosticBag ?? BindingDiagnosticBag.Discarded;
			CrefSignatureSyntax signature = reference.Signature;
			if (signature.ArgumentTypes.Count > 0)
			{
				signatureTypes = ArrayBuilder<SignatureElement>.GetInstance();
				SeparatedSyntaxList<CrefSignaturePartSyntax>.Enumerator enumerator = signature.ArgumentTypes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CrefSignaturePartSyntax current = enumerator.Current;
					signatureTypes.Add(new SignatureElement(orCreateTypeParametersAwareBinder.BindTypeSyntax(current.Type, diagBag), VisualBasicExtensions.Kind(current.Modifier) == SyntaxKind.ByRefKeyword));
				}
			}
			if (reference.AsClause != null)
			{
				returnType = orCreateTypeParametersAwareBinder.BindTypeSyntax(reference.AsClause.Type, diagBag);
			}
		}

		private void CollectCrefNameSymbolsStrict(TypeSyntax nameFromCref, int argsCount, Dictionary<string, CrefTypeParameterSymbol> typeParameters, ArrayBuilder<Symbol> symbols, bool preserveAlias, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			switch (nameFromCref.Kind())
			{
			case SyntaxKind.QualifiedCrefOperatorReference:
				CollectQualifiedOperatorReferenceSymbolsStrict((QualifiedCrefOperatorReferenceSyntax)nameFromCref, argsCount, typeParameters, symbols, ref useSiteInfo);
				break;
			case SyntaxKind.CrefOperatorReference:
				CollectTopLevelOperatorReferenceStrict((CrefOperatorReferenceSyntax)nameFromCref, argsCount, symbols, ref useSiteInfo);
				break;
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GenericName:
				CollectSimpleNameSymbolsStrict((SimpleNameSyntax)nameFromCref, typeParameters, symbols, preserveAlias, ref useSiteInfo, typeOrNamespaceOnly: false);
				break;
			case SyntaxKind.QualifiedName:
				CollectQualifiedNameSymbolsStrict((QualifiedNameSyntax)nameFromCref, typeParameters, symbols, preserveAlias, ref useSiteInfo);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(nameFromCref.Kind());
			}
		}

		private void CollectTopLevelOperatorReferenceStrict(CrefOperatorReferenceSyntax reference, int argCount, ArrayBuilder<Symbol> symbols, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			CollectOperatorsAndConversionsInType(reference, argCount, ContainingType, symbols, ref useSiteInfo);
		}

		private void CollectSimpleNameSymbolsStrict(SimpleNameSyntax node, Dictionary<string, CrefTypeParameterSymbol> typeParameters, ArrayBuilder<Symbol> symbols, bool preserveAlias, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool typeOrNamespaceOnly)
		{
			if (node.ContainsDiagnostics)
			{
				return;
			}
			if (node.Kind() == SyntaxKind.GenericName)
			{
				GenericNameSyntax genericNameSyntax = (GenericNameSyntax)node;
				CollectSimpleNameSymbolsStrict(genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList.Arguments.Count, symbols, preserveAlias, ref useSiteInfo, typeOrNamespaceOnly);
				CreateTypeParameterSymbolsAndConstructSymbols(genericNameSyntax, symbols, typeParameters);
				return;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)node;
			SyntaxToken identifier = identifierNameSyntax.Identifier;
			if (CaseInsensitiveComparison.Equals(identifierNameSyntax.Identifier.ValueText, SyntaxFacts.GetText(SyntaxKind.NewKeyword)) && !VisualBasicExtensions.IsBracketed(identifier))
			{
				CollectConstructorsSymbolsStrict(symbols);
			}
			else
			{
				CollectSimpleNameSymbolsStrict(identifierNameSyntax.Identifier.ValueText, 0, symbols, preserveAlias, ref useSiteInfo, typeOrNamespaceOnly);
			}
		}

		private void CollectQualifiedNameSymbolsStrict(QualifiedNameSyntax node, Dictionary<string, CrefTypeParameterSymbol> typeParameters, ArrayBuilder<Symbol> symbols, bool preserveAlias, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (node.ContainsDiagnostics)
			{
				return;
			}
			bool allowColorColor = true;
			NameSyntax left = node.Left;
			switch (left.Kind())
			{
			case SyntaxKind.IdentifierName:
				CollectSimpleNameSymbolsStrict((SimpleNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo, typeOrNamespaceOnly: true);
				break;
			case SyntaxKind.GenericName:
				CollectSimpleNameSymbolsStrict((SimpleNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo, typeOrNamespaceOnly: true);
				break;
			case SyntaxKind.QualifiedName:
				CollectQualifiedNameSymbolsStrict((QualifiedNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo);
				allowColorColor = false;
				break;
			case SyntaxKind.GlobalName:
				symbols.Add(base.Compilation.GlobalNamespace);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(left.Kind());
			}
			if (symbols.Count != 1)
			{
				typeParameters.Clear();
				symbols.Clear();
				return;
			}
			Symbol containingSymbol = symbols[0];
			symbols.Clear();
			SimpleNameSyntax right = node.Right;
			if (right.Kind() == SyntaxKind.GenericName)
			{
				GenericNameSyntax genericNameSyntax = (GenericNameSyntax)right;
				CollectSimpleNameSymbolsStrict(containingSymbol, allowColorColor, genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList.Arguments.Count, symbols, preserveAlias, ref useSiteInfo);
				CreateTypeParameterSymbolsAndConstructSymbols(genericNameSyntax, symbols, typeParameters);
				return;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)right;
			SyntaxToken identifier = identifierNameSyntax.Identifier;
			if (CaseInsensitiveComparison.Equals(identifierNameSyntax.Identifier.ValueText, SyntaxFacts.GetText(SyntaxKind.NewKeyword)) && !VisualBasicExtensions.IsBracketed(identifier))
			{
				CollectConstructorsSymbolsStrict(containingSymbol, symbols);
			}
			else
			{
				CollectSimpleNameSymbolsStrict(containingSymbol, allowColorColor, identifierNameSyntax.Identifier.ValueText, 0, symbols, preserveAlias, ref useSiteInfo);
			}
		}

		private void CollectQualifiedOperatorReferenceSymbolsStrict(QualifiedCrefOperatorReferenceSyntax node, int argCount, Dictionary<string, CrefTypeParameterSymbol> typeParameters, ArrayBuilder<Symbol> symbols, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (node.ContainsDiagnostics)
			{
				return;
			}
			NameSyntax left = node.Left;
			switch (left.Kind())
			{
			case SyntaxKind.IdentifierName:
				CollectSimpleNameSymbolsStrict((SimpleNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo, typeOrNamespaceOnly: true);
				break;
			case SyntaxKind.GenericName:
				CollectSimpleNameSymbolsStrict((SimpleNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo, typeOrNamespaceOnly: true);
				break;
			case SyntaxKind.QualifiedName:
				CollectQualifiedNameSymbolsStrict((QualifiedNameSyntax)left, typeParameters, symbols, preserveAlias: false, ref useSiteInfo);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(left.Kind());
			}
			if (symbols.Count != 1)
			{
				typeParameters.Clear();
				symbols.Clear();
				return;
			}
			Symbol symbol = symbols[0];
			symbols.Clear();
			if (symbol.Kind == SymbolKind.Alias)
			{
				symbol = ((AliasSymbol)symbol).Target;
			}
			CollectOperatorsAndConversionsInType(node.Right, argCount, symbol as TypeSymbol, symbols, ref useSiteInfo);
		}

		private void CollectConstructorsSymbolsStrict(ArrayBuilder<Symbol> symbols)
		{
			Symbol symbol = ContainingMember;
			if ((object)symbol != null)
			{
				if (symbol.Kind != SymbolKind.NamedType)
				{
					symbol = symbol.ContainingType;
				}
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
				if ((object)namedTypeSymbol != null)
				{
					symbols.AddRange(namedTypeSymbol.InstanceConstructors);
				}
			}
		}

		private static void CollectConstructorsSymbolsStrict(Symbol containingSymbol, ArrayBuilder<Symbol> symbols)
		{
			if (containingSymbol.Kind == SymbolKind.NamedType)
			{
				symbols.AddRange(((NamedTypeSymbol)containingSymbol).InstanceConstructors);
			}
		}

		private void CollectSimpleNameSymbolsStrict(string name, int arity, ArrayBuilder<Symbol> symbols, bool preserveAlias, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool typeOrNamespaceOnly)
		{
			LookupResult instance = LookupResult.GetInstance();
			Lookup(instance, name, arity, typeOrNamespaceOnly ? (LookupOptions.NamespacesOrTypesOnly | LookupOptions.MustNotBeReturnValueVariable | LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility | LookupOptions.MustNotBeLocalOrParameter | LookupOptions.NoSystemObjectLookupForInterfaces) : (LookupOptions.MustNotBeReturnValueVariable | LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility | LookupOptions.MustNotBeLocalOrParameter | LookupOptions.NoSystemObjectLookupForInterfaces), ref useSiteInfo);
			if (!instance.IsGoodOrAmbiguous || !instance.HasSymbol)
			{
				instance.Free();
				return;
			}
			CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias);
			instance.Free();
		}

		private void CollectSimpleNameSymbolsStrict(Symbol containingSymbol, bool allowColorColor, string name, int arity, ArrayBuilder<Symbol> symbols, bool preserveAlias, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			LookupResult instance = LookupResult.GetInstance();
			LookupOptions options = LookupOptions.MustNotBeReturnValueVariable | LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility | LookupOptions.MustNotBeLocalOrParameter | LookupOptions.NoSystemObjectLookupForInterfaces;
			while (true)
			{
				switch (containingSymbol.Kind)
				{
				case SymbolKind.Namespace:
					LookupMember(instance, (NamespaceSymbol)containingSymbol, name, arity, options, ref useSiteInfo);
					break;
				case SymbolKind.Alias:
					containingSymbol = ((AliasSymbol)containingSymbol).Target;
					continue;
				case SymbolKind.ArrayType:
				case SymbolKind.NamedType:
					LookupMember(instance, (TypeSymbol)containingSymbol, name, arity, options, ref useSiteInfo);
					break;
				case SymbolKind.Property:
					if (allowColorColor)
					{
						PropertySymbol obj = (PropertySymbol)containingSymbol;
						TypeSymbol type = obj.Type;
						if (CaseInsensitiveComparison.Equals(obj.Name, type.Name))
						{
							containingSymbol = type;
							continue;
						}
					}
					break;
				case SymbolKind.Field:
					if (allowColorColor)
					{
						FieldSymbol obj2 = (FieldSymbol)containingSymbol;
						TypeSymbol type2 = obj2.Type;
						if (CaseInsensitiveComparison.Equals(obj2.Name, type2.Name))
						{
							containingSymbol = type2;
							continue;
						}
					}
					break;
				case SymbolKind.Method:
				{
					if (!allowColorColor)
					{
						break;
					}
					MethodSymbol methodSymbol = (MethodSymbol)containingSymbol;
					if (!methodSymbol.IsSub)
					{
						TypeSymbol returnType = methodSymbol.ReturnType;
						if (CaseInsensitiveComparison.Equals(methodSymbol.Name, returnType.Name))
						{
							containingSymbol = returnType;
							continue;
						}
					}
					break;
				}
				}
				break;
			}
			if (!instance.IsGoodOrAmbiguous || !instance.HasSymbol)
			{
				instance.Free();
				return;
			}
			CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias);
			instance.Free();
		}

		private static void CreateTypeParameterSymbolsAndConstructSymbols(GenericNameSyntax genericName, ArrayBuilder<Symbol> symbols, Dictionary<string, CrefTypeParameterSymbol> typeParameters)
		{
			SeparatedSyntaxList<TypeSyntax> arguments = genericName.TypeArgumentList.Arguments;
			TypeSymbol[] array = new TypeSymbol[arguments.Count - 1 + 1];
			int num = arguments.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSyntax typeSyntax = arguments[i];
				CrefTypeParameterSymbol crefTypeParameterSymbol = null;
				SyntaxKind syntaxKind = typeSyntax.Kind();
				if (syntaxKind == SyntaxKind.IdentifierName)
				{
					IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)typeSyntax;
					crefTypeParameterSymbol = (CrefTypeParameterSymbol)(array[i] = new CrefTypeParameterSymbol(i, identifierNameSyntax.Identifier.ValueText, identifierNameSyntax));
				}
				else
				{
					crefTypeParameterSymbol = (CrefTypeParameterSymbol)(array[i] = new CrefTypeParameterSymbol(i, "?", typeSyntax));
				}
				typeParameters[crefTypeParameterSymbol.Name] = crefTypeParameterSymbol;
			}
			int num2 = symbols.Count - 1;
			int num3 = 0;
			while (num3 <= num2)
			{
				Symbol symbol = symbols[num3];
				while (true)
				{
					switch (symbol.Kind)
					{
					case SymbolKind.Method:
					{
						MethodSymbol methodSymbol = (MethodSymbol)symbol;
						symbols[num3] = methodSymbol.Construct(array.AsImmutableOrNull().As<TypeSymbol>());
						goto default;
					}
					case SymbolKind.ErrorType:
					case SymbolKind.NamedType:
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
						symbols[num3] = namedTypeSymbol.Construct(array.AsImmutableOrNull().As<TypeSymbol>());
						goto default;
					}
					case SymbolKind.Alias:
						symbol = ((AliasSymbol)symbol).Target;
						break;
					default:
						num3++;
						goto end_IL_00bb;
					}
					continue;
					end_IL_00bb:
					break;
				}
			}
		}

		private static void CollectGoodOrAmbiguousFromLookupResult(LookupResult lookupResult, ArrayBuilder<Symbol> symbols, bool preserveAlias)
		{
			DiagnosticInfo diagnostic = lookupResult.Diagnostic;
			if (diagnostic is AmbiguousSymbolDiagnostic)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = ((AmbiguousSymbolDiagnostic)diagnostic).AmbiguousSymbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					symbols.Add(preserveAlias ? current : SymbolExtensions.UnwrapAlias(current));
				}
			}
			else
			{
				ArrayBuilder<Symbol>.Enumerator enumerator2 = lookupResult.Symbols.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					symbols.Add(preserveAlias ? current2 : SymbolExtensions.UnwrapAlias(current2));
				}
			}
		}

		private static void CollectOperatorsAndConversionsInType(CrefOperatorReferenceSyntax crefOperator, int argCount, TypeSymbol type, ArrayBuilder<Symbol> symbols, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((object)type == null || argCount > 2 || argCount < 1)
			{
				return;
			}
			switch (VisualBasicExtensions.Kind(crefOperator.OperatorToken))
			{
			case SyntaxKind.IsTrueKeyword:
				if (argCount == 1)
				{
					OverloadResolution.OperatorInfo info5 = new OverloadResolution.OperatorInfo(UnaryOperatorKind.IsTrue);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_True", info5, ref useSiteInfo);
				}
				break;
			case SyntaxKind.IsFalseKeyword:
				if (argCount == 1)
				{
					OverloadResolution.OperatorInfo info12 = new OverloadResolution.OperatorInfo(UnaryOperatorKind.IsFalse);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_False", info12, ref useSiteInfo);
				}
				break;
			case SyntaxKind.NotKeyword:
				if (argCount == 1)
				{
					OverloadResolution.OperatorInfo operatorInfo5 = new OverloadResolution.OperatorInfo(UnaryOperatorKind.Not);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_OnesComplement", operatorInfo5, ref useSiteInfo, "op_LogicalNot", operatorInfo5);
				}
				break;
			case SyntaxKind.PlusToken:
				if (argCount == 1)
				{
					OverloadResolution.OperatorInfo info19 = new OverloadResolution.OperatorInfo(UnaryOperatorKind.Plus);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_UnaryPlus", info19, ref useSiteInfo);
				}
				else
				{
					OverloadResolution.OperatorInfo info20 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Add);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Addition", info20, ref useSiteInfo);
				}
				break;
			case SyntaxKind.MinusToken:
				if (argCount == 1)
				{
					OverloadResolution.OperatorInfo info7 = new OverloadResolution.OperatorInfo(UnaryOperatorKind.Minus);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_UnaryNegation", info7, ref useSiteInfo);
				}
				else
				{
					OverloadResolution.OperatorInfo info8 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Subtract);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Subtraction", info8, ref useSiteInfo);
				}
				break;
			case SyntaxKind.AsteriskToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info2 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Multiply);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Multiply", info2, ref useSiteInfo);
				}
				break;
			case SyntaxKind.SlashToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info17 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Divide);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Division", info17, ref useSiteInfo);
				}
				break;
			case SyntaxKind.BackslashToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info14 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.IntegerDivide);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_IntegerDivision", info14, ref useSiteInfo);
				}
				break;
			case SyntaxKind.ModKeyword:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info10 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Modulo);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Modulus", info10, ref useSiteInfo);
				}
				break;
			case SyntaxKind.CaretToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info6 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Power);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Exponent", info6, ref useSiteInfo);
				}
				break;
			case SyntaxKind.EqualsToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info3 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Equals);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Equality", info3, ref useSiteInfo);
				}
				break;
			case SyntaxKind.LessThanGreaterThanToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info = new OverloadResolution.OperatorInfo(BinaryOperatorKind.NotEquals);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Inequality", info, ref useSiteInfo);
				}
				break;
			case SyntaxKind.LessThanToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info18 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThan);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LessThan", info18, ref useSiteInfo);
				}
				break;
			case SyntaxKind.GreaterThanToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info16 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThan);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_GreaterThan", info16, ref useSiteInfo);
				}
				break;
			case SyntaxKind.LessThanEqualsToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info15 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThanOrEqual);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LessThanOrEqual", info15, ref useSiteInfo);
				}
				break;
			case SyntaxKind.GreaterThanEqualsToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info13 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThanOrEqual);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_GreaterThanOrEqual", info13, ref useSiteInfo);
				}
				break;
			case SyntaxKind.LikeKeyword:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info11 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Like);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Like", info11, ref useSiteInfo);
				}
				break;
			case SyntaxKind.AmpersandToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info9 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Concatenate);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Concatenate", info9, ref useSiteInfo);
				}
				break;
			case SyntaxKind.AndKeyword:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo operatorInfo4 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.And);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_BitwiseAnd", operatorInfo4, ref useSiteInfo, "op_LogicalAnd", operatorInfo4);
				}
				break;
			case SyntaxKind.OrKeyword:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo operatorInfo3 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Or);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_BitwiseOr", operatorInfo3, ref useSiteInfo, "op_LogicalOr", operatorInfo3);
				}
				break;
			case SyntaxKind.XorKeyword:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo info4 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.Xor);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_ExclusiveOr", info4, ref useSiteInfo);
				}
				break;
			case SyntaxKind.LessThanLessThanToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo operatorInfo2 = new OverloadResolution.OperatorInfo(BinaryOperatorKind.LeftShift);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LeftShift", operatorInfo2, ref useSiteInfo, "op_UnsignedLeftShift", operatorInfo2);
				}
				break;
			case SyntaxKind.GreaterThanGreaterThanToken:
				if (argCount == 2)
				{
					OverloadResolution.OperatorInfo operatorInfo = new OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_RightShift", operatorInfo, ref useSiteInfo, "op_UnsignedRightShift", operatorInfo);
				}
				break;
			case SyntaxKind.CTypeKeyword:
				if (argCount == 1)
				{
					new OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift);
					CollectOperatorsAndConversionsInType(type, symbols, MethodKind.Conversion, "op_Implicit", new OverloadResolution.OperatorInfo(UnaryOperatorKind.Implicit), ref useSiteInfo, "op_Explicit", new OverloadResolution.OperatorInfo(UnaryOperatorKind.Explicit));
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(crefOperator.OperatorToken));
			}
		}

		private static void CollectOperatorsAndConversionsInType(TypeSymbol type, ArrayBuilder<Symbol> symbols, MethodKind kind, string name1, OverloadResolution.OperatorInfo info1, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, string name2 = null, OverloadResolution.OperatorInfo info2 = default(OverloadResolution.OperatorInfo))
		{
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			OverloadResolution.CollectUserDefinedOperators(type, null, kind, name1, info1, name2, info2, instance, ref useSiteInfo);
			symbols.AddRange(instance);
			instance.Free();
		}

		private static bool NameSyntaxHasComplexGenericArguments(TypeSyntax name)
		{
			switch (name.Kind())
			{
			case SyntaxKind.PredefinedType:
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GlobalName:
			case SyntaxKind.CrefOperatorReference:
				return false;
			case SyntaxKind.QualifiedCrefOperatorReference:
				return NameSyntaxHasComplexGenericArguments(((QualifiedCrefOperatorReferenceSyntax)name).Left);
			case SyntaxKind.QualifiedName:
			{
				QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)name;
				return NameSyntaxHasComplexGenericArguments(qualifiedNameSyntax.Left) || NameSyntaxHasComplexGenericArguments(qualifiedNameSyntax.Right);
			}
			case SyntaxKind.GenericName:
			{
				SeparatedSyntaxList<TypeSyntax> arguments = ((GenericNameSyntax)name).TypeArgumentList.Arguments;
				int num = arguments.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					if (arguments[i].Kind() != SyntaxKind.IdentifierName)
					{
						return true;
					}
				}
				return false;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(name.Kind());
			}
		}

		private static bool CrefReferenceIsLegalForLegacyMode(TypeSyntax nameFromCref)
		{
			SyntaxKind syntaxKind = nameFromCref.Kind();
			if (syntaxKind - 398 <= (SyntaxKind)3)
			{
				return true;
			}
			return false;
		}

		private ImmutableArray<Symbol> BindNameInsideCrefReferenceInLegacyMode(TypeSyntax nameFromCref, bool preserveAliases, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!CrefReferenceIsLegalForLegacyMode(nameFromCref))
			{
				return ImmutableArray<Symbol>.Empty;
			}
			VisualBasicSyntaxNode parent = nameFromCref.Parent;
			while ((parent != null) & (parent.Kind() != SyntaxKind.CrefReference))
			{
				if (parent.Kind() != SyntaxKind.QualifiedName)
				{
					return ImmutableArray.Create(preserveAliases ? BindTypeOrAliasSyntax(nameFromCref, BindingDiagnosticBag.Discarded) : BindTypeSyntax(nameFromCref, BindingDiagnosticBag.Discarded));
				}
				parent = parent.Parent;
			}
			if (nameFromCref.ContainsDiagnostics)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			switch (nameFromCref.Kind())
			{
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GenericName:
				BindSimpleNameForCref((SimpleNameSyntax)nameFromCref, instance, preserveAliases, ref useSiteInfo, typeOrNamespaceOnly: false);
				break;
			case SyntaxKind.PredefinedType:
				BindPredefinedTypeForCref((PredefinedTypeSyntax)nameFromCref, instance);
				break;
			case SyntaxKind.QualifiedName:
				BindQualifiedNameForCref((QualifiedNameSyntax)nameFromCref, instance, preserveAliases, ref useSiteInfo);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(nameFromCref.Kind());
			}
			DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance);
			return instance.ToImmutableAndFree();
		}

		private void BindQualifiedNameForCref(QualifiedNameSyntax node, ArrayBuilder<Symbol> symbols, bool preserveAliases, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool allowColorColor = true;
			NameSyntax left = node.Left;
			switch (left.Kind())
			{
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GenericName:
				BindSimpleNameForCref((SimpleNameSyntax)left, symbols, preserveAliases, ref useSiteInfo, typeOrNamespaceOnly: true);
				break;
			case SyntaxKind.QualifiedName:
				BindQualifiedNameForCref((QualifiedNameSyntax)left, symbols, preserveAliases, ref useSiteInfo);
				allowColorColor = false;
				break;
			case SyntaxKind.GlobalName:
				symbols.Add(base.Compilation.GlobalNamespace);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(left.Kind());
			}
			if (symbols.Count != 1)
			{
				symbols.Clear();
				return;
			}
			Symbol containingSymbol = symbols[0];
			symbols.Clear();
			SimpleNameSyntax right = node.Right;
			if (right.Kind() == SyntaxKind.GenericName)
			{
				GenericNameSyntax genericNameSyntax = (GenericNameSyntax)right;
				BindSimpleNameForCref(genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList.Arguments.Count, symbols, preserveAliases, ref useSiteInfo, containingSymbol, allowColorColor);
				if (symbols.Count == 1)
				{
					symbols[0] = ConstructGenericSymbolWithTypeArgumentsForCref(symbols[0], genericNameSyntax);
				}
			}
			else
			{
				string valueText = ((IdentifierNameSyntax)right).Identifier.ValueText;
				BindSimpleNameForCref(valueText, 0, symbols, preserveAliases, ref useSiteInfo, containingSymbol, allowColorColor);
				if (symbols.Count <= 0)
				{
					BindSimpleNameForCref(valueText, -1, symbols, preserveAliases, ref useSiteInfo, containingSymbol, allowColorColor);
				}
			}
		}

		private void LookupSimpleNameInContainingSymbol(Symbol containingSymbol, bool allowColorColor, string name, int arity, bool preserveAliases, LookupResult lookupResult, LookupOptions options, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			while (true)
			{
				switch (containingSymbol.Kind)
				{
				default:
					return;
				case SymbolKind.Namespace:
					LookupMember(lookupResult, (NamespaceSymbol)containingSymbol, name, arity, options, ref useSiteInfo);
					return;
				case SymbolKind.Alias:
					if (!preserveAliases)
					{
						containingSymbol = ((AliasSymbol)containingSymbol).Target;
						break;
					}
					return;
				case SymbolKind.ArrayType:
				case SymbolKind.NamedType:
					LookupMember(lookupResult, (TypeSymbol)containingSymbol, name, arity, options, ref useSiteInfo);
					return;
				case SymbolKind.Property:
					if (allowColorColor)
					{
						PropertySymbol obj = (PropertySymbol)containingSymbol;
						TypeSymbol type = obj.Type;
						if (CaseInsensitiveComparison.Equals(obj.Name, type.Name))
						{
							containingSymbol = type;
							break;
						}
						return;
					}
					return;
				case SymbolKind.Field:
					if (allowColorColor)
					{
						FieldSymbol obj2 = (FieldSymbol)containingSymbol;
						TypeSymbol type2 = obj2.Type;
						if (CaseInsensitiveComparison.Equals(obj2.Name, type2.Name))
						{
							containingSymbol = type2;
							break;
						}
						return;
					}
					return;
				case SymbolKind.Method:
					if (allowColorColor)
					{
						MethodSymbol methodSymbol = (MethodSymbol)containingSymbol;
						if (!methodSymbol.IsSub)
						{
							TypeSymbol returnType = methodSymbol.ReturnType;
							if (CaseInsensitiveComparison.Equals(methodSymbol.Name, returnType.Name))
							{
								containingSymbol = returnType;
								break;
							}
							return;
						}
						return;
					}
					return;
				}
			}
		}

		private void BindSimpleNameForCref(string name, int arity, ArrayBuilder<Symbol> symbols, bool preserveAliases, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, Symbol containingSymbol = null, bool allowColorColor = false, bool typeOrNamespaceOnly = false)
		{
			if (!string.IsNullOrEmpty(name))
			{
				LookupOptions lookupOptions = LookupOptions.MustNotBeReturnValueVariable | LookupOptions.IgnoreAccessibility | LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility | LookupOptions.MustNotBeLocalOrParameter | LookupOptions.NoSystemObjectLookupForInterfaces;
				if (arity < 0)
				{
					lookupOptions |= LookupOptions.AllMethodsOfAnyArity;
				}
				if (typeOrNamespaceOnly)
				{
					lookupOptions |= LookupOptions.NamespacesOrTypesOnly;
				}
				LookupResult instance = LookupResult.GetInstance();
				if ((object)containingSymbol == null)
				{
					Lookup(instance, name, arity, lookupOptions, ref useSiteInfo);
				}
				else
				{
					LookupSimpleNameInContainingSymbol(containingSymbol, allowColorColor, name, arity, preserveAliases, instance, lookupOptions, ref useSiteInfo);
				}
				if (!instance.IsGoodOrAmbiguous || !instance.HasSymbol)
				{
					instance.Free();
				}
				else
				{
					CreateGoodOrAmbiguousFromLookupResultAndFree(instance, symbols, preserveAliases);
				}
			}
		}

		private void BindSimpleNameForCref(SimpleNameSyntax node, ArrayBuilder<Symbol> symbols, bool preserveAliases, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool typeOrNamespaceOnly)
		{
			if (node.ContainsDiagnostics)
			{
				return;
			}
			if (node.Kind() == SyntaxKind.GenericName)
			{
				GenericNameSyntax genericNameSyntax = (GenericNameSyntax)node;
				BindSimpleNameForCref(genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList.Arguments.Count, symbols, preserveAliases, ref useSiteInfo, null, allowColorColor: false, typeOrNamespaceOnly);
				if (symbols.Count == 1)
				{
					symbols[0] = ConstructGenericSymbolWithTypeArgumentsForCref(symbols[0], genericNameSyntax);
				}
			}
			else
			{
				string valueText = ((IdentifierNameSyntax)node).Identifier.ValueText;
				BindSimpleNameForCref(valueText, 0, symbols, preserveAliases, ref useSiteInfo, null, allowColorColor: false, typeOrNamespaceOnly);
				if (symbols.Count <= 0)
				{
					BindSimpleNameForCref(valueText, -1, symbols, preserveAliases, ref useSiteInfo, null, allowColorColor: false, typeOrNamespaceOnly);
				}
			}
		}

		private void BindPredefinedTypeForCref(PredefinedTypeSyntax node, ArrayBuilder<Symbol> symbols)
		{
			if (!node.ContainsDiagnostics)
			{
				symbols.Add(GetSpecialType(VisualBasicExtensions.Kind(node.Keyword) switch
				{
					SyntaxKind.ObjectKeyword => SpecialType.System_Object, 
					SyntaxKind.BooleanKeyword => SpecialType.System_Boolean, 
					SyntaxKind.DateKeyword => SpecialType.System_DateTime, 
					SyntaxKind.CharKeyword => SpecialType.System_Char, 
					SyntaxKind.StringKeyword => SpecialType.System_String, 
					SyntaxKind.DecimalKeyword => SpecialType.System_Decimal, 
					SyntaxKind.ByteKeyword => SpecialType.System_Byte, 
					SyntaxKind.SByteKeyword => SpecialType.System_SByte, 
					SyntaxKind.UShortKeyword => SpecialType.System_UInt16, 
					SyntaxKind.ShortKeyword => SpecialType.System_Int16, 
					SyntaxKind.UIntegerKeyword => SpecialType.System_UInt32, 
					SyntaxKind.IntegerKeyword => SpecialType.System_Int32, 
					SyntaxKind.ULongKeyword => SpecialType.System_UInt64, 
					SyntaxKind.LongKeyword => SpecialType.System_Int64, 
					SyntaxKind.SingleKeyword => SpecialType.System_Single, 
					SyntaxKind.DoubleKeyword => SpecialType.System_Double, 
					_ => throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(node.Keyword)), 
				}, node, BindingDiagnosticBag.Discarded));
			}
		}

		private Symbol ConstructGenericSymbolWithTypeArgumentsForCref(Symbol genericSymbol, GenericNameSyntax genericName)
		{
			switch (genericSymbol.Kind)
			{
			case SymbolKind.Method:
				return ((MethodSymbol)genericSymbol).Construct(BingTypeArgumentsForCref(genericName.TypeArgumentList.Arguments));
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
				return ((NamedTypeSymbol)genericSymbol).Construct(BingTypeArgumentsForCref(genericName.TypeArgumentList.Arguments));
			case SymbolKind.Alias:
			{
				AliasSymbol aliasSymbol = (AliasSymbol)genericSymbol;
				return ConstructGenericSymbolWithTypeArgumentsForCref(aliasSymbol.Target, genericName);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(genericSymbol.Kind);
			}
		}

		private ImmutableArray<TypeSymbol> BingTypeArgumentsForCref(SeparatedSyntaxList<TypeSyntax> args)
		{
			TypeSymbol[] array = new TypeSymbol[args.Count - 1 + 1];
			int num = args.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = BindTypeSyntax(args[i], BindingDiagnosticBag.Discarded);
			}
			return array.AsImmutableOrNull();
		}

		private static void CreateGoodOrAmbiguousFromLookupResultAndFree(LookupResult lookupResult, ArrayBuilder<Symbol> result, bool preserveAliases)
		{
			DiagnosticInfo diagnostic = lookupResult.Diagnostic;
			if (diagnostic is AmbiguousSymbolDiagnostic)
			{
				ImmutableArray<Symbol> ambiguousSymbols = ((AmbiguousSymbolDiagnostic)diagnostic).AmbiguousSymbols;
				if (preserveAliases)
				{
					result.AddRange(ambiguousSymbols);
				}
				else
				{
					ImmutableArray<Symbol>.Enumerator enumerator = ambiguousSymbols.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						result.Add(SymbolExtensions.UnwrapAlias(current));
					}
				}
			}
			else if (preserveAliases)
			{
				result.AddRange(lookupResult.Symbols);
			}
			else
			{
				ArrayBuilder<Symbol>.Enumerator enumerator2 = lookupResult.Symbols.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					result.Add(SymbolExtensions.UnwrapAlias(current2));
				}
			}
			lookupResult.Free();
		}
	}
}
