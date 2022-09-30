using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class PreprocessingSymbol : Symbol, IPreprocessingSymbol
	{
		private readonly string _name;

		public override string Name => _name;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<VisualBasicSyntaxNode>(Locations);

		public override SymbolKind Kind => SymbolKind.Preprocessing;

		public override Symbol ContainingSymbol => null;

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal PreprocessingSymbol(string name)
		{
			_name = name;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			return obj is PreprocessingSymbol preprocessingSymbol && CaseInsensitiveComparison.Equals(Name, preprocessingSymbol.Name);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override void Accept(SymbolVisitor visitor)
		{
			throw new NotSupportedException();
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			throw new NotSupportedException();
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			throw new NotSupportedException();
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			throw new NotSupportedException();
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			throw new NotSupportedException();
		}
	}
}
