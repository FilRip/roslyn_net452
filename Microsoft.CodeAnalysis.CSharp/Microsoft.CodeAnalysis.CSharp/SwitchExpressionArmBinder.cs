using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class SwitchExpressionArmBinder : Binder
    {
        private readonly SwitchExpressionArmSyntax _arm;

        private readonly ExpressionVariableBinder _armScopeBinder;

        private readonly SwitchExpressionBinder _switchExpressionBinder;

        public SwitchExpressionArmBinder(SwitchExpressionArmSyntax arm, ExpressionVariableBinder armScopeBinder, SwitchExpressionBinder switchExpressionBinder)
            : base(armScopeBinder)
        {
            _arm = arm;
            _armScopeBinder = armScopeBinder;
            _switchExpressionBinder = switchExpressionBinder;
        }

        internal BoundSwitchExpressionArm BindSwitchExpressionArm(SwitchExpressionArmSyntax node, BindingDiagnosticBag diagnostics)
        {
            var (switchGoverningType, switchGoverningValEscape) = _switchExpressionBinder.GetInputTypeAndValEscape();
            return BindSwitchExpressionArm(node, switchGoverningType, switchGoverningValEscape, diagnostics);
        }

        internal override BoundSwitchExpressionArm BindSwitchExpressionArm(SwitchExpressionArmSyntax node, TypeSymbol switchGoverningType, uint switchGoverningValEscape, BindingDiagnosticBag diagnostics)
        {
            Binder requiredBinder = GetRequiredBinder(node);
            bool flag = switchGoverningType.IsErrorType();
            ImmutableArray<LocalSymbol> locals = _armScopeBinder.Locals;
            BoundPattern boundPattern = requiredBinder.BindPattern(node.Pattern, switchGoverningType, switchGoverningValEscape, permitDesignations: true, flag, diagnostics);
            BoundExpression whenClause = ((node.WhenClause != null) ? requiredBinder.BindBooleanExpression(node.WhenClause!.Condition, diagnostics) : null);
            BoundExpression value = requiredBinder.BindValue(node.Expression, diagnostics, BindValueKind.RValue);
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("arm");
            return new BoundSwitchExpressionArm(node, locals, boundPattern, whenClause, value, label, flag | boundPattern.HasErrors);
        }
    }
}
