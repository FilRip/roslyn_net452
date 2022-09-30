using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NamedFieldInitializerSyntax : FieldInitializerSyntax
	{
		internal IdentifierNameSyntax _name;

		internal ExpressionSyntax _expression;

		public new SyntaxToken KeyKeyword
		{
			get
			{
				KeywordSyntax keyKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)base.Green)._keyKeyword;
				return (keyKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, keyKeyword, base.Position, 0);
			}
		}

		public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)base.Green)._dotToken, GetChildPosition(1), GetChildIndex(1));

		public IdentifierNameSyntax Name => GetRed(ref _name, 2);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)base.Green)._equalsToken, GetChildPosition(3), GetChildIndex(3));

		public ExpressionSyntax Expression => GetRed(ref _expression, 4);

		internal NamedFieldInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NamedFieldInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax(kind, errors, annotations, keyKeyword, dotToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, equalsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		internal override SyntaxToken GetKeyKeywordCore()
		{
			return KeyKeyword;
		}

		internal override FieldInitializerSyntax WithKeyKeywordCore(SyntaxToken keyKeyword)
		{
			return WithKeyKeyword(keyKeyword);
		}

		public new NamedFieldInitializerSyntax WithKeyKeyword(SyntaxToken keyKeyword)
		{
			return Update(keyKeyword, DotToken, Name, EqualsToken, Expression);
		}

		public NamedFieldInitializerSyntax WithDotToken(SyntaxToken dotToken)
		{
			return Update(KeyKeyword, dotToken, Name, EqualsToken, Expression);
		}

		public NamedFieldInitializerSyntax WithName(IdentifierNameSyntax name)
		{
			return Update(KeyKeyword, DotToken, name, EqualsToken, Expression);
		}

		public NamedFieldInitializerSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(KeyKeyword, DotToken, Name, equalsToken, Expression);
		}

		public NamedFieldInitializerSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(KeyKeyword, DotToken, Name, EqualsToken, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _name, 
				4 => _expression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => Name, 
				4 => Expression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNamedFieldInitializer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNamedFieldInitializer(this);
		}

		public NamedFieldInitializerSyntax Update(SyntaxToken keyKeyword, SyntaxToken dotToken, IdentifierNameSyntax name, SyntaxToken equalsToken, ExpressionSyntax expression)
		{
			if (keyKeyword != KeyKeyword || dotToken != DotToken || name != Name || equalsToken != EqualsToken || expression != Expression)
			{
				NamedFieldInitializerSyntax namedFieldInitializerSyntax = SyntaxFactory.NamedFieldInitializer(keyKeyword, dotToken, name, equalsToken, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(namedFieldInitializerSyntax, annotations);
				}
				return namedFieldInitializerSyntax;
			}
			return this;
		}
	}
}
