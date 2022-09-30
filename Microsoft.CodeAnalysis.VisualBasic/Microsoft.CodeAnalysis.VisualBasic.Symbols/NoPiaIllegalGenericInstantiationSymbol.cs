using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class NoPiaIllegalGenericInstantiationSymbol : ErrorTypeSymbol
	{
		private readonly NamedTypeSymbol _underlyingSymbol;

		public NamedTypeSymbol UnderlyingSymbol => _underlyingSymbol;

		internal override bool MangleName => false;

		internal override DiagnosticInfo ErrorInfo
		{
			get
			{
				if (TypeSymbolExtensions.IsErrorType(_underlyingSymbol))
				{
					DiagnosticInfo errorInfo = ((ErrorTypeSymbol)_underlyingSymbol).ErrorInfo;
					if (errorInfo != null)
					{
						return errorInfo;
					}
				}
				return ErrorFactory.ErrorInfo(ERRID.ERR_CannotUseGenericTypeAcrossAssemblyBoundaries, _underlyingSymbol);
			}
		}

		public NoPiaIllegalGenericInstantiationSymbol(NamedTypeSymbol underlyingSymbol)
		{
			_underlyingSymbol = underlyingSymbol;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			return (object)obj == this;
		}
	}
}
