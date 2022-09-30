using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConditionalAccessReceiverPlaceholder : BoundRValuePlaceholderBase
	{
		private readonly int _PlaceholderId;

		public int PlaceholderId => _PlaceholderId;

		public BoundConditionalAccessReceiverPlaceholder(SyntaxNode syntax, int placeholderId, TypeSymbol type, bool hasErrors)
			: base(BoundKind.ConditionalAccessReceiverPlaceholder, syntax, type, hasErrors)
		{
			_PlaceholderId = placeholderId;
		}

		public BoundConditionalAccessReceiverPlaceholder(SyntaxNode syntax, int placeholderId, TypeSymbol type)
			: base(BoundKind.ConditionalAccessReceiverPlaceholder, syntax, type)
		{
			_PlaceholderId = placeholderId;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConditionalAccessReceiverPlaceholder(this);
		}

		public BoundConditionalAccessReceiverPlaceholder Update(int placeholderId, TypeSymbol type)
		{
			if (placeholderId != PlaceholderId || (object)type != base.Type)
			{
				BoundConditionalAccessReceiverPlaceholder boundConditionalAccessReceiverPlaceholder = new BoundConditionalAccessReceiverPlaceholder(base.Syntax, placeholderId, type, base.HasErrors);
				boundConditionalAccessReceiverPlaceholder.CopyAttributes(this);
				return boundConditionalAccessReceiverPlaceholder;
			}
			return this;
		}
	}
}
