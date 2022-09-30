using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundExpression : BoundNode
	{
		private readonly TypeSymbol _Type;

		public bool IsConstant => (object)ConstantValueOpt != null;

		public virtual ConstantValue ConstantValueOpt => null;

		public virtual Symbol ExpressionSymbol => null;

		public virtual LookupResultKind ResultKind => LookupResultKind.Good;

		public virtual bool SuppressVirtualCalls => false;

		public virtual bool IsLValue => false;

		public TypeSymbol Type => _Type;

		public BoundExpression MakeRValue()
		{
			return MakeRValueImpl();
		}

		protected virtual BoundExpression MakeRValueImpl()
		{
			return this;
		}

		protected BoundExpression(BoundKind kind, SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
			_Type = type;
		}

		protected BoundExpression(BoundKind kind, SyntaxNode syntax, TypeSymbol type)
			: base(kind, syntax)
		{
			_Type = type;
		}
	}
}
