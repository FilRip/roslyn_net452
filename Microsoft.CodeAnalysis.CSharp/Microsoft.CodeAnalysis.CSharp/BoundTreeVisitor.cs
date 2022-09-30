using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundTreeVisitor<A, R>
    {
        public virtual R Visit(BoundNode node, A arg)
        {
            if (node == null)
            {
                return default(R);
            }
            return node.Kind switch
            {
                BoundKind.TypeExpression => VisitTypeExpression(node as BoundTypeExpression, arg),
                BoundKind.NamespaceExpression => VisitNamespaceExpression(node as BoundNamespaceExpression, arg),
                BoundKind.UnaryOperator => VisitUnaryOperator(node as BoundUnaryOperator, arg),
                BoundKind.IncrementOperator => VisitIncrementOperator(node as BoundIncrementOperator, arg),
                BoundKind.BinaryOperator => VisitBinaryOperator(node as BoundBinaryOperator, arg),
                BoundKind.CompoundAssignmentOperator => VisitCompoundAssignmentOperator(node as BoundCompoundAssignmentOperator, arg),
                BoundKind.AssignmentOperator => VisitAssignmentOperator(node as BoundAssignmentOperator, arg),
                BoundKind.NullCoalescingOperator => VisitNullCoalescingOperator(node as BoundNullCoalescingOperator, arg),
                BoundKind.ConditionalOperator => VisitConditionalOperator(node as BoundConditionalOperator, arg),
                BoundKind.ArrayAccess => VisitArrayAccess(node as BoundArrayAccess, arg),
                BoundKind.TypeOfOperator => VisitTypeOfOperator(node as BoundTypeOfOperator, arg),
                BoundKind.DefaultLiteral => VisitDefaultLiteral(node as BoundDefaultLiteral, arg),
                BoundKind.DefaultExpression => VisitDefaultExpression(node as BoundDefaultExpression, arg),
                BoundKind.IsOperator => VisitIsOperator(node as BoundIsOperator, arg),
                BoundKind.AsOperator => VisitAsOperator(node as BoundAsOperator, arg),
                BoundKind.Conversion => VisitConversion(node as BoundConversion, arg),
                BoundKind.SequencePointExpression => VisitSequencePointExpression(node as BoundSequencePointExpression, arg),
                BoundKind.SequencePoint => VisitSequencePoint(node as BoundSequencePoint, arg),
                BoundKind.SequencePointWithSpan => VisitSequencePointWithSpan(node as BoundSequencePointWithSpan, arg),
                BoundKind.Block => VisitBlock(node as BoundBlock, arg),
                BoundKind.LocalDeclaration => VisitLocalDeclaration(node as BoundLocalDeclaration, arg),
                BoundKind.MultipleLocalDeclarations => VisitMultipleLocalDeclarations(node as BoundMultipleLocalDeclarations, arg),
                BoundKind.Sequence => VisitSequence(node as BoundSequence, arg),
                BoundKind.NoOpStatement => VisitNoOpStatement(node as BoundNoOpStatement, arg),
                BoundKind.ReturnStatement => VisitReturnStatement(node as BoundReturnStatement, arg),
                BoundKind.ThrowStatement => VisitThrowStatement(node as BoundThrowStatement, arg),
                BoundKind.ExpressionStatement => VisitExpressionStatement(node as BoundExpressionStatement, arg),
                BoundKind.BreakStatement => VisitBreakStatement(node as BoundBreakStatement, arg),
                BoundKind.ContinueStatement => VisitContinueStatement(node as BoundContinueStatement, arg),
                BoundKind.IfStatement => VisitIfStatement(node as BoundIfStatement, arg),
                BoundKind.ForEachStatement => VisitForEachStatement(node as BoundForEachStatement, arg),
                BoundKind.TryStatement => VisitTryStatement(node as BoundTryStatement, arg),
                BoundKind.Literal => VisitLiteral(node as BoundLiteral, arg),
                BoundKind.ThisReference => VisitThisReference(node as BoundThisReference, arg),
                BoundKind.Local => VisitLocal(node as BoundLocal, arg),
                BoundKind.Parameter => VisitParameter(node as BoundParameter, arg),
                BoundKind.LabelStatement => VisitLabelStatement(node as BoundLabelStatement, arg),
                BoundKind.GotoStatement => VisitGotoStatement(node as BoundGotoStatement, arg),
                BoundKind.LabeledStatement => VisitLabeledStatement(node as BoundLabeledStatement, arg),
                BoundKind.StatementList => VisitStatementList(node as BoundStatementList, arg),
                BoundKind.ConditionalGoto => VisitConditionalGoto(node as BoundConditionalGoto, arg),
                BoundKind.Call => VisitCall(node as BoundCall, arg),
                BoundKind.ObjectCreationExpression => VisitObjectCreationExpression(node as BoundObjectCreationExpression, arg),
                BoundKind.DelegateCreationExpression => VisitDelegateCreationExpression(node as BoundDelegateCreationExpression, arg),
                BoundKind.FieldAccess => VisitFieldAccess(node as BoundFieldAccess, arg),
                BoundKind.PropertyAccess => VisitPropertyAccess(node as BoundPropertyAccess, arg),
                BoundKind.Lambda => VisitLambda(node as BoundLambda, arg),
                BoundKind.NameOfOperator => VisitNameOfOperator(node as BoundNameOfOperator, arg),
                _ => VisitInternal(node, arg),
            };
        }

        public virtual R DefaultVisit(BoundNode node, A arg)
        {
            return default(R);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DebuggerStepThrough]
        internal R VisitInternal(BoundNode node, A arg)
        {
            return node.Kind switch
            {
                BoundKind.FieldEqualsValue => VisitFieldEqualsValue((BoundFieldEqualsValue)node, arg),
                BoundKind.PropertyEqualsValue => VisitPropertyEqualsValue((BoundPropertyEqualsValue)node, arg),
                BoundKind.ParameterEqualsValue => VisitParameterEqualsValue((BoundParameterEqualsValue)node, arg),
                BoundKind.GlobalStatementInitializer => VisitGlobalStatementInitializer((BoundGlobalStatementInitializer)node, arg),
                BoundKind.DeconstructValuePlaceholder => VisitDeconstructValuePlaceholder((BoundDeconstructValuePlaceholder)node, arg),
                BoundKind.TupleOperandPlaceholder => VisitTupleOperandPlaceholder((BoundTupleOperandPlaceholder)node, arg),
                BoundKind.AwaitableValuePlaceholder => VisitAwaitableValuePlaceholder((BoundAwaitableValuePlaceholder)node, arg),
                BoundKind.DisposableValuePlaceholder => VisitDisposableValuePlaceholder((BoundDisposableValuePlaceholder)node, arg),
                BoundKind.ObjectOrCollectionValuePlaceholder => VisitObjectOrCollectionValuePlaceholder((BoundObjectOrCollectionValuePlaceholder)node, arg),
                BoundKind.Dup => VisitDup((BoundDup)node, arg),
                BoundKind.PassByCopy => VisitPassByCopy((BoundPassByCopy)node, arg),
                BoundKind.BadExpression => VisitBadExpression((BoundBadExpression)node, arg),
                BoundKind.BadStatement => VisitBadStatement((BoundBadStatement)node, arg),
                BoundKind.ExtractedFinallyBlock => VisitExtractedFinallyBlock((BoundExtractedFinallyBlock)node, arg),
                BoundKind.TypeExpression => VisitTypeExpression((BoundTypeExpression)node, arg),
                BoundKind.TypeOrValueExpression => VisitTypeOrValueExpression((BoundTypeOrValueExpression)node, arg),
                BoundKind.NamespaceExpression => VisitNamespaceExpression((BoundNamespaceExpression)node, arg),
                BoundKind.UnaryOperator => VisitUnaryOperator((BoundUnaryOperator)node, arg),
                BoundKind.IncrementOperator => VisitIncrementOperator((BoundIncrementOperator)node, arg),
                BoundKind.AddressOfOperator => VisitAddressOfOperator((BoundAddressOfOperator)node, arg),
                BoundKind.UnconvertedAddressOfOperator => VisitUnconvertedAddressOfOperator((BoundUnconvertedAddressOfOperator)node, arg),
                BoundKind.FunctionPointerLoad => VisitFunctionPointerLoad((BoundFunctionPointerLoad)node, arg),
                BoundKind.PointerIndirectionOperator => VisitPointerIndirectionOperator((BoundPointerIndirectionOperator)node, arg),
                BoundKind.PointerElementAccess => VisitPointerElementAccess((BoundPointerElementAccess)node, arg),
                BoundKind.FunctionPointerInvocation => VisitFunctionPointerInvocation((BoundFunctionPointerInvocation)node, arg),
                BoundKind.RefTypeOperator => VisitRefTypeOperator((BoundRefTypeOperator)node, arg),
                BoundKind.MakeRefOperator => VisitMakeRefOperator((BoundMakeRefOperator)node, arg),
                BoundKind.RefValueOperator => VisitRefValueOperator((BoundRefValueOperator)node, arg),
                BoundKind.FromEndIndexExpression => VisitFromEndIndexExpression((BoundFromEndIndexExpression)node, arg),
                BoundKind.RangeExpression => VisitRangeExpression((BoundRangeExpression)node, arg),
                BoundKind.BinaryOperator => VisitBinaryOperator((BoundBinaryOperator)node, arg),
                BoundKind.TupleBinaryOperator => VisitTupleBinaryOperator((BoundTupleBinaryOperator)node, arg),
                BoundKind.UserDefinedConditionalLogicalOperator => VisitUserDefinedConditionalLogicalOperator((BoundUserDefinedConditionalLogicalOperator)node, arg),
                BoundKind.CompoundAssignmentOperator => VisitCompoundAssignmentOperator((BoundCompoundAssignmentOperator)node, arg),
                BoundKind.AssignmentOperator => VisitAssignmentOperator((BoundAssignmentOperator)node, arg),
                BoundKind.DeconstructionAssignmentOperator => VisitDeconstructionAssignmentOperator((BoundDeconstructionAssignmentOperator)node, arg),
                BoundKind.NullCoalescingOperator => VisitNullCoalescingOperator((BoundNullCoalescingOperator)node, arg),
                BoundKind.NullCoalescingAssignmentOperator => VisitNullCoalescingAssignmentOperator((BoundNullCoalescingAssignmentOperator)node, arg),
                BoundKind.UnconvertedConditionalOperator => VisitUnconvertedConditionalOperator((BoundUnconvertedConditionalOperator)node, arg),
                BoundKind.ConditionalOperator => VisitConditionalOperator((BoundConditionalOperator)node, arg),
                BoundKind.ArrayAccess => VisitArrayAccess((BoundArrayAccess)node, arg),
                BoundKind.ArrayLength => VisitArrayLength((BoundArrayLength)node, arg),
                BoundKind.AwaitableInfo => VisitAwaitableInfo((BoundAwaitableInfo)node, arg),
                BoundKind.AwaitExpression => VisitAwaitExpression((BoundAwaitExpression)node, arg),
                BoundKind.TypeOfOperator => VisitTypeOfOperator((BoundTypeOfOperator)node, arg),
                BoundKind.MethodDefIndex => VisitMethodDefIndex((BoundMethodDefIndex)node, arg),
                BoundKind.MaximumMethodDefIndex => VisitMaximumMethodDefIndex((BoundMaximumMethodDefIndex)node, arg),
                BoundKind.InstrumentationPayloadRoot => VisitInstrumentationPayloadRoot((BoundInstrumentationPayloadRoot)node, arg),
                BoundKind.ModuleVersionId => VisitModuleVersionId((BoundModuleVersionId)node, arg),
                BoundKind.ModuleVersionIdString => VisitModuleVersionIdString((BoundModuleVersionIdString)node, arg),
                BoundKind.SourceDocumentIndex => VisitSourceDocumentIndex((BoundSourceDocumentIndex)node, arg),
                BoundKind.MethodInfo => VisitMethodInfo((BoundMethodInfo)node, arg),
                BoundKind.FieldInfo => VisitFieldInfo((BoundFieldInfo)node, arg),
                BoundKind.DefaultLiteral => VisitDefaultLiteral((BoundDefaultLiteral)node, arg),
                BoundKind.DefaultExpression => VisitDefaultExpression((BoundDefaultExpression)node, arg),
                BoundKind.IsOperator => VisitIsOperator((BoundIsOperator)node, arg),
                BoundKind.AsOperator => VisitAsOperator((BoundAsOperator)node, arg),
                BoundKind.SizeOfOperator => VisitSizeOfOperator((BoundSizeOfOperator)node, arg),
                BoundKind.Conversion => VisitConversion((BoundConversion)node, arg),
                BoundKind.ReadOnlySpanFromArray => VisitReadOnlySpanFromArray((BoundReadOnlySpanFromArray)node, arg),
                BoundKind.ArgList => VisitArgList((BoundArgList)node, arg),
                BoundKind.ArgListOperator => VisitArgListOperator((BoundArgListOperator)node, arg),
                BoundKind.FixedLocalCollectionInitializer => VisitFixedLocalCollectionInitializer((BoundFixedLocalCollectionInitializer)node, arg),
                BoundKind.SequencePoint => VisitSequencePoint((BoundSequencePoint)node, arg),
                BoundKind.SequencePointWithSpan => VisitSequencePointWithSpan((BoundSequencePointWithSpan)node, arg),
                BoundKind.SavePreviousSequencePoint => VisitSavePreviousSequencePoint((BoundSavePreviousSequencePoint)node, arg),
                BoundKind.RestorePreviousSequencePoint => VisitRestorePreviousSequencePoint((BoundRestorePreviousSequencePoint)node, arg),
                BoundKind.StepThroughSequencePoint => VisitStepThroughSequencePoint((BoundStepThroughSequencePoint)node, arg),
                BoundKind.Block => VisitBlock((BoundBlock)node, arg),
                BoundKind.Scope => VisitScope((BoundScope)node, arg),
                BoundKind.StateMachineScope => VisitStateMachineScope((BoundStateMachineScope)node, arg),
                BoundKind.LocalDeclaration => VisitLocalDeclaration((BoundLocalDeclaration)node, arg),
                BoundKind.MultipleLocalDeclarations => VisitMultipleLocalDeclarations((BoundMultipleLocalDeclarations)node, arg),
                BoundKind.UsingLocalDeclarations => VisitUsingLocalDeclarations((BoundUsingLocalDeclarations)node, arg),
                BoundKind.LocalFunctionStatement => VisitLocalFunctionStatement((BoundLocalFunctionStatement)node, arg),
                BoundKind.NoOpStatement => VisitNoOpStatement((BoundNoOpStatement)node, arg),
                BoundKind.ReturnStatement => VisitReturnStatement((BoundReturnStatement)node, arg),
                BoundKind.YieldReturnStatement => VisitYieldReturnStatement((BoundYieldReturnStatement)node, arg),
                BoundKind.YieldBreakStatement => VisitYieldBreakStatement((BoundYieldBreakStatement)node, arg),
                BoundKind.ThrowStatement => VisitThrowStatement((BoundThrowStatement)node, arg),
                BoundKind.ExpressionStatement => VisitExpressionStatement((BoundExpressionStatement)node, arg),
                BoundKind.BreakStatement => VisitBreakStatement((BoundBreakStatement)node, arg),
                BoundKind.ContinueStatement => VisitContinueStatement((BoundContinueStatement)node, arg),
                BoundKind.SwitchStatement => VisitSwitchStatement((BoundSwitchStatement)node, arg),
                BoundKind.SwitchDispatch => VisitSwitchDispatch((BoundSwitchDispatch)node, arg),
                BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)node, arg),
                BoundKind.DoStatement => VisitDoStatement((BoundDoStatement)node, arg),
                BoundKind.WhileStatement => VisitWhileStatement((BoundWhileStatement)node, arg),
                BoundKind.ForStatement => VisitForStatement((BoundForStatement)node, arg),
                BoundKind.ForEachStatement => VisitForEachStatement((BoundForEachStatement)node, arg),
                BoundKind.ForEachDeconstructStep => VisitForEachDeconstructStep((BoundForEachDeconstructStep)node, arg),
                BoundKind.UsingStatement => VisitUsingStatement((BoundUsingStatement)node, arg),
                BoundKind.FixedStatement => VisitFixedStatement((BoundFixedStatement)node, arg),
                BoundKind.LockStatement => VisitLockStatement((BoundLockStatement)node, arg),
                BoundKind.TryStatement => VisitTryStatement((BoundTryStatement)node, arg),
                BoundKind.CatchBlock => VisitCatchBlock((BoundCatchBlock)node, arg),
                BoundKind.Literal => VisitLiteral((BoundLiteral)node, arg),
                BoundKind.ThisReference => VisitThisReference((BoundThisReference)node, arg),
                BoundKind.PreviousSubmissionReference => VisitPreviousSubmissionReference((BoundPreviousSubmissionReference)node, arg),
                BoundKind.HostObjectMemberReference => VisitHostObjectMemberReference((BoundHostObjectMemberReference)node, arg),
                BoundKind.BaseReference => VisitBaseReference((BoundBaseReference)node, arg),
                BoundKind.Local => VisitLocal((BoundLocal)node, arg),
                BoundKind.PseudoVariable => VisitPseudoVariable((BoundPseudoVariable)node, arg),
                BoundKind.RangeVariable => VisitRangeVariable((BoundRangeVariable)node, arg),
                BoundKind.Parameter => VisitParameter((BoundParameter)node, arg),
                BoundKind.LabelStatement => VisitLabelStatement((BoundLabelStatement)node, arg),
                BoundKind.GotoStatement => VisitGotoStatement((BoundGotoStatement)node, arg),
                BoundKind.LabeledStatement => VisitLabeledStatement((BoundLabeledStatement)node, arg),
                BoundKind.Label => VisitLabel((BoundLabel)node, arg),
                BoundKind.StatementList => VisitStatementList((BoundStatementList)node, arg),
                BoundKind.ConditionalGoto => VisitConditionalGoto((BoundConditionalGoto)node, arg),
                BoundKind.SwitchExpressionArm => VisitSwitchExpressionArm((BoundSwitchExpressionArm)node, arg),
                BoundKind.UnconvertedSwitchExpression => VisitUnconvertedSwitchExpression((BoundUnconvertedSwitchExpression)node, arg),
                BoundKind.ConvertedSwitchExpression => VisitConvertedSwitchExpression((BoundConvertedSwitchExpression)node, arg),
                BoundKind.DecisionDag => VisitDecisionDag((BoundDecisionDag)node, arg),
                BoundKind.EvaluationDecisionDagNode => VisitEvaluationDecisionDagNode((BoundEvaluationDecisionDagNode)node, arg),
                BoundKind.TestDecisionDagNode => VisitTestDecisionDagNode((BoundTestDecisionDagNode)node, arg),
                BoundKind.WhenDecisionDagNode => VisitWhenDecisionDagNode((BoundWhenDecisionDagNode)node, arg),
                BoundKind.LeafDecisionDagNode => VisitLeafDecisionDagNode((BoundLeafDecisionDagNode)node, arg),
                BoundKind.DagTemp => VisitDagTemp((BoundDagTemp)node, arg),
                BoundKind.DagTypeTest => VisitDagTypeTest((BoundDagTypeTest)node, arg),
                BoundKind.DagNonNullTest => VisitDagNonNullTest((BoundDagNonNullTest)node, arg),
                BoundKind.DagExplicitNullTest => VisitDagExplicitNullTest((BoundDagExplicitNullTest)node, arg),
                BoundKind.DagValueTest => VisitDagValueTest((BoundDagValueTest)node, arg),
                BoundKind.DagRelationalTest => VisitDagRelationalTest((BoundDagRelationalTest)node, arg),
                BoundKind.DagDeconstructEvaluation => VisitDagDeconstructEvaluation((BoundDagDeconstructEvaluation)node, arg),
                BoundKind.DagTypeEvaluation => VisitDagTypeEvaluation((BoundDagTypeEvaluation)node, arg),
                BoundKind.DagFieldEvaluation => VisitDagFieldEvaluation((BoundDagFieldEvaluation)node, arg),
                BoundKind.DagPropertyEvaluation => VisitDagPropertyEvaluation((BoundDagPropertyEvaluation)node, arg),
                BoundKind.DagIndexEvaluation => VisitDagIndexEvaluation((BoundDagIndexEvaluation)node, arg),
                BoundKind.SwitchSection => VisitSwitchSection((BoundSwitchSection)node, arg),
                BoundKind.SwitchLabel => VisitSwitchLabel((BoundSwitchLabel)node, arg),
                BoundKind.SequencePointExpression => VisitSequencePointExpression((BoundSequencePointExpression)node, arg),
                BoundKind.Sequence => VisitSequence((BoundSequence)node, arg),
                BoundKind.SpillSequence => VisitSpillSequence((BoundSpillSequence)node, arg),
                BoundKind.DynamicMemberAccess => VisitDynamicMemberAccess((BoundDynamicMemberAccess)node, arg),
                BoundKind.DynamicInvocation => VisitDynamicInvocation((BoundDynamicInvocation)node, arg),
                BoundKind.ConditionalAccess => VisitConditionalAccess((BoundConditionalAccess)node, arg),
                BoundKind.LoweredConditionalAccess => VisitLoweredConditionalAccess((BoundLoweredConditionalAccess)node, arg),
                BoundKind.ConditionalReceiver => VisitConditionalReceiver((BoundConditionalReceiver)node, arg),
                BoundKind.ComplexConditionalReceiver => VisitComplexConditionalReceiver((BoundComplexConditionalReceiver)node, arg),
                BoundKind.MethodGroup => VisitMethodGroup((BoundMethodGroup)node, arg),
                BoundKind.PropertyGroup => VisitPropertyGroup((BoundPropertyGroup)node, arg),
                BoundKind.Call => VisitCall((BoundCall)node, arg),
                BoundKind.EventAssignmentOperator => VisitEventAssignmentOperator((BoundEventAssignmentOperator)node, arg),
                BoundKind.Attribute => VisitAttribute((BoundAttribute)node, arg),
                BoundKind.UnconvertedObjectCreationExpression => VisitUnconvertedObjectCreationExpression((BoundUnconvertedObjectCreationExpression)node, arg),
                BoundKind.ObjectCreationExpression => VisitObjectCreationExpression((BoundObjectCreationExpression)node, arg),
                BoundKind.TupleLiteral => VisitTupleLiteral((BoundTupleLiteral)node, arg),
                BoundKind.ConvertedTupleLiteral => VisitConvertedTupleLiteral((BoundConvertedTupleLiteral)node, arg),
                BoundKind.DynamicObjectCreationExpression => VisitDynamicObjectCreationExpression((BoundDynamicObjectCreationExpression)node, arg),
                BoundKind.NoPiaObjectCreationExpression => VisitNoPiaObjectCreationExpression((BoundNoPiaObjectCreationExpression)node, arg),
                BoundKind.ObjectInitializerExpression => VisitObjectInitializerExpression((BoundObjectInitializerExpression)node, arg),
                BoundKind.ObjectInitializerMember => VisitObjectInitializerMember((BoundObjectInitializerMember)node, arg),
                BoundKind.DynamicObjectInitializerMember => VisitDynamicObjectInitializerMember((BoundDynamicObjectInitializerMember)node, arg),
                BoundKind.CollectionInitializerExpression => VisitCollectionInitializerExpression((BoundCollectionInitializerExpression)node, arg),
                BoundKind.CollectionElementInitializer => VisitCollectionElementInitializer((BoundCollectionElementInitializer)node, arg),
                BoundKind.DynamicCollectionElementInitializer => VisitDynamicCollectionElementInitializer((BoundDynamicCollectionElementInitializer)node, arg),
                BoundKind.ImplicitReceiver => VisitImplicitReceiver((BoundImplicitReceiver)node, arg),
                BoundKind.AnonymousObjectCreationExpression => VisitAnonymousObjectCreationExpression((BoundAnonymousObjectCreationExpression)node, arg),
                BoundKind.AnonymousPropertyDeclaration => VisitAnonymousPropertyDeclaration((BoundAnonymousPropertyDeclaration)node, arg),
                BoundKind.NewT => VisitNewT((BoundNewT)node, arg),
                BoundKind.DelegateCreationExpression => VisitDelegateCreationExpression((BoundDelegateCreationExpression)node, arg),
                BoundKind.ArrayCreation => VisitArrayCreation((BoundArrayCreation)node, arg),
                BoundKind.ArrayInitialization => VisitArrayInitialization((BoundArrayInitialization)node, arg),
                BoundKind.StackAllocArrayCreation => VisitStackAllocArrayCreation((BoundStackAllocArrayCreation)node, arg),
                BoundKind.ConvertedStackAllocExpression => VisitConvertedStackAllocExpression((BoundConvertedStackAllocExpression)node, arg),
                BoundKind.FieldAccess => VisitFieldAccess((BoundFieldAccess)node, arg),
                BoundKind.HoistedFieldAccess => VisitHoistedFieldAccess((BoundHoistedFieldAccess)node, arg),
                BoundKind.PropertyAccess => VisitPropertyAccess((BoundPropertyAccess)node, arg),
                BoundKind.EventAccess => VisitEventAccess((BoundEventAccess)node, arg),
                BoundKind.IndexerAccess => VisitIndexerAccess((BoundIndexerAccess)node, arg),
                BoundKind.IndexOrRangePatternIndexerAccess => VisitIndexOrRangePatternIndexerAccess((BoundIndexOrRangePatternIndexerAccess)node, arg),
                BoundKind.DynamicIndexerAccess => VisitDynamicIndexerAccess((BoundDynamicIndexerAccess)node, arg),
                BoundKind.Lambda => VisitLambda((BoundLambda)node, arg),
                BoundKind.UnboundLambda => VisitUnboundLambda((UnboundLambda)node, arg),
                BoundKind.QueryClause => VisitQueryClause((BoundQueryClause)node, arg),
                BoundKind.TypeOrInstanceInitializers => VisitTypeOrInstanceInitializers((BoundTypeOrInstanceInitializers)node, arg),
                BoundKind.NameOfOperator => VisitNameOfOperator((BoundNameOfOperator)node, arg),
                BoundKind.UnconvertedInterpolatedString => VisitUnconvertedInterpolatedString((BoundUnconvertedInterpolatedString)node, arg),
                BoundKind.InterpolatedString => VisitInterpolatedString((BoundInterpolatedString)node, arg),
                BoundKind.StringInsert => VisitStringInsert((BoundStringInsert)node, arg),
                BoundKind.IsPatternExpression => VisitIsPatternExpression((BoundIsPatternExpression)node, arg),
                BoundKind.ConstantPattern => VisitConstantPattern((BoundConstantPattern)node, arg),
                BoundKind.DiscardPattern => VisitDiscardPattern((BoundDiscardPattern)node, arg),
                BoundKind.DeclarationPattern => VisitDeclarationPattern((BoundDeclarationPattern)node, arg),
                BoundKind.RecursivePattern => VisitRecursivePattern((BoundRecursivePattern)node, arg),
                BoundKind.ITuplePattern => VisitITuplePattern((BoundITuplePattern)node, arg),
                BoundKind.Subpattern => VisitSubpattern((BoundSubpattern)node, arg),
                BoundKind.TypePattern => VisitTypePattern((BoundTypePattern)node, arg),
                BoundKind.BinaryPattern => VisitBinaryPattern((BoundBinaryPattern)node, arg),
                BoundKind.NegatedPattern => VisitNegatedPattern((BoundNegatedPattern)node, arg),
                BoundKind.RelationalPattern => VisitRelationalPattern((BoundRelationalPattern)node, arg),
                BoundKind.DiscardExpression => VisitDiscardExpression((BoundDiscardExpression)node, arg),
                BoundKind.ThrowExpression => VisitThrowExpression((BoundThrowExpression)node, arg),
                BoundKind.OutVariablePendingInference => VisitOutVariablePendingInference((OutVariablePendingInference)node, arg),
                BoundKind.DeconstructionVariablePendingInference => VisitDeconstructionVariablePendingInference((DeconstructionVariablePendingInference)node, arg),
                BoundKind.OutDeconstructVarPendingInference => VisitOutDeconstructVarPendingInference((OutDeconstructVarPendingInference)node, arg),
                BoundKind.NonConstructorMethodBody => VisitNonConstructorMethodBody((BoundNonConstructorMethodBody)node, arg),
                BoundKind.ConstructorMethodBody => VisitConstructorMethodBody((BoundConstructorMethodBody)node, arg),
                BoundKind.ExpressionWithNullability => VisitExpressionWithNullability((BoundExpressionWithNullability)node, arg),
                BoundKind.WithExpression => VisitWithExpression((BoundWithExpression)node, arg),
                _ => default(R),
            };
        }

        public virtual R VisitFieldEqualsValue(BoundFieldEqualsValue node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPropertyEqualsValue(BoundPropertyEqualsValue node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitParameterEqualsValue(BoundParameterEqualsValue node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDup(BoundDup node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPassByCopy(BoundPassByCopy node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBadExpression(BoundBadExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBadStatement(BoundBadStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTypeExpression(BoundTypeExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTypeOrValueExpression(BoundTypeOrValueExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNamespaceExpression(BoundNamespaceExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnaryOperator(BoundUnaryOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIncrementOperator(BoundIncrementOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAddressOfOperator(BoundAddressOfOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFunctionPointerLoad(BoundFunctionPointerLoad node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPointerElementAccess(BoundPointerElementAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRefTypeOperator(BoundRefTypeOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMakeRefOperator(BoundMakeRefOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRefValueOperator(BoundRefValueOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFromEndIndexExpression(BoundFromEndIndexExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRangeExpression(BoundRangeExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBinaryOperator(BoundBinaryOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTupleBinaryOperator(BoundTupleBinaryOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAssignmentOperator(BoundAssignmentOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNullCoalescingOperator(BoundNullCoalescingOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConditionalOperator(BoundConditionalOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArrayAccess(BoundArrayAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArrayLength(BoundArrayLength node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAwaitableInfo(BoundAwaitableInfo node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAwaitExpression(BoundAwaitExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTypeOfOperator(BoundTypeOfOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMethodDefIndex(BoundMethodDefIndex node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitModuleVersionId(BoundModuleVersionId node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitModuleVersionIdString(BoundModuleVersionIdString node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSourceDocumentIndex(BoundSourceDocumentIndex node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMethodInfo(BoundMethodInfo node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFieldInfo(BoundFieldInfo node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDefaultLiteral(BoundDefaultLiteral node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDefaultExpression(BoundDefaultExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIsOperator(BoundIsOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAsOperator(BoundAsOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSizeOfOperator(BoundSizeOfOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConversion(BoundConversion node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArgList(BoundArgList node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArgListOperator(BoundArgListOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSequencePoint(BoundSequencePoint node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSequencePointWithSpan(BoundSequencePointWithSpan node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBlock(BoundBlock node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitScope(BoundScope node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitStateMachineScope(BoundStateMachineScope node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLocalDeclaration(BoundLocalDeclaration node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLocalFunctionStatement(BoundLocalFunctionStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNoOpStatement(BoundNoOpStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitReturnStatement(BoundReturnStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitYieldReturnStatement(BoundYieldReturnStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitYieldBreakStatement(BoundYieldBreakStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitThrowStatement(BoundThrowStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitExpressionStatement(BoundExpressionStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBreakStatement(BoundBreakStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitContinueStatement(BoundContinueStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSwitchStatement(BoundSwitchStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSwitchDispatch(BoundSwitchDispatch node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIfStatement(BoundIfStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDoStatement(BoundDoStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitWhileStatement(BoundWhileStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitForStatement(BoundForStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitForEachStatement(BoundForEachStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitForEachDeconstructStep(BoundForEachDeconstructStep node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUsingStatement(BoundUsingStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFixedStatement(BoundFixedStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLockStatement(BoundLockStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTryStatement(BoundTryStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitCatchBlock(BoundCatchBlock node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLiteral(BoundLiteral node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitThisReference(BoundThisReference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitHostObjectMemberReference(BoundHostObjectMemberReference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBaseReference(BoundBaseReference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLocal(BoundLocal node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPseudoVariable(BoundPseudoVariable node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRangeVariable(BoundRangeVariable node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitParameter(BoundParameter node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLabelStatement(BoundLabelStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitGotoStatement(BoundGotoStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLabeledStatement(BoundLabeledStatement node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLabel(BoundLabel node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitStatementList(BoundStatementList node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConditionalGoto(BoundConditionalGoto node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSwitchExpressionArm(BoundSwitchExpressionArm node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDecisionDag(BoundDecisionDag node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTestDecisionDagNode(BoundTestDecisionDagNode node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagTemp(BoundDagTemp node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagTypeTest(BoundDagTypeTest node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagNonNullTest(BoundDagNonNullTest node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagExplicitNullTest(BoundDagExplicitNullTest node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagValueTest(BoundDagValueTest node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagRelationalTest(BoundDagRelationalTest node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagTypeEvaluation(BoundDagTypeEvaluation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagFieldEvaluation(BoundDagFieldEvaluation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDagIndexEvaluation(BoundDagIndexEvaluation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSwitchSection(BoundSwitchSection node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSwitchLabel(BoundSwitchLabel node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSequencePointExpression(BoundSequencePointExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSequence(BoundSequence node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSpillSequence(BoundSpillSequence node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicMemberAccess(BoundDynamicMemberAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicInvocation(BoundDynamicInvocation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConditionalAccess(BoundConditionalAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConditionalReceiver(BoundConditionalReceiver node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitMethodGroup(BoundMethodGroup node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPropertyGroup(BoundPropertyGroup node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitCall(BoundCall node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitEventAssignmentOperator(BoundEventAssignmentOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAttribute(BoundAttribute node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitObjectCreationExpression(BoundObjectCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTupleLiteral(BoundTupleLiteral node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitObjectInitializerExpression(BoundObjectInitializerExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitObjectInitializerMember(BoundObjectInitializerMember node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitCollectionElementInitializer(BoundCollectionElementInitializer node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitImplicitReceiver(BoundImplicitReceiver node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNewT(BoundNewT node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDelegateCreationExpression(BoundDelegateCreationExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArrayCreation(BoundArrayCreation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitArrayInitialization(BoundArrayInitialization node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitFieldAccess(BoundFieldAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitHoistedFieldAccess(BoundHoistedFieldAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitPropertyAccess(BoundPropertyAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitEventAccess(BoundEventAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIndexerAccess(BoundIndexerAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitLambda(BoundLambda node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnboundLambda(UnboundLambda node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitQueryClause(BoundQueryClause node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNameOfOperator(BoundNameOfOperator node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitInterpolatedString(BoundInterpolatedString node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitStringInsert(BoundStringInsert node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitIsPatternExpression(BoundIsPatternExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConstantPattern(BoundConstantPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDiscardPattern(BoundDiscardPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDeclarationPattern(BoundDeclarationPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRecursivePattern(BoundRecursivePattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitITuplePattern(BoundITuplePattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitSubpattern(BoundSubpattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitTypePattern(BoundTypePattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitBinaryPattern(BoundBinaryPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNegatedPattern(BoundNegatedPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitRelationalPattern(BoundRelationalPattern node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDiscardExpression(BoundDiscardExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitThrowExpression(BoundThrowExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitOutVariablePendingInference(OutVariablePendingInference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitConstructorMethodBody(BoundConstructorMethodBody node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitExpressionWithNullability(BoundExpressionWithNullability node, A arg)
        {
            return DefaultVisit(node, arg);
        }

        public virtual R VisitWithExpression(BoundWithExpression node, A arg)
        {
            return DefaultVisit(node, arg);
        }
    }
    public abstract class BoundTreeVisitor
    {
        public class CancelledByStackGuardException : Exception
        {
            public readonly BoundNode Node;

            public CancelledByStackGuardException(Exception inner, BoundNode node)
                : base(inner.Message, inner)
            {
                Node = node;
            }

            public void AddAnError(DiagnosticBag diagnostics)
            {
                diagnostics.Add(ErrorCode.ERR_InsufficientStack, GetTooLongOrComplexExpressionErrorLocation(Node));
            }

            public void AddAnError(BindingDiagnosticBag diagnostics)
            {
                diagnostics.Add(ErrorCode.ERR_InsufficientStack, GetTooLongOrComplexExpressionErrorLocation(Node));
            }

            public static Location GetTooLongOrComplexExpressionErrorLocation(BoundNode node)
            {
                SyntaxNode syntaxNode = node.Syntax;
                if (!(syntaxNode is ExpressionSyntax))
                {
                    syntaxNode = syntaxNode.DescendantNodes((SyntaxNode n) => !(n is ExpressionSyntax)).OfType<ExpressionSyntax>().FirstOrDefault() ?? syntaxNode;
                }
                return syntaxNode.GetFirstToken().GetLocation();
            }
        }

        [DebuggerHidden]
        public virtual BoundNode Visit(BoundNode node)
        {
            return node?.Accept(this);
        }

        [DebuggerHidden]
        public virtual BoundNode DefaultVisit(BoundNode node)
        {
            return null;
        }

        [DebuggerStepThrough]
        protected BoundExpression VisitExpressionWithStackGuard(ref int recursionDepth, BoundExpression node)
        {
            recursionDepth++;
            BoundExpression result;
            if (recursionDepth > 1 || !ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException())
            {
                EnsureSufficientExecutionStack(recursionDepth);
                result = VisitExpressionWithoutStackGuard(node);
            }
            else
            {
                result = VisitExpressionWithStackGuard(node);
            }
            recursionDepth--;
            return result;
        }

        protected virtual void EnsureSufficientExecutionStack(int recursionDepth)
        {
            StackGuard.EnsureSufficientExecutionStack(recursionDepth);
        }

        protected virtual bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
        {
            return true;
        }

        [DebuggerStepThrough]
        private BoundExpression? VisitExpressionWithStackGuard(BoundExpression node)
        {
            try
            {
                return VisitExpressionWithoutStackGuard(node);
            }
            catch (InsufficientExecutionStackException inner)
            {
                throw new CancelledByStackGuardException(inner, node);
            }
        }

        protected abstract BoundExpression? VisitExpressionWithoutStackGuard(BoundExpression node);

        public virtual BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDup(BoundDup node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPassByCopy(BoundPassByCopy node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBadStatement(BoundBadStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBinaryOperator(BoundBinaryOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConditionalOperator(BoundConditionalOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArrayLength(BoundArrayLength node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitModuleVersionId(BoundModuleVersionId node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMethodInfo(BoundMethodInfo node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFieldInfo(BoundFieldInfo node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDefaultExpression(BoundDefaultExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConversion(BoundConversion node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArgList(BoundArgList node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSequencePoint(BoundSequencePoint node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBlock(BoundBlock node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitScope(BoundScope node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitStateMachineScope(BoundStateMachineScope node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNoOpStatement(BoundNoOpStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitReturnStatement(BoundReturnStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitYieldBreakStatement(BoundYieldBreakStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitThrowStatement(BoundThrowStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitExpressionStatement(BoundExpressionStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBreakStatement(BoundBreakStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitContinueStatement(BoundContinueStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSwitchStatement(BoundSwitchStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIfStatement(BoundIfStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitForStatement(BoundForStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitForEachDeconstructStep(BoundForEachDeconstructStep node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLockStatement(BoundLockStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTryStatement(BoundTryStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLiteral(BoundLiteral node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitThisReference(BoundThisReference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBaseReference(BoundBaseReference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLocal(BoundLocal node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPseudoVariable(BoundPseudoVariable node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitParameter(BoundParameter node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLabelStatement(BoundLabelStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitGotoStatement(BoundGotoStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLabeledStatement(BoundLabeledStatement node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLabel(BoundLabel node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitStatementList(BoundStatementList node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConditionalGoto(BoundConditionalGoto node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDecisionDag(BoundDecisionDag node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTestDecisionDagNode(BoundTestDecisionDagNode node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagTemp(BoundDagTemp node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagTypeTest(BoundDagTypeTest node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagNonNullTest(BoundDagNonNullTest node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagExplicitNullTest(BoundDagExplicitNullTest node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagValueTest(BoundDagValueTest node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagRelationalTest(BoundDagRelationalTest node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagTypeEvaluation(BoundDagTypeEvaluation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSwitchSection(BoundSwitchSection node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSwitchLabel(BoundSwitchLabel node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSequence(BoundSequence node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSpillSequence(BoundSpillSequence node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPropertyGroup(BoundPropertyGroup node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitCall(BoundCall node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAttribute(BoundAttribute node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNewT(BoundNewT node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitLambda(BoundLambda node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnboundLambda(UnboundLambda node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConstantPattern(BoundConstantPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDiscardPattern(BoundDiscardPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRecursivePattern(BoundRecursivePattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitITuplePattern(BoundITuplePattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitSubpattern(BoundSubpattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitTypePattern(BoundTypePattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitBinaryPattern(BoundBinaryPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNegatedPattern(BoundNegatedPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitRelationalPattern(BoundRelationalPattern node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDiscardExpression(BoundDiscardExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            return DefaultVisit(node);
        }

        public virtual BoundNode? VisitWithExpression(BoundWithExpression node)
        {
            return DefaultVisit(node);
        }
    }
}
