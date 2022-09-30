using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTypeArguments : BoundExpression
	{
		private readonly ImmutableArray<TypeSymbol> _Arguments;

		public ImmutableArray<TypeSymbol> Arguments => _Arguments;

		public BoundTypeArguments(SyntaxNode syntax, ImmutableArray<TypeSymbol> arguments, bool hasErrors)
			: base(BoundKind.TypeArguments, syntax, null, hasErrors)
		{
			_Arguments = arguments;
		}

		public BoundTypeArguments(SyntaxNode syntax, ImmutableArray<TypeSymbol> arguments)
			: base(BoundKind.TypeArguments, syntax, null)
		{
			_Arguments = arguments;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTypeArguments(this);
		}

		public BoundTypeArguments Update(ImmutableArray<TypeSymbol> arguments)
		{
			if (arguments != Arguments)
			{
				BoundTypeArguments boundTypeArguments = new BoundTypeArguments(base.Syntax, arguments, base.HasErrors);
				boundTypeArguments.CopyAttributes(this);
				return boundTypeArguments;
			}
			return this;
		}
	}
}
