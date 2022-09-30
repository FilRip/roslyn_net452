using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class RangeCaseClauseSyntax : CaseClauseSyntax
	{
		internal readonly ExpressionSyntax _lowerBound;

		internal readonly KeywordSyntax _toKeyword;

		internal readonly ExpressionSyntax _upperBound;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax LowerBound => _lowerBound;

		internal KeywordSyntax ToKeyword => _toKeyword;

		internal ExpressionSyntax UpperBound => _upperBound;

		internal RangeCaseClauseSyntax(SyntaxKind kind, ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lowerBound);
			_lowerBound = lowerBound;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(upperBound);
			_upperBound = upperBound;
		}

		internal RangeCaseClauseSyntax(SyntaxKind kind, ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lowerBound);
			_lowerBound = lowerBound;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(upperBound);
			_upperBound = upperBound;
		}

		internal RangeCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lowerBound);
			_lowerBound = lowerBound;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(upperBound);
			_upperBound = upperBound;
		}

		internal RangeCaseClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_lowerBound = expressionSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_toKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_upperBound = expressionSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lowerBound);
			writer.WriteValue(_toKeyword);
			writer.WriteValue(_upperBound);
		}

		static RangeCaseClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new RangeCaseClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(RangeCaseClauseSyntax), (ObjectReader r) => new RangeCaseClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lowerBound, 
				1 => _toKeyword, 
				2 => _upperBound, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new RangeCaseClauseSyntax(base.Kind, newErrors, GetAnnotations(), _lowerBound, _toKeyword, _upperBound);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new RangeCaseClauseSyntax(base.Kind, GetDiagnostics(), annotations, _lowerBound, _toKeyword, _upperBound);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitRangeCaseClause(this);
		}
	}
}
