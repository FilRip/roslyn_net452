using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundParameter : BoundExpression
	{
		private readonly ParameterSymbol _ParameterSymbol;

		private readonly bool _IsLValue;

		private readonly bool _SuppressVirtualCalls;

		public override Symbol ExpressionSymbol => ParameterSymbol;

		public ParameterSymbol ParameterSymbol => _ParameterSymbol;

		public override bool IsLValue => _IsLValue;

		public override bool SuppressVirtualCalls => _SuppressVirtualCalls;

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, bool isLValue, TypeSymbol type, bool hasErrors)
			: this(syntax, parameterSymbol, isLValue, suppressVirtualCalls: false, type, hasErrors)
		{
		}

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, bool isLValue, TypeSymbol type)
			: this(syntax, parameterSymbol, isLValue, suppressVirtualCalls: false, type)
		{
		}

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type, bool hasErrors)
			: this(syntax, parameterSymbol, isLValue: true, suppressVirtualCalls: false, type, hasErrors)
		{
		}

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type)
			: this(syntax, parameterSymbol, isLValue: true, suppressVirtualCalls: false, type)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundParameter MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(_ParameterSymbol, isLValue: false, SuppressVirtualCalls, base.Type);
			}
			return this;
		}

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, bool isLValue, bool suppressVirtualCalls, TypeSymbol type, bool hasErrors)
			: base(BoundKind.Parameter, syntax, type, hasErrors)
		{
			_ParameterSymbol = parameterSymbol;
			_IsLValue = isLValue;
			_SuppressVirtualCalls = suppressVirtualCalls;
		}

		public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, bool isLValue, bool suppressVirtualCalls, TypeSymbol type)
			: base(BoundKind.Parameter, syntax, type)
		{
			_ParameterSymbol = parameterSymbol;
			_IsLValue = isLValue;
			_SuppressVirtualCalls = suppressVirtualCalls;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitParameter(this);
		}

		public BoundParameter Update(ParameterSymbol parameterSymbol, bool isLValue, bool suppressVirtualCalls, TypeSymbol type)
		{
			if ((object)parameterSymbol != ParameterSymbol || isLValue != IsLValue || suppressVirtualCalls != SuppressVirtualCalls || (object)type != base.Type)
			{
				BoundParameter boundParameter = new BoundParameter(base.Syntax, parameterSymbol, isLValue, suppressVirtualCalls, type, base.HasErrors);
				boundParameter.CopyAttributes(this);
				return boundParameter;
			}
			return this;
		}
	}
}
