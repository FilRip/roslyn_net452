namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class NameSyntax : TypeSyntax
	{
		public int Arity
		{
			get
			{
				if (this is GenericNameSyntax)
				{
					return ((GenericNameSyntax)this).TypeArgumentList.Arguments.Count;
				}
				return 0;
			}
		}

		internal NameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
