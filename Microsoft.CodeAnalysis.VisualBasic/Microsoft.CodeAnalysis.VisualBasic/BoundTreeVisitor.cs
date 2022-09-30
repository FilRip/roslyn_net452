using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTreeVisitor<A, R>
	{
		public virtual R Visit(BoundNode node, A arg)
		{
			if (node == null)
			{
				return default(R);
			}
			return node.Kind switch
			{
				BoundKind.OmittedArgument => VisitOmittedArgument((BoundOmittedArgument)node, arg), 
				BoundKind.Parenthesized => VisitParenthesized((BoundParenthesized)node, arg), 
				BoundKind.ArrayAccess => VisitArrayAccess((BoundArrayAccess)node, arg), 
				BoundKind.TypeExpression => VisitTypeExpression((BoundTypeExpression)node, arg), 
				BoundKind.NamespaceExpression => VisitNamespaceExpression((BoundNamespaceExpression)node, arg), 
				BoundKind.UnaryOperator => VisitUnaryOperator((BoundUnaryOperator)node, arg), 
				BoundKind.BinaryOperator => VisitBinaryOperator((BoundBinaryOperator)node, arg), 
				BoundKind.AssignmentOperator => VisitAssignmentOperator((BoundAssignmentOperator)node, arg), 
				BoundKind.TernaryConditionalExpression => VisitTernaryConditionalExpression((BoundTernaryConditionalExpression)node, arg), 
				BoundKind.BinaryConditionalExpression => VisitBinaryConditionalExpression((BoundBinaryConditionalExpression)node, arg), 
				BoundKind.Conversion => VisitConversion((BoundConversion)node, arg), 
				BoundKind.DirectCast => VisitDirectCast((BoundDirectCast)node, arg), 
				BoundKind.TryCast => VisitTryCast((BoundTryCast)node, arg), 
				BoundKind.TypeOf => VisitTypeOf((BoundTypeOf)node, arg), 
				BoundKind.SequencePoint => VisitSequencePoint((BoundSequencePoint)node, arg), 
				BoundKind.SequencePointWithSpan => VisitSequencePointWithSpan((BoundSequencePointWithSpan)node, arg), 
				BoundKind.NoOpStatement => VisitNoOpStatement((BoundNoOpStatement)node, arg), 
				BoundKind.MethodGroup => VisitMethodGroup((BoundMethodGroup)node, arg), 
				BoundKind.PropertyGroup => VisitPropertyGroup((BoundPropertyGroup)node, arg), 
				BoundKind.ReturnStatement => VisitReturnStatement((BoundReturnStatement)node, arg), 
				BoundKind.Call => VisitCall((BoundCall)node, arg), 
				BoundKind.ObjectCreationExpression => VisitObjectCreationExpression((BoundObjectCreationExpression)node, arg), 
				BoundKind.DelegateCreationExpression => VisitDelegateCreationExpression((BoundDelegateCreationExpression)node, arg), 
				BoundKind.FieldAccess => VisitFieldAccess((BoundFieldAccess)node, arg), 
				BoundKind.PropertyAccess => VisitPropertyAccess((BoundPropertyAccess)node, arg), 
				BoundKind.Block => VisitBlock((BoundBlock)node, arg), 
				BoundKind.LocalDeclaration => VisitLocalDeclaration((BoundLocalDeclaration)node, arg), 
				BoundKind.FieldInitializer => VisitFieldInitializer((BoundFieldInitializer)node, arg), 
				BoundKind.PropertyInitializer => VisitPropertyInitializer((BoundPropertyInitializer)node, arg), 
				BoundKind.Sequence => VisitSequence((BoundSequence)node, arg), 
				BoundKind.ExpressionStatement => VisitExpressionStatement((BoundExpressionStatement)node, arg), 
				BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)node, arg), 
				BoundKind.ForToStatement => VisitForToStatement((BoundForToStatement)node, arg), 
				BoundKind.ExitStatement => VisitExitStatement((BoundExitStatement)node, arg), 
				BoundKind.ContinueStatement => VisitContinueStatement((BoundContinueStatement)node, arg), 
				BoundKind.TryStatement => VisitTryStatement((BoundTryStatement)node, arg), 
				BoundKind.CatchBlock => VisitCatchBlock((BoundCatchBlock)node, arg), 
				BoundKind.Literal => VisitLiteral((BoundLiteral)node, arg), 
				BoundKind.MeReference => VisitMeReference((BoundMeReference)node, arg), 
				BoundKind.Local => VisitLocal((BoundLocal)node, arg), 
				BoundKind.Parameter => VisitParameter((BoundParameter)node, arg), 
				BoundKind.ByRefArgumentPlaceholder => VisitByRefArgumentPlaceholder((BoundByRefArgumentPlaceholder)node, arg), 
				BoundKind.ByRefArgumentWithCopyBack => VisitByRefArgumentWithCopyBack((BoundByRefArgumentWithCopyBack)node, arg), 
				BoundKind.LabelStatement => VisitLabelStatement((BoundLabelStatement)node, arg), 
				BoundKind.GotoStatement => VisitGotoStatement((BoundGotoStatement)node, arg), 
				BoundKind.StatementList => VisitStatementList((BoundStatementList)node, arg), 
				BoundKind.ConditionalGoto => VisitConditionalGoto((BoundConditionalGoto)node, arg), 
				BoundKind.Lambda => VisitLambda((BoundLambda)node, arg), 
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
				BoundKind.TypeArguments => VisitTypeArguments((BoundTypeArguments)node, arg), 
				BoundKind.OmittedArgument => VisitOmittedArgument((BoundOmittedArgument)node, arg), 
				BoundKind.LValueToRValueWrapper => VisitLValueToRValueWrapper((BoundLValueToRValueWrapper)node, arg), 
				BoundKind.WithLValueExpressionPlaceholder => VisitWithLValueExpressionPlaceholder((BoundWithLValueExpressionPlaceholder)node, arg), 
				BoundKind.WithRValueExpressionPlaceholder => VisitWithRValueExpressionPlaceholder((BoundWithRValueExpressionPlaceholder)node, arg), 
				BoundKind.RValuePlaceholder => VisitRValuePlaceholder((BoundRValuePlaceholder)node, arg), 
				BoundKind.LValuePlaceholder => VisitLValuePlaceholder((BoundLValuePlaceholder)node, arg), 
				BoundKind.Dup => VisitDup((BoundDup)node, arg), 
				BoundKind.BadExpression => VisitBadExpression((BoundBadExpression)node, arg), 
				BoundKind.BadStatement => VisitBadStatement((BoundBadStatement)node, arg), 
				BoundKind.Parenthesized => VisitParenthesized((BoundParenthesized)node, arg), 
				BoundKind.BadVariable => VisitBadVariable((BoundBadVariable)node, arg), 
				BoundKind.ArrayAccess => VisitArrayAccess((BoundArrayAccess)node, arg), 
				BoundKind.ArrayLength => VisitArrayLength((BoundArrayLength)node, arg), 
				BoundKind.GetType => VisitGetType((BoundGetType)node, arg), 
				BoundKind.FieldInfo => VisitFieldInfo((BoundFieldInfo)node, arg), 
				BoundKind.MethodInfo => VisitMethodInfo((BoundMethodInfo)node, arg), 
				BoundKind.TypeExpression => VisitTypeExpression((BoundTypeExpression)node, arg), 
				BoundKind.TypeOrValueExpression => VisitTypeOrValueExpression((BoundTypeOrValueExpression)node, arg), 
				BoundKind.NamespaceExpression => VisitNamespaceExpression((BoundNamespaceExpression)node, arg), 
				BoundKind.MethodDefIndex => VisitMethodDefIndex((BoundMethodDefIndex)node, arg), 
				BoundKind.MaximumMethodDefIndex => VisitMaximumMethodDefIndex((BoundMaximumMethodDefIndex)node, arg), 
				BoundKind.InstrumentationPayloadRoot => VisitInstrumentationPayloadRoot((BoundInstrumentationPayloadRoot)node, arg), 
				BoundKind.ModuleVersionId => VisitModuleVersionId((BoundModuleVersionId)node, arg), 
				BoundKind.ModuleVersionIdString => VisitModuleVersionIdString((BoundModuleVersionIdString)node, arg), 
				BoundKind.SourceDocumentIndex => VisitSourceDocumentIndex((BoundSourceDocumentIndex)node, arg), 
				BoundKind.UnaryOperator => VisitUnaryOperator((BoundUnaryOperator)node, arg), 
				BoundKind.UserDefinedUnaryOperator => VisitUserDefinedUnaryOperator((BoundUserDefinedUnaryOperator)node, arg), 
				BoundKind.NullableIsTrueOperator => VisitNullableIsTrueOperator((BoundNullableIsTrueOperator)node, arg), 
				BoundKind.BinaryOperator => VisitBinaryOperator((BoundBinaryOperator)node, arg), 
				BoundKind.UserDefinedBinaryOperator => VisitUserDefinedBinaryOperator((BoundUserDefinedBinaryOperator)node, arg), 
				BoundKind.UserDefinedShortCircuitingOperator => VisitUserDefinedShortCircuitingOperator((BoundUserDefinedShortCircuitingOperator)node, arg), 
				BoundKind.CompoundAssignmentTargetPlaceholder => VisitCompoundAssignmentTargetPlaceholder((BoundCompoundAssignmentTargetPlaceholder)node, arg), 
				BoundKind.AssignmentOperator => VisitAssignmentOperator((BoundAssignmentOperator)node, arg), 
				BoundKind.ReferenceAssignment => VisitReferenceAssignment((BoundReferenceAssignment)node, arg), 
				BoundKind.AddressOfOperator => VisitAddressOfOperator((BoundAddressOfOperator)node, arg), 
				BoundKind.TernaryConditionalExpression => VisitTernaryConditionalExpression((BoundTernaryConditionalExpression)node, arg), 
				BoundKind.BinaryConditionalExpression => VisitBinaryConditionalExpression((BoundBinaryConditionalExpression)node, arg), 
				BoundKind.Conversion => VisitConversion((BoundConversion)node, arg), 
				BoundKind.RelaxationLambda => VisitRelaxationLambda((BoundRelaxationLambda)node, arg), 
				BoundKind.ConvertedTupleElements => VisitConvertedTupleElements((BoundConvertedTupleElements)node, arg), 
				BoundKind.UserDefinedConversion => VisitUserDefinedConversion((BoundUserDefinedConversion)node, arg), 
				BoundKind.DirectCast => VisitDirectCast((BoundDirectCast)node, arg), 
				BoundKind.TryCast => VisitTryCast((BoundTryCast)node, arg), 
				BoundKind.TypeOf => VisitTypeOf((BoundTypeOf)node, arg), 
				BoundKind.SequencePoint => VisitSequencePoint((BoundSequencePoint)node, arg), 
				BoundKind.SequencePointExpression => VisitSequencePointExpression((BoundSequencePointExpression)node, arg), 
				BoundKind.SequencePointWithSpan => VisitSequencePointWithSpan((BoundSequencePointWithSpan)node, arg), 
				BoundKind.NoOpStatement => VisitNoOpStatement((BoundNoOpStatement)node, arg), 
				BoundKind.MethodGroup => VisitMethodGroup((BoundMethodGroup)node, arg), 
				BoundKind.PropertyGroup => VisitPropertyGroup((BoundPropertyGroup)node, arg), 
				BoundKind.ReturnStatement => VisitReturnStatement((BoundReturnStatement)node, arg), 
				BoundKind.YieldStatement => VisitYieldStatement((BoundYieldStatement)node, arg), 
				BoundKind.ThrowStatement => VisitThrowStatement((BoundThrowStatement)node, arg), 
				BoundKind.RedimStatement => VisitRedimStatement((BoundRedimStatement)node, arg), 
				BoundKind.RedimClause => VisitRedimClause((BoundRedimClause)node, arg), 
				BoundKind.EraseStatement => VisitEraseStatement((BoundEraseStatement)node, arg), 
				BoundKind.Call => VisitCall((BoundCall)node, arg), 
				BoundKind.Attribute => VisitAttribute((BoundAttribute)node, arg), 
				BoundKind.LateMemberAccess => VisitLateMemberAccess((BoundLateMemberAccess)node, arg), 
				BoundKind.LateInvocation => VisitLateInvocation((BoundLateInvocation)node, arg), 
				BoundKind.LateAddressOfOperator => VisitLateAddressOfOperator((BoundLateAddressOfOperator)node, arg), 
				BoundKind.TupleLiteral => VisitTupleLiteral((BoundTupleLiteral)node, arg), 
				BoundKind.ConvertedTupleLiteral => VisitConvertedTupleLiteral((BoundConvertedTupleLiteral)node, arg), 
				BoundKind.ObjectCreationExpression => VisitObjectCreationExpression((BoundObjectCreationExpression)node, arg), 
				BoundKind.NoPiaObjectCreationExpression => VisitNoPiaObjectCreationExpression((BoundNoPiaObjectCreationExpression)node, arg), 
				BoundKind.AnonymousTypeCreationExpression => VisitAnonymousTypeCreationExpression((BoundAnonymousTypeCreationExpression)node, arg), 
				BoundKind.AnonymousTypePropertyAccess => VisitAnonymousTypePropertyAccess((BoundAnonymousTypePropertyAccess)node, arg), 
				BoundKind.AnonymousTypeFieldInitializer => VisitAnonymousTypeFieldInitializer((BoundAnonymousTypeFieldInitializer)node, arg), 
				BoundKind.ObjectInitializerExpression => VisitObjectInitializerExpression((BoundObjectInitializerExpression)node, arg), 
				BoundKind.CollectionInitializerExpression => VisitCollectionInitializerExpression((BoundCollectionInitializerExpression)node, arg), 
				BoundKind.NewT => VisitNewT((BoundNewT)node, arg), 
				BoundKind.DelegateCreationExpression => VisitDelegateCreationExpression((BoundDelegateCreationExpression)node, arg), 
				BoundKind.ArrayCreation => VisitArrayCreation((BoundArrayCreation)node, arg), 
				BoundKind.ArrayLiteral => VisitArrayLiteral((BoundArrayLiteral)node, arg), 
				BoundKind.ArrayInitialization => VisitArrayInitialization((BoundArrayInitialization)node, arg), 
				BoundKind.FieldAccess => VisitFieldAccess((BoundFieldAccess)node, arg), 
				BoundKind.PropertyAccess => VisitPropertyAccess((BoundPropertyAccess)node, arg), 
				BoundKind.EventAccess => VisitEventAccess((BoundEventAccess)node, arg), 
				BoundKind.Block => VisitBlock((BoundBlock)node, arg), 
				BoundKind.StateMachineScope => VisitStateMachineScope((BoundStateMachineScope)node, arg), 
				BoundKind.LocalDeclaration => VisitLocalDeclaration((BoundLocalDeclaration)node, arg), 
				BoundKind.AsNewLocalDeclarations => VisitAsNewLocalDeclarations((BoundAsNewLocalDeclarations)node, arg), 
				BoundKind.DimStatement => VisitDimStatement((BoundDimStatement)node, arg), 
				BoundKind.Initializer => VisitInitializer((BoundInitializer)node, arg), 
				BoundKind.FieldInitializer => VisitFieldInitializer((BoundFieldInitializer)node, arg), 
				BoundKind.PropertyInitializer => VisitPropertyInitializer((BoundPropertyInitializer)node, arg), 
				BoundKind.ParameterEqualsValue => VisitParameterEqualsValue((BoundParameterEqualsValue)node, arg), 
				BoundKind.GlobalStatementInitializer => VisitGlobalStatementInitializer((BoundGlobalStatementInitializer)node, arg), 
				BoundKind.Sequence => VisitSequence((BoundSequence)node, arg), 
				BoundKind.ExpressionStatement => VisitExpressionStatement((BoundExpressionStatement)node, arg), 
				BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)node, arg), 
				BoundKind.SelectStatement => VisitSelectStatement((BoundSelectStatement)node, arg), 
				BoundKind.CaseBlock => VisitCaseBlock((BoundCaseBlock)node, arg), 
				BoundKind.CaseStatement => VisitCaseStatement((BoundCaseStatement)node, arg), 
				BoundKind.SimpleCaseClause => VisitSimpleCaseClause((BoundSimpleCaseClause)node, arg), 
				BoundKind.RangeCaseClause => VisitRangeCaseClause((BoundRangeCaseClause)node, arg), 
				BoundKind.RelationalCaseClause => VisitRelationalCaseClause((BoundRelationalCaseClause)node, arg), 
				BoundKind.DoLoopStatement => VisitDoLoopStatement((BoundDoLoopStatement)node, arg), 
				BoundKind.WhileStatement => VisitWhileStatement((BoundWhileStatement)node, arg), 
				BoundKind.ForToUserDefinedOperators => VisitForToUserDefinedOperators((BoundForToUserDefinedOperators)node, arg), 
				BoundKind.ForToStatement => VisitForToStatement((BoundForToStatement)node, arg), 
				BoundKind.ForEachStatement => VisitForEachStatement((BoundForEachStatement)node, arg), 
				BoundKind.ExitStatement => VisitExitStatement((BoundExitStatement)node, arg), 
				BoundKind.ContinueStatement => VisitContinueStatement((BoundContinueStatement)node, arg), 
				BoundKind.TryStatement => VisitTryStatement((BoundTryStatement)node, arg), 
				BoundKind.CatchBlock => VisitCatchBlock((BoundCatchBlock)node, arg), 
				BoundKind.Literal => VisitLiteral((BoundLiteral)node, arg), 
				BoundKind.MeReference => VisitMeReference((BoundMeReference)node, arg), 
				BoundKind.ValueTypeMeReference => VisitValueTypeMeReference((BoundValueTypeMeReference)node, arg), 
				BoundKind.MyBaseReference => VisitMyBaseReference((BoundMyBaseReference)node, arg), 
				BoundKind.MyClassReference => VisitMyClassReference((BoundMyClassReference)node, arg), 
				BoundKind.PreviousSubmissionReference => VisitPreviousSubmissionReference((BoundPreviousSubmissionReference)node, arg), 
				BoundKind.HostObjectMemberReference => VisitHostObjectMemberReference((BoundHostObjectMemberReference)node, arg), 
				BoundKind.Local => VisitLocal((BoundLocal)node, arg), 
				BoundKind.PseudoVariable => VisitPseudoVariable((BoundPseudoVariable)node, arg), 
				BoundKind.Parameter => VisitParameter((BoundParameter)node, arg), 
				BoundKind.ByRefArgumentPlaceholder => VisitByRefArgumentPlaceholder((BoundByRefArgumentPlaceholder)node, arg), 
				BoundKind.ByRefArgumentWithCopyBack => VisitByRefArgumentWithCopyBack((BoundByRefArgumentWithCopyBack)node, arg), 
				BoundKind.LateBoundArgumentSupportingAssignmentWithCapture => VisitLateBoundArgumentSupportingAssignmentWithCapture((BoundLateBoundArgumentSupportingAssignmentWithCapture)node, arg), 
				BoundKind.LabelStatement => VisitLabelStatement((BoundLabelStatement)node, arg), 
				BoundKind.Label => VisitLabel((BoundLabel)node, arg), 
				BoundKind.GotoStatement => VisitGotoStatement((BoundGotoStatement)node, arg), 
				BoundKind.StatementList => VisitStatementList((BoundStatementList)node, arg), 
				BoundKind.ConditionalGoto => VisitConditionalGoto((BoundConditionalGoto)node, arg), 
				BoundKind.WithStatement => VisitWithStatement((BoundWithStatement)node, arg), 
				BoundKind.UnboundLambda => VisitUnboundLambda((UnboundLambda)node, arg), 
				BoundKind.Lambda => VisitLambda((BoundLambda)node, arg), 
				BoundKind.QueryExpression => VisitQueryExpression((BoundQueryExpression)node, arg), 
				BoundKind.QuerySource => VisitQuerySource((BoundQuerySource)node, arg), 
				BoundKind.ToQueryableCollectionConversion => VisitToQueryableCollectionConversion((BoundToQueryableCollectionConversion)node, arg), 
				BoundKind.QueryableSource => VisitQueryableSource((BoundQueryableSource)node, arg), 
				BoundKind.QueryClause => VisitQueryClause((BoundQueryClause)node, arg), 
				BoundKind.Ordering => VisitOrdering((BoundOrdering)node, arg), 
				BoundKind.QueryLambda => VisitQueryLambda((BoundQueryLambda)node, arg), 
				BoundKind.RangeVariableAssignment => VisitRangeVariableAssignment((BoundRangeVariableAssignment)node, arg), 
				BoundKind.GroupTypeInferenceLambda => VisitGroupTypeInferenceLambda((GroupTypeInferenceLambda)node, arg), 
				BoundKind.AggregateClause => VisitAggregateClause((BoundAggregateClause)node, arg), 
				BoundKind.GroupAggregation => VisitGroupAggregation((BoundGroupAggregation)node, arg), 
				BoundKind.RangeVariable => VisitRangeVariable((BoundRangeVariable)node, arg), 
				BoundKind.AddHandlerStatement => VisitAddHandlerStatement((BoundAddHandlerStatement)node, arg), 
				BoundKind.RemoveHandlerStatement => VisitRemoveHandlerStatement((BoundRemoveHandlerStatement)node, arg), 
				BoundKind.RaiseEventStatement => VisitRaiseEventStatement((BoundRaiseEventStatement)node, arg), 
				BoundKind.UsingStatement => VisitUsingStatement((BoundUsingStatement)node, arg), 
				BoundKind.SyncLockStatement => VisitSyncLockStatement((BoundSyncLockStatement)node, arg), 
				BoundKind.XmlName => VisitXmlName((BoundXmlName)node, arg), 
				BoundKind.XmlNamespace => VisitXmlNamespace((BoundXmlNamespace)node, arg), 
				BoundKind.XmlDocument => VisitXmlDocument((BoundXmlDocument)node, arg), 
				BoundKind.XmlDeclaration => VisitXmlDeclaration((BoundXmlDeclaration)node, arg), 
				BoundKind.XmlProcessingInstruction => VisitXmlProcessingInstruction((BoundXmlProcessingInstruction)node, arg), 
				BoundKind.XmlComment => VisitXmlComment((BoundXmlComment)node, arg), 
				BoundKind.XmlAttribute => VisitXmlAttribute((BoundXmlAttribute)node, arg), 
				BoundKind.XmlElement => VisitXmlElement((BoundXmlElement)node, arg), 
				BoundKind.XmlMemberAccess => VisitXmlMemberAccess((BoundXmlMemberAccess)node, arg), 
				BoundKind.XmlEmbeddedExpression => VisitXmlEmbeddedExpression((BoundXmlEmbeddedExpression)node, arg), 
				BoundKind.XmlCData => VisitXmlCData((BoundXmlCData)node, arg), 
				BoundKind.ResumeStatement => VisitResumeStatement((BoundResumeStatement)node, arg), 
				BoundKind.OnErrorStatement => VisitOnErrorStatement((BoundOnErrorStatement)node, arg), 
				BoundKind.UnstructuredExceptionHandlingStatement => VisitUnstructuredExceptionHandlingStatement((BoundUnstructuredExceptionHandlingStatement)node, arg), 
				BoundKind.UnstructuredExceptionHandlingCatchFilter => VisitUnstructuredExceptionHandlingCatchFilter((BoundUnstructuredExceptionHandlingCatchFilter)node, arg), 
				BoundKind.UnstructuredExceptionOnErrorSwitch => VisitUnstructuredExceptionOnErrorSwitch((BoundUnstructuredExceptionOnErrorSwitch)node, arg), 
				BoundKind.UnstructuredExceptionResumeSwitch => VisitUnstructuredExceptionResumeSwitch((BoundUnstructuredExceptionResumeSwitch)node, arg), 
				BoundKind.AwaitOperator => VisitAwaitOperator((BoundAwaitOperator)node, arg), 
				BoundKind.SpillSequence => VisitSpillSequence((BoundSpillSequence)node, arg), 
				BoundKind.StopStatement => VisitStopStatement((BoundStopStatement)node, arg), 
				BoundKind.EndStatement => VisitEndStatement((BoundEndStatement)node, arg), 
				BoundKind.MidResult => VisitMidResult((BoundMidResult)node, arg), 
				BoundKind.ConditionalAccess => VisitConditionalAccess((BoundConditionalAccess)node, arg), 
				BoundKind.ConditionalAccessReceiverPlaceholder => VisitConditionalAccessReceiverPlaceholder((BoundConditionalAccessReceiverPlaceholder)node, arg), 
				BoundKind.LoweredConditionalAccess => VisitLoweredConditionalAccess((BoundLoweredConditionalAccess)node, arg), 
				BoundKind.ComplexConditionalAccessReceiver => VisitComplexConditionalAccessReceiver((BoundComplexConditionalAccessReceiver)node, arg), 
				BoundKind.NameOfOperator => VisitNameOfOperator((BoundNameOfOperator)node, arg), 
				BoundKind.TypeAsValueExpression => VisitTypeAsValueExpression((BoundTypeAsValueExpression)node, arg), 
				BoundKind.InterpolatedStringExpression => VisitInterpolatedStringExpression((BoundInterpolatedStringExpression)node, arg), 
				BoundKind.Interpolation => VisitInterpolation((BoundInterpolation)node, arg), 
				_ => DefaultVisit(node, arg), 
			};
		}

		public virtual R VisitTypeArguments(BoundTypeArguments node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitOmittedArgument(BoundOmittedArgument node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRValuePlaceholder(BoundRValuePlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLValuePlaceholder(BoundLValuePlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitDup(BoundDup node, A arg)
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

		public virtual R VisitParenthesized(BoundParenthesized node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitBadVariable(BoundBadVariable node, A arg)
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

		public virtual R VisitGetType(BoundGetType node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitFieldInfo(BoundFieldInfo node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitMethodInfo(BoundMethodInfo node, A arg)
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

		public virtual R VisitUnaryOperator(BoundUnaryOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitBinaryOperator(BoundBinaryOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAssignmentOperator(BoundAssignmentOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitReferenceAssignment(BoundReferenceAssignment node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAddressOfOperator(BoundAddressOfOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitConversion(BoundConversion node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRelaxationLambda(BoundRelaxationLambda node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitConvertedTupleElements(BoundConvertedTupleElements node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUserDefinedConversion(BoundUserDefinedConversion node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitDirectCast(BoundDirectCast node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitTryCast(BoundTryCast node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitTypeOf(BoundTypeOf node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSequencePoint(BoundSequencePoint node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSequencePointExpression(BoundSequencePointExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSequencePointWithSpan(BoundSequencePointWithSpan node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitNoOpStatement(BoundNoOpStatement node, A arg)
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

		public virtual R VisitReturnStatement(BoundReturnStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitYieldStatement(BoundYieldStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitThrowStatement(BoundThrowStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRedimStatement(BoundRedimStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRedimClause(BoundRedimClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitEraseStatement(BoundEraseStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitCall(BoundCall node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAttribute(BoundAttribute node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLateMemberAccess(BoundLateMemberAccess node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLateInvocation(BoundLateInvocation node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLateAddressOfOperator(BoundLateAddressOfOperator node, A arg)
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

		public virtual R VisitObjectCreationExpression(BoundObjectCreationExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitObjectInitializerExpression(BoundObjectInitializerExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, A arg)
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

		public virtual R VisitArrayLiteral(BoundArrayLiteral node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitArrayInitialization(BoundArrayInitialization node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitFieldAccess(BoundFieldAccess node, A arg)
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

		public virtual R VisitBlock(BoundBlock node, A arg)
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

		public virtual R VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitDimStatement(BoundDimStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitInitializer(BoundInitializer node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitFieldInitializer(BoundFieldInitializer node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitPropertyInitializer(BoundPropertyInitializer node, A arg)
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

		public virtual R VisitSequence(BoundSequence node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitExpressionStatement(BoundExpressionStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitIfStatement(BoundIfStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSelectStatement(BoundSelectStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitCaseBlock(BoundCaseBlock node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitCaseStatement(BoundCaseStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSimpleCaseClause(BoundSimpleCaseClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRangeCaseClause(BoundRangeCaseClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRelationalCaseClause(BoundRelationalCaseClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitDoLoopStatement(BoundDoLoopStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitWhileStatement(BoundWhileStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitForToUserDefinedOperators(BoundForToUserDefinedOperators node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitForToStatement(BoundForToStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitForEachStatement(BoundForEachStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitExitStatement(BoundExitStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitContinueStatement(BoundContinueStatement node, A arg)
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

		public virtual R VisitMeReference(BoundMeReference node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitValueTypeMeReference(BoundValueTypeMeReference node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitMyBaseReference(BoundMyBaseReference node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitMyClassReference(BoundMyClassReference node, A arg)
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

		public virtual R VisitLocal(BoundLocal node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitPseudoVariable(BoundPseudoVariable node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitParameter(BoundParameter node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLabelStatement(BoundLabelStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLabel(BoundLabel node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitGotoStatement(BoundGotoStatement node, A arg)
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

		public virtual R VisitWithStatement(BoundWithStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUnboundLambda(UnboundLambda node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLambda(BoundLambda node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitQueryExpression(BoundQueryExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitQuerySource(BoundQuerySource node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitQueryableSource(BoundQueryableSource node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitQueryClause(BoundQueryClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitOrdering(BoundOrdering node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitQueryLambda(BoundQueryLambda node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRangeVariableAssignment(BoundRangeVariableAssignment node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAggregateClause(BoundAggregateClause node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitGroupAggregation(BoundGroupAggregation node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRangeVariable(BoundRangeVariable node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAddHandlerStatement(BoundAddHandlerStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitRaiseEventStatement(BoundRaiseEventStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUsingStatement(BoundUsingStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSyncLockStatement(BoundSyncLockStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlName(BoundXmlName node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlNamespace(BoundXmlNamespace node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlDocument(BoundXmlDocument node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlDeclaration(BoundXmlDeclaration node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlComment(BoundXmlComment node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlAttribute(BoundXmlAttribute node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlElement(BoundXmlElement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlMemberAccess(BoundXmlMemberAccess node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitXmlCData(BoundXmlCData node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitResumeStatement(BoundResumeStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitOnErrorStatement(BoundOnErrorStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUnstructuredExceptionHandlingCatchFilter(BoundUnstructuredExceptionHandlingCatchFilter node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitAwaitOperator(BoundAwaitOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitSpillSequence(BoundSpillSequence node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitStopStatement(BoundStopStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitEndStatement(BoundEndStatement node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitMidResult(BoundMidResult node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitConditionalAccess(BoundConditionalAccess node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitNameOfOperator(BoundNameOfOperator node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitTypeAsValueExpression(BoundTypeAsValueExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node, A arg)
		{
			return DefaultVisit(node, arg);
		}

		public virtual R VisitInterpolation(BoundInterpolation node, A arg)
		{
			return DefaultVisit(node, arg);
		}
	}
	internal abstract class BoundTreeVisitor
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
				DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_TooLongOrComplexExpression, GetTooLongOrComplexExpressionErrorLocation(Node));
			}

			public void AddAnError(BindingDiagnosticBag diagnostics)
			{
				diagnostics.Add(ERRID.ERR_TooLongOrComplexExpression, GetTooLongOrComplexExpressionErrorLocation(Node));
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
				StackGuard.EnsureSufficientExecutionStack(recursionDepth);
				result = VisitExpressionWithoutStackGuard(node);
			}
			else
			{
				result = VisitExpressionWithStackGuard(node);
			}
			recursionDepth--;
			return result;
		}

		protected virtual bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
		{
			return true;
		}

		[DebuggerStepThrough]
		private BoundExpression VisitExpressionWithStackGuard(BoundExpression node)
		{
			try
			{
				return VisitExpressionWithoutStackGuard(node);
			}
			catch (InsufficientExecutionStackException ex)
			{
				ProjectData.SetProjectError(ex);
				InsufficientExecutionStackException inner = ex;
				throw new CancelledByStackGuardException(inner, node);
			}
		}

		protected abstract BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node);

		public virtual BoundNode VisitTypeArguments(BoundTypeArguments node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitOmittedArgument(BoundOmittedArgument node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRValuePlaceholder(BoundRValuePlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLValuePlaceholder(BoundLValuePlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitDup(BoundDup node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBadExpression(BoundBadExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBadStatement(BoundBadStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitParenthesized(BoundParenthesized node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBadVariable(BoundBadVariable node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitArrayAccess(BoundArrayAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitArrayLength(BoundArrayLength node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitGetType(BoundGetType node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitFieldInfo(BoundFieldInfo node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMethodInfo(BoundMethodInfo node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTypeExpression(BoundTypeExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNamespaceExpression(BoundNamespaceExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMethodDefIndex(BoundMethodDefIndex node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitModuleVersionId(BoundModuleVersionId node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitModuleVersionIdString(BoundModuleVersionIdString node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnaryOperator(BoundUnaryOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConversion(BoundConversion node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRelaxationLambda(BoundRelaxationLambda node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConvertedTupleElements(BoundConvertedTupleElements node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUserDefinedConversion(BoundUserDefinedConversion node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitDirectCast(BoundDirectCast node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTryCast(BoundTryCast node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTypeOf(BoundTypeOf node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSequencePoint(BoundSequencePoint node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSequencePointExpression(BoundSequencePointExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNoOpStatement(BoundNoOpStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMethodGroup(BoundMethodGroup node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitPropertyGroup(BoundPropertyGroup node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitYieldStatement(BoundYieldStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitThrowStatement(BoundThrowStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRedimStatement(BoundRedimStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRedimClause(BoundRedimClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitEraseStatement(BoundEraseStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCall(BoundCall node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAttribute(BoundAttribute node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTupleLiteral(BoundTupleLiteral node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNewT(BoundNewT node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitArrayLiteral(BoundArrayLiteral node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitArrayInitialization(BoundArrayInitialization node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitEventAccess(BoundEventAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitBlock(BoundBlock node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitStateMachineScope(BoundStateMachineScope node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitDimStatement(BoundDimStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitInitializer(BoundInitializer node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitFieldInitializer(BoundFieldInitializer node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitPropertyInitializer(BoundPropertyInitializer node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitParameterEqualsValue(BoundParameterEqualsValue node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSequence(BoundSequence node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitExpressionStatement(BoundExpressionStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitIfStatement(BoundIfStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCaseBlock(BoundCaseBlock node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCaseStatement(BoundCaseStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSimpleCaseClause(BoundSimpleCaseClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRangeCaseClause(BoundRangeCaseClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRelationalCaseClause(BoundRelationalCaseClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitForToUserDefinedOperators(BoundForToUserDefinedOperators node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitForToStatement(BoundForToStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitExitStatement(BoundExitStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitContinueStatement(BoundContinueStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTryStatement(BoundTryStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitCatchBlock(BoundCatchBlock node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLiteral(BoundLiteral node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMeReference(BoundMeReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitValueTypeMeReference(BoundValueTypeMeReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLocal(BoundLocal node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitPseudoVariable(BoundPseudoVariable node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitParameter(BoundParameter node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLabel(BoundLabel node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitStatementList(BoundStatementList node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConditionalGoto(BoundConditionalGoto node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitWithStatement(BoundWithStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnboundLambda(UnboundLambda node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLambda(BoundLambda node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitQuerySource(BoundQuerySource node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitQueryClause(BoundQueryClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitOrdering(BoundOrdering node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitGroupAggregation(BoundGroupAggregation node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAddHandlerStatement(BoundAddHandlerStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitRaiseEventStatement(BoundRaiseEventStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSyncLockStatement(BoundSyncLockStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlName(BoundXmlName node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlNamespace(BoundXmlNamespace node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlDocument(BoundXmlDocument node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlDeclaration(BoundXmlDeclaration node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlComment(BoundXmlComment node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlAttribute(BoundXmlAttribute node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlElement(BoundXmlElement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitXmlCData(BoundXmlCData node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitResumeStatement(BoundResumeStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitOnErrorStatement(BoundOnErrorStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnstructuredExceptionHandlingCatchFilter(BoundUnstructuredExceptionHandlingCatchFilter node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitSpillSequence(BoundSpillSequence node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitStopStatement(BoundStopStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitEndStatement(BoundEndStatement node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitMidResult(BoundMidResult node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitNameOfOperator(BoundNameOfOperator node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitTypeAsValueExpression(BoundTypeAsValueExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node)
		{
			return DefaultVisit(node);
		}

		public virtual BoundNode VisitInterpolation(BoundInterpolation node)
		{
			return DefaultVisit(node);
		}
	}
}
