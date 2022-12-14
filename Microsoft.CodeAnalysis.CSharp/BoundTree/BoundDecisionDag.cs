// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundDecisionDag
    {
        private ImmutableHashSet<LabelSymbol> _reachableLabels;
        private ImmutableArray<BoundDecisionDagNode> _topologicallySortedNodes;

        internal static ImmutableArray<BoundDecisionDagNode> Successors(BoundDecisionDagNode node)
        {
            return node switch
            {
                BoundEvaluationDecisionDagNode p => ImmutableArray.Create(p.Next),
                BoundTestDecisionDagNode p => ImmutableArray.Create(p.WhenFalse, p.WhenTrue),
                BoundLeafDecisionDagNode _ => ImmutableArray<BoundDecisionDagNode>.Empty,
                BoundWhenDecisionDagNode w => (w.WhenFalse != null) ? ImmutableArray.Create(w.WhenTrue, w.WhenFalse) : ImmutableArray.Create(w.WhenTrue),
                _ => throw ExceptionUtilities.UnexpectedValue(node.Kind),
            };
        }

        public ImmutableHashSet<LabelSymbol> ReachableLabels
        {
            get
            {
                if (_reachableLabels == null)
                {
                    var result = ImmutableHashSet.CreateBuilder<LabelSymbol>(Symbols.SymbolEqualityComparer.ConsiderEverything);
                    foreach (var node in this.TopologicallySortedNodes)
                    {
                        if (node is BoundLeafDecisionDagNode leaf)
                        {
                            result.Add(leaf.Label);
                        }
                    }

                    _reachableLabels = result.ToImmutableHashSet();
                }

                return _reachableLabels;
            }
        }

        /// <summary>
        /// A list of all the nodes reachable from the root node, in a topologically sorted order.
        /// </summary>
        public ImmutableArray<BoundDecisionDagNode> TopologicallySortedNodes
        {
            get
            {
                if (_topologicallySortedNodes.IsDefault)
                {
                    // We use an iterative topological sort to avoid overflowing the compiler's runtime stack for a large switch statement.
                    bool wasAcyclic = TopologicalSort.TryIterativeSort<BoundDecisionDagNode>(new[] { this.RootNode }, Successors, out _topologicallySortedNodes);

                    // Since these nodes were constructed by an isomorphic mapping from a known acyclic graph, it cannot be cyclic
                }

                return _topologicallySortedNodes;
            }
        }

        /// <summary>
        /// Rewrite a decision dag, using a mapping function that rewrites one node at a time. That function
        /// takes as its input the node to be rewritten and a function that returns the previously computed
        /// rewritten node for successor nodes.
        /// </summary>
        public BoundDecisionDag Rewrite(Func<BoundDecisionDagNode, Func<BoundDecisionDagNode, BoundDecisionDagNode>, BoundDecisionDagNode> makeReplacement)
        {
            // First, we topologically sort the nodes of the dag so that we can translate the nodes bottom-up.
            // This will avoid overflowing the compiler's runtime stack which would occur for a large switch
            // statement if we were using a recursive strategy.
            ImmutableArray<BoundDecisionDagNode> sortedNodes = this.TopologicallySortedNodes;

            // Cache simplified/translated replacement for each translated dag node. Since we always visit
            // a node's successors before the node, the replacement should always be in the cache when we need it.
            var replacement = PooledDictionary<BoundDecisionDagNode, BoundDecisionDagNode>.GetInstance();

            BoundDecisionDagNode getReplacementForChild(BoundDecisionDagNode n) => replacement[n];

            // Loop backwards through the topologically sorted nodes to translate them, so that we always visit a node after its successors
            for (int i = sortedNodes.Length - 1; i >= 0; i--)
            {
                BoundDecisionDagNode node = sortedNodes[i];
                BoundDecisionDagNode newNode = makeReplacement(node, getReplacementForChild);
                replacement.Add(node, newNode);
            }

            // Return the computed replacement root node
            var newRoot = replacement[this.RootNode];
            replacement.Free();
            return this.Update(newRoot);
        }

        /// <summary>
        /// A trivial node replacement function for use with <see cref="Rewrite(Func{BoundDecisionDagNode, Func{BoundDecisionDagNode, BoundDecisionDagNode}, BoundDecisionDagNode})"/>.
        /// </summary>
        public static BoundDecisionDagNode TrivialReplacement(BoundDecisionDagNode dag, Func<BoundDecisionDagNode, BoundDecisionDagNode> replacement)
        {
            return dag switch
            {
                BoundEvaluationDecisionDagNode p => p.Update(p.Evaluation, replacement(p.Next)),
                BoundTestDecisionDagNode p => p.Update(p.Test, replacement(p.WhenTrue), replacement(p.WhenFalse)),
                BoundWhenDecisionDagNode p => p.Update(p.Bindings, p.WhenExpression, replacement(p.WhenTrue), (p.WhenFalse != null) ? replacement(p.WhenFalse) : null),
                BoundLeafDecisionDagNode p => p,
                _ => throw ExceptionUtilities.UnexpectedValue(dag),
            };
        }

        /// <summary>
        /// Given a decision dag and a constant-valued input, produce a simplified decision dag that has removed all the
        /// tests that are unnecessary due to that constant value. This simplification affects flow analysis (reachability
        /// and definite assignment) and permits us to simplify the generated code.
        /// </summary>
        public BoundDecisionDag SimplifyDecisionDagIfConstantInput(BoundExpression input)
        {
            if (input.ConstantValue == null)
            {
                return this;
            }
            else
            {
                ConstantValue inputConstant = input.ConstantValue;
                return Rewrite(makeReplacement);

                // Make a replacement for a given node, using the precomputed replacements for its successors.
                BoundDecisionDagNode makeReplacement(BoundDecisionDagNode dag, Func<BoundDecisionDagNode, BoundDecisionDagNode> replacement)
                {
                    if (dag is BoundTestDecisionDagNode p)
                    {
                        // This is the key to the optimization. The result of a top-level test might be known if the input is constant.
                        switch (knownResult(p.Test))
                        {
                            case true:
                                return replacement(p.WhenTrue);
                            case false:
                                return replacement(p.WhenFalse);
                        }
                    }

                    return TrivialReplacement(dag, replacement);
                }

                // Is the decision's result known because the input is a constant?
                bool? knownResult(BoundDagTest choice)
                {
                    if (!choice.Input.IsOriginalInput)
                    {
                        // This is a test of something other than the main input; result unknown
                        return null;
                    }

                    switch (choice)
                    {
                        case BoundDagExplicitNullTest d:
                            return inputConstant.IsNull;
                        case BoundDagNonNullTest d:
                            return !inputConstant.IsNull;
                        case BoundDagValueTest d:
                            return d.Value == inputConstant;
                        case BoundDagTypeTest d:
                            return inputConstant.IsNull ? false : null;
                        case BoundDagRelationalTest d:
                            var f = ValueSetFactory.ForType(input.Type);
                            if (f is null) return null;
                            // TODO: When ValueSetFactory has a method for comparing two values, use it.
                            var set = f.Related(d.Relation.Operator(), d.Value);
                            return set.Any(BinaryOperatorKind.Equal, inputConstant);
                        default:
                            throw ExceptionUtilities.UnexpectedValue(choice);
                    }
                }
            }
        }

#if DEBUG
        /// <summary>
        /// Starting with `this` state, produce a human-readable description of the state tables.
        /// This is very useful for debugging and optimizing the dag state construction.
        /// </summary>
        internal new string Dump()
        {
            var allStates = this.TopologicallySortedNodes;
            var stateIdentifierMap = PooledDictionary<BoundDecisionDagNode, int>.GetInstance();
            for (int i = 0; i < allStates.Length; i++)
            {
                stateIdentifierMap.Add(allStates[i], i);
            }

            int nextTempNumber = 0;
            var tempIdentifierMap = PooledDictionary<BoundDagEvaluation, int>.GetInstance();
            int tempIdentifier(BoundDagEvaluation e)
            {
                return (e == null) ? 0 : tempIdentifierMap.TryGetValue(e, out int value) ? value : tempIdentifierMap[e] = ++nextTempNumber;
            }

            string tempName(BoundDagTemp t)
            {
                return $"t{tempIdentifier(t.Source)}{(t.Index != 0 ? $".{t.Index}" : "")}";
            }

            var resultBuilder = PooledStringBuilder.GetInstance();
            var result = resultBuilder.Builder;

            foreach (var state in allStates)
            {
                result.AppendLine($"State " + stateIdentifierMap[state]);
                switch (state)
                {
                    case BoundTestDecisionDagNode node:
                        result.AppendLine($"  Test: {dumpDagTest(node.Test)}");
                        if (node.WhenTrue != null)
                        {
                            result.AppendLine($"  WhenTrue: {stateIdentifierMap[node.WhenTrue]}");
                        }

                        if (node.WhenFalse != null)
                        {
                            result.AppendLine($"  WhenFalse: {stateIdentifierMap[node.WhenFalse]}");
                        }
                        break;
                    case BoundEvaluationDecisionDagNode node:
                        result.AppendLine($"  Test: {dumpDagTest(node.Evaluation)}");
                        if (node.Next != null)
                        {
                            result.AppendLine($"  Next: {stateIdentifierMap[node.Next]}");
                        }
                        break;
                    case BoundWhenDecisionDagNode node:
                        result.AppendLine($"  WhenClause: " + node.WhenExpression?.Syntax);
                        if (node.WhenTrue != null)
                        {
                            result.AppendLine($"  WhenTrue: {stateIdentifierMap[node.WhenTrue]}");
                        }

                        if (node.WhenFalse != null)
                        {
                            result.AppendLine($"  WhenFalse: {stateIdentifierMap[node.WhenFalse]}");
                        }
                        break;
                    case BoundLeafDecisionDagNode node:
                        result.AppendLine($"  Case: {node.Label.Name}" + node.Syntax);
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(state);
                }
            }

            stateIdentifierMap.Free();
            tempIdentifierMap.Free();
            return resultBuilder.ToStringAndFree();

            string dumpDagTest(BoundDagTest d)
            {
                switch (d)
                {
                    case BoundDagTypeEvaluation a:
                        return $"t{tempIdentifier(a)}={a.Kind}(t{tempIdentifier(a)} as {a.Type})";
                    case BoundDagEvaluation e:
                        return $"t{tempIdentifier(e)}={e.Kind}(t{tempIdentifier(e)})";
                    case BoundDagTypeTest b:
                        return $"?{d.Kind}({tempName(d.Input)} is {b.Type})";
                    case BoundDagValueTest v:
                        return $"?{d.Kind}({tempName(d.Input)} == {v.Value})";
                    case BoundDagRelationalTest r:
                        var operatorName = r.Relation.Operator() switch
                        {
                            BinaryOperatorKind.LessThan => "<",
                            BinaryOperatorKind.LessThanOrEqual => "<=",
                            BinaryOperatorKind.GreaterThan => ">",
                            BinaryOperatorKind.GreaterThanOrEqual => ">=",
                            _ => "??"
                        };
                        return $"?{d.Kind}({tempName(d.Input)} {operatorName} {r.Value})";
                    default:
                        return $"?{d.Kind}({tempName(d.Input)})";
                }
            }
        }
#endif
    }
}
