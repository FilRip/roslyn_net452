using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class NewExpressionSyntax : ExpressionSyntax
	{
		internal SyntaxNode _attributeLists;

		public SyntaxToken NewKeyword => GetNewKeywordCore();

		public SyntaxList<AttributeListSyntax> AttributeLists => GetAttributeListsCore();

		internal NewExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetNewKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax)base.Green)._newKeyword, base.Position, 0);
		}

		public NewExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
		{
			return WithNewKeywordCore(newKeyword);
		}

		internal abstract NewExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword);

		internal virtual SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			SyntaxNode red = GetRed(ref _attributeLists, 1);
			return new SyntaxList<AttributeListSyntax>(red);
		}

		public NewExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeListsCore(attributeLists);
		}

		internal abstract NewExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

		public NewExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return AddAttributeListsCore(items);
		}

		internal abstract NewExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items);
	}
}
