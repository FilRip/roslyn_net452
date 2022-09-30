using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LocalRewriter : BoundTreeRewriterWithStackGuard
    {
        private abstract class DecisionDagRewriter : PatternLocalRewriter
        {
            protected sealed class WhenClauseMightAssignPatternVariableWalker : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
            {
                private bool _mightAssignSomething;

                public bool MightAssignSomething(BoundExpression expr)
                {
                    if (expr == null)
                    {
                        return false;
                    }
                    _mightAssignSomething = false;
                    Visit(expr);
                    return _mightAssignSomething;
                }

                public override BoundNode Visit(BoundNode node)
                {
                    if (node is BoundExpression boundExpression && (object)boundExpression.ConstantValue != null)
                    {
                        return null;
                    }
                    if (!_mightAssignSomething)
                    {
                        return base.Visit(node);
                    }
                    return null;
                }

                public override BoundNode VisitCall(BoundCall node)
                {
                    if (node.Method.MethodKind == MethodKind.LocalFunction || !node.ArgumentRefKindsOpt.IsDefault || MethodMayMutateReceiver(node.ReceiverOpt, node.Method))
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitCall(node);
                    }
                    return null;
                }

                private static bool MethodMayMutateReceiver(BoundExpression receiver, MethodSymbol method)
                {
                    if (method != null && !method.IsStatic && !method.IsEffectivelyReadOnly)
                    {
                        TypeSymbol? type = receiver.Type;
                        if ((object)type != null && !type!.IsReferenceType)
                        {
                            return !method.ContainingType.SpecialType.IsPrimitiveRecursiveStruct();
                        }
                    }
                    return false;
                }

                public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
                {
                    if (MethodMayMutateReceiver(node.ReceiverOpt, node.PropertySymbol.GetMethod))
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitPropertyAccess(node);
                    }
                    return null;
                }

                public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
                {
                    _mightAssignSomething = true;
                    return null;
                }

                public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
                {
                    _mightAssignSomething = true;
                    return null;
                }

                public override BoundNode VisitConversion(BoundConversion node)
                {
                    visitConversion(node.Conversion);
                    if (!_mightAssignSomething)
                    {
                        base.VisitConversion(node);
                    }
                    return null;
                    void visitConversion(Conversion conversion)
                    {
                        if (conversion.Kind == ConversionKind.MethodGroup)
                        {
                            if (conversion.Method!.MethodKind == MethodKind.LocalFunction)
                            {
                                _mightAssignSomething = true;
                            }
                        }
                        else if (!conversion.UnderlyingConversions.IsDefault)
                        {
                            ImmutableArray<Conversion>.Enumerator enumerator = conversion.UnderlyingConversions.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Conversion current = enumerator.Current;
                                visitConversion(current);
                                if (_mightAssignSomething)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
                {
                    MethodSymbol? methodOpt = node.MethodOpt;
                    if ((object)methodOpt != null && methodOpt!.MethodKind == MethodKind.LocalFunction)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitDelegateCreationExpression(node);
                    }
                    return null;
                }

                public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
                {
                    _mightAssignSomething = true;
                    return null;
                }

                public override BoundNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
                {
                    _mightAssignSomething = true;
                    return null;
                }

                public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
                {
                    _mightAssignSomething = true;
                    return null;
                }

                public override BoundNode VisitDynamicInvocation(BoundDynamicInvocation node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitDynamicInvocation(node);
                    }
                    return null;
                }

                public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitObjectCreationExpression(node);
                    }
                    return null;
                }

                public override BoundNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitDynamicObjectCreationExpression(node);
                    }
                    return null;
                }

                public override BoundNode VisitObjectInitializerMember(BoundObjectInitializerMember node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitObjectInitializerMember(node);
                    }
                    return null;
                }

                public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault || MethodMayMutateReceiver(node.ReceiverOpt, node.Indexer.GetMethod))
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitIndexerAccess(node);
                    }
                    return null;
                }

                public override BoundNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
                {
                    if (!node.ArgumentRefKindsOpt.IsDefault)
                    {
                        _mightAssignSomething = true;
                    }
                    else
                    {
                        base.VisitDynamicIndexerAccess(node);
                    }
                    return null;
                }
            }

            private sealed class CasesComparer : IComparer<(ConstantValue value, LabelSymbol label)>
            {
                private readonly IValueSetFactory _fac;

                public CasesComparer(TypeSymbol type)
                {
                    _fac = ValueSetFactory.ForType(type);
                }

                int IComparer<(ConstantValue value, LabelSymbol label)>.Compare((ConstantValue value, LabelSymbol label) left, (ConstantValue value, LabelSymbol label) right)
                {
                    var x = left.value;
                    var y = right.value;
                    // Sort NaN values into the "highest" position so they fall naturally into the last bucket
                    // when partitioned using less-than.
                    return
                        isNaN(x) ? 1 :
                        isNaN(y) ? -1 :
                        _fac.Related(BinaryOperatorKind.LessThanOrEqual, x, y) ?
                            (_fac.Related(BinaryOperatorKind.LessThanOrEqual, y, x) ? 0 : -1) :
                        1;

                    static bool isNaN(ConstantValue value) =>
                        (value.Discriminator == ConstantValueTypeDiscriminator.Single || value.Discriminator == ConstantValueTypeDiscriminator.Double) &&
                        double.IsNaN(value.DoubleValue);
                }
            }

            private abstract class ValueDispatchNode
            {
                internal sealed class SwitchDispatch : ValueDispatchNode
                {
                    public readonly ImmutableArray<(ConstantValue value, LabelSymbol label)> Cases;

                    public readonly LabelSymbol Otherwise;

                    public SwitchDispatch(SyntaxNode syntax, ImmutableArray<(ConstantValue value, LabelSymbol label)> dispatches, LabelSymbol otherwise)
                        : base(syntax)
                    {
                        Cases = dispatches;
                        Otherwise = otherwise;
                    }

                    public override string ToString()
                    {
                        return "[" + string.Join(",", Cases.Select(((ConstantValue value, LabelSymbol label) c) => c.value)) + "]";
                    }
                }

                internal sealed class LeafDispatchNode : ValueDispatchNode
                {
                    public readonly LabelSymbol Label;

                    public LeafDispatchNode(SyntaxNode syntax, LabelSymbol Label)
                        : base(syntax)
                    {
                        this.Label = Label;
                    }

                    public override string ToString()
                    {
                        return "Leaf";
                    }
                }

                internal sealed class RelationalDispatch : ValueDispatchNode
                {
                    private int _height;

                    public readonly ConstantValue Value;

                    public readonly BinaryOperatorKind Operator;

                    protected override int Height => _height;

                    private ValueDispatchNode Left { get; set; }

                    private ValueDispatchNode Right { get; set; }

                    public ValueDispatchNode WhenTrue
                    {
                        get
                        {
                            if (!IsReversed(Operator))
                            {
                                return Left;
                            }
                            return Right;
                        }
                    }

                    public ValueDispatchNode WhenFalse
                    {
                        get
                        {
                            if (!IsReversed(Operator))
                            {
                                return Right;
                            }
                            return Left;
                        }
                    }

                    private RelationalDispatch(SyntaxNode syntax, ConstantValue value, BinaryOperatorKind op, ValueDispatchNode left, ValueDispatchNode right)
                        : base(syntax)
                    {
                        Value = value;
                        Operator = op;
                        WithLeftAndRight(left, right);
                    }

                    public override string ToString()
                    {
                        return $"RelationalDispatch.{Height}({Left} {Operator.Operator()} {Value} {Right})";
                    }

                    private static bool IsReversed(BinaryOperatorKind op)
                    {
                        return op.Operator() switch
                        {
                            BinaryOperatorKind.GreaterThan => true,
                            BinaryOperatorKind.GreaterThanOrEqual => true,
                            _ => false,
                        };
                    }

                    private RelationalDispatch WithLeftAndRight(ValueDispatchNode left, ValueDispatchNode right)
                    {
                        int height = left.Height;
                        int height2 = right.Height;
                        Left = left;
                        Right = right;
                        _height = Math.Max(height, height2) + 1;
                        return this;
                    }

                    public RelationalDispatch WithTrueAndFalseChildren(ValueDispatchNode whenTrue, ValueDispatchNode whenFalse)
                    {
                        if (whenTrue == WhenTrue && whenFalse == WhenFalse)
                        {
                            return this;
                        }
                        ValueDispatchNode left;
                        ValueDispatchNode right;
                        if (!IsReversed(Operator))
                        {
                            ValueDispatchNode valueDispatchNode = whenFalse;
                            left = whenTrue;
                            right = valueDispatchNode;
                        }
                        else
                        {
                            ValueDispatchNode valueDispatchNode = whenTrue;
                            left = whenFalse;
                            right = valueDispatchNode;
                        }
                        return WithLeftAndRight(left, right);
                    }

                    public static ValueDispatchNode CreateBalanced(SyntaxNode syntax, ConstantValue value, BinaryOperatorKind op, ValueDispatchNode whenTrue, ValueDispatchNode whenFalse)
                    {
                        ValueDispatchNode left;
                        ValueDispatchNode right;
                        if (!IsReversed(op))
                        {
                            ValueDispatchNode valueDispatchNode = whenFalse;
                            left = whenTrue;
                            right = valueDispatchNode;
                        }
                        else
                        {
                            ValueDispatchNode valueDispatchNode = whenTrue;
                            left = whenFalse;
                            right = valueDispatchNode;
                        }
                        return CreateBalancedCore(syntax, value, op, left, right);
                    }

                    private static ValueDispatchNode CreateBalancedCore(SyntaxNode syntax, ConstantValue value, BinaryOperatorKind op, ValueDispatchNode left, ValueDispatchNode right)
                    {
                        if (left.Height > right.Height + 1)
                        {
                            RelationalDispatch relationalDispatch = (RelationalDispatch)left;
                            ValueDispatchNode valueDispatchNode = CreateBalancedCore(syntax, value, op, relationalDispatch.Right, right);
                            SyntaxNode syntax2 = relationalDispatch.Syntax;
                            ConstantValue value2 = relationalDispatch.Value;
                            BinaryOperatorKind @operator = relationalDispatch.Operator;
                            ValueDispatchNode left2 = relationalDispatch.Left;
                            ValueDispatchNode valueDispatchNode2 = valueDispatchNode;
                            syntax = syntax2;
                            value = value2;
                            op = @operator;
                            left = left2;
                            right = valueDispatchNode2;
                        }
                        else if (right.Height > left.Height + 1)
                        {
                            RelationalDispatch relationalDispatch2 = (RelationalDispatch)right;
                            ValueDispatchNode valueDispatchNode3 = CreateBalancedCore(syntax, value, op, left, relationalDispatch2.Left);
                            SyntaxNode syntax2 = relationalDispatch2.Syntax;
                            ConstantValue value2 = relationalDispatch2.Value;
                            BinaryOperatorKind @operator = relationalDispatch2.Operator;
                            ValueDispatchNode valueDispatchNode2 = valueDispatchNode3;
                            ValueDispatchNode right2 = relationalDispatch2.Right;
                            syntax = syntax2;
                            value = value2;
                            op = @operator;
                            left = valueDispatchNode2;
                            right = right2;
                        }
                        if (left.Height == right.Height + 2)
                        {
                            RelationalDispatch relationalDispatch3 = (RelationalDispatch)left;
                            if (relationalDispatch3.Left.Height == right.Height)
                            {
                                RelationalDispatch relationalDispatch4 = relationalDispatch3;
                                ValueDispatchNode left3 = relationalDispatch4.Left;
                                RelationalDispatch obj = (RelationalDispatch)relationalDispatch4.Right;
                                ValueDispatchNode left4 = obj.Left;
                                ValueDispatchNode right3 = obj.Right;
                                ValueDispatchNode right4 = right;
                                return obj.WithLeftAndRight(relationalDispatch4.WithLeftAndRight(left3, left4), new RelationalDispatch(syntax, value, op, right3, right4));
                            }
                            ValueDispatchNode left5 = relationalDispatch3.Left;
                            ValueDispatchNode right5 = relationalDispatch3.Right;
                            ValueDispatchNode right6 = right;
                            return relationalDispatch3.WithLeftAndRight(left5, new RelationalDispatch(syntax, value, op, right5, right6));
                        }
                        if (right.Height == left.Height + 2)
                        {
                            RelationalDispatch relationalDispatch5 = (RelationalDispatch)right;
                            if (relationalDispatch5.Right.Height == left.Height)
                            {
                                ValueDispatchNode left6 = left;
                                RelationalDispatch relationalDispatch6 = relationalDispatch5;
                                RelationalDispatch obj2 = (RelationalDispatch)relationalDispatch6.Left;
                                ValueDispatchNode left7 = obj2.Left;
                                ValueDispatchNode right7 = obj2.Right;
                                return obj2.WithLeftAndRight(right: relationalDispatch6.WithLeftAndRight(right7, relationalDispatch6.Right), left: new RelationalDispatch(syntax, value, op, left6, left7));
                            }
                            ValueDispatchNode left8 = left;
                            ValueDispatchNode left9 = relationalDispatch5.Left;
                            return relationalDispatch5.WithLeftAndRight(right: relationalDispatch5.Right, left: new RelationalDispatch(syntax, value, op, left8, left9));
                        }
                        return new RelationalDispatch(syntax, value, op, left, right);
                    }
                }

                public readonly SyntaxNode Syntax;

                protected virtual int Height => 1;

                public ValueDispatchNode(SyntaxNode syntax)
                {
                    Syntax = syntax;
                }
            }

            private ArrayBuilder<BoundStatement> _loweredDecisionDag;

            protected readonly PooledDictionary<BoundDecisionDagNode, LabelSymbol> _dagNodeLabels = PooledDictionary<BoundDecisionDagNode, LabelSymbol>.GetInstance();

            protected abstract ArrayBuilder<BoundStatement> BuilderForSection(SyntaxNode section);

            protected DecisionDagRewriter(SyntaxNode node, LocalRewriter localRewriter, bool generateInstrumentation)
                : base(node, localRewriter, generateInstrumentation)
            {
            }

            private void ComputeLabelSet(BoundDecisionDag decisionDag)
            {
                PooledHashSet<BoundDecisionDagNode> hasPredecessor = PooledHashSet<BoundDecisionDagNode>.GetInstance();
                ImmutableArray<BoundDecisionDagNode>.Enumerator enumerator = decisionDag.TopologicallySortedNodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundDecisionDagNode current = enumerator.Current;
                    if (!(current is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        if (!(current is BoundLeafDecisionDagNode boundLeafDecisionDagNode))
                        {
                            if (!(current is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                            {
                                if (!(current is BoundTestDecisionDagNode boundTestDecisionDagNode))
                                {
                                    throw ExceptionUtilities.UnexpectedValue(current.Kind);
                                }
                                notePredecessor(boundTestDecisionDagNode.WhenTrue);
                                notePredecessor(boundTestDecisionDagNode.WhenFalse);
                            }
                            else
                            {
                                notePredecessor(boundEvaluationDecisionDagNode.Next);
                            }
                        }
                        else
                        {
                            _dagNodeLabels[current] = boundLeafDecisionDagNode.Label;
                        }
                    }
                    else
                    {
                        GetDagNodeLabel(current);
                        if (boundWhenDecisionDagNode.WhenFalse != null)
                        {
                            GetDagNodeLabel(boundWhenDecisionDagNode.WhenFalse);
                        }
                    }
                }
                hasPredecessor.Free();
                void notePredecessor(BoundDecisionDagNode successor)
                {
                    if (successor != null && !hasPredecessor.Add(successor))
                    {
                        GetDagNodeLabel(successor);
                    }
                }
            }

            protected new void Free()
            {
                _dagNodeLabels.Free();
                base.Free();
            }

            protected virtual LabelSymbol GetDagNodeLabel(BoundDecisionDagNode dag)
            {
                if (!_dagNodeLabels.TryGetValue(dag, out var value))
                {
                    _dagNodeLabels.Add(dag, value = ((dag is BoundLeafDecisionDagNode boundLeafDecisionDagNode) ? boundLeafDecisionDagNode.Label : _factory.GenerateLabel("dagNode")));
                }
                return value;
            }

            protected BoundDecisionDag ShareTempsIfPossibleAndEvaluateInput(BoundDecisionDag decisionDag, BoundExpression loweredSwitchGoverningExpression, ArrayBuilder<BoundStatement> result, out BoundExpression savedInputExpression)
            {
                WhenClauseMightAssignPatternVariableWalker mightAssignWalker = new WhenClauseMightAssignPatternVariableWalker();
                if (!decisionDag.TopologicallySortedNodes.Any((BoundDecisionDagNode node) => node is BoundWhenDecisionDagNode boundWhenDecisionDagNode && mightAssignWalker.MightAssignSomething(boundWhenDecisionDagNode.WhenExpression)))
                {
                    decisionDag = ShareTempsAndEvaluateInput(loweredSwitchGoverningExpression, decisionDag, delegate (BoundExpression expr)
                    {
                        result.Add(_factory.ExpressionStatement(expr));
                    }, out savedInputExpression);
                }
                else
                {
                    BoundExpression temp = _tempAllocator.GetTemp(BoundDagTemp.ForOriginalInput(loweredSwitchGoverningExpression));
                    result.Add(_factory.Assignment(temp, loweredSwitchGoverningExpression));
                    savedInputExpression = temp;
                }
                return decisionDag;
            }

            protected ImmutableArray<BoundStatement> LowerDecisionDagCore(BoundDecisionDag decisionDag)
            {
                _loweredDecisionDag = ArrayBuilder<BoundStatement>.GetInstance();
                ComputeLabelSet(decisionDag);
                ImmutableArray<BoundDecisionDagNode> topologicallySortedNodes = decisionDag.TopologicallySortedNodes;
                BoundDecisionDagNode boundDecisionDagNode = topologicallySortedNodes[0];
                if (boundDecisionDagNode is BoundWhenDecisionDagNode || boundDecisionDagNode is BoundLeafDecisionDagNode)
                {
                    _loweredDecisionDag.Add(_factory.Goto(GetDagNodeLabel(boundDecisionDagNode)));
                }
                ImmutableArray<BoundDecisionDagNode>.Enumerator enumerator = topologicallySortedNodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is BoundWhenDecisionDagNode whenClause)
                    {
                        LowerWhenClause(whenClause);
                    }
                }
                ImmutableArray<BoundDecisionDagNode> nodesToLower = topologicallySortedNodes.WhereAsArray((BoundDecisionDagNode n) => n.Kind != BoundKind.WhenDecisionDagNode && n.Kind != BoundKind.LeafDecisionDagNode);
                PooledHashSet<BoundDecisionDagNode> instance = PooledHashSet<BoundDecisionDagNode>.GetInstance();
                int i = 0;
                for (int length = nodesToLower.Length; i < length; i++)
                {
                    BoundDecisionDagNode boundDecisionDagNode2 = nodesToLower[i];
                    bool flag = instance.Contains(boundDecisionDagNode2);
                    if (flag && !_dagNodeLabels.TryGetValue(boundDecisionDagNode2, out var _))
                    {
                        continue;
                    }
                    if (_dagNodeLabels.TryGetValue(boundDecisionDagNode2, out var value2))
                    {
                        _loweredDecisionDag.Add(_factory.Label(value2));
                    }
                    if ((flag || !GenerateSwitchDispatch(boundDecisionDagNode2, instance)) && !GenerateTypeTestAndCast(boundDecisionDagNode2, instance, nodesToLower, i))
                    {
                        BoundDecisionDagNode boundDecisionDagNode3 = ((i + 1 < length) ? nodesToLower[i + 1] : null);
                        if (boundDecisionDagNode3 != null && instance.Contains(boundDecisionDagNode3))
                        {
                            boundDecisionDagNode3 = null;
                        }
                        LowerDecisionDagNode(boundDecisionDagNode2, boundDecisionDagNode3);
                    }
                }
                instance.Free();
                ImmutableArray<BoundStatement> result = _loweredDecisionDag.ToImmutableAndFree();
                _loweredDecisionDag = null;
                return result;
            }

            private bool GenerateTypeTestAndCast(BoundDecisionDagNode node, HashSet<BoundDecisionDagNode> loweredNodes, ImmutableArray<BoundDecisionDagNode> nodesToLower, int indexOfNode)
            {
                if (node is BoundTestDecisionDagNode boundTestDecisionDagNode && boundTestDecisionDagNode.WhenTrue is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode && TryLowerTypeTestAndCast(boundTestDecisionDagNode.Test, boundEvaluationDecisionDagNode.Evaluation, out var sideEffect, out var testExpression))
                {
                    BoundDecisionDagNode next = boundEvaluationDecisionDagNode.Next;
                    BoundDecisionDagNode whenFalse = boundTestDecisionDagNode.WhenFalse;
                    bool flag = !_dagNodeLabels.ContainsKey(boundEvaluationDecisionDagNode);
                    if (flag)
                    {
                        loweredNodes.Add(boundEvaluationDecisionDagNode);
                    }
                    BoundDecisionDagNode nextNode = ((indexOfNode + 2 < nodesToLower.Length && flag && nodesToLower[indexOfNode + 1] == boundEvaluationDecisionDagNode && !loweredNodes.Contains(nodesToLower[indexOfNode + 2])) ? nodesToLower[indexOfNode + 2] : null);
                    _loweredDecisionDag.Add(_factory.ExpressionStatement(sideEffect));
                    GenerateTest(testExpression, next, whenFalse, nextNode);
                    return true;
                }
                return false;
            }

            private void GenerateTest(BoundExpression test, BoundDecisionDagNode whenTrue, BoundDecisionDagNode whenFalse, BoundDecisionDagNode nextNode)
            {
                _factory.Syntax = test.Syntax;
                if (nextNode == whenFalse)
                {
                    _loweredDecisionDag.Add(_factory.ConditionalGoto(test, GetDagNodeLabel(whenTrue), jumpIfTrue: true));
                    return;
                }
                if (nextNode == whenTrue)
                {
                    _loweredDecisionDag.Add(_factory.ConditionalGoto(test, GetDagNodeLabel(whenFalse), jumpIfTrue: false));
                    return;
                }
                _loweredDecisionDag.Add(_factory.ConditionalGoto(test, GetDagNodeLabel(whenTrue), jumpIfTrue: true));
                _loweredDecisionDag.Add(_factory.Goto(GetDagNodeLabel(whenFalse)));
            }

            private bool GenerateSwitchDispatch(BoundDecisionDagNode node, HashSet<BoundDecisionDagNode> loweredNodes)
            {
                if (!canGenerateSwitchDispatch(node))
                {
                    return false;
                }
                BoundDagTemp input = ((BoundTestDecisionDagNode)node).Test.Input;
                ValueDispatchNode n = GatherValueDispatchNodes(node, loweredNodes, input);
                LowerValueDispatchNode(n, _tempAllocator.GetTemp(input));
                return true;
                bool canDispatch(BoundTestDecisionDagNode test1, BoundTestDecisionDagNode test2)
                {
                    if (_dagNodeLabels.ContainsKey(test2))
                    {
                        return false;
                    }
                    BoundDagTest test3 = test1.Test;
                    BoundDagTest test4 = test2.Test;
                    if (!(test3 is BoundDagValueTest) && !(test3 is BoundDagRelationalTest))
                    {
                        return false;
                    }
                    if (!(test4 is BoundDagValueTest) && !(test4 is BoundDagRelationalTest))
                    {
                        return false;
                    }
                    if (!test3.Input.Equals(test4.Input))
                    {
                        return false;
                    }
                    return true;
                }
                bool canGenerateSwitchDispatch(BoundDecisionDagNode node)
                {
                    if (node is BoundTestDecisionDagNode boundTestDecisionDagNode)
                    {
                        if (boundTestDecisionDagNode.WhenFalse is BoundTestDecisionDagNode test5)
                        {
                            return canDispatch(boundTestDecisionDagNode, test5);
                        }
                        if (boundTestDecisionDagNode.WhenTrue is BoundTestDecisionDagNode test6)
                        {
                            BoundTestDecisionDagNode test7 = boundTestDecisionDagNode;
                            return canDispatch(test7, test6);
                        }
                    }
                    return false;
                }
            }

            private ValueDispatchNode GatherValueDispatchNodes(BoundDecisionDagNode node, HashSet<BoundDecisionDagNode> loweredNodes, BoundDagTemp input)
            {
                IValueSetFactory fac = ValueSetFactory.ForType(input.Type);
                return GatherValueDispatchNodes(node, loweredNodes, input, fac);
            }

            private ValueDispatchNode GatherValueDispatchNodes(BoundDecisionDagNode node, HashSet<BoundDecisionDagNode> loweredNodes, BoundDagTemp input, IValueSetFactory fac)
            {
                if (loweredNodes.Contains(node))
                {
                    _dagNodeLabels.TryGetValue(node, out var value);
                    return new ValueDispatchNode.LeafDispatchNode(node.Syntax, value);
                }
                if (!(node is BoundTestDecisionDagNode boundTestDecisionDagNode) || !boundTestDecisionDagNode.Test.Input.Equals(input))
                {
                    LabelSymbol dagNodeLabel = GetDagNodeLabel(node);
                    return new ValueDispatchNode.LeafDispatchNode(node.Syntax, dagNodeLabel);
                }
                BoundDagTest test = boundTestDecisionDagNode.Test;
                if (!(test is BoundDagRelationalTest boundDagRelationalTest))
                {
                    if (test is BoundDagValueTest boundDagValueTest)
                    {
                        loweredNodes.Add(boundTestDecisionDagNode);
                        ArrayBuilder<(ConstantValue, LabelSymbol)> instance = ArrayBuilder<(ConstantValue, LabelSymbol)>.GetInstance();
                        instance.Add((boundDagValueTest.Value, GetDagNodeLabel(boundTestDecisionDagNode.WhenTrue)));
                        BoundTestDecisionDagNode boundTestDecisionDagNode2 = boundTestDecisionDagNode;
                        while (boundTestDecisionDagNode2.WhenFalse is BoundTestDecisionDagNode boundTestDecisionDagNode3 && boundTestDecisionDagNode3.Test is BoundDagValueTest boundDagValueTest2 && boundDagValueTest2.Input.Equals(input) && !_dagNodeLabels.ContainsKey(boundTestDecisionDagNode3) && !loweredNodes.Contains(boundTestDecisionDagNode3))
                        {
                            instance.Add((boundDagValueTest2.Value, GetDagNodeLabel(boundTestDecisionDagNode3.WhenTrue)));
                            loweredNodes.Add(boundTestDecisionDagNode3);
                            boundTestDecisionDagNode2 = boundTestDecisionDagNode3;
                        }
                        ValueDispatchNode otherwise = GatherValueDispatchNodes(boundTestDecisionDagNode2.WhenFalse, loweredNodes, input, fac);
                        return PushEqualityTestsIntoTree(boundDagValueTest.Syntax, otherwise, instance.ToImmutableAndFree(), fac);
                    }
                    LabelSymbol dagNodeLabel2 = GetDagNodeLabel(node);
                    return new ValueDispatchNode.LeafDispatchNode(node.Syntax, dagNodeLabel2);
                }
                loweredNodes.Add(boundTestDecisionDagNode);
                ValueDispatchNode whenTrue = GatherValueDispatchNodes(boundTestDecisionDagNode.WhenTrue, loweredNodes, input, fac);
                ValueDispatchNode whenFalse = GatherValueDispatchNodes(boundTestDecisionDagNode.WhenFalse, loweredNodes, input, fac);
                return ValueDispatchNode.RelationalDispatch.CreateBalanced(boundTestDecisionDagNode.Syntax, boundDagRelationalTest.Value, boundDagRelationalTest.OperatorKind, whenTrue, whenFalse);
            }

            private ValueDispatchNode PushEqualityTestsIntoTree(SyntaxNode syntax, ValueDispatchNode otherwise, ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, IValueSetFactory fac)
            {
                if (cases.IsEmpty)
                {
                    return otherwise;
                }
                if (!(otherwise is ValueDispatchNode.LeafDispatchNode leafDispatchNode))
                {
                    if (!(otherwise is ValueDispatchNode.SwitchDispatch switchDispatch))
                    {
                        if (otherwise is ValueDispatchNode.RelationalDispatch relationalDispatch)
                        {
                            BinaryOperatorKind @operator = relationalDispatch.Operator;
                            ConstantValue value2 = relationalDispatch.Value;
                            ValueDispatchNode whenTrue = relationalDispatch.WhenTrue;
                            ValueDispatchNode whenFalse = relationalDispatch.WhenFalse;
                            (ImmutableArray<(ConstantValue value, LabelSymbol label)> whenTrueCases, ImmutableArray<(ConstantValue value, LabelSymbol label)> whenFalseCases) tuple = splitCases(cases, @operator, value2);
                            ImmutableArray<(ConstantValue, LabelSymbol)> item = tuple.whenTrueCases;
                            ImmutableArray<(ConstantValue, LabelSymbol)> item2 = tuple.whenFalseCases;
                            whenTrue = PushEqualityTestsIntoTree(syntax, whenTrue, item, fac);
                            whenFalse = PushEqualityTestsIntoTree(syntax, whenFalse, item2, fac);
                            return relationalDispatch.WithTrueAndFalseChildren(whenTrue, whenFalse);
                        }
                        throw ExceptionUtilities.UnexpectedValue(otherwise);
                    }
                    return new ValueDispatchNode.SwitchDispatch(switchDispatch.Syntax, switchDispatch.Cases.Concat(cases), switchDispatch.Otherwise);
                }
                return new ValueDispatchNode.SwitchDispatch(syntax, cases, leafDispatchNode.Label);
                (ImmutableArray<(ConstantValue value, LabelSymbol label)> whenTrueCases, ImmutableArray<(ConstantValue value, LabelSymbol label)> whenFalseCases) splitCases(ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, BinaryOperatorKind op, ConstantValue value)
                {
                    ArrayBuilder<(ConstantValue, LabelSymbol)> instance = ArrayBuilder<(ConstantValue, LabelSymbol)>.GetInstance();
                    ArrayBuilder<(ConstantValue, LabelSymbol)> instance2 = ArrayBuilder<(ConstantValue, LabelSymbol)>.GetInstance();
                    op = op.Operator();
                    ImmutableArray<(ConstantValue, LabelSymbol)>.Enumerator enumerator = cases.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        (ConstantValue, LabelSymbol) current = enumerator.Current;
                        (fac.Related(op, current.Item1, value) ? instance : instance2).Add(current);
                    }
                    return (instance.ToImmutableAndFree(), instance2.ToImmutableAndFree());
                }
            }

            private void LowerValueDispatchNode(ValueDispatchNode n, BoundExpression input)
            {
                if (!(n is ValueDispatchNode.LeafDispatchNode leafDispatchNode))
                {
                    if (!(n is ValueDispatchNode.SwitchDispatch node))
                    {
                        if (!(n is ValueDispatchNode.RelationalDispatch rel))
                        {
                            throw ExceptionUtilities.UnexpectedValue(n);
                        }
                        LowerRelationalDispatchNode(rel, input);
                    }
                    else
                    {
                        LowerSwitchDispatchNode(node, input);
                    }
                }
                else
                {
                    _loweredDecisionDag.Add(_factory.Goto(leafDispatchNode.Label));
                }
            }

            private void LowerRelationalDispatchNode(ValueDispatchNode.RelationalDispatch rel, BoundExpression input)
            {
                BoundExpression condition = MakeRelationalTest(rel.Syntax, input, rel.Operator, rel.Value);
                if (rel.WhenTrue is ValueDispatchNode.LeafDispatchNode leafDispatchNode)
                {
                    LabelSymbol label = leafDispatchNode.Label;
                    _loweredDecisionDag.Add(_factory.ConditionalGoto(condition, label, jumpIfTrue: true));
                    LowerValueDispatchNode(rel.WhenFalse, input);
                }
                else if (rel.WhenFalse is ValueDispatchNode.LeafDispatchNode leafDispatchNode2)
                {
                    LabelSymbol label2 = leafDispatchNode2.Label;
                    _loweredDecisionDag.Add(_factory.ConditionalGoto(condition, label2, jumpIfTrue: false));
                    LowerValueDispatchNode(rel.WhenTrue, input);
                }
                else
                {
                    LabelSymbol label3 = _factory.GenerateLabel("relationalDispatch");
                    _loweredDecisionDag.Add(_factory.ConditionalGoto(condition, label3, jumpIfTrue: false));
                    LowerValueDispatchNode(rel.WhenTrue, input);
                    _loweredDecisionDag.Add(_factory.Label(label3));
                    LowerValueDispatchNode(rel.WhenFalse, input);
                }
            }

            private void LowerSwitchDispatchNode(ValueDispatchNode.SwitchDispatch node, BoundExpression input)
            {
                LabelSymbol defaultLabel = node.Otherwise;
                BinaryOperatorKind lessThanOrEqualOperator;
                ImmutableArray<(ConstantValue value, LabelSymbol label)> cases;
                if (input.Type.IsValidV6SwitchGoverningType())
                {
                    MethodSymbol equalityMethod = null;
                    if (input.Type!.SpecialType == SpecialType.System_String)
                    {
                        EnsureStringHashFunction(node.Cases.Length, node.Syntax);
                        equalityMethod = _localRewriter.UnsafeGetSpecialTypeMethod(node.Syntax, SpecialMember.System_String__op_Equality);
                    }
                    BoundSwitchDispatch item = new BoundSwitchDispatch(node.Syntax, input, node.Cases, defaultLabel, equalityMethod);
                    _loweredDecisionDag.Add(item);
                }
                else if (input.Type!.IsNativeIntegerType)
                {
                    ImmutableArray<(ConstantValue, LabelSymbol)> cases2;
                    switch (input.Type!.SpecialType)
                    {
                        case SpecialType.System_IntPtr:
                            input = _factory.Convert(_factory.SpecialType(SpecialType.System_Int64), input);
                            cases2 = node.Cases.SelectAsArray(((ConstantValue value, LabelSymbol label) p) => (ConstantValue.Create((long)p.value.Int32Value), p.label));
                            break;
                        case SpecialType.System_UIntPtr:
                            input = _factory.Convert(_factory.SpecialType(SpecialType.System_UInt64), input);
                            cases2 = node.Cases.SelectAsArray(((ConstantValue value, LabelSymbol label) p) => (ConstantValue.Create((ulong)p.value.UInt32Value), p.label));
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(input.Type);
                    }
                    BoundSwitchDispatch item2 = new BoundSwitchDispatch(node.Syntax, input, cases2, defaultLabel, null);
                    _loweredDecisionDag.Add(item2);
                }
                else
                {
                    lessThanOrEqualOperator = input.Type!.SpecialType switch
                    {
                        SpecialType.System_Single => BinaryOperatorKind.FloatLessThanOrEqual,
                        SpecialType.System_Double => BinaryOperatorKind.DoubleLessThanOrEqual,
                        SpecialType.System_Decimal => BinaryOperatorKind.DecimalLessThanOrEqual,
                        _ => throw ExceptionUtilities.UnexpectedValue(input.Type!.SpecialType),
                    };
                    cases = node.Cases.Sort(new CasesComparer(input.Type));
                    lowerFloatDispatch(0, cases.Length);
                }
                void lowerFloatDispatch(int firstIndex, int count)
                {
                    if (count <= 3)
                    {
                        int i = firstIndex;
                        for (int num = firstIndex + count; i < num; i++)
                        {
                            _loweredDecisionDag.Add(_factory.ConditionalGoto(MakeValueTest(node.Syntax, input, cases[i].value), cases[i].label, jumpIfTrue: true));
                        }
                        _loweredDecisionDag.Add(_factory.Goto(defaultLabel));
                    }
                    else
                    {
                        int num2 = count / 2;
                        GeneratedLabelSymbol label = _factory.GenerateLabel("greaterThanMidpoint");
                        _loweredDecisionDag.Add(_factory.ConditionalGoto(MakeRelationalTest(node.Syntax, input, lessThanOrEqualOperator, cases[firstIndex + num2 - 1].value), label, jumpIfTrue: false));
                        lowerFloatDispatch(firstIndex, num2);
                        _loweredDecisionDag.Add(_factory.Label(label));
                        lowerFloatDispatch(firstIndex + num2, count - num2);
                    }
                }
            }

            private void EnsureStringHashFunction(int labelsCount, SyntaxNode syntaxNode)
            {
                PEModuleBuilder emitModule = _localRewriter.EmitModule;
                if (emitModule == null || !SwitchStringJumpTableEmitter.ShouldGenerateHashTableSwitch(emitModule, labelsCount))
                {
                    return;
                }
                PrivateImplementationDetails privateImplClass = emitModule.GetPrivateImplClass(syntaxNode, _localRewriter._diagnostics.DiagnosticBag);
                if (privateImplClass.GetMethod("ComputeStringHash") == null)
                {
                    Symbol specialTypeMember = _localRewriter._compilation.GetSpecialTypeMember(SpecialMember.System_String__Chars);
                    if ((object)specialTypeMember != null && !specialTypeMember.HasUseSiteError)
                    {
                        TypeSymbol returnType = _factory.SpecialType(SpecialType.System_UInt32);
                        TypeSymbol paramType = _factory.SpecialType(SpecialType.System_String);
                        SynthesizedStringSwitchHashMethod synthesizedStringSwitchHashMethod = new SynthesizedStringSwitchHashMethod(emitModule.SourceModule, privateImplClass, returnType, paramType);
                        privateImplClass.TryAddSynthesizedMethod(synthesizedStringSwitchHashMethod.GetCciAdapter());
                    }
                }
            }

            private void LowerWhenClause(BoundWhenDecisionDagNode whenClause)
            {
                BoundLeafDecisionDagNode dag = (BoundLeafDecisionDagNode)whenClause.WhenTrue;
                LabelSymbol dagNodeLabel = GetDagNodeLabel(whenClause);
                ArrayBuilder<BoundStatement> arrayBuilder = BuilderForSection(whenClause.Syntax);
                arrayBuilder.Add(_factory.Label(dagNodeLabel));
                ImmutableArray<BoundPatternBinding>.Enumerator enumerator = whenClause.Bindings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundPatternBinding current = enumerator.Current;
                    BoundExpression boundExpression = _localRewriter.VisitExpression(current.VariableAccess);
                    BoundExpression temp = _tempAllocator.GetTemp(current.TempContainingValue);
                    if (boundExpression != temp)
                    {
                        arrayBuilder.Add(_factory.Assignment(boundExpression, temp));
                    }
                }
                BoundDecisionDagNode whenFalse = whenClause.WhenFalse;
                LabelSymbol dagNodeLabel2 = GetDagNodeLabel(dag);
                if (whenClause.WhenExpression != null && whenClause.WhenExpression!.ConstantValue != ConstantValue.True)
                {
                    _factory.Syntax = whenClause.Syntax;
                    BoundStatement boundStatement = _factory.ConditionalGoto(_localRewriter.VisitExpression(whenClause.WhenExpression), dagNodeLabel2, jumpIfTrue: true);
                    if (base.GenerateInstrumentation && !whenClause.WhenExpression!.WasCompilerGenerated)
                    {
                        boundStatement = _localRewriter._instrumenter.InstrumentSwitchWhenClauseConditionalGotoBody(whenClause.WhenExpression, boundStatement);
                    }
                    arrayBuilder.Add(boundStatement);
                    BoundStatement boundStatement2 = _factory.Goto(GetDagNodeLabel(whenFalse));
                    arrayBuilder.Add(base.GenerateInstrumentation ? _factory.HiddenSequencePoint(boundStatement2) : boundStatement2);
                }
                else
                {
                    arrayBuilder.Add(_factory.Goto(dagNodeLabel2));
                }
            }

            private void LowerDecisionDagNode(BoundDecisionDagNode node, BoundDecisionDagNode nextNode)
            {
                _factory.Syntax = node.Syntax;
                if (!(node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                {
                    if (node is BoundTestDecisionDagNode boundTestDecisionDagNode)
                    {
                        BoundExpression test = LowerTest(boundTestDecisionDagNode.Test);
                        GenerateTest(test, boundTestDecisionDagNode.WhenTrue, boundTestDecisionDagNode.WhenFalse, nextNode);
                        return;
                    }
                    throw ExceptionUtilities.UnexpectedValue(node.Kind);
                }
                BoundExpression expr = LowerEvaluation(boundEvaluationDecisionDagNode.Evaluation);
                _loweredDecisionDag.Add(_factory.ExpressionStatement(expr));
                if (base.GenerateInstrumentation)
                {
                    _loweredDecisionDag.Add(_factory.HiddenSequencePoint());
                }
                if (nextNode != boundEvaluationDecisionDagNode.Next)
                {
                    _loweredDecisionDag.Add(_factory.Goto(GetDagNodeLabel(boundEvaluationDecisionDagNode.Next)));
                }
            }
        }

        private abstract class PatternLocalRewriter
        {
            public sealed class DagTempAllocator
            {
                private readonly SyntheticBoundNodeFactory _factory;

                private readonly PooledDictionary<BoundDagTemp, BoundExpression> _map = PooledDictionary<BoundDagTemp, BoundExpression>.GetInstance();

                private readonly ArrayBuilder<LocalSymbol> _temps = ArrayBuilder<LocalSymbol>.GetInstance();

                private readonly SyntaxNode _node;

                private readonly bool _generateSequencePoints;

                public DagTempAllocator(SyntheticBoundNodeFactory factory, SyntaxNode node, bool generateSequencePoints)
                {
                    _factory = factory;
                    _node = node;
                    _generateSequencePoints = generateSequencePoints;
                }

                public void Free()
                {
                    _temps.Free();
                    _map.Free();
                }

                public BoundExpression GetTemp(BoundDagTemp dagTemp)
                {
                    if (!_map.TryGetValue(dagTemp, out var value))
                    {
                        SynthesizedLocalKind kind = (_generateSequencePoints ? SynthesizedLocalKind.SwitchCasePatternMatching : SynthesizedLocalKind.LoweringTemp);
                        LocalSymbol localSymbol = _factory.SynthesizedLocal(dagTemp.Type, _node, isPinned: false, RefKind.None, kind);
                        value = _factory.Local(localSymbol);
                        _map.Add(dagTemp, value);
                        _temps.Add(localSymbol);
                    }
                    return value;
                }

                public bool TrySetTemp(BoundDagTemp dagTemp, BoundExpression translation)
                {
                    if (!_map.ContainsKey(dagTemp))
                    {
                        _map.Add(dagTemp, translation);
                        return true;
                    }
                    return false;
                }

                public ImmutableArray<LocalSymbol> AllTemps()
                {
                    return _temps.ToImmutableArray();
                }
            }

            protected readonly LocalRewriter _localRewriter;

            protected readonly SyntheticBoundNodeFactory _factory;

            protected readonly DagTempAllocator _tempAllocator;

            protected bool GenerateInstrumentation { get; }

            public PatternLocalRewriter(SyntaxNode node, LocalRewriter localRewriter, bool generateInstrumentation)
            {
                _localRewriter = localRewriter;
                _factory = localRewriter._factory;
                GenerateInstrumentation = generateInstrumentation;
                _tempAllocator = new DagTempAllocator(_factory, node, generateInstrumentation);
            }

            public void Free()
            {
                _tempAllocator.Free();
            }

            protected BoundExpression LowerEvaluation(BoundDagEvaluation evaluation)
            {
                BoundExpression boundExpression = _tempAllocator.GetTemp(evaluation.Input);
                ArrayBuilder<RefKind> refKindBuilder;
                ArrayBuilder<BoundExpression> argBuilder;
                if (!(evaluation is BoundDagFieldEvaluation boundDagFieldEvaluation))
                {
                    if (!(evaluation is BoundDagPropertyEvaluation boundDagPropertyEvaluation))
                    {
                        if (!(evaluation is BoundDagDeconstructEvaluation boundDagDeconstructEvaluation))
                        {
                            if (!(evaluation is BoundDagTypeEvaluation boundDagTypeEvaluation))
                            {
                                if (evaluation is BoundDagIndexEvaluation boundDagIndexEvaluation)
                                {
                                    TypeSymbol returnType = boundDagIndexEvaluation.Property.GetMethod.ReturnType;
                                    BoundDagTemp dagTemp = new BoundDagTemp(boundDagIndexEvaluation.Syntax, returnType, boundDagIndexEvaluation);
                                    BoundExpression temp = _tempAllocator.GetTemp(dagTemp);
                                    return _factory.AssignmentExpression(temp, _factory.Call(boundExpression, boundDagIndexEvaluation.Property.GetMethod, _factory.Literal(boundDagIndexEvaluation.Index)));
                                }
                                throw ExceptionUtilities.UnexpectedValue(evaluation);
                            }
                            TypeSymbol typeSymbol = boundExpression.Type;
                            if (typeSymbol.IsDynamic())
                            {
                                typeSymbol = _factory.SpecialType(SpecialType.System_Object);
                                boundExpression = _factory.Convert(typeSymbol, boundExpression);
                            }
                            TypeSymbol type = boundDagTypeEvaluation.Type;
                            BoundDagTemp dagTemp2 = new BoundDagTemp(boundDagTypeEvaluation.Syntax, type, boundDagTypeEvaluation);
                            BoundExpression temp2 = _tempAllocator.GetTemp(dagTemp2);
                            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _localRewriter.GetNewCompoundUseSiteInfo();
                            Conversion conversion = _factory.Compilation.Conversions.ClassifyBuiltInConversion(typeSymbol, temp2.Type, ref useSiteInfo);
                            _localRewriter._diagnostics.Add(boundDagTypeEvaluation.Syntax, useSiteInfo);
                            BoundExpression right = ((!conversion.Exists) ? _factory.As(boundExpression, type) : ((conversion.Kind != ConversionKind.ExplicitNullable || !typeSymbol.GetNullableUnderlyingType().Equals(temp2.Type, TypeCompareKind.AllIgnoreOptions) || !_localRewriter.TryGetNullableMethod(boundDagTypeEvaluation.Syntax, typeSymbol, SpecialMember.System_Nullable_T_GetValueOrDefault, out MethodSymbol result)) ? _factory.Convert(type, boundExpression, conversion) : _factory.Call(boundExpression, result)));
                            return _factory.AssignmentExpression(temp2, right);
                        }
                        MethodSymbol deconstructMethod = boundDagDeconstructEvaluation.DeconstructMethod;
                        refKindBuilder = ArrayBuilder<RefKind>.GetInstance();
                        argBuilder = ArrayBuilder<BoundExpression>.GetInstance();
                        BoundExpression receiver;
                        int num;
                        if (deconstructMethod.IsStatic)
                        {
                            receiver = _factory.Type(deconstructMethod.ContainingType);
                            addArg(deconstructMethod.ParameterRefKinds[0], boundExpression);
                            num = 1;
                        }
                        else
                        {
                            receiver = boundExpression;
                            num = 0;
                        }
                        for (int i = num; i < deconstructMethod.ParameterCount; i++)
                        {
                            ParameterSymbol parameterSymbol = deconstructMethod.Parameters[i];
                            BoundDagTemp dagTemp3 = new BoundDagTemp(boundDagDeconstructEvaluation.Syntax, parameterSymbol.Type, boundDagDeconstructEvaluation, i - num);
                            addArg(RefKind.Out, _tempAllocator.GetTemp(dagTemp3));
                        }
                        return _factory.Call(receiver, deconstructMethod, refKindBuilder.ToImmutableAndFree(), argBuilder.ToImmutableAndFree());
                    }
                    PropertySymbol property = boundDagPropertyEvaluation.Property;
                    BoundDagTemp dagTemp4 = new BoundDagTemp(boundDagPropertyEvaluation.Syntax, property.Type, boundDagPropertyEvaluation);
                    BoundExpression temp3 = _tempAllocator.GetTemp(dagTemp4);
                    return _factory.AssignmentExpression(temp3, _factory.Property(boundExpression, property));
                }
                FieldSymbol field = boundDagFieldEvaluation.Field;
                BoundDagTemp dagTemp5 = new BoundDagTemp(boundDagFieldEvaluation.Syntax, field.Type, boundDagFieldEvaluation);
                BoundExpression temp4 = _tempAllocator.GetTemp(dagTemp5);
                BoundExpression boundExpression2 = _localRewriter.MakeFieldAccess(boundDagFieldEvaluation.Syntax, boundExpression, field, null, LookupResultKind.Viable, field.Type);
                boundExpression2.WasCompilerGenerated = true;
                return _factory.AssignmentExpression(temp4, boundExpression2);
                void addArg(RefKind refKind, BoundExpression expression)
                {
                    refKindBuilder.Add(refKind);
                    argBuilder.Add(expression);
                }
            }

            protected BoundExpression LowerTest(BoundDagTest test)
            {
                _factory.Syntax = test.Syntax;
                BoundExpression temp = _tempAllocator.GetTemp(test.Input);
                if (!(test is BoundDagNonNullTest boundDagNonNullTest))
                {
                    if (!(test is BoundDagTypeTest boundDagTypeTest))
                    {
                        if (!(test is BoundDagExplicitNullTest boundDagExplicitNullTest))
                        {
                            if (!(test is BoundDagValueTest boundDagValueTest))
                            {
                                if (test is BoundDagRelationalTest boundDagRelationalTest)
                                {
                                    return MakeRelationalTest(boundDagRelationalTest.Syntax, temp, boundDagRelationalTest.OperatorKind, boundDagRelationalTest.Value);
                                }
                                throw ExceptionUtilities.UnexpectedValue(test);
                            }
                            return MakeValueTest(boundDagValueTest.Syntax, temp, boundDagValueTest.Value);
                        }
                        return MakeNullCheck(boundDagExplicitNullTest.Syntax, temp, temp.Type.IsNullableType() ? BinaryOperatorKind.NullableNullEqual : BinaryOperatorKind.Equal);
                    }
                    return _factory.Is(temp, boundDagTypeTest.Type);
                }
                return MakeNullCheck(boundDagNonNullTest.Syntax, temp, temp.Type.IsNullableType() ? BinaryOperatorKind.NullableNullNotEqual : BinaryOperatorKind.NotEqual);
            }

            private BoundExpression MakeNullCheck(SyntaxNode syntax, BoundExpression rewrittenExpr, BinaryOperatorKind operatorKind)
            {
                if (rewrittenExpr.Type.IsPointerOrFunctionPointer())
                {
                    TypeSymbol type = _factory.SpecialType(SpecialType.System_Object);
                    PointerTypeSymbol type2 = new PointerTypeSymbol(TypeWithAnnotations.Create(_factory.SpecialType(SpecialType.System_Void)));
                    return _localRewriter.MakeBinaryOperator(syntax, operatorKind, _factory.Convert(type2, rewrittenExpr), _factory.Convert(type2, new BoundLiteral(syntax, ConstantValue.Null, type)), _factory.SpecialType(SpecialType.System_Boolean), null);
                }
                return _localRewriter.MakeNullCheck(syntax, rewrittenExpr, operatorKind);
            }

            protected BoundExpression MakeValueTest(SyntaxNode syntax, BoundExpression input, ConstantValue value)
            {
                BinaryOperatorKind binaryOperatorKind = Binder.RelationalOperatorType(input.Type.EnumUnderlyingTypeOrSelf());
                BinaryOperatorKind operatorKind = BinaryOperatorKind.Equal | binaryOperatorKind;
                return MakeRelationalTest(syntax, input, operatorKind, value);
            }

            protected BoundExpression MakeRelationalTest(SyntaxNode syntax, BoundExpression input, BinaryOperatorKind operatorKind, ConstantValue value)
            {
                if ((input.Type!.SpecialType == SpecialType.System_Double && double.IsNaN(value.DoubleValue)) || (input.Type!.SpecialType == SpecialType.System_Single && float.IsNaN(value.SingleValue)))
                {
                    return _factory.MakeIsNotANumberTest(input);
                }
                BoundExpression boundExpression = _localRewriter.MakeLiteral(syntax, value, input.Type);
                TypeSymbol typeSymbol = input.Type.EnumUnderlyingTypeOrSelf();
                if (operatorKind.OperandTypes() == BinaryOperatorKind.Int && typeSymbol.SpecialType != SpecialType.System_Int32)
                {
                    typeSymbol = _factory.SpecialType(SpecialType.System_Int32);
                    input = _factory.Convert(typeSymbol, input);
                    boundExpression = _factory.Convert(typeSymbol, boundExpression);
                }
                return _localRewriter.MakeBinaryOperator(_factory.Syntax, operatorKind, input, boundExpression, _factory.SpecialType(SpecialType.System_Boolean), null);
            }

            protected bool TryLowerTypeTestAndCast(BoundDagTest test, BoundDagEvaluation evaluation, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out BoundExpression sideEffect, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out BoundExpression testExpression)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _localRewriter.GetNewCompoundUseSiteInfo();
                if (test is BoundDagTypeTest boundDagTypeTest && evaluation is BoundDagTypeEvaluation boundDagTypeEvaluation && boundDagTypeTest.Type.IsReferenceType && boundDagTypeEvaluation.Type.Equals(boundDagTypeTest.Type, TypeCompareKind.AllIgnoreOptions) && boundDagTypeEvaluation.Input == boundDagTypeTest.Input)
                {
                    BoundExpression temp = _tempAllocator.GetTemp(test.Input);
                    BoundExpression temp2 = _tempAllocator.GetTemp(new BoundDagTemp(evaluation.Syntax, boundDagTypeEvaluation.Type, evaluation));
                    sideEffect = _factory.AssignmentExpression(temp2, _factory.As(temp, boundDagTypeEvaluation.Type));
                    testExpression = _factory.ObjectNotEqual(temp2, _factory.Null(temp2.Type));
                    return true;
                }
                if (test is BoundDagNonNullTest boundDagNonNullTest && evaluation is BoundDagTypeEvaluation boundDagTypeEvaluation2)
                {
                    Conversion conversion = _factory.Compilation.Conversions.ClassifyBuiltInConversion(test.Input.Type, boundDagTypeEvaluation2.Type, ref useSiteInfo);
                    if ((conversion.IsIdentity || conversion.Kind == ConversionKind.ImplicitReference || conversion.IsBoxing) && boundDagTypeEvaluation2.Input == boundDagNonNullTest.Input)
                    {
                        BoundExpression temp3 = _tempAllocator.GetTemp(test.Input);
                        TypeSymbol type = boundDagTypeEvaluation2.Type;
                        BoundExpression temp4 = _tempAllocator.GetTemp(new BoundDagTemp(evaluation.Syntax, type, evaluation));
                        sideEffect = _factory.AssignmentExpression(temp4, _factory.Convert(type, temp3));
                        testExpression = _factory.ObjectNotEqual(temp4, _factory.Null(type));
                        _localRewriter._diagnostics.Add(test.Syntax, useSiteInfo);
                        return true;
                    }
                }
                sideEffect = (testExpression = null);
                return false;
            }

            protected BoundDecisionDag ShareTempsAndEvaluateInput(BoundExpression loweredInput, BoundDecisionDag decisionDag, Action<BoundExpression> addCode, out BoundExpression savedInputExpression)
            {
                bool flag = decisionDag.TopologicallySortedNodes.Any(delegate (BoundDecisionDagNode node)
                {
                    if (node is BoundWhenDecisionDagNode boundWhenDecisionDagNode3)
                    {
                        BoundExpression whenExpression = boundWhenDecisionDagNode3.WhenExpression;
                        if (whenExpression != null)
                        {
                            return (object)whenExpression.ConstantValue == null;
                        }
                    }
                    return false;
                });
                BoundDagTemp dagTemp = BoundDagTemp.ForOriginalInput(loweredInput);
                if ((loweredInput.Kind == BoundKind.Local || loweredInput.Kind == BoundKind.Parameter) && loweredInput.GetRefKind() == RefKind.None && !flag)
                {
                    _tempAllocator.TrySetTemp(dagTemp, loweredInput);
                }
                ImmutableArray<BoundDecisionDagNode>.Enumerator enumerator = decisionDag.TopologicallySortedNodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!(enumerator.Current is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        continue;
                    }
                    ImmutableArray<BoundPatternBinding>.Enumerator enumerator2 = boundWhenDecisionDagNode.Bindings.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        BoundPatternBinding current = enumerator2.Current;
                        if (current.VariableAccess is BoundLocal)
                        {
                            _tempAllocator.TrySetTemp(current.TempContainingValue, current.VariableAccess);
                        }
                    }
                }
                if (loweredInput.Type!.IsTupleType && !loweredInput.Type!.OriginalDefinition.Equals(_factory.Compilation.GetWellKnownType(WellKnownType.System_ValueTuple_TRest)) && loweredInput.Syntax.Kind() == SyntaxKind.TupleExpression && loweredInput is BoundObjectCreationExpression loweredInput2 && !decisionDag.TopologicallySortedNodes.Any((BoundDecisionDagNode n) => usesOriginalInput(n)))
                {
                    decisionDag = RewriteTupleInput(decisionDag, loweredInput2, addCode, !flag, out savedInputExpression);
                }
                else
                {
                    BoundExpression boundExpression = (savedInputExpression = _tempAllocator.GetTemp(dagTemp));
                    if (boundExpression != loweredInput)
                    {
                        addCode(_factory.AssignmentExpression(boundExpression, loweredInput));
                    }
                }
                return decisionDag;
                static bool usesOriginalInput(BoundDecisionDagNode node)
                {
                    if (node is BoundWhenDecisionDagNode boundWhenDecisionDagNode2)
                    {
                        return boundWhenDecisionDagNode2.Bindings.Any((BoundPatternBinding b) => b.TempContainingValue.IsOriginalInput);
                    }
                    if (node is BoundTestDecisionDagNode boundTestDecisionDagNode)
                    {
                        return boundTestDecisionDagNode.Test.Input.IsOriginalInput;
                    }
                    if (node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode)
                    {
                        if (boundEvaluationDecisionDagNode.Evaluation is BoundDagFieldEvaluation boundDagFieldEvaluation)
                        {
                            if (boundDagFieldEvaluation.Input.IsOriginalInput)
                            {
                                return !boundDagFieldEvaluation.Field.IsTupleElement();
                            }
                            return false;
                        }
                        return boundEvaluationDecisionDagNode.Evaluation.Input.IsOriginalInput;
                    }
                    return false;
                }
            }

            private BoundDecisionDag RewriteTupleInput(BoundDecisionDag decisionDag, BoundObjectCreationExpression loweredInput, Action<BoundExpression> addCode, bool canShareInputs, out BoundExpression savedInputExpression)
            {
                int length = loweredInput.Arguments.Length;
                BoundDagTemp input = BoundDagTemp.ForOriginalInput(loweredInput.Syntax, loweredInput.Type);
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(loweredInput.Arguments.Length);
                for (int i = 0; i < length; i++)
                {
                    FieldSymbol correspondingTupleField = loweredInput.Type.TupleElements[i].CorrespondingTupleField;
                    BoundExpression boundExpression = loweredInput.Arguments[i];
                    BoundDagFieldEvaluation source = new BoundDagFieldEvaluation(boundExpression.Syntax, correspondingTupleField, input);
                    BoundDagTemp boundDagTemp = new BoundDagTemp(boundExpression.Syntax, boundExpression.Type, source);
                    storeToTemp(boundDagTemp, boundExpression);
                    instance.Add(_tempAllocator.GetTemp(boundDagTemp));
                }
                BoundDecisionDag result = decisionDag.Rewrite(makeReplacement);
                savedInputExpression = loweredInput.Update(loweredInput.Constructor, instance.ToImmutableAndFree(), loweredInput.ArgumentNamesOpt, loweredInput.ArgumentRefKindsOpt, loweredInput.Expanded, loweredInput.ArgsToParamsOpt, loweredInput.DefaultArguments, loweredInput.ConstantValueOpt, loweredInput.InitializerExpressionOpt, loweredInput.Type);
                return result;
                static BoundDecisionDagNode makeReplacement(BoundDecisionDagNode node, Func<BoundDecisionDagNode, BoundDecisionDagNode> replacement)
                {
                    if (!(node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                    {
                        if (node is BoundTestDecisionDagNode)
                        {
                        }
                    }
                    else if (boundEvaluationDecisionDagNode.Evaluation is BoundDagFieldEvaluation boundDagFieldEvaluation && boundDagFieldEvaluation.Input.IsOriginalInput)
                    {
                        FieldSymbol field = boundDagFieldEvaluation.Field;
                        if (field.CorrespondingTupleField != null)
                        {
                            _ = field.TupleElementIndex;
                            return replacement(boundEvaluationDecisionDagNode.Next);
                        }
                    }
                    return BoundDecisionDag.TrivialReplacement(node, replacement);
                }
                void storeToTemp(BoundDagTemp temp, BoundExpression expr)
                {
                    if (!canShareInputs || (expr.Kind != BoundKind.Parameter && expr.Kind != BoundKind.Local) || !_tempAllocator.TrySetTemp(temp, expr))
                    {
                        BoundExpression temp2 = _tempAllocator.GetTemp(temp);
                        addCode(_factory.AssignmentExpression(temp2, expr));
                    }
                }
            }
        }

        private abstract class BaseSwitchLocalRewriter : DecisionDagRewriter
        {
            private readonly PooledDictionary<SyntaxNode, ArrayBuilder<BoundStatement>> _switchArms = PooledDictionary<SyntaxNode, ArrayBuilder<BoundStatement>>.GetInstance();

            protected override ArrayBuilder<BoundStatement> BuilderForSection(SyntaxNode whenClauseSyntax)
            {
                SyntaxNode key = ((whenClauseSyntax is SwitchLabelSyntax switchLabelSyntax) ? switchLabelSyntax.Parent : whenClauseSyntax);
                if (!_switchArms.TryGetValue(key, out var value) || value == null)
                {
                    throw new InvalidOperationException();
                }
                return value;
            }

            protected BaseSwitchLocalRewriter(SyntaxNode node, LocalRewriter localRewriter, ImmutableArray<SyntaxNode> arms, bool generateInstrumentation)
                : base(node, localRewriter, generateInstrumentation)
            {
                ImmutableArray<SyntaxNode>.Enumerator enumerator = arms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNode current = enumerator.Current;
                    ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                    if (base.GenerateInstrumentation)
                    {
                        instance.Add(_factory.HiddenSequencePoint());
                    }
                    _switchArms.Add(current, instance);
                }
            }

            protected new void Free()
            {
                _switchArms.Free();
                base.Free();
            }

            protected (ImmutableArray<BoundStatement> loweredDag, ImmutableDictionary<SyntaxNode, ImmutableArray<BoundStatement>> switchSections) LowerDecisionDag(BoundDecisionDag decisionDag)
            {
                ImmutableArray<BoundStatement> item = LowerDecisionDagCore(decisionDag);
                ImmutableDictionary<SyntaxNode, ImmutableArray<BoundStatement>> item2 = _switchArms.ToImmutableDictionary<KeyValuePair<SyntaxNode, ArrayBuilder<BoundStatement>>, SyntaxNode, ImmutableArray<BoundStatement>>((KeyValuePair<SyntaxNode, ArrayBuilder<BoundStatement>> kv) => kv.Key, (KeyValuePair<SyntaxNode, ArrayBuilder<BoundStatement>> kv) => kv.Value.ToImmutableAndFree());
                _switchArms.Clear();
                return (item, item2);
            }
        }

        private enum ConditionalAccessLoweringKind
        {
            LoweredConditionalAccess,
            Conditional,
            ConditionalCaptureReceiverByVal
        }

        private class DeconstructionSideEffects
        {
            internal ArrayBuilder<BoundExpression> init;

            internal ArrayBuilder<BoundExpression> deconstructions;

            internal ArrayBuilder<BoundExpression> conversions;

            internal ArrayBuilder<BoundExpression> assignments;

            internal static DeconstructionSideEffects GetInstance()
            {
                return new DeconstructionSideEffects
                {
                    init = ArrayBuilder<BoundExpression>.GetInstance(),
                    deconstructions = ArrayBuilder<BoundExpression>.GetInstance(),
                    conversions = ArrayBuilder<BoundExpression>.GetInstance(),
                    assignments = ArrayBuilder<BoundExpression>.GetInstance()
                };
            }

            internal void Consolidate()
            {
                init.AddRange(deconstructions);
                init.AddRange(conversions);
                init.AddRange(assignments);
                deconstructions.Free();
                conversions.Free();
                assignments.Free();
            }

            internal BoundExpression? PopLast()
            {
                if (init.Count == 0)
                {
                    return null;
                }
                BoundExpression result = init.Last();
                init.RemoveLast();
                return result;
            }

            internal ImmutableArray<BoundExpression> ToImmutableAndFree()
            {
                return init.ToImmutableAndFree();
            }

            internal void Free()
            {
                init.Free();
            }
        }

        private enum EventAssignmentKind
        {
            Assignment,
            Addition,
            Subtraction
        }

        private sealed class IsPatternExpressionGeneralLocalRewriter : DecisionDagRewriter
        {
            private readonly ArrayBuilder<BoundStatement> _statements = ArrayBuilder<BoundStatement>.GetInstance();

            public IsPatternExpressionGeneralLocalRewriter(SyntaxNode node, LocalRewriter localRewriter)
                : base(node, localRewriter, generateInstrumentation: false)
            {
            }

            protected override ArrayBuilder<BoundStatement> BuilderForSection(SyntaxNode section)
            {
                return _statements;
            }

            public new void Free()
            {
                base.Free();
                _statements.Free();
            }

            internal BoundExpression LowerGeneralIsPattern(BoundIsPatternExpression node)
            {
                _factory.Syntax = node.Syntax;
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                BoundExpression loweredSwitchGoverningExpression = _localRewriter.VisitExpression(node.Expression);
                BoundDecisionDag decisionDag = ShareTempsIfPossibleAndEvaluateInput(node.DecisionDag, loweredSwitchGoverningExpression, instance, out BoundExpression savedInputExpression);
                ImmutableArray<BoundStatement> statements = LowerDecisionDagCore(decisionDag);
                instance.Add(_factory.Block(statements));
                LocalSymbol localSymbol = _factory.SynthesizedLocal(node.Type, node.Syntax);
                LabelSymbol label = _factory.GenerateLabel("afterIsPatternExpression");
                LabelSymbol whenTrueLabel = node.WhenTrueLabel;
                LabelSymbol whenFalseLabel = node.WhenFalseLabel;
                if (_statements.Count != 0)
                {
                    instance.Add(_factory.Block(_statements.ToArray()));
                }
                instance.Add(_factory.Label(whenTrueLabel));
                instance.Add(_factory.Assignment(_factory.Local(localSymbol), _factory.Literal(value: true)));
                instance.Add(_factory.Goto(label));
                instance.Add(_factory.Label(whenFalseLabel));
                instance.Add(_factory.Assignment(_factory.Local(localSymbol), _factory.Literal(value: false)));
                instance.Add(_factory.Label(label));
                _localRewriter._needsSpilling = true;
                return _factory.SpillSequence(_tempAllocator.AllTemps().Add(localSymbol), instance.ToImmutableAndFree(), _factory.Local(localSymbol));
            }
        }

        private sealed class IsPatternExpressionLinearLocalRewriter : PatternLocalRewriter
        {
            private readonly ArrayBuilder<BoundExpression> _sideEffectBuilder;

            private readonly ArrayBuilder<BoundExpression> _conjunctBuilder;

            public IsPatternExpressionLinearLocalRewriter(BoundIsPatternExpression node, LocalRewriter localRewriter)
                : base(node.Syntax, localRewriter, generateInstrumentation: false)
            {
                _conjunctBuilder = ArrayBuilder<BoundExpression>.GetInstance();
                _sideEffectBuilder = ArrayBuilder<BoundExpression>.GetInstance();
            }

            public new void Free()
            {
                _conjunctBuilder.Free();
                _sideEffectBuilder.Free();
                base.Free();
            }

            private void AddConjunct(BoundExpression test)
            {
                TypeSymbol? type = test.Type;
                if ((object)type != null && !type.IsErrorType())
                {
                    if (_sideEffectBuilder.Count != 0)
                    {
                        test = _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, _sideEffectBuilder.ToImmutable(), test);
                        _sideEffectBuilder.Clear();
                    }
                    _conjunctBuilder.Add(test);
                }
            }

            private void LowerOneTest(BoundDagTest test, bool invert = false)
            {
                _factory.Syntax = test.Syntax;
                if (test is BoundDagEvaluation evaluation)
                {
                    BoundExpression item = LowerEvaluation(evaluation);
                    _sideEffectBuilder.Add(item);
                    return;
                }
                BoundExpression boundExpression = LowerTest(test);
                if (boundExpression != null)
                {
                    if (invert)
                    {
                        boundExpression = _factory.Not(boundExpression);
                    }
                    AddConjunct(boundExpression);
                }
            }

            public BoundExpression LowerIsPatternAsLinearTestSequence(BoundIsPatternExpression isPatternExpression, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel)
            {
                BoundDecisionDag decisionDag = isPatternExpression.DecisionDag;
                BoundExpression loweredInput = _localRewriter.VisitExpression(isPatternExpression.Expression);
                decisionDag = ShareTempsAndEvaluateInput(loweredInput, decisionDag, delegate (BoundExpression expr)
                {
                    _sideEffectBuilder.Add(expr);
                }, out var _);
                BoundDecisionDagNode rootNode = decisionDag.RootNode;
                return ProduceLinearTestSequence(rootNode, whenTrueLabel, whenFalseLabel);
            }

            private BoundExpression ProduceLinearTestSequence(BoundDecisionDagNode node, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel)
            {
                while (node.Kind != BoundKind.LeafDecisionDagNode && node.Kind != BoundKind.WhenDecisionDagNode)
                {
                    if (!(node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                    {
                        if (node is BoundTestDecisionDagNode boundTestDecisionDagNode)
                        {
                            if (boundTestDecisionDagNode.WhenTrue is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode2 && TryLowerTypeTestAndCast(boundTestDecisionDagNode.Test, boundEvaluationDecisionDagNode2.Evaluation, out var sideEffect, out var testExpression))
                            {
                                _sideEffectBuilder.Add(sideEffect);
                                AddConjunct(testExpression);
                                node = boundEvaluationDecisionDagNode2.Next;
                            }
                            else
                            {
                                bool flag = IsFailureNode(boundTestDecisionDagNode.WhenTrue, whenFalseLabel);
                                LowerOneTest(boundTestDecisionDagNode.Test, flag);
                                node = (flag ? boundTestDecisionDagNode.WhenFalse : boundTestDecisionDagNode.WhenTrue);
                            }
                        }
                    }
                    else
                    {
                        LowerOneTest(boundEvaluationDecisionDagNode.Evaluation);
                        node = boundEvaluationDecisionDagNode.Next;
                    }
                }
                if (!(node is BoundLeafDecisionDagNode))
                {
                    if (!(node is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        throw ExceptionUtilities.UnexpectedValue(node.Kind);
                    }
                    ImmutableArray<BoundPatternBinding>.Enumerator enumerator = boundWhenDecisionDagNode.Bindings.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        BoundPatternBinding current = enumerator.Current;
                        BoundExpression boundExpression = _localRewriter.VisitExpression(current.VariableAccess);
                        BoundExpression temp = _tempAllocator.GetTemp(current.TempContainingValue);
                        if (boundExpression != temp)
                        {
                            _sideEffectBuilder.Add(_factory.AssignmentExpression(boundExpression, temp));
                        }
                    }
                }
                if (_sideEffectBuilder.Count > 0 || _conjunctBuilder.Count == 0)
                {
                    AddConjunct(_factory.Literal(value: true));
                }
                BoundExpression boundExpression2 = null;
                ArrayBuilder<BoundExpression>.Enumerator enumerator2 = _conjunctBuilder.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundExpression current2 = enumerator2.Current;
                    boundExpression2 = ((boundExpression2 == null) ? current2 : _factory.LogicalAnd(boundExpression2, current2));
                }
                _conjunctBuilder.Clear();
                ImmutableArray<LocalSymbol> locals = _tempAllocator.AllTemps();
                if (locals.Length > 0)
                {
                    boundExpression2 = _factory.Sequence(locals, ImmutableArray<BoundExpression>.Empty, boundExpression2);
                }
                return boundExpression2;
            }
        }

        private sealed class SwitchStatementLocalRewriter : BaseSwitchLocalRewriter
        {
            private readonly Dictionary<SyntaxNode, LabelSymbol> _sectionLabels = PooledDictionary<SyntaxNode, LabelSymbol>.GetInstance();

            public static BoundStatement Rewrite(LocalRewriter localRewriter, BoundSwitchStatement node)
            {
                SwitchStatementLocalRewriter switchStatementLocalRewriter = new SwitchStatementLocalRewriter(node, localRewriter);
                BoundStatement result = switchStatementLocalRewriter.LowerSwitchStatement(node);
                switchStatementLocalRewriter.Free();
                return result;
            }

            protected override LabelSymbol GetDagNodeLabel(BoundDecisionDagNode dag)
            {
                LabelSymbol dagNodeLabel = base.GetDagNodeLabel(dag);
                if (dag is BoundLeafDecisionDagNode boundLeafDecisionDagNode)
                {
                    SyntaxNode parent = boundLeafDecisionDagNode.Syntax.Parent;
                    if (parent != null && parent.Kind() == SyntaxKind.SwitchSection)
                    {
                        if (_sectionLabels.TryGetValue(parent, out var value))
                        {
                            return value;
                        }
                        _sectionLabels.Add(parent, dagNodeLabel);
                    }
                }
                return dagNodeLabel;
            }

            private SwitchStatementLocalRewriter(BoundSwitchStatement node, LocalRewriter localRewriter)
                : base(node.Syntax, localRewriter, node.SwitchSections.SelectAsArray((BoundSwitchSection section) => section.Syntax), localRewriter.Instrument && !node.WasCompilerGenerated)
            {
            }

            private BoundStatement LowerSwitchStatement(BoundSwitchStatement node)
            {
                _factory.Syntax = node.Syntax;
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
                BoundExpression boundExpression = _localRewriter.VisitExpression(node.Expression);
                if (!node.WasCompilerGenerated && _localRewriter.Instrument)
                {
                    BoundExpression boundExpression2 = _localRewriter._instrumenter.InstrumentSwitchStatementExpression(node, boundExpression, _factory);
                    if (boundExpression.ConstantValue == null)
                    {
                        boundExpression = boundExpression2;
                    }
                    else
                    {
                        instance.Add(_factory.ExpressionStatement(boundExpression2));
                    }
                }
                instance2.AddRange(node.InnerLocals);
                BoundDecisionDag decisionDag = ShareTempsIfPossibleAndEvaluateInput(node.DecisionDag, boundExpression, instance, out BoundExpression savedInputExpression);
                if (base.GenerateInstrumentation)
                {
                    if (instance.Count == 0)
                    {
                        instance.Add(_factory.NoOp(NoOpStatementFlavor.Default));
                    }
                    instance.Add(_factory.HiddenSequencePoint());
                }
                var (statements, immutableDictionary) = LowerDecisionDag(decisionDag);
                instance.Add(_factory.Block(statements));
                ImmutableArray<BoundSwitchSection>.Enumerator enumerator = node.SwitchSections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundSwitchSection current = enumerator.Current;
                    _factory.Syntax = current.Syntax;
                    ArrayBuilder<BoundStatement> instance3 = ArrayBuilder<BoundStatement>.GetInstance();
                    instance3.AddRange(immutableDictionary[current.Syntax]);
                    ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = current.SwitchLabels.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        BoundSwitchLabel current2 = enumerator2.Current;
                        instance3.Add(_factory.Label(current2.Label));
                    }
                    instance3.AddRange(_localRewriter.VisitList(current.Statements));
                    ImmutableArray<BoundStatement> statements2 = instance3.ToImmutableAndFree();
                    if (current.Locals.IsEmpty)
                    {
                        instance.Add(_factory.StatementList(statements2));
                        continue;
                    }
                    instance2.AddRange(current.Locals);
                    instance.Add(new BoundScope(current.Syntax, current.Locals, statements2));
                }
                instance2.AddRange(_tempAllocator.AllTemps());
                _factory.Syntax = node.Syntax;
                if (base.GenerateInstrumentation)
                {
                    instance.Add(_factory.HiddenSequencePoint());
                }
                instance.Add(_factory.Label(node.BreakLabel));
                BoundStatement boundStatement = _factory.Block(instance2.ToImmutableAndFree(), node.InnerLocalFunctions, instance.ToImmutableAndFree());
                if (base.GenerateInstrumentation)
                {
                    boundStatement = _localRewriter._instrumenter.InstrumentSwitchStatement(node, boundStatement);
                }
                return boundStatement;
            }
        }

        private sealed class SwitchExpressionLocalRewriter : BaseSwitchLocalRewriter
        {
            private SwitchExpressionLocalRewriter(BoundConvertedSwitchExpression node, LocalRewriter localRewriter)
                : base(node.Syntax, localRewriter, node.SwitchArms.SelectAsArray((BoundSwitchExpressionArm arm) => arm.Syntax), !node.WasCompilerGenerated && localRewriter.Instrument)
            {
            }

            public static BoundExpression Rewrite(LocalRewriter localRewriter, BoundConvertedSwitchExpression node)
            {
                SwitchExpressionLocalRewriter switchExpressionLocalRewriter = new SwitchExpressionLocalRewriter(node, localRewriter);
                BoundExpression result = switchExpressionLocalRewriter.LowerSwitchExpression(node);
                switchExpressionLocalRewriter.Free();
                return result;
            }

            private BoundExpression LowerSwitchExpression(BoundConvertedSwitchExpression node)
            {
                bool flag = base.GenerateInstrumentation && _localRewriter._compilation.Options.OptimizationLevel != OptimizationLevel.Release;
                _factory.Syntax = node.Syntax;
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
                BoundExpression loweredSwitchGoverningExpression = _localRewriter.VisitExpression(node.Expression);
                BoundDecisionDag decisionDag = ShareTempsIfPossibleAndEvaluateInput(node.DecisionDag, loweredSwitchGoverningExpression, instance, out BoundExpression savedInputExpression);
                object identifier = new object();
                object identifier2 = new object();
                var (statements, immutableDictionary) = LowerDecisionDag(decisionDag);
                if (flag)
                {
                    SwitchExpressionSyntax switchExpressionSyntax = (SwitchExpressionSyntax)node.Syntax;
                    instance.Add(new BoundSavePreviousSequencePoint(switchExpressionSyntax, identifier));
                    int start = switchExpressionSyntax.SwitchKeyword.Span.Start;
                    int end = switchExpressionSyntax.Span.End;
                    instance.Add(new BoundStepThroughSequencePoint(span: new TextSpan(start, end - start), syntax: node.Syntax));
                    instance.Add(new BoundSavePreviousSequencePoint(switchExpressionSyntax, identifier2));
                }
                instance.Add(_factory.Block(statements));
                LocalSymbol localSymbol = _factory.SynthesizedLocal(node.Type, node.Syntax);
                LabelSymbol label = _factory.GenerateLabel("afterSwitchExpression");
                ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = node.SwitchArms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundSwitchExpressionArm current = enumerator.Current;
                    _factory.Syntax = current.Syntax;
                    ArrayBuilder<BoundStatement> instance3 = ArrayBuilder<BoundStatement>.GetInstance();
                    instance3.AddRange(immutableDictionary[current.Syntax]);
                    instance3.Add(_factory.Label(current.Label));
                    BoundExpression boundExpression = _localRewriter.VisitExpression(current.Value);
                    if (base.GenerateInstrumentation)
                    {
                        boundExpression = _localRewriter._instrumenter.InstrumentSwitchExpressionArmExpression(current.Value, boundExpression, _factory);
                    }
                    instance3.Add(_factory.Assignment(_factory.Local(localSymbol), boundExpression));
                    instance3.Add(_factory.Goto(label));
                    ImmutableArray<BoundStatement> statements2 = instance3.ToImmutableAndFree();
                    if (current.Locals.IsEmpty)
                    {
                        instance.Add(_factory.StatementList(statements2));
                        continue;
                    }
                    instance2.AddRange(current.Locals);
                    instance.Add(new BoundScope(current.Syntax, current.Locals, statements2));
                }
                _factory.Syntax = node.Syntax;
                if (node.DefaultLabel != null)
                {
                    instance.Add(_factory.Label(node.DefaultLabel));
                    if (flag)
                    {
                        instance.Add(new BoundRestorePreviousSequencePoint(node.Syntax, identifier2));
                    }
                    NamedTypeSymbol type2 = _factory.SpecialType(SpecialType.System_Object);
                    BoundObjectCreationExpression e = ((implicitConversionExists(savedInputExpression, type2) && _factory.WellKnownMember(WellKnownMember.System_Runtime_CompilerServices_SwitchExpressionException__ctorObject, isOptional: true) is MethodSymbol ctor) ? _factory.New(ctor, _factory.Convert(type2, savedInputExpression)) : ((_factory.WellKnownMember(WellKnownMember.System_Runtime_CompilerServices_SwitchExpressionException__ctor, isOptional: true) is MethodSymbol ctor2) ? _factory.New(ctor2) : _factory.New(_factory.WellKnownMethod(WellKnownMember.System_InvalidOperationException__ctor))));
                    instance.Add(_factory.Throw(e));
                }
                if (base.GenerateInstrumentation)
                {
                    instance.Add(_factory.HiddenSequencePoint());
                }
                instance.Add(_factory.Label(label));
                if (flag)
                {
                    instance.Add(new BoundRestorePreviousSequencePoint(node.Syntax, identifier));
                }
                instance2.Add(localSymbol);
                instance2.AddRange(_tempAllocator.AllTemps());
                return _factory.SpillSequence(instance2.ToImmutableAndFree(), instance.ToImmutableAndFree(), _factory.Local(localSymbol));
                bool implicitConversionExists(BoundExpression expression, TypeSymbol type)
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    return _localRewriter._compilation.Conversions.ClassifyConversionFromExpression(expression, type, ref useSiteInfo).IsImplicit;
                }
            }
        }

        private readonly CSharpCompilation _compilation;

        private readonly SyntheticBoundNodeFactory _factory;

        private readonly SynthesizedSubmissionFields _previousSubmissionFields;

        private readonly bool _allowOmissionOfConditionalCalls;

        private LoweredDynamicOperationFactory _dynamicFactory;

        private bool _sawLambdas;

        private int _availableLocalFunctionOrdinal;

        private bool _inExpressionLambda;

        private bool _sawAwait;

        private bool _sawAwaitInExceptionHandler;

        private bool _needsSpilling;

        private readonly BindingDiagnosticBag _diagnostics;

        private Instrumenter _instrumenter;

        private readonly BoundStatement _rootStatement;

        private Dictionary<BoundValuePlaceholderBase, BoundExpression>? _placeholderReplacementMapDoNotUseDirectly;

        private BoundExpression? _currentConditionalAccessTarget;

        private int _currentConditionalAccessID;

        private Dictionary<BoundNode, HashSet<LabelSymbol>>? _lazyUnmatchedLabelCache;

        private bool Instrument => !_inExpressionLambda;

        private PEModuleBuilder? EmitModule => _factory.CompilationState.ModuleBuilderOpt;

        private bool IsLambdaOrExpressionBodiedMember
        {
            get
            {
                MethodSymbol currentFunction = _factory.CurrentFunction;
                if (currentFunction is LambdaSymbol)
                {
                    return true;
                }
                return (currentFunction as SourceMemberMethodSymbol)?.IsExpressionBodied ?? (currentFunction as LocalFunctionSymbol)?.IsExpressionBodied ?? false;
            }
        }

        private LocalRewriter(CSharpCompilation compilation, MethodSymbol containingMethod, int containingMethodOrdinal, BoundStatement rootStatement, NamedTypeSymbol? containingType, SyntheticBoundNodeFactory factory, SynthesizedSubmissionFields previousSubmissionFields, bool allowOmissionOfConditionalCalls, BindingDiagnosticBag diagnostics, Instrumenter instrumenter)
        {
            _compilation = compilation;
            _factory = factory;
            _factory.CurrentFunction = containingMethod;
            _dynamicFactory = new LoweredDynamicOperationFactory(factory, containingMethodOrdinal);
            _previousSubmissionFields = previousSubmissionFields;
            _allowOmissionOfConditionalCalls = allowOmissionOfConditionalCalls;
            _diagnostics = diagnostics;
            _instrumenter = instrumenter;
            _rootStatement = rootStatement;
        }

        public static BoundStatement Rewrite(CSharpCompilation compilation, MethodSymbol method, int methodOrdinal, NamedTypeSymbol containingType, BoundStatement statement, TypeCompilationState compilationState, SynthesizedSubmissionFields previousSubmissionFields, bool allowOmissionOfConditionalCalls, bool instrumentForDynamicAnalysis, ref ImmutableArray<SourceSpan> dynamicAnalysisSpans, DebugDocumentProvider debugDocumentProvider, BindingDiagnosticBag diagnostics, out bool sawLambdas, out bool sawLocalFunctions, out bool sawAwaitInExceptionHandler)
        {
            try
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(method, statement.Syntax, compilationState, diagnostics);
                DynamicAnalysisInjector dynamicAnalysisInjector = (instrumentForDynamicAnalysis ? DynamicAnalysisInjector.TryCreate(method, statement, syntheticBoundNodeFactory, diagnostics, debugDocumentProvider, Instrumenter.NoOp) : null);
                LocalRewriter localRewriter = new LocalRewriter(compilation, method, methodOrdinal, statement, containingType, syntheticBoundNodeFactory, previousSubmissionFields, allowOmissionOfConditionalCalls, diagnostics, (dynamicAnalysisInjector != null) ? new DebugInfoInjector(dynamicAnalysisInjector) : DebugInfoInjector.Singleton);
                BoundStatement boundStatement = localRewriter.VisitStatement(statement);
                sawLambdas = localRewriter._sawLambdas;
                sawLocalFunctions = localRewriter._availableLocalFunctionOrdinal != 0;
                sawAwaitInExceptionHandler = localRewriter._sawAwaitInExceptionHandler;
                if (localRewriter._needsSpilling && !boundStatement.HasErrors)
                {
                    boundStatement = SpillSequenceSpiller.Rewrite(boundStatement, method, compilationState, diagnostics);
                }
                if (dynamicAnalysisInjector != null)
                {
                    dynamicAnalysisSpans = dynamicAnalysisInjector.DynamicAnalysisSpans;
                }
                return boundStatement;
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                sawLambdas = (sawLocalFunctions = (sawAwaitInExceptionHandler = false));
                return new BoundBadStatement(statement.Syntax, ImmutableArray.Create((BoundNode)statement), hasErrors: true);
            }
        }

        public override BoundNode? Visit(BoundNode? node)
        {
            if (node == null)
            {
                return node;
            }
            if (node is BoundExpression node2)
            {
                return VisitExpressionImpl(node2);
            }
            return node!.Accept(this);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("node")]
        private BoundExpression? VisitExpression(BoundExpression? node)
        {
            if (node == null)
            {
                return node;
            }
            return VisitExpressionImpl(node);
        }

        private BoundStatement? VisitStatement(BoundStatement? node)
        {
            if (node == null)
            {
                return node;
            }
            return (BoundStatement)node!.Accept(this);
        }

        private BoundExpression? VisitExpressionImpl(BoundExpression node)
        {
            ConstantValue constantValue = node.ConstantValue;
            if (constantValue != null)
            {
                TypeSymbol type = node.Type;
                if ((object)type == null || !type.IsNullableType())
                {
                    return MakeLiteral(node.Syntax, constantValue, type);
                }
            }
            BoundExpression boundExpression = VisitExpressionWithStackGuard(node);
            if (boundExpression != null && boundExpression != node && node.Kind != BoundKind.ImplicitReceiver && node.Kind != BoundKind.ObjectOrCollectionValuePlaceholder && !CanBePassedByReference(node) && CanBePassedByReference(boundExpression))
            {
                boundExpression = RefAccessMustMakeCopy(boundExpression);
            }
            return boundExpression;
        }

        private static BoundExpression RefAccessMustMakeCopy(BoundExpression visited)
        {
            visited = new BoundPassByCopy(visited.Syntax, visited, visited.Type);
            return visited;
        }

        private static bool IsUnusedDeconstruction(BoundExpression node)
        {
            if (node.Kind == BoundKind.DeconstructionAssignmentOperator)
            {
                return !((BoundDeconstructionAssignmentOperator)node).IsUsed;
            }
            return false;
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            _sawLambdas = true;
            LambdaSymbol symbol = node.Symbol;
            CheckRefReadOnlySymbols(symbol);
            MethodSymbol currentFunction = _factory.CurrentFunction;
            Instrumenter instrumenter = _instrumenter;
            try
            {
                _factory.CurrentFunction = symbol;
                if (symbol.IsDirectlyExcludedFromCodeCoverage)
                {
                    _instrumenter = RemoveDynamicAnalysisInjectors(instrumenter);
                }
                return base.VisitLambda(node);
            }
            finally
            {
                _factory.CurrentFunction = currentFunction;
                _instrumenter = instrumenter;
            }
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            int localFunctionOrdinal = _availableLocalFunctionOrdinal++;
            LocalFunctionSymbol symbol = node.Symbol;
            CheckRefReadOnlySymbols(symbol);
            PEModuleBuilder moduleBuilderOpt = _factory.CompilationState.ModuleBuilderOpt;
            if (moduleBuilderOpt != null)
            {
                ImmutableArray<TypeParameterSymbol> typeParameters = symbol.TypeParameters;
                if (typeParameters.Any((TypeParameterSymbol typeParameter) => typeParameter.HasUnmanagedTypeConstraint))
                {
                    moduleBuilderOpt.EnsureIsUnmanagedAttributeExists();
                }
                if (hasReturnTypeOrParameter(symbol, (TypeWithAnnotations t) => t.ContainsNativeInteger()) || typeParameters.Any((TypeParameterSymbol t) => t.ConstraintTypesNoUseSiteDiagnostics.Any((TypeWithAnnotations t) => t.ContainsNativeInteger())))
                {
                    moduleBuilderOpt.EnsureNativeIntegerAttributeExists();
                }
                if (_factory.CompilationState.Compilation.ShouldEmitNullableAttributes(symbol) && (typeParameters.Any((TypeParameterSymbol typeParameter) => ((SourceTypeParameterSymbolBase)typeParameter).ConstraintsNeedNullableAttribute()) || hasReturnTypeOrParameter(symbol, (TypeWithAnnotations t) => t.NeedsNullableAttribute())))
                {
                    moduleBuilderOpt.EnsureNullableAttributeExists();
                }
            }
            MethodSymbol currentFunction = _factory.CurrentFunction;
            Instrumenter instrumenter = _instrumenter;
            LoweredDynamicOperationFactory dynamicFactory = _dynamicFactory;
            try
            {
                _factory.CurrentFunction = symbol;
                if (symbol.IsDirectlyExcludedFromCodeCoverage)
                {
                    _instrumenter = RemoveDynamicAnalysisInjectors(instrumenter);
                }
                if (symbol.IsGenericMethod)
                {
                    _dynamicFactory = new LoweredDynamicOperationFactory(_factory, _dynamicFactory.MethodOrdinal, localFunctionOrdinal);
                }
                return base.VisitLocalFunctionStatement(node);
            }
            finally
            {
                _factory.CurrentFunction = currentFunction;
                _instrumenter = instrumenter;
                _dynamicFactory = dynamicFactory;
            }
            static bool hasReturnTypeOrParameter(LocalFunctionSymbol localFunction, Func<TypeWithAnnotations, bool> predicate)
            {
                if (!predicate(localFunction.ReturnTypeWithAnnotations))
                {
                    return localFunction.ParameterTypesWithAnnotations.Any(predicate);
                }
                return true;
            }
        }

        private static Instrumenter RemoveDynamicAnalysisInjectors(Instrumenter instrumenter)
        {
            if (instrumenter is DynamicAnalysisInjector dynamicAnalysisInjector)
            {
                Instrumenter previous = dynamicAnalysisInjector.Previous;
                return RemoveDynamicAnalysisInjectors(previous);
            }
            if (instrumenter is DebugInfoInjector debugInfoInjector)
            {
                Instrumenter previous2 = debugInfoInjector.Previous;
                Instrumenter instrumenter2 = RemoveDynamicAnalysisInjectors(previous2);
                if (instrumenter2 == previous2)
                {
                    return debugInfoInjector;
                }
                if (instrumenter2 == Instrumenter.NoOp)
                {
                    return DebugInfoInjector.Singleton;
                }
                return new DebugInfoInjector(previous2);
            }
            if (instrumenter is CompoundInstrumenter o)
            {
                throw ExceptionUtilities.UnexpectedValue(o);
            }
            return instrumenter;
        }

        public override BoundNode VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            return PlaceholderReplacement(node);
        }

        public override BoundNode VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            if (_inExpressionLambda)
            {
                return node;
            }
            return PlaceholderReplacement(node);
        }

        private BoundExpression PlaceholderReplacement(BoundValuePlaceholderBase placeholder)
        {
            return _placeholderReplacementMapDoNotUseDirectly![placeholder];
        }

        [Conditional("DEBUG")]
        private static void AssertPlaceholderReplacement(BoundValuePlaceholderBase placeholder, BoundExpression value)
        {
        }

        private void AddPlaceholderReplacement(BoundValuePlaceholderBase placeholder, BoundExpression value)
        {
            if (_placeholderReplacementMapDoNotUseDirectly == null)
            {
                _placeholderReplacementMapDoNotUseDirectly = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
            }
            _placeholderReplacementMapDoNotUseDirectly!.Add(placeholder, value);
        }

        private void RemovePlaceholderReplacement(BoundValuePlaceholderBase placeholder)
        {
            _placeholderReplacementMapDoNotUseDirectly!.Remove(placeholder);
        }

        public sealed override BoundNode VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitBadExpression(BoundBadExpression node)
        {
            return node;
        }

        private static BoundExpression BadExpression(BoundExpression node)
        {
            return BadExpression(node.Syntax, node.Type, ImmutableArray.Create(node));
        }

        private static BoundExpression BadExpression(SyntaxNode syntax, TypeSymbol resultType, BoundExpression child)
        {
            return BadExpression(syntax, resultType, ImmutableArray.Create(child));
        }

        private static BoundExpression BadExpression(SyntaxNode syntax, TypeSymbol resultType, BoundExpression child1, BoundExpression child2)
        {
            return BadExpression(syntax, resultType, ImmutableArray.Create(child1, child2));
        }

        private static BoundExpression BadExpression(SyntaxNode syntax, TypeSymbol resultType, ImmutableArray<BoundExpression> children)
        {
            return new BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, children, resultType);
        }

        private bool TryGetWellKnownTypeMember<TSymbol>(SyntaxNode? syntax, WellKnownMember member, out TSymbol symbol, bool isOptional = false, Location? location = null) where TSymbol : Symbol
        {
            CSharpCompilation compilation = _compilation;
            BindingDiagnosticBag diagnostics = _diagnostics;
            bool isOptional2 = isOptional;
            symbol = (TSymbol)Binder.GetWellKnownTypeMember(compilation, member, diagnostics, location, syntax, isOptional2);
            return symbol != null;
        }

        private MethodSymbol UnsafeGetSpecialTypeMethod(SyntaxNode syntax, SpecialMember specialMember)
        {
            return UnsafeGetSpecialTypeMethod(syntax, specialMember, _compilation, _diagnostics);
        }

        private static MethodSymbol UnsafeGetSpecialTypeMethod(SyntaxNode syntax, SpecialMember specialMember, CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
        {
            if (TryGetSpecialTypeMethod(syntax, specialMember, compilation, diagnostics, out var method))
            {
                return method;
            }
            MemberDescriptor descriptor = SpecialMembers.GetDescriptor(specialMember);
            SpecialType type = (SpecialType)descriptor.DeclaringTypeId;
            NamedTypeSymbol specialType = compilation.Assembly.GetSpecialType(type);
            TypeSymbol returnType = new ExtendedErrorTypeSymbol(compilation, descriptor.Name, descriptor.Arity, null);
            return new ErrorMethodSymbol(specialType, returnType, "Missing");
        }

        private bool TryGetSpecialTypeMethod(SyntaxNode syntax, SpecialMember specialMember, out MethodSymbol method)
        {
            return TryGetSpecialTypeMethod(syntax, specialMember, _compilation, _diagnostics, out method);
        }

        private static bool TryGetSpecialTypeMethod(SyntaxNode syntax, SpecialMember specialMember, CSharpCompilation compilation, BindingDiagnosticBag diagnostics, out MethodSymbol method)
        {
            return Binder.TryGetSpecialTypeMember<MethodSymbol>(compilation, specialMember, syntax, diagnostics, out method);
        }

        public override BoundNode VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            BoundTypeExpression sourceType = (BoundTypeExpression)Visit(node.SourceType);
            TypeSymbol type = VisitType(node.Type);
            if (!TryGetWellKnownTypeMember<MethodSymbol>(node.Syntax, WellKnownMember.System_Type__GetTypeFromHandle, out var symbol))
            {
                return new BoundTypeOfOperator(node.Syntax, sourceType, null, type, hasErrors: true);
            }
            return node.Update(sourceType, symbol, type);
        }

        public override BoundNode VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            BoundExpression operand = VisitExpression(node.Operand);
            TypeSymbol type = VisitType(node.Type);
            if (!TryGetWellKnownTypeMember<MethodSymbol>(node.Syntax, WellKnownMember.System_Type__GetTypeFromHandle, out var symbol))
            {
                return new BoundRefTypeOperator(node.Syntax, operand, null, type, hasErrors: true);
            }
            return node.Update(operand, symbol, type);
        }

        public override BoundNode VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node)
        {
            ImmutableArray<BoundStatement> statements = node.Statements;
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance(node.Statements.Length);
            ImmutableArray<BoundStatement>.Enumerator enumerator = statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundStatement current = enumerator.Current;
                if (IsFieldOrPropertyInitializer(current))
                {
                    if (current.Kind == BoundKind.Block)
                    {
                        BoundBlock boundBlock = (BoundBlock)current;
                        BoundStatement item = RewriteExpressionStatement((BoundExpressionStatement)boundBlock.Statements.Single(), suppressInstrumentation: true);
                        instance.Add(boundBlock.Update(boundBlock.Locals, boundBlock.LocalFunctions, ImmutableArray.Create(item)));
                    }
                    else
                    {
                        instance.Add(RewriteExpressionStatement((BoundExpressionStatement)current, suppressInstrumentation: true));
                    }
                }
                else
                {
                    instance.Add(VisitStatement(current));
                }
            }
            int num = 0;
            bool flag = _compilation.Options.OptimizationLevel == OptimizationLevel.Release;
            for (int i = 0; i < instance.Count; i++)
            {
                BoundStatement boundStatement = instance[i];
                if (boundStatement == null || (flag && IsFieldOrPropertyInitializer(statements[i]) && ShouldOptimizeOutInitializer(boundStatement)))
                {
                    num++;
                    MethodSymbol? currentFunction = _factory.CurrentFunction;
                    if ((object)currentFunction != null && !currentFunction!.IsStatic)
                    {
                        instance[i] = null;
                    }
                }
            }
            ImmutableArray<BoundStatement> statements2;
            if (num == instance.Count)
            {
                statements2 = ImmutableArray<BoundStatement>.Empty;
                instance.Free();
            }
            else
            {
                int num2 = 0;
                for (int j = 0; j < instance.Count; j++)
                {
                    BoundStatement boundStatement2 = instance[j];
                    if (boundStatement2 == null)
                    {
                        continue;
                    }
                    if (IsFieldOrPropertyInitializer(statements[j]))
                    {
                        BoundStatement boundStatement3 = statements[j];
                        if (Instrument && !boundStatement3.WasCompilerGenerated)
                        {
                            boundStatement2 = _instrumenter.InstrumentFieldOrPropertyInitializer(boundStatement3, boundStatement2);
                        }
                    }
                    instance[num2] = boundStatement2;
                    num2++;
                }
                instance.Count = num2;
                statements2 = instance.ToImmutableAndFree();
            }
            return new BoundStatementList(node.Syntax, statements2, node.HasErrors);
        }

        public override BoundNode VisitArrayAccess(BoundArrayAccess node)
        {
            if (node.Indices.Length != 1)
            {
                return base.VisitArrayAccess(node);
            }
            TypeSymbol left = VisitType(node.Indices[0].Type);
            SyntheticBoundNodeFactory factory = _factory;
            if (TypeSymbol.Equals(left, _compilation.GetWellKnownType(WellKnownType.System_Index), TypeCompareKind.ConsiderEverything))
            {
                BoundLocal boundLocal = factory.StoreToTemp(VisitExpression(node.Expression), out BoundAssignmentOperator store);
                BoundExpression item = MakePatternIndexOffsetExpression(node.Indices[0], factory.ArrayLength(boundLocal), out bool usedLength);
                return factory.Sequence(ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), factory.ArrayAccess(boundLocal, ImmutableArray.Create(item)));
            }
            if (TypeSymbol.Equals(left, _compilation.GetWellKnownType(WellKnownType.System_Range), TypeCompareKind.ConsiderEverything))
            {
                TypeWithAnnotations elementTypeWithAnnotations = ((ArrayTypeSymbol)node.Expression.Type).ElementTypeWithAnnotations;
                return factory.Call(null, factory.WellKnownMethod(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetSubArray_T).Construct(ImmutableArray.Create(elementTypeWithAnnotations)), ImmutableArray.Create(VisitExpression(node.Expression), VisitExpression(node.Indices[0])));
            }
            return base.VisitArrayAccess(node);
        }

        internal static bool IsFieldOrPropertyInitializer(BoundStatement initializer)
        {
            SyntaxNode syntax = initializer.Syntax;
            if (syntax.IsKind(SyntaxKind.Parameter))
            {
                return true;
            }
            if (syntax is ExpressionSyntax expressionSyntax)
            {
                CSharpSyntaxNode parent = expressionSyntax.Parent;
                if (parent != null && parent.Kind() == SyntaxKind.EqualsValueClause)
                {
                    SyntaxKind syntaxKind = parent.Parent!.Kind();
                    if (syntaxKind == SyntaxKind.VariableDeclarator || syntaxKind == SyntaxKind.PropertyDeclaration)
                    {
                        BoundKind kind = initializer.Kind;
                        if (kind != BoundKind.Block)
                        {
                            if (kind == BoundKind.ExpressionStatement)
                            {
                                goto IL_00a2;
                            }
                        }
                        else
                        {
                            BoundBlock boundBlock = (BoundBlock)initializer;
                            if (boundBlock.Statements.Length == 1)
                            {
                                initializer = boundBlock.Statements.First();
                                if (initializer.Kind == BoundKind.ExpressionStatement)
                                {
                                    goto IL_00a2;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        IL_00a2:
            return ((BoundExpressionStatement)initializer).Expression.Kind == BoundKind.AssignmentOperator;
        }

        private static bool ShouldOptimizeOutInitializer(BoundStatement initializer)
        {
            if (initializer.Kind != BoundKind.ExpressionStatement)
            {
                return false;
            }
            if (!(((BoundExpressionStatement)initializer).Expression is BoundAssignmentOperator boundAssignmentOperator))
            {
                return false;
            }
            FieldSymbol fieldSymbol = ((BoundFieldAccess)boundAssignmentOperator.Left).FieldSymbol;
            if (!fieldSymbol.IsStatic && fieldSymbol.ContainingType.IsStructType())
            {
                return false;
            }
            return boundAssignmentOperator.Right.IsDefaultValue();
        }

        internal static bool CanBePassedByReference(BoundExpression expr)
        {
            if (expr.ConstantValue != null)
            {
                return false;
            }
            switch (expr.Kind)
            {
                case BoundKind.PointerIndirectionOperator:
                case BoundKind.PointerElementAccess:
                case BoundKind.RefValueOperator:
                case BoundKind.ArrayAccess:
                case BoundKind.ThisReference:
                case BoundKind.Local:
                case BoundKind.PseudoVariable:
                case BoundKind.Parameter:
                case BoundKind.DiscardExpression:
                    return true;
                case BoundKind.DeconstructValuePlaceholder:
                    return true;
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)expr;
                        if (boundEventAccess.IsUsableAsField)
                        {
                            if (boundEventAccess.EventSymbol.IsStatic)
                            {
                                return true;
                            }
                            return CanBePassedByReference(boundEventAccess.ReceiverOpt);
                        }
                        return false;
                    }
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
                        if (!boundFieldAccess.FieldSymbol.IsStatic)
                        {
                            return CanBePassedByReference(boundFieldAccess.ReceiverOpt);
                        }
                        return true;
                    }
                case BoundKind.Sequence:
                    return CanBePassedByReference(((BoundSequence)expr).Value);
                case BoundKind.AssignmentOperator:
                    return ((BoundAssignmentOperator)expr).IsRef;
                case BoundKind.ConditionalOperator:
                    return ((BoundConditionalOperator)expr).IsRef;
                case BoundKind.Call:
                    return ((BoundCall)expr).Method.RefKind != RefKind.None;
                case BoundKind.PropertyAccess:
                    return ((BoundPropertyAccess)expr).PropertySymbol.RefKind != RefKind.None;
                case BoundKind.IndexerAccess:
                    return ((BoundIndexerAccess)expr).Indexer.RefKind != RefKind.None;
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    {
                        BoundIndexOrRangePatternIndexerAccess boundIndexOrRangePatternIndexerAccess = (BoundIndexOrRangePatternIndexerAccess)expr;
                        Symbol patternSymbol = boundIndexOrRangePatternIndexerAccess.PatternSymbol;
                        RefKind refKind;
                        if (!(patternSymbol is PropertySymbol propertySymbol))
                        {
                            if (!(patternSymbol is MethodSymbol methodSymbol))
                            {
                                throw ExceptionUtilities.UnexpectedValue(boundIndexOrRangePatternIndexerAccess.PatternSymbol);
                            }
                            refKind = methodSymbol.RefKind;
                        }
                        else
                        {
                            refKind = propertySymbol.RefKind;
                        }
                        return refKind != RefKind.None;
                    }
                default:
                    return false;
            }
        }

        private void CheckRefReadOnlySymbols(MethodSymbol symbol)
        {
            if (symbol.ReturnsByRefReadonly || symbol.Parameters.Any((ParameterSymbol p) => p.RefKind == RefKind.In))
            {
                _factory.CompilationState.ModuleBuilderOpt?.EnsureIsReadOnlyAttributeExists();
            }
        }

        private CompoundUseSiteInfo<AssemblySymbol> GetNewCompoundUseSiteInfo()
        {
            return new CompoundUseSiteInfo<AssemblySymbol>(_diagnostics, _compilation.Assembly);
        }

        public override BoundNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            return new BoundObjectCreationExpression(node.Syntax, node.Constructor, arguments, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), null, null, node.Type);
        }

        public override BoundNode VisitAsOperator(BoundAsOperator node)
        {
            BoundExpression rewrittenOperand = VisitExpression(node.Operand);
            BoundTypeExpression rewrittenTargetType = (BoundTypeExpression)VisitTypeExpression(node.TargetType);
            TypeSymbol rewrittenType = VisitType(node.Type);
            return MakeAsOperator(node, node.Syntax, rewrittenOperand, rewrittenTargetType, node.Conversion, rewrittenType);
        }

        public override BoundNode VisitTypeExpression(BoundTypeExpression node)
        {
            return base.VisitTypeExpression(node);
        }

        private BoundExpression MakeAsOperator(BoundAsOperator oldNode, SyntaxNode syntax, BoundExpression rewrittenOperand, BoundTypeExpression rewrittenTargetType, Conversion conversion, TypeSymbol rewrittenType)
        {
            if (!_inExpressionLambda)
            {
                ConstantValue asOperatorConstantResult = Binder.GetAsOperatorConstantResult(rewrittenOperand.Type, rewrittenType, conversion.Kind, rewrittenOperand.ConstantValue);
                if (asOperatorConstantResult != null)
                {
                    BoundExpression boundExpression = (rewrittenType.IsNullableType() ? new BoundDefaultExpression(syntax, rewrittenType) : MakeLiteral(syntax, asOperatorConstantResult, rewrittenType));
                    if (rewrittenOperand.ConstantValue != null)
                    {
                        return boundExpression;
                    }
                    return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(rewrittenOperand), boundExpression, rewrittenType);
                }
                if (conversion.IsImplicit)
                {
                    return MakeConversionNode(syntax, rewrittenOperand, conversion, rewrittenType, @checked: false);
                }
            }
            return oldNode.Update(rewrittenOperand, rewrittenTargetType, conversion, rewrittenType);
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            return VisitAssignmentOperator(node, used: true);
        }

        private BoundExpression VisitAssignmentOperator(BoundAssignmentOperator node, bool used)
        {
            BoundExpression boundExpression = VisitExpression(node.Right);
            BoundExpression left = node.Left;
            BoundExpression rewrittenLeft;
            switch (left.Kind)
            {
                case BoundKind.PropertyAccess:
                    rewrittenLeft = VisitPropertyAccess((BoundPropertyAccess)left, isLeftOfAssignment: true);
                    break;
                case BoundKind.IndexerAccess:
                    rewrittenLeft = VisitIndexerAccess((BoundIndexerAccess)left, isLeftOfAssignment: true);
                    break;
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    rewrittenLeft = VisitIndexOrRangePatternIndexerAccess((BoundIndexOrRangePatternIndexerAccess)left, isLeftOfAssignment: true);
                    break;
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)left;
                        if (boundEventAccess.EventSymbol.IsWindowsRuntimeEvent)
                        {
                            return VisitWindowsRuntimeEventFieldAssignmentOperator(node.Syntax, boundEventAccess, boundExpression);
                        }
                        goto default;
                    }
                case BoundKind.DynamicMemberAccess:
                    {
                        BoundDynamicMemberAccess boundDynamicMemberAccess = (BoundDynamicMemberAccess)left;
                        BoundExpression loweredReceiver2 = VisitExpression(boundDynamicMemberAccess.Receiver);
                        return _dynamicFactory.MakeDynamicSetMember(loweredReceiver2, boundDynamicMemberAccess.Name, boundExpression).ToExpression();
                    }
                case BoundKind.DynamicIndexerAccess:
                    {
                        BoundDynamicIndexerAccess boundDynamicIndexerAccess = (BoundDynamicIndexerAccess)left;
                        BoundExpression loweredReceiver = VisitExpression(boundDynamicIndexerAccess.Receiver);
                        ImmutableArray<BoundExpression> loweredArguments = VisitList(boundDynamicIndexerAccess.Arguments);
                        return MakeDynamicSetIndex(boundDynamicIndexerAccess, loweredReceiver, loweredArguments, boundDynamicIndexerAccess.ArgumentNamesOpt, boundDynamicIndexerAccess.ArgumentRefKindsOpt, boundExpression);
                    }
                default:
                    rewrittenLeft = VisitExpression(left);
                    break;
            }
            return MakeStaticAssignmentOperator(node.Syntax, rewrittenLeft, boundExpression, node.IsRef, node.Type, used);
        }

        private BoundExpression MakeAssignmentOperator(SyntaxNode syntax, BoundExpression rewrittenLeft, BoundExpression rewrittenRight, TypeSymbol type, bool used, bool isChecked, bool isCompoundAssignment)
        {
            switch (rewrittenLeft.Kind)
            {
                case BoundKind.DynamicIndexerAccess:
                    {
                        BoundDynamicIndexerAccess boundDynamicIndexerAccess = (BoundDynamicIndexerAccess)rewrittenLeft;
                        return MakeDynamicSetIndex(boundDynamicIndexerAccess, boundDynamicIndexerAccess.Receiver, boundDynamicIndexerAccess.Arguments, boundDynamicIndexerAccess.ArgumentNamesOpt, boundDynamicIndexerAccess.ArgumentRefKindsOpt, rewrittenRight, isCompoundAssignment, isChecked);
                    }
                case BoundKind.DynamicMemberAccess:
                    {
                        BoundDynamicMemberAccess boundDynamicMemberAccess = (BoundDynamicMemberAccess)rewrittenLeft;
                        return _dynamicFactory.MakeDynamicSetMember(boundDynamicMemberAccess.Receiver, boundDynamicMemberAccess.Name, rewrittenRight, isCompoundAssignment, isChecked).ToExpression();
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)rewrittenLeft;
                        if (boundEventAccess.EventSymbol.IsWindowsRuntimeEvent)
                        {
                            return RewriteWindowsRuntimeEventAssignmentOperator(boundEventAccess.Syntax, boundEventAccess.EventSymbol, EventAssignmentKind.Assignment, isDynamic: false, boundEventAccess.ReceiverOpt, rewrittenRight);
                        }
                        throw ExceptionUtilities.Unreachable;
                    }
                default:
                    return MakeStaticAssignmentOperator(syntax, rewrittenLeft, rewrittenRight, isRef: false, type, used);
            }
        }

        private BoundExpression MakeDynamicSetIndex(BoundDynamicIndexerAccess indexerAccess, BoundExpression loweredReceiver, ImmutableArray<BoundExpression> loweredArguments, ImmutableArray<string> argumentNames, ImmutableArray<RefKind> refKinds, BoundExpression loweredRight, bool isCompoundAssignment = false, bool isChecked = false)
        {
            EmbedIfNeedTo(loweredReceiver, indexerAccess.ApplicableIndexers, indexerAccess.Syntax);
            return _dynamicFactory.MakeDynamicSetIndex(MakeDynamicIndexerAccessReceiver(indexerAccess, loweredReceiver), loweredArguments, argumentNames, refKinds, loweredRight, isCompoundAssignment, isChecked).ToExpression();
        }

        private BoundExpression MakeStaticAssignmentOperator(SyntaxNode syntax, BoundExpression rewrittenLeft, BoundExpression rewrittenRight, bool isRef, TypeSymbol type, bool used)
        {
            switch (rewrittenLeft.Kind)
            {
                case BoundKind.DynamicMemberAccess:
                case BoundKind.DynamicIndexerAccess:
                    throw ExceptionUtilities.UnexpectedValue(rewrittenLeft.Kind);
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess obj = (BoundPropertyAccess)rewrittenLeft;
                        BoundExpression receiverOpt2 = obj.ReceiverOpt;
                        PropertySymbol propertySymbol = obj.PropertySymbol;
                        return MakePropertyAssignment(syntax, receiverOpt2, propertySymbol, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), rewrittenRight, type, used);
                    }
                case BoundKind.IndexerAccess:
                    {
                        BoundIndexerAccess boundIndexerAccess = (BoundIndexerAccess)rewrittenLeft;
                        BoundExpression receiverOpt = boundIndexerAccess.ReceiverOpt;
                        ImmutableArray<BoundExpression> arguments = boundIndexerAccess.Arguments;
                        PropertySymbol indexer = boundIndexerAccess.Indexer;
                        return MakePropertyAssignment(syntax, receiverOpt, indexer, arguments, boundIndexerAccess.ArgumentRefKindsOpt, boundIndexerAccess.Expanded, boundIndexerAccess.ArgsToParamsOpt, rewrittenRight, type, used);
                    }
                case BoundKind.Local:
                    return new BoundAssignmentOperator(syntax, rewrittenLeft, rewrittenRight, type, isRef);
                case BoundKind.Parameter:
                    return new BoundAssignmentOperator(syntax, rewrittenLeft, rewrittenRight, isRef, type);
                case BoundKind.DiscardExpression:
                    return rewrittenRight;
                case BoundKind.Sequence:
                    {
                        BoundSequence boundSequence = (BoundSequence)rewrittenLeft;
                        if (boundSequence.Value.Kind == BoundKind.IndexerAccess)
                        {
                            return boundSequence.Update(boundSequence.Locals, boundSequence.SideEffects, MakeStaticAssignmentOperator(syntax, boundSequence.Value, rewrittenRight, isRef, type, used), type);
                        }
                        break;
                    }
            }
            return new BoundAssignmentOperator(syntax, rewrittenLeft, rewrittenRight, type);
        }

        private BoundExpression MakePropertyAssignment(SyntaxNode syntax, BoundExpression? rewrittenReceiver, PropertySymbol property, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BoundExpression rewrittenRight, TypeSymbol type, bool used)
        {
            MethodSymbol ownOrInheritedSetMethod = property.GetOwnOrInheritedSetMethod();
            if ((object)ownOrInheritedSetMethod == null)
            {
                SynthesizedBackingFieldSymbol backingField = ((SourcePropertySymbolBase)property.OriginalDefinition).BackingField;
                return _factory.AssignmentExpression(_factory.Field(rewrittenReceiver, backingField), rewrittenRight);
            }
            rewrittenArguments = MakeArguments(syntax, rewrittenArguments, property, expanded, argsToParamsOpt, ref argumentRefKindsOpt, out var temps, invokedAsExtensionMethod: false, ThreeState.True);
            if (used)
            {
                TypeSymbol type2 = rewrittenRight.Type;
                LocalSymbol localSymbol = _factory.SynthesizedLocal(type2);
                BoundExpression boundExpression = new BoundLocal(syntax, localSymbol, null, type2);
                BoundExpression newElement = new BoundAssignmentOperator(syntax, boundExpression, rewrittenRight, type2);
                BoundExpression item = BoundCall.Synthesized(syntax, rewrittenReceiver, ownOrInheritedSetMethod, AppendToPossibleNull(rewrittenArguments, newElement));
                return new BoundSequence(syntax, AppendToPossibleNull(temps, localSymbol), ImmutableArray.Create(item), boundExpression, type);
            }
            BoundCall boundCall = BoundCall.Synthesized(syntax, rewrittenReceiver, ownOrInheritedSetMethod, AppendToPossibleNull(rewrittenArguments, rewrittenRight));
            if (temps.IsDefaultOrEmpty)
            {
                return boundCall;
            }
            return new BoundSequence(syntax, temps, ImmutableArray<BoundExpression>.Empty, boundCall, ownOrInheritedSetMethod.ReturnType);
        }

        private static ImmutableArray<T> AppendToPossibleNull<T>(ImmutableArray<T> possibleNull, T newElement) where T : notnull
        {
            return possibleNull.NullToEmpty().Add(newElement);
        }

        public override BoundNode VisitAwaitExpression(BoundAwaitExpression node)
        {
            return VisitAwaitExpression(node, used: true);
        }

        public BoundExpression VisitAwaitExpression(BoundAwaitExpression node, bool used)
        {
            return RewriteAwaitExpression((BoundExpression)base.VisitAwaitExpression(node), used);
        }

        private BoundExpression RewriteAwaitExpression(SyntaxNode syntax, BoundExpression rewrittenExpression, BoundAwaitableInfo awaitableInfo, TypeSymbol type, bool used)
        {
            return RewriteAwaitExpression(new BoundAwaitExpression(syntax, rewrittenExpression, awaitableInfo, type)
            {
                WasCompilerGenerated = true
            }, used);
        }

        private BoundExpression RewriteAwaitExpression(BoundExpression rewrittenAwait, bool used)
        {
            _sawAwait = true;
            if (!used)
            {
                return rewrittenAwait;
            }
            _needsSpilling = true;
            BoundLocal boundLocal = _factory.StoreToTemp(rewrittenAwait, out BoundAssignmentOperator store, RefKind.None, SynthesizedLocalKind.Spill, rewrittenAwait.Syntax);
            return new BoundSpillSequence(rewrittenAwait.Syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), boundLocal, boundLocal.Type);
        }

        public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
        {
            return VisitBinaryOperator(node, null);
        }

        public override BoundNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            SyntaxNode syntax = node.Syntax;
            BinaryOperatorKind operatorKind = node.OperatorKind;
            TypeSymbol type = node.Type;
            BoundExpression boundExpression = VisitExpression(node.Left);
            BoundExpression boundExpression2 = VisitExpression(node.Right);
            if (_inExpressionLambda)
            {
                return node.Update(operatorKind, node.LogicalOperator, node.TrueOperator, node.FalseOperator, node.ResultKind, boundExpression, boundExpression2, type);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store);
            BoundCall rewrittenCondition = BoundCall.Synthesized(syntax, null, (operatorKind.Operator() == BinaryOperatorKind.And) ? node.FalseOperator : node.TrueOperator, boundLocal);
            BoundExpression rewrittenAlternative = LowerUserDefinedBinaryOperator(syntax, operatorKind & ~BinaryOperatorKind.Logical, boundLocal, boundExpression2, type, node.LogicalOperator);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, boundLocal, rewrittenAlternative, null, type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, type);
        }

        public BoundExpression VisitBinaryOperator(BoundBinaryOperator node, BoundUnaryOperator? applyParentUnaryOperator)
        {
            ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
            BoundBinaryOperator boundBinaryOperator = node;
            while (boundBinaryOperator != null && boundBinaryOperator.ConstantValue == null)
            {
                instance.Push(boundBinaryOperator);
                boundBinaryOperator = boundBinaryOperator.Left as BoundBinaryOperator;
            }
            BoundExpression boundExpression = VisitExpression(instance.Peek().Left);
            while (instance.Count > 0)
            {
                BoundBinaryOperator boundBinaryOperator2 = instance.Pop();
                BoundExpression loweredRight = VisitExpression(boundBinaryOperator2.Right);
                boundExpression = MakeBinaryOperator(boundBinaryOperator2, boundBinaryOperator2.Syntax, boundBinaryOperator2.OperatorKind, boundExpression, loweredRight, boundBinaryOperator2.Type, boundBinaryOperator2.MethodOpt, isPointerElementAccess: false, isCompoundAssignment: false, (instance.Count == 0) ? applyParentUnaryOperator : null);
            }
            instance.Free();
            return boundExpression;
        }

        private BoundExpression MakeBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, MethodSymbol? method, bool isPointerElementAccess = false, bool isCompoundAssignment = false, BoundUnaryOperator? applyParentUnaryOperator = null)
        {
            return MakeBinaryOperator(null, syntax, operatorKind, loweredLeft, loweredRight, type, method, isPointerElementAccess, isCompoundAssignment, applyParentUnaryOperator);
        }

        private BoundExpression MakeBinaryOperator(BoundBinaryOperator? oldNode, SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, MethodSymbol? method, bool isPointerElementAccess = false, bool isCompoundAssignment = false, BoundUnaryOperator? applyParentUnaryOperator = null)
        {
            if (_inExpressionLambda)
            {
                switch (operatorKind.Operator() | operatorKind.OperandTypes())
                {
                    case BinaryOperatorKind.StringConcatenation:
                    case BinaryOperatorKind.StringAndObjectConcatenation:
                    case BinaryOperatorKind.ObjectAndStringConcatenation:
                        return RewriteStringConcatenation(syntax, operatorKind, loweredLeft, loweredRight, type);
                    case BinaryOperatorKind.DelegateCombination:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__Combine);
                    case BinaryOperatorKind.DelegateRemoval:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__Remove);
                    case BinaryOperatorKind.DelegateEqual:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__op_Equality);
                    case BinaryOperatorKind.DelegateNotEqual:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__op_Inequality);
                }
            }
            else
            {
                if (operatorKind.IsDynamic())
                {
                    if (operatorKind.IsLogical())
                    {
                        return MakeDynamicLogicalBinaryOperator(syntax, operatorKind, loweredLeft, loweredRight, method, type, isCompoundAssignment, applyParentUnaryOperator);
                    }
                    return _dynamicFactory.MakeDynamicBinaryOperator(operatorKind, loweredLeft, loweredRight, isCompoundAssignment, type).ToExpression();
                }
                if (operatorKind.IsLifted())
                {
                    return RewriteLiftedBinaryOperator(syntax, operatorKind, loweredLeft, loweredRight, type, method);
                }
                if (operatorKind.IsUserDefined())
                {
                    return LowerUserDefinedBinaryOperator(syntax, operatorKind, loweredLeft, loweredRight, type, method);
                }
                switch (operatorKind.OperatorWithLogical() | operatorKind.OperandTypes())
                {
                    case BinaryOperatorKind.NullableNullEqual:
                    case BinaryOperatorKind.NullableNullNotEqual:
                        return RewriteNullableNullEquality(syntax, operatorKind, loweredLeft, loweredRight, type);
                    case BinaryOperatorKind.StringConcatenation:
                    case BinaryOperatorKind.StringAndObjectConcatenation:
                    case BinaryOperatorKind.ObjectAndStringConcatenation:
                        return RewriteStringConcatenation(syntax, operatorKind, loweredLeft, loweredRight, type);
                    case BinaryOperatorKind.StringEqual:
                        return RewriteStringEquality(oldNode, syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_String__op_Equality);
                    case BinaryOperatorKind.StringNotEqual:
                        return RewriteStringEquality(oldNode, syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_String__op_Inequality);
                    case BinaryOperatorKind.DelegateCombination:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__Combine);
                    case BinaryOperatorKind.DelegateRemoval:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__Remove);
                    case BinaryOperatorKind.DelegateEqual:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__op_Equality);
                    case BinaryOperatorKind.DelegateNotEqual:
                        return RewriteDelegateOperation(syntax, operatorKind, loweredLeft, loweredRight, type, SpecialMember.System_Delegate__op_Inequality);
                    case BinaryOperatorKind.LogicalBoolAnd:
                        if (loweredRight.ConstantValue == ConstantValue.True)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return loweredRight;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return loweredLeft;
                        }
                        if (loweredRight.Kind == BoundKind.Local || loweredRight.Kind == BoundKind.Parameter)
                        {
                            operatorKind &= ~BinaryOperatorKind.Logical;
                        }
                        break;
                    case BinaryOperatorKind.LogicalBoolOr:
                        if (loweredRight.ConstantValue == ConstantValue.False)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return loweredRight;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return loweredLeft;
                        }
                        if (loweredRight.Kind == BoundKind.Local || loweredRight.Kind == BoundKind.Parameter)
                        {
                            operatorKind &= ~BinaryOperatorKind.Logical;
                        }
                        break;
                    case BinaryOperatorKind.BoolAnd:
                        if (loweredRight.ConstantValue == ConstantValue.True)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return loweredRight;
                        }
                        if (loweredLeft.IsDefaultValue())
                        {
                            return _factory.MakeSequence(loweredRight, loweredLeft);
                        }
                        if (loweredRight.IsDefaultValue())
                        {
                            return _factory.MakeSequence(loweredLeft, loweredRight);
                        }
                        break;
                    case BinaryOperatorKind.BoolOr:
                        if (loweredRight.ConstantValue == ConstantValue.False)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return loweredRight;
                        }
                        break;
                    case BinaryOperatorKind.BoolEqual:
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return loweredRight;
                        }
                        if (loweredRight.ConstantValue == ConstantValue.True)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredRight, loweredRight.Type);
                        }
                        if (loweredRight.ConstantValue == ConstantValue.False)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredLeft, loweredLeft.Type);
                        }
                        break;
                    case BinaryOperatorKind.BoolNotEqual:
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return loweredRight;
                        }
                        if (loweredRight.ConstantValue == ConstantValue.False)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredRight, loweredRight.Type);
                        }
                        if (loweredRight.ConstantValue == ConstantValue.True)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredLeft, loweredLeft.Type);
                        }
                        break;
                    case BinaryOperatorKind.BoolXor:
                        if (loweredLeft.ConstantValue == ConstantValue.False)
                        {
                            return loweredRight;
                        }
                        if (loweredRight.ConstantValue == ConstantValue.False)
                        {
                            return loweredLeft;
                        }
                        if (loweredLeft.ConstantValue == ConstantValue.True)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredRight, loweredRight.Type);
                        }
                        if (loweredRight.ConstantValue == ConstantValue.True)
                        {
                            return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredLeft, loweredLeft.Type);
                        }
                        break;
                    case BinaryOperatorKind.IntLeftShift:
                    case BinaryOperatorKind.UIntLeftShift:
                    case BinaryOperatorKind.IntRightShift:
                    case BinaryOperatorKind.UIntRightShift:
                        return RewriteBuiltInShiftOperation(oldNode, syntax, operatorKind, loweredLeft, loweredRight, type, 31);
                    case BinaryOperatorKind.LongLeftShift:
                    case BinaryOperatorKind.ULongLeftShift:
                    case BinaryOperatorKind.LongRightShift:
                    case BinaryOperatorKind.ULongRightShift:
                        return RewriteBuiltInShiftOperation(oldNode, syntax, operatorKind, loweredLeft, loweredRight, type, 63);
                    case BinaryOperatorKind.DecimalMultiplication:
                    case BinaryOperatorKind.DecimalAddition:
                    case BinaryOperatorKind.DecimalSubtraction:
                    case BinaryOperatorKind.DecimalDivision:
                    case BinaryOperatorKind.DecimalRemainder:
                    case BinaryOperatorKind.DecimalEqual:
                    case BinaryOperatorKind.DecimalNotEqual:
                    case BinaryOperatorKind.DecimalGreaterThan:
                    case BinaryOperatorKind.DecimalLessThan:
                    case BinaryOperatorKind.DecimalGreaterThanOrEqual:
                    case BinaryOperatorKind.DecimalLessThanOrEqual:
                        return RewriteDecimalBinaryOperation(syntax, loweredLeft, loweredRight, operatorKind);
                    case BinaryOperatorKind.PointerAndIntAddition:
                    case BinaryOperatorKind.PointerAndUIntAddition:
                    case BinaryOperatorKind.PointerAndLongAddition:
                    case BinaryOperatorKind.PointerAndULongAddition:
                    case BinaryOperatorKind.PointerAndIntSubtraction:
                    case BinaryOperatorKind.PointerAndUIntSubtraction:
                    case BinaryOperatorKind.PointerAndLongSubtraction:
                    case BinaryOperatorKind.PointerAndULongSubtraction:
                        if (loweredRight.IsDefaultValue())
                        {
                            return loweredLeft;
                        }
                        return RewritePointerNumericOperator(syntax, operatorKind, loweredLeft, loweredRight, type, isPointerElementAccess, isLeftPointer: true);
                    case BinaryOperatorKind.IntAndPointerAddition:
                    case BinaryOperatorKind.UIntAndPointerAddition:
                    case BinaryOperatorKind.LongAndPointerAddition:
                    case BinaryOperatorKind.ULongAndPointerAddition:
                        if (loweredLeft.IsDefaultValue())
                        {
                            return loweredRight;
                        }
                        return RewritePointerNumericOperator(syntax, operatorKind, loweredLeft, loweredRight, type, isPointerElementAccess, isLeftPointer: false);
                    case BinaryOperatorKind.PointerSubtraction:
                        return RewritePointerSubtraction(operatorKind, loweredLeft, loweredRight, type);
                    case BinaryOperatorKind.IntAddition:
                    case BinaryOperatorKind.UIntAddition:
                    case BinaryOperatorKind.LongAddition:
                    case BinaryOperatorKind.ULongAddition:
                        if (loweredLeft.IsDefaultValue())
                        {
                            return loweredRight;
                        }
                        if (loweredRight.IsDefaultValue())
                        {
                            return loweredLeft;
                        }
                        break;
                    case BinaryOperatorKind.IntSubtraction:
                    case BinaryOperatorKind.UIntSubtraction:
                    case BinaryOperatorKind.LongSubtraction:
                    case BinaryOperatorKind.ULongSubtraction:
                        if (loweredRight.IsDefaultValue())
                        {
                            return loweredLeft;
                        }
                        break;
                    case BinaryOperatorKind.IntMultiplication:
                    case BinaryOperatorKind.UIntMultiplication:
                    case BinaryOperatorKind.LongMultiplication:
                    case BinaryOperatorKind.ULongMultiplication:
                        {
                            if (loweredLeft.IsDefaultValue())
                            {
                                return _factory.MakeSequence(loweredRight, loweredLeft);
                            }
                            if (loweredRight.IsDefaultValue())
                            {
                                return _factory.MakeSequence(loweredLeft, loweredRight);
                            }
                            ConstantValue? constantValue = loweredLeft.ConstantValue;
                            if ((object)constantValue != null && constantValue!.UInt64Value == 1)
                            {
                                return loweredRight;
                            }
                            ConstantValue? constantValue2 = loweredRight.ConstantValue;
                            if ((object)constantValue2 != null && constantValue2!.UInt64Value == 1)
                            {
                                return loweredLeft;
                            }
                            break;
                        }
                    case BinaryOperatorKind.IntGreaterThan:
                    case BinaryOperatorKind.IntLessThanOrEqual:
                        if (loweredLeft.Kind == BoundKind.ArrayLength && loweredRight.IsDefaultValue())
                        {
                            BinaryOperatorKind binaryOperatorKind2 = ((operatorKind == BinaryOperatorKind.IntGreaterThan) ? BinaryOperatorKind.NotEqual : BinaryOperatorKind.Equal);
                            operatorKind &= ~BinaryOperatorKind.OpMask;
                            operatorKind |= binaryOperatorKind2;
                            loweredLeft = UnconvertArrayLength((BoundArrayLength)loweredLeft);
                        }
                        break;
                    case BinaryOperatorKind.IntLessThan:
                    case BinaryOperatorKind.IntGreaterThanOrEqual:
                        if (loweredRight.Kind == BoundKind.ArrayLength && loweredLeft.IsDefaultValue())
                        {
                            BinaryOperatorKind binaryOperatorKind = ((operatorKind == BinaryOperatorKind.IntLessThan) ? BinaryOperatorKind.NotEqual : BinaryOperatorKind.Equal);
                            operatorKind &= ~BinaryOperatorKind.OpMask;
                            operatorKind |= binaryOperatorKind;
                            loweredRight = UnconvertArrayLength((BoundArrayLength)loweredRight);
                        }
                        break;
                    case BinaryOperatorKind.IntEqual:
                    case BinaryOperatorKind.IntNotEqual:
                        if (loweredLeft.Kind == BoundKind.ArrayLength && loweredRight.IsDefaultValue())
                        {
                            loweredLeft = UnconvertArrayLength((BoundArrayLength)loweredLeft);
                        }
                        else if (loweredRight.Kind == BoundKind.ArrayLength && loweredLeft.IsDefaultValue())
                        {
                            loweredRight = UnconvertArrayLength((BoundArrayLength)loweredRight);
                        }
                        break;
                }
            }
            if (oldNode == null)
            {
                return new BoundBinaryOperator(syntax, operatorKind, null, null, LookupResultKind.Viable, loweredLeft, loweredRight, type);
            }
            return oldNode!.Update(operatorKind, oldNode!.ConstantValueOpt, oldNode!.MethodOpt, oldNode!.ResultKind, loweredLeft, loweredRight, type);
        }

        private BoundExpression RewriteLiftedBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, MethodSymbol? method)
        {
            BoundLoweredConditionalAccess boundLoweredConditionalAccess = loweredLeft as BoundLoweredConditionalAccess;
            int num;
            if (boundLoweredConditionalAccess != null && operatorKind != BinaryOperatorKind.LiftedBoolOr && operatorKind != BinaryOperatorKind.LiftedBoolAnd && !ReadIsSideeffecting(loweredRight))
            {
                if (boundLoweredConditionalAccess.WhenNullOpt != null)
                {
                    num = (boundLoweredConditionalAccess.WhenNullOpt.IsDefaultValue() ? 1 : 0);
                    if (num == 0)
                    {
                        goto IL_0047;
                    }
                }
                else
                {
                    num = 1;
                }
                loweredLeft = boundLoweredConditionalAccess.WhenNotNull;
            }
            else
            {
                num = 0;
            }
            goto IL_0047;
        IL_0047:
            BoundExpression boundExpression = ((!operatorKind.IsComparison()) ? LowerLiftedBinaryArithmeticOperator(syntax, operatorKind, loweredLeft, loweredRight, type, method) : (operatorKind.IsUserDefined() ? LowerLiftedUserDefinedComparisonOperator(syntax, operatorKind, loweredLeft, loweredRight, method) : LowerLiftedBuiltInComparisonOperator(syntax, operatorKind, loweredLeft, loweredRight)));
            if (num != 0)
            {
                BoundExpression whenNullOpt = null;
                if (operatorKind.Operator() == BinaryOperatorKind.NotEqual || operatorKind.Operator() == BinaryOperatorKind.Equal)
                {
                    whenNullOpt = RewriteLiftedBinaryOperator(syntax, operatorKind, _factory.Default(loweredLeft.Type), loweredRight, type, method);
                }
                boundExpression = boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.Receiver, boundLoweredConditionalAccess.HasValueMethodOpt, boundExpression, whenNullOpt, boundLoweredConditionalAccess.Id, boundExpression.Type);
            }
            return boundExpression;
        }

        private BoundExpression UnconvertArrayLength(BoundArrayLength arrLength)
        {
            return arrLength.Update(arrLength.Expression, _factory.SpecialType(SpecialType.System_UIntPtr));
        }

        private BoundExpression MakeDynamicLogicalBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, MethodSymbol? leftTruthOperator, TypeSymbol type, bool isCompoundAssignment, BoundUnaryOperator? applyParentUnaryOperator)
        {
            bool flag = operatorKind.Operator() == BinaryOperatorKind.And;
            UnaryOperatorKind unaryOperatorKind = (flag ? UnaryOperatorKind.DynamicFalse : UnaryOperatorKind.DynamicTrue);
            ConstantValue constantValue = loweredLeft.ConstantValue ?? UnboxConstant(loweredLeft);
            if ((unaryOperatorKind == UnaryOperatorKind.DynamicFalse && constantValue == ConstantValue.False) || (unaryOperatorKind == UnaryOperatorKind.DynamicTrue && constantValue == ConstantValue.True))
            {
                if (applyParentUnaryOperator != null)
                {
                    return _factory.Literal(value: true);
                }
                return MakeConversionNode(loweredLeft, type, @checked: false);
            }
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundAssignmentOperator boundAssignmentOperator;
            BoundLocal boundLocal;
            if (constantValue == null && loweredLeft.Kind != BoundKind.Local && loweredLeft.Kind != BoundKind.Parameter)
            {
                BoundExpression boundExpression = (loweredLeft = _factory.StoreToTemp(loweredLeft, out BoundAssignmentOperator store));
                boundAssignmentOperator = store;
                boundLocal = (BoundLocal)boundExpression;
            }
            else
            {
                boundAssignmentOperator = null;
                boundLocal = null;
            }
            BoundExpression boundExpression2 = _dynamicFactory.MakeDynamicBinaryOperator(operatorKind, loweredLeft, loweredRight, isCompoundAssignment, type).ToExpression();
            bool flag2 = (unaryOperatorKind == UnaryOperatorKind.DynamicFalse && constantValue == ConstantValue.True) || (unaryOperatorKind == UnaryOperatorKind.DynamicTrue && constantValue == ConstantValue.False);
            BoundExpression boundExpression3;
            if (applyParentUnaryOperator != null)
            {
                boundExpression3 = _dynamicFactory.MakeDynamicUnaryOperator(unaryOperatorKind, boundExpression2, specialType).ToExpression();
                if (!flag2)
                {
                    BoundExpression left = MakeTruthTestForDynamicLogicalOperator(syntax, loweredLeft, specialType, leftTruthOperator, flag);
                    boundExpression3 = _factory.Binary(BinaryOperatorKind.LogicalOr, specialType, left, boundExpression3);
                }
            }
            else if (flag2)
            {
                boundExpression3 = boundExpression2;
            }
            else
            {
                BoundExpression condition = MakeTruthTestForDynamicLogicalOperator(syntax, loweredLeft, specialType, leftTruthOperator, flag);
                BoundExpression consequence = MakeConversionNode(loweredLeft, type, @checked: false);
                boundExpression3 = _factory.Conditional(condition, consequence, boundExpression2, type);
            }
            if (boundAssignmentOperator != null)
            {
                return _factory.Sequence(ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)boundAssignmentOperator), boundExpression3);
            }
            return boundExpression3;
        }

        private static ConstantValue? UnboxConstant(BoundExpression expression)
        {
            if (expression.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)expression;
                if (boundConversion.ConversionKind == ConversionKind.Boxing)
                {
                    return boundConversion.Operand.ConstantValue;
                }
            }
            return null;
        }

        private BoundExpression MakeTruthTestForDynamicLogicalOperator(SyntaxNode syntax, BoundExpression loweredLeft, TypeSymbol boolean, MethodSymbol? leftTruthOperator, bool negative)
        {
            if (loweredLeft.HasDynamicType())
            {
                return _dynamicFactory.MakeDynamicUnaryOperator(negative ? UnaryOperatorKind.DynamicFalse : UnaryOperatorKind.DynamicTrue, loweredLeft, boolean).ToExpression();
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
            Conversion conversion = _compilation.Conversions.ClassifyConversionFromExpression(loweredLeft, boolean, ref useSiteInfo);
            _diagnostics.Add(loweredLeft.Syntax, useSiteInfo);
            if (conversion.IsImplicit)
            {
                BoundExpression boundExpression = MakeConversionNode(loweredLeft, boolean, @checked: false);
                if (negative)
                {
                    return new BoundUnaryOperator(syntax, UnaryOperatorKind.BoolLogicalNegation, boundExpression, null, null, LookupResultKind.Viable, boolean)
                    {
                        WasCompilerGenerated = true
                    };
                }
                return boundExpression;
            }
            return BoundCall.Synthesized(syntax, null, leftTruthOperator, loweredLeft);
        }

        private BoundExpression LowerUserDefinedBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, MethodSymbol? method)
        {
            if (operatorKind.IsLifted())
            {
                return RewriteLiftedBinaryOperator(syntax, operatorKind, loweredLeft, loweredRight, type, method);
            }
            return BoundCall.Synthesized(syntax, null, method, loweredLeft, loweredRight);
        }

        private BoundExpression? TrivialLiftedComparisonOperatorOptimizations(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right, MethodSymbol? method)
        {
            bool flag = NullableNeverHasValue(left);
            bool flag2 = NullableNeverHasValue(right);
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            if (flag && flag2)
            {
                return MakeLiteral(syntax, ConstantValue.Create(kind.Operator() == BinaryOperatorKind.Equal), specialType);
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(left);
            BoundExpression boundExpression2 = NullableAlwaysHasValue(right);
            if (boundExpression != null && boundExpression2 != null)
            {
                return MakeBinaryOperator(syntax, kind.Unlifted(), boundExpression, boundExpression2, specialType, method);
            }
            BinaryOperatorKind binaryOperatorKind = kind.Operator();
            if ((flag && boundExpression2 != null) || (flag2 && boundExpression != null))
            {
                BoundExpression boundExpression3 = MakeLiteral(syntax, ConstantValue.Create(binaryOperatorKind == BinaryOperatorKind.NotEqual), specialType);
                BoundExpression boundExpression4 = (flag ? boundExpression2 : boundExpression);
                if (ReadIsSideeffecting(boundExpression4))
                {
                    boundExpression3 = new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundExpression4), boundExpression3, specialType);
                }
                return boundExpression3;
            }
            if (flag || flag2)
            {
                BoundExpression boundExpression5 = (flag ? right : left);
                if (binaryOperatorKind == BinaryOperatorKind.Equal || binaryOperatorKind == BinaryOperatorKind.NotEqual)
                {
                    BoundExpression boundExpression6 = MakeNullableHasValue(syntax, boundExpression5);
                    if (binaryOperatorKind != BinaryOperatorKind.Equal)
                    {
                        return boundExpression6;
                    }
                    return MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, boundExpression6, specialType);
                }
                BoundExpression boundExpression7 = MakeBooleanConstant(syntax, binaryOperatorKind == BinaryOperatorKind.NotEqual);
                return _factory.MakeSequence(boundExpression5, boundExpression7);
            }
            return null;
        }

        private BoundExpression MakeOptimizedGetValueOrDefault(SyntaxNode syntax, BoundExpression expression)
        {
            if (expression.Type.IsNullableType())
            {
                return BoundCall.Synthesized(syntax, expression, UnsafeGetNullableMethod(syntax, expression.Type, SpecialMember.System_Nullable_T_GetValueOrDefault));
            }
            return expression;
        }

        private BoundExpression MakeBooleanConstant(SyntaxNode syntax, bool value)
        {
            return MakeLiteral(syntax, ConstantValue.Create(value), _compilation.GetSpecialType(SpecialType.System_Boolean));
        }

        private BoundExpression MakeOptimizedHasValue(SyntaxNode syntax, BoundExpression expression)
        {
            if (expression.Type.IsNullableType())
            {
                return MakeNullableHasValue(syntax, expression);
            }
            return MakeBooleanConstant(syntax, value: true);
        }

        private BoundExpression MakeNullableHasValue(SyntaxNode syntax, BoundExpression expression)
        {
            return BoundCall.Synthesized(syntax, expression, UnsafeGetNullableMethod(syntax, expression.Type, SpecialMember.System_Nullable_T_get_HasValue));
        }

        private BoundExpression LowerLiftedBuiltInComparisonOperator(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight)
        {
            BoundExpression boundExpression = TrivialLiftedComparisonOperatorOptimizations(syntax, kind, loweredLeft, loweredRight, null);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundExpression boundExpression2 = NullableAlwaysHasValue(loweredLeft);
            BoundExpression boundExpression3 = NullableAlwaysHasValue(loweredRight);
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression2 ?? loweredLeft, out BoundAssignmentOperator store);
            BoundLocal boundLocal2 = _factory.StoreToTemp(boundExpression3 ?? loweredRight, out BoundAssignmentOperator store2);
            BoundExpression loweredLeft2 = MakeOptimizedGetValueOrDefault(syntax, boundLocal);
            BoundExpression loweredRight2 = MakeOptimizedGetValueOrDefault(syntax, boundLocal2);
            BoundExpression loweredLeft3 = MakeOptimizedHasValue(syntax, boundLocal);
            BoundExpression loweredRight3 = MakeOptimizedHasValue(syntax, boundLocal2);
            BinaryOperatorKind binaryOperatorKind = kind.Operator();
            BinaryOperatorKind kind2;
            BinaryOperatorKind operatorKind;
            if (binaryOperatorKind == BinaryOperatorKind.Equal || binaryOperatorKind == BinaryOperatorKind.NotEqual)
            {
                kind2 = BinaryOperatorKind.Equal;
                operatorKind = BinaryOperatorKind.BoolEqual;
            }
            else
            {
                kind2 = binaryOperatorKind;
                operatorKind = BinaryOperatorKind.BoolAnd;
            }
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression loweredLeft4 = MakeBinaryOperator(syntax, kind2.WithType(kind.OperandTypes()), loweredLeft2, loweredRight2, specialType, null);
            BoundExpression loweredRight4 = MakeBinaryOperator(syntax, operatorKind, loweredLeft3, loweredRight3, specialType, null);
            BoundExpression boundExpression4 = MakeBinaryOperator(syntax, BinaryOperatorKind.BoolAnd, loweredLeft4, loweredRight4, specialType, null);
            if (binaryOperatorKind == BinaryOperatorKind.NotEqual)
            {
                boundExpression4 = _factory.Not(boundExpression4);
            }
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol, boundLocal2.LocalSymbol), ImmutableArray.Create(store, (BoundExpression)store2), boundExpression4, specialType);
        }

        private BoundExpression LowerLiftedUserDefinedComparisonOperator(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight, MethodSymbol? method)
        {
            BoundExpression boundExpression = TrivialLiftedComparisonOperatorOptimizations(syntax, kind, loweredLeft, loweredRight, method);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundExpression boundExpression2 = NullableAlwaysHasValue(loweredLeft);
            BoundExpression boundExpression3 = NullableAlwaysHasValue(loweredRight);
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression2 ?? loweredLeft, out BoundAssignmentOperator store);
            BoundLocal boundLocal2 = _factory.StoreToTemp(boundExpression3 ?? loweredRight, out BoundAssignmentOperator store2);
            BoundExpression loweredLeft2 = MakeOptimizedGetValueOrDefault(syntax, boundLocal);
            BoundExpression loweredRight2 = MakeOptimizedGetValueOrDefault(syntax, boundLocal2);
            BoundExpression boundExpression4 = MakeOptimizedHasValue(syntax, boundLocal);
            BoundExpression loweredRight3 = MakeOptimizedHasValue(syntax, boundLocal2);
            BinaryOperatorKind binaryOperatorKind = kind.Operator();
            BinaryOperatorKind operatorKind = ((binaryOperatorKind != BinaryOperatorKind.Equal && binaryOperatorKind != BinaryOperatorKind.NotEqual) ? BinaryOperatorKind.BoolAnd : BinaryOperatorKind.BoolEqual);
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression rewrittenCondition = MakeBinaryOperator(syntax, operatorKind, boundExpression4, loweredRight3, specialType, null);
            BoundExpression boundExpression5 = MakeBinaryOperator(syntax, kind.Unlifted(), loweredLeft2, loweredRight2, specialType, method);
            BoundExpression rewrittenConsequence = ((binaryOperatorKind != BinaryOperatorKind.Equal && binaryOperatorKind != BinaryOperatorKind.NotEqual) ? boundExpression5 : RewriteConditionalOperator(syntax, boundExpression4, boundExpression5, MakeLiteral(syntax, ConstantValue.Create(binaryOperatorKind == BinaryOperatorKind.Equal), specialType), null, specialType, isRef: false));
            BoundExpression rewrittenAlternative = MakeBooleanConstant(syntax, binaryOperatorKind == BinaryOperatorKind.NotEqual);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, specialType, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol, boundLocal2.LocalSymbol), ImmutableArray.Create(store, (BoundExpression)store2), value, specialType);
        }

        private BoundExpression? TrivialLiftedBinaryArithmeticOptimizations(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right, TypeSymbol type, MethodSymbol? method)
        {
            bool num = NullableNeverHasValue(left);
            bool flag = NullableNeverHasValue(right);
            if (num && flag)
            {
                return new BoundDefaultExpression(syntax, type);
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(left);
            BoundExpression boundExpression2 = NullableAlwaysHasValue(right);
            if (boundExpression != null && boundExpression2 != null)
            {
                return MakeLiftedBinaryOperatorConsequence(syntax, kind, boundExpression, boundExpression2, type, method);
            }
            return null;
        }

        private BoundExpression MakeLiftedBinaryOperatorConsequence(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right, TypeSymbol type, MethodSymbol? method)
        {
            BoundExpression boundExpression = MakeBinaryOperator(syntax, kind.Unlifted(), left, right, type.GetNullableUnderlyingType(), method);
            return new BoundObjectCreationExpression(syntax, UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor), boundExpression);
        }

        private static BoundExpression? OptimizeLiftedArithmeticOperatorOneNull(SyntaxNode syntax, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            bool flag = NullableNeverHasValue(left);
            bool flag2 = NullableNeverHasValue(right);
            if (!(flag || flag2))
            {
                return null;
            }
            BoundExpression boundExpression = (flag ? right : left);
            BoundExpression boundExpression2 = NullableAlwaysHasValue(boundExpression) ?? boundExpression;
            if (boundExpression2.ConstantValue != null)
            {
                return new BoundDefaultExpression(syntax, type);
            }
            return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundExpression2), new BoundDefaultExpression(syntax, type), type);
        }

        private BoundExpression LowerLiftedBinaryArithmeticOperator(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, MethodSymbol? method)
        {
            BoundExpression boundExpression = OptimizeLiftedBinaryArithmetic(syntax, kind, loweredLeft, loweredRight, type, method);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
            BoundExpression boundExpression2 = NullableAlwaysHasValue(loweredLeft);
            BoundExpression boundExpression3 = NullableAlwaysHasValue(loweredRight);
            if (boundExpression2 == null)
            {
                boundExpression2 = loweredLeft;
            }
            BoundExpression operand = boundExpression2;
            operand = CaptureExpressionInTempIfNeeded(operand, instance, instance2);
            BoundExpression operand2 = boundExpression3 ?? loweredRight;
            operand2 = CaptureExpressionInTempIfNeeded(operand2, instance, instance2);
            BoundExpression left = MakeOptimizedGetValueOrDefault(syntax, operand);
            BoundExpression right = MakeOptimizedGetValueOrDefault(syntax, operand2);
            BoundExpression loweredLeft2 = MakeOptimizedHasValue(syntax, operand);
            BoundExpression loweredRight2 = MakeOptimizedHasValue(syntax, operand2);
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression rewrittenCondition = MakeBinaryOperator(syntax, BinaryOperatorKind.BoolAnd, loweredLeft2, loweredRight2, specialType, null);
            BoundExpression rewrittenConsequence = MakeLiftedBinaryOperatorConsequence(syntax, kind, left, right, type, method);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, type);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, type, isRef: false);
            return new BoundSequence(syntax, instance2.ToImmutableAndFree(), instance.ToImmutableAndFree(), value, type);
        }

        private BoundExpression CaptureExpressionInTempIfNeeded(BoundExpression operand, ArrayBuilder<BoundExpression> sideeffects, ArrayBuilder<LocalSymbol> locals, SynthesizedLocalKind kind = SynthesizedLocalKind.LoweringTemp)
        {
            if (CanChangeValueBetweenReads(operand))
            {
                BoundLocal boundLocal = _factory.StoreToTemp(operand, out BoundAssignmentOperator store, RefKind.None, kind);
                sideeffects.Add(store);
                locals.Add(boundLocal.LocalSymbol);
                operand = boundLocal;
            }
            return operand;
        }

        private BoundExpression? OptimizeLiftedBinaryArithmetic(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right, TypeSymbol type, MethodSymbol? method)
        {
            BoundExpression boundExpression = TrivialLiftedBinaryArithmeticOptimizations(syntax, kind, left, right, type, method);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            if (kind == BinaryOperatorKind.LiftedBoolAnd || kind == BinaryOperatorKind.LiftedBoolOr)
            {
                return LowerLiftedBooleanOperator(syntax, kind, left, right);
            }
            boundExpression = OptimizeLiftedArithmeticOperatorOneNull(syntax, left, right, type);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundExpression boundExpression2 = NullableAlwaysHasValue(right);
            if (boundExpression2 != null && boundExpression2.ConstantValue != null && left.Kind == BoundKind.Sequence)
            {
                BoundSequence boundSequence = (BoundSequence)left;
                if (boundSequence.Value.Kind == BoundKind.ConditionalOperator)
                {
                    BoundConditionalOperator boundConditionalOperator = (BoundConditionalOperator)boundSequence.Value;
                    if (NullableAlwaysHasValue(boundConditionalOperator.Consequence) != null && NullableNeverHasValue(boundConditionalOperator.Alternative))
                    {
                        return new BoundSequence(syntax, boundSequence.Locals, boundSequence.SideEffects, RewriteConditionalOperator(syntax, boundConditionalOperator.Condition, MakeBinaryOperator(syntax, kind, boundConditionalOperator.Consequence, right, type, method), MakeBinaryOperator(syntax, kind, boundConditionalOperator.Alternative, right, type, method), null, type, isRef: false), type);
                    }
                }
            }
            return null;
        }

        private BoundExpression MakeNewNullableBoolean(SyntaxNode syntax, bool? value)
        {
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Nullable_T);
            TypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Boolean);
            NamedTypeSymbol namedTypeSymbol = specialType.Construct(specialType2);
            if (!value.HasValue)
            {
                return new BoundDefaultExpression(syntax, namedTypeSymbol);
            }
            return new BoundObjectCreationExpression(syntax, UnsafeGetNullableMethod(syntax, namedTypeSymbol, SpecialMember.System_Nullable_T__ctor), MakeBooleanConstant(syntax, value.GetValueOrDefault()));
        }

        private BoundExpression? OptimizeLiftedBooleanOperatorOneNull(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right)
        {
            bool flag = NullableNeverHasValue(left);
            bool flag2 = NullableNeverHasValue(right);
            if (!(flag || flag2))
            {
                return null;
            }
            BoundExpression boundExpression = (flag ? left : right);
            BoundExpression boundExpression2 = (flag ? right : left);
            BoundExpression boundExpression3 = NullableAlwaysHasValue(boundExpression2);
            BoundExpression boundExpression4 = new BoundDefaultExpression(syntax, boundExpression.Type);
            if (boundExpression3 != null)
            {
                BoundExpression boundExpression5 = MakeNewNullableBoolean(syntax, kind == BinaryOperatorKind.LiftedBoolOr);
                return RewriteConditionalOperator(syntax, boundExpression3, (kind == BinaryOperatorKind.LiftedBoolAnd) ? boundExpression4 : boundExpression5, (kind == BinaryOperatorKind.LiftedBoolAnd) ? boundExpression5 : boundExpression4, null, boundExpression.Type, isRef: false);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression2, out BoundAssignmentOperator store);
            BoundExpression rewrittenCondition = MakeOptimizedGetValueOrDefault(syntax, boundLocal);
            BoundExpression rewrittenConsequence = ((kind == BinaryOperatorKind.LiftedBoolAnd) ? boundExpression4 : boundLocal);
            BoundExpression rewrittenAlternative = ((kind == BinaryOperatorKind.LiftedBoolAnd) ? boundLocal : boundExpression4);
            BoundExpression boundExpression6 = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, boundExpression.Type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), boundExpression6, boundExpression6.Type);
        }

        private BoundExpression? OptimizeLiftedBooleanOperatorOneNonNull(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression left, BoundExpression right)
        {
            BoundExpression boundExpression = NullableAlwaysHasValue(left);
            BoundExpression boundExpression2 = NullableAlwaysHasValue(right);
            if (boundExpression == null && boundExpression2 == null)
            {
                return null;
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression ?? left, out BoundAssignmentOperator store);
            BoundLocal boundLocal2 = _factory.StoreToTemp(boundExpression2 ?? right, out BoundAssignmentOperator store2);
            BoundLocal obj = ((boundExpression == null) ? boundLocal2 : boundLocal);
            BoundExpression boundExpression3 = ((boundExpression == null) ? boundLocal : boundLocal2);
            BoundExpression rewrittenCondition = obj;
            BoundExpression boundExpression4 = MakeNewNullableBoolean(syntax, kind == BinaryOperatorKind.LiftedBoolOr);
            BoundExpression rewrittenConsequence = ((kind == BinaryOperatorKind.LiftedBoolOr) ? boundExpression4 : boundExpression3);
            BoundExpression rewrittenAlternative = ((kind == BinaryOperatorKind.LiftedBoolOr) ? boundExpression3 : boundExpression4);
            BoundExpression boundExpression5 = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, boundExpression4.Type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol, boundLocal2.LocalSymbol), ImmutableArray.Create(store, (BoundExpression)store2), boundExpression5, boundExpression5.Type);
        }

        private BoundExpression LowerLiftedBooleanOperator(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight)
        {
            BoundExpression boundExpression = OptimizeLiftedBooleanOperatorOneNull(syntax, kind, loweredLeft, loweredRight);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            boundExpression = OptimizeLiftedBooleanOperatorOneNonNull(syntax, kind, loweredLeft, loweredRight);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundLocal boundLocal = _factory.StoreToTemp(loweredLeft, out BoundAssignmentOperator store);
            BoundLocal boundLocal2 = _factory.StoreToTemp(loweredRight, out BoundAssignmentOperator store2);
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            MethodSymbol method = UnsafeGetNullableMethod(syntax, boundLocal.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            MethodSymbol method2 = UnsafeGetNullableMethod(syntax, boundLocal2.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            BoundExpression loweredLeft2 = BoundCall.Synthesized(syntax, boundLocal, method);
            BoundExpression loweredLeft3 = BoundCall.Synthesized(syntax, boundLocal2, method2);
            BoundExpression loweredRight2 = MakeNullableHasValue(syntax, boundLocal);
            BoundExpression loweredOperand = MakeBinaryOperator(syntax, BinaryOperatorKind.LogicalBoolOr, loweredLeft3, loweredRight2, specialType, null);
            BoundExpression loweredRight3 = MakeUnaryOperator(UnaryOperatorKind.BoolLogicalNegation, syntax, null, loweredOperand, specialType);
            BoundExpression rewrittenCondition = MakeBinaryOperator(syntax, BinaryOperatorKind.LogicalBoolOr, loweredLeft2, loweredRight3, specialType, null);
            BoundExpression rewrittenConsequence = ((kind == BinaryOperatorKind.LiftedBoolAnd) ? boundLocal2 : boundLocal);
            BoundExpression boundExpression2 = ((kind == BinaryOperatorKind.LiftedBoolAnd) ? boundLocal : boundLocal2);
            BoundExpression boundExpression3 = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, boundExpression2, null, boundExpression2.Type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol, boundLocal2.LocalSymbol), ImmutableArray.Create(store, (BoundExpression)store2), boundExpression3, boundExpression3.Type);
        }

        private MethodSymbol UnsafeGetNullableMethod(SyntaxNode syntax, TypeSymbol nullableType, SpecialMember member)
        {
            return UnsafeGetNullableMethod(syntax, nullableType, member, _compilation, _diagnostics);
        }

        private static MethodSymbol UnsafeGetNullableMethod(SyntaxNode syntax, TypeSymbol nullableType, SpecialMember member, CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol newOwner = nullableType as NamedTypeSymbol;
            return UnsafeGetSpecialTypeMethod(syntax, member, compilation, diagnostics).AsMember(newOwner);
        }

        private bool TryGetNullableMethod(SyntaxNode syntax, TypeSymbol nullableType, SpecialMember member, out MethodSymbol result)
        {
            NamedTypeSymbol newOwner = (NamedTypeSymbol)nullableType;
            if (TryGetSpecialTypeMethod(syntax, member, out result))
            {
                result = result.AsMember(newOwner);
                return true;
            }
            return false;
        }

        private BoundExpression RewriteNullableNullEquality(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol returnType)
        {
            BoundExpression boundExpression = (loweredRight.IsLiteralNull() ? loweredLeft : loweredRight);
            if (NullableNeverHasValue(boundExpression))
            {
                return MakeLiteral(syntax, ConstantValue.Create(kind == BinaryOperatorKind.NullableNullEqual), returnType);
            }
            BoundExpression boundExpression2 = NullableAlwaysHasValue(boundExpression);
            if (boundExpression2 != null)
            {
                return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundExpression2), MakeBooleanConstant(syntax, kind == BinaryOperatorKind.NullableNullNotEqual), returnType);
            }
            if (boundExpression is BoundLoweredConditionalAccess boundLoweredConditionalAccess && (boundLoweredConditionalAccess.WhenNullOpt == null || boundLoweredConditionalAccess.WhenNullOpt.IsDefaultValue()))
            {
                BoundExpression boundExpression3 = RewriteNullableNullEquality(syntax, kind, boundLoweredConditionalAccess.WhenNotNull, loweredLeft.IsLiteralNull() ? loweredLeft : loweredRight, returnType);
                BoundExpression whenNullOpt = ((kind == BinaryOperatorKind.NullableNullEqual) ? MakeBooleanConstant(syntax, value: true) : null);
                return boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.Receiver, boundLoweredConditionalAccess.HasValueMethodOpt, boundExpression3, whenNullOpt, boundLoweredConditionalAccess.Id, boundExpression3.Type);
            }
            BoundExpression boundExpression4 = MakeNullableHasValue(syntax, boundExpression);
            if (kind != BinaryOperatorKind.NullableNullNotEqual)
            {
                return new BoundUnaryOperator(syntax, UnaryOperatorKind.BoolLogicalNegation, boundExpression4, null, null, LookupResultKind.Viable, returnType);
            }
            return boundExpression4;
        }

        private BoundExpression RewriteStringEquality(BoundBinaryOperator? oldNode, SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, SpecialMember member)
        {
            if (oldNode != null && (loweredLeft.ConstantValue == ConstantValue.Null || loweredRight.ConstantValue == ConstantValue.Null))
            {
                return oldNode!.Update(operatorKind, oldNode!.ConstantValueOpt, oldNode!.MethodOpt, oldNode!.ResultKind, loweredLeft, loweredRight, type);
            }
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, member);
            return BoundCall.Synthesized(syntax, null, method, loweredLeft, loweredRight);
        }

        private BoundExpression RewriteDelegateOperation(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, SpecialMember member)
        {
            MethodSymbol methodSymbol;
            if (operatorKind == BinaryOperatorKind.DelegateEqual || operatorKind == BinaryOperatorKind.DelegateNotEqual)
            {
                methodSymbol = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(member);
                if (loweredRight.IsLiteralNull() || loweredLeft.IsLiteralNull() || (object)(methodSymbol = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(member)) == null)
                {
                    operatorKind = (operatorKind & ~BinaryOperatorKind.Delegate) | BinaryOperatorKind.Object;
                    return new BoundBinaryOperator(syntax, operatorKind, null, null, LookupResultKind.Empty, loweredLeft, loweredRight, type);
                }
            }
            else
            {
                methodSymbol = UnsafeGetSpecialTypeMethod(syntax, member);
            }
            BoundExpression boundExpression = (_inExpressionLambda ? new BoundBinaryOperator(syntax, operatorKind, null, methodSymbol, LookupResultKind.Empty, loweredLeft, loweredRight, methodSymbol.ReturnType) : ((BoundExpression)BoundCall.Synthesized(syntax, null, methodSymbol, loweredLeft, loweredRight)));
            if (methodSymbol.ReturnType.SpecialType != SpecialType.System_Delegate)
            {
                return boundExpression;
            }
            return MakeConversionNode(syntax, boundExpression, Conversion.ExplicitReference, type, @checked: false);
        }

        private BoundExpression RewriteDecimalBinaryOperation(SyntaxNode syntax, BoundExpression loweredLeft, BoundExpression loweredRight, BinaryOperatorKind operatorKind)
        {
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, operatorKind switch
            {
                BinaryOperatorKind.DecimalAddition => SpecialMember.System_Decimal__op_Addition,
                BinaryOperatorKind.DecimalSubtraction => SpecialMember.System_Decimal__op_Subtraction,
                BinaryOperatorKind.DecimalMultiplication => SpecialMember.System_Decimal__op_Multiply,
                BinaryOperatorKind.DecimalDivision => SpecialMember.System_Decimal__op_Division,
                BinaryOperatorKind.DecimalRemainder => SpecialMember.System_Decimal__op_Modulus,
                BinaryOperatorKind.DecimalEqual => SpecialMember.System_Decimal__op_Equality,
                BinaryOperatorKind.DecimalNotEqual => SpecialMember.System_Decimal__op_Inequality,
                BinaryOperatorKind.DecimalLessThan => SpecialMember.System_Decimal__op_LessThan,
                BinaryOperatorKind.DecimalLessThanOrEqual => SpecialMember.System_Decimal__op_LessThanOrEqual,
                BinaryOperatorKind.DecimalGreaterThan => SpecialMember.System_Decimal__op_GreaterThan,
                BinaryOperatorKind.DecimalGreaterThanOrEqual => SpecialMember.System_Decimal__op_GreaterThanOrEqual,
                _ => throw ExceptionUtilities.UnexpectedValue(operatorKind),
            });
            return BoundCall.Synthesized(syntax, null, method, loweredLeft, loweredRight);
        }

        private BoundExpression MakeNullCheck(SyntaxNode syntax, BoundExpression rewrittenExpr, BinaryOperatorKind operatorKind)
        {
            TypeSymbol type = rewrittenExpr.Type;
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            if (rewrittenExpr.ConstantValue != null)
            {
                switch (operatorKind)
                {
                    case BinaryOperatorKind.Equal:
                        return MakeLiteral(syntax, ConstantValue.Create(rewrittenExpr.ConstantValue!.IsNull, ConstantValueTypeDiscriminator.Boolean), specialType);
                    case BinaryOperatorKind.NotEqual:
                        return MakeLiteral(syntax, ConstantValue.Create(!rewrittenExpr.ConstantValue!.IsNull, ConstantValueTypeDiscriminator.Boolean), specialType);
                }
            }
            TypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Object);
            if ((object)type != null)
            {
                if (type.Kind == SymbolKind.TypeParameter)
                {
                    rewrittenExpr = MakeConversionNode(syntax, rewrittenExpr, Conversion.Boxing, specialType2, @checked: false);
                }
                else if (type.IsNullableType())
                {
                    operatorKind |= BinaryOperatorKind.NullableNull;
                }
            }
            return MakeBinaryOperator(syntax, operatorKind, rewrittenExpr, MakeLiteral(syntax, ConstantValue.Null, specialType2), specialType, null);
        }

        private BoundExpression RewriteBuiltInShiftOperation(BoundBinaryOperator? oldNode, SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type, int rightMask)
        {
            SyntaxNode syntax2 = loweredRight.Syntax;
            ConstantValue constantValue = loweredRight.ConstantValue;
            TypeSymbol type2 = loweredRight.Type;
            if (constantValue != null && constantValue.IsIntegral)
            {
                int num = constantValue.Int32Value & rightMask;
                if (num == 0)
                {
                    return loweredLeft;
                }
                loweredRight = MakeLiteral(syntax2, ConstantValue.Create(num), type2);
            }
            else
            {
                BinaryOperatorKind operatorKind2 = (operatorKind & ~BinaryOperatorKind.OpMask) | BinaryOperatorKind.And;
                loweredRight = new BoundBinaryOperator(syntax2, operatorKind2, null, null, LookupResultKind.Viable, loweredRight, MakeLiteral(syntax2, ConstantValue.Create(rightMask), type2), type2);
            }
            if (oldNode != null)
            {
                return oldNode!.Update(operatorKind, null, null, oldNode!.ResultKind, loweredLeft, loweredRight, type);
            }
            return new BoundBinaryOperator(syntax, operatorKind, null, null, LookupResultKind.Viable, loweredLeft, loweredRight, type);
        }

        private BoundExpression RewritePointerNumericOperator(SyntaxNode syntax, BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol returnType, bool isPointerElementAccess, bool isLeftPointer)
        {
            if (isLeftPointer)
            {
                loweredRight = MakeSizeOfMultiplication(loweredRight, (PointerTypeSymbol)loweredLeft.Type, kind.IsChecked());
            }
            else
            {
                loweredLeft = MakeSizeOfMultiplication(loweredLeft, (PointerTypeSymbol)loweredRight.Type, kind.IsChecked());
            }
            if (isPointerElementAccess)
            {
                kind &= ~BinaryOperatorKind.Checked;
            }
            return new BoundBinaryOperator(syntax, kind, null, null, LookupResultKind.Viable, loweredLeft, loweredRight, returnType);
        }

        private BoundExpression MakeSizeOfMultiplication(BoundExpression numericOperand, PointerTypeSymbol pointerType, bool isChecked)
        {
            BoundExpression boundExpression = _factory.Sizeof(pointerType.PointedAtType);
            ConstantValue? constantValue = numericOperand.ConstantValue;
            if ((object)constantValue != null && constantValue!.UInt64Value == 1)
            {
                return boundExpression;
            }
            SpecialType specialType = numericOperand.Type!.SpecialType;
            ConstantValue? constantValue2 = boundExpression.ConstantValue;
            if ((object)constantValue2 != null && constantValue2!.Int32Value == 1)
            {
                SpecialType specialType2 = specialType;
                switch (specialType)
                {
                    case SpecialType.System_Int32:
                        if (isChecked)
                        {
                            ConstantValue constantValue4 = numericOperand.ConstantValue;
                            if (constantValue4 == null || constantValue4.Int32Value < 0)
                            {
                                specialType2 = SpecialType.System_IntPtr;
                            }
                        }
                        break;
                    case SpecialType.System_UInt32:
                        {
                            ConstantValue constantValue3 = numericOperand.ConstantValue;
                            if (constantValue3 == null || constantValue3.UInt32Value > int.MaxValue)
                            {
                                specialType2 = SpecialType.System_UIntPtr;
                            }
                            break;
                        }
                    case SpecialType.System_Int64:
                        specialType2 = SpecialType.System_IntPtr;
                        break;
                    case SpecialType.System_UInt64:
                        specialType2 = SpecialType.System_UIntPtr;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(specialType);
                }
                if (specialType2 != specialType)
                {
                    return _factory.Convert(_factory.SpecialType(specialType2), numericOperand, Conversion.IntegerToPointer);
                }
                return numericOperand;
            }
            BinaryOperatorKind binaryOperatorKind = BinaryOperatorKind.Multiplication;
            TypeSymbol typeSymbol2;
            TypeSymbol typeSymbol3;
            switch (specialType)
            {
                case SpecialType.System_Int32:
                    {
                        TypeSymbol typeSymbol6 = _factory.SpecialType(SpecialType.System_IntPtr);
                        numericOperand = _factory.Convert(typeSymbol6, numericOperand, Conversion.IntegerToPointer, isChecked);
                        binaryOperatorKind |= BinaryOperatorKind.Int;
                        typeSymbol2 = typeSymbol6;
                        typeSymbol3 = typeSymbol6;
                        break;
                    }
                case SpecialType.System_UInt32:
                    {
                        TypeSymbol typeSymbol5 = _factory.SpecialType(SpecialType.System_Int64);
                        NamedTypeSymbol namedTypeSymbol3 = _factory.SpecialType(SpecialType.System_IntPtr);
                        numericOperand = _factory.Convert(typeSymbol5, numericOperand, Conversion.ExplicitNumeric, isChecked);
                        boundExpression = _factory.Convert(typeSymbol5, boundExpression, Conversion.ExplicitNumeric, isChecked);
                        binaryOperatorKind |= BinaryOperatorKind.Long;
                        typeSymbol2 = typeSymbol5;
                        typeSymbol3 = namedTypeSymbol3;
                        break;
                    }
                case SpecialType.System_Int64:
                    {
                        TypeSymbol typeSymbol4 = _factory.SpecialType(SpecialType.System_Int64);
                        NamedTypeSymbol namedTypeSymbol2 = _factory.SpecialType(SpecialType.System_IntPtr);
                        boundExpression = _factory.Convert(typeSymbol4, boundExpression, Conversion.ExplicitNumeric, isChecked);
                        binaryOperatorKind |= BinaryOperatorKind.Long;
                        typeSymbol2 = typeSymbol4;
                        typeSymbol3 = namedTypeSymbol2;
                        break;
                    }
                case SpecialType.System_UInt64:
                    {
                        TypeSymbol typeSymbol = _factory.SpecialType(SpecialType.System_UInt64);
                        NamedTypeSymbol namedTypeSymbol = _factory.SpecialType(SpecialType.System_UIntPtr);
                        boundExpression = _factory.Convert(typeSymbol, boundExpression, Conversion.ExplicitNumeric, isChecked);
                        binaryOperatorKind |= BinaryOperatorKind.ULong;
                        typeSymbol2 = typeSymbol;
                        typeSymbol3 = namedTypeSymbol;
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(specialType);
            }
            if (isChecked)
            {
                binaryOperatorKind |= BinaryOperatorKind.Checked;
            }
            BoundBinaryOperator boundBinaryOperator = _factory.Binary(binaryOperatorKind, typeSymbol2, numericOperand, boundExpression);
            if (!TypeSymbol.Equals(typeSymbol3, typeSymbol2, TypeCompareKind.ConsiderEverything))
            {
                return _factory.Convert(typeSymbol3, boundBinaryOperator, Conversion.IntegerToPointer);
            }
            return boundBinaryOperator;
        }

        private BoundExpression RewritePointerSubtraction(BinaryOperatorKind kind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol returnType)
        {
            PointerTypeSymbol pointerTypeSymbol = (PointerTypeSymbol)loweredLeft.Type;
            BoundExpression right = _factory.Sizeof(pointerTypeSymbol.PointedAtType);
            return _factory.Convert(returnType, _factory.Binary(BinaryOperatorKind.Division, _factory.SpecialType(SpecialType.System_IntPtr), _factory.Binary(kind & ~BinaryOperatorKind.Checked, returnType, loweredLeft, loweredRight), right), Conversion.PointerToInteger);
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            VisitStatementSubList(instance, node.Statements);
            if (!Instrument || (node != _rootStatement && (node.WasCompilerGenerated || node.Syntax.Kind() != SyntaxKind.Block)))
            {
                return node.Update(node.Locals, node.LocalFunctions, instance.ToImmutableAndFree());
            }
            BoundStatement boundStatement = _instrumenter.CreateBlockPrologue(node, out LocalSymbol synthesizedLocal);
            if (boundStatement != null)
            {
                instance.Insert(0, boundStatement);
            }
            else if (node == _rootStatement && _factory.TopLevelMethod is SynthesizedSimpleProgramEntryPointSymbol)
            {
                instance.Insert(0, _factory.HiddenSequencePoint());
            }
            BoundStatement boundStatement2 = _instrumenter.CreateBlockEpilogue(node);
            if (boundStatement2 != null)
            {
                instance.Add(boundStatement2);
            }
            return new BoundBlock(node.Syntax, (synthesizedLocal == null) ? node.Locals : node.Locals.Add(synthesizedLocal), node.LocalFunctions, instance.ToImmutableAndFree(), node.HasErrors);
        }

        public void VisitStatementSubList(ArrayBuilder<BoundStatement> builder, ImmutableArray<BoundStatement> statements, int startIndex = 0)
        {
            for (int i = startIndex; i < statements.Length; i++)
            {
                BoundStatement boundStatement = VisitPossibleUsingDeclaration(statements[i], statements, i, out bool replacedLocalDeclarations);
                if (boundStatement != null)
                {
                    builder.Add(boundStatement);
                }
                if (replacedLocalDeclarations)
                {
                    break;
                }
            }
        }

        public BoundStatement? VisitPossibleUsingDeclaration(BoundStatement node, ImmutableArray<BoundStatement> statements, int statementIndex, out bool replacedLocalDeclarations)
        {
            switch (node.Kind)
            {
                case BoundKind.LabeledStatement:
                    {
                        BoundLabeledStatement boundLabeledStatement = (BoundLabeledStatement)node;
                        return MakeLabeledStatement(boundLabeledStatement, VisitPossibleUsingDeclaration(boundLabeledStatement.Body, statements, statementIndex, out replacedLocalDeclarations));
                    }
                case BoundKind.UsingLocalDeclarations:
                    {
                        ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                        VisitStatementSubList(instance, statements, statementIndex + 1);
                        replacedLocalDeclarations = true;
                        return MakeLocalUsingDeclarationStatement((BoundUsingLocalDeclarations)node, instance.ToImmutableAndFree());
                    }
                default:
                    replacedLocalDeclarations = false;
                    return VisitStatement(node);
            }
        }

        public override BoundNode VisitNoOpStatement(BoundNoOpStatement node)
        {
            if (!node.WasCompilerGenerated && Instrument)
            {
                return _instrumenter.InstrumentNoOpStatement(node, node);
            }
            return new BoundBlock(node.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty);
        }

        public override BoundNode VisitBreakStatement(BoundBreakStatement node)
        {
            BoundStatement boundStatement = new BoundGotoStatement(node.Syntax, node.Label, node.HasErrors);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentBreakStatement(node, boundStatement);
            }
            return boundStatement;
        }

        public override BoundNode VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            return VisitDynamicInvocation(node, resultDiscarded: false);
        }

        public BoundExpression VisitDynamicInvocation(BoundDynamicInvocation node, bool resultDiscarded)
        {
            ImmutableArray<BoundExpression> loweredArguments = VisitList(node.Arguments);
            BoundMethodGroup boundMethodGroup;
            ImmutableArray<TypeWithAnnotations> typeArgumentsOpt;
            string name;
            bool flag;
            BoundExpression boundExpression;
            switch (node.Expression.Kind)
            {
                case BoundKind.MethodGroup:
                    boundMethodGroup = (BoundMethodGroup)node.Expression;
                    typeArgumentsOpt = boundMethodGroup.TypeArgumentsOpt;
                    name = boundMethodGroup.Name;
                    flag = ((uint?)boundMethodGroup.Flags & 2u) != 0;
                    if (boundMethodGroup.ReceiverOpt == null)
                    {
                        NamedTypeSymbol containingType = node.ApplicableMethods.First().ContainingType;
                        boundExpression = new BoundTypeExpression(node.Syntax, null, containingType);
                    }
                    else
                    {
                        if (flag)
                        {
                            MethodSymbol topLevelMethod = _factory.TopLevelMethod;
                            if ((object)topLevelMethod != null && !topLevelMethod.RequiresInstanceReceiver)
                            {
                                boundExpression = new BoundTypeExpression(node.Syntax, null, _factory.CurrentType);
                                goto IL_010b;
                            }
                        }
                        boundExpression = VisitExpression(boundMethodGroup.ReceiverOpt);
                    }
                    goto IL_010b;
                case BoundKind.DynamicMemberAccess:
                    {
                        BoundDynamicMemberAccess boundDynamicMemberAccess = (BoundDynamicMemberAccess)node.Expression;
                        name = boundDynamicMemberAccess.Name;
                        typeArgumentsOpt = boundDynamicMemberAccess.TypeArgumentsOpt;
                        boundExpression = VisitExpression(boundDynamicMemberAccess.Receiver);
                        flag = false;
                        break;
                    }
                default:
                    {
                        BoundExpression loweredReceiver = VisitExpression(node.Expression);
                        return _dynamicFactory.MakeDynamicInvocation(loweredReceiver, loweredArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, resultDiscarded).ToExpression();
                    }
                IL_010b:
                    EmbedIfNeedTo(boundExpression, boundMethodGroup.Methods, node.Syntax);
                    break;
            }
            return _dynamicFactory.MakeDynamicMemberInvocation(name, boundExpression, typeArgumentsOpt, loweredArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, flag, resultDiscarded).ToExpression();
        }

        private void EmbedIfNeedTo(BoundExpression receiver, ImmutableArray<MethodSymbol> methods, SyntaxNode syntaxNode)
        {
            PEModuleBuilder emitModule = EmitModule;
            if (emitModule == null || receiver == null || (object)receiver.Type == null)
            {
                return;
            }
            AssemblySymbol containingAssembly = receiver.Type!.ContainingAssembly;
            if ((object)containingAssembly != null && containingAssembly.IsLinked)
            {
                ImmutableArray<MethodSymbol>.Enumerator enumerator = methods.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MethodSymbol current = enumerator.Current;
                    emitModule.EmbeddedTypesManagerOpt.EmbedMethodIfNeedTo(current.OriginalDefinition.GetCciAdapter(), syntaxNode, _diagnostics.DiagnosticBag);
                }
            }
        }

        private void EmbedIfNeedTo(BoundExpression receiver, ImmutableArray<PropertySymbol> properties, SyntaxNode syntaxNode)
        {
            PEModuleBuilder emitModule = EmitModule;
            if (emitModule == null || receiver == null || (object)receiver.Type == null)
            {
                return;
            }
            AssemblySymbol containingAssembly = receiver.Type!.ContainingAssembly;
            if ((object)containingAssembly != null && containingAssembly.IsLinked)
            {
                ImmutableArray<PropertySymbol>.Enumerator enumerator = properties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PropertySymbol current = enumerator.Current;
                    emitModule.EmbeddedTypesManagerOpt.EmbedPropertyIfNeedTo(current.OriginalDefinition.GetCciAdapter(), syntaxNode, _diagnostics.DiagnosticBag);
                }
            }
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
            return MakeCall(node.Syntax, rewrittenReceiver, node.Method, rewrittenArguments, node.ArgumentRefKindsOpt, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.ResultKind, node.Type, node);
        }

        private BoundExpression MakeCall(SyntaxNode syntax, BoundExpression? rewrittenReceiver, MethodSymbol method, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, LookupResultKind resultKind, TypeSymbol type, BoundCall? nodeOpt = null)
        {
            rewrittenArguments = MakeArguments(syntax, rewrittenArguments, method, expanded, argsToParamsOpt, ref argumentRefKindsOpt, out var temps, invokedAsExtensionMethod);
            return MakeCall(nodeOpt, syntax, rewrittenReceiver, method, rewrittenArguments, argumentRefKindsOpt, invokedAsExtensionMethod, resultKind, type, temps);
        }

        private BoundExpression MakeCall(BoundCall? node, SyntaxNode syntax, BoundExpression? rewrittenReceiver, MethodSymbol method, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<RefKind> argumentRefKinds, bool invokedAsExtensionMethod, LookupResultKind resultKind, TypeSymbol type, ImmutableArray<LocalSymbol> temps = default(ImmutableArray<LocalSymbol>))
        {
            BoundExpression boundExpression = ((method.IsStatic && method.ContainingType.IsObjectType() && !_inExpressionLambda && (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_Object__ReferenceEquals)) ? new BoundBinaryOperator(syntax, BinaryOperatorKind.ObjectEqual, null, null, resultKind, rewrittenArguments[0], rewrittenArguments[1], type) : ((BoundExpression)((node != null) ? node!.Update(rewrittenReceiver, method, rewrittenArguments, default(ImmutableArray<string>), argumentRefKinds, node!.IsDelegateCall, expanded: false, node!.InvokedAsExtensionMethod, default(ImmutableArray<int>), default(BitVector), node!.ResultKind, node!.Type) : new BoundCall(syntax, rewrittenReceiver, method, rewrittenArguments, default(ImmutableArray<string>), argumentRefKinds, isDelegateCall: false, expanded: false, invokedAsExtensionMethod, default(ImmutableArray<int>), default(BitVector), resultKind, type))));
            if (!temps.IsDefaultOrEmpty)
            {
                return new BoundSequence(syntax, temps, ImmutableArray<BoundExpression>.Empty, boundExpression, type);
            }
            return boundExpression;
        }

        private BoundExpression MakeCall(SyntaxNode syntax, BoundExpression? rewrittenReceiver, MethodSymbol method, ImmutableArray<BoundExpression> rewrittenArguments, TypeSymbol type)
        {
            return MakeCall(null, syntax, rewrittenReceiver, method, rewrittenArguments, default(ImmutableArray<RefKind>), invokedAsExtensionMethod: false, LookupResultKind.Viable, type);
        }

        private static bool IsSafeForReordering(BoundExpression expression, RefKind kind)
        {
            BoundExpression boundExpression = expression;
            while (!(boundExpression.ConstantValue != null))
            {
                switch (boundExpression.Kind)
                {
                    default:
                        return false;
                    case BoundKind.Local:
                    case BoundKind.Parameter:
                        return kind != RefKind.None;
                    case BoundKind.PassByCopy:
                        return IsSafeForReordering(((BoundPassByCopy)boundExpression).Expression, kind);
                    case BoundKind.Conversion:
                        {
                            BoundConversion boundConversion = (BoundConversion)boundExpression;
                            switch (boundConversion.ConversionKind)
                            {
                                case ConversionKind.NullLiteral:
                                case ConversionKind.ImplicitConstant:
                                case ConversionKind.AnonymousFunction:
                                case ConversionKind.MethodGroup:
                                case ConversionKind.DefaultLiteral:
                                    return true;
                                case ConversionKind.Identity:
                                case ConversionKind.ImplicitNumeric:
                                case ConversionKind.ImplicitEnumeration:
                                case ConversionKind.ImplicitNullable:
                                case ConversionKind.ImplicitReference:
                                case ConversionKind.Boxing:
                                case ConversionKind.ImplicitPointerToVoid:
                                case ConversionKind.ImplicitNullToPointer:
                                case ConversionKind.ImplicitDynamic:
                                case ConversionKind.ExplicitDynamic:
                                case ConversionKind.ExplicitNumeric:
                                case ConversionKind.ExplicitEnumeration:
                                case ConversionKind.ExplicitNullable:
                                case ConversionKind.ExplicitReference:
                                case ConversionKind.Unboxing:
                                case ConversionKind.ExplicitPointerToPointer:
                                case ConversionKind.ExplicitIntegerToPointer:
                                case ConversionKind.ExplicitPointerToInteger:
                                    break;
                                case ConversionKind.ImplicitThrow:
                                case ConversionKind.ImplicitUserDefined:
                                case ConversionKind.ExplicitUserDefined:
                                case ConversionKind.IntPtr:
                                    return false;
                                default:
                                    return false;
                            }
                            boundExpression = boundConversion.Operand;
                            break;
                        }
                }
            }
            return true;
        }

        private ImmutableArray<BoundExpression> MakeArguments(SyntaxNode syntax, ImmutableArray<BoundExpression> rewrittenArguments, Symbol methodOrIndexer, bool expanded, ImmutableArray<int> argsToParamsOpt, ref ImmutableArray<RefKind> argumentRefKindsOpt, out ImmutableArray<LocalSymbol> temps, bool invokedAsExtensionMethod = false, ThreeState enableCallerInfo = ThreeState.Unknown)
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            rewrittenArguments = _factory.MakeTempsForDiscardArguments(rewrittenArguments, instance);
            ImmutableArray<ParameterSymbol> parameters = methodOrIndexer.GetParameters();
            if (CanSkipRewriting(rewrittenArguments, methodOrIndexer, expanded, argsToParamsOpt, invokedAsExtensionMethod, ignoreComReceiver: false, out var isComReceiver))
            {
                temps = instance.ToImmutableAndFree();
                argumentRefKindsOpt = GetEffectiveArgumentRefKinds(argumentRefKindsOpt, parameters);
                return rewrittenArguments;
            }
            BoundExpression[] array = new BoundExpression[parameters.Length];
            ArrayBuilder<BoundAssignmentOperator> instance2 = ArrayBuilder<BoundAssignmentOperator>.GetInstance(rewrittenArguments.Length);
            ArrayBuilder<RefKind> instance3 = ArrayBuilder<RefKind>.GetInstance(parameters.Length, RefKind.None);
            BuildStoresToTemps(expanded, argsToParamsOpt, parameters, argumentRefKindsOpt, rewrittenArguments, forceLambdaSpilling: false, array, instance3, instance2);
            OptimizeTemporaries(array, instance2, instance);
            if (expanded)
            {
                array[^1] = BuildParamsArray(syntax, methodOrIndexer, argsToParamsOpt, rewrittenArguments, parameters, array[^1]);
            }
            if (isComReceiver)
            {
                RewriteArgumentsForComCall(parameters, array, instance3, instance);
            }
            temps = instance.ToImmutableAndFree();
            instance2.Free();
            argumentRefKindsOpt = GetRefKindsOrNull(instance3);
            instance3.Free();
            return array.AsImmutableOrNull();
        }

        private static ImmutableArray<RefKind> GetEffectiveArgumentRefKinds(ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<ParameterSymbol> parameters)
        {
            ArrayBuilder<RefKind> arrayBuilder = null;
            for (int i = 0; i < parameters.Length; i++)
            {
                RefKind refKind = parameters[i].RefKind;
                if (refKind != RefKind.In)
                {
                    continue;
                }
                RefKind refKind2 = ((!argumentRefKindsOpt.IsDefault) ? argumentRefKindsOpt[i] : RefKind.None);
                if (arrayBuilder == null)
                {
                    if (!argumentRefKindsOpt.IsDefault)
                    {
                        arrayBuilder = ArrayBuilder<RefKind>.GetInstance(parameters.Length);
                        arrayBuilder.AddRange(argumentRefKindsOpt);
                    }
                    else
                    {
                        arrayBuilder = ArrayBuilder<RefKind>.GetInstance(parameters.Length, RefKind.None);
                    }
                }
                arrayBuilder[i] = ((refKind2 == RefKind.None) ? refKind : ((RefKind)4));
            }
            if (arrayBuilder != null)
            {
                argumentRefKindsOpt = arrayBuilder.ToImmutableAndFree();
            }
            return argumentRefKindsOpt;
        }

        internal static ImmutableArray<IArgumentOperation> MakeArgumentsInEvaluationOrder(CSharpOperationFactory operationFactory, CSharpCompilation compilation, SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, Symbol methodOrIndexer, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool invokedAsExtensionMethod)
        {
            if (CanSkipRewriting(arguments, methodOrIndexer, expanded, argsToParamsOpt, invokedAsExtensionMethod, ignoreComReceiver: true, out var _))
            {
                ImmutableArray<ParameterSymbol> parameters = methodOrIndexer.GetParameters();
                ArrayBuilder<IArgumentOperation> instance = ArrayBuilder<IArgumentOperation>.GetInstance(arguments.Length);
                int i;
                for (i = 0; i < parameters.Length; i++)
                {
                    ArgumentKind kind = ((!defaultArguments[i]) ? ArgumentKind.Explicit : ArgumentKind.DefaultValue);
                    instance.Add(operationFactory.CreateArgumentOperation(kind, parameters[i].GetPublicSymbol(), arguments[i]));
                }
                for (; i < arguments.Length; i++)
                {
                    ArgumentKind kind2 = ((!defaultArguments[i]) ? ArgumentKind.Explicit : ArgumentKind.DefaultValue);
                    instance.Add(operationFactory.CreateArgumentOperation(kind2, null, arguments[i]));
                }
                return instance.ToImmutableAndFree();
            }
            return BuildArgumentsInEvaluationOrder(operationFactory, syntax, methodOrIndexer, expanded, argsToParamsOpt, defaultArguments, arguments, compilation);
        }

        private static bool CanSkipRewriting(ImmutableArray<BoundExpression> rewrittenArguments, Symbol methodOrIndexer, bool expanded, ImmutableArray<int> argsToParamsOpt, bool invokedAsExtensionMethod, bool ignoreComReceiver, out bool isComReceiver)
        {
            isComReceiver = false;
            if (methodOrIndexer.GetIsVararg())
            {
                return true;
            }
            if (!ignoreComReceiver)
            {
                isComReceiver = (invokedAsExtensionMethod ? (((MethodSymbol)methodOrIndexer).Parameters[0].Type as NamedTypeSymbol) : methodOrIndexer.ContainingType)?.IsComImport ?? false;
            }
            if (rewrittenArguments.Length == methodOrIndexer.GetParameterCount() && argsToParamsOpt.IsDefault && !expanded)
            {
                return !isComReceiver;
            }
            return false;
        }

        private static ImmutableArray<RefKind> GetRefKindsOrNull(ArrayBuilder<RefKind> refKinds)
        {
            ArrayBuilder<RefKind>.Enumerator enumerator = refKinds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != 0)
                {
                    return refKinds.ToImmutable();
                }
            }
            return default(ImmutableArray<RefKind>);
        }

        private void BuildStoresToTemps(bool expanded, ImmutableArray<int> argsToParamsOpt, ImmutableArray<ParameterSymbol> parameters, ImmutableArray<RefKind> argumentRefKinds, ImmutableArray<BoundExpression> rewrittenArguments, bool forceLambdaSpilling, BoundExpression[] arguments, ArrayBuilder<RefKind> refKinds, ArrayBuilder<BoundAssignmentOperator> storesToTemps)
        {
            for (int i = 0; i < rewrittenArguments.Length; i++)
            {
                BoundExpression boundExpression = rewrittenArguments[i];
                int num = ((!argsToParamsOpt.IsDefault) ? argsToParamsOpt[i] : i);
                RefKind refKind = argumentRefKinds.RefKinds(i);
                RefKind refKind2 = parameters[num].RefKind;
                if (IsBeginningOfParamArray(num, i, expanded, arguments.Length, rewrittenArguments, argsToParamsOpt, out var numberOfParamArrayArguments) && i + numberOfParamArrayArguments == rewrittenArguments.Length)
                {
                    break;
                }
                if ((!forceLambdaSpilling || !isLambdaConversion(boundExpression)) && IsSafeForReordering(boundExpression, refKind))
                {
                    arguments[num] = boundExpression;
                }
                else
                {
                    BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store, (refKind2 == RefKind.In) ? RefKind.In : refKind);
                    storesToTemps.Add(store);
                    arguments[num] = boundLocal;
                }
                if (refKind2 == RefKind.In)
                {
                    refKind = ((refKind == RefKind.None) ? RefKind.In : ((RefKind)4));
                }
                refKinds[num] = refKind;
            }
            static bool isLambdaConversion(BoundExpression expr)
            {
                if (expr is BoundConversion boundConversion)
                {
                    return boundConversion.ConversionKind == ConversionKind.AnonymousFunction;
                }
                return false;
            }
        }

        private static ImmutableArray<IArgumentOperation> BuildArgumentsInEvaluationOrder(CSharpOperationFactory operationFactory, SyntaxNode syntax, Symbol methodOrIndexer, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ImmutableArray<BoundExpression> arguments, CSharpCompilation compilation)
        {
            ImmutableArray<ParameterSymbol> parameters = methodOrIndexer.GetParameters();
            ArrayBuilder<IArgumentOperation> instance = ArrayBuilder<IArgumentOperation>.GetInstance(parameters.Length);
            bool flag = false;
            for (int i = 0; i < arguments.Length; i++)
            {
                BoundExpression expression = arguments[i];
                int num = ((!argsToParamsOpt.IsDefault) ? argsToParamsOpt[i] : i);
                ParameterSymbol parameterSymbol = parameters[num];
                if (!flag)
                {
                    flag = num == parameters.Length - 1;
                }
                ArgumentKind kind = ((!defaultArguments[i]) ? ArgumentKind.Explicit : ArgumentKind.DefaultValue);
                if (IsBeginningOfParamArray(num, i, expanded, parameters.Length, arguments, argsToParamsOpt, out var numberOfParamArrayArguments))
                {
                    int num2 = i + numberOfParamArrayArguments;
                    kind = ArgumentKind.ParamArray;
                    ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance(numberOfParamArrayArguments);
                    for (int j = i; j < num2; j++)
                    {
                        instance2.Add(arguments[j]);
                    }
                    i = num2 - 1;
                    expression = CreateParamArrayArgument(syntax, parameterSymbol.Type, instance2.ToImmutableAndFree(), compilation, null);
                }
                instance.Add(operationFactory.CreateArgumentOperation(kind, parameterSymbol.GetPublicSymbol(), expression));
            }
            ParameterSymbol parameterSymbol2 = ((!parameters.IsEmpty) ? parameters[^1] : null);
            if (expanded && (object)parameterSymbol2 != null && !flag)
            {
                BoundExpression expression2 = CreateParamArrayArgument(syntax, parameterSymbol2.Type, ImmutableArray<BoundExpression>.Empty, compilation, null);
                ArgumentKind kind2 = ArgumentKind.ParamArray;
                instance.Add(operationFactory.CreateArgumentOperation(kind2, parameterSymbol2.GetPublicSymbol(), expression2));
            }
            return instance.ToImmutableAndFree();
        }

        private static bool IsBeginningOfParamArray(int parameterIndex, int argumentIndex, bool expanded, int parameterCount, ImmutableArray<BoundExpression> arguments, ImmutableArray<int> argsToParamsOpt, out int numberOfParamArrayArguments)
        {
            numberOfParamArrayArguments = 0;
            if (expanded && parameterIndex == parameterCount - 1)
            {
                int i;
                for (i = argumentIndex + 1; i < arguments.Length && ((!argsToParamsOpt.IsDefault) ? argsToParamsOpt[i] : i) == parameterCount - 1; i++)
                {
                }
                numberOfParamArrayArguments = i - argumentIndex;
                return true;
            }
            return false;
        }

        private BoundExpression BuildParamsArray(SyntaxNode syntax, Symbol methodOrIndexer, ImmutableArray<int> argsToParamsOpt, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<ParameterSymbol> parameters, BoundExpression tempStoreArgument)
        {
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            int num = parameters.Length - 1;
            if (tempStoreArgument != null)
            {
                instance.Add(tempStoreArgument);
            }
            else
            {
                for (int i = 0; i < rewrittenArguments.Length; i++)
                {
                    BoundExpression item = rewrittenArguments[i];
                    if (((!argsToParamsOpt.IsDefault) ? argsToParamsOpt[i] : i) == num)
                    {
                        instance.Add(item);
                    }
                }
            }
            TypeSymbol type = parameters[num].Type;
            ImmutableArray<BoundExpression> arrayArgs = instance.ToImmutableAndFree();
            if (arrayArgs.Length == 0 && !_inExpressionLambda && type is ArrayTypeSymbol arrayTypeSymbol && !arrayTypeSymbol.ElementType.IsPointerOrFunctionPointer())
            {
                MethodSymbol methodSymbol = _compilation.GetWellKnownTypeMember(WellKnownMember.System_Array__Empty) as MethodSymbol;
                if (methodSymbol != null)
                {
                    _diagnostics.ReportUseSite(methodSymbol, syntax);
                    methodSymbol = methodSymbol.Construct(ImmutableArray.Create(arrayTypeSymbol.ElementType));
                    return new BoundCall(syntax, null, methodSymbol, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, methodSymbol.ReturnType);
                }
            }
            return CreateParamArrayArgument(syntax, type, arrayArgs, _compilation, this);
        }

        private static BoundExpression CreateParamArrayArgument(SyntaxNode syntax, TypeSymbol paramArrayType, ImmutableArray<BoundExpression> arrayArgs, CSharpCompilation compilation, LocalRewriter? localRewriter)
        {
            TypeSymbol specialType = compilation.GetSpecialType(SpecialType.System_Int32);
            BoundExpression item = MakeLiteral(syntax, ConstantValue.Create(arrayArgs.Length), specialType, localRewriter);
            return new BoundArrayCreation(syntax, ImmutableArray.Create(item), new BoundArrayInitialization(syntax, arrayArgs)
            {
                WasCompilerGenerated = true
            }, paramArrayType)
            {
                WasCompilerGenerated = true
            };
        }

        private static BoundExpression MakeLiteral(SyntaxNode syntax, ConstantValue constantValue, TypeSymbol type, LocalRewriter? localRewriter)
        {
            if (localRewriter != null)
            {
                return localRewriter!.MakeLiteral(syntax, constantValue, type);
            }
            return new BoundLiteral(syntax, constantValue, type, constantValue.IsBad)
            {
                WasCompilerGenerated = true
            };
        }

        private static void OptimizeTemporaries(BoundExpression[] arguments, ArrayBuilder<BoundAssignmentOperator> storesToTemps, ArrayBuilder<LocalSymbol> temporariesBuilder)
        {
            if (storesToTemps.Count <= 0 || MergeArgumentsAndSideEffects(arguments, storesToTemps) <= 0)
            {
                return;
            }
            ArrayBuilder<BoundAssignmentOperator>.Enumerator enumerator = storesToTemps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundAssignmentOperator current = enumerator.Current;
                if (current != null)
                {
                    temporariesBuilder.Add(((BoundLocal)current.Left).LocalSymbol);
                }
            }
        }

        private static int MergeArgumentsAndSideEffects(BoundExpression[] arguments, ArrayBuilder<BoundAssignmentOperator> tempStores)
        {
            int num = tempStores.Count;
            int num2 = 0;
            for (int i = 0; i < arguments.Length; i++)
            {
                BoundExpression boundExpression = arguments[i];
                if (boundExpression == null || boundExpression.Kind != BoundKind.Local)
                {
                    continue;
                }
                int num3 = -1;
                for (int j = num2; j < tempStores.Count; j++)
                {
                    if (tempStores[j].Left == boundExpression)
                    {
                        num3 = j;
                        break;
                    }
                }
                if (num3 == -1)
                {
                    continue;
                }
                BoundExpression right = tempStores[num3].Right;
                tempStores[num3] = null;
                num--;
                if (num3 == num2)
                {
                    arguments[i] = right;
                }
                else
                {
                    BoundExpression[] array = new BoundExpression[num3 - num2];
                    for (int k = 0; k < array.Length; k++)
                    {
                        array[k] = tempStores[num2 + k];
                    }
                    arguments[i] = new BoundSequence(right.Syntax, ImmutableArray<LocalSymbol>.Empty, array.AsImmutableOrNull(), right, right.Type);
                }
                num2 = num3 + 1;
            }
            return num;
        }

        private void RewriteArgumentsForComCall(ImmutableArray<ParameterSymbol> parameters, BoundExpression[] actualArguments, ArrayBuilder<RefKind> argsRefKindsBuilder, ArrayBuilder<LocalSymbol> temporariesBuilder)
        {
            int num = actualArguments.Length;
            for (int i = 0; i < num; i++)
            {
                RefKind refKind = parameters[i].RefKind;
                if (argsRefKindsBuilder[i] == RefKind.None && refKind == RefKind.Ref)
                {
                    BoundExpression boundExpression = actualArguments[i];
                    if (boundExpression.Kind != BoundKind.Local || ((BoundLocal)boundExpression).LocalSymbol.RefKind != RefKind.Ref)
                    {
                        BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store);
                        actualArguments[i] = new BoundSequence(boundExpression.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)store), boundLocal, boundLocal.Type);
                        argsRefKindsBuilder[i] = RefKind.Ref;
                        temporariesBuilder.Add(boundLocal.LocalSymbol);
                    }
                }
            }
        }

        public override BoundNode VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            if (node.Invoked)
            {
                return node;
            }
            BoundExpression loweredReceiver = VisitExpression(node.Receiver);
            return _dynamicFactory.MakeDynamicGetMember(loweredReceiver, node.Name, node.Indexed).ToExpression();
        }

        public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            return VisitCompoundAssignmentOperator(node, used: true);
        }

        private BoundExpression VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, bool used)
        {
            BoundCompoundAssignmentOperator node2 = node;
            BoundExpression loweredRight = VisitExpression(node2.Right);
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            BinaryOperatorKind kind = node2.Operator.Kind;
            bool isChecked = kind.IsChecked();
            bool isDynamic = kind.IsDynamic();
            BinaryOperatorKind binaryOperatorKind = kind.Operator();
            BoundExpression transformedLHS = TransformCompoundAssignmentLHS(node2.Left, instance2, instance, isDynamic);
            BoundExpression boundExpression = MakeRValue(transformedLHS);
            BoundExpression boundExpression2;
            if (node2.Left.Kind == BoundKind.DynamicMemberAccess && (binaryOperatorKind == BinaryOperatorKind.Addition || binaryOperatorKind == BinaryOperatorKind.Subtraction))
            {
                ArrayBuilder<LocalSymbol> instance3 = ArrayBuilder<LocalSymbol>.GetInstance();
                ArrayBuilder<BoundExpression> instance4 = ArrayBuilder<BoundExpression>.GetInstance();
                BoundDynamicMemberAccess boundDynamicMemberAccess = (BoundDynamicMemberAccess)transformedLHS;
                BoundLocal boundLocal = _factory.StoreToTemp(_dynamicFactory.MakeDynamicIsEventTest(boundDynamicMemberAccess.Name, boundDynamicMemberAccess.Receiver).ToExpression(), out BoundAssignmentOperator store);
                instance3.Add(boundLocal.LocalSymbol);
                instance4.Add(store);
                boundExpression = _factory.StoreToTemp(boundExpression, out var store2);
                instance3.Add(((BoundLocal)boundExpression).LocalSymbol);
                BoundLocal boundLocal2 = _factory.StoreToTemp(_factory.Conditional(_factory.Not(boundLocal), store2, _factory.Null(store2.Type), store2.Type), out BoundAssignmentOperator store3);
                instance3.Add(boundLocal2.LocalSymbol);
                instance4.Add(store3);
                if (CanChangeValueBetweenReads(loweredRight))
                {
                    loweredRight = _factory.StoreToTemp(loweredRight, out var store4);
                    instance3.Add(((BoundLocal)loweredRight).LocalSymbol);
                    instance4.Add(store4);
                }
                LoweredDynamicOperation loweredDynamicOperation = _dynamicFactory.MakeDynamicEventAccessorInvocation(((binaryOperatorKind == BinaryOperatorKind.Addition) ? "add_" : "remove_") + boundDynamicMemberAccess.Name, boundDynamicMemberAccess.Receiver, loweredRight);
                boundExpression2 = rewriteAssignment(boundExpression);
                BoundExpression boundExpression3 = _factory.Conditional(boundLocal, loweredDynamicOperation.ToExpression(), boundExpression2, boundExpression2.Type);
                boundExpression2 = new BoundSequence(node2.Syntax, instance3.ToImmutableAndFree(), instance4.ToImmutableAndFree(), boundExpression3, boundExpression3.Type);
            }
            else
            {
                boundExpression2 = rewriteAssignment(boundExpression);
            }
            BoundExpression result = ((instance.Count == 0 && instance2.Count == 0) ? boundExpression2 : new BoundSequence(node2.Syntax, instance.ToImmutable(), instance2.ToImmutable(), boundExpression2, boundExpression2.Type));
            instance.Free();
            instance2.Free();
            return result;
            BoundExpression rewriteAssignment(BoundExpression leftRead)
            {
                SyntaxNode syntax = node2.Syntax;
                BoundExpression loweredLeft = (isDynamic ? leftRead : MakeConversionNode(syntax, leftRead, node2.LeftConversion, node2.Operator.LeftType, isChecked));
                BoundExpression boundExpression4 = MakeBinaryOperator(syntax, node2.Operator.Kind, loweredLeft, loweredRight, node2.Operator.ReturnType, node2.Operator.Method, isPointerElementAccess: false, isCompoundAssignment: true);
                BoundExpression rewrittenRight = MakeConversionNode(syntax, boundExpression4, node2.FinalConversion, node2.Left.Type, explicitCastInCode: isDynamic, @checked: isChecked);
                return MakeAssignmentOperator(syntax, transformedLHS, rewrittenRight, node2.Left.Type, used, isChecked, isCompoundAssignment: true);
            }
        }

        private BoundExpression? TransformPropertyOrEventReceiver(Symbol propertyOrEvent, BoundExpression? receiverOpt, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            if (receiverOpt == null || propertyOrEvent.IsStatic || !CanChangeValueBetweenReads(receiverOpt))
            {
                return receiverOpt;
            }
            BoundExpression boundExpression = VisitExpression(receiverOpt);
            bool flag = boundExpression.Type!.IsValueType || boundExpression.Type!.Kind == SymbolKind.TypeParameter;
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store, flag ? RefKind.Ref : RefKind.None);
            stores.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            return boundLocal;
        }

        private BoundDynamicMemberAccess TransformDynamicMemberAccess(BoundDynamicMemberAccess memberAccess, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            if (!CanChangeValueBetweenReads(memberAccess.Receiver))
            {
                return memberAccess;
            }
            BoundExpression argument = VisitExpression(memberAccess.Receiver);
            BoundLocal boundLocal = _factory.StoreToTemp(argument, out BoundAssignmentOperator store);
            stores.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            return new BoundDynamicMemberAccess(memberAccess.Syntax, boundLocal, memberAccess.TypeArgumentsOpt, memberAccess.Name, memberAccess.Invoked, memberAccess.Indexed, memberAccess.Type);
        }

        private BoundIndexerAccess TransformIndexerAccess(BoundIndexerAccess indexerAccess, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            BoundExpression receiverOpt = indexerAccess.ReceiverOpt;
            BoundExpression receiverOpt2;
            if (CanChangeValueBetweenReads(receiverOpt))
            {
                BoundExpression boundExpression = VisitExpression(receiverOpt);
                bool flag = boundExpression.Type!.IsValueType || boundExpression.Type!.Kind == SymbolKind.TypeParameter;
                BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store, flag ? RefKind.Ref : RefKind.None);
                receiverOpt2 = boundLocal;
                stores.Add(store);
                temps.Add(boundLocal.LocalSymbol);
            }
            else
            {
                receiverOpt2 = VisitExpression(receiverOpt);
            }
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(indexerAccess.Arguments);
            SyntaxNode syntax = indexerAccess.Syntax;
            PropertySymbol indexer = indexerAccess.Indexer;
            ImmutableArray<RefKind> argumentRefKindsOpt = indexerAccess.ArgumentRefKindsOpt;
            bool expanded = indexerAccess.Expanded;
            ImmutableArray<int> argsToParamsOpt = indexerAccess.ArgsToParamsOpt;
            ImmutableArray<ParameterSymbol> parameters = indexer.Parameters;
            BoundExpression[] array = new BoundExpression[parameters.Length];
            ArrayBuilder<BoundAssignmentOperator> instance = ArrayBuilder<BoundAssignmentOperator>.GetInstance(rewrittenArguments.Length);
            ArrayBuilder<RefKind> instance2 = ArrayBuilder<RefKind>.GetInstance(parameters.Length, RefKind.None);
            BuildStoresToTemps(expanded, argsToParamsOpt, parameters, argumentRefKindsOpt, rewrittenArguments, forceLambdaSpilling: true, array, instance2, instance);
            if (expanded)
            {
                BoundExpression argument = BuildParamsArray(syntax, indexer, argsToParamsOpt, rewrittenArguments, parameters, array[^1]);
                BoundLocal boundLocal2 = _factory.StoreToTemp(argument, out BoundAssignmentOperator store2);
                stores.Add(store2);
                temps.Add(boundLocal2.LocalSymbol);
                array[^1] = boundLocal2;
            }
            if ((object)indexer.GetOwnOrInheritedGetMethod() == null)
            {
                indexer.GetOwnOrInheritedSetMethod();
            }
            if (indexer.ContainingType.IsComImport)
            {
                RewriteArgumentsForComCall(parameters, array, instance2, temps);
            }
            rewrittenArguments = array.AsImmutableOrNull();
            ArrayBuilder<BoundAssignmentOperator>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundAssignmentOperator current = enumerator.Current;
                temps.Add(((BoundLocal)current.Left).LocalSymbol);
                stores.Add(current);
            }
            instance.Free();
            argumentRefKindsOpt = GetRefKindsOrNull(instance2);
            instance2.Free();
            return new BoundIndexerAccess(syntax, receiverOpt2, indexer, rewrittenArguments, default(ImmutableArray<string>), argumentRefKindsOpt, expanded: false, default(ImmutableArray<int>), default(BitVector), indexerAccess.Type);
        }

        private BoundExpression TransformPatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess indexerAccess, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps, bool isDynamicAssignment)
        {
            BoundSequence boundSequence = VisitIndexOrRangePatternIndexerAccess(indexerAccess, isLeftOfAssignment: true);
            stores.AddRange(boundSequence.SideEffects);
            temps.AddRange(boundSequence.Locals);
            return TransformCompoundAssignmentLHS(boundSequence.Value, stores, temps, isDynamicAssignment);
        }

        private bool TransformCompoundAssignmentFieldOrEventAccessReceiver(Symbol fieldOrEvent, ref BoundExpression? receiver, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            if (fieldOrEvent.IsStatic)
            {
                return true;
            }
            if (!CanChangeValueBetweenReads(receiver))
            {
                return true;
            }
            if (!receiver!.Type!.IsReferenceType)
            {
                return false;
            }
            BoundExpression boundExpression = VisitExpression(receiver);
            if (boundExpression.Type.IsTypeParameter())
            {
                NamedTypeSymbol containingType = fieldOrEvent.ContainingType;
                boundExpression = BoxReceiver(boundExpression, containingType);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store);
            stores.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            receiver = boundLocal;
            return true;
        }

        private BoundDynamicIndexerAccess TransformDynamicIndexerAccess(BoundDynamicIndexerAccess indexerAccess, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            BoundExpression receiver;
            if (CanChangeValueBetweenReads(indexerAccess.Receiver))
            {
                BoundLocal boundLocal = _factory.StoreToTemp(VisitExpression(indexerAccess.Receiver), out BoundAssignmentOperator store);
                stores.Add(store);
                temps.Add(boundLocal.LocalSymbol);
                receiver = boundLocal;
            }
            else
            {
                receiver = indexerAccess.Receiver;
            }
            ImmutableArray<BoundExpression> arguments = indexerAccess.Arguments;
            BoundExpression[] array = new BoundExpression[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                if (CanChangeValueBetweenReads(arguments[i]))
                {
                    BoundLocal boundLocal2 = _factory.StoreToTemp(VisitExpression(arguments[i]), out BoundAssignmentOperator store2, (indexerAccess.ArgumentRefKindsOpt.RefKinds(i) != 0) ? RefKind.Ref : RefKind.None);
                    stores.Add(store2);
                    temps.Add(boundLocal2.LocalSymbol);
                    array[i] = boundLocal2;
                }
                else
                {
                    array[i] = arguments[i];
                }
            }
            return new BoundDynamicIndexerAccess(indexerAccess.Syntax, receiver, array.AsImmutableOrNull(), indexerAccess.ArgumentNamesOpt, indexerAccess.ArgumentRefKindsOpt, indexerAccess.ApplicableIndexers, indexerAccess.Type);
        }

        private BoundExpression TransformCompoundAssignmentLHS(BoundExpression originalLHS, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps, bool isDynamicAssignment)
        {
            switch (originalLHS.Kind)
            {
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)originalLHS;
                        if (boundPropertyAccess.PropertySymbol.RefKind == RefKind.None)
                        {
                            return boundPropertyAccess.Update(TransformPropertyOrEventReceiver(boundPropertyAccess.PropertySymbol, boundPropertyAccess.ReceiverOpt, stores, temps), boundPropertyAccess.PropertySymbol, boundPropertyAccess.ResultKind, boundPropertyAccess.Type);
                        }
                        break;
                    }
                case BoundKind.IndexerAccess:
                    if (((BoundIndexerAccess)originalLHS).Indexer.RefKind == RefKind.None)
                    {
                        return TransformIndexerAccess((BoundIndexerAccess)originalLHS, stores, temps);
                    }
                    break;
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    {
                        BoundIndexOrRangePatternIndexerAccess boundIndexOrRangePatternIndexerAccess = (BoundIndexOrRangePatternIndexerAccess)originalLHS;
                        Symbol patternSymbol = boundIndexOrRangePatternIndexerAccess.PatternSymbol;
                        RefKind refKind;
                        if (!(patternSymbol is PropertySymbol propertySymbol))
                        {
                            if (!(patternSymbol is MethodSymbol methodSymbol))
                            {
                                throw ExceptionUtilities.UnexpectedValue(patternSymbol);
                            }
                            refKind = methodSymbol.RefKind;
                        }
                        else
                        {
                            refKind = propertySymbol.RefKind;
                        }
                        if (refKind == RefKind.None)
                        {
                            return TransformPatternIndexerAccess(boundIndexOrRangePatternIndexerAccess, stores, temps, isDynamicAssignment);
                        }
                        break;
                    }
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)originalLHS;
                        BoundExpression receiver2 = boundFieldAccess.ReceiverOpt;
                        if (TransformCompoundAssignmentFieldOrEventAccessReceiver(boundFieldAccess.FieldSymbol, ref receiver2, stores, temps))
                        {
                            return MakeFieldAccess(boundFieldAccess.Syntax, receiver2, boundFieldAccess.FieldSymbol, boundFieldAccess.ConstantValueOpt, boundFieldAccess.ResultKind, boundFieldAccess.Type, boundFieldAccess);
                        }
                        break;
                    }
                case BoundKind.ArrayAccess:
                    {
                        BoundArrayAccess boundArrayAccess = (BoundArrayAccess)originalLHS;
                        if (isDynamicAssignment || !IsInvariantArray(boundArrayAccess.Expression.Type))
                        {
                            BoundExpression loweredExpression = VisitExpression(boundArrayAccess.Expression);
                            ImmutableArray<BoundExpression> loweredIndices = VisitList(boundArrayAccess.Indices);
                            return SpillArrayElementAccess(loweredExpression, loweredIndices, stores, temps);
                        }
                        break;
                    }
                case BoundKind.DynamicMemberAccess:
                    return TransformDynamicMemberAccess((BoundDynamicMemberAccess)originalLHS, stores, temps);
                case BoundKind.DynamicIndexerAccess:
                    return TransformDynamicIndexerAccess((BoundDynamicIndexerAccess)originalLHS, stores, temps);
                case BoundKind.ThisReference:
                case BoundKind.Local:
                case BoundKind.PseudoVariable:
                case BoundKind.Parameter:
                    return originalLHS;
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)originalLHS;
                        BoundExpression receiver = boundEventAccess.ReceiverOpt;
                        if (boundEventAccess.EventSymbol.IsWindowsRuntimeEvent)
                        {
                            return boundEventAccess.Update(TransformPropertyOrEventReceiver(boundEventAccess.EventSymbol, boundEventAccess.ReceiverOpt, stores, temps), boundEventAccess.EventSymbol, boundEventAccess.IsUsableAsField, boundEventAccess.ResultKind, boundEventAccess.Type);
                        }
                        if (TransformCompoundAssignmentFieldOrEventAccessReceiver(boundEventAccess.EventSymbol, ref receiver, stores, temps))
                        {
                            return MakeEventAccess(boundEventAccess.Syntax, receiver, boundEventAccess.EventSymbol, boundEventAccess.ConstantValue, boundEventAccess.ResultKind, boundEventAccess.Type);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(originalLHS.Kind);
                case BoundKind.PointerIndirectionOperator:
                case BoundKind.PointerElementAccess:
                case BoundKind.FunctionPointerInvocation:
                case BoundKind.RefValueOperator:
                case BoundKind.AssignmentOperator:
                case BoundKind.ConditionalOperator:
                case BoundKind.Call:
                    break;
            }
            BoundExpression argument = VisitExpression(originalLHS);
            BoundLocal boundLocal = _factory.StoreToTemp(argument, out BoundAssignmentOperator store, RefKind.Ref);
            stores.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            return boundLocal;
        }

        private static bool IsInvariantArray(TypeSymbol? type)
        {
            return (type as ArrayTypeSymbol)?.ElementType.IsSealed ?? false;
        }

        private BoundExpression BoxReceiver(BoundExpression rewrittenReceiver, NamedTypeSymbol memberContainingType)
        {
            return MakeConversionNode(rewrittenReceiver.Syntax, rewrittenReceiver, Conversion.Boxing, memberContainingType, @checked: false, explicitCastInCode: false, rewrittenReceiver.ConstantValue);
        }

        private BoundExpression SpillArrayElementAccess(BoundExpression loweredExpression, ImmutableArray<BoundExpression> loweredIndices, ArrayBuilder<BoundExpression> stores, ArrayBuilder<LocalSymbol> temps)
        {
            BoundLocal boundLocal = _factory.StoreToTemp(loweredExpression, out BoundAssignmentOperator store);
            stores.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            BoundLocal array = boundLocal;
            BoundExpression[] array2 = new BoundExpression[loweredIndices.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                if (CanChangeValueBetweenReads(loweredIndices[i]))
                {
                    BoundLocal boundLocal2 = _factory.StoreToTemp(loweredIndices[i], out BoundAssignmentOperator store2);
                    stores.Add(store2);
                    temps.Add(boundLocal2.LocalSymbol);
                    array2[i] = boundLocal2;
                }
                else
                {
                    array2[i] = loweredIndices[i];
                }
            }
            return _factory.ArrayAccess(array, array2);
        }

        internal static bool CanChangeValueBetweenReads(BoundExpression expression, bool localsMayBeAssignedOrCaptured = true, bool structThisCanChangeValueBetweenReads = false)
        {
            if (expression.IsDefaultValue())
            {
                return false;
            }
            if (expression.ConstantValue != null)
            {
                return !ConstantValueIsTrivial(expression.Type);
            }
            switch (expression.Kind)
            {
                case BoundKind.ThisReference:
                    if (structThisCanChangeValueBetweenReads)
                    {
                        return ((BoundThisReference)expression).Type.IsStructType();
                    }
                    return false;
                case BoundKind.BaseReference:
                    return false;
                case BoundKind.Literal:
                    return !ConstantValueIsTrivial(expression.Type);
                case BoundKind.Parameter:
                    if (!localsMayBeAssignedOrCaptured)
                    {
                        return ((BoundParameter)expression).ParameterSymbol.RefKind != RefKind.None;
                    }
                    return true;
                case BoundKind.Local:
                    if (!localsMayBeAssignedOrCaptured)
                    {
                        return ((BoundLocal)expression).LocalSymbol.RefKind != RefKind.None;
                    }
                    return true;
                case BoundKind.TypeExpression:
                    return false;
                default:
                    return true;
            }
        }

        internal static bool ReadIsSideeffecting(BoundExpression expression)
        {
            if (expression.ConstantValue != null)
            {
                return false;
            }
            if (expression.IsDefaultValue())
            {
                return false;
            }
            switch (expression.Kind)
            {
                case BoundKind.Literal:
                case BoundKind.ThisReference:
                case BoundKind.BaseReference:
                case BoundKind.Local:
                case BoundKind.Parameter:
                case BoundKind.Lambda:
                    return false;
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)expression;
                        if (!boundConversion.ConversionHasSideEffects())
                        {
                            return ReadIsSideeffecting(boundConversion.Operand);
                        }
                        return true;
                    }
                case BoundKind.PassByCopy:
                    return ReadIsSideeffecting(((BoundPassByCopy)expression).Expression);
                case BoundKind.ObjectCreationExpression:
                    if (expression.Type.IsNullableType())
                    {
                        BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)expression;
                        if (boundObjectCreationExpression.Arguments.Length == 1)
                        {
                            return ReadIsSideeffecting(boundObjectCreationExpression.Arguments[0]);
                        }
                        return false;
                    }
                    return true;
                case BoundKind.Call:
                    {
                        BoundCall boundCall = (BoundCall)expression;
                        MethodSymbol method = boundCall.Method;
                        NamedTypeSymbol containingType = method.ContainingType;
                        if ((object)containingType != null && containingType.IsNullableType() && (IsSpecialMember(method, SpecialMember.System_Nullable_T_GetValueOrDefault) || IsSpecialMember(method, SpecialMember.System_Nullable_T_get_HasValue)))
                        {
                            return ReadIsSideeffecting(boundCall.ReceiverOpt);
                        }
                        return true;
                    }
                default:
                    return true;
            }
        }

        private static bool IsSpecialMember(MethodSymbol method, SpecialMember specialMember)
        {
            method = method.OriginalDefinition;
            return method.ContainingAssembly?.GetSpecialTypeMember(specialMember) == method;
        }

        private static bool ConstantValueIsTrivial(TypeSymbol? type)
        {
            if ((object)type != null && !type!.SpecialType.IsClrInteger() && !type!.IsReferenceType)
            {
                return type.IsEnumType();
            }
            return true;
        }

        public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
        {
            return RewriteConditionalAccess(node, used: true);
        }

        public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal BoundExpression? RewriteConditionalAccess(BoundConditionalAccess node, bool used)
        {
            BoundExpression boundExpression = VisitExpression(node.Receiver);
            TypeSymbol type = boundExpression.Type;
            if (boundExpression.IsDefaultValue() && type.IsReferenceType)
            {
                return _factory.Default(node.Type);
            }
            ConditionalAccessLoweringKind conditionalAccessLoweringKind = (node.AccessExpression.Type.IsDynamic() ? ((!CanChangeValueBetweenReads(boundExpression)) ? ConditionalAccessLoweringKind.Conditional : ConditionalAccessLoweringKind.ConditionalCaptureReceiverByVal) : ConditionalAccessLoweringKind.LoweredConditionalAccess);
            BoundExpression currentConditionalAccessTarget = _currentConditionalAccessTarget;
            int id = ++_currentConditionalAccessID;
            LocalSymbol localSymbol = null;
            switch (conditionalAccessLoweringKind)
            {
                case ConditionalAccessLoweringKind.LoweredConditionalAccess:
                    _currentConditionalAccessTarget = new BoundConditionalReceiver(boundExpression.Syntax, id, type);
                    break;
                case ConditionalAccessLoweringKind.Conditional:
                    _currentConditionalAccessTarget = boundExpression;
                    break;
                case ConditionalAccessLoweringKind.ConditionalCaptureReceiverByVal:
                    localSymbol = _factory.SynthesizedLocal(type);
                    _currentConditionalAccessTarget = _factory.Local(localSymbol);
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(conditionalAccessLoweringKind);
            }
            BoundExpression boundExpression2;
            if (used)
            {
                boundExpression2 = VisitExpression(node.AccessExpression);
            }
            else
            {
                boundExpression2 = VisitUnusedExpression(node.AccessExpression);
                if (boundExpression2 == null)
                {
                    return null;
                }
            }
            _currentConditionalAccessTarget = currentConditionalAccessTarget;
            TypeSymbol type2 = VisitType(node.Type);
            TypeSymbol typeSymbol = node.Type;
            TypeSymbol type3 = boundExpression2.Type;
            if (type3.IsVoidType())
            {
                type2 = (typeSymbol = type3);
            }
            if (!TypeSymbol.Equals(type3, typeSymbol, TypeCompareKind.ConsiderEverything) && typeSymbol.IsNullableType())
            {
                boundExpression2 = _factory.New((NamedTypeSymbol)typeSymbol, boundExpression2);
            }
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Object);
            BoundExpression boundExpression3;
            switch (conditionalAccessLoweringKind)
            {
                case ConditionalAccessLoweringKind.LoweredConditionalAccess:
                    boundExpression3 = new BoundLoweredConditionalAccess(node.Syntax, boundExpression, type.IsNullableType() ? UnsafeGetNullableMethod(node.Syntax, boundExpression.Type, SpecialMember.System_Nullable_T_get_HasValue) : null, boundExpression2, null, id, type2);
                    break;
                case ConditionalAccessLoweringKind.ConditionalCaptureReceiverByVal:
                    boundExpression = _factory.MakeSequence(_factory.AssignmentExpression(_factory.Local(localSymbol), boundExpression), _factory.Local(localSymbol));
                    goto case ConditionalAccessLoweringKind.Conditional;
                case ConditionalAccessLoweringKind.Conditional:
                    {
                        BoundBinaryOperator rewrittenCondition = _factory.ObjectNotEqual(_factory.Convert(specialType, boundExpression), _factory.Null(specialType));
                        BoundExpression rewrittenConsequence = boundExpression2;
                        boundExpression3 = RewriteConditionalOperator(node.Syntax, rewrittenCondition, rewrittenConsequence, _factory.Default(typeSymbol), null, typeSymbol, isRef: false);
                        if (localSymbol != null)
                        {
                            boundExpression3 = _factory.MakeSequence(localSymbol, boundExpression3);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(conditionalAccessLoweringKind);
            }
            return boundExpression3;
        }

        public override BoundNode VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            BoundExpression boundExpression = _currentConditionalAccessTarget;
            if (boundExpression.Type.IsNullableType())
            {
                boundExpression = MakeOptimizedGetValueOrDefault(node.Syntax, boundExpression);
            }
            return boundExpression;
        }

        public override BoundNode VisitConditionalOperator(BoundConditionalOperator node)
        {
            BoundExpression boundExpression = VisitExpression(node.Condition);
            BoundExpression boundExpression2 = VisitExpression(node.Consequence);
            BoundExpression boundExpression3 = VisitExpression(node.Alternative);
            if (boundExpression.ConstantValue == null)
            {
                return node.Update(node.IsRef, boundExpression, boundExpression2, boundExpression3, node.ConstantValueOpt, node.NaturalTypeOpt, node.WasTargetTyped, node.Type);
            }
            return RewriteConditionalOperator(node.Syntax, boundExpression, boundExpression2, boundExpression3, node.ConstantValueOpt, node.Type, node.IsRef);
        }

        private static BoundExpression RewriteConditionalOperator(SyntaxNode syntax, BoundExpression rewrittenCondition, BoundExpression rewrittenConsequence, BoundExpression rewrittenAlternative, ConstantValue? constantValueOpt, TypeSymbol rewrittenType, bool isRef)
        {
            ConstantValue constantValue = rewrittenCondition.ConstantValue;
            if (constantValue == ConstantValue.True)
            {
                return rewrittenConsequence;
            }
            if (constantValue == ConstantValue.False)
            {
                return rewrittenAlternative;
            }
            return new BoundConditionalOperator(syntax, isRef, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, constantValueOpt, rewrittenType, wasTargetTyped: false, rewrittenType);
        }

        public override BoundNode VisitContinueStatement(BoundContinueStatement node)
        {
            BoundStatement boundStatement = new BoundGotoStatement(node.Syntax, node.Label, node.HasErrors);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentContinueStatement(node, boundStatement);
            }
            return boundStatement;
        }

        public override BoundNode VisitConversion(BoundConversion node)
        {
            if (node.ConversionKind == ConversionKind.InterpolatedString)
            {
                return RewriteInterpolatedStringConversion(node);
            }
            ConversionKind conversionKind = node.ConversionKind;
            if (conversionKind == ConversionKind.SwitchExpression || conversionKind == ConversionKind.ConditionalExpression)
            {
                return Visit(node.Operand);
            }
            TypeSymbol typeSymbol = VisitType(node.Type);
            bool inExpressionLambda = _inExpressionLambda;
            _inExpressionLambda = _inExpressionLambda || (node.ConversionKind == ConversionKind.AnonymousFunction && !inExpressionLambda && typeSymbol.IsExpressionTree());
            BoundExpression rewrittenOperand = VisitExpression(node.Operand);
            _inExpressionLambda = inExpressionLambda;
            BoundExpression result = MakeConversionNode(node, node.Syntax, rewrittenOperand, node.Conversion, node.Checked, node.ExplicitCastInCode, node.ConstantValue, typeSymbol);
            _ = node.Type;
            return result;
        }

        private static bool IsFloatingPointExpressionOfUnknownPrecision(BoundExpression rewrittenNode)
        {
            if (rewrittenNode == null)
            {
                return false;
            }
            if (rewrittenNode.ConstantValue != null)
            {
                return false;
            }
            TypeSymbol type = rewrittenNode.Type;
            if (type.SpecialType != SpecialType.System_Double && type.SpecialType != SpecialType.System_Single)
            {
                return false;
            }
            switch (rewrittenNode.Kind)
            {
                case BoundKind.Sequence:
                    return IsFloatingPointExpressionOfUnknownPrecision(((BoundSequence)rewrittenNode).Value);
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)rewrittenNode;
                        if (boundConversion.ConversionKind == ConversionKind.Identity)
                        {
                            return !boundConversion.ExplicitCastInCode;
                        }
                        return false;
                    }
                default:
                    return true;
            }
        }

        private BoundExpression MakeConversionNode(BoundConversion? oldNodeOpt, SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, ConstantValue? constantValueOpt, TypeSymbol rewrittenType)
        {
            BoundExpression boundExpression = MakeConversionNodeCore(oldNodeOpt, syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, constantValueOpt, rewrittenType);
            if (!_inExpressionLambda && explicitCastInCode && IsFloatingPointExpressionOfUnknownPrecision(boundExpression))
            {
                boundExpression = new BoundConversion(syntax, boundExpression, Conversion.Identity, isBaseConversion: false, @checked: false, explicitCastInCode: true, null, null, boundExpression.Type);
            }
            return boundExpression;
        }

        private BoundExpression MakeConversionNodeCore(BoundConversion? oldNodeOpt, SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, ConstantValue? constantValueOpt, TypeSymbol rewrittenType)
        {
            if (_inExpressionLambda)
            {
                @checked = @checked && NeedsCheckedConversionInExpressionTree(rewrittenOperand.Type, rewrittenType, explicitCastInCode);
            }
            ConversionGroup conversionGroupOpt;
            switch (conversion.Kind)
            {
                case ConversionKind.Identity:
                    if (!_inExpressionLambda && rewrittenOperand.Type!.Equals(rewrittenType, TypeCompareKind.ConsiderEverything))
                    {
                        if (!explicitCastInCode)
                        {
                            return rewrittenOperand;
                        }
                        if (!IsFloatingPointExpressionOfUnknownPrecision(rewrittenOperand))
                        {
                            return rewrittenOperand;
                        }
                    }
                    break;
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return RewriteUserDefinedConversion(syntax, rewrittenOperand, conversion, rewrittenType);
                case ConversionKind.IntPtr:
                    return RewriteIntPtrConversion(oldNodeOpt, syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, constantValueOpt, rewrittenType);
                case ConversionKind.ImplicitNullable:
                case ConversionKind.ExplicitNullable:
                    return RewriteNullableConversion(syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, rewrittenType);
                case ConversionKind.Boxing:
                    if (!_inExpressionLambda)
                    {
                        if (NullableNeverHasValue(rewrittenOperand))
                        {
                            return new BoundDefaultExpression(syntax, rewrittenType);
                        }
                        BoundExpression boundExpression = NullableAlwaysHasValue(rewrittenOperand);
                        if (boundExpression != null)
                        {
                            return MakeConversionNode(oldNodeOpt, syntax, boundExpression, conversion, @checked, explicitCastInCode, constantValueOpt, rewrittenType);
                        }
                    }
                    break;
                case ConversionKind.NullLiteral:
                case ConversionKind.DefaultLiteral:
                    if (!_inExpressionLambda || !explicitCastInCode)
                    {
                        return new BoundDefaultExpression(syntax, rewrittenType);
                    }
                    break;
                case ConversionKind.ImplicitReference:
                case ConversionKind.ExplicitReference:
                    if (rewrittenOperand.IsDefaultValue() && (!_inExpressionLambda || !explicitCastInCode))
                    {
                        return new BoundDefaultExpression(syntax, rewrittenType);
                    }
                    break;
                case ConversionKind.ImplicitConstant:
                    conversion = Conversion.ExplicitNumeric;
                    @checked = false;
                    goto case ConversionKind.ImplicitNumeric;
                case ConversionKind.ImplicitNumeric:
                case ConversionKind.ExplicitNumeric:
                    if (rewrittenOperand.IsDefaultValue() && (!_inExpressionLambda || !explicitCastInCode))
                    {
                        return new BoundDefaultExpression(syntax, rewrittenType);
                    }
                    if (rewrittenType.SpecialType == SpecialType.System_Decimal || rewrittenOperand.Type!.SpecialType == SpecialType.System_Decimal)
                    {
                        return RewriteDecimalConversion(syntax, rewrittenOperand, rewrittenOperand.Type, rewrittenType, @checked, conversion.Kind.IsImplicitConversion(), constantValueOpt);
                    }
                    break;
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ExplicitTupleLiteral:
                    return rewrittenOperand;
                case ConversionKind.ImplicitThrow:
                    {
                        BoundThrowExpression boundThrowExpression = (BoundThrowExpression)rewrittenOperand;
                        return _factory.ThrowExpression(boundThrowExpression.Expression, rewrittenType);
                    }
                case ConversionKind.ImplicitEnumeration:
                    if (rewrittenType.IsNullableType())
                    {
                        BoundExpression rewrittenOperand2 = MakeConversionNode(oldNodeOpt, syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, constantValueOpt, rewrittenType.GetNullableUnderlyingType());
                        Conversion conversion3 = new Conversion(ConversionKind.ImplicitNullable, Conversion.IdentityUnderlying);
                        return MakeConversionNode(oldNodeOpt, syntax, rewrittenOperand2, conversion3, @checked, explicitCastInCode, constantValueOpt, rewrittenType);
                    }
                    goto case ConversionKind.ExplicitEnumeration;
                case ConversionKind.ExplicitEnumeration:
                    if (!rewrittenType.IsNullableType() && rewrittenOperand.IsDefaultValue() && (!_inExpressionLambda || !explicitCastInCode))
                    {
                        return new BoundDefaultExpression(syntax, rewrittenType);
                    }
                    if (rewrittenType.SpecialType == SpecialType.System_Decimal)
                    {
                        NamedTypeSymbol enumUnderlyingType = rewrittenOperand.Type.GetEnumUnderlyingType();
                        rewrittenOperand = MakeConversionNode(rewrittenOperand, enumUnderlyingType, @checked: false);
                        return RewriteDecimalConversion(syntax, rewrittenOperand, enumUnderlyingType, rewrittenType, @checked, isImplicit: false, constantValueOpt);
                    }
                    if (rewrittenOperand.Type!.SpecialType == SpecialType.System_Decimal)
                    {
                        NamedTypeSymbol enumUnderlyingType2 = rewrittenType.GetEnumUnderlyingType();
                        BoundExpression operand = RewriteDecimalConversion(syntax, rewrittenOperand, rewrittenOperand.Type, enumUnderlyingType2, @checked, isImplicit: false, constantValueOpt);
                        Conversion conversion2 = conversion;
                        conversionGroupOpt = oldNodeOpt?.ConversionGroupOpt;
                        return new BoundConversion(syntax, operand, conversion2, isBaseConversion: false, @checked: false, explicitCastInCode, constantValueOpt, conversionGroupOpt, rewrittenType);
                    }
                    break;
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitDynamic:
                    return _dynamicFactory.MakeDynamicConversion(rewrittenOperand, explicitCastInCode || conversion.Kind == ConversionKind.ExplicitDynamic, conversion.IsArrayIndex, @checked, rewrittenType).ToExpression();
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ExplicitTuple:
                    return RewriteTupleConversion(syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, (NamedTypeSymbol)rewrittenType);
                case ConversionKind.MethodGroup:
                    {
                        if (oldNodeOpt != null)
                        {
                            TypeSymbol type = oldNodeOpt!.Type;
                            if ((object)type != null && type.TypeKind == TypeKind.FunctionPointer)
                            {
                                _ = (BoundMethodGroup)rewrittenOperand;
                                return new BoundFunctionPointerLoad(oldNodeOpt!.Syntax, oldNodeOpt!.SymbolOpt, type, hasErrors: false);
                            }
                        }
                        BoundMethodGroup boundMethodGroup = (BoundMethodGroup)rewrittenOperand;
                        MethodSymbol symbolOpt = oldNodeOpt!.SymbolOpt;
                        SyntaxNode syntax2 = _factory.Syntax;
                        _factory.Syntax = (boundMethodGroup.ReceiverOpt ?? boundMethodGroup)!.Syntax;
                        BoundExpression argument = ((!symbolOpt.RequiresInstanceReceiver && !oldNodeOpt!.IsExtensionMethod) ? _factory.Type(symbolOpt.ContainingType) : boundMethodGroup.ReceiverOpt);
                        _factory.Syntax = syntax2;
                        return new BoundDelegateCreationExpression(syntax, argument, symbolOpt, oldNodeOpt!.IsExtensionMethod, rewrittenType);
                    }
            }
            if (oldNodeOpt == null)
            {
                return new BoundConversion(syntax, rewrittenOperand, conversion, isBaseConversion: false, @checked, explicitCastInCode, constantValueOpt, null, rewrittenType);
            }
            BoundExpression operand2 = rewrittenOperand;
            Conversion conversion4 = conversion;
            bool isBaseConversion = oldNodeOpt!.IsBaseConversion;
            bool checked2 = @checked;
            conversionGroupOpt = oldNodeOpt!.ConversionGroupOpt;
            return oldNodeOpt!.Update(operand2, conversion4, isBaseConversion, checked2, explicitCastInCode, constantValueOpt, conversionGroupOpt, rewrittenType);
        }

        private static bool NeedsCheckedConversionInExpressionTree(TypeSymbol? source, TypeSymbol target, bool explicitCastInCode)
        {
            if ((object)source == null)
            {
                return false;
            }
            SpecialType specialType = GetUnderlyingSpecialType(source);
            SpecialType specialType2 = GetUnderlyingSpecialType(target);
            if ((explicitCastInCode || specialType != specialType2) && IsInRange(specialType, SpecialType.System_Char, SpecialType.System_Double))
            {
                return IsInRange(specialType2, SpecialType.System_Char, SpecialType.System_UInt64);
            }
            return false;
            static SpecialType GetUnderlyingSpecialType(TypeSymbol type)
            {
                return type.StrippedType().EnumUnderlyingTypeOrSelf().SpecialType;
            }
            static bool IsInRange(SpecialType type, SpecialType low, SpecialType high)
            {
                if (low <= type)
                {
                    return type <= high;
                }
                return false;
            }
        }

        private BoundExpression MakeConversionNode(BoundExpression rewrittenOperand, TypeSymbol rewrittenType, bool @checked, bool acceptFailingConversion = false)
        {
            Conversion conversion = MakeConversion(rewrittenOperand, rewrittenType, _compilation, _diagnostics, acceptFailingConversion);
            if (!conversion.IsValid)
            {
                return _factory.NullOrDefault(rewrittenType);
            }
            return MakeConversionNode(rewrittenOperand.Syntax, rewrittenOperand, conversion, rewrittenType, @checked);
        }

        private static Conversion MakeConversion(BoundExpression rewrittenOperand, TypeSymbol rewrittenType, CSharpCompilation compilation, BindingDiagnosticBag diagnostics, bool acceptFailingConversion)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, compilation.Assembly);
            Conversion result = compilation.Conversions.ClassifyConversionFromType(rewrittenOperand.Type, rewrittenType, ref useSiteInfo);
            diagnostics.Add(rewrittenOperand.Syntax, useSiteInfo);
            if (!result.IsValid && (!acceptFailingConversion || (rewrittenOperand.Type!.SpecialType != SpecialType.System_Decimal && rewrittenOperand.Type!.SpecialType != SpecialType.System_DateTime)))
            {
                diagnostics.Add(ErrorCode.ERR_NoImplicitConv, rewrittenOperand.Syntax.Location, rewrittenOperand.Type, rewrittenType);
            }
            return result;
        }

        private static BoundExpression MakeConversionForIOperation(BoundExpression operand, TypeSymbol type, SyntaxNode syntax, CSharpCompilation compilation, BindingDiagnosticBag diagnostics, bool @checked, bool acceptFailingConversion = false)
        {
            Conversion conversion = MakeConversion(operand, type, compilation, diagnostics, acceptFailingConversion);
            if (conversion.IsIdentity)
            {
                return operand;
            }
            return new BoundConversion(syntax, operand, conversion, @checked, explicitCastInCode: false, null, null, type, !conversion.IsValid)
            {
                WasCompilerGenerated = true
            };
        }

        private BoundExpression MakeImplicitConversion(BoundExpression rewrittenOperand, TypeSymbol rewrittenType)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
            Conversion conversion = _compilation.Conversions.ClassifyConversionFromType(rewrittenOperand.Type, rewrittenType, ref useSiteInfo);
            _diagnostics.Add(rewrittenOperand.Syntax, useSiteInfo);
            if (!conversion.IsImplicit)
            {
                _diagnostics.Add(ErrorCode.ERR_NoImplicitConv, rewrittenOperand.Syntax.Location, rewrittenOperand.Type, rewrittenType);
                return _factory.NullOrDefault(rewrittenType);
            }
            return MakeConversionNode(rewrittenOperand.Syntax, rewrittenOperand, conversion, rewrittenType, @checked: false);
        }

        private BoundExpression MakeConversionNode(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, TypeSymbol rewrittenType, bool @checked, bool explicitCastInCode = false, ConstantValue? constantValueOpt = null)
        {
            if (conversion.Kind.IsUserDefinedConversion())
            {
                if (!TypeSymbol.Equals(rewrittenOperand.Type, conversion.BestUserDefinedConversionAnalysis!.FromType, TypeCompareKind.ConsiderEverything))
                {
                    rewrittenOperand = MakeConversionNode(syntax, rewrittenOperand, conversion.UserDefinedFromConversion, conversion.BestUserDefinedConversionAnalysis!.FromType, @checked);
                }
                if (!TypeSymbol.Equals(rewrittenOperand.Type, conversion.Method!.GetParameterType(0), TypeCompareKind.ConsiderEverything))
                {
                    rewrittenOperand = MakeConversionNode(rewrittenOperand, conversion.BestUserDefinedConversionAnalysis!.FromType, @checked);
                }
                TypeSymbol typeSymbol = conversion.Method!.ReturnType;
                if (rewrittenOperand.Type.IsNullableType() && conversion.Method!.GetParameterType(0).Equals(rewrittenOperand.Type.GetNullableUnderlyingType(), TypeCompareKind.AllIgnoreOptions) && !typeSymbol.IsNullableType() && typeSymbol.IsValueType)
                {
                    typeSymbol = ((NamedTypeSymbol)rewrittenOperand.Type!.OriginalDefinition).Construct(typeSymbol);
                }
                BoundExpression boundExpression = RewriteUserDefinedConversion(syntax, rewrittenOperand, conversion, typeSymbol);
                if (!TypeSymbol.Equals(boundExpression.Type, conversion.BestUserDefinedConversionAnalysis!.ToType, TypeCompareKind.ConsiderEverything))
                {
                    boundExpression = MakeConversionNode(boundExpression, conversion.BestUserDefinedConversionAnalysis!.ToType, @checked);
                }
                if (!TypeSymbol.Equals(boundExpression.Type, rewrittenType, TypeCompareKind.ConsiderEverything))
                {
                    boundExpression = MakeConversionNode(syntax, boundExpression, conversion.UserDefinedToConversion, rewrittenType, @checked);
                }
                return boundExpression;
            }
            return MakeConversionNode(null, syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, constantValueOpt, rewrittenType);
        }

        private BoundExpression RewriteTupleConversion(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, NamedTypeSymbol rewrittenType)
        {
            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = rewrittenType.TupleElementTypesWithAnnotations;
            int length = tupleElementTypesWithAnnotations.Length;
            ImmutableArray<FieldSymbol> tupleElements = ((NamedTypeSymbol)rewrittenOperand.Type).TupleElements;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
            BoundLocal boundLocal = _factory.StoreToTemp(rewrittenOperand, out BoundAssignmentOperator store);
            ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
            for (int i = 0; i < length; i++)
            {
                BoundExpression rewrittenOperand2 = MakeTupleFieldAccessAndReportUseSiteDiagnostics(boundLocal, syntax, tupleElements[i]);
                BoundExpression item = MakeConversionNode(syntax, rewrittenOperand2, underlyingConversions[i], tupleElementTypesWithAnnotations[i].Type, @checked, explicitCastInCode);
                instance.Add(item);
            }
            BoundExpression boundExpression = MakeTupleCreationExpression(syntax, rewrittenType, instance.ToImmutableAndFree());
            return _factory.MakeSequence(boundLocal.LocalSymbol, store, boundExpression);
        }

        private static bool NullableNeverHasValue(BoundExpression expression)
        {
            return expression.NullableNeverHasValue();
        }

        private static BoundExpression? NullableAlwaysHasValue(BoundExpression expression)
        {
            if (!expression.Type.IsNullableType())
            {
                return null;
            }
            if (expression is BoundObjectCreationExpression boundObjectCreationExpression)
            {
                ImmutableArray<BoundExpression> arguments = boundObjectCreationExpression.Arguments;
                if (arguments.Length == 1)
                {
                    ImmutableArray<BoundExpression> immutableArray = arguments;
                    return immutableArray[0];
                }
            }
            else if (expression is BoundConversion boundConversion)
            {
                Conversion conversion = boundConversion.Conversion;
                if (conversion.Kind == ConversionKind.ImplicitNullable)
                {
                    BoundExpression operand = boundConversion.Operand;
                    BoundExpression boundExpression = operand;
                    if (boundExpression.Type!.Equals(expression.Type.StrippedType(), TypeCompareKind.AllIgnoreOptions))
                    {
                        return boundExpression;
                    }
                    ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
                    ImmutableArray<Conversion> immutableArray2 = underlyingConversions;
                    BoundExpression boundExpression2 = operand;
                    BoundConversion boundConversion2 = boundConversion;
                    if (immutableArray2.Length == 1 && immutableArray2[0].Kind == ConversionKind.ImplicitTuple && !boundExpression2.Type.IsNullableType())
                    {
                        return new BoundConversion(expression.Syntax, boundExpression2, immutableArray2[0], boundConversion2.Checked, boundConversion2.ExplicitCastInCode, null, null, boundConversion2.Type.StrippedType(), boundConversion2.HasErrors);
                    }
                }
            }
            return null;
        }

        private BoundExpression RewriteNullableConversion(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, TypeSymbol rewrittenType)
        {
            if (_inExpressionLambda)
            {
                return RewriteLiftedConversionInExpressionTree(syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, rewrittenType);
            }
            TypeSymbol type = rewrittenOperand.Type;
            if (type.IsNullableType() && rewrittenType.IsNullableType())
            {
                return RewriteFullyLiftedBuiltInConversion(syntax, rewrittenOperand, conversion, @checked, rewrittenType);
            }
            if (rewrittenType.IsNullableType())
            {
                BoundExpression boundExpression = MakeConversionNode(syntax, rewrittenOperand, conversion.UnderlyingConversions[0], rewrittenType.GetNullableUnderlyingType(), @checked);
                MethodSymbol constructor = UnsafeGetNullableMethod(syntax, rewrittenType, SpecialMember.System_Nullable_T__ctor);
                return new BoundObjectCreationExpression(syntax, constructor, boundExpression);
            }
            BoundExpression boundExpression2 = NullableAlwaysHasValue(rewrittenOperand);
            if (boundExpression2 == null)
            {
                MethodSymbol method = UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T_get_Value);
                boundExpression2 = BoundCall.Synthesized(syntax, rewrittenOperand, method);
            }
            return MakeConversionNode(syntax, boundExpression2, conversion.UnderlyingConversions[0], rewrittenType, @checked);
        }

        private BoundExpression RewriteLiftedConversionInExpressionTree(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, TypeSymbol rewrittenType)
        {
            TypeSymbol type = rewrittenOperand.Type;
            ConversionGroup conversionGroupOpt = null;
            TypeSymbol typeSymbol = type.StrippedType();
            TypeSymbol typeSymbol2 = rewrittenType.StrippedType();
            if (!TypeSymbol.Equals(typeSymbol, typeSymbol2, TypeCompareKind.ConsiderEverything) && (typeSymbol.SpecialType == SpecialType.System_Decimal || typeSymbol2.SpecialType == SpecialType.System_Decimal))
            {
                TypeSymbol typeSymbol3 = typeSymbol;
                TypeSymbol typeTo = typeSymbol2;
                if (typeSymbol.IsEnumType())
                {
                    typeSymbol3 = typeSymbol.GetEnumUnderlyingType();
                    type = (type.IsNullableType() ? ((NamedTypeSymbol)type.OriginalDefinition).Construct(typeSymbol3) : typeSymbol3);
                    rewrittenOperand = BoundConversion.SynthesizedNonUserDefined(syntax, rewrittenOperand, Conversion.ImplicitEnumeration, type);
                }
                else if (typeSymbol2.IsEnumType())
                {
                    typeTo = typeSymbol2.GetEnumUnderlyingType();
                }
                MethodSymbol conversionMethod = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(DecimalConversionMethod(typeSymbol3, typeTo));
                ConversionKind kind = (conversion.Kind.IsImplicitConversion() ? ConversionKind.ImplicitUserDefined : ConversionKind.ExplicitUserDefined);
                return new BoundConversion(syntax, rewrittenOperand, new Conversion(kind, conversionMethod, isExtensionMethod: false), @checked, explicitCastInCode, conversionGroupOpt, null, rewrittenType);
            }
            return new BoundConversion(syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, conversionGroupOpt, null, rewrittenType);
        }

        private BoundExpression RewriteFullyLiftedBuiltInConversion(SyntaxNode syntax, BoundExpression operand, Conversion conversion, bool @checked, TypeSymbol type)
        {
            BoundExpression boundExpression = OptimizeLiftedBuiltInConversion(syntax, operand, conversion, @checked, type);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundLocal boundLocal = _factory.StoreToTemp(operand, out BoundAssignmentOperator store);
            if (!TryGetNullableMethod(syntax, boundLocal.Type, SpecialMember.System_Nullable_T_GetValueOrDefault, out var result))
            {
                return BadExpression(syntax, type, operand);
            }
            BoundExpression rewrittenCondition = MakeNullableHasValue(syntax, boundLocal);
            BoundExpression rewrittenConsequence = new BoundObjectCreationExpression(syntax, UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor), MakeConversionNode(syntax, BoundCall.Synthesized(syntax, boundLocal, result), conversion.UnderlyingConversions[0], type.GetNullableUnderlyingType(), @checked));
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, type);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, type);
        }

        private BoundExpression? OptimizeLiftedUserDefinedConversion(SyntaxNode syntax, BoundExpression operand, Conversion conversion, TypeSymbol type)
        {
            if (NullableNeverHasValue(operand))
            {
                return new BoundDefaultExpression(syntax, type);
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(operand);
            if (boundExpression != null)
            {
                return MakeLiftedUserDefinedConversionConsequence(BoundCall.Synthesized(syntax, null, conversion.Method, boundExpression), type);
            }
            return DistributeLiftedConversionIntoLiftedOperand(syntax, operand, conversion, @checked: false, type);
        }

        private BoundExpression? OptimizeLiftedBuiltInConversion(SyntaxNode syntax, BoundExpression operand, Conversion conversion, bool @checked, TypeSymbol type)
        {
            if (NullableNeverHasValue(operand))
            {
                return new BoundDefaultExpression(syntax, type);
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(operand);
            if (boundExpression != null)
            {
                return new BoundObjectCreationExpression(syntax, UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor), MakeConversionNode(syntax, boundExpression, conversion.UnderlyingConversions[0], type.GetNullableUnderlyingType(), @checked));
            }
            return DistributeLiftedConversionIntoLiftedOperand(syntax, operand, conversion, @checked, type);
        }

        private BoundExpression? DistributeLiftedConversionIntoLiftedOperand(SyntaxNode syntax, BoundExpression operand, Conversion conversion, bool @checked, TypeSymbol type)
        {
            if (operand.Kind == BoundKind.Sequence)
            {
                BoundSequence boundSequence = (BoundSequence)operand;
                if (boundSequence.Value.Kind == BoundKind.ConditionalOperator)
                {
                    BoundConditionalOperator boundConditionalOperator = (BoundConditionalOperator)boundSequence.Value;
                    if (NullableAlwaysHasValue(boundConditionalOperator.Consequence) != null && NullableNeverHasValue(boundConditionalOperator.Alternative))
                    {
                        return new BoundSequence(boundSequence.Syntax, boundSequence.Locals, boundSequence.SideEffects, RewriteConditionalOperator(boundConditionalOperator.Syntax, boundConditionalOperator.Condition, MakeConversionNode(null, syntax, boundConditionalOperator.Consequence, conversion, @checked, explicitCastInCode: false, null, type), MakeConversionNode(null, syntax, boundConditionalOperator.Alternative, conversion, @checked, explicitCastInCode: false, null, type), null, type, isRef: false), type);
                    }
                }
            }
            return null;
        }

        private BoundExpression RewriteUserDefinedConversion(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, TypeSymbol rewrittenType)
        {
            if (rewrittenOperand.Type.IsNullableType())
            {
                TypeSymbol parameterType = conversion.Method!.GetParameterType(0);
                if (parameterType.Equals(rewrittenOperand.Type.GetNullableUnderlyingType(), TypeCompareKind.AllIgnoreOptions) && !parameterType.IsNullableType() && parameterType.IsValueType)
                {
                    return RewriteLiftedUserDefinedConversion(syntax, rewrittenOperand, conversion, rewrittenType);
                }
            }
            if (_inExpressionLambda)
            {
                return BoundConversion.Synthesized(syntax, rewrittenOperand, conversion, @checked: false, explicitCastInCode: true, null, null, rewrittenType);
            }
            if (rewrittenOperand.Type.IsArray() && _compilation.IsReadOnlySpanType(rewrittenType))
            {
                return new BoundReadOnlySpanFromArray(syntax, rewrittenOperand, conversion.Method, rewrittenType)
                {
                    WasCompilerGenerated = true
                };
            }
            return BoundCall.Synthesized(syntax, null, conversion.Method, rewrittenOperand);
        }

        private BoundExpression MakeLiftedUserDefinedConversionConsequence(BoundCall call, TypeSymbol resultType)
        {
            if (call.Method.ReturnType.IsNonNullableValueType())
            {
                MethodSymbol constructor = UnsafeGetNullableMethod(call.Syntax, resultType, SpecialMember.System_Nullable_T__ctor);
                return new BoundObjectCreationExpression(call.Syntax, constructor, call);
            }
            return call;
        }

        private BoundExpression RewriteLiftedUserDefinedConversion(SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, TypeSymbol rewrittenType)
        {
            if (_inExpressionLambda)
            {
                Conversion conversion2 = TryMakeConversion(syntax, conversion, rewrittenOperand.Type, rewrittenType);
                return BoundConversion.Synthesized(syntax, rewrittenOperand, conversion2, @checked: false, explicitCastInCode: true, null, null, rewrittenType);
            }
            BoundExpression boundExpression = OptimizeLiftedUserDefinedConversion(syntax, rewrittenOperand, conversion, rewrittenType);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundLocal boundLocal = _factory.StoreToTemp(rewrittenOperand, out BoundAssignmentOperator store);
            MethodSymbol method = UnsafeGetNullableMethod(syntax, boundLocal.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            BoundExpression rewrittenCondition = MakeNullableHasValue(syntax, boundLocal);
            BoundCall arg = BoundCall.Synthesized(syntax, boundLocal, method);
            BoundCall call = BoundCall.Synthesized(syntax, null, conversion.Method, arg);
            BoundExpression rewrittenConsequence = MakeLiftedUserDefinedConversionConsequence(call, rewrittenType);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, rewrittenType);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, rewrittenType, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, rewrittenType);
        }

        private BoundExpression RewriteIntPtrConversion(BoundConversion? oldNode, SyntaxNode syntax, BoundExpression rewrittenOperand, Conversion conversion, bool @checked, bool explicitCastInCode, ConstantValue? constantValueOpt, TypeSymbol rewrittenType)
        {
            TypeSymbol type = rewrittenOperand.Type;
            SpecialMember intPtrConversionMethod = GetIntPtrConversionMethod(type, rewrittenType);
            if (!TryGetSpecialTypeMethod(syntax, intPtrConversionMethod, out var method))
            {
                return BadExpression(syntax, rewrittenType, rewrittenOperand);
            }
            conversion = conversion.SetConversionMethod(method);
            if (type.IsNullableType() && rewrittenType.IsNullableType())
            {
                return RewriteLiftedUserDefinedConversion(syntax, rewrittenOperand, conversion, rewrittenType);
            }
            if (type.IsNullableType())
            {
                rewrittenOperand = MakeConversionNode(rewrittenOperand, type.StrippedType(), @checked);
            }
            rewrittenOperand = MakeConversionNode(rewrittenOperand, method.GetParameterType(0), @checked);
            TypeSymbol returnType = method.ReturnType;
            if (_inExpressionLambda)
            {
                return BoundConversion.Synthesized(syntax, rewrittenOperand, conversion, @checked, explicitCastInCode, null, constantValueOpt, rewrittenType);
            }
            BoundExpression rewrittenOperand2 = MakeCall(syntax, null, method, ImmutableArray.Create(rewrittenOperand), returnType);
            return MakeConversionNode(rewrittenOperand2, rewrittenType, @checked);
        }

        public static SpecialMember GetIntPtrConversionMethod(TypeSymbol source, TypeSymbol target)
        {
            TypeSymbol typeSymbol = target.StrippedType();
            TypeSymbol typeSymbol2 = source.StrippedType();
            SpecialType specialType = (typeSymbol.IsEnumType() ? typeSymbol.GetEnumUnderlyingType()!.SpecialType : typeSymbol.SpecialType);
            SpecialType specialType2 = (typeSymbol2.IsEnumType() ? typeSymbol2.GetEnumUnderlyingType()!.SpecialType : typeSymbol2.SpecialType);
            switch (specialType)
            {
                case SpecialType.System_IntPtr:
                    if (source.IsPointerOrFunctionPointer())
                    {
                        return SpecialMember.System_IntPtr__op_Explicit_FromPointer;
                    }
                    switch (specialType2)
                    {
                        case SpecialType.System_Char:
                        case SpecialType.System_SByte:
                        case SpecialType.System_Byte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_Int32:
                            return SpecialMember.System_IntPtr__op_Explicit_FromInt32;
                        case SpecialType.System_UInt32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                        case SpecialType.System_Decimal:
                        case SpecialType.System_Single:
                        case SpecialType.System_Double:
                            return SpecialMember.System_IntPtr__op_Explicit_FromInt64;
                    }
                    break;
                case SpecialType.System_UIntPtr:
                    if (source.IsPointerOrFunctionPointer())
                    {
                        return SpecialMember.System_UIntPtr__op_Explicit_FromPointer;
                    }
                    switch (specialType2)
                    {
                        case SpecialType.System_Char:
                        case SpecialType.System_Byte:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_UInt32:
                            return SpecialMember.System_UIntPtr__op_Explicit_FromUInt32;
                        case SpecialType.System_SByte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_Int32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                        case SpecialType.System_Decimal:
                        case SpecialType.System_Single:
                        case SpecialType.System_Double:
                            return SpecialMember.System_UIntPtr__op_Explicit_FromUInt64;
                    }
                    break;
                default:
                    switch (specialType2)
                    {
                        case SpecialType.System_IntPtr:
                            if (target.IsPointerOrFunctionPointer())
                            {
                                return SpecialMember.System_IntPtr__op_Explicit_ToPointer;
                            }
                            switch (specialType)
                            {
                                case SpecialType.System_Char:
                                case SpecialType.System_SByte:
                                case SpecialType.System_Byte:
                                case SpecialType.System_Int16:
                                case SpecialType.System_UInt16:
                                case SpecialType.System_Int32:
                                case SpecialType.System_UInt32:
                                    return SpecialMember.System_IntPtr__op_Explicit_ToInt32;
                                case SpecialType.System_Int64:
                                case SpecialType.System_UInt64:
                                case SpecialType.System_Decimal:
                                case SpecialType.System_Single:
                                case SpecialType.System_Double:
                                    return SpecialMember.System_IntPtr__op_Explicit_ToInt64;
                            }
                            break;
                        case SpecialType.System_UIntPtr:
                            if (target.IsPointerOrFunctionPointer())
                            {
                                return SpecialMember.System_UIntPtr__op_Explicit_ToPointer;
                            }
                            switch (specialType)
                            {
                                case SpecialType.System_Char:
                                case SpecialType.System_SByte:
                                case SpecialType.System_Byte:
                                case SpecialType.System_Int16:
                                case SpecialType.System_UInt16:
                                case SpecialType.System_Int32:
                                case SpecialType.System_UInt32:
                                    return SpecialMember.System_UIntPtr__op_Explicit_ToUInt32;
                                case SpecialType.System_Int64:
                                case SpecialType.System_UInt64:
                                case SpecialType.System_Decimal:
                                case SpecialType.System_Single:
                                case SpecialType.System_Double:
                                    return SpecialMember.System_UIntPtr__op_Explicit_ToUInt64;
                            }
                            break;
                    }
                    break;
            }
            throw ExceptionUtilities.Unreachable;
        }

        private static SpecialMember DecimalConversionMethod(TypeSymbol typeFrom, TypeSymbol typeTo)
        {
            if (typeFrom.SpecialType == SpecialType.System_Decimal)
            {
                return typeTo.SpecialType switch
                {
                    SpecialType.System_Char => SpecialMember.System_Decimal__op_Explicit_ToChar,
                    SpecialType.System_SByte => SpecialMember.System_Decimal__op_Explicit_ToSByte,
                    SpecialType.System_Byte => SpecialMember.System_Decimal__op_Explicit_ToByte,
                    SpecialType.System_Int16 => SpecialMember.System_Decimal__op_Explicit_ToInt16,
                    SpecialType.System_UInt16 => SpecialMember.System_Decimal__op_Explicit_ToUInt16,
                    SpecialType.System_Int32 => SpecialMember.System_Decimal__op_Explicit_ToInt32,
                    SpecialType.System_UInt32 => SpecialMember.System_Decimal__op_Explicit_ToUInt32,
                    SpecialType.System_Int64 => SpecialMember.System_Decimal__op_Explicit_ToInt64,
                    SpecialType.System_UInt64 => SpecialMember.System_Decimal__op_Explicit_ToUInt64,
                    SpecialType.System_Single => SpecialMember.System_Decimal__op_Explicit_ToSingle,
                    SpecialType.System_Double => SpecialMember.System_Decimal__op_Explicit_ToDouble,
                    _ => throw ExceptionUtilities.UnexpectedValue(typeTo.SpecialType),
                };
            }
            return typeFrom.SpecialType switch
            {
                SpecialType.System_Char => SpecialMember.System_Decimal__op_Implicit_FromChar,
                SpecialType.System_SByte => SpecialMember.System_Decimal__op_Implicit_FromSByte,
                SpecialType.System_Byte => SpecialMember.System_Decimal__op_Implicit_FromByte,
                SpecialType.System_Int16 => SpecialMember.System_Decimal__op_Implicit_FromInt16,
                SpecialType.System_UInt16 => SpecialMember.System_Decimal__op_Implicit_FromUInt16,
                SpecialType.System_Int32 => SpecialMember.System_Decimal__op_Implicit_FromInt32,
                SpecialType.System_UInt32 => SpecialMember.System_Decimal__op_Implicit_FromUInt32,
                SpecialType.System_Int64 => SpecialMember.System_Decimal__op_Implicit_FromInt64,
                SpecialType.System_UInt64 => SpecialMember.System_Decimal__op_Implicit_FromUInt64,
                SpecialType.System_Single => SpecialMember.System_Decimal__op_Explicit_FromSingle,
                SpecialType.System_Double => SpecialMember.System_Decimal__op_Explicit_FromDouble,
                _ => throw ExceptionUtilities.UnexpectedValue(typeFrom.SpecialType),
            };
        }

        private BoundExpression RewriteDecimalConversion(SyntaxNode syntax, BoundExpression operand, TypeSymbol fromType, TypeSymbol toType, bool @checked, bool isImplicit, ConstantValue? constantValueOpt)
        {
            if (fromType.SpecialType == SpecialType.System_Decimal)
            {
                SpecialType specialType = toType.SpecialType;
                if ((uint)(specialType - 21) <= 1u)
                {
                    operand = RewriteDecimalConversionCore(syntax, operand, fromType, get64BitType(_compilation, toType.SpecialType == SpecialType.System_IntPtr), isImplicit, constantValueOpt);
                    return MakeConversionNode(operand, toType, @checked);
                }
            }
            else
            {
                SpecialType specialType = fromType.SpecialType;
                if ((uint)(specialType - 21) <= 1u)
                {
                    operand = MakeConversionNode(operand, get64BitType(_compilation, fromType.SpecialType == SpecialType.System_IntPtr), @checked);
                    return RewriteDecimalConversionCore(syntax, operand, operand.Type, toType, isImplicit, constantValueOpt);
                }
            }
            return RewriteDecimalConversionCore(syntax, operand, fromType, toType, isImplicit, constantValueOpt);
            static TypeSymbol get64BitType(CSharpCompilation compilation, bool signed)
            {
                return compilation.GetSpecialType(signed ? SpecialType.System_Int64 : SpecialType.System_UInt64);
            }
        }

        private BoundExpression RewriteDecimalConversionCore(SyntaxNode syntax, BoundExpression operand, TypeSymbol fromType, TypeSymbol toType, bool isImplicit, ConstantValue? constantValueOpt)
        {
            SpecialMember member = DecimalConversionMethod(fromType, toType);
            MethodSymbol methodSymbol = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(member);
            if (_inExpressionLambda)
            {
                ConversionKind kind = (isImplicit ? ConversionKind.ImplicitUserDefined : ConversionKind.ExplicitUserDefined);
                Conversion conversion = new Conversion(kind, methodSymbol, isExtensionMethod: false);
                return new BoundConversion(syntax, operand, conversion, @checked: false, explicitCastInCode: false, null, constantValueOpt, toType);
            }
            return BoundCall.Synthesized(syntax, null, methodSymbol, operand);
        }

        private Conversion TryMakeConversion(SyntaxNode syntax, Conversion conversion, TypeSymbol fromType, TypeSymbol toType)
        {
            switch (conversion.Kind)
            {
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    {
                        MethodSymbol method5 = conversion.Method;
                        Conversion conversion2 = TryMakeConversion(syntax, conversion.UserDefinedFromConversion, fromType, method5.Parameters[0].Type);
                        if (!conversion2.Exists)
                        {
                            return Conversion.NoConversion;
                        }
                        Conversion conversion3 = TryMakeConversion(syntax, conversion.UserDefinedToConversion, method5.ReturnType, toType);
                        if (!conversion3.Exists)
                        {
                            return Conversion.NoConversion;
                        }
                        if (conversion2 == conversion.UserDefinedFromConversion && conversion3 == conversion.UserDefinedToConversion)
                        {
                            return conversion;
                        }
                        return new Conversion(UserDefinedConversionResult.Valid(ImmutableArray.Create(UserDefinedConversionAnalysis.Normal(method5, conversion2, conversion3, fromType, toType)), 0), conversion.IsImplicit);
                    }
                case ConversionKind.IntPtr:
                    {
                        SpecialMember intPtrConversionMethod = GetIntPtrConversionMethod(fromType, toType);
                        if (!TryGetSpecialTypeMethod(syntax, intPtrConversionMethod, out var method4))
                        {
                            return Conversion.NoConversion;
                        }
                        return TryMakeUserDefinedConversion(syntax, method4, fromType, toType, conversion.IsImplicit);
                    }
                case ConversionKind.ImplicitNumeric:
                case ConversionKind.ExplicitNumeric:
                    if (fromType.SpecialType == SpecialType.System_Decimal || toType.SpecialType == SpecialType.System_Decimal)
                    {
                        SpecialMember specialMember3 = DecimalConversionMethod(fromType, toType);
                        if (!TryGetSpecialTypeMethod(syntax, specialMember3, out var method3))
                        {
                            return Conversion.NoConversion;
                        }
                        return TryMakeUserDefinedConversion(syntax, method3, fromType, toType, conversion.IsImplicit);
                    }
                    return conversion;
                case ConversionKind.ImplicitEnumeration:
                case ConversionKind.ExplicitEnumeration:
                    if (fromType.SpecialType == SpecialType.System_Decimal)
                    {
                        NamedTypeSymbol enumUnderlyingType = toType.GetEnumUnderlyingType();
                        SpecialMember specialMember = DecimalConversionMethod(fromType, enumUnderlyingType);
                        if (!TryGetSpecialTypeMethod(syntax, specialMember, out var method))
                        {
                            return Conversion.NoConversion;
                        }
                        return TryMakeUserDefinedConversion(syntax, method, fromType, toType, conversion.IsImplicit);
                    }
                    if (toType.SpecialType == SpecialType.System_Decimal)
                    {
                        SpecialMember specialMember2 = DecimalConversionMethod(fromType.GetEnumUnderlyingType(), toType);
                        if (!TryGetSpecialTypeMethod(syntax, specialMember2, out var method2))
                        {
                            return Conversion.NoConversion;
                        }
                        return TryMakeUserDefinedConversion(syntax, method2, fromType, toType, conversion.IsImplicit);
                    }
                    return conversion;
                default:
                    return conversion;
            }
        }

        private Conversion TryMakeConversion(SyntaxNode syntax, TypeSymbol fromType, TypeSymbol toType)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
            Conversion result = TryMakeConversion(syntax, _compilation.Conversions.ClassifyConversionFromType(fromType, toType, ref useSiteInfo), fromType, toType);
            _diagnostics.Add(syntax, useSiteInfo);
            return result;
        }

        private Conversion TryMakeUserDefinedConversion(SyntaxNode syntax, MethodSymbol meth, TypeSymbol fromType, TypeSymbol toType, bool isImplicit = true)
        {
            Conversion sourceConversion = TryMakeConversion(syntax, fromType, meth.Parameters[0].Type);
            if (!sourceConversion.Exists)
            {
                return Conversion.NoConversion;
            }
            Conversion targetConversion = TryMakeConversion(syntax, meth.ReturnType, toType);
            if (!targetConversion.Exists)
            {
                return Conversion.NoConversion;
            }
            return new Conversion(UserDefinedConversionResult.Valid(ImmutableArray.Create(UserDefinedConversionAnalysis.Normal(meth, sourceConversion, targetConversion, fromType, toType)), 0), isImplicit);
        }

        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            BoundConversion right = node.Right;
            return RewriteDeconstruction(node.Left, right.Conversion, right.Operand, node.IsUsed);
        }

        private BoundExpression? RewriteDeconstruction(BoundTupleExpression left, Conversion conversion, BoundExpression right, bool isUsed)
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<Binder.DeconstructionVariable> assignmentTargetsAndSideEffects = GetAssignmentTargetsAndSideEffects(left, instance, instance2);
            BoundExpression boundExpression = RewriteDeconstruction(assignmentTargetsAndSideEffects, conversion, left.Type, right, isUsed);
            Binder.DeconstructionVariable.FreeDeconstructionVariables(assignmentTargetsAndSideEffects);
            if (boundExpression == null)
            {
                instance.Free();
                instance2.Free();
                return null;
            }
            return _factory.Sequence(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), boundExpression);
        }

        private BoundExpression? RewriteDeconstruction(ArrayBuilder<Binder.DeconstructionVariable> lhsTargets, Conversion conversion, TypeSymbol leftType, BoundExpression right, bool isUsed)
        {
            if (right.Kind == BoundKind.ConditionalOperator)
            {
                BoundConditionalOperator boundConditionalOperator = (BoundConditionalOperator)right;
                return boundConditionalOperator.Update(boundConditionalOperator.IsRef, VisitExpression(boundConditionalOperator.Condition), RewriteDeconstruction(lhsTargets, conversion, leftType, boundConditionalOperator.Consequence, isUsed: true), RewriteDeconstruction(lhsTargets, conversion, leftType, boundConditionalOperator.Alternative, isUsed: true), boundConditionalOperator.ConstantValue, leftType, wasTargetTyped: true, leftType);
            }
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            DeconstructionSideEffects instance2 = DeconstructionSideEffects.GetInstance();
            BoundExpression boundExpression = ApplyDeconstructionConversion(lhsTargets, right, conversion, instance, instance2, isUsed, inInit: true);
            instance2.Consolidate();
            if (!isUsed)
            {
                BoundExpression boundExpression2 = instance2.PopLast();
                if (boundExpression2 == null)
                {
                    instance.Free();
                    instance2.Free();
                    return null;
                }
                return _factory.Sequence(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), boundExpression2);
            }
            if (!boundExpression.HasErrors)
            {
                boundExpression = VisitExpression(boundExpression);
            }
            return _factory.Sequence(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), boundExpression);
        }

        private BoundExpression? ApplyDeconstructionConversion(ArrayBuilder<Binder.DeconstructionVariable> leftTargets, BoundExpression right, Conversion conversion, ArrayBuilder<LocalSymbol> temps, DeconstructionSideEffects effects, bool isUsed, bool inInit)
        {
            ImmutableArray<BoundExpression> rightParts = GetRightParts(right, conversion, temps, effects, ref inInit);
            ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
            ArrayBuilder<BoundExpression> arrayBuilder = (isUsed ? ArrayBuilder<BoundExpression>.GetInstance(leftTargets.Count) : null);
            for (int i = 0; i < leftTargets.Count; i++)
            {
                ArrayBuilder<Binder.DeconstructionVariable> nestedVariables = leftTargets[i].NestedVariables;
                BoundExpression boundExpression;
                if (nestedVariables != null)
                {
                    boundExpression = ApplyDeconstructionConversion(nestedVariables, rightParts[i], underlyingConversions[i], temps, effects, isUsed, inInit);
                }
                else
                {
                    BoundExpression boundExpression2 = rightParts[i];
                    if (inInit)
                    {
                        boundExpression2 = EvaluateSideEffectingArgumentToTemp(boundExpression2, effects.init, temps);
                    }
                    BoundExpression single = leftTargets[i].Single;
                    boundExpression = EvaluateConversionToTemp(boundExpression2, underlyingConversions[i], single.Type, temps, effects.conversions);
                    if (single.Kind != BoundKind.DiscardExpression)
                    {
                        effects.assignments.Add(MakeAssignmentOperator(boundExpression.Syntax, single, boundExpression, single.Type, used: true, isChecked: false, isCompoundAssignment: false));
                    }
                }
                arrayBuilder?.Add(boundExpression);
            }
            if (isUsed)
            {
                NamedTypeSymbol type = NamedTypeSymbol.CreateTuple(null, arrayBuilder.SelectAsArray((BoundExpression e) => TypeWithAnnotations.Create(e.Type)), default(ImmutableArray<Location>), default(ImmutableArray<string>), _compilation, shouldCheckConstraints: false, includeNullability: false, default(ImmutableArray<bool>), (CSharpSyntaxNode)right.Syntax, _diagnostics);
                return new BoundConvertedTupleLiteral(right.Syntax, null, wasTargetTyped: false, arrayBuilder.ToImmutableAndFree(), default(ImmutableArray<string>), default(ImmutableArray<bool>), type);
            }
            return null;
        }

        private ImmutableArray<BoundExpression> GetRightParts(BoundExpression right, Conversion conversion, ArrayBuilder<LocalSymbol> temps, DeconstructionSideEffects effects, ref bool inInit)
        {
            DeconstructMethodInfo deconstructionInfo = conversion.DeconstructionInfo;
            if (!deconstructionInfo.IsDefault)
            {
                BoundExpression target = EvaluateSideEffectingArgumentToTemp(right, inInit ? effects.init : effects.deconstructions, temps);
                inInit = false;
                return InvokeDeconstructMethod(deconstructionInfo, target, effects.deconstructions, temps);
            }
            if (IsTupleExpression(right.Kind))
            {
                return ((BoundTupleExpression)right).Arguments;
            }
            if (right.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)right;
                if ((boundConversion.Conversion.Kind == ConversionKind.ImplicitTupleLiteral || boundConversion.Conversion.Kind == ConversionKind.Identity) && IsTupleExpression(boundConversion.Operand.Kind))
                {
                    return ((BoundTupleExpression)boundConversion.Operand).Arguments;
                }
            }
            if (right.Type!.IsTupleType)
            {
                inInit = false;
                return AccessTupleFields(VisitExpression(right), temps, effects.deconstructions);
            }
            throw ExceptionUtilities.Unreachable;
        }

        private static bool IsTupleExpression(BoundKind kind)
        {
            if (kind != BoundKind.TupleLiteral)
            {
                return kind == BoundKind.ConvertedTupleLiteral;
            }
            return true;
        }

        private ImmutableArray<BoundExpression> AccessTupleFields(BoundExpression expression, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects)
        {
            TypeSymbol? type = expression.Type;
            int length = type!.TupleElementTypesWithAnnotations.Length;
            BoundExpression tuple;
            if (CanChangeValueBetweenReads(expression))
            {
                BoundLocal boundLocal = _factory.StoreToTemp(expression, out BoundAssignmentOperator store);
                effects.Add(store);
                temps.Add(boundLocal.LocalSymbol);
                tuple = boundLocal;
            }
            else
            {
                tuple = expression;
            }
            ImmutableArray<FieldSymbol> tupleElements = type!.TupleElements;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
            for (int i = 0; i < length; i++)
            {
                BoundExpression item = MakeTupleFieldAccessAndReportUseSiteDiagnostics(tuple, expression.Syntax, tupleElements[i]);
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        private BoundExpression EvaluateConversionToTemp(BoundExpression expression, Conversion conversion, TypeSymbol destinationType, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects)
        {
            if (conversion.IsIdentity)
            {
                return expression;
            }
            BoundExpression arg = MakeConversionNode(expression.Syntax, expression, conversion, destinationType, @checked: false);
            return EvaluateSideEffectingArgumentToTemp(arg, effects, temps);
        }

        private ImmutableArray<BoundExpression> InvokeDeconstructMethod(DeconstructMethodInfo deconstruction, BoundExpression target, ArrayBuilder<BoundExpression> effects, ArrayBuilder<LocalSymbol> temps)
        {
            AddPlaceholderReplacement(deconstruction.InputPlaceholder, target);
            ImmutableArray<BoundDeconstructValuePlaceholder> outputPlaceholders = deconstruction.OutputPlaceholders;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(outputPlaceholders.Length);
            ImmutableArray<BoundDeconstructValuePlaceholder>.Enumerator enumerator = outputPlaceholders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundDeconstructValuePlaceholder current = enumerator.Current;
                SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_factory.CurrentFunction, TypeWithAnnotations.Create(current.Type), SynthesizedLocalKind.LoweringTemp);
                BoundLocal boundLocal = new BoundLocal(target.Syntax, synthesizedLocal, null, current.Type)
                {
                    WasCompilerGenerated = true
                };
                temps.Add(synthesizedLocal);
                AddPlaceholderReplacement(current, boundLocal);
                instance.Add(boundLocal);
            }
            effects.Add(VisitExpression(deconstruction.Invocation));
            RemovePlaceholderReplacement(deconstruction.InputPlaceholder);
            enumerator = outputPlaceholders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundDeconstructValuePlaceholder current2 = enumerator.Current;
                RemovePlaceholderReplacement(current2);
            }
            return instance.ToImmutableAndFree();
        }

        private BoundExpression EvaluateSideEffectingArgumentToTemp(BoundExpression arg, ArrayBuilder<BoundExpression> effects, ArrayBuilder<LocalSymbol> temps)
        {
            BoundExpression boundExpression = VisitExpression(arg);
            if (CanChangeValueBetweenReads(boundExpression, localsMayBeAssignedOrCaptured: true, structThisCanChangeValueBetweenReads: true))
            {
                BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store);
                temps.Add(boundLocal.LocalSymbol);
                effects.Add(store);
                return boundLocal;
            }
            return boundExpression;
        }

        private ArrayBuilder<Binder.DeconstructionVariable> GetAssignmentTargetsAndSideEffects(BoundTupleExpression variables, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects)
        {
            ArrayBuilder<Binder.DeconstructionVariable> instance = ArrayBuilder<Binder.DeconstructionVariable>.GetInstance(variables.Arguments.Length);
            ImmutableArray<BoundExpression>.Enumerator enumerator = variables.Arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                switch (current.Kind)
                {
                    case BoundKind.DiscardExpression:
                        instance.Add(new Binder.DeconstructionVariable(current, current.Syntax));
                        break;
                    case BoundKind.TupleLiteral:
                    case BoundKind.ConvertedTupleLiteral:
                        {
                            BoundTupleExpression boundTupleExpression = (BoundTupleExpression)current;
                            instance.Add(new Binder.DeconstructionVariable(GetAssignmentTargetsAndSideEffects(boundTupleExpression, temps, effects), boundTupleExpression.Syntax));
                            break;
                        }
                    default:
                        {
                            BoundExpression variable = TransformCompoundAssignmentLHS(current, effects, temps, current.Type.IsDynamic());
                            instance.Add(new Binder.DeconstructionVariable(variable, current.Syntax));
                            break;
                        }
                }
            }
            return instance;
        }

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument.HasDynamicType())
            {
                BoundExpression loweredOperand = VisitExpression(node.Argument);
                BoundExpression argument = _dynamicFactory.MakeDynamicConversion(loweredOperand, isExplicit: false, isArrayIndex: false, isChecked: false, node.Type).ToExpression();
                return new BoundDelegateCreationExpression(node.Syntax, argument, null, isExtensionMethod: false, node.Type);
            }
            if (node.Argument.Kind == BoundKind.MethodGroup)
            {
                BoundMethodGroup boundMethodGroup = (BoundMethodGroup)node.Argument;
                MethodSymbol methodOpt = node.MethodOpt;
                SyntaxNode syntax = _factory.Syntax;
                _factory.Syntax = (boundMethodGroup.ReceiverOpt ?? boundMethodGroup)!.Syntax;
                BoundExpression argument2 = ((!methodOpt.RequiresInstanceReceiver && !node.IsExtensionMethod) ? _factory.Type(methodOpt.ContainingType) : VisitExpression(boundMethodGroup.ReceiverOpt));
                _factory.Syntax = syntax;
                return node.Update(argument2, methodOpt, node.IsExtensionMethod, node.Type);
            }
            return base.VisitDelegateCreationExpression(node);
        }

        public override BoundNode VisitDoStatement(BoundDoStatement node)
        {
            BoundExpression boundExpression = VisitExpression(node.Condition);
            BoundStatement boundStatement = VisitStatement(node.Body);
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("start");
            SyntaxNode syntax = node.Syntax;
            if (!node.WasCompilerGenerated && Instrument)
            {
                boundExpression = _instrumenter.InstrumentDoStatementCondition(node, boundExpression, _factory);
            }
            BoundStatement boundStatement2 = new BoundConditionalGoto(syntax, boundExpression, jumpIfTrue: true, label);
            if (!node.WasCompilerGenerated && Instrument)
            {
                boundStatement2 = _instrumenter.InstrumentDoStatementConditionalGotoStart(node, boundStatement2);
            }
            if (node.Locals.IsEmpty)
            {
                return BoundStatementList.Synthesized(syntax, node.HasErrors, new BoundLabelStatement(syntax, label), boundStatement, new BoundLabelStatement(syntax, node.ContinueLabel), boundStatement2, new BoundLabelStatement(syntax, node.BreakLabel));
            }
            return BoundStatementList.Synthesized(syntax, node.HasErrors, new BoundLabelStatement(syntax, label), new BoundBlock(syntax, node.Locals, ImmutableArray.Create(boundStatement, new BoundLabelStatement(syntax, node.ContinueLabel), boundStatement2)), new BoundLabelStatement(syntax, node.BreakLabel));
        }

        public override BoundNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            BoundExpression boundExpression = VisitExpression(node.ReceiverOpt);
            BoundExpression boundExpression2 = VisitExpression(node.Argument);
            if (boundExpression != null && node.Event.ContainingAssembly.IsLinked && node.Event.ContainingType.IsInterfaceType())
            {
                NamedTypeSymbol containingType = node.Event.ContainingType;
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator = containingType.GetAttributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpAttributeData current = enumerator.Current;
                    if (current.IsTargetAttribute(containingType, AttributeDescription.ComEventInterfaceAttribute) && current.CommonConstructorArguments.Length == 2)
                    {
                        return RewriteNoPiaEventAssignmentOperator(node, boundExpression, boundExpression2);
                    }
                }
            }
            if (node.Event.IsWindowsRuntimeEvent)
            {
                EventAssignmentKind kind = (node.IsAddition ? EventAssignmentKind.Addition : EventAssignmentKind.Subtraction);
                return RewriteWindowsRuntimeEventAssignmentOperator(node.Syntax, node.Event, kind, node.IsDynamic, boundExpression, boundExpression2);
            }
            ImmutableArray<BoundExpression> rewrittenArguments = ImmutableArray.Create(boundExpression2);
            MethodSymbol method = (node.IsAddition ? node.Event.AddMethod : node.Event.RemoveMethod);
            return MakeCall(node.Syntax, boundExpression, method, rewrittenArguments, node.Type);
        }

        private BoundExpression RewriteWindowsRuntimeEventAssignmentOperator(SyntaxNode syntax, EventSymbol eventSymbol, EventAssignmentKind kind, bool isDynamic, BoundExpression? rewrittenReceiverOpt, BoundExpression rewrittenArgument)
        {
            BoundAssignmentOperator store = null;
            BoundLocal boundLocal = null;
            if (!eventSymbol.IsStatic && CanChangeValueBetweenReads(rewrittenReceiverOpt))
            {
                boundLocal = _factory.StoreToTemp(rewrittenReceiverOpt, out store);
            }
            NamedTypeSymbol namedTypeSymbol = _factory.WellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
            _factory.WellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal);
            NamedTypeSymbol type = _factory.WellKnownType(WellKnownType.System_Action_T).Construct(namedTypeSymbol);
            TypeSymbol type2 = eventSymbol.Type;
            BoundExpression argument = boundLocal ?? rewrittenReceiverOpt ?? _factory.Type(type2);
            BoundDelegateCreationExpression boundDelegateCreationExpression = new BoundDelegateCreationExpression(syntax, argument, eventSymbol.RemoveMethod, isExtensionMethod: false, type);
            BoundExpression boundExpression = null;
            if (kind == EventAssignmentKind.Assignment)
            {
                boundExpression = ((!TryGetWellKnownTypeMember<MethodSymbol>(syntax, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveAllEventHandlers, out var symbol)) ? new BoundBadExpression(syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)boundDelegateCreationExpression), ErrorTypeSymbol.UnknownResultType) : MakeCall(syntax, null, symbol, ImmutableArray.Create((BoundExpression)boundDelegateCreationExpression), symbol.ReturnType));
            }
            WellKnownMember member;
            ImmutableArray<BoundExpression> immutableArray;
            if (kind == EventAssignmentKind.Subtraction)
            {
                member = WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T;
                immutableArray = ImmutableArray.Create(boundDelegateCreationExpression, rewrittenArgument);
            }
            else
            {
                NamedTypeSymbol type3 = _factory.WellKnownType(WellKnownType.System_Func_T2).Construct(type2, namedTypeSymbol);
                BoundDelegateCreationExpression item = new BoundDelegateCreationExpression(syntax, argument, eventSymbol.AddMethod, isExtensionMethod: false, type3);
                member = WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T;
                immutableArray = ImmutableArray.Create(item, boundDelegateCreationExpression, rewrittenArgument);
            }
            BoundExpression boundExpression2;
            if (TryGetWellKnownTypeMember<MethodSymbol>(syntax, member, out var symbol2))
            {
                symbol2 = symbol2.Construct(type2);
                boundExpression2 = MakeCall(syntax, null, symbol2, immutableArray, symbol2.ReturnType);
            }
            else
            {
                boundExpression2 = new BoundBadExpression(syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, immutableArray, ErrorTypeSymbol.UnknownResultType);
            }
            if (boundLocal == null && boundExpression == null)
            {
                return boundExpression2;
            }
            ImmutableArray<LocalSymbol> locals = ((boundLocal == null) ? ImmutableArray<LocalSymbol>.Empty : ImmutableArray.Create(boundLocal.LocalSymbol));
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(2);
            if (boundExpression != null)
            {
                instance.Add(boundExpression);
            }
            if (store != null)
            {
                instance.Add(store);
            }
            return new BoundSequence(syntax, locals, instance.ToImmutableAndFree(), boundExpression2, boundExpression2.Type);
        }

        private BoundExpression VisitWindowsRuntimeEventFieldAssignmentOperator(SyntaxNode syntax, BoundEventAccess left, BoundExpression rewrittenRight)
        {
            EventSymbol eventSymbol = left.EventSymbol;
            BoundExpression rewrittenReceiverOpt = VisitExpression(left.ReceiverOpt);
            return RewriteWindowsRuntimeEventAssignmentOperator(syntax, eventSymbol, EventAssignmentKind.Assignment, isDynamic: false, rewrittenReceiverOpt, rewrittenRight);
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);
            return MakeEventAccess(node.Syntax, rewrittenReceiver, node.EventSymbol, node.ConstantValue, node.ResultKind, node.Type);
        }

        private BoundExpression MakeEventAccess(SyntaxNode syntax, BoundExpression? rewrittenReceiver, EventSymbol eventSymbol, ConstantValue? constantValueOpt, LookupResultKind resultKind, TypeSymbol type)
        {
            FieldSymbol associatedField = eventSymbol.AssociatedField;
            if (!eventSymbol.IsWindowsRuntimeEvent)
            {
                return MakeFieldAccess(syntax, rewrittenReceiver, associatedField, constantValueOpt, resultKind, type);
            }
            NamedTypeSymbol newOwner = (NamedTypeSymbol)associatedField.Type;
            BoundFieldAccess boundFieldAccess = new BoundFieldAccess(syntax, associatedField.IsStatic ? null : rewrittenReceiver, associatedField, null)
            {
                WasCompilerGenerated = true
            };
            BoundExpression boundExpression;
            if (TryGetWellKnownTypeMember<MethodSymbol>(syntax, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable, out var symbol))
            {
                symbol = symbol.AsMember(newOwner);
                boundExpression = BoundCall.Synthesized(syntax, null, symbol, boundFieldAccess);
            }
            else
            {
                boundExpression = new BoundBadExpression(syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)boundFieldAccess), ErrorTypeSymbol.UnknownResultType);
            }
            if (TryGetWellKnownTypeMember<PropertySymbol>(syntax, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList, out var symbol2))
            {
                MethodSymbol getMethod = symbol2.GetMethod;
                if ((object)getMethod != null)
                {
                    getMethod = getMethod.AsMember(newOwner);
                    return _factory.Call(boundExpression, getMethod);
                }
                string accessorName = SourcePropertyAccessorSymbol.GetAccessorName(symbol2.Name, getNotSet: true, symbol2.IsCompilationOutputWinMdObj());
                _diagnostics.Add(new CSDiagnosticInfo(ErrorCode.ERR_MissingPredefinedMember, symbol2.ContainingType, accessorName), syntax.Location);
            }
            return new BoundBadExpression(syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(boundExpression), ErrorTypeSymbol.UnknownResultType);
        }

        private BoundExpression RewriteNoPiaEventAssignmentOperator(BoundEventAssignmentOperator node, BoundExpression rewrittenReceiver, BoundExpression rewrittenArgument)
        {
            BoundExpression boundExpression = null;
            SyntaxNode syntax = _factory.Syntax;
            _factory.Syntax = node.Syntax;
            MethodSymbol methodSymbol = _factory.WellKnownMethod(WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__ctor);
            if ((object)methodSymbol != null)
            {
                MethodSymbol methodSymbol2 = _factory.WellKnownMethod(node.IsAddition ? WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler : WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler);
                if ((object)methodSymbol2 != null)
                {
                    BoundExpression receiver = _factory.New(methodSymbol, _factory.Typeof(node.Event.ContainingType), _factory.Literal(node.Event.MetadataName));
                    boundExpression = _factory.Call(receiver, methodSymbol2, _factory.Convert(methodSymbol2.Parameters[0].Type, rewrittenReceiver), _factory.Convert(methodSymbol2.Parameters[1].Type, rewrittenArgument));
                }
            }
            _factory.Syntax = syntax;
            EmitModule?.EmbeddedTypesManagerOpt.EmbedEventIfNeedTo(node.Event.GetCciAdapter(), node.Syntax, _diagnostics.DiagnosticBag, isUsedForComAwareEventBinding: true);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            return new BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray.Create((Symbol)node.Event), ImmutableArray.Create(rewrittenReceiver, rewrittenArgument), ErrorTypeSymbol.UnknownResultType);
        }

        public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
        {
            return RewriteExpressionStatement(node) ?? BoundStatementList.Synthesized(node.Syntax);
        }

        private BoundStatement? RewriteExpressionStatement(BoundExpressionStatement node, bool suppressInstrumentation = false)
        {
            BoundExpression boundExpression = VisitUnusedExpression(node.Expression);
            if (boundExpression == null)
            {
                return null;
            }
            BoundStatement boundStatement = node.Update(boundExpression);
            if (!suppressInstrumentation && Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentExpressionStatement(node, boundStatement);
            }
            return boundStatement;
        }

        private BoundExpression? VisitUnusedExpression(BoundExpression expression)
        {
            if (expression.HasErrors)
            {
                return expression;
            }
            switch (expression.Kind)
            {
                case BoundKind.AwaitExpression:
                    return VisitAwaitExpression((BoundAwaitExpression)expression, used: false);
                case BoundKind.AssignmentOperator:
                    return VisitAssignmentOperator((BoundAssignmentOperator)expression, used: false);
                case BoundKind.CompoundAssignmentOperator:
                    return VisitCompoundAssignmentOperator((BoundCompoundAssignmentOperator)expression, used: false);
                case BoundKind.Call:
                    if (_allowOmissionOfConditionalCalls)
                    {
                        BoundCall boundCall = (BoundCall)expression;
                        if (boundCall.Method.CallsAreOmitted(boundCall.SyntaxTree))
                        {
                            return null;
                        }
                    }
                    break;
                case BoundKind.DynamicInvocation:
                    return VisitDynamicInvocation((BoundDynamicInvocation)expression, resultDiscarded: true);
                case BoundKind.ConditionalAccess:
                    return RewriteConditionalAccess((BoundConditionalAccess)expression, used: false);
            }
            return VisitExpression(expression);
        }

        public override BoundNode VisitFieldAccess(BoundFieldAccess node)
        {
            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);
            return MakeFieldAccess(node.Syntax, rewrittenReceiver, node.FieldSymbol, node.ConstantValue, node.ResultKind, node.Type, node);
        }

        private BoundExpression MakeFieldAccess(SyntaxNode syntax, BoundExpression? rewrittenReceiver, FieldSymbol fieldSymbol, ConstantValue? constantValueOpt, LookupResultKind resultKind, TypeSymbol type, BoundFieldAccess? oldNodeOpt = null)
        {
            if (fieldSymbol.ContainingType.IsTupleType)
            {
                return MakeTupleFieldAccess(syntax, fieldSymbol, rewrittenReceiver);
            }
            BoundExpression boundExpression = ((oldNodeOpt != null) ? oldNodeOpt!.Update(rewrittenReceiver, fieldSymbol, constantValueOpt, resultKind, type) : new BoundFieldAccess(syntax, rewrittenReceiver, fieldSymbol, constantValueOpt, resultKind, type));
            if (fieldSymbol.IsFixedSizeBuffer)
            {
                boundExpression = new BoundAddressOfOperator(syntax, boundExpression, type);
            }
            return boundExpression;
        }

        private BoundExpression MakeTupleFieldAccess(SyntaxNode syntax, FieldSymbol tupleField, BoundExpression? rewrittenReceiver)
        {
            NamedTypeSymbol namedTypeSymbol = tupleField.ContainingType;
            FieldSymbol tupleUnderlyingField = tupleField.TupleUnderlyingField;
            if ((object)tupleUnderlyingField == null)
            {
                return _factory.BadExpression(tupleField.Type);
            }
            if (rewrittenReceiver != null && rewrittenReceiver!.Kind == BoundKind.DefaultExpression)
            {
                return new BoundDefaultExpression(syntax, tupleField.Type);
            }
            if (!TypeSymbol.Equals(tupleUnderlyingField.ContainingType, namedTypeSymbol, TypeCompareKind.ConsiderEverything))
            {
                WellKnownMember tupleTypeMember = NamedTypeSymbol.GetTupleTypeMember(8, 8);
                FieldSymbol fieldSymbol = (FieldSymbol)NamedTypeSymbol.GetWellKnownMemberInType(namedTypeSymbol.OriginalDefinition, tupleTypeMember, _diagnostics, syntax);
                if ((object)fieldSymbol == null)
                {
                    return _factory.BadExpression(tupleField.Type);
                }
                do
                {
                    FieldSymbol f = fieldSymbol.AsMember(namedTypeSymbol);
                    rewrittenReceiver = _factory.Field(rewrittenReceiver, f);
                    namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                }
                while (!TypeSymbol.Equals(tupleUnderlyingField.ContainingType, namedTypeSymbol, TypeCompareKind.ConsiderEverything));
            }
            return _factory.Field(rewrittenReceiver, tupleUnderlyingField);
        }

        private BoundExpression MakeTupleFieldAccessAndReportUseSiteDiagnostics(BoundExpression tuple, SyntaxNode syntax, FieldSymbol field)
        {
            field = field.CorrespondingTupleField ?? field;
            UseSiteInfo<AssemblySymbol> info = field.GetUseSiteInfo();
            DiagnosticInfo? diagnosticInfo = info.DiagnosticInfo;
            if (diagnosticInfo == null || diagnosticInfo!.Severity != DiagnosticSeverity.Error)
            {
                info = info.AdjustDiagnosticInfo(null);
            }
            _diagnostics.Add(info, syntax.Location);
            return MakeTupleFieldAccess(syntax, field, tuple);
        }

        public override BoundNode VisitFixedStatement(BoundFixedStatement node)
        {
            ImmutableArray<BoundLocalDeclaration> localDeclarations = node.Declarations.LocalDeclarations;
            int length = localDeclarations.Length;
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance(node.Locals.Length);
            instance.AddRange(node.Locals);
            ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance(length + 1 + 1);
            BoundStatement[] array = new BoundStatement[length];
            for (int i = 0; i < length; i++)
            {
                BoundLocalDeclaration localDecl = localDeclarations[i];
                instance2.Add(InitializeFixedStatementLocal(localDecl, _factory, out var pinnedTemp));
                instance.Add(pinnedTemp);
                if (pinnedTemp.RefKind == RefKind.None)
                {
                    array[i] = _factory.Assignment(_factory.Local(pinnedTemp), _factory.Null(pinnedTemp.Type));
                }
                else
                {
                    array[i] = _factory.Assignment(_factory.Local(pinnedTemp), new BoundPointerIndirectionOperator(_factory.Syntax, _factory.Default(new PointerTypeSymbol(pinnedTemp.TypeWithAnnotations)), pinnedTemp.Type), isRef: true);
                }
            }
            BoundStatement boundStatement = VisitStatement(node.Body);
            instance2.Add(boundStatement);
            instance2.Add(_factory.HiddenSequencePoint());
            if (IsInTryBlock(node) || HasGotoOut(boundStatement))
            {
                return _factory.Block(instance.ToImmutableAndFree(), new BoundTryStatement(_factory.Syntax, _factory.Block(instance2.ToImmutableAndFree()), ImmutableArray<BoundCatchBlock>.Empty, _factory.Block(array)));
            }
            instance2.AddRange(array);
            return _factory.Block(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree());
        }

        private static bool IsInTryBlock(BoundFixedStatement boundFixed)
        {
            SyntaxNode parent = boundFixed.Syntax.Parent;
            while (parent != null)
            {
                switch (parent.Kind())
                {
                    case SyntaxKind.TryStatement:
                        return true;
                    case SyntaxKind.UsingStatement:
                        return true;
                    case SyntaxKind.ForEachStatement:
                    case SyntaxKind.ForEachVariableStatement:
                        return true;
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return false;
                    case SyntaxKind.CatchClause:
                        if (((TryStatementSyntax)parent.Parent).Finally != null)
                        {
                            return true;
                        }
                        goto case SyntaxKind.FinallyClause;
                    case SyntaxKind.FinallyClause:
                        parent = parent.Parent;
                        parent = parent.Parent;
                        continue;
                }
                if (parent is MemberDeclarationSyntax)
                {
                    return false;
                }
                parent = parent.Parent;
            }
            return false;
        }

        private bool HasGotoOut(BoundNode node)
        {
            if (_lazyUnmatchedLabelCache == null)
            {
                _lazyUnmatchedLabelCache = new Dictionary<BoundNode, HashSet<LabelSymbol>>();
            }
            HashSet<LabelSymbol> hashSet = UnmatchedGotoFinder.Find(node, _lazyUnmatchedLabelCache, base.RecursionDepth);
            _lazyUnmatchedLabelCache!.Add(node, hashSet);
            if (hashSet != null)
            {
                return hashSet.Count > 0;
            }
            return false;
        }

        public override BoundNode VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private BoundStatement InitializeFixedStatementLocal(BoundLocalDeclaration localDecl, SyntheticBoundNodeFactory factory, out LocalSymbol pinnedTemp)
        {
            BoundExpression? initializerOpt = localDecl.InitializerOpt;
            LocalSymbol localSymbol = localDecl.LocalSymbol;
            BoundFixedLocalCollectionInitializer boundFixedLocalCollectionInitializer = (BoundFixedLocalCollectionInitializer)initializerOpt;
            if ((object)boundFixedLocalCollectionInitializer.GetPinnableOpt != null)
            {
                return InitializeFixedStatementGetPinnable(localDecl, localSymbol, boundFixedLocalCollectionInitializer, factory, out pinnedTemp);
            }
            TypeSymbol type = boundFixedLocalCollectionInitializer.Expression.Type;
            if ((object)type != null && type.SpecialType == SpecialType.System_String)
            {
                return InitializeFixedStatementStringLocal(localDecl, localSymbol, boundFixedLocalCollectionInitializer, factory, out pinnedTemp);
            }
            type = boundFixedLocalCollectionInitializer.Expression.Type;
            if ((object)type != null && type.TypeKind == TypeKind.Array)
            {
                return InitializeFixedStatementArrayLocal(localDecl, localSymbol, boundFixedLocalCollectionInitializer, factory, out pinnedTemp);
            }
            return InitializeFixedStatementRegularLocal(localDecl, localSymbol, boundFixedLocalCollectionInitializer, factory, out pinnedTemp);
        }

        private BoundStatement InitializeFixedStatementRegularLocal(BoundLocalDeclaration localDecl, LocalSymbol localSymbol, BoundFixedLocalCollectionInitializer fixedInitializer, SyntheticBoundNodeFactory factory, out LocalSymbol pinnedTemp)
        {
            TypeSymbol type = localSymbol.Type;
            BoundExpression boundExpression = VisitExpression(fixedInitializer.Expression);
            TypeSymbol pointedAtType = ((PointerTypeSymbol)boundExpression.Type).PointedAtType;
            boundExpression = ((BoundAddressOfOperator)boundExpression).Operand;
            VariableDeclaratorSyntax syntax = fixedInitializer.Syntax.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
            pinnedTemp = factory.SynthesizedLocal(pointedAtType, syntax, isPinned: true, RefKind.In, SynthesizedLocalKind.FixedReference);
            BoundStatement boundStatement = factory.Assignment(factory.Local(pinnedTemp), boundExpression, isRef: true);
            BoundAddressOfOperator arg = new BoundAddressOfOperator(factory.Syntax, factory.Local(pinnedTemp), fixedInitializer.ElementPointerType);
            BoundExpression right = factory.Convert(type, arg, fixedInitializer.ElementPointerTypeConversion);
            BoundStatement boundStatement2 = InstrumentLocalDeclarationIfNecessary(localDecl, localSymbol, factory.Assignment(factory.Local(localSymbol), right));
            return factory.Block(boundStatement, boundStatement2);
        }

        private BoundStatement InitializeFixedStatementGetPinnable(BoundLocalDeclaration localDecl, LocalSymbol localSymbol, BoundFixedLocalCollectionInitializer fixedInitializer, SyntheticBoundNodeFactory factory, out LocalSymbol pinnedTemp)
        {
            TypeSymbol type = localSymbol.Type;
            BoundExpression boundExpression = VisitExpression(fixedInitializer.Expression);
            TypeSymbol type2 = boundExpression.Type;
            SyntaxNode syntax = boundExpression.Syntax;
            MethodSymbol getPinnableOpt = fixedInitializer.GetPinnableOpt;
            VariableDeclaratorSyntax syntax2 = fixedInitializer.Syntax.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
            pinnedTemp = factory.SynthesizedLocal(getPinnableOpt.ReturnType, syntax2, isPinned: true, RefKind.In, SynthesizedLocalKind.FixedReference);
            int id = 0;
            bool num = !type2.IsValueType;
            BoundExpression boundExpression2;
            if (num)
            {
                id = ++_currentConditionalAccessID;
                boundExpression2 = new BoundConditionalReceiver(syntax, id, type2);
            }
            else
            {
                boundExpression2 = boundExpression;
            }
            BoundAssignmentOperator item = factory.AssignmentExpression(right: getPinnableOpt.IsStatic ? factory.Call(null, getPinnableOpt, boundExpression2) : factory.Call(boundExpression2, getPinnableOpt), left: factory.Local(pinnedTemp), isRef: true);
            BoundAddressOfOperator arg = new BoundAddressOfOperator(factory.Syntax, factory.Local(pinnedTemp), fixedInitializer.ElementPointerType);
            BoundExpression boundExpression3 = factory.Sequence(result: factory.Convert(type, arg, fixedInitializer.ElementPointerTypeConversion), locals: ImmutableArray<LocalSymbol>.Empty, sideEffects: ImmutableArray.Create((BoundExpression)item));
            if (num)
            {
                boundExpression3 = new BoundLoweredConditionalAccess(syntax, boundExpression, null, boundExpression3, null, id, type);
            }
            return InstrumentLocalDeclarationIfNecessary(localDecl, localSymbol, factory.Assignment(factory.Local(localSymbol), boundExpression3));
        }

        private BoundStatement InitializeFixedStatementStringLocal(BoundLocalDeclaration localDecl, LocalSymbol localSymbol, BoundFixedLocalCollectionInitializer fixedInitializer, SyntheticBoundNodeFactory factory, out LocalSymbol pinnedTemp)
        {
            TypeSymbol type = localSymbol.Type;
            BoundExpression boundExpression = VisitExpression(fixedInitializer.Expression);
            TypeSymbol type2 = boundExpression.Type;
            VariableDeclaratorSyntax syntax = fixedInitializer.Syntax.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
            pinnedTemp = factory.SynthesizedLocal(type2, syntax, isPinned: true, RefKind.None, SynthesizedLocalKind.FixedReference);
            BoundStatement boundStatement = factory.Assignment(factory.Local(pinnedTemp), boundExpression);
            BoundExpression arg = factory.Convert(fixedInitializer.ElementPointerType, factory.Local(pinnedTemp), Conversion.PinnedObjectToPointer);
            BoundExpression right = factory.Convert(type, arg, fixedInitializer.ElementPointerTypeConversion);
            BoundStatement boundStatement2 = InstrumentLocalDeclarationIfNecessary(localDecl, localSymbol, factory.Assignment(factory.Local(localSymbol), right));
            BoundExpression condition = MakeNullCheck(factory.Syntax, factory.Local(localSymbol), BinaryOperatorKind.NotEqual);
            BoundExpression right3 = factory.Binary(right: (!TryGetWellKnownTypeMember<MethodSymbol>(fixedInitializer.Syntax, WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__get_OffsetToStringData, out MethodSymbol symbol)) ? new BoundBadExpression(fixedInitializer.Syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType) : factory.Call(null, symbol), kind: BinaryOperatorKind.PointerAndIntAddition, type: type, left: factory.Local(localSymbol));
            BoundStatement boundStatement3 = factory.If(condition, factory.Assignment(factory.Local(localSymbol), right3));
            return factory.Block(boundStatement, boundStatement2, boundStatement3);
        }

        private BoundStatement InitializeFixedStatementArrayLocal(BoundLocalDeclaration localDecl, LocalSymbol localSymbol, BoundFixedLocalCollectionInitializer fixedInitializer, SyntheticBoundNodeFactory factory, out LocalSymbol pinnedTemp)
        {
            TypeSymbol type = localSymbol.Type;
            BoundExpression boundExpression = VisitExpression(fixedInitializer.Expression);
            TypeSymbol type2 = boundExpression.Type;
            pinnedTemp = factory.SynthesizedLocal(type2, null, isPinned: true);
            ArrayTypeSymbol obj = (ArrayTypeSymbol)pinnedTemp.Type;
            TypeWithAnnotations elementTypeWithAnnotations = obj.ElementTypeWithAnnotations;
            BoundExpression left = MakeNullCheck(rewrittenExpr: factory.AssignmentExpression(factory.Local(pinnedTemp), boundExpression), syntax: factory.Syntax, operatorKind: BinaryOperatorKind.NotEqual);
            BoundExpression right = factory.Binary(left: obj.IsSZArray ? factory.ArrayLength(factory.Local(pinnedTemp)) : ((!TryGetWellKnownTypeMember<MethodSymbol>(fixedInitializer.Syntax, WellKnownMember.System_Array__get_Length, out MethodSymbol symbol)) ? new BoundBadExpression(fixedInitializer.Syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)factory.Local(pinnedTemp)), ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)factory.Call(factory.Local(pinnedTemp), symbol))), kind: BinaryOperatorKind.IntNotEqual, type: factory.SpecialType(SpecialType.System_Boolean), right: factory.Literal(0));
            BoundExpression condition = factory.Binary(BinaryOperatorKind.LogicalBoolAnd, factory.SpecialType(SpecialType.System_Boolean), left, right);
            BoundExpression operand = factory.ArrayAccessFirstElement(factory.Local(pinnedTemp));
            BoundExpression arg = new BoundAddressOfOperator(factory.Syntax, operand, new PointerTypeSymbol(elementTypeWithAnnotations));
            BoundExpression right2 = factory.Convert(type, arg, fixedInitializer.ElementPointerTypeConversion);
            BoundExpression consequence = factory.AssignmentExpression(factory.Local(localSymbol), right2);
            BoundExpression alternative = factory.AssignmentExpression(factory.Local(localSymbol), factory.Null(type));
            BoundStatement rewrittenLocalDeclaration = factory.ExpressionStatement(new BoundConditionalOperator(factory.Syntax, isRef: false, condition, consequence, alternative, null, type, wasTargetTyped: false, type));
            return InstrumentLocalDeclarationIfNecessary(localDecl, localSymbol, rewrittenLocalDeclaration);
        }

        public override BoundNode VisitForEachStatement(BoundForEachStatement node)
        {
            if (node.HasErrors)
            {
                return node;
            }
            TypeSymbol type = GetUnconvertedCollectionExpression(node).Type;
            if (type.Kind == SymbolKind.ArrayType)
            {
                if (((ArrayTypeSymbol)type).IsSZArray)
                {
                    return RewriteSingleDimensionalArrayForEachStatement(node);
                }
                return RewriteMultiDimensionalArrayForEachStatement(node);
            }
            if (node.AwaitOpt == null && CanRewriteForEachAsFor(node.Syntax, type, out var indexerGet, out var lengthGet))
            {
                return RewriteForEachStatementAsFor(node, indexerGet, lengthGet);
            }
            return RewriteEnumeratorForEachStatement(node);
        }

        private bool CanRewriteForEachAsFor(SyntaxNode forEachSyntax, TypeSymbol nodeExpressionType, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out MethodSymbol? indexerGet, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out MethodSymbol? lengthGet)
        {
            lengthGet = (indexerGet = null);
            TypeSymbol originalDefinition = nodeExpressionType.OriginalDefinition;
            if (originalDefinition.SpecialType == SpecialType.System_String)
            {
                lengthGet = UnsafeGetSpecialTypeMethod(forEachSyntax, SpecialMember.System_String__Length);
                indexerGet = UnsafeGetSpecialTypeMethod(forEachSyntax, SpecialMember.System_String__Chars);
            }
            else if ((object)originalDefinition == _compilation.GetWellKnownType(WellKnownType.System_Span_T))
            {
                NamedTypeSymbol newOwner = (NamedTypeSymbol)nodeExpressionType;
                lengthGet = (MethodSymbol)(_factory.WellKnownMember(WellKnownMember.System_Span_T__get_Length, isOptional: true)?.SymbolAsMember(newOwner));
                indexerGet = (MethodSymbol)(_factory.WellKnownMember(WellKnownMember.System_Span_T__get_Item, isOptional: true)?.SymbolAsMember(newOwner));
            }
            else if ((object)originalDefinition == _compilation.GetWellKnownType(WellKnownType.System_ReadOnlySpan_T))
            {
                NamedTypeSymbol newOwner2 = (NamedTypeSymbol)nodeExpressionType;
                lengthGet = (MethodSymbol)(_factory.WellKnownMember(WellKnownMember.System_ReadOnlySpan_T__get_Length, isOptional: true)?.SymbolAsMember(newOwner2));
                indexerGet = (MethodSymbol)(_factory.WellKnownMember(WellKnownMember.System_ReadOnlySpan_T__get_Item, isOptional: true)?.SymbolAsMember(newOwner2));
            }
            if ((object)lengthGet != null)
            {
                return (object)indexerGet != null;
            }
            return false;
        }

        private BoundStatement RewriteEnumeratorForEachStatement(BoundForEachStatement node)
        {
            CommonForEachStatementSyntax commonForEachStatementSyntax = (CommonForEachStatementSyntax)node.Syntax;
            bool flag = node.AwaitOpt != null;
            ForEachEnumeratorInfo enumeratorInfoOpt = node.EnumeratorInfoOpt;
            BoundExpression unconvertedCollectionExpression = GetUnconvertedCollectionExpression(node);
            BoundExpression receiver = VisitExpression(unconvertedCollectionExpression);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            MethodArgumentInfo methodArgumentInfo = enumeratorInfoOpt.GetEnumeratorInfo;
            TypeSymbol returnType = methodArgumentInfo.Method.ReturnType;
            TypeSymbol elementType = enumeratorInfoOpt.ElementType;
            LocalSymbol localSymbol = _factory.SynthesizedLocal(returnType, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachEnumerator);
            BoundLocal boundLocal = MakeBoundLocal(commonForEachStatementSyntax, localSymbol, returnType);
            BoundExpression boundExpression = ConvertReceiverForInvocation(commonForEachStatementSyntax, receiver, methodArgumentInfo.Method, enumeratorInfoOpt.CollectionConversion, enumeratorInfoOpt.CollectionType);
            if (methodArgumentInfo.Method.IsExtensionMethod)
            {
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(methodArgumentInfo.Arguments.Length);
                instance.Add(boundExpression);
                instance.AddRange(methodArgumentInfo.Arguments, 1, methodArgumentInfo.Arguments.Length - 1);
                methodArgumentInfo = methodArgumentInfo with
                {
                    Arguments = instance.ToImmutableAndFree()
                };
                boundExpression = null;
            }
            BoundExpression rewrittenInitialValue = SynthesizeCall(methodArgumentInfo, commonForEachStatementSyntax, boundExpression, flag || methodArgumentInfo.Method.IsExtensionMethod, assertParametersAreOptional: false);
            BoundStatement collectionVarDecl = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol, rewrittenInitialValue);
            InstrumentForEachStatementCollectionVarDeclaration(node, ref collectionVarDecl);
            BoundExpression iterationVarValue = MakeConversionNode(commonForEachStatementSyntax, MakeConversionNode(commonForEachStatementSyntax, BoundCall.Synthesized(commonForEachStatementSyntax, boundLocal, enumeratorInfoOpt.CurrentPropertyGetter), enumeratorInfoOpt.CurrentConversion, elementType, node.Checked), node.ElementConversion, node.IterationVariableType.Type, node.Checked);
            ImmutableArray<LocalSymbol> iterationVariables = node.IterationVariables;
            BoundStatement iterationVarDecl = LocalOrDeconstructionDeclaration(node, iterationVariables, iterationVarValue);
            InstrumentForEachStatementIterationVarDeclaration(node, ref iterationVarDecl);
            BoundBlock rewrittenBody2 = CreateBlockDeclaringIterationVariables(iterationVariables, iterationVarDecl, rewrittenBody, commonForEachStatementSyntax);
            BoundExpression boundExpression2 = SynthesizeCall(enumeratorInfoOpt.MoveNextInfo, commonForEachStatementSyntax, boundLocal, flag);
            if (flag)
            {
                boundExpression2 = RewriteAwaitExpression(commonForEachStatementSyntax, boundExpression2, node.AwaitOpt, node.AwaitOpt!.GetResult!.ReturnType, used: true);
            }
            BoundStatement boundStatement = RewriteWhileStatement(node, boundExpression2, rewrittenBody2, node.BreakLabel, node.ContinueLabel, hasErrors: false);
            BoundStatement result;
            if (enumeratorInfoOpt.NeedsDisposal)
            {
                BoundStatement item = WrapWithTryFinallyDispose(commonForEachStatementSyntax, enumeratorInfoOpt, returnType, boundLocal, boundStatement);
                result = new BoundBlock(commonForEachStatementSyntax, ImmutableArray.Create(localSymbol), ImmutableArray.Create(collectionVarDecl, item));
            }
            else
            {
                result = new BoundBlock(commonForEachStatementSyntax, ImmutableArray.Create(localSymbol), ImmutableArray.Create(collectionVarDecl, boundStatement));
            }
            InstrumentForEachStatement(node, ref result);
            return result;
        }

        private bool TryGetDisposeMethod(CommonForEachStatementSyntax forEachSyntax, ForEachEnumeratorInfo enumeratorInfo, out MethodSymbol disposeMethod)
        {
            if (enumeratorInfo.IsAsync)
            {
                disposeMethod = (MethodSymbol)Binder.GetWellKnownTypeMember(_compilation, WellKnownMember.System_IAsyncDisposable__DisposeAsync, _diagnostics, null, forEachSyntax);
                return (object)disposeMethod != null;
            }
            return Binder.TryGetSpecialTypeMember<MethodSymbol>(_compilation, SpecialMember.System_IDisposable__Dispose, forEachSyntax, _diagnostics, out disposeMethod);
        }

        private BoundStatement WrapWithTryFinallyDispose(CommonForEachStatementSyntax forEachSyntax, ForEachEnumeratorInfo enumeratorInfo, TypeSymbol enumeratorType, BoundLocal boundEnumeratorVar, BoundStatement rewrittenBody)
        {
            NamedTypeSymbol namedTypeSymbol = null;
            bool flag = false;
            MethodSymbol disposeMethod = enumeratorInfo.PatternDisposeInfo?.Method;
            if ((object)disposeMethod == null)
            {
                TryGetDisposeMethod(forEachSyntax, enumeratorInfo, out disposeMethod);
                if ((object)disposeMethod == null)
                {
                    return rewrittenBody;
                }
                namedTypeSymbol = disposeMethod.ContainingType;
                TypeConversions typeConversions = new TypeConversions(_factory.CurrentFunction!.ContainingAssembly.CorLibrary);
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
                flag = typeConversions.ClassifyImplicitConversionFromType(enumeratorType, namedTypeSymbol, ref useSiteInfo).IsImplicit;
                _diagnostics.Add(forEachSyntax, useSiteInfo);
            }
            Binder.ReportDiagnosticsIfObsolete(_diagnostics, disposeMethod, forEachSyntax, hasBaseReceiver: false, _factory.CurrentFunction, _factory.CurrentType, enumeratorInfo.Location);
            BoundBlock finallyBlockOpt;
            if (flag || (object)enumeratorInfo.PatternDisposeInfo != null)
            {
                Conversion receiverConversion = (enumeratorType.IsStructType() ? Conversion.Boxing : Conversion.ImplicitReference);
                MethodArgumentInfo methodArgumentInfo = enumeratorInfo.PatternDisposeInfo;
                BoundExpression expression;
                if ((object)methodArgumentInfo == null)
                {
                    methodArgumentInfo = MethodArgumentInfo.CreateParameterlessMethod(disposeMethod);
                    expression = ConvertReceiverForInvocation(forEachSyntax, boundEnumeratorVar, disposeMethod, receiverConversion, namedTypeSymbol);
                }
                else
                {
                    expression = boundEnumeratorVar;
                }
                BoundExpression boundExpression = MakeCallWithNoExplicitArgument(methodArgumentInfo, forEachSyntax, expression);
                BoundAwaitableInfo disposeAwaitableInfo = enumeratorInfo.DisposeAwaitableInfo;
                BoundStatement boundStatement;
                if (disposeAwaitableInfo != null)
                {
                    boundStatement = WrapWithAwait(forEachSyntax, boundExpression, disposeAwaitableInfo);
                    _sawAwaitInExceptionHandler = true;
                }
                else
                {
                    boundStatement = new BoundExpressionStatement(forEachSyntax, boundExpression);
                }
                finallyBlockOpt = new BoundBlock(statements: ImmutableArray.Create((!enumeratorType.IsValueType) ? RewriteIfStatement(forEachSyntax, new BoundBinaryOperator(forEachSyntax, BinaryOperatorKind.NotEqual, null, null, LookupResultKind.Viable, MakeConversionNode(forEachSyntax, boundEnumeratorVar, enumeratorInfo.EnumeratorConversion, _compilation.GetSpecialType(SpecialType.System_Object), @checked: false), MakeLiteral(forEachSyntax, ConstantValue.Null, null), _compilation.GetSpecialType(SpecialType.System_Boolean)), boundStatement, null, hasErrors: false) : boundStatement), syntax: forEachSyntax, locals: ImmutableArray<LocalSymbol>.Empty);
            }
            else
            {
                LocalSymbol localSymbol = _factory.SynthesizedLocal(namedTypeSymbol);
                BoundLocal boundLocal = MakeBoundLocal(forEachSyntax, localSymbol, namedTypeSymbol);
                BoundTypeExpression targetType = new BoundTypeExpression(forEachSyntax, null, namedTypeSymbol);
                BoundExpression rewrittenInitialValue = new BoundAsOperator(forEachSyntax, boundEnumeratorVar, targetType, Conversion.ExplicitReference, namedTypeSymbol);
                BoundStatement item = MakeLocalDeclaration(forEachSyntax, localSymbol, rewrittenInitialValue);
                BoundExpression expression2 = BoundCall.Synthesized(forEachSyntax, boundLocal, disposeMethod);
                BoundStatement rewrittenConsequence = new BoundExpressionStatement(forEachSyntax, expression2);
                BoundStatement item2 = RewriteIfStatement(forEachSyntax, new BoundBinaryOperator(forEachSyntax, BinaryOperatorKind.NotEqual, null, null, LookupResultKind.Viable, boundLocal, MakeLiteral(forEachSyntax, ConstantValue.Null, null), _compilation.GetSpecialType(SpecialType.System_Boolean)), rewrittenConsequence, null, hasErrors: false);
                finallyBlockOpt = new BoundBlock(forEachSyntax, ImmutableArray.Create(localSymbol), ImmutableArray.Create(item, item2));
            }
            return new BoundTryStatement(forEachSyntax, new BoundBlock(forEachSyntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(rewrittenBody)), ImmutableArray<BoundCatchBlock>.Empty, finallyBlockOpt);
        }

        private BoundStatement WrapWithAwait(CommonForEachStatementSyntax forEachSyntax, BoundExpression disposeCall, BoundAwaitableInfo disposeAwaitableInfoOpt)
        {
            TypeSymbol type = disposeAwaitableInfoOpt.GetResult?.ReturnType ?? _compilation.DynamicType;
            BoundExpression expression = RewriteAwaitExpression(forEachSyntax, disposeCall, disposeAwaitableInfoOpt, type, used: false);
            return new BoundExpressionStatement(forEachSyntax, expression);
        }

        private BoundExpression ConvertReceiverForInvocation(CSharpSyntaxNode syntax, BoundExpression receiver, MethodSymbol method, Conversion receiverConversion, TypeSymbol convertedReceiverType)
        {
            if (receiver.Type!.IsReferenceType || !method.ContainingType.IsInterface)
            {
                receiver = MakeConversionNode(syntax, receiver, receiverConversion, convertedReceiverType, @checked: false);
            }
            return receiver;
        }

        private BoundExpression SynthesizeCall(MethodArgumentInfo methodArgumentInfo, CSharpSyntaxNode syntax, BoundExpression? receiver, bool allowExtensionAndOptionalParameters, bool assertParametersAreOptional = true)
        {
            if (allowExtensionAndOptionalParameters)
            {
                return MakeCallWithNoExplicitArgument(methodArgumentInfo, syntax, receiver, assertParametersAreOptional);
            }
            return BoundCall.Synthesized(syntax, receiver, methodArgumentInfo.Method, ImmutableArray<BoundExpression>.Empty);
        }

        private BoundStatement RewriteForEachStatementAsFor(BoundForEachStatement node, MethodSymbol indexerGet, MethodSymbol lengthGet)
        {
            CommonForEachStatementSyntax commonForEachStatementSyntax = (CommonForEachStatementSyntax)node.Syntax;
            BoundExpression unconvertedCollectionExpression = GetUnconvertedCollectionExpression(node);
            NamedTypeSymbol type = (NamedTypeSymbol)unconvertedCollectionExpression.Type;
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Int32);
            TypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression rewrittenInitialValue = VisitExpression(unconvertedCollectionExpression);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            LocalSymbol localSymbol = _factory.SynthesizedLocal(type, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArray);
            BoundStatement collectionVarDecl = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol, rewrittenInitialValue);
            InstrumentForEachStatementCollectionVarDeclaration(node, ref collectionVarDecl);
            BoundLocal receiverOpt = MakeBoundLocal(commonForEachStatementSyntax, localSymbol, type);
            LocalSymbol localSymbol2 = _factory.SynthesizedLocal(specialType, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArrayIndex);
            BoundLocal boundLocal = MakeBoundLocal(commonForEachStatementSyntax, localSymbol2, specialType);
            BoundStatement item = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol2, MakeLiteral(commonForEachStatementSyntax, ConstantValue.Default(SpecialType.System_Int32), specialType));
            BoundExpression iterationVarValue = MakeConversionNode(commonForEachStatementSyntax, BoundCall.Synthesized(commonForEachStatementSyntax, receiverOpt, indexerGet, boundLocal), node.ElementConversion, node.IterationVariableType.Type, node.Checked);
            ImmutableArray<LocalSymbol> iterationVariables = node.IterationVariables;
            BoundStatement iterationVarDecl = LocalOrDeconstructionDeclaration(node, iterationVariables, iterationVarValue);
            InstrumentForEachStatementIterationVarDeclaration(node, ref iterationVarDecl);
            BoundStatement rewrittenInitializer = new BoundStatementList(commonForEachStatementSyntax, ImmutableArray.Create(collectionVarDecl, item));
            BoundExpression right = BoundCall.Synthesized(commonForEachStatementSyntax, receiverOpt, lengthGet);
            BoundExpression rewrittenCondition = new BoundBinaryOperator(commonForEachStatementSyntax, BinaryOperatorKind.IntLessThan, null, null, LookupResultKind.Viable, boundLocal, right, specialType2);
            BoundStatement rewrittenIncrement = MakePositionIncrement(commonForEachStatementSyntax, boundLocal, specialType);
            BoundStatement rewrittenBody2 = CreateBlockDeclaringIterationVariables(iterationVariables, iterationVarDecl, rewrittenBody, commonForEachStatementSyntax);
            BoundStatement result = RewriteForStatementWithoutInnerLocals(node, ImmutableArray.Create(localSymbol, localSymbol2), rewrittenInitializer, rewrittenCondition, rewrittenIncrement, rewrittenBody2, node.BreakLabel, node.ContinueLabel, node.HasErrors);
            InstrumentForEachStatement(node, ref result);
            return result;
        }

        private BoundStatement LocalOrDeconstructionDeclaration(BoundForEachStatement forEachBound, ImmutableArray<LocalSymbol> iterationVariables, BoundExpression iterationVarValue)
        {
            CommonForEachStatementSyntax syntax = (CommonForEachStatementSyntax)forEachBound.Syntax;
            BoundForEachDeconstructStep deconstructionOpt = forEachBound.DeconstructionOpt;
            BoundStatement result;
            if (deconstructionOpt == null)
            {
                result = MakeLocalDeclaration(syntax, iterationVariables[0], iterationVarValue);
            }
            else
            {
                BoundDeconstructionAssignmentOperator deconstructionAssignment = deconstructionOpt.DeconstructionAssignment;
                AddPlaceholderReplacement(deconstructionOpt.TargetPlaceholder, iterationVarValue);
                BoundExpression expression = VisitExpression(deconstructionAssignment);
                result = new BoundExpressionStatement(deconstructionAssignment.Syntax, expression);
                RemovePlaceholderReplacement(deconstructionOpt.TargetPlaceholder);
            }
            return result;
        }

        private static BoundBlock CreateBlockDeclaringIterationVariables(ImmutableArray<LocalSymbol> iterationVariables, BoundStatement iteratorVariableInitialization, BoundStatement rewrittenBody, CommonForEachStatementSyntax forEachSyntax)
        {
            return new BoundBlock(forEachSyntax, iterationVariables, ImmutableArray.Create(iteratorVariableInitialization, rewrittenBody));
        }

        private static BoundBlock CreateBlockDeclaringIterationVariables(ImmutableArray<LocalSymbol> iterationVariables, BoundStatement iteratorVariableInitialization, BoundStatement checkAndBreak, BoundStatement rewrittenBody, LabelSymbol continueLabel, CommonForEachStatementSyntax forEachSyntax)
        {
            return new BoundBlock(forEachSyntax, iterationVariables, ImmutableArray.Create(iteratorVariableInitialization, checkAndBreak, rewrittenBody, new BoundLabelStatement(forEachSyntax, continueLabel)));
        }

        private BoundStatement RewriteSingleDimensionalArrayForEachStatement(BoundForEachStatement node)
        {
            CommonForEachStatementSyntax commonForEachStatementSyntax = (CommonForEachStatementSyntax)node.Syntax;
            BoundExpression unconvertedCollectionExpression = GetUnconvertedCollectionExpression(node);
            ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)unconvertedCollectionExpression.Type;
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Int32);
            TypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression rewrittenInitialValue = VisitExpression(unconvertedCollectionExpression);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            LocalSymbol localSymbol = _factory.SynthesizedLocal(arrayTypeSymbol, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArray);
            BoundStatement collectionVarDecl = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol, rewrittenInitialValue);
            InstrumentForEachStatementCollectionVarDeclaration(node, ref collectionVarDecl);
            BoundLocal expression = MakeBoundLocal(commonForEachStatementSyntax, localSymbol, arrayTypeSymbol);
            LocalSymbol localSymbol2 = _factory.SynthesizedLocal(specialType, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArrayIndex);
            BoundLocal boundLocal = MakeBoundLocal(commonForEachStatementSyntax, localSymbol2, specialType);
            BoundStatement item = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol2, MakeLiteral(commonForEachStatementSyntax, ConstantValue.Default(SpecialType.System_Int32), specialType));
            BoundExpression iterationVarValue = MakeConversionNode(commonForEachStatementSyntax, new BoundArrayAccess(commonForEachStatementSyntax, expression, ImmutableArray.Create((BoundExpression)boundLocal), arrayTypeSymbol.ElementType), node.ElementConversion, node.IterationVariableType.Type, node.Checked);
            ImmutableArray<LocalSymbol> iterationVariables = node.IterationVariables;
            BoundStatement iterationVarDecl = LocalOrDeconstructionDeclaration(node, iterationVariables, iterationVarValue);
            InstrumentForEachStatementIterationVarDeclaration(node, ref iterationVarDecl);
            BoundStatement rewrittenInitializer = new BoundStatementList(commonForEachStatementSyntax, ImmutableArray.Create(collectionVarDecl, item));
            BoundExpression right = new BoundArrayLength(commonForEachStatementSyntax, expression, specialType);
            BoundExpression rewrittenCondition = new BoundBinaryOperator(commonForEachStatementSyntax, BinaryOperatorKind.IntLessThan, null, null, LookupResultKind.Viable, boundLocal, right, specialType2);
            BoundStatement rewrittenIncrement = MakePositionIncrement(commonForEachStatementSyntax, boundLocal, specialType);
            BoundStatement rewrittenBody2 = CreateBlockDeclaringIterationVariables(iterationVariables, iterationVarDecl, rewrittenBody, commonForEachStatementSyntax);
            BoundStatement result = RewriteForStatementWithoutInnerLocals(node, ImmutableArray.Create(localSymbol, localSymbol2), rewrittenInitializer, rewrittenCondition, rewrittenIncrement, rewrittenBody2, node.BreakLabel, node.ContinueLabel, node.HasErrors);
            InstrumentForEachStatement(node, ref result);
            return result;
        }

        private BoundStatement RewriteMultiDimensionalArrayForEachStatement(BoundForEachStatement node)
        {
            CommonForEachStatementSyntax commonForEachStatementSyntax = (CommonForEachStatementSyntax)node.Syntax;
            BoundExpression unconvertedCollectionExpression = GetUnconvertedCollectionExpression(node);
            ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)unconvertedCollectionExpression.Type;
            int rank = arrayTypeSymbol.Rank;
            TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Int32);
            TypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Boolean);
            MethodSymbol method = UnsafeGetSpecialTypeMethod(commonForEachStatementSyntax, SpecialMember.System_Array__GetLowerBound);
            MethodSymbol method2 = UnsafeGetSpecialTypeMethod(commonForEachStatementSyntax, SpecialMember.System_Array__GetUpperBound);
            BoundExpression rewrittenInitialValue = VisitExpression(unconvertedCollectionExpression);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            LocalSymbol localSymbol = _factory.SynthesizedLocal(arrayTypeSymbol, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArray);
            BoundLocal boundLocal = MakeBoundLocal(commonForEachStatementSyntax, localSymbol, arrayTypeSymbol);
            BoundStatement collectionVarDecl = MakeLocalDeclaration(commonForEachStatementSyntax, localSymbol, rewrittenInitialValue);
            InstrumentForEachStatementCollectionVarDeclaration(node, ref collectionVarDecl);
            LocalSymbol[] array = new LocalSymbol[rank];
            BoundLocal[] array2 = new BoundLocal[rank];
            BoundStatement[] array3 = new BoundStatement[rank];
            for (int i = 0; i < rank; i++)
            {
                array[i] = _factory.SynthesizedLocal(specialType, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArrayLimit);
                array2[i] = MakeBoundLocal(commonForEachStatementSyntax, array[i], specialType);
                ImmutableArray<BoundExpression> arguments = ImmutableArray.Create(MakeLiteral(commonForEachStatementSyntax, ConstantValue.Create(i, ConstantValueTypeDiscriminator.Int32), specialType));
                BoundExpression rewrittenInitialValue2 = BoundCall.Synthesized(commonForEachStatementSyntax, boundLocal, method2, arguments);
                array3[i] = MakeLocalDeclaration(commonForEachStatementSyntax, array[i], rewrittenInitialValue2);
            }
            LocalSymbol[] array4 = new LocalSymbol[rank];
            BoundLocal[] array5 = new BoundLocal[rank];
            for (int j = 0; j < rank; j++)
            {
                array4[j] = _factory.SynthesizedLocal(specialType, commonForEachStatementSyntax, isPinned: false, RefKind.None, SynthesizedLocalKind.ForEachArrayIndex);
                array5[j] = MakeBoundLocal(commonForEachStatementSyntax, array4[j], specialType);
            }
            BoundExpression[] items = array5;
            BoundExpression iterationVarValue = MakeConversionNode(commonForEachStatementSyntax, new BoundArrayAccess(commonForEachStatementSyntax, boundLocal, ImmutableArray.Create(items), arrayTypeSymbol.ElementType), node.ElementConversion, node.IterationVariableType.Type, node.Checked);
            ImmutableArray<LocalSymbol> iterationVariables = node.IterationVariables;
            BoundStatement iterationVarDecl = LocalOrDeconstructionDeclaration(node, iterationVariables, iterationVarValue);
            InstrumentForEachStatementIterationVarDeclaration(node, ref iterationVarDecl);
            BoundStatement boundStatement = CreateBlockDeclaringIterationVariables(iterationVariables, iterationVarDecl, rewrittenBody, commonForEachStatementSyntax);
            BoundStatement boundStatement2 = null;
            for (int num = rank - 1; num >= 0; num--)
            {
                ImmutableArray<BoundExpression> arguments2 = ImmutableArray.Create(MakeLiteral(commonForEachStatementSyntax, ConstantValue.Create(num, ConstantValueTypeDiscriminator.Int32), specialType));
                BoundExpression rewrittenInitialValue3 = BoundCall.Synthesized(commonForEachStatementSyntax, boundLocal, method, arguments2);
                BoundStatement rewrittenInitializer = MakeLocalDeclaration(commonForEachStatementSyntax, array4[num], rewrittenInitialValue3);
                GeneratedLabelSymbol breakLabel = ((num == 0) ? node.BreakLabel : new GeneratedLabelSymbol("break"));
                BoundExpression rewrittenCondition = new BoundBinaryOperator(commonForEachStatementSyntax, BinaryOperatorKind.IntLessThanOrEqual, null, null, LookupResultKind.Viable, array5[num], array2[num], specialType2);
                BoundStatement rewrittenIncrement = MakePositionIncrement(commonForEachStatementSyntax, array5[num], specialType);
                BoundStatement rewrittenBody2;
                GeneratedLabelSymbol continueLabel;
                if (boundStatement2 == null)
                {
                    rewrittenBody2 = boundStatement;
                    continueLabel = node.ContinueLabel;
                }
                else
                {
                    rewrittenBody2 = boundStatement2;
                    continueLabel = new GeneratedLabelSymbol("continue");
                }
                boundStatement2 = RewriteForStatementWithoutInnerLocals(node, ImmutableArray.Create(array4[num]), rewrittenInitializer, rewrittenCondition, rewrittenIncrement, rewrittenBody2, breakLabel, continueLabel, node.HasErrors);
            }
            BoundStatement result = new BoundBlock(commonForEachStatementSyntax, ImmutableArray.Create(localSymbol).Concat(array.AsImmutableOrNull()), ImmutableArray.Create(collectionVarDecl).Concat(array3.AsImmutableOrNull()).Add(boundStatement2));
            InstrumentForEachStatement(node, ref result);
            return result;
        }

        private static BoundExpression GetUnconvertedCollectionExpression(BoundForEachStatement node)
        {
            BoundExpression expression = node.Expression;
            if (expression.Kind == BoundKind.Conversion)
            {
                return ((BoundConversion)expression).Operand;
            }
            return expression;
        }

        private static BoundLocal MakeBoundLocal(CSharpSyntaxNode syntax, LocalSymbol local, TypeSymbol type)
        {
            return new BoundLocal(syntax, local, null, type);
        }

        private BoundStatement MakeLocalDeclaration(CSharpSyntaxNode syntax, LocalSymbol local, BoundExpression rewrittenInitialValue)
        {
            return RewriteLocalDeclaration(null, syntax, local, rewrittenInitialValue);
        }

        private BoundStatement MakePositionIncrement(CSharpSyntaxNode syntax, BoundLocal boundPositionVar, TypeSymbol intType)
        {
            return BoundSequencePoint.CreateHidden(new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, boundPositionVar, new BoundBinaryOperator(syntax, BinaryOperatorKind.IntAddition, null, null, LookupResultKind.Viable, boundPositionVar, MakeLiteral(syntax, ConstantValue.Create(1), intType), intType), intType)));
        }

        private void InstrumentForEachStatementCollectionVarDeclaration(BoundForEachStatement original, [System.Diagnostics.CodeAnalysis.NotNullIfNotNull("collectionVarDecl")] ref BoundStatement? collectionVarDecl)
        {
            if (Instrument)
            {
                collectionVarDecl = _instrumenter.InstrumentForEachStatementCollectionVarDeclaration(original, collectionVarDecl);
            }
        }

        private void InstrumentForEachStatementIterationVarDeclaration(BoundForEachStatement original, ref BoundStatement iterationVarDecl)
        {
            if (Instrument)
            {
                if (((CommonForEachStatementSyntax)original.Syntax) is ForEachVariableStatementSyntax)
                {
                    iterationVarDecl = _instrumenter.InstrumentForEachStatementDeconstructionVariablesDeclaration(original, iterationVarDecl);
                }
                else
                {
                    iterationVarDecl = _instrumenter.InstrumentForEachStatementIterationVarDeclaration(original, iterationVarDecl);
                }
            }
        }

        private void InstrumentForEachStatement(BoundForEachStatement original, ref BoundStatement result)
        {
            if (Instrument)
            {
                result = _instrumenter.InstrumentForEachStatement(original, result);
            }
        }

        private BoundStatement MakeWhileTrueLoop(BoundForEachStatement loop, BoundBlock body)
        {
            SyntaxNode syntax = loop.Syntax;
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("still-true");
            BoundStatement boundStatement = new BoundLabelStatement(syntax, label);
            if (Instrument)
            {
                boundStatement = BoundSequencePoint.CreateHidden(boundStatement);
            }
            return BoundStatementList.Synthesized(syntax, false, boundStatement, body, new BoundGotoStatement(syntax, label));
        }

        public override BoundNode VisitForStatement(BoundForStatement node)
        {
            BoundStatement rewrittenInitializer = VisitStatement(node.Initializer);
            BoundExpression boundExpression = VisitExpression(node.Condition);
            BoundStatement rewrittenIncrement = VisitStatement(node.Increment);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            if (boundExpression != null && Instrument)
            {
                boundExpression = _instrumenter.InstrumentForStatementCondition(node, boundExpression, _factory);
            }
            return RewriteForStatement(node, rewrittenInitializer, boundExpression, rewrittenIncrement, rewrittenBody);
        }

        private BoundStatement RewriteForStatementWithoutInnerLocals(BoundLoopStatement original, ImmutableArray<LocalSymbol> outerLocals, BoundStatement? rewrittenInitializer, BoundExpression? rewrittenCondition, BoundStatement? rewrittenIncrement, BoundStatement rewrittenBody, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors)
        {
            SyntaxNode syntax = original.Syntax;
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            if (rewrittenInitializer != null)
            {
                instance.Add(rewrittenInitializer);
            }
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("start");
            GeneratedLabelSymbol label2 = new GeneratedLabelSymbol("end");
            BoundStatement boundStatement = new BoundGotoStatement(syntax, label2);
            if (Instrument)
            {
                boundStatement = BoundSequencePoint.CreateHidden(boundStatement);
            }
            instance.Add(boundStatement);
            instance.Add(new BoundLabelStatement(syntax, label));
            instance.Add(rewrittenBody);
            instance.Add(new BoundLabelStatement(syntax, continueLabel));
            if (rewrittenIncrement != null)
            {
                instance.Add(rewrittenIncrement);
            }
            instance.Add(new BoundLabelStatement(syntax, label2));
            BoundStatement boundStatement2 = null;
            boundStatement2 = ((rewrittenCondition == null) ? new BoundGotoStatement(syntax, label) : ((BoundStatement)new BoundConditionalGoto(rewrittenCondition!.Syntax, rewrittenCondition, jumpIfTrue: true, label)));
            if (Instrument)
            {
                boundStatement2 = original.Kind switch
                {
                    BoundKind.ForEachStatement => _instrumenter.InstrumentForEachStatementConditionalGotoStart((BoundForEachStatement)original, boundStatement2),
                    BoundKind.ForStatement => _instrumenter.InstrumentForStatementConditionalGotoStartOrBreak((BoundForStatement)original, boundStatement2),
                    _ => throw ExceptionUtilities.UnexpectedValue(original.Kind),
                };
            }
            instance.Add(boundStatement2);
            instance.Add(new BoundLabelStatement(syntax, breakLabel));
            ImmutableArray<BoundStatement> statements = instance.ToImmutableAndFree();
            return new BoundBlock(syntax, outerLocals, statements, hasErrors);
        }

        private BoundStatement RewriteForStatement(BoundForStatement node, BoundStatement? rewrittenInitializer, BoundExpression? rewrittenCondition, BoundStatement? rewrittenIncrement, BoundStatement rewrittenBody)
        {
            if (node.InnerLocals.IsEmpty)
            {
                return RewriteForStatementWithoutInnerLocals(node, node.OuterLocals, rewrittenInitializer, rewrittenCondition, rewrittenIncrement, rewrittenBody, node.BreakLabel, node.ContinueLabel, node.HasErrors);
            }
            SyntaxNode syntax = node.Syntax;
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            if (rewrittenInitializer != null)
            {
                instance.Add(rewrittenInitializer);
            }
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("start");
            BoundStatement boundStatement = new BoundLabelStatement(syntax, label);
            if (Instrument)
            {
                boundStatement = BoundSequencePoint.CreateHidden(boundStatement);
            }
            instance.Add(boundStatement);
            ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
            if (rewrittenCondition != null)
            {
                BoundStatement boundStatement2 = new BoundConditionalGoto(rewrittenCondition!.Syntax, rewrittenCondition, jumpIfTrue: false, node.BreakLabel);
                if (Instrument)
                {
                    boundStatement2 = _instrumenter.InstrumentForStatementConditionalGotoStartOrBreak(node, boundStatement2);
                }
                instance2.Add(boundStatement2);
            }
            instance2.Add(rewrittenBody);
            instance2.Add(new BoundLabelStatement(syntax, node.ContinueLabel));
            if (rewrittenIncrement != null)
            {
                instance2.Add(rewrittenIncrement);
            }
            instance2.Add(new BoundGotoStatement(syntax, label));
            instance.Add(new BoundBlock(syntax, node.InnerLocals, instance2.ToImmutableAndFree()));
            instance.Add(new BoundLabelStatement(syntax, node.BreakLabel));
            ImmutableArray<BoundStatement> statements = instance.ToImmutableAndFree();
            return new BoundBlock(syntax, node.OuterLocals, statements, node.HasErrors);
        }

        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            BoundExpression invokedExpression = VisitExpression(node.InvokedExpression);
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
            MethodSymbol signature = node.FunctionPointer.Signature;
            ImmutableArray<RefKind> argumentRefKindsOpt = node.ArgumentRefKindsOpt;
            rewrittenArguments = MakeArguments(node.Syntax, rewrittenArguments, signature, expanded: false, default(ImmutableArray<int>), ref argumentRefKindsOpt, out var temps, invokedAsExtensionMethod: false, ThreeState.False);
            node = node.Update(invokedExpression, rewrittenArguments, argumentRefKindsOpt, node.ResultKind, node.Type);
            if (temps.IsDefaultOrEmpty)
            {
                return node;
            }
            return new BoundSequence(node.Syntax, temps, ImmutableArray<BoundExpression>.Empty, node, node.Type);
        }

        public override BoundNode VisitGotoStatement(BoundGotoStatement node)
        {
            BoundExpression caseExpressionOpt = null;
            BoundLabel labelExpressionOpt = null;
            BoundStatement boundStatement = node.Update(node.Label, caseExpressionOpt, labelExpressionOpt);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentGotoStatement(node, boundStatement);
            }
            return boundStatement;
        }

        public override BoundNode? VisitLabel(BoundLabel node)
        {
            return null;
        }

        public override BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            SyntaxNode syntax = node.Syntax;
            FieldSymbol hostObjectField = _previousSubmissionFields.GetHostObjectField();
            BoundThisReference receiver = new BoundThisReference(syntax, _factory.CurrentType);
            return new BoundFieldAccess(syntax, receiver, hostObjectField, null);
        }

        public override BoundNode VisitIfStatement(BoundIfStatement node)
        {
            BoundExpression rewrittenCondition = VisitExpression(node.Condition);
            BoundStatement rewrittenConsequence = VisitStatement(node.Consequence);
            BoundStatement rewrittenAlternativeOpt = VisitStatement(node.AlternativeOpt);
            IfStatementSyntax syntax = (IfStatementSyntax)node.Syntax;
            if (Instrument && !node.WasCompilerGenerated)
            {
                rewrittenCondition = _instrumenter.InstrumentIfStatementCondition(node, rewrittenCondition, _factory);
            }
            BoundStatement boundStatement = RewriteIfStatement(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternativeOpt, node.HasErrors);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentIfStatement(node, boundStatement);
            }
            return boundStatement;
        }

        private static BoundStatement RewriteIfStatement(SyntaxNode syntax, BoundExpression rewrittenCondition, BoundStatement rewrittenConsequence, BoundStatement? rewrittenAlternativeOpt, bool hasErrors)
        {
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("afterif");
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            if (rewrittenAlternativeOpt == null)
            {
                instance.Add(new BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, jumpIfTrue: false, label));
                instance.Add(rewrittenConsequence);
                instance.Add(BoundSequencePoint.CreateHidden());
                instance.Add(new BoundLabelStatement(syntax, label));
                ImmutableArray<BoundStatement> statements = instance.ToImmutableAndFree();
                return new BoundStatementList(syntax, statements, hasErrors);
            }
            GeneratedLabelSymbol label2 = new GeneratedLabelSymbol("alternative");
            instance.Add(new BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, jumpIfTrue: false, label2));
            instance.Add(rewrittenConsequence);
            instance.Add(BoundSequencePoint.CreateHidden());
            instance.Add(new BoundGotoStatement(syntax, label));
            instance.Add(new BoundLabelStatement(syntax, label2));
            instance.Add(rewrittenAlternativeOpt);
            instance.Add(BoundSequencePoint.CreateHidden());
            instance.Add(new BoundLabelStatement(syntax, label));
            return new BoundStatementList(syntax, instance.ToImmutableAndFree(), hasErrors);
        }

        public override BoundNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BoundExpression boundExpression = MakeLiteral(node.Syntax, ConstantValue.Create(value: true), specialType);
            BoundExpression boundExpression2 = VisitExpression(node.Operand);
            if (NullableNeverHasValue(boundExpression2))
            {
                boundExpression2 = new BoundDefaultExpression(boundExpression2.Syntax, boundExpression2.Type.GetNullableUnderlyingType());
            }
            boundExpression2 = NullableAlwaysHasValue(boundExpression2) ?? boundExpression2;
            if (!node.Type.IsNullableType())
            {
                return new BoundObjectCreationExpression(node.Syntax, node.MethodOpt, boundExpression2, boundExpression);
            }
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
            boundExpression2 = CaptureExpressionInTempIfNeeded(boundExpression2, instance, instance2);
            BoundExpression rewrittenCondition = MakeOptimizedHasValue(boundExpression2.Syntax, boundExpression2);
            BoundExpression boundExpression3 = MakeOptimizedGetValueOrDefault(boundExpression2.Syntax, boundExpression2);
            BoundExpression boundExpression4 = new BoundObjectCreationExpression(node.Syntax, node.MethodOpt, boundExpression3, boundExpression);
            if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out var result))
            {
                return BadExpression(node.Syntax, node.Type, boundExpression2);
            }
            BoundExpression rewrittenConsequence = new BoundObjectCreationExpression(node.Syntax, result, boundExpression4);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(node.Syntax, node.Type);
            BoundExpression value = RewriteConditionalOperator(node.Syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, node.Type, isRef: false);
            return new BoundSequence(node.Syntax, instance2.ToImmutableAndFree(), instance.ToImmutableAndFree(), value, node.Type);
        }

        private BoundExpression MakeDynamicIndexerAccessReceiver(BoundDynamicIndexerAccess indexerAccess, BoundExpression loweredReceiver)
        {
            string text = indexerAccess.TryGetIndexedPropertyName();
            if (text != null)
            {
                return _dynamicFactory.MakeDynamicGetMember(loweredReceiver, text, resultIndexed: true).ToExpression();
            }
            return loweredReceiver;
        }

        public override BoundNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            BoundExpression loweredReceiver = VisitExpression(node.Receiver);
            ImmutableArray<BoundExpression> loweredArguments = VisitList(node.Arguments);
            return MakeDynamicGetIndex(node, loweredReceiver, loweredArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt);
        }

        private BoundExpression MakeDynamicGetIndex(BoundDynamicIndexerAccess node, BoundExpression loweredReceiver, ImmutableArray<BoundExpression> loweredArguments, ImmutableArray<string> argumentNames, ImmutableArray<RefKind> refKinds)
        {
            EmbedIfNeedTo(loweredReceiver, node.ApplicableIndexers, node.Syntax);
            return _dynamicFactory.MakeDynamicGetIndex(MakeDynamicIndexerAccessReceiver(node, loweredReceiver), loweredArguments, argumentNames, refKinds).ToExpression();
        }

        public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
        {
            return VisitIndexerAccess(node, isLeftOfAssignment: false);
        }

        private BoundExpression VisitIndexerAccess(BoundIndexerAccess node, bool isLeftOfAssignment)
        {
            PropertySymbol indexer = node.Indexer;
            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
            return MakeIndexerAccess(node.Syntax, rewrittenReceiver, indexer, rewrittenArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.Type, node, isLeftOfAssignment);
        }

        private BoundExpression MakeIndexerAccess(SyntaxNode syntax, BoundExpression rewrittenReceiver, PropertySymbol indexer, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, TypeSymbol type, BoundIndexerAccess? oldNodeOpt, bool isLeftOfAssignment)
        {
            if (isLeftOfAssignment && indexer.RefKind == RefKind.None)
            {
                if (oldNodeOpt == null)
                {
                    return new BoundIndexerAccess(syntax, rewrittenReceiver, indexer, rewrittenArguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, type);
                }
                return oldNodeOpt!.Update(rewrittenReceiver, indexer, rewrittenArguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, type);
            }
            MethodSymbol ownOrInheritedGetMethod = indexer.GetOwnOrInheritedGetMethod();
            rewrittenArguments = MakeArguments(syntax, rewrittenArguments, indexer, expanded, argsToParamsOpt, ref argumentRefKindsOpt, out var temps, invokedAsExtensionMethod: false, ThreeState.True);
            BoundExpression boundExpression = MakePropertyGetAccess(syntax, rewrittenReceiver, indexer, rewrittenArguments, ownOrInheritedGetMethod);
            if (temps.IsDefaultOrEmpty)
            {
                return boundExpression;
            }
            return new BoundSequence(syntax, temps, ImmutableArray<BoundExpression>.Empty, boundExpression, type);
        }

        public override BoundNode VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            return VisitIndexOrRangePatternIndexerAccess(node, isLeftOfAssignment: false);
        }

        private BoundSequence VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node, bool isLeftOfAssignment)
        {
            if (TypeSymbol.Equals(node.Argument.Type, _compilation.GetWellKnownType(WellKnownType.System_Index), TypeCompareKind.ConsiderEverything))
            {
                return VisitIndexPatternIndexerAccess(node.Syntax, node.Receiver, node.LengthOrCountProperty, (PropertySymbol)node.PatternSymbol, node.Argument, isLeftOfAssignment);
            }
            return VisitRangePatternIndexerAccess(node.Receiver, node.LengthOrCountProperty, (MethodSymbol)node.PatternSymbol, node.Argument);
        }

        private BoundSequence VisitIndexPatternIndexerAccess(SyntaxNode syntax, BoundExpression receiver, PropertySymbol lengthOrCountProperty, PropertySymbol intIndexer, BoundExpression argument, bool isLeftOfAssignment)
        {
            SyntheticBoundNodeFactory factory = _factory;
            BoundLocal boundLocal = factory.StoreToTemp(VisitExpression(receiver), out BoundAssignmentOperator store, (!receiver.Type!.IsReferenceType) ? RefKind.Ref : RefKind.None);
            BoundLocal boundLocal2 = factory.StoreToTemp(factory.Property(boundLocal, lengthOrCountProperty), out BoundAssignmentOperator store2);
            BoundLocal boundLocal3 = factory.StoreToTemp(MakePatternIndexOffsetExpression(argument, boundLocal2, out bool usedLength), out BoundAssignmentOperator store3);
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance(3);
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance(3);
            instance.Add(boundLocal.LocalSymbol);
            instance2.Add(store);
            if (usedLength)
            {
                instance.Add(boundLocal2.LocalSymbol);
                instance2.Add(store2);
            }
            instance.Add(boundLocal3.LocalSymbol);
            instance2.Add(store3);
            return (BoundSequence)factory.Sequence(instance.ToImmutable(), instance2.ToImmutable(), MakeIndexerAccess(syntax, boundLocal, intIndexer, ImmutableArray.Create((BoundExpression)boundLocal3), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), intIndexer.Type, null, isLeftOfAssignment));
        }

        private BoundExpression MakePatternIndexOffsetExpression(BoundExpression unloweredExpr, BoundExpression lengthAccess, out bool usedLength)
        {
            SyntheticBoundNodeFactory factory = _factory;
            if (unloweredExpr is BoundFromEndIndexExpression boundFromEndIndexExpression)
            {
                usedLength = true;
                return factory.IntSubtract(lengthAccess, VisitExpression(boundFromEndIndexExpression.Operand));
            }
            if (unloweredExpr is BoundConversion boundConversion)
            {
                BoundExpression operand = boundConversion.Operand;
                if (operand != null)
                {
                    TypeSymbol type = operand.Type;
                    if ((object)type != null && type.SpecialType == SpecialType.System_Int32)
                    {
                        usedLength = false;
                        return VisitExpression(operand);
                    }
                }
            }
            usedLength = true;
            return factory.Call(VisitExpression(unloweredExpr), WellKnownMember.System_Index__GetOffset, lengthAccess);
        }

        private BoundSequence VisitRangePatternIndexerAccess(BoundExpression receiver, PropertySymbol lengthOrCountProperty, MethodSymbol sliceMethod, BoundExpression rangeArg)
        {
            SyntheticBoundNodeFactory factory = _factory;
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            BoundLocal boundLocal = factory.StoreToTemp(VisitExpression(receiver), out BoundAssignmentOperator store);
            BoundLocal boundLocal2 = factory.StoreToTemp(factory.Property(boundLocal, lengthOrCountProperty), out BoundAssignmentOperator store2);
            instance.Add(boundLocal.LocalSymbol);
            instance2.Add(store);
            BoundExpression boundExpression;
            BoundExpression arg;
            if (rangeArg is BoundRangeExpression boundRangeExpression)
            {
                bool usedLength = false;
                if (boundRangeExpression.LeftOperandOpt != null)
                {
                    BoundLocal boundLocal3 = factory.StoreToTemp(MakePatternIndexOffsetExpression(boundRangeExpression.LeftOperandOpt, boundLocal2, out usedLength), out BoundAssignmentOperator store3);
                    instance.Add(boundLocal3.LocalSymbol);
                    instance2.Add(store3);
                    boundExpression = boundLocal3;
                }
                else
                {
                    boundExpression = factory.Literal(0);
                }
                BoundExpression rightOperandOpt = boundRangeExpression.RightOperandOpt;
                BoundExpression left;
                if (rightOperandOpt != null)
                {
                    left = MakePatternIndexOffsetExpression(rightOperandOpt, boundLocal2, out var usedLength2);
                    usedLength = usedLength || usedLength2;
                }
                else
                {
                    usedLength = true;
                    left = boundLocal2;
                }
                if (usedLength)
                {
                    instance.Insert(1, boundLocal2.LocalSymbol);
                    instance2.Insert(1, store2);
                }
                BoundLocal boundLocal4 = factory.StoreToTemp(factory.IntSubtract(left, boundExpression), out BoundAssignmentOperator store4);
                instance.Add(boundLocal4.LocalSymbol);
                instance2.Add(store4);
                arg = boundLocal4;
            }
            else
            {
                BoundLocal boundLocal5 = factory.StoreToTemp(VisitExpression(rangeArg), out BoundAssignmentOperator store5);
                instance.Add(boundLocal2.LocalSymbol);
                instance2.Add(store2);
                instance.Add(boundLocal5.LocalSymbol);
                instance2.Add(store5);
                BoundLocal boundLocal6 = factory.StoreToTemp(factory.Call(factory.Call(boundLocal5, factory.WellKnownMethod(WellKnownMember.System_Range__get_Start)), factory.WellKnownMethod(WellKnownMember.System_Index__GetOffset), boundLocal2), out BoundAssignmentOperator store6);
                instance.Add(boundLocal6.LocalSymbol);
                instance2.Add(store6);
                boundExpression = boundLocal6;
                BoundLocal boundLocal7 = factory.StoreToTemp(factory.IntSubtract(factory.Call(factory.Call(boundLocal5, factory.WellKnownMethod(WellKnownMember.System_Range__get_End)), factory.WellKnownMethod(WellKnownMember.System_Index__GetOffset), boundLocal2), boundExpression), out BoundAssignmentOperator store7);
                instance.Add(boundLocal7.LocalSymbol);
                instance2.Add(store7);
                arg = boundLocal7;
            }
            return (BoundSequence)factory.Sequence(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), factory.Call(boundLocal, sliceMethod, boundExpression, arg));
        }

        public override BoundNode VisitIsOperator(BoundIsOperator node)
        {
            BoundExpression rewrittenOperand = VisitExpression(node.Operand);
            BoundTypeExpression rewrittenTargetType = (BoundTypeExpression)VisitTypeExpression(node.TargetType);
            TypeSymbol rewrittenType = VisitType(node.Type);
            return MakeIsOperator(node, node.Syntax, rewrittenOperand, rewrittenTargetType, node.Conversion, rewrittenType);
        }

        private BoundExpression MakeIsOperator(BoundIsOperator oldNode, SyntaxNode syntax, BoundExpression rewrittenOperand, BoundTypeExpression rewrittenTargetType, Conversion conversion, TypeSymbol rewrittenType)
        {
            if (rewrittenOperand.Kind == BoundKind.MethodGroup)
            {
                BoundExpression receiverOpt = ((BoundMethodGroup)rewrittenOperand).ReceiverOpt;
                if (receiverOpt != null && receiverOpt.Kind != BoundKind.ThisReference)
                {
                    return RewriteConstantIsOperator(receiverOpt.Syntax, receiverOpt, ConstantValue.False, rewrittenType);
                }
                return MakeLiteral(syntax, ConstantValue.False, rewrittenType);
            }
            TypeSymbol type = rewrittenOperand.Type;
            TypeSymbol type2 = rewrittenTargetType.Type;
            if (!_inExpressionLambda)
            {
                ConstantValue isOperatorConstantResult = Binder.GetIsOperatorConstantResult(type, type2, conversion.Kind, rewrittenOperand.ConstantValue);
                if (isOperatorConstantResult != null)
                {
                    return RewriteConstantIsOperator(syntax, rewrittenOperand, isOperatorConstantResult, rewrittenType);
                }
                if (conversion.IsImplicit)
                {
                    return MakeNullCheck(syntax, rewrittenOperand, BinaryOperatorKind.NotEqual);
                }
            }
            return oldNode.Update(rewrittenOperand, rewrittenTargetType, conversion, rewrittenType);
        }

        private BoundExpression RewriteConstantIsOperator(SyntaxNode syntax, BoundExpression loweredOperand, ConstantValue constantValue, TypeSymbol type)
        {
            return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(loweredOperand), MakeLiteral(syntax, constantValue, type), type);
        }

        public override BoundNode VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            bool flag = node.IsNegated;
            BoundExpression boundExpression;
            if (canProduceLinearSequence(node.DecisionDag.RootNode, node.WhenTrueLabel, node.WhenFalseLabel))
            {
                IsPatternExpressionLinearLocalRewriter isPatternExpressionLinearLocalRewriter = new IsPatternExpressionLinearLocalRewriter(node, this);
                boundExpression = isPatternExpressionLinearLocalRewriter.LowerIsPatternAsLinearTestSequence(node, node.WhenTrueLabel, node.WhenFalseLabel);
                isPatternExpressionLinearLocalRewriter.Free();
            }
            else if (canProduceLinearSequence(node.DecisionDag.RootNode, node.WhenFalseLabel, node.WhenTrueLabel))
            {
                flag = !flag;
                IsPatternExpressionLinearLocalRewriter isPatternExpressionLinearLocalRewriter2 = new IsPatternExpressionLinearLocalRewriter(node, this);
                boundExpression = isPatternExpressionLinearLocalRewriter2.LowerIsPatternAsLinearTestSequence(node, node.WhenFalseLabel, node.WhenTrueLabel);
                isPatternExpressionLinearLocalRewriter2.Free();
            }
            else
            {
                IsPatternExpressionGeneralLocalRewriter isPatternExpressionGeneralLocalRewriter = new IsPatternExpressionGeneralLocalRewriter(node.Syntax, this);
                boundExpression = isPatternExpressionGeneralLocalRewriter.LowerGeneralIsPattern(node);
                isPatternExpressionGeneralLocalRewriter.Free();
            }
            if (flag)
            {
                boundExpression = _factory.Not(boundExpression);
            }
            return boundExpression;
            static bool canProduceLinearSequence(BoundDecisionDagNode node, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel)
            {
                while (true)
                {
                    if (!(node is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        if (node is BoundLeafDecisionDagNode boundLeafDecisionDagNode)
                        {
                            return boundLeafDecisionDagNode.Label == whenTrueLabel;
                        }
                        if (!(node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                        {
                            if (!(node is BoundTestDecisionDagNode boundTestDecisionDagNode))
                            {
                                break;
                            }
                            bool flag2 = IsFailureNode(boundTestDecisionDagNode.WhenFalse, whenFalseLabel);
                            if (flag2 == IsFailureNode(boundTestDecisionDagNode.WhenTrue, whenFalseLabel))
                            {
                                return false;
                            }
                            node = (flag2 ? boundTestDecisionDagNode.WhenTrue : boundTestDecisionDagNode.WhenFalse);
                        }
                        else
                        {
                            node = boundEvaluationDecisionDagNode.Next;
                        }
                    }
                    else
                    {
                        node = boundWhenDecisionDagNode.WhenTrue;
                    }
                }
                throw ExceptionUtilities.UnexpectedValue(node);
            }
        }

        private static bool IsFailureNode(BoundDecisionDagNode node, LabelSymbol whenFalseLabel)
        {
            if (node is BoundWhenDecisionDagNode boundWhenDecisionDagNode)
            {
                node = boundWhenDecisionDagNode.WhenTrue;
            }
            if (node is BoundLeafDecisionDagNode boundLeafDecisionDagNode)
            {
                return boundLeafDecisionDagNode.Label == whenFalseLabel;
            }
            return false;
        }

        public override BoundNode VisitLabeledStatement(BoundLabeledStatement node)
        {
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            return MakeLabeledStatement(node, rewrittenBody);
        }

        private BoundStatement MakeLabeledStatement(BoundLabeledStatement node, BoundStatement? rewrittenBody)
        {
            BoundStatement boundStatement = new BoundLabelStatement(node.Syntax, node.Label);
            if (Instrument && node.Syntax is LabeledStatementSyntax)
            {
                boundStatement = _instrumenter.InstrumentLabelStatement(node, boundStatement);
            }
            if (rewrittenBody == null)
            {
                return boundStatement;
            }
            return BoundStatementList.Synthesized(node.Syntax, boundStatement, rewrittenBody);
        }

        public override BoundNode VisitLiteral(BoundLiteral node)
        {
            return MakeLiteral(node.Syntax, node.ConstantValue, node.Type, node);
        }

        private BoundExpression MakeLiteral(SyntaxNode syntax, ConstantValue constantValue, TypeSymbol? type, BoundLiteral? oldNodeOpt = null)
        {
            if (constantValue.IsDecimal)
            {
                return MakeDecimalLiteral(syntax, constantValue);
            }
            if (constantValue.IsDateTime)
            {
                return MakeDateTimeLiteral(syntax, constantValue);
            }
            if (oldNodeOpt != null)
            {
                return oldNodeOpt!.Update(constantValue, type);
            }
            return new BoundLiteral(syntax, constantValue, type, constantValue.IsBad);
        }

        private BoundExpression MakeDecimalLiteral(SyntaxNode syntax, ConstantValue constantValue)
        {
            decimal decimalValue = constantValue.DecimalValue;
            decimalValue.GetBits(out var isNegative, out var scale, out var low, out var mid, out var high);
            ArrayBuilder<BoundExpression> arrayBuilder = new ArrayBuilder<BoundExpression>();
            SpecialMember member;
            if (scale == 0 && -2147483648m <= decimalValue && decimalValue <= 2147483647m)
            {
                MethodSymbol currentFunction = _factory.CurrentFunction;
                if ((currentFunction.MethodKind != MethodKind.StaticConstructor || currentFunction.ContainingType.SpecialType != SpecialType.System_Decimal) && !_inExpressionLambda)
                {
                    Symbol symbol = null;
                    if (decimalValue == 0m)
                    {
                        symbol = _compilation.GetSpecialTypeMember(SpecialMember.System_Decimal__Zero);
                    }
                    else if (decimalValue == 1m)
                    {
                        symbol = _compilation.GetSpecialTypeMember(SpecialMember.System_Decimal__One);
                    }
                    else if (decimalValue == -1m)
                    {
                        symbol = _compilation.GetSpecialTypeMember(SpecialMember.System_Decimal__MinusOne);
                    }
                    if ((object)symbol != null && !symbol.HasUseSiteError)
                    {
                        NamedTypeSymbol containingType = symbol.ContainingType;
                        if ((object)containingType != null && !containingType.HasUseSiteError)
                        {
                            FieldSymbol fieldSymbol = (FieldSymbol)symbol;
                            return new BoundFieldAccess(syntax, null, fieldSymbol, constantValue);
                        }
                    }
                }
                member = SpecialMember.System_Decimal__CtorInt32;
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create((int)decimalValue), _compilation.GetSpecialType(SpecialType.System_Int32)));
            }
            else if (scale == 0 && 0m <= decimalValue && decimalValue <= -1m)
            {
                member = SpecialMember.System_Decimal__CtorUInt32;
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create((uint)decimalValue), _compilation.GetSpecialType(SpecialType.System_UInt32)));
            }
            else if (scale == 0 && new decimal(long.MinValue) <= decimalValue && decimalValue <= new decimal(long.MaxValue))
            {
                member = SpecialMember.System_Decimal__CtorInt64;
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create((long)decimalValue), _compilation.GetSpecialType(SpecialType.System_Int64)));
            }
            else if (scale == 0 && 0m <= decimalValue && decimalValue <= new decimal(ulong.MaxValue))
            {
                member = SpecialMember.System_Decimal__CtorUInt64;
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create((ulong)decimalValue), _compilation.GetSpecialType(SpecialType.System_UInt64)));
            }
            else
            {
                member = SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte;
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(low), _compilation.GetSpecialType(SpecialType.System_Int32)));
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(mid), _compilation.GetSpecialType(SpecialType.System_Int32)));
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(high), _compilation.GetSpecialType(SpecialType.System_Int32)));
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(isNegative), _compilation.GetSpecialType(SpecialType.System_Boolean)));
                arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(scale), _compilation.GetSpecialType(SpecialType.System_Byte)));
            }
            MethodSymbol methodSymbol = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(member);
            return new BoundObjectCreationExpression(syntax, methodSymbol, arrayBuilder.ToImmutableAndFree(), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), constantValue, null, methodSymbol.ContainingType);
        }

        private BoundExpression MakeDateTimeLiteral(SyntaxNode syntax, ConstantValue constantValue)
        {
            ArrayBuilder<BoundExpression> arrayBuilder = new ArrayBuilder<BoundExpression>();
            arrayBuilder.Add(new BoundLiteral(syntax, ConstantValue.Create(constantValue.DateTimeValue.Ticks), _compilation.GetSpecialType(SpecialType.System_Int64)));
            MethodSymbol methodSymbol = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CtorInt64);
            return new BoundObjectCreationExpression(syntax, methodSymbol, arrayBuilder.ToImmutableAndFree(), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), null, null, methodSymbol.ContainingType);
        }

        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            return RewriteLocalDeclaration(node, node.Syntax, node.LocalSymbol, VisitExpression(node.InitializerOpt), node.HasErrors);
        }

        private BoundStatement? RewriteLocalDeclaration(BoundLocalDeclaration? originalOpt, SyntaxNode syntax, LocalSymbol localSymbol, BoundExpression? rewrittenInitializer, bool hasErrors = false)
        {
            if (rewrittenInitializer == null)
            {
                return null;
            }
            if (localSymbol.IsConst)
            {
                if (localSymbol.Type.IsReferenceType || localSymbol.ConstantValue != null)
                {
                    return null;
                }
                hasErrors = true;
            }
            if (syntax is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
            {
                syntax = localDeclarationStatementSyntax.Declaration.Variables[0];
            }
            BoundStatement rewrittenLocalDeclaration = new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, new BoundLocal(syntax, localSymbol, null, localSymbol.Type), rewrittenInitializer, localSymbol.Type, localSymbol.IsRef), hasErrors);
            return InstrumentLocalDeclarationIfNecessary(originalOpt, localSymbol, rewrittenLocalDeclaration);
        }

        private BoundStatement InstrumentLocalDeclarationIfNecessary(BoundLocalDeclaration? originalOpt, LocalSymbol localSymbol, BoundStatement rewrittenLocalDeclaration)
        {
            if (Instrument && originalOpt != null && !originalOpt!.WasCompilerGenerated && !localSymbol.IsConst && (originalOpt!.Syntax.Kind() == SyntaxKind.VariableDeclarator || (originalOpt!.Syntax.Kind() == SyntaxKind.LocalDeclarationStatement && ((LocalDeclarationStatementSyntax)originalOpt!.Syntax).Declaration.Variables.Count == 1)))
            {
                rewrittenLocalDeclaration = _instrumenter.InstrumentLocalInitialization(originalOpt, rewrittenLocalDeclaration);
            }
            return rewrittenLocalDeclaration;
        }

        public sealed override BoundNode VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitLockStatement(BoundLockStatement node)
        {
            LockStatementSyntax lockStatementSyntax = (LockStatementSyntax)node.Syntax;
            BoundExpression boundExpression = VisitExpression(node.Argument);
            BoundStatement boundStatement = VisitStatement(node.Body);
            TypeSymbol typeSymbol = boundExpression.Type;
            if ((object)typeSymbol == null)
            {
                typeSymbol = _compilation.GetSpecialType(SpecialType.System_Object);
                boundExpression = MakeLiteral(boundExpression.Syntax, boundExpression.ConstantValue, typeSymbol);
            }
            if (typeSymbol.Kind == SymbolKind.TypeParameter)
            {
                typeSymbol = _compilation.GetSpecialType(SpecialType.System_Object);
                boundExpression = MakeConversionNode(boundExpression.Syntax, boundExpression, Conversion.Boxing, typeSymbol, @checked: false, explicitCastInCode: false, boundExpression.ConstantValue);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store, RefKind.None, SynthesizedLocalKind.Lock, lockStatementSyntax);
            BoundStatement lockTargetCapture = new BoundExpressionStatement(lockStatementSyntax, store);
            BoundExpression expression = ((!TryGetWellKnownTypeMember<MethodSymbol>(lockStatementSyntax, WellKnownMember.System_Threading_Monitor__Exit, out MethodSymbol symbol)) ? new BoundBadExpression(lockStatementSyntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)boundLocal), ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)BoundCall.Synthesized(lockStatementSyntax, null, symbol, boundLocal)));
            BoundStatement boundStatement2 = new BoundExpressionStatement(lockStatementSyntax, expression);
            if ((TryGetWellKnownTypeMember<MethodSymbol>(lockStatementSyntax, WellKnownMember.System_Threading_Monitor__Enter2, out var symbol2, isOptional: true) || TryGetWellKnownTypeMember<MethodSymbol>(lockStatementSyntax, WellKnownMember.System_Threading_Monitor__Enter, out symbol2)) && symbol2.ParameterCount == 2)
            {
                TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                BoundLocal boundLocal2 = _factory.StoreToTemp(MakeLiteral(boundExpression.Syntax, ConstantValue.False, specialType), out BoundAssignmentOperator store2, RefKind.None, SynthesizedLocalKind.LockTaken, lockStatementSyntax);
                BoundStatement item = new BoundExpressionStatement(lockStatementSyntax, store2);
                BoundStatement item2 = new BoundExpressionStatement(lockStatementSyntax, BoundCall.Synthesized(lockStatementSyntax, null, symbol2, boundLocal, boundLocal2));
                boundStatement2 = RewriteIfStatement(lockStatementSyntax, boundLocal2, boundStatement2, null, node.HasErrors);
                return new BoundBlock(lockStatementSyntax, ImmutableArray.Create(boundLocal.LocalSymbol, boundLocal2.LocalSymbol), ImmutableArray.Create(InstrumentLockTargetCapture(node, lockTargetCapture), item, new BoundTryStatement(lockStatementSyntax, BoundBlock.SynthesizedNoLocals(lockStatementSyntax, ImmutableArray.Create(item2, boundStatement)), ImmutableArray<BoundCatchBlock>.Empty, BoundBlock.SynthesizedNoLocals(lockStatementSyntax, boundStatement2))));
            }
            BoundExpression expression2 = (((object)symbol2 == null) ? new BoundBadExpression(lockStatementSyntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)boundLocal), ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)BoundCall.Synthesized(lockStatementSyntax, null, symbol2, boundLocal)));
            BoundStatement item3 = new BoundExpressionStatement(lockStatementSyntax, expression2);
            return new BoundBlock(lockStatementSyntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create(InstrumentLockTargetCapture(node, lockTargetCapture), item3, new BoundTryStatement(lockStatementSyntax, BoundBlock.SynthesizedNoLocals(lockStatementSyntax, boundStatement), ImmutableArray<BoundCatchBlock>.Empty, BoundBlock.SynthesizedNoLocals(lockStatementSyntax, boundStatement2))));
        }

        private BoundStatement InstrumentLockTargetCapture(BoundLockStatement original, BoundStatement lockTargetCapture)
        {
            if (!Instrument)
            {
                return lockTargetCapture;
            }
            return _instrumenter.InstrumentLockTargetCapture(original, lockTargetCapture);
        }

        public override BoundNode? VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node)
        {
            return VisitMultipleLocalDeclarationsBase(node);
        }

        public override BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            return VisitMultipleLocalDeclarationsBase(node);
        }

        private BoundNode? VisitMultipleLocalDeclarationsBase(BoundMultipleLocalDeclarationsBase node)
        {
            ArrayBuilder<BoundStatement> arrayBuilder = null;
            ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator = node.LocalDeclarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundLocalDeclaration current = enumerator.Current;
                BoundNode boundNode = VisitLocalDeclaration(current);
                if (boundNode != null)
                {
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<BoundStatement>.GetInstance();
                    }
                    arrayBuilder.Add((BoundStatement)boundNode);
                }
            }
            if (arrayBuilder != null)
            {
                return BoundStatementList.Synthesized(node.Syntax, node.HasErrors, arrayBuilder.ToImmutableAndFree());
            }
            return null;
        }

        public override BoundNode VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            BoundNullCoalescingAssignmentOperator node2 = node;
            SyntaxNode syntax = node2.Syntax;
            ArrayBuilder<LocalSymbol> temps = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> stores = ArrayBuilder<BoundExpression>.GetInstance();
            BoundExpression transformedLHS = TransformCompoundAssignmentLHS(node2.LeftOperand, stores, temps, node2.LeftOperand.HasDynamicType());
            BoundExpression lhsRead = MakeRValue(transformedLHS);
            BoundExpression loweredRight = VisitExpression(node2.RightOperand);
            if (!node2.IsNullableValueTypeAssignment)
            {
                return rewriteNullCoalscingAssignmentStandard();
            }
            return rewriteNullCoalescingAssignmentForValueType();
            BoundExpression rewriteNullCoalescingAssignmentForValueType()
            {
                BoundExpression leftOperand = node2.LeftOperand;
                if (!TryGetNullableMethod(leftOperand.Syntax, leftOperand.Type, SpecialMember.System_Nullable_T_GetValueOrDefault, out var result))
                {
                    return BadExpression(node2);
                }
                if (!TryGetNullableMethod(leftOperand.Syntax, leftOperand.Type, SpecialMember.System_Nullable_T_get_HasValue, out var result2))
                {
                    return BadExpression(node2);
                }
                if (lhsRead.Kind == BoundKind.Call)
                {
                    BoundLocal boundLocal = _factory.StoreToTemp(lhsRead, out BoundAssignmentOperator store);
                    stores.Add(store);
                    temps.Add(boundLocal.LocalSymbol);
                    lhsRead = boundLocal;
                }
                BoundLocal boundLocal2 = _factory.StoreToTemp(BoundCall.Synthesized(leftOperand.Syntax, lhsRead, result), out BoundAssignmentOperator store2);
                stores.Add(store2);
                temps.Add(boundLocal2.LocalSymbol);
                BoundExpression item = MakeAssignmentOperator(node2.Syntax, boundLocal2, loweredRight, node2.Type, used: true, isChecked: false, isCompoundAssignment: false);
                BoundExpression item2 = MakeAssignmentOperator(node2.Syntax, transformedLHS, MakeConversionNode(boundLocal2, transformedLHS.Type, @checked: false), node2.LeftOperand.Type, used: true, isChecked: false, isCompoundAssignment: false);
                BoundCall condition = BoundCall.Synthesized(leftOperand.Syntax, lhsRead, result2);
                BoundExpression alternative = _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item, item2), boundLocal2);
                BoundExpression result3 = _factory.Conditional(condition, boundLocal2, alternative, boundLocal2.Type);
                return _factory.Sequence(temps.ToImmutableAndFree(), stores.ToImmutableAndFree(), result3);
            }
            BoundExpression rewriteNullCoalscingAssignmentStandard()
            {
                BoundExpression rewrittenRight = MakeAssignmentOperator(syntax, transformedLHS, loweredRight, node2.LeftOperand.Type, used: true, isChecked: false, isCompoundAssignment: false);
                BoundExpression boundExpression = MakeNullCoalescingOperator(syntax, lhsRead, rewrittenRight, Conversion.Identity, BoundNullCoalescingOperatorResultKind.LeftType, node2.LeftOperand.Type);
                if (temps.Count != 0 || stores.Count != 0)
                {
                    return new BoundSequence(syntax, temps.ToImmutableAndFree(), stores.ToImmutableAndFree(), boundExpression, boundExpression.Type);
                }
                return boundExpression;
            }
        }

        public override BoundNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundExpression rewrittenLeft = VisitExpression(node.LeftOperand);
            BoundExpression rewrittenRight = VisitExpression(node.RightOperand);
            TypeSymbol rewrittenResultType = VisitType(node.Type);
            return MakeNullCoalescingOperator(node.Syntax, rewrittenLeft, rewrittenRight, node.LeftConversion, node.OperatorResultKind, rewrittenResultType);
        }

        private BoundExpression MakeNullCoalescingOperator(SyntaxNode syntax, BoundExpression rewrittenLeft, BoundExpression rewrittenRight, Conversion leftConversion, BoundNullCoalescingOperatorResultKind resultKind, TypeSymbol? rewrittenResultType)
        {
            if (_inExpressionLambda)
            {
                TypeSymbol fromType = rewrittenLeft.Type.StrippedType();
                Conversion leftConversion2 = TryMakeConversion(syntax, leftConversion, fromType, rewrittenResultType);
                if (!leftConversion2.Exists)
                {
                    return BadExpression(syntax, rewrittenResultType, rewrittenLeft, rewrittenRight);
                }
                return new BoundNullCoalescingOperator(syntax, rewrittenLeft, rewrittenRight, leftConversion2, resultKind, rewrittenResultType);
            }
            TypeSymbol type = rewrittenLeft.Type;
            if ((object)type == null || type.IsReferenceType || type.IsValueType)
            {
                if (rewrittenLeft.IsDefaultValue())
                {
                    return rewrittenRight;
                }
                if (rewrittenLeft.ConstantValue != null)
                {
                    return GetConvertedLeftForNullCoalescingOperator(rewrittenLeft, leftConversion, rewrittenResultType);
                }
            }
            if (IsStringConcat(rewrittenLeft))
            {
                return GetConvertedLeftForNullCoalescingOperator(rewrittenLeft, leftConversion, rewrittenResultType);
            }
            if (rewrittenLeft.Type!.IsReferenceType && leftConversion.IsImplicit && !leftConversion.IsUserDefined)
            {
                if (!leftConversion.IsIdentity)
                {
                    rewrittenLeft = MakeConversionNode(rewrittenLeft.Syntax, rewrittenLeft, leftConversion, rewrittenResultType, @checked: false);
                }
                return new BoundNullCoalescingOperator(syntax, rewrittenLeft, rewrittenRight, Conversion.Identity, resultKind, rewrittenResultType);
            }
            if ((leftConversion.IsIdentity || leftConversion.Kind == ConversionKind.ExplicitNullable) && rewrittenLeft is BoundLoweredConditionalAccess boundLoweredConditionalAccess && (boundLoweredConditionalAccess.WhenNullOpt == null || NullableNeverHasValue(boundLoweredConditionalAccess.WhenNullOpt)))
            {
                BoundExpression boundExpression = NullableAlwaysHasValue(boundLoweredConditionalAccess.WhenNotNull);
                if (boundExpression != null)
                {
                    BoundExpression boundExpression2 = rewrittenRight;
                    if (boundExpression2.Type.IsNullableType())
                    {
                        boundExpression = boundLoweredConditionalAccess.WhenNotNull;
                    }
                    if (boundExpression2.IsDefaultValue() && boundExpression2.Type!.SpecialType != SpecialType.System_Decimal)
                    {
                        boundExpression2 = null;
                    }
                    return boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.Receiver, boundLoweredConditionalAccess.HasValueMethodOpt, boundExpression, boundExpression2, boundLoweredConditionalAccess.Id, rewrittenResultType);
                }
            }
            if (rewrittenLeft.Type.IsNullableType() && RemoveIdentityConversions(rewrittenRight).IsDefaultValue() && rewrittenRight.Type!.Equals(rewrittenLeft.Type.GetNullableUnderlyingType(), TypeCompareKind.AllIgnoreOptions) && TryGetNullableMethod(rewrittenLeft.Syntax, rewrittenLeft.Type, SpecialMember.System_Nullable_T_GetValueOrDefault, out var result))
            {
                return BoundCall.Synthesized(rewrittenLeft.Syntax, rewrittenLeft, result);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(rewrittenLeft, out BoundAssignmentOperator store);
            BoundExpression rewrittenCondition = MakeNullCheck(syntax, boundLocal, BinaryOperatorKind.NotEqual);
            BoundExpression convertedLeftForNullCoalescingOperator = GetConvertedLeftForNullCoalescingOperator(boundLocal, leftConversion, rewrittenResultType);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, convertedLeftForNullCoalescingOperator, rewrittenRight, null, rewrittenResultType, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, rewrittenResultType);
        }

        private bool IsStringConcat(BoundExpression expression)
        {
            if (expression.Kind != BoundKind.Call)
            {
                return false;
            }
            MethodSymbol method = ((BoundCall)expression).Method;
            if (method.IsStatic && method.ContainingType.SpecialType == SpecialType.System_String && ((object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObject) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectObject) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectObjectObject) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectArray)))
            {
                return true;
            }
            return false;
        }

        private static BoundExpression RemoveIdentityConversions(BoundExpression expression)
        {
            while (expression.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)expression;
                if (boundConversion.ConversionKind != ConversionKind.Identity)
                {
                    return expression;
                }
                expression = boundConversion.Operand;
            }
            return expression;
        }

        private BoundExpression GetConvertedLeftForNullCoalescingOperator(BoundExpression rewrittenLeft, Conversion leftConversion, TypeSymbol rewrittenResultType)
        {
            TypeSymbol type = rewrittenLeft.Type;
            if (!TypeSymbol.Equals(type, rewrittenResultType, TypeCompareKind.ConsiderEverything) && type.IsNullableType())
            {
                TypeSymbol nullableUnderlyingType = type.GetNullableUnderlyingType();
                rewrittenLeft = BoundCall.Synthesized(method: UnsafeGetNullableMethod(rewrittenLeft.Syntax, type, SpecialMember.System_Nullable_T_GetValueOrDefault), syntax: rewrittenLeft.Syntax, receiverOpt: rewrittenLeft);
                if (TypeSymbol.Equals(nullableUnderlyingType, rewrittenResultType, TypeCompareKind.ConsiderEverything))
                {
                    return rewrittenLeft;
                }
            }
            return MakeConversionNode(rewrittenLeft.Syntax, rewrittenLeft, leftConversion, rewrittenResultType, @checked: false);
        }

        public override BoundNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> loweredArguments = VisitList(node.Arguments);
            BoundExpression boundExpression = _dynamicFactory.MakeDynamicConstructorInvocation(node.Syntax, node.Type, loweredArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt).ToExpression();
            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt!.HasErrors)
            {
                return boundExpression;
            }
            return MakeExpressionWithInitializer(node.Syntax, boundExpression, node.InitializerExpressionOpt, node.Type);
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
            ImmutableArray<RefKind> argumentRefKindsOpt = node.ArgumentRefKindsOpt;
            rewrittenArguments = MakeArguments(node.Syntax, rewrittenArguments, node.Constructor, node.Expanded, node.ArgsToParamsOpt, ref argumentRefKindsOpt, out var temps);
            BoundExpression boundExpression;
            if (_inExpressionLambda)
            {
                if (!temps.IsDefaultOrEmpty)
                {
                    throw ExceptionUtilities.UnexpectedValue(temps.Length);
                }
                boundExpression = node.UpdateArgumentsAndInitializer(rewrittenArguments, argumentRefKindsOpt, MakeObjectCreationInitializerForExpressionTree(node.InitializerExpressionOpt), node.Constructor.ContainingType);
                if (node.Type.IsInterfaceType())
                {
                    boundExpression = MakeConversionNode(boundExpression, node.Type, @checked: false);
                }
                return boundExpression;
            }
            boundExpression = node.UpdateArgumentsAndInitializer(rewrittenArguments, argumentRefKindsOpt, null, node.Constructor.ContainingType);
            if (node.Constructor.IsDefaultValueTypeConstructor())
            {
                boundExpression = new BoundDefaultExpression(boundExpression.Syntax, boundExpression.Type);
            }
            if (!temps.IsDefaultOrEmpty)
            {
                boundExpression = new BoundSequence(node.Syntax, temps, ImmutableArray<BoundExpression>.Empty, boundExpression, node.Type);
            }
            if (node.Type.IsInterfaceType())
            {
                boundExpression = MakeConversionNode(boundExpression, node.Type, @checked: false);
            }
            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt!.HasErrors)
            {
                return boundExpression;
            }
            return MakeExpressionWithInitializer(node.Syntax, boundExpression, node.InitializerExpressionOpt, node.Type);
        }

        public override BoundNode VisitWithExpression(BoundWithExpression withExpr)
        {
            return MakeExpressionWithInitializer(rewrittenExpression: (!withExpr.Type.IsValueType) ? _factory.Convert(withExpr.Type, _factory.Call(VisitExpression(withExpr.Receiver), withExpr.CloneMethod)) : VisitExpression(withExpr.Receiver), syntax: withExpr.Syntax, initializerExpression: withExpr.InitializerExpression, type: withExpr.Type);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("initializerExpressionOpt")]
        private BoundObjectInitializerExpressionBase? MakeObjectCreationInitializerForExpressionTree(BoundObjectInitializerExpressionBase? initializerExpressionOpt)
        {
            if (initializerExpressionOpt != null && !initializerExpressionOpt!.HasErrors)
            {
                ImmutableArray<BoundExpression> newInitializers = MakeObjectOrCollectionInitializersForExpressionTree(initializerExpressionOpt);
                return UpdateInitializers(initializerExpressionOpt, newInitializers);
            }
            return null;
        }

        private BoundExpression MakeExpressionWithInitializer(SyntaxNode syntax, BoundExpression rewrittenExpression, BoundExpression initializerExpression, TypeSymbol type)
        {
            BoundLocal boundLocal = _factory.StoreToTemp(rewrittenExpression, out BoundAssignmentOperator store);
            ArrayBuilder<BoundExpression> dynamicSiteInitializers = null;
            ArrayBuilder<LocalSymbol> temps = null;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            AddObjectOrCollectionInitializers(ref dynamicSiteInitializers, ref temps, instance, boundLocal, initializerExpression);
            int num = dynamicSiteInitializers?.Count ?? 0;
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance(1 + num + instance.Count);
            instance2.Add(store);
            if (num > 0)
            {
                instance2.AddRange(dynamicSiteInitializers);
                dynamicSiteInitializers.Free();
            }
            instance2.AddRange(instance);
            instance.Free();
            ImmutableArray<LocalSymbol> locals;
            if (temps == null)
            {
                locals = ImmutableArray.Create(boundLocal.LocalSymbol);
            }
            else
            {
                temps.Insert(0, boundLocal.LocalSymbol);
                locals = temps.ToImmutableAndFree();
            }
            return new BoundSequence(syntax, locals, instance2.ToImmutableAndFree(), boundLocal, type);
        }

        public override BoundNode VisitNewT(BoundNewT node)
        {
            if (_inExpressionLambda)
            {
                return node.Update(MakeObjectCreationInitializerForExpressionTree(node.InitializerExpressionOpt), node.Type);
            }
            BoundExpression boundExpression = MakeNewT(node.Syntax, (TypeParameterSymbol)node.Type);
            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt!.HasErrors)
            {
                return boundExpression;
            }
            return MakeExpressionWithInitializer(node.Syntax, boundExpression, node.InitializerExpressionOpt, boundExpression.Type);
        }

        private BoundExpression MakeNewT(SyntaxNode syntax, TypeParameterSymbol typeParameter)
        {
            if (!TryGetWellKnownTypeMember<MethodSymbol>(syntax, WellKnownMember.System_Activator__CreateInstance_T, out var symbol))
            {
                return new BoundDefaultExpression(syntax, typeParameter, hasErrors: true);
            }
            symbol = symbol.Construct(ImmutableArray.Create((TypeSymbol)typeParameter));
            return new BoundCall(syntax, null, symbol, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, typeParameter);
        }

        public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            SyntaxNode syntax = _factory.Syntax;
            _factory.Syntax = node.Syntax;
            MethodSymbol methodSymbol = _factory.WellKnownMethod(WellKnownMember.System_Guid__ctor);
            BoundExpression arg = (((object)methodSymbol == null) ? new BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)_factory.New(methodSymbol, _factory.Literal(node.GuidString))));
            MethodSymbol methodSymbol2 = _factory.WellKnownMethod(WellKnownMember.System_Runtime_InteropServices_Marshal__GetTypeFromCLSID, isOptional: true);
            if ((object)methodSymbol2 == null)
            {
                methodSymbol2 = _factory.WellKnownMethod(WellKnownMember.System_Type__GetTypeFromCLSID);
            }
            BoundExpression arg2 = (((object)methodSymbol2 == null) ? new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)_factory.Call(null, methodSymbol2, arg)));
            MethodSymbol methodSymbol3 = _factory.WellKnownMethod(WellKnownMember.System_Activator__CreateInstance);
            BoundExpression boundExpression = (((object)methodSymbol3 == null) ? new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, node.Type) : _factory.Convert(node.Type, _factory.Call(null, methodSymbol3, arg2)));
            _factory.Syntax = syntax;
            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt!.HasErrors)
            {
                return boundExpression;
            }
            return MakeExpressionWithInitializer(node.Syntax, boundExpression, node.InitializerExpressionOpt, node.Type);
        }

        private static BoundObjectInitializerExpressionBase UpdateInitializers(BoundObjectInitializerExpressionBase initializerExpression, ImmutableArray<BoundExpression> newInitializers)
        {
            if (!(initializerExpression is BoundObjectInitializerExpression boundObjectInitializerExpression))
            {
                if (initializerExpression is BoundCollectionInitializerExpression boundCollectionInitializerExpression)
                {
                    return boundCollectionInitializerExpression.Update(boundCollectionInitializerExpression.Placeholder, newInitializers, initializerExpression.Type);
                }
                throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind);
            }
            return boundObjectInitializerExpression.Update(boundObjectInitializerExpression.Placeholder, newInitializers, initializerExpression.Type);
        }

        private void AddObjectOrCollectionInitializers(ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers, ref ArrayBuilder<LocalSymbol>? temps, ArrayBuilder<BoundExpression> result, BoundExpression rewrittenReceiver, BoundExpression initializerExpression)
        {
            if (!(initializerExpression is BoundObjectInitializerExpression boundObjectInitializerExpression))
            {
                if (!(initializerExpression is BoundCollectionInitializerExpression boundCollectionInitializerExpression))
                {
                    throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind);
                }
                BoundObjectOrCollectionValuePlaceholder placeholder = boundCollectionInitializerExpression.Placeholder;
                AddPlaceholderReplacement(placeholder, rewrittenReceiver);
                AddCollectionInitializers(ref dynamicSiteInitializers, result, rewrittenReceiver, boundCollectionInitializerExpression.Initializers);
                RemovePlaceholderReplacement(placeholder);
            }
            else
            {
                BoundObjectOrCollectionValuePlaceholder placeholder2 = boundObjectInitializerExpression.Placeholder;
                AddPlaceholderReplacement(placeholder2, rewrittenReceiver);
                AddObjectInitializers(ref dynamicSiteInitializers, ref temps, result, rewrittenReceiver, boundObjectInitializerExpression.Initializers);
                RemovePlaceholderReplacement(placeholder2);
            }
        }

        private ImmutableArray<BoundExpression> MakeObjectOrCollectionInitializersForExpressionTree(BoundExpression initializerExpression)
        {
            switch (initializerExpression.Kind)
            {
                case BoundKind.ObjectInitializerExpression:
                    return VisitList(((BoundObjectInitializerExpression)initializerExpression).Initializers);
                case BoundKind.CollectionInitializerExpression:
                    {
                        ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
                        ArrayBuilder<BoundExpression> dynamicSiteInitializers = null;
                        AddCollectionInitializers(ref dynamicSiteInitializers, instance, null, ((BoundCollectionInitializerExpression)initializerExpression).Initializers);
                        return instance.ToImmutableAndFree();
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind);
            }
        }

        private void AddCollectionInitializers(ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers, ArrayBuilder<BoundExpression> result, BoundExpression? rewrittenReceiver, ImmutableArray<BoundExpression> initializers)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                BoundExpression boundExpression = ((current.Kind != BoundKind.CollectionElementInitializer) ? MakeDynamicCollectionInitializer(rewrittenReceiver, (BoundDynamicCollectionElementInitializer)current) : MakeCollectionInitializer(rewrittenReceiver, (BoundCollectionElementInitializer)current));
                if (boundExpression != null)
                {
                    result.Add(boundExpression);
                }
            }
        }

        private BoundExpression MakeDynamicCollectionInitializer(BoundExpression rewrittenReceiver, BoundDynamicCollectionElementInitializer initializer)
        {
            ImmutableArray<BoundExpression> loweredArguments = VisitList(initializer.Arguments);
            EmbedIfNeedTo(rewrittenReceiver, initializer.ApplicableMethods, initializer.Syntax);
            return _dynamicFactory.MakeDynamicMemberInvocation("Add", rewrittenReceiver, ImmutableArray<TypeWithAnnotations>.Empty, loweredArguments, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), hasImplicitReceiver: false, resultDiscarded: true).ToExpression();
        }

        private BoundExpression? MakeCollectionInitializer(BoundExpression? rewrittenReceiver, BoundCollectionElementInitializer initializer)
        {
            MethodSymbol addMethod = initializer.AddMethod;
            SyntaxNode syntax = initializer.Syntax;
            if (_allowOmissionOfConditionalCalls && addMethod.CallsAreOmitted(initializer.SyntaxTree))
            {
                return null;
            }
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(initializer.Arguments);
            TypeSymbol type = VisitType(initializer.Type);
            ImmutableArray<RefKind> argumentRefKindsOpt = default(ImmutableArray<RefKind>);
            if (addMethod.Parameters[0].RefKind == RefKind.Ref)
            {
                ArrayBuilder<RefKind> instance = ArrayBuilder<RefKind>.GetInstance(addMethod.Parameters.Length, RefKind.None);
                instance[0] = RefKind.Ref;
                argumentRefKindsOpt = instance.ToImmutableAndFree();
            }
            rewrittenArguments = MakeArguments(syntax, rewrittenArguments, addMethod, initializer.Expanded, initializer.ArgsToParamsOpt, ref argumentRefKindsOpt, out var temps, invokedAsExtensionMethod: false, ThreeState.True);
            if (initializer.InvokedAsExtensionMethod)
            {
                rewrittenReceiver = null;
            }
            if (_inExpressionLambda)
            {
                return initializer.Update(addMethod, rewrittenArguments, rewrittenReceiver, expanded: false, default(ImmutableArray<int>), default(BitVector), initializer.InvokedAsExtensionMethod, initializer.ResultKind, type);
            }
            return MakeCall(null, syntax, rewrittenReceiver, addMethod, rewrittenArguments, argumentRefKindsOpt, initializer.InvokedAsExtensionMethod, initializer.ResultKind, addMethod.ReturnType, temps);
        }

        private void AddObjectInitializers(ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers, ref ArrayBuilder<LocalSymbol>? temps, ArrayBuilder<BoundExpression> result, BoundExpression rewrittenReceiver, ImmutableArray<BoundExpression> initializers)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                AddObjectInitializer(ref dynamicSiteInitializers, ref temps, result, rewrittenReceiver, (BoundAssignmentOperator)current);
            }
        }

        private void AddObjectInitializer(ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers, ref ArrayBuilder<LocalSymbol>? temps, ArrayBuilder<BoundExpression> result, BoundExpression rewrittenReceiver, BoundAssignmentOperator assignment)
        {
            BoundExpression boundExpression = null;
            if (assignment.Left.Kind != BoundKind.PointerElementAccess)
            {
                boundExpression = VisitExpression(assignment.Left);
            }
            BoundKind kind = assignment.Right.Kind;
            bool flag = kind == BoundKind.ObjectInitializerExpression || kind == BoundKind.CollectionInitializerExpression;
            BoundExpression boundExpression2;
            switch ((boundExpression ?? assignment.Left).Kind)
            {
                case BoundKind.ObjectInitializerMember:
                    {
                        BoundObjectInitializerMember boundObjectInitializerMember = (BoundObjectInitializerMember)boundExpression;
                        if (!boundObjectInitializerMember.Arguments.IsDefaultOrEmpty)
                        {
                            ImmutableArray<BoundExpression> arguments = EvaluateSideEffectingArgumentsToTemps(boundObjectInitializerMember.Arguments, boundObjectInitializerMember.MemberSymbol?.GetParameterRefKinds() ?? default(ImmutableArray<RefKind>), result, ref temps);
                            boundObjectInitializerMember = boundObjectInitializerMember.Update(boundObjectInitializerMember.MemberSymbol, arguments, boundObjectInitializerMember.ArgumentNamesOpt, boundObjectInitializerMember.ArgumentRefKindsOpt, boundObjectInitializerMember.Expanded, boundObjectInitializerMember.ArgsToParamsOpt, boundObjectInitializerMember.DefaultArguments, boundObjectInitializerMember.ResultKind, boundObjectInitializerMember.ReceiverType, boundObjectInitializerMember.Type);
                        }
                        if (boundObjectInitializerMember.MemberSymbol == null && boundObjectInitializerMember.Type.IsDynamic())
                        {
                            if (dynamicSiteInitializers == null)
                            {
                                dynamicSiteInitializers = ArrayBuilder<BoundExpression>.GetInstance();
                            }
                            if (!flag)
                            {
                                BoundExpression loweredRight = VisitExpression(assignment.Right);
                                LoweredDynamicOperation loweredDynamicOperation = _dynamicFactory.MakeDynamicSetIndex(rewrittenReceiver, boundObjectInitializerMember.Arguments, boundObjectInitializerMember.ArgumentNamesOpt, boundObjectInitializerMember.ArgumentRefKindsOpt, loweredRight);
                                dynamicSiteInitializers!.Add(loweredDynamicOperation.SiteInitialization);
                                result.Add(loweredDynamicOperation.SiteInvocation);
                                return;
                            }
                            LoweredDynamicOperation loweredDynamicOperation2 = _dynamicFactory.MakeDynamicGetIndex(rewrittenReceiver, boundObjectInitializerMember.Arguments, boundObjectInitializerMember.ArgumentNamesOpt, boundObjectInitializerMember.ArgumentRefKindsOpt);
                            dynamicSiteInitializers!.Add(loweredDynamicOperation2.SiteInitialization);
                            boundExpression2 = loweredDynamicOperation2.SiteInvocation;
                        }
                        else
                        {
                            boundExpression2 = MakeObjectInitializerMemberAccess(rewrittenReceiver, boundObjectInitializerMember, flag);
                            if (!flag)
                            {
                                BoundExpression rewrittenRight2 = VisitExpression(assignment.Right);
                                result.Add(MakeStaticAssignmentOperator(assignment.Syntax, boundExpression2, rewrittenRight2, isRef: false, assignment.Type, used: false));
                                return;
                            }
                        }
                        break;
                    }
                case BoundKind.DynamicObjectInitializerMember:
                    {
                        if (dynamicSiteInitializers == null)
                        {
                            dynamicSiteInitializers = ArrayBuilder<BoundExpression>.GetInstance();
                        }
                        BoundDynamicObjectInitializerMember boundDynamicObjectInitializerMember = (BoundDynamicObjectInitializerMember)boundExpression;
                        if (!flag)
                        {
                            BoundExpression loweredRight2 = VisitExpression(assignment.Right);
                            LoweredDynamicOperation loweredDynamicOperation3 = _dynamicFactory.MakeDynamicSetMember(rewrittenReceiver, boundDynamicObjectInitializerMember.MemberName, loweredRight2);
                            dynamicSiteInitializers!.Add(loweredDynamicOperation3.SiteInitialization);
                            result.Add(loweredDynamicOperation3.SiteInvocation);
                            return;
                        }
                        LoweredDynamicOperation loweredDynamicOperation4 = _dynamicFactory.MakeDynamicGetMember(rewrittenReceiver, boundDynamicObjectInitializerMember.MemberName, resultIndexed: false);
                        dynamicSiteInitializers!.Add(loweredDynamicOperation4.SiteInitialization);
                        boundExpression2 = loweredDynamicOperation4.SiteInvocation;
                        break;
                    }
                case BoundKind.ArrayAccess:
                    {
                        BoundArrayAccess boundArrayAccess = (BoundArrayAccess)boundExpression;
                        ImmutableArray<BoundExpression> indices = EvaluateSideEffectingArgumentsToTemps(boundArrayAccess.Indices, default(ImmutableArray<RefKind>), result, ref temps);
                        boundExpression2 = boundArrayAccess.Update(rewrittenReceiver, indices, boundArrayAccess.Type);
                        if (!flag)
                        {
                            BoundExpression rewrittenRight = VisitExpression(assignment.Right);
                            result.Add(MakeStaticAssignmentOperator(assignment.Syntax, boundExpression2, rewrittenRight, isRef: false, assignment.Type, used: false));
                            return;
                        }
                        break;
                    }
                case BoundKind.PointerElementAccess:
                    {
                        BoundPointerElementAccess boundPointerElementAccess = (BoundPointerElementAccess)assignment.Left;
                        BoundExpression boundExpression3 = VisitExpression(boundPointerElementAccess.Index);
                        if (CanChangeValueBetweenReads(boundExpression3))
                        {
                            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression3, out BoundAssignmentOperator store);
                            boundExpression3 = boundLocal;
                            if (temps == null)
                            {
                                temps = ArrayBuilder<LocalSymbol>.GetInstance();
                            }
                            temps!.Add(boundLocal.LocalSymbol);
                            result.Add(store);
                        }
                        boundExpression2 = RewritePointerElementAccess(boundPointerElementAccess, rewrittenReceiver, boundExpression3);
                        if (!flag)
                        {
                            BoundExpression rewrittenRight3 = VisitExpression(assignment.Right);
                            result.Add(MakeStaticAssignmentOperator(assignment.Syntax, boundExpression2, rewrittenRight3, isRef: false, assignment.Type, used: false));
                            return;
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue((boundExpression ?? assignment.Left).Kind);
            }
            AddObjectOrCollectionInitializers(ref dynamicSiteInitializers, ref temps, result, boundExpression2, assignment.Right);
        }

        private ImmutableArray<BoundExpression> EvaluateSideEffectingArgumentsToTemps(ImmutableArray<BoundExpression> args, ImmutableArray<RefKind> paramRefKindsOpt, ArrayBuilder<BoundExpression> sideeffects, ref ArrayBuilder<LocalSymbol>? temps)
        {
            ArrayBuilder<BoundExpression> arrayBuilder = null;
            for (int i = 0; i < args.Length; i++)
            {
                BoundExpression boundExpression = args[i];
                if (CanChangeValueBetweenReads(boundExpression))
                {
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<BoundExpression>.GetInstance(args.Length);
                        arrayBuilder.AddRange(args, i);
                    }
                    RefKind refKind = paramRefKindsOpt.RefKinds(i);
                    BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store, refKind);
                    arrayBuilder.Add(boundLocal);
                    if (temps == null)
                    {
                        temps = ArrayBuilder<LocalSymbol>.GetInstance();
                    }
                    temps!.Add(boundLocal.LocalSymbol);
                    sideeffects.Add(store);
                }
                else
                {
                    arrayBuilder?.Add(boundExpression);
                }
            }
            return arrayBuilder?.ToImmutableAndFree() ?? args;
        }

        private BoundExpression MakeObjectInitializerMemberAccess(BoundExpression rewrittenReceiver, BoundObjectInitializerMember rewrittenLeft, bool isRhsNestedInitializer)
        {
            Symbol memberSymbol = rewrittenLeft.MemberSymbol;
            switch (memberSymbol.Kind)
            {
                case SymbolKind.Field:
                    {
                        FieldSymbol fieldSymbol = (FieldSymbol)memberSymbol;
                        return MakeFieldAccess(rewrittenLeft.Syntax, rewrittenReceiver, fieldSymbol, null, rewrittenLeft.ResultKind, fieldSymbol.Type);
                    }
                case SymbolKind.Property:
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)memberSymbol;
                        if (!rewrittenLeft.Arguments.IsEmpty || propertySymbol.IsIndexedProperty)
                        {
                            return MakeIndexerAccess(rewrittenLeft.Syntax, rewrittenReceiver, propertySymbol, rewrittenLeft.Arguments, rewrittenLeft.ArgumentNamesOpt, rewrittenLeft.ArgumentRefKindsOpt, rewrittenLeft.Expanded, rewrittenLeft.ArgsToParamsOpt, rewrittenLeft.DefaultArguments, propertySymbol.Type, null, !isRhsNestedInitializer);
                        }
                        return MakePropertyAccess(rewrittenLeft.Syntax, rewrittenReceiver, propertySymbol, rewrittenLeft.ResultKind, propertySymbol.Type, !isRhsNestedInitializer);
                    }
                case SymbolKind.Event:
                    {
                        EventSymbol eventSymbol = (EventSymbol)memberSymbol;
                        return MakeEventAccess(rewrittenLeft.Syntax, rewrittenReceiver, eventSymbol, null, rewrittenLeft.ResultKind, eventSymbol.Type);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(memberSymbol.Kind);
            }
        }

        public override BoundNode VisitSwitchStatement(BoundSwitchStatement node)
        {
            return SwitchStatementLocalRewriter.Rewrite(this, node);
        }

        public override BoundNode VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            BoundExpression rewrittenExpression = LowerReceiverOfPointerElementAccess(node.Expression);
            BoundExpression rewrittenIndex = VisitExpression(node.Index);
            return RewritePointerElementAccess(node, rewrittenExpression, rewrittenIndex);
        }

        private BoundExpression LowerReceiverOfPointerElementAccess(BoundExpression receiver)
        {
            if (receiver is BoundFieldAccess boundFieldAccess && boundFieldAccess.FieldSymbol.IsFixedSizeBuffer)
            {
                BoundExpression receiver2 = VisitExpression(boundFieldAccess.ReceiverOpt);
                BoundFieldAccess boundFieldAccess2 = boundFieldAccess.Update(receiver2, boundFieldAccess.FieldSymbol, boundFieldAccess.ConstantValueOpt, boundFieldAccess.ResultKind, boundFieldAccess.Type);
                return new BoundAddressOfOperator(receiver.Syntax, boundFieldAccess2, isManaged: true, boundFieldAccess2.Type);
            }
            return VisitExpression(receiver);
        }

        private BoundExpression RewritePointerElementAccess(BoundPointerElementAccess node, BoundExpression rewrittenExpression, BoundExpression rewrittenIndex)
        {
            if (rewrittenIndex.IsDefaultValue())
            {
                return new BoundPointerIndirectionOperator(node.Syntax, rewrittenExpression, node.Type);
            }
            BinaryOperatorKind binaryOperatorKind = BinaryOperatorKind.Addition;
            binaryOperatorKind = rewrittenIndex.Type!.SpecialType switch
            {
                SpecialType.System_Int32 => binaryOperatorKind | BinaryOperatorKind.PointerAndIntAddition,
                SpecialType.System_UInt32 => binaryOperatorKind | BinaryOperatorKind.PointerAndUIntAddition,
                SpecialType.System_Int64 => binaryOperatorKind | BinaryOperatorKind.PointerAndLongAddition,
                SpecialType.System_UInt64 => binaryOperatorKind | BinaryOperatorKind.PointerAndULongAddition,
                _ => throw ExceptionUtilities.UnexpectedValue(rewrittenIndex.Type!.SpecialType),
            };
            if (node.Checked)
            {
                binaryOperatorKind |= BinaryOperatorKind.Checked;
            }
            return new BoundPointerIndirectionOperator(node.Syntax, MakeBinaryOperator(node.Syntax, binaryOperatorKind, rewrittenExpression, rewrittenIndex, rewrittenExpression.Type, null, isPointerElementAccess: true), node.Type);
        }

        public override BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            ImplicitNamedTypeSymbol previousSubmissionType = (ImplicitNamedTypeSymbol)node.Type;
            SyntaxNode syntax = node.Syntax;
            FieldSymbol orMakeField = _previousSubmissionFields.GetOrMakeField(previousSubmissionType);
            BoundThisReference receiver = new BoundThisReference(syntax, _factory.CurrentType);
            return new BoundFieldAccess(syntax, receiver, orMakeField, null);
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            return VisitPropertyAccess(node, isLeftOfAssignment: false);
        }

        private BoundExpression VisitPropertyAccess(BoundPropertyAccess node, bool isLeftOfAssignment)
        {
            BoundExpression rewrittenReceiverOpt = VisitExpression(node.ReceiverOpt);
            return MakePropertyAccess(node.Syntax, rewrittenReceiverOpt, node.PropertySymbol, node.ResultKind, node.Type, isLeftOfAssignment, node);
        }

        private BoundExpression MakePropertyAccess(SyntaxNode syntax, BoundExpression? rewrittenReceiverOpt, PropertySymbol propertySymbol, LookupResultKind resultKind, TypeSymbol type, bool isLeftOfAssignment, BoundPropertyAccess? oldNodeOpt = null)
        {
            if (rewrittenReceiverOpt != null)
            {
                TypeSymbol type2 = rewrittenReceiverOpt!.Type;
                if ((object)type2 != null && type2.TypeKind == TypeKind.Array && !isLeftOfAssignment && ((ArrayTypeSymbol)rewrittenReceiverOpt!.Type).IsSZArray && ((object)propertySymbol == _compilation.GetSpecialTypeMember(SpecialMember.System_Array__Length) || (!_inExpressionLambda && (object)propertySymbol == _compilation.GetSpecialTypeMember(SpecialMember.System_Array__LongLength))))
                {
                    return new BoundArrayLength(syntax, rewrittenReceiverOpt, type);
                }
            }
            if (isLeftOfAssignment && propertySymbol.RefKind == RefKind.None)
            {
                if (oldNodeOpt == null)
                {
                    return new BoundPropertyAccess(syntax, rewrittenReceiverOpt, propertySymbol, resultKind, type);
                }
                return oldNodeOpt!.Update(rewrittenReceiverOpt, propertySymbol, resultKind, type);
            }
            return MakePropertyGetAccess(syntax, rewrittenReceiverOpt, propertySymbol, oldNodeOpt);
        }

        private BoundExpression MakePropertyGetAccess(SyntaxNode syntax, BoundExpression? rewrittenReceiver, PropertySymbol property, BoundPropertyAccess? oldNodeOpt)
        {
            return MakePropertyGetAccess(syntax, rewrittenReceiver, property, ImmutableArray<BoundExpression>.Empty, null, oldNodeOpt);
        }

        private BoundExpression MakePropertyGetAccess(SyntaxNode syntax, BoundExpression? rewrittenReceiver, PropertySymbol property, ImmutableArray<BoundExpression> rewrittenArguments, MethodSymbol? getMethodOpt = null, BoundPropertyAccess? oldNodeOpt = null)
        {
            if (_inExpressionLambda && rewrittenArguments.IsEmpty)
            {
                if (oldNodeOpt == null)
                {
                    return new BoundPropertyAccess(syntax, rewrittenReceiver, property, LookupResultKind.Viable, property.Type);
                }
                return oldNodeOpt!.Update(rewrittenReceiver, property, LookupResultKind.Viable, property.Type);
            }
            MethodSymbol method = getMethodOpt ?? property.GetOwnOrInheritedGetMethod();
            return BoundCall.Synthesized(syntax, rewrittenReceiver, method, rewrittenArguments);
        }

        public override BoundNode VisitRangeVariable(BoundRangeVariable node)
        {
            return VisitExpression(node.Value);
        }

        public override BoundNode VisitQueryClause(BoundQueryClause node)
        {
            return VisitExpression(node.Value);
        }

        public override BoundNode VisitRangeExpression(BoundRangeExpression node)
        {
            bool needLifting = false;
            _ = _factory;
            BoundExpression boundExpression = node.LeftOperandOpt;
            if (boundExpression != null)
            {
                boundExpression = tryOptimizeOperand(boundExpression);
            }
            BoundExpression boundExpression2 = node.RightOperandOpt;
            if (boundExpression2 != null)
            {
                boundExpression2 = tryOptimizeOperand(boundExpression2);
            }
            if (needLifting)
            {
                return LiftRangeExpression(node, boundExpression, boundExpression2);
            }
            BoundExpression boundExpression3 = MakeRangeExpression(node.MethodOpt, boundExpression, boundExpression2);
            if (node.Type.IsNullableType())
            {
                if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out var result))
                {
                    return BadExpression(node.Syntax, node.Type, node);
                }
                return new BoundObjectCreationExpression(node.Syntax, result, boundExpression3);
            }
            return boundExpression3;
            BoundExpression tryOptimizeOperand(BoundExpression operand)
            {
                operand = VisitExpression(operand);
                if (NullableNeverHasValue(operand))
                {
                    operand = new BoundDefaultExpression(operand.Syntax, operand.Type.GetNullableUnderlyingType());
                }
                else
                {
                    operand = NullableAlwaysHasValue(operand) ?? operand;
                    if (operand.Type.IsNullableType())
                    {
                        needLifting = true;
                    }
                }
                return operand;
            }
        }

        private BoundExpression LiftRangeExpression(BoundRangeExpression node, BoundExpression? left, BoundExpression? right)
        {
            BoundRangeExpression node2 = node;
            ArrayBuilder<BoundExpression> sideeffects = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> locals = ArrayBuilder<LocalSymbol>.GetInstance();
            BoundExpression condition = null;
            left = getIndexFromPossibleNullable(left);
            right = getIndexFromPossibleNullable(right);
            BoundExpression boundExpression = MakeRangeExpression(node2.MethodOpt, left, right);
            if (!TryGetNullableMethod(node2.Syntax, node2.Type, SpecialMember.System_Nullable_T__ctor, out var result))
            {
                return BadExpression(node2.Syntax, node2.Type, node2);
            }
            BoundExpression rewrittenConsequence = new BoundObjectCreationExpression(node2.Syntax, result, boundExpression);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(node2.Syntax, node2.Type);
            BoundExpression value = RewriteConditionalOperator(node2.Syntax, condition, rewrittenConsequence, rewrittenAlternative, null, node2.Type, isRef: false);
            return new BoundSequence(node2.Syntax, locals.ToImmutableAndFree(), sideeffects.ToImmutableAndFree(), value, node2.Type);
            BoundExpression? getIndexFromPossibleNullable(BoundExpression? arg)
            {
                if (arg == null)
                {
                    return null;
                }
                BoundExpression boundExpression2 = CaptureExpressionInTempIfNeeded(arg, sideeffects, locals);
                if (boundExpression2.Type.IsNullableType())
                {
                    BoundExpression boundExpression3 = MakeOptimizedHasValue(boundExpression2.Syntax, boundExpression2);
                    if (condition == null)
                    {
                        condition = boundExpression3;
                    }
                    else
                    {
                        TypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                        condition = MakeBinaryOperator(node2.Syntax, BinaryOperatorKind.BoolAnd, condition, boundExpression3, specialType, null);
                    }
                    return MakeOptimizedGetValueOrDefault(boundExpression2.Syntax, boundExpression2);
                }
                return boundExpression2;
            }
        }

        private BoundExpression MakeRangeExpression(MethodSymbol constructionMethod, BoundExpression? left, BoundExpression? right)
        {
            SyntheticBoundNodeFactory F = _factory;
            switch (constructionMethod.MethodKind)
            {
                case MethodKind.Constructor:
                    left = left ?? newIndexZero(fromEnd: false);
                    right = right ?? newIndexZero(fromEnd: true);
                    return F.New(constructionMethod, ImmutableArray.Create<BoundExpression>(left, right));
                case MethodKind.Ordinary:
                    {
                        BoundExpression item = left ?? right;
                        return F.StaticCall(constructionMethod, ImmutableArray.Create(item));
                    }
                case MethodKind.PropertyGet:
                    return F.StaticCall(constructionMethod, ImmutableArray<BoundExpression>.Empty);
                default:
                    throw ExceptionUtilities.UnexpectedValue(constructionMethod.MethodKind);
            }
            BoundExpression newIndexZero(bool fromEnd)
            {
                return F.New(WellKnownMember.System_Index__ctor, ImmutableArray.Create(F.Literal(0), (BoundExpression)F.Literal(fromEnd)));
            }
        }

        public override BoundNode VisitReturnStatement(BoundReturnStatement node)
        {
            BoundStatement boundStatement = (BoundStatement)base.VisitReturnStatement(node);
            bool num;
            if (Instrument)
            {
                if (!node.WasCompilerGenerated)
                {
                    goto IL_005e;
                }
                if (node.ExpressionOpt != null)
                {
                    num = IsLambdaOrExpressionBodiedMember;
                    goto IL_005c;
                }
                if (node.Syntax.Kind() == SyntaxKind.Block)
                {
                    MethodSymbol? currentFunction = _factory.CurrentFunction;
                    if ((object)currentFunction != null)
                    {
                        num = !currentFunction!.IsAsync;
                        goto IL_005c;
                    }
                }
            }
            goto IL_006c;
        IL_005e:
            boundStatement = _instrumenter.InstrumentReturnStatement(node, boundStatement);
            goto IL_006c;
        IL_006c:
            return boundStatement;
        IL_005c:
            if (num)
            {
                goto IL_005e;
            }
            goto IL_006c;
        }

        public override BoundNode VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression stackAllocNode)
        {
            return VisitStackAllocArrayCreationBase(stackAllocNode);
        }

        public override BoundNode VisitStackAllocArrayCreation(BoundStackAllocArrayCreation stackAllocNode)
        {
            return VisitStackAllocArrayCreationBase(stackAllocNode);
        }

        private BoundNode VisitStackAllocArrayCreationBase(BoundStackAllocArrayCreationBase stackAllocNode)
        {
            BoundExpression boundExpression = VisitExpression(stackAllocNode.Count);
            TypeSymbol type = stackAllocNode.Type;
            ConstantValue? constantValue = boundExpression.ConstantValue;
            if ((object)constantValue != null && constantValue!.Int32Value == 0)
            {
                return _factory.Default(type);
            }
            TypeSymbol elementType = stackAllocNode.ElementType;
            BoundArrayInitialization boundArrayInitialization = stackAllocNode.InitializerOpt;
            if (boundArrayInitialization != null)
            {
                boundArrayInitialization = boundArrayInitialization.Update(VisitList(boundArrayInitialization.Initializers));
            }
            if (type.IsPointerType())
            {
                BoundExpression count = RewriteStackAllocCountToSize(boundExpression, elementType);
                return new BoundConvertedStackAllocExpression(stackAllocNode.Syntax, elementType, count, boundArrayInitialization, type);
            }
            if (TypeSymbol.Equals(type.OriginalDefinition, _compilation.GetWellKnownType(WellKnownType.System_Span_T), TypeCompareKind.ConsiderEverything))
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
                ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
                BoundExpression boundExpression2 = CaptureExpressionInTempIfNeeded(boundExpression, instance, instance2, SynthesizedLocalKind.Spill);
                BoundExpression count2 = RewriteStackAllocCountToSize(boundExpression2, elementType);
                stackAllocNode = new BoundConvertedStackAllocExpression(stackAllocNode.Syntax, elementType, count2, boundArrayInitialization, _compilation.CreatePointerTypeSymbol(elementType));
                BoundExpression argument = ((!TryGetWellKnownTypeMember<MethodSymbol>(stackAllocNode.Syntax, WellKnownMember.System_Span_T__ctor, out MethodSymbol symbol)) ? new BoundBadExpression(stackAllocNode.Syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType) : ((BoundExpression)_factory.New((MethodSymbol)symbol.SymbolAsMember(namedTypeSymbol), stackAllocNode, boundExpression2)));
                _needsSpilling = true;
                BoundLocal boundLocal = _factory.StoreToTemp(argument, out BoundAssignmentOperator store, RefKind.None, SynthesizedLocalKind.LoweringTemp, stackAllocNode.Syntax);
                instance.Add(store);
                instance2.Add(boundLocal.LocalSymbol);
                return new BoundSpillSequence(stackAllocNode.Syntax, instance2.ToImmutableAndFree(), instance.ToImmutableAndFree(), boundLocal, namedTypeSymbol);
            }
            throw ExceptionUtilities.UnexpectedValue(type);
        }

        private BoundExpression RewriteStackAllocCountToSize(BoundExpression countExpression, TypeSymbol elementType)
        {
            TypeSymbol type = _factory.SpecialType(SpecialType.System_UInt32);
            TypeSymbol type2 = _factory.SpecialType(SpecialType.System_UIntPtr);
            BoundExpression boundExpression = _factory.Sizeof(elementType);
            ConstantValue constantValue = boundExpression.ConstantValue;
            if (constantValue != null)
            {
                int int32Value = constantValue.Int32Value;
                ConstantValue constantValue2 = countExpression.ConstantValue;
                if (constantValue2 != null)
                {
                    long num = (uint)constantValue2.Int32Value * int32Value;
                    if (num < uint.MaxValue)
                    {
                        return _factory.Convert(type2, _factory.Literal((uint)num), Conversion.IntegerToPointer);
                    }
                }
            }
            BoundExpression arg = _factory.Convert(type, countExpression, Conversion.ExplicitNumeric);
            arg = _factory.Convert(type2, arg, Conversion.IntegerToPointer);
            if ((object)constantValue != null && constantValue.Int32Value == 1)
            {
                return arg;
            }
            BinaryOperatorKind kind = BinaryOperatorKind.UIntMultiplication | BinaryOperatorKind.Checked;
            return _factory.Binary(kind, type2, arg, boundExpression);
        }

        private BoundExpression RewriteStringConcatenation(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type)
        {
            if (_inExpressionLambda)
            {
                return RewriteStringConcatInExpressionLambda(syntax, operatorKind, loweredLeft, loweredRight, type);
            }
            loweredLeft = ConvertConcatExprToString(syntax, loweredLeft);
            loweredRight = ConvertConcatExprToString(syntax, loweredRight);
            BoundExpression boundExpression = TryFoldTwoConcatOperands(syntax, loweredLeft, loweredRight);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            FlattenConcatArg(loweredLeft, instance);
            FlattenConcatArg(loweredRight, instance2);
            if (instance.Any() && instance2.Any())
            {
                boundExpression = TryFoldTwoConcatOperands(syntax, instance.Last(), instance2.First());
                if (boundExpression != null)
                {
                    instance2[0] = boundExpression;
                    instance.RemoveLast();
                }
            }
            instance.AddRange(instance2);
            instance2.Free();
            BoundExpression result;
            switch (instance.Count)
            {
                case 0:
                    result = _factory.StringLiteral(string.Empty);
                    break;
                case 1:
                    result = instance[0];
                    break;
                case 2:
                    {
                        BoundExpression loweredLeft2 = instance[0];
                        BoundExpression loweredRight2 = instance[1];
                        result = RewriteStringConcatenationTwoExprs(syntax, loweredLeft2, loweredRight2);
                        break;
                    }
                case 3:
                    {
                        BoundExpression loweredFirst2 = instance[0];
                        BoundExpression loweredSecond2 = instance[1];
                        BoundExpression loweredThird2 = instance[2];
                        result = RewriteStringConcatenationThreeExprs(syntax, loweredFirst2, loweredSecond2, loweredThird2);
                        break;
                    }
                case 4:
                    {
                        BoundExpression loweredFirst = instance[0];
                        BoundExpression loweredSecond = instance[1];
                        BoundExpression loweredThird = instance[2];
                        BoundExpression loweredFourth = instance[3];
                        result = RewriteStringConcatenationFourExprs(syntax, loweredFirst, loweredSecond, loweredThird, loweredFourth);
                        break;
                    }
                default:
                    result = RewriteStringConcatenationManyExprs(syntax, instance.ToImmutable());
                    break;
            }
            instance.Free();
            return result;
        }

        private void FlattenConcatArg(BoundExpression lowered, ArrayBuilder<BoundExpression> flattened)
        {
            if (TryExtractStringConcatArgs(lowered, out var arguments))
            {
                flattened.AddRange(arguments);
            }
            else
            {
                flattened.Add(lowered);
            }
        }

        private bool TryExtractStringConcatArgs(BoundExpression lowered, out ImmutableArray<BoundExpression> arguments)
        {
            switch (lowered.Kind)
            {
                case BoundKind.Call:
                    {
                        BoundCall boundCall = (BoundCall)lowered;
                        MethodSymbol method = boundCall.Method;
                        if (!method.IsStatic || method.ContainingType.SpecialType != SpecialType.System_String)
                        {
                            break;
                        }
                        if ((object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString) || (object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString))
                        {
                            arguments = boundCall.Arguments;
                            return true;
                        }
                        if ((object)method == _compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray) && boundCall.Arguments[0] is BoundArrayCreation boundArrayCreation)
                        {
                            BoundArrayInitialization initializerOpt = boundArrayCreation.InitializerOpt;
                            if (initializerOpt != null)
                            {
                                arguments = initializerOpt.Initializers;
                                return true;
                            }
                        }
                        break;
                    }
                case BoundKind.NullCoalescingOperator:
                    {
                        BoundNullCoalescingOperator boundNullCoalescingOperator = (BoundNullCoalescingOperator)lowered;
                        if (boundNullCoalescingOperator.LeftConversion.IsIdentity)
                        {
                            ConstantValue constantValue = boundNullCoalescingOperator.RightOperand.ConstantValue;
                            if (constantValue != null && constantValue.IsString && constantValue.StringValue!.Length == 0)
                            {
                                arguments = ImmutableArray.Create(boundNullCoalescingOperator.LeftOperand);
                                return true;
                            }
                        }
                        break;
                    }
            }
            arguments = default(ImmutableArray<BoundExpression>);
            return false;
        }

        private BoundExpression? TryFoldTwoConcatOperands(SyntaxNode syntax, BoundExpression loweredLeft, BoundExpression loweredRight)
        {
            ConstantValue constantValue = loweredLeft.ConstantValue;
            ConstantValue constantValue2 = loweredRight.ConstantValue;
            if (constantValue != null && constantValue2 != null)
            {
                ConstantValue constantValue3 = TryFoldTwoConcatConsts(constantValue, constantValue2);
                if (constantValue3 != null)
                {
                    return _factory.StringLiteral(constantValue3);
                }
            }
            if (IsNullOrEmptyStringConstant(loweredLeft))
            {
                if (IsNullOrEmptyStringConstant(loweredRight))
                {
                    return _factory.Literal(string.Empty);
                }
                return RewriteStringConcatenationOneExpr(syntax, loweredRight);
            }
            if (IsNullOrEmptyStringConstant(loweredRight))
            {
                return RewriteStringConcatenationOneExpr(syntax, loweredLeft);
            }
            return null;
        }

        private static bool IsNullOrEmptyStringConstant(BoundExpression operand)
        {
            if (!(operand.ConstantValue != null) || !string.IsNullOrEmpty(operand.ConstantValue!.StringValue))
            {
                return operand.IsDefaultValue();
            }
            return true;
        }

        private static ConstantValue? TryFoldTwoConcatConsts(ConstantValue leftConst, ConstantValue rightConst)
        {
            string stringValue = leftConst.StringValue;
            string stringValue2 = rightConst.StringValue;
            if (!leftConst.IsDefaultValue && !rightConst.IsDefaultValue && stringValue.Length + stringValue2.Length < 0)
            {
                return null;
            }
            return ConstantValue.Create(stringValue + stringValue2);
        }

        private BoundExpression RewriteStringConcatenationOneExpr(SyntaxNode syntax, BoundExpression loweredOperand)
        {
            if (TryExtractStringConcatArgs(loweredOperand, out var _))
            {
                return loweredOperand;
            }
            return _factory.Coalesce(loweredOperand, _factory.Literal(""));
        }

        private BoundExpression RewriteStringConcatenationTwoExprs(SyntaxNode syntax, BoundExpression loweredLeft, BoundExpression loweredRight)
        {
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, SpecialMember.System_String__ConcatStringString);
            return BoundCall.Synthesized(syntax, null, method, loweredLeft, loweredRight);
        }

        private BoundExpression RewriteStringConcatenationThreeExprs(SyntaxNode syntax, BoundExpression loweredFirst, BoundExpression loweredSecond, BoundExpression loweredThird)
        {
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, SpecialMember.System_String__ConcatStringStringString);
            return BoundCall.Synthesized(syntax, null, method, ImmutableArray.Create(loweredFirst, loweredSecond, loweredThird));
        }

        private BoundExpression RewriteStringConcatenationFourExprs(SyntaxNode syntax, BoundExpression loweredFirst, BoundExpression loweredSecond, BoundExpression loweredThird, BoundExpression loweredFourth)
        {
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, SpecialMember.System_String__ConcatStringStringStringString);
            return BoundCall.Synthesized(syntax, null, method, ImmutableArray.Create(loweredFirst, loweredSecond, loweredThird, loweredFourth));
        }

        private BoundExpression RewriteStringConcatenationManyExprs(SyntaxNode syntax, ImmutableArray<BoundExpression> loweredArgs)
        {
            MethodSymbol method = UnsafeGetSpecialTypeMethod(syntax, SpecialMember.System_String__ConcatStringArray);
            BoundExpression arg = _factory.ArrayOrEmpty(_factory.SpecialType(SpecialType.System_String), loweredArgs);
            return BoundCall.Synthesized(syntax, null, method, arg);
        }

        private BoundExpression RewriteStringConcatInExpressionLambda(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression loweredLeft, BoundExpression loweredRight, TypeSymbol type)
        {
            SpecialMember specialMember = ((operatorKind == BinaryOperatorKind.StringConcatenation) ? SpecialMember.System_String__ConcatStringString : SpecialMember.System_String__ConcatObjectObject);
            MethodSymbol methodOpt = UnsafeGetSpecialTypeMethod(syntax, specialMember);
            return new BoundBinaryOperator(syntax, operatorKind, null, methodOpt, LookupResultKind.Empty, loweredLeft, loweredRight, type);
        }

        private BoundExpression ConvertConcatExprToString(SyntaxNode syntax, BoundExpression expr)
        {
            SyntaxNode syntax2 = syntax;
            BoundExpression expr2 = expr;
            if (expr2.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)expr2;
                if (boundConversion.ConversionKind == ConversionKind.Boxing)
                {
                    expr2 = boundConversion.Operand;
                }
            }
            if (expr2 != null)
            {
                ConstantValue constantValue = expr2.ConstantValue;
                if ((object)constantValue != null)
                {
                    if (constantValue.SpecialType == SpecialType.System_Char)
                    {
                        return _factory.StringLiteral(constantValue.CharValue.ToString());
                    }
                    if (constantValue.IsNull)
                    {
                        return expr2;
                    }
                }
            }
            if (expr2.Type.IsStringType())
            {
                return expr2;
            }
            MethodSymbol objectToStringMethod = UnsafeGetSpecialTypeMethod(syntax2, SpecialMember.System_Object__ToString);
            MethodSymbol methodSymbol = null;
            if (expr2.Type!.IsValueType && !expr2.Type.IsTypeParameter())
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)expr2.Type;
                ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(objectToStringMethod.Name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is MethodSymbol methodSymbol2 && (object)methodSymbol2.GetLeastOverriddenMethod(namedTypeSymbol) == objectToStringMethod)
                    {
                        methodSymbol = methodSymbol2;
                        break;
                    }
                }
            }
            if (methodSymbol != null && expr2.Type!.SpecialType != 0 && !isFieldOfMarshalByRef(expr2, _compilation))
            {
                return BoundCall.Synthesized(expr2.Syntax, expr2, methodSymbol);
            }
            bool flag = expr2.Type!.IsReferenceType || expr2.ConstantValue != null || (methodSymbol == null && !expr2.Type.IsTypeParameter()) || (methodSymbol?.IsEffectivelyReadOnly ?? false);
            if (expr2.Type!.IsValueType)
            {
                if (!flag)
                {
                    expr2 = new BoundPassByCopy(expr2.Syntax, expr2, expr2.Type);
                }
                return BoundCall.Synthesized(expr2.Syntax, expr2, objectToStringMethod);
            }
            if (flag)
            {
                return makeConditionalAccess(expr2);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(expr2, out BoundAssignmentOperator store);
            return _factory.Sequence(ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), makeConditionalAccess(boundLocal));
            static bool isFieldOfMarshalByRef(BoundExpression expr, CSharpCompilation compilation)
            {
                if (expr is BoundFieldAccess fieldAccess)
                {
                    return DiagnosticsPass.IsNonAgileFieldAccess(fieldAccess, compilation);
                }
                return false;
            }
            BoundExpression makeConditionalAccess(BoundExpression receiver)
            {
                int id = ++_currentConditionalAccessID;
                return new BoundLoweredConditionalAccess(syntax2, receiver, null, BoundCall.Synthesized(syntax2, new BoundConditionalReceiver(syntax2, id, expr2.Type), objectToStringMethod), null, id, _compilation.GetSpecialType(SpecialType.System_String));
            }
        }

        private BoundExpression RewriteInterpolatedStringConversion(BoundConversion conversion)
        {
            MakeInterpolatedStringFormat((BoundInterpolatedString)conversion.Operand, out var format, out var expressions);
            expressions.Insert(0, format);
            NamedTypeSymbol receiver = _factory.WellKnownType(WellKnownType.System_Runtime_CompilerServices_FormattableStringFactory);
            BoundExpression boundExpression = _factory.StaticCall(receiver, "Create", expressions.ToImmutableAndFree(), allowUnexpandedForm: false);
            if (!boundExpression.HasAnyErrors)
            {
                boundExpression = VisitExpression(boundExpression);
                boundExpression = MakeImplicitConversion(boundExpression, conversion.Type);
            }
            return boundExpression;
        }

        private bool CanLowerToStringConcatenation(BoundInterpolatedString node)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Parts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is BoundStringInsert boundStringInsert))
                {
                    continue;
                }
                if (!_inExpressionLambda && !boundStringInsert.HasErrors)
                {
                    TypeSymbol? type = boundStringInsert.Value.Type;
                    if ((object)type != null && type!.SpecialType == SpecialType.System_String && boundStringInsert.Alignment == null && boundStringInsert.Format == null)
                    {
                        continue;
                    }
                }
                return false;
            }
            return true;
        }

        private void MakeInterpolatedStringFormat(BoundInterpolatedString node, out BoundExpression format, out ArrayBuilder<BoundExpression> expressions)
        {
            _factory.Syntax = node.Syntax;
            int num = node.Parts.Length - 1;
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            expressions = ArrayBuilder<BoundExpression>.GetInstance(num + 1);
            int num2 = 0;
            for (int i = 0; i <= num; i++)
            {
                BoundExpression boundExpression = node.Parts[i];
                if (!(boundExpression is BoundStringInsert boundStringInsert))
                {
                    builder.Append(boundExpression.ConstantValue!.StringValue);
                    continue;
                }
                builder.Append('{').Append(num2++);
                if (boundStringInsert.Alignment != null && !boundStringInsert.Alignment!.HasErrors)
                {
                    builder.Append(',').Append(boundStringInsert.Alignment!.ConstantValue!.Int64Value);
                }
                if (boundStringInsert.Format != null && !boundStringInsert.Format!.HasErrors)
                {
                    builder.Append(':').Append(boundStringInsert.Format!.ConstantValue!.StringValue);
                }
                builder.Append('}');
                BoundExpression boundExpression2 = boundStringInsert.Value;
                TypeSymbol? type = boundExpression2.Type;
                if ((object)type != null && type!.TypeKind == TypeKind.Dynamic)
                {
                    boundExpression2 = MakeConversionNode(boundExpression2, _compilation.ObjectType, @checked: false);
                }
                expressions.Add(boundExpression2);
            }
            format = _factory.StringLiteral(instance.ToStringAndFree());
        }

        public override BoundNode VisitInterpolatedString(BoundInterpolatedString node)
        {
            BoundExpression boundExpression;
            if (CanLowerToStringConcatenation(node))
            {
                int length = node.Parts.Length;
                if (length == 0)
                {
                    return _factory.StringLiteral("");
                }
                boundExpression = null;
                for (int i = 0; i < length; i++)
                {
                    BoundExpression boundExpression2 = node.Parts[i];
                    boundExpression2 = ((!(boundExpression2 is BoundStringInsert boundStringInsert)) ? _factory.StringLiteral(ConstantValueUtils.UnescapeInterpolatedStringLiteral(boundExpression2.ConstantValue!.StringValue)) : boundStringInsert.Value);
                    boundExpression = ((boundExpression == null) ? boundExpression2 : _factory.Binary(BinaryOperatorKind.StringConcatenation, node.Type, boundExpression, boundExpression2));
                }
                if (length == 1)
                {
                    boundExpression = _factory.Coalesce(boundExpression, _factory.StringLiteral(""));
                }
            }
            else
            {
                MakeInterpolatedStringFormat(node, out var format, out var expressions);
                expressions.Insert(0, format);
                TypeSymbol type = node.Type;
                boundExpression = _factory.StaticCall(type, "Format", expressions.ToImmutableAndFree(), allowUnexpandedForm: false);
            }
            if (!boundExpression.HasAnyErrors)
            {
                boundExpression = VisitExpression(boundExpression);
                boundExpression = MakeImplicitConversion(boundExpression, node.Type);
            }
            return boundExpression;
        }

        public override BoundNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            _needsSpilling = true;
            return SwitchExpressionLocalRewriter.Rewrite(this, node);
        }

        public override BoundNode VisitThrowStatement(BoundThrowStatement node)
        {
            BoundStatement boundStatement = (BoundStatement)base.VisitThrowStatement(node);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentThrowStatement(node, boundStatement);
            }
            return boundStatement;
        }

        public override BoundNode VisitTryStatement(BoundTryStatement node)
        {
            BoundBlock boundBlock = (BoundBlock)Visit(node.TryBlock);
            bool sawAwait = _sawAwait;
            _sawAwait = false;
            bool num = _compilation.Options.OptimizationLevel == OptimizationLevel.Release;
            ImmutableArray<BoundCatchBlock> catchBlocks = ((num && !HasSideEffects(boundBlock)) ? ImmutableArray<BoundCatchBlock>.Empty : VisitList(node.CatchBlocks));
            BoundBlock boundBlock2 = (BoundBlock)Visit(node.FinallyBlockOpt);
            _sawAwaitInExceptionHandler |= _sawAwait;
            _sawAwait |= sawAwait;
            if (num && !HasSideEffects(boundBlock2))
            {
                boundBlock2 = null;
            }
            if (!catchBlocks.IsDefaultOrEmpty || boundBlock2 != null)
            {
                return node.Update(boundBlock, catchBlocks, boundBlock2, node.FinallyLabelOpt, node.PreferFaultHandler);
            }
            return boundBlock;
        }

        private static bool HasSideEffects([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] BoundStatement? statement)
        {
            if (statement == null)
            {
                return false;
            }
            switch (statement!.Kind)
            {
                case BoundKind.NoOpStatement:
                    return false;
                case BoundKind.Block:
                    {
                        ImmutableArray<BoundStatement>.Enumerator enumerator = ((BoundBlock)statement).Statements.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (HasSideEffects(enumerator.Current))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case BoundKind.SequencePoint:
                    return HasSideEffects(((BoundSequencePoint)statement).StatementOpt);
                case BoundKind.SequencePointWithSpan:
                    return HasSideEffects(((BoundSequencePointWithSpan)statement).StatementOpt);
                default:
                    return true;
            }
        }

        public override BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            if (node.ExceptionFilterOpt == null)
            {
                return base.VisitCatchBlock(node);
            }
            ConstantValue? constantValue = node.ExceptionFilterOpt!.ConstantValue;
            if ((object)constantValue != null && !constantValue!.BooleanValue)
            {
                return null;
            }
            BoundExpression exceptionSourceOpt = (BoundExpression)Visit(node.ExceptionSourceOpt);
            BoundStatementList exceptionFilterPrologueOpt = (BoundStatementList)Visit(node.ExceptionFilterPrologueOpt);
            BoundExpression boundExpression = (BoundExpression)Visit(node.ExceptionFilterOpt);
            BoundBlock body = (BoundBlock)Visit(node.Body);
            TypeSymbol exceptionTypeOpt = VisitType(node.ExceptionTypeOpt);
            if (boundExpression != null && !node.WasCompilerGenerated && Instrument)
            {
                boundExpression = _instrumenter.InstrumentCatchClauseFilter(node, boundExpression, _factory);
            }
            return node.Update(node.Locals, exceptionSourceOpt, exceptionTypeOpt, exceptionFilterPrologueOpt, boundExpression, body, node.IsSynthesizedAsyncCatchAll);
        }

        public override BoundNode VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            TypeSymbol type = node.Type;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
            BoundExpression left = ReplaceTerminalElementsWithTemps(node.Left, node.Operators, instance, instance2);
            BoundExpression right = ReplaceTerminalElementsWithTemps(node.Right, node.Operators, instance, instance2);
            BoundExpression result = RewriteTupleNestedOperators(node.Operators, left, right, type, instance2, node.OperatorKind);
            return _factory.Sequence(instance2.ToImmutableAndFree(), instance.ToImmutableAndFree(), result);
        }

        private bool IsLikeTupleExpression(BoundExpression expr, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out BoundTupleExpression? tuple)
        {
            if (!(expr is BoundTupleExpression boundTupleExpression))
            {
                if (expr is BoundConversion boundConversion)
                {
                    Conversion conversion = boundConversion.Conversion;
                    ConversionKind kind = conversion.Kind;
                    BoundExpression operand;
                    switch (kind)
                    {
                        case ConversionKind.Identity:
                            {
                                operand = boundConversion.Operand;
                                BoundExpression expr3 = operand;
                                return IsLikeTupleExpression(expr3, out tuple);
                            }
                        case ConversionKind.ImplicitTupleLiteral:
                            {
                                operand = boundConversion.Operand;
                                BoundExpression expr2 = operand;
                                return IsLikeTupleExpression(expr2, out tuple);
                            }
                    }
                    operand = boundConversion.Operand;
                    Conversion conversion2 = conversion;
                    BoundExpression expr4 = operand;
                    BoundConversion boundConversion2 = boundConversion;
                    if (conversion2.IsTupleConversion || conversion2.IsTupleLiteralConversion)
                    {
                        if (!IsLikeTupleExpression(expr4, out tuple))
                        {
                            return false;
                        }
                        ImmutableArray<Conversion> underlyingConversions = conversion2.UnderlyingConversions;
                        ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = boundConversion2.Type.TupleElementTypesWithAnnotations;
                        ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(tuple!.Arguments.Length);
                        for (int i = 0; i < tuple!.Arguments.Length; i++)
                        {
                            BoundExpression operand2 = tuple!.Arguments[i];
                            Conversion conversion3 = underlyingConversions[i];
                            TypeSymbol type = tupleElementTypesWithAnnotations[i].Type;
                            BoundConversion item = new BoundConversion(expr.Syntax, operand2, conversion3, boundConversion2.Checked, boundConversion2.ExplicitCastInCode, null, null, type, boundConversion2.HasErrors);
                            instance.Add(item);
                        }
                        ImmutableArray<BoundExpression> arguments = instance.ToImmutableAndFree();
                        tuple = new BoundConvertedTupleLiteral(tuple!.Syntax, null, wasTargetTyped: true, arguments, ImmutableArray<string>.Empty, ImmutableArray<bool>.Empty, boundConversion2.Type, boundConversion2.HasErrors);
                        return true;
                    }
                    ConversionKind conversionKind = kind;
                    BoundExpression boundExpression = operand;
                    if (conversionKind == ConversionKind.ImplicitNullable || conversionKind == ConversionKind.ExplicitNullable)
                    {
                        TypeSymbol type2 = expr.Type;
                        if ((object)type2 != null && type2.IsNullableType() && type2.StrippedType().Equals(boundExpression.Type, TypeCompareKind.AllIgnoreOptions))
                        {
                            return IsLikeTupleExpression(boundExpression, out tuple);
                        }
                    }
                }
                tuple = null;
                return false;
            }
            BoundTupleExpression boundTupleExpression2 = (tuple = boundTupleExpression);
            return true;
        }

        private BoundExpression PushDownImplicitTupleConversion(BoundExpression expr, ArrayBuilder<BoundExpression> initEffects, ArrayBuilder<LocalSymbol> temps)
        {
            if (expr is BoundConversion boundConversion && boundConversion.ConversionKind == ConversionKind.ImplicitTuple)
            {
                Conversion conversion = boundConversion.Conversion;
                SyntaxNode syntax = boundConversion.Syntax;
                ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = expr.Type!.TupleElementTypesWithAnnotations;
                int length = tupleElementTypesWithAnnotations.Length;
                ImmutableArray<FieldSymbol> tupleElements = boundConversion.Operand.Type!.TupleElements;
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
                BoundExpression tuple = DeferSideEffectingArgumentToTempForTupleEquality(LowerConversions(boundConversion.Operand), initEffects, temps);
                ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
                for (int i = 0; i < length; i++)
                {
                    BoundExpression operand = MakeTupleFieldAccessAndReportUseSiteDiagnostics(tuple, syntax, tupleElements[i]);
                    BoundConversion item = new BoundConversion(syntax, operand, underlyingConversions[i], boundConversion.Checked, boundConversion.ExplicitCastInCode, null, null, tupleElementTypesWithAnnotations[i].Type, boundConversion.HasErrors);
                    instance.Add(item);
                }
                return new BoundConvertedTupleLiteral(syntax, null, wasTargetTyped: true, instance.ToImmutableAndFree(), ImmutableArray<string>.Empty, ImmutableArray<bool>.Empty, expr.Type, expr.HasErrors);
            }
            return expr;
        }

        private BoundExpression ReplaceTerminalElementsWithTemps(BoundExpression expr, TupleBinaryOperatorInfo operators, ArrayBuilder<BoundExpression> initEffects, ArrayBuilder<LocalSymbol> temps)
        {
            if (operators.InfoKind == TupleBinaryOperatorInfoKind.Multiple)
            {
                expr = PushDownImplicitTupleConversion(expr, initEffects, temps);
                if (IsLikeTupleExpression(expr, out var tuple))
                {
                    TupleBinaryOperatorInfo.Multiple multiple = (TupleBinaryOperatorInfo.Multiple)operators;
                    ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(tuple.Arguments.Length);
                    for (int i = 0; i < tuple.Arguments.Length; i++)
                    {
                        BoundExpression expr2 = tuple.Arguments[i];
                        BoundExpression item = ReplaceTerminalElementsWithTemps(expr2, multiple.Operators[i], initEffects, temps);
                        instance.Add(item);
                    }
                    ImmutableArray<BoundExpression> arguments = instance.ToImmutableAndFree();
                    return new BoundConvertedTupleLiteral(tuple.Syntax, null, wasTargetTyped: false, arguments, ImmutableArray<string>.Empty, ImmutableArray<bool>.Empty, tuple.Type, tuple.HasErrors);
                }
            }
            return DeferSideEffectingArgumentToTempForTupleEquality(expr, initEffects, temps);
        }

        private BoundExpression DeferSideEffectingArgumentToTempForTupleEquality(BoundExpression expr, ArrayBuilder<BoundExpression> effects, ArrayBuilder<LocalSymbol> temps, bool enclosingConversionWasExplicit = false)
        {
            if (expr != null)
            {
                if ((object)expr.ConstantValue != null)
                {
                    return VisitExpression(expr);
                }
                if (expr is BoundConversion boundConversion)
                {
                    Conversion conversion = boundConversion.Conversion;
                    ConversionKind kind2 = conversion.Kind;
                    if (kind2 == ConversionKind.DefaultLiteral)
                    {
                        return EvaluateSideEffectingArgumentToTemp(expr, effects, temps);
                    }
                    ConversionKind kind3 = kind2;
                    if (!conversionMustBePerformedOnOriginalExpression(boundConversion, kind3))
                    {
                        if (conversion.IsUserDefined && (boundConversion.ExplicitCastInCode || enclosingConversionWasExplicit))
                        {
                            return EvaluateSideEffectingArgumentToTemp(expr, effects, temps);
                        }
                        BoundConversion boundConversion2 = boundConversion;
                        BoundExpression operand = DeferSideEffectingArgumentToTempForTupleEquality(boundConversion2.Operand, effects, temps, boundConversion2.ExplicitCastInCode || enclosingConversionWasExplicit);
                        return boundConversion2.UpdateOperand(operand);
                    }
                    return EvaluateSideEffectingArgumentToTemp(expr, effects, temps);
                }
                if (expr is BoundObjectCreationExpression boundObjectCreationExpression)
                {
                    switch (boundObjectCreationExpression.Arguments.Length)
                    {
                        case 0:
                            {
                                TypeSymbol type = boundObjectCreationExpression.Type;
                                if ((object)type == null || !type.IsNullableType())
                                {
                                    break;
                                }
                                return new BoundLiteral(expr.Syntax, ConstantValue.Null, expr.Type);
                            }
                        case 1:
                            {
                                TypeSymbol type = boundObjectCreationExpression.Type;
                                if ((object)type != null)
                                {
                                    TypeSymbol type2 = type;
                                    BoundObjectCreationExpression boundObjectCreationExpression2 = boundObjectCreationExpression;
                                    if (type2.IsNullableType())
                                    {
                                        BoundExpression operand2 = DeferSideEffectingArgumentToTempForTupleEquality(boundObjectCreationExpression2.Arguments[0], effects, temps, enclosingConversionWasExplicit: true);
                                        return new BoundConversion(expr.Syntax, operand2, Conversion.MakeNullableConversion(ConversionKind.ImplicitNullable, Conversion.Identity), @checked: false, explicitCastInCode: true, null, null, type2, expr.HasErrors);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return EvaluateSideEffectingArgumentToTemp(expr, effects, temps);
            static bool conversionMustBePerformedOnOriginalExpression(BoundConversion expr, ConversionKind kind)
            {
                switch (kind)
                {
                    case ConversionKind.AnonymousFunction:
                    case ConversionKind.MethodGroup:
                    case ConversionKind.InterpolatedString:
                    case ConversionKind.SwitchExpression:
                    case ConversionKind.ConditionalExpression:
                    case ConversionKind.StackAllocToPointerType:
                    case ConversionKind.StackAllocToSpanType:
                    case ConversionKind.ObjectCreation:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private BoundExpression RewriteTupleOperator(TupleBinaryOperatorInfo @operator, BoundExpression left, BoundExpression right, TypeSymbol boolType, ArrayBuilder<LocalSymbol> temps, BinaryOperatorKind operatorKind)
        {
            switch (@operator.InfoKind)
            {
                case TupleBinaryOperatorInfoKind.Multiple:
                    return RewriteTupleNestedOperators((TupleBinaryOperatorInfo.Multiple)@operator, left, right, boolType, temps, operatorKind);
                case TupleBinaryOperatorInfoKind.Single:
                    return RewriteTupleSingleOperator((TupleBinaryOperatorInfo.Single)@operator, left, right, boolType, operatorKind);
                case TupleBinaryOperatorInfoKind.NullNull:
                    {
                        TupleBinaryOperatorInfo.NullNull nullNull = (TupleBinaryOperatorInfo.NullNull)@operator;
                        return new BoundLiteral(left.Syntax, ConstantValue.Create(nullNull.Kind == BinaryOperatorKind.Equal), boolType);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(@operator.InfoKind);
            }
        }

        private BoundExpression RewriteTupleNestedOperators(TupleBinaryOperatorInfo.Multiple operators, BoundExpression left, BoundExpression right, TypeSymbol boolType, ArrayBuilder<LocalSymbol> temps, BinaryOperatorKind operatorKind)
        {
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            MakeNullableParts(left, temps, instance2, instance, saveHasValue: true, out var hasValue, out var value, out var isNullable);
            MakeNullableParts(right, temps, instance2, instance, saveHasValue: false, out var hasValue2, out var value2, out var isNullable2);
            BoundExpression result = RewriteNonNullableNestedTupleOperators(operators, value, value2, boolType, temps, instance2, operatorKind);
            BoundExpression boundExpression = _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, instance2.ToImmutableAndFree(), result);
            if (!isNullable && !isNullable2)
            {
                return boundExpression;
            }
            bool flag = operatorKind == BinaryOperatorKind.Equal;
            if (hasValue2.ConstantValue == ConstantValue.False)
            {
                return _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree(), flag ? _factory.Not(hasValue) : hasValue);
            }
            if (hasValue.ConstantValue == ConstantValue.False)
            {
                return _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree(), flag ? _factory.Not(hasValue2) : hasValue2);
            }
            return _factory.Sequence(ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree(), _factory.Conditional(_factory.Binary(BinaryOperatorKind.Equal, boolType, hasValue, hasValue2), _factory.Conditional(hasValue, boundExpression, MakeBooleanConstant(right.Syntax, flag), boolType), MakeBooleanConstant(right.Syntax, !flag), boolType));
        }

        private void MakeNullableParts(BoundExpression expr, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> innerEffects, ArrayBuilder<BoundExpression> outerEffects, bool saveHasValue, out BoundExpression hasValue, out BoundExpression value, out bool isNullable)
        {
            isNullable = !(expr is BoundTupleExpression) && (object)expr.Type != null && expr.Type.IsNullableType();
            if (!isNullable)
            {
                hasValue = MakeBooleanConstant(expr.Syntax, value: true);
                expr = PushDownImplicitTupleConversion(expr, innerEffects, temps);
                value = expr;
                return;
            }
            if (NullableNeverHasValue(expr))
            {
                hasValue = MakeBooleanConstant(expr.Syntax, value: false);
                value = new BoundDefaultExpression(expr.Syntax, expr.Type.StrippedType());
                return;
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(expr);
            if (boundExpression != null)
            {
                hasValue = MakeBooleanConstant(expr.Syntax, value: true);
                value = PushDownImplicitTupleConversion(boundExpression, innerEffects, temps);
                value = LowerConversions(value);
                isNullable = false;
                return;
            }
            hasValue = makeNullableHasValue(expr);
            if (saveHasValue)
            {
                hasValue = MakeTemp(hasValue, temps, outerEffects);
            }
            value = MakeValueOrDefaultTemp(expr, temps, innerEffects);
            BoundExpression makeNullableHasValue(BoundExpression expr)
            {
                if (expr is BoundConversion boundConversion)
                {
                    Conversion conversion = boundConversion.Conversion;
                    if (conversion.IsIdentity)
                    {
                        BoundExpression operand = boundConversion.Operand;
                        BoundExpression expr2 = operand;
                        return makeNullableHasValue(expr2);
                    }
                    if (conversion.IsNullable)
                    {
                        ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
                        BoundExpression operand = boundConversion.Operand;
                        ImmutableArray<Conversion> immutableArray = underlyingConversions;
                        BoundExpression boundExpression2 = operand;
                        if (expr.Type.IsNullableType() && (object)boundExpression2.Type != null && boundExpression2.Type.IsNullableType() && !immutableArray[0].IsUserDefined)
                        {
                            return makeNullableHasValue(boundExpression2);
                        }
                    }
                }
                return MakeNullableHasValue(expr.Syntax, expr);
            }
        }

        private BoundLocal MakeTemp(BoundExpression loweredExpression, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects)
        {
            BoundLocal boundLocal = _factory.StoreToTemp(loweredExpression, out BoundAssignmentOperator store);
            effects.Add(store);
            temps.Add(boundLocal.LocalSymbol);
            return boundLocal;
        }

        private BoundExpression MakeValueOrDefaultTemp(BoundExpression expr, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects)
        {
            if (expr is BoundConversion boundConversion)
            {
                Conversion conversion2 = boundConversion.Conversion;
                if (conversion2.IsIdentity)
                {
                    BoundExpression operand = boundConversion.Operand;
                    BoundExpression expr2 = operand;
                    return MakeValueOrDefaultTemp(expr2, temps, effects);
                }
                if (conversion2.IsNullable)
                {
                    ImmutableArray<Conversion> underlyingConversions = conversion2.UnderlyingConversions;
                    BoundExpression operand = boundConversion.Operand;
                    ImmutableArray<Conversion> immutableArray = underlyingConversions;
                    BoundExpression boundExpression = operand;
                    BoundConversion enclosing2 = boundConversion;
                    TypeSymbol type2 = expr.Type;
                    if ((object)type2 != null && type2.IsNullableType() && (object)boundExpression.Type != null && boundExpression.Type.IsNullableType())
                    {
                        Conversion conversion3 = immutableArray[0];
                        if (conversion3.IsTupleConversion)
                        {
                            BoundExpression boundExpression2 = MakeValueOrDefaultTemp(boundExpression, temps, effects);
                            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = expr.Type.GetNullableUnderlyingType().TupleElementTypesWithAnnotations;
                            int length = boundExpression2.Type!.TupleElementTypesWithAnnotations.Length;
                            ImmutableArray<Conversion> underlyingConversions2 = conversion3.UnderlyingConversions;
                            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
                            for (int i = 0; i < length; i++)
                            {
                                instance.Add(MakeBoundConversion(GetTuplePart(boundExpression2, i), underlyingConversions2[i], tupleElementTypesWithAnnotations[i], enclosing2));
                            }
                            return new BoundConvertedTupleLiteral(boundExpression2.Syntax, null, wasTargetTyped: false, instance.ToImmutableAndFree(), ImmutableArray<string>.Empty, ImmutableArray<bool>.Empty, expr.Type, expr.HasErrors).WithSuppression(expr.IsSuppressed);
                        }
                    }
                }
            }
            BoundExpression loweredExpression = MakeOptimizedGetValueOrDefault(expr.Syntax, expr);
            return MakeTemp(loweredExpression, temps, effects);
            static BoundExpression MakeBoundConversion(BoundExpression expr, Conversion conversion, TypeWithAnnotations type, BoundConversion enclosing)
            {
                return new BoundConversion(expr.Syntax, expr, conversion, enclosing.Checked, enclosing.ExplicitCastInCode, null, null, type.Type);
            }
        }

        private BoundExpression RewriteNonNullableNestedTupleOperators(TupleBinaryOperatorInfo.Multiple operators, BoundExpression left, BoundExpression right, TypeSymbol type, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> effects, BinaryOperatorKind operatorKind)
        {
            ImmutableArray<TupleBinaryOperatorInfo> operators2 = operators.Operators;
            BoundExpression boundExpression = null;
            for (int i = 0; i < operators2.Length; i++)
            {
                BoundExpression tuplePart = GetTuplePart(left, i);
                BoundExpression tuplePart2 = GetTuplePart(right, i);
                BoundExpression boundExpression2 = RewriteTupleOperator(operators2[i], tuplePart, tuplePart2, type, temps, operatorKind);
                if (boundExpression == null)
                {
                    boundExpression = boundExpression2;
                    continue;
                }
                BinaryOperatorKind kind = ((operatorKind == BinaryOperatorKind.Equal) ? BinaryOperatorKind.LogicalBoolAnd : BinaryOperatorKind.LogicalBoolOr);
                boundExpression = _factory.Binary(kind, type, boundExpression, boundExpression2);
            }
            return boundExpression;
        }

        private BoundExpression GetTuplePart(BoundExpression tuple, int i)
        {
            if (tuple is BoundTupleExpression boundTupleExpression)
            {
                return boundTupleExpression.Arguments[i];
            }
            return MakeTupleFieldAccessAndReportUseSiteDiagnostics(tuple, tuple.Syntax, tuple.Type!.TupleElements[i]);
        }

        private BoundExpression RewriteTupleSingleOperator(TupleBinaryOperatorInfo.Single single, BoundExpression left, BoundExpression right, TypeSymbol boolType, BinaryOperatorKind operatorKind)
        {
            left = LowerConversions(left);
            right = LowerConversions(right);
            if (single.Kind.IsDynamic())
            {
                BoundExpression loweredOperand = _dynamicFactory.MakeDynamicBinaryOperator(single.Kind, left, right, isCompoundAssignment: false, _compilation.DynamicType).ToExpression();
                if (operatorKind == BinaryOperatorKind.Equal)
                {
                    return _factory.Not(MakeUnaryOperator(UnaryOperatorKind.DynamicFalse, left.Syntax, null, loweredOperand, boolType));
                }
                return MakeUnaryOperator(UnaryOperatorKind.DynamicTrue, left.Syntax, null, loweredOperand, boolType);
            }
            if (left.IsLiteralNull() && right.IsLiteralNull())
            {
                return new BoundLiteral(left.Syntax, ConstantValue.Create(operatorKind == BinaryOperatorKind.Equal), boolType);
            }
            BoundExpression boundExpression = MakeBinaryOperator(_factory.Syntax, single.Kind, left, right, single.MethodSymbolOpt?.ReturnType ?? boolType, single.MethodSymbolOpt);
            UnaryOperatorSignature boolOperator = single.BoolOperator;
            Conversion conversionForBool = single.ConversionForBool;
            BoundExpression boundExpression2;
            if (boolOperator.Kind == UnaryOperatorKind.Error)
            {
                boundExpression2 = (conversionForBool.IsIdentity ? boundExpression : MakeConversionNode(_factory.Syntax, boundExpression, conversionForBool, boolType, @checked: false));
            }
            else
            {
                BoundExpression loweredOperand2 = MakeConversionNode(_factory.Syntax, boundExpression, conversionForBool, boolOperator.OperandType, @checked: false);
                boundExpression2 = MakeUnaryOperator(boolOperator.Kind, boundExpression.Syntax, boolOperator.Method, loweredOperand2, boolType);
                if (operatorKind == BinaryOperatorKind.Equal)
                {
                    boundExpression2 = _factory.Not(boundExpression2);
                }
            }
            return boundExpression2;
        }

        private BoundExpression LowerConversions(BoundExpression expr)
        {
            if (!(expr is BoundConversion boundConversion))
            {
                return expr;
            }
            return MakeConversionNode(boundConversion, boundConversion.Syntax, LowerConversions(boundConversion.Operand), boundConversion.Conversion, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValue, boundConversion.Type);
        }

        public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            return VisitTupleExpression(node);
        }

        private BoundNode VisitTupleExpression(BoundTupleExpression node)
        {
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
            return RewriteTupleCreationExpression(node, rewrittenArguments);
        }

        private BoundExpression RewriteTupleCreationExpression(BoundTupleExpression node, ImmutableArray<BoundExpression> rewrittenArguments)
        {
            return MakeTupleCreationExpression(node.Syntax, (NamedTypeSymbol)node.Type, rewrittenArguments);
        }

        private BoundExpression MakeTupleCreationExpression(SyntaxNode syntax, NamedTypeSymbol type, ImmutableArray<BoundExpression> rewrittenArguments)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            NamedTypeSymbol.GetUnderlyingTypeChain(type, instance);
            try
            {
                NamedTypeSymbol namedTypeSymbol = instance.Pop();
                ImmutableArray<BoundExpression> arguments = ImmutableArray.Create(rewrittenArguments, instance.Count * 7, namedTypeSymbol.Arity);
                MethodSymbol methodSymbol = (MethodSymbol)NamedTypeSymbol.GetWellKnownMemberInType(namedTypeSymbol.OriginalDefinition, NamedTypeSymbol.GetTupleCtor(namedTypeSymbol.Arity), _diagnostics, syntax);
                if ((object)methodSymbol == null)
                {
                    return _factory.BadExpression(type);
                }
                MethodSymbol constructor = methodSymbol.AsMember(namedTypeSymbol);
                BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(syntax, constructor, arguments);
                if (instance.Count > 0)
                {
                    MethodSymbol methodSymbol2 = (MethodSymbol)NamedTypeSymbol.GetWellKnownMemberInType(instance.Peek().OriginalDefinition, NamedTypeSymbol.GetTupleCtor(8), _diagnostics, syntax);
                    if ((object)methodSymbol2 == null)
                    {
                        return _factory.BadExpression(type);
                    }
                    do
                    {
                        ImmutableArray<BoundExpression> arguments2 = ImmutableArray.Create(rewrittenArguments, (instance.Count - 1) * 7, 7).Add(boundObjectCreationExpression);
                        MethodSymbol constructor2 = methodSymbol2.AsMember(instance.Pop());
                        boundObjectCreationExpression = new BoundObjectCreationExpression(syntax, constructor2, arguments2);
                    }
                    while (instance.Count > 0);
                }
                return boundObjectCreationExpression.Update(boundObjectCreationExpression.Constructor, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.ArgumentNamesOpt, boundObjectCreationExpression.ArgumentRefKindsOpt, boundObjectCreationExpression.Expanded, boundObjectCreationExpression.ArgsToParamsOpt, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.ConstantValue, boundObjectCreationExpression.InitializerExpressionOpt, type);
            }
            finally
            {
                instance.Free();
            }
        }

        public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
        {
            switch (node.OperatorKind.Operator())
            {
                case UnaryOperatorKind.PostfixIncrement:
                case UnaryOperatorKind.PostfixDecrement:
                case UnaryOperatorKind.PrefixIncrement:
                case UnaryOperatorKind.PrefixDecrement:
                    return base.VisitUnaryOperator(node);
                default:
                    {
                        if (node.Operand.Kind == BoundKind.BinaryOperator)
                        {
                            BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)node.Operand;
                            if ((node.OperatorKind == UnaryOperatorKind.DynamicTrue && boundBinaryOperator.OperatorKind == BinaryOperatorKind.DynamicLogicalOr) || (node.OperatorKind == UnaryOperatorKind.DynamicFalse && boundBinaryOperator.OperatorKind == BinaryOperatorKind.DynamicLogicalAnd))
                            {
                                return VisitBinaryOperator(boundBinaryOperator, node);
                            }
                        }
                        BoundExpression loweredOperand = VisitExpression(node.Operand);
                        return MakeUnaryOperator(node, node.OperatorKind, node.Syntax, node.MethodOpt, loweredOperand, node.Type);
                    }
            }
        }

        private BoundExpression MakeUnaryOperator(UnaryOperatorKind kind, SyntaxNode syntax, MethodSymbol? method, BoundExpression loweredOperand, TypeSymbol type)
        {
            return MakeUnaryOperator(null, kind, syntax, method, loweredOperand, type);
        }

        private BoundExpression MakeUnaryOperator(BoundUnaryOperator? oldNode, UnaryOperatorKind kind, SyntaxNode syntax, MethodSymbol? method, BoundExpression loweredOperand, TypeSymbol type)
        {
            if (kind.IsDynamic())
            {
                ConstantValue constantValue = UnboxConstant(loweredOperand);
                if (constantValue == ConstantValue.True || constantValue == ConstantValue.False)
                {
                    switch (kind)
                    {
                        case UnaryOperatorKind.DynamicTrue:
                            return _factory.Literal(constantValue.BooleanValue);
                        case UnaryOperatorKind.DynamicLogicalNegation:
                            return MakeConversionNode(_factory.Literal(!constantValue.BooleanValue), type, @checked: false);
                    }
                }
                return _dynamicFactory.MakeDynamicUnaryOperator(kind, loweredOperand, type).ToExpression();
            }
            if (kind.IsLifted())
            {
                if (!_inExpressionLambda)
                {
                    return LowerLiftedUnaryOperator(kind, syntax, method, loweredOperand, type);
                }
            }
            else if (kind.IsUserDefined())
            {
                if (!_inExpressionLambda || kind == UnaryOperatorKind.UserDefinedTrue || kind == UnaryOperatorKind.UserDefinedFalse)
                {
                    return BoundCall.Synthesized(syntax, null, method, loweredOperand);
                }
            }
            else if (kind.Operator() == UnaryOperatorKind.UnaryPlus)
            {
                return loweredOperand;
            }
            switch (kind)
            {
                case UnaryOperatorKind.EnumBitwiseComplement:
                    {
                        NamedTypeSymbol enumUnderlyingType = loweredOperand.Type.GetEnumUnderlyingType();
                        SpecialType enumPromotedType = Binder.GetEnumPromotedType(enumUnderlyingType.SpecialType);
                        NamedTypeSymbol namedTypeSymbol = ((enumPromotedType == enumUnderlyingType.SpecialType) ? enumUnderlyingType : _compilation.GetSpecialType(enumPromotedType));
                        BoundExpression boundExpression = MakeConversionNode(loweredOperand, namedTypeSymbol, @checked: false);
                        UnaryOperatorKind operatorKind = kind.Operator().WithType(enumPromotedType);
                        BoundUnaryOperator boundUnaryOperator = ((oldNode != null) ? oldNode!.Update(operatorKind, boundExpression, oldNode!.ConstantValueOpt, method, boundExpression.ResultKind, namedTypeSymbol) : new BoundUnaryOperator(syntax, operatorKind, boundExpression, null, method, LookupResultKind.Viable, namedTypeSymbol));
                        return MakeConversionNode(boundUnaryOperator.Syntax, boundUnaryOperator, Conversion.ExplicitEnumeration, type, @checked: false);
                    }
                case UnaryOperatorKind.DecimalUnaryMinus:
                    method = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(SpecialMember.System_Decimal__op_UnaryNegation);
                    if (!_inExpressionLambda)
                    {
                        return BoundCall.Synthesized(syntax, null, method, loweredOperand);
                    }
                    break;
            }
            if (oldNode == null)
            {
                return new BoundUnaryOperator(syntax, kind, loweredOperand, null, method, LookupResultKind.Viable, type);
            }
            return oldNode!.Update(kind, loweredOperand, oldNode!.ConstantValueOpt, method, oldNode!.ResultKind, type);
        }

        private BoundExpression LowerLiftedUnaryOperator(UnaryOperatorKind kind, SyntaxNode syntax, MethodSymbol? method, BoundExpression loweredOperand, TypeSymbol type)
        {
            BoundExpression boundExpression = OptimizeLiftedUnaryOperator(kind, syntax, method, loweredOperand, type);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            BoundLocal boundLocal = _factory.StoreToTemp(loweredOperand, out BoundAssignmentOperator store);
            MethodSymbol method2 = UnsafeGetNullableMethod(syntax, boundLocal.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            BoundExpression rewrittenCondition = MakeNullableHasValue(syntax, boundLocal);
            BoundExpression nonNullOperand = BoundCall.Synthesized(syntax, boundLocal, method2);
            BoundExpression liftedUnaryOperatorConsequence = GetLiftedUnaryOperatorConsequence(kind, syntax, method, type, nonNullOperand);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, type);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, liftedUnaryOperatorConsequence, rewrittenAlternative, null, type, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, type);
        }

        private BoundExpression? OptimizeLiftedUnaryOperator(UnaryOperatorKind operatorKind, SyntaxNode syntax, MethodSymbol? method, BoundExpression loweredOperand, TypeSymbol type)
        {
            if (NullableNeverHasValue(loweredOperand))
            {
                return new BoundDefaultExpression(syntax, type);
            }
            BoundExpression boundExpression = NullableAlwaysHasValue(loweredOperand);
            if (boundExpression != null)
            {
                return GetLiftedUnaryOperatorConsequence(operatorKind, syntax, method, type, boundExpression);
            }
            if (loweredOperand is BoundLoweredConditionalAccess boundLoweredConditionalAccess && (boundLoweredConditionalAccess.WhenNullOpt == null || boundLoweredConditionalAccess.WhenNullOpt.IsDefaultValue()))
            {
                BoundExpression boundExpression2 = LowerLiftedUnaryOperator(operatorKind, syntax, method, boundLoweredConditionalAccess.WhenNotNull, type);
                return boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.Receiver, boundLoweredConditionalAccess.HasValueMethodOpt, boundExpression2, null, boundLoweredConditionalAccess.Id, boundExpression2.Type);
            }
            if (loweredOperand.Kind == BoundKind.Sequence)
            {
                BoundSequence boundSequence = (BoundSequence)loweredOperand;
                if (boundSequence.Value.Kind == BoundKind.ConditionalOperator)
                {
                    BoundConditionalOperator boundConditionalOperator = (BoundConditionalOperator)boundSequence.Value;
                    if (NullableAlwaysHasValue(boundConditionalOperator.Consequence) != null && NullableNeverHasValue(boundConditionalOperator.Alternative))
                    {
                        return new BoundSequence(syntax, boundSequence.Locals, boundSequence.SideEffects, RewriteConditionalOperator(syntax, boundConditionalOperator.Condition, MakeUnaryOperator(operatorKind, syntax, method, boundConditionalOperator.Consequence, type), MakeUnaryOperator(operatorKind, syntax, method, boundConditionalOperator.Alternative, type), null, type, isRef: false), type);
                    }
                }
            }
            return null;
        }

        private BoundExpression GetLiftedUnaryOperatorConsequence(UnaryOperatorKind kind, SyntaxNode syntax, MethodSymbol? method, TypeSymbol type, BoundExpression nonNullOperand)
        {
            MethodSymbol constructor = UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor);
            BoundExpression boundExpression = MakeUnaryOperator(null, kind.Unlifted(), syntax, method, nonNullOperand, type.GetNullableUnderlyingType());
            return new BoundObjectCreationExpression(syntax, constructor, boundExpression);
        }

        private static bool IsIncrement(BoundIncrementOperator node)
        {
            UnaryOperatorKind unaryOperatorKind = node.OperatorKind.Operator();
            if (unaryOperatorKind != UnaryOperatorKind.PostfixIncrement)
            {
                return unaryOperatorKind == UnaryOperatorKind.PrefixIncrement;
            }
            return true;
        }

        private static bool IsPrefix(BoundIncrementOperator node)
        {
            UnaryOperatorKind unaryOperatorKind = node.OperatorKind.Operator();
            if (unaryOperatorKind != UnaryOperatorKind.PrefixIncrement)
            {
                return unaryOperatorKind == UnaryOperatorKind.PrefixDecrement;
            }
            return true;
        }

        public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
        {
            bool flag = IsPrefix(node);
            bool isDynamicAssignment = node.OperatorKind.IsDynamic();
            bool isChecked = node.OperatorKind.IsChecked();
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
            SyntaxNode syntax = node.Syntax;
            BoundExpression boundExpression = TransformCompoundAssignmentLHS(node.Operand, instance2, instance, isDynamicAssignment);
            TypeSymbol type = boundExpression.Type;
            LocalSymbol localSymbol = _factory.SynthesizedLocal(type);
            instance.Add(localSymbol);
            BoundExpression boundExpression2 = new BoundLocal(syntax, localSymbol, null, type);
            BoundExpression newValue = MakeIncrementOperator(node, flag ? MakeRValue(boundExpression) : boundExpression2);
            if (IsIndirectOrInstanceField(boundExpression))
            {
                return RewriteWithRefOperand(flag, isChecked, instance, instance2, syntax, boundExpression, type, boundExpression2, newValue);
            }
            return RewriteWithNotRefOperand(flag, isChecked, instance, instance2, syntax, boundExpression, type, boundExpression2, newValue);
        }

        private static bool IsIndirectOrInstanceField(BoundExpression expression)
        {
            return expression.Kind switch
            {
                BoundKind.Local => ((BoundLocal)expression).LocalSymbol.RefKind != RefKind.None,
                BoundKind.Parameter => ((BoundParameter)expression).ParameterSymbol.RefKind != RefKind.None,
                BoundKind.FieldAccess => !((BoundFieldAccess)expression).FieldSymbol.IsStatic,
                _ => false,
            };
        }

        private BoundNode RewriteWithNotRefOperand(bool isPrefix, bool isChecked, ArrayBuilder<LocalSymbol> tempSymbols, ArrayBuilder<BoundExpression> tempInitializers, SyntaxNode syntax, BoundExpression transformedLHS, TypeSymbol operandType, BoundExpression boundTemp, BoundExpression newValue)
        {
            ImmutableArray<BoundExpression> second = ImmutableArray.Create(MakeAssignmentOperator(syntax, boundTemp, isPrefix ? newValue : MakeRValue(transformedLHS), operandType, used: false, isChecked, isCompoundAssignment: false), MakeAssignmentOperator(syntax, transformedLHS, isPrefix ? boundTemp : newValue, operandType, used: false, isChecked, isCompoundAssignment: false));
            return new BoundSequence(syntax, tempSymbols.ToImmutableAndFree(), tempInitializers.ToImmutableAndFree().Concat(second), boundTemp, operandType);
        }

        private BoundNode RewriteWithRefOperand(bool isPrefix, bool isChecked, ArrayBuilder<LocalSymbol> tempSymbols, ArrayBuilder<BoundExpression> tempInitializers, SyntaxNode syntax, BoundExpression operand, TypeSymbol operandType, BoundExpression boundTemp, BoundExpression newValue)
        {
            BoundExpression boundExpression = (isPrefix ? newValue : MakeRValue(operand));
            BoundExpression item = MakeAssignmentOperator(syntax, boundTemp, boundExpression, operandType, used: false, isChecked, isCompoundAssignment: false);
            BoundExpression value = (isPrefix ? boundTemp : newValue);
            BoundSequence rewrittenRight = new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item), value, boundExpression.Type);
            BoundExpression item2 = MakeAssignmentOperator(syntax, operand, rewrittenRight, operandType, used: false, isChecked, isCompoundAssignment: false);
            tempInitializers.Add(item2);
            return new BoundSequence(syntax, tempSymbols.ToImmutableAndFree(), tempInitializers.ToImmutableAndFree(), boundTemp, operandType);
        }

        private BoundExpression MakeIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            if (node.OperatorKind.IsDynamic())
            {
                return _dynamicFactory.MakeDynamicUnaryOperator(node.OperatorKind, rewrittenValueToIncrement, node.Type).ToExpression();
            }
            BoundExpression boundExpression = ((node.OperatorKind.OperandTypes() != UnaryOperatorKind.UserDefined) ? MakeBuiltInIncrementOperator(node, rewrittenValueToIncrement) : MakeUserDefinedIncrementOperator(node, rewrittenValueToIncrement));
            if (!node.ResultConversion.IsIdentity)
            {
                boundExpression = MakeConversionNode(node.Syntax, boundExpression, node.ResultConversion, node.Type, node.OperatorKind.IsChecked());
            }
            return boundExpression;
        }

        private BoundExpression MakeUserDefinedIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            bool flag = node.OperatorKind.IsLifted();
            bool @checked = node.OperatorKind.IsChecked();
            BoundExpression boundExpression = rewrittenValueToIncrement;
            SyntaxNode syntax = node.Syntax;
            TypeSymbol typeSymbol = node.MethodOpt!.GetParameterType(0);
            if (flag)
            {
                typeSymbol = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(typeSymbol);
            }
            if (!node.OperandConversion.IsIdentity)
            {
                boundExpression = MakeConversionNode(syntax, rewrittenValueToIncrement, node.OperandConversion, typeSymbol, @checked);
            }
            if (!flag)
            {
                return BoundCall.Synthesized(syntax, null, node.MethodOpt, boundExpression);
            }
            BoundLocal boundLocal = _factory.StoreToTemp(boundExpression, out BoundAssignmentOperator store);
            MethodSymbol method = UnsafeGetNullableMethod(syntax, typeSymbol, SpecialMember.System_Nullable_T_GetValueOrDefault);
            MethodSymbol constructor = UnsafeGetNullableMethod(syntax, typeSymbol, SpecialMember.System_Nullable_T__ctor);
            BoundExpression rewrittenCondition = MakeNullableHasValue(node.Syntax, boundLocal);
            BoundExpression arg = BoundCall.Synthesized(syntax, boundLocal, method);
            BoundExpression boundExpression2 = BoundCall.Synthesized(syntax, null, node.MethodOpt, arg);
            BoundExpression rewrittenConsequence = new BoundObjectCreationExpression(syntax, constructor, boundExpression2);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, typeSymbol);
            BoundExpression value = RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, typeSymbol, isRef: false);
            return new BoundSequence(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, typeSymbol);
        }

        private BoundExpression MakeBuiltInIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            TypeSymbol unaryOperatorType = GetUnaryOperatorType(node);
            BinaryOperatorKind correspondingBinaryOperator = GetCorrespondingBinaryOperator(node);
            correspondingBinaryOperator |= (IsIncrement(node) ? BinaryOperatorKind.Addition : BinaryOperatorKind.Subtraction);
            (TypeSymbol, ConstantValue) constantOneForIncrement = GetConstantOneForIncrement(_compilation, correspondingBinaryOperator);
            TypeSymbol typeSymbol = constantOneForIncrement.Item1;
            ConstantValue item = constantOneForIncrement.Item2;
            BoundExpression boundExpression = MakeLiteral(node.Syntax, item, typeSymbol);
            if (correspondingBinaryOperator.IsLifted())
            {
                typeSymbol = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(typeSymbol);
                MethodSymbol constructor = UnsafeGetNullableMethod(node.Syntax, typeSymbol, SpecialMember.System_Nullable_T__ctor);
                boundExpression = new BoundObjectCreationExpression(node.Syntax, constructor, boundExpression);
            }
            BoundExpression boundExpression2 = rewrittenValueToIncrement;
            bool @checked = node.OperatorKind.IsChecked();
            if (!node.OperandConversion.IsIdentity)
            {
                boundExpression2 = MakeConversionNode(node.Syntax, boundExpression2, node.OperandConversion, unaryOperatorType, @checked);
            }
            if (node.OperatorKind.OperandTypes() == UnaryOperatorKind.Pointer)
            {
                return MakeBinaryOperator(node.Syntax, correspondingBinaryOperator, boundExpression2, boundExpression, boundExpression2.Type, null);
            }
            boundExpression2 = MakeConversionNode(boundExpression2, typeSymbol, @checked);
            BoundExpression rewrittenOperand = ((unaryOperatorType.SpecialType == SpecialType.System_Decimal) ? MakeDecimalIncDecOperator(node.Syntax, correspondingBinaryOperator, boundExpression2) : ((!unaryOperatorType.IsNullableType() || unaryOperatorType.GetNullableUnderlyingType().SpecialType != SpecialType.System_Decimal) ? MakeBinaryOperator(node.Syntax, correspondingBinaryOperator, boundExpression2, boundExpression, typeSymbol, null) : MakeLiftedDecimalIncDecOperator(node.Syntax, correspondingBinaryOperator, boundExpression2)));
            return MakeConversionNode(rewrittenOperand, unaryOperatorType, @checked);
        }

        private MethodSymbol GetDecimalIncDecOperator(BinaryOperatorKind oper)
        {
            SpecialMember member = oper.Operator() switch
            {
                BinaryOperatorKind.Addition => SpecialMember.System_Decimal__op_Increment,
                BinaryOperatorKind.Subtraction => SpecialMember.System_Decimal__op_Decrement,
                _ => throw ExceptionUtilities.UnexpectedValue(oper.Operator()),
            };
            return (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(member);
        }

        private BoundExpression MakeDecimalIncDecOperator(SyntaxNode syntax, BinaryOperatorKind oper, BoundExpression operand)
        {
            MethodSymbol decimalIncDecOperator = GetDecimalIncDecOperator(oper);
            return BoundCall.Synthesized(syntax, null, decimalIncDecOperator, operand);
        }

        private BoundExpression MakeLiftedDecimalIncDecOperator(SyntaxNode syntax, BinaryOperatorKind oper, BoundExpression operand)
        {
            MethodSymbol decimalIncDecOperator = GetDecimalIncDecOperator(oper);
            MethodSymbol method = UnsafeGetNullableMethod(syntax, operand.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            MethodSymbol constructor = UnsafeGetNullableMethod(syntax, operand.Type, SpecialMember.System_Nullable_T__ctor);
            BoundExpression rewrittenCondition = MakeNullableHasValue(syntax, operand);
            BoundExpression arg = BoundCall.Synthesized(syntax, operand, method);
            BoundExpression boundExpression = BoundCall.Synthesized(syntax, null, decimalIncDecOperator, arg);
            BoundExpression rewrittenConsequence = new BoundObjectCreationExpression(syntax, constructor, boundExpression);
            BoundExpression rewrittenAlternative = new BoundDefaultExpression(syntax, operand.Type);
            return RewriteConditionalOperator(syntax, rewrittenCondition, rewrittenConsequence, rewrittenAlternative, null, operand.Type, isRef: false);
        }

        private BoundExpression MakeRValue(BoundExpression transformedExpression)
        {
            switch (transformedExpression.Kind)
            {
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)transformedExpression;
                        return MakePropertyGetAccess(transformedExpression.Syntax, boundPropertyAccess.ReceiverOpt, boundPropertyAccess.PropertySymbol, boundPropertyAccess);
                    }
                case BoundKind.DynamicMemberAccess:
                    {
                        BoundDynamicMemberAccess boundDynamicMemberAccess = (BoundDynamicMemberAccess)transformedExpression;
                        return _dynamicFactory.MakeDynamicGetMember(boundDynamicMemberAccess.Receiver, boundDynamicMemberAccess.Name, resultIndexed: false).ToExpression();
                    }
                case BoundKind.IndexerAccess:
                    {
                        BoundIndexerAccess boundIndexerAccess = (BoundIndexerAccess)transformedExpression;
                        return MakePropertyGetAccess(transformedExpression.Syntax, boundIndexerAccess.ReceiverOpt, boundIndexerAccess.Indexer, boundIndexerAccess.Arguments);
                    }
                case BoundKind.DynamicIndexerAccess:
                    {
                        BoundDynamicIndexerAccess boundDynamicIndexerAccess = (BoundDynamicIndexerAccess)transformedExpression;
                        return MakeDynamicGetIndex(boundDynamicIndexerAccess, boundDynamicIndexerAccess.Receiver, boundDynamicIndexerAccess.Arguments, boundDynamicIndexerAccess.ArgumentNamesOpt, boundDynamicIndexerAccess.ArgumentRefKindsOpt);
                    }
                default:
                    return transformedExpression;
            }
        }

        private TypeSymbol GetUnaryOperatorType(BoundIncrementOperator node)
        {
            UnaryOperatorKind unaryOperatorKind = node.OperatorKind.OperandTypes();
            SpecialType specialType;
            switch (unaryOperatorKind)
            {
                case UnaryOperatorKind.Enum:
                    return node.Type;
                case UnaryOperatorKind.Int:
                    specialType = SpecialType.System_Int32;
                    break;
                case UnaryOperatorKind.SByte:
                    specialType = SpecialType.System_SByte;
                    break;
                case UnaryOperatorKind.Short:
                    specialType = SpecialType.System_Int16;
                    break;
                case UnaryOperatorKind.Byte:
                    specialType = SpecialType.System_Byte;
                    break;
                case UnaryOperatorKind.UShort:
                    specialType = SpecialType.System_UInt16;
                    break;
                case UnaryOperatorKind.Char:
                    specialType = SpecialType.System_Char;
                    break;
                case UnaryOperatorKind.UInt:
                    specialType = SpecialType.System_UInt32;
                    break;
                case UnaryOperatorKind.Long:
                    specialType = SpecialType.System_Int64;
                    break;
                case UnaryOperatorKind.ULong:
                    specialType = SpecialType.System_UInt64;
                    break;
                case UnaryOperatorKind.NInt:
                    specialType = SpecialType.System_IntPtr;
                    break;
                case UnaryOperatorKind.NUInt:
                    specialType = SpecialType.System_UIntPtr;
                    break;
                case UnaryOperatorKind.Float:
                    specialType = SpecialType.System_Single;
                    break;
                case UnaryOperatorKind.Double:
                    specialType = SpecialType.System_Double;
                    break;
                case UnaryOperatorKind.Decimal:
                    specialType = SpecialType.System_Decimal;
                    break;
                case UnaryOperatorKind.Pointer:
                    return node.Type;
                default:
                    throw ExceptionUtilities.UnexpectedValue(unaryOperatorKind);
            }
            NamedTypeSymbol namedTypeSymbol = _compilation.GetSpecialType(specialType);
            if (node.OperatorKind.IsLifted())
            {
                namedTypeSymbol = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(namedTypeSymbol);
            }
            return namedTypeSymbol;
        }

        private static BinaryOperatorKind GetCorrespondingBinaryOperator(BoundIncrementOperator node)
        {
            UnaryOperatorKind operatorKind = node.OperatorKind;
            BinaryOperatorKind binaryOperatorKind;
            switch (operatorKind.OperandTypes())
            {
                case UnaryOperatorKind.SByte:
                case UnaryOperatorKind.Short:
                case UnaryOperatorKind.Int:
                    binaryOperatorKind = BinaryOperatorKind.Int;
                    break;
                case UnaryOperatorKind.Byte:
                case UnaryOperatorKind.UShort:
                case UnaryOperatorKind.UInt:
                case UnaryOperatorKind.Char:
                    binaryOperatorKind = BinaryOperatorKind.UInt;
                    break;
                case UnaryOperatorKind.Long:
                    binaryOperatorKind = BinaryOperatorKind.Long;
                    break;
                case UnaryOperatorKind.ULong:
                    binaryOperatorKind = BinaryOperatorKind.ULong;
                    break;
                case UnaryOperatorKind.NInt:
                    binaryOperatorKind = BinaryOperatorKind.NInt;
                    break;
                case UnaryOperatorKind.NUInt:
                    binaryOperatorKind = BinaryOperatorKind.NUInt;
                    break;
                case UnaryOperatorKind.Float:
                    binaryOperatorKind = BinaryOperatorKind.Float;
                    break;
                case UnaryOperatorKind.Double:
                    binaryOperatorKind = BinaryOperatorKind.Double;
                    break;
                case UnaryOperatorKind.Decimal:
                    binaryOperatorKind = BinaryOperatorKind.Decimal;
                    break;
                case UnaryOperatorKind.Enum:
                    {
                        TypeSymbol type = node.Type;
                        if (type.IsNullableType())
                        {
                            type = type.GetNullableUnderlyingType();
                        }
                        type = type.GetEnumUnderlyingType();
                        switch (type.SpecialType)
                        {
                            case SpecialType.System_SByte:
                            case SpecialType.System_Int16:
                            case SpecialType.System_Int32:
                                binaryOperatorKind = BinaryOperatorKind.Int;
                                break;
                            case SpecialType.System_Byte:
                            case SpecialType.System_UInt16:
                            case SpecialType.System_UInt32:
                                binaryOperatorKind = BinaryOperatorKind.UInt;
                                break;
                            case SpecialType.System_Int64:
                                binaryOperatorKind = BinaryOperatorKind.Long;
                                break;
                            case SpecialType.System_UInt64:
                                binaryOperatorKind = BinaryOperatorKind.ULong;
                                break;
                            default:
                                throw ExceptionUtilities.UnexpectedValue(type.SpecialType);
                        }
                        break;
                    }
                case UnaryOperatorKind.Pointer:
                    binaryOperatorKind = BinaryOperatorKind.PointerAndInt;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(operatorKind.OperandTypes());
            }
            if ((uint)(binaryOperatorKind - 5) <= 5u || binaryOperatorKind == BinaryOperatorKind.PointerAndInt)
            {
                binaryOperatorKind = (BinaryOperatorKind)((int)binaryOperatorKind | (int)operatorKind.OverflowChecks());
            }
            if (operatorKind.IsLifted())
            {
                binaryOperatorKind |= BinaryOperatorKind.Lifted;
            }
            return binaryOperatorKind;
        }

        private static (TypeSymbol, ConstantValue) GetConstantOneForIncrement(CSharpCompilation compilation, BinaryOperatorKind binaryOperatorKind)
        {
            ConstantValue constantValue;
            switch (binaryOperatorKind.OperandTypes())
            {
                case BinaryOperatorKind.Int:
                case BinaryOperatorKind.PointerAndInt:
                    constantValue = ConstantValue.Create(1);
                    break;
                case BinaryOperatorKind.UInt:
                    constantValue = ConstantValue.Create(1u);
                    break;
                case BinaryOperatorKind.Long:
                    constantValue = ConstantValue.Create(1L);
                    break;
                case BinaryOperatorKind.ULong:
                    constantValue = ConstantValue.Create(1uL);
                    break;
                case BinaryOperatorKind.NInt:
                    constantValue = ConstantValue.Create(1);
                    return (compilation.CreateNativeIntegerTypeSymbol(signed: true), constantValue);
                case BinaryOperatorKind.NUInt:
                    constantValue = ConstantValue.Create(1u);
                    return (compilation.CreateNativeIntegerTypeSymbol(signed: false), constantValue);
                case BinaryOperatorKind.Float:
                    constantValue = ConstantValue.Create(1f);
                    break;
                case BinaryOperatorKind.Double:
                    constantValue = ConstantValue.Create(1.0);
                    break;
                case BinaryOperatorKind.Decimal:
                    constantValue = ConstantValue.Create(1m);
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(binaryOperatorKind.OperandTypes());
            }
            return (compilation.GetSpecialType(constantValue.SpecialType), constantValue);
        }

        public override BoundNode VisitUsingStatement(BoundUsingStatement node)
        {
            BoundStatement boundStatement = VisitStatement(node.Body);
            BoundBlock boundBlock = ((boundStatement.Kind == BoundKind.Block) ? ((BoundBlock)boundStatement) : BoundBlock.SynthesizedNoLocals(node.Syntax, boundStatement));
            if (node.ExpressionOpt != null)
            {
                return MakeExpressionUsingStatement(node, boundBlock);
            }
            SyntaxToken awaitKeyword = ((node.Syntax.Kind() == SyntaxKind.UsingStatement) ? ((UsingStatementSyntax)node.Syntax).AwaitKeyword : default(SyntaxToken));
            return MakeDeclarationUsingStatement(node.Syntax, boundBlock, node.Locals, node.DeclarationsOpt!.LocalDeclarations, node.IDisposableConversion, node.PatternDisposeInfoOpt, node.AwaitOpt, awaitKeyword);
        }

        private BoundStatement MakeDeclarationUsingStatement(SyntaxNode syntax, BoundBlock body, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundLocalDeclaration> declarations, Conversion iDisposableConversion, MethodArgumentInfo? patternDisposeInfo, BoundAwaitableInfo? awaitOpt, SyntaxToken awaitKeyword)
        {
            BoundBlock boundBlock = body;
            for (int num = declarations.Length - 1; num >= 0; num--)
            {
                boundBlock = RewriteDeclarationUsingStatement(syntax, declarations[num], boundBlock, iDisposableConversion, awaitKeyword, awaitOpt, patternDisposeInfo);
            }
            return new BoundBlock(syntax, locals, ImmutableArray.Create((BoundStatement)boundBlock));
        }

        private BoundStatement MakeLocalUsingDeclarationStatement(BoundUsingLocalDeclarations usingDeclarations, ImmutableArray<BoundStatement> statements)
        {
            LocalDeclarationStatementSyntax localDeclarationStatementSyntax = (LocalDeclarationStatementSyntax)usingDeclarations.Syntax;
            BoundBlock body = new BoundBlock(localDeclarationStatementSyntax, ImmutableArray<LocalSymbol>.Empty, statements);
            return MakeDeclarationUsingStatement(localDeclarationStatementSyntax, body, ImmutableArray<LocalSymbol>.Empty, usingDeclarations.LocalDeclarations, usingDeclarations.IDisposableConversion, usingDeclarations.PatternDisposeInfoOpt, usingDeclarations.AwaitOpt, localDeclarationStatementSyntax.AwaitKeyword);
        }

        private BoundBlock MakeExpressionUsingStatement(BoundUsingStatement node, BoundBlock tryBlock)
        {
            BoundExpression boundExpression = VisitExpression(node.ExpressionOpt);
            if (boundExpression.ConstantValue == ConstantValue.Null)
            {
                return tryBlock;
            }
            TypeSymbol type = boundExpression.Type;
            SyntaxNode syntax = boundExpression.Syntax;
            UsingStatementSyntax usingStatementSyntax = (UsingStatementSyntax)node.Syntax;
            BoundLocal boundLocal;
            BoundAssignmentOperator store;
            if ((object)type == null || type.IsDynamic())
            {
                TypeSymbol typeSymbol = ((node.AwaitOpt == null) ? _compilation.GetSpecialType(SpecialType.System_IDisposable) : _compilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable));
                _diagnostics.ReportUseSite(typeSymbol, usingStatementSyntax);
                BoundExpression argument = MakeConversionNode(syntax, boundExpression, Conversion.GetTrivialConversion(node.IDisposableConversion.Kind), typeSymbol, @checked: false, explicitCastInCode: false, boundExpression.ConstantValue);
                boundLocal = _factory.StoreToTemp(argument, out store, RefKind.None, SynthesizedLocalKind.Using);
            }
            else
            {
                boundLocal = _factory.StoreToTemp(boundExpression, out store, RefKind.None, SynthesizedLocalKind.Using, usingStatementSyntax);
            }
            BoundStatement boundStatement = new BoundExpressionStatement(syntax, store);
            if (Instrument)
            {
                boundStatement = _instrumenter.InstrumentUsingTargetCapture(node, boundStatement);
            }
            BoundStatement item = RewriteUsingStatementTryFinally(usingStatementSyntax, tryBlock, boundLocal, usingStatementSyntax.AwaitKeyword, node.AwaitOpt, node.PatternDisposeInfoOpt);
            return new BoundBlock(usingStatementSyntax, node.Locals.Add(boundLocal.LocalSymbol), ImmutableArray.Create(boundStatement, item));
        }

        private BoundBlock RewriteDeclarationUsingStatement(SyntaxNode usingSyntax, BoundLocalDeclaration localDeclaration, BoundBlock tryBlock, Conversion iDisposableConversion, SyntaxToken awaitKeywordOpt, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfo)
        {
            SyntaxNode syntax = localDeclaration.Syntax;
            LocalSymbol localSymbol = localDeclaration.LocalSymbol;
            TypeSymbol type = localSymbol.Type;
            BoundLocal boundLocal = new BoundLocal(syntax, localSymbol, localDeclaration.InitializerOpt!.ConstantValue, type);
            BoundStatement boundStatement = VisitStatement(localDeclaration);
            if (boundLocal.ConstantValue == ConstantValue.Null)
            {
                return BoundBlock.SynthesizedNoLocals(usingSyntax, boundStatement, tryBlock);
            }
            if (type.IsDynamic())
            {
                TypeSymbol typeSymbol = ((awaitOpt == null) ? _compilation.GetSpecialType(SpecialType.System_IDisposable) : _compilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable));
                _diagnostics.ReportUseSite(typeSymbol, usingSyntax);
                BoundExpression argument = MakeConversionNode(syntax, boundLocal, iDisposableConversion, typeSymbol, @checked: false);
                BoundLocal boundLocal2 = _factory.StoreToTemp(argument, out BoundAssignmentOperator store, RefKind.None, SynthesizedLocalKind.Using);
                BoundStatement item = RewriteUsingStatementTryFinally(usingSyntax, tryBlock, boundLocal2, awaitKeywordOpt, awaitOpt, patternDisposeInfo);
                return new BoundBlock(usingSyntax, ImmutableArray.Create(boundLocal2.LocalSymbol), ImmutableArray.Create(boundStatement, new BoundExpressionStatement(syntax, store), item));
            }
            BoundStatement boundStatement2 = RewriteUsingStatementTryFinally(usingSyntax, tryBlock, boundLocal, awaitKeywordOpt, awaitOpt, patternDisposeInfo);
            return BoundBlock.SynthesizedNoLocals(usingSyntax, boundStatement, boundStatement2);
        }

        private BoundStatement RewriteUsingStatementTryFinally(SyntaxNode syntax, BoundBlock tryBlock, BoundLocal local, SyntaxToken awaitKeywordOpt, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfo)
        {
            bool num = local.Type.IsNullableType();
            BoundExpression disposedExpression;
            if (num)
            {
                MethodSymbol method = UnsafeGetNullableMethod(syntax, local.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
                disposedExpression = BoundCall.Synthesized(syntax, local, method);
            }
            else
            {
                disposedExpression = local;
            }
            BoundExpression expression = GenerateDisposeCall(syntax, disposedExpression, patternDisposeInfo, awaitOpt, awaitKeywordOpt);
            BoundStatement boundStatement = new BoundExpressionStatement(syntax, expression);
            BoundExpression boundExpression = (num ? MakeNullableHasValue(syntax, local) : ((!local.Type.IsValueType) ? MakeNullCheck(syntax, local, BinaryOperatorKind.NotEqual) : null));
            return new BoundTryStatement(finallyBlockOpt: BoundBlock.SynthesizedNoLocals(syntax, (boundExpression != null) ? RewriteIfStatement(syntax, boundExpression, boundStatement, null, hasErrors: false) : boundStatement), syntax: syntax, tryBlock: tryBlock, catchBlocks: ImmutableArray<BoundCatchBlock>.Empty);
        }

        private BoundExpression GenerateDisposeCall(SyntaxNode syntax, BoundExpression disposedExpression, MethodArgumentInfo? disposeInfo, BoundAwaitableInfo? awaitOpt, SyntaxToken awaitKeyword)
        {
            MethodSymbol symbol = disposeInfo?.Method;
            if ((object)symbol == null)
            {
                if (awaitOpt == null)
                {
                    Binder.TryGetSpecialTypeMember<MethodSymbol>(_compilation, SpecialMember.System_IDisposable__Dispose, syntax, _diagnostics, out symbol);
                }
                else
                {
                    TryGetWellKnownTypeMember<MethodSymbol>(null, WellKnownMember.System_IAsyncDisposable__DisposeAsync, out symbol, isOptional: false, awaitKeyword.GetLocation());
                }
            }
            BoundExpression boundExpression;
            if ((object)symbol == null)
            {
                boundExpression = new BoundBadExpression(syntax, LookupResultKind.NotInvocable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(disposedExpression), ErrorTypeSymbol.UnknownResultType);
            }
            else
            {
                if ((object)disposeInfo == null)
                {
                    disposeInfo = MethodArgumentInfo.CreateParameterlessMethod(symbol);
                }
                boundExpression = MakeCallWithNoExplicitArgument(disposeInfo, syntax, disposedExpression);
                if (awaitOpt != null)
                {
                    _sawAwaitInExceptionHandler = true;
                    TypeSymbol type = awaitOpt!.GetResult?.ReturnType ?? _compilation.DynamicType;
                    boundExpression = RewriteAwaitExpression(syntax, boundExpression, awaitOpt, type, used: false);
                }
            }
            return boundExpression;
        }

        private BoundExpression MakeCallWithNoExplicitArgument(MethodArgumentInfo methodArgumentInfo, SyntaxNode syntax, BoundExpression? expression, bool assertParametersAreOptional = true)
        {
            MethodSymbol method = methodArgumentInfo.Method;
            return MakeCall(syntax, expression, method, methodArgumentInfo.Arguments, default(ImmutableArray<RefKind>), methodArgumentInfo.Expanded, method.IsExtensionMethod, methodArgumentInfo.ArgsToParamsOpt, LookupResultKind.Viable, method.ReturnType);
        }

        public override BoundNode VisitWhileStatement(BoundWhileStatement node)
        {
            BoundExpression rewrittenCondition = VisitExpression(node.Condition);
            BoundStatement rewrittenBody = VisitStatement(node.Body);
            if (!node.WasCompilerGenerated && Instrument)
            {
                rewrittenCondition = _instrumenter.InstrumentWhileStatementCondition(node, rewrittenCondition, _factory);
            }
            return RewriteWhileStatement(node, node.Locals, rewrittenCondition, rewrittenBody, node.BreakLabel, node.ContinueLabel, node.HasErrors);
        }

        private BoundStatement RewriteWhileStatement(BoundLoopStatement loop, BoundExpression rewrittenCondition, BoundStatement rewrittenBody, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors)
        {
            SyntaxNode syntax = loop.Syntax;
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("start");
            BoundStatement boundStatement = new BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, jumpIfTrue: true, label);
            BoundStatement boundStatement2 = new BoundGotoStatement(syntax, continueLabel);
            if (Instrument && !loop.WasCompilerGenerated)
            {
                boundStatement = loop.Kind switch
                {
                    BoundKind.WhileStatement => _instrumenter.InstrumentWhileStatementConditionalGotoStartOrBreak((BoundWhileStatement)loop, boundStatement),
                    BoundKind.ForEachStatement => _instrumenter.InstrumentForEachStatementConditionalGotoStart((BoundForEachStatement)loop, boundStatement),
                    _ => throw ExceptionUtilities.UnexpectedValue(loop.Kind),
                };
                boundStatement2 = BoundSequencePoint.CreateHidden(boundStatement2);
            }
            return BoundStatementList.Synthesized(syntax, hasErrors, boundStatement2, new BoundLabelStatement(syntax, label), rewrittenBody, new BoundLabelStatement(syntax, continueLabel), boundStatement, new BoundLabelStatement(syntax, breakLabel));
        }

        private BoundStatement RewriteWhileStatement(BoundWhileStatement loop, ImmutableArray<LocalSymbol> locals, BoundExpression rewrittenCondition, BoundStatement rewrittenBody, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors)
        {
            if (locals.IsEmpty)
            {
                return RewriteWhileStatement(loop, rewrittenCondition, rewrittenBody, breakLabel, continueLabel, hasErrors);
            }
            SyntaxNode syntax = loop.Syntax;
            BoundStatement boundStatement = new BoundLabelStatement(syntax, continueLabel);
            BoundStatement boundStatement2 = new BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, jumpIfTrue: false, breakLabel);
            if (Instrument && !loop.WasCompilerGenerated)
            {
                boundStatement2 = _instrumenter.InstrumentWhileStatementConditionalGotoStartOrBreak(loop, boundStatement2);
                boundStatement = BoundSequencePoint.CreateHidden(boundStatement);
            }
            return BoundStatementList.Synthesized(syntax, hasErrors, boundStatement, new BoundBlock(syntax, locals, ImmutableArray.Create(boundStatement2, rewrittenBody, new BoundGotoStatement(syntax, continueLabel))), new BoundLabelStatement(syntax, breakLabel));
        }

        public override BoundNode VisitYieldBreakStatement(BoundYieldBreakStatement node)
        {
            BoundStatement boundStatement = (BoundStatement)base.VisitYieldBreakStatement(node);
            if (Instrument)
            {
                if (!node.WasCompilerGenerated)
                {
                    goto IL_004b;
                }
                if (node.Syntax.Kind() == SyntaxKind.Block)
                {
                    MethodSymbol? currentFunction = _factory.CurrentFunction;
                    if ((object)currentFunction != null && !currentFunction!.IsAsync)
                    {
                        goto IL_004b;
                    }
                }
            }
            goto IL_0059;
        IL_004b:
            boundStatement = _instrumenter.InstrumentYieldBreakStatement(node, boundStatement);
            goto IL_0059;
        IL_0059:
            return boundStatement;
        }

        public override BoundNode VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            BoundStatement boundStatement = (BoundStatement)base.VisitYieldReturnStatement(node);
            if (Instrument && !node.WasCompilerGenerated)
            {
                boundStatement = _instrumenter.InstrumentYieldReturnStatement(node, boundStatement);
            }
            return boundStatement;
        }
    }
}
