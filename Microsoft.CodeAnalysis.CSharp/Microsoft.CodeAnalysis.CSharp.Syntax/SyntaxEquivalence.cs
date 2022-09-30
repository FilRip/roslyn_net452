using System;
using System.Collections.Generic;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal static class SyntaxEquivalence
    {
        internal static bool AreEquivalent(SyntaxTree? before, SyntaxTree? after, Func<SyntaxKind, bool>? ignoreChildNode, bool topLevel)
        {
            if (before == after)
            {
                return true;
            }
            if (before == null || after == null)
            {
                return false;
            }
            return AreEquivalent(before!.GetRoot(), after!.GetRoot(), ignoreChildNode, topLevel);
        }

        public static bool AreEquivalent(SyntaxNode? before, SyntaxNode? after, Func<SyntaxKind, bool>? ignoreChildNode, bool topLevel)
        {
            if (before == null || after == null)
            {
                return before == after;
            }
            return AreEquivalentRecursive(before!.Green, after!.Green, ignoreChildNode, topLevel);
        }

        public static bool AreEquivalent(SyntaxTokenList before, SyntaxTokenList after)
        {
            return AreEquivalentRecursive(before.Node, after.Node, null, topLevel: false);
        }

        public static bool AreEquivalent(SyntaxToken before, SyntaxToken after)
        {
            if (before.RawKind == after.RawKind)
            {
                if (before.Node != null)
                {
                    return AreTokensEquivalent(before.Node, after.Node, null);
                }
                return true;
            }
            return false;
        }

        private static bool AreTokensEquivalent(GreenNode? before, GreenNode? after, Func<SyntaxKind, bool>? ignoreChildNode)
        {
            if (before == null || after == null)
            {
                if (before == null)
                {
                    return after == null;
                }
                return false;
            }
            if (before!.IsMissing != after!.IsMissing)
            {
                return false;
            }
            switch ((ushort)before!.RawKind)
            {
                case 8508:
                    if (((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)before).ValueText != ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)after).ValueText)
                    {
                        return false;
                    }
                    break;
                case 8509:
                case 8510:
                case 8511:
                case 8517:
                    if (((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)before).Text != ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)after).Text)
                    {
                        return false;
                    }
                    break;
            }
            return AreNullableDirectivesEquivalent(before, after, ignoreChildNode);
        }

        private static bool AreEquivalentRecursive(GreenNode? before, GreenNode? after, Func<SyntaxKind, bool>? ignoreChildNode, bool topLevel)
        {
            if (before == after)
            {
                return true;
            }
            if (before == null || after == null)
            {
                return false;
            }
            if (before!.RawKind != after!.RawKind)
            {
                return false;
            }
            if (before!.IsToken)
            {
                return AreTokensEquivalent(before, after, ignoreChildNode);
            }
            if (topLevel)
            {
                SyntaxKind syntaxKind = (SyntaxKind)before!.RawKind;
                if (syntaxKind == SyntaxKind.Block || syntaxKind == SyntaxKind.ArrowExpressionClause)
                {
                    return AreNullableDirectivesEquivalent(before, after, ignoreChildNode);
                }
                if ((ushort)before!.RawKind == 8873)
                {
                    Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FieldDeclarationSyntax obj = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FieldDeclarationSyntax)before;
                    Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FieldDeclarationSyntax fieldDeclarationSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FieldDeclarationSyntax)after;
                    bool num = obj.Modifiers.Any(8350);
                    bool flag = fieldDeclarationSyntax.Modifiers.Any(8350);
                    if (!num && !flag)
                    {
                        ignoreChildNode = (SyntaxKind childKind) => childKind == SyntaxKind.EqualsValueClause;
                    }
                }
            }
            if (ignoreChildNode != null)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator enumerator = before!.ChildNodesAndTokens().GetEnumerator();
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator enumerator2 = after!.ChildNodesAndTokens().GetEnumerator();
                GreenNode greenNode;
                GreenNode greenNode2;
                do
                {
                    greenNode = null;
                    greenNode2 = null;
                    while (enumerator.MoveNext())
                    {
                        GreenNode current = enumerator.Current;
                        if (current != null && (current.IsToken || !ignoreChildNode!((SyntaxKind)current.RawKind)))
                        {
                            greenNode = current;
                            break;
                        }
                    }
                    while (enumerator2.MoveNext())
                    {
                        GreenNode current2 = enumerator2.Current;
                        if (current2 != null && (current2.IsToken || !ignoreChildNode!((SyntaxKind)current2.RawKind)))
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
                while (AreEquivalentRecursive(greenNode, greenNode2, ignoreChildNode, topLevel));
                return false;
            }
            int slotCount = before!.SlotCount;
            if (slotCount != after!.SlotCount)
            {
                return false;
            }
            for (int i = 0; i < slotCount; i++)
            {
                GreenNode? slot = before!.GetSlot(i);
                GreenNode slot2 = after!.GetSlot(i);
                if (!AreEquivalentRecursive(slot, slot2, ignoreChildNode, topLevel))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AreNullableDirectivesEquivalent(GreenNode before, GreenNode after, Func<SyntaxKind, bool>? ignoreChildNode)
        {
            if (ignoreChildNode != null && ignoreChildNode!(SyntaxKind.NullableDirectiveTrivia))
            {
                return true;
            }
            using (IEnumerator<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax> enumerator2 = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode)before).GetDirectives().GetEnumerator())
            {
                using IEnumerator<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax> enumerator3 = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode)after).GetDirectives().GetEnumerator();
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax directiveTriviaSyntax;
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax directiveTriviaSyntax2;
                do
                {
                    directiveTriviaSyntax = getNextNullableDirective(enumerator2, ignoreChildNode);
                    directiveTriviaSyntax2 = getNextNullableDirective(enumerator3, ignoreChildNode);
                    if (directiveTriviaSyntax == null || directiveTriviaSyntax2 == null)
                    {
                        return directiveTriviaSyntax == directiveTriviaSyntax2;
                    }
                }
                while (AreEquivalentRecursive(directiveTriviaSyntax, directiveTriviaSyntax2, ignoreChildNode, topLevel: false));
                return false;
            }
            static Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax? getNextNullableDirective(IEnumerator<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax> enumerator, Func<SyntaxKind, bool>? ignoreChildNode)
            {
                while (enumerator.MoveNext())
                {
                    Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DirectiveTriviaSyntax current = enumerator.Current;
                    if (current.Kind == SyntaxKind.NullableDirectiveTrivia)
                    {
                        return current;
                    }
                }
                return null;
            }
        }
    }
}
