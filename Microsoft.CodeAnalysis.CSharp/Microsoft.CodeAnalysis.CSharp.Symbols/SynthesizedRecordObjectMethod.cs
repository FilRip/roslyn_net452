namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedRecordObjectMethod : SynthesizedRecordOrdinaryMethod
    {
        protected abstract SpecialMember OverriddenSpecialMember { get; }

        protected SynthesizedRecordObjectMethod(SourceMemberContainerTypeSymbol containingType, string name, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, name, hasBody: true, memberOffset, diagnostics)
        {
        }

        protected sealed override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            return DeclarationModifiers.Public | DeclarationModifiers.Override;
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            base.MethodChecks(diagnostics);
            VerifyOverridesMethodFromObject(this, OverriddenSpecialMember, diagnostics);
        }

        internal static bool VerifyOverridesMethodFromObject(MethodSymbol overriding, SpecialMember overriddenSpecialMember, BindingDiagnosticBag diagnostics)
        {
            bool flag = false;
            if (!overriding.IsOverride)
            {
                flag = true;
            }
            else
            {
                MethodSymbol methodSymbol = overriding.OverriddenMethod?.OriginalDefinition;
                if ((object)methodSymbol != null && (!(methodSymbol.ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol) || !sourceMemberContainerTypeSymbol.IsRecord || !(methodSymbol.ContainingModule == overriding.ContainingModule)))
                {
                    MethodSymbol leastOverriddenMethod = overriding.GetLeastOverriddenMethod(null);
                    flag = (object)leastOverriddenMethod != overriding.ContainingAssembly.GetSpecialTypeMember(overriddenSpecialMember) && leastOverriddenMethod.ReturnType.Equals(overriding.ReturnType, TypeCompareKind.AllIgnoreOptions);
                }
            }
            if (flag)
            {
                diagnostics.Add(ErrorCode.ERR_DoesNotOverrideMethodFromObject, overriding.Locations[0], overriding);
            }
            return flag;
        }
    }
}
