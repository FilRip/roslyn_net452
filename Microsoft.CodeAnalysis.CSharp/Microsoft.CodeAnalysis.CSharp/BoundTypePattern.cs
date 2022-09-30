using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTypePattern : BoundPattern
    {
        public BoundTypeExpression DeclaredType { get; }

        public bool IsExplicitNotNullTest { get; }

        public BoundTypePattern(SyntaxNode syntax, BoundTypeExpression declaredType, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.TypePattern, syntax, inputType, narrowedType, hasErrors || declaredType.HasErrors())
        {
            DeclaredType = declaredType;
            IsExplicitNotNullTest = isExplicitNotNullTest;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTypePattern(this);
        }

        public BoundTypePattern Update(BoundTypeExpression declaredType, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (declaredType != DeclaredType || isExplicitNotNullTest != IsExplicitNotNullTest || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundTypePattern boundTypePattern = new BoundTypePattern(Syntax, declaredType, isExplicitNotNullTest, inputType, narrowedType, base.HasErrors);
                boundTypePattern.CopyAttributes(this);
                return boundTypePattern;
            }
            return this;
        }
    }
}
