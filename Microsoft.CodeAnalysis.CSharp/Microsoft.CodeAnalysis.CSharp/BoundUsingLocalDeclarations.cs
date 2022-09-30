using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUsingLocalDeclarations : BoundMultipleLocalDeclarationsBase
    {
        public MethodArgumentInfo? PatternDisposeInfoOpt { get; }

        public Conversion IDisposableConversion { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }

        public BoundUsingLocalDeclarations(SyntaxNode syntax, MethodArgumentInfo? patternDisposeInfoOpt, Conversion iDisposableConversion, BoundAwaitableInfo? awaitOpt, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(BoundKind.UsingLocalDeclarations, syntax, localDeclarations, hasErrors || awaitOpt.HasErrors() || localDeclarations.HasErrors())
        {
            PatternDisposeInfoOpt = patternDisposeInfoOpt;
            IDisposableConversion = iDisposableConversion;
            AwaitOpt = awaitOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUsingLocalDeclarations(this);
        }

        public BoundUsingLocalDeclarations Update(MethodArgumentInfo? patternDisposeInfoOpt, Conversion iDisposableConversion, BoundAwaitableInfo? awaitOpt, ImmutableArray<BoundLocalDeclaration> localDeclarations)
        {
            if (patternDisposeInfoOpt != PatternDisposeInfoOpt || iDisposableConversion != IDisposableConversion || awaitOpt != AwaitOpt || localDeclarations != base.LocalDeclarations)
            {
                BoundUsingLocalDeclarations boundUsingLocalDeclarations = new BoundUsingLocalDeclarations(Syntax, patternDisposeInfoOpt, iDisposableConversion, awaitOpt, localDeclarations, base.HasErrors);
                boundUsingLocalDeclarations.CopyAttributes(this);
                return boundUsingLocalDeclarations;
            }
            return this;
        }
    }
}
