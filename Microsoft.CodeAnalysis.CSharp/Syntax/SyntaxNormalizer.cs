// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal class SyntaxNormalizer : CSharpSyntaxRewriter
    {
        private readonly TextSpan _consideredSpan;
        private readonly int _initialDepth;
        private readonly string _indentWhitespace;
        private readonly bool _useElasticTrivia;
        private readonly SyntaxTrivia _eolTrivia;

        private bool _isInStructuredTrivia;

        private SyntaxToken _previousToken;

        private bool _afterLineBreak;
        private bool _afterIndentation;
        private bool _inSingleLineInterpolation;

        // CONSIDER: if we become concerned about space, we shouldn't actually need any 
        // of the values between indentations[0] and indentations[initialDepth] (exclusive).
        private ArrayBuilder<SyntaxTrivia>? _indentations;

        private SyntaxNormalizer(TextSpan consideredSpan, int initialDepth, string indentWhitespace, string eolWhitespace, bool useElasticTrivia)
            : base(visitIntoStructuredTrivia: true)
        {
            _consideredSpan = consideredSpan;
            _initialDepth = initialDepth;
            _indentWhitespace = indentWhitespace;
            _useElasticTrivia = useElasticTrivia;
            _eolTrivia = useElasticTrivia ? SyntaxFactory.ElasticEndOfLine(eolWhitespace) : SyntaxFactory.EndOfLine(eolWhitespace);
            _afterLineBreak = true;
        }

        internal static TNode Normalize<TNode>(TNode node, string indentWhitespace, string eolWhitespace, bool useElasticTrivia = false)
            where TNode : SyntaxNode
        {
            var normalizer = new SyntaxNormalizer(node.FullSpan, GetDeclarationDepth(node), indentWhitespace, eolWhitespace, useElasticTrivia);
            var result = (TNode)normalizer.Visit(node);
            normalizer.Free();
            return result;
        }

        internal static SyntaxToken Normalize(SyntaxToken token, string indentWhitespace, string eolWhitespace, bool useElasticTrivia = false)
        {
            var normalizer = new SyntaxNormalizer(token.FullSpan, GetDeclarationDepth(token), indentWhitespace, eolWhitespace, useElasticTrivia);
            var result = normalizer.VisitToken(token);
            normalizer.Free();
            return result;
        }

        internal static SyntaxTriviaList Normalize(SyntaxTriviaList trivia, string indentWhitespace, string eolWhitespace, bool useElasticTrivia = false)
        {
            var normalizer = new SyntaxNormalizer(trivia.FullSpan, GetDeclarationDepth(trivia.Token), indentWhitespace, eolWhitespace, useElasticTrivia);
            var result = normalizer.RewriteTrivia(
                trivia,
                GetDeclarationDepth(trivia.ElementAt(0).Token),
                isTrailing: false,
                indentAfterLineBreak: false,
                mustHaveSeparator: false,
                lineBreaksAfter: 0);
            normalizer.Free();
            return result;
        }

        private void Free()
        {
            _indentations?.Free();
        }

        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            if (token.Kind() == SyntaxKind.None || (token.IsMissing && token.FullWidth == 0))
            {
                return token;
            }

            try
            {
                var tk = token;

                var depth = GetDeclarationDepth(token);

                tk = tk.WithLeadingTrivia(RewriteTrivia(
                    token.LeadingTrivia,
                    depth,
                    isTrailing: false,
                    indentAfterLineBreak: NeedsIndentAfterLineBreak(token),
                    mustHaveSeparator: false,
                    lineBreaksAfter: 0));

                var nextToken = this.GetNextRelevantToken(token);

                _afterLineBreak = IsLineBreak(token);
                _afterIndentation = false;

                var lineBreaksAfter = LineBreaksAfter(token, nextToken);
                var needsSeparatorAfter = NeedsSeparator(token, nextToken);
                tk = tk.WithTrailingTrivia(RewriteTrivia(
                    token.TrailingTrivia,
                    depth,
                    isTrailing: true,
                    indentAfterLineBreak: false,
                    mustHaveSeparator: needsSeparatorAfter,
                    lineBreaksAfter: lineBreaksAfter));

                return tk;
            }
            finally
            {
                // to help debugging
                _previousToken = token;
            }
        }

        private SyntaxToken GetNextRelevantToken(SyntaxToken token)
        {
            // get next token, skipping zero width tokens except for end-of-directive tokens
            var nextToken = token.GetNextToken(
                t => SyntaxToken.NonZeroWidth(t) || t.Kind() == SyntaxKind.EndOfDirectiveToken,
                t => t.Kind() == SyntaxKind.SkippedTokensTrivia);

            if (_consideredSpan.Contains(nextToken.FullSpan))
            {
                return nextToken;
            }
            else
            {
                return default;
            }
        }

        private SyntaxTrivia GetIndentation(int count)
        {
            count = Math.Max(count - _initialDepth, 0);

            int capacity = count + 1;
            if (_indentations == null)
            {
                _indentations = ArrayBuilder<SyntaxTrivia>.GetInstance(capacity);
            }
            else
            {
                _indentations.EnsureCapacity(capacity);
            }

            // grow indentation collection if necessary
            for (int i = _indentations.Count; i <= count; i++)
            {
                string text = i == 0
                    ? ""
                    : _indentations[i - 1].ToString() + _indentWhitespace;
                _indentations.Add(_useElasticTrivia ? SyntaxFactory.ElasticWhitespace(text) : SyntaxFactory.Whitespace(text));
            }

            return _indentations[count];
        }

        private static bool NeedsIndentAfterLineBreak(SyntaxToken token)
        {
            return !token.IsKind(SyntaxKind.EndOfFileToken);
        }

        private int LineBreaksAfter(SyntaxToken currentToken, SyntaxToken nextToken)
        {
            if (_inSingleLineInterpolation)
            {
                return 0;
            }

            if (currentToken.IsKind(SyntaxKind.EndOfDirectiveToken))
            {
                return 1;
            }

            if (nextToken.Kind() == SyntaxKind.None)
            {
                return 0;
            }

            // none of the following tests currently have meaning for structured trivia
            if (_isInStructuredTrivia)
            {
                return 0;
            }

            if (nextToken.IsKind(SyntaxKind.CloseBraceToken) &&
                IsAccessorListWithoutAccessorsWithBlockBody(currentToken.Parent?.Parent))
            {
                return 0;
            }

            switch (currentToken.Kind())
            {
                case SyntaxKind.None:
                    return 0;

                case SyntaxKind.OpenBraceToken:
                    return LineBreaksAfterOpenBrace(currentToken/*, nextToken*/);

                case SyntaxKind.FinallyKeyword:
                    return 1;

                case SyntaxKind.CloseBraceToken:
                    return LineBreaksAfterCloseBrace(currentToken, nextToken);

                case SyntaxKind.CloseParenToken:
                    // Note: the `where` case handles constraints on method declarations
                    //  and also `where` clauses (consistently with other LINQ cases below)
                    return (((currentToken.Parent is StatementSyntax) && nextToken.Parent != currentToken.Parent)
                        || nextToken.Kind() == SyntaxKind.OpenBraceToken
                        || nextToken.Kind() == SyntaxKind.WhereKeyword) ? 1 : 0;

                case SyntaxKind.CloseBracketToken:
                    if (currentToken.Parent is AttributeListSyntax && currentToken.Parent.Parent is not ParameterSyntax)
                    {
                        return 1;
                    }
                    break;

                case SyntaxKind.SemicolonToken:
                    return LineBreaksAfterSemicolon(currentToken, nextToken);

                case SyntaxKind.CommaToken:
                    return currentToken.Parent is EnumDeclarationSyntax ? 1 : 0;
                case SyntaxKind.ElseKeyword:
                    return nextToken.Kind() != SyntaxKind.IfKeyword ? 1 : 0;
                case SyntaxKind.ColonToken:
                    if (currentToken.Parent is LabeledStatementSyntax || currentToken.Parent is SwitchLabelSyntax)
                    {
                        return 1;
                    }
                    break;
            }

            if ((nextToken.IsKind(SyntaxKind.FromKeyword) && nextToken.Parent.IsKind(SyntaxKind.FromClause)) ||
                (nextToken.IsKind(SyntaxKind.LetKeyword) && nextToken.Parent.IsKind(SyntaxKind.LetClause)) ||
                (nextToken.IsKind(SyntaxKind.WhereKeyword) && nextToken.Parent.IsKind(SyntaxKind.WhereClause)) ||
                (nextToken.IsKind(SyntaxKind.JoinKeyword) && nextToken.Parent.IsKind(SyntaxKind.JoinClause)) ||
                (nextToken.IsKind(SyntaxKind.JoinKeyword) && nextToken.Parent.IsKind(SyntaxKind.JoinIntoClause)) ||
                (nextToken.IsKind(SyntaxKind.OrderByKeyword) && nextToken.Parent.IsKind(SyntaxKind.OrderByClause)) ||
                (nextToken.IsKind(SyntaxKind.SelectKeyword) && nextToken.Parent.IsKind(SyntaxKind.SelectClause)) ||
                (nextToken.IsKind(SyntaxKind.GroupKeyword) && nextToken.Parent.IsKind(SyntaxKind.GroupClause)))
            {
                return 1;
            }

            return nextToken.Kind() switch
            {
                SyntaxKind.OpenBraceToken => LineBreaksBeforeOpenBrace(nextToken),
                SyntaxKind.CloseBraceToken => LineBreaksBeforeCloseBrace(nextToken),
                SyntaxKind.ElseKeyword or SyntaxKind.FinallyKeyword => 1,
                SyntaxKind.OpenBracketToken => (nextToken.Parent is AttributeListSyntax && nextToken.Parent.Parent is not ParameterSyntax) ? 1 : 0,
                SyntaxKind.WhereKeyword => currentToken.Parent is TypeParameterListSyntax ? 1 : 0,
                _ => 0,
            };
        }

        private static bool IsAccessorListWithoutAccessorsWithBlockBody(SyntaxNode? node)
            => node is AccessorListSyntax accessorList &&
                accessorList.Accessors.All(a => a.Body == null);

        private static bool IsAccessorListFollowedByInitializer([NotNullWhen(true)] SyntaxNode? node)
            => node is AccessorListSyntax &&
                node.Parent is PropertyDeclarationSyntax property &&
                property.Initializer != null;

        private static int LineBreaksBeforeOpenBrace(SyntaxToken openBraceToken)
        {
            if (openBraceToken.Parent.IsKind(SyntaxKind.Interpolation) ||
                openBraceToken.Parent is InitializerExpressionSyntax ||
                IsAccessorListWithoutAccessorsWithBlockBody(openBraceToken.Parent))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static int LineBreaksBeforeCloseBrace(SyntaxToken closeBraceToken)
        {
            if (closeBraceToken.Parent.IsKind(SyntaxKind.Interpolation) ||
                closeBraceToken.Parent is InitializerExpressionSyntax)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static int LineBreaksAfterOpenBrace(SyntaxToken currentToken/*, SyntaxToken nextToken*/)
        {
            if (currentToken.Parent is InitializerExpressionSyntax ||
                currentToken.Parent.IsKind(SyntaxKind.Interpolation) ||
                IsAccessorListWithoutAccessorsWithBlockBody(currentToken.Parent))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static int LineBreaksAfterCloseBrace(SyntaxToken currentToken, SyntaxToken nextToken)
        {
            if (currentToken.Parent is InitializerExpressionSyntax ||
                currentToken.Parent.IsKind(SyntaxKind.Interpolation) ||
                currentToken.Parent?.Parent is AnonymousFunctionExpressionSyntax ||
                IsAccessorListFollowedByInitializer(currentToken.Parent))
            {
                return 0;
            }

            var kind = nextToken.Kind();
            switch (kind)
            {
                case SyntaxKind.EndOfFileToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                case SyntaxKind.ElseKeyword:
                    return 1;
                default:
                    if (kind == SyntaxKind.WhileKeyword &&
                        nextToken.Parent.IsKind(SyntaxKind.DoStatement))
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
            }
        }

        private static int LineBreaksAfterSemicolon(SyntaxToken currentToken, SyntaxToken nextToken)
        {
            if (currentToken.Parent.IsKind(SyntaxKind.ForStatement))
            {
                return 0;
            }
            else if (nextToken.Kind() == SyntaxKind.CloseBraceToken)
            {
                return 1;
            }
            else if (currentToken.Parent.IsKind(SyntaxKind.UsingDirective))
            {
                return nextToken.Parent.IsKind(SyntaxKind.UsingDirective) ? 1 : 2;
            }
            else if (currentToken.Parent.IsKind(SyntaxKind.ExternAliasDirective))
            {
                return nextToken.Parent.IsKind(SyntaxKind.ExternAliasDirective) ? 1 : 2;
            }
            else if (currentToken.Parent is AccessorDeclarationSyntax &&
                IsAccessorListWithoutAccessorsWithBlockBody(currentToken.Parent.Parent))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static bool NeedsSeparator(SyntaxToken token, SyntaxToken next)
        {
            if (token.Parent == null || next.Parent == null)
            {
                return false;
            }

            if (IsAccessorListWithoutAccessorsWithBlockBody(next.Parent) ||
                IsAccessorListWithoutAccessorsWithBlockBody(next.Parent.Parent))
            {
                // when the accessors are formatted inline, the separator is needed
                // unless there is a semicolon. For example: "{ get; set; }" 
                return !next.IsKind(SyntaxKind.SemicolonToken);
            }

            if (IsXmlTextToken(token.Kind()) || IsXmlTextToken(next.Kind()))
            {
                return false;
            }

            if (next.Kind() == SyntaxKind.EndOfDirectiveToken)
            {
                // In a directive, there's often no token between the directive keyword and 
                // the end-of-directive, so we may need a separator.
                return IsKeyword(token.Kind()) && next.LeadingWidth > 0;
            }

            if ((token.Parent is AssignmentExpressionSyntax && AssignmentTokenNeedsSeparator(token.Kind())) ||
                (next.Parent is AssignmentExpressionSyntax && AssignmentTokenNeedsSeparator(next.Kind())) ||
                (token.Parent is BinaryExpressionSyntax && BinaryTokenNeedsSeparator(token.Kind())) ||
                (next.Parent is BinaryExpressionSyntax && BinaryTokenNeedsSeparator(next.Kind())))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.GreaterThanToken) && token.Parent.IsKind(SyntaxKind.TypeArgumentList) &&
                !SyntaxFacts.IsPunctuation(next.Kind()))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.GreaterThanToken) && token.Parent.IsKind(SyntaxKind.FunctionPointerParameterList))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.CommaToken) &&
                !next.IsKind(SyntaxKind.CommaToken) &&
                !token.Parent.IsKind(SyntaxKind.EnumDeclaration))
            {
                return true;
            }

            if (token.Kind() == SyntaxKind.SemicolonToken
                && !(next.Kind() == SyntaxKind.SemicolonToken || next.Kind() == SyntaxKind.CloseParenToken))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.QuestionToken)
                && (token.Parent.IsKind(SyntaxKind.ConditionalExpression) || token.Parent is TypeSyntax)
                && !token.Parent.Parent.IsKind(SyntaxKind.TypeArgumentList))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.ColonToken))
            {
                return !token.Parent.IsKind(SyntaxKind.InterpolationFormatClause) &&
                    !token.Parent.IsKind(SyntaxKind.XmlPrefix);
            }

            if (next.IsKind(SyntaxKind.ColonToken) &&
                (next.Parent.IsKind(SyntaxKind.BaseList) ||
                    next.Parent.IsKind(SyntaxKind.TypeParameterConstraintClause)))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.CloseBracketToken) && IsWord(next.Kind()))
            {
                return true;
            }

            // We don't want to add extra space after cast, we want space only after tuple
            if (token.IsKind(SyntaxKind.CloseParenToken) && IsWord(next.Kind()) && token.Parent.IsKind(SyntaxKind.TupleType) == true)
            {
                return true;
            }

            if ((next.IsKind(SyntaxKind.QuestionToken) || next.IsKind(SyntaxKind.ColonToken))
                && (next.Parent.IsKind(SyntaxKind.ConditionalExpression)))
            {
                return true;
            }

            if (token.IsKind(SyntaxKind.EqualsToken))
            {
                return !token.Parent.IsKind(SyntaxKind.XmlTextAttribute);
            }

            if (next.IsKind(SyntaxKind.EqualsToken))
            {
                return !next.Parent.IsKind(SyntaxKind.XmlTextAttribute);
            }

            // Rules for function pointer below are taken from:
            // https://github.com/dotnet/roslyn/blob/1cca63b5d8ea170f8d8e88e1574aa3ebe354c23b/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Formatting/Rules/SpacingFormattingRule.cs#L321-L413
            if (token.Parent.IsKind(SyntaxKind.FunctionPointerType))
            {
                // No spacing between delegate and *
                if (next.IsKind(SyntaxKind.AsteriskToken) && token.IsKind(SyntaxKind.DelegateKeyword))
                {
                    return false;
                }

                // Force a space between * and the calling convention
                if (token.IsKind(SyntaxKind.AsteriskToken) && next.Parent.IsKind(SyntaxKind.FunctionPointerCallingConvention))
                {
                    switch (next.Kind())
                    {
                        case SyntaxKind.IdentifierToken:
                        case SyntaxKind.ManagedKeyword:
                        case SyntaxKind.UnmanagedKeyword:
                            return true;
                    }
                }
            }

            if (next.Parent.IsKind(SyntaxKind.FunctionPointerParameterList) && next.IsKind(SyntaxKind.LessThanToken))
            {
                switch (token.Kind())
                {
                    // No spacing between the * and < tokens if there is no calling convention
                    case SyntaxKind.AsteriskToken:
                    // No spacing between the calling convention and opening angle bracket of function pointer types:
                    // delegate* managed<
                    case SyntaxKind.ManagedKeyword:
                    case SyntaxKind.UnmanagedKeyword:
                    // No spacing between the calling convention specifier and the opening angle
                    // delegate* unmanaged[Cdecl]<
                    case SyntaxKind.CloseBracketToken when token.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList):
                        return false;
                }
            }

            // No space between unmanaged and the [
            // delegate* unmanaged[
            if (token.Parent.IsKind(SyntaxKind.FunctionPointerCallingConvention) && next.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList) &&
                next.IsKind(SyntaxKind.OpenBracketToken))
            {
                return false;
            }

            // Function pointer calling convention adjustments
            if (next.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList) && token.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList))
            {
                if (next.IsKind(SyntaxKind.IdentifierToken))
                {
                    if (token.IsKind(SyntaxKind.OpenBracketToken))
                    {
                        return false;
                    }
                    // Space after the ,
                    // unmanaged[Cdecl, Thiscall
                    else if (token.IsKind(SyntaxKind.CommaToken))
                    {
                        return true;
                    }
                }

                // No space between identifier and comma
                // unmanaged[Cdecl,
                if (next.IsKind(SyntaxKind.CommaToken))
                {
                    return false;
                }

                // No space before the ]
                // unmanaged[Cdecl]
                if (next.IsKind(SyntaxKind.CloseBracketToken))
                {
                    return false;
                }
            }

            // No space after the < in function pointer parameter lists
            // delegate*<void
            if (token.IsKind(SyntaxKind.LessThanToken) && token.Parent.IsKind(SyntaxKind.FunctionPointerParameterList))
            {
                return false;
            }

            // No space before the > in function pointer parameter lists
            // delegate*<void>
            if (next.IsKind(SyntaxKind.GreaterThanToken) && next.Parent.IsKind(SyntaxKind.FunctionPointerParameterList))
            {
                return false;
            }

            if (token.IsKind(SyntaxKind.EqualsGreaterThanToken) || next.IsKind(SyntaxKind.EqualsGreaterThanToken))
            {
                return true;
            }

            // Can happen in directives (e.g. #line 1 "file")
            if (SyntaxFacts.IsLiteral(token.Kind()) && SyntaxFacts.IsLiteral(next.Kind()))
            {
                return true;
            }

            // No space before an asterisk that's part of a PointerTypeSyntax.
            if (next.IsKind(SyntaxKind.AsteriskToken) && next.Parent is PointerTypeSyntax)
            {
                return false;
            }

            // The last asterisk of a pointer declaration should be followed by a space.
            if (token.IsKind(SyntaxKind.AsteriskToken) && token.Parent is PointerTypeSyntax &&
                (next.IsKind(SyntaxKind.IdentifierToken) || next.Parent.IsKind(SyntaxKind.IndexerDeclaration)))
            {
                return true;
            }

                if (IsKeyword(token.Kind()) && (!next.IsKind(SyntaxKind.ColonToken) &&
                    !next.IsKind(SyntaxKind.DotToken) &&
                    !next.IsKind(SyntaxKind.QuestionToken) &&
                    !next.IsKind(SyntaxKind.SemicolonToken) &&
                    !next.IsKind(SyntaxKind.OpenBracketToken) &&
                    (!next.IsKind(SyntaxKind.OpenParenToken) || KeywordNeedsSeparatorBeforeOpenParen(token.Kind()) || next.Parent.IsKind(SyntaxKind.TupleType)) &&
                    !next.IsKind(SyntaxKind.CloseParenToken) &&
                    !next.IsKind(SyntaxKind.CloseBraceToken) &&
                    !next.IsKind(SyntaxKind.ColonColonToken) &&
                    !next.IsKind(SyntaxKind.GreaterThanToken) &&
                    !next.IsKind(SyntaxKind.CommaToken)))
            {
                return true;
            }

            if (IsWord(token.Kind()) && IsWord(next.Kind()))
            {
                return true;
            }
            else if (token.Width > 1 && next.Width > 1)
            {
                var tokenLastChar = token.Text.Last();
                var nextFirstChar = next.Text.First();
                if (tokenLastChar == nextFirstChar && TokenCharacterCanBeDoubled(tokenLastChar))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool KeywordNeedsSeparatorBeforeOpenParen(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.TypeOfKeyword or SyntaxKind.DefaultKeyword or SyntaxKind.NewKeyword or SyntaxKind.BaseKeyword or SyntaxKind.ThisKeyword or SyntaxKind.CheckedKeyword or SyntaxKind.UncheckedKeyword or SyntaxKind.SizeOfKeyword or SyntaxKind.ArgListKeyword => false,
                _ => true,
            };
        }

        private static bool IsXmlTextToken(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.XmlTextLiteralNewLineToken or SyntaxKind.XmlTextLiteralToken => true,
                _ => false,
            };
        }

        private static bool BinaryTokenNeedsSeparator(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.DotToken or SyntaxKind.MinusGreaterThanToken => false,
                _ => SyntaxFacts.GetBinaryExpression(kind) != SyntaxKind.None,
            };
        }

        private static bool AssignmentTokenNeedsSeparator(SyntaxKind kind)
        {
            return SyntaxFacts.GetAssignmentExpression(kind) != SyntaxKind.None;
        }

        private SyntaxTriviaList RewriteTrivia(
            SyntaxTriviaList triviaList,
            int depth,
            bool isTrailing,
            bool indentAfterLineBreak,
            bool mustHaveSeparator,
            int lineBreaksAfter)
        {
            ArrayBuilder<SyntaxTrivia> currentTriviaList = ArrayBuilder<SyntaxTrivia>.GetInstance(triviaList.Count);
            try
            {
                foreach (var trivia in triviaList)
                {
                    if (trivia.IsKind(SyntaxKind.WhitespaceTrivia) ||
                        trivia.IsKind(SyntaxKind.EndOfLineTrivia) ||
                        trivia.FullWidth == 0)
                    {
                        continue;
                    }

                    var needsSeparator =
                        (currentTriviaList.Count > 0 && NeedsSeparatorBetween(currentTriviaList.Last())) ||
                            (currentTriviaList.Count == 0 && isTrailing);

                    var needsLineBreak = NeedsLineBreakBefore(trivia, isTrailing)
                        || (currentTriviaList.Count > 0 && NeedsLineBreakBetween(currentTriviaList.Last(), trivia, isTrailing));

                    if (needsLineBreak && !_afterLineBreak)
                    {
                        currentTriviaList.Add(GetEndOfLine());
                        _afterLineBreak = true;
                        _afterIndentation = false;
                    }

                    if (_afterLineBreak)
                    {
                        if (!_afterIndentation && NeedsIndentAfterLineBreak(trivia))
                        {
                            currentTriviaList.Add(this.GetIndentation(GetDeclarationDepth(trivia)));
                            _afterIndentation = true;
                        }
                    }
                    else if (needsSeparator)
                    {
                        currentTriviaList.Add(GetSpace());
                        _afterLineBreak = false;
                        _afterIndentation = false;
                    }

                    if (trivia.HasStructure)
                    {
                        var tr = this.VisitStructuredTrivia(trivia);
                        currentTriviaList.Add(tr);
                    }
                    else if (trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
                    {
                        // recreate exterior to remove any leading whitespace
                        currentTriviaList.Add(s_trimmedDocCommentExterior);
                    }
                    else
                    {
                        currentTriviaList.Add(trivia);
                    }

                    if (NeedsLineBreakAfter(trivia, isTrailing)
                        && (currentTriviaList.Count == 0 || !EndsInLineBreak(currentTriviaList.Last())))
                    {
                        currentTriviaList.Add(GetEndOfLine());
                        _afterLineBreak = true;
                        _afterIndentation = false;
                    }
                }

                if (lineBreaksAfter > 0)
                {
                    if (currentTriviaList.Count > 0
                        && EndsInLineBreak(currentTriviaList.Last()))
                    {
                        lineBreaksAfter--;
                    }

                    for (int i = 0; i < lineBreaksAfter; i++)
                    {
                        currentTriviaList.Add(GetEndOfLine());
                        _afterLineBreak = true;
                        _afterIndentation = false;
                    }
                }
                else if (indentAfterLineBreak && _afterLineBreak && !_afterIndentation)
                {
                    currentTriviaList.Add(this.GetIndentation(depth));
                    _afterIndentation = true;
                }
                else if (mustHaveSeparator)
                {
                    currentTriviaList.Add(GetSpace());
                    _afterLineBreak = false;
                    _afterIndentation = false;
                }

                if (currentTriviaList.Count == 0)
                {
                    return default;
                }
                else if (currentTriviaList.Count == 1)
                {
                    return SyntaxFactory.TriviaList(currentTriviaList.First());
                }
                else
                {
                    return SyntaxFactory.TriviaList(currentTriviaList);
                }
            }
            finally
            {
                currentTriviaList.Free();
            }
        }

        private static readonly SyntaxTrivia s_trimmedDocCommentExterior = SyntaxFactory.DocumentationCommentExterior("///");

        private SyntaxTrivia GetSpace()
        {
            return _useElasticTrivia ? SyntaxFactory.ElasticSpace : SyntaxFactory.Space;
        }

        private SyntaxTrivia GetEndOfLine()
        {
            return _eolTrivia;
        }

        private SyntaxTrivia VisitStructuredTrivia(SyntaxTrivia trivia)
        {
            bool oldIsInStructuredTrivia = _isInStructuredTrivia;
            _isInStructuredTrivia = true;

            SyntaxToken oldPreviousToken = _previousToken;
            _previousToken = default;

            SyntaxTrivia result = VisitTrivia(trivia);

            _isInStructuredTrivia = oldIsInStructuredTrivia;
            _previousToken = oldPreviousToken;

            return result;
        }

        private static bool NeedsSeparatorBetween(SyntaxTrivia trivia)
        {
            return trivia.Kind() switch
            {
                SyntaxKind.None or SyntaxKind.WhitespaceTrivia or SyntaxKind.DocumentationCommentExteriorTrivia => false,
                _ => !SyntaxFacts.IsPreprocessorDirective(trivia.Kind()),
            };
        }

        private static bool NeedsLineBreakBetween(SyntaxTrivia trivia, SyntaxTrivia next, bool isTrailingTrivia)
        {
            return NeedsLineBreakAfter(trivia, isTrailingTrivia)
                || NeedsLineBreakBefore(next, isTrailingTrivia);
        }

        private static bool NeedsLineBreakBefore(SyntaxTrivia trivia, bool isTrailingTrivia)
        {
            var kind = trivia.Kind();
            return kind switch
            {
                SyntaxKind.DocumentationCommentExteriorTrivia => !isTrailingTrivia,
                _ => SyntaxFacts.IsPreprocessorDirective(kind),
            };
        }

        private static bool NeedsLineBreakAfter(SyntaxTrivia trivia, bool isTrailingTrivia)
        {
            var kind = trivia.Kind();
            return kind switch
            {
                SyntaxKind.SingleLineCommentTrivia => true,
                SyntaxKind.MultiLineCommentTrivia => !isTrailingTrivia,
                _ => SyntaxFacts.IsPreprocessorDirective(kind),
            };
        }

        private static bool NeedsIndentAfterLineBreak(SyntaxTrivia trivia)
        {
            return trivia.Kind() switch
            {
                SyntaxKind.SingleLineCommentTrivia or SyntaxKind.MultiLineCommentTrivia or SyntaxKind.DocumentationCommentExteriorTrivia or SyntaxKind.SingleLineDocumentationCommentTrivia or SyntaxKind.MultiLineDocumentationCommentTrivia => true,
                _ => false,
            };
        }

        private static bool IsLineBreak(SyntaxToken token)
        {
            return token.Kind() == SyntaxKind.XmlTextLiteralNewLineToken;
        }

        private static bool EndsInLineBreak(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.EndOfLineTrivia)
            {
                return true;
            }

            if (trivia.Kind() == SyntaxKind.PreprocessingMessageTrivia || trivia.Kind() == SyntaxKind.DisabledTextTrivia)
            {
                var text = trivia.ToFullString();
                return text.Length > 0 && SyntaxFacts.IsNewLine(text.Last());
            }

            if (trivia.HasStructure)
            {
                var node = trivia.GetStructure()!;
                var trailing = node.GetTrailingTrivia();
                if (trailing.Count > 0)
                {
                    return EndsInLineBreak(trailing.Last());
                }
                else
                {
                    return IsLineBreak(node.GetLastToken());
                }
            }

            return false;
        }

        private static bool IsWord(SyntaxKind kind)
        {
            return kind == SyntaxKind.IdentifierToken || IsKeyword(kind);
        }

        private static bool IsKeyword(SyntaxKind kind)
        {
            return SyntaxFacts.IsKeywordKind(kind) || SyntaxFacts.IsPreprocessorKeyword(kind);
        }

        private static bool TokenCharacterCanBeDoubled(char c)
        {
            return c switch
            {
                '+' or '-' or '<' or ':' or '?' or '=' or '"' => true,
                _ => false,
            };
        }

        private static int GetDeclarationDepth(SyntaxToken token)
        {
            return GetDeclarationDepth(token.Parent);
        }

        private static int GetDeclarationDepth(SyntaxTrivia trivia)
        {
            if (SyntaxFacts.IsPreprocessorDirective(trivia.Kind()))
            {
                return 0;
            }

            return GetDeclarationDepth(trivia.Token);
        }

        private static int GetDeclarationDepth(SyntaxNode? node)
        {
            if (node != null)
            {
                if (node.IsStructuredTrivia)
                {
                    var tr = ((StructuredTriviaSyntax)node).ParentTrivia;
                    return GetDeclarationDepth(tr);
                }
                else if (node.Parent != null)
                {
                    if (node.Parent.IsKind(SyntaxKind.CompilationUnit))
                    {
                        return 0;
                    }

                    int parentDepth = GetDeclarationDepth(node.Parent);

                    if (node.Parent.IsKind(SyntaxKind.GlobalStatement))
                    {
                        return parentDepth;
                    }

                    if (node.IsKind(SyntaxKind.IfStatement) && node.Parent.IsKind(SyntaxKind.ElseClause))
                    {
                        return parentDepth;
                    }

                    if (node.Parent is BlockSyntax ||
                        (node is StatementSyntax && node is not BlockSyntax))
                    {
                        // all nested statements are indented one level
                        return parentDepth + 1;
                    }

                    if (node is MemberDeclarationSyntax ||
                        node is AccessorDeclarationSyntax ||
                        node is TypeParameterConstraintClauseSyntax ||
                        node is SwitchSectionSyntax ||
                        node is UsingDirectiveSyntax ||
                        node is ExternAliasDirectiveSyntax ||
                        node is QueryExpressionSyntax ||
                        node is QueryContinuationSyntax)
                    {
                        return parentDepth + 1;
                    }

                    return parentDepth;
                }
            }

            return 0;
        }

        public override SyntaxNode? VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            if (node.StringStartToken.Kind() == SyntaxKind.InterpolatedStringStartToken)
            {
                //Just for non verbatim strings we want to make sure that the formatting of interpolations does not emit line breaks.
                //See: https://github.com/dotnet/roslyn/issues/50742
                //
                //The flag _inSingleLineInterpolation is set to true while visiting InterpolatedStringExpressionSyntax and checked in LineBreaksAfter
                //to suppress adding newlines.
                var old = _inSingleLineInterpolation;
                _inSingleLineInterpolation = true;
                try
                {
                    return base.VisitInterpolatedStringExpression(node);
                }
                finally
                {
                    _inSingleLineInterpolation = old;
                }
            }

            return base.VisitInterpolatedStringExpression(node);
        }
    }
}
