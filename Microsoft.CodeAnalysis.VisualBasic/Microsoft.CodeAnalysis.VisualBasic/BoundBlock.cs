using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBlock : BoundStatement
	{
		private readonly SyntaxList<StatementSyntax> _StatementListSyntax;

		private readonly ImmutableArray<LocalSymbol> _Locals;

		private readonly ImmutableArray<BoundStatement> _Statements;

		public SyntaxList<StatementSyntax> StatementListSyntax => _StatementListSyntax;

		public ImmutableArray<LocalSymbol> Locals => _Locals;

		public ImmutableArray<BoundStatement> Statements => _Statements;

		public BoundBlock(SyntaxNode syntax, SyntaxList<StatementSyntax> statementListSyntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
			: base(BoundKind.Block, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statements))
		{
			_StatementListSyntax = statementListSyntax;
			_Locals = locals;
			_Statements = statements;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBlock(this);
		}

		public BoundBlock Update(SyntaxList<StatementSyntax> statementListSyntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements)
		{
			if (statementListSyntax != StatementListSyntax || locals != Locals || statements != Statements)
			{
				BoundBlock boundBlock = new BoundBlock(base.Syntax, statementListSyntax, locals, statements, base.HasErrors);
				boundBlock.CopyAttributes(this);
				return boundBlock;
			}
			return this;
		}
	}
}
