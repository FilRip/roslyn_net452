// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// This is an implementation of a special symbol comparer, which is supposed to be used for sorting
    /// original definition symbols (explicitly or implicitly declared in source within the same compilation)
    /// in lexical order of their declarations. It will not work on anything that uses non-source locations.
    /// </summary>
    internal class LexicalOrderSymbolComparer : IComparer<Symbol>
    {
        public static readonly LexicalOrderSymbolComparer Instance = new();

        private LexicalOrderSymbolComparer()
        {
        }

        public int Compare(Symbol x, Symbol y)
        {
            int comparison;
            if (x == y)
            {
                return 0;
            }

            var xSortKey = x.GetLexicalSortKey();
            var ySortKey = y.GetLexicalSortKey();

            comparison = LexicalSortKey.Compare(xSortKey, ySortKey);
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = x.Kind.ToSortOrder() - y.Kind.ToSortOrder();
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = string.CompareOrdinal(x.Name, y.Name);
            return comparison;
        }
    }
}
