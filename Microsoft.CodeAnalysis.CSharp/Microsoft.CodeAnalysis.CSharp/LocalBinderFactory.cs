using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LocalBinderFactory : CSharpSyntaxWalker
    {
        private readonly SmallDictionary<SyntaxNode, Binder> _map;

        private Symbol _containingMemberOrLambda;

        private Binder _enclosing;

        private readonly SyntaxNode _root;

        private void Visit(CSharpSyntaxNode syntax, Binder enclosing)
        {
            if (_enclosing == enclosing)
            {
                Visit(syntax);
                return;
            }
            Binder enclosing2 = _enclosing;
            _enclosing = enclosing;
            Visit(syntax);
            _enclosing = enclosing2;
        }

        private void VisitRankSpecifiers(TypeSyntax type, Binder enclosing)
        {
            Action<ArrayRankSpecifierSyntax, (LocalBinderFactory, Binder)> action = delegate (ArrayRankSpecifierSyntax rankSpecifier, (LocalBinderFactory localBinderFactory, Binder binder) args)
            {
                SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator = rankSpecifier.Sizes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ExpressionSyntax current = enumerator.Current;
                    if (current.Kind() != SyntaxKind.OmittedArraySizeExpression)
                    {
                        args.localBinderFactory.Visit(current, args.binder);
                    }
                }
            };
            (LocalBinderFactory, Binder) argument = (this, enclosing);
            type.VisitRankSpecifiers(action, in argument);
        }

        public static SmallDictionary<SyntaxNode, Binder> BuildMap(Symbol containingMemberOrLambda, SyntaxNode syntax, Binder enclosing, Action<Binder, SyntaxNode> binderUpdatedHandler = null)
        {
            LocalBinderFactory localBinderFactory = new LocalBinderFactory(containingMemberOrLambda, syntax, enclosing);
            if (syntax is ExpressionSyntax syntax2)
            {
                enclosing = new ExpressionVariableBinder(syntax, enclosing);
                binderUpdatedHandler?.Invoke(enclosing, syntax);
                localBinderFactory.AddToMap(syntax, enclosing);
                localBinderFactory.Visit(syntax2, enclosing);
            }
            else if (syntax.Kind() != SyntaxKind.Block && syntax is StatementSyntax statementSyntax)
            {
                enclosing = localBinderFactory.GetBinderForPossibleEmbeddedStatement(statementSyntax, enclosing, out var embeddedScopeDesignator);
                binderUpdatedHandler?.Invoke(enclosing, embeddedScopeDesignator);
                if (embeddedScopeDesignator != null)
                {
                    localBinderFactory.AddToMap(embeddedScopeDesignator, enclosing);
                }
                localBinderFactory.Visit(statementSyntax, enclosing);
            }
            else
            {
                binderUpdatedHandler?.Invoke(enclosing, null);
                localBinderFactory.Visit((CSharpSyntaxNode)syntax, enclosing);
            }
            return localBinderFactory._map;
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = node.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberDeclarationSyntax current = enumerator.Current;
                if (current.Kind() == SyntaxKind.GlobalStatement)
                {
                    Visit(current);
                }
            }
        }

        private LocalBinderFactory(Symbol containingMemberOrLambda, SyntaxNode root, Binder enclosing)
        {
            _map = new SmallDictionary<SyntaxNode, Binder>(ReferenceEqualityComparer.Instance);
            _containingMemberOrLambda = containingMemberOrLambda;
            _enclosing = enclosing;
            _root = root;
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Visit(node.Body);
            Visit(node.ExpressionBody);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            Binder binder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, binder);
            Visit(node.Initializer, binder);
            Visit(node.Body, binder);
            Visit(node.ExpressionBody, binder);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            Binder binder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, binder);
            Visit(node.PrimaryConstructorBaseTypeIfClass, binder);
        }

        public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            Binder binder = _enclosing.WithAdditionalFlags(BinderFlags.ConstructorInitializer);
            AddToMap(node, binder);
            VisitConstructorInitializerArgumentList(node, node.ArgumentList, binder);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            Visit(node.Body);
            Visit(node.ExpressionBody);
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            Visit(node.Body);
            Visit(node.ExpressionBody);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            Visit(node.Body);
            Visit(node.ExpressionBody);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            Visit(node.Body);
            Visit(node.ExpressionBody);
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            VisitLambdaExpression(node);
        }

        private void VisitLambdaExpression(LambdaExpressionSyntax node)
        {
            if (_root == node)
            {
                CSharpSyntaxNode body = node.Body;
                if (body.Kind() == SyntaxKind.Block)
                {
                    VisitBlock((BlockSyntax)body);
                    return;
                }
                ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(body, _enclosing);
                AddToMap(body, expressionVariableBinder);
                Visit(body, expressionVariableBinder);
            }
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            VisitLambdaExpression(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            Symbol containingMemberOrLambda = _containingMemberOrLambda;
            Binder enclosing = _enclosing;
            LocalFunctionSymbol localFunctionSymbol = FindLocalFunction(node, _enclosing);
            if ((object)localFunctionSymbol != null)
            {
                _containingMemberOrLambda = localFunctionSymbol;
                enclosing = (localFunctionSymbol.IsGenericMethod ? new WithMethodTypeParametersBinder(localFunctionSymbol, _enclosing) : _enclosing);
                enclosing = enclosing.WithUnsafeRegionIfNecessary(node.Modifiers);
                enclosing = new InMethodBinder(localFunctionSymbol, enclosing);
            }
            BlockSyntax body = node.Body;
            if (body != null)
            {
                Visit(body, enclosing);
            }
            ArrowExpressionClauseSyntax expressionBody = node.ExpressionBody;
            if (expressionBody != null)
            {
                Visit(expressionBody, enclosing);
            }
            _containingMemberOrLambda = containingMemberOrLambda;
        }

        private static LocalFunctionSymbol FindLocalFunction(LocalFunctionStatementSyntax node, Binder enclosing)
        {
            LocalFunctionSymbol result = null;
            Binder binder = enclosing;
            while (binder != null && !binder.IsLocalFunctionsScopeBinder)
            {
                binder = binder.Next;
            }
            if (binder != null)
            {
                ImmutableArray<LocalFunctionSymbol>.Enumerator enumerator = binder.LocalFunctions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalFunctionSymbol current = enumerator.Current;
                    if (current.Locations[0] == node.Identifier.GetLocation())
                    {
                        result = current;
                    }
                }
            }
            return result;
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, expressionVariableBinder);
            Visit(node.Expression, expressionVariableBinder);
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, expressionVariableBinder);
            Visit(node.Value, expressionVariableBinder);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, expressionVariableBinder);
            AttributeArgumentListSyntax? argumentList = node.ArgumentList;
            if (argumentList != null && argumentList!.Arguments.Count > 0)
            {
                SeparatedSyntaxList<AttributeArgumentSyntax>.Enumerator enumerator = node.ArgumentList!.Arguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttributeArgumentSyntax current = enumerator.Current;
                    Visit(current.Expression, expressionVariableBinder);
                }
            }
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            Binder binder = _enclosing.WithAdditionalFlags(BinderFlags.ConstructorInitializer);
            AddToMap(node, binder);
            VisitConstructorInitializerArgumentList(node, node.ArgumentList, binder);
        }

        private void VisitConstructorInitializerArgumentList(CSharpSyntaxNode node, ArgumentListSyntax argumentList, Binder binder)
        {
            if (argumentList != null)
            {
                if (_root == node)
                {
                    binder = new ExpressionVariableBinder(argumentList, binder);
                    AddToMap(argumentList, binder);
                }
                Visit(argumentList, binder);
            }
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            if (_root == node)
            {
                VisitBlock(node.Block);
            }
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            Visit(node.Statement);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            BlockBinder blockBinder = new BlockBinder(_enclosing, node);
            AddToMap(node, blockBinder);
            SyntaxList<StatementSyntax>.Enumerator enumerator = node.Statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StatementSyntax current = enumerator.Current;
                Visit(current, blockBinder);
            }
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            UsingStatementBinder usingStatementBinder = new UsingStatementBinder(_enclosing, node);
            AddToMap(node, usingStatementBinder);
            ExpressionSyntax expression = node.Expression;
            VariableDeclarationSyntax declaration = node.Declaration;
            if (expression != null)
            {
                Visit(expression, usingStatementBinder);
            }
            else
            {
                VisitRankSpecifiers(declaration.Type, usingStatementBinder);
                SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = declaration.Variables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VariableDeclaratorSyntax current = enumerator.Current;
                    Visit(current, usingStatementBinder);
                }
            }
            VisitPossibleEmbeddedStatement(node.Statement, usingStatementBinder);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            WhileBinder whileBinder = new WhileBinder(_enclosing, node);
            AddToMap(node, whileBinder);
            Visit(node.Condition, whileBinder);
            VisitPossibleEmbeddedStatement(node.Statement, whileBinder);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            WhileBinder whileBinder = new WhileBinder(_enclosing, node);
            AddToMap(node, whileBinder);
            Visit(node.Condition, whileBinder);
            VisitPossibleEmbeddedStatement(node.Statement, whileBinder);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            Binder binder = new ForLoopBinder(_enclosing, node);
            AddToMap(node, binder);
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration != null)
            {
                VisitRankSpecifiers(declaration.Type, binder);
                SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = declaration.Variables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VariableDeclaratorSyntax current = enumerator.Current;
                    Visit(current, binder);
                }
            }
            else
            {
                SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator2 = node.Initializers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ExpressionSyntax current2 = enumerator2.Current;
                    Visit(current2, binder);
                }
            }
            ExpressionSyntax condition = node.Condition;
            if (condition != null)
            {
                binder = new ExpressionVariableBinder(condition, binder);
                AddToMap(condition, binder);
                Visit(condition, binder);
            }
            SeparatedSyntaxList<ExpressionSyntax> incrementors = node.Incrementors;
            if (incrementors.Count > 0)
            {
                ExpressionListVariableBinder expressionListVariableBinder = new ExpressionListVariableBinder(incrementors, binder);
                AddToMap(incrementors.First(), expressionListVariableBinder);
                SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator2 = incrementors.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ExpressionSyntax current3 = enumerator2.Current;
                    Visit(current3, expressionListVariableBinder);
                }
            }
            VisitPossibleEmbeddedStatement(node.Statement, binder);
        }

        private void VisitCommonForEachStatement(CommonForEachStatementSyntax node)
        {
            ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(node.Expression, _enclosing);
            AddToMap(node.Expression, expressionVariableBinder);
            Visit(node.Expression, expressionVariableBinder);
            ForEachLoopBinder forEachLoopBinder = new ForEachLoopBinder(expressionVariableBinder, node);
            AddToMap(node, forEachLoopBinder);
            VisitPossibleEmbeddedStatement(node.Statement, forEachLoopBinder);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            VisitCommonForEachStatement(node);
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            VisitCommonForEachStatement(node);
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            Binder binder = _enclosing.WithCheckedOrUncheckedRegion(node.Kind() == SyntaxKind.CheckedStatement);
            AddToMap(node, binder);
            Visit(node.Block, binder);
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            Binder binder = _enclosing.WithAdditionalFlags(BinderFlags.UnsafeRegion);
            AddToMap(node, binder);
            Visit(node.Block, binder);
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            FixedStatementBinder fixedStatementBinder = new FixedStatementBinder(_enclosing, node);
            AddToMap(node, fixedStatementBinder);
            if (node.Declaration != null)
            {
                VisitRankSpecifiers(node.Declaration.Type, fixedStatementBinder);
                SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = node.Declaration.Variables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VariableDeclaratorSyntax current = enumerator.Current;
                    Visit(current, fixedStatementBinder);
                }
            }
            VisitPossibleEmbeddedStatement(node.Statement, fixedStatementBinder);
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            LockBinder lockBinder = new LockBinder(_enclosing, node);
            AddToMap(node, lockBinder);
            Visit(node.Expression, lockBinder);
            StatementSyntax statement = node.Statement;
            Binder binder = lockBinder.WithAdditionalFlags(BinderFlags.InLockBody);
            if (binder != lockBinder)
            {
                AddToMap(statement, binder);
            }
            VisitPossibleEmbeddedStatement(statement, binder);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            AddToMap(node.Expression, _enclosing);
            Visit(node.Expression, _enclosing);
            SwitchBinder switchBinder = SwitchBinder.Create(_enclosing, node);
            AddToMap(node, switchBinder);
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = node.Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchSectionSyntax current = enumerator.Current;
                Visit(current, switchBinder);
            }
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            ExpressionVariableBinder expressionVariableBinder = new ExpressionVariableBinder(node, _enclosing);
            AddToMap(node, expressionVariableBinder);
            SyntaxList<SwitchLabelSyntax>.Enumerator enumerator = node.Labels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchLabelSyntax current = enumerator.Current;
                switch (current.Kind())
                {
                    case SyntaxKind.CasePatternSwitchLabel:
                        {
                            CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = (CasePatternSwitchLabelSyntax)current;
                            Visit(casePatternSwitchLabelSyntax.Pattern, expressionVariableBinder);
                            if (casePatternSwitchLabelSyntax.WhenClause != null)
                            {
                                Visit(casePatternSwitchLabelSyntax.WhenClause!.Condition, expressionVariableBinder);
                            }
                            break;
                        }
                    case SyntaxKind.CaseSwitchLabel:
                        {
                            CaseSwitchLabelSyntax caseSwitchLabelSyntax = (CaseSwitchLabelSyntax)current;
                            Visit(caseSwitchLabelSyntax.Value, expressionVariableBinder);
                            break;
                        }
                }
            }
            SyntaxList<StatementSyntax>.Enumerator enumerator2 = node.Statements.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                StatementSyntax current2 = enumerator2.Current;
                Visit(current2, expressionVariableBinder);
            }
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            SwitchExpressionBinder switchExpressionBinder = new SwitchExpressionBinder(node, _enclosing);
            AddToMap(node, switchExpressionBinder);
            Visit(node.GoverningExpression, switchExpressionBinder);
            SeparatedSyntaxList<SwitchExpressionArmSyntax>.Enumerator enumerator = node.Arms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchExpressionArmSyntax current = enumerator.Current;
                ExpressionVariableBinder armScopeBinder = new ExpressionVariableBinder(current, switchExpressionBinder);
                SwitchExpressionArmBinder switchExpressionArmBinder = new SwitchExpressionArmBinder(current, armScopeBinder, switchExpressionBinder);
                AddToMap(current, switchExpressionArmBinder);
                Visit(current.Pattern, switchExpressionArmBinder);
                if (current.WhenClause != null)
                {
                    Visit(current.WhenClause, switchExpressionArmBinder);
                }
                Visit(current.Expression, switchExpressionArmBinder);
            }
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            Visit(node.Condition, _enclosing);
            VisitPossibleEmbeddedStatement(node.Statement, _enclosing);
            Visit(node.Else, _enclosing);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            VisitPossibleEmbeddedStatement(node.Statement, _enclosing);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            Visit(node.Statement, _enclosing);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            if (node.Catches.Any())
            {
                Visit(node.Block, _enclosing.WithAdditionalFlags(BinderFlags.InTryBlockOfTryCatch));
            }
            else
            {
                Visit(node.Block, _enclosing);
            }
            SyntaxList<CatchClauseSyntax>.Enumerator enumerator = node.Catches.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CatchClauseSyntax current = enumerator.Current;
                Visit(current, _enclosing);
            }
            if (node.Finally != null)
            {
                Visit(node.Finally, _enclosing);
            }
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            CatchClauseBinder catchClauseBinder = new CatchClauseBinder(_enclosing, node);
            AddToMap(node, catchClauseBinder);
            if (node.Filter != null)
            {
                Binder binder = catchClauseBinder.WithAdditionalFlags(BinderFlags.InCatchFilter);
                AddToMap(node.Filter, binder);
                Visit(node.Filter, binder);
            }
            Visit(node.Block, catchClauseBinder);
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            Visit(node.FilterExpression);
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            BinderFlags binderFlags = BinderFlags.InFinallyBlock;
            if (_enclosing.Flags.Includes(BinderFlags.InCatchBlock))
            {
                binderFlags |= BinderFlags.InNestedFinallyBlock;
            }
            Visit(node.Block, _enclosing.WithAdditionalFlags(binderFlags));
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            if (node.Expression != null)
            {
                Visit(node.Expression, _enclosing);
            }
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            Visit(node.Expression, _enclosing);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            VisitRankSpecifiers(node.Declaration.Type, _enclosing);
            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = node.Declaration.Variables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VariableDeclaratorSyntax current = enumerator.Current;
                Visit(current);
            }
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            Visit(node.ArgumentList);
            Visit(node.Initializer?.Value);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (node.Expression != null)
            {
                Visit(node.Expression, _enclosing);
            }
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            if (node.Expression != null)
            {
                Visit(node.Expression, _enclosing);
            }
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            while (true)
            {
                Visit(node.Right);
                if (!(node.Left is BinaryExpressionSyntax binaryExpressionSyntax))
                {
                    break;
                }
                node = binaryExpressionSyntax;
            }
            Visit(node.Left);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        private void AddToMap(SyntaxNode node, Binder binder)
        {
            _map[node] = binder;
        }

        private Binder GetBinderForPossibleEmbeddedStatement(StatementSyntax statement, Binder enclosing, out CSharpSyntaxNode embeddedScopeDesignator)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ThrowStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.LocalFunctionStatement:
                    embeddedScopeDesignator = statement;
                    return new EmbeddedStatementBinder(enclosing, statement);
                case SyntaxKind.SwitchStatement:
                    {
                        SwitchStatementSyntax switchStatementSyntax = (SwitchStatementSyntax)statement;
                        embeddedScopeDesignator = switchStatementSyntax.Expression;
                        return new ExpressionVariableBinder(switchStatementSyntax.Expression, enclosing);
                    }
                default:
                    embeddedScopeDesignator = null;
                    return enclosing;
            }
        }

        private void VisitPossibleEmbeddedStatement(StatementSyntax statement, Binder enclosing)
        {
            if (statement != null)
            {
                enclosing = GetBinderForPossibleEmbeddedStatement(statement, enclosing, out var embeddedScopeDesignator);
                if (embeddedScopeDesignator != null)
                {
                    AddToMap(embeddedScopeDesignator, enclosing);
                }
                Visit(statement, enclosing);
            }
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            Visit(node.FromClause.Expression);
            Visit(node.Body);
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            SyntaxList<QueryClauseSyntax>.Enumerator enumerator = node.Clauses.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QueryClauseSyntax current = enumerator.Current;
                if (current.Kind() == SyntaxKind.JoinClause)
                {
                    Visit(((JoinClauseSyntax)current).InExpression);
                }
            }
            Visit(node.Continuation);
        }
    }
}
