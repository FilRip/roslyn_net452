using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class MissingCorLibrarySymbol : MissingAssemblySymbol
	{
		internal static readonly MissingCorLibrarySymbol Instance = new MissingCorLibrarySymbol();

		private NamedTypeSymbol[] _lazySpecialTypes;

		private MissingCorLibrarySymbol()
			: base(new AssemblyIdentity("<Missing Core Assembly>"))
		{
			SetCorLibrary(this);
		}

		internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
		{
			if (_lazySpecialTypes == null)
			{
				Interlocked.CompareExchange(ref _lazySpecialTypes, new NamedTypeSymbol[46], null);
			}
			if ((object)_lazySpecialTypes[(int)type] == null)
			{
				MetadataTypeName fullname = MetadataTypeName.FromFullName(type.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
				NamedTypeSymbol value = new MissingMetadataTypeSymbol.TopLevel(m_ModuleSymbol, ref fullname, type);
				Interlocked.CompareExchange(ref _lazySpecialTypes[(int)type], value, null);
			}
			return _lazySpecialTypes[(int)type];
		}
	}
}
