using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class DeclarationPatternOperation : BasePatternOperation, IDeclarationPatternOperation, IPatternOperation, IOperation
    {
        public ITypeSymbol? MatchedType { get; }

        public bool MatchesNull { get; }

        public ISymbol? DeclaredSymbol { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.DeclarationPattern;

        public DeclarationPatternOperation(ITypeSymbol? matchedType, bool matchesNull, ISymbol? declaredSymbol, ITypeSymbol inputType, ITypeSymbol narrowedType, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(inputType, narrowedType, semanticModel, syntax, isImplicit)
        {
            MatchedType = matchedType;
            MatchesNull = matchesNull;
            DeclaredSymbol = declaredSymbol;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            return (false, int.MinValue, int.MinValue);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitDeclarationPattern(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitDeclarationPattern(this, argument);
        }
    }
}
