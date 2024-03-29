// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Common base for ordinary methods overriding methods from object synthesized by compiler for records.
    /// </summary>
    internal abstract class SynthesizedRecordObjectMethod : SynthesizedRecordOrdinaryMethod
    {
        protected SynthesizedRecordObjectMethod(SourceMemberContainerTypeSymbol containingType, string name, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, name, hasBody: true, memberOffset, diagnostics)
        {
        }

        protected sealed override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            const DeclarationModifiers result = DeclarationModifiers.Public | DeclarationModifiers.Override;
            return result;
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            base.MethodChecks(diagnostics);
            VerifyOverridesMethodFromObject(this, OverriddenSpecialMember, diagnostics);
        }

        protected abstract SpecialMember OverriddenSpecialMember { get; }

        /// <summary>
        /// Returns true if reported an error
        /// </summary>
        internal static bool VerifyOverridesMethodFromObject(MethodSymbol overriding, SpecialMember overriddenSpecialMember, BindingDiagnosticBag diagnostics)
        {
            bool reportAnError = false;

            if (!overriding.IsOverride)
            {
                reportAnError = true;
            }
            else
            {
                var overridden = overriding.OverriddenMethod?.OriginalDefinition;

                if (overridden is not null && !(overridden.ContainingType is SourceMemberContainerTypeSymbol { IsRecord: true } && overridden.ContainingModule == overriding.ContainingModule))
                {
                    MethodSymbol leastOverridden = overriding.GetLeastOverriddenMethod(accessingTypeOpt: null);

                    reportAnError = (object)leastOverridden != overriding.ContainingAssembly.GetSpecialTypeMember(overriddenSpecialMember) &&
                                    leastOverridden.ReturnType.Equals(overriding.ReturnType, TypeCompareKind.AllIgnoreOptions);
                }
            }

            if (reportAnError)
            {
                diagnostics.Add(ErrorCode.ERR_DoesNotOverrideMethodFromObject, overriding.Locations[0], overriding);
            }

            return reportAnError;
        }
    }
}
