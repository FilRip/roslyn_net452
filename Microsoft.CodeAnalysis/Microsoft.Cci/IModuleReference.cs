using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface IModuleReference : IUnitReference, IReference, INamedEntity
    {
        IAssemblyReference GetContainingAssembly(EmitContext context);
    }
}
