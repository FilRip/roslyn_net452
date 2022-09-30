using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct MethodGroupResolution
    {
        public readonly MethodGroup MethodGroup;

        public readonly Symbol OtherSymbol;

        public readonly OverloadResolutionResult<MethodSymbol> OverloadResolutionResult;

        public readonly AnalyzedArguments AnalyzedArguments;

        public readonly ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics;

        public readonly LookupResultKind ResultKind;

        public bool IsEmpty
        {
            get
            {
                if (MethodGroup == null)
                {
                    return (object)OtherSymbol == null;
                }
                return false;
            }
        }

        public bool HasAnyErrors => Diagnostics.Diagnostics.HasAnyErrors();

        public bool HasAnyApplicableMethod
        {
            get
            {
                if (MethodGroup != null && ResultKind == LookupResultKind.Viable)
                {
                    if (OverloadResolutionResult != null)
                    {
                        return OverloadResolutionResult.HasAnyApplicableMember;
                    }
                    return true;
                }
                return false;
            }
        }

        public bool IsExtensionMethodGroup
        {
            get
            {
                if (MethodGroup != null)
                {
                    return MethodGroup.IsExtensionMethodGroup;
                }
                return false;
            }
        }

        public bool IsLocalFunctionInvocation
        {
            get
            {
                MethodGroup methodGroup = MethodGroup;
                if (methodGroup != null && methodGroup.Methods.Count == 1)
                {
                    return MethodGroup.Methods[0].MethodKind == MethodKind.LocalFunction;
                }
                return false;
            }
        }

        public MethodGroupResolution(MethodGroup methodGroup, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
            : this(methodGroup, null, null, null, methodGroup.ResultKind, diagnostics)
        {
        }

        public MethodGroupResolution(Symbol otherSymbol, LookupResultKind resultKind, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
            : this(null, otherSymbol, null, null, resultKind, diagnostics)
        {
        }

        public MethodGroupResolution(MethodGroup methodGroup, Symbol otherSymbol, OverloadResolutionResult<MethodSymbol> overloadResolutionResult, AnalyzedArguments analyzedArguments, LookupResultKind resultKind, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
        {
            MethodGroup = methodGroup;
            OtherSymbol = otherSymbol;
            OverloadResolutionResult = overloadResolutionResult;
            AnalyzedArguments = analyzedArguments;
            ResultKind = resultKind;
            Diagnostics = diagnostics;
        }

        public void Free()
        {
            AnalyzedArguments?.Free();
            MethodGroup?.Free();
            OverloadResolutionResult?.Free();
        }
    }
}
