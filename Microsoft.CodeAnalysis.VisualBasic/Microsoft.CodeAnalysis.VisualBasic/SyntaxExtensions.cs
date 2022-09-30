using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class SyntaxExtensions
	{
		internal static bool ReportDocumentationCommentDiagnostics(this SyntaxTree tree)
		{
			return tree.Options.DocumentationMode >= DocumentationMode.Diagnose;
		}

		public static SyntaxTriviaList ToSyntaxTriviaList(this IEnumerable<SyntaxTrivia> sequence)
		{
			return SyntaxFactory.TriviaList(sequence);
		}

		public static TNode NormalizeWhitespace<TNode>(this TNode node, bool useDefaultCasing, string indentation, bool elasticTrivia) where TNode : SyntaxNode
		{
			return (TNode)SyntaxNormalizer.Normalize(node, indentation, "\r\n", elasticTrivia, useDefaultCasing);
		}

		public static TNode NormalizeWhitespace<TNode>(this TNode node, bool useDefaultCasing, string indentation = "    ", string eol = "\r\n", bool elasticTrivia = false) where TNode : SyntaxNode
		{
			return (TNode)SyntaxNormalizer.Normalize(node, indentation, eol, elasticTrivia, useDefaultCasing);
		}

		public static SyntaxToken NormalizeWhitespace(this SyntaxToken token, string indentation, bool elasticTrivia)
		{
			return SyntaxNormalizer.Normalize(token, indentation, "\r\n", elasticTrivia, useDefaultCasing: false);
		}

		public static SyntaxToken NormalizeWhitespace(this SyntaxToken token, string indentation = "    ", string eol = "\r\n", bool elasticTrivia = false, bool useDefaultCasing = false)
		{
			return SyntaxNormalizer.Normalize(token, indentation, eol, elasticTrivia, useDefaultCasing);
		}

		public static SyntaxTriviaList NormalizeWhitespace(this SyntaxTriviaList trivia, string indentation = "    ", string eol = "\r\n", bool elasticTrivia = false, bool useDefaultCasing = false)
		{
			return SyntaxNormalizer.Normalize(trivia, indentation, eol, elasticTrivia, useDefaultCasing);
		}

		public static TypeSyntax Type(this NewExpressionSyntax newExpressionSyntax)
		{
			return newExpressionSyntax.Kind() switch
			{
				SyntaxKind.ObjectCreationExpression => ((ObjectCreationExpressionSyntax)newExpressionSyntax).Type, 
				SyntaxKind.AnonymousObjectCreationExpression => null, 
				SyntaxKind.ArrayCreationExpression => ((ArrayCreationExpressionSyntax)newExpressionSyntax).Type, 
				_ => throw ExceptionUtilities.UnexpectedValue(newExpressionSyntax.Kind()), 
			};
		}

		public static TypeSyntax Type(this AsClauseSyntax asClauseSyntax)
		{
			return asClauseSyntax.Kind() switch
			{
				SyntaxKind.SimpleAsClause => ((SimpleAsClauseSyntax)asClauseSyntax).Type, 
				SyntaxKind.AsNewClause => Type(((AsNewClauseSyntax)asClauseSyntax).NewExpression), 
				_ => throw ExceptionUtilities.UnexpectedValue(asClauseSyntax.Kind()), 
			};
		}

		public static SyntaxList<AttributeListSyntax> Attributes(this AsClauseSyntax asClauseSyntax)
		{
			return asClauseSyntax.Kind() switch
			{
				SyntaxKind.SimpleAsClause => ((SimpleAsClauseSyntax)asClauseSyntax).AttributeLists, 
				SyntaxKind.AsNewClause => ((AsNewClauseSyntax)asClauseSyntax).NewExpression.AttributeLists, 
				_ => throw ExceptionUtilities.UnexpectedValue(asClauseSyntax.Kind()), 
			};
		}

		public static SimpleNameSyntax WithIdentifier(this SimpleNameSyntax simpleName, SyntaxToken identifier)
		{
			if (simpleName.Kind() != SyntaxKind.IdentifierName)
			{
				return ((GenericNameSyntax)simpleName).WithIdentifier(identifier);
			}
			return ((IdentifierNameSyntax)simpleName).WithIdentifier(identifier);
		}

		public static string TryGetInferredMemberName(this SyntaxNode syntax)
		{
			if (syntax == null)
			{
				return null;
			}
			if (!(syntax is ExpressionSyntax input))
			{
				return null;
			}
			XmlNameSyntax failedToInferFromXmlName = null;
			SyntaxToken token = SyntaxNodeExtensions.ExtractAnonymousTypeMemberName(input, out failedToInferFromXmlName);
			return (VisualBasicExtensions.Kind(token) == SyntaxKind.IdentifierToken) ? token.ValueText : null;
		}
	}
}
