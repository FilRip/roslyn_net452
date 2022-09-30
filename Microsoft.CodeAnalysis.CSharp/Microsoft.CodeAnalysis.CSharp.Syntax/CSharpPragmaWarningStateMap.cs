using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal class CSharpPragmaWarningStateMap : AbstractWarningStateMap<PragmaWarningState>
    {
        public CSharpPragmaWarningStateMap(SyntaxTree syntaxTree)
            : base(syntaxTree)
        {
        }

        protected override WarningStateMapEntry[] CreateWarningStateMapEntries(SyntaxTree syntaxTree)
        {
            ArrayBuilder<DirectiveTriviaSyntax> instance = ArrayBuilder<DirectiveTriviaSyntax>.GetInstance();
            GetAllPragmaWarningDirectives(syntaxTree, instance);
            WarningStateMapEntry[] result = CreatePragmaWarningStateEntries(instance);
            instance.Free();
            return result;
        }

        private static void GetAllPragmaWarningDirectives(SyntaxTree syntaxTree, ArrayBuilder<DirectiveTriviaSyntax> directiveList)
        {
            foreach (DirectiveTriviaSyntax directive in syntaxTree.GetRoot().GetDirectives())
            {
                if (directive.IsActive && directive.Kind() == SyntaxKind.PragmaWarningDirectiveTrivia)
                {
                    PragmaWarningDirectiveTriviaSyntax pragmaWarningDirectiveTriviaSyntax = (PragmaWarningDirectiveTriviaSyntax)directive;
                    if (!pragmaWarningDirectiveTriviaSyntax.DisableOrRestoreKeyword.IsMissing && !pragmaWarningDirectiveTriviaSyntax.WarningKeyword.IsMissing)
                    {
                        directiveList.Add(pragmaWarningDirectiveTriviaSyntax);
                    }
                }
            }
        }

        private static WarningStateMapEntry[] CreatePragmaWarningStateEntries(ArrayBuilder<DirectiveTriviaSyntax> directiveList)
        {
            WarningStateMapEntry[] array = new WarningStateMapEntry[directiveList.Count + 1];
            int num = 0;
            ImmutableDictionary<string, PragmaWarningState> immutableDictionary = ImmutableDictionary.Create<string, PragmaWarningState>();
            PragmaWarningState general = PragmaWarningState.Default;
            WarningStateMapEntry warningStateMapEntry = (array[num] = new WarningStateMapEntry(0, PragmaWarningState.Default, immutableDictionary));
            while (num < directiveList.Count)
            {
                DirectiveTriviaSyntax directiveTriviaSyntax = directiveList[num];
                PragmaWarningDirectiveTriviaSyntax pragmaWarningDirectiveTriviaSyntax = (PragmaWarningDirectiveTriviaSyntax)directiveTriviaSyntax;
                SyntaxKind syntaxKind = pragmaWarningDirectiveTriviaSyntax.DisableOrRestoreKeyword.Kind();
                PragmaWarningState pragmaWarningState = syntaxKind switch
                {
                    SyntaxKind.DisableKeyword => PragmaWarningState.Disabled,
                    SyntaxKind.RestoreKeyword => PragmaWarningState.Default,
                    SyntaxKind.EnableKeyword => PragmaWarningState.Enabled,
                    _ => throw ExceptionUtilities.UnexpectedValue(syntaxKind),
                };
                if (pragmaWarningDirectiveTriviaSyntax.ErrorCodes.Count == 0)
                {
                    general = pragmaWarningState;
                    immutableDictionary = ImmutableDictionary.Create<string, PragmaWarningState>();
                }
                else
                {
                    for (int i = 0; i < pragmaWarningDirectiveTriviaSyntax.ErrorCodes.Count; i++)
                    {
                        ExpressionSyntax expressionSyntax = pragmaWarningDirectiveTriviaSyntax.ErrorCodes[i];
                        if (!expressionSyntax.IsMissing && !expressionSyntax.ContainsDiagnostics)
                        {
                            string text = string.Empty;
                            if (expressionSyntax.Kind() == SyntaxKind.NumericLiteralExpression)
                            {
                                SyntaxToken token = ((LiteralExpressionSyntax)expressionSyntax).Token;
                                text = MessageProvider.Instance.GetIdForErrorCode((int)token.Value);
                            }
                            else if (expressionSyntax.Kind() == SyntaxKind.IdentifierName)
                            {
                                text = ((IdentifierNameSyntax)expressionSyntax).Identifier.ValueText;
                            }
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                immutableDictionary = immutableDictionary.SetItem(text, pragmaWarningState);
                            }
                        }
                    }
                }
                warningStateMapEntry = new WarningStateMapEntry(directiveTriviaSyntax.Location.SourceSpan.End, general, immutableDictionary);
                num++;
                array[num] = warningStateMapEntry;
            }
            return array;
        }
    }
}
