using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConstructorMethodBody : BoundMethodBodyBase
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpressionStatement? Initializer { get; }

        public BoundConstructorMethodBody(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpressionStatement? initializer, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.ConstructorMethodBody, syntax, blockBody, expressionBody, hasErrors || initializer.HasErrors() || blockBody.HasErrors() || expressionBody.HasErrors())
        {
            Locals = locals;
            Initializer = initializer;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConstructorMethodBody(this);
        }

        public BoundConstructorMethodBody Update(ImmutableArray<LocalSymbol> locals, BoundExpressionStatement? initializer, BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (locals != Locals || initializer != Initializer || blockBody != base.BlockBody || expressionBody != base.ExpressionBody)
            {
                BoundConstructorMethodBody boundConstructorMethodBody = new BoundConstructorMethodBody(Syntax, locals, initializer, blockBody, expressionBody, base.HasErrors);
                boundConstructorMethodBody.CopyAttributes(this);
                return boundConstructorMethodBody;
            }
            return this;
        }
    }
}
