﻿using System;
using System.Globalization;

namespace SystemExtensions
{
    public abstract class FormattableString : IFormattable
    {
        public abstract string Format { get; }

        public abstract int ArgumentCount { get; }

        public abstract object[] GetArguments();

        public abstract object GetArgument(int index);

        public abstract string ToString(IFormatProvider formatProvider);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(formatProvider);
        }

        public static string Invariant(string formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException("formattable");
            }
            return formattable.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }
    }
}
