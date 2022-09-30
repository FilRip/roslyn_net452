using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceLabelSymbol : LabelSymbol
	{
		private readonly SyntaxToken _labelName;

		private readonly MethodSymbol _containingMethod;

		private readonly Binder _binder;

		internal override SyntaxToken LabelName => _labelName;

		public override MethodSymbol ContainingMethod => _containingMethod;

		public override Symbol ContainingSymbol => _containingMethod;

		public override ImmutableArray<Location> Locations
		{
			get
			{
				SyntaxToken labelName = _labelName;
				return ImmutableArray.Create(labelName.GetLocation());
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				SyntaxToken labelName = _labelName;
				return ImmutableArray.Create(labelName.Parent!.GetReference());
			}
		}

		public SourceLabelSymbol(SyntaxToken labelNameToken, MethodSymbol containingMethod, Binder binder)
			: base(labelNameToken.ValueText)
		{
			_labelName = labelNameToken;
			_containingMethod = containingMethod;
			_binder = binder;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			int result;
			if (obj is SourceLabelSymbol sourceLabelSymbol)
			{
				SyntaxToken labelName = sourceLabelSymbol._labelName;
				if (labelName.Equals(_labelName))
				{
					result = (object.Equals(sourceLabelSymbol._containingMethod, _containingMethod) ? 1 : 0);
					goto IL_003c;
				}
			}
			result = 0;
			goto IL_003c;
			IL_003c:
			return (byte)result != 0;
		}

		public override int GetHashCode()
		{
			SyntaxToken labelName = _labelName;
			return labelName.GetHashCode();
		}
	}
}
