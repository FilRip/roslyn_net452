using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
{
	internal class StackScheduler
	{
		private enum ExprContext
		{
			None,
			Sideeffects,
			Value,
			Address,
			AssignmentTarget,
			Box
		}

		private sealed class Analyzer : BoundTreeRewriter
		{
			private readonly Symbol _container;

			private int _counter;

			private readonly ArrayBuilder<(BoundExpression expression, ExprContext context)> _evalStack;

			private readonly bool _debugFriendly;

			private ExprContext _context;

			private BoundLocal _assignmentLocal;

			private readonly Dictionary<LocalSymbol, LocalDefUseInfo> _locals;

			private readonly DummyLocal _empty;

			private readonly Dictionary<object, DummyLocal> _dummyVariables;

			private int _recursionDepth;

			private Analyzer(Symbol container, ArrayBuilder<(BoundExpression, ExprContext)> evalStack, bool debugFriendly)
			{
				_counter = 0;
				_context = ExprContext.None;
				_assignmentLocal = null;
				_locals = new Dictionary<LocalSymbol, LocalDefUseInfo>();
				_dummyVariables = new Dictionary<object, DummyLocal>();
				_container = container;
				_evalStack = evalStack;
				_debugFriendly = debugFriendly;
				_empty = new DummyLocal(container);
				DeclareLocal(_empty, 0);
				RecordDummyWrite(_empty);
			}

			public static BoundNode Analyze(Symbol container, BoundNode node, bool debugFriendly, out Dictionary<LocalSymbol, LocalDefUseInfo> locals)
			{
				ArrayBuilder<(BoundExpression, ExprContext)> instance = ArrayBuilder<(BoundExpression, ExprContext)>.GetInstance();
				Analyzer analyzer = new Analyzer(container, instance, debugFriendly);
				BoundNode result = analyzer.Visit(node);
				instance.Free();
				locals = analyzer._locals;
				return result;
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (node is BoundExpression node2)
				{
					return VisitExpression(node2, ExprContext.Value);
				}
				return VisitStatement(node);
			}

			private BoundExpression VisitExpressionCore(BoundExpression node, ExprContext context)
			{
				if (node == null)
				{
					_counter++;
					return node;
				}
				ExprContext context2 = _context;
				int stackDepth = StackDepth();
				_context = context;
				BoundExpression result = (((object)node.ConstantValueOpt == null) ? ((BoundExpression)base.Visit(node)) : node);
				_context = context2;
				_counter++;
				switch (context)
				{
				case ExprContext.Sideeffects:
					SetStackDepth(stackDepth);
					break;
				case ExprContext.Value:
				case ExprContext.Address:
				case ExprContext.Box:
					SetStackDepth(stackDepth);
					PushEvalStack(node, context);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(context);
				case ExprContext.AssignmentTarget:
					break;
				}
				return result;
			}

			private void PushEvalStack(BoundExpression result, ExprContext context)
			{
				_evalStack.Add((result, context));
			}

			private int StackDepth()
			{
				return _evalStack.Count;
			}

			private bool EvalStackIsEmpty()
			{
				return StackDepth() == 0;
			}

			private void SetStackDepth(int depth)
			{
				_evalStack.Clip(depth);
			}

			private void PopEvalStack()
			{
				SetStackDepth(_evalStack.Count - 1);
			}

			private void ClearEvalStack()
			{
				_evalStack.Clear();
			}

			private BoundExpression VisitExpression(BoundExpression node, ExprContext context)
			{
				_recursionDepth++;
				BoundExpression result;
				if (_recursionDepth > 1)
				{
					StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
					result = VisitExpressionCore(node, context);
				}
				else
				{
					result = VisitExpressionCoreWithStackGuard(node, context);
				}
				_recursionDepth--;
				return result;
			}

			private BoundExpression VisitExpressionCoreWithStackGuard(BoundExpression node, ExprContext context)
			{
				try
				{
					return VisitExpressionCore(node, context);
				}
				catch (InsufficientExecutionStackException ex)
				{
					ProjectData.SetProjectError(ex);
					InsufficientExecutionStackException inner = ex;
					throw new CancelledByStackGuardException(inner, node);
				}
			}

			protected override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override BoundNode VisitSpillSequence(BoundSpillSequence node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			private BoundNode VisitStatement(BoundNode node)
			{
				int stackDepth = StackDepth();
				ExprContext context = _context;
				BoundNode result = base.Visit(node);
				if (_debugFriendly)
				{
					EnsureOnlyEvalStack();
				}
				_context = context;
				SetStackDepth(stackDepth);
				_counter++;
				return result;
			}

			private bool LhsUsesStackWhenAssignedTo(BoundNode node, ExprContext context)
			{
				if (node == null)
				{
					return false;
				}
				switch (node.Kind)
				{
				case BoundKind.Local:
				case BoundKind.Parameter:
					return false;
				case BoundKind.FieldAccess:
					return !((BoundFieldAccess)node).FieldSymbol.IsShared;
				case BoundKind.Sequence:
					return LhsUsesStackWhenAssignedTo(((BoundSequence)node).ValueOpt, context);
				default:
					return true;
				}
			}

			public override BoundNode VisitBlock(BoundBlock node)
			{
				DeclareLocals(node.Locals, 0);
				return base.VisitBlock(node);
			}

			public override BoundNode VisitSequence(BoundSequence node)
			{
				int stack = StackDepth();
				ImmutableArray<LocalSymbol> locals = node.Locals;
				if (!locals.IsEmpty)
				{
					if (_context == ExprContext.Sideeffects)
					{
						DeclareLocals(locals, stack);
					}
					else
					{
						DeclareLocals(locals, stack);
					}
				}
				ExprContext context = _context;
				ImmutableArray<BoundExpression> sideEffects = node.SideEffects;
				ArrayBuilder<BoundExpression> arrayBuilder = null;
				if (!sideEffects.IsDefault)
				{
					int num = sideEffects.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						BoundExpression boundExpression = sideEffects[i];
						BoundExpression boundExpression2 = VisitExpression(boundExpression, ExprContext.Sideeffects);
						if (arrayBuilder == null && boundExpression2 != boundExpression)
						{
							arrayBuilder = ArrayBuilder<BoundExpression>.GetInstance();
							arrayBuilder.AddRange(sideEffects, i);
						}
						arrayBuilder?.Add(boundExpression2);
					}
				}
				BoundExpression valueOpt = VisitExpression(node.ValueOpt, context);
				return node.Update(node.Locals, arrayBuilder?.ToImmutableAndFree() ?? sideEffects, valueOpt, node.Type);
			}

			public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
			{
				return node.Update(VisitExpression(node.Expression, ExprContext.Sideeffects));
			}

			public override BoundNode VisitLocal(BoundLocal node)
			{
				if ((object)node.ConstantValueOpt == null)
				{
					switch (_context)
					{
					case ExprContext.Address:
						if (node.LocalSymbol.IsByRef)
						{
							RecordVarRead(node.LocalSymbol);
						}
						else
						{
							RecordVarRef(node.LocalSymbol);
						}
						break;
					case ExprContext.AssignmentTarget:
						_assignmentLocal = node;
						break;
					case ExprContext.Value:
					case ExprContext.Box:
						RecordVarRead(node.LocalSymbol);
						break;
					}
				}
				return base.VisitLocal(node);
			}

			public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
			{
				BoundLocal byRefLocal = (BoundLocal)VisitExpression(node.ByRefLocal, ExprContext.AssignmentTarget);
				BoundLocal assignmentLocal = _assignmentLocal;
				_assignmentLocal = null;
				BoundExpression lValue = VisitExpression(node.LValue, ExprContext.Address);
				RecordVarWrite(assignmentLocal.LocalSymbol);
				return node.Update(byRefLocal, lValue, node.IsLValue, node.Type);
			}

			public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
			{
				bool flag = IsIndirectAssignment(node);
				BoundExpression boundExpression = VisitExpression(node.Left, flag ? ExprContext.Address : ExprContext.AssignmentTarget);
				BoundLocal assignmentLocal = _assignmentLocal;
				_assignmentLocal = null;
				ExprContext context = ((_context != ExprContext.Address) ? ExprContext.Value : ExprContext.Address);
				BoundExpression right = node.Right;
				_ = boundExpression.Type;
				bool flag2 = false;
				if (right.Kind == BoundKind.ObjectCreationExpression)
				{
					MethodSymbol constructorOpt = ((BoundObjectCreationExpression)right).ConstructorOpt;
					if ((object)constructorOpt != null && constructorOpt.ParameterCount != 0)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					PushEvalStack(null, ExprContext.None);
				}
				right = VisitExpression(node.Right, context);
				if (flag2)
				{
					PopEvalStack();
				}
				if (assignmentLocal != null)
				{
					RecordVarWrite(assignmentLocal.LocalSymbol);
				}
				return node.Update(boundExpression, null, right, node.SuppressObjectClone, node.Type);
			}

			private static bool IsIndirectAssignment(BoundAssignmentOperator node)
			{
				return IsByRefVariable(node.Left);
			}

			private static bool IsByRefVariable(BoundExpression node)
			{
				switch (node.Kind)
				{
				case BoundKind.Parameter:
					return ((BoundParameter)node).ParameterSymbol.IsByRef;
				case BoundKind.Local:
					return ((BoundLocal)node).LocalSymbol.IsByRef;
				case BoundKind.Call:
					return ((BoundCall)node).Method.ReturnsByRef;
				case BoundKind.Sequence:
					return false;
				case BoundKind.PseudoVariable:
					return true;
				case BoundKind.ReferenceAssignment:
					return true;
				case BoundKind.ValueTypeMeReference:
					return true;
				case BoundKind.InstrumentationPayloadRoot:
				case BoundKind.ModuleVersionId:
					return false;
				case BoundKind.ArrayAccess:
				case BoundKind.FieldAccess:
					return false;
				default:
					throw ExceptionUtilities.UnexpectedValue(node.Kind);
				}
			}

			private static bool IsVerifierRef(TypeSymbol type)
			{
				if (type.TypeKind != TypeKind.TypeParameter)
				{
					return type.IsReferenceType;
				}
				return false;
			}

			private static bool IsVerifierVal(TypeSymbol type)
			{
				if (type.TypeKind != TypeKind.TypeParameter)
				{
					return type.IsValueType;
				}
				return false;
			}

			public override BoundNode VisitCall(BoundCall node)
			{
				BoundExpression boundExpression = node.ReceiverOpt;
				if (!node.Method.IsShared)
				{
					TypeSymbol type = boundExpression.Type;
					ExprContext context = ((!type.IsReferenceType) ? ExprContext.Address : ((!TypeSymbolExtensions.IsTypeParameter(type)) ? ExprContext.Value : ExprContext.Box));
					boundExpression = VisitExpression(boundExpression, context);
				}
				else
				{
					_counter++;
				}
				MethodSymbol method = node.Method;
				ImmutableArray<BoundExpression> arguments = VisitArguments(node.Arguments, method.Parameters);
				return node.Update(method, node.MethodGroupOpt, boundExpression, arguments, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, node.Type);
			}

			private ImmutableArray<BoundExpression> VisitArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<ParameterSymbol> parameters)
			{
				ArrayBuilder<BoundExpression> arrayBuilder = null;
				int num = arguments.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					ExprContext context = ((i == parameters.Length || !parameters[i].IsByRef) ? ExprContext.Value : ExprContext.Address);
					BoundExpression boundExpression = arguments[i];
					BoundExpression boundExpression2 = VisitExpression(boundExpression, context);
					if (arrayBuilder == null && boundExpression != boundExpression2)
					{
						arrayBuilder = ArrayBuilder<BoundExpression>.GetInstance();
						arrayBuilder.AddRange(arguments, i);
					}
					arrayBuilder?.Add(boundExpression2);
				}
				return arrayBuilder?.ToImmutableAndFree() ?? arguments;
			}

			public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
			{
				MethodSymbol constructorOpt = node.ConstructorOpt;
				ImmutableArray<BoundExpression> arguments = (((object)constructorOpt == null) ? node.Arguments : VisitArguments(node.Arguments, constructorOpt.Parameters));
				_counter++;
				return node.Update(constructorOpt, arguments, node.DefaultArguments, null, node.Type);
			}

			public override BoundNode VisitArrayAccess(BoundArrayAccess node)
			{
				ExprContext context = _context;
				_context = ExprContext.Value;
				BoundNode result = base.VisitArrayAccess(node);
				_context = context;
				return result;
			}

			public override BoundNode VisitFieldAccess(BoundFieldAccess node)
			{
				FieldSymbol fieldSymbol = node.FieldSymbol;
				BoundExpression receiverOpt = node.ReceiverOpt;
				if (!fieldSymbol.IsShared)
				{
					receiverOpt = (TypeSymbolExtensions.IsTypeParameter(receiverOpt.Type) ? VisitExpression(receiverOpt, ExprContext.Box) : ((!receiverOpt.Type.IsValueType || (_context != ExprContext.AssignmentTarget && _context != ExprContext.Address && !CodeGenerator.FieldLoadMustUseRef(receiverOpt))) ? VisitExpression(receiverOpt, ExprContext.Value) : VisitExpression(receiverOpt, ExprContext.Address)));
				}
				else
				{
					_counter++;
					receiverOpt = null;
				}
				return node.Update(receiverOpt, fieldSymbol, node.IsLValue, node.SuppressVirtualCalls, null, node.Type);
			}

			public override BoundNode VisitLabelStatement(BoundLabelStatement node)
			{
				RecordLabel(node.Label);
				return base.VisitLabelStatement(node);
			}

			public override BoundNode VisitGotoStatement(BoundGotoStatement node)
			{
				BoundNode result = base.VisitGotoStatement(node);
				RecordBranch(node.Label);
				return result;
			}

			public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
			{
				BoundNode result = base.VisitConditionalGoto(node);
				PopEvalStack();
				RecordBranch(node.Label);
				return result;
			}

			public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
			{
				int stackDepth = StackDepth();
				BoundExpression testExpression = (BoundExpression)Visit(node.TestExpression);
				object objectValue = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
				SetStackDepth(stackDepth);
				BoundExpression elseExpression = (BoundExpression)Visit(node.ElseExpression);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				return node.Update(testExpression, null, null, elseExpression, node.ConstantValueOpt, node.Type);
			}

			public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
			{
				int stackDepth = StackDepth();
				BoundExpression condition = (BoundExpression)Visit(node.Condition);
				object objectValue = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
				SetStackDepth(stackDepth);
				BoundExpression whenTrue = (BoundExpression)Visit(node.WhenTrue);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				SetStackDepth(stackDepth);
				BoundExpression whenFalse = (BoundExpression)Visit(node.WhenFalse);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				return node.Update(condition, whenTrue, whenFalse, node.ConstantValueOpt, node.Type);
			}

			public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
			{
				if (!TypeSymbolExtensions.IsBooleanType(node.ReceiverOrCondition.Type))
				{
					EnsureOnlyEvalStack();
				}
				int stackDepth = StackDepth();
				BoundExpression receiverOrCondition = (BoundExpression)Visit(node.ReceiverOrCondition);
				object objectValue = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
				SetStackDepth(stackDepth);
				BoundExpression whenNotNull = (BoundExpression)Visit(node.WhenNotNull);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				BoundExpression whenNullOpt = null;
				if (node.WhenNullOpt != null)
				{
					SetStackDepth(stackDepth);
					whenNullOpt = (BoundExpression)Visit(node.WhenNullOpt);
					EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				}
				return node.Update(receiverOrCondition, node.CaptureReceiver, node.PlaceholderId, whenNotNull, whenNullOpt, node.Type);
			}

			public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
			{
				return base.VisitConditionalAccessReceiverPlaceholder(node);
			}

			public override BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
			{
				EnsureOnlyEvalStack();
				int stackDepth = StackDepth();
				PushEvalStack(null, ExprContext.None);
				object objectValue = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
				SetStackDepth(stackDepth);
				BoundExpression valueTypeReceiver = (BoundExpression)Visit(node.ValueTypeReceiver);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				SetStackDepth(stackDepth);
				BoundExpression referenceTypeReceiver = (BoundExpression)Visit(node.ReferenceTypeReceiver);
				EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
				return node.Update(valueTypeReceiver, referenceTypeReceiver, node.Type);
			}

			public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
			{
				BoundExpression left = node.Left;
				if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
				{
					return VisitBinaryOperatorSimple(node);
				}
				ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
				instance.Push(node);
				BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)left;
				while (true)
				{
					instance.Push(boundBinaryOperator);
					left = boundBinaryOperator.Left;
					if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
					{
						break;
					}
					boundBinaryOperator = (BoundBinaryOperator)left;
				}
				int stackDepth = StackDepth();
				BoundExpression boundExpression = (BoundExpression)Visit(left);
				while (true)
				{
					boundBinaryOperator = instance.Pop();
					object obj = null;
					BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
					bool flag;
					if (binaryOperatorKind == BinaryOperatorKind.OrElse || binaryOperatorKind == BinaryOperatorKind.AndAlso)
					{
						flag = true;
						obj = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
						SetStackDepth(stackDepth);
					}
					else
					{
						flag = false;
					}
					BoundExpression right = (BoundExpression)Visit(boundBinaryOperator.Right);
					if (flag)
					{
						EnsureStackState(RuntimeHelpers.GetObjectValue(obj));
					}
					TypeSymbol type = VisitType(boundBinaryOperator.Type);
					boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, type);
					if (instance.Count == 0)
					{
						break;
					}
					_counter++;
					SetStackDepth(stackDepth);
					PushEvalStack(node, ExprContext.Value);
				}
				instance.Free();
				return boundExpression;
			}

			private BoundNode VisitBinaryOperatorSimple(BoundBinaryOperator node)
			{
				BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
				if (binaryOperatorKind == BinaryOperatorKind.OrElse || binaryOperatorKind == BinaryOperatorKind.AndAlso)
				{
					int stackDepth = StackDepth();
					BoundExpression left = (BoundExpression)Visit(node.Left);
					object objectValue = RuntimeHelpers.GetObjectValue(GetStackStateCookie());
					SetStackDepth(stackDepth);
					BoundExpression right = (BoundExpression)Visit(node.Right);
					EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue));
					return node.Update(node.OperatorKind, left, right, node.Checked, node.ConstantValueOpt, node.Type);
				}
				return base.VisitBinaryOperator(node);
			}

			public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
			{
				if (node.Checked && (node.OperatorKind & UnaryOperatorKind.OpMask) == UnaryOperatorKind.Minus)
				{
					int stackDepth = StackDepth();
					PushEvalStack(null, ExprContext.None);
					BoundExpression operand = (BoundExpression)Visit(node.Operand);
					SetStackDepth(stackDepth);
					return node.Update(node.OperatorKind, operand, node.Checked, node.ConstantValueOpt, node.Type);
				}
				return base.VisitUnaryOperator(node);
			}

			public override BoundNode VisitSelectStatement(BoundSelectStatement node)
			{
				EnsureOnlyEvalStack();
				BoundExpressionStatement boundExpressionStatement = (BoundExpressionStatement)Visit(node.ExpressionStatement);
				if (boundExpressionStatement.Expression.Kind == BoundKind.Local)
				{
					LocalSymbol localSymbol = ((BoundLocal)boundExpressionStatement.Expression).LocalSymbol;
					ShouldNotSchedule(localSymbol);
				}
				int stackDepth = StackDepth();
				EnsureOnlyEvalStack();
				BoundRValuePlaceholder exprPlaceholderOpt = (BoundRValuePlaceholder)Visit(node.ExprPlaceholderOpt);
				SetStackDepth(stackDepth);
				EnsureOnlyEvalStack();
				ImmutableArray<BoundCaseBlock> caseBlocks = VisitList(node.CaseBlocks);
				LabelSymbol exitLabel = node.ExitLabel;
				if ((object)exitLabel != null)
				{
					RecordLabel(exitLabel);
				}
				EnsureOnlyEvalStack();
				return node.Update(boundExpressionStatement, exprPlaceholderOpt, caseBlocks, node.RecommendSwitchTable, exitLabel);
			}

			public override BoundNode VisitCaseBlock(BoundCaseBlock node)
			{
				EnsureOnlyEvalStack();
				BoundCaseStatement caseStatement = (BoundCaseStatement)Visit(node.CaseStatement);
				EnsureOnlyEvalStack();
				BoundBlock body = (BoundBlock)Visit(node.Body);
				EnsureOnlyEvalStack();
				return node.Update(caseStatement, body);
			}

			public override BoundNode VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node)
			{
				BoundExpression value = (BoundExpression)Visit(node.Value);
				PopEvalStack();
				return node.Update(value, VisitList(node.Jumps));
			}

			public override BoundNode VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node)
			{
				BoundLabelStatement resumeLabel = (BoundLabelStatement)Visit(node.ResumeLabel);
				BoundLabelStatement resumeNextLabel = (BoundLabelStatement)Visit(node.ResumeNextLabel);
				BoundLocal resumeTargetTemporary = (BoundLocal)Visit(node.ResumeTargetTemporary);
				PopEvalStack();
				return node.Update(resumeTargetTemporary, resumeLabel, resumeNextLabel, node.Jumps);
			}

			public override BoundNode VisitTryStatement(BoundTryStatement node)
			{
				EnsureOnlyEvalStack();
				BoundBlock tryBlock = (BoundBlock)Visit(node.TryBlock);
				EnsureOnlyEvalStack();
				ImmutableArray<BoundCatchBlock> catchBlocks = VisitList(node.CatchBlocks);
				EnsureOnlyEvalStack();
				BoundBlock finallyBlockOpt = (BoundBlock)Visit(node.FinallyBlockOpt);
				EnsureOnlyEvalStack();
				if ((object)node.ExitLabelOpt != null)
				{
					RecordLabel(node.ExitLabelOpt);
				}
				EnsureOnlyEvalStack();
				return node.Update(tryBlock, catchBlocks, finallyBlockOpt, node.ExitLabelOpt);
			}

			public override BoundNode VisitCatchBlock(BoundCatchBlock node)
			{
				int num = StackDepth();
				DeclareLocal(node.LocalOpt, num);
				EnsureOnlyEvalStack();
				BoundExpression exceptionSourceOpt = VisitExpression(node.ExceptionSourceOpt, ExprContext.Value);
				SetStackDepth(num);
				EnsureOnlyEvalStack();
				BoundExpression errorLineNumberOpt = VisitExpression(node.ErrorLineNumberOpt, ExprContext.Value);
				SetStackDepth(num);
				EnsureOnlyEvalStack();
				BoundExpression exceptionFilterOpt = VisitExpression(node.ExceptionFilterOpt, ExprContext.Value);
				SetStackDepth(num);
				EnsureOnlyEvalStack();
				BoundBlock body = (BoundBlock)Visit(node.Body);
				EnsureOnlyEvalStack();
				return node.Update(node.LocalOpt, exceptionSourceOpt, errorLineNumberOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
			}

			public override BoundNode VisitArrayInitialization(BoundArrayInitialization node)
			{
				EnsureOnlyEvalStack();
				ImmutableArray<BoundExpression> initializers = node.Initializers;
				ArrayBuilder<BoundExpression> arrayBuilder = null;
				if (!initializers.IsDefault)
				{
					int num = initializers.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						EnsureOnlyEvalStack();
						BoundExpression boundExpression = initializers[i];
						BoundExpression boundExpression2 = VisitExpression(boundExpression, ExprContext.Value);
						if (arrayBuilder == null && boundExpression2 != boundExpression)
						{
							arrayBuilder = ArrayBuilder<BoundExpression>.GetInstance();
							arrayBuilder.AddRange(initializers, i);
						}
						arrayBuilder?.Add(boundExpression2);
					}
				}
				return node.Update(arrayBuilder?.ToImmutableAndFree() ?? initializers, node.Type);
			}

			public override BoundNode VisitReturnStatement(BoundReturnStatement node)
			{
				BoundExpression expressionOpt = Visit(node.ExpressionOpt) as BoundExpression;
				EnsureOnlyEvalStack();
				return node.Update(expressionOpt, node.FunctionLocalOpt, node.ExitLabelOpt);
			}

			private void EnsureOnlyEvalStack()
			{
				RecordVarRead(_empty);
			}

			private object GetStackStateCookie()
			{
				DummyLocal dummyLocal = new DummyLocal(_container);
				_dummyVariables.Add(dummyLocal, dummyLocal);
				_locals.Add(dummyLocal, new LocalDefUseInfo(StackDepth()));
				RecordDummyWrite(dummyLocal);
				return dummyLocal;
			}

			private void EnsureStackState(object cookie)
			{
				RecordVarRead(_dummyVariables[RuntimeHelpers.GetObjectValue(cookie)]);
			}

			private void RecordBranch(LabelSymbol label)
			{
				DummyLocal value = null;
				if (_dummyVariables.TryGetValue(label, out value))
				{
					RecordVarRead(value);
					return;
				}
				value = new DummyLocal(_container);
				_dummyVariables.Add(label, value);
				_locals.Add(value, new LocalDefUseInfo(StackDepth()));
				RecordDummyWrite(value);
			}

			private void RecordLabel(LabelSymbol label)
			{
				DummyLocal value = null;
				if (_dummyVariables.TryGetValue(label, out value))
				{
					RecordVarRead(value);
					return;
				}
				value = _empty;
				_dummyVariables.Add(label, value);
				RecordVarRead(value);
			}

			private void RecordVarRef(LocalSymbol local)
			{
				if (CanScheduleToStack(local))
				{
					ShouldNotSchedule(local);
				}
			}

			private void RecordVarRead(LocalSymbol local)
			{
				if (!CanScheduleToStack(local))
				{
					return;
				}
				LocalDefUseInfo localDefUseInfo = _locals[local];
				if (!localDefUseInfo.CannotSchedule)
				{
					if (localDefUseInfo.localDefs.Count == 0)
					{
						localDefUseInfo.ShouldNotSchedule();
						return;
					}
					if (!(local is DummyLocal) && localDefUseInfo.StackAtDeclaration != StackDepth() && !EvalStackHasLocal(local))
					{
						localDefUseInfo.ShouldNotSchedule();
						return;
					}
					localDefUseInfo.localDefs.Last().SetEnd(_counter);
					LocalDefUseSpan item = new LocalDefUseSpan(_counter);
					localDefUseInfo.localDefs.Add(item);
				}
			}

			private bool EvalStackHasLocal(LocalSymbol local)
			{
				(BoundExpression, ExprContext) tuple = _evalStack.Last();
				if (tuple.Item2 == (ExprContext)((!local.IsByRef) ? 2 : 3) && tuple.Item1.Kind == BoundKind.Local)
				{
					return ((BoundLocal)tuple.Item1).LocalSymbol == local;
				}
				return false;
			}

			private void RecordVarWrite(LocalSymbol local)
			{
				if (!CanScheduleToStack(local))
				{
					return;
				}
				LocalDefUseInfo localDefUseInfo = _locals[local];
				if (!localDefUseInfo.CannotSchedule)
				{
					int num = StackDepth() - 1;
					if (localDefUseInfo.StackAtDeclaration != num)
					{
						localDefUseInfo.ShouldNotSchedule();
						return;
					}
					LocalDefUseSpan item = new LocalDefUseSpan(_counter);
					localDefUseInfo.localDefs.Add(item);
				}
			}

			private void RecordDummyWrite(LocalSymbol local)
			{
				LocalDefUseInfo localDefUseInfo = _locals[local];
				LocalDefUseSpan item = new LocalDefUseSpan(_counter);
				localDefUseInfo.localDefs.Add(item);
			}

			private void ShouldNotSchedule(LocalSymbol local)
			{
				LocalDefUseInfo value = null;
				if (_locals.TryGetValue(local, out value))
				{
					value.ShouldNotSchedule();
				}
			}

			private bool CanScheduleToStack(LocalSymbol local)
			{
				if (local.CanScheduleToStack)
				{
					if (_debugFriendly)
					{
						return !local.SynthesizedKind.IsLongLived();
					}
					return true;
				}
				return false;
			}

			private void DeclareLocals(ImmutableArray<LocalSymbol> locals, int stack)
			{
				ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					DeclareLocal(current, stack);
				}
			}

			private void DeclareLocal(LocalSymbol local, int stack)
			{
				if ((object)local != null && CanScheduleToStack(local))
				{
					_locals.Add(local, new LocalDefUseInfo(stack));
				}
			}
		}

		private class DummyLocal : SynthesizedLocal
		{
			public DummyLocal(Symbol container)
				: base(container, null, SynthesizedLocalKind.OptimizerTemp)
			{
			}

			internal override TypeSymbol ComputeType(Binder containingBinder = null)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private class LocalDefUseInfo
		{
			public readonly int StackAtDeclaration;

			public readonly List<LocalDefUseSpan> localDefs;

			private bool _cannotSchedule;

			public bool CannotSchedule => _cannotSchedule;

			public void ShouldNotSchedule()
			{
				_cannotSchedule = true;
			}

			public LocalDefUseInfo(int stackAtDeclaration)
			{
				localDefs = new List<LocalDefUseSpan>(8);
				_cannotSchedule = false;
				StackAtDeclaration = stackAtDeclaration;
			}
		}

		private class LocalDefUseSpan
		{
			public readonly int Start;

			private int _end;

			public int End => _end;

			public LocalDefUseSpan(int assigned)
			{
				Start = assigned;
				_end = assigned;
			}

			public void SetEnd(int newEnd)
			{
				_end = newEnd;
			}

			public override string ToString()
			{
				return "[" + Start + ", " + End + ")";
			}

			public bool ConflictsWith(LocalDefUseSpan other)
			{
				return Contains(other.Start) ^ Contains(other.End);
			}

			private bool Contains(int val)
			{
				if (Start < val)
				{
					return End > val;
				}
				return false;
			}

			public bool ConflictsWithDummy(LocalDefUseSpan dummy)
			{
				return Includes(dummy.Start) ^ Includes(dummy.End);
			}

			private bool Includes(int val)
			{
				if (Start <= val)
				{
					return End >= val;
				}
				return false;
			}
		}

		private sealed class Rewriter : BoundTreeRewriterWithStackGuard
		{
			private int _nodeCounter;

			private readonly Dictionary<LocalSymbol, LocalDefUseInfo> _info;

			private Rewriter(Dictionary<LocalSymbol, LocalDefUseInfo> info)
			{
				_nodeCounter = 0;
				_info = null;
				_info = info;
			}

			public static BoundStatement Rewrite(BoundStatement src, Dictionary<LocalSymbol, LocalDefUseInfo> info)
			{
				return (BoundStatement)new Rewriter(info).Visit(src);
			}

			public override BoundNode Visit(BoundNode node)
			{
				BoundNode boundNode = null;
				boundNode = ((!(node is BoundExpression boundExpression) || (object)boundExpression.ConstantValueOpt == null) ? base.Visit(node) : node);
				_nodeCounter++;
				return boundNode;
			}

			public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
			{
				BoundExpression left = node.Left;
				if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
				{
					return VisitBinaryOperatorSimple(node);
				}
				ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
				instance.Push(node);
				BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)left;
				while (true)
				{
					instance.Push(boundBinaryOperator);
					left = boundBinaryOperator.Left;
					if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
					{
						break;
					}
					boundBinaryOperator = (BoundBinaryOperator)left;
				}
				BoundExpression boundExpression = (BoundExpression)Visit(left);
				while (true)
				{
					boundBinaryOperator = instance.Pop();
					BoundExpression right = (BoundExpression)Visit(boundBinaryOperator.Right);
					TypeSymbol type = VisitType(boundBinaryOperator.Type);
					boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, type);
					if (instance.Count == 0)
					{
						break;
					}
					_nodeCounter++;
				}
				instance.Free();
				return boundExpression;
			}

			private BoundNode VisitBinaryOperatorSimple(BoundBinaryOperator node)
			{
				return base.VisitBinaryOperator(node);
			}

			private static bool IsLastAccess(LocalDefUseInfo locInfo, int counter)
			{
				return locInfo.localDefs.Any((LocalDefUseSpan d) => counter == d.Start && counter == d.End);
			}

			public override BoundNode VisitLocal(BoundLocal node)
			{
				LocalDefUseInfo value = null;
				if (!_info.TryGetValue(node.LocalSymbol, out value))
				{
					return base.VisitLocal(node);
				}
				if (!IsLastAccess(value, _nodeCounter))
				{
					return new BoundDup(node.Syntax, node.LocalSymbol.IsByRef, node.Type);
				}
				return base.VisitLocal(node);
			}

			public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
			{
				LocalDefUseInfo value = null;
				BoundLocal byRefLocal = node.ByRefLocal;
				if (!_info.TryGetValue(byRefLocal.LocalSymbol, out value))
				{
					return base.VisitReferenceAssignment(node);
				}
				_nodeCounter++;
				BoundExpression boundExpression = (BoundExpression)Visit(node.LValue);
				if (IsLastAccess(value, _nodeCounter))
				{
					return boundExpression;
				}
				return node.Update(byRefLocal, boundExpression, node.IsLValue, node.Type);
			}

			private BoundNode VisitAssignmentOperatorDefault(BoundAssignmentOperator node)
			{
				BoundExpression left = (BoundExpression)Visit(node.Left);
				BoundExpression right = (BoundExpression)Visit(node.Right);
				return node.Update(left, null, right, node.SuppressObjectClone, node.Type);
			}

			public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
			{
				LocalDefUseInfo value = null;
				if (!(node.Left is BoundLocal boundLocal) || !_info.TryGetValue(boundLocal.LocalSymbol, out value))
				{
					return VisitAssignmentOperatorDefault(node);
				}
				if (boundLocal.LocalSymbol.IsByRef)
				{
					return VisitAssignmentOperatorDefault(node);
				}
				_nodeCounter++;
				BoundExpression boundExpression = (BoundExpression)Visit(node.Right);
				if (IsLastAccess(value, _nodeCounter))
				{
					return boundExpression;
				}
				return node.Update(boundLocal, node.LeftOnTheRightOpt, boundExpression, node.SuppressObjectClone, node.Type);
			}

			public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
			{
				BoundExpression receiverOrCondition = (BoundExpression)Visit(node.ReceiverOrCondition);
				BoundExpression whenNotNull = (BoundExpression)Visit(node.WhenNotNull);
				BoundExpression boundExpression = node.WhenNullOpt;
				if (boundExpression != null)
				{
					boundExpression = (BoundExpression)Visit(boundExpression);
				}
				return node.Update(receiverOrCondition, node.CaptureReceiver, node.PlaceholderId, whenNotNull, boundExpression, node.Type);
			}
		}

		public static BoundStatement OptimizeLocalsOut(Symbol container, BoundStatement src, bool debugFriendly, out HashSet<LocalSymbol> stackLocals)
		{
			Dictionary<LocalSymbol, LocalDefUseInfo> locals = null;
			src = (BoundStatement)Analyzer.Analyze(container, src, debugFriendly, out locals);
			locals = FilterValidStackLocals(locals);
			if (locals.Count == 0)
			{
				stackLocals = new HashSet<LocalSymbol>();
				return src;
			}
			stackLocals = new HashSet<LocalSymbol>(locals.Keys);
			return Rewriter.Rewrite(src, locals);
		}

		private static Dictionary<LocalSymbol, LocalDefUseInfo> FilterValidStackLocals(Dictionary<LocalSymbol, LocalDefUseInfo> info)
		{
			List<LocalDefUseInfo> list = new List<LocalDefUseInfo>();
			LocalSymbol[] array = info.Keys.ToArray();
			foreach (LocalSymbol localSymbol in array)
			{
				LocalDefUseInfo localDefUseInfo = info[localSymbol];
				if (localSymbol is DummyLocal)
				{
					list.Add(localDefUseInfo);
					info.Remove(localSymbol);
				}
				else if (localDefUseInfo.CannotSchedule)
				{
					info.Remove(localSymbol);
				}
			}
			if (info.Count == 0)
			{
				return info;
			}
			List<LocalDefUseSpan> list2 = new List<LocalDefUseSpan>();
			foreach (LocalDefUseInfo item in list)
			{
				foreach (LocalDefUseSpan localDef in item.localDefs)
				{
					if (localDef.Start < localDef.End)
					{
						list2.Add(localDef);
					}
				}
			}
			int count = list2.Count;
			foreach (KeyValuePair<LocalSymbol, LocalDefUseInfo> item2 in from i in info
				where i.Value.localDefs.Count > 0
				orderby i.Value.localDefs.Count descending, i.Value.localDefs[0].Start descending
				select (i))
			{
				if (!info.ContainsKey(item2.Key))
				{
					continue;
				}
				bool flag = false;
				ArrayBuilder<LocalDefUseSpan> instance = ArrayBuilder<LocalDefUseSpan>.GetInstance();
				foreach (LocalDefUseSpan localDef2 in item2.Value.localDefs)
				{
					int num = count - 1;
					for (int k = 0; k <= num; k++)
					{
						if (localDef2.ConflictsWithDummy(list2[k]))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						int num2 = list2.Count - 1;
						for (int l = count; l <= num2; l++)
						{
							if (localDef2.ConflictsWith(list2[l]))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						info.Remove(item2.Key);
						break;
					}
					instance.Add(localDef2);
				}
				if (!flag)
				{
					list2.AddRange(instance);
				}
				instance.Free();
			}
			return info;
		}
	}
}
