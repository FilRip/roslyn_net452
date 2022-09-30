using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class BinderBuilder
	{
		public static Binder CreateBinderForSourceFileImports(SourceModuleSymbol moduleSymbol, SyntaxTree tree)
		{
			Binder containingBinder = new IgnoreBaseClassesBinder(new NamespaceBinder(new SourceFileBinder(CreateSourceModuleBinder(moduleSymbol), moduleSymbol.TryGetSourceFile(tree), tree), moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace));
			return new LocationSpecificBinder(BindingLocation.SourceFileImportsDeclaration, containingBinder);
		}

		public static Binder CreateBinderForProjectImports(SourceModuleSymbol moduleSymbol, SyntaxTree tree)
		{
			Binder containingBinder = new IgnoreBaseClassesBinder(new NamespaceBinder(new ProjectImportsBinder(CreateSourceModuleBinder(moduleSymbol), tree), moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace));
			return new LocationSpecificBinder(BindingLocation.ProjectImportsDeclaration, containingBinder);
		}

		private static Binder CreateBinderForSourceFile(SourceModuleSymbol moduleSymbol, SyntaxTree tree)
		{
			Binder binder = CreateSourceModuleBinder(moduleSymbol);
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports = moduleSymbol.MemberImports;
			if (memberImports.Length > 0)
			{
				binder = new TypesOfImportedNamespacesMembersBinder(binder, memberImports);
				binder = new ImportedTypesAndNamespacesMembersBinder(binder, memberImports);
			}
			Dictionary<string, AliasAndImportsClausePosition> aliasImportsMap = moduleSymbol.AliasImportsMap;
			if (aliasImportsMap != null)
			{
				binder = new ImportAliasesBinder(binder, aliasImportsMap);
			}
			Dictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespaces = moduleSymbol.XmlNamespaces;
			if (xmlNamespaces != null)
			{
				binder = new XmlNamespaceImportsBinder(binder, xmlNamespaces);
			}
			SourceFile sourceFile = moduleSymbol.TryGetSourceFile(tree);
			if (sourceFile == null)
			{
				return binder;
			}
			Binder binder2 = new SourceFileBinder(binder, sourceFile, tree);
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports2 = sourceFile.MemberImports;
			if (!memberImports2.IsEmpty)
			{
				binder2 = new TypesOfImportedNamespacesMembersBinder(binder2, memberImports2);
				binder2 = new ImportedTypesAndNamespacesMembersBinder(binder2, memberImports2);
			}
			IReadOnlyDictionary<string, AliasAndImportsClausePosition> aliasImportsOpt = sourceFile.AliasImportsOpt;
			if (aliasImportsOpt != null)
			{
				binder2 = new ImportAliasesBinder(binder2, aliasImportsOpt);
			}
			IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespacesOpt = sourceFile.XmlNamespacesOpt;
			if (xmlNamespacesOpt != null)
			{
				binder2 = new XmlNamespaceImportsBinder(binder2, xmlNamespacesOpt);
			}
			return binder2;
		}

		public static Binder CreateBinderForProjectLevelNamespace(SourceModuleSymbol moduleSymbol, SyntaxTree tree)
		{
			NamespaceSymbol rootNamespace = moduleSymbol.RootNamespace;
			return CreateBinderForNamespace(moduleSymbol, tree, rootNamespace);
		}

		public static NamespaceBinder CreateBinderForNamespace(SourceModuleSymbol moduleSymbol, SyntaxTree tree, NamespaceSymbol nsSymbol)
		{
			NamespaceSymbol containingNamespace = nsSymbol.ContainingNamespace;
			if ((object)containingNamespace == null)
			{
				Binder containingBinder = CreateBinderForSourceFile(moduleSymbol, tree);
				return new NamespaceBinder(containingBinder, moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace);
			}
			ArrayBuilder<NamespaceSymbol> instance = ArrayBuilder<NamespaceSymbol>.GetInstance();
			while ((object)containingNamespace != null)
			{
				instance.Push(nsSymbol);
				nsSymbol = containingNamespace;
				containingNamespace = nsSymbol.ContainingNamespace;
			}
			NamespaceBinder namespaceBinder = CreateBinderForNamespace(moduleSymbol, tree, nsSymbol);
			while (instance.Count > 0)
			{
				nsSymbol = instance.Pop();
				containingNamespace = nsSymbol.ContainingNamespace;
				if (namespaceBinder.NamespaceSymbol.Extent.Kind != nsSymbol.Extent.Kind)
				{
					nsSymbol = (NamespaceSymbol)namespaceBinder.NamespaceSymbol.GetMembers(nsSymbol.Name).First((Symbol s) => s.Kind == SymbolKind.Namespace);
				}
				namespaceBinder = new NamespaceBinder(namespaceBinder, nsSymbol);
			}
			instance.Free();
			return namespaceBinder;
		}

		public static Binder CreateBinderForType(SourceModuleSymbol moduleSymbol, SyntaxTree tree, NamedTypeSymbol typeSymbol)
		{
			Symbol containingSymbol = typeSymbol.ContainingSymbol;
			if (containingSymbol.Kind == SymbolKind.Namespace)
			{
				return new NamedTypeBinder(CreateBinderForNamespace(moduleSymbol, tree, (NamespaceSymbol)containingSymbol), typeSymbol);
			}
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			instance.Push(typeSymbol);
			while (containingSymbol.Kind != SymbolKind.Namespace)
			{
				typeSymbol = (NamedTypeSymbol)containingSymbol;
				containingSymbol = typeSymbol.ContainingSymbol;
				instance.Push(typeSymbol);
			}
			Binder binder = CreateBinderForNamespace(moduleSymbol, tree, (NamespaceSymbol)containingSymbol);
			while (instance.Count > 0)
			{
				typeSymbol = instance.Pop();
				binder = new NamedTypeBinder(binder, typeSymbol);
			}
			instance.Free();
			return binder;
		}

		public static AttributeBinder CreateBinderForAttribute(SourceModuleSymbol moduleSymbol, SyntaxTree tree, Symbol target)
		{
			SymbolKind kind = target.Kind;
			NamedTypeSymbol namedTypeSymbol = ((kind != SymbolKind.Parameter) ? target.ContainingType : target.ContainingSymbol.ContainingType);
			Binder containingBinder = (((object)namedTypeSymbol == null) ? CreateBinderForNamespace(moduleSymbol, tree, target.ContainingNamespace) : CreateBinderForType(moduleSymbol, tree, namedTypeSymbol));
			if (target is SourceMethodSymbol methodSymbol)
			{
				containingBinder = CreateBinderForMethodDeclaration(methodSymbol, containingBinder);
			}
			return new AttributeBinder(containingBinder, tree);
		}

		public static AttributeBinder CreateBinderForAttribute(SyntaxTree tree, Binder containingBinder, VisualBasicSyntaxNode node)
		{
			return new AttributeBinder(containingBinder, tree, node);
		}

		public static Binder CreateBinderForParameterDefaultValue(SourceModuleSymbol moduleSymbol, SyntaxTree tree, ParameterSymbol parameterSymbol, VisualBasicSyntaxNode node)
		{
			Symbol containingSymbol = parameterSymbol.ContainingSymbol;
			Binder next;
			if (containingSymbol is SourceMethodSymbol methodSymbol)
			{
				next = CreateBinderForMethodDeclaration(moduleSymbol, tree, methodSymbol);
			}
			else
			{
				NamedTypeSymbol containingType = containingSymbol.ContainingType;
				next = CreateBinderForType(moduleSymbol, tree, containingType);
			}
			return new DeclarationInitializerBinder(parameterSymbol, ImmutableArray<Symbol>.Empty, next, node);
		}

		public static Binder CreateBinderForParameterDefaultValue(ParameterSymbol parameterSymbol, Binder containingBinder, VisualBasicSyntaxNode node)
		{
			if (parameterSymbol.ContainingSymbol is SourceMethodSymbol methodSymbol)
			{
				containingBinder = CreateBinderForMethodDeclaration(methodSymbol, containingBinder);
			}
			return new DeclarationInitializerBinder(parameterSymbol, ImmutableArray<Symbol>.Empty, containingBinder, node);
		}

		public static Binder CreateBinderForDocumentationComment(Binder containingBinder, Symbol commentedSymbol, DocumentationCommentBinder.BinderType binderType)
		{
			return binderType switch
			{
				DocumentationCommentBinder.BinderType.Cref => new DocumentationCommentCrefBinder(containingBinder, commentedSymbol), 
				DocumentationCommentBinder.BinderType.NameInParamOrParamRef => new DocumentationCommentParamBinder(containingBinder, commentedSymbol), 
				DocumentationCommentBinder.BinderType.NameInTypeParam => new DocumentationCommentTypeParamBinder(containingBinder, commentedSymbol), 
				DocumentationCommentBinder.BinderType.NameInTypeParamRef => new DocumentationCommentTypeParamRefBinder(containingBinder, commentedSymbol), 
				_ => throw ExceptionUtilities.UnexpectedValue(binderType), 
			};
		}

		public static Binder CreateBinderForMethodDeclaration(MethodSymbol methodSymbol, Binder containingBinder)
		{
			if (methodSymbol.IsGenericMethod)
			{
				return new MethodTypeParametersBinder(containingBinder, methodSymbol.TypeParameters);
			}
			return containingBinder;
		}

		public static Binder CreateBinderForGenericMethodDeclaration(SourceMethodSymbol methodSymbol, Binder containingBinder)
		{
			return new MethodTypeParametersBinder(containingBinder, methodSymbol.TypeParameters);
		}

		public static Binder CreateBinderForMethodDeclaration(SourceModuleSymbol moduleSymbol, SyntaxTree tree, SourceMethodSymbol methodSymbol)
		{
			return CreateBinderForMethodDeclaration(methodSymbol, CreateBinderForType(moduleSymbol, tree, methodSymbol.ContainingType));
		}

		public static Binder CreateBinderForMethodBody(MethodSymbol methodSymbol, SyntaxNode root, Binder containingBinder)
		{
			Binder binder = CreateBinderForMethodDeclaration(methodSymbol, containingBinder);
			if (!binder.OptionExplicit)
			{
				binder = new ImplicitVariableBinder(binder, methodSymbol);
			}
			return new MethodBodyBinder(methodSymbol, root, binder);
		}

		public static Binder CreateBinderForMethodBody(SourceModuleSymbol moduleSymbol, SyntaxTree tree, SourceMethodSymbol methodSymbol)
		{
			return CreateBinderForMethodBody(methodSymbol, methodSymbol.Syntax, CreateBinderForType(moduleSymbol, tree, methodSymbol.ContainingType));
		}

		public static Binder CreateBinderForInitializer(Binder containingBinder, Symbol fieldOrProperty, ImmutableArray<Symbol> additionalFieldsOrProperties)
		{
			VisualBasicSyntaxNode root = ((fieldOrProperty.Kind != SymbolKind.Field) ? ((SourcePropertySymbol)fieldOrProperty).DeclarationSyntax : ((SourceFieldSymbol)fieldOrProperty).DeclarationSyntax);
			return new DeclarationInitializerBinder(fieldOrProperty, additionalFieldsOrProperties, containingBinder, root);
		}

		public static Binder CreateSourceModuleBinder(SourceModuleSymbol moduleSymbol)
		{
			return new SourceModuleBinder(new BackstopBinder(), moduleSymbol);
		}
	}
}
