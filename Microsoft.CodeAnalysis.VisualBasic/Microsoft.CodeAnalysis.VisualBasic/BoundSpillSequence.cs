using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSpillSequence : BoundExpression
	{
		private readonly ImmutableArray<LocalSymbol> _Locals;

		private readonly ImmutableArray<FieldSymbol> _SpillFields;

		private readonly ImmutableArray<BoundStatement> _Statements;

		private readonly BoundExpression _ValueOpt;

		public override bool IsLValue
		{
			get
			{
				if (ValueOpt == null)
				{
					return false;
				}
				return ValueOpt.IsLValue;
			}
		}

		public ImmutableArray<LocalSymbol> Locals => _Locals;

		public ImmutableArray<FieldSymbol> SpillFields => _SpillFields;

		public ImmutableArray<BoundStatement> Statements => _Statements;

		public BoundExpression ValueOpt => _ValueOpt;

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundSpillSequence MakeRValue()
		{
			if (IsLValue)
			{
				return Update(Locals, SpillFields, Statements, ValueOpt.MakeRValue(), base.Type);
			}
			return this;
		}

		public BoundSpillSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<FieldSymbol> spillFields, ImmutableArray<BoundStatement> statements, BoundExpression valueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.SpillSequence, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statements) || BoundNodeExtensions.NonNullAndHasErrors(valueOpt))
		{
			_Locals = locals;
			_SpillFields = spillFields;
			_Statements = statements;
			_ValueOpt = valueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSpillSequence(this);
		}

		public BoundSpillSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<FieldSymbol> spillFields, ImmutableArray<BoundStatement> statements, BoundExpression valueOpt, TypeSymbol type)
		{
			if (locals != Locals || spillFields != SpillFields || statements != Statements || valueOpt != ValueOpt || (object)type != base.Type)
			{
				BoundSpillSequence boundSpillSequence = new BoundSpillSequence(base.Syntax, locals, spillFields, statements, valueOpt, type, base.HasErrors);
				boundSpillSequence.CopyAttributes(this);
				return boundSpillSequence;
			}
			return this;
		}
	}
}
