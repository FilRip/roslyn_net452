using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class SwitchBinder : LocalScopeBinder
    {
        protected readonly SwitchStatementSyntax SwitchSyntax;

        private readonly GeneratedLabelSymbol _breakLabel;

        private BoundExpression _switchGoverningExpression;

        private BindingDiagnosticBag _switchGoverningDiagnostics;

        private Dictionary<object, SourceLabelSymbol> _lazySwitchLabelsMap;

        private static readonly object s_defaultKey = new object();

        private static readonly object s_nullKey = new object();

        private Dictionary<SyntaxNode, LabelSymbol> _labelsByNode;

        protected bool PatternsEnabled => ((CSharpParseOptions)SwitchSyntax.SyntaxTree.Options)?.IsFeatureEnabled(MessageID.IDS_FeaturePatternMatching) ?? true;

        protected BoundExpression SwitchGoverningExpression
        {
            get
            {
                EnsureSwitchGoverningExpressionAndDiagnosticsBound();
                return _switchGoverningExpression;
            }
        }

        protected TypeSymbol SwitchGoverningType => SwitchGoverningExpression.Type;

        protected uint SwitchGoverningValEscape => Binder.GetValEscape(SwitchGoverningExpression, LocalScopeDepth);

        protected BindingDiagnosticBag SwitchGoverningDiagnostics
        {
            get
            {
                EnsureSwitchGoverningExpressionAndDiagnosticsBound();
                return _switchGoverningDiagnostics;
            }
        }

        private Dictionary<object, SourceLabelSymbol> LabelsByValue
        {
            get
            {
                if (_lazySwitchLabelsMap == null && Labels.Length > 0)
                {
                    _lazySwitchLabelsMap = BuildLabelsByValue(Labels);
                }
                return _lazySwitchLabelsMap;
            }
        }

        internal override bool IsLocalFunctionsScopeBinder => true;

        internal override GeneratedLabelSymbol BreakLabel => _breakLabel;

        internal override bool IsLabelsScopeBinder => true;

        internal override SyntaxNode ScopeDesignator => SwitchSyntax;

        protected Dictionary<SyntaxNode, LabelSymbol> LabelsByNode
        {
            get
            {
                if (_labelsByNode == null)
                {
                    Dictionary<SyntaxNode, LabelSymbol> dictionary = new Dictionary<SyntaxNode, LabelSymbol>();
                    ImmutableArray<LabelSymbol>.Enumerator enumerator = Labels.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        LabelSymbol current = enumerator.Current;
                        SyntaxNode syntaxNode = ((SourceLabelSymbol)current).IdentifierNodeOrToken.AsNode();
                        if (syntaxNode != null)
                        {
                            dictionary.Add(syntaxNode, current);
                        }
                    }
                    _labelsByNode = dictionary;
                }
                return _labelsByNode;
            }
        }

        private SwitchBinder(Binder next, SwitchStatementSyntax switchSyntax)
            : base(next)
        {
            SwitchSyntax = switchSyntax;
            _breakLabel = new GeneratedLabelSymbol("break");
        }

        private void EnsureSwitchGoverningExpressionAndDiagnosticsBound()
        {
            if (_switchGoverningExpression == null)
            {
                BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag();
                BoundExpression value = BindSwitchGoverningExpression(bindingDiagnosticBag);
                _switchGoverningDiagnostics = bindingDiagnosticBag;
                Interlocked.CompareExchange(ref _switchGoverningExpression, value, null);
            }
        }

        private static Dictionary<object, SourceLabelSymbol> BuildLabelsByValue(ImmutableArray<LabelSymbol> labels)
        {
            Dictionary<object, SourceLabelSymbol> dictionary = new Dictionary<object, SourceLabelSymbol>(labels.Length, new SwitchConstantValueHelper.SwitchLabelsComparer());
            ImmutableArray<LabelSymbol>.Enumerator enumerator = labels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SourceLabelSymbol sourceLabelSymbol = (SourceLabelSymbol)enumerator.Current;
                SyntaxKind syntaxKind = sourceLabelSymbol.IdentifierNodeOrToken.Kind();
                if (syntaxKind != SyntaxKind.IdentifierToken)
                {
                    ConstantValue switchCaseLabelConstant = sourceLabelSymbol.SwitchCaseLabelConstant;
                    object key = (((object)switchCaseLabelConstant != null && !switchCaseLabelConstant.IsBad) ? KeyForConstant(switchCaseLabelConstant) : ((syntaxKind != SyntaxKind.DefaultSwitchLabel) ? sourceLabelSymbol.IdentifierNodeOrToken.AsNode() : s_defaultKey));
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, sourceLabelSymbol);
                    }
                }
            }
            return dictionary;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = SwitchSyntax.Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchSectionSyntax current = enumerator.Current;
                instance.AddRange(BuildLocals(current.Statements, GetBinder(current)));
            }
            return instance.ToImmutableAndFree();
        }

        protected override ImmutableArray<LocalFunctionSymbol> BuildLocalFunctions()
        {
            ArrayBuilder<LocalFunctionSymbol> instance = ArrayBuilder<LocalFunctionSymbol>.GetInstance();
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = SwitchSyntax.Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchSectionSyntax current = enumerator.Current;
                instance.AddRange(BuildLocalFunctions(current.Statements));
            }
            return instance.ToImmutableAndFree();
        }

        protected override ImmutableArray<LabelSymbol> BuildLabels()
        {
            ArrayBuilder<LabelSymbol> labels = ArrayBuilder<LabelSymbol>.GetInstance();
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = SwitchSyntax.Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchSectionSyntax current = enumerator.Current;
                BuildSwitchLabels(current.Labels, GetBinder(current), labels, BindingDiagnosticBag.Discarded);
                BuildLabels(current.Statements, ref labels);
            }
            return labels.ToImmutableAndFree();
        }

        private void BuildSwitchLabels(SyntaxList<SwitchLabelSyntax> labelsSyntax, Binder sectionBinder, ArrayBuilder<LabelSymbol> labels, BindingDiagnosticBag tempDiagnosticBag)
        {
            SyntaxList<SwitchLabelSyntax>.Enumerator enumerator = labelsSyntax.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchLabelSyntax current = enumerator.Current;
                ConstantValue constantValueOpt = null;
                switch (current.Kind())
                {
                    case SyntaxKind.CaseSwitchLabel:
                        {
                            CaseSwitchLabelSyntax caseSwitchLabelSyntax = (CaseSwitchLabelSyntax)current;
                            BoundExpression boundExpression = sectionBinder.BindTypeOrRValue(caseSwitchLabelSyntax.Value, tempDiagnosticBag);
                            if (!(boundExpression is BoundTypeExpression))
                            {
                                ConvertCaseExpression(current, boundExpression, sectionBinder, out constantValueOpt, tempDiagnosticBag);
                            }
                            break;
                        }
                    case SyntaxKind.CasePatternSwitchLabel:
                        {
                            CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = (CasePatternSwitchLabelSyntax)current;
                            sectionBinder.BindPattern(casePatternSwitchLabelSyntax.Pattern, SwitchGoverningType, SwitchGoverningValEscape, permitDesignations: true, current.HasErrors, tempDiagnosticBag);
                            break;
                        }
                }
                labels.Add(new SourceLabelSymbol((MethodSymbol)ContainingMemberOrLambda, current, constantValueOpt));
            }
        }

        protected BoundExpression ConvertCaseExpression(CSharpSyntaxNode node, BoundExpression caseExpression, Binder sectionBinder, out ConstantValue constantValueOpt, BindingDiagnosticBag diagnostics, bool isGotoCaseExpr = false)
        {
            bool hasErrors = false;
            if (isGotoCaseExpr)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                Conversion conversion = base.Conversions.ClassifyConversionFromExpression(caseExpression, SwitchGoverningType, ref useSiteInfo);
                diagnostics.Add(node, useSiteInfo);
                if (!conversion.IsValid)
                {
                    GenerateImplicitConversionError(diagnostics, node, conversion, caseExpression, SwitchGoverningType);
                    hasErrors = true;
                }
                else if (!conversion.IsImplicit)
                {
                    diagnostics.Add(ErrorCode.WRN_GotoCaseShouldConvert, node.Location, SwitchGoverningType);
                    hasErrors = true;
                }
                caseExpression = CreateConversion(caseExpression, conversion, SwitchGoverningType, diagnostics);
            }
            return ConvertPatternExpression(SwitchGoverningType, node, caseExpression, out constantValueOpt, hasErrors, diagnostics);
        }

        protected static object KeyForConstant(ConstantValue constantValue)
        {
            if (!constantValue.IsNull)
            {
                return constantValue.Value;
            }
            return s_nullKey;
        }

        protected SourceLabelSymbol FindMatchingSwitchCaseLabel(ConstantValue constantValue, CSharpSyntaxNode labelSyntax)
        {
            object key = (((object)constantValue == null || constantValue.IsBad) ? labelSyntax : KeyForConstant(constantValue));
            return FindMatchingSwitchLabel(key);
        }

        private SourceLabelSymbol GetDefaultLabel()
        {
            return FindMatchingSwitchLabel(s_defaultKey);
        }

        private SourceLabelSymbol FindMatchingSwitchLabel(object key)
        {
            Dictionary<object, SourceLabelSymbol> labelsByValue = LabelsByValue;
            if (labelsByValue != null && labelsByValue.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (SwitchSyntax == scopeDesignator)
            {
                return Locals;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            if (SwitchSyntax == scopeDesignator)
            {
                return LocalFunctions;
            }
            throw ExceptionUtilities.Unreachable;
        }

        private BoundExpression BindSwitchGoverningExpression(BindingDiagnosticBag diagnostics)
        {
            ExpressionSyntax expression = SwitchSyntax.Expression;
            Binder binder = GetBinder(expression);
            BoundExpression boundExpression = binder.BindRValueWithoutTargetType(expression, diagnostics);
            TypeSymbol typeSymbol = boundExpression.Type;
            if ((object)typeSymbol != null && !typeSymbol.IsErrorType())
            {
                if (typeSymbol.IsValidV6SwitchGoverningType())
                {
                    if (typeSymbol.SpecialType == SpecialType.System_Boolean)
                    {
                        Binder.CheckFeatureAvailability(expression, MessageID.IDS_FeatureSwitchOnBool, diagnostics);
                    }
                    return boundExpression;
                }
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                Conversion conversion = binder.Conversions.ClassifyImplicitUserDefinedConversionForV6SwitchGoverningType(typeSymbol, out TypeSymbol switchGoverningType, ref useSiteInfo);
                diagnostics.Add(expression, useSiteInfo);
                if (conversion.IsValid)
                {
                    return binder.CreateConversion(expression, boundExpression, conversion, isCast: false, null, switchGoverningType, diagnostics);
                }
                if (!typeSymbol.IsVoidType())
                {
                    if (!PatternsEnabled)
                    {
                        diagnostics.Add(ErrorCode.ERR_V6SwitchGoverningTypeValueExpected, expression.Location);
                    }
                    return boundExpression;
                }
                typeSymbol = CreateErrorType(typeSymbol.Name);
            }
            if (!boundExpression.HasAnyErrors)
            {
                diagnostics.Add(ErrorCode.ERR_SwitchExpressionValueExpected, expression.Location, boundExpression.Display);
            }
            return new BoundBadExpression(expression, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(boundExpression), typeSymbol ?? CreateErrorType());
        }

        internal BoundStatement BindGotoCaseOrDefault(GotoStatementSyntax node, Binder gotoBinder, BindingDiagnosticBag diagnostics)
        {
            BoundExpression boundExpression = null;
            if (!node.HasErrors)
            {
                ConstantValue constantValueOpt = null;
                bool flag = false;
                SourceLabelSymbol sourceLabelSymbol;
                if (node.Expression != null)
                {
                    boundExpression = gotoBinder.BindValue(node.Expression, diagnostics, BindValueKind.RValue);
                    boundExpression = ConvertCaseExpression(node, boundExpression, gotoBinder, out constantValueOpt, diagnostics, isGotoCaseExpr: true);
                    flag = flag || boundExpression.HasAnyErrors;
                    if (!flag && constantValueOpt == null)
                    {
                        diagnostics.Add(ErrorCode.ERR_ConstantExpected, node.Location);
                        flag = true;
                    }
                    ConstantValueUtils.CheckLangVersionForConstantValue(boundExpression, diagnostics);
                    sourceLabelSymbol = FindMatchingSwitchCaseLabel(constantValueOpt, node);
                }
                else
                {
                    sourceLabelSymbol = GetDefaultLabel();
                }
                if ((object)sourceLabelSymbol != null)
                {
                    return new BoundGotoStatement(node, sourceLabelSymbol, boundExpression, null, flag);
                }
                if (!flag)
                {
                    string text = SyntaxFacts.GetText(node.CaseOrDefaultKeyword.Kind());
                    if (node.Kind() == SyntaxKind.GotoCaseStatement)
                    {
                        text = text + " " + constantValueOpt.Value;
                    }
                    text += ":";
                    diagnostics.Add(ErrorCode.ERR_LabelNotFound, node.Location, text);
                    flag = true;
                }
            }
            return new BoundBadStatement(node, (boundExpression != null) ? ImmutableArray.Create((BoundNode)boundExpression) : ImmutableArray<BoundNode>.Empty, hasErrors: true);
        }

        internal static SwitchBinder Create(Binder next, SwitchStatementSyntax switchSyntax)
        {
            return new SwitchBinder(next, switchSyntax);
        }

        internal override BoundStatement BindSwitchStatementCore(SwitchStatementSyntax node, Binder originalBinder, BindingDiagnosticBag diagnostics)
        {
            if (node.Sections.Count == 0)
            {
                diagnostics.Add(ErrorCode.WRN_EmptySwitch, node.OpenBraceToken.GetLocation());
            }
            BoundExpression switchGoverningExpression = SwitchGoverningExpression;
            diagnostics.AddRange(SwitchGoverningDiagnostics, allowMismatchInDependencyAccumulation: true);
            ImmutableArray<BoundSwitchSection> switchSections = BindSwitchSections(originalBinder, diagnostics, out BoundSwitchLabel defaultLabel);
            ImmutableArray<LocalSymbol> declaredLocalsForScope = GetDeclaredLocalsForScope(node);
            ImmutableArray<LocalFunctionSymbol> declaredLocalFunctionsForScope = GetDeclaredLocalFunctionsForScope(node);
            BoundDecisionDag boundDecisionDag = DecisionDagBuilder.CreateDecisionDagForSwitchStatement(base.Compilation, node, switchGoverningExpression, switchSections, defaultLabel?.Label ?? BreakLabel, diagnostics);
            CheckSwitchErrors(node, switchGoverningExpression, ref switchSections, boundDecisionDag, diagnostics);
            boundDecisionDag = boundDecisionDag.SimplifyDecisionDagIfConstantInput(switchGoverningExpression);
            ImmutableArray<BoundSwitchSection> switchSections2 = switchSections;
            BoundSwitchLabel defaultLabel2 = defaultLabel;
            GeneratedLabelSymbol breakLabel = BreakLabel;
            return new BoundSwitchStatement(node, switchGoverningExpression, declaredLocalsForScope, declaredLocalFunctionsForScope, switchSections2, boundDecisionDag, defaultLabel2, breakLabel);
        }

        private void CheckSwitchErrors(SwitchStatementSyntax node, BoundExpression boundSwitchGoverningExpression, ref ImmutableArray<BoundSwitchSection> switchSections, BoundDecisionDag decisionDag, BindingDiagnosticBag diagnostics)
        {
            ImmutableHashSet<LabelSymbol> reachableLabels = decisionDag.ReachableLabels;
            if (!switchSections.Any((BoundSwitchSection s) => s.SwitchLabels.Any((BoundSwitchLabel l) => isSubsumed(l))))
            {
                return;
            }
            ArrayBuilder<BoundSwitchSection> instance = ArrayBuilder<BoundSwitchSection>.GetInstance(switchSections.Length);
            bool flag = false;
            ImmutableArray<BoundSwitchSection>.Enumerator enumerator = switchSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchSection current = enumerator.Current;
                ArrayBuilder<BoundSwitchLabel> instance2 = ArrayBuilder<BoundSwitchLabel>.GetInstance(current.SwitchLabels.Length);
                ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = current.SwitchLabels.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundSwitchLabel current2 = enumerator2.Current;
                    BoundSwitchLabel item = current2;
                    if (!current2.HasErrors && isSubsumed(current2) && current2.Syntax.Kind() != SyntaxKind.DefaultSwitchLabel)
                    {
                        SyntaxNode syntax = current2.Syntax;
                        if (!(syntax is CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax))
                        {
                            if (!(syntax is CaseSwitchLabelSyntax caseSwitchLabelSyntax))
                            {
                                throw ExceptionUtilities.UnexpectedValue(syntax.Kind());
                            }
                            if (current2.Pattern is BoundConstantPattern boundConstantPattern && !boundConstantPattern.ConstantValue.IsBad && FindMatchingSwitchCaseLabel(boundConstantPattern.ConstantValue, caseSwitchLabelSyntax) != current2.Label)
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateCaseLabel, syntax.Location, boundConstantPattern.ConstantValue.GetValueToDisplay());
                            }
                            else if (!current2.Pattern.HasErrors && !flag)
                            {
                                diagnostics.Add(ErrorCode.ERR_SwitchCaseSubsumed, caseSwitchLabelSyntax.Value.Location);
                            }
                        }
                        else if (!casePatternSwitchLabelSyntax.Pattern.HasErrors && !flag)
                        {
                            diagnostics.Add(ErrorCode.ERR_SwitchCaseSubsumed, casePatternSwitchLabelSyntax.Pattern.Location);
                        }
                        item = new BoundSwitchLabel(current2.Syntax, current2.Label, current2.Pattern, current2.WhenClause, hasErrors: true);
                    }
                    flag |= current2.HasErrors;
                    instance2.Add(item);
                }
                instance.Add(current.Update(current.Locals, instance2.ToImmutableAndFree(), current.Statements));
            }
            switchSections = instance.ToImmutableAndFree();
            bool isSubsumed(BoundSwitchLabel switchLabel)
            {
                return !reachableLabels.Contains(switchLabel.Label);
            }
        }

        internal override void BindPatternSwitchLabelForInference(CasePatternSwitchLabelSyntax node, BindingDiagnosticBag diagnostics)
        {
            BoundSwitchLabel defaultLabel = null;
            BindSwitchSectionLabel(GetBinder(node.Parent), node, LabelsByNode[node], ref defaultLabel, diagnostics);
        }

        private ImmutableArray<BoundSwitchSection> BindSwitchSections(Binder originalBinder, BindingDiagnosticBag diagnostics, out BoundSwitchLabel defaultLabel)
        {
            ArrayBuilder<BoundSwitchSection> instance = ArrayBuilder<BoundSwitchSection>.GetInstance(SwitchSyntax.Sections.Count);
            defaultLabel = null;
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = SwitchSyntax.Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchSectionSyntax current = enumerator.Current;
                BoundSwitchSection item = BindSwitchSection(current, originalBinder, ref defaultLabel, diagnostics);
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        private BoundSwitchSection BindSwitchSection(SwitchSectionSyntax node, Binder originalBinder, ref BoundSwitchLabel defaultLabel, BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<BoundSwitchLabel> instance = ArrayBuilder<BoundSwitchLabel>.GetInstance(node.Labels.Count);
            Binder binder = originalBinder.GetBinder(node);
            Dictionary<SyntaxNode, LabelSymbol> labelsByNode = LabelsByNode;
            SyntaxList<SwitchLabelSyntax>.Enumerator enumerator = node.Labels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SwitchLabelSyntax current = enumerator.Current;
                LabelSymbol label = labelsByNode[current];
                BoundSwitchLabel item = BindSwitchSectionLabel(binder, current, label, ref defaultLabel, diagnostics);
                instance.Add(item);
            }
            ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance(node.Statements.Count);
            SyntaxList<StatementSyntax>.Enumerator enumerator2 = node.Statements.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                StatementSyntax current2 = enumerator2.Current;
                BoundStatement boundStatement = binder.BindStatement(current2, diagnostics);
                if (ContainsUsingVariable(boundStatement))
                {
                    diagnostics.Add(ErrorCode.ERR_UsingVarInSwitchCase, current2.Location);
                }
                instance2.Add(boundStatement);
            }
            return new BoundSwitchSection(node, binder.GetDeclaredLocalsForScope(node), instance.ToImmutableAndFree(), instance2.ToImmutableAndFree());
        }

        internal static bool ContainsUsingVariable(BoundStatement boundStatement)
        {
            if (boundStatement is BoundLocalDeclaration boundLocalDeclaration)
            {
                return boundLocalDeclaration.LocalSymbol.IsUsing;
            }
            if (boundStatement is BoundMultipleLocalDeclarationsBase boundMultipleLocalDeclarationsBase && !boundMultipleLocalDeclarationsBase.LocalDeclarations.IsDefaultOrEmpty)
            {
                return boundMultipleLocalDeclarationsBase.LocalDeclarations[0].LocalSymbol.IsUsing;
            }
            return false;
        }

        private BoundSwitchLabel BindSwitchSectionLabel(Binder sectionBinder, SwitchLabelSyntax node, LabelSymbol label, ref BoundSwitchLabel defaultLabel, BindingDiagnosticBag diagnostics)
        {
            switch (node.Kind())
            {
                case SyntaxKind.CaseSwitchLabel:
                    {
                        CaseSwitchLabelSyntax caseSwitchLabelSyntax = (CaseSwitchLabelSyntax)node;
                        bool hasErrors = node.HasErrors;
                        BoundPattern boundPattern = sectionBinder.BindConstantPatternWithFallbackToTypePattern(caseSwitchLabelSyntax.Value, caseSwitchLabelSyntax.Value, SwitchGoverningType, hasErrors, diagnostics);
                        boundPattern.WasCompilerGenerated = true;
                        reportIfConstantNamedUnderscore(boundPattern, caseSwitchLabelSyntax.Value);
                        return new BoundSwitchLabel(node, label, boundPattern, null, boundPattern.HasErrors);
                    }
                case SyntaxKind.DefaultSwitchLabel:
                    {
                        BoundDiscardPattern boundDiscardPattern = new BoundDiscardPattern(node, SwitchGoverningType, SwitchGoverningType);
                        bool hasErrors2 = boundDiscardPattern.HasErrors;
                        if (defaultLabel != null)
                        {
                            diagnostics.Add(ErrorCode.ERR_DuplicateCaseLabel, node.Location, label.Name);
                            hasErrors2 = true;
                            return new BoundSwitchLabel(node, label, boundDiscardPattern, null, hasErrors2);
                        }
                        return defaultLabel = new BoundSwitchLabel(node, label, boundDiscardPattern, null, hasErrors2);
                    }
                case SyntaxKind.CasePatternSwitchLabel:
                    {
                        CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = (CasePatternSwitchLabelSyntax)node;
                        BoundPattern pattern2 = sectionBinder.BindPattern(casePatternSwitchLabelSyntax.Pattern, SwitchGoverningType, SwitchGoverningValEscape, permitDesignations: true, node.HasErrors, diagnostics);
                        if (casePatternSwitchLabelSyntax.Pattern is ConstantPatternSyntax constantPatternSyntax)
                        {
                            reportIfConstantNamedUnderscore(pattern2, constantPatternSyntax.Expression);
                        }
                        return new BoundSwitchLabel(node, label, pattern2, (casePatternSwitchLabelSyntax.WhenClause != null) ? sectionBinder.BindBooleanExpression(casePatternSwitchLabelSyntax.WhenClause!.Condition, diagnostics) : null, node.HasErrors);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(node);
            }
            void reportIfConstantNamedUnderscore(BoundPattern pattern, ExpressionSyntax expression)
            {
                if (pattern is BoundConstantPattern && !pattern.HasErrors && Binder.IsUnderscore(expression))
                {
                    diagnostics.Add(ErrorCode.WRN_CaseConstantNamedUnderscore, expression.Location);
                }
            }
        }
    }
}
