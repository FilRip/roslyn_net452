using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFunctionPointerLoad : BoundExpression
    {
        public MethodSymbol TargetMethod { get; }

        public new TypeSymbol Type => base.Type;

        public BoundFunctionPointerLoad(SyntaxNode syntax, MethodSymbol targetMethod, TypeSymbol type, bool hasErrors)
            : base(BoundKind.FunctionPointerLoad, syntax, type, hasErrors)
        {
            TargetMethod = targetMethod;
        }

        public BoundFunctionPointerLoad(SyntaxNode syntax, MethodSymbol targetMethod, TypeSymbol type)
            : base(BoundKind.FunctionPointerLoad, syntax, type)
        {
            TargetMethod = targetMethod;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFunctionPointerLoad(this);
        }

        public BoundFunctionPointerLoad Update(MethodSymbol targetMethod, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(targetMethod, TargetMethod) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundFunctionPointerLoad boundFunctionPointerLoad = new BoundFunctionPointerLoad(Syntax, targetMethod, type, base.HasErrors);
                boundFunctionPointerLoad.CopyAttributes(this);
                return boundFunctionPointerLoad;
            }
            return this;
        }
    }
}
