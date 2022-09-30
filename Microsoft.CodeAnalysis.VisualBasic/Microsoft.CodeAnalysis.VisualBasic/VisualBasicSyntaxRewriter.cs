using System;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public abstract class VisualBasicSyntaxRewriter : VisualBasicSyntaxVisitor<SyntaxNode>
	{
		private readonly bool _visitIntoStructuredTrivia;

		private int _recursionDepth;

		public virtual bool VisitIntoStructuredTrivia => _visitIntoStructuredTrivia;

		public override SyntaxNode VisitEmptyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Empty).Node;
			if (node.Empty.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEndBlockStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EndKeyword).Node;
			if (node.EndKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.BlockKeyword).Node;
			if (node.BlockKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitCompilationUnit(Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax> syntaxList = VisitList(node.Options);
			if (node._options != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax> syntaxList2 = VisitList(node.Imports);
			if (node._imports != syntaxList2.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax> syntaxList3 = VisitList(node.Attributes);
			if (node._attributes != syntaxList3.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList4 = VisitList(node.Members);
			if (node._members != syntaxList4.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EndOfFileToken).Node;
			if (node.EndOfFileToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxList2.Node, syntaxList3.Node, syntaxList4.Node, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitOptionStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OptionKeyword).Node;
			if (node.OptionKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.NameKeyword).Node;
			if (node.NameKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.ValueKeyword).Node;
			if (node.ValueKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitImportsStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ImportsKeyword).Node;
			if (node.ImportsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax> separatedSyntaxList = VisitList(node.ImportsClauses);
			if (node._importsClauses != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitSimpleImportsClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax importAliasClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax)Visit(node.Alias);
			if (node.Alias != importAliasClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax nameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)Visit(node.Name);
			if (node.Name != nameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), importAliasClauseSyntax, nameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitImportAliasClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlNamespaceImportsClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanToken).Node;
			if (node.LessThanToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax xmlAttributeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)Visit(node.XmlNamespace);
			if (node.XmlNamespace != xmlAttributeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.GreaterThanToken).Node;
			if (node.GreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlAttributeSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitNamespaceBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax namespaceStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax)Visit(node.NamespaceStatement);
			if (node.NamespaceStatement != namespaceStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Members);
			if (node._members != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndNamespaceStatement);
			if (node.EndNamespaceStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), namespaceStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitNamespaceStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NamespaceKeyword).Node;
			if (node.NamespaceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax nameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)Visit(node.Name);
			if (node.Name != nameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, nameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitModuleBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax moduleStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax)Visit(node.ModuleStatement);
			if (node.ModuleStatement != moduleStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndModuleStatement);
			if (node.EndModuleStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), moduleStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitStructureBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax structureStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax)Visit(node.StructureStatement);
			if (node.StructureStatement != structureStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndStructureStatement);
			if (node.EndStructureStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), structureStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInterfaceBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax interfaceStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax)Visit(node.InterfaceStatement);
			if (node.InterfaceStatement != interfaceStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndInterfaceStatement);
			if (node.EndInterfaceStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), interfaceStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitClassBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax classStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax)Visit(node.ClassStatement);
			if (node.ClassStatement != classStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndClassStatement);
			if (node.EndClassStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), classStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEnumBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax enumStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax)Visit(node.EnumStatement);
			if (node.EnumStatement != enumStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Members);
			if (node._members != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndEnumStatement);
			if (node.EndEnumStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), enumStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInheritsStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.InheritsKeyword).Node;
			if (node.InheritsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax> separatedSyntaxList = VisitList(node.Types);
			if (node._types != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitImplementsStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ImplementsKeyword).Node;
			if (node.ImplementsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax> separatedSyntaxList = VisitList(node.Types);
			if (node._types != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitModuleStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ModuleKeyword).Node;
			if (node.ModuleKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitStructureStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.StructureKeyword).Node;
			if (node.StructureKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInterfaceStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.InterfaceKeyword).Node;
			if (node.InterfaceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitClassStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ClassKeyword).Node;
			if (node.ClassKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEnumStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EnumKeyword).Node;
			if (node.EnumKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax asClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)Visit(node.UnderlyingType);
			if (node.UnderlyingType != asClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, asClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTypeParameterList(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OfKeyword).Node;
			if (node.OfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax> separatedSyntaxList = VisitList(node.Parameters);
			if (node._parameters != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitTypeParameter(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.VarianceKeyword).Node;
			if (node.VarianceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax typeParameterConstraintClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax)Visit(node.TypeParameterConstraintClause);
			if (node.TypeParameterConstraintClause != typeParameterConstraintClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierTokenSyntax, typeParameterConstraintClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTypeParameterSingleConstraintClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AsKeyword).Node;
			if (node.AsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax constraintSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)Visit(node.Constraint);
			if (node.Constraint != constraintSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, constraintSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTypeParameterMultipleConstraintClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AsKeyword).Node;
			if (node.AsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenBraceToken).Node;
			if (node.OpenBraceToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax> separatedSyntaxList = VisitList(node.Constraints);
			if (node._constraints != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseBraceToken).Node;
			if (node.CloseBraceToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitSpecialConstraint(Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ConstraintKeyword).Node;
			if (node.ConstraintKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTypeConstraint(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEnumMemberDeclaration(Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax equalsValueSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)Visit(node.Initializer);
			if (node.Initializer != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, identifierTokenSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitMethodBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax methodStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax)Visit(node.SubOrFunctionStatement);
			if (node.SubOrFunctionStatement != methodStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndSubOrFunctionStatement);
			if (node.EndSubOrFunctionStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), methodStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitConstructorBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax subNewStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax)Visit(node.SubNewStatement);
			if (node.SubNewStatement != subNewStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndSubStatement);
			if (node.EndSubStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), subNewStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitOperatorBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax operatorStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax)Visit(node.OperatorStatement);
			if (node.OperatorStatement != operatorStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndOperatorStatement);
			if (node.EndOperatorStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), operatorStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAccessorBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax accessorStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax)Visit(node.AccessorStatement);
			if (node.AccessorStatement != accessorStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndAccessorStatement);
			if (node.EndAccessorStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), accessorStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPropertyBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax propertyStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)Visit(node.PropertyStatement);
			if (node.PropertyStatement != propertyStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax> syntaxList = VisitList(node.Accessors);
			if (node._accessors != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndPropertyStatement);
			if (node.EndPropertyStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), propertyStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEventBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax eventStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax)Visit(node.EventStatement);
			if (node.EventStatement != eventStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax> syntaxList = VisitList(node.Accessors);
			if (node._accessors != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndEventStatement);
			if (node.EndEventStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), eventStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitParameterList(Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax> separatedSyntaxList = VisitList(node.Parameters);
			if (node._parameters != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitMethodStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SubOrFunctionKeyword).Node;
			if (node.SubOrFunctionKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax handlesClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax)Visit(node.HandlesClause);
			if (node.HandlesClause != handlesClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax implementsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)Visit(node.ImplementsClause);
			if (node.ImplementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSubNewStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SubKeyword).Node;
			if (node.SubKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.NewKeyword).Node;
			if (node.NewKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, keywordSyntax2, parameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitDeclareStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.DeclareKeyword).Node;
			if (node.DeclareKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.CharsetKeyword).Node;
			if (node.CharsetKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.SubOrFunctionKeyword).Node;
			if (node.SubOrFunctionKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)VisitToken(node.LibKeyword).Node;
			if (node.LibKeyword.Node != keywordSyntax4)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax literalExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)Visit(node.LibraryName);
			if (node.LibraryName != literalExpressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax5 = (KeywordSyntax)VisitToken(node.AliasKeyword).Node;
			if (node.AliasKeyword.Node != keywordSyntax5)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax literalExpressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)Visit(node.AliasName);
			if (node.AliasName != literalExpressionSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, keywordSyntax2, keywordSyntax3, identifierTokenSyntax, keywordSyntax4, literalExpressionSyntax, keywordSyntax5, literalExpressionSyntax2, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitDelegateStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.DelegateKeyword).Node;
			if (node.DelegateKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.SubOrFunctionKeyword).Node;
			if (node.SubOrFunctionKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax typeParameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)Visit(node.TypeParameterList);
			if (node.TypeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, keywordSyntax2, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEventStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.CustomKeyword).Node;
			if (node.CustomKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.EventKeyword).Node;
			if (node.EventKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax implementsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)Visit(node.ImplementsClause);
			if (node.ImplementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, keywordSyntax2, identifierTokenSyntax, parameterListSyntax, simpleAsClauseSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitOperatorStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OperatorKeyword).Node;
			if (node.OperatorKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != syntaxToken)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, syntaxToken, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPropertyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.PropertyKeyword).Node;
			if (node.PropertyKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax asClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != asClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax equalsValueSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)Visit(node.Initializer);
			if (node.Initializer != equalsValueSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax implementsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)Visit(node.ImplementsClause);
			if (node.ImplementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, identifierTokenSyntax, parameterListSyntax, asClauseSyntax, equalsValueSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAccessorStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AccessorKeyword).Node;
			if (node.AccessorKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, parameterListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitImplementsClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ImplementsKeyword).Node;
			if (node.ImplementsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax> separatedSyntaxList = VisitList(node.InterfaceMembers);
			if (node._interfaceMembers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitHandlesClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.HandlesKeyword).Node;
			if (node.HandlesKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax> separatedSyntaxList = VisitList(node.Events);
			if (node._events != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitKeywordEventContainer(Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWithEventsEventContainer(Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWithEventsPropertyEventContainer(Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax withEventsEventContainerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax)Visit(node.WithEventsContainer);
			if (node.WithEventsContainer != withEventsEventContainerSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DotToken).Node;
			if (node.DotToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.Property);
			if (node.Property != identifierNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), withEventsEventContainerSyntax, punctuationSyntax, identifierNameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitHandlesClauseItem(Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax eventContainerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax)Visit(node.EventContainer);
			if (node.EventContainer != eventContainerSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DotToken).Node;
			if (node.DotToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.EventMember);
			if (node.EventMember != identifierNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), eventContainerSyntax, punctuationSyntax, identifierNameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitIncompleteMember(Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.MissingIdentifier).Node;
			if (node.MissingIdentifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, identifierTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitFieldDeclaration(Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Declarators);
			if (node._declarators != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitVariableDeclarator(Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax node)
		{
			bool flag = false;
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax> separatedSyntaxList = VisitList(node.Names);
			if (node._names != separatedSyntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax asClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != asClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax equalsValueSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)Visit(node.Initializer);
			if (node.Initializer != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), separatedSyntaxList.Node, asClauseSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSimpleAsClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AsKeyword).Node;
			if (node.AsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAsNewClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AsKeyword).Node;
			if (node.AsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax newExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax)Visit(node.NewExpression);
			if (node.NewExpression != newExpressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, newExpressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitObjectMemberInitializer(Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WithKeyword).Node;
			if (node.WithKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenBraceToken).Node;
			if (node.OpenBraceToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax> separatedSyntaxList = VisitList(node.Initializers);
			if (node._initializers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseBraceToken).Node;
			if (node.CloseBraceToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitObjectCollectionInitializer(Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.FromKeyword).Node;
			if (node.FromKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax collectionInitializerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)Visit(node.Initializer);
			if (node.Initializer != collectionInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, collectionInitializerSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInferredFieldInitializer(Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.KeyKeyword).Node;
			if (node.KeyKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitNamedFieldInitializer(Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.KeyKeyword).Node;
			if (node.KeyKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DotToken).Node;
			if (node.DotToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.Name);
			if (node.Name != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, identifierNameSyntax, punctuationSyntax2, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEqualsValue(Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Value);
			if (node.Value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitParameter(Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax modifiedIdentifierSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)Visit(node.Identifier);
			if (node.Identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax equalsValueSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)Visit(node.Default);
			if (node.Default != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, modifiedIdentifierSyntax, simpleAsClauseSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitModifiedIdentifier(Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Nullable).Node;
			if (node.Nullable.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArrayBounds);
			if (node.ArrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax> syntaxList = VisitList(node.ArrayRankSpecifiers);
			if (node._arrayRankSpecifiers != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, argumentListSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitArrayRankSpecifier(Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.CommaTokens);
			if (node.CommaTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitAttributeList(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanToken).Node;
			if (node.LessThanToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax> separatedSyntaxList = VisitList(node.Attributes);
			if (node._attributes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.GreaterThanToken).Node;
			if (node.GreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax attributeTargetSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax)Visit(node.Target);
			if (node.Target != attributeTargetSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Name);
			if (node.Name != typeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArgumentList);
			if (node.ArgumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeTargetSyntax, typeSyntax, argumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAttributeTarget(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AttributeModifier).Node;
			if (node.AttributeModifier.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.ColonToken).Node;
			if (node.ColonToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAttributesStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitExpressionStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPrintStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.QuestionToken).Node;
			if (node.QuestionToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWhileBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax whileStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax)Visit(node.WhileStatement);
			if (node.WhileStatement != whileStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndWhileStatement);
			if (node.EndWhileStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), whileStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitUsingBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax usingStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax)Visit(node.UsingStatement);
			if (node.UsingStatement != usingStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndUsingStatement);
			if (node.EndUsingStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), usingStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSyncLockBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax syncLockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax)Visit(node.SyncLockStatement);
			if (node.SyncLockStatement != syncLockStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndSyncLockStatement);
			if (node.EndSyncLockStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syncLockStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWithBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax withStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax)Visit(node.WithStatement);
			if (node.WithStatement != withStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndWithStatement);
			if (node.EndWithStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), withStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitLocalDeclarationStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax node)
		{
			bool flag = false;
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Declarators);
			if (node._declarators != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenList.Node, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitLabelStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.LabelToken).Node;
			if (node.LabelToken.Node != syntaxToken)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.ColonToken).Node;
			if (node.ColonToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGoToStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GoToKeyword).Node;
			if (node.GoToKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax labelSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)Visit(node.Label);
			if (node.Label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, labelSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitLabel(Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.LabelToken).Node;
			if (node.LabelToken.Node != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken);
			}
			return node;
		}

		public override SyntaxNode VisitStopOrEndStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.StopOrEndKeyword).Node;
			if (node.StopOrEndKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitExitStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ExitKeyword).Node;
			if (node.ExitKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.BlockKeyword).Node;
			if (node.BlockKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitContinueStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ContinueKeyword).Node;
			if (node.ContinueKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.BlockKeyword).Node;
			if (node.BlockKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ReturnKeyword).Node;
			if (node.ReturnKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSingleLineIfStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.IfKeyword).Node;
			if (node.IfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ThenKeyword).Node;
			if (node.ThenKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax singleLineElseClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax)Visit(node.ElseClause);
			if (node.ElseClause != singleLineElseClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2, syntaxList.Node, singleLineElseClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSingleLineElseClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseKeyword).Node;
			if (node.ElseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitMultiLineIfBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax ifStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax)Visit(node.IfStatement);
			if (node.IfStatement != ifStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax> syntaxList2 = VisitList(node.ElseIfBlocks);
			if (node._elseIfBlocks != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax elseBlockSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax)Visit(node.ElseBlock);
			if (node.ElseBlock != elseBlockSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndIfStatement);
			if (node.EndIfStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), ifStatementSyntax, syntaxList.Node, syntaxList2.Node, elseBlockSyntax, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitIfStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.IfKeyword).Node;
			if (node.IfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ThenKeyword).Node;
			if (node.ThenKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitElseIfBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax elseIfStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax)Visit(node.ElseIfStatement);
			if (node.ElseIfStatement != elseIfStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), elseIfStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitElseIfStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseIfKeyword).Node;
			if (node.ElseIfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ThenKeyword).Node;
			if (node.ThenKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitElseBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax elseStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax)Visit(node.ElseStatement);
			if (node.ElseStatement != elseStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), elseStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitElseStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseKeyword).Node;
			if (node.ElseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTryBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax tryStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax)Visit(node.TryStatement);
			if (node.TryStatement != tryStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax> syntaxList2 = VisitList(node.CatchBlocks);
			if (node._catchBlocks != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax finallyBlockSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax)Visit(node.FinallyBlock);
			if (node.FinallyBlock != finallyBlockSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndTryStatement);
			if (node.EndTryStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), tryStatementSyntax, syntaxList.Node, syntaxList2.Node, finallyBlockSyntax, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTryStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.TryKeyword).Node;
			if (node.TryKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCatchBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax catchStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax)Visit(node.CatchStatement);
			if (node.CatchStatement != catchStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), catchStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitCatchStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.CatchKeyword).Node;
			if (node.CatchKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.IdentifierName);
			if (node.IdentifierName != identifierNameSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax catchFilterClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax)Visit(node.WhenClause);
			if (node.WhenClause != catchFilterClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierNameSyntax, simpleAsClauseSyntax, catchFilterClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCatchFilterClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WhenKeyword).Node;
			if (node.WhenKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Filter);
			if (node.Filter != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitFinallyBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax finallyStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax)Visit(node.FinallyStatement);
			if (node.FinallyStatement != finallyStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), finallyStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitFinallyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.FinallyKeyword).Node;
			if (node.FinallyKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitErrorStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ErrorKeyword).Node;
			if (node.ErrorKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.ErrorNumber);
			if (node.ErrorNumber != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitOnErrorGoToStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OnKeyword).Node;
			if (node.OnKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ErrorKeyword).Node;
			if (node.ErrorKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.GoToKeyword).Node;
			if (node.GoToKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Minus).Node;
			if (node.Minus.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax labelSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)Visit(node.Label);
			if (node.Label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3, punctuationSyntax, labelSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitOnErrorResumeNextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OnKeyword).Node;
			if (node.OnKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ErrorKeyword).Node;
			if (node.ErrorKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.ResumeKeyword).Node;
			if (node.ResumeKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)VisitToken(node.NextKeyword).Node;
			if (node.NextKeyword.Node != keywordSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3, keywordSyntax4);
			}
			return node;
		}

		public override SyntaxNode VisitResumeStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ResumeKeyword).Node;
			if (node.ResumeKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax labelSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)Visit(node.Label);
			if (node.Label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, labelSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSelectBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax selectStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax)Visit(node.SelectStatement);
			if (node.SelectStatement != selectStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax> syntaxList = VisitList(node.CaseBlocks);
			if (node._caseBlocks != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndSelectStatement);
			if (node.EndSelectStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), selectStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSelectStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SelectKeyword).Node;
			if (node.SelectKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.CaseKeyword).Node;
			if (node.CaseKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCaseBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax caseStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax)Visit(node.CaseStatement);
			if (node.CaseStatement != caseStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), caseStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitCaseStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.CaseKeyword).Node;
			if (node.CaseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax> separatedSyntaxList = VisitList(node.Cases);
			if (node._cases != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitElseCaseClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseKeyword).Node;
			if (node.ElseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSimpleCaseClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Value);
			if (node.Value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitRangeCaseClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.LowerBound);
			if (node.LowerBound != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ToKeyword).Node;
			if (node.ToKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.UpperBound);
			if (node.UpperBound != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitRelationalCaseClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.IsKeyword).Node;
			if (node.IsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Value);
			if (node.Value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSyncLockStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SyncLockKeyword).Node;
			if (node.SyncLockKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitDoLoopBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax doStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax)Visit(node.DoStatement);
			if (node.DoStatement != doStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax loopStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax)Visit(node.LoopStatement);
			if (node.LoopStatement != loopStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), doStatementSyntax, syntaxList.Node, loopStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitDoStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.DoKeyword).Node;
			if (node.DoKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)Visit(node.WhileOrUntilClause);
			if (node.WhileOrUntilClause != whileOrUntilClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitLoopStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.LoopKeyword).Node;
			if (node.LoopKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)Visit(node.WhileOrUntilClause);
			if (node.WhileOrUntilClause != whileOrUntilClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWhileOrUntilClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WhileOrUntilKeyword).Node;
			if (node.WhileOrUntilKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWhileStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WhileKeyword).Node;
			if (node.WhileKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitForBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax forStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax)Visit(node.ForStatement);
			if (node.ForStatement != forStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax nextStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)Visit(node.NextStatement);
			if (node.NextStatement != nextStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), forStatementSyntax, syntaxList.Node, nextStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitForEachBlock(Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax forEachStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax)Visit(node.ForEachStatement);
			if (node.ForEachStatement != forEachStatementSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax nextStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)Visit(node.NextStatement);
			if (node.NextStatement != nextStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), forEachStatementSyntax, syntaxList.Node, nextStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitForStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ForKeyword).Node;
			if (node.ForKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)Visit(node.ControlVariable);
			if (node.ControlVariable != visualBasicSyntaxNode)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.FromValue);
			if (node.FromValue != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ToKeyword).Node;
			if (node.ToKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.ToValue);
			if (node.ToValue != expressionSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax forStepClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax)Visit(node.StepClause);
			if (node.StepClause != forStepClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, visualBasicSyntaxNode, punctuationSyntax, expressionSyntax, keywordSyntax2, expressionSyntax2, forStepClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitForStepClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.StepKeyword).Node;
			if (node.StepKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.StepValue);
			if (node.StepValue != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitForEachStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ForKeyword).Node;
			if (node.ForKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.EachKeyword).Node;
			if (node.EachKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)Visit(node.ControlVariable);
			if (node.ControlVariable != visualBasicSyntaxNode)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.InKeyword).Node;
			if (node.InKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, visualBasicSyntaxNode, keywordSyntax3, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitNextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NextKeyword).Node;
			if (node.NextKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax> separatedSyntaxList = VisitList(node.ControlVariables);
			if (node._controlVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitUsingStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.UsingKeyword).Node;
			if (node.UsingKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitThrowStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ThrowKeyword).Node;
			if (node.ThrowKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAssignmentStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Left);
			if (node.Left != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Right);
			if (node.Right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitMidExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Mid).Node;
			if (node.Mid.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArgumentList);
			if (node.ArgumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, argumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCallStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.CallKeyword).Node;
			if (node.CallKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Invocation);
			if (node.Invocation != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAddRemoveHandlerStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AddHandlerOrRemoveHandlerKeyword).Node;
			if (node.AddHandlerOrRemoveHandlerKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.EventExpression);
			if (node.EventExpression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.DelegateExpression);
			if (node.DelegateExpression != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitRaiseEventStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.RaiseEventKeyword).Node;
			if (node.RaiseEventKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.Name);
			if (node.Name != identifierNameSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArgumentList);
			if (node.ArgumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierNameSyntax, argumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWithStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WithKeyword).Node;
			if (node.WithKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitReDimStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ReDimKeyword).Node;
			if (node.ReDimKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.PreserveKeyword).Node;
			if (node.PreserveKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax> separatedSyntaxList = VisitList(node.Clauses);
			if (node._clauses != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitRedimClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArrayBounds);
			if (node.ArrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, argumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEraseStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EraseKeyword).Node;
			if (node.EraseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax> separatedSyntaxList = VisitList(node.Expressions);
			if (node._expressions != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitLiteralExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.Token).Node;
			if (node.Token.Node != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken);
			}
			return node;
		}

		public override SyntaxNode VisitParenthesizedExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitTupleExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitTupleType(Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax> separatedSyntaxList = VisitList(node.Elements);
			if (node._elements != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitTypedTupleElement(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypedTupleElementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypedTupleElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitNamedTupleElement(Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitMeExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitMyBaseExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitMyClassExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MyClassExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MyClassExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGetTypeExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.GetTypeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GetTypeKeyword).Node;
			if (node.GetTypeKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GetTypeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, typeSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitTypeOfExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.TypeOfKeyword).Node;
			if (node.TypeOfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2, typeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGetXmlNamespaceExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GetXmlNamespaceKeyword).Node;
			if (node.GetXmlNamespaceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax xmlPrefixNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax)Visit(node.Name);
			if (node.Name != xmlPrefixNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, xmlPrefixNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitMemberAccessExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax simpleNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)Visit(node.Name);
			if (node.Name != simpleNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, simpleNameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlMemberAccessExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Base);
			if (node.Base != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Token1).Node;
			if (node.Token1.Node != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.Token2).Node;
			if (node.Token2.Node != punctuationSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.Token3).Node;
			if (node.Token3.Node != punctuationSyntax3)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, punctuationSyntax2, punctuationSyntax3, xmlNodeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInvocationExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArgumentList);
			if (node.ArgumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, argumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitObjectCreationExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NewKeyword).Node;
			if (node.NewKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArgumentList);
			if (node.ArgumentList != argumentListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax objectCreationInitializerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax)Visit(node.Initializer);
			if (node.Initializer != objectCreationInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, objectCreationInitializerSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAnonymousObjectCreationExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NewKeyword).Node;
			if (node.NewKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax objectMemberInitializerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax)Visit(node.Initializer);
			if (node.Initializer != objectMemberInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxList.Node, objectMemberInitializerSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitArrayCreationExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NewKeyword).Node;
			if (node.NewKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax argumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)Visit(node.ArrayBounds);
			if (node.ArrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax> syntaxList2 = VisitList(node.RankSpecifiers);
			if (node._rankSpecifiers != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax collectionInitializerSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)Visit(node.Initializer);
			if (node.Initializer != collectionInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, syntaxList2.Node, collectionInitializerSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCollectionInitializer(Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenBraceToken).Node;
			if (node.OpenBraceToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax> separatedSyntaxList = VisitList(node.Initializers);
			if (node._initializers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseBraceToken).Node;
			if (node.CloseBraceToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitCTypeExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitDirectCastExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitTryCastExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.TryCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TryCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitPredefinedCastExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitBinaryExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Left);
			if (node.Left != expressionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != syntaxToken)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Right);
			if (node.Right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, syntaxToken, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitUnaryExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != syntaxToken)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Operand);
			if (node.Operand != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitBinaryConditionalExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.IfKeyword).Node;
			if (node.IfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.FirstExpression);
			if (node.FirstExpression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.SecondExpression);
			if (node.SecondExpression != expressionSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, expressionSyntax2, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitTernaryConditionalExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.IfKeyword).Node;
			if (node.IfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.FirstCommaToken).Node;
			if (node.FirstCommaToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.WhenTrue);
			if (node.WhenTrue != expressionSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.SecondCommaToken).Node;
			if (node.SecondCommaToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax3 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.WhenFalse);
			if (node.WhenFalse != expressionSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, expressionSyntax2, punctuationSyntax3, expressionSyntax3, punctuationSyntax4);
			}
			return node;
		}

		public override SyntaxNode VisitSingleLineLambdaExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax lambdaHeaderSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)Visit(node.SubOrFunctionHeader);
			if (node.SubOrFunctionHeader != lambdaHeaderSyntax)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)Visit(node.Body);
			if (node.Body != visualBasicSyntaxNode)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), lambdaHeaderSyntax, visualBasicSyntaxNode);
			}
			return node;
		}

		public override SyntaxNode VisitMultiLineLambdaExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax lambdaHeaderSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)Visit(node.SubOrFunctionHeader);
			if (node.SubOrFunctionHeader != lambdaHeaderSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax endBlockStatementSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)Visit(node.EndSubOrFunctionStatement);
			if (node.EndSubOrFunctionStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), lambdaHeaderSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitLambdaHeader(Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.Modifiers);
			if (node.Modifiers.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SubOrFunctionKeyword).Node;
			if (node.SubOrFunctionKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax parameterListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)Visit(node.ParameterList);
			if (node.ParameterList != parameterListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node, syntaxTokenList.Node, keywordSyntax, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitArgumentList(Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitOmittedArgument(Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Empty).Node;
			if (node.Empty.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSimpleArgument(Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax nameColonEqualsSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax)Visit(node.NameColonEquals);
			if (node.NameColonEquals != nameColonEqualsSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameColonEqualsSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitNameColonEquals(Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.Name);
			if (node.Name != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.ColonEqualsToken).Node;
			if (node.ColonEqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierNameSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitRangeArgument(Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.LowerBound);
			if (node.LowerBound != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ToKeyword).Node;
			if (node.ToKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.UpperBound);
			if (node.UpperBound != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitQueryExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax> syntaxList = VisitList(node.Clauses);
			if (node._clauses != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitCollectionRangeVariable(Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax modifiedIdentifierSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)Visit(node.Identifier);
			if (node.Identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.InKeyword).Node;
			if (node.InKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitExpressionRangeVariable(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax variableNameEqualsSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)Visit(node.NameEquals);
			if (node.NameEquals != variableNameEqualsSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), variableNameEqualsSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAggregationRangeVariable(Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax variableNameEqualsSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)Visit(node.NameEquals);
			if (node.NameEquals != variableNameEqualsSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax aggregationSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax)Visit(node.Aggregation);
			if (node.Aggregation != aggregationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), variableNameEqualsSyntax, aggregationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitVariableNameEquals(Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax modifiedIdentifierSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)Visit(node.Identifier);
			if (node.Identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitFunctionAggregation(Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.FunctionName).Node;
			if (node.FunctionName.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Argument);
			if (node.Argument != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitGroupAggregation(Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GroupKeyword).Node;
			if (node.GroupKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitFromClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.FromKeyword).Node;
			if (node.FromKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitLetClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.LetKeyword).Node;
			if (node.LetKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitAggregateClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AggregateKeyword).Node;
			if (node.AggregateKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax> syntaxList = VisitList(node.AdditionalQueryOperators);
			if (node._additionalQueryOperators != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.IntoKeyword).Node;
			if (node.IntoKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax> separatedSyntaxList2 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node);
			}
			return node;
		}

		public override SyntaxNode VisitDistinctClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.DistinctKeyword).Node;
			if (node.DistinctKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitWhereClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.WhereKeyword).Node;
			if (node.WhereKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPartitionWhileClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SkipOrTakeKeyword).Node;
			if (node.SkipOrTakeKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.WhileKeyword).Node;
			if (node.WhileKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPartitionClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SkipOrTakeKeyword).Node;
			if (node.SkipOrTakeKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Count);
			if (node.Count != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGroupByClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GroupKeyword).Node;
			if (node.GroupKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Items);
			if (node._items != separatedSyntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ByKeyword).Node;
			if (node.ByKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax> separatedSyntaxList2 = VisitList(node.Keys);
			if (node._keys != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.IntoKeyword).Node;
			if (node.IntoKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax> separatedSyntaxList3 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList3.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node, keywordSyntax3, separatedSyntaxList3.Node);
			}
			return node;
		}

		public override SyntaxNode VisitJoinCondition(Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Left);
			if (node.Left != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EqualsKeyword).Node;
			if (node.EqualsKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Right);
			if (node.Right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitSimpleJoinClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.JoinKeyword).Node;
			if (node.JoinKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.JoinedVariables);
			if (node._joinedVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax> syntaxList = VisitList(node.AdditionalJoins);
			if (node._additionalJoins != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.OnKeyword).Node;
			if (node.OnKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax> separatedSyntaxList2 = VisitList(node.JoinConditions);
			if (node._joinConditions != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node);
			}
			return node;
		}

		public override SyntaxNode VisitGroupJoinClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GroupKeyword).Node;
			if (node.GroupKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.JoinKeyword).Node;
			if (node.JoinKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.JoinedVariables);
			if (node._joinedVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax> syntaxList = VisitList(node.AdditionalJoins);
			if (node._additionalJoins != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.OnKeyword).Node;
			if (node.OnKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax> separatedSyntaxList2 = VisitList(node.JoinConditions);
			if (node._joinConditions != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)VisitToken(node.IntoKeyword).Node;
			if (node.IntoKeyword.Node != keywordSyntax4)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax> separatedSyntaxList3 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList3.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax3, separatedSyntaxList2.Node, keywordSyntax4, separatedSyntaxList3.Node);
			}
			return node;
		}

		public override SyntaxNode VisitOrderByClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OrderKeyword).Node;
			if (node.OrderKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ByKeyword).Node;
			if (node.ByKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax> separatedSyntaxList = VisitList(node.Orderings);
			if (node._orderings != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitOrdering(Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AscendingOrDescendingKeyword).Node;
			if (node.AscendingOrDescendingKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSelectClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.SelectKeyword).Node;
			if (node.SelectKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitXmlDocument(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax xmlDeclarationSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax)Visit(node.Declaration);
			if (node.Declaration != xmlDeclarationSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList = VisitList(node.PrecedingMisc);
			if (node._precedingMisc != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Root);
			if (node.Root != xmlNodeSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList2 = VisitList(node.FollowingMisc);
			if (node._followingMisc != syntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlDeclarationSyntax, syntaxList.Node, xmlNodeSyntax, syntaxList2.Node);
			}
			return node;
		}

		public override SyntaxNode VisitXmlDeclaration(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanQuestionToken).Node;
			if (node.LessThanQuestionToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.XmlKeyword).Node;
			if (node.XmlKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)Visit(node.Version);
			if (node.Version != xmlDeclarationOptionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)Visit(node.Encoding);
			if (node.Encoding != xmlDeclarationOptionSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax3 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)Visit(node.Standalone);
			if (node.Standalone != xmlDeclarationOptionSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.QuestionGreaterThanToken).Node;
			if (node.QuestionGreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, xmlDeclarationOptionSyntax, xmlDeclarationOptionSyntax2, xmlDeclarationOptionSyntax3, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlDeclarationOption(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != xmlNameTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.Equals).Node;
			if (node.Equals.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax xmlStringSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax)Visit(node.Value);
			if (node.Value != xmlStringSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax, xmlStringSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlElement(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax xmlElementStartTagSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax)Visit(node.StartTag);
			if (node.StartTag != xmlElementStartTagSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList = VisitList(node.Content);
			if (node._content != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax xmlElementEndTagSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax)Visit(node.EndTag);
			if (node.EndTag != xmlElementEndTagSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlElementStartTagSyntax, syntaxList.Node, xmlElementEndTagSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlText(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax node)
		{
			bool flag = false;
			SyntaxTokenList syntaxTokenList = VisitList(node.TextTokens);
			if (node.TextTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitXmlElementStartTag(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanToken).Node;
			if (node.LessThanToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node._attributes != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.GreaterThanToken).Node;
			if (node.GreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlElementEndTag(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanSlashToken).Node;
			if (node.LessThanSlashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax xmlNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.GreaterThanToken).Node;
			if (node.GreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlEmptyElement(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanToken).Node;
			if (node.LessThanToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node._attributes != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.SlashGreaterThanToken).Node;
			if (node.SlashGreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax xmlNodeSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)Visit(node.Value);
			if (node.Value != xmlNodeSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlString(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.StartQuoteToken).Node;
			if (node.StartQuoteToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.TextTokens);
			if (node.TextTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.EndQuoteToken).Node;
			if (node.EndQuoteToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlPrefixName(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != xmlNameTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlName(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax xmlPrefixSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax)Visit(node.Prefix);
			if (node.Prefix != xmlPrefixSyntax)
			{
				flag = true;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitToken(node.LocalName).Node;
			if (node.LocalName.Node != xmlNameTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlPrefixSyntax, xmlNameTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlBracketedName(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanToken).Node;
			if (node.LessThanToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax xmlNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.GreaterThanToken).Node;
			if (node.GreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlPrefix(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != xmlNameTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.ColonToken).Node;
			if (node.ColonToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitXmlComment(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanExclamationMinusMinusToken).Node;
			if (node.LessThanExclamationMinusMinusToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.TextTokens);
			if (node.TextTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.MinusMinusGreaterThanToken).Node;
			if (node.MinusMinusGreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlProcessingInstruction(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanQuestionToken).Node;
			if (node.LessThanQuestionToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != xmlNameTokenSyntax)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.TextTokens);
			if (node.TextTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.QuestionGreaterThanToken).Node;
			if (node.QuestionGreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameTokenSyntax, syntaxTokenList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlCDataSection(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.BeginCDataToken).Node;
			if (node.BeginCDataToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SyntaxTokenList syntaxTokenList = VisitList(node.TextTokens);
			if (node.TextTokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.EndCDataToken).Node;
			if (node.EndCDataToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitXmlEmbeddedExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.LessThanPercentEqualsToken).Node;
			if (node.LessThanPercentEqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.PercentGreaterThanToken).Node;
			if (node.PercentGreaterThanToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitArrayType(Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.ElementType);
			if (node.ElementType != typeSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax> syntaxList = VisitList(node.RankSpecifiers);
			if (node._rankSpecifiers != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitNullableType(Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.ElementType);
			if (node.ElementType != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.QuestionMarkToken).Node;
			if (node.QuestionMarkToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitPredefinedType(Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Keyword).Node;
			if (node.Keyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitIdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGenericName(Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Identifier).Node;
			if (node.Identifier.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax typeArgumentListSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax)Visit(node.TypeArgumentList);
			if (node.TypeArgumentList != typeArgumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, typeArgumentListSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitQualifiedName(Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax nameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)Visit(node.Left);
			if (node.Left != nameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DotToken).Node;
			if (node.DotToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax simpleNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)Visit(node.Right);
			if (node.Right != simpleNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameSyntax, punctuationSyntax, simpleNameSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitGlobalName(Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.GlobalKeyword).Node;
			if (node.GlobalKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitTypeArgumentList(Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OfKeyword).Node;
			if (node.OfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitCrefReference(Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Name);
			if (node.Name != typeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax crefSignatureSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax)Visit(node.Signature);
			if (node.Signature != crefSignatureSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax simpleAsClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)Visit(node.AsClause);
			if (node.AsClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, crefSignatureSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCrefSignature(Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax> separatedSyntaxList = VisitList(node.ArgumentTypes);
			if (node._argumentTypes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitCrefSignaturePart(Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.Modifier).Node;
			if (node.Modifier.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax typeSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)Visit(node.Type);
			if (node.Type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, typeSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitCrefOperatorReference(Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.OperatorKeyword).Node;
			if (node.OperatorKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken syntaxToken = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)VisitToken(node.OperatorToken).Node;
			if (node.OperatorToken.Node != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxToken);
			}
			return node;
		}

		public override SyntaxNode VisitQualifiedCrefOperatorReference(Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax nameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)Visit(node.Left);
			if (node.Left != nameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DotToken).Node;
			if (node.DotToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax crefOperatorReferenceSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax)Visit(node.Right);
			if (node.Right != crefOperatorReferenceSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameSyntax, punctuationSyntax, crefOperatorReferenceSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitYieldStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.YieldStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.YieldKeyword).Node;
			if (node.YieldKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.YieldStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitAwaitExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.AwaitKeyword).Node;
			if (node.AwaitKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitSkippedTokensTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax node)
		{
			bool flag = false;
			SyntaxTokenList syntaxTokenList = VisitList(node.Tokens);
			if (node.Tokens.Node != syntaxTokenList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitDocumentationCommentTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax node)
		{
			bool flag = false;
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax> syntaxList = VisitList(node.Content);
			if (node._content != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitXmlCrefAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax xmlNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.StartQuoteToken).Node;
			if (node.StartQuoteToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax crefReferenceSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax)Visit(node.Reference);
			if (node.Reference != crefReferenceSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.EndQuoteToken).Node;
			if (node.EndQuoteToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax2, crefReferenceSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitXmlNameAttribute(Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax xmlNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.StartQuoteToken).Node;
			if (node.StartQuoteToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)Visit(node.Reference);
			if (node.Reference != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.EndQuoteToken).Node;
			if (node.EndQuoteToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax2, identifierNameSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitConditionalAccessExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.QuestionMarkToken).Node;
			if (node.QuestionMarkToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.WhenNotNull);
			if (node.WhenNotNull != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitNameOfExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.NameOfKeyword).Node;
			if (node.NameOfKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Argument);
			if (node.Argument != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitInterpolatedStringExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.DollarSignDoubleQuoteToken).Node;
			if (node.DollarSignDoubleQuoteToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax> syntaxList = VisitList(node.Contents);
			if (node._contents != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.DoubleQuoteToken).Node;
			if (node.DoubleQuoteToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitInterpolatedStringText(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax node)
		{
			bool flag = false;
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)VisitToken(node.TextToken).Node;
			if (node.TextToken.Node != interpolatedStringTextTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), interpolatedStringTextTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInterpolation(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.OpenBraceToken).Node;
			if (node.OpenBraceToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Expression);
			if (node.Expression != expressionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax)Visit(node.AlignmentClause);
			if (node.AlignmentClause != interpolationAlignmentClauseSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax)Visit(node.FormatClause);
			if (node.FormatClause != interpolationFormatClauseSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.CloseBraceToken).Node;
			if (node.CloseBraceToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, interpolationAlignmentClauseSyntax, interpolationFormatClauseSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitInterpolationAlignmentClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Value);
			if (node.Value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitInterpolationFormatClause(Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.ColonToken).Node;
			if (node.ColonToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)VisitToken(node.FormatStringToken).Node;
			if (node.FormatStringToken.Node != interpolatedStringTextTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, interpolatedStringTextTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitConstDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ConstKeyword).Node;
			if (node.ConstKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.EqualsToken).Node;
			if (node.EqualsToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Value);
			if (node.Value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, identifierTokenSyntax, punctuationSyntax2, expressionSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitIfDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseKeyword).Node;
			if (node.ElseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.IfOrElseIfKeyword).Node;
			if (node.IfOrElseIfKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)Visit(node.Condition);
			if (node.Condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)VisitToken(node.ThenKeyword).Node;
			if (node.ThenKeyword.Node != keywordSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, expressionSyntax, keywordSyntax3);
			}
			return node;
		}

		public override SyntaxNode VisitElseDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ElseKeyword).Node;
			if (node.ElseKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEndIfDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EndKeyword).Node;
			if (node.EndKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.IfKeyword).Node;
			if (node.IfKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitRegionDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.RegionKeyword).Node;
			if (node.RegionKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)VisitToken(node.Name).Node;
			if (node.Name.Node != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitEndRegionDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EndKeyword).Node;
			if (node.EndKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.RegionKeyword).Node;
			if (node.RegionKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitExternalSourceDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ExternalSourceKeyword).Node;
			if (node.ExternalSourceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)VisitToken(node.ExternalSource).Node;
			if (node.ExternalSource.Node != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.CommaToken).Node;
			if (node.CommaToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			IntegerLiteralTokenSyntax integerLiteralTokenSyntax = (IntegerLiteralTokenSyntax)VisitToken(node.LineStart).Node;
			if (node.LineStart.Node != integerLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax2, stringLiteralTokenSyntax, punctuationSyntax3, integerLiteralTokenSyntax, punctuationSyntax4);
			}
			return node;
		}

		public override SyntaxNode VisitEndExternalSourceDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.EndExternalSourceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EndKeyword).Node;
			if (node.EndKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.ExternalSourceKeyword).Node;
			if (node.ExternalSourceKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndExternalSourceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override SyntaxNode VisitExternalChecksumDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ExternalChecksumKeyword).Node;
			if (node.ExternalChecksumKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitToken(node.OpenParenToken).Node;
			if (node.OpenParenToken.Node != punctuationSyntax2)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)VisitToken(node.ExternalSource).Node;
			if (node.ExternalSource.Node != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)VisitToken(node.FirstCommaToken).Node;
			if (node.FirstCommaToken.Node != punctuationSyntax3)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax2 = (StringLiteralTokenSyntax)VisitToken(node.Guid).Node;
			if (node.Guid.Node != stringLiteralTokenSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)VisitToken(node.SecondCommaToken).Node;
			if (node.SecondCommaToken.Node != punctuationSyntax4)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax3 = (StringLiteralTokenSyntax)VisitToken(node.Checksum).Node;
			if (node.Checksum.Node != stringLiteralTokenSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax5 = (PunctuationSyntax)VisitToken(node.CloseParenToken).Node;
			if (node.CloseParenToken.Node != punctuationSyntax5)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax2, stringLiteralTokenSyntax, punctuationSyntax3, stringLiteralTokenSyntax2, punctuationSyntax4, stringLiteralTokenSyntax3, punctuationSyntax5);
			}
			return node;
		}

		public override SyntaxNode VisitEnableWarningDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.EnableKeyword).Node;
			if (node.EnableKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.WarningKeyword).Node;
			if (node.WarningKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax> separatedSyntaxList = VisitList(node.ErrorCodes);
			if (node._errorCodes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitDisableWarningDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.DisableKeyword).Node;
			if (node.DisableKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)VisitToken(node.WarningKeyword).Node;
			if (node.WarningKeyword.Node != keywordSyntax2)
			{
				flag = true;
			}
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax> separatedSyntaxList = VisitList(node.ErrorCodes);
			if (node._errorCodes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override SyntaxNode VisitReferenceDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)VisitToken(node.ReferenceKeyword).Node;
			if (node.ReferenceKeyword.Node != keywordSyntax)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)VisitToken(node.File).Node;
			if (node.File.Node != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax);
			}
			return node;
		}

		public override SyntaxNode VisitBadDirectiveTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitToken(node.HashToken).Node;
			if (node.HashToken.Node != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public VisualBasicSyntaxRewriter(bool visitIntoStructuredTrivia = false)
		{
			_visitIntoStructuredTrivia = visitIntoStructuredTrivia;
		}

		public override SyntaxNode Visit(SyntaxNode node)
		{
			if (node != null)
			{
				_recursionDepth++;
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				SyntaxNode result = ((VisualBasicSyntaxNode)node).Accept(this);
				_recursionDepth--;
				return result;
			}
			return node;
		}

		public virtual SyntaxToken VisitToken(SyntaxToken token)
		{
			SyntaxTriviaList syntaxTriviaList = VisitList(token.LeadingTrivia);
			SyntaxTriviaList syntaxTriviaList2 = VisitList(token.TrailingTrivia);
			if (syntaxTriviaList != token.LeadingTrivia || syntaxTriviaList2 != token.TrailingTrivia)
			{
				if (syntaxTriviaList != token.LeadingTrivia)
				{
					token = token.WithLeadingTrivia(syntaxTriviaList);
				}
				if (syntaxTriviaList2 != token.TrailingTrivia)
				{
					token = token.WithTrailingTrivia(syntaxTriviaList2);
				}
			}
			return token;
		}

		public virtual SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
		{
			if (VisitIntoStructuredTrivia && trivia.HasStructure)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)trivia.GetStructure();
				Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax structuredTriviaSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax)Visit(visualBasicSyntaxNode);
				if (structuredTriviaSyntax != visualBasicSyntaxNode)
				{
					if (structuredTriviaSyntax != null)
					{
						return SyntaxFactory.Trivia(structuredTriviaSyntax);
					}
					return default(SyntaxTrivia);
				}
			}
			return trivia;
		}

		public virtual SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list) where TNode : SyntaxNode
		{
			SyntaxListBuilder syntaxListBuilder = null;
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				TNode val = list[i];
				TNode val2 = VisitListElement(val);
				if (val != val2 && syntaxListBuilder == null)
				{
					syntaxListBuilder = new SyntaxListBuilder(count);
					syntaxListBuilder.AddRange(list, 0, i);
				}
				if (syntaxListBuilder != null && val2 != null && VisualBasicExtensions.Kind(val2) != 0)
				{
					syntaxListBuilder.Add(val2);
				}
			}
			return syntaxListBuilder?.ToList<TNode>() ?? list;
		}

		public virtual TNode VisitListElement<TNode>(TNode node) where TNode : SyntaxNode
		{
			return (TNode)Visit(node);
		}

		public virtual SyntaxTokenList VisitList(SyntaxTokenList list)
		{
			SyntaxTokenListBuilder syntaxTokenListBuilder = null;
			int num = -1;
			int count = list.Count;
			SyntaxTokenList.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxToken current = enumerator.Current;
				num++;
				SyntaxToken syntaxToken = VisitListElement(current);
				if (current != syntaxToken && syntaxTokenListBuilder == null)
				{
					syntaxTokenListBuilder = new SyntaxTokenListBuilder(count);
					syntaxTokenListBuilder.Add(list, 0, num);
				}
				if (syntaxTokenListBuilder != null && VisualBasicExtensions.Kind(syntaxToken) != 0)
				{
					syntaxTokenListBuilder.Add(syntaxToken);
				}
			}
			return syntaxTokenListBuilder?.ToList() ?? list;
		}

		public virtual SyntaxToken VisitListElement(SyntaxToken token)
		{
			return VisitToken(token);
		}

		public virtual SeparatedSyntaxList<TNode> VisitList<TNode>(SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
		{
			int count = list.Count;
			int separatorCount = list.SeparatorCount;
			SeparatedSyntaxListBuilder<TNode> separatedSyntaxListBuilder = default(SeparatedSyntaxListBuilder<TNode>);
			int i;
			for (i = 0; i < separatorCount; i++)
			{
				TNode val = list[i];
				TNode val2 = VisitListElement(val);
				SyntaxToken separator = list.GetSeparator(i);
				SyntaxToken separatorToken = VisitListSeparator(separator);
				if (separatedSyntaxListBuilder.IsNull && (val != val2 || separator != separatorToken))
				{
					separatedSyntaxListBuilder = new SeparatedSyntaxListBuilder<TNode>(count);
					separatedSyntaxListBuilder.AddRange(in list, i);
				}
				if (separatedSyntaxListBuilder.IsNull)
				{
					continue;
				}
				if (val2 != null)
				{
					separatedSyntaxListBuilder.Add(val2);
					if (separatorToken.RawKind == 0)
					{
						throw new InvalidOperationException("separator is expected");
					}
					separatedSyntaxListBuilder.AddSeparator(in separatorToken);
				}
				else if (val2 == null)
				{
					throw new InvalidOperationException("element is expected");
				}
			}
			if (i < count)
			{
				TNode val3 = list[i];
				TNode val4 = VisitListElement(val3);
				if (separatedSyntaxListBuilder.IsNull && val3 != val4)
				{
					separatedSyntaxListBuilder = new SeparatedSyntaxListBuilder<TNode>(count);
					separatedSyntaxListBuilder.AddRange(in list, i);
				}
				if (!separatedSyntaxListBuilder.IsNull && val4 != null)
				{
					separatedSyntaxListBuilder.Add(val4);
				}
			}
			if (!separatedSyntaxListBuilder.IsNull)
			{
				return separatedSyntaxListBuilder.ToList();
			}
			return list;
		}

		public virtual SyntaxToken VisitListSeparator(SyntaxToken token)
		{
			return VisitToken(token);
		}

		public virtual SyntaxTriviaList VisitList(SyntaxTriviaList list)
		{
			int count = list.Count;
			if (count != 0)
			{
				SyntaxTriviaListBuilder syntaxTriviaListBuilder = null;
				int num = -1;
				SyntaxTriviaList.Enumerator enumerator = list.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia current = enumerator.Current;
					num++;
					SyntaxTrivia syntaxTrivia = VisitListElement(current);
					if (syntaxTrivia != current && syntaxTriviaListBuilder == null)
					{
						syntaxTriviaListBuilder = new SyntaxTriviaListBuilder(count);
						syntaxTriviaListBuilder.Add(in list, 0, num);
					}
					if (syntaxTriviaListBuilder != null && VisualBasicExtensions.Kind(syntaxTrivia) != 0)
					{
						syntaxTriviaListBuilder.Add(syntaxTrivia);
					}
				}
				if (syntaxTriviaListBuilder != null)
				{
					return syntaxTriviaListBuilder.ToList();
				}
			}
			return list;
		}

		public virtual SyntaxTrivia VisitListElement(SyntaxTrivia element)
		{
			return VisitTrivia(element);
		}
	}
}
