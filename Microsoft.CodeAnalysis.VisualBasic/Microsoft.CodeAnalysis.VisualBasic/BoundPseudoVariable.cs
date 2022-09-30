using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundPseudoVariable : BoundExpression
	{
		private readonly LocalSymbol _LocalSymbol;

		private readonly bool _IsLValue;

		private readonly PseudoVariableExpressions _EmitExpressions;

		public LocalSymbol LocalSymbol => _LocalSymbol;

		public override bool IsLValue => _IsLValue;

		public PseudoVariableExpressions EmitExpressions => _EmitExpressions;

		protected override BoundExpression MakeRValueImpl()
		{
			if (!_IsLValue)
			{
				return this;
			}
			return Update(_LocalSymbol, isLValue: false, _EmitExpressions, base.Type);
		}

		public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, bool isLValue, PseudoVariableExpressions emitExpressions, TypeSymbol type, bool hasErrors)
			: base(BoundKind.PseudoVariable, syntax, type, hasErrors)
		{
			_LocalSymbol = localSymbol;
			_IsLValue = isLValue;
			_EmitExpressions = emitExpressions;
		}

		public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, bool isLValue, PseudoVariableExpressions emitExpressions, TypeSymbol type)
			: base(BoundKind.PseudoVariable, syntax, type)
		{
			_LocalSymbol = localSymbol;
			_IsLValue = isLValue;
			_EmitExpressions = emitExpressions;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitPseudoVariable(this);
		}

		public BoundPseudoVariable Update(LocalSymbol localSymbol, bool isLValue, PseudoVariableExpressions emitExpressions, TypeSymbol type)
		{
			if ((object)localSymbol != LocalSymbol || isLValue != IsLValue || emitExpressions != EmitExpressions || (object)type != base.Type)
			{
				BoundPseudoVariable boundPseudoVariable = new BoundPseudoVariable(base.Syntax, localSymbol, isLValue, emitExpressions, type, base.HasErrors);
				boundPseudoVariable.CopyAttributes(this);
				return boundPseudoVariable;
			}
			return this;
		}
	}
}
