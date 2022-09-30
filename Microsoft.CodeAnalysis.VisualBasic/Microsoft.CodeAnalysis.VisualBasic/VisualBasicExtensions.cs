using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class VisualBasicExtensions
	{
		internal static bool IsVisualBasicKind(int rawKind)
		{
			return rawKind <= 8192;
		}

		public static SyntaxKind Kind(this SyntaxTrivia trivia)
		{
			int rawKind = trivia.RawKind;
			if (!IsVisualBasicKind(rawKind))
			{
				return SyntaxKind.None;
			}
			return (SyntaxKind)rawKind;
		}

		public static SyntaxKind Kind(this SyntaxToken token)
		{
			int rawKind = token.RawKind;
			if (!IsVisualBasicKind(rawKind))
			{
				return SyntaxKind.None;
			}
			return (SyntaxKind)rawKind;
		}

		public static SyntaxKind Kind(this SyntaxNode node)
		{
			int rawKind = node.RawKind;
			if (!IsVisualBasicKind(rawKind))
			{
				return SyntaxKind.None;
			}
			return (SyntaxKind)rawKind;
		}

		public static SyntaxKind Kind(this SyntaxNodeOrToken nodeOrToken)
		{
			int rawKind = nodeOrToken.RawKind;
			if (!IsVisualBasicKind(rawKind))
			{
				return SyntaxKind.None;
			}
			return (SyntaxKind)rawKind;
		}

		internal static Location GetLocation(this SyntaxReference syntaxReference)
		{
			VisualBasicSyntaxTree visualBasicSyntaxTree = syntaxReference.SyntaxTree as VisualBasicSyntaxTree;
			if (syntaxReference.SyntaxTree != null)
			{
				if (EmbeddedSymbolExtensions.IsEmbeddedSyntaxTree(visualBasicSyntaxTree))
				{
					return new EmbeddedTreeLocation(EmbeddedSymbolExtensions.GetEmbeddedKind(visualBasicSyntaxTree), syntaxReference.Span);
				}
				if (visualBasicSyntaxTree.IsMyTemplate)
				{
					return new MyTemplateLocation(visualBasicSyntaxTree, syntaxReference.Span);
				}
			}
			return new SourceLocation(syntaxReference);
		}

		internal static bool IsMyTemplate(this SyntaxTree syntaxTree)
		{
			if (syntaxTree is VisualBasicSyntaxTree visualBasicSyntaxTree)
			{
				return visualBasicSyntaxTree.IsMyTemplate;
			}
			return false;
		}

		internal static bool HasReferenceDirectives(this SyntaxTree syntaxTree)
		{
			if (syntaxTree is VisualBasicSyntaxTree visualBasicSyntaxTree)
			{
				return visualBasicSyntaxTree.HasReferenceDirectives;
			}
			return false;
		}

		internal static bool IsAnyPreprocessorSymbolDefined(this SyntaxTree syntaxTree, IEnumerable<string> conditionalSymbolNames, SyntaxNodeOrToken atNode)
		{
			if (syntaxTree is VisualBasicSyntaxTree visualBasicSyntaxTree)
			{
				return visualBasicSyntaxTree.IsAnyPreprocessorSymbolDefined(conditionalSymbolNames, atNode);
			}
			return false;
		}

		internal static VisualBasicSyntaxNode GetVisualBasicSyntax(this SyntaxReference syntaxReference, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (VisualBasicSyntaxNode)syntaxReference.GetSyntax(cancellationToken);
		}

		internal static VisualBasicSyntaxNode GetVisualBasicRoot(this SyntaxTree syntaxTree, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (VisualBasicSyntaxNode)syntaxTree.GetRoot(cancellationToken);
		}

		internal static VisualBasicPreprocessingSymbolInfo GetPreprocessingSymbolInfo(this SyntaxTree syntaxTree, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNode)
		{
			return ((VisualBasicSyntaxTree)syntaxTree).GetPreprocessingSymbolInfo(identifierNode);
		}

		internal static SyntaxDiagnosticInfoList Errors(this SyntaxTrivia trivia)
		{
			return new SyntaxDiagnosticInfoList((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)trivia.UnderlyingNode);
		}

		internal static SyntaxDiagnosticInfoList Errors(this SyntaxToken token)
		{
			return new SyntaxDiagnosticInfoList((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)token.Node);
		}

		internal static ReadOnlyCollection<Diagnostic> GetSyntaxErrors(this SyntaxToken token, SyntaxTree tree)
		{
			return VisualBasicSyntaxNode.DoGetSyntaxErrors(tree, token);
		}

		public static bool IsBracketed(this SyntaxToken token)
		{
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(token, SyntaxKind.IdentifierToken))
			{
				return ((IdentifierTokenSyntax)token.Node).IsBracketed;
			}
			return false;
		}

		public static TypeCharacter GetTypeCharacter(this SyntaxToken token)
		{
			return Kind(token) switch
			{
				SyntaxKind.IdentifierToken => ((IdentifierTokenSyntax)token.Node).TypeCharacter, 
				SyntaxKind.IntegerLiteralToken => ((IntegerLiteralTokenSyntax)token.Node).TypeSuffix, 
				SyntaxKind.FloatingLiteralToken => ((FloatingLiteralTokenSyntax)token.Node).TypeSuffix, 
				SyntaxKind.DecimalLiteralToken => ((DecimalLiteralTokenSyntax)token.Node).TypeSuffix, 
				_ => TypeCharacter.None, 
			};
		}

		public static LiteralBase? GetBase(this SyntaxToken token)
		{
			LiteralBase? result;
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(token, SyntaxKind.IntegerLiteralToken))
			{
				IntegerLiteralTokenSyntax integerLiteralTokenSyntax = (IntegerLiteralTokenSyntax)token.Node;
				result = integerLiteralTokenSyntax.Base;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static bool IsKeyword(this SyntaxToken token)
		{
			return SyntaxFacts.IsKeywordKind(Kind(token));
		}

		public static bool IsReservedKeyword(this SyntaxToken token)
		{
			return SyntaxFacts.IsReservedKeyword(Kind(token));
		}

		public static bool IsContextualKeyword(this SyntaxToken token)
		{
			return SyntaxFacts.IsContextualKeyword(Kind(token));
		}

		public static bool IsPreprocessorKeyword(this SyntaxToken token)
		{
			return SyntaxFacts.IsPreprocessorKeyword(Kind(token));
		}

		public static string GetIdentifierText(this SyntaxToken token)
		{
			if (token.Node == null)
			{
				return string.Empty;
			}
			if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(token, SyntaxKind.IdentifierToken))
			{
				return token.ToString();
			}
			return ((IdentifierTokenSyntax)token.Node).IdentifierText;
		}

		public static SyntaxTokenList Insert(this SyntaxTokenList list, int index, params SyntaxToken[] items)
		{
			if (index < 0 || index > list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (list.Count == 0)
			{
				return SyntaxFactory.TokenList(items);
			}
			SyntaxTokenListBuilder syntaxTokenListBuilder = new SyntaxTokenListBuilder(list.Count + items.Length);
			if (index > 0)
			{
				syntaxTokenListBuilder.Add(list, 0, index);
			}
			syntaxTokenListBuilder.Add(items);
			if (index < list.Count)
			{
				syntaxTokenListBuilder.Add(list, index, list.Count - index);
			}
			return syntaxTokenListBuilder.ToList();
		}

		public static SyntaxTokenList Add(this SyntaxTokenList list, params SyntaxToken[] items)
		{
			return Insert(list, list.Count, items);
		}

		public static SyntaxToken ReplaceTrivia(this SyntaxToken token, SyntaxTrivia oldTrivia, SyntaxTrivia newTrivia)
		{
			return SyntaxReplacer.Replace(token, null, null, null, null, new SyntaxTrivia[1] { oldTrivia }, (SyntaxTrivia o, SyntaxTrivia r) => newTrivia);
		}

		public static SyntaxToken ReplaceTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia, Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia)
		{
			return SyntaxReplacer.Replace(token, null, null, null, null, trivia, computeReplacementTrivia);
		}

		internal static SeparatedSyntaxList<TOther> AsSeparatedList<TOther>(this SyntaxNodeOrTokenList list) where TOther : SyntaxNode
		{
			Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TOther> separatedSyntaxListBuilder = Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TOther>.Create();
			foreach (SyntaxNodeOrToken item in list)
			{
				SyntaxNode syntaxNode = item.AsNode();
				if (syntaxNode != null)
				{
					separatedSyntaxListBuilder.Add((TOther)syntaxNode);
					continue;
				}
				SyntaxToken separatorToken = item.AsToken();
				separatedSyntaxListBuilder.AddSeparator(in separatorToken);
			}
			return separatedSyntaxListBuilder.ToList();
		}

		public static IList<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax> GetDirectives(this SyntaxNode node, Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> filter = null)
		{
			return ((VisualBasicSyntaxNode)node).GetDirectives(filter);
		}

		public static Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax GetFirstDirective(this SyntaxNode node, Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> predicate = null)
		{
			return ((VisualBasicSyntaxNode)node).GetFirstDirective(predicate);
		}

		public static Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax GetLastDirective(this SyntaxNode node, Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> predicate = null)
		{
			return ((VisualBasicSyntaxNode)node).GetLastDirective(predicate);
		}

		public static Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax GetCompilationUnitRoot(this SyntaxTree tree)
		{
			return (Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)tree.GetRoot();
		}

		internal static ReportDiagnostic GetWarningState(this SyntaxTree tree, string id, int position)
		{
			return ((VisualBasicSyntaxTree)tree).GetWarningState(id, position);
		}

		public static bool IsShared(this ISymbol symbol)
		{
			return symbol.IsStatic;
		}

		public static bool IsOverrides(this ISymbol symbol)
		{
			return symbol.IsOverride;
		}

		public static bool IsOverridable(this ISymbol symbol)
		{
			return symbol.IsVirtual;
		}

		public static bool IsNotOverridable(this ISymbol symbol)
		{
			return symbol.IsSealed;
		}

		public static bool IsMustOverride(this ISymbol symbol)
		{
			return symbol.IsAbstract;
		}

		public static bool IsMe(this IParameterSymbol parameterSymbol)
		{
			return parameterSymbol.IsThis;
		}

		public static bool IsOverloads(this IMethodSymbol methodSymbol)
		{
			if (methodSymbol is MethodSymbol methodSymbol2)
			{
				return methodSymbol2.IsOverloads;
			}
			return false;
		}

		public static bool IsOverloads(this IPropertySymbol propertySymbol)
		{
			if (propertySymbol is PropertySymbol propertySymbol2)
			{
				return propertySymbol2.IsOverloads;
			}
			return false;
		}

		public static bool IsDefault(this IPropertySymbol propertySymbol)
		{
			if (propertySymbol is PropertySymbol propertySymbol2)
			{
				return propertySymbol2.IsDefault;
			}
			return false;
		}

		public static ImmutableArray<HandledEvent> HandledEvents(this IMethodSymbol methodSymbol)
		{
			if (methodSymbol is MethodSymbol methodSymbol2)
			{
				return methodSymbol2.HandledEvents;
			}
			return ImmutableArray<HandledEvent>.Empty;
		}

		public static bool IsFor(this ILocalSymbol localSymbol)
		{
			if (localSymbol is LocalSymbol localSymbol2)
			{
				return localSymbol2.IsFor;
			}
			return false;
		}

		public static bool IsForEach(this ILocalSymbol localSymbol)
		{
			if (localSymbol is LocalSymbol localSymbol2)
			{
				return localSymbol2.IsForEach;
			}
			return false;
		}

		public static bool IsCatch(this ILocalSymbol localSymbol)
		{
			if (localSymbol is LocalSymbol localSymbol2)
			{
				return localSymbol2.IsCatch;
			}
			return false;
		}

		public static IFieldSymbol AssociatedField(this IEventSymbol eventSymbol)
		{
			if (!(eventSymbol is EventSymbol eventSymbol2))
			{
				return null;
			}
			return eventSymbol2.AssociatedField;
		}

		public static bool HasAssociatedField(this IEventSymbol eventSymbol)
		{
			if (eventSymbol is EventSymbol eventSymbol2)
			{
				return eventSymbol2.HasAssociatedField;
			}
			return false;
		}

		public static ImmutableArray<AttributeData> GetFieldAttributes(this IEventSymbol eventSymbol)
		{
			if (eventSymbol is EventSymbol eventSymbol2)
			{
				return StaticCast<AttributeData>.From(eventSymbol2.GetFieldAttributes());
			}
			return ImmutableArray<AttributeData>.Empty;
		}

		public static bool IsImplicitlyDeclared(this IEventSymbol eventSymbol)
		{
			if (eventSymbol is EventSymbol eventSymbol2)
			{
				return eventSymbol2.IsImplicitlyDeclared;
			}
			return false;
		}

		public static ImmutableArray<INamedTypeSymbol> GetModuleMembers(this INamespaceSymbol @namespace)
		{
			if (@namespace is NamespaceSymbol namespaceSymbol)
			{
				return StaticCast<INamedTypeSymbol>.From(namespaceSymbol.GetModuleMembers());
			}
			return ImmutableArray.Create<INamedTypeSymbol>();
		}

		public static ImmutableArray<INamedTypeSymbol> GetModuleMembers(this INamespaceSymbol @namespace, string name)
		{
			if (@namespace is NamespaceSymbol namespaceSymbol)
			{
				return StaticCast<INamedTypeSymbol>.From(namespaceSymbol.GetModuleMembers(name));
			}
			return ImmutableArray.Create<INamedTypeSymbol>();
		}

		public static OptionStrict OptionStrict(this SemanticModel semanticModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.OptionStrict;
			}
			return Microsoft.CodeAnalysis.VisualBasic.OptionStrict.Off;
		}

		public static bool OptionInfer(this SemanticModel semanticModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.OptionInfer;
			}
			return false;
		}

		public static bool OptionExplicit(this SemanticModel semanticModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.OptionExplicit;
			}
			return false;
		}

		public static bool OptionCompareText(this SemanticModel semanticModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.OptionCompareText;
			}
			return false;
		}

		public static INamespaceSymbol RootNamespace(this Compilation compilation)
		{
			if (compilation is VisualBasicCompilation visualBasicCompilation)
			{
				return visualBasicCompilation.RootNamespace;
			}
			return null;
		}

		public static ImmutableArray<IAliasSymbol> AliasImports(this Compilation compilation)
		{
			if (compilation is VisualBasicCompilation visualBasicCompilation)
			{
				return StaticCast<IAliasSymbol>.From(visualBasicCompilation.AliasImports);
			}
			return ImmutableArray.Create<IAliasSymbol>();
		}

		public static ImmutableArray<INamespaceOrTypeSymbol> MemberImports(this Compilation compilation)
		{
			if (compilation is VisualBasicCompilation visualBasicCompilation)
			{
				return StaticCast<INamespaceOrTypeSymbol>.From(visualBasicCompilation.MemberImports);
			}
			return ImmutableArray.Create<INamespaceOrTypeSymbol>();
		}

		public static Conversion ClassifyConversion(this Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
		{
			if (compilation is VisualBasicCompilation visualBasicCompilation)
			{
				return visualBasicCompilation.ClassifyConversion((TypeSymbol)source, (TypeSymbol)destination);
			}
			return default(Conversion);
		}

		public static INamedTypeSymbol GetSpecialType(this Compilation compilation, SpecialType typeId)
		{
			if (compilation is VisualBasicCompilation visualBasicCompilation)
			{
				return visualBasicCompilation.GetSpecialType(typeId);
			}
			return null;
		}

		public static Conversion ClassifyConversion(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, ITypeSymbol destination)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.ClassifyConversion(expression, (TypeSymbol)destination);
			}
			return default(Conversion);
		}

		public static Conversion ClassifyConversion(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, ITypeSymbol destination)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.ClassifyConversion(position, expression, (TypeSymbol)destination);
			}
			return default(Conversion);
		}

		public static ISymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax identifierSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(identifierSyntax, cancellationToken);
			}
			return null;
		}

		public static ISymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax elementSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(elementSyntax, cancellationToken);
			}
			return null;
		}

		public static IPropertySymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax fieldInitializerSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken);
			}
			return null;
		}

		public static IRangeVariableSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return null;
		}

		public static IRangeVariableSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return null;
		}

		public static IRangeVariableSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return null;
		}

		public static ILabelSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IFieldSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamespaceSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static INamespaceSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IParameterSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax parameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(parameter, cancellationToken);
			}
			return null;
		}

		public static ITypeParameterSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(typeParameter, cancellationToken);
			}
			return null;
		}

		public static INamedTypeSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IPropertySymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IEventSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IPropertySymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IEventSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static ILocalSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IMethodSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static IAliasSymbol GetDeclaredSymbol(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return null;
		}

		public static ForEachStatementInfo GetForEachStatementInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax node)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetForEachStatementInfo(node);
			}
			return default(ForEachStatementInfo);
		}

		public static ForEachStatementInfo GetForEachStatementInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax node)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetForEachStatementInfo(node);
			}
			return default(ForEachStatementInfo);
		}

		public static AwaitExpressionInfo GetAwaitExpressionInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetAwaitExpressionInfo(awaitExpression, cancellationToken);
			}
			return default(AwaitExpressionInfo);
		}

		public static PreprocessingSymbolInfo GetPreprocessingSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax node)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetPreprocessingSymbolInfo(node);
			}
			return PreprocessingSymbolInfo.None;
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(expression, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetCollectionInitializerSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetCollectionInitializerSymbolInfo(expression, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax crefReference, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(crefReference, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(attribute, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSpeculativeSymbolInfo(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeSymbolInfo(position, expression, bindingOption);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSpeculativeSymbolInfo(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax attribute)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeSymbolInfo(position, attribute);
			}
			return default(SymbolInfo);
		}

		public static Conversion GetConversion(this SemanticModel semanticModel, SyntaxNode expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetConversion(expression, cancellationToken);
			}
			return default(Conversion);
		}

		public static Conversion GetConversion(this IConversionOperation conversionExpression)
		{
			if (EmbeddedOperators.CompareString(conversionExpression.Language, "Visual Basic", TextCompare: false) == 0)
			{
				return (Conversion)(object)((ConversionOperation)conversionExpression).ConversionConvertible;
			}
			throw new ArgumentException(string.Format(VBResources.IConversionExpressionIsNotVisualBasicConversion, "IConversionOperation"), "conversionExpression");
		}

		public static Conversion GetInConversion(this IArgumentOperation argument)
		{
			if (EmbeddedOperators.CompareString(argument.Language, "Visual Basic", TextCompare: false) == 0)
			{
				IConvertibleConversion inConversionConvertible = ((ArgumentOperation)argument).InConversionConvertible;
				return (inConversionConvertible != null) ? ((Conversion)(object)inConversionConvertible) : new Conversion(Conversions.Identity);
			}
			throw new ArgumentException(string.Format(VBResources.IArgumentIsNotVisualBasicArgument, "IArgumentOperation"), "argument");
		}

		public static Conversion GetOutConversion(this IArgumentOperation argument)
		{
			if (EmbeddedOperators.CompareString(argument.Language, "Visual Basic", TextCompare: false) == 0)
			{
				IConvertibleConversion outConversionConvertible = ((ArgumentOperation)argument).OutConversionConvertible;
				return (outConversionConvertible != null) ? ((Conversion)(object)outConversionConvertible) : new Conversion(Conversions.Identity);
			}
			throw new ArgumentException(string.Format(VBResources.IArgumentIsNotVisualBasicArgument, "IArgumentOperation"), "argument");
		}

		public static Conversion GetInConversion(this ICompoundAssignmentOperation compoundAssignment)
		{
			if (compoundAssignment == null)
			{
				throw new ArgumentNullException("compoundAssignment");
			}
			if (EmbeddedOperators.CompareString(compoundAssignment.Language, "Visual Basic", TextCompare: false) == 0)
			{
				return (Conversion)(object)((CompoundAssignmentOperation)compoundAssignment).InConversionConvertible;
			}
			throw new ArgumentException(string.Format(VBResources.ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment, "compoundAssignment"), "compoundAssignment");
		}

		public static Conversion GetOutConversion(this ICompoundAssignmentOperation compoundAssignment)
		{
			if (compoundAssignment == null)
			{
				throw new ArgumentNullException("compoundAssignment");
			}
			if (EmbeddedOperators.CompareString(compoundAssignment.Language, "Visual Basic", TextCompare: false) == 0)
			{
				return (Conversion)(object)((CompoundAssignmentOperation)compoundAssignment).OutConversionConvertible;
			}
			throw new ArgumentException(string.Format(VBResources.ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment, "compoundAssignment"), "compoundAssignment");
		}

		public static Conversion GetSpeculativeConversion(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeConversion(position, expression, bindingOption);
			}
			return default(Conversion);
		}

		public static TypeInfo GetTypeInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetTypeInfo(expression, cancellationToken);
			}
			return default(TypeInfo);
		}

		public static TypeInfo GetSpeculativeTypeInfo(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeTypeInfo(position, expression, bindingOption);
			}
			return default(TypeInfo);
		}

		public static TypeInfo GetTypeInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetTypeInfo(attribute, cancellationToken);
			}
			return default(TypeInfo);
		}

		public static ImmutableArray<ISymbol> GetMemberGroup(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetMemberGroup(expression, cancellationToken);
			}
			return ImmutableArray.Create<ISymbol>();
		}

		public static ImmutableArray<ISymbol> GetSpeculativeMemberGroup(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeMemberGroup(position, expression);
			}
			return ImmutableArray.Create<ISymbol>();
		}

		public static ImmutableArray<ISymbol> GetMemberGroup(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetMemberGroup(attribute, cancellationToken);
			}
			return ImmutableArray.Create<ISymbol>();
		}

		public static IAliasSymbol GetAliasInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax nameSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetAliasInfo(nameSyntax, cancellationToken);
			}
			return null;
		}

		public static IAliasSymbol GetSpeculativeAliasInfo(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax nameSyntax, SpeculativeBindingOption bindingOption)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSpeculativeAliasInfo(position, nameSyntax, bindingOption);
			}
			return null;
		}

		public static CollectionRangeVariableSymbolInfo GetCollectionRangeVariableSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax variableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetCollectionRangeVariableSymbolInfo(variableSyntax, cancellationToken);
			}
			return default(CollectionRangeVariableSymbolInfo);
		}

		public static AggregateClauseSymbolInfo GetAggregateClauseSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax aggregateSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetAggregateClauseSymbolInfo(aggregateSyntax, cancellationToken);
			}
			return default(AggregateClauseSymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax clauseSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(clauseSyntax, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax variableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(variableSyntax, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax functionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(functionSyntax, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static SymbolInfo GetSymbolInfo(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax orderingSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.GetSymbolInfo(orderingSyntax, cancellationToken);
			}
			return default(SymbolInfo);
		}

		public static ControlFlowAnalysis AnalyzeControlFlow(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax firstStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax lastStatement)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.AnalyzeControlFlow(firstStatement, lastStatement);
			}
			return null;
		}

		public static ControlFlowAnalysis AnalyzeControlFlow(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax statement)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.AnalyzeControlFlow(statement);
			}
			return null;
		}

		public static DataFlowAnalysis AnalyzeDataFlow(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expression)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.AnalyzeDataFlow(expression);
			}
			return null;
		}

		public static DataFlowAnalysis AnalyzeDataFlow(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax firstStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax lastStatement)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.AnalyzeDataFlow(firstStatement, lastStatement);
			}
			return null;
		}

		public static DataFlowAnalysis AnalyzeDataFlow(this SemanticModel semanticModel, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax statement)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.AnalyzeDataFlow(statement);
			}
			return null;
		}

		public static bool TryGetSpeculativeSemanticModelForMethodBody(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModelForMethodBody(position, method, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public static bool TryGetSpeculativeSemanticModel(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax rangeArgument, out SemanticModel speculativeModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModel(position, rangeArgument, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public static bool TryGetSpeculativeSemanticModel(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModel(position, statement, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public static bool TryGetSpeculativeSemanticModel(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModel(position, initializer, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public static bool TryGetSpeculativeSemanticModel(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax attribute, out SemanticModel speculativeModel)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModel(position, attribute, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public static bool TryGetSpeculativeSemanticModel(this SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax type, out SemanticModel speculativeModel, SpeculativeBindingOption bindingOption = SpeculativeBindingOption.BindAsExpression)
		{
			if (semanticModel is VBSemanticModel vBSemanticModel)
			{
				return vBSemanticModel.TryGetSpeculativeSemanticModel(position, type, out speculativeModel, bindingOption);
			}
			speculativeModel = null;
			return false;
		}
	}
}
