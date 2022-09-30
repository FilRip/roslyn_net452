using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IPropertyDefinition : ISignature, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        MetadataConstant? DefaultValue { get; }

        IMethodReference? Getter { get; }

        bool HasDefaultValue { get; }

        bool IsRuntimeSpecial { get; }

        bool IsSpecialName { get; }

        ImmutableArray<IParameterDefinition> Parameters { get; }

        IMethodReference? Setter { get; }

        IEnumerable<IMethodReference> GetAccessors(EmitContext context);
    }
}
