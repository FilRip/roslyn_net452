using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public static class Extensions
    {
        public static bool HasBody(this IMethodDefinition methodDef)
        {
            if (!methodDef.IsAbstract && !methodDef.IsExternal)
            {
                if (methodDef.ContainingTypeDefinition != null)
                {
                    return !methodDef.ContainingTypeDefinition.IsComObject;
                }
                return true;
            }
            return false;
        }

        public static bool ShouldInclude(this ITypeDefinitionMember member, EmitContext context)
        {
            if (context.IncludePrivateMembers)
            {
                return true;
            }
            if (member is IMethodDefinition methodDefinition && methodDefinition.IsVirtual)
            {
                return true;
            }
            switch (member.Visibility)
            {
                case TypeMemberVisibility.Private:
                    return context.IncludePrivateMembers;
                case TypeMemberVisibility.FamilyAndAssembly:
                case TypeMemberVisibility.Assembly:
                    if (!context.IncludePrivateMembers)
                    {
                        return context.Module.SourceAssemblyOpt?.InternalsAreVisible ?? false;
                    }
                    return true;
                default:
                    return true;
            }
        }
    }
}
