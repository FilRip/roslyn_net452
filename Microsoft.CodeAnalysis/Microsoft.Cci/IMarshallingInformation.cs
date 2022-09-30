using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface IMarshallingInformation
    {
        string CustomMarshallerRuntimeArgument { get; }

        UnmanagedType ElementType { get; }

        int IidParameterIndex { get; }

        UnmanagedType UnmanagedType { get; }

        int NumberOfElements { get; }

        short ParamIndex { get; }

        VarEnum SafeArrayElementSubtype { get; }

        object GetCustomMarshaller(EmitContext context);

        ITypeReference GetSafeArrayElementUserDefinedSubtype(EmitContext context);
    }
}
