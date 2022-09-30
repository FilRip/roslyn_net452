using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedRegularMethodBase : SynthesizedMethodBase
	{
		protected readonly string m_name;

		protected readonly bool m_isShared;

		protected readonly VisualBasicSyntaxNode m_SyntaxNode;

		public sealed override string Name => m_name;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public sealed override bool IsShared => m_isShared;

		internal sealed override bool HasSpecialName => false;

		public override ImmutableArray<Location> Locations => m_containingType.Locations;

		public sealed override MethodKind MethodKind => MethodKind.Ordinary;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		internal sealed override SyntaxNode Syntax => m_SyntaxNode;

		protected SynthesizedRegularMethodBase(VisualBasicSyntaxNode syntaxNode, NamedTypeSymbol container, string name, bool isShared = false)
			: base(container)
		{
			m_SyntaxNode = syntaxNode;
			m_isShared = isShared;
			m_name = name;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return m_containingType.GetLexicalSortKey();
		}
	}
}
