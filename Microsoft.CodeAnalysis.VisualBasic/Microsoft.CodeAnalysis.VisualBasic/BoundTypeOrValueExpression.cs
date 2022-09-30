using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTypeOrValueExpression : BoundExpression
	{
		private readonly BoundTypeOrValueData _Data;

		public BoundTypeOrValueData Data => _Data;

		public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type, bool hasErrors)
			: base(BoundKind.TypeOrValueExpression, syntax, type, hasErrors)
		{
			_Data = data;
		}

		public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type)
			: base(BoundKind.TypeOrValueExpression, syntax, type)
		{
			_Data = data;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTypeOrValueExpression(this);
		}

		public BoundTypeOrValueExpression Update(BoundTypeOrValueData data, TypeSymbol type)
		{
			if (data != Data || (object)type != base.Type)
			{
				BoundTypeOrValueExpression boundTypeOrValueExpression = new BoundTypeOrValueExpression(base.Syntax, data, type, base.HasErrors);
				boundTypeOrValueExpression.CopyAttributes(this);
				return boundTypeOrValueExpression;
			}
			return this;
		}
	}
}
