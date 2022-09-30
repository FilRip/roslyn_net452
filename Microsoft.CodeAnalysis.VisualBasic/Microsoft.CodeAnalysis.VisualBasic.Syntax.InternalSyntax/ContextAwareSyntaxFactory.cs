using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class ContextAwareSyntaxFactory
	{
		private readonly ISyntaxFactoryContext _factoryContext;

		public ContextAwareSyntaxFactory(ISyntaxFactoryContext factoryContext)
		{
			_factoryContext = factoryContext;
		}

		internal EmptyStatementSyntax EmptyStatement(PunctuationSyntax empty)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(2, empty, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EmptyStatementSyntax)greenNode;
			}
			EmptyStatementSyntax emptyStatementSyntax = new EmptyStatementSyntax(SyntaxKind.EmptyStatement, empty, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(emptyStatementSyntax, hash);
			}
			return emptyStatementSyntax;
		}

		internal EndBlockStatementSyntax EndIfStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(5, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndIfStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndUsingStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(6, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndUsingStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndWithStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(7, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndWithStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndSelectStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(8, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSelectStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndStructureStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(9, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndStructureStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndEnumStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(10, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndEnumStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndInterfaceStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(11, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndInterfaceStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndClassStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(12, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndClassStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndModuleStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(13, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndModuleStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndNamespaceStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(14, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndNamespaceStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndSubStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(15, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSubStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndFunctionStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(16, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndFunctionStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndGetStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(17, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndGetStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndSetStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(18, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSetStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndPropertyStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(19, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndPropertyStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndOperatorStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(20, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndOperatorStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndEventStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(21, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndEventStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndAddHandlerStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(22, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndAddHandlerStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndRemoveHandlerStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(23, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndRemoveHandlerStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndRaiseEventStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(24, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndRaiseEventStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndWhileStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(25, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndWhileStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndTryStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(26, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndTryStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndSyncLockStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(27, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSyncLockStatement, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal EndBlockStatementSyntax EndBlockStatement(SyntaxKind kind, KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, endKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(kind, endKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> options, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> imports, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, PunctuationSyntax endOfFileToken)
		{
			return new CompilationUnitSyntax(SyntaxKind.CompilationUnit, options.Node, imports.Node, attributes.Node, members.Node, endOfFileToken, _factoryContext);
		}

		internal OptionStatementSyntax OptionStatement(KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(41, optionKeyword, nameKeyword, valueKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OptionStatementSyntax)greenNode;
			}
			OptionStatementSyntax optionStatementSyntax = new OptionStatementSyntax(SyntaxKind.OptionStatement, optionKeyword, nameKeyword, valueKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(optionStatementSyntax, hash);
			}
			return optionStatementSyntax;
		}

		internal ImportsStatementSyntax ImportsStatement(KeywordSyntax importsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> importsClauses)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(42, importsKeyword, importsClauses.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ImportsStatementSyntax)greenNode;
			}
			ImportsStatementSyntax importsStatementSyntax = new ImportsStatementSyntax(SyntaxKind.ImportsStatement, importsKeyword, importsClauses.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(importsStatementSyntax, hash);
			}
			return importsStatementSyntax;
		}

		internal SimpleImportsClauseSyntax SimpleImportsClause(ImportAliasClauseSyntax alias, NameSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(44, alias, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SimpleImportsClauseSyntax)greenNode;
			}
			SimpleImportsClauseSyntax simpleImportsClauseSyntax = new SimpleImportsClauseSyntax(SyntaxKind.SimpleImportsClause, alias, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleImportsClauseSyntax, hash);
			}
			return simpleImportsClauseSyntax;
		}

		internal ImportAliasClauseSyntax ImportAliasClause(IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(754, identifier, equalsToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ImportAliasClauseSyntax)greenNode;
			}
			ImportAliasClauseSyntax importAliasClauseSyntax = new ImportAliasClauseSyntax(SyntaxKind.ImportAliasClause, identifier, equalsToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(importAliasClauseSyntax, hash);
			}
			return importAliasClauseSyntax;
		}

		internal XmlNamespaceImportsClauseSyntax XmlNamespaceImportsClause(PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(45, lessThanToken, xmlNamespace, greaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlNamespaceImportsClauseSyntax)greenNode;
			}
			XmlNamespaceImportsClauseSyntax xmlNamespaceImportsClauseSyntax = new XmlNamespaceImportsClauseSyntax(SyntaxKind.XmlNamespaceImportsClause, lessThanToken, xmlNamespace, greaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlNamespaceImportsClauseSyntax, hash);
			}
			return xmlNamespaceImportsClauseSyntax;
		}

		internal NamespaceBlockSyntax NamespaceBlock(NamespaceStatementSyntax namespaceStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endNamespaceStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(48, namespaceStatement, members.Node, endNamespaceStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NamespaceBlockSyntax)greenNode;
			}
			NamespaceBlockSyntax namespaceBlockSyntax = new NamespaceBlockSyntax(SyntaxKind.NamespaceBlock, namespaceStatement, members.Node, endNamespaceStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namespaceBlockSyntax, hash);
			}
			return namespaceBlockSyntax;
		}

		internal NamespaceStatementSyntax NamespaceStatement(KeywordSyntax namespaceKeyword, NameSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(49, namespaceKeyword, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NamespaceStatementSyntax)greenNode;
			}
			NamespaceStatementSyntax namespaceStatementSyntax = new NamespaceStatementSyntax(SyntaxKind.NamespaceStatement, namespaceKeyword, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namespaceStatementSyntax, hash);
			}
			return namespaceStatementSyntax;
		}

		internal ModuleBlockSyntax ModuleBlock(ModuleStatementSyntax moduleStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endModuleStatement)
		{
			return new ModuleBlockSyntax(SyntaxKind.ModuleBlock, moduleStatement, inherits.Node, implements.Node, members.Node, endModuleStatement, _factoryContext);
		}

		internal StructureBlockSyntax StructureBlock(StructureStatementSyntax structureStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endStructureStatement)
		{
			return new StructureBlockSyntax(SyntaxKind.StructureBlock, structureStatement, inherits.Node, implements.Node, members.Node, endStructureStatement, _factoryContext);
		}

		internal InterfaceBlockSyntax InterfaceBlock(InterfaceStatementSyntax interfaceStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endInterfaceStatement)
		{
			return new InterfaceBlockSyntax(SyntaxKind.InterfaceBlock, interfaceStatement, inherits.Node, implements.Node, members.Node, endInterfaceStatement, _factoryContext);
		}

		internal ClassBlockSyntax ClassBlock(ClassStatementSyntax classStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endClassStatement)
		{
			return new ClassBlockSyntax(SyntaxKind.ClassBlock, classStatement, inherits.Node, implements.Node, members.Node, endClassStatement, _factoryContext);
		}

		internal EnumBlockSyntax EnumBlock(EnumStatementSyntax enumStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endEnumStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(54, enumStatement, members.Node, endEnumStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EnumBlockSyntax)greenNode;
			}
			EnumBlockSyntax enumBlockSyntax = new EnumBlockSyntax(SyntaxKind.EnumBlock, enumStatement, members.Node, endEnumStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(enumBlockSyntax, hash);
			}
			return enumBlockSyntax;
		}

		internal InheritsStatementSyntax InheritsStatement(KeywordSyntax inheritsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> types)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(57, inheritsKeyword, types.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InheritsStatementSyntax)greenNode;
			}
			InheritsStatementSyntax inheritsStatementSyntax = new InheritsStatementSyntax(SyntaxKind.InheritsStatement, inheritsKeyword, types.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(inheritsStatementSyntax, hash);
			}
			return inheritsStatementSyntax;
		}

		internal ImplementsStatementSyntax ImplementsStatement(KeywordSyntax implementsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> types)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(58, implementsKeyword, types.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ImplementsStatementSyntax)greenNode;
			}
			ImplementsStatementSyntax implementsStatementSyntax = new ImplementsStatementSyntax(SyntaxKind.ImplementsStatement, implementsKeyword, types.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(implementsStatementSyntax, hash);
			}
			return implementsStatementSyntax;
		}

		internal ModuleStatementSyntax ModuleStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax moduleKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new ModuleStatementSyntax(SyntaxKind.ModuleStatement, attributeLists.Node, modifiers.Node, moduleKeyword, identifier, typeParameterList, _factoryContext);
		}

		internal StructureStatementSyntax StructureStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new StructureStatementSyntax(SyntaxKind.StructureStatement, attributeLists.Node, modifiers.Node, structureKeyword, identifier, typeParameterList, _factoryContext);
		}

		internal InterfaceStatementSyntax InterfaceStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax interfaceKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new InterfaceStatementSyntax(SyntaxKind.InterfaceStatement, attributeLists.Node, modifiers.Node, interfaceKeyword, identifier, typeParameterList, _factoryContext);
		}

		internal ClassStatementSyntax ClassStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new ClassStatementSyntax(SyntaxKind.ClassStatement, attributeLists.Node, modifiers.Node, classKeyword, identifier, typeParameterList, _factoryContext);
		}

		internal EnumStatementSyntax EnumStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType)
		{
			return new EnumStatementSyntax(SyntaxKind.EnumStatement, attributeLists.Node, modifiers.Node, enumKeyword, identifier, underlyingType, _factoryContext);
		}

		internal TypeParameterListSyntax TypeParameterList(PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> parameters, PunctuationSyntax closeParenToken)
		{
			return new TypeParameterListSyntax(SyntaxKind.TypeParameterList, openParenToken, ofKeyword, parameters.Node, closeParenToken, _factoryContext);
		}

		internal TypeParameterSyntax TypeParameter(KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(67, varianceKeyword, identifier, typeParameterConstraintClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TypeParameterSyntax)greenNode;
			}
			TypeParameterSyntax typeParameterSyntax = new TypeParameterSyntax(SyntaxKind.TypeParameter, varianceKeyword, identifier, typeParameterConstraintClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeParameterSyntax, hash);
			}
			return typeParameterSyntax;
		}

		internal TypeParameterSingleConstraintClauseSyntax TypeParameterSingleConstraintClause(KeywordSyntax asKeyword, ConstraintSyntax constraint)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(70, asKeyword, constraint, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TypeParameterSingleConstraintClauseSyntax)greenNode;
			}
			TypeParameterSingleConstraintClauseSyntax typeParameterSingleConstraintClauseSyntax = new TypeParameterSingleConstraintClauseSyntax(SyntaxKind.TypeParameterSingleConstraintClause, asKeyword, constraint, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeParameterSingleConstraintClauseSyntax, hash);
			}
			return typeParameterSingleConstraintClauseSyntax;
		}

		internal TypeParameterMultipleConstraintClauseSyntax TypeParameterMultipleConstraintClause(KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> constraints, PunctuationSyntax closeBraceToken)
		{
			return new TypeParameterMultipleConstraintClauseSyntax(SyntaxKind.TypeParameterMultipleConstraintClause, asKeyword, openBraceToken, constraints.Node, closeBraceToken, _factoryContext);
		}

		internal SpecialConstraintSyntax NewConstraint(KeywordSyntax constraintKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(72, constraintKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.NewConstraint, constraintKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal SpecialConstraintSyntax ClassConstraint(KeywordSyntax constraintKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(73, constraintKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.ClassConstraint, constraintKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal SpecialConstraintSyntax StructureConstraint(KeywordSyntax constraintKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(74, constraintKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.StructureConstraint, constraintKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal SpecialConstraintSyntax SpecialConstraint(SyntaxKind kind, KeywordSyntax constraintKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, constraintKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(kind, constraintKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal TypeConstraintSyntax TypeConstraint(TypeSyntax type)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(75, type, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TypeConstraintSyntax)greenNode;
			}
			TypeConstraintSyntax typeConstraintSyntax = new TypeConstraintSyntax(SyntaxKind.TypeConstraint, type, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeConstraintSyntax, hash);
			}
			return typeConstraintSyntax;
		}

		internal EnumMemberDeclarationSyntax EnumMemberDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(78, attributeLists.Node, identifier, initializer, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EnumMemberDeclarationSyntax)greenNode;
			}
			EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = new EnumMemberDeclarationSyntax(SyntaxKind.EnumMemberDeclaration, attributeLists.Node, identifier, initializer, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(enumMemberDeclarationSyntax, hash);
			}
			return enumMemberDeclarationSyntax;
		}

		internal MethodBlockSyntax SubBlock(MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(79, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(SyntaxKind.SubBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal MethodBlockSyntax FunctionBlock(MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(80, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(SyntaxKind.FunctionBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal MethodBlockSyntax MethodBlock(SyntaxKind kind, MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal ConstructorBlockSyntax ConstructorBlock(SubNewStatementSyntax subNewStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(81, subNewStatement, statements.Node, endSubStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ConstructorBlockSyntax)greenNode;
			}
			ConstructorBlockSyntax constructorBlockSyntax = new ConstructorBlockSyntax(SyntaxKind.ConstructorBlock, subNewStatement, statements.Node, endSubStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(constructorBlockSyntax, hash);
			}
			return constructorBlockSyntax;
		}

		internal OperatorBlockSyntax OperatorBlock(OperatorStatementSyntax operatorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endOperatorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(82, operatorStatement, statements.Node, endOperatorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OperatorBlockSyntax)greenNode;
			}
			OperatorBlockSyntax operatorBlockSyntax = new OperatorBlockSyntax(SyntaxKind.OperatorBlock, operatorStatement, statements.Node, endOperatorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(operatorBlockSyntax, hash);
			}
			return operatorBlockSyntax;
		}

		internal AccessorBlockSyntax GetAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(83, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.GetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal AccessorBlockSyntax SetAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(84, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.SetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal AccessorBlockSyntax AddHandlerAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(85, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.AddHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal AccessorBlockSyntax RemoveHandlerAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(86, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.RemoveHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal AccessorBlockSyntax RaiseEventAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(87, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.RaiseEventAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal AccessorBlockSyntax AccessorBlock(SyntaxKind kind, AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, accessorStatement, statements.Node, endAccessorStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(kind, accessorStatement, statements.Node, endAccessorStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal PropertyBlockSyntax PropertyBlock(PropertyStatementSyntax propertyStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> accessors, EndBlockStatementSyntax endPropertyStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(88, propertyStatement, accessors.Node, endPropertyStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PropertyBlockSyntax)greenNode;
			}
			PropertyBlockSyntax propertyBlockSyntax = new PropertyBlockSyntax(SyntaxKind.PropertyBlock, propertyStatement, accessors.Node, endPropertyStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(propertyBlockSyntax, hash);
			}
			return propertyBlockSyntax;
		}

		internal EventBlockSyntax EventBlock(EventStatementSyntax eventStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> accessors, EndBlockStatementSyntax endEventStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(89, eventStatement, accessors.Node, endEventStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EventBlockSyntax)greenNode;
			}
			EventBlockSyntax eventBlockSyntax = new EventBlockSyntax(SyntaxKind.EventBlock, eventStatement, accessors.Node, endEventStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(eventBlockSyntax, hash);
			}
			return eventBlockSyntax;
		}

		internal ParameterListSyntax ParameterList(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> parameters, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(92, openParenToken, parameters.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ParameterListSyntax)greenNode;
			}
			ParameterListSyntax parameterListSyntax = new ParameterListSyntax(SyntaxKind.ParameterList, openParenToken, parameters.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(parameterListSyntax, hash);
			}
			return parameterListSyntax;
		}

		internal MethodStatementSyntax SubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(SyntaxKind.SubStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, _factoryContext);
		}

		internal MethodStatementSyntax FunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(SyntaxKind.FunctionStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, _factoryContext);
		}

		internal MethodStatementSyntax MethodStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, _factoryContext);
		}

		internal SubNewStatementSyntax SubNewStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList)
		{
			return new SubNewStatementSyntax(SyntaxKind.SubNewStatement, attributeLists.Node, modifiers.Node, subKeyword, newKeyword, parameterList, _factoryContext);
		}

		internal DeclareStatementSyntax DeclareSubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(SyntaxKind.DeclareSubStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, _factoryContext);
		}

		internal DeclareStatementSyntax DeclareFunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(SyntaxKind.DeclareFunctionStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, _factoryContext);
		}

		internal DeclareStatementSyntax DeclareStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(kind, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, _factoryContext);
		}

		internal DelegateStatementSyntax DelegateSubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(SyntaxKind.DelegateSubStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, _factoryContext);
		}

		internal DelegateStatementSyntax DelegateFunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(SyntaxKind.DelegateFunctionStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, _factoryContext);
		}

		internal DelegateStatementSyntax DelegateStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(kind, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, _factoryContext);
		}

		internal EventStatementSyntax EventStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
		{
			return new EventStatementSyntax(SyntaxKind.EventStatement, attributeLists.Node, modifiers.Node, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause, _factoryContext);
		}

		internal OperatorStatementSyntax OperatorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new OperatorStatementSyntax(SyntaxKind.OperatorStatement, attributeLists.Node, modifiers.Node, operatorKeyword, operatorToken, parameterList, asClause, _factoryContext);
		}

		internal PropertyStatementSyntax PropertyStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
		{
			return new PropertyStatementSyntax(SyntaxKind.PropertyStatement, attributeLists.Node, modifiers.Node, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause, _factoryContext);
		}

		internal AccessorStatementSyntax GetAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.GetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal AccessorStatementSyntax SetAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.SetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal AccessorStatementSyntax AddHandlerAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.AddHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal AccessorStatementSyntax RemoveHandlerAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.RemoveHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal AccessorStatementSyntax RaiseEventAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.RaiseEventAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal AccessorStatementSyntax AccessorStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(kind, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, _factoryContext);
		}

		internal ImplementsClauseSyntax ImplementsClause(KeywordSyntax implementsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> interfaceMembers)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(112, implementsKeyword, interfaceMembers.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ImplementsClauseSyntax)greenNode;
			}
			ImplementsClauseSyntax implementsClauseSyntax = new ImplementsClauseSyntax(SyntaxKind.ImplementsClause, implementsKeyword, interfaceMembers.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(implementsClauseSyntax, hash);
			}
			return implementsClauseSyntax;
		}

		internal HandlesClauseSyntax HandlesClause(KeywordSyntax handlesKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> events)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(113, handlesKeyword, events.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (HandlesClauseSyntax)greenNode;
			}
			HandlesClauseSyntax handlesClauseSyntax = new HandlesClauseSyntax(SyntaxKind.HandlesClause, handlesKeyword, events.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(handlesClauseSyntax, hash);
			}
			return handlesClauseSyntax;
		}

		internal KeywordEventContainerSyntax KeywordEventContainer(KeywordSyntax keyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(114, keyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (KeywordEventContainerSyntax)greenNode;
			}
			KeywordEventContainerSyntax keywordEventContainerSyntax = new KeywordEventContainerSyntax(SyntaxKind.KeywordEventContainer, keyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(keywordEventContainerSyntax, hash);
			}
			return keywordEventContainerSyntax;
		}

		internal WithEventsEventContainerSyntax WithEventsEventContainer(IdentifierTokenSyntax identifier)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(115, identifier, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WithEventsEventContainerSyntax)greenNode;
			}
			WithEventsEventContainerSyntax withEventsEventContainerSyntax = new WithEventsEventContainerSyntax(SyntaxKind.WithEventsEventContainer, identifier, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withEventsEventContainerSyntax, hash);
			}
			return withEventsEventContainerSyntax;
		}

		internal WithEventsPropertyEventContainerSyntax WithEventsPropertyEventContainer(WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(116, withEventsContainer, dotToken, property, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WithEventsPropertyEventContainerSyntax)greenNode;
			}
			WithEventsPropertyEventContainerSyntax withEventsPropertyEventContainerSyntax = new WithEventsPropertyEventContainerSyntax(SyntaxKind.WithEventsPropertyEventContainer, withEventsContainer, dotToken, property, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withEventsPropertyEventContainerSyntax, hash);
			}
			return withEventsPropertyEventContainerSyntax;
		}

		internal HandlesClauseItemSyntax HandlesClauseItem(EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(117, eventContainer, dotToken, eventMember, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (HandlesClauseItemSyntax)greenNode;
			}
			HandlesClauseItemSyntax handlesClauseItemSyntax = new HandlesClauseItemSyntax(SyntaxKind.HandlesClauseItem, eventContainer, dotToken, eventMember, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(handlesClauseItemSyntax, hash);
			}
			return handlesClauseItemSyntax;
		}

		internal IncompleteMemberSyntax IncompleteMember(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, IdentifierTokenSyntax missingIdentifier)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(118, attributeLists.Node, modifiers.Node, missingIdentifier, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (IncompleteMemberSyntax)greenNode;
			}
			IncompleteMemberSyntax incompleteMemberSyntax = new IncompleteMemberSyntax(SyntaxKind.IncompleteMember, attributeLists.Node, modifiers.Node, missingIdentifier, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(incompleteMemberSyntax, hash);
			}
			return incompleteMemberSyntax;
		}

		internal FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> declarators)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(119, attributeLists.Node, modifiers.Node, declarators.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (FieldDeclarationSyntax)greenNode;
			}
			FieldDeclarationSyntax fieldDeclarationSyntax = new FieldDeclarationSyntax(SyntaxKind.FieldDeclaration, attributeLists.Node, modifiers.Node, declarators.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(fieldDeclarationSyntax, hash);
			}
			return fieldDeclarationSyntax;
		}

		internal VariableDeclaratorSyntax VariableDeclarator(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(122, names.Node, asClause, initializer, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (VariableDeclaratorSyntax)greenNode;
			}
			VariableDeclaratorSyntax variableDeclaratorSyntax = new VariableDeclaratorSyntax(SyntaxKind.VariableDeclarator, names.Node, asClause, initializer, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(variableDeclaratorSyntax, hash);
			}
			return variableDeclaratorSyntax;
		}

		internal SimpleAsClauseSyntax SimpleAsClause(KeywordSyntax asKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(123, asKeyword, attributeLists.Node, type, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SimpleAsClauseSyntax)greenNode;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = new SimpleAsClauseSyntax(SyntaxKind.SimpleAsClause, asKeyword, attributeLists.Node, type, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleAsClauseSyntax, hash);
			}
			return simpleAsClauseSyntax;
		}

		internal AsNewClauseSyntax AsNewClause(KeywordSyntax asKeyword, NewExpressionSyntax newExpression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(124, asKeyword, newExpression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AsNewClauseSyntax)greenNode;
			}
			AsNewClauseSyntax asNewClauseSyntax = new AsNewClauseSyntax(SyntaxKind.AsNewClause, asKeyword, newExpression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(asNewClauseSyntax, hash);
			}
			return asNewClauseSyntax;
		}

		internal ObjectMemberInitializerSyntax ObjectMemberInitializer(KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> initializers, PunctuationSyntax closeBraceToken)
		{
			return new ObjectMemberInitializerSyntax(SyntaxKind.ObjectMemberInitializer, withKeyword, openBraceToken, initializers.Node, closeBraceToken, _factoryContext);
		}

		internal ObjectCollectionInitializerSyntax ObjectCollectionInitializer(KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(126, fromKeyword, initializer, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ObjectCollectionInitializerSyntax)greenNode;
			}
			ObjectCollectionInitializerSyntax objectCollectionInitializerSyntax = new ObjectCollectionInitializerSyntax(SyntaxKind.ObjectCollectionInitializer, fromKeyword, initializer, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(objectCollectionInitializerSyntax, hash);
			}
			return objectCollectionInitializerSyntax;
		}

		internal InferredFieldInitializerSyntax InferredFieldInitializer(KeywordSyntax keyKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(127, keyKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InferredFieldInitializerSyntax)greenNode;
			}
			InferredFieldInitializerSyntax inferredFieldInitializerSyntax = new InferredFieldInitializerSyntax(SyntaxKind.InferredFieldInitializer, keyKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(inferredFieldInitializerSyntax, hash);
			}
			return inferredFieldInitializerSyntax;
		}

		internal NamedFieldInitializerSyntax NamedFieldInitializer(KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression)
		{
			return new NamedFieldInitializerSyntax(SyntaxKind.NamedFieldInitializer, keyKeyword, dotToken, name, equalsToken, expression, _factoryContext);
		}

		internal EqualsValueSyntax EqualsValue(PunctuationSyntax equalsToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(129, equalsToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EqualsValueSyntax)greenNode;
			}
			EqualsValueSyntax equalsValueSyntax = new EqualsValueSyntax(SyntaxKind.EqualsValue, equalsToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(equalsValueSyntax, hash);
			}
			return equalsValueSyntax;
		}

		internal ParameterSyntax Parameter(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
		{
			return new ParameterSyntax(SyntaxKind.Parameter, attributeLists.Node, modifiers.Node, identifier, asClause, @default, _factoryContext);
		}

		internal ModifiedIdentifierSyntax ModifiedIdentifier(IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> arrayRankSpecifiers)
		{
			return new ModifiedIdentifierSyntax(SyntaxKind.ModifiedIdentifier, identifier, nullable, arrayBounds, arrayRankSpecifiers.Node, _factoryContext);
		}

		internal ArrayRankSpecifierSyntax ArrayRankSpecifier(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> commaTokens, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(134, openParenToken, commaTokens.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ArrayRankSpecifierSyntax)greenNode;
			}
			ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = new ArrayRankSpecifierSyntax(SyntaxKind.ArrayRankSpecifier, openParenToken, commaTokens.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(arrayRankSpecifierSyntax, hash);
			}
			return arrayRankSpecifierSyntax;
		}

		internal AttributeListSyntax AttributeList(PunctuationSyntax lessThanToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> attributes, PunctuationSyntax greaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(135, lessThanToken, attributes.Node, greaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AttributeListSyntax)greenNode;
			}
			AttributeListSyntax attributeListSyntax = new AttributeListSyntax(SyntaxKind.AttributeList, lessThanToken, attributes.Node, greaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeListSyntax, hash);
			}
			return attributeListSyntax;
		}

		internal AttributeSyntax Attribute(AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(136, target, name, argumentList, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AttributeSyntax)greenNode;
			}
			AttributeSyntax attributeSyntax = new AttributeSyntax(SyntaxKind.Attribute, target, name, argumentList, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeSyntax, hash);
			}
			return attributeSyntax;
		}

		internal AttributeTargetSyntax AttributeTarget(KeywordSyntax attributeModifier, PunctuationSyntax colonToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(137, attributeModifier, colonToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AttributeTargetSyntax)greenNode;
			}
			AttributeTargetSyntax attributeTargetSyntax = new AttributeTargetSyntax(SyntaxKind.AttributeTarget, attributeModifier, colonToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeTargetSyntax, hash);
			}
			return attributeTargetSyntax;
		}

		internal AttributesStatementSyntax AttributesStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(138, attributeLists.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AttributesStatementSyntax)greenNode;
			}
			AttributesStatementSyntax attributesStatementSyntax = new AttributesStatementSyntax(SyntaxKind.AttributesStatement, attributeLists.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributesStatementSyntax, hash);
			}
			return attributesStatementSyntax;
		}

		internal ExpressionStatementSyntax ExpressionStatement(ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(139, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExpressionStatementSyntax)greenNode;
			}
			ExpressionStatementSyntax expressionStatementSyntax = new ExpressionStatementSyntax(SyntaxKind.ExpressionStatement, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(expressionStatementSyntax, hash);
			}
			return expressionStatementSyntax;
		}

		internal PrintStatementSyntax PrintStatement(PunctuationSyntax questionToken, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(140, questionToken, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PrintStatementSyntax)greenNode;
			}
			PrintStatementSyntax printStatementSyntax = new PrintStatementSyntax(SyntaxKind.PrintStatement, questionToken, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(printStatementSyntax, hash);
			}
			return printStatementSyntax;
		}

		internal WhileBlockSyntax WhileBlock(WhileStatementSyntax whileStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endWhileStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(141, whileStatement, statements.Node, endWhileStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhileBlockSyntax)greenNode;
			}
			WhileBlockSyntax whileBlockSyntax = new WhileBlockSyntax(SyntaxKind.WhileBlock, whileStatement, statements.Node, endWhileStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileBlockSyntax, hash);
			}
			return whileBlockSyntax;
		}

		internal UsingBlockSyntax UsingBlock(UsingStatementSyntax usingStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endUsingStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(144, usingStatement, statements.Node, endUsingStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UsingBlockSyntax)greenNode;
			}
			UsingBlockSyntax usingBlockSyntax = new UsingBlockSyntax(SyntaxKind.UsingBlock, usingStatement, statements.Node, endUsingStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(usingBlockSyntax, hash);
			}
			return usingBlockSyntax;
		}

		internal SyncLockBlockSyntax SyncLockBlock(SyncLockStatementSyntax syncLockStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSyncLockStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(145, syncLockStatement, statements.Node, endSyncLockStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SyncLockBlockSyntax)greenNode;
			}
			SyncLockBlockSyntax syncLockBlockSyntax = new SyncLockBlockSyntax(SyntaxKind.SyncLockBlock, syncLockStatement, statements.Node, endSyncLockStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(syncLockBlockSyntax, hash);
			}
			return syncLockBlockSyntax;
		}

		internal WithBlockSyntax WithBlock(WithStatementSyntax withStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endWithStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(146, withStatement, statements.Node, endWithStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WithBlockSyntax)greenNode;
			}
			WithBlockSyntax withBlockSyntax = new WithBlockSyntax(SyntaxKind.WithBlock, withStatement, statements.Node, endWithStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withBlockSyntax, hash);
			}
			return withBlockSyntax;
		}

		internal LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> declarators)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(147, modifiers.Node, declarators.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LocalDeclarationStatementSyntax)greenNode;
			}
			LocalDeclarationStatementSyntax localDeclarationStatementSyntax = new LocalDeclarationStatementSyntax(SyntaxKind.LocalDeclarationStatement, modifiers.Node, declarators.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(localDeclarationStatementSyntax, hash);
			}
			return localDeclarationStatementSyntax;
		}

		internal LabelStatementSyntax LabelStatement(SyntaxToken labelToken, PunctuationSyntax colonToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(148, labelToken, colonToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LabelStatementSyntax)greenNode;
			}
			LabelStatementSyntax labelStatementSyntax = new LabelStatementSyntax(SyntaxKind.LabelStatement, labelToken, colonToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelStatementSyntax, hash);
			}
			return labelStatementSyntax;
		}

		internal GoToStatementSyntax GoToStatement(KeywordSyntax goToKeyword, LabelSyntax label)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(149, goToKeyword, label, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (GoToStatementSyntax)greenNode;
			}
			GoToStatementSyntax goToStatementSyntax = new GoToStatementSyntax(SyntaxKind.GoToStatement, goToKeyword, label, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(goToStatementSyntax, hash);
			}
			return goToStatementSyntax;
		}

		internal LabelSyntax IdentifierLabel(SyntaxToken labelToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(150, labelToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.IdentifierLabel, labelToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal LabelSyntax NumericLabel(SyntaxToken labelToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(151, labelToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.NumericLabel, labelToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal LabelSyntax NextLabel(SyntaxToken labelToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(152, labelToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.NextLabel, labelToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal LabelSyntax Label(SyntaxKind kind, SyntaxToken labelToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, labelToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(kind, labelToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal StopOrEndStatementSyntax StopStatement(KeywordSyntax stopOrEndKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(153, stopOrEndKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(SyntaxKind.StopStatement, stopOrEndKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal StopOrEndStatementSyntax EndStatement(KeywordSyntax stopOrEndKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(156, stopOrEndKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(SyntaxKind.EndStatement, stopOrEndKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal StopOrEndStatementSyntax StopOrEndStatement(SyntaxKind kind, KeywordSyntax stopOrEndKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, stopOrEndKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(kind, stopOrEndKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal ExitStatementSyntax ExitDoStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(157, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitDoStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitForStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(158, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitForStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitSubStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(159, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitSubStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitFunctionStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(160, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitFunctionStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitOperatorStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(161, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitOperatorStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitPropertyStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(162, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitPropertyStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitTryStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(163, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitTryStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitSelectStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(164, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitSelectStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitWhileStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(165, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitWhileStatement, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ExitStatementSyntax ExitStatement(SyntaxKind kind, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, exitKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(kind, exitKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal ContinueStatementSyntax ContinueWhileStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(166, continueKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueWhileStatement, continueKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal ContinueStatementSyntax ContinueDoStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(167, continueKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueDoStatement, continueKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal ContinueStatementSyntax ContinueForStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(168, continueKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueForStatement, continueKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal ContinueStatementSyntax ContinueStatement(SyntaxKind kind, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, continueKeyword, blockKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(kind, continueKeyword, blockKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal ReturnStatementSyntax ReturnStatement(KeywordSyntax returnKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(169, returnKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ReturnStatementSyntax)greenNode;
			}
			ReturnStatementSyntax returnStatementSyntax = new ReturnStatementSyntax(SyntaxKind.ReturnStatement, returnKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(returnStatementSyntax, hash);
			}
			return returnStatementSyntax;
		}

		internal SingleLineIfStatementSyntax SingleLineIfStatement(KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, SingleLineElseClauseSyntax elseClause)
		{
			return new SingleLineIfStatementSyntax(SyntaxKind.SingleLineIfStatement, ifKeyword, condition, thenKeyword, statements.Node, elseClause, _factoryContext);
		}

		internal SingleLineElseClauseSyntax SingleLineElseClause(KeywordSyntax elseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(172, elseKeyword, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SingleLineElseClauseSyntax)greenNode;
			}
			SingleLineElseClauseSyntax singleLineElseClauseSyntax = new SingleLineElseClauseSyntax(SyntaxKind.SingleLineElseClause, elseKeyword, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineElseClauseSyntax, hash);
			}
			return singleLineElseClauseSyntax;
		}

		internal MultiLineIfBlockSyntax MultiLineIfBlock(IfStatementSyntax ifStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
		{
			return new MultiLineIfBlockSyntax(SyntaxKind.MultiLineIfBlock, ifStatement, statements.Node, elseIfBlocks.Node, elseBlock, endIfStatement, _factoryContext);
		}

		internal IfStatementSyntax IfStatement(KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(182, ifKeyword, condition, thenKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (IfStatementSyntax)greenNode;
			}
			IfStatementSyntax ifStatementSyntax = new IfStatementSyntax(SyntaxKind.IfStatement, ifKeyword, condition, thenKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(ifStatementSyntax, hash);
			}
			return ifStatementSyntax;
		}

		internal ElseIfBlockSyntax ElseIfBlock(ElseIfStatementSyntax elseIfStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(180, elseIfStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ElseIfBlockSyntax)greenNode;
			}
			ElseIfBlockSyntax elseIfBlockSyntax = new ElseIfBlockSyntax(SyntaxKind.ElseIfBlock, elseIfStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseIfBlockSyntax, hash);
			}
			return elseIfBlockSyntax;
		}

		internal ElseIfStatementSyntax ElseIfStatement(KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(183, elseIfKeyword, condition, thenKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ElseIfStatementSyntax)greenNode;
			}
			ElseIfStatementSyntax elseIfStatementSyntax = new ElseIfStatementSyntax(SyntaxKind.ElseIfStatement, elseIfKeyword, condition, thenKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseIfStatementSyntax, hash);
			}
			return elseIfStatementSyntax;
		}

		internal ElseBlockSyntax ElseBlock(ElseStatementSyntax elseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(181, elseStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ElseBlockSyntax)greenNode;
			}
			ElseBlockSyntax elseBlockSyntax = new ElseBlockSyntax(SyntaxKind.ElseBlock, elseStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseBlockSyntax, hash);
			}
			return elseBlockSyntax;
		}

		internal ElseStatementSyntax ElseStatement(KeywordSyntax elseKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(184, elseKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ElseStatementSyntax)greenNode;
			}
			ElseStatementSyntax elseStatementSyntax = new ElseStatementSyntax(SyntaxKind.ElseStatement, elseKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseStatementSyntax, hash);
			}
			return elseStatementSyntax;
		}

		internal TryBlockSyntax TryBlock(TryStatementSyntax tryStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
		{
			return new TryBlockSyntax(SyntaxKind.TryBlock, tryStatement, statements.Node, catchBlocks.Node, finallyBlock, endTryStatement, _factoryContext);
		}

		internal TryStatementSyntax TryStatement(KeywordSyntax tryKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(189, tryKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TryStatementSyntax)greenNode;
			}
			TryStatementSyntax tryStatementSyntax = new TryStatementSyntax(SyntaxKind.TryStatement, tryKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tryStatementSyntax, hash);
			}
			return tryStatementSyntax;
		}

		internal CatchBlockSyntax CatchBlock(CatchStatementSyntax catchStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(187, catchStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CatchBlockSyntax)greenNode;
			}
			CatchBlockSyntax catchBlockSyntax = new CatchBlockSyntax(SyntaxKind.CatchBlock, catchStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(catchBlockSyntax, hash);
			}
			return catchBlockSyntax;
		}

		internal CatchStatementSyntax CatchStatement(KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
		{
			return new CatchStatementSyntax(SyntaxKind.CatchStatement, catchKeyword, identifierName, asClause, whenClause, _factoryContext);
		}

		internal CatchFilterClauseSyntax CatchFilterClause(KeywordSyntax whenKeyword, ExpressionSyntax filter)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(191, whenKeyword, filter, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CatchFilterClauseSyntax)greenNode;
			}
			CatchFilterClauseSyntax catchFilterClauseSyntax = new CatchFilterClauseSyntax(SyntaxKind.CatchFilterClause, whenKeyword, filter, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(catchFilterClauseSyntax, hash);
			}
			return catchFilterClauseSyntax;
		}

		internal FinallyBlockSyntax FinallyBlock(FinallyStatementSyntax finallyStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(188, finallyStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (FinallyBlockSyntax)greenNode;
			}
			FinallyBlockSyntax finallyBlockSyntax = new FinallyBlockSyntax(SyntaxKind.FinallyBlock, finallyStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(finallyBlockSyntax, hash);
			}
			return finallyBlockSyntax;
		}

		internal FinallyStatementSyntax FinallyStatement(KeywordSyntax finallyKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(194, finallyKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (FinallyStatementSyntax)greenNode;
			}
			FinallyStatementSyntax finallyStatementSyntax = new FinallyStatementSyntax(SyntaxKind.FinallyStatement, finallyKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(finallyStatementSyntax, hash);
			}
			return finallyStatementSyntax;
		}

		internal ErrorStatementSyntax ErrorStatement(KeywordSyntax errorKeyword, ExpressionSyntax errorNumber)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(195, errorKeyword, errorNumber, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ErrorStatementSyntax)greenNode;
			}
			ErrorStatementSyntax errorStatementSyntax = new ErrorStatementSyntax(SyntaxKind.ErrorStatement, errorKeyword, errorNumber, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(errorStatementSyntax, hash);
			}
			return errorStatementSyntax;
		}

		internal OnErrorGoToStatementSyntax OnErrorGoToZeroStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToZeroStatement, onKeyword, errorKeyword, goToKeyword, minus, label, _factoryContext);
		}

		internal OnErrorGoToStatementSyntax OnErrorGoToMinusOneStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToMinusOneStatement, onKeyword, errorKeyword, goToKeyword, minus, label, _factoryContext);
		}

		internal OnErrorGoToStatementSyntax OnErrorGoToLabelStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToLabelStatement, onKeyword, errorKeyword, goToKeyword, minus, label, _factoryContext);
		}

		internal OnErrorGoToStatementSyntax OnErrorGoToStatement(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(kind, onKeyword, errorKeyword, goToKeyword, minus, label, _factoryContext);
		}

		internal OnErrorResumeNextStatementSyntax OnErrorResumeNextStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword)
		{
			return new OnErrorResumeNextStatementSyntax(SyntaxKind.OnErrorResumeNextStatement, onKeyword, errorKeyword, resumeKeyword, nextKeyword, _factoryContext);
		}

		internal ResumeStatementSyntax ResumeStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(200, resumeKeyword, label, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeStatement, resumeKeyword, label, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal ResumeStatementSyntax ResumeLabelStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(201, resumeKeyword, label, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeLabelStatement, resumeKeyword, label, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal ResumeStatementSyntax ResumeNextStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(202, resumeKeyword, label, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeNextStatement, resumeKeyword, label, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal SelectBlockSyntax SelectBlock(SelectStatementSyntax selectStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> caseBlocks, EndBlockStatementSyntax endSelectStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(203, selectStatement, caseBlocks.Node, endSelectStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SelectBlockSyntax)greenNode;
			}
			SelectBlockSyntax selectBlockSyntax = new SelectBlockSyntax(SyntaxKind.SelectBlock, selectStatement, caseBlocks.Node, endSelectStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectBlockSyntax, hash);
			}
			return selectBlockSyntax;
		}

		internal SelectStatementSyntax SelectStatement(KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(204, selectKeyword, caseKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SelectStatementSyntax)greenNode;
			}
			SelectStatementSyntax selectStatementSyntax = new SelectStatementSyntax(SyntaxKind.SelectStatement, selectKeyword, caseKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectStatementSyntax, hash);
			}
			return selectStatementSyntax;
		}

		internal CaseBlockSyntax CaseBlock(CaseStatementSyntax caseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(207, caseStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CaseBlockSyntax)greenNode;
			}
			CaseBlockSyntax caseBlockSyntax = new CaseBlockSyntax(SyntaxKind.CaseBlock, caseStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseBlockSyntax, hash);
			}
			return caseBlockSyntax;
		}

		internal CaseBlockSyntax CaseElseBlock(CaseStatementSyntax caseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(210, caseStatement, statements.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CaseBlockSyntax)greenNode;
			}
			CaseBlockSyntax caseBlockSyntax = new CaseBlockSyntax(SyntaxKind.CaseElseBlock, caseStatement, statements.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseBlockSyntax, hash);
			}
			return caseBlockSyntax;
		}

		internal CaseStatementSyntax CaseStatement(KeywordSyntax caseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> cases)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(211, caseKeyword, cases.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CaseStatementSyntax)greenNode;
			}
			CaseStatementSyntax caseStatementSyntax = new CaseStatementSyntax(SyntaxKind.CaseStatement, caseKeyword, cases.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseStatementSyntax, hash);
			}
			return caseStatementSyntax;
		}

		internal CaseStatementSyntax CaseElseStatement(KeywordSyntax caseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> cases)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(212, caseKeyword, cases.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CaseStatementSyntax)greenNode;
			}
			CaseStatementSyntax caseStatementSyntax = new CaseStatementSyntax(SyntaxKind.CaseElseStatement, caseKeyword, cases.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseStatementSyntax, hash);
			}
			return caseStatementSyntax;
		}

		internal ElseCaseClauseSyntax ElseCaseClause(KeywordSyntax elseKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(213, elseKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ElseCaseClauseSyntax)greenNode;
			}
			ElseCaseClauseSyntax elseCaseClauseSyntax = new ElseCaseClauseSyntax(SyntaxKind.ElseCaseClause, elseKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseCaseClauseSyntax, hash);
			}
			return elseCaseClauseSyntax;
		}

		internal SimpleCaseClauseSyntax SimpleCaseClause(ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(214, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SimpleCaseClauseSyntax)greenNode;
			}
			SimpleCaseClauseSyntax simpleCaseClauseSyntax = new SimpleCaseClauseSyntax(SyntaxKind.SimpleCaseClause, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleCaseClauseSyntax, hash);
			}
			return simpleCaseClauseSyntax;
		}

		internal RangeCaseClauseSyntax RangeCaseClause(ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(215, lowerBound, toKeyword, upperBound, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RangeCaseClauseSyntax)greenNode;
			}
			RangeCaseClauseSyntax rangeCaseClauseSyntax = new RangeCaseClauseSyntax(SyntaxKind.RangeCaseClause, lowerBound, toKeyword, upperBound, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(rangeCaseClauseSyntax, hash);
			}
			return rangeCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseEqualsClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(216, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseEqualsClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseNotEqualsClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(217, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseNotEqualsClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseLessThanClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(218, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseLessThanOrEqualClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(219, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanOrEqualClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseGreaterThanOrEqualClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(222, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanOrEqualClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax CaseGreaterThanClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(223, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanClause, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal RelationalCaseClauseSyntax RelationalCaseClause(SyntaxKind kind, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, isKeyword, operatorToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(kind, isKeyword, operatorToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal SyncLockStatementSyntax SyncLockStatement(KeywordSyntax syncLockKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(226, syncLockKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SyncLockStatementSyntax)greenNode;
			}
			SyncLockStatementSyntax syncLockStatementSyntax = new SyncLockStatementSyntax(SyntaxKind.SyncLockStatement, syncLockKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(syncLockStatementSyntax, hash);
			}
			return syncLockStatementSyntax;
		}

		internal DoLoopBlockSyntax SimpleDoLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(756, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.SimpleDoLoopBlock, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoLoopBlockSyntax DoWhileLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(757, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoWhileLoopBlock, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoLoopBlockSyntax DoUntilLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(758, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoUntilLoopBlock, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoLoopBlockSyntax DoLoopWhileBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(759, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoLoopWhileBlock, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoLoopBlockSyntax DoLoopUntilBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(760, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoLoopUntilBlock, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoLoopBlockSyntax DoLoopBlock(SyntaxKind kind, DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, doStatement, statements.Node, loopStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(kind, doStatement, statements.Node, loopStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal DoStatementSyntax SimpleDoStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(770, doKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.SimpleDoStatement, doKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal DoStatementSyntax DoWhileStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(771, doKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.DoWhileStatement, doKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal DoStatementSyntax DoUntilStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(772, doKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.DoUntilStatement, doKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal DoStatementSyntax DoStatement(SyntaxKind kind, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, doKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(kind, doKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal LoopStatementSyntax SimpleLoopStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(773, loopKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.SimpleLoopStatement, loopKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal LoopStatementSyntax LoopWhileStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(774, loopKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.LoopWhileStatement, loopKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal LoopStatementSyntax LoopUntilStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(775, loopKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.LoopUntilStatement, loopKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal LoopStatementSyntax LoopStatement(SyntaxKind kind, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, loopKeyword, whileOrUntilClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(kind, loopKeyword, whileOrUntilClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal WhileOrUntilClauseSyntax WhileClause(KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(776, whileOrUntilKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(SyntaxKind.WhileClause, whileOrUntilKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal WhileOrUntilClauseSyntax UntilClause(KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(777, whileOrUntilKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(SyntaxKind.UntilClause, whileOrUntilKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal WhileOrUntilClauseSyntax WhileOrUntilClause(SyntaxKind kind, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, whileOrUntilKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(kind, whileOrUntilKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal WhileStatementSyntax WhileStatement(KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(234, whileKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhileStatementSyntax)greenNode;
			}
			WhileStatementSyntax whileStatementSyntax = new WhileStatementSyntax(SyntaxKind.WhileStatement, whileKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileStatementSyntax, hash);
			}
			return whileStatementSyntax;
		}

		internal ForBlockSyntax ForBlock(ForStatementSyntax forStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, NextStatementSyntax nextStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(237, forStatement, statements.Node, nextStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ForBlockSyntax)greenNode;
			}
			ForBlockSyntax forBlockSyntax = new ForBlockSyntax(SyntaxKind.ForBlock, forStatement, statements.Node, nextStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forBlockSyntax, hash);
			}
			return forBlockSyntax;
		}

		internal ForEachBlockSyntax ForEachBlock(ForEachStatementSyntax forEachStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, NextStatementSyntax nextStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(238, forEachStatement, statements.Node, nextStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ForEachBlockSyntax)greenNode;
			}
			ForEachBlockSyntax forEachBlockSyntax = new ForEachBlockSyntax(SyntaxKind.ForEachBlock, forEachStatement, statements.Node, nextStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forEachBlockSyntax, hash);
			}
			return forEachBlockSyntax;
		}

		internal ForStatementSyntax ForStatement(KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
		{
			return new ForStatementSyntax(SyntaxKind.ForStatement, forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause, _factoryContext);
		}

		internal ForStepClauseSyntax ForStepClause(KeywordSyntax stepKeyword, ExpressionSyntax stepValue)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(240, stepKeyword, stepValue, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ForStepClauseSyntax)greenNode;
			}
			ForStepClauseSyntax forStepClauseSyntax = new ForStepClauseSyntax(SyntaxKind.ForStepClause, stepKeyword, stepValue, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forStepClauseSyntax, hash);
			}
			return forStepClauseSyntax;
		}

		internal ForEachStatementSyntax ForEachStatement(KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression)
		{
			return new ForEachStatementSyntax(SyntaxKind.ForEachStatement, forKeyword, eachKeyword, controlVariable, inKeyword, expression, _factoryContext);
		}

		internal NextStatementSyntax NextStatement(KeywordSyntax nextKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> controlVariables)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(242, nextKeyword, controlVariables.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NextStatementSyntax)greenNode;
			}
			NextStatementSyntax nextStatementSyntax = new NextStatementSyntax(SyntaxKind.NextStatement, nextKeyword, controlVariables.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nextStatementSyntax, hash);
			}
			return nextStatementSyntax;
		}

		internal UsingStatementSyntax UsingStatement(KeywordSyntax usingKeyword, ExpressionSyntax expression, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(243, usingKeyword, expression, variables.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UsingStatementSyntax)greenNode;
			}
			UsingStatementSyntax usingStatementSyntax = new UsingStatementSyntax(SyntaxKind.UsingStatement, usingKeyword, expression, variables.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(usingStatementSyntax, hash);
			}
			return usingStatementSyntax;
		}

		internal ThrowStatementSyntax ThrowStatement(KeywordSyntax throwKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(246, throwKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ThrowStatementSyntax)greenNode;
			}
			ThrowStatementSyntax throwStatementSyntax = new ThrowStatementSyntax(SyntaxKind.ThrowStatement, throwKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(throwStatementSyntax, hash);
			}
			return throwStatementSyntax;
		}

		internal AssignmentStatementSyntax SimpleAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(247, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.SimpleAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax MidAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(248, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.MidAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax AddAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(249, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.AddAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax SubtractAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(250, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.SubtractAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax MultiplyAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(251, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.MultiplyAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax DivideAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(252, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.DivideAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax IntegerDivideAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(253, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.IntegerDivideAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax ExponentiateAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(254, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.ExponentiateAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax LeftShiftAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(255, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.LeftShiftAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax RightShiftAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(258, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.RightShiftAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax ConcatenateAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(259, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.ConcatenateAssignmentStatement, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal AssignmentStatementSyntax AssignmentStatement(SyntaxKind kind, ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(kind, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal MidExpressionSyntax MidExpression(IdentifierTokenSyntax mid, ArgumentListSyntax argumentList)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(260, mid, argumentList, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MidExpressionSyntax)greenNode;
			}
			MidExpressionSyntax midExpressionSyntax = new MidExpressionSyntax(SyntaxKind.MidExpression, mid, argumentList, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(midExpressionSyntax, hash);
			}
			return midExpressionSyntax;
		}

		internal CallStatementSyntax CallStatement(KeywordSyntax callKeyword, ExpressionSyntax invocation)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(261, callKeyword, invocation, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CallStatementSyntax)greenNode;
			}
			CallStatementSyntax callStatementSyntax = new CallStatementSyntax(SyntaxKind.CallStatement, callKeyword, invocation, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(callStatementSyntax, hash);
			}
			return callStatementSyntax;
		}

		internal AddRemoveHandlerStatementSyntax AddHandlerStatement(KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(SyntaxKind.AddHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, _factoryContext);
		}

		internal AddRemoveHandlerStatementSyntax RemoveHandlerStatement(KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(SyntaxKind.RemoveHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, _factoryContext);
		}

		internal AddRemoveHandlerStatementSyntax AddRemoveHandlerStatement(SyntaxKind kind, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, _factoryContext);
		}

		internal RaiseEventStatementSyntax RaiseEventStatement(KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(264, raiseEventKeyword, name, argumentList, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RaiseEventStatementSyntax)greenNode;
			}
			RaiseEventStatementSyntax raiseEventStatementSyntax = new RaiseEventStatementSyntax(SyntaxKind.RaiseEventStatement, raiseEventKeyword, name, argumentList, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(raiseEventStatementSyntax, hash);
			}
			return raiseEventStatementSyntax;
		}

		internal WithStatementSyntax WithStatement(KeywordSyntax withKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(265, withKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WithStatementSyntax)greenNode;
			}
			WithStatementSyntax withStatementSyntax = new WithStatementSyntax(SyntaxKind.WithStatement, withKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withStatementSyntax, hash);
			}
			return withStatementSyntax;
		}

		internal ReDimStatementSyntax ReDimStatement(KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> clauses)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(266, reDimKeyword, preserveKeyword, clauses.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ReDimStatementSyntax)greenNode;
			}
			ReDimStatementSyntax reDimStatementSyntax = new ReDimStatementSyntax(SyntaxKind.ReDimStatement, reDimKeyword, preserveKeyword, clauses.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(reDimStatementSyntax, hash);
			}
			return reDimStatementSyntax;
		}

		internal ReDimStatementSyntax ReDimPreserveStatement(KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> clauses)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(267, reDimKeyword, preserveKeyword, clauses.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ReDimStatementSyntax)greenNode;
			}
			ReDimStatementSyntax reDimStatementSyntax = new ReDimStatementSyntax(SyntaxKind.ReDimPreserveStatement, reDimKeyword, preserveKeyword, clauses.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(reDimStatementSyntax, hash);
			}
			return reDimStatementSyntax;
		}

		internal RedimClauseSyntax RedimClause(ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(270, expression, arrayBounds, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RedimClauseSyntax)greenNode;
			}
			RedimClauseSyntax redimClauseSyntax = new RedimClauseSyntax(SyntaxKind.RedimClause, expression, arrayBounds, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(redimClauseSyntax, hash);
			}
			return redimClauseSyntax;
		}

		internal EraseStatementSyntax EraseStatement(KeywordSyntax eraseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> expressions)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(271, eraseKeyword, expressions.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (EraseStatementSyntax)greenNode;
			}
			EraseStatementSyntax eraseStatementSyntax = new EraseStatementSyntax(SyntaxKind.EraseStatement, eraseKeyword, expressions.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(eraseStatementSyntax, hash);
			}
			return eraseStatementSyntax;
		}

		internal LiteralExpressionSyntax CharacterLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(272, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.CharacterLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax TrueLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(273, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax FalseLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(274, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax NumericLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(275, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.NumericLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax DateLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(276, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.DateLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax StringLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(279, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.StringLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax NothingLiteralExpression(SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(280, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.NothingLiteralExpression, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal LiteralExpressionSyntax LiteralExpression(SyntaxKind kind, SyntaxToken token)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, token, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(kind, token, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal ParenthesizedExpressionSyntax ParenthesizedExpression(PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(281, openParenToken, expression, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ParenthesizedExpressionSyntax)greenNode;
			}
			ParenthesizedExpressionSyntax parenthesizedExpressionSyntax = new ParenthesizedExpressionSyntax(SyntaxKind.ParenthesizedExpression, openParenToken, expression, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(parenthesizedExpressionSyntax, hash);
			}
			return parenthesizedExpressionSyntax;
		}

		internal TupleExpressionSyntax TupleExpression(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(788, openParenToken, arguments.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TupleExpressionSyntax)greenNode;
			}
			TupleExpressionSyntax tupleExpressionSyntax = new TupleExpressionSyntax(SyntaxKind.TupleExpression, openParenToken, arguments.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tupleExpressionSyntax, hash);
			}
			return tupleExpressionSyntax;
		}

		internal TupleTypeSyntax TupleType(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> elements, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(789, openParenToken, elements.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TupleTypeSyntax)greenNode;
			}
			TupleTypeSyntax tupleTypeSyntax = new TupleTypeSyntax(SyntaxKind.TupleType, openParenToken, elements.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tupleTypeSyntax, hash);
			}
			return tupleTypeSyntax;
		}

		internal TypedTupleElementSyntax TypedTupleElement(TypeSyntax type)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(790, type, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (TypedTupleElementSyntax)greenNode;
			}
			TypedTupleElementSyntax typedTupleElementSyntax = new TypedTupleElementSyntax(SyntaxKind.TypedTupleElement, type, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typedTupleElementSyntax, hash);
			}
			return typedTupleElementSyntax;
		}

		internal NamedTupleElementSyntax NamedTupleElement(IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(791, identifier, asClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NamedTupleElementSyntax)greenNode;
			}
			NamedTupleElementSyntax namedTupleElementSyntax = new NamedTupleElementSyntax(SyntaxKind.NamedTupleElement, identifier, asClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namedTupleElementSyntax, hash);
			}
			return namedTupleElementSyntax;
		}

		internal MeExpressionSyntax MeExpression(KeywordSyntax keyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(282, keyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MeExpressionSyntax)greenNode;
			}
			MeExpressionSyntax meExpressionSyntax = new MeExpressionSyntax(SyntaxKind.MeExpression, keyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(meExpressionSyntax, hash);
			}
			return meExpressionSyntax;
		}

		internal MyBaseExpressionSyntax MyBaseExpression(KeywordSyntax keyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(283, keyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MyBaseExpressionSyntax)greenNode;
			}
			MyBaseExpressionSyntax myBaseExpressionSyntax = new MyBaseExpressionSyntax(SyntaxKind.MyBaseExpression, keyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(myBaseExpressionSyntax, hash);
			}
			return myBaseExpressionSyntax;
		}

		internal MyClassExpressionSyntax MyClassExpression(KeywordSyntax keyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(284, keyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MyClassExpressionSyntax)greenNode;
			}
			MyClassExpressionSyntax myClassExpressionSyntax = new MyClassExpressionSyntax(SyntaxKind.MyClassExpression, keyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(myClassExpressionSyntax, hash);
			}
			return myClassExpressionSyntax;
		}

		internal GetTypeExpressionSyntax GetTypeExpression(KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new GetTypeExpressionSyntax(SyntaxKind.GetTypeExpression, getTypeKeyword, openParenToken, type, closeParenToken, _factoryContext);
		}

		internal TypeOfExpressionSyntax TypeOfIsExpression(KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(SyntaxKind.TypeOfIsExpression, typeOfKeyword, expression, operatorToken, type, _factoryContext);
		}

		internal TypeOfExpressionSyntax TypeOfIsNotExpression(KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(SyntaxKind.TypeOfIsNotExpression, typeOfKeyword, expression, operatorToken, type, _factoryContext);
		}

		internal TypeOfExpressionSyntax TypeOfExpression(SyntaxKind kind, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(kind, typeOfKeyword, expression, operatorToken, type, _factoryContext);
		}

		internal GetXmlNamespaceExpressionSyntax GetXmlNamespaceExpression(KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken)
		{
			return new GetXmlNamespaceExpressionSyntax(SyntaxKind.GetXmlNamespaceExpression, getXmlNamespaceKeyword, openParenToken, name, closeParenToken, _factoryContext);
		}

		internal MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(291, expression, operatorToken, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal MemberAccessExpressionSyntax DictionaryAccessExpression(ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(292, expression, operatorToken, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(SyntaxKind.DictionaryAccessExpression, expression, operatorToken, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, expression, operatorToken, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(kind, expression, operatorToken, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal XmlMemberAccessExpressionSyntax XmlElementAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlElementAccessExpression, @base, token1, token2, token3, name, _factoryContext);
		}

		internal XmlMemberAccessExpressionSyntax XmlDescendantAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlDescendantAccessExpression, @base, token1, token2, token3, name, _factoryContext);
		}

		internal XmlMemberAccessExpressionSyntax XmlAttributeAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlAttributeAccessExpression, @base, token1, token2, token3, name, _factoryContext);
		}

		internal XmlMemberAccessExpressionSyntax XmlMemberAccessExpression(SyntaxKind kind, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(kind, @base, token1, token2, token3, name, _factoryContext);
		}

		internal InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, ArgumentListSyntax argumentList)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(296, expression, argumentList, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InvocationExpressionSyntax)greenNode;
			}
			InvocationExpressionSyntax invocationExpressionSyntax = new InvocationExpressionSyntax(SyntaxKind.InvocationExpression, expression, argumentList, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(invocationExpressionSyntax, hash);
			}
			return invocationExpressionSyntax;
		}

		internal ObjectCreationExpressionSyntax ObjectCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
		{
			return new ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, newKeyword, attributeLists.Node, type, argumentList, initializer, _factoryContext);
		}

		internal AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, ObjectMemberInitializerSyntax initializer)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(298, newKeyword, attributeLists.Node, initializer, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AnonymousObjectCreationExpressionSyntax)greenNode;
			}
			AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax = new AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, newKeyword, attributeLists.Node, initializer, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(anonymousObjectCreationExpressionSyntax, hash);
			}
			return anonymousObjectCreationExpressionSyntax;
		}

		internal ArrayCreationExpressionSyntax ArrayCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> rankSpecifiers, CollectionInitializerSyntax initializer)
		{
			return new ArrayCreationExpressionSyntax(SyntaxKind.ArrayCreationExpression, newKeyword, attributeLists.Node, type, arrayBounds, rankSpecifiers.Node, initializer, _factoryContext);
		}

		internal CollectionInitializerSyntax CollectionInitializer(PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> initializers, PunctuationSyntax closeBraceToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(302, openBraceToken, initializers.Node, closeBraceToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CollectionInitializerSyntax)greenNode;
			}
			CollectionInitializerSyntax collectionInitializerSyntax = new CollectionInitializerSyntax(SyntaxKind.CollectionInitializer, openBraceToken, initializers.Node, closeBraceToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(collectionInitializerSyntax, hash);
			}
			return collectionInitializerSyntax;
		}

		internal CTypeExpressionSyntax CTypeExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new CTypeExpressionSyntax(SyntaxKind.CTypeExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, _factoryContext);
		}

		internal DirectCastExpressionSyntax DirectCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new DirectCastExpressionSyntax(SyntaxKind.DirectCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, _factoryContext);
		}

		internal TryCastExpressionSyntax TryCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new TryCastExpressionSyntax(SyntaxKind.TryCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, _factoryContext);
		}

		internal PredefinedCastExpressionSyntax PredefinedCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
		{
			return new PredefinedCastExpressionSyntax(SyntaxKind.PredefinedCastExpression, keyword, openParenToken, expression, closeParenToken, _factoryContext);
		}

		internal BinaryExpressionSyntax AddExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(307, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AddExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax SubtractExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(308, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.SubtractExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax MultiplyExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(309, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.MultiplyExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax DivideExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(310, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.DivideExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax IntegerDivideExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(311, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IntegerDivideExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax ExponentiateExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(314, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ExponentiateExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax LeftShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(315, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LeftShiftExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax RightShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(316, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.RightShiftExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax ConcatenateExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(317, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ConcatenateExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax ModuloExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(318, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ModuloExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(319, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.EqualsExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(320, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.NotEqualsExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax LessThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(321, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LessThanExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax LessThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(322, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LessThanOrEqualExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax GreaterThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(323, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.GreaterThanOrEqualExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax GreaterThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(324, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.GreaterThanExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax IsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(325, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IsExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax IsNotExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(326, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IsNotExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax LikeExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(327, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LikeExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax OrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(328, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.OrExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax ExclusiveOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(329, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ExclusiveOrExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax AndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(330, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AndExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax OrElseExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(331, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.OrElseExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax AndAlsoExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(332, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AndAlsoExpression, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, left, operatorToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(kind, left, operatorToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal UnaryExpressionSyntax UnaryPlusExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(333, operatorToken, operand, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.UnaryPlusExpression, operatorToken, operand, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal UnaryExpressionSyntax UnaryMinusExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(334, operatorToken, operand, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.UnaryMinusExpression, operatorToken, operand, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal UnaryExpressionSyntax NotExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(335, operatorToken, operand, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.NotExpression, operatorToken, operand, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal UnaryExpressionSyntax AddressOfExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(336, operatorToken, operand, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.AddressOfExpression, operatorToken, operand, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal UnaryExpressionSyntax UnaryExpression(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, operatorToken, operand, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(kind, operatorToken, operand, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal BinaryConditionalExpressionSyntax BinaryConditionalExpression(KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken)
		{
			return new BinaryConditionalExpressionSyntax(SyntaxKind.BinaryConditionalExpression, ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken, _factoryContext);
		}

		internal TernaryConditionalExpressionSyntax TernaryConditionalExpression(KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken)
		{
			return new TernaryConditionalExpressionSyntax(SyntaxKind.TernaryConditionalExpression, ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken, _factoryContext);
		}

		internal SingleLineLambdaExpressionSyntax SingleLineFunctionLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(339, subOrFunctionHeader, body, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineFunctionLambdaExpression, subOrFunctionHeader, body, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal SingleLineLambdaExpressionSyntax SingleLineSubLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(342, subOrFunctionHeader, body, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineSubLambdaExpression, subOrFunctionHeader, body, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal SingleLineLambdaExpressionSyntax SingleLineLambdaExpression(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, subOrFunctionHeader, body, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(kind, subOrFunctionHeader, body, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal MultiLineLambdaExpressionSyntax MultiLineFunctionLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(343, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineFunctionLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal MultiLineLambdaExpressionSyntax MultiLineSubLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(344, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineSubLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal MultiLineLambdaExpressionSyntax MultiLineLambdaExpression(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal LambdaHeaderSyntax SubLambdaHeader(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(SyntaxKind.SubLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, _factoryContext);
		}

		internal LambdaHeaderSyntax FunctionLambdaHeader(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(SyntaxKind.FunctionLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, _factoryContext);
		}

		internal LambdaHeaderSyntax LambdaHeader(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, _factoryContext);
		}

		internal ArgumentListSyntax ArgumentList(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(347, openParenToken, arguments.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ArgumentListSyntax)greenNode;
			}
			ArgumentListSyntax argumentListSyntax = new ArgumentListSyntax(SyntaxKind.ArgumentList, openParenToken, arguments.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(argumentListSyntax, hash);
			}
			return argumentListSyntax;
		}

		internal OmittedArgumentSyntax OmittedArgument(PunctuationSyntax empty)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(348, empty, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OmittedArgumentSyntax)greenNode;
			}
			OmittedArgumentSyntax omittedArgumentSyntax = new OmittedArgumentSyntax(SyntaxKind.OmittedArgument, empty, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(omittedArgumentSyntax, hash);
			}
			return omittedArgumentSyntax;
		}

		internal SimpleArgumentSyntax SimpleArgument(NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(349, nameColonEquals, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SimpleArgumentSyntax)greenNode;
			}
			SimpleArgumentSyntax simpleArgumentSyntax = new SimpleArgumentSyntax(SyntaxKind.SimpleArgument, nameColonEquals, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleArgumentSyntax, hash);
			}
			return simpleArgumentSyntax;
		}

		internal NameColonEqualsSyntax NameColonEquals(IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(755, name, colonEqualsToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NameColonEqualsSyntax)greenNode;
			}
			NameColonEqualsSyntax nameColonEqualsSyntax = new NameColonEqualsSyntax(SyntaxKind.NameColonEquals, name, colonEqualsToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nameColonEqualsSyntax, hash);
			}
			return nameColonEqualsSyntax;
		}

		internal RangeArgumentSyntax RangeArgument(ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(351, lowerBound, toKeyword, upperBound, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (RangeArgumentSyntax)greenNode;
			}
			RangeArgumentSyntax rangeArgumentSyntax = new RangeArgumentSyntax(SyntaxKind.RangeArgument, lowerBound, toKeyword, upperBound, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(rangeArgumentSyntax, hash);
			}
			return rangeArgumentSyntax;
		}

		internal QueryExpressionSyntax QueryExpression(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> clauses)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(352, clauses.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (QueryExpressionSyntax)greenNode;
			}
			QueryExpressionSyntax queryExpressionSyntax = new QueryExpressionSyntax(SyntaxKind.QueryExpression, clauses.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(queryExpressionSyntax, hash);
			}
			return queryExpressionSyntax;
		}

		internal CollectionRangeVariableSyntax CollectionRangeVariable(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression)
		{
			return new CollectionRangeVariableSyntax(SyntaxKind.CollectionRangeVariable, identifier, asClause, inKeyword, expression, _factoryContext);
		}

		internal ExpressionRangeVariableSyntax ExpressionRangeVariable(VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(354, nameEquals, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ExpressionRangeVariableSyntax)greenNode;
			}
			ExpressionRangeVariableSyntax expressionRangeVariableSyntax = new ExpressionRangeVariableSyntax(SyntaxKind.ExpressionRangeVariable, nameEquals, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(expressionRangeVariableSyntax, hash);
			}
			return expressionRangeVariableSyntax;
		}

		internal AggregationRangeVariableSyntax AggregationRangeVariable(VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(355, nameEquals, aggregation, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AggregationRangeVariableSyntax)greenNode;
			}
			AggregationRangeVariableSyntax aggregationRangeVariableSyntax = new AggregationRangeVariableSyntax(SyntaxKind.AggregationRangeVariable, nameEquals, aggregation, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(aggregationRangeVariableSyntax, hash);
			}
			return aggregationRangeVariableSyntax;
		}

		internal VariableNameEqualsSyntax VariableNameEquals(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(356, identifier, asClause, equalsToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (VariableNameEqualsSyntax)greenNode;
			}
			VariableNameEqualsSyntax variableNameEqualsSyntax = new VariableNameEqualsSyntax(SyntaxKind.VariableNameEquals, identifier, asClause, equalsToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(variableNameEqualsSyntax, hash);
			}
			return variableNameEqualsSyntax;
		}

		internal FunctionAggregationSyntax FunctionAggregation(IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
		{
			return new FunctionAggregationSyntax(SyntaxKind.FunctionAggregation, functionName, openParenToken, argument, closeParenToken, _factoryContext);
		}

		internal GroupAggregationSyntax GroupAggregation(KeywordSyntax groupKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(358, groupKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (GroupAggregationSyntax)greenNode;
			}
			GroupAggregationSyntax groupAggregationSyntax = new GroupAggregationSyntax(SyntaxKind.GroupAggregation, groupKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(groupAggregationSyntax, hash);
			}
			return groupAggregationSyntax;
		}

		internal FromClauseSyntax FromClause(KeywordSyntax fromKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(359, fromKeyword, variables.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (FromClauseSyntax)greenNode;
			}
			FromClauseSyntax fromClauseSyntax = new FromClauseSyntax(SyntaxKind.FromClause, fromKeyword, variables.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(fromClauseSyntax, hash);
			}
			return fromClauseSyntax;
		}

		internal LetClauseSyntax LetClause(KeywordSyntax letKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(360, letKeyword, variables.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (LetClauseSyntax)greenNode;
			}
			LetClauseSyntax letClauseSyntax = new LetClauseSyntax(SyntaxKind.LetClause, letKeyword, variables.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(letClauseSyntax, hash);
			}
			return letClauseSyntax;
		}

		internal AggregateClauseSyntax AggregateClause(KeywordSyntax aggregateKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalQueryOperators, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new AggregateClauseSyntax(SyntaxKind.AggregateClause, aggregateKeyword, variables.Node, additionalQueryOperators.Node, intoKeyword, aggregationVariables.Node, _factoryContext);
		}

		internal DistinctClauseSyntax DistinctClause(KeywordSyntax distinctKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(362, distinctKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (DistinctClauseSyntax)greenNode;
			}
			DistinctClauseSyntax distinctClauseSyntax = new DistinctClauseSyntax(SyntaxKind.DistinctClause, distinctKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(distinctClauseSyntax, hash);
			}
			return distinctClauseSyntax;
		}

		internal WhereClauseSyntax WhereClause(KeywordSyntax whereKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(363, whereKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (WhereClauseSyntax)greenNode;
			}
			WhereClauseSyntax whereClauseSyntax = new WhereClauseSyntax(SyntaxKind.WhereClause, whereKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whereClauseSyntax, hash);
			}
			return whereClauseSyntax;
		}

		internal PartitionWhileClauseSyntax SkipWhileClause(KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(364, skipOrTakeKeyword, whileKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(SyntaxKind.SkipWhileClause, skipOrTakeKeyword, whileKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal PartitionWhileClauseSyntax TakeWhileClause(KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(365, skipOrTakeKeyword, whileKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(SyntaxKind.TakeWhileClause, skipOrTakeKeyword, whileKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal PartitionWhileClauseSyntax PartitionWhileClause(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, skipOrTakeKeyword, whileKeyword, condition, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(kind, skipOrTakeKeyword, whileKeyword, condition, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal PartitionClauseSyntax SkipClause(KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(366, skipOrTakeKeyword, count, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(SyntaxKind.SkipClause, skipOrTakeKeyword, count, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal PartitionClauseSyntax TakeClause(KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(367, skipOrTakeKeyword, count, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(SyntaxKind.TakeClause, skipOrTakeKeyword, count, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal PartitionClauseSyntax PartitionClause(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, skipOrTakeKeyword, count, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(kind, skipOrTakeKeyword, count, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal GroupByClauseSyntax GroupByClause(KeywordSyntax groupKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> items, KeywordSyntax byKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> keys, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new GroupByClauseSyntax(SyntaxKind.GroupByClause, groupKeyword, items.Node, byKeyword, keys.Node, intoKeyword, aggregationVariables.Node, _factoryContext);
		}

		internal JoinConditionSyntax JoinCondition(ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(369, left, equalsKeyword, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (JoinConditionSyntax)greenNode;
			}
			JoinConditionSyntax joinConditionSyntax = new JoinConditionSyntax(SyntaxKind.JoinCondition, left, equalsKeyword, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(joinConditionSyntax, hash);
			}
			return joinConditionSyntax;
		}

		internal SimpleJoinClauseSyntax SimpleJoinClause(KeywordSyntax joinKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinedVariables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalJoins, KeywordSyntax onKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinConditions)
		{
			return new SimpleJoinClauseSyntax(SyntaxKind.SimpleJoinClause, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, _factoryContext);
		}

		internal GroupJoinClauseSyntax GroupJoinClause(KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinedVariables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalJoins, KeywordSyntax onKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinConditions, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new GroupJoinClauseSyntax(SyntaxKind.GroupJoinClause, groupKeyword, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, intoKeyword, aggregationVariables.Node, _factoryContext);
		}

		internal OrderByClauseSyntax OrderByClause(KeywordSyntax orderKeyword, KeywordSyntax byKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> orderings)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(372, orderKeyword, byKeyword, orderings.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OrderByClauseSyntax)greenNode;
			}
			OrderByClauseSyntax orderByClauseSyntax = new OrderByClauseSyntax(SyntaxKind.OrderByClause, orderKeyword, byKeyword, orderings.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderByClauseSyntax, hash);
			}
			return orderByClauseSyntax;
		}

		internal OrderingSyntax AscendingOrdering(ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(375, expression, ascendingOrDescendingKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(SyntaxKind.AscendingOrdering, expression, ascendingOrDescendingKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal OrderingSyntax DescendingOrdering(ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(376, expression, ascendingOrDescendingKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(SyntaxKind.DescendingOrdering, expression, ascendingOrDescendingKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal OrderingSyntax Ordering(SyntaxKind kind, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode((int)kind, expression, ascendingOrDescendingKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(kind, expression, ascendingOrDescendingKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal SelectClauseSyntax SelectClause(KeywordSyntax selectKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(377, selectKeyword, variables.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (SelectClauseSyntax)greenNode;
			}
			SelectClauseSyntax selectClauseSyntax = new SelectClauseSyntax(SyntaxKind.SelectClause, selectKeyword, variables.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectClauseSyntax, hash);
			}
			return selectClauseSyntax;
		}

		internal XmlDocumentSyntax XmlDocument(XmlDeclarationSyntax declaration, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> precedingMisc, XmlNodeSyntax root, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> followingMisc)
		{
			return new XmlDocumentSyntax(SyntaxKind.XmlDocument, declaration, precedingMisc.Node, root, followingMisc.Node, _factoryContext);
		}

		internal XmlDeclarationSyntax XmlDeclaration(PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken)
		{
			return new XmlDeclarationSyntax(SyntaxKind.XmlDeclaration, lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken, _factoryContext);
		}

		internal XmlDeclarationOptionSyntax XmlDeclarationOption(XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(380, name, equals, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlDeclarationOptionSyntax)greenNode;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = new XmlDeclarationOptionSyntax(SyntaxKind.XmlDeclarationOption, name, equals, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlDeclarationOptionSyntax, hash);
			}
			return xmlDeclarationOptionSyntax;
		}

		internal XmlElementSyntax XmlElement(XmlElementStartTagSyntax startTag, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> content, XmlElementEndTagSyntax endTag)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(381, startTag, content.Node, endTag, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlElementSyntax)greenNode;
			}
			XmlElementSyntax xmlElementSyntax = new XmlElementSyntax(SyntaxKind.XmlElement, startTag, content.Node, endTag, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlElementSyntax, hash);
			}
			return xmlElementSyntax;
		}

		internal XmlTextSyntax XmlText(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(382, textTokens.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlTextSyntax)greenNode;
			}
			XmlTextSyntax xmlTextSyntax = new XmlTextSyntax(SyntaxKind.XmlText, textTokens.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlTextSyntax, hash);
			}
			return xmlTextSyntax;
		}

		internal XmlElementStartTagSyntax XmlElementStartTag(PunctuationSyntax lessThanToken, XmlNodeSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, PunctuationSyntax greaterThanToken)
		{
			return new XmlElementStartTagSyntax(SyntaxKind.XmlElementStartTag, lessThanToken, name, attributes.Node, greaterThanToken, _factoryContext);
		}

		internal XmlElementEndTagSyntax XmlElementEndTag(PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(384, lessThanSlashToken, name, greaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlElementEndTagSyntax)greenNode;
			}
			XmlElementEndTagSyntax xmlElementEndTagSyntax = new XmlElementEndTagSyntax(SyntaxKind.XmlElementEndTag, lessThanSlashToken, name, greaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlElementEndTagSyntax, hash);
			}
			return xmlElementEndTagSyntax;
		}

		internal XmlEmptyElementSyntax XmlEmptyElement(PunctuationSyntax lessThanToken, XmlNodeSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, PunctuationSyntax slashGreaterThanToken)
		{
			return new XmlEmptyElementSyntax(SyntaxKind.XmlEmptyElement, lessThanToken, name, attributes.Node, slashGreaterThanToken, _factoryContext);
		}

		internal XmlAttributeSyntax XmlAttribute(XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(386, name, equalsToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlAttributeSyntax)greenNode;
			}
			XmlAttributeSyntax xmlAttributeSyntax = new XmlAttributeSyntax(SyntaxKind.XmlAttribute, name, equalsToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlAttributeSyntax, hash);
			}
			return xmlAttributeSyntax;
		}

		internal XmlStringSyntax XmlString(PunctuationSyntax startQuoteToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax endQuoteToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(387, startQuoteToken, textTokens.Node, endQuoteToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlStringSyntax)greenNode;
			}
			XmlStringSyntax xmlStringSyntax = new XmlStringSyntax(SyntaxKind.XmlString, startQuoteToken, textTokens.Node, endQuoteToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlStringSyntax, hash);
			}
			return xmlStringSyntax;
		}

		internal XmlPrefixNameSyntax XmlPrefixName(XmlNameTokenSyntax name)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(388, name, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlPrefixNameSyntax)greenNode;
			}
			XmlPrefixNameSyntax xmlPrefixNameSyntax = new XmlPrefixNameSyntax(SyntaxKind.XmlPrefixName, name, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlPrefixNameSyntax, hash);
			}
			return xmlPrefixNameSyntax;
		}

		internal XmlNameSyntax XmlName(XmlPrefixSyntax prefix, XmlNameTokenSyntax localName)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(389, prefix, localName, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlNameSyntax)greenNode;
			}
			XmlNameSyntax xmlNameSyntax = new XmlNameSyntax(SyntaxKind.XmlName, prefix, localName, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlNameSyntax, hash);
			}
			return xmlNameSyntax;
		}

		internal XmlBracketedNameSyntax XmlBracketedName(PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(390, lessThanToken, name, greaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlBracketedNameSyntax)greenNode;
			}
			XmlBracketedNameSyntax xmlBracketedNameSyntax = new XmlBracketedNameSyntax(SyntaxKind.XmlBracketedName, lessThanToken, name, greaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlBracketedNameSyntax, hash);
			}
			return xmlBracketedNameSyntax;
		}

		internal XmlPrefixSyntax XmlPrefix(XmlNameTokenSyntax name, PunctuationSyntax colonToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(391, name, colonToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlPrefixSyntax)greenNode;
			}
			XmlPrefixSyntax xmlPrefixSyntax = new XmlPrefixSyntax(SyntaxKind.XmlPrefix, name, colonToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlPrefixSyntax, hash);
			}
			return xmlPrefixSyntax;
		}

		internal XmlCommentSyntax XmlComment(PunctuationSyntax lessThanExclamationMinusMinusToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax minusMinusGreaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(392, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlCommentSyntax)greenNode;
			}
			XmlCommentSyntax xmlCommentSyntax = new XmlCommentSyntax(SyntaxKind.XmlComment, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlCommentSyntax, hash);
			}
			return xmlCommentSyntax;
		}

		internal XmlProcessingInstructionSyntax XmlProcessingInstruction(PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax questionGreaterThanToken)
		{
			return new XmlProcessingInstructionSyntax(SyntaxKind.XmlProcessingInstruction, lessThanQuestionToken, name, textTokens.Node, questionGreaterThanToken, _factoryContext);
		}

		internal XmlCDataSectionSyntax XmlCDataSection(PunctuationSyntax beginCDataToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax endCDataToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(394, beginCDataToken, textTokens.Node, endCDataToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlCDataSectionSyntax)greenNode;
			}
			XmlCDataSectionSyntax xmlCDataSectionSyntax = new XmlCDataSectionSyntax(SyntaxKind.XmlCDataSection, beginCDataToken, textTokens.Node, endCDataToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlCDataSectionSyntax, hash);
			}
			return xmlCDataSectionSyntax;
		}

		internal XmlEmbeddedExpressionSyntax XmlEmbeddedExpression(PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(395, lessThanPercentEqualsToken, expression, percentGreaterThanToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (XmlEmbeddedExpressionSyntax)greenNode;
			}
			XmlEmbeddedExpressionSyntax xmlEmbeddedExpressionSyntax = new XmlEmbeddedExpressionSyntax(SyntaxKind.XmlEmbeddedExpression, lessThanPercentEqualsToken, expression, percentGreaterThanToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlEmbeddedExpressionSyntax, hash);
			}
			return xmlEmbeddedExpressionSyntax;
		}

		internal ArrayTypeSyntax ArrayType(TypeSyntax elementType, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> rankSpecifiers)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(396, elementType, rankSpecifiers.Node, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ArrayTypeSyntax)greenNode;
			}
			ArrayTypeSyntax arrayTypeSyntax = new ArrayTypeSyntax(SyntaxKind.ArrayType, elementType, rankSpecifiers.Node, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(arrayTypeSyntax, hash);
			}
			return arrayTypeSyntax;
		}

		internal NullableTypeSyntax NullableType(TypeSyntax elementType, PunctuationSyntax questionMarkToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(397, elementType, questionMarkToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (NullableTypeSyntax)greenNode;
			}
			NullableTypeSyntax nullableTypeSyntax = new NullableTypeSyntax(SyntaxKind.NullableType, elementType, questionMarkToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nullableTypeSyntax, hash);
			}
			return nullableTypeSyntax;
		}

		internal PredefinedTypeSyntax PredefinedType(KeywordSyntax keyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(398, keyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (PredefinedTypeSyntax)greenNode;
			}
			PredefinedTypeSyntax predefinedTypeSyntax = new PredefinedTypeSyntax(SyntaxKind.PredefinedType, keyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(predefinedTypeSyntax, hash);
			}
			return predefinedTypeSyntax;
		}

		internal IdentifierNameSyntax IdentifierName(IdentifierTokenSyntax identifier)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(399, identifier, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (IdentifierNameSyntax)greenNode;
			}
			IdentifierNameSyntax identifierNameSyntax = new IdentifierNameSyntax(SyntaxKind.IdentifierName, identifier, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(identifierNameSyntax, hash);
			}
			return identifierNameSyntax;
		}

		internal GenericNameSyntax GenericName(IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(400, identifier, typeArgumentList, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (GenericNameSyntax)greenNode;
			}
			GenericNameSyntax genericNameSyntax = new GenericNameSyntax(SyntaxKind.GenericName, identifier, typeArgumentList, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(genericNameSyntax, hash);
			}
			return genericNameSyntax;
		}

		internal QualifiedNameSyntax QualifiedName(NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(401, left, dotToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (QualifiedNameSyntax)greenNode;
			}
			QualifiedNameSyntax qualifiedNameSyntax = new QualifiedNameSyntax(SyntaxKind.QualifiedName, left, dotToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(qualifiedNameSyntax, hash);
			}
			return qualifiedNameSyntax;
		}

		internal GlobalNameSyntax GlobalName(KeywordSyntax globalKeyword)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(402, globalKeyword, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (GlobalNameSyntax)greenNode;
			}
			GlobalNameSyntax globalNameSyntax = new GlobalNameSyntax(SyntaxKind.GlobalName, globalKeyword, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(globalNameSyntax, hash);
			}
			return globalNameSyntax;
		}

		internal TypeArgumentListSyntax TypeArgumentList(PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			return new TypeArgumentListSyntax(SyntaxKind.TypeArgumentList, openParenToken, ofKeyword, arguments.Node, closeParenToken, _factoryContext);
		}

		internal CrefReferenceSyntax CrefReference(TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(404, name, signature, asClause, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CrefReferenceSyntax)greenNode;
			}
			CrefReferenceSyntax crefReferenceSyntax = new CrefReferenceSyntax(SyntaxKind.CrefReference, name, signature, asClause, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefReferenceSyntax, hash);
			}
			return crefReferenceSyntax;
		}

		internal CrefSignatureSyntax CrefSignature(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> argumentTypes, PunctuationSyntax closeParenToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(407, openParenToken, argumentTypes.Node, closeParenToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CrefSignatureSyntax)greenNode;
			}
			CrefSignatureSyntax crefSignatureSyntax = new CrefSignatureSyntax(SyntaxKind.CrefSignature, openParenToken, argumentTypes.Node, closeParenToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefSignatureSyntax, hash);
			}
			return crefSignatureSyntax;
		}

		internal CrefSignaturePartSyntax CrefSignaturePart(KeywordSyntax modifier, TypeSyntax type)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(408, modifier, type, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CrefSignaturePartSyntax)greenNode;
			}
			CrefSignaturePartSyntax crefSignaturePartSyntax = new CrefSignaturePartSyntax(SyntaxKind.CrefSignaturePart, modifier, type, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefSignaturePartSyntax, hash);
			}
			return crefSignaturePartSyntax;
		}

		internal CrefOperatorReferenceSyntax CrefOperatorReference(KeywordSyntax operatorKeyword, SyntaxToken operatorToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(409, operatorKeyword, operatorToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (CrefOperatorReferenceSyntax)greenNode;
			}
			CrefOperatorReferenceSyntax crefOperatorReferenceSyntax = new CrefOperatorReferenceSyntax(SyntaxKind.CrefOperatorReference, operatorKeyword, operatorToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefOperatorReferenceSyntax, hash);
			}
			return crefOperatorReferenceSyntax;
		}

		internal QualifiedCrefOperatorReferenceSyntax QualifiedCrefOperatorReference(NameSyntax left, PunctuationSyntax dotToken, CrefOperatorReferenceSyntax right)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(410, left, dotToken, right, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (QualifiedCrefOperatorReferenceSyntax)greenNode;
			}
			QualifiedCrefOperatorReferenceSyntax qualifiedCrefOperatorReferenceSyntax = new QualifiedCrefOperatorReferenceSyntax(SyntaxKind.QualifiedCrefOperatorReference, left, dotToken, right, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(qualifiedCrefOperatorReferenceSyntax, hash);
			}
			return qualifiedCrefOperatorReferenceSyntax;
		}

		internal YieldStatementSyntax YieldStatement(KeywordSyntax yieldKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(411, yieldKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (YieldStatementSyntax)greenNode;
			}
			YieldStatementSyntax yieldStatementSyntax = new YieldStatementSyntax(SyntaxKind.YieldStatement, yieldKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(yieldStatementSyntax, hash);
			}
			return yieldStatementSyntax;
		}

		internal AwaitExpressionSyntax AwaitExpression(KeywordSyntax awaitKeyword, ExpressionSyntax expression)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(412, awaitKeyword, expression, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (AwaitExpressionSyntax)greenNode;
			}
			AwaitExpressionSyntax awaitExpressionSyntax = new AwaitExpressionSyntax(SyntaxKind.AwaitExpression, awaitKeyword, expression, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(awaitExpressionSyntax, hash);
			}
			return awaitExpressionSyntax;
		}

		internal XmlNameTokenSyntax XmlNameToken(string text, SyntaxKind possibleKeywordKind, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlNameTokenSyntax(SyntaxKind.XmlNameToken, text, leadingTrivia, trailingTrivia, possibleKeywordKind, _factoryContext);
		}

		internal XmlTextTokenSyntax XmlTextLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.XmlTextLiteralToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal XmlTextTokenSyntax XmlEntityLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.XmlEntityLiteralToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal XmlTextTokenSyntax DocumentationCommentLineBreakToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.DocumentationCommentLineBreakToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal XmlTextTokenSyntax XmlTextToken(SyntaxKind kind, string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(kind, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal InterpolatedStringTextTokenSyntax InterpolatedStringTextToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new InterpolatedStringTextTokenSyntax(SyntaxKind.InterpolatedStringTextToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal DecimalLiteralTokenSyntax DecimalLiteralToken(string text, TypeCharacter typeSuffix, decimal value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new DecimalLiteralTokenSyntax(SyntaxKind.DecimalLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value, _factoryContext);
		}

		internal DateLiteralTokenSyntax DateLiteralToken(string text, DateTime value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new DateLiteralTokenSyntax(SyntaxKind.DateLiteralToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal StringLiteralTokenSyntax StringLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new StringLiteralTokenSyntax(SyntaxKind.StringLiteralToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal CharacterLiteralTokenSyntax CharacterLiteralToken(string text, char value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new CharacterLiteralTokenSyntax(SyntaxKind.CharacterLiteralToken, text, leadingTrivia, trailingTrivia, value, _factoryContext);
		}

		internal SkippedTokensTriviaSyntax SkippedTokensTrivia(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> tokens)
		{
			return new SkippedTokensTriviaSyntax(SyntaxKind.SkippedTokensTrivia, tokens.Node, _factoryContext);
		}

		internal DocumentationCommentTriviaSyntax DocumentationCommentTrivia(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> content)
		{
			return new DocumentationCommentTriviaSyntax(SyntaxKind.DocumentationCommentTrivia, content.Node, _factoryContext);
		}

		internal XmlCrefAttributeSyntax XmlCrefAttribute(XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, CrefReferenceSyntax reference, PunctuationSyntax endQuoteToken)
		{
			return new XmlCrefAttributeSyntax(SyntaxKind.XmlCrefAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken, _factoryContext);
		}

		internal XmlNameAttributeSyntax XmlNameAttribute(XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken)
		{
			return new XmlNameAttributeSyntax(SyntaxKind.XmlNameAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken, _factoryContext);
		}

		internal ConditionalAccessExpressionSyntax ConditionalAccessExpression(ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(713, expression, questionMarkToken, whenNotNull, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (ConditionalAccessExpressionSyntax)greenNode;
			}
			ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = new ConditionalAccessExpressionSyntax(SyntaxKind.ConditionalAccessExpression, expression, questionMarkToken, whenNotNull, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(conditionalAccessExpressionSyntax, hash);
			}
			return conditionalAccessExpressionSyntax;
		}

		internal NameOfExpressionSyntax NameOfExpression(KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
		{
			return new NameOfExpressionSyntax(SyntaxKind.NameOfExpression, nameOfKeyword, openParenToken, argument, closeParenToken, _factoryContext);
		}

		internal InterpolatedStringExpressionSyntax InterpolatedStringExpression(PunctuationSyntax dollarSignDoubleQuoteToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> contents, PunctuationSyntax doubleQuoteToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(780, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InterpolatedStringExpressionSyntax)greenNode;
			}
			InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = new InterpolatedStringExpressionSyntax(SyntaxKind.InterpolatedStringExpression, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolatedStringExpressionSyntax, hash);
			}
			return interpolatedStringExpressionSyntax;
		}

		internal InterpolatedStringTextSyntax InterpolatedStringText(InterpolatedStringTextTokenSyntax textToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(781, textToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InterpolatedStringTextSyntax)greenNode;
			}
			InterpolatedStringTextSyntax interpolatedStringTextSyntax = new InterpolatedStringTextSyntax(SyntaxKind.InterpolatedStringText, textToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolatedStringTextSyntax, hash);
			}
			return interpolatedStringTextSyntax;
		}

		internal InterpolationSyntax Interpolation(PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken)
		{
			return new InterpolationSyntax(SyntaxKind.Interpolation, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken, _factoryContext);
		}

		internal InterpolationAlignmentClauseSyntax InterpolationAlignmentClause(PunctuationSyntax commaToken, ExpressionSyntax value)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(783, commaToken, value, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InterpolationAlignmentClauseSyntax)greenNode;
			}
			InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = new InterpolationAlignmentClauseSyntax(SyntaxKind.InterpolationAlignmentClause, commaToken, value, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolationAlignmentClauseSyntax, hash);
			}
			return interpolationAlignmentClauseSyntax;
		}

		internal InterpolationFormatClauseSyntax InterpolationFormatClause(PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken)
		{
			int hash = default(int);
			GreenNode greenNode = VisualBasicSyntaxNodeCache.TryGetNode(784, colonToken, formatStringToken, _factoryContext, ref hash);
			if (greenNode != null)
			{
				return (InterpolationFormatClauseSyntax)greenNode;
			}
			InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = new InterpolationFormatClauseSyntax(SyntaxKind.InterpolationFormatClause, colonToken, formatStringToken, _factoryContext);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolationFormatClauseSyntax, hash);
			}
			return interpolationFormatClauseSyntax;
		}

		internal SyntaxTrivia WhitespaceTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia EndOfLineTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia ColonTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.ColonTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia CommentTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.CommentTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia ConflictMarkerTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.ConflictMarkerTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia LineContinuationTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.LineContinuationTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia DocumentationCommentExteriorTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia DisabledTextTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.DisabledTextTrivia, text, _factoryContext);
		}

		internal SyntaxTrivia SyntaxTrivia(SyntaxKind kind, string text)
		{
			return new SyntaxTrivia(kind, text, _factoryContext);
		}

		internal ConstDirectiveTriviaSyntax ConstDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value)
		{
			return new ConstDirectiveTriviaSyntax(SyntaxKind.ConstDirectiveTrivia, hashToken, constKeyword, name, equalsToken, value, _factoryContext);
		}

		internal IfDirectiveTriviaSyntax IfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			return new IfDirectiveTriviaSyntax(SyntaxKind.IfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword, _factoryContext);
		}

		internal IfDirectiveTriviaSyntax ElseIfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			return new IfDirectiveTriviaSyntax(SyntaxKind.ElseIfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword, _factoryContext);
		}

		internal ElseDirectiveTriviaSyntax ElseDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword)
		{
			return new ElseDirectiveTriviaSyntax(SyntaxKind.ElseDirectiveTrivia, hashToken, elseKeyword, _factoryContext);
		}

		internal EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword)
		{
			return new EndIfDirectiveTriviaSyntax(SyntaxKind.EndIfDirectiveTrivia, hashToken, endKeyword, ifKeyword, _factoryContext);
		}

		internal RegionDirectiveTriviaSyntax RegionDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name)
		{
			return new RegionDirectiveTriviaSyntax(SyntaxKind.RegionDirectiveTrivia, hashToken, regionKeyword, name, _factoryContext);
		}

		internal EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword)
		{
			return new EndRegionDirectiveTriviaSyntax(SyntaxKind.EndRegionDirectiveTrivia, hashToken, endKeyword, regionKeyword, _factoryContext);
		}

		internal ExternalSourceDirectiveTriviaSyntax ExternalSourceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken)
		{
			return new ExternalSourceDirectiveTriviaSyntax(SyntaxKind.ExternalSourceDirectiveTrivia, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken, _factoryContext);
		}

		internal EndExternalSourceDirectiveTriviaSyntax EndExternalSourceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword)
		{
			return new EndExternalSourceDirectiveTriviaSyntax(SyntaxKind.EndExternalSourceDirectiveTrivia, hashToken, endKeyword, externalSourceKeyword, _factoryContext);
		}

		internal ExternalChecksumDirectiveTriviaSyntax ExternalChecksumDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken)
		{
			return new ExternalChecksumDirectiveTriviaSyntax(SyntaxKind.ExternalChecksumDirectiveTrivia, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken, _factoryContext);
		}

		internal EnableWarningDirectiveTriviaSyntax EnableWarningDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> errorCodes)
		{
			return new EnableWarningDirectiveTriviaSyntax(SyntaxKind.EnableWarningDirectiveTrivia, hashToken, enableKeyword, warningKeyword, errorCodes.Node, _factoryContext);
		}

		internal DisableWarningDirectiveTriviaSyntax DisableWarningDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> errorCodes)
		{
			return new DisableWarningDirectiveTriviaSyntax(SyntaxKind.DisableWarningDirectiveTrivia, hashToken, disableKeyword, warningKeyword, errorCodes.Node, _factoryContext);
		}

		internal ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file)
		{
			return new ReferenceDirectiveTriviaSyntax(SyntaxKind.ReferenceDirectiveTrivia, hashToken, referenceKeyword, file, _factoryContext);
		}

		internal BadDirectiveTriviaSyntax BadDirectiveTrivia(PunctuationSyntax hashToken)
		{
			return new BadDirectiveTriviaSyntax(SyntaxKind.BadDirectiveTrivia, hashToken, _factoryContext);
		}
	}
}
