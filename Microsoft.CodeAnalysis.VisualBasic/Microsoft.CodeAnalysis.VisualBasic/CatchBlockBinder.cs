using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class CatchBlockBinder : BlockBaseBinder
	{
		private readonly CatchBlockSyntax _syntax;

		private ImmutableArray<LocalSymbol> _locals;

		internal override ImmutableArray<LocalSymbol> Locals
		{
			get
			{
				if (_locals.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _locals, BuildLocals(), default(ImmutableArray<LocalSymbol>));
				}
				return _locals;
			}
		}

		public CatchBlockBinder(Binder enclosing, CatchBlockSyntax syntax)
			: base(enclosing)
		{
			_locals = default(ImmutableArray<LocalSymbol>);
			_syntax = syntax;
		}

		private ImmutableArray<LocalSymbol> BuildLocals()
		{
			CatchStatementSyntax catchStatement = _syntax.CatchStatement;
			SimpleAsClauseSyntax asClause = catchStatement.AsClause;
			if (asClause != null)
			{
				return ImmutableArray.Create(LocalSymbol.Create(ContainingMember, this, catchStatement.IdentifierName.Identifier, null, asClause, null, LocalDeclarationKind.Catch));
			}
			return ImmutableArray<LocalSymbol>.Empty;
		}
	}
}
