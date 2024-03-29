// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// This portion of the binder converts StatementSyntax nodes into BoundStatements
    /// </summary>
    public partial class Binder
    {
        /// <summary>
        /// This is the set of parameters and local variables that were used as arguments to
        /// lock or using statements in enclosing scopes.
        /// </summary>
        /// <remarks>
        /// using (x) { } // x counts
        /// using (IDisposable y = null) { } // y does not count
        /// </remarks>
        internal virtual ImmutableHashSet<Symbol> LockedOrDisposedVariables
        {
            get { return Next.LockedOrDisposedVariables; }
        }

        /// <remarks>
        /// Noteworthy override is in MemberSemanticModel.IncrementalBinder (used for caching).
        /// </remarks>
        public virtual BoundStatement BindStatement(StatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (node.AttributeLists.Count > 0)
            {
                var attributeList = node.AttributeLists[0];

                // Currently, attributes are only allowed on local-functions.
                if (node.Kind() == SyntaxKind.LocalFunctionStatement)
                {
                    CheckFeatureAvailability(attributeList, MessageID.IDS_FeatureLocalFunctionAttributes, diagnostics);
                }
                else if (node.Kind() != SyntaxKind.Block)
                {
                    // Don't explicitly error here for blocks.  Some codepaths bypass BindStatement
                    // to directly call BindBlock.
                    Error(diagnostics, ErrorCode.ERR_AttributesNotAllowed, attributeList);
                }
            }

            BoundStatement result = node.Kind() switch
            {
                SyntaxKind.Block => BindBlock((BlockSyntax)node, diagnostics),
                SyntaxKind.LocalDeclarationStatement => BindLocalDeclarationStatement((LocalDeclarationStatementSyntax)node, diagnostics),
                SyntaxKind.LocalFunctionStatement => BindLocalFunctionStatement((LocalFunctionStatementSyntax)node, diagnostics),
                SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)node, diagnostics),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)node, diagnostics),
                SyntaxKind.SwitchStatement => BindSwitchStatement((SwitchStatementSyntax)node, diagnostics),
                SyntaxKind.DoStatement => BindDo((DoStatementSyntax)node, diagnostics),
                SyntaxKind.WhileStatement => BindWhile((WhileStatementSyntax)node, diagnostics),
                SyntaxKind.ForStatement => BindFor((ForStatementSyntax)node, diagnostics),
                SyntaxKind.ForEachStatement or SyntaxKind.ForEachVariableStatement => BindForEach((CommonForEachStatementSyntax)node, diagnostics),
                SyntaxKind.BreakStatement => BindBreak((BreakStatementSyntax)node, diagnostics),
                SyntaxKind.ContinueStatement => BindContinue((ContinueStatementSyntax)node, diagnostics),
                SyntaxKind.ReturnStatement => BindReturn((ReturnStatementSyntax)node, diagnostics),
                SyntaxKind.FixedStatement => BindFixedStatement((FixedStatementSyntax)node, diagnostics),
                SyntaxKind.LabeledStatement => BindLabeled((LabeledStatementSyntax)node, diagnostics),
                SyntaxKind.GotoStatement or SyntaxKind.GotoCaseStatement or SyntaxKind.GotoDefaultStatement => BindGoto((GotoStatementSyntax)node, diagnostics),
                SyntaxKind.TryStatement => BindTryStatement((TryStatementSyntax)node, diagnostics),
                SyntaxKind.EmptyStatement => BindEmpty((EmptyStatementSyntax)node),
                SyntaxKind.ThrowStatement => BindThrow((ThrowStatementSyntax)node, diagnostics),
                SyntaxKind.UnsafeStatement => BindUnsafeStatement((UnsafeStatementSyntax)node, diagnostics),
                SyntaxKind.UncheckedStatement or SyntaxKind.CheckedStatement => BindCheckedStatement((CheckedStatementSyntax)node, diagnostics),
                SyntaxKind.UsingStatement => BindUsingStatement((UsingStatementSyntax)node, diagnostics),
                SyntaxKind.YieldBreakStatement => BindYieldBreakStatement((YieldStatementSyntax)node, diagnostics),
                SyntaxKind.YieldReturnStatement => BindYieldReturnStatement((YieldStatementSyntax)node, diagnostics),
                SyntaxKind.LockStatement => BindLockStatement((LockStatementSyntax)node, diagnostics),
                _ => new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, hasErrors: true),// NOTE: We could probably throw an exception here, but it's conceivable
                                                                                                   // that a non-parser syntax tree could reach this point with an unexpected
                                                                                                   // SyntaxKind and we don't want to throw if that occurs.
            };
            return result;
        }

        private BoundStatement BindCheckedStatement(CheckedStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            return BindEmbeddedBlock(node.Block, diagnostics);
        }

        private BoundStatement BindUnsafeStatement(UnsafeStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            this.GetBinder(node);

            if (!this.Compilation.Options.AllowUnsafe)
            {
                Error(diagnostics, ErrorCode.ERR_IllegalUnsafe, node.UnsafeKeyword);
            }
            else if (this.IsIndirectlyInIterator) // called *after* we know the binder map has been created.
            {
                // Spec 8.2: "An iterator block always defines a safe context, even when its declaration
                // is nested in an unsafe context."
                Error(diagnostics, ErrorCode.ERR_IllegalInnerUnsafe, node.UnsafeKeyword);
            }

            return BindEmbeddedBlock(node.Block, diagnostics);
        }

        private BoundStatement BindFixedStatement(FixedStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var fixedBinder = this.GetBinder(node);

            fixedBinder.ReportUnsafeIfNotAllowed(node, diagnostics);

            return fixedBinder.BindFixedStatementParts(node, diagnostics);
        }

        private BoundStatement BindFixedStatementParts(FixedStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            VariableDeclarationSyntax declarationSyntax = node.Declaration;

            BindForOrUsingOrFixedDeclarations(declarationSyntax, LocalDeclarationKind.FixedVariable, diagnostics, out ImmutableArray<BoundLocalDeclaration> declarations);


            BoundMultipleLocalDeclarations boundMultipleDeclarations = new(declarationSyntax, declarations);

            BoundStatement boundBody = BindPossibleEmbeddedStatement(node.Statement, diagnostics);

            return new BoundFixedStatement(node,
                                           GetDeclaredLocalsForScope(node),
                                           boundMultipleDeclarations,
                                           boundBody);
        }

        private void CheckRequiredLangVersionForAsyncIteratorMethods(BindingDiagnosticBag diagnostics)
        {
            var method = (MethodSymbol)this.ContainingMemberOrLambda;
            if (method.IsAsync)
            {
                MessageID.IDS_FeatureAsyncStreams.CheckFeatureAvailability(
                    diagnostics,
                    method.DeclaringCompilation,
                    method.Locations[0]);
            }
        }

        protected virtual void ValidateYield(YieldStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            Next?.ValidateYield(node, diagnostics);
        }

        private BoundStatement BindYieldReturnStatement(YieldStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            ValidateYield(node, diagnostics);
            TypeSymbol elementType = GetIteratorElementType().Type;
            BoundExpression argument = (node.Expression == null)
                ? BadExpression(node).MakeCompilerGenerated()
                : BindValue(node.Expression, diagnostics, BindValueKind.RValue);
            argument = ValidateEscape(argument, ExternalScope, isByRef: false, diagnostics: diagnostics);

            if (!argument.HasAnyErrors)
            {
                argument = GenerateConversionForAssignment(elementType, argument, diagnostics);
            }
            else
            {
                argument = BindToTypeForErrorRecovery(argument);
            }

            // NOTE: it's possible that more than one of these conditions is satisfied and that
            // we won't report the syntactically innermost.  However, dev11 appears to check
            // them in this order, regardless of syntactic nesting (StatementBinder::bindYield).
            if (this.Flags.Includes(EBinder.InFinallyBlock))
            {
                Error(diagnostics, ErrorCode.ERR_BadYieldInFinally, node.YieldKeyword);
            }
            else if (this.Flags.Includes(EBinder.InTryBlockOfTryCatch))
            {
                Error(diagnostics, ErrorCode.ERR_BadYieldInTryOfCatch, node.YieldKeyword);
            }
            else if (this.Flags.Includes(EBinder.InCatchBlock))
            {
                Error(diagnostics, ErrorCode.ERR_BadYieldInCatch, node.YieldKeyword);
            }
            else if (BindingTopLevelScriptCode)
            {
                Error(diagnostics, ErrorCode.ERR_YieldNotAllowedInScript, node.YieldKeyword);
            }

            CheckRequiredLangVersionForAsyncIteratorMethods(diagnostics);
            return new BoundYieldReturnStatement(node, argument);
        }

        private BoundStatement BindYieldBreakStatement(YieldStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (this.Flags.Includes(EBinder.InFinallyBlock))
            {
                Error(diagnostics, ErrorCode.ERR_BadYieldInFinally, node.YieldKeyword);
            }
            else if (BindingTopLevelScriptCode)
            {
                Error(diagnostics, ErrorCode.ERR_YieldNotAllowedInScript, node.YieldKeyword);
            }

            ValidateYield(node, diagnostics);
            CheckRequiredLangVersionForAsyncIteratorMethods(diagnostics);
            return new BoundYieldBreakStatement(node);
        }

        private BoundStatement BindLockStatement(LockStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var lockBinder = this.GetBinder(node);
            return lockBinder.BindLockStatementParts(diagnostics, lockBinder);
        }

        internal virtual BoundStatement BindLockStatementParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindLockStatementParts(diagnostics, originalBinder);
        }


        private BoundStatement BindUsingStatement(UsingStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var usingBinder = this.GetBinder(node);
            return usingBinder.BindUsingStatementParts(diagnostics, usingBinder);
        }

        internal virtual BoundStatement BindUsingStatementParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindUsingStatementParts(diagnostics, originalBinder);
        }

        internal BoundStatement BindPossibleEmbeddedStatement(StatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            Binder binder;

            switch (node.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    // Local declarations are not legal in contexts where we need embedded statements.
                    diagnostics.Add(ErrorCode.ERR_BadEmbeddedStmt, node.GetLocation());

                    // fall through
                    goto case SyntaxKind.ExpressionStatement;

                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.ThrowStatement:
                    binder = this.GetBinder(node);
                    return binder.WrapWithVariablesIfAny(node, binder.BindStatement(node, diagnostics));

                case SyntaxKind.LabeledStatement:
                case SyntaxKind.LocalFunctionStatement:
                    // Labeled statements and local function statements are not legal in contexts where we need embedded statements.
                    diagnostics.Add(ErrorCode.ERR_BadEmbeddedStmt, node.GetLocation());

                    binder = this.GetBinder(node);
                    return binder.WrapWithVariablesAndLocalFunctionsIfAny(node, binder.BindStatement(node, diagnostics));

                case SyntaxKind.SwitchStatement:
                    var switchStatement = (SwitchStatementSyntax)node;
                    binder = this.GetBinder(switchStatement.Expression);
                    return binder.WrapWithVariablesIfAny(switchStatement.Expression, binder.BindStatement(node, diagnostics));

                case SyntaxKind.EmptyStatement:
                    var emptyStatement = (EmptyStatementSyntax)node;
                    if (!emptyStatement.SemicolonToken.IsMissing)
                    {
                        switch (node.Parent.Kind())
                        {
                            case SyntaxKind.ForStatement:
                            case SyntaxKind.ForEachStatement:
                            case SyntaxKind.ForEachVariableStatement:
                            case SyntaxKind.WhileStatement:
                                // For loop constructs, only warn if we see a block following the statement.
                                // That indicates code like:  "while (x) ; { }"
                                // which is most likely a bug.
                                if (emptyStatement.SemicolonToken.GetNextToken().Kind() != SyntaxKind.OpenBraceToken)
                                {
                                    break;
                                }

                                goto default;

                            default:
                                // For non-loop constructs, always warn.  This is for code like:
                                // "if (x) ;" which is almost certainly a bug.
                                diagnostics.Add(ErrorCode.WRN_PossibleMistakenNullStatement, node.GetLocation());
                                break;
                        }
                    }

                    // fall through
                    goto default;

                default:
                    return BindStatement(node, diagnostics);
            }
        }

        private BoundExpression BindThrownExpression(ExpressionSyntax exprSyntax, BindingDiagnosticBag diagnostics, ref bool hasErrors)
        {
            var boundExpr = BindValue(exprSyntax, diagnostics, BindValueKind.RValue);
            if (Compilation.LanguageVersion < MessageID.IDS_FeatureSwitchExpression.RequiredVersion())
            {
                // This is the pre-C# 8 algorithm for binding a thrown expression.
                // SPEC VIOLATION: The spec requires the thrown exception to have a type, and that the type
                // be System.Exception or derived from System.Exception. (Or, if a type parameter, to have
                // an effective base class that meets that criterion.) However, we allow the literal null
                // to be thrown, even though it does not meet that criterion and will at runtime always
                // produce a null reference exception.
                if (!boundExpr.IsLiteralNull())
                {
                    boundExpr = BindToNaturalType(boundExpr, diagnostics);
                    var type = boundExpr.Type;

                    // If the expression is a lambda, anonymous method, or method group then it will
                    // have no compile-time type; give the same error as if the type was wrong.
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);

                    if (type is null || !type.IsErrorType() && !Compilation.IsExceptionType(type.EffectiveType(ref useSiteInfo), ref useSiteInfo))
                    {
                        diagnostics.Add(ErrorCode.ERR_BadExceptionType, exprSyntax.Location);
                        hasErrors = true;
                        diagnostics.Add(exprSyntax, useSiteInfo);
                    }
                    else
                    {
                        diagnostics.AddDependencies(useSiteInfo);
                    }
                }
            }
            else
            {
                // In C# 8 and later we follow the ECMA specification, which neatly handles null and expressions of exception type.
                boundExpr = GenerateConversionForAssignment(GetWellKnownType(WellKnownType.System_Exception, diagnostics, exprSyntax), boundExpr, diagnostics);
            }

            return boundExpr;
        }

        private BoundStatement BindThrow(ThrowStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            BoundExpression boundExpr = null;
            bool hasErrors = false;

            ExpressionSyntax exprSyntax = node.Expression;
            if (exprSyntax != null)
            {
                boundExpr = BindThrownExpression(exprSyntax, diagnostics, ref hasErrors);
            }
            else if (!this.Flags.Includes(EBinder.InCatchBlock))
            {
                diagnostics.Add(ErrorCode.ERR_BadEmptyThrow, node.ThrowKeyword.GetLocation());
                hasErrors = true;
            }
            else if (this.Flags.Includes(EBinder.InNestedFinallyBlock))
            {
                // There's a special error code for a rethrow in a finally clause in a catch clause.
                // Best guess interpretation: if an exception occurs within the nested try block
                // (i.e. the one in the catch clause, to which the finally clause is attached),
                // then it's not clear whether the runtime will try to rethrow the "inner" exception
                // or the "outer" exception. For this reason, the case is disallowed.

                diagnostics.Add(ErrorCode.ERR_BadEmptyThrowInFinally, node.ThrowKeyword.GetLocation());
                hasErrors = true;
            }

            return new BoundThrowStatement(node, boundExpr, hasErrors);
        }

        private static BoundStatement BindEmpty(EmptyStatementSyntax node)
        {
            return new BoundNoOpStatement(node, NoOpStatementFlavor.Default);
        }

        private BoundLabeledStatement BindLabeled(LabeledStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            // TODO: verify that goto label lookup was valid (e.g. error checking of symbol resolution for labels)
            bool hasError = false;

            var result = LookupResult.GetInstance();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            var binder = this.LookupSymbolsWithFallback(result, node.Identifier.ValueText, arity: 0, useSiteInfo: ref useSiteInfo, options: LookupOptions.LabelsOnly);

            // result.Symbols can be empty in some malformed code, e.g. when a labeled statement is used an embedded statement in an if or foreach statement
            // In this case we create new label symbol on the fly, and an error is reported by parser
            var symbol = result.Symbols.Count > 0 && result.IsMultiViable ?
                (LabelSymbol)result.Symbols.First() :
                new SourceLabelSymbol((MethodSymbol)ContainingMemberOrLambda, node.Identifier);

            if (!symbol.IdentifierNodeOrToken.IsToken || symbol.IdentifierNodeOrToken.AsToken() != node.Identifier)
            {
                Error(diagnostics, ErrorCode.ERR_DuplicateLabel, node.Identifier, node.Identifier.ValueText);
                hasError = true;
            }

            // check to see if this label (illegally) hides a label from an enclosing scope
            if (binder != null)
            {
                result.Clear();
                binder.Next.LookupSymbolsWithFallback(result, node.Identifier.ValueText, arity: 0, useSiteInfo: ref useSiteInfo, options: LookupOptions.LabelsOnly);
                if (result.IsMultiViable)
                {
                    // The label '{0}' shadows another label by the same name in a contained scope
                    Error(diagnostics, ErrorCode.ERR_LabelShadow, node.Identifier, node.Identifier.ValueText);
                    hasError = true;
                }
            }

            diagnostics.Add(node, useSiteInfo);
            result.Free();

            var body = BindStatement(node.Statement, diagnostics);
            return new BoundLabeledStatement(node, symbol, body, hasError);
        }

        private BoundStatement BindGoto(GotoStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            switch (node.Kind())
            {
                case SyntaxKind.GotoStatement:
                    var expression = BindLabel(node.Expression, diagnostics);
                    if (expression is not BoundLabel boundLabel)
                    {
                        // diagnostics already reported
                        return new BoundBadStatement(node, ImmutableArray.Create<BoundNode>(expression), true);
                    }
                    var symbol = boundLabel.Label;
                    return new BoundGotoStatement(node, symbol, null, boundLabel);

                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:

                    // SPEC:    If the goto case statement is not enclosed by a switch statement, a compile-time error occurs.
                    // SPEC:    If the goto default statement is not enclosed by a switch statement, a compile-time error occurs.

                    SwitchBinder binder = GetSwitchBinder(this);
                    if (binder == null)
                    {
                        Error(diagnostics, ErrorCode.ERR_InvalidGotoCase, node);
                        ImmutableArray<BoundNode> childNodes;
                        if (node.Expression != null)
                        {
                            var value = BindRValueWithoutTargetType(node.Expression, BindingDiagnosticBag.Discarded);
                            childNodes = ImmutableArray.Create<BoundNode>(value);
                        }
                        else
                        {
                            childNodes = ImmutableArray<BoundNode>.Empty;
                        }
                        return new BoundBadStatement(node, childNodes, true);
                    }
                    return binder.BindGotoCaseOrDefault(node, this, diagnostics);

                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind());
            }
        }

        private BoundStatement BindLocalFunctionStatement(LocalFunctionStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            // already defined symbol in containing block
            var localSymbol = this.LookupLocalFunction(node.Identifier);

            var hasErrors = localSymbol.ScopeBinder
                .ValidateDeclarationNameConflictsInScope(localSymbol, diagnostics);

            BoundBlock blockBody = null;
            BoundBlock expressionBody = null;
            if (node.Body != null)
            {
                blockBody = runAnalysis(BindEmbeddedBlock(node.Body, diagnostics), diagnostics);

                if (node.ExpressionBody != null)
                {
                    expressionBody = runAnalysis(BindExpressionBodyAsBlock(node.ExpressionBody, BindingDiagnosticBag.Discarded), BindingDiagnosticBag.Discarded);
                }
            }
            else if (node.ExpressionBody != null)
            {
                expressionBody = runAnalysis(BindExpressionBodyAsBlock(node.ExpressionBody, diagnostics), diagnostics);
            }
            else if (!hasErrors && (!localSymbol.IsExtern || !localSymbol.IsStatic))
            {
                hasErrors = true;
                diagnostics.Add(ErrorCode.ERR_LocalFunctionMissingBody, localSymbol.Locations[0], localSymbol);
            }

            if (!hasErrors && (blockBody != null || expressionBody != null) && localSymbol.IsExtern)
            {
                hasErrors = true;
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, localSymbol.Locations[0], localSymbol);
            }


            localSymbol.GetDeclarationDiagnostics(diagnostics);

            Symbol.CheckForBlockAndExpressionBody(
                node.Body, node.ExpressionBody, node, diagnostics);

            return new BoundLocalFunctionStatement(node, localSymbol, blockBody, expressionBody, hasErrors);

            BoundBlock runAnalysis(BoundBlock block, BindingDiagnosticBag blockDiagnostics)
            {
                if (block != null)
                {
                    // Have to do ControlFlowPass here because in MethodCompiler, we don't call this for synthed methods
                    // rather we go directly to LowerBodyOrInitializer, which skips over flow analysis (which is in CompileMethod)
                    // (the same thing - calling ControlFlowPass.Analyze in the lowering - is done for lambdas)
                    // It's a bit of code duplication, but refactoring would make things worse.
                    // However, we don't need to report diagnostics here. They will be reported when analyzing the parent method.
                    var ignored = DiagnosticBag.GetInstance();
                    var endIsReachable = ControlFlowPass.Analyze(localSymbol.DeclaringCompilation, localSymbol, block, ignored);
                    ignored.Free();
                    if (endIsReachable)
                    {
                        if (ImplicitReturnIsOkay(localSymbol))
                        {
                            block = FlowAnalysisPass.AppendImplicitReturn(block, localSymbol);
                        }
                        else
                        {
                            blockDiagnostics.Add(ErrorCode.ERR_ReturnExpected, localSymbol.Locations[0], localSymbol);
                        }
                    }
                }

                return block;
            }
        }

        private bool ImplicitReturnIsOkay(MethodSymbol method)
        {
            return method.ReturnsVoid || method.IsIterator || method.IsAsyncReturningTask(this.Compilation);
        }

        public BoundStatement BindExpressionStatement(ExpressionStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            return BindExpressionStatement(node, node.Expression, node.AllowsAnyExpression, diagnostics);
        }

        private BoundExpressionStatement BindExpressionStatement(CSharpSyntaxNode node, ExpressionSyntax syntax, bool allowsAnyExpression, BindingDiagnosticBag diagnostics)
        {
            BoundExpressionStatement expressionStatement;

            var expression = BindRValueWithoutTargetType(syntax, diagnostics);
            ReportSuppressionIfNeeded(expression, diagnostics);
            if (!allowsAnyExpression && !IsValidStatementExpression(syntax, expression))
            {
                if (!node.HasErrors)
                {
                    Error(diagnostics, ErrorCode.ERR_IllegalStatement, syntax);
                }

                expressionStatement = new BoundExpressionStatement(node, expression, hasErrors: true);
            }
            else
            {
                expressionStatement = new BoundExpressionStatement(node, expression);
            }

            CheckForUnobservedAwaitable(expression, diagnostics);

            return expressionStatement;
        }

        /// <summary>
        /// Report an error if this is an awaitable async method invocation that is not being awaited.
        /// </summary>
        /// <remarks>
        /// The checks here are equivalent to StatementBinder::CheckForUnobservedAwaitable() in the native compiler.
        /// </remarks>
        private void CheckForUnobservedAwaitable(BoundExpression expression, BindingDiagnosticBag diagnostics)
        {
            if (CouldBeAwaited(expression))
            {
                Error(diagnostics, ErrorCode.WRN_UnobservedAwaitableExpression, expression.Syntax);
            }
        }

        internal BoundStatement BindLocalDeclarationStatement(LocalDeclarationStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (node.UsingKeyword != default)
            {
                return BindUsingDeclarationStatementParts(node, diagnostics);
            }
            else
            {
                return BindDeclarationStatementParts(node, diagnostics);
            }
        }

        private BoundStatement BindUsingDeclarationStatementParts(LocalDeclarationStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var usingDeclaration = UsingStatementBinder.BindUsingStatementOrDeclarationFromParts(node, node.UsingKeyword, node.AwaitKeyword, originalBinder: this, usingBinderOpt: null, diagnostics);
            return usingDeclaration;
        }

        private BoundStatement BindDeclarationStatementParts(LocalDeclarationStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var typeSyntax = node.Declaration.Type.SkipRef(out _);
            bool isConst = node.IsConst;

            TypeWithAnnotations declType = BindVariableTypeWithAnnotations(node.Declaration, diagnostics, typeSyntax, ref isConst, isVar: out bool isVar, alias: out AliasSymbol alias);

            var kind = isConst ? LocalDeclarationKind.Constant : LocalDeclarationKind.RegularVariable;
            var variableList = node.Declaration.Variables;
            int variableCount = variableList.Count;
            if (variableCount == 1)
            {
                return BindVariableDeclaration(kind, isVar, variableList[0], typeSyntax, declType, alias, diagnostics, includeBoundType: true, associatedSyntaxNode: node);
            }
            else
            {
                BoundLocalDeclaration[] boundDeclarations = new BoundLocalDeclaration[variableCount];
                int i = 0;
                foreach (var variableDeclarationSyntax in variableList)
                {
                    bool includeBoundType = i == 0; //To avoid duplicated expressions, only the first declaration should contain the bound type.
                    boundDeclarations[i++] = BindVariableDeclaration(kind, isVar, variableDeclarationSyntax, typeSyntax, declType, alias, diagnostics, includeBoundType);
                }
                return new BoundMultipleLocalDeclarations(node, boundDeclarations.AsImmutableOrNull());
            }
        }

        /// <summary>
        /// Checks for a Dispose method on <paramref name="expr"/> and returns its <see cref="MethodSymbol"/> if found.
        /// </summary>
        /// <param name="expr">Expression on which to perform lookup</param>
        /// <param name="syntaxNode">The syntax node to perform lookup on</param>
        /// <param name="diagnostics">Populated with invocation errors, and warnings of near misses</param>
        /// <returns>The <see cref="MethodSymbol"/> of the Dispose method if one is found, otherwise null.</returns>
        internal MethodSymbol TryFindDisposePatternMethod(BoundExpression expr, SyntaxNode syntaxNode, bool hasAwait, BindingDiagnosticBag diagnostics)
        {

            var result = PerformPatternMethodLookup(expr,
                                                    hasAwait ? WellKnownMemberNames.DisposeAsyncMethodName : WellKnownMemberNames.DisposeMethodName,
                                                    syntaxNode,
                                                    diagnostics,
                                                    out var disposeMethod);

            if (disposeMethod?.IsExtensionMethod == true)
            {
                // Extension methods should just be ignored, rather than rejected after-the-fact
                // Tracked by https://github.com/dotnet/roslyn/issues/32767

                // extension methods do not contribute to pattern-based disposal
                disposeMethod = null;
            }
            else if ((!hasAwait && disposeMethod?.ReturnsVoid == false)
                || result == PatternLookupResult.NotAMethod)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                if (this.IsAccessible(disposeMethod, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.WRN_PatternBadSignature, syntaxNode.Location, expr.Type, MessageID.IDS_Disposable.Localize(), disposeMethod);
                }

                diagnostics.Add(syntaxNode, useSiteInfo);
                disposeMethod = null;
            }

            return disposeMethod;
        }

        private TypeWithAnnotations BindVariableTypeWithAnnotations(CSharpSyntaxNode declarationNode, BindingDiagnosticBag diagnostics, TypeSyntax typeSyntax, ref bool isConst, out bool isVar, out AliasSymbol alias)
        {
            // If the type is "var" then suppress errors when binding it. "var" might be a legal type
            // or it might not; if it is not then we do not want to report an error. If it is, then
            // we want to treat the declaration as an explicitly typed declaration.

            TypeWithAnnotations declType = BindTypeOrVarKeyword(typeSyntax.SkipRef(out _), diagnostics, out isVar, out alias);

            if (isVar)
            {
                // There are a number of ways in which a var decl can be illegal, but in these
                // cases we should report an error and then keep right on going with the inference.

                if (isConst)
                {
                    Error(diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableCannotBeConst, declarationNode);
                    // Keep processing it as a non-const local.
                    isConst = false;
                }

                // In the dev10 compiler the error recovery semantics for the illegal case
                // "var x = 10, y = 123.4;" are somewhat undesirable.
                //
                // First off, this is an error because a straw poll of language designers and
                // users showed that there was no consensus on whether the above should mean
                // "double x = 10, y = 123.4;", taking the best type available and substituting
                // that for "var", or treating it as "var x = 10; var y = 123.4;" -- since there
                // was no consensus we decided to simply make it illegal.
                //
                // In dev10 for error recovery in the IDE we do an odd thing -- we simply take
                // the type of the first variable and use it. So that is "int x = 10, y = 123.4;".
                //
                // This seems less than ideal. In the error recovery scenario it probably makes
                // more sense to treat that as "var x = 10; var y = 123.4;" and do each inference
                // separately.

                if (declarationNode.Parent.Kind() == SyntaxKind.LocalDeclarationStatement &&
                    ((VariableDeclarationSyntax)declarationNode).Variables.Count > 1 && !declarationNode.HasErrors)
                {
                    Error(diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableMultipleDeclarator, declarationNode);
                }
            }
            else
            {
                // In the native compiler when given a situation like
#pragma warning disable S125 // Sections of code should not be commented out
                //
                // D[] x;
                //
                // where D is a static type we report both that D cannot be an element type
                // of an array, and that D[] is not a valid type for a local variable.
                // This seems silly; the first error is entirely sufficient. We no longer
                // produce additional errors for local variables of arrays of static types.
#pragma warning restore S125 // Sections of code should not be commented out

                if (declType.IsStatic)
                {
                    Error(diagnostics, ErrorCode.ERR_VarDeclIsStaticClass, typeSyntax, declType.Type);
                }

                if (isConst && !declType.Type.CanBeConst())
                {
                    Error(diagnostics, ErrorCode.ERR_BadConstType, typeSyntax, declType.Type);
                    // Keep processing it as a non-const local.
                    isConst = false;
                }
            }

            return declType;
        }

        internal BoundExpression BindInferredVariableInitializer(BindingDiagnosticBag diagnostics, RefKind refKind, EqualsValueClauseSyntax initializer,
            CSharpSyntaxNode errorSyntax)
        {
            IsInitializerRefKindValid(initializer, initializer, refKind, diagnostics, out BindValueKind valueKind, out ExpressionSyntax value); // The return value isn't important here; we just want the diagnostics and the BindValueKind
            return BindInferredVariableInitializer(diagnostics, value, valueKind, /*refKind, */errorSyntax);
        }

        // The location where the error is reported might not be the initializer.
        protected BoundExpression BindInferredVariableInitializer(BindingDiagnosticBag diagnostics, ExpressionSyntax initializer, BindValueKind valueKind, /*RefKind refKind, */CSharpSyntaxNode errorSyntax)
        {
            if (initializer == null)
            {
                if (!errorSyntax.HasErrors)
                {
                    Error(diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableWithNoInitializer, errorSyntax);
                }

                return null;
            }

            if (initializer.Kind() == SyntaxKind.ArrayInitializerExpression)
            {
                var result = BindUnexpectedArrayInitializer((InitializerExpressionSyntax)initializer,
                    diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableAssignedArrayInitializer, errorSyntax);

                return CheckValue(result, valueKind, diagnostics);
            }

            BoundExpression expression = BindToNaturalType(BindValue(initializer, diagnostics, valueKind), diagnostics);

            // Certain expressions (null literals, method groups and anonymous functions) have no type of
            // their own and therefore cannot be the initializer of an implicitly typed local.
            if (!expression.HasAnyErrors && !expression.HasExpressionType())
            {
                // Cannot assign {0} to an implicitly-typed local variable
                Error(diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableAssignedBadValue, errorSyntax, expression.Display);
            }

            return expression;
        }

        private static bool IsInitializerRefKindValid(
            EqualsValueClauseSyntax initializer,
            CSharpSyntaxNode node,
            RefKind variableRefKind,
            BindingDiagnosticBag diagnostics,
            out BindValueKind valueKind,
            out ExpressionSyntax value)
        {
            RefKind expressionRefKind = RefKind.None;
            value = initializer?.Value.CheckAndUnwrapRefExpression(diagnostics, out expressionRefKind);
            if (variableRefKind == RefKind.None)
            {
                valueKind = BindValueKind.RValue;
                if (expressionRefKind == RefKind.Ref)
                {
                    Error(diagnostics, ErrorCode.ERR_InitializeByValueVariableWithReference, node);
                    return false;
                }
            }
            else
            {
                valueKind = variableRefKind == RefKind.RefReadOnly
                    ? BindValueKind.ReadonlyRef
                    : BindValueKind.RefOrOut;

                if (initializer == null)
                {
                    Error(diagnostics, ErrorCode.ERR_ByReferenceVariableMustBeInitialized, node);
                    return false;
                }
                else if (expressionRefKind != RefKind.Ref)
                {
                    Error(diagnostics, ErrorCode.ERR_InitializeByReferenceVariableWithValue, node);
                    return false;
                }
            }

            return true;
        }

        protected BoundLocalDeclaration BindVariableDeclaration(
            LocalDeclarationKind kind,
            bool isVar,
            VariableDeclaratorSyntax declarator,
            TypeSyntax typeSyntax,
            TypeWithAnnotations declTypeOpt,
            AliasSymbol aliasOpt,
            BindingDiagnosticBag diagnostics,
            bool includeBoundType,
            CSharpSyntaxNode associatedSyntaxNode = null)
        {

            return BindVariableDeclaration(LocateDeclaredVariableSymbol(declarator, typeSyntax, kind),
                                           kind,
                                           isVar,
                                           declarator,
                                           typeSyntax,
                                           declTypeOpt,
                                           aliasOpt,
                                           diagnostics,
                                           includeBoundType,
                                           associatedSyntaxNode);
        }

        protected BoundLocalDeclaration BindVariableDeclaration(
            SourceLocalSymbol localSymbol,
            LocalDeclarationKind kind,
            bool isVar,
            VariableDeclaratorSyntax declarator,
            TypeSyntax typeSyntax,
            TypeWithAnnotations declTypeOpt,
            AliasSymbol aliasOpt,
            BindingDiagnosticBag diagnostics,
            bool includeBoundType,
            CSharpSyntaxNode associatedSyntaxNode = null)
        {

            var localDiagnostics = BindingDiagnosticBag.GetInstance(withDiagnostics: true, diagnostics.AccumulatesDependencies);
            // if we are not given desired syntax, we use declarator
            associatedSyntaxNode ??= declarator;

            // Check for variable declaration errors.
            // Use the binder that owns the scope for the local because this (the current) binder
            // might own nested scope.
            bool nameConflict = localSymbol.ScopeBinder.ValidateDeclarationNameConflictsInScope(localSymbol, diagnostics);
            bool hasErrors = false;

            if (localSymbol.RefKind != RefKind.None)
            {
                CheckRefLocalInAsyncOrIteratorMethod(localSymbol.IdentifierToken, diagnostics);
            }

            EqualsValueClauseSyntax equalsClauseSyntax = declarator.Initializer;

            if (!IsInitializerRefKindValid(equalsClauseSyntax, declarator, localSymbol.RefKind, diagnostics, out BindValueKind valueKind, out ExpressionSyntax value))
            {
                hasErrors = true;
            }

            BoundExpression initializerOpt;
            if (isVar)
            {
                aliasOpt = null;

                initializerOpt = BindInferredVariableInitializer(diagnostics, value, valueKind, /*localSymbol.RefKind, */declarator);

                // If we got a good result then swap the inferred type for the "var"
                TypeSymbol initializerType = initializerOpt?.Type;
                if (initializerType is not null)
                {
                    declTypeOpt = TypeWithAnnotations.Create(initializerType);

                    if (declTypeOpt.IsVoidType())
                    {
                        Error(localDiagnostics, ErrorCode.ERR_ImplicitlyTypedVariableAssignedBadValue, declarator, declTypeOpt.Type);
                        declTypeOpt = TypeWithAnnotations.Create(CreateErrorType("var"));
                        hasErrors = true;
                    }

                    if (!declTypeOpt.Type.IsErrorType() && declTypeOpt.IsStatic)
                    {
                        Error(localDiagnostics, ErrorCode.ERR_VarDeclIsStaticClass, typeSyntax, initializerType);
                        hasErrors = true;
                    }
                }
                else
                {
                    declTypeOpt = TypeWithAnnotations.Create(CreateErrorType("var"));
                    hasErrors = true;
                }
            }
            else
            {
                if (equalsClauseSyntax is null)
                {
                    initializerOpt = null;
                }
                else
                {
                    // Basically inlined BindVariableInitializer, but with conversion optional.
                    initializerOpt = BindPossibleArrayInitializer(value, declTypeOpt.Type, valueKind, diagnostics);
                    if (kind != LocalDeclarationKind.FixedVariable)
                    {
                        // If this is for a fixed statement, we'll do our own conversion since there are some special cases.
                        initializerOpt = GenerateConversionForAssignment(
                            declTypeOpt.Type,
                            initializerOpt,
                            localDiagnostics,
                            isRefAssignment: localSymbol.RefKind != RefKind.None);
                    }
                }
            }


            if (kind == LocalDeclarationKind.FixedVariable)
            {
                // NOTE: this is an error, but it won't prevent further binding.
                if (isVar)
                {
                    if (!hasErrors)
                    {
                        Error(localDiagnostics, ErrorCode.ERR_ImplicitlyTypedLocalCannotBeFixed, declarator);
                        hasErrors = true;
                    }
                }

                if (!declTypeOpt.Type.IsPointerType())
                {
                    if (!hasErrors)
                    {
                        Error(localDiagnostics, declTypeOpt.Type.IsFunctionPointer() ? ErrorCode.ERR_CannotUseFunctionPointerAsFixedLocal : ErrorCode.ERR_BadFixedInitType, declarator);
                        hasErrors = true;
                    }
                }
                else if (!IsValidFixedVariableInitializer(declTypeOpt.Type, ref initializerOpt, localDiagnostics))
                {
                    hasErrors = true;
                }
            }

            if (CheckRestrictedTypeInAsyncMethod(this.ContainingMemberOrLambda, declTypeOpt.Type, localDiagnostics, typeSyntax))
            {
                hasErrors = true;
            }

            localSymbol.SetTypeWithAnnotations(declTypeOpt);

            if (initializerOpt != null)
            {
                var currentScope = LocalScopeDepth;

                localSymbol.SetValEscape(GetValEscape(initializerOpt, currentScope));

                if (localSymbol.RefKind != RefKind.None)
                {
                    localSymbol.SetRefEscape(GetRefEscape(initializerOpt, currentScope));
                }
            }

            ImmutableArray<BoundExpression> arguments = BindDeclaratorArguments(declarator, localDiagnostics);

            if (kind == LocalDeclarationKind.FixedVariable || kind == LocalDeclarationKind.UsingVariable)
            {
                // CONSIDER: The error message is "you must provide an initializer in a fixed
                // CONSIDER: or using declaration". The error message could be targeted to
                // CONSIDER: the actual situation. "you must provide an initializer in a
                // CONSIDER: 'fixed' declaration."

                if (initializerOpt == null)
                {
                    Error(localDiagnostics, ErrorCode.ERR_FixedMustInit, declarator);
                    hasErrors = true;
                }
            }
            else if (kind == LocalDeclarationKind.Constant && initializerOpt != null && !localDiagnostics.HasAnyResolvedErrors())
            {
                var constantValueDiagnostics = localSymbol.GetConstantValueDiagnostics(initializerOpt);
                diagnostics.AddRange(constantValueDiagnostics, allowMismatchInDependencyAccumulation: true);
                hasErrors = constantValueDiagnostics.Diagnostics.HasAnyErrors();
            }

            diagnostics.AddRangeAndFree(localDiagnostics);

            BoundTypeExpression boundDeclType = null;

            if (includeBoundType)
            {
                var invalidDimensions = ArrayBuilder<BoundExpression>.GetInstance();

                typeSyntax.VisitRankSpecifiers((rankSpecifier, args) =>
                {
                    bool _ = false;
                    foreach (var expressionSyntax in rankSpecifier.Sizes)
                    {
                        var size = args.binder.BindArrayDimension(expressionSyntax, args.diagnostics, ref _);
                        if (size != null)
                        {
                            args.invalidDimensions.Add(size);
                        }
                    }
                }, (binder: this, invalidDimensions, diagnostics));

                boundDeclType = new BoundTypeExpression(typeSyntax, aliasOpt, dimensionsOpt: invalidDimensions.ToImmutableAndFree(), typeWithAnnotations: declTypeOpt);
            }

            return new BoundLocalDeclaration(
                syntax: associatedSyntaxNode,
                localSymbol: localSymbol,
                declaredTypeOpt: boundDeclType,
                initializerOpt: hasErrors ? BindToTypeForErrorRecovery(initializerOpt)?.WithHasErrors() : initializerOpt,
                argumentsOpt: arguments,
                inferredType: isVar,
                hasErrors: hasErrors | nameConflict);
        }

        protected bool CheckRefLocalInAsyncOrIteratorMethod(SyntaxToken identifierToken, BindingDiagnosticBag diagnostics)
        {
            if (IsInAsyncMethod())
            {
                Error(diagnostics, ErrorCode.ERR_BadAsyncLocalType, identifierToken);
                return true;
            }
            else if (IsDirectlyInIterator)
            {
                Error(diagnostics, ErrorCode.ERR_BadIteratorLocalType, identifierToken);
                return true;
            }

            return false;
        }

        internal ImmutableArray<BoundExpression> BindDeclaratorArguments(VariableDeclaratorSyntax declarator, BindingDiagnosticBag diagnostics)
        {
            // It is possible that we have a bracketed argument list, like "int x[];" or "int x[123];"
            // in a non-fixed-size-array declaration . This is a common error made by C++ programmers.
            // We have already given a good error at parse time telling the user to either make it "fixed"
            // or to move the brackets to the type. However, we should still do semantic analysis of
            // the arguments, so that errors in them are discovered, hovering over them in the IDE
            // gives good results, and so on.

            var arguments = default(ImmutableArray<BoundExpression>);

            if (declarator.ArgumentList != null)
            {
                AnalyzedArguments analyzedArguments = AnalyzedArguments.GetInstance();
                BindArgumentsAndNames(declarator.ArgumentList, diagnostics, analyzedArguments);
                arguments = BuildArgumentsForErrorRecovery(analyzedArguments);
                analyzedArguments.Free();
            }

            return arguments;
        }

        private SourceLocalSymbol LocateDeclaredVariableSymbol(VariableDeclaratorSyntax declarator, TypeSyntax typeSyntax, LocalDeclarationKind outerKind)
        {
            LocalDeclarationKind kind = outerKind == LocalDeclarationKind.UsingVariable ? LocalDeclarationKind.UsingVariable : LocalDeclarationKind.RegularVariable;
            return LocateDeclaredVariableSymbol(declarator.Identifier, typeSyntax, declarator.Initializer, kind);
        }

        private SourceLocalSymbol LocateDeclaredVariableSymbol(SyntaxToken identifier, TypeSyntax typeSyntax, EqualsValueClauseSyntax equalsValue, LocalDeclarationKind kind)
        {
            SourceLocalSymbol localSymbol = this.LookupLocal(identifier);

            // In error scenarios with misplaced code, it is possible we can't bind the local declaration.
            // This occurs through the semantic model.  In that case concoct a plausible result.
            localSymbol ??= SourceLocalSymbol.MakeLocal(
                    ContainingMemberOrLambda,
                    this,
                    false, // do not allow ref
                    typeSyntax,
                    identifier,
                    kind,
                    equalsValue);

            return localSymbol;
        }

        private bool IsValidFixedVariableInitializer(TypeSymbol declType, ref BoundExpression initializerOpt, BindingDiagnosticBag diagnostics)
        {

            if (initializerOpt?.HasAnyErrors != false)
            {
                return false;
            }

            TypeSymbol initializerType = initializerOpt.Type;
            SyntaxNode initializerSyntax = initializerOpt.Syntax;

            if (initializerType is null)
            {
                Error(diagnostics, ErrorCode.ERR_ExprCannotBeFixed, initializerSyntax);
                return false;
            }

            TypeSymbol elementType;
            bool hasErrors = false;
            MethodSymbol fixedPatternMethod = null;

            switch (initializerOpt.Kind)
            {
                case BoundKind.AddressOfOperator:
                    elementType = ((BoundAddressOfOperator)initializerOpt).Operand.Type;
                    break;

                case BoundKind.FieldAccess:
                    var fa = (BoundFieldAccess)initializerOpt;
                    if (fa.FieldSymbol.IsFixedSizeBuffer)
                    {
                        elementType = ((PointerTypeSymbol)fa.Type).PointedAtType;
                        break;
                    }

                    goto default;

                default:
                    //  fixed (T* variable = <expr>) ...

                    // check for arrays
                    if (initializerType.IsArray())
                    {
                        // See ExpressionBinder::BindPtrToArray (though most of that functionality is now in LocalRewriter).
                        elementType = ((ArrayTypeSymbol)initializerType).ElementType;
                        break;
                    }

                    // check for a special ref-returning method
                    var additionalDiagnostics = BindingDiagnosticBag.GetInstance(diagnostics);
                    fixedPatternMethod = GetFixedPatternMethodOpt(initializerOpt, additionalDiagnostics);

                    // check for String
                    // NOTE: We will allow the pattern method to take precedence, but only if it is an instance member of System.String
                    if (initializerType.SpecialType == SpecialType.System_String &&
                        (fixedPatternMethod is null || fixedPatternMethod.ContainingType.SpecialType != SpecialType.System_String))
                    {
                        fixedPatternMethod = null;
                        elementType = this.GetSpecialType(SpecialType.System_Char, diagnostics, initializerSyntax);
                        additionalDiagnostics.Free();
                        break;
                    }

                    // if the feature was enabled, but something went wrong with the method, report that, otherwise don't.
                    // If feature is not enabled, additional errors would be just noise.
                    bool extensibleFixedEnabled = ((CSharpParseOptions)initializerOpt.SyntaxTree.Options)?.IsFeatureEnabled(MessageID.IDS_FeatureExtensibleFixedStatement) != false;
                    if (extensibleFixedEnabled)
                    {
                        diagnostics.AddRange(additionalDiagnostics);
                    }

                    additionalDiagnostics.Free();

                    if (fixedPatternMethod is not null)
                    {
                        elementType = fixedPatternMethod.ReturnType;
                        CheckFeatureAvailability(initializerOpt.Syntax, MessageID.IDS_FeatureExtensibleFixedStatement, diagnostics);
                        break;
                    }
                    else
                    {
                        Error(diagnostics, ErrorCode.ERR_ExprCannotBeFixed, initializerSyntax);
                        return false;
                    }
            }

            if (CheckManagedAddr(Compilation, elementType, initializerSyntax.Location, diagnostics))
            {
                hasErrors = true;
            }

            initializerOpt = BindToNaturalType(initializerOpt, diagnostics, reportNoTargetType: false);
            initializerOpt = GetFixedLocalCollectionInitializer(initializerOpt, elementType, declType, fixedPatternMethod, hasErrors, diagnostics);
            return true;
        }

        private MethodSymbol GetFixedPatternMethodOpt(BoundExpression initializer, BindingDiagnosticBag additionalDiagnostics)
        {
            if (initializer.Type.IsVoidType())
            {
                return null;
            }

            const string methodName = "GetPinnableReference";

            PerformPatternMethodLookup(initializer, methodName, initializer.Syntax, additionalDiagnostics, out var patternMethodSymbol);

            if (patternMethodSymbol is null)
            {
                return null;
            }

            if (HasOptionalOrVariableParameters(patternMethodSymbol) ||
                patternMethodSymbol.ReturnsVoid ||
                !patternMethodSymbol.RefKind.IsManagedReference() ||
                !(patternMethodSymbol.ParameterCount == 0 || patternMethodSymbol.IsStatic && patternMethodSymbol.ParameterCount == 1))
            {
                // the method does not fit the pattern
                additionalDiagnostics.Add(ErrorCode.WRN_PatternBadSignature, initializer.Syntax.Location, initializer.Type, "fixed", patternMethodSymbol);
                return null;
            }

            return patternMethodSymbol;
        }

        /// <summary>
        /// Wrap the initializer in a BoundFixedLocalCollectionInitializer so that the rewriter will have the
        /// information it needs (e.g. conversions, helper methods).
        /// </summary>
        private BoundExpression GetFixedLocalCollectionInitializer(
            BoundExpression initializer,
            TypeSymbol elementType,
            TypeSymbol declType,
            MethodSymbol patternMethodOpt,
            bool hasErrors,
            BindingDiagnosticBag diagnostics)
        {

            SyntaxNode initializerSyntax = initializer.Syntax;

            TypeSymbol pointerType = new PointerTypeSymbol(TypeWithAnnotations.Create(elementType));
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            Conversion elementConversion = this.Conversions.ClassifyConversionFromType(pointerType, declType, ref useSiteInfo);
            diagnostics.Add(initializerSyntax, useSiteInfo);

            if (!elementConversion.IsValid || !elementConversion.IsImplicit)
            {
                GenerateImplicitConversionError(diagnostics, this.Compilation, initializerSyntax, elementConversion, pointerType, declType);
                hasErrors = true;
            }

            return new BoundFixedLocalCollectionInitializer(
                initializerSyntax,
                pointerType,
                elementConversion,
                initializer,
                patternMethodOpt,
                declType,
                hasErrors);
        }

        private BoundExpression BindAssignment(AssignmentExpressionSyntax node, BindingDiagnosticBag diagnostics)
        {

            node.Left.CheckDeconstructionCompatibleArgument(diagnostics);

            if (node.Left.Kind() == SyntaxKind.TupleExpression || node.Left.Kind() == SyntaxKind.DeclarationExpression)
            {
                return BindDeconstruction(node, diagnostics);
            }

            BindValueKind lhsKind;
            BindValueKind rhsKind;
            ExpressionSyntax rhsExpr;
            bool isRef = false;

            if (node.Right.Kind() == SyntaxKind.RefExpression)
            {
                isRef = true;
                lhsKind = BindValueKind.RefAssignable;
                rhsKind = BindValueKind.RefersToLocation;
                rhsExpr = ((RefExpressionSyntax)node.Right).Expression;
            }
            else
            {
                lhsKind = BindValueKind.Assignable;
                rhsKind = BindValueKind.RValue;
                rhsExpr = node.Right;
            }

            var op1 = BindValue(node.Left, diagnostics, lhsKind);
            ReportSuppressionIfNeeded(op1, diagnostics);

            // If the LHS is a ref (not ref-readonly), the rhs
            // must also be value-assignable
            if (lhsKind == BindValueKind.RefAssignable && !op1.HasErrors)
            {
                // We should now know that op1 is a valid lvalue
                RefKind lhsRefKind = op1.GetRefKind();
                if (lhsRefKind == RefKind.Ref || lhsRefKind == RefKind.Out)
                {
                    rhsKind |= BindValueKind.Assignable;
                }
            }

            var op2 = BindValue(rhsExpr, diagnostics, rhsKind);

            if (op1.Kind == BoundKind.DiscardExpression)
            {
                op2 = BindToNaturalType(op2, diagnostics);
                op1 = InferTypeForDiscardAssignment((BoundDiscardExpression)op1, op2, diagnostics);
            }

            return BindAssignment(node, op1, op2, isRef, diagnostics);
        }

        private BoundExpression InferTypeForDiscardAssignment(BoundDiscardExpression op1, BoundExpression op2, BindingDiagnosticBag diagnostics)
        {
            var inferredType = op2.Type;
            if (inferredType is null)
            {
                return op1.FailInference(this, diagnostics);
            }

            if (inferredType.IsVoidType())
            {
                diagnostics.Add(ErrorCode.ERR_VoidAssignment, op1.Syntax.Location);
            }

            return op1.SetInferredTypeWithAnnotations(TypeWithAnnotations.Create(inferredType));
        }

        private BoundAssignmentOperator BindAssignment(
            SyntaxNode node,
            BoundExpression op1,
            BoundExpression op2,
            bool isRef,
            BindingDiagnosticBag diagnostics)
        {

            bool hasErrors = op1.HasAnyErrors || op2.HasAnyErrors;

            if (!op1.HasAnyErrors)
            {
                // Build bound conversion. The node might not be used if this is a dynamic conversion
                // but diagnostics should be reported anyways.
                var conversion = GenerateConversionForAssignment(op1.Type, op2, diagnostics, isRefAssignment: isRef);

                // If the result is a dynamic assignment operation (SetMember or SetIndex),
                // don't generate the boxing conversion to the dynamic type.
                // Leave the values as they are, and deal with the conversions at runtime.
                if (op1.Kind != BoundKind.DynamicIndexerAccess &&
                    op1.Kind != BoundKind.DynamicMemberAccess &&
                    op1.Kind != BoundKind.DynamicObjectInitializerMember)
                {
                    op2 = conversion;
                }
                else
                {
                    op2 = BindToNaturalType(op2, diagnostics);
                }

                if (isRef)
                {
                    var leftEscape = GetRefEscape(op1, LocalScopeDepth);
                    var rightEscape = GetRefEscape(op2, LocalScopeDepth);
                    if (leftEscape < rightEscape)
                    {
                        Error(diagnostics, ErrorCode.ERR_RefAssignNarrower, node, op1.ExpressionSymbol.Name, op2.Syntax);
                        op2 = ToBadExpression(op2);
                    }
                }

                if (op1.Type.IsRefLikeType)
                {
                    var leftEscape = GetValEscape(op1, LocalScopeDepth);
                    op2 = ValidateEscape(op2, leftEscape, isByRef: false, diagnostics);
                }
            }
            else
            {
                op2 = BindToTypeForErrorRecovery(op2);
            }

            TypeSymbol type;
            if ((op1.Kind == BoundKind.EventAccess) &&
                ((BoundEventAccess)op1).EventSymbol.IsWindowsRuntimeEvent)
            {
                // Event assignment is a call to void WindowsRuntimeMarshal.AddEventHandler<T>().
                type = this.GetSpecialType(SpecialType.System_Void, diagnostics, node);
            }
            else
            {
                type = op1.Type;
            }

            return new BoundAssignmentOperator(node, op1, op2, isRef, type, hasErrors);
        }

        private static PropertySymbol GetPropertySymbol(BoundExpression expr, out BoundExpression receiver, out SyntaxNode propertySyntax)
        {
            PropertySymbol propertySymbol;
            switch (expr.Kind)
            {
                case BoundKind.PropertyAccess:
                    {
                        var propertyAccess = (BoundPropertyAccess)expr;
                        receiver = propertyAccess.ReceiverOpt;
                        propertySymbol = propertyAccess.PropertySymbol;
                    }
                    break;
                case BoundKind.IndexerAccess:
                    {
                        var indexerAccess = (BoundIndexerAccess)expr;
                        receiver = indexerAccess.ReceiverOpt;
                        propertySymbol = indexerAccess.Indexer;
                    }
                    break;
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    {
                        var patternIndexer = (BoundIndexOrRangePatternIndexerAccess)expr;
                        receiver = patternIndexer.Receiver;
                        propertySymbol = (PropertySymbol)patternIndexer.PatternSymbol;
                    }
                    break;
                default:
                    receiver = null;
                    //propertySymbol = null;
                    propertySyntax = null;
                    return null;
            }

            var syntax = expr.Syntax;
            propertySyntax = syntax.Kind() switch
            {
                SyntaxKind.SimpleMemberAccessExpression or SyntaxKind.PointerMemberAccessExpression => ((MemberAccessExpressionSyntax)syntax).Name,
                SyntaxKind.IdentifierName => syntax,
                SyntaxKind.ElementAccessExpression => ((ElementAccessExpressionSyntax)syntax).ArgumentList,
                _ => syntax,// Other syntax types, such as QualifiedName,
                            // might occur in invalid code.
            };
            return propertySymbol;
        }

        private static SyntaxNode GetEventName(BoundEventAccess expr)
        {
            SyntaxNode syntax = expr.Syntax;
            return syntax.Kind() switch
            {
                SyntaxKind.SimpleMemberAccessExpression or SyntaxKind.PointerMemberAccessExpression => ((MemberAccessExpressionSyntax)syntax).Name,
                SyntaxKind.QualifiedName => ((QualifiedNameSyntax)syntax).Right,// This case is reachable only through SemanticModel
                SyntaxKind.IdentifierName => syntax,
                SyntaxKind.MemberBindingExpression => ((MemberBindingExpressionSyntax)syntax).Name,
                _ => throw ExceptionUtilities.UnexpectedValue(syntax.Kind()),
            };
        }

        /// <summary>
        /// There are two BadEventUsage error codes and this method decides which one should
        /// be used for a given event.
        /// </summary>
        private DiagnosticInfo GetBadEventUsageDiagnosticInfo(EventSymbol eventSymbol)
        {
            var leastOverridden = (EventSymbol)eventSymbol.GetLeastOverriddenMember(this.ContainingType);
            return leastOverridden.HasAssociatedField ?
                new CSDiagnosticInfo(ErrorCode.ERR_BadEventUsage, leastOverridden, leastOverridden.ContainingType) :
                new CSDiagnosticInfo(ErrorCode.ERR_BadEventUsageNoField, leastOverridden);
        }

        internal static bool AccessingAutoPropertyFromConstructor(BoundPropertyAccess propertyAccess, Symbol fromMember)
        {
            return AccessingAutoPropertyFromConstructor(propertyAccess.ReceiverOpt, propertyAccess.PropertySymbol, fromMember);
        }

        private static bool AccessingAutoPropertyFromConstructor(BoundExpression receiver, PropertySymbol propertySymbol, Symbol fromMember)
        {
            if (!propertySymbol.IsDefinition && propertySymbol.ContainingType.Equals(propertySymbol.ContainingType.OriginalDefinition, TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                propertySymbol = propertySymbol.OriginalDefinition;
            }

            var propertyIsStatic = propertySymbol.IsStatic;

            return propertySymbol is SourcePropertySymbolBase sourceProperty &&
                    sourceProperty.IsAutoPropertyWithGetAccessor &&
                    TypeSymbol.Equals(sourceProperty.ContainingType, fromMember.ContainingType, TypeCompareKind.ConsiderEverything2) &&
                    IsConstructorOrField(fromMember, isStatic: propertyIsStatic) &&
                    (propertyIsStatic || receiver.Kind == BoundKind.ThisReference);
        }

        private static bool IsConstructorOrField(Symbol member, bool isStatic)
        {
            return (member as MethodSymbol)?.MethodKind == (isStatic ?
                                                                MethodKind.StaticConstructor :
                                                                MethodKind.Constructor) ||
                    (member as FieldSymbol)?.IsStatic == isStatic;
        }

        private TypeSymbol GetAccessThroughType(BoundExpression receiver)
        {
            if (receiver == null)
            {
                return this.ContainingType;
            }
            else if (receiver.Kind == BoundKind.BaseReference)
            {
                // Allow protected access to members defined
                // in base classes. See spec section 3.5.3.
                return null;
            }
            else
            {
                return receiver.Type;
            }
        }

        private BoundExpression BindPossibleArrayInitializer(
            ExpressionSyntax node,
            TypeSymbol destinationType,
            BindValueKind valueKind,
            BindingDiagnosticBag diagnostics)
        {

            if (node.Kind() != SyntaxKind.ArrayInitializerExpression)
            {
                return BindValue(node, diagnostics, valueKind);
            }

            BoundExpression result;
            if (destinationType.Kind == SymbolKind.ArrayType)
            {
                result = BindArrayCreationWithInitializer(diagnostics, null,
                    (InitializerExpressionSyntax)node, (ArrayTypeSymbol)destinationType,
                    ImmutableArray<BoundExpression>.Empty);
            }
            else
            {
                result = BindUnexpectedArrayInitializer((InitializerExpressionSyntax)node, diagnostics, ErrorCode.ERR_ArrayInitToNonArrayType);
            }

            return CheckValue(result, valueKind, diagnostics);
        }

        protected virtual SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            return Next.LookupLocal(nameToken);
        }

        protected virtual LocalFunctionSymbol LookupLocalFunction(SyntaxToken nameToken)
        {
            return Next.LookupLocalFunction(nameToken);
        }

        /// <summary>
        /// Returns a value that tells how many local scopes are visible, including the current.
        /// I.E. outside of any method will be 0
        ///      immediately inside a method - 1
        /// </summary>
        internal virtual uint LocalScopeDepth => Next.LocalScopeDepth;

        internal virtual BoundBlock BindEmbeddedBlock(BlockSyntax node, BindingDiagnosticBag diagnostics)
        {
            return BindBlock(node, diagnostics);
        }

        private BoundBlock BindBlock(BlockSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (node.AttributeLists.Count > 0)
            {
                Error(diagnostics, ErrorCode.ERR_AttributesNotAllowed, node.AttributeLists[0]);
            }

            var binder = GetBinder(node);

            return binder.BindBlockParts(node, diagnostics);
        }

        private BoundBlock BindBlockParts(BlockSyntax node, BindingDiagnosticBag diagnostics)
        {
            var syntaxStatements = node.Statements;
            int nStatements = syntaxStatements.Count;

            ArrayBuilder<BoundStatement> boundStatements = ArrayBuilder<BoundStatement>.GetInstance(nStatements);

            for (int i = 0; i < nStatements; i++)
            {
                var boundStatement = BindStatement(syntaxStatements[i], diagnostics);
                boundStatements.Add(boundStatement);
            }

            return FinishBindBlockParts(node, boundStatements.ToImmutableAndFree()/*, diagnostics*/);
        }

        private BoundBlock FinishBindBlockParts(CSharpSyntaxNode node, ImmutableArray<BoundStatement> boundStatements/*, BindingDiagnosticBag diagnostics*/)
        {
            ImmutableArray<LocalSymbol> locals = GetDeclaredLocalsForScope(node);

            if (IsDirectlyInIterator)
            {
                if (ContainingMemberOrLambda is MethodSymbol method)
                {
                    method.IteratorElementTypeWithAnnotations = GetIteratorElementType();
                }
            }

            return new BoundBlock(
                node,
                locals,
                GetDeclaredLocalFunctionsForScope(node),
                boundStatements);
        }

        internal BoundExpression GenerateConversionForAssignment(TypeSymbol targetType, BoundExpression expression, BindingDiagnosticBag diagnostics, bool isDefaultParameter = false, bool isRefAssignment = false)
        {

            // We wish to avoid "cascading" errors, so if the expression we are
            // attempting to convert to a type had errors, suppress additional
            // diagnostics. However, if the expression
            // with errors is an unbound lambda then the errors are almost certainly
            // syntax errors. For error recovery analysis purposes we wish to bind
            // error lambdas like "Action<int> f = x=>{ x. };" because IntelliSense
            // needs to know that x is of type int.

            if (expression.HasAnyErrors && expression.Kind != BoundKind.UnboundLambda)
            {
                diagnostics = BindingDiagnosticBag.Discarded;
            }

            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            var conversion = this.Conversions.ClassifyConversionFromExpression(expression, targetType, ref useSiteInfo);
            diagnostics.Add(expression.Syntax, useSiteInfo);

            if (isRefAssignment)
            {
                if (conversion.Kind != ConversionKind.Identity)
                {
                    Error(diagnostics, ErrorCode.ERR_RefAssignmentMustHaveIdentityConversion, expression.Syntax, targetType);
                }
                else
                {
                    return expression;
                }
            }
            else if (!conversion.IsImplicit || !conversion.IsValid)
            {
                // We suppress conversion errors on default parameters; eg,
                // if someone says "void M(string s = 123) {}". We will report
                // a special error in the default parameter binder.

                if (!isDefaultParameter)
                {
                    GenerateImplicitConversionError(diagnostics, expression.Syntax, conversion, expression, targetType);
                }

                // Suppress any additional diagnostics
                diagnostics = BindingDiagnosticBag.Discarded;
            }

            return CreateConversion(expression.Syntax, expression, conversion, isCast: false, conversionGroupOpt: null, targetType, diagnostics);
        }

        internal void GenerateAnonymousFunctionConversionError(BindingDiagnosticBag diagnostics, SyntaxNode syntax,
            UnboundLambda anonymousFunction, TypeSymbol targetType)
        {

            // Is the target type simply bad?

            // If the target type is an error then we've already reported a diagnostic. Don't bother
            // reporting the conversion error.
            if (targetType.IsErrorType())
            {
                return;
            }

            // CONSIDER: Instead of computing this again, cache the reason why the conversion failed in
            // CONSIDER: the Conversion result, and simply report that.

            var reason = Conversions.IsAnonymousFunctionCompatibleWithType(anonymousFunction, targetType);

            // It is possible that the conversion from lambda to delegate is just fine, and
            // that we ended up here because the target type, though itself is not an error
            // type, contains a type argument which is an error type. For example, converting
            // (Goo goo)=>{} to Action<Goo> is a perfectly legal conversion even if Goo is undefined!
            // In that case we have already reported an error that Goo is undefined, so just bail out.

            if (reason == LambdaConversionResult.Success)
            {
                return;
            }

            var id = anonymousFunction.MessageID.Localize();

            if (reason == LambdaConversionResult.BadTargetType)
            {
                if (ReportDelegateInvokeUseSiteDiagnostic(diagnostics, targetType, node: syntax))
                {
                    return;
                }

                // Cannot convert {0} to type '{1}' because it is not a delegate type
                Error(diagnostics, ErrorCode.ERR_AnonMethToNonDel, syntax, id, targetType);
                return;
            }

            if (reason == LambdaConversionResult.ExpressionTreeMustHaveDelegateTypeArgument)
            {
                Error(diagnostics, ErrorCode.ERR_ExpressionTreeMustHaveDelegate, syntax, ((NamedTypeSymbol)targetType).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type);
                return;
            }

            if (reason == LambdaConversionResult.ExpressionTreeFromAnonymousMethod)
            {
                Error(diagnostics, ErrorCode.ERR_AnonymousMethodToExpressionTree, syntax);
                return;
            }

            // At this point we know that we have either a delegate type or an expression type for the target.

            var delegateType = targetType.GetDelegateType();

            // The target type is a valid delegate or expression tree type. Is there something wrong with the
            // parameter list?

            // First off, is there a parameter list at all?

            if (reason == LambdaConversionResult.MissingSignatureWithOutParameter)
            {
                // COMPATIBILITY: The C# 4 compiler produces two errors for:
                //
                // delegate void D (out int x);
                // ...
                // D d = delegate {};
                //
                // error CS1676: Parameter 1 must be declared with the 'out' keyword
                // error CS1688: Cannot convert anonymous method block without a parameter list
                // to delegate type 'D' because it has one or more out parameters
                //
                // This seems redundant, (because there is no "parameter 1" in the source code)
                // and unnecessary. I propose that we eliminate the first error.

                Error(diagnostics, ErrorCode.ERR_CantConvAnonMethNoParams, syntax, targetType);
                return;
            }

            // There is a parameter list. Does it have the right number of elements?

            if (reason == LambdaConversionResult.BadParameterCount)
            {
                // Delegate '{0}' does not take {1} arguments
                Error(diagnostics, ErrorCode.ERR_BadDelArgCount, syntax, delegateType, anonymousFunction.ParameterCount);
                return;
            }

            // The parameter list exists and had the right number of parameters. Were any of its types bad?

            // If any parameter type of the lambda is an error type then suppress
            // further errors. We've already reported errors on the bad type.
            if (anonymousFunction.HasExplicitlyTypedParameterList)
            {
                for (int i = 0; i < anonymousFunction.ParameterCount; ++i)
                {
                    if (anonymousFunction.ParameterType(i).IsErrorType())
                    {
                        return;
                    }
                }
            }

            // The parameter list exists and had the right number of parameters. Were any of its types
            // mismatched with the delegate parameter types?

            // The simplest possible case is (x, y, z)=>whatever where the target type has a ref or out parameter.

            var delegateParameters = delegateType.DelegateParameters();
            if (reason == LambdaConversionResult.RefInImplicitlyTypedLambda)
            {
                for (int i = 0; i < anonymousFunction.ParameterCount; ++i)
                {
                    var delegateRefKind = delegateParameters[i].RefKind;
                    if (delegateRefKind != RefKind.None)
                    {
                        // Parameter {0} must be declared with the '{1}' keyword
                        Error(diagnostics, ErrorCode.ERR_BadParamRef, anonymousFunction.ParameterLocation(i),
                            i + 1, delegateRefKind.ToParameterDisplayString());
                    }
                }
                return;
            }

            // See the comments in IsAnonymousFunctionCompatibleWithDelegate for an explanation of this one.
            if (reason == LambdaConversionResult.StaticTypeInImplicitlyTypedLambda)
            {
                for (int i = 0; i < anonymousFunction.ParameterCount; ++i)
                {
                    if (delegateParameters[i].TypeWithAnnotations.IsStatic)
                    {
                        // {0}: Static types cannot be used as parameter
                        Error(diagnostics, ErrorFacts.GetStaticClassParameterCode(useWarning: false), anonymousFunction.ParameterLocation(i), delegateParameters[i].Type);
                    }
                }
                return;
            }

            // Otherwise, there might be a more complex reason why the parameter types are mismatched.

            if (reason == LambdaConversionResult.MismatchedParameterType)
            {
                // Cannot convert {0} to type '{1}' because the parameter types do not match the delegate parameter types
                Error(diagnostics, ErrorCode.ERR_CantConvAnonMethParams, syntax, id, targetType);
                for (int i = 0; i < anonymousFunction.ParameterCount; ++i)
                {
                    var lambdaParameterType = anonymousFunction.ParameterType(i);
                    if (lambdaParameterType.IsErrorType())
                    {
                        continue;
                    }

                    var lambdaParameterLocation = anonymousFunction.ParameterLocation(i);
                    var lambdaRefKind = anonymousFunction.RefKind(i);
                    var delegateParameterType = delegateParameters[i].Type;
                    var delegateRefKind = delegateParameters[i].RefKind;

                    if (!lambdaParameterType.Equals(delegateParameterType, TypeCompareKind.AllIgnoreOptions))
                    {
                        SymbolDistinguisher distinguisher = new(this.Compilation, lambdaParameterType, delegateParameterType);

                        // Parameter {0} is declared as type '{1}{2}' but should be '{3}{4}'
                        Error(diagnostics, ErrorCode.ERR_BadParamType, lambdaParameterLocation,
                            i + 1, lambdaRefKind.ToParameterPrefix(), distinguisher.First, delegateRefKind.ToParameterPrefix(), distinguisher.Second);
                    }
                    else if (lambdaRefKind != delegateRefKind)
                    {
                        if (delegateRefKind == RefKind.None)
                        {
                            // Parameter {0} should not be declared with the '{1}' keyword
                            Error(diagnostics, ErrorCode.ERR_BadParamExtraRef, lambdaParameterLocation, i + 1, lambdaRefKind.ToParameterDisplayString());
                        }
                        else
                        {
                            // Parameter {0} must be declared with the '{1}' keyword
                            Error(diagnostics, ErrorCode.ERR_BadParamRef, lambdaParameterLocation, i + 1, delegateRefKind.ToParameterDisplayString());
                        }
                    }
                }
                return;
            }

            if (reason == LambdaConversionResult.BindingFailed)
            {
                var bindingResult = anonymousFunction.Bind(delegateType);
                diagnostics.AddRange(bindingResult.Diagnostics);
                return;
            }

            // UNDONE: LambdaConversionResult.VoidExpressionLambdaMustBeStatementExpression:

            diagnostics.Add(ErrorCode.ERR_InternalError, syntax.Location);
        }

        protected static void GenerateImplicitConversionError(BindingDiagnosticBag diagnostics, CSharpCompilation compilation, SyntaxNode syntax,
            Conversion conversion, TypeSymbol sourceType, TypeSymbol targetType, ConstantValue sourceConstantValueOpt = null)
        {

            // If the either type is an error then an error has already been reported
            // for some aspect of the analysis of this expression. (For example, something like
            // "garbage g = null; short s = g;" -- we don't want to report that g is not
            // convertible to short because we've already reported that g does not have a good type.
            if (!sourceType.IsErrorType() && !targetType.IsErrorType())
            {
                if (conversion.IsExplicit)
                {
                    if (sourceType.SpecialType == SpecialType.System_Double && syntax.Kind() == SyntaxKind.NumericLiteralExpression &&
                        (targetType.SpecialType == SpecialType.System_Single || targetType.SpecialType == SpecialType.System_Decimal))
                    {
                        Error(diagnostics, ErrorCode.ERR_LiteralDoubleCast, syntax, (targetType.SpecialType == SpecialType.System_Single) ? "F" : "M", targetType);
                    }
                    else if (conversion.Kind == ConversionKind.ExplicitNumeric && sourceConstantValueOpt != null && sourceConstantValueOpt != ConstantValue.Bad &&
                        ConversionsBase.HasImplicitConstantExpressionConversion(new BoundLiteral(syntax, ConstantValue.Bad, sourceType), targetType))
                    {
                        // CLEVERNESS: By passing ConstantValue.Bad, we tell HasImplicitConstantExpressionConversion to ignore the constant
                        // value and only consider the types.

                        // If there would be an implicit constant conversion for a different constant of the same type
                        // (i.e. one that's not out of range), then it's more helpful to report the range check failure
                        // than to suggest inserting a cast.
                        Error(diagnostics, ErrorCode.ERR_ConstOutOfRange, syntax, sourceConstantValueOpt.Value, targetType);
                    }
                    else
                    {
                        SymbolDistinguisher distinguisher = new(compilation, sourceType, targetType);
                        Error(diagnostics, ErrorCode.ERR_NoImplicitConvCast, syntax, distinguisher.First, distinguisher.Second);
                    }
                }
                else if (conversion.ResultKind() == LookupResultKind.OverloadResolutionFailure)
                {

                    ImmutableArray<MethodSymbol> originalUserDefinedConversions = conversion.OriginalUserDefinedConversions;
                    if (originalUserDefinedConversions.Length > 1)
                    {
                        Error(diagnostics, ErrorCode.ERR_AmbigUDConv, syntax, originalUserDefinedConversions[0], originalUserDefinedConversions[1], sourceType, targetType);
                    }
                    else
                    {
                        SymbolDistinguisher distinguisher = new(compilation, sourceType, targetType);
                        Error(diagnostics, ErrorCode.ERR_NoImplicitConv, syntax, distinguisher.First, distinguisher.Second);
                    }
                }
                else if (TypeSymbol.Equals(sourceType, targetType, TypeCompareKind.ConsiderEverything2))
                {
                    // This occurs for `void`, which cannot even convert to itself. Since SymbolDistinguisher
                    // requires two distinct types, we preempt its use here. The diagnostic is strange, but correct.
                    // Though this diagnostic tends to be a cascaded one, we cannot suppress it until
                    // we have proven that it is always so.
                    Error(diagnostics, ErrorCode.ERR_NoImplicitConv, syntax, sourceType, targetType);
                }
                else
                {
                    SymbolDistinguisher distinguisher = new(compilation, sourceType, targetType);
                    Error(diagnostics, ErrorCode.ERR_NoImplicitConv, syntax, distinguisher.First, distinguisher.Second);
                }
            }
        }

        protected void GenerateImplicitConversionError(
            BindingDiagnosticBag diagnostics,
            SyntaxNode syntax,
            Conversion conversion,
            BoundExpression operand,
            TypeSymbol targetType)
        {

            if (targetType.TypeKind == TypeKind.Error)
            {
                return;
            }

            if (targetType.IsVoidType())
            {
                Error(diagnostics, ErrorCode.ERR_NoImplicitConv, syntax, operand.Display, targetType);
                return;
            }

            switch (operand.Kind)
            {
                case BoundKind.BadExpression:
                    {
                        return;
                    }
                case BoundKind.UnboundLambda:
                    {
                        GenerateAnonymousFunctionConversionError(diagnostics, syntax, (UnboundLambda)operand, targetType);
                        return;
                    }
                case BoundKind.TupleLiteral:
                    {
                        var tuple = (BoundTupleLiteral)operand;

                        // If target is a tuple or compatible type with the same number of elements,
                        // report errors for tuple arguments that failed to convert, which would be more useful.
                        if (targetType.TryGetElementTypesWithAnnotationsIfTupleType(out ImmutableArray<TypeWithAnnotations> targetElementTypes) &&
                            targetElementTypes.Length == tuple.Arguments.Length)
                        {
                            GenerateImplicitConversionErrorsForTupleLiteralArguments(diagnostics, tuple.Arguments, targetElementTypes);
                            return;
                        }

                        // target is not compatible with source and source does not have a type
                        if (tuple.Type is null)
                        {
                            Error(diagnostics, ErrorCode.ERR_ConversionNotTupleCompatible, syntax, tuple.Arguments.Length, targetType);
                            return;
                        }

                        // Otherwise it is just a regular conversion failure from T1 to T2.
                        break;
                    }
                case BoundKind.MethodGroup:
                    {
                        reportMethodGroupErrors((BoundMethodGroup)operand, fromAddressOf: false);
                        return;
                    }
                case BoundKind.UnconvertedAddressOfOperator:
                    {
                        reportMethodGroupErrors(((BoundUnconvertedAddressOfOperator)operand).Operand, fromAddressOf: true);
                        return;
                    }
                case BoundKind.Literal:
                    {
                        if (operand.IsLiteralNull())
                        {
                            if (targetType.TypeKind == TypeKind.TypeParameter)
                            {
                                Error(diagnostics, ErrorCode.ERR_TypeVarCantBeNull, syntax, targetType);
                                return;
                            }
                            if (targetType.IsValueType)
                            {
                                Error(diagnostics, ErrorCode.ERR_ValueCantBeNull, syntax, targetType);
                                return;
                            }
                        }
                        break;
                    }
                case BoundKind.StackAllocArrayCreation:
                    {
                        var stackAllocExpression = (BoundStackAllocArrayCreation)operand;
                        Error(diagnostics, ErrorCode.ERR_StackAllocConversionNotPossible, syntax, stackAllocExpression.ElementType, targetType);
                        return;
                    }
                case BoundKind.UnconvertedSwitchExpression:
                    {
                        var switchExpression = (BoundUnconvertedSwitchExpression)operand;
                        var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                        bool reportedError = false;
                        foreach (var arm in switchExpression.SwitchArms)
                        {
                            tryConversion(arm.Value, ref reportedError, ref discardedUseSiteInfo);
                        }

                        return;
                    }

                case BoundKind.AddressOfOperator when targetType.IsFunctionPointer():
                    {
                        Error(diagnostics, ErrorCode.ERR_InvalidAddrOp, ((BoundAddressOfOperator)operand).Operand.Syntax);
                        return;
                    }
                case BoundKind.UnconvertedConditionalOperator:
                    {
                        var conditionalOperator = (BoundUnconvertedConditionalOperator)operand;
                        var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                        bool reportedError = false;
                        tryConversion(conditionalOperator.Consequence, ref reportedError, ref discardedUseSiteInfo);
                        tryConversion(conditionalOperator.Alternative, ref reportedError, ref discardedUseSiteInfo);
                        return;
                    }

                    void tryConversion(BoundExpression expr, ref bool reportedError, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
                    {
                        var conversion = this.Conversions.ClassifyImplicitConversionFromExpression(expr, targetType, ref useSiteInfo);
                        if (!conversion.IsImplicit || !conversion.IsValid)
                        {
                            GenerateImplicitConversionError(diagnostics, expr.Syntax, conversion, expr, targetType);
                            reportedError = true;
                        }
                    }
            }

            var sourceType = operand.Type;
            if (sourceType is not null)
            {
                GenerateImplicitConversionError(diagnostics, this.Compilation, syntax, conversion, sourceType, targetType, operand.ConstantValue);
                return;
            }


            void reportMethodGroupErrors(BoundMethodGroup methodGroup, bool fromAddressOf)
            {
                if (!Conversions.ReportDelegateOrFunctionPointerMethodGroupDiagnostics(this, methodGroup, targetType, diagnostics))
                {
                    var nodeForError = syntax;
                    while (nodeForError.Kind() == SyntaxKind.ParenthesizedExpression)
                    {
                        nodeForError = ((ParenthesizedExpressionSyntax)nodeForError).Expression;
                    }

                    if (nodeForError.Kind() == SyntaxKind.SimpleMemberAccessExpression || nodeForError.Kind() == SyntaxKind.PointerMemberAccessExpression)
                    {
                        nodeForError = ((MemberAccessExpressionSyntax)nodeForError).Name;
                    }

                    var location = nodeForError.Location;

                    if (ReportDelegateInvokeUseSiteDiagnostic(diagnostics, targetType, location))
                    {
                        return;
                    }

                    ErrorCode errorCode;

                    switch (targetType.TypeKind)
                    {
                        case TypeKind.FunctionPointer when fromAddressOf:
                            errorCode = ErrorCode.ERR_MethFuncPtrMismatch;
                            break;
                        case TypeKind.FunctionPointer:
                            Error(diagnostics, ErrorCode.ERR_MissingAddressOf, location);
                            return;
                        case TypeKind.Delegate when fromAddressOf:
                            errorCode = ErrorCode.ERR_CannotConvertAddressOfToDelegate;
                            break;
                        case TypeKind.Delegate:
                            errorCode = ErrorCode.ERR_MethDelegateMismatch;
                            break;
                        default:
                            if (fromAddressOf)
                            {
                                errorCode = ErrorCode.ERR_AddressOfToNonFunctionPointer;
                            }
                            else if (targetType.SpecialType == SpecialType.System_Delegate &&
                                syntax.IsFeatureEnabled(MessageID.IDS_FeatureInferredDelegateType))
                            {
                                Error(diagnostics, ErrorCode.ERR_CannotInferDelegateType, location);
                                return;
                            }
                            else
                            {
                                errorCode = ErrorCode.ERR_MethGrpToNonDel;
                            }
                            break;
                    }

                    Error(diagnostics, errorCode, location, methodGroup.Name, targetType);
                }
            }
        }

        private void GenerateImplicitConversionErrorsForTupleLiteralArguments(
            BindingDiagnosticBag diagnostics,
            ImmutableArray<BoundExpression> tupleArguments,
            ImmutableArray<TypeWithAnnotations> targetElementTypes)
        {
            _ = tupleArguments.Length;

            // report all leaf elements of the tuple literal that failed to convert
            // NOTE: we are not responsible for reporting use site errors here, just the failed leaf conversions.
            // By the time we get here we have done analysis and know we have failed the cast in general, and diagnostics collected in the process is already in the bag.
            // The only thing left is to form a diagnostics about the actually failing conversion(s).
            // This whole method does not itself collect any usesite diagnostics. Its only purpose is to produce an error better than "conversion failed here"
            var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;

            for (int i = 0; i < targetElementTypes.Length; i++)
            {
                var argument = tupleArguments[i];
                var targetElementType = targetElementTypes[i].Type;

                var elementConversion = Conversions.ClassifyImplicitConversionFromExpression(argument, targetElementType, ref discardedUseSiteInfo);
                if (!elementConversion.IsValid)
                {
                    GenerateImplicitConversionError(diagnostics, argument.Syntax, elementConversion, argument, targetElementType);
                }
            }
        }

        private BoundStatement BindIfStatement(IfStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var condition = BindBooleanExpression(node.Condition, diagnostics);
            var consequence = BindPossibleEmbeddedStatement(node.Statement, diagnostics);
            BoundStatement alternative = (node.Else == null) ? null : BindPossibleEmbeddedStatement(node.Else.Statement, diagnostics);

            BoundStatement result = new BoundIfStatement(node, condition, consequence, alternative);
            return result;
        }

        internal BoundExpression BindBooleanExpression(ExpressionSyntax node, BindingDiagnosticBag diagnostics)
        {
            // SPEC:
            // A boolean-expression is an expression that yields a result of type bool;
            // either directly or through application of operator true in certain
            // contexts as specified in the following.
            //
            // The controlling conditional expression of an if-statement, while-statement,
            // do-statement, or for-statement is a boolean-expression. The controlling
            // conditional expression of the ?: operator follows the same rules as a
            // boolean-expression, but for reasons of operator precedence is classified
            // as a conditional-or-expression.
            //
            // A boolean-expression is required to be implicitly convertible to bool
            // or of a type that implements operator true. If neither requirement
            // is satisfied, a binding-time error occurs.
            //
            // When a boolean expression cannot be implicitly converted to bool but does
            // implement operator true, then following evaluation of the expression,
            // the operator true implementation provided by that type is invoked
            // to produce a bool value.
            //
            // SPEC ERROR: The third paragraph above is obviously not correct; we need
            // SPEC ERROR: to do more than just check to see whether the type implements
            // SPEC ERROR: operator true. First off, the type could implement the operator
            // SPEC ERROR: several times: if it is a struct then it could implement it
            // SPEC ERROR: twice, to take both nullable and non-nullable arguments, and
            // SPEC ERROR: if it is a class or type parameter then it could have several
            // SPEC ERROR: implementations on its base classes or effective base classes.
            // SPEC ERROR: Second, the type of the argument could be S? where S implements
            // SPEC ERROR: operator true(S?); we want to look at S, not S?, when looking
            // SPEC ERROR: for applicable candidates.
            //
            // SPEC ERROR: Basically, the spec should say "use unary operator overload resolution
            // SPEC ERROR: to find the candidate set and choose a unique best operator true".

            var expr = BindValue(node, diagnostics, BindValueKind.RValue);
            var boolean = GetSpecialType(SpecialType.System_Boolean, diagnostics, node);

            if (expr.HasAnyErrors)
            {
                // The expression could not be bound. Insert a fake conversion
                // around it to bool and keep on going.
                // NOTE: no user-defined conversion candidates.
                return BoundConversion.Synthesized(node, BindToTypeForErrorRecovery(expr), Conversion.NoConversion, false, explicitCastInCode: false, conversionGroupOpt: null, ConstantValue.NotAvailable, boolean, hasErrors: true);
            }

            // Oddly enough, "if(dyn)" is bound not as a dynamic conversion to bool, but as a dynamic
            // invocation of operator true.

            if (expr.HasDynamicType())
            {
                return new BoundUnaryOperator(
                    node,
                    UnaryOperatorKind.DynamicTrue,
                    BindToNaturalType(expr, diagnostics),
                    ConstantValue.NotAvailable,
                    null,
                    LookupResultKind.Viable,
                    boolean)
                {
                    WasCompilerGenerated = true
                };
            }

            // Is the operand implicitly convertible to bool?

            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            var conversion = this.Conversions.ClassifyConversionFromExpression(expr, boolean, ref useSiteInfo);
            diagnostics.Add(expr.Syntax, useSiteInfo);

            if (conversion.IsImplicit)
            {
                if (conversion.Kind == ConversionKind.Identity)
                {
                    // Check to see if we're assigning a boolean literal in a place where an
                    // equality check would be more conventional.
                    // NOTE: Don't do this check unless the expression will be returned
                    // without being wrapped in another bound node (i.e. identity conversion).
                    if (expr.Kind == BoundKind.AssignmentOperator)
                    {
                        var assignment = (BoundAssignmentOperator)expr;
                        if (assignment.Right.Kind == BoundKind.Literal && assignment.Right.ConstantValue.Discriminator == ConstantValueTypeDiscriminator.Boolean)
                        {
                            Error(diagnostics, ErrorCode.WRN_IncorrectBooleanAssg, assignment.Syntax);
                        }
                    }
                }

                return CreateConversion(
                    syntax: expr.Syntax,
                    source: expr,
                    conversion: conversion,
                    isCast: false,
                    conversionGroupOpt: null,
                    wasCompilerGenerated: true,
                    destination: boolean,
                    diagnostics: diagnostics);
            }

            // It was not. Does it implement operator true?
            expr = BindToNaturalType(expr, diagnostics);
            var best = this.UnaryOperatorOverloadResolution(UnaryOperatorKind.True, expr, node, diagnostics, out LookupResultKind resultKind, out ImmutableArray<MethodSymbol> originalUserDefinedOperators);
            if (!best.HasValue)
            {
                // No. Give a "not convertible to bool" error.
                GenerateImplicitConversionError(diagnostics, node, conversion, expr, boolean);
                return BoundConversion.Synthesized(node, expr, Conversion.NoConversion, false, explicitCastInCode: false, conversionGroupOpt: null, ConstantValue.NotAvailable, boolean, hasErrors: true);
            }

            UnaryOperatorSignature signature = best.Signature;

            BoundExpression resultOperand = CreateConversion(
                node,
                expr,
                best.Conversion,
                isCast: false,
                conversionGroupOpt: null,
                destination: best.Signature.OperandType,
                diagnostics: diagnostics);

            // Consider op_true to be compiler-generated so that it doesn't appear in the semantic model.
            // UNDONE: If we decide to expose the operator in the semantic model, we'll have to remove the
            // WasCompilerGenerated flag (and possibly suppress the symbol in specific APIs).
            return new BoundUnaryOperator(node, signature.Kind, resultOperand, ConstantValue.NotAvailable, signature.Method, resultKind, originalUserDefinedOperators, signature.ReturnType)
            {
                WasCompilerGenerated = true
            };
        }

        private BoundStatement BindSwitchStatement(SwitchStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            Binder switchBinder = this.GetBinder(node);
            return switchBinder.BindSwitchStatementCore(node, switchBinder, diagnostics);
        }

        internal virtual BoundStatement BindSwitchStatementCore(SwitchStatementSyntax node, Binder originalBinder, BindingDiagnosticBag diagnostics)
        {
            return this.Next.BindSwitchStatementCore(node, originalBinder, diagnostics);
        }

        internal virtual void BindPatternSwitchLabelForInference(CasePatternSwitchLabelSyntax node, BindingDiagnosticBag diagnostics)
        {
            this.Next.BindPatternSwitchLabelForInference(node, diagnostics);
        }

        private BoundStatement BindWhile(WhileStatementSyntax node, BindingDiagnosticBag diagnostics)
        {

            var loopBinder = this.GetBinder(node);
            return loopBinder.BindWhileParts(diagnostics, loopBinder);
        }

        internal virtual BoundWhileStatement BindWhileParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindWhileParts(diagnostics, originalBinder);
        }

        private BoundStatement BindDo(DoStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var loopBinder = this.GetBinder(node);

            return loopBinder.BindDoParts(diagnostics, loopBinder);
        }

        internal virtual BoundDoStatement BindDoParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindDoParts(diagnostics, originalBinder);
        }

        internal BoundForStatement BindFor(ForStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var loopBinder = this.GetBinder(node);
            return loopBinder.BindForParts(diagnostics, loopBinder);
        }

        internal virtual BoundForStatement BindForParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindForParts(diagnostics, originalBinder);
        }

        internal BoundStatement BindForOrUsingOrFixedDeclarations(VariableDeclarationSyntax nodeOpt, LocalDeclarationKind localKind, BindingDiagnosticBag diagnostics, out ImmutableArray<BoundLocalDeclaration> declarations)
        {
            if (nodeOpt == null)
            {
                declarations = ImmutableArray<BoundLocalDeclaration>.Empty;
                return null;
            }

            var typeSyntax = nodeOpt.Type;
            // Fixed and using variables are not allowed to be ref-like, but regular variables are
            if (localKind == LocalDeclarationKind.RegularVariable)
            {
                typeSyntax = typeSyntax.SkipRef(out _);
            }

            TypeWithAnnotations declType = BindTypeOrVarKeyword(typeSyntax, diagnostics, out bool isVar, out AliasSymbol alias);


            var variables = nodeOpt.Variables;
            int count = variables.Count;

            if (isVar && count > 1)
            {
                // There are a number of ways in which a var decl can be illegal, but in these
                // cases we should report an error and then keep right on going with the inference.

                Error(diagnostics, ErrorCode.ERR_ImplicitlyTypedVariableMultipleDeclarator, nodeOpt);
            }

            var declarationArray = new BoundLocalDeclaration[count];

            for (int i = 0; i < count; i++)
            {
                var variableDeclarator = variables[i];
                bool includeBoundType = i == 0; //To avoid duplicated expressions, only the first declaration should contain the bound type.
                var declaration = BindVariableDeclaration(localKind, isVar, variableDeclarator, typeSyntax, declType, alias, diagnostics, includeBoundType);

                declarationArray[i] = declaration;
            }

            declarations = declarationArray.AsImmutableOrNull();

            return (count == 1) ?
                declarations[0] :
                new BoundMultipleLocalDeclarations(nodeOpt, declarations);
        }

        internal BoundStatement BindStatementExpressionList(SeparatedSyntaxList<ExpressionSyntax> statements, BindingDiagnosticBag diagnostics)
        {
            int count = statements.Count;
            if (count == 0)
            {
                return null;
            }
            else if (count == 1)
            {
                var syntax = statements[0];
                return BindExpressionStatement(syntax, syntax, false, diagnostics);
            }
            else
            {
                var statementBuilder = ArrayBuilder<BoundStatement>.GetInstance();
                for (int i = 0; i < count; i++)
                {
                    var syntax = statements[i];
                    var statement = BindExpressionStatement(syntax, syntax, false, diagnostics);
                    statementBuilder.Add(statement);
                }
                return BoundStatementList.Synthesized(statements.Node, statementBuilder.ToImmutableAndFree());
            }
        }

        private BoundStatement BindForEach(CommonForEachStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            Binder loopBinder = this.GetBinder(node);
            return this.GetBinder(node.Expression).WrapWithVariablesIfAny(node.Expression, loopBinder.BindForEachParts(diagnostics, loopBinder));
        }

        internal virtual BoundStatement BindForEachParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindForEachParts(diagnostics, originalBinder);
        }

        /// <summary>
        /// Like BindForEachParts, but only bind the deconstruction part of the foreach, for purpose of inferring the types of the declared locals.
        /// </summary>
        internal virtual BoundStatement BindForEachDeconstruction(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return this.Next.BindForEachDeconstruction(diagnostics, originalBinder);
        }

        private BoundStatement BindBreak(BreakStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var target = this.BreakLabel;
            if (target is null)
            {
                Error(diagnostics, ErrorCode.ERR_NoBreakOrCont, node);
                return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, hasErrors: true);
            }
            return new BoundBreakStatement(node, target);
        }

        private BoundStatement BindContinue(ContinueStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
            var target = this.ContinueLabel;
            if (target is null)
            {
                Error(diagnostics, ErrorCode.ERR_NoBreakOrCont, node);
                return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, hasErrors: true);
            }
            return new BoundContinueStatement(node, target);
        }

        private static SwitchBinder GetSwitchBinder(Binder binder)
        {
            SwitchBinder switchBinder = binder as SwitchBinder;
            while (binder != null && switchBinder == null)
            {
                binder = binder.Next;
                switchBinder = binder as SwitchBinder;
            }
            return switchBinder;
        }

        protected static bool IsInAsyncMethod(MethodSymbol method)
        {
            return method is not null && method.IsAsync;
        }

        protected bool IsInAsyncMethod()
        {
            return IsInAsyncMethod(this.ContainingMemberOrLambda as MethodSymbol);
        }

        protected bool IsTaskReturningAsyncMethod()
        {
            var symbol = this.ContainingMemberOrLambda;
            return symbol?.Kind == SymbolKind.Method && ((MethodSymbol)symbol).IsAsyncReturningTask(this.Compilation);
        }

        protected bool IsGenericTaskReturningAsyncMethod()
        {
            var symbol = this.ContainingMemberOrLambda;
            return symbol?.Kind == SymbolKind.Method && ((MethodSymbol)symbol).IsAsyncReturningGenericTask(this.Compilation);
        }

        protected bool IsIAsyncEnumerableOrIAsyncEnumeratorReturningAsyncMethod()
        {
            var symbol = this.ContainingMemberOrLambda;
            if (symbol?.Kind == SymbolKind.Method)
            {
                var method = (MethodSymbol)symbol;
                return method.IsAsyncReturningIAsyncEnumerable(this.Compilation) ||
                    method.IsAsyncReturningIAsyncEnumerator(this.Compilation);
            }
            return false;
        }

        protected virtual TypeSymbol GetCurrentReturnType(out RefKind refKind)
        {
            if (this.ContainingMemberOrLambda is MethodSymbol symbol)
            {
                refKind = symbol.RefKind;

                TypeSymbol returnType = symbol.ReturnType;

                if ((object)returnType == LambdaSymbol.ReturnTypeIsBeingInferred)
                {
                    return null;
                }

                return returnType;
            }

            refKind = RefKind.None;
            return null;
        }

        private BoundStatement BindReturn(ReturnStatementSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            var refKind = RefKind.None;
            var expressionSyntax = syntax.Expression?.CheckAndUnwrapRefExpression(diagnostics, out refKind);
            BoundExpression arg = null;
            if (expressionSyntax != null)
            {
                BindValueKind requiredValueKind = GetRequiredReturnValueKind(refKind);
                arg = BindValue(expressionSyntax, diagnostics, requiredValueKind);
            }
            else
            {
                // If this is a void return statement in a script, return default(T).
                var interactiveInitializerMethod = this.ContainingMemberOrLambda as SynthesizedInteractiveInitializerMethod;
                if (interactiveInitializerMethod != null)
                {
                    arg = new BoundDefaultExpression(interactiveInitializerMethod.GetNonNullSyntaxNode(), interactiveInitializerMethod.ResultType);
                }
            }

            TypeSymbol retType = GetCurrentReturnType(out RefKind sigRefKind);

            bool hasErrors = false;
            if (IsDirectlyInIterator)
            {
                diagnostics.Add(ErrorCode.ERR_ReturnInIterator, syntax.ReturnKeyword.GetLocation());
                hasErrors = true;
            }
            else if (IsInAsyncMethod())
            {
                if (refKind != RefKind.None)
                {
                    // This can happen if we are binding an async anonymous method to a delegate type.
                    diagnostics.Add(ErrorCode.ERR_MustNotHaveRefReturn, syntax.ReturnKeyword.GetLocation());
                    hasErrors = true;
                }
                else if (IsIAsyncEnumerableOrIAsyncEnumeratorReturningAsyncMethod())
                {
                    diagnostics.Add(ErrorCode.ERR_ReturnInIterator, syntax.ReturnKeyword.GetLocation());
                    hasErrors = true;
                }
            }
            else if (retType is not null && (refKind != RefKind.None) != (sigRefKind != RefKind.None))
            {
                var errorCode = refKind != RefKind.None
                    ? ErrorCode.ERR_MustNotHaveRefReturn
                    : ErrorCode.ERR_MustHaveRefReturn;
                diagnostics.Add(errorCode, syntax.ReturnKeyword.GetLocation());
                hasErrors = true;
            }

            if (arg != null)
            {
                hasErrors |= arg.HasErrors || (arg.Type is not null && arg.Type.IsErrorType());
            }

            if (hasErrors)
            {
                return new BoundReturnStatement(syntax, refKind, BindToTypeForErrorRecovery(arg), hasErrors: true);
            }

            // The return type could be null; we might be attempting to infer the return type either
            // because of method type inference, or because we are attempting to do error analysis
            // on a lambda expression of unknown return type.
            if (retType is not null)
            {
                if (retType.IsVoidType() || IsTaskReturningAsyncMethod())
                {
                    if (arg != null)
                    {
                        var container = this.ContainingMemberOrLambda;
                        if (container is LambdaSymbol)
                        {
                            // Error case: void-returning or async task-returning method or lambda with "return x;"
                            var errorCode = retType.IsVoidType()
                                ? ErrorCode.ERR_RetNoObjectRequiredLambda
                                : ErrorCode.ERR_TaskRetNoObjectRequiredLambda;

                            // Anonymous function converted to a void returning delegate cannot return a value
                            Error(diagnostics, errorCode, syntax.ReturnKeyword);
                            hasErrors = true;

                            // COMPATIBILITY: The native compiler also produced an error
                            // COMPATIBILITY: "Cannot convert lambda expression to delegate type 'Action' because some of the
                            // COMPATIBILITY: return types in the block are not implicitly convertible to the delegate return type"
                            // COMPATIBILITY: This error doesn't make sense in the "void" case because the whole idea of
                            // COMPATIBILITY: "conversion to void" is a bit unusual, and we've already given a good error.
                        }
                        else
                        {
                            // Error case: void-returning or async task-returning method or lambda with "return x;"
                            var errorCode = retType.IsVoidType()
                                ? ErrorCode.ERR_RetNoObjectRequired
                                : ErrorCode.ERR_TaskRetNoObjectRequired;

                            Error(diagnostics, errorCode, syntax.ReturnKeyword, container);
                            hasErrors = true;
                        }
                    }
                }
                else
                {
                    if (arg == null)
                    {
                        // Error case: non-void-returning or Task<T>-returning method or lambda but just have "return;"
                        var requiredType = IsGenericTaskReturningAsyncMethod()
                            ? retType.GetMemberTypeArgumentsNoUseSiteDiagnostics().Single()
                            : retType;

                        Error(diagnostics, ErrorCode.ERR_RetObjectRequired, syntax.ReturnKeyword, requiredType);
                        hasErrors = true;
                    }
                    else
                    {
                        arg = CreateReturnConversion(syntax, diagnostics, arg, sigRefKind, retType);
                        arg = ValidateEscape(arg, Binder.ExternalScope, refKind != RefKind.None, diagnostics);
                    }
                }
            }
            else
            {
                // Check that the returned expression is not void.
                if (arg?.Type is not null && arg.Type.IsVoidType())
                {
                    Error(diagnostics, ErrorCode.ERR_CantReturnVoid, expressionSyntax);
                    hasErrors = true;
                }
            }

            return new BoundReturnStatement(syntax, refKind, hasErrors ? BindToTypeForErrorRecovery(arg) : arg, hasErrors);
        }

        internal BoundExpression CreateReturnConversion(
            SyntaxNode syntax,
            BindingDiagnosticBag diagnostics,
            BoundExpression argument,
            RefKind returnRefKind,
            TypeSymbol returnType)
        {
            // If the return type is not void then the expression must be implicitly convertible.

            Conversion conversion;
            bool badAsyncReturnAlreadyReported = false;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            if (IsInAsyncMethod())
            {

                if (!IsGenericTaskReturningAsyncMethod())
                {
                    conversion = Conversion.NoConversion;
                    badAsyncReturnAlreadyReported = true;
                }
                else
                {
                    returnType = returnType.GetMemberTypeArgumentsNoUseSiteDiagnostics().Single();
                    conversion = this.Conversions.ClassifyConversionFromExpression(argument, returnType, ref useSiteInfo);
                }
            }
            else
            {
                conversion = this.Conversions.ClassifyConversionFromExpression(argument, returnType, ref useSiteInfo);
            }

            diagnostics.Add(syntax, useSiteInfo);

            if (!argument.HasAnyErrors)
            {
                if (returnRefKind != RefKind.None)
                {
                    if (conversion.Kind != ConversionKind.Identity)
                    {
                        Error(diagnostics, ErrorCode.ERR_RefReturnMustHaveIdentityConversion, argument.Syntax, returnType);
                        argument = argument.WithHasErrors();
                    }
                    else
                    {
                        return BindToNaturalType(argument, diagnostics);
                    }
                }
                else if (!conversion.IsImplicit || !conversion.IsValid)
                {
                    if (!badAsyncReturnAlreadyReported)
                    {
                        if (IsGenericTaskReturningAsyncMethod() && TypeSymbol.Equals(argument.Type, this.GetCurrentReturnType(out RefKind _), TypeCompareKind.ConsiderEverything2))
                        {
                            // Since this is an async method, the return expression must be of type '{0}' rather than 'Task<{0}>'
                            Error(diagnostics, ErrorCode.ERR_BadAsyncReturnExpression, argument.Syntax, returnType);
                        }
                        else
                        {
                            GenerateImplicitConversionError(diagnostics, argument.Syntax, conversion, argument, returnType);
                            if (this.ContainingMemberOrLambda is LambdaSymbol)
                            {
                                ReportCantConvertLambdaReturn(argument.Syntax, diagnostics);
                            }
                        }
                    }
                }
            }

            return CreateConversion(argument.Syntax, argument, conversion, isCast: false, conversionGroupOpt: null, returnType, diagnostics);
        }

        private BoundTryStatement BindTryStatement(TryStatementSyntax node, BindingDiagnosticBag diagnostics)
        {

            var tryBlock = BindEmbeddedBlock(node.Block, diagnostics);
            var catchBlocks = BindCatchBlocks(node.Catches, diagnostics);
            var finallyBlockOpt = (node.Finally != null) ? BindEmbeddedBlock(node.Finally.Block, diagnostics) : null;
            return new BoundTryStatement(node, tryBlock, catchBlocks, finallyBlockOpt);
        }

        private ImmutableArray<BoundCatchBlock> BindCatchBlocks(SyntaxList<CatchClauseSyntax> catchClauses, BindingDiagnosticBag diagnostics)
        {
            int n = catchClauses.Count;
            if (n == 0)
            {
                return ImmutableArray<BoundCatchBlock>.Empty;
            }

            var catchBlocks = ArrayBuilder<BoundCatchBlock>.GetInstance(n);
            var hasCatchAll = false;

            foreach (var catchSyntax in catchClauses)
            {
                if (hasCatchAll)
                {
                    diagnostics.Add(ErrorCode.ERR_TooManyCatches, catchSyntax.CatchKeyword.GetLocation());
                }

                var catchBinder = this.GetBinder(catchSyntax);
                var catchBlock = catchBinder.BindCatchBlock(catchSyntax, catchBlocks, diagnostics);
                catchBlocks.Add(catchBlock);

                hasCatchAll |= catchSyntax.Declaration == null && catchSyntax.Filter == null;
            }
            return catchBlocks.ToImmutableAndFree();
        }

        private BoundCatchBlock BindCatchBlock(CatchClauseSyntax node, ArrayBuilder<BoundCatchBlock> previousBlocks, BindingDiagnosticBag diagnostics)
        {
            bool hasError = false;
            TypeSymbol type = null;
            BoundExpression boundFilter = null;
            var declaration = node.Declaration;
            if (declaration != null)
            {
                // Note: The type is being bound twice: here and in LocalSymbol.Type. Currently,
                // LocalSymbol.Type ignores diagnostics so it seems cleaner to bind the type here
                // as well. However, if LocalSymbol.Type is changed to report diagnostics, we'll
                // need to avoid binding here since that will result in duplicate diagnostics.
                type = this.BindType(declaration.Type, diagnostics).Type;

                if (type.IsErrorType())
                {
                    hasError = true;
                }
                else
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                    TypeSymbol effectiveType = type.EffectiveType(ref useSiteInfo);
                    if (!Compilation.IsExceptionType(effectiveType, ref useSiteInfo))
                    {
                        // "The type caught or thrown must be derived from System.Exception"
                        Error(diagnostics, ErrorCode.ERR_BadExceptionType, declaration.Type);
                        hasError = true;
                        diagnostics.Add(declaration.Type, useSiteInfo);
                    }
                    else
                    {
                        diagnostics.AddDependencies(useSiteInfo);
                    }
                }
            }

            var filter = node.Filter;
            if (filter != null)
            {
                var filterBinder = this.GetBinder(filter);
                boundFilter = filterBinder.BindCatchFilter(filter, diagnostics);
                hasError |= boundFilter.HasAnyErrors;
            }

            if (!hasError)
            {
                // TODO: Loop is O(n), caller is O(n^2).  Perhaps we could iterate in reverse order (since it's easier to find
                // base types than to find derived types).
                foreach (var previousBlock in previousBlocks)
                {
                    var previousType = previousBlock.ExceptionTypeOpt;

                    // If the previous type is a generic parameter we don't know what exception types it's gonna catch exactly.
                    // If it is a class-type we know it's gonna catch all exception types of its type and types that are derived from it.
                    // So if the current type is a class-type (or an effective base type of a generic parameter)
                    // that derives from the previous type the current catch is unreachable.

                    if (previousBlock.ExceptionFilterOpt == null && previousType is not null && !previousType.IsErrorType())
                    {
                        if (type is not null)
                        {
                            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);

                            if (Conversions.HasIdentityOrImplicitReferenceConversion(type, previousType, ref useSiteInfo))
                            {
                                // "A previous catch clause already catches all exceptions of this or of a super type ('{0}')"
                                Error(diagnostics, ErrorCode.ERR_UnreachableCatch, declaration.Type, previousType);
                                diagnostics.Add(declaration.Type, useSiteInfo);
                                hasError = true;
                                break;
                            }

                            diagnostics.Add(declaration.Type, useSiteInfo);
                        }
                        else if (TypeSymbol.Equals(previousType, Compilation.GetWellKnownType(WellKnownType.System_Exception), TypeCompareKind.ConsiderEverything2) &&
                                 Compilation.SourceAssembly.RuntimeCompatibilityWrapNonExceptionThrows)
                        {
                            // If the RuntimeCompatibility(WrapNonExceptionThrows = false) is applied on the source assembly or any referenced netmodule.
                            // an empty catch may catch exceptions that don't derive from System.Exception.

                            // "A previous catch clause already catches all exceptions..."
                            Error(diagnostics, ErrorCode.WRN_UnreachableGeneralCatch, node.CatchKeyword);
                            break;
                        }
                    }
                }
            }

            var binder = GetBinder(node);

            ImmutableArray<LocalSymbol> locals = binder.GetDeclaredLocalsForScope(node);
            BoundExpression exceptionSource = null;
            LocalSymbol local = locals.FirstOrDefault();

            if (local?.DeclarationKind == LocalDeclarationKind.CatchVariable)
            {

                // Check for local variable conflicts in the *enclosing* binder, not the *current* binder;
                // obviously we will find a local of the given name in the current binder.
                hasError |= this.ValidateDeclarationNameConflictsInScope(local, diagnostics);

                exceptionSource = new BoundLocal(declaration, local, ConstantValue.NotAvailable, local.Type);
            }

            var block = BindEmbeddedBlock(node.Block, diagnostics);
            return new BoundCatchBlock(node, locals, exceptionSource, type, exceptionFilterPrologueOpt: null, boundFilter, block, hasError);
        }

        private BoundExpression BindCatchFilter(CatchFilterClauseSyntax filter, BindingDiagnosticBag diagnostics)
        {
            BoundExpression boundFilter = this.BindBooleanExpression(filter.FilterExpression, diagnostics);
            if (boundFilter.ConstantValue != ConstantValue.NotAvailable)
            {
                // Depending on whether the filter constant is true or false, and whether there are other catch clauses,
                // we suggest different actions
                var errorCode = boundFilter.ConstantValue.BooleanValue
                    ? ErrorCode.WRN_FilterIsConstantTrue
                    : (filter.Parent.Parent is TryStatementSyntax s && s.Catches.Count == 1 && s.Finally == null)
                        ? ErrorCode.WRN_FilterIsConstantFalseRedundantTryCatch
                        : ErrorCode.WRN_FilterIsConstantFalse;

                // Since the expression is a constant, the name can be retrieved from the first token
                Error(diagnostics, errorCode, filter.FilterExpression);
            }

            return boundFilter;
        }

        // Report an extra error on the return if we are in a lambda conversion.
        private void ReportCantConvertLambdaReturn(SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            // Suppress this error if the lambda is a result of a query rewrite.
            if (syntax.Parent is QueryClauseSyntax || syntax.Parent is SelectOrGroupClauseSyntax)
                return;

            if (this.ContainingMemberOrLambda is LambdaSymbol lambda)
            {
                Location location = GetLocationForDiagnostics(syntax);
                if (IsInAsyncMethod())
                {
                    // Cannot convert async {0} to intended delegate type. An async {0} may return void, Task or Task<T>, none of which are convertible to '{1}'.
                    Error(diagnostics, ErrorCode.ERR_CantConvAsyncAnonFuncReturns,
                        location,
                        lambda.MessageID.Localize(), lambda.ReturnType);
                }
                else
                {
                    // Cannot convert {0} to intended delegate type because some of the return types in the block are not implicitly convertible to the delegate return type
                    Error(diagnostics, ErrorCode.ERR_CantConvAnonMethReturns,
                        location,
                        lambda.MessageID.Localize());
                }
            }
        }

        private static Location GetLocationForDiagnostics(SyntaxNode node)
        {
            return node switch
            {
                LambdaExpressionSyntax lambdaSyntax => Location.Create(lambdaSyntax.SyntaxTree,
                                       Text.TextSpan.FromBounds(lambdaSyntax.SpanStart, lambdaSyntax.ArrowToken.Span.End)),
                AnonymousMethodExpressionSyntax anonymousMethodSyntax => Location.Create(anonymousMethodSyntax.SyntaxTree,
                                       Text.TextSpan.FromBounds(anonymousMethodSyntax.SpanStart,
                                       anonymousMethodSyntax.ParameterList?.Span.End ?? anonymousMethodSyntax.DelegateKeyword.Span.End)),
                _ => node.Location,
            };
        }

        private static bool IsValidStatementExpression(SyntaxNode syntax, BoundExpression expression)
        {
            bool syntacticallyValid = SyntaxFacts.IsStatementExpression(syntax);
            if (!syntacticallyValid)
            {
                return false;
            }

            if (expression.IsSuppressed)
            {
                return false;
            }

            // It is possible that an expression is syntactically valid but semantic analysis
            // reveals it to be illegal in a statement expression: "new MyDelegate(M)" for example
            // is not legal because it is a delegate-creation-expression and not an
            // object-creation-expression, but of course we don't know that syntactically.

            if (expression.Kind == BoundKind.DelegateCreationExpression || expression.Kind == BoundKind.NameOfOperator)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Wrap a given expression e into a block as either { e; } or { return e; }
        /// Shared between lambda and expression-bodied method binding.
        /// </summary>
        internal BoundBlock CreateBlockFromExpression(CSharpSyntaxNode node, ImmutableArray<LocalSymbol> locals, RefKind refKind, BoundExpression expression, ExpressionSyntax expressionSyntax, BindingDiagnosticBag diagnostics)
        {
            var returnType = GetCurrentReturnType(out RefKind returnRefKind);
            var syntax = expressionSyntax ?? expression.Syntax;

            BoundStatement statement;
            if (IsInAsyncMethod() && refKind != RefKind.None)
            {
                // This can happen if we are binding an async anonymous method to a delegate type.
                Error(diagnostics, ErrorCode.ERR_MustNotHaveRefReturn, syntax);
                expression = BindToTypeForErrorRecovery(expression);
                statement = new BoundReturnStatement(syntax, refKind, expression) { WasCompilerGenerated = true };
            }
            else if (returnType is not null)
            {
                if ((refKind != RefKind.None) != (returnRefKind != RefKind.None) && expression.Kind != BoundKind.ThrowExpression)
                {
                    var errorCode = refKind != RefKind.None
                        ? ErrorCode.ERR_MustNotHaveRefReturn
                        : ErrorCode.ERR_MustHaveRefReturn;
                    Error(diagnostics, errorCode, syntax);
                    expression = BindToTypeForErrorRecovery(expression);
                    statement = new BoundReturnStatement(syntax, RefKind.None, expression) { WasCompilerGenerated = true };
                }
                else if (returnType.IsVoidType() || IsTaskReturningAsyncMethod())
                {
                    // If the return type is void then the expression is required to be a legal
                    // statement expression.


                    bool errors = false;
                    if (expressionSyntax == null || !IsValidExpressionBody(expressionSyntax, expression))
                    {
                        expression = BindToTypeForErrorRecovery(expression);
                        Error(diagnostics, ErrorCode.ERR_IllegalStatement, syntax);
                        errors = true;
                    }
                    else
                    {
                        expression = BindToNaturalType(expression, diagnostics);
                    }

                    // Don't mark compiler generated so that the rewriter generates sequence points
                    var expressionStatement = new BoundExpressionStatement(syntax, expression, errors);

                    CheckForUnobservedAwaitable(expression, diagnostics);
                    statement = expressionStatement;
                }
                else if (IsIAsyncEnumerableOrIAsyncEnumeratorReturningAsyncMethod())
                {
                    Error(diagnostics, ErrorCode.ERR_ReturnInIterator, syntax);
                    expression = BindToTypeForErrorRecovery(expression);
                    statement = new BoundReturnStatement(syntax, returnRefKind, expression) { WasCompilerGenerated = true };
                }
                else
                {
                    expression = returnType.IsErrorType()
                        ? BindToTypeForErrorRecovery(expression)
                        : CreateReturnConversion(syntax, diagnostics, expression, refKind, returnType);
                    statement = new BoundReturnStatement(syntax, returnRefKind, expression) { WasCompilerGenerated = true };
                }
            }
            else if (expression.Type?.SpecialType == SpecialType.System_Void)
            {
                expression = BindToNaturalType(expression, diagnostics);
                statement = new BoundExpressionStatement(syntax, expression) { WasCompilerGenerated = true };
            }
            else
            {
                // When binding for purpose of inferring the return type of a lambda, we do not require returned expressions (such as `default` or switch expressions) to have a natural type
                var inferringLambda = this.ContainingMemberOrLambda is MethodSymbol method && (object)method.ReturnType == LambdaSymbol.ReturnTypeIsBeingInferred;
                if (!inferringLambda)
                {
                    expression = BindToNaturalType(expression, diagnostics);
                }
                statement = new BoundReturnStatement(syntax, refKind, expression) { WasCompilerGenerated = true };
            }

            // Need to attach the tree for when we generate sequence points.
            return new BoundBlock(node, locals, ImmutableArray.Create(statement)) { WasCompilerGenerated = node.Kind() != SyntaxKind.ArrowExpressionClause };
        }

        private static bool IsValidExpressionBody(SyntaxNode expressionSyntax, BoundExpression expression)
        {
            return IsValidStatementExpression(expressionSyntax, expression) || expressionSyntax.Kind() == SyntaxKind.ThrowExpression;
        }

        /// <summary>
        /// Binds an expression-bodied member with expression e as either { return e; } or { e; }.
        /// </summary>
        internal virtual BoundBlock BindExpressionBodyAsBlock(ArrowExpressionClauseSyntax expressionBody,
                                                      BindingDiagnosticBag diagnostics)
        {
            Binder bodyBinder = this.GetBinder(expressionBody);

            return bindExpressionBodyAsBlockInternal(expressionBody, bodyBinder, diagnostics);

            // Use static local function to prevent accidentally calling instance methods on `this` instead of `bodyBinder`
            static BoundBlock bindExpressionBodyAsBlockInternal(ArrowExpressionClauseSyntax expressionBody, Binder bodyBinder, BindingDiagnosticBag diagnostics)
            {
                ExpressionSyntax expressionSyntax = expressionBody.Expression.CheckAndUnwrapRefExpression(diagnostics, out RefKind refKind);
                BindValueKind requiredValueKind = bodyBinder.GetRequiredReturnValueKind(refKind);
                BoundExpression expression = bodyBinder.BindValue(expressionSyntax, diagnostics, requiredValueKind);
                expression = bodyBinder.ValidateEscape(expression, Binder.ExternalScope, refKind != RefKind.None, diagnostics);

                return bodyBinder.CreateBlockFromExpression(expressionBody, bodyBinder.GetDeclaredLocalsForScope(expressionBody), refKind, expression, expressionSyntax, diagnostics);
            }
        }

        /// <summary>
        /// Binds a lambda with expression e as either { return e; } or { e; }.
        /// </summary>
        public BoundBlock BindLambdaExpressionAsBlock(ExpressionSyntax body, BindingDiagnosticBag diagnostics)
        {
            Binder bodyBinder = this.GetBinder(body);

            var expressionSyntax = body.CheckAndUnwrapRefExpression(diagnostics, out RefKind refKind);
            BindValueKind requiredValueKind = GetRequiredReturnValueKind(refKind);
            BoundExpression expression = bodyBinder.BindValue(expressionSyntax, diagnostics, requiredValueKind);
            expression = ValidateEscape(expression, Binder.ExternalScope, refKind != RefKind.None, diagnostics);

            return bodyBinder.CreateBlockFromExpression(body, bodyBinder.GetDeclaredLocalsForScope(body), refKind, expression, expressionSyntax, diagnostics);
        }

        public BoundBlock CreateBlockFromExpression(ExpressionSyntax body, BoundExpression expression, BindingDiagnosticBag diagnostics)
        {
            Binder bodyBinder = this.GetBinder(body);

            return bodyBinder.CreateBlockFromExpression(body, bodyBinder.GetDeclaredLocalsForScope(body), RefKind.None, expression, body, diagnostics);
        }

        private BindValueKind GetRequiredReturnValueKind(RefKind refKind)
        {
            BindValueKind requiredValueKind = BindValueKind.RValue;
            if (refKind != RefKind.None)
            {
                GetCurrentReturnType(out var sigRefKind);
                requiredValueKind = sigRefKind == RefKind.Ref ?
                                        BindValueKind.RefReturn :
                                        BindValueKind.ReadonlyRef;
            }

            return requiredValueKind;
        }

        public virtual BoundNode BindMethodBody(CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            switch (syntax)
            {
                case RecordDeclarationSyntax recordDecl:
                    return BindRecordConstructorBody(recordDecl, diagnostics);

                case BaseMethodDeclarationSyntax method:
                    if (method.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        return BindConstructorBody((ConstructorDeclarationSyntax)method, diagnostics);
                    }

                    return BindMethodBody(method, method.Body, method.ExpressionBody, diagnostics);

                case AccessorDeclarationSyntax accessor:
                    return BindMethodBody(accessor, accessor.Body, accessor.ExpressionBody, diagnostics);

                case ArrowExpressionClauseSyntax arrowExpression:
                    return BindExpressionBodyAsBlock(arrowExpression, diagnostics);

                case CompilationUnitSyntax compilationUnit:
                    return BindSimpleProgram(compilationUnit, diagnostics);

                default:
                    throw ExceptionUtilities.UnexpectedValue(syntax.Kind());
            }
        }

        private BoundNode BindSimpleProgram(CompilationUnitSyntax compilationUnit, BindingDiagnosticBag diagnostics)
        {
            _ = (SynthesizedSimpleProgramEntryPointSymbol)ContainingMemberOrLambda;

            return GetBinder(compilationUnit).BindSimpleProgramCompilationUnit(compilationUnit, /*simpleProgram, */diagnostics);
        }

        private BoundNode BindSimpleProgramCompilationUnit(CompilationUnitSyntax compilationUnit, /*SynthesizedSimpleProgramEntryPointSymbol simpleProgram, */BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<BoundStatement> boundStatements = ArrayBuilder<BoundStatement>.GetInstance();
            foreach (var statement in compilationUnit.Members)
            {
                if (statement is GlobalStatementSyntax topLevelStatement)
                {
                    var boundStatement = BindStatement(topLevelStatement.Statement, diagnostics);
                    boundStatements.Add(boundStatement);
                }
            }

            return new BoundNonConstructorMethodBody(compilationUnit,
                                                     FinishBindBlockParts(compilationUnit, boundStatements.ToImmutableAndFree()/*, diagnostics*/).MakeCompilerGenerated(),
                                                     expressionBody: null);
        }

        private BoundNode BindRecordConstructorBody(RecordDeclarationSyntax recordDecl, BindingDiagnosticBag diagnostics)
        {

            Binder bodyBinder = this.GetBinder(recordDecl);

            BoundExpressionStatement initializer = null;
            if (recordDecl.PrimaryConstructorBaseTypeIfClass is PrimaryConstructorBaseTypeSyntax baseWithArguments)
            {
                initializer = bodyBinder.BindConstructorInitializer(baseWithArguments, diagnostics);
            }

            return new BoundConstructorMethodBody(recordDecl,
                                                  bodyBinder.GetDeclaredLocalsForScope(recordDecl),
                                                  initializer,
                                                  blockBody: new BoundBlock(recordDecl, ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty).MakeCompilerGenerated(),
                                                  expressionBody: null);
        }

        internal virtual BoundExpressionStatement BindConstructorInitializer(PrimaryConstructorBaseTypeSyntax initializer, BindingDiagnosticBag diagnostics)
        {
            BoundExpression initializerInvocation = GetBinder(initializer).BindConstructorInitializer(initializer.ArgumentList, (MethodSymbol)this.ContainingMember(), diagnostics);
            var constructorInitializer = new BoundExpressionStatement(initializer, initializerInvocation);
            return constructorInitializer;
        }

        private BoundNode BindConstructorBody(ConstructorDeclarationSyntax constructor, BindingDiagnosticBag diagnostics)
        {
            if (constructor.Initializer == null && constructor.Body == null && constructor.ExpressionBody == null)
            {
                return null;
            }

            Binder bodyBinder = this.GetBinder(constructor);

            if (constructor.Initializer?.IsKind(SyntaxKind.ThisConstructorInitializer) != true &&
                ContainingType.GetMembersUnordered().OfType<SynthesizedRecordConstructor>().Any())
            {
                var constructorSymbol = (MethodSymbol)this.ContainingMember();
                if (!constructorSymbol.IsStatic &&
                    !SynthesizedRecordCopyCtor.IsCopyConstructor(constructorSymbol))
                {
                    // Note: we check the constructor initializer of copy constructors elsewhere
                    Error(diagnostics, ErrorCode.ERR_UnexpectedOrMissingConstructorInitializerInRecord, constructor.Initializer?.ThisOrBaseKeyword ?? constructor.Identifier);
                }
            }

            // Using BindStatement to bind block to make sure we are reusing results of partial binding in SemanticModel
            return new BoundConstructorMethodBody(constructor,
                                                  bodyBinder.GetDeclaredLocalsForScope(constructor),
                                                  constructor.Initializer == null ? null : bodyBinder.BindConstructorInitializer(constructor.Initializer, diagnostics),
                                                  constructor.Body == null ? null : (BoundBlock)bodyBinder.BindStatement(constructor.Body, diagnostics),
                                                  constructor.ExpressionBody == null ?
                                                      null :
                                                      bodyBinder.BindExpressionBodyAsBlock(constructor.ExpressionBody,
                                                                                           constructor.Body == null ? diagnostics : BindingDiagnosticBag.Discarded));
        }

        internal virtual BoundExpressionStatement BindConstructorInitializer(ConstructorInitializerSyntax initializer, BindingDiagnosticBag diagnostics)
        {
            BoundExpression initializerInvocation = GetBinder(initializer).BindConstructorInitializer(initializer.ArgumentList, (MethodSymbol)this.ContainingMember(), diagnostics);
            //  Base WasCompilerGenerated state off of whether constructor is implicitly declared, this will ensure proper instrumentation.
            var constructorInitializer = new BoundExpressionStatement(initializer, initializerInvocation);
            return constructorInitializer;
        }

        private BoundNode BindMethodBody(CSharpSyntaxNode declaration, BlockSyntax blockBody, ArrowExpressionClauseSyntax expressionBody, BindingDiagnosticBag diagnostics)
        {
            if (blockBody == null && expressionBody == null)
            {
                return null;
            }

            // Using BindStatement to bind block to make sure we are reusing results of partial binding in SemanticModel
            return new BoundNonConstructorMethodBody(declaration,
                                                     blockBody == null ? null : (BoundBlock)BindStatement(blockBody, diagnostics),
                                                     expressionBody == null ?
                                                         null :
                                                         BindExpressionBodyAsBlock(expressionBody,
                                                                                   blockBody == null ? diagnostics : BindingDiagnosticBag.Discarded));
        }

        internal virtual ImmutableArray<LocalSymbol> Locals
        {
            get
            {
                return ImmutableArray<LocalSymbol>.Empty;
            }
        }

        internal virtual ImmutableArray<LocalFunctionSymbol> LocalFunctions
        {
            get
            {
                return ImmutableArray<LocalFunctionSymbol>.Empty;
            }
        }

        internal virtual ImmutableArray<LabelSymbol> Labels
        {
            get
            {
                return ImmutableArray<LabelSymbol>.Empty;
            }
        }

        /// <summary>
        /// If this binder owns the scope that can declare extern aliases, a set of declared aliases should be returned (even if empty).
        /// Otherwise, a default instance should be returned. 
        /// </summary>
        internal virtual ImmutableArray<AliasAndExternAliasDirective> ExternAliases
        {
            get
            {
                return default;
            }
        }

        /// <summary>
        /// If this binder owns the scope that can declare using aliases, a set of declared aliases should be returned (even if empty).
        /// Otherwise, a default instance should be returned. 
        /// Note, only aliases syntactically declared within the enclosing declaration are included. For example, global aliases
        /// declared in a different compilation units are not included.
        /// </summary>
        internal virtual ImmutableArray<AliasAndUsingDirective> UsingAliases
        {
            get
            {
                return default;
            }
        }

        /// <summary>
        /// Perform a lookup for the specified method on the specified expression by attempting to invoke it
        /// </summary>
        /// <param name="receiver">The expression to perform pattern lookup on</param>
        /// <param name="methodName">Method to search for.</param>
        /// <param name="syntaxNode">The expression for which lookup is being performed</param>
        /// <param name="diagnostics">Populated with binding diagnostics.</param>
        /// <param name="result">The method symbol that was looked up, or null</param>
        /// <returns>A <see cref="PatternLookupResult"/> value with the outcome of the lookup</returns>
        internal PatternLookupResult PerformPatternMethodLookup(BoundExpression receiver, string methodName,
                                                                SyntaxNode syntaxNode, BindingDiagnosticBag diagnostics, out MethodSymbol result)
        {
            var bindingDiagnostics = BindingDiagnosticBag.GetInstance(diagnostics);

            try
            {
                result = null;

                var boundAccess = BindInstanceMemberAccess(
                       syntaxNode,
                       syntaxNode,
                       receiver,
                       methodName,
                       rightArity: 0,
                       typeArgumentsSyntax: default,
                       typeArgumentsWithAnnotations: default,
                       invoked: true,
                       indexed: false,
                       bindingDiagnostics);

                if (boundAccess.Kind != BoundKind.MethodGroup)
                {
                    // the thing is not a method
                    return PatternLookupResult.NotAMethod;
                }

                // NOTE: Because we're calling this method with no arguments and we
                //       explicitly ignore default values for params parameters
                //       (see ParameterSymbol.IsOptional) we know that no ParameterArray
                //       containing method can be invoked in normal form which allows
                //       us to skip some work during the lookup.

                var analyzedArguments = AnalyzedArguments.GetInstance();
                var patternMethodCall = BindMethodGroupInvocation(
                    syntaxNode,
                    syntaxNode,
                    methodName,
                    (BoundMethodGroup)boundAccess,
                    analyzedArguments,
                    bindingDiagnostics,
                    queryClause: null,
                    allowUnexpandedForm: false,
                    anyApplicableCandidates: out _);

                analyzedArguments.Free();

                if (patternMethodCall.Kind != BoundKind.Call)
                {
                    return PatternLookupResult.NotCallable;
                }

                var call = (BoundCall)patternMethodCall;
                if (call.ResultKind == LookupResultKind.Empty)
                {
                    return PatternLookupResult.NoResults;
                }

                // we have succeeded or almost succeeded to bind the method
                // report additional binding diagnostics that we have seen so far
                diagnostics.AddRange(bindingDiagnostics);

                var patternMethodSymbol = call.Method;
                if (patternMethodSymbol is ErrorMethodSymbol ||
                    patternMethodCall.HasAnyErrors)
                {
                    return PatternLookupResult.ResultHasErrors;
                }

                // Success!
                result = patternMethodSymbol;
                return PatternLookupResult.Success;
            }
            finally
            {
                bindingDiagnostics.Free();
            }
        }
    }
}
