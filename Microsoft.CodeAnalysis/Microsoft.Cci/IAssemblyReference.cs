using System;

using Microsoft.CodeAnalysis;

#nullable enable

namespace Microsoft.Cci
{
    public interface IAssemblyReference : IModuleReference, IUnitReference, IReference, INamedEntity
    {
        AssemblyIdentity Identity { get; }

        Version? AssemblyVersionPattern { get; }
    }
}
