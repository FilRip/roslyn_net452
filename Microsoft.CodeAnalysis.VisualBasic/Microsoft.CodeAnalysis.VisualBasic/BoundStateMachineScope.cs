using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundStateMachineScope : BoundStatement
	{
		private readonly ImmutableArray<FieldSymbol> _Fields;

		private readonly BoundStatement _Statement;

		public ImmutableArray<FieldSymbol> Fields => _Fields;

		public BoundStatement Statement => _Statement;

		public BoundStateMachineScope(SyntaxNode syntax, ImmutableArray<FieldSymbol> fields, BoundStatement statement, bool hasErrors = false)
			: base(BoundKind.StateMachineScope, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statement))
		{
			_Fields = fields;
			_Statement = statement;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitStateMachineScope(this);
		}

		public BoundStateMachineScope Update(ImmutableArray<FieldSymbol> fields, BoundStatement statement)
		{
			if (fields != Fields || statement != Statement)
			{
				BoundStateMachineScope boundStateMachineScope = new BoundStateMachineScope(base.Syntax, fields, statement, base.HasErrors);
				boundStateMachineScope.CopyAttributes(this);
				return boundStateMachineScope;
			}
			return this;
		}
	}
}
