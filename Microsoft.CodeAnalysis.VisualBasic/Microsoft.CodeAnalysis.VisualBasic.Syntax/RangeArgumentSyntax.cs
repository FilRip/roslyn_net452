using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class RangeArgumentSyntax : ArgumentSyntax
	{
		internal ExpressionSyntax _lowerBound;

		internal ExpressionSyntax _upperBound;

		public ExpressionSyntax LowerBound => GetRedAtZero(ref _lowerBound);

		public SyntaxToken ToKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax)base.Green)._toKeyword, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax UpperBound => GetRed(ref _upperBound, 2);

		public sealed override bool IsNamed => false;

		internal RangeArgumentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal RangeArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)lowerBound.Green, toKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)upperBound.Green), null, 0)
		{
		}

		public RangeArgumentSyntax WithLowerBound(ExpressionSyntax lowerBound)
		{
			return Update(lowerBound, ToKeyword, UpperBound);
		}

		public RangeArgumentSyntax WithToKeyword(SyntaxToken toKeyword)
		{
			return Update(LowerBound, toKeyword, UpperBound);
		}

		public RangeArgumentSyntax WithUpperBound(ExpressionSyntax upperBound)
		{
			return Update(LowerBound, ToKeyword, upperBound);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _lowerBound, 
				2 => _upperBound, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => LowerBound, 
				2 => UpperBound, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitRangeArgument(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitRangeArgument(this);
		}

		public RangeArgumentSyntax Update(ExpressionSyntax lowerBound, SyntaxToken toKeyword, ExpressionSyntax upperBound)
		{
			if (lowerBound != LowerBound || toKeyword != ToKeyword || upperBound != UpperBound)
			{
				RangeArgumentSyntax rangeArgumentSyntax = SyntaxFactory.RangeArgument(lowerBound, toKeyword, upperBound);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(rangeArgumentSyntax, annotations);
				}
				return rangeArgumentSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override ExpressionSyntax GetExpression()
		{
			return UpperBound;
		}
	}
}
