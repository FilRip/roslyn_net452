using System.Collections.Generic;
using System.Diagnostics;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MemberSignatureComparer : IEqualityComparer<Symbol>
	{
		public static readonly MemberSignatureComparer WinRTComparer = new MemberSignatureComparer(MethodSignatureComparer.WinRTConflictComparer, PropertySignatureComparer.WinRTConflictComparer, EventSignatureComparer.WinRTConflictComparer);

		private readonly MethodSignatureComparer _methodComparer;

		private readonly PropertySignatureComparer _propertyComparer;

		private readonly EventSignatureComparer _eventComparer;

		private MemberSignatureComparer(MethodSignatureComparer methodComparer, PropertySignatureComparer propertyComparer, EventSignatureComparer eventComparer)
		{
			_methodComparer = methodComparer;
			_propertyComparer = propertyComparer;
			_eventComparer = eventComparer;
		}

		public bool Equals(Symbol sym1, Symbol sym2)
		{
			if (sym1.Kind != sym2.Kind)
			{
				return false;
			}
			return sym1.Kind switch
			{
				SymbolKind.Method => _methodComparer.Equals((MethodSymbol)sym1, (MethodSymbol)sym2), 
				SymbolKind.Property => _propertyComparer.Equals((PropertySymbol)sym1, (PropertySymbol)sym2), 
				SymbolKind.Event => _eventComparer.Equals((EventSymbol)sym1, (EventSymbol)sym2), 
				_ => false, 
			};
		}

		bool IEqualityComparer<Symbol>.Equals(Symbol sym1, Symbol sym2)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(sym1, sym2);
		}

		public int GetHashCode(Symbol sym)
		{
			return sym.Kind switch
			{
				SymbolKind.Method => _methodComparer.GetHashCode((MethodSymbol)sym), 
				SymbolKind.Property => _propertyComparer.GetHashCode((PropertySymbol)sym), 
				SymbolKind.Event => _eventComparer.GetHashCode((EventSymbol)sym), 
				_ => throw ExceptionUtilities.UnexpectedValue(sym.Kind), 
			};
		}

		int IEqualityComparer<Symbol>.GetHashCode(Symbol sym)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
			return this.GetHashCode(sym);
		}

		[Conditional("DEBUG")]
		private void CheckSymbolKind(Symbol sym)
		{
			SymbolKind kind = sym.Kind;
			if (kind != SymbolKind.Event && kind != SymbolKind.Method)
			{
				_ = 15;
			}
		}
	}
}
