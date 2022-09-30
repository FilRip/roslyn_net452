using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMethodInfo : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public MethodSymbol Method { get; }

        public MethodSymbol? GetMethodFromHandle { get; }

        public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MethodInfo, syntax, type, hasErrors)
        {
            Method = method;
            GetMethodFromHandle = getMethodFromHandle;
        }

        public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type)
            : base(BoundKind.MethodInfo, syntax, type)
        {
            Method = method;
            GetMethodFromHandle = getMethodFromHandle;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMethodInfo(this);
        }

        public BoundMethodInfo Update(MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, Method) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getMethodFromHandle, GetMethodFromHandle) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundMethodInfo boundMethodInfo = new BoundMethodInfo(Syntax, method, getMethodFromHandle, type, base.HasErrors);
                boundMethodInfo.CopyAttributes(this);
                return boundMethodInfo;
            }
            return this;
        }
    }
}
