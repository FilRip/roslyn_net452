using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLocal : BoundExpression
	{
		private readonly LocalSymbol _LocalSymbol;

		private readonly bool _IsLValue;

		public override Symbol ExpressionSymbol => LocalSymbol;

		public override ConstantValue ConstantValueOpt
		{
			get
			{
				if (base.HasErrors || TypeSymbolExtensions.IsErrorType(base.Type))
				{
					return null;
				}
				return LocalSymbol.GetConstantValue(null);
			}
		}

		public LocalSymbol LocalSymbol => _LocalSymbol;

		public override bool IsLValue => _IsLValue;

		public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, TypeSymbol type, bool hasErrors)
			: this(syntax, localSymbol, !localSymbol.IsReadOnly, type, hasErrors)
		{
		}

		public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, TypeSymbol type)
			: this(syntax, localSymbol, !localSymbol.IsReadOnly, type)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundLocal MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(_LocalSymbol, isLValue: false, base.Type);
			}
			return this;
		}

		public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, bool isLValue, TypeSymbol type, bool hasErrors)
			: base(BoundKind.Local, syntax, type, hasErrors)
		{
			_LocalSymbol = localSymbol;
			_IsLValue = isLValue;
		}

		public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, bool isLValue, TypeSymbol type)
			: base(BoundKind.Local, syntax, type)
		{
			_LocalSymbol = localSymbol;
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLocal(this);
		}

		public BoundLocal Update(LocalSymbol localSymbol, bool isLValue, TypeSymbol type)
		{
			if ((object)localSymbol != LocalSymbol || isLValue != IsLValue || (object)type != base.Type)
			{
				BoundLocal boundLocal = new BoundLocal(base.Syntax, localSymbol, isLValue, type, base.HasErrors);
				boundLocal.CopyAttributes(this);
				return boundLocal;
			}
			return this;
		}
	}
}
