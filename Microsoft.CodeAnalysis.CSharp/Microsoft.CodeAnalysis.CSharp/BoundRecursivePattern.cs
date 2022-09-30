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
    public sealed class BoundRecursivePattern : BoundPattern
    {
        public BoundTypeExpression? DeclaredType { get; }

        public MethodSymbol? DeconstructMethod { get; }

        public ImmutableArray<BoundSubpattern> Deconstruction { get; }

        public ImmutableArray<BoundSubpattern> Properties { get; }

        public Symbol? Variable { get; }

        public BoundExpression? VariableAccess { get; }

        public bool IsExplicitNotNullTest { get; }

        public BoundRecursivePattern(SyntaxNode syntax, BoundTypeExpression? declaredType, MethodSymbol? deconstructMethod, ImmutableArray<BoundSubpattern> deconstruction, ImmutableArray<BoundSubpattern> properties, Symbol? variable, BoundExpression? variableAccess, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.RecursivePattern, syntax, inputType, narrowedType, hasErrors || declaredType.HasErrors() || deconstruction.HasErrors() || properties.HasErrors() || variableAccess.HasErrors())
        {
            DeclaredType = declaredType;
            DeconstructMethod = deconstructMethod;
            Deconstruction = deconstruction;
            Properties = properties;
            Variable = variable;
            VariableAccess = variableAccess;
            IsExplicitNotNullTest = isExplicitNotNullTest;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRecursivePattern(this);
        }

        public BoundRecursivePattern Update(BoundTypeExpression? declaredType, MethodSymbol? deconstructMethod, ImmutableArray<BoundSubpattern> deconstruction, ImmutableArray<BoundSubpattern> properties, Symbol? variable, BoundExpression? variableAccess, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (declaredType != DeclaredType || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(deconstructMethod, DeconstructMethod) || deconstruction != Deconstruction || properties != Properties || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variable, Variable) || variableAccess != VariableAccess || isExplicitNotNullTest != IsExplicitNotNullTest || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundRecursivePattern boundRecursivePattern = new BoundRecursivePattern(Syntax, declaredType, deconstructMethod, deconstruction, properties, variable, variableAccess, isExplicitNotNullTest, inputType, narrowedType, base.HasErrors);
                boundRecursivePattern.CopyAttributes(this);
                return boundRecursivePattern;
            }
            return this;
        }
    }
}
