using System.Diagnostics;

using Microsoft.Cci;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public sealed class MetadataConstant : IMetadataExpression
    {
        public ITypeReference Type { get; }

        public object? Value { get; }

        public MetadataConstant(ITypeReference type, object? value)
        {
            Type = type;
            Value = value;
        }

        void IMetadataExpression.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        [Conditional("DEBUG")]
        internal static void AssertValidConstant(object? value)
        {
        }
    }
}
