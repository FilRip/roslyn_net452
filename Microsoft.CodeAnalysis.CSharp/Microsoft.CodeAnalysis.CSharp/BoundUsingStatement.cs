using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUsingStatement : BoundStatement
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundMultipleLocalDeclarations? DeclarationsOpt { get; }

        public BoundExpression? ExpressionOpt { get; }

        public Conversion IDisposableConversion { get; }

        public BoundStatement Body { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }

        public MethodArgumentInfo? PatternDisposeInfoOpt { get; }

        public BoundUsingStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations? declarationsOpt, BoundExpression? expressionOpt, Conversion iDisposableConversion, BoundStatement body, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfoOpt, bool hasErrors = false)
            : base(BoundKind.UsingStatement, syntax, hasErrors || declarationsOpt.HasErrors() || expressionOpt.HasErrors() || body.HasErrors() || awaitOpt.HasErrors())
        {
            Locals = locals;
            DeclarationsOpt = declarationsOpt;
            ExpressionOpt = expressionOpt;
            IDisposableConversion = iDisposableConversion;
            Body = body;
            AwaitOpt = awaitOpt;
            PatternDisposeInfoOpt = patternDisposeInfoOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUsingStatement(this);
        }

        public BoundUsingStatement Update(ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations? declarationsOpt, BoundExpression? expressionOpt, Conversion iDisposableConversion, BoundStatement body, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfoOpt)
        {
            if (locals != Locals || declarationsOpt != DeclarationsOpt || expressionOpt != ExpressionOpt || iDisposableConversion != IDisposableConversion || body != Body || awaitOpt != AwaitOpt || patternDisposeInfoOpt != PatternDisposeInfoOpt)
            {
                BoundUsingStatement boundUsingStatement = new BoundUsingStatement(Syntax, locals, declarationsOpt, expressionOpt, iDisposableConversion, body, awaitOpt, patternDisposeInfoOpt, base.HasErrors);
                boundUsingStatement.CopyAttributes(this);
                return boundUsingStatement;
            }
            return this;
        }
    }
}
