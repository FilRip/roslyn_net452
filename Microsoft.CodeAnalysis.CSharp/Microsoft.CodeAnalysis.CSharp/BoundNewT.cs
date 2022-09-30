using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNewT : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }

        public BoundNewT(SyntaxNode syntax, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NewT, syntax, type, hasErrors || initializerExpressionOpt.HasErrors())
        {
            InitializerExpressionOpt = initializerExpressionOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNewT(this);
        }

        public BoundNewT Update(BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            if (initializerExpressionOpt != InitializerExpressionOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundNewT boundNewT = new BoundNewT(Syntax, initializerExpressionOpt, type, base.HasErrors);
                boundNewT.CopyAttributes(this);
                return boundNewT;
            }
            return this;
        }
    }
}
