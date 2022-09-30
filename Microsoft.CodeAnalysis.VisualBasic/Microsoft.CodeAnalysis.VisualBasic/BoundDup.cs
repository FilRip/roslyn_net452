using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundDup : BoundExpression
	{
		private readonly bool _IsReference;

		public bool IsReference => _IsReference;

		public BoundDup(SyntaxNode syntax, bool isReference, TypeSymbol type, bool hasErrors)
			: base(BoundKind.Dup, syntax, type, hasErrors)
		{
			_IsReference = isReference;
		}

		public BoundDup(SyntaxNode syntax, bool isReference, TypeSymbol type)
			: base(BoundKind.Dup, syntax, type)
		{
			_IsReference = isReference;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitDup(this);
		}

		public BoundDup Update(bool isReference, TypeSymbol type)
		{
			if (isReference != IsReference || (object)type != base.Type)
			{
				BoundDup boundDup = new BoundDup(base.Syntax, isReference, type, base.HasErrors);
				boundDup.CopyAttributes(this);
				return boundDup;
			}
			return this;
		}
	}
}
