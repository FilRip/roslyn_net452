using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class LabelSymbol : Symbol, ILabelSymbol
	{
		private readonly string _name;

		public override string Name => _name;

		public override bool IsNotOverridable => false;

		public override bool IsMustOverride => false;

		public override bool IsOverrides => false;

		public override bool IsOverridable => false;

		public override bool IsShared => false;

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public virtual MethodSymbol ContainingMethod => null;

		public override Symbol ContainingSymbol => ContainingMethod;

		public override SymbolKind Kind => SymbolKind.Label;

		internal virtual SyntaxToken LabelName => default(SyntaxToken);

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal IMethodSymbol ILabelSymbol_ContainingMethod => ContainingMethod;

		public LabelSymbol(string name)
		{
			_name = name;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitLabel(this, arg);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitLabel(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitLabel(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitLabel(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitLabel(this);
		}
	}
}
