using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDecisionDag : BoundNode
    {
        private ImmutableHashSet<LabelSymbol> _reachableLabels;

        private ImmutableArray<BoundDecisionDagNode> _topologicallySortedNodes;

        public ImmutableHashSet<LabelSymbol> ReachableLabels
        {
            get
            {
                if (_reachableLabels == null)
                {
                    ImmutableHashSet<LabelSymbol>.Builder builder = ImmutableHashSet.CreateBuilder((IEqualityComparer<LabelSymbol>?)Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything);
                    ImmutableArray<BoundDecisionDagNode>.Enumerator enumerator = TopologicallySortedNodes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is BoundLeafDecisionDagNode boundLeafDecisionDagNode)
                        {
                            builder.Add(boundLeafDecisionDagNode.Label);
                        }
                    }
                    _reachableLabels = builder.ToImmutableHashSet();
                }
                return _reachableLabels;
            }
        }

        public ImmutableArray<BoundDecisionDagNode> TopologicallySortedNodes
        {
            get
            {
                if (_topologicallySortedNodes.IsDefault)
                {
                    TopologicalSort.TryIterativeSort(new BoundDecisionDagNode[1] { RootNode }, Successors, out _topologicallySortedNodes);
                }
                return _topologicallySortedNodes;
            }
        }

        public BoundDecisionDagNode RootNode { get; }

        internal static ImmutableArray<BoundDecisionDagNode> Successors(BoundDecisionDagNode node)
        {
            if (!(node is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
            {
                if (!(node is BoundTestDecisionDagNode boundTestDecisionDagNode))
                {
                    if (!(node is BoundLeafDecisionDagNode))
                    {
                        if (node is BoundWhenDecisionDagNode boundWhenDecisionDagNode)
                        {
                            if (boundWhenDecisionDagNode.WhenFalse == null)
                            {
                                return ImmutableArray.Create(boundWhenDecisionDagNode.WhenTrue);
                            }
                            return ImmutableArray.Create<BoundDecisionDagNode>(boundWhenDecisionDagNode.WhenTrue, boundWhenDecisionDagNode.WhenFalse);
                        }
                        throw ExceptionUtilities.UnexpectedValue(node.Kind);
                    }
                    return ImmutableArray<BoundDecisionDagNode>.Empty;
                }
                return ImmutableArray.Create(boundTestDecisionDagNode.WhenFalse, boundTestDecisionDagNode.WhenTrue);
            }
            return ImmutableArray.Create(boundEvaluationDecisionDagNode.Next);
        }

        public BoundDecisionDag Rewrite(Func<BoundDecisionDagNode, Func<BoundDecisionDagNode, BoundDecisionDagNode>, BoundDecisionDagNode> makeReplacement)
        {
            ImmutableArray<BoundDecisionDagNode> topologicallySortedNodes = TopologicallySortedNodes;
            PooledDictionary<BoundDecisionDagNode, BoundDecisionDagNode> replacement = PooledDictionary<BoundDecisionDagNode, BoundDecisionDagNode>.GetInstance();
            BoundDecisionDagNode arg(BoundDecisionDagNode n) => replacement[n];
            for (int num = topologicallySortedNodes.Length - 1; num >= 0; num--)
            {
                BoundDecisionDagNode boundDecisionDagNode = topologicallySortedNodes[num];
                BoundDecisionDagNode value = makeReplacement(boundDecisionDagNode, arg);
                replacement.Add(boundDecisionDagNode, value);
            }
            BoundDecisionDagNode rootNode = replacement[RootNode];
            replacement.Free();
            return Update(rootNode);
        }

        public static BoundDecisionDagNode TrivialReplacement(BoundDecisionDagNode dag, Func<BoundDecisionDagNode, BoundDecisionDagNode> replacement)
        {
            if (!(dag is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
            {
                if (!(dag is BoundTestDecisionDagNode boundTestDecisionDagNode))
                {
                    if (!(dag is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        if (dag is BoundLeafDecisionDagNode result)
                        {
                            return result;
                        }
                        throw ExceptionUtilities.UnexpectedValue(dag);
                    }
                    return boundWhenDecisionDagNode.Update(boundWhenDecisionDagNode.Bindings, boundWhenDecisionDagNode.WhenExpression, replacement(boundWhenDecisionDagNode.WhenTrue), (boundWhenDecisionDagNode.WhenFalse != null) ? replacement(boundWhenDecisionDagNode.WhenFalse) : null);
                }
                return boundTestDecisionDagNode.Update(boundTestDecisionDagNode.Test, replacement(boundTestDecisionDagNode.WhenTrue), replacement(boundTestDecisionDagNode.WhenFalse));
            }
            return boundEvaluationDecisionDagNode.Update(boundEvaluationDecisionDagNode.Evaluation, replacement(boundEvaluationDecisionDagNode.Next));
        }

        public BoundDecisionDag SimplifyDecisionDagIfConstantInput(BoundExpression input)
        {
            if (input.ConstantValue == null)
            {
                return this;
            }
            ConstantValue inputConstant = input.ConstantValue;
            return Rewrite(makeReplacement);
            bool? knownResult(BoundDagTest choice)
            {
                if (!choice.Input.IsOriginalInput)
                {
                    return null;
                }
                if (choice is BoundDagExplicitNullTest)
                {
                    return inputConstant.IsNull;
                }
                if (choice is BoundDagNonNullTest)
                {
                    return !inputConstant.IsNull;
                }
                if (choice is BoundDagValueTest boundDagValueTest)
                {
                    return boundDagValueTest.Value == inputConstant;
                }
                if (choice is BoundDagTypeTest)
                {
                    if (!inputConstant.IsNull)
                    {
                        return null;
                    }
                    return false;
                }
                if (choice is BoundDagRelationalTest boundDagRelationalTest)
                {
                    return ValueSetFactory.ForType(input.Type)?.Related(boundDagRelationalTest.Relation.Operator(), boundDagRelationalTest.Value).Any(BinaryOperatorKind.Equal, inputConstant);
                }
                throw ExceptionUtilities.UnexpectedValue(choice);
            }
            BoundDecisionDagNode makeReplacement(BoundDecisionDagNode dag, Func<BoundDecisionDagNode, BoundDecisionDagNode> replacement)
            {
                if (dag is BoundTestDecisionDagNode boundTestDecisionDagNode)
                {
                    bool? flag = knownResult(boundTestDecisionDagNode.Test);
                    if (flag.HasValue)
                    {
                        if (flag.GetValueOrDefault())
                        {
                            return replacement(boundTestDecisionDagNode.WhenTrue);
                        }
                        return replacement(boundTestDecisionDagNode.WhenFalse);
                    }
                }
                return TrivialReplacement(dag, replacement);
            }
        }

        public BoundDecisionDag(SyntaxNode syntax, BoundDecisionDagNode rootNode, bool hasErrors = false)
            : base(BoundKind.DecisionDag, syntax, hasErrors || rootNode.HasErrors())
        {
            RootNode = rootNode;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDecisionDag(this);
        }

        public BoundDecisionDag Update(BoundDecisionDagNode rootNode)
        {
            if (rootNode != RootNode)
            {
                BoundDecisionDag boundDecisionDag = new BoundDecisionDag(Syntax, rootNode, base.HasErrors);
                boundDecisionDag.CopyAttributes(this);
                return boundDecisionDag;
            }
            return this;
        }
    }
}
