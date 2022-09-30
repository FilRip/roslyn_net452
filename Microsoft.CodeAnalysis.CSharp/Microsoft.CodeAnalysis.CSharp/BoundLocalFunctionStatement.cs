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

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLocalFunctionStatement : BoundStatement, IBoundLambdaOrFunction
    {
        public BoundBlock? Body => BlockBody ?? ExpressionBody;

        MethodSymbol IBoundLambdaOrFunction.Symbol => Symbol;

        SyntaxNode IBoundLambdaOrFunction.Syntax => Syntax;

        BoundBlock? IBoundLambdaOrFunction.Body => Body;

        public LocalFunctionSymbol Symbol { get; }

        public BoundBlock? BlockBody { get; }

        public BoundBlock? ExpressionBody { get; }

        public BoundLocalFunctionStatement(SyntaxNode syntax, LocalFunctionSymbol symbol, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.LocalFunctionStatement, syntax, hasErrors || blockBody.HasErrors() || expressionBody.HasErrors())
        {
            Symbol = symbol;
            BlockBody = blockBody;
            ExpressionBody = expressionBody;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLocalFunctionStatement(this);
        }

        public BoundLocalFunctionStatement Update(LocalFunctionSymbol symbol, BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, Symbol) || blockBody != BlockBody || expressionBody != ExpressionBody)
            {
                BoundLocalFunctionStatement boundLocalFunctionStatement = new BoundLocalFunctionStatement(Syntax, symbol, blockBody, expressionBody, base.HasErrors);
                boundLocalFunctionStatement.CopyAttributes(this);
                return boundLocalFunctionStatement;
            }
            return this;
        }
    }
}
