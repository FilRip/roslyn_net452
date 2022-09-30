using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSwitchSection : BoundStatementList
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundSwitchLabel> SwitchLabels { get; }

        public BoundSwitchSection(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundSwitchLabel> switchLabels, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.SwitchSection, syntax, statements, hasErrors || switchLabels.HasErrors() || statements.HasErrors())
        {
            Locals = locals;
            SwitchLabels = switchLabels;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSwitchSection(this);
        }

        public BoundSwitchSection Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundSwitchLabel> switchLabels, ImmutableArray<BoundStatement> statements)
        {
            if (locals != Locals || switchLabels != SwitchLabels || statements != base.Statements)
            {
                BoundSwitchSection boundSwitchSection = new BoundSwitchSection(Syntax, locals, switchLabels, statements, base.HasErrors);
                boundSwitchSection.CopyAttributes(this);
                return boundSwitchSection;
            }
            return this;
        }
    }
}
