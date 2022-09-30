using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class EmbeddedTreeLocation : VBLocation
	{
		internal readonly EmbeddedSymbolKind _embeddedKind;

		internal readonly TextSpan _span;

		public override LocationKind Kind => LocationKind.None;

		internal override EmbeddedSymbolKind EmbeddedKind => _embeddedKind;

		internal override TextSpan PossiblyEmbeddedOrMySourceSpan => _span;

		internal override SyntaxTree PossiblyEmbeddedOrMySourceTree => EmbeddedSymbolManager.GetEmbeddedTree(_embeddedKind);

		public EmbeddedTreeLocation(EmbeddedSymbolKind embeddedKind, TextSpan span)
		{
			_embeddedKind = embeddedKind;
			_span = span;
		}

		public bool Equals(EmbeddedTreeLocation other)
		{
			if ((object)this == other)
			{
				return true;
			}
			int result;
			if ((object)other != null && other.EmbeddedKind == _embeddedKind)
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
			return Equals(obj as EmbeddedTreeLocation);
		}

		public override int GetHashCode()
		{
			int hashCode = ((int)_embeddedKind).GetHashCode();
			TextSpan span = _span;
			return Hash.Combine(hashCode, span.GetHashCode());
		}
	}
}
