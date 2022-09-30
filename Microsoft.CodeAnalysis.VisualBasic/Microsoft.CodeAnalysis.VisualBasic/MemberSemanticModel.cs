using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class MemberSemanticModel : VBSemanticModel
	{
		private class CompilerGeneratedNodeFinder : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private readonly VisualBasicSyntaxNode _targetSyntax;

			private readonly BoundKind _targetBoundKind;

			private BoundNode _found;

			private CompilerGeneratedNodeFinder(VisualBasicSyntaxNode targetSyntax, BoundKind targetBoundKind)
			{
				_targetSyntax = targetSyntax;
				_targetBoundKind = targetBoundKind;
			}

			public static BoundNode FindIn(BoundNode context, VisualBasicSyntaxNode targetSyntax, BoundKind targetBoundKind)
			{
				CompilerGeneratedNodeFinder compilerGeneratedNodeFinder = new CompilerGeneratedNodeFinder(targetSyntax, targetBoundKind);
				compilerGeneratedNodeFinder.Visit(context);
				return compilerGeneratedNodeFinder._found;
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (node == null || _found != null)
				{
					return null;
				}
				if (node.WasCompilerGenerated && node.Syntax == _targetSyntax && node.Kind == _targetBoundKind)
				{
					_found = node;
					return null;
				}
				return base.Visit(node);
			}

			protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
			{
				return false;
			}
		}

		internal class IncrementalBinder : Binder
		{
			private readonly MemberSemanticModel _binding;

			internal IncrementalBinder(MemberSemanticModel binding, Binder next)
				: base(next)
			{
				_binding = binding;
			}

			public override Binder GetBinder(SyntaxNode node)
			{
				Binder binder = base.ContainingBinder.GetBinder(node);
				if (binder != null)
				{
					return new IncrementalBinder(_binding, binder);
				}
				return null;
			}

			public override Binder GetBinder(SyntaxList<StatementSyntax> list)
			{
				Binder binder = base.ContainingBinder.GetBinder(list);
				if (binder != null)
				{
					return new IncrementalBinder(_binding, binder);
				}
				return null;
			}

			public override BoundStatement BindStatement(StatementSyntax node, BindingDiagnosticBag diagnostics)
			{
				ImmutableArray<BoundNode> immutableArray = _binding.GuardedGetBoundNodesFromMap(node);
				if (immutableArray.IsDefault)
				{
					return base.BindStatement(node, diagnostics);
				}
				return (BoundStatement)immutableArray.First();
			}
		}

		private class SemanticModelMapsBuilder : BoundTreeWalkerWithStackGuard
		{
			private readonly MemberSemanticModel _semanticModel;

			private readonly SyntaxNode _thisSyntaxNodeOnly;

			private Dictionary<BoundValuePlaceholderBase, BoundExpression> _placeholderReplacementMap;

			private readonly OrderPreservingMultiDictionary<SyntaxNode, BoundNode> _nodeCache;

			private SemanticModelMapsBuilder(MemberSemanticModel semanticModel, SyntaxNode thisSyntaxNodeOnly, OrderPreservingMultiDictionary<SyntaxNode, BoundNode> nodeCache)
			{
				_semanticModel = semanticModel;
				_thisSyntaxNodeOnly = thisSyntaxNodeOnly;
				_nodeCache = nodeCache;
			}

			public static void GuardedCacheBoundNodes(BoundNode root, MemberSemanticModel semanticModel, SmallDictionary<SyntaxNode, ImmutableArray<BoundNode>> nodeCache, SyntaxNode thisSyntaxNodeOnly = null)
			{
				OrderPreservingMultiDictionary<SyntaxNode, BoundNode> instance = OrderPreservingMultiDictionary<SyntaxNode, BoundNode>.GetInstance();
				new SemanticModelMapsBuilder(semanticModel, thisSyntaxNodeOnly, instance).Visit(root);
				foreach (SyntaxNode key in instance.Keys)
				{
					if (!nodeCache.ContainsKey(key))
					{
						nodeCache[key] = instance[key];
					}
				}
				instance.Free();
			}

			public bool RecordNode(BoundNode node, bool allowCompilerGenerated = false)
			{
				if (!allowCompilerGenerated && node.WasCompilerGenerated)
				{
					return false;
				}
				switch (node.Kind)
				{
				case BoundKind.UnboundLambda:
					return false;
				case BoundKind.Conversion:
				{
					if (allowCompilerGenerated)
					{
						break;
					}
					BoundConversion boundConversion = (BoundConversion)node;
					if (!boundConversion.ExplicitCastInCode && boundConversion.Operand.WasCompilerGenerated)
					{
						BoundKind kind = boundConversion.Operand.Kind;
						if (kind - 3 <= BoundKind.WithLValueExpressionPlaceholder)
						{
							return false;
						}
					}
					break;
				}
				}
				if (_thisSyntaxNodeOnly != null && node.Syntax != _thisSyntaxNodeOnly)
				{
					return false;
				}
				return true;
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (node == null)
				{
					return null;
				}
				if (RecordNode(node))
				{
					_nodeCache.Add(node.Syntax, node);
				}
				return base.Visit(node);
			}

			protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
			{
				return false;
			}

			public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
			{
				if (node.Left.Kind != BoundKind.BinaryOperator)
				{
					return base.VisitBinaryOperator(node);
				}
				ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
				instance.Push(node.Right);
				BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)node.Left;
				if (RecordNode(boundBinaryOperator))
				{
					_nodeCache.Add(boundBinaryOperator.Syntax, boundBinaryOperator);
				}
				instance.Push(boundBinaryOperator.Right);
				BoundExpression left = boundBinaryOperator.Left;
				while (left.Kind == BoundKind.BinaryOperator)
				{
					boundBinaryOperator = (BoundBinaryOperator)left;
					if (RecordNode(boundBinaryOperator))
					{
						_nodeCache.Add(boundBinaryOperator.Syntax, boundBinaryOperator);
					}
					instance.Push(boundBinaryOperator.Right);
					left = boundBinaryOperator.Left;
				}
				Visit(left);
				while (instance.Count > 0)
				{
					Visit(instance.Pop());
				}
				instance.Free();
				return null;
			}

			public override BoundNode VisitUnboundLambda(UnboundLambda node)
			{
				return Visit(node.BindForErrorRecovery());
			}

			public override BoundNode VisitCall(BoundCall node)
			{
				BoundExpression receiverOpt = node.ReceiverOpt;
				Visit(receiverOpt);
				BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
				if (methodGroupOpt != null)
				{
					if (methodGroupOpt.Syntax != node.Syntax)
					{
						Visit(methodGroupOpt);
					}
					else if (node.Method.IsShared)
					{
						Visit(methodGroupOpt.ReceiverOpt);
					}
				}
				VisitList(node.Arguments);
				return null;
			}

			public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
			{
				BoundExpression receiverOpt = node.ReceiverOpt;
				Visit(receiverOpt);
				BoundPropertyGroup propertyGroupOpt = node.PropertyGroupOpt;
				if (propertyGroupOpt != null)
				{
					if (propertyGroupOpt.Syntax != node.Syntax)
					{
						Visit(node.PropertyGroupOpt);
					}
					else if (node.PropertySymbol.IsShared)
					{
						Visit(propertyGroupOpt.ReceiverOpt);
					}
				}
				VisitList(node.Arguments);
				return null;
			}

			public override BoundNode VisitTypeExpression(BoundTypeExpression node)
			{
				Visit(node.UnevaluatedReceiverOpt);
				return base.VisitTypeExpression(node);
			}

			public override BoundNode VisitAttribute(BoundAttribute node)
			{
				VisitList(node.ConstructorArguments);
				ImmutableArray<BoundExpression>.Enumerator enumerator = node.NamedArguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					Visit(current);
				}
				return null;
			}

			public override BoundNode VisitQueryClause(BoundQueryClause node)
			{
				if (RecordNode(node))
				{
					_semanticModel._guardedQueryBindersMap[node.Syntax] = node.Binders;
				}
				return base.VisitQueryClause(node);
			}

			public override BoundNode VisitAggregateClause(BoundAggregateClause node)
			{
				if (RecordNode(node))
				{
					_semanticModel._guardedQueryBindersMap[node.Syntax] = node.Binders;
				}
				return base.VisitAggregateClause(node);
			}

			public override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
			{
				if (RecordNode(node, allowCompilerGenerated: true) && node.Syntax is FieldInitializerSyntax key)
				{
					_semanticModel._guardedAnonymousTypeBinderMap[key] = node.Binder;
				}
				return base.VisitAnonymousTypeFieldInitializer(node);
			}

			public override BoundNode VisitConversion(BoundConversion node)
			{
				return Visit(node.Operand);
			}

			public override BoundNode VisitDirectCast(BoundDirectCast node)
			{
				return Visit(node.Operand);
			}

			public override BoundNode VisitTryCast(BoundTryCast node)
			{
				return Visit(node.Operand);
			}

			public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
			{
				BoundExpression receiverOpt = node.ReceiverOpt;
				Visit(receiverOpt);
				BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
				if (methodGroupOpt != null)
				{
					if (methodGroupOpt.Syntax != node.Syntax)
					{
						Visit(methodGroupOpt);
					}
					else if (node.Method.IsShared)
					{
						Visit(methodGroupOpt.ReceiverOpt);
					}
				}
				return null;
			}

			public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
			{
				if (node.LeftOnTheRightOpt == null)
				{
					return base.VisitAssignmentOperator(node);
				}
				if (_placeholderReplacementMap == null)
				{
					_placeholderReplacementMap = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
				}
				_placeholderReplacementMap.Add(node.LeftOnTheRightOpt, node.Left);
				Visit(node.Right);
				_placeholderReplacementMap.Remove(node.LeftOnTheRightOpt);
				return null;
			}

			public override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
			{
				BoundExpression value = null;
				if (_placeholderReplacementMap != null && _placeholderReplacementMap.TryGetValue(node, out value))
				{
					return Visit(value);
				}
				return base.VisitCompoundAssignmentTargetPlaceholder(node);
			}

			public override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
			{
				BoundExpression value = null;
				if (_placeholderReplacementMap != null && _placeholderReplacementMap.TryGetValue(node, out value))
				{
					return Visit(value);
				}
				return base.VisitByRefArgumentPlaceholder(node);
			}

			public override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
			{
				if (_placeholderReplacementMap == null)
				{
					_placeholderReplacementMap = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
				}
				_placeholderReplacementMap.Add(node.InPlaceholder, node.OriginalArgument);
				Visit(node.InConversion);
				_placeholderReplacementMap.Remove(node.InPlaceholder);
				return null;
			}

			private BoundNode VisitObjectInitializerExpressionBase(BoundObjectInitializerExpressionBase node)
			{
				VisitList(node.Initializers);
				return null;
			}

			public override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
			{
				return VisitObjectInitializerExpressionBase(node);
			}

			public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
			{
				return VisitObjectInitializerExpressionBase(node);
			}

			public override BoundNode VisitLateInvocation(BoundLateInvocation node)
			{
				base.VisitLateInvocation(node);
				if (node.Member is BoundLateMemberAccess boundLateMemberAccess && boundLateMemberAccess.ReceiverOpt == null && node.MethodOrPropertyGroupOpt != null)
				{
					Visit(node.MethodOrPropertyGroupOpt.ReceiverOpt);
				}
				return null;
			}
		}

		private readonly SyntaxNode _root;

		private readonly Binder _rootBinder;

		private readonly SyntaxTreeSemanticModel _containingSemanticModelOpt;

		private readonly SyntaxTreeSemanticModel _parentSemanticModelOpt;

		private readonly int _speculatedPosition;

		private readonly bool _ignoresAccessibility;

		private readonly Lazy<VisualBasicOperationFactory> _operationFactory;

		private readonly ReaderWriterLockSlim _rwLock;

		private readonly SmallDictionary<SyntaxNode, ImmutableArray<BoundNode>> _guardedBoundNodeMap;

		private readonly Dictionary<SyntaxNode, IOperation> _guardedIOperationNodeMap;

		private readonly Dictionary<SyntaxNode, ImmutableArray<Binder>> _guardedQueryBindersMap;

		private readonly Dictionary<FieldInitializerSyntax, Binder.AnonymousTypeFieldInitializerBinder> _guardedAnonymousTypeBinderMap;

		internal Binder RootBinder => _rootBinder;

		internal sealed override SyntaxNode Root => _root;

		public sealed override bool IsSpeculativeSemanticModel => _parentSemanticModelOpt != null;

		public sealed override int OriginalPositionForSpeculation => _speculatedPosition;

		public sealed override SemanticModel ParentModel => _parentSemanticModelOpt;

		internal sealed override SemanticModel ContainingModelOrSelf => (SemanticModel)(((object)_containingSemanticModelOpt) ?? ((object)this));

		public sealed override bool IgnoresAccessibility => _ignoresAccessibility;

		public override VisualBasicCompilation Compilation => RootBinder.Compilation;

		internal Symbol MemberSymbol => RootBinder.ContainingMember;

		public override SyntaxTree SyntaxTree => Root.SyntaxTree;

		internal MemberSemanticModel(SyntaxNode root, Binder rootBinder, SyntaxTreeSemanticModel containingSemanticModelOpt, SyntaxTreeSemanticModel parentSemanticModelOpt, int speculatedPosition, bool ignoreAccessibility = false)
		{
			_rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			_guardedBoundNodeMap = new SmallDictionary<SyntaxNode, ImmutableArray<BoundNode>>(ReferenceEqualityComparer.Instance);
			_guardedIOperationNodeMap = new Dictionary<SyntaxNode, IOperation>();
			_guardedQueryBindersMap = new Dictionary<SyntaxNode, ImmutableArray<Binder>>();
			_guardedAnonymousTypeBinderMap = new Dictionary<FieldInitializerSyntax, Binder.AnonymousTypeFieldInitializerBinder>();
			_root = root;
			_ignoresAccessibility = ignoreAccessibility;
			_rootBinder = SemanticModelBinder.Mark(rootBinder, ignoreAccessibility);
			_containingSemanticModelOpt = containingSemanticModelOpt;
			_parentSemanticModelOpt = parentSemanticModelOpt;
			_speculatedPosition = speculatedPosition;
			_operationFactory = new Lazy<VisualBasicOperationFactory>(() => new VisualBasicOperationFactory(this));
		}

		internal sealed override Binder GetEnclosingBinder(int position)
		{
			return SemanticModelBinder.Mark(GetEnclosingBinderInternal(RootBinder, Root, FindInitialNodeFromPosition(position), position), IgnoresAccessibility);
		}

		private Binder GetEnclosingBinder(SyntaxNode node)
		{
			return SemanticModelBinder.Mark(GetEnclosingBinderInternal(RootBinder, Root, node, node.SpanStart), IgnoresAccessibility);
		}

		internal virtual BoundNode GetBoundRoot()
		{
			return GetUpperBoundNode(Root);
		}

		public override Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination)
		{
			CheckSyntaxNode(expression);
			expression = SyntaxFactory.GetStandaloneExpression(expression);
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			TypeSymbol typeSymbol = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(destination, "destination");
			BoundExpression boundExpression = GetLowerBoundNode(expression) as BoundExpression;
			Conversion result;
			if (boundExpression == null || TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				result = new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
			}
			else
			{
				switch (boundExpression.Kind)
				{
				case BoundKind.Lambda:
					if (((BoundLambda)boundExpression).LambdaSymbol is SourceLambdaSymbol sourceLambdaSymbol)
					{
						boundExpression = sourceLambdaSymbol.UnboundLambda;
					}
					break;
				case BoundKind.ArrayCreation:
				{
					BoundArrayLiteral arrayLiteralOpt = ((BoundArrayCreation)boundExpression).ArrayLiteralOpt;
					if (arrayLiteralOpt != null)
					{
						boundExpression = arrayLiteralOpt;
					}
					break;
				}
				}
				BoundExpression source = boundExpression;
				Binder enclosingBinder = GetEnclosingBinder(boundExpression.Syntax);
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				result = new Conversion(Conversions.ClassifyConversion(source, typeSymbol, enclosingBinder, ref useSiteInfo));
			}
			return result;
		}

		internal BoundNode GetUpperBoundNode(SyntaxNode node)
		{
			ImmutableArray<BoundNode> boundNodes = GetBoundNodes(node);
			if (boundNodes.Length == 0)
			{
				return null;
			}
			return boundNodes[0];
		}

		internal BoundNode GetLowerBoundNode(VisualBasicSyntaxNode node)
		{
			ImmutableArray<BoundNode> boundNodes = GetBoundNodes(node);
			if (boundNodes.Length == 0)
			{
				return null;
			}
			return boundNodes[boundNodes.Length - 1];
		}

		protected VisualBasicSyntaxNode GetBindableParent(VisualBasicSyntaxNode node)
		{
			VisualBasicSyntaxNode parent = node.Parent;
			if (parent == null || node == Root)
			{
				return null;
			}
			if (parent is ExpressionSyntax node2)
			{
				return SyntaxFactory.GetStandaloneExpression(node2);
			}
			if (parent is StatementSyntax statementSyntax && IsStandaloneStatement(statementSyntax))
			{
				return statementSyntax;
			}
			if (parent is AttributeSyntax result)
			{
				return result;
			}
			return null;
		}

		internal BoundNodeSummary GetBoundNodeSummary(VisualBasicSyntaxNode node)
		{
			BoundNode upperBoundNode = GetUpperBoundNode(node);
			BoundNode lowerBoundNode = GetLowerBoundNode(node);
			VisualBasicSyntaxNode bindableParent = GetBindableParent(node);
			BoundNode lowestBoundOfSyntacticParent = ((bindableParent == null) ? null : GetLowerBoundNode(bindableParent));
			return new BoundNodeSummary(lowerBoundNode, upperBoundNode, lowestBoundOfSyntacticParent);
		}

		internal override BoundNodeSummary GetInvokeSummaryForRaiseEvent(RaiseEventStatementSyntax node)
		{
			BoundNode highestBound = UnwrapRaiseEvent(GetUpperBoundNode(node));
			BoundNode lowestBound = UnwrapRaiseEvent(GetLowerBoundNode(node));
			VisualBasicSyntaxNode bindableParent = GetBindableParent(node);
			BoundNode lowestBoundOfSyntacticParent = ((bindableParent == null) ? null : UnwrapRaiseEvent(GetLowerBoundNode(bindableParent)));
			return new BoundNodeSummary(lowestBound, highestBound, lowestBoundOfSyntacticParent);
		}

		private static BoundNode UnwrapRaiseEvent(BoundNode node)
		{
			if (node is BoundRaiseEventStatement boundRaiseEventStatement)
			{
				return boundRaiseEventStatement.EventInvocation;
			}
			return node;
		}

		private static bool IsStandaloneStatement(StatementSyntax node)
		{
			switch (node.Kind())
			{
			case SyntaxKind.EmptyStatement:
			case SyntaxKind.OptionStatement:
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
			case SyntaxKind.ExpressionStatement:
			case SyntaxKind.PrintStatement:
			case SyntaxKind.WhileBlock:
			case SyntaxKind.UsingBlock:
			case SyntaxKind.SyncLockBlock:
			case SyntaxKind.WithBlock:
			case SyntaxKind.LocalDeclarationStatement:
			case SyntaxKind.LabelStatement:
			case SyntaxKind.GoToStatement:
			case SyntaxKind.StopStatement:
			case SyntaxKind.EndStatement:
			case SyntaxKind.ExitDoStatement:
			case SyntaxKind.ExitForStatement:
			case SyntaxKind.ExitSubStatement:
			case SyntaxKind.ExitFunctionStatement:
			case SyntaxKind.ExitOperatorStatement:
			case SyntaxKind.ExitPropertyStatement:
			case SyntaxKind.ExitTryStatement:
			case SyntaxKind.ExitSelectStatement:
			case SyntaxKind.ExitWhileStatement:
			case SyntaxKind.ContinueWhileStatement:
			case SyntaxKind.ContinueDoStatement:
			case SyntaxKind.ContinueForStatement:
			case SyntaxKind.ReturnStatement:
			case SyntaxKind.SingleLineIfStatement:
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.TryBlock:
			case SyntaxKind.ErrorStatement:
			case SyntaxKind.OnErrorGoToZeroStatement:
			case SyntaxKind.OnErrorGoToMinusOneStatement:
			case SyntaxKind.OnErrorGoToLabelStatement:
			case SyntaxKind.OnErrorResumeNextStatement:
			case SyntaxKind.ResumeStatement:
			case SyntaxKind.ResumeLabelStatement:
			case SyntaxKind.ResumeNextStatement:
			case SyntaxKind.SelectBlock:
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			case SyntaxKind.ThrowStatement:
			case SyntaxKind.SimpleAssignmentStatement:
			case SyntaxKind.MidAssignmentStatement:
			case SyntaxKind.AddAssignmentStatement:
			case SyntaxKind.SubtractAssignmentStatement:
			case SyntaxKind.MultiplyAssignmentStatement:
			case SyntaxKind.DivideAssignmentStatement:
			case SyntaxKind.IntegerDivideAssignmentStatement:
			case SyntaxKind.ExponentiateAssignmentStatement:
			case SyntaxKind.LeftShiftAssignmentStatement:
			case SyntaxKind.RightShiftAssignmentStatement:
			case SyntaxKind.ConcatenateAssignmentStatement:
			case SyntaxKind.CallStatement:
			case SyntaxKind.AddHandlerStatement:
			case SyntaxKind.RemoveHandlerStatement:
			case SyntaxKind.RaiseEventStatement:
			case SyntaxKind.ReDimStatement:
			case SyntaxKind.ReDimPreserveStatement:
			case SyntaxKind.EraseStatement:
			case SyntaxKind.YieldStatement:
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				return true;
			case SyntaxKind.EndIfStatement:
			case SyntaxKind.EndUsingStatement:
			case SyntaxKind.EndWithStatement:
			case SyntaxKind.EndSelectStatement:
			case SyntaxKind.EndStructureStatement:
			case SyntaxKind.EndEnumStatement:
			case SyntaxKind.EndInterfaceStatement:
			case SyntaxKind.EndClassStatement:
			case SyntaxKind.EndModuleStatement:
			case SyntaxKind.EndNamespaceStatement:
			case SyntaxKind.EndSubStatement:
			case SyntaxKind.EndFunctionStatement:
			case SyntaxKind.EndGetStatement:
			case SyntaxKind.EndSetStatement:
			case SyntaxKind.EndPropertyStatement:
			case SyntaxKind.EndOperatorStatement:
			case SyntaxKind.EndEventStatement:
			case SyntaxKind.EndAddHandlerStatement:
			case SyntaxKind.EndRemoveHandlerStatement:
			case SyntaxKind.EndRaiseEventStatement:
			case SyntaxKind.EndWhileStatement:
			case SyntaxKind.EndTryStatement:
			case SyntaxKind.EndSyncLockStatement:
			case SyntaxKind.ImportsStatement:
			case SyntaxKind.InheritsStatement:
			case SyntaxKind.ImplementsStatement:
			case SyntaxKind.EnumMemberDeclaration:
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
			case SyntaxKind.IncompleteMember:
			case SyntaxKind.FieldDeclaration:
			case SyntaxKind.IfStatement:
			case SyntaxKind.ElseIfStatement:
			case SyntaxKind.ElseStatement:
			case SyntaxKind.TryStatement:
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
			case SyntaxKind.SelectStatement:
			case SyntaxKind.CaseStatement:
			case SyntaxKind.CaseElseStatement:
			case SyntaxKind.SyncLockStatement:
			case SyntaxKind.WhileStatement:
			case SyntaxKind.ForStatement:
			case SyntaxKind.ForEachStatement:
			case SyntaxKind.NextStatement:
			case SyntaxKind.UsingStatement:
			case SyntaxKind.WithStatement:
			case SyntaxKind.SubLambdaHeader:
			case SyntaxKind.FunctionLambdaHeader:
			case SyntaxKind.SimpleDoStatement:
			case SyntaxKind.DoWhileStatement:
			case SyntaxKind.DoUntilStatement:
			case SyntaxKind.SimpleLoopStatement:
			case SyntaxKind.LoopWhileStatement:
			case SyntaxKind.LoopUntilStatement:
				return false;
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind());
			}
		}

		public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override INamedTypeSymbol GetDeclaredSymbol(TypeStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override INamedTypeSymbol GetDeclaredSymbol(EnumStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override INamespaceSymbol GetDeclaredSymbol(NamespaceStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		internal override ISymbol GetDeclaredSymbol(MethodBaseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override IParameterSymbol GetDeclaredSymbol(ParameterSyntax parameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			VisualBasicSyntaxNode parent = parameter.Parent;
			_ = parameter.Parent;
			if (parent != null && parent.Kind() == SyntaxKind.ParameterList && parent.Parent is LambdaHeaderSyntax lambdaHeaderSyntax && lambdaHeaderSyntax.Parent is LambdaExpressionSyntax node && GetLowerBoundNode(node) is BoundLambda boundLambda)
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = boundLambda.LambdaSymbol.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Location current2 = enumerator2.Current;
						if (parameter.Span.Contains(current2.SourceSpan))
						{
							return current;
						}
					}
				}
			}
			return null;
		}

		public override IAliasSymbol GetDeclaredSymbol(SimpleImportsClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public override ISymbol GetDeclaredSymbol(ModifiedIdentifierSyntax identifierSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (identifierSyntax == null)
			{
				throw new ArgumentNullException("identifierSyntax");
			}
			if (!IsInTree(identifierSyntax))
			{
				throw new ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree);
			}
			VisualBasicSyntaxNode parent = identifierSyntax.Parent;
			if (parent != null)
			{
				switch (parent.Kind())
				{
				case SyntaxKind.CollectionRangeVariable:
					return GetDeclaredSymbol((CollectionRangeVariableSyntax)parent, cancellationToken);
				case SyntaxKind.VariableNameEquals:
					parent = parent.Parent;
					if (parent != null)
					{
						switch (parent.Kind())
						{
						case SyntaxKind.ExpressionRangeVariable:
							return GetDeclaredSymbol((ExpressionRangeVariableSyntax)parent, cancellationToken);
						case SyntaxKind.AggregationRangeVariable:
							return GetDeclaredSymbol((AggregationRangeVariableSyntax)parent, cancellationToken);
						}
					}
					break;
				}
			}
			return base.GetDeclaredSymbol(identifierSyntax, cancellationToken);
		}

		public override INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (anonymousObjectCreationExpressionSyntax == null)
			{
				throw new ArgumentNullException("anonymousObjectCreationExpressionSyntax");
			}
			if (!IsInTree(anonymousObjectCreationExpressionSyntax))
			{
				throw new ArgumentException(VBResources.AnonymousObjectCreationExpressionSyntaxNotWithinTree);
			}
			if (!(GetLowerBoundNode(anonymousObjectCreationExpressionSyntax) is BoundExpression boundExpression))
			{
				return null;
			}
			return boundExpression.Type as AnonymousTypeManager.AnonymousTypePublicSymbol;
		}

		public override IPropertySymbol GetDeclaredSymbol(FieldInitializerSyntax fieldInitializerSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (fieldInitializerSyntax == null)
			{
				throw new ArgumentNullException("fieldInitializerSyntax");
			}
			if (!IsInTree(fieldInitializerSyntax))
			{
				throw new ArgumentException(VBResources.FieldInitializerSyntaxNotWithinSyntaxTree);
			}
			if (!(fieldInitializerSyntax.Parent is ObjectMemberInitializerSyntax objectMemberInitializerSyntax))
			{
				return null;
			}
			if (!(objectMemberInitializerSyntax.Parent is AnonymousObjectCreationExpressionSyntax node))
			{
				return null;
			}
			if (!(GetLowerBoundNode(node) is BoundExpression boundExpression))
			{
				return null;
			}
			if (!(boundExpression.Type is AnonymousTypeManager.AnonymousTypePublicSymbol anonymousTypePublicSymbol))
			{
				return null;
			}
			int index = objectMemberInitializerSyntax.Initializers.IndexOf(fieldInitializerSyntax);
			return anonymousTypePublicSymbol.Properties[index];
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(CollectionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree);
			}
			BoundNode lowerBoundNode = GetLowerBoundNode(rangeVariableSyntax);
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.QueryableSource)
			{
				BoundQueryableSource boundQueryableSource = (BoundQueryableSource)lowerBoundNode;
				if ((object)boundQueryableSource.RangeVariableOpt != null)
				{
					return boundQueryableSource.RangeVariableOpt;
				}
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(ExpressionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree);
			}
			BoundNode lowerBoundNode = GetLowerBoundNode(rangeVariableSyntax);
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.RangeVariableAssignment)
			{
				return ((BoundRangeVariableAssignment)lowerBoundNode).RangeVariable;
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(AggregationRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree);
			}
			BoundNode lowerBoundNode = GetLowerBoundNode(rangeVariableSyntax);
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.RangeVariableAssignment)
			{
				return ((BoundRangeVariableAssignment)lowerBoundNode).RangeVariable;
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		internal override ImmutableArray<ISymbol> GetDeclaredSymbols(FieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ImmutableArray.Create<ISymbol>();
		}

		internal override ForEachStatementInfo GetForEachStatementInfoWorker(ForEachBlockSyntax node)
		{
			BoundForEachStatement boundForEachStatement = (BoundForEachStatement)GetUpperBoundNode(node);
			if (boundForEachStatement != null)
			{
				VisualBasicCompilation compilation = Compilation;
				ImmutableArray<BoundExpression> getEnumeratorArguments = default(ImmutableArray<BoundExpression>);
				BitVector getEnumeratorDefaultArguments = default(BitVector);
				ImmutableArray<BoundExpression> moveNextArguments = default(ImmutableArray<BoundExpression>);
				BitVector moveNextDefaultArguments = default(BitVector);
				ImmutableArray<BoundExpression> currentArguments = default(ImmutableArray<BoundExpression>);
				BitVector currentDefaultArguments = default(BitVector);
				return GetForEachStatementInfo(boundForEachStatement, compilation, out getEnumeratorArguments, out getEnumeratorDefaultArguments, out moveNextArguments, out moveNextDefaultArguments, out currentArguments, out currentDefaultArguments);
			}
			return default(ForEachStatementInfo);
		}

		internal static ForEachStatementInfo GetForEachStatementInfo(BoundForEachStatement boundForEach, VisualBasicCompilation compilation, out ImmutableArray<BoundExpression> getEnumeratorArguments, out BitVector getEnumeratorDefaultArguments, out ImmutableArray<BoundExpression> moveNextArguments, out BitVector moveNextDefaultArguments, out ImmutableArray<BoundExpression> currentArguments, out BitVector currentDefaultArguments)
		{
			getEnumeratorArguments = default(ImmutableArray<BoundExpression>);
			moveNextArguments = default(ImmutableArray<BoundExpression>);
			currentArguments = default(ImmutableArray<BoundExpression>);
			ForEachEnumeratorInfo enumeratorInfo = boundForEach.EnumeratorInfo;
			MethodSymbol getEnumeratorMethod = null;
			if (enumeratorInfo.GetEnumerator != null && enumeratorInfo.GetEnumerator.Kind == BoundKind.Call)
			{
				BoundCall boundCall = (BoundCall)enumeratorInfo.GetEnumerator;
				getEnumeratorMethod = boundCall.Method;
				getEnumeratorArguments = boundCall.Arguments;
				getEnumeratorDefaultArguments = boundCall.DefaultArguments;
			}
			MethodSymbol moveNextMethod = null;
			if (enumeratorInfo.MoveNext != null && enumeratorInfo.MoveNext.Kind == BoundKind.Call)
			{
				BoundCall boundCall2 = (BoundCall)enumeratorInfo.MoveNext;
				moveNextMethod = boundCall2.Method;
				moveNextArguments = boundCall2.Arguments;
				moveNextDefaultArguments = boundCall2.DefaultArguments;
			}
			PropertySymbol propertySymbol = null;
			if (enumeratorInfo.Current != null && enumeratorInfo.Current.Kind == BoundKind.PropertyAccess)
			{
				BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)enumeratorInfo.Current;
				propertySymbol = boundPropertyAccess.PropertySymbol;
				currentArguments = boundPropertyAccess.Arguments;
				currentDefaultArguments = boundPropertyAccess.DefaultArguments;
			}
			Conversion currentConversion = default(Conversion);
			Conversion elementConversion = default(Conversion);
			TypeSymbol elementType = enumeratorInfo.ElementType;
			if ((object)elementType != null && !TypeSymbolExtensions.IsErrorType(elementType))
			{
				if ((object)propertySymbol != null && !TypeSymbolExtensions.IsErrorType(propertySymbol.Type))
				{
					TypeSymbol type = propertySymbol.Type;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					currentConversion = new Conversion(Conversions.ClassifyConversion(type, elementType, ref useSiteInfo));
				}
				BoundExpression currentConversion2 = enumeratorInfo.CurrentConversion;
				if (currentConversion2 != null && !TypeSymbolExtensions.IsErrorType(currentConversion2.Type))
				{
					TypeSymbol type2 = currentConversion2.Type;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					elementConversion = new Conversion(Conversions.ClassifyConversion(elementType, type2, ref useSiteInfo));
				}
			}
			BoundExpression boundExpression = boundForEach.Collection;
			if (boundExpression.Kind == BoundKind.Conversion)
			{
				BoundConversion boundConversion = (BoundConversion)boundExpression;
				if (!boundConversion.ExplicitCastInCode)
				{
					boundExpression = boundConversion.Operand;
				}
			}
			return new ForEachStatementInfo(getEnumeratorMethod, moveNextMethod, propertySymbol, (enumeratorInfo.NeedToDispose || ((object)boundExpression.Type != null && TypeSymbolExtensions.IsArrayType(boundExpression.Type))) ? ((MethodSymbol)compilation.GetSpecialTypeMember(SpecialMember.System_IDisposable__Dispose)) : null, elementType, elementConversion, currentConversion);
		}

		internal override SymbolInfo GetAttributeSymbolInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, GetBoundNodeSummary(attribute), null);
		}

		internal override VisualBasicTypeInfo GetAttributeTypeInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetTypeInfoForNode(GetBoundNodeSummary(attribute));
		}

		internal override ImmutableArray<Symbol> GetAttributeMemberGroup(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberGroupForNode(GetBoundNodeSummary(attribute), null);
		}

		internal override SymbolInfo GetExpressionSymbolInfo(ExpressionSyntax node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidateSymbolInfoOptions(options);
			if (IsSpeculativeSemanticModel)
			{
				node = SyntaxFactory.GetStandaloneExpression(node);
			}
			if (SyntaxNodeExtensions.EnclosingStructuredTrivia(node) != null)
			{
				return SymbolInfo.None;
			}
			return GetSymbolInfoForNode(options, GetBoundNodeSummary(node), null);
		}

		internal override IOperation GetOperationWorker(VisualBasicSyntaxNode node, CancellationToken cancellationToken)
		{
			IOperation value = null;
			try
			{
				_rwLock.EnterReadLock();
				if (_guardedIOperationNodeMap.Count > 0)
				{
					return _guardedIOperationNodeMap.TryGetValue(node, out value) ? value : null;
				}
			}
			finally
			{
				_rwLock.ExitReadLock();
			}
			BoundNode boundRoot = GetBoundRoot();
			IOperation operation = _operationFactory.Value.Create(boundRoot);
			try
			{
				_rwLock.EnterWriteLock();
				if (_guardedIOperationNodeMap.Count > 0)
				{
					return _guardedIOperationNodeMap.TryGetValue(node, out value) ? value : null;
				}
				Operation.SetParentOperation(operation, null);
				OperationMapBuilder.AddToMap(operation, _guardedIOperationNodeMap);
				return _guardedIOperationNodeMap.TryGetValue(node, out value) ? value : null;
			}
			finally
			{
				_rwLock.ExitWriteLock();
			}
		}

		internal override VisualBasicTypeInfo GetExpressionTypeInfo(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (IsSpeculativeSemanticModel)
			{
				node = SyntaxFactory.GetStandaloneExpression(node);
			}
			if (SyntaxNodeExtensions.EnclosingStructuredTrivia(node) != null)
			{
				return VisualBasicTypeInfo.None;
			}
			return GetTypeInfoForNode(GetBoundNodeSummary(node));
		}

		internal override ImmutableArray<Symbol> GetExpressionMemberGroup(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (IsSpeculativeSemanticModel)
			{
				node = SyntaxFactory.GetStandaloneExpression(node);
			}
			if (SyntaxNodeExtensions.EnclosingStructuredTrivia(node) != null)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			return GetMemberGroupForNode(GetBoundNodeSummary(node), null);
		}

		internal override ConstantValue GetExpressionConstantValue(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (IsSpeculativeSemanticModel)
			{
				node = SyntaxFactory.GetStandaloneExpression(node);
			}
			if (SyntaxNodeExtensions.EnclosingStructuredTrivia(node) != null)
			{
				return null;
			}
			return GetConstantValueForNode(GetBoundNodeSummary(node));
		}

		internal override SymbolInfo GetCollectionInitializerAddSymbolInfo(ObjectCreationExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (GetLowerBoundNode(collectionInitializer.Initializer) is BoundCollectionInitializerExpression boundCollectionInitializerExpression)
			{
				BoundExpression boundExpression = boundCollectionInitializerExpression.Initializers[((ObjectCollectionInitializerSyntax)collectionInitializer.Initializer).Initializer.Initializers.IndexOf(node)];
				if (boundExpression.WasCompilerGenerated)
				{
					return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundExpression, boundExpression, null), null);
				}
			}
			return SymbolInfo.None;
		}

		internal override SymbolInfo GetCrefReferenceSymbolInfo(CrefReferenceSyntax crefReference, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SymbolInfo.None;
		}

		internal override SymbolInfo GetQueryClauseSymbolInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			SyntaxKind syntaxKind = node.Kind();
			BoundNode lowerBoundNode;
			if (syntaxKind == SyntaxKind.FromClause)
			{
				if (((FromClauseSyntax)node).Variables.Count < 2 && node.Parent != null && node.Parent.Kind() == SyntaxKind.QueryExpression)
				{
					QueryExpressionSyntax queryExpressionSyntax = (QueryExpressionSyntax)node.Parent;
					if (queryExpressionSyntax.Clauses.Count == 1 && queryExpressionSyntax.Clauses[0] == node)
					{
						lowerBoundNode = GetLowerBoundNode(queryExpressionSyntax);
						if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.QueryExpression)
						{
							BoundQueryExpression boundQueryExpression = (BoundQueryExpression)lowerBoundNode;
							if (boundQueryExpression.LastOperator.Syntax == node)
							{
								return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundQueryExpression.LastOperator, boundQueryExpression.LastOperator, null), null);
							}
						}
					}
				}
				return SymbolInfo.None;
			}
			lowerBoundNode = GetLowerBoundNode(node);
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.QueryClause)
			{
				if (syntaxKind == SyntaxKind.SelectClause && ((BoundQueryClause)lowerBoundNode).UnderlyingExpression.Kind == BoundKind.QueryClause)
				{
					return SymbolInfo.None;
				}
				return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(lowerBoundNode, lowerBoundNode, null), null);
			}
			return SymbolInfo.None;
		}

		internal override SymbolInfo GetLetClauseSymbolInfo(ExpressionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			BoundNode upperBoundNode = GetUpperBoundNode(node);
			if (upperBoundNode != null && upperBoundNode.Kind == BoundKind.QueryClause)
			{
				if (((BoundQueryClause)upperBoundNode).UnderlyingExpression.Kind == BoundKind.QueryClause)
				{
					return SymbolInfo.None;
				}
				return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(upperBoundNode, upperBoundNode, null), null);
			}
			return SymbolInfo.None;
		}

		internal override SymbolInfo GetOrderingSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			BoundNode lowerBoundNode = GetLowerBoundNode(node);
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.Ordering)
			{
				return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(lowerBoundNode, lowerBoundNode, null), null);
			}
			return SymbolInfo.None;
		}

		internal override AggregateClauseSymbolInfo GetAggregateClauseSymbolInfoWorker(AggregateClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			BoundNode lowerBoundNode = GetLowerBoundNode(node);
			AggregateClauseSymbolInfo result;
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.AggregateClause)
			{
				if (((BoundAggregateClause)lowerBoundNode).UnderlyingExpression is BoundQueryClauseBase)
				{
					result = new AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None);
				}
				else
				{
					SymbolInfo symbolInfoForNode = GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(lowerBoundNode, lowerBoundNode, null), null);
					BoundQueryClause boundQueryClause = (BoundQueryClause)CompilerGeneratedNodeFinder.FindIn(lowerBoundNode, node, BoundKind.QueryClause);
					result = ((boundQueryClause == null) ? new AggregateClauseSymbolInfo(symbolInfoForNode) : ((!(boundQueryClause.UnderlyingExpression is BoundQueryClauseBase)) ? new AggregateClauseSymbolInfo(GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundQueryClause, boundQueryClause, null), null), symbolInfoForNode) : new AggregateClauseSymbolInfo(SymbolInfo.None, symbolInfoForNode)));
				}
			}
			else
			{
				result = new AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None);
			}
			return result;
		}

		internal override CollectionRangeVariableSymbolInfo GetCollectionRangeVariableSymbolInfoWorker(CollectionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			ImmutableArray<BoundNode> boundNodes = GetBoundNodes(node);
			BoundNode boundNode = null;
			int num = boundNodes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (boundNodes[i].Kind == BoundKind.QueryClause || boundNodes[i].Kind == BoundKind.QueryableSource)
				{
					boundNode = boundNodes[i];
					break;
				}
			}
			if (boundNode == null)
			{
				return CollectionRangeVariableSymbolInfo.None;
			}
			SymbolInfo toQueryableCollectionConversion = SymbolInfo.None;
			SymbolInfo asClauseConversion = SymbolInfo.None;
			SymbolInfo selectMany = SymbolInfo.None;
			if (boundNode.Kind == BoundKind.QueryClause)
			{
				selectMany = GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundNode, boundNode, null), null);
				boundNode = GetLowerBoundNode(node);
			}
			if (boundNode != null && boundNode.Kind == BoundKind.QueryableSource)
			{
				BoundQueryableSource boundQueryableSource = (BoundQueryableSource)boundNode;
				switch (boundQueryableSource.Source.Kind)
				{
				case BoundKind.QueryClause:
				{
					asClauseConversion = GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundQueryableSource.Source, boundQueryableSource.Source, null), null);
					BoundToQueryableCollectionConversion boundToQueryableCollectionConversion = (BoundToQueryableCollectionConversion)CompilerGeneratedNodeFinder.FindIn(((BoundQueryClause)boundQueryableSource.Source).UnderlyingExpression, node.Expression, BoundKind.ToQueryableCollectionConversion);
					toQueryableCollectionConversion = ((boundToQueryableCollectionConversion != null) ? GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundToQueryableCollectionConversion, boundToQueryableCollectionConversion, null), null) : SymbolInfo.None);
					break;
				}
				case BoundKind.ToQueryableCollectionConversion:
					asClauseConversion = SymbolInfo.None;
					toQueryableCollectionConversion = GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, new BoundNodeSummary(boundQueryableSource.Source, boundQueryableSource.Source, null), null);
					break;
				case BoundKind.QuerySource:
					asClauseConversion = SymbolInfo.None;
					toQueryableCollectionConversion = SymbolInfo.None;
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(boundQueryableSource.Source.Kind);
				}
			}
			else
			{
				selectMany = SymbolInfo.None;
			}
			return new CollectionRangeVariableSymbolInfo(toQueryableCollectionConversion, asClauseConversion, selectMany);
		}

		internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel)
		{
			Binder speculativeBinderForExpression = GetSpeculativeBinderForExpression(position, type, bindingOption);
			if (speculativeBinderForExpression == null)
			{
				speculativeModel = null;
				return false;
			}
			speculativeModel = new SpeculativeMemberSemanticModel(parentModel, type, speculativeBinderForExpression, position);
			return true;
		}

		internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, RangeArgumentSyntax rangeArgument, out SemanticModel speculativeModel)
		{
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder == null)
			{
				speculativeModel = null;
				return false;
			}
			enclosingBinder = SpeculativeBinder.Create(enclosingBinder);
			speculativeModel = new SpeculativeMemberSemanticModel(parentModel, rangeArgument, enclosingBinder, position);
			return true;
		}

		internal void CacheBoundNodes(BoundNode boundNode, SyntaxNode thisSyntaxNodeOnly = null)
		{
			_rwLock.EnterWriteLock();
			try
			{
				SemanticModelMapsBuilder.GuardedCacheBoundNodes(boundNode, this, _guardedBoundNodeMap, thisSyntaxNodeOnly);
			}
			finally
			{
				_rwLock.ExitWriteLock();
			}
		}

		private void EnsureFullyBoundIfImplicitVariablesAllowed()
		{
			if (!RootBinder.ImplicitVariableDeclarationAllowed || RootBinder.AllImplicitVariableDeclarationsAreHandled)
			{
				return;
			}
			_rwLock.EnterWriteLock();
			try
			{
				if (!RootBinder.AllImplicitVariableDeclarationsAreHandled)
				{
					GuardedIncrementalBind(Root, RootBinder);
					RootBinder.DisallowFurtherImplicitVariableDeclaration(BindingDiagnosticBag.Discarded);
				}
			}
			finally
			{
				_rwLock.ExitWriteLock();
			}
		}

		private ImmutableArray<BoundNode> GuardedGetBoundNodesFromMap(SyntaxNode node)
		{
			ImmutableArray<BoundNode> value = default(ImmutableArray<BoundNode>);
			if (!_guardedBoundNodeMap.TryGetValue(node, out value))
			{
				return default(ImmutableArray<BoundNode>);
			}
			return value;
		}

		private Binder GetEnclosingBinderInternal(Binder memberBinder, SyntaxNode binderRoot, SyntaxNode node, int position)
		{
			Binder binder = null;
			EnsureFullyBoundIfImplicitVariablesAllowed();
			SyntaxNode syntaxNode = node;
			while (true)
			{
				SyntaxList<StatementSyntax> body = default(SyntaxList<StatementSyntax>);
				if (VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.DocumentationCommentTrivia)
				{
					return GetEnclosingBinderInternal(memberBinder, binderRoot, (VisualBasicSyntaxNode)((DocumentationCommentTriviaSyntax)syntaxNode).ParentTrivia.Token.Parent, position);
				}
				if (SyntaxFacts.InBlockInterior(syntaxNode, position, ref body))
				{
					binder = memberBinder.GetBinder(body);
					if (binder != null)
					{
						return binder;
					}
				}
				else if (SyntaxFacts.InLambdaInterior(syntaxNode, position))
				{
					if (syntaxNode != binderRoot)
					{
						LambdaBodyBinder lambdaBodyBinder = GetLambdaBodyBinder((LambdaExpressionSyntax)syntaxNode);
						if (lambdaBodyBinder != null)
						{
							if ((VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.MultiLineFunctionLambdaExpression || VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.MultiLineSubLambdaExpression) && ((MultiLineLambdaExpressionSyntax)syntaxNode).SubOrFunctionHeader.FullSpan.Contains(position))
							{
								return lambdaBodyBinder;
							}
							binder = GetEnclosingBinderInternal(lambdaBodyBinder, lambdaBodyBinder.Root, node, position);
							if (binder != null)
							{
								return binder;
							}
						}
					}
					else if (VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.MultiLineFunctionLambdaExpression || VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.MultiLineSubLambdaExpression)
					{
						binder = memberBinder.GetBinder(((MultiLineLambdaExpressionSyntax)syntaxNode).Statements);
						if (binder != null)
						{
							return binder;
						}
					}
					else if (VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.SingleLineSubLambdaExpression)
					{
						binder = memberBinder.GetBinder(((SingleLineLambdaExpressionSyntax)syntaxNode).Statements);
						if (binder != null)
						{
							return binder;
						}
					}
				}
				else
				{
					if (InQueryInterior(syntaxNode, position, out binder))
					{
						return binder;
					}
					if (InAnonymousTypeInitializerInterior(syntaxNode, position, out binder))
					{
						return binder;
					}
					if (InWithStatementExpressionInterior(syntaxNode))
					{
						syntaxNode = syntaxNode.Parent!.Parent;
						if (syntaxNode == binderRoot)
						{
							return memberBinder;
						}
						syntaxNode = syntaxNode.Parent;
					}
				}
				binder = memberBinder.GetBinder(syntaxNode);
				if (binder != null)
				{
					return binder;
				}
				if (syntaxNode == binderRoot)
				{
					break;
				}
				syntaxNode = syntaxNode.Parent;
			}
			return memberBinder;
		}

		private bool InQueryInterior(SyntaxNode node, int position, out Binder binder)
		{
			binder = null;
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.WhereClause:
			{
				WhereClauseSyntax whereClauseSyntax = (WhereClauseSyntax)node;
				binder = GetSingleLambdaClauseLambdaBinder(whereClauseSyntax, whereClauseSyntax.WhereKeyword, position);
				break;
			}
			case SyntaxKind.SkipWhileClause:
			case SyntaxKind.TakeWhileClause:
			{
				PartitionWhileClauseSyntax partitionWhileClauseSyntax = (PartitionWhileClauseSyntax)node;
				binder = GetSingleLambdaClauseLambdaBinder(partitionWhileClauseSyntax, partitionWhileClauseSyntax.WhileKeyword, position);
				break;
			}
			case SyntaxKind.SelectClause:
			{
				SelectClauseSyntax selectClauseSyntax = (SelectClauseSyntax)node;
				binder = GetSingleLambdaClauseLambdaBinder(selectClauseSyntax, selectClauseSyntax.SelectKeyword, position);
				break;
			}
			case SyntaxKind.LetClause:
				binder = GetLetClauseLambdaBinder((LetClauseSyntax)node, position);
				break;
			case SyntaxKind.FromClause:
				binder = GetFromClauseLambdaBinder((FromClauseSyntax)node, position);
				break;
			case SyntaxKind.GroupByClause:
				binder = GetGroupByClauseLambdaBinder((GroupByClauseSyntax)node, position);
				break;
			case SyntaxKind.OrderByClause:
			{
				OrderByClauseSyntax orderByClauseSyntax = (OrderByClauseSyntax)node;
				binder = GetSingleLambdaClauseLambdaBinder(orderByClauseSyntax, orderByClauseSyntax.ByKeyword.IsMissing ? orderByClauseSyntax.OrderKeyword : orderByClauseSyntax.ByKeyword, position);
				break;
			}
			case SyntaxKind.SimpleJoinClause:
				binder = GetJoinClauseLambdaBinder((SimpleJoinClauseSyntax)node, position);
				break;
			case SyntaxKind.GroupJoinClause:
				binder = GetGroupJoinClauseLambdaBinder((GroupJoinClauseSyntax)node, position);
				break;
			case SyntaxKind.AggregateClause:
				binder = GetAggregateClauseLambdaBinder((AggregateClauseSyntax)node, position);
				break;
			case SyntaxKind.FunctionAggregation:
				binder = GetFunctionAggregationLambdaBinder((FunctionAggregationSyntax)node, position);
				break;
			}
			return binder != null;
		}

		private Binder GetAggregateClauseLambdaBinder(AggregateClauseSyntax aggregate, int position)
		{
			Binder binder = null;
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(aggregate, position))
			{
				if (!aggregate.IntoKeyword.IsMissing && aggregate.IntoKeyword.SpanStart <= position)
				{
					ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(aggregate);
					if (!queryClauseLambdaBinders.IsEmpty)
					{
						binder = queryClauseLambdaBinders.Last();
					}
				}
				else if (aggregate.AggregateKeyword.SpanStart <= position)
				{
					binder = GetCollectionRangeVariablesLambdaBinder(aggregate.Variables, position);
					if (binder == null)
					{
						ImmutableArray<Binder> queryClauseLambdaBinders2 = GetQueryClauseLambdaBinders(aggregate);
						if (!queryClauseLambdaBinders2.IsDefault && queryClauseLambdaBinders2.Length == 2)
						{
							binder = queryClauseLambdaBinders2[0];
						}
					}
				}
			}
			return binder;
		}

		private Binder GetGroupJoinClauseLambdaBinder(GroupJoinClauseSyntax join, int position)
		{
			Binder result = null;
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(join, position))
			{
				if (!join.IntoKeyword.IsMissing && join.IntoKeyword.SpanStart <= position)
				{
					ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(join);
					if (!queryClauseLambdaBinders.IsDefault && queryClauseLambdaBinders.Length == 3)
					{
						result = queryClauseLambdaBinders[2];
					}
				}
				else
				{
					result = GetJoinClauseLambdaBinder(join, position);
				}
			}
			return result;
		}

		private Binder GetJoinClauseLambdaBinder(JoinClauseSyntax join, int position)
		{
			Binder result = null;
			if (!join.OnKeyword.IsMissing && join.OnKeyword.SpanStart <= position && SyntaxFacts.InSpanOrEffectiveTrailingOfNode(join, position))
			{
				ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(join);
				if (!queryClauseLambdaBinders.IsEmpty)
				{
					result = queryClauseLambdaBinders[0];
				}
			}
			return result;
		}

		private Binder GetFromClauseLambdaBinder(FromClauseSyntax from, int position)
		{
			Binder result = null;
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(from, position))
			{
				result = GetCollectionRangeVariablesLambdaBinder(from.Variables, position);
			}
			return result;
		}

		private Binder GetCollectionRangeVariablesLambdaBinder(SeparatedSyntaxList<CollectionRangeVariableSyntax> variables, int position)
		{
			Binder result = null;
			int num = variables.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				CollectionRangeVariableSyntax collectionRangeVariableSyntax = variables[i];
				if (!SyntaxFacts.InSpanOrEffectiveTrailingOfNode(collectionRangeVariableSyntax, position) && position >= collectionRangeVariableSyntax.SpanStart)
				{
					continue;
				}
				if (i > 0 || (collectionRangeVariableSyntax.Parent.Kind() != SyntaxKind.AggregateClause && collectionRangeVariableSyntax.Parent.Parent != null && (collectionRangeVariableSyntax.Parent.Parent.Kind() != SyntaxKind.QueryExpression || ((QueryExpressionSyntax)collectionRangeVariableSyntax.Parent.Parent).Clauses.FirstOrDefault() != collectionRangeVariableSyntax.Parent)))
				{
					ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(collectionRangeVariableSyntax);
					if (!queryClauseLambdaBinders.IsEmpty)
					{
						result = queryClauseLambdaBinders[0];
					}
				}
				break;
			}
			return result;
		}

		private Binder GetLetClauseLambdaBinder(LetClauseSyntax let, int position)
		{
			Binder result = null;
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(let, position))
			{
				SeparatedSyntaxList<ExpressionRangeVariableSyntax>.Enumerator enumerator = let.Variables.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ExpressionRangeVariableSyntax current = enumerator.Current;
					if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(current, position) || position < current.SpanStart)
					{
						ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(current);
						if (!queryClauseLambdaBinders.IsEmpty)
						{
							result = queryClauseLambdaBinders[0];
						}
						break;
					}
				}
			}
			return result;
		}

		private Binder GetGroupByClauseLambdaBinder(GroupByClauseSyntax groupBy, int position)
		{
			Binder result = null;
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(groupBy, position))
			{
				ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(groupBy);
				if (!queryClauseLambdaBinders.IsEmpty)
				{
					result = ((position < groupBy.ByKeyword.SpanStart) ? ((queryClauseLambdaBinders.Length > 2) ? queryClauseLambdaBinders[1] : queryClauseLambdaBinders[0]) : ((position >= groupBy.IntoKeyword.SpanStart) ? queryClauseLambdaBinders.Last() : queryClauseLambdaBinders[0]));
				}
			}
			return result;
		}

		private Binder GetFunctionAggregationLambdaBinder(FunctionAggregationSyntax func, int position)
		{
			Binder result = null;
			if (!func.OpenParenToken.IsMissing && func.OpenParenToken.SpanStart <= position && ((func.CloseParenToken.IsMissing && SyntaxFacts.InSpanOrEffectiveTrailingOfNode(func, position)) || position < func.CloseParenToken.SpanStart))
			{
				ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(func);
				if (!queryClauseLambdaBinders.IsDefaultOrEmpty)
				{
					result = queryClauseLambdaBinders[0];
				}
			}
			return result;
		}

		private Binder GetSingleLambdaClauseLambdaBinder(QueryClauseSyntax operatorSyntax, SyntaxToken operatorKeyWord, int position)
		{
			if (operatorKeyWord.SpanStart <= position && SyntaxFacts.InSpanOrEffectiveTrailingOfNode(operatorSyntax, position))
			{
				ImmutableArray<Binder> queryClauseLambdaBinders = GetQueryClauseLambdaBinders(operatorSyntax);
				if (!queryClauseLambdaBinders.IsDefaultOrEmpty)
				{
					return queryClauseLambdaBinders[0];
				}
			}
			return null;
		}

		private ImmutableArray<Binder> GetQueryClauseLambdaBinders(VisualBasicSyntaxNode node)
		{
			ImmutableArray<Binder> value = default(ImmutableArray<Binder>);
			_rwLock.EnterReadLock();
			try
			{
				if (_guardedQueryBindersMap.TryGetValue(node, out value))
				{
					return value;
				}
			}
			finally
			{
				_rwLock.ExitReadLock();
			}
			BoundNode upperBoundNode = GetUpperBoundNode(node);
			_rwLock.EnterWriteLock();
			try
			{
				if (_guardedQueryBindersMap.TryGetValue(node, out value))
				{
					return value;
				}
				if (upperBoundNode != null && upperBoundNode.Kind == BoundKind.NoOpStatement)
				{
					_ = upperBoundNode.HasErrors;
				}
				_guardedQueryBindersMap.Add(node, default(ImmutableArray<Binder>));
				return default(ImmutableArray<Binder>);
			}
			finally
			{
				_rwLock.ExitWriteLock();
			}
		}

		private bool InAnonymousTypeInitializerInterior(SyntaxNode node, int position, out Binder binder)
		{
			binder = null;
			if ((VisualBasicExtensions.Kind(node) == SyntaxKind.InferredFieldInitializer || VisualBasicExtensions.Kind(node) == SyntaxKind.NamedFieldInitializer) && node.Parent != null && VisualBasicExtensions.Kind(node.Parent) == SyntaxKind.ObjectMemberInitializer && node.Parent!.Parent != null && VisualBasicExtensions.Kind(node.Parent!.Parent) == SyntaxKind.AnonymousObjectCreationExpression)
			{
				FieldInitializerSyntax fieldInitializerSyntax = (FieldInitializerSyntax)node;
				if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(fieldInitializerSyntax, position))
				{
					Binder.AnonymousTypeFieldInitializerBinder value = null;
					_rwLock.EnterReadLock();
					try
					{
						if (_guardedAnonymousTypeBinderMap.TryGetValue(fieldInitializerSyntax, out value))
						{
							binder = value;
							return binder != null;
						}
					}
					finally
					{
						_rwLock.ExitReadLock();
					}
					BoundNode upperBoundNode = GetUpperBoundNode(fieldInitializerSyntax.Parent.Parent);
					_rwLock.EnterReadLock();
					try
					{
						if (_guardedAnonymousTypeBinderMap.TryGetValue(fieldInitializerSyntax, out value))
						{
							binder = value;
							return binder != null;
						}
						if (upperBoundNode != null && upperBoundNode.Kind == BoundKind.NoOpStatement)
						{
							_ = upperBoundNode.HasErrors;
						}
					}
					finally
					{
						_rwLock.ExitReadLock();
					}
				}
			}
			return false;
		}

		private static bool InWithStatementExpressionInterior(SyntaxNode node)
		{
			if (node is ExpressionSyntax expressionSyntax)
			{
				VisualBasicSyntaxNode parent = expressionSyntax.Parent;
				if (parent != null && parent.Kind() == SyntaxKind.WithStatement)
				{
					parent = parent.Parent;
					return parent != null && parent.Kind() == SyntaxKind.WithBlock && parent.Parent != null;
				}
			}
			return false;
		}

		private LambdaBodyBinder GetLambdaBodyBinder(LambdaExpressionSyntax lambda)
		{
			return GetBoundLambda(lambda)?.LambdaBinderOpt;
		}

		private BoundLambda GetBoundLambda(LambdaExpressionSyntax lambda)
		{
			return (BoundLambda)GetLowerBoundNode(lambda);
		}

		[Conditional("DEBUG")]
		private void AssertIfShouldHaveFound(VisualBasicSyntaxNode node)
		{
		}

		internal ImmutableArray<BoundNode> GetBoundNodes(SyntaxNode node)
		{
			ImmutableArray<BoundNode> result = default(ImmutableArray<BoundNode>);
			EnsureFullyBoundIfImplicitVariablesAllowed();
			_rwLock.EnterReadLock();
			try
			{
				result = GuardedGetBoundNodesFromMap(node);
			}
			finally
			{
				_rwLock.ExitReadLock();
			}
			if (!result.IsDefault)
			{
				return result;
			}
			if (IsNonExpressionCollectionInitializer(node))
			{
				return ImmutableArray<BoundNode>.Empty;
			}
			SyntaxNode bindingRoot = GetBindingRoot(node);
			Binder enclosingBinder = GetEnclosingBinder(bindingRoot);
			_rwLock.EnterWriteLock();
			try
			{
				if (GuardedGetBoundNodesFromMap(node).IsDefault)
				{
					GuardedIncrementalBind(bindingRoot, enclosingBinder);
				}
				result = GuardedGetBoundNodesFromMap(node);
				if (!result.IsDefault)
				{
					return result;
				}
			}
			finally
			{
				_rwLock.ExitWriteLock();
			}
			if (node is ExpressionSyntax || node is StatementSyntax)
			{
				IncrementalBinder binder = new IncrementalBinder(this, GetEnclosingBinder(node));
				_rwLock.EnterWriteLock();
				try
				{
					if (GuardedGetBoundNodesFromMap(node).IsDefault)
					{
						SemanticModelMapsBuilder.GuardedCacheBoundNodes(Bind(binder, node, BindingDiagnosticBag.Discarded), this, _guardedBoundNodeMap, node);
					}
					result = GuardedGetBoundNodesFromMap(node);
					if (!result.IsDefault)
					{
						return result;
					}
				}
				finally
				{
					_rwLock.ExitWriteLock();
				}
			}
			return ImmutableArray<BoundNode>.Empty;
		}

		private static bool IsNonExpressionCollectionInitializer(SyntaxNode syntax)
		{
			SyntaxNode parent = syntax.Parent;
			if (VisualBasicExtensions.Kind(syntax) == SyntaxKind.CollectionInitializer && parent != null)
			{
				if (VisualBasicExtensions.Kind(parent) == SyntaxKind.ObjectCollectionInitializer)
				{
					return true;
				}
				if (VisualBasicExtensions.Kind(parent) == SyntaxKind.CollectionInitializer)
				{
					parent = parent.Parent;
					return parent != null && VisualBasicExtensions.Kind(parent) == SyntaxKind.ObjectCollectionInitializer;
				}
			}
			return false;
		}

		private void GuardedIncrementalBind(SyntaxNode bindingRoot, Binder enclosingBinder)
		{
			if (_guardedBoundNodeMap.ContainsKey(bindingRoot))
			{
				return;
			}
			IncrementalBinder binder = new IncrementalBinder(this, enclosingBinder);
			BoundNode boundNode = Bind(binder, bindingRoot, BindingDiagnosticBag.Discarded);
			if (boundNode != null)
			{
				SemanticModelMapsBuilder.GuardedCacheBoundNodes(boundNode, this, _guardedBoundNodeMap);
				if (!_guardedBoundNodeMap.ContainsKey(bindingRoot))
				{
					_guardedBoundNodeMap.Add(bindingRoot, ImmutableArray.Create(boundNode));
				}
			}
		}

		private SyntaxNode GetBindingRoot(SyntaxNode node)
		{
			StatementSyntax statementSyntax = null;
			while (node != Root)
			{
				if (statementSyntax == null && node is StatementSyntax statementSyntax2 && IsStandaloneStatement(statementSyntax2))
				{
					statementSyntax = statementSyntax2;
				}
				if (VisualBasicExtensions.Kind(node) == SyntaxKind.DocumentationCommentTrivia)
				{
					node = (VisualBasicSyntaxNode)((DocumentationCommentTriviaSyntax)node).ParentTrivia.Token.Parent;
					continue;
				}
				if (SyntaxNodeExtensions.IsLambdaExpressionSyntax(node))
				{
					statementSyntax = null;
				}
				node = node.Parent;
			}
			if (statementSyntax != null)
			{
				return statementSyntax;
			}
			return Root;
		}

		internal override AwaitExpressionInfo GetAwaitExpressionInfoWorker(AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken = default(CancellationToken))
		{
			BoundNode lowerBoundNode = GetLowerBoundNode(awaitExpression);
			AwaitExpressionInfo result;
			if (lowerBoundNode != null && lowerBoundNode.Kind == BoundKind.AwaitOperator)
			{
				BoundAwaitOperator boundAwaitOperator = (BoundAwaitOperator)lowerBoundNode;
				result = new AwaitExpressionInfo(boundAwaitOperator.GetAwaiter.ExpressionSymbol as MethodSymbol, boundAwaitOperator.IsCompleted.ExpressionSymbol as PropertySymbol, boundAwaitOperator.GetResult.ExpressionSymbol as MethodSymbol);
			}
			else
			{
				result = default(AwaitExpressionInfo);
			}
			return result;
		}
	}
}
