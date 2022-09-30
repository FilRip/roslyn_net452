using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundParameter : BoundExpression
    {
        public override Symbol ExpressionSymbol => ParameterSymbol;

        public new TypeSymbol Type => base.Type;

        public ParameterSymbol ParameterSymbol { get; }

        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, bool hasErrors = false)
            : this(syntax, parameterSymbol, parameterSymbol.Type, hasErrors)
        {
        }

        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol)
            : this(syntax, parameterSymbol, parameterSymbol.Type)
        {
        }

        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type, bool hasErrors)
            : base(BoundKind.Parameter, syntax, type, hasErrors)
        {
            ParameterSymbol = parameterSymbol;
        }

        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type)
            : base(BoundKind.Parameter, syntax, type)
        {
            ParameterSymbol = parameterSymbol;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitParameter(this);
        }

        public BoundParameter Update(ParameterSymbol parameterSymbol, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(parameterSymbol, ParameterSymbol) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundParameter boundParameter = new BoundParameter(Syntax, parameterSymbol, type, base.HasErrors);
                boundParameter.CopyAttributes(this);
                return boundParameter;
            }
            return this;
        }
    }
}
