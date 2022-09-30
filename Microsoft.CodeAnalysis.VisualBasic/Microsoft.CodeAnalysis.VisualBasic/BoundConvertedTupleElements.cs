using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConvertedTupleElements : BoundExtendedConversionInfo
	{
		private readonly ImmutableArray<BoundRValuePlaceholder> _ElementPlaceholders;

		private readonly ImmutableArray<BoundExpression> _ConvertedElements;

		public ImmutableArray<BoundRValuePlaceholder> ElementPlaceholders => _ElementPlaceholders;

		public ImmutableArray<BoundExpression> ConvertedElements => _ConvertedElements;

		public BoundConvertedTupleElements(SyntaxNode syntax, ImmutableArray<BoundRValuePlaceholder> elementPlaceholders, ImmutableArray<BoundExpression> convertedElements, bool hasErrors = false)
			: base(BoundKind.ConvertedTupleElements, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(elementPlaceholders) || BoundNodeExtensions.NonNullAndHasErrors(convertedElements))
		{
			_ElementPlaceholders = elementPlaceholders;
			_ConvertedElements = convertedElements;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConvertedTupleElements(this);
		}

		public BoundConvertedTupleElements Update(ImmutableArray<BoundRValuePlaceholder> elementPlaceholders, ImmutableArray<BoundExpression> convertedElements)
		{
			if (elementPlaceholders != ElementPlaceholders || convertedElements != ConvertedElements)
			{
				BoundConvertedTupleElements boundConvertedTupleElements = new BoundConvertedTupleElements(base.Syntax, elementPlaceholders, convertedElements, base.HasErrors);
				boundConvertedTupleElements.CopyAttributes(this);
				return boundConvertedTupleElements;
			}
			return this;
		}
	}
}
