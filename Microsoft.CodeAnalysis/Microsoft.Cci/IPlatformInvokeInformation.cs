using System.Reflection;

#nullable enable

#nullable enable

namespace Microsoft.Cci
{
    public interface IPlatformInvokeInformation
    {
        string? ModuleName { get; }

        string? EntryPointName { get; }

        MethodImportAttributes Flags { get; }
    }
}
