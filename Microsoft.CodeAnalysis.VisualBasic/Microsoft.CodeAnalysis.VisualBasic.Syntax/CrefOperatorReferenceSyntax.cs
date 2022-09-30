using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CrefOperatorReferenceSyntax : NameSyntax
	{
		public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)base.Green)._operatorKeyword, base.Position, 0);

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)base.Green)._operatorToken, GetChildPosition(1), GetChildIndex(1));

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use OperatorKeyword or a more specific property (e.g. OperatorKeyword) instead.", true)]
		public SyntaxToken Keyword => OperatorKeyword;

		internal CrefOperatorReferenceSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CrefOperatorReferenceSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax operatorKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken operatorToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax(kind, errors, annotations, operatorKeyword, operatorToken), null, 0)
		{
		}

		public CrefOperatorReferenceSyntax WithOperatorKeyword(SyntaxToken operatorKeyword)
		{
			return Update(operatorKeyword, OperatorToken);
		}

		public CrefOperatorReferenceSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(OperatorKeyword, operatorToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCrefOperatorReference(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCrefOperatorReference(this);
		}

		public CrefOperatorReferenceSyntax Update(SyntaxToken operatorKeyword, SyntaxToken operatorToken)
		{
			if (operatorKeyword != OperatorKeyword || operatorToken != OperatorToken)
			{
				CrefOperatorReferenceSyntax crefOperatorReferenceSyntax = SyntaxFactory.CrefOperatorReference(operatorKeyword, operatorToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(crefOperatorReferenceSyntax, annotations);
				}
				return crefOperatorReferenceSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use OperatorKeyword or a more specific property (e.g. WithOperatorKeyword) instead.", true)]
		public CrefOperatorReferenceSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithOperatorKeyword(keyword);
		}
	}
}
