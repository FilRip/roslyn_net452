using Microsoft.Cci;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public sealed class MetadataNamedArgument : IMetadataNamedArgument, IMetadataExpression
    {
        private readonly ISymbolInternal _entity;

        private readonly ITypeReference _type;

        private readonly IMetadataExpression _value;

        string IMetadataNamedArgument.ArgumentName => _entity.Name;

        IMetadataExpression IMetadataNamedArgument.ArgumentValue => _value;

        bool IMetadataNamedArgument.IsField => _entity.Kind == SymbolKind.Field;

        ITypeReference IMetadataExpression.Type => _type;

        public MetadataNamedArgument(ISymbolInternal entity, ITypeReference type, IMetadataExpression value)
        {
            _entity = entity;
            _type = type;
            _value = value;
        }

        void IMetadataExpression.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
