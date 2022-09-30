using System.Diagnostics;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSourceDocumentIndex : BoundExpression
	{
		private readonly DebugSourceDocument _Document;

		public DebugSourceDocument Document => _Document;

		public BoundSourceDocumentIndex(SyntaxNode syntax, DebugSourceDocument document, TypeSymbol type, bool hasErrors)
			: base(BoundKind.SourceDocumentIndex, syntax, type, hasErrors)
		{
			_Document = document;
		}

		public BoundSourceDocumentIndex(SyntaxNode syntax, DebugSourceDocument document, TypeSymbol type)
			: base(BoundKind.SourceDocumentIndex, syntax, type)
		{
			_Document = document;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSourceDocumentIndex(this);
		}

		public BoundSourceDocumentIndex Update(DebugSourceDocument document, TypeSymbol type)
		{
			if (document != Document || (object)type != base.Type)
			{
				BoundSourceDocumentIndex boundSourceDocumentIndex = new BoundSourceDocumentIndex(base.Syntax, document, type, base.HasErrors);
				boundSourceDocumentIndex.CopyAttributes(this);
				return boundSourceDocumentIndex;
			}
			return this;
		}
	}
}
