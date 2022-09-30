using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundPropertyGroup : BoundMethodOrPropertyGroup
	{
		private readonly ImmutableArray<PropertySymbol> _Properties;

		private readonly LookupResultKind _ResultKind;

		public ImmutableArray<PropertySymbol> Properties => _Properties;

		public override LookupResultKind ResultKind => _ResultKind;

		public BoundPropertyGroup(SyntaxNode syntax, ImmutableArray<PropertySymbol> properties, LookupResultKind resultKind, BoundExpression receiverOpt, QualificationKind qualificationKind, bool hasErrors = false)
			: base(BoundKind.PropertyGroup, syntax, receiverOpt, qualificationKind, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt))
		{
			_Properties = properties;
			_ResultKind = resultKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitPropertyGroup(this);
		}

		public BoundPropertyGroup Update(ImmutableArray<PropertySymbol> properties, LookupResultKind resultKind, BoundExpression receiverOpt, QualificationKind qualificationKind)
		{
			if (properties != Properties || resultKind != ResultKind || receiverOpt != base.ReceiverOpt || qualificationKind != base.QualificationKind)
			{
				BoundPropertyGroup boundPropertyGroup = new BoundPropertyGroup(base.Syntax, properties, resultKind, receiverOpt, qualificationKind, base.HasErrors);
				boundPropertyGroup.CopyAttributes(this);
				return boundPropertyGroup;
			}
			return this;
		}
	}
}
