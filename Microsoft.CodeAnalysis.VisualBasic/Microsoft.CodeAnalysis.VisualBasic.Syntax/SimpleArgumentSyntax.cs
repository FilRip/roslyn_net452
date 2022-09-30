using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SimpleArgumentSyntax : ArgumentSyntax
	{
		internal NameColonEqualsSyntax _nameColonEquals;

		internal ExpressionSyntax _expression;

		public NameColonEqualsSyntax NameColonEquals => GetRedAtZero(ref _nameColonEquals);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public sealed override bool IsNamed => NameColonEquals != null;

		internal SimpleArgumentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SimpleArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax(kind, errors, annotations, (nameColonEquals != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)nameColonEquals.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public SimpleArgumentSyntax WithNameColonEquals(NameColonEqualsSyntax nameColonEquals)
		{
			return Update(nameColonEquals, Expression);
		}

		public SimpleArgumentSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(NameColonEquals, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _nameColonEquals, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => NameColonEquals, 
				1 => Expression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSimpleArgument(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSimpleArgument(this);
		}

		public SimpleArgumentSyntax Update(NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
		{
			if (nameColonEquals != NameColonEquals || expression != Expression)
			{
				SimpleArgumentSyntax simpleArgumentSyntax = SyntaxFactory.SimpleArgument(nameColonEquals, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(simpleArgumentSyntax, annotations);
				}
				return simpleArgumentSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override ExpressionSyntax GetExpression()
		{
			return Expression;
		}
	}
}
