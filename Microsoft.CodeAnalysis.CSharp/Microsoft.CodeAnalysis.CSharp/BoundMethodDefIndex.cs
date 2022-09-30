using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMethodDefIndex : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public MethodSymbol Method { get; }

        public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MethodDefIndex, syntax, type, hasErrors)
        {
            Method = method;
        }

        public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type)
            : base(BoundKind.MethodDefIndex, syntax, type)
        {
            Method = method;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMethodDefIndex(this);
        }

        public BoundMethodDefIndex Update(MethodSymbol method, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, Method) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundMethodDefIndex boundMethodDefIndex = new BoundMethodDefIndex(Syntax, method, type, base.HasErrors);
                boundMethodDefIndex.CopyAttributes(this);
                return boundMethodDefIndex;
            }
            return this;
        }
    }
}
