using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMethodDefIndex : BoundExpression
	{
		private readonly MethodSymbol _Method;

		public MethodSymbol Method => _Method;

		public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MethodDefIndex, syntax, type, hasErrors)
		{
			_Method = method;
		}

		public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type)
			: base(BoundKind.MethodDefIndex, syntax, type)
		{
			_Method = method;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMethodDefIndex(this);
		}

		public BoundMethodDefIndex Update(MethodSymbol method, TypeSymbol type)
		{
			if ((object)method != Method || (object)type != base.Type)
			{
				BoundMethodDefIndex boundMethodDefIndex = new BoundMethodDefIndex(base.Syntax, method, type, base.HasErrors);
				boundMethodDefIndex.CopyAttributes(this);
				return boundMethodDefIndex;
			}
			return this;
		}
	}
}
