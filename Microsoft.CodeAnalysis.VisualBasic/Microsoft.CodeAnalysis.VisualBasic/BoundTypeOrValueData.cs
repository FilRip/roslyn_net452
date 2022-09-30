using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct BoundTypeOrValueData : IEquatable<BoundTypeOrValueData>
	{
		private readonly BoundExpression _valueExpression;

		private readonly BindingDiagnosticBag _valueDiagnostics;

		private readonly BoundExpression _typeExpression;

		private readonly BindingDiagnosticBag _typeDiagnostics;

		public BoundExpression ValueExpression => _valueExpression;

		public BindingDiagnosticBag ValueDiagnostics => _valueDiagnostics;

		public BoundExpression TypeExpression => _typeExpression;

		public BindingDiagnosticBag TypeDiagnostics => _typeDiagnostics;

		public BoundTypeOrValueData(BoundExpression valueExpression, BindingDiagnosticBag valueDiagnostics, BoundExpression typeExpression, BindingDiagnosticBag typeDiagnostics)
		{
			this = default(BoundTypeOrValueData);
			_valueExpression = valueExpression;
			_valueDiagnostics = valueDiagnostics;
			_typeExpression = typeExpression;
			_typeDiagnostics = typeDiagnostics;
		}

		public static bool operator ==(BoundTypeOrValueData a, BoundTypeOrValueData b)
		{
			if (a.ValueExpression == b.ValueExpression && a.ValueDiagnostics == b.ValueDiagnostics && a.TypeExpression == b.TypeExpression)
			{
				return a.TypeDiagnostics == b.TypeDiagnostics;
			}
			return false;
		}

		public static bool operator !=(BoundTypeOrValueData a, BoundTypeOrValueData b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is BoundTypeOrValueData)
			{
				return (BoundTypeOrValueData)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(ValueExpression.GetHashCode(), Hash.Combine(ValueDiagnostics.GetHashCode(), Hash.Combine(TypeExpression.GetHashCode(), TypeDiagnostics.GetHashCode())));
		}

		private bool Equals(BoundTypeOrValueData b)
		{
			return b == this;
		}

		bool IEquatable<BoundTypeOrValueData>.Equals(BoundTypeOrValueData b)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(b);
		}
	}
}
