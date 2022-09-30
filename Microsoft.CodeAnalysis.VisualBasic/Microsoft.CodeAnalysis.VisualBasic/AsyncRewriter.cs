using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AsyncRewriter : StateMachineRewriter<AsyncRewriter.CapturedSymbolOrExpression>
	{
		internal class AsyncMethodToClassRewriter : StateMachineMethodToClassRewriter
		{
			private class ConditionalAccessReceiverPlaceholderReplacementInfo
			{
				public readonly int PlaceholderId;

				public bool IsSpilled;

				public ConditionalAccessReceiverPlaceholderReplacementInfo(int placeholderId)
				{
					PlaceholderId = placeholderId;
					IsSpilled = false;
				}
			}

			private sealed class ConditionalAccessReceiverPlaceholderReplacement : BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			{
				private readonly int _placeholderId;

				private readonly BoundExpression _replaceWith;

				private bool _replaced;

				public bool Replaced => _replaced;

				public ConditionalAccessReceiverPlaceholderReplacement(int placeholderId, BoundExpression replaceWith, int recursionDepth)
					: base(recursionDepth)
				{
					_placeholderId = placeholderId;
					_replaceWith = replaceWith;
				}

				public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
				{
					if (_placeholderId == node.PlaceholderId)
					{
						_replaced = true;
						BoundExpression replaceWith = _replaceWith;
						if (node.IsLValue)
						{
							return replaceWith;
						}
						return replaceWith.MakeRValue();
					}
					return node;
				}
			}

			private struct ExpressionsWithReceiver
			{
				public readonly BoundExpression ReceiverOpt;

				public readonly ImmutableArray<BoundExpression> Arguments;

				public ExpressionsWithReceiver(BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments)
				{
					this = default(ExpressionsWithReceiver);
					ReceiverOpt = receiverOpt;
					Arguments = arguments;
				}
			}

			private ConditionalAccessReceiverPlaceholderReplacementInfo _conditionalAccessReceiverPlaceholderReplacementInfo;

			private readonly MethodSymbol _method;

			private readonly FieldSymbol _builder;

			private readonly LabelSymbol _exprReturnLabel;

			private readonly LabelSymbol _exitLabel;

			private readonly LocalSymbol _exprRetValue;

			private readonly AsyncMethodKind _asyncMethodKind;

			private readonly Dictionary<TypeSymbol, FieldSymbol> _awaiterFields;

			private int _nextAwaiterId;

			private readonly AsyncRewriter _owner;

			private readonly SpillFieldAllocator _spillFieldAllocator;

			private readonly Dictionary<TypeSymbol, bool> _typesNeedingClearingCache;

			protected override string ResumeLabelName => "asyncLabel";

			protected override bool IsInExpressionLambda => false;

			public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
			{
				SpillBuilder builder = default(SpillBuilder);
				TypeSymbol type = node.GetAwaiter.Type.InternalSubstituteTypeParameters(TypeMap).Type;
				LocalSymbol localSymbol = F.SynthesizedLocal(type, SynthesizedLocalKind.Awaiter, node.Syntax);
				builder.AddLocal(localSymbol);
				BoundLValuePlaceholder awaiterInstancePlaceholder = node.AwaiterInstancePlaceholder;
				PlaceholderReplacementMap.Add(awaiterInstancePlaceholder, F.Local(localSymbol, isLValue: true));
				BoundRValuePlaceholder awaitableInstancePlaceholder = node.AwaitableInstancePlaceholder;
				PlaceholderReplacementMap.Add(awaitableInstancePlaceholder, VisitExpression(node.Operand));
				BoundExpression expression = VisitExpression(node.GetAwaiter);
				BoundExpression boundExpression = VisitExpression(node.IsCompleted);
				BoundExpression boundExpression2 = VisitExpression(node.GetResult);
				TypeSymbol typeSymbol = VisitType(node.Type);
				PlaceholderReplacementMap.Remove(awaiterInstancePlaceholder);
				PlaceholderReplacementMap.Remove(awaitableInstancePlaceholder);
				builder.AddStatement(MakeAssignmentStatement(expression, localSymbol, ref builder));
				builder.AddStatement(SyntheticBoundNodeFactory.HiddenSequencePoint());
				BoundStatement boundStatement = GenerateAwaitForIncompleteTask(localSymbol);
				if (TypeSymbolExtensions.IsObjectType(type))
				{
					builder.AddStatement(F.If(F.Convert(F.SpecialType(SpecialType.System_Boolean), boundExpression), F.StatementList(), boundStatement));
				}
				else
				{
					builder.AddStatement(F.If(F.Not(boundExpression), boundStatement));
				}
				BoundExpression boundExpression3 = null;
				BoundExpression boundExpression4 = F.AssignmentExpression(F.Local(localSymbol, isLValue: true), F.Null(localSymbol.Type));
				if (typeSymbol.SpecialType != SpecialType.System_Void)
				{
					LocalSymbol localSymbol2 = F.SynthesizedLocal(typeSymbol);
					boundExpression3 = F.Sequence(localSymbol2, F.AssignmentExpression(F.Local(localSymbol2, isLValue: true), boundExpression2), boundExpression4, F.Local(localSymbol2, isLValue: false));
				}
				else
				{
					boundExpression3 = F.Sequence(boundExpression2, boundExpression4);
				}
				return builder.BuildSequenceAndFree(F, boundExpression3);
			}

			private BoundBlock GenerateAwaitForIncompleteTask(LocalSymbol awaiterTemp)
			{
				StateInfo stateInfo = AddState();
				TypeSymbol type = awaiterTemp.Type;
				TypeSymbol typeSymbol = type;
				if (TypeSymbolExtensions.IsVerifierReference(typeSymbol))
				{
					typeSymbol = F.SpecialType(SpecialType.System_Object);
				}
				FieldSymbol awaiterField = GetAwaiterField(typeSymbol);
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				instance.Add(F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(stateInfo.Number))));
				instance.Add(F.NoOp(NoOpStatementFlavor.AwaitYieldPoint));
				instance.Add(F.Assignment(F.Field(F.Me(), awaiterField, isLValue: true), TypeSymbol.Equals(awaiterField.Type, awaiterTemp.Type, TypeCompareKind.ConsiderEverything) ? ((BoundExpression)F.Local(awaiterTemp, isLValue: false)) : ((BoundExpression)F.Convert(typeSymbol, F.Local(awaiterTemp, isLValue: false)))));
				bool flag = TypeSymbolExtensions.IsObjectType(type);
				BoundExpression receiver = F.Field(F.Me(), _builder, isLValue: false);
				NamedTypeSymbol namedTypeSymbol = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_ICriticalNotifyCompletion);
				if (!TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
				{
					if (flag)
					{
						LocalSymbol localSymbol = F.SynthesizedLocal(namedTypeSymbol);
						LocalSymbol localSymbol2 = F.SynthesizedLocal(F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_INotifyCompletion));
						BoundLocal expression = F.Local(awaiterTemp, isLValue: false);
						BoundLocal boundLocal = F.Local(localSymbol, isLValue: true);
						BoundLocal boundLocal2 = F.Local(localSymbol2, isLValue: true);
						BoundStatement boundStatement = MakeAssignmentStatement(F.TryCast(expression, localSymbol.Type), localSymbol);
						BoundStatement thenClause = F.ExpressionStatement(_owner.GenerateMethodCall(receiver, _owner._builderType, "AwaitUnsafeOnCompleted", ImmutableArray.Create(localSymbol.Type, F.Me().Type), boundLocal, F.ReferenceOrByrefMe()));
						BoundStatement boundStatement2 = MakeAssignmentStatement(F.DirectCast(expression, localSymbol2.Type), localSymbol2);
						BoundStatement boundStatement3 = F.ExpressionStatement(_owner.GenerateMethodCall(receiver, _owner._builderType, "AwaitOnCompleted", ImmutableArray.Create(localSymbol2.Type, F.Me().Type), boundLocal2, F.ReferenceOrByrefMe()));
						instance.Add(F.Block(ImmutableArray.Create(localSymbol, localSymbol2), boundStatement, F.If(F.Not(F.ReferenceIsNothing(F.Local(localSymbol, isLValue: false))), thenClause, F.Block(boundStatement2, boundStatement3))));
					}
					else
					{
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(F.Diagnostics, CompilationState.Compilation.Assembly);
						bool flag2 = Conversions.IsWideningConversion(Conversions.ClassifyDirectCastConversion(type, namedTypeSymbol, ref useSiteInfo));
						((BindingDiagnosticBag<AssemblySymbol>)F.Diagnostics).Add(F.Syntax, useSiteInfo);
						instance.Add(F.ExpressionStatement(_owner.GenerateMethodCall(receiver, _owner._builderType, flag2 ? "AwaitUnsafeOnCompleted" : "AwaitOnCompleted", ImmutableArray.Create(type, F.Me().Type), F.Local(awaiterTemp, isLValue: true), F.ReferenceOrByrefMe())));
					}
				}
				instance.Add(F.Goto(_exitLabel));
				instance.Add(F.Label(stateInfo.ResumeLabel));
				instance.Add(F.NoOp(NoOpStatementFlavor.AwaitResumePoint));
				instance.Add(F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(StateMachineStates.NotStartedStateMachine))));
				instance.Add(F.Assignment(F.Local(awaiterTemp, isLValue: true), TypeSymbol.Equals(awaiterTemp.Type, awaiterField.Type, TypeCompareKind.ConsiderEverything) ? ((BoundExpression)F.Field(F.Me(), awaiterField, isLValue: false)) : ((BoundExpression)F.Convert(awaiterTemp.Type, F.Field(F.Me(), awaiterField, isLValue: false)))));
				instance.Add(F.Assignment(F.Field(F.Me(), awaiterField, isLValue: true), F.Null(awaiterField.Type)));
				return F.Block(instance.ToImmutableAndFree());
			}

			protected override BoundNode MaterializeProxy(BoundExpression origExpression, CapturedSymbolOrExpression proxy)
			{
				return proxy.Materialize(this, origExpression.IsLValue);
			}

			internal override void AddProxyFieldsForStateMachineScope(CapturedSymbolOrExpression proxy, ArrayBuilder<FieldSymbol> proxyFields)
			{
				proxy.AddProxyFieldsForStateMachineScope(proxyFields);
			}

			public BoundExpression VisitExpression(BoundExpression expression)
			{
				return (BoundExpression)Visit(expression);
			}

			public override BoundNode VisitSpillSequence(BoundSpillSequence node)
			{
				ImmutableArray<BoundStatement> immutableArray = VisitList(node.Statements);
				BoundExpression boundExpression = VisitExpression(node.ValueOpt);
				TypeSymbol type = VisitType(node.Type);
				if (boundExpression == null || boundExpression.Kind != BoundKind.SpillSequence)
				{
					return node.Update(node.Locals, node.SpillFields, immutableArray, boundExpression, type);
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)boundExpression;
				return node.Update(node.Locals.Concat(boundSpillSequence.Locals), node.SpillFields.Concat(boundSpillSequence.SpillFields), immutableArray.Concat(boundSpillSequence.Statements), boundSpillSequence.ValueOpt, type);
			}

			public override BoundNode VisitSequence(BoundSequence node)
			{
				BoundSequence boundSequence = (BoundSequence)base.VisitSequence(node);
				ImmutableArray<LocalSymbol> locals = boundSequence.Locals;
				ImmutableArray<BoundExpression> sideEffects = boundSequence.SideEffects;
				BoundExpression valueOpt = boundSequence.ValueOpt;
				bool flag = NeedsSpill(sideEffects);
				bool flag2 = NeedsSpill(valueOpt);
				if (!flag && !flag2)
				{
					return boundSequence;
				}
				SpillBuilder builder = default(SpillBuilder);
				builder.AddLocals(locals);
				if (flag)
				{
					ImmutableArray<BoundExpression>.Enumerator enumerator = sideEffects.GetEnumerator();
					while (enumerator.MoveNext())
					{
						BoundExpression current = enumerator.Current;
						builder.AddStatement(MakeExpressionStatement(current, ref builder));
					}
				}
				if (flag2)
				{
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)valueOpt;
					builder.AddSpill(boundSpillSequence);
					valueOpt = boundSpillSequence.ValueOpt;
				}
				return builder.BuildSequenceAndFree(F, valueOpt);
			}

			private BoundStatement MakeExpressionStatement(BoundExpression expression, ref SpillBuilder builder)
			{
				if (NeedsSpill(expression))
				{
					BoundSpillSequence expression2 = (BoundSpillSequence)expression;
					builder.AssumeFieldsIfNeeded(ref expression2);
					return RewriteSpillSequenceIntoBlock(expression2, addValueAsExpression: true);
				}
				return F.ExpressionStatement(expression);
			}

			public override BoundNode VisitCall(BoundCall node)
			{
				BoundCall boundCall = (BoundCall)base.VisitCall(node);
				BoundExpression receiverOpt = boundCall.ReceiverOpt;
				ImmutableArray<BoundExpression> arguments = boundCall.Arguments;
				if (!NeedsSpill(arguments) && !NeedsSpill(receiverOpt))
				{
					return boundCall;
				}
				SpillBuilder spillBuilder = default(SpillBuilder);
				ExpressionsWithReceiver expressionsWithReceiver = SpillExpressionsWithReceiver(receiverOpt, isReceiverOfAMethodCall: true, arguments, ref spillBuilder);
				return spillBuilder.BuildSequenceAndFree(F, boundCall.Update(boundCall.Method, boundCall.MethodGroupOpt, expressionsWithReceiver.ReceiverOpt, expressionsWithReceiver.Arguments, boundCall.DefaultArguments, boundCall.ConstantValueOpt, boundCall.IsLValue, boundCall.SuppressObjectClone, boundCall.Type));
			}

			public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)base.VisitObjectCreationExpression(node);
				ImmutableArray<BoundExpression> arguments = boundObjectCreationExpression.Arguments;
				if (!NeedsSpill(arguments))
				{
					return boundObjectCreationExpression;
				}
				SpillBuilder builder = default(SpillBuilder);
				arguments = SpillExpressionList(ref builder, arguments, firstArgumentIsAReceiverOfAMethodCall: false);
				return builder.BuildSequenceAndFree(F, boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, boundObjectCreationExpression.Type));
			}

			public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
			{
				BoundDelegateCreationExpression boundDelegateCreationExpression = (BoundDelegateCreationExpression)base.VisitDelegateCreationExpression(node);
				BoundExpression receiverOpt = boundDelegateCreationExpression.ReceiverOpt;
				if (!NeedsSpill(receiverOpt))
				{
					return boundDelegateCreationExpression;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)receiverOpt;
				return SpillSequenceWithNewValue(boundSpillSequence, boundDelegateCreationExpression.Update(boundSpillSequence.ValueOpt, boundDelegateCreationExpression.Method, boundDelegateCreationExpression.RelaxationLambdaOpt, boundDelegateCreationExpression.RelaxationReceiverPlaceholderOpt, boundDelegateCreationExpression.MethodGroupOpt, boundDelegateCreationExpression.Type));
			}

			public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
			{
				BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)base.VisitBinaryOperator(node);
				BoundExpression left = boundBinaryOperator.Left;
				BoundExpression right = boundBinaryOperator.Right;
				if (!NeedsSpill(left) && !NeedsSpill(right))
				{
					return boundBinaryOperator;
				}
				SpillBuilder builder = default(SpillBuilder);
				BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
				if (binaryOperatorKind == BinaryOperatorKind.AndAlso || binaryOperatorKind == BinaryOperatorKind.OrElse)
				{
					BoundExpression condition = SpillValue(left, ref builder);
					LocalSymbol localSymbol = F.SynthesizedLocal(boundBinaryOperator.Type);
					builder.AddLocal(localSymbol);
					builder.AddStatement((binaryOperatorKind == BinaryOperatorKind.AndAlso) ? F.If(condition, MakeAssignmentStatement(right, localSymbol, ref builder), MakeAssignmentStatement(F.Literal(value: false), localSymbol)) : F.If(condition, MakeAssignmentStatement(F.Literal(value: true), localSymbol), MakeAssignmentStatement(right, localSymbol, ref builder)));
					return builder.BuildSequenceAndFree(F, F.Local(localSymbol, isLValue: false));
				}
				ImmutableArray<BoundExpression> immutableArray = SpillExpressionList(ref builder, left, right);
				return builder.BuildSequenceAndFree(F, boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, immutableArray[0], immutableArray[1], boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, boundBinaryOperator.Type));
			}

			public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
			{
				return ProcessRewrittenAssignmentOperator((BoundAssignmentOperator)base.VisitAssignmentOperator(node));
			}

			internal BoundExpression ProcessRewrittenAssignmentOperator(BoundAssignmentOperator rewritten)
			{
				BoundExpression left = rewritten.Left;
				BoundExpression right = rewritten.Right;
				bool num = NeedsSpill(left);
				bool flag = NeedsSpill(right);
				if (!num && !flag)
				{
					return rewritten;
				}
				if (!flag)
				{
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)left;
					return SpillSequenceWithNewValue(boundSpillSequence, rewritten.Update(boundSpillSequence.ValueOpt, rewritten.LeftOnTheRightOpt, right, rewritten.SuppressObjectClone, rewritten.Type));
				}
				SpillBuilder builder = default(SpillBuilder);
				BoundExpression left2 = SpillLValue(left, isReceiver: false, evaluateSideEffects: true, ref builder, isAssignmentTarget: true);
				BoundSpillSequence boundSpillSequence2 = (BoundSpillSequence)right;
				builder.AddSpill(boundSpillSequence2);
				return builder.BuildSequenceAndFree(F, rewritten.Update(left2, rewritten.LeftOnTheRightOpt, boundSpillSequence2.ValueOpt, rewritten.SuppressObjectClone, rewritten.Type));
			}

			public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
			{
				BoundLocal byRefLocal = node.ByRefLocal;
				LocalSymbol localSymbol = byRefLocal.LocalSymbol;
				TypeSymbol type = VisitType(node.Type);
				if (!Proxies.ContainsKey(localSymbol))
				{
					BoundLocal byRefLocal2 = (BoundLocal)VisitExpression(byRefLocal);
					BoundExpression boundExpression = VisitExpression(node.LValue);
					if (!NeedsSpill(boundExpression))
					{
						return node.Update(byRefLocal2, boundExpression, node.IsLValue, type);
					}
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)boundExpression;
					return SpillSequenceWithNewValue(boundSpillSequence, node.Update(byRefLocal2, boundSpillSequence.ValueOpt, node.IsLValue, type));
				}
				CapturedSymbolOrExpression capturedSymbolOrExpression = Proxies[localSymbol];
				ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
				capturedSymbolOrExpression.CreateCaptureInitializationCode(this, instance);
				BoundExpression boundExpression2 = capturedSymbolOrExpression.Materialize(this, node.IsLValue);
				if (instance.Count == 0)
				{
					instance.Free();
					return boundExpression2;
				}
				instance.Add(boundExpression2);
				return F.Sequence(instance.ToArrayAndFree());
			}

			public override BoundNode VisitFieldAccess(BoundFieldAccess node)
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)base.VisitFieldAccess(node);
				BoundExpression receiverOpt = boundFieldAccess.ReceiverOpt;
				if (!NeedsSpill(receiverOpt))
				{
					return boundFieldAccess;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)receiverOpt;
				return SpillSequenceWithNewValue(boundSpillSequence, boundFieldAccess.Update(boundSpillSequence.ValueOpt, boundFieldAccess.FieldSymbol, boundFieldAccess.IsLValue, boundFieldAccess.SuppressVirtualCalls, null, boundFieldAccess.Type));
			}

			public override BoundNode VisitDirectCast(BoundDirectCast node)
			{
				BoundDirectCast boundDirectCast = (BoundDirectCast)base.VisitDirectCast(node);
				BoundExpression operand = boundDirectCast.Operand;
				if (!NeedsSpill(operand))
				{
					return boundDirectCast;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)operand;
				return SpillSequenceWithNewValue(boundSpillSequence, boundDirectCast.Update(boundSpillSequence.ValueOpt, boundDirectCast.ConversionKind, boundDirectCast.SuppressVirtualCalls, boundDirectCast.ConstantValueOpt, boundDirectCast.RelaxationLambdaOpt, boundDirectCast.Type));
			}

			public override BoundNode VisitTryCast(BoundTryCast node)
			{
				BoundTryCast boundTryCast = (BoundTryCast)base.VisitTryCast(node);
				BoundExpression operand = boundTryCast.Operand;
				if (!NeedsSpill(operand))
				{
					return boundTryCast;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)operand;
				return SpillSequenceWithNewValue(boundSpillSequence, boundTryCast.Update(boundSpillSequence.ValueOpt, boundTryCast.ConversionKind, boundTryCast.ConstantValueOpt, boundTryCast.RelaxationLambdaOpt, boundTryCast.Type));
			}

			public override BoundNode VisitConversion(BoundConversion node)
			{
				BoundConversion boundConversion = (BoundConversion)base.VisitConversion(node);
				BoundExpression operand = boundConversion.Operand;
				if (!NeedsSpill(operand))
				{
					return boundConversion;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)operand;
				return SpillSequenceWithNewValue(boundSpillSequence, boundConversion.Update(boundSpillSequence.ValueOpt, boundConversion.ConversionKind, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValueOpt, boundConversion.ExtendedInfoOpt, boundConversion.Type));
			}

			public override BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
			{
				BoundLValueToRValueWrapper boundLValueToRValueWrapper = (BoundLValueToRValueWrapper)base.VisitLValueToRValueWrapper(node);
				BoundExpression underlyingLValue = boundLValueToRValueWrapper.UnderlyingLValue;
				if (!NeedsSpill(underlyingLValue))
				{
					return boundLValueToRValueWrapper;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)underlyingLValue;
				return SpillSequenceWithNewValue(boundSpillSequence, boundLValueToRValueWrapper.Update(boundSpillSequence.ValueOpt, boundLValueToRValueWrapper.Type));
			}

			public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
			{
				BoundTernaryConditionalExpression boundTernaryConditionalExpression = (BoundTernaryConditionalExpression)base.VisitTernaryConditionalExpression(node);
				BoundExpression boundExpression = boundTernaryConditionalExpression.Condition;
				BoundExpression whenTrue = boundTernaryConditionalExpression.WhenTrue;
				BoundExpression whenFalse = boundTernaryConditionalExpression.WhenFalse;
				bool flag = NeedsSpill(boundExpression);
				if (!flag && !NeedsSpill(whenTrue) && !NeedsSpill(whenFalse))
				{
					return boundTernaryConditionalExpression;
				}
				SpillBuilder builder = default(SpillBuilder);
				if (flag)
				{
					boundExpression = SpillRValue(boundExpression, ref builder);
				}
				BoundExpression expression;
				if (!TypeSymbolExtensions.IsVoidType(boundTernaryConditionalExpression.Type))
				{
					LocalSymbol localSymbol = F.SynthesizedLocal(boundTernaryConditionalExpression.Type);
					builder.AddLocal(localSymbol);
					builder.AddStatement(F.If(boundExpression, MakeAssignmentStatement(whenTrue, localSymbol, ref builder), MakeAssignmentStatement(whenFalse, localSymbol, ref builder)));
					expression = F.Local(localSymbol, isLValue: false);
				}
				else
				{
					builder.AddStatement(F.If(boundExpression, MakeExpressionStatement(whenTrue, ref builder), MakeExpressionStatement(whenFalse, ref builder)));
					expression = null;
				}
				return builder.BuildSequenceAndFree(F, expression);
			}

			private BoundStatement MakeAssignmentStatement(BoundExpression expression, LocalSymbol temp, [In][Out] ref SpillBuilder builder)
			{
				if (NeedsSpill(expression))
				{
					BoundSpillSequence expression2 = (BoundSpillSequence)expression;
					builder.AssumeFieldsIfNeeded(ref expression2);
					return RewriteSpillSequenceIntoBlock(expression2, false, F.Assignment(F.Local(temp, isLValue: true), expression2.ValueOpt));
				}
				return F.Assignment(F.Local(temp, isLValue: true), expression);
			}

			private BoundStatement MakeAssignmentStatement(BoundExpression expression, LocalSymbol temp)
			{
				return F.Assignment(F.Local(temp, isLValue: true), expression);
			}

			public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
			{
				TypeSymbol type = VisitType(node.Type);
				BoundExpression boundExpression = (BoundExpression)Visit(node.ReceiverOrCondition);
				bool flag = NeedsSpill(boundExpression);
				ConditionalAccessReceiverPlaceholderReplacementInfo conditionalAccessReceiverPlaceholderReplacementInfo = _conditionalAccessReceiverPlaceholderReplacementInfo;
				ConditionalAccessReceiverPlaceholderReplacementInfo conditionalAccessReceiverPlaceholderReplacementInfo2 = (_conditionalAccessReceiverPlaceholderReplacementInfo = ((node.PlaceholderId == 0) ? null : new ConditionalAccessReceiverPlaceholderReplacementInfo(node.PlaceholderId)));
				BoundExpression boundExpression2 = (BoundExpression)Visit(node.WhenNotNull);
				bool flag2 = NeedsSpill(boundExpression2);
				_conditionalAccessReceiverPlaceholderReplacementInfo = null;
				BoundExpression boundExpression3 = (BoundExpression)Visit(node.WhenNullOpt);
				bool flag3 = boundExpression3 != null && NeedsSpill(boundExpression3);
				_conditionalAccessReceiverPlaceholderReplacementInfo = conditionalAccessReceiverPlaceholderReplacementInfo;
				if (!flag && !flag2 && !flag3)
				{
					return node.Update(boundExpression, node.CaptureReceiver, node.PlaceholderId, boundExpression2, boundExpression3, type);
				}
				if (!flag2 && !flag3)
				{
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)boundExpression;
					return SpillSequenceWithNewValue(boundSpillSequence, node.Update(boundSpillSequence.ValueOpt, node.CaptureReceiver, node.PlaceholderId, boundExpression2, boundExpression3, type));
				}
				SpillBuilder builder = default(SpillBuilder);
				if (flag)
				{
					BoundSpillSequence boundSpillSequence2 = (BoundSpillSequence)boundExpression;
					builder.AddSpill(boundSpillSequence2);
					boundExpression = boundSpillSequence2.ValueOpt;
				}
				BoundExpression condition;
				if (node.PlaceholderId == 0)
				{
					condition = boundExpression;
				}
				else
				{
					BoundExpression boundExpression4 = boundExpression;
					BoundExpression boundExpression5;
					BoundExpression boundExpression7;
					if (node.CaptureReceiver || conditionalAccessReceiverPlaceholderReplacementInfo2.IsSpilled)
					{
						if (node.CaptureReceiver)
						{
							if (!boundExpression4.Type.IsReferenceType)
							{
								boundExpression4 = SpillValue(boundExpression4, isReceiver: true, evaluateSideEffects: true, ref builder);
							}
							BoundExpression boundExpression6;
							if (conditionalAccessReceiverPlaceholderReplacementInfo2.IsSpilled)
							{
								boundExpression5 = SpillRValue(boundExpression4.MakeRValue(), ref builder);
								boundExpression6 = boundExpression5;
							}
							else
							{
								LocalSymbol localSymbol = null;
								localSymbol = F.SynthesizedLocal(boundExpression4.Type);
								builder.AddLocal(localSymbol);
								boundExpression5 = F.AssignmentExpression(F.Local(localSymbol, isLValue: true), boundExpression4.MakeRValue());
								boundExpression6 = F.Local(localSymbol, isLValue: true);
							}
							boundExpression7 = ((!boundExpression5.Type.IsReferenceType) ? new BoundComplexConditionalAccessReceiver(F.Syntax, boundExpression4, boundExpression6, boundExpression4.Type) : boundExpression6);
						}
						else
						{
							boundExpression7 = SpillValue(boundExpression4, isReceiver: true, evaluateSideEffects: true, ref builder);
							boundExpression5 = boundExpression7.MakeRValue();
						}
					}
					else
					{
						boundExpression7 = boundExpression4;
						boundExpression5 = boundExpression7.MakeRValue();
					}
					boundExpression2 = (BoundExpression)new ConditionalAccessReceiverPlaceholderReplacement(node.PlaceholderId, boundExpression7, base.RecursionDepth).Visit(boundExpression2);
					if (boundExpression5.Type.IsReferenceType)
					{
						condition = F.ReferenceIsNotNothing(boundExpression5);
					}
					else
					{
						BoundBinaryOperator left = F.ReferenceIsNotNothing(F.DirectCast(F.DirectCast(F.Null(), boundExpression5.Type), F.SpecialType(SpecialType.System_Object)));
						condition = F.LogicalOrElse(left, F.ReferenceIsNotNothing(F.DirectCast(boundExpression5, F.SpecialType(SpecialType.System_Object))));
					}
				}
				if (boundExpression3 == null)
				{
					builder.AddStatement(F.If(condition, MakeExpressionStatement(boundExpression2, ref builder)));
					return builder.BuildSequenceAndFree(F, null);
				}
				LocalSymbol localSymbol2 = F.SynthesizedLocal(type);
				builder.AddLocal(localSymbol2);
				builder.AddStatement(F.If(condition, MakeAssignmentStatement(boundExpression2, localSymbol2, ref builder), MakeAssignmentStatement(boundExpression3, localSymbol2, ref builder)));
				return builder.BuildSequenceAndFree(F, F.Local(localSymbol2, isLValue: false));
			}

			public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
			{
				if (_conditionalAccessReceiverPlaceholderReplacementInfo == null || _conditionalAccessReceiverPlaceholderReplacementInfo.PlaceholderId != node.PlaceholderId)
				{
					throw ExceptionUtilities.Unreachable;
				}
				return base.VisitConditionalAccessReceiverPlaceholder(node);
			}

			public override BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override BoundNode VisitArrayCreation(BoundArrayCreation node)
			{
				ImmutableArray<BoundExpression> immutableArray = VisitList(node.Bounds);
				BoundArrayInitialization boundArrayInitialization = (BoundArrayInitialization)Visit(node.InitializerOpt);
				TypeSymbol type = VisitType(node.Type);
				bool num = NeedsSpill(immutableArray);
				bool flag = ArrayInitializerNeedsSpill(boundArrayInitialization);
				if (!num && !flag)
				{
					return node.Update(node.IsParamArrayArgument, immutableArray, boundArrayInitialization, null, ConversionKind.DelegateRelaxationLevelNone, type);
				}
				SpillBuilder builder = default(SpillBuilder);
				immutableArray = SpillExpressionList(ref builder, immutableArray, firstArgumentIsAReceiverOfAMethodCall: false);
				if (boundArrayInitialization != null)
				{
					boundArrayInitialization = boundArrayInitialization.Update(SpillExpressionList(ref builder, boundArrayInitialization.Initializers, firstArgumentIsAReceiverOfAMethodCall: false), boundArrayInitialization.Type);
				}
				return builder.BuildSequenceAndFree(F, node.Update(node.IsParamArrayArgument, immutableArray, boundArrayInitialization, null, ConversionKind.DelegateRelaxationLevelNone, type));
			}

			private BoundExpression VisitArrayInitializationParts(BoundArrayInitialization node)
			{
				ImmutableArray<BoundExpression> initializers = node.Initializers;
				int length = initializers.Length;
				BoundExpression[] array = new BoundExpression[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					BoundExpression boundExpression = initializers[i];
					array[i] = ((boundExpression.Kind == BoundKind.ArrayInitialization) ? VisitArrayInitializationParts((BoundArrayInitialization)boundExpression) : VisitExpression(boundExpression));
				}
				TypeSymbol type = VisitType(node.Type);
				return node.Update(array.AsImmutableOrNull(), type);
			}

			public override BoundNode VisitArrayInitialization(BoundArrayInitialization node)
			{
				if (node == null)
				{
					return null;
				}
				return VisitArrayInitializationParts(node);
			}

			public override BoundNode VisitArrayAccess(BoundArrayAccess node)
			{
				BoundArrayAccess boundArrayAccess = (BoundArrayAccess)base.VisitArrayAccess(node);
				BoundExpression expression = boundArrayAccess.Expression;
				ImmutableArray<BoundExpression> indices = boundArrayAccess.Indices;
				if (!NeedsSpill(expression) && !NeedsSpill(indices))
				{
					return boundArrayAccess;
				}
				SpillBuilder spillBuilder = default(SpillBuilder);
				ExpressionsWithReceiver expressionsWithReceiver = SpillExpressionsWithReceiver(expression, isReceiverOfAMethodCall: false, indices, ref spillBuilder);
				return spillBuilder.BuildSequenceAndFree(F, boundArrayAccess.Update(expressionsWithReceiver.ReceiverOpt, expressionsWithReceiver.Arguments, boundArrayAccess.IsLValue, boundArrayAccess.Type));
			}

			public override BoundNode VisitArrayLength(BoundArrayLength node)
			{
				BoundArrayLength boundArrayLength = (BoundArrayLength)base.VisitArrayLength(node);
				BoundExpression expression = boundArrayLength.Expression;
				if (!NeedsSpill(expression))
				{
					return boundArrayLength;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expression;
				return SpillSequenceWithNewValue(boundSpillSequence, boundArrayLength.Update(boundSpillSequence.ValueOpt, boundArrayLength.Type));
			}

			public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
			{
				BoundUnaryOperator boundUnaryOperator = (BoundUnaryOperator)base.VisitUnaryOperator(node);
				BoundExpression operand = boundUnaryOperator.Operand;
				if (!NeedsSpill(operand))
				{
					return boundUnaryOperator;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)operand;
				return SpillSequenceWithNewValue(boundSpillSequence, boundUnaryOperator.Update(boundUnaryOperator.OperatorKind, boundSpillSequence.ValueOpt, boundUnaryOperator.Checked, boundUnaryOperator.ConstantValueOpt, boundUnaryOperator.Type));
			}

			public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
			{
				BoundBinaryConditionalExpression boundBinaryConditionalExpression = (BoundBinaryConditionalExpression)base.VisitBinaryConditionalExpression(node);
				BoundExpression testExpression = boundBinaryConditionalExpression.TestExpression;
				BoundExpression elseExpression = boundBinaryConditionalExpression.ElseExpression;
				if (!NeedsSpill(testExpression) && !NeedsSpill(elseExpression))
				{
					return boundBinaryConditionalExpression;
				}
				SpillBuilder builder = default(SpillBuilder);
				LocalSymbol localSymbol = F.SynthesizedLocal(boundBinaryConditionalExpression.Type);
				builder.AddLocal(localSymbol);
				builder.AddStatement(MakeAssignmentStatement(testExpression, localSymbol, ref builder));
				builder.AddStatement(F.If(F.ReferenceIsNothing(F.Local(localSymbol, isLValue: false)), MakeAssignmentStatement(elseExpression, localSymbol, ref builder)));
				return builder.BuildSequenceAndFree(F, F.Local(localSymbol, isLValue: false));
			}

			public override BoundNode VisitTypeOf(BoundTypeOf node)
			{
				BoundTypeOf boundTypeOf = (BoundTypeOf)base.VisitTypeOf(node);
				BoundExpression operand = boundTypeOf.Operand;
				if (!NeedsSpill(operand))
				{
					return boundTypeOf;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)operand;
				return SpillSequenceWithNewValue(boundSpillSequence, boundTypeOf.Update(boundSpillSequence.ValueOpt, boundTypeOf.IsTypeOfIsNotExpression, boundTypeOf.TargetType, boundTypeOf.Type));
			}

			public override BoundNode VisitSequencePointExpression(BoundSequencePointExpression node)
			{
				BoundSequencePointExpression boundSequencePointExpression = (BoundSequencePointExpression)base.VisitSequencePointExpression(node);
				BoundExpression expression = boundSequencePointExpression.Expression;
				if (!NeedsSpill(expression))
				{
					return boundSequencePointExpression;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expression;
				return SpillSequenceWithNewValue(boundSpillSequence, boundSequencePointExpression.Update(boundSpillSequence.ValueOpt, boundSequencePointExpression.Type));
			}

			private static bool NeedsSpill(BoundExpression node)
			{
				if (node == null)
				{
					return false;
				}
				return node.Kind switch
				{
					BoundKind.SpillSequence => true, 
					BoundKind.ArrayInitialization => throw ExceptionUtilities.UnexpectedValue(node.Kind), 
					_ => false, 
				};
			}

			private static bool NeedsSpill(ImmutableArray<BoundExpression> nodes)
			{
				if (!nodes.IsEmpty)
				{
					ImmutableArray<BoundExpression>.Enumerator enumerator = nodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (NeedsSpill(enumerator.Current))
						{
							return true;
						}
					}
				}
				return false;
			}

			private static bool ArrayInitializerNeedsSpill(BoundArrayInitialization node)
			{
				if (node == null)
				{
					return false;
				}
				ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					if (current.Kind == BoundKind.ArrayInitialization)
					{
						if (ArrayInitializerNeedsSpill((BoundArrayInitialization)current))
						{
							return true;
						}
					}
					else if (NeedsSpill(current))
					{
						return true;
					}
				}
				return false;
			}

			private ExpressionsWithReceiver SpillExpressionsWithReceiver(BoundExpression receiverOpt, bool isReceiverOfAMethodCall, ImmutableArray<BoundExpression> expressions, [In][Out] ref SpillBuilder spillBuilder)
			{
				ExpressionsWithReceiver result;
				if (receiverOpt == null)
				{
					result = new ExpressionsWithReceiver(null, SpillExpressionList(ref spillBuilder, expressions, firstArgumentIsAReceiverOfAMethodCall: false));
				}
				else
				{
					ImmutableArray<BoundExpression> expressions2 = ImmutableArray.Create(receiverOpt).Concat(expressions);
					ImmutableArray<BoundExpression> immutableArray = SpillExpressionList(ref spillBuilder, expressions2, isReceiverOfAMethodCall);
					result = new ExpressionsWithReceiver(immutableArray.First(), immutableArray.RemoveAt(0));
				}
				return result;
			}

			private ImmutableArray<BoundExpression> SpillExpressionList([In][Out] ref SpillBuilder builder, ImmutableArray<BoundExpression> expressions, bool firstArgumentIsAReceiverOfAMethodCall)
			{
				ArrayBuilder<SpillBuilder> instance = ArrayBuilder<SpillBuilder>.GetInstance();
				bool spilledFirstArg = false;
				ImmutableArray<BoundExpression> result = SpillArgumentListInner(expressions, instance, firstArgumentIsAReceiverOfAMethodCall, ref spilledFirstArg);
				for (int i = instance.Count - 1; i >= 0; i += -1)
				{
					ArrayBuilder<SpillBuilder> arrayBuilder;
					int index;
					SpillBuilder spill = (arrayBuilder = instance)[index = i];
					builder.AddSpill(ref spill);
					arrayBuilder[index] = spill;
					instance[i].Free();
				}
				instance.Free();
				return result;
			}

			private ImmutableArray<BoundExpression> SpillExpressionList([In][Out] ref SpillBuilder builder, params BoundExpression[] expressions)
			{
				return SpillExpressionList(ref builder, expressions.AsImmutableOrNull(), firstArgumentIsAReceiverOfAMethodCall: false);
			}

			private ImmutableArray<BoundExpression> SpillArgumentListInner(ImmutableArray<BoundExpression> arguments, ArrayBuilder<SpillBuilder> spillBuilders, bool firstArgumentIsAReceiverOfAMethodCall, [In][Out] ref bool spilledFirstArg)
			{
				BoundExpression[] array = new BoundExpression[arguments.Length - 1 + 1];
				for (int i = arguments.Length - 1; i >= 0; i += -1)
				{
					BoundExpression boundExpression = arguments[i];
					if (boundExpression.Kind == BoundKind.ArrayInitialization)
					{
						BoundArrayInitialization boundArrayInitialization = (BoundArrayInitialization)boundExpression;
						ImmutableArray<BoundExpression> initializers = SpillArgumentListInner(boundArrayInitialization.Initializers, spillBuilders, firstArgumentIsAReceiverOfAMethodCall: false, ref spilledFirstArg);
						array[i] = boundArrayInitialization.Update(initializers, boundArrayInitialization.Type);
						continue;
					}
					SpillBuilder builder = default(SpillBuilder);
					BoundExpression boundExpression2;
					if (!spilledFirstArg)
					{
						if (boundExpression.Kind == BoundKind.SpillSequence)
						{
							spilledFirstArg = true;
							BoundSpillSequence boundSpillSequence = (BoundSpillSequence)boundExpression;
							builder.AddSpill(boundSpillSequence);
							boundExpression2 = boundSpillSequence.ValueOpt;
						}
						else
						{
							boundExpression2 = boundExpression;
						}
					}
					else
					{
						boundExpression2 = SpillValue(boundExpression, i == 0 && firstArgumentIsAReceiverOfAMethodCall, evaluateSideEffects: true, ref builder);
					}
					array[i] = boundExpression2;
					if (!builder.IsEmpty)
					{
						spillBuilders.Add(builder);
					}
				}
				return array.AsImmutableOrNull();
			}

			private BoundExpression SpillValue(BoundExpression expr, [In][Out] ref SpillBuilder builder)
			{
				return SpillValue(expr, isReceiver: false, evaluateSideEffects: true, ref builder);
			}

			private BoundExpression SpillValue(BoundExpression expr, bool isReceiver, bool evaluateSideEffects, [In][Out] ref SpillBuilder builder)
			{
				if (Unspillable(expr))
				{
					return expr;
				}
				if (isReceiver || expr.IsLValue)
				{
					return SpillLValue(expr, isReceiver, evaluateSideEffects, ref builder);
				}
				return SpillRValue(expr, ref builder);
			}

			private BoundExpression SpillLValue(BoundExpression expr, bool isReceiver, bool evaluateSideEffects, [In][Out] ref SpillBuilder builder, bool isAssignmentTarget = false)
			{
				if (isReceiver && expr.Type.IsReferenceType && !TypeSymbolExtensions.IsTypeParameter(expr.Type))
				{
					return SpillRValue(expr.MakeRValue(), ref builder);
				}
				switch (expr.Kind)
				{
				case BoundKind.Sequence:
				{
					BoundSequence boundSequence = (BoundSequence)expr;
					builder.AddLocals(boundSequence.Locals);
					ImmutableArray<BoundExpression> sideEffects = boundSequence.SideEffects;
					if (!sideEffects.IsEmpty)
					{
						ImmutableArray<BoundExpression>.Enumerator enumerator = sideEffects.GetEnumerator();
						while (enumerator.MoveNext())
						{
							BoundExpression current = enumerator.Current;
							if (NeedsSpill(current))
							{
								BoundSpillSequence expression = (BoundSpillSequence)current;
								builder.AssumeFieldsIfNeeded(ref expression);
								builder.AddStatement(RewriteSpillSequenceIntoBlock(expression, addValueAsExpression: true));
							}
							else
							{
								builder.AddStatement(F.ExpressionStatement(current));
							}
						}
					}
					return SpillLValue(boundSequence.ValueOpt, evaluateSideEffects, isReceiver, ref builder);
				}
				case BoundKind.SpillSequence:
				{
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expr;
					builder.AddSpill(boundSpillSequence);
					return SpillLValue(boundSpillSequence.ValueOpt, isReceiver, evaluateSideEffects, ref builder);
				}
				case BoundKind.ArrayAccess:
				{
					BoundArrayAccess boundArrayAccess = (BoundArrayAccess)expr;
					BoundExpression expression2 = SpillRValue(boundArrayAccess.Expression, ref builder);
					ImmutableArray<BoundExpression> indices = boundArrayAccess.Indices;
					BoundExpression[] array = new BoundExpression[indices.Length - 1 + 1];
					int num = indices.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = SpillRValue(indices[i], ref builder);
					}
					boundArrayAccess = boundArrayAccess.Update(expression2, array.AsImmutableOrNull(), boundArrayAccess.IsLValue, boundArrayAccess.Type);
					if (evaluateSideEffects && !isAssignmentTarget)
					{
						builder.AddStatement(F.ExpressionStatement(boundArrayAccess));
					}
					return boundArrayAccess;
				}
				case BoundKind.FieldAccess:
				{
					BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
					if (Unspillable(boundFieldAccess.ReceiverOpt))
					{
						return boundFieldAccess;
					}
					bool flag = (evaluateSideEffects && !isAssignmentTarget) & boundFieldAccess.FieldSymbol.ContainingType.IsReferenceType;
					BoundExpression receiverOpt = SpillValue(boundFieldAccess.ReceiverOpt, isReceiver: true, evaluateSideEffects && !flag, ref builder);
					boundFieldAccess = boundFieldAccess.Update(receiverOpt, boundFieldAccess.FieldSymbol, boundFieldAccess.IsLValue, boundFieldAccess.SuppressVirtualCalls, null, boundFieldAccess.Type);
					if (flag)
					{
						builder.AddStatement(F.ExpressionStatement(boundFieldAccess));
					}
					return boundFieldAccess;
				}
				case BoundKind.Local:
					return expr;
				case BoundKind.Parameter:
					return expr;
				default:
					return SpillRValue(expr, ref builder);
				}
			}

			private BoundExpression SpillRValue(BoundExpression expr, [In][Out] ref SpillBuilder builder)
			{
				switch (expr.Kind)
				{
				case BoundKind.Literal:
					return expr;
				case BoundKind.SpillSequence:
				{
					BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expr;
					builder.AddSpill(boundSpillSequence);
					return SpillRValue(boundSpillSequence.ValueOpt, ref builder);
				}
				case BoundKind.ArrayInitialization:
				{
					BoundArrayInitialization boundArrayInitialization = (BoundArrayInitialization)expr;
					return boundArrayInitialization.Update(SpillExpressionList(ref builder, boundArrayInitialization.Initializers, firstArgumentIsAReceiverOfAMethodCall: false), boundArrayInitialization.Type);
				}
				case BoundKind.ConditionalAccessReceiverPlaceholder:
					if (_conditionalAccessReceiverPlaceholderReplacementInfo == null || _conditionalAccessReceiverPlaceholderReplacementInfo.PlaceholderId != ((BoundConditionalAccessReceiverPlaceholder)expr).PlaceholderId)
					{
						throw ExceptionUtilities.Unreachable;
					}
					_conditionalAccessReceiverPlaceholderReplacementInfo.IsSpilled = true;
					return expr;
				case BoundKind.ComplexConditionalAccessReceiver:
					throw ExceptionUtilities.Unreachable;
				default:
				{
					FieldSymbol fieldSymbol = _spillFieldAllocator.AllocateField(expr.Type);
					BoundStatement boundStatement = F.Assignment(F.Field(F.Me(), fieldSymbol, isLValue: true), expr);
					if (expr.Kind == BoundKind.SpillSequence)
					{
						boundStatement = RewriteSpillSequenceIntoBlock((BoundSpillSequence)expr, true, boundStatement);
					}
					builder.AddFieldWithInitialization(fieldSymbol, boundStatement);
					return F.Field(F.Me(), fieldSymbol, isLValue: false);
				}
				}
			}

			private BoundBlock RewriteSpillSequenceIntoBlock(BoundSpillSequence spill, bool addValueAsExpression)
			{
				return RewriteSpillSequenceIntoBlock(spill, addValueAsExpression, Array.Empty<BoundStatement>());
			}

			private BoundBlock RewriteSpillSequenceIntoBlock(BoundSpillSequence spill, bool addValueAsExpression, params BoundStatement[] additional)
			{
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				instance.AddRange(spill.Statements);
				if (addValueAsExpression && spill.ValueOpt != null)
				{
					instance.Add(F.ExpressionStatement(spill.ValueOpt));
				}
				instance.AddRange(additional);
				ImmutableArray<FieldSymbol> spillFields = spill.SpillFields;
				int num = spillFields.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					FieldSymbol fieldSymbol = spillFields[i];
					if (TypeNeedsClearing(fieldSymbol.Type))
					{
						instance.Add(F.Assignment(F.Field(F.Me(), fieldSymbol, isLValue: true), F.Null(fieldSymbol.Type)));
					}
					_spillFieldAllocator.FreeField(fieldSymbol);
				}
				return F.Block(spill.Locals, instance.ToImmutableAndFree());
			}

			private bool TypeNeedsClearing(TypeSymbol type)
			{
				bool value = false;
				if (_typesNeedingClearingCache.TryGetValue(type, out value))
				{
					return value;
				}
				if (TypeSymbolExtensions.IsArrayType(type) || TypeSymbolExtensions.IsTypeParameter(type))
				{
					_typesNeedingClearingCache.Add(type, value: true);
					return true;
				}
				if (TypeSymbolExtensions.IsErrorType(type) || TypeSymbolExtensions.IsEnumType(type))
				{
					_typesNeedingClearingCache.Add(type, value: false);
					return false;
				}
				switch (type.SpecialType)
				{
				case SpecialType.System_Void:
				case SpecialType.System_Boolean:
				case SpecialType.System_Char:
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
				case SpecialType.System_IntPtr:
				case SpecialType.System_UIntPtr:
				case SpecialType.System_TypedReference:
				case SpecialType.System_ArgIterator:
				case SpecialType.System_RuntimeArgumentHandle:
					value = false;
					break;
				case SpecialType.System_Object:
				case SpecialType.System_String:
					value = true;
					break;
				default:
				{
					if (type is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
					{
						value = true;
						break;
					}
					if (type.TypeKind != TypeKind.Struct)
					{
						value = true;
						break;
					}
					_typesNeedingClearingCache.Add(type, value: true);
					value = false;
					ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembersUnordered().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (current.IsShared)
						{
							continue;
						}
						switch (current.Kind)
						{
						case SymbolKind.Event:
							if (TypeNeedsClearing(((EventSymbol)current).AssociatedField.Type))
							{
								value = true;
							}
							break;
						case SymbolKind.Field:
							if (TypeNeedsClearing(((FieldSymbol)current).Type))
							{
								value = true;
							}
							break;
						}
					}
					_typesNeedingClearingCache.Remove(type);
					break;
				}
				}
				_typesNeedingClearingCache.Add(type, value);
				return value;
			}

			private static bool Unspillable(BoundExpression node)
			{
				if (node == null)
				{
					return true;
				}
				switch (node.Kind)
				{
				case BoundKind.Literal:
					return true;
				case BoundKind.MeReference:
					return true;
				case BoundKind.MyBaseReference:
				case BoundKind.MyClassReference:
					throw ExceptionUtilities.UnexpectedValue(node.Kind);
				case BoundKind.TypeExpression:
					return true;
				default:
					return false;
				}
			}

			private static BoundSpillSequence SpillSequenceWithNewValue(BoundSpillSequence spill, BoundExpression newValue)
			{
				return spill.Update(spill.Locals, spill.SpillFields, spill.Statements, newValue, newValue.Type);
			}

			public override BoundNode VisitReturnStatement(BoundReturnStatement node)
			{
				BoundExpression expressionOpt = ((BoundReturnStatement)base.VisitReturnStatement(node)).ExpressionOpt;
				if (expressionOpt != null)
				{
					if (expressionOpt.Kind == BoundKind.SpillSequence)
					{
						BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expressionOpt;
						return F.Block(RewriteSpillSequenceIntoBlock(boundSpillSequence, false, F.Assignment(F.Local(_exprRetValue, isLValue: true), boundSpillSequence.ValueOpt)), F.Goto(_exprReturnLabel));
					}
					return F.Block(F.Assignment(F.Local(_exprRetValue, isLValue: true), expressionOpt), F.Goto(_exprReturnLabel));
				}
				return F.Goto(_exprReturnLabel);
			}

			public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
			{
				BoundExpressionStatement boundExpressionStatement = (BoundExpressionStatement)base.VisitExpressionStatement(node);
				BoundExpression expression = boundExpressionStatement.Expression;
				if (expression.Kind != BoundKind.SpillSequence)
				{
					return boundExpressionStatement;
				}
				return RewriteSpillSequenceIntoBlock((BoundSpillSequence)expression, addValueAsExpression: true);
			}

			public override BoundNode VisitThrowStatement(BoundThrowStatement node)
			{
				BoundThrowStatement boundThrowStatement = (BoundThrowStatement)base.VisitThrowStatement(node);
				BoundExpression expressionOpt = boundThrowStatement.ExpressionOpt;
				if (expressionOpt == null || expressionOpt.Kind != BoundKind.SpillSequence)
				{
					return boundThrowStatement;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)expressionOpt;
				return RewriteSpillSequenceIntoBlock(boundSpillSequence, false, boundThrowStatement.Update(boundSpillSequence.ValueOpt));
			}

			public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
			{
				BoundConditionalGoto boundConditionalGoto = (BoundConditionalGoto)base.VisitConditionalGoto(node);
				BoundExpression condition = boundConditionalGoto.Condition;
				if (condition.Kind != BoundKind.SpillSequence)
				{
					return boundConditionalGoto;
				}
				BoundSpillSequence boundSpillSequence = (BoundSpillSequence)condition;
				return RewriteSpillSequenceIntoBlock(boundSpillSequence, false, node.Update(boundSpillSequence.ValueOpt, node.JumpIfTrue, node.Label));
			}

			internal AsyncMethodToClassRewriter(MethodSymbol method, SyntheticBoundNodeFactory F, FieldSymbol state, FieldSymbol builder, IReadOnlySet<Symbol> hoistedVariables, Dictionary<Symbol, CapturedSymbolOrExpression> nonReusableLocalProxies, SynthesizedLocalOrdinalsDispenser synthesizedLocalOrdinals, VariableSlotAllocator slotAllocatorOpt, int nextFreeHoistedLocalSlot, AsyncRewriter owner, BindingDiagnosticBag diagnostics)
				: base(F, state, hoistedVariables, nonReusableLocalProxies, synthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics)
			{
				_conditionalAccessReceiverPlaceholderReplacementInfo = null;
				_exprRetValue = null;
				_awaiterFields = new Dictionary<TypeSymbol, FieldSymbol>();
				_typesNeedingClearingCache = new Dictionary<TypeSymbol, bool>();
				_method = method;
				_builder = builder;
				_exprReturnLabel = F.GenerateLabel("exprReturn");
				_exitLabel = F.GenerateLabel("exitLabel");
				_owner = owner;
				_asyncMethodKind = GetAsyncMethodKind(_method);
				_spillFieldAllocator = new SpillFieldAllocator(F);
				if (_asyncMethodKind == AsyncMethodKind.GenericTaskFunction)
				{
					_exprRetValue = base.F.SynthesizedLocal(_owner._resultType, SynthesizedLocalKind.AsyncMethodReturnValue, F.Syntax);
				}
				_nextAwaiterId = slotAllocatorOpt?.PreviousAwaiterSlotCount ?? 0;
			}

			private FieldSymbol GetAwaiterField(TypeSymbol awaiterType)
			{
				FieldSymbol value = null;
				if (!_awaiterFields.TryGetValue(awaiterType, out value))
				{
					int slotIndex = -1;
					if (SlotAllocatorOpt == null || !SlotAllocatorOpt.TryGetPreviousAwaiterSlotIndex(F.CompilationState.ModuleBuilderOpt.Translate(awaiterType, F.Syntax, F.Diagnostics.DiagnosticBag), F.Diagnostics.DiagnosticBag, out slotIndex))
					{
						slotIndex = _nextAwaiterId;
						_nextAwaiterId++;
					}
					string name = GeneratedNames.MakeStateMachineAwaiterFieldName(slotIndex);
					value = F.StateMachineField(awaiterType, _method, name, SynthesizedLocalKind.AwaiterField, slotIndex, Accessibility.Internal);
					_awaiterFields.Add(awaiterType, value);
				}
				return value;
			}

			internal void GenerateMoveNext(BoundStatement body, MethodSymbol moveNextMethod)
			{
				F.CurrentMethod = moveNextMethod;
				BoundStatement statement = (BoundStatement)Visit(body);
				ImmutableArray<FieldSymbol> hoistedLocals = default(ImmutableArray<FieldSymbol>);
				TryUnwrapBoundStateMachineScope(ref statement, out hoistedLocals);
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				instance.Add(F.Assignment(F.Local(CachedState, isLValue: true), F.Field(F.Me(), StateField, isLValue: false)));
				LocalSymbol localSymbol = F.SynthesizedLocal(F.WellKnownType(WellKnownType.System_Exception));
				instance.Add(F.Try(F.Block(ImmutableArray<LocalSymbol>.Empty, SyntheticBoundNodeFactory.HiddenSequencePoint(), Dispatch(), statement), F.CatchBlocks(F.Catch(localSymbol, F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.Literal(StateMachineStates.FinishedStateMachine)), F.ExpressionStatement(_owner.GenerateMethodCall(F.Field(F.Me(), _builder, isLValue: false), _owner._builderType, "SetException", F.Local(localSymbol, isLValue: false))), F.Goto(_exitLabel)), isSynthesizedAsyncCatchAll: true))));
				instance.Add(F.Label(_exprReturnLabel));
				BoundExpressionStatement boundExpressionStatement = F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(StateMachineStates.FinishedStateMachine)));
				if (!(body.Syntax is MethodBlockSyntax methodBlockSyntax))
				{
					instance.Add(boundExpressionStatement);
				}
				else
				{
					instance.Add(F.SequencePointWithSpan(methodBlockSyntax, methodBlockSyntax.EndBlockStatement.Span, boundExpressionStatement));
					instance.Add(SyntheticBoundNodeFactory.HiddenSequencePoint());
				}
				instance.Add(F.ExpressionStatement(_owner.GenerateMethodCall(F.Field(F.Me(), _builder, isLValue: false), _owner._builderType, "SetResult", (_asyncMethodKind != AsyncMethodKind.GenericTaskFunction) ? Array.Empty<BoundExpression>() : new BoundExpression[1] { F.Local(_exprRetValue, isLValue: false) })));
				instance.Add(F.Label(_exitLabel));
				instance.Add(F.Return());
				ImmutableArray<BoundStatement> statements = instance.ToImmutableAndFree();
				BoundBlock boundBlock = F.Block(((object)_exprRetValue != null) ? ImmutableArray.Create(_exprRetValue, CachedState) : ImmutableArray.Create(CachedState), statements);
				if (hoistedLocals.Length > 0)
				{
					boundBlock = MakeStateMachineScope(hoistedLocals, boundBlock);
				}
				_owner.CloseMethod(boundBlock);
			}

			protected override BoundStatement GenerateReturn(bool finished)
			{
				return F.Goto(_exitLabel);
			}
		}

		internal abstract class CapturedSymbolOrExpression
		{
			internal abstract BoundExpression Materialize(AsyncMethodToClassRewriter rewriter, bool isLValue);

			internal virtual void AddProxyFieldsForStateMachineScope(ArrayBuilder<FieldSymbol> proxyFields)
			{
			}

			internal abstract void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue);
		}

		private class CapturedConstantExpression : CapturedSymbolOrExpression
		{
			private readonly ConstantValue _constValue;

			private readonly TypeSymbol _type;

			public CapturedConstantExpression(ConstantValue constValue, TypeSymbol type)
			{
				_constValue = constValue;
				_type = type;
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
			}

			internal override BoundExpression Materialize(AsyncMethodToClassRewriter rewriter, bool isLValue)
			{
				return new BoundLiteral(rewriter.F.Syntax, _constValue, _type);
			}
		}

		private abstract class SingleFieldCapture : CapturedSymbolOrExpression
		{
			internal readonly FieldSymbol Field;

			public SingleFieldCapture(FieldSymbol field)
			{
				Field = field;
			}

			internal override BoundExpression Materialize(AsyncMethodToClassRewriter rewriter, bool isLValue)
			{
				SyntaxNode syntax = rewriter.F.Syntax;
				BoundExpression boundExpression = rewriter.FramePointer(syntax, Field.ContainingType);
				FieldSymbol f = Field.AsMember((NamedTypeSymbol)boundExpression.Type);
				return rewriter.F.Field(boundExpression, f, isLValue);
			}
		}

		private class CapturedParameterSymbol : SingleFieldCapture
		{
			public CapturedParameterSymbol(FieldSymbol field)
				: base(field)
			{
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
			}
		}

		private class CapturedLocalSymbol : SingleFieldCapture
		{
			internal readonly LocalSymbol Local;

			public CapturedLocalSymbol(FieldSymbol field, LocalSymbol local)
				: base(field)
			{
				Local = local;
			}

			internal override void AddProxyFieldsForStateMachineScope(ArrayBuilder<FieldSymbol> proxyFields)
			{
				proxyFields.Add(Field);
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
			}
		}

		private class CapturedRValueExpression : SingleFieldCapture
		{
			internal readonly BoundExpression Expression;

			public CapturedRValueExpression(FieldSymbol field, BoundExpression expr)
				: base(field)
			{
				Expression = expr;
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
				prologue.Add(rewriter.ProcessRewrittenAssignmentOperator(rewriter.F.AssignmentExpression(Materialize(rewriter, isLValue: true), rewriter.VisitExpression(Expression))));
			}
		}

		private class CapturedFieldAccessExpression : CapturedSymbolOrExpression
		{
			internal readonly CapturedSymbolOrExpression ReceiverOpt;

			internal readonly FieldSymbol Field;

			public CapturedFieldAccessExpression(CapturedSymbolOrExpression receiverOpt, FieldSymbol field)
			{
				ReceiverOpt = receiverOpt;
				Field = field;
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
				if (ReceiverOpt != null)
				{
					ReceiverOpt.CreateCaptureInitializationCode(rewriter, prologue);
				}
			}

			internal override BoundExpression Materialize(AsyncMethodToClassRewriter rewriter, bool isLValue)
			{
				BoundExpression receiver = null;
				if (ReceiverOpt != null)
				{
					receiver = ReceiverOpt.Materialize(rewriter, Field.ContainingType.IsValueType);
				}
				return rewriter.F.Field(receiver, Field, isLValue);
			}
		}

		private class CapturedArrayAccessExpression : CapturedSymbolOrExpression
		{
			internal readonly CapturedSymbolOrExpression ArrayPointer;

			internal readonly ImmutableArray<CapturedSymbolOrExpression> Indices;

			public CapturedArrayAccessExpression(CapturedSymbolOrExpression arrayPointer, ImmutableArray<CapturedSymbolOrExpression> indices)
			{
				ArrayPointer = arrayPointer;
				Indices = indices;
			}

			internal override void CreateCaptureInitializationCode(AsyncMethodToClassRewriter rewriter, ArrayBuilder<BoundExpression> prologue)
			{
				ArrayPointer.CreateCaptureInitializationCode(rewriter, prologue);
				ImmutableArray<CapturedSymbolOrExpression>.Enumerator enumerator = Indices.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.CreateCaptureInitializationCode(rewriter, prologue);
				}
			}

			internal override BoundExpression Materialize(AsyncMethodToClassRewriter rewriter, bool isLValue)
			{
				ImmutableArray<CapturedSymbolOrExpression> indices = Indices;
				int length = indices.Length;
				BoundExpression[] array = new BoundExpression[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = indices[i].Materialize(rewriter, isLValue: false);
				}
				BoundExpression boundExpression = ArrayPointer.Materialize(rewriter, isLValue: false);
				_ = ((ArrayTypeSymbol)boundExpression.Type).ElementType;
				return rewriter.F.ArrayAccess(boundExpression, isLValue, array);
			}
		}

		private struct SpillBuilder
		{
			private ArrayBuilder<LocalSymbol> _locals;

			private ArrayBuilder<FieldSymbol> _fields;

			private ArrayBuilder<BoundStatement> _statements;

			public bool IsEmpty
			{
				get
				{
					if (_locals == null && _statements == null)
					{
						return _fields == null;
					}
					return false;
				}
			}

			internal void Free()
			{
				if (_locals != null)
				{
					_locals.Free();
					_locals = null;
				}
				if (_statements != null)
				{
					_statements.Free();
					_statements = null;
				}
				if (_fields != null)
				{
					_fields.Free();
					_fields = null;
				}
			}

			internal BoundExpression BuildSequenceAndFree(SyntheticBoundNodeFactory F, BoundExpression expression)
			{
				if (!IsEmpty)
				{
					expression = F.SpillSequence((_locals == null) ? ImmutableArray<LocalSymbol>.Empty : _locals.ToImmutableAndFree(), (_fields == null) ? ImmutableArray<FieldSymbol>.Empty : _fields.ToImmutableAndFree(), (_statements == null) ? ImmutableArray<BoundStatement>.Empty : _statements.ToImmutableAndFree(), expression);
					_locals = null;
					_statements = null;
					_fields = null;
				}
				return expression;
			}

			internal void AddSpill([In] ref SpillBuilder spill)
			{
				if (!spill.IsEmpty)
				{
					AddRange(ref _locals, spill._locals);
					AddRange(ref _fields, spill._fields);
					AddRange(ref _statements, spill._statements);
				}
			}

			internal void AddSpill(BoundSpillSequence spill)
			{
				AddRange(ref _locals, spill.Locals);
				AddRange(ref _fields, spill.SpillFields);
				AddRange(ref _statements, spill.Statements);
			}

			internal void AddFieldWithInitialization(FieldSymbol field, BoundStatement init)
			{
				Add(ref _fields, field);
				Add(ref _statements, init);
			}

			internal void AddLocal(LocalSymbol local)
			{
				Add(ref _locals, local);
			}

			internal void AddLocals(ImmutableArray<LocalSymbol> locals)
			{
				AddRange(ref _locals, locals);
			}

			internal void AddStatement(BoundStatement statement)
			{
				Add(ref _statements, statement);
			}

			internal void AssumeFieldsIfNeeded([In][Out] ref BoundSpillSequence expression)
			{
				if (!expression.SpillFields.IsEmpty)
				{
					AddRange(ref _fields, expression.SpillFields);
					expression = expression.Update(expression.Locals, ImmutableArray<FieldSymbol>.Empty, expression.Statements, expression.ValueOpt, expression.Type);
				}
			}

			private static void EnsureArrayBuilder<T>([In][Out] ref ArrayBuilder<T> array)
			{
				if (array == null)
				{
					array = ArrayBuilder<T>.GetInstance();
				}
			}

			private static void Add<T>([In][Out] ref ArrayBuilder<T> array, T element)
			{
				EnsureArrayBuilder(ref array);
				array.Add(element);
			}

			private static void AddRange<T>([In][Out] ref ArrayBuilder<T> array, ArrayBuilder<T> other)
			{
				if (other != null && other.Count != 0)
				{
					EnsureArrayBuilder(ref array);
					array.AddRange(other);
				}
			}

			private static void AddRange<T>([In][Out] ref ArrayBuilder<T> array, ImmutableArray<T> other)
			{
				if (!other.IsEmpty)
				{
					EnsureArrayBuilder(ref array);
					array.AddRange(other);
				}
			}
		}

		private class SpillFieldAllocator
		{
			private readonly SyntheticBoundNodeFactory _F;

			private readonly KeyedStack<TypeSymbol, FieldSymbol> _allocatedFields;

			private readonly HashSet<FieldSymbol> _realizedSpills;

			private int _nextHoistedFieldId;

			internal SpillFieldAllocator(SyntheticBoundNodeFactory f)
			{
				_allocatedFields = new KeyedStack<TypeSymbol, FieldSymbol>();
				_realizedSpills = new HashSet<FieldSymbol>(ReferenceEqualityComparer.Instance);
				_F = f;
				_nextHoistedFieldId = 0;
			}

			internal FieldSymbol AllocateField(TypeSymbol type)
			{
				FieldSymbol value = null;
				if (!_allocatedFields.TryPop(type, out value))
				{
					_nextHoistedFieldId++;
					value = _F.StateMachineField(type, _F.CurrentMethod, GeneratedNames.ReusableHoistedLocalFieldName(_nextHoistedFieldId), Accessibility.Internal);
				}
				_realizedSpills.Add(value);
				return value;
			}

			internal void FreeField(FieldSymbol field)
			{
				_realizedSpills.Remove(field);
				_allocatedFields.Push(field.Type, field);
			}
		}

		internal enum AsyncMethodKind
		{
			None,
			Sub,
			TaskFunction,
			GenericTaskFunction
		}

		private readonly Binder _binder;

		private readonly LookupOptions _lookupOptions;

		private readonly AsyncMethodKind _asyncMethodKind;

		private readonly NamedTypeSymbol _builderType;

		private readonly TypeSymbol _resultType;

		private FieldSymbol _builderField;

		private int _lastExpressionCaptureNumber;

		protected override bool PreserveInitialParameterValues => false;

		internal override TypeSubstitution TypeMap => StateMachineType.TypeSubstitution;

		public AsyncRewriter(BoundStatement body, MethodSymbol method, AsyncStateMachine stateMachineType, VariableSlotAllocator slotAllocatorOpt, AsyncMethodKind asyncKind, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
			: base(body, method, (StateMachineTypeSymbol)stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
		{
			_binder = CreateMethodBinder(method);
			_lookupOptions = LookupOptions.NoBaseClassLookup | LookupOptions.AllMethodsOfAnyArity | LookupOptions.IgnoreExtensionMethods;
			if (compilationState.ModuleBuilderOpt.IgnoreAccessibility)
			{
				_binder = new IgnoreAccessibilityBinder(_binder);
				_lookupOptions |= LookupOptions.IgnoreAccessibility;
			}
			_asyncMethodKind = asyncKind;
			switch (_asyncMethodKind)
			{
			case AsyncMethodKind.Sub:
				_resultType = F.SpecialType(SpecialType.System_Void);
				_builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder);
				break;
			case AsyncMethodKind.TaskFunction:
				_resultType = F.SpecialType(SpecialType.System_Void);
				_builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder);
				break;
			case AsyncMethodKind.GenericTaskFunction:
				_resultType = ((NamedTypeSymbol)Method.ReturnType).TypeArgumentsNoUseSiteDiagnostics.Single().InternalSubstituteTypeParameters(TypeMap).Type;
				_builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T).Construct(_resultType);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(_asyncMethodKind);
			}
		}

		internal static BoundBlock Rewrite(BoundBlock body, MethodSymbol method, int methodOrdinal, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out AsyncStateMachine stateMachineType)
		{
			if (body.HasErrors)
			{
				return body;
			}
			AsyncMethodKind asyncMethodKind = GetAsyncMethodKind(method);
			if (asyncMethodKind == AsyncMethodKind.None)
			{
				return body;
			}
			TypeKind typeKind = (compilationState.Compilation.Options.EnableEditAndContinue ? TypeKind.Class : TypeKind.Struct);
			stateMachineType = new AsyncStateMachine(slotAllocatorOpt, compilationState, method, methodOrdinal, typeKind);
			compilationState.ModuleBuilderOpt.CompilationState.SetStateMachineType(method, stateMachineType);
			AsyncRewriter asyncRewriter = new AsyncRewriter(body, method, stateMachineType, slotAllocatorOpt, asyncMethodKind, compilationState, diagnostics);
			if (asyncRewriter.EnsureAllSymbolsAndSignature())
			{
				return body;
			}
			return asyncRewriter.Rewrite();
		}

		private static Binder CreateMethodBinder(MethodSymbol method)
		{
			if (method is SourceMethodSymbol sourceMethodSymbol)
			{
				return BinderBuilder.CreateBinderForMethodBody((SourceModuleSymbol)sourceMethodSymbol.ContainingModule, LocationExtensions.PossiblyEmbeddedOrMySourceTree(sourceMethodSymbol.ContainingType.Locations[0]), sourceMethodSymbol);
			}
			NamedTypeSymbol containingType = method.ContainingType;
			while ((object)containingType != null)
			{
				SyntaxTree syntaxTree = containingType.Locations.FirstOrDefault()?.SourceTree;
				if (syntaxTree != null)
				{
					return BinderBuilder.CreateBinderForType((SourceModuleSymbol)containingType.ContainingModule, syntaxTree, containingType);
				}
				containingType = containingType.ContainingType;
			}
			throw ExceptionUtilities.Unreachable;
		}

		protected override void GenerateControlFields()
		{
			StateField = F.StateMachineField(F.SpecialType(SpecialType.System_Int32), Method, GeneratedNames.MakeStateMachineStateFieldName(), Accessibility.Public);
			_builderField = F.StateMachineField(_builderType, Method, GeneratedNames.MakeStateMachineBuilderFieldName(), Accessibility.Public);
		}

		protected override void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal)
		{
			if (frameType.TypeKind == TypeKind.Class)
			{
				bodyBuilder.Add(F.Assignment(F.Local(stateMachineLocal, isLValue: true), F.New(SymbolExtensions.AsMember(StateMachineType.Constructor, frameType))));
			}
			else
			{
				bodyBuilder.Add(F.Assignment(F.Local(stateMachineLocal, isLValue: true), F.Null(stateMachineLocal.Type)));
			}
		}

		protected override void GenerateMethodImplementations()
		{
			GenerateMoveNext(OpenMoveNextMethodImplementation(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext, Accessibility.Internal));
			OpenMethodImplementation(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine, "System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine", Accessibility.Private);
			if (F.CurrentType.TypeKind == TypeKind.Class)
			{
				F.CloseMethod(F.Return());
			}
			else
			{
				CloseMethod(F.Block(F.ExpressionStatement(GenerateMethodCall(F.Field(F.Me(), _builderField, isLValue: false), _builderType, "SetStateMachine", F.Parameter(F.CurrentMethod.Parameters[0]))), F.Return()));
			}
			if (StateMachineType.TypeKind == TypeKind.Class)
			{
				F.CurrentMethod = StateMachineType.Constructor;
				F.CloseMethod(F.Block(ImmutableArray.Create(F.BaseInitialization(), F.Return())));
			}
		}

		protected override BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			BoundExpression left = F.Field(F.Local(stateMachineVariable, isLValue: true), StateField.AsMember(frameType), isLValue: true);
			instance.Add(F.Assignment(left, F.Literal(StateMachineStates.NotStartedStateMachine)));
			FieldSymbol fieldSymbol = _builderField.AsMember(frameType);
			BoundExpression boundExpression = F.Field(F.Local(stateMachineVariable, isLValue: true), fieldSymbol, isLValue: true);
			TypeSymbol type = fieldSymbol.Type;
			instance.Add(F.Assignment(boundExpression, GenerateMethodCall(null, type, "Create")));
			instance.Add(F.ExpressionStatement(GenerateMethodCall(boundExpression, type, "Start", ImmutableArray.Create((TypeSymbol)frameType), F.Local(stateMachineVariable, isLValue: true))));
			instance.Add((_asyncMethodKind == AsyncMethodKind.Sub) ? F.Return() : F.Return(GeneratePropertyGet(boundExpression, type, "Task")));
			return RewriteBodyIfNeeded(F.Block(ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree()), F.TopLevelMethod, Method);
		}

		private void GenerateMoveNext(MethodSymbol moveNextMethod)
		{
			new AsyncMethodToClassRewriter(Method, F, StateField, _builderField, hoistedVariables, nonReusableLocalProxies, SynthesizedLocalOrdinals, SlotAllocatorOpt, nextFreeHoistedLocalSlot, this, Diagnostics).GenerateMoveNext(Body, moveNextMethod);
		}

		internal override BoundStatement RewriteBodyIfNeeded(BoundStatement body, MethodSymbol topMethod, MethodSymbol currentMethod)
		{
			if (body.HasErrors)
			{
				return body;
			}
			HashSet<BoundNode> rewrittenNodes = null;
			bool hasLambdas = false;
			ISet<Symbol> symbolsCapturedWithoutCopyCtor = null;
			if (body.Kind != BoundKind.Block)
			{
				body = F.Block(body);
			}
			return LocalRewriter.Rewrite((BoundBlock)body, topMethod, F.CompilationState, null, Diagnostics, out rewrittenNodes, out hasLambdas, out symbolsCapturedWithoutCopyCtor, LocalRewriter.RewritingFlags.AllowSequencePoints | LocalRewriter.RewritingFlags.AllowEndOfMethodReturnWithExpression | LocalRewriter.RewritingFlags.AllowCatchWithErrorLineNumberReference, null, currentMethod);
		}

		internal override bool EnsureAllSymbolsAndSignature()
		{
			if (base.EnsureAllSymbolsAndSignature())
			{
				return true;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, Diagnostics.AccumulatesDependencies);
			EnsureSpecialType(SpecialType.System_Object, instance);
			EnsureSpecialType(SpecialType.System_Void, instance);
			EnsureSpecialType(SpecialType.System_ValueType, instance);
			EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine, instance);
			EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext, instance);
			EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine, instance);
			switch (_asyncMethodKind)
			{
			case AsyncMethodKind.GenericTaskFunction:
				EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T, instance);
				break;
			case AsyncMethodKind.TaskFunction:
				EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder, instance);
				break;
			case AsyncMethodKind.Sub:
				EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder, instance);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(_asyncMethodKind);
			}
			bool num = instance.HasAnyErrors();
			if (num)
			{
				Diagnostics.AddRange(instance);
			}
			instance.Free();
			return num;
		}

		internal static AsyncMethodKind GetAsyncMethodKind(MethodSymbol method)
		{
			if (!method.IsAsync)
			{
				return AsyncMethodKind.None;
			}
			if (method.IsSub)
			{
				return AsyncMethodKind.Sub;
			}
			VisualBasicCompilation declaringCompilation = method.DeclaringCompilation;
			TypeSymbol returnType = method.ReturnType;
			if (TypeSymbol.Equals(returnType, declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task), TypeCompareKind.ConsiderEverything))
			{
				return AsyncMethodKind.TaskFunction;
			}
			if (returnType.Kind == SymbolKind.NamedType && TypeSymbol.Equals(returnType.OriginalDefinition, declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T), TypeCompareKind.ConsiderEverything))
			{
				return AsyncMethodKind.GenericTaskFunction;
			}
			return AsyncMethodKind.None;
		}

		protected override CapturedSymbolOrExpression CreateByRefLocalCapture(TypeSubstitution typeMap, LocalSymbol local, Dictionary<LocalSymbol, BoundExpression> initializers)
		{
			return CaptureExpression(typeMap, initializers[local], initializers);
		}

		private CapturedSymbolOrExpression CaptureExpression(TypeSubstitution typeMap, BoundExpression expression, Dictionary<LocalSymbol, BoundExpression> initializers)
		{
			if (expression == null)
			{
				return null;
			}
			switch (expression.Kind)
			{
			case BoundKind.Literal:
				return new CapturedConstantExpression(expression.ConstantValueOpt, expression.Type.InternalSubstituteTypeParameters(typeMap).Type);
			case BoundKind.Local:
				return CaptureLocalSymbol(typeMap, ((BoundLocal)expression).LocalSymbol, initializers);
			case BoundKind.Parameter:
				return CaptureParameterSymbol(typeMap, ((BoundParameter)expression).ParameterSymbol);
			case BoundKind.MeReference:
				return CaptureParameterSymbol(typeMap, Method.MeParameter);
			case BoundKind.MyClassReference:
				return CaptureParameterSymbol(typeMap, Method.MeParameter);
			case BoundKind.MyBaseReference:
				return CaptureParameterSymbol(typeMap, Method.MeParameter);
			case BoundKind.FieldAccess:
				if (expression.IsLValue)
				{
					BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expression;
					return new CapturedFieldAccessExpression(CaptureExpression(typeMap, boundFieldAccess.ReceiverOpt, initializers), boundFieldAccess.FieldSymbol);
				}
				break;
			case BoundKind.ArrayAccess:
				if (expression.IsLValue)
				{
					BoundArrayAccess boundArrayAccess = (BoundArrayAccess)expression;
					CapturedSymbolOrExpression arrayPointer = CaptureExpression(typeMap, boundArrayAccess.Expression, initializers);
					ImmutableArray<BoundExpression> indices = boundArrayAccess.Indices;
					int length = indices.Length;
					CapturedSymbolOrExpression[] array = new CapturedSymbolOrExpression[length - 1 + 1];
					int num = length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = CaptureExpression(typeMap, indices[i], initializers);
					}
					return new CapturedArrayAccessExpression(arrayPointer, array.AsImmutableOrNull());
				}
				break;
			}
			_lastExpressionCaptureNumber++;
			return new CapturedRValueExpression(F.StateMachineField(expression.Type, Method, "$V" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(_lastExpressionCaptureNumber), Accessibility.Internal), expression);
		}

		protected override void InitializeParameterWithProxy(ParameterSymbol parameter, CapturedSymbolOrExpression proxy, LocalSymbol stateMachineVariable, ArrayBuilder<BoundExpression> initializers)
		{
			FieldSymbol field = ((CapturedParameterSymbol)proxy).Field;
			NamedTypeSymbol newOwner = (Method.IsGenericMethod ? StateMachineType.Construct(Method.TypeArguments) : StateMachineType);
			BoundExpression right = (parameter.IsMe ? ((BoundExpression)F.Me()) : ((BoundExpression)F.Parameter(parameter).MakeRValue()));
			initializers.Add(F.AssignmentExpression(F.Field(F.Local(stateMachineVariable, isLValue: true), field.AsMember(newOwner), isLValue: true), right));
		}

		protected override CapturedSymbolOrExpression CreateByValLocalCapture(FieldSymbol field, LocalSymbol local)
		{
			return new CapturedLocalSymbol(field, local);
		}

		protected override CapturedSymbolOrExpression CreateParameterCapture(FieldSymbol field, ParameterSymbol parameter)
		{
			return new CapturedParameterSymbol(field);
		}

		private BoundExpression GenerateMethodCall(BoundExpression receiver, TypeSymbol type, string methodName, params BoundExpression[] arguments)
		{
			return GenerateMethodCall(receiver, type, methodName, ImmutableArray<TypeSymbol>.Empty, arguments);
		}

		private BoundExpression GenerateMethodCall(BoundExpression receiver, TypeSymbol type, string methodName, ImmutableArray<TypeSymbol> typeArgs, params BoundExpression[] arguments)
		{
			BoundMethodGroup boundMethodGroup = FindMethodAndReturnMethodGroup(receiver, type, methodName, typeArgs);
			if (boundMethodGroup == null || arguments.Any((BoundExpression a) => a.HasErrors) || (receiver != null && receiver.HasErrors))
			{
				return F.BadExpression(arguments);
			}
			return _binder.BindInvocationExpression(F.Syntax, F.Syntax, TypeCharacter.None, boundMethodGroup, ImmutableArray.Create(arguments), default(ImmutableArray<string>), Diagnostics, null);
		}

		private BoundMethodGroup FindMethodAndReturnMethodGroup(BoundExpression receiver, TypeSymbol type, string methodName, ImmutableArray<TypeSymbol> typeArgs)
		{
			BoundMethodGroup boundMethodGroup = null;
			LookupResult instance = LookupResult.GetInstance();
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _binder.GetNewCompoundUseSiteInfo(Diagnostics);
			_binder.LookupMember(instance, type, methodName, 0, _lookupOptions, ref useSiteInfo);
			((BindingDiagnosticBag<AssemblySymbol>)Diagnostics).Add(F.Syntax, useSiteInfo);
			if (instance.IsGood)
			{
				_ = instance.Symbols[0];
				if (instance.Symbols[0].Kind == SymbolKind.Method)
				{
					boundMethodGroup = new BoundMethodGroup(F.Syntax, F.TypeArguments(typeArgs), instance.Symbols.ToDowncastedImmutable<MethodSymbol>(), instance.Kind, receiver, QualificationKind.QualifiedViaValue);
				}
			}
			if (boundMethodGroup == null)
			{
				Diagnostics.Add(instance.HasDiagnostic ? instance.Diagnostic : ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, methodName, type), F.Syntax.GetLocation());
			}
			instance.Free();
			return boundMethodGroup;
		}

		private BoundExpression GeneratePropertyGet(BoundExpression receiver, TypeSymbol type, string propertyName)
		{
			BoundPropertyGroup boundPropertyGroup = FindPropertyAndReturnPropertyGroup(receiver, type, propertyName);
			if (boundPropertyGroup == null || (receiver != null && receiver.HasErrors))
			{
				return F.BadExpression();
			}
			BoundExpression boundExpression = _binder.BindInvocationExpression(F.Syntax, F.Syntax, TypeCharacter.None, boundPropertyGroup, default(ImmutableArray<BoundExpression>), default(ImmutableArray<string>), Diagnostics, null);
			if (boundExpression.Kind == BoundKind.PropertyAccess)
			{
				boundExpression = ((BoundPropertyAccess)boundExpression).SetAccessKind(PropertyAccessKind.Get);
			}
			return boundExpression;
		}

		private BoundPropertyGroup FindPropertyAndReturnPropertyGroup(BoundExpression receiver, TypeSymbol type, string propertyName)
		{
			BoundPropertyGroup boundPropertyGroup = null;
			LookupResult instance = LookupResult.GetInstance();
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _binder.GetNewCompoundUseSiteInfo(Diagnostics);
			_binder.LookupMember(instance, type, propertyName, 0, _lookupOptions, ref useSiteInfo);
			((BindingDiagnosticBag<AssemblySymbol>)Diagnostics).Add(F.Syntax, useSiteInfo);
			if (instance.IsGood)
			{
				_ = instance.Symbols[0];
				if (instance.Symbols[0].Kind == SymbolKind.Property)
				{
					boundPropertyGroup = new BoundPropertyGroup(F.Syntax, instance.Symbols.ToDowncastedImmutable<PropertySymbol>(), instance.Kind, receiver, QualificationKind.QualifiedViaValue);
				}
			}
			if (boundPropertyGroup == null)
			{
				Diagnostics.Add(instance.HasDiagnostic ? instance.Diagnostic : ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, propertyName, type), F.Syntax.GetLocation());
			}
			instance.Free();
			return boundPropertyGroup;
		}
	}
}
