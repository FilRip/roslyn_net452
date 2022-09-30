using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundEventAccess : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol ExpressionSymbol => EventSymbol;

        public new TypeSymbol Type => base.Type;

        public BoundExpression? ReceiverOpt { get; }

        public EventSymbol EventSymbol { get; }

        public bool IsUsableAsField { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public BoundEventAccess(SyntaxNode syntax, BoundExpression? receiverOpt, EventSymbol eventSymbol, bool isUsableAsField, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.EventAccess, syntax, type, hasErrors || receiverOpt.HasErrors())
        {
            ReceiverOpt = receiverOpt;
            EventSymbol = eventSymbol;
            IsUsableAsField = isUsableAsField;
            _ResultKind = resultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitEventAccess(this);
        }

        public BoundEventAccess Update(BoundExpression? receiverOpt, EventSymbol eventSymbol, bool isUsableAsField, LookupResultKind resultKind, TypeSymbol type)
        {
            if (receiverOpt != ReceiverOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(eventSymbol, EventSymbol) || isUsableAsField != IsUsableAsField || resultKind != ResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundEventAccess boundEventAccess = new BoundEventAccess(Syntax, receiverOpt, eventSymbol, isUsableAsField, resultKind, type, base.HasErrors);
                boundEventAccess.CopyAttributes(this);
                return boundEventAccess;
            }
            return this;
        }
    }
}
