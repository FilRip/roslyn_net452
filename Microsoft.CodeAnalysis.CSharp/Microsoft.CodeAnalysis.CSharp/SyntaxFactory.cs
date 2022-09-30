using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public static class SyntaxFactory
    {
        public static SyntaxTrivia CarriageReturnLineFeed { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CarriageReturnLineFeed;


        public static SyntaxTrivia LineFeed { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LineFeed;


        public static SyntaxTrivia CarriageReturn { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CarriageReturn;


        public static SyntaxTrivia Space { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Space;


        public static SyntaxTrivia Tab { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Tab;


        public static SyntaxTrivia ElasticCarriageReturnLineFeed { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturnLineFeed;


        public static SyntaxTrivia ElasticLineFeed { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticLineFeed;


        public static SyntaxTrivia ElasticCarriageReturn { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturn;


        public static SyntaxTrivia ElasticSpace { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticSpace;


        public static SyntaxTrivia ElasticTab { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticTab;


        public static SyntaxTrivia ElasticMarker { get; } = Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElasticZeroSpace;


        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression()
        {
            return AnonymousMethodExpression(default(SyntaxToken), Token(SyntaxKind.DelegateKeyword), null, Block(), null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression(SyntaxToken asyncKeyword, SyntaxToken delegateKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expressionBody)
        {
            return AnonymousMethodExpression(TokenList(asyncKeyword), delegateKeyword, parameterList, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            return Block(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), openBraceToken, statements, closeBraceToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BreakStatementSyntax BreakStatement(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            return BreakStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), breakKeyword, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax CheckedStatement(SyntaxKind kind, SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            return CheckedStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), keyword, block);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
        {
            return ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, body, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
        {
            return ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, null, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            return ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, null, expressionBody, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax ContinueStatement(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            return ContinueStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), continueKeyword, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
        {
            return DestructorDeclaration(attributeLists, modifiers, Token(SyntaxKind.TildeToken), identifier, parameterList, body, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken tildeToken, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return DestructorDeclaration(attributeLists, modifiers, tildeToken, identifier, parameterList, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
        {
            return DestructorDeclaration(attributeLists, modifiers, Token(SyntaxKind.TildeToken), identifier, parameterList, null, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken tildeToken, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            return DestructorDeclaration(attributeLists, modifiers, tildeToken, identifier, parameterList, null, expressionBody, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax DoStatement(SyntaxToken doKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            return DoStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax EmptyStatement(SyntaxToken semicolonToken)
        {
            return EmptyStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax ExpressionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return ExpressionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), expression, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax FixedStatement(SyntaxToken fixedKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return FixedStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), fixedKeyword, openParenToken, declaration, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachStatement(default(SyntaxToken), forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachVariableStatement(default(SyntaxToken), forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachVariableStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), awaitKeyword, forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax ForStatement(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> initializers, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> incrementors, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), declaration, initializers, condition, incrementors, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax ForStatement(SyntaxToken forKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> incrementors, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxToken caseOrDefaultKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return GotoStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), caseOrDefaultKeyword, expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return GotoStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax IfStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax @else)
        {
            return IfStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), condition, statement, @else);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax IfStatement(SyntaxToken ifKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax @else)
        {
            return IfStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax IndexerDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
        {
            return IndexerDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, parameterList, accessorList, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax LabeledStatement(SyntaxToken identifier, SyntaxToken colonToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LabeledStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), identifier, colonToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            return LocalDeclarationStatement(default(SyntaxToken), default(SyntaxToken), modifiers, declaration, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            return LocalDeclarationStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return LocalDeclarationStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, declaration);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
        {
            return LocalFunctionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            return LocalFunctionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LockStatementSyntax LockStatement(SyntaxToken lockKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LockStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), lockKeyword, openParenToken, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return MethodDeclaration(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, constraintClauses, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxToken asyncKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return ParenthesizedLambdaExpression(TokenList(asyncKeyword), parameterList, arrowToken, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return ParenthesizedLambdaExpression(default(SyntaxTokenList), parameterList, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return ParenthesizedLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, parameterList, arrowToken, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return ParenthesizedLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, parameterList, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
        {
            return AccessorDeclaration(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(GetAccessorDeclarationKeywordKind(kind)), body, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax? parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            return RecordDeclaration(SyntaxKind.RecordDeclaration, attributeLists, modifiers, keyword, default(SyntaxToken), identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return RecordDeclaration(SyntaxKind.RecordDeclaration, attributeLists, modifiers, keyword, default(SyntaxToken), identifier, typeParameterList, parameterList, baseList, constraintClauses, default(SyntaxToken), members, default(SyntaxToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxToken keyword, string identifier)
        {
            return RecordDeclaration(keyword, Identifier(identifier));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxToken keyword, SyntaxToken identifier)
        {
            return RecordDeclaration(SyntaxKind.RecordDeclaration, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), keyword, default(SyntaxToken), identifier, null, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), default(SyntaxToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), default(SyntaxToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax RefType(SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return RefType(refKeyword, default(SyntaxToken), type);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax ReturnStatement(SyntaxToken returnKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return ReturnStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), returnKeyword, expression, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxToken asyncKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return SimpleLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), TokenList(asyncKeyword), parameter, arrowToken, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return SimpleLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), parameter, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return SimpleLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, parameter, arrowToken, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return SimpleLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), modifiers, parameter, block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return StackAllocArrayCreationExpression(stackAllocKeyword, type, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchStatementSyntax SwitchStatement(SyntaxToken switchKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            return SwitchStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), switchKeyword, openParenToken, expression, closeParenToken, openBraceToken, sections, closeBraceToken);
        }

        public static SyntaxTrivia EndOfLine(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text);
        }

        public static SyntaxTrivia ElasticEndOfLine(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text, elastic: true);
        }

        [Obsolete("Use SyntaxFactory.EndOfLine or SyntaxFactory.ElasticEndOfLine")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static SyntaxTrivia EndOfLine(string text, bool elastic)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text, elastic);
        }

        public static SyntaxTrivia Whitespace(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Whitespace(text);
        }

        public static SyntaxTrivia ElasticWhitespace(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Whitespace(text);
        }

        [Obsolete("Use SyntaxFactory.Whitespace or SyntaxFactory.ElasticWhitespace")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static SyntaxTrivia Whitespace(string text, bool elastic)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Whitespace(text, elastic);
        }

        public static SyntaxTrivia Comment(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Comment(text);
        }

        public static SyntaxTrivia DisabledText(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DisabledText(text);
        }

        public static SyntaxTrivia PreprocessingMessage(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PreprocessingMessage(text);
        }

        public static SyntaxTrivia SyntaxTrivia(SyntaxKind kind, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (kind - 8539 <= (SyntaxKind)4 || kind == SyntaxKind.DisabledTextTrivia)
            {
                SyntaxToken token = default(SyntaxToken);
                return new SyntaxTrivia(in token, new Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxTrivia(kind, text), 0, 0);
            }
            throw new ArgumentException("kind");
        }

        public static SyntaxToken Token(SyntaxKind kind)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Token(ElasticMarker.UnderlyingNode, kind, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Token(SyntaxTriviaList leading, SyntaxKind kind, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Token(leading.Node, kind, trailing.Node));
        }

        public static SyntaxToken Token(SyntaxTriviaList leading, SyntaxKind kind, string text, string valueText, SyntaxTriviaList trailing)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                    throw new ArgumentException(CSharpResources.UseVerbatimIdentifier, "kind");
                case SyntaxKind.CharacterLiteralToken:
                    throw new ArgumentException(CSharpResources.UseLiteralForTokens, "kind");
                case SyntaxKind.NumericLiteralToken:
                    throw new ArgumentException(CSharpResources.UseLiteralForNumeric, "kind");
                default:
                    if (!SyntaxFacts.IsAnyToken(kind))
                    {
                        throw new ArgumentException(string.Format(CSharpResources.ThisMethodCanOnlyBeUsedToCreateTokens, kind), "kind");
                    }
                    return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Token(leading.Node, kind, text, valueText, trailing.Node));
            }
        }

        public static SyntaxToken MissingToken(SyntaxKind kind)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MissingToken(ElasticMarker.UnderlyingNode, kind, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken MissingToken(SyntaxTriviaList leading, SyntaxKind kind, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MissingToken(leading.Node, kind, trailing.Node));
        }

        public static SyntaxToken Identifier(string text)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Identifier(ElasticMarker.UnderlyingNode, text, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Identifier(SyntaxTriviaList leading, string text, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Identifier(leading.Node, text, trailing.Node));
        }

        public static SyntaxToken VerbatimIdentifier(SyntaxTriviaList leading, string text, string valueText, SyntaxTriviaList trailing)
        {
            if (text.StartsWith("@", StringComparison.Ordinal))
            {
                throw new ArgumentException("text should not start with an @ character.");
            }
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Identifier(SyntaxKind.IdentifierName, leading.Node, "@" + text, valueText, trailing.Node));
        }

        public static SyntaxToken Identifier(SyntaxTriviaList leading, SyntaxKind contextualKind, string text, string valueText, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Identifier(contextualKind, leading.Node, text, valueText, trailing.Node));
        }

        public static SyntaxToken Literal(int value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.None), value);
        }

        public static SyntaxToken Literal(string text, int value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, int value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(uint value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.IncludeTypeSuffix), value);
        }

        public static SyntaxToken Literal(string text, uint value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, uint value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(long value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.IncludeTypeSuffix), value);
        }

        public static SyntaxToken Literal(string text, long value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, long value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(ulong value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.IncludeTypeSuffix), value);
        }

        public static SyntaxToken Literal(string text, ulong value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, ulong value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(float value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.IncludeTypeSuffix), value);
        }

        public static SyntaxToken Literal(string text, float value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, float value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(double value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.None), value);
        }

        public static SyntaxToken Literal(string text, double value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, double value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(decimal value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.IncludeTypeSuffix), value);
        }

        public static SyntaxToken Literal(string text, decimal value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, decimal value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(string value)
        {
            return Literal(SymbolDisplay.FormatLiteral(value, quote: true), value);
        }

        public static SyntaxToken Literal(string text, string value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, string value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken Literal(char value)
        {
            return Literal(ObjectDisplay.FormatLiteral(value, ObjectDisplayOptions.UseQuotes | ObjectDisplayOptions.EscapeNonPrintableCharacters), value);
        }

        public static SyntaxToken Literal(string text, char value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        public static SyntaxToken Literal(SyntaxTriviaList leading, string text, char value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Literal(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken BadToken(SyntaxTriviaList leading, string text, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BadToken(leading.Node, text, trailing.Node));
        }

        public static SyntaxToken XmlTextLiteral(SyntaxTriviaList leading, string text, string value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteral(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken XmlEntity(SyntaxTriviaList leading, string text, string value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlEntity(leading.Node, text, value, trailing.Node));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax DocumentationComment(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, List(content)).WithLeadingTrivia(DocumentationCommentExterior("/// ")).WithTrailingTrivia(EndOfLine(""));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlSummaryElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlSummaryElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlSummaryElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlMultiLineElement("summary", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlSeeElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref)
        {
            return XmlEmptyElement("see").AddAttributes(XmlCrefAttribute(cref));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlSeeAlsoElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref)
        {
            return XmlEmptyElement("seealso").AddAttributes(XmlCrefAttribute(cref));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlSeeAlsoElement(Uri linkAddress, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> linkText)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax xmlElementSyntax = XmlElement("seealso", linkText);
            return xmlElementSyntax.WithStartTag(xmlElementSyntax.StartTag.AddAttributes(XmlTextAttribute("cref", linkAddress.ToString())));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlThreadSafetyElement()
        {
            return XmlThreadSafetyElement(isStatic: true, isInstance: false);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlThreadSafetyElement(bool isStatic, bool isInstance)
        {
            return XmlEmptyElement("threadsafety").AddAttributes(XmlTextAttribute("static", isStatic.ToString().ToLowerInvariant()), XmlTextAttribute("instance", isInstance.ToString().ToLowerInvariant()));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax XmlNameAttribute(string parameterName)
        {
            return XmlNameAttribute(XmlName("name"), Token(SyntaxKind.DoubleQuoteToken), parameterName, Token(SyntaxKind.DoubleQuoteToken)).WithLeadingTrivia(Whitespace(" "));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlPreliminaryElement()
        {
            return XmlEmptyElement("preliminary");
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax XmlCrefAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref)
        {
            return XmlCrefAttribute(cref, SyntaxKind.DoubleQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax XmlCrefAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, SyntaxKind quoteKind)
        {
            cref = cref.ReplaceTokens(cref.DescendantTokens(), XmlReplaceBracketTokens);
            return XmlCrefAttribute(XmlName("cref"), Token(quoteKind), cref, Token(quoteKind)).WithLeadingTrivia(Whitespace(" "));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlRemarksElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlRemarksElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlRemarksElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlMultiLineElement("remarks", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlReturnsElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlReturnsElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlReturnsElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlMultiLineElement("returns", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlValueElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlValueElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlValueElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlMultiLineElement("value", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlExceptionElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlExceptionElement(cref, List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlExceptionElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax xmlElementSyntax = XmlElement("exception", content);
            return xmlElementSyntax.WithStartTag(xmlElementSyntax.StartTag.AddAttributes(XmlCrefAttribute(cref)));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlPermissionElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlPermissionElement(cref, List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlPermissionElement(Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax xmlElementSyntax = XmlElement("permission", content);
            return xmlElementSyntax.WithStartTag(xmlElementSyntax.StartTag.AddAttributes(XmlCrefAttribute(cref)));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlExampleElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlExampleElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlExampleElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax xmlElementSyntax = XmlElement("example", content);
            return xmlElementSyntax.WithStartTag(xmlElementSyntax.StartTag);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlParaElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlParaElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlParaElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlElement("para", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlParamElement(string parameterName, params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlParamElement(parameterName, List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlParamElement(string parameterName, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax xmlElementSyntax = XmlElement("param", content);
            return xmlElementSyntax.WithStartTag(xmlElementSyntax.StartTag.AddAttributes(XmlNameAttribute(parameterName)));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlParamRefElement(string parameterName)
        {
            return XmlEmptyElement("paramref").AddAttributes(XmlNameAttribute(parameterName));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlNullKeywordElement()
        {
            return XmlKeywordElement("null");
        }

        private static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlKeywordElement(string keyword)
        {
            return XmlEmptyElement("see").AddAttributes(XmlTextAttribute("langword", keyword));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlPlaceholderElement(params Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax[] content)
        {
            return XmlPlaceholderElement(List(content));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlPlaceholderElement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlElement("placeholder", content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlEmptyElement(string localName)
        {
            return XmlEmptyElement(XmlName(localName));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlElement(string localName, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlElement(XmlName(localName), content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlElement(XmlElementStartTag(name), content, XmlElementEndTag(name));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(string name, string value)
        {
            return XmlTextAttribute(name, XmlTextLiteral(value));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(string name, params SyntaxToken[] textTokens)
        {
            return XmlTextAttribute(XmlName(name), SyntaxKind.DoubleQuoteToken, TokenList(textTokens));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(string name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return XmlTextAttribute(XmlName(name), quoteKind, textTokens);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return XmlTextAttribute(name, Token(quoteKind), textTokens, Token(quoteKind)).WithLeadingTrivia(Whitespace(" "));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlMultiLineElement(string localName, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlMultiLineElement(XmlName(localName), content);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlMultiLineElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content)
        {
            return XmlElement(XmlElementStartTag(name), content, XmlElementEndTag(name));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax XmlNewLine(string text)
        {
            return XmlText(XmlTextNewLine(text));
        }

        public static SyntaxToken XmlTextNewLine(string text)
        {
            return XmlTextNewLine(text, continueXmlDocumentationComment: true);
        }

        public static SyntaxToken XmlTextNewLine(SyntaxTriviaList leading, string text, string value, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlTextNewLine(leading.Node, text, value, trailing.Node));
        }

        public static SyntaxToken XmlTextNewLine(string text, bool continueXmlDocumentationComment)
        {
            SyntaxToken result = new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlTextNewLine(ElasticMarker.UnderlyingNode, text, text, ElasticMarker.UnderlyingNode));
            if (continueXmlDocumentationComment)
            {
                return result.WithTrailingTrivia(result.TrailingTrivia.Add(DocumentationCommentExterior("/// ")));
            }
            return result;
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax XmlText(string value)
        {
            return XmlText(XmlTextLiteral(value));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax XmlText(params SyntaxToken[] textTokens)
        {
            return XmlText(TokenList(textTokens));
        }

        public static SyntaxToken XmlTextLiteral(string value)
        {
            string text = new XText(value).ToString();
            return XmlTextLiteral(TriviaList(), text, value, TriviaList());
        }

        public static SyntaxToken XmlTextLiteral(string text, string value)
        {
            return new SyntaxToken(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteral(ElasticMarker.UnderlyingNode, text, value, ElasticMarker.UnderlyingNode));
        }

        private static SyntaxToken XmlReplaceBracketTokens(SyntaxToken originalToken, SyntaxToken rewrittenToken)
        {
            if (rewrittenToken.IsKind(SyntaxKind.LessThanToken) && string.Equals("<", rewrittenToken.Text, StringComparison.Ordinal))
            {
                return Token(rewrittenToken.LeadingTrivia, SyntaxKind.LessThanToken, "{", rewrittenToken.ValueText, rewrittenToken.TrailingTrivia);
            }
            if (rewrittenToken.IsKind(SyntaxKind.GreaterThanToken) && string.Equals(">", rewrittenToken.Text, StringComparison.Ordinal))
            {
                return Token(rewrittenToken.LeadingTrivia, SyntaxKind.GreaterThanToken, "}", rewrittenToken.ValueText, rewrittenToken.TrailingTrivia);
            }
            return rewrittenToken;
        }

        public static SyntaxTrivia DocumentationCommentExterior(string text)
        {
            return Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentExteriorTrivia(text);
        }

        public static SyntaxList<TNode> List<TNode>() where TNode : SyntaxNode
        {
            return default(SyntaxList<TNode>);
        }

        public static SyntaxList<TNode> SingletonList<TNode>(TNode node) where TNode : SyntaxNode
        {
            return new SyntaxList<TNode>(node);
        }

        public static SyntaxList<TNode> List<TNode>(IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return new SyntaxList<TNode>(nodes);
        }

        public static SyntaxTokenList TokenList()
        {
            return default(SyntaxTokenList);
        }

        public static SyntaxTokenList TokenList(SyntaxToken token)
        {
            return new SyntaxTokenList(token);
        }

        public static SyntaxTokenList TokenList(params SyntaxToken[] tokens)
        {
            return new SyntaxTokenList(tokens);
        }

        public static SyntaxTokenList TokenList(IEnumerable<SyntaxToken> tokens)
        {
            return new SyntaxTokenList(tokens);
        }

        public static SyntaxTrivia Trivia(Microsoft.CodeAnalysis.CSharp.Syntax.StructuredTriviaSyntax node)
        {
            SyntaxToken token = default(SyntaxToken);
            return new SyntaxTrivia(in token, node.Green, 0, 0);
        }

        public static SyntaxTriviaList TriviaList()
        {
            return default(SyntaxTriviaList);
        }

        public static SyntaxTriviaList TriviaList(SyntaxTrivia trivia)
        {
            return new SyntaxTriviaList(trivia);
        }

        public static SyntaxTriviaList TriviaList(params SyntaxTrivia[] trivias)
        {
            return new SyntaxTriviaList(trivias);
        }

        public static SyntaxTriviaList TriviaList(IEnumerable<SyntaxTrivia> trivias)
        {
            return new SyntaxTriviaList(trivias);
        }

        public static SeparatedSyntaxList<TNode> SeparatedList<TNode>() where TNode : SyntaxNode
        {
            return default(SeparatedSyntaxList<TNode>);
        }

        public static SeparatedSyntaxList<TNode> SingletonSeparatedList<TNode>(TNode node) where TNode : SyntaxNode
        {
            return new SeparatedSyntaxList<TNode>(new SyntaxNodeOrTokenList(node, 0));
        }

        public static SeparatedSyntaxList<TNode> SeparatedList<TNode>(IEnumerable<TNode>? nodes) where TNode : SyntaxNode
        {
            if (nodes == null)
            {
                return default(SeparatedSyntaxList<TNode>);
            }
            ICollection<TNode> collection = nodes as ICollection<TNode>;
            if (collection != null && collection.Count == 0)
            {
                return default(SeparatedSyntaxList<TNode>);
            }
            using IEnumerator<TNode> enumerator = nodes!.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(SeparatedSyntaxList<TNode>);
            }
            TNode current = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                return SingletonSeparatedList(current);
            }
            Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TNode> separatedSyntaxListBuilder = new Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TNode>(collection?.Count ?? 3);
            separatedSyntaxListBuilder.Add(current);
            SyntaxToken separatorToken = Token(SyntaxKind.CommaToken);
            do
            {
                separatedSyntaxListBuilder.AddSeparator(in separatorToken);
                separatedSyntaxListBuilder.Add(enumerator.Current);
            }
            while (enumerator.MoveNext());
            return separatedSyntaxListBuilder.ToList();
        }

        public static SeparatedSyntaxList<TNode> SeparatedList<TNode>(IEnumerable<TNode> nodes, IEnumerable<SyntaxToken> separators) where TNode : SyntaxNode
        {
            if (nodes != null)
            {
                IEnumerator<TNode> enumerator = nodes.GetEnumerator();
                Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TNode> separatedSyntaxListBuilder = Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder<TNode>.Create();
                if (separators != null)
                {
                    foreach (SyntaxToken separator in separators)
                    {
                        SyntaxToken separatorToken = separator;
                        if (!enumerator.MoveNext())
                        {
                            throw new ArgumentException("nodes must not be empty.", "nodes");
                        }
                        separatedSyntaxListBuilder.Add(enumerator.Current);
                        separatedSyntaxListBuilder.AddSeparator(in separatorToken);
                    }
                }
                if (enumerator.MoveNext())
                {
                    separatedSyntaxListBuilder.Add(enumerator.Current);
                    if (enumerator.MoveNext())
                    {
                        throw new ArgumentException("separators must have 1 fewer element than nodes", "separators");
                    }
                }
                return separatedSyntaxListBuilder.ToList();
            }
            if (separators != null)
            {
                throw new ArgumentException("When nodes is null, separators must also be null.", "separators");
            }
            return default(SeparatedSyntaxList<TNode>);
        }

        public static SeparatedSyntaxList<TNode> SeparatedList<TNode>(IEnumerable<SyntaxNodeOrToken> nodesAndTokens) where TNode : SyntaxNode
        {
            return SeparatedList<TNode>(NodeOrTokenList(nodesAndTokens));
        }

        public static SeparatedSyntaxList<TNode> SeparatedList<TNode>(SyntaxNodeOrTokenList nodesAndTokens) where TNode : SyntaxNode
        {
            if (!HasSeparatedNodeTokenPattern(nodesAndTokens))
            {
                throw new ArgumentException(CodeAnalysisResources.NodeOrTokenOutOfSequence);
            }
            if (!NodesAreCorrectType<TNode>(nodesAndTokens))
            {
                throw new ArgumentException(CodeAnalysisResources.UnexpectedTypeOfNodeInList);
            }
            return new SeparatedSyntaxList<TNode>(nodesAndTokens);
        }

        private static bool NodesAreCorrectType<TNode>(SyntaxNodeOrTokenList list)
        {
            int i = 0;
            for (int count = list.Count; i < count; i++)
            {
                SyntaxNodeOrToken syntaxNodeOrToken = list[i];
                if (syntaxNodeOrToken.IsNode && !(syntaxNodeOrToken.AsNode() is TNode))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HasSeparatedNodeTokenPattern(SyntaxNodeOrTokenList list)
        {
            int i = 0;
            for (int count = list.Count; i < count; i++)
            {
                if (list[i].IsToken == ((i & 1) == 0))
                {
                    return false;
                }
            }
            return true;
        }

        public static SyntaxNodeOrTokenList NodeOrTokenList()
        {
            return default(SyntaxNodeOrTokenList);
        }

        public static SyntaxNodeOrTokenList NodeOrTokenList(IEnumerable<SyntaxNodeOrToken> nodesAndTokens)
        {
            return new SyntaxNodeOrTokenList(nodesAndTokens);
        }

        public static SyntaxNodeOrTokenList NodeOrTokenList(params SyntaxNodeOrToken[] nodesAndTokens)
        {
            return new SyntaxNodeOrTokenList(nodesAndTokens);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax IdentifierName(string name)
        {
            return IdentifierName(Identifier(name));
        }

        public static SyntaxTree SyntaxTree(SyntaxNode root, ParseOptions? options = null, string path = "", Encoding? encoding = null)
        {
            return CSharpSyntaxTree.Create((CSharpSyntaxNode)root, (CSharpParseOptions)options, path, encoding);
        }

        public static SyntaxTree ParseSyntaxTree(string text, ParseOptions? options = null, string path = "", Encoding? encoding = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)options, path, encoding, cancellationToken);
        }

        public static SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions? options = null, string path = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            return CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)options, path, cancellationToken);
        }

        public static SyntaxTriviaList ParseLeadingTrivia(string text, int offset = 0)
        {
            return ParseLeadingTrivia(text, CSharpParseOptions.Default, offset);
        }

        internal static SyntaxTriviaList ParseLeadingTrivia(string text, CSharpParseOptions? options, int offset = 0)
        {
            using Lexer lexer = new Lexer(MakeSourceText(text, offset), options);
            return lexer.LexSyntaxLeadingTrivia();
        }

        public static SyntaxTriviaList ParseTrailingTrivia(string text, int offset = 0)
        {
            using Lexer lexer = new Lexer(MakeSourceText(text, offset), CSharpParseOptions.Default);
            return lexer.LexSyntaxTrailingTrivia();
        }

        internal static Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax? ParseCref(string text)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax xmlAttributeSyntax = ((Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax)((Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax)ParseLeadingTrivia($"/// <see cref=\"{text}\"/>", CSharpParseOptions.Default.WithDocumentationMode(DocumentationMode.Diagnose)).First().GetStructure()).Content[1]).Attributes[0];
            if (xmlAttributeSyntax.Kind() != SyntaxKind.XmlCrefAttribute)
            {
                return null;
            }
            return ((Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax)xmlAttributeSyntax).Cref;
        }

        public static SyntaxToken ParseToken(string text, int offset = 0)
        {
            using Lexer lexer = new Lexer(MakeSourceText(text, offset), CSharpParseOptions.Default);
            return new SyntaxToken(lexer.Lex(LexerMode.Syntax));
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text, int offset = 0, int initialTokenPosition = 0, CSharpParseOptions? options = null)
        {
            using Lexer lexer = new Lexer(MakeSourceText(text, offset), options ?? CSharpParseOptions.Default);
            int position = initialTokenPosition;
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken token;
            do
            {
                token = lexer.Lex(LexerMode.Syntax);
                yield return new SyntaxToken(null, token, position, 0);
                position += token.FullWidth;
            }
            while (token.Kind != SyntaxKind.EndOfFileToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax ParseName(string text, int offset = 0, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax nameSyntax = languageParser.ParseName();
            if (consumeFullText)
            {
                nameSyntax = languageParser.ConsumeUnexpectedTokens(nameSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax)nameSyntax.CreateRed();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ParseTypeName(string text, int offset, bool consumeFullText)
        {
            return ParseTypeName(text, offset, null, consumeFullText);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ParseTypeName(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax typeSyntax = languageParser.ParseTypeName();
            if (consumeFullText)
            {
                typeSyntax = languageParser.ConsumeUnexpectedTokens(typeSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax)typeSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax ParseExpression(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax expressionSyntax = languageParser.ParseExpression();
            if (consumeFullText)
            {
                expressionSyntax = languageParser.ConsumeUnexpectedTokens(expressionSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)expressionSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax ParseStatement(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax statementSyntax = languageParser.ParseStatement();
            if (consumeFullText)
            {
                statementSyntax = languageParser.ConsumeUnexpectedTokens(statementSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax)statementSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax? ParseMemberDeclaration(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax memberDeclarationSyntax = languageParser.ParseMemberDeclaration();
            if (memberDeclarationSyntax == null)
            {
                return null;
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax)(consumeFullText ? languageParser.ConsumeUnexpectedTokens(memberDeclarationSyntax) : memberDeclarationSyntax).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax ParseCompilationUnit(string text, int offset = 0, CSharpParseOptions? options = null)
        {
            using Lexer lexer = MakeLexer(text, offset, options);
            using LanguageParser languageParser = MakeParser(lexer);
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)languageParser.ParseCompilationUnit().CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax ParseParameterList(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax parameterListSyntax = languageParser.ParseParenthesizedParameterList();
            if (consumeFullText)
            {
                parameterListSyntax = languageParser.ConsumeUnexpectedTokens(parameterListSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax)parameterListSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax ParseBracketedParameterList(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedParameterListSyntax bracketedParameterListSyntax = languageParser.ParseBracketedParameterList();
            if (consumeFullText)
            {
                bracketedParameterListSyntax = languageParser.ConsumeUnexpectedTokens(bracketedParameterListSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax)bracketedParameterListSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax ParseArgumentList(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax argumentListSyntax = languageParser.ParseParenthesizedArgumentList();
            if (consumeFullText)
            {
                argumentListSyntax = languageParser.ConsumeUnexpectedTokens(argumentListSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax)argumentListSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax ParseBracketedArgumentList(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax bracketedArgumentListSyntax = languageParser.ParseBracketedArgumentList();
            if (consumeFullText)
            {
                bracketedArgumentListSyntax = languageParser.ConsumeUnexpectedTokens(bracketedArgumentListSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax)bracketedArgumentListSyntax.CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax ParseAttributeArgumentList(string text, int offset = 0, ParseOptions? options = null, bool consumeFullText = true)
        {
            using Lexer lexer = MakeLexer(text, offset, (CSharpParseOptions)options);
            using LanguageParser languageParser = MakeParser(lexer);
            Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeArgumentListSyntax attributeArgumentListSyntax = languageParser.ParseAttributeArgumentList();
            if (consumeFullText)
            {
                attributeArgumentListSyntax = languageParser.ConsumeUnexpectedTokens(attributeArgumentListSyntax);
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax)attributeArgumentListSyntax.CreateRed();
        }

        private static SourceText MakeSourceText(string text, int offset)
        {
            return SourceText.From(text, Encoding.UTF8).GetSubText(offset);
        }

        private static Lexer MakeLexer(string text, int offset, CSharpParseOptions? options = null)
        {
            return new Lexer(MakeSourceText(text, offset), options ?? CSharpParseOptions.Default);
        }

        private static LanguageParser MakeParser(Lexer lexer)
        {
            return new LanguageParser(lexer, null, null);
        }

        public static bool AreEquivalent(SyntaxTree? oldTree, SyntaxTree? newTree, bool topLevel)
        {
            if (oldTree == null && newTree == null)
            {
                return true;
            }
            if (oldTree == null || newTree == null)
            {
                return false;
            }
            return SyntaxEquivalence.AreEquivalent(oldTree, newTree, null, topLevel);
        }

        public static bool AreEquivalent(SyntaxNode? oldNode, SyntaxNode? newNode, bool topLevel)
        {
            return SyntaxEquivalence.AreEquivalent(oldNode, newNode, null, topLevel);
        }

        public static bool AreEquivalent(SyntaxNode? oldNode, SyntaxNode? newNode, Func<SyntaxKind, bool>? ignoreChildNode = null)
        {
            return SyntaxEquivalence.AreEquivalent(oldNode, newNode, ignoreChildNode, topLevel: false);
        }

        public static bool AreEquivalent(SyntaxToken oldToken, SyntaxToken newToken)
        {
            return SyntaxEquivalence.AreEquivalent(oldToken, newToken);
        }

        public static bool AreEquivalent(SyntaxTokenList oldList, SyntaxTokenList newList)
        {
            return SyntaxEquivalence.AreEquivalent(oldList, newList);
        }

        public static bool AreEquivalent<TNode>(SyntaxList<TNode> oldList, SyntaxList<TNode> newList, bool topLevel) where TNode : CSharpSyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, null, topLevel);
        }

        public static bool AreEquivalent<TNode>(SyntaxList<TNode> oldList, SyntaxList<TNode> newList, Func<SyntaxKind, bool>? ignoreChildNode = null) where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, ignoreChildNode, topLevel: false);
        }

        public static bool AreEquivalent<TNode>(SeparatedSyntaxList<TNode> oldList, SeparatedSyntaxList<TNode> newList, bool topLevel) where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, null, topLevel);
        }

        public static bool AreEquivalent<TNode>(SeparatedSyntaxList<TNode> oldList, SeparatedSyntaxList<TNode> newList, Func<SyntaxKind, bool>? ignoreChildNode = null) where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, ignoreChildNode, topLevel: false);
        }

        internal static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? GetStandaloneType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? node)
        {
            if (node != null && node!.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expressionSyntax && (node!.Kind() == SyntaxKind.IdentifierName || node!.Kind() == SyntaxKind.GenericName))
            {
                switch (expressionSyntax.Kind())
                {
                    case SyntaxKind.QualifiedName:
                        {
                            Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualifiedNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)expressionSyntax;
                            if (qualifiedNameSyntax.Right == node)
                            {
                                return qualifiedNameSyntax;
                            }
                            break;
                        }
                    case SyntaxKind.AliasQualifiedName:
                        {
                            Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax aliasQualifiedNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)expressionSyntax;
                            if (aliasQualifiedNameSyntax.Name == node)
                            {
                                return aliasQualifiedNameSyntax;
                            }
                            break;
                        }
                }
            }
            return node;
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax GetStandaloneExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return (GetStandaloneNode(expression) as Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax) ?? expression;
        }

        internal static CSharpSyntaxNode? GetStandaloneNode(CSharpSyntaxNode? node)
        {
            if (node == null || (!(node is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax) && !(node is Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax)))
            {
                return node;
            }
            switch (node!.Kind())
            {
                default:
                    return node;
                case SyntaxKind.NameMemberCref:
                case SyntaxKind.IndexerMemberCref:
                case SyntaxKind.OperatorMemberCref:
                case SyntaxKind.ConversionOperatorMemberCref:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                case SyntaxKind.ArrayType:
                case SyntaxKind.NullableType:
                    {
                        CSharpSyntaxNode parent = node!.Parent;
                        if (parent == null)
                        {
                            return node;
                        }
                        switch (parent.Kind())
                        {
                            case SyntaxKind.QualifiedName:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)parent).Right == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.AliasQualifiedName:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)parent).Name == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.SimpleMemberAccessExpression:
                            case SyntaxKind.PointerMemberAccessExpression:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax)parent).Name == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.MemberBindingExpression:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.MemberBindingExpressionSyntax)parent).Name == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.NameMemberCref:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.NameMemberCrefSyntax)parent).Name == node)
                                {
                                    CSharpSyntaxNode parent2 = parent.Parent;
                                    if (parent2 == null || parent2.Kind() != SyntaxKind.QualifiedCref)
                                    {
                                        return parent;
                                    }
                                    return parent2;
                                }
                                break;
                            case SyntaxKind.QualifiedCref:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedCrefSyntax)parent).Member == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.ArrayCreationExpression:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax)parent).Type == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.ObjectCreationExpression:
                                if (node!.Kind() == SyntaxKind.NullableType && ((Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax)parent).Type == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.StackAllocArrayCreationExpression:
                                if (((Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax)parent).Type == node)
                                {
                                    return parent;
                                }
                                break;
                            case SyntaxKind.NameColon:
                                if (parent.Parent.IsKind(SyntaxKind.Subpattern))
                                {
                                    return parent.Parent;
                                }
                                break;
                        }
                        return node;
                    }
            }
        }

        internal static Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax? FindConditionalAccessNodeForBinding(CSharpSyntaxNode node)
        {
            CSharpSyntaxNode cSharpSyntaxNode = node;
            while (cSharpSyntaxNode != null)
            {
                cSharpSyntaxNode = cSharpSyntaxNode.Parent;
                if (cSharpSyntaxNode.Kind() == SyntaxKind.ConditionalAccessExpression)
                {
                    Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax)cSharpSyntaxNode;
                    if (conditionalAccessExpressionSyntax.OperatorToken.EndPosition == node.Position)
                    {
                        return conditionalAccessExpressionSyntax;
                    }
                }
            }
            return null;
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? GetNonGenericExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (expression != null)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.PointerMemberAccessExpression:
                        {
                            Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax memberAccessExpressionSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax)expression;
                            if (memberAccessExpressionSyntax.Name.Kind() == SyntaxKind.GenericName)
                            {
                                Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax genericNameSyntax2 = (Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax)memberAccessExpressionSyntax.Name;
                                return BinaryExpression(expression.Kind(), memberAccessExpressionSyntax.Expression, memberAccessExpressionSyntax.OperatorToken, IdentifierName(genericNameSyntax2.Identifier));
                            }
                            break;
                        }
                    case SyntaxKind.QualifiedName:
                        {
                            Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualifiedNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)expression;
                            if (qualifiedNameSyntax.Right.Kind() == SyntaxKind.GenericName)
                            {
                                Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax genericNameSyntax3 = (Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax)qualifiedNameSyntax.Right;
                                return QualifiedName(qualifiedNameSyntax.Left, qualifiedNameSyntax.DotToken, IdentifierName(genericNameSyntax3.Identifier));
                            }
                            break;
                        }
                    case SyntaxKind.AliasQualifiedName:
                        {
                            Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax aliasQualifiedNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)expression;
                            if (aliasQualifiedNameSyntax.Name.Kind() == SyntaxKind.GenericName)
                            {
                                Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax genericNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax)aliasQualifiedNameSyntax.Name;
                                return AliasQualifiedName(aliasQualifiedNameSyntax.Alias, aliasQualifiedNameSyntax.ColonColonToken, IdentifierName(genericNameSyntax.Identifier));
                            }
                            break;
                        }
                }
            }
            return expression;
        }

        public static bool IsCompleteSubmission(SyntaxTree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException("tree");
            }
            if (tree.Options.Kind != SourceCodeKind.Script)
            {
                throw new ArgumentException(CSharpResources.SyntaxTreeIsNotASubmission);
            }
            if (!tree.HasCompilationUnitRoot)
            {
                return false;
            }
            Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnitSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)tree.GetRoot();
            if (!compilationUnitSyntax.HasErrors)
            {
                return true;
            }
            foreach (Diagnostic diagnostic in compilationUnitSyntax.EndOfFileToken.GetDiagnostics())
            {
                ErrorCode code = (ErrorCode)diagnostic.Code;
                if (code == ErrorCode.ERR_EndifDirectiveExpected || code == ErrorCode.ERR_OpenEndedComment || code == ErrorCode.ERR_EndRegionDirectiveExpected)
                {
                    return false;
                }
            }
            SyntaxNode syntaxNode = compilationUnitSyntax.ChildNodes().LastOrDefault();
            if (syntaxNode == null)
            {
                return true;
            }
            if (syntaxNode.HasTrailingTrivia && syntaxNode.ContainsDiagnostics && HasUnterminatedMultiLineComment(syntaxNode.GetTrailingTrivia()))
            {
                return false;
            }
            if (syntaxNode.IsKind(SyntaxKind.IncompleteMember))
            {
                return false;
            }
            if (!syntaxNode.IsKind(SyntaxKind.GlobalStatement))
            {
                return !syntaxNode.GetLastToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true).IsMissing;
            }
            Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax globalStatementSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax)syntaxNode;
            SyntaxToken token = syntaxNode.GetLastToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true);
            if (token.IsMissing)
            {
                if (tree.Options.Kind == SourceCodeKind.Regular || !globalStatementSyntax.Statement.IsKind(SyntaxKind.ExpressionStatement) || !token.IsKind(SyntaxKind.SemicolonToken))
                {
                    return false;
                }
                token = token.GetPreviousToken(SyntaxToken.Any, Microsoft.CodeAnalysis.SyntaxTrivia.Any);
                if (token.IsMissing)
                {
                    return false;
                }
            }
            using (IEnumerator<Diagnostic> enumerator = token.GetDiagnostics().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current.Code)
                    {
                        case 1010:
                        case 1022:
                        case 1039:
                        case 7017:
                            return false;
                    }
                }
            }
            return true;
        }

        private static bool HasUnterminatedMultiLineComment(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator enumerator = triviaList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxTrivia current = enumerator.Current;
                if (current.ContainsDiagnostics && current.Kind() == SyntaxKind.MultiLineCommentTrivia)
                {
                    return true;
                }
            }
            return false;
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CaseSwitchLabelSyntax CaseSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value)
        {
            return CaseSwitchLabel(Token(SyntaxKind.CaseKeyword), value, Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultSwitchLabelSyntax DefaultSwitchLabel()
        {
            return DefaultSwitchLabel(Token(SyntaxKind.DefaultKeyword), Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(params Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax[] statements)
        {
            return Block(List(statements));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
        {
            return Block(List(statements));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
        {
            return PropertyDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, identifier, accessorList, null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return ConversionOperatorDeclaration(attributeLists, modifiers, implicitOrExplicitKeyword, operatorKeyword, type, parameterList, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax OperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return OperatorDeclaration(attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingDirective(Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax alias, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return UsingDirective(Token(SyntaxKind.UsingKeyword), default(SyntaxToken), alias, name, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingDirective(SyntaxToken usingKeyword, SyntaxToken staticKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax? alias, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxToken semicolonToken)
        {
            return UsingDirective(default(SyntaxToken), usingKeyword, staticKeyword, alias, name, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax ClassOrStructConstraint(SyntaxKind kind, SyntaxToken classOrStructKeyword)
        {
            return ClassOrStructConstraint(kind, classOrStructKeyword, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
        {
            return AccessorDeclaration(kind, attributeLists, modifiers, body, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, SyntaxToken semicolonToken)
        {
            return AccessorDeclaration(kind, attributeLists, modifiers, keyword, body, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
        {
            return AccessorDeclaration(kind, attributeLists, modifiers, null, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            return AccessorDeclaration(kind, attributeLists, modifiers, keyword, null, expressionBody, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax equalsValue)
        {
            return EnumMemberDeclaration(attributeLists, default(SyntaxTokenList), identifier, equalsValue);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax NamespaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return NamespaceDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), name, externs, usings, members);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax NamespaceDeclaration(SyntaxToken namespaceKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            return NamespaceDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
        {
            return EventDeclaration(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            return EventDeclaration(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, null, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchStatementSyntax SwitchStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax> sections)
        {
            bool num = !(expression is Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax);
            SyntaxToken openParenToken = (num ? Token(SyntaxKind.OpenParenToken) : default(SyntaxToken));
            SyntaxToken closeParenToken = (num ? Token(SyntaxKind.CloseParenToken) : default(SyntaxToken));
            return SwitchStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.SwitchKeyword), openParenToken, expression, closeParenToken, Token(SyntaxKind.OpenBraceToken), sections, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchStatementSyntax SwitchStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return SwitchStatement(expression, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                return SimpleLambdaExpression(parameter, null, (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)body);
            }
            return SimpleLambdaExpression(parameter, block, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxToken asyncKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, SyntaxToken arrowToken, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                return SimpleLambdaExpression(asyncKeyword, parameter, arrowToken, null, (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)body);
            }
            return SimpleLambdaExpression(asyncKeyword, parameter, arrowToken, block, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(CSharpSyntaxNode body)
        {
            return ParenthesizedLambdaExpression(ParameterList(), body);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                return ParenthesizedLambdaExpression(parameterList, null, (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)body);
            }
            return ParenthesizedLambdaExpression(parameterList, block, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxToken asyncKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxToken arrowToken, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                return ParenthesizedLambdaExpression(asyncKeyword, parameterList, arrowToken, null, (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)body);
            }
            return ParenthesizedLambdaExpression(asyncKeyword, parameterList, arrowToken, block, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression(CSharpSyntaxNode body)
        {
            return AnonymousMethodExpression(null, body);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax? parameterList, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                throw new ArgumentException("body");
            }
            return AnonymousMethodExpression(default(SyntaxTokenList), Token(SyntaxKind.DelegateKeyword), parameterList, block, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression(SyntaxToken asyncKeyword, SyntaxToken delegateKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, CSharpSyntaxNode body)
        {
            if (!(body is Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block))
            {
                throw new ArgumentException("body");
            }
            return AnonymousMethodExpression(asyncKeyword, delegateKeyword, parameterList, block, null);
        }

        [Obsolete("The diagnosticOptions parameter is obsolete due to performance problems, if you are passing non-null use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static SyntaxTree ParseSyntaxTree(string text, ParseOptions? options, string path, Encoding? encoding, ImmutableDictionary<string, ReportDiagnostic>? diagnosticOptions, CancellationToken cancellationToken)
        {
            return ParseSyntaxTree(SourceText.From(text, encoding), options, path, diagnosticOptions, null, cancellationToken);
        }

        [Obsolete("The diagnosticOptions parameter is obsolete due to performance problems, if you are passing non-null use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions? options, string path, ImmutableDictionary<string, ReportDiagnostic>? diagnosticOptions, CancellationToken cancellationToken)
        {
            return CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)options, path, diagnosticOptions, null, cancellationToken);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("The diagnosticOptions and isGeneratedCode parameters are obsolete due to performance problems, if you are using them use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        public static SyntaxTree ParseSyntaxTree(string text, ParseOptions? options, string path, Encoding? encoding, ImmutableDictionary<string, ReportDiagnostic>? diagnosticOptions, bool? isGeneratedCode, CancellationToken cancellationToken)
        {
            return ParseSyntaxTree(SourceText.From(text, encoding), options, path, diagnosticOptions, isGeneratedCode, cancellationToken);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("The diagnosticOptions and isGeneratedCode parameters are obsolete due to performance problems, if you are using them use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        public static SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions? options, string path, ImmutableDictionary<string, ReportDiagnostic>? diagnosticOptions, bool? isGeneratedCode, CancellationToken cancellationToken)
        {
            return CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)options, path, diagnosticOptions, isGeneratedCode, cancellationToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax ThrowStatement(SyntaxToken throwKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return ThrowStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), throwKeyword, expression, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax TryStatement(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax> catches, Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax @finally)
        {
            return TryStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), block, catches, @finally);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax TryStatement(SyntaxToken tryKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax> catches, Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax @finally)
        {
            return TryStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), tryKeyword, block, catches, @finally);
        }

        internal static SyntaxKind GetTypeDeclarationKeywordKind(DeclarationKind kind)
        {
            return kind switch
            {
                DeclarationKind.Class => SyntaxKind.ClassKeyword,
                DeclarationKind.Struct => SyntaxKind.StructKeyword,
                DeclarationKind.Interface => SyntaxKind.InterfaceKeyword,
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        private static SyntaxKind GetTypeDeclarationKeywordKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return SyntaxKind.ClassKeyword;
                case SyntaxKind.StructDeclaration:
                    return SyntaxKind.StructKeyword;
                case SyntaxKind.InterfaceDeclaration:
                    return SyntaxKind.InterfaceKeyword;
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return SyntaxKind.RecordKeyword;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, SyntaxToken identifier)
        {
            return TypeDeclaration(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(GetTypeDeclarationKeywordKind(kind)), identifier, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, string identifier)
        {
            return TypeDeclaration(kind, Identifier(identifier));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributes, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            return kind switch
            {
                SyntaxKind.ClassDeclaration => ClassDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken),
                SyntaxKind.StructDeclaration => StructDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken),
                SyntaxKind.InterfaceDeclaration => InterfaceDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken),
                SyntaxKind.RecordDeclaration => RecordDeclaration(SyntaxKind.RecordDeclaration, attributes, modifiers, keyword, default(SyntaxToken), identifier, typeParameterList, null, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken),
                SyntaxKind.RecordStructDeclaration => RecordDeclaration(SyntaxKind.RecordStructDeclaration, attributes, modifiers, keyword, Token(SyntaxKind.StructKeyword), identifier, typeParameterList, null, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken),
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeStatementSyntax UnsafeStatement(SyntaxToken unsafeKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            return UnsafeStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), unsafeKeyword, block);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(SyntaxToken usingKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return UsingStatement(default(SyntaxToken), usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return UsingStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return UsingStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), declaration, expression, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax WhileStatement(SyntaxToken whileKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return WhileStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), whileKeyword, openParenToken, condition, closeParenToken, statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return YieldStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), yieldKeyword, returnOrBreakKeyword, expression, semicolonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax IdentifierName(SyntaxToken identifier)
        {
            SyntaxKind syntaxKind = identifier.Kind();
            if (syntaxKind != SyntaxKind.GlobalKeyword && syntaxKind != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IdentifierName((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax QualifiedName(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax left, SyntaxToken dotToken, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (dotToken.Kind() != SyntaxKind.DotToken)
            {
                throw new ArgumentException("dotToken");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.QualifiedName((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax)left.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)dotToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SimpleNameSyntax)right.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax QualifiedName(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax right)
        {
            return QualifiedName(left, Token(SyntaxKind.DotToken), right);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax GenericName(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeArgumentListSyntax typeArgumentList)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (typeArgumentList == null)
            {
                throw new ArgumentNullException("typeArgumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.GenericName((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeArgumentListSyntax)typeArgumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax GenericName(SyntaxToken identifier)
        {
            return GenericName(identifier, TypeArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax GenericName(string identifier)
        {
            return GenericName(Identifier(identifier), TypeArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeArgumentListSyntax TypeArgumentList(SyntaxToken lessThanToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax> arguments, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken)
            {
                throw new ArgumentException("lessThanToken");
            }
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken)
            {
                throw new ArgumentException("greaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeArgumentListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeArgumentList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node, arguments.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeArgumentListSyntax TypeArgumentList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax> arguments = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax>))
        {
            return TypeArgumentList(Token(SyntaxKind.LessThanToken), arguments, Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax AliasQualifiedName(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax alias, SyntaxToken colonColonToken, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            if (alias == null)
            {
                throw new ArgumentNullException("alias");
            }
            if (colonColonToken.Kind() != SyntaxKind.ColonColonToken)
            {
                throw new ArgumentException("colonColonToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AliasQualifiedName((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IdentifierNameSyntax)alias.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonColonToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax AliasQualifiedName(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax alias, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            return AliasQualifiedName(alias, Token(SyntaxKind.ColonColonToken), name);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax AliasQualifiedName(string alias, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            return AliasQualifiedName(IdentifierName(alias), Token(SyntaxKind.ColonColonToken), name);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax PredefinedType(SyntaxToken keyword)
        {
            SyntaxKind syntaxKind = keyword.Kind();
            if (syntaxKind - 8304 > (SyntaxKind)15)
            {
                throw new ArgumentException("keyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PredefinedType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax ArrayType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ArrayType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)elementType.Green, rankSpecifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrayRankSpecifierSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax ArrayType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType)
        {
            return ArrayType(elementType, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax ArrayRankSpecifier(SyntaxToken openBracketToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> sizes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ArrayRankSpecifier((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, sizes.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayRankSpecifierSyntax ArrayRankSpecifier(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> sizes = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>))
        {
            return ArrayRankSpecifier(Token(SyntaxKind.OpenBracketToken), sizes, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PointerTypeSyntax PointerType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType, SyntaxToken asteriskToken)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            if (asteriskToken.Kind() != SyntaxKind.AsteriskToken)
            {
                throw new ArgumentException("asteriskToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PointerTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PointerType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)elementType.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)asteriskToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PointerTypeSyntax PointerType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType)
        {
            return PointerType(elementType, Token(SyntaxKind.AsteriskToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax FunctionPointerType(SyntaxToken delegateKeyword, SyntaxToken asteriskToken, Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax? callingConvention, Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax parameterList)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword)
            {
                throw new ArgumentException("delegateKeyword");
            }
            if (asteriskToken.Kind() != SyntaxKind.AsteriskToken)
            {
                throw new ArgumentException("asteriskToken");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)asteriskToken.Node, (callingConvention == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerCallingConventionSyntax)callingConvention!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerParameterListSyntax)parameterList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax FunctionPointerType(Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax? callingConvention, Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax parameterList)
        {
            return FunctionPointerType(Token(SyntaxKind.DelegateKeyword), Token(SyntaxKind.AsteriskToken), callingConvention, parameterList);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax FunctionPointerType()
        {
            return FunctionPointerType(Token(SyntaxKind.DelegateKeyword), Token(SyntaxKind.AsteriskToken), null, FunctionPointerParameterList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax FunctionPointerParameterList(SyntaxToken lessThanToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken)
            {
                throw new ArgumentException("lessThanToken");
            }
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken)
            {
                throw new ArgumentException("greaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax FunctionPointerParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax>))
        {
            return FunctionPointerParameterList(Token(SyntaxKind.LessThanToken), parameters, Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax FunctionPointerCallingConvention(SyntaxToken managedOrUnmanagedKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
        {
            SyntaxKind syntaxKind = managedOrUnmanagedKeyword.Kind();
            if (syntaxKind - 8445 > SyntaxKind.List)
            {
                throw new ArgumentException("managedOrUnmanagedKeyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerCallingConvention((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)managedOrUnmanagedKeyword.Node, (unmanagedCallingConventionList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)unmanagedCallingConventionList!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax FunctionPointerCallingConvention(SyntaxToken managedOrUnmanagedKeyword)
        {
            return FunctionPointerCallingConvention(managedOrUnmanagedKeyword, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax FunctionPointerUnmanagedCallingConventionList(SyntaxToken openBracketToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax> callingConventions, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerUnmanagedCallingConventionList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, callingConventions.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax FunctionPointerUnmanagedCallingConventionList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax> callingConventions = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax>))
        {
            return FunctionPointerUnmanagedCallingConventionList(Token(SyntaxKind.OpenBracketToken), callingConventions, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax FunctionPointerUnmanagedCallingConvention(SyntaxToken name)
        {
            if (name.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerUnmanagedCallingConvention((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)name.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax NullableType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType, SyntaxToken questionToken)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            if (questionToken.Kind() != SyntaxKind.QuestionToken)
            {
                throw new ArgumentException("questionToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NullableType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)elementType.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)questionToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax NullableType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax elementType)
        {
            return NullableType(elementType, Token(SyntaxKind.QuestionToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax TupleType(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TupleType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, elements.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleElementSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax TupleType(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax>))
        {
            return TupleType(Token(SyntaxKind.OpenParenToken), elements, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax TupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            SyntaxKind syntaxKind = identifier.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TupleElement((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax TupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return TupleElement(type, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OmittedTypeArgumentSyntax OmittedTypeArgument(SyntaxToken omittedTypeArgumentToken)
        {
            if (omittedTypeArgumentToken.Kind() != SyntaxKind.OmittedTypeArgumentToken)
            {
                throw new ArgumentException("omittedTypeArgumentToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.OmittedTypeArgumentSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.OmittedTypeArgument((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)omittedTypeArgumentToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OmittedTypeArgumentSyntax OmittedTypeArgument()
        {
            return OmittedTypeArgument(Token(SyntaxKind.OmittedTypeArgumentToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax RefType(SyntaxToken refKeyword, SyntaxToken readOnlyKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (refKeyword.Kind() != SyntaxKind.RefKeyword)
            {
                throw new ArgumentException("refKeyword");
            }
            SyntaxKind syntaxKind = readOnlyKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.ReadOnlyKeyword)
            {
                throw new ArgumentException("readOnlyKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RefType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)refKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)readOnlyKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax RefType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return RefType(Token(SyntaxKind.RefKeyword), default(SyntaxToken), type);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedExpressionSyntax ParenthesizedExpression(SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ParenthesizedExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedExpressionSyntax ParenthesizedExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ParenthesizedExpression(Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax TupleExpression(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TupleExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, arguments.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax TupleExpression(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>))
        {
            return TupleExpression(Token(SyntaxKind.OpenParenToken), arguments, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PrefixUnaryExpressionSyntax PrefixUnaryExpression(SyntaxKind kind, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax operand)
        {
            if (kind - 8730 > (SyntaxKind)7 && kind != SyntaxKind.IndexExpression)
            {
                throw new ArgumentException("kind");
            }
            switch (operatorToken.Kind())
            {
                default:
                    throw new ArgumentException("operatorToken");
                case SyntaxKind.TildeToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                    if (operand == null)
                    {
                        throw new ArgumentNullException("operand");
                    }
                    return (Microsoft.CodeAnalysis.CSharp.Syntax.PrefixUnaryExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PrefixUnaryExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)operand.Green).CreateRed();
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PrefixUnaryExpressionSyntax PrefixUnaryExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax operand)
        {
            return PrefixUnaryExpression(kind, Token(GetPrefixUnaryExpressionOperatorTokenKind(kind)), operand);
        }

        private static SyntaxKind GetPrefixUnaryExpressionOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.UnaryPlusExpression => SyntaxKind.PlusToken,
                SyntaxKind.UnaryMinusExpression => SyntaxKind.MinusToken,
                SyntaxKind.BitwiseNotExpression => SyntaxKind.TildeToken,
                SyntaxKind.LogicalNotExpression => SyntaxKind.ExclamationToken,
                SyntaxKind.PreIncrementExpression => SyntaxKind.PlusPlusToken,
                SyntaxKind.PreDecrementExpression => SyntaxKind.MinusMinusToken,
                SyntaxKind.AddressOfExpression => SyntaxKind.AmpersandToken,
                SyntaxKind.PointerIndirectionExpression => SyntaxKind.AsteriskToken,
                SyntaxKind.IndexExpression => SyntaxKind.CaretToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AwaitExpressionSyntax AwaitExpression(SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (awaitKeyword.Kind() != SyntaxKind.AwaitKeyword)
            {
                throw new ArgumentException("awaitKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AwaitExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AwaitExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AwaitExpressionSyntax AwaitExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return AwaitExpression(Token(SyntaxKind.AwaitKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PostfixUnaryExpressionSyntax PostfixUnaryExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            if (kind - 8738 > SyntaxKind.List && kind != SyntaxKind.SuppressNullableWarningExpression)
            {
                throw new ArgumentException("kind");
            }
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }
            SyntaxKind syntaxKind = operatorToken.Kind();
            if (syntaxKind != SyntaxKind.ExclamationToken && syntaxKind - 8262 > SyntaxKind.List)
            {
                throw new ArgumentException("operatorToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PostfixUnaryExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PostfixUnaryExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)operand.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PostfixUnaryExpressionSyntax PostfixUnaryExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax operand)
        {
            return PostfixUnaryExpression(kind, operand, Token(GetPostfixUnaryExpressionOperatorTokenKind(kind)));
        }

        private static SyntaxKind GetPostfixUnaryExpressionOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PostIncrementExpression => SyntaxKind.PlusPlusToken,
                SyntaxKind.PostDecrementExpression => SyntaxKind.MinusMinusToken,
                SyntaxKind.SuppressNullableWarningExpression => SyntaxKind.ExclamationToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            if (kind - 8689 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            SyntaxKind syntaxKind = operatorToken.Kind();
            if (syntaxKind != SyntaxKind.DotToken && syntaxKind != SyntaxKind.MinusGreaterThanToken)
            {
                throw new ArgumentException("operatorToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MemberAccessExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            return MemberAccessExpression(kind, expression, Token(GetMemberAccessExpressionOperatorTokenKind(kind)), name);
        }

        private static SyntaxKind GetMemberAccessExpressionOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.SimpleMemberAccessExpression => SyntaxKind.DotToken,
                SyntaxKind.PointerMemberAccessExpression => SyntaxKind.MinusGreaterThanToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax ConditionalAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenNotNull)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (operatorToken.Kind() != SyntaxKind.QuestionToken)
            {
                throw new ArgumentException("operatorToken");
            }
            if (whenNotNull == null)
            {
                throw new ArgumentNullException("whenNotNull");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConditionalAccessExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)whenNotNull.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax ConditionalAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenNotNull)
        {
            return ConditionalAccessExpression(expression, Token(SyntaxKind.QuestionToken), whenNotNull);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberBindingExpressionSyntax MemberBindingExpression(SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            if (operatorToken.Kind() != SyntaxKind.DotToken)
            {
                throw new ArgumentException("operatorToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.MemberBindingExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MemberBindingExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberBindingExpressionSyntax MemberBindingExpression(Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
        {
            return MemberBindingExpression(Token(SyntaxKind.DotToken), name);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElementBindingExpressionSyntax ElementBindingExpression(Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax argumentList)
        {
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ElementBindingExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElementBindingExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElementBindingExpressionSyntax ElementBindingExpression()
        {
            return ElementBindingExpression(BracketedArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax RangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? leftOperand, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? rightOperand)
        {
            if (operatorToken.Kind() != SyntaxKind.DotDotToken)
            {
                throw new ArgumentException("operatorToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RangeExpression((leftOperand == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)leftOperand!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (rightOperand == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)rightOperand!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax RangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? leftOperand, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? rightOperand)
        {
            return RangeExpression(leftOperand, Token(SyntaxKind.DotDotToken), rightOperand);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax RangeExpression()
        {
            return RangeExpression(null, Token(SyntaxKind.DotDotToken), null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitElementAccessSyntax ImplicitElementAccess(Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax argumentList)
        {
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitElementAccessSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ImplicitElementAccess((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitElementAccessSyntax ImplicitElementAccess()
        {
            return ImplicitElementAccess(BracketedArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
        {
            if (kind - 8668 > (SyntaxKind)20)
            {
                throw new ArgumentException("kind");
            }
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            switch (operatorToken.Kind())
            {
                default:
                    throw new ArgumentException("operatorToken");
                case SyntaxKind.PercentToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.QuestionQuestionToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.AsKeyword:
                    if (right == null)
                    {
                        throw new ArgumentNullException("right");
                    }
                    return (Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BinaryExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)left.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)right.Green).CreateRed();
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
        {
            return BinaryExpression(kind, left, Token(GetBinaryExpressionOperatorTokenKind(kind)), right);
        }

        private static SyntaxKind GetBinaryExpressionOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.AddExpression => SyntaxKind.PlusToken,
                SyntaxKind.SubtractExpression => SyntaxKind.MinusToken,
                SyntaxKind.MultiplyExpression => SyntaxKind.AsteriskToken,
                SyntaxKind.DivideExpression => SyntaxKind.SlashToken,
                SyntaxKind.ModuloExpression => SyntaxKind.PercentToken,
                SyntaxKind.LeftShiftExpression => SyntaxKind.LessThanLessThanToken,
                SyntaxKind.RightShiftExpression => SyntaxKind.GreaterThanGreaterThanToken,
                SyntaxKind.LogicalOrExpression => SyntaxKind.BarBarToken,
                SyntaxKind.LogicalAndExpression => SyntaxKind.AmpersandAmpersandToken,
                SyntaxKind.BitwiseOrExpression => SyntaxKind.BarToken,
                SyntaxKind.BitwiseAndExpression => SyntaxKind.AmpersandToken,
                SyntaxKind.ExclusiveOrExpression => SyntaxKind.CaretToken,
                SyntaxKind.EqualsExpression => SyntaxKind.EqualsEqualsToken,
                SyntaxKind.NotEqualsExpression => SyntaxKind.ExclamationEqualsToken,
                SyntaxKind.LessThanExpression => SyntaxKind.LessThanToken,
                SyntaxKind.LessThanOrEqualExpression => SyntaxKind.LessThanEqualsToken,
                SyntaxKind.GreaterThanExpression => SyntaxKind.GreaterThanToken,
                SyntaxKind.GreaterThanOrEqualExpression => SyntaxKind.GreaterThanEqualsToken,
                SyntaxKind.IsExpression => SyntaxKind.IsKeyword,
                SyntaxKind.AsExpression => SyntaxKind.AsKeyword,
                SyntaxKind.CoalesceExpression => SyntaxKind.QuestionQuestionToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax AssignmentExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
        {
            if (kind - 8714 > (SyntaxKind)11)
            {
                throw new ArgumentException("kind");
            }
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            SyntaxKind syntaxKind = operatorToken.Kind();
            if (syntaxKind != SyntaxKind.EqualsToken && syntaxKind != SyntaxKind.LessThanLessThanEqualsToken && syntaxKind - 8275 > (SyntaxKind)9)
            {
                throw new ArgumentException("operatorToken");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AssignmentExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)left.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)right.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax AssignmentExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
        {
            return AssignmentExpression(kind, left, Token(GetAssignmentExpressionOperatorTokenKind(kind)), right);
        }

        private static SyntaxKind GetAssignmentExpressionOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.SimpleAssignmentExpression => SyntaxKind.EqualsToken,
                SyntaxKind.AddAssignmentExpression => SyntaxKind.PlusEqualsToken,
                SyntaxKind.SubtractAssignmentExpression => SyntaxKind.MinusEqualsToken,
                SyntaxKind.MultiplyAssignmentExpression => SyntaxKind.AsteriskEqualsToken,
                SyntaxKind.DivideAssignmentExpression => SyntaxKind.SlashEqualsToken,
                SyntaxKind.ModuloAssignmentExpression => SyntaxKind.PercentEqualsToken,
                SyntaxKind.AndAssignmentExpression => SyntaxKind.AmpersandEqualsToken,
                SyntaxKind.ExclusiveOrAssignmentExpression => SyntaxKind.CaretEqualsToken,
                SyntaxKind.OrAssignmentExpression => SyntaxKind.BarEqualsToken,
                SyntaxKind.LeftShiftAssignmentExpression => SyntaxKind.LessThanLessThanEqualsToken,
                SyntaxKind.RightShiftAssignmentExpression => SyntaxKind.GreaterThanGreaterThanEqualsToken,
                SyntaxKind.CoalesceAssignmentExpression => SyntaxKind.QuestionQuestionEqualsToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax ConditionalExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken questionToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenTrue, SyntaxToken colonToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenFalse)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (questionToken.Kind() != SyntaxKind.QuestionToken)
            {
                throw new ArgumentException("questionToken");
            }
            if (whenTrue == null)
            {
                throw new ArgumentNullException("whenTrue");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            if (whenFalse == null)
            {
                throw new ArgumentNullException("whenFalse");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConditionalExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)questionToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)whenTrue.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)whenFalse.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax ConditionalExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenTrue, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax whenFalse)
        {
            return ConditionalExpression(condition, Token(SyntaxKind.QuestionToken), whenTrue, Token(SyntaxKind.ColonToken), whenFalse);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThisExpressionSyntax ThisExpression(SyntaxToken token)
        {
            if (token.Kind() != SyntaxKind.ThisKeyword)
            {
                throw new ArgumentException("token");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ThisExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ThisExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)token.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThisExpressionSyntax ThisExpression()
        {
            return ThisExpression(Token(SyntaxKind.ThisKeyword));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseExpressionSyntax BaseExpression(SyntaxToken token)
        {
            if (token.Kind() != SyntaxKind.BaseKeyword)
            {
                throw new ArgumentException("token");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BaseExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BaseExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)token.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseExpressionSyntax BaseExpression()
        {
            return BaseExpression(Token(SyntaxKind.BaseKeyword));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax LiteralExpression(SyntaxKind kind, SyntaxToken token)
        {
            if (kind - 8748 > (SyntaxKind)7)
            {
                throw new ArgumentException("kind");
            }
            switch (token.Kind())
            {
                default:
                    throw new ArgumentException("token");
                case SyntaxKind.NullKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.StringLiteralToken:
                    return (Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LiteralExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)token.Node).CreateRed();
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax LiteralExpression(SyntaxKind kind)
        {
            return LiteralExpression(kind, Token(GetLiteralExpressionTokenKind(kind)));
        }

        private static SyntaxKind GetLiteralExpressionTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.ArgListExpression => SyntaxKind.ArgListKeyword,
                SyntaxKind.NumericLiteralExpression => SyntaxKind.NumericLiteralToken,
                SyntaxKind.StringLiteralExpression => SyntaxKind.StringLiteralToken,
                SyntaxKind.CharacterLiteralExpression => SyntaxKind.CharacterLiteralToken,
                SyntaxKind.TrueLiteralExpression => SyntaxKind.TrueKeyword,
                SyntaxKind.FalseLiteralExpression => SyntaxKind.FalseKeyword,
                SyntaxKind.NullLiteralExpression => SyntaxKind.NullKeyword,
                SyntaxKind.DefaultLiteralExpression => SyntaxKind.DefaultKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MakeRefExpressionSyntax MakeRefExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.MakeRefKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.MakeRefExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MakeRefExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MakeRefExpressionSyntax MakeRefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return MakeRefExpression(Token(SyntaxKind.MakeRefKeyword), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeExpressionSyntax RefTypeExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.RefTypeKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RefTypeExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeExpressionSyntax RefTypeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return RefTypeExpression(Token(SyntaxKind.RefTypeKeyword), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefValueExpressionSyntax RefValueExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken comma, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.RefValueKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (comma.Kind() != SyntaxKind.CommaToken)
            {
                throw new ArgumentException("comma");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RefValueExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RefValueExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)comma.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefValueExpressionSyntax RefValueExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return RefValueExpression(Token(SyntaxKind.RefValueKeyword), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CommaToken), type, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax CheckedExpression(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (kind - 8762 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            SyntaxKind syntaxKind = keyword.Kind();
            if (syntaxKind - 8379 > SyntaxKind.List)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CheckedExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax CheckedExpression(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return CheckedExpression(kind, Token(GetCheckedExpressionKeywordKind(kind)), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken));
        }

        private static SyntaxKind GetCheckedExpressionKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.CheckedExpression => SyntaxKind.CheckedKeyword,
                SyntaxKind.UncheckedExpression => SyntaxKind.UncheckedKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultExpressionSyntax DefaultExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.DefaultKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DefaultExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DefaultExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultExpressionSyntax DefaultExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return DefaultExpression(Token(SyntaxKind.DefaultKeyword), Token(SyntaxKind.OpenParenToken), type, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeOfExpressionSyntax TypeOfExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.TypeOfKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeOfExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeOfExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeOfExpressionSyntax TypeOfExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return TypeOfExpression(Token(SyntaxKind.TypeOfKeyword), Token(SyntaxKind.OpenParenToken), type, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SizeOfExpressionSyntax SizeOfExpression(SyntaxToken keyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.SizeOfKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SizeOfExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SizeOfExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SizeOfExpressionSyntax SizeOfExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return SizeOfExpression(Token(SyntaxKind.SizeOfKeyword), Token(SyntaxKind.OpenParenToken), type, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax InvocationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax argumentList)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InvocationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax InvocationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return InvocationExpression(expression, ArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax ElementAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax argumentList)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElementAccessExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax ElementAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ElementAccessExpression(expression, BracketedArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax ArgumentList(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ArgumentList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, arguments.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax ArgumentList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>))
        {
            return ArgumentList(Token(SyntaxKind.OpenParenToken), arguments, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax BracketedArgumentList(SyntaxToken openBracketToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BracketedArgumentList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, arguments.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax BracketedArgumentList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>))
        {
            return BracketedArgumentList(Token(SyntaxKind.OpenBracketToken), arguments, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax Argument(Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax? nameColon, SyntaxToken refKindKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            SyntaxKind syntaxKind = refKindKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8360 > (SyntaxKind)2)
            {
                throw new ArgumentException("refKindKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Argument((nameColon == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameColonSyntax)nameColon!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)refKindKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax Argument(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return Argument(null, default(SyntaxToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax NameColon(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name, SyntaxToken colonToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NameColon((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax NameColon(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name)
        {
            return NameColon(name, Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax NameColon(string name)
        {
            return NameColon(IdentifierName(name), Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax DeclarationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (designation == null)
            {
                throw new ArgumentNullException("designation");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DeclarationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax CastExpression(SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CastExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax CastExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return CastExpression(Token(SyntaxKind.OpenParenToken), type, Token(SyntaxKind.CloseParenToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax AnonymousMethodExpression(SyntaxTokenList modifiers, SyntaxToken delegateKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax? parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword)
            {
                throw new ArgumentException("delegateKeyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousMethodExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AnonymousMethodExpression(modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node, (parameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green, (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expressionBody!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken)
            {
                throw new ArgumentException("arrowToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SimpleLambdaExpression(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterSyntax)parameter.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)arrowToken.Node, (block == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expressionBody!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return SimpleLambdaExpression(attributeLists, modifiers, parameter, Token(SyntaxKind.EqualsGreaterThanToken), block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax SimpleLambdaExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter)
        {
            return SimpleLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), parameter, Token(SyntaxKind.EqualsGreaterThanToken), null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax RefExpression(SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (refKeyword.Kind() != SyntaxKind.RefKeyword)
            {
                throw new ArgumentException("refKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RefExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)refKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax RefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return RefExpression(Token(SyntaxKind.RefKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken)
            {
                throw new ArgumentException("arrowToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ParenthesizedLambdaExpression(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)arrowToken.Node, (block == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expressionBody!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expressionBody)
        {
            return ParenthesizedLambdaExpression(attributeLists, modifiers, parameterList, Token(SyntaxKind.EqualsGreaterThanToken), block, expressionBody);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression()
        {
            return ParenthesizedLambdaExpression(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), ParameterList(), Token(SyntaxKind.EqualsGreaterThanToken), null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax InitializerExpression(SyntaxKind kind, SyntaxToken openBraceToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            if (kind - 8644 > (SyntaxKind)2 && kind != SyntaxKind.ComplexElementInitializerExpression && kind != SyntaxKind.WithInitializerExpression)
            {
                throw new ArgumentException("kind");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InitializerExpression(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, expressions.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax InitializerExpression(SyntaxKind kind, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>))
        {
            return InitializerExpression(kind, Token(SyntaxKind.OpenBraceToken), expressions, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression(SyntaxToken newKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax argumentList, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ImplicitObjectCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green, (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax argumentList, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            return ImplicitObjectCreationExpression(Token(SyntaxKind.NewKeyword), argumentList, initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression()
        {
            return ImplicitObjectCreationExpression(Token(SyntaxKind.NewKeyword), ArgumentList(), null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax ObjectCreationExpression(SyntaxToken newKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax? argumentList, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ObjectCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (argumentList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)argumentList!.Green), (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax ObjectCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax? argumentList, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            return ObjectCreationExpression(Token(SyntaxKind.NewKeyword), type, argumentList, initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax ObjectCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return ObjectCreationExpression(Token(SyntaxKind.NewKeyword), type, null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WithExpressionSyntax WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken withKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (withKeyword.Kind() != SyntaxKind.WithKeyword)
            {
                throw new ArgumentException("withKeyword");
            }
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.WithExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.WithExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)withKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WithExpressionSyntax WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            return WithExpression(expression, Token(SyntaxKind.WithKeyword), initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax AnonymousObjectMemberDeclarator(Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax? nameEquals, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AnonymousObjectMemberDeclarator((nameEquals == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameEqualsSyntax)nameEquals!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax AnonymousObjectMemberDeclarator(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return AnonymousObjectMemberDeclarator(null, expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(SyntaxToken newKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax> initializers, SyntaxToken closeBraceToken)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AnonymousObjectCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, initializers.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AnonymousObjectMemberDeclaratorSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax> initializers = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax>))
        {
            return AnonymousObjectCreationExpression(Token(SyntaxKind.NewKeyword), Token(SyntaxKind.OpenBraceToken), initializers, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax ArrayCreationExpression(SyntaxToken newKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ArrayCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrayTypeSyntax)type.Green, (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax ArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            return ArrayCreationExpression(Token(SyntaxKind.NewKeyword), type, initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax ArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax type)
        {
            return ArrayCreationExpression(Token(SyntaxKind.NewKeyword), type, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(SyntaxToken newKeyword, SyntaxToken openBracketToken, SyntaxTokenList commas, SyntaxToken closeBracketToken, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitArrayCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ImplicitArrayCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, commas.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(SyntaxTokenList commas, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            return ImplicitArrayCreationExpression(Token(SyntaxKind.NewKeyword), Token(SyntaxKind.OpenBracketToken), commas, Token(SyntaxKind.CloseBracketToken), initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            return ImplicitArrayCreationExpression(Token(SyntaxKind.NewKeyword), Token(SyntaxKind.OpenBracketToken), default(SyntaxTokenList), Token(SyntaxKind.CloseBracketToken), initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            if (stackAllocKeyword.Kind() != SyntaxKind.StackAllocKeyword)
            {
                throw new ArgumentException("stackAllocKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.StackAllocArrayCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)stackAllocKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax? initializer)
        {
            return StackAllocArrayCreationExpression(Token(SyntaxKind.StackAllocKeyword), type, initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return StackAllocArrayCreationExpression(Token(SyntaxKind.StackAllocKeyword), type, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax ImplicitStackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword.Kind() != SyntaxKind.StackAllocKeyword)
            {
                throw new ArgumentException("stackAllocKeyword");
            }
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ImplicitStackAllocArrayCreationExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)stackAllocKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax ImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer)
        {
            return ImplicitStackAllocArrayCreationExpression(Token(SyntaxKind.StackAllocKeyword), Token(SyntaxKind.OpenBracketToken), Token(SyntaxKind.CloseBracketToken), initializer);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryExpressionSyntax QueryExpression(Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax fromClause, Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax body)
        {
            if (fromClause == null)
            {
                throw new ArgumentNullException("fromClause");
            }
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.QueryExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.QueryExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FromClauseSyntax)fromClause.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QueryBodySyntax)body.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax QueryBody(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.QueryClauseSyntax> clauses, Microsoft.CodeAnalysis.CSharp.Syntax.SelectOrGroupClauseSyntax selectOrGroup, Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax? continuation)
        {
            if (selectOrGroup == null)
            {
                throw new ArgumentNullException("selectOrGroup");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.QueryBody(clauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QueryClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SelectOrGroupClauseSyntax)selectOrGroup.Green, (continuation == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QueryContinuationSyntax)continuation!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax QueryBody(Microsoft.CodeAnalysis.CSharp.Syntax.SelectOrGroupClauseSyntax selectOrGroup)
        {
            return QueryBody(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.QueryClauseSyntax>), selectOrGroup, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax FromClause(SyntaxToken fromKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (fromKeyword.Kind() != SyntaxKind.FromKeyword)
            {
                throw new ArgumentException("fromKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (inKeyword.Kind() != SyntaxKind.InKeyword)
            {
                throw new ArgumentException("inKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FromClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)fromKeyword.Node, (type == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)inKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax FromClause(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return FromClause(Token(SyntaxKind.FromKeyword), type, identifier, Token(SyntaxKind.InKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax FromClause(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return FromClause(Token(SyntaxKind.FromKeyword), null, identifier, Token(SyntaxKind.InKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax FromClause(string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return FromClause(Token(SyntaxKind.FromKeyword), null, Identifier(identifier), Token(SyntaxKind.InKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LetClauseSyntax LetClause(SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (letKeyword.Kind() != SyntaxKind.LetKeyword)
            {
                throw new ArgumentException("letKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LetClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LetClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)letKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LetClauseSyntax LetClause(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return LetClause(Token(SyntaxKind.LetKeyword), identifier, Token(SyntaxKind.EqualsToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LetClauseSyntax LetClause(string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return LetClause(Token(SyntaxKind.LetKeyword), Identifier(identifier), Token(SyntaxKind.EqualsToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax JoinClause(SyntaxToken joinKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax inExpression, SyntaxToken onKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftExpression, SyntaxToken equalsKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightExpression, Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax? into)
        {
            if (joinKeyword.Kind() != SyntaxKind.JoinKeyword)
            {
                throw new ArgumentException("joinKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (inKeyword.Kind() != SyntaxKind.InKeyword)
            {
                throw new ArgumentException("inKeyword");
            }
            if (inExpression == null)
            {
                throw new ArgumentNullException("inExpression");
            }
            if (onKeyword.Kind() != SyntaxKind.OnKeyword)
            {
                throw new ArgumentException("onKeyword");
            }
            if (leftExpression == null)
            {
                throw new ArgumentNullException("leftExpression");
            }
            if (equalsKeyword.Kind() != SyntaxKind.EqualsKeyword)
            {
                throw new ArgumentException("equalsKeyword");
            }
            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.JoinClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)joinKeyword.Node, (type == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)inKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)inExpression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)onKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)leftExpression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)rightExpression.Green, (into == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.JoinIntoClauseSyntax)into!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax JoinClause(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax inExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightExpression, Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax? into)
        {
            return JoinClause(Token(SyntaxKind.JoinKeyword), type, identifier, Token(SyntaxKind.InKeyword), inExpression, Token(SyntaxKind.OnKeyword), leftExpression, Token(SyntaxKind.EqualsKeyword), rightExpression, into);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax JoinClause(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax inExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightExpression)
        {
            return JoinClause(Token(SyntaxKind.JoinKeyword), null, identifier, Token(SyntaxKind.InKeyword), inExpression, Token(SyntaxKind.OnKeyword), leftExpression, Token(SyntaxKind.EqualsKeyword), rightExpression, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax JoinClause(string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax inExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightExpression)
        {
            return JoinClause(Token(SyntaxKind.JoinKeyword), null, Identifier(identifier), Token(SyntaxKind.InKeyword), inExpression, Token(SyntaxKind.OnKeyword), leftExpression, Token(SyntaxKind.EqualsKeyword), rightExpression, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax JoinIntoClause(SyntaxToken intoKeyword, SyntaxToken identifier)
        {
            if (intoKeyword.Kind() != SyntaxKind.IntoKeyword)
            {
                throw new ArgumentException("intoKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.JoinIntoClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)intoKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax JoinIntoClause(SyntaxToken identifier)
        {
            return JoinIntoClause(Token(SyntaxKind.IntoKeyword), identifier);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.JoinIntoClauseSyntax JoinIntoClause(string identifier)
        {
            return JoinIntoClause(Token(SyntaxKind.IntoKeyword), Identifier(identifier));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhereClauseSyntax WhereClause(SyntaxToken whereKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            if (whereKeyword.Kind() != SyntaxKind.WhereKeyword)
            {
                throw new ArgumentException("whereKeyword");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.WhereClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.WhereClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whereKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhereClauseSyntax WhereClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            return WhereClause(Token(SyntaxKind.WhereKeyword), condition);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OrderByClauseSyntax OrderByClause(SyntaxToken orderByKeyword, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax> orderings)
        {
            if (orderByKeyword.Kind() != SyntaxKind.OrderByKeyword)
            {
                throw new ArgumentException("orderByKeyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.OrderByClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.OrderByClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)orderByKeyword.Node, orderings.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.OrderingSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OrderByClauseSyntax OrderByClause(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax> orderings = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax>))
        {
            return OrderByClause(Token(SyntaxKind.OrderByKeyword), orderings);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax Ordering(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken ascendingOrDescendingKeyword)
        {
            if (kind - 8782 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            SyntaxKind syntaxKind = ascendingOrDescendingKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8432 > SyntaxKind.List)
            {
                throw new ArgumentException("ascendingOrDescendingKeyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Ordering(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)ascendingOrDescendingKeyword.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OrderingSyntax Ordering(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return Ordering(kind, expression, default(SyntaxToken));
        }

        private static SyntaxKind GetOrderingAscendingOrDescendingKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.AscendingOrdering => SyntaxKind.AscendingKeyword,
                SyntaxKind.DescendingOrdering => SyntaxKind.DescendingKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SelectClauseSyntax SelectClause(SyntaxToken selectKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (selectKeyword.Kind() != SyntaxKind.SelectKeyword)
            {
                throw new ArgumentException("selectKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SelectClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SelectClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)selectKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SelectClauseSyntax SelectClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return SelectClause(Token(SyntaxKind.SelectKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GroupClauseSyntax GroupClause(SyntaxToken groupKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax groupExpression, SyntaxToken byKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax byExpression)
        {
            if (groupKeyword.Kind() != SyntaxKind.GroupKeyword)
            {
                throw new ArgumentException("groupKeyword");
            }
            if (groupExpression == null)
            {
                throw new ArgumentNullException("groupExpression");
            }
            if (byKeyword.Kind() != SyntaxKind.ByKeyword)
            {
                throw new ArgumentException("byKeyword");
            }
            if (byExpression == null)
            {
                throw new ArgumentNullException("byExpression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.GroupClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.GroupClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)groupKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)groupExpression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)byKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)byExpression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GroupClauseSyntax GroupClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax groupExpression, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax byExpression)
        {
            return GroupClause(Token(SyntaxKind.GroupKeyword), groupExpression, Token(SyntaxKind.ByKeyword), byExpression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax QueryContinuation(SyntaxToken intoKeyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax body)
        {
            if (intoKeyword.Kind() != SyntaxKind.IntoKeyword)
            {
                throw new ArgumentException("intoKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.QueryContinuation((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)intoKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QueryBodySyntax)body.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax QueryContinuation(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax body)
        {
            return QueryContinuation(Token(SyntaxKind.IntoKeyword), identifier, body);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QueryContinuationSyntax QueryContinuation(string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.QueryBodySyntax body)
        {
            return QueryContinuation(Token(SyntaxKind.IntoKeyword), Identifier(identifier), body);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OmittedArraySizeExpressionSyntax OmittedArraySizeExpression(SyntaxToken omittedArraySizeExpressionToken)
        {
            if (omittedArraySizeExpressionToken.Kind() != SyntaxKind.OmittedArraySizeExpressionToken)
            {
                throw new ArgumentException("omittedArraySizeExpressionToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.OmittedArraySizeExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.OmittedArraySizeExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)omittedArraySizeExpressionToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OmittedArraySizeExpressionSyntax OmittedArraySizeExpression()
        {
            return OmittedArraySizeExpression(Token(SyntaxKind.OmittedArraySizeExpressionToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringContentSyntax> contents, SyntaxToken stringEndToken)
        {
            SyntaxKind syntaxKind = stringStartToken.Kind();
            if (syntaxKind != SyntaxKind.InterpolatedStringStartToken && syntaxKind != SyntaxKind.InterpolatedVerbatimStringStartToken)
            {
                throw new ArgumentException("stringStartToken");
            }
            if (stringEndToken.Kind() != SyntaxKind.InterpolatedStringEndToken)
            {
                throw new ArgumentException("stringEndToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)stringStartToken.Node, contents.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolatedStringContentSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)stringEndToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringContentSyntax> contents)
        {
            return InterpolatedStringExpression(stringStartToken, contents, Token(SyntaxKind.InterpolatedStringEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken)
        {
            return InterpolatedStringExpression(stringStartToken, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringContentSyntax>), Token(SyntaxKind.InterpolatedStringEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax IsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken isKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (isKeyword.Kind() != SyntaxKind.IsKeyword)
            {
                throw new ArgumentException("isKeyword");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IsPatternExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)isKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax IsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            return IsPatternExpression(expression, Token(SyntaxKind.IsKeyword), pattern);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax ThrowExpression(SyntaxToken throwKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (throwKeyword.Kind() != SyntaxKind.ThrowKeyword)
            {
                throw new ArgumentException("throwKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ThrowExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)throwKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax ThrowExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ThrowExpression(Token(SyntaxKind.ThrowKeyword), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax WhenClause(SyntaxToken whenKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            if (whenKeyword.Kind() != SyntaxKind.WhenKeyword)
            {
                throw new ArgumentException("whenKeyword");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.WhenClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whenKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax WhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            return WhenClause(Token(SyntaxKind.WhenKeyword), condition);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax DiscardPattern(SyntaxToken underscoreToken)
        {
            if (underscoreToken.Kind() != SyntaxKind.UnderscoreToken)
            {
                throw new ArgumentException("underscoreToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DiscardPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)underscoreToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax DiscardPattern()
        {
            return DiscardPattern(Token(SyntaxKind.UnderscoreToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax DeclarationPattern(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (designation == null)
            {
                throw new ArgumentNullException("designation");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DeclarationPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax VarPattern(SyntaxToken varKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation)
        {
            if (varKeyword.Kind() != SyntaxKind.VarKeyword)
            {
                throw new ArgumentException("varKeyword");
            }
            if (designation == null)
            {
                throw new ArgumentNullException("designation");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.VarPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)varKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax VarPattern(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation)
        {
            return VarPattern(Token(SyntaxKind.VarKeyword), designation);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax RecursivePattern(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax? positionalPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax? propertyPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax? designation)
        {
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RecursivePattern((type == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type!.Green), (positionalPatternClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PositionalPatternClauseSyntax)positionalPatternClause!.Green), (propertyPatternClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PropertyPatternClauseSyntax)propertyPatternClause!.Green), (designation == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDesignationSyntax)designation!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax RecursivePattern()
        {
            return RecursivePattern(null, null, null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax PositionalPatternClause(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PositionalPatternClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, subpatterns.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SubpatternSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax PositionalPatternClause(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>))
        {
            return PositionalPatternClause(Token(SyntaxKind.OpenParenToken), subpatterns, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax PropertyPatternClause(SyntaxToken openBraceToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PropertyPatternClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, subpatterns.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SubpatternSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax PropertyPatternClause(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>))
        {
            return PropertyPatternClause(Token(SyntaxKind.OpenBraceToken), subpatterns, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax Subpattern(Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax? nameColon, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Subpattern((nameColon == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameColonSyntax)nameColon!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax Subpattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            return Subpattern(null, pattern);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax ConstantPattern(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConstantPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedPatternSyntax ParenthesizedPattern(SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ParenthesizedPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedPatternSyntax ParenthesizedPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            return ParenthesizedPattern(Token(SyntaxKind.OpenParenToken), pattern, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RelationalPatternSyntax RelationalPattern(SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            switch (operatorToken.Kind())
            {
                default:
                    throw new ArgumentException("operatorToken");
                case SyntaxKind.LessThanToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanEqualsToken:
                    if (expression == null)
                    {
                        throw new ArgumentNullException("expression");
                    }
                    return (Microsoft.CodeAnalysis.CSharp.Syntax.RelationalPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RelationalPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypePatternSyntax TypePattern(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypePatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypePattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryPatternSyntax BinaryPattern(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax left, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax right)
        {
            if (kind - 9031 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            SyntaxKind syntaxKind = operatorToken.Kind();
            if (syntaxKind - 8438 > SyntaxKind.List)
            {
                throw new ArgumentException("operatorToken");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BinaryPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BinaryPattern(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)left.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)right.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryPatternSyntax BinaryPattern(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax right)
        {
            return BinaryPattern(kind, left, Token(GetBinaryPatternOperatorTokenKind(kind)), right);
        }

        private static SyntaxKind GetBinaryPatternOperatorTokenKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.OrPattern => SyntaxKind.OrKeyword,
                SyntaxKind.AndPattern => SyntaxKind.AndKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnaryPatternSyntax UnaryPattern(SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            if (operatorToken.Kind() != SyntaxKind.NotKeyword)
            {
                throw new ArgumentException("operatorToken");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.UnaryPatternSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.UnaryPattern((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnaryPatternSyntax UnaryPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern)
        {
            return UnaryPattern(Token(SyntaxKind.NotKeyword), pattern);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringTextSyntax InterpolatedStringText(SyntaxToken textToken)
        {
            if (textToken.Kind() != SyntaxKind.InterpolatedStringTextToken)
            {
                throw new ArgumentException("textToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringTextSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringText((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)textToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringTextSyntax InterpolatedStringText()
        {
            return InterpolatedStringText(Token(SyntaxKind.InterpolatedStringTextToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax Interpolation(SyntaxToken openBraceToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationAlignmentClauseSyntax? alignmentClause, Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Interpolation((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (alignmentClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)alignmentClause!.Green), (formatClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)formatClause!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax Interpolation(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationAlignmentClauseSyntax? alignmentClause, Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax? formatClause)
        {
            return Interpolation(Token(SyntaxKind.OpenBraceToken), expression, alignmentClause, formatClause, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax Interpolation(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return Interpolation(Token(SyntaxKind.OpenBraceToken), expression, null, null, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationAlignmentClauseSyntax InterpolationAlignmentClause(SyntaxToken commaToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationAlignmentClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InterpolationAlignmentClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)commaToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)value.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax InterpolationFormatClause(SyntaxToken colonToken, SyntaxToken formatStringToken)
        {
            if (formatStringToken.Kind() != SyntaxKind.InterpolatedStringTextToken)
            {
                throw new ArgumentException("formatStringToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InterpolationFormatClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)formatStringToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationFormatClauseSyntax InterpolationFormatClause(SyntaxToken colonToken)
        {
            return InterpolationFormatClause(colonToken, Token(SyntaxKind.InterpolatedStringTextToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax GlobalStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.GlobalStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax GlobalStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return GlobalStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Block(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, statements.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
        {
            return Block(attributeLists, Token(SyntaxKind.OpenBraceToken), statements, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements = default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>))
        {
            return Block(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.OpenBraceToken), statements, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LocalFunctionStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return LocalFunctionStatement(attributeLists, modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier)
        {
            return LocalFunctionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), returnType, identifier, null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, string identifier)
        {
            return LocalFunctionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), returnType, Identifier(identifier), null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            SyntaxKind syntaxKind = awaitKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.AwaitKeyword)
            {
                throw new ArgumentException("awaitKeyword");
            }
            syntaxKind = usingKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.UsingKeyword)
            {
                throw new ArgumentException("usingKeyword");
            }
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LocalDeclarationStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)usingKeyword.Node, modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return LocalDeclarationStatement(attributeLists, default(SyntaxToken), default(SyntaxToken), modifiers, declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return LocalDeclarationStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), default(SyntaxToken), default(SyntaxTokenList), declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax> variables)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.VariableDeclaration((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, variables.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclaratorSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return VariableDeclaration(type, default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax? argumentList, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax? initializer)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.VariableDeclarator((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (argumentList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList!.Green), (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EqualsValueClauseSyntax)initializer!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier)
        {
            return VariableDeclarator(identifier, null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax VariableDeclarator(string identifier)
        {
            return VariableDeclarator(Identifier(identifier), null, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax EqualsValueClause(SyntaxToken equalsToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value)
        {
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EqualsValueClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)value.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax EqualsValueClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value)
        {
            return EqualsValueClause(Token(SyntaxKind.EqualsToken), value);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax SingleVariableDesignation(SyntaxToken identifier)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SingleVariableDesignation((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax DiscardDesignation(SyntaxToken underscoreToken)
        {
            if (underscoreToken.Kind() != SyntaxKind.UnderscoreToken)
            {
                throw new ArgumentException("underscoreToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DiscardDesignation((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)underscoreToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax DiscardDesignation()
        {
            return DiscardDesignation(Token(SyntaxKind.UnderscoreToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax ParenthesizedVariableDesignation(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ParenthesizedVariableDesignation((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, variables.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDesignationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax ParenthesizedVariableDesignation(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax>))
        {
            return ParenthesizedVariableDesignation(Token(SyntaxKind.OpenParenToken), variables, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax ExpressionStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ExpressionStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax ExpressionStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ExpressionStatement(attributeLists, expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax ExpressionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ExpressionStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax EmptyStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken semicolonToken)
        {
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax EmptyStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists)
        {
            return EmptyStatement(attributeLists, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax EmptyStatement()
        {
            return EmptyStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax LabeledStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken identifier, SyntaxToken colonToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LabeledStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax LabeledStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LabeledStatement(attributeLists, identifier, Token(SyntaxKind.ColonToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax LabeledStatement(SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LabeledStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), identifier, Token(SyntaxKind.ColonToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LabeledStatementSyntax LabeledStatement(string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LabeledStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Identifier(identifier), Token(SyntaxKind.ColonToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (kind - 8800 > (SyntaxKind)2)
            {
                throw new ArgumentException("kind");
            }
            if (gotoKeyword.Kind() != SyntaxKind.GotoKeyword)
            {
                throw new ArgumentException("gotoKeyword");
            }
            SyntaxKind syntaxKind = caseOrDefaultKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8332 > SyntaxKind.List)
            {
                throw new ArgumentException("caseOrDefaultKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.GotoStatement(kind, attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)gotoKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)caseOrDefaultKeyword.Node, (expression == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken caseOrDefaultKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression)
        {
            return GotoStatement(kind, attributeLists, Token(SyntaxKind.GotoKeyword), caseOrDefaultKeyword, expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.GotoStatementSyntax GotoStatement(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression = null)
        {
            return GotoStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.GotoKeyword), default(SyntaxToken), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BreakStatementSyntax BreakStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            if (breakKeyword.Kind() != SyntaxKind.BreakKeyword)
            {
                throw new ArgumentException("breakKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BreakStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BreakStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)breakKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BreakStatementSyntax BreakStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists)
        {
            return BreakStatement(attributeLists, Token(SyntaxKind.BreakKeyword), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BreakStatementSyntax BreakStatement()
        {
            return BreakStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.BreakKeyword), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax ContinueStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            if (continueKeyword.Kind() != SyntaxKind.ContinueKeyword)
            {
                throw new ArgumentException("continueKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ContinueStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)continueKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax ContinueStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists)
        {
            return ContinueStatement(attributeLists, Token(SyntaxKind.ContinueKeyword), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ContinueStatementSyntax ContinueStatement()
        {
            return ContinueStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.ContinueKeyword), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax ReturnStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken returnKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (returnKeyword.Kind() != SyntaxKind.ReturnKeyword)
            {
                throw new ArgumentException("returnKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ReturnStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)returnKeyword.Node, (expression == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax ReturnStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression)
        {
            return ReturnStatement(attributeLists, Token(SyntaxKind.ReturnKeyword), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax ReturnStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression = null)
        {
            return ReturnStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.ReturnKeyword), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax ThrowStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken throwKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (throwKeyword.Kind() != SyntaxKind.ThrowKeyword)
            {
                throw new ArgumentException("throwKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ThrowStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)throwKeyword.Node, (expression == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax ThrowStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression)
        {
            return ThrowStatement(attributeLists, Token(SyntaxKind.ThrowKeyword), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax ThrowStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression = null)
        {
            return ThrowStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.ThrowKeyword), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (kind - 8806 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (yieldKeyword.Kind() != SyntaxKind.YieldKeyword)
            {
                throw new ArgumentException("yieldKeyword");
            }
            SyntaxKind syntaxKind = returnOrBreakKeyword.Kind();
            if (syntaxKind != SyntaxKind.BreakKeyword && syntaxKind != SyntaxKind.ReturnKeyword)
            {
                throw new ArgumentException("returnOrBreakKeyword");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.YieldStatement(kind, attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)yieldKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)returnOrBreakKeyword.Node, (expression == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression)
        {
            return YieldStatement(kind, attributeLists, Token(SyntaxKind.YieldKeyword), Token(GetYieldStatementReturnOrBreakKeywordKind(kind)), expression, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax YieldStatement(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression = null)
        {
            return YieldStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.YieldKeyword), Token(GetYieldStatementReturnOrBreakKeywordKind(kind)), expression, Token(SyntaxKind.SemicolonToken));
        }

        private static SyntaxKind GetYieldStatementReturnOrBreakKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.YieldReturnStatement => SyntaxKind.ReturnKeyword,
                SyntaxKind.YieldBreakStatement => SyntaxKind.BreakKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax WhileStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (whileKeyword.Kind() != SyntaxKind.WhileKeyword)
            {
                throw new ArgumentException("whileKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.WhileStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whileKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax WhileStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return WhileStatement(attributeLists, Token(SyntaxKind.WhileKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WhileStatementSyntax WhileStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return WhileStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.WhileKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax DoStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken doKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            if (doKeyword.Kind() != SyntaxKind.DoKeyword)
            {
                throw new ArgumentException("doKeyword");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            if (whileKeyword.Kind() != SyntaxKind.WhileKeyword)
            {
                throw new ArgumentException("whileKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DoStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)doKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whileKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax DoStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            return DoStatement(attributeLists, Token(SyntaxKind.DoKeyword), statement, Token(SyntaxKind.WhileKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DoStatementSyntax DoStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition)
        {
            return DoStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.DoKeyword), statement, Token(SyntaxKind.WhileKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax ForStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> incrementors, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (forKeyword.Kind() != SyntaxKind.ForKeyword)
            {
                throw new ArgumentException("forKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (firstSemicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("firstSemicolonToken");
            }
            if (secondSemicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("secondSemicolonToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ForStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)forKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (declaration == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration!.Green), initializers.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)firstSemicolonToken.Node, (condition == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)secondSemicolonToken.Node, incrementors.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax ForStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> initializers, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? condition, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> incrementors, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForStatement(attributeLists, Token(SyntaxKind.ForKeyword), Token(SyntaxKind.OpenParenToken), declaration, initializers, Token(SyntaxKind.SemicolonToken), condition, Token(SyntaxKind.SemicolonToken), incrementors, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax ForStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.ForKeyword), Token(SyntaxKind.OpenParenToken), null, default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>), Token(SyntaxKind.SemicolonToken), null, Token(SyntaxKind.SemicolonToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>), Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            SyntaxKind syntaxKind = awaitKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.AwaitKeyword)
            {
                throw new ArgumentException("awaitKeyword");
            }
            if (forEachKeyword.Kind() != SyntaxKind.ForEachKeyword)
            {
                throw new ArgumentException("forEachKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (inKeyword.Kind() != SyntaxKind.InKeyword)
            {
                throw new ArgumentException("inKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ForEachStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)forEachKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)inKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachStatement(attributeLists, default(SyntaxToken), Token(SyntaxKind.ForEachKeyword), Token(SyntaxKind.OpenParenToken), type, identifier, Token(SyntaxKind.InKeyword), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), Token(SyntaxKind.ForEachKeyword), Token(SyntaxKind.OpenParenToken), type, identifier, Token(SyntaxKind.InKeyword), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax ForEachStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), Token(SyntaxKind.ForEachKeyword), Token(SyntaxKind.OpenParenToken), type, Identifier(identifier), Token(SyntaxKind.InKeyword), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            SyntaxKind syntaxKind = awaitKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.AwaitKeyword)
            {
                throw new ArgumentException("awaitKeyword");
            }
            if (forEachKeyword.Kind() != SyntaxKind.ForEachKeyword)
            {
                throw new ArgumentException("forEachKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (variable == null)
            {
                throw new ArgumentNullException("variable");
            }
            if (inKeyword.Kind() != SyntaxKind.InKeyword)
            {
                throw new ArgumentException("inKeyword");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ForEachVariableStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)forEachKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)variable.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)inKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachVariableStatement(attributeLists, default(SyntaxToken), Token(SyntaxKind.ForEachKeyword), Token(SyntaxKind.OpenParenToken), variable, Token(SyntaxKind.InKeyword), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax ForEachVariableStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ForEachVariableStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), Token(SyntaxKind.ForEachKeyword), Token(SyntaxKind.OpenParenToken), variable, Token(SyntaxKind.InKeyword), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            SyntaxKind syntaxKind = awaitKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.AwaitKeyword)
            {
                throw new ArgumentException("awaitKeyword");
            }
            if (usingKeyword.Kind() != SyntaxKind.UsingKeyword)
            {
                throw new ArgumentException("usingKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.UsingStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)usingKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (declaration == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration!.Green), (expression == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return UsingStatement(attributeLists, default(SyntaxToken), Token(SyntaxKind.UsingKeyword), Token(SyntaxKind.OpenParenToken), declaration, expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax UsingStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return UsingStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), Token(SyntaxKind.UsingKeyword), Token(SyntaxKind.OpenParenToken), null, null, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax FixedStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (fixedKeyword.Kind() != SyntaxKind.FixedKeyword)
            {
                throw new ArgumentException("fixedKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FixedStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)fixedKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax FixedStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return FixedStatement(attributeLists, Token(SyntaxKind.FixedKeyword), Token(SyntaxKind.OpenParenToken), declaration, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax FixedStatement(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return FixedStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.FixedKeyword), Token(SyntaxKind.OpenParenToken), declaration, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax CheckedStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            if (kind - 8815 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            SyntaxKind syntaxKind = keyword.Kind();
            if (syntaxKind - 8379 > SyntaxKind.List)
            {
                throw new ArgumentException("keyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CheckedStatement(kind, attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax CheckedStatement(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            return CheckedStatement(kind, attributeLists, Token(GetCheckedStatementKeywordKind(kind)), block);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedStatementSyntax CheckedStatement(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block = null)
        {
            return CheckedStatement(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(GetCheckedStatementKeywordKind(kind)), block ?? Block());
        }

        private static SyntaxKind GetCheckedStatementKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.CheckedStatement => SyntaxKind.CheckedKeyword,
                SyntaxKind.UncheckedStatement => SyntaxKind.UncheckedKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeStatementSyntax UnsafeStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken unsafeKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            if (unsafeKeyword.Kind() != SyntaxKind.UnsafeKeyword)
            {
                throw new ArgumentException("unsafeKeyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.UnsafeStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)unsafeKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeStatementSyntax UnsafeStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            return UnsafeStatement(attributeLists, Token(SyntaxKind.UnsafeKeyword), block);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeStatementSyntax UnsafeStatement(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block = null)
        {
            return UnsafeStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.UnsafeKeyword), block ?? Block());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LockStatementSyntax LockStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken lockKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (lockKeyword.Kind() != SyntaxKind.LockKeyword)
            {
                throw new ArgumentException("lockKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LockStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LockStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lockKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LockStatementSyntax LockStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LockStatement(attributeLists, Token(SyntaxKind.LockKeyword), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LockStatementSyntax LockStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return LockStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.LockKeyword), Token(SyntaxKind.OpenParenToken), expression, Token(SyntaxKind.CloseParenToken), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax IfStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax? @else)
        {
            if (ifKeyword.Kind() != SyntaxKind.IfKeyword)
            {
                throw new ArgumentException("ifKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IfStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)ifKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green, (@else == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseClauseSyntax)@else!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax IfStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax? @else)
        {
            return IfStatement(attributeLists, Token(SyntaxKind.IfKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), statement, @else);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax IfStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return IfStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.IfKeyword), Token(SyntaxKind.OpenParenToken), condition, Token(SyntaxKind.CloseParenToken), statement, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax ElseClause(SyntaxToken elseKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            if (elseKeyword.Kind() != SyntaxKind.ElseKeyword)
            {
                throw new ArgumentException("elseKeyword");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElseClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)elseKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax ElseClause(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
        {
            return ElseClause(Token(SyntaxKind.ElseKeyword), statement);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchStatementSyntax SwitchStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken switchKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            if (switchKeyword.Kind() != SyntaxKind.SwitchKeyword)
            {
                throw new ArgumentException("switchKeyword");
            }
            SyntaxKind syntaxKind = openParenToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            syntaxKind = closeParenToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SwitchStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SwitchStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)switchKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, sections.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchSectionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax SwitchSection(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax> labels, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
        {
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SwitchSection(labels.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchLabelSyntax>(), statements.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StatementSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax SwitchSection()
        {
            return SwitchSection(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax CasePatternSwitchLabel(SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax? whenClause, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.CaseKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CasePatternSwitchLabel((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green, (whenClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhenClauseSyntax)whenClause!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax CasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax? whenClause, SyntaxToken colonToken)
        {
            return CasePatternSwitchLabel(Token(SyntaxKind.CaseKeyword), pattern, whenClause, colonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax CasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, SyntaxToken colonToken)
        {
            return CasePatternSwitchLabel(Token(SyntaxKind.CaseKeyword), pattern, null, colonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CaseSwitchLabelSyntax CaseSwitchLabel(SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.CaseKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CaseSwitchLabelSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CaseSwitchLabel((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)value.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CaseSwitchLabelSyntax CaseSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value, SyntaxToken colonToken)
        {
            return CaseSwitchLabel(Token(SyntaxKind.CaseKeyword), value, colonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultSwitchLabelSyntax DefaultSwitchLabel(SyntaxToken keyword, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.DefaultKeyword)
            {
                throw new ArgumentException("keyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DefaultSwitchLabelSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DefaultSwitchLabel((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultSwitchLabelSyntax DefaultSwitchLabel(SyntaxToken colonToken)
        {
            return DefaultSwitchLabel(Token(SyntaxKind.DefaultKeyword), colonToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms, SyntaxToken closeBraceToken)
        {
            if (governingExpression == null)
            {
                throw new ArgumentNullException("governingExpression");
            }
            if (switchKeyword.Kind() != SyntaxKind.SwitchKeyword)
            {
                throw new ArgumentException("switchKeyword");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SwitchExpression((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)governingExpression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)switchKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, arms.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchExpressionArmSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms)
        {
            return SwitchExpression(governingExpression, Token(SyntaxKind.SwitchKeyword), Token(SyntaxKind.OpenBraceToken), arms, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression)
        {
            return SwitchExpression(governingExpression, Token(SyntaxKind.SwitchKeyword), Token(SyntaxKind.OpenBraceToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax>), Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (equalsGreaterThanToken.Kind() != SyntaxKind.EqualsGreaterThanToken)
            {
                throw new ArgumentException("equalsGreaterThanToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SwitchExpressionArm((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PatternSyntax)pattern.Green, (whenClause == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhenClauseSyntax)whenClause!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsGreaterThanToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax? whenClause, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return SwitchExpressionArm(pattern, whenClause, Token(SyntaxKind.EqualsGreaterThanToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return SwitchExpressionArm(pattern, null, Token(SyntaxKind.EqualsGreaterThanToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax TryStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken tryKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax> catches, Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax? @finally)
        {
            if (tryKeyword.Kind() != SyntaxKind.TryKeyword)
            {
                throw new ArgumentException("tryKeyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TryStatement(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)tryKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green, catches.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchClauseSyntax>(), (@finally == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FinallyClauseSyntax)@finally!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax TryStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax> catches, Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax? @finally)
        {
            return TryStatement(attributeLists, Token(SyntaxKind.TryKeyword), block, catches, @finally);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax TryStatement(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax> catches = default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax>))
        {
            return TryStatement(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), Token(SyntaxKind.TryKeyword), Block(), catches, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax CatchClause(SyntaxToken catchKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax? filter, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            if (catchKeyword.Kind() != SyntaxKind.CatchKeyword)
            {
                throw new ArgumentException("catchKeyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CatchClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)catchKeyword.Node, (declaration == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchDeclarationSyntax)declaration!.Green), (filter == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchFilterClauseSyntax)filter!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax CatchClause(Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax? declaration, Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax? filter, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            return CatchClause(Token(SyntaxKind.CatchKeyword), declaration, filter, block);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax CatchClause()
        {
            return CatchClause(Token(SyntaxKind.CatchKeyword), null, null, Block());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax CatchDeclaration(SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            SyntaxKind syntaxKind = identifier.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CatchDeclaration((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax CatchDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier)
        {
            return CatchDeclaration(Token(SyntaxKind.OpenParenToken), type, identifier, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax CatchDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return CatchDeclaration(Token(SyntaxKind.OpenParenToken), type, default(SyntaxToken), Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax CatchFilterClause(SyntaxToken whenKeyword, SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
        {
            if (whenKeyword.Kind() != SyntaxKind.WhenKeyword)
            {
                throw new ArgumentException("whenKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (filterExpression == null)
            {
                throw new ArgumentNullException("filterExpression");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CatchFilterClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whenKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)filterExpression.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax CatchFilterClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax filterExpression)
        {
            return CatchFilterClause(Token(SyntaxKind.WhenKeyword), Token(SyntaxKind.OpenParenToken), filterExpression, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax FinallyClause(SyntaxToken finallyKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
        {
            if (finallyKeyword.Kind() != SyntaxKind.FinallyKeyword)
            {
                throw new ArgumentException("finallyKeyword");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FinallyClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)finallyKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax FinallyClause(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? block = null)
        {
            return FinallyClause(Token(SyntaxKind.FinallyKeyword), block ?? Block());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken endOfFileToken)
        {
            if (endOfFileToken.Kind() != SyntaxKind.EndOfFileToken)
            {
                throw new ArgumentException("endOfFileToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CompilationUnit(externs.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExternAliasDirectiveSyntax>(), usings.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax>(), attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfFileToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return CompilationUnit(externs, usings, attributeLists, members, Token(SyntaxKind.EndOfFileToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit()
        {
            return CompilationUnit(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.EndOfFileToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax ExternAliasDirective(SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            if (externKeyword.Kind() != SyntaxKind.ExternKeyword)
            {
                throw new ArgumentException("externKeyword");
            }
            if (aliasKeyword.Kind() != SyntaxKind.AliasKeyword)
            {
                throw new ArgumentException("aliasKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ExternAliasDirective((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)externKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)aliasKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax ExternAliasDirective(SyntaxToken identifier)
        {
            return ExternAliasDirective(Token(SyntaxKind.ExternKeyword), Token(SyntaxKind.AliasKeyword), identifier, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax ExternAliasDirective(string identifier)
        {
            return ExternAliasDirective(Token(SyntaxKind.ExternKeyword), Token(SyntaxKind.AliasKeyword), Identifier(identifier), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingDirective(SyntaxToken globalKeyword, SyntaxToken usingKeyword, SyntaxToken staticKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax? alias, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxToken semicolonToken)
        {
            SyntaxKind syntaxKind = globalKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.GlobalKeyword)
            {
                throw new ArgumentException("globalKeyword");
            }
            if (usingKeyword.Kind() != SyntaxKind.UsingKeyword)
            {
                throw new ArgumentException("usingKeyword");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.UsingDirective((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)globalKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)usingKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)staticKeyword.Node, (alias == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameEqualsSyntax)alias!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingDirective(SyntaxToken staticKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax? alias, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return UsingDirective(default(SyntaxToken), Token(SyntaxKind.UsingKeyword), staticKeyword, alias, name, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingDirective(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return UsingDirective(default(SyntaxToken), Token(SyntaxKind.UsingKeyword), default(SyntaxToken), null, name, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax NamespaceDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken namespaceKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (namespaceKeyword.Kind() != SyntaxKind.NamespaceKeyword)
            {
                throw new ArgumentException("namespaceKeyword");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NamespaceDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)namespaceKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, externs.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExternAliasDirectiveSyntax>(), usings.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax>(), members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax NamespaceDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return NamespaceDeclaration(attributeLists, modifiers, Token(SyntaxKind.NamespaceKeyword), name, Token(SyntaxKind.OpenBraceToken), externs, usings, members, Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax NamespaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return NamespaceDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.NamespaceKeyword), name, Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax AttributeList(SyntaxToken openBracketToken, Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax? target, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> attributes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AttributeList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, (target == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)target!.Green), attributes.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax AttributeList(Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax? target, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> attributes)
        {
            return AttributeList(Token(SyntaxKind.OpenBracketToken), target, attributes, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax AttributeList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> attributes = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>))
        {
            return AttributeList(Token(SyntaxKind.OpenBracketToken), null, attributes, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax AttributeTargetSpecifier(SyntaxToken identifier, SyntaxToken colonToken)
        {
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AttributeTargetSpecifier((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeTargetSpecifierSyntax AttributeTargetSpecifier(SyntaxToken identifier)
        {
            return AttributeTargetSpecifier(identifier, Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax Attribute(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax? argumentList)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Attribute((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax)name.Green, (argumentList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeArgumentListSyntax)argumentList!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax Attribute(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return Attribute(name, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax AttributeArgumentList(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AttributeArgumentList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, arguments.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeArgumentSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax AttributeArgumentList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax> arguments = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax>))
        {
            return AttributeArgumentList(Token(SyntaxKind.OpenParenToken), arguments, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax AttributeArgument(Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax? nameEquals, Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax? nameColon, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AttributeArgument((nameEquals == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameEqualsSyntax)nameEquals!.Green), (nameColon == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameColonSyntax)nameColon!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax AttributeArgument(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return AttributeArgument(null, null, expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax NameEquals(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name, SyntaxToken equalsToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NameEquals((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax NameEquals(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name)
        {
            return NameEquals(name, Token(SyntaxKind.EqualsToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameEqualsSyntax NameEquals(string name)
        {
            return NameEquals(IdentifierName(name), Token(SyntaxKind.EqualsToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax TypeParameterList(SyntaxToken lessThanToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken)
            {
                throw new ArgumentException("lessThanToken");
            }
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken)
            {
                throw new ArgumentException("greaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax TypeParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax>))
        {
            return TypeParameterList(Token(SyntaxKind.LessThanToken), parameters, Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax TypeParameter(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxToken varianceKeyword, SyntaxToken identifier)
        {
            SyntaxKind syntaxKind = varianceKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8361 > SyntaxKind.List)
            {
                throw new ArgumentException("varianceKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeParameter(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)varianceKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax TypeParameter(SyntaxToken identifier)
        {
            return TypeParameter(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), identifier);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax TypeParameter(string identifier)
        {
            return TypeParameter(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxToken), Identifier(identifier));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.ClassKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ClassDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (baseList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)baseList!.Green), constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return ClassDeclaration(attributeLists, modifiers, Token(SyntaxKind.ClassKeyword), identifier, typeParameterList, baseList, constraintClauses, Token(SyntaxKind.OpenBraceToken), members, Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(SyntaxToken identifier)
        {
            return ClassDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.ClassKeyword), identifier, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(string identifier)
        {
            return ClassDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.ClassKeyword), Identifier(identifier), null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.StructKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.StructDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (baseList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)baseList!.Green), constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return StructDeclaration(attributeLists, modifiers, Token(SyntaxKind.StructKeyword), identifier, typeParameterList, baseList, constraintClauses, Token(SyntaxKind.OpenBraceToken), members, Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(SyntaxToken identifier)
        {
            return StructDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.StructKeyword), identifier, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(string identifier)
        {
            return StructDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.StructKeyword), Identifier(identifier), null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.InterfaceKeyword)
            {
                throw new ArgumentException("keyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.InterfaceDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (baseList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)baseList!.Green), constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return InterfaceDeclaration(attributeLists, modifiers, Token(SyntaxKind.InterfaceKeyword), identifier, typeParameterList, baseList, constraintClauses, Token(SyntaxKind.OpenBraceToken), members, Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxToken identifier)
        {
            return InterfaceDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.InterfaceKeyword), identifier, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax InterfaceDeclaration(string identifier)
        {
            return InterfaceDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.InterfaceKeyword), Identifier(identifier), null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.OpenBraceToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken classOrStructKeyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax? parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (kind != SyntaxKind.RecordDeclaration && kind != SyntaxKind.RecordStructDeclaration)
            {
                throw new ArgumentException("kind");
            }
            SyntaxKind syntaxKind = classOrStructKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8374 > SyntaxKind.List)
            {
                throw new ArgumentException("classOrStructKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            syntaxKind = openBraceToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            syntaxKind = closeBraceToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RecordDeclaration(kind, attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)classOrStructKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (parameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList!.Green), (baseList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)baseList!.Green), constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, members.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax? parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
        {
            return RecordDeclaration(kind, attributeLists, modifiers, keyword, default(SyntaxToken), identifier, typeParameterList, parameterList, baseList, constraintClauses, default(SyntaxToken), members, default(SyntaxToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxToken keyword, SyntaxToken identifier)
        {
            return RecordDeclaration(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), keyword, default(SyntaxToken), identifier, null, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), default(SyntaxToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), default(SyntaxToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxToken keyword, string identifier)
        {
            return RecordDeclaration(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), keyword, default(SyntaxToken), Identifier(identifier), null, null, null, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), default(SyntaxToken), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>), default(SyntaxToken), default(SyntaxToken));
        }

        private static SyntaxKind GetRecordDeclarationClassOrStructKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.RecordDeclaration => SyntaxKind.ClassKeyword,
                SyntaxKind.RecordStructDeclaration => SyntaxKind.StructKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax EnumDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SyntaxToken openBraceToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (enumKeyword.Kind() != SyntaxKind.EnumKeyword)
            {
                throw new ArgumentException("enumKeyword");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EnumDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)enumKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (baseList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)baseList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, members.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EnumMemberDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax EnumDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax? baseList, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax> members)
        {
            return EnumDeclaration(attributeLists, modifiers, Token(SyntaxKind.EnumKeyword), identifier, baseList, Token(SyntaxKind.OpenBraceToken), members, Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax EnumDeclaration(SyntaxToken identifier)
        {
            return EnumDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.EnumKeyword), identifier, null, Token(SyntaxKind.OpenBraceToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax EnumDeclaration(string identifier)
        {
            return EnumDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.EnumKeyword), Identifier(identifier), null, Token(SyntaxKind.OpenBraceToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax>), Token(SyntaxKind.CloseBraceToken), default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken delegateKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken semicolonToken)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword)
            {
                throw new ArgumentException("delegateKeyword");
            }
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DelegateDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return DelegateDeclaration(attributeLists, modifiers, Token(SyntaxKind.DelegateKeyword), returnType, identifier, typeParameterList, parameterList, constraintClauses, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier)
        {
            return DelegateDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.DelegateKeyword), returnType, identifier, null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, string identifier)
        {
            return DelegateDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.DelegateKeyword), returnType, Identifier(identifier), null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax? equalsValue)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EnumMemberDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (equalsValue == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EqualsValueClauseSyntax)equalsValue!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxToken identifier)
        {
            return EnumMemberDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), identifier, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax EnumMemberDeclaration(string identifier)
        {
            return EnumMemberDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Identifier(identifier), null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(SyntaxToken colonToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax> types)
        {
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BaseList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, types.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseTypeSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax> types = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax>))
        {
            return BaseList(Token(SyntaxKind.ColonToken), types);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SimpleBaseTypeSyntax SimpleBaseType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SimpleBaseTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SimpleBaseType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax argumentList)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PrimaryConstructorBaseTypeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PrimaryConstructorBaseType((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return PrimaryConstructorBaseType(type, ArgumentList());
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(SyntaxToken whereKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name, SyntaxToken colonToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintSyntax> constraints)
        {
            if (whereKeyword.Kind() != SyntaxKind.WhereKeyword)
            {
                throw new ArgumentException("whereKeyword");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeParameterConstraintClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)whereKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, constraints.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintSyntax>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintSyntax> constraints)
        {
            return TypeParameterConstraintClause(Token(SyntaxKind.WhereKeyword), name, Token(SyntaxKind.ColonToken), constraints);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax name)
        {
            return TypeParameterConstraintClause(Token(SyntaxKind.WhereKeyword), name, Token(SyntaxKind.ColonToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(string name)
        {
            return TypeParameterConstraintClause(Token(SyntaxKind.WhereKeyword), IdentifierName(name), Token(SyntaxKind.ColonToken), default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintSyntax>));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorConstraintSyntax ConstructorConstraint(SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword)
            {
                throw new ArgumentException("newKeyword");
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorConstraintSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConstructorConstraint((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)newKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorConstraintSyntax ConstructorConstraint()
        {
            return ConstructorConstraint(Token(SyntaxKind.NewKeyword), Token(SyntaxKind.OpenParenToken), Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax ClassOrStructConstraint(SyntaxKind kind, SyntaxToken classOrStructKeyword, SyntaxToken questionToken)
        {
            if (kind - 8868 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            SyntaxKind syntaxKind = classOrStructKeyword.Kind();
            if (syntaxKind - 8374 > SyntaxKind.List)
            {
                throw new ArgumentException("classOrStructKeyword");
            }
            syntaxKind = questionToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.QuestionToken)
            {
                throw new ArgumentException("questionToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ClassOrStructConstraint(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)classOrStructKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)questionToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax ClassOrStructConstraint(SyntaxKind kind)
        {
            return ClassOrStructConstraint(kind, Token(GetClassOrStructConstraintClassOrStructKeywordKind(kind)), default(SyntaxToken));
        }

        private static SyntaxKind GetClassOrStructConstraintClassOrStructKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.ClassConstraint => SyntaxKind.ClassKeyword,
                SyntaxKind.StructConstraint => SyntaxKind.StructKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeConstraintSyntax TypeConstraint(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeConstraintSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeConstraint((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultConstraintSyntax DefaultConstraint(SyntaxToken defaultKeyword)
        {
            if (defaultKeyword.Kind() != SyntaxKind.DefaultKeyword)
            {
                throw new ArgumentException("defaultKeyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DefaultConstraintSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DefaultConstraint((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)defaultKeyword.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefaultConstraintSyntax DefaultConstraint()
        {
            return DefaultConstraint(Token(SyntaxKind.DefaultKeyword));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FieldDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return FieldDeclaration(attributeLists, modifiers, declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return FieldDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (eventKeyword.Kind() != SyntaxKind.EventKeyword)
            {
                throw new ArgumentException("eventKeyword");
            }
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EventFieldDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)eventKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return EventFieldDeclaration(attributeLists, modifiers, Token(SyntaxKind.EventKeyword), declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax EventFieldDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration)
        {
            return EventFieldDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.EventKeyword), declaration, Token(SyntaxKind.SemicolonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax ExplicitInterfaceSpecifier(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, SyntaxToken dotToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (dotToken.Kind() != SyntaxKind.DotToken)
            {
                throw new ArgumentException("dotToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ExplicitInterfaceSpecifier((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)dotToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax ExplicitInterfaceSpecifier(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return ExplicitInterfaceSpecifier(name, Token(SyntaxKind.DotToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.MethodDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)returnType.Green, (explicitInterfaceSpecifier == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (typeParameterList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax? typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return MethodDeclaration(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken identifier)
        {
            return MethodDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), returnType, null, identifier, null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, string identifier)
        {
            return MethodDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), returnType, null, Identifier(identifier), null, ParameterList(), default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax OperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword)
            {
                throw new ArgumentException("operatorKeyword");
            }
            switch (operatorToken.Kind())
            {
                default:
                    throw new ArgumentException("operatorToken");
                case SyntaxKind.TildeToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.IsKeyword:
                    {
                        if (parameterList == null)
                        {
                            throw new ArgumentNullException("parameterList");
                        }
                        SyntaxKind syntaxKind = semicolonToken.Kind();
                        if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
                        {
                            throw new ArgumentException("semicolonToken");
                        }
                        return (Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.OperatorDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
                    }
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax OperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return OperatorDeclaration(attributeLists, modifiers, returnType, Token(SyntaxKind.OperatorKeyword), operatorToken, parameterList, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax OperatorDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, SyntaxToken operatorToken)
        {
            return OperatorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), returnType, Token(SyntaxKind.OperatorKeyword), operatorToken, ParameterList(), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            SyntaxKind syntaxKind = implicitOrExplicitKeyword.Kind();
            if (syntaxKind - 8383 > SyntaxKind.List)
            {
                throw new ArgumentException("implicitOrExplicitKeyword");
            }
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword)
            {
                throw new ArgumentException("operatorKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConversionOperatorDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)implicitOrExplicitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return ConversionOperatorDeclaration(attributeLists, modifiers, implicitOrExplicitKeyword, Token(SyntaxKind.OperatorKeyword), type, parameterList, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxToken implicitOrExplicitKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return ConversionOperatorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), implicitOrExplicitKeyword, Token(SyntaxKind.OperatorKeyword), type, ParameterList(), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax? initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConstructorDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorInitializerSyntax)initializer!.Green), (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax? initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxToken identifier)
        {
            return ConstructorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), identifier, ParameterList(), null, null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax ConstructorDeclaration(string identifier)
        {
            return ConstructorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Identifier(identifier), ParameterList(), null, null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax ConstructorInitializer(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax argumentList)
        {
            if (kind - 8889 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            SyntaxKind syntaxKind = thisOrBaseKeyword.Kind();
            if (syntaxKind - 8370 > SyntaxKind.List)
            {
                throw new ArgumentException("thisOrBaseKeyword");
            }
            if (argumentList == null)
            {
                throw new ArgumentNullException("argumentList");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConstructorInitializer(kind, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)thisOrBaseKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax ConstructorInitializer(SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax? argumentList = null)
        {
            return ConstructorInitializer(kind, Token(SyntaxKind.ColonToken), Token(GetConstructorInitializerThisOrBaseKeywordKind(kind)), argumentList ?? ArgumentList());
        }

        private static SyntaxKind GetConstructorInitializerThisOrBaseKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.BaseConstructorInitializer => SyntaxKind.BaseKeyword,
                SyntaxKind.ThisConstructorInitializer => SyntaxKind.ThisKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken tildeToken, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (tildeToken.Kind() != SyntaxKind.TildeToken)
            {
                throw new ArgumentException("tildeToken");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DestructorDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)tildeToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return DestructorDeclaration(attributeLists, modifiers, Token(SyntaxKind.TildeToken), identifier, parameterList, body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(SyntaxToken identifier)
        {
            return DestructorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.TildeToken), identifier, ParameterList(), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax DestructorDeclaration(string identifier)
        {
            return DestructorDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.TildeToken), Identifier(identifier), ParameterList(), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax? initializer, SyntaxToken semicolonToken)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PropertyDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (explicitInterfaceSpecifier == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (accessorList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorListSyntax)accessorList!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (initializer == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EqualsValueClauseSyntax)initializer!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax? initializer)
        {
            return PropertyDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, identifier, accessorList, expressionBody, initializer, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier)
        {
            return PropertyDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), type, null, identifier, null, null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier)
        {
            return PropertyDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), type, null, Identifier(identifier), null, null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax ArrowExpressionClause(SyntaxToken arrowToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken)
            {
                throw new ArgumentException("arrowToken");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ArrowExpressionClause((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)arrowToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax ArrowExpressionClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
        {
            return ArrowExpressionClause(Token(SyntaxKind.EqualsGreaterThanToken), expression);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList, SyntaxToken semicolonToken)
        {
            if (eventKeyword.Kind() != SyntaxKind.EventKeyword)
            {
                throw new ArgumentException("eventKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EventDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)eventKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (explicitInterfaceSpecifier == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (accessorList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorListSyntax)accessorList!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList)
        {
            return EventDeclaration(attributeLists, modifiers, Token(SyntaxKind.EventKeyword), type, explicitInterfaceSpecifier, identifier, accessorList, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, SyntaxToken identifier)
        {
            return EventDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.EventKeyword), type, null, identifier, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier)
        {
            return EventDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(SyntaxKind.EventKeyword), type, null, Identifier(identifier), null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax IndexerDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken thisKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (thisKeyword.Kind() != SyntaxKind.ThisKeyword)
            {
                throw new ArgumentException("thisKeyword");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            SyntaxKind syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IndexerDeclaration(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (explicitInterfaceSpecifier == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)thisKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedParameterListSyntax)parameterList.Green, (accessorList == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorListSyntax)accessorList!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax IndexerDeclaration(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax? accessorList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return IndexerDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, Token(SyntaxKind.ThisKeyword), parameterList, accessorList, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax IndexerDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return IndexerDeclaration(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), type, null, Token(SyntaxKind.ThisKeyword), BracketedParameterList(), null, null, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax AccessorList(SyntaxToken openBraceToken, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax> accessors, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken)
            {
                throw new ArgumentException("openBraceToken");
            }
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken)
            {
                throw new ArgumentException("closeBraceToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AccessorList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node, accessors.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorDeclarationSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax AccessorList(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax> accessors = default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax>))
        {
            return AccessorList(Token(SyntaxKind.OpenBraceToken), accessors, Token(SyntaxKind.CloseBraceToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (kind - 8896 > (SyntaxKind)4 && kind != SyntaxKind.InitAccessorDeclaration)
            {
                throw new ArgumentException("kind");
            }
            SyntaxKind syntaxKind = keyword.Kind();
            if (syntaxKind - 8417 > (SyntaxKind)3 && syntaxKind != SyntaxKind.InitKeyword && syntaxKind != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("keyword");
            }
            syntaxKind = semicolonToken.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.SemicolonToken)
            {
                throw new ArgumentException("semicolonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.AccessorDeclaration(kind, attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)keyword.Node, (body == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)body!.Green), (expressionBody == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax? body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax? expressionBody)
        {
            return AccessorDeclaration(kind, attributeLists, modifiers, Token(GetAccessorDeclarationKeywordKind(kind)), body, expressionBody, default(SyntaxToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind)
        {
            return AccessorDeclaration(kind, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), Token(GetAccessorDeclarationKeywordKind(kind)), null, null, default(SyntaxToken));
        }

        private static SyntaxKind GetAccessorDeclarationKeywordKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.GetAccessorDeclaration => SyntaxKind.GetKeyword,
                SyntaxKind.SetAccessorDeclaration => SyntaxKind.SetKeyword,
                SyntaxKind.InitAccessorDeclaration => SyntaxKind.InitKeyword,
                SyntaxKind.AddAccessorDeclaration => SyntaxKind.AddKeyword,
                SyntaxKind.RemoveAccessorDeclaration => SyntaxKind.RemoveKeyword,
                SyntaxKind.UnknownAccessorDeclaration => SyntaxKind.IdentifierToken,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax ParameterList(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax ParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax>))
        {
            return ParameterList(Token(SyntaxKind.OpenParenToken), parameters, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax BracketedParameterList(SyntaxToken openBracketToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BracketedParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax BracketedParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax>))
        {
            return BracketedParameterList(Token(SyntaxKind.OpenBracketToken), parameters, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax Parameter(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax? @default)
        {
            SyntaxKind syntaxKind = identifier.Kind();
            if (syntaxKind != SyntaxKind.ArgListKeyword && syntaxKind != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("identifier");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.Parameter(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (type == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (@default == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EqualsValueClauseSyntax)@default!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax Parameter(SyntaxToken identifier)
        {
            return Parameter(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), null, identifier, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax FunctionPointerParameter(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.FunctionPointerParameter(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax FunctionPointerParameter(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return FunctionPointerParameter(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), type);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax IncompleteMember(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type)
        {
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IncompleteMember(attributeLists.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (type == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax IncompleteMember(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type = null)
        {
            return IncompleteMember(default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>), default(SyntaxTokenList), type);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SkippedTokensTriviaSyntax SkippedTokensTrivia(SyntaxTokenList tokens)
        {
            return (Microsoft.CodeAnalysis.CSharp.Syntax.SkippedTokensTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.SkippedTokensTrivia(tokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.SkippedTokensTriviaSyntax SkippedTokensTrivia()
        {
            return SkippedTokensTrivia(default(SyntaxTokenList));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax DocumentationCommentTrivia(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content, SyntaxToken endOfComment)
        {
            if (kind - 8544 > SyntaxKind.List)
            {
                throw new ArgumentException("kind");
            }
            if (endOfComment.Kind() != SyntaxKind.EndOfDocumentationCommentToken)
            {
                throw new ArgumentException("endOfComment");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentTrivia(kind, content.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNodeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfComment.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax DocumentationCommentTrivia(SyntaxKind kind, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content = default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax>))
        {
            return DocumentationCommentTrivia(kind, content, Token(SyntaxKind.EndOfDocumentationCommentToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeCrefSyntax TypeCref(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.TypeCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.TypeCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedCrefSyntax QualifiedCref(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax container, SyntaxToken dotToken, Microsoft.CodeAnalysis.CSharp.Syntax.MemberCrefSyntax member)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (dotToken.Kind() != SyntaxKind.DotToken)
            {
                throw new ArgumentException("dotToken");
            }
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.QualifiedCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)container.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)dotToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberCrefSyntax)member.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedCrefSyntax QualifiedCref(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax container, Microsoft.CodeAnalysis.CSharp.Syntax.MemberCrefSyntax member)
        {
            return QualifiedCref(container, Token(SyntaxKind.DotToken), member);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameMemberCrefSyntax NameMemberCref(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax name, Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax? parameters)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NameMemberCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NameMemberCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)name.Green, (parameters == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterListSyntax)parameters!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NameMemberCrefSyntax NameMemberCref(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax name)
        {
            return NameMemberCref(name, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerMemberCrefSyntax IndexerMemberCref(SyntaxToken thisKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.CrefBracketedParameterListSyntax? parameters)
        {
            if (thisKeyword.Kind() != SyntaxKind.ThisKeyword)
            {
                throw new ArgumentException("thisKeyword");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IndexerMemberCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IndexerMemberCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)thisKeyword.Node, (parameters == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefBracketedParameterListSyntax)parameters!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IndexerMemberCrefSyntax IndexerMemberCref(Microsoft.CodeAnalysis.CSharp.Syntax.CrefBracketedParameterListSyntax? parameters = null)
        {
            return IndexerMemberCref(Token(SyntaxKind.ThisKeyword), parameters);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorKeyword, SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax? parameters)
        {
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword)
            {
                throw new ArgumentException("operatorKeyword");
            }
            switch (operatorToken.Kind())
            {
                default:
                    throw new ArgumentException("operatorToken");
                case SyntaxKind.TildeToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return (Microsoft.CodeAnalysis.CSharp.Syntax.OperatorMemberCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.OperatorMemberCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorToken.Node, (parameters == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterListSyntax)parameters!.Green)).CreateRed();
            }
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax? parameters)
        {
            return OperatorMemberCref(Token(SyntaxKind.OperatorKeyword), operatorToken, parameters);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorToken)
        {
            return OperatorMemberCref(Token(SyntaxKind.OperatorKeyword), operatorToken, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax? parameters)
        {
            SyntaxKind syntaxKind = implicitOrExplicitKeyword.Kind();
            if (syntaxKind - 8383 > SyntaxKind.List)
            {
                throw new ArgumentException("implicitOrExplicitKeyword");
            }
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword)
            {
                throw new ArgumentException("operatorKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ConversionOperatorMemberCref((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)implicitOrExplicitKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green, (parameters == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterListSyntax)parameters!.Green)).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax? parameters)
        {
            return ConversionOperatorMemberCref(implicitOrExplicitKeyword, Token(SyntaxKind.OperatorKeyword), type, parameters);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return ConversionOperatorMemberCref(implicitOrExplicitKeyword, Token(SyntaxKind.OperatorKeyword), type, null);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax CrefParameterList(SyntaxToken openParenToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken)
            {
                throw new ArgumentException("openParenToken");
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken)
            {
                throw new ArgumentException("closeParenToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CrefParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openParenToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax CrefParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax>))
        {
            return CrefParameterList(Token(SyntaxKind.OpenParenToken), parameters, Token(SyntaxKind.CloseParenToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefBracketedParameterListSyntax CrefBracketedParameterList(SyntaxToken openBracketToken, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken)
            {
                throw new ArgumentException("openBracketToken");
            }
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken)
            {
                throw new ArgumentException("closeBracketToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CrefBracketedParameterListSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CrefBracketedParameterList((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node, parameters.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefBracketedParameterListSyntax CrefBracketedParameterList(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax> parameters = default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax>))
        {
            return CrefBracketedParameterList(Token(SyntaxKind.OpenBracketToken), parameters, Token(SyntaxKind.CloseBracketToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax CrefParameter(SyntaxToken refKindKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            SyntaxKind syntaxKind = refKindKeyword.Kind();
            if (syntaxKind != 0 && syntaxKind - 8360 > (SyntaxKind)2)
            {
                throw new ArgumentException("refKindKeyword");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.CrefParameter((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)refKindKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax CrefParameter(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type)
        {
            return CrefParameter(default(SyntaxToken), type);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax startTag, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax> content, Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax endTag)
        {
            if (startTag == null)
            {
                throw new ArgumentNullException("startTag");
            }
            if (endTag == null)
            {
                throw new ArgumentNullException("endTag");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlElement((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlElementStartTagSyntax)startTag.Green, content.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNodeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlElementEndTagSyntax)endTag.Green).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax XmlElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax startTag, Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax endTag)
        {
            return XmlElement(startTag, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax>), endTag);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax XmlElementStartTag(SyntaxToken lessThanToken, Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax> attributes, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken)
            {
                throw new ArgumentException("lessThanToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken)
            {
                throw new ArgumentException("greaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlElementStartTag((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, attributes.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlAttributeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax XmlElementStartTag(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax> attributes)
        {
            return XmlElementStartTag(Token(SyntaxKind.LessThanToken), name, attributes, Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementStartTagSyntax XmlElementStartTag(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name)
        {
            return XmlElementStartTag(Token(SyntaxKind.LessThanToken), name, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax>), Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax XmlElementEndTag(SyntaxToken lessThanSlashToken, Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken greaterThanToken)
        {
            if (lessThanSlashToken.Kind() != SyntaxKind.LessThanSlashToken)
            {
                throw new ArgumentException("lessThanSlashToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken)
            {
                throw new ArgumentException("greaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlElementEndTag((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanSlashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementEndTagSyntax XmlElementEndTag(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name)
        {
            return XmlElementEndTag(Token(SyntaxKind.LessThanSlashToken), name, Token(SyntaxKind.GreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlEmptyElement(SyntaxToken lessThanToken, Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax> attributes, SyntaxToken slashGreaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken)
            {
                throw new ArgumentException("lessThanToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (slashGreaterThanToken.Kind() != SyntaxKind.SlashGreaterThanToken)
            {
                throw new ArgumentException("slashGreaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlEmptyElement((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, attributes.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlAttributeSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)slashGreaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlEmptyElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax> attributes)
        {
            return XmlEmptyElement(Token(SyntaxKind.LessThanToken), name, attributes, Token(SyntaxKind.SlashGreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax XmlEmptyElement(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name)
        {
            return XmlEmptyElement(Token(SyntaxKind.LessThanToken), name, default(SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax>), Token(SyntaxKind.SlashGreaterThanToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax XmlName(Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax? prefix, SyntaxToken localName)
        {
            if (localName.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("localName");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlName((prefix == null) ? null : ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlPrefixSyntax)prefix!.Green), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)localName.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax XmlName(SyntaxToken localName)
        {
            return XmlName(null, localName);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax XmlName(string localName)
        {
            return XmlName(null, Identifier(localName));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax XmlPrefix(SyntaxToken prefix, SyntaxToken colonToken)
        {
            if (prefix.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("prefix");
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken)
            {
                throw new ArgumentException("colonToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlPrefix((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)prefix.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)colonToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax XmlPrefix(SyntaxToken prefix)
        {
            return XmlPrefix(prefix, Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlPrefixSyntax XmlPrefix(string prefix)
        {
            return XmlPrefix(Identifier(prefix), Token(SyntaxKind.ColonToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            SyntaxKind syntaxKind = startQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("startQuoteToken");
            }
            syntaxKind = endQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("endQuoteToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlTextAttribute((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node, textTokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
        {
            return XmlTextAttribute(name, Token(SyntaxKind.EqualsToken), startQuoteToken, textTokens, endQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextAttributeSyntax XmlTextAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken startQuoteToken, SyntaxToken endQuoteToken)
        {
            return XmlTextAttribute(name, Token(SyntaxKind.EqualsToken), startQuoteToken, default(SyntaxTokenList), endQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax XmlCrefAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, SyntaxToken endQuoteToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            SyntaxKind syntaxKind = startQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("startQuoteToken");
            }
            if (cref == null)
            {
                throw new ArgumentNullException("cref");
            }
            syntaxKind = endQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("endQuoteToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlCrefAttribute((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefSyntax)cref.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCrefAttributeSyntax XmlCrefAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken startQuoteToken, Microsoft.CodeAnalysis.CSharp.Syntax.CrefSyntax cref, SyntaxToken endQuoteToken)
        {
            return XmlCrefAttribute(name, Token(SyntaxKind.EqualsToken), startQuoteToken, cref, endQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax XmlNameAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (equalsToken.Kind() != SyntaxKind.EqualsToken)
            {
                throw new ArgumentException("equalsToken");
            }
            SyntaxKind syntaxKind = startQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("startQuoteToken");
            }
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }
            syntaxKind = endQuoteToken.Kind();
            if (syntaxKind - 8213 > SyntaxKind.List)
            {
                throw new ArgumentException("endQuoteToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlNameAttribute((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)equalsToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IdentifierNameSyntax)identifier.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax XmlNameAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken startQuoteToken, Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            return XmlNameAttribute(name, Token(SyntaxKind.EqualsToken), startQuoteToken, identifier, endQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameAttributeSyntax XmlNameAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxToken startQuoteToken, string identifier, SyntaxToken endQuoteToken)
        {
            return XmlNameAttribute(name, Token(SyntaxKind.EqualsToken), startQuoteToken, IdentifierName(identifier), endQuoteToken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax XmlText(SyntaxTokenList textTokens)
        {
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlText(textTokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>()).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlTextSyntax XmlText()
        {
            return XmlText(default(SyntaxTokenList));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCDataSectionSyntax XmlCDataSection(SyntaxToken startCDataToken, SyntaxTokenList textTokens, SyntaxToken endCDataToken)
        {
            if (startCDataToken.Kind() != SyntaxKind.XmlCDataStartToken)
            {
                throw new ArgumentException("startCDataToken");
            }
            if (endCDataToken.Kind() != SyntaxKind.XmlCDataEndToken)
            {
                throw new ArgumentException("endCDataToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlCDataSectionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlCDataSection((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)startCDataToken.Node, textTokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endCDataToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCDataSectionSyntax XmlCDataSection(SyntaxTokenList textTokens = default(SyntaxTokenList))
        {
            return XmlCDataSection(Token(SyntaxKind.XmlCDataStartToken), textTokens, Token(SyntaxKind.XmlCDataEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlProcessingInstructionSyntax XmlProcessingInstruction(SyntaxToken startProcessingInstructionToken, Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxTokenList textTokens, SyntaxToken endProcessingInstructionToken)
        {
            if (startProcessingInstructionToken.Kind() != SyntaxKind.XmlProcessingInstructionStartToken)
            {
                throw new ArgumentException("startProcessingInstructionToken");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (endProcessingInstructionToken.Kind() != SyntaxKind.XmlProcessingInstructionEndToken)
            {
                throw new ArgumentException("endProcessingInstructionToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlProcessingInstructionSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlProcessingInstruction((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)startProcessingInstructionToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)name.Green, textTokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endProcessingInstructionToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlProcessingInstructionSyntax XmlProcessingInstruction(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name, SyntaxTokenList textTokens)
        {
            return XmlProcessingInstruction(Token(SyntaxKind.XmlProcessingInstructionStartToken), name, textTokens, Token(SyntaxKind.XmlProcessingInstructionEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlProcessingInstructionSyntax XmlProcessingInstruction(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name)
        {
            return XmlProcessingInstruction(Token(SyntaxKind.XmlProcessingInstructionStartToken), name, default(SyntaxTokenList), Token(SyntaxKind.XmlProcessingInstructionEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCommentSyntax XmlComment(SyntaxToken lessThanExclamationMinusMinusToken, SyntaxTokenList textTokens, SyntaxToken minusMinusGreaterThanToken)
        {
            if (lessThanExclamationMinusMinusToken.Kind() != SyntaxKind.XmlCommentStartToken)
            {
                throw new ArgumentException("lessThanExclamationMinusMinusToken");
            }
            if (minusMinusGreaterThanToken.Kind() != SyntaxKind.XmlCommentEndToken)
            {
                throw new ArgumentException("minusMinusGreaterThanToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.XmlCommentSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.XmlComment((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lessThanExclamationMinusMinusToken.Node, textTokens.Node.ToGreenList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)minusMinusGreaterThanToken.Node).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.XmlCommentSyntax XmlComment(SyntaxTokenList textTokens = default(SyntaxTokenList))
        {
            return XmlComment(Token(SyntaxKind.XmlCommentStartToken), textTokens, Token(SyntaxKind.XmlCommentEndToken));
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfDirectiveTriviaSyntax IfDirectiveTrivia(SyntaxToken hashToken, SyntaxToken ifKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (ifKeyword.Kind() != SyntaxKind.IfKeyword)
            {
                throw new ArgumentException("ifKeyword");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.IfDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.IfDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)ifKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive, branchTaken, conditionValue).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.IfDirectiveTriviaSyntax IfDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, bool isActive, bool branchTaken, bool conditionValue)
        {
            return IfDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.IfKeyword), condition, Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken, conditionValue);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElifDirectiveTriviaSyntax ElifDirectiveTrivia(SyntaxToken hashToken, SyntaxToken elifKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (elifKeyword.Kind() != SyntaxKind.ElifKeyword)
            {
                throw new ArgumentException("elifKeyword");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ElifDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElifDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)elifKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive, branchTaken, conditionValue).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElifDirectiveTriviaSyntax ElifDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition, bool isActive, bool branchTaken, bool conditionValue)
        {
            return ElifDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.ElifKeyword), condition, Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken, conditionValue);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElseDirectiveTriviaSyntax ElseDirectiveTrivia(SyntaxToken hashToken, SyntaxToken elseKeyword, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (elseKeyword.Kind() != SyntaxKind.ElseKeyword)
            {
                throw new ArgumentException("elseKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ElseDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ElseDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)elseKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive, branchTaken).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ElseDirectiveTriviaSyntax ElseDirectiveTrivia(bool isActive, bool branchTaken)
        {
            return ElseDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.ElseKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (endIfKeyword.Kind() != SyntaxKind.EndIfKeyword)
            {
                throw new ArgumentException("endIfKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EndIfDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EndIfDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endIfKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(bool isActive)
        {
            return EndIfDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.EndIfKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax RegionDirectiveTrivia(SyntaxToken hashToken, SyntaxToken regionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (regionKeyword.Kind() != SyntaxKind.RegionKeyword)
            {
                throw new ArgumentException("regionKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.RegionDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)regionKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax RegionDirectiveTrivia(bool isActive)
        {
            return RegionDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.RegionKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(SyntaxToken hashToken, SyntaxToken endRegionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (endRegionKeyword.Kind() != SyntaxKind.EndRegionKeyword)
            {
                throw new ArgumentException("endRegionKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.EndRegionDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endRegionKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(bool isActive)
        {
            return EndRegionDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.EndRegionKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ErrorDirectiveTriviaSyntax ErrorDirectiveTrivia(SyntaxToken hashToken, SyntaxToken errorKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (errorKeyword.Kind() != SyntaxKind.ErrorKeyword)
            {
                throw new ArgumentException("errorKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ErrorDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ErrorDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)errorKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ErrorDirectiveTriviaSyntax ErrorDirectiveTrivia(bool isActive)
        {
            return ErrorDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.ErrorKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WarningDirectiveTriviaSyntax WarningDirectiveTrivia(SyntaxToken hashToken, SyntaxToken warningKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (warningKeyword.Kind() != SyntaxKind.WarningKeyword)
            {
                throw new ArgumentException("warningKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.WarningDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.WarningDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)warningKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.WarningDirectiveTriviaSyntax WarningDirectiveTrivia(bool isActive)
        {
            return WarningDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.WarningKeyword), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BadDirectiveTriviaSyntax BadDirectiveTrivia(SyntaxToken hashToken, SyntaxToken identifier, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.BadDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.BadDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)identifier.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.BadDirectiveTriviaSyntax BadDirectiveTrivia(SyntaxToken identifier, bool isActive)
        {
            return BadDirectiveTrivia(Token(SyntaxKind.HashToken), identifier, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefineDirectiveTriviaSyntax DefineDirectiveTrivia(SyntaxToken hashToken, SyntaxToken defineKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (defineKeyword.Kind() != SyntaxKind.DefineKeyword)
            {
                throw new ArgumentException("defineKeyword");
            }
            if (name.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("name");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.DefineDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.DefineDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)defineKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)name.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefineDirectiveTriviaSyntax DefineDirectiveTrivia(SyntaxToken name, bool isActive)
        {
            return DefineDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.DefineKeyword), name, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.DefineDirectiveTriviaSyntax DefineDirectiveTrivia(string name, bool isActive)
        {
            return DefineDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.DefineKeyword), Identifier(name), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UndefDirectiveTriviaSyntax UndefDirectiveTrivia(SyntaxToken hashToken, SyntaxToken undefKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (undefKeyword.Kind() != SyntaxKind.UndefKeyword)
            {
                throw new ArgumentException("undefKeyword");
            }
            if (name.Kind() != SyntaxKind.IdentifierToken)
            {
                throw new ArgumentException("name");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.UndefDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.UndefDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)undefKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)name.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UndefDirectiveTriviaSyntax UndefDirectiveTrivia(SyntaxToken name, bool isActive)
        {
            return UndefDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.UndefKeyword), name, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.UndefDirectiveTriviaSyntax UndefDirectiveTrivia(string name, bool isActive)
        {
            return UndefDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.UndefKeyword), Identifier(name), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (lineKeyword.Kind() != SyntaxKind.LineKeyword)
            {
                throw new ArgumentException("lineKeyword");
            }
            SyntaxKind syntaxKind = line.Kind();
            if (syntaxKind != SyntaxKind.DefaultKeyword && syntaxKind != SyntaxKind.HiddenKeyword && syntaxKind != SyntaxKind.NumericLiteralToken)
            {
                throw new ArgumentException("line");
            }
            syntaxKind = file.Kind();
            if (syntaxKind != 0 && syntaxKind != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("file");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LineDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)lineKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)line.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)file.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken line, SyntaxToken file, bool isActive)
        {
            return LineDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.LineKeyword), line, file, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken line, bool isActive)
        {
            return LineDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.LineKeyword), line, default(SyntaxToken), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> errorCodes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (pragmaKeyword.Kind() != SyntaxKind.PragmaKeyword)
            {
                throw new ArgumentException("pragmaKeyword");
            }
            if (warningKeyword.Kind() != SyntaxKind.WarningKeyword)
            {
                throw new ArgumentException("warningKeyword");
            }
            SyntaxKind syntaxKind = disableOrRestoreKeyword.Kind();
            if (syntaxKind - 8479 > SyntaxKind.List)
            {
                throw new ArgumentException("disableOrRestoreKeyword");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PragmaWarningDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PragmaWarningDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)pragmaKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)warningKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)disableOrRestoreKeyword.Node, errorCodes.Node.ToGreenSeparatedList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionSyntax>(), (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken disableOrRestoreKeyword, SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> errorCodes, bool isActive)
        {
            return PragmaWarningDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.PragmaKeyword), Token(SyntaxKind.WarningKeyword), disableOrRestoreKeyword, errorCodes, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken disableOrRestoreKeyword, bool isActive)
        {
            return PragmaWarningDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.PragmaKeyword), Token(SyntaxKind.WarningKeyword), disableOrRestoreKeyword, default(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PragmaChecksumDirectiveTriviaSyntax PragmaChecksumDirectiveTrivia(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (pragmaKeyword.Kind() != SyntaxKind.PragmaKeyword)
            {
                throw new ArgumentException("pragmaKeyword");
            }
            if (checksumKeyword.Kind() != SyntaxKind.ChecksumKeyword)
            {
                throw new ArgumentException("checksumKeyword");
            }
            if (file.Kind() != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("file");
            }
            if (guid.Kind() != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("guid");
            }
            if (bytes.Kind() != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("bytes");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.PragmaChecksumDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.PragmaChecksumDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)pragmaKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)checksumKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)file.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)guid.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)bytes.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.PragmaChecksumDirectiveTriviaSyntax PragmaChecksumDirectiveTrivia(SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, bool isActive)
        {
            return PragmaChecksumDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.PragmaKeyword), Token(SyntaxKind.ChecksumKeyword), file, guid, bytes, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(SyntaxToken hashToken, SyntaxToken referenceKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (referenceKeyword.Kind() != SyntaxKind.ReferenceKeyword)
            {
                throw new ArgumentException("referenceKeyword");
            }
            if (file.Kind() != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("file");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ReferenceDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)referenceKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)file.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(SyntaxToken file, bool isActive)
        {
            return ReferenceDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.ReferenceKeyword), file, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LoadDirectiveTriviaSyntax LoadDirectiveTrivia(SyntaxToken hashToken, SyntaxToken loadKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (loadKeyword.Kind() != SyntaxKind.LoadKeyword)
            {
                throw new ArgumentException("loadKeyword");
            }
            if (file.Kind() != SyntaxKind.StringLiteralToken)
            {
                throw new ArgumentException("file");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.LoadDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.LoadDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)loadKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)file.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.LoadDirectiveTriviaSyntax LoadDirectiveTrivia(SyntaxToken file, bool isActive)
        {
            return LoadDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.LoadKeyword), file, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ShebangDirectiveTriviaSyntax ShebangDirectiveTrivia(SyntaxToken hashToken, SyntaxToken exclamationToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (exclamationToken.Kind() != SyntaxKind.ExclamationToken)
            {
                throw new ArgumentException("exclamationToken");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.ShebangDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.ShebangDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)exclamationToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.ShebangDirectiveTriviaSyntax ShebangDirectiveTrivia(bool isActive)
        {
            return ShebangDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.ExclamationToken), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken)
            {
                throw new ArgumentException("hashToken");
            }
            if (nullableKeyword.Kind() != SyntaxKind.NullableKeyword)
            {
                throw new ArgumentException("nullableKeyword");
            }
            SyntaxKind syntaxKind = settingToken.Kind();
            if (syntaxKind - 8479 > SyntaxKind.List && syntaxKind != SyntaxKind.EnableKeyword)
            {
                throw new ArgumentException("settingToken");
            }
            syntaxKind = targetToken.Kind();
            if (syntaxKind != 0 && syntaxKind - 8488 > SyntaxKind.List)
            {
                throw new ArgumentException("targetToken");
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken)
            {
                throw new ArgumentException("endOfDirectiveToken");
            }
            return (Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax)Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxFactory.NullableDirectiveTrivia((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)hashToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)nullableKeyword.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)settingToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)targetToken.Node, (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node, isActive).CreateRed();
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken settingToken, SyntaxToken targetToken, bool isActive)
        {
            return NullableDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.NullableKeyword), settingToken, targetToken, Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }

        public static Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken settingToken, bool isActive)
        {
            return NullableDirectiveTrivia(Token(SyntaxKind.HashToken), Token(SyntaxKind.NullableKeyword), settingToken, default(SyntaxToken), Token(SyntaxKind.EndOfDirectiveToken), isActive);
        }
    }
}
