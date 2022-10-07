using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class RecursivePatternOperation : BasePatternOperation, IRecursivePatternOperation, IPatternOperation, IOperation
    {
        public ITypeSymbol MatchedType { get; }

        public ISymbol? DeconstructSymbol { get; }

        public ImmutableArray<IPatternOperation> DeconstructionSubpatterns { get; }

        public ImmutableArray<IPropertySubpatternOperation> PropertySubpatterns { get; }

        public ISymbol? DeclaredSymbol { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.RecursivePattern;

        public RecursivePatternOperation(ITypeSymbol matchedType, ISymbol? deconstructSymbol, ImmutableArray<IPatternOperation> deconstructionSubpatterns, ImmutableArray<IPropertySubpatternOperation> propertySubpatterns, ISymbol? declaredSymbol, ITypeSymbol inputType, ITypeSymbol narrowedType, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(inputType, narrowedType, semanticModel, syntax, isImplicit)
        {
            MatchedType = matchedType;
            DeconstructSymbol = deconstructSymbol;
            DeconstructionSubpatterns = Operation.SetParentOperation(deconstructionSubpatterns, this);
            PropertySubpatterns = Operation.SetParentOperation(propertySubpatterns, this);
            DeclaredSymbol = declaredSymbol;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < DeconstructionSubpatterns.Length)
                    {
                        return DeconstructionSubpatterns[index];
                    }
                    break;
                case 1:
                    if (index < PropertySubpatterns.Length)
                    {
                        return PropertySubpatterns[index];
                    }
                    break;
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!DeconstructionSubpatterns.IsEmpty) return (true, 0, 0);
                    else goto case 0;
                case 0 when previousIndex + 1 < DeconstructionSubpatterns.Length:
                    return (true, 0, previousIndex + 1);
                case 0:
                    if (!PropertySubpatterns.IsEmpty) return (true, 1, 0);
                    else goto case 1;
                case 1 when previousIndex + 1 < PropertySubpatterns.Length:
                    return (true, 1, previousIndex + 1);
                case 1:
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitRecursivePattern(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitRecursivePattern(this, argument);
        }
    }
}
