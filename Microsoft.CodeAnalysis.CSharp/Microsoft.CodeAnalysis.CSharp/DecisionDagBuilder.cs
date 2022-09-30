using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class DecisionDagBuilder
    {
        private sealed class DecisionDag
        {
            public readonly DagState RootNode;

            public DecisionDag(DagState rootNode)
            {
                RootNode = rootNode;
            }

            private static ImmutableArray<DagState> Successor(DagState state)
            {
                if (state.TrueBranch != null && state.FalseBranch != null)
                {
                    return ImmutableArray.Create<DagState>(state.FalseBranch, state.TrueBranch);
                }
                if (state.TrueBranch != null)
                {
                    return ImmutableArray.Create(state.TrueBranch);
                }
                if (state.FalseBranch != null)
                {
                    return ImmutableArray.Create(state.FalseBranch);
                }
                return ImmutableArray<DagState>.Empty;
            }

            public bool TryGetTopologicallySortedReachableStates(out ImmutableArray<DagState> result)
            {
                return TopologicalSort.TryIterativeSort(SpecializedCollections.SingletonEnumerable(RootNode), Successor, out result);
            }
        }

        private sealed class DagState
        {
            public readonly ImmutableArray<StateForCase> Cases;

            public BoundDagTest? SelectedTest;

            public DagState? TrueBranch;

            public DagState? FalseBranch;

            public BoundDecisionDagNode? Dag;

            public ImmutableDictionary<BoundDagTemp, IValueSet> RemainingValues { get; private set; }

            public DagState(ImmutableArray<StateForCase> cases, ImmutableDictionary<BoundDagTemp, IValueSet> remainingValues)
            {
                Cases = cases;
                RemainingValues = remainingValues;
            }

            internal BoundDagTest ComputeSelectedTest()
            {
                return Cases[0].RemainingTests.ComputeSelectedTest();
            }

            internal void UpdateRemainingValues(ImmutableDictionary<BoundDagTemp, IValueSet> newRemainingValues)
            {
                RemainingValues = newRemainingValues;
                SelectedTest = null;
                TrueBranch = null;
                FalseBranch = null;
            }
        }

        private sealed class DagStateEquivalence : IEqualityComparer<DagState>
        {
            public static readonly DagStateEquivalence Instance = new DagStateEquivalence();

            private DagStateEquivalence()
            {
            }

            public bool Equals(DagState? x, DagState? y)
            {
                if (x != y)
                {
                    return x!.Cases.SequenceEqual(y!.Cases, (StateForCase a, StateForCase b) => a.Equals(b));
                }
                return true;
            }

            public int GetHashCode(DagState x)
            {
                return Hash.Combine(Hash.CombineValues(x.Cases), x.Cases.Length);
            }
        }

        private sealed class StateForCase
        {
            public readonly int Index;

            public readonly SyntaxNode Syntax;

            public readonly Tests RemainingTests;

            public readonly ImmutableArray<BoundPatternBinding> Bindings;

            public readonly BoundExpression? WhenClause;

            public readonly LabelSymbol CaseLabel;

            public bool IsFullyMatched
            {
                get
                {
                    if (RemainingTests is Tests.True)
                    {
                        if (WhenClause != null)
                        {
                            return WhenClause!.ConstantValue == ConstantValue.True;
                        }
                        return true;
                    }
                    return false;
                }
            }

            public bool PatternIsSatisfied => RemainingTests is Tests.True;

            public bool IsImpossible => RemainingTests is Tests.False;

            public StateForCase(int Index, SyntaxNode Syntax, Tests RemainingTests, ImmutableArray<BoundPatternBinding> Bindings, BoundExpression? WhenClause, LabelSymbol CaseLabel)
            {
                this.Index = Index;
                this.Syntax = Syntax;
                this.RemainingTests = RemainingTests;
                this.Bindings = Bindings;
                this.WhenClause = WhenClause;
                this.CaseLabel = CaseLabel;
            }

            public override bool Equals(object? obj)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public bool Equals(StateForCase other)
            {
                if (this != other)
                {
                    if (other != null && Index == other.Index)
                    {
                        return RemainingTests.Equals(other.RemainingTests);
                    }
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(RemainingTests.GetHashCode(), Index);
            }
        }

        private abstract class Tests
        {
            public sealed class True : Tests
            {
                public static readonly True Instance = new True();

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    return "TRUE";
                }

                public override void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest)
                {
                    whenTrue = (whenFalse = this);
                }
            }

            public sealed class False : Tests
            {
                public static readonly False Instance = new False();

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    return "FALSE";
                }

                public override void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest)
                {
                    whenTrue = (whenFalse = this);
                }
            }

            public sealed class One : Tests
            {
                public readonly BoundDagTest Test;

                public One(BoundDagTest test)
                {
                    Test = test;
                }

                public void Deconstruct(out BoundDagTest Test)
                {
                    Test = this.Test;
                }

                public override void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest)
                {
                    builder.CheckConsistentDecision(test, Test, whenTrueValues, whenFalseValues, test.Syntax, out var trueTestPermitsTrueOther, out var falseTestPermitsTrueOther, out var trueTestImpliesTrueOther, out var falseTestImpliesTrueOther, ref foundExplicitNullTest);
                    whenTrue = (trueTestImpliesTrueOther ? True.Instance : (trueTestPermitsTrueOther ? this : ((Tests)False.Instance)));
                    whenFalse = (falseTestImpliesTrueOther ? True.Instance : (falseTestPermitsTrueOther ? this : ((Tests)False.Instance)));
                }

                public override BoundDagTest ComputeSelectedTest()
                {
                    return Test;
                }

                public override Tests RemoveEvaluation(BoundDagEvaluation e)
                {
                    if (!e.Equals(Test))
                    {
                        return this;
                    }
                    return True.Instance;
                }

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    return dump(Test);
                }

                public override bool Equals(object? obj)
                {
                    if (this != obj)
                    {
                        if (obj is One one)
                        {
                            return Test.Equals(one.Test);
                        }
                        return false;
                    }
                    return true;
                }

                public override int GetHashCode()
                {
                    return Test.GetHashCode();
                }
            }

            public sealed class Not : Tests
            {
                public readonly Tests Negated;

                private Not(Tests negated)
                {
                    Negated = negated;
                }

                public static Tests Create(Tests negated)
                {
                    if (!(negated is True))
                    {
                        if (!(negated is False))
                        {
                            if (!(negated is Not not))
                            {
                                if (!(negated is AndSequence negated2))
                                {
                                    if (!(negated is OrSequence orSequence))
                                    {
                                        if (negated is One negated3)
                                        {
                                            return new Not(negated3);
                                        }
                                        throw ExceptionUtilities.UnexpectedValue(negated);
                                    }
                                    return AndSequence.Create(NegateSequenceElements(orSequence.RemainingTests));
                                }
                                return new Not(negated2);
                            }
                            return not.Negated;
                        }
                        return True.Instance;
                    }
                    return False.Instance;
                }

                private static ArrayBuilder<Tests> NegateSequenceElements(ImmutableArray<Tests> seq)
                {
                    ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(seq.Length);
                    ImmutableArray<Tests>.Enumerator enumerator = seq.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Tests current = enumerator.Current;
                        instance.Add(Create(current));
                    }
                    return instance;
                }

                public override Tests RemoveEvaluation(BoundDagEvaluation e)
                {
                    return Create(Negated.RemoveEvaluation(e));
                }

                public override BoundDagTest ComputeSelectedTest()
                {
                    return Negated.ComputeSelectedTest();
                }

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    return "Not (" + Negated.Dump(dump) + ")";
                }

                public override void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest)
                {
                    Negated.Filter(builder, test, whenTrueValues, whenFalseValues, out var whenTrue2, out var whenFalse2, ref foundExplicitNullTest);
                    whenTrue = Create(whenTrue2);
                    whenFalse = Create(whenFalse2);
                }

                public override bool Equals(object? obj)
                {
                    if (this != obj)
                    {
                        if (obj is Not not)
                        {
                            return Negated.Equals(not.Negated);
                        }
                        return false;
                    }
                    return true;
                }

                public override int GetHashCode()
                {
                    return Hash.Combine(Negated.GetHashCode(), typeof(Not).GetHashCode());
                }
            }

            public abstract class SequenceTests : Tests
            {
                public readonly ImmutableArray<Tests> RemainingTests;

                protected SequenceTests(ImmutableArray<Tests> remainingTests)
                {
                    RemainingTests = remainingTests;
                }

                public abstract Tests Update(ArrayBuilder<Tests> remainingTests);

                public override void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest)
                {
                    ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(RemainingTests.Length);
                    ArrayBuilder<Tests> instance2 = ArrayBuilder<Tests>.GetInstance(RemainingTests.Length);
                    ImmutableArray<Tests>.Enumerator enumerator = RemainingTests.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Filter(builder, test, whenTrueValues, whenFalseValues, out var whenTrue2, out var whenFalse2, ref foundExplicitNullTest);
                        instance.Add(whenTrue2);
                        instance2.Add(whenFalse2);
                    }
                    whenTrue = Update(instance);
                    whenFalse = Update(instance2);
                }

                public override Tests RemoveEvaluation(BoundDagEvaluation e)
                {
                    ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(RemainingTests.Length);
                    ImmutableArray<Tests>.Enumerator enumerator = RemainingTests.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Tests current = enumerator.Current;
                        instance.Add(current.RemoveEvaluation(e));
                    }
                    return Update(instance);
                }

                public override bool Equals(object? obj)
                {
                    if (this != obj)
                    {
                        if (obj is SequenceTests sequenceTests && GetType() == sequenceTests.GetType())
                        {
                            return RemainingTests.SequenceEqual(sequenceTests.RemainingTests);
                        }
                        return false;
                    }
                    return true;
                }

                public override int GetHashCode()
                {
                    int currentKey = Hash.Combine(RemainingTests.Length, GetType().GetHashCode());
                    return Hash.Combine(Hash.CombineValues(RemainingTests), currentKey);
                }
            }

            public sealed class AndSequence : SequenceTests
            {
                private AndSequence(ImmutableArray<Tests> remainingTests)
                    : base(remainingTests)
                {
                }

                public override Tests Update(ArrayBuilder<Tests> remainingTests)
                {
                    return Create(remainingTests);
                }

                public static Tests Create(ArrayBuilder<Tests> remainingTests)
                {
                    for (int num = remainingTests.Count - 1; num >= 0; num--)
                    {
                        Tests tests = remainingTests[num];
                        if (!(tests is True))
                        {
                            if (tests is False result)
                            {
                                remainingTests.Free();
                                return result;
                            }
                            if (tests is AndSequence andSequence)
                            {
                                ImmutableArray<Tests> remainingTests2 = andSequence.RemainingTests;
                                remainingTests.RemoveAt(num);
                                int i = 0;
                                for (int length = remainingTests2.Length; i < length; i++)
                                {
                                    remainingTests.Insert(num + i, remainingTests2[i]);
                                }
                            }
                        }
                        else
                        {
                            remainingTests.RemoveAt(num);
                        }
                    }
                    object result2 = remainingTests.Count switch
                    {
                        0 => True.Instance,
                        1 => remainingTests[0],
                        _ => new AndSequence(remainingTests.ToImmutable()),
                    };
                    remainingTests.Free();
                    return (Tests)result2;
                }

                public override BoundDagTest ComputeSelectedTest()
                {
                    if (RemainingTests[0] is One one)
                    {
                        BoundDagTest test = one.Test;
                        if (test != null && test.Kind == BoundKind.DagNonNullTest && RemainingTests[1] is One one2)
                        {
                            BoundDagTest test2 = one2.Test;
                            if (test2 != null)
                            {
                                switch (test2.Kind)
                                {
                                    case BoundKind.DagTypeTest:
                                        if (test.Input != test2.Input)
                                        {
                                            return test;
                                        }
                                        return test2;
                                    case BoundKind.DagValueTest:
                                        {
                                            BoundDagTest boundDagTest = test2;
                                            if (test.Input != boundDagTest.Input)
                                            {
                                                return test;
                                            }
                                            return boundDagTest;
                                        }
                                }
                            }
                        }
                    }
                    return RemainingTests[0].ComputeSelectedTest();
                }

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    Func<BoundDagTest, string> dump2 = dump;
                    return "AND(" + string.Join(", ", RemainingTests.Select((Tests t) => t.Dump(dump2))) + ")";
                }
            }

            public sealed class OrSequence : SequenceTests
            {
                private OrSequence(ImmutableArray<Tests> remainingTests)
                    : base(remainingTests)
                {
                }

                public override BoundDagTest ComputeSelectedTest()
                {
                    return RemainingTests[0].ComputeSelectedTest();
                }

                public override Tests Update(ArrayBuilder<Tests> remainingTests)
                {
                    return Create(remainingTests);
                }

                public static Tests Create(ArrayBuilder<Tests> remainingTests)
                {
                    for (int num = remainingTests.Count - 1; num >= 0; num--)
                    {
                        Tests tests = remainingTests[num];
                        if (!(tests is False))
                        {
                            if (tests is True result)
                            {
                                remainingTests.Free();
                                return result;
                            }
                            if (tests is OrSequence orSequence)
                            {
                                remainingTests.RemoveAt(num);
                                ImmutableArray<Tests> remainingTests2 = orSequence.RemainingTests;
                                int i = 0;
                                for (int length = remainingTests2.Length; i < length; i++)
                                {
                                    remainingTests.Insert(num + i, remainingTests2[i]);
                                }
                            }
                        }
                        else
                        {
                            remainingTests.RemoveAt(num);
                        }
                    }
                    object result2 = remainingTests.Count switch
                    {
                        0 => False.Instance,
                        1 => remainingTests[0],
                        _ => new OrSequence(remainingTests.ToImmutable()),
                    };
                    remainingTests.Free();
                    return (Tests)result2;
                }

                public override string Dump(Func<BoundDagTest, string> dump)
                {
                    Func<BoundDagTest, string> dump2 = dump;
                    return "OR(" + string.Join(", ", RemainingTests.Select((Tests t) => t.Dump(dump2))) + ")";
                }
            }

            private Tests()
            {
            }

            public abstract void Filter(DecisionDagBuilder builder, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out Tests whenTrue, out Tests whenFalse, ref bool foundExplicitNullTest);

            public virtual BoundDagTest ComputeSelectedTest()
            {
                throw ExceptionUtilities.Unreachable;
            }

            public virtual Tests RemoveEvaluation(BoundDagEvaluation e)
            {
                return this;
            }

            public abstract string Dump(Func<BoundDagTest, string> dump);
        }

        private readonly CSharpCompilation _compilation;

        private readonly Conversions _conversions;

        private readonly BindingDiagnosticBag _diagnostics;

        private readonly LabelSymbol _defaultLabel;

        private DecisionDagBuilder(CSharpCompilation compilation, LabelSymbol defaultLabel, BindingDiagnosticBag diagnostics)
        {
            _compilation = compilation;
            _conversions = compilation.Conversions;
            _diagnostics = diagnostics;
            _defaultLabel = defaultLabel;
        }

        public static BoundDecisionDag CreateDecisionDagForSwitchStatement(CSharpCompilation compilation, SyntaxNode syntax, BoundExpression switchGoverningExpression, ImmutableArray<BoundSwitchSection> switchSections, LabelSymbol defaultLabel, BindingDiagnosticBag diagnostics)
        {
            return new DecisionDagBuilder(compilation, defaultLabel, diagnostics).CreateDecisionDagForSwitchStatement(syntax, switchGoverningExpression, switchSections);
        }

        public static BoundDecisionDag CreateDecisionDagForSwitchExpression(CSharpCompilation compilation, SyntaxNode syntax, BoundExpression switchExpressionInput, ImmutableArray<BoundSwitchExpressionArm> switchArms, LabelSymbol defaultLabel, BindingDiagnosticBag diagnostics)
        {
            return new DecisionDagBuilder(compilation, defaultLabel, diagnostics).CreateDecisionDagForSwitchExpression(syntax, switchExpressionInput, switchArms);
        }

        public static BoundDecisionDag CreateDecisionDagForIsPattern(CSharpCompilation compilation, SyntaxNode syntax, BoundExpression inputExpression, BoundPattern pattern, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel, BindingDiagnosticBag diagnostics)
        {
            return new DecisionDagBuilder(compilation, whenFalseLabel, diagnostics).CreateDecisionDagForIsPattern(syntax, inputExpression, pattern, whenTrueLabel);
        }

        private BoundDecisionDag CreateDecisionDagForIsPattern(SyntaxNode syntax, BoundExpression inputExpression, BoundPattern pattern, LabelSymbol whenTrueLabel)
        {
            BoundDagTemp input = BoundDagTemp.ForOriginalInput(inputExpression);
            return MakeBoundDecisionDag(syntax, ImmutableArray.Create(MakeTestsForPattern(1, pattern.Syntax, input, pattern, null, whenTrueLabel)));
        }

        private BoundDecisionDag CreateDecisionDagForSwitchStatement(SyntaxNode syntax, BoundExpression switchGoverningExpression, ImmutableArray<BoundSwitchSection> switchSections)
        {
            BoundDagTemp input = BoundDagTemp.ForOriginalInput(switchGoverningExpression);
            int num = 0;
            ArrayBuilder<StateForCase> instance = ArrayBuilder<StateForCase>.GetInstance(switchSections.Length);
            ImmutableArray<BoundSwitchSection>.Enumerator enumerator = switchSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = enumerator.Current.SwitchLabels.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundSwitchLabel current = enumerator2.Current;
                    if (current.Syntax.Kind() != SyntaxKind.DefaultSwitchLabel)
                    {
                        instance.Add(MakeTestsForPattern(++num, current.Syntax, input, current.Pattern, current.WhenClause, current.Label));
                    }
                }
            }
            return MakeBoundDecisionDag(syntax, instance.ToImmutableAndFree());
        }

        private BoundDecisionDag CreateDecisionDagForSwitchExpression(SyntaxNode syntax, BoundExpression switchExpressionInput, ImmutableArray<BoundSwitchExpressionArm> switchArms)
        {
            BoundDagTemp input = BoundDagTemp.ForOriginalInput(switchExpressionInput);
            int num = 0;
            ArrayBuilder<StateForCase> instance = ArrayBuilder<StateForCase>.GetInstance(switchArms.Length);
            ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = switchArms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchExpressionArm current = enumerator.Current;
                instance.Add(MakeTestsForPattern(++num, current.Syntax, input, current.Pattern, current.WhenClause, current.Label));
            }
            return MakeBoundDecisionDag(syntax, instance.ToImmutableAndFree());
        }

        private StateForCase MakeTestsForPattern(int index, SyntaxNode syntax, BoundDagTemp input, BoundPattern pattern, BoundExpression? whenClause, LabelSymbol label)
        {
            Tests remainingTests = MakeAndSimplifyTestsAndBindings(input, pattern, out ImmutableArray<BoundPatternBinding> bindings);
            return new StateForCase(index, syntax, remainingTests, bindings, whenClause, label);
        }

        private Tests MakeAndSimplifyTestsAndBindings(BoundDagTemp input, BoundPattern pattern, out ImmutableArray<BoundPatternBinding> bindings)
        {
            ArrayBuilder<BoundPatternBinding> instance = ArrayBuilder<BoundPatternBinding>.GetInstance();
            Tests result = SimplifyTestsAndBindings(MakeTestsAndBindings(input, pattern, instance), instance);
            bindings = instance.ToImmutableAndFree();
            return result;
        }

        private static Tests SimplifyTestsAndBindings(Tests tests, ArrayBuilder<BoundPatternBinding> bindingsBuilder)
        {
            PooledHashSet<BoundDagEvaluation> usedValues = PooledHashSet<BoundDagEvaluation>.GetInstance();
            ArrayBuilder<BoundPatternBinding>.Enumerator enumerator = bindingsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundDagTemp tempContainingValue = enumerator.Current.TempContainingValue;
                if (tempContainingValue.Source != null)
                {
                    usedValues.Add(tempContainingValue.Source);
                }
            }
            Tests result = scanAndSimplify(tests);
            usedValues.Free();
            return result;
            Tests scanAndSimplify(Tests tests)
            {
                if (tests is Tests.SequenceTests sequenceTests)
                {
                    ImmutableArray<Tests> remainingTests = sequenceTests.RemainingTests;
                    int length = remainingTests.Length;
                    ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(length);
                    instance.AddRange(remainingTests);
                    for (int num = length - 1; num >= 0; num--)
                    {
                        instance[num] = scanAndSimplify(instance[num]);
                    }
                    return sequenceTests.Update(instance);
                }
                if (tests is Tests.True || tests is Tests.False)
                {
                    return tests;
                }
                if (tests is Tests.One one)
                {
                    one.Deconstruct(out var Test);
                    if (Test is BoundDagEvaluation boundDagEvaluation)
                    {
                        if (usedValues.Contains(boundDagEvaluation))
                        {
                            if (boundDagEvaluation.Input.Source != null)
                            {
                                usedValues.Add(boundDagEvaluation.Input.Source);
                            }
                            return tests;
                        }
                        return Tests.True.Instance;
                    }
                    if (Test != null)
                    {
                        if (Test.Input.Source != null)
                        {
                            usedValues.Add(Test.Input.Source);
                        }
                        return tests;
                    }
                }
                else if (tests is Tests.Not not)
                {
                    return Tests.Not.Create(scanAndSimplify(not.Negated));
                }
                throw ExceptionUtilities.UnexpectedValue(tests);
            }
        }

        private Tests MakeTestsAndBindings(BoundDagTemp input, BoundPattern pattern, ArrayBuilder<BoundPatternBinding> bindings)
        {
            return MakeTestsAndBindings(input, pattern, out BoundDagTemp output, bindings);
        }

        private Tests MakeTestsAndBindings(BoundDagTemp input, BoundPattern pattern, out BoundDagTemp output, ArrayBuilder<BoundPatternBinding> bindings)
        {
            if (!(pattern is BoundDeclarationPattern declaration))
            {
                if (!(pattern is BoundConstantPattern constant))
                {
                    if (!(pattern is BoundDiscardPattern))
                    {
                        if (!(pattern is BoundRecursivePattern recursive))
                        {
                            if (!(pattern is BoundITuplePattern pattern2))
                            {
                                if (!(pattern is BoundTypePattern typePattern))
                                {
                                    if (!(pattern is BoundRelationalPattern rel))
                                    {
                                        if (!(pattern is BoundNegatedPattern neg))
                                        {
                                            if (pattern is BoundBinaryPattern bin)
                                            {
                                                return MakeTestsAndBindingsForBinaryPattern(input, bin, out output, bindings);
                                            }
                                            throw ExceptionUtilities.UnexpectedValue(pattern.Kind);
                                        }
                                        output = input;
                                        return MakeTestsAndBindingsForNegatedPattern(input, neg, bindings);
                                    }
                                    return MakeTestsAndBindingsForRelationalPattern(input, rel, out output);
                                }
                                return MakeTestsForTypePattern(input, typePattern, out output);
                            }
                            return MakeTestsAndBindingsForITuplePattern(input, pattern2, out output, bindings);
                        }
                        return MakeTestsAndBindingsForRecursivePattern(input, recursive, out output, bindings);
                    }
                    output = input;
                    return Tests.True.Instance;
                }
                return MakeTestsForConstantPattern(input, constant, out output);
            }
            return MakeTestsAndBindingsForDeclarationPattern(input, declaration, out output, bindings);
        }

        private Tests MakeTestsAndBindingsForITuplePattern(BoundDagTemp input, BoundITuplePattern pattern, out BoundDagTemp output, ArrayBuilder<BoundPatternBinding> bindings)
        {
            SyntaxNode syntax = pattern.Syntax;
            int length = pattern.Subpatterns.Length;
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Object);
            PropertySymbol propertySymbol = (PropertySymbol)pattern.GetLengthMethod.AssociatedSymbol;
            PropertySymbol propertySymbol2 = (PropertySymbol)pattern.GetItemMethod.AssociatedSymbol;
            NamedTypeSymbol containingType = propertySymbol.ContainingType;
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(4 + length * 2);
            instance.Add(new Tests.One(new BoundDagTypeTest(syntax, containingType, input)));
            BoundDagTypeEvaluation boundDagTypeEvaluation = new BoundDagTypeEvaluation(syntax, containingType, input);
            instance.Add(new Tests.One(boundDagTypeEvaluation));
            BoundDagTemp input2 = (output = new BoundDagTemp(syntax, containingType, boundDagTypeEvaluation));
            BoundDagPropertyEvaluation boundDagPropertyEvaluation = new BoundDagPropertyEvaluation(syntax, propertySymbol, OriginalInput(input2, propertySymbol));
            instance.Add(new Tests.One(boundDagPropertyEvaluation));
            BoundDagTemp input3 = new BoundDagTemp(syntax, _compilation.GetSpecialType(SpecialType.System_Int32), boundDagPropertyEvaluation);
            instance.Add(new Tests.One(new BoundDagValueTest(syntax, ConstantValue.Create(length), input3)));
            BoundDagTemp input4 = OriginalInput(input2, propertySymbol2);
            for (int i = 0; i < length; i++)
            {
                BoundDagIndexEvaluation boundDagIndexEvaluation = new BoundDagIndexEvaluation(syntax, propertySymbol2, i, input4);
                instance.Add(new Tests.One(boundDagIndexEvaluation));
                BoundDagTemp input5 = new BoundDagTemp(syntax, specialType, boundDagIndexEvaluation);
                instance.Add(MakeTestsAndBindings(input5, pattern.Subpatterns[i].Pattern, bindings));
            }
            return Tests.AndSequence.Create(instance);
        }

        private BoundDagTemp OriginalInput(BoundDagTemp input, Symbol symbol)
        {
            while (input.Source is BoundDagTypeEvaluation boundDagTypeEvaluation && IsDerivedType(boundDagTypeEvaluation.Input.Type, symbol.ContainingType))
            {
                input = boundDagTypeEvaluation.Input;
            }
            return input;
        }

        private bool IsDerivedType(TypeSymbol possibleDerived, TypeSymbol possibleBase)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return _conversions.HasIdentityOrImplicitReferenceConversion(possibleDerived, possibleBase, ref useSiteInfo);
        }

        private Tests MakeTestsAndBindingsForDeclarationPattern(BoundDagTemp input, BoundDeclarationPattern declaration, out BoundDagTemp output, ArrayBuilder<BoundPatternBinding> bindings)
        {
            TypeSymbol type = declaration.DeclaredType?.Type;
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(1);
            if (!declaration.IsVar)
            {
                input = MakeConvertToType(input, declaration.Syntax, type, isExplicitTest: false, instance);
            }
            BoundExpression variableAccess = declaration.VariableAccess;
            if (variableAccess != null)
            {
                bindings.Add(new BoundPatternBinding(variableAccess, input));
            }
            output = input;
            return Tests.AndSequence.Create(instance);
        }

        private Tests MakeTestsForTypePattern(BoundDagTemp input, BoundTypePattern typePattern, out BoundDagTemp output)
        {
            TypeSymbol type = typePattern.DeclaredType.Type;
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(4);
            output = MakeConvertToType(input, typePattern.Syntax, type, typePattern.IsExplicitNotNullTest, instance);
            return Tests.AndSequence.Create(instance);
        }

        private static void MakeCheckNotNull(BoundDagTemp input, SyntaxNode syntax, bool isExplicitTest, ArrayBuilder<Tests> tests)
        {
            if (input.Type.CanContainNull())
            {
                tests.Add(new Tests.One(new BoundDagNonNullTest(syntax, isExplicitTest, input)));
            }
        }

        private BoundDagTemp MakeConvertToType(BoundDagTemp input, SyntaxNode syntax, TypeSymbol type, bool isExplicitTest, ArrayBuilder<Tests> tests)
        {
            MakeCheckNotNull(input, syntax, isExplicitTest, tests);
            if (!input.Type.Equals(type, TypeCompareKind.AllIgnoreOptions))
            {
                TypeSymbol source = input.Type.StrippedType();
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(_diagnostics, _compilation.Assembly);
                Conversion conversion = _conversions.ClassifyBuiltInConversion(source, type, ref useSiteInfo);
                _diagnostics.Add(syntax, useSiteInfo);
                if (!(input.Type.IsDynamic() ? (type.SpecialType == SpecialType.System_Object) : conversion.IsImplicit))
                {
                    tests.Add(new Tests.One(new BoundDagTypeTest(syntax, type, input)));
                }
                BoundDagTypeEvaluation boundDagTypeEvaluation = new BoundDagTypeEvaluation(syntax, type, input);
                input = new BoundDagTemp(syntax, type, boundDagTypeEvaluation);
                tests.Add(new Tests.One(boundDagTypeEvaluation));
            }
            return input;
        }

        private Tests MakeTestsForConstantPattern(BoundDagTemp input, BoundConstantPattern constant, out BoundDagTemp output)
        {
            if (constant.ConstantValue == ConstantValue.Null)
            {
                output = input;
                return new Tests.One(new BoundDagExplicitNullTest(constant.Syntax, input));
            }
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(2);
            instance.Add(new Tests.One(new BoundDagValueTest(input: output = MakeConvertToType(input, constant.Syntax, constant.Value.Type, isExplicitTest: false, instance), syntax: constant.Syntax, value: constant.ConstantValue)));
            return Tests.AndSequence.Create(instance);
        }

        private Tests MakeTestsAndBindingsForRecursivePattern(BoundDagTemp input, BoundRecursivePattern recursive, out BoundDagTemp output, ArrayBuilder<BoundPatternBinding> bindings)
        {
            TypeSymbol typeSymbol = recursive.DeclaredType?.Type ?? input.Type.StrippedType();
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(5);
            output = (input = MakeConvertToType(input, recursive.Syntax, typeSymbol, recursive.IsExplicitNotNullTest, instance));
            if (!recursive.Deconstruction.IsDefault)
            {
                if (recursive.DeconstructMethod != null)
                {
                    MethodSymbol deconstructMethod = recursive.DeconstructMethod;
                    BoundDagDeconstructEvaluation boundDagDeconstructEvaluation = new BoundDagDeconstructEvaluation(recursive.Syntax, deconstructMethod, OriginalInput(input, deconstructMethod));
                    instance.Add(new Tests.One(boundDagDeconstructEvaluation));
                    int num = (deconstructMethod.IsStatic ? 1 : 0);
                    int num2 = Math.Min(deconstructMethod.ParameterCount - num, recursive.Deconstruction.Length);
                    for (int i = 0; i < num2; i++)
                    {
                        BoundPattern pattern = recursive.Deconstruction[i].Pattern;
                        BoundDagTemp input2 = new BoundDagTemp(pattern.Syntax, deconstructMethod.Parameters[i + num].Type, boundDagDeconstructEvaluation, i);
                        instance.Add(MakeTestsAndBindings(input2, pattern, bindings));
                    }
                }
                else if (!Binder.IsZeroElementTupleType(typeSymbol))
                {
                    if (typeSymbol.IsTupleType)
                    {
                        ImmutableArray<FieldSymbol> tupleElements = typeSymbol.TupleElements;
                        int num3 = Math.Min(typeSymbol.TupleElementTypesWithAnnotations.Length, recursive.Deconstruction.Length);
                        for (int j = 0; j < num3; j++)
                        {
                            BoundPattern pattern2 = recursive.Deconstruction[j].Pattern;
                            SyntaxNode syntax = pattern2.Syntax;
                            FieldSymbol fieldSymbol = tupleElements[j];
                            BoundDagFieldEvaluation boundDagFieldEvaluation = new BoundDagFieldEvaluation(syntax, fieldSymbol, OriginalInput(input, fieldSymbol));
                            instance.Add(new Tests.One(boundDagFieldEvaluation));
                            BoundDagTemp input3 = new BoundDagTemp(syntax, fieldSymbol.Type, boundDagFieldEvaluation);
                            instance.Add(MakeTestsAndBindings(input3, pattern2, bindings));
                        }
                    }
                    else
                    {
                        instance.Add(new Tests.One(new BoundDagTypeTest(recursive.Syntax, ErrorType(), input, hasErrors: true)));
                    }
                }
            }
            if (!recursive.Properties.IsDefault)
            {
                for (int k = 0; k < recursive.Properties.Length; k++)
                {
                    BoundSubpattern boundSubpattern = recursive.Properties[k];
                    Symbol symbol = boundSubpattern.Symbol;
                    BoundPattern pattern3 = boundSubpattern.Pattern;
                    BoundDagEvaluation boundDagEvaluation;
                    if (!(symbol is PropertySymbol propertySymbol))
                    {
                        if (!(symbol is FieldSymbol fieldSymbol2))
                        {
                            instance.Add(new Tests.One(new BoundDagTypeTest(recursive.Syntax, ErrorType(), input, hasErrors: true)));
                            continue;
                        }
                        boundDagEvaluation = new BoundDagFieldEvaluation(pattern3.Syntax, fieldSymbol2, OriginalInput(input, fieldSymbol2));
                    }
                    else
                    {
                        boundDagEvaluation = new BoundDagPropertyEvaluation(pattern3.Syntax, propertySymbol, OriginalInput(input, propertySymbol));
                    }
                    instance.Add(new Tests.One(boundDagEvaluation));
                    BoundDagTemp input4 = new BoundDagTemp(pattern3.Syntax, symbol.GetTypeOrReturnType().Type, boundDagEvaluation);
                    instance.Add(MakeTestsAndBindings(input4, pattern3, bindings));
                }
            }
            if (recursive.VariableAccess != null)
            {
                bindings.Add(new BoundPatternBinding(recursive.VariableAccess, input));
            }
            return Tests.AndSequence.Create(instance);
        }

        private Tests MakeTestsAndBindingsForNegatedPattern(BoundDagTemp input, BoundNegatedPattern neg, ArrayBuilder<BoundPatternBinding> bindings)
        {
            return Tests.Not.Create(MakeTestsAndBindings(input, neg.Negated, bindings));
        }

        private Tests MakeTestsAndBindingsForBinaryPattern(BoundDagTemp input, BoundBinaryPattern bin, out BoundDagTemp output, ArrayBuilder<BoundPatternBinding> bindings)
        {
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(2);
            if (bin.Disjunction)
            {
                instance.Add(MakeTestsAndBindings(input, bin.Left, bindings));
                instance.Add(MakeTestsAndBindings(input, bin.Right, bindings));
                Tests tests = Tests.OrSequence.Create(instance);
                if (bin.InputType.Equals(bin.NarrowedType))
                {
                    output = input;
                    return tests;
                }
                instance = ArrayBuilder<Tests>.GetInstance(2);
                instance.Add(tests);
                output = MakeConvertToType(input, bin.Syntax, bin.NarrowedType, isExplicitTest: false, instance);
                return Tests.AndSequence.Create(instance);
            }
            instance.Add(MakeTestsAndBindings(input, bin.Left, out var output2, bindings));
            instance.Add(MakeTestsAndBindings(output2, bin.Right, out var output3, bindings));
            output = output3;
            return Tests.AndSequence.Create(instance);
        }

        private Tests MakeTestsAndBindingsForRelationalPattern(BoundDagTemp input, BoundRelationalPattern rel, out BoundDagTemp output)
        {
            ArrayBuilder<Tests> instance = ArrayBuilder<Tests>.GetInstance(2);
            output = MakeConvertToType(input, rel.Syntax, rel.Value.Type, isExplicitTest: false, instance);
            IValueSet valueSet = ValueSetFactory.ForType(input.Type)?.Related(rel.Relation.Operator(), rel.ConstantValue);
            if (valueSet != null && valueSet.IsEmpty)
            {
                instance.Add(Tests.False.Instance);
            }
            else if (valueSet == null || !valueSet.Complement().IsEmpty)
            {
                instance.Add(new Tests.One(new BoundDagRelationalTest(rel.Syntax, rel.Relation, rel.ConstantValue, output, rel.HasErrors)));
            }
            return Tests.AndSequence.Create(instance);
        }

        private TypeSymbol ErrorType(string name = "")
        {
            return new ExtendedErrorTypeSymbol(_compilation, name, 0, null);
        }

        private BoundDecisionDag MakeBoundDecisionDag(SyntaxNode syntax, ImmutableArray<StateForCase> cases)
        {
            DecisionDag decisionDag = MakeDecisionDag(cases);
            BoundLeafDecisionDagNode defaultDecision = new BoundLeafDecisionDagNode(syntax, _defaultLabel);
            ComputeBoundDecisionDagNodes(decisionDag, defaultDecision);
            BoundDecisionDagNode dag = decisionDag.RootNode.Dag;
            return new BoundDecisionDag(dag.Syntax, dag);
        }

        private DecisionDag MakeDecisionDag(ImmutableArray<StateForCase> casesForRootNode)
        {
            ArrayBuilder<DagState> workList = ArrayBuilder<DagState>.GetInstance();
            Dictionary<DagState, DagState> uniqueState = new Dictionary<DagState, DagState>(DagStateEquivalence.Instance);
            ArrayBuilder<StateForCase> instance = ArrayBuilder<StateForCase>.GetInstance(casesForRootNode.Length);
            ImmutableArray<StateForCase>.Enumerator enumerator = casesForRootNode.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StateForCase current = enumerator.Current;
                if (!current.IsImpossible)
                {
                    instance.Add(current);
                    if (current.IsFullyMatched)
                    {
                        break;
                    }
                }
            }
            DagState rootNode = uniqifyState(instance.ToImmutableAndFree(), ImmutableDictionary<BoundDagTemp, IValueSet>.Empty);
            while (workList.Count != 0)
            {
                DagState dagState = workList.Pop();
                if (dagState.Cases.IsDefaultOrEmpty)
                {
                    continue;
                }
                StateForCase stateForCase = dagState.Cases[0];
                if (stateForCase.PatternIsSatisfied)
                {
                    if (!stateForCase.IsFullyMatched)
                    {
                        ImmutableArray<StateForCase> cases2 = dagState.Cases.RemoveAt(0);
                        dagState.FalseBranch = uniqifyState(cases2, dagState.RemainingValues);
                    }
                    continue;
                }
                BoundDagTest boundDagTest = (dagState.SelectedTest = dagState.ComputeSelectedTest());
                if (!(boundDagTest is BoundDagEvaluation e))
                {
                    if (boundDagTest == null)
                    {
                        throw ExceptionUtilities.UnexpectedValue(boundDagTest.Kind);
                    }
                    bool foundExplicitNullTest = false;
                    SplitCases(dagState.Cases, dagState.RemainingValues, boundDagTest, out var whenTrue, out var whenFalse, out var whenTrueValues, out var whenFalseValues, ref foundExplicitNullTest);
                    dagState.TrueBranch = uniqifyState(whenTrue, whenTrueValues);
                    dagState.FalseBranch = uniqifyState(whenFalse, whenFalseValues);
                    if (foundExplicitNullTest && boundDagTest is BoundDagNonNullTest boundDagNonNullTest && !boundDagNonNullTest.IsExplicitTest)
                    {
                        dagState.SelectedTest = new BoundDagNonNullTest(boundDagNonNullTest.Syntax, isExplicitTest: true, boundDagNonNullTest.Input, boundDagNonNullTest.HasErrors);
                    }
                }
                else
                {
                    dagState.TrueBranch = uniqifyState(RemoveEvaluation(dagState.Cases, e), dagState.RemainingValues);
                }
            }
            workList.Free();
            return new DecisionDag(rootNode);
            DagState uniqifyState(ImmutableArray<StateForCase> cases, ImmutableDictionary<BoundDagTemp, IValueSet> remainingValues)
            {
                DagState dagState2 = new DagState(cases, remainingValues);
                if (uniqueState.TryGetValue(dagState2, out var value))
                {
                    ImmutableDictionary<BoundDagTemp, IValueSet>.Builder newRemainingValues = ImmutableDictionary.CreateBuilder<BoundDagTemp, IValueSet>();
                    foreach (var (key, other) in remainingValues)
                    {
                        if (value.RemainingValues.TryGetValue(key, out var value2))
                        {
                            IValueSet value3 = value2.Union(other);
                            newRemainingValues.Add(key, value3);
                        }
                    }
                    if (value.RemainingValues.Count != newRemainingValues.Count || !value.RemainingValues.All<KeyValuePair<BoundDagTemp, IValueSet>>((KeyValuePair<BoundDagTemp, IValueSet> kv) => newRemainingValues.TryGetValue(kv.Key, out var value4) && kv.Value.Equals(value4)))
                    {
                        value.UpdateRemainingValues(newRemainingValues.ToImmutable());
                        if (!workList.Contains(value))
                        {
                            workList.Push(value);
                        }
                    }
                    return value;
                }
                uniqueState.Add(dagState2, dagState2);
                workList.Push(dagState2);
                return dagState2;
            }
        }

        private void ComputeBoundDecisionDagNodes(DecisionDag decisionDag, BoundLeafDecisionDagNode defaultDecision)
        {
            // Process the states in topological order, leaves first, and assign a BoundDecisionDag to each DagState.
            bool wasAcyclic = decisionDag.TryGetTopologicallySortedReachableStates(out ImmutableArray<DagState> sortedStates);
            if (!wasAcyclic)
            {
                // Since we intend the set of DagState nodes to be acyclic by construction, we do not expect
                // this to occur. Just in case it does due to bugs, we recover gracefully to avoid crashing the
                // compiler in production.  If you find that this happens (the assert fails), please modify the
                // DagState construction process to avoid creating a cyclic state graph.

                // If the dag contains a cycle, return a short-circuit dag instead.
                decisionDag.RootNode.Dag = defaultDecision;
                return;
            }

            // We "intern" the dag nodes, so that we only have a single object representing one
            // semantic node. We do this because different states may end up mapping to the same
            // set of successor states. In this case we merge them when producing the bound state machine.
            var uniqueNodes = PooledDictionary<BoundDecisionDagNode, BoundDecisionDagNode>.GetInstance();
            BoundDecisionDagNode uniqifyDagNode(BoundDecisionDagNode node) => uniqueNodes.GetOrAdd(node, node);

            _ = uniqifyDagNode(defaultDecision);

            for (int i = sortedStates.Length - 1; i >= 0; i--)
            {
                var state = sortedStates[i];
                if (state.Cases.IsDefaultOrEmpty)
                {
                    state.Dag = defaultDecision;
                    continue;
                }

                StateForCase first = state.Cases[0];
                if (first.PatternIsSatisfied)
                {
                    if (first.IsFullyMatched)
                    {
                        // there is no when clause we need to evaluate
                        state.Dag = finalState(first.Syntax, first.CaseLabel, first.Bindings);
                    }
                    else
                    {
                        // The final state here does not need bindings, as they will be performed before evaluating the when clause (see below)
                        BoundDecisionDagNode whenTrue = finalState(first.Syntax, first.CaseLabel, default);
                        BoundDecisionDagNode? whenFalse = state.FalseBranch.Dag;
                        state.Dag = uniqifyDagNode(new BoundWhenDecisionDagNode(first.Syntax, first.Bindings, first.WhenClause, whenTrue, whenFalse));
                    }

                    BoundDecisionDagNode finalState(SyntaxNode syntax, LabelSymbol label, ImmutableArray<BoundPatternBinding> bindings)
                    {
                        BoundDecisionDagNode final = uniqifyDagNode(new BoundLeafDecisionDagNode(syntax, label));
                        return bindings.IsDefaultOrEmpty ? final : uniqifyDagNode(new BoundWhenDecisionDagNode(syntax, bindings, null, final, null));
                    }
                }
                else
                {
                    switch (state.SelectedTest)
                    {
                        case BoundDagEvaluation e:
                            {
                                BoundDecisionDagNode? next = state.TrueBranch!.Dag;
                                state.Dag = uniqifyDagNode(new BoundEvaluationDecisionDagNode(e.Syntax, e, next));
                            }
                            break;
                        case BoundDagTest d:
                            {
                                BoundDecisionDagNode? whenTrue = state.TrueBranch!.Dag;
                                BoundDecisionDagNode? whenFalse = state.FalseBranch!.Dag;
                                state.Dag = uniqifyDagNode(new BoundTestDecisionDagNode(d.Syntax, d, whenTrue, whenFalse));
                            }
                            break;
                        case var n:
                            throw ExceptionUtilities.UnexpectedValue(n?.Kind);
                    }
                }
            }

            uniqueNodes.Free();
        }

        private void SplitCase(StateForCase stateForCase, BoundDagTest test, IValueSet? whenTrueValues, IValueSet? whenFalseValues, out StateForCase whenTrue, out StateForCase whenFalse, ref bool foundExplicitNullTest)
        {
            StateForCase stateForCase2 = stateForCase;
            stateForCase2.RemainingTests.Filter(this, test, whenTrueValues, whenFalseValues, out var whenTrue2, out var whenFalse2, ref foundExplicitNullTest);
            whenTrue = makeNext(whenTrue2);
            whenFalse = makeNext(whenFalse2);
            StateForCase makeNext(Tests remainingTests)
            {
                if (!remainingTests.Equals(stateForCase2.RemainingTests))
                {
                    return new StateForCase(stateForCase2.Index, stateForCase2.Syntax, remainingTests, stateForCase2.Bindings, stateForCase2.WhenClause, stateForCase2.CaseLabel);
                }
                return stateForCase2;
            }
        }

        private void SplitCases(ImmutableArray<StateForCase> statesForCases, ImmutableDictionary<BoundDagTemp, IValueSet> values, BoundDagTest test, out ImmutableArray<StateForCase> whenTrue, out ImmutableArray<StateForCase> whenFalse, out ImmutableDictionary<BoundDagTemp, IValueSet> whenTrueValues, out ImmutableDictionary<BoundDagTemp, IValueSet> whenFalseValues, ref bool foundExplicitNullTest)
        {
            ArrayBuilder<StateForCase> instance = ArrayBuilder<StateForCase>.GetInstance(statesForCases.Length);
            ArrayBuilder<StateForCase> instance2 = ArrayBuilder<StateForCase>.GetInstance(statesForCases.Length);
            (ImmutableDictionary<BoundDagTemp, IValueSet>, ImmutableDictionary<BoundDagTemp, IValueSet>, bool, bool) tuple = SplitValues(values, test);
            whenTrueValues = tuple.Item1;
            whenFalseValues = tuple.Item2;
            bool item = tuple.Item3;
            bool item2 = tuple.Item4;
            ImmutableArray<StateForCase>.Enumerator enumerator = statesForCases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StateForCase current = enumerator.Current;
                SplitCase(current, test, whenTrueValues.TryGetValue(test.Input, out var value) ? value : null, whenFalseValues.TryGetValue(test.Input, out var value2) ? value2 : null, out var whenTrue2, out var whenFalse2, ref foundExplicitNullTest);
                if (item && !whenTrue2.IsImpossible && (!instance.Any() || !instance.Last().IsFullyMatched))
                {
                    instance.Add(whenTrue2);
                }
                if (item2 && !whenFalse2.IsImpossible && (!instance2.Any() || !instance2.Last().IsFullyMatched))
                {
                    instance2.Add(whenFalse2);
                }
            }
            whenTrue = instance.ToImmutableAndFree();
            whenFalse = instance2.ToImmutableAndFree();
        }

        private static (ImmutableDictionary<BoundDagTemp, IValueSet> whenTrueValues, ImmutableDictionary<BoundDagTemp, IValueSet> whenFalseValues, bool truePossible, bool falsePossible) SplitValues(ImmutableDictionary<BoundDagTemp, IValueSet> values, BoundDagTest test)
        {
            BoundDagTest test2 = test;
            ImmutableDictionary<BoundDagTemp, IValueSet> values2 = values;
            if (!(test2 is BoundDagEvaluation) && !(test2 is BoundDagExplicitNullTest) && !(test2 is BoundDagNonNullTest) && !(test2 is BoundDagTypeTest))
            {
                if (!(test2 is BoundDagValueTest boundDagValueTest))
                {
                    if (test2 is BoundDagRelationalTest boundDagRelationalTest)
                    {
                        return resultForRelation(boundDagRelationalTest.Relation, boundDagRelationalTest.Value);
                    }
                    throw ExceptionUtilities.UnexpectedValue(test2);
                }
                return resultForRelation(BinaryOperatorKind.Equal, boundDagValueTest.Value);
            }
            return (values2, values2, true, true);
            (ImmutableDictionary<BoundDagTemp, IValueSet> whenTrueValues, ImmutableDictionary<BoundDagTemp, IValueSet> whenFalseValues, bool truePossible, bool falsePossible) resultForRelation(BinaryOperatorKind relation, ConstantValue value)
            {
                BoundDagTemp input = test2.Input;
                IValueSetFactory valueSetFactory = ValueSetFactory.ForType(input.Type);
                if (valueSetFactory == null || value.IsBad)
                {
                    return (values2, values2, true, true);
                }
                IValueSet valueSet = valueSetFactory.Related(relation.Operator(), value);
                IValueSet valueSet2 = valueSet.Complement();
                if (values2.TryGetValue(test2.Input, out var value2))
                {
                    valueSet = valueSet.Intersect(value2);
                    valueSet2 = valueSet2.Intersect(value2);
                }
                ImmutableDictionary<BoundDagTemp, IValueSet> item = values2.SetItem(input, valueSet);
                ImmutableDictionary<BoundDagTemp, IValueSet> item2 = values2.SetItem(input, valueSet2);
                return (item, item2, !valueSet.IsEmpty, !valueSet2.IsEmpty);
            }
        }

        private static ImmutableArray<StateForCase> RemoveEvaluation(ImmutableArray<StateForCase> cases, BoundDagEvaluation e)
        {
            ArrayBuilder<StateForCase> instance = ArrayBuilder<StateForCase>.GetInstance(cases.Length);
            ImmutableArray<StateForCase>.Enumerator enumerator = cases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StateForCase current = enumerator.Current;
                Tests tests = current.RemainingTests.RemoveEvaluation(e);
                if (!(tests is Tests.False))
                {
                    instance.Add(new StateForCase(current.Index, current.Syntax, tests, current.Bindings, current.WhenClause, current.CaseLabel));
                }
            }
            return instance.ToImmutableAndFree();
        }

        private void CheckConsistentDecision(BoundDagTest test, BoundDagTest other, IValueSet? whenTrueValues, IValueSet? whenFalseValues, SyntaxNode syntax, out bool trueTestPermitsTrueOther, out bool falseTestPermitsTrueOther, out bool trueTestImpliesTrueOther, out bool falseTestImpliesTrueOther, ref bool foundExplicitNullTest)
        {
            BoundDagTest test2 = test;
            BoundDagTest other2 = other;
            IValueSet whenTrueValues2 = whenTrueValues;
            IValueSet whenFalseValues2 = whenFalseValues;
            trueTestPermitsTrueOther = true;
            falseTestPermitsTrueOther = true;
            trueTestImpliesTrueOther = false;
            falseTestImpliesTrueOther = false;
            if (!test2.Input.Equals(other2.Input))
            {
                return;
            }
            if (!(test2 is BoundDagNonNullTest))
            {
                if (!(test2 is BoundDagTypeTest boundDagTypeTest))
                {
                    if (!(test2 is BoundDagValueTest) && !(test2 is BoundDagRelationalTest))
                    {
                        if (!(test2 is BoundDagExplicitNullTest))
                        {
                            return;
                        }
                        foundExplicitNullTest = true;
                        if (!(other2 is BoundDagNonNullTest boundDagNonNullTest))
                        {
                            if (!(other2 is BoundDagTypeTest))
                            {
                                if (!(other2 is BoundDagExplicitNullTest))
                                {
                                    if (other2 is BoundDagValueTest)
                                    {
                                        trueTestPermitsTrueOther = false;
                                    }
                                }
                                else
                                {
                                    foundExplicitNullTest = true;
                                    trueTestImpliesTrueOther = true;
                                    falseTestPermitsTrueOther = false;
                                }
                            }
                            else
                            {
                                trueTestPermitsTrueOther = false;
                            }
                        }
                        else
                        {
                            if (boundDagNonNullTest.IsExplicitTest)
                            {
                                foundExplicitNullTest = true;
                            }
                            trueTestPermitsTrueOther = false;
                            falseTestImpliesTrueOther = true;
                        }
                    }
                    else if (!(other2 is BoundDagNonNullTest boundDagNonNullTest2))
                    {
                        if (other2 is BoundDagTypeTest)
                        {
                            return;
                        }
                        if (!(other2 is BoundDagExplicitNullTest))
                        {
                            if (!(other2 is BoundDagRelationalTest boundDagRelationalTest))
                            {
                                if (other2 is BoundDagValueTest boundDagValueTest)
                                {
                                    handleRelationWithValue(BinaryOperatorKind.Equal, boundDagValueTest.Value, out trueTestPermitsTrueOther, out falseTestPermitsTrueOther, out trueTestImpliesTrueOther, out falseTestImpliesTrueOther);
                                }
                            }
                            else
                            {
                                handleRelationWithValue(boundDagRelationalTest.Relation, boundDagRelationalTest.Value, out trueTestPermitsTrueOther, out falseTestPermitsTrueOther, out trueTestImpliesTrueOther, out falseTestImpliesTrueOther);
                            }
                        }
                        else
                        {
                            foundExplicitNullTest = true;
                            trueTestPermitsTrueOther = false;
                        }
                    }
                    else
                    {
                        if (boundDagNonNullTest2.IsExplicitTest)
                        {
                            foundExplicitNullTest = true;
                        }
                        trueTestImpliesTrueOther = true;
                    }
                }
                else if (!(other2 is BoundDagNonNullTest boundDagNonNullTest3))
                {
                    if (!(other2 is BoundDagTypeTest boundDagTypeTest2))
                    {
                        if (!(other2 is BoundDagValueTest) && other2 is BoundDagExplicitNullTest)
                        {
                            foundExplicitNullTest = true;
                            trueTestPermitsTrueOther = false;
                        }
                        return;
                    }
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(_diagnostics, _compilation.Assembly);
                    bool? flag = ExpressionOfTypeMatchesPatternTypeForLearningFromSuccessfulTypeTest(boundDagTypeTest.Type, boundDagTypeTest2.Type, ref useSiteInfo);
                    if (flag == false)
                    {
                        trueTestPermitsTrueOther = false;
                    }
                    else if (flag == true)
                    {
                        trueTestImpliesTrueOther = true;
                    }
                    flag = Binder.ExpressionOfTypeMatchesPatternType(_conversions, boundDagTypeTest2.Type, boundDagTypeTest.Type, ref useSiteInfo, out var _);
                    _diagnostics.Add(syntax, useSiteInfo);
                    if (flag == true)
                    {
                        falseTestPermitsTrueOther = false;
                    }
                }
                else
                {
                    if (boundDagNonNullTest3.IsExplicitTest)
                    {
                        foundExplicitNullTest = true;
                    }
                    trueTestImpliesTrueOther = true;
                }
            }
            else if (!(other2 is BoundDagValueTest))
            {
                if (!(other2 is BoundDagExplicitNullTest))
                {
                    if (other2 is BoundDagNonNullTest boundDagNonNullTest4)
                    {
                        if (boundDagNonNullTest4.IsExplicitTest)
                        {
                            foundExplicitNullTest = true;
                        }
                        trueTestImpliesTrueOther = true;
                        falseTestPermitsTrueOther = false;
                    }
                    else
                    {
                        falseTestPermitsTrueOther = false;
                    }
                }
                else
                {
                    foundExplicitNullTest = true;
                    trueTestPermitsTrueOther = false;
                    falseTestImpliesTrueOther = true;
                }
            }
            else
            {
                falseTestPermitsTrueOther = false;
            }
            void handleRelationWithValue(BinaryOperatorKind relation, ConstantValue value, out bool trueTestPermitsTrueOther, out bool falseTestPermitsTrueOther, out bool trueTestImpliesTrueOther, out bool falseTestImpliesTrueOther)
            {
                bool flag2 = test2.Equals(other2);
                trueTestPermitsTrueOther = whenTrueValues2?.Any(relation, value) ?? true;
                trueTestImpliesTrueOther = flag2 || (trueTestPermitsTrueOther && (whenTrueValues2?.All(relation, value) ?? false));
                falseTestPermitsTrueOther = !flag2 && (whenFalseValues2?.Any(relation, value) ?? true);
                falseTestImpliesTrueOther = falseTestPermitsTrueOther && (whenFalseValues2?.All(relation, value) ?? false);
            }
        }

        private bool? ExpressionOfTypeMatchesPatternTypeForLearningFromSuccessfulTypeTest(TypeSymbol expressionType, TypeSymbol patternType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool? result = Binder.ExpressionOfTypeMatchesPatternType(_conversions, expressionType, patternType, ref useSiteInfo, out Conversion conversion);
            if (conversion.Exists || !isRuntimeSimilar(expressionType, patternType))
            {
                return result;
            }
            return null;
            static bool isRuntimeSimilar(TypeSymbol expressionType, TypeSymbol patternType)
            {
                TypeSymbol elementType;
                TypeSymbol elementType2;
                TypeSymbol typeSymbol;
                TypeSymbol typeSymbol2;
                for (; expressionType is ArrayTypeSymbol arrayTypeSymbol; typeSymbol = elementType, typeSymbol2 = elementType2, expressionType = typeSymbol, patternType = typeSymbol2)
                {
                    elementType = arrayTypeSymbol.ElementType;
                    bool isSZArray = arrayTypeSymbol.IsSZArray;
                    int rank = arrayTypeSymbol.Rank;
                    if (!(patternType is ArrayTypeSymbol arrayTypeSymbol2))
                    {
                        break;
                    }
                    elementType2 = arrayTypeSymbol2.ElementType;
                    bool isSZArray2 = arrayTypeSymbol2.IsSZArray;
                    int rank2 = arrayTypeSymbol2.Rank;
                    if (isSZArray != isSZArray2 || rank != rank2)
                    {
                        break;
                    }
                    elementType = elementType.EnumUnderlyingTypeOrSelf();
                    elementType2 = elementType2.EnumUnderlyingTypeOrSelf();
                    SpecialType specialType = elementType.SpecialType;
                    SpecialType specialType2 = elementType2.SpecialType;
                    if (specialType != specialType2)
                    {
                        switch (specialType)
                        {
                            case SpecialType.System_SByte:
                                if (specialType2 != SpecialType.System_Byte)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_Byte:
                                if (specialType2 != SpecialType.System_SByte)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_Int16:
                                if (specialType2 != SpecialType.System_UInt16)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_UInt16:
                                if (specialType2 != SpecialType.System_Int16)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_Int32:
                                if (specialType2 != SpecialType.System_UInt32 && (uint)(specialType2 - 21) > 1u)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_UInt32:
                                if (specialType2 != SpecialType.System_Int32 && (uint)(specialType2 - 21) > 1u)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_Int64:
                                if (specialType2 != SpecialType.System_UInt64 && (uint)(specialType2 - 21) > 1u)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_UInt64:
                                if (specialType2 != SpecialType.System_Int64 && (uint)(specialType2 - 21) > 1u)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_IntPtr:
                                if ((uint)(specialType2 - 13) > 3u && specialType2 != SpecialType.System_UIntPtr)
                                {
                                    continue;
                                }
                                break;
                            case SpecialType.System_UIntPtr:
                                if ((uint)(specialType2 - 13) > 3u && specialType2 != SpecialType.System_IntPtr)
                                {
                                    continue;
                                }
                                break;
                            default:
                                continue;
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
