using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundCatchBlock : BoundNode
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpression? ExceptionSourceOpt { get; }

        public TypeSymbol? ExceptionTypeOpt { get; }

        public BoundStatementList? ExceptionFilterPrologueOpt { get; }

        public BoundExpression? ExceptionFilterOpt { get; }

        public BoundBlock Body { get; }

        public bool IsSynthesizedAsyncCatchAll { get; }

        public BoundCatchBlock(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression? exceptionSourceOpt, TypeSymbol? exceptionTypeOpt, BoundStatementList? exceptionFilterPrologueOpt, BoundExpression? exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll, bool hasErrors = false)
            : base(BoundKind.CatchBlock, syntax, hasErrors || exceptionSourceOpt.HasErrors() || exceptionFilterPrologueOpt.HasErrors() || exceptionFilterOpt.HasErrors() || body.HasErrors())
        {
            Locals = locals;
            ExceptionSourceOpt = exceptionSourceOpt;
            ExceptionTypeOpt = exceptionTypeOpt;
            ExceptionFilterPrologueOpt = exceptionFilterPrologueOpt;
            ExceptionFilterOpt = exceptionFilterOpt;
            Body = body;
            IsSynthesizedAsyncCatchAll = isSynthesizedAsyncCatchAll;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitCatchBlock(this);
        }

        public BoundCatchBlock Update(ImmutableArray<LocalSymbol> locals, BoundExpression? exceptionSourceOpt, TypeSymbol? exceptionTypeOpt, BoundStatementList? exceptionFilterPrologueOpt, BoundExpression? exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll)
        {
            if (locals != Locals || exceptionSourceOpt != ExceptionSourceOpt || !TypeSymbol.Equals(exceptionTypeOpt, ExceptionTypeOpt, TypeCompareKind.ConsiderEverything) || exceptionFilterPrologueOpt != ExceptionFilterPrologueOpt || exceptionFilterOpt != ExceptionFilterOpt || body != Body || isSynthesizedAsyncCatchAll != IsSynthesizedAsyncCatchAll)
            {
                BoundCatchBlock boundCatchBlock = new BoundCatchBlock(Syntax, locals, exceptionSourceOpt, exceptionTypeOpt, exceptionFilterPrologueOpt, exceptionFilterOpt, body, isSynthesizedAsyncCatchAll, base.HasErrors);
                boundCatchBlock.CopyAttributes(this);
                return boundCatchBlock;
            }
            return this;
        }
    }
}
