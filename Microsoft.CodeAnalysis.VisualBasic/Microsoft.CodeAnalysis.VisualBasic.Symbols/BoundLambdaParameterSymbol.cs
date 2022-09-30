using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class BoundLambdaParameterSymbol : LambdaParameterSymbol
	{
		private LambdaSymbol _lambdaSymbol;

		private readonly SyntaxNode _syntaxNode;

		public SyntaxNode Syntax => _syntaxNode;

		public override Symbol ContainingSymbol => _lambdaSymbol;

		public BoundLambdaParameterSymbol(string name, int ordinal, TypeSymbol type, bool isByRef, SyntaxNode syntaxNode, Location location)
			: base(name, ordinal, type, isByRef, location)
		{
			_syntaxNode = syntaxNode;
		}

		public void SetLambdaSymbol(LambdaSymbol lambda)
		{
			_lambdaSymbol = lambda;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			return obj is BoundLambdaParameterSymbol boundLambdaParameterSymbol && object.Equals(boundLambdaParameterSymbol._lambdaSymbol, _lambdaSymbol) && boundLambdaParameterSymbol.Ordinal == base.Ordinal;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_lambdaSymbol.GetHashCode(), base.Ordinal);
		}
	}
}
