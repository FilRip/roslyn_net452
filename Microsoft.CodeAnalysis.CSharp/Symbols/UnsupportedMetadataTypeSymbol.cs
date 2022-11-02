// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class UnsupportedMetadataTypeSymbol : ErrorTypeSymbol
    {
#pragma warning disable IDE0052
        private readonly BadImageFormatException? _mrEx;
#pragma warning restore IDE0052

        internal UnsupportedMetadataTypeSymbol(BadImageFormatException? mrEx = null)
        {
            _mrEx = mrEx;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return this;
        }

        internal override DiagnosticInfo ErrorInfo
        {
            get
            {
                return new CSDiagnosticInfo(ErrorCode.ERR_BogusType, string.Empty);
            }
        }

        internal override bool MangleName
        {
            get
            {
                return false;
            }
        }
    }
}
