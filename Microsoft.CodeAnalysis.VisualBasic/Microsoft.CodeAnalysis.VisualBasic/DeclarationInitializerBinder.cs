using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DeclarationInitializerBinder : Binder
	{
		private readonly Symbol _symbol;

		private readonly ImmutableArray<Symbol> _additionalSymbols;

		private readonly VisualBasicSyntaxNode _root;

		public override Symbol ContainingMember => _symbol;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => _additionalSymbols;

		internal VisualBasicSyntaxNode Root => _root;

		public DeclarationInitializerBinder(Symbol symbol, ImmutableArray<Symbol> additionalSymbols, Binder next, VisualBasicSyntaxNode root)
			: base(next)
		{
			_symbol = symbol;
			_additionalSymbols = additionalSymbols;
			_root = root;
		}

		public override Binder GetBinder(SyntaxNode node)
		{
			return null;
		}

		public override Binder GetBinder(SyntaxList<StatementSyntax> stmts)
		{
			return null;
		}
	}
}
