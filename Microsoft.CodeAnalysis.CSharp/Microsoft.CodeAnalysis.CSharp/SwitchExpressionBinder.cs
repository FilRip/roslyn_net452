using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class SwitchExpressionBinder : Binder
    {
        private readonly SwitchExpressionSyntax SwitchExpressionSyntax;

        internal SwitchExpressionBinder(SwitchExpressionSyntax switchExpressionSyntax, Binder next)
            : base(next)
        {
            SwitchExpressionSyntax = switchExpressionSyntax;
        }

        internal override BoundExpression BindSwitchExpressionCore(SwitchExpressionSyntax node, Binder originalBinder, BindingDiagnosticBag diagnostics)
        {
            BoundExpression boundExpression = BindSwitchGoverningExpression(diagnostics);
            ImmutableArray<BoundSwitchExpressionArm> immutableArray = BindSwitchExpressionArms(node, originalBinder, boundExpression, diagnostics);
            TypeSymbol type = InferResultType(immutableArray, diagnostics);
            bool reportedNotExhaustive = CheckSwitchExpressionExhaustive(node, boundExpression, immutableArray, out BoundDecisionDag decisionDag, out LabelSymbol defaultLabel, diagnostics);
            decisionDag = decisionDag.SimplifyDecisionDagIfConstantInput(boundExpression);
            return new BoundUnconvertedSwitchExpression(node, boundExpression, immutableArray, decisionDag, defaultLabel, reportedNotExhaustive, type);
        }

        private bool CheckSwitchExpressionExhaustive(SwitchExpressionSyntax node, BoundExpression boundInputExpression, ImmutableArray<BoundSwitchExpressionArm> switchArms, out BoundDecisionDag decisionDag, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out LabelSymbol? defaultLabel, BindingDiagnosticBag diagnostics)
        {
            defaultLabel = new GeneratedLabelSymbol("default");
            decisionDag = DecisionDagBuilder.CreateDecisionDagForSwitchExpression(base.Compilation, node, boundInputExpression, switchArms, defaultLabel, diagnostics);
            ImmutableHashSet<LabelSymbol> reachableLabels = decisionDag.ReachableLabels;
            bool flag = false;
            ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = switchArms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchExpressionArm current = enumerator.Current;
                flag |= current.HasErrors;
                if (!flag && !reachableLabels.Contains(current.Label))
                {
                    diagnostics.Add(ErrorCode.ERR_SwitchArmSubsumed, current.Pattern.Syntax.Location);
                }
            }
            if (!reachableLabels.Contains(defaultLabel))
            {
                defaultLabel = null;
                return false;
            }
            if (flag)
            {
                return true;
            }
            TopologicalSort.TryIterativeSort(new BoundDecisionDagNode[1] { decisionDag.RootNode }, nonNullSuccessors, out var result);
            ImmutableArray<BoundDecisionDagNode>.Enumerator enumerator2 = result.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                BoundDecisionDagNode current2 = enumerator2.Current;
                if (current2 is BoundLeafDecisionDagNode boundLeafDecisionDagNode && boundLeafDecisionDagNode.Label == defaultLabel)
                {
                    string text = PatternExplainer.SamplePatternForPathToDagNode(BoundDagTemp.ForOriginalInput(boundInputExpression), result, current2, nullPaths: false, out bool requiresFalseWhenClause, out bool unnamedEnumValue);
                    ErrorCode code = (requiresFalseWhenClause ? ErrorCode.WRN_SwitchExpressionNotExhaustiveWithWhen : (unnamedEnumValue ? ErrorCode.WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue : ErrorCode.WRN_SwitchExpressionNotExhaustive));
                    diagnostics.Add(code, node.SwitchKeyword.GetLocation(), text);
                    return true;
                }
            }
            return false;
            static ImmutableArray<BoundDecisionDagNode> nonNullSuccessors(BoundDecisionDagNode n)
            {
                if (n is BoundTestDecisionDagNode boundTestDecisionDagNode)
                {
                    BoundDagTest test = boundTestDecisionDagNode.Test;
                    if (test is BoundDagNonNullTest)
                    {
                        return ImmutableArray.Create(boundTestDecisionDagNode.WhenTrue);
                    }
                    if (test is BoundDagExplicitNullTest)
                    {
                        return ImmutableArray.Create(boundTestDecisionDagNode.WhenFalse);
                    }
                    return BoundDecisionDag.Successors(n);
                }
                return BoundDecisionDag.Successors(n);
            }
        }

        private TypeSymbol? InferResultType(ImmutableArray<BoundSwitchExpressionArm> switchCases, BindingDiagnosticBag diagnostics)
        {
            PooledHashSet<TypeSymbol> pooledSymbolHashSetInstance = SpecializedSymbolCollections.GetPooledSymbolHashSetInstance<TypeSymbol>();
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
            ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = switchCases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeSymbol type = enumerator.Current.Value.Type;
                if ((object)type != null && pooledSymbolHashSetInstance.Add(type))
                {
                    instance.Add(type);
                }
            }
            pooledSymbolHashSetInstance.Free();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            TypeSymbol typeSymbol = BestTypeInferrer.GetBestType(instance, base.Conversions, ref useSiteInfo);
            instance.Free();
            if ((object)typeSymbol != null)
            {
                enumerator = switchCases.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundSwitchExpressionArm current = enumerator.Current;
                    if (!base.Conversions.ClassifyImplicitConversionFromExpression(current.Value, typeSymbol, ref useSiteInfo).Exists)
                    {
                        typeSymbol = null;
                        break;
                    }
                }
            }
            diagnostics.Add(SwitchExpressionSyntax, useSiteInfo);
            return typeSymbol;
        }

        private ImmutableArray<BoundSwitchExpressionArm> BindSwitchExpressionArms(SwitchExpressionSyntax node, Binder originalBinder, BoundExpression inputExpression, BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<BoundSwitchExpressionArm> instance = ArrayBuilder<BoundSwitchExpressionArm>.GetInstance();
            (TypeSymbol GoverningType, uint GoverningValEscape) inputTypeAndValEscape = GetInputTypeAndValEscape(inputExpression);
            TypeSymbol item = inputTypeAndValEscape.GoverningType;
            uint item2 = inputTypeAndValEscape.GoverningValEscape;
            SeparatedSyntaxList<SwitchExpressionArmSyntax>.Enumerator enumerator = node.Arms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchExpressionArmSyntax current = enumerator.Current;
                BoundSwitchExpressionArm item3 = originalBinder.GetRequiredBinder(current).BindSwitchExpressionArm(current, item, item2, diagnostics);
                instance.Add(item3);
            }
            return instance.ToImmutableAndFree();
        }

        internal (TypeSymbol GoverningType, uint GoverningValEscape) GetInputTypeAndValEscape(BoundExpression? inputExpression = null)
        {
            if (inputExpression == null)
            {
                inputExpression = BindSwitchGoverningExpression(BindingDiagnosticBag.Discarded);
            }
            return (inputExpression!.Type, Binder.GetValEscape(inputExpression, LocalScopeDepth));
        }

        private BoundExpression BindSwitchGoverningExpression(BindingDiagnosticBag diagnostics)
        {
            BoundExpression boundExpression = BindRValueWithoutTargetType(SwitchExpressionSyntax.GoverningExpression, diagnostics);
            if ((object)boundExpression.Type == null || boundExpression.Type.IsVoidType())
            {
                diagnostics.Add(ErrorCode.ERR_BadPatternExpression, SwitchExpressionSyntax.GoverningExpression.Location, boundExpression.Display);
                boundExpression = GenerateConversionForAssignment(CreateErrorType(), boundExpression, diagnostics);
            }
            return boundExpression;
        }
    }
}
