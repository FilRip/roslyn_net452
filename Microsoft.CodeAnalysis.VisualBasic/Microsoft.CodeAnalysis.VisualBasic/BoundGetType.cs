using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundGetType : BoundExpression
	{
		private readonly BoundTypeExpression _SourceType;

		public BoundTypeExpression SourceType => _SourceType;

		public BoundGetType(SyntaxNode syntax, BoundTypeExpression sourceType, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.GetType, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(sourceType))
		{
			_SourceType = sourceType;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitGetType(this);
		}

		public BoundGetType Update(BoundTypeExpression sourceType, TypeSymbol type)
		{
			if (sourceType != SourceType || (object)type != base.Type)
			{
				BoundGetType boundGetType = new BoundGetType(base.Syntax, sourceType, type, base.HasErrors);
				boundGetType.CopyAttributes(this);
				return boundGetType;
			}
			return this;
		}
	}
}
