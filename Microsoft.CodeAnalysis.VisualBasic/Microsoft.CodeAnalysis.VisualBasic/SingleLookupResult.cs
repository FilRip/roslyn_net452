using System;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct SingleLookupResult
	{
		internal readonly LookupResultKind Kind;

		internal readonly Symbol Symbol;

		internal readonly DiagnosticInfo Diagnostic;

		public static readonly SingleLookupResult Empty = new SingleLookupResult(LookupResultKind.Empty, null, null);

		public static readonly SingleLookupResult EmptyAndStopLookup = new SingleLookupResult(LookupResultKind.EmptyAndStopLookup, null, null);

		public bool HasDiagnostic => Diagnostic != null;

		public bool StopFurtherLookup => Kind >= LookupResultKind.WrongArityAndStopLookup;

		public bool IsGoodOrAmbiguous
		{
			get
			{
				if (Kind != LookupResultKind.Good)
				{
					return Kind == LookupResultKind.Ambiguous;
				}
				return true;
			}
		}

		public bool IsGood => Kind == LookupResultKind.Good;

		public bool IsAmbiguous => Kind == LookupResultKind.Ambiguous;

		internal SingleLookupResult(LookupResultKind kind, Symbol symbol, DiagnosticInfo diagInfo)
		{
			this = default(SingleLookupResult);
			Kind = kind;
			Symbol = symbol;
			Diagnostic = diagInfo;
		}

		public static SingleLookupResult Good(Symbol sym)
		{
			return new SingleLookupResult(LookupResultKind.Good, sym, null);
		}

		public static SingleLookupResult Ambiguous(ImmutableArray<Symbol> syms, Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> generateAmbiguityDiagnostic)
		{
			DiagnosticInfo diagInfo = generateAmbiguityDiagnostic(syms);
			return new SingleLookupResult(LookupResultKind.Ambiguous, syms.First(), diagInfo);
		}

		public static SingleLookupResult WrongArityAndStopLookup(Symbol sym, ERRID err)
		{
			return new SingleLookupResult(LookupResultKind.WrongArityAndStopLookup, sym, new BadSymbolDiagnostic(sym, err));
		}

		public static SingleLookupResult WrongArityAndStopLookup(Symbol sym, DiagnosticInfo diagInfo)
		{
			return new SingleLookupResult(LookupResultKind.WrongArityAndStopLookup, sym, diagInfo);
		}

		public static SingleLookupResult WrongArity(Symbol sym, DiagnosticInfo diagInfo)
		{
			return new SingleLookupResult(LookupResultKind.WrongArity, sym, diagInfo);
		}

		public static SingleLookupResult WrongArity(Symbol sym, ERRID err)
		{
			return new SingleLookupResult(LookupResultKind.WrongArity, sym, new BadSymbolDiagnostic(sym, err));
		}

		public static SingleLookupResult MustNotBeInstance(Symbol sym, ERRID err)
		{
			return new SingleLookupResult(LookupResultKind.MustNotBeInstance, sym, new BadSymbolDiagnostic(sym, err));
		}

		public static SingleLookupResult MustBeInstance(Symbol sym)
		{
			return new SingleLookupResult(LookupResultKind.MustBeInstance, sym, null);
		}

		public static SingleLookupResult Inaccessible(Symbol sym, DiagnosticInfo diagInfo)
		{
			return new SingleLookupResult(LookupResultKind.Inaccessible, sym, diagInfo);
		}

		internal static SingleLookupResult NotAnAttributeType(Symbol sym, DiagnosticInfo error)
		{
			return new SingleLookupResult(LookupResultKind.NotAnAttributeType, sym, error);
		}
	}
}
