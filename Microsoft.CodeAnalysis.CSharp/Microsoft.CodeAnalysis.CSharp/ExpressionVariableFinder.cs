using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class ExpressionVariableFinder<TFieldOrLocalSymbol> : CSharpSyntaxWalker where TFieldOrLocalSymbol : Symbol
    {
        private ArrayBuilder<TFieldOrLocalSymbol> _variablesBuilder;

        private SyntaxNode _nodeToBind;

        protected void FindExpressionVariables(ArrayBuilder<TFieldOrLocalSymbol> builder, CSharpSyntaxNode node)
        {
            ArrayBuilder<TFieldOrLocalSymbol> variablesBuilder = _variablesBuilder;
            _variablesBuilder = builder;
            VisitNodeToBind(node);
            _variablesBuilder = variablesBuilder;
        }

        public override void Visit(SyntaxNode node)
        {
            if (node != null)
            {
                ((CSharpSyntaxNode)node).Accept(this);
            }
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            Visit(node.GoverningExpression);
        }

        public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            SyntaxNode nodeToBind = _nodeToBind;
            _nodeToBind = node;
            Visit(node.Pattern);
            Visit(node.WhenClause?.Condition);
            Visit(node.Expression);
            _nodeToBind = nodeToBind;
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.ArgumentList != null)
            {
                SeparatedSyntaxList<ArgumentSyntax>.Enumerator enumerator = node.ArgumentList!.Arguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ArgumentSyntax current = enumerator.Current;
                    Visit(current.Expression);
                }
            }
            VisitNodeToBind(node.Initializer);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            if (node.Kind() == SyntaxKind.GotoCaseStatement)
            {
                Visit(node.Expression);
            }
        }

        private void VisitNodeToBind(CSharpSyntaxNode node)
        {
            SyntaxNode nodeToBind = _nodeToBind;
            _nodeToBind = node;
            Visit(node);
            _nodeToBind = nodeToBind;
        }

        protected void FindExpressionVariables(ArrayBuilder<TFieldOrLocalSymbol> builder, SeparatedSyntaxList<ExpressionSyntax> nodes)
        {
            ArrayBuilder<TFieldOrLocalSymbol> variablesBuilder = _variablesBuilder;
            _variablesBuilder = builder;
            SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator = nodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ExpressionSyntax current = enumerator.Current;
                VisitNodeToBind(current);
            }
            _variablesBuilder = variablesBuilder;
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            VisitNodeToBind(node.Value);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            SyntaxList<SwitchLabelSyntax>.Enumerator enumerator = node.Labels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchLabelSyntax current = enumerator.Current;
                switch (current.Kind())
                {
                    case SyntaxKind.CasePatternSwitchLabel:
                        {
                            CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = (CasePatternSwitchLabelSyntax)current;
                            SyntaxNode nodeToBind = _nodeToBind;
                            _nodeToBind = casePatternSwitchLabelSyntax;
                            Visit(casePatternSwitchLabelSyntax.Pattern);
                            if (casePatternSwitchLabelSyntax.WhenClause != null)
                            {
                                VisitNodeToBind(casePatternSwitchLabelSyntax.WhenClause!.Condition);
                            }
                            _nodeToBind = nodeToBind;
                            break;
                        }
                    case SyntaxKind.CaseSwitchLabel:
                        {
                            CaseSwitchLabelSyntax caseSwitchLabelSyntax = (CaseSwitchLabelSyntax)current;
                            VisitNodeToBind(caseSwitchLabelSyntax.Value);
                            break;
                        }
                }
            }
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            if (node.ArgumentList != null)
            {
                SeparatedSyntaxList<AttributeArgumentSyntax>.Enumerator enumerator = node.ArgumentList!.Arguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttributeArgumentSyntax current = enumerator.Current;
                    VisitNodeToBind(current.Expression);
                }
            }
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            VisitNodeToBind(node.Condition);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            VisitNodeToBind(node.Expression);
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            VariableDesignationSyntax designation = node.Designation;
            if (designation != null && designation.Kind() == SyntaxKind.SingleVariableDesignation)
            {
                TFieldOrLocalSymbol val = MakePatternVariable(node.Type, (SingleVariableDesignationSyntax)node.Designation, _nodeToBind);
                if (val != null)
                {
                    _variablesBuilder.Add(val);
                }
            }
            base.VisitDeclarationPattern(node);
        }

        public override void VisitVarPattern(VarPatternSyntax node)
        {
            VisitPatternDesignation(node.Designation);
            base.VisitVarPattern(node);
        }

        private void VisitPatternDesignation(VariableDesignationSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    {
                        TFieldOrLocalSymbol val = MakePatternVariable(null, (SingleVariableDesignationSyntax)node, _nodeToBind);
                        if (val != null)
                        {
                            _variablesBuilder.Add(val);
                        }
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                    {
                        SeparatedSyntaxList<VariableDesignationSyntax>.Enumerator enumerator = ((ParenthesizedVariableDesignationSyntax)node).Variables.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            VariableDesignationSyntax current = enumerator.Current;
                            VisitPatternDesignation(current);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind());
                case SyntaxKind.DiscardDesignation:
                    break;
            }
        }

        public override void VisitRecursivePattern(RecursivePatternSyntax node)
        {
            TFieldOrLocalSymbol val = MakePatternVariable(node, _nodeToBind);
            if (val != null)
            {
                _variablesBuilder.Add(val);
            }
            base.VisitRecursivePattern(node);
        }

        protected abstract TFieldOrLocalSymbol MakePatternVariable(TypeSyntax type, SingleVariableDesignationSyntax designation, SyntaxNode nodeToBind);

        protected abstract TFieldOrLocalSymbol MakePatternVariable(RecursivePatternSyntax node, SyntaxNode nodeToBind);

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            VisitNodeToBind(node.FromClause.Expression);
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
                    VisitNodeToBind(((JoinClauseSyntax)current).InExpression);
                }
            }
            Visit(node.Continuation);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            ArrayBuilder<ExpressionSyntax> instance = ArrayBuilder<ExpressionSyntax>.GetInstance();
            ExpressionSyntax expressionSyntax = node;
            do
            {
                BinaryExpressionSyntax binaryExpressionSyntax = (BinaryExpressionSyntax)expressionSyntax;
                instance.Push(binaryExpressionSyntax.Right);
                expressionSyntax = binaryExpressionSyntax.Left;
            }
            while (expressionSyntax is BinaryExpressionSyntax);
            Visit(expressionSyntax);
            while (instance.Count > 0)
            {
                Visit(instance.Pop());
            }
            instance.Free();
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            BaseArgumentListSyntax argumentListSyntaxOpt = (node.Parent as ArgumentSyntax)?.Parent as BaseArgumentListSyntax;
            VisitDeclarationExpressionDesignation(node, node.Designation, argumentListSyntaxOpt);
        }

        private void VisitDeclarationExpressionDesignation(DeclarationExpressionSyntax node, VariableDesignationSyntax designation, BaseArgumentListSyntax argumentListSyntaxOpt)
        {
            switch (designation.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    {
                        TFieldOrLocalSymbol val = MakeDeclarationExpressionVariable(node, (SingleVariableDesignationSyntax)designation, argumentListSyntaxOpt, _nodeToBind);
                        if (val != null)
                        {
                            _variablesBuilder.Add(val);
                        }
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                    {
                        SeparatedSyntaxList<VariableDesignationSyntax>.Enumerator enumerator = ((ParenthesizedVariableDesignationSyntax)designation).Variables.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            VariableDesignationSyntax current = enumerator.Current;
                            VisitDeclarationExpressionDesignation(node, current, argumentListSyntaxOpt);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(designation.Kind());
                case SyntaxKind.DiscardDesignation:
                    break;
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            if (node.IsDeconstruction())
            {
                CollectVariablesFromDeconstruction(node.Left, node);
            }
            else
            {
                Visit(node.Left);
            }
            Visit(node.Right);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (node.Initializer != null)
            {
                VisitNodeToBind(node.Initializer);
            }
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeIfClass = node.PrimaryConstructorBaseTypeIfClass;
            if (primaryConstructorBaseTypeIfClass != null)
            {
                VisitNodeToBind(primaryConstructorBaseTypeIfClass);
            }
        }

        private void CollectVariablesFromDeconstruction(ExpressionSyntax possibleTupleDeclaration, AssignmentExpressionSyntax deconstruction)
        {
            switch (possibleTupleDeclaration.Kind())
            {
                case SyntaxKind.TupleExpression:
                    {
                        SeparatedSyntaxList<ArgumentSyntax>.Enumerator enumerator = ((TupleExpressionSyntax)possibleTupleDeclaration).Arguments.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ArgumentSyntax current = enumerator.Current;
                            CollectVariablesFromDeconstruction(current.Expression, deconstruction);
                        }
                        break;
                    }
                case SyntaxKind.DeclarationExpression:
                    {
                        DeclarationExpressionSyntax declarationExpressionSyntax = (DeclarationExpressionSyntax)possibleTupleDeclaration;
                        CollectVariablesFromDeconstruction(declarationExpressionSyntax.Designation, declarationExpressionSyntax.Type, deconstruction);
                        break;
                    }
                default:
                    Visit(possibleTupleDeclaration);
                    break;
            }
        }

        private void CollectVariablesFromDeconstruction(VariableDesignationSyntax designation, TypeSyntax closestTypeSyntax, AssignmentExpressionSyntax deconstruction)
        {
            switch (designation.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    {
                        SingleVariableDesignationSyntax designation2 = (SingleVariableDesignationSyntax)designation;
                        TFieldOrLocalSymbol val = MakeDeconstructionVariable(closestTypeSyntax, designation2, deconstruction);
                        if (val != null)
                        {
                            _variablesBuilder.Add(val);
                        }
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                    {
                        SeparatedSyntaxList<VariableDesignationSyntax>.Enumerator enumerator = ((ParenthesizedVariableDesignationSyntax)designation).Variables.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            VariableDesignationSyntax current = enumerator.Current;
                            CollectVariablesFromDeconstruction(current, closestTypeSyntax, deconstruction);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(designation.Kind());
                case SyntaxKind.DiscardDesignation:
                    break;
            }
        }

        protected abstract TFieldOrLocalSymbol MakeDeclarationExpressionVariable(DeclarationExpressionSyntax node, SingleVariableDesignationSyntax designation, BaseArgumentListSyntax argumentListSyntax, SyntaxNode nodeToBind);

        protected abstract TFieldOrLocalSymbol MakeDeconstructionVariable(TypeSyntax closestTypeSyntax, SingleVariableDesignationSyntax designation, AssignmentExpressionSyntax deconstruction);
    }
    internal class ExpressionVariableFinder : ExpressionVariableFinder<LocalSymbol>
    {
        private Binder _scopeBinder;

        private Binder _enclosingBinder;

        private static readonly ObjectPool<ExpressionVariableFinder> s_poolInstance = CreatePool();

        internal static void FindExpressionVariables(Binder scopeBinder, ArrayBuilder<LocalSymbol> builder, CSharpSyntaxNode node, Binder enclosingBinderOpt = null)
        {
            if (node != null)
            {
                ExpressionVariableFinder expressionVariableFinder = s_poolInstance.Allocate();
                expressionVariableFinder._scopeBinder = scopeBinder;
                expressionVariableFinder._enclosingBinder = enclosingBinderOpt ?? scopeBinder;
                expressionVariableFinder.FindExpressionVariables(builder, node);
                expressionVariableFinder._scopeBinder = null;
                expressionVariableFinder._enclosingBinder = null;
                s_poolInstance.Free(expressionVariableFinder);
            }
        }

        internal static void FindExpressionVariables(Binder binder, ArrayBuilder<LocalSymbol> builder, SeparatedSyntaxList<ExpressionSyntax> nodes)
        {
            if (nodes.Count != 0)
            {
                ExpressionVariableFinder expressionVariableFinder = s_poolInstance.Allocate();
                expressionVariableFinder._scopeBinder = binder;
                expressionVariableFinder._enclosingBinder = binder;
                expressionVariableFinder.FindExpressionVariables(builder, nodes);
                expressionVariableFinder._scopeBinder = null;
                expressionVariableFinder._enclosingBinder = null;
                s_poolInstance.Free(expressionVariableFinder);
            }
        }

        protected override LocalSymbol MakePatternVariable(TypeSyntax type, SingleVariableDesignationSyntax designation, SyntaxNode nodeToBind)
        {
            return MakePatternVariable(type, designation, nodeToBind);
        }

        protected override LocalSymbol MakePatternVariable(RecursivePatternSyntax node, SyntaxNode nodeToBind)
        {
            return MakePatternVariable(node.Type, node.Designation, nodeToBind);
        }

        private LocalSymbol MakePatternVariable(TypeSyntax type, VariableDesignationSyntax variableDesignation, SyntaxNode nodeToBind)
        {
            if (!(variableDesignation is SingleVariableDesignationSyntax singleVariableDesignationSyntax))
            {
                return null;
            }
            NamedTypeSymbol containingType = _scopeBinder.ContainingType;
            if ((object)containingType != null && containingType.IsScriptClass && (object)_scopeBinder.LookupDeclaredField(singleVariableDesignationSyntax) != null)
            {
                return null;
            }
            return SourceLocalSymbol.MakeLocalSymbolWithEnclosingContext(_scopeBinder.ContainingMemberOrLambda, _scopeBinder, _enclosingBinder, type, singleVariableDesignationSyntax.Identifier, LocalDeclarationKind.PatternVariable, nodeToBind, null);
        }

        protected override LocalSymbol MakeDeclarationExpressionVariable(DeclarationExpressionSyntax node, SingleVariableDesignationSyntax designation, BaseArgumentListSyntax argumentListSyntaxOpt, SyntaxNode nodeToBind)
        {
            NamedTypeSymbol containingType = _scopeBinder.ContainingType;
            if ((object)containingType != null && containingType.IsScriptClass && (object)_scopeBinder.LookupDeclaredField(designation) != null)
            {
                return null;
            }
            return SourceLocalSymbol.MakeLocalSymbolWithEnclosingContext(_scopeBinder.ContainingMemberOrLambda, _scopeBinder, _enclosingBinder, node.Type, designation.Identifier, node.IsOutVarDeclaration() ? LocalDeclarationKind.OutVariable : LocalDeclarationKind.DeclarationExpressionVariable, nodeToBind, argumentListSyntaxOpt);
        }

        protected override LocalSymbol MakeDeconstructionVariable(TypeSyntax closestTypeSyntax, SingleVariableDesignationSyntax designation, AssignmentExpressionSyntax deconstruction)
        {
            NamedTypeSymbol containingType = _scopeBinder.ContainingType;
            if ((object)containingType != null && containingType.IsScriptClass && (object)_scopeBinder.LookupDeclaredField(designation) != null)
            {
                return null;
            }
            return SourceLocalSymbol.MakeDeconstructionLocal(_scopeBinder.ContainingMemberOrLambda, _scopeBinder, _enclosingBinder, closestTypeSyntax, designation.Identifier, LocalDeclarationKind.DeconstructionVariable, deconstruction);
        }

        public static ObjectPool<ExpressionVariableFinder> CreatePool()
        {
            return new ObjectPool<ExpressionVariableFinder>(() => new ExpressionVariableFinder(), 10);
        }
    }
}
