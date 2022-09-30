using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

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
    public sealed class BoundQueryClause : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Value);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Value { get; }

        public RangeVariableSymbol? DefinedSymbol { get; }

        public BoundExpression? Operation { get; }

        public BoundExpression? Cast { get; }

        public Binder Binder { get; }

        public BoundExpression? UnoptimizedForm { get; }

        public BoundQueryClause(SyntaxNode syntax, BoundExpression value, RangeVariableSymbol? definedSymbol, BoundExpression? operation, BoundExpression? cast, Binder binder, BoundExpression? unoptimizedForm, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.QueryClause, syntax, type, hasErrors || value.HasErrors() || operation.HasErrors() || cast.HasErrors() || unoptimizedForm.HasErrors())
        {
            Value = value;
            DefinedSymbol = definedSymbol;
            Operation = operation;
            Cast = cast;
            Binder = binder;
            UnoptimizedForm = unoptimizedForm;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitQueryClause(this);
        }

        public BoundQueryClause Update(BoundExpression value, RangeVariableSymbol? definedSymbol, BoundExpression? operation, BoundExpression? cast, Binder binder, BoundExpression? unoptimizedForm, TypeSymbol type)
        {
            if (value != Value || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(definedSymbol, DefinedSymbol) || operation != Operation || cast != Cast || binder != Binder || unoptimizedForm != UnoptimizedForm || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundQueryClause boundQueryClause = new BoundQueryClause(Syntax, value, definedSymbol, operation, cast, binder, unoptimizedForm, type, base.HasErrors);
                boundQueryClause.CopyAttributes(this);
                return boundQueryClause;
            }
            return this;
        }
    }
}
