using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class CSharpOperationFactory
    {
        internal class Helper
        {
            internal static bool IsPostfixIncrementOrDecrement(Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind operatorKind)
            {
                Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind unaryOperatorKind = operatorKind.Operator();
                if (unaryOperatorKind == Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.PostfixIncrement || unaryOperatorKind == Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.PostfixDecrement)
                {
                    return true;
                }
                return false;
            }

            internal static bool IsDecrement(Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind operatorKind)
            {
                Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind unaryOperatorKind = operatorKind.Operator();
                if (unaryOperatorKind == Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.PostfixDecrement || unaryOperatorKind == Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.PrefixDecrement)
                {
                    return true;
                }
                return false;
            }

            internal static UnaryOperatorKind DeriveUnaryOperatorKind(Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind operatorKind)
            {
                return operatorKind.Operator() switch
                {
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.UnaryPlus => UnaryOperatorKind.Plus,
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.UnaryMinus => UnaryOperatorKind.Minus,
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.LogicalNegation => UnaryOperatorKind.Not,
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.BitwiseComplement => UnaryOperatorKind.BitwiseNegation,
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.True => UnaryOperatorKind.True,
                    Microsoft.CodeAnalysis.CSharp.UnaryOperatorKind.False => UnaryOperatorKind.False,
                    _ => UnaryOperatorKind.None,
                };
            }

            internal static BinaryOperatorKind DeriveBinaryOperatorKind(Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind operatorKind)
            {
                return operatorKind.OperatorWithLogical() switch
                {
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Addition => BinaryOperatorKind.Add,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Subtraction => BinaryOperatorKind.Subtract,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Multiplication => BinaryOperatorKind.Multiply,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Division => BinaryOperatorKind.Divide,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Remainder => BinaryOperatorKind.Remainder,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.LeftShift => BinaryOperatorKind.LeftShift,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.RightShift => BinaryOperatorKind.RightShift,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.And => BinaryOperatorKind.And,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Or => BinaryOperatorKind.Or,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Xor => BinaryOperatorKind.ExclusiveOr,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.LessThan => BinaryOperatorKind.LessThan,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.LessThanOrEqual => BinaryOperatorKind.LessThanOrEqual,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.Equal => BinaryOperatorKind.Equals,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.NotEqual => BinaryOperatorKind.NotEquals,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.GreaterThanOrEqual => BinaryOperatorKind.GreaterThanOrEqual,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.GreaterThan => BinaryOperatorKind.GreaterThan,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.LogicalAnd => BinaryOperatorKind.ConditionalAnd,
                    Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.LogicalOr => BinaryOperatorKind.ConditionalOr,
                    _ => BinaryOperatorKind.None,
                };
            }
        }

        private readonly SemanticModel _semanticModel;

        public CSharpOperationFactory(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("boundNode")]
        public IOperation? Create(BoundNode? boundNode)
        {
            if (boundNode == null)
            {
                return null;
            }
            switch (boundNode!.Kind)
            {
                case BoundKind.DeconstructValuePlaceholder:
                    return CreateBoundDeconstructValuePlaceholderOperation((BoundDeconstructValuePlaceholder)boundNode);
                case BoundKind.DeconstructionAssignmentOperator:
                    return CreateBoundDeconstructionAssignmentOperator((BoundDeconstructionAssignmentOperator)boundNode);
                case BoundKind.Call:
                    return CreateBoundCallOperation((BoundCall)boundNode);
                case BoundKind.Local:
                    return CreateBoundLocalOperation((BoundLocal)boundNode);
                case BoundKind.FieldAccess:
                    return CreateBoundFieldAccessOperation((BoundFieldAccess)boundNode);
                case BoundKind.PropertyAccess:
                    return CreateBoundPropertyAccessOperation((BoundPropertyAccess)boundNode);
                case BoundKind.IndexerAccess:
                    return CreateBoundIndexerAccessOperation((BoundIndexerAccess)boundNode);
                case BoundKind.EventAccess:
                    return CreateBoundEventAccessOperation((BoundEventAccess)boundNode);
                case BoundKind.EventAssignmentOperator:
                    return CreateBoundEventAssignmentOperatorOperation((BoundEventAssignmentOperator)boundNode);
                case BoundKind.Parameter:
                    return CreateBoundParameterOperation((BoundParameter)boundNode);
                case BoundKind.Literal:
                    return CreateBoundLiteralOperation((BoundLiteral)boundNode);
                case BoundKind.DynamicInvocation:
                    return CreateBoundDynamicInvocationExpressionOperation((BoundDynamicInvocation)boundNode);
                case BoundKind.DynamicIndexerAccess:
                    return CreateBoundDynamicIndexerAccessExpressionOperation((BoundDynamicIndexerAccess)boundNode);
                case BoundKind.ObjectCreationExpression:
                    return CreateBoundObjectCreationExpressionOperation((BoundObjectCreationExpression)boundNode);
                case BoundKind.WithExpression:
                    return CreateBoundWithExpressionOperation((BoundWithExpression)boundNode);
                case BoundKind.DynamicObjectCreationExpression:
                    return CreateBoundDynamicObjectCreationExpressionOperation((BoundDynamicObjectCreationExpression)boundNode);
                case BoundKind.ObjectInitializerExpression:
                    return CreateBoundObjectInitializerExpressionOperation((BoundObjectInitializerExpression)boundNode);
                case BoundKind.CollectionInitializerExpression:
                    return CreateBoundCollectionInitializerExpressionOperation((BoundCollectionInitializerExpression)boundNode);
                case BoundKind.ObjectInitializerMember:
                    return CreateBoundObjectInitializerMemberOperation((BoundObjectInitializerMember)boundNode);
                case BoundKind.CollectionElementInitializer:
                    return CreateBoundCollectionElementInitializerOperation((BoundCollectionElementInitializer)boundNode);
                case BoundKind.DynamicObjectInitializerMember:
                    return CreateBoundDynamicObjectInitializerMemberOperation((BoundDynamicObjectInitializerMember)boundNode);
                case BoundKind.DynamicMemberAccess:
                    return CreateBoundDynamicMemberAccessOperation((BoundDynamicMemberAccess)boundNode);
                case BoundKind.DynamicCollectionElementInitializer:
                    return CreateBoundDynamicCollectionElementInitializerOperation((BoundDynamicCollectionElementInitializer)boundNode);
                case BoundKind.UnboundLambda:
                    return CreateUnboundLambdaOperation((UnboundLambda)boundNode);
                case BoundKind.Lambda:
                    return CreateBoundLambdaOperation((BoundLambda)boundNode);
                case BoundKind.Conversion:
                    return CreateBoundConversionOperation((BoundConversion)boundNode);
                case BoundKind.AsOperator:
                    return CreateBoundAsOperatorOperation((BoundAsOperator)boundNode);
                case BoundKind.IsOperator:
                    return CreateBoundIsOperatorOperation((BoundIsOperator)boundNode);
                case BoundKind.SizeOfOperator:
                    return CreateBoundSizeOfOperatorOperation((BoundSizeOfOperator)boundNode);
                case BoundKind.TypeOfOperator:
                    return CreateBoundTypeOfOperatorOperation((BoundTypeOfOperator)boundNode);
                case BoundKind.ArrayCreation:
                    return CreateBoundArrayCreationOperation((BoundArrayCreation)boundNode);
                case BoundKind.ArrayInitialization:
                    return CreateBoundArrayInitializationOperation((BoundArrayInitialization)boundNode);
                case BoundKind.DefaultLiteral:
                    return CreateBoundDefaultLiteralOperation((BoundDefaultLiteral)boundNode);
                case BoundKind.DefaultExpression:
                    return CreateBoundDefaultExpressionOperation((BoundDefaultExpression)boundNode);
                case BoundKind.BaseReference:
                    return CreateBoundBaseReferenceOperation((BoundBaseReference)boundNode);
                case BoundKind.ThisReference:
                    return CreateBoundThisReferenceOperation((BoundThisReference)boundNode);
                case BoundKind.AssignmentOperator:
                    return CreateBoundAssignmentOperatorOrMemberInitializerOperation((BoundAssignmentOperator)boundNode);
                case BoundKind.CompoundAssignmentOperator:
                    return CreateBoundCompoundAssignmentOperatorOperation((BoundCompoundAssignmentOperator)boundNode);
                case BoundKind.IncrementOperator:
                    return CreateBoundIncrementOperatorOperation((BoundIncrementOperator)boundNode);
                case BoundKind.BadExpression:
                    return CreateBoundBadExpressionOperation((BoundBadExpression)boundNode);
                case BoundKind.NewT:
                    return CreateBoundNewTOperation((BoundNewT)boundNode);
                case BoundKind.NoPiaObjectCreationExpression:
                    return CreateNoPiaObjectCreationExpressionOperation((BoundNoPiaObjectCreationExpression)boundNode);
                case BoundKind.UnaryOperator:
                    return CreateBoundUnaryOperatorOperation((BoundUnaryOperator)boundNode);
                case BoundKind.BinaryOperator:
                case BoundKind.UserDefinedConditionalLogicalOperator:
                    return CreateBoundBinaryOperatorBase((BoundBinaryOperatorBase)boundNode);
                case BoundKind.TupleBinaryOperator:
                    return CreateBoundTupleBinaryOperatorOperation((BoundTupleBinaryOperator)boundNode);
                case BoundKind.ConditionalOperator:
                    return CreateBoundConditionalOperatorOperation((BoundConditionalOperator)boundNode);
                case BoundKind.NullCoalescingOperator:
                    return CreateBoundNullCoalescingOperatorOperation((BoundNullCoalescingOperator)boundNode);
                case BoundKind.AwaitExpression:
                    return CreateBoundAwaitExpressionOperation((BoundAwaitExpression)boundNode);
                case BoundKind.ArrayAccess:
                    return CreateBoundArrayAccessOperation((BoundArrayAccess)boundNode);
                case BoundKind.NameOfOperator:
                    return CreateBoundNameOfOperatorOperation((BoundNameOfOperator)boundNode);
                case BoundKind.ThrowExpression:
                    return CreateBoundThrowExpressionOperation((BoundThrowExpression)boundNode);
                case BoundKind.AddressOfOperator:
                    return CreateBoundAddressOfOperatorOperation((BoundAddressOfOperator)boundNode);
                case BoundKind.ImplicitReceiver:
                    return CreateBoundImplicitReceiverOperation((BoundImplicitReceiver)boundNode);
                case BoundKind.ConditionalAccess:
                    return CreateBoundConditionalAccessOperation((BoundConditionalAccess)boundNode);
                case BoundKind.ConditionalReceiver:
                    return CreateBoundConditionalReceiverOperation((BoundConditionalReceiver)boundNode);
                case BoundKind.FieldEqualsValue:
                    return CreateBoundFieldEqualsValueOperation((BoundFieldEqualsValue)boundNode);
                case BoundKind.PropertyEqualsValue:
                    return CreateBoundPropertyEqualsValueOperation((BoundPropertyEqualsValue)boundNode);
                case BoundKind.ParameterEqualsValue:
                    return CreateBoundParameterEqualsValueOperation((BoundParameterEqualsValue)boundNode);
                case BoundKind.Block:
                    return CreateBoundBlockOperation((BoundBlock)boundNode);
                case BoundKind.ContinueStatement:
                    return CreateBoundContinueStatementOperation((BoundContinueStatement)boundNode);
                case BoundKind.BreakStatement:
                    return CreateBoundBreakStatementOperation((BoundBreakStatement)boundNode);
                case BoundKind.YieldBreakStatement:
                    return CreateBoundYieldBreakStatementOperation((BoundYieldBreakStatement)boundNode);
                case BoundKind.GotoStatement:
                    return CreateBoundGotoStatementOperation((BoundGotoStatement)boundNode);
                case BoundKind.NoOpStatement:
                    return CreateBoundNoOpStatementOperation((BoundNoOpStatement)boundNode);
                case BoundKind.IfStatement:
                    return CreateBoundIfStatementOperation((BoundIfStatement)boundNode);
                case BoundKind.WhileStatement:
                    return CreateBoundWhileStatementOperation((BoundWhileStatement)boundNode);
                case BoundKind.DoStatement:
                    return CreateBoundDoStatementOperation((BoundDoStatement)boundNode);
                case BoundKind.ForStatement:
                    return CreateBoundForStatementOperation((BoundForStatement)boundNode);
                case BoundKind.ForEachStatement:
                    return CreateBoundForEachStatementOperation((BoundForEachStatement)boundNode);
                case BoundKind.TryStatement:
                    return CreateBoundTryStatementOperation((BoundTryStatement)boundNode);
                case BoundKind.CatchBlock:
                    return CreateBoundCatchBlockOperation((BoundCatchBlock)boundNode);
                case BoundKind.FixedStatement:
                    return CreateBoundFixedStatementOperation((BoundFixedStatement)boundNode);
                case BoundKind.UsingStatement:
                    return CreateBoundUsingStatementOperation((BoundUsingStatement)boundNode);
                case BoundKind.ThrowStatement:
                    return CreateBoundThrowStatementOperation((BoundThrowStatement)boundNode);
                case BoundKind.ReturnStatement:
                    return CreateBoundReturnStatementOperation((BoundReturnStatement)boundNode);
                case BoundKind.YieldReturnStatement:
                    return CreateBoundYieldReturnStatementOperation((BoundYieldReturnStatement)boundNode);
                case BoundKind.LockStatement:
                    return CreateBoundLockStatementOperation((BoundLockStatement)boundNode);
                case BoundKind.BadStatement:
                    return CreateBoundBadStatementOperation((BoundBadStatement)boundNode);
                case BoundKind.LocalDeclaration:
                    return CreateBoundLocalDeclarationOperation((BoundLocalDeclaration)boundNode);
                case BoundKind.MultipleLocalDeclarations:
                case BoundKind.UsingLocalDeclarations:
                    return CreateBoundMultipleLocalDeclarationsBaseOperation((BoundMultipleLocalDeclarationsBase)boundNode);
                case BoundKind.LabelStatement:
                    return CreateBoundLabelStatementOperation((BoundLabelStatement)boundNode);
                case BoundKind.LabeledStatement:
                    return CreateBoundLabeledStatementOperation((BoundLabeledStatement)boundNode);
                case BoundKind.ExpressionStatement:
                    return CreateBoundExpressionStatementOperation((BoundExpressionStatement)boundNode);
                case BoundKind.TupleLiteral:
                case BoundKind.ConvertedTupleLiteral:
                    return CreateBoundTupleOperation((BoundTupleExpression)boundNode);
                case BoundKind.UnconvertedInterpolatedString:
                    throw ExceptionUtilities.Unreachable;
                case BoundKind.InterpolatedString:
                    return CreateBoundInterpolatedStringExpressionOperation((BoundInterpolatedString)boundNode);
                case BoundKind.StringInsert:
                    return CreateBoundInterpolationOperation((BoundStringInsert)boundNode);
                case BoundKind.LocalFunctionStatement:
                    return CreateBoundLocalFunctionStatementOperation((BoundLocalFunctionStatement)boundNode);
                case BoundKind.AnonymousObjectCreationExpression:
                    return CreateBoundAnonymousObjectCreationExpressionOperation((BoundAnonymousObjectCreationExpression)boundNode);
                case BoundKind.AnonymousPropertyDeclaration:
                    throw ExceptionUtilities.Unreachable;
                case BoundKind.ConstantPattern:
                    return CreateBoundConstantPatternOperation((BoundConstantPattern)boundNode);
                case BoundKind.DeclarationPattern:
                    return CreateBoundDeclarationPatternOperation((BoundDeclarationPattern)boundNode);
                case BoundKind.RecursivePattern:
                    return CreateBoundRecursivePatternOperation((BoundRecursivePattern)boundNode);
                case BoundKind.ITuplePattern:
                    return CreateBoundRecursivePatternOperation((BoundITuplePattern)boundNode);
                case BoundKind.DiscardPattern:
                    return CreateBoundDiscardPatternOperation((BoundDiscardPattern)boundNode);
                case BoundKind.BinaryPattern:
                    return CreateBoundBinaryPatternOperation((BoundBinaryPattern)boundNode);
                case BoundKind.NegatedPattern:
                    return CreateBoundNegatedPatternOperation((BoundNegatedPattern)boundNode);
                case BoundKind.RelationalPattern:
                    return CreateBoundRelationalPatternOperation((BoundRelationalPattern)boundNode);
                case BoundKind.TypePattern:
                    return CreateBoundTypePatternOperation((BoundTypePattern)boundNode);
                case BoundKind.SwitchStatement:
                    return CreateBoundSwitchStatementOperation((BoundSwitchStatement)boundNode);
                case BoundKind.SwitchLabel:
                    return CreateBoundSwitchLabelOperation((BoundSwitchLabel)boundNode);
                case BoundKind.IsPatternExpression:
                    return CreateBoundIsPatternExpressionOperation((BoundIsPatternExpression)boundNode);
                case BoundKind.QueryClause:
                    return CreateBoundQueryClauseOperation((BoundQueryClause)boundNode);
                case BoundKind.DelegateCreationExpression:
                    return CreateBoundDelegateCreationExpressionOperation((BoundDelegateCreationExpression)boundNode);
                case BoundKind.RangeVariable:
                    return CreateBoundRangeVariableOperation((BoundRangeVariable)boundNode);
                case BoundKind.ConstructorMethodBody:
                    return CreateConstructorBodyOperation((BoundConstructorMethodBody)boundNode);
                case BoundKind.NonConstructorMethodBody:
                    return CreateMethodBodyOperation((BoundNonConstructorMethodBody)boundNode);
                case BoundKind.DiscardExpression:
                    return CreateBoundDiscardExpressionOperation((BoundDiscardExpression)boundNode);
                case BoundKind.NullCoalescingAssignmentOperator:
                    return CreateBoundNullCoalescingAssignmentOperatorOperation((BoundNullCoalescingAssignmentOperator)boundNode);
                case BoundKind.FromEndIndexExpression:
                    return CreateFromEndIndexExpressionOperation((BoundFromEndIndexExpression)boundNode);
                case BoundKind.RangeExpression:
                    return CreateRangeExpressionOperation((BoundRangeExpression)boundNode);
                case BoundKind.SwitchSection:
                    return CreateBoundSwitchSectionOperation((BoundSwitchSection)boundNode);
                case BoundKind.UnconvertedConditionalOperator:
                    throw ExceptionUtilities.Unreachable;
                case BoundKind.UnconvertedSwitchExpression:
                    throw ExceptionUtilities.Unreachable;
                case BoundKind.ConvertedSwitchExpression:
                    return CreateBoundSwitchExpressionOperation((BoundSwitchExpression)boundNode);
                case BoundKind.SwitchExpressionArm:
                    return CreateBoundSwitchExpressionArmOperation((BoundSwitchExpressionArm)boundNode);
                case BoundKind.ObjectOrCollectionValuePlaceholder:
                    return CreateCollectionValuePlaceholderOperation((BoundObjectOrCollectionValuePlaceholder)boundNode);
                case BoundKind.FunctionPointerInvocation:
                    return CreateBoundFunctionPointerInvocationOperation((BoundFunctionPointerInvocation)boundNode);
                case BoundKind.UnconvertedAddressOfOperator:
                    return CreateBoundUnconvertedAddressOfOperatorOperation((BoundUnconvertedAddressOfOperator)boundNode);
                case BoundKind.GlobalStatementInitializer:
                case BoundKind.TypeExpression:
                case BoundKind.TypeOrValueExpression:
                case BoundKind.NamespaceExpression:
                case BoundKind.PointerIndirectionOperator:
                case BoundKind.PointerElementAccess:
                case BoundKind.RefTypeOperator:
                case BoundKind.MakeRefOperator:
                case BoundKind.RefValueOperator:
                case BoundKind.ArgList:
                case BoundKind.ArgListOperator:
                case BoundKind.FixedLocalCollectionInitializer:
                case BoundKind.PreviousSubmissionReference:
                case BoundKind.HostObjectMemberReference:
                case BoundKind.Sequence:
                case BoundKind.MethodGroup:
                case BoundKind.Attribute:
                case BoundKind.StackAllocArrayCreation:
                case BoundKind.ConvertedStackAllocExpression:
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    {
                        ConstantValue constantValue = (boundNode as BoundExpression)?.ConstantValue;
                        bool flag = boundNode!.WasCompilerGenerated;
                        if (!flag && boundNode!.Kind == BoundKind.FixedLocalCollectionInitializer)
                        {
                            flag = true;
                        }
                        return new NoneOperation(GetIOperationChildren(boundNode), _semanticModel, boundNode!.Syntax, null, constantValue, flag);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(boundNode!.Kind);
            }
        }

        public ImmutableArray<TOperation> CreateFromArray<TBoundNode, TOperation>(ImmutableArray<TBoundNode> boundNodes) where TBoundNode : BoundNode where TOperation : class, IOperation
        {
            if (boundNodes.IsDefault)
            {
                return ImmutableArray<TOperation>.Empty;
            }
            ArrayBuilder<TOperation> instance = ArrayBuilder<TOperation>.GetInstance(boundNodes.Length);
            ImmutableArray<TBoundNode>.Enumerator enumerator = boundNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TBoundNode current = enumerator.Current;
                instance.AddIfNotNull((TOperation)Create(current));
            }
            return instance.ToImmutableAndFree();
        }

        private IMethodBodyOperation CreateMethodBodyOperation(BoundNonConstructorMethodBody boundNode)
        {
            return new MethodBodyOperation((IBlockOperation)Create(boundNode.BlockBody), (IBlockOperation)Create(boundNode.ExpressionBody), _semanticModel, boundNode.Syntax, boundNode.WasCompilerGenerated);
        }

        private IConstructorBodyOperation CreateConstructorBodyOperation(BoundConstructorMethodBody boundNode)
        {
            return new ConstructorBodyOperation(boundNode.Locals.GetPublicSymbols(), Create(boundNode.Initializer), (IBlockOperation)Create(boundNode.BlockBody), (IBlockOperation)Create(boundNode.ExpressionBody), _semanticModel, boundNode.Syntax, boundNode.WasCompilerGenerated);
        }

        internal ImmutableArray<IOperation> GetIOperationChildren(IBoundNodeWithIOperationChildren boundNodeWithChildren)
        {
            ImmutableArray<BoundNode> children = boundNodeWithChildren.Children;
            if (children.IsDefaultOrEmpty)
            {
                return ImmutableArray<IOperation>.Empty;
            }
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(children.Length);
            ImmutableArray<BoundNode>.Enumerator enumerator = children.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundNode current = enumerator.Current;
                if (current != null)
                {
                    IOperation item = Create(current);
                    instance.Add(item);
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal ImmutableArray<IVariableDeclaratorOperation> CreateVariableDeclarator(BoundNode declaration, SyntaxNode declarationSyntax)
        {
            switch (declaration.Kind)
            {
                case BoundKind.LocalDeclaration:
                    return ImmutableArray.Create(CreateVariableDeclaratorInternal((BoundLocalDeclaration)declaration, (declarationSyntax as VariableDeclarationSyntax)?.Variables[0] ?? declarationSyntax));
                case BoundKind.MultipleLocalDeclarations:
                case BoundKind.UsingLocalDeclarations:
                    {
                        BoundMultipleLocalDeclarationsBase obj = (BoundMultipleLocalDeclarationsBase)declaration;
                        ArrayBuilder<IVariableDeclaratorOperation> instance = ArrayBuilder<IVariableDeclaratorOperation>.GetInstance(obj.LocalDeclarations.Length);
                        ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator = obj.LocalDeclarations.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            BoundLocalDeclaration current = enumerator.Current;
                            instance.Add(CreateVariableDeclaratorInternal(current, current.Syntax));
                        }
                        return instance.ToImmutableAndFree();
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaration.Kind);
            }
        }

        private IPlaceholderOperation CreateBoundDeconstructValuePlaceholderOperation(BoundDeconstructValuePlaceholder boundDeconstructValuePlaceholder)
        {
            SyntaxNode syntax = boundDeconstructValuePlaceholder.Syntax;
            ITypeSymbol publicTypeSymbol = boundDeconstructValuePlaceholder.GetPublicTypeSymbol();
            bool wasCompilerGenerated = boundDeconstructValuePlaceholder.WasCompilerGenerated;
            return new PlaceholderOperation(PlaceholderKind.Unspecified, _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IDeconstructionAssignmentOperation CreateBoundDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator boundDeconstructionAssignmentOperator)
        {
            return new DeconstructionAssignmentOperation(Create(boundDeconstructionAssignmentOperator.Left), Create(boundDeconstructionAssignmentOperator.Right.Operand), syntax: boundDeconstructionAssignmentOperator.Syntax, type: boundDeconstructionAssignmentOperator.GetPublicTypeSymbol(), isImplicit: boundDeconstructionAssignmentOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundCallOperation(BoundCall boundCall)
        {
            MethodSymbol method = boundCall.Method;
            SyntaxNode syntax = boundCall.Syntax;
            ITypeSymbol publicTypeSymbol = boundCall.GetPublicTypeSymbol();
            ConstantValue constantValue = boundCall.ConstantValue;
            bool wasCompilerGenerated = boundCall.WasCompilerGenerated;
            if (!boundCall.OriginalMethodsOpt.IsDefault || IsMethodInvalid(boundCall.ResultKind, method))
            {
                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundCall).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
            }
            bool isVirtual = IsCallVirtual(method, boundCall.ReceiverOpt);
            IOperation instance = CreateReceiverOperation(boundCall.ReceiverOpt, method);
            ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundCall);
            return new InvocationOperation(method.GetPublicSymbol(), instance, isVirtual, arguments, _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IOperation CreateBoundFunctionPointerInvocationOperation(BoundFunctionPointerInvocation boundFunctionPointerInvocation)
        {
            ITypeSymbol publicTypeSymbol = boundFunctionPointerInvocation.GetPublicTypeSymbol();
            SyntaxNode syntax = boundFunctionPointerInvocation.Syntax;
            bool wasCompilerGenerated = boundFunctionPointerInvocation.WasCompilerGenerated;
            if (boundFunctionPointerInvocation.ResultKind != LookupResultKind.Viable)
            {
                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundFunctionPointerInvocation).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, null, wasCompilerGenerated);
            }
            return new NoneOperation(GetIOperationChildren(boundFunctionPointerInvocation), _semanticModel, syntax, publicTypeSymbol, null, wasCompilerGenerated);
        }

        private IOperation CreateBoundUnconvertedAddressOfOperatorOperation(BoundUnconvertedAddressOfOperator boundUnconvertedAddressOf)
        {
            return new AddressOfOperation(Create(boundUnconvertedAddressOf.Operand), _semanticModel, boundUnconvertedAddressOf.Syntax, boundUnconvertedAddressOf.GetPublicTypeSymbol(), boundUnconvertedAddressOf.WasCompilerGenerated);
        }

        internal ImmutableArray<IOperation> CreateIgnoredDimensions(BoundNode declaration, SyntaxNode declarationSyntax)
        {
            switch (declaration.Kind)
            {
                case BoundKind.LocalDeclaration:
                    {
                        BoundTypeExpression declaredTypeOpt = ((BoundLocalDeclaration)declaration).DeclaredTypeOpt;
                        return CreateFromArray<BoundExpression, IOperation>(declaredTypeOpt.BoundDimensionsOpt);
                    }
                case BoundKind.MultipleLocalDeclarations:
                case BoundKind.UsingLocalDeclarations:
                    {
                        ImmutableArray<BoundLocalDeclaration> localDeclarations = ((BoundMultipleLocalDeclarationsBase)declaration).LocalDeclarations;
                        ImmutableArray<BoundExpression> boundNodes = ((localDeclarations.Length <= 0) ? ImmutableArray<BoundExpression>.Empty : localDeclarations[0].DeclaredTypeOpt!.BoundDimensionsOpt);
                        return CreateFromArray<BoundExpression, IOperation>(boundNodes);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaration.Kind);
            }
        }

        internal IOperation CreateBoundLocalOperation(BoundLocal boundLocal, bool createDeclaration = true)
        {
            ILocalSymbol publicSymbol = boundLocal.LocalSymbol.GetPublicSymbol();
            bool flag = boundLocal.DeclarationKind != BoundLocalDeclarationKind.None;
            SyntaxNode syntaxNode = boundLocal.Syntax;
            ITypeSymbol publicTypeSymbol = boundLocal.GetPublicTypeSymbol();
            ConstantValue constantValue = boundLocal.ConstantValue;
            bool wasCompilerGenerated = boundLocal.WasCompilerGenerated;
            if (flag && syntaxNode is DeclarationExpressionSyntax declarationExpressionSyntax)
            {
                syntaxNode = declarationExpressionSyntax.Designation;
                if (createDeclaration)
                {
                    return new DeclarationExpressionOperation(CreateBoundLocalOperation(boundLocal, createDeclaration: false), _semanticModel, declarationExpressionSyntax, publicTypeSymbol, isImplicit: false);
                }
            }
            return new LocalReferenceOperation(publicSymbol, flag, _semanticModel, syntaxNode, publicTypeSymbol, constantValue, wasCompilerGenerated);
        }

        internal IOperation CreateBoundFieldAccessOperation(BoundFieldAccess boundFieldAccess, bool createDeclaration = true)
        {
            IFieldSymbol publicSymbol = boundFieldAccess.FieldSymbol.GetPublicSymbol();
            bool isDeclaration = boundFieldAccess.IsDeclaration;
            SyntaxNode syntaxNode = boundFieldAccess.Syntax;
            ITypeSymbol publicTypeSymbol = boundFieldAccess.GetPublicTypeSymbol();
            ConstantValue constantValue = boundFieldAccess.ConstantValue;
            bool wasCompilerGenerated = boundFieldAccess.WasCompilerGenerated;
            if (isDeclaration && syntaxNode is DeclarationExpressionSyntax declarationExpressionSyntax)
            {
                syntaxNode = declarationExpressionSyntax.Designation;
                if (createDeclaration)
                {
                    return new DeclarationExpressionOperation(CreateBoundFieldAccessOperation(boundFieldAccess, createDeclaration: false), _semanticModel, declarationExpressionSyntax, publicTypeSymbol, isImplicit: false);
                }
            }
            IOperation instance = CreateReceiverOperation(boundFieldAccess.ReceiverOpt, boundFieldAccess.FieldSymbol);
            return new FieldReferenceOperation(publicSymbol, isDeclaration, instance, _semanticModel, syntaxNode, publicTypeSymbol, constantValue, wasCompilerGenerated);
        }

        internal IOperation? CreateBoundPropertyReferenceInstance(BoundNode boundNode)
        {
            if (!(boundNode is BoundPropertyAccess boundPropertyAccess))
            {
                if (!(boundNode is BoundObjectInitializerMember boundObjectInitializerMember))
                {
                    if (boundNode is BoundIndexerAccess boundIndexerAccess)
                    {
                        return CreateReceiverOperation(boundIndexerAccess.ReceiverOpt, boundIndexerAccess.ExpressionSymbol);
                    }
                    throw ExceptionUtilities.UnexpectedValue(boundNode.Kind);
                }
                Symbol? memberSymbol = boundObjectInitializerMember.MemberSymbol;
                if ((object)memberSymbol == null || !memberSymbol!.IsStatic)
                {
                    return CreateImplicitReceiver(boundObjectInitializerMember.Syntax, boundObjectInitializerMember.ReceiverType);
                }
                return null;
            }
            return CreateReceiverOperation(boundPropertyAccess.ReceiverOpt, boundPropertyAccess.PropertySymbol);
        }

        private IPropertyReferenceOperation CreateBoundPropertyAccessOperation(BoundPropertyAccess boundPropertyAccess)
        {
            IOperation instance = CreateReceiverOperation(boundPropertyAccess.ReceiverOpt, boundPropertyAccess.PropertySymbol);
            ImmutableArray<IArgumentOperation> empty = ImmutableArray<IArgumentOperation>.Empty;
            return new PropertyReferenceOperation(boundPropertyAccess.PropertySymbol.GetPublicSymbol(), syntax: boundPropertyAccess.Syntax, type: boundPropertyAccess.GetPublicTypeSymbol(), isImplicit: boundPropertyAccess.WasCompilerGenerated, arguments: empty, instance: instance, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundIndexerAccessOperation(BoundIndexerAccess boundIndexerAccess)
        {
            PropertySymbol indexer = boundIndexerAccess.Indexer;
            SyntaxNode syntax = boundIndexerAccess.Syntax;
            ITypeSymbol publicTypeSymbol = boundIndexerAccess.GetPublicTypeSymbol();
            bool wasCompilerGenerated = boundIndexerAccess.WasCompilerGenerated;
            if (!boundIndexerAccess.OriginalIndexersOpt.IsDefault || boundIndexerAccess.ResultKind == LookupResultKind.OverloadResolutionFailure)
            {
                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundIndexerAccess).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, null, wasCompilerGenerated);
            }
            ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundIndexerAccess, isObjectOrCollectionInitializer: false);
            IOperation instance = CreateReceiverOperation(boundIndexerAccess.ReceiverOpt, boundIndexerAccess.ExpressionSymbol);
            return new PropertyReferenceOperation(indexer.GetPublicSymbol(), arguments, instance, _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IEventReferenceOperation CreateBoundEventAccessOperation(BoundEventAccess boundEventAccess)
        {
            return new EventReferenceOperation(boundEventAccess.EventSymbol.GetPublicSymbol(), CreateReceiverOperation(boundEventAccess.ReceiverOpt, boundEventAccess.EventSymbol), syntax: boundEventAccess.Syntax, type: boundEventAccess.GetPublicTypeSymbol(), isImplicit: boundEventAccess.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IEventAssignmentOperation CreateBoundEventAssignmentOperatorOperation(BoundEventAssignmentOperator boundEventAssignmentOperator)
        {
            return new EventAssignmentOperation(CreateBoundEventAccessOperation(boundEventAssignmentOperator), Create(boundEventAssignmentOperator.Argument), syntax: boundEventAssignmentOperator.Syntax, adds: boundEventAssignmentOperator.IsAddition, type: boundEventAssignmentOperator.GetPublicTypeSymbol(), isImplicit: boundEventAssignmentOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IParameterReferenceOperation CreateBoundParameterOperation(BoundParameter boundParameter)
        {
            return new ParameterReferenceOperation(boundParameter.ParameterSymbol.GetPublicSymbol(), syntax: boundParameter.Syntax, type: boundParameter.GetPublicTypeSymbol(), isImplicit: boundParameter.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        internal ILiteralOperation CreateBoundLiteralOperation(BoundLiteral boundLiteral, bool @implicit = false)
        {
            SyntaxNode syntax = boundLiteral.Syntax;
            ITypeSymbol publicTypeSymbol = boundLiteral.GetPublicTypeSymbol();
            ConstantValue constantValue = boundLiteral.ConstantValue;
            bool isImplicit = boundLiteral.WasCompilerGenerated || @implicit;
            return new LiteralOperation(_semanticModel, syntax, publicTypeSymbol, constantValue, isImplicit);
        }

        private IAnonymousObjectCreationOperation CreateBoundAnonymousObjectCreationExpressionOperation(BoundAnonymousObjectCreationExpression boundAnonymousObjectCreationExpression)
        {
            SyntaxNode syntax = boundAnonymousObjectCreationExpression.Syntax;
            ITypeSymbol publicTypeSymbol = boundAnonymousObjectCreationExpression.GetPublicTypeSymbol();
            bool wasCompilerGenerated = boundAnonymousObjectCreationExpression.WasCompilerGenerated;
            return new AnonymousObjectCreationOperation(GetAnonymousObjectCreationInitializers(boundAnonymousObjectCreationExpression.Arguments, boundAnonymousObjectCreationExpression.Declarations, syntax, publicTypeSymbol, wasCompilerGenerated), _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IOperation CreateBoundObjectCreationExpressionOperation(BoundObjectCreationExpression boundObjectCreationExpression)
        {
            MethodSymbol constructor = boundObjectCreationExpression.Constructor;
            SyntaxNode syntax = boundObjectCreationExpression.Syntax;
            ITypeSymbol publicTypeSymbol = boundObjectCreationExpression.GetPublicTypeSymbol();
            ConstantValue constantValue = boundObjectCreationExpression.ConstantValue;
            bool wasCompilerGenerated = boundObjectCreationExpression.WasCompilerGenerated;
            if (boundObjectCreationExpression.ResultKind == LookupResultKind.OverloadResolutionFailure || constructor == null || constructor.OriginalDefinition is ErrorMethodSymbol)
            {
                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundObjectCreationExpression).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
            }
            if (boundObjectCreationExpression.Type.IsAnonymousType)
            {
                return new AnonymousObjectCreationOperation(GetAnonymousObjectCreationInitializers(boundObjectCreationExpression.Arguments, ImmutableArray<BoundAnonymousPropertyDeclaration>.Empty, syntax, publicTypeSymbol, wasCompilerGenerated), _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
            }
            ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundObjectCreationExpression);
            IObjectOrCollectionInitializerOperation initializer = (IObjectOrCollectionInitializerOperation)Create(boundObjectCreationExpression.InitializerExpressionOpt);
            return new ObjectCreationOperation(constructor.GetPublicSymbol(), initializer, arguments, _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
        }

        private IOperation CreateBoundWithExpressionOperation(BoundWithExpression boundWithExpression)
        {
            IOperation? operand = Create(boundWithExpression.Receiver);
            IObjectOrCollectionInitializerOperation initializer = (IObjectOrCollectionInitializerOperation)Create(boundWithExpression.InitializerExpression);
            MethodSymbol cloneMethod = boundWithExpression.CloneMethod;
            return new WithOperation(syntax: boundWithExpression.Syntax, type: boundWithExpression.GetPublicTypeSymbol(), isImplicit: boundWithExpression.WasCompilerGenerated, operand: operand, cloneMethod: cloneMethod.GetPublicSymbol(), initializer: initializer, semanticModel: _semanticModel);
        }

        private IDynamicObjectCreationOperation CreateBoundDynamicObjectCreationExpressionOperation(BoundDynamicObjectCreationExpression boundDynamicObjectCreationExpression)
        {
            return new DynamicObjectCreationOperation((IObjectOrCollectionInitializerOperation)Create(boundDynamicObjectCreationExpression.InitializerExpressionOpt), CreateFromArray<BoundExpression, IOperation>(boundDynamicObjectCreationExpression.Arguments), boundDynamicObjectCreationExpression.ArgumentNamesOpt.NullToEmpty(), boundDynamicObjectCreationExpression.ArgumentRefKindsOpt.NullToEmpty(), syntax: boundDynamicObjectCreationExpression.Syntax, type: boundDynamicObjectCreationExpression.GetPublicTypeSymbol(), isImplicit: boundDynamicObjectCreationExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        internal IOperation CreateBoundDynamicInvocationExpressionReceiver(BoundNode receiver)
        {
            if (!(receiver is BoundObjectOrCollectionValuePlaceholder boundObjectOrCollectionValuePlaceholder))
            {
                if (receiver is BoundMethodGroup boundMethodGroup)
                {
                    return CreateBoundDynamicMemberAccessOperation(boundMethodGroup.ReceiverOpt, TypeMap.AsTypeSymbols(boundMethodGroup.TypeArgumentsOpt), boundMethodGroup.Name, boundMethodGroup.Syntax, boundMethodGroup.GetPublicTypeSymbol(), boundMethodGroup.WasCompilerGenerated);
                }
                return Create(receiver);
            }
            return CreateBoundDynamicMemberAccessOperation(boundObjectOrCollectionValuePlaceholder, ImmutableArray<TypeSymbol>.Empty, "Add", boundObjectOrCollectionValuePlaceholder.Syntax, null, isImplicit: true);
        }

        private IDynamicInvocationOperation CreateBoundDynamicInvocationExpressionOperation(BoundDynamicInvocation boundDynamicInvocation)
        {
            return new DynamicInvocationOperation(CreateBoundDynamicInvocationExpressionReceiver(boundDynamicInvocation.Expression), CreateFromArray<BoundExpression, IOperation>(boundDynamicInvocation.Arguments), boundDynamicInvocation.ArgumentNamesOpt.NullToEmpty(), boundDynamicInvocation.ArgumentRefKindsOpt.NullToEmpty(), syntax: boundDynamicInvocation.Syntax, type: boundDynamicInvocation.GetPublicTypeSymbol(), isImplicit: boundDynamicInvocation.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        internal IOperation CreateBoundDynamicIndexerAccessExpressionReceiver(BoundExpression indexer)
        {
            if (!(indexer is BoundDynamicIndexerAccess boundDynamicIndexerAccess))
            {
                if (indexer is BoundObjectInitializerMember boundObjectInitializerMember)
                {
                    return CreateImplicitReceiver(boundObjectInitializerMember.Syntax, boundObjectInitializerMember.ReceiverType);
                }
                throw ExceptionUtilities.UnexpectedValue(indexer.Kind);
            }
            return Create(boundDynamicIndexerAccess.Receiver);
        }

        internal ImmutableArray<IOperation> CreateBoundDynamicIndexerAccessArguments(BoundExpression indexer)
        {
            if (!(indexer is BoundDynamicIndexerAccess boundDynamicIndexerAccess))
            {
                if (indexer is BoundObjectInitializerMember boundObjectInitializerMember)
                {
                    return CreateFromArray<BoundExpression, IOperation>(boundObjectInitializerMember.Arguments);
                }
                throw ExceptionUtilities.UnexpectedValue(indexer.Kind);
            }
            return CreateFromArray<BoundExpression, IOperation>(boundDynamicIndexerAccess.Arguments);
        }

        private IDynamicIndexerAccessOperation CreateBoundDynamicIndexerAccessExpressionOperation(BoundDynamicIndexerAccess boundDynamicIndexerAccess)
        {
            return new DynamicIndexerAccessOperation(CreateBoundDynamicIndexerAccessExpressionReceiver(boundDynamicIndexerAccess), CreateBoundDynamicIndexerAccessArguments(boundDynamicIndexerAccess), boundDynamicIndexerAccess.ArgumentNamesOpt.NullToEmpty(), boundDynamicIndexerAccess.ArgumentRefKindsOpt.NullToEmpty(), syntax: boundDynamicIndexerAccess.Syntax, type: boundDynamicIndexerAccess.GetPublicTypeSymbol(), isImplicit: boundDynamicIndexerAccess.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IObjectOrCollectionInitializerOperation CreateBoundObjectInitializerExpressionOperation(BoundObjectInitializerExpression boundObjectInitializerExpression)
        {
            return new ObjectOrCollectionInitializerOperation(CreateFromArray<BoundExpression, IOperation>(BoundObjectCreationExpression.GetChildInitializers(boundObjectInitializerExpression)), syntax: boundObjectInitializerExpression.Syntax, type: boundObjectInitializerExpression.GetPublicTypeSymbol(), isImplicit: boundObjectInitializerExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IObjectOrCollectionInitializerOperation CreateBoundCollectionInitializerExpressionOperation(BoundCollectionInitializerExpression boundCollectionInitializerExpression)
        {
            return new ObjectOrCollectionInitializerOperation(CreateFromArray<BoundExpression, IOperation>(BoundObjectCreationExpression.GetChildInitializers(boundCollectionInitializerExpression)), syntax: boundCollectionInitializerExpression.Syntax, type: boundCollectionInitializerExpression.GetPublicTypeSymbol(), isImplicit: boundCollectionInitializerExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundObjectInitializerMemberOperation(BoundObjectInitializerMember boundObjectInitializerMember, bool isObjectOrCollectionInitializer = false)
        {
            BoundObjectInitializerMember boundObjectInitializerMember2 = boundObjectInitializerMember;
            Symbol memberSymbol = boundObjectInitializerMember2.MemberSymbol;
            SyntaxNode syntax = boundObjectInitializerMember2.Syntax;
            ITypeSymbol publicTypeSymbol = boundObjectInitializerMember2.GetPublicTypeSymbol();
            bool wasCompilerGenerated = boundObjectInitializerMember2.WasCompilerGenerated;
            if ((object)memberSymbol == null)
            {
                IOperation operation = CreateBoundDynamicIndexerAccessExpressionReceiver(boundObjectInitializerMember2);
                ImmutableArray<IOperation> arguments = CreateBoundDynamicIndexerAccessArguments(boundObjectInitializerMember2);
                ImmutableArray<string> argumentNames = boundObjectInitializerMember2.ArgumentNamesOpt.NullToEmpty();
                ImmutableArray<RefKind> argumentRefKinds = boundObjectInitializerMember2.ArgumentRefKindsOpt.NullToEmpty();
                return new DynamicIndexerAccessOperation(operation, arguments, argumentNames, argumentRefKinds, _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
            }
            switch (memberSymbol.Kind)
            {
                case SymbolKind.Field:
                    {
                        FieldSymbol symbol = (FieldSymbol)memberSymbol;
                        bool isDeclaration = false;
                        return new FieldReferenceOperation(symbol.GetPublicSymbol(), isDeclaration, createReceiver(), _semanticModel, syntax, publicTypeSymbol, null, wasCompilerGenerated);
                    }
                case SymbolKind.Event:
                    return new EventReferenceOperation(((EventSymbol)memberSymbol).GetPublicSymbol(), createReceiver(), _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
                case SymbolKind.Property:
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)memberSymbol;
                        ImmutableArray<IArgumentOperation> arguments2;
                        if (!boundObjectInitializerMember2.Arguments.IsEmpty)
                        {
                            MethodSymbol methodSymbol = (isObjectOrCollectionInitializer ? propertySymbol.GetOwnOrInheritedGetMethod() : propertySymbol.GetOwnOrInheritedSetMethod());
                            if (methodSymbol == null || boundObjectInitializerMember2.ResultKind == LookupResultKind.OverloadResolutionFailure || methodSymbol.OriginalDefinition is ErrorMethodSymbol)
                            {
                                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundObjectInitializerMember2).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, null, wasCompilerGenerated);
                            }
                            arguments2 = DeriveArguments(boundObjectInitializerMember2, isObjectOrCollectionInitializer);
                        }
                        else
                        {
                            arguments2 = ImmutableArray<IArgumentOperation>.Empty;
                        }
                        return new PropertyReferenceOperation(propertySymbol.GetPublicSymbol(), arguments2, createReceiver(), _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
                    }
                default:
                    throw ExceptionUtilities.Unreachable;
            }
            IOperation? createReceiver()
            {
                Symbol symbol2 = memberSymbol;
                if ((object)symbol2 == null || !symbol2.IsStatic)
                {
                    return CreateImplicitReceiver(boundObjectInitializerMember2.Syntax, boundObjectInitializerMember2.ReceiverType);
                }
                return null;
            }
        }

        private IOperation CreateBoundDynamicObjectInitializerMemberOperation(BoundDynamicObjectInitializerMember boundDynamicObjectInitializerMember)
        {
            return new DynamicMemberReferenceOperation(CreateImplicitReceiver(boundDynamicObjectInitializerMember.Syntax, boundDynamicObjectInitializerMember.ReceiverType), boundDynamicObjectInitializerMember.MemberName, ImmutableArray<ITypeSymbol>.Empty, boundDynamicObjectInitializerMember.ReceiverType.GetPublicSymbol(), syntax: boundDynamicObjectInitializerMember.Syntax, type: boundDynamicObjectInitializerMember.GetPublicTypeSymbol(), isImplicit: boundDynamicObjectInitializerMember.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundCollectionElementInitializerOperation(BoundCollectionElementInitializer boundCollectionElementInitializer)
        {
            MethodSymbol addMethod = boundCollectionElementInitializer.AddMethod;
            IOperation instance = CreateReceiverOperation(boundCollectionElementInitializer.ImplicitReceiverOpt, addMethod);
            ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundCollectionElementInitializer);
            SyntaxNode syntax = boundCollectionElementInitializer.Syntax;
            ITypeSymbol publicTypeSymbol = boundCollectionElementInitializer.GetPublicTypeSymbol();
            ConstantValue constantValue = boundCollectionElementInitializer.ConstantValue;
            bool wasCompilerGenerated = boundCollectionElementInitializer.WasCompilerGenerated;
            if (IsMethodInvalid(boundCollectionElementInitializer.ResultKind, addMethod))
            {
                return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundCollectionElementInitializer).InvalidNodeChildren), _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
            }
            bool isVirtual = IsCallVirtual(addMethod, boundCollectionElementInitializer.ImplicitReceiverOpt);
            return new InvocationOperation(addMethod.GetPublicSymbol(), instance, isVirtual, arguments, _semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IDynamicMemberReferenceOperation CreateBoundDynamicMemberAccessOperation(BoundDynamicMemberAccess boundDynamicMemberAccess)
        {
            return CreateBoundDynamicMemberAccessOperation(boundDynamicMemberAccess.Receiver, TypeMap.AsTypeSymbols(boundDynamicMemberAccess.TypeArgumentsOpt), boundDynamicMemberAccess.Name, boundDynamicMemberAccess.Syntax, boundDynamicMemberAccess.GetPublicTypeSymbol(), boundDynamicMemberAccess.WasCompilerGenerated);
        }

        private IDynamicMemberReferenceOperation CreateBoundDynamicMemberAccessOperation(BoundExpression? receiver, ImmutableArray<TypeSymbol> typeArgumentsOpt, string memberName, SyntaxNode syntaxNode, ITypeSymbol? type, bool isImplicit)
        {
            ITypeSymbol containingType = null;
            if (receiver != null && receiver!.Kind == BoundKind.TypeExpression)
            {
                containingType = receiver!.GetPublicTypeSymbol();
                receiver = null;
            }
            ImmutableArray<ITypeSymbol> typeArguments = ImmutableArray<ITypeSymbol>.Empty;
            if (!typeArgumentsOpt.IsDefault)
            {
                typeArguments = typeArgumentsOpt.GetPublicSymbols();
            }
            return new DynamicMemberReferenceOperation(Create(receiver), memberName, typeArguments, containingType, _semanticModel, syntaxNode, type, isImplicit);
        }

        private IDynamicInvocationOperation CreateBoundDynamicCollectionElementInitializerOperation(BoundDynamicCollectionElementInitializer boundCollectionElementInitializer)
        {
            return new DynamicInvocationOperation(CreateBoundDynamicInvocationExpressionReceiver(boundCollectionElementInitializer.Expression), CreateFromArray<BoundExpression, IOperation>(boundCollectionElementInitializer.Arguments), syntax: boundCollectionElementInitializer.Syntax, type: boundCollectionElementInitializer.GetPublicTypeSymbol(), isImplicit: boundCollectionElementInitializer.WasCompilerGenerated, argumentNames: ImmutableArray<string>.Empty, argumentRefKinds: ImmutableArray<RefKind>.Empty, semanticModel: _semanticModel);
        }

        private IOperation CreateUnboundLambdaOperation(UnboundLambda unboundLambda)
        {
            BoundLambda boundNode = unboundLambda.BindForErrorRecovery();
            return Create(boundNode);
        }

        private IAnonymousFunctionOperation CreateBoundLambdaOperation(BoundLambda boundLambda)
        {
            return new AnonymousFunctionOperation(boundLambda.Symbol.GetPublicSymbol(), (IBlockOperation)Create(boundLambda.Body), syntax: boundLambda.Syntax, isImplicit: boundLambda.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ILocalFunctionOperation CreateBoundLocalFunctionStatementOperation(BoundLocalFunctionStatement boundLocalFunctionStatement)
        {
            IBlockOperation? body = (IBlockOperation?)Create(boundLocalFunctionStatement.Body);
            IBlockOperation? ignoredBody = boundLocalFunctionStatement is { BlockBody: { }, ExpressionBody: { } exprBody }
                ? (IBlockOperation?)Create(exprBody)
                : null;
            IMethodSymbol symbol = boundLocalFunctionStatement.Symbol.GetPublicSymbol();
            SyntaxNode syntax = boundLocalFunctionStatement.Syntax;
            bool isImplicit = boundLocalFunctionStatement.WasCompilerGenerated;
            return new LocalFunctionOperation(symbol, body, ignoredBody, _semanticModel, syntax, isImplicit);
        }

        private IOperation CreateBoundConversionOperation(BoundConversion boundConversion)
        {
            bool isImplicit = boundConversion.WasCompilerGenerated || !boundConversion.ExplicitCastInCode;
            BoundExpression operand = boundConversion.Operand;
            if (boundConversion.ConversionKind == ConversionKind.MethodGroup)
            {
                SyntaxNode syntax = boundConversion.Syntax;
                ITypeSymbol publicTypeSymbol = boundConversion.GetPublicTypeSymbol();
                _ = boundConversion.ConstantValue;
                if (boundConversion.Type is FunctionPointerTypeSymbol)
                {
                    return new AddressOfOperation(CreateBoundMethodGroupSingleMethodOperation((BoundMethodGroup)boundConversion.Operand, boundConversion.SymbolOpt, suppressVirtualCalls: false), _semanticModel, syntax, publicTypeSymbol, boundConversion.WasCompilerGenerated);
                }
                return new DelegateCreationOperation(CreateDelegateTargetOperation(boundConversion), _semanticModel, syntax, publicTypeSymbol, isImplicit);
            }
            SyntaxNode syntax2 = boundConversion.Syntax;
            if (syntax2.IsMissing)
            {
                return Create(operand);
            }
            BoundConversion boundConversion2 = boundConversion;
            Conversion conversion = boundConversion.Conversion;
            if (operand.Syntax == boundConversion.Syntax)
            {
                if (operand.Kind == BoundKind.ConvertedTupleLiteral && TypeSymbol.Equals(operand.Type, boundConversion.Type, TypeCompareKind.ConsiderEverything))
                {
                    return Create(operand);
                }
                isImplicit = true;
            }
            if (boundConversion.ExplicitCastInCode && conversion.IsIdentity && operand.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion3 = (BoundConversion)operand;
                BoundExpression operand2 = boundConversion3.Operand;
                if (boundConversion3.Syntax == operand2.Syntax && boundConversion3.ExplicitCastInCode && operand2.Kind == BoundKind.ConvertedTupleLiteral && !TypeSymbol.Equals(boundConversion3.Type, operand2.Type, TypeCompareKind.ConsiderEverything))
                {
                    conversion = boundConversion3.Conversion;
                    boundConversion2 = boundConversion3;
                }
            }
            ITypeSymbol publicTypeSymbol2 = boundConversion.GetPublicTypeSymbol();
            ConstantValue constantValue = boundConversion.ConstantValue;
            if ((operand.Kind == BoundKind.Lambda || operand.Kind == BoundKind.UnboundLambda || operand.Kind == BoundKind.MethodGroup) && boundConversion.Type.IsDelegateType())
            {
                return new DelegateCreationOperation(CreateDelegateTargetOperation(boundConversion2), _semanticModel, syntax2, publicTypeSymbol2, isImplicit);
            }
            bool isTryCast = false;
            bool isChecked = conversion.IsNumeric && boundConversion.Checked;
            return new ConversionOperation(Create(boundConversion2.Operand), conversion, isTryCast, isChecked, _semanticModel, syntax2, publicTypeSymbol2, constantValue, isImplicit);
        }

        private IConversionOperation CreateBoundAsOperatorOperation(BoundAsOperator boundAsOperator)
        {
            IOperation? operand = Create(boundAsOperator.Operand);
            SyntaxNode syntax = boundAsOperator.Syntax;
            Conversion conversion = boundAsOperator.Conversion;
            return new ConversionOperation(isTryCast: true, isChecked: false, type: boundAsOperator.GetPublicTypeSymbol(), isImplicit: boundAsOperator.WasCompilerGenerated, operand: operand, conversion: conversion, semanticModel: _semanticModel, syntax: syntax, constantValue: null);
        }

        private IDelegateCreationOperation CreateBoundDelegateCreationExpressionOperation(BoundDelegateCreationExpression boundDelegateCreationExpression)
        {
            return new DelegateCreationOperation(CreateDelegateTargetOperation(boundDelegateCreationExpression), syntax: boundDelegateCreationExpression.Syntax, type: boundDelegateCreationExpression.GetPublicTypeSymbol(), isImplicit: boundDelegateCreationExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IMethodReferenceOperation CreateBoundMethodGroupSingleMethodOperation(BoundMethodGroup boundMethodGroup, MethodSymbol methodSymbol, bool suppressVirtualCalls)
        {
            bool isVirtual = (methodSymbol.IsAbstract || methodSymbol.IsOverride || methodSymbol.IsVirtual) && !suppressVirtualCalls;
            IOperation instance = CreateReceiverOperation(boundMethodGroup.ReceiverOpt, methodSymbol);
            SyntaxNode syntax = boundMethodGroup.Syntax;
            ITypeSymbol type = null;
            _ = boundMethodGroup.WasCompilerGenerated;
            return new MethodReferenceOperation(methodSymbol.GetPublicSymbol(), isVirtual, instance, _semanticModel, syntax, type, boundMethodGroup.WasCompilerGenerated);
        }

        private IIsTypeOperation CreateBoundIsOperatorOperation(BoundIsOperator boundIsOperator)
        {
            return new IsTypeOperation(Create(boundIsOperator.Operand), boundIsOperator.TargetType.GetPublicTypeSymbol(), syntax: boundIsOperator.Syntax, type: boundIsOperator.GetPublicTypeSymbol(), isNegated: false, isImplicit: boundIsOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ISizeOfOperation CreateBoundSizeOfOperatorOperation(BoundSizeOfOperator boundSizeOfOperator)
        {
            return new SizeOfOperation(boundSizeOfOperator.SourceType.GetPublicTypeSymbol(), syntax: boundSizeOfOperator.Syntax, type: boundSizeOfOperator.GetPublicTypeSymbol(), constantValue: boundSizeOfOperator.ConstantValue, isImplicit: boundSizeOfOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ITypeOfOperation CreateBoundTypeOfOperatorOperation(BoundTypeOfOperator boundTypeOfOperator)
        {
            return new TypeOfOperation(boundTypeOfOperator.SourceType.GetPublicTypeSymbol(), syntax: boundTypeOfOperator.Syntax, type: boundTypeOfOperator.GetPublicTypeSymbol(), isImplicit: boundTypeOfOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IArrayCreationOperation CreateBoundArrayCreationOperation(BoundArrayCreation boundArrayCreation)
        {
            ImmutableArray<IOperation> dimensionSizes = CreateFromArray<BoundExpression, IOperation>(boundArrayCreation.Bounds);
            IArrayInitializerOperation initializer = (IArrayInitializerOperation)Create(boundArrayCreation.InitializerOpt);
            SyntaxNode syntax = boundArrayCreation.Syntax;
            return new ArrayCreationOperation(type: boundArrayCreation.GetPublicTypeSymbol(), isImplicit: boundArrayCreation.WasCompilerGenerated || (boundArrayCreation.InitializerOpt?.Syntax == syntax && !boundArrayCreation.InitializerOpt!.WasCompilerGenerated), dimensionSizes: dimensionSizes, initializer: initializer, semanticModel: _semanticModel, syntax: syntax);
        }

        private IArrayInitializerOperation CreateBoundArrayInitializationOperation(BoundArrayInitialization boundArrayInitialization)
        {
            return new ArrayInitializerOperation(CreateFromArray<BoundExpression, IOperation>(boundArrayInitialization.Initializers), syntax: boundArrayInitialization.Syntax, isImplicit: boundArrayInitialization.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IDefaultValueOperation CreateBoundDefaultLiteralOperation(BoundDefaultLiteral boundDefaultLiteral)
        {
            SyntaxNode syntax = boundDefaultLiteral.Syntax;
            ConstantValue constantValue = boundDefaultLiteral.ConstantValue;
            bool wasCompilerGenerated = boundDefaultLiteral.WasCompilerGenerated;
            return new DefaultValueOperation(_semanticModel, syntax, null, constantValue, wasCompilerGenerated);
        }

        private IDefaultValueOperation CreateBoundDefaultExpressionOperation(BoundDefaultExpression boundDefaultExpression)
        {
            SyntaxNode syntax = boundDefaultExpression.Syntax;
            ITypeSymbol publicTypeSymbol = boundDefaultExpression.GetPublicTypeSymbol();
            ConstantValue constantValue = boundDefaultExpression.ConstantValue;
            bool wasCompilerGenerated = boundDefaultExpression.WasCompilerGenerated;
            return new DefaultValueOperation(_semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
        }

        private IInstanceReferenceOperation CreateBoundBaseReferenceOperation(BoundBaseReference boundBaseReference)
        {
            return new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, syntax: boundBaseReference.Syntax, type: boundBaseReference.GetPublicTypeSymbol(), isImplicit: boundBaseReference.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IInstanceReferenceOperation CreateBoundThisReferenceOperation(BoundThisReference boundThisReference)
        {
            return new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, syntax: boundThisReference.Syntax, type: boundThisReference.GetPublicTypeSymbol(), isImplicit: boundThisReference.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundAssignmentOperatorOrMemberInitializerOperation(BoundAssignmentOperator boundAssignmentOperator)
        {
            if (!IsMemberInitializer(boundAssignmentOperator))
            {
                return CreateBoundAssignmentOperatorOperation(boundAssignmentOperator);
            }
            return CreateBoundMemberInitializerOperation(boundAssignmentOperator);
        }

        private static bool IsMemberInitializer(BoundAssignmentOperator boundAssignmentOperator)
        {
            BoundExpression right = boundAssignmentOperator.Right;
            if (right == null || right.Kind != BoundKind.ObjectInitializerExpression)
            {
                BoundExpression right2 = boundAssignmentOperator.Right;
                if (right2 == null)
                {
                    return false;
                }
                return right2.Kind == BoundKind.CollectionInitializerExpression;
            }
            return true;
        }

        private ISimpleAssignmentOperation CreateBoundAssignmentOperatorOperation(BoundAssignmentOperator boundAssignmentOperator)
        {
            IOperation target = Create(boundAssignmentOperator.Left);
            IOperation value = Create(boundAssignmentOperator.Right);
            return new SimpleAssignmentOperation(boundAssignmentOperator.IsRef, syntax: boundAssignmentOperator.Syntax, type: boundAssignmentOperator.GetPublicTypeSymbol(), constantValue: boundAssignmentOperator.ConstantValue, isImplicit: boundAssignmentOperator.WasCompilerGenerated, target: target, value: value, semanticModel: _semanticModel);
        }

        private IMemberInitializerOperation CreateBoundMemberInitializerOperation(BoundAssignmentOperator boundAssignmentOperator)
        {
            return new MemberInitializerOperation(CreateMemberInitializerInitializedMember(boundAssignmentOperator.Left), (IObjectOrCollectionInitializerOperation)Create(boundAssignmentOperator.Right), syntax: boundAssignmentOperator.Syntax, type: boundAssignmentOperator.GetPublicTypeSymbol(), isImplicit: boundAssignmentOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ICompoundAssignmentOperation CreateBoundCompoundAssignmentOperatorOperation(BoundCompoundAssignmentOperator boundCompoundAssignmentOperator)
        {
            IOperation target = Create(boundCompoundAssignmentOperator.Left);
            IOperation value = Create(boundCompoundAssignmentOperator.Right);
            BinaryOperatorKind operatorKind = Helper.DeriveBinaryOperatorKind(boundCompoundAssignmentOperator.Operator.Kind);
            Conversion leftConversion = boundCompoundAssignmentOperator.LeftConversion;
            Conversion finalConversion = boundCompoundAssignmentOperator.FinalConversion;
            return new CompoundAssignmentOperation(isLifted: boundCompoundAssignmentOperator.Operator.Kind.IsLifted(), isChecked: boundCompoundAssignmentOperator.Operator.Kind.IsChecked(), operatorMethod: boundCompoundAssignmentOperator.Operator.Method.GetPublicSymbol(), syntax: boundCompoundAssignmentOperator.Syntax, type: boundCompoundAssignmentOperator.GetPublicTypeSymbol(), isImplicit: boundCompoundAssignmentOperator.WasCompilerGenerated, inConversion: leftConversion, outConversion: finalConversion, operatorKind: operatorKind, target: target, value: value, semanticModel: _semanticModel);
        }

        private IIncrementOrDecrementOperation CreateBoundIncrementOperatorOperation(BoundIncrementOperator boundIncrementOperator)
        {
            OperationKind kind = (Helper.IsDecrement(boundIncrementOperator.OperatorKind) ? OperationKind.Decrement : OperationKind.Increment);
            return new IncrementOrDecrementOperation(Helper.IsPostfixIncrementOrDecrement(boundIncrementOperator.OperatorKind), boundIncrementOperator.OperatorKind.IsLifted(), boundIncrementOperator.OperatorKind.IsChecked(), Create(boundIncrementOperator.Operand), boundIncrementOperator.MethodOpt.GetPublicSymbol(), syntax: boundIncrementOperator.Syntax, type: boundIncrementOperator.GetPublicTypeSymbol(), isImplicit: boundIncrementOperator.WasCompilerGenerated, kind: kind, semanticModel: _semanticModel);
        }

        private IInvalidOperation CreateBoundBadExpressionOperation(BoundBadExpression boundBadExpression)
        {
            BoundBadExpression boundBadExpression2 = boundBadExpression;
            SyntaxNode syntax = boundBadExpression2.Syntax;
            ITypeSymbol type = (syntax.IsMissing ? null : boundBadExpression2.GetPublicTypeSymbol());
            bool isImplicit = boundBadExpression2.WasCompilerGenerated || boundBadExpression2.ChildBoundNodes.Any((BoundExpression e) => e?.Syntax == boundBadExpression2.Syntax);
            return new InvalidOperation(CreateFromArray<BoundExpression, IOperation>(boundBadExpression2.ChildBoundNodes), _semanticModel, syntax, type, null, isImplicit);
        }

        private ITypeParameterObjectCreationOperation CreateBoundNewTOperation(BoundNewT boundNewT)
        {
            return new TypeParameterObjectCreationOperation((IObjectOrCollectionInitializerOperation)Create(boundNewT.InitializerExpressionOpt), syntax: boundNewT.Syntax, type: boundNewT.GetPublicTypeSymbol(), isImplicit: boundNewT.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private INoPiaObjectCreationOperation CreateNoPiaObjectCreationExpressionOperation(BoundNoPiaObjectCreationExpression creation)
        {
            return new NoPiaObjectCreationOperation((IObjectOrCollectionInitializerOperation)Create(creation.InitializerExpressionOpt), syntax: creation.Syntax, type: creation.GetPublicTypeSymbol(), isImplicit: creation.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IUnaryOperation CreateBoundUnaryOperatorOperation(BoundUnaryOperator boundUnaryOperator)
        {
            return new UnaryOperation(Helper.DeriveUnaryOperatorKind(boundUnaryOperator.OperatorKind), Create(boundUnaryOperator.Operand), operatorMethod: boundUnaryOperator.MethodOpt.GetPublicSymbol(), syntax: boundUnaryOperator.Syntax, type: boundUnaryOperator.GetPublicTypeSymbol(), constantValue: boundUnaryOperator.ConstantValue, isLifted: boundUnaryOperator.OperatorKind.IsLifted(), isChecked: boundUnaryOperator.OperatorKind.IsChecked(), isImplicit: boundUnaryOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundBinaryOperatorBase(BoundBinaryOperatorBase boundBinaryOperatorBase)
        {
            ArrayBuilder<BoundBinaryOperatorBase> instance = ArrayBuilder<BoundBinaryOperatorBase>.GetInstance();
            BoundBinaryOperatorBase result = boundBinaryOperatorBase;
            do
            {
                instance.Push(result);
                result = result.Left as BoundBinaryOperatorBase;
            }
            while (result != null);
            IOperation operation = null;
            while (instance.TryPop(out result))
            {
                operation = operation ?? Create(result.Left);
                IOperation right2 = Create(result.Right);
                IBinaryOperation binaryOperation;
                if (!(result is BoundBinaryOperator boundBinaryOperator2))
                {
                    if (!(result is BoundUserDefinedConditionalLogicalOperator boundBinaryOperator3))
                    {
                        if (result != null)
                        {
                            BoundKind kind = result.Kind;
                            throw ExceptionUtilities.UnexpectedValue(kind);
                        }
                        throw new InvalidOperationException();
                    }
                    binaryOperation = createBoundUserDefinedConditionalLogicalOperator(boundBinaryOperator3, operation, right2);
                }
                else
                {
                    binaryOperation = createBoundBinaryOperatorOperation(boundBinaryOperator2, operation, right2);
                }
                operation = binaryOperation;
            }
            instance.Free();
            return operation;
            IBinaryOperation createBoundBinaryOperatorOperation(BoundBinaryOperator boundBinaryOperator, IOperation left, IOperation right)
            {
                BinaryOperatorKind binaryOperatorKind = Helper.DeriveBinaryOperatorKind(boundBinaryOperator.OperatorKind);
                IMethodSymbol methodSymbol = boundBinaryOperator.MethodOpt.GetPublicSymbol();
                IMethodSymbol unaryOperatorMethod = null;
                if (boundBinaryOperator.Type.IsDynamic() && (binaryOperatorKind == BinaryOperatorKind.ConditionalAnd || binaryOperatorKind == BinaryOperatorKind.ConditionalOr) && methodSymbol != null && methodSymbol.Parameters.Length == 1)
                {
                    unaryOperatorMethod = methodSymbol;
                    methodSymbol = null;
                }
                SyntaxNode syntax = boundBinaryOperator.Syntax;
                ITypeSymbol publicTypeSymbol = boundBinaryOperator.GetPublicTypeSymbol();
                ConstantValue constantValue = boundBinaryOperator.ConstantValue;
                bool isLifted = boundBinaryOperator.OperatorKind.IsLifted();
                bool isChecked = boundBinaryOperator.OperatorKind.IsChecked();
                bool isCompareText = false;
                bool wasCompilerGenerated = boundBinaryOperator.WasCompilerGenerated;
                return new BinaryOperation(binaryOperatorKind, left, right, isLifted, isChecked, isCompareText, methodSymbol, unaryOperatorMethod, _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
            }
            IBinaryOperation createBoundUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator boundBinaryOperator, IOperation left, IOperation right)
            {
                return new BinaryOperation(Helper.DeriveBinaryOperatorKind(boundBinaryOperator.OperatorKind), operatorMethod: boundBinaryOperator.LogicalOperator.GetPublicSymbol(), unaryOperatorMethod: (boundBinaryOperator.OperatorKind.Operator() == Microsoft.CodeAnalysis.CSharp.BinaryOperatorKind.And) ? boundBinaryOperator.FalseOperator.GetPublicSymbol() : boundBinaryOperator.TrueOperator.GetPublicSymbol(), syntax: boundBinaryOperator.Syntax, type: boundBinaryOperator.GetPublicTypeSymbol(), constantValue: boundBinaryOperator.ConstantValue, isLifted: boundBinaryOperator.OperatorKind.IsLifted(), isChecked: boundBinaryOperator.OperatorKind.IsChecked(), isCompareText: false, isImplicit: boundBinaryOperator.WasCompilerGenerated, leftOperand: left, rightOperand: right, semanticModel: _semanticModel);
            }
        }

        private ITupleBinaryOperation CreateBoundTupleBinaryOperatorOperation(BoundTupleBinaryOperator boundTupleBinaryOperator)
        {
            IOperation leftOperand = Create(boundTupleBinaryOperator.Left);
            IOperation rightOperand = Create(boundTupleBinaryOperator.Right);
            return new TupleBinaryOperation(Helper.DeriveBinaryOperatorKind(boundTupleBinaryOperator.OperatorKind), syntax: boundTupleBinaryOperator.Syntax, type: boundTupleBinaryOperator.GetPublicTypeSymbol(), isImplicit: boundTupleBinaryOperator.WasCompilerGenerated, leftOperand: leftOperand, rightOperand: rightOperand, semanticModel: _semanticModel);
        }

        private IConditionalOperation CreateBoundConditionalOperatorOperation(BoundConditionalOperator boundConditionalOperator)
        {
            return new ConditionalOperation(Create(boundConditionalOperator.Condition), Create(boundConditionalOperator.Consequence), Create(boundConditionalOperator.Alternative), boundConditionalOperator.IsRef, syntax: boundConditionalOperator.Syntax, type: boundConditionalOperator.GetPublicTypeSymbol(), constantValue: boundConditionalOperator.ConstantValue, isImplicit: boundConditionalOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ICoalesceOperation CreateBoundNullCoalescingOperatorOperation(BoundNullCoalescingOperator boundNullCoalescingOperator)
        {
            IOperation? value = Create(boundNullCoalescingOperator.LeftOperand);
            IOperation whenNull = Create(boundNullCoalescingOperator.RightOperand);
            SyntaxNode syntax = boundNullCoalescingOperator.Syntax;
            ITypeSymbol publicTypeSymbol = boundNullCoalescingOperator.GetPublicTypeSymbol();
            ConstantValue constantValue = boundNullCoalescingOperator.ConstantValue;
            bool wasCompilerGenerated = boundNullCoalescingOperator.WasCompilerGenerated;
            Conversion conversion = boundNullCoalescingOperator.LeftConversion;
            if (conversion.Exists && !conversion.IsIdentity && boundNullCoalescingOperator.Type.Equals(boundNullCoalescingOperator.LeftOperand.Type?.StrippedType(), TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                conversion = Conversion.Identity;
            }
            return new CoalesceOperation(value, whenNull, conversion, _semanticModel, syntax, publicTypeSymbol, constantValue, wasCompilerGenerated);
        }

        private IOperation CreateBoundNullCoalescingAssignmentOperatorOperation(BoundNullCoalescingAssignmentOperator boundNode)
        {
            return new CoalesceAssignmentOperation(Create(boundNode.LeftOperand), Create(boundNode.RightOperand), syntax: boundNode.Syntax, type: boundNode.GetPublicTypeSymbol(), isImplicit: boundNode.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IAwaitOperation CreateBoundAwaitExpressionOperation(BoundAwaitExpression boundAwaitExpression)
        {
            return new AwaitOperation(Create(boundAwaitExpression.Expression), syntax: boundAwaitExpression.Syntax, type: boundAwaitExpression.GetPublicTypeSymbol(), isImplicit: boundAwaitExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IArrayElementReferenceOperation CreateBoundArrayAccessOperation(BoundArrayAccess boundArrayAccess)
        {
            return new ArrayElementReferenceOperation(Create(boundArrayAccess.Expression), CreateFromArray<BoundExpression, IOperation>(boundArrayAccess.Indices), syntax: boundArrayAccess.Syntax, type: boundArrayAccess.GetPublicTypeSymbol(), isImplicit: boundArrayAccess.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private INameOfOperation CreateBoundNameOfOperatorOperation(BoundNameOfOperator boundNameOfOperator)
        {
            return new NameOfOperation(Create(boundNameOfOperator.Argument), syntax: boundNameOfOperator.Syntax, type: boundNameOfOperator.GetPublicTypeSymbol(), constantValue: boundNameOfOperator.ConstantValue, isImplicit: boundNameOfOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IThrowOperation CreateBoundThrowExpressionOperation(BoundThrowExpression boundThrowExpression)
        {
            return new ThrowOperation(Create(boundThrowExpression.Expression), syntax: boundThrowExpression.Syntax, type: boundThrowExpression.GetPublicTypeSymbol(), isImplicit: boundThrowExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IAddressOfOperation CreateBoundAddressOfOperatorOperation(BoundAddressOfOperator boundAddressOfOperator)
        {
            return new AddressOfOperation(Create(boundAddressOfOperator.Operand), syntax: boundAddressOfOperator.Syntax, type: boundAddressOfOperator.GetPublicTypeSymbol(), isImplicit: boundAddressOfOperator.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IInstanceReferenceOperation CreateBoundImplicitReceiverOperation(BoundImplicitReceiver boundImplicitReceiver)
        {
            return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, syntax: boundImplicitReceiver.Syntax, type: boundImplicitReceiver.GetPublicTypeSymbol(), isImplicit: boundImplicitReceiver.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IConditionalAccessOperation CreateBoundConditionalAccessOperation(BoundConditionalAccess boundConditionalAccess)
        {
            return new ConditionalAccessOperation(Create(boundConditionalAccess.Receiver), Create(boundConditionalAccess.AccessExpression), syntax: boundConditionalAccess.Syntax, type: boundConditionalAccess.GetPublicTypeSymbol(), isImplicit: boundConditionalAccess.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IConditionalAccessInstanceOperation CreateBoundConditionalReceiverOperation(BoundConditionalReceiver boundConditionalReceiver)
        {
            SyntaxNode syntax = boundConditionalReceiver.Syntax;
            ITypeSymbol publicTypeSymbol = boundConditionalReceiver.GetPublicTypeSymbol();
            bool wasCompilerGenerated = boundConditionalReceiver.WasCompilerGenerated;
            return new ConditionalAccessInstanceOperation(_semanticModel, syntax, publicTypeSymbol, wasCompilerGenerated);
        }

        private IFieldInitializerOperation CreateBoundFieldEqualsValueOperation(BoundFieldEqualsValue boundFieldEqualsValue)
        {
            return new FieldInitializerOperation(ImmutableArray.Create(boundFieldEqualsValue.Field.GetPublicSymbol()), value: Create(boundFieldEqualsValue.Value), syntax: boundFieldEqualsValue.Syntax, isImplicit: boundFieldEqualsValue.WasCompilerGenerated, locals: boundFieldEqualsValue.Locals.GetPublicSymbols(), semanticModel: _semanticModel);
        }

        private IPropertyInitializerOperation CreateBoundPropertyEqualsValueOperation(BoundPropertyEqualsValue boundPropertyEqualsValue)
        {
            return new PropertyInitializerOperation(ImmutableArray.Create(boundPropertyEqualsValue.Property.GetPublicSymbol()), value: Create(boundPropertyEqualsValue.Value), syntax: boundPropertyEqualsValue.Syntax, isImplicit: boundPropertyEqualsValue.WasCompilerGenerated, locals: boundPropertyEqualsValue.Locals.GetPublicSymbols(), semanticModel: _semanticModel);
        }

        private IParameterInitializerOperation CreateBoundParameterEqualsValueOperation(BoundParameterEqualsValue boundParameterEqualsValue)
        {
            return new ParameterInitializerOperation(boundParameterEqualsValue.Parameter.GetPublicSymbol(), value: Create(boundParameterEqualsValue.Value), syntax: boundParameterEqualsValue.Syntax, isImplicit: boundParameterEqualsValue.WasCompilerGenerated, locals: boundParameterEqualsValue.Locals.GetPublicSymbols(), semanticModel: _semanticModel);
        }

        private IBlockOperation CreateBoundBlockOperation(BoundBlock boundBlock)
        {
            return new BlockOperation(CreateFromArray<BoundStatement, IOperation>(boundBlock.Statements), boundBlock.Locals.GetPublicSymbols(), syntax: boundBlock.Syntax, isImplicit: boundBlock.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IBranchOperation CreateBoundContinueStatementOperation(BoundContinueStatement boundContinueStatement)
        {
            return new BranchOperation(boundContinueStatement.Label.GetPublicSymbol(), BranchKind.Continue, syntax: boundContinueStatement.Syntax, isImplicit: boundContinueStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IBranchOperation CreateBoundBreakStatementOperation(BoundBreakStatement boundBreakStatement)
        {
            return new BranchOperation(boundBreakStatement.Label.GetPublicSymbol(), BranchKind.Break, syntax: boundBreakStatement.Syntax, isImplicit: boundBreakStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IReturnOperation CreateBoundYieldBreakStatementOperation(BoundYieldBreakStatement boundYieldBreakStatement)
        {
            return new ReturnOperation(null, syntax: boundYieldBreakStatement.Syntax, isImplicit: boundYieldBreakStatement.WasCompilerGenerated, kind: OperationKind.YieldBreak, semanticModel: _semanticModel);
        }

        private IBranchOperation CreateBoundGotoStatementOperation(BoundGotoStatement boundGotoStatement)
        {
            return new BranchOperation(boundGotoStatement.Label.GetPublicSymbol(), BranchKind.GoTo, syntax: boundGotoStatement.Syntax, isImplicit: boundGotoStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IEmptyOperation CreateBoundNoOpStatementOperation(BoundNoOpStatement boundNoOpStatement)
        {
            SyntaxNode syntax = boundNoOpStatement.Syntax;
            bool wasCompilerGenerated = boundNoOpStatement.WasCompilerGenerated;
            return new EmptyOperation(_semanticModel, syntax, wasCompilerGenerated);
        }

        private IConditionalOperation CreateBoundIfStatementOperation(BoundIfStatement boundIfStatement)
        {
            return new ConditionalOperation(Create(boundIfStatement.Condition), Create(boundIfStatement.Consequence), Create(boundIfStatement.AlternativeOpt), isRef: false, syntax: boundIfStatement.Syntax, type: null, constantValue: null, isImplicit: boundIfStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IWhileLoopOperation CreateBoundWhileStatementOperation(BoundWhileStatement boundWhileStatement)
        {
            return new WhileLoopOperation(Create(boundWhileStatement.Condition), body: Create(boundWhileStatement.Body), locals: boundWhileStatement.Locals.GetPublicSymbols(), continueLabel: boundWhileStatement.ContinueLabel.GetPublicSymbol(), exitLabel: boundWhileStatement.BreakLabel.GetPublicSymbol(), conditionIsTop: true, conditionIsUntil: false, syntax: boundWhileStatement.Syntax, isImplicit: boundWhileStatement.WasCompilerGenerated, ignoredCondition: null, semanticModel: _semanticModel);
        }

        private IWhileLoopOperation CreateBoundDoStatementOperation(BoundDoStatement boundDoStatement)
        {
            return new WhileLoopOperation(Create(boundDoStatement.Condition), body: Create(boundDoStatement.Body), continueLabel: boundDoStatement.ContinueLabel.GetPublicSymbol(), exitLabel: boundDoStatement.BreakLabel.GetPublicSymbol(), conditionIsTop: false, conditionIsUntil: false, locals: boundDoStatement.Locals.GetPublicSymbols(), syntax: boundDoStatement.Syntax, isImplicit: boundDoStatement.WasCompilerGenerated, ignoredCondition: null, semanticModel: _semanticModel);
        }

        private IForLoopOperation CreateBoundForStatementOperation(BoundForStatement boundForStatement)
        {
            return new ForLoopOperation(CreateFromArray<BoundStatement, IOperation>(ToStatements(boundForStatement.Initializer)), condition: Create(boundForStatement.Condition), atLoopBottom: CreateFromArray<BoundStatement, IOperation>(ToStatements(boundForStatement.Increment)), body: Create(boundForStatement.Body), locals: boundForStatement.OuterLocals.GetPublicSymbols(), conditionLocals: boundForStatement.InnerLocals.GetPublicSymbols(), continueLabel: boundForStatement.ContinueLabel.GetPublicSymbol(), exitLabel: boundForStatement.BreakLabel.GetPublicSymbol(), syntax: boundForStatement.Syntax, isImplicit: boundForStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        internal ForEachLoopOperationInfo? GetForEachLoopOperatorInfo(BoundForEachStatement boundForEachStatement)
        {
            ForEachEnumeratorInfo enumeratorInfoOpt = boundForEachStatement.EnumeratorInfoOpt;
            ITypeSymbol? publicSymbol;
            IMethodSymbol? publicSymbol2;
            IPropertySymbol? publicSymbol3;
            IMethodSymbol? publicSymbol4;
            bool isAsync;
            bool needsDisposal;
            bool knownToImplementIDisposable;
            IMethodSymbol? patternDisposeMethod;
            object currentConversion;
            object elementConversion;
            ImmutableArray<IArgumentOperation> getEnumeratorArguments;
            if (enumeratorInfoOpt != null)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                CSharpCompilation cSharpCompilation = (CSharpCompilation)_semanticModel.Compilation;
                NamedTypeSymbol destination = (enumeratorInfoOpt.IsAsync ? cSharpCompilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable) : cSharpCompilation.GetSpecialType(SpecialType.System_IDisposable));
                publicSymbol = enumeratorInfoOpt.ElementType.GetPublicSymbol();
                publicSymbol2 = enumeratorInfoOpt.GetEnumeratorInfo.Method.GetPublicSymbol();
                publicSymbol3 = ((PropertySymbol)enumeratorInfoOpt.CurrentPropertyGetter.AssociatedSymbol).GetPublicSymbol();
                publicSymbol4 = enumeratorInfoOpt.MoveNextInfo.Method.GetPublicSymbol();
                isAsync = enumeratorInfoOpt.IsAsync;
                needsDisposal = enumeratorInfoOpt.NeedsDisposal;
                knownToImplementIDisposable = enumeratorInfoOpt.NeedsDisposal && cSharpCompilation.Conversions.ClassifyImplicitConversionFromType(enumeratorInfoOpt.GetEnumeratorInfo.Method.ReturnType, destination, ref useSiteInfo).IsImplicit;
                patternDisposeMethod = enumeratorInfoOpt.PatternDisposeInfo?.Method.GetPublicSymbol();
                currentConversion = enumeratorInfoOpt.CurrentConversion;
                elementConversion = boundForEachStatement.ElementConversion;
                MethodArgumentInfo getEnumeratorInfo = enumeratorInfoOpt.GetEnumeratorInfo;
                if ((object)getEnumeratorInfo != null)
                {
                    MethodSymbol method = getEnumeratorInfo.Method;
                    if ((object)method != null && method.IsExtensionMethod)
                    {
                        getEnumeratorArguments = Operation.SetParentOperation(DeriveArguments(getEnumeratorInfo.Method, getEnumeratorInfo.Arguments, default(ImmutableArray<int>), getEnumeratorInfo.DefaultArguments, getEnumeratorInfo.Expanded, boundForEachStatement.Expression.Syntax, invokedAsExtensionMethod: true), null);
                        goto IL_015b;
                    }
                }
                getEnumeratorArguments = default(ImmutableArray<IArgumentOperation>);
                goto IL_015b;
            }
            return null;
        IL_015b:
            ImmutableArray<IArgumentOperation> disposeArguments = (((object)enumeratorInfoOpt.PatternDisposeInfo != null) ? CreateDisposeArguments(enumeratorInfoOpt.PatternDisposeInfo, boundForEachStatement.Syntax) : default(ImmutableArray<IArgumentOperation>));
            return new ForEachLoopOperationInfo(publicSymbol, publicSymbol2, publicSymbol3, publicSymbol4, isAsync, needsDisposal, knownToImplementIDisposable, patternDisposeMethod, (IConvertibleConversion)currentConversion, (IConvertibleConversion)elementConversion, getEnumeratorArguments, default(ImmutableArray<IArgumentOperation>), default(ImmutableArray<IArgumentOperation>), disposeArguments);
        }

        internal IOperation CreateBoundForEachStatementLoopControlVariable(BoundForEachStatement boundForEachStatement)
        {
            if (boundForEachStatement.DeconstructionOpt != null)
            {
                return Create(boundForEachStatement.DeconstructionOpt!.DeconstructionAssignment.Left);
            }
            if (boundForEachStatement.IterationErrorExpressionOpt != null)
            {
                return Create(boundForEachStatement.IterationErrorExpressionOpt);
            }
            LocalSymbol symbol = boundForEachStatement.IterationVariables[0];
            return new VariableDeclaratorOperation(syntax: boundForEachStatement.IterationVariableType.Syntax, symbol: symbol.GetPublicSymbol(), initializer: null, ignoredArguments: ImmutableArray<IOperation>.Empty, semanticModel: _semanticModel, isImplicit: false);
        }

        private IForEachLoopOperation CreateBoundForEachStatementOperation(BoundForEachStatement boundForEachStatement)
        {
            IOperation loopControlVariable = CreateBoundForEachStatementLoopControlVariable(boundForEachStatement);
            IOperation collection = Create(boundForEachStatement.Expression);
            ImmutableArray<IOperation> empty = ImmutableArray<IOperation>.Empty;
            IOperation body = Create(boundForEachStatement.Body);
            ForEachLoopOperationInfo forEachLoopOperatorInfo = GetForEachLoopOperatorInfo(boundForEachStatement);
            ImmutableArray<ILocalSymbol> publicSymbols = boundForEachStatement.IterationVariables.GetPublicSymbols();
            ILabelSymbol publicSymbol = boundForEachStatement.ContinueLabel.GetPublicSymbol();
            ILabelSymbol publicSymbol2 = boundForEachStatement.BreakLabel.GetPublicSymbol();
            SyntaxNode syntax = boundForEachStatement.Syntax;
            bool wasCompilerGenerated = boundForEachStatement.WasCompilerGenerated;
            bool isAsynchronous = boundForEachStatement.AwaitOpt != null;
            return new ForEachLoopOperation(loopControlVariable, collection, empty, forEachLoopOperatorInfo, isAsynchronous, body, publicSymbols, publicSymbol, publicSymbol2, _semanticModel, syntax, wasCompilerGenerated);
        }

        private ITryOperation CreateBoundTryStatementOperation(BoundTryStatement boundTryStatement)
        {
            return new TryOperation((IBlockOperation)Create(boundTryStatement.TryBlock), CreateFromArray<BoundCatchBlock, ICatchClauseOperation>(boundTryStatement.CatchBlocks), (IBlockOperation)Create(boundTryStatement.FinallyBlockOpt), syntax: boundTryStatement.Syntax, isImplicit: boundTryStatement.WasCompilerGenerated, exitLabel: null, semanticModel: _semanticModel);
        }

        private ICatchClauseOperation CreateBoundCatchBlockOperation(BoundCatchBlock boundCatchBlock)
        {
            return new CatchClauseOperation(CreateVariableDeclarator((BoundLocal)boundCatchBlock.ExceptionSourceOpt), filter: Create(boundCatchBlock.ExceptionFilterOpt), handler: (IBlockOperation)Create(boundCatchBlock.Body), exceptionType: boundCatchBlock.ExceptionTypeOpt.GetPublicSymbol() ?? _semanticModel.Compilation.ObjectType, locals: boundCatchBlock.Locals.GetPublicSymbols(), syntax: boundCatchBlock.Syntax, isImplicit: boundCatchBlock.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IFixedOperation CreateBoundFixedStatementOperation(BoundFixedStatement boundFixedStatement)
        {
            IVariableDeclarationGroupOperation variables = (IVariableDeclarationGroupOperation)Create(boundFixedStatement.Declarations);
            IOperation body = Create(boundFixedStatement.Body);
            return new FixedOperation(boundFixedStatement.Locals.GetPublicSymbols(), syntax: boundFixedStatement.Syntax, isImplicit: boundFixedStatement.WasCompilerGenerated, variables: variables, body: body, semanticModel: _semanticModel);
        }

        private IUsingOperation CreateBoundUsingStatementOperation(BoundUsingStatement boundUsingStatement)
        {
            return new UsingOperation(Create((BoundNode?)(boundUsingStatement.DeclarationsOpt ?? ((object)boundUsingStatement.ExpressionOpt))), Create(boundUsingStatement.Body), boundUsingStatement.Locals.GetPublicSymbols(), boundUsingStatement.AwaitOpt != null, ((object)boundUsingStatement.PatternDisposeInfoOpt != null) ? new DisposeOperationInfo(boundUsingStatement.PatternDisposeInfoOpt!.Method.GetPublicSymbol(), CreateDisposeArguments(boundUsingStatement.PatternDisposeInfoOpt, boundUsingStatement.Syntax)) : default(DisposeOperationInfo), syntax: boundUsingStatement.Syntax, isImplicit: boundUsingStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IThrowOperation CreateBoundThrowStatementOperation(BoundThrowStatement boundThrowStatement)
        {
            return new ThrowOperation(Create(boundThrowStatement.ExpressionOpt), syntax: boundThrowStatement.Syntax, type: null, isImplicit: boundThrowStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IReturnOperation CreateBoundReturnStatementOperation(BoundReturnStatement boundReturnStatement)
        {
            return new ReturnOperation(Create(boundReturnStatement.ExpressionOpt), syntax: boundReturnStatement.Syntax, isImplicit: boundReturnStatement.WasCompilerGenerated, kind: OperationKind.Return, semanticModel: _semanticModel);
        }

        private IReturnOperation CreateBoundYieldReturnStatementOperation(BoundYieldReturnStatement boundYieldReturnStatement)
        {
            return new ReturnOperation(Create(boundYieldReturnStatement.Expression), syntax: boundYieldReturnStatement.Syntax, isImplicit: boundYieldReturnStatement.WasCompilerGenerated, kind: OperationKind.YieldReturn, semanticModel: _semanticModel);
        }

        private ILockOperation CreateBoundLockStatementOperation(BoundLockStatement boundLockStatement)
        {
            ILocalSymbol lockTakenSymbol = ((_semanticModel.Compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Enter2) == null) ? null : new SynthesizedLocal((_semanticModel.GetEnclosingSymbol(boundLockStatement.Syntax.SpanStart) as IMethodSymbol).GetSymbol(), TypeWithAnnotations.Create(((CSharpCompilation)_semanticModel.Compilation).GetSpecialType(SpecialType.System_Boolean)), SynthesizedLocalKind.LockTaken, boundLockStatement.Argument.Syntax).GetPublicSymbol());
            return new LockOperation(Create(boundLockStatement.Argument), Create(boundLockStatement.Body), syntax: boundLockStatement.Syntax, isImplicit: boundLockStatement.WasCompilerGenerated, lockTakenSymbol: lockTakenSymbol, semanticModel: _semanticModel);
        }

        private IInvalidOperation CreateBoundBadStatementOperation(BoundBadStatement boundBadStatement)
        {
            BoundBadStatement boundBadStatement2 = boundBadStatement;
            SyntaxNode syntax = boundBadStatement2.Syntax;
            bool isImplicit = boundBadStatement2.WasCompilerGenerated || boundBadStatement2.ChildBoundNodes.Any((BoundNode e) => e?.Syntax == boundBadStatement2.Syntax);
            return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(boundBadStatement2.ChildBoundNodes), _semanticModel, syntax, null, null, isImplicit);
        }

        private IOperation CreateBoundLocalDeclarationOperation(BoundLocalDeclaration boundLocalDeclaration)
        {
            SyntaxNode syntax = boundLocalDeclaration.Syntax;
            SyntaxNode syntaxNode;
            SyntaxNode syntaxNode2;
            switch (syntax.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    syntaxNode = ((LocalDeclarationStatementSyntax)(syntaxNode2 = (LocalDeclarationStatementSyntax)syntax)).Declaration;
                    break;
                case SyntaxKind.VariableDeclarator:
                    syntaxNode2 = syntax.Parent;
                    syntaxNode = syntax.Parent;
                    break;
                default:
                    syntaxNode2 = (syntaxNode = syntax);
                    break;
            }
            bool wasCompilerGenerated = boundLocalDeclaration.WasCompilerGenerated;
            ImmutableArray<IVariableDeclaratorOperation> declarators = CreateVariableDeclarator(boundLocalDeclaration, syntaxNode);
            ImmutableArray<IOperation> ignoredDimensions = CreateIgnoredDimensions(boundLocalDeclaration, syntaxNode);
            VariableDeclarationOperation item = new VariableDeclarationOperation(declarators, null, ignoredDimensions, _semanticModel, syntaxNode, wasCompilerGenerated);
            return new VariableDeclarationGroupOperation(isImplicit: syntaxNode2 == syntaxNode || boundLocalDeclaration.WasCompilerGenerated, declarations: ImmutableArray.Create((IVariableDeclarationOperation)item), semanticModel: _semanticModel, syntax: syntaxNode2);
        }

        private IOperation CreateBoundMultipleLocalDeclarationsBaseOperation(BoundMultipleLocalDeclarationsBase boundMultipleLocalDeclarations)
        {
            SyntaxNode syntax = boundMultipleLocalDeclarations.Syntax;
            SyntaxNode syntaxNode = (syntax.IsKind(SyntaxKind.LocalDeclarationStatement) ? ((LocalDeclarationStatementSyntax)syntax).Declaration : syntax);
            bool wasCompilerGenerated = boundMultipleLocalDeclarations.WasCompilerGenerated;
            ImmutableArray<IVariableDeclaratorOperation> declarators = CreateVariableDeclarator(boundMultipleLocalDeclarations, syntaxNode);
            ImmutableArray<IOperation> ignoredDimensions = CreateIgnoredDimensions(boundMultipleLocalDeclarations, syntaxNode);
            VariableDeclarationOperation item = new VariableDeclarationOperation(declarators, null, ignoredDimensions, _semanticModel, syntaxNode, wasCompilerGenerated);
            VariableDeclarationGroupOperation variableDeclarationGroupOperation = new VariableDeclarationGroupOperation(isImplicit: syntax == syntaxNode || boundMultipleLocalDeclarations.WasCompilerGenerated || boundMultipleLocalDeclarations is BoundUsingLocalDeclarations, declarations: ImmutableArray.Create((IVariableDeclarationOperation)item), semanticModel: _semanticModel, syntax: syntax);
            if (boundMultipleLocalDeclarations is BoundUsingLocalDeclarations boundUsingLocalDeclarations)
            {
                return new UsingDeclarationOperation(variableDeclarationGroupOperation, boundUsingLocalDeclarations.AwaitOpt != null, ((object)boundUsingLocalDeclarations.PatternDisposeInfoOpt != null) ? new DisposeOperationInfo(boundUsingLocalDeclarations.PatternDisposeInfoOpt!.Method.GetPublicSymbol(), CreateDisposeArguments(boundUsingLocalDeclarations.PatternDisposeInfoOpt, boundUsingLocalDeclarations.Syntax)) : default(DisposeOperationInfo), _semanticModel, syntax, boundMultipleLocalDeclarations.WasCompilerGenerated);
            }
            return variableDeclarationGroupOperation;
        }

        private ILabeledOperation CreateBoundLabelStatementOperation(BoundLabelStatement boundLabelStatement)
        {
            return new LabeledOperation(boundLabelStatement.Label.GetPublicSymbol(), syntax: boundLabelStatement.Syntax, isImplicit: boundLabelStatement.WasCompilerGenerated, operation: null, semanticModel: _semanticModel);
        }

        private ILabeledOperation CreateBoundLabeledStatementOperation(BoundLabeledStatement boundLabeledStatement)
        {
            return new LabeledOperation(boundLabeledStatement.Label.GetPublicSymbol(), Create(boundLabeledStatement.Body), syntax: boundLabeledStatement.Syntax, isImplicit: boundLabeledStatement.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IExpressionStatementOperation CreateBoundExpressionStatementOperation(BoundExpressionStatement boundExpressionStatement)
        {
            bool isImplicit = boundExpressionStatement.WasCompilerGenerated || boundExpressionStatement.Syntax == boundExpressionStatement.Expression.Syntax;
            SyntaxNode syntax = boundExpressionStatement.Syntax;
            IOperation? operation = Create(boundExpressionStatement.Expression);
            if (boundExpressionStatement.Expression is BoundSequence)
            {
                isImplicit = true;
            }
            return new ExpressionStatementOperation(operation, _semanticModel, syntax, isImplicit);
        }

        internal IOperation CreateBoundTupleOperation(BoundTupleExpression boundTupleExpression, bool createDeclaration = true)
        {
            SyntaxNode syntaxNode = boundTupleExpression.Syntax;
            bool wasCompilerGenerated = boundTupleExpression.WasCompilerGenerated;
            ITypeSymbol publicTypeSymbol = boundTupleExpression.GetPublicTypeSymbol();
            if (syntaxNode is DeclarationExpressionSyntax declarationExpressionSyntax)
            {
                syntaxNode = declarationExpressionSyntax.Designation;
                if (createDeclaration)
                {
                    return new DeclarationExpressionOperation(CreateBoundTupleOperation(boundTupleExpression, createDeclaration: false), _semanticModel, declarationExpressionSyntax, publicTypeSymbol, isImplicit: false);
                }
            }
            TypeSymbol typeSymbol;
            if (boundTupleExpression is BoundTupleLiteral boundTupleLiteral)
            {
                TypeSymbol type = boundTupleLiteral.Type;
                typeSymbol = type;
            }
            else
            {
                if (!(boundTupleExpression is BoundConvertedTupleLiteral boundConvertedTupleLiteral))
                {
                    if (boundTupleExpression != null)
                    {
                        BoundKind kind = boundTupleExpression.Kind;
                        throw ExceptionUtilities.UnexpectedValue(kind);
                    }
                    throw new InvalidOperationException();
                }
                BoundTupleLiteral sourceTuple = boundConvertedTupleLiteral.SourceTuple;
                if (sourceTuple != null)
                {
                    TypeSymbol type2 = sourceTuple.Type;
                    typeSymbol = type2;
                }
                else
                {
                    typeSymbol = null;
                }
            }
            TypeSymbol symbol = typeSymbol;
            return new TupleOperation(CreateFromArray<BoundExpression, IOperation>(boundTupleExpression.Arguments), symbol.GetPublicSymbol(), _semanticModel, syntaxNode, publicTypeSymbol, wasCompilerGenerated);
        }

        private IInterpolatedStringOperation CreateBoundInterpolatedStringExpressionOperation(BoundInterpolatedString boundInterpolatedString)
        {
            return new InterpolatedStringOperation(CreateBoundInterpolatedStringContentOperation(boundInterpolatedString.Parts), syntax: boundInterpolatedString.Syntax, type: boundInterpolatedString.GetPublicTypeSymbol(), constantValue: boundInterpolatedString.ConstantValue, isImplicit: boundInterpolatedString.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        internal ImmutableArray<IInterpolatedStringContentOperation> CreateBoundInterpolatedStringContentOperation(ImmutableArray<BoundExpression> parts)
        {
            ArrayBuilder<IInterpolatedStringContentOperation> instance = ArrayBuilder<IInterpolatedStringContentOperation>.GetInstance(parts.Length);
            ImmutableArray<BoundExpression>.Enumerator enumerator = parts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current.Kind == BoundKind.StringInsert)
                {
                    instance.Add((IInterpolatedStringContentOperation)Create(current));
                }
                else
                {
                    instance.Add(CreateBoundInterpolatedStringTextOperation((BoundLiteral)current));
                }
            }
            return instance.ToImmutableAndFree();
        }

        private IInterpolationOperation CreateBoundInterpolationOperation(BoundStringInsert boundStringInsert)
        {
            return new InterpolationOperation(Create(boundStringInsert.Value), Create(boundStringInsert.Alignment), Create(boundStringInsert.Format), syntax: boundStringInsert.Syntax, isImplicit: boundStringInsert.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IInterpolatedStringTextOperation CreateBoundInterpolatedStringTextOperation(BoundLiteral boundNode)
        {
            return new InterpolatedStringTextOperation(CreateBoundLiteralOperation(boundNode, @implicit: true), syntax: boundNode.Syntax, isImplicit: boundNode.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IConstantPatternOperation CreateBoundConstantPatternOperation(BoundConstantPattern boundConstantPattern)
        {
            IOperation? value = Create(boundConstantPattern.Value);
            SyntaxNode syntax = boundConstantPattern.Syntax;
            bool wasCompilerGenerated = boundConstantPattern.WasCompilerGenerated;
            TypeSymbol inputType = boundConstantPattern.InputType;
            return new ConstantPatternOperation(narrowedType: boundConstantPattern.NarrowedType.GetPublicSymbol(), value: value, inputType: inputType.GetPublicSymbol(), semanticModel: _semanticModel, syntax: syntax, isImplicit: wasCompilerGenerated);
        }

        private IOperation CreateBoundRelationalPatternOperation(BoundRelationalPattern boundRelationalPattern)
        {
            BinaryOperatorKind operatorKind = Helper.DeriveBinaryOperatorKind(boundRelationalPattern.Relation);
            IOperation value = Create(boundRelationalPattern.Value);
            SyntaxNode syntax = boundRelationalPattern.Syntax;
            bool wasCompilerGenerated = boundRelationalPattern.WasCompilerGenerated;
            TypeSymbol inputType = boundRelationalPattern.InputType;
            return new RelationalPatternOperation(narrowedType: boundRelationalPattern.NarrowedType.GetPublicSymbol(), operatorKind: operatorKind, value: value, inputType: inputType.GetPublicSymbol(), semanticModel: _semanticModel, syntax: syntax, isImplicit: wasCompilerGenerated);
        }

        private IDeclarationPatternOperation CreateBoundDeclarationPatternOperation(BoundDeclarationPattern boundDeclarationPattern)
        {
            ISymbol publicSymbol = boundDeclarationPattern.Variable.GetPublicSymbol();
            if (publicSymbol == null)
            {
                BoundExpression? variableAccess = boundDeclarationPattern.VariableAccess;
                if (variableAccess != null && variableAccess!.Kind == BoundKind.DiscardExpression)
                {
                    publicSymbol = ((BoundDiscardExpression)boundDeclarationPattern.VariableAccess).ExpressionSymbol.GetPublicSymbol();
                }
            }
            ITypeSymbol publicSymbol2 = boundDeclarationPattern.InputType.GetPublicSymbol();
            ITypeSymbol publicSymbol3 = boundDeclarationPattern.NarrowedType.GetPublicSymbol();
            bool isVar = boundDeclarationPattern.IsVar;
            return new DeclarationPatternOperation(isVar ? null : boundDeclarationPattern.DeclaredType.GetPublicTypeSymbol(), syntax: boundDeclarationPattern.Syntax, isImplicit: boundDeclarationPattern.WasCompilerGenerated, matchesNull: isVar, declaredSymbol: publicSymbol, inputType: publicSymbol2, narrowedType: publicSymbol3, semanticModel: _semanticModel);
        }

        private IRecursivePatternOperation CreateBoundRecursivePatternOperation(BoundRecursivePattern boundRecursivePattern)
        {
            ITypeSymbol publicSymbol = (boundRecursivePattern.DeclaredType?.Type ?? boundRecursivePattern.InputType.StrippedType()).GetPublicSymbol();
            ImmutableArray<BoundSubpattern> deconstruction = boundRecursivePattern.Deconstruction;
            ImmutableArray<IPatternOperation> deconstructionSubpatterns = ((!deconstruction.IsDefault) ? deconstruction.SelectAsArray((BoundSubpattern p, CSharpOperationFactory fac) => (IPatternOperation)fac.Create(p.Pattern), this) : ImmutableArray<IPatternOperation>.Empty);
            ImmutableArray<BoundSubpattern> properties = boundRecursivePattern.Properties;
            ImmutableArray<IPropertySubpatternOperation> propertySubpatterns = ((!properties.IsDefault) ? properties.SelectAsArray((BoundSubpattern p, (CSharpOperationFactory Fac, ITypeSymbol MatchedType) arg) => arg.Fac.CreatePropertySubpattern(p, arg.MatchedType), (this, publicSymbol)) : ImmutableArray<IPropertySubpatternOperation>.Empty);
            return new RecursivePatternOperation(publicSymbol, boundRecursivePattern.DeconstructMethod.GetPublicSymbol(), deconstructionSubpatterns, propertySubpatterns, boundRecursivePattern.Variable.GetPublicSymbol(), boundRecursivePattern.InputType.GetPublicSymbol(), boundRecursivePattern.NarrowedType.GetPublicSymbol(), _semanticModel, boundRecursivePattern.Syntax, boundRecursivePattern.WasCompilerGenerated);
        }

        private IRecursivePatternOperation CreateBoundRecursivePatternOperation(BoundITuplePattern boundITuplePattern)
        {
            ImmutableArray<BoundSubpattern> subpatterns = boundITuplePattern.Subpatterns;
            ImmutableArray<IPatternOperation> deconstructionSubpatterns = ((!subpatterns.IsDefault) ? subpatterns.SelectAsArray((BoundSubpattern p, CSharpOperationFactory fac) => (IPatternOperation)fac.Create(p.Pattern), this) : ImmutableArray<IPatternOperation>.Empty);
            return new RecursivePatternOperation(boundITuplePattern.InputType.StrippedType().GetPublicSymbol(), boundITuplePattern.GetLengthMethod.ContainingType.GetPublicSymbol(), deconstructionSubpatterns, ImmutableArray<IPropertySubpatternOperation>.Empty, null, boundITuplePattern.InputType.GetPublicSymbol(), boundITuplePattern.NarrowedType.GetPublicSymbol(), _semanticModel, boundITuplePattern.Syntax, boundITuplePattern.WasCompilerGenerated);
        }

        private IOperation CreateBoundTypePatternOperation(BoundTypePattern boundTypePattern)
        {
            return new TypePatternOperation(boundTypePattern.NarrowedType.GetPublicSymbol(), boundTypePattern.InputType.GetPublicSymbol(), boundTypePattern.NarrowedType.GetPublicSymbol(), _semanticModel, boundTypePattern.Syntax, boundTypePattern.WasCompilerGenerated);
        }

        private IOperation CreateBoundNegatedPatternOperation(BoundNegatedPattern boundNegatedPattern)
        {
            return new NegatedPatternOperation((IPatternOperation)Create(boundNegatedPattern.Negated), boundNegatedPattern.InputType.GetPublicSymbol(), boundNegatedPattern.NarrowedType.GetPublicSymbol(), _semanticModel, boundNegatedPattern.Syntax, boundNegatedPattern.WasCompilerGenerated);
        }

        private IOperation CreateBoundBinaryPatternOperation(BoundBinaryPattern boundBinaryPattern)
        {
            return new BinaryPatternOperation(boundBinaryPattern.Disjunction ? BinaryOperatorKind.Or : BinaryOperatorKind.And, (IPatternOperation)Create(boundBinaryPattern.Left), (IPatternOperation)Create(boundBinaryPattern.Right), boundBinaryPattern.InputType.GetPublicSymbol(), boundBinaryPattern.NarrowedType.GetPublicSymbol(), _semanticModel, boundBinaryPattern.Syntax, boundBinaryPattern.WasCompilerGenerated);
        }

        private ISwitchOperation CreateBoundSwitchStatementOperation(BoundSwitchStatement boundSwitchStatement)
        {
            IOperation value = Create(boundSwitchStatement.Expression);
            ImmutableArray<ISwitchCaseOperation> cases = CreateFromArray<BoundSwitchSection, ISwitchCaseOperation>(boundSwitchStatement.SwitchSections);
            return new SwitchOperation(boundSwitchStatement.InnerLocals.GetPublicSymbols(), exitLabel: boundSwitchStatement.BreakLabel.GetPublicSymbol(), syntax: boundSwitchStatement.Syntax, isImplicit: boundSwitchStatement.WasCompilerGenerated, value: value, cases: cases, semanticModel: _semanticModel);
        }

        private ISwitchCaseOperation CreateBoundSwitchSectionOperation(BoundSwitchSection boundSwitchSection)
        {
            ImmutableArray<ICaseClauseOperation> clauses = CreateFromArray<BoundSwitchLabel, ICaseClauseOperation>(boundSwitchSection.SwitchLabels);
            ImmutableArray<IOperation> body = CreateFromArray<BoundStatement, IOperation>(boundSwitchSection.Statements);
            ImmutableArray<ILocalSymbol> publicSymbols = boundSwitchSection.Locals.GetPublicSymbols();
            return new SwitchCaseOperation(clauses, body, publicSymbols, null, _semanticModel, boundSwitchSection.Syntax, boundSwitchSection.WasCompilerGenerated);
        }

        private ISwitchExpressionOperation CreateBoundSwitchExpressionOperation(BoundSwitchExpression boundSwitchExpression)
        {
            IOperation? value = Create(boundSwitchExpression.Expression);
            ImmutableArray<ISwitchExpressionArmOperation> arms = CreateFromArray<BoundSwitchExpressionArm, ISwitchExpressionArmOperation>(boundSwitchExpression.SwitchArms);
            return new SwitchExpressionOperation(value, arms, _semanticModel, boundSwitchExpression.Syntax, boundSwitchExpression.GetPublicTypeSymbol(), boundSwitchExpression.WasCompilerGenerated);
        }

        private ISwitchExpressionArmOperation CreateBoundSwitchExpressionArmOperation(BoundSwitchExpressionArm boundSwitchExpressionArm)
        {
            IPatternOperation pattern = (IPatternOperation)Create(boundSwitchExpressionArm.Pattern);
            IOperation guard = Create(boundSwitchExpressionArm.WhenClause);
            IOperation value = Create(boundSwitchExpressionArm.Value);
            return new SwitchExpressionArmOperation(pattern, guard, value, boundSwitchExpressionArm.Locals.GetPublicSymbols(), _semanticModel, boundSwitchExpressionArm.Syntax, boundSwitchExpressionArm.WasCompilerGenerated);
        }

        private ICaseClauseOperation CreateBoundSwitchLabelOperation(BoundSwitchLabel boundSwitchLabel)
        {
            SyntaxNode syntax = boundSwitchLabel.Syntax;
            bool wasCompilerGenerated = boundSwitchLabel.WasCompilerGenerated;
            LabelSymbol label = boundSwitchLabel.Label;
            if (boundSwitchLabel.Syntax.Kind() == SyntaxKind.DefaultSwitchLabel)
            {
                return new DefaultCaseClauseOperation(label.GetPublicSymbol(), _semanticModel, syntax, wasCompilerGenerated);
            }
            if (boundSwitchLabel.WhenClause == null && boundSwitchLabel.Pattern.Kind == BoundKind.ConstantPattern && boundSwitchLabel.Pattern is BoundConstantPattern boundConstantPattern && boundConstantPattern.InputType.IsValidV6SwitchGoverningType())
            {
                return new SingleValueCaseClauseOperation(Create(boundConstantPattern.Value), label.GetPublicSymbol(), _semanticModel, syntax, wasCompilerGenerated);
            }
            IPatternOperation pattern = (IPatternOperation)Create(boundSwitchLabel.Pattern);
            IOperation guard = Create(boundSwitchLabel.WhenClause);
            return new PatternCaseClauseOperation(label.GetPublicSymbol(), pattern, guard, _semanticModel, syntax, wasCompilerGenerated);
        }

        private IIsPatternOperation CreateBoundIsPatternExpressionOperation(BoundIsPatternExpression boundIsPatternExpression)
        {
            return new IsPatternOperation(Create(boundIsPatternExpression.Expression), (IPatternOperation)Create(boundIsPatternExpression.Pattern), syntax: boundIsPatternExpression.Syntax, type: boundIsPatternExpression.GetPublicTypeSymbol(), isImplicit: boundIsPatternExpression.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundQueryClauseOperation(BoundQueryClause boundQueryClause)
        {
            if (boundQueryClause.Syntax.Kind() != SyntaxKind.QueryExpression)
            {
                return Create(boundQueryClause.Value);
            }
            return new TranslatedQueryOperation(Create(boundQueryClause.Value), syntax: boundQueryClause.Syntax, type: boundQueryClause.GetPublicTypeSymbol(), isImplicit: boundQueryClause.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private IOperation CreateBoundRangeVariableOperation(BoundRangeVariable boundRangeVariable)
        {
            return Create(boundRangeVariable.Value);
        }

        private IOperation CreateBoundDiscardExpressionOperation(BoundDiscardExpression boundNode)
        {
            return new DiscardOperation(((DiscardSymbol)boundNode.ExpressionSymbol).GetPublicSymbol(), _semanticModel, boundNode.Syntax, boundNode.GetPublicTypeSymbol(), boundNode.WasCompilerGenerated);
        }

        private IOperation CreateFromEndIndexExpressionOperation(BoundFromEndIndexExpression boundIndex)
        {
            return new UnaryOperation(UnaryOperatorKind.Hat, Create(boundIndex.Operand), boundIndex.Type.IsNullableType(), isChecked: false, null, _semanticModel, boundIndex.Syntax, boundIndex.GetPublicTypeSymbol(), null, boundIndex.WasCompilerGenerated);
        }

        private IOperation CreateRangeExpressionOperation(BoundRangeExpression boundRange)
        {
            IOperation? leftOperand = Create(boundRange.LeftOperandOpt);
            IOperation rightOperand = Create(boundRange.RightOperandOpt);
            return new RangeOperation(leftOperand, rightOperand, boundRange.Type.IsNullableType(), boundRange.MethodOpt.GetPublicSymbol(), _semanticModel, boundRange.Syntax, boundRange.GetPublicTypeSymbol(), boundRange.WasCompilerGenerated);
        }

        private IOperation CreateBoundDiscardPatternOperation(BoundDiscardPattern boundNode)
        {
            return new DiscardPatternOperation(boundNode.InputType.GetPublicSymbol(), boundNode.NarrowedType.GetPublicSymbol(), _semanticModel, boundNode.Syntax, boundNode.WasCompilerGenerated);
        }

        internal IPropertySubpatternOperation CreatePropertySubpattern(BoundSubpattern subpattern, ITypeSymbol matchedType)
        {
            SyntaxNode syntax = subpattern.Syntax;
            IOperation member = CreatePropertySubpatternMember(subpattern.Symbol, matchedType, syntax);
            IPatternOperation pattern = (IPatternOperation)Create(subpattern.Pattern);
            return new PropertySubpatternOperation(member, pattern, _semanticModel, syntax, isImplicit: false);
        }

        internal IOperation CreatePropertySubpatternMember(Symbol? symbol, ITypeSymbol matchedType, SyntaxNode syntax)
        {
            Symbol symbol2 = symbol;
            ITypeSymbol matchedType2 = matchedType;
            SyntaxNode nameSyntax = ((!(syntax is SubpatternSyntax subpatternSyntax)) ? null : subpatternSyntax.NameColon?.Name) ?? syntax;
            bool isImplicit = nameSyntax == syntax;
            if (!(symbol2 is FieldSymbol fieldSymbol))
            {
                if (symbol2 is PropertySymbol propertySymbol)
                {
                    return new PropertyReferenceOperation(propertySymbol.GetPublicSymbol(), ImmutableArray<IArgumentOperation>.Empty, createReceiver(), _semanticModel, nameSyntax, propertySymbol.Type.GetPublicSymbol(), isImplicit);
                }
                return OperationFactory.CreateInvalidOperation(_semanticModel, nameSyntax, ImmutableArray<IOperation>.Empty, isImplicit);
            }
            ConstantValue constantValue = fieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
            return new FieldReferenceOperation(fieldSymbol.GetPublicSymbol(), isDeclaration: false, createReceiver(), _semanticModel, nameSyntax, fieldSymbol.Type.GetPublicSymbol(), constantValue, isImplicit);
            IOperation? createReceiver()
            {
                Symbol symbol3 = symbol2;
                if ((object)symbol3 == null || symbol3.IsStatic)
                {
                    return null;
                }
                return new InstanceReferenceOperation(InstanceReferenceKind.PatternInput, _semanticModel, nameSyntax, matchedType2, isImplicit: true);
            }
        }

        private IInstanceReferenceOperation CreateCollectionValuePlaceholderOperation(BoundObjectOrCollectionValuePlaceholder placeholder)
        {
            return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, syntax: placeholder.Syntax, type: placeholder.GetPublicTypeSymbol(), isImplicit: placeholder.WasCompilerGenerated, semanticModel: _semanticModel);
        }

        private ImmutableArray<IArgumentOperation> CreateDisposeArguments(MethodArgumentInfo patternDisposeInfo, SyntaxNode syntax)
        {
            if (patternDisposeInfo.Method.ParameterCount == 0)
            {
                return ImmutableArray<IArgumentOperation>.Empty;
            }
            return Operation.SetParentOperation(DeriveArguments(patternDisposeInfo.Method, patternDisposeInfo.Arguments, patternDisposeInfo.ArgsToParamsOpt, patternDisposeInfo.DefaultArguments, patternDisposeInfo.Expanded, syntax), null);
        }

        internal ImmutableArray<BoundStatement> ToStatements(BoundStatement? statement)
        {
            if (statement == null)
            {
                return ImmutableArray<BoundStatement>.Empty;
            }
            if (statement!.Kind == BoundKind.StatementList)
            {
                return ((BoundStatementList)statement).Statements;
            }
            return ImmutableArray.Create(statement);
        }

        private IInstanceReferenceOperation CreateImplicitReceiver(SyntaxNode syntax, TypeSymbol type)
        {
            return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, _semanticModel, syntax, type.GetPublicSymbol(), isImplicit: true);
        }

        internal IArgumentOperation CreateArgumentOperation(ArgumentKind kind, IParameterSymbol? parameter, BoundExpression expression)
        {
            IOperation operation = Create(expression);
            SyntaxNode syntax = expression.Syntax;
            SyntaxNode syntax2;
            bool isImplicit;
            if (syntax == null || !(syntax.Parent is ArgumentSyntax argumentSyntax))
            {
                syntax2 = operation.Syntax;
                isImplicit = true;
            }
            else
            {
                bool wasCompilerGenerated = expression.WasCompilerGenerated;
                syntax2 = argumentSyntax;
                isImplicit = wasCompilerGenerated;
            }
            return new ArgumentOperation(kind, parameter, operation, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, _semanticModel, syntax2, isImplicit);
        }

        internal IVariableInitializerOperation? CreateVariableDeclaratorInitializer(BoundLocalDeclaration boundLocalDeclaration, SyntaxNode syntax)
        {
            if (boundLocalDeclaration.InitializerOpt != null)
            {
                SyntaxNode syntaxNode = null;
                bool isImplicit = false;
                if (syntax is VariableDeclaratorSyntax variableDeclaratorSyntax)
                {
                    syntaxNode = variableDeclaratorSyntax.Initializer;
                }
                if (syntaxNode == null)
                {
                    syntaxNode = boundLocalDeclaration.InitializerOpt!.Syntax;
                    isImplicit = true;
                }
                IOperation value = Create(boundLocalDeclaration.InitializerOpt);
                return new VariableInitializerOperation(ImmutableArray<ILocalSymbol>.Empty, value, _semanticModel, syntaxNode, isImplicit);
            }
            return null;
        }

        private IVariableDeclaratorOperation CreateVariableDeclaratorInternal(BoundLocalDeclaration boundLocalDeclaration, SyntaxNode syntax)
        {
            ILocalSymbol? publicSymbol = boundLocalDeclaration.LocalSymbol.GetPublicSymbol();
            bool isImplicit = false;
            IVariableInitializerOperation initializer = CreateVariableDeclaratorInitializer(boundLocalDeclaration, syntax);
            ImmutableArray<IOperation> ignoredArguments = CreateFromArray<BoundExpression, IOperation>(boundLocalDeclaration.ArgumentsOpt);
            return new VariableDeclaratorOperation(publicSymbol, initializer, ignoredArguments, _semanticModel, syntax, isImplicit);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("boundLocal")]
        internal IVariableDeclaratorOperation? CreateVariableDeclarator(BoundLocal? boundLocal)
        {
            if (boundLocal != null)
            {
                return new VariableDeclaratorOperation(boundLocal!.LocalSymbol.GetPublicSymbol(), null, ImmutableArray<IOperation>.Empty, _semanticModel, boundLocal!.Syntax, isImplicit: false);
            }
            return null;
        }

        internal IOperation? CreateReceiverOperation(BoundNode? instance, Symbol? symbol)
        {
            if (instance == null || instance!.Kind == BoundKind.TypeExpression)
            {
                return null;
            }
            if (symbol != null && symbol!.IsStatic && instance!.WasCompilerGenerated && instance!.Kind == BoundKind.ThisReference)
            {
                return null;
            }
            return Create(instance);
        }

        private bool IsCallVirtual(MethodSymbol? targetMethod, BoundExpression? receiver)
        {
            if ((object)targetMethod != null && receiver != null && (targetMethod!.IsVirtual || targetMethod!.IsAbstract || targetMethod!.IsOverride))
            {
                return !receiver!.SuppressVirtualCalls;
            }
            return false;
        }

        private bool IsMethodInvalid(LookupResultKind resultKind, MethodSymbol targetMethod)
        {
            if (resultKind != LookupResultKind.OverloadResolutionFailure)
            {
                return targetMethod?.OriginalDefinition is ErrorMethodSymbol;
            }
            return true;
        }

        internal IEventReferenceOperation CreateBoundEventAccessOperation(BoundEventAssignmentOperator boundEventAssignmentOperator)
        {
            SyntaxNode syntax = boundEventAssignmentOperator.Syntax;
            IEventSymbol publicSymbol = boundEventAssignmentOperator.Event.GetPublicSymbol();
            IOperation instance = CreateReceiverOperation(boundEventAssignmentOperator.ReceiverOpt, boundEventAssignmentOperator.Event);
            SyntaxNode left = ((AssignmentExpressionSyntax)syntax).Left;
            bool wasCompilerGenerated = boundEventAssignmentOperator.WasCompilerGenerated;
            return new EventReferenceOperation(publicSymbol, instance, _semanticModel, left, publicSymbol.Type, wasCompilerGenerated);
        }

        internal IOperation CreateDelegateTargetOperation(BoundNode delegateNode)
        {
            if (delegateNode is BoundConversion boundConversion)
            {
                if (boundConversion.ConversionKind == ConversionKind.MethodGroup)
                {
                    return CreateBoundMethodGroupSingleMethodOperation((BoundMethodGroup)boundConversion.Operand, boundConversion.SymbolOpt, boundConversion.SuppressVirtualCalls);
                }
                return Create(boundConversion.Operand);
            }
            BoundDelegateCreationExpression boundDelegateCreationExpression = (BoundDelegateCreationExpression)delegateNode;
            if (boundDelegateCreationExpression.Argument.Kind == BoundKind.MethodGroup && boundDelegateCreationExpression.MethodOpt != null)
            {
                BoundMethodGroup boundMethodGroup = (BoundMethodGroup)boundDelegateCreationExpression.Argument;
                return CreateBoundMethodGroupSingleMethodOperation(boundMethodGroup, boundDelegateCreationExpression.MethodOpt, boundMethodGroup.SuppressVirtualCalls);
            }
            return Create(boundDelegateCreationExpression.Argument);
        }

        internal IOperation CreateMemberInitializerInitializedMember(BoundNode initializedMember)
        {
            if (!(initializedMember is BoundObjectInitializerMember boundObjectInitializerMember))
            {
                if (initializedMember is BoundDynamicObjectInitializerMember boundDynamicObjectInitializerMember)
                {
                    return CreateBoundDynamicObjectInitializerMemberOperation(boundDynamicObjectInitializerMember);
                }
                return Create(initializedMember);
            }
            return CreateBoundObjectInitializerMemberOperation(boundObjectInitializerMember, isObjectOrCollectionInitializer: true);
        }

        internal ImmutableArray<IArgumentOperation> DeriveArguments(BoundNode containingExpression, bool isObjectOrCollectionInitializer)
        {
            if (containingExpression.Kind == BoundKind.ObjectInitializerMember)
            {
                BoundObjectInitializerMember boundObjectInitializerMember = (BoundObjectInitializerMember)containingExpression;
                PropertySymbol methodOrIndexer = (PropertySymbol)boundObjectInitializerMember.MemberSymbol;
                return DeriveArguments(methodOrIndexer, boundObjectInitializerMember.Arguments, boundObjectInitializerMember.ArgsToParamsOpt, boundObjectInitializerMember.DefaultArguments, boundObjectInitializerMember.Expanded, boundObjectInitializerMember.Syntax);
            }
            return DeriveArguments(containingExpression);
        }

        internal ImmutableArray<IArgumentOperation> DeriveArguments(BoundNode containingExpression)
        {
            switch (containingExpression.Kind)
            {
                case BoundKind.IndexerAccess:
                    {
                        BoundIndexerAccess boundIndexerAccess = (BoundIndexerAccess)containingExpression;
                        return DeriveArguments(boundIndexerAccess.Indexer, boundIndexerAccess.Arguments, boundIndexerAccess.ArgsToParamsOpt, boundIndexerAccess.DefaultArguments, boundIndexerAccess.Expanded, boundIndexerAccess.Syntax);
                    }
                case BoundKind.ObjectCreationExpression:
                    {
                        BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)containingExpression;
                        return DeriveArguments(boundObjectCreationExpression.Constructor, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.ArgsToParamsOpt, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.Expanded, boundObjectCreationExpression.Syntax);
                    }
                case BoundKind.Call:
                    {
                        BoundCall boundCall = (BoundCall)containingExpression;
                        return DeriveArguments(boundCall.Method, boundCall.Arguments, boundCall.ArgsToParamsOpt, boundCall.DefaultArguments, boundCall.Expanded, boundCall.Syntax, boundCall.InvokedAsExtensionMethod);
                    }
                case BoundKind.CollectionElementInitializer:
                    {
                        BoundCollectionElementInitializer boundCollectionElementInitializer = (BoundCollectionElementInitializer)containingExpression;
                        return DeriveArguments(boundCollectionElementInitializer.AddMethod, boundCollectionElementInitializer.Arguments, boundCollectionElementInitializer.ArgsToParamsOpt, boundCollectionElementInitializer.DefaultArguments, boundCollectionElementInitializer.Expanded, boundCollectionElementInitializer.Syntax, boundCollectionElementInitializer.InvokedAsExtensionMethod);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(containingExpression.Kind);
            }
        }

        private ImmutableArray<IArgumentOperation> DeriveArguments(Symbol methodOrIndexer, ImmutableArray<BoundExpression> boundArguments, ImmutableArray<int> argumentsToParametersOpt, BitVector defaultArguments, bool expanded, SyntaxNode invocationSyntax, bool invokedAsExtensionMethod = false)
        {
            if (methodOrIndexer.GetParameters().IsDefaultOrEmpty && boundArguments.IsDefaultOrEmpty)
            {
                return ImmutableArray<IArgumentOperation>.Empty;
            }
            return LocalRewriter.MakeArgumentsInEvaluationOrder(this, (CSharpCompilation)_semanticModel.Compilation, invocationSyntax, boundArguments, methodOrIndexer, expanded, argumentsToParametersOpt, defaultArguments, invokedAsExtensionMethod);
        }

        internal static ImmutableArray<BoundNode> CreateInvalidChildrenFromArgumentsExpression(BoundNode? receiverOpt, ImmutableArray<BoundExpression> arguments, BoundExpression? additionalNodeOpt = null)
        {
            ArrayBuilder<BoundNode> instance = ArrayBuilder<BoundNode>.GetInstance();
            if (receiverOpt != null && (!receiverOpt!.WasCompilerGenerated || (receiverOpt!.Kind != BoundKind.ThisReference && receiverOpt!.Kind != BoundKind.BaseReference && receiverOpt!.Kind != BoundKind.ObjectOrCollectionValuePlaceholder)))
            {
                instance.Add(receiverOpt);
            }
            instance.AddRange(StaticCast<BoundNode>.From(arguments));
            instance.AddIfNotNull(additionalNodeOpt);
            return instance.ToImmutableAndFree();
        }

        internal ImmutableArray<IOperation> GetAnonymousObjectCreationInitializers(ImmutableArray<BoundExpression> arguments, ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, SyntaxNode syntax, ITypeSymbol type, bool isImplicit)
        {
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(arguments.Length);
            int currentDeclarationIndex2 = 0;
            for (int i = 0; i < arguments.Length; i++)
            {
                IOperation operation = Create(arguments[i]);
                InstanceReferenceOperation instance2 = new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, _semanticModel, syntax, type, isImplicit: true);
                PropertySymbol anonymousTypeProperty = AnonymousTypeManager.GetAnonymousTypeProperty(type.GetSymbol<NamedTypeSymbol>(), i);
                BoundAnonymousPropertyDeclaration boundAnonymousPropertyDeclaration = getDeclaration(declarations, anonymousTypeProperty, ref currentDeclarationIndex2);
                IOperation operation2;
                bool isImplicit2;
                if (boundAnonymousPropertyDeclaration == null)
                {
                    operation2 = new PropertyReferenceOperation(anonymousTypeProperty.GetPublicSymbol(), ImmutableArray<IArgumentOperation>.Empty, instance2, _semanticModel, operation.Syntax, anonymousTypeProperty.Type.GetPublicSymbol(), isImplicit: true);
                    isImplicit2 = true;
                }
                else
                {
                    operation2 = new PropertyReferenceOperation(boundAnonymousPropertyDeclaration.Property.GetPublicSymbol(), ImmutableArray<IArgumentOperation>.Empty, instance2, _semanticModel, boundAnonymousPropertyDeclaration.Syntax, boundAnonymousPropertyDeclaration.GetPublicTypeSymbol(), boundAnonymousPropertyDeclaration.WasCompilerGenerated);
                    isImplicit2 = isImplicit;
                }
                SyntaxNode syntax2 = operation.Syntax?.Parent ?? syntax;
                ITypeSymbol type2 = operation2.Type;
                SimpleAssignmentOperation item = new SimpleAssignmentOperation(isRef: false, operation2, operation, _semanticModel, syntax2, type2, operation.GetConstantValue(), isImplicit2);
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
            static BoundAnonymousPropertyDeclaration? getDeclaration(ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, PropertySymbol currentProperty, ref int currentDeclarationIndex)
            {
                if (currentDeclarationIndex >= declarations.Length)
                {
                    return null;
                }
                BoundAnonymousPropertyDeclaration boundAnonymousPropertyDeclaration2 = declarations[currentDeclarationIndex];
                if (currentProperty.MemberIndexOpt == boundAnonymousPropertyDeclaration2.Property.MemberIndexOpt)
                {
                    currentDeclarationIndex++;
                    return boundAnonymousPropertyDeclaration2;
                }
                return null;
            }
        }
    }
}
