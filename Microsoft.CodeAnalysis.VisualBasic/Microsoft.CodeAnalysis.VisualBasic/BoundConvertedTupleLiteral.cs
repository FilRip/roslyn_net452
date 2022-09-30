using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConvertedTupleLiteral : BoundTupleExpression
	{
		private readonly TypeSymbol _NaturalTypeOpt;

		public TypeSymbol NaturalTypeOpt => _NaturalTypeOpt;

		public BoundConvertedTupleLiteral(SyntaxNode syntax, TypeSymbol naturalTypeOpt, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ConvertedTupleLiteral, syntax, arguments, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(arguments))
		{
			_NaturalTypeOpt = naturalTypeOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConvertedTupleLiteral(this);
		}

		public BoundConvertedTupleLiteral Update(TypeSymbol naturalTypeOpt, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
		{
			if ((object)naturalTypeOpt != NaturalTypeOpt || arguments != base.Arguments || (object)type != base.Type)
			{
				BoundConvertedTupleLiteral boundConvertedTupleLiteral = new BoundConvertedTupleLiteral(base.Syntax, naturalTypeOpt, arguments, type, base.HasErrors);
				boundConvertedTupleLiteral.CopyAttributes(this);
				return boundConvertedTupleLiteral;
			}
			return this;
		}
	}
}
