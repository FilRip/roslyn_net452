using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTypeOrValueExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundTypeOrValueData Data { get; }

        public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type, bool hasErrors)
            : base(BoundKind.TypeOrValueExpression, syntax, type, hasErrors)
        {
            Data = data;
        }

        public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type)
            : base(BoundKind.TypeOrValueExpression, syntax, type)
        {
            Data = data;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTypeOrValueExpression(this);
        }

        public BoundTypeOrValueExpression Update(BoundTypeOrValueData data, TypeSymbol type)
        {
            if (data != Data || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTypeOrValueExpression boundTypeOrValueExpression = new BoundTypeOrValueExpression(Syntax, data, type, base.HasErrors);
                boundTypeOrValueExpression.CopyAttributes(this);
                return boundTypeOrValueExpression;
            }
            return this;
        }
    }
}
