using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundEventAssignmentOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public EventSymbol Event { get; }

        public bool IsAddition { get; }

        public bool IsDynamic { get; }

        public BoundExpression? ReceiverOpt { get; }

        public BoundExpression Argument { get; }

        public BoundEventAssignmentOperator(SyntaxNode syntax, EventSymbol @event, bool isAddition, bool isDynamic, BoundExpression? receiverOpt, BoundExpression argument, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.EventAssignmentOperator, syntax, type, hasErrors || receiverOpt.HasErrors() || argument.HasErrors())
        {
            Event = @event;
            IsAddition = isAddition;
            IsDynamic = isDynamic;
            ReceiverOpt = receiverOpt;
            Argument = argument;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitEventAssignmentOperator(this);
        }

        public BoundEventAssignmentOperator Update(EventSymbol @event, bool isAddition, bool isDynamic, BoundExpression? receiverOpt, BoundExpression argument, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(@event, Event) || isAddition != IsAddition || isDynamic != IsDynamic || receiverOpt != ReceiverOpt || argument != Argument || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundEventAssignmentOperator boundEventAssignmentOperator = new BoundEventAssignmentOperator(Syntax, @event, isAddition, isDynamic, receiverOpt, argument, type, base.HasErrors);
                boundEventAssignmentOperator.CopyAttributes(this);
                return boundEventAssignmentOperator;
            }
            return this;
        }
    }
}
