using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface ISignature
    {
        CallingConvention CallingConvention { get; }

        ushort ParameterCount { get; }

        ImmutableArray<ICustomModifier> ReturnValueCustomModifiers { get; }

        ImmutableArray<ICustomModifier> RefCustomModifiers { get; }

        bool ReturnValueIsByRef { get; }

        ImmutableArray<IParameterTypeInformation> GetParameters(EmitContext context);

        ITypeReference GetType(EmitContext context);
    }
}
