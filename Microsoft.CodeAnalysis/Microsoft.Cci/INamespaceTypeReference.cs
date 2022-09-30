using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface INamespaceTypeReference : INamedTypeReference, ITypeReference, IReference, INamedEntity
    {
        string NamespaceName { get; }

        IUnitReference GetUnit(EmitContext context);
    }
}
