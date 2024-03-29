// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class Binder
    {
        /// <summary>
        /// True if we are currently in an unsafe region (type, member, or block).
        /// </summary>
        /// <remarks>
        /// Does not imply that this compilation allows unsafe regions (could be in an error recovery scenario).
        /// To determine that, check this.Compilation.Options.AllowUnsafe.
        /// </remarks>
        internal bool InUnsafeRegion
        {
            get { return this.Flags.Includes(EBinder.UnsafeRegion); }
        }

        /// <returns>True if a diagnostic was reported</returns>
        internal bool ReportUnsafeIfNotAllowed(SyntaxNode node, BindingDiagnosticBag diagnostics, TypeSymbol sizeOfTypeOpt = null)
        {
            var diagnosticInfo = GetUnsafeDiagnosticInfo(sizeOfTypeOpt);
            if (diagnosticInfo == null)
            {
                return false;
            }

            diagnostics.Add(new CSDiagnostic(diagnosticInfo, node.Location));
            return true;
        }

        /// <returns>True if a diagnostic was reported</returns>
        internal bool ReportUnsafeIfNotAllowed(Location location, BindingDiagnosticBag diagnostics)
        {
            var diagnosticInfo = GetUnsafeDiagnosticInfo(sizeOfTypeOpt: null);
            if (diagnosticInfo == null)
            {
                return false;
            }

            diagnostics.Add(new CSDiagnostic(diagnosticInfo, location));
            return true;
        }

        private CSDiagnosticInfo GetUnsafeDiagnosticInfo(TypeSymbol sizeOfTypeOpt)
        {
            if (this.Flags.Includes(EBinder.SuppressUnsafeDiagnostics))
            {
                return null;
            }
            else if (this.IsIndirectlyInIterator)
            {
                // Spec 8.2: "An iterator block always defines a safe context, even when its declaration
                // is nested in an unsafe context."
                return new CSDiagnosticInfo(ErrorCode.ERR_IllegalInnerUnsafe);
            }
            else if (!this.InUnsafeRegion)
            {
                return (sizeOfTypeOpt is null)
                    ? new CSDiagnosticInfo(ErrorCode.ERR_UnsafeNeeded)
                    : new CSDiagnosticInfo(ErrorCode.ERR_SizeofUnsafe, sizeOfTypeOpt);
            }
            else
            {
                return null;
            }
        }
    }
}
