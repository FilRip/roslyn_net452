using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundArgList : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundArgList(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ArgList, syntax, type, hasErrors)
        {
        }

        public BoundArgList(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ArgList, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitArgList(this);
        }

        public BoundArgList Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundArgList boundArgList = new BoundArgList(Syntax, type, base.HasErrors);
                boundArgList.CopyAttributes(this);
                return boundArgList;
            }
            return this;
        }
    }
}
