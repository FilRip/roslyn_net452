// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// Packages up the various parts returned when resolving a method group. 
    /// </summary>
    public struct MethodGroupResolution
    {
        public readonly MethodGroup MethodGroup;
        public readonly Symbol OtherSymbol;
        public readonly OverloadResolutionResult<MethodSymbol> OverloadResolutionResult;
        public readonly AnalyzedArguments AnalyzedArguments;
        public readonly ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics;
        public readonly LookupResultKind ResultKind;

        public MethodGroupResolution(MethodGroup methodGroup, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
            : this(methodGroup, otherSymbol: null, overloadResolutionResult: null, analyzedArguments: null, methodGroup.ResultKind, diagnostics)
        {
        }

        public MethodGroupResolution(Symbol otherSymbol, LookupResultKind resultKind, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
            : this(methodGroup: null, otherSymbol, overloadResolutionResult: null, analyzedArguments: null, resultKind, diagnostics)
        {
        }

        public MethodGroupResolution(
            MethodGroup methodGroup,
            Symbol otherSymbol,
            OverloadResolutionResult<MethodSymbol> overloadResolutionResult,
            AnalyzedArguments analyzedArguments,
            LookupResultKind resultKind,
            ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
        {
            // Methods should be represented in the method group.

            this.MethodGroup = methodGroup;
            this.OtherSymbol = otherSymbol;
            this.OverloadResolutionResult = overloadResolutionResult;
            this.AnalyzedArguments = analyzedArguments;
            this.ResultKind = resultKind;
            this.Diagnostics = diagnostics;
        }

        public bool IsEmpty
        {
            get { return (this.MethodGroup == null) && (this.OtherSymbol is null); }
        }

        public bool HasAnyErrors
        {
            get { return this.Diagnostics.Diagnostics.HasAnyErrors(); }
        }

        public bool HasAnyApplicableMethod
        {
            get
            {
                return (this.MethodGroup != null) &&
                    (this.ResultKind == LookupResultKind.Viable) &&
                    ((this.OverloadResolutionResult == null) || this.OverloadResolutionResult.HasAnyApplicableMember);
            }
        }

        public bool IsExtensionMethodGroup
        {
            get { return (this.MethodGroup != null) && this.MethodGroup.IsExtensionMethodGroup; }
        }

        public bool IsLocalFunctionInvocation =>
            MethodGroup?.Methods.Count == 1 && // Local functions cannot be overloaded
            MethodGroup.Methods[0].MethodKind == MethodKind.LocalFunction;

        public void Free()
        {
            this.AnalyzedArguments?.Free();
            this.MethodGroup?.Free();
            this.OverloadResolutionResult?.Free();
        }
    }
}
