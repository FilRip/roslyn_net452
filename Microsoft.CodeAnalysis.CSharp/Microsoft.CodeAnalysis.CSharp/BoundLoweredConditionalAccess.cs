using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLoweredConditionalAccess : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public MethodSymbol? HasValueMethodOpt { get; }

        public BoundExpression WhenNotNull { get; }

        public BoundExpression? WhenNullOpt { get; }

        public int Id { get; }

        public BoundLoweredConditionalAccess(SyntaxNode syntax, BoundExpression receiver, MethodSymbol? hasValueMethodOpt, BoundExpression whenNotNull, BoundExpression? whenNullOpt, int id, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.LoweredConditionalAccess, syntax, type, hasErrors || receiver.HasErrors() || whenNotNull.HasErrors() || whenNullOpt.HasErrors())
        {
            Receiver = receiver;
            HasValueMethodOpt = hasValueMethodOpt;
            WhenNotNull = whenNotNull;
            WhenNullOpt = whenNullOpt;
            Id = id;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLoweredConditionalAccess(this);
        }

        public BoundLoweredConditionalAccess Update(BoundExpression receiver, MethodSymbol? hasValueMethodOpt, BoundExpression whenNotNull, BoundExpression? whenNullOpt, int id, TypeSymbol type)
        {
            if (receiver != Receiver || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(hasValueMethodOpt, HasValueMethodOpt) || whenNotNull != WhenNotNull || whenNullOpt != WhenNullOpt || id != Id || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundLoweredConditionalAccess boundLoweredConditionalAccess = new BoundLoweredConditionalAccess(Syntax, receiver, hasValueMethodOpt, whenNotNull, whenNullOpt, id, type, base.HasErrors);
                boundLoweredConditionalAccess.CopyAttributes(this);
                return boundLoweredConditionalAccess;
            }
            return this;
        }
    }
}
