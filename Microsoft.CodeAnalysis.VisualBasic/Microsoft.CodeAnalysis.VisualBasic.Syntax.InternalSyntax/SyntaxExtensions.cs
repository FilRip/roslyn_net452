using System;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class SyntaxExtensions
	{
		public static TNode WithAnnotations<TNode>(this TNode node, params SyntaxAnnotation[] annotations) where TNode : VisualBasicSyntaxNode
		{
			if (annotations == null)
			{
				throw new ArgumentNullException("annotations");
			}
			return (TNode)node.SetAnnotations(annotations);
		}

		public static TNode WithAdditionalAnnotations<TNode>(this TNode node, params SyntaxAnnotation[] annotations) where TNode : VisualBasicSyntaxNode
		{
			if (annotations == null)
			{
				throw new ArgumentNullException("annotations");
			}
			return (TNode)node.SetAnnotations(node.GetAnnotations().Concat<SyntaxAnnotation>(annotations).ToArray());
		}

		public static TNode WithoutAnnotations<TNode>(this TNode node, params SyntaxAnnotation[] removalAnnotations) where TNode : VisualBasicSyntaxNode
		{
			ArrayBuilder<SyntaxAnnotation> instance = ArrayBuilder<SyntaxAnnotation>.GetInstance();
			SyntaxAnnotation[] annotations = node.GetAnnotations();
			foreach (SyntaxAnnotation syntaxAnnotation in annotations)
			{
				if (Array.IndexOf(removalAnnotations, syntaxAnnotation) < 0)
				{
					instance.Add(syntaxAnnotation);
				}
			}
			return (TNode)node.SetAnnotations(instance.ToArrayAndFree());
		}

		public static TNode WithAdditionalDiagnostics<TNode>(this TNode node, params DiagnosticInfo[] diagnostics) where TNode : GreenNode
		{
			DiagnosticInfo[] diagnostics2 = node.GetDiagnostics();
			if (diagnostics2 != null)
			{
				return (TNode)node.SetDiagnostics(diagnostics2.Concat(diagnostics).ToArray());
			}
			return WithDiagnostics(node, diagnostics);
		}

		public static TNode WithDiagnostics<TNode>(this TNode node, params DiagnosticInfo[] diagnostics) where TNode : GreenNode
		{
			return (TNode)node.SetDiagnostics(diagnostics);
		}

		public static TNode WithoutDiagnostics<TNode>(this TNode node) where TNode : VisualBasicSyntaxNode
		{
			DiagnosticInfo[] diagnostics = node.GetDiagnostics();
			if (diagnostics == null || diagnostics.Length == 0)
			{
				return node;
			}
			return (TNode)node.SetDiagnostics(null);
		}

		public static VisualBasicSyntaxNode LastTriviaIfAny(this VisualBasicSyntaxNode node)
		{
			GreenNode trailingTrivia = node.GetTrailingTrivia();
			if (trailingTrivia == null)
			{
				return null;
			}
			return new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(trailingTrivia).Last;
		}

		public static bool EndsWithEndOfLineOrColonTrivia(this VisualBasicSyntaxNode node)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = LastTriviaIfAny(node);
			if (visualBasicSyntaxNode != null)
			{
				if (visualBasicSyntaxNode.Kind != SyntaxKind.EndOfLineTrivia)
				{
					return visualBasicSyntaxNode.Kind == SyntaxKind.ColonTrivia;
				}
				return true;
			}
			return false;
		}
	}
}
