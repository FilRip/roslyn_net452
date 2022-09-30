using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPropertyAccess : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol? ExpressionSymbol => PropertySymbol;

        public new TypeSymbol Type => base.Type;

        public BoundExpression? ReceiverOpt { get; }

        public PropertySymbol PropertySymbol { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public BoundPropertyAccess(SyntaxNode syntax, BoundExpression? receiverOpt, PropertySymbol propertySymbol, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PropertyAccess, syntax, type, hasErrors || receiverOpt.HasErrors())
        {
            ReceiverOpt = receiverOpt;
            PropertySymbol = propertySymbol;
            _ResultKind = resultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPropertyAccess(this);
        }

        public BoundPropertyAccess Update(BoundExpression? receiverOpt, PropertySymbol propertySymbol, LookupResultKind resultKind, TypeSymbol type)
        {
            if (receiverOpt != ReceiverOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(propertySymbol, PropertySymbol) || resultKind != ResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPropertyAccess boundPropertyAccess = new BoundPropertyAccess(Syntax, receiverOpt, propertySymbol, resultKind, type, base.HasErrors);
                boundPropertyAccess.CopyAttributes(this);
                return boundPropertyAccess;
            }
            return this;
        }
    }
}
