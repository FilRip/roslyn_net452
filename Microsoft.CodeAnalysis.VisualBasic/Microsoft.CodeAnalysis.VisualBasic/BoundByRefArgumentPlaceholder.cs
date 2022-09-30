using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundByRefArgumentPlaceholder : BoundValuePlaceholderBase
	{
		private readonly bool _IsOut;

		public bool IsOut => _IsOut;

		public BoundByRefArgumentPlaceholder(SyntaxNode syntax, bool isOut, TypeSymbol type, bool hasErrors)
			: base(BoundKind.ByRefArgumentPlaceholder, syntax, type, hasErrors)
		{
			_IsOut = isOut;
		}

		public BoundByRefArgumentPlaceholder(SyntaxNode syntax, bool isOut, TypeSymbol type)
			: base(BoundKind.ByRefArgumentPlaceholder, syntax, type)
		{
			_IsOut = isOut;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitByRefArgumentPlaceholder(this);
		}

		public BoundByRefArgumentPlaceholder Update(bool isOut, TypeSymbol type)
		{
			if (isOut != IsOut || (object)type != base.Type)
			{
				BoundByRefArgumentPlaceholder boundByRefArgumentPlaceholder = new BoundByRefArgumentPlaceholder(base.Syntax, isOut, type, base.HasErrors);
				boundByRefArgumentPlaceholder.CopyAttributes(this);
				return boundByRefArgumentPlaceholder;
			}
			return this;
		}
	}
}
