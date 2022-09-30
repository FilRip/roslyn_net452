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
    public sealed class BoundNoPiaObjectCreationExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public string? GuidString { get; }

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }

        public BoundNoPiaObjectCreationExpression(SyntaxNode syntax, string? guidString, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NoPiaObjectCreationExpression, syntax, type, hasErrors || initializerExpressionOpt.HasErrors())
        {
            GuidString = guidString;
            InitializerExpressionOpt = initializerExpressionOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNoPiaObjectCreationExpression(this);
        }

        public BoundNoPiaObjectCreationExpression Update(string? guidString, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            if (guidString != GuidString || initializerExpressionOpt != InitializerExpressionOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundNoPiaObjectCreationExpression boundNoPiaObjectCreationExpression = new BoundNoPiaObjectCreationExpression(Syntax, guidString, initializerExpressionOpt, type, base.HasErrors);
                boundNoPiaObjectCreationExpression.CopyAttributes(this);
                return boundNoPiaObjectCreationExpression;
            }
            return this;
        }
    }
}
