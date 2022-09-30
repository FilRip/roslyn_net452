using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class ParameterTypeInformation : IParameterTypeInformation, IParameterListEntry
    {
        private readonly ParameterSymbol _underlyingParameter;

        ImmutableArray<ICustomModifier> IParameterTypeInformation.CustomModifiers => ImmutableArray<ICustomModifier>.CastUp(_underlyingParameter.TypeWithAnnotations.CustomModifiers);

        bool IParameterTypeInformation.IsByReference => _underlyingParameter.RefKind != RefKind.None;

        ImmutableArray<ICustomModifier> IParameterTypeInformation.RefCustomModifiers => ImmutableArray<ICustomModifier>.CastUp(_underlyingParameter.RefCustomModifiers);

        ushort IParameterListEntry.Index => (ushort)_underlyingParameter.Ordinal;

        public ParameterTypeInformation(ParameterSymbol underlyingParameter)
        {
            _underlyingParameter = underlyingParameter;
        }

        ITypeReference IParameterTypeInformation.GetType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(_underlyingParameter.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        public override string ToString()
        {
            return _underlyingParameter.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat);
        }

        public sealed override bool Equals(object obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
