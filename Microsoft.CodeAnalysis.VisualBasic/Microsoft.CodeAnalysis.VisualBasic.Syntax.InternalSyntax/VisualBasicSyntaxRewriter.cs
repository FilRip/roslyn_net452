using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class VisualBasicSyntaxRewriter : VisualBasicSyntaxVisitor
	{
		public override VisualBasicSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Empty);
			if (node._empty != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EmptyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEndBlockStatement(EndBlockStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EndKeyword);
			if (node._endKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.BlockKeyword);
			if (node._blockKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new EndBlockStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax> syntaxList = VisitList(node.Options);
			if (node._options != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax> syntaxList2 = VisitList(node.Imports);
			if (node._imports != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax> syntaxList3 = VisitList(node.Attributes);
			if (node._attributes != syntaxList3.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList4 = VisitList(node.Members);
			if (node._members != syntaxList4.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EndOfFileToken);
			if (node._endOfFileToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CompilationUnitSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, syntaxList3.Node, syntaxList4.Node, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOptionStatement(OptionStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OptionKeyword);
			if (node._optionKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.NameKeyword);
			if (node._nameKeyword != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.ValueKeyword);
			if (node._valueKeyword != keywordSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new OptionStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitImportsStatement(ImportsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ImportsKeyword);
			if (node._importsKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ImportsClauseSyntax> separatedSyntaxList = VisitList(node.ImportsClauses);
			if (node._importsClauses != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ImportsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSimpleImportsClause(SimpleImportsClauseSyntax node)
		{
			bool flag = false;
			ImportAliasClauseSyntax importAliasClauseSyntax = (ImportAliasClauseSyntax)Visit(node._alias);
			if (node._alias != importAliasClauseSyntax)
			{
				flag = true;
			}
			NameSyntax nameSyntax = (NameSyntax)Visit(node._name);
			if (node._name != nameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SimpleImportsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), importAliasClauseSyntax, nameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitImportAliasClause(ImportAliasClauseSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ImportAliasClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlNamespaceImportsClause(XmlNamespaceImportsClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanToken);
			if (node._lessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlAttributeSyntax xmlAttributeSyntax = (XmlAttributeSyntax)Visit(node._xmlNamespace);
			if (node._xmlNamespace != xmlAttributeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.GreaterThanToken);
			if (node._greaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlNamespaceImportsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlAttributeSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			bool flag = false;
			NamespaceStatementSyntax namespaceStatementSyntax = (NamespaceStatementSyntax)Visit(node._namespaceStatement);
			if (node._namespaceStatement != namespaceStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Members);
			if (node._members != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endNamespaceStatement);
			if (node._endNamespaceStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NamespaceBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), namespaceStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNamespaceStatement(NamespaceStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NamespaceKeyword);
			if (node._namespaceKeyword != keywordSyntax)
			{
				flag = true;
			}
			NameSyntax nameSyntax = (NameSyntax)Visit(node._name);
			if (node._name != nameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NamespaceStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, nameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitModuleBlock(ModuleBlockSyntax node)
		{
			bool flag = false;
			ModuleStatementSyntax moduleStatementSyntax = (ModuleStatementSyntax)Visit(node._moduleStatement);
			if (node._moduleStatement != moduleStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endModuleStatement);
			if (node._endModuleStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ModuleBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), moduleStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitStructureBlock(StructureBlockSyntax node)
		{
			bool flag = false;
			StructureStatementSyntax structureStatementSyntax = (StructureStatementSyntax)Visit(node._structureStatement);
			if (node._structureStatement != structureStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endStructureStatement);
			if (node._endStructureStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new StructureBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), structureStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			bool flag = false;
			InterfaceStatementSyntax interfaceStatementSyntax = (InterfaceStatementSyntax)Visit(node._interfaceStatement);
			if (node._interfaceStatement != interfaceStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endInterfaceStatement);
			if (node._endInterfaceStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterfaceBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), interfaceStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitClassBlock(ClassBlockSyntax node)
		{
			bool flag = false;
			ClassStatementSyntax classStatementSyntax = (ClassStatementSyntax)Visit(node._classStatement);
			if (node._classStatement != classStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> syntaxList = VisitList(node.Inherits);
			if (node._inherits != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> syntaxList2 = VisitList(node.Implements);
			if (node._implements != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList3 = VisitList(node.Members);
			if (node._members != syntaxList3.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endClassStatement);
			if (node._endClassStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ClassBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), classStatementSyntax, syntaxList.Node, syntaxList2.Node, syntaxList3.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEnumBlock(EnumBlockSyntax node)
		{
			bool flag = false;
			EnumStatementSyntax enumStatementSyntax = (EnumStatementSyntax)Visit(node._enumStatement);
			if (node._enumStatement != enumStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Members);
			if (node._members != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endEnumStatement);
			if (node._endEnumStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EnumBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), enumStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInheritsStatement(InheritsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.InheritsKeyword);
			if (node._inheritsKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> separatedSyntaxList = VisitList(node.Types);
			if (node._types != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new InheritsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitImplementsStatement(ImplementsStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ImplementsKeyword);
			if (node._implementsKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> separatedSyntaxList = VisitList(node.Types);
			if (node._types != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ImplementsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitModuleStatement(ModuleStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ModuleKeyword);
			if (node._moduleKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ModuleStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitStructureStatement(StructureStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.StructureKeyword);
			if (node._structureKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new StructureStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterfaceStatement(InterfaceStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.InterfaceKeyword);
			if (node._interfaceKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterfaceStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitClassStatement(ClassStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ClassKeyword);
			if (node._classKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ClassStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEnumStatement(EnumStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EnumKeyword);
			if (node._enumKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)Visit(node._underlyingType);
			if (node._underlyingType != asClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EnumStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, asClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OfKeyword);
			if (node._ofKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax> separatedSyntaxList = VisitList(node.Parameters);
			if (node._parameters != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeParameterListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeParameter(TypeParameterSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.VarianceKeyword);
			if (node._varianceKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterConstraintClauseSyntax typeParameterConstraintClauseSyntax = (TypeParameterConstraintClauseSyntax)Visit(node._typeParameterConstraintClause);
			if (node._typeParameterConstraintClause != typeParameterConstraintClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeParameterSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierTokenSyntax, typeParameterConstraintClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeParameterSingleConstraintClause(TypeParameterSingleConstraintClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AsKeyword);
			if (node._asKeyword != keywordSyntax)
			{
				flag = true;
			}
			ConstraintSyntax constraintSyntax = (ConstraintSyntax)Visit(node._constraint);
			if (node._constraint != constraintSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeParameterSingleConstraintClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, constraintSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeParameterMultipleConstraintClause(TypeParameterMultipleConstraintClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AsKeyword);
			if (node._asKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenBraceToken);
			if (node._openBraceToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ConstraintSyntax> separatedSyntaxList = VisitList(node.Constraints);
			if (node._constraints != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseBraceToken);
			if (node._closeBraceToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeParameterMultipleConstraintClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSpecialConstraint(SpecialConstraintSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ConstraintKeyword);
			if (node._constraintKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SpecialConstraintSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeConstraint(TypeConstraintSyntax node)
		{
			bool flag = false;
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeConstraintSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)Visit(node._initializer);
			if (node._initializer != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EnumMemberDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, identifierTokenSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMethodBlock(MethodBlockSyntax node)
		{
			bool flag = false;
			MethodStatementSyntax methodStatementSyntax = (MethodStatementSyntax)Visit(node._subOrFunctionStatement);
			if (node._subOrFunctionStatement != methodStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endSubOrFunctionStatement);
			if (node._endSubOrFunctionStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MethodBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), methodStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			bool flag = false;
			SubNewStatementSyntax subNewStatementSyntax = (SubNewStatementSyntax)Visit(node._subNewStatement);
			if (node._subNewStatement != subNewStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endSubStatement);
			if (node._endSubStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ConstructorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), subNewStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOperatorBlock(OperatorBlockSyntax node)
		{
			bool flag = false;
			OperatorStatementSyntax operatorStatementSyntax = (OperatorStatementSyntax)Visit(node._operatorStatement);
			if (node._operatorStatement != operatorStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endOperatorStatement);
			if (node._endOperatorStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new OperatorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), operatorStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAccessorBlock(AccessorBlockSyntax node)
		{
			bool flag = false;
			AccessorStatementSyntax accessorStatementSyntax = (AccessorStatementSyntax)Visit(node._accessorStatement);
			if (node._accessorStatement != accessorStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endAccessorStatement);
			if (node._endAccessorStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AccessorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), accessorStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPropertyBlock(PropertyBlockSyntax node)
		{
			bool flag = false;
			PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)Visit(node._propertyStatement);
			if (node._propertyStatement != propertyStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax> syntaxList = VisitList(node.Accessors);
			if (node._accessors != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endPropertyStatement);
			if (node._endPropertyStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PropertyBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), propertyStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEventBlock(EventBlockSyntax node)
		{
			bool flag = false;
			EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)Visit(node._eventStatement);
			if (node._eventStatement != eventStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax> syntaxList = VisitList(node.Accessors);
			if (node._accessors != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endEventStatement);
			if (node._endEventStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EventBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), eventStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitParameterList(ParameterListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = VisitList(node.Parameters);
			if (node._parameters != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ParameterListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMethodStatement(MethodStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SubOrFunctionKeyword);
			if (node._subOrFunctionKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			HandlesClauseSyntax handlesClauseSyntax = (HandlesClauseSyntax)Visit(node._handlesClause);
			if (node._handlesClause != handlesClauseSyntax)
			{
				flag = true;
			}
			ImplementsClauseSyntax implementsClauseSyntax = (ImplementsClauseSyntax)Visit(node._implementsClause);
			if (node._implementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MethodStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSubNewStatement(SubNewStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SubKeyword);
			if (node._subKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.NewKeyword);
			if (node._newKeyword != keywordSyntax2)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SubNewStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, keywordSyntax2, parameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDeclareStatement(DeclareStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.DeclareKeyword);
			if (node._declareKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.CharsetKeyword);
			if (node._charsetKeyword != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.SubOrFunctionKeyword);
			if (node._subOrFunctionKeyword != keywordSyntax3)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)Visit(node.LibKeyword);
			if (node._libKeyword != keywordSyntax4)
			{
				flag = true;
			}
			LiteralExpressionSyntax literalExpressionSyntax = (LiteralExpressionSyntax)Visit(node._libraryName);
			if (node._libraryName != literalExpressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax5 = (KeywordSyntax)Visit(node.AliasKeyword);
			if (node._aliasKeyword != keywordSyntax5)
			{
				flag = true;
			}
			LiteralExpressionSyntax literalExpressionSyntax2 = (LiteralExpressionSyntax)Visit(node._aliasName);
			if (node._aliasName != literalExpressionSyntax2)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new DeclareStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, keywordSyntax2, keywordSyntax3, identifierTokenSyntax, keywordSyntax4, literalExpressionSyntax, keywordSyntax5, literalExpressionSyntax2, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDelegateStatement(DelegateStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.DelegateKeyword);
			if (node._delegateKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.SubOrFunctionKeyword);
			if (node._subOrFunctionKeyword != keywordSyntax2)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)Visit(node._typeParameterList);
			if (node._typeParameterList != typeParameterListSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new DelegateStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, keywordSyntax2, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEventStatement(EventStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.CustomKeyword);
			if (node._customKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.EventKeyword);
			if (node._eventKeyword != keywordSyntax2)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			ImplementsClauseSyntax implementsClauseSyntax = (ImplementsClauseSyntax)Visit(node._implementsClause);
			if (node._implementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EventStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, keywordSyntax2, identifierTokenSyntax, parameterListSyntax, simpleAsClauseSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOperatorStatement(OperatorStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OperatorKeyword);
			if (node._operatorKeyword != keywordSyntax)
			{
				flag = true;
			}
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.OperatorToken);
			if (node._operatorToken != syntaxToken)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new OperatorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, syntaxToken, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPropertyStatement(PropertyStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.PropertyKeyword);
			if (node._propertyKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)Visit(node._asClause);
			if (node._asClause != asClauseSyntax)
			{
				flag = true;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)Visit(node._initializer);
			if (node._initializer != equalsValueSyntax)
			{
				flag = true;
			}
			ImplementsClauseSyntax implementsClauseSyntax = (ImplementsClauseSyntax)Visit(node._implementsClause);
			if (node._implementsClause != implementsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PropertyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, identifierTokenSyntax, parameterListSyntax, asClauseSyntax, equalsValueSyntax, implementsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAccessorStatement(AccessorStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AccessorKeyword);
			if (node._accessorKeyword != keywordSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AccessorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, parameterListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitImplementsClause(ImplementsClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ImplementsKeyword);
			if (node._implementsKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<QualifiedNameSyntax> separatedSyntaxList = VisitList(node.InterfaceMembers);
			if (node._interfaceMembers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ImplementsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitHandlesClause(HandlesClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.HandlesKeyword);
			if (node._handlesKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<HandlesClauseItemSyntax> separatedSyntaxList = VisitList(node.Events);
			if (node._events != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new HandlesClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitKeywordEventContainer(KeywordEventContainerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new KeywordEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWithEventsEventContainer(WithEventsEventContainerSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WithEventsEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWithEventsPropertyEventContainer(WithEventsPropertyEventContainerSyntax node)
		{
			bool flag = false;
			WithEventsEventContainerSyntax withEventsEventContainerSyntax = (WithEventsEventContainerSyntax)Visit(node._withEventsContainer);
			if (node._withEventsContainer != withEventsEventContainerSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DotToken);
			if (node._dotToken != punctuationSyntax)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._property);
			if (node._property != identifierNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WithEventsPropertyEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), withEventsEventContainerSyntax, punctuationSyntax, identifierNameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitHandlesClauseItem(HandlesClauseItemSyntax node)
		{
			bool flag = false;
			EventContainerSyntax eventContainerSyntax = (EventContainerSyntax)Visit(node._eventContainer);
			if (node._eventContainer != eventContainerSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DotToken);
			if (node._dotToken != punctuationSyntax)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._eventMember);
			if (node._eventMember != identifierNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new HandlesClauseItemSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), eventContainerSyntax, punctuationSyntax, identifierNameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitIncompleteMember(IncompleteMemberSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.MissingIdentifier);
			if (node._missingIdentifier != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new IncompleteMemberSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, identifierTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Declarators);
			if (node._declarators != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new FieldDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ModifiedIdentifierSyntax> separatedSyntaxList = VisitList(node.Names);
			if (node._names != separatedSyntaxList.Node)
			{
				flag = true;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)Visit(node._asClause);
			if (node._asClause != asClauseSyntax)
			{
				flag = true;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)Visit(node._initializer);
			if (node._initializer != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new VariableDeclaratorSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), separatedSyntaxList.Node, asClauseSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSimpleAsClause(SimpleAsClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AsKeyword);
			if (node._asKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SimpleAsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAsNewClause(AsNewClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AsKeyword);
			if (node._asKeyword != keywordSyntax)
			{
				flag = true;
			}
			NewExpressionSyntax newExpressionSyntax = (NewExpressionSyntax)Visit(node._newExpression);
			if (node._newExpression != newExpressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AsNewClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, newExpressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitObjectMemberInitializer(ObjectMemberInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WithKeyword);
			if (node._withKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenBraceToken);
			if (node._openBraceToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FieldInitializerSyntax> separatedSyntaxList = VisitList(node.Initializers);
			if (node._initializers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseBraceToken);
			if (node._closeBraceToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ObjectMemberInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitObjectCollectionInitializer(ObjectCollectionInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.FromKeyword);
			if (node._fromKeyword != keywordSyntax)
			{
				flag = true;
			}
			CollectionInitializerSyntax collectionInitializerSyntax = (CollectionInitializerSyntax)Visit(node._initializer);
			if (node._initializer != collectionInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ObjectCollectionInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, collectionInitializerSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInferredFieldInitializer(InferredFieldInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.KeyKeyword);
			if (node._keyKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InferredFieldInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNamedFieldInitializer(NamedFieldInitializerSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.KeyKeyword);
			if (node._keyKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DotToken);
			if (node._dotToken != punctuationSyntax)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._name);
			if (node._name != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NamedFieldInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, identifierNameSyntax, punctuationSyntax2, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEqualsValue(EqualsValueSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._value);
			if (node._value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new EqualsValueSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitParameter(ParameterSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)Visit(node._identifier);
			if (node._identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)Visit(node._default);
			if (node._default != equalsValueSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ParameterSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, modifiedIdentifierSyntax, simpleAsClauseSyntax, equalsValueSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitModifiedIdentifier(ModifiedIdentifierSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Nullable);
			if (node._nullable != punctuationSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._arrayBounds);
			if (node._arrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> syntaxList = VisitList(node.ArrayRankSpecifiers);
			if (node._arrayRankSpecifiers != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ModifiedIdentifierSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, argumentListSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> syntaxList = VisitList(node.CommaTokens);
			if (node._commaTokens != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ArrayRankSpecifierSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAttributeList(AttributeListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanToken);
			if (node._lessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeSyntax> separatedSyntaxList = VisitList(node.Attributes);
			if (node._attributes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.GreaterThanToken);
			if (node._greaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new AttributeListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAttribute(AttributeSyntax node)
		{
			bool flag = false;
			AttributeTargetSyntax attributeTargetSyntax = (AttributeTargetSyntax)Visit(node._target);
			if (node._target != attributeTargetSyntax)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._name);
			if (node._name != typeSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._argumentList);
			if (node._argumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), attributeTargetSyntax, typeSyntax, argumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAttributeTarget(AttributeTargetSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AttributeModifier);
			if (node._attributeModifier != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.ColonToken);
			if (node._colonToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AttributeTargetSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAttributesStatement(AttributesStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new AttributesStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ExpressionStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPrintStatement(PrintStatementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.QuestionToken);
			if (node._questionToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PrintStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWhileBlock(WhileBlockSyntax node)
		{
			bool flag = false;
			WhileStatementSyntax whileStatementSyntax = (WhileStatementSyntax)Visit(node._whileStatement);
			if (node._whileStatement != whileStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endWhileStatement);
			if (node._endWhileStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WhileBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), whileStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitUsingBlock(UsingBlockSyntax node)
		{
			bool flag = false;
			UsingStatementSyntax usingStatementSyntax = (UsingStatementSyntax)Visit(node._usingStatement);
			if (node._usingStatement != usingStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endUsingStatement);
			if (node._endUsingStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new UsingBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), usingStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			bool flag = false;
			SyncLockStatementSyntax syncLockStatementSyntax = (SyncLockStatementSyntax)Visit(node._syncLockStatement);
			if (node._syncLockStatement != syncLockStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endSyncLockStatement);
			if (node._endSyncLockStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SyncLockBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syncLockStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWithBlock(WithBlockSyntax node)
		{
			bool flag = false;
			WithStatementSyntax withStatementSyntax = (WithStatementSyntax)Visit(node._withStatement);
			if (node._withStatement != withStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endWithStatement);
			if (node._endWithStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WithBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), withStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Declarators);
			if (node._declarators != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new LocalDeclarationStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLabelStatement(LabelStatementSyntax node)
		{
			bool flag = false;
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.LabelToken);
			if (node._labelToken != syntaxToken)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.ColonToken);
			if (node._colonToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new LabelStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGoToStatement(GoToStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GoToKeyword);
			if (node._goToKeyword != keywordSyntax)
			{
				flag = true;
			}
			LabelSyntax labelSyntax = (LabelSyntax)Visit(node._label);
			if (node._label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new GoToStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, labelSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLabel(LabelSyntax node)
		{
			bool flag = false;
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.LabelToken);
			if (node._labelToken != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new LabelSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitStopOrEndStatement(StopOrEndStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.StopOrEndKeyword);
			if (node._stopOrEndKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new StopOrEndStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitExitStatement(ExitStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ExitKeyword);
			if (node._exitKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.BlockKeyword);
			if (node._blockKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ExitStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ContinueKeyword);
			if (node._continueKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.BlockKeyword);
			if (node._blockKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ContinueStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ReturnKeyword);
			if (node._returnKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ReturnStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.IfKeyword);
			if (node._ifKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ThenKeyword);
			if (node._thenKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			SingleLineElseClauseSyntax singleLineElseClauseSyntax = (SingleLineElseClauseSyntax)Visit(node._elseClause);
			if (node._elseClause != singleLineElseClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SingleLineIfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2, syntaxList.Node, singleLineElseClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseKeyword);
			if (node._elseKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new SingleLineElseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			bool flag = false;
			IfStatementSyntax ifStatementSyntax = (IfStatementSyntax)Visit(node._ifStatement);
			if (node._ifStatement != ifStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ElseIfBlockSyntax> syntaxList2 = VisitList(node.ElseIfBlocks);
			if (node._elseIfBlocks != syntaxList2.Node)
			{
				flag = true;
			}
			ElseBlockSyntax elseBlockSyntax = (ElseBlockSyntax)Visit(node._elseBlock);
			if (node._elseBlock != elseBlockSyntax)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endIfStatement);
			if (node._endIfStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MultiLineIfBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), ifStatementSyntax, syntaxList.Node, syntaxList2.Node, elseBlockSyntax, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitIfStatement(IfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.IfKeyword);
			if (node._ifKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ThenKeyword);
			if (node._thenKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new IfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			bool flag = false;
			ElseIfStatementSyntax elseIfStatementSyntax = (ElseIfStatementSyntax)Visit(node._elseIfStatement);
			if (node._elseIfStatement != elseIfStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseIfBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), elseIfStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseIfStatement(ElseIfStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseIfKeyword);
			if (node._elseIfKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ThenKeyword);
			if (node._thenKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseIfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseBlock(ElseBlockSyntax node)
		{
			bool flag = false;
			ElseStatementSyntax elseStatementSyntax = (ElseStatementSyntax)Visit(node._elseStatement);
			if (node._elseStatement != elseStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), elseStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseStatement(ElseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseKeyword);
			if (node._elseKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTryBlock(TryBlockSyntax node)
		{
			bool flag = false;
			TryStatementSyntax tryStatementSyntax = (TryStatementSyntax)Visit(node._tryStatement);
			if (node._tryStatement != tryStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchBlockSyntax> syntaxList2 = VisitList(node.CatchBlocks);
			if (node._catchBlocks != syntaxList2.Node)
			{
				flag = true;
			}
			FinallyBlockSyntax finallyBlockSyntax = (FinallyBlockSyntax)Visit(node._finallyBlock);
			if (node._finallyBlock != finallyBlockSyntax)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endTryStatement);
			if (node._endTryStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TryBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), tryStatementSyntax, syntaxList.Node, syntaxList2.Node, finallyBlockSyntax, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTryStatement(TryStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.TryKeyword);
			if (node._tryKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TryStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCatchBlock(CatchBlockSyntax node)
		{
			bool flag = false;
			CatchStatementSyntax catchStatementSyntax = (CatchStatementSyntax)Visit(node._catchStatement);
			if (node._catchStatement != catchStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new CatchBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), catchStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCatchStatement(CatchStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.CatchKeyword);
			if (node._catchKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._identifierName);
			if (node._identifierName != identifierNameSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			CatchFilterClauseSyntax catchFilterClauseSyntax = (CatchFilterClauseSyntax)Visit(node._whenClause);
			if (node._whenClause != catchFilterClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CatchStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierNameSyntax, simpleAsClauseSyntax, catchFilterClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WhenKeyword);
			if (node._whenKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._filter);
			if (node._filter != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CatchFilterClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitFinallyBlock(FinallyBlockSyntax node)
		{
			bool flag = false;
			FinallyStatementSyntax finallyStatementSyntax = (FinallyStatementSyntax)Visit(node._finallyStatement);
			if (node._finallyStatement != finallyStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new FinallyBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), finallyStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitFinallyStatement(FinallyStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.FinallyKeyword);
			if (node._finallyKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new FinallyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitErrorStatement(ErrorStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ErrorKeyword);
			if (node._errorKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._errorNumber);
			if (node._errorNumber != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ErrorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OnKeyword);
			if (node._onKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ErrorKeyword);
			if (node._errorKeyword != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.GoToKeyword);
			if (node._goToKeyword != keywordSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Minus);
			if (node._minus != punctuationSyntax)
			{
				flag = true;
			}
			LabelSyntax labelSyntax = (LabelSyntax)Visit(node._label);
			if (node._label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new OnErrorGoToStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3, punctuationSyntax, labelSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OnKeyword);
			if (node._onKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ErrorKeyword);
			if (node._errorKeyword != keywordSyntax2)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.ResumeKeyword);
			if (node._resumeKeyword != keywordSyntax3)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)Visit(node.NextKeyword);
			if (node._nextKeyword != keywordSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new OnErrorResumeNextStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, keywordSyntax3, keywordSyntax4);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitResumeStatement(ResumeStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ResumeKeyword);
			if (node._resumeKeyword != keywordSyntax)
			{
				flag = true;
			}
			LabelSyntax labelSyntax = (LabelSyntax)Visit(node._label);
			if (node._label != labelSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ResumeStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, labelSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSelectBlock(SelectBlockSyntax node)
		{
			bool flag = false;
			SelectStatementSyntax selectStatementSyntax = (SelectStatementSyntax)Visit(node._selectStatement);
			if (node._selectStatement != selectStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CaseBlockSyntax> syntaxList = VisitList(node.CaseBlocks);
			if (node._caseBlocks != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endSelectStatement);
			if (node._endSelectStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SelectBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), selectStatementSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSelectStatement(SelectStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SelectKeyword);
			if (node._selectKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.CaseKeyword);
			if (node._caseKeyword != keywordSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SelectStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCaseBlock(CaseBlockSyntax node)
		{
			bool flag = false;
			CaseStatementSyntax caseStatementSyntax = (CaseStatementSyntax)Visit(node._caseStatement);
			if (node._caseStatement != caseStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new CaseBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), caseStatementSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCaseStatement(CaseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.CaseKeyword);
			if (node._caseKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CaseClauseSyntax> separatedSyntaxList = VisitList(node.Cases);
			if (node._cases != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new CaseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseCaseClause(ElseCaseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseKeyword);
			if (node._elseKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSimpleCaseClause(SimpleCaseClauseSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._value);
			if (node._value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SimpleCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRangeCaseClause(RangeCaseClauseSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._lowerBound);
			if (node._lowerBound != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ToKeyword);
			if (node._toKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._upperBound);
			if (node._upperBound != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new RangeCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRelationalCaseClause(RelationalCaseClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.IsKeyword);
			if (node._isKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OperatorToken);
			if (node._operatorToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._value);
			if (node._value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new RelationalCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSyncLockStatement(SyncLockStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SyncLockKeyword);
			if (node._syncLockKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SyncLockStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			bool flag = false;
			DoStatementSyntax doStatementSyntax = (DoStatementSyntax)Visit(node._doStatement);
			if (node._doStatement != doStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			LoopStatementSyntax loopStatementSyntax = (LoopStatementSyntax)Visit(node._loopStatement);
			if (node._loopStatement != loopStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new DoLoopBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), doStatementSyntax, syntaxList.Node, loopStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDoStatement(DoStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.DoKeyword);
			if (node._doKeyword != keywordSyntax)
			{
				flag = true;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = (WhileOrUntilClauseSyntax)Visit(node._whileOrUntilClause);
			if (node._whileOrUntilClause != whileOrUntilClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new DoStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLoopStatement(LoopStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.LoopKeyword);
			if (node._loopKeyword != keywordSyntax)
			{
				flag = true;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = (WhileOrUntilClauseSyntax)Visit(node._whileOrUntilClause);
			if (node._whileOrUntilClause != whileOrUntilClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new LoopStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWhileOrUntilClause(WhileOrUntilClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WhileOrUntilKeyword);
			if (node._whileOrUntilKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WhileOrUntilClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWhileStatement(WhileStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WhileKeyword);
			if (node._whileKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WhileStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitForBlock(ForBlockSyntax node)
		{
			bool flag = false;
			ForStatementSyntax forStatementSyntax = (ForStatementSyntax)Visit(node._forStatement);
			if (node._forStatement != forStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			NextStatementSyntax nextStatementSyntax = (NextStatementSyntax)Visit(node._nextStatement);
			if (node._nextStatement != nextStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ForBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), forStatementSyntax, syntaxList.Node, nextStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitForEachBlock(ForEachBlockSyntax node)
		{
			bool flag = false;
			ForEachStatementSyntax forEachStatementSyntax = (ForEachStatementSyntax)Visit(node._forEachStatement);
			if (node._forEachStatement != forEachStatementSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			NextStatementSyntax nextStatementSyntax = (NextStatementSyntax)Visit(node._nextStatement);
			if (node._nextStatement != nextStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ForEachBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), forEachStatementSyntax, syntaxList.Node, nextStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitForStatement(ForStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ForKeyword);
			if (node._forKeyword != keywordSyntax)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = Visit(node._controlVariable);
			if (node._controlVariable != visualBasicSyntaxNode)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._fromValue);
			if (node._fromValue != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ToKeyword);
			if (node._toKeyword != keywordSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._toValue);
			if (node._toValue != expressionSyntax2)
			{
				flag = true;
			}
			ForStepClauseSyntax forStepClauseSyntax = (ForStepClauseSyntax)Visit(node._stepClause);
			if (node._stepClause != forStepClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ForStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, visualBasicSyntaxNode, punctuationSyntax, expressionSyntax, keywordSyntax2, expressionSyntax2, forStepClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitForStepClause(ForStepClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.StepKeyword);
			if (node._stepKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._stepValue);
			if (node._stepValue != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ForStepClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ForKeyword);
			if (node._forKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.EachKeyword);
			if (node._eachKeyword != keywordSyntax2)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = Visit(node._controlVariable);
			if (node._controlVariable != visualBasicSyntaxNode)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.InKeyword);
			if (node._inKeyword != keywordSyntax3)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ForEachStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, visualBasicSyntaxNode, keywordSyntax3, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNextStatement(NextStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NextKeyword);
			if (node._nextKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList = VisitList(node.ControlVariables);
			if (node._controlVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new NextStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitUsingStatement(UsingStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.UsingKeyword);
			if (node._usingKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new UsingStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ThrowKeyword);
			if (node._throwKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ThrowStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAssignmentStatement(AssignmentStatementSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._left);
			if (node._left != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OperatorToken);
			if (node._operatorToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._right);
			if (node._right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new AssignmentStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMidExpression(MidExpressionSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Mid);
			if (node._mid != identifierTokenSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._argumentList);
			if (node._argumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MidExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, argumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCallStatement(CallStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.CallKeyword);
			if (node._callKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._invocation);
			if (node._invocation != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CallStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAddRemoveHandlerStatement(AddRemoveHandlerStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AddHandlerOrRemoveHandlerKeyword);
			if (node._addHandlerOrRemoveHandlerKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._eventExpression);
			if (node._eventExpression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._delegateExpression);
			if (node._delegateExpression != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new AddRemoveHandlerStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRaiseEventStatement(RaiseEventStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.RaiseEventKeyword);
			if (node._raiseEventKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._name);
			if (node._name != identifierNameSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._argumentList);
			if (node._argumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new RaiseEventStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierNameSyntax, argumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWithStatement(WithStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WithKeyword);
			if (node._withKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WithStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitReDimStatement(ReDimStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ReDimKeyword);
			if (node._reDimKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.PreserveKeyword);
			if (node._preserveKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<RedimClauseSyntax> separatedSyntaxList = VisitList(node.Clauses);
			if (node._clauses != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ReDimStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRedimClause(RedimClauseSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._arrayBounds);
			if (node._arrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new RedimClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, argumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEraseStatement(EraseStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EraseKeyword);
			if (node._eraseKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList = VisitList(node.Expressions);
			if (node._expressions != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new EraseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			bool flag = false;
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.Token);
			if (node._token != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new LiteralExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ParenthesizedExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTupleExpression(TupleExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SimpleArgumentSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new TupleExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTupleType(TupleTypeSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TupleElementSyntax> separatedSyntaxList = VisitList(node.Elements);
			if (node._elements != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new TupleTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypedTupleElement(TypedTupleElementSyntax node)
		{
			bool flag = false;
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypedTupleElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNamedTupleElement(NamedTupleElementSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NamedTupleElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMeExpression(MeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMyBaseExpression(MyBaseExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MyBaseExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMyClassExpression(MyClassExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MyClassExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGetTypeExpression(GetTypeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GetTypeKeyword);
			if (node._getTypeKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new GetTypeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, typeSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.TypeOfKeyword);
			if (node._typeOfKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.OperatorToken);
			if (node._operatorToken != keywordSyntax2)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeOfExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax2, typeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGetXmlNamespaceExpression(GetXmlNamespaceExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GetXmlNamespaceKeyword);
			if (node._getXmlNamespaceKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlPrefixNameSyntax xmlPrefixNameSyntax = (XmlPrefixNameSyntax)Visit(node._name);
			if (node._name != xmlPrefixNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new GetXmlNamespaceExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, xmlPrefixNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OperatorToken);
			if (node._operatorToken != punctuationSyntax)
			{
				flag = true;
			}
			SimpleNameSyntax simpleNameSyntax = (SimpleNameSyntax)Visit(node._name);
			if (node._name != simpleNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MemberAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, simpleNameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlMemberAccessExpression(XmlMemberAccessExpressionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._base);
			if (node._base != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Token1);
			if (node._token1 != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.Token2);
			if (node._token2 != punctuationSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.Token3);
			if (node._token3 != punctuationSyntax3)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node._name);
			if (node._name != xmlNodeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlMemberAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, punctuationSyntax2, punctuationSyntax3, xmlNodeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._argumentList);
			if (node._argumentList != argumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InvocationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, argumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NewKeyword);
			if (node._newKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._argumentList);
			if (node._argumentList != argumentListSyntax)
			{
				flag = true;
			}
			ObjectCreationInitializerSyntax objectCreationInitializerSyntax = (ObjectCreationInitializerSyntax)Visit(node._initializer);
			if (node._initializer != objectCreationInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ObjectCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, objectCreationInitializerSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NewKeyword);
			if (node._newKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			ObjectMemberInitializerSyntax objectMemberInitializerSyntax = (ObjectMemberInitializerSyntax)Visit(node._initializer);
			if (node._initializer != objectMemberInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AnonymousObjectCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, objectMemberInitializerSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NewKeyword);
			if (node._newKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)Visit(node._arrayBounds);
			if (node._arrayBounds != argumentListSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> syntaxList2 = VisitList(node.RankSpecifiers);
			if (node._rankSpecifiers != syntaxList2.Node)
			{
				flag = true;
			}
			CollectionInitializerSyntax collectionInitializerSyntax = (CollectionInitializerSyntax)Visit(node._initializer);
			if (node._initializer != collectionInitializerSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ArrayCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, syntaxList2.Node, collectionInitializerSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCollectionInitializer(CollectionInitializerSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenBraceToken);
			if (node._openBraceToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList = VisitList(node.Initializers);
			if (node._initializers != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseBraceToken);
			if (node._closeBraceToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new CollectionInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCTypeExpression(CTypeExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax2)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new CTypeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDirectCastExpression(DirectCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax2)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new DirectCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTryCastExpression(TryCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax2)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new TryCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, typeSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new PredefinedCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._left);
			if (node._left != expressionSyntax)
			{
				flag = true;
			}
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.OperatorToken);
			if (node._operatorToken != syntaxToken)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._right);
			if (node._right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new BinaryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, syntaxToken, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitUnaryExpression(UnaryExpressionSyntax node)
		{
			bool flag = false;
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.OperatorToken);
			if (node._operatorToken != syntaxToken)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._operand);
			if (node._operand != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new UnaryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitBinaryConditionalExpression(BinaryConditionalExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.IfKeyword);
			if (node._ifKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._firstExpression);
			if (node._firstExpression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._secondExpression);
			if (node._secondExpression != expressionSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new BinaryConditionalExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, expressionSyntax2, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTernaryConditionalExpression(TernaryConditionalExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.IfKeyword);
			if (node._ifKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.FirstCommaToken);
			if (node._firstCommaToken != punctuationSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._whenTrue);
			if (node._whenTrue != expressionSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.SecondCommaToken);
			if (node._secondCommaToken != punctuationSyntax3)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax3 = (ExpressionSyntax)Visit(node._whenFalse);
			if (node._whenFalse != expressionSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new TernaryConditionalExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2, expressionSyntax2, punctuationSyntax3, expressionSyntax3, punctuationSyntax4);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSingleLineLambdaExpression(SingleLineLambdaExpressionSyntax node)
		{
			bool flag = false;
			LambdaHeaderSyntax lambdaHeaderSyntax = (LambdaHeaderSyntax)Visit(node._subOrFunctionHeader);
			if (node._subOrFunctionHeader != lambdaHeaderSyntax)
			{
				flag = true;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = Visit(node._body);
			if (node._body != visualBasicSyntaxNode)
			{
				flag = true;
			}
			if (flag)
			{
				return new SingleLineLambdaExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), lambdaHeaderSyntax, visualBasicSyntaxNode);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			bool flag = false;
			LambdaHeaderSyntax lambdaHeaderSyntax = (LambdaHeaderSyntax)Visit(node._subOrFunctionHeader);
			if (node._subOrFunctionHeader != lambdaHeaderSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = VisitList(node.Statements);
			if (node._statements != syntaxList.Node)
			{
				flag = true;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)Visit(node._endSubOrFunctionStatement);
			if (node._endSubOrFunctionStatement != endBlockStatementSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new MultiLineLambdaExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), lambdaHeaderSyntax, syntaxList.Node, endBlockStatementSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLambdaHeader(LambdaHeaderSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = VisitList(node.AttributeLists);
			if (node._attributeLists != syntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList2 = VisitList(node.Modifiers);
			if (node._modifiers != syntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SubOrFunctionKeyword);
			if (node._subOrFunctionKeyword != keywordSyntax)
			{
				flag = true;
			}
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)Visit(node._parameterList);
			if (node._parameterList != parameterListSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new LambdaHeaderSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList2.Node, keywordSyntax, parameterListSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitArgumentList(ArgumentListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ArgumentListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOmittedArgument(OmittedArgumentSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Empty);
			if (node._empty != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new OmittedArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSimpleArgument(SimpleArgumentSyntax node)
		{
			bool flag = false;
			NameColonEqualsSyntax nameColonEqualsSyntax = (NameColonEqualsSyntax)Visit(node._nameColonEquals);
			if (node._nameColonEquals != nameColonEqualsSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new SimpleArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameColonEqualsSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNameColonEquals(NameColonEqualsSyntax node)
		{
			bool flag = false;
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._name);
			if (node._name != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.ColonEqualsToken);
			if (node._colonEqualsToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NameColonEqualsSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierNameSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRangeArgument(RangeArgumentSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._lowerBound);
			if (node._lowerBound != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ToKeyword);
			if (node._toKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._upperBound);
			if (node._upperBound != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new RangeArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitQueryExpression(QueryExpressionSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> syntaxList = VisitList(node.Clauses);
			if (node._clauses != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new QueryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCollectionRangeVariable(CollectionRangeVariableSyntax node)
		{
			bool flag = false;
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)Visit(node._identifier);
			if (node._identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.InKeyword);
			if (node._inKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CollectionRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitExpressionRangeVariable(ExpressionRangeVariableSyntax node)
		{
			bool flag = false;
			VariableNameEqualsSyntax variableNameEqualsSyntax = (VariableNameEqualsSyntax)Visit(node._nameEquals);
			if (node._nameEquals != variableNameEqualsSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ExpressionRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), variableNameEqualsSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAggregationRangeVariable(AggregationRangeVariableSyntax node)
		{
			bool flag = false;
			VariableNameEqualsSyntax variableNameEqualsSyntax = (VariableNameEqualsSyntax)Visit(node._nameEquals);
			if (node._nameEquals != variableNameEqualsSyntax)
			{
				flag = true;
			}
			AggregationSyntax aggregationSyntax = (AggregationSyntax)Visit(node._aggregation);
			if (node._aggregation != aggregationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AggregationRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), variableNameEqualsSyntax, aggregationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitVariableNameEquals(VariableNameEqualsSyntax node)
		{
			bool flag = false;
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)Visit(node._identifier);
			if (node._identifier != modifiedIdentifierSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new VariableNameEqualsSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitFunctionAggregation(FunctionAggregationSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.FunctionName);
			if (node._functionName != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._argument);
			if (node._argument != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new FunctionAggregationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGroupAggregation(GroupAggregationSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GroupKeyword);
			if (node._groupKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new GroupAggregationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitFromClause(FromClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.FromKeyword);
			if (node._fromKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new FromClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitLetClause(LetClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.LetKeyword);
			if (node._letKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new LetClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAggregateClause(AggregateClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AggregateKeyword);
			if (node._aggregateKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> syntaxList = VisitList(node.AdditionalQueryOperators);
			if (node._additionalQueryOperators != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.IntoKeyword);
			if (node._intoKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList2 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new AggregateClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDistinctClause(DistinctClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.DistinctKeyword);
			if (node._distinctKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new DistinctClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitWhereClause(WhereClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.WhereKeyword);
			if (node._whereKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new WhereClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPartitionWhileClause(PartitionWhileClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SkipOrTakeKeyword);
			if (node._skipOrTakeKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.WhileKeyword);
			if (node._whileKeyword != keywordSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PartitionWhileClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPartitionClause(PartitionClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SkipOrTakeKeyword);
			if (node._skipOrTakeKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._count);
			if (node._count != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PartitionClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGroupByClause(GroupByClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GroupKeyword);
			if (node._groupKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Items);
			if (node._items != separatedSyntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ByKeyword);
			if (node._byKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList2 = VisitList(node.Keys);
			if (node._keys != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.IntoKeyword);
			if (node._intoKeyword != keywordSyntax3)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList3 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList3.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new GroupByClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node, keywordSyntax3, separatedSyntaxList3.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitJoinCondition(JoinConditionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._left);
			if (node._left != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EqualsKeyword);
			if (node._equalsKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._right);
			if (node._right != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new JoinConditionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSimpleJoinClause(SimpleJoinClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.JoinKeyword);
			if (node._joinKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.JoinedVariables);
			if (node._joinedVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinClauseSyntax> syntaxList = VisitList(node.AdditionalJoins);
			if (node._additionalJoins != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.OnKeyword);
			if (node._onKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> separatedSyntaxList2 = VisitList(node.JoinConditions);
			if (node._joinConditions != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new SimpleJoinClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax2, separatedSyntaxList2.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGroupJoinClause(GroupJoinClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GroupKeyword);
			if (node._groupKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.JoinKeyword);
			if (node._joinKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList = VisitList(node.JoinedVariables);
			if (node._joinedVariables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinClauseSyntax> syntaxList = VisitList(node.AdditionalJoins);
			if (node._additionalJoins != syntaxList.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.OnKeyword);
			if (node._onKeyword != keywordSyntax3)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> separatedSyntaxList2 = VisitList(node.JoinConditions);
			if (node._joinConditions != separatedSyntaxList2.Node)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)Visit(node.IntoKeyword);
			if (node._intoKeyword != keywordSyntax4)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList3 = VisitList(node.AggregationVariables);
			if (node._aggregationVariables != separatedSyntaxList3.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new GroupJoinClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax3, separatedSyntaxList2.Node, keywordSyntax4, separatedSyntaxList3.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOrderByClause(OrderByClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OrderKeyword);
			if (node._orderKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ByKeyword);
			if (node._byKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> separatedSyntaxList = VisitList(node.Orderings);
			if (node._orderings != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new OrderByClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitOrdering(OrderingSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AscendingOrDescendingKeyword);
			if (node._ascendingOrDescendingKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new OrderingSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSelectClause(SelectClauseSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.SelectKeyword);
			if (node._selectKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList = VisitList(node.Variables);
			if (node._variables != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new SelectClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlDocument(XmlDocumentSyntax node)
		{
			bool flag = false;
			XmlDeclarationSyntax xmlDeclarationSyntax = (XmlDeclarationSyntax)Visit(node._declaration);
			if (node._declaration != xmlDeclarationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.PrecedingMisc);
			if (node._precedingMisc != syntaxList.Node)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node._root);
			if (node._root != xmlNodeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList2 = VisitList(node.FollowingMisc);
			if (node._followingMisc != syntaxList2.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlDocumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlDeclarationSyntax, syntaxList.Node, xmlNodeSyntax, syntaxList2.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlDeclaration(XmlDeclarationSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanQuestionToken);
			if (node._lessThanQuestionToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.XmlKeyword);
			if (node._xmlKeyword != keywordSyntax)
			{
				flag = true;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = (XmlDeclarationOptionSyntax)Visit(node._version);
			if (node._version != xmlDeclarationOptionSyntax)
			{
				flag = true;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax2 = (XmlDeclarationOptionSyntax)Visit(node._encoding);
			if (node._encoding != xmlDeclarationOptionSyntax2)
			{
				flag = true;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax3 = (XmlDeclarationOptionSyntax)Visit(node._standalone);
			if (node._standalone != xmlDeclarationOptionSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.QuestionGreaterThanToken);
			if (node._questionGreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, xmlDeclarationOptionSyntax, xmlDeclarationOptionSyntax2, xmlDeclarationOptionSyntax3, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlDeclarationOption(XmlDeclarationOptionSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Visit(node.Name);
			if (node._name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.Equals);
			if (node._equals != punctuationSyntax)
			{
				flag = true;
			}
			XmlStringSyntax xmlStringSyntax = (XmlStringSyntax)Visit(node._value);
			if (node._value != xmlStringSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlDeclarationOptionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax, xmlStringSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlElement(XmlElementSyntax node)
		{
			bool flag = false;
			XmlElementStartTagSyntax xmlElementStartTagSyntax = (XmlElementStartTagSyntax)Visit(node._startTag);
			if (node._startTag != xmlElementStartTagSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Content);
			if (node._content != syntaxList.Node)
			{
				flag = true;
			}
			XmlElementEndTagSyntax xmlElementEndTagSyntax = (XmlElementEndTagSyntax)Visit(node._endTag);
			if (node._endTag != xmlElementEndTagSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlElementStartTagSyntax, syntaxList.Node, xmlElementEndTagSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlText(XmlTextSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = VisitList(node.TextTokens);
			if (node._textTokens != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlTextSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlElementStartTag(XmlElementStartTagSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanToken);
			if (node._lessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node._name);
			if (node._name != xmlNodeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node._attributes != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.GreaterThanToken);
			if (node._greaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlElementStartTagSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlElementEndTag(XmlElementEndTagSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanSlashToken);
			if (node._lessThanSlashToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node._name);
			if (node._name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.GreaterThanToken);
			if (node._greaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlElementEndTagSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlEmptyElement(XmlEmptyElementSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanToken);
			if (node._lessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node._name);
			if (node._name != xmlNodeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node._attributes != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.SlashGreaterThanToken);
			if (node._slashGreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlEmptyElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlAttribute(XmlAttributeSyntax node)
		{
			bool flag = false;
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node._name);
			if (node._name != xmlNodeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax2 = (XmlNodeSyntax)Visit(node._value);
			if (node._value != xmlNodeSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlString(XmlStringSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.StartQuoteToken);
			if (node._startQuoteToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = VisitList(node.TextTokens);
			if (node._textTokens != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.EndQuoteToken);
			if (node._endQuoteToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlStringSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlPrefixName(XmlPrefixNameSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Visit(node.Name);
			if (node._name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlPrefixNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlName(XmlNameSyntax node)
		{
			bool flag = false;
			XmlPrefixSyntax xmlPrefixSyntax = (XmlPrefixSyntax)Visit(node._prefix);
			if (node._prefix != xmlPrefixSyntax)
			{
				flag = true;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Visit(node.LocalName);
			if (node._localName != xmlNameTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, xmlNameTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlBracketedName(XmlBracketedNameSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanToken);
			if (node._lessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node._name);
			if (node._name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.GreaterThanToken);
			if (node._greaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlBracketedNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlPrefix(XmlPrefixSyntax node)
		{
			bool flag = false;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Visit(node.Name);
			if (node._name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.ColonToken);
			if (node._colonToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlPrefixSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlComment(XmlCommentSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanExclamationMinusMinusToken);
			if (node._lessThanExclamationMinusMinusToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = VisitList(node.TextTokens);
			if (node._textTokens != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.MinusMinusGreaterThanToken);
			if (node._minusMinusGreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlCommentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanQuestionToken);
			if (node._lessThanQuestionToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Visit(node.Name);
			if (node._name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = VisitList(node.TextTokens);
			if (node._textTokens != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.QuestionGreaterThanToken);
			if (node._questionGreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlProcessingInstructionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameTokenSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlCDataSection(XmlCDataSectionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.BeginCDataToken);
			if (node._beginCDataToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = VisitList(node.TextTokens);
			if (node._textTokens != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.EndCDataToken);
			if (node._endCDataToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlCDataSectionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlEmbeddedExpression(XmlEmbeddedExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanPercentEqualsToken);
			if (node._lessThanPercentEqualsToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.PercentGreaterThanToken);
			if (node._percentGreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlEmbeddedExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitArrayType(ArrayTypeSyntax node)
		{
			bool flag = false;
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._elementType);
			if (node._elementType != typeSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> syntaxList = VisitList(node.RankSpecifiers);
			if (node._rankSpecifiers != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new ArrayTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNullableType(NullableTypeSyntax node)
		{
			bool flag = false;
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._elementType);
			if (node._elementType != typeSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.QuestionMarkToken);
			if (node._questionMarkToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new NullableTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, punctuationSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Keyword);
			if (node._keyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new PredefinedTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new IdentifierNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGenericName(GenericNameSyntax node)
		{
			bool flag = false;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Identifier);
			if (node._identifier != identifierTokenSyntax)
			{
				flag = true;
			}
			TypeArgumentListSyntax typeArgumentListSyntax = (TypeArgumentListSyntax)Visit(node._typeArgumentList);
			if (node._typeArgumentList != typeArgumentListSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new GenericNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, typeArgumentListSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
		{
			bool flag = false;
			NameSyntax nameSyntax = (NameSyntax)Visit(node._left);
			if (node._left != nameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DotToken);
			if (node._dotToken != punctuationSyntax)
			{
				flag = true;
			}
			SimpleNameSyntax simpleNameSyntax = (SimpleNameSyntax)Visit(node._right);
			if (node._right != simpleNameSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new QualifiedNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameSyntax, punctuationSyntax, simpleNameSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitGlobalName(GlobalNameSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.GlobalKeyword);
			if (node._globalKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new GlobalNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitTypeArgumentList(TypeArgumentListSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OfKeyword);
			if (node._ofKeyword != keywordSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> separatedSyntaxList = VisitList(node.Arguments);
			if (node._arguments != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new TypeArgumentListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCrefReference(CrefReferenceSyntax node)
		{
			bool flag = false;
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._name);
			if (node._name != typeSyntax)
			{
				flag = true;
			}
			CrefSignatureSyntax crefSignatureSyntax = (CrefSignatureSyntax)Visit(node._signature);
			if (node._signature != crefSignatureSyntax)
			{
				flag = true;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)Visit(node._asClause);
			if (node._asClause != simpleAsClauseSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CrefReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, crefSignatureSyntax, simpleAsClauseSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCrefSignature(CrefSignatureSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefSignaturePartSyntax> separatedSyntaxList = VisitList(node.ArgumentTypes);
			if (node._argumentTypes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new CrefSignatureSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCrefSignaturePart(CrefSignaturePartSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.Modifier);
			if (node._modifier != keywordSyntax)
			{
				flag = true;
			}
			TypeSyntax typeSyntax = (TypeSyntax)Visit(node._type);
			if (node._type != typeSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new CrefSignaturePartSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, typeSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitCrefOperatorReference(CrefOperatorReferenceSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.OperatorKeyword);
			if (node._operatorKeyword != keywordSyntax)
			{
				flag = true;
			}
			SyntaxToken syntaxToken = (SyntaxToken)Visit(node.OperatorToken);
			if (node._operatorToken != syntaxToken)
			{
				flag = true;
			}
			if (flag)
			{
				return new CrefOperatorReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitQualifiedCrefOperatorReference(QualifiedCrefOperatorReferenceSyntax node)
		{
			bool flag = false;
			NameSyntax nameSyntax = (NameSyntax)Visit(node._left);
			if (node._left != nameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DotToken);
			if (node._dotToken != punctuationSyntax)
			{
				flag = true;
			}
			CrefOperatorReferenceSyntax crefOperatorReferenceSyntax = (CrefOperatorReferenceSyntax)Visit(node._right);
			if (node._right != crefOperatorReferenceSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new QualifiedCrefOperatorReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameSyntax, punctuationSyntax, crefOperatorReferenceSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitYieldStatement(YieldStatementSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.YieldKeyword);
			if (node._yieldKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new YieldStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.AwaitKeyword);
			if (node._awaitKeyword != keywordSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new AwaitExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = VisitList(node.Tokens);
			if (node._tokens != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new SkippedTokensTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			bool flag = false;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Content);
			if (node._content != syntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new DocumentationCommentTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
		{
			bool flag = false;
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node._name);
			if (node._name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.StartQuoteToken);
			if (node._startQuoteToken != punctuationSyntax2)
			{
				flag = true;
			}
			CrefReferenceSyntax crefReferenceSyntax = (CrefReferenceSyntax)Visit(node._reference);
			if (node._reference != crefReferenceSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.EndQuoteToken);
			if (node._endQuoteToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlCrefAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax2, crefReferenceSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlNameAttribute(XmlNameAttributeSyntax node)
		{
			bool flag = false;
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node._name);
			if (node._name != xmlNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.StartQuoteToken);
			if (node._startQuoteToken != punctuationSyntax2)
			{
				flag = true;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)Visit(node._reference);
			if (node._reference != identifierNameSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.EndQuoteToken);
			if (node._endQuoteToken != punctuationSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new XmlNameAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax2, identifierNameSyntax, punctuationSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
		{
			bool flag = false;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.QuestionMarkToken);
			if (node._questionMarkToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)Visit(node._whenNotNull);
			if (node._whenNotNull != expressionSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new ConditionalAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitNameOfExpression(NameOfExpressionSyntax node)
		{
			bool flag = false;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.NameOfKeyword);
			if (node._nameOfKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._argument);
			if (node._argument != expressionSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new NameOfExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.DollarSignDoubleQuoteToken);
			if (node._dollarSignDoubleQuoteToken != punctuationSyntax)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax> syntaxList = VisitList(node.Contents);
			if (node._contents != syntaxList.Node)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.DoubleQuoteToken);
			if (node._doubleQuoteToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterpolatedStringExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
		{
			bool flag = false;
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)Visit(node.TextToken);
			if (node._textToken != interpolatedStringTextTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterpolatedStringTextSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), interpolatedStringTextTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterpolation(InterpolationSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.OpenBraceToken);
			if (node._openBraceToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._expression);
			if (node._expression != expressionSyntax)
			{
				flag = true;
			}
			InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = (InterpolationAlignmentClauseSyntax)Visit(node._alignmentClause);
			if (node._alignmentClause != interpolationAlignmentClauseSyntax)
			{
				flag = true;
			}
			InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = (InterpolationFormatClauseSyntax)Visit(node._formatClause);
			if (node._formatClause != interpolationFormatClauseSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.CloseBraceToken);
			if (node._closeBraceToken != punctuationSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterpolationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, interpolationAlignmentClauseSyntax, interpolationFormatClauseSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._value);
			if (node._value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterpolationAlignmentClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.ColonToken);
			if (node._colonToken != punctuationSyntax)
			{
				flag = true;
			}
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)Visit(node.FormatStringToken);
			if (node._formatStringToken != interpolatedStringTextTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new InterpolationFormatClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, interpolatedStringTextTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitConstDirectiveTrivia(ConstDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ConstKeyword);
			if (node._constKeyword != keywordSyntax)
			{
				flag = true;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)Visit(node.Name);
			if (node._name != identifierTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.EqualsToken);
			if (node._equalsToken != punctuationSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._value);
			if (node._value != expressionSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ConstDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, identifierTokenSyntax, punctuationSyntax2, expressionSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseKeyword);
			if (node._elseKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.IfOrElseIfKeyword);
			if (node._ifOrElseIfKeyword != keywordSyntax2)
			{
				flag = true;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)Visit(node._condition);
			if (node._condition != expressionSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)Visit(node.ThenKeyword);
			if (node._thenKeyword != keywordSyntax3)
			{
				flag = true;
			}
			if (flag)
			{
				return new IfDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, expressionSyntax, keywordSyntax3);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ElseKeyword);
			if (node._elseKeyword != keywordSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ElseDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EndKeyword);
			if (node._endKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.IfKeyword);
			if (node._ifKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new EndIfDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.RegionKeyword);
			if (node._regionKeyword != keywordSyntax)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)Visit(node.Name);
			if (node._name != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new RegionDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EndKeyword);
			if (node._endKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.RegionKeyword);
			if (node._regionKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new EndRegionDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ExternalSourceKeyword);
			if (node._externalSourceKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)Visit(node.ExternalSource);
			if (node._externalSource != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.CommaToken);
			if (node._commaToken != punctuationSyntax3)
			{
				flag = true;
			}
			IntegerLiteralTokenSyntax integerLiteralTokenSyntax = (IntegerLiteralTokenSyntax)Visit(node.LineStart);
			if (node._lineStart != integerLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax4)
			{
				flag = true;
			}
			if (flag)
			{
				return new ExternalSourceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax2, stringLiteralTokenSyntax, punctuationSyntax3, integerLiteralTokenSyntax, punctuationSyntax4);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEndExternalSourceDirectiveTrivia(EndExternalSourceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EndKeyword);
			if (node._endKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.ExternalSourceKeyword);
			if (node._externalSourceKeyword != keywordSyntax2)
			{
				flag = true;
			}
			if (flag)
			{
				return new EndExternalSourceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitExternalChecksumDirectiveTrivia(ExternalChecksumDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ExternalChecksumKeyword);
			if (node._externalChecksumKeyword != keywordSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Visit(node.OpenParenToken);
			if (node._openParenToken != punctuationSyntax2)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)Visit(node.ExternalSource);
			if (node._externalSource != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)Visit(node.FirstCommaToken);
			if (node._firstCommaToken != punctuationSyntax3)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax2 = (StringLiteralTokenSyntax)Visit(node.Guid);
			if (node._guid != stringLiteralTokenSyntax2)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)Visit(node.SecondCommaToken);
			if (node._secondCommaToken != punctuationSyntax4)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax3 = (StringLiteralTokenSyntax)Visit(node.Checksum);
			if (node._checksum != stringLiteralTokenSyntax3)
			{
				flag = true;
			}
			PunctuationSyntax punctuationSyntax5 = (PunctuationSyntax)Visit(node.CloseParenToken);
			if (node._closeParenToken != punctuationSyntax5)
			{
				flag = true;
			}
			if (flag)
			{
				return new ExternalChecksumDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax2, stringLiteralTokenSyntax, punctuationSyntax3, stringLiteralTokenSyntax2, punctuationSyntax4, stringLiteralTokenSyntax3, punctuationSyntax5);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitEnableWarningDirectiveTrivia(EnableWarningDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.EnableKeyword);
			if (node._enableKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.WarningKeyword);
			if (node._warningKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax> separatedSyntaxList = VisitList(node.ErrorCodes);
			if (node._errorCodes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new EnableWarningDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitDisableWarningDirectiveTrivia(DisableWarningDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.DisableKeyword);
			if (node._disableKeyword != keywordSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)Visit(node.WarningKeyword);
			if (node._warningKeyword != keywordSyntax2)
			{
				flag = true;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax> separatedSyntaxList = VisitList(node.ErrorCodes);
			if (node._errorCodes != separatedSyntaxList.Node)
			{
				flag = true;
			}
			if (flag)
			{
				return new DisableWarningDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax2, separatedSyntaxList.Node);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.ReferenceKeyword);
			if (node._referenceKeyword != keywordSyntax)
			{
				flag = true;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)Visit(node.File);
			if (node._file != stringLiteralTokenSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new ReferenceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
		{
			bool flag = false;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.HashToken);
			if (node._hashToken != punctuationSyntax)
			{
				flag = true;
			}
			if (flag)
			{
				return new BadDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax);
			}
			return node;
		}

		public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TNode> VisitList<TNode>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TNode> list) where TNode : VisualBasicSyntaxNode
		{
			SyntaxListBuilder<TNode> syntaxListBuilder = default(SyntaxListBuilder<TNode>);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				TNode val = list[i];
				TNode val2 = (TNode)Visit(val);
				if (val != val2 && syntaxListBuilder.IsNull)
				{
					syntaxListBuilder = new SyntaxListBuilder<TNode>(count);
					syntaxListBuilder.AddRange(list, 0, i);
				}
				if (!syntaxListBuilder.IsNull && val2 != null && val2.Kind != 0)
				{
					syntaxListBuilder.Add(val2);
				}
			}
			if (!syntaxListBuilder.IsNull)
			{
				return syntaxListBuilder.ToList();
			}
			return list;
		}

		public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TNode> VisitList<TNode>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TNode> list) where TNode : VisualBasicSyntaxNode
		{
			SeparatedSyntaxListBuilder<TNode> separatedSyntaxListBuilder = default(SeparatedSyntaxListBuilder<TNode>);
			int i = 0;
			int count = list.Count;
			int separatorCount = list.SeparatorCount;
			for (; i < count; i++)
			{
				TNode val = list[i];
				VisualBasicSyntaxNode visualBasicSyntaxNode = Visit(val);
				GreenNode greenNode = null;
				GreenNode greenNode2 = null;
				if (i < separatorCount)
				{
					greenNode = list.GetSeparator(i);
					greenNode2 = (SyntaxToken)Visit((VisualBasicSyntaxNode)greenNode);
				}
				if ((val != visualBasicSyntaxNode || greenNode != greenNode2) && separatedSyntaxListBuilder.IsNull)
				{
					separatedSyntaxListBuilder = new SeparatedSyntaxListBuilder<TNode>(count);
					separatedSyntaxListBuilder.AddRange(in list, i);
				}
				if (separatedSyntaxListBuilder.IsNull)
				{
					continue;
				}
				if (visualBasicSyntaxNode != null && visualBasicSyntaxNode.Kind != 0)
				{
					separatedSyntaxListBuilder.Add((TNode)visualBasicSyntaxNode);
					if (greenNode2 != null)
					{
						separatedSyntaxListBuilder.AddSeparator(greenNode2);
					}
				}
				else if (i >= separatorCount && separatedSyntaxListBuilder.Count > 0)
				{
					separatedSyntaxListBuilder.RemoveLast();
				}
			}
			if (!separatedSyntaxListBuilder.IsNull)
			{
				return separatedSyntaxListBuilder.ToList();
			}
			return list;
		}
	}
}
