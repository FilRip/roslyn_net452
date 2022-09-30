using System;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ExtendedErrorTypeSymbol : InstanceErrorTypeSymbol
	{
		private readonly DiagnosticInfo _diagnosticInfo;

		private readonly bool _reportErrorWhenReferenced;

		private readonly string _name;

		private readonly ImmutableArray<Symbol> _candidateSymbols;

		private readonly LookupResultKind _resultKind;

		private readonly NamespaceOrTypeSymbol _containingSymbol;

		public override Symbol ContainingSymbol => _containingSymbol;

		public override ImmutableArray<Symbol> CandidateSymbols => _candidateSymbols;

		internal override LookupResultKind ResultKind => _resultKind;

		internal override DiagnosticInfo ErrorInfo => _diagnosticInfo;

		public override string Name => _name;

		internal override bool MangleName => base.Arity > 0;

		internal ExtendedErrorTypeSymbol(DiagnosticInfo errorInfo, bool reportErrorWhenReferenced = false, NamedTypeSymbol nonErrorGuessType = null)
			: this(errorInfo, string.Empty, 0, reportErrorWhenReferenced, nonErrorGuessType)
		{
		}

		internal ExtendedErrorTypeSymbol(DiagnosticInfo errorInfo, string name, bool reportErrorWhenReferenced = false, NamedTypeSymbol nonErrorGuessType = null)
			: this(errorInfo, name, 0, reportErrorWhenReferenced, nonErrorGuessType)
		{
		}

		internal ExtendedErrorTypeSymbol(DiagnosticInfo errorInfo, string name, int arity, ImmutableArray<Symbol> candidateSymbols, LookupResultKind resultKind, bool reportErrorWhenReferenced = false)
			: base(arity)
		{
			_name = name;
			_diagnosticInfo = errorInfo;
			_reportErrorWhenReferenced = reportErrorWhenReferenced;
			if (candidateSymbols.Length == 1 && candidateSymbols[0].Kind == SymbolKind.Namespace && ((NamespaceSymbol)candidateSymbols[0]).NamespaceKind == (NamespaceKind)0)
			{
				_candidateSymbols = StaticCast<Symbol>.From(((NamespaceSymbol)candidateSymbols[0]).ConstituentNamespaces);
			}
			else
			{
				_candidateSymbols = candidateSymbols;
			}
			_resultKind = resultKind;
		}

		internal ExtendedErrorTypeSymbol(DiagnosticInfo errorInfo, string name, int arity, bool reportErrorWhenReferenced = false, NamedTypeSymbol nonErrorGuessType = null)
			: base(arity)
		{
			_name = name;
			_diagnosticInfo = errorInfo;
			_reportErrorWhenReferenced = reportErrorWhenReferenced;
			if ((object)nonErrorGuessType != null)
			{
				_candidateSymbols = ImmutableArray.Create((Symbol)nonErrorGuessType);
				_resultKind = LookupResultKind.NotATypeOrNamespace;
			}
			else
			{
				_candidateSymbols = ImmutableArray<Symbol>.Empty;
				_resultKind = LookupResultKind.Empty;
			}
		}

		internal ExtendedErrorTypeSymbol(NamespaceOrTypeSymbol containingSymbol, string name, int arity)
			: this(null, name, arity, reportErrorWhenReferenced: false, null)
		{
			_containingSymbol = containingSymbol;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return (!_reportErrorWhenReferenced) ? default(UseSiteInfo<AssemblySymbol>) : new UseSiteInfo<AssemblySymbol>(ErrorInfo);
		}

		protected override bool SpecializedEquals(InstanceErrorTypeSymbol obj)
		{
			if (!(obj is ExtendedErrorTypeSymbol extendedErrorTypeSymbol))
			{
				return false;
			}
			return object.Equals(ContainingSymbol, extendedErrorTypeSymbol.ContainingSymbol) && string.Equals(Name, extendedErrorTypeSymbol.Name, StringComparison.Ordinal) && base.Arity == extendedErrorTypeSymbol.Arity;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(base.Arity, Hash.Combine(((object)ContainingSymbol != null) ? ContainingSymbol.GetHashCode() : 0, (Name != null) ? Name.GetHashCode() : 0));
		}
	}
}
