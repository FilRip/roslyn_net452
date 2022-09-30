using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.Emit
{
    public interface IPEDeltaAssemblyBuilder
    {
        void OnCreatedIndices(DiagnosticBag diagnostics);

        IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> GetAnonymousTypeMap();
    }
}
