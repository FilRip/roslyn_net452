using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceMethodSymbol : MethodSymbol
    {
        protected bool AreContainingSymbolLocalsZeroed
        {
            get
            {
                if (ContainingSymbol is SourceMethodSymbol sourceMethodSymbol)
                {
                    return sourceMethodSymbol.AreLocalsZeroed;
                }
                if (ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
                {
                    return sourceMemberContainerTypeSymbol.AreLocalsZeroed;
                }
                return true;
            }
        }

        public abstract ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes();

        public abstract ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds();

        protected static void ReportBadRefToken(TypeSyntax returnTypeSyntax, BindingDiagnosticBag diagnostics)
        {
            if (!returnTypeSyntax.HasErrors)
            {
                SyntaxToken firstToken = returnTypeSyntax.GetFirstToken();
                diagnostics.Add(ErrorCode.ERR_UnexpectedToken, firstToken.GetLocation(), firstToken.ToString());
            }
        }

        internal void ReportAsyncParameterErrors(BindingDiagnosticBag diagnostics, Location location)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.RefKind != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_BadAsyncArgType, getLocation(current, location));
                }
                else if (current.Type.IsUnsafe())
                {
                    diagnostics.Add(ErrorCode.ERR_UnsafeAsyncArgType, getLocation(current, location));
                }
                else if (current.Type.IsRestrictedType())
                {
                    diagnostics.Add(ErrorCode.ERR_BadSpecialByRefLocal, getLocation(current, location), current.Type);
                }
            }
            static Location getLocation(ParameterSymbol parameter, Location location)
            {
                return parameter.Locations.FirstOrDefault() ?? location;
            }
        }
    }
}
