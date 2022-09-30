using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SourceFileBinder : Binder
	{
		private readonly SourceFile _sourceFile;

		public override OptionStrict OptionStrict
		{
			get
			{
				if (_sourceFile.OptionStrict.HasValue)
				{
					return _sourceFile.OptionStrict.Value ? OptionStrict.On : OptionStrict.Off;
				}
				return m_containingBinder.OptionStrict;
			}
		}

		public override bool OptionInfer
		{
			get
			{
				if (_sourceFile.OptionInfer.HasValue)
				{
					return _sourceFile.OptionInfer.Value;
				}
				return m_containingBinder.OptionInfer;
			}
		}

		public override bool OptionExplicit
		{
			get
			{
				if (_sourceFile.OptionExplicit.HasValue)
				{
					return _sourceFile.OptionExplicit.Value;
				}
				return m_containingBinder.OptionExplicit;
			}
		}

		public override bool OptionCompareText
		{
			get
			{
				if (_sourceFile.OptionCompareText.HasValue)
				{
					return _sourceFile.OptionCompareText.Value;
				}
				return m_containingBinder.OptionCompareText;
			}
		}

		public override QuickAttributeChecker QuickAttributeChecker => _sourceFile.QuickAttributeChecker;

		public SourceFileBinder(Binder containingBinder, SourceFile sourceFile, SyntaxTree tree)
			: base(containingBinder, tree)
		{
			_sourceFile = sourceFile;
		}

		public override SyntaxReference GetSyntaxReference(VisualBasicSyntaxNode node)
		{
			return base.SyntaxTree.GetReference(node);
		}
	}
}
