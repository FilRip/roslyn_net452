using System.Collections.Generic;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class EventSignatureComparer : IEqualityComparer<EventSymbol>
	{
		public static readonly EventSignatureComparer ExplicitEventImplementationComparer = new EventSignatureComparer(considerName: false, considerType: false, considerCustomModifiers: false, considerTupleNames: false);

		public static readonly EventSignatureComparer ExplicitEventImplementationWithTupleNamesComparer = new EventSignatureComparer(considerName: false, considerType: false, considerCustomModifiers: false, considerTupleNames: true);

		public static readonly EventSignatureComparer OverrideSignatureComparer = new EventSignatureComparer(considerName: true, considerType: false, considerCustomModifiers: false, considerTupleNames: false);

		public static readonly EventSignatureComparer RuntimeEventSignatureComparer = new EventSignatureComparer(considerName: true, considerType: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly EventSignatureComparer WinRTConflictComparer = new EventSignatureComparer(considerName: true, considerType: false, considerCustomModifiers: false, considerTupleNames: false);

		private readonly bool _considerName;

		private readonly bool _considerType;

		private readonly bool _considerCustomModifiers;

		private readonly bool _considerTupleNames;

		private EventSignatureComparer(bool considerName, bool considerType, bool considerCustomModifiers, bool considerTupleNames)
		{
			_considerName = considerName;
			_considerType = considerType;
			_considerCustomModifiers = considerCustomModifiers;
			_considerTupleNames = considerTupleNames;
		}

		public bool Equals(EventSymbol event1, EventSymbol event2)
		{
			if (event1 == event2)
			{
				return true;
			}
			if ((object)event1 == null || (object)event2 == null)
			{
				return false;
			}
			if (_considerName && !CaseInsensitiveComparison.Equals(event1.Name, event2.Name))
			{
				return false;
			}
			if (_considerType)
			{
				TypeCompareKind compareKind = MethodSignatureComparer.MakeTypeCompareKind(_considerCustomModifiers, _considerTupleNames);
				if (!TypeSymbolExtensions.IsSameType(event1.Type, event2.Type, compareKind))
				{
					return false;
				}
			}
			if ((event1.DelegateParameters.Length > 0 || event2.DelegateParameters.Length > 0) && !MethodSignatureComparer.HaveSameParameterTypes(event1.DelegateParameters, null, event2.DelegateParameters, null, considerByRef: true, _considerCustomModifiers, _considerTupleNames))
			{
				return false;
			}
			return true;
		}

		bool IEqualityComparer<EventSymbol>.Equals(EventSymbol event1, EventSymbol event2)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(event1, event2);
		}

		public int GetHashCode(EventSymbol @event)
		{
			int num = 1;
			if ((object)@event != null)
			{
				if (_considerName)
				{
					num = Hash.Combine(@event.Name, num);
				}
				if (_considerType && !_considerCustomModifiers)
				{
					num = Hash.Combine(@event.Type, num);
				}
				num = Hash.Combine(num, @event.DelegateParameters.Length);
			}
			return num;
		}

		int IEqualityComparer<EventSymbol>.GetHashCode(EventSymbol @event)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
			return this.GetHashCode(@event);
		}
	}
}
