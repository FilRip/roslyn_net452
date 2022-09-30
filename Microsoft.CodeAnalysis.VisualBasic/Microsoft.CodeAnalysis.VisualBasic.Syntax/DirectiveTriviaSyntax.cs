using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class DirectiveTriviaSyntax : StructuredTriviaSyntax
	{
		private static readonly Func<SyntaxToken, bool> s_hasDirectivesFunction = (SyntaxToken n) => n.ContainsDirectives;

		public SyntaxToken HashToken => GetHashTokenCore();

		internal DirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetHashTokenCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);
		}

		public DirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return WithHashTokenCore(hashToken);
		}

		internal abstract DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken);

		public List<DirectiveTriviaSyntax> GetRelatedDirectives()
		{
			List<DirectiveTriviaSyntax> list = new List<DirectiveTriviaSyntax>();
			GetRelatedDirectives(list);
			return list;
		}

		private void GetRelatedDirectives(List<DirectiveTriviaSyntax> list)
		{
			list.Clear();
			for (DirectiveTriviaSyntax previousRelatedDirective = GetPreviousRelatedDirective(); previousRelatedDirective != null; previousRelatedDirective = previousRelatedDirective.GetPreviousRelatedDirective())
			{
				list.Add(previousRelatedDirective);
			}
			list.Reverse();
			list.Add(this);
			for (DirectiveTriviaSyntax nextRelatedDirective = GetNextRelatedDirective(); nextRelatedDirective != null; nextRelatedDirective = nextRelatedDirective.GetNextRelatedDirective())
			{
				list.Add(nextRelatedDirective);
			}
		}

		private DirectiveTriviaSyntax GetNextRelatedDirective()
		{
			DirectiveTriviaSyntax directiveTriviaSyntax = this;
			switch (directiveTriviaSyntax.Kind())
			{
			case SyntaxKind.IfDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind2 = directiveTriviaSyntax.Kind();
					if (syntaxKind2 - 738 <= SyntaxKind.EmptyStatement)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.ElseIfDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
					if (syntaxKind - 739 <= SyntaxKind.List)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.ElseDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					if (directiveTriviaSyntax.Kind() == SyntaxKind.EndIfDirectiveTrivia)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.RegionDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					if (directiveTriviaSyntax.Kind() == SyntaxKind.EndRegionDirectiveTrivia)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
				}
				break;
			}
			return null;
		}

		private DirectiveTriviaSyntax GetNextPossiblyRelatedDirective()
		{
			DirectiveTriviaSyntax directiveTriviaSyntax = this;
			while (directiveTriviaSyntax != null)
			{
				directiveTriviaSyntax = directiveTriviaSyntax.GetNextDirective();
				if (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
					if (syntaxKind == SyntaxKind.IfDirectiveTrivia)
					{
						while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.EndIfDirectiveTrivia)
						{
							directiveTriviaSyntax = directiveTriviaSyntax.GetNextRelatedDirective();
						}
						continue;
					}
					if (syntaxKind == SyntaxKind.RegionDirectiveTrivia)
					{
						while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.EndRegionDirectiveTrivia)
						{
							directiveTriviaSyntax = directiveTriviaSyntax.GetNextRelatedDirective();
						}
						continue;
					}
				}
				return directiveTriviaSyntax;
			}
			return null;
		}

		private DirectiveTriviaSyntax GetPreviousRelatedDirective()
		{
			DirectiveTriviaSyntax directiveTriviaSyntax = this;
			switch (directiveTriviaSyntax.Kind())
			{
			case SyntaxKind.EndIfDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind2 = directiveTriviaSyntax.Kind();
					if (syntaxKind2 - 737 <= SyntaxKind.EmptyStatement)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.ElseIfDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					if (directiveTriviaSyntax.Kind() == SyntaxKind.IfDirectiveTrivia)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.ElseDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
					if (syntaxKind - 737 <= SyntaxKind.List)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
				}
				break;
			case SyntaxKind.EndRegionDirectiveTrivia:
				while (directiveTriviaSyntax != null)
				{
					if (directiveTriviaSyntax.Kind() == SyntaxKind.RegionDirectiveTrivia)
					{
						return directiveTriviaSyntax;
					}
					directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
				}
				break;
			}
			return null;
		}

		private DirectiveTriviaSyntax GetPreviousPossiblyRelatedDirective()
		{
			DirectiveTriviaSyntax directiveTriviaSyntax = this;
			while (directiveTriviaSyntax != null)
			{
				directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousDirective();
				if (directiveTriviaSyntax != null)
				{
					SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
					if (syntaxKind == SyntaxKind.EndIfDirectiveTrivia)
					{
						while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.IfDirectiveTrivia)
						{
							directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousRelatedDirective();
						}
						continue;
					}
					if (syntaxKind == SyntaxKind.EndRegionDirectiveTrivia)
					{
						while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.RegionDirectiveTrivia)
						{
							directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousRelatedDirective();
						}
						continue;
					}
				}
				return directiveTriviaSyntax;
			}
			return null;
		}

		public DirectiveTriviaSyntax GetNextDirective(Func<DirectiveTriviaSyntax, bool> predicate = null)
		{
			SyntaxToken token = base.ParentTrivia.Token;
			bool flag = false;
			while (VisualBasicExtensions.Kind(token) != 0)
			{
				SyntaxTriviaList.Enumerator enumerator = token.LeadingTrivia.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia current = enumerator.Current;
					if (flag)
					{
						if (current.IsDirective)
						{
							DirectiveTriviaSyntax directiveTriviaSyntax = (DirectiveTriviaSyntax)current.GetStructure();
							if (predicate == null || predicate(directiveTriviaSyntax))
							{
								return directiveTriviaSyntax;
							}
						}
					}
					else if (current.UnderlyingNode == base.Green)
					{
						flag = true;
					}
				}
				token = token.GetNextToken(s_hasDirectivesFunction);
			}
			return null;
		}

		public DirectiveTriviaSyntax GetPreviousDirective(Func<DirectiveTriviaSyntax, bool> predicate = null)
		{
			SyntaxToken token = base.ParentTrivia.Token;
			bool flag = false;
			while (VisualBasicExtensions.Kind(token) != 0)
			{
				SyntaxTriviaList.Reversed.Enumerator enumerator = token.LeadingTrivia.Reverse().GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia current = enumerator.Current;
					if (flag)
					{
						if (current.IsDirective)
						{
							DirectiveTriviaSyntax directiveTriviaSyntax = (DirectiveTriviaSyntax)current.GetStructure();
							if (predicate == null || predicate(directiveTriviaSyntax))
							{
								return directiveTriviaSyntax;
							}
						}
					}
					else if (current.UnderlyingNode == base.Green)
					{
						flag = true;
					}
				}
				token = token.GetPreviousToken(s_hasDirectivesFunction);
			}
			return null;
		}
	}
}
