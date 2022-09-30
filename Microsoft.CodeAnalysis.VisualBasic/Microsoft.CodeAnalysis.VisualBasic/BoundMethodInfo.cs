using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMethodInfo : BoundExpression
	{
		private readonly MethodSymbol _Method;

		public MethodSymbol Method => _Method;

		public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MethodInfo, syntax, type, hasErrors)
		{
			_Method = method;
		}

		public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, TypeSymbol type)
			: base(BoundKind.MethodInfo, syntax, type)
		{
			_Method = method;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMethodInfo(this);
		}

		public BoundMethodInfo Update(MethodSymbol method, TypeSymbol type)
		{
			if ((object)method != Method || (object)type != base.Type)
			{
				BoundMethodInfo boundMethodInfo = new BoundMethodInfo(base.Syntax, method, type, base.HasErrors);
				boundMethodInfo.CopyAttributes(this);
				return boundMethodInfo;
			}
			return this;
		}
	}
}
