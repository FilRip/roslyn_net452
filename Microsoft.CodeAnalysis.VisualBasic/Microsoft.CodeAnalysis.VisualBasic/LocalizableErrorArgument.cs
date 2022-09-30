using System;
using System.Globalization;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct LocalizableErrorArgument : IFormattable
	{
		private readonly ERRID _id;

		internal LocalizableErrorArgument(ERRID id)
		{
			this = default(LocalizableErrorArgument);
			_id = id;
		}

		public override string ToString()
		{
			return ToString_IFormattable(null, null);
		}

		public string ToString_IFormattable(string format, IFormatProvider formatProvider)
		{
			return ErrorFactory.IdToString(_id, (CultureInfo)formatProvider);
		}

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ToString_IFormattable
			return this.ToString_IFormattable(format, formatProvider);
		}
	}
}
