using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	internal class SynthesizedLocal : LocalSymbol
	{
		private readonly SynthesizedLocalKind _kind;

		private readonly bool _isByRef;

		private readonly SyntaxNode _syntaxOpt;

		public override ImmutableArray<Location> Locations
		{
			get
			{
				if (_syntaxOpt != null)
				{
					return ImmutableArray.Create(_syntaxOpt.GetLocation());
				}
				return ImmutableArray<Location>.Empty;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				if (_syntaxOpt != null)
				{
					return ImmutableArray.Create(_syntaxOpt.GetReference());
				}
				return ImmutableArray<SyntaxReference>.Empty;
			}
		}

		internal sealed override SyntaxToken IdentifierToken => default(SyntaxToken);

		internal sealed override Location IdentifierLocation => NoLocation.Singleton;

		public override string Name => null;

		internal override bool IsByRef => _isByRef;

		internal override SynthesizedLocalKind SynthesizedKind => _kind;

		internal override LocalDeclarationKind DeclarationKind => LocalDeclarationKind.None;

		public override bool IsFunctionValue => _kind == SynthesizedLocalKind.FunctionReturnValue;

		internal SynthesizedLocal(Symbol container, TypeSymbol type, SynthesizedLocalKind kind, SyntaxNode syntaxOpt = null, bool isByRef = false)
			: base(container, type)
		{
			_kind = kind;
			_syntaxOpt = syntaxOpt;
			_isByRef = isByRef;
		}

		internal override SyntaxNode GetDeclaratorSyntax()
		{
			return _syntaxOpt;
		}

		private string GetDebuggerDisplay()
		{
			StringBuilder stringBuilder = new StringBuilder();
			SynthesizedLocalKind kind = _kind;
			stringBuilder.Append(kind.ToString());
			stringBuilder.Append(" ");
			stringBuilder.Append(Type.ToDisplayString(SymbolDisplayFormat.TestFormat));
			return stringBuilder.ToString();
		}
	}
}
