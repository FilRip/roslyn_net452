using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAwaitableInfo : BoundNode
    {
        public BoundAwaitableValuePlaceholder? AwaitableInstancePlaceholder { get; }

        public bool IsDynamic { get; }

        public BoundExpression? GetAwaiter { get; }

        public PropertySymbol? IsCompleted { get; }

        public MethodSymbol? GetResult { get; }

        public BoundAwaitableInfo(SyntaxNode syntax, BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder, bool isDynamic, BoundExpression? getAwaiter, PropertySymbol? isCompleted, MethodSymbol? getResult, bool hasErrors = false)
            : base(BoundKind.AwaitableInfo, syntax, hasErrors || awaitableInstancePlaceholder.HasErrors() || getAwaiter.HasErrors())
        {
            AwaitableInstancePlaceholder = awaitableInstancePlaceholder;
            IsDynamic = isDynamic;
            GetAwaiter = getAwaiter;
            IsCompleted = isCompleted;
            GetResult = getResult;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAwaitableInfo(this);
        }

        public BoundAwaitableInfo Update(BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder, bool isDynamic, BoundExpression? getAwaiter, PropertySymbol? isCompleted, MethodSymbol? getResult)
        {
            if (awaitableInstancePlaceholder != AwaitableInstancePlaceholder || isDynamic != IsDynamic || getAwaiter != GetAwaiter || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(isCompleted, IsCompleted) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getResult, GetResult))
            {
                BoundAwaitableInfo boundAwaitableInfo = new BoundAwaitableInfo(Syntax, awaitableInstancePlaceholder, isDynamic, getAwaiter, isCompleted, getResult, base.HasErrors);
                boundAwaitableInfo.CopyAttributes(this);
                return boundAwaitableInfo;
            }
            return this;
        }
    }
}
