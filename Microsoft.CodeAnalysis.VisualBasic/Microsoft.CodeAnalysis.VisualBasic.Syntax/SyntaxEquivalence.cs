using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	[StandardModule]
	internal sealed class SyntaxEquivalence
	{
		internal static bool AreEquivalent(SyntaxTree before, SyntaxTree after, Func<SyntaxKind, bool> ignoreChildNode, bool topLevel)
		{
			if (before == after)
			{
				return true;
			}
			if (before == null || after == null)
			{
				return false;
			}
			return AreEquivalent(before.GetRoot(), after.GetRoot(), ignoreChildNode, topLevel);
		}

		public static bool AreEquivalent(SyntaxNode before, SyntaxNode after, Func<SyntaxKind, bool> ignoreChildNode, bool topLevel)
		{
			if (before == null || after == null)
			{
				return before == after;
			}
			return AreEquivalentRecursive(before.Green, after.Green, SyntaxKind.None, ignoreChildNode, topLevel);
		}

		public static bool AreEquivalent(SyntaxTokenList before, SyntaxTokenList after)
		{
			return AreEquivalentRecursive(before.Node, after.Node, SyntaxKind.None, null, topLevel: false);
		}

		public static bool AreEquivalent(SyntaxToken before, SyntaxToken after)
		{
			if (before.RawKind == after.RawKind)
			{
				if (before.Node != null)
				{
					return AreTokensEquivalent(before.Node, after.Node);
				}
				return true;
			}
			return false;
		}

		private static bool AreTokensEquivalent(GreenNode before, GreenNode after)
		{
			if (before.IsMissing != after.IsMissing)
			{
				return false;
			}
			SyntaxKind syntaxKind = (SyntaxKind)before.RawKind;
			if (syntaxKind - 700 <= SyntaxKind.EndUsingStatement || syntaxKind == SyntaxKind.InterpolatedStringTextToken)
			{
				return string.Equals(((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)before).Text, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)after).Text, StringComparison.Ordinal);
			}
			return true;
		}

		private static bool AreEquivalentRecursive(GreenNode before, GreenNode after, SyntaxKind parentKind, Func<SyntaxKind, bool> ignoreChildNode, bool topLevel)
		{
			if (before == after)
			{
				return true;
			}
			if (before == null || after == null)
			{
				return false;
			}
			if (before.RawKind != after.RawKind)
			{
				return false;
			}
			if (before.IsToken)
			{
				return AreTokensEquivalent(before, after);
			}
			SyntaxKind syntaxKind = (SyntaxKind)before.RawKind;
			if (!AreModifiersEquivalent(before, after, syntaxKind))
			{
				return false;
			}
			if (topLevel)
			{
				switch (syntaxKind)
				{
				case SyntaxKind.SubBlock:
				case SyntaxKind.FunctionBlock:
				case SyntaxKind.ConstructorBlock:
				case SyntaxKind.OperatorBlock:
				case SyntaxKind.GetAccessorBlock:
				case SyntaxKind.SetAccessorBlock:
				case SyntaxKind.AddHandlerAccessorBlock:
				case SyntaxKind.RemoveHandlerAccessorBlock:
				case SyntaxKind.RaiseEventAccessorBlock:
					return AreEquivalentRecursive(((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax)before).Begin, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax)after).Begin, syntaxKind, null, topLevel: true);
				case SyntaxKind.FieldDeclaration:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax obj = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)before;
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax fieldDeclarationSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)after;
					bool num = obj.Modifiers.Any(441);
					bool flag = fieldDeclarationSyntax.Modifiers.Any(441);
					if (!num && !flag)
					{
						ignoreChildNode = (SyntaxKind childKind) => childKind == SyntaxKind.EqualsValue || childKind == SyntaxKind.AsNewClause;
					}
					break;
				}
				case SyntaxKind.EqualsValue:
					if (parentKind == SyntaxKind.PropertyStatement)
					{
						return true;
					}
					break;
				}
			}
			if (ignoreChildNode != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator enumerator = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)before).ChildNodesAndTokens().GetEnumerator();
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator enumerator2 = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)after).ChildNodesAndTokens().GetEnumerator();
				GreenNode greenNode;
				GreenNode greenNode2;
				do
				{
					greenNode = null;
					greenNode2 = null;
					while (enumerator.MoveNext())
					{
						GreenNode current = enumerator.Current;
						if (current != null && (current.IsToken || !ignoreChildNode((SyntaxKind)current.RawKind)))
						{
							greenNode = current;
							break;
						}
					}
					while (enumerator2.MoveNext())
					{
						GreenNode current2 = enumerator2.Current;
						if (current2 != null && (current2.IsToken || !ignoreChildNode((SyntaxKind)current2.RawKind)))
						{
							greenNode2 = current2;
							break;
						}
					}
					if (greenNode == null || greenNode2 == null)
					{
						return greenNode == greenNode2;
					}
				}
				while (AreEquivalentRecursive(greenNode, greenNode2, syntaxKind, ignoreChildNode, topLevel));
				return false;
			}
			int slotCount = before.SlotCount;
			if (slotCount != after.SlotCount)
			{
				return false;
			}
			int num2 = slotCount - 1;
			for (int i = 0; i <= num2; i++)
			{
				GreenNode? slot = before.GetSlot(i);
				GreenNode slot2 = after.GetSlot(i);
				if (!AreEquivalentRecursive(slot, slot2, syntaxKind, ignoreChildNode, topLevel))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AreModifiersEquivalent(GreenNode before, GreenNode after, SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 79 <= SyntaxKind.List)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax)before).Begin.Modifiers;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers2 = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax)after).Begin.Modifiers;
				if (modifiers.Count != modifiers2.Count)
				{
					return false;
				}
				int num = modifiers.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!modifiers.Any((int)modifiers2[i]!.Kind))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
