using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class RaiseEventStatementSyntax : ExecutableStatementSyntax
	{
		internal IdentifierNameSyntax _name;

		internal ArgumentListSyntax _argumentList;

		public SyntaxToken RaiseEventKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax)base.Green)._raiseEventKeyword, base.Position, 0);

		public IdentifierNameSyntax Name => GetRed(ref _name, 1);

		public ArgumentListSyntax ArgumentList => GetRed(ref _argumentList, 2);

		internal RaiseEventStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal RaiseEventStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax(kind, errors, annotations, raiseEventKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, (argumentList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green) : null), null, 0)
		{
		}

		public RaiseEventStatementSyntax WithRaiseEventKeyword(SyntaxToken raiseEventKeyword)
		{
			return Update(raiseEventKeyword, Name, ArgumentList);
		}

		public RaiseEventStatementSyntax WithName(IdentifierNameSyntax name)
		{
			return Update(RaiseEventKeyword, name, ArgumentList);
		}

		public RaiseEventStatementSyntax WithArgumentList(ArgumentListSyntax argumentList)
		{
			return Update(RaiseEventKeyword, Name, argumentList);
		}

		public RaiseEventStatementSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArgumentList != null) ? ArgumentList : SyntaxFactory.ArgumentList());
			return WithArgumentList(argumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _name, 
				2 => _argumentList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Name, 
				2 => ArgumentList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitRaiseEventStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitRaiseEventStatement(this);
		}

		public RaiseEventStatementSyntax Update(SyntaxToken raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
		{
			if (raiseEventKeyword != RaiseEventKeyword || name != Name || argumentList != ArgumentList)
			{
				RaiseEventStatementSyntax raiseEventStatementSyntax = SyntaxFactory.RaiseEventStatement(raiseEventKeyword, name, argumentList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(raiseEventStatementSyntax, annotations);
				}
				return raiseEventStatementSyntax;
			}
			return this;
		}
	}
}
