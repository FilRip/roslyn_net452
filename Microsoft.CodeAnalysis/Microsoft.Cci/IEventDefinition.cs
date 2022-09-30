using System.Collections.Generic;

using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IEventDefinition : ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        IMethodReference Adder { get; }

        IMethodReference? Caller { get; }

        bool IsRuntimeSpecial { get; }

        bool IsSpecialName { get; }

        IMethodReference Remover { get; }

        IEnumerable<IMethodReference> GetAccessors(EmitContext context);

        ITypeReference GetType(EmitContext context);
    }
}
