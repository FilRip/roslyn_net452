using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLiteral : BoundExpression
	{
		private readonly ConstantValue _Value;

		public override ConstantValue ConstantValueOpt => Value;

		public ConstantValue Value => _Value;

		public BoundLiteral(SyntaxNode syntax, ConstantValue value, TypeSymbol type, bool hasErrors)
			: base(BoundKind.Literal, syntax, type, hasErrors)
		{
			_Value = value;
		}

		public BoundLiteral(SyntaxNode syntax, ConstantValue value, TypeSymbol type)
			: base(BoundKind.Literal, syntax, type)
		{
			_Value = value;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLiteral(this);
		}

		public BoundLiteral Update(ConstantValue value, TypeSymbol type)
		{
			if ((object)value != Value || (object)type != base.Type)
			{
				BoundLiteral boundLiteral = new BoundLiteral(base.Syntax, value, type, base.HasErrors);
				boundLiteral.CopyAttributes(this);
				return boundLiteral;
			}
			return this;
		}
	}
}
