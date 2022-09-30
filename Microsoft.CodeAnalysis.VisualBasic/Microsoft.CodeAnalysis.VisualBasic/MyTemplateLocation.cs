using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MyTemplateLocation : VBLocation
	{
		private readonly TextSpan _span;

		private readonly SyntaxTree _tree;

		public override LocationKind Kind => LocationKind.None;

		internal override TextSpan PossiblyEmbeddedOrMySourceSpan => _span;

		internal override SyntaxTree PossiblyEmbeddedOrMySourceTree => _tree;

		public MyTemplateLocation(SyntaxTree tree, TextSpan span)
		{
			_span = span;
			_tree = tree;
		}

		public bool Equals(MyTemplateLocation other)
		{
			if ((object)this == other)
			{
				return true;
			}
			int result;
			if ((object)other != null && _tree == other._tree)
			{
				TextSpan span = other._span;
				result = (span.Equals(_span) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MyTemplateLocation);
		}

		public override int GetHashCode()
		{
			TextSpan span = _span;
			return span.GetHashCode();
		}
	}
}
