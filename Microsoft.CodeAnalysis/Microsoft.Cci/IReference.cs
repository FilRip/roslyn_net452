using System.Collections.Generic;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.Cci
{
    public interface IReference
    {
        IEnumerable<ICustomAttribute> GetAttributes(EmitContext context);

        void Dispatch(MetadataVisitor visitor);

        IDefinition? AsDefinition(EmitContext context);

        ISymbolInternal? GetInternalSymbol();
    }
}
