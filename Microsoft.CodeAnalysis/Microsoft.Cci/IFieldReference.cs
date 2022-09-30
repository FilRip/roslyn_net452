using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IFieldReference : ITypeMemberReference, IReference, INamedEntity
    {
        ISpecializedFieldReference? AsSpecializedFieldReference { get; }

        bool IsContextualNamedEntity { get; }

        ITypeReference GetType(EmitContext context);

        IFieldDefinition? GetResolvedField(EmitContext context);
    }
}
