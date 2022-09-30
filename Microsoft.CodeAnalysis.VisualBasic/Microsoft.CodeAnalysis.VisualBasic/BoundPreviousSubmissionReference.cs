using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundPreviousSubmissionReference : BoundExpression
	{
		private readonly NamedTypeSymbol _SourceType;

		public NamedTypeSymbol SourceType => _SourceType;

		public BoundPreviousSubmissionReference(SyntaxNode syntax, NamedTypeSymbol sourceType, TypeSymbol type, bool hasErrors)
			: base(BoundKind.PreviousSubmissionReference, syntax, type, hasErrors)
		{
			_SourceType = sourceType;
		}

		public BoundPreviousSubmissionReference(SyntaxNode syntax, NamedTypeSymbol sourceType, TypeSymbol type)
			: base(BoundKind.PreviousSubmissionReference, syntax, type)
		{
			_SourceType = sourceType;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitPreviousSubmissionReference(this);
		}

		public BoundPreviousSubmissionReference Update(NamedTypeSymbol sourceType, TypeSymbol type)
		{
			if ((object)sourceType != SourceType || (object)type != base.Type)
			{
				BoundPreviousSubmissionReference boundPreviousSubmissionReference = new BoundPreviousSubmissionReference(base.Syntax, sourceType, type, base.HasErrors);
				boundPreviousSubmissionReference.CopyAttributes(this);
				return boundPreviousSubmissionReference;
			}
			return this;
		}
	}
}
