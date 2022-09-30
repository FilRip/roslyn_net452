using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundToQueryableCollectionConversion : BoundQueryPart
	{
		private readonly BoundCall _ConversionCall;

		public override Symbol ExpressionSymbol => ConversionCall.ExpressionSymbol;

		public override LookupResultKind ResultKind => ConversionCall.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)ConversionCall);

		public BoundCall ConversionCall => _ConversionCall;

		public BoundToQueryableCollectionConversion(BoundCall call)
			: this(call.Syntax, call, call.Type)
		{
		}

		public BoundToQueryableCollectionConversion(SyntaxNode syntax, BoundCall conversionCall, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ToQueryableCollectionConversion, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(conversionCall))
		{
			_ConversionCall = conversionCall;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitToQueryableCollectionConversion(this);
		}

		public BoundToQueryableCollectionConversion Update(BoundCall conversionCall, TypeSymbol type)
		{
			if (conversionCall != ConversionCall || (object)type != base.Type)
			{
				BoundToQueryableCollectionConversion boundToQueryableCollectionConversion = new BoundToQueryableCollectionConversion(base.Syntax, conversionCall, type, base.HasErrors);
				boundToQueryableCollectionConversion.CopyAttributes(this);
				return boundToQueryableCollectionConversion;
			}
			return this;
		}
	}
}
