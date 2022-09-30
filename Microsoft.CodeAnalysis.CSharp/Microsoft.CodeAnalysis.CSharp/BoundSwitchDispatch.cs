using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSwitchDispatch : BoundStatement
    {
        public BoundExpression Expression { get; }

        public ImmutableArray<(ConstantValue value, LabelSymbol label)> Cases { get; }

        public LabelSymbol DefaultLabel { get; }

        public MethodSymbol? EqualityMethod { get; }

        public BoundSwitchDispatch(SyntaxNode syntax, BoundExpression expression, ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, LabelSymbol defaultLabel, MethodSymbol? equalityMethod, bool hasErrors = false)
            : base(BoundKind.SwitchDispatch, syntax, hasErrors || expression.HasErrors())
        {
            Expression = expression;
            Cases = cases;
            DefaultLabel = defaultLabel;
            EqualityMethod = equalityMethod;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSwitchDispatch(this);
        }

        public BoundSwitchDispatch Update(BoundExpression expression, ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, LabelSymbol defaultLabel, MethodSymbol? equalityMethod)
        {
            if (expression != Expression || cases != Cases || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, DefaultLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(equalityMethod, EqualityMethod))
            {
                BoundSwitchDispatch boundSwitchDispatch = new BoundSwitchDispatch(Syntax, expression, cases, defaultLabel, equalityMethod, base.HasErrors);
                boundSwitchDispatch.CopyAttributes(this);
                return boundSwitchDispatch;
            }
            return this;
        }
    }
}
