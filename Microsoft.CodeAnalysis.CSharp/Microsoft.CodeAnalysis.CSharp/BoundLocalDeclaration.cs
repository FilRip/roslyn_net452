using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLocalDeclaration : BoundStatement
    {
        public LocalSymbol LocalSymbol { get; }

        public BoundTypeExpression? DeclaredTypeOpt { get; }

        public BoundExpression? InitializerOpt { get; }

        public ImmutableArray<BoundExpression> ArgumentsOpt { get; }

        public bool InferredType { get; }

        public BoundLocalDeclaration(SyntaxNode syntax, LocalSymbol localSymbol, BoundTypeExpression? declaredTypeOpt, BoundExpression? initializerOpt, ImmutableArray<BoundExpression> argumentsOpt, bool inferredType, bool hasErrors = false)
            : base(BoundKind.LocalDeclaration, syntax, hasErrors || declaredTypeOpt.HasErrors() || initializerOpt.HasErrors() || argumentsOpt.HasErrors())
        {
            LocalSymbol = localSymbol;
            DeclaredTypeOpt = declaredTypeOpt;
            InitializerOpt = initializerOpt;
            ArgumentsOpt = argumentsOpt;
            InferredType = inferredType;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLocalDeclaration(this);
        }

        public BoundLocalDeclaration Update(LocalSymbol localSymbol, BoundTypeExpression? declaredTypeOpt, BoundExpression? initializerOpt, ImmutableArray<BoundExpression> argumentsOpt, bool inferredType)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, LocalSymbol) || declaredTypeOpt != DeclaredTypeOpt || initializerOpt != InitializerOpt || argumentsOpt != ArgumentsOpt || inferredType != InferredType)
            {
                BoundLocalDeclaration boundLocalDeclaration = new BoundLocalDeclaration(Syntax, localSymbol, declaredTypeOpt, initializerOpt, argumentsOpt, inferredType, base.HasErrors);
                boundLocalDeclaration.CopyAttributes(this);
                return boundLocalDeclaration;
            }
            return this;
        }
    }
}
