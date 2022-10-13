#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public partial class CSharpSyntaxVisitor<TResult>
    {
        /// <summary>Called when the visitor visits a IdentifierNameSyntax node.</summary>
        public virtual TResult? VisitIdentifierName(IdentifierNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QualifiedNameSyntax node.</summary>
        public virtual TResult? VisitQualifiedName(QualifiedNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GenericNameSyntax node.</summary>
        public virtual TResult? VisitGenericName(GenericNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeArgumentListSyntax node.</summary>
        public virtual TResult? VisitTypeArgumentList(TypeArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AliasQualifiedNameSyntax node.</summary>
        public virtual TResult? VisitAliasQualifiedName(AliasQualifiedNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PredefinedTypeSyntax node.</summary>
        public virtual TResult? VisitPredefinedType(PredefinedTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayTypeSyntax node.</summary>
        public virtual TResult? VisitArrayType(ArrayTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayRankSpecifierSyntax node.</summary>
        public virtual TResult? VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PointerTypeSyntax node.</summary>
        public virtual TResult? VisitPointerType(PointerTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerTypeSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerType(FunctionPointerTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerParameterListSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerCallingConventionSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerUnmanagedCallingConventionListSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerUnmanagedCallingConventionSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NullableTypeSyntax node.</summary>
        public virtual TResult? VisitNullableType(NullableTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleTypeSyntax node.</summary>
        public virtual TResult? VisitTupleType(TupleTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleElementSyntax node.</summary>
        public virtual TResult? VisitTupleElement(TupleElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OmittedTypeArgumentSyntax node.</summary>
        public virtual TResult? VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefTypeSyntax node.</summary>
        public virtual TResult? VisitRefType(RefTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedExpressionSyntax node.</summary>
        public virtual TResult? VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleExpressionSyntax node.</summary>
        public virtual TResult? VisitTupleExpression(TupleExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PrefixUnaryExpressionSyntax node.</summary>
        public virtual TResult? VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AwaitExpressionSyntax node.</summary>
        public virtual TResult? VisitAwaitExpression(AwaitExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PostfixUnaryExpressionSyntax node.</summary>
        public virtual TResult? VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MemberAccessExpressionSyntax node.</summary>
        public virtual TResult? VisitMemberAccessExpression(MemberAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConditionalAccessExpressionSyntax node.</summary>
        public virtual TResult? VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MemberBindingExpressionSyntax node.</summary>
        public virtual TResult? VisitMemberBindingExpression(MemberBindingExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElementBindingExpressionSyntax node.</summary>
        public virtual TResult? VisitElementBindingExpression(ElementBindingExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RangeExpressionSyntax node.</summary>
        public virtual TResult? VisitRangeExpression(RangeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitElementAccessSyntax node.</summary>
        public virtual TResult? VisitImplicitElementAccess(ImplicitElementAccessSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BinaryExpressionSyntax node.</summary>
        public virtual TResult? VisitBinaryExpression(BinaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AssignmentExpressionSyntax node.</summary>
        public virtual TResult? VisitAssignmentExpression(AssignmentExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConditionalExpressionSyntax node.</summary>
        public virtual TResult? VisitConditionalExpression(ConditionalExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThisExpressionSyntax node.</summary>
        public virtual TResult? VisitThisExpression(ThisExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BaseExpressionSyntax node.</summary>
        public virtual TResult? VisitBaseExpression(BaseExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LiteralExpressionSyntax node.</summary>
        public virtual TResult? VisitLiteralExpression(LiteralExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MakeRefExpressionSyntax node.</summary>
        public virtual TResult? VisitMakeRefExpression(MakeRefExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefTypeExpressionSyntax node.</summary>
        public virtual TResult? VisitRefTypeExpression(RefTypeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefValueExpressionSyntax node.</summary>
        public virtual TResult? VisitRefValueExpression(RefValueExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CheckedExpressionSyntax node.</summary>
        public virtual TResult? VisitCheckedExpression(CheckedExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultExpressionSyntax node.</summary>
        public virtual TResult? VisitDefaultExpression(DefaultExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeOfExpressionSyntax node.</summary>
        public virtual TResult? VisitTypeOfExpression(TypeOfExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SizeOfExpressionSyntax node.</summary>
        public virtual TResult? VisitSizeOfExpression(SizeOfExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InvocationExpressionSyntax node.</summary>
        public virtual TResult? VisitInvocationExpression(InvocationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElementAccessExpressionSyntax node.</summary>
        public virtual TResult? VisitElementAccessExpression(ElementAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArgumentListSyntax node.</summary>
        public virtual TResult? VisitArgumentList(ArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BracketedArgumentListSyntax node.</summary>
        public virtual TResult? VisitBracketedArgumentList(BracketedArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArgumentSyntax node.</summary>
        public virtual TResult? VisitArgument(ArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameColonSyntax node.</summary>
        public virtual TResult? VisitNameColon(NameColonSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DeclarationExpressionSyntax node.</summary>
        public virtual TResult? VisitDeclarationExpression(DeclarationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CastExpressionSyntax node.</summary>
        public virtual TResult? VisitCastExpression(CastExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousMethodExpressionSyntax node.</summary>
        public virtual TResult? VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SimpleLambdaExpressionSyntax node.</summary>
        public virtual TResult? VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefExpressionSyntax node.</summary>
        public virtual TResult? VisitRefExpression(RefExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedLambdaExpressionSyntax node.</summary>
        public virtual TResult? VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InitializerExpressionSyntax node.</summary>
        public virtual TResult? VisitInitializerExpression(InitializerExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitObjectCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ObjectCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WithExpressionSyntax node.</summary>
        public virtual TResult? VisitWithExpression(WithExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousObjectMemberDeclaratorSyntax node.</summary>
        public virtual TResult? VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousObjectCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitArrayCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a StackAllocArrayCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitStackAllocArrayCreationExpressionSyntax node.</summary>
        public virtual TResult? VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryExpressionSyntax node.</summary>
        public virtual TResult? VisitQueryExpression(QueryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryBodySyntax node.</summary>
        public virtual TResult? VisitQueryBody(QueryBodySyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FromClauseSyntax node.</summary>
        public virtual TResult? VisitFromClause(FromClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LetClauseSyntax node.</summary>
        public virtual TResult? VisitLetClause(LetClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a JoinClauseSyntax node.</summary>
        public virtual TResult? VisitJoinClause(JoinClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a JoinIntoClauseSyntax node.</summary>
        public virtual TResult? VisitJoinIntoClause(JoinIntoClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhereClauseSyntax node.</summary>
        public virtual TResult? VisitWhereClause(WhereClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OrderByClauseSyntax node.</summary>
        public virtual TResult? VisitOrderByClause(OrderByClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OrderingSyntax node.</summary>
        public virtual TResult? VisitOrdering(OrderingSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SelectClauseSyntax node.</summary>
        public virtual TResult? VisitSelectClause(SelectClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GroupClauseSyntax node.</summary>
        public virtual TResult? VisitGroupClause(GroupClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryContinuationSyntax node.</summary>
        public virtual TResult? VisitQueryContinuation(QueryContinuationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OmittedArraySizeExpressionSyntax node.</summary>
        public virtual TResult? VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolatedStringExpressionSyntax node.</summary>
        public virtual TResult? VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IsPatternExpressionSyntax node.</summary>
        public virtual TResult? VisitIsPatternExpression(IsPatternExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThrowExpressionSyntax node.</summary>
        public virtual TResult? VisitThrowExpression(ThrowExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhenClauseSyntax node.</summary>
        public virtual TResult? VisitWhenClause(WhenClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DiscardPatternSyntax node.</summary>
        public virtual TResult? VisitDiscardPattern(DiscardPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DeclarationPatternSyntax node.</summary>
        public virtual TResult? VisitDeclarationPattern(DeclarationPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VarPatternSyntax node.</summary>
        public virtual TResult? VisitVarPattern(VarPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RecursivePatternSyntax node.</summary>
        public virtual TResult? VisitRecursivePattern(RecursivePatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PositionalPatternClauseSyntax node.</summary>
        public virtual TResult? VisitPositionalPatternClause(PositionalPatternClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PropertyPatternClauseSyntax node.</summary>
        public virtual TResult? VisitPropertyPatternClause(PropertyPatternClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SubpatternSyntax node.</summary>
        public virtual TResult? VisitSubpattern(SubpatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstantPatternSyntax node.</summary>
        public virtual TResult? VisitConstantPattern(ConstantPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedPatternSyntax node.</summary>
        public virtual TResult? VisitParenthesizedPattern(ParenthesizedPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RelationalPatternSyntax node.</summary>
        public virtual TResult? VisitRelationalPattern(RelationalPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypePatternSyntax node.</summary>
        public virtual TResult? VisitTypePattern(TypePatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BinaryPatternSyntax node.</summary>
        public virtual TResult? VisitBinaryPattern(BinaryPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UnaryPatternSyntax node.</summary>
        public virtual TResult? VisitUnaryPattern(UnaryPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolatedStringTextSyntax node.</summary>
        public virtual TResult? VisitInterpolatedStringText(InterpolatedStringTextSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationSyntax node.</summary>
        public virtual TResult? VisitInterpolation(InterpolationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationAlignmentClauseSyntax node.</summary>
        public virtual TResult? VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationFormatClauseSyntax node.</summary>
        public virtual TResult? VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GlobalStatementSyntax node.</summary>
        public virtual TResult? VisitGlobalStatement(GlobalStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BlockSyntax node.</summary>
        public virtual TResult? VisitBlock(BlockSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LocalFunctionStatementSyntax node.</summary>
        public virtual TResult? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LocalDeclarationStatementSyntax node.</summary>
        public virtual TResult? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VariableDeclarationSyntax node.</summary>
        public virtual TResult? VisitVariableDeclaration(VariableDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VariableDeclaratorSyntax node.</summary>
        public virtual TResult? VisitVariableDeclarator(VariableDeclaratorSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EqualsValueClauseSyntax node.</summary>
        public virtual TResult? VisitEqualsValueClause(EqualsValueClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SingleVariableDesignationSyntax node.</summary>
        public virtual TResult? VisitSingleVariableDesignation(SingleVariableDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DiscardDesignationSyntax node.</summary>
        public virtual TResult? VisitDiscardDesignation(DiscardDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedVariableDesignationSyntax node.</summary>
        public virtual TResult? VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExpressionStatementSyntax node.</summary>
        public virtual TResult? VisitExpressionStatement(ExpressionStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EmptyStatementSyntax node.</summary>
        public virtual TResult? VisitEmptyStatement(EmptyStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LabeledStatementSyntax node.</summary>
        public virtual TResult? VisitLabeledStatement(LabeledStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GotoStatementSyntax node.</summary>
        public virtual TResult? VisitGotoStatement(GotoStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BreakStatementSyntax node.</summary>
        public virtual TResult? VisitBreakStatement(BreakStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ContinueStatementSyntax node.</summary>
        public virtual TResult? VisitContinueStatement(ContinueStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ReturnStatementSyntax node.</summary>
        public virtual TResult? VisitReturnStatement(ReturnStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThrowStatementSyntax node.</summary>
        public virtual TResult? VisitThrowStatement(ThrowStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a YieldStatementSyntax node.</summary>
        public virtual TResult? VisitYieldStatement(YieldStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhileStatementSyntax node.</summary>
        public virtual TResult? VisitWhileStatement(WhileStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DoStatementSyntax node.</summary>
        public virtual TResult? VisitDoStatement(DoStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForStatementSyntax node.</summary>
        public virtual TResult? VisitForStatement(ForStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForEachStatementSyntax node.</summary>
        public virtual TResult? VisitForEachStatement(ForEachStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForEachVariableStatementSyntax node.</summary>
        public virtual TResult? VisitForEachVariableStatement(ForEachVariableStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UsingStatementSyntax node.</summary>
        public virtual TResult? VisitUsingStatement(UsingStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FixedStatementSyntax node.</summary>
        public virtual TResult? VisitFixedStatement(FixedStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CheckedStatementSyntax node.</summary>
        public virtual TResult? VisitCheckedStatement(CheckedStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UnsafeStatementSyntax node.</summary>
        public virtual TResult? VisitUnsafeStatement(UnsafeStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LockStatementSyntax node.</summary>
        public virtual TResult? VisitLockStatement(LockStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IfStatementSyntax node.</summary>
        public virtual TResult? VisitIfStatement(IfStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElseClauseSyntax node.</summary>
        public virtual TResult? VisitElseClause(ElseClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchStatementSyntax node.</summary>
        public virtual TResult? VisitSwitchStatement(SwitchStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchSectionSyntax node.</summary>
        public virtual TResult? VisitSwitchSection(SwitchSectionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CasePatternSwitchLabelSyntax node.</summary>
        public virtual TResult? VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CaseSwitchLabelSyntax node.</summary>
        public virtual TResult? VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultSwitchLabelSyntax node.</summary>
        public virtual TResult? VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchExpressionSyntax node.</summary>
        public virtual TResult? VisitSwitchExpression(SwitchExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchExpressionArmSyntax node.</summary>
        public virtual TResult? VisitSwitchExpressionArm(SwitchExpressionArmSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TryStatementSyntax node.</summary>
        public virtual TResult? VisitTryStatement(TryStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchClauseSyntax node.</summary>
        public virtual TResult? VisitCatchClause(CatchClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchDeclarationSyntax node.</summary>
        public virtual TResult? VisitCatchDeclaration(CatchDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchFilterClauseSyntax node.</summary>
        public virtual TResult? VisitCatchFilterClause(CatchFilterClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FinallyClauseSyntax node.</summary>
        public virtual TResult? VisitFinallyClause(FinallyClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CompilationUnitSyntax node.</summary>
        public virtual TResult? VisitCompilationUnit(CompilationUnitSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExternAliasDirectiveSyntax node.</summary>
        public virtual TResult? VisitExternAliasDirective(ExternAliasDirectiveSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UsingDirectiveSyntax node.</summary>
        public virtual TResult? VisitUsingDirective(UsingDirectiveSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NamespaceDeclarationSyntax node.</summary>
        public virtual TResult? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeListSyntax node.</summary>
        public virtual TResult? VisitAttributeList(AttributeListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeTargetSpecifierSyntax node.</summary>
        public virtual TResult? VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeSyntax node.</summary>
        public virtual TResult? VisitAttribute(AttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeArgumentListSyntax node.</summary>
        public virtual TResult? VisitAttributeArgumentList(AttributeArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeArgumentSyntax node.</summary>
        public virtual TResult? VisitAttributeArgument(AttributeArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameEqualsSyntax node.</summary>
        public virtual TResult? VisitNameEquals(NameEqualsSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterListSyntax node.</summary>
        public virtual TResult? VisitTypeParameterList(TypeParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterSyntax node.</summary>
        public virtual TResult? VisitTypeParameter(TypeParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ClassDeclarationSyntax node.</summary>
        public virtual TResult? VisitClassDeclaration(ClassDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a StructDeclarationSyntax node.</summary>
        public virtual TResult? VisitStructDeclaration(StructDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterfaceDeclarationSyntax node.</summary>
        public virtual TResult? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RecordDeclarationSyntax node.</summary>
        public virtual TResult? VisitRecordDeclaration(RecordDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EnumDeclarationSyntax node.</summary>
        public virtual TResult? VisitEnumDeclaration(EnumDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DelegateDeclarationSyntax node.</summary>
        public virtual TResult? VisitDelegateDeclaration(DelegateDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EnumMemberDeclarationSyntax node.</summary>
        public virtual TResult? VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BaseListSyntax node.</summary>
        public virtual TResult? VisitBaseList(BaseListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SimpleBaseTypeSyntax node.</summary>
        public virtual TResult? VisitSimpleBaseType(SimpleBaseTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PrimaryConstructorBaseTypeSyntax node.</summary>
        public virtual TResult? VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterConstraintClauseSyntax node.</summary>
        public virtual TResult? VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorConstraintSyntax node.</summary>
        public virtual TResult? VisitConstructorConstraint(ConstructorConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ClassOrStructConstraintSyntax node.</summary>
        public virtual TResult? VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeConstraintSyntax node.</summary>
        public virtual TResult? VisitTypeConstraint(TypeConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultConstraintSyntax node.</summary>
        public virtual TResult? VisitDefaultConstraint(DefaultConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FieldDeclarationSyntax node.</summary>
        public virtual TResult? VisitFieldDeclaration(FieldDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EventFieldDeclarationSyntax node.</summary>
        public virtual TResult? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExplicitInterfaceSpecifierSyntax node.</summary>
        public virtual TResult? VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MethodDeclarationSyntax node.</summary>
        public virtual TResult? VisitMethodDeclaration(MethodDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OperatorDeclarationSyntax node.</summary>
        public virtual TResult? VisitOperatorDeclaration(OperatorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConversionOperatorDeclarationSyntax node.</summary>
        public virtual TResult? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorDeclarationSyntax node.</summary>
        public virtual TResult? VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorInitializerSyntax node.</summary>
        public virtual TResult? VisitConstructorInitializer(ConstructorInitializerSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DestructorDeclarationSyntax node.</summary>
        public virtual TResult? VisitDestructorDeclaration(DestructorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PropertyDeclarationSyntax node.</summary>
        public virtual TResult? VisitPropertyDeclaration(PropertyDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrowExpressionClauseSyntax node.</summary>
        public virtual TResult? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EventDeclarationSyntax node.</summary>
        public virtual TResult? VisitEventDeclaration(EventDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IndexerDeclarationSyntax node.</summary>
        public virtual TResult? VisitIndexerDeclaration(IndexerDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AccessorListSyntax node.</summary>
        public virtual TResult? VisitAccessorList(AccessorListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AccessorDeclarationSyntax node.</summary>
        public virtual TResult? VisitAccessorDeclaration(AccessorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParameterListSyntax node.</summary>
        public virtual TResult? VisitParameterList(ParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BracketedParameterListSyntax node.</summary>
        public virtual TResult? VisitBracketedParameterList(BracketedParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParameterSyntax node.</summary>
        public virtual TResult? VisitParameter(ParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerParameterSyntax node.</summary>
        public virtual TResult? VisitFunctionPointerParameter(FunctionPointerParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IncompleteMemberSyntax node.</summary>
        public virtual TResult? VisitIncompleteMember(IncompleteMemberSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SkippedTokensTriviaSyntax node.</summary>
        public virtual TResult? VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DocumentationCommentTriviaSyntax node.</summary>
        public virtual TResult? VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeCrefSyntax node.</summary>
        public virtual TResult? VisitTypeCref(TypeCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QualifiedCrefSyntax node.</summary>
        public virtual TResult? VisitQualifiedCref(QualifiedCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameMemberCrefSyntax node.</summary>
        public virtual TResult? VisitNameMemberCref(NameMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IndexerMemberCrefSyntax node.</summary>
        public virtual TResult? VisitIndexerMemberCref(IndexerMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OperatorMemberCrefSyntax node.</summary>
        public virtual TResult? VisitOperatorMemberCref(OperatorMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConversionOperatorMemberCrefSyntax node.</summary>
        public virtual TResult? VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefParameterListSyntax node.</summary>
        public virtual TResult? VisitCrefParameterList(CrefParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefBracketedParameterListSyntax node.</summary>
        public virtual TResult? VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefParameterSyntax node.</summary>
        public virtual TResult? VisitCrefParameter(CrefParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementSyntax node.</summary>
        public virtual TResult? VisitXmlElement(XmlElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementStartTagSyntax node.</summary>
        public virtual TResult? VisitXmlElementStartTag(XmlElementStartTagSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementEndTagSyntax node.</summary>
        public virtual TResult? VisitXmlElementEndTag(XmlElementEndTagSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlEmptyElementSyntax node.</summary>
        public virtual TResult? VisitXmlEmptyElement(XmlEmptyElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlNameSyntax node.</summary>
        public virtual TResult? VisitXmlName(XmlNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlPrefixSyntax node.</summary>
        public virtual TResult? VisitXmlPrefix(XmlPrefixSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlTextAttributeSyntax node.</summary>
        public virtual TResult? VisitXmlTextAttribute(XmlTextAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCrefAttributeSyntax node.</summary>
        public virtual TResult? VisitXmlCrefAttribute(XmlCrefAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlNameAttributeSyntax node.</summary>
        public virtual TResult? VisitXmlNameAttribute(XmlNameAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlTextSyntax node.</summary>
        public virtual TResult? VisitXmlText(XmlTextSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCDataSectionSyntax node.</summary>
        public virtual TResult? VisitXmlCDataSection(XmlCDataSectionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlProcessingInstructionSyntax node.</summary>
        public virtual TResult? VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCommentSyntax node.</summary>
        public virtual TResult? VisitXmlComment(XmlCommentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IfDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElifDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElseDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EndIfDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RegionDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EndRegionDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ErrorDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WarningDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BadDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefineDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UndefDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LineDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PragmaWarningDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PragmaChecksumDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ReferenceDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LoadDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ShebangDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NullableDirectiveTriviaSyntax node.</summary>
        public virtual TResult? VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node) => this.DefaultVisit(node);
    }

    public partial class CSharpSyntaxVisitor
    {
        /// <summary>Called when the visitor visits a IdentifierNameSyntax node.</summary>
        public virtual void VisitIdentifierName(IdentifierNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QualifiedNameSyntax node.</summary>
        public virtual void VisitQualifiedName(QualifiedNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GenericNameSyntax node.</summary>
        public virtual void VisitGenericName(GenericNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeArgumentListSyntax node.</summary>
        public virtual void VisitTypeArgumentList(TypeArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AliasQualifiedNameSyntax node.</summary>
        public virtual void VisitAliasQualifiedName(AliasQualifiedNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PredefinedTypeSyntax node.</summary>
        public virtual void VisitPredefinedType(PredefinedTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayTypeSyntax node.</summary>
        public virtual void VisitArrayType(ArrayTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayRankSpecifierSyntax node.</summary>
        public virtual void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PointerTypeSyntax node.</summary>
        public virtual void VisitPointerType(PointerTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerTypeSyntax node.</summary>
        public virtual void VisitFunctionPointerType(FunctionPointerTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerParameterListSyntax node.</summary>
        public virtual void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerCallingConventionSyntax node.</summary>
        public virtual void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerUnmanagedCallingConventionListSyntax node.</summary>
        public virtual void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerUnmanagedCallingConventionSyntax node.</summary>
        public virtual void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NullableTypeSyntax node.</summary>
        public virtual void VisitNullableType(NullableTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleTypeSyntax node.</summary>
        public virtual void VisitTupleType(TupleTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleElementSyntax node.</summary>
        public virtual void VisitTupleElement(TupleElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OmittedTypeArgumentSyntax node.</summary>
        public virtual void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefTypeSyntax node.</summary>
        public virtual void VisitRefType(RefTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedExpressionSyntax node.</summary>
        public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TupleExpressionSyntax node.</summary>
        public virtual void VisitTupleExpression(TupleExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PrefixUnaryExpressionSyntax node.</summary>
        public virtual void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AwaitExpressionSyntax node.</summary>
        public virtual void VisitAwaitExpression(AwaitExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PostfixUnaryExpressionSyntax node.</summary>
        public virtual void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MemberAccessExpressionSyntax node.</summary>
        public virtual void VisitMemberAccessExpression(MemberAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConditionalAccessExpressionSyntax node.</summary>
        public virtual void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MemberBindingExpressionSyntax node.</summary>
        public virtual void VisitMemberBindingExpression(MemberBindingExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElementBindingExpressionSyntax node.</summary>
        public virtual void VisitElementBindingExpression(ElementBindingExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RangeExpressionSyntax node.</summary>
        public virtual void VisitRangeExpression(RangeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitElementAccessSyntax node.</summary>
        public virtual void VisitImplicitElementAccess(ImplicitElementAccessSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BinaryExpressionSyntax node.</summary>
        public virtual void VisitBinaryExpression(BinaryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AssignmentExpressionSyntax node.</summary>
        public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConditionalExpressionSyntax node.</summary>
        public virtual void VisitConditionalExpression(ConditionalExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThisExpressionSyntax node.</summary>
        public virtual void VisitThisExpression(ThisExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BaseExpressionSyntax node.</summary>
        public virtual void VisitBaseExpression(BaseExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LiteralExpressionSyntax node.</summary>
        public virtual void VisitLiteralExpression(LiteralExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MakeRefExpressionSyntax node.</summary>
        public virtual void VisitMakeRefExpression(MakeRefExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefTypeExpressionSyntax node.</summary>
        public virtual void VisitRefTypeExpression(RefTypeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefValueExpressionSyntax node.</summary>
        public virtual void VisitRefValueExpression(RefValueExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CheckedExpressionSyntax node.</summary>
        public virtual void VisitCheckedExpression(CheckedExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultExpressionSyntax node.</summary>
        public virtual void VisitDefaultExpression(DefaultExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeOfExpressionSyntax node.</summary>
        public virtual void VisitTypeOfExpression(TypeOfExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SizeOfExpressionSyntax node.</summary>
        public virtual void VisitSizeOfExpression(SizeOfExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InvocationExpressionSyntax node.</summary>
        public virtual void VisitInvocationExpression(InvocationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElementAccessExpressionSyntax node.</summary>
        public virtual void VisitElementAccessExpression(ElementAccessExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArgumentListSyntax node.</summary>
        public virtual void VisitArgumentList(ArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BracketedArgumentListSyntax node.</summary>
        public virtual void VisitBracketedArgumentList(BracketedArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArgumentSyntax node.</summary>
        public virtual void VisitArgument(ArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameColonSyntax node.</summary>
        public virtual void VisitNameColon(NameColonSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DeclarationExpressionSyntax node.</summary>
        public virtual void VisitDeclarationExpression(DeclarationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CastExpressionSyntax node.</summary>
        public virtual void VisitCastExpression(CastExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousMethodExpressionSyntax node.</summary>
        public virtual void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SimpleLambdaExpressionSyntax node.</summary>
        public virtual void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RefExpressionSyntax node.</summary>
        public virtual void VisitRefExpression(RefExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedLambdaExpressionSyntax node.</summary>
        public virtual void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InitializerExpressionSyntax node.</summary>
        public virtual void VisitInitializerExpression(InitializerExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitObjectCreationExpressionSyntax node.</summary>
        public virtual void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ObjectCreationExpressionSyntax node.</summary>
        public virtual void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WithExpressionSyntax node.</summary>
        public virtual void VisitWithExpression(WithExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousObjectMemberDeclaratorSyntax node.</summary>
        public virtual void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AnonymousObjectCreationExpressionSyntax node.</summary>
        public virtual void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrayCreationExpressionSyntax node.</summary>
        public virtual void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitArrayCreationExpressionSyntax node.</summary>
        public virtual void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a StackAllocArrayCreationExpressionSyntax node.</summary>
        public virtual void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ImplicitStackAllocArrayCreationExpressionSyntax node.</summary>
        public virtual void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryExpressionSyntax node.</summary>
        public virtual void VisitQueryExpression(QueryExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryBodySyntax node.</summary>
        public virtual void VisitQueryBody(QueryBodySyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FromClauseSyntax node.</summary>
        public virtual void VisitFromClause(FromClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LetClauseSyntax node.</summary>
        public virtual void VisitLetClause(LetClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a JoinClauseSyntax node.</summary>
        public virtual void VisitJoinClause(JoinClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a JoinIntoClauseSyntax node.</summary>
        public virtual void VisitJoinIntoClause(JoinIntoClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhereClauseSyntax node.</summary>
        public virtual void VisitWhereClause(WhereClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OrderByClauseSyntax node.</summary>
        public virtual void VisitOrderByClause(OrderByClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OrderingSyntax node.</summary>
        public virtual void VisitOrdering(OrderingSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SelectClauseSyntax node.</summary>
        public virtual void VisitSelectClause(SelectClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GroupClauseSyntax node.</summary>
        public virtual void VisitGroupClause(GroupClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QueryContinuationSyntax node.</summary>
        public virtual void VisitQueryContinuation(QueryContinuationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OmittedArraySizeExpressionSyntax node.</summary>
        public virtual void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolatedStringExpressionSyntax node.</summary>
        public virtual void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IsPatternExpressionSyntax node.</summary>
        public virtual void VisitIsPatternExpression(IsPatternExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThrowExpressionSyntax node.</summary>
        public virtual void VisitThrowExpression(ThrowExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhenClauseSyntax node.</summary>
        public virtual void VisitWhenClause(WhenClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DiscardPatternSyntax node.</summary>
        public virtual void VisitDiscardPattern(DiscardPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DeclarationPatternSyntax node.</summary>
        public virtual void VisitDeclarationPattern(DeclarationPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VarPatternSyntax node.</summary>
        public virtual void VisitVarPattern(VarPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RecursivePatternSyntax node.</summary>
        public virtual void VisitRecursivePattern(RecursivePatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PositionalPatternClauseSyntax node.</summary>
        public virtual void VisitPositionalPatternClause(PositionalPatternClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PropertyPatternClauseSyntax node.</summary>
        public virtual void VisitPropertyPatternClause(PropertyPatternClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SubpatternSyntax node.</summary>
        public virtual void VisitSubpattern(SubpatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstantPatternSyntax node.</summary>
        public virtual void VisitConstantPattern(ConstantPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedPatternSyntax node.</summary>
        public virtual void VisitParenthesizedPattern(ParenthesizedPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RelationalPatternSyntax node.</summary>
        public virtual void VisitRelationalPattern(RelationalPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypePatternSyntax node.</summary>
        public virtual void VisitTypePattern(TypePatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BinaryPatternSyntax node.</summary>
        public virtual void VisitBinaryPattern(BinaryPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UnaryPatternSyntax node.</summary>
        public virtual void VisitUnaryPattern(UnaryPatternSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolatedStringTextSyntax node.</summary>
        public virtual void VisitInterpolatedStringText(InterpolatedStringTextSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationSyntax node.</summary>
        public virtual void VisitInterpolation(InterpolationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationAlignmentClauseSyntax node.</summary>
        public virtual void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterpolationFormatClauseSyntax node.</summary>
        public virtual void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GlobalStatementSyntax node.</summary>
        public virtual void VisitGlobalStatement(GlobalStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BlockSyntax node.</summary>
        public virtual void VisitBlock(BlockSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LocalFunctionStatementSyntax node.</summary>
        public virtual void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LocalDeclarationStatementSyntax node.</summary>
        public virtual void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VariableDeclarationSyntax node.</summary>
        public virtual void VisitVariableDeclaration(VariableDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a VariableDeclaratorSyntax node.</summary>
        public virtual void VisitVariableDeclarator(VariableDeclaratorSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EqualsValueClauseSyntax node.</summary>
        public virtual void VisitEqualsValueClause(EqualsValueClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SingleVariableDesignationSyntax node.</summary>
        public virtual void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DiscardDesignationSyntax node.</summary>
        public virtual void VisitDiscardDesignation(DiscardDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParenthesizedVariableDesignationSyntax node.</summary>
        public virtual void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExpressionStatementSyntax node.</summary>
        public virtual void VisitExpressionStatement(ExpressionStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EmptyStatementSyntax node.</summary>
        public virtual void VisitEmptyStatement(EmptyStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LabeledStatementSyntax node.</summary>
        public virtual void VisitLabeledStatement(LabeledStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a GotoStatementSyntax node.</summary>
        public virtual void VisitGotoStatement(GotoStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BreakStatementSyntax node.</summary>
        public virtual void VisitBreakStatement(BreakStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ContinueStatementSyntax node.</summary>
        public virtual void VisitContinueStatement(ContinueStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ReturnStatementSyntax node.</summary>
        public virtual void VisitReturnStatement(ReturnStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ThrowStatementSyntax node.</summary>
        public virtual void VisitThrowStatement(ThrowStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a YieldStatementSyntax node.</summary>
        public virtual void VisitYieldStatement(YieldStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WhileStatementSyntax node.</summary>
        public virtual void VisitWhileStatement(WhileStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DoStatementSyntax node.</summary>
        public virtual void VisitDoStatement(DoStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForStatementSyntax node.</summary>
        public virtual void VisitForStatement(ForStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForEachStatementSyntax node.</summary>
        public virtual void VisitForEachStatement(ForEachStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ForEachVariableStatementSyntax node.</summary>
        public virtual void VisitForEachVariableStatement(ForEachVariableStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UsingStatementSyntax node.</summary>
        public virtual void VisitUsingStatement(UsingStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FixedStatementSyntax node.</summary>
        public virtual void VisitFixedStatement(FixedStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CheckedStatementSyntax node.</summary>
        public virtual void VisitCheckedStatement(CheckedStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UnsafeStatementSyntax node.</summary>
        public virtual void VisitUnsafeStatement(UnsafeStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LockStatementSyntax node.</summary>
        public virtual void VisitLockStatement(LockStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IfStatementSyntax node.</summary>
        public virtual void VisitIfStatement(IfStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElseClauseSyntax node.</summary>
        public virtual void VisitElseClause(ElseClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchStatementSyntax node.</summary>
        public virtual void VisitSwitchStatement(SwitchStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchSectionSyntax node.</summary>
        public virtual void VisitSwitchSection(SwitchSectionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CasePatternSwitchLabelSyntax node.</summary>
        public virtual void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CaseSwitchLabelSyntax node.</summary>
        public virtual void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultSwitchLabelSyntax node.</summary>
        public virtual void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchExpressionSyntax node.</summary>
        public virtual void VisitSwitchExpression(SwitchExpressionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SwitchExpressionArmSyntax node.</summary>
        public virtual void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TryStatementSyntax node.</summary>
        public virtual void VisitTryStatement(TryStatementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchClauseSyntax node.</summary>
        public virtual void VisitCatchClause(CatchClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchDeclarationSyntax node.</summary>
        public virtual void VisitCatchDeclaration(CatchDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CatchFilterClauseSyntax node.</summary>
        public virtual void VisitCatchFilterClause(CatchFilterClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FinallyClauseSyntax node.</summary>
        public virtual void VisitFinallyClause(FinallyClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CompilationUnitSyntax node.</summary>
        public virtual void VisitCompilationUnit(CompilationUnitSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExternAliasDirectiveSyntax node.</summary>
        public virtual void VisitExternAliasDirective(ExternAliasDirectiveSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UsingDirectiveSyntax node.</summary>
        public virtual void VisitUsingDirective(UsingDirectiveSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NamespaceDeclarationSyntax node.</summary>
        public virtual void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeListSyntax node.</summary>
        public virtual void VisitAttributeList(AttributeListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeTargetSpecifierSyntax node.</summary>
        public virtual void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeSyntax node.</summary>
        public virtual void VisitAttribute(AttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeArgumentListSyntax node.</summary>
        public virtual void VisitAttributeArgumentList(AttributeArgumentListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AttributeArgumentSyntax node.</summary>
        public virtual void VisitAttributeArgument(AttributeArgumentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameEqualsSyntax node.</summary>
        public virtual void VisitNameEquals(NameEqualsSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterListSyntax node.</summary>
        public virtual void VisitTypeParameterList(TypeParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterSyntax node.</summary>
        public virtual void VisitTypeParameter(TypeParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ClassDeclarationSyntax node.</summary>
        public virtual void VisitClassDeclaration(ClassDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a StructDeclarationSyntax node.</summary>
        public virtual void VisitStructDeclaration(StructDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a InterfaceDeclarationSyntax node.</summary>
        public virtual void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RecordDeclarationSyntax node.</summary>
        public virtual void VisitRecordDeclaration(RecordDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EnumDeclarationSyntax node.</summary>
        public virtual void VisitEnumDeclaration(EnumDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DelegateDeclarationSyntax node.</summary>
        public virtual void VisitDelegateDeclaration(DelegateDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EnumMemberDeclarationSyntax node.</summary>
        public virtual void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BaseListSyntax node.</summary>
        public virtual void VisitBaseList(BaseListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SimpleBaseTypeSyntax node.</summary>
        public virtual void VisitSimpleBaseType(SimpleBaseTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PrimaryConstructorBaseTypeSyntax node.</summary>
        public virtual void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeParameterConstraintClauseSyntax node.</summary>
        public virtual void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorConstraintSyntax node.</summary>
        public virtual void VisitConstructorConstraint(ConstructorConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ClassOrStructConstraintSyntax node.</summary>
        public virtual void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeConstraintSyntax node.</summary>
        public virtual void VisitTypeConstraint(TypeConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefaultConstraintSyntax node.</summary>
        public virtual void VisitDefaultConstraint(DefaultConstraintSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FieldDeclarationSyntax node.</summary>
        public virtual void VisitFieldDeclaration(FieldDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EventFieldDeclarationSyntax node.</summary>
        public virtual void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ExplicitInterfaceSpecifierSyntax node.</summary>
        public virtual void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a MethodDeclarationSyntax node.</summary>
        public virtual void VisitMethodDeclaration(MethodDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OperatorDeclarationSyntax node.</summary>
        public virtual void VisitOperatorDeclaration(OperatorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConversionOperatorDeclarationSyntax node.</summary>
        public virtual void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorDeclarationSyntax node.</summary>
        public virtual void VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConstructorInitializerSyntax node.</summary>
        public virtual void VisitConstructorInitializer(ConstructorInitializerSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DestructorDeclarationSyntax node.</summary>
        public virtual void VisitDestructorDeclaration(DestructorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PropertyDeclarationSyntax node.</summary>
        public virtual void VisitPropertyDeclaration(PropertyDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ArrowExpressionClauseSyntax node.</summary>
        public virtual void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EventDeclarationSyntax node.</summary>
        public virtual void VisitEventDeclaration(EventDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IndexerDeclarationSyntax node.</summary>
        public virtual void VisitIndexerDeclaration(IndexerDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AccessorListSyntax node.</summary>
        public virtual void VisitAccessorList(AccessorListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a AccessorDeclarationSyntax node.</summary>
        public virtual void VisitAccessorDeclaration(AccessorDeclarationSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParameterListSyntax node.</summary>
        public virtual void VisitParameterList(ParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BracketedParameterListSyntax node.</summary>
        public virtual void VisitBracketedParameterList(BracketedParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ParameterSyntax node.</summary>
        public virtual void VisitParameter(ParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a FunctionPointerParameterSyntax node.</summary>
        public virtual void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IncompleteMemberSyntax node.</summary>
        public virtual void VisitIncompleteMember(IncompleteMemberSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a SkippedTokensTriviaSyntax node.</summary>
        public virtual void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DocumentationCommentTriviaSyntax node.</summary>
        public virtual void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a TypeCrefSyntax node.</summary>
        public virtual void VisitTypeCref(TypeCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a QualifiedCrefSyntax node.</summary>
        public virtual void VisitQualifiedCref(QualifiedCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NameMemberCrefSyntax node.</summary>
        public virtual void VisitNameMemberCref(NameMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IndexerMemberCrefSyntax node.</summary>
        public virtual void VisitIndexerMemberCref(IndexerMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a OperatorMemberCrefSyntax node.</summary>
        public virtual void VisitOperatorMemberCref(OperatorMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ConversionOperatorMemberCrefSyntax node.</summary>
        public virtual void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefParameterListSyntax node.</summary>
        public virtual void VisitCrefParameterList(CrefParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefBracketedParameterListSyntax node.</summary>
        public virtual void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a CrefParameterSyntax node.</summary>
        public virtual void VisitCrefParameter(CrefParameterSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementSyntax node.</summary>
        public virtual void VisitXmlElement(XmlElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementStartTagSyntax node.</summary>
        public virtual void VisitXmlElementStartTag(XmlElementStartTagSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlElementEndTagSyntax node.</summary>
        public virtual void VisitXmlElementEndTag(XmlElementEndTagSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlEmptyElementSyntax node.</summary>
        public virtual void VisitXmlEmptyElement(XmlEmptyElementSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlNameSyntax node.</summary>
        public virtual void VisitXmlName(XmlNameSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlPrefixSyntax node.</summary>
        public virtual void VisitXmlPrefix(XmlPrefixSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlTextAttributeSyntax node.</summary>
        public virtual void VisitXmlTextAttribute(XmlTextAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCrefAttributeSyntax node.</summary>
        public virtual void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlNameAttributeSyntax node.</summary>
        public virtual void VisitXmlNameAttribute(XmlNameAttributeSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlTextSyntax node.</summary>
        public virtual void VisitXmlText(XmlTextSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCDataSectionSyntax node.</summary>
        public virtual void VisitXmlCDataSection(XmlCDataSectionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlProcessingInstructionSyntax node.</summary>
        public virtual void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a XmlCommentSyntax node.</summary>
        public virtual void VisitXmlComment(XmlCommentSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a IfDirectiveTriviaSyntax node.</summary>
        public virtual void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElifDirectiveTriviaSyntax node.</summary>
        public virtual void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ElseDirectiveTriviaSyntax node.</summary>
        public virtual void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EndIfDirectiveTriviaSyntax node.</summary>
        public virtual void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a RegionDirectiveTriviaSyntax node.</summary>
        public virtual void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a EndRegionDirectiveTriviaSyntax node.</summary>
        public virtual void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ErrorDirectiveTriviaSyntax node.</summary>
        public virtual void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a WarningDirectiveTriviaSyntax node.</summary>
        public virtual void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a BadDirectiveTriviaSyntax node.</summary>
        public virtual void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a DefineDirectiveTriviaSyntax node.</summary>
        public virtual void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a UndefDirectiveTriviaSyntax node.</summary>
        public virtual void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LineDirectiveTriviaSyntax node.</summary>
        public virtual void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PragmaWarningDirectiveTriviaSyntax node.</summary>
        public virtual void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a PragmaChecksumDirectiveTriviaSyntax node.</summary>
        public virtual void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ReferenceDirectiveTriviaSyntax node.</summary>
        public virtual void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a LoadDirectiveTriviaSyntax node.</summary>
        public virtual void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a ShebangDirectiveTriviaSyntax node.</summary>
        public virtual void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node) => this.DefaultVisit(node);

        /// <summary>Called when the visitor visits a NullableDirectiveTriviaSyntax node.</summary>
        public virtual void VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node) => this.DefaultVisit(node);
    }

    public partial class CSharpSyntaxRewriter : CSharpSyntaxVisitor<SyntaxNode?>
    {
        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
            => node.Update(VisitToken(node.Identifier));

        public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
            => node.Update((NameSyntax?)Visit(node.Left) ?? throw new ArgumentNullException("left"), VisitToken(node.DotToken), (SimpleNameSyntax?)Visit(node.Right) ?? throw new ArgumentNullException("right"));

        public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
            => node.Update(VisitToken(node.Identifier), (TypeArgumentListSyntax?)Visit(node.TypeArgumentList) ?? throw new ArgumentNullException("typeArgumentList"));

        public override SyntaxNode? VisitTypeArgumentList(TypeArgumentListSyntax node)
            => node.Update(VisitToken(node.LessThanToken), VisitList(node.Arguments), VisitToken(node.GreaterThanToken));

        public override SyntaxNode? VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
            => node.Update((IdentifierNameSyntax?)Visit(node.Alias) ?? throw new ArgumentNullException("alias"), VisitToken(node.ColonColonToken), (SimpleNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"));

        public override SyntaxNode? VisitPredefinedType(PredefinedTypeSyntax node)
            => node.Update(VisitToken(node.Keyword));

        public override SyntaxNode? VisitArrayType(ArrayTypeSyntax node)
            => node.Update((TypeSyntax?)Visit(node.ElementType) ?? throw new ArgumentNullException("elementType"), VisitList(node.RankSpecifiers));

        public override SyntaxNode? VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Sizes), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitPointerType(PointerTypeSyntax node)
            => node.Update((TypeSyntax?)Visit(node.ElementType) ?? throw new ArgumentNullException("elementType"), VisitToken(node.AsteriskToken));

        public override SyntaxNode? VisitFunctionPointerType(FunctionPointerTypeSyntax node)
            => node.Update(VisitToken(node.DelegateKeyword), VisitToken(node.AsteriskToken), (FunctionPointerCallingConventionSyntax?)Visit(node.CallingConvention), (FunctionPointerParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"));

        public override SyntaxNode? VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
            => node.Update(VisitToken(node.LessThanToken), VisitList(node.Parameters), VisitToken(node.GreaterThanToken));

        public override SyntaxNode? VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
            => node.Update(VisitToken(node.ManagedOrUnmanagedKeyword), (FunctionPointerUnmanagedCallingConventionListSyntax?)Visit(node.UnmanagedCallingConventionList));

        public override SyntaxNode? VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), VisitList(node.CallingConventions), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
            => node.Update(VisitToken(node.Name));

        public override SyntaxNode? VisitNullableType(NullableTypeSyntax node)
            => node.Update((TypeSyntax?)Visit(node.ElementType) ?? throw new ArgumentNullException("elementType"), VisitToken(node.QuestionToken));

        public override SyntaxNode? VisitTupleType(TupleTypeSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Elements), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitTupleElement(TupleElementSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier));

        public override SyntaxNode? VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
            => node.Update(VisitToken(node.OmittedTypeArgumentToken));

        public override SyntaxNode? VisitRefType(RefTypeSyntax node)
            => node.Update(VisitToken(node.RefKeyword), VisitToken(node.ReadOnlyKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitTupleExpression(TupleExpressionSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
            => node.Update(VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.Operand) ?? throw new ArgumentNullException("operand"));

        public override SyntaxNode? VisitAwaitExpression(AwaitExpressionSyntax node)
            => node.Update(VisitToken(node.AwaitKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Operand) ?? throw new ArgumentNullException("operand"), VisitToken(node.OperatorToken));

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.OperatorToken), (SimpleNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"));

        public override SyntaxNode? VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.WhenNotNull) ?? throw new ArgumentNullException("whenNotNull"));

        public override SyntaxNode? VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
            => node.Update(VisitToken(node.OperatorToken), (SimpleNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"));

        public override SyntaxNode? VisitElementBindingExpression(ElementBindingExpressionSyntax node)
            => node.Update((BracketedArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitRangeExpression(RangeExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.LeftOperand), VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.RightOperand));

        public override SyntaxNode? VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
            => node.Update((BracketedArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Left) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.Right) ?? throw new ArgumentNullException("right"));

        public override SyntaxNode? VisitAssignmentExpression(AssignmentExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Left) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.Right) ?? throw new ArgumentNullException("right"));

        public override SyntaxNode? VisitConditionalExpression(ConditionalExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.QuestionToken), (ExpressionSyntax?)Visit(node.WhenTrue) ?? throw new ArgumentNullException("whenTrue"), VisitToken(node.ColonToken), (ExpressionSyntax?)Visit(node.WhenFalse) ?? throw new ArgumentNullException("whenFalse"));

        public override SyntaxNode? VisitThisExpression(ThisExpressionSyntax node)
            => node.Update(VisitToken(node.Token));

        public override SyntaxNode? VisitBaseExpression(BaseExpressionSyntax node)
            => node.Update(VisitToken(node.Token));

        public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
            => node.Update(VisitToken(node.Token));

        public override SyntaxNode? VisitMakeRefExpression(MakeRefExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitRefTypeExpression(RefTypeExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitRefValueExpression(RefValueExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.Comma), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitCheckedExpression(CheckedExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitDefaultExpression(DefaultExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitTypeOfExpression(TypeOfExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitSizeOfExpression(SizeOfExpressionSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), (ArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitElementAccessExpression(ElementAccessExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), (BracketedArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitArgumentList(ArgumentListSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitBracketedArgumentList(BracketedArgumentListSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Arguments), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitArgument(ArgumentSyntax node)
            => node.Update((NameColonSyntax?)Visit(node.NameColon), VisitToken(node.RefKindKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitNameColon(NameColonSyntax node)
            => node.Update((IdentifierNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitDeclarationExpression(DeclarationExpressionSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (VariableDesignationSyntax?)Visit(node.Designation) ?? throw new ArgumentNullException("designation"));

        public override SyntaxNode? VisitCastExpression(CastExpressionSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
            => node.Update(VisitList(node.Modifiers), VisitToken(node.DelegateKeyword), (ParameterListSyntax?)Visit(node.ParameterList), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"), (ExpressionSyntax?)Visit(node.ExpressionBody));

        public override SyntaxNode? VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (ParameterSyntax?)Visit(node.Parameter) ?? throw new ArgumentNullException("parameter"), VisitToken(node.ArrowToken), (BlockSyntax?)Visit(node.Block), (ExpressionSyntax?)Visit(node.ExpressionBody));

        public override SyntaxNode? VisitRefExpression(RefExpressionSyntax node)
            => node.Update(VisitToken(node.RefKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), VisitToken(node.ArrowToken), (BlockSyntax?)Visit(node.Block), (ExpressionSyntax?)Visit(node.ExpressionBody));

        public override SyntaxNode? VisitInitializerExpression(InitializerExpressionSyntax node)
            => node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Expressions), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
            => node.Update(VisitToken(node.NewKeyword), (ArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"), (InitializerExpressionSyntax?)Visit(node.Initializer));

        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
            => node.Update(VisitToken(node.NewKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ArgumentListSyntax?)Visit(node.ArgumentList), (InitializerExpressionSyntax?)Visit(node.Initializer));

        public override SyntaxNode? VisitWithExpression(WithExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.WithKeyword), (InitializerExpressionSyntax?)Visit(node.Initializer) ?? throw new ArgumentNullException("initializer"));

        public override SyntaxNode? VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
            => node.Update((NameEqualsSyntax?)Visit(node.NameEquals), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
            => node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenBraceToken), VisitList(node.Initializers), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
            => node.Update(VisitToken(node.NewKeyword), (ArrayTypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (InitializerExpressionSyntax?)Visit(node.Initializer));

        public override SyntaxNode? VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
            => node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenBracketToken), VisitList(node.Commas), VisitToken(node.CloseBracketToken), (InitializerExpressionSyntax?)Visit(node.Initializer) ?? throw new ArgumentNullException("initializer"));

        public override SyntaxNode? VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
            => node.Update(VisitToken(node.StackAllocKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (InitializerExpressionSyntax?)Visit(node.Initializer));

        public override SyntaxNode? VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
            => node.Update(VisitToken(node.StackAllocKeyword), VisitToken(node.OpenBracketToken), VisitToken(node.CloseBracketToken), (InitializerExpressionSyntax?)Visit(node.Initializer) ?? throw new ArgumentNullException("initializer"));

        public override SyntaxNode? VisitQueryExpression(QueryExpressionSyntax node)
            => node.Update((FromClauseSyntax?)Visit(node.FromClause) ?? throw new ArgumentNullException("fromClause"), (QueryBodySyntax?)Visit(node.Body) ?? throw new ArgumentNullException("body"));

        public override SyntaxNode? VisitQueryBody(QueryBodySyntax node)
            => node.Update(VisitList(node.Clauses), (SelectOrGroupClauseSyntax?)Visit(node.SelectOrGroup) ?? throw new ArgumentNullException("selectOrGroup"), (QueryContinuationSyntax?)Visit(node.Continuation));

        public override SyntaxNode? VisitFromClause(FromClauseSyntax node)
            => node.Update(VisitToken(node.FromKeyword), (TypeSyntax?)Visit(node.Type), VisitToken(node.Identifier), VisitToken(node.InKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitLetClause(LetClauseSyntax node)
            => node.Update(VisitToken(node.LetKeyword), VisitToken(node.Identifier), VisitToken(node.EqualsToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitJoinClause(JoinClauseSyntax node)
            => node.Update(VisitToken(node.JoinKeyword), (TypeSyntax?)Visit(node.Type), VisitToken(node.Identifier), VisitToken(node.InKeyword), (ExpressionSyntax?)Visit(node.InExpression) ?? throw new ArgumentNullException("inExpression"), VisitToken(node.OnKeyword), (ExpressionSyntax?)Visit(node.LeftExpression) ?? throw new ArgumentNullException("leftExpression"), VisitToken(node.EqualsKeyword), (ExpressionSyntax?)Visit(node.RightExpression) ?? throw new ArgumentNullException("rightExpression"), (JoinIntoClauseSyntax?)Visit(node.Into));

        public override SyntaxNode? VisitJoinIntoClause(JoinIntoClauseSyntax node)
            => node.Update(VisitToken(node.IntoKeyword), VisitToken(node.Identifier));

        public override SyntaxNode? VisitWhereClause(WhereClauseSyntax node)
            => node.Update(VisitToken(node.WhereKeyword), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"));

        public override SyntaxNode? VisitOrderByClause(OrderByClauseSyntax node)
            => node.Update(VisitToken(node.OrderByKeyword), VisitList(node.Orderings));

        public override SyntaxNode? VisitOrdering(OrderingSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.AscendingOrDescendingKeyword));

        public override SyntaxNode? VisitSelectClause(SelectClauseSyntax node)
            => node.Update(VisitToken(node.SelectKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitGroupClause(GroupClauseSyntax node)
            => node.Update(VisitToken(node.GroupKeyword), (ExpressionSyntax?)Visit(node.GroupExpression) ?? throw new ArgumentNullException("groupExpression"), VisitToken(node.ByKeyword), (ExpressionSyntax?)Visit(node.ByExpression) ?? throw new ArgumentNullException("byExpression"));

        public override SyntaxNode? VisitQueryContinuation(QueryContinuationSyntax node)
            => node.Update(VisitToken(node.IntoKeyword), VisitToken(node.Identifier), (QueryBodySyntax?)Visit(node.Body) ?? throw new ArgumentNullException("body"));

        public override SyntaxNode? VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
            => node.Update(VisitToken(node.OmittedArraySizeExpressionToken));

        public override SyntaxNode? VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
            => node.Update(VisitToken(node.StringStartToken), VisitList(node.Contents), VisitToken(node.StringEndToken));

        public override SyntaxNode? VisitIsPatternExpression(IsPatternExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.IsKeyword), (PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"));

        public override SyntaxNode? VisitThrowExpression(ThrowExpressionSyntax node)
            => node.Update(VisitToken(node.ThrowKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitWhenClause(WhenClauseSyntax node)
            => node.Update(VisitToken(node.WhenKeyword), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"));

        public override SyntaxNode? VisitDiscardPattern(DiscardPatternSyntax node)
            => node.Update(VisitToken(node.UnderscoreToken));

        public override SyntaxNode? VisitDeclarationPattern(DeclarationPatternSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (VariableDesignationSyntax?)Visit(node.Designation) ?? throw new ArgumentNullException("designation"));

        public override SyntaxNode? VisitVarPattern(VarPatternSyntax node)
            => node.Update(VisitToken(node.VarKeyword), (VariableDesignationSyntax?)Visit(node.Designation) ?? throw new ArgumentNullException("designation"));

        public override SyntaxNode? VisitRecursivePattern(RecursivePatternSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type), (PositionalPatternClauseSyntax?)Visit(node.PositionalPatternClause), (PropertyPatternClauseSyntax?)Visit(node.PropertyPatternClause), (VariableDesignationSyntax?)Visit(node.Designation));

        public override SyntaxNode? VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Subpatterns), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
            => node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Subpatterns), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitSubpattern(SubpatternSyntax node)
            => node.Update((NameColonSyntax?)Visit(node.NameColon), (PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"));

        public override SyntaxNode? VisitConstantPattern(ConstantPatternSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), (PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitRelationalPattern(RelationalPatternSyntax node)
            => node.Update(VisitToken(node.OperatorToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitTypePattern(TypePatternSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitBinaryPattern(BinaryPatternSyntax node)
            => node.Update((PatternSyntax?)Visit(node.Left) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), (PatternSyntax?)Visit(node.Right) ?? throw new ArgumentNullException("right"));

        public override SyntaxNode? VisitUnaryPattern(UnaryPatternSyntax node)
            => node.Update(VisitToken(node.OperatorToken), (PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"));

        public override SyntaxNode? VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
            => node.Update(VisitToken(node.TextToken));

        public override SyntaxNode? VisitInterpolation(InterpolationSyntax node)
            => node.Update(VisitToken(node.OpenBraceToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), (InterpolationAlignmentClauseSyntax?)Visit(node.AlignmentClause), (InterpolationFormatClauseSyntax?)Visit(node.FormatClause), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
            => node.Update(VisitToken(node.CommaToken), (ExpressionSyntax?)Visit(node.Value) ?? throw new ArgumentNullException("value"));

        public override SyntaxNode? VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
            => node.Update(VisitToken(node.ColonToken), VisitToken(node.FormatStringToken));

        public override SyntaxNode? VisitGlobalStatement(GlobalStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitBlock(BlockSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.OpenBraceToken), VisitList(node.Statements), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.ReturnType) ?? throw new ArgumentNullException("returnType"), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.UsingKeyword), VisitList(node.Modifiers), (VariableDeclarationSyntax?)Visit(node.Declaration) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitVariableDeclaration(VariableDeclarationSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitList(node.Variables));

        public override SyntaxNode? VisitVariableDeclarator(VariableDeclaratorSyntax node)
            => node.Update(VisitToken(node.Identifier), (BracketedArgumentListSyntax?)Visit(node.ArgumentList), (EqualsValueClauseSyntax?)Visit(node.Initializer));

        public override SyntaxNode? VisitEqualsValueClause(EqualsValueClauseSyntax node)
            => node.Update(VisitToken(node.EqualsToken), (ExpressionSyntax?)Visit(node.Value) ?? throw new ArgumentNullException("value"));

        public override SyntaxNode? VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
            => node.Update(VisitToken(node.Identifier));

        public override SyntaxNode? VisitDiscardDesignation(DiscardDesignationSyntax node)
            => node.Update(VisitToken(node.UnderscoreToken));

        public override SyntaxNode? VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Variables), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitEmptyStatement(EmptyStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitLabeledStatement(LabeledStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.Identifier), VisitToken(node.ColonToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitGotoStatement(GotoStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.GotoKeyword), VisitToken(node.CaseOrDefaultKeyword), (ExpressionSyntax?)Visit(node.Expression), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitBreakStatement(BreakStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.BreakKeyword), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitContinueStatement(ContinueStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.ContinueKeyword), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitReturnStatement(ReturnStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.ReturnKeyword), (ExpressionSyntax?)Visit(node.Expression), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.ThrowKeyword), (ExpressionSyntax?)Visit(node.Expression), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitYieldStatement(YieldStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.YieldKeyword), VisitToken(node.ReturnOrBreakKeyword), (ExpressionSyntax?)Visit(node.Expression), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.WhileKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.DoKeyword), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"), VisitToken(node.WhileKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.ForKeyword), VisitToken(node.OpenParenToken), (VariableDeclarationSyntax?)Visit(node.Declaration), VisitList(node.Initializers), VisitToken(node.FirstSemicolonToken), (ExpressionSyntax?)Visit(node.Condition), VisitToken(node.SecondSemicolonToken), VisitList(node.Incrementors), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.ForEachKeyword), VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier), VisitToken(node.InKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.ForEachKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Variable) ?? throw new ArgumentNullException("variable"), VisitToken(node.InKeyword), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitUsingStatement(UsingStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.UsingKeyword), VisitToken(node.OpenParenToken), (VariableDeclarationSyntax?)Visit(node.Declaration), (ExpressionSyntax?)Visit(node.Expression), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitFixedStatement(FixedStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.FixedKeyword), VisitToken(node.OpenParenToken), (VariableDeclarationSyntax?)Visit(node.Declaration) ?? throw new ArgumentNullException("declaration"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitCheckedStatement(CheckedStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.Keyword), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"));

        public override SyntaxNode? VisitUnsafeStatement(UnsafeStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.UnsafeKeyword), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"));

        public override SyntaxNode? VisitLockStatement(LockStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.LockKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitIfStatement(IfStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.IfKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"), (ElseClauseSyntax?)Visit(node.Else));

        public override SyntaxNode? VisitElseClause(ElseClauseSyntax node)
            => node.Update(VisitToken(node.ElseKeyword), (StatementSyntax?)Visit(node.Statement) ?? throw new ArgumentNullException("statement"));

        public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.SwitchKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), VisitToken(node.OpenBraceToken), VisitList(node.Sections), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node)
            => node.Update(VisitList(node.Labels), VisitList(node.Statements));

        public override SyntaxNode? VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
            => node.Update(VisitToken(node.Keyword), (PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"), (WhenClauseSyntax?)Visit(node.WhenClause), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
            => node.Update(VisitToken(node.Keyword), (ExpressionSyntax?)Visit(node.Value) ?? throw new ArgumentNullException("value"), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
            => node.Update(VisitToken(node.Keyword), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitSwitchExpression(SwitchExpressionSyntax node)
            => node.Update((ExpressionSyntax?)Visit(node.GoverningExpression) ?? throw new ArgumentNullException("governingExpression"), VisitToken(node.SwitchKeyword), VisitToken(node.OpenBraceToken), VisitList(node.Arms), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
            => node.Update((PatternSyntax?)Visit(node.Pattern) ?? throw new ArgumentNullException("pattern"), (WhenClauseSyntax?)Visit(node.WhenClause), VisitToken(node.EqualsGreaterThanToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitTryStatement(TryStatementSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.TryKeyword), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"), VisitList(node.Catches), (FinallyClauseSyntax?)Visit(node.Finally));

        public override SyntaxNode? VisitCatchClause(CatchClauseSyntax node)
            => node.Update(VisitToken(node.CatchKeyword), (CatchDeclarationSyntax?)Visit(node.Declaration), (CatchFilterClauseSyntax?)Visit(node.Filter), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"));

        public override SyntaxNode? VisitCatchDeclaration(CatchDeclarationSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitCatchFilterClause(CatchFilterClauseSyntax node)
            => node.Update(VisitToken(node.WhenKeyword), VisitToken(node.OpenParenToken), (ExpressionSyntax?)Visit(node.FilterExpression) ?? throw new ArgumentNullException("filterExpression"), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitFinallyClause(FinallyClauseSyntax node)
            => node.Update(VisitToken(node.FinallyKeyword), (BlockSyntax?)Visit(node.Block) ?? throw new ArgumentNullException("block"));

        public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
            => node.Update(VisitList(node.Externs), VisitList(node.Usings), VisitList(node.AttributeLists), VisitList(node.Members), VisitToken(node.EndOfFileToken));

        public override SyntaxNode? VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
            => node.Update(VisitToken(node.ExternKeyword), VisitToken(node.AliasKeyword), VisitToken(node.Identifier), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node)
            => node.Update(VisitToken(node.GlobalKeyword), VisitToken(node.UsingKeyword), VisitToken(node.StaticKeyword), (NameEqualsSyntax?)Visit(node.Alias), (NameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.NamespaceKeyword), (NameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.OpenBraceToken), VisitList(node.Externs), VisitList(node.Usings), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitAttributeList(AttributeListSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), (AttributeTargetSpecifierSyntax?)Visit(node.Target), VisitList(node.Attributes), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
            => node.Update(VisitToken(node.Identifier), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitAttribute(AttributeSyntax node)
            => node.Update((NameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), (AttributeArgumentListSyntax?)Visit(node.ArgumentList));

        public override SyntaxNode? VisitAttributeArgumentList(AttributeArgumentListSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitAttributeArgument(AttributeArgumentSyntax node)
            => node.Update((NameEqualsSyntax?)Visit(node.NameEquals), (NameColonSyntax?)Visit(node.NameColon), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitNameEquals(NameEqualsSyntax node)
            => node.Update((IdentifierNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken));

        public override SyntaxNode? VisitTypeParameterList(TypeParameterListSyntax node)
            => node.Update(VisitToken(node.LessThanToken), VisitList(node.Parameters), VisitToken(node.GreaterThanToken));

        public override SyntaxNode? VisitTypeParameter(TypeParameterSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitToken(node.VarianceKeyword), VisitToken(node.Identifier));

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (BaseListSyntax?)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (BaseListSyntax?)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (BaseListSyntax?)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.ClassOrStructKeyword), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (ParameterListSyntax?)Visit(node.ParameterList), (BaseListSyntax?)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitEnumDeclaration(EnumDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EnumKeyword), VisitToken(node.Identifier), (BaseListSyntax?)Visit(node.BaseList), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitDelegateDeclaration(DelegateDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.DelegateKeyword), (TypeSyntax?)Visit(node.ReturnType) ?? throw new ArgumentNullException("returnType"), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Identifier), (EqualsValueClauseSyntax?)Visit(node.EqualsValue));

        public override SyntaxNode? VisitBaseList(BaseListSyntax node)
            => node.Update(VisitToken(node.ColonToken), VisitList(node.Types));

        public override SyntaxNode? VisitSimpleBaseType(SimpleBaseTypeSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
            => node.Update(VisitToken(node.WhereKeyword), (IdentifierNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.ColonToken), VisitList(node.Constraints));

        public override SyntaxNode? VisitConstructorConstraint(ConstructorConstraintSyntax node)
            => node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenParenToken), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
            => node.Update(VisitToken(node.ClassOrStructKeyword), VisitToken(node.QuestionToken));

        public override SyntaxNode? VisitTypeConstraint(TypeConstraintSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitDefaultConstraint(DefaultConstraintSyntax node)
            => node.Update(VisitToken(node.DefaultKeyword));

        public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (VariableDeclarationSyntax?)Visit(node.Declaration) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EventKeyword), (VariableDeclarationSyntax?)Visit(node.Declaration) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
            => node.Update((NameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.DotToken));

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.ReturnType) ?? throw new ArgumentNullException("returnType"), (ExplicitInterfaceSpecifierSyntax?)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (TypeParameterListSyntax?)Visit(node.TypeParameterList), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.ReturnType) ?? throw new ArgumentNullException("returnType"), VisitToken(node.OperatorKeyword), VisitToken(node.OperatorToken), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.ImplicitOrExplicitKeyword), VisitToken(node.OperatorKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Identifier), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), (ConstructorInitializerSyntax?)Visit(node.Initializer), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitConstructorInitializer(ConstructorInitializerSyntax node)
            => node.Update(VisitToken(node.ColonToken), VisitToken(node.ThisOrBaseKeyword), (ArgumentListSyntax?)Visit(node.ArgumentList) ?? throw new ArgumentNullException("argumentList"));

        public override SyntaxNode? VisitDestructorDeclaration(DestructorDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.TildeToken), VisitToken(node.Identifier), (ParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax?)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (AccessorListSyntax?)Visit(node.AccessorList), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), (EqualsValueClauseSyntax?)Visit(node.Initializer), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
            => node.Update(VisitToken(node.ArrowToken), (ExpressionSyntax?)Visit(node.Expression) ?? throw new ArgumentNullException("expression"));

        public override SyntaxNode? VisitEventDeclaration(EventDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EventKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax?)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (AccessorListSyntax?)Visit(node.AccessorList), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax?)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.ThisKeyword), (BracketedParameterListSyntax?)Visit(node.ParameterList) ?? throw new ArgumentNullException("parameterList"), (AccessorListSyntax?)Visit(node.AccessorList), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitAccessorList(AccessorListSyntax node)
            => node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Accessors), VisitToken(node.CloseBraceToken));

        public override SyntaxNode? VisitAccessorDeclaration(AccessorDeclarationSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), (BlockSyntax?)Visit(node.Body), (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));

        public override SyntaxNode? VisitParameterList(ParameterListSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Parameters), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitBracketedParameterList(BracketedParameterListSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Parameters), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitParameter(ParameterSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.Type), VisitToken(node.Identifier), (EqualsValueClauseSyntax?)Visit(node.Default));

        public override SyntaxNode? VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitIncompleteMember(IncompleteMemberSyntax node)
            => node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax?)Visit(node.Type));

        public override SyntaxNode? VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
            => node.Update(VisitList(node.Tokens));

        public override SyntaxNode? VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
            => node.Update(VisitList(node.Content), VisitToken(node.EndOfComment));

        public override SyntaxNode? VisitTypeCref(TypeCrefSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitQualifiedCref(QualifiedCrefSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Container) ?? throw new ArgumentNullException("container"), VisitToken(node.DotToken), (MemberCrefSyntax?)Visit(node.Member) ?? throw new ArgumentNullException("member"));

        public override SyntaxNode? VisitNameMemberCref(NameMemberCrefSyntax node)
            => node.Update((TypeSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), (CrefParameterListSyntax?)Visit(node.Parameters));

        public override SyntaxNode? VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
            => node.Update(VisitToken(node.ThisKeyword), (CrefBracketedParameterListSyntax?)Visit(node.Parameters));

        public override SyntaxNode? VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
            => node.Update(VisitToken(node.OperatorKeyword), VisitToken(node.OperatorToken), (CrefParameterListSyntax?)Visit(node.Parameters));

        public override SyntaxNode? VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
            => node.Update(VisitToken(node.ImplicitOrExplicitKeyword), VisitToken(node.OperatorKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"), (CrefParameterListSyntax?)Visit(node.Parameters));

        public override SyntaxNode? VisitCrefParameterList(CrefParameterListSyntax node)
            => node.Update(VisitToken(node.OpenParenToken), VisitList(node.Parameters), VisitToken(node.CloseParenToken));

        public override SyntaxNode? VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
            => node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Parameters), VisitToken(node.CloseBracketToken));

        public override SyntaxNode? VisitCrefParameter(CrefParameterSyntax node)
            => node.Update(VisitToken(node.RefKindKeyword), (TypeSyntax?)Visit(node.Type) ?? throw new ArgumentNullException("type"));

        public override SyntaxNode? VisitXmlElement(XmlElementSyntax node)
            => node.Update((XmlElementStartTagSyntax?)Visit(node.StartTag) ?? throw new ArgumentNullException("startTag"), VisitList(node.Content), (XmlElementEndTagSyntax?)Visit(node.EndTag) ?? throw new ArgumentNullException("endTag"));

        public override SyntaxNode? VisitXmlElementStartTag(XmlElementStartTagSyntax node)
            => node.Update(VisitToken(node.LessThanToken), (XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitList(node.Attributes), VisitToken(node.GreaterThanToken));

        public override SyntaxNode? VisitXmlElementEndTag(XmlElementEndTagSyntax node)
            => node.Update(VisitToken(node.LessThanSlashToken), (XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.GreaterThanToken));

        public override SyntaxNode? VisitXmlEmptyElement(XmlEmptyElementSyntax node)
            => node.Update(VisitToken(node.LessThanToken), (XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitList(node.Attributes), VisitToken(node.SlashGreaterThanToken));

        public override SyntaxNode? VisitXmlName(XmlNameSyntax node)
            => node.Update((XmlPrefixSyntax?)Visit(node.Prefix), VisitToken(node.LocalName));

        public override SyntaxNode? VisitXmlPrefix(XmlPrefixSyntax node)
            => node.Update(VisitToken(node.Prefix), VisitToken(node.ColonToken));

        public override SyntaxNode? VisitXmlTextAttribute(XmlTextAttributeSyntax node)
            => node.Update((XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), VisitList(node.TextTokens), VisitToken(node.EndQuoteToken));

        public override SyntaxNode? VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
            => node.Update((XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), (CrefSyntax?)Visit(node.Cref) ?? throw new ArgumentNullException("cref"), VisitToken(node.EndQuoteToken));

        public override SyntaxNode? VisitXmlNameAttribute(XmlNameAttributeSyntax node)
            => node.Update((XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), (IdentifierNameSyntax?)Visit(node.Identifier) ?? throw new ArgumentNullException("identifier"), VisitToken(node.EndQuoteToken));

        public override SyntaxNode? VisitXmlText(XmlTextSyntax node)
            => node.Update(VisitList(node.TextTokens));

        public override SyntaxNode? VisitXmlCDataSection(XmlCDataSectionSyntax node)
            => node.Update(VisitToken(node.StartCDataToken), VisitList(node.TextTokens), VisitToken(node.EndCDataToken));

        public override SyntaxNode? VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
            => node.Update(VisitToken(node.StartProcessingInstructionToken), (XmlNameSyntax?)Visit(node.Name) ?? throw new ArgumentNullException("name"), VisitList(node.TextTokens), VisitToken(node.EndProcessingInstructionToken));

        public override SyntaxNode? VisitXmlComment(XmlCommentSyntax node)
            => node.Update(VisitToken(node.LessThanExclamationMinusMinusToken), VisitList(node.TextTokens), VisitToken(node.MinusMinusGreaterThanToken));

        public override SyntaxNode? VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.IfKeyword), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);

        public override SyntaxNode? VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.ElifKeyword), (ExpressionSyntax?)Visit(node.Condition) ?? throw new ArgumentNullException("condition"), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);

        public override SyntaxNode? VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.ElseKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken);

        public override SyntaxNode? VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.EndIfKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.RegionKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.EndRegionKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.ErrorKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.WarningKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.Identifier), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.DefineKeyword), VisitToken(node.Name), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.UndefKeyword), VisitToken(node.Name), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.LineKeyword), VisitToken(node.Line), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.PragmaKeyword), VisitToken(node.WarningKeyword), VisitToken(node.DisableOrRestoreKeyword), VisitList(node.ErrorCodes), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.PragmaKeyword), VisitToken(node.ChecksumKeyword), VisitToken(node.File), VisitToken(node.Guid), VisitToken(node.Bytes), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.ReferenceKeyword), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.LoadKeyword), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.ExclamationToken), VisitToken(node.EndOfDirectiveToken), node.IsActive);

        public override SyntaxNode? VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
            => node.Update(VisitToken(node.HashToken), VisitToken(node.NullableKeyword), VisitToken(node.SettingToken), VisitToken(node.TargetToken), VisitToken(node.EndOfDirectiveToken), node.IsActive);
    }

    public static partial class SyntaxFactory
    {

        /// <summary>Creates a new IdentifierNameSyntax instance.</summary>
        public static IdentifierNameSyntax IdentifierName(SyntaxToken identifier)
        {
            switch (identifier.Kind())
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.GlobalKeyword: break;
                default: throw new ArgumentException(nameof(identifier));
            }
            return (IdentifierNameSyntax)Syntax.InternalSyntax.SyntaxFactory.IdentifierName((Syntax.InternalSyntax.SyntaxToken)identifier.Node!).CreateRed();
        }

        /// <summary>Creates a new QualifiedNameSyntax instance.</summary>
        public static QualifiedNameSyntax QualifiedName(NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (dotToken.Kind() != SyntaxKind.DotToken) throw new ArgumentException(nameof(dotToken));
            if (right == null) throw new ArgumentNullException(nameof(right));
            return (QualifiedNameSyntax)Syntax.InternalSyntax.SyntaxFactory.QualifiedName((Syntax.InternalSyntax.NameSyntax)left.Green, (Syntax.InternalSyntax.SyntaxToken)dotToken.Node!, (Syntax.InternalSyntax.SimpleNameSyntax)right.Green).CreateRed();
        }

        /// <summary>Creates a new QualifiedNameSyntax instance.</summary>
        public static QualifiedNameSyntax QualifiedName(NameSyntax left, SimpleNameSyntax right)
            => SyntaxFactory.QualifiedName(left, SyntaxFactory.Token(SyntaxKind.DotToken), right);

        /// <summary>Creates a new GenericNameSyntax instance.</summary>
        public static GenericNameSyntax GenericName(SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (typeArgumentList == null) throw new ArgumentNullException(nameof(typeArgumentList));
            return (GenericNameSyntax)Syntax.InternalSyntax.SyntaxFactory.GenericName((Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.TypeArgumentListSyntax)typeArgumentList.Green).CreateRed();
        }

        /// <summary>Creates a new GenericNameSyntax instance.</summary>
        public static GenericNameSyntax GenericName(SyntaxToken identifier)
            => SyntaxFactory.GenericName(identifier, SyntaxFactory.TypeArgumentList());

        /// <summary>Creates a new GenericNameSyntax instance.</summary>
        public static GenericNameSyntax GenericName(string identifier)
            => SyntaxFactory.GenericName(SyntaxFactory.Identifier(identifier), SyntaxFactory.TypeArgumentList());

        /// <summary>Creates a new TypeArgumentListSyntax instance.</summary>
        public static TypeArgumentListSyntax TypeArgumentList(SyntaxToken lessThanToken, SeparatedSyntaxList<TypeSyntax> arguments, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken) throw new ArgumentException(nameof(lessThanToken));
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken) throw new ArgumentException(nameof(greaterThanToken));
            return (TypeArgumentListSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeArgumentList((Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node!, arguments.Node.ToGreenSeparatedList<Syntax.InternalSyntax.TypeSyntax>(), (Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new TypeArgumentListSyntax instance.</summary>
        public static TypeArgumentListSyntax TypeArgumentList(SeparatedSyntaxList<TypeSyntax> arguments = default)
            => SyntaxFactory.TypeArgumentList(SyntaxFactory.Token(SyntaxKind.LessThanToken), arguments, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new AliasQualifiedNameSyntax instance.</summary>
        public static AliasQualifiedNameSyntax AliasQualifiedName(IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            if (colonColonToken.Kind() != SyntaxKind.ColonColonToken) throw new ArgumentException(nameof(colonColonToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return (AliasQualifiedNameSyntax)Syntax.InternalSyntax.SyntaxFactory.AliasQualifiedName((Syntax.InternalSyntax.IdentifierNameSyntax)alias.Green, (Syntax.InternalSyntax.SyntaxToken)colonColonToken.Node!, (Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        /// <summary>Creates a new AliasQualifiedNameSyntax instance.</summary>
        public static AliasQualifiedNameSyntax AliasQualifiedName(IdentifierNameSyntax alias, SimpleNameSyntax name)
            => SyntaxFactory.AliasQualifiedName(alias, SyntaxFactory.Token(SyntaxKind.ColonColonToken), name);

        /// <summary>Creates a new AliasQualifiedNameSyntax instance.</summary>
        public static AliasQualifiedNameSyntax AliasQualifiedName(string alias, SimpleNameSyntax name)
            => SyntaxFactory.AliasQualifiedName(SyntaxFactory.IdentifierName(alias), SyntaxFactory.Token(SyntaxKind.ColonColonToken), name);

        /// <summary>Creates a new PredefinedTypeSyntax instance.</summary>
        public static PredefinedTypeSyntax PredefinedType(SyntaxToken keyword)
        {
            switch (keyword.Kind())
            {
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.UShortKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.CharKeyword:
                case SyntaxKind.ObjectKeyword:
                case SyntaxKind.VoidKeyword: break;
                default: throw new ArgumentException(nameof(keyword));
            }
            return (PredefinedTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.PredefinedType((Syntax.InternalSyntax.SyntaxToken)keyword.Node!).CreateRed();
        }

        /// <summary>Creates a new ArrayTypeSyntax instance.</summary>
        public static ArrayTypeSyntax ArrayType(TypeSyntax elementType, SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            return (ArrayTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.ArrayType((Syntax.InternalSyntax.TypeSyntax)elementType.Green, rankSpecifiers.Node.ToGreenList<Syntax.InternalSyntax.ArrayRankSpecifierSyntax>()).CreateRed();
        }

        /// <summary>Creates a new ArrayTypeSyntax instance.</summary>
        public static ArrayTypeSyntax ArrayType(TypeSyntax elementType)
            => SyntaxFactory.ArrayType(elementType, default);

        /// <summary>Creates a new ArrayRankSpecifierSyntax instance.</summary>
        public static ArrayRankSpecifierSyntax ArrayRankSpecifier(SyntaxToken openBracketToken, SeparatedSyntaxList<ExpressionSyntax> sizes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (ArrayRankSpecifierSyntax)Syntax.InternalSyntax.SyntaxFactory.ArrayRankSpecifier((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, sizes.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ExpressionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ArrayRankSpecifierSyntax instance.</summary>
        public static ArrayRankSpecifierSyntax ArrayRankSpecifier(SeparatedSyntaxList<ExpressionSyntax> sizes = default)
            => SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), sizes, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new PointerTypeSyntax instance.</summary>
        public static PointerTypeSyntax PointerType(TypeSyntax elementType, SyntaxToken asteriskToken)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            if (asteriskToken.Kind() != SyntaxKind.AsteriskToken) throw new ArgumentException(nameof(asteriskToken));
            return (PointerTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.PointerType((Syntax.InternalSyntax.TypeSyntax)elementType.Green, (Syntax.InternalSyntax.SyntaxToken)asteriskToken.Node!).CreateRed();
        }

        /// <summary>Creates a new PointerTypeSyntax instance.</summary>
        public static PointerTypeSyntax PointerType(TypeSyntax elementType)
            => SyntaxFactory.PointerType(elementType, SyntaxFactory.Token(SyntaxKind.AsteriskToken));

        /// <summary>Creates a new FunctionPointerTypeSyntax instance.</summary>
        public static FunctionPointerTypeSyntax FunctionPointerType(SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword) throw new ArgumentException(nameof(delegateKeyword));
            if (asteriskToken.Kind() != SyntaxKind.AsteriskToken) throw new ArgumentException(nameof(asteriskToken));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            return (FunctionPointerTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerType((Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)asteriskToken.Node!, callingConvention == null ? null : (Syntax.InternalSyntax.FunctionPointerCallingConventionSyntax)callingConvention.Green, (Syntax.InternalSyntax.FunctionPointerParameterListSyntax)parameterList.Green).CreateRed();
        }

        /// <summary>Creates a new FunctionPointerTypeSyntax instance.</summary>
        public static FunctionPointerTypeSyntax FunctionPointerType(FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList)
            => SyntaxFactory.FunctionPointerType(SyntaxFactory.Token(SyntaxKind.DelegateKeyword), SyntaxFactory.Token(SyntaxKind.AsteriskToken), callingConvention, parameterList);

        /// <summary>Creates a new FunctionPointerTypeSyntax instance.</summary>
        public static FunctionPointerTypeSyntax FunctionPointerType()
            => SyntaxFactory.FunctionPointerType(SyntaxFactory.Token(SyntaxKind.DelegateKeyword), SyntaxFactory.Token(SyntaxKind.AsteriskToken), default, SyntaxFactory.FunctionPointerParameterList());

        /// <summary>Creates a new FunctionPointerParameterListSyntax instance.</summary>
        public static FunctionPointerParameterListSyntax FunctionPointerParameterList(SyntaxToken lessThanToken, SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken) throw new ArgumentException(nameof(lessThanToken));
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken) throw new ArgumentException(nameof(greaterThanToken));
            return (FunctionPointerParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerParameterList((Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.FunctionPointerParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new FunctionPointerParameterListSyntax instance.</summary>
        public static FunctionPointerParameterListSyntax FunctionPointerParameterList(SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters = default)
            => SyntaxFactory.FunctionPointerParameterList(SyntaxFactory.Token(SyntaxKind.LessThanToken), parameters, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new FunctionPointerCallingConventionSyntax instance.</summary>
        public static FunctionPointerCallingConventionSyntax FunctionPointerCallingConvention(SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
        {
            switch (managedOrUnmanagedKeyword.Kind())
            {
                case SyntaxKind.ManagedKeyword:
                case SyntaxKind.UnmanagedKeyword: break;
                default: throw new ArgumentException(nameof(managedOrUnmanagedKeyword));
            }
            return (FunctionPointerCallingConventionSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerCallingConvention((Syntax.InternalSyntax.SyntaxToken)managedOrUnmanagedKeyword.Node!, unmanagedCallingConventionList == null ? null : (Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)unmanagedCallingConventionList.Green).CreateRed();
        }

        /// <summary>Creates a new FunctionPointerCallingConventionSyntax instance.</summary>
        public static FunctionPointerCallingConventionSyntax FunctionPointerCallingConvention(SyntaxToken managedOrUnmanagedKeyword)
            => SyntaxFactory.FunctionPointerCallingConvention(managedOrUnmanagedKeyword, default);

        /// <summary>Creates a new FunctionPointerUnmanagedCallingConventionListSyntax instance.</summary>
        public static FunctionPointerUnmanagedCallingConventionListSyntax FunctionPointerUnmanagedCallingConventionList(SyntaxToken openBracketToken, SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (FunctionPointerUnmanagedCallingConventionListSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerUnmanagedCallingConventionList((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, callingConventions.Node.ToGreenSeparatedList<Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new FunctionPointerUnmanagedCallingConventionListSyntax instance.</summary>
        public static FunctionPointerUnmanagedCallingConventionListSyntax FunctionPointerUnmanagedCallingConventionList(SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions = default)
            => SyntaxFactory.FunctionPointerUnmanagedCallingConventionList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), callingConventions, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new FunctionPointerUnmanagedCallingConventionSyntax instance.</summary>
        public static FunctionPointerUnmanagedCallingConventionSyntax FunctionPointerUnmanagedCallingConvention(SyntaxToken name)
        {
            if (name.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(name));
            return (FunctionPointerUnmanagedCallingConventionSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerUnmanagedCallingConvention((Syntax.InternalSyntax.SyntaxToken)name.Node!).CreateRed();
        }

        /// <summary>Creates a new NullableTypeSyntax instance.</summary>
        public static NullableTypeSyntax NullableType(TypeSyntax elementType, SyntaxToken questionToken)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            if (questionToken.Kind() != SyntaxKind.QuestionToken) throw new ArgumentException(nameof(questionToken));
            return (NullableTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.NullableType((Syntax.InternalSyntax.TypeSyntax)elementType.Green, (Syntax.InternalSyntax.SyntaxToken)questionToken.Node!).CreateRed();
        }

        /// <summary>Creates a new NullableTypeSyntax instance.</summary>
        public static NullableTypeSyntax NullableType(TypeSyntax elementType)
            => SyntaxFactory.NullableType(elementType, SyntaxFactory.Token(SyntaxKind.QuestionToken));

        /// <summary>Creates a new TupleTypeSyntax instance.</summary>
        public static TupleTypeSyntax TupleType(SyntaxToken openParenToken, SeparatedSyntaxList<TupleElementSyntax> elements, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (TupleTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.TupleType((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, elements.Node.ToGreenSeparatedList<Syntax.InternalSyntax.TupleElementSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new TupleTypeSyntax instance.</summary>
        public static TupleTypeSyntax TupleType(SeparatedSyntaxList<TupleElementSyntax> elements = default)
            => SyntaxFactory.TupleType(SyntaxFactory.Token(SyntaxKind.OpenParenToken), elements, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new TupleElementSyntax instance.</summary>
        public static TupleElementSyntax TupleElement(TypeSyntax type, SyntaxToken identifier)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            switch (identifier.Kind())
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(identifier));
            }
            return (TupleElementSyntax)Syntax.InternalSyntax.SyntaxFactory.TupleElement((Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken?)identifier.Node).CreateRed();
        }

        /// <summary>Creates a new TupleElementSyntax instance.</summary>
        public static TupleElementSyntax TupleElement(TypeSyntax type)
            => SyntaxFactory.TupleElement(type, default);

        /// <summary>Creates a new OmittedTypeArgumentSyntax instance.</summary>
        public static OmittedTypeArgumentSyntax OmittedTypeArgument(SyntaxToken omittedTypeArgumentToken)
        {
            if (omittedTypeArgumentToken.Kind() != SyntaxKind.OmittedTypeArgumentToken) throw new ArgumentException(nameof(omittedTypeArgumentToken));
            return (OmittedTypeArgumentSyntax)Syntax.InternalSyntax.SyntaxFactory.OmittedTypeArgument((Syntax.InternalSyntax.SyntaxToken)omittedTypeArgumentToken.Node!).CreateRed();
        }

        /// <summary>Creates a new OmittedTypeArgumentSyntax instance.</summary>
        public static OmittedTypeArgumentSyntax OmittedTypeArgument()
            => SyntaxFactory.OmittedTypeArgument(SyntaxFactory.Token(SyntaxKind.OmittedTypeArgumentToken));

        /// <summary>Creates a new RefTypeSyntax instance.</summary>
        public static RefTypeSyntax RefType(SyntaxToken refKeyword, SyntaxToken readOnlyKeyword, TypeSyntax type)
        {
            if (refKeyword.Kind() != SyntaxKind.RefKeyword) throw new ArgumentException(nameof(refKeyword));
            switch (readOnlyKeyword.Kind())
            {
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(readOnlyKeyword));
            }
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (RefTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.RefType((Syntax.InternalSyntax.SyntaxToken)refKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)readOnlyKeyword.Node, (Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new RefTypeSyntax instance.</summary>
        public static RefTypeSyntax RefType(TypeSyntax type)
            => SyntaxFactory.RefType(SyntaxFactory.Token(SyntaxKind.RefKeyword), default, type);

        /// <summary>Creates a new ParenthesizedExpressionSyntax instance.</summary>
        public static ParenthesizedExpressionSyntax ParenthesizedExpression(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ParenthesizedExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ParenthesizedExpression((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ParenthesizedExpressionSyntax instance.</summary>
        public static ParenthesizedExpressionSyntax ParenthesizedExpression(ExpressionSyntax expression)
            => SyntaxFactory.ParenthesizedExpression(SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new TupleExpressionSyntax instance.</summary>
        public static TupleExpressionSyntax TupleExpression(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (TupleExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.TupleExpression((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, arguments.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ArgumentSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new TupleExpressionSyntax instance.</summary>
        public static TupleExpressionSyntax TupleExpression(SeparatedSyntaxList<ArgumentSyntax> arguments = default)
            => SyntaxFactory.TupleExpression(SyntaxFactory.Token(SyntaxKind.OpenParenToken), arguments, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new PrefixUnaryExpressionSyntax instance.</summary>
        public static PrefixUnaryExpressionSyntax PrefixUnaryExpression(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            switch (kind)
            {
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                case SyntaxKind.IndexExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (operatorToken.Kind())
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.TildeToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.CaretToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            return (PrefixUnaryExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.PrefixUnaryExpression(kind, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)operand.Green).CreateRed();
        }

        /// <summary>Creates a new PrefixUnaryExpressionSyntax instance.</summary>
        public static PrefixUnaryExpressionSyntax PrefixUnaryExpression(SyntaxKind kind, ExpressionSyntax operand)
            => SyntaxFactory.PrefixUnaryExpression(kind, SyntaxFactory.Token(GetPrefixUnaryExpressionOperatorTokenKind(kind)), operand);

        private static SyntaxKind GetPrefixUnaryExpressionOperatorTokenKind(SyntaxKind kind)
            => kind switch
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

        /// <summary>Creates a new AwaitExpressionSyntax instance.</summary>
        public static AwaitExpressionSyntax AwaitExpression(SyntaxToken awaitKeyword, ExpressionSyntax expression)
        {
            if (awaitKeyword.Kind() != SyntaxKind.AwaitKeyword) throw new ArgumentException(nameof(awaitKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (AwaitExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.AwaitExpression((Syntax.InternalSyntax.SyntaxToken)awaitKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new AwaitExpressionSyntax instance.</summary>
        public static AwaitExpressionSyntax AwaitExpression(ExpressionSyntax expression)
            => SyntaxFactory.AwaitExpression(SyntaxFactory.Token(SyntaxKind.AwaitKeyword), expression);

        /// <summary>Creates a new PostfixUnaryExpressionSyntax instance.</summary>
        public static PostfixUnaryExpressionSyntax PostfixUnaryExpression(SyntaxKind kind, ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            switch (kind)
            {
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.SuppressNullableWarningExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.ExclamationToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            return (PostfixUnaryExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.PostfixUnaryExpression(kind, (Syntax.InternalSyntax.ExpressionSyntax)operand.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!).CreateRed();
        }

        /// <summary>Creates a new PostfixUnaryExpressionSyntax instance.</summary>
        public static PostfixUnaryExpressionSyntax PostfixUnaryExpression(SyntaxKind kind, ExpressionSyntax operand)
            => SyntaxFactory.PostfixUnaryExpression(kind, operand, SyntaxFactory.Token(GetPostfixUnaryExpressionOperatorTokenKind(kind)));

        private static SyntaxKind GetPostfixUnaryExpressionOperatorTokenKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.PostIncrementExpression => SyntaxKind.PlusPlusToken,
                SyntaxKind.PostDecrementExpression => SyntaxKind.MinusMinusToken,
                SyntaxKind.SuppressNullableWarningExpression => SyntaxKind.ExclamationToken,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new MemberAccessExpressionSyntax instance.</summary>
        public static MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            switch (kind)
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.PointerMemberAccessExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.DotToken:
                case SyntaxKind.MinusGreaterThanToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (name == null) throw new ArgumentNullException(nameof(name));
            return (MemberAccessExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.MemberAccessExpression(kind, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        /// <summary>Creates a new MemberAccessExpressionSyntax instance.</summary>
        public static MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, ExpressionSyntax expression, SimpleNameSyntax name)
            => SyntaxFactory.MemberAccessExpression(kind, expression, SyntaxFactory.Token(GetMemberAccessExpressionOperatorTokenKind(kind)), name);

        private static SyntaxKind GetMemberAccessExpressionOperatorTokenKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.SimpleMemberAccessExpression => SyntaxKind.DotToken,
                SyntaxKind.PointerMemberAccessExpression => SyntaxKind.MinusGreaterThanToken,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new ConditionalAccessExpressionSyntax instance.</summary>
        public static ConditionalAccessExpressionSyntax ConditionalAccessExpression(ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (operatorToken.Kind() != SyntaxKind.QuestionToken) throw new ArgumentException(nameof(operatorToken));
            if (whenNotNull == null) throw new ArgumentNullException(nameof(whenNotNull));
            return (ConditionalAccessExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ConditionalAccessExpression((Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)whenNotNull.Green).CreateRed();
        }

        /// <summary>Creates a new ConditionalAccessExpressionSyntax instance.</summary>
        public static ConditionalAccessExpressionSyntax ConditionalAccessExpression(ExpressionSyntax expression, ExpressionSyntax whenNotNull)
            => SyntaxFactory.ConditionalAccessExpression(expression, SyntaxFactory.Token(SyntaxKind.QuestionToken), whenNotNull);

        /// <summary>Creates a new MemberBindingExpressionSyntax instance.</summary>
        public static MemberBindingExpressionSyntax MemberBindingExpression(SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (operatorToken.Kind() != SyntaxKind.DotToken) throw new ArgumentException(nameof(operatorToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return (MemberBindingExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.MemberBindingExpression((Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.SimpleNameSyntax)name.Green).CreateRed();
        }

        /// <summary>Creates a new MemberBindingExpressionSyntax instance.</summary>
        public static MemberBindingExpressionSyntax MemberBindingExpression(SimpleNameSyntax name)
            => SyntaxFactory.MemberBindingExpression(SyntaxFactory.Token(SyntaxKind.DotToken), name);

        /// <summary>Creates a new ElementBindingExpressionSyntax instance.</summary>
        public static ElementBindingExpressionSyntax ElementBindingExpression(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (ElementBindingExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ElementBindingExpression((Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new ElementBindingExpressionSyntax instance.</summary>
        public static ElementBindingExpressionSyntax ElementBindingExpression()
            => SyntaxFactory.ElementBindingExpression(SyntaxFactory.BracketedArgumentList());

        /// <summary>Creates a new RangeExpressionSyntax instance.</summary>
        public static RangeExpressionSyntax RangeExpression(ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand)
        {
            if (operatorToken.Kind() != SyntaxKind.DotDotToken) throw new ArgumentException(nameof(operatorToken));
            return (RangeExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.RangeExpression(leftOperand == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)leftOperand.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, rightOperand == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)rightOperand.Green).CreateRed();
        }

        /// <summary>Creates a new RangeExpressionSyntax instance.</summary>
        public static RangeExpressionSyntax RangeExpression(ExpressionSyntax? leftOperand, ExpressionSyntax? rightOperand)
            => SyntaxFactory.RangeExpression(leftOperand, SyntaxFactory.Token(SyntaxKind.DotDotToken), rightOperand);

        /// <summary>Creates a new RangeExpressionSyntax instance.</summary>
        public static RangeExpressionSyntax RangeExpression()
            => SyntaxFactory.RangeExpression(default, SyntaxFactory.Token(SyntaxKind.DotDotToken), default);

        /// <summary>Creates a new ImplicitElementAccessSyntax instance.</summary>
        public static ImplicitElementAccessSyntax ImplicitElementAccess(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (ImplicitElementAccessSyntax)Syntax.InternalSyntax.SyntaxFactory.ImplicitElementAccess((Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new ImplicitElementAccessSyntax instance.</summary>
        public static ImplicitElementAccessSyntax ImplicitElementAccess()
            => SyntaxFactory.ImplicitElementAccess(SyntaxFactory.BracketedArgumentList());

        /// <summary>Creates a new BinaryExpressionSyntax instance.</summary>
        public static BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            switch (kind)
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.CoalesceExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (left == null) throw new ArgumentNullException(nameof(left));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.AsKeyword:
                case SyntaxKind.QuestionQuestionToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (right == null) throw new ArgumentNullException(nameof(right));
            return (BinaryExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.BinaryExpression(kind, (Syntax.InternalSyntax.ExpressionSyntax)left.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)right.Green).CreateRed();
        }

        /// <summary>Creates a new BinaryExpressionSyntax instance.</summary>
        public static BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, ExpressionSyntax left, ExpressionSyntax right)
            => SyntaxFactory.BinaryExpression(kind, left, SyntaxFactory.Token(GetBinaryExpressionOperatorTokenKind(kind)), right);

        private static SyntaxKind GetBinaryExpressionOperatorTokenKind(SyntaxKind kind)
            => kind switch
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

        /// <summary>Creates a new AssignmentExpressionSyntax instance.</summary>
        public static AssignmentExpressionSyntax AssignmentExpression(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            switch (kind)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (left == null) throw new ArgumentNullException(nameof(left));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.EqualsToken:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.AsteriskEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                case SyntaxKind.PercentEqualsToken:
                case SyntaxKind.AmpersandEqualsToken:
                case SyntaxKind.CaretEqualsToken:
                case SyntaxKind.BarEqualsToken:
                case SyntaxKind.LessThanLessThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                case SyntaxKind.QuestionQuestionEqualsToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (right == null) throw new ArgumentNullException(nameof(right));
            return (AssignmentExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.AssignmentExpression(kind, (Syntax.InternalSyntax.ExpressionSyntax)left.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)right.Green).CreateRed();
        }

        /// <summary>Creates a new AssignmentExpressionSyntax instance.</summary>
        public static AssignmentExpressionSyntax AssignmentExpression(SyntaxKind kind, ExpressionSyntax left, ExpressionSyntax right)
            => SyntaxFactory.AssignmentExpression(kind, left, SyntaxFactory.Token(GetAssignmentExpressionOperatorTokenKind(kind)), right);

        private static SyntaxKind GetAssignmentExpressionOperatorTokenKind(SyntaxKind kind)
            => kind switch
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

        /// <summary>Creates a new ConditionalExpressionSyntax instance.</summary>
        public static ConditionalExpressionSyntax ConditionalExpression(ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (questionToken.Kind() != SyntaxKind.QuestionToken) throw new ArgumentException(nameof(questionToken));
            if (whenTrue == null) throw new ArgumentNullException(nameof(whenTrue));
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            if (whenFalse == null) throw new ArgumentNullException(nameof(whenFalse));
            return (ConditionalExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ConditionalExpression((Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)questionToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)whenTrue.Green, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)whenFalse.Green).CreateRed();
        }

        /// <summary>Creates a new ConditionalExpressionSyntax instance.</summary>
        public static ConditionalExpressionSyntax ConditionalExpression(ExpressionSyntax condition, ExpressionSyntax whenTrue, ExpressionSyntax whenFalse)
            => SyntaxFactory.ConditionalExpression(condition, SyntaxFactory.Token(SyntaxKind.QuestionToken), whenTrue, SyntaxFactory.Token(SyntaxKind.ColonToken), whenFalse);

        /// <summary>Creates a new ThisExpressionSyntax instance.</summary>
        public static ThisExpressionSyntax ThisExpression(SyntaxToken token)
        {
            if (token.Kind() != SyntaxKind.ThisKeyword) throw new ArgumentException(nameof(token));
            return (ThisExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ThisExpression((Syntax.InternalSyntax.SyntaxToken)token.Node!).CreateRed();
        }

        /// <summary>Creates a new ThisExpressionSyntax instance.</summary>
        public static ThisExpressionSyntax ThisExpression()
            => SyntaxFactory.ThisExpression(SyntaxFactory.Token(SyntaxKind.ThisKeyword));

        /// <summary>Creates a new BaseExpressionSyntax instance.</summary>
        public static BaseExpressionSyntax BaseExpression(SyntaxToken token)
        {
            if (token.Kind() != SyntaxKind.BaseKeyword) throw new ArgumentException(nameof(token));
            return (BaseExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.BaseExpression((Syntax.InternalSyntax.SyntaxToken)token.Node!).CreateRed();
        }

        /// <summary>Creates a new BaseExpressionSyntax instance.</summary>
        public static BaseExpressionSyntax BaseExpression()
            => SyntaxFactory.BaseExpression(SyntaxFactory.Token(SyntaxKind.BaseKeyword));

        /// <summary>Creates a new LiteralExpressionSyntax instance.</summary>
        public static LiteralExpressionSyntax LiteralExpression(SyntaxKind kind, SyntaxToken token)
        {
            switch (kind)
            {
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.DefaultLiteralExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (token.Kind())
            {
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.DefaultKeyword: break;
                default: throw new ArgumentException(nameof(token));
            }
            return (LiteralExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.LiteralExpression(kind, (Syntax.InternalSyntax.SyntaxToken)token.Node!).CreateRed();
        }

        /// <summary>Creates a new LiteralExpressionSyntax instance.</summary>
        public static LiteralExpressionSyntax LiteralExpression(SyntaxKind kind)
            => SyntaxFactory.LiteralExpression(kind, SyntaxFactory.Token(GetLiteralExpressionTokenKind(kind)));

        private static SyntaxKind GetLiteralExpressionTokenKind(SyntaxKind kind)
            => kind switch
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

        /// <summary>Creates a new MakeRefExpressionSyntax instance.</summary>
        public static MakeRefExpressionSyntax MakeRefExpression(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.MakeRefKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (MakeRefExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.MakeRefExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new MakeRefExpressionSyntax instance.</summary>
        public static MakeRefExpressionSyntax MakeRefExpression(ExpressionSyntax expression)
            => SyntaxFactory.MakeRefExpression(SyntaxFactory.Token(SyntaxKind.MakeRefKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new RefTypeExpressionSyntax instance.</summary>
        public static RefTypeExpressionSyntax RefTypeExpression(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.RefTypeKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (RefTypeExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.RefTypeExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new RefTypeExpressionSyntax instance.</summary>
        public static RefTypeExpressionSyntax RefTypeExpression(ExpressionSyntax expression)
            => SyntaxFactory.RefTypeExpression(SyntaxFactory.Token(SyntaxKind.RefTypeKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new RefValueExpressionSyntax instance.</summary>
        public static RefValueExpressionSyntax RefValueExpression(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken comma, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.RefValueKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (comma.Kind() != SyntaxKind.CommaToken) throw new ArgumentException(nameof(comma));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (RefValueExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.RefValueExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)comma.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new RefValueExpressionSyntax instance.</summary>
        public static RefValueExpressionSyntax RefValueExpression(ExpressionSyntax expression, TypeSyntax type)
            => SyntaxFactory.RefValueExpression(SyntaxFactory.Token(SyntaxKind.RefValueKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CommaToken), type, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new CheckedExpressionSyntax instance.</summary>
        public static CheckedExpressionSyntax CheckedExpression(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            switch (kind)
            {
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (keyword.Kind())
            {
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword: break;
                default: throw new ArgumentException(nameof(keyword));
            }
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (CheckedExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.CheckedExpression(kind, (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CheckedExpressionSyntax instance.</summary>
        public static CheckedExpressionSyntax CheckedExpression(SyntaxKind kind, ExpressionSyntax expression)
            => SyntaxFactory.CheckedExpression(kind, SyntaxFactory.Token(GetCheckedExpressionKeywordKind(kind)), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        private static SyntaxKind GetCheckedExpressionKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.CheckedExpression => SyntaxKind.CheckedKeyword,
                SyntaxKind.UncheckedExpression => SyntaxKind.UncheckedKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new DefaultExpressionSyntax instance.</summary>
        public static DefaultExpressionSyntax DefaultExpression(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.DefaultKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (DefaultExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.DefaultExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DefaultExpressionSyntax instance.</summary>
        public static DefaultExpressionSyntax DefaultExpression(TypeSyntax type)
            => SyntaxFactory.DefaultExpression(SyntaxFactory.Token(SyntaxKind.DefaultKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new TypeOfExpressionSyntax instance.</summary>
        public static TypeOfExpressionSyntax TypeOfExpression(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.TypeOfKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (TypeOfExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeOfExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new TypeOfExpressionSyntax instance.</summary>
        public static TypeOfExpressionSyntax TypeOfExpression(TypeSyntax type)
            => SyntaxFactory.TypeOfExpression(SyntaxFactory.Token(SyntaxKind.TypeOfKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new SizeOfExpressionSyntax instance.</summary>
        public static SizeOfExpressionSyntax SizeOfExpression(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword.Kind() != SyntaxKind.SizeOfKeyword) throw new ArgumentException(nameof(keyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (SizeOfExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.SizeOfExpression((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new SizeOfExpressionSyntax instance.</summary>
        public static SizeOfExpressionSyntax SizeOfExpression(TypeSyntax type)
            => SyntaxFactory.SizeOfExpression(SyntaxFactory.Token(SyntaxKind.SizeOfKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new InvocationExpressionSyntax instance.</summary>
        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, ArgumentListSyntax argumentList)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (InvocationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.InvocationExpression((Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new InvocationExpressionSyntax instance.</summary>
        public static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression)
            => SyntaxFactory.InvocationExpression(expression, SyntaxFactory.ArgumentList());

        /// <summary>Creates a new ElementAccessExpressionSyntax instance.</summary>
        public static ElementAccessExpressionSyntax ElementAccessExpression(ExpressionSyntax expression, BracketedArgumentListSyntax argumentList)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (ElementAccessExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ElementAccessExpression((Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new ElementAccessExpressionSyntax instance.</summary>
        public static ElementAccessExpressionSyntax ElementAccessExpression(ExpressionSyntax expression)
            => SyntaxFactory.ElementAccessExpression(expression, SyntaxFactory.BracketedArgumentList());

        /// <summary>Creates a new ArgumentListSyntax instance.</summary>
        public static ArgumentListSyntax ArgumentList(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ArgumentListSyntax)Syntax.InternalSyntax.SyntaxFactory.ArgumentList((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, arguments.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ArgumentSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ArgumentListSyntax instance.</summary>
        public static ArgumentListSyntax ArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments = default)
            => SyntaxFactory.ArgumentList(SyntaxFactory.Token(SyntaxKind.OpenParenToken), arguments, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new BracketedArgumentListSyntax instance.</summary>
        public static BracketedArgumentListSyntax BracketedArgumentList(SyntaxToken openBracketToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (BracketedArgumentListSyntax)Syntax.InternalSyntax.SyntaxFactory.BracketedArgumentList((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, arguments.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ArgumentSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new BracketedArgumentListSyntax instance.</summary>
        public static BracketedArgumentListSyntax BracketedArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments = default)
            => SyntaxFactory.BracketedArgumentList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), arguments, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new ArgumentSyntax instance.</summary>
        public static ArgumentSyntax Argument(NameColonSyntax? nameColon, SyntaxToken refKindKeyword, ExpressionSyntax expression)
        {
            switch (refKindKeyword.Kind())
            {
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(refKindKeyword));
            }
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (ArgumentSyntax)Syntax.InternalSyntax.SyntaxFactory.Argument(nameColon == null ? null : (Syntax.InternalSyntax.NameColonSyntax)nameColon.Green, (Syntax.InternalSyntax.SyntaxToken?)refKindKeyword.Node, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new ArgumentSyntax instance.</summary>
        public static ArgumentSyntax Argument(ExpressionSyntax expression)
            => SyntaxFactory.Argument(default, default, expression);

        /// <summary>Creates a new NameColonSyntax instance.</summary>
        public static NameColonSyntax NameColon(IdentifierNameSyntax name, SyntaxToken colonToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            return (NameColonSyntax)Syntax.InternalSyntax.SyntaxFactory.NameColon((Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new NameColonSyntax instance.</summary>
        public static NameColonSyntax NameColon(IdentifierNameSyntax name)
            => SyntaxFactory.NameColon(name, SyntaxFactory.Token(SyntaxKind.ColonToken));

        /// <summary>Creates a new NameColonSyntax instance.</summary>
        public static NameColonSyntax NameColon(string name)
            => SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(name), SyntaxFactory.Token(SyntaxKind.ColonToken));

        /// <summary>Creates a new DeclarationExpressionSyntax instance.</summary>
        public static DeclarationExpressionSyntax DeclarationExpression(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (designation == null) throw new ArgumentNullException(nameof(designation));
            return (DeclarationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.DeclarationExpression((Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        /// <summary>Creates a new CastExpressionSyntax instance.</summary>
        public static CastExpressionSyntax CastExpression(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (CastExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.CastExpression((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new CastExpressionSyntax instance.</summary>
        public static CastExpressionSyntax CastExpression(TypeSyntax type, ExpressionSyntax expression)
            => SyntaxFactory.CastExpression(SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, SyntaxFactory.Token(SyntaxKind.CloseParenToken), expression);

        /// <summary>Creates a new AnonymousMethodExpressionSyntax instance.</summary>
        public static AnonymousMethodExpressionSyntax AnonymousMethodExpression(SyntaxTokenList modifiers, SyntaxToken delegateKeyword, ParameterListSyntax? parameterList, BlockSyntax block, ExpressionSyntax? expressionBody)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword) throw new ArgumentException(nameof(delegateKeyword));
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (AnonymousMethodExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.AnonymousMethodExpression(modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node!, parameterList == null ? null : (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (Syntax.InternalSyntax.BlockSyntax)block.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expressionBody.Green).CreateRed();
        }

        /// <summary>Creates a new SimpleLambdaExpressionSyntax instance.</summary>
        public static SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken) throw new ArgumentException(nameof(arrowToken));
            return (SimpleLambdaExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.SimpleLambdaExpression(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.ParameterSyntax)parameter.Green, (Syntax.InternalSyntax.SyntaxToken)arrowToken.Node!, block == null ? null : (Syntax.InternalSyntax.BlockSyntax)block.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expressionBody.Green).CreateRed();
        }

        /// <summary>Creates a new SimpleLambdaExpressionSyntax instance.</summary>
        public static SimpleLambdaExpressionSyntax SimpleLambdaExpression(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterSyntax parameter, BlockSyntax? block, ExpressionSyntax? expressionBody)
            => SyntaxFactory.SimpleLambdaExpression(attributeLists, modifiers, parameter, SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), block, expressionBody);

        /// <summary>Creates a new SimpleLambdaExpressionSyntax instance.</summary>
        public static SimpleLambdaExpressionSyntax SimpleLambdaExpression(ParameterSyntax parameter)
            => SyntaxFactory.SimpleLambdaExpression(default, default(SyntaxTokenList), parameter, SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), default, default);

        /// <summary>Creates a new RefExpressionSyntax instance.</summary>
        public static RefExpressionSyntax RefExpression(SyntaxToken refKeyword, ExpressionSyntax expression)
        {
            if (refKeyword.Kind() != SyntaxKind.RefKeyword) throw new ArgumentException(nameof(refKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (RefExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.RefExpression((Syntax.InternalSyntax.SyntaxToken)refKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new RefExpressionSyntax instance.</summary>
        public static RefExpressionSyntax RefExpression(ExpressionSyntax expression)
            => SyntaxFactory.RefExpression(SyntaxFactory.Token(SyntaxKind.RefKeyword), expression);

        /// <summary>Creates a new ParenthesizedLambdaExpressionSyntax instance.</summary>
        public static ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken) throw new ArgumentException(nameof(arrowToken));
            return (ParenthesizedLambdaExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ParenthesizedLambdaExpression(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, (Syntax.InternalSyntax.SyntaxToken)arrowToken.Node!, block == null ? null : (Syntax.InternalSyntax.BlockSyntax)block.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expressionBody.Green).CreateRed();
        }

        /// <summary>Creates a new ParenthesizedLambdaExpressionSyntax instance.</summary>
        public static ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterListSyntax parameterList, BlockSyntax? block, ExpressionSyntax? expressionBody)
            => SyntaxFactory.ParenthesizedLambdaExpression(attributeLists, modifiers, parameterList, SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), block, expressionBody);

        /// <summary>Creates a new ParenthesizedLambdaExpressionSyntax instance.</summary>
        public static ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression()
            => SyntaxFactory.ParenthesizedLambdaExpression(default, default(SyntaxTokenList), SyntaxFactory.ParameterList(), SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), default, default);

        /// <summary>Creates a new InitializerExpressionSyntax instance.</summary>
        public static InitializerExpressionSyntax InitializerExpression(SyntaxKind kind, SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            switch (kind)
            {
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                case SyntaxKind.WithInitializerExpression: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (InitializerExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.InitializerExpression(kind, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, expressions.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ExpressionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new InitializerExpressionSyntax instance.</summary>
        public static InitializerExpressionSyntax InitializerExpression(SyntaxKind kind, SeparatedSyntaxList<ExpressionSyntax> expressions = default)
            => SyntaxFactory.InitializerExpression(kind, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), expressions, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new ImplicitObjectCreationExpressionSyntax instance.</summary>
        public static ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression(SyntaxToken newKeyword, ArgumentListSyntax argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (ImplicitObjectCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ImplicitObjectCreationExpression((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green, initializer == null ? null : (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new ImplicitObjectCreationExpressionSyntax instance.</summary>
        public static ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression(ArgumentListSyntax argumentList, InitializerExpressionSyntax? initializer)
            => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), argumentList, initializer);

        /// <summary>Creates a new ImplicitObjectCreationExpressionSyntax instance.</summary>
        public static ImplicitObjectCreationExpressionSyntax ImplicitObjectCreationExpression()
            => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), SyntaxFactory.ArgumentList(), default);

        /// <summary>Creates a new ObjectCreationExpressionSyntax instance.</summary>
        public static ObjectCreationExpressionSyntax ObjectCreationExpression(SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (ObjectCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ObjectCreationExpression((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, argumentList == null ? null : (Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green, initializer == null ? null : (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new ObjectCreationExpressionSyntax instance.</summary>
        public static ObjectCreationExpressionSyntax ObjectCreationExpression(TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer)
            => SyntaxFactory.ObjectCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, argumentList, initializer);

        /// <summary>Creates a new ObjectCreationExpressionSyntax instance.</summary>
        public static ObjectCreationExpressionSyntax ObjectCreationExpression(TypeSyntax type)
            => SyntaxFactory.ObjectCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, default, default);

        /// <summary>Creates a new WithExpressionSyntax instance.</summary>
        public static WithExpressionSyntax WithExpression(ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (withKeyword.Kind() != SyntaxKind.WithKeyword) throw new ArgumentException(nameof(withKeyword));
            if (initializer == null) throw new ArgumentNullException(nameof(initializer));
            return (WithExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.WithExpression((Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)withKeyword.Node!, (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new WithExpressionSyntax instance.</summary>
        public static WithExpressionSyntax WithExpression(ExpressionSyntax expression, InitializerExpressionSyntax initializer)
            => SyntaxFactory.WithExpression(expression, SyntaxFactory.Token(SyntaxKind.WithKeyword), initializer);

        /// <summary>Creates a new AnonymousObjectMemberDeclaratorSyntax instance.</summary>
        public static AnonymousObjectMemberDeclaratorSyntax AnonymousObjectMemberDeclarator(NameEqualsSyntax? nameEquals, ExpressionSyntax expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (AnonymousObjectMemberDeclaratorSyntax)Syntax.InternalSyntax.SyntaxFactory.AnonymousObjectMemberDeclarator(nameEquals == null ? null : (Syntax.InternalSyntax.NameEqualsSyntax)nameEquals.Green, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new AnonymousObjectMemberDeclaratorSyntax instance.</summary>
        public static AnonymousObjectMemberDeclaratorSyntax AnonymousObjectMemberDeclarator(ExpressionSyntax expression)
            => SyntaxFactory.AnonymousObjectMemberDeclarator(default, expression);

        /// <summary>Creates a new AnonymousObjectCreationExpressionSyntax instance.</summary>
        public static AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(SyntaxToken newKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers, SyntaxToken closeBraceToken)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (AnonymousObjectCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.AnonymousObjectCreationExpression((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, initializers.Node.ToGreenSeparatedList<Syntax.InternalSyntax.AnonymousObjectMemberDeclaratorSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new AnonymousObjectCreationExpressionSyntax instance.</summary>
        public static AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers = default)
            => SyntaxFactory.AnonymousObjectCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), SyntaxFactory.Token(SyntaxKind.OpenBraceToken), initializers, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new ArrayCreationExpressionSyntax instance.</summary>
        public static ArrayCreationExpressionSyntax ArrayCreationExpression(SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (ArrayCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ArrayCreationExpression((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.ArrayTypeSyntax)type.Green, initializer == null ? null : (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new ArrayCreationExpressionSyntax instance.</summary>
        public static ArrayCreationExpressionSyntax ArrayCreationExpression(ArrayTypeSyntax type, InitializerExpressionSyntax? initializer)
            => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, initializer);

        /// <summary>Creates a new ArrayCreationExpressionSyntax instance.</summary>
        public static ArrayCreationExpressionSyntax ArrayCreationExpression(ArrayTypeSyntax type)
            => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, default);

        /// <summary>Creates a new ImplicitArrayCreationExpressionSyntax instance.</summary>
        public static ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(SyntaxToken newKeyword, SyntaxToken openBracketToken, SyntaxTokenList commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            if (initializer == null) throw new ArgumentNullException(nameof(initializer));
            return (ImplicitArrayCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ImplicitArrayCreationExpression((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, commas.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!, (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new ImplicitArrayCreationExpressionSyntax instance.</summary>
        public static ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(SyntaxTokenList commas, InitializerExpressionSyntax initializer)
            => SyntaxFactory.ImplicitArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), SyntaxFactory.Token(SyntaxKind.OpenBracketToken), commas, SyntaxFactory.Token(SyntaxKind.CloseBracketToken), initializer);

        /// <summary>Creates a new ImplicitArrayCreationExpressionSyntax instance.</summary>
        public static ImplicitArrayCreationExpressionSyntax ImplicitArrayCreationExpression(InitializerExpressionSyntax initializer)
            => SyntaxFactory.ImplicitArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), SyntaxFactory.Token(SyntaxKind.OpenBracketToken), default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.CloseBracketToken), initializer);

        /// <summary>Creates a new StackAllocArrayCreationExpressionSyntax instance.</summary>
        public static StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer)
        {
            if (stackAllocKeyword.Kind() != SyntaxKind.StackAllocKeyword) throw new ArgumentException(nameof(stackAllocKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (StackAllocArrayCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.StackAllocArrayCreationExpression((Syntax.InternalSyntax.SyntaxToken)stackAllocKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, initializer == null ? null : (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new StackAllocArrayCreationExpressionSyntax instance.</summary>
        public static StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(TypeSyntax type, InitializerExpressionSyntax? initializer)
            => SyntaxFactory.StackAllocArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.StackAllocKeyword), type, initializer);

        /// <summary>Creates a new StackAllocArrayCreationExpressionSyntax instance.</summary>
        public static StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(TypeSyntax type)
            => SyntaxFactory.StackAllocArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.StackAllocKeyword), type, default);

        /// <summary>Creates a new ImplicitStackAllocArrayCreationExpressionSyntax instance.</summary>
        public static ImplicitStackAllocArrayCreationExpressionSyntax ImplicitStackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword.Kind() != SyntaxKind.StackAllocKeyword) throw new ArgumentException(nameof(stackAllocKeyword));
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            if (initializer == null) throw new ArgumentNullException(nameof(initializer));
            return (ImplicitStackAllocArrayCreationExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ImplicitStackAllocArrayCreationExpression((Syntax.InternalSyntax.SyntaxToken)stackAllocKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!, (Syntax.InternalSyntax.InitializerExpressionSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new ImplicitStackAllocArrayCreationExpressionSyntax instance.</summary>
        public static ImplicitStackAllocArrayCreationExpressionSyntax ImplicitStackAllocArrayCreationExpression(InitializerExpressionSyntax initializer)
            => SyntaxFactory.ImplicitStackAllocArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.StackAllocKeyword), SyntaxFactory.Token(SyntaxKind.OpenBracketToken), SyntaxFactory.Token(SyntaxKind.CloseBracketToken), initializer);

        /// <summary>Creates a new QueryExpressionSyntax instance.</summary>
        public static QueryExpressionSyntax QueryExpression(FromClauseSyntax fromClause, QueryBodySyntax body)
        {
            if (fromClause == null) throw new ArgumentNullException(nameof(fromClause));
            if (body == null) throw new ArgumentNullException(nameof(body));
            return (QueryExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.QueryExpression((Syntax.InternalSyntax.FromClauseSyntax)fromClause.Green, (Syntax.InternalSyntax.QueryBodySyntax)body.Green).CreateRed();
        }

        /// <summary>Creates a new QueryBodySyntax instance.</summary>
        public static QueryBodySyntax QueryBody(SyntaxList<QueryClauseSyntax> clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax? continuation)
        {
            if (selectOrGroup == null) throw new ArgumentNullException(nameof(selectOrGroup));
            return (QueryBodySyntax)Syntax.InternalSyntax.SyntaxFactory.QueryBody(clauses.Node.ToGreenList<Syntax.InternalSyntax.QueryClauseSyntax>(), (Syntax.InternalSyntax.SelectOrGroupClauseSyntax)selectOrGroup.Green, continuation == null ? null : (Syntax.InternalSyntax.QueryContinuationSyntax)continuation.Green).CreateRed();
        }

        /// <summary>Creates a new QueryBodySyntax instance.</summary>
        public static QueryBodySyntax QueryBody(SelectOrGroupClauseSyntax selectOrGroup)
            => SyntaxFactory.QueryBody(default, selectOrGroup, default);

        /// <summary>Creates a new FromClauseSyntax instance.</summary>
        public static FromClauseSyntax FromClause(SyntaxToken fromKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression)
        {
            if (fromKeyword.Kind() != SyntaxKind.FromKeyword) throw new ArgumentException(nameof(fromKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (inKeyword.Kind() != SyntaxKind.InKeyword) throw new ArgumentException(nameof(inKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (FromClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.FromClause((Syntax.InternalSyntax.SyntaxToken)fromKeyword.Node!, type == null ? null : (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)inKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new FromClauseSyntax instance.</summary>
        public static FromClauseSyntax FromClause(TypeSyntax? type, SyntaxToken identifier, ExpressionSyntax expression)
            => SyntaxFactory.FromClause(SyntaxFactory.Token(SyntaxKind.FromKeyword), type, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), expression);

        /// <summary>Creates a new FromClauseSyntax instance.</summary>
        public static FromClauseSyntax FromClause(SyntaxToken identifier, ExpressionSyntax expression)
            => SyntaxFactory.FromClause(SyntaxFactory.Token(SyntaxKind.FromKeyword), default, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), expression);

        /// <summary>Creates a new FromClauseSyntax instance.</summary>
        public static FromClauseSyntax FromClause(string identifier, ExpressionSyntax expression)
            => SyntaxFactory.FromClause(SyntaxFactory.Token(SyntaxKind.FromKeyword), default, SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.InKeyword), expression);

        /// <summary>Creates a new LetClauseSyntax instance.</summary>
        public static LetClauseSyntax LetClause(SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            if (letKeyword.Kind() != SyntaxKind.LetKeyword) throw new ArgumentException(nameof(letKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (LetClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.LetClause((Syntax.InternalSyntax.SyntaxToken)letKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new LetClauseSyntax instance.</summary>
        public static LetClauseSyntax LetClause(SyntaxToken identifier, ExpressionSyntax expression)
            => SyntaxFactory.LetClause(SyntaxFactory.Token(SyntaxKind.LetKeyword), identifier, SyntaxFactory.Token(SyntaxKind.EqualsToken), expression);

        /// <summary>Creates a new LetClauseSyntax instance.</summary>
        public static LetClauseSyntax LetClause(string identifier, ExpressionSyntax expression)
            => SyntaxFactory.LetClause(SyntaxFactory.Token(SyntaxKind.LetKeyword), SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.EqualsToken), expression);

        /// <summary>Creates a new JoinClauseSyntax instance.</summary>
        public static JoinClauseSyntax JoinClause(SyntaxToken joinKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax inExpression, SyntaxToken onKeyword, ExpressionSyntax leftExpression, SyntaxToken equalsKeyword, ExpressionSyntax rightExpression, JoinIntoClauseSyntax? into)
        {
            if (joinKeyword.Kind() != SyntaxKind.JoinKeyword) throw new ArgumentException(nameof(joinKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (inKeyword.Kind() != SyntaxKind.InKeyword) throw new ArgumentException(nameof(inKeyword));
            if (inExpression == null) throw new ArgumentNullException(nameof(inExpression));
            if (onKeyword.Kind() != SyntaxKind.OnKeyword) throw new ArgumentException(nameof(onKeyword));
            if (leftExpression == null) throw new ArgumentNullException(nameof(leftExpression));
            if (equalsKeyword.Kind() != SyntaxKind.EqualsKeyword) throw new ArgumentException(nameof(equalsKeyword));
            if (rightExpression == null) throw new ArgumentNullException(nameof(rightExpression));
            return (JoinClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.JoinClause((Syntax.InternalSyntax.SyntaxToken)joinKeyword.Node!, type == null ? null : (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)inKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)inExpression.Green, (Syntax.InternalSyntax.SyntaxToken)onKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)leftExpression.Green, (Syntax.InternalSyntax.SyntaxToken)equalsKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)rightExpression.Green, into == null ? null : (Syntax.InternalSyntax.JoinIntoClauseSyntax)into.Green).CreateRed();
        }

        /// <summary>Creates a new JoinClauseSyntax instance.</summary>
        public static JoinClauseSyntax JoinClause(TypeSyntax? type, SyntaxToken identifier, ExpressionSyntax inExpression, ExpressionSyntax leftExpression, ExpressionSyntax rightExpression, JoinIntoClauseSyntax? into)
            => SyntaxFactory.JoinClause(SyntaxFactory.Token(SyntaxKind.JoinKeyword), type, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), inExpression, SyntaxFactory.Token(SyntaxKind.OnKeyword), leftExpression, SyntaxFactory.Token(SyntaxKind.EqualsKeyword), rightExpression, into);

        /// <summary>Creates a new JoinClauseSyntax instance.</summary>
        public static JoinClauseSyntax JoinClause(SyntaxToken identifier, ExpressionSyntax inExpression, ExpressionSyntax leftExpression, ExpressionSyntax rightExpression)
            => SyntaxFactory.JoinClause(SyntaxFactory.Token(SyntaxKind.JoinKeyword), default, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), inExpression, SyntaxFactory.Token(SyntaxKind.OnKeyword), leftExpression, SyntaxFactory.Token(SyntaxKind.EqualsKeyword), rightExpression, default);

        /// <summary>Creates a new JoinClauseSyntax instance.</summary>
        public static JoinClauseSyntax JoinClause(string identifier, ExpressionSyntax inExpression, ExpressionSyntax leftExpression, ExpressionSyntax rightExpression)
            => SyntaxFactory.JoinClause(SyntaxFactory.Token(SyntaxKind.JoinKeyword), default, SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.InKeyword), inExpression, SyntaxFactory.Token(SyntaxKind.OnKeyword), leftExpression, SyntaxFactory.Token(SyntaxKind.EqualsKeyword), rightExpression, default);

        /// <summary>Creates a new JoinIntoClauseSyntax instance.</summary>
        public static JoinIntoClauseSyntax JoinIntoClause(SyntaxToken intoKeyword, SyntaxToken identifier)
        {
            if (intoKeyword.Kind() != SyntaxKind.IntoKeyword) throw new ArgumentException(nameof(intoKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            return (JoinIntoClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.JoinIntoClause((Syntax.InternalSyntax.SyntaxToken)intoKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!).CreateRed();
        }

        /// <summary>Creates a new JoinIntoClauseSyntax instance.</summary>
        public static JoinIntoClauseSyntax JoinIntoClause(SyntaxToken identifier)
            => SyntaxFactory.JoinIntoClause(SyntaxFactory.Token(SyntaxKind.IntoKeyword), identifier);

        /// <summary>Creates a new JoinIntoClauseSyntax instance.</summary>
        public static JoinIntoClauseSyntax JoinIntoClause(string identifier)
            => SyntaxFactory.JoinIntoClause(SyntaxFactory.Token(SyntaxKind.IntoKeyword), SyntaxFactory.Identifier(identifier));

        /// <summary>Creates a new WhereClauseSyntax instance.</summary>
        public static WhereClauseSyntax WhereClause(SyntaxToken whereKeyword, ExpressionSyntax condition)
        {
            if (whereKeyword.Kind() != SyntaxKind.WhereKeyword) throw new ArgumentException(nameof(whereKeyword));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return (WhereClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.WhereClause((Syntax.InternalSyntax.SyntaxToken)whereKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green).CreateRed();
        }

        /// <summary>Creates a new WhereClauseSyntax instance.</summary>
        public static WhereClauseSyntax WhereClause(ExpressionSyntax condition)
            => SyntaxFactory.WhereClause(SyntaxFactory.Token(SyntaxKind.WhereKeyword), condition);

        /// <summary>Creates a new OrderByClauseSyntax instance.</summary>
        public static OrderByClauseSyntax OrderByClause(SyntaxToken orderByKeyword, SeparatedSyntaxList<OrderingSyntax> orderings)
        {
            if (orderByKeyword.Kind() != SyntaxKind.OrderByKeyword) throw new ArgumentException(nameof(orderByKeyword));
            return (OrderByClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.OrderByClause((Syntax.InternalSyntax.SyntaxToken)orderByKeyword.Node!, orderings.Node.ToGreenSeparatedList<Syntax.InternalSyntax.OrderingSyntax>()).CreateRed();
        }

        /// <summary>Creates a new OrderByClauseSyntax instance.</summary>
        public static OrderByClauseSyntax OrderByClause(SeparatedSyntaxList<OrderingSyntax> orderings = default)
            => SyntaxFactory.OrderByClause(SyntaxFactory.Token(SyntaxKind.OrderByKeyword), orderings);

        /// <summary>Creates a new OrderingSyntax instance.</summary>
        public static OrderingSyntax Ordering(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken ascendingOrDescendingKeyword)
        {
            switch (kind)
            {
                case SyntaxKind.AscendingOrdering:
                case SyntaxKind.DescendingOrdering: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            switch (ascendingOrDescendingKeyword.Kind())
            {
                case SyntaxKind.AscendingKeyword:
                case SyntaxKind.DescendingKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(ascendingOrDescendingKeyword));
            }
            return (OrderingSyntax)Syntax.InternalSyntax.SyntaxFactory.Ordering(kind, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken?)ascendingOrDescendingKeyword.Node).CreateRed();
        }

        /// <summary>Creates a new OrderingSyntax instance.</summary>
        public static OrderingSyntax Ordering(SyntaxKind kind, ExpressionSyntax expression)
            => SyntaxFactory.Ordering(kind, expression, default);

        private static SyntaxKind GetOrderingAscendingOrDescendingKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.AscendingOrdering => SyntaxKind.AscendingKeyword,
                SyntaxKind.DescendingOrdering => SyntaxKind.DescendingKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new SelectClauseSyntax instance.</summary>
        public static SelectClauseSyntax SelectClause(SyntaxToken selectKeyword, ExpressionSyntax expression)
        {
            if (selectKeyword.Kind() != SyntaxKind.SelectKeyword) throw new ArgumentException(nameof(selectKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (SelectClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.SelectClause((Syntax.InternalSyntax.SyntaxToken)selectKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new SelectClauseSyntax instance.</summary>
        public static SelectClauseSyntax SelectClause(ExpressionSyntax expression)
            => SyntaxFactory.SelectClause(SyntaxFactory.Token(SyntaxKind.SelectKeyword), expression);

        /// <summary>Creates a new GroupClauseSyntax instance.</summary>
        public static GroupClauseSyntax GroupClause(SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression)
        {
            if (groupKeyword.Kind() != SyntaxKind.GroupKeyword) throw new ArgumentException(nameof(groupKeyword));
            if (groupExpression == null) throw new ArgumentNullException(nameof(groupExpression));
            if (byKeyword.Kind() != SyntaxKind.ByKeyword) throw new ArgumentException(nameof(byKeyword));
            if (byExpression == null) throw new ArgumentNullException(nameof(byExpression));
            return (GroupClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.GroupClause((Syntax.InternalSyntax.SyntaxToken)groupKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)groupExpression.Green, (Syntax.InternalSyntax.SyntaxToken)byKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)byExpression.Green).CreateRed();
        }

        /// <summary>Creates a new GroupClauseSyntax instance.</summary>
        public static GroupClauseSyntax GroupClause(ExpressionSyntax groupExpression, ExpressionSyntax byExpression)
            => SyntaxFactory.GroupClause(SyntaxFactory.Token(SyntaxKind.GroupKeyword), groupExpression, SyntaxFactory.Token(SyntaxKind.ByKeyword), byExpression);

        /// <summary>Creates a new QueryContinuationSyntax instance.</summary>
        public static QueryContinuationSyntax QueryContinuation(SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body)
        {
            if (intoKeyword.Kind() != SyntaxKind.IntoKeyword) throw new ArgumentException(nameof(intoKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (body == null) throw new ArgumentNullException(nameof(body));
            return (QueryContinuationSyntax)Syntax.InternalSyntax.SyntaxFactory.QueryContinuation((Syntax.InternalSyntax.SyntaxToken)intoKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.QueryBodySyntax)body.Green).CreateRed();
        }

        /// <summary>Creates a new QueryContinuationSyntax instance.</summary>
        public static QueryContinuationSyntax QueryContinuation(SyntaxToken identifier, QueryBodySyntax body)
            => SyntaxFactory.QueryContinuation(SyntaxFactory.Token(SyntaxKind.IntoKeyword), identifier, body);

        /// <summary>Creates a new QueryContinuationSyntax instance.</summary>
        public static QueryContinuationSyntax QueryContinuation(string identifier, QueryBodySyntax body)
            => SyntaxFactory.QueryContinuation(SyntaxFactory.Token(SyntaxKind.IntoKeyword), SyntaxFactory.Identifier(identifier), body);

        /// <summary>Creates a new OmittedArraySizeExpressionSyntax instance.</summary>
        public static OmittedArraySizeExpressionSyntax OmittedArraySizeExpression(SyntaxToken omittedArraySizeExpressionToken)
        {
            if (omittedArraySizeExpressionToken.Kind() != SyntaxKind.OmittedArraySizeExpressionToken) throw new ArgumentException(nameof(omittedArraySizeExpressionToken));
            return (OmittedArraySizeExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.OmittedArraySizeExpression((Syntax.InternalSyntax.SyntaxToken)omittedArraySizeExpressionToken.Node!).CreateRed();
        }

        /// <summary>Creates a new OmittedArraySizeExpressionSyntax instance.</summary>
        public static OmittedArraySizeExpressionSyntax OmittedArraySizeExpression()
            => SyntaxFactory.OmittedArraySizeExpression(SyntaxFactory.Token(SyntaxKind.OmittedArraySizeExpressionToken));

        /// <summary>Creates a new InterpolatedStringExpressionSyntax instance.</summary>
        public static InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken, SyntaxList<InterpolatedStringContentSyntax> contents, SyntaxToken stringEndToken)
        {
            switch (stringStartToken.Kind())
            {
                case SyntaxKind.InterpolatedStringStartToken:
                case SyntaxKind.InterpolatedVerbatimStringStartToken: break;
                default: throw new ArgumentException(nameof(stringStartToken));
            }
            if (stringEndToken.Kind() != SyntaxKind.InterpolatedStringEndToken) throw new ArgumentException(nameof(stringEndToken));
            return (InterpolatedStringExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringExpression((Syntax.InternalSyntax.SyntaxToken)stringStartToken.Node!, contents.Node.ToGreenList<Syntax.InternalSyntax.InterpolatedStringContentSyntax>(), (Syntax.InternalSyntax.SyntaxToken)stringEndToken.Node!).CreateRed();
        }

        /// <summary>Creates a new InterpolatedStringExpressionSyntax instance.</summary>
        public static InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken, SyntaxList<InterpolatedStringContentSyntax> contents)
            => SyntaxFactory.InterpolatedStringExpression(stringStartToken, contents, SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken));

        /// <summary>Creates a new InterpolatedStringExpressionSyntax instance.</summary>
        public static InterpolatedStringExpressionSyntax InterpolatedStringExpression(SyntaxToken stringStartToken)
            => SyntaxFactory.InterpolatedStringExpression(stringStartToken, default, SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken));

        /// <summary>Creates a new IsPatternExpressionSyntax instance.</summary>
        public static IsPatternExpressionSyntax IsPatternExpression(ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (isKeyword.Kind() != SyntaxKind.IsKeyword) throw new ArgumentException(nameof(isKeyword));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            return (IsPatternExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.IsPatternExpression((Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)isKeyword.Node!, (Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        /// <summary>Creates a new IsPatternExpressionSyntax instance.</summary>
        public static IsPatternExpressionSyntax IsPatternExpression(ExpressionSyntax expression, PatternSyntax pattern)
            => SyntaxFactory.IsPatternExpression(expression, SyntaxFactory.Token(SyntaxKind.IsKeyword), pattern);

        /// <summary>Creates a new ThrowExpressionSyntax instance.</summary>
        public static ThrowExpressionSyntax ThrowExpression(SyntaxToken throwKeyword, ExpressionSyntax expression)
        {
            if (throwKeyword.Kind() != SyntaxKind.ThrowKeyword) throw new ArgumentException(nameof(throwKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (ThrowExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.ThrowExpression((Syntax.InternalSyntax.SyntaxToken)throwKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new ThrowExpressionSyntax instance.</summary>
        public static ThrowExpressionSyntax ThrowExpression(ExpressionSyntax expression)
            => SyntaxFactory.ThrowExpression(SyntaxFactory.Token(SyntaxKind.ThrowKeyword), expression);

        /// <summary>Creates a new WhenClauseSyntax instance.</summary>
        public static WhenClauseSyntax WhenClause(SyntaxToken whenKeyword, ExpressionSyntax condition)
        {
            if (whenKeyword.Kind() != SyntaxKind.WhenKeyword) throw new ArgumentException(nameof(whenKeyword));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return (WhenClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.WhenClause((Syntax.InternalSyntax.SyntaxToken)whenKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green).CreateRed();
        }

        /// <summary>Creates a new WhenClauseSyntax instance.</summary>
        public static WhenClauseSyntax WhenClause(ExpressionSyntax condition)
            => SyntaxFactory.WhenClause(SyntaxFactory.Token(SyntaxKind.WhenKeyword), condition);

        /// <summary>Creates a new DiscardPatternSyntax instance.</summary>
        public static DiscardPatternSyntax DiscardPattern(SyntaxToken underscoreToken)
        {
            if (underscoreToken.Kind() != SyntaxKind.UnderscoreToken) throw new ArgumentException(nameof(underscoreToken));
            return (DiscardPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.DiscardPattern((Syntax.InternalSyntax.SyntaxToken)underscoreToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DiscardPatternSyntax instance.</summary>
        public static DiscardPatternSyntax DiscardPattern()
            => SyntaxFactory.DiscardPattern(SyntaxFactory.Token(SyntaxKind.UnderscoreToken));

        /// <summary>Creates a new DeclarationPatternSyntax instance.</summary>
        public static DeclarationPatternSyntax DeclarationPattern(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (designation == null) throw new ArgumentNullException(nameof(designation));
            return (DeclarationPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.DeclarationPattern((Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        /// <summary>Creates a new VarPatternSyntax instance.</summary>
        public static VarPatternSyntax VarPattern(SyntaxToken varKeyword, VariableDesignationSyntax designation)
        {
            if (varKeyword.Kind() != SyntaxKind.VarKeyword) throw new ArgumentException(nameof(varKeyword));
            if (designation == null) throw new ArgumentNullException(nameof(designation));
            return (VarPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.VarPattern((Syntax.InternalSyntax.SyntaxToken)varKeyword.Node!, (Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        /// <summary>Creates a new VarPatternSyntax instance.</summary>
        public static VarPatternSyntax VarPattern(VariableDesignationSyntax designation)
            => SyntaxFactory.VarPattern(SyntaxFactory.Token(SyntaxKind.VarKeyword), designation);

        /// <summary>Creates a new RecursivePatternSyntax instance.</summary>
        public static RecursivePatternSyntax RecursivePattern(TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation)
        {
            return (RecursivePatternSyntax)Syntax.InternalSyntax.SyntaxFactory.RecursivePattern(type == null ? null : (Syntax.InternalSyntax.TypeSyntax)type.Green, positionalPatternClause == null ? null : (Syntax.InternalSyntax.PositionalPatternClauseSyntax)positionalPatternClause.Green, propertyPatternClause == null ? null : (Syntax.InternalSyntax.PropertyPatternClauseSyntax)propertyPatternClause.Green, designation == null ? null : (Syntax.InternalSyntax.VariableDesignationSyntax)designation.Green).CreateRed();
        }

        /// <summary>Creates a new RecursivePatternSyntax instance.</summary>
        public static RecursivePatternSyntax RecursivePattern()
            => SyntaxFactory.RecursivePattern(default, default, default, default);

        /// <summary>Creates a new PositionalPatternClauseSyntax instance.</summary>
        public static PositionalPatternClauseSyntax PositionalPatternClause(SyntaxToken openParenToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (PositionalPatternClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.PositionalPatternClause((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, subpatterns.Node.ToGreenSeparatedList<Syntax.InternalSyntax.SubpatternSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new PositionalPatternClauseSyntax instance.</summary>
        public static PositionalPatternClauseSyntax PositionalPatternClause(SeparatedSyntaxList<SubpatternSyntax> subpatterns = default)
            => SyntaxFactory.PositionalPatternClause(SyntaxFactory.Token(SyntaxKind.OpenParenToken), subpatterns, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new PropertyPatternClauseSyntax instance.</summary>
        public static PropertyPatternClauseSyntax PropertyPatternClause(SyntaxToken openBraceToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (PropertyPatternClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.PropertyPatternClause((Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, subpatterns.Node.ToGreenSeparatedList<Syntax.InternalSyntax.SubpatternSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new PropertyPatternClauseSyntax instance.</summary>
        public static PropertyPatternClauseSyntax PropertyPatternClause(SeparatedSyntaxList<SubpatternSyntax> subpatterns = default)
            => SyntaxFactory.PropertyPatternClause(SyntaxFactory.Token(SyntaxKind.OpenBraceToken), subpatterns, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new SubpatternSyntax instance.</summary>
        public static SubpatternSyntax Subpattern(NameColonSyntax? nameColon, PatternSyntax pattern)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            return (SubpatternSyntax)Syntax.InternalSyntax.SyntaxFactory.Subpattern(nameColon == null ? null : (Syntax.InternalSyntax.NameColonSyntax)nameColon.Green, (Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        /// <summary>Creates a new SubpatternSyntax instance.</summary>
        public static SubpatternSyntax Subpattern(PatternSyntax pattern)
            => SyntaxFactory.Subpattern(default, pattern);

        /// <summary>Creates a new ConstantPatternSyntax instance.</summary>
        public static ConstantPatternSyntax ConstantPattern(ExpressionSyntax expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (ConstantPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.ConstantPattern((Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new ParenthesizedPatternSyntax instance.</summary>
        public static ParenthesizedPatternSyntax ParenthesizedPattern(SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ParenthesizedPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.ParenthesizedPattern((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.PatternSyntax)pattern.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ParenthesizedPatternSyntax instance.</summary>
        public static ParenthesizedPatternSyntax ParenthesizedPattern(PatternSyntax pattern)
            => SyntaxFactory.ParenthesizedPattern(SyntaxFactory.Token(SyntaxKind.OpenParenToken), pattern, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new RelationalPatternSyntax instance.</summary>
        public static RelationalPatternSyntax RelationalPattern(SyntaxToken operatorToken, ExpressionSyntax expression)
        {
            switch (operatorToken.Kind())
            {
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (RelationalPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.RelationalPattern((Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new TypePatternSyntax instance.</summary>
        public static TypePatternSyntax TypePattern(TypeSyntax type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (TypePatternSyntax)Syntax.InternalSyntax.SyntaxFactory.TypePattern((Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new BinaryPatternSyntax instance.</summary>
        public static BinaryPatternSyntax BinaryPattern(SyntaxKind kind, PatternSyntax left, SyntaxToken operatorToken, PatternSyntax right)
        {
            switch (kind)
            {
                case SyntaxKind.OrPattern:
                case SyntaxKind.AndPattern: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (left == null) throw new ArgumentNullException(nameof(left));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.OrKeyword:
                case SyntaxKind.AndKeyword: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (right == null) throw new ArgumentNullException(nameof(right));
            return (BinaryPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.BinaryPattern(kind, (Syntax.InternalSyntax.PatternSyntax)left.Green, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.PatternSyntax)right.Green).CreateRed();
        }

        /// <summary>Creates a new BinaryPatternSyntax instance.</summary>
        public static BinaryPatternSyntax BinaryPattern(SyntaxKind kind, PatternSyntax left, PatternSyntax right)
            => SyntaxFactory.BinaryPattern(kind, left, SyntaxFactory.Token(GetBinaryPatternOperatorTokenKind(kind)), right);

        private static SyntaxKind GetBinaryPatternOperatorTokenKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.OrPattern => SyntaxKind.OrKeyword,
                SyntaxKind.AndPattern => SyntaxKind.AndKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new UnaryPatternSyntax instance.</summary>
        public static UnaryPatternSyntax UnaryPattern(SyntaxToken operatorToken, PatternSyntax pattern)
        {
            if (operatorToken.Kind() != SyntaxKind.NotKeyword) throw new ArgumentException(nameof(operatorToken));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            return (UnaryPatternSyntax)Syntax.InternalSyntax.SyntaxFactory.UnaryPattern((Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.PatternSyntax)pattern.Green).CreateRed();
        }

        /// <summary>Creates a new UnaryPatternSyntax instance.</summary>
        public static UnaryPatternSyntax UnaryPattern(PatternSyntax pattern)
            => SyntaxFactory.UnaryPattern(SyntaxFactory.Token(SyntaxKind.NotKeyword), pattern);

        /// <summary>Creates a new InterpolatedStringTextSyntax instance.</summary>
        public static InterpolatedStringTextSyntax InterpolatedStringText(SyntaxToken textToken)
        {
            if (textToken.Kind() != SyntaxKind.InterpolatedStringTextToken) throw new ArgumentException(nameof(textToken));
            return (InterpolatedStringTextSyntax)Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringText((Syntax.InternalSyntax.SyntaxToken)textToken.Node!).CreateRed();
        }

        /// <summary>Creates a new InterpolatedStringTextSyntax instance.</summary>
        public static InterpolatedStringTextSyntax InterpolatedStringText()
            => SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(SyntaxKind.InterpolatedStringTextToken));

        /// <summary>Creates a new InterpolationSyntax instance.</summary>
        public static InterpolationSyntax Interpolation(SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (InterpolationSyntax)Syntax.InternalSyntax.SyntaxFactory.Interpolation((Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, alignmentClause == null ? null : (Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)alignmentClause.Green, formatClause == null ? null : (Syntax.InternalSyntax.InterpolationFormatClauseSyntax)formatClause.Green, (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new InterpolationSyntax instance.</summary>
        public static InterpolationSyntax Interpolation(ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause)
            => SyntaxFactory.Interpolation(SyntaxFactory.Token(SyntaxKind.OpenBraceToken), expression, alignmentClause, formatClause, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new InterpolationSyntax instance.</summary>
        public static InterpolationSyntax Interpolation(ExpressionSyntax expression)
            => SyntaxFactory.Interpolation(SyntaxFactory.Token(SyntaxKind.OpenBraceToken), expression, default, default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new InterpolationAlignmentClauseSyntax instance.</summary>
        public static InterpolationAlignmentClauseSyntax InterpolationAlignmentClause(SyntaxToken commaToken, ExpressionSyntax value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (InterpolationAlignmentClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.InterpolationAlignmentClause((Syntax.InternalSyntax.SyntaxToken)commaToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)value.Green).CreateRed();
        }

        /// <summary>Creates a new InterpolationFormatClauseSyntax instance.</summary>
        public static InterpolationFormatClauseSyntax InterpolationFormatClause(SyntaxToken colonToken, SyntaxToken formatStringToken)
        {
            if (formatStringToken.Kind() != SyntaxKind.InterpolatedStringTextToken) throw new ArgumentException(nameof(formatStringToken));
            return (InterpolationFormatClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.InterpolationFormatClause((Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, (Syntax.InternalSyntax.SyntaxToken)formatStringToken.Node!).CreateRed();
        }

        /// <summary>Creates a new InterpolationFormatClauseSyntax instance.</summary>
        public static InterpolationFormatClauseSyntax InterpolationFormatClause(SyntaxToken colonToken)
            => SyntaxFactory.InterpolationFormatClause(colonToken, SyntaxFactory.Token(SyntaxKind.InterpolatedStringTextToken));

        /// <summary>Creates a new GlobalStatementSyntax instance.</summary>
        public static GlobalStatementSyntax GlobalStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, StatementSyntax statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (GlobalStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.GlobalStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new GlobalStatementSyntax instance.</summary>
        public static GlobalStatementSyntax GlobalStatement(StatementSyntax statement)
            => SyntaxFactory.GlobalStatement(default, default(SyntaxTokenList), statement);

        /// <summary>Creates a new BlockSyntax instance.</summary>
        public static BlockSyntax Block(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken openBraceToken, SyntaxList<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (BlockSyntax)Syntax.InternalSyntax.SyntaxFactory.Block(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, statements.Node.ToGreenList<Syntax.InternalSyntax.StatementSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new BlockSyntax instance.</summary>
        public static BlockSyntax Block(SyntaxList<AttributeListSyntax> attributeLists, SyntaxList<StatementSyntax> statements)
            => SyntaxFactory.Block(attributeLists, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), statements, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

#pragma warning disable RS0027
        /// <summary>Creates a new BlockSyntax instance.</summary>
        public static BlockSyntax Block(SyntaxList<StatementSyntax> statements = default)
            => SyntaxFactory.Block(default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), statements, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
#pragma warning restore RS0027

        /// <summary>Creates a new LocalFunctionStatementSyntax instance.</summary>
        public static LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (LocalFunctionStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.LocalFunctionStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new LocalFunctionStatementSyntax instance.</summary>
        public static LocalFunctionStatementSyntax LocalFunctionStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.LocalFunctionStatement(attributeLists, modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, default);

        /// <summary>Creates a new LocalFunctionStatementSyntax instance.</summary>
        public static LocalFunctionStatementSyntax LocalFunctionStatement(TypeSyntax returnType, SyntaxToken identifier)
            => SyntaxFactory.LocalFunctionStatement(default, default(SyntaxTokenList), returnType, identifier, default, SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new LocalFunctionStatementSyntax instance.</summary>
        public static LocalFunctionStatementSyntax LocalFunctionStatement(TypeSyntax returnType, string identifier)
            => SyntaxFactory.LocalFunctionStatement(default, default(SyntaxTokenList), returnType, SyntaxFactory.Identifier(identifier), default, SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new LocalDeclarationStatementSyntax instance.</summary>
        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            switch (awaitKeyword.Kind())
            {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(awaitKeyword));
            }
            switch (usingKeyword.Kind())
            {
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(usingKeyword));
            }
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (LocalDeclarationStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.LocalDeclarationStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)awaitKeyword.Node, (Syntax.InternalSyntax.SyntaxToken?)usingKeyword.Node, modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new LocalDeclarationStatementSyntax instance.</summary>
        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration)
            => SyntaxFactory.LocalDeclarationStatement(attributeLists, default, default, modifiers, declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new LocalDeclarationStatementSyntax instance.</summary>
        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(VariableDeclarationSyntax declaration)
            => SyntaxFactory.LocalDeclarationStatement(default, default, default, default(SyntaxTokenList), declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new VariableDeclarationSyntax instance.</summary>
        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (VariableDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.VariableDeclaration((Syntax.InternalSyntax.TypeSyntax)type.Green, variables.Node.ToGreenSeparatedList<Syntax.InternalSyntax.VariableDeclaratorSyntax>()).CreateRed();
        }

        /// <summary>Creates a new VariableDeclarationSyntax instance.</summary>
        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type)
            => SyntaxFactory.VariableDeclaration(type, default);

        /// <summary>Creates a new VariableDeclaratorSyntax instance.</summary>
        public static VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            return (VariableDeclaratorSyntax)Syntax.InternalSyntax.SyntaxFactory.VariableDeclarator((Syntax.InternalSyntax.SyntaxToken)identifier.Node!, argumentList == null ? null : (Syntax.InternalSyntax.BracketedArgumentListSyntax)argumentList.Green, initializer == null ? null : (Syntax.InternalSyntax.EqualsValueClauseSyntax)initializer.Green).CreateRed();
        }

        /// <summary>Creates a new VariableDeclaratorSyntax instance.</summary>
        public static VariableDeclaratorSyntax VariableDeclarator(SyntaxToken identifier)
            => SyntaxFactory.VariableDeclarator(identifier, default, default);

        /// <summary>Creates a new VariableDeclaratorSyntax instance.</summary>
        public static VariableDeclaratorSyntax VariableDeclarator(string identifier)
            => SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(identifier), default, default);

        /// <summary>Creates a new EqualsValueClauseSyntax instance.</summary>
        public static EqualsValueClauseSyntax EqualsValueClause(SyntaxToken equalsToken, ExpressionSyntax value)
        {
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (EqualsValueClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.EqualsValueClause((Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)value.Green).CreateRed();
        }

        /// <summary>Creates a new EqualsValueClauseSyntax instance.</summary>
        public static EqualsValueClauseSyntax EqualsValueClause(ExpressionSyntax value)
            => SyntaxFactory.EqualsValueClause(SyntaxFactory.Token(SyntaxKind.EqualsToken), value);

        /// <summary>Creates a new SingleVariableDesignationSyntax instance.</summary>
        public static SingleVariableDesignationSyntax SingleVariableDesignation(SyntaxToken identifier)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            return (SingleVariableDesignationSyntax)Syntax.InternalSyntax.SyntaxFactory.SingleVariableDesignation((Syntax.InternalSyntax.SyntaxToken)identifier.Node!).CreateRed();
        }

        /// <summary>Creates a new DiscardDesignationSyntax instance.</summary>
        public static DiscardDesignationSyntax DiscardDesignation(SyntaxToken underscoreToken)
        {
            if (underscoreToken.Kind() != SyntaxKind.UnderscoreToken) throw new ArgumentException(nameof(underscoreToken));
            return (DiscardDesignationSyntax)Syntax.InternalSyntax.SyntaxFactory.DiscardDesignation((Syntax.InternalSyntax.SyntaxToken)underscoreToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DiscardDesignationSyntax instance.</summary>
        public static DiscardDesignationSyntax DiscardDesignation()
            => SyntaxFactory.DiscardDesignation(SyntaxFactory.Token(SyntaxKind.UnderscoreToken));

        /// <summary>Creates a new ParenthesizedVariableDesignationSyntax instance.</summary>
        public static ParenthesizedVariableDesignationSyntax ParenthesizedVariableDesignation(SyntaxToken openParenToken, SeparatedSyntaxList<VariableDesignationSyntax> variables, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ParenthesizedVariableDesignationSyntax)Syntax.InternalSyntax.SyntaxFactory.ParenthesizedVariableDesignation((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, variables.Node.ToGreenSeparatedList<Syntax.InternalSyntax.VariableDesignationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ParenthesizedVariableDesignationSyntax instance.</summary>
        public static ParenthesizedVariableDesignationSyntax ParenthesizedVariableDesignation(SeparatedSyntaxList<VariableDesignationSyntax> variables = default)
            => SyntaxFactory.ParenthesizedVariableDesignation(SyntaxFactory.Token(SyntaxKind.OpenParenToken), variables, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new ExpressionStatementSyntax instance.</summary>
        public static ExpressionStatementSyntax ExpressionStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (ExpressionStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ExpressionStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ExpressionStatementSyntax instance.</summary>
        public static ExpressionStatementSyntax ExpressionStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression)
            => SyntaxFactory.ExpressionStatement(attributeLists, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ExpressionStatementSyntax instance.</summary>
        public static ExpressionStatementSyntax ExpressionStatement(ExpressionSyntax expression)
            => SyntaxFactory.ExpressionStatement(default, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new EmptyStatementSyntax instance.</summary>
        public static EmptyStatementSyntax EmptyStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken semicolonToken)
        {
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (EmptyStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new EmptyStatementSyntax instance.</summary>
        public static EmptyStatementSyntax EmptyStatement(SyntaxList<AttributeListSyntax> attributeLists)
            => SyntaxFactory.EmptyStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new EmptyStatementSyntax instance.</summary>
        public static EmptyStatementSyntax EmptyStatement()
            => SyntaxFactory.EmptyStatement(default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new LabeledStatementSyntax instance.</summary>
        public static LabeledStatementSyntax LabeledStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, SyntaxToken colonToken, StatementSyntax statement)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (LabeledStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.LabeledStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new LabeledStatementSyntax instance.</summary>
        public static LabeledStatementSyntax LabeledStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, StatementSyntax statement)
            => SyntaxFactory.LabeledStatement(attributeLists, identifier, SyntaxFactory.Token(SyntaxKind.ColonToken), statement);

        /// <summary>Creates a new LabeledStatementSyntax instance.</summary>
        public static LabeledStatementSyntax LabeledStatement(SyntaxToken identifier, StatementSyntax statement)
            => SyntaxFactory.LabeledStatement(default, identifier, SyntaxFactory.Token(SyntaxKind.ColonToken), statement);

        /// <summary>Creates a new LabeledStatementSyntax instance.</summary>
        public static LabeledStatementSyntax LabeledStatement(string identifier, StatementSyntax statement)
            => SyntaxFactory.LabeledStatement(default, SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.ColonToken), statement);

        /// <summary>Creates a new GotoStatementSyntax instance.</summary>
        public static GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            switch (kind)
            {
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (gotoKeyword.Kind() != SyntaxKind.GotoKeyword) throw new ArgumentException(nameof(gotoKeyword));
            switch (caseOrDefaultKeyword.Kind())
            {
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(caseOrDefaultKeyword));
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (GotoStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.GotoStatement(kind, attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)gotoKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)caseOrDefaultKeyword.Node, expression == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new GotoStatementSyntax instance.</summary>
        public static GotoStatementSyntax GotoStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax? expression)
            => SyntaxFactory.GotoStatement(kind, attributeLists, SyntaxFactory.Token(SyntaxKind.GotoKeyword), caseOrDefaultKeyword, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

#pragma warning disable RS0027
        /// <summary>Creates a new GotoStatementSyntax instance.</summary>
        public static GotoStatementSyntax GotoStatement(SyntaxKind kind, ExpressionSyntax? expression = default)
            => SyntaxFactory.GotoStatement(kind, default, SyntaxFactory.Token(SyntaxKind.GotoKeyword), default, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
#pragma warning restore RS0027

        /// <summary>Creates a new BreakStatementSyntax instance.</summary>
        public static BreakStatementSyntax BreakStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            if (breakKeyword.Kind() != SyntaxKind.BreakKeyword) throw new ArgumentException(nameof(breakKeyword));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (BreakStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.BreakStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)breakKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new BreakStatementSyntax instance.</summary>
        public static BreakStatementSyntax BreakStatement(SyntaxList<AttributeListSyntax> attributeLists)
            => SyntaxFactory.BreakStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.BreakKeyword), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new BreakStatementSyntax instance.</summary>
        public static BreakStatementSyntax BreakStatement()
            => SyntaxFactory.BreakStatement(default, SyntaxFactory.Token(SyntaxKind.BreakKeyword), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ContinueStatementSyntax instance.</summary>
        public static ContinueStatementSyntax ContinueStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            if (continueKeyword.Kind() != SyntaxKind.ContinueKeyword) throw new ArgumentException(nameof(continueKeyword));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (ContinueStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ContinueStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)continueKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ContinueStatementSyntax instance.</summary>
        public static ContinueStatementSyntax ContinueStatement(SyntaxList<AttributeListSyntax> attributeLists)
            => SyntaxFactory.ContinueStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.ContinueKeyword), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ContinueStatementSyntax instance.</summary>
        public static ContinueStatementSyntax ContinueStatement()
            => SyntaxFactory.ContinueStatement(default, SyntaxFactory.Token(SyntaxKind.ContinueKeyword), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ReturnStatementSyntax instance.</summary>
        public static ReturnStatementSyntax ReturnStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (returnKeyword.Kind() != SyntaxKind.ReturnKeyword) throw new ArgumentException(nameof(returnKeyword));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (ReturnStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ReturnStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)returnKeyword.Node!, expression == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ReturnStatementSyntax instance.</summary>
        public static ReturnStatementSyntax ReturnStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax? expression)
            => SyntaxFactory.ReturnStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.ReturnKeyword), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

#pragma warning disable RS0027
        /// <summary>Creates a new ReturnStatementSyntax instance.</summary>
        public static ReturnStatementSyntax ReturnStatement(ExpressionSyntax? expression = default)
            => SyntaxFactory.ReturnStatement(default, SyntaxFactory.Token(SyntaxKind.ReturnKeyword), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
#pragma warning restore RS0027

        /// <summary>Creates a new ThrowStatementSyntax instance.</summary>
        public static ThrowStatementSyntax ThrowStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (throwKeyword.Kind() != SyntaxKind.ThrowKeyword) throw new ArgumentException(nameof(throwKeyword));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (ThrowStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ThrowStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)throwKeyword.Node!, expression == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ThrowStatementSyntax instance.</summary>
        public static ThrowStatementSyntax ThrowStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax? expression)
            => SyntaxFactory.ThrowStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.ThrowKeyword), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

#pragma warning disable RS0027
        /// <summary>Creates a new ThrowStatementSyntax instance.</summary>
        public static ThrowStatementSyntax ThrowStatement(ExpressionSyntax? expression = default)
            => SyntaxFactory.ThrowStatement(default, SyntaxFactory.Token(SyntaxKind.ThrowKeyword), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
#pragma warning restore RS0027

        /// <summary>Creates a new YieldStatementSyntax instance.</summary>
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            switch (kind)
            {
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.YieldBreakStatement: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (yieldKeyword.Kind() != SyntaxKind.YieldKeyword) throw new ArgumentException(nameof(yieldKeyword));
            switch (returnOrBreakKeyword.Kind())
            {
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.BreakKeyword: break;
                default: throw new ArgumentException(nameof(returnOrBreakKeyword));
            }
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (YieldStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.YieldStatement(kind, attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)yieldKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)returnOrBreakKeyword.Node!, expression == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new YieldStatementSyntax instance.</summary>
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax? expression)
            => SyntaxFactory.YieldStatement(kind, attributeLists, SyntaxFactory.Token(SyntaxKind.YieldKeyword), SyntaxFactory.Token(GetYieldStatementReturnOrBreakKeywordKind(kind)), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

#pragma warning disable RS0027
        /// <summary>Creates a new YieldStatementSyntax instance.</summary>
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, ExpressionSyntax? expression = default)
            => SyntaxFactory.YieldStatement(kind, default, SyntaxFactory.Token(SyntaxKind.YieldKeyword), SyntaxFactory.Token(GetYieldStatementReturnOrBreakKeywordKind(kind)), expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
#pragma warning restore RS0027

        private static SyntaxKind GetYieldStatementReturnOrBreakKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.YieldReturnStatement => SyntaxKind.ReturnKeyword,
                SyntaxKind.YieldBreakStatement => SyntaxKind.BreakKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new WhileStatementSyntax instance.</summary>
        public static WhileStatementSyntax WhileStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (whileKeyword.Kind() != SyntaxKind.WhileKeyword) throw new ArgumentException(nameof(whileKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (WhileStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.WhileStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)whileKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new WhileStatementSyntax instance.</summary>
        public static WhileStatementSyntax WhileStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax condition, StatementSyntax statement)
            => SyntaxFactory.WhileStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.WhileKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new WhileStatementSyntax instance.</summary>
        public static WhileStatementSyntax WhileStatement(ExpressionSyntax condition, StatementSyntax statement)
            => SyntaxFactory.WhileStatement(default, SyntaxFactory.Token(SyntaxKind.WhileKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new DoStatementSyntax instance.</summary>
        public static DoStatementSyntax DoStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            if (doKeyword.Kind() != SyntaxKind.DoKeyword) throw new ArgumentException(nameof(doKeyword));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            if (whileKeyword.Kind() != SyntaxKind.WhileKeyword) throw new ArgumentException(nameof(whileKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (DoStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.DoStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)doKeyword.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green, (Syntax.InternalSyntax.SyntaxToken)whileKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DoStatementSyntax instance.</summary>
        public static DoStatementSyntax DoStatement(SyntaxList<AttributeListSyntax> attributeLists, StatementSyntax statement, ExpressionSyntax condition)
            => SyntaxFactory.DoStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.DoKeyword), statement, SyntaxFactory.Token(SyntaxKind.WhileKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new DoStatementSyntax instance.</summary>
        public static DoStatementSyntax DoStatement(StatementSyntax statement, ExpressionSyntax condition)
            => SyntaxFactory.DoStatement(default, SyntaxFactory.Token(SyntaxKind.DoKeyword), statement, SyntaxFactory.Token(SyntaxKind.WhileKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ForStatementSyntax instance.</summary>
        public static ForStatementSyntax ForStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<ExpressionSyntax> incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (forKeyword.Kind() != SyntaxKind.ForKeyword) throw new ArgumentException(nameof(forKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (firstSemicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(firstSemicolonToken));
            if (secondSemicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(secondSemicolonToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (ForStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ForStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)forKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, declaration == null ? null : (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, initializers.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ExpressionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)firstSemicolonToken.Node!, condition == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)secondSemicolonToken.Node!, incrementors.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ExpressionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new ForStatementSyntax instance.</summary>
        public static ForStatementSyntax ForStatement(SyntaxList<AttributeListSyntax> attributeLists, VariableDeclarationSyntax? declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, ExpressionSyntax? condition, SeparatedSyntaxList<ExpressionSyntax> incrementors, StatementSyntax statement)
            => SyntaxFactory.ForStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.ForKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), declaration, initializers, SyntaxFactory.Token(SyntaxKind.SemicolonToken), condition, SyntaxFactory.Token(SyntaxKind.SemicolonToken), incrementors, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForStatementSyntax instance.</summary>
        public static ForStatementSyntax ForStatement(StatementSyntax statement)
            => SyntaxFactory.ForStatement(default, SyntaxFactory.Token(SyntaxKind.ForKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), default, default, SyntaxFactory.Token(SyntaxKind.SemicolonToken), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken), default, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForEachStatementSyntax instance.</summary>
        public static ForEachStatementSyntax ForEachStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            switch (awaitKeyword.Kind())
            {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(awaitKeyword));
            }
            if (forEachKeyword.Kind() != SyntaxKind.ForEachKeyword) throw new ArgumentException(nameof(forEachKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (inKeyword.Kind() != SyntaxKind.InKeyword) throw new ArgumentException(nameof(inKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (ForEachStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ForEachStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)awaitKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)forEachKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)inKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new ForEachStatementSyntax instance.</summary>
        public static ForEachStatementSyntax ForEachStatement(SyntaxList<AttributeListSyntax> attributeLists, TypeSyntax type, SyntaxToken identifier, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.ForEachStatement(attributeLists, default, SyntaxFactory.Token(SyntaxKind.ForEachKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForEachStatementSyntax instance.</summary>
        public static ForEachStatementSyntax ForEachStatement(TypeSyntax type, SyntaxToken identifier, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.ForEachStatement(default, default, SyntaxFactory.Token(SyntaxKind.ForEachKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, identifier, SyntaxFactory.Token(SyntaxKind.InKeyword), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForEachStatementSyntax instance.</summary>
        public static ForEachStatementSyntax ForEachStatement(TypeSyntax type, string identifier, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.ForEachStatement(default, default, SyntaxFactory.Token(SyntaxKind.ForEachKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.InKeyword), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForEachVariableStatementSyntax instance.</summary>
        public static ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            switch (awaitKeyword.Kind())
            {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(awaitKeyword));
            }
            if (forEachKeyword.Kind() != SyntaxKind.ForEachKeyword) throw new ArgumentException(nameof(forEachKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            if (inKeyword.Kind() != SyntaxKind.InKeyword) throw new ArgumentException(nameof(inKeyword));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (ForEachVariableStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.ForEachVariableStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)awaitKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)forEachKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)variable.Green, (Syntax.InternalSyntax.SyntaxToken)inKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new ForEachVariableStatementSyntax instance.</summary>
        public static ForEachVariableStatementSyntax ForEachVariableStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax variable, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.ForEachVariableStatement(attributeLists, default, SyntaxFactory.Token(SyntaxKind.ForEachKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), variable, SyntaxFactory.Token(SyntaxKind.InKeyword), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new ForEachVariableStatementSyntax instance.</summary>
        public static ForEachVariableStatementSyntax ForEachVariableStatement(ExpressionSyntax variable, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.ForEachVariableStatement(default, default, SyntaxFactory.Token(SyntaxKind.ForEachKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), variable, SyntaxFactory.Token(SyntaxKind.InKeyword), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new UsingStatementSyntax instance.</summary>
        public static UsingStatementSyntax UsingStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            switch (awaitKeyword.Kind())
            {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(awaitKeyword));
            }
            if (usingKeyword.Kind() != SyntaxKind.UsingKeyword) throw new ArgumentException(nameof(usingKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (UsingStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.UsingStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)awaitKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)usingKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, declaration == null ? null : (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, expression == null ? null : (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new UsingStatementSyntax instance.</summary>
        public static UsingStatementSyntax UsingStatement(SyntaxList<AttributeListSyntax> attributeLists, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, StatementSyntax statement)
            => SyntaxFactory.UsingStatement(attributeLists, default, SyntaxFactory.Token(SyntaxKind.UsingKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), declaration, expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new UsingStatementSyntax instance.</summary>
        public static UsingStatementSyntax UsingStatement(StatementSyntax statement)
            => SyntaxFactory.UsingStatement(default, default, SyntaxFactory.Token(SyntaxKind.UsingKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), default, default, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new FixedStatementSyntax instance.</summary>
        public static FixedStatementSyntax FixedStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (fixedKeyword.Kind() != SyntaxKind.FixedKeyword) throw new ArgumentException(nameof(fixedKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (FixedStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.FixedStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)fixedKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new FixedStatementSyntax instance.</summary>
        public static FixedStatementSyntax FixedStatement(SyntaxList<AttributeListSyntax> attributeLists, VariableDeclarationSyntax declaration, StatementSyntax statement)
            => SyntaxFactory.FixedStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.FixedKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), declaration, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new FixedStatementSyntax instance.</summary>
        public static FixedStatementSyntax FixedStatement(VariableDeclarationSyntax declaration, StatementSyntax statement)
            => SyntaxFactory.FixedStatement(default, SyntaxFactory.Token(SyntaxKind.FixedKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), declaration, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new CheckedStatementSyntax instance.</summary>
        public static CheckedStatementSyntax CheckedStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken keyword, BlockSyntax block)
        {
            switch (kind)
            {
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (keyword.Kind())
            {
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword: break;
                default: throw new ArgumentException(nameof(keyword));
            }
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (CheckedStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.CheckedStatement(kind, attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        /// <summary>Creates a new CheckedStatementSyntax instance.</summary>
        public static CheckedStatementSyntax CheckedStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, BlockSyntax block)
            => SyntaxFactory.CheckedStatement(kind, attributeLists, SyntaxFactory.Token(GetCheckedStatementKeywordKind(kind)), block);

#pragma warning disable RS0027
        /// <summary>Creates a new CheckedStatementSyntax instance.</summary>
        public static CheckedStatementSyntax CheckedStatement(SyntaxKind kind, BlockSyntax? block = default)
            => SyntaxFactory.CheckedStatement(kind, default, SyntaxFactory.Token(GetCheckedStatementKeywordKind(kind)), block ?? SyntaxFactory.Block());
#pragma warning restore RS0027

        private static SyntaxKind GetCheckedStatementKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.CheckedStatement => SyntaxKind.CheckedKeyword,
                SyntaxKind.UncheckedStatement => SyntaxKind.UncheckedKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new UnsafeStatementSyntax instance.</summary>
        public static UnsafeStatementSyntax UnsafeStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken unsafeKeyword, BlockSyntax block)
        {
            if (unsafeKeyword.Kind() != SyntaxKind.UnsafeKeyword) throw new ArgumentException(nameof(unsafeKeyword));
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (UnsafeStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.UnsafeStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)unsafeKeyword.Node!, (Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        /// <summary>Creates a new UnsafeStatementSyntax instance.</summary>
        public static UnsafeStatementSyntax UnsafeStatement(SyntaxList<AttributeListSyntax> attributeLists, BlockSyntax block)
            => SyntaxFactory.UnsafeStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.UnsafeKeyword), block);

#pragma warning disable RS0027
        /// <summary>Creates a new UnsafeStatementSyntax instance.</summary>
        public static UnsafeStatementSyntax UnsafeStatement(BlockSyntax? block = default)
            => SyntaxFactory.UnsafeStatement(default, SyntaxFactory.Token(SyntaxKind.UnsafeKeyword), block ?? SyntaxFactory.Block());
#pragma warning restore RS0027

        /// <summary>Creates a new LockStatementSyntax instance.</summary>
        public static LockStatementSyntax LockStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken lockKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (lockKeyword.Kind() != SyntaxKind.LockKeyword) throw new ArgumentException(nameof(lockKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (LockStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.LockStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)lockKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new LockStatementSyntax instance.</summary>
        public static LockStatementSyntax LockStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.LockStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.LockKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new LockStatementSyntax instance.</summary>
        public static LockStatementSyntax LockStatement(ExpressionSyntax expression, StatementSyntax statement)
            => SyntaxFactory.LockStatement(default, SyntaxFactory.Token(SyntaxKind.LockKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), expression, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement);

        /// <summary>Creates a new IfStatementSyntax instance.</summary>
        public static IfStatementSyntax IfStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else)
        {
            if (ifKeyword.Kind() != SyntaxKind.IfKeyword) throw new ArgumentException(nameof(ifKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (IfStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.IfStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)ifKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green, @else == null ? null : (Syntax.InternalSyntax.ElseClauseSyntax)@else.Green).CreateRed();
        }

        /// <summary>Creates a new IfStatementSyntax instance.</summary>
        public static IfStatementSyntax IfStatement(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax condition, StatementSyntax statement, ElseClauseSyntax? @else)
            => SyntaxFactory.IfStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.IfKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement, @else);

        /// <summary>Creates a new IfStatementSyntax instance.</summary>
        public static IfStatementSyntax IfStatement(ExpressionSyntax condition, StatementSyntax statement)
            => SyntaxFactory.IfStatement(default, SyntaxFactory.Token(SyntaxKind.IfKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), condition, SyntaxFactory.Token(SyntaxKind.CloseParenToken), statement, default);

        /// <summary>Creates a new ElseClauseSyntax instance.</summary>
        public static ElseClauseSyntax ElseClause(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            if (elseKeyword.Kind() != SyntaxKind.ElseKeyword) throw new ArgumentException(nameof(elseKeyword));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return (ElseClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.ElseClause((Syntax.InternalSyntax.SyntaxToken)elseKeyword.Node!, (Syntax.InternalSyntax.StatementSyntax)statement.Green).CreateRed();
        }

        /// <summary>Creates a new ElseClauseSyntax instance.</summary>
        public static ElseClauseSyntax ElseClause(StatementSyntax statement)
            => SyntaxFactory.ElseClause(SyntaxFactory.Token(SyntaxKind.ElseKeyword), statement);

        /// <summary>Creates a new SwitchStatementSyntax instance.</summary>
        public static SwitchStatementSyntax SwitchStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken switchKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            if (switchKeyword.Kind() != SyntaxKind.SwitchKeyword) throw new ArgumentException(nameof(switchKeyword));
            switch (openParenToken.Kind())
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(openParenToken));
            }
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            switch (closeParenToken.Kind())
            {
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(closeParenToken));
            }
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (SwitchStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.SwitchStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)switchKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)openParenToken.Node, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Syntax.InternalSyntax.SyntaxToken?)closeParenToken.Node, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, sections.Node.ToGreenList<Syntax.InternalSyntax.SwitchSectionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new SwitchSectionSyntax instance.</summary>
        public static SwitchSectionSyntax SwitchSection(SyntaxList<SwitchLabelSyntax> labels, SyntaxList<StatementSyntax> statements)
        {
            return (SwitchSectionSyntax)Syntax.InternalSyntax.SyntaxFactory.SwitchSection(labels.Node.ToGreenList<Syntax.InternalSyntax.SwitchLabelSyntax>(), statements.Node.ToGreenList<Syntax.InternalSyntax.StatementSyntax>()).CreateRed();
        }

        /// <summary>Creates a new SwitchSectionSyntax instance.</summary>
        public static SwitchSectionSyntax SwitchSection()
            => SyntaxFactory.SwitchSection(default, default);

        /// <summary>Creates a new CasePatternSwitchLabelSyntax instance.</summary>
        public static CasePatternSwitchLabelSyntax CasePatternSwitchLabel(SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.CaseKeyword) throw new ArgumentException(nameof(keyword));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            return (CasePatternSwitchLabelSyntax)Syntax.InternalSyntax.SyntaxFactory.CasePatternSwitchLabel((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.PatternSyntax)pattern.Green, whenClause == null ? null : (Syntax.InternalSyntax.WhenClauseSyntax)whenClause.Green, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CasePatternSwitchLabelSyntax instance.</summary>
        public static CasePatternSwitchLabelSyntax CasePatternSwitchLabel(PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken)
            => SyntaxFactory.CasePatternSwitchLabel(SyntaxFactory.Token(SyntaxKind.CaseKeyword), pattern, whenClause, colonToken);

        /// <summary>Creates a new CasePatternSwitchLabelSyntax instance.</summary>
        public static CasePatternSwitchLabelSyntax CasePatternSwitchLabel(PatternSyntax pattern, SyntaxToken colonToken)
            => SyntaxFactory.CasePatternSwitchLabel(SyntaxFactory.Token(SyntaxKind.CaseKeyword), pattern, default, colonToken);

        /// <summary>Creates a new CaseSwitchLabelSyntax instance.</summary>
        public static CaseSwitchLabelSyntax CaseSwitchLabel(SyntaxToken keyword, ExpressionSyntax value, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.CaseKeyword) throw new ArgumentException(nameof(keyword));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (CaseSwitchLabelSyntax)Syntax.InternalSyntax.SyntaxFactory.CaseSwitchLabel((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)value.Green, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CaseSwitchLabelSyntax instance.</summary>
        public static CaseSwitchLabelSyntax CaseSwitchLabel(ExpressionSyntax value, SyntaxToken colonToken)
            => SyntaxFactory.CaseSwitchLabel(SyntaxFactory.Token(SyntaxKind.CaseKeyword), value, colonToken);

        /// <summary>Creates a new DefaultSwitchLabelSyntax instance.</summary>
        public static DefaultSwitchLabelSyntax DefaultSwitchLabel(SyntaxToken keyword, SyntaxToken colonToken)
        {
            if (keyword.Kind() != SyntaxKind.DefaultKeyword) throw new ArgumentException(nameof(keyword));
            return (DefaultSwitchLabelSyntax)Syntax.InternalSyntax.SyntaxFactory.DefaultSwitchLabel((Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DefaultSwitchLabelSyntax instance.</summary>
        public static DefaultSwitchLabelSyntax DefaultSwitchLabel(SyntaxToken colonToken)
            => SyntaxFactory.DefaultSwitchLabel(SyntaxFactory.Token(SyntaxKind.DefaultKeyword), colonToken);

        /// <summary>Creates a new SwitchExpressionSyntax instance.</summary>
        public static SwitchExpressionSyntax SwitchExpression(ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<SwitchExpressionArmSyntax> arms, SyntaxToken closeBraceToken)
        {
            if (governingExpression == null) throw new ArgumentNullException(nameof(governingExpression));
            if (switchKeyword.Kind() != SyntaxKind.SwitchKeyword) throw new ArgumentException(nameof(switchKeyword));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (SwitchExpressionSyntax)Syntax.InternalSyntax.SyntaxFactory.SwitchExpression((Syntax.InternalSyntax.ExpressionSyntax)governingExpression.Green, (Syntax.InternalSyntax.SyntaxToken)switchKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, arms.Node.ToGreenSeparatedList<Syntax.InternalSyntax.SwitchExpressionArmSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new SwitchExpressionSyntax instance.</summary>
        public static SwitchExpressionSyntax SwitchExpression(ExpressionSyntax governingExpression, SeparatedSyntaxList<SwitchExpressionArmSyntax> arms)
            => SyntaxFactory.SwitchExpression(governingExpression, SyntaxFactory.Token(SyntaxKind.SwitchKeyword), SyntaxFactory.Token(SyntaxKind.OpenBraceToken), arms, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new SwitchExpressionSyntax instance.</summary>
        public static SwitchExpressionSyntax SwitchExpression(ExpressionSyntax governingExpression)
            => SyntaxFactory.SwitchExpression(governingExpression, SyntaxFactory.Token(SyntaxKind.SwitchKeyword), SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new SwitchExpressionArmSyntax instance.</summary>
        public static SwitchExpressionArmSyntax SwitchExpressionArm(PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (equalsGreaterThanToken.Kind() != SyntaxKind.EqualsGreaterThanToken) throw new ArgumentException(nameof(equalsGreaterThanToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (SwitchExpressionArmSyntax)Syntax.InternalSyntax.SyntaxFactory.SwitchExpressionArm((Syntax.InternalSyntax.PatternSyntax)pattern.Green, whenClause == null ? null : (Syntax.InternalSyntax.WhenClauseSyntax)whenClause.Green, (Syntax.InternalSyntax.SyntaxToken)equalsGreaterThanToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new SwitchExpressionArmSyntax instance.</summary>
        public static SwitchExpressionArmSyntax SwitchExpressionArm(PatternSyntax pattern, WhenClauseSyntax? whenClause, ExpressionSyntax expression)
            => SyntaxFactory.SwitchExpressionArm(pattern, whenClause, SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), expression);

        /// <summary>Creates a new SwitchExpressionArmSyntax instance.</summary>
        public static SwitchExpressionArmSyntax SwitchExpressionArm(PatternSyntax pattern, ExpressionSyntax expression)
            => SyntaxFactory.SwitchExpressionArm(pattern, default, SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), expression);

        /// <summary>Creates a new TryStatementSyntax instance.</summary>
        public static TryStatementSyntax TryStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken tryKeyword, BlockSyntax block, SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax? @finally)
        {
            if (tryKeyword.Kind() != SyntaxKind.TryKeyword) throw new ArgumentException(nameof(tryKeyword));
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (TryStatementSyntax)Syntax.InternalSyntax.SyntaxFactory.TryStatement(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken)tryKeyword.Node!, (Syntax.InternalSyntax.BlockSyntax)block.Green, catches.Node.ToGreenList<Syntax.InternalSyntax.CatchClauseSyntax>(), @finally == null ? null : (Syntax.InternalSyntax.FinallyClauseSyntax)@finally.Green).CreateRed();
        }

        /// <summary>Creates a new TryStatementSyntax instance.</summary>
        public static TryStatementSyntax TryStatement(SyntaxList<AttributeListSyntax> attributeLists, BlockSyntax block, SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax? @finally)
            => SyntaxFactory.TryStatement(attributeLists, SyntaxFactory.Token(SyntaxKind.TryKeyword), block, catches, @finally);

#pragma warning disable RS0027
        /// <summary>Creates a new TryStatementSyntax instance.</summary>
        public static TryStatementSyntax TryStatement(SyntaxList<CatchClauseSyntax> catches = default)
            => SyntaxFactory.TryStatement(default, SyntaxFactory.Token(SyntaxKind.TryKeyword), SyntaxFactory.Block(), catches, default);
#pragma warning restore RS0027

        /// <summary>Creates a new CatchClauseSyntax instance.</summary>
        public static CatchClauseSyntax CatchClause(SyntaxToken catchKeyword, CatchDeclarationSyntax? declaration, CatchFilterClauseSyntax? filter, BlockSyntax block)
        {
            if (catchKeyword.Kind() != SyntaxKind.CatchKeyword) throw new ArgumentException(nameof(catchKeyword));
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (CatchClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.CatchClause((Syntax.InternalSyntax.SyntaxToken)catchKeyword.Node!, declaration == null ? null : (Syntax.InternalSyntax.CatchDeclarationSyntax)declaration.Green, filter == null ? null : (Syntax.InternalSyntax.CatchFilterClauseSyntax)filter.Green, (Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        /// <summary>Creates a new CatchClauseSyntax instance.</summary>
        public static CatchClauseSyntax CatchClause(CatchDeclarationSyntax? declaration, CatchFilterClauseSyntax? filter, BlockSyntax block)
            => SyntaxFactory.CatchClause(SyntaxFactory.Token(SyntaxKind.CatchKeyword), declaration, filter, block);

        /// <summary>Creates a new CatchClauseSyntax instance.</summary>
        public static CatchClauseSyntax CatchClause()
            => SyntaxFactory.CatchClause(SyntaxFactory.Token(SyntaxKind.CatchKeyword), default, default, SyntaxFactory.Block());

        /// <summary>Creates a new CatchDeclarationSyntax instance.</summary>
        public static CatchDeclarationSyntax CatchDeclaration(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (type == null) throw new ArgumentNullException(nameof(type));
            switch (identifier.Kind())
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(identifier));
            }
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (CatchDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.CatchDeclaration((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken?)identifier.Node, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CatchDeclarationSyntax instance.</summary>
        public static CatchDeclarationSyntax CatchDeclaration(TypeSyntax type, SyntaxToken identifier)
            => SyntaxFactory.CatchDeclaration(SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, identifier, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new CatchDeclarationSyntax instance.</summary>
        public static CatchDeclarationSyntax CatchDeclaration(TypeSyntax type)
            => SyntaxFactory.CatchDeclaration(SyntaxFactory.Token(SyntaxKind.OpenParenToken), type, default, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new CatchFilterClauseSyntax instance.</summary>
        public static CatchFilterClauseSyntax CatchFilterClause(SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
        {
            if (whenKeyword.Kind() != SyntaxKind.WhenKeyword) throw new ArgumentException(nameof(whenKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (CatchFilterClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.CatchFilterClause((Syntax.InternalSyntax.SyntaxToken)whenKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)filterExpression.Green, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CatchFilterClauseSyntax instance.</summary>
        public static CatchFilterClauseSyntax CatchFilterClause(ExpressionSyntax filterExpression)
            => SyntaxFactory.CatchFilterClause(SyntaxFactory.Token(SyntaxKind.WhenKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), filterExpression, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new FinallyClauseSyntax instance.</summary>
        public static FinallyClauseSyntax FinallyClause(SyntaxToken finallyKeyword, BlockSyntax block)
        {
            if (finallyKeyword.Kind() != SyntaxKind.FinallyKeyword) throw new ArgumentException(nameof(finallyKeyword));
            if (block == null) throw new ArgumentNullException(nameof(block));
            return (FinallyClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.FinallyClause((Syntax.InternalSyntax.SyntaxToken)finallyKeyword.Node!, (Syntax.InternalSyntax.BlockSyntax)block.Green).CreateRed();
        }

        /// <summary>Creates a new FinallyClauseSyntax instance.</summary>
        public static FinallyClauseSyntax FinallyClause(BlockSyntax? block = default)
            => SyntaxFactory.FinallyClause(SyntaxFactory.Token(SyntaxKind.FinallyKeyword), block ?? SyntaxFactory.Block());

        /// <summary>Creates a new CompilationUnitSyntax instance.</summary>
        public static CompilationUnitSyntax CompilationUnit(SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<AttributeListSyntax> attributeLists, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken endOfFileToken)
        {
            if (endOfFileToken.Kind() != SyntaxKind.EndOfFileToken) throw new ArgumentException(nameof(endOfFileToken));
            return (CompilationUnitSyntax)Syntax.InternalSyntax.SyntaxFactory.CompilationUnit(externs.Node.ToGreenList<Syntax.InternalSyntax.ExternAliasDirectiveSyntax>(), usings.Node.ToGreenList<Syntax.InternalSyntax.UsingDirectiveSyntax>(), attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)endOfFileToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CompilationUnitSyntax instance.</summary>
        public static CompilationUnitSyntax CompilationUnit(SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<AttributeListSyntax> attributeLists, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.CompilationUnit(externs, usings, attributeLists, members, SyntaxFactory.Token(SyntaxKind.EndOfFileToken));

        /// <summary>Creates a new CompilationUnitSyntax instance.</summary>
        public static CompilationUnitSyntax CompilationUnit()
            => SyntaxFactory.CompilationUnit(default, default, default, default, SyntaxFactory.Token(SyntaxKind.EndOfFileToken));

        /// <summary>Creates a new ExternAliasDirectiveSyntax instance.</summary>
        public static ExternAliasDirectiveSyntax ExternAliasDirective(SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            if (externKeyword.Kind() != SyntaxKind.ExternKeyword) throw new ArgumentException(nameof(externKeyword));
            if (aliasKeyword.Kind() != SyntaxKind.AliasKeyword) throw new ArgumentException(nameof(aliasKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (ExternAliasDirectiveSyntax)Syntax.InternalSyntax.SyntaxFactory.ExternAliasDirective((Syntax.InternalSyntax.SyntaxToken)externKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)aliasKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ExternAliasDirectiveSyntax instance.</summary>
        public static ExternAliasDirectiveSyntax ExternAliasDirective(SyntaxToken identifier)
            => SyntaxFactory.ExternAliasDirective(SyntaxFactory.Token(SyntaxKind.ExternKeyword), SyntaxFactory.Token(SyntaxKind.AliasKeyword), identifier, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ExternAliasDirectiveSyntax instance.</summary>
        public static ExternAliasDirectiveSyntax ExternAliasDirective(string identifier)
            => SyntaxFactory.ExternAliasDirective(SyntaxFactory.Token(SyntaxKind.ExternKeyword), SyntaxFactory.Token(SyntaxKind.AliasKeyword), SyntaxFactory.Identifier(identifier), SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new UsingDirectiveSyntax instance.</summary>
        public static UsingDirectiveSyntax UsingDirective(SyntaxToken globalKeyword, SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken)
        {
            switch (globalKeyword.Kind())
            {
                case SyntaxKind.GlobalKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(globalKeyword));
            }
            if (usingKeyword.Kind() != SyntaxKind.UsingKeyword) throw new ArgumentException(nameof(usingKeyword));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (UsingDirectiveSyntax)Syntax.InternalSyntax.SyntaxFactory.UsingDirective((Syntax.InternalSyntax.SyntaxToken?)globalKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)usingKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)staticKeyword.Node, alias == null ? null : (Syntax.InternalSyntax.NameEqualsSyntax)alias.Green, (Syntax.InternalSyntax.NameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new UsingDirectiveSyntax instance.</summary>
        public static UsingDirectiveSyntax UsingDirective(SyntaxToken staticKeyword, NameEqualsSyntax? alias, NameSyntax name)
            => SyntaxFactory.UsingDirective(default, SyntaxFactory.Token(SyntaxKind.UsingKeyword), staticKeyword, alias, name, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new UsingDirectiveSyntax instance.</summary>
        public static UsingDirectiveSyntax UsingDirective(NameSyntax name)
            => SyntaxFactory.UsingDirective(default, SyntaxFactory.Token(SyntaxKind.UsingKeyword), default, default, name, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new NamespaceDeclarationSyntax instance.</summary>
        public static NamespaceDeclarationSyntax NamespaceDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (namespaceKeyword.Kind() != SyntaxKind.NamespaceKeyword) throw new ArgumentException(nameof(namespaceKeyword));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (NamespaceDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.NamespaceDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)namespaceKeyword.Node!, (Syntax.InternalSyntax.NameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, externs.Node.ToGreenList<Syntax.InternalSyntax.ExternAliasDirectiveSyntax>(), usings.Node.ToGreenList<Syntax.InternalSyntax.UsingDirectiveSyntax>(), members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new NamespaceDeclarationSyntax instance.</summary>
        public static NamespaceDeclarationSyntax NamespaceDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, NameSyntax name, SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.NamespaceDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.NamespaceKeyword), name, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), externs, usings, members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new NamespaceDeclarationSyntax instance.</summary>
        public static NamespaceDeclarationSyntax NamespaceDeclaration(NameSyntax name)
            => SyntaxFactory.NamespaceDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.NamespaceKeyword), name, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, default, default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new AttributeListSyntax instance.</summary>
        public static AttributeListSyntax AttributeList(SyntaxToken openBracketToken, AttributeTargetSpecifierSyntax? target, SeparatedSyntaxList<AttributeSyntax> attributes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (AttributeListSyntax)Syntax.InternalSyntax.SyntaxFactory.AttributeList((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, target == null ? null : (Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)target.Green, attributes.Node.ToGreenSeparatedList<Syntax.InternalSyntax.AttributeSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new AttributeListSyntax instance.</summary>
        public static AttributeListSyntax AttributeList(AttributeTargetSpecifierSyntax? target, SeparatedSyntaxList<AttributeSyntax> attributes)
            => SyntaxFactory.AttributeList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), target, attributes, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new AttributeListSyntax instance.</summary>
        public static AttributeListSyntax AttributeList(SeparatedSyntaxList<AttributeSyntax> attributes = default)
            => SyntaxFactory.AttributeList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), default, attributes, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new AttributeTargetSpecifierSyntax instance.</summary>
        public static AttributeTargetSpecifierSyntax AttributeTargetSpecifier(SyntaxToken identifier, SyntaxToken colonToken)
        {
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            return (AttributeTargetSpecifierSyntax)Syntax.InternalSyntax.SyntaxFactory.AttributeTargetSpecifier((Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new AttributeTargetSpecifierSyntax instance.</summary>
        public static AttributeTargetSpecifierSyntax AttributeTargetSpecifier(SyntaxToken identifier)
            => SyntaxFactory.AttributeTargetSpecifier(identifier, SyntaxFactory.Token(SyntaxKind.ColonToken));

        /// <summary>Creates a new AttributeSyntax instance.</summary>
        public static AttributeSyntax Attribute(NameSyntax name, AttributeArgumentListSyntax? argumentList)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return (AttributeSyntax)Syntax.InternalSyntax.SyntaxFactory.Attribute((Syntax.InternalSyntax.NameSyntax)name.Green, argumentList == null ? null : (Syntax.InternalSyntax.AttributeArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new AttributeSyntax instance.</summary>
        public static AttributeSyntax Attribute(NameSyntax name)
            => SyntaxFactory.Attribute(name, default);

        /// <summary>Creates a new AttributeArgumentListSyntax instance.</summary>
        public static AttributeArgumentListSyntax AttributeArgumentList(SyntaxToken openParenToken, SeparatedSyntaxList<AttributeArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (AttributeArgumentListSyntax)Syntax.InternalSyntax.SyntaxFactory.AttributeArgumentList((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, arguments.Node.ToGreenSeparatedList<Syntax.InternalSyntax.AttributeArgumentSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new AttributeArgumentListSyntax instance.</summary>
        public static AttributeArgumentListSyntax AttributeArgumentList(SeparatedSyntaxList<AttributeArgumentSyntax> arguments = default)
            => SyntaxFactory.AttributeArgumentList(SyntaxFactory.Token(SyntaxKind.OpenParenToken), arguments, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new AttributeArgumentSyntax instance.</summary>
        public static AttributeArgumentSyntax AttributeArgument(NameEqualsSyntax? nameEquals, NameColonSyntax? nameColon, ExpressionSyntax expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (AttributeArgumentSyntax)Syntax.InternalSyntax.SyntaxFactory.AttributeArgument(nameEquals == null ? null : (Syntax.InternalSyntax.NameEqualsSyntax)nameEquals.Green, nameColon == null ? null : (Syntax.InternalSyntax.NameColonSyntax)nameColon.Green, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new AttributeArgumentSyntax instance.</summary>
        public static AttributeArgumentSyntax AttributeArgument(ExpressionSyntax expression)
            => SyntaxFactory.AttributeArgument(default, default, expression);

        /// <summary>Creates a new NameEqualsSyntax instance.</summary>
        public static NameEqualsSyntax NameEquals(IdentifierNameSyntax name, SyntaxToken equalsToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            return (NameEqualsSyntax)Syntax.InternalSyntax.SyntaxFactory.NameEquals((Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!).CreateRed();
        }

        /// <summary>Creates a new NameEqualsSyntax instance.</summary>
        public static NameEqualsSyntax NameEquals(IdentifierNameSyntax name)
            => SyntaxFactory.NameEquals(name, SyntaxFactory.Token(SyntaxKind.EqualsToken));

        /// <summary>Creates a new NameEqualsSyntax instance.</summary>
        public static NameEqualsSyntax NameEquals(string name)
            => SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(name), SyntaxFactory.Token(SyntaxKind.EqualsToken));

        /// <summary>Creates a new TypeParameterListSyntax instance.</summary>
        public static TypeParameterListSyntax TypeParameterList(SyntaxToken lessThanToken, SeparatedSyntaxList<TypeParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken) throw new ArgumentException(nameof(lessThanToken));
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken) throw new ArgumentException(nameof(greaterThanToken));
            return (TypeParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeParameterList((Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.TypeParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new TypeParameterListSyntax instance.</summary>
        public static TypeParameterListSyntax TypeParameterList(SeparatedSyntaxList<TypeParameterSyntax> parameters = default)
            => SyntaxFactory.TypeParameterList(SyntaxFactory.Token(SyntaxKind.LessThanToken), parameters, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new TypeParameterSyntax instance.</summary>
        public static TypeParameterSyntax TypeParameter(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken varianceKeyword, SyntaxToken identifier)
        {
            switch (varianceKeyword.Kind())
            {
                case SyntaxKind.InKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(varianceKeyword));
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            return (TypeParameterSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeParameter(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)varianceKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!).CreateRed();
        }

        /// <summary>Creates a new TypeParameterSyntax instance.</summary>
        public static TypeParameterSyntax TypeParameter(SyntaxToken identifier)
            => SyntaxFactory.TypeParameter(default, default, identifier);

        /// <summary>Creates a new TypeParameterSyntax instance.</summary>
        public static TypeParameterSyntax TypeParameter(string identifier)
            => SyntaxFactory.TypeParameter(default, default, SyntaxFactory.Identifier(identifier));

        /// <summary>Creates a new ClassDeclarationSyntax instance.</summary>
        public static ClassDeclarationSyntax ClassDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.ClassKeyword) throw new ArgumentException(nameof(keyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (ClassDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.ClassDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, baseList == null ? null : (Syntax.InternalSyntax.BaseListSyntax)baseList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new ClassDeclarationSyntax instance.</summary>
        public static ClassDeclarationSyntax ClassDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.ClassDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.ClassKeyword), identifier, typeParameterList, baseList, constraintClauses, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new ClassDeclarationSyntax instance.</summary>
        public static ClassDeclarationSyntax ClassDeclaration(SyntaxToken identifier)
            => SyntaxFactory.ClassDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.ClassKeyword), identifier, default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new ClassDeclarationSyntax instance.</summary>
        public static ClassDeclarationSyntax ClassDeclaration(string identifier)
            => SyntaxFactory.ClassDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.ClassKeyword), SyntaxFactory.Identifier(identifier), default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new StructDeclarationSyntax instance.</summary>
        public static StructDeclarationSyntax StructDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.StructKeyword) throw new ArgumentException(nameof(keyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (StructDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.StructDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, baseList == null ? null : (Syntax.InternalSyntax.BaseListSyntax)baseList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new StructDeclarationSyntax instance.</summary>
        public static StructDeclarationSyntax StructDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.StructDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.StructKeyword), identifier, typeParameterList, baseList, constraintClauses, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new StructDeclarationSyntax instance.</summary>
        public static StructDeclarationSyntax StructDeclaration(SyntaxToken identifier)
            => SyntaxFactory.StructDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.StructKeyword), identifier, default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new StructDeclarationSyntax instance.</summary>
        public static StructDeclarationSyntax StructDeclaration(string identifier)
            => SyntaxFactory.StructDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.StructKeyword), SyntaxFactory.Identifier(identifier), default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new InterfaceDeclarationSyntax instance.</summary>
        public static InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (keyword.Kind() != SyntaxKind.InterfaceKeyword) throw new ArgumentException(nameof(keyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (InterfaceDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.InterfaceDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, baseList == null ? null : (Syntax.InternalSyntax.BaseListSyntax)baseList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new InterfaceDeclarationSyntax instance.</summary>
        public static InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.InterfaceDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.InterfaceKeyword), identifier, typeParameterList, baseList, constraintClauses, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new InterfaceDeclarationSyntax instance.</summary>
        public static InterfaceDeclarationSyntax InterfaceDeclaration(SyntaxToken identifier)
            => SyntaxFactory.InterfaceDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.InterfaceKeyword), identifier, default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new InterfaceDeclarationSyntax instance.</summary>
        public static InterfaceDeclarationSyntax InterfaceDeclaration(string identifier)
            => SyntaxFactory.InterfaceDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.InterfaceKeyword), SyntaxFactory.Identifier(identifier), default, default, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new RecordDeclarationSyntax instance.</summary>
        public static RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            switch (kind)
            {
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (classOrStructKeyword.Kind())
            {
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(classOrStructKeyword));
            }
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            switch (openBraceToken.Kind())
            {
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(openBraceToken));
            }
            switch (closeBraceToken.Kind())
            {
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(closeBraceToken));
            }
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (RecordDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.RecordDeclaration(kind, attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)classOrStructKeyword.Node, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, parameterList == null ? null : (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, baseList == null ? null : (Syntax.InternalSyntax.BaseListSyntax)baseList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)openBraceToken.Node, members.Node.ToGreenList<Syntax.InternalSyntax.MemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken?)closeBraceToken.Node, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new RecordDeclarationSyntax instance.</summary>
        public static RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxList<MemberDeclarationSyntax> members)
            => SyntaxFactory.RecordDeclaration(kind, attributeLists, modifiers, keyword, default, identifier, typeParameterList, parameterList, baseList, constraintClauses, default, members, default, default);

        /// <summary>Creates a new RecordDeclarationSyntax instance.</summary>
        public static RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxToken keyword, SyntaxToken identifier)
            => SyntaxFactory.RecordDeclaration(kind, default, default(SyntaxTokenList), keyword, default, identifier, default, default, default, default, default, default, default, default);

        /// <summary>Creates a new RecordDeclarationSyntax instance.</summary>
        public static RecordDeclarationSyntax RecordDeclaration(SyntaxKind kind, SyntaxToken keyword, string identifier)
            => SyntaxFactory.RecordDeclaration(kind, default, default(SyntaxTokenList), keyword, default, SyntaxFactory.Identifier(identifier), default, default, default, default, default, default, default, default);

        private static SyntaxKind GetRecordDeclarationClassOrStructKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.RecordDeclaration => SyntaxKind.ClassKeyword,
                SyntaxKind.RecordStructDeclaration => SyntaxKind.StructKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new EnumDeclarationSyntax instance.</summary>
        public static EnumDeclarationSyntax EnumDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax? baseList, SyntaxToken openBraceToken, SeparatedSyntaxList<EnumMemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (enumKeyword.Kind() != SyntaxKind.EnumKeyword) throw new ArgumentException(nameof(enumKeyword));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (EnumDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.EnumDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)enumKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, baseList == null ? null : (Syntax.InternalSyntax.BaseListSyntax)baseList.Green, (Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, members.Node.ToGreenSeparatedList<Syntax.InternalSyntax.EnumMemberDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new EnumDeclarationSyntax instance.</summary>
        public static EnumDeclarationSyntax EnumDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, BaseListSyntax? baseList, SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
            => SyntaxFactory.EnumDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.EnumKeyword), identifier, baseList, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new EnumDeclarationSyntax instance.</summary>
        public static EnumDeclarationSyntax EnumDeclaration(SyntaxToken identifier)
            => SyntaxFactory.EnumDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.EnumKeyword), identifier, default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new EnumDeclarationSyntax instance.</summary>
        public static EnumDeclarationSyntax EnumDeclaration(string identifier)
            => SyntaxFactory.EnumDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.EnumKeyword), SyntaxFactory.Identifier(identifier), default, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), default, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), default);

        /// <summary>Creates a new DelegateDeclarationSyntax instance.</summary>
        public static DelegateDeclarationSyntax DelegateDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken semicolonToken)
        {
            if (delegateKeyword.Kind() != SyntaxKind.DelegateKeyword) throw new ArgumentException(nameof(delegateKeyword));
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (DelegateDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.DelegateDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)delegateKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new DelegateDeclarationSyntax instance.</summary>
        public static DelegateDeclarationSyntax DelegateDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
            => SyntaxFactory.DelegateDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.DelegateKeyword), returnType, identifier, typeParameterList, parameterList, constraintClauses, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new DelegateDeclarationSyntax instance.</summary>
        public static DelegateDeclarationSyntax DelegateDeclaration(TypeSyntax returnType, SyntaxToken identifier)
            => SyntaxFactory.DelegateDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.DelegateKeyword), returnType, identifier, default, SyntaxFactory.ParameterList(), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new DelegateDeclarationSyntax instance.</summary>
        public static DelegateDeclarationSyntax DelegateDeclaration(TypeSyntax returnType, string identifier)
            => SyntaxFactory.DelegateDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.DelegateKeyword), returnType, SyntaxFactory.Identifier(identifier), default, SyntaxFactory.ParameterList(), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new EnumMemberDeclarationSyntax instance.</summary>
        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            return (EnumMemberDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.EnumMemberDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, equalsValue == null ? null : (Syntax.InternalSyntax.EqualsValueClauseSyntax)equalsValue.Green).CreateRed();
        }

        /// <summary>Creates a new EnumMemberDeclarationSyntax instance.</summary>
        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(SyntaxToken identifier)
            => SyntaxFactory.EnumMemberDeclaration(default, default(SyntaxTokenList), identifier, default);

        /// <summary>Creates a new EnumMemberDeclarationSyntax instance.</summary>
        public static EnumMemberDeclarationSyntax EnumMemberDeclaration(string identifier)
            => SyntaxFactory.EnumMemberDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Identifier(identifier), default);

        /// <summary>Creates a new BaseListSyntax instance.</summary>
        public static BaseListSyntax BaseList(SyntaxToken colonToken, SeparatedSyntaxList<BaseTypeSyntax> types)
        {
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            return (BaseListSyntax)Syntax.InternalSyntax.SyntaxFactory.BaseList((Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, types.Node.ToGreenSeparatedList<Syntax.InternalSyntax.BaseTypeSyntax>()).CreateRed();
        }

        /// <summary>Creates a new BaseListSyntax instance.</summary>
        public static BaseListSyntax BaseList(SeparatedSyntaxList<BaseTypeSyntax> types = default)
            => SyntaxFactory.BaseList(SyntaxFactory.Token(SyntaxKind.ColonToken), types);

        /// <summary>Creates a new SimpleBaseTypeSyntax instance.</summary>
        public static SimpleBaseTypeSyntax SimpleBaseType(TypeSyntax type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (SimpleBaseTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.SimpleBaseType((Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new PrimaryConstructorBaseTypeSyntax instance.</summary>
        public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(TypeSyntax type, ArgumentListSyntax argumentList)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (PrimaryConstructorBaseTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.PrimaryConstructorBaseType((Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new PrimaryConstructorBaseTypeSyntax instance.</summary>
        public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(TypeSyntax type)
            => SyntaxFactory.PrimaryConstructorBaseType(type, SyntaxFactory.ArgumentList());

        /// <summary>Creates a new TypeParameterConstraintClauseSyntax instance.</summary>
        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            if (whereKeyword.Kind() != SyntaxKind.WhereKeyword) throw new ArgumentException(nameof(whereKeyword));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            return (TypeParameterConstraintClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeParameterConstraintClause((Syntax.InternalSyntax.SyntaxToken)whereKeyword.Node!, (Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, constraints.Node.ToGreenSeparatedList<Syntax.InternalSyntax.TypeParameterConstraintSyntax>()).CreateRed();
        }

        /// <summary>Creates a new TypeParameterConstraintClauseSyntax instance.</summary>
        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(IdentifierNameSyntax name, SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
            => SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.Token(SyntaxKind.WhereKeyword), name, SyntaxFactory.Token(SyntaxKind.ColonToken), constraints);

        /// <summary>Creates a new TypeParameterConstraintClauseSyntax instance.</summary>
        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(IdentifierNameSyntax name)
            => SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.Token(SyntaxKind.WhereKeyword), name, SyntaxFactory.Token(SyntaxKind.ColonToken), default);

        /// <summary>Creates a new TypeParameterConstraintClauseSyntax instance.</summary>
        public static TypeParameterConstraintClauseSyntax TypeParameterConstraintClause(string name)
            => SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.Token(SyntaxKind.WhereKeyword), SyntaxFactory.IdentifierName(name), SyntaxFactory.Token(SyntaxKind.ColonToken), default);

        /// <summary>Creates a new ConstructorConstraintSyntax instance.</summary>
        public static ConstructorConstraintSyntax ConstructorConstraint(SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
        {
            if (newKeyword.Kind() != SyntaxKind.NewKeyword) throw new ArgumentException(nameof(newKeyword));
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ConstructorConstraintSyntax)Syntax.InternalSyntax.SyntaxFactory.ConstructorConstraint((Syntax.InternalSyntax.SyntaxToken)newKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ConstructorConstraintSyntax instance.</summary>
        public static ConstructorConstraintSyntax ConstructorConstraint()
            => SyntaxFactory.ConstructorConstraint(SyntaxFactory.Token(SyntaxKind.NewKeyword), SyntaxFactory.Token(SyntaxKind.OpenParenToken), SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new ClassOrStructConstraintSyntax instance.</summary>
        public static ClassOrStructConstraintSyntax ClassOrStructConstraint(SyntaxKind kind, SyntaxToken classOrStructKeyword, SyntaxToken questionToken)
        {
            switch (kind)
            {
                case SyntaxKind.ClassConstraint:
                case SyntaxKind.StructConstraint: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (classOrStructKeyword.Kind())
            {
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword: break;
                default: throw new ArgumentException(nameof(classOrStructKeyword));
            }
            switch (questionToken.Kind())
            {
                case SyntaxKind.QuestionToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(questionToken));
            }
            return (ClassOrStructConstraintSyntax)Syntax.InternalSyntax.SyntaxFactory.ClassOrStructConstraint(kind, (Syntax.InternalSyntax.SyntaxToken)classOrStructKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken?)questionToken.Node).CreateRed();
        }

        /// <summary>Creates a new ClassOrStructConstraintSyntax instance.</summary>
        public static ClassOrStructConstraintSyntax ClassOrStructConstraint(SyntaxKind kind)
            => SyntaxFactory.ClassOrStructConstraint(kind, SyntaxFactory.Token(GetClassOrStructConstraintClassOrStructKeywordKind(kind)), default);

        private static SyntaxKind GetClassOrStructConstraintClassOrStructKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.ClassConstraint => SyntaxKind.ClassKeyword,
                SyntaxKind.StructConstraint => SyntaxKind.StructKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new TypeConstraintSyntax instance.</summary>
        public static TypeConstraintSyntax TypeConstraint(TypeSyntax type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (TypeConstraintSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeConstraint((Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new DefaultConstraintSyntax instance.</summary>
        public static DefaultConstraintSyntax DefaultConstraint(SyntaxToken defaultKeyword)
        {
            if (defaultKeyword.Kind() != SyntaxKind.DefaultKeyword) throw new ArgumentException(nameof(defaultKeyword));
            return (DefaultConstraintSyntax)Syntax.InternalSyntax.SyntaxFactory.DefaultConstraint((Syntax.InternalSyntax.SyntaxToken)defaultKeyword.Node!).CreateRed();
        }

        /// <summary>Creates a new DefaultConstraintSyntax instance.</summary>
        public static DefaultConstraintSyntax DefaultConstraint()
            => SyntaxFactory.DefaultConstraint(SyntaxFactory.Token(SyntaxKind.DefaultKeyword));

        /// <summary>Creates a new FieldDeclarationSyntax instance.</summary>
        public static FieldDeclarationSyntax FieldDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (FieldDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.FieldDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new FieldDeclarationSyntax instance.</summary>
        public static FieldDeclarationSyntax FieldDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration)
            => SyntaxFactory.FieldDeclaration(attributeLists, modifiers, declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new FieldDeclarationSyntax instance.</summary>
        public static FieldDeclarationSyntax FieldDeclaration(VariableDeclarationSyntax declaration)
            => SyntaxFactory.FieldDeclaration(default, default(SyntaxTokenList), declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new EventFieldDeclarationSyntax instance.</summary>
        public static EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (eventKeyword.Kind() != SyntaxKind.EventKeyword) throw new ArgumentException(nameof(eventKeyword));
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            if (semicolonToken.Kind() != SyntaxKind.SemicolonToken) throw new ArgumentException(nameof(semicolonToken));
            return (EventFieldDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.EventFieldDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)eventKeyword.Node!, (Syntax.InternalSyntax.VariableDeclarationSyntax)declaration.Green, (Syntax.InternalSyntax.SyntaxToken)semicolonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new EventFieldDeclarationSyntax instance.</summary>
        public static EventFieldDeclarationSyntax EventFieldDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration)
            => SyntaxFactory.EventFieldDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.EventKeyword), declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new EventFieldDeclarationSyntax instance.</summary>
        public static EventFieldDeclarationSyntax EventFieldDeclaration(VariableDeclarationSyntax declaration)
            => SyntaxFactory.EventFieldDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.EventKeyword), declaration, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        /// <summary>Creates a new ExplicitInterfaceSpecifierSyntax instance.</summary>
        public static ExplicitInterfaceSpecifierSyntax ExplicitInterfaceSpecifier(NameSyntax name, SyntaxToken dotToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (dotToken.Kind() != SyntaxKind.DotToken) throw new ArgumentException(nameof(dotToken));
            return (ExplicitInterfaceSpecifierSyntax)Syntax.InternalSyntax.SyntaxFactory.ExplicitInterfaceSpecifier((Syntax.InternalSyntax.NameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)dotToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ExplicitInterfaceSpecifierSyntax instance.</summary>
        public static ExplicitInterfaceSpecifierSyntax ExplicitInterfaceSpecifier(NameSyntax name)
            => SyntaxFactory.ExplicitInterfaceSpecifier(name, SyntaxFactory.Token(SyntaxKind.DotToken));

        /// <summary>Creates a new MethodDeclarationSyntax instance.</summary>
        public static MethodDeclarationSyntax MethodDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (MethodDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.MethodDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)returnType.Green, explicitInterfaceSpecifier == null ? null : (Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, typeParameterList == null ? null : (Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, constraintClauses.Node.ToGreenList<Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax>(), body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new MethodDeclarationSyntax instance.</summary>
        public static MethodDeclarationSyntax MethodDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.MethodDeclaration(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, default);

        /// <summary>Creates a new MethodDeclarationSyntax instance.</summary>
        public static MethodDeclarationSyntax MethodDeclaration(TypeSyntax returnType, SyntaxToken identifier)
            => SyntaxFactory.MethodDeclaration(default, default(SyntaxTokenList), returnType, default, identifier, default, SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new MethodDeclarationSyntax instance.</summary>
        public static MethodDeclarationSyntax MethodDeclaration(TypeSyntax returnType, string identifier)
            => SyntaxFactory.MethodDeclaration(default, default(SyntaxTokenList), returnType, default, SyntaxFactory.Identifier(identifier), default, SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new OperatorDeclarationSyntax instance.</summary>
        public static OperatorDeclarationSyntax OperatorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword) throw new ArgumentException(nameof(operatorKeyword));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.TildeToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.IsKeyword: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (OperatorDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.OperatorDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)returnType.Green, (Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new OperatorDeclarationSyntax instance.</summary>
        public static OperatorDeclarationSyntax OperatorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.OperatorDeclaration(attributeLists, modifiers, returnType, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), operatorToken, parameterList, body, expressionBody, default);

        /// <summary>Creates a new OperatorDeclarationSyntax instance.</summary>
        public static OperatorDeclarationSyntax OperatorDeclaration(TypeSyntax returnType, SyntaxToken operatorToken)
            => SyntaxFactory.OperatorDeclaration(default, default(SyntaxTokenList), returnType, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), operatorToken, SyntaxFactory.ParameterList(), default, default, default);

        /// <summary>Creates a new ConversionOperatorDeclarationSyntax instance.</summary>
        public static ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            switch (implicitOrExplicitKeyword.Kind())
            {
                case SyntaxKind.ImplicitKeyword:
                case SyntaxKind.ExplicitKeyword: break;
                default: throw new ArgumentException(nameof(implicitOrExplicitKeyword));
            }
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword) throw new ArgumentException(nameof(operatorKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (ConversionOperatorDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.ConversionOperatorDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)implicitOrExplicitKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new ConversionOperatorDeclarationSyntax instance.</summary>
        public static ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.ConversionOperatorDeclaration(attributeLists, modifiers, implicitOrExplicitKeyword, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), type, parameterList, body, expressionBody, default);

        /// <summary>Creates a new ConversionOperatorDeclarationSyntax instance.</summary>
        public static ConversionOperatorDeclarationSyntax ConversionOperatorDeclaration(SyntaxToken implicitOrExplicitKeyword, TypeSyntax type)
            => SyntaxFactory.ConversionOperatorDeclaration(default, default(SyntaxTokenList), implicitOrExplicitKeyword, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), type, SyntaxFactory.ParameterList(), default, default, default);

        /// <summary>Creates a new ConstructorDeclarationSyntax instance.</summary>
        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, ConstructorInitializerSyntax? initializer, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (ConstructorDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.ConstructorDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, initializer == null ? null : (Syntax.InternalSyntax.ConstructorInitializerSyntax)initializer.Green, body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new ConstructorDeclarationSyntax instance.</summary>
        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, ConstructorInitializerSyntax? initializer, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, body, expressionBody, default);

        /// <summary>Creates a new ConstructorDeclarationSyntax instance.</summary>
        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxToken identifier)
            => SyntaxFactory.ConstructorDeclaration(default, default(SyntaxTokenList), identifier, SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new ConstructorDeclarationSyntax instance.</summary>
        public static ConstructorDeclarationSyntax ConstructorDeclaration(string identifier)
            => SyntaxFactory.ConstructorDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Identifier(identifier), SyntaxFactory.ParameterList(), default, default, default, default);

        /// <summary>Creates a new ConstructorInitializerSyntax instance.</summary>
        public static ConstructorInitializerSyntax ConstructorInitializer(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList)
        {
            switch (kind)
            {
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.ThisConstructorInitializer: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            switch (thisOrBaseKeyword.Kind())
            {
                case SyntaxKind.BaseKeyword:
                case SyntaxKind.ThisKeyword: break;
                default: throw new ArgumentException(nameof(thisOrBaseKeyword));
            }
            if (argumentList == null) throw new ArgumentNullException(nameof(argumentList));
            return (ConstructorInitializerSyntax)Syntax.InternalSyntax.SyntaxFactory.ConstructorInitializer(kind, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!, (Syntax.InternalSyntax.SyntaxToken)thisOrBaseKeyword.Node!, (Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green).CreateRed();
        }

        /// <summary>Creates a new ConstructorInitializerSyntax instance.</summary>
        public static ConstructorInitializerSyntax ConstructorInitializer(SyntaxKind kind, ArgumentListSyntax? argumentList = default)
            => SyntaxFactory.ConstructorInitializer(kind, SyntaxFactory.Token(SyntaxKind.ColonToken), SyntaxFactory.Token(GetConstructorInitializerThisOrBaseKeywordKind(kind)), argumentList ?? SyntaxFactory.ArgumentList());

        private static SyntaxKind GetConstructorInitializerThisOrBaseKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.BaseConstructorInitializer => SyntaxKind.BaseKeyword,
                SyntaxKind.ThisConstructorInitializer => SyntaxKind.ThisKeyword,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new DestructorDeclarationSyntax instance.</summary>
        public static DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken tildeToken, SyntaxToken identifier, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (tildeToken.Kind() != SyntaxKind.TildeToken) throw new ArgumentException(nameof(tildeToken));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (DestructorDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.DestructorDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)tildeToken.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green, body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new DestructorDeclarationSyntax instance.</summary>
        public static DestructorDeclarationSyntax DestructorDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.DestructorDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.TildeToken), identifier, parameterList, body, expressionBody, default);

        /// <summary>Creates a new DestructorDeclarationSyntax instance.</summary>
        public static DestructorDeclarationSyntax DestructorDeclaration(SyntaxToken identifier)
            => SyntaxFactory.DestructorDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.TildeToken), identifier, SyntaxFactory.ParameterList(), default, default, default);

        /// <summary>Creates a new DestructorDeclarationSyntax instance.</summary>
        public static DestructorDeclarationSyntax DestructorDeclaration(string identifier)
            => SyntaxFactory.DestructorDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.TildeToken), SyntaxFactory.Identifier(identifier), SyntaxFactory.ParameterList(), default, default, default);

        /// <summary>Creates a new PropertyDeclarationSyntax instance.</summary>
        public static PropertyDeclarationSyntax PropertyDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody, EqualsValueClauseSyntax? initializer, SyntaxToken semicolonToken)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (PropertyDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.PropertyDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)type.Green, explicitInterfaceSpecifier == null ? null : (Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, accessorList == null ? null : (Syntax.InternalSyntax.AccessorListSyntax)accessorList.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, initializer == null ? null : (Syntax.InternalSyntax.EqualsValueClauseSyntax)initializer.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new PropertyDeclarationSyntax instance.</summary>
        public static PropertyDeclarationSyntax PropertyDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody, EqualsValueClauseSyntax? initializer)
            => SyntaxFactory.PropertyDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, identifier, accessorList, expressionBody, initializer, default);

        /// <summary>Creates a new PropertyDeclarationSyntax instance.</summary>
        public static PropertyDeclarationSyntax PropertyDeclaration(TypeSyntax type, SyntaxToken identifier)
            => SyntaxFactory.PropertyDeclaration(default, default(SyntaxTokenList), type, default, identifier, default, default, default, default);

        /// <summary>Creates a new PropertyDeclarationSyntax instance.</summary>
        public static PropertyDeclarationSyntax PropertyDeclaration(TypeSyntax type, string identifier)
            => SyntaxFactory.PropertyDeclaration(default, default(SyntaxTokenList), type, default, SyntaxFactory.Identifier(identifier), default, default, default, default);

        /// <summary>Creates a new ArrowExpressionClauseSyntax instance.</summary>
        public static ArrowExpressionClauseSyntax ArrowExpressionClause(SyntaxToken arrowToken, ExpressionSyntax expression)
        {
            if (arrowToken.Kind() != SyntaxKind.EqualsGreaterThanToken) throw new ArgumentException(nameof(arrowToken));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return (ArrowExpressionClauseSyntax)Syntax.InternalSyntax.SyntaxFactory.ArrowExpressionClause((Syntax.InternalSyntax.SyntaxToken)arrowToken.Node!, (Syntax.InternalSyntax.ExpressionSyntax)expression.Green).CreateRed();
        }

        /// <summary>Creates a new ArrowExpressionClauseSyntax instance.</summary>
        public static ArrowExpressionClauseSyntax ArrowExpressionClause(ExpressionSyntax expression)
            => SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), expression);

        /// <summary>Creates a new EventDeclarationSyntax instance.</summary>
        public static EventDeclarationSyntax EventDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken semicolonToken)
        {
            if (eventKeyword.Kind() != SyntaxKind.EventKeyword) throw new ArgumentException(nameof(eventKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (identifier.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(identifier));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (EventDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.EventDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)eventKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, explicitInterfaceSpecifier == null ? null : (Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, accessorList == null ? null : (Syntax.InternalSyntax.AccessorListSyntax)accessorList.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new EventDeclarationSyntax instance.</summary>
        public static EventDeclarationSyntax EventDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList)
            => SyntaxFactory.EventDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.EventKeyword), type, explicitInterfaceSpecifier, identifier, accessorList, default);

        /// <summary>Creates a new EventDeclarationSyntax instance.</summary>
        public static EventDeclarationSyntax EventDeclaration(TypeSyntax type, SyntaxToken identifier)
            => SyntaxFactory.EventDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.EventKeyword), type, default, identifier, default, default);

        /// <summary>Creates a new EventDeclarationSyntax instance.</summary>
        public static EventDeclarationSyntax EventDeclaration(TypeSyntax type, string identifier)
            => SyntaxFactory.EventDeclaration(default, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.EventKeyword), type, default, SyntaxFactory.Identifier(identifier), default, default);

        /// <summary>Creates a new IndexerDeclarationSyntax instance.</summary>
        public static IndexerDeclarationSyntax IndexerDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken thisKeyword, BracketedParameterListSyntax parameterList, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (thisKeyword.Kind() != SyntaxKind.ThisKeyword) throw new ArgumentException(nameof(thisKeyword));
            if (parameterList == null) throw new ArgumentNullException(nameof(parameterList));
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (IndexerDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.IndexerDeclaration(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)type.Green, explicitInterfaceSpecifier == null ? null : (Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)explicitInterfaceSpecifier.Green, (Syntax.InternalSyntax.SyntaxToken)thisKeyword.Node!, (Syntax.InternalSyntax.BracketedParameterListSyntax)parameterList.Green, accessorList == null ? null : (Syntax.InternalSyntax.AccessorListSyntax)accessorList.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new IndexerDeclarationSyntax instance.</summary>
        public static IndexerDeclarationSyntax IndexerDeclaration(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, BracketedParameterListSyntax parameterList, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.IndexerDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, SyntaxFactory.Token(SyntaxKind.ThisKeyword), parameterList, accessorList, expressionBody, default);

        /// <summary>Creates a new IndexerDeclarationSyntax instance.</summary>
        public static IndexerDeclarationSyntax IndexerDeclaration(TypeSyntax type)
            => SyntaxFactory.IndexerDeclaration(default, default(SyntaxTokenList), type, default, SyntaxFactory.Token(SyntaxKind.ThisKeyword), SyntaxFactory.BracketedParameterList(), default, default, default);

        /// <summary>Creates a new AccessorListSyntax instance.</summary>
        public static AccessorListSyntax AccessorList(SyntaxToken openBraceToken, SyntaxList<AccessorDeclarationSyntax> accessors, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.Kind() != SyntaxKind.OpenBraceToken) throw new ArgumentException(nameof(openBraceToken));
            if (closeBraceToken.Kind() != SyntaxKind.CloseBraceToken) throw new ArgumentException(nameof(closeBraceToken));
            return (AccessorListSyntax)Syntax.InternalSyntax.SyntaxFactory.AccessorList((Syntax.InternalSyntax.SyntaxToken)openBraceToken.Node!, accessors.Node.ToGreenList<Syntax.InternalSyntax.AccessorDeclarationSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBraceToken.Node!).CreateRed();
        }

        /// <summary>Creates a new AccessorListSyntax instance.</summary>
        public static AccessorListSyntax AccessorList(SyntaxList<AccessorDeclarationSyntax> accessors = default)
            => SyntaxFactory.AccessorList(SyntaxFactory.Token(SyntaxKind.OpenBraceToken), accessors, SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        /// <summary>Creates a new AccessorDeclarationSyntax instance.</summary>
        public static AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            switch (kind)
            {
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration: break;
                default: throw new ArgumentException(nameof(kind));
            }
            switch (keyword.Kind())
            {
                case SyntaxKind.GetKeyword:
                case SyntaxKind.SetKeyword:
                case SyntaxKind.InitKeyword:
                case SyntaxKind.AddKeyword:
                case SyntaxKind.RemoveKeyword:
                case SyntaxKind.IdentifierToken: break;
                default: throw new ArgumentException(nameof(keyword));
            }
            switch (semicolonToken.Kind())
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(semicolonToken));
            }
            return (AccessorDeclarationSyntax)Syntax.InternalSyntax.SyntaxFactory.AccessorDeclaration(kind, attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)keyword.Node!, body == null ? null : (Syntax.InternalSyntax.BlockSyntax)body.Green, expressionBody == null ? null : (Syntax.InternalSyntax.ArrowExpressionClauseSyntax)expressionBody.Green, (Syntax.InternalSyntax.SyntaxToken?)semicolonToken.Node).CreateRed();
        }

        /// <summary>Creates a new AccessorDeclarationSyntax instance.</summary>
        public static AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody)
            => SyntaxFactory.AccessorDeclaration(kind, attributeLists, modifiers, SyntaxFactory.Token(GetAccessorDeclarationKeywordKind(kind)), body, expressionBody, default);

        /// <summary>Creates a new AccessorDeclarationSyntax instance.</summary>
        public static AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind)
            => SyntaxFactory.AccessorDeclaration(kind, default, default(SyntaxTokenList), SyntaxFactory.Token(GetAccessorDeclarationKeywordKind(kind)), default, default, default);

        private static SyntaxKind GetAccessorDeclarationKeywordKind(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.GetAccessorDeclaration => SyntaxKind.GetKeyword,
                SyntaxKind.SetAccessorDeclaration => SyntaxKind.SetKeyword,
                SyntaxKind.InitAccessorDeclaration => SyntaxKind.InitKeyword,
                SyntaxKind.AddAccessorDeclaration => SyntaxKind.AddKeyword,
                SyntaxKind.RemoveAccessorDeclaration => SyntaxKind.RemoveKeyword,
                SyntaxKind.UnknownAccessorDeclaration => SyntaxKind.IdentifierToken,
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>Creates a new ParameterListSyntax instance.</summary>
        public static ParameterListSyntax ParameterList(SyntaxToken openParenToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (ParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.ParameterList((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new ParameterListSyntax instance.</summary>
        public static ParameterListSyntax ParameterList(SeparatedSyntaxList<ParameterSyntax> parameters = default)
            => SyntaxFactory.ParameterList(SyntaxFactory.Token(SyntaxKind.OpenParenToken), parameters, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new BracketedParameterListSyntax instance.</summary>
        public static BracketedParameterListSyntax BracketedParameterList(SyntaxToken openBracketToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (BracketedParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.BracketedParameterList((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new BracketedParameterListSyntax instance.</summary>
        public static BracketedParameterListSyntax BracketedParameterList(SeparatedSyntaxList<ParameterSyntax> parameters = default)
            => SyntaxFactory.BracketedParameterList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), parameters, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new ParameterSyntax instance.</summary>
        public static ParameterSyntax Parameter(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default)
        {
            switch (identifier.Kind())
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.ArgListKeyword: break;
                default: throw new ArgumentException(nameof(identifier));
            }
            return (ParameterSyntax)Syntax.InternalSyntax.SyntaxFactory.Parameter(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), type == null ? null : (Syntax.InternalSyntax.TypeSyntax)type.Green, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, @default == null ? null : (Syntax.InternalSyntax.EqualsValueClauseSyntax)@default.Green).CreateRed();
        }

        /// <summary>Creates a new ParameterSyntax instance.</summary>
        public static ParameterSyntax Parameter(SyntaxToken identifier)
            => SyntaxFactory.Parameter(default, default(SyntaxTokenList), default, identifier, default);

        /// <summary>Creates a new FunctionPointerParameterSyntax instance.</summary>
        public static FunctionPointerParameterSyntax FunctionPointerParameter(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (FunctionPointerParameterSyntax)Syntax.InternalSyntax.SyntaxFactory.FunctionPointerParameter(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new FunctionPointerParameterSyntax instance.</summary>
        public static FunctionPointerParameterSyntax FunctionPointerParameter(TypeSyntax type)
            => SyntaxFactory.FunctionPointerParameter(default, default(SyntaxTokenList), type);

        /// <summary>Creates a new IncompleteMemberSyntax instance.</summary>
        public static IncompleteMemberSyntax IncompleteMember(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax? type)
        {
            return (IncompleteMemberSyntax)Syntax.InternalSyntax.SyntaxFactory.IncompleteMember(attributeLists.Node.ToGreenList<Syntax.InternalSyntax.AttributeListSyntax>(), modifiers.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), type == null ? null : (Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

#pragma warning disable RS0027
        /// <summary>Creates a new IncompleteMemberSyntax instance.</summary>
        public static IncompleteMemberSyntax IncompleteMember(TypeSyntax? type = default)
            => SyntaxFactory.IncompleteMember(default, default(SyntaxTokenList), type);
#pragma warning restore RS0027

        /// <summary>Creates a new SkippedTokensTriviaSyntax instance.</summary>
        public static SkippedTokensTriviaSyntax SkippedTokensTrivia(SyntaxTokenList tokens)
        {
            return (SkippedTokensTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.SkippedTokensTrivia(tokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>()).CreateRed();
        }

        /// <summary>Creates a new SkippedTokensTriviaSyntax instance.</summary>
        public static SkippedTokensTriviaSyntax SkippedTokensTrivia()
            => SyntaxFactory.SkippedTokensTrivia(default(SyntaxTokenList));

        /// <summary>Creates a new DocumentationCommentTriviaSyntax instance.</summary>
        public static DocumentationCommentTriviaSyntax DocumentationCommentTrivia(SyntaxKind kind, SyntaxList<XmlNodeSyntax> content, SyntaxToken endOfComment)
        {
            switch (kind)
            {
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia: break;
                default: throw new ArgumentException(nameof(kind));
            }
            if (endOfComment.Kind() != SyntaxKind.EndOfDocumentationCommentToken) throw new ArgumentException(nameof(endOfComment));
            return (DocumentationCommentTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentTrivia(kind, content.Node.ToGreenList<Syntax.InternalSyntax.XmlNodeSyntax>(), (Syntax.InternalSyntax.SyntaxToken)endOfComment.Node!).CreateRed();
        }

        /// <summary>Creates a new DocumentationCommentTriviaSyntax instance.</summary>
        public static DocumentationCommentTriviaSyntax DocumentationCommentTrivia(SyntaxKind kind, SyntaxList<XmlNodeSyntax> content = default)
            => SyntaxFactory.DocumentationCommentTrivia(kind, content, SyntaxFactory.Token(SyntaxKind.EndOfDocumentationCommentToken));

        /// <summary>Creates a new TypeCrefSyntax instance.</summary>
        public static TypeCrefSyntax TypeCref(TypeSyntax type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (TypeCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.TypeCref((Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new QualifiedCrefSyntax instance.</summary>
        public static QualifiedCrefSyntax QualifiedCref(TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (dotToken.Kind() != SyntaxKind.DotToken) throw new ArgumentException(nameof(dotToken));
            if (member == null) throw new ArgumentNullException(nameof(member));
            return (QualifiedCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.QualifiedCref((Syntax.InternalSyntax.TypeSyntax)container.Green, (Syntax.InternalSyntax.SyntaxToken)dotToken.Node!, (Syntax.InternalSyntax.MemberCrefSyntax)member.Green).CreateRed();
        }

        /// <summary>Creates a new QualifiedCrefSyntax instance.</summary>
        public static QualifiedCrefSyntax QualifiedCref(TypeSyntax container, MemberCrefSyntax member)
            => SyntaxFactory.QualifiedCref(container, SyntaxFactory.Token(SyntaxKind.DotToken), member);

        /// <summary>Creates a new NameMemberCrefSyntax instance.</summary>
        public static NameMemberCrefSyntax NameMemberCref(TypeSyntax name, CrefParameterListSyntax? parameters)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return (NameMemberCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.NameMemberCref((Syntax.InternalSyntax.TypeSyntax)name.Green, parameters == null ? null : (Syntax.InternalSyntax.CrefParameterListSyntax)parameters.Green).CreateRed();
        }

        /// <summary>Creates a new NameMemberCrefSyntax instance.</summary>
        public static NameMemberCrefSyntax NameMemberCref(TypeSyntax name)
            => SyntaxFactory.NameMemberCref(name, default);

        /// <summary>Creates a new IndexerMemberCrefSyntax instance.</summary>
        public static IndexerMemberCrefSyntax IndexerMemberCref(SyntaxToken thisKeyword, CrefBracketedParameterListSyntax? parameters)
        {
            if (thisKeyword.Kind() != SyntaxKind.ThisKeyword) throw new ArgumentException(nameof(thisKeyword));
            return (IndexerMemberCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.IndexerMemberCref((Syntax.InternalSyntax.SyntaxToken)thisKeyword.Node!, parameters == null ? null : (Syntax.InternalSyntax.CrefBracketedParameterListSyntax)parameters.Green).CreateRed();
        }

        /// <summary>Creates a new IndexerMemberCrefSyntax instance.</summary>
        public static IndexerMemberCrefSyntax IndexerMemberCref(CrefBracketedParameterListSyntax? parameters = default)
            => SyntaxFactory.IndexerMemberCref(SyntaxFactory.Token(SyntaxKind.ThisKeyword), parameters);

        /// <summary>Creates a new OperatorMemberCrefSyntax instance.</summary>
        public static OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax? parameters)
        {
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword) throw new ArgumentException(nameof(operatorKeyword));
            switch (operatorToken.Kind())
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.TildeToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword: break;
                default: throw new ArgumentException(nameof(operatorToken));
            }
            return (OperatorMemberCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.OperatorMemberCref((Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)operatorToken.Node!, parameters == null ? null : (Syntax.InternalSyntax.CrefParameterListSyntax)parameters.Green).CreateRed();
        }

        /// <summary>Creates a new OperatorMemberCrefSyntax instance.</summary>
        public static OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorToken, CrefParameterListSyntax? parameters)
            => SyntaxFactory.OperatorMemberCref(SyntaxFactory.Token(SyntaxKind.OperatorKeyword), operatorToken, parameters);

        /// <summary>Creates a new OperatorMemberCrefSyntax instance.</summary>
        public static OperatorMemberCrefSyntax OperatorMemberCref(SyntaxToken operatorToken)
            => SyntaxFactory.OperatorMemberCref(SyntaxFactory.Token(SyntaxKind.OperatorKeyword), operatorToken, default);

        /// <summary>Creates a new ConversionOperatorMemberCrefSyntax instance.</summary>
        public static ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters)
        {
            switch (implicitOrExplicitKeyword.Kind())
            {
                case SyntaxKind.ImplicitKeyword:
                case SyntaxKind.ExplicitKeyword: break;
                default: throw new ArgumentException(nameof(implicitOrExplicitKeyword));
            }
            if (operatorKeyword.Kind() != SyntaxKind.OperatorKeyword) throw new ArgumentException(nameof(operatorKeyword));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (ConversionOperatorMemberCrefSyntax)Syntax.InternalSyntax.SyntaxFactory.ConversionOperatorMemberCref((Syntax.InternalSyntax.SyntaxToken)implicitOrExplicitKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)operatorKeyword.Node!, (Syntax.InternalSyntax.TypeSyntax)type.Green, parameters == null ? null : (Syntax.InternalSyntax.CrefParameterListSyntax)parameters.Green).CreateRed();
        }

        /// <summary>Creates a new ConversionOperatorMemberCrefSyntax instance.</summary>
        public static ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, TypeSyntax type, CrefParameterListSyntax? parameters)
            => SyntaxFactory.ConversionOperatorMemberCref(implicitOrExplicitKeyword, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), type, parameters);

        /// <summary>Creates a new ConversionOperatorMemberCrefSyntax instance.</summary>
        public static ConversionOperatorMemberCrefSyntax ConversionOperatorMemberCref(SyntaxToken implicitOrExplicitKeyword, TypeSyntax type)
            => SyntaxFactory.ConversionOperatorMemberCref(implicitOrExplicitKeyword, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), type, default);

        /// <summary>Creates a new CrefParameterListSyntax instance.</summary>
        public static CrefParameterListSyntax CrefParameterList(SyntaxToken openParenToken, SeparatedSyntaxList<CrefParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken.Kind() != SyntaxKind.OpenParenToken) throw new ArgumentException(nameof(openParenToken));
            if (closeParenToken.Kind() != SyntaxKind.CloseParenToken) throw new ArgumentException(nameof(closeParenToken));
            return (CrefParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.CrefParameterList((Syntax.InternalSyntax.SyntaxToken)openParenToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.CrefParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeParenToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CrefParameterListSyntax instance.</summary>
        public static CrefParameterListSyntax CrefParameterList(SeparatedSyntaxList<CrefParameterSyntax> parameters = default)
            => SyntaxFactory.CrefParameterList(SyntaxFactory.Token(SyntaxKind.OpenParenToken), parameters, SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        /// <summary>Creates a new CrefBracketedParameterListSyntax instance.</summary>
        public static CrefBracketedParameterListSyntax CrefBracketedParameterList(SyntaxToken openBracketToken, SeparatedSyntaxList<CrefParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken.Kind() != SyntaxKind.OpenBracketToken) throw new ArgumentException(nameof(openBracketToken));
            if (closeBracketToken.Kind() != SyntaxKind.CloseBracketToken) throw new ArgumentException(nameof(closeBracketToken));
            return (CrefBracketedParameterListSyntax)Syntax.InternalSyntax.SyntaxFactory.CrefBracketedParameterList((Syntax.InternalSyntax.SyntaxToken)openBracketToken.Node!, parameters.Node.ToGreenSeparatedList<Syntax.InternalSyntax.CrefParameterSyntax>(), (Syntax.InternalSyntax.SyntaxToken)closeBracketToken.Node!).CreateRed();
        }

        /// <summary>Creates a new CrefBracketedParameterListSyntax instance.</summary>
        public static CrefBracketedParameterListSyntax CrefBracketedParameterList(SeparatedSyntaxList<CrefParameterSyntax> parameters = default)
            => SyntaxFactory.CrefBracketedParameterList(SyntaxFactory.Token(SyntaxKind.OpenBracketToken), parameters, SyntaxFactory.Token(SyntaxKind.CloseBracketToken));

        /// <summary>Creates a new CrefParameterSyntax instance.</summary>
        public static CrefParameterSyntax CrefParameter(SyntaxToken refKindKeyword, TypeSyntax type)
        {
            switch (refKindKeyword.Kind())
            {
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(refKindKeyword));
            }
            if (type == null) throw new ArgumentNullException(nameof(type));
            return (CrefParameterSyntax)Syntax.InternalSyntax.SyntaxFactory.CrefParameter((Syntax.InternalSyntax.SyntaxToken?)refKindKeyword.Node, (Syntax.InternalSyntax.TypeSyntax)type.Green).CreateRed();
        }

        /// <summary>Creates a new CrefParameterSyntax instance.</summary>
        public static CrefParameterSyntax CrefParameter(TypeSyntax type)
            => SyntaxFactory.CrefParameter(default, type);

        /// <summary>Creates a new XmlElementSyntax instance.</summary>
        public static XmlElementSyntax XmlElement(XmlElementStartTagSyntax startTag, SyntaxList<XmlNodeSyntax> content, XmlElementEndTagSyntax endTag)
        {
            if (startTag == null) throw new ArgumentNullException(nameof(startTag));
            if (endTag == null) throw new ArgumentNullException(nameof(endTag));
            return (XmlElementSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlElement((Syntax.InternalSyntax.XmlElementStartTagSyntax)startTag.Green, content.Node.ToGreenList<Syntax.InternalSyntax.XmlNodeSyntax>(), (Syntax.InternalSyntax.XmlElementEndTagSyntax)endTag.Green).CreateRed();
        }

        /// <summary>Creates a new XmlElementSyntax instance.</summary>
        public static XmlElementSyntax XmlElement(XmlElementStartTagSyntax startTag, XmlElementEndTagSyntax endTag)
            => SyntaxFactory.XmlElement(startTag, default, endTag);

        /// <summary>Creates a new XmlElementStartTagSyntax instance.</summary>
        public static XmlElementStartTagSyntax XmlElementStartTag(SyntaxToken lessThanToken, XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes, SyntaxToken greaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken) throw new ArgumentException(nameof(lessThanToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken) throw new ArgumentException(nameof(greaterThanToken));
            return (XmlElementStartTagSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlElementStartTag((Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node!, (Syntax.InternalSyntax.XmlNameSyntax)name.Green, attributes.Node.ToGreenList<Syntax.InternalSyntax.XmlAttributeSyntax>(), (Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlElementStartTagSyntax instance.</summary>
        public static XmlElementStartTagSyntax XmlElementStartTag(XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes)
            => SyntaxFactory.XmlElementStartTag(SyntaxFactory.Token(SyntaxKind.LessThanToken), name, attributes, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new XmlElementStartTagSyntax instance.</summary>
        public static XmlElementStartTagSyntax XmlElementStartTag(XmlNameSyntax name)
            => SyntaxFactory.XmlElementStartTag(SyntaxFactory.Token(SyntaxKind.LessThanToken), name, default, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new XmlElementEndTagSyntax instance.</summary>
        public static XmlElementEndTagSyntax XmlElementEndTag(SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
        {
            if (lessThanSlashToken.Kind() != SyntaxKind.LessThanSlashToken) throw new ArgumentException(nameof(lessThanSlashToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (greaterThanToken.Kind() != SyntaxKind.GreaterThanToken) throw new ArgumentException(nameof(greaterThanToken));
            return (XmlElementEndTagSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlElementEndTag((Syntax.InternalSyntax.SyntaxToken)lessThanSlashToken.Node!, (Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)greaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlElementEndTagSyntax instance.</summary>
        public static XmlElementEndTagSyntax XmlElementEndTag(XmlNameSyntax name)
            => SyntaxFactory.XmlElementEndTag(SyntaxFactory.Token(SyntaxKind.LessThanSlashToken), name, SyntaxFactory.Token(SyntaxKind.GreaterThanToken));

        /// <summary>Creates a new XmlEmptyElementSyntax instance.</summary>
        public static XmlEmptyElementSyntax XmlEmptyElement(SyntaxToken lessThanToken, XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes, SyntaxToken slashGreaterThanToken)
        {
            if (lessThanToken.Kind() != SyntaxKind.LessThanToken) throw new ArgumentException(nameof(lessThanToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (slashGreaterThanToken.Kind() != SyntaxKind.SlashGreaterThanToken) throw new ArgumentException(nameof(slashGreaterThanToken));
            return (XmlEmptyElementSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlEmptyElement((Syntax.InternalSyntax.SyntaxToken)lessThanToken.Node!, (Syntax.InternalSyntax.XmlNameSyntax)name.Green, attributes.Node.ToGreenList<Syntax.InternalSyntax.XmlAttributeSyntax>(), (Syntax.InternalSyntax.SyntaxToken)slashGreaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlEmptyElementSyntax instance.</summary>
        public static XmlEmptyElementSyntax XmlEmptyElement(XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes)
            => SyntaxFactory.XmlEmptyElement(SyntaxFactory.Token(SyntaxKind.LessThanToken), name, attributes, SyntaxFactory.Token(SyntaxKind.SlashGreaterThanToken));

        /// <summary>Creates a new XmlEmptyElementSyntax instance.</summary>
        public static XmlEmptyElementSyntax XmlEmptyElement(XmlNameSyntax name)
            => SyntaxFactory.XmlEmptyElement(SyntaxFactory.Token(SyntaxKind.LessThanToken), name, default, SyntaxFactory.Token(SyntaxKind.SlashGreaterThanToken));

        /// <summary>Creates a new XmlNameSyntax instance.</summary>
        public static XmlNameSyntax XmlName(XmlPrefixSyntax? prefix, SyntaxToken localName)
        {
            if (localName.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(localName));
            return (XmlNameSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlName(prefix == null ? null : (Syntax.InternalSyntax.XmlPrefixSyntax)prefix.Green, (Syntax.InternalSyntax.SyntaxToken)localName.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlNameSyntax instance.</summary>
        public static XmlNameSyntax XmlName(SyntaxToken localName)
            => SyntaxFactory.XmlName(default, localName);

        /// <summary>Creates a new XmlNameSyntax instance.</summary>
        public static XmlNameSyntax XmlName(string localName)
            => SyntaxFactory.XmlName(default, SyntaxFactory.Identifier(localName));

        /// <summary>Creates a new XmlPrefixSyntax instance.</summary>
        public static XmlPrefixSyntax XmlPrefix(SyntaxToken prefix, SyntaxToken colonToken)
        {
            if (prefix.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(prefix));
            if (colonToken.Kind() != SyntaxKind.ColonToken) throw new ArgumentException(nameof(colonToken));
            return (XmlPrefixSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlPrefix((Syntax.InternalSyntax.SyntaxToken)prefix.Node!, (Syntax.InternalSyntax.SyntaxToken)colonToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlPrefixSyntax instance.</summary>
        public static XmlPrefixSyntax XmlPrefix(SyntaxToken prefix)
            => SyntaxFactory.XmlPrefix(prefix, SyntaxFactory.Token(SyntaxKind.ColonToken));

        /// <summary>Creates a new XmlPrefixSyntax instance.</summary>
        public static XmlPrefixSyntax XmlPrefix(string prefix)
            => SyntaxFactory.XmlPrefix(SyntaxFactory.Identifier(prefix), SyntaxFactory.Token(SyntaxKind.ColonToken));

        /// <summary>Creates a new XmlTextAttributeSyntax instance.</summary>
        public static XmlTextAttributeSyntax XmlTextAttribute(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            switch (startQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(startQuoteToken));
            }
            switch (endQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(endQuoteToken));
            }
            return (XmlTextAttributeSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlTextAttribute((Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!, (Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node!, textTokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlTextAttributeSyntax instance.</summary>
        public static XmlTextAttributeSyntax XmlTextAttribute(XmlNameSyntax name, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
            => SyntaxFactory.XmlTextAttribute(name, SyntaxFactory.Token(SyntaxKind.EqualsToken), startQuoteToken, textTokens, endQuoteToken);

        /// <summary>Creates a new XmlTextAttributeSyntax instance.</summary>
        public static XmlTextAttributeSyntax XmlTextAttribute(XmlNameSyntax name, SyntaxToken startQuoteToken, SyntaxToken endQuoteToken)
            => SyntaxFactory.XmlTextAttribute(name, SyntaxFactory.Token(SyntaxKind.EqualsToken), startQuoteToken, default(SyntaxTokenList), endQuoteToken);

        /// <summary>Creates a new XmlCrefAttributeSyntax instance.</summary>
        public static XmlCrefAttributeSyntax XmlCrefAttribute(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            switch (startQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(startQuoteToken));
            }
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            switch (endQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(endQuoteToken));
            }
            return (XmlCrefAttributeSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlCrefAttribute((Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!, (Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node!, (Syntax.InternalSyntax.CrefSyntax)cref.Green, (Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlCrefAttributeSyntax instance.</summary>
        public static XmlCrefAttributeSyntax XmlCrefAttribute(XmlNameSyntax name, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken)
            => SyntaxFactory.XmlCrefAttribute(name, SyntaxFactory.Token(SyntaxKind.EqualsToken), startQuoteToken, cref, endQuoteToken);

        /// <summary>Creates a new XmlNameAttributeSyntax instance.</summary>
        public static XmlNameAttributeSyntax XmlNameAttribute(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (equalsToken.Kind() != SyntaxKind.EqualsToken) throw new ArgumentException(nameof(equalsToken));
            switch (startQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(startQuoteToken));
            }
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));
            switch (endQuoteToken.Kind())
            {
                case SyntaxKind.SingleQuoteToken:
                case SyntaxKind.DoubleQuoteToken: break;
                default: throw new ArgumentException(nameof(endQuoteToken));
            }
            return (XmlNameAttributeSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlNameAttribute((Syntax.InternalSyntax.XmlNameSyntax)name.Green, (Syntax.InternalSyntax.SyntaxToken)equalsToken.Node!, (Syntax.InternalSyntax.SyntaxToken)startQuoteToken.Node!, (Syntax.InternalSyntax.IdentifierNameSyntax)identifier.Green, (Syntax.InternalSyntax.SyntaxToken)endQuoteToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlNameAttributeSyntax instance.</summary>
        public static XmlNameAttributeSyntax XmlNameAttribute(XmlNameSyntax name, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
            => SyntaxFactory.XmlNameAttribute(name, SyntaxFactory.Token(SyntaxKind.EqualsToken), startQuoteToken, identifier, endQuoteToken);

        /// <summary>Creates a new XmlNameAttributeSyntax instance.</summary>
        public static XmlNameAttributeSyntax XmlNameAttribute(XmlNameSyntax name, SyntaxToken startQuoteToken, string identifier, SyntaxToken endQuoteToken)
            => SyntaxFactory.XmlNameAttribute(name, SyntaxFactory.Token(SyntaxKind.EqualsToken), startQuoteToken, SyntaxFactory.IdentifierName(identifier), endQuoteToken);

        /// <summary>Creates a new XmlTextSyntax instance.</summary>
        public static XmlTextSyntax XmlText(SyntaxTokenList textTokens)
        {
            return (XmlTextSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlText(textTokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>()).CreateRed();
        }

        /// <summary>Creates a new XmlTextSyntax instance.</summary>
        public static XmlTextSyntax XmlText()
            => SyntaxFactory.XmlText(default(SyntaxTokenList));

        /// <summary>Creates a new XmlCDataSectionSyntax instance.</summary>
        public static XmlCDataSectionSyntax XmlCDataSection(SyntaxToken startCDataToken, SyntaxTokenList textTokens, SyntaxToken endCDataToken)
        {
            if (startCDataToken.Kind() != SyntaxKind.XmlCDataStartToken) throw new ArgumentException(nameof(startCDataToken));
            if (endCDataToken.Kind() != SyntaxKind.XmlCDataEndToken) throw new ArgumentException(nameof(endCDataToken));
            return (XmlCDataSectionSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlCDataSection((Syntax.InternalSyntax.SyntaxToken)startCDataToken.Node!, textTokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)endCDataToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlCDataSectionSyntax instance.</summary>
        public static XmlCDataSectionSyntax XmlCDataSection(SyntaxTokenList textTokens = default)
            => SyntaxFactory.XmlCDataSection(SyntaxFactory.Token(SyntaxKind.XmlCDataStartToken), textTokens, SyntaxFactory.Token(SyntaxKind.XmlCDataEndToken));

        /// <summary>Creates a new XmlProcessingInstructionSyntax instance.</summary>
        public static XmlProcessingInstructionSyntax XmlProcessingInstruction(SyntaxToken startProcessingInstructionToken, XmlNameSyntax name, SyntaxTokenList textTokens, SyntaxToken endProcessingInstructionToken)
        {
            if (startProcessingInstructionToken.Kind() != SyntaxKind.XmlProcessingInstructionStartToken) throw new ArgumentException(nameof(startProcessingInstructionToken));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (endProcessingInstructionToken.Kind() != SyntaxKind.XmlProcessingInstructionEndToken) throw new ArgumentException(nameof(endProcessingInstructionToken));
            return (XmlProcessingInstructionSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlProcessingInstruction((Syntax.InternalSyntax.SyntaxToken)startProcessingInstructionToken.Node!, (Syntax.InternalSyntax.XmlNameSyntax)name.Green, textTokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)endProcessingInstructionToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlProcessingInstructionSyntax instance.</summary>
        public static XmlProcessingInstructionSyntax XmlProcessingInstruction(XmlNameSyntax name, SyntaxTokenList textTokens)
            => SyntaxFactory.XmlProcessingInstruction(SyntaxFactory.Token(SyntaxKind.XmlProcessingInstructionStartToken), name, textTokens, SyntaxFactory.Token(SyntaxKind.XmlProcessingInstructionEndToken));

        /// <summary>Creates a new XmlProcessingInstructionSyntax instance.</summary>
        public static XmlProcessingInstructionSyntax XmlProcessingInstruction(XmlNameSyntax name)
            => SyntaxFactory.XmlProcessingInstruction(SyntaxFactory.Token(SyntaxKind.XmlProcessingInstructionStartToken), name, default(SyntaxTokenList), SyntaxFactory.Token(SyntaxKind.XmlProcessingInstructionEndToken));

        /// <summary>Creates a new XmlCommentSyntax instance.</summary>
        public static XmlCommentSyntax XmlComment(SyntaxToken lessThanExclamationMinusMinusToken, SyntaxTokenList textTokens, SyntaxToken minusMinusGreaterThanToken)
        {
            if (lessThanExclamationMinusMinusToken.Kind() != SyntaxKind.XmlCommentStartToken) throw new ArgumentException(nameof(lessThanExclamationMinusMinusToken));
            if (minusMinusGreaterThanToken.Kind() != SyntaxKind.XmlCommentEndToken) throw new ArgumentException(nameof(minusMinusGreaterThanToken));
            return (XmlCommentSyntax)Syntax.InternalSyntax.SyntaxFactory.XmlComment((Syntax.InternalSyntax.SyntaxToken)lessThanExclamationMinusMinusToken.Node!, textTokens.Node.ToGreenList<Syntax.InternalSyntax.SyntaxToken>(), (Syntax.InternalSyntax.SyntaxToken)minusMinusGreaterThanToken.Node!).CreateRed();
        }

        /// <summary>Creates a new XmlCommentSyntax instance.</summary>
        public static XmlCommentSyntax XmlComment(SyntaxTokenList textTokens = default)
            => SyntaxFactory.XmlComment(SyntaxFactory.Token(SyntaxKind.XmlCommentStartToken), textTokens, SyntaxFactory.Token(SyntaxKind.XmlCommentEndToken));

        /// <summary>Creates a new IfDirectiveTriviaSyntax instance.</summary>
        public static IfDirectiveTriviaSyntax IfDirectiveTrivia(SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (ifKeyword.Kind() != SyntaxKind.IfKeyword) throw new ArgumentException(nameof(ifKeyword));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (IfDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.IfDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)ifKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive, branchTaken, conditionValue).CreateRed();
        }

        /// <summary>Creates a new IfDirectiveTriviaSyntax instance.</summary>
        public static IfDirectiveTriviaSyntax IfDirectiveTrivia(ExpressionSyntax condition, bool isActive, bool branchTaken, bool conditionValue)
            => SyntaxFactory.IfDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.IfKeyword), condition, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken, conditionValue);

        /// <summary>Creates a new ElifDirectiveTriviaSyntax instance.</summary>
        public static ElifDirectiveTriviaSyntax ElifDirectiveTrivia(SyntaxToken hashToken, SyntaxToken elifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (elifKeyword.Kind() != SyntaxKind.ElifKeyword) throw new ArgumentException(nameof(elifKeyword));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (ElifDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.ElifDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)elifKeyword.Node!, (Syntax.InternalSyntax.ExpressionSyntax)condition.Green, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive, branchTaken, conditionValue).CreateRed();
        }

        /// <summary>Creates a new ElifDirectiveTriviaSyntax instance.</summary>
        public static ElifDirectiveTriviaSyntax ElifDirectiveTrivia(ExpressionSyntax condition, bool isActive, bool branchTaken, bool conditionValue)
            => SyntaxFactory.ElifDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.ElifKeyword), condition, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken, conditionValue);

        /// <summary>Creates a new ElseDirectiveTriviaSyntax instance.</summary>
        public static ElseDirectiveTriviaSyntax ElseDirectiveTrivia(SyntaxToken hashToken, SyntaxToken elseKeyword, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (elseKeyword.Kind() != SyntaxKind.ElseKeyword) throw new ArgumentException(nameof(elseKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (ElseDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.ElseDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)elseKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive, branchTaken).CreateRed();
        }

        /// <summary>Creates a new ElseDirectiveTriviaSyntax instance.</summary>
        public static ElseDirectiveTriviaSyntax ElseDirectiveTrivia(bool isActive, bool branchTaken)
            => SyntaxFactory.ElseDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.ElseKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive, branchTaken);

        /// <summary>Creates a new EndIfDirectiveTriviaSyntax instance.</summary>
        public static EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (endIfKeyword.Kind() != SyntaxKind.EndIfKeyword) throw new ArgumentException(nameof(endIfKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (EndIfDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.EndIfDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)endIfKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new EndIfDirectiveTriviaSyntax instance.</summary>
        public static EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(bool isActive)
            => SyntaxFactory.EndIfDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.EndIfKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new RegionDirectiveTriviaSyntax instance.</summary>
        public static RegionDirectiveTriviaSyntax RegionDirectiveTrivia(SyntaxToken hashToken, SyntaxToken regionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (regionKeyword.Kind() != SyntaxKind.RegionKeyword) throw new ArgumentException(nameof(regionKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (RegionDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.RegionDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)regionKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new RegionDirectiveTriviaSyntax instance.</summary>
        public static RegionDirectiveTriviaSyntax RegionDirectiveTrivia(bool isActive)
            => SyntaxFactory.RegionDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.RegionKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new EndRegionDirectiveTriviaSyntax instance.</summary>
        public static EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(SyntaxToken hashToken, SyntaxToken endRegionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (endRegionKeyword.Kind() != SyntaxKind.EndRegionKeyword) throw new ArgumentException(nameof(endRegionKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (EndRegionDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.EndRegionDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)endRegionKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new EndRegionDirectiveTriviaSyntax instance.</summary>
        public static EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(bool isActive)
            => SyntaxFactory.EndRegionDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.EndRegionKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new ErrorDirectiveTriviaSyntax instance.</summary>
        public static ErrorDirectiveTriviaSyntax ErrorDirectiveTrivia(SyntaxToken hashToken, SyntaxToken errorKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (errorKeyword.Kind() != SyntaxKind.ErrorKeyword) throw new ArgumentException(nameof(errorKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (ErrorDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.ErrorDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)errorKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new ErrorDirectiveTriviaSyntax instance.</summary>
        public static ErrorDirectiveTriviaSyntax ErrorDirectiveTrivia(bool isActive)
            => SyntaxFactory.ErrorDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.ErrorKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new WarningDirectiveTriviaSyntax instance.</summary>
        public static WarningDirectiveTriviaSyntax WarningDirectiveTrivia(SyntaxToken hashToken, SyntaxToken warningKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (warningKeyword.Kind() != SyntaxKind.WarningKeyword) throw new ArgumentException(nameof(warningKeyword));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (WarningDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.WarningDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)warningKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new WarningDirectiveTriviaSyntax instance.</summary>
        public static WarningDirectiveTriviaSyntax WarningDirectiveTrivia(bool isActive)
            => SyntaxFactory.WarningDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.WarningKeyword), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new BadDirectiveTriviaSyntax instance.</summary>
        public static BadDirectiveTriviaSyntax BadDirectiveTrivia(SyntaxToken hashToken, SyntaxToken identifier, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (BadDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.BadDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)identifier.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new BadDirectiveTriviaSyntax instance.</summary>
        public static BadDirectiveTriviaSyntax BadDirectiveTrivia(SyntaxToken identifier, bool isActive)
            => SyntaxFactory.BadDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), identifier, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new DefineDirectiveTriviaSyntax instance.</summary>
        public static DefineDirectiveTriviaSyntax DefineDirectiveTrivia(SyntaxToken hashToken, SyntaxToken defineKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (defineKeyword.Kind() != SyntaxKind.DefineKeyword) throw new ArgumentException(nameof(defineKeyword));
            if (name.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(name));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (DefineDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.DefineDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)defineKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)name.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new DefineDirectiveTriviaSyntax instance.</summary>
        public static DefineDirectiveTriviaSyntax DefineDirectiveTrivia(SyntaxToken name, bool isActive)
            => SyntaxFactory.DefineDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.DefineKeyword), name, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new DefineDirectiveTriviaSyntax instance.</summary>
        public static DefineDirectiveTriviaSyntax DefineDirectiveTrivia(string name, bool isActive)
            => SyntaxFactory.DefineDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.DefineKeyword), SyntaxFactory.Identifier(name), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new UndefDirectiveTriviaSyntax instance.</summary>
        public static UndefDirectiveTriviaSyntax UndefDirectiveTrivia(SyntaxToken hashToken, SyntaxToken undefKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (undefKeyword.Kind() != SyntaxKind.UndefKeyword) throw new ArgumentException(nameof(undefKeyword));
            if (name.Kind() != SyntaxKind.IdentifierToken) throw new ArgumentException(nameof(name));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (UndefDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.UndefDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)undefKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)name.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new UndefDirectiveTriviaSyntax instance.</summary>
        public static UndefDirectiveTriviaSyntax UndefDirectiveTrivia(SyntaxToken name, bool isActive)
            => SyntaxFactory.UndefDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.UndefKeyword), name, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new UndefDirectiveTriviaSyntax instance.</summary>
        public static UndefDirectiveTriviaSyntax UndefDirectiveTrivia(string name, bool isActive)
            => SyntaxFactory.UndefDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.UndefKeyword), SyntaxFactory.Identifier(name), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new LineDirectiveTriviaSyntax instance.</summary>
        public static LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (lineKeyword.Kind() != SyntaxKind.LineKeyword) throw new ArgumentException(nameof(lineKeyword));
            switch (line.Kind())
            {
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.HiddenKeyword: break;
                default: throw new ArgumentException(nameof(line));
            }
            switch (file.Kind())
            {
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(file));
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (LineDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.LineDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)lineKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)line.Node!, (Syntax.InternalSyntax.SyntaxToken?)file.Node, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new LineDirectiveTriviaSyntax instance.</summary>
        public static LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken line, SyntaxToken file, bool isActive)
            => SyntaxFactory.LineDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.LineKeyword), line, file, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new LineDirectiveTriviaSyntax instance.</summary>
        public static LineDirectiveTriviaSyntax LineDirectiveTrivia(SyntaxToken line, bool isActive)
            => SyntaxFactory.LineDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.LineKeyword), line, default, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new PragmaWarningDirectiveTriviaSyntax instance.</summary>
        public static PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, SeparatedSyntaxList<ExpressionSyntax> errorCodes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (pragmaKeyword.Kind() != SyntaxKind.PragmaKeyword) throw new ArgumentException(nameof(pragmaKeyword));
            if (warningKeyword.Kind() != SyntaxKind.WarningKeyword) throw new ArgumentException(nameof(warningKeyword));
            switch (disableOrRestoreKeyword.Kind())
            {
                case SyntaxKind.DisableKeyword:
                case SyntaxKind.RestoreKeyword: break;
                default: throw new ArgumentException(nameof(disableOrRestoreKeyword));
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (PragmaWarningDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.PragmaWarningDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)pragmaKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)warningKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)disableOrRestoreKeyword.Node!, errorCodes.Node.ToGreenSeparatedList<Syntax.InternalSyntax.ExpressionSyntax>(), (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new PragmaWarningDirectiveTriviaSyntax instance.</summary>
        public static PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken disableOrRestoreKeyword, SeparatedSyntaxList<ExpressionSyntax> errorCodes, bool isActive)
            => SyntaxFactory.PragmaWarningDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.PragmaKeyword), SyntaxFactory.Token(SyntaxKind.WarningKeyword), disableOrRestoreKeyword, errorCodes, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new PragmaWarningDirectiveTriviaSyntax instance.</summary>
        public static PragmaWarningDirectiveTriviaSyntax PragmaWarningDirectiveTrivia(SyntaxToken disableOrRestoreKeyword, bool isActive)
            => SyntaxFactory.PragmaWarningDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.PragmaKeyword), SyntaxFactory.Token(SyntaxKind.WarningKeyword), disableOrRestoreKeyword, default, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new PragmaChecksumDirectiveTriviaSyntax instance.</summary>
        public static PragmaChecksumDirectiveTriviaSyntax PragmaChecksumDirectiveTrivia(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (pragmaKeyword.Kind() != SyntaxKind.PragmaKeyword) throw new ArgumentException(nameof(pragmaKeyword));
            if (checksumKeyword.Kind() != SyntaxKind.ChecksumKeyword) throw new ArgumentException(nameof(checksumKeyword));
            if (file.Kind() != SyntaxKind.StringLiteralToken) throw new ArgumentException(nameof(file));
            if (guid.Kind() != SyntaxKind.StringLiteralToken) throw new ArgumentException(nameof(guid));
            if (bytes.Kind() != SyntaxKind.StringLiteralToken) throw new ArgumentException(nameof(bytes));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (PragmaChecksumDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.PragmaChecksumDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)pragmaKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)checksumKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)file.Node!, (Syntax.InternalSyntax.SyntaxToken)guid.Node!, (Syntax.InternalSyntax.SyntaxToken)bytes.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new PragmaChecksumDirectiveTriviaSyntax instance.</summary>
        public static PragmaChecksumDirectiveTriviaSyntax PragmaChecksumDirectiveTrivia(SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, bool isActive)
            => SyntaxFactory.PragmaChecksumDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.PragmaKeyword), SyntaxFactory.Token(SyntaxKind.ChecksumKeyword), file, guid, bytes, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new ReferenceDirectiveTriviaSyntax instance.</summary>
        public static ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(SyntaxToken hashToken, SyntaxToken referenceKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (referenceKeyword.Kind() != SyntaxKind.ReferenceKeyword) throw new ArgumentException(nameof(referenceKeyword));
            if (file.Kind() != SyntaxKind.StringLiteralToken) throw new ArgumentException(nameof(file));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (ReferenceDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.ReferenceDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)referenceKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)file.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new ReferenceDirectiveTriviaSyntax instance.</summary>
        public static ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(SyntaxToken file, bool isActive)
            => SyntaxFactory.ReferenceDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.ReferenceKeyword), file, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new LoadDirectiveTriviaSyntax instance.</summary>
        public static LoadDirectiveTriviaSyntax LoadDirectiveTrivia(SyntaxToken hashToken, SyntaxToken loadKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (loadKeyword.Kind() != SyntaxKind.LoadKeyword) throw new ArgumentException(nameof(loadKeyword));
            if (file.Kind() != SyntaxKind.StringLiteralToken) throw new ArgumentException(nameof(file));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (LoadDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.LoadDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)loadKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)file.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new LoadDirectiveTriviaSyntax instance.</summary>
        public static LoadDirectiveTriviaSyntax LoadDirectiveTrivia(SyntaxToken file, bool isActive)
            => SyntaxFactory.LoadDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.LoadKeyword), file, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new ShebangDirectiveTriviaSyntax instance.</summary>
        public static ShebangDirectiveTriviaSyntax ShebangDirectiveTrivia(SyntaxToken hashToken, SyntaxToken exclamationToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (exclamationToken.Kind() != SyntaxKind.ExclamationToken) throw new ArgumentException(nameof(exclamationToken));
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (ShebangDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.ShebangDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)exclamationToken.Node!, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new ShebangDirectiveTriviaSyntax instance.</summary>
        public static ShebangDirectiveTriviaSyntax ShebangDirectiveTrivia(bool isActive)
            => SyntaxFactory.ShebangDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.ExclamationToken), SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new NullableDirectiveTriviaSyntax instance.</summary>
        public static NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken.Kind() != SyntaxKind.HashToken) throw new ArgumentException(nameof(hashToken));
            if (nullableKeyword.Kind() != SyntaxKind.NullableKeyword) throw new ArgumentException(nameof(nullableKeyword));
            switch (settingToken.Kind())
            {
                case SyntaxKind.EnableKeyword:
                case SyntaxKind.DisableKeyword:
                case SyntaxKind.RestoreKeyword: break;
                default: throw new ArgumentException(nameof(settingToken));
            }
            switch (targetToken.Kind())
            {
                case SyntaxKind.WarningsKeyword:
                case SyntaxKind.AnnotationsKeyword:
                case SyntaxKind.None: break;
                default: throw new ArgumentException(nameof(targetToken));
            }
            if (endOfDirectiveToken.Kind() != SyntaxKind.EndOfDirectiveToken) throw new ArgumentException(nameof(endOfDirectiveToken));
            return (NullableDirectiveTriviaSyntax)Syntax.InternalSyntax.SyntaxFactory.NullableDirectiveTrivia((Syntax.InternalSyntax.SyntaxToken)hashToken.Node!, (Syntax.InternalSyntax.SyntaxToken)nullableKeyword.Node!, (Syntax.InternalSyntax.SyntaxToken)settingToken.Node!, (Syntax.InternalSyntax.SyntaxToken?)targetToken.Node, (Syntax.InternalSyntax.SyntaxToken)endOfDirectiveToken.Node!, isActive).CreateRed();
        }

        /// <summary>Creates a new NullableDirectiveTriviaSyntax instance.</summary>
        public static NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken settingToken, SyntaxToken targetToken, bool isActive)
            => SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.NullableKeyword), settingToken, targetToken, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);

        /// <summary>Creates a new NullableDirectiveTriviaSyntax instance.</summary>
        public static NullableDirectiveTriviaSyntax NullableDirectiveTrivia(SyntaxToken settingToken, bool isActive)
            => SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.HashToken), SyntaxFactory.Token(SyntaxKind.NullableKeyword), settingToken, default, SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken), isActive);
    }
}
