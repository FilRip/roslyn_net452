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
    public sealed class BoundDeclarationPattern : BoundPattern
    {
        public Symbol? Variable { get; }

        public BoundExpression? VariableAccess { get; }

        public BoundTypeExpression DeclaredType { get; }

        public bool IsVar { get; }

        public BoundDeclarationPattern(SyntaxNode syntax, Symbol? variable, BoundExpression? variableAccess, BoundTypeExpression declaredType, bool isVar, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.DeclarationPattern, syntax, inputType, narrowedType, hasErrors || variableAccess.HasErrors() || declaredType.HasErrors())
        {
            Variable = variable;
            VariableAccess = variableAccess;
            DeclaredType = declaredType;
            IsVar = isVar;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDeclarationPattern(this);
        }

        public BoundDeclarationPattern Update(Symbol? variable, BoundExpression? variableAccess, BoundTypeExpression declaredType, bool isVar, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variable, Variable) || variableAccess != VariableAccess || declaredType != DeclaredType || isVar != IsVar || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundDeclarationPattern boundDeclarationPattern = new BoundDeclarationPattern(Syntax, variable, variableAccess, declaredType, isVar, inputType, narrowedType, base.HasErrors);
                boundDeclarationPattern.CopyAttributes(this);
                return boundDeclarationPattern;
            }
            return this;
        }
    }
}
