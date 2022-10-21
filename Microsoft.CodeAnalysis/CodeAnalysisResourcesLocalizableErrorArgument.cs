// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct CodeAnalysisResourcesLocalizableErrorArgument : IFormattable
    {
        private readonly string _targetResourceId;

        public CodeAnalysisResourcesLocalizableErrorArgument(string targetResourceId)
        {
            RoslynDebug.Assert(targetResourceId != null);
            _targetResourceId = targetResourceId;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (_targetResourceId != null)
            {
                return Properties.Resources.ResourceManager.GetString(_targetResourceId, formatProvider as System.Globalization.CultureInfo) ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
