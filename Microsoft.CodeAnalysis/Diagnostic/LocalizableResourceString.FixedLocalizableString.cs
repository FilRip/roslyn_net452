// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract partial class LocalizableString
    {
        private sealed class FixedLocalizableString : LocalizableString
        {
            /// <summary>
            /// FixedLocalizableString representing an empty string.
            /// </summary>
            private static readonly FixedLocalizableString s_empty = new(string.Empty);

            private readonly string _fixedString;

            public static FixedLocalizableString Create(string? fixedResource)
            {
                if (RoslynString.IsNullOrEmpty(fixedResource))
                {
                    return s_empty;
                }

                return new FixedLocalizableString(fixedResource);
            }

            private FixedLocalizableString(string fixedResource)
            {
                _fixedString = fixedResource;
            }

            protected override string GetText(IFormatProvider? formatProvider)
            {
                return _fixedString;
            }

            protected override bool AreEqual(object? other)
            {
                return other is FixedLocalizableString fixedStr && string.Equals(_fixedString, fixedStr._fixedString);
            }

            protected override int GetHash()
            {
                return _fixedString?.GetHashCode() ?? 0;
            }

            internal override bool CanThrowExceptions => false;
        }
    }
}
